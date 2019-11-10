using System.Collections.Generic;
using System;
using System.IO;
using Search.Query;
public class MaxPriorityQueue
{
    public class InvertedIndex
    {
        private double rank;
        private int docID;

        public InvertedIndex(double rank, int docID)
        {
            this.rank = rank;
            this.docID = docID;
        }

        public double GetRank()
        {
            return rank;
        }
        public int GetDocumentId()
        {
            return docID;
        }
    }

    private List<InvertedIndex> priorityQueue;


    public MaxPriorityQueue()
    {
        priorityQueue = new List<InvertedIndex>();
    }

    private int Left(int index)
    {
        int Left = index * 2 + 1;
        return Left;
    }

    private int Parent(int index)
    {

        if (index < 1)
            return 0;
        else
        {
            int parent = (int)Math.Ceiling((double)index / 2);
            return parent;
        }

    }
    private int Right(int index)
    {
        int Right = index * 2 + 2;
        return Right;
    }

    private void MaxHeapify(List<InvertedIndex> queue, int index)
    {

        int leftChild = Left(index);
        int rightChild = Right(index);
        int largest = index;

        if (leftChild <= queue.Count - 1 && rightChild <= queue.Count - 1)
        {
            if (leftChild <= queue.Count - 1 && queue[leftChild].GetRank() > queue[index].GetRank())
                largest = leftChild;
            if (rightChild <= queue.Count - 1 && queue[rightChild].GetRank() > queue[largest].GetRank())
                largest = rightChild;
        }
        else if (leftChild <= queue.Count - 1 && rightChild > queue.Count - 1)
        {
            if (queue[leftChild].GetRank() > queue[index].GetRank())
                largest = leftChild;
        }

        else if (rightChild <= queue.Count && leftChild > queue.Count)
        {
            if (queue[rightChild].GetRank() > queue[index].GetRank())
                largest = rightChild;
        }

        if (largest != index)
        {
            InvertedIndex temp = queue[index];
            queue[index] = queue[largest];
            queue[largest] = temp;
            MaxHeapify(queue, largest);
        }
    }



    public void MaxHeapInsert(double rank, int docId)
    {
        InvertedIndex element = new InvertedIndex(rank, docId);
        List<InvertedIndex> tempQueue = GetPriorityQueue();
        tempQueue.Add(element);

        for (int i = tempQueue.Count / 2; i >= 0; i--)
        {
            MaxHeapify(tempQueue, i);
        }

        SetPriorityQueue(tempQueue);

    }

    public List<InvertedIndex> RetrieveTopTen()
    {
        List<InvertedIndex> priorityQueue = GetPriorityQueue();
        List<InvertedIndex> topTen = new List<InvertedIndex>();
        for (int i = 0; i < 10; i++)
        {
            InvertedIndex max = ExtractMax(priorityQueue);
            topTen.Add(max);
        }

        return topTen;

    }

    private InvertedIndex ExtractMax(List<InvertedIndex> queue)
    {
        if (queue.Count < 1)
            Console.WriteLine("ERROR: Underflow");
        InvertedIndex max = queue[0];
        queue[0] = queue[queue.Count - 1];
        queue.RemoveAt(queue.Count - 1);
        MaxHeapify(queue, 0);

        return max;

    }

    public List<InvertedIndex> GetPriorityQueue()
    {
        return priorityQueue;
    }

    public void SetPriorityQueue(List<InvertedIndex> sortedRankedList)
    {
        priorityQueue = sortedRankedList;
    }

}






