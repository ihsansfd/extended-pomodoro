using System.Data;

namespace ExtendedPomodoro.Models.DbConnections
{
    public interface IDbConnectionFactory
    {
        IDbConnection Connect();
    }
}
