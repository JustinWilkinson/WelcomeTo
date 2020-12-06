using System;
using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Shared.Abstractions
{
    public record CityPlans 
    {
        public CityPlan Plan1 { get; init; }

        public CityPlan Plan2 { get; init; }

        public CityPlan Plan3 { get; init; }

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