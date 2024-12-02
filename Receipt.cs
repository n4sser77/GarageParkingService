using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GarageParking
{
    /// <summary>
    /// Parking receipt data, currently used for logging in json
    /// </summary>
    public class Receipt
    {
        static string filePath = "receipts.json";
        static public double TotalSales { get; set; }
        public string VehicleType { get; set; }
        public string LicensePlate { get; set; }
        public int ParkingSpot { get; set; }
        public double Price { get; set; }
        public DateTime Timestamp { get; set; }

        public Receipt()
        {
            Timestamp = DateTime.Now;

        }


        public void LogReceipt(Receipt receipt)
        {

            List<Receipt> receipts = new List<Receipt>();

            // Read existing data if file exists
            if (File.Exists(filePath))
            {
                string existingData = File.ReadAllText(filePath);
                receipts = JsonSerializer.Deserialize<List<Receipt>>(existingData) ?? new List<Receipt>();
            }

            // Add the new receipt and write back to file
            receipts.Add(receipt);
            string jsonData = JsonSerializer.Serialize(receipts, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonData);
        }

        static public void UpdateTotalSales()
        {

            List<Receipt> rs = GetAllReceipts();
            double total = 0;
            foreach (Receipt r in rs)
            {
                total += r.Price;
            }
            TotalSales = Math.Round(total,2);

        }

        static public List<Receipt> GetAllReceipts()
        {

            List<Receipt> rs = new List<Receipt>();
            if (File.Exists(filePath))
            {
                string existingData = File.ReadAllText(filePath);
                rs = JsonSerializer.Deserialize<List<Receipt>>(existingData) ?? new List<Receipt>();
                return rs;
            }
            else
            {
                return null;
            }


        }

    }

}
