
using System.Collections.Generic;
using EmbyStat.Services.Models.Charts;

namespace EmbyStat.Services.Models.Music
{
    public class MusicCharts
    {
        public List<Chart> BarCharts { get; set; }

        public MusicCharts()
        {
            BarCharts = new List<Chart>();
        }
    }
}
