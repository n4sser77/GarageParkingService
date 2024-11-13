﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageParking
{
    public class Helpers
    {
        static private string Input { get; set; }


        /// <summary>
        /// Generats a new random vehicle
        /// </summary>
        /// <returns></returns>
        public static Vehicle RandomVehicle()
        {

            string[] colors = { "blue", "red", "black", "gray" };

            string[] allBikeMakes = { "Harley Davidson", "Yamaha", "Ducati", "Kawasaki", "Honda", "BMW", "Suzuki", "Triumph", "Indian", "Royal Enfield", "Aprilia", "KTM", "Moto Guzzi", "Victory", "Husqvarna" };
            //       plate
            // car,       bus,        bike
            // electric,  passengers,  make
            Random rnd = new Random();
            int type = rnd.Next(1, 4);
            int electric = rnd.Next(0, 1);
            int passangers = rnd.Next(7, 14);
            int makeIndex = rnd.Next(0, 14);
            int color = rnd.Next(0, 4);


            switch (type)
            {
                case 1:
                    {
                        if (electric == 1)
                        {
                            return new Car(colors[color], true);
                        }
                        else
                        {
                            return new Car(colors[color], false);
                        }
                    }

                case 2:
                    {
                        return new Bus(colors[color], passangers);
                    }

                case 3:
                    {
                        return new MotorCycle(colors[color], allBikeMakes[makeIndex]);
                    }
                default:
                    Console.WriteLine("Could not make random vehicle");
                    return null;
            }
        }


        /// <summary>
        /// takes plate input returns vehicle obejct
        /// </summary>
        /// <param name="garage"></param>
        /// <returns></returns>
        public static Vehicle GetVehicleFromGaragge(Garage garage)
        {
            try
            {

                Input = Console.ReadLine();
                Vehicle v = garage.FindPlate(Input.ToUpper());

                if (Input != null && Input.Length <= 6 && v != null)
                {
                    return v;
                }


                return null;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }



        }

        public static string GeneratePlate()
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

        public static MotorCycle FindBike(MotorCycle[] bikes, MotorCycle bike)
        {
            for (int i = 0; i < bikes.Length; i++)
            {
                if (bikes[i] == bike)
                {
                    return bike;

                }
            }
            return null;
        }



    }
}
