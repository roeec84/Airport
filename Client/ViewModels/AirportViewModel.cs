using Client.Services;
using Common.Models;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Client.ViewModels
{
    public class AirportViewModel : ViewModelBase
    {
        public readonly IHubService hubService;
        private ObservableCollection<Station> stations;

        public ObservableCollection<Station> Stations { get => stations; set => Set(ref stations, value); }

        public AirportViewModel(IHubService hubService)
        {
            this.hubService = hubService;
            hubService.StationsUpdated += StationsUpdated;
            Stations = new ObservableCollection<Station>();
            GetStations();
        }

        private void StationsUpdated(List<Station> stations)
        {
            Stations = new ObservableCollection<Station>(stations);
        }

        private void GetStations()
        {
            Dispatcher.CurrentDispatcher.InvokeAsync(async () =>
            {
                var stations = await hubService.GetStationsAsync();
                Stations = new ObservableCollection<Station>(stations);
            });
        }
    }
}
