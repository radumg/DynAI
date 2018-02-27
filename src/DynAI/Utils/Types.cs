using Autodesk.DesignScript.Runtime;
using System.Collections;
using System.Collections.Generic;

namespace AI.Utils
{
    internal static class Types
    {
        [IsVisibleInDynamoLibrary(false)]
        public static bool IsList(object o)
        {
            if (o == null) return false;
            return o is IList &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        [IsVisibleInDynamoLibrary(false)]
        public static bool IsDictionary(object o)
        {
            if (o == null) return false;
            return o is IDictionary &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));
        }
    }
}