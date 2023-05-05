using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Core.Timeout
{
    public delegate void RegisterWaitTimeoutCallback(Action callback, TimeSpan waitFor);

    public class WaitTimeoutProvider
    {
        public static void RegisterWaitTimeout(Action callback, TimeSpan waitFor)
        {
            ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(false),
                (_, _) => callback(), null, waitFor, true);
        }
    }
}
