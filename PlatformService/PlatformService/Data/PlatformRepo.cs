using Microsoft.OpenApi.Models;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepo : IPlatformRepo
    {
        private AddDbContext _context;

        public PlatformRepo(AddDbContext context)
        {
            _context = context;
        }

        public void CreatePlatform(Platform plat)
        {
            if (plat == null) 
            {
                throw new ArgumentNullException(nameof(plat));
            }

            _context.Add(plat);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _context.Platforms.ToList();
        }

        public Platform GetPlatformById(int id)
        {
            return _context.Platforms.FirstOrDefault(p => p.Id == id);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
