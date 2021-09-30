using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lms.Data.Data;
using Lms.Core.Entities;
using AutoMapper;
using Lms.Core.Repositories;
using Lms.Core.Dtos;
using Lms.Data.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Lms.Core.Models;

namespace Lms.Api.Controllers
{
    [Route("api/courses")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork uow;

        public CoursesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.mapper = mapper;
            uow = unitOfWork;
        }

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<PagedCollectionResponse<Course>>> GetCourses(bool includedModules,[FromQuery] SampleFilterModel filter)
        {
            var courses = await uow.CourseRepository.GetAllCourses(includedModules);

            var dto = mapper.Map<IEnumerable<CourseDto>>(courses);

            //////////////////////////////////////////////////////////////////////////////////////
            //Filtering logic  
            Func<SampleFilterModel, IEnumerable<CourseDto>> filterData = (filterModel) =>
            {
                return dto.Where(c => c.Title.StartsWith(filterModel.Term ?? String.Empty, StringComparison.InvariantCultureIgnoreCase))
                .Skip((filterModel.Page - 1) * filter.Limit)
                .Take(filterModel.Limit);
            };

            //Get the data for the current page  
            var result = new PagedCollectionResponse<CourseDto>();
            result.Items = filterData(filter);

            //Get next page URL string  
            SampleFilterModel nextFilter = filter.Clone() as SampleFilterModel;
            nextFilter.Page += 1;
            String nextUrl = filterData(nextFilter).Count() <= 0 ? null : this.Url.Action("GetCourses", null,new { includedModules= includedModules, Term = nextFilter.Term , Page = nextFilter.Page ,Limit = nextFilter.Limit}, Request.Scheme);

            //Get previous page URL string  
            SampleFilterModel previousFilter = filter.Clone() as SampleFilterModel;
            previousFilter.Page -= 1;
            String previousUrl = previousFilter.Page <= 0 ? null : this.Url.Action("GetCourses", null, new { includedModules=includedModules,Term = previousFilter.Term, Page = previousFilter.Page, Limit = previousFilter.Limit}, Request.Scheme);

            result.NextPage = !String.IsNullOrWhiteSpace(nextUrl) ? new Uri(nextUrl) : null;
            result.PreviousPage = !String.IsNullOrWhiteSpace(previousUrl) ? new Uri(previousUrl) : null;

        
            
            return Ok(result);
   
        }

        // GET: api/Courses/5
        [HttpGet("GetCourses/{id}")]
        public async Task<ActionResult<Course>> GetCourse(int? id, bool includedModules)
        {
            if (id is null) return BadRequest();
            var course = await uow.CourseRepository.GetCourse(id, includedModules);

            if (course == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<CourseDto>(course);

            return Ok(dto);
        }


        [HttpGet("GetCourse/{title}")]
        public async Task<ActionResult<Course>> GetCourse(string title, bool includedModules)
        {
            if (string.IsNullOrWhiteSpace(title)) return BadRequest();

            var course = await uow.CourseRepository.GetCourse(title, includedModules);

            if (course == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<CourseDto>(course);

            return Ok(dto);
        }

        // PUT: api/Courses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(int? id, CourseDto courseDto)
        {
            if (id is null) return BadRequest();
            if (courseDto == null)
            {
                return NotFound();
            }
            var course = await uow.CourseRepository.FindAsync(id);


            if (course is null) return StatusCode(StatusCodes.Status404NotFound);

            mapper.Map(courseDto, course);

            if (await uow.CompleteAsync())
            {
                return Ok(mapper.Map<CourseDto>(course));
            }
            else
            {
                return StatusCode(500);
            }
        }

        // POST: api/Courses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Course>> PostCourse(CourseDto courseDto)
        {
            if (courseDto == null)
            {
                return NotFound();
            }
            if (await uow.CourseRepository.AnyAsync(courseDto.Title))
            {
                ModelState.AddModelError("Title", "Title is in use");
                return BadRequest(ModelState);
            }

            var course = mapper.Map<Course>(courseDto);
            await uow.CourseRepository.AddAsync(course);

            if (await uow.CompleteAsync())
            {
                var model = mapper.Map<CourseDto>(course);
                return CreatedAtAction(nameof(GetCourse), new { title = model.Title }, model);
            }
            else
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int? id)
        {
            if (id is null) return BadRequest();
            var course = await uow.CourseRepository.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            uow.CourseRepository.Remove(course);
            await uow.CompleteAsync();

            return NoContent();
        }

        [HttpPatch("{title}")]
        public async Task<ActionResult<CourseDto>> PatchCourse(string title, JsonPatchDocument<CourseDto> patchDocument)
        {
            if (string.IsNullOrWhiteSpace(title)) return BadRequest(); 

            var course = await uow.CourseRepository.FindAsync(title);

            if (course is null) return NotFound();

            var courseDto = mapper.Map<CourseDto>(course);

            patchDocument.ApplyTo(courseDto, ModelState);

            if (!TryValidateModel(courseDto)) return BadRequest(ModelState);

            mapper.Map(courseDto, course);

            if (await uow.CompleteAsync())
                return Ok(mapper.Map<CourseDto>(course));
            else
                return StatusCode(500);
        }
    }
}
