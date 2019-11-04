using System.Collections.Generic;
using System;
using System.IO;
using Search.Query;
public class MaxPriorityQueue
{
    public class RevertedIndex
    {
        private double rank;
        private int docID;

        public RevertedIndex(double rank, int docID)
        {
            this.rank = rank;
            this.docID = docID;
        }

        public double GetRank()
        {
            return rank;
        }
    }

    private List<RevertedIndex> priorityQueue;


    public MaxPriorityQueue()
    {
        priorityQueue = new List<RevertedIndex>();
    }

    private int LEFT(int index)
    {
        int LEFT = index * 2 + 1;
        return LEFT;
    }

    private int PARENT(int index)
    {

        if (index < 1)
            return 0;
        else
        {
            int parent = (int)Math.Ceiling((double)index / 2);
            return parent;
        }

    }
    private int RIGHT(int index)
    {
        int RIGHT = index * 2 + 2;
        return RIGHT;
    }

    private void MAXHEAPIFY(List<RevertedIndex> queue, int index)
    {

        int leftChild = LEFT(index);
        int rightChild = RIGHT(index);
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
            RevertedIndex temp = queue[index];
            queue[index] = queue[largest];
            queue[largest] = temp;
            MAXHEAPIFY(queue, largest);
        }
    }



    public void MAXHEAPINSERT(double queue, int docId)
    {
        RevertedIndex element = new RevertedIndex(queue, docId);
        List<RevertedIndex> tempQueue = GetPriorityQueue();
        tempQueue.Add(element);

        for (int i = tempQueue.Count / 2; i >= 0; i--)
        {
            MAXHEAPIFY(tempQueue, i);
        }

        SetPriorityQueue(tempQueue);

    }

    public List<RevertedIndex> RetrieveTopTen()
    {
        List<RevertedIndex> priorityQueue = GetPriorityQueue();
        List<RevertedIndex> topTen = new List<RevertedIndex>();
        for (int i = 0; i < 10; i++)
        {
            RevertedIndex max = EXTRACTMAX(priorityQueue);
            topTen.Add(max);
        }

        return topTen;

    }

    private RevertedIndex EXTRACTMAX(List<RevertedIndex> queue)
    {
        if (queue.Count < 1)
            Console.WriteLine("ERROR: Underflow");
        RevertedIndex max = queue[0];
        queue[0] = queue[queue.Count - 1];
        queue.RemoveAt(queue.Count - 1);
        MAXHEAPIFY(queue, 0);

        return max;

    }

    public List<RevertedIndex> GetPriorityQueue()
    {
        return priorityQueue;
    }

    public void SetPriorityQueue(List<RevertedIndex> sortedRankedList)
    {
        priorityQueue = sortedRankedList;
    }

}






