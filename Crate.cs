/**       
 *--------------------------------------------------------------------
 *          Author:         Braxton Olterman (oltermanb@etsu.edu)				
 *          Course-Section: CSCI-2210-001
 *          Assignment:     Project 3
 *          Description:    A class that represents a crate.	
 * -------------------------------------------------------------------
 */

namespace _2210_001_CollinsSam_BraxtonOlterman_Project_3
{
    class Crate
    {
        public string ID { get; set; }
        public double Price { get; set; }

        /// <summary>
        /// makes new crate
        /// </summary>
        /// <param name="id"> identifier for the crate </param>
        public Crate(string id)
        {
            ID = id;
            Random randy = new Random();
            Price = Math.Round(randy.NextDouble() * (500 - 50) + 50, 2);
        }

        /// <summary>
        /// returns string representing the crate
        /// </summary>
        public override string ToString()
        {
            return $"{ID},{Price}";
        }
    }
}
