using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class Flight : BaseModel
    {
        public FlightType FlightType { get; set; }
        public string FlightNumber { get; set; }
        public DateTime FlightTime { get; set; }
        public DateTime? ActualFlightTime { get; set; }
        public bool Delayed { get; set; }
        public bool IsOver { get; set; }
        public virtual Airplane Airplane { get; set; }
        public int AirplaneId { get; set; }
    }

    public enum FlightType
    {
        Arrival,
        Departure
    }
}
