/**       
 *--------------------------------------------------------------------
 *          Author:         Sam Collins (collinss5@etsu.edu)				
 *          Course-Section: CSCI-2210-001
 *          Assignment:     Project 3
 *          Description:    A class that represents a warehouse.
 * -------------------------------------------------------------------
 */

namespace _2210_001_CollinsSam_BraxtonOlterman_Project_3
{
    class Warehouse
    {
        private List<Dock> Docks;
        private Queue<Truck> Entrance;

        private const int MAX_DOCKS = 15;
        private int CurrentDockID = 0;
        private int CurrentCrateID = 0;

        private const int TIME_INCREMENTS = 48;

        public Warehouse()
        {
            Docks = new List<Dock>();
            Entrance = new Queue<Truck>();
        }

        public void Run()
        {
            // TODO : Make this better
            Console.Write("Enter number of docks (1-15):\n>");
            string numberOfDocksStr = Console.ReadLine();
            int numberOfDocks = int.Parse(numberOfDocksStr);

            // Make the number of desired docks and assign each of them a unique ID
            for (int i = 0; i < numberOfDocks; i++)
            {
                Docks.Add(new Dock(CurrentDockID.ToString()));
                CurrentDockID++;
            }

            // Warehouse time increment based simulation
            for (int i = 0; i < TIME_INCREMENTS; i++)
            {
                // A truck arrives
                Truck truck = new Truck();

                // Give this new truck a random number of random crates
                Random randy = new Random();
                int numCrates = randy.Next(1, 11);

                for (int c = 0; c < numCrates; c++)
                {
                    truck.Load(new Crate(CurrentCrateID.ToString()));
                    CurrentCrateID++;
                }

                Console.WriteLine($"{truck.driver}, {truck.deliveryCompany}");
                Console.ReadLine();
            }
        }
    }
}
