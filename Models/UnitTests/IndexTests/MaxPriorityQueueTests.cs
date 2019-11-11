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
            //random rank values corresponding to list of document id
            List<double> rankValue = new List<double>{8.5,9.5,12.5,2.5,6.5,8.5,17.5,3.5,0.5,1.5};

            List<int> docId = new List<int>{1,2,3,4,5,6,7,8,9,10};

            //insert into the priority queue using max heap property
            for(int i =0 ; i <10 ; i++)
            {
                testingQueue.MaxHeapInsert(rankValue[i],docId[i]);
            }
           
            //order for documents according to their id based off of their rank values
            List<int> expected = new List<int>{7,1,3,8,5,6,2,4,9,10};

            //testing to make sure set up for extract max is correct. this tree contains the max heap property 
            List<MaxPriorityQueue.InvertedIndex> heapQueue = testingQueue.GetPriorityQueue();

            for(int i = 0; i<10; i++)
            {
                heapQueue[i].GetDocumentId().Should().Be(expected[i]);
            }
          

            // this implements extract max from priority queue. which pulls highest documents continuously and returns a list
            List<MaxPriorityQueue.InvertedIndex> testList =  testingQueue.RetrieveTopTen();
            testList.Count.Should().Be(10);

            // highest value (17.5) in the priority queue 
            testList[0].GetDocumentId().Should().Be(7); 

            // document with second highest value (12.5) in the priority queue
            testList[1].GetDocumentId().Should().Be(3); 
            
        }
    }
}