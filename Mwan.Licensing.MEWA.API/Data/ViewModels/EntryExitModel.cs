using System;

namespace Mwan.Licensing.MEWA.API.Data.ViewModels
{
    public class EntryExitModel
    {
        public string Name { get; set; }

        public string Department { get; set; }
        public DateTime? EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public DateTime? EarlyEntryTime { get; set; }
        public DateTime? EarlyExitTime { get; set; }


    }
}
