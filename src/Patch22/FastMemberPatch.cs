using FastMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patch22
{
    public class FastMemberPatch<T> : PatchOfBase<T>
    {
        protected readonly Func<T, T> cloneFunc;
        private readonly TypeAccessor accessor;

        internal FastMemberPatch(Func<T, T> cloneFunc)
        {
            this.cloneFunc = cloneFunc;
            this.accessor = TypeAccessor.Create(typeof(T), true);
        }

        public override T Apply(T source)
        {
            var clone = this.cloneFunc(source);

            foreach (var change in this.changes)
            {
                this.accessor[clone, change.Key] = change.Value;
            }

            return clone;
        }
    }

    public class FastMemberPatchFactory : IPatchFactory
    {
        public IPatchOf<T> Create<T>()
            where T : class, ICloneable
        {
            return new FastMemberPatch<T>(x => x.Clone() as T);
        }
    }
}
