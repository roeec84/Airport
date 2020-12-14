using System;
using System.Collections.Generic;
using System.Text;

namespace Client.ViewModels
{
    public class ViewModelLocator
    {
        public FlightsViewModel Flights => (FlightsViewModel)App.Provider.GetService(typeof(FlightsViewModel));
        public AirportViewModel Airport => (AirportViewModel)App.Provider.GetService(typeof(AirportViewModel));
    }
}
