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
            // Note: After the 48 time increments, the trucks will stop arriving

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
                // Assign truck at front of entrance line to a dock
                Dock dockToUse = new Dock("empty");
                int shortestLine = 1;

                for (int d = 0; d < Docks.Count(); d++)
                {
                    if (Docks[d].Line.Count() < shortestLine)
                    {
                        shortestLine = Docks[d].Line.Count();
                        dockToUse = Docks[d];
                    }
                }

                if (Entrance.Count() > 0)
                {
                    dockToUse.Line.Enqueue(Entrance.Dequeue());
                }

                // A truck arrives
                Truck newTruck = new Truck();

                // Give this new truck a random number of random crates
                Random randy = new Random();
                int numCrates = randy.Next(1, 11);

                for (int c = 0; c < numCrates; c++)
                {
                    newTruck.Load(new Crate(CurrentCrateID.ToString()));
                    CurrentCrateID++;
                }

                Console.WriteLine("New Truck:\n" + newTruck.ToString());
                Console.ReadLine();

                // Put the new truck at the back of the entrance line
                Entrance.Enqueue(newTruck);

                // Unload the crates
                for (int c = 0; c < numberOfDocks; c++)
                {
                    if (Docks[c].Line.Count() > 0)
                    {
                        Truck truck = Docks[c].Line.Peek();
                        if (truck.Trailer.Count() > 0)
                        {
                            Crate crate = truck.Unload();

                            Console.WriteLine($"{crate.ID}, ${crate.Price}");
                            Console.ReadLine();
                        }
                        else
                        {
                            Docks[c].Line.Dequeue();
                        }
                    }
                }
            }
        }
    }
}
