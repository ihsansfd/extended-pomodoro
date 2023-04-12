using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Models.DTOs
{
    public class SqliteUpsertDailySessionDTO
    {
        public DateOnly SessionDate { get; set; }
        public int TimeSpentInSeconds {get; set;} 
        public int TotalPomodoroCompleted {get; set;}
        public int TotalShortBreaksCompleted {get; set;}
        public int TotalLongBreaksCompleted {get; set;}
        public DateTime CreatedAt {get; set;}
        public DateTime UpdatedAt {get; set;}
    }

}
