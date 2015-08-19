using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
// ReSharper disable InconsistentNaming
// ReSharper disable JoinDeclarationAndInitializer

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
            { "Nr", 0 },
            { "FinPitch", 0 },

            { "QWaterInitial", 0 },
            { "TWaterInitial", 0 },
            { "PWaterInitial", 0 },

            { "QAirInitial", 0 },
            { "TAirInitial", 0 },
            { "PAirInitial", 0 }
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
                _input.TubeOutsideStaggered.SurfaceAreaDensity = _input.TubeOutsideStaggered.TotalHeatArea /
                                                                 _input.TubeOutsideStaggered.HeatExchTotalVolume;
            }
        }

        public static void ComputeAdditionalProperty(ref SystemInputStream _in)
        {
            const double Epsilon = 0.75;

            //All properties are final state
            double DensityWaterFinal;
            double DensityAirFinal;

            double MassFlowWaterFinal;
            double MassFlowAirFinal;

            double TWaterFinal;
            double TAirFinal;
            double TWaterMeanFinal;
            double TAirMeanFinal;

            double L1 = _in.NumericalReadings.First(x => x.Parameter == "L1").Value;
            double L2 = _in.NumericalReadings.First(x => x.Parameter == "L2").Value;
            double L3 = _in.NumericalReadings.First(x => x.Parameter == "L3").Value;
            double Xt = _in.NumericalReadings.First(x => x.Parameter == "Xt").Value;
            double Xl = _in.NumericalReadings.First(x => x.Parameter == "Xl").Value;
            double Do = _in.NumericalReadings.First(x => x.Parameter == "Do").Value;
            double Di = _in.NumericalReadings.First(x => x.Parameter == "Di").Value;
            double DelH = _in.NumericalReadings.First(x => x.Parameter == "DelH").Value;

            double QWaterInitial = _in.NumericalReadings.First(x => x.Parameter == "QWaterInitial").Value;
            double TWaterInitial = _in.NumericalReadings.First(x => x.Parameter == "TWaterInitial").Value;
            double PWaterInitial = _in.NumericalReadings.First(x => x.Parameter == "PWaterInitial").Value;

            double QAirInitial = _in.NumericalReadings.First(x => x.Parameter == "QAirInitial").Value;
            double TAirInitial = _in.NumericalReadings.First(x => x.Parameter == "TAirInitial").Value;
            double PAirInitial = _in.NumericalReadings.First(x => x.Parameter == "PAirInitial").Value;
            double Nr = _in.NumericalReadings.First(x => x.Parameter == "Nr").Value;
            double FinPitch = _in.NumericalReadings.First(x => x.Parameter == "FinPitch").Value;

            DensityWaterFinal = PWaterInitial / (287.04 * (273.15 + TWaterInitial));
            DensityAirFinal = PAirInitial / (287.04 * (273.15 + TAirInitial));

            MassFlowWaterFinal = QWaterInitial * DensityWaterFinal;
            MassFlowAirFinal = QAirInitial * DensityAirFinal;

            TWaterFinal = TWaterInitial - Epsilon * (TWaterInitial - TAirInitial);
            TAirFinal = TAirInitial + Epsilon * (MassFlowWaterFinal / MassFlowAirFinal) * (TWaterInitial - TAirInitial);
            TWaterMeanFinal = (TWaterFinal + TWaterInitial) / 2;
            TAirMeanFinal = (TAirFinal + TAirInitial) / 2;


            //Calculate additional properties
            double ViscosityAir = CoolProp.PropsSI("V", "T", TAirMeanFinal, "P", 101325, "Air");
            double ViscosityWater = CoolProp.PropsSI("V", "T", TWaterMeanFinal, "P", 101325, "Water");
            double CpAir = CoolProp.PropsSI("CPMASS", "T", TWaterMeanFinal, "P", 101325, "Water");
            double CpWater = CoolProp.PropsSI("CPMASS", "T", TWaterMeanFinal, "P", 101325, "Water");
            double PrandtlAir = CoolProp.PropsSI("CPMASS", "T", TWaterMeanFinal, "P", 101325, "Water");
            double PrandtlWater = CoolProp.PropsSI("CPMASS", "T", TWaterMeanFinal, "P", 101325, "Water");

            double G_Air = MassFlowAirFinal /
                              ((_in.SysType == SystemType.Inline)
                                  ? _in.TubeOutsideInlineData.TotalMinFfArea
                                  : _in.TubeOutsideStaggered.TotalMinFfArea);

            double G_Water = MassFlowWaterFinal /
                                          ((_in.SysType == SystemType.Inline)
                                              ? _in.TubeOutsideInlineData.TotalMinFfArea
                                              : _in.TubeOutsideStaggered.TotalMinFfArea);

            double Reynolds_Air = G_Air * ((_in.SysType == SystemType.Inline)
                ? _in.TubeOutsideInlineData.HydDiameter
                : _in.TubeOutsideStaggered.HydDiameter) / ViscosityAir;

            double Reynolds_Water = G_Water * ((_in.SysType == SystemType.Inline)
                ? _in.TubeOutsideInlineData.HydDiameter
                : _in.TubeOutsideStaggered.HydDiameter) / ViscosityWater;

            double J_Water;
            double F_Water;
            double J_Air;
            double F_Air;

            #region Calculation of J
            if (Nr <= 1)
            {
                if (_in.SysType == SystemType.Inline)
                {

                    double c1 = 1.9 - 0.23 * Math.Log(Reynolds_Water);
                    double c2 = -0.236 + 0.126 * Math.Log(Reynolds_Water);
                    J_Water = 0.108 * Math.Pow(Reynolds_Water, -0.29) * Math.Pow(Xt / Xl, c1) * Math.Pow(FinPitch / Di, 1.084) *
                              Math.Pow(FinPitch / _in.TubeOutsideInlineData.HydDiameter, -0.786) * Math.Pow(FinPitch / Xt, c2);


                    c1 = 1.9 - 0.23 * Math.Log(Reynolds_Air);
                    c2 = -0.236 + 0.126 * Math.Log(Reynolds_Air);
                    J_Air = 0.108 * Math.Pow(Reynolds_Air, -0.29) * Math.Pow(Xt / Xl, c1) * Math.Pow(FinPitch / Di, 1.084) *
                              Math.Pow(FinPitch / _in.TubeOutsideInlineData.HydDiameter, -0.786) * Math.Pow(FinPitch / Xt, c2);
                }
                else
                {
                    double c1 = 1.9 - 0.23 * Math.Log(Reynolds_Water);
                    double c2 = -0.236 + 0.126 * Math.Log(Reynolds_Water);
                    J_Water = 0.108 * Math.Pow(Reynolds_Water, -0.29) * Math.Pow(Xt / Xl, c1) * Math.Pow(FinPitch / Di, 1.084) *
                              Math.Pow(FinPitch / _in.TubeOutsideStaggered.HydDiameter, -0.786) * Math.Pow(FinPitch / Xt, c2);


                    c1 = 1.9 - 0.23 * Math.Log(Reynolds_Air);
                    c2 = -0.236 + 0.126 * Math.Log(Reynolds_Air);
                    J_Air = 0.108 * Math.Pow(Reynolds_Air, -0.29) * Math.Pow(Xt / Xl, c1) * Math.Pow(FinPitch / Di, 1.084) *
                              Math.Pow(FinPitch / _in.TubeOutsideStaggered.HydDiameter, -0.786) * Math.Pow(FinPitch / Xt, c2);
                }
            }
            else
            {

                if (_in.SysType == SystemType.Inline)
                {
                    //Water
                    double c3 = -0.361 - 0.042 * Nr / Math.Log(Reynolds_Water) + 0.158 * Math.Log(Nr * Math.Pow(FinPitch / Di, 0.41));
                    double c4 = -1.224 - 0.076 * Math.Pow(Xl / _in.TubeOutsideInlineData.HydDiameter, 1.42) / Math.Log(Reynolds_Water);
                    double c5 = -0.083 + 0.058 * Nr / Math.Log(Reynolds_Water);
                    double c6 = -5.735 + 1.21 * Math.Log(Reynolds_Water / Nr);
                    J_Water = 0.086 * Math.Pow(Reynolds_Water, c3) * Math.Pow(Nr, c4) * Math.Pow(FinPitch / Di, c5) *
                              Math.Pow(FinPitch / _in.TubeOutsideInlineData.HydDiameter, c6) * Math.Pow(FinPitch / Xt, -0.93);
                    //Air
                    c3 = -0.361 - 0.042 * Nr / Math.Log(Reynolds_Air) + 0.158 * Math.Log(Nr * Math.Pow(FinPitch / Di, 0.41));
                    c4 = -1.224 - 0.076 * Math.Pow(Xl / _in.TubeOutsideInlineData.HydDiameter, 1.42) / Math.Log(Reynolds_Air);
                    c5 = -0.083 + 0.058 * Nr / Math.Log(Reynolds_Air);
                    c6 = -5.735 + 1.21 * Math.Log(Reynolds_Air / Nr);
                    J_Air = 0.086 * Math.Pow(Reynolds_Air, c3) * Math.Pow(Nr, c4) * Math.Pow(FinPitch / Di, c5) *
                              Math.Pow(FinPitch / _in.TubeOutsideInlineData.HydDiameter, c6) * Math.Pow(FinPitch / Xt, -0.93);
                }
                else
                {
                    //Water
                    double c3 = -0.361 - 0.042 * Nr / Math.Log(Reynolds_Water) + 0.158 * Math.Log(Nr * Math.Pow(FinPitch / Di, 0.41));
                    double c4 = -1.224 - 0.076 * Math.Pow(Xl / _in.TubeOutsideStaggered.HydDiameter, 1.42) / Math.Log(Reynolds_Water);
                    double c5 = -0.083 + 0.058 * Nr / Math.Log(Reynolds_Water);
                    double c6 = -5.735 + 1.21 * Math.Log(Reynolds_Water / Nr);
                    J_Water = 0.086 * Math.Pow(Reynolds_Water, c3) * Math.Pow(Nr, c4) * Math.Pow(FinPitch / Di, c5) *
                              Math.Pow(FinPitch / _in.TubeOutsideStaggered.HydDiameter, c6) * Math.Pow(FinPitch / Xt, -0.93);
                    //Air
                    c3 = -0.361 - 0.042 * Nr / Math.Log(Reynolds_Air) + 0.158 * Math.Log(Nr * Math.Pow(FinPitch / Di, 0.41));
                    c4 = -1.224 - 0.076 * Math.Pow(Xl / _in.TubeOutsideStaggered.HydDiameter, 1.42) / Math.Log(Reynolds_Air);
                    c5 = -0.083 + 0.058 * Nr / Math.Log(Reynolds_Air);
                    c6 = -5.735 + 1.21 * Math.Log(Reynolds_Air / Nr);
                    J_Air = 0.086 * Math.Pow(Reynolds_Air, c3) * Math.Pow(Nr, c4) * Math.Pow(FinPitch / Di, c5) *
                              Math.Pow(FinPitch / _in.TubeOutsideStaggered.HydDiameter, c6) * Math.Pow(FinPitch / Xt, -0.93);
                }
            }
            #endregion  

            //double H_Water = J_Water * G_Water * CpWater / Math.Pow(PrandtlWater, (double)2 / 3);
            //double H_Air = J_Air * G_Air * CpAir / Math.Pow(PrandtlAir, (double)2 / 3);

            //double AbsCpWater;
            //double AbsCpAir;

            using (StreamWriter streamWriter = new StreamWriter("Output.txt"))
            {
                streamWriter.WriteLine("JWater," + J_Water);
                streamWriter.WriteLine("JAir," + J_Air);
                streamWriter.WriteLine("ReyWater," + Reynolds_Water);
                streamWriter.WriteLine("ReyAir," + Reynolds_Air);
                streamWriter.WriteLine("GWater," + G_Water);
                streamWriter.WriteLine("GAir," + G_Air);
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
