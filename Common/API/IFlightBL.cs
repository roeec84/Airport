using Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.API
{
    public interface IFlightBL
    {
        List<Flight> Flights { get; set; }

        event Action<Flight> SetAirplaneToFirstStation;

        Task<bool> Add(Flight flight);
        Flight GetAirplaneFlight(int airplaneId);
    }
}