using System;
using System.Collections.Generic;
using System.Windows;

namespace HeatExchange
{
    public class SciHelper
    {
        //System Type
        public static readonly string[] SystemTypes = { "Inline", "Staggered" };
        public enum SystemType
        {
            Inline,
            Staggered
        };

        //Fluid System type
        public static readonly string[] FluidSystemTypes = { "Both Air", "Air Outside, Water Inside" };
        public enum FluidSystemType
        {
            BothAir,
            AirOutWaterIn
        };

        //Material
        #region Material Dictionary
        public static readonly Dictionary<string, double> Materials = new Dictionary<string, double>()
        {
            {"Aluminium",202},
            {"Aluminium Brass",100},
            {"Brass",99},
            {"Carbon Steel",45},
            {"Carbon Moly",43},
            {"Chrome-MolySteel (2 Cr- Mo)",42},
            {"2.25 Cr - 0.5 Mo",38},
            {"5 Cr - 0.5 Mo",35},
            {"12 Cr - 1 Mo",28},
            {"Copper",386},
            {"Cupro Nickel (90 Cu - 10 Ni)",71},
            {"Cupro Nickel (70 Cu - 30 Ni)",29},
            {"Inconel",19},
            {"Lead",35},
            {"Monel (67 Ni - 30 Cu - 1.4 Fe)",26},
            {"Nickel",62},
            {"Red Brass (85 Cu - 15 Zn)",159},
            {"Stainless Steel",16},
            {"Titanium",19}
        };
        #endregion

        //Numerical Inputs
        #region Numerical Inputs
        public static Dictionary<string, double> NumericalInputs = new Dictionary<string, double>()
        {
            { "L1", 0 },
            { "L2", 0 },
            { "L3", 0 },
            { "Xt", 0 },
            { "Xl", 0 },
            { "Do", 0 },
            { "Di", 0 },
            { "DelH", 0 },
            { "Qout", 0 },
            { "Qin", 0 },
            { "Tout", 0 },
            { "Tin", 0 },
            { "Pout", 0 },
            { "Pin", 0 },
        };
        #endregion

    }

    public class Reading
    {
        public string Parameter { get; set; }
        public string ValueString { get; set; }
        public double Value
        {
            get
            {
                double parsedValue;
                if (double.TryParse(ValueString, out parsedValue))
                {
                    return parsedValue;
                }
                Application.Current.Dispatcher.BeginInvoke(new Action<Reading>((sender) =>
                {
                    MessageBox.Show("Invalid input => " + ValueString);
                }), this);
                return 0;
            }
        }
    }

    public class TubeInside
    {
        public double Nt { get; set; }
        public double TotalHeatArea { get; set; }
        public double TotalMinFfArea { get; set; }
        public double CoreFrontalArea { get; set; }
        public double RatioFfFrontalArea { get; set; }
        public double HydDiameter { get; set; }
        public double TubeLforHeatTransfer { get; set; }
        public double TubeLforPressureDrop { get; set; }
        public double SurfaceAreaDensity { get; set; }
    }

    public class TubeOutsideInline
    {
        public double NtPrime { get; set; }
        public double TotalHeatArea { get; set; }
        public double TotalMinFfArea { get; set; }
        public double FrontalArea { get; set; }
        public double RatioFfFrontalArea { get; set; }
        public double HydDiameter { get; set; }
        public double HeatExchTotalVolume { get; set; }
        public double TubeLforPressureDrop { get; set; }
        public double SurfaceAreaDensity { get; set; }
    }

    public class TubeOutsideStaggered
    {
        public double NtStaggered { get; set; }
        public double TotalHeatArea { get; set; }
        public double TotalMinFfArea { get; set; }
        public double FrontalArea { get; set; }
        public double RatioFfFrontalArea { get; set; }
        public double HydDiameter { get; set; }
        public double HeatExchTotalVolume { get; set; }
        public double TubeLforPressureDrop { get; set; }
        public double SurfaceAreaDensity { get; set; }
    }

    public class SystemInputStream
    {
        public List<Reading> NumericalReadings { get; set; }
        public SciHelper.SystemType SysType { get; set; }
        public SciHelper.FluidSystemType FluidSysType { get; set; }
        public double MaterialCoeff { get; set; }

    }

}
