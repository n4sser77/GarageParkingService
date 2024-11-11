using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace GarageParking
{

    // valde abstract istället för interface pga behov av konstruktur. 
    /// <summary>
    /// Base abstract class for all vehicles.
    /// </summary>
    internal abstract class Vehicle
    {
        public string LicensePlate { get; set; }
        public string Color { get; set; }
        public int ParkedAt { get; set; }
        Stopwatch sw { get; set; }

        // alla subclasser återanvänder basklassens ToString() och lägger till egna detaljer på strängen.
        public override string ToString()
        {
            return $"type: {this.GetType().Name} | Color: {Color} | Plate: {LicensePlate}";
        }


        public Vehicle(string color)
        {
            LicensePlate = Helpers.GeneratePlate();
            Color = color;
            sw = new Stopwatch();

        }

        public void StartParkingTimer()
        {
            sw.Start();
        }

        public TimeSpan StopParkingTimer(double pricePerMin)
        {
            sw.Stop();
            TimeSpan timespan = sw.Elapsed;
            double total = timespan.TotalMinutes / pricePerMin;
            Receipt r = new Receipt()
            {
                VehicleType = this.GetType().Name,
                LicensePlate = this.LicensePlate,
                ParkingSpot = this.ParkedAt,
                Price = Math.Round(total,2)
                
            };


            r.LogReceipt(r);

            return timespan;
        }


    }

    class Car : Vehicle
    {
        public bool Electric { get; set; }

        public Car(string color, bool electric)
           : base(color)
        {
            Electric = electric;
        }

        public override string ToString()
        {
            return base.ToString() + (Electric ? " | Electric" : " | Not electric");
        }

    }

    class Bus : Vehicle
    {
        public int NumberOfPassengers { get; set; }


        public Bus(string color, int numberOfPassengers)
            : base(color)
        {
            NumberOfPassengers = numberOfPassengers;
        }
        public override string ToString()
        {
            return base.ToString() + " | Passengers: " + NumberOfPassengers;
        }
    }

    class MotorCycle : Vehicle
    {
        public string Make { get; set; }


        public MotorCycle(string color, string make)
           : base(color)
        {
            Make = make;
        }

        public override string ToString()
        {
            return base.ToString() + " | Make: " + Make;
        }
    }
}
