using System.Collections.Generic;

namespace WelcomeTo.Shared
{
    public class ScoreSheet
    {
        public List<RealEstateValueTableCell> RealEstateValueTableCell { get; set; }

        public int Plan1 { get; set; }

        public int Plan2{ get; set; }

        public int Plan3 { get; set; }

        public int TopParks{ get; set; }

        public int MiddleParks { get; set; }

        public int BottomParks { get; set; }

        public int Pools { get; set; }

        public int TempAgency { get; set; }

        public int EstateValue { get; set; }

        public int Bis { get; set; }

        public int Refusals { get; set; }

        public int Total => Plan1 + Plan2 + Plan3 + TopParks + MiddleParks + BottomParks + TempAgency + EstateValue - Bis - Refusals;
    }
}