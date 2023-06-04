using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Services.Entities
{
    public class AssessmentMessage
    {
        public AssessmentResult Result { get; set; }
        public string ShortMessage { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Suggestion { get; set; } = string.Empty;
    }
}
