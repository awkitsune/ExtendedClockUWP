﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace ExtendedClock.View
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class ClockPage : Page
    {
        BrightnessOverride brigtnessControl;
        ApplicationView appView;

        public ClockPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            brigtnessControl = BrightnessOverride.GetForCurrentView();
            appView = ApplicationView.GetForCurrentView();
            if (brigtnessControl.IsSupported)
            {
                brigtnessControl.SetBrightnessLevel(0.0, DisplayBrightnessOverrideOptions.None);
                brigtnessControl.StartOverride();
            }

            appView.TryEnterFullScreenMode();
        }
    }
}