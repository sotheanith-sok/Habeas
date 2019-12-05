using System.Collections.Generic;
using System;
using System.Linq;
using Search.Index;
using Search.Document;
using System.Diagnostics;


namespace Metrics.MeanAveragePrecision
{
    public class MeanAveragePrecision
    {
        private static string corpusPath = "../../../corpus/Cranfield/";
        private static string queryFilePath = corpusPath + "relevance/~queries";
        private static string qrelFilePath = corpusPath + "relevance/qrel";
        private BackendProgram BEP;

        public MeanAveragePrecision()
        {
            BEP = new BackendProgram();
            BEP.GetIndex(corpusPath);

        }

        /// <summary>
        /// Gets MeanAveragePrecision for 'Cranfield' corpus
        /// </summary>
        /// <returns>MAP value</returns>
        public float GetMAP()
        {
            List<string> queries = ReadStringList(queryFilePath);
            List<List<int>> relevances = ReadIntList(qrelFilePath);

            List<List<int>> retrievals = new List<List<int>>();
             long sum = 0;

            IList<MaxPriorityQueue.InvertedIndex> topDocs;
            foreach (string query in queries)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                topDocs= BEP.SearchRankedRetrieval(query);
                stopwatch.Stop();
                sum += stopwatch.ElapsedMilliseconds;

                retrievals.Add(ConvertRankedResult(topDocs));
            }

            Console.WriteLine((double)sum / queries.Count);

            float meanAP = CalculateMAP(retrievals, relevances);
            Console.WriteLine("MAP: " + meanAP);
            return meanAP;
        }

        /// <summary>
        /// Converts the result from ranked retrieval to list of integer(fileName) for Cranfield corpus
        /// </summary>
        /// <param name="topDocs"></param>
        /// <returns></returns>
        private List<int> ConvertRankedResult(IList<MaxPriorityQueue.InvertedIndex> topDocs)
        {
            List<int> list = new List<int>();

            foreach (var p in topDocs)
            {
                int docId = p.GetDocumentId();
                IDocument doc = BackendProgram.corpus.GetDocument(docId);
                string fileName = ((IFileDocument)doc).FileName;
                //Removes leading '0's in the file name
                fileName = fileName.TrimStart('0');
                //Removes '.json'
                try
                {
                    list.Add(Int32.Parse(fileName));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

            }

            return list;
        }

        /// <summary>
        /// Calculates MeanAveragePrecision of the query results.
        /// </summary>
        /// <param name="results">list of all query results</param>
        /// <param name="actuals">list of actual relevances for all queries</param>
        /// <returns></returns>
        public float CalculateMAP(List<List<int>> results, List<List<int>> actuals)
        {
            float sumaps = 0;
            for (int i = 0; i < results.Count; i++)
            {
                sumaps += CalculateAP(results[i], actuals[i]);
            }
            return sumaps / results.Count;
        }

        /// <summary>
        /// Calculates AveragePrecision with the query result and actual relevance
        /// </summary>
        /// <param name="result"></param>
        /// <param name="actual"></param>
        public float CalculateAP(List<int> result, List<int> actual)
        {
            int totalRelevant = 0;
            List<float> pks = new List<float>();
            float sumpks = 0;

            for (int i = 0; i < result.Count; i++)
            {
                if (actual.Contains(result[i]))
                {
                    totalRelevant++;
                    sumpks += totalRelevant / (i + 1);
                }
            }
            return sumpks / actual.Count;
        }

        /// <summary>
        /// Reads the queries to test from file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public List<string> ReadStringList(string fileName)
        {
            int counter = 0;
            string line;


            System.IO.StreamReader queryFile = new System.IO.StreamReader(fileName);
            List<String> s = new List<string>();
            while ((line = queryFile.ReadLine()) != null)
            {
                counter++;
                s.Add(line);
            }

            queryFile.Close();
            System.Console.WriteLine("There were {0} lines.", counter);
            return s;
        }

        /// <summary>
        /// Reads the relevances(list of file names) for queries from file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public List<List<int>> ReadIntList(string fileName)
        {
            int counter = 0;
            string line;
            List<List<int>> listOfRelevanceResults = new List<List<int>>();

            System.IO.StreamReader queryFile = new System.IO.StreamReader(fileName);
            List<int> i = new List<int>();
            while ((line = queryFile.ReadLine()) != null)
            {
                List<int> relevanceJudgements = line.Split(' ').Select(Int32.Parse).ToList();
                listOfRelevanceResults.Add(relevanceJudgements);
            }

            queryFile.Close();
            System.Console.WriteLine("There were {0} lines.", counter);
            return listOfRelevanceResults;
        }



        public Stopwatch TestSpeed(string query)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            BEP.SearchRankedRetrieval(query);
            stopwatch.Stop();

            return stopwatch;
        }

        public double GetAverageSpeed()
        {
            long sum = 0;
            List<string> queries = ReadStringList(queryFilePath);
            foreach (string query in queries)
            {
                Stopwatch stopwatch = TestSpeed(query);
                sum += stopwatch.ElapsedMilliseconds;
            }

            return ((double)sum / queries.Count);
        }

    }
}