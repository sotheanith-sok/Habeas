using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using Metrics.MeanAveragePrecision;

namespace UnitTests.EvaluateTests
{
    public class MAPTests
    {
        private static string corpusPath = "../../../corpus/Cranfield/";

        private static MeanAveragePrecision mAP = new MeanAveragePrecision();


        [Fact]
        public void TestGetMAP()
        {
            mAP.GetMAP();
        }

        [Fact]
        public void TestCalculateMAP()
        {
            List<List<int>> result = new List<List<int>>{
                new List<int>{10},
                new List<int>{10},
                new List<int>{10},
                new List<int>{10},
            };
            List<List<int>> actual = new List<List<int>>{
                new List<int>{10},
                new List<int>{10},
                new List<int>{10,20},
                new List<int>{10,20},
            };

            mAP.CalculateMAP(result, actual).Should().Be( 0.75F );
        }


        [Fact]
        public void TestCalculateAP()
        {
            List<int> result = new List<int>{1,2,33,4,55,66,77,8};
            List<int> actual = new List<int>{1,2,3,4,5,6,7,8};

            mAP.CalculateAP(result, actual).Should().Be( 13/32 );
        }

    }
}