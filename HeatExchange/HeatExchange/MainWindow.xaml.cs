using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HeatExchange
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        //Global variables
        public SystemInputStream GlobalInputs;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            GlobalInputs = new SystemInputStream();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Populate system type
            foreach (string s in SciHelper.SystemTypes)
            {
                SystemTypeComboBox.Items.Add(s);
            }

            //Populate Input parameters
            GlobalInputs.NumericalReadings = new List<Reading>();
            foreach (KeyValuePair<string, double> systemInputPair in SciHelper.NumericalInputs)
            {
                GlobalInputs.NumericalReadings.Add(new Reading() { Parameter = systemInputPair.Key, ValueString = "0" });
            }
            InputListView.ItemsSource = GlobalInputs.NumericalReadings;
        }

        private void SystemType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GlobalInputs.SysType = (SciHelper.SystemType)SystemTypeComboBox.SelectedIndex;
            ConfigImage.Source = new BitmapImage(new Uri("Assets/" + GlobalInputs.SysType + ".png", UriKind.Relative));
        }
    }
}
