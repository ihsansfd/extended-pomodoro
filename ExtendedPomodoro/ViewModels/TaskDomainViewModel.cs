using ExtendedPomodoro.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.ViewModels
{
    public record class TaskDomainViewModel(
        int Id,
        string Name,
        string? Description,
        int? EstPomodoro,
        int ActPomodoro,
        string CreatedAt,
        int TaskStatus,
        int TimeSpentInMinutes
        );
}
