using Lms.Core.Dtos;
using Lms.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lms.Core.Repositories
{
    public  interface ICourseRepository
    {

        Task<IEnumerable<Course>> GetAllCourses(bool includedModules);
        Task<Course> GetCourse(int? id, bool includedModules);
        Task<Course> GetCourse(string title, bool includedModules);
        Task<Course> FindAsync(int? id);
        Task<Course> FindAsync(string title);
        Task<bool> AnyAsync(string title);
        Task AddAsync(Course course);
        void Update(Course course);
        void Remove(Course course);
    }
}