using AhRulesBot.BotRequestsProcessing.Interfaces;

namespace AhRulesBot.CustomRulesImport
{
    internal class RulesDriveService : Google.Apis.Drive.v3.DriveService, IDriveService
    {
        public RulesDriveService(Initializer initializer) : base(initializer) { }
    }
}
