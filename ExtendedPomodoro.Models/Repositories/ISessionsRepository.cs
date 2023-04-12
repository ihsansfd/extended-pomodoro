using ExtendedPomodoro.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Models.Repositories
{
    public interface ISessionsRepository
    {
        Task UpsertDailySession(UpsertDailySessionDomain domain);
        Task UpsertDailySessionTaskLink(DailySessionTaskLinkDomain domain);
    }
}
