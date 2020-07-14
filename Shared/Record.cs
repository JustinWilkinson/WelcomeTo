using System;
using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Shared
{
    public class Record
    {
        public RecordType Type { get; set; } 

        public string Game { get; set; }

        public string Player { get; set; }

        public int Score { get; set; }

        public int Position { get; set; }

        public DateTime Date { get; set; }
    }
}