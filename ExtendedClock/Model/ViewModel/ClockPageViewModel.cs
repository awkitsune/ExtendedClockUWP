using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using Windows.UI.ViewManagement;
using ExtendedClock.View;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace ExtendedClock.Model.ViewModel
{
    class ClockPageViewModel : BaseViewModel
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        private Random rnd = new Random();
        ApplicationView appView = ApplicationView.GetForCurrentView();

        private string hostIpAddress = "";
        private IPEndPoint hostIpPoint;

        private double cpuLoad  = 0.0;
        private double gpuLoad  = 0.0;
        private double ramLoad  = 0.0;

        private DateTime currentTime = DateTime.Now;

        private Thickness savingMargin = new Thickness(16, 16, 16, 16);
        private Visibility hostLoadVisibility = Visibility.Collapsed;

        public ICommand OpenSettings { get; private set; }

        public ClockPageViewModel()
        {
            OpenSettings = new DelegateCommand(Settings);

            try
            {
                hostIpAddress = localSettings.Values[Constant.KEY_HOST_ADDRESS] as string;
                hostIpPoint = new IPEndPoint(IPAddress.Parse(hostIpAddress), Constant.HOST_PORT);
            }
            catch (Exception)
            {
                
            }

            var timeUpdateTimer = new DispatcherTimer();
            timeUpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            timeUpdateTimer.Tick += UpdateTime;
            timeUpdateTimer.Start();

            var screenSaverTimer = new DispatcherTimer();
            screenSaverTimer.Interval = TimeSpan.FromMinutes(2);
            screenSaverTimer.Tick += UpdateMargin;
            screenSaverTimer.Start();

            var dataUpdateTimer = new DispatcherTimer();
            dataUpdateTimer.Interval = TimeSpan.FromMilliseconds(1000);
            dataUpdateTimer.Tick += GetDataFromHost;
            dataUpdateTimer.Start();
        }

        #region Commands
        private void Settings()
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(SetupPage));
        }
        #endregion

        #region Methods
        void UpdateTime(object sender, object e)
        {
            CurrentTime = DateTime.Now;
            //add data loading
        }
        void UpdateMargin(object sender, object e)
        {
            SavingMargin = new Thickness(rnd.Next(8, 16), 16, 16, rnd.Next(8, 16));
        }
        void GetDataFromHost(object sender, object e)
        {
            var sb = new StringBuilder();

            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    socket.Connect(hostIpPoint);
                    socket.Send(Encoding.Unicode.GetBytes("data"));

                    var data = new byte[256];
                    int bytes = 0;

                    do
                    {
                        bytes = socket.Receive(data, data.Length, 0);
                        sb.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    } while (socket.Available > 0);

                    socket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }

            var responce = sb.ToString();
            var dataGot = responce.Split(';');
            try
            {
                CpuLoad = dataGot[0];
                GpuLoad = dataGot[1];
                RamLoad = dataGot[2];
                HostLoadVisibility = Visibility.Visible;
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region Properties
        public Visibility HostLoadVisibility
        {
            get => hostLoadVisibility;
            private set
            {
                hostLoadVisibility = value;
                OnPropertyChanged(nameof(HostLoadVisibility));
                OnPropertyChanged(nameof(ConnectionVisibility));
            }
        }
        public Visibility ConnectionVisibility
        {
            get
            {
                switch (hostLoadVisibility)
                {
                    case Visibility.Visible:
                        return Visibility.Collapsed;
                    default:
                        return Visibility.Visible;
                }
            }
        }
        public Thickness SavingMargin
        {
            get => savingMargin;
            private set
            {
                savingMargin = value;
                OnPropertyChanged(nameof(SavingMargin));
            }
        }
        public string Time
        {
            get => currentTime.ToString("HH:mm:ss");
        }
        public string Date
        {
            get => currentTime.ToString("MM.dd.yyyy");
        }
        public DateTime CurrentTime
        {
            get => currentTime;
            private set
            {
                currentTime = value;
                OnPropertyChanged(nameof(Time));
                OnPropertyChanged(nameof(Date));
            }
        }

        public string CpuLoad
        {
            get => $"{cpuLoad} %";
            private set
            {
                cpuLoad = Double.Parse(value);
                OnPropertyChanged(nameof(CpuLoad));
            }
        }
        public string GpuLoad
        {
            get => $"{gpuLoad} %";
            private set
            {
                gpuLoad = Double.Parse(value);
                OnPropertyChanged(nameof(GpuLoad));
            }
        }
        public string RamLoad
        {
            get => $"{ramLoad} MiB";
            private set
            {
                ramLoad = Double.Parse(value);
                OnPropertyChanged(nameof(RamLoad));
            }
        }
        #endregion
    }
}
