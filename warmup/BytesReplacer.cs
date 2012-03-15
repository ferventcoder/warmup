﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace warmup
{
    public class BytesReplacer
    {
        private Encoding _encoding;

        public static Encoding FileEncoding(string path)
        {
            using (var streamReader = new StreamReader(path))
            {
                return streamReader.CurrentEncoding;
            }
        }

        public BytesReplacer(Encoding encoding)
        {
            _encoding = encoding;
        }

        public byte[] Replace(byte[] content, string searchFor, string replaceWith)
        {
            var occurences = OccurrencesOf(_encoding.GetString(content), searchFor);

            var newBytes = content;

            for (int i = 0; i < occurences; i++)
            {
                newBytes = ReplaceBytes(newBytes, _encoding.GetBytes(searchFor), _encoding.GetBytes(replaceWith));
            }

            return newBytes;
        }

        //http://www.dijksterhuis.org/manipulating-strings-in-csharp-finding-all-occurrences-of-a-string-within-another-string/
        public static int OccurrencesOf(string source, string search)
        {
            int pos = 0, offset = 0, occurrences = 0;

            while ((pos = source.IndexOf(search)) > 0)
            {
                source = source.Substring(pos + search.Length);
                offset += pos;
                occurrences += 1;
            }

            return occurrences;
        }

        //http://stackoverflow.com/questions/5132890/c-sharp-replace-bytes-in-byte
        public static byte[] ReplaceBytes(byte[] src, byte[] search, byte[] repl)
        {
            byte[] dst = null;

            int index = FindBytes(src, search);

            if (index >= 0)
            {
                dst = new byte[src.Length - search.Length + repl.Length];

                // before found array
                Buffer.BlockCopy(src, 0, dst, 0, index);

                // repl copy
                Buffer.BlockCopy(repl, 0, dst, index, repl.Length);

                // rest of src array
                Buffer.BlockCopy(
                    src,
                    index + search.Length,
                    dst,
                    index + repl.Length,
                    src.Length - (index + search.Length));
            }

            return dst;
        }

        public static int FindBytes(byte[] source, byte[] find, int index = 0)
        {
            if (index >= source.Length) return -1;

            if (Match(source, find, index)) return index;

            return FindBytes(source, find, ++index);
        }

        public static bool Match(byte[] source, byte[] find, int index)
        {
            for (int i = 0; i < find.Length; i++, index++)
            {
                if (index >= source.Length) return false;

                if (source[index] != find[i]) return false;
            }

            return true;
        }
    }
}