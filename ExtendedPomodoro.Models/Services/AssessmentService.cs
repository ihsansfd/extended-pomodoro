using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.Services.Interfaces;

namespace ExtendedPomodoro.Models.Services
{
    public class AssessmentService
    {
        private readonly IEnumerable<TaskDomain> _taskDomains;
        private readonly IEnumerable<DailySessionDomain> _sessionDomains;

        public AssessmentService(IEnumerable<TaskDomain> taskDomains, IEnumerable<DailySessionDomain> sessionDomains)
        {
            _taskDomains = taskDomains;
            _sessionDomains = sessionDomains;
        }

        public EstPomodoroAssessmentDomain GenerateEstPomodoroAssessment()
        {
            int correctEstimationCount = 0;
            int overEstimationCount = 0;
            int underEstimationCount = 0;

             foreach (var record in _taskDomains)
             { 
                 if (record.TaskState != TaskState.COMPLETED) continue;

                if (record.EstPomodoro == record.ActPomodoro)
                {
                    correctEstimationCount++;
                }

                else if (record.EstPomodoro > record.ActPomodoro)
                {
                    overEstimationCount++;
                }

                else
                {
                    underEstimationCount++;
                }
            }
             
            return new EstPomodoroAssessmentDomain()
            {
                CorrectEstimationCount = correctEstimationCount,
                UnderEstimationCount = underEstimationCount,
                OverEstimationCount = overEstimationCount,
            };
        }

        public TaskCompletionAssessmentDomain GenerateTaskCompletionAssessment()
        {
            int completedTasksCount = 0;
            int uncompletedTasksCount = 0;

            foreach (var record in _taskDomains)
            {
                if (record.TaskState == TaskState.COMPLETED) completedTasksCount++;
                else uncompletedTasksCount++;
            }

            return new TaskCompletionAssessmentDomain()
            {
                CompletedTasksCount = completedTasksCount,
                UncompletedTasksCount = uncompletedTasksCount
            };
        }

        public TaskPunctualityAssessmentDomain GenerateTaskPunctualityAssessment()
        {
            int tasksCompletedOnTimeCount = 0;
            int tasksCompletedLateCount = 0;

            foreach (var record in _taskDomains)
            {
                if (record.TaskState != TaskState.COMPLETED) continue;

                var completedAt = (DateTime)record.CompletedAt!;

                if (DateOnly.FromDateTime(record.CreatedAt) == DateOnly.FromDateTime(completedAt))
                {
                    tasksCompletedOnTimeCount++;
                }
                else
                {
                    tasksCompletedLateCount++;
                }
            }

            return new TaskPunctualityAssessmentDomain()
            {
                TaskCompletedOnTimeCount = tasksCompletedOnTimeCount,
                TaskCompletedLateCount = tasksCompletedLateCount
            };
        }

        public DailyPomodoroTargetAssessmentDomain GenerateDailyPomodoroTargetAssessment()
        {
            int dailyPomodoroTargetReachedCount = 0;
            int dailyPomodoroTargetUnreachedCount = 0;

            foreach (var record in _sessionDomains)
            {
                if (record.TotalPomodoroCompleted >= record.DailyPomodoroTarget) dailyPomodoroTargetReachedCount++;
                else dailyPomodoroTargetUnreachedCount++;
            }

            return new DailyPomodoroTargetAssessmentDomain()
            {
                ReachedCount = dailyPomodoroTargetReachedCount,
                UnreachedCount = dailyPomodoroTargetUnreachedCount
            };
        }
    }
}
