using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using MediaBrowser.Model.Entities;

namespace Tests.Unit.Builders
{
    public class MusicBuilder
    {
        private readonly Song _song;

        public MusicBuilder(int id)
        {
            _song = new Song
            {
                Id = id,
                Name = "The lord of the rings",
                PremiereDate = new DateTime(2002, 4, 2, 0, 0, 0),
                DateCreated = new DateTime(2018, 1, 1, 0, 0, 0),
                Primary = "primaryImage",
                Codec = MediaContainer.Flac.ToString(),
                Genres = new[] {"id1"},
                People = new[] {new ExtraPerson {Id = Guid.NewGuid().ToString(), Name = "Gimli", Type = PersonType.Actor}}
            };
        }

        public MusicBuilder AddName(string title)
        {
            _song.Name = title;
            return this;
        }

        public MusicBuilder AddPrimaryImage(string image)
        {
            _song.Primary = image;
            return this;
        }

        public MusicBuilder AddPremiereDate(DateTime date)
        {
            _song.PremiereDate = date;
            return this;
        }

        public MusicBuilder AddGenres(params string[] genres)
        {
            _song.Genres = genres;
            return this;
        }

        public MusicBuilder AddPerson(ExtraPerson person)
        {
            var list = _song.People.ToList();
            list.Add(person);
            _song.People = list.ToArray();
            return this;
        }

        public MusicBuilder AddCodec(string codec)
        {
            _song.Codec = codec;
            return this;
        }

        public MusicBuilder AddRunTimeTicks(int hours, int minute, int seconds)
        {
            var ticks = new TimeSpan(hours, minute, seconds);
            _song.RunTimeTicks = ticks.Ticks;
            return this;
        }

        public Song Build()
        {
            return _song;
        }
    }
}
