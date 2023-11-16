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

        private const int TIME_INCREMENTS = 48;
        private int CurrentTime = 0;

        public Warehouse()
        {
            Docks = new List<Dock>();
            Entrance = new Queue<Truck>();
        }

        public void Run()
        {
            // Note: After the 48 time increments, the trucks will stop arriving
            // TODO: Make trucks arrive according to normal distribution

            Console.Write("Enter number of docks (1-15):\n>");
            string numberOfDocksStr = Console.ReadLine();
            NumberOfDocks = int.Parse(numberOfDocksStr);

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
                //if (tickSW.ElapsedMilliseconds >= 2000)
                //{
                //    Step();
                //    tickSW.Restart();
                //}

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
                        dockText = $"${Math.Round(Docks[i].TotalSales * 0.001f, 2)}k";
                    else
                        dockText = $"${Math.Round(Docks[i].TotalSales, 2)}";
                        
                        //dockText = "$999.99";

                    var dockTextSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), dockText, 16, 1.0f);
                    var dockRectSize = new Vector2(80.0f, 80.0f);

                    Raylib.DrawRectangle(
                        (int)(45 + dockX) - (int)(dockRectSize.X/2),
                        150 - (int)(dockRectSize.Y/2),
                        (int)dockRectSize.X, (int)dockRectSize.Y, Color.BLUE);

                    Raylib.DrawTextPro(Raylib.GetFontDefault(), dockText,
                        new Vector2((int)(35 + dockX) - (dockTextSize.X/2), 150 - (dockTextSize.Y / 2)),
                        Vector2.Zero, 0.0f, 22.0f, 1.0f, Color.GREEN);

                    dockX += 85.0f;
                }

                Raylib.EndDrawing();
            }

            Raylib.UnloadTexture(truckTexture);

            Raylib.CloseWindow();
        }

        private void Step()
        {
            // Assign truck at front of entrance line to a dock
            Dock dockToUse = Docks[0];
            int shortestLine = dockToUse.Line.Count();

            for (int d = 1; d < Docks.Count(); d++)
            {
                if (Docks[d].Line.Count() < shortestLine)
                {
                    shortestLine = Docks[d].Line.Count();
                    dockToUse = Docks[d];
                }
            }

            if (Entrance.Count() > 0)
            {
                dockToUse.JoinLine(Entrance.Dequeue());
                dockToUse.TotalTrucks++;
            }

            Truck newTruck = new Truck();
            // A truck arrives
            Random randy = new Random();
            float chance = 0;
            bool truckHasArrived = false;
            double diceRoll = randy.NextDouble() * 2.0f;
            if (CurrentTime < 24)
            {
                chance = CurrentTime / 24;
                if (chance >= diceRoll) truckHasArrived = true;
            }
            else if (CurrentTime >= 24)
            {
                chance = (48 - CurrentTime) / 24;
                if (chance >= diceRoll) truckHasArrived = true;
            }

            // Give this new truck a random number of random crates
            int numCrates = randy.Next(1, 11);

            for (int c = 0; c < numCrates; c++)
            {
                newTruck.Load(new Crate(CurrentCrateID.ToString()));
                CurrentCrateID++;
            }

            Console.WriteLine("New Truck:\n" + newTruck.ToString());

            // Put the new truck at the back of the entrance line
            Entrance.Enqueue(newTruck);

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
    }
}
