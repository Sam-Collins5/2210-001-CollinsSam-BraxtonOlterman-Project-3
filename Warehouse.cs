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

        private int MaxNewTrucks = 3;
        private int NumOfTrucksToProcess = 2;

        private const int TIME_INCREMENTS = 48;
        private int CurrentTime = 0;
        private int TickIntervalMS = 0;

        private StringBuilder SimulationDataSB;
        private StringBuilder CrateDataSB;
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
            CrateDataSB = new StringBuilder();
            CratePrices = new List<double>();
            TruckPrices = new List<double>();
        }

        public void Run()
        {
            // Prompt user for number of docks
            {
                bool inputValid = false;
                while (!inputValid)
                {
                    Console.Write("Enter number of docks (1-15):\n>");
                    string numberOfDocksStr = Console.ReadLine();

                    try
                    {
                        NumberOfDocks = int.Parse(numberOfDocksStr);
                        if (NumberOfDocks > 0 && NumberOfDocks <= 15)
                        {
                            inputValid = true;
                            break;
                        }
                        Console.Write("Invalid input, try again.\n\n");
                    }
                    catch
                    {
                        Console.Write("Invalid input, try again.\n\n");
                    }
                }
            }

            // Prompt user for simulation speed
            {
                bool inputValid = false;
                while (!inputValid)
                {
                    Console.Write("Enter simulation step speed (ms) (enter nothing to use default value 2000):\n>");
                    string tickIntervalStr = Console.ReadLine();

                    if (string.IsNullOrEmpty(tickIntervalStr))
                    {
                        TickIntervalMS = 2000;
                        inputValid = true;
                        break;
                    }

                    try
                    {
                        TickIntervalMS = int.Parse(tickIntervalStr);
                        if (TickIntervalMS > -1)
                        {
                            inputValid = true;
                            break;
                        }
                        Console.Write("Invalid input, try again.\n\n");
                    }
                    catch
                    {
                        Console.Write("Invalid input, try again.\n\n");
                    }
                }
            }

            // Prompt user for the maximum amount of trucks that can arrive at once
            {
                bool inputValid = false;
                while (!inputValid)
                {
                    Console.Write("Enter maximum number of trucks that can arrive at once (enter nothing to use default value 3):\n>");
                    string maxNewTrucksStr = Console.ReadLine();

                    if (string.IsNullOrEmpty(maxNewTrucksStr))
                    {
                        MaxNewTrucks = 3;
                        inputValid = true;
                        break;
                    }

                    try
                    {
                        MaxNewTrucks = int.Parse(maxNewTrucksStr);
                        if (MaxNewTrucks > 0)
                        {
                            inputValid = true;
                            break;
                        }
                        Console.Write("Invalid input, try again.\n\n");
                    }
                    catch
                    {
                        Console.Write("Invalid input, try again.\n\n");
                    }
                }
            }

            // Prompt user for number of trucks that can be processed at the entrance at once
            {
                bool inputValid = false;
                while (!inputValid)
                {
                    Console.Write("Enter number of trucks that can be processed at the entrance at the entrance at once (enter nothing to use default value 2):\n>");
                    string numOfTrucksToProcessStr = Console.ReadLine();

                    if (string.IsNullOrEmpty(numOfTrucksToProcessStr))
                    {
                        NumOfTrucksToProcess = 2;
                        inputValid = true;
                        break;
                    }

                    try
                    {
                        NumOfTrucksToProcess = int.Parse(numOfTrucksToProcessStr);
                        if (NumOfTrucksToProcess > 0)
                        {
                            inputValid = true;
                            break;
                        }
                        Console.Write("Invalid input, try again.\n\n");
                    }
                    catch
                    {
                        Console.Write("Invalid input, try again.\n\n");
                    }
                }
            }

            TimesThatDocksInUse = new int[NumberOfDocks];
            TimesThatDocksNotInUse = new int[NumberOfDocks];

            // Make the number of desired docks and assign each of them a unique ID
            for (int i = 0; i < NumberOfDocks; i++)
            {
                Docks.Add(new Dock(CurrentDockID.ToString()));
                CurrentDockID++;
            }

            // Disable raylib logger
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

                if (tickSW.ElapsedMilliseconds >= TickIntervalMS && trucksStillUnloading)
                {
                    Console.WriteLine($"{CurrentTime}----------------");
                    Step();
                    tickSW.Restart();
                }
                else if (tickSW.ElapsedMilliseconds >= TickIntervalMS && CurrentTime < TIME_INCREMENTS)
                {
                    Console.WriteLine($"{CurrentTime}----------------");
                    Step();
                    tickSW.Restart();
                }

                // Export all simulation data when the simulation is finished
                if (!trucksStillUnloading && CurrentTime > TIME_INCREMENTS)
                {
                    ExportResults();
                    ExportLoggedCrates();
                    return;
                }

                if (Raylib.IsKeyPressed(KeyboardKey.KEY_RIGHT))
                {
                    Console.WriteLine($"{CurrentTime}----------------");
                    Step();
                }

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.GRAY);

                // Draw current simulation time in the top right corner of the window
                Raylib.DrawText($"Time: {CurrentTime}", 1280 - 140, 20, 32, Color.BLACK);

                // Draw warehouse entrance
                Raylib.DrawRectangle((1280 - 100) - (120/2), (720 - 80) - (120/2), 120, 120, Color.BLACK);
                Raylib.DrawText("Entrance", 1280 - 150, 720 - 95, 22, Color.WHITE);

                // Draw trucks waiting at the entrance
                var entranceTrucks = Entrance.ToArray();
                float truckX = 0.0f;
                int numOfEntranceTrucksToDraw = entranceTrucks.Count() <= 22 ? entranceTrucks.Count() : 22;
                for (int i = 0; i < entranceTrucks.Count(); i++)
                {
                    Raylib.DrawTextureEx(truckTexture, new Vector2((1280.0f - 180.0f) - truckX, 720.0f - 100.0f), 90.0f, 1.0f, Color.WHITE);
                    truckX += 50.0f;
                }

                // Draw docks
                float dockX = 0;
                for (int i = 0; i < NumberOfDocks; i++)
                {
                    // Draw dock rectangle
                    var dockRectSize = new Vector2(80.0f, 80.0f);
                    int dockRectPosX = (int)(45 + dockX) - (int)(dockRectSize.X / 2);
                    int dockRectPosY = 120 - (int)(dockRectSize.Y / 2);

                    Raylib.DrawRectangle(
                        dockRectPosX, dockRectPosY,
                        (int)dockRectSize.X, (int)dockRectSize.Y, Color.BLUE);

                    float dockTruckPosX = (45 + dockX) - (truckTexture.Width / 2);
                    float dockTruckPosY = 120 - (int)(dockRectSize.Y / 2);
                    float dockTruckYOffset = 0.0f;
                    // Draw trucks that are in line, but draw 9 at maximum
                    int numOfTrucksToDraw = Docks[i].Line.Count() <= 9 ? Docks[i].Line.Count() : 9;
                    for (int t = 0; t < numOfTrucksToDraw; t++)
                    {
                        Raylib.DrawTextureEx(
                            truckTexture,
                            new Vector2(dockTruckPosX, dockTruckPosY + (85.0f + dockTruckYOffset)),
                            0.0f, 1.0f, Color.WHITE);
                        dockTruckYOffset += 50.0f;
                    }

                    // Draw dock revenue text
                    string dockRevenueText = string.Empty;
                    if (Docks[i].TotalSales > 1000)
                        dockRevenueText = $"${Math.Round(Docks[i].TotalSales * 0.001f, 2)}k\n";
                    else
                        dockRevenueText = $"${Math.Round(Docks[i].TotalSales, 2)}\n";

                    var dockRevenueTextSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), dockRevenueText, 16, 1.0f);

                    Color salesTextColor = Color.GREEN;
                    if (Docks[i].TotalSales < 0.0f) salesTextColor = Color.RED;

                    Raylib.DrawTextPro(Raylib.GetFontDefault(), dockRevenueText,
                        new Vector2((int)(35 + dockX) - (dockRevenueTextSize.X/2), 120 - (dockRevenueTextSize.Y / 2)),
                        Vector2.Zero, 0.0f, 22.0f, 1.0f, salesTextColor);

                    // Draw dock line text
                    string dockLineText = $"\nLine: {Docks[i].Line.Count()}";

                    var dockLineTextSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), dockLineText, 16, 1.0f);

                    Raylib.DrawTextPro(Raylib.GetFontDefault(), dockLineText,
                        new Vector2((int)(35 + dockX) - (dockLineTextSize.X / 2), 120 - (dockLineTextSize.Y / 2)),
                        Vector2.Zero, 0.0f, 22.0f, 1.0f, Color.WHITE);

                    dockX += 85.0f;
                }

                Raylib.EndDrawing();
            }

            Raylib.UnloadTexture(truckTexture);

            Raylib.CloseWindow();
        }

        /// <summary>
        /// Advances the warehouse simulatio by one "step".
        /// </summary>
        private void Step()
        {
            for (int i = 0; i < Docks.Count(); i++)
            {
                Docks[i].TotalSales -= 100.0f;
                if (Docks[i].Line.Count() > 0)
                {
                    TimesThatDocksInUse[i]++;
                }
                else
                {
                    TimesThatDocksNotInUse[i]++;
                }
            }

            int longestLine = GetDockWithLongestLine().Line.Count();
            if (longestLine > LongestDockLine) LongestDockLine = longestLine;

            // Assign trucks at front of entrance line to a dock
            for (int i = 0; i < NumOfTrucksToProcess; i++)
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

                for (int i = 0; i < MaxNewTrucks; i++)
                {
                    double diceRoll = randy.NextDouble();
                    if (chance >= diceRoll)
                    {
                        Truck newTruck = new Truck();
                        GiveRandomCrates(newTruck);
                        TruckPrices.Add(GetTruckValue(newTruck));
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
                        CratePrices.Add(crate.Price);

                        string scenario = string.Empty;
                        if (truck.Trailer.Count() > 0)
                        {
                            scenario = "The crate was unloaded but the truck had more to unload.";
                        }
                        else if (truck.Trailer.Count() == 0 && Docks[c].Line.Count() > 0)
                        {
                            scenario = "The crate was unloaded and the truck had no more crates but there was another truck in line behind it.";
                        }
                        else if (truck.Trailer.Count() == 0 && Docks[c].Line.Count() == 0)
                        {
                            scenario = "The crate was unloaded and the truck had no more crates and there were no trucks in line behind it.";
                        }

                        LogCrate(crate, truck, CurrentTime, scenario);
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

        private double GetTruckValue(Truck truck)
        {
            double totalValue = 0.0f;
            foreach (var crate in truck.Trailer)
            {
                totalValue += crate.Price;
            }
            return totalValue;
        }

        private void LogCrate(Crate crate, Truck truck, int time, string scenario)
        {
            CrateDataSB.Append($"{time},{truck.driver},{truck.deliveryCompany},{crate.ID},{crate.Price},{scenario}\n");
        }

        private void ExportLoggedCrates()
        {
            // Prompt user for file path
            string crateDataFilePath = string.Empty;
            {
                Console.Write("Enter file path for crate data (enter nothing to use default):\n>");
                crateDataFilePath = Console.ReadLine();

                if (string.IsNullOrEmpty(crateDataFilePath))
                {
                    crateDataFilePath = "CrateData.csv";
                }
            }

            // CurrentCrateID is given to avoid the empty string that comes
            // after the last crate string is split on its \n
            string[] loggedCrates = CrateDataSB.ToString().Split('\n', CurrentCrateID);

            string[] unloadedTimes = new string[loggedCrates.Length];
            string[] drivers = new string[loggedCrates.Length];
            string[] companies = new string[loggedCrates.Length];
            string[] crateIDs = new string[loggedCrates.Length];
            string[] values = new string[loggedCrates.Length];
            string[] scenarios = new string[loggedCrates.Length];

            for (int i = 0; i < loggedCrates.Length; i++)
            {
                string[] cratefields = loggedCrates[i].Split(',');
                unloadedTimes[i] = cratefields[0];
                drivers[i] = cratefields[1];
                companies[i] = cratefields[2];
                crateIDs[i] = cratefields[3];
                values[i] = cratefields[4];
                scenarios[i] = cratefields[5];
            }

            StringBuilder fileContent = new StringBuilder();
            fileContent.Append("Time Unloaded,Driver,Company,ID,Value,Scenario\n");
            for (int i = 0; i < loggedCrates.Length; i++)
            {
                fileContent.Append($"{unloadedTimes[i]},{drivers[i]},{companies[i]},{crateIDs[i]},${values[i]},{scenarios[i]}\n");
            }

            Utils.WriteToFile(crateDataFilePath, fileContent.ToString());
        }

        private void ExportResults()
        {
            // Prompt user for file path
            string resultsFilePath = string.Empty;
            {
                Console.Write("Enter file path for simulation results (enter nothing to use default):\n>");
                resultsFilePath = Console.ReadLine();

                if (string.IsNullOrEmpty(resultsFilePath))
                {
                    resultsFilePath = "Simulation Results.txt";
                }
            }

            SimulationDataSB.Append($"Number of Docks: {NumberOfDocks}\n");
            SimulationDataSB.Append($"Longest Line: {LongestDockLine}\n");

            int totalTrucks = 0;
            foreach (var dock in Docks)
            {
                totalTrucks += dock.TotalTrucks;
            }

            SimulationDataSB.Append($"Trucks Processed: {totalTrucks}\n");
            SimulationDataSB.Append($"Crates Unloaded: {CurrentCrateID + 1}\n");

            double totalCrateValue = 0.0f;
            foreach (var price in CratePrices)
            {
                totalCrateValue += price;
            }

            SimulationDataSB.Append($"Total Crate Value: ${Math.Round(totalCrateValue, 2)}\n");
            SimulationDataSB.Append($"Average Crate Value: ${Math.Round(totalCrateValue / CratePrices.Count, 2)}\n");

            double totalTruckValue = 0.0f;
            foreach (var price in TruckPrices)
            {
                totalTruckValue += price;
            }

            SimulationDataSB.Append($"Average Truck Value: ${Math.Round(totalTruckValue / TruckPrices.Count, 2)}\n");
            SimulationDataSB.Append($"Times Docks Were in Use:\n");

            for (int i = 0; i < NumberOfDocks; i++)
            {
                SimulationDataSB.Append($"  Dock {i + 1}: {TimesThatDocksInUse[i]}\n");
            }

            SimulationDataSB.Append($"Times Docks Were not in Use:\n");

            for (int i = 0; i < NumberOfDocks; i++)
            {
                SimulationDataSB.Append($"  Dock {i + 1}: {TimesThatDocksNotInUse[i]}\n");
            }

            float totalTimeDocksInUse = 0.0f;
            foreach (var time in TimesThatDocksInUse)
            {
                totalTimeDocksInUse += time;
            }

            SimulationDataSB.Append($"Average Time Docks were in Use: {Math.Round(totalTimeDocksInUse / TimesThatDocksInUse.Count(), 2)}\n");
            SimulationDataSB.Append($"Total Dock Operation Cost: ${Math.Round((NumberOfDocks * 100.0f) * CurrentTime, 2)}\n");
            SimulationDataSB.Append($"Total Warehouse Revenue: ${Math.Round(totalCrateValue - ((NumberOfDocks * 100.0f) * CurrentTime), 2)}");

            Utils.WriteToFile(resultsFilePath, SimulationDataSB.ToString());
        }
    }
}
