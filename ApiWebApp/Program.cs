﻿using ApiWebApp.Controllers;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ApiWebApp
{
    class Program
    {
        static Task Main()
        {
            // Create a HttpListener that listens on the loopback address (localhost), on the port 8080
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();

            Console.WriteLine("Listening for requests...");

            while (true)
            {
                try
                {
                    HttpListenerContext context = listener.GetContext();

                    // _ is a convention to show that the result is intentionally being ignored
                    _ = RouteAndHandleRequestAsync(context);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occured while receiving the request : {ex.Message}");
                }
            }
        }
        
        static async Task RouteAndHandleRequestAsync(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            
            string route = request.RawUrl;
            
            switch (route)
            {
                case "/accounts":
                    try
                    {
                        await AccountController.HandleAccountsAsync(context);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in handling /accounts : {ex.Message}");
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    }
                    break;
                case "/products":
                    // await ProductController.HandleProductsAsync(context);
                    break;
                default:
                    // 404 error if route doesn't exist
                    response.StatusCode = (int)HttpStatusCode.NotFound;

                    // Get the output where data will be written
                    using (Stream output = response.OutputStream)
                    {
                        // Provide an error message in the response body
                        string notFoundResponse = "404 Not Found";
                        byte[] notFoundResponseBytes = System.Text.Encoding.UTF8.GetBytes(notFoundResponse);
                        response.ContentLength64 = notFoundResponseBytes.Length;
                        
                        // Write error response
                        await output.WriteAsync(notFoundResponseBytes, 0, notFoundResponseBytes.Length);
                    }
                    break;
            }
            response.Close();
        }
    }
}
