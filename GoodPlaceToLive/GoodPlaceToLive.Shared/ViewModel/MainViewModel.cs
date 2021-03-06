﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using Callisto.Controls;
using GoodPlaceToLive.Models;
using Microsoft.WindowsAzure.MobileServices;

namespace GoodPlaceToLive.ViewModel
{
    public class MainViewModel : ViewModelBase
    {


        public MainViewModel()
        {
            /*HelloWorld = IsInDesignMode
                ? "Runs in design mode"
                : "Runs in runtime mode";*/
            LoadData();
        }

        public async void LoadData()
        {
            this.Loading = true;

            Items = new ObservableCollection<BasePlaceItem>();
            await LoadHospitalsData();
            await LoadChildPlacesData();
            await LoadMinFinData();
            await GetNearestItems();

            this.Loading = false;
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
            try
            {
                var nitems = Items.Where(c => c.Distance < 3);
                double count = 0;
                NearestItems = new ObservableCollection<BasePlaceItem>();
                foreach (var item in nitems)
                {
                    count = count + item.PlaceCoefficient;
                    NearestItems.Add(item);
                }
                CurrentCoefficient = Math.Round(count);
                OnNearestChanged(null);
            }
            catch
            {
            }
            return true;
        }

        //public event EventHandler<MyEventArgs> Changed;
        //public event EventHandler NearestChanged;
        protected virtual void OnNearestChanged(EventArgs e)
        {
            EventHandler handler = NearestChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler NearestChanged;

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
                return HospitalItems.OrderByDescending(c => c.PlaceCoefficient).Take(6).ToList();
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

        private ChildPlaceItem _currentChildItem = new ChildPlaceItem();
        public ChildPlaceItem CurrentChildItem
        {
            get { return _currentChildItem; }
            set
            {
                _currentChildItem = value;
                RaisePropertyChanged("CurrentChildItem");
            }
        }

        private MinFinItem _currentMinFinItem = new MinFinItem();
        /// <summary>
        /// 
        /// </summary>
        public MinFinItem CurrentMinFinItem
        {
            get { return _currentMinFinItem; }
            set
            {
                _currentMinFinItem = value;
                RaisePropertyChanged("CurrentMinFinItem");
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
                OnLoadedChanged(null);
                RaisePropertyChanged("Loading");
            }
        }

        protected virtual void OnLoadedChanged(EventArgs e)
        {
            EventHandler handler = onLoaded;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        public event EventHandler onLoaded;

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

        /*private Geoposition _myCoordinate;
        /// <summary>
        /// My current coordinates
        /// </summary>
        public Geoposition MyCoordinate
        {
            get { return _myCoordinate; }
            set
            {
                _myCoordinate = value;
                RaisePropertyChanged("MyCoordinate");
            }
        }*/

        private double _lat = 55.75;
        /// <summary>
        /// 
        /// </summary>
        public double Latitude
        {
            get { return _lat; }
            set
            {
                _lat = value;
                RaisePropertyChanged("Latitude");
            }
        }

        private double _lon = 37.6;
        /// <summary>
        /// 
        /// </summary>
        public double Longitude
        {
            get { return _lon; }
            set { _lon = value; RaisePropertyChanged("Longitude"); }
        }
        
        

        private MobileServiceCollection<HospitalAdultItem, HospitalAdultItem> hospitalAdultsItems;
        private IMobileServiceTable<HospitalAdultItem> HospitalsTable = App.MobileService.GetTable<HospitalAdultItem>();

        public async Task<bool> LoadHospitalsData()
        {
            this.Loading = true;
            try
            {
                HospitalItems = await HospitalsTable.ToCollectionAsync(100);
                try
                {
                    Geolocator _geolocator = new Geolocator();
                    Geoposition pos = await _geolocator.GetGeopositionAsync();
                    Geocoordinate posGeo = pos.Coordinate;
                    //MyCoordinate = pos;
                    Latitude = pos.Coordinate.Latitude;
                    Longitude = pos.Coordinate.Longitude;
                }
                catch
                {
                    //Geocoordinate posGeo = new Geocoordinate();
                }
                //Location mylocation = new Location(pos.Coordinate.Point.Position.Latitude, pos.Coordinate.Point.Position.Longitude);
                foreach (var item in HospitalItems)
                {
                    item.Latitude = Double.Parse(item.Y);
                    item.Longitude = Double.Parse(item.X);

                    item.CalculateDistance(Latitude, Longitude);
                    Items.Add(item);
                }
            }
            catch
            {
            }
            RaisePropertyChanged("BestAllHospitalItems");
            RaisePropertyChanged("BestHospitalItems");
            RaisePropertyChanged("HospitalItems");
            RaisePropertyChanged("ShortHospitalItems");
            this.Loading = false;

            return true;
        }

        private ObservableCollection<ChildPlaceItem> _childPlaceItems = new ObservableCollection<ChildPlaceItem>();
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<ChildPlaceItem> ChildPlaceItems
        {
            get { return _childPlaceItems; }
            set
            {
                _childPlaceItems = value;
                RaisePropertyChanged("ChildPlacesItems");
            }
        }

        private ObservableCollection<MinFinItem> _minFinItems = new ObservableCollection<MinFinItem>();

        public ObservableCollection<MinFinItem> MinFinItems
        {
            get { return _minFinItems; }
            set
            {
                _minFinItems = value;
                RaisePropertyChanged("MinFinItems");
            }
        }

        public List<MinFinItem> BestMinFinItems
        {
            get
            {
                return MinFinItems.OrderByDescending(c => c.PlaceCoefficient).Take(9).ToList();
            }
            private set { }
        }
        

        public List<ChildPlaceItem> BestChildPlaceItems
        {
            get
            {
                return ChildPlaceItems.OrderByDescending(c => c.PlaceCoefficient).Take(9).ToList();
            }
            private set { }
        }

        private MobileServiceCollection<ChildPlaceItem, ChildPlaceItem> ChildPlacesItems;
        private IMobileServiceTable<ChildPlaceItem> ChildPlacesTable = App.MobileService.GetTable<ChildPlaceItem>();

        private MobileServiceCollection<MinFinItem, MinFinItem> minFinItems;
        private IMobileServiceTable<MinFinItem> MinFinTable = App.MobileService.GetTable<MinFinItem>();

        public async Task<bool> LoadMinFinData()
        {
            this.Loading = true;
            try
            {
                MinFinItems = await MinFinTable.ToCollectionAsync(999);
                try
                {
                    Geolocator _geolocator = new Geolocator();
                    Geoposition pos = await _geolocator.GetGeopositionAsync();
                    Geocoordinate posGeo = pos.Coordinate;

                    //MyCoordinate = pos;
                    Longitude = pos.Coordinate.Longitude;
                    Latitude = pos.Coordinate.Latitude;
                }
                catch
                {
                }
                //MinFinItems = new ObservableCollection<MinFinItem>();
                //Location mylocation = new Location(pos.Coordinate.Point.Position.Latitude, pos.Coordinate.Point.Position.Longitude);
                foreach (var item in MinFinItems)
                {
                    item.Latitude = Double.Parse(item.Y.Replace(".",","));
                    item.Longitude = Double.Parse(item.X.Replace(".", ","));
                    Items.Add(item);

                    item.CalculateDistance(Latitude, Longitude);
                    //MinFinItems.Add(item);
                }
            }
            catch
            {
            }
            RaisePropertyChanged("MinFinItems");
            RaisePropertyChanged("BestMinFinItems");
            this.Loading = false;
            return true;
        }

        public async Task<bool> LoadChildPlacesData()
        {
            this.Loading = true;
            try
            {
                ChildPlacesItems = await ChildPlacesTable.ToCollectionAsync(999);
                try
                {
                    Geolocator _geolocator = new Geolocator();
                    Geoposition pos = await _geolocator.GetGeopositionAsync();
                    Geocoordinate posGeo = pos.Coordinate;

                    //MyCoordinate = pos;
                    Longitude = pos.Coordinate.Longitude;
                    Latitude = pos.Coordinate.Latitude;
                }
                catch
                {
                }
                //ChildPlaceItems = new ObservableCollection<ChildPlaceItem>();
                //Location mylocation = new Location(pos.Coordinate.Point.Position.Latitude, pos.Coordinate.Point.Position.Longitude);
                foreach (var item in ChildPlacesItems)
                {
                    item.Latitude = Double.Parse(item.Y);
                    item.Longitude = Double.Parse(item.X);
                    Items.Add(item);

                    item.CalculateDistance(Latitude, Longitude);
                    ChildPlaceItems.Add(item);
                }
            }
            catch
            {
            }
            RaisePropertyChanged("ChildPlaceItems");
            RaisePropertyChanged("BestChildPlaceItems");
            this.Loading = false;
            return true;
        }

        public void UpdateDistances()
        {
            Items = new ObservableCollection<BasePlaceItem>();
            foreach (var item in ChildPlacesItems)
            {
                item.CalculateDistance(Latitude, Longitude);
                Items.Add(item);
            }
            foreach (var item in HospitalItems)
            {
                item.CalculateDistance(Latitude, Longitude);
                Items.Add(item);
            } 
            //throw new NotImplementedException();
        }
    }
}
