using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Services.Models.Movie;

namespace EmbyStat.Controllers.Music
{
    public class MusicStatisticsViewModel
    {
        public MusicGeneralViewModel General { get; set; }
        public MusicChartsViewModel Charts { get; set; }
        public PersonStatsViewModel People { get; set; }
        //public SuspiciousTablesViewModel Suspicious { get; set; }
    }
}
