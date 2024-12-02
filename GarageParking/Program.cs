using System.Data;
using System.Diagnostics;
using System.Net;

namespace GarageParking
{
    internal class Program
    {
        async static Task Main(string[] args)
        {


            Garage garage = new Garage();

            MyServer myserver = new MyServer(garage);

            Thread thread2 = new Thread(() => UpdateTotal(garage));

            thread2.Start();


            myserver.StartServer();

            StartConsoleInstance(garage);


        }






        static void StartConsoleInstance(Garage garage)
        {
            Console.SetWindowSize(140, 30);

            // intially parked vehicles
            garage.park(NewVehicleArival());
            garage.park(NewVehicleArival());
            garage.park(NewVehicleArival());
            garage.park(NewVehicleArival());
            garage.park(NewVehicleArival());
            garage.park(NewVehicleArival());
            garage.park(NewVehicleArival());
            garage.park(NewVehicleArival());
            garage.park(NewVehicleArival());
            garage.park(NewVehicleArival());
            garage.park(NewVehicleArival());




            while (true)
            {

                PrintNavagtion();
                Vehicle v = NewVehicleArival();

                garage.PrintGarageSET();


                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.P:

                        while (true)
                        {
                            (int l, int t) = Console.GetCursorPosition();
                            Console.SetCursorPosition(l, t);
                            Console.Write("Enter plate to park: ");
                            string input = Console.ReadLine();

                            if (input == "x")
                            {
                                break;
                            }

                            // Check if input matches the license plate and parking is successful
                            if (input.ToUpper() == v.LicensePlate)
                            {
                                if (garage.park(v))
                                {

                                    Console.WriteLine("Vehicle parked successfully!");
                                }
                                else
                                {
                                    break;
                                }
                                break;
                            }
                            else if (input.ToUpper() != v.LicensePlate)
                            {
                                // Get the current cursor position after the input
                                (int left, int top) = Console.GetCursorPosition();

                                // Display error message on the same line
                                Console.SetCursorPosition(0, top - 1);  // Move cursor up to overwrite the line with the prompt
                                Console.Write(new string(' ', Console.WindowWidth));  // Clear the line
                                Console.SetCursorPosition(0, top - 1);  // Set cursor back to start of cleared line
                                Console.Write("Could not park vehicle... ");

                                // Pause for feedback, then reset line for new input
                                Thread.Sleep(2000);
                                Console.SetCursorPosition(0, top - 1);  // Position cursor again to overwrite with "Try again"
                                Console.Write(new string(' ', Console.WindowWidth));  // Clear line again
                                Console.SetCursorPosition(0, top - 1);
                                Console.Write("Try again, or type X to exit park mode");
                            }





                        }
                        break;
                    case ConsoleKey.C:
                        Console.Write("Enter plate to checkout: ");
                        Vehicle c = Helpers.GetVehicleFromGaragge(garage);
                        if (c == null)
                        {
                            Console.Write("Vehcile not found...");
                            Thread.Sleep(2000);
                            (int left, int top) = Console.GetCursorPosition();
                            Console.SetCursorPosition(0, top - 1);
                            Console.WriteLine("                                                                      ");
                            Console.WriteLine("                                                                      ");
                            continue;
                        }
                        if (garage.CheckOut(c)) Console.WriteLine(c.ToString() + " Checked out successfully");
                        else Console.WriteLine("Could not checkout...");
                        break;


                    default:

                        Console.Write("skipping intervall......");
                        Thread.Sleep(200);

                        Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
                        Console.Write(new string(' ', Console.WindowWidth));
                        continue;


                }

                //var v = Helpers.GetPlate(garage);
                Console.WriteLine("skipping intervall......");
                Thread.Sleep(4000);


                Console.Clear();


            }

        }

        static Vehicle NewVehicleArival()
        {
            Vehicle v = Helpers.RandomVehicle();
            Console.SetCursorPosition(0, 1);

            for (int i = 0; i < 130; i++)
            {
                Console.Write("   ");
            }
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("vehicle arrived: ");
            for (int i = 0; i < 100; i++)
            {
                Console.Write(" ");
            }
            Console.SetCursorPosition(0, 1);
            if (v is Car car) Console.Write(car.ToString());
            if (v is Bus bus) Console.Write(bus.ToString());
            if (v is MotorCycle bike) Console.Write(bike.ToString());
            Console.WriteLine("\n");
            return v;
        }

        static void PrintNavagtion()
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(" Press ");
            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" P ");
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(" to park vehicles");

            Console.Write(", Press ");
            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" C ");
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(" to checkout");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(", Press ");
            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" X ");
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(" to exit park mode");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(", Press any key to skip interval ");

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }


        static public void UpdateTotal(Garage garage)
        {
            while (true)
            {


                foreach (Space s in garage.Space)
                {
                    if (s.Vehicle != null)
                    {
                        double total = s.Vehicle.sw.Elapsed.TotalMinutes / garage.PricePerMin;
                        s.Vehicle.Total = Math.Round(total, 2);
                    }

                    if (s.Bikes[0] != null)
                    {
                        double total = s.Bikes[0].sw.Elapsed.TotalMinutes / garage.PricePerMin;
                        (s.Bikes[0] as Vehicle).Total = Math.Round(total, 2);
                    }

                    if (s.Bikes[1] != null)
                    {
                        double total = s.Bikes[1].sw.Elapsed.TotalMinutes / garage.PricePerMin;
                        (s.Bikes[1] as Vehicle).Total = Math.Round(total, 2);
                    }


                }
                Thread.Sleep(new TimeSpan(0, 1, 0));





            }



        }


        async static public Task UpdateTotalAsync(Garage garage)
        {
            while (true)
            {


                foreach (Space s in garage.Space)
                {
                    if (s.Vehicle != null)
                    {
                        double total = s.Vehicle.sw.Elapsed.TotalMinutes / garage.PricePerMin;
                        s.Vehicle.Total = Math.Round(total, 2);
                    }

                    if (s.Bikes[0] != null)
                    {
                        double total = s.Bikes[0].sw.Elapsed.TotalMinutes / garage.PricePerMin;
                        (s.Bikes[0] as Vehicle).Total = Math.Round(total, 2);
                    }

                    if (s.Bikes[1] != null)
                    {
                        double total = s.Bikes[1].sw.Elapsed.TotalMinutes / garage.PricePerMin;
                        (s.Bikes[1] as Vehicle).Total = Math.Round(total, 2);
                    }

                }





            }

        }



    }
}
