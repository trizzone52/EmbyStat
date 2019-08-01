﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers.Music
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MusicController : Controller
    {
        private readonly IMusicService _musicService;
        private readonly IMapper _mapper;

        public MusicController(IMusicService musicService, IMapper mapper)
        {
            _musicService = musicService;
            _mapper = mapper;
        }
        
        [HttpGet]
        [Route("collections")]
        public IActionResult GetCollections()
        {
            var result = _musicService.GetMusicCollections();
            return Ok(_mapper.Map<IList<CollectionViewModel>>(result));
        }

        [HttpGet]
        [Route("statistics")]
        public async Task<IActionResult> GetGeneralStats(List<string> collectionIds)
        {
            var result = await _musicService.GetMusicStatisticsAsync(collectionIds);
            var convert = _mapper.Map<MusicStatisticsViewModel>(result);
            return Ok(convert);
        }

        [HttpGet]
        [Route("typepresent")]
        public IActionResult MovieTypeIsPresent()
        {
            return Ok(_musicService.TypeIsPresent());
        }
    }
}
