using Battleship.Library.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Battleship.Library.Formatters
{
    public class GridCustomFormatter : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
            {
                return this;
            }

            return null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            return arg.ToString();
        }
    }
}
