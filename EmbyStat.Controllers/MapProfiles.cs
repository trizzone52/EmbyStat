﻿using System;
using AutoMapper;
using EmbyStat.Clients.GitHub.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Controllers.About;
using EmbyStat.Controllers.Emby;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Controllers.Job;
using EmbyStat.Controllers.Log;
using EmbyStat.Controllers.Movie;
using EmbyStat.Controllers.Music;
using EmbyStat.Controllers.Settings;
using EmbyStat.Controllers.Show;
using EmbyStat.Controllers.System;
using EmbyStat.Services.Models.Charts;
using EmbyStat.Services.Models.Emby;
using EmbyStat.Services.Models.Movie;
using EmbyStat.Services.Models.Music;
using EmbyStat.Services.Models.Show;
using EmbyStat.Services.Models.Stat;
using MediaBrowser.Model.System;
using LogFile = EmbyStat.Services.Models.Logs.LogFile;

namespace EmbyStat.Controllers
{
    public class MapProfiles : Profile
    {
	    public MapProfiles()
	    { 
            CreateMap<MediaBrowser.Model.Plugins.PluginInfo, PluginInfo>();
            CreateMap<UserSettings, FullSettingsViewModel>()
                .ForMember(x => x.Version, x => x.Ignore())
                .ReverseMap()
                .ForMember(x => x.Version, x => x.Ignore());
            CreateMap<EmbySettings, FullSettingsViewModel.EmbySettingsViewModel>().ReverseMap();
            CreateMap<TvdbSettings, FullSettingsViewModel.TvdbSettingsViewModel>().ReverseMap();
            CreateMap<Language, LanguageViewModel>();
		    CreateMap<EmbyUdpBroadcast, EmbyUdpBroadcastViewModel>().ReverseMap();
		    CreateMap<EmbyLogin, EmbyLoginViewModel>().ReverseMap();
            CreateMap<EmbyToken, EmbyTokenViewModel>()
                .ForMember(x => x.IsAdmin, opt => opt.MapFrom<BooleanToCheckBoolean>());
		    CreateMap<PluginInfo, EmbyPluginViewModel>();
		    CreateMap<ServerInfo, ServerInfoViewModel>();
            CreateMap<UpdateResult, UpdateResultViewModel>();
	        CreateMap<Common.Models.Entities.Job, JobViewModel>();
	        CreateMap<TimeSpanCard, TimeSpanCardViewModel>();
	        CreateMap( typeof(Card<>), typeof(CardViewModel<>));
	        CreateMap<MoviePoster, MoviePosterViewModel>();
            CreateMap<SongPoster, SongPosterViewModel>();
            CreateMap<ShowPoster, ShowPosterViewModel>();
            CreateMap<PersonPoster, PersonPosterViewModel>();
            CreateMap<MovieStatistics, MovieStatisticsViewModel>();
            CreateMap<MusicStatistics, MusicStatisticsViewModel>();
            CreateMap<ShowStatistics, ShowStatisticsViewModel>();
            CreateMap<MovieGeneral, MovieGeneralViewModel>();
            CreateMap<MusicGeneral, MusicGeneralViewModel>();
            CreateMap<PersonStats, PersonStatsViewModel>();
            CreateMap<Collection, CollectionViewModel>();
	        CreateMap<MovieDuplicate, MovieDuplicateViewModel>();
	        CreateMap<MovieDuplicateItem, MovieDuplicateItemViewModel>();
            CreateMap<SongDuplicate, SongDuplicateViewModel>();
            CreateMap<SongDuplicateItem, SongDuplicateItemViewModel>();
            CreateMap<Chart, ChartViewModel>();
	        CreateMap<MovieCharts, MovieChartsViewModel>();
            CreateMap<MusicCharts, MusicChartsViewModel>();
            CreateMap<ShowCharts, ShowChartsViewModel>();
            CreateMap<ShortMovie, ShortMovieViewModel>();
	        CreateMap<SuspiciousTables, SuspiciousTablesViewModel>();
            CreateMap<SuspiciousSongTables, SuspiciousSongTablesViewModel>();
            CreateMap<ShowGeneral, ShowGeneralViewModel>();
	        CreateMap<ShowCollectionRow, ShowCollectionRowViewModel>();
	        CreateMap<LogFile, LogFileViewModel>();
	        CreateMap<Services.Models.About.About, AboutViewModel>();
	        CreateMap<SuspiciousMovie, SuspiciousMovieViewModel>();
            CreateMap<SuspiciousSong, SuspiciousSongViewModel>();
            CreateMap<EmbyUser, UserIdViewModel>();
            CreateMap<EmbyUser, EmbyUserOverviewViewModel>();
            CreateMap<EmbyUser, EmbyUserFullViewModel>();
            CreateMap<UserAccessSchedule, UserAccessScheduleViewModel>();
            CreateMap<UserMediaView, UserMediaViewViewModel>();
            CreateMap<SystemInfo, ServerInfo>()
			    .ForMember(x => x.Id, y => Guid.NewGuid())
			    .ReverseMap()
			    .ForMember(x => x.CompletedInstallations, y => y.Ignore());
	    }

        private class BooleanToCheckBoolean : IValueResolver<EmbyToken, EmbyTokenViewModel, int>
        {
            public int Resolve(EmbyToken source, EmbyTokenViewModel destination, int destMember, ResolutionContext context)
            {
                return source.IsAdmin ? 2 : 3;
            }
        }
    }
}
