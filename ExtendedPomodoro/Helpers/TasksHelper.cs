﻿using ExtendedPomodoro.Models.Domains;
using System.ComponentModel.DataAnnotations;

namespace ExtendedPomodoro.Helpers
{
    public class TasksHelper
    {
        public static TaskState ConvertIntegerToTaskState(int taskStateInInteger)
        {
            return taskStateInInteger == 1 ? TaskState.COMPLETED : TaskState.IN_PROGRESS;
        }

        public static int ConvertTaskStateToInteger(TaskState taskState)
        {
            return taskState == TaskState.COMPLETED ? 1 : 0;
        }

        public static string ConvertIntegerToTaskStateString(int taskStateInInteger)
        {
            return taskStateInInteger == 1 ? "Completed" : "In Progress";
        }

        public static ValidationResult ValidateEstPomodoro(string? estPomodoro, ValidationContext context)
        {
            return (string.IsNullOrWhiteSpace(estPomodoro) || int.TryParse(estPomodoro, out _) ?
                ValidationResult.Success : new("Est. Pomodoro must be an integer"))!;
        }
    }
}
