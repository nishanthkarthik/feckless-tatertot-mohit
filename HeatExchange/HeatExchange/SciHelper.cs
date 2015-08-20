using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using HeatExchange.CoolPropHelper;

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
            //{ "Nr", 0 },
            { "FinPitch", 0 },
            { "FinThickness", 0 },

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

        public static void ComputeAdditionalProperty(ref SystemInputStream _in, ref double AirX, ref double WaterX)
        {
            #region Variable declarations
            const double Epsilon = 0.75;

            //All properties are final state
            double DensityWaterInitial;
            double DensityAirInitial;

            double MassFlowWaterFinal;
            double MassFlowAirFinal;

            double TWaterFinal;
            double TAirFinal;
            double TWaterMeanFinal;
            double TAirMeanFinal;
            double ViscosityAir;
            double ViscosityWater;
            double CpAir;
            double CpWater;
            double PrandtlAir;
            double PrandtlWater;
            double G_Air;
            double G_Water;
            double Reynolds_Air;
            double Reynolds_Water;
            double J_Water;
            double F_Water;
            double J_Air;
            double F_Air;
            double H_Water;
            double H_Air;
            double c7;
            double c8;
            double c9;
            double VelocityAir;
            double FinEfficiency;
            double OverallEfficiency;
            double Rw;
            double UaInverse;
            double C_Water;
            double C_Air;
            double C_Min;
            double C_Star;
            double NTU;
            double epsilon;
            double ConductionAreaWater;
            double ConductionAreaAir;
            double lambdaAir;
            double lambdaWater;
            double M1;
            double M2;
            double EpsilonRatio;
            double deltaEpsilon;
            double ActualEpsilon;
            double HeatTransferRate;
            double prevvalWater;
            double prevvalAir;
            #endregion

            #region User Input acquisition
            double L1 = _in.NumericalReadings.First(x => x.Parameter == "L1").Value;
            double L2 = _in.NumericalReadings.First(x => x.Parameter == "L2").Value;
            double L3 = _in.NumericalReadings.First(x => x.Parameter == "L3").Value;
            double Xt = _in.NumericalReadings.First(x => x.Parameter == "Xt").Value;
            double Xl = _in.NumericalReadings.First(x => x.Parameter == "Xl").Value;
            double Do = _in.NumericalReadings.First(x => x.Parameter == "Do").Value;
            double Di = _in.NumericalReadings.First(x => x.Parameter == "Di").Value;
            double DelH = _in.NumericalReadings.First(x => x.Parameter == "DelH").Value;
            double FinThickness = _in.NumericalReadings.First(x => x.Parameter == "FinThickness").Value;

            double QWaterInitial = _in.NumericalReadings.First(x => x.Parameter == "QWaterInitial").Value;
            double TWaterInitial = _in.NumericalReadings.First(x => x.Parameter == "TWaterInitial").Value;
            double PWaterInitial = _in.NumericalReadings.First(x => x.Parameter == "PWaterInitial").Value;

            double QAirInitial = _in.NumericalReadings.First(x => x.Parameter == "QAirInitial").Value;
            double TAirInitial = _in.NumericalReadings.First(x => x.Parameter == "TAirInitial").Value;
            double PAirInitial = _in.NumericalReadings.First(x => x.Parameter == "PAirInitial").Value;
            //double Nr = _in.NumericalReadings.First(x => x.Parameter == "Nr").Value;
            double FinPitch = _in.NumericalReadings.First(x => x.Parameter == "FinPitch").Value;

            double Nr = L3 / Xt;

            DensityWaterInitial = PWaterInitial / (287.04 * (TWaterInitial));
            DensityAirInitial = PAirInitial / (287.04 * (TAirInitial));

            MassFlowWaterFinal = QWaterInitial * DensityWaterInitial;
            MassFlowAirFinal = QAirInitial * DensityAirInitial;
            #endregion

            #region Iteration
            do
            {
                #region TMean calculation

                TWaterFinal = TWaterInitial - Epsilon * (TWaterInitial - TAirInitial);
                TAirFinal = TAirInitial + Epsilon * (MassFlowWaterFinal / MassFlowAirFinal) * (TWaterInitial - TAirInitial);
                TWaterMeanFinal = (TWaterFinal + TWaterInitial) / 2;
                TAirMeanFinal = (TAirFinal + TAirInitial) / 2;

                #endregion

                prevvalAir = TAirFinal;
                prevvalWater = TWaterFinal;

                #region CoolProp

                //Calculate additional properties
                ViscosityAir = CoolProp.PropsSI("V", "T", TAirMeanFinal, "P", 101325, "Air");
                ViscosityWater = CoolProp.PropsSI("V", "T", TWaterMeanFinal, "P", 101325, "Water");
                CpAir = CoolProp.PropsSI("CPMASS", "T", TWaterMeanFinal, "P", 101325, "Water");
                CpWater = CoolProp.PropsSI("CPMASS", "T", TWaterMeanFinal, "P", 101325, "Water");
                PrandtlAir = CoolProp.PropsSI("CPMASS", "T", TWaterMeanFinal, "P", 101325, "Water");
                PrandtlWater = CoolProp.PropsSI("CPMASS", "T", TWaterMeanFinal, "P", 101325, "Water");

                #endregion

                #region Calculate G and Re

                G_Air = MassFlowAirFinal /
                        ((_in.SysType == SystemType.Inline)
                            ? _in.TubeOutsideInlineData.TotalMinFfArea
                            : _in.TubeOutsideStaggered.TotalMinFfArea);

                G_Water = MassFlowWaterFinal /
                          ((_in.SysType == SystemType.Inline)
                              ? _in.TubeOutsideInlineData.TotalMinFfArea
                              : _in.TubeOutsideStaggered.TotalMinFfArea);

                Reynolds_Air = G_Air * ((_in.SysType == SystemType.Inline)
                    ? _in.TubeOutsideInlineData.HydDiameter
                    : _in.TubeOutsideStaggered.HydDiameter) / ViscosityAir;

                Reynolds_Water = G_Water * ((_in.SysType == SystemType.Inline)
                    ? _in.TubeOutsideInlineData.HydDiameter
                    : _in.TubeOutsideStaggered.HydDiameter) / ViscosityWater;

                #endregion

                #region Calculation of J

                if (Nr <= 1)
                {
                    if (_in.SysType == SystemType.Inline)
                    {

                        double c1 = 1.9 - 0.23 * Math.Log(Reynolds_Water);
                        double c2 = -0.236 + 0.126 * Math.Log(Reynolds_Water);
                        J_Water = 0.108 * Math.Pow(Reynolds_Water, -0.29) * Math.Pow(Xt / Xl, c1) * Math.Pow(FinPitch / Do, 1.084) *
                                  Math.Pow(FinPitch / _in.TubeOutsideInlineData.HydDiameter, -0.786) *
                                  Math.Pow(FinPitch / Xt, c2);


                        c1 = 1.9 - 0.23 * Math.Log(Reynolds_Air);
                        c2 = -0.236 + 0.126 * Math.Log(Reynolds_Air);
                        J_Air = 0.108 * Math.Pow(Reynolds_Air, -0.29) * Math.Pow(Xt / Xl, c1) * Math.Pow(FinPitch / Do, 1.084) *
                                Math.Pow(FinPitch / _in.TubeOutsideInlineData.HydDiameter, -0.786) *
                                Math.Pow(FinPitch / Xt, c2);
                    }
                    else
                    {
                        double c1 = 1.9 - 0.23 * Math.Log(Reynolds_Water);
                        double c2 = -0.236 + 0.126 * Math.Log(Reynolds_Water);
                        J_Water = 0.108 * Math.Pow(Reynolds_Water, -0.29) * Math.Pow(Xt / Xl, c1) * Math.Pow(FinPitch / Do, 1.084) *
                                  Math.Pow(FinPitch / _in.TubeOutsideStaggered.HydDiameter, -0.786) *
                                  Math.Pow(FinPitch / Xt, c2);


                        c1 = 1.9 - 0.23 * Math.Log(Reynolds_Air);
                        c2 = -0.236 + 0.126 * Math.Log(Reynolds_Air);
                        J_Air = 0.108 * Math.Pow(Reynolds_Air, -0.29) * Math.Pow(Xt / Xl, c1) * Math.Pow(FinPitch / Do, 1.084) *
                                Math.Pow(FinPitch / _in.TubeOutsideStaggered.HydDiameter, -0.786) *
                                Math.Pow(FinPitch / Xt, c2);
                    }
                }
                else
                {

                    if (_in.SysType == SystemType.Inline)
                    {
                        //Water
                        double c3 = -0.361 - 0.042 * Nr / Math.Log(Reynolds_Water) +
                                    0.158 * Math.Log(Nr * Math.Pow(FinPitch / Do, 0.41));
                        double c4 = -1.224 -
                                    0.076 * Math.Pow(Xl / _in.TubeOutsideInlineData.HydDiameter, 1.42) /
                                    Math.Log(Reynolds_Water);
                        double c5 = -0.083 + 0.058 * Nr / Math.Log(Reynolds_Water);
                        double c6 = -5.735 + 1.21 * Math.Log(Reynolds_Water / Nr);
                        J_Water = 0.086 * Math.Pow(Reynolds_Water, c3) * Math.Pow(Nr, c4) * Math.Pow(FinPitch / Do, c5) *
                                  Math.Pow(FinPitch / _in.TubeOutsideInlineData.HydDiameter, c6) *
                                  Math.Pow(FinPitch / Xt, -0.93);
                        //Air
                        c3 = -0.361 - 0.042 * Nr / Math.Log(Reynolds_Air) + 0.158 * Math.Log(Nr * Math.Pow(FinPitch / Do, 0.41));
                        c4 = -1.224 -
                             0.076 * Math.Pow(Xl / _in.TubeOutsideInlineData.HydDiameter, 1.42) / Math.Log(Reynolds_Air);
                        c5 = -0.083 + 0.058 * Nr / Math.Log(Reynolds_Air);
                        c6 = -5.735 + 1.21 * Math.Log(Reynolds_Air / Nr);
                        J_Air = 0.086 * Math.Pow(Reynolds_Air, c3) * Math.Pow(Nr, c4) * Math.Pow(FinPitch / Do, c5) *
                                Math.Pow(FinPitch / _in.TubeOutsideInlineData.HydDiameter, c6) *
                                Math.Pow(FinPitch / Xt, -0.93);
                    }
                    else
                    {
                        //Water
                        double c3 = -0.361 - 0.042 * Nr / Math.Log(Reynolds_Water) +
                                    0.158 * Math.Log(Nr * Math.Pow(FinPitch / Do, 0.41));
                        double c4 = -1.224 -
                                    0.076 * Math.Pow(Xl / _in.TubeOutsideStaggered.HydDiameter, 1.42) /
                                    Math.Log(Reynolds_Water);
                        double c5 = -0.083 + 0.058 * Nr / Math.Log(Reynolds_Water);
                        double c6 = -5.735 + 1.21 * Math.Log(Reynolds_Water / Nr);
                        J_Water = 0.086 * Math.Pow(Reynolds_Water, c3) * Math.Pow(Nr, c4) * Math.Pow(FinPitch / Do, c5) *
                                  Math.Pow(FinPitch / _in.TubeOutsideStaggered.HydDiameter, c6) *
                                  Math.Pow(FinPitch / Xt, -0.93);
                        //Air
                        c3 = -0.361 - 0.042 * Nr / Math.Log(Reynolds_Air) + 0.158 * Math.Log(Nr * Math.Pow(FinPitch / Do, 0.41));
                        c4 = -1.224 -
                             0.076 * Math.Pow(Xl / _in.TubeOutsideStaggered.HydDiameter, 1.42) / Math.Log(Reynolds_Air);
                        c5 = -0.083 + 0.058 * Nr / Math.Log(Reynolds_Air);
                        c6 = -5.735 + 1.21 * Math.Log(Reynolds_Air / Nr);
                        J_Air = 0.086 * Math.Pow(Reynolds_Air, c3) * Math.Pow(Nr, c4) * Math.Pow(FinPitch / Do, c5) *
                                Math.Pow(FinPitch / _in.TubeOutsideStaggered.HydDiameter, c6) * Math.Pow(FinPitch / Xt, -0.93);
                    }
                }

                #endregion

                #region H Calculation

                H_Water = J_Water * G_Water * CpWater / Math.Pow(PrandtlWater, (double)2 / 3);
                H_Air = J_Air * G_Air * CpAir / Math.Pow(PrandtlAir, (double)2 / 3);

                #endregion

                #region F Calculation

                c7 = -0.764 + 0.739 * Xt / Xl + 0.177 * FinPitch / Do - 0.00758 / Nr;
                c8 = -15.689 + 64.021 / Math.Log(Reynolds_Water);
                c9 = 1.696 - 15.695 / Math.Log(Reynolds_Water);
                F_Water = 0.0267 * Math.Pow(Reynolds_Water, c7) * Math.Pow(Xt / Xl, c8) * Math.Pow(FinPitch / Do, c9);

                c7 = -0.764 + 0.739 * Xt / Xl + 0.177 * FinPitch / Do - 0.00758 / Nr;
                c8 = -15.689 + 64.021 / Math.Log(Reynolds_Air);
                c9 = 1.696 - 15.695 / Math.Log(Reynolds_Air);
                F_Air = 0.0267 * Math.Pow(Reynolds_Air, c7) * Math.Pow(Xt / Xl, c8) * Math.Pow(FinPitch / Do, c9);

                #endregion

                #region Fin Efficiency

                VelocityAir = QAirInitial / (L1 * L3 - Nr * L1 * Do);
                if (VelocityAir <= 15 && VelocityAir >= 6)
                {
                    MessageBox.Show("Velocity should be between 6m/s and 15m/s", "Input out of limits");
                    return;
                }

                FinEfficiency = 7.41 * Math.Pow(VelocityAir, -0.12) *
                                Math.Pow(Xt / (2 * FinThickness), -2.32) * Math.Pow(Xl / (2 * FinThickness), -0.198);

                OverallEfficiency = 1 - ((1 - FinEfficiency) * (L2 * L3 - _in.Nt * Math.PI * Do * Do) /
                                         ((_in.SysType == SystemType.Inline)
                                             ? _in.TubeOutsideInlineData.TotalHeatArea
                                             : _in.TubeOutsideStaggered.TotalHeatArea));

                Rw = (Do - Di) / (_in.MaterialCoeff * (_in.Nt + 1) * 2 * L1 * L2);

                #endregion

                #region UA Calculation


                if (_in.SysType == SystemType.Inline)
                    UaInverse = Rw + (1 / (OverallEfficiency * H_Water * _in.TubeOutsideInlineData.TotalHeatArea)) +
                                (1 / (OverallEfficiency * H_Air * _in.TubeOutsideInlineData.TotalHeatArea));
                else
                    UaInverse = Rw + (1 / (OverallEfficiency * H_Water * _in.TubeOutsideStaggered.TotalHeatArea)) +
                                (1 / (OverallEfficiency * H_Air * _in.TubeOutsideStaggered.TotalHeatArea));

                C_Water = MassFlowWaterFinal * CpWater;
                C_Air = MassFlowAirFinal * CpAir;
                C_Min = Math.Min(C_Air, C_Water);
                C_Star = (C_Water / C_Air);
                if (C_Star > 1) C_Star = 1 / C_Star;

                NTU = 1 / (UaInverse * C_Min);

                #endregion

                epsilon = FactorialCompute(25, NTU, C_Star);
                ConductionAreaWater = _in.Nt * Math.PI * Di * L1;
                ConductionAreaAir = _in.Nt * Math.PI * Do * L1;

                lambdaAir = _in.MaterialCoeff * ConductionAreaAir / (L1 * C_Air);
                lambdaWater = _in.MaterialCoeff * ConductionAreaWater / (L1 * C_Water);

                M1 = H_Air / H_Water;
                M2 = lambdaWater / lambdaAir;

                XPlatHelper.OpenImageTables();


                while (true)
                {
                    UserInputDialog userInput = new UserInputDialog("Enter Delta(Epsilon)/Epsilon");
                    userInput.ShowDialog();
                    if (!string.IsNullOrWhiteSpace(userInput.Answer))
                    {
                        double x;
                        if (double.TryParse(userInput.Answer, out x))
                        {
                            EpsilonRatio = x;
                            break;
                        }
                    }
                }

                deltaEpsilon = EpsilonRatio * Epsilon;
                ActualEpsilon = Epsilon - deltaEpsilon;

                HeatTransferRate = ActualEpsilon * (TAirInitial - TWaterInitial) * C_Min;

                TWaterFinal = prevvalWater - HeatTransferRate / C_Water;
                TAirFinal = prevvalAir - HeatTransferRate / C_Air;
            } while (!((Math.Abs((prevvalWater - TWaterFinal) / prevvalWater) < 0.05) &&
                     (Math.Abs((prevvalAir - TAirFinal) / prevvalAir) < 0.05)));
            #endregion

            {
                double TMeanAir = (TAirInitial + TAirFinal) / 2;
                double TMeanWater = (TWaterInitial + TWaterFinal) / 2;
                double RoFinalAir = (PAirInitial / 287.04) / TAirFinal;
                double RoFinalWater = (PWaterInitial / 287.04) / TWaterFinal;
                double RoMeanAir = 2 * RoFinalAir * DensityAirInitial / (RoFinalAir + DensityAirInitial);
                double RoMeanWater = 2 * RoFinalWater * DensityWaterInitial / (RoFinalWater + DensityWaterInitial);
                double RhFinal = (_in.SysType == SystemType.Inline)
                    ? _in.TubeOutsideInlineData.TotalMinFfArea
                    : _in.TubeOutsideStaggered.TotalMinFfArea;
                RhFinal /= 2;
                double GAirFinal = MassFlowAirFinal / ((_in.SysType == SystemType.Inline)
                    ? _in.TubeOutsideInlineData.TotalMinFfArea
                    : _in.TubeOutsideStaggered.TotalMinFfArea);
                double GWaterFinal = MassFlowWaterFinal / ((_in.SysType == SystemType.Inline)
                    ? _in.TubeOutsideInlineData.TotalMinFfArea
                    : _in.TubeOutsideStaggered.TotalMinFfArea);

                XPlatHelper.OpenFinalTable();
                double Kc, Ke;
                while (true)
                {
                    UserInputDialog userInput = new UserInputDialog("Enter Kc");
                    userInput.ShowDialog();
                    if (!string.IsNullOrWhiteSpace(userInput.Answer))
                    {
                        double x;
                        if (double.TryParse(userInput.Answer, out x))
                        {
                            Kc = x;
                            break;
                        }
                    }
                }
                while (true)
                {
                    UserInputDialog userInput = new UserInputDialog("Enter Ke");
                    userInput.ShowDialog();
                    if (!string.IsNullOrWhiteSpace(userInput.Answer))
                    {
                        double x;
                        if (double.TryParse(userInput.Answer, out x))
                        {
                            Ke = x;
                            break;
                        }
                    }
                }

                double Sigma = (_in.SysType == SystemType.Inline)
                    ? _in.TubeOutsideInlineData.RatioFfFrontalArea
                    : _in.TubeOutsideStaggered.RatioFfFrontalArea;

                double delPAir =
                    Math.Pow(GAirFinal, 2)/(2*DensityAirInitial)*(
                        (1 - Sigma*Sigma + Kc) + 2*(DensityAirInitial/RoFinalAir - 1) +
                        (F_Air*L1/RhFinal*DensityAirInitial/RoMeanAir)
                        - (1 - Sigma*Sigma - Ke*DensityAirInitial/RoFinalAir));

                double delPWater =
                    Math.Pow(GWaterFinal, 2) / (2 * DensityWaterInitial) * (
                        (1 - Sigma * Sigma + Kc) + 2 * (DensityWaterInitial / RoFinalWater - 1) +
                        (F_Water * L1 / RhFinal * DensityWaterInitial / RoMeanWater)
                        - (1 - Sigma * Sigma - Ke * DensityWaterInitial / RoFinalWater));

                AirX = delPAir;
                WaterX = delPWater;

            }

        }

        public static double FactorialCompute(int n, double ntu, double c_star)
        {
            double sum = 0;
            double nfactorial = MathNet.Numerics.SpecialFunctions.Factorial(n + 1);
            for (int i = 1; i <= n; i++)
            {
                sum += (n + 1 - i) / MathNet.Numerics.SpecialFunctions.Factorial(i) * Math.Pow(ntu, n + i);
            }
            sum /= nfactorial;

            double epsilon = 0;
            double secsum = 0;
            for (int i = 1; i <= 25; i++)
            {
                secsum += Math.Pow(c_star, n);
            }
            epsilon = 1 - Math.Exp(-1 * ntu) - Math.Exp(-1 * (1 + c_star) * ntu) * secsum;
            return epsilon;
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
