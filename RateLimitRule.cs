namespace SampleAppRateLimited.Services.Application.NotificationService
{
    public class RateLimitRule
    {
        public string RuleName { get; set; }
        public string NotificationType { get; set; }
        public int Limit { get; set; }
        public string UnitOfTime { get; set; }
    }
}


