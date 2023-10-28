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
        public Queue<Truck> Lines { get; set; }
        public double TotalSales { get; set; }
        public int TotalCrates { get; set; }
        public int TotalTrucks { get; set; }
        public int TimeInUse { get; set; }
        public int TimeNotInUse { get; set; }

        public Dock(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new InvalidDataException();
            Id = id;
            Lines = new Queue<Truck>();
            TotalSales = 0.0;
            TotalCrates = 0;
            TotalTrucks = 0;
            TimeInUse = 0;
            TimeNotInUse = 0;
        }

        void JoinLine(Truck truck)
        {
            Lines.Enqueue(truck);
        }

        Truck SendOff()
        {
            return Lines.Dequeue();
        }
    }
}
