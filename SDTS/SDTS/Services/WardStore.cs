using SDTS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDTS.Services
{
    class WardStore:IWardStore<Ward>
    {
        readonly List<Ward> wards;

        public WardStore()
        {
            wards = new List<Ward>()
            {
                new Ward{UserID = 1,Name="cqf",Information="1",Birthday=DateTime.Now,Gender="1",PhoneNumber="2"},
                new Ward{UserID = 2,Name="ccc",Information="2",Birthday=DateTime.Now,Gender="1",PhoneNumber="2"},
                new Ward{UserID = 3,Name="qqq",Information="3",Birthday=DateTime.Now,Gender="1",PhoneNumber="2"},
                new Ward{UserID = 4,Name="fff",Information="4",Birthday=DateTime.Now,Gender="1",PhoneNumber="2"}
            };
        }
        public async Task<bool> AddWardAsync(Ward ward)
        {
            wards.Add(ward);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateWardAsync(Ward ward)
        {
            var oldItem = wards.Find(a => a.UserID == ward.UserID);
            wards.Remove(oldItem);
            wards.Add(ward);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteWardAsync(string id)
        {
            //var oldItem = wards.Where((Item arg) => arg.Id == id).FirstOrDefault();
            var oldItem = wards.Find(a => a.UserID.ToString() == id);
            wards.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Ward> GetWardAsync(string id)
        {
            return await Task.FromResult(wards.FirstOrDefault(s => s.UserID.ToString() == id));
        }

        public async Task<IEnumerable<Ward>> GetWardsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(wards);
        }
    }
}
