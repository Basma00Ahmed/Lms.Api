using Bogus;
using Lms.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lms.Data.Data
{
    public class SeedData
    {
        public static async Task InitializeAcync(IServiceProvider services)
        {
            using var db = new LmsApiContext(services.GetRequiredService<DbContextOptions<LmsApiContext>>());

            if (await db.Course.AnyAsync()) return;

            var faker = new Faker("sv");
            var courses = new List<Course>();

            for (int i = 0; i < 50; i++)
            {
                var date = DateTime.Now.AddDays(faker.Random.Int(-20, 20));
                courses.Add(new Course
                {
                    Title = faker.Company.CompanyName(),
                    StartDate = date,
                    Modules = new Module[]
                    {
                            new Module
                            {
                              Title = faker.Commerce.ProductName(),
                              StartDate = date.AddDays(3)
                          },
                            new Module
                            {
                              Title = faker.Commerce.ProductName(),
                               StartDate = date.AddDays(33)
                            }
                    }
                }); ; ;

            }

            db.AddRange(courses);
            await db.SaveChangesAsync();

        }
    }

}
