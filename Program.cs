using System.Data;
using System.Diagnostics;
using System.Net;

namespace GarageParking
{
    internal class Program
    {
        static void Main(string[] args)
        {


            Garage garage = new Garage();

            MyServer myserver = new MyServer(garage);

            Thread thread2 = new Thread(() => UpdateTotal(garage));


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

                if (key == ConsoleKey.P)
                {
                    Console.Write("Enter plate to park: ");
                    while (true)
                    {

                        string input = Console.ReadLine();
                        if (input.ToUpper() == v.LicensePlate && garage.park(v))
                        {
                            Console.WriteLine("Vehicle parked successfully! ");
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Could not park vehicle...");
                        }
                        if (Console.ReadKey().Key == ConsoleKey.X)
                        {
                            break;
                        }
                    }

                }
                if (key == ConsoleKey.C)
                {
                    Console.Write("Enter plate to checkout: ");
                    Vehicle c = Helpers.GetVehicleFromGaragge(garage);
                    if (c == null)
                    {
                        Console.WriteLine("Vehcile not found...");
                        continue;
                    }
                    if (garage.CheckOut(c)) Console.WriteLine(c.ToString() + " Checked out successfully");
                    else Console.WriteLine("Could not checkout...");

                }
                //var v = Helpers.GetPlate(garage);

                Thread.Sleep(3000);


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



                Thread.Sleep(1000);

            }
        }

    }



}
