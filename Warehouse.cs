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
        public List<Dock> Docks;
        public Queue<Truck> Entrance;

        public Warehouse()
        {
            Docks = new List<Dock>();
            Entrance = new Queue<Truck>();
        }

        void Run()
        {

        }
    }
}
