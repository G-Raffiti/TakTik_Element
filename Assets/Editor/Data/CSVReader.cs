using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Editor.Data
{
    public static class CsvReader
    {
        private static string _dataSeparator = @",|;";
        private static string _lineSeparator = @"\r\n|\n\r|\n|\r";
        private static char[] _trimChars = { '\"' };

        public static List<Dictionary<string, object>> Read(string _file)
        {
            List<Dictionary<string, object>> _retList = new List<Dictionary<string, object>>();

            string _rawText = System.IO.File.ReadAllText(_file);

            string[] _lines = Regex.Split(_rawText, _lineSeparator);

            if (_lines.Length <= 1) return _retList;

            string[] _header = Regex.Split(_lines[0], _dataSeparator);
            for (int _i = 1; _i < _lines.Length; _i++)
            {
                string[] _values = Regex.Split(_lines[_i], _dataSeparator);
                if (_values.Length == 0 || _values[0] == "") continue;

                Dictionary<string, object> _entry = new Dictionary<string, object>();
                for (int _j = 0; _j < _header.Length && _j < _values.Length; _j++)
                {
                    string _value = _values[_j];
                    _value = _value.TrimStart(_trimChars).TrimEnd(_trimChars).Replace("\\", "");
                    object _finalValue = _value;
                    int _n;
                    float _f;
                    
                    if (int.TryParse(_value, out _n))
                        _finalValue = _n;
                    else if (float.TryParse(_value, out _f))
                        _finalValue = _f;

                    _entry[_header[_j]] = _finalValue;
                }
                
                _retList.Add(_entry);
            }

            return _retList;
        }

    }
}