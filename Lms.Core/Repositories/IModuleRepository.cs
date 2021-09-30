using Lms.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lms.Core.Repositories
{
    public  interface IModuleRepository
    {

        Task<IEnumerable<Module>> GetAllModules();
        Task<Module> GetModule(int? id);
        Task<Module> GetModule(string title);
        Task<Module> FindAsync(int? id);
        Task<Module> FindAsync(string title);
        Task<bool> AnyAsync(string title);
        Task<IEnumerable<Module>> GetModuleForCourse(string title);
        Task AddAsync(Module module);
        void Update(Module module);
        void Remove(Module module);

    }
}