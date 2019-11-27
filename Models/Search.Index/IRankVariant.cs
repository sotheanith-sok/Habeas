using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using Search.Text;
using Search.Document;
using Search.Query;

namespace Search.Index
{
    public interface IRankVariant
    {

        double calculateQuery2TermWeight(int docFrequency, int corpusSize);
       

        double calculateDoc2TermWeight(int termFrequency, int docID, int corpusSize, IIndex index);

        double GetDocumentWeight(int docID, IIndex index);
        
    }


}
