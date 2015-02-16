using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using ClearSpendingSDK;
using ClearSpendingSDK.Models;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace GoodPlaceToLive.Models
{
    public class MinFinItem : BasePlaceItem
    {
        //public string Id { get; set; }

        private string _address;

        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        private string _name;
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _x;
        /// <summary>
        /// 
        /// </summary>
        public string X
        {
            get { return _x; }
            set { _x = value; }
        }

        private string _y;
        /// <summary>
        /// 
        /// </summary>
        public string Y
        {
            get { return _y; }
            set { _y = value; }
        }

        [IgnoreDataMember]
        public string Image
        {
            private set
            {
            }
            get
            {
                string outStr = "http://maps.googleapis.com/maps/api/streetview?size=500x400&location=" + this.Y.Replace(",", ".") + ", " + this.X.Replace(",", ".") + "&fov=180&heading=235&pitch=10&sensor=false";
                return outStr;
            }
        }

        private string _phone;

        public string Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }

        private string _site;

        public string Site
        {
            get { return _site; }
            set { _site = value; }
        }

        private string _email;

        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        /*private string _global_id;

        public string Global_id
        {
            get { return _global_id; }
            set { _global_id = value; }
        }*/

        private string _fax;
        /// <summary>
        /// факс
        /// </summary>
        public string Fax
        {
            get { return _fax; }
            set { _fax = value; }
        }

        private string _functions;
        /// <summary>
        /// 
        /// </summary>
        public string Functions
        {
            get { return _functions; }
            set { _functions = value; }
        }

        private string _officialAddress;

        public string OfficialAddress
        {
            get { return _officialAddress; }
            set { _officialAddress = value; }
        }

        private string _chief;
        /// <summary>
        /// 
        /// </summary>
        public string Chief
        {
            get { return _chief; }
            set { _chief = value; }
        }
        


        public async Task<bool> LoadCustomerData()
        {
            ContractSearch search = new ContractSearch();
            SearchParamItem searchParams = new SearchParamItem();
            searchParams.CurrentType = SearchParamItem.SearchType.Customers;
            searchParams.Customer.Namesearch = this.Name;
            await search.StartSearch(searchParams);

            try
            {
                var items = search.CustomerItems;
                var item = items.FirstOrDefault();
                if (item != null)
                {
                    Customer = item;
                    this.ContractCount = Customer.ContractsCount;
                    this.ContractSum = Customer.ContractsSum;
                }
            }
            catch
            {
            }

            return true;
        }

        [IgnoreDataMember]
        private CustomerItem _customer;
        /// <summary>
        /// 
        /// </summary>
        [IgnoreDataMember]
        public CustomerItem Customer
        {
            get { return _customer; }
            set
            {
                _customer = value;
                RaisePropertyChanged("Customer");
            }
        }

    }
}
