using System;
using System.Linq.Expressions;

namespace Patch22
{
    public interface IPatchOf<T>
    {
        T Apply(T source);
        PatchOfBase<T> Set<TProp>(Expression<Func<T, TProp>> memberExpression, TProp value);
    }
}