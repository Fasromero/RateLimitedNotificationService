using System;

namespace SampleAppRateLimited.Services.Application.NotificationService
{
    public class Gateway
    {
        public bool Send(string userId, string message)
        {
            var succesfullySent = false;
            try
            {
                succesfullySent = true;
                Console.WriteLine($"Sending message {message} to user {userId}");

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


