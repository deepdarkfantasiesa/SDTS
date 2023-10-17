using Domain.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Domain.AggregatesModel.UserAggregate
{
    public class Role : Enumeration
    {
        public static Role Volunteer = new Role(1, nameof(Volunteer).ToLowerInvariant());
        public static Role Guardian = new Role(2,nameof(Guardian).ToLowerInvariant());
        public static Role Ward = new Role(3, nameof(Ward).ToLowerInvariant());

        public static IEnumerable<Role> List() =>
        new[] { Volunteer, Guardian, Ward };

        public Role(int id, string name) : base(id, name)
        {
        }

        public static Role FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                // throw new OrderingDomainException($"Possible values for OrderStatus: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static Role From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                // throw new OrderingDomainException($"Possible values for OrderStatus: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
