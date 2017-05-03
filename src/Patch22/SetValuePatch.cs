using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patch22
{
    public class SetValuePatch<T> : PatchOfBase<T>
    {
        protected readonly Func<T, T> cloneFunc;

        internal SetValuePatch(Func<T, T> cloneFunc) 
        {
            this.cloneFunc = cloneFunc;
        }

        public override T Apply(T source)
        {
            var clone = this.cloneFunc(source);

            foreach (var change in this.changes)
            {
                typeof(T).GetField(change.Key).SetValue(clone, change.Value);
            }

            return clone;
        }
    }

    public class SetValuePatchFactory : IPatchFactory
    {
        public IPatchOf<T> Create<T>()
            where T : class, ICloneable
        {
            return new SetValuePatch<T>(x => x.Clone() as T);
        }
    }
}
