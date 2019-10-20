using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Suduko.BusinessLogic;
using Suduko.Models;  

namespace Suduko.Controllers
{
    public class HomeController : Controller
    {
        private SudukoModel _sudukoModel = new SudukoModel();
        private SudokuInitializer _sudokuInitializer = new SudokuInitializer(); 
        public IActionResult Index(String  id , string session)
        {
            if (string.IsNullOrEmpty(id))
            {
                var getfile = _sudokuInitializer.getAllSudukoFiles(""); 
                ViewBag.Locations = getfile;
                _sudukoModel = _sudokuInitializer.ConvertToSudocoTable(_sudokuInitializer.FetchDatafromFile(getfile[0].Value)); 
                
            }
            else  
            { 
                var getfile = _sudokuInitializer.getAllSudukoFiles(HttpUtility.UrlDecode(session));                 
                ViewBag.Locations = getfile;
                _sudukoModel = JsonConvert.DeserializeObject<SudukoModel>(id)  ;
            }
          return View(_sudukoModel);
        }

        
        [HttpPost]
        public ActionResult Solve(SudukoModel data)
        { 
            Helper obj = new Helper(data); 
           _sudukoModel = obj.SolveSudoko(); 
           return RedirectToAction("Index",new { id= JsonConvert.SerializeObject(_sudukoModel), session = "" });
        } 
        public IActionResult indexchange(string value)
        {
            _sudukoModel= _sudokuInitializer.ConvertToSudocoTable(_sudokuInitializer.FetchDatafromFile(value)); 
            return RedirectToAction("Index", new { id = JsonConvert.SerializeObject(_sudukoModel), session=value });
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       

    }
}
