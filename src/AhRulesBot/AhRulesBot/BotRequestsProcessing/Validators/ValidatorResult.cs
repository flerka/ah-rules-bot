
namespace AhRulesBot.BotRequestsProcessing.Validators
{
    internal class ValidatorResult
    {
        internal ValidatorResult(bool valid, string message)
        {
            IsValid = valid;
            ValidationMessage = message;
        }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
    }

    internal class InvalidResult : ValidatorResult
    {
        public InvalidResult() : base(false, null) { }
        public InvalidResult(string text) : base(false, text) { }
    }

    internal class ValidResult : ValidatorResult
    {
        public ValidResult() : base(true, null) { }
    }
}
