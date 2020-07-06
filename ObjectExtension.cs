using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System
{
    public static class ObjectExtension
    {
        public static bool ToBool(this object obj)
        {
            if (obj == null) { return false; }
            if (obj.ToString() == "") { return false; }
            try { if (Convert.ToInt32(obj.ToString()) == 0) { return false; } else { return true; } }
            catch
            {
                if (obj as IEnumerable<object> != null) { return (obj as IEnumerable<object>).Count() > 0; }
                try { if ((bool)obj == false) { return false; } }
                catch { return true; }
            }
            return true;
        }

        public static bool InLowerCase(this char obj)
        {
            return obj.ToString().ToLower() == obj.ToString(); 
        }

        public static string PutSpaces(this string obj)
        {
            var UpperCaseLetters = obj.Where(letter => !letter.InLowerCase());
            string res = obj;
            foreach (char UpperCaseLetter in UpperCaseLetters)
            {
                res = res.Replace(UpperCaseLetter.ToString(), " " + UpperCaseLetter.ToString());
            }
            return res;
        }
    }
}
