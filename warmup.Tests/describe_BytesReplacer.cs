using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSpec;
using System.IO;
using System.Collections;

namespace warmup.Tests
{
    class describe_BytesReplacer : nspec
    {
        string fileContainingByteOffsetMap = "BomTestFiles\\DynamicBlog.slnfile";

        string fileWithoutByteOffsetMap = "BomTestFiles\\dev.yml";

        string fileContainingPrecedingUnderScores = "BomTestFiles\\kata.txt";

        int firstFewBytes = 10;

        string searchFor = "__NAME__";

        string replaceWith = "Replaced";

        byte[] source;

        byte[] find;

        void file_encodings()
        {
            it["byte offset map/preamble is retained after conversion"] = () =>
            {
                var processedFile = Process(fileContainingByteOffsetMap, searchFor, replaceWith);

                Text(fileContainingByteOffsetMap).Replace(searchFor, replaceWith).should_be(Text(processedFile));

                FirstFewBytes(fileContainingByteOffsetMap).should_be(FirstFewBytes(processedFile));
            };

            it["byte offset map/preamble isn't added if it didn't have one in the first place"] = () =>
            {
                var processedFile = Process(fileWithoutByteOffsetMap, searchFor, replaceWith);

                Text(fileWithoutByteOffsetMap).Replace(searchFor, replaceWith).should_be(Text(processedFile));

                FirstFewBytes(fileWithoutByteOffsetMap).should_be(FirstFewBytes(processedFile));
            };
        }

        void replacing_strings_that_have_preceding_underscores()
        {
            it["the content with describe___NAME__ (preceding underscores) is converted to describe_Replaced"] = () =>
            {
                var processedFile = Process(fileContainingPrecedingUnderScores, searchFor, replaceWith);

                Text(fileContainingPrecedingUnderScores).Replace(searchFor, replaceWith).should_be("describe_Replaced");
            };
        }

        void finding_bytes()
        {
            context["bytes match in the begining"] = () =>
            {
                before = () =>
                {
                    source = new byte[] { 8, 7, 8, 9, 10 };

                    find = new byte[] { 8, 7, 8 };
                };

                it["returns middle index (index of 2) as the matching index"] = () =>
                    BytesReplacer.FindBytes(source, find).should_be(0);
            };

            context["bytes match in the middle"] = () =>
            {
                before = () =>
                {
                    source = new byte[] { 8, 7, 8, 9, 10 };

                    find = new byte[] { 8, 9 };
                };

                it["returns middle index (index of 2) as the matching index"] = () =>
                    BytesReplacer.FindBytes(source, find).should_be(2);
            };

            context["bytes match at the end"] = () =>
            {
                before = () =>
                {
                    source = new byte[] { 7, 8, 9, 10 };

                    find = new byte[] { 9, 10 };
                };

                it["returns last index (index of 2) as the matching index"] = () =>
                    BytesReplacer.FindBytes(source, find).should_be(2);
            };

            context["bytes do not match"] = () =>
            {
                before = () =>
                {
                    source = new byte[] { 1, 2, 4, 5, 3 };

                    find = new byte[] { 2, 3 };
                };

                it["returns index -1"] = () =>
                    BytesReplacer.FindBytes(source, find).should_be(-1);
            };
        }

        string Text(string path)
        {
            return File.ReadAllText(path);
        }

        byte[] FirstFewBytes(string path)
        {
            return File.ReadAllBytes(path).Take(firstFewBytes).ToArray();
        }

        string Process(string path, string searchFor, string replaceWith)
        {
            var replacer = new BytesReplacer(BytesReplacer.FileEncoding(path));

            string clonePath = path + ".clone";

            var bytes = File.ReadAllBytes(path);

            var newBytes = replacer.Replace(bytes, searchFor, replaceWith);

            File.WriteAllBytes(clonePath, newBytes);

            return clonePath;
        }
    }
}
