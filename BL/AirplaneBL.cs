using Common.API;
using Common.Models;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using System;

namespace BL
{
    public class AirplaneBL : IAirplaneBL
    {
        private readonly IRepository<Airplane> repository;
        private readonly object setAirplaneAvailable = new object();
        public event Action<Airplane, Flight> AirplaneIsReady;
        public event Action<Airplane> AirplaneSetFree;

        public List<Airplane> Airplanes { get; set; }

        public AirplaneBL(IRepository<Airplane> repository)
        {
            this.repository = repository;
            Airplanes = repository.GetAll().ToList();
        }

        public async Task<bool> Add(Airplane airplane)
        {
            bool res = await repository.Add(airplane);
            if (res)
                Airplanes.Add(airplane);
            return res;
        }

        public Airplane GetAirplaneById(int airplaneId)
        {
            return Airplanes.FirstOrDefault(a => a.Id == airplaneId);
        }

        public Airplane GetAvailableAirplane()
        {
            Airplane airplane = Airplanes.FirstOrDefault(a => a.IsAvailable == true);
            if (airplane != null)
            {
                airplane.IsAvailable = false;
            }
            return airplane;
        }

        public async Task<Airplane> CreateAirplane()
        {
            Airplane newAirplane = new Airplane() { IsAvailable = false, ReadyForNextStation = false };
            bool res = await Add(newAirplane);
            if (!res) return null;
            return newAirplane;
        }

        public void WaitInStation(Airplane airplane, Flight flight)
        {
            if (airplane != null)
            {
                airplane.ReadyForNextStation = false;
                Timer timer = new Timer();
                Random rand = new Random();
                timer.Interval = airplane.TimeToStay * 1000 * 60;
                timer.AutoReset = false;
                timer.Elapsed += (e, s) => SetAirplaneReady(airplane, flight);
                timer.Start();
            }
        }

        private void SetAirplaneReady(Airplane airplane, Flight flight)
        {
            lock (setAirplaneAvailable)
            {
                airplane.ReadyForNextStation = true;
                AirplaneIsReady?.Invoke(airplane, flight);
            }
        }

        public void SetArplaneFree(int airplaneId)
        {
            Airplane airplane = Airplanes.FirstOrDefault(a => a.Id == airplaneId);
            if (airplane != null)
            {
                airplane.IsAvailable = true;
                AirplaneSetFree?.Invoke(airplane);
            }
        }
    }
}
