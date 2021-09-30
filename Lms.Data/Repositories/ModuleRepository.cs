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
    public class ModuleRepository : IModuleRepository
    {
        private readonly LmsApiContext db;

        public ModuleRepository(LmsApiContext context)
        {
            this.db = context;
        }

         async Task IModuleRepository.AddAsync(Module module)
        {
            await db.Module.AddAsync(module);
        }

         async Task<bool> IModuleRepository.AnyAsync(string title)
        {
            return await db.Module.AnyAsync(e => e.Title == title);
        }

         async Task<Module> IModuleRepository.FindAsync(int? id)
        {
            return await db.Module.FirstAsync(e => e.Id == id);
        }

        async Task<Module> IModuleRepository.FindAsync(string title)
        {
            return await db.Module.FirstAsync(e => e.Title == title);
        }

        async Task<IEnumerable<Module>> IModuleRepository.GetAllModules()
        {
            var query = db.Module.AsQueryable();
            return await query.OrderBy(m=>m.Title).ToListAsync();
        }

         async Task<Module> IModuleRepository.GetModule(int? id)
        {
            var query = db.Module.AsQueryable();
            return await query.FirstOrDefaultAsync(m => m.Id == id);
        }

        async Task<Module> IModuleRepository.GetModule(string title)
        {
            var query = db.Module.AsQueryable();
            return await query.FirstOrDefaultAsync(m => m.Title == title);
        }

        void IModuleRepository.Remove(Module module)
        {
            db.Remove(module);
        }

          void IModuleRepository.Update(Module module)
        {
            db.Entry(module).State = EntityState.Modified;
        }
        public async Task<IEnumerable<Module>> GetModuleForCourse(string title)
        {
            return await db.Module.Where(l => l.Course.Title == title).OrderBy(m => m.Title).ToListAsync();
        }
    }
}
