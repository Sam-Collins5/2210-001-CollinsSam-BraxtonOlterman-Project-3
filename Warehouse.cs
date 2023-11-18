/**       
 *--------------------------------------------------------------------
 *          Author:         Sam Collins (collinss5@etsu.edu)				
 *          Course-Section: CSCI-2210-001
 *          Assignment:     Project 3
 *          Description:    A class that represents a warehouse.
 * -------------------------------------------------------------------
 */
using Raylib_cs;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Threading.Channels;

namespace _2210_001_CollinsSam_BraxtonOlterman_Project_3
{
    class Warehouse
    {
        private List<Dock> Docks;
        private Queue<Truck> Entrance;

        private const int MAX_DOCKS = 15;
        private int NumberOfDocks = 0;
        private int CurrentDockID = 0;
        private int CurrentCrateID = 0;

        private const int MAX_NEW_TRUCKS = 3;
        private const int NUM_OF_TRUCKS_TO_PROCESS = 2;

        private const int TIME_INCREMENTS = 48;
        private int CurrentTime = 0;

        private StringBuilder SimulationDataSB;
        private int LongestDockLine = 0;
        private List<double> CratePrices;
        private List<double> TruckPrices;
        private int[] TimesThatDocksInUse;
        private int[] TimesThatDocksNotInUse;

        public Warehouse()
        {
            Docks = new List<Dock>();
            Entrance = new Queue<Truck>();

            SimulationDataSB = new StringBuilder();
            CratePrices = new List<double>();
            TruckPrices = new List<double>();
        }

        public void Run()
        {
            Console.Write("Enter number of docks (1-15):\n>");
            string numberOfDocksStr = Console.ReadLine();
            NumberOfDocks = int.Parse(numberOfDocksStr);

            SimulationDataSB.Append($"Number of Docks: {NumberOfDocks}");
            TimesThatDocksInUse = new int[NumberOfDocks];
            TimesThatDocksNotInUse = new int[NumberOfDocks];

            // Make the number of desired docks and assign each of them a unique ID
            for (int i = 0; i < NumberOfDocks; i++)
            {
                Docks.Add(new Dock(CurrentDockID.ToString()));
                CurrentDockID++;
            }

            // Raylib
            unsafe
            {
                Raylib.SetTraceLogCallback(&Logger.LogCustom);
            }

            Raylib.InitWindow(1280, 720, "Warehouse Simulation");

            Texture2D truckTexture = Raylib.LoadTexture("textures/Truck.png");

            Stopwatch tickSW = new Stopwatch();
            tickSW.Start();

            while (!Raylib.WindowShouldClose())
            {
                bool trucksStillUnloading = false;
                if (Entrance.Count() > 0) trucksStillUnloading = true;
                foreach (var dock in Docks)
                {
                    if (dock.Line.Count() > 0)
                    {
                        trucksStillUnloading = true;
                        break;
                    }
                }

                if (tickSW.ElapsedMilliseconds >= 500 && trucksStillUnloading)
                {
                    Step();
                    tickSW.Restart();
                }
                else if (tickSW.ElapsedMilliseconds >= 500 && CurrentTime < TIME_INCREMENTS)
                {
                    Step();
                    tickSW.Restart();
                }

                if (Raylib.IsKeyPressed(KeyboardKey.KEY_RIGHT))
                {
                    Console.WriteLine($"{CurrentTime}----------------");
                    Step();
                }

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.GRAY);

                Raylib.DrawText($"Time: {CurrentTime}", 1280 - 140, 20, 32, Color.BLACK);

                var entranceTrucks = Entrance.ToArray();

                float truckX = 0;
                for (int i = 0; i < entranceTrucks.Count(); i++)
                {
                    Raylib.DrawTextureEx(truckTexture, new Vector2((1280.0f - 80.0f)/2 - truckX, 720.0f - 100.0f), 90.0f, 1.0f, Color.WHITE);
                    truckX += 20.0f;
                }

                float dockX = 0;
                for (int i = 0; i < NumberOfDocks; i++)
                {
                    string dockText = string.Empty;
                    if (Docks[i].TotalSales > 1000)
                        dockText = $"${Math.Round(Docks[i].TotalSales * 0.001f, 2)}k\n{Docks[i].Line.Count()}";
                    else
                        dockText = $"${Math.Round(Docks[i].TotalSales, 2)}\n{Docks[i].Line.Count()}";

                    var dockTextSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), dockText, 16, 1.0f);
                    var dockRectSize = new Vector2(80.0f, 80.0f);

