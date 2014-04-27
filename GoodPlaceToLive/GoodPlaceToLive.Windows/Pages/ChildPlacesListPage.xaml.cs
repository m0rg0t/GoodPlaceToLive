using Windows.Devices.Geolocation;
using Windows.UI.ApplicationSettings;
using Bing.Maps;
using GoodPlaceToLive.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Шаблон элемента страницы концентратора задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=321224
using GoodPlaceToLive.Controls;
using GoodPlaceToLive.Models;
using GoodPlaceToLive.ViewModel;
using Microsoft.Practices.ServiceLocation;

namespace GoodPlaceToLive.Pages
{
    /// <summary>
    /// Страница, на которой отображается сгруппированная коллекция элементов.
    /// </summary>
    public sealed partial class ChildPlacesListPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// Эту настройку можно изменить на модель строго типизированных представлений.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper используется на каждой странице для облегчения навигации и 
        /// управление жизненным циклом процесса
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public ChildPlacesListPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
        }


        /// <summary>
        /// Заполняет страницу содержимым, передаваемым в процессе навигации.  Также предоставляется любое сохраненное состояние
        /// при повторном создании страницы из предыдущего сеанса.
        /// </summary>
        /// <param name="sender">
        /// Источник события; как правило, <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Данные события, предоставляющие параметр навигации, который передается
        /// <see cref="Frame.Navigate(Type, Object)"/> при первоначальном запросе этой страницы и
        /// словарь состояний, сохраненных этой страницей в ходе предыдущего
        /// сеанса.  Это состояние будет равно NULL при первом посещении страницы.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Присвоение коллекции привязываемых групп объекту this.DefaultViewModel["Groups"]
        }

        #region Регистрация NavigationHelper

        /// Методы, предоставленные в этом разделе, используются исключительно для того, чтобы
        /// NavigationHelper для отклика на методы навигации страницы.
        /// 
        /// Логика страницы должна быть размещена в обработчиках событий для 
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// и <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// Параметр навигации доступен в методе LoadState 
        /// в дополнение к состоянию страницы, сохраненному в ходе предыдущего сеанса.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SettingsPane.GetForCurrentView().CommandsRequested += Settings_CommandsRequested;
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            SettingsPane.GetForCurrentView().CommandsRequested -= Settings_CommandsRequested;
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void ItemChildGridView_OnItemClick(object sender, ItemClickEventArgs e)
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

        private Geolocator _geolocator = null;

        private async void Map_Loaded(object sender, RoutedEventArgs e)
        {
            var bounds = Window.Current.Bounds;
            double height = bounds.Height;
            double width = bounds.Width;
            ((Map) sender).Height = height;

            var map = ((Map) sender);

            _geolocator = new Geolocator();
            Geoposition pos = await _geolocator.GetGeopositionAsync();
            Location mylocation = new Location(pos.Coordinate.Point.Position.Latitude, pos.Coordinate.Point.Position.Longitude);
            var zoomLevel = 10;
            map.SetView(mylocation, zoomLevel);

            var rmain = ServiceLocator.Current.GetInstance<MainViewModel>();
            foreach (ChildPlaceItem item in rmain.ChildPlaceItems)
            {
                Pushpin pushpin = new Pushpin();
                var location = new Location(Double.Parse(item.Y), Double.Parse(item.X));
                MapLayer.SetPosition(pushpin, location);
                pushpin.Name = item.Id.ToString();
                pushpin.Tapped += pushpinTapped;
                map.Children.Add(pushpin);
            };
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

        private void pushpinTapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                var rmain = ServiceLocator.Current.GetInstance<MainViewModel>();
                rmain.CurrentChildItem = rmain.ChildPlaceItems.FirstOrDefault(c=>c.Id==((Pushpin)sender).Name.ToString());
                if (rmain.CurrentChildItem != null)
                {
                    //var item = ((HospitalAdultItem)e.ClickedItem);
                    this.Frame.Navigate(typeof (ChildPlacesDetailPage));
                }
            }
            catch
            {
            }
        }
    }
}
