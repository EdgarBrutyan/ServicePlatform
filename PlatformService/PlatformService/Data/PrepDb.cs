using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation( IApplicationBuilder app, bool isProd)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AddDbContext>(), isProd);
            }
        }

        private static void SeedData(AddDbContext db, bool isProd)
        {
            if(isProd)
            {
                Console.WriteLine("--> Attepmting to apply migration...");

                try
                {
                    db.Database.Migrate();
                }
                catch(Exception ex) 
                {
                    Console.WriteLine($"Could not run teh migration {ex.Message}");
                }
            }

            if (!db.Platforms.Any())
            {
                Console.WriteLine("Seeding data ...");

                db.Platforms.AddRange(
                    
                   new Platform() {Name ="Dotnet", Publisher = "Microsoft", Cost= "Free"},
                   new Platform() { Name = "SQL Server", Publisher = "Microsoft", Cost = "Free" },
                   new Platform() { Name = "Visual Studio", Publisher = "Microsoft", Cost = "Free" }
                );

                db.SaveChanges();
            }

            else
            {
                Console.WriteLine("There is a data in Database");
            }
        }
    }
}
