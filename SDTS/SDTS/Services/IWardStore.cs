using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SDTS.Services
{
    public interface IWardStore<T>
    {
        Task<bool> AddWardAsync(T ward);
        Task<bool> UpdateWardAsync(T ward);
        Task<bool> DeleteWardAsync(string id);
        Task<T> GetWardAsync(string id);
        Task<IEnumerable<T>> GetWardsAsync(bool forceRefresh = false);
    }
}
