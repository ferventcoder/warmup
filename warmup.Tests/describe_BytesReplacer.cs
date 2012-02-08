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

        int firstFewBytes = 10;

        string searchFor = "__NAME__";

        string replaceWith = "Replaced";

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
