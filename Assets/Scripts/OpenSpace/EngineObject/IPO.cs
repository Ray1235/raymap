﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.EngineObject {
    /// <summary>
    /// IPO = Instantiated Physical Object. Used for level geometry
    /// </summary>
    public class IPO : IEngineObject {
        public Pointer offset;
        public Pointer off_data;
        public Pointer off_radiosity;
        public PhysicalObject data;
        public string name = "";
        private GameObject gao;
        public GameObject Gao {
            get {
                if (gao == null) {
                    gao = new GameObject(name);
                }
                return gao;
            }
        }

        private SuperObject superObject;
        public SuperObject SuperObject {
            get {
                return superObject;
            }
        }

        public IPO(Pointer offset, SuperObject so) {
            this.offset = offset;
            this.superObject = so;
        }

        public static IPO Read(EndianBinaryReader reader, Pointer offset, SuperObject so) {
            MapLoader l = MapLoader.Loader;
            IPO ipo = new IPO(offset, so);
            ipo.off_data = Pointer.Read(reader);
            ipo.off_radiosity = Pointer.Read(reader);
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            ipo.name = "IPO";
            if (l.mode == MapLoader.Mode.Rayman3GC) ipo.name = new string(reader.ReadChars(0x32)).TrimEnd('\0');
            Pointer.Goto(ref reader, ipo.off_data);
            ipo.data = PhysicalObject.Read(reader, ipo.off_data);
            if (ipo.data != null) {
                ipo.data.Gao.transform.parent = ipo.Gao.transform;
            }
            /*if (ipo.data != null && ipo.data.visualSet.Count > 0) {
                if (ipo.data.visualSet[0].obj is R3Mesh) {
                    GameObject meshGAO = ((R3Mesh)ipo.data.visualSet[0].obj).gao;
                    meshGAO.transform.parent = ipo.Gao.transform;
                }
            }*/
            return ipo;
        }

    }
}
