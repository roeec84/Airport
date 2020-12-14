using Common.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services
{
    public class HubService : IHubService
    {
        private HubConnection connection;
        public event Action<List<Flight>> ReceivedNewFlight;
        public event Action<List<Station>> StationsUpdated;
        public HubService()
        {
            ConnectAsync();
        }

        public async Task ConnectAsync()
        {
            connection = new HubConnectionBuilder()
                            .WithUrl("http://localhost:61576/Airport")
                            .WithAutomaticReconnect()
                            .Build();
            await connection.StartAsync();
            connection.On<List<Flight>>("ReceiveFlight", (fl) => ReceivedNewFlight?.Invoke(fl));
            connection.On<List<Station>>("StationsUpdated", (stations) => StationsUpdated?.Invoke(stations));
        }

        public async Task<List<Flight>> GetFlightsAsync()
        {
            var flights = await connection.InvokeAsync<List<Flight>>("GetAllFlights");
            return flights;
        }

        public async Task<List<Station>> GetStationsAsync()
        {
            var stations = await connection.InvokeAsync<List<Station>>("GetAllStations");
            return stations;
        }
    }
}
