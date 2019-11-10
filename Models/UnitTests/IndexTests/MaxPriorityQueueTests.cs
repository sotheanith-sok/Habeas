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
            testingQueue.MaxHeapInsert(8.5, 1);
            testingQueue.MaxHeapInsert(9.5, 2);
            testingQueue.MaxHeapInsert(12.5, 3);
            testingQueue.MaxHeapInsert(2.5, 4);
            testingQueue.MaxHeapInsert(6.5, 5);
            testingQueue.MaxHeapInsert(8.5, 6);
            testingQueue.MaxHeapInsert(17.5, 7);
            testingQueue.MaxHeapInsert(3.5, 8);
            testingQueue.MaxHeapInsert(0.5, 9);
            testingQueue.MaxHeapInsert(1.5, 10);


            testingQueue.RetrieveTopTen();

        }
    }
}