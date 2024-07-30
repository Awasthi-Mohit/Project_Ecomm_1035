using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Project_Ecomm_App_1035.DataAccess.Data;
using Project_Ecomm_App_1035.DataAccess.Repository.IRepository;

using System.Data;


namespace Project_Ecomm_App_1035.DataAccess.Repository
{
    public class SPCALL : ISPCALL
    {
        private readonly ApplicationDbContext _context;
        private static string connectionString = "";
        public SPCALL(ApplicationDbContext context)
        {
            _context = context;
            connectionString = _context.Database.GetDbConnection().ConnectionString;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Execute(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                sqlCon.Execute(procedureName, param,commandType:CommandType.StoredProcedure);
            }
        }

        public IEnumerable<T> List<T>(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                return sqlCon.Query<T>(procedureName,param,commandType:CommandType.StoredProcedure);
            }
        }

        public Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                var result=sqlCon.QueryMultiple(procedureName, param, 
                    commandType:CommandType.StoredProcedure);
                var item1=result.Read<T1>();
                var item2=result.Read<T2>();
                if(item1 != null && item2 != null) 
                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(item1, item2);
                    return new Tuple <IEnumerable<T1>, IEnumerable<T2>>(new List<T1>(),new List<T2>());

            }
        }

        public T OneRecord<T>(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                var value = sqlCon.Query<T>(procedureName,param,commandType:CommandType.StoredProcedure);
                return value.FirstOrDefault();
            }
        }

        public T Single<T>(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                return sqlCon.ExecuteScalar<T>(procedureName,param,commandType:CommandType.StoredProcedure);
            }

        }
    }
}
