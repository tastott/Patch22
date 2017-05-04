using FastMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Patch22
{
    public class ConstructorPatch<T> : PatchOfBase<T>
    {
        private delegate T Applier(T source, IDictionary<string, object> changes);

        private static Applier GenerateApplier()
        {
            var type = typeof(T);
            var members = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var field in type.GetFields())
            {
                members[field.Name] = field.FieldType;
            }
              
            foreach (var property in type.GetProperties())
            {
                members[property.Name] = property.PropertyType;
            }

            ConstructorInfo constructor = null;
            string[] parameterNames = null;
            foreach (var constructorInfo in type.GetConstructors())
            {
                var parameters = constructorInfo.GetParameters()
                    .ToDictionary(parameter => parameter.Name, parameter => parameter.ParameterType, StringComparer.InvariantCultureIgnoreCase);

                if (members.All(member => parameters.ContainsKey(member.Key) && parameters[member.Key].Equals(member.Value)))
                {
                    constructor = constructorInfo;
                    parameterNames = parameters
                        .Select(parameter => members.Single(s => s.Key.Equals(parameter.Key, StringComparison.InvariantCultureIgnoreCase)).Key)
                        .ToArray();
                }
            };

            var accessor = TypeAccessor.Create(type, true);

            return (source, changes) =>
            {
                var parameters = parameterNames.Select(parameterName =>
                {
                    object value;
                    if (changes.TryGetValue(parameterName, out value))
                    {
                        return value;
                    }
                    else
                    {
                        return accessor[source, parameterName];
                    }
                });

                return (T)constructor.Invoke(parameters.ToArray());
            };
        }

        private static readonly Applier _Applier;

        static ConstructorPatch()
        {
            _Applier = GenerateApplier();
        }

        public override T Apply(T source)
        {
            return _Applier(source, this.changes);
        }
    }

    public class ConstructorPatchFactory : IPatchFactory
    {
        public IPatchOf<T> Create<T>()
            where T : class, ICloneable
        {
            return new ConstructorPatch<T>();
        }
    }
}
