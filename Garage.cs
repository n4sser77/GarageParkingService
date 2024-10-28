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

        public void park(Vehicle vehicle)
        {
            int spot = LookForSpot(vehicle);
            if (spot < 0)
            {
                Console.WriteLine("No spots found for this vehicle");
                return;
            }
            else
            {
                if (vehicle is Bus && Space[spot + 1].IsTaken)
                    Console.WriteLine("No room for bus");


                if (vehicle is MotorCycle)
                    for (int i = 0; i < Space.Count; i++)
                    {
                        for (int j = 0; j < Space[spot].Bikes.Length && Space[i].Vehicle == null; j++)
                        {
                            if (Space[i].Bikes[j] == null)
                            {
                                Space[i].Bikes[j] = vehicle as MotorCycle;
                                Space[i].IsTaken = true;
                                VehiclesParked.Add(vehicle);
                                return;
                            }
                        }
                    }

                else
                {
                    vehicle.ParkedAt = spot;
                    VehiclesParked.Add(vehicle);
                    Space[spot].IsTaken = true;
                    Space[spot].Vehicle = vehicle;
                    return;
                }





            }



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

        public void CheckOut(Vehicle v)
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
                            return;
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
                        }
                        Console.WriteLine("Bus checked out from spots " + i + " and " + (i + 1));
                    }

                    else // For cars or other vehicles occupying one spot
                    {
                        Console.WriteLine("Vehicle " + Space[i].Vehicle.LicensePlate + " is now checked out.");
                        Space[i].Vehicle = null;
                        Space[i].IsTaken = false;
                        VehiclesParked.Remove(VehiclesParked.FirstOrDefault(v));
                    }


                    found = true;
                    return;
                }
            }

            // If the vehicle was not found in any spot
            if (!found)
            {
                Console.WriteLine("Vehicle not found");
            }
        }


        private int LookForSpot(Vehicle vehicle)
        {
            for (int i = 0; i < Space.Count; i++)
            {
                if (vehicle is MotorCycle)
                {

                    return i;
                }

                if (vehicle is Bus && Space[i].IsTaken)
                {
                    continue;
                }

                if (vehicle is Car && Space[i].IsTaken)
                {
                    continue;
                }

                if (!Space[i].IsTaken)
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
                if (Space[i].Vehicle is Bus)
                {
                    Console.WriteLine($"Spot: {i} and {i + 1} - Vehicle Plate: {Space[i].Vehicle.LicensePlate} | Type: Bus (occupies two spots)");
                    i++; // Skip the next spot since it's occupied by the same bus
                    continue;
                }

                // Check and print motorcycles in the Bikes array
                if (Space[i].Bikes[0] != null || Space[i].Bikes[1] != null)
                {
                    Console.WriteLine($"Spot: {i} contains motorcycles:\n");
                    foreach (var bike in Space[i].Bikes)
                    {
                        if (bike != null)
                        {
                            Console.WriteLine($"Motorcycle - Plate: {bike.LicensePlate} | Type: {bike.GetType().Name}");
                        }
                    }
                }

                // Print other vehicle types
                if (Space[i].Vehicle != null && !(Space[i].Vehicle is Bus))
                {
                    Console.WriteLine($"Spot: {i} - Vehicle Plate: {Space[i].Vehicle.LicensePlate} | Type: {Space[i].Vehicle.GetType().Name}");
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
