using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using System;
using System.Linq;
using Metrics.MeanAveragePrecision;

namespace UnitTests.EvaluateTests
{
    public class MAPTests
    {
        private static string corpusPath = "../../../corpus/Cranfield/relevance/";
        private static string queryFilePath = corpusPath + "Actualqueries";
        private static string qrelFilePath = corpusPath + "qrel";

        private static MeanAveragePrecision mAP = new MeanAveragePrecision();


        [Fact]
        public void TestConvertRankedResult()
        {
            mAP.GetMAP();
        }

        [Fact]
        public void TestGetMAP()
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
        public void TestGetAP()
        {
            List<int> result = new List<int>{1,2,33,4,55,66,77,8};
            List<int> actual = new List<int>{1,2,3,4,5,6,7,8};

            mAP.CalculateAP(result, actual).Should().Be( 13/32 );
        }

    }
}