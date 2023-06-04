using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Models.Repositories
{
    public interface ITasksRepository
    {
        Task CreateTask(CreateTaskDTO dto);
        Task UpdateTask(UpdateTaskDTO dto);
        Task UpdateTaskState(UpdateTaskStateDTO dto);
        Task UpdateTimeSpent(UpdateTimeSpentDTO dto);
        Task UpdateActPomodoro(UpdateActPomodoroDTO dto);
        Task<IEnumerable<TaskDTO>> GetTasks(GetTaskDTO data);
        Task<IEnumerable<TaskDTO>> GetTasks(DateTime from, DateTime to);
        Task DeleteTask(int taskId);
        Task<int> GetTotalRows(int IsTaskCompleted);
    }
}
