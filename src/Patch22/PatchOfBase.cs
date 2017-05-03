using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Patch22
{
    public abstract class PatchOfBase<T> : IPatchOf<T>
    {
        protected readonly Dictionary<string, object> changes;

        protected PatchOfBase()
        {
            this.changes = new Dictionary<string, object>();
        }

        public PatchOfBase<T> Set<TProp>(Expression<Func<T, TProp>> memberExpression, TProp value)
        {
            var memberName = (memberExpression.Body as MemberExpression).Member.Name;
            this.changes[memberName] = value;
            return this;
        }

        public abstract T Apply(T source);
    }
}
