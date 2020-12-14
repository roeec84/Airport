using Airport.Tests.Mocks;
using Common.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Airport.Tests.BL
{
    [TestFixture]
    public class StationBLTests
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
        public void MoveAirplaneToNextStation_NextStationIsNotAvailable_AirplaneStayInTheCurrentStation()
        {
            //Arrange
            Flight flight = new Flight
            {
                Id = 1,
                AirplaneId = 1,
                Airplane = new Airplane { Id = 1, IsAvailable = false, ReadyForNextStation = false },
                FlightType = FlightType.Arrival
            };

            Flight flight2 = new Flight
            {
                Id = 2,
                AirplaneId = 2,
                Airplane = new Airplane { Id = 2, IsAvailable = false, ReadyForNextStation = false },
                FlightType = FlightType.Arrival
            };

            stationBL.Stations[2].IsAvailable = false;
            stationBL.Stations[2].AirplaneId = flight.AirplaneId;
            stationBL.Stations[1].IsAvailable = false;
            stationBL.Stations[1].AirplaneId = flight2.AirplaneId;

            //Act
            stationBL.MoveAirplaneToNextStation(flight2.Airplane, flight2);

            //Assert
            Assert.That(stationBL.Stations[2].AirplaneId, Is.EqualTo(flight.AirplaneId));
        }

        [Test]
        public void MoveAirplaneToNextStation_NextStationIsAvailableAndClearTheCurrentStation_AirplaneIsMoving()
        {
            //Arrange
            Flight flight = new Flight
            {
                Id = 1,
                AirplaneId = 1,
                Airplane = new Airplane { Id = 1, IsAvailable = false, ReadyForNextStation = false },
                FlightType = FlightType.Arrival
            };

            stationBL.Stations[2].IsAvailable = false;
            stationBL.Stations[2].AirplaneId = flight.AirplaneId;

            //Act
            bool res = stationBL.MoveAirplaneToNextStation(flight.Airplane, flight);

            //Assert
            Assert.That(stationBL.Stations[2].AirplaneId, Is.Not.EqualTo(flight.AirplaneId));
            Assert.That(stationBL.Stations[2].IsAvailable, Is.True);
            Assert.That(stationBL.Stations[3].AirplaneId, Is.EqualTo(flight.AirplaneId));
            Assert.That(res, Is.True);
        }
        [Test]
        public void MoveAirplaneToNextStation_StationWasUnavailableAndNowItsFree_PlaceAirplaneInStation()
        {
            //Arrange
            Flight flight = new Flight
            {
                Id = 1,
                AirplaneId = 1,
                Airplane = new Airplane { Id = 1, IsAvailable = false, ReadyForNextStation = false },
                FlightType = FlightType.Arrival
            };

            Flight flight2 = new Flight
            {
                Id = 2,
                AirplaneId = 2,
                Airplane = new Airplane { Id = 2, IsAvailable = false, ReadyForNextStation = false },
                FlightType = FlightType.Arrival
            };

            stationBL.Stations[2].IsAvailable = false;
            stationBL.Stations[2].AirplaneId = flight.AirplaneId;
            stationBL.Stations[1].IsAvailable = false;
            stationBL.Stations[1].AirplaneId = flight2.AirplaneId;

            //Act
            bool flight2Res = stationBL.MoveAirplaneToNextStation(flight2.Airplane, flight2); //Should be false
            bool flightRes = stationBL.MoveAirplaneToNextStation(flight.Airplane, flight); //Should be true

            //Assert
            Assert.That(flight2Res, Is.False);
            Assert.That(flightRes, Is.True);
            Assert.That(stationBL.Stations[2].AirplaneId, Is.EqualTo(flight2.AirplaneId));
        }
    }
}
