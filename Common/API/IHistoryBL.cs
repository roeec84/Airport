using Common.Models;
using System.Collections.Generic;

namespace Common.API
{
    public interface IHistoryBL
    {
        List<History> Histories { get; set; }

        void EndRecord(Station station, Flight flight);
        void StartRecord(Station station, Flight flight);
    }
}