﻿using Models;
using SDTS.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDTS.DataAccess.Interface
{
    public interface ISecureAreaRepository
    {
        public Task<bool> AddareaAsync(SecureArea secureArea);
        public Task<SecureArea> FindareaAsync(string secureAreaid);
        public Task<SecureArea> AlterareaAsync(SecureArea secureArea);
     
        public Task<bool> DeleteareaAsync(string secureAreaid);

        public Task<List<SecureArea>> FindWardAreasAsync(string wardaccount);
    }
}
