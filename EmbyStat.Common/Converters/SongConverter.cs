using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;

namespace EmbyStat.Common.Converters
{
    public static class SongConverter
    {
        public static Song ConvertToSong(BaseItemDto x, string collectionId)
        {
            return new Song
            {
                Id = Convert.ToInt32(x.Id),
                CollectionId = collectionId,
                Name = x.Name,
                ParentId = x.ParentId,
                OriginalTitle = x.OriginalTitle,
                DateCreated = x.DateCreated,
                Path = x.Path,
                SortName = x.SortName,
                Codec = x.Container,
                RunTimeTicks = x.RunTimeTicks,
                MediaType = x.MediaType,
                PremiereDate = x.PremiereDate,
                ProductionYear = x.ProductionYear,
                Primary = x.ImageTags.FirstOrDefault(y => y.Key == ImageType.Primary).Value,
                Thumb = x.ImageTags.FirstOrDefault(y => y.Key == ImageType.Thumb).Value,
                Logo = x.ImageTags.FirstOrDefault(y => y.Key == ImageType.Logo).Value,
                Banner = x.ImageTags.FirstOrDefault(y => y.Key == ImageType.Banner).Value,
                Genres = x.Genres,
                People = x.People.Select(y => new ExtraPerson
                {
                    Id = y.Id,
                    Name = y.Name,
                    Type = y.Type
                }).ToArray()
    };
        }
    }
}
