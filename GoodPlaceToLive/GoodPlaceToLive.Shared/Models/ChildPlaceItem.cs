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
    public class ChildPlaceItem: BasePlaceItem
    {
        public string Id { get; set; }

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

        private string _telephone;

        public string Telephone
        {
            get { return _telephone; }
            set { _telephone = value; }
        }

        private string _web_site;

        public string Web_site
        {
            get { return _web_site; }
            set { _web_site = value; }
        }

        private string _e_mail;

        public string E_mail
        {
            get { return _e_mail; }
            set { _e_mail = value; }
        }

        private string _global_id;

        public string Global_id
        {
            get { return _global_id; }
            set { _global_id = value; }
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
