using FastMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Patch22
{
    public class ConstructorInvokePatch<T> : PatchOfBase<T>
    {
        public delegate T Applier(T source, IDictionary<string, object> changes);

        public static Applier GenerateApplier(Func<ConstructorInfo, ReflectionUtilities.ConstructorOf<T>> getConstructorInvoker)
        {
            var type = typeof(T);
            var constructor = type.FindMemberNamesConstructor();
            var memberNames = type.GetPropertiesAndFields()
                .Select(entry => entry.Item1)
                .ToDictionary(name => name, StringComparer.InvariantCultureIgnoreCase);

            var memberNamesInParameterOrder = constructor.GetParameters()
                .Select(parameter => memberNames[parameter.Name])
                .ToArray();

            var accessor = TypeAccessor.Create(type, true);
            var invoker = getConstructorInvoker(constructor);
            return (source, changes) =>
            {
                var parameters = memberNamesInParameterOrder.Select(memberName =>
                {
                    object value;
                    if (changes.TryGetValue(memberName, out value))
                    {
                        return value;
                    }
                    else
                    {
                        return accessor[source, memberName];
                    }
                });

                return invoker(parameters.ToArray());
            };
        }

        private static readonly Applier _Applier;

        static ConstructorInvokePatch()
        {
            _Applier = GenerateApplier(constructor => args => (T)constructor.Invoke(args));
        }

        public override T Apply(T source)
        {
            return _Applier(source, this.changes);
        }
    }

    public class ConstructorInvokePatchFactory : IPatchFactory
    {
        public IPatchOf<T> Create<T>()
            where T : class, ICloneable
        {
            return new ConstructorInvokePatch<T>();
        }
    }
}
