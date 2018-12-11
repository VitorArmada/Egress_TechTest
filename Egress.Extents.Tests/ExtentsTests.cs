using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Egress.Extents.App;
using C5;
using Shouldly;

namespace Egress.Extents.Tests
{
    [TestClass]
    public class ExtentsTests
    {
        private readonly List<Tuple<int, int>> _ranges = new List<Tuple<int, int>>
        {
            new Tuple<int, int>(1, 8),
            new Tuple<int, int>(4, 12),
            new Tuple<int, int>(2, 6),
            new Tuple<int, int>(3, 4),
            new Tuple<int, int>(0, 3)
        };

        private readonly List<int> _numbers = new List<int>{1,3,5,9,13,-1, 7,-20000};

        [TestMethod]
        public void ProcessRanges_Returns_Correct_TreeMapResult()
        {
            var processedRanges = Program.ProcessRanges(_ranges);

            processedRanges.Keys.ShouldBe(new List<int>{0,1,2,3,4,6,8,12});
            processedRanges.Values.ShouldBe(new List<int>{1,2,3,3,3,2,1,0});
        }

        [TestMethod]
        public void ProcessNumbers_Returns_Correct_ListOfOutputs()
        {
            var processedRanges = Program.ProcessRanges(_ranges);
            var output = Program.ProcessOutput(processedRanges, _numbers);

            output.ShouldBe(new List<int>{1,3,3,1,0,0,2,0});
        }
    }
}
