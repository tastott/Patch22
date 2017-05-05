using FastMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Patch22
{
    public class ConstructorCompilePatch<T> : PatchOfBase<T>
    {
        private static readonly ConstructorInvokePatch<T>.Applier _Applier;

        static ConstructorCompilePatch()
        {
            _Applier = ConstructorInvokePatch<T>.GenerateApplier(constructor => ReflectionUtilities.CompileConstructor<T>(constructor));
        }

        public override T Apply(T source)
        {
            return _Applier(source, this.changes);
        }
    }

    public class ConstructorCompilePatchFactory : IPatchFactory
    {
        public IPatchOf<T> Create<T>()
            where T : class, ICloneable
        {
            return new ConstructorCompilePatch<T>();
        }
    }
}
