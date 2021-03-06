﻿using OpenSpace.FileFormat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenSpace {
    public class Pointer : IEquatable<Pointer> {
        public uint offset;
        public FileWithPointers file;
        public Pointer(uint offset, FileWithPointers file) {
            this.offset = offset;
            this.file = file;
        }

        public override bool Equals(System.Object obj) {
            return obj is Pointer && this == (Pointer)obj;
        }
        public override int GetHashCode() {
            return offset.GetHashCode() ^ file.GetHashCode();
        }

        public bool Equals(Pointer other) {
            return this == (Pointer)other;
        }

        public static bool operator ==(Pointer x, Pointer y) {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return x.offset == y.offset && x.file == y.file;
        }
        public static bool operator !=(Pointer x, Pointer y) {
            return !(x == y);
        }
        public static Pointer operator +(Pointer x, Decimal y) {
            return new Pointer((uint)((Decimal)x.offset + y), x.file);
        }
        public override string ToString() {
            return file.name + "|" + String.Format("0x{0:X8}", offset);
        }

        public static Pointer GetPointerAtOffset(Pointer pointer) {
            MapLoader l = MapLoader.Loader;
            if (pointer.file.pointers.ContainsKey(pointer.offset)) {
                Pointer ptr = pointer.file.pointers[pointer.offset];
                if (ptr.offset == 0) return null;
                return ptr;
            }
            return null;
        }

        public static Pointer Read(EndianBinaryReader reader) {
            MapLoader l = MapLoader.Loader;
            uint current_off = (uint)(reader.BaseStream.Position);
            uint value = reader.ReadUInt32();
            FileWithPointers file = l.GetFileByReader(reader);
            if (file == null) {
                throw new FormatException("Reader wasn't recognized.");
            }
            uint fileOff = (uint)(current_off - file.baseOffset);
            if (!file.pointers.ContainsKey(fileOff)) {
                if (value == 0) return null;
                if (!l.allowDeadPointers) throw new FormatException("Not a valid pointer in file " + file.name + " at position " + current_off);
                return null;
            }
            return file.pointers[fileOff];
        }

        // For readers
        public Pointer Goto(ref EndianBinaryReader reader) {
            Pointer oldPos = Current(reader);
            reader = file.reader;
            reader.BaseStream.Seek(offset + file.baseOffset, SeekOrigin.Begin);
            return oldPos;
        }

        public static Pointer Goto(ref EndianBinaryReader reader, Pointer newPos) {
            if (newPos != null) return newPos.Goto(ref reader);
            return null;
        }

        public static Pointer Current(EndianBinaryReader reader) {
            MapLoader l = MapLoader.Loader;
            uint curPos = (uint)reader.BaseStream.Position;
            FileWithPointers curFile = l.GetFileByReader(reader);
            return new Pointer((uint)(curPos - curFile.baseOffset), curFile);
        }

        // For writers
        public Pointer Goto(ref EndianBinaryWriter writer) {
            Pointer oldPos = Current(writer);
            writer = file.writer;
            writer.BaseStream.Seek(offset + file.baseOffset, SeekOrigin.Begin);
            return oldPos;
        }

        public static Pointer Goto(ref EndianBinaryWriter writer, Pointer newPos) {
            if (newPos != null) return newPos.Goto(ref writer);
            return null;
        }

        public static Pointer Current(EndianBinaryWriter writer) {
            MapLoader l = MapLoader.Loader;
            uint curPos = (uint)writer.BaseStream.Position;
            FileWithPointers curFile = l.GetFileByWriter(writer);
            return new Pointer((uint)(curPos - curFile.baseOffset), curFile);
        }
    }
}
