using Autodesk.DesignScript.Runtime;
using System.Collections;
using System.Collections.Generic;

namespace AI.Utils
{
    internal static class Types
    {
        public static bool IsList(this object o)
        {
            if (o == null) return false;
            var type = o.GetType();

            // immediate confirmation of arrays and lists
            if (type.IsArray) return true;
            if (o is IList) return true;
            if (o is IEnumerable) return true;

            return type.IsGenericType &&
                   type.IsAssignableFrom(typeof(List<>));
        }

        public static bool IsDictionary(this object o)
        {
            if (o == null) return false;
            return o is IDictionary &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));
        }
    }
}