using Common.API;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Airport.Tests.Mocks
{
    public class AirplaneBLMock
    {
        public List<Airplane> Airplanes { get; set; }

        public bool Add(Airplane airplane)
        {
            if (airplane == null) return false;
            Airplanes.Add(airplane);
            return true;
        }

        public Airplane CreateAirplane()
        {
            Airplane newAirplane = new Airplane { Id = Airplanes.Count + 1, IsAvailable = false, ReadyForNextStation = false };
            bool res = Add(newAirplane);
            if (!res) return null;
            return newAirplane;
        }
    }
}
