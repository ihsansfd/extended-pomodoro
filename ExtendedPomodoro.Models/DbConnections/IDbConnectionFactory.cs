using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Models.DbConnections
{
    public interface IDbConnectionFactory
    {
        IDbConnection Connect();
    }
}
