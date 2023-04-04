using ExtendedPomodoro.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Models.Repositories
{
    public interface ITasksRepository
    {
        Task CreateTask(CreateTaskDomain task);
        IAsyncEnumerable<TaskDomain> GetTasks(int limit = 20);
    }
}
