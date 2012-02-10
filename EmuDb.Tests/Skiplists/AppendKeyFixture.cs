using System;
using EmuDb.Skiplists;
using NUnit.Framework;

namespace EmuDb.Tests.Skiplists
{
    [TestFixture]
    public class AppendKeyFixture
    {
        [Test]
        public void ValuesWithSameKeyAreAppendedInReverseOrder()
        {
            var list = new Skiplist<int, int>(new SkiplistMemoryArena<int, int>(), KeyStrategy.Append) {{1, 1}, {1, 2}};

            Assert.That(new []{1,1},Is.EqualTo(list.Keys));
            Assert.That(new[] { 2,1 }, Is.EqualTo(list.Values));
        }

        [Test]
        public void IndexerReturnsLastAdded()
        {
            var list = new Skiplist<int, int>(new SkiplistMemoryArena<int, int>(), KeyStrategy.Append) { { 1, 111 }, { 1, 234 } };

            Assert.That(234, Is.EqualTo(list[1]));
        }

        [Test]
        public void IndexerUpdatesLastAdded()
        {
            var list = new Skiplist<int, int>(new SkiplistMemoryArena<int, int>(), KeyStrategy.Append) { { 1, 123 }, { 1, 234 } };

            list[1] = 12345;
            Assert.That(list[1], Is.EqualTo(12345));

            list.Dump(Console.Out);
            //Assert.That(new[] { 12345, 123 }, Is.EqualTo(list.Values));
        }

    }
}