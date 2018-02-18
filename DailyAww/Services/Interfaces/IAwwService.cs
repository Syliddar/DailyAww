using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp;
using RedditSharp.Things;


namespace DailyAww.Interfaces
{
    public interface IAwwService
    {
        string GetDailyAwws();
        string GetHourlyAwws();
        string GetWeeklyAwws();
    }
}
