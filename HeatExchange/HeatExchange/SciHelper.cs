using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
// ReSharper disable InconsistentNaming

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

        //Sceondary Input parameter computation
        public static void ComputeSecondaryInputs(ref SystemInputStream _input)
        {
            double L1 = _input.NumericalReadings.First(x => x.Parameter == "L1").Value;
            double L2 = _input.NumericalReadings.First(x => x.Parameter == "L2").Value;
            double L3 = _input.NumericalReadings.First(x => x.Parameter == "L3").Value;
            double Xt = _input.NumericalReadings.First(x => x.Parameter == "Xt").Value;
            double Xl = _input.NumericalReadings.First(x => x.Parameter == "Xl").Value;
            double Do = _input.NumericalReadings.First(x => x.Parameter == "Do").Value;
            double Di = _input.NumericalReadings.First(x => x.Parameter == "Di").Value;
            double DelH = _input.NumericalReadings.First(x => x.Parameter == "DelH").Value;
            double Qout = _input.NumericalReadings.First(x => x.Parameter == "Qout").Value;
            double Qin = _input.NumericalReadings.First(x => x.Parameter == "Qin").Value;
            double Tout = _input.NumericalReadings.First(x => x.Parameter == "Tout").Value;
            double Tin = _input.NumericalReadings.First(x => x.Parameter == "Tin").Value;
            double Pout = _input.NumericalReadings.First(x => x.Parameter == "Pout").Value;
            double Pin = _input.NumericalReadings.First(x => x.Parameter == "Pin").Value;

            if (_input.SysType == SystemType.Inline)
            {
                //Tube inside computation
                _input.Nt = L2 * L3 / (Xt * Xl);
                _input.TubeInsideData.TotalHeatArea = Math.PI * Di * L1 * _input.Nt;
                _input.TubeInsideData.TotalMinFfArea = Math.PI / 4 * Di * Di * _input.Nt;
                _input.TubeInsideData.CoreFrontalArea = L2 * L3;
                _input.TubeInsideData.RatioFfFrontalArea = (Math.PI * Di * Di) / (4 * Xt * Xl);
                _input.TubeInsideData.HydDiameter = Di;
                _input.TubeInsideData.TubeLforHeatTransfer = L1;
                _input.TubeInsideData.TubeLforPressureDrop = L1 + (2 * DelH);
                _input.TubeInsideData.SurfaceAreaDensity = (Math.PI * Di * L1 * _input.Nt) / (L1 * L2 * L3);

                //Tube outside computation
                _input.TubeOutsideInlineData.TotalHeatArea = (Math.PI * Do * L1 * _input.Nt) +
                                                           2 * ((L2 * L3) - (Math.PI * Di * Di * _input.Nt / 4));
                _input.TubeOutsideInlineData.NtDash = L3 / Xt;
                _input.TubeOutsideInlineData.TotalMinFfArea = (Xt - Do) * _input.TubeOutsideInlineData.NtDash * L1;
                _input.TubeOutsideInlineData.FrontalArea = L1 * L3;
                _input.TubeOutsideInlineData.RatioFfFrontalArea = (Xt - Do) / Xt;
                _input.TubeOutsideInlineData.HydDiameter = 4 * _input.TubeOutsideInlineData.TotalMinFfArea * L2 /
                                                           _input.TubeOutsideInlineData.TotalHeatArea;
                _input.TubeOutsideInlineData.TubeLforPressureDrop = L2;
                _input.TubeOutsideInlineData.HeatExchTotalVolume = L1 * L2 * L3;
                _input.TubeOutsideInlineData.SurfaceAreaDensity = _input.TubeOutsideInlineData.TotalHeatArea / _input.TubeOutsideInlineData.HeatExchTotalVolume;
            }

            else if (_input.SysType == SystemType.Staggered)
            {
                //Tube inside computation
                _input.Nt = (L3 / Xt) * ((L2 / Xl + 1) / 2) + ((L3 / Xt) - 1) * ((L2 / Xl - 1) / 2);
                _input.TubeInsideData.TotalHeatArea = Math.PI * Di * L1 * _input.Nt;
                _input.TubeInsideData.TotalMinFfArea = Math.PI / 4 * Di * Di * _input.Nt;
                _input.TubeInsideData.CoreFrontalArea = L2 * L3;
                _input.TubeInsideData.RatioFfFrontalArea = (Math.PI * Di * Di) / (4 * Xt * Xl);
                _input.TubeInsideData.HydDiameter = Di;
                _input.TubeInsideData.TubeLforHeatTransfer = L1;
                _input.TubeInsideData.TubeLforPressureDrop = L1 + (2 * DelH);
                _input.TubeInsideData.SurfaceAreaDensity = (Math.PI * Di * L1 * _input.Nt) / (L1 * L2 * L3);

                //Tube outside computation
                _input.TubeOutsideStaggered.TotalHeatArea = (Math.PI * Do * L1 * _input.Nt) +
                                                            2 * ((L2 * L3) - (Math.PI * Do * Do * _input.Nt / 4));
                _input.TubeOutsideStaggered.A = (Xt - Do) / 2;
                _input.TubeOutsideStaggered.B = Math.Sqrt(Math.Pow(Xt / 2, 2) + Math.Pow(Xl, 2)) - Do;
                _input.TubeOutsideStaggered.C = (_input.TubeOutsideStaggered.A < _input.TubeOutsideStaggered.B)
                    ? (2 * _input.TubeOutsideStaggered.A)
                    : (2 * _input.TubeOutsideStaggered.B);
                _input.TubeOutsideStaggered.TotalMinFfArea = L1 *
                                                             (((L3 / Xt) - 1) * _input.TubeOutsideStaggered.C + (Xt - Do));
                _input.TubeOutsideStaggered.FrontalArea = L1 * L3;
                _input.TubeOutsideStaggered.RatioFfFrontalArea = _input.TubeOutsideStaggered.TotalMinFfArea /
                                                                 _input.TubeOutsideStaggered.FrontalArea;
                _input.TubeOutsideStaggered.HydDiameter = 4 * _input.TubeOutsideStaggered.TotalMinFfArea * L2 /
                                                          _input.TubeOutsideStaggered.TotalHeatArea;
                _input.TubeOutsideStaggered.TubeLforPressureDrop = L2;
                _input.TubeOutsideStaggered.HeatExchTotalVolume = L1 * L2 * L3;
                _input.TubeOutsideStaggered.SurfaceAreaDensity = _input.TubeOutsideStaggered.TotalHeatArea/
                                                                 _input.TubeOutsideStaggered.HeatExchTotalVolume;
            }
        }

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
                Application.Current.Dispatcher.BeginInvoke(new Action<Reading>(sender =>
                {
                    MessageBox.Show("Invalid input => " + ValueString);
                }), this);
                return 0;
            }
        }
    }

    public class TubeInside
    {
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
        public double NtDash { get; set; }
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
        public double TotalHeatArea { get; set; }
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
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
        public double MaterialCoeff { get; set; }
        public double Nt { get; set; }
        public TubeInside TubeInsideData { get; set; }
        public TubeOutsideInline TubeOutsideInlineData { get; set; }
        public TubeOutsideStaggered TubeOutsideStaggered { get; set; }

        public SystemInputStream()
        {
            NumericalReadings = new List<Reading>();
            TubeInsideData = new TubeInside();
            TubeOutsideInlineData = new TubeOutsideInline();
            TubeOutsideStaggered = new TubeOutsideStaggered();
        }
    }

}
