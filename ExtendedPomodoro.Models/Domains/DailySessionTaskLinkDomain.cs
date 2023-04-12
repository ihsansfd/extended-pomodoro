using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Models.Domains { 

    public record class DailySessionTaskLinkDomain(DateOnly SessionDate, int TaskId, bool IsTaskCompletedInThisSession);
}
