﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2210_001_CollinsSam_BraxtonOlterman_Project_3
{
    class Truck
    {
        public string driver { get; set; }
        public string deliveryCompany { get; set; }
        public Stack<Crate> Trailer { get; set; }

        public Truck() 
        {
            string[] drivers = {"Larry", "Terry", "Jerry", "Barry", "Harry", "Carey"};
            string[] deliveryCompanies = { "Walmart", "UPS", "Navigator Express", "DHL Group", "FedEx", "SF Express" };
            
            Random randy = new Random();
            driver = drivers[randy.Next(drivers.Length)];
            deliveryCompany = deliveryCompanies[randy.Next(deliveryCompanies.Length)];

            Trailer = new Stack<Crate>();
        }

        void Load(Crate crate)
        {
            Trailer.Push(crate);
        }

        Crate Unload()
        {
            return Trailer.Pop();
        }
    }
}
