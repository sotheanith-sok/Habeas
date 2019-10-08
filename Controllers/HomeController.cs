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

        BackendProgram BEP = new BackendProgram();

        public IActionResult Index()
        {
            if (HybridSupport.IsElectronActive)
            {
                Electron.IpcMain.On("select-directory", async (args) => {
                    var mainWindow = Electron.WindowManager.BrowserWindows.First();
                    var options = new OpenDialogOptions
                    {
                        Properties = new OpenDialogProperty[] {
                        OpenDialogProperty.openFile,
                        OpenDialogProperty.openDirectory
                    }
                    };

                    string[] files = await Electron.Dialog.ShowOpenDialogAsync(mainWindow, options);
                    
                    string check = String.Join("", files);
                    

                    if (BEP.PathIsValid(check) && BEP.PathContainsContent(check))
                    {

                        BEP.GenerateIndex(check);
                        Electron.IpcMain.Send(mainWindow, "select-directory-reply", "true");

                    }
                    else if (!BEP.PathIsValid(check))
                    {

                        Electron.IpcMain.Send(mainWindow, "select-directory-reply", "invalidPath");
                    }
                    else {

                        Electron.IpcMain.Send(mainWindow, "select-directory-reply", "emptyFile");
                    }

                });

                Electron.IpcMain.On("stemTerm", (args) =>
                {
                    var mainWindow = Electron.WindowManager.BrowserWindows.First();
                    string term = args.ToString();
                    string stemmedTerm = BEP.termStemmer(term);
                    Electron.IpcMain.Send(mainWindow, "stemmedTerm", stemmedTerm);
                });

                Electron.IpcMain.On("info-dialog", async (args) =>
                {
                    var options = new MessageBoxOptions(
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

                    var result = await Electron.Dialog.ShowMessageBoxAsync(options);

                    var mainWindow = Electron.WindowManager.BrowserWindows.First();
                    Electron.IpcMain.Send(mainWindow, "information-dialog-reply", result.Response);
                });

                

                Electron.IpcMain.On("chooseVocab", (args) =>
                {
                    List<String> result = BEP.PrintVocab(1000);
                    var mainWindow = Electron.WindowManager.BrowserWindows.First();
                    Electron.IpcMain.Send(mainWindow, "vocabList", result);
                });

                Electron.IpcMain.On("searchText", (args) =>
                {
                    var mainWindow = Electron.WindowManager.BrowserWindows.First();
                    string term = args.ToString();
                    List<string> Postings = BEP.searchTerm(term);
                    Electron.IpcMain.Send(mainWindow, "searchText", Postings);
                });

                Electron.IpcMain.On("soundexText", (args) =>
                {
                    var mainWindow = Electron.WindowManager.BrowserWindows.First();
                    string term = args.ToString();
                    List<string> Postings = BEP.soundexTerm(term);
                    Electron.IpcMain.Send(mainWindow, "soundexText", Postings);
                });

                Electron.IpcMain.On("readDoc", (args) =>
                {
                    string term = args.ToString();
                    string Content = BEP.getDocContent(term);
                    var options = new MessageBoxOptions(
                        Content
                        )
                    {
                        Type = MessageBoxType.none,
                        Title = BEP.getDocTitle(term)
                    };

                    var result = Electron.Dialog.ShowMessageBoxAsync(options);

                    var mainWindow = Electron.WindowManager.BrowserWindows.First();

                });

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
