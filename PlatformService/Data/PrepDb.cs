using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation( IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AddDbContext>());
            }
        }

        private static void SeedData(AddDbContext db)
        {
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
