using System;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace csvToSRT
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  csvToSRT source [maxchars] [target]");
                Console.WriteLine("    source   -  csv file to read.  Expected format is start time, stop time, subtitle text");
                Console.WriteLine("                e.g.: 00:00:00.000,00:00:05.000,\"This would be a five second subtitle\"");
                Console.WriteLine("    maxchars - Optional value limiting the length of each subtitle line (useful for editors that don't wrap)");
                Console.WriteLine("    target   - Optional value to specify the output file.  The default is <source>.srt");
                Environment.Exit(0);
            }
            var source = args[0];
            if (!File.Exists(source))
            {
                Console.WriteLine("Error:");
                Console.WriteLine($"  Source file not found - {source}");
                Environment.Exit(1);
            }
            var maxChars = 0;
            if (args.Length > 1)
            {
                if (!int.TryParse(args[1], out maxChars))
                {
                    Console.WriteLine("Error:");
                    Console.WriteLine($"  The second argument (maxchars) must be a number - {args[1]}");
                    Environment.Exit(1);
                }
            }
            var target = Path.GetDirectoryName(source)+Path.DirectorySeparatorChar+Path.GetFileNameWithoutExtension(source) + ".srt";
            if (args.Length > 2)
            {
                target = args[2];
            }

            ConvertToSRT(source, maxChars, target);
        }

        static void ConvertToSRT(string source, int maxChars, string target)
        {
            var counter = 1;
            using (var outFile = new StreamWriter(target, false))
            {
                using (TextFieldParser parser = new TextFieldParser(source))
                {
                    parser.SetDelimiters(",");
                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();
                        if (fields.Length != 3)
                        {
                            Console.WriteLine("Error:");
                            Console.WriteLine($"  Incorrect number of columns in line {counter}");
                            Environment.Exit(1);
                        }
                        var st = new Subtitle
                        {
                            number = counter,
                            start = fields[0],
                            stop = fields[1],
                            text = fields[2]
                        };
                        if (!st.checkValues(maxChars))
                        {
                            Console.WriteLine($"Line {counter} skipped: {st.errorString}");
                        }
                        else
                        {
                            outFile.Write(st.genSRT());
                        }

                        counter++;
                    }
                }
            }


        }
    }
}
