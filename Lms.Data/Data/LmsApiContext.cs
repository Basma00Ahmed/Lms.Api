using Lms.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lms.Data.Data
{
    public class LmsApiContext : DbContext
    {
        public LmsApiContext(DbContextOptions<LmsApiContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Course { get; set; }

        public DbSet<Module> Module { get; set; }
    }
}
