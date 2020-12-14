using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class History : BaseModel
    {
        public int StationId { get; set; }
        public virtual Station Station { get; set; }
        public int FlightId { get; set; }
        public virtual Flight Flight { get; set; }
        public DateTime? EnterToStation { get; set; }
        public DateTime? LeftStation { get; set; }
    }
}
