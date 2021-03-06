﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenSpace.FileFormat {
    public abstract class FileWithPointers : IDisposable {
        public string name = "Unknown";
        public EndianBinaryReader reader;
        public EndianBinaryWriter writer;
        public Dictionary<uint, Pointer> pointers = new Dictionary<uint, Pointer>();
        public int baseOffset;
        public uint headerOffset = 0;

        public void Dispose() {
            if (reader != null) reader.Close();
            if (writer != null) writer.Close();
        }

        public void AddPointer(uint offset, Pointer pointer) {
            pointers[offset] = pointer;
        }

        public void GotoHeader() {
            if (reader != null) {
                reader.BaseStream.Seek(headerOffset + baseOffset, SeekOrigin.Begin);
            }
        }
    }
}
