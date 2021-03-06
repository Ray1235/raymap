﻿using OpenSpace.EngineObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class Script {
        public Pointer offset; // offset of the pointer to the script

        public Pointer off_script; // offset where the script starts
        public List<ScriptNode> scriptNodes = new List<ScriptNode>();

        public Script(Pointer offset) {
            this.offset = offset;
        }

        public static Script Read(EndianBinaryReader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            Script s = new Script(offset);
            
            s.off_script = Pointer.Read(reader);
            if (s.off_script != null) {
                Pointer off_current = Pointer.Goto(ref reader, s.off_script);
                bool endReached = false;
                while (!endReached) {
                    ScriptNode sn = ScriptNode.Read(reader, Pointer.Current(reader));
                    s.scriptNodes.Add(sn);

                    bool waypointRef = false;
                    if (l.mode == MapLoader.Mode.Rayman2PC) {
                        if (R2AIFunctionTypes.getNodeType(sn.type) == R2AIFunctionTypes.NodeType.WayPointRef) {
                            waypointRef = true;
                        }
                    } else if (l.mode == MapLoader.Mode.Rayman3PC) {
                        if (R3AIFunctionTypes.getNodeType(sn.type) == R3AIFunctionTypes.NodeType.WayPointRef) {
                            waypointRef = true;
                        }
                    }

                    if (waypointRef) {
                        Pointer off_wp = sn.param_ptr;
                        Pointer original = Pointer.Goto(ref reader, off_wp);
                        WayPoint waypoint = WayPoint.Read(reader, off_wp);

                        l.print("Waypoint at " + waypoint.position.x + ", " + waypoint.position.y + ", " + waypoint.position.z);

                        Pointer.Goto(ref reader, original);
                    }

                    if (sn.indent == 0) endReached = true;
                }
                Pointer.Goto(ref reader, off_current);
            }
            return s;
        }

        public void print(Perso perso) {
            // TODO: Use perso to print states, etc.
            MapLoader l = MapLoader.Loader;
            StringBuilder builder = new StringBuilder();
            builder.Append("Script @ offset: " + offset + "\n");
            foreach (ScriptNode sn in scriptNodes) {
                if (sn.indent == 0) {
                    builder.Append("---- END OF SCRIPT ----");
                } else {
                    builder.Append(new String(' ', (sn.indent - 1) * 4));
                    if (l.mode == MapLoader.Mode.Rayman2PC) {
                        builder.Append(R2AIFunctionTypes.readableFunctionSubType(sn, perso));
                    } else {
                        builder.Append(R3AIFunctionTypes.readableFunctionSubType(sn, perso));
                    }
                }
                builder.Append("\n");
            }
            l.print(builder.ToString());
        }
    }
}
