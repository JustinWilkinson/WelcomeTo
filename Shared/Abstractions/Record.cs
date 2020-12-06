using System;
using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Shared.Abstractions
{
    public record Record
    {
        public RecordType Type { get; init; } 

        public string Game { get; init; }

        public string Player { get; init; }

        public int Score { get; init; }

        public int Position { get; set; }

        public DateTime Date { get; init; }
    }
}