using Common.API;
using Common.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace BL
{
    public class StationBL : IStationBL
    {
        private readonly IRepository<Station> repository;
        private readonly IAirplaneBL airplaneBL;
        private readonly IFlightBL flightBL;
        private readonly IHistoryBL historyBL;

        public List<Station> Stations { get; set; }
        private HubConnection connection;
        private readonly object moveAirplane = new object();
        private List<int> arrivalCourseList = new List<int> { 2, 3, 4, 5 };
        private List<int> departureCourseList = new List<int> { 8 };
        private List<int> arrivalLandingStations = new List<int> { 1 };
        private List<int> departuresLandingStations = new List<int> { 6, 7 };
        private List<int> arrivalTakeoffStations = new List<int> { 6, 7 };
        private List<int> departuresTakeoffStations = new List<int> { 4 };
        private List<KeyValuePair<Airplane, Flight>> waitingToMoveAirplanes;
        private event Action StationIsAvailable;

        public StationBL(IRepository<Station> repository, IAirplaneBL airplaneBL, IFlightBL flightBL, IHistoryBL historyBL)
        {
            this.repository = repository;
            this.airplaneBL = airplaneBL;
            this.flightBL = flightBL;
            this.historyBL = historyBL;
            airplaneBL.AirplaneIsReady += MoveAirplaneToNextStation;
            flightBL.SetAirplaneToFirstStation += SetAirplaneToFirstStation;
            waitingToMoveAirplanes = new List<KeyValuePair<Airplane, Flight>>();
            StationIsAvailable += PlaceWaitingAirplanes;
            Stations = repository.GetAll().ToList();
            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:61576/Airport")
                .WithAutomaticReconnect()
                .Build();
            connection.StartAsync().Wait();
            StartAirplanesMovement();
        }

        private void SetAirplaneToFirstStation(Flight flight)
        {
            if (flight == null) return;
            switch (flight.FlightType)
            {
                case FlightType.Arrival:
                    for (int i = 0; i < arrivalLandingStations.Count; i++)
                    {
                        Station station = Stations.FirstOrDefault(s => s.Id == arrivalLandingStations[i]);
                        if (station.IsAvailable)
                        {
                            PutAirplaneInFirstStation(flight, station);
                            break;
                        }
                        if (i == arrivalLandingStations.Count - 1)
                        {
                            PutAirplaneInWaitingList(flight.Airplane, flight);
                        }
                    }
                    break;
                case FlightType.Departure:
                    for (int i = 0; i < departuresLandingStations.Count; i++)
                    {
                        Station station = Stations.FirstOrDefault(s => s.Id == departuresLandingStations[i]);
                        if (station.IsAvailable)
                        {
                            PutAirplaneInFirstStation(flight, station);
                            break;
                        }
                        if (i == departuresLandingStations.Count - 1)
                        {
                            PutAirplaneInWaitingList(flight.Airplane, flight);
                        }
                    }
                    break;
                default:
                    break;
            }
            connection.InvokeAsync("StationDataChanged", Stations);
            SaveChanges();
        }

        private void PutAirplaneInFirstStation(Flight flight, Station station)
        {
            station.AirplaneId = flight.AirplaneId;
            station.IsAvailable = false;
            historyBL.StartRecord(station, flight);
            airplaneBL.WaitInStation(flight.Airplane, flight);
            KeyValuePair<Airplane, Flight> keyPair = waitingToMoveAirplanes.FirstOrDefault(kp => (kp.Key == flight.Airplane && kp.Value == flight));
            if (!keyPair.Equals(default(KeyValuePair<Airplane, Flight>)))
                waitingToMoveAirplanes.Remove(keyPair);
        }

        private void MoveAirplaneToNextStation(Airplane airplane, Flight flight)
        {
            lock (moveAirplane)
            {
                Timer timer = new Timer();
                if (airplane.ReadyForNextStation)
                {
                    Station currentStation = Stations.FirstOrDefault(s => s.AirplaneId == airplane.Id);
                    if (currentStation != null)
                    {
                        switch (flight.FlightType)
                        {
                            case FlightType.Arrival:
                                int arrivalCourseIdx = arrivalCourseList.IndexOf(currentStation.Id);
                                for (int i = 0; i < arrivalTakeoffStations.Count; i++)
                                {
                                    if (currentStation.Id == arrivalTakeoffStations[i])
                                    {
                                        SetStationFree(airplane, flight, currentStation);
                                        return;
                                    }
                                }
                                Station nextArrivalStation;
                                if (currentStation.Id == arrivalCourseList[arrivalCourseList.Count - 1])
                                {
                                    for (int i = 0; i < arrivalTakeoffStations.Count; i++)
                                    {
                                        nextArrivalStation = Stations.FirstOrDefault(s => s.Id == arrivalTakeoffStations[i]);
                                        if (nextArrivalStation.IsAvailable)
                                        {
                                            PutAirplaneInNextStation(airplane, flight, currentStation, nextArrivalStation);
                                            break;
                                        }
                                        if (i == arrivalTakeoffStations.Count - 1)
                                        {
                                            PutAirplaneInWaitingList(airplane, flight);
                                        }
                                    }
                                }
                                else
                                {
                                    nextArrivalStation = Stations.FirstOrDefault(s => s.Id == arrivalCourseList[arrivalCourseIdx + 1]);
                                    if (nextArrivalStation.IsAvailable)
                                    {
                                        PutAirplaneInNextStation(airplane, flight, currentStation, nextArrivalStation);
                                        break;
                                    }
                                    else
                                    {
                                        PutAirplaneInWaitingList(airplane, flight);
                                    }
                                }
                                break;
                            case FlightType.Departure:
                                int departureCourseIdx = departureCourseList.IndexOf(currentStation.Id);
                                for (int i = 0; i < departuresTakeoffStations.Count; i++)
                                {
                                    if (currentStation.Id == departuresTakeoffStations[i])
                                    {
                                        SetStationFree(airplane, flight, currentStation);
                                        return;
                                    }
                                }
                                Station nextDepartureStation;
                                if (currentStation.Id == departureCourseList[departureCourseList.Count - 1])
                                {
                                    for (int i = 0; i < departuresTakeoffStations.Count; i++)
                                    {
                                        nextDepartureStation = Stations.FirstOrDefault(s => s.Id == departuresTakeoffStations[i]);
                                        if (nextDepartureStation.IsAvailable)
                                        {
                                            PutAirplaneInNextStation(airplane, flight, currentStation, nextDepartureStation);
                                            break;
                                        }
                                        if (i == departuresTakeoffStations.Count - 1)
                                        {
                                            PutAirplaneInWaitingList(airplane, flight);
                                        }
                                    }
                                }
                                else
                                {
                                    nextDepartureStation = Stations.FirstOrDefault(s => s.Id == departureCourseList[departureCourseIdx + 1]);
                                    if (nextDepartureStation.IsAvailable)
                                    {
                                        PutAirplaneInNextStation(airplane, flight, currentStation, nextDepartureStation);
                                        break;
                                    }
                                    else
                                    {
                                        PutAirplaneInWaitingList(airplane, flight);
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                connection.InvokeAsync("StationDataChanged", Stations);
                SaveChanges();
            }
        }

        private void SetStationFree(Airplane airplane, Flight flight, Station currentStation)
        {
            airplaneBL.SetArplaneFree(currentStation.AirplaneId);
            flight.IsOver = true;
            historyBL.EndRecord(currentStation, flight);
            currentStation.AirplaneId = 0;
            currentStation.IsAvailable = true;
            KeyValuePair<Airplane, Flight> keyPair = waitingToMoveAirplanes.FirstOrDefault(kp => (kp.Key == airplane && kp.Value == flight));
            if (!keyPair.Equals(default(KeyValuePair<Airplane, Flight>)))
                waitingToMoveAirplanes.Remove(keyPair);
            StationIsAvailable?.Invoke();
            connection.InvokeAsync("StationDataChanged", Stations);
            SaveChanges();
        }

        private void PutAirplaneInWaitingList(Airplane airplane, Flight flight)
        {
            KeyValuePair<Airplane, Flight> keyPair = waitingToMoveAirplanes.FirstOrDefault(kp => (kp.Key == airplane && kp.Value == flight));
            if (keyPair.Equals(default(KeyValuePair<Airplane, Flight>)))
                waitingToMoveAirplanes.Add(new KeyValuePair<Airplane, Flight>(airplane, flight));
        }

        private void PutAirplaneInNextStation(Airplane airplane, Flight flight, Station currentStation, Station nextStation)
        {
            historyBL.EndRecord(currentStation, flight);
            currentStation.IsAvailable = true;
            currentStation.AirplaneId = 0;
            nextStation.AirplaneId = airplane.Id;
            nextStation.IsAvailable = false;
            historyBL.StartRecord(nextStation, flight);
            airplaneBL.WaitInStation(airplane, flight);
            KeyValuePair<Airplane, Flight> keyPair = waitingToMoveAirplanes.FirstOrDefault(kp => (kp.Key == airplane && kp.Value == flight));
            if (!keyPair.Equals(default(KeyValuePair<Airplane, Flight>)))
                waitingToMoveAirplanes.Remove(keyPair);
            StationIsAvailable?.Invoke();
        }

        private void StartAirplanesMovement()
        {
            foreach (Station station in Stations)
            {
                if (station.AirplaneId > 0)
                {
                    Airplane airplane = airplaneBL.GetAirplaneById(station.AirplaneId);
                    if (airplane != null)
                    {
                        airplane.ReadyForNextStation = true;
                        Flight flight = flightBL.GetAirplaneFlight(airplane.Id);
                        if (DateTime.Now > flight.FlightTime)
                            flight.Delayed = true;
                        Task.Run(() =>
                        {
                            MoveAirplaneToNextStation(airplane, flight);
                        });
                    }
                }
            }
        }

        private void SaveChanges()
        {
            Task.Run(() =>
            {
                repository.Save();
            });
        }

        private void PlaceWaitingAirplanes()
        {
            for (int i = 0; i < waitingToMoveAirplanes.Count; i++)
            {
                MoveAirplaneToNextStation(waitingToMoveAirplanes[i].Key, waitingToMoveAirplanes[i].Value);
            }
        }
    }
}
