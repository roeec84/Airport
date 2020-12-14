using Common.API;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class HistoryBL : IHistoryBL
    {
        private readonly IRepository<History> repository;

        public List<History> Histories { get; set; }
        public HistoryBL(IRepository<History> repository)
        {
            this.repository = repository;
            Histories = repository.GetAll().ToList();
        }

        public void StartRecord(Station station, Flight flight)
        {
            History history = new History { FlightId = flight.Id, StationId = station.Id, EnterToStation = DateTime.Now };
            Histories.Add(history);
            repository.Add(history);
            repository.Save();
        }

        public void EndRecord(Station station, Flight flight)
        {
            History history = Histories.FirstOrDefault(h => (h.FlightId == flight.Id && h.StationId == station.Id));
            if (history != null)
            {
                history.LeftStation = DateTime.Now;
                repository.Save();
            }
        }
    }
}
