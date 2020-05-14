using System;
using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Shared
{
    public class CityPlans
    {
        public CityPlan Plan1 { get; set; }

        public CityPlan Plan2 { get; set; }

        public CityPlan Plan3 { get; set; }

        public CityPlan this[PlanType planType]
        {
            get => planType switch
            {
                PlanType.No1 => Plan1,
                PlanType.No2 => Plan2,
                PlanType.No3 => Plan3,
                _ => throw new ArgumentException($"Unrecognized plan type {planType}"!)
            };
        }
    }
}