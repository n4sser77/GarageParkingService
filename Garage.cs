using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace GarageParking
{
    public class Garage
    {

        public double MaxParkingSpaces { get; private set; } = 15;
        public double PricePerMin { get; private set; } = 1.5;


        public List<Space> Space { get; private set; }
        public bool IsFull { get; set; }


        public List<Vehicle> VehiclesParked { get; private set; }   // for stats and debugging only


        public Garage()
        {

            VehiclesParked = new List<Vehicle>();

            Space = new List<Space>();
            for (int i = 0; i < MaxParkingSpaces; i++)
            {
                Space.Add(new Space());
            }

        }

        public bool park(Vehicle vehicle)
        {
            IsFull = IsGarageFull();
            int spot = LookForSpot(vehicle);
            if (spot < 0)
            {
                Console.Write("No spots found for this vehicle");
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
                Thread.Sleep(2000);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top-1);
                Console.Write(new string(' ', Console.WindowWidth));
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
                Space[spot + 1].Vehicle = vehicle;
                Space[spot].VehicleType = vehicle.GetType().Name;
                Space[spot + 1].VehicleType = vehicle.GetType().Name;

                vehicle.StartParkingTimer();
                return true;
            }

            if (vehicle is MotorCycle)
            {
                for (int j = 0; j < Space[spot].Bikes.Length; j++)
                {
                    if (Space[spot].Bikes[j] == null)
                    {
                        Space[spot].Bikes[j] = vehicle as MotorCycle;
                        Space[spot].Bikes[j].ParkedAt = spot;
                        Space[spot].IsTaken = true;
                        VehiclesParked.Add(vehicle);
                        Space[spot].VehicleType = vehicle.GetType().Name;

                        vehicle.StartParkingTimer();
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
                Space[spot].VehicleType = vehicle.GetType().Name;

                vehicle.StartParkingTimer();

                return true;
            }

            return false;

        }

        public Vehicle FindPlate(string input)
        {

            if (input == null)
            {
                return null;
            }
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

        public bool CheckOut(Vehicle v, out string message)
        {
            bool found = false;
            IsFull = IsGarageFull();

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
                            double total = ((double)v.StopParkingTimer(PricePerMin).TotalMinutes) / PricePerMin;
                            v.Total = total;
                            Space[i].Bikes[j] = null; // Remove the motorcycle from this spot
                            Console.WriteLine("Vehicle " + v.LicensePlate + " is now checked out." + " Total price is " + total.ToString("0.00") + "SEK");
                            message = "Vehicle " + v.LicensePlate + " is now checked out." + " Total price is " + total.ToString("0.00") + "SEK";
                            found = true;
                            if (Space[i].Bikes[0] == null && Space[i].Bikes[1] == null)
                            {
                                Space[i].IsTaken = false;
                            }

                            VehiclesParked.Remove(VehiclesParked.FirstOrDefault(v));
                            IsFull = IsGarageFull();
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
                            double total = ((double)v.StopParkingTimer(PricePerMin).TotalMinutes) / PricePerMin;
                            v.Total = total;
                            Console.WriteLine("Vehicle " + v.LicensePlate + " is now checked out." + " Total price is " + total.ToString("0.00") + "SEK");
                            message = "Vehicle " + v.LicensePlate + " is now checked out." + " Total price is " + total.ToString("0.00") + "SEK";
                            Space[i + 1].Vehicle = null; // Clear the second spot of the bus
                            Space[i + 1].IsTaken = false;
                            VehiclesParked.Remove(VehiclesParked.FirstOrDefault(v));
                            IsFull = IsGarageFull();
                            return true;
                        }
                    }

                    else if (v is Car) // For cars or other vehicles occupying one spot
                    {

                        double total = ((double)v.StopParkingTimer(PricePerMin).TotalMinutes) / PricePerMin;
                        v.Total = total;
                        Console.WriteLine("Vehicle " + v.LicensePlate + " is now checked out." + " Total price is " + total.ToString("0.00") + "SEK");
                        message = "Vehicle " + v.LicensePlate + " is now checked out." + " Total price is " + total.ToString("0.00") + "SEK";
                        Space[i].Vehicle = null;
                        Space[i].IsTaken = false;
                        VehiclesParked.Remove(VehiclesParked.FirstOrDefault(v));
                        IsFull = IsGarageFull();
                        return true;
                    }


                    message = "Could not successfully checkout vehicle";
                    return false;
                }
            }

            // If the vehicle was not found in any spot
            if (!found)
            {
                Console.WriteLine("Vehicle not found");
                message = "Vehicle not found";
                return false;
            }
            message = "Not found";
            return false;
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
                            double total = ((double)v.StopParkingTimer(PricePerMin).TotalMinutes) / PricePerMin;
                            v.Total = total;
                            Space[i].Bikes[j] = null; // Remove the motorcycle from this spot
                            Console.WriteLine("Motorcycle checked out from spot " + i + " Total price is " + v.Total.ToString("0.00") + "SEK");
                            found = true;
                            if (Space[i].Bikes[0] == null && Space[i].Bikes[1] == null)
                            {
                                Space[i].IsTaken = false;
                            }

                            VehiclesParked.Remove(VehiclesParked.FirstOrDefault(v));
                            IsFull = IsGarageFull();
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
                            double total = ((double)v.StopParkingTimer(PricePerMin).TotalMinutes) / PricePerMin;
                            v.Total = total;
                            Space[i + 1].Vehicle = null; // Clear the second spot of the bus
                            Space[i + 1].IsTaken = false;
                            VehiclesParked.Remove(VehiclesParked.FirstOrDefault(v));
                            Console.WriteLine("Bus checked out from spots " + i + " and " + (i + 1) + " Total price is " + v.Total.ToString("0.00") + "SEK");

                            IsFull = IsGarageFull();
                            return true;
                        }
                    }

                    else if (v is Car) // For cars or other vehicles occupying one spot
                    {

                        double total = ((double)v.StopParkingTimer(PricePerMin).TotalMinutes) / PricePerMin;
                        v.Total = total;
                        Console.WriteLine("Vehicle " + Space[i].Vehicle.LicensePlate + " is now checked out." + " Total price is " + total.ToString("0.00") + "SEK");
                        Space[i].Vehicle = null;
                        Space[i].IsTaken = false;
                        VehiclesParked.Remove(VehiclesParked.FirstOrDefault(v));
                        IsFull = IsGarageFull();
                        return true;
                    }



                    return false;
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




                if ((vehicle is MotorCycle && Space[i].IsTaken))
                {
                    if (Space[i].Vehicle == null && (Space[i].Bikes[0] == null || Space[i].Bikes[1] == null))
                    {

                        return i;
                    }
                }
                else if (vehicle is MotorCycle && !Space[i].IsTaken)
                {
                    if (Space[i].Vehicle == null && (Space[i].Bikes[0] == null || Space[i].Bikes[1] == null))
                    {

                        return i;
                    }
                }

                if (vehicle is Car && !Space[i].IsTaken)
                {
                    return i;
                }

                if (vehicle is Bus && i == Space.Count - 1)
                {
                    return -1;
                }
                else if ((vehicle is Bus) && (!Space[i].IsTaken) && (!Space[i + 1].IsTaken))
                {
                    return i;
                }




            }
            return -1;

        }


        // need console clear
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

                // Handle busses occupying two spots
                if (Space[i].Vehicle is Bus bus) // va häftigt med Bus bus
                {
                    Console.WriteLine($"Spot: {i} and {i + 1} - Vehicle " + bus.ToString());
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
                            Console.WriteLine($" - " + bike.ToString());
                        }
                    }
                }

                // cars
                if (Space[i].Vehicle != null && (Space[i].Vehicle is Car car))
                {


                    Console.WriteLine($"Spot: {i} - Vehicle " + car.ToString());
                }
            }
        }


        // updates using cursor pos
        public void PrintGarageSET()
        {
            int row = 3;
            for (int i = 0; i < Space.Count; i++)
            {
                Console.SetCursorPosition(0, row);

                // Check if the spot is empty
                if (Space[i].Vehicle == null && (Space[i].Bikes == null || Space[i].Bikes.All(b => b == null)))
                {
                    Console.WriteLine($"Spot {i,-2}: [Empty]");
                    row++;
                    continue;
                }

                // Handle busses occupying two spots
                if (Space[i].Vehicle is Bus bus)
                {
                    Console.WriteLine($"Spot {i,-2} & {i + 1,-2}: [Bus] {bus}");
                    row++;
                    i++; // Skip the next spot since it's occupied by the same bus
                    continue;
                }

                // Check and print motorcycles in the Bikes array
                if (Space[i].Bikes != null && (Space[i].Bikes[0] != null || Space[i].Bikes[1] != null))
                {
                    Console.WriteLine($"Spot {i,-2}: [Motorcycles]");
                    row++;

                    foreach (var bike in Space[i].Bikes)
                    {
                        if (bike != null)
                        {
                            Console.SetCursorPosition(4, row); // Indent motorcycles within the spot
                            Console.WriteLine($"- {bike}");
                            row++;
                        }
                    }
                }

                // Handle cars
                if (Space[i].Vehicle is Car car)
                {
                    Console.WriteLine($"Spot {i,-2}: [Car] {car}");
                    row++;
                }
            }
        }


        public bool IsGarageFull()
        {
            int takenSpot = 0;
            foreach (Space space in Space)
            {
                if (space.IsTaken)
                {
                    takenSpot++;
                }
            }

            if (takenSpot == MaxParkingSpaces)
            {
                return true;
            }
            return false;
        }

    }

    public class Space
    {
        public bool IsTaken { get; set; }
        public string VehicleType { get; set; }
        public Vehicle Vehicle { get; set; }

        public MotorCycle[] Bikes { get; set; }

        public Space()
        {
            IsTaken = false;
            Bikes = new MotorCycle[2];


        }



    }

}