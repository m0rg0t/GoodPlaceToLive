using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using ClearSpendingSDK;
using ClearSpendingSDK.Models;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoodPlaceToLive.Models
{
    public class HospitalAdultItem: BasePlaceItem
    {
        private string _rownum;
        /// <summary>
        /// String id
        /// </summary>
        public string Rownum
        {
            get { return _rownum; }
            set { _rownum = value; }
        }

        private string _fullName;
        /// <summary>
        /// 
        /// </summary>
        public string FullName
        {
            get { return _fullName; }
            set { _fullName = value; }
        }

        private string _shortName;
        /// <summary>
        /// 
        /// </summary>
        public string ShortName
        {
            get { return _shortName; }
            set { _shortName = value; }
        }

        private string _area;
        /// <summary>
        /// 
        /// </summary>
        public string Area
        {
            get { return _area; }
            set { _area = value; }
        }

        private string _district;
        /// <summary>
        /// 
        /// </summary>
        public string District
        {
            get { return _district; }
            set { _district = value; }
        }

        private string _postalCode;
        /// <summary>
        /// 
        /// </summary>
        public string PostalCode
        {
            get { return _postalCode; }
            set { _postalCode = value; }
        }

        private string _address;
        /// <summary>
        /// 
        /// </summary>
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        private string _chiefName;
        /// <summary>
        /// 
        /// </summary>
        public string ChiefName
        {
            get { return _chiefName; }
            set { _chiefName = value; }
        }

        private string _chiefPosition;
        /// <summary>
        /// 
        /// </summary>
        public string ChiefPosition
        {
            get { return _chiefPosition; }
            set { _chiefPosition = value; }
        }

        private string _chiefGender;
        /// <summary>
        /// 
        /// </summary>
        public string ChiefGender
        {
            get { return _chiefGender; }
            set { _chiefGender = value; }
        }

        private string _publicPhone;
        /// <summary>
        /// 
        /// </summary>
        public string PublicPhone
        {
            get
            {
                if (String.IsNullOrEmpty(_publicPhone))
                {
                    return "Не указан";
                }
                return _publicPhone;
            }
            set { _publicPhone = value; }
        }

        private string _fax;
        /// <summary>
        /// 
        /// </summary>
        public string Fax
        {
            get
            {
                if (String.IsNullOrEmpty(_fax))
                {
                    return "Не указан";
                }
                return _fax;
            }
            set { _fax = value; }
        }

        private string _email;
        /// <summary>
        /// 
        /// </summary>
        public string Email
        {
            get
            {
                if (String.IsNullOrEmpty(_email))
                {
                    return "Не указан";
                }
                return _email;
            }
            set { _email = value; }
        }

        private string _PaidServicesInfo;
        /// <summary>
        /// 
        /// </summary>
        public string PaidServicesInfo
        {
            get { return _PaidServicesInfo; }
            set { _PaidServicesInfo = value; }
        }

        private string _FreeServicesInfo;
        /// <summary>
        /// 
        /// </summary>
        public string FreeServicesInfo
        {
            get { return _FreeServicesInfo; }
            set { _FreeServicesInfo = value; }
        }

        private string _WorkingHours;
        /// <summary>
        /// 
        /// </summary>
        public string WorkingHours
        {
            get
            {
                if (String.IsNullOrEmpty(_WorkingHours))
                {
                    return "Не указаны";
                }
                return _WorkingHours;
            }
            set { _WorkingHours = value; }
        }

        private string _Specialization;
        /// <summary>
        /// 
        /// </summary>
        public string Specialization
        {
            get { return _Specialization; }
            set { _Specialization = value; }
        }

        private string _Bedspace;
        /// <summary>
        /// 
        /// </summary>
        public string Bedspace
        {
            get { return _Bedspace; }
            set { _Bedspace = value; }
        }

        private string _Departments;
        /// <summary>
        /// 
        /// </summary>
        public string Departments
        {
            get { return _Departments; }
            set { _Departments = value; }
        }

        private string _ExtraInfo;
        /// <summary>
        /// 
        /// </summary>
        public string ExtraInfo
        {
            get { return _ExtraInfo; }
            set { _ExtraInfo = value; }
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

        private string _globalid;

        public string GLOBALID
        {
            get { return _globalid; }
            set { _globalid = value; }
        }

        /*private string _id;
        /// <summary>
        /// 
        /// </summary>
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }*/

        [IgnoreDataMember]
        public string Image
        {
            private set
            {
            }
            get
            {
                string outStr = "http://maps.googleapis.com/maps/api/streetview?size=200x200&location=" + this.X.Replace(",",".") + ", " + this.Y.Replace(",",".") + "&fov=180&heading=235&pitch=10&sensor=false";
                return outStr;
            }
        }
        

        public async Task<bool> LoadCustomerData()
        {
            ContractSearch search = new ContractSearch();
            SearchParamItem searchParams = new SearchParamItem();
            searchParams.CurrentType = SearchParamItem.SearchType.Customers;
            searchParams.Customer.Namesearch = this.FullName;
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
