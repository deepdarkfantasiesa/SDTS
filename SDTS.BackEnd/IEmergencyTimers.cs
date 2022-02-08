using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;

namespace SDTS.BackEnd
{
    public interface IEmergencyTimers
    {
        public void Init();
        public void InitTimer();
    }
}
