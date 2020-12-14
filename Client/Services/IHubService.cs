using Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Services
{
    public interface IHubService
    {
        event Action<List<Flight>> ReceivedNewFlight;
        event Action<List<Station>> StationsUpdated;

        Task ConnectAsync();
        Task<List<Flight>> GetFlightsAsync();
        Task<List<Station>> GetStationsAsync();
    }
}