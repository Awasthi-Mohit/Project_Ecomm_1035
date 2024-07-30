using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecomm_App_1035.DataAccess.Repository.IRepository
{
    public interface ISPCALL : IDisposable
    {
        void Execute(string procedureName, DynamicParameters param = null);
        T Single<T>(string procedureName, DynamicParameters param = null);
        T OneRecord<T>(string procedureName, DynamicParameters param = null);
        IEnumerable<T> List<T>(string procedureName, DynamicParameters param = null);
        Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(String procedureName, DynamicParameters param = null);

    }
}
