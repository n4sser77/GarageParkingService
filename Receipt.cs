using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GarageParking
{
    public class Receipt
    {
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
            string filePath = "receipts.json";
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

    }

}
