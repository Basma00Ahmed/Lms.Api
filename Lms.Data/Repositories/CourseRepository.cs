using Lms.Core.Dtos;
using Lms.Core.Entities;
using Lms.Core.Repositories;
using Lms.Data.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lms.Data.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly LmsApiContext db;

        public CourseRepository(LmsApiContext context)
        {
            this.db = context;
        }

         async Task ICourseRepository.AddAsync(Course course)
        {
           await db.Course.AddAsync(course);
        }

         async Task<bool> ICourseRepository.AnyAsync(string title)
        {

            return await db.Course.AnyAsync(e => e.Title == title);
        }

        async Task<Course> ICourseRepository.FindAsync(int? id)
        {
            return await db.Course.FirstAsync(e => e.Id == id);
        }

        async Task<Course> ICourseRepository.FindAsync(string title)
        {
            return await db.Course.FirstAsync(e => e.Title == title);
        }

        async Task<IEnumerable<Course>> ICourseRepository.GetAllCourses(bool includedModules)
        {
            var query = db.Course.AsQueryable();
            if (includedModules)
                query = query.Include(m=>m.Modules);
            return await query.OrderBy(c => c.Title).ToListAsync();
        }

        async Task<Course> ICourseRepository.GetCourse(int? id, bool includedModules)
        {
            var query = db.Course.AsQueryable();
            if (includedModules)
                query = query.Include(m => m.Modules);
            return await query.FirstOrDefaultAsync(m=>m.Id==id);
        }

        async Task<Course> ICourseRepository.GetCourse(string title, bool includedModules)
        {
            var query = db.Course.AsQueryable();
            if (includedModules)
                query = query.Include(m => m.Modules);
            return await query.FirstOrDefaultAsync(m => m.Title == title);
        }

        void ICourseRepository.Remove(Course course)
        {
            db.Remove(course);
        }

        void ICourseRepository.Update(Course course)
        {
            db.Entry(course).State = EntityState.Modified;
        }
    }
}
