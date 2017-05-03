using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patch22.Performance
{
    public class SmallImmutableThing : ICloneable
    {
        public readonly int MyInt;
        public readonly string MyString;

        public SmallImmutableThing(int myInt, string myString)
        {
            this.MyInt = myInt;
            this.MyString = myString;
        }

        public object Clone()
        {
            return new SmallImmutableThing(this.MyInt, this.MyString);
        }
    }
}
