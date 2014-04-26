using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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
            LoadHospitalsData();
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
        public List<HospitalAdultItem> MostFoundedHospitalItems
        {
            get
            {
                return HospitalItems.OrderByDescending(c=>c.ContractSum).Take(9).ToList();
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

        private MobileServiceCollection<HospitalAdultItem, HospitalAdultItem> hospitalAdultsItems;
        private IMobileServiceTable<HospitalAdultItem> HospitalsTable = App.MobileService.GetTable<HospitalAdultItem>();

        public async Task<bool> LoadHospitalsData()
        {
            this.Loading = true;
            HospitalItems = await HospitalsTable.ToCollectionAsync(100);
            RaisePropertyChanged("HospitalItems");
            RaisePropertyChanged("ShortHospitalItems");
            this.Loading = false;

            return true;
        }
        
    }
}
