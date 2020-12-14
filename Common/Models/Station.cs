using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class Station : BaseModel
    {
        public bool IsAvailable { get; set; }
        public int AirplaneId { get; set; }
    }
}