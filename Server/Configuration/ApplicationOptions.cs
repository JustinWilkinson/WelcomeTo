﻿using System.Collections.Generic;
using WelcomeTo.Shared;
using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Server.Configuration
{
    public class ApplicationOptions
    {
        public string ConnectionString { get; set; }

        public Dictionary<CardType, List<int>> CardDistribution { get; set; }

        public List<CityPlan> CityPlans { get; set; }

        public Dictionary<StreetPosition, List<bool>> PoolPositions { get; set; }

        public Dictionary<StreetPosition, List<int>> ParkPoints { get; set; }
    }
}