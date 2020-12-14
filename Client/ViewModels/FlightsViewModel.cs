using Client.Services;
using Common.Models;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public class FlightsViewModel : ViewModelBase
    {
        private readonly IHubService hubService;
        private ObservableCollection<Flight> flights;
        private ObservableCollection<Flight> arrivals;
        private ObservableCollection<Flight> departures;

        public ObservableCollection<Flight> Flights { get => flights; set => Set(ref flights, value); }
        public ObservableCollection<Flight> Arrivals { get => arrivals; set => Set(ref arrivals, value); }
        public ObservableCollection<Flight> Departures { get => departures; set => Set(ref departures, value); }

        public FlightsViewModel(IHubService hubService)
        {
            this.hubService = hubService;
            Flights = new ObservableCollection<Flight>();
            Arrivals = new ObservableCollection<Flight>();
            Departures = new ObservableCollection<Flight>();
            hubService.ReceivedNewFlight += ReceiveNewFlight;
            GetFlights();
        }

        private void ReceiveNewFlight(List<Flight> flights)
        {
            ArrangeFlights(flights);
        }

        private void GetFlights()
        {
            Task.Run(async () =>
            {
                List<Flight> flights = await hubService.GetFlightsAsync();
                ArrangeFlights(flights);
            });
        }

        private void ArrangeFlights(List<Flight> flights)
        {
            Flights = new ObservableCollection<Flight>(flights);
            Arrivals.Clear();
            Departures.Clear();
            foreach (Flight flight in Flights)
            {
                AssignFlightToList(flight);
            }
        }

        private void AssignFlightToList(Flight flight)
        {
            switch (flight.FlightType)
            {
                case FlightType.Arrival:
                    Arrivals.Add(flight);
                    break;
                case FlightType.Departure:
                    Departures.Add(flight);
                    break;
            }
        }
    }
}
