using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Extensions.Utility
{
    public static class OrderQueryBuilder
    {
        public static string CreateOrderQuery<T>(string orderByQueryString)
        {
            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(T).GetProperties(BindingFlags.Public |
                BindingFlags.Instance);
            var orderQuerybuilder = new StringBuilder();
            foreach (var param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param))
                    continue;
                var propertyFromQueryName = param.Split(" ")[0];
                var objectproperty = propertyInfos.FirstOrDefault(pi =>
                pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));
                if (objectproperty == null)
                    continue;
                var direction = param.EndsWith(" desc") ? "descending" : "ascending";
                orderQuerybuilder.Append($"{objectproperty.Name.ToString()} {direction}, ");
            }
            var orderQuery = orderQuerybuilder.ToString().TrimEnd(',', ' ');
            return orderQuery;
        }
    }
}
