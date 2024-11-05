using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace GarageParking
{
    internal class Garage
    {

        public double MaxParkingSpaces { get; private set; } = 15;
        public double PricePerMin { get; private set; } = 1.5;


        public List<Space> Space { get; private set; }
        public List<Vehicle> VehiclesParked { get; private set; }

        public Garage()
        {

            VehiclesParked = new List<Vehicle>();

            Space = new List<Space>();
            for (int i = 0; i < 15; i++)
            {
                Space.Add(new Space());
            }

        }

        public bool park(Vehicle vehicle)
        {
            int spot = LookForSpot(vehicle);
            if (spot < 0)
            {
                Console.WriteLine("No spots found for this vehicle");
                return false;
            }


            if (vehicle is Bus && Space[spot + 1].IsTaken)
            {
                Console.WriteLine("No room for bus");
                return false;
            }
            else if (vehicle is Bus && !Space[spot + 1].IsTaken)
            {
                vehicle.ParkedAt = spot;
                VehiclesParked.Add(vehicle);
                Space[spot].IsTaken = true;
                Space[spot + 1].IsTaken = true;
                Space[spot].Vehicle = vehicle;
                return true;
            }

            if (vehicle is MotorCycle)
            {
                for (int j = 0; j < Space[spot].Bikes.Length && Space[spot].Vehicle == null; j++)
                {
                    if (Space[spot].Bikes[j] == null)
                    {
                        Space[spot].Bikes[j] = vehicle as MotorCycle;
                        Space[spot].IsTaken = true;
                        VehiclesParked.Add(vehicle);

                        return true;
                    }
                }

                return false;
            }


            if (vehicle is Car && !Space[spot].IsTaken)
            {
                vehicle.ParkedAt = spot;
                VehiclesParked.Add(vehicle);
                Space[spot].IsTaken = true;
                Space[spot].Vehicle = vehicle;
                return true;
            }

            return false;

        }

        public Vehicle FindPlate(string input)
        {
            // Convert input to uppercase once to avoid repeated conversions in the loop
            string upperInput = input.ToUpper();

            for (int i = 0; i < Space.Count; i++)
            {
                // Check if the current spot has a Vehicle
                if (Space[i].Bikes[0] != null && Space[i].Bikes[0].LicensePlate == upperInput)
                {
                    return Space[i].Bikes[0];
                }
                else if (Space[i].Bikes[1] != null && Space[i].Bikes[1].LicensePlate == upperInput)
                {
                    return Space[i].Bikes[1];
                }


                if (Space[i].Vehicle != null && Space[i].Vehicle.LicensePlate == upperInput)
                {

                    return Space[i].Vehicle; // Return the Vehicle if LicensePlate matches
                }
                else if (Space[i]?.Vehicle == null)
                {
                    continue;
                }
            }

            // Return null if no matching vehicle is found

            return null;
        }

        public bool CheckOut(Vehicle v)
        {
            bool found = false;

            for (int i = 0; i < Space.Count; i++)
            {


                // Check if it's a motorcycle and if it’s parked in the Bikes array
                if (v is MotorCycle)
                {
                    // Loop through Bikes array to find and remove the motorcycle
                    for (int j = 0; j < Space[i].Bikes.Length; j++)
                    {
                        if (Space[i].Bikes[j] == v as MotorCycle)
                        {
                            Space[i].Bikes[j] = null; // Remove the motorcycle from this spot
                            Console.WriteLine("Motorcycle checked out from spot " + i);
                            found = true;
                            if (Space[i].Bikes[0] == null && Space[i].Bikes[1] == null)
                            {
                                Space[i].IsTaken = false;
                            }

                            VehiclesParked.Remove(VehiclesParked.FirstOrDefault(v));
                            return true;
                        }
                    }
                }
                else if (Space[i].Vehicle == v) // For other types of vehicles
                {
                    // Check if it's a bus occupying two consecutive spots
                    if (v is Bus)
                    {
                        Space[i].Vehicle = null; // Clear the first spot of the bus
                        Space[i].IsTaken = false;

                        if (i + 1 < Space.Count && Space[i + 1].Vehicle == v)
                        {
                            Space[i + 1].Vehicle = null; // Clear the second spot of the bus
                            Space[i + 1].IsTaken = false;
                            VehiclesParked.Remove(VehiclesParked.FirstOrDefault(v));
                            Console.WriteLine("Bus checked out from spots " + i + " and " + (i + 1));
                            return true;
                        }
                    }

                    else // For cars or other vehicles occupying one spot
                    {
                        Console.WriteLine("Vehicle " + Space[i].Vehicle.LicensePlate + " is now checked out.");
                        Space[i].Vehicle = null;
                        Space[i].IsTaken = false;
                        VehiclesParked.Remove(VehiclesParked.FirstOrDefault(v));
                        return true;
                    }


                    found = true;
                    return true;
                }
            }

            // If the vehicle was not found in any spot
            if (!found)
            {
                Console.WriteLine("Vehicle not found");
                return false;
            }
            return false;
        }


        private int LookForSpot(Vehicle vehicle)
        {
            for (int i = 0; i < Space.Count; i++)
            {


                if ((vehicle is MotorCycle) && (Space[i].Vehicle == null && !Space[i].IsTaken) && ((Space[i].Bikes[0] == null) || (Space[i].Bikes[1] == null)))
                {

                    return i;
                }

                if ((vehicle is Bus) && (!Space[i].IsTaken) && (!Space[i + 1].IsTaken))
                {
                    return i;
                }
                if (vehicle is Car && !Space[i].IsTaken)
                {
                    return i;
                }


            }
            return -1;

        }



        public void PrintGarage()
        {
            for (int i = 0; i < Space.Count; i++)
            {


                // Check if the spot is empty
                if (Space[i].Vehicle == null && (Space[i].Bikes == null || Space[i].Bikes.All(b => b == null)))
                {
                    Console.WriteLine($"Spot: {i} is empty.");
                    continue;
                }

                // Handle buses occupying two spots
                if (Space[i].Vehicle is Bus bus)
                {
                    Console.WriteLine($"Spot: {i} and {i + 1} - Vehicle Plate: {bus.LicensePlate} | Type: Bus | {bus.Color} | {bus.NumberOfPassengers} ");
                    i++; // Skip the next spot since it's occupied by the same bus
                    continue;
                }

                // Check and print motorcycles in the Bikes array
                if (Space[i].Bikes[0] != null || Space[i].Bikes[1] != null)
                {
                    Console.WriteLine($"Spot: {i} contains motorcycles:");
                    foreach (var bike in Space[i].Bikes)
                    {
                        if (bike != null)
                        {
                            Console.WriteLine($" - Plate: {bike.LicensePlate} | Type: {bike.GetType().Name} |Color: {bike.Color} | Make: {bike.Make}");
                        }
                    }
                }

                // cars
                if (Space[i].Vehicle != null && (Space[i].Vehicle is Car car))
                {
                    string s;
                    s = car.Electric ? "Electric" : "Not electric";
                    Console.WriteLine($"Spot: {i} - Vehicle Plate: {car.LicensePlate} | Type: {car.GetType().Name} | Color: {car.Color} | {s} ");
                }
            }
        }






    }


    class Space
    {
        public bool IsTaken { get; set; }

        public Vehicle Vehicle { get; set; }

        public MotorCycle[] Bikes { get; set; }

        public Space()
        {
            IsTaken = false;
            Bikes = new MotorCycle[2];

        }

    }
}
