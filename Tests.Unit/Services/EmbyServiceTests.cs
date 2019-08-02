﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Clients.Emby.Http;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Common.Net;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Emby;
using FluentAssertions;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Session;
using MediaBrowser.Model.Users;
using Moq;
using Xunit;
using PluginInfo = EmbyStat.Common.Models.Entities.PluginInfo;

namespace Tests.Unit.Services
{
	[Collection("Mapper collection")]
	public class EmbyServiceTests
    {
	    private readonly EmbyService _subject;
	    private readonly Mock<IEmbyClient> _embyClientMock;
        private readonly ServerInfo _serverInfo;

		public EmbyServiceTests()
	    {
            var plugins = new List<PluginInfo>
            {
                new PluginInfo { Name = "EmbyStat plugin" },
                new PluginInfo { Name = "Trakt plugin" }
            };

            var embyPlugins = new List<MediaBrowser.Model.Plugins.PluginInfo>
            {
                new MediaBrowser.Model.Plugins.PluginInfo {Name = "EmbyStat plugin"},
                new MediaBrowser.Model.Plugins.PluginInfo {Name = "Trakt plugin"}
            };

			_serverInfo = new ServerInfo
			{
				Id = Guid.NewGuid().ToString(),
				HttpServerPortNumber = 8096,
				HttpsPortNumber = 8097
			};

            var embyDrives = new List<FileSystemEntryInfo>
		    {
			    new FileSystemEntryInfo()
		    };

			var systemInfo = new ServerInfo();

			_embyClientMock = new Mock<IEmbyClient>();
		    _embyClientMock.Setup(x => x.GetInstalledPluginsAsync()).Returns(Task.FromResult(embyPlugins));
		    _embyClientMock.Setup(x => x.AuthenticateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
		    _embyClientMock.Setup(x => x.GetServerInfoAsync()).Returns(Task.FromResult(systemInfo));
		    _embyClientMock.Setup(x => x.GetLocalDrivesAsync()).Returns(Task.FromResult(embyDrives));

            var embyRepository = new Mock<IEmbyRepository>();
            embyRepository.Setup(x => x.GetAllPlugins()).Returns(plugins);
            embyRepository.Setup(x => x.RemoveAllAndInsertPluginRange(It.IsAny<List<PluginInfo>>()));
            embyRepository.Setup(x => x.UpsertServerInfo(It.IsAny<ServerInfo>()));
            embyRepository.Setup(x => x.GetServerInfo()).Returns(_serverInfo);

            var settingsServiceMock = new Mock<ISettingsService>();
	        settingsServiceMock.Setup(x => x.GetUserSettings()).Returns(new UserSettings());

            var movieRepositoryMock = new Mock<IMovieRepository>();
            var musicRepositoryMock = new Mock<IMusicRepository>();
            var showRepositoryMock = new Mock<IShowRepository>();
            var embyRepositoryMock = new Mock<IEmbyRepository>();
            var sessionServiceMock = new Mock<ISessionService>();

            _subject = new EmbyService(_embyClientMock.Object, embyRepositoryMock.Object, sessionServiceMock.Object,
                settingsServiceMock.Object, movieRepositoryMock.Object, musicRepositoryMock.Object, showRepositoryMock.Object);
	    }

        [Fact]
        public async void GetEmbyToken()
        {
            var authResult = new AuthenticationResult
            {
                AccessToken = "00000000",
                ServerId = "1111",
                User = new UserDto {ConnectUserName = "reggi", Policy = new UserPolicy {IsAdministrator = false}, Id = Guid.NewGuid()},
                SessionInfo = new SessionInfoDto()
            };

            _embyClientMock.Setup(x => x.AuthenticateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(authResult));
            var login = new EmbyLogin
            {
                Password = "AdminPass",
                Address = "http://localhost",
                UserName = "reggi"
            };
            var result = await _subject.GetEmbyToken(login);

            result.Should().NotBeNull();
            result.Id = authResult.User.Id;
            result.IsAdmin = authResult.User.Policy.IsAdministrator;
            result.Username = authResult.User.ConnectUserName;
            result.Token = authResult.AccessToken;
        }

        [Fact]
		public async void GetEmbyTokenWithNoLoginInfo()
	    {
		    var ex = await Assert.ThrowsAsync<BusinessException>(() => _subject.GetEmbyToken(null));

		    ex.Message.Should().Be("TOKEN_FAILED");
		    ex.StatusCode.Should().Be(500);
	    }

	    [Fact]
	    public async void GetEmbyTokenWithNoPassword()
	    {
			var login = new EmbyLogin
			{
				UserName = "Admin",
				Address = "http://localhost"
			};
		    var ex = await Assert.ThrowsAsync<BusinessException>(() => _subject.GetEmbyToken(login));

		    ex.Message.Should().Be("TOKEN_FAILED");
		    ex.StatusCode.Should().Be(500);
	    }

	    [Fact]
	    public async void GetEmbyTokenWithNoUserName()
	    {
		    var login = new EmbyLogin
		    {
			    Password = "AdminPass",
			    Address = "http://localhost"
		    };
		    var ex = await Assert.ThrowsAsync<BusinessException>(() => _subject.GetEmbyToken(login));

		    ex.Message.Should().Be("TOKEN_FAILED");
		    ex.StatusCode.Should().Be(500);
	    }

	    [Fact]
	    public async void GetEmbyTokenFailedLogin()
	    {
		    _embyClientMock.Setup(x => x.AuthenticateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
			    .ThrowsAsync(new Exception());
			var login = new EmbyLogin
		    {
			    Password = "AdminPass",
			    Address = "http://localhost",
				UserName = "Admin"
		    };
		    var ex = await Assert.ThrowsAsync<BusinessException>(() => _subject.GetEmbyToken(login));

		    ex.Message.Should().Be("TOKEN_FAILED");
		    ex.StatusCode.Should().Be(500);
	    }
	}
}