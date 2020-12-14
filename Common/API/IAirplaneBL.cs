using Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.API
{
    public interface IAirplaneBL
    {
        List<Airplane> Airplanes { get; set; }

        event Action<Airplane, Flight> AirplaneIsReady;
        event Action<Airplane> AirplaneSetFree;

        Task<bool> Add(Airplane airplane);
        Task<Airplane> CreateAirplane();
        Airplane GetAirplaneById(int airplaneId);
        Airplane GetAvailableAirplane();
        void SetArplaneFree(int airplaneId);
        void WaitInStation(Airplane airplane, Flight flight);
    }
}