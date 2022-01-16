using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class SetupPage : Page
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public SetupPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                addressTextBox.Text = localSettings.Values[Constant.KEY_HOST_ADDRESS] as string;
            }
            catch (Exception)
            {
                
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var check = new Regex(Constant.REGEX_IP_PATTERN);
            if (string.IsNullOrEmpty(addressTextBox.Text))
            {
                errorTextBlock.Text = "Please fill host address";
            }
            else
            {
                if (check.IsMatch(addressTextBox.Text, 0))
                {
                    errorTextBlock.Text = "";
                    localSettings.Values[Constant.KEY_HOST_ADDRESS] = addressTextBox.Text;
                    localSettings.Values[Constant.KEY_FIRST_LAUNCH] = "false";
                    Frame rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(ClockPage));
                }
                else
                {
                    errorTextBlock.Text = "Please fill host address";
                }
            }
        }
    }
}
