using System.Collections.Generic;
using System;
using System.IO;
using Search.Query;
using Search.Index;
public class MaxPriorityQueue
{
    /// <summary>
    /// object that contains the rank and document id which is the object the priority queue is maintianing 
    /// </summary>
    public class InvertedIndex
    {
        private double rank;
        private int docID;

        private int value;

        private Tuple<int, int> docTierTuple;

        public InvertedIndex(double rank, int docID)
        {
            this.rank = rank;
            this.docID = docID;
        }

        public InvertedIndex(int value, int docID)
        {
            this.value = value;
            this.docID = docID;
        }

        public InvertedIndex(int frequency, Tuple<int, int> docTier)
        {
            this.value = frequency;
            this.docTierTuple = docTier;

        }

        public InvertedIndex(double rank, Tuple<int, int> docTier)
        {
            this.rank = rank;
            this.docTierTuple = docTier;

        }



        public double GetRank()
        {
            return this.rank;
        }
        public int GetDocumentId()
        {
            return this.docID;
        }

        public int GetTermFreq()
        {
            return this.value;
        }

        public Tuple<int, int> GetTuple()
        {
            return this.docTierTuple;
        }


    }

    private List<InvertedIndex> priorityQueue;


    public MaxPriorityQueue()
    {
        this.priorityQueue = new List<InvertedIndex>();
    }

    /// <summary>
    /// method to retrieve the left child at a specific index in the binary heap
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private int Left(int index)
    {
        int Left = index * 2 + 1;
        return Left;
    }

    /// <summary>
    /// method to retrieve the parent at a specific node in the binary heap
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
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

    /// <summary>
    /// method to retrieve the right child at a specific node in the binary heap
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private int Right(int index)
    {
        int Right = index * 2 + 2;
        return Right;
    }

    /// <summary>
    /// method that maintaings the max heap property whenever a new element is added to the list
    /// </summary>
    /// <param name="queue">the current state of the queue</param>
    /// <param name="index">tells us specifically the node to apply the max heap property </param>
    private void MaxHeapify(List<InvertedIndex> queue, int index)
    {

        int leftChild = Left(index);
        int rightChild = Right(index);
        int largest = index;

        //if there exists a left child and a right childe
        if (leftChild <= queue.Count - 1 && rightChild <= queue.Count - 1)
        {
            //compare the left child node value, save the left child index to swap child and parent
            if (leftChild <= queue.Count - 1 && queue[leftChild].GetRank() > queue[index].GetRank())
                largest = leftChild;

            // compare the right child node value, save the right child index to swap child and parent
            if (rightChild <= queue.Count - 1 && queue[rightChild].GetRank() > queue[largest].GetRank())
                largest = rightChild;
        }
        // if only a left child exists
        else if (leftChild <= queue.Count - 1 && rightChild > queue.Count - 1)
        {
            //store child value if child value is greater than parent value to swap after
            if (queue[leftChild].GetRank() > queue[index].GetRank())
                largest = leftChild;
        }
        // if only a right child exists
        else if (rightChild <= queue.Count && leftChild > queue.Count)
        {
            //store child value if child value is greater than parent value to swap after
            if (queue[rightChild].GetRank() > queue[index].GetRank())
                largest = rightChild;
        }

        //swap the parent and the greate child value
        if (largest != index)
        {
            InvertedIndex temp = queue[index];
            queue[index] = queue[largest];
            queue[largest] = temp;

            //recursion call to make sure higher values float towards the top
            MaxHeapify(queue, largest);
        }
    }


    /// <summary>
    /// add a new object to the priority queue 
    /// </summary>
    /// <param name="rank"></param>
    /// <param name="docId"></param>
    public void MaxHeapInsert(double value, int docId)
    {
        InvertedIndex element = new InvertedIndex(value, docId);

        //get current state of priority queue
        List<InvertedIndex> tempQueue = this.priorityQueue;

        //add new element to the list
        tempQueue.Add(element);

        //need to maxheapify through half the elements in the PQ to maintain max heap property
        for (int i = tempQueue.Count / 2; i >= 0; i--)
        {
            MaxHeapify(tempQueue, i);
        }

        //update current state of the priority queue
        this.priorityQueue = tempQueue;

    }



    public void MaxHeapInsert(int value, int docId)
    {
        InvertedIndex element = new InvertedIndex(value, docId);

        //get current state of priority queue
        List<InvertedIndex> tempQueue = this.priorityQueue;

        //add new element to the list
        tempQueue.Add(element);

        //need to maxheapify through half the elements in the PQ to maintain max heap property
        for (int i = tempQueue.Count / 2; i >= 0; i--)
        {
            MaxHeapify(tempQueue, i);
        }

        //update current state of the priority queue
        this.priorityQueue = tempQueue;

    }

