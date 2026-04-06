using System.Data;

namespace Payments.Infrastructure.DBContext;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
