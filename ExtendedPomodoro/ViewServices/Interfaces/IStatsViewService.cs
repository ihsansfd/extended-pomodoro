﻿using ExtendedPomodoro.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.ViewServices.Interfaces
{
    public interface IStatsViewService
    {
        void OpenScatterPlotStats(ChartDataDomainViewModel axes);
    }
}
