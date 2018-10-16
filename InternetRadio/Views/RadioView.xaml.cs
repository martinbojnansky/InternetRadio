using InternetRadio.ViewModels;
using System;
using System.Linq;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace InternetRadio.Views
{
    public sealed partial class RadioView : UserControl
    {
        public static RadioView Current;
        public RadioViewModel RadioViewModel = RadioViewModel.Instance;

        public RadioView()
        {
            this.InitializeComponent();
            Current = this;
        }

        /// <summary>
        /// Changes pivot index.
        /// </summary>
        /// <param name="index"></param>
        public void SetRadiosPivotSelectedItem(int index)
        {
            try { RadiosPivot.SelectedIndex = index; } catch { }
        }
    }
}
