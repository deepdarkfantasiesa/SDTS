﻿using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SDTS.DataAccess.Interface
{
    public interface IRescureGroupRepository
    {
        public Task<bool> AddToRescureGroup(RescureGroup rescureGroup);
        public Task<RescureGroup> QueryRescurer(string rescureraccount);
        public Task<bool> DeleteRescurerAsync(string rescureraccount);
        public Task<IEnumerable<RescureGroup>> DeleteRescurersAsync(string groupname);
    }
}
