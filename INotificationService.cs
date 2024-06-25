namespace SampleAppRateLimited.Services.Application.NotificationService
{
    public interface INotificationService
    {
        public bool Send(string type, string userId, string message);
    }
}


