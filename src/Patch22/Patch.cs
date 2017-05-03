using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Patch22
{
    public class PatchOf<T>
    {
        internal PatchOf()
        {

        }

        public PatchOf<T> Set<TProp>(Expression<Func<T, TProp>> memberExpression, TProp value)
        {
            return this;
        }

        public T Apply(T source)
        {
            return source;
        }
    }

    public static class Patch
    {
        public static PatchOf<T> Of<T>()
            where T : class, ICloneable
        {
            return new PatchOf<T>();
        }
    }
}
