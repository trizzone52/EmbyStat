using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Models.Music
{
    public class MusicStatistics
    {
        public MusicGeneral General { get; set; }
        public MusicCharts Charts { get; set; }
        public PersonStats People { get; set; }
    }
}
