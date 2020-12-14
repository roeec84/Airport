using Common.API;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.Tests.Mocks
{
    public class FlightBLMock
    {
        public List<Flight> Flights { get; set; }

        public event Action<Flight> SetAirplaneToFirstStation;
        public FlightBLMock()
        {
            Flights = new List<Flight>();
        }

        public bool Add(Flight flight)
        {
            if (flight == null) return false;
            Flights.Add(flight);
            SetAirplaneToFirstStation?.Invoke(flight);
            return true;
        }

        public Flight GetAirplaneFlight(int airplaneId)
        {
            Flight flight = Flights.FirstOrDefault(f => f.AirplaneId == airplaneId);
            return flight;
        }
    }
}
