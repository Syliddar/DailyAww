using RedditSharp.Things;

namespace DailyAww.Interfaces
{
    public interface IAwwService
    {
        string GetAwws(FromTime fromTime);
    }
}