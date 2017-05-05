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
            var thing = new ImmutableThing(4, "blah");
            var patch = new ConstructorCompilePatchFactory().Create<ImmutableThing>()
                .Set(x => x.MyReadOnlyInt, 2)
                .Set(x => x.MyReadOnlyString, "foo");
            var expected = new ImmutableThing(2, "foo");

            var patched = patch.Apply(thing);

            patched.ShouldBeEquivalentTo(expected);
            thing.MyReadOnlyInt.Should().Be(4); // Original is unchanged
        }

        private class ImmutableThing : ICloneable
        {
            public readonly int MyReadOnlyInt;
            public readonly string MyReadOnlyString;

            public ImmutableThing(int myReadOnlyInt, string myReadOnlyString)
            {
                this.MyReadOnlyInt = myReadOnlyInt;
                this.MyReadOnlyString = myReadOnlyString;
            }

            public object Clone()
            {
                return new ImmutableThing(this.MyReadOnlyInt, this.MyReadOnlyString);
            }
        }
    }
}
