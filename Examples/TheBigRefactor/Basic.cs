using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ProtoBuf.Decorators;

namespace Examples.TheBigRefactor
{
    [TestFixture]
    public class Basic
    {
#if EMIT
        [Test]
        public void TestPerformance()
        {
            PerfTest.RunPerformance();
        }
#endif
    }
}
