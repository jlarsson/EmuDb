using System;
using System.Linq;
using EmuDb.Skiplists;
using NUnit.Framework;

namespace EmuDb.Tests.Skiplists
{
    [TestFixture]
    public class Scratchpad
    {
        [Test]
        public void Test()
        {
            var list = new Skiplist<int, int>(new SkiplistMemoryArena<int, int>());


            var keys = Enumerable.Range(1, 40);

            foreach(var key in keys)
            {
                list.Add(key, key);
                //list.Dump(Console.Out);
            }
            list.Dump(Console.Out);

            var x = list.Keys;
        }
    }
}
