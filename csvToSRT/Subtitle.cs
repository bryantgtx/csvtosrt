using System.Collections.Generic;

namespace csvToSRT
{
    class Subtitle
    {
        public int number = 0;
        public string start = "";
        public string stop = "";
        public string text = "";
        private readonly List<string> titles = new List<string>();
             
        public string errorString;

        public bool checkValues(int maxChars)
        {
            var okay = true;
            var splitChars = " -.,".ToCharArray();
            var check = checkTime(start);
            if (check.Length == 0)
            {
                okay = false;
                errorString += $"incorrect format for start value, expected 00:00:00.000, got {start};";
            }
            else
            {
                start = check;
            }

            check = checkTime(stop);
            if (check.Length == 0)
            {
                okay = false;
                errorString += $"incorrect format for stop value, expected 00:00:00.000, got {stop};";
            }
            else
            {
                stop = check;
            }

            var lines = text.Split('\n');
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (maxChars == 0 || line.Length <= maxChars)
                {
                    titles.Add(line);
                }
                else
                {
                    while (line.Length > maxChars)
                    {
                        var splitPoint = line.LastIndexOfAny(splitChars, maxChars - 1);
                        if (splitPoint > 0)
                        {
                            titles.Add(line.Substring(0, splitPoint + 1));
                            line = line.Substring(splitPoint + 1);
                        }
                        else // no breaks before maxchars (use case - URL?  German?)
                        {
                            okay = false;
                            errorString += $"unable to break {maxChars} characters;";
                            splitPoint = line.IndexOfAny(splitChars, maxChars);
                            if (splitPoint > 0)
                            {
                                titles.Add(line.Substring(0, splitPoint));
                                line = line.Substring(splitPoint + 1);
                            }
                            else  // nothing that direction, either
                            {
                                break;
                            }
                        }
                    }
                    titles.Add(line);
                }
            }
            return okay;
        }


        public string genSRT()
        {
            var result = "";
            result += $"{number}\n";
            result += $"{start} --> {stop}\n";
            foreach (var line in titles)
            {
                result += line + "\n";
            }

            result += "\n";
            return result;
        }

        private string checkTime(string timecode)
        {
            var splits = timecode.Replace('.',',').Split(':');
            if (splits.Length != 3)
            {
                return "";
            }
            var secs = splits[2].Split(',');
            var millis = secs.Length > 1 ? secs[1] : "0";

            return $"{splits[0].PadLeft(2,'0')}:{splits[1].PadLeft(2, '0')}:{secs[0].PadLeft(2, '0')},{millis.PadLeft(3, '0')}";
        }
    }
}
