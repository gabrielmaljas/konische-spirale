using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VMTKonischeSpirale.Interfaces
{
    interface ICsvFormattable
    {
        public string ToCsvString(object obj)
        {
            var csv = new StringBuilder();

            var properties = obj.GetType().GetProperties();

            csv.AppendLine(string.Join(";", Array.ConvertAll(properties, p => p.GetValue(obj).ToString())));

            return csv.ToString(); 
        }
    }
}
