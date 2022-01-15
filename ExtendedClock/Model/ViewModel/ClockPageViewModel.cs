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

namespace ExtendedClock.Model.ViewModel
{
    class ClockPageViewModel : BaseViewModel
    {
        ApplicationView appView = ApplicationView.GetForCurrentView();

        private double cpuLoad  = 0.0;
        private double gpuLoad  = 0.0;
        private double vramLoad = 0.0;
        private double ramLoad  = 0.0;

        private DateTime currentTime = DateTime.Now;

        public ICommand ChangeFullScreen { get; private set; }

        public ClockPageViewModel()
        {
            ChangeFullScreen = new DelegateCommand(FullScreenSwitch);

            var modelUpdateTimer = new DispatcherTimer();
            modelUpdateTimer.Interval = TimeSpan.FromMilliseconds(200);
            modelUpdateTimer.Tick += UpdateInfo;
            modelUpdateTimer.Start();
        }

        #region Commands
        private void FullScreenSwitch()
        {
            if (appView.IsFullScreenMode)
            {
                appView.ExitFullScreenMode();
            }
            else
            {
                appView.TryEnterFullScreenMode();
            }

        }
        #endregion

        #region Methods
        void UpdateInfo(object sender, object e)
        {
            CurrentTime = DateTime.Now;
            //add data loading
        }
        #endregion

        #region Properties
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
        public string VramLoad
        {
            get => $"{vramLoad} %";
            private set
            {
                vramLoad = Double.Parse(value);
                OnPropertyChanged(nameof(VramLoad));
            }
        }
        public string RamLoad
        {
            get => $"{ramLoad} %";
            private set
            {
                ramLoad = Double.Parse(value);
                OnPropertyChanged(nameof(RamLoad));
            }
        }
        #endregion
    }
}
