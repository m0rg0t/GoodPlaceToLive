// Test-only stand-in for the MVVM Light notification contract.
// This does not validate the legacy Windows SDK or MVVM Light binaries.
using System.ComponentModel;

namespace GalaSoft.MvvmLight
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

namespace Windows.Devices.Geolocation { public class Geocoordinate { public double Latitude { get; set; } public double Longitude { get; set; } } }
