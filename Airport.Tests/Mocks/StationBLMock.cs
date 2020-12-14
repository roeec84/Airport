using Common.API;
using Common.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Airport.Tests.Mocks
{
    [TestFixture]
    public class StationBLMock
    {
        private readonly FlightBLMock flightBL;
        private List<int> arrivalCourseList = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
        private List<int> departureCourseList = new List<int> { 6, 7, 8, 4 };
        private List<KeyValuePair<Airplane, Flight>> waitingToMoveAirplanes;
        private event Action StationIsAvailable;

        public List<Station> Stations { get; set; }
        public StationBLMock(FlightBLMock flightBL)
        {
            Stations = new List<Station>
            {
                new Station {Id = 1, AirplaneId = 0, IsAvailable = true},
                new Station {Id = 2, AirplaneId = 0, IsAvailable = true},
                new Station {Id = 3, AirplaneId = 0, IsAvailable = true},
                new Station {Id = 4, AirplaneId = 0, IsAvailable = true},
                new Station {Id = 5, AirplaneId = 0, IsAvailable = true},
                new Station {Id = 6, AirplaneId = 0, IsAvailable = true},
                new Station {Id = 7, AirplaneId = 0, IsAvailable = true},
                new Station {Id = 8, AirplaneId = 0, IsAvailable = true}
            };
            this.flightBL = flightBL;
            flightBL.SetAirplaneToFirstStation += SetAirplaneToFirstStation;
            waitingToMoveAirplanes = new List<KeyValuePair<Airplane, Flight>>();
            StationIsAvailable += PlaceWaitingAirplanes;
        }

        private void PlaceWaitingAirplanes()
        {
            for (int i = 0; i < waitingToMoveAirplanes.Count; i++)
            {
                MoveAirplaneToNextStation(waitingToMoveAirplanes[i].Key, waitingToMoveAirplanes[i].Value);
            }
        }

        public void SetAirplaneToFirstStation(Flight flight)
        {
            switch (flight.FlightType)
            {
                case FlightType.Arrival:
                    if (Stations[0].IsAvailable)
                    {
                        Stations[0].IsAvailable = false;
                        Stations[0].AirplaneId = flight.AirplaneId;
                    }
                    break;
                case FlightType.Departure:
                    if (Stations[5].IsAvailable)
                    {
                        Stations[5].IsAvailable = false;
                        Stations[5].AirplaneId = flight.AirplaneId;
                    }
                    else if (Stations[6].IsAvailable)
                    {
                        Stations[6].IsAvailable = false;
                        Stations[6].AirplaneId = flight.AirplaneId;
                    }
                    break;
                default:
                    break;
            }
        }

        public bool MoveAirplaneToNextStation(Airplane airplane, Flight flight)
        {
            bool res = false;
            Station currentStation = Stations.FirstOrDefault(s => s.AirplaneId == airplane.Id);
            switch (flight.FlightType)
            {
                case FlightType.Arrival:
                    int idx = arrivalCourseList.IndexOf(currentStation.Id);
                    if (idx == arrivalCourseList.Count - 1 || idx == arrivalCourseList.Count - 2) //Take off
                    {
                        currentStation.IsAvailable = true;
                        currentStation.AirplaneId = 0;
                        airplane.IsAvailable = true;
                        KeyValuePair<Airplane, Flight> keyPair = waitingToMoveAirplanes.FirstOrDefault(kp => (kp.Key == airplane && kp.Value == flight));
                        if (!keyPair.Equals(default(KeyValuePair<Airplane, Flight>)))
                            waitingToMoveAirplanes.Remove(keyPair);
                        StationIsAvailable?.Invoke();
                    }
                    else
                    {
                        Station nextArrivalStation = Stations.FirstOrDefault(s => s.Id == arrivalCourseList[idx + 1]);
                        if (nextArrivalStation.IsAvailable)
                        {
                            currentStation.IsAvailable = true;
                            currentStation.AirplaneId = 0;
                            nextArrivalStation.AirplaneId = airplane.Id;
                            nextArrivalStation.IsAvailable = false;
                            KeyValuePair<Airplane, Flight> keyPair = waitingToMoveAirplanes.FirstOrDefault(kp => (kp.Key == airplane && kp.Value == flight));
                            if (!keyPair.Equals(default(KeyValuePair<Airplane, Flight>)))
                                waitingToMoveAirplanes.Remove(keyPair);
                            StationIsAvailable?.Invoke();
                            res = true;
                        }
                        else
                        {
                            KeyValuePair<Airplane, Flight> keyPair = waitingToMoveAirplanes.FirstOrDefault(kp => (kp.Key == airplane && kp.Value == flight));
                            if (keyPair.Equals(default(KeyValuePair<Airplane, Flight>)))
                                waitingToMoveAirplanes.Add(new KeyValuePair<Airplane, Flight>(airplane, flight));
                            res = false;
                        }
                    }
                    break;
                case FlightType.Departure:
                    idx = departureCourseList.IndexOf(currentStation.Id);
                    if (idx == departureCourseList.Count - 1)
                    {
                        currentStation.IsAvailable = true;
                        currentStation.AirplaneId = 0;
                        airplane.IsAvailable = true;
                        KeyValuePair<Airplane, Flight> keyPair = waitingToMoveAirplanes.FirstOrDefault(kp => (kp.Key == airplane && kp.Value == flight));
                        if (!keyPair.Equals(default(KeyValuePair<Airplane, Flight>)))
                            waitingToMoveAirplanes.Remove(keyPair);
                        StationIsAvailable?.Invoke();
                    }
                    else
                    {
                        Station nextDepartureStation = Stations.FirstOrDefault(s => s.Id == arrivalCourseList[idx + 1]);
                        if (nextDepartureStation.IsAvailable)
                        {
                            currentStation.IsAvailable = true;
                            currentStation.AirplaneId = 0;
                            nextDepartureStation.AirplaneId = airplane.Id;
                            nextDepartureStation.IsAvailable = false;
                            KeyValuePair<Airplane, Flight> keyPair = waitingToMoveAirplanes.FirstOrDefault(kp => (kp.Key == airplane && kp.Value == flight));
                            if (!keyPair.Equals(default(KeyValuePair<Airplane, Flight>)))
                                waitingToMoveAirplanes.Remove(keyPair);
                            StationIsAvailable?.Invoke();
                            res = true;
                        }
                        else
                        {
                            KeyValuePair<Airplane, Flight> keyPair = waitingToMoveAirplanes.FirstOrDefault(kp => (kp.Key == airplane && kp.Value == flight));
                            if (keyPair.Equals(default(KeyValuePair<Airplane, Flight>)))
                                waitingToMoveAirplanes.Add(new KeyValuePair<Airplane, Flight>(airplane, flight));
                            res = false;
                        }
                    }
                    break;
                default:
                    break;
            }
            return res;
        }
    }
}
