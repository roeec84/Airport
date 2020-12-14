using Common.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Simulator
{
    public class FlightBuilder
    {

        public FlightBuilder()
        {
        }

        public async Task<Flight> CreateFlightAsync()
        {
            var flight = await Task.Run(() => CreateFlight());
            return flight;
        }

        public Flight CreateFlight()
        {
            Random rand = new Random();
            Array values = Enum.GetValues(typeof(FlightType));
            FlightType type = (FlightType)values.GetValue(rand.Next(values.Length));
            double mins = rand.Next(1, 2);
            string flightNumber = GenerateFlightNumber(4);
            Flight flight = new Flight { FlightType = type, FlightTime = DateTime.Now.AddMinutes(mins), FlightNumber = flightNumber };
            return flight;
        }

        private string GenerateFlightNumber(int len)
        {
            Random rand = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, len)
              .Select(s => s[rand.Next(s.Length)]).ToArray());
        }
    }
}
