using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDTS.VolunteerViews
{
    public class VolunteerFlyoutPageFlyoutMenuItem
    {
        public VolunteerFlyoutPageFlyoutMenuItem()
        {
            TargetType = typeof(VolunteerFlyoutPageFlyoutMenuItem);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}