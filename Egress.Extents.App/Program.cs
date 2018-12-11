using System;
using System.Collections.Generic;
using System.Linq;
using C5;
using System.Diagnostics;

namespace Egress.Extents.App
{
    public class Program
    {
        public static TreeDictionary<int, int> ProcessRanges(IEnumerable<Tuple<int, int>> ranges)
        {
            var summedRangeEdges = new TreeDictionary<int, int>();

            foreach (var currentRange in ranges)
            {
                var currentRangeLeft = currentRange.Item1;
                var currentRangeRight = currentRange.Item2;

                // Add left boundary if it isn't already there, with its value set as the value of the summed range it is included in, otherwise zero
                // E.g.: 1 to 10 with value 1 becomes 1 to 5 with value 1, 5 to 10 with value 1

                // Check if the node exists in the Tree
                if (!summedRangeEdges.Exists(x => x.Key == currentRangeLeft))
                {
                    summedRangeEdges.Add(currentRangeLeft,
                        summedRangeEdges.TryPredecessor(currentRangeLeft, out var predecessor)
                            ? predecessor.Value
                            : 0);
                }

                // Increment the value of all summed range edges between the left boundary (inclusive) and the right boundary (exclusive)
                var summedRangesSubMap = summedRangeEdges.RangeFromTo(currentRangeLeft, currentRangeRight);

                var keys = new SortedArray<int>();
                foreach(var key in summedRangesSubMap)
                {
                    keys.Add(key.Key);
                }

                foreach (var key in keys)
                {
                    summedRangeEdges[key]++;
                }

                // Add the right boundary
                // If there isn't a summed range edge to its left, use 0 for the value (should never happen as per the "put" above)
                // If the right boundary was already in the map, leave it as is
                // If the right boundary wasn't in the map, use the value to its left minus 1 (since this is a right boundary, which means a range was closed)

                if (summedRangeEdges.Exists(x => x.Key == currentRangeRight)) continue;
                {
                    if (summedRangeEdges.TryPredecessor(currentRangeRight, out var predecessor))
                    {
                        summedRangeEdges.Add(currentRangeRight, predecessor.Value - 1);
                    }
                    else
                    {
                        summedRangeEdges.Add(currentRangeRight, 0);
                    }
                }
            }

            return summedRangeEdges;
        }

        public static List<int> ProcessOutput(TreeDictionary<int, int> summedRangeEdges, List<int> numbers)
        {
            var outputs = new List<int>();

            var firstSummedRangeEdge = summedRangeEdges.First().Key;

            foreach (var number in numbers)
            {

                if (summedRangeEdges.TryPredecessor(number, out var floorEntry))
                {
                    var value = floorEntry.Value;

                    // Use the value of the summed range this number belongs to
                    // If this number is on top of the left boundary of that range, if there is another range before this one, count that as well by adding 1

                    if (floorEntry.Key != firstSummedRangeEdge && floorEntry.Key == number)
                    {
                        value++;
                    }

                    Debug.WriteLine(value);
                    outputs.Add(value);
                }
                else
                {
                    Debug.WriteLine(0);
                    outputs.Add(0);
                }
            }

            return outputs;
        }

        private static void Main(string[] args)
        {
            var ranges = new List<Tuple<int, int>>();

            using (var sr = new System.IO.StreamReader(@"extents.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var res = line.Split(' ');

                    try
                    {
                        ranges.Add(new Tuple<int, int>(int.Parse(res[0]), int.Parse(res[1])));
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine(e.Message);
                    }    
                }
            }

            var summedRangeEdges = ProcessRanges(ranges);

            var numbers = new List<int>();

            using (var sr = new System.IO.StreamReader(@"points.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    try
                    {
                        numbers.Add(int.Parse(line));
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }

            ProcessOutput(summedRangeEdges, numbers);
        }
    }
}
