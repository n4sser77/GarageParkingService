using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

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
            Console.SetCursorPosition(0, 26);
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
                case "/":
                    await HandleRoot(response);
                    break;
                case "/arrive":
                    await HandleArrive(response, request);
                    break;
                case "/checkout":
                    await HandleCheckout(response, request);
                    break;
                case "/api/list":
                    await HandleListJson(response);
                    break;
                case "/api/isfull":
                    await HandleIsFull(response);
                    break;

                default:
                    response.StatusCode = 404;
                    await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Endpoint not found"));
                    break;
            }

            response.Close();
        }



        async Task HandleIsFull(HttpListenerResponse response)
        {
            string jsonbool = JsonSerializer.Serialize(Garage.IsFull);

            // Set the response type to JSON
            response.ContentType = "application/json";
            response.StatusCode = 200;


            await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(jsonbool));
        }


        // Route handlers (minimal responses for now)
        async Task HandleArrive(HttpListenerResponse response, HttpListenerRequest request)
        {
            if (request.HttpMethod == "POST")
            {


                string message = " proccess started";
                try
                {
                    Vehicle v = Helpers.RandomVehicle();
                    if (Garage.park(v))
                    {
                        message = v.ToString() + " Parked in " + (v.ParkedAt + 1);
                    }
                    else
                    {
                        message = "No room for vehicle " + v.ToString();
                    }

                }
                catch (Exception e)
                {

                    message = $"Error proccessing checkout: {e.Message}";
                    response.StatusCode = 500;
                }

                response.StatusCode = 200;
                await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(message));


            }


        }

        async Task HandleCheckout(HttpListenerResponse response, HttpListenerRequest request)
        {
            if (request.HttpMethod == "POST")
            {
                using (StreamReader sr = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    string licensePlateData = await sr.ReadToEndAsync();
                    string message = "NOT FOUND";

                    /* only when using named input field in html
                    // NameValueCollection ParsedData = HttpUtility.ParseQueryString(licensePlateData);
                    // string licensePlate = ParsedData["licensePlate"];
                    */

                    try
                    {
                        Vehicle v = Garage.FindPlate(licensePlateData);
                        if (v != null)
                        {

                            bool success = Garage.CheckOut(v, out message);

                            if (success)
                            {
                                response.StatusCode = 200;
                            }
                            else
                            {
                                response.StatusCode = 404;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        message = $"Error proccessing checkout: {e.Message}";
                        response.StatusCode = 500;
                    }



                    byte[] buffer = Encoding.UTF8.GetBytes(message);
                    await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);

                }
            }
            else
            {
                response.StatusCode = 405;
                byte[] buffer = Encoding.UTF8.GetBytes("Method Not allowed");
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            }
        }

        async Task HandleRoot(HttpListenerResponse response)
        {


            // Construct a response.
            string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
            response.ContentType = "text/html";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();


        }






        async Task HandleListJson(HttpListenerResponse response)
        {

            // Serialize the list of spaces and their vehicles into JSON
            string jsonResponse = JsonSerializer.Serialize(Garage.Space);



            // Set the response type to JSON
            response.ContentType = "application/json";
            response.StatusCode = 200;

            // Write the serialized JSON to the response stream
            await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(jsonResponse));


        }






    }
}




