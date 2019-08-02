﻿using EmbyStat.Clients.Emby.Http;
using EmbyStat.Common;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Events;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Emby;
using EmbyStat.Services.Models.Stat;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EmbyStat.Services
{
    public class EmbyService : IEmbyService
    {
        private readonly IEmbyClient _embyClient;
        private readonly IEmbyRepository _embyRepository;
        private readonly ISessionService _sessionService;
        private readonly ISettingsService _settingsService;
        private readonly IMovieRepository _movieRepository;
        private readonly IMusicRepository _musicRepository;
        private readonly IShowRepository _showRepository;
        private readonly Logger _logger;

        public EmbyService(IEmbyClient embyClient, IEmbyRepository embyRepository, ISessionService sessionService,
            ISettingsService settingsService, IMovieRepository movieRepository, IMusicRepository musicRepository, IShowRepository showRepository)
        {
            _embyClient = embyClient;
            _embyRepository = embyRepository;
            _sessionService = sessionService;
            _settingsService = settingsService;
            _movieRepository = movieRepository;
            _musicRepository = musicRepository;
            _showRepository = showRepository;
            _logger = LogManager.GetCurrentClassLogger();
        }

        #region Server
        public EmbyUdpBroadcast SearchEmby()
        {
            using (var client = new UdpClient())
            {
                var requestData = Encoding.ASCII.GetBytes("who is EmbyServer?");
                var serverEp = new IPEndPoint(IPAddress.Any, 7359);

                client.EnableBroadcast = true;
                client.Send(requestData, requestData.Length, new IPEndPoint(IPAddress.Broadcast, 7359));

                var timeToWait = TimeSpan.FromSeconds(2);

                var asyncResult = client.BeginReceive(null, null);
                asyncResult.AsyncWaitHandle.WaitOne(timeToWait);
                if (asyncResult.IsCompleted)
                {
                    try
                    {
                        var receivedData = client.EndReceive(asyncResult, ref serverEp);
                        var serverResponse = Encoding.ASCII.GetString(receivedData);
                        var udpBroadcastResult = JsonConvert.DeserializeObject<EmbyUdpBroadcast>(serverResponse);

                        var settings = _settingsService.GetUserSettings();
                        settings.Emby.ServerName = udpBroadcastResult.Name;
                        _settingsService.SaveUserSettingsAsync(settings);

                        if (!string.IsNullOrWhiteSpace(udpBroadcastResult.Address))
                        {
                            _logger.Info($"{Constants.LogPrefix.ServerApi}\tEmby server found at: " + udpBroadcastResult.Address);
                        }

                        return udpBroadcastResult;

                    }
                    catch (Exception)
                    {
                        // No data received, swallow exception and return empty object
                    }
                }

                return new EmbyUdpBroadcast();
            }
        }

        public async Task<EmbyToken> GetEmbyToken(EmbyLogin login)
        {
            if (!string.IsNullOrEmpty(login?.Password) && !string.IsNullOrEmpty(login.UserName))
            {
                try
                {
                    var token = await _embyClient.AuthenticateUserAsync(login.UserName, login.Password, login.Address);
                    return new EmbyToken
                    {
                        Token = token.AccessToken,
                        Username = token.User.ConnectUserName,
                        IsAdmin = token.User.Policy.IsAdministrator,
                        Id = token.User.Id
                    };
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                    _logger.Warn($"{Constants.LogPrefix.ServerApi}\tUsername or password are wrong, user should try again with other credentials!");
                    throw new BusinessException("TOKEN_FAILED", 500, e);
                }
            }

            _logger.Warn("Username or password are empty, no use to try a login!");
            throw new BusinessException("TOKEN_FAILED");
        }

        public async Task<ServerInfo> GetServerInfo()
        {
            var server = _embyRepository.GetServerInfo();
            if (server == null)
            {
                var settings = _settingsService.GetUserSettings();
                await GetAndProcessServerInfo(settings.FullEmbyServerAddress, settings.Emby.AccessToken);
            }

            return server;
        }

        public EmbyStatus GetEmbyStatus()
        {
            return _embyRepository.GetEmbyStatus();
        }

        public async Task<string> PingEmbyAsync(string embyAddress, string accessToken, CancellationToken cancellationToken)
        {
            return await _embyClient.PingEmbyAsync(embyAddress, cancellationToken);
        }

        public void ResetMissedPings()
        {
            _embyRepository.ResetMissedPings();
        }

        public void IncreaseMissedPings()
        {
            _embyRepository.IncreaseMissedPings();
        }

        #endregion

        #region Plugin

        public List<PluginInfo> GetAllPlugins()
        {
            return _embyRepository.GetAllPlugins();
        }

        #endregion

        #region Users

        public IEnumerable<EmbyUser> GetAllUsers()
        {
            return _embyRepository.GetAllUsers();
        }

        public EmbyUser GetUserById(string id)
        {
            return _embyRepository.GetUserById(id);
        }

        public Card<int> GetViewedEpisodeCountByUserId(string id)
        {
            var episodeIds = _sessionService.GetMediaIdsForUser(id, PlayType.Episode);
            return new Card<int>
            {
                Title = Constants.Users.TotalWatchedEpisodes,
                Value = episodeIds.Count()
            };
        }

        public Card<int> GetViewedMovieCountByUserId(string id)
        {
            var movieIds = _sessionService.GetMediaIdsForUser(id, PlayType.Movie);
            return new Card<int>
            {
                Title = Constants.Users.TotalWatchedMovies,
                Value = movieIds.Count()
            };
        }

        public Card<int> GetViewedMusicCountByUserId(string id)
        {
            var musicIds = _sessionService.GetMediaIdsForUser(id, PlayType.Music);
            return new Card<int>
            {
                Title = Constants.Users.TotalWatchedMusic,
                Value = musicIds.Count()
            };
        }

        public IEnumerable<UserMediaView> GetUserViewPageByUserId(string id, int page, int size)
        {
            var skip = page * size;
            var sessions = _sessionService.GetSessionsForUser(id).ToList();
            var plays = sessions.SelectMany(x => x.Plays).Skip(skip).Take(size).ToList();
            var deviceIds = sessions.Where(x => plays.Any(y => x.Plays.Any(z => z.Id == y.Id))).Select(x => x.DeviceId);

            var devices = _embyRepository.GetDeviceById(deviceIds).ToList();

            foreach (var play in plays)
            {
                var currentSession = sessions.Single(x => x.Plays.Any(y => y.Id == play.Id));
                var device = devices.SingleOrDefault(x => x.Id == currentSession.DeviceId);

                if (play.Type == PlayType.Movie)
                {
                    yield return CreateUserMediaViewFromMovie(play, device);
                }
                else if (play.Type == PlayType.Episode)
                {
                    yield return CreateUserMediaViewFromEpisode(play, device);
                }

                //if media is null this means a user has watched something that is not yet in our DB
                //TODO: try starting a sync here
            }
        }

        public int GetUserViewCount(string id)
        {
            return _sessionService.GetPlayCountForUser(id);
        }

        #endregion

        #region JobHelpers

        public async Task GetAndProcessServerInfo(string embyAddress, string accessToken)
        {
            var server = await _embyClient.GetServerInfoAsync();

            if (string.IsNullOrEmpty(server.LocalAddress))
            {
                server.LocalAddress = string.Empty;
            }

            _embyRepository.UpsertServerInfo(server);
        }

        public async Task GetAndProcessPluginInfo(string embyAddress, string accessToken)
        {
            var plugins = await _embyClient.GetInstalledPluginsAsync();

            _embyRepository.RemoveAllAndInsertPluginRange(PluginConverter.ConvertToPluginList(plugins));
        }

        public async Task GetAndProcessEmbyUsers(string embyAddress, string accessToken)
        {
            var usersJson = await _embyClient.GetEmbyUsersAsync();
            var users = UserConverter.ConvertToUserList(usersJson).ToList();
            _embyRepository.UpsertUsers(users);

            var localUsers = _embyRepository.GetAllUsers();
            var removedUsers = localUsers.Where(u => users.All(u2 => u2.Id != u.Id));
            _embyRepository.MarkUsersAsDeleted(removedUsers);
        }

        public async Task GetAndProcessDevices(string embyAddress, string accessToken)
        {
            var devicesJson = await _embyClient.GetEmbyDevicesAsync();
            var devices = DeviceConverter.ConvertToDeviceList(devicesJson).ToList();
            _embyRepository.UpsertDevices(devices);

            var localDevices = _embyRepository.GetAllDevices();
            var removedDevices = localDevices.Where(d => devices.All(d2 => d2.Id != d.Id));
            _embyRepository.MarkDevicesAsDeleted(removedDevices);
        }

        #endregion

        private UserMediaView CreateUserMediaViewFromMovie(Play play, Device device)
        {
            var movie = _movieRepository.GetMovieById(play.MediaId);
            if (movie == null)
            {
                throw new BusinessException("MOVIENOTFOUND");
            }

            var startedPlaying = play.PlayStates.Min(x => x.TimeLogged);
            var endedPlaying = play.PlayStates.Max(x => x.TimeLogged);
            var watchedTime = endedPlaying - startedPlaying;

            return new UserMediaView
            {
                Id = movie.Id,
                Name = movie.Name,
                ParentId = movie.ParentId,
                Primary = movie.Primary,
                StartedWatching = startedPlaying,
                EndedWatching = endedPlaying,
                WatchedTime = Math.Round(watchedTime.TotalSeconds),
                WatchedPercentage = CalculateWatchedPercentage(play, movie),
                DeviceId = device?.Id ?? string.Empty,
                DeviceLogo = device?.IconUrl ?? string.Empty
            };
        }

        private UserMediaView CreateUserMediaViewFromSong(Play play, Device device)
        {
            var song = _musicRepository.GetSongById(play.MediaId);
            if (song == null)
            {
                throw new BusinessException("MOVIENOTFOUND");
            }

            var startedPlaying = play.PlayStates.Min(x => x.TimeLogged);
            var endedPlaying = play.PlayStates.Max(x => x.TimeLogged);
            var watchedTime = endedPlaying - startedPlaying;

            return new UserMediaView
            {
                Id = song.Id,
                Name = song.Name,
                ParentId = song.ParentId,
                Primary = song.Primary,
                StartedWatching = startedPlaying,
                EndedWatching = endedPlaying,
                WatchedTime = Math.Round(watchedTime.TotalSeconds),
                WatchedPercentage = CalculateWatchedPercentage(play, song),
                DeviceId = device?.Id ?? string.Empty,
                DeviceLogo = device?.IconUrl ?? string.Empty
            };
        }

        private UserMediaView CreateUserMediaViewFromEpisode(Play play, Device device)
        {
            var episode = _showRepository.GetEpisodeById(play.MediaId);
            if (episode == null)
            {
                throw new BusinessException("EPISODENOTFOUND");
            }

            var season = _showRepository.GetSeasonById(play.ParentId);
            var seasonNumber = season.IndexNumber;
            var name = $"{episode.ShowName} - {seasonNumber}x{episode.IndexNumber} - {episode.Name}";

            var startedPlaying = play.PlayStates.Min(x => x.TimeLogged);
            var endedPlaying = play.PlayStates.Max(x => x.TimeLogged);
            var watchedTime = endedPlaying - startedPlaying;


            return new UserMediaView
            {
                Id = episode.Id,
                Name = name,
                ParentId = episode.ParentId,
                Primary = episode.Primary,
                StartedWatching = startedPlaying,
                EndedWatching = endedPlaying,
                WatchedTime = Math.Round(watchedTime.TotalSeconds),
                WatchedPercentage = CalculateWatchedPercentage(play, episode),
                DeviceId = device.Id,
                DeviceLogo = device?.IconUrl ?? ""
            };
        }

        private decimal? CalculateWatchedPercentage(Play play, Extra media)
        {
            decimal? watchedPercentage = null;
            if (media.RunTimeTicks.HasValue)
            {
                var playStates = play.PlayStates.Where(x => x.PositionTicks.HasValue).ToList();
                var watchedTicks = playStates.Max(x => x.PositionTicks) -
                                   playStates.Min(x => x.PositionTicks);
                watchedPercentage = Math.Round((watchedTicks.Value / (decimal)media.RunTimeTicks.Value) * 1000) / 10;
            }

            return watchedPercentage;
        }

        private decimal? CalculateWatchedPercentage(Play play, Audio media)
        {
            decimal? watchedPercentage = null;
            if (media.RunTimeTicks.HasValue)
            {
                var playStates = play.PlayStates.Where(x => x.PositionTicks.HasValue).ToList();
                var watchedTicks = playStates.Max(x => x.PositionTicks) -
                                   playStates.Min(x => x.PositionTicks);
                watchedPercentage = Math.Round((watchedTicks.Value / (decimal)media.RunTimeTicks.Value) * 1000) / 10;
            }

            return watchedPercentage;
        }

        public void Dispose()
        {
            _embyClient?.Dispose();
        }
    }
}
