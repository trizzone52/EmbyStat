﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.Unit.Services
{
	[Collection("Mapper collection")]
    public class LanguageServiceTests
    {
        private readonly List<Language> _languages;
        private readonly LanguageService _languageService;
        private readonly Mock<ILanguageRepository> _languageRepositoryMock;

        public LanguageServiceTests()
        {
            _languages = new List<Language>
            {
                new Language{ Name = "Nederlands", Code = "nl-BE", Id = "100" },
                new Language{ Name = "English", Code = "en-US", Id = "101" }
            };

            _languageRepositoryMock = new Mock<ILanguageRepository>();
            _languageRepositoryMock.Setup(x => x.GetLanguages()).Returns(_languages);

            _languageService = new LanguageService(_languageRepositoryMock.Object);
        }

        [Fact]
        public void GetLanguages()
        {
            var languages = _languageService.GetLanguages().ToList();

            languages.Should().NotBeNull();
            languages.Count.Should().Be(_languages.Count);

            languages[0].Name.Should().Be("Nederlands");
            languages[0].Code.Should().Be("nl-BE");
            languages[0].Id.Should().Be("100");

            languages[1].Name.Should().Be("English");
            languages[1].Code.Should().Be("en-US");
            languages[1].Id.Should().Be("101");

            _languageRepositoryMock.Verify(x => x.GetLanguages(), Times.Exactly(1));
        }
    }
}
