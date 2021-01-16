using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlSystemsLibrary.Services
{
    class Strings
    {
        public static string RemoveCharacters(string InputText)
        {
            string temp = "";
            foreach (char ch in InputText)
            {
                if (ch == ' ' || ch == ',' || ch == '.' || ch == '-' || ch == '_' || ch == '=' || ch == '+' || ch == '|' || ch == '"' || ch == '`' || ch == ')' || ch == '(' || ch == '{' || ch == '}' || ch == '[' || ch == ']' || ch == ';' || ch == ':' || ch == '@' || ch == '#' || ch == '$' || ch == '%' || ch == '&' || ch == '*' || ch == '/' || ch == '>' || ch == '<' || ch == '!' || ch == '?')
                    continue;
                temp += ch;
            }
            return temp;
        }
    }
}
