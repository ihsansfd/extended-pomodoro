using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Models.DbSetup
{
    public interface IDatabaseSetup
    {
        Task Setup();
    }
}
