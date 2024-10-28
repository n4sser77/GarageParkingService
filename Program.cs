using System.Data;

namespace GarageParking
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Car car = new Car("red", true);
            Bus bus = new Bus("white", 8);
            MotorCycle bike = new MotorCycle("black", "chevy");
            MotorCycle bike2 = new MotorCycle("yello", "strand");

            Garage garage = new Garage();




            garage.park(car);
            garage.park(new Car("blue", true));
            garage.park(bike);
            garage.park(bike2);
            garage.park(bus);

            while (true)
            {
                Console.Clear();
                garage.PrintGarage();
                var v = Helpers.GetPlate(garage);
                garage.CheckOut(v);
                Thread.Sleep(2000);



            }



        }


    }
}
