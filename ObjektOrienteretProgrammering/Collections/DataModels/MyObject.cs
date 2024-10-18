using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collections.DataModels
{
    public class MyObject
    {
        public int Number { get; set; }
        public string Text { get; set; }

        public MyObject(int number, string text)
        {
            Number = number;
            Text = text;
        }

        public override string ToString()
        {
            return $"Number: {Number}, Text: {Text}";
        }
    }

}
