
using System;


namespace Search.Index
{    public class Default : IRankVariant
    {
        public double calculateQuery2TermWeight(int docFrequency, int corpusSize)
        {
            return Math.Log(1 + (double)corpusSize / docFrequency);
        }

        public double calculateDoc2TermWeight(double termFrequency, int docID, int corpusSize, IIndex index)
        {
            return (double)(1 + Math.Log(termFrequency));
        }

        public double GetDocumentWeight(int docID, IIndex index)
        {
            DiskPositionalIndex.PostingDocWeight temp = index.GetPostingDocWeight(docID);
            return temp.GetDocWeight();
        }

    }


    public class Tf_Idf : IRankVariant
    {
        public double calculateQuery2TermWeight(int docFrequency, int corpusSize)
        {
            return Math.Log((double)corpusSize / docFrequency);
        }
        public double calculateDoc2TermWeight(double termFrequency, int docID, int corpusSize, IIndex index)
        {
            return termFrequency;
        }


        public double GetDocumentWeight(int docID, IIndex index)
        {
            double docWeight;
            DiskPositionalIndex.PostingDocWeight temp = index.GetPostingDocWeight(docID);
            docWeight = temp.GetDocWeight();
            return docWeight;

        }
    }

    public class Okapi : IRankVariant
    {
        public double calculateQuery2TermWeight(int docFrequency, int corpusSize)
        {
            double OkapiWqtValue = Math.Log((double)(corpusSize - docFrequency + 0.5) / (docFrequency + 0.5));
            if (0.1 > OkapiWqtValue)
            {
                return 0.1;
            }
            else
                return OkapiWqtValue;
        }
        public double calculateDoc2TermWeight(double termFrequency, int docID, int corpusSize, IIndex index)
        {

            DiskPositionalIndex.PostingDocWeight temp = index.GetPostingDocWeight(docID);
            int documentLength = temp.GetDocTokenCount();
            double numeratorO = 2.2 * termFrequency;
            double denominatorO = 1.2 * (0.25 + 0.75 * (double)(documentLength / Indexer.averageDocLength)) + termFrequency;
            double OkapiWdtValue = (double)numeratorO / denominatorO;
            return OkapiWdtValue;
        }


        public double GetDocumentWeight(int docID, IIndex index)
        {
            return 1.0;
        }

    }

    public class Wacky : IRankVariant
    {
        public double calculateQuery2TermWeight(int docFrequency, int corpusSize)
        {
            int numerator = corpusSize - docFrequency;

            double division = (double)numerator / docFrequency;
            if (division > 1)
            {
                double WackyWqtValue = Math.Log(division);
                return WackyWqtValue;

            }
            else
                return 0.0;
        }

        public double calculateDoc2TermWeight(double termFrequency, int docID, int corpusSize, IIndex index)
        {

            DiskPositionalIndex.PostingDocWeight temp = index.GetPostingDocWeight(docID);

            
            double avDocTermFreq = temp.GetDocAveTermFreq();
            double numeratorW = (double)1 + Math.Log(termFrequency);
            double denominatorW = (double)1 + Math.Log(avDocTermFreq);
            double WackyWdtValue = (double)numeratorW / denominatorW;
            return WackyWdtValue;
        }


        public double GetDocumentWeight(int docID, IIndex index)
        {
            DiskPositionalIndex.PostingDocWeight temp = index.GetPostingDocWeight(docID);
            int fileSizeInByte = temp.GetDocByteSize();
            double WackyLd = (double)(Math.Sqrt(fileSizeInByte));
            //Console.WriteLine(WackyLd);
            return WackyLd;

        }
    }

}