using Common.API;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Airport.Tests.BL
{
    [TestFixture]
    public class AirplaneBLTests
    {
        private readonly IAirplaneBL airplaneBL;

        public AirplaneBLTests(IAirplaneBL airplaneBL)
        {
            this.airplaneBL = airplaneBL;
        }
    }
}
