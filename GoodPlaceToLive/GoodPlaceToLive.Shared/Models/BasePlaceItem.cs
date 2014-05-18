using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Windows.Devices.Geolocation;
using GalaSoft.MvvmLight;
//using System.Device.Location;
using GoodPlaceToLive.Common;

namespace GoodPlaceToLive.Models
{
    public class BasePlaceItem: ViewModelBase
    {
        public async void CalculateDistance(Geocoordinate coordinates)
        {
            this.Distance = DistanceHelper.Distance(coordinates.Latitude, coordinates.Longitude, Latitude, Longitude, 'K');
        }

        public async void CalculateDistance(double lat, double lon)
        {
            this.Distance = DistanceHelper.Distance(lat, lon, Latitude, Longitude, 'K');
        }

        public string Id { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        private double _distance = 0;
        /// <summary>
        /// Расстояние до места
        /// </summary>
        public double Distance
        {
            get { return _distance; }
            set
            {
                _distance = value;
                RaisePropertyChanged("Distance");
            }
        }

        private double _contractSum = 0;
        /// <summary>
        /// Сумма контрактов
        /// </summary>
        public double ContractSum
        {
            get { return _contractSum; }
            set
            {
                _contractSum = value;
                RaisePropertyChanged("ContractSum");
                RaisePropertyChanged("PlaceCoefficient");
                RaisePropertyChanged("PlaceCoefficientString");
            }
        }

        [IgnoreDataMember]
        public string ContractSumString
        {
            private set { }
            get
            {
                //return ContractSum.ToString();
                return string.Format("{0:#,###,##0.00 руб}", ContractSum);
            }
        }

        private double _contractCount = 0;
        /// <summary>
        /// Количество контрактов
        /// </summary>
        public double ContractCount
        {
            get { return _contractCount; }
            set { _contractCount = value; }
        }


        /// <summary>
        /// Коэффициент места
        /// </summary>
        [IgnoreDataMember]
        public double PlaceCoefficient
        {
            get
            {
                double k = 0;
                if (Distance > 0)
                {
                    k = ContractSum/Distance;
                }
                else
                {
                    k = ContractSum;
                }
                return k;
            }
            private set { }
        }

        [IgnoreDataMember]
        public string PlaceCoefficientString
        {
            private set { }
            get
            {
                //return ContractSum.ToString();
                return string.Format("{0:#,###,##0.00}", PlaceCoefficient);
            }
        }
        

    }
}
