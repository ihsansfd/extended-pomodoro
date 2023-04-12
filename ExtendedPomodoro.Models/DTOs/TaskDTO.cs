﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Models.DTOs
{
    public class SqliteTaskDTO
    {
        public int Id {get; set;}
        public string Name {get; set;} = string.Empty;
        public string? Description { get; set; }
        public int? EstPomodoro {get; set;}
        public int ActPomodoro {get; set;}
        public int IsTaskCompleted {get; set;}
        public int TimeSpentInSeconds {get; set;}
        public DateTime CreatedAt {get; set;}
        public DateTime UpdatedAt { get; set; }
    }
}
