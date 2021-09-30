using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lms.Data;
using Lms.Core.Entities;
using AutoMapper;
using Lms.Core.Repositories;
using Lms.Core.Dtos;
using Microsoft.AspNetCore.JsonPatch;
using Lms.Core.Models;

namespace Lms.Api.Controllers
{
    [Route("api/modules")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork uow;

        public ModulesController( IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.mapper = mapper;
            uow = unitOfWork;
        }

        // GET: api/Modules
        [HttpGet]
        public async Task<ActionResult<PagedCollectionResponse<Module>>> GetModules([FromQuery] SampleFilterModel filter)
        {
            var modules = await uow.ModuleRepository.GetAllModules();
            var dto=mapper.Map<IEnumerable<ModuleDto>>(modules);

            //////////////////////////////////////////////////////////////

            Func<SampleFilterModel, IEnumerable<ModuleDto>> filterData = (filterModel) =>
            {
                return dto.Where(m => m.Title.StartsWith(filterModel.Term ?? String.Empty, StringComparison.InvariantCultureIgnoreCase))
                .Skip((filterModel.Page - 1) * filter.Limit)
                .Take(filterModel.Limit);
            };

            //Get the data for the current page  
            var result = new PagedCollectionResponse<ModuleDto>();
            result.Items = filterData(filter);

            //Get next page URL string  
            SampleFilterModel nextFilter = filter.Clone() as SampleFilterModel;
            nextFilter.Page += 1;
            String nextUrl = filterData(nextFilter).Count() <= 0 ? null : this.Url.Action("GetModules", null, nextFilter, Request.Scheme);

            //Get previous page URL string  
            SampleFilterModel previousFilter = filter.Clone() as SampleFilterModel;
            previousFilter.Page -= 1;
            String previousUrl = previousFilter.Page <= 0 ? null : this.Url.Action("GetModules", null, previousFilter, Request.Scheme);

            result.NextPage = !String.IsNullOrWhiteSpace(nextUrl) ? new Uri(nextUrl) : null;
            result.PreviousPage = !String.IsNullOrWhiteSpace(previousUrl) ? new Uri(previousUrl) : null;



            return Ok(result);
        }

        // GET: api/Modules/5
        [HttpGet("GetModule/{id}")]
        public async Task<ActionResult<Module>> GetModule(int? id)
        {
            if (id is null) return BadRequest();
            var module = await uow.ModuleRepository.GetModule(id);

            if (module == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<ModuleDto>(module);

            return Ok(dto);
        }

        [HttpGet("GetModuleWithTitle/{title}")]
  
        public async Task<ActionResult<Module>> GetModuleWithTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return BadRequest();
            var module = await uow.ModuleRepository.GetModule(title);

            if (module == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<ModuleDto>(module);

            return Ok(dto);
        }

        [HttpGet("GetModuleForCourse/{title}")]

        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModuleForCourse(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return BadRequest();

            var modules = await uow.ModuleRepository.GetModuleForCourse(title);
            return Ok(mapper.Map<IEnumerable<ModuleDto>>(modules));
        }

        // PUT: api/Modules/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutModule(int? id, ModuleDto moduleDto)
        {

            if (id is null) return BadRequest();
            if (moduleDto == null)
            {
                return NotFound();
            }
            var module = await uow.ModuleRepository.FindAsync(id);


            if (module is null) return StatusCode(StatusCodes.Status404NotFound);

            mapper.Map(moduleDto, module);

            if (await uow.CompleteAsync())
            {
                return Ok(mapper.Map<ModuleDto>(module));
            }
            else
            {
                return StatusCode(500);
            }
        }

        // POST: api/Modules
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Module>> PostModule(string title,ModuleDto moduleDto)
        {
            if (string.IsNullOrWhiteSpace(title)) return BadRequest();

            var course = await uow.CourseRepository.FindAsync(title);

            if (course is null) return NotFound();

            if (moduleDto == null)
            {
                return NotFound();
            }
            if (await uow.ModuleRepository.AnyAsync(moduleDto.Title))
            {
                ModelState.AddModelError("Title", "Title is in use");
                return BadRequest(ModelState);
            }

            var module = mapper.Map<Module>(moduleDto);
          
            module.Course = course;
            await uow.ModuleRepository.AddAsync(module);

            if (await uow.CompleteAsync())
            {
                var model = mapper.Map<ModuleDto>(module);
                return CreatedAtAction(nameof(GetModuleWithTitle), new { title = model.Title }, model);
            }
            else
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        // DELETE: api/Modules/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModule(int? id)
        {
            if (id is null) return BadRequest();
            var model = await uow.ModuleRepository.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            uow.ModuleRepository.Remove(model);
            await uow.CompleteAsync();

            return NoContent();
        }

        [HttpPatch("{title}")]
        public async Task<ActionResult<ModuleDto>> PatchModule(string title, JsonPatchDocument<ModuleDto> patchDocument)
        {
            if (string.IsNullOrWhiteSpace(title)) return BadRequest();

            var module = await uow.ModuleRepository.FindAsync(title);

            if (module is null) return NotFound();

            var moduleDto = mapper.Map<ModuleDto>(module);

            patchDocument.ApplyTo(moduleDto, ModelState);

            if (!TryValidateModel(moduleDto)) return BadRequest(ModelState);

            mapper.Map(moduleDto, module);

            if (await uow.CompleteAsync())
                return Ok(mapper.Map<ModuleDto>(module));
            else
                return StatusCode(500);
        }
    }
}
