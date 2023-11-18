/**       
 *--------------------------------------------------------------------
 *          Author:         Sam Collins (collinss5@etsu.edu)				
 *          Course-Section: CSCI-2210-001
 *          Assignment:     Project 3
 *          Description:    A class that represents a truck loading dock.	
 * -------------------------------------------------------------------
 */

namespace _2210_001_CollinsSam_BraxtonOlterman_Project_3
{
    class Dock
    {
        public string Id { get; set; }
        public Queue<Truck> Line { get; set; }
        public double TotalSales { get; set; }
        public int TotalCrates { get; set; }
        public int TotalTrucks { get; set; }
        public int TimeInUse { get; set; }
        public int TimeNotInUse { get; set; }

        /// <summary>
        /// makes a new dock
        /// </summary>
        /// <param name="id"> identifier for the dock </param>
        public Dock(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new InvalidDataException();
            Id = id;
            Line = new Queue<Truck>();
            TotalSales = 0.0;
            TotalCrates = 0;
            TotalTrucks = 0;
            TimeInUse = 0;
            TimeNotInUse = 0;
        }

        /// <summary>
        /// truck moves to join a line
        /// </summary>
        /// <param name="truck"> uses a truck </param>
        public void JoinLine(Truck truck)
        {
            Line.Enqueue(truck);
        }

        /// <summary>
        /// truck is removed from the line into the dock
        /// </summary>
        public Truck SendOff()
        {
            return Line.Dequeue();
        }
    }
}
