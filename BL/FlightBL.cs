using Common.API;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.SignalR.Client;

namespace BL
{
    public class FlightBL : IFlightBL
    {
        private readonly IRepository<Flight> repository;
        private readonly IAirplaneBL airplaneBL;
        private HubConnection connection;
        public event Action<Flight> SetAirplaneToFirstStation;

        public List<Flight> Flights { get; set; }
        public FlightBL(IRepository<Flight> repository, IAirplaneBL airplaneBL)
        {
            this.repository = repository;
            this.airplaneBL = airplaneBL;
            Flights = repository.GetAll().ToList();
            airplaneBL.AirplaneIsReady += CheckIfDelayed;
            airplaneBL.AirplaneSetFree += FlightIsOver;
            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:61576/Airport")
                .WithAutomaticReconnect()
                .Build();
            connection.StartAsync().Wait();
        }

        private void FlightIsOver(Airplane airplane)
        {
            Flight flight = Flights.FirstOrDefault(f => f.AirplaneId == airplane.Id);
            if (flight != null)
            {
                flight.Delayed = false;
                flight.ActualFlightTime = DateTime.Now;
                connection.InvokeAsync("FlightsUpdated", Flights);
            }
        }

        private void CheckIfDelayed(Airplane airplane, Flight flight)
        {
            if(flight != null)
            {
                if(DateTime.Now > flight.FlightTime)
                {
                    flight.Delayed = true;
                    connection.InvokeAsync("FlightsUpdated", Flights);
                }
            }
        }

        public async Task<bool> Add(Flight flight)
        {
            if (flight.AirplaneId == 0)
            {
                bool res = await AssignAirplane(flight);
                if (!res) return false;
            }
            var isSuccess = await repository.Add(flight);
            if (!isSuccess) return false;
            Flights.Add(flight);
            SetAirplaneToFirstStation?.Invoke(flight);
            return true;
        }

        private async Task<bool> AssignAirplane(Flight flight)
        {
            bool flag;
            Airplane airplane = airplaneBL.GetAvailableAirplane();
            if (airplane != null)
            {
                flight.AirplaneId = airplane.Id;
                flight.Airplane = airplane;
                CalculateRestTime(flight, airplane);
                flag = true;
            }
            else
            {
                airplane = await airplaneBL.CreateAirplane();
                if (airplane == null) return false;
                CalculateRestTime(flight, airplane);
                flight.AirplaneId = airplane.Id;
                flight.Airplane = airplane;
                flag = true;
            }
            return flag;
        }

        private void CalculateRestTime(Flight flight, Airplane airplane)
        {
            TimeSpan ts = flight.FlightTime - DateTime.Now;
            switch (flight.FlightType)
            {
                case FlightType.Arrival:
                    airplane.TimeToStay = ts.TotalMinutes / 6;
                    break;
                case FlightType.Departure:
                    airplane.TimeToStay = ts.TotalMinutes / 3;
                    break;
                default:
                    break;
            }
        }

        public Flight GetAirplaneFlight(int airplaneId)
        {
            var flight = Flights.FirstOrDefault(f => f.AirplaneId == airplaneId);
            return flight;
        }
    }
}
