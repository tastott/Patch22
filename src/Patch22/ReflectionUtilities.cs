using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Patch22
{
    public static class ReflectionUtilities
    {
        public static IEnumerable<Tuple<string, Type>> GetPropertiesAndFields(this Type type)
        {
            foreach (var field in type.GetFields())
            {
                yield return Tuple.Create(field.Name, field.FieldType);
            }

            foreach (var property in type.GetProperties())
            {
                yield return Tuple.Create(property.Name, property.PropertyType);
            }
        }

        public static ConstructorInfo FindMemberNamesConstructor(this Type type)
        {
            var members = type.GetPropertiesAndFields()
                .ToDictionary(entry => entry.Item1, entry => entry.Item2, StringComparer.InvariantCultureIgnoreCase);

            return type.GetConstructors().First(constructorInfo =>
            {
                var parameters = constructorInfo.GetParameters()
                    .ToDictionary(parameter => parameter.Name, parameter => parameter.ParameterType, StringComparer.InvariantCultureIgnoreCase);

                return members.All(member => parameters.ContainsKey(member.Key) && parameters[member.Key].Equals(member.Value));
            });
        }

        public delegate T ConstructorOf<T>(object[] args);

        // Courtesy of https://rogerjohansson.blog/2008/02/28/linq-expressions-creating-objects/
        public static ConstructorOf<T> CompileConstructor<T>(ConstructorInfo ctor)
        {
            Type type = ctor.DeclaringType;
            ParameterInfo[] paramsInfo = ctor.GetParameters();

            //create a single param of type object[]
            ParameterExpression param = Expression.Parameter(typeof(object[]), "args");

            Expression[] argsExp = new Expression[paramsInfo.Length];

            //pick each arg from the params array 
            //and create a typed expression of them
            for (int i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;

                Expression paramAccessorExp =
                    Expression.ArrayIndex(param, index);

                Expression paramCastExp =
                    Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            //make a NewExpression that calls the
            //ctor with the args we just created
            NewExpression newExp = Expression.New(ctor, argsExp);

            //create a lambda with the New
            //Expression as body and our param object[] as arg
            LambdaExpression lambda = Expression.Lambda(typeof(ConstructorOf<T>), newExp, param);

            //compile it
            ConstructorOf<T> compiled = (ConstructorOf<T>)lambda.Compile();
            return compiled;
        }
    }
}
