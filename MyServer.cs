using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GarageParking
{
    public class MyServer
    {



        public Garage Garage { get; set; }
        public MyServer(Garage garage)
        {
            Garage = garage;
        }


        public async Task StartServer()
        {



            // Initialize HTTP listener
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5000/");

            listener.Start();
            Console.SetCursorPosition(0, 23);
            Console.WriteLine("HTTP Server started on http://localhost:5000");

            while (true)
            {
                // Wait for an incoming request
                HttpListenerContext context = await listener.GetContextAsync();
                _ = Task.Run(() => HandleRequest(context)); // Handle each request in a new task
            }
        }

        // Handle incoming requests
        async Task HandleRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;


            // Enable CORS by setting the headers
            response.Headers.Add("Access-Control-Allow-Origin", "*");  // Allow all origins
            response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS"); // Allow specific methods
            response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");  // Allow specific headers


            if (request.HttpMethod == "OPTIONS")
            {
                // Handle preflight requests (for CORS)
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Close();
                return;
            }


            switch (request.Url.AbsolutePath)
            {
                case "/arrive":
                    await HandleArrive(response);
                    break;
                case "/checkout":
                    await HandleCheckout(response);
                    break;
                case "/list":
                    await ListJson(response);
                    break;
                default:
                    response.StatusCode = 404;
                    await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Endpoint not found"));
                    break;
            }

            response.Close();
        }

        // Route handlers (minimal responses for now)
        async Task HandleArrive(HttpListenerResponse response)
        {
            string message = "Arrive route accessed";
            response.StatusCode = 200;
            await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(message));
        }

        async Task HandleCheckout(HttpListenerResponse response)
        {
            string message = "Checkout route accessed";
            response.StatusCode = 200;
            await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(message));
        }

        async Task HandleList(HttpListenerResponse response)
        {
            string message = "List route accessed";
            response.StatusCode = 200;
            await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(message));
        }

        async Task ListJson(HttpListenerResponse response)
        {
            //List<TimeSpan> vehiclesTimer = new List<TimeSpan>();
            //foreach (Space s in Garage.Space)
            //{
            //    if (s.Vehicle != null)
            //    {
            //        TimeSpan time = s.Vehicle.sw.Elapsed;
            //        vehiclesTimer.Add(time);
            //    }
            //    if (s.Bikes[0] != null)
            //    {
            //        TimeSpan time = s.Bikes[0].sw.Elapsed;
            //        vehiclesTimer.Add(time);
            //    }
            //    if (s.Bikes[1] != null)
            //    {
            //        TimeSpan time = s.Bikes[1].sw.Elapsed;
            //        vehiclesTimer.Add(time);
            //    }

            //}
            // Serialize the list of spaces and their vehicles into JSON
            string jsonResponse = JsonSerializer.Serialize(Garage.Space);


            // Set the response type to JSON
            response.ContentType = "application/json";
            response.StatusCode = 200;

            // Write the serialized JSON to the response stream
            await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(jsonResponse));
            response.Close();
        }
    }
}




