using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.Services;
using ExtendedPomodoro.Services.Entities;

namespace ExtendedPomodoro.Services
{
    public enum AssessmentResult
    {
        SUCCESS = 0,
        WARNING = 1,
        FAILURE = 2
    }

    public class AssessmentMessagesService
    {
        private static readonly double SuccessTolerance = 1.0;
        private static readonly double WarningTolerance = 0.7;

        public static IEnumerable<AssessmentMessage> GenerateMessages(
            IEnumerable<TaskDomain> taskDomains, IEnumerable<DailySessionDomain> sessionDomains)
        {
            List<AssessmentMessage> messages = new();

            var assessmentService = new AssessmentService(taskDomains, sessionDomains);

            messages.Add(GenerateEstPomodoroAssessmentMessage(assessmentService));
            messages.Add(GenerateTaskPunctualityAssessmentMessage(assessmentService));
            messages.Add(GenerateTaskCompletionAssessmentMessage(assessmentService));
            messages.Add(GenerateDailyPomodoroTargetAssessmentMessage(assessmentService));

            return messages;
        }

        private static AssessmentMessage GenerateEstPomodoroAssessmentMessage(AssessmentService assessmentService)
        {
            var assessment = assessmentService.GenerateEstPomodoroAssessment();

            var assessmentResult = GetAssessmentResult(assessment.GenerateSuccessRate());

            string shortMessage;

            string description;

            string? suggestion = null;

            if (assessmentResult == AssessmentResult.SUCCESS)
            {
                shortMessage = "All pomodoros are estimated correctly.";
                description = $"All {assessment.CorrectEstimationCount} est. pomodoros are estimated correctly!";
            }

            else
            {
                shortMessage = $"{assessment.GenerateSuccessRate():P} correct est. pomodoro(s)";
                description = string.Format(
                    "You have {0} correct estimation, {1} overestimation, " +
                    "and {2} underestimation of your est. pomodoro(s)", 
                    assessment.CorrectEstimationCount, 
                    assessment.OverEstimationCount, 
                    assessment.UnderEstimationCount);

                suggestion = "Underestimation can be reduced by adding more subtasks to your task. " +
                             "Overestimation can be reduced by dividing your task into several smaller tasks with smaller est. pomodoro!";
            }

            return new AssessmentMessage()
            {
                Result = assessmentResult,
                ShortMessage = shortMessage,
                Description = description,
                Suggestion = suggestion
            };
        }

        private static AssessmentMessage GenerateTaskPunctualityAssessmentMessage(AssessmentService assessmentService)
        {
            var assessment = assessmentService.GenerateTaskPunctualityAssessment();

            var assessmentResult = GetAssessmentResult(assessment.GenerateSuccessRate());

            string shortMessage;

            string description;

            string? suggestion = null;

            if (assessmentResult == AssessmentResult.SUCCESS)
            {
                shortMessage = "All tasks are completed in time.";
                description = $"All {assessment.TaskCompletedOnTimeCount} tasks are completed the same day they're created!";

            }

            else
            {
                shortMessage = $"{assessment.GenerateSuccessRate():P} tasks are completed in time.";
                description =
                    string.Format("You have {0} tasks that are completed in-time, and {1} that are completed late. " +
                                  "An in-time completed task is the one being completed the same day it's created.",
                                  assessment.TaskCompletedOnTimeCount, assessment.TaskCompletedLateCount);

                suggestion = "Try to create tasks that you only are able to finish on that day. " +
                             "You can always add more tasks tomorrow!";

            }

            return new AssessmentMessage()
            {
                Result = assessmentResult,
                ShortMessage = shortMessage,
                Description = description,
                Suggestion = suggestion
            };
        }

        private static AssessmentMessage GenerateTaskCompletionAssessmentMessage(AssessmentService assessmentService)
        {
            var assessment = assessmentService.GenerateTaskCompletionAssessment();

            var assessmentResult = GetAssessmentResult(assessment.GenerateSuccessRate());

            string shortMessage;

            string description;

            string? suggestion = null;

            if (assessmentResult == AssessmentResult.SUCCESS)
            {
                shortMessage = "All tasks are completed.";
                description = $"All {assessment.CompletedTasksCount} tasks are completed!";
            }

            else
            {
                shortMessage = $"{assessment.GenerateSuccessRate():P} tasks are completed.";
                description = string.Format("You have {0} completed tasks and {1} uncompleted tasks.",
                    assessment.CompletedTasksCount, assessment.UncompletedTasksCount);

                suggestion =
                    "Try to add tasks little by little. You can always add more tasks after you've completed previous tasks!";
            }

            return new AssessmentMessage()
            {
                Result = assessmentResult,
                ShortMessage = shortMessage,
                Description = description,
                Suggestion = suggestion
            };
        }

        private static AssessmentMessage GenerateDailyPomodoroTargetAssessmentMessage(
            AssessmentService assessmentService)
        {
            var assessment = assessmentService.GenerateDailyPomodoroTargetAssessment();

            var assessmentResult = GetAssessmentResult(assessment.GenerateSuccessRate());

            string shortMessage;

            string description;

            string? suggestion = null;

            if (assessmentResult == AssessmentResult.SUCCESS)
            {
                shortMessage = "All daily pomodoro targets are reached";
                description = $"All {assessment.ReachedCount} daily pomodoro targets are reached!";
            }

            else
            {
                shortMessage = $"{assessment.GenerateSuccessRate():P} daily pomodoro targets are reached";
                description = string.Format("You have {0} daily pomodoro targets that are reached and {1} daily pomodoro targets that are unreached", 
                    assessment.ReachedCount, assessment.UnreachedCount);
                suggestion =
                    "Try to be more precise on making the daily pomodoro target. Try to allocate more time for your tasks!";
            }

            return new AssessmentMessage()
            {
                Result = assessmentResult,
                ShortMessage = shortMessage,
                Description = description,
                Suggestion = suggestion
            };
        }

        private static AssessmentResult GetAssessmentResult(double successRate)
        {
            if(successRate >= SuccessTolerance) return AssessmentResult.SUCCESS;
            if(successRate >= WarningTolerance) return AssessmentResult.WARNING;
            return AssessmentResult.FAILURE;
        }
    }
}
