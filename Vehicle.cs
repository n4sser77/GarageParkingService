using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace GarageParking
{
    internal abstract class Vehicle
    {
        public string LicensePlate { get; set; }
        public string Color { get; set; }
        public virtual double ParkingSpace { get; protected set; } = 1.0;
        public int ParkedAt { get; set; }
        public bool IsParked { get; set; }


        public Vehicle(string color)
        {
            LicensePlate = GetPlate();
            Color = color;

        }

        private string GetPlate()
        {
            Random random = new Random();
            int[] num = new int[3];
            char[] chars = new char[3];

            for (int i = 0; i < 3; i++)
            {
                // 65 = A - 90 = Z in ASCI
                chars[i] = (char)random.Next(65, 90); ;


            }

            for (int i = 0; i < 3; i++)
            {
                num[i] = random.Next(0, 10);
            }

            // format output "ABC123"
            return $"{chars[0]}{chars[1]}{chars[2]}{num[0]}{num[1]}{num[2]}";



        }


    }

    class Car : Vehicle
    {
        public bool Electric { get; set; }

        public Car(string color, bool electric)
           : base(color)
        {

        }

    }

    class Bus : Vehicle
    {
        public int NumberOfPassengers { get; set; }
        public override double ParkingSpace { get; protected set; } = 2.0;

        public Bus(string color, int numberOfPassengers)
            : base(color)
        {

        }
    }

    class MotorCycle : Vehicle
    {
        public string Make { get; set; }
        public override double ParkingSpace { get; protected set; } = 0.5;

        public MotorCycle(string color, string make)
           : base(color)
        {

        }
    }
}
