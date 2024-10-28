using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageParking
{
    internal class Helpers
    {
        static private string Input { get; set; }



        /// <summary>
        /// takes plate input returns vehicle obejct
        /// </summary>
        /// <param name="garage"></param>
        /// <returns></returns>
        static public Vehicle GetPlate(Garage garage)
        {
            try
            {
                Console.Write("Enter plate: ");
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
