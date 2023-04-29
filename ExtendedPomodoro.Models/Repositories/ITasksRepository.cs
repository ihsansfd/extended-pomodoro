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
        Task UpdateTaskState(int taskId, int IsTaskCompleted);
        Task UpdateTimeSpent(int taskId, int timeSpentInSeconds);
        Task UpdateActPomodoro(UpdateActPomodoroDTO dto);
        Task<IEnumerable<TaskDTO>> GetTasks(GetTaskDTO data);
        Task DeleteTask(int taskId);
        Task<int> GetTotalRows(int IsTaskCompleted);
    }
}