                    Raylib.DrawRectangle(
                        (int)(45 + dockX) - (int)(dockRectSize.X/2),
                        150 - (int)(dockRectSize.Y/2),
                        (int)dockRectSize.X, (int)dockRectSize.Y, Color.BLUE);

                    Color salesTextColor = Color.GREEN;
                    if (Docks[i].TotalSales < 0.0f) salesTextColor = Color.RED;

                    Raylib.DrawTextPro(Raylib.GetFontDefault(), dockText,
                        new Vector2((int)(35 + dockX) - (dockTextSize.X/2), 150 - (dockTextSize.Y / 2)),
                        Vector2.Zero, 0.0f, 22.0f, 1.0f, salesTextColor);

                    dockX += 85.0f;
                }

                Raylib.EndDrawing();
            }

            Raylib.UnloadTexture(truckTexture);

            Raylib.CloseWindow();
        }

        private void Step()
        {
            foreach (var dock in Docks)
            {
                dock.TotalSales -= 100.0f;
            }

            int longestLine = GetDockWithLongestLine().Line.Count();
            if (longestLine > LongestDockLine) LongestDockLine = longestLine;

            // Assign trucks at front of entrance line to a dock
            for (int i = 0; i < NUM_OF_TRUCKS_TO_PROCESS; i++)
            {
                Dock dockToUse = GetDockWithShortestLine();

                if (Entrance.Count() > 0)
                {
                    dockToUse.JoinLine(Entrance.Dequeue());
                    dockToUse.TotalTrucks++;
                }
            }

            Random randy = new Random();

            if (CurrentTime < TIME_INCREMENTS)
            {
                // New trucks arrive
                float chance = ChanceFormula(CurrentTime);

                for (int i = 0; i < MAX_NEW_TRUCKS; i++)
                {
                    double diceRoll = randy.NextDouble();
                    if (chance >= diceRoll)
                    {
                        Truck newTruck = new Truck();
                        GiveRandomCrates(newTruck);
                        Console.WriteLine("New Truck:\n" + newTruck.ToString());
                        Entrance.Enqueue(newTruck);
                    }
                }
            }

            // Unload the crates
            for (int c = 0; c < NumberOfDocks; c++)
            {
                if (Docks[c].Line.Count() > 0)
                {
                    Truck truck = Docks[c].Line.Peek();
                    if (truck.Trailer.Count() > 0)
                    {
                        Crate crate = truck.Unload();
                        Docks[c].TotalCrates++;
                        Docks[c].TotalSales += crate.Price;
                        Console.WriteLine($"Crate Unloaded: {crate.ID}, ${crate.Price}\n");
                    }
                    else
                    {
                        Docks[c].SendOff();
                    }
                }
            }

            CurrentTime++;
        }

        private float ChanceFormula(int time)
        {
            if (time < 0 || time > TIME_INCREMENTS) return -1.0f;

            float chance = 0.0f;

            if (time < 24)
            {
                chance = time / 24.0f;
            }
            else if (time >= 24)
            {
                chance = (48.0f - time) / 24.0f;
            }

            return chance;
        }

        private Truck GiveRandomCrates(Truck truck)
        {
            Random randy = new Random();
            int numCrates = randy.Next(1, 11);

            for (int c = 0; c < numCrates; c++)
            {
                truck.Load(new Crate(CurrentCrateID.ToString()));
                CurrentCrateID++;
            }

            return truck;
        }

        private Dock GetDockWithShortestLine()
        {
            Dock dock = Docks[0];
            int shortestLine = dock.Line.Count();

            for (int d = 1; d < Docks.Count(); d++)
            {
                if (Docks[d].Line.Count() < shortestLine)
                {
                    shortestLine = Docks[d].Line.Count();
                    dock = Docks[d];
                }
            }

            return dock;
        }

        private Dock GetDockWithLongestLine()
        {
            Dock dock = Docks[0];
            int longestLine = dock.Line.Count();

            for (int d = 1; d < Docks.Count(); d++)
            {
                if (Docks[d].Line.Count() > longestLine)
                {
                    longestLine = Docks[d].Line.Count();
                    dock = Docks[d];
                }
            }

            return dock;
        }
    }
}
