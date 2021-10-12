
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Editor.Data
{
    public static class CSVReader
    {
        private static string DataSeparator = @",|;";
        private static string LineSeparator = @"\r\n|\n\r|\n|\r";
        private static char[] trim_Chars = { '\"' };

        public static List<Dictionary<string, object>> Read(string file)
        {
            List<Dictionary<string, object>> RetList = new List<Dictionary<string, object>>();

            string rawText = System.IO.File.ReadAllText(file);

            string[] lines = Regex.Split(rawText, LineSeparator);

            if (lines.Length <= 1) return RetList;

            string[] header = Regex.Split(lines[0], DataSeparator);
            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = Regex.Split(lines[i], DataSeparator);
                if (values.Length == 0 || values[0] == "") continue;

                Dictionary<string, object> entry = new Dictionary<string, object>();
                for (int j = 0; j < header.Length && j < values.Length; j++)
                {
                    string value = values[j];
                    value = value.TrimStart(trim_Chars).TrimEnd(trim_Chars).Replace("\\", "");
                    object finalValue = value;
                    int n;
                    float f;
                    
                    if (int.TryParse(value, out n))
                        finalValue = n;
                    else if (float.TryParse(value, out f))
                        finalValue = f;

                    entry[header[j]] = finalValue;
                }
                
                RetList.Add(entry);
            }

            return RetList;
        }

    }
}