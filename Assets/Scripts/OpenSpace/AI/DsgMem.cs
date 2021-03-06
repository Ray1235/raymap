﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class DsgMem {
        public Pointer offset;

        public Pointer off_dsgVar;
        public Pointer memBufferInitial; // initial state
        public Pointer memBuffer; // current state

        public DsgVar dsgVar;

        public DsgMem(Pointer offset) {
            this.offset = offset;
        }

        public static DsgMem Read(EndianBinaryReader reader, Pointer offset) {
            DsgMem dsgMem = new DsgMem(offset);

            Pointer dsgVarPointer = Pointer.Read(reader);
            Pointer original = Pointer.Goto(ref reader, dsgVarPointer);
            dsgMem.off_dsgVar = Pointer.Read(reader);
            Pointer.Goto(ref reader, original);

            dsgMem.memBufferInitial = Pointer.Read(reader);
            dsgMem.memBuffer = Pointer.Read(reader);

            if (dsgMem.off_dsgVar != null) {
                Pointer.Goto(ref reader, dsgMem.off_dsgVar);
                dsgMem.dsgVar = DsgVar.Read(reader, dsgMem.off_dsgVar, dsgMem);
            }
            return dsgMem;
        }
    }
}
