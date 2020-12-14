using Common.API;
using Common.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Hubs
{
    public class AirportHub : Hub
    {
        private readonly IFlightBL flightBL;
        private readonly IStationBL stationBL;

        public AirportHub(IFlightBL flightBL, IStationBL stationBL)
        {
            this.flightBL = flightBL;
            this.stationBL = stationBL;
        }

        public async Task StationDataChanged(List<Station> stations)
        {
            await Clients.All.SendAsync("StationsUpdated", stations);
        }

        public async Task FlightsUpdated(List<Flight> flights)
        {
            List<Flight> relevantFlights = flights.Where(f => (f.FlightTime > DateTime.Now) || f.Delayed == true && f.IsOver == false).ToList();
            await Clients.All.SendAsync("ReceiveFlight", relevantFlights);
        }

        public async Task SendFlight(Flight flight)
        {
            bool isAdded = await flightBL.Add(flight);
            if (isAdded)
                await FlightsUpdated(GetAllFlights());
        }

        public List<Flight> GetAllFlights()
        {
            List<Flight> flights = flightBL.Flights.Where(f => (f.FlightTime > DateTime.Now) || f.Delayed == true && f.IsOver == false).ToList();
            return flights;
        }

        public List<Station> GetAllStations()
        {
            return stationBL.Stations;
        }
    }
}
