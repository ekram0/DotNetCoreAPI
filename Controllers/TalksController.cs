using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [ApiController]
    //[ApiVersion("2.0")]
    [Route("api/camps/{moniker}/talks")]
    public class TalksController : Controller
    {
        public IMapper Mapper { get; }
        private readonly ICampRepository campRepository;
        private readonly LinkGenerator linkGenerator;

        public TalksController(ICampRepository campRepository,
            IMapper mapper, LinkGenerator linkGenerator)
        {
            this.campRepository = campRepository;
            Mapper = mapper;
            this.linkGenerator = linkGenerator;
        }


        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> Get(string moniker)
        {
            try
            {
                var talks = await campRepository.GetTalksByMonikerAsync(moniker, true);
                if(talks is null) return NotFound("No talks are existing");

                return Mapper.Map<TalkModel[]>(talks);
            }

            catch (Exception)
            {
                return StatusCode(500, "Bad DataBase");
            }
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<TalkModel>> Get(string moniker, int id)
        {
            try
            {
                var talks = await campRepository.GetTalkByMonikerAsync(moniker, id);
                if (talks is null) return NotFound("No talk is found");

                return Mapper.Map<TalkModel>(talks);
            }
            catch (Exception)
            {
                return StatusCode(500, "Bad DataBase");
            }
        }


        [HttpGet("{id:int}")]
        [MapToApiVersion("1.5")]
        public async Task<ActionResult<TalkModel>> Get15(string moniker, int id)
        {
            try
            {
                var talks = await campRepository.GetTalkByMonikerAsync(moniker, id, true);
                if (talks is null) return NotFound("No talk is found");

                return Mapper.Map<TalkModel>(talks);
            }
            catch (Exception)
            {
                return StatusCode(500, "Bad DataBase");
            }
        }


        [HttpPost]
        public async Task<ActionResult<TalkModel>> Post(string moniker, TalkModel model)
        {
            try
            {
                var camp = campRepository.GetCampAsync(moniker);
                if (camp is null) return NotFound("Camp talk is not found");

                var talk = Mapper.Map<Talk>(model);
                talk.Camp = await camp;

                if (model.Speaker is null) return BadRequest("Speaker ID is required");
                var speaker = await campRepository.GetSpeakerAsync(model.Speaker.SpeakerId);
                if (speaker is null) return BadRequest("not exist speaker");
                talk.Speaker = speaker;

                campRepository.Add(talk);

                if (await campRepository.SaveChangesAsync())
                {
                    var url = linkGenerator.GetPathByAction(HttpContext, "Post", values: new { moniker, id = talk.TalkId });
                    return Created(url, Mapper.Map<TalkModel>(talk));
                }
                else
                {
                    return BadRequest("Bad Request");
                }

            }
            catch (Exception)
            {
                return StatusCode(500, "DataBase Failed");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TalkModel>> Put(string moniker, int id, TalkModel model)
        {
            try
            {
                var talk = await campRepository.GetTalkByMonikerAsync(moniker, id);
                if (talk is null) return NotFound("Couldn't find the talk");

                if (model.Speaker != null)
                {
                    var speaker = await campRepository.GetSpeakerAsync(id);
                    if (speaker != null)
                        talk.Speaker = speaker;
                }

                Mapper.Map(model, talk);
                if (await campRepository.SaveChangesAsync())
                {
                    return Mapper.Map<TalkModel>(talk);
                }
                else
                    return BadRequest(" Failed to update database. ");


            }
            catch (Exception)
            {
                return StatusCode(500, "DataBase failed");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string moniker, int id)
        {
            try
            {
                var talk = await campRepository.GetTalkByMonikerAsync(moniker, id);
                if (talk is null) return NotFound("Failed to find the talk to delete");

                campRepository.Delete(talk);

                if (await campRepository.SaveChangesAsync())
                    return Ok();
                else
                    return BadRequest("Failed to delete talk.");

            }
            catch (Exception)
            {
                return StatusCode(500, "DataBase failed");
            }
        }

         
    }
}
