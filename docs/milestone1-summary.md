CECS 529 Search Engine Technology
### Milestone1 :thumbsup:
Demo: Oct 8 (Tue)  
For more detailed description on requirements, check [milestone1.pdf](milestone1.pdf)

# Features and Their Requirements
  
[Core Features](#core-modules)  
- [x] 1. Corpus to handle Json
- [x] 2. Building positional index
- [x] 3. Indexing and Tokenization
- [x] 4. Query Language
- [x] 5. Main Application
- [x] 6. JsonFileDocument

[Additional Features](#additional-features)  
- [x] 1. Unit Testing 
- [x] 2. GUI 
- [x] 3. Wildcard Queries 
- [x] 4. SoundEx 
- [x] 5. Near Operator



## Core Modules
### 1. Corpus
- all -nps-sites.json corpus
- Split into individual documents
- Need to support multiple corpora in the search to enter p folder/directory to index
- Index should be built in that folder
- **Modify the DirectoryCorpus** to know how to load JSON document

### 2. Building Positional Index
- Positional Inverted Index() 
  - Postings list needs
    - Needs documentId 
    - List of of positions
  - (documentID, [position1, position2])
- Implemented from **IIndex**
- Update Postings class to have list of positions
- Modify **addPosting** to account for the position of the term within the document

### 3. Indexing and Tokenization
- **Tokenize using EnglishTokenStream class**
- To Normalize token to term -- Write a new **TokenProcessor** that has the following rules DON'T modify BasicTokenProcess write a new implementation
  - Remove all non-alphanumeric characters from the beginning and end of the token but not the middle
  - Remove all apostrophes
  - Remove quotation marks
  - Hyphenated Words:
    - Remove hyphens from the token and then proceed with the modified token
    - Split hyphenated word
      - Create multiple tokens 
        - Example HewlettPackardComputing, Hewleett, Packard, and Computing
  - Convert the token to lowercase
  - Stemp the token using Porter2 stemmer

### 4. Query Language
- Update BooleanQueryParser()
  - Needs to read phrase literals correctly
    - Update findNextLiteral to recognize and construct PhraselLiteral objects
  - Only handles individual terms
- After parsing phrase literals
  - Update getPostings() for And Query, orQuery, and PhraseLiteral
    - Incorporate TokenProcessor into getPostings()
- Integrate BooleanQueryParser() into your application
- Special queries
  - Refer to handout
  - Need to implement architecture for special queries

### 5. Main Application
- Ask user for the name of the directory
  - Construct DirectoryCorpus from that directory
- Index All documents in the corpus
  - PRINT how long this process takes
- Loop:
  - Ask for a search query
    - If it is a special query , perform that action
    - If it is not, then parse the query and retrieve its postings
      - Output: 
        - The name of the documents returned from the query one per line
        - The number of documents returned from the query, after the document names
        - Ask the user if they would like to select a document to view. If the user selects a document to view, print the entire content of the document to the screen

### 6. JsonFileDocuments
- Create new implementation of FileDocument(JsonFileDocument) to work with json files
  - New getContent()
  - New getTitle()



## Additional Features
Listed in Priority 

### 1. Unit Testing 
- on Positional Index
- on Query language
- on other modules

### 2. GUI 
..

### 3. Wildcard Queries 
..

### 4. SoundEx 
- `SoundexIndex` class
- hashMap [sound hash key] -> [docIDs]
- `Author` field in `IDocument` and its implementations
- author query `:author ______` handled in Main()

### 5. Near Operator
- NearLiteral class
- Implement GetPostings()
- Upgrade findNextLiteral() to recognize NEAR query
- unit test on it
#### Near query example:  
`[angels NEAR/2 baseball]`  
To find documents that `baseball` appears at most 2 positions away from `angels`.  
`angels baseball` - match  
`angels love baseball` - match  
`angels really love baseball` - not match  