using Client.Services;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for StationUC.xaml
    /// </summary>
    public partial class StationUC : UserControl
    {
        public Station Station
        {
            get { return (Station)GetValue(StationProperty); }
            set { SetValue(StationProperty, value); }
        }
        private int prevAirplane;
        private Duration duration = new Duration(new TimeSpan(0, 0, 5));
        private DoubleAnimation moveLeft;
        private DoubleAnimation moveRight;
        private IHubService hubService => (IHubService)App.Provider.GetService(typeof(IHubService));
        private List<Flight> flights;
        public Flight CurrentFlight { get; set; }
        public static readonly DependencyProperty StationProperty =
            DependencyProperty.Register("Station", typeof(Station), typeof(StationUC), new PropertyMetadata(null));

        public StationUC()
        {
            InitializeComponent();
            moveLeft = new DoubleAnimation(-70, duration);
            moveRight = new DoubleAnimation(70, duration);
            airplaneStack.RenderTransform = new TranslateTransform();
            hubService.StationsUpdated += OnStationChanged;
            GetFlights();
            hubService.ReceivedNewFlight += NewFlight;
        }

        private void NewFlight(List<Flight> flights)
        {
            this.flights = flights;
        }

        private void OnStationChanged(List<Station> stations)
        {
            CurrentFlight = flights.FirstOrDefault(f => f.AirplaneId == Station.AirplaneId);
            if (Station.AirplaneId > 0 && prevAirplane != Station.AirplaneId)
            {
                prevAirplane = Station.AirplaneId;
                if(CurrentFlight != null)
                {
                    if(Station.Id == 6 || Station.Id == 7)
                    {
                        if(CurrentFlight.FlightType == FlightType.Arrival)
                        {
                            airplaneStack.RenderTransform.BeginAnimation(TranslateTransform.XProperty, moveRight);
                            return;
                        }
                    }
                }
                airplaneStack.RenderTransform.BeginAnimation(TranslateTransform.XProperty, moveLeft);
            }
            else if(Station.AirplaneId == 0)
            {
                airplaneStack.RenderTransform.BeginAnimation(TranslateTransform.XProperty, moveRight);

            }
        }
        private void GetFlights()
        {
            Task.Run(async () =>
            {
                flights = await hubService.GetFlightsAsync();
            });
        }
    }
}
