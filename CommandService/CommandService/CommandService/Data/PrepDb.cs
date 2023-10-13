using CommandService.Models;
using CommandService.SyncDataService.Grpc;

namespace CommandService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder applicationBuilder) 
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope()) 
            {
                var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();

                var platforms = grpcClient.ReturnAllPlatforms();

                SeedData(serviceScope.ServiceProvider.GetService<ICommandRepo>(), platforms);
            }
        }

        private static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms) 
        {
            Console.WriteLine("Seeding new platforms ...");
            foreach (var plat in platforms) 
            {
                if (!repo.ExternalPlatformIdExists(plat.ExternalID))
                {
                    repo.CreatePlatform(plat);
                }
                repo.SaveChanges();
            }
        }
    }
}
