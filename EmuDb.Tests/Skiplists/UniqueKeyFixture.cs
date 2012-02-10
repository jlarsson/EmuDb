using EmuDb.Skiplists;
using NUnit.Framework;

namespace EmuDb.Tests.Skiplists
{
    [TestFixture]
    public class UniqueKeyFixture
    {
        [Test]
        public void ThrowsOnDuplicateKey()
        {
            var list = new Skiplist<int, int>(new SkiplistMemoryArena<int, int>(), KeyStrategy.Unique) {{1, 1}};
            Assert.Throws<DuplicateKeyException>(() => list.Add(1, 2));
        }

        [Test]
        public void IndexSetterUpdatesExisting()
        {
            var list = new Skiplist<int, int>(new SkiplistMemoryArena<int, int>()) {{1, 1}};

            list[1] = 123;

            Assert.AreEqual(123, list[1]);
        }

        [Test]
        public void IndexSetterAddsIfMissing()
        {
            var list = new Skiplist<int, int>(new SkiplistMemoryArena<int, int>());

            list[1] = 123;

            Assert.AreEqual(123, list[1]);
        }
    }
}
