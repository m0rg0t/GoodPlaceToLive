using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Bing.Maps;
using GoodPlaceToLive.Controls;
using GoodPlaceToLive.Data;
using GoodPlaceToLive.Common;
using GoodPlaceToLive.Models;
using GoodPlaceToLive.Pages;
using GoodPlaceToLive.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Microsoft.WindowsAzure.MobileServices;
using System.Net.Http;

// Документацию по шаблону проекта "Универсальное приложение с Hub" см. по адресу http://go.microsoft.com/fwlink/?LinkID=391955
using Newtonsoft.Json;

namespace GoodPlaceToLive
{
    /// <summary>
    /// Страница, на которой отображается сгруппированная коллекция элементов.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private MobileServiceCollection<HospitalAdultItem, HospitalAdultItem> parkItems;
        private IMobileServiceTable<HospitalAdultItem> HospitalsTable = App.MobileService.GetTable<HospitalAdultItem>();

        private MobileServiceCollection<ChildPlaceItem, ChildPlaceItem> childItems;
        private IMobileServiceTable<ChildPlaceItem> ChildTable = App.MobileService.GetTable<ChildPlaceItem>();

        /// <summary>
        /// Получает NavigationHelper, используемый для облегчения навигации и управления жизненным циклом процессов.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        void Settings_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            try
            {
                var viewAboutPage = new SettingsCommand("", "Об авторе", cmd =>
                {
                    //(Window.Current.Content as Frame).Navigate(typeof(AboutPage));
                    var settingsFlyout = new Callisto.Controls.SettingsFlyout();
                    settingsFlyout.Content = new About();
                    settingsFlyout.HeaderText = "Об авторе";

                    settingsFlyout.IsOpen = true;
                });
                args.Request.ApplicationCommands.Add(viewAboutPage);

                var viewAboutMalukahPage = new SettingsCommand("", "Политика конфиденциальности", cmd =>
                {
                    var settingsFlyout = new Callisto.Controls.SettingsFlyout();
                    settingsFlyout.Content = new Privacy();
                    settingsFlyout.HeaderText = "Политика конфиденциальности";

                    settingsFlyout.IsOpen = true;
                });
                args.Request.ApplicationCommands.Add(viewAboutMalukahPage);
            }
            catch { };
        }

        /// <summary>
        /// Получает DefaultViewModel. Эту модель можно изменить на модель строго типизированных представлений.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public HubPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
        }

        /// <summary>
        /// Заполняет страницу содержимым, передаваемым в процессе навигации.  Также предоставляется любое сохраненное состояние
        /// при повторном создании страницы из предыдущего сеанса.
        /// </summary>
        /// <param name="sender">
        /// Источник события; как правило, <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Данные события, предоставляющие параметр навигации, который передается
        /// <see cref="Frame.Navigate(Type, object)"/> при первоначальном запросе этой страницы, и
        /// словарь состояний, сохраненных этой страницей в ходе предыдущего
        /// сеанса.  Это состояние будет равно NULL при первом посещении страницы.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Создание соответствующей модели данных для области проблемы, чтобы заменить пример данных
            var sampleDataGroup = await SampleDataSource.GetGroupAsync("Group-4");
            this.DefaultViewModel["Section3Items"] = sampleDataGroup;

            var rmain = ServiceLocator.Current.GetInstance<MainViewModel>();
            rmain.NearestChanged+=rmain_NearestChanged;

            LoadCSV();
        }

        void rmain_NearestChanged(object sender, EventArgs e)
        {
            var rmain = ServiceLocator.Current.GetInstance<MainViewModel>();
            placeMap.SetView(new Location(rmain.MyCoordinate.Latitude, rmain.MyCoordinate.Longitude), 12);
            placeMap.Children.Clear();
            foreach (BasePlaceItem item in rmain.NearestItems)
            {
                Pushpin pushpin = new Pushpin();
                var location = new Location((item.Latitude), (item.Longitude));
                MapLayer.SetPosition(pushpin, location);
                //pushpin.Background = new SolidColorBrush() { Color = new Color() {A=0, B=0, G=0, R=255}};
                pushpin.Name = item.Id.ToString();
                //pushpin.Tapped += pushpinTapped;
                placeMap.Children.Add(pushpin);
            };
            //throw new NotImplementedException();
        }

        public async void LoadCSV()
        {
            /*HttpClient client = new HttpClient();
            string earthQuakeData = await client.GetStringAsync("http://goodplacetolive.azurewebsites.net/child.txt");
            var results = JsonConvert.DeserializeObject<ObservableCollection<ChildPlaceItem>>(earthQuakeData.ToString());
            foreach (var item in results)
            {
                await item.LoadCustomerData();
                await ChildTable.InsertAsync(item);
            }*/
            Debug.WriteLine("Import finished");
        }

        /*public async void LoadCSV()
        {
            HttpClient client = new HttpClient();
            string earthQuakeData = await client.GetStringAsync("http://goodplacetolive.azurewebsites.net/hospital_adult1.txt");
            var results = JsonConvert.DeserializeObject<ObservableCollection<HospitalAdultItem>>(earthQuakeData.ToString());
            foreach (var item in results)
            {
                await item.LoadCustomerData();
                await HospitalsTable.InsertAsync(item);
            }
            Debug.WriteLine("Import finished");
        }*/

        /// <summary>
        /// Вызывается при щелчке заголовка HubSection.
        /// </summary>
        /// <param name="sender">Главная страница, которая содержит элемент HubSection, заголовок которого щелкнул пользователь.</param>
        /// <param name="e">Данные события, описывающие, каким образом был инициирован щелчок.</param>
        void Hub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            HubSection section = e.Section;
            //var group = section.DataContext;
            //this.Frame.Navigate(typeof(SectionPage), ((SampleDataGroup)group).UniqueId);
            switch (e.Section.Name)
            {
                case "Hospitals":
                    this.Frame.Navigate(typeof(HospitalsListPage));
                    break;
                case "ChildPlaces":
                    this.Frame.Navigate(typeof (ChildPlacesListPage));
                    break;
                case "PlaceInfo":
                    this.Frame.Navigate(typeof (PlaceChangePage));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Вызывается при нажатии элемента внутри раздела.
        /// </summary>
        /// <param name="sender">Представление сетки или списка
        /// в котором отображается нажатый элемент.</param>
        /// <param name="e">Данные о событии, описывающие нажатый элемент.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Переход к соответствующей странице назначения и настройка новой страницы
            // путем передачи необходимой информации в виде параметра навигации
            
            //var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            //this.Frame.Navigate(typeof(ItemPage), itemId);
            try
            {
                var item = ((HospitalAdultItem)e.ClickedItem);
                var rmain = ServiceLocator.Current.GetInstance<MainViewModel>();
                rmain.CurrentItem = item;
                this.Frame.Navigate(typeof(HospitalDetailPage));
            }
            catch
            {
            }
            //item.LoadCustomerData();
        }
        #region Регистрация NavigationHelper

        /// <summary>
        /// Методы, предоставленные в этом разделе, используются исключительно для того, чтобы
        /// NavigationHelper для отклика на методы навигации страницы.
        /// Логика страницы должна быть размещена в обработчиках событий для 
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// и <see cref="Common.NavigationHelper.SaveState"/>.
        /// Параметр навигации доступен в методе LoadState 
        /// в дополнение к состоянию страницы, сохраненному в ходе предыдущего сеанса.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SettingsPane.GetForCurrentView().CommandsRequested += Settings_CommandsRequested;
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            SettingsPane.GetForCurrentView().CommandsRequested -= Settings_CommandsRequested;
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private Map placeMap;

        private void MapPlace_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                placeMap = ((Map)sender);
                rmain_NearestChanged(placeMap, null);
            }
            catch
            {
            }
        }

        private void ItemChildPlacesGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                var item = ((ChildPlaceItem)e.ClickedItem);
                var rmain = ServiceLocator.Current.GetInstance<MainViewModel>();
                rmain.CurrentChildItem = item;
                this.Frame.Navigate(typeof(ChildPlacesDetailPage));
            }
            catch
            {
            }
        }
    }
}
