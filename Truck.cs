/**       
 *--------------------------------------------------------------------
 *          Author:         Braxton Olterman (oltermanb@etsu.edu)				
 *          Course-Section: CSCI-2210-001
 *          Assignment:     Project 3
 *          Description:    A class that represents a truck.	
 * -------------------------------------------------------------------
 */

namespace _2210_001_CollinsSam_BraxtonOlterman_Project_3
{
    class Truck
    {
        public string driver { get; set; }
        public string deliveryCompany { get; set; }
        public Stack<Crate> Trailer { get; set; }

        /// <summary>
        /// makes a new truck
        /// </summary>
        public Truck() 
        {
            string[] drivers = { "James", "John", "Robert", "Michael", "William", "David", "Richard", "Joseph", "Charles", "Thomas", "Daniel", "Matthew", "Donald", "Anthony", "Paul", "Mark", "George", "Steven", "Kenneth", "Andrew", "Edward", "Brian", "Ronald", "Timothy", "Jason", "Kevin", "Jeffrey", "Gary", "Scott", "Frank", "Eric", "Stephen", "William", "Patrick", "Gregory", "Douglas", "Brian", "Dennis", "Peter", "Larry", "Ryan", "Edward", "Jonathan", "Adam", "Russell", "Jerry", "Harry", "Tyler" };
            string[] deliveryCompanies = { "Swift Shipping", "ExpressEagle", "SpeedyFleet", "PrimeParcel", "GlobalMovers", "QuickDispatch", "RapidRoadways", "StarLogistics", "ShipNow", "FastForward", "SwiftCargo", "AceDeliveries", "SpeedyHaul", "PrestigeShip", "TransSwift", "DeliverPro", "GlobalMovers", "QuickCrate", "AirSwift", "SwiftPulse", "PlanetExpress", "ExpressXpress", "FastFreight", "RainbowCourier", "BlueSkyLogistics", "GoldenGateDelivery", "EcoMovers", "SkywardCargo", "PrimeLogistics", "TransitMasters", "FastTracks", "SwiftSprint", "RocketLogistics", "ZipZapDelivery", "RoadRunnerExpress", "EagleEyeShip", "DynamicDispatch", "DeliveryDynamo", "SwiftWheels", "GreenCargo", "MoonbeamMovers", "FleetMaster", "SwiftWings", "SunriseDeliveries", "CruiseCargo", "WaveRunnerLogistics", "OrangeBoxCourier", "DirectSwift", "SwiftSailor", "VelocityVan", "AceExpress" };

            Random randy = new Random();
            driver = drivers[randy.Next(drivers.Length)];
            deliveryCompany = deliveryCompanies[randy.Next(deliveryCompanies.Length)];

            Trailer = new Stack<Crate>();
        }

        /// <summary>
        /// loads crates to trailer
        /// </summary>
        /// <param name="crate"> uses a crate </param>
        public void Load(Crate crate)
        {
            Trailer.Push(crate);
        }

        /// <summary>
        /// unloads crates from trailer
        /// </summary>
        public Crate Unload()
        {
            return Trailer.Pop();
        }

        /// <summary>
        /// returns string representing the truck
        /// </summary>
        /// <returns></returns>
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
