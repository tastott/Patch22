using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patch22.Performance
{
    class Program
    {
        static void Main(string[] args)
        {
            var patchFactories = new IPatchFactory[]
            {
                new SetValuePatchFactory(),
                new FastMemberPatchFactory(),
                new ConstructorPatchFactory()
            };

            foreach (var factory in patchFactories)
            {
                var thing = new SmallImmutableThing(1, "blah");
                var patch = factory.Create<SmallImmutableThing>()
                    .Set(x => x.MyInt, 2)
                    .Set(x => x.MyString, "foo");


                GetDurationOf(() => patch.Apply(thing), 1000000, $"{patch.GetType().Name} on small object");
            }



            //var mutableThing = new SmallMutableThing();
            //Action mutateThing = () =>
            //{
            //    mutableThing.MyInt = 2;
            //    mutableThing.MyString = "foo";
            //};
            //GetDurationOf(mutateThing, 1000000, "Direct mutation of small object");

            Console.ReadLine();
        }

        private static long GetDurationOf(Action action, int repetitions = 1, string name = null)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (var i = 0; i < repetitions; i++)
            {
                action();
            }

            stopwatch.Stop();

            if (!string.IsNullOrEmpty(name))
            {
                Console.WriteLine($"Doing {name} {repetitions:#,#} times took {stopwatch.ElapsedMilliseconds}ms.");
            }
            
            return stopwatch.ElapsedMilliseconds;
        }
    }
}
