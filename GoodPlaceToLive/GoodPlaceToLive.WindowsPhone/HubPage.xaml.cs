﻿using Windows.UI.Popups;
using GoodPlaceToLive.Common;
using GoodPlaceToLive.Controls;
using GoodPlaceToLive.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону проекта "Универсальное приложение с Hub" см. по адресу http://go.microsoft.com/fwlink/?LinkID=391955
using GoodPlaceToLive.Models;
using GoodPlaceToLive.Pages;
using GoodPlaceToLive.ViewModel;
using Microsoft.Practices.ServiceLocation;

namespace GoodPlaceToLive
{
    /// <summary>
    /// Страница, на которой отображается сгруппированная коллекция элементов.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        public HubPage()
        {
            this.InitializeComponent();

            // Элемент управления Hub ("Главный раздел") поддерживается только в книжной ориентации
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Получает объект <see cref="NavigationHelper"/>, связанный с данным объектом <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Получает модель представлений для данного объекта <see cref="Page"/>.
        /// Эту настройку можно изменить на модель строго типизированных представлений.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
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
            var sampleDataGroups = await SampleDataSource.GetGroupsAsync();
            this.DefaultViewModel["Groups"] = sampleDataGroups;
        }

        /// <summary>
        /// Сохраняет состояние, связанное с данной страницей, в случае приостановки приложения или
        /// удаления страницы из кэша навигации.  Значения должны соответствовать требованиям сериализации
        /// <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">Источник события; как правило, <see cref="NavigationHelper"/></param>
        /// <param name="e">Данные события, которые предоставляют пустой словарь для заполнения
        /// сериализуемым состоянием.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: Сохраните здесь уникальное состояние страницы.
        }

        /// <summary>
        /// Показывает сведения о группе, которую щелкнул пользователь, в объекте <see cref="SectionPage"/>.
        /// </summary>
        /// <param name="sender">Источник события щелчка.</param>
        /// <param name="e">Сведения о событии щелчка.</param>
        private void GroupSection_ItemClick(object sender, ItemClickEventArgs e)
        {
            var groupId = ((SampleDataGroup)e.ClickedItem).UniqueId;
            if (!Frame.Navigate(typeof(SectionPage), groupId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        /// <summary>
        /// Показывает сведения об элементе, который щелкнул пользователь, в объекте <see cref="ItemPage"/>
        /// </summary>
        /// <param name="sender">Источник события щелчка.</param>
        /// <param name="e">Значения по умолчанию для события щелчка.</param>
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = ((HospitalAdultItem)e.ClickedItem);
            var rmain = ServiceLocator.Current.GetInstance<MainViewModel>();
            rmain.CurrentItem = item;
            Frame.Navigate(typeof(HospitalDetailPage));
        }

        #region Регистрация NavigationHelper

        /// <summary>
        /// Методы, предоставленные в этом разделе, используются исключительно для того, чтобы
        /// NavigationHelper для отклика на методы навигации страницы.
        /// <para>
        /// Логика страницы должна быть размещена в обработчиках событий для
        /// <see cref="NavigationHelper.LoadState"/>
        /// и <see cref="NavigationHelper.SaveState"/>.
        /// Параметр навигации доступен в методе LoadState
        /// в дополнение к состоянию страницы, сохраненному в ходе предыдущего сеанса.
        /// </para>
        /// </summary>
        /// <param name="e">Данные события, описывающие, каким образом была достигнута эта страница.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void ChildList_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //throw new NotImplementedException();
            var item = ((ChildPlaceItem)e.ClickedItem);
            var rmain = ServiceLocator.Current.GetInstance<MainViewModel>();
            rmain.CurrentChildItem = item;
            Frame.Navigate(typeof (ChildPlacePage));
        }

        private MainViewModel _rmain;

        private async void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _rmain = ServiceLocator.Current.GetInstance<MainViewModel>();

                StatusBar statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                statusBar.ShowAsync();
                statusBar.ProgressIndicator.ShowAsync();

                _rmain.onLoaded += _rmain_onLoaded;
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _rmain_onLoaded(object sender, EventArgs e)
        {
            StatusBar statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            if (_rmain.Loading == false)
            {
                statusBar.HideAsync();
                statusBar.ProgressIndicator.HideAsync();
            }
            else
            {
                statusBar.ShowAsync();
                statusBar.ProgressIndicator.ShowAsync();

            }
        }

        private void HospitalsTile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                Frame.Navigate(typeof(HospitalList));
            }
            catch { }
        }

        private void Border_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                Frame.Navigate(typeof(ChildPlaceList));
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutTile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                Frame.Navigate(typeof(AboutPage));
            }
            catch { }
        }

        private void MapTile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                Frame.Navigate(typeof(MapPage));
            }
            catch { }
        }
    }
}