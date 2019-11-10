
	const { ipcRenderer } = require("electron");
	var dropdown = document.getElementById("searchChoice");
	var stemInput = document.getElementById("stemText");
	var searchInput = document.getElementById("searchText");
	var soundexInput = document.getElementById("soundexText");
	var mainHeading= document.getElementById("titleHeading");
	var enterButton = document.getElementById("search");
	var vocabDIV = document.getElementById("vocabSection");
	var stemDIV = document.getElementById("stemmedTerm");
	var vocabList = document.getElementById("vocabContainer");
	var enterButtonDIV = document.getElementById("enterDIV");
	var searchLoading = document.getElementById("loading");
	var noPathWarning = document.getElementById("NoPath");
	var emptyPathWarning = document.getElementById("EmptyFile");
	var selectDirectoryDIV = document.getElementById("walla");
	var indexingSpan = document.getElementById("indexing");
	var chooseDirectoryDIV = document.getElementById("indicatePath");
	var directorySelecter = document.getElementById("select-directory");
	var searchDirectoryDIV = document.getElementById("searchDirectory");
	var stemmedTerm = document.getElementById("sTerm");
	var originalTerm = document.getElementById("originalTerm");
	var vocabTotal = document.getElementById("vocabTotal");
	var noResultsMessage = "Sorry, your query did not produce any results.¯\\_(ツ)_/¯";
	
	//Sending messages

	//user decides to see the corpus' vocab
	document.getElementById("chooseVocab").addEventListener("click", () => {
		stemDIV.style.display = "none";
		//sends message to the controller
		ipcRenderer.send("chooseVocab");
	});
	
	//user decides to read a document
	document.addEventListener('click', function (e) {
		//if user clicks an article span...
		if (e.target && e.target.class == 'articles') {
			//send message to the view
			ipcRenderer.send("readDoc", e.target.id);
		}
	});

	//user changes the dropdown option
	dropdown.addEventListener("change", (sender, path) => {
		stemInput.value = "";
		searchInput.value = "";
		//if user chooses to search the corpus...
		if (dropdown.value == "searchOption") {
			stemInput.style.display = "none";
			soundexInput.style.display = "none";
			searchInput.style.display = "inline-block";
			mainHeading.innerHTML = "Search Corpus";
		}
		//if user chooses to stem...
		else if (dropdown.value == "stem") {
			stemInput.style.display = "inline-block";
			searchInput.style.display = "none";
			soundexInput.style.display = "none";
			mainHeading.innerHTML = "Stem Term";
		}
		//if the user chooses to do the soundex search
		else {
			mainHeading.innerHTML = "Search Corpus";
			soundexInput.style.display = "inline-block";
			stemInput.style.display = "none";
			searchInput.style.display = "none";
		}
	});

	//user clicks the enter button
	enterButton.addEventListener("click", (sender, path) => {
		vocabDIV.style.display = "none";
		stemDIV.style.display = "none";
		vocabList.innerHTML = "";
		//if the dropdown is set to search
		if (dropdown.value == "searchOption") {
			//if the text input isn't empty
			if(searchInput.value != ""){
			enterButtonDIV.style.display = "none";
			//display loading text
			searchLoading.style.display = "block";
			//send message to controller
			ipcRenderer.send("searchText", searchInput.value);
			}
		}
		//if the dropdown is set to stem
		else if (dropdown.value == "stem") {
			//if the text input isn't empty
			if(stemInput.value != ""){
			//display original term
			originalTerm.innerHTML = stemInput.value + " -> ";
			//sends message to controller
			ipcRenderer.send("stemTerm", stemInput.value);
			}		
		}
		//the dropdown is set to soundex
		else {
			//assuming the text input isn't empty
			if(soundexInput.value != ""){
			enterButtonDIV.style.display = "none";
			//displays search text
			searchLoading.style.display = "block";
			//sends message to the controller
			ipcRenderer.send("soundexText", soundexInput.value);
			}
		}
	});

	//the user attempts to choose the directory path
	directorySelecter.addEventListener("click", () => {
		//clear user warnings
		noPathWarning.style.display = "none";
		emptyPathWarning.style.display = "none";
		//remove select directory button
		selectDirectoryDIV.style.display = "none";
		//display indexing text
		indexingSpan.style.display = "block";
		chooseDirectoryDIV.class = "text-center";
		//send message to the controller
		ipcRenderer.send("select-directory");
	});

	//if user chooses to change the corpus to search
	document.getElementById("headerIndex").addEventListener("click", () => {
		//displays DIV for choosing directory
		chooseDirectoryDIV.style.display = "block";
		//clears remaining elements of the GUI
		searchDirectoryDIV.style.display = "none";
		document.getElementById("headerIndex").style.display = "none";
		document.getElementById("chooseVocab").style.display = "none";
		vocabTotal.innerHTML = "";
		vocabContainer.innerHTML ="";
		originalTerm.innerHTML ="";
		sTerm.innerHTML ="";
		stemInput.value="";
		searchInput.value="";
		soundexInput.value="";
	});

	//getting messages from controller

	//controller comes back with the stemmed term
	ipcRenderer.on("stemmedTerm", (sender, path) => {
		stemDIV.style.display = "block"
		stemInput.value = "";
		//display the stemmed term
		stemmedTerm.innerHTML = path;
	});

	//controller comes back with the vocab list
	ipcRenderer.on("vocabList", (sender, vocab) => {
		//display vocab section
		vocabDIV.style.display = "block";
		//tell user total number of vocab terms in the corpus
		vocabTotal.innerHTML = "The total number of items in the vocabulary is: " + vocab[0];
		//clear vocab list on GUI
		vocabList.innerHTML = "";
		//print vocab to window
		for (i = 1; i <= 1000; i++) {
		var node = document.createElement("P");
		node.innerText = "#" + [i] + ") " +vocab[i];   
		vocabList.appendChild(node);
		}	
	});

	//controller comes back with soundex
	ipcRenderer.on("soundexText", (sender, posting) => {
		vocabDIV.style.display = "block";
		enterButtonDIV.style.display = "block";
		searchLoading.style.display = "none";
		//if nothing comes back from soundex, tell user
		if (posting[0] == "0") {
			vocabTotal.innerHTML = noResultsMessage;
		}
		//if something does come back from soundex search...
		else {
			var limit = posting[0];
			//tells user how many results have been found
			vocabTotal.innerHTML = "Habeas found " + limit + " results for your query";
			//prints results to window
			for (i = 1; i <= (limit * 2); i += 2) {
				//creates new element
				var node = document.createElement("a");
				//gives document name
				node.innerHTML = posting[i];
				//set element class
				node.class = "articles";
				//sets element id
				node.id = posting[i + 1];
				//on hover, cursor becomes pointer
				node.style.cursor = "pointer";
				//adds element to window
				vocabList.appendChild(node);
				var newElem = document.createElement("BR");
				vocabList.appendChild(newElem);
			}
		}
	});

	//returns results of query
	ipcRenderer.on("searchText", (sender, posting) => {
		vocabDIV.style.display = "block";
		enterButtonDIV.style.display = "block";
		searchLoading.style.display = "none";
		//if no results, inform user
		if (posting[0] == "0") {
			vocabTotal.innerHTML = noResultsMessage;
		}
		//if there are results
		else {
			//tell the user how many results have been found
			var limit = posting[0];
			vocabTotal.innerHTML = "Habeas found "+limit+" results for your query";
			//print results to the window
			for (i = 1; i <= (limit*2) ; i +=2) {
				//create element
				var node = document.createElement("a");
				//give name of document
				node.innerHTML = posting[i];
				//give element class
				node.class = "articles";
				//give element id
				node.id = posting[i+1];
				node.style.cursor = "pointer";
				//adds element to window
				vocabList.appendChild(node);
				var newElem = document.createElement("BR");
				vocabList.appendChild(newElem);
			}
		}
	});

	//controller returns with info about the chosen directory
	ipcRenderer.on("select-directory-reply", (sender, path) => {
		selectDirectoryDIV.style.display = "block";
		indexingSpan.style.display = "none";
		chooseDirectoryDIV.class = "text-center";
		//if the path is valid
		if (path == "true") {
			//clear error texts
			noPathWarning.style.display = "none";
			emptyPathWarning.style.display = "none";
			chooseDirectoryDIV.style.display = "none";
			//show search portion of GUI
			searchDirectoryDIV.style.display = "block";
			document.getElementById("headerIndex").style.display = "block";
			document.getElementById("chooseVocab").style.display = "block";
		}
		//if path does not exist...
		else if (path == "invalidPath") {
			//display message to user
			noPathWarning.style.display = "block";
			emptyPathWarning.style.display = "none";
		}
		//if path does not contain content...
		else if (path == "emptyFile") {
			//display message to user
			emptyPathWarning.style.display = "block";
			noPathWarning.style.display = "none";
		}
		else {
			noPathWarning.style.display = "none";
			emptyPathWarning.style.display = "none";
		}
	});