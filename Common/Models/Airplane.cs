using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class Airplane : BaseModel
    {
        public bool IsAvailable { get; set; }
        public bool ReadyForNextStation { get; set; }
        public double TimeToStay { get; set; }
    }
}
