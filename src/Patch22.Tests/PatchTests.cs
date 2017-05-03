using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Patch22
{
    [TestClass]
    public class PatchTests
    {
        [TestMethod]
        public void ReadOnlyField()
        {
            var thing = new ImmutableThing(4);
            var patch = Patch.Of<ImmutableThing>()
                .Set(x => x.MyReadOnlyInt, 2);
            var expected = new ImmutableThing(2);

            var patched = patch.Apply(thing);

            patched.ShouldBeEquivalentTo(expected);
            thing.MyReadOnlyInt.Should().Be(4); // Original is unchanged
        }

        private class ImmutableThing : ICloneable
        {
            public readonly int MyReadOnlyInt;

            public ImmutableThing(int myReadOnlyInt)
            {
                this.MyReadOnlyInt = myReadOnlyInt;
            }

            public object Clone()
            {
                return new ImmutableThing(this.MyReadOnlyInt);
            }
        }
    }
}
