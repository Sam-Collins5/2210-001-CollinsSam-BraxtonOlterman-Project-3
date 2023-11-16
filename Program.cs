/**       
 *--------------------------------------------------------------------
 *          Course-Section: CSCI-2210-001
 *          Assignment:     Project 3
 * -------------------------------------------------------------------
 */

using Raylib_cs;
using System.Numerics;
using System.Runtime.InteropServices;

namespace _2210_001_CollinsSam_BraxtonOlterman_Project_3
{
    // Needed in order to disable Raylib's logger
    static unsafe class Logger
    {
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        public static void LogCustom(int logLevel, sbyte* text, sbyte* args)
        {
            return;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Warehouse warehouse = new Warehouse();
            warehouse.Run();
        }
    }
}
