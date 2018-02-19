using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DailyAww.Models
{
    public class ScheduleViewModel
    {
        public AwwTypes AwwType { get; set; }
        public string CronExpression { get; set; }
        public TimeZoneInfo TimeZoneInfo { get; set; }
    }

    public enum AwwTypes
    {
        HourlyAwws,
        DailyAwws,
        WeeklyAwws,

    }
}