using Airport.Tests.Mocks;
using Common.API;
using Common.Models;
using NUnit.Framework;

namespace Airport.Tests.BL
{
    [TestFixture]
    public class FlightBLTests
    {
        private FlightBLMock flightBL;
        private StationBLMock stationBL;

        [SetUp]
        public void SetUp()
        {
            flightBL = new FlightBLMock();
            stationBL = new StationBLMock(flightBL);
        }

        [Test]
        public void Add_StationNotAvailable_FlightWillNotLand()
        {
            //Arrange
            stationBL.Stations[0].IsAvailable = false;
            Flight flight = new Flight
            {
                Id = 1,
                AirplaneId = 1,
                Airplane = new Airplane { Id = 1, IsAvailable = false, ReadyForNextStation = false },
                FlightType = FlightType.Arrival
            };

            //Act
            flightBL.Add(flight);

            //Assert
            Assert.That(stationBL.Stations[0].AirplaneId, Is.Not.EqualTo(1));
        }

        [Test]
        public void Add_StationIsAvailable_FlightWillLand()
        {
            //Arrange
            Flight flight = new Flight
            {
                Id = 1,
                AirplaneId = 1,
                Airplane = new Airplane { Id = 1, IsAvailable = false, ReadyForNextStation = false },
                FlightType = FlightType.Arrival
            };

            //Act
            flightBL.Add(flight);

            //Assert
            Assert.That(stationBL.Stations[0].IsAvailable, Is.False);
        }
    }
}
