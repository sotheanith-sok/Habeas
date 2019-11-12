using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Habeas.Models;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Search.Index;

namespace Habeas.Controllers
{
    public class HomeController : Controller
    {
        //create instance of BackendProgram
        //BackendProgram controls every part of the model
        BackendProgram BEP = new BackendProgram();

        public IActionResult Index()
        {
            try
            {
                if (HybridSupport.IsElectronActive)
                {

                    //Responds to user selecting a directory
                    Electron.IpcMain.On("select-directory", async (args) =>
                    {
                        var mainWindow = Electron.WindowManager.BrowserWindows.First();
                        //creates a seperate window for choosing the directory path
                        var options = new OpenDialogOptions
                        {
                            Properties = new OpenDialogProperty[] {
                        OpenDialogProperty.openFile,
                        OpenDialogProperty.openDirectory
                        }
                        };
                        //the result is saved as a string[]
                        string[] files = await Electron.Dialog.ShowOpenDialogAsync(mainWindow, options);
                        //the result is then converted to a string
                        string path = String.Join("", files);
                        //if the path is valid and contains content...
                        if (BEP.CheckIfPathValid(path) && BEP.CheckIfPathContainsContent(path))
                        {
                            //index it, it's the new corpus
                            BEP.GetIndex(path);
                            //send message to the view
                            Electron.IpcMain.Send(mainWindow, "select-directory-reply", "true");

                        }
                        //otherwise, if the path does not exist...
                        else if (!BEP.CheckIfPathValid(path))
                        {
                            //send a message to the view
                            Electron.IpcMain.Send(mainWindow, "select-directory-reply", "invalidPath");
                        }
                        else
                        {
                            //sends a message to the view
                            Electron.IpcMain.Send(mainWindow, "select-directory-reply", "emptyFile");
                        }
                    });

                    //Responds to user choosing to stem a term
                    Electron.IpcMain.On("stemTerm", (args) =>
                    {
                        var mainWindow = Electron.WindowManager.BrowserWindows.First();
                        //turns argument into a string
                        string term = args.ToString();
                        //stems the argument
                        string stemmedTerm = BEP.StemTerm(term);
                        //sends result back to the view
                        Electron.IpcMain.Send(mainWindow, "stemmedTerm", stemmedTerm);
                    });

                    //Responds to user desiring to get info on Habeas
                    Electron.IpcMain.On("info-dialog", async (args) =>
                    {
                        //set details of message box
                        var options = new MessageBoxOptions
                            (
                                "Habeas supports" + Environment.NewLine + Environment.NewLine +
                                "1. single query" + Environment.NewLine +
                                "2. boolean query -> put a space between keywords for AND queries, put a '+' for OR queries" + Environment.NewLine +
                                "3. phrase query -> \"term1 term2...\"" + Environment.NewLine +
                                "4. near query -> [term1 NEAR/k term2]" + Environment.NewLine +
                                "5. wildcard query -> colo*r" + Environment.NewLine +
                                "6. soundex for author name" + Environment.NewLine + Environment.NewLine +
                                "To STEM a word, select 'stem' on the dropdown to the left of the search field. Then type the word you wish to stem into the search field and click the enter button." + Environment.NewLine + Environment.NewLine + "To view the VOCAB list for an index, click the 'Vocab' label on the navigation bar." + Environment.NewLine + Environment.NewLine + "To INDEX a new directory, click the 'Index' label on the navigation bar."
                            )
                        {
                            Type = MessageBoxType.info,
                            Title = "Information"
                        };
                        //creates message box from options
                        var result = await Electron.Dialog.ShowMessageBoxAsync(options);
                        var mainWindow = Electron.WindowManager.BrowserWindows.First();
                        //sends message box to the main window
                        Electron.IpcMain.Send(mainWindow, "information-dialog-reply", result.Response);
                    });

                    //Responds to user attempting to get the vocab list from the corpus
                    Electron.IpcMain.On("chooseVocab", (args) =>
                    {
                        //has backend get 1000 vocab terms
                        List<String> result = BEP.PrintVocab(1000);
                        var mainWindow = Electron.WindowManager.BrowserWindows.First();
                        //send list to view
                        Electron.IpcMain.Send(mainWindow, "vocabList", result);
                    });

                    //Responds to user searching the corpus
                    Electron.IpcMain.On("searchText", (args) =>
                    {
                        var mainWindow = Electron.WindowManager.BrowserWindows.First();
                        //terms argument into string
                        string term = args.ToString();
                        //gets list of posting titles from backend
                        List<string> Postings = BEP.SearchQuery(term);
                        //sends results to view
                        Electron.IpcMain.Send(mainWindow, "searchText", Postings);
                    });

                    //Responds to user doing a soundex search
                    Electron.IpcMain.On("soundexText", (args) =>
                    {
                        var mainWindow = Electron.WindowManager.BrowserWindows.First();
                        //turns argument into string
                        string term = args.ToString();
                        //gets list of strings from backend
                        List<string> Postings = BEP.SearchSoundexQuery(term);
                        //sends results to view
                        Electron.IpcMain.Send(mainWindow, "soundexText", Postings);
                    });

                    //
                    Electron.IpcMain.On("RetType", async (args) =>
                    {
                        //set details of message box
                        var options = new MessageBoxOptions
                            (
                                "Select Ranked Retrieval type" + Environment.NewLine
                            )
                        {
                            Type = MessageBoxType.info,
                            Title = "RetrievalType",
                            Buttons = new string[] { "Default", "Tf-Idf", "Okapi", "Wacky" }
                        };
                        //creates message box from options
                        var result = await Electron.Dialog.ShowMessageBoxAsync(options);
                        switch (result.Response)
                        {
                            case 0:
                                BEP.selectRetrieval("Default");
                                break;
                            case 1:
                                BEP.selectRetrieval("Tf-idf");
                                break;
                            case 2:
                                BEP.selectRetrieval("Okapi");
                                break;
                            case 3:
                                BEP.selectRetrieval("Wacky");
                                break;
                        }
                    });

                    //Responds to user attempting to read a document
                    Electron.IpcMain.On("readDoc", (args) =>
                    {
                        //argument is turned into string
                        string term = args.ToString();
                        //content of document retrieved from backend
                        string Content = BEP.GetDocContent(term);
                        //creates message box out of document content
                        var options = new MessageBoxOptions(
                                Content
                                )
                        {
                            Type = MessageBoxType.none,
                            Title = BEP.GetDocTitle(term)
                        };
                        var result = Electron.Dialog.ShowMessageBoxAsync(options);
                        var mainWindow = Electron.WindowManager.BrowserWindows.First();

                    });

                    Electron.IpcMain.On("modeSwitch", (args) =>
                    {
                        BEP.switchMode();
                    });

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
