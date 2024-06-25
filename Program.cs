using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace SampleAppRateLimited.Services.Application.NotificationService
{
    class Program
    {
        static void Main(string[] args)
        {
            PlayRateLimits();
        }

        private static void PlayRateLimits()
        {
            const string Minute = "minute";
            const string Hour = "hour";
            const string Day = "day";
            string notificationType = "";
            string userId = "";
            string notificationMessage = "";
            int minutesOfRule = 0;
            RateLimitRule rateLimitRule;
            IEnumerable<Notification> notificationsByUser;

            List<RateLimitRule> rateLimitRules = SetDefaultRateLimitRules(Minute, Hour, Day);
            List<Notification> notifications = SetSeedOnNotifications();

            while (notificationType != "end")
            {
                try
                {
                    SetNotification(out notificationType, out userId, out notificationMessage);
                    var currentNotificationTime = DateTime.Now;

                    var rateLimitRulesByCurrentNotificationType = (from r in rateLimitRules
                                                                   where r.NotificationType == notificationType
                                                                   select r);

                    if (rateLimitRulesByCurrentNotificationType.Any())
                    {
                        rateLimitRule = rateLimitRulesByCurrentNotificationType.First();
                        minutesOfRule = rateLimitRule.UnitOfTime == Minute ? 1 : rateLimitRule.UnitOfTime == Hour ? 60 : rateLimitRule.UnitOfTime == Day ? 1440 : 0;

                        notificationsByUser = from n in notifications
                                              where n.UserId == userId && n.Type == rateLimitRule.NotificationType
                                              && n.Timestamp >= currentNotificationTime.AddMinutes(-minutesOfRule)
                                              select n;

                        if (notificationsByUser.Count() < rateLimitRule.Limit)
                        {
                            var service = new NotificationService(new Gateway());
                            var successfullySent = service.Send(notificationType, userId, notificationMessage);
                            if (successfullySent)
                            {
                                notifications.Add(new Notification() { Type = notificationType, UserId = userId, Timestamp = currentNotificationTime });
                                notificationsByUser = from n in notifications
                                                      where n.UserId == userId && n.Type == rateLimitRule.NotificationType
                                                      && n.Timestamp >= currentNotificationTime.AddMinutes(-minutesOfRule)
                                                      orderby n.Timestamp descending
                                                      select n;
                                Console.WriteLine("Notification successfully sent");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Request rejected rate limit rule {rateLimitRule.RuleName}");
                        }
                        Console.WriteLine("******************************");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ops!, we got an error. Let's see {ex.Message} ");
                }

            }
        }

        private static List<RateLimitRule> SetDefaultRateLimitRules(string Minute, string Hour, string Day)
        {
            return new List<RateLimitRule>() {
                new RateLimitRule(){ RuleName = "Status: not more than 2 per minute for each recipient", NotificationType="Status", Limit = 2, UnitOfTime = Minute },
                new RateLimitRule(){ RuleName = "News: not more than 1 per day for each recipient", NotificationType="News", Limit = 1, UnitOfTime = Day },
                new RateLimitRule(){ RuleName = "Marketing: not more than 3 per hour for each recipient", NotificationType="Marketing", Limit = 3, UnitOfTime = Hour }
            };
        }

        private static void SetNotification(out string notificationType, out string userId, out string notificationMessage)
        {
            userId = "";
            notificationMessage = "";
            Console.WriteLine("Set values for current notification, type end to finish. ");
            Console.Write("Notification type: ");
            notificationType = Console.ReadLine();
            if (notificationType != "end")
            {
                Console.Write("Notification userId: ");
                userId = Console.ReadLine();
                Console.Write("Notification message: ");
                notificationMessage = Console.ReadLine();
                Console.WriteLine();
            }
        }

        private static List<Notification> SetSeedOnNotifications()
        {
            return new List<Notification>() {
                new Notification(){ Type = "Status", UserId="1", Timestamp = DateTime.Now },
                new Notification(){ Type = "News", UserId="1", Timestamp = DateTime.Now },
                new Notification(){ Type = "Marketing", UserId="1", Timestamp = DateTime.Now },
                new Notification(){ Type = "Status", UserId="1", Timestamp = DateTime.Now }
            };
        }
    }
}
