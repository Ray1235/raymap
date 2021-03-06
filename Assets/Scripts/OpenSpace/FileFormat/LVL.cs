﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenSpace.FileFormat {
    public class LVL : FileWithPointers {
        public LVL(string name, string path) : this(name, File.OpenRead(path)) { }

        public LVL(string name, Stream stream) {
            baseOffset = 4;
            headerOffset = 0;
            this.name = name;
            reader = new EndianBinaryReader(stream, MapLoader.Loader.IsLittleEndian);
        }

        public void ReadPTR(string path) {
            MapLoader l = MapLoader.Loader;
            if (!File.Exists(path)) return;
            Stream ptrStream = File.OpenRead(path);
            long totalSize = ptrStream.Length;
            using (EndianBinaryReader ptrReader = new EndianBinaryReader(ptrStream, l.IsLittleEndian)) {
                uint num_ptrs = ptrReader.ReadUInt32();
                for (uint j = 0; j < num_ptrs; j++) {
                    int file = ptrReader.ReadInt32();
                    uint ptr_ptr = ptrReader.ReadUInt32();
                    reader.BaseStream.Seek(ptr_ptr + baseOffset, SeekOrigin.Begin);
                    uint ptr = reader.ReadUInt32();
                    pointers[ptr_ptr] = new Pointer(ptr, l.files_array[file]);
                }
                long num_fillInPtrs = (totalSize - ptrStream.Position) / 16;
                for (uint j = 0; j < num_fillInPtrs; j++) {
                    uint ptr_ptr = ptrReader.ReadUInt32(); // the address the pointer should be located at
                    int src_file = ptrReader.ReadInt32(); // the file the pointer should be located in
                    uint ptr = ptrReader.ReadUInt32();
                    int target_file = ptrReader.ReadInt32();
                    l.files_array[src_file].pointers[ptr_ptr] = new Pointer(ptr, (l.files_array[target_file])); // can overwrite if necessary
                }
            }
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            ptrStream.Close();
        }
    }
}
