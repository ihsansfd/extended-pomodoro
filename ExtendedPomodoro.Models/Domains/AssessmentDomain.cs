using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Models.Domains;

public class EstPomodoroAssessmentDomain
{
    public int CorrectEstimationCount { get; set; }
    public int OverEstimationCount { get; set; }
    public int UnderEstimationCount { get; set; }

    public double GenerateSuccessRate()
    {
        int totalCount = CorrectEstimationCount + OverEstimationCount + UnderEstimationCount;

        if (totalCount <= 0) return 1;

        return Math.Min((double)CorrectEstimationCount / (double)totalCount, 1);
    }
}

public class TaskCompletionAssessmentDomain
{
    public int UncompletedTasksCount { get; set; }
    public int CompletedTasksCount { get; set; }

    public double GenerateSuccessRate() {
        
        int totalCount = UncompletedTasksCount + CompletedTasksCount;

        if (totalCount <= 0) return 1;

        return Math.Min((double) CompletedTasksCount / (double) totalCount, 1);
    }
}

public class TaskPunctualityAssessmentDomain
{
    public int TaskCompletedOnTimeCount { get; set; }
    public int TaskCompletedLateCount { get; set; }

    public double GenerateSuccessRate()
    {
        int totalCount = TaskCompletedOnTimeCount + TaskCompletedLateCount;

        if (totalCount <= 0) return 1;

        return Math.Min((double)TaskCompletedOnTimeCount / (double)totalCount, 1);
    }
}

public class DailyPomodoroTargetAssessmentDomain
{
    public int ReachedCount { get; set; }
    public int UnreachedCount { get; set; }

    public double GenerateSuccessRate()
    {
        int totalCount = ReachedCount + UnreachedCount;

        if (totalCount <= 0) return 1;

        return Math.Min((double)ReachedCount / (double)totalCount, 1);
    }
}