using System;
using System.IO;

namespace ED9FontCreator
{
    public static class FileStreamExtension
    {
        public static int ReadShort(this FileStream fs)
        {
            var b1 = fs.ReadByte();
            var b2 = fs.ReadByte();
            return (b2 << 8) + b1;
        }

        public static int ReadInt(this FileStream fs)
        {
            var b1 = fs.ReadByte();
            var b2 = fs.ReadByte();
            var b3 = fs.ReadByte();
            var b4 = fs.ReadByte();
            return (b4 << 24) + (b3 << 16) + (b2 << 8) + b1;
        }

        public static void WriteInt(this FileStream fs, int vaule)
        {
            var bytes = BitConverter.GetBytes(vaule);
            fs.Write(bytes);
        }

        public static void WriteShort(this FileStream fs, short vaule)
        {
            var bytes = BitConverter.GetBytes(vaule);
            fs.Write(bytes);
        }
    }
}