    public void MaxHeapInsert(double value, Tuple<int, int> tuple)
    {
        InvertedIndex element = new InvertedIndex(value, tuple);

        //get current state of priority queue
        List<InvertedIndex> tempQueue = this.priorityQueue;

        //add new element to the list
        tempQueue.Add(element);


        //need to maxheapify through half the elements in the PQ to maintain max heap property
        for (int i = tempQueue.Count / 2; i >= 0; i--)
        {
            MaxHeapify(tempQueue, i);
        }

        //update current state of the priority queue
        this.priorityQueue = tempQueue;

    }


    /// <summary>
    /// extracts from the priority queue the top ten documents
    /// </summary>
    /// <returns>List of (rank, docid) of top ten documents.</returns>
    public List<InvertedIndex> RetrieveTopTen()
    {
        List<InvertedIndex> priorityQueue = this.priorityQueue;

        // Console.WriteLine("In Retrieve Top Ten for Priority Queue------------------------------------");
        // foreach (MaxPriorityQueue.InvertedIndex item in priorityQueue)
        // {
        //     Console.WriteLine("Document ID: " + item.GetTuple().Item1);
        //     Console.WriteLine("From Tier: " + item.GetTuple().Item2);
        // }




        List<InvertedIndex> topTen = new List<InvertedIndex>();

        while (topTen.Count < 20)
        {
            if (priorityQueue.Count == 0)
            {
                break;
            }
            else
            {
                InvertedIndex max = ExtractMax(priorityQueue);
                topTen.Add(max);
            }
        }

        // Console.WriteLine("In Retrieve Top Ten ------------------------------------");
        // foreach (MaxPriorityQueue.InvertedIndex item in topTen)
        // {
        //     Console.WriteLine("Document ID: " + item.GetTuple().Item1);
        //     Console.WriteLine("From Tier: " + item.GetTuple().Item2);
        // }

        priorityQueue.Clear();
        return topTen;

    }



    /// <summary>
    /// extracts from the priority queue the top documents within a certain percent 
    /// </summary>
    /// <param name="percentOfDocuments"> integer representing percent of documents we need </param>
    /// <returns>List of (rank, docid) of top 20 documents.</returns>
    public List<InvertedIndex> RetrieveTier(double percentOfDocuments)
    {
        List<InvertedIndex> topDocuments = new List<InvertedIndex>();
        double limit = Math.Floor((percentOfDocuments * this.priorityQueue.Count) / 100);

        if (limit <= 1)
        {
            while (this.priorityQueue.Count > 0)
            {
                InvertedIndex max = ExtractMax(this.priorityQueue);
                topDocuments.Add(max);
            }
        }
        else
        {
            //floor the division so that we don't overestimate... underestimating is okay
            while (topDocuments.Count < limit)
            {
                if (priorityQueue.Count == 0)
                {
                    break;
                }
                else
                {
                    InvertedIndex max = ExtractMax(this.priorityQueue);
                    topDocuments.Add(max);
                }
            }
        }

        //we only want to clear the queue once we have pulled every item from the queue to create the tiers
        if (priorityQueue.Count == 0)
        {
            priorityQueue.Clear();
        }


        //prepares the tier to be stored onto disk by sorting according to ascending order of document ID, needed for encoding
        topDocuments.Sort(delegate (MaxPriorityQueue.InvertedIndex x, MaxPriorityQueue.InvertedIndex y)
        {
            return x.GetDocumentId().CompareTo(y.GetDocumentId());
        });
        return topDocuments;



    }


    /// <summary>
    /// extracts the highest value and then maintains the max heap property
    /// </summary>
    /// <param name="queue"></param>
    /// <returns></returns>
    private InvertedIndex ExtractMax(List<InvertedIndex> queue)
    {
        if (queue.Count < 1)
            Console.WriteLine("ERROR: Underflow");
        InvertedIndex max = queue[0];
        //swaps first and last value
        queue[0] = queue[queue.Count - 1];

        //removes the max value from the priority queue
        queue.RemoveAt(queue.Count - 1);

        //reorders the priority queue in order to maitain max heap property
        MaxHeapify(queue, 0);

        //returns extracted max object
        return max;

    }

    /// <summary>
    /// Get the instance of priority queue
    /// </summary>
    /// <returns>Priority Queue</returns>
    public List<InvertedIndex> GetPriorityQueue()
    {
        return this.priorityQueue;
    }

    public void ClearHeap()
    {
        this.priorityQueue.Clear();
    }

    public int CompareObjectByDocId(MaxPriorityQueue.InvertedIndex x, MaxPriorityQueue.InvertedIndex y)
    {

        int retval = x.GetDocumentId().CompareTo(y.GetDocumentId());
        return retval;
    }


}





