﻿using System;
using System.IO;

namespace OpenSpace {
    public class EndianBinaryReader : BinaryReader {
        bool isLittleEndian = true;
        bool masking = false; // for Rayman 2
        uint mask = 0;
        public EndianBinaryReader(System.IO.Stream stream) : base(stream) { isLittleEndian = true; }
        public EndianBinaryReader(System.IO.Stream stream, bool isLittleEndian) : base(stream) { this.isLittleEndian = isLittleEndian; }

        public override int ReadInt32() {
            var data = ReadBytes(4);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToInt32(data, 0);
        }

        public override float ReadSingle() {
            var data = ReadBytes(4);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToSingle(data, 0);
        }

        public override Int16 ReadInt16() {
            var data = ReadBytes(2);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToInt16(data, 0);
        }

        public override UInt16 ReadUInt16() {
            var data = ReadBytes(2);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToUInt16(data, 0);
        }

        public override Int64 ReadInt64() {
            var data = ReadBytes(8);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToInt64(data, 0);
        }

        public override UInt32 ReadUInt32() {
            var data = ReadBytes(4);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToUInt32(data, 0);
        }

        public override byte[] ReadBytes(int count) {
            byte[] bytes = base.ReadBytes(count);
            if (masking) {
                for (int i = 0; i < count; i++) {
                    bytes[i] = decodeByte(bytes[i], mask);
                    mask = getNextMask(mask);
                }
            }
            return bytes;
        }

        public override byte ReadByte() {
            byte result = base.ReadByte();
            if (masking) {
                result = decodeByte(result, mask);
                mask = getNextMask(mask);
            }
            return result;
        }

        public string ReadNullDelimitedString() {
            string result = "";
            char c = ReadChar();
            while (c != 0x0) {
                result += c;
                c = ReadChar();
            }
            return result;
        }

        // To make sure position is a multiple of alignBytes
        public void Align(int alignBytes) {
            if (BaseStream.Position % alignBytes != 0) {
                ReadBytes(alignBytes - (int)(BaseStream.Position % alignBytes));
            }
        }

        // Used in Rayman 2 when reading blocks with size sizeToRead, so that it is aligned after reading the block
        public void PreAlign(int sizeToRead, int alignBytes) {
            int rest = sizeToRead % alignBytes;
            if (rest > 0) {
                ReadBytes(alignBytes - rest);
            }
        }


        #region Masking (Rayman 2)
        public void ReadMask() {
            SetMask(ReadUInt32());
        }

        public void SetMask(uint mask) {
            this.mask = mask;
            masking = true;
        }

        byte decodeByte(byte toDecode, uint mask) {
            return (byte)(toDecode ^ ((mask >> 8) & 0xFF));
        }

        uint getNextMask(uint currentMask) {
            return (uint)(16807 * (currentMask ^ 0x75BD924) - 0x7FFFFFFF * ((currentMask ^ 0x75BD924) / 0x1F31D));
        }

        // Turn off masking for this binary reader
        public void MaskingOff() {
            masking = false;
        }
        #endregion
    }
}