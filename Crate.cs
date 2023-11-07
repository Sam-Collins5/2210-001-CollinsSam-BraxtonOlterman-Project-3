using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace _2210_001_CollinsSam_BraxtonOlterman_Project_3
{
    class Crate
    {
        public string ID { get; set; }
        public double Price { get; set; }

        public Crate(string id)
        {
            ID = id;
            Random randy = new Random();
            Price = Math.Round(randy.NextDouble() * (500 - 50) + 50, 2);
        }

        public override string ToString()
        {
            return $"{ID},{Price}";
        }
    }
}
