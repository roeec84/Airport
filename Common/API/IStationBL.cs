using Common.Models;
using System.Collections.Generic;

namespace Common.API
{
    public interface IStationBL
    {
        List<Station> Stations { get; set; }
    }
}