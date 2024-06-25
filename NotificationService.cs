using System;

namespace SampleAppRateLimited.Services.Application.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly Gateway gateway;

        public NotificationService(Gateway gateway)
        {
            this.gateway = gateway;
        }

        public bool Send(string type, string userId, string message)
        {
            var succesfullySent = false;
            try
            {
                succesfullySent = true;
                this.gateway.Send(userId, message);
            }
            catch (Exception ex)
            {
                // Catching the error and handle it in the current method
            }
            finally
            {
                // close all the sensitive channels and instances
            }
            return succesfullySent;
        }
    }
}


