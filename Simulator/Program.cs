using Common.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace Simulator
{
    class Program
    {
        private static bool isReadyToCreateFlight = true;
        private static bool flightIsAlreadyCreated = false;
        private static readonly object lockObj = new object();
        private static Timer nextFlight;

        static void Main(string[] args)
        {
            nextFlight = new Timer();
            FlightBuilder flightBuilder = new FlightBuilder();
            HubConnection connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:61576/Airport")
                .WithAutomaticReconnect()
                .Build();
            connection.StartAsync().Wait();

            while (true)
            {
                if (isReadyToCreateFlight)
                {
                    isReadyToCreateFlight = false;
                    Task.Run(() => BuildNewFlight(flightBuilder, connection));
                }
            }
        }

        private static void BuildNewFlight(FlightBuilder flightBuilder, HubConnection connection)
        {
            lock (lockObj)
            {
                if (!flightIsAlreadyCreated)
                {
                    Flight flight = flightBuilder.CreateFlight();
                    connection.InvokeAsync("SendFlight", flight);
                    Console.WriteLine($"Flight {flight.FlightNumber} has sent");
                    Random rand = new Random();
                    nextFlight.Interval = (rand.NextDouble() * 1000 * rand.Next(40, 61));
                    nextFlight.Elapsed += NextFlightTimer;
                    nextFlight.AutoReset = false;
                    nextFlight.Start();
                    flightIsAlreadyCreated = true;
                }
            }
        }

        private static void NextFlightTimer(object sender, ElapsedEventArgs e)
        {
            isReadyToCreateFlight = true;
            flightIsAlreadyCreated = false;
            nextFlight.Stop();
        }
    }
}
