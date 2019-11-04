using Xunit;
using System.Collections.Generic;
using Search.Document;
using Search.Index;
using Search.Text;
using System.Runtime.InteropServices;
using System;
using FluentAssertions;

namespace UnitTests.IndexTests
{

    public class MaxPriorityQueueTests
    {

        private MaxPriorityQueue testingQueue = new MaxPriorityQueue();

        [Fact]
        public void testingMaxPQ()
        {
            testingQueue.MAXHEAPINSERT(8.5, 1);
            testingQueue.MAXHEAPINSERT(9.5, 2);
            testingQueue.MAXHEAPINSERT(12.5, 3);
            testingQueue.MAXHEAPINSERT(2.5, 4);
            testingQueue.MAXHEAPINSERT(6.5, 5);
            testingQueue.MAXHEAPINSERT(8.5, 6);
            testingQueue.MAXHEAPINSERT(17.5, 7);
            testingQueue.MAXHEAPINSERT(3.5, 8);
            testingQueue.MAXHEAPINSERT(0.5, 9);
            testingQueue.MAXHEAPINSERT(1.5, 10);


            testingQueue.RetrieveTopTen();

        }
    }
}