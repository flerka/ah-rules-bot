using Google.Apis.Drive.v3;

namespace AhRulesBot.BotRequestsProcessing.Interfaces
{
    internal interface IDriveService
    {
        public FilesResource Files { get; }
    }
}