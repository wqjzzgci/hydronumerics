using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroCat.Core
{
    public class LinearReservoirsMassBalance
    {
        // -- surface runoff linear reservoir ---
        [DescriptionAttribute("Water added to the linear reservoir for routing overlandflow [Unit: Millimiters]"), CategoryAttribute("Overland flow")]
        public double OverlandFlowLrInflow { get; private set; }
        [DescriptionAttribute("Outflow from the linear reservoir for routing overlandflow  [Unit: Millimiters]"), CategoryAttribute("Overland flow")]
        public double OverlandFlowLrOutflow { get; private set; }
        [DescriptionAttribute("Storage change in the linear reservoir for routing overlandflow [Unit: Millimiters]"), CategoryAttribute("Overland flow")]
        public double OverlandFlowLRStorageChange 
        {
            get
            {
                return OverlandFlowLrInflow - OverlandFlowLrOutflow;
            }
        }

        // -- interflow linear reservoir ---
        [DescriptionAttribute("Water added to the linear reservoir for routing interflow[Unit: Millimiters]"), CategoryAttribute("Interflow")]
        public double InterflowLrInflow { get; private set; }
        [DescriptionAttribute("Outflow from the linear reservoir for routing interflow [Unit: Millimiters]"), CategoryAttribute("Interflow")]
        public double InterflowLrOutflow { get; private set; }
        [DescriptionAttribute("Storage change in the linear reservoir for routing interflow[Unit: Millimiter]"), CategoryAttribute("Interflow")]
        public double InterflowLrStorageChange 
        {
            get { return InterflowLrInflow - InterflowLrOutflow; }
        }

        // -- BaseFlow (groundwater) linear reservoir ---
        [DescriptionAttribute("Water added to the linear reservoir for routing baseflow [Unit: Millimiters]"), CategoryAttribute("Baseflow")]
        public double BaseflowLrInflow { get; private set; }
        [DescriptionAttribute("Outflow from the linear reservoir for routing baseflow [Unit: Millimiters]"), CategoryAttribute("Baseflow")]
        public double BaseflowLrOutflow { get; private set; }
        [DescriptionAttribute("Storage change in the linear reservoir for routing baseflow[Unit: Millimiter]"), CategoryAttribute("Baseflow")]
        public double BaseflowLrStorageChange 
        {
            get { return BaseflowLrInflow - BaseflowLrOutflow; }
        }

        public void SetValues(double overlandflowLrInflow, double overlandflowLrOutflow, double interflowLrInflow, double interflowLrOutflow, double baseflowLrInflow, double baseflowLrOutflow)
        {
            OverlandFlowLrInflow += overlandflowLrInflow;
            OverlandFlowLrOutflow += overlandflowLrOutflow;
            InterflowLrInflow += interflowLrInflow;
            InterflowLrOutflow += interflowLrOutflow;
            BaseflowLrInflow += baseflowLrInflow;
            BaseflowLrOutflow += baseflowLrOutflow;
        }

        public void Initialize()
        {
            OverlandFlowLrInflow = 0;
            OverlandFlowLrOutflow = 0;
            InterflowLrInflow = 0;
            InterflowLrOutflow = 0;
            BaseflowLrInflow = 0;
            BaseflowLrOutflow = 0;
        }
    }
}
