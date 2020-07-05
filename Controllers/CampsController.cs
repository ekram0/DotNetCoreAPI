using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CampsController : Controller
    {
        public IMapper Mapper { get; }
        private readonly ICampRepository campRepository;
        private readonly LinkGenerator linkGenerator;

        public ICampRepository CampRepository => campRepository;

        public CampsController(ICampRepository campRepository, 
            IMapper mapper, LinkGenerator linkGenerator)
        {
            this.campRepository = campRepository;
            Mapper = mapper;
            this.linkGenerator = linkGenerator;
        }


        [HttpGet]
        public async Task<ActionResult<CampModel[]>> GetCampsAsync(bool includeTalks = false)
        {
            try
            {
                var result = await campRepository.GetAllCampsAsync(includeTalks);

                //CampModel[] models = Mapper.Map<CampModel[]>(result);
                return Mapper.Map<CampModel[]>(result);
            }
            catch (Exception)
            {
                return StatusCode(statusCode: 500, "DataBase Failed");
            }
        }

        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModel>> GetAsync(string moniker)
        {
            try
            {
                var result = await campRepository.GetCampAsync(moniker);

                if (result is null) return NotFound();

                return Mapper.Map<CampModel>(result);
            }
            catch (Exception)
            {
                return StatusCode(500, "DataBase failed");
            }
        }

        [HttpGet]
        public async Task<ActionResult<CampModel[]>> searchByDate(DateTime theDate, bool includeTalks = false)
        {
            try
            {
                var result = await campRepository.GetAllCampsByEventDate(theDate, includeTalks);
                if (!result.Any()) return NotFound();

                return Mapper.Map<CampModel[]>(result);

            }
            catch (Exception)
            {
                return StatusCode(500, "DataBase Failed");
            }
        }

        public async Task<ActionResult<CampModel>> Post(CampModel model)
        {
            try
            {
                var campExisting = await CampRepository.GetCampAsync(model.Moniker);
                if (campExisting != null)
                {
                    return BadRequest("Moniker");
                }
                var location = linkGenerator.GetPathByAction("Get", "Camps", new { moniker = model.Moniker });

                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Could not use current moniker");
                }

                var camp = Mapper.Map<CampModel>(model);

                campRepository.Add(camp);

                if (await campRepository.SaveChangesAsync())
                    return Created($"/api/camps/{model.Moniker}", Mapper.Map<CampModel>(camp));

            }
            catch (Exception)
            {
                return StatusCode(500, "DataBase failed");
            }
            return BadRequest();
        }

        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker, CampModel model)
        {
            try
            {
                var oldCamp = await CampRepository.GetCampAsync(moniker);
                if (oldCamp is null) return NotFound($"Could not find camp with moniker {moniker}");

                Mapper.Map(model, oldCamp);

                if (await CampRepository.SaveChangesAsync())
                    return Mapper.Map<CampModel>(oldCamp);

            }
            catch (Exception)
            {
                return StatusCode(500, "DataBase failed");
            }
            return BadRequest();

        }

        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete(string moniker)
        {
            try
            {
                var deleteCamp = await CampRepository.GetCampAsync(moniker);
                if (deleteCamp is null) return NotFound($"Could not find camp {moniker}");

                CampRepository.Delete(deleteCamp);

                if (await CampRepository.SaveChangesAsync())
                    return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "dataBase failed");
            }
            return BadRequest();
        }
    }


}
