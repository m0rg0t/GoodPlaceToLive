using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;
using GoodPlaceToLive.Models;
using Microsoft.WindowsAzure.MobileServices;

namespace GoodPlaceToLive.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string _helloWorld;

        public string HelloWorld
        {
            get
            {
                return _helloWorld;
            }
            set
            {
                Set(() => HelloWorld, ref _helloWorld, value);
            }
        }

        public MainViewModel()
        {
            HelloWorld = IsInDesignMode
                ? "Runs in design mode"
                : "Runs in runtime mode";
            LoadData();
        }

        public async void LoadData()
        {
            await LoadHospitalsData();
            await GetNearestItems();
        }

        private ObservableCollection<HospitalAdultItem> _hospitalItems = new ObservableCollection<HospitalAdultItem>();
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<HospitalAdultItem> HospitalItems
        {
            get
            {
                return _hospitalItems;
            }
            set
            {
                _hospitalItems = value;
                RaisePropertyChanged("HospitalItems");
                RaisePropertyChanged("ShortHospitalItems");
                RaisePropertyChanged("MostFoundedHospitalItems");
                RaisePropertyChanged("BestHospitalItems");
            }
        }

        public List<HospitalAdultItem> ShortHospitalItems
        {
            private set { }
            get
            {
                try
                {
                    return HospitalItems.Take(9).ToList();
                }
                catch
                {
                    return new List<HospitalAdultItem>();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> GetNearestItems()
        {
            var nitems = HospitalItems.Where(c => c.Distance < 3);
            double count = 0;
            NearestItems = new ObservableCollection<BasePlaceItem>();
            foreach (var item in nitems)
            {
                count = count + item.PlaceCoefficient;
                NearestItems.Add(item);
            }
            CurrentCoefficient = Math.Round(count);
            return true;
        }

        private ObservableCollection<BasePlaceItem> _nearestItems = new ObservableCollection<BasePlaceItem>();
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<BasePlaceItem> NearestItems
        {
            get { return _nearestItems; }
            set
            {
                _nearestItems = value;
                RaisePropertyChanged("NearestItems");
            }
        }

        private ObservableCollection<BasePlaceItem> _items = new ObservableCollection<BasePlaceItem>();
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<BasePlaceItem> Items
        {
            get { return _items; }
            set { _items = value; }
        }
        
        

        public List<HospitalAdultItem> BestHospitalItems
        {
            get
            {
                return HospitalItems.OrderByDescending(c => c.PlaceCoefficient).Take(9).ToList();
            }
            private set { }
        }

        public List<HospitalAdultItem> BestAllHospitalItems
        {
            get
            {
                return HospitalItems.OrderByDescending(c => c.PlaceCoefficient).ToList();
            }
            private set { }
        }

        private HospitalAdultItem _currentItem = new HospitalAdultItem();
        /// <summary>
        /// current hospital item
        /// </summary>
        public HospitalAdultItem CurrentItem
        {
            get { return _currentItem; }
            set
            {
                _currentItem = value;
                RaisePropertyChanged("CurrentItem");
            }
        }
        

        private bool _loading = false;
        /// <summary>
        /// Loading progress
        /// </summary>
        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged("Loading");
            }
        }

        private double _CurrentCoefficient = 0;
        /// <summary>
        /// Current place coeffcient
        /// </summary>
        public double CurrentCoefficient
        {
            get { return _CurrentCoefficient; }
            set
            {
                _CurrentCoefficient = value;
                RaisePropertyChanged("CurrentCoefficient");
                RaisePropertyChanged("CurrentCoefficientString");
            }
        }

        public string CurrentCoefficientString
        {
            private set { }
            get
            {
                //return ContractSum.ToString();
                return string.Format("{0:#,###,##0.00}", CurrentCoefficient);
            }
        }

        private Geocoordinate _myCoordinate;
        /// <summary>
        /// My current coordinates
        /// </summary>
        public Geocoordinate MyCoordinate
        {
            get { return _myCoordinate; }
            set
            {
                _myCoordinate = value;
                RaisePropertyChanged("MyCoordinate");
            }
        }
        

        private MobileServiceCollection<HospitalAdultItem, HospitalAdultItem> hospitalAdultsItems;
        private IMobileServiceTable<HospitalAdultItem> HospitalsTable = App.MobileService.GetTable<HospitalAdultItem>();

        public async Task<bool> LoadHospitalsData()
        {
            this.Loading = true;
            HospitalItems = await HospitalsTable.ToCollectionAsync(100);
            Geolocator _geolocator = new Geolocator();
            Geoposition pos = await _geolocator.GetGeopositionAsync();
            Geocoordinate posGeo = pos.Coordinate;
            MyCoordinate = pos.Coordinate;
            //Location mylocation = new Location(pos.Coordinate.Point.Position.Latitude, pos.Coordinate.Point.Position.Longitude);
            foreach (var item in HospitalItems)
            {
                item.Latitude = Double.Parse(item.Y);
                item.Longitude =  Double.Parse(item.X);

                item.CalculateDistance(posGeo);
            }

            RaisePropertyChanged("BestAllHospitalItems");
            RaisePropertyChanged("BestHospitalItems");
            RaisePropertyChanged("HospitalItems");
            RaisePropertyChanged("ShortHospitalItems");
            this.Loading = false;

            return true;
        }
        
    }
}
