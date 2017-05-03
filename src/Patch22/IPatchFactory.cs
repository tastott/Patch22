using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patch22
{
    public interface IPatchFactory
    {
        IPatchOf<T> Create<T>() where T : class, ICloneable;
    }
}
