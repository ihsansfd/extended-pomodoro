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
            var correctEstimationCount = 0;
            var overEstimationCount = 0;
            var underEstimationCount = 0;

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
            var completedTasksCount = 0;
            var uncompletedTasksCount = 0;

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
            var tasksCompletedOnTimeCount = 0;
            var tasksCompletedLateCount = 0;

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
            var dailyPomodoroTargetReachedCount = 0;
            var dailyPomodoroTargetUnreachedCount = 0;

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
