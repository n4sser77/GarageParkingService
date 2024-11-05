using System.Data;

namespace GarageParking
{
    internal class Program
    {
        static void Main(string[] args)
        {


            Garage garage = new Garage();




            while (true)
            {

                Vehicle v = NewVehicleArival();

                garage.PrintGarage();

                ConsoleKey key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.P)
                {
                    Console.Write("Enter plate to park: ");
                    string input = Console.ReadLine();
                    if (input.ToUpper() == v.LicensePlate && garage.park(v))
                    {
                        Console.WriteLine("Vehicle parked successfully! ");
                    }
                    else
                    {
                        Console.WriteLine("Could not park vehicle...");
                    }

                }
                if (key == ConsoleKey.C)
                {
                    Console.Write("Enter plate to checkout: ");
                    Vehicle c = Helpers.GetPlate(garage);
                    if(garage.CheckOut(c)) Console.WriteLine(c.ToString() + " Checked out successfully");
                    else Console.WriteLine("Could not checkout...");

                }
                //var v = Helpers.GetPlate(garage);

                Thread.Sleep(3000);



            }



        }

        static Vehicle NewVehicleArival()
        {
            Vehicle v = Helpers.RandomVehicle();
            Console.Clear();
            Console.WriteLine("vehicle arrived: ");
            if (v is Car car) Console.Write(car.ToString());
            if (v is Bus bus) Console.Write(bus.ToString());
            if (v is MotorCycle bike) Console.Write(bike.ToString());
            Console.WriteLine("\n");
            return v;
        }


    }
}
