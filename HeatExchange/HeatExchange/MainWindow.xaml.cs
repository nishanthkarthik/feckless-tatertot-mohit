﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

            //Populate Materials
            foreach (KeyValuePair<string, double> keyValuePair in SciHelper.Materials)
            {
                MaterialComboBox.Items.Add(keyValuePair.Key);
            }

            //Populate Fluid flow
            foreach (string s in SciHelper.FluidSystemTypes)
            {
                FluidTypeComboBox.Items.Add(s);
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

        private void MaterialComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GlobalInputs.MaterialCoeff = SciHelper.Materials.ElementAt(MaterialComboBox.SelectedIndex).Value;
        }

        private void FluidTypeComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GlobalInputs.FluidSysType = (SciHelper.FluidSystemType)FluidTypeComboBox.SelectedIndex;
        }

        private void ComputeSecondary_OnClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
