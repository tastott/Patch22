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
        private readonly Func<T, T> cloneFunc;
        private readonly Dictionary<string, object> changes;

        internal PatchOf(Func<T, T> cloneFunc)
        {
            this.cloneFunc = cloneFunc;
            this.changes = new Dictionary<string, object>();
        }

        public PatchOf<T> Set<TProp>(Expression<Func<T, TProp>> memberExpression, TProp value)
        {
            var memberName = (memberExpression.Body as MemberExpression).Member.Name;
            this.changes[memberName] = value;
            return this;
        }

        public T Apply(T source)
        {
            var clone = this.cloneFunc(source);
            foreach (var change in this.changes)
            {
                typeof(T).GetField(change.Key).SetValue(clone, change.Value);
            }

            return clone;
        }
    }

    public static class Patch
    {
        public static PatchOf<T> Of<T>()
            where T : class, ICloneable
        {
            return new PatchOf<T>(instance => instance.Clone() as T);
        }
    }
}
