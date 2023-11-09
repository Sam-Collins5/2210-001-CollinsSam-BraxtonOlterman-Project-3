using System;
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
            string[] drivers = { "James", "John", "Robert", "Michael", "William", "David", "Richard", "Joseph", "Charles", "Thomas", "Daniel", "Matthew", "Donald", "Anthony", "Paul", "Mark", "George", "Steven", "Kenneth", "Andrew", "Edward", "Brian", "Ronald", "Timothy", "Jason", "Kevin", "Jeffrey", "Gary", "Scott", "Frank", "Eric", "Stephen", "William", "Patrick", "Gregory", "Douglas", "Brian", "Dennis", "Peter", "Larry", "Ryan", "Edward", "Jonathan", "Adam", "Russell", "Jerry", "Harry", "Tyler" };
            string[] deliveryCompanies = { "Swift Shipping", "ExpressEagle", "SpeedyFleet", "PrimeParcel", "GlobalMovers", "QuickDispatch", "RapidRoadways", "StarLogistics", "ShipNow", "FastForward", "SwiftCargo", "AceDeliveries", "SpeedyHaul", "PrestigeShip", "TransSwift", "DeliverPro", "GlobalMovers", "QuickCrate", "AirSwift", "SwiftPulse", "PlanetExpress", "ExpressXpress", "FastFreight", "RainbowCourier", "BlueSkyLogistics", "GoldenGateDelivery", "EcoMovers", "SkywardCargo", "PrimeLogistics", "TransitMasters", "FastTracks", "SwiftSprint", "RocketLogistics", "ZipZapDelivery", "RoadRunnerExpress", "EagleEyeShip", "DynamicDispatch", "DeliveryDynamo", "SwiftWheels", "GreenCargo", "MoonbeamMovers", "FleetMaster", "SwiftWings", "SunriseDeliveries", "CruiseCargo", "WaveRunnerLogistics", "OrangeBoxCourier", "DirectSwift", "SwiftSailor", "VelocityVan", "AceExpress" };

            Random randy = new Random();
            driver = drivers[randy.Next(drivers.Length)];
            deliveryCompany = deliveryCompanies[randy.Next(deliveryCompanies.Length)];

            Trailer = new Stack<Crate>();
        }

        public void Load(Crate crate)
        {
            Trailer.Push(crate);
        }

        public Crate Unload()
        {
            return Trailer.Pop();
        }

        public override string ToString()
        {
            string result = $"Driver:  {driver}\n";
            result += $"Company: {deliveryCompany}\n";
            result += $"Crates:\n";
            var crates = Trailer.ToArray();
            for (int i = 0; i < Trailer.Count; i++)
            {
                result += $"  {i}: {crates[i].ID} ${crates[i].Price}\n";
            }
            return result;
        }
    }
}
