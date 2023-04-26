using ExtendedPomodoro.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Entities
{
    public record class FullScreenStatsDialogMessage(StatAxesDomainViewModel Axes, 
        bool OpenRequested = true);
}
