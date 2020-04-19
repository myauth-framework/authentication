using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic;

namespace MyAuth.Authentication
{
    class ClaimsReader
    {
        static readonly char[] SkitPreKeyRead = new char[]{' ', '\t', ','};

        private readonly string _str;

        public int CurrentPosition { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="ClaimsReader"/>
        /// </summary>
        public ClaimsReader(string str)
        {
            _str = str.Trim();
        }

        public string ReadKey()
        {
            var sb = new StringBuilder();
            
            for (int i = CurrentPosition; i < _str.Length && _str[i] != '='; i++)
            {
                sb.Append(_str[i]);
                CurrentPosition++;
            }

            for (int i = CurrentPosition; i < _str.Length && (i== 0 || _str[i-1] != '\"'); i++)
            {
                CurrentPosition++;
            }

            return sb.ToString();
        }

        public string ReadValue()
        {
            var sb = new StringBuilder();
            
            for (int i = CurrentPosition; i < _str.Length && (_str[i] != '\"' || _str[i-1] == '\\'); i++)
            {
                sb.Append(_str[i]);
                CurrentPosition++;
            }

            for (int i = CurrentPosition; i < _str.Length && !char.IsLetter(_str[i]); i++)
            {
                CurrentPosition++;
            }

            return sb.ToString();
        }

        public bool Eof()
        {
            return CurrentPosition >= _str.Length-1;
        }
    }
}
