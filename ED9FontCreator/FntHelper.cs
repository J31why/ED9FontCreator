using ED9FontCreator.Models;
using ED9FontCreator.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;

namespace ED9FontCreator
{
    internal static class FntHelper
    {
        public static readonly List<ReplaceItem> ReplaceGroup = [];

        public static bool ContainsSequence(byte[] array, byte[] sequence)
        {
            for (int i = 0; i <= array.Length - sequence.Length; i++)
            {
                if (array.Skip(i).Take(sequence.Length).SequenceEqual(sequence))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool GetFnt(string? path, [MaybeNullWhen(false)] out Fnt fnt)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                fnt = null;
                return false;
            }

            try
            {
                var f = new Fnt();
                using var fs = File.OpenRead(path);
                f.Head = new byte[0x28];
                var read = fs.Read(f.Head);
                if (read != 0x28 || !ContainsSequence(f.Head, [0x46, 0x4C, 0x54, 0x49]))
                    throw new Exception("fnt file error.");

                fs.Seek(8, SeekOrigin.Begin);
                f.TotalChars = fs.ReadShort();
                fs.Seek(36, SeekOrigin.Begin);
                f.DataLength = fs.ReadInt();
                for (var i = 0; i < f.TotalChars; i++)
                    f.Chars.Add(GetFntChar(fs));
                fnt = f;
                return true;
            }
            catch
            {
                fnt = null;
                return false;
            }
        }

        private static FntChar GetFntChar(FileStream fs)
        {
            return new FntChar
            {
                Offset = fs.Position,
                Char = (char)fs.ReadInt(),
                Type = fs.ReadInt(),
                X = (short)fs.ReadShort(),
                Y = (short)fs.ReadShort(),
                PixelWidth = (short)fs.ReadShort(),
                PixelHeight = (short)fs.ReadShort(),
                ColorChannel = (short)fs.ReadShort(),
                XOffset = (short)fs.ReadShort(),
                YOffset = (short)fs.ReadShort(),
                Width = (short)fs.ReadShort()
            };
        }

        public static char Replace(char @char, bool simplify, bool replace = true)
        {
            var text = @char.ToString();
            var match = ReplaceGroup.FirstOrDefault(x => x.Old == text);
            if (match != null) 
                text = match.New;
            if (simplify && text != null)
                text = ChineseConverter.Convert(text, ChineseConversionDirection.TraditionalToSimplified);
            var charArray = text?.ToCharArray();
            return charArray?.First() ?? @char;
        }

        public static void InitReplaceGroup(string replace)
        {
            ReplaceGroup.Clear();
            var regex = new Regex(@"\[(.*?)\]\s*=\s*\[(.*?)\]");
            var matches = regex.Matches(replace);
            foreach (Match match in matches)
            {
                var group = new ReplaceItem(match.Groups[1].Value, match.Groups[2].Value);
                ReplaceGroup.Add(group);
            }
        }
    }
}