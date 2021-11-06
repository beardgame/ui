using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Bearded.UI.Navigation
{
    public sealed class DependencyResolver
    {
        private readonly Dictionary<Type, object> dict = new Dictionary<Type, object>();

        public void Add<T>(T dependency)
        {
            if (dependency == null)
                throw new ArgumentNullException(nameof(dependency));

            dict[typeof(T)] = dependency;
        }

        public T Resolve<T>()
        {
            return (T) dict[typeof(T)];
        }

        public bool TryResolve<T>([NotNullWhen(returnValue: true)] out T? value) where T : class
        {
            if (dict.TryGetValue(typeof(T), out var obj))
            {
                value = (T) obj;
                return true;
            }

            value = null;
            return false;
        }

        public bool TryResolve<T>(out T? value) where T : struct
        {
            if (dict.TryGetValue(typeof(T), out var obj))
            {
                value = (T) obj;
                return true;
            }

            value = default(T);
            return false;
        }
    }
}
