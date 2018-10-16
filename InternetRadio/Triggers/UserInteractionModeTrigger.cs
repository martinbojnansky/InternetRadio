using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace InternetRadio.Triggers
{
    public class UserInteractionModeTrigger : StateTriggerBase
    {
        private UserInteractionMode _userInteractionMode;
        public UserInteractionMode UserInteractionMode
        {
            get { return _userInteractionMode; }
            set { _userInteractionMode = value; WindowSizeChanged(); }
        }

        public UserInteractionModeTrigger()
        {
            Window.Current.SizeChanged += new WindowSizeChangedEventHandler((object sender, WindowSizeChangedEventArgs e) =>
            {
                WindowSizeChanged();
            });
        }

        private void WindowSizeChanged()
        {
            SetActive(UIViewSettings.GetForCurrentView().UserInteractionMode.Equals(UserInteractionMode));
        }
    }
}
