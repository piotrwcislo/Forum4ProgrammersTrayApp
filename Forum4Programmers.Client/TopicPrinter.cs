using Forum4Programmers.Client.Model;
using System;

namespace Forum4Programmers.Client
{
    public class TopicPrinter
    {
        private const int MaxSubjectLenForShort = 60;
        public static string PrintShort(Topic topic)
        {
            string subject = topic.Subject.Length > MaxSubjectLenForShort
                ? $"{topic.Subject.Substring(0, MaxSubjectLenForShort-3)}..."
                : string.Format("{0}", topic.Subject);
            return $"{subject} ({GetTimeAgo(topic)})";
        }

        public static string Print(Topic topic)
        {
            return $@"{topic.Subject} - {GetLastPostDateTimeFormatted(topic)} ({GetTimeAgo(topic)})
{topic.Url}
";
        }

        private static string GetLastPostDateTimeFormatted(Topic topic)
        {
            return $"{topic.LastPostCreatedAt.ToLongDateString()} {topic.LastPostCreatedAt.Hour:00}:{topic.LastPostCreatedAt.Minute:00}";
        }

        private static object GetTimeAgo(Topic topic)
        {
            TimeSpan timeDiff = DateTime.Now - topic.LastPostCreatedAt;
            if (timeDiff.TotalDays >= 2)
            {
                return $"{Math.Round(timeDiff.TotalDays)} dni temu";
            }

            if (timeDiff.TotalDays >= 1)
            {
                return $"{Math.Floor(timeDiff.TotalDays)} dzień {Math.Floor(timeDiff.TotalHours - 24)} godz. temu";
            }

            if (timeDiff.TotalHours >= 2)
            {
                return $"{Math.Round(timeDiff.TotalHours)} godz. temu";
            }

            if (timeDiff.TotalMinutes > 1)
            {
                return $"{Math.Round(timeDiff.TotalMinutes)} min. temu";
            }

            return $"{Math.Round(timeDiff.TotalSeconds)} sek. temu";
        }
    }
}
