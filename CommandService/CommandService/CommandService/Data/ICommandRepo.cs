using CommandService.Models;

namespace CommandService.Data
{
    public interface ICommandRepo
    { 
        // Platform
        bool SaveChanges();
        IEnumerable<Platform> GetAllPlatforms();
        void CreatePlatform(Platform platform);
        bool PlatformExits(int platformId);

        bool ExternalPlatformIdExists(int externalplatformId);
        
        // Command 
        IEnumerable<Command> GetCommandsForPlatform(int platformId);
        Command GetCommand(int platformId, int commandId);
        void CreateCommand(int  platformId, Command command);

    }
}
