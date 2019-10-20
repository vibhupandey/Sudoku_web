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
using Suduko.Models;  

namespace Suduko.Controllers
{
    public class HomeController : Controller
    {
        private SudukoModel dataq = new SudukoModel();
        private int a = 0;
        public IActionResult Index(String  id , string session)
        {
            if (string.IsNullOrEmpty(id))
            {
                var getfile = getAllSudukoFiles(""); 
                ViewBag.Locations = getfile;
                dataq = ConvertToSudocoTable(FetchDatafromFile(getfile[0].Value)); 
                
            }
            else  
            { 
                var getfile = getAllSudukoFiles(HttpUtility.UrlDecode(session));
                 
                ViewBag.Locations = getfile;
                dataq = JsonConvert.DeserializeObject<SudukoModel>(id)  ;
            }
                return View(dataq);
        }

        [NonAction]
        protected int [] FetchDatafromFile(string filePath)
        { 
            string text = System.IO.File.ReadAllText(filePath);
            text = text.Replace("\r\n", ""); 
            text = text.Replace("{", "").Replace("}", "");
            text = text.Trim();
            int [] input = text.Split(',').Select(x => int.Parse(x)).ToArray<int>();
            return input;
        }
        [NonAction]
        protected SudukoModel ConvertToSudocoTable(int [] ProblemArray)
        {
            var data = new SudukoModel();
            data.SudData = new int[9][];//Row Declaration.  
            for (var i = 0; i < data.SudData.Length; i++)
            {
                data.SudData[i] = new int[9];//Column Declaration
            }
            Func<int, int, int> FindCurrentCell = (Row, Column) => (Row * 9) + Column;
            //Push the values into the array.
            for (var i = 0; i < data.SudData.Length; i++)
            {
                for (var j = 0; j < data.SudData.Length; j++)
                {
                    data.SudData[i][j] = ProblemArray[FindCurrentCell(i, j)];
                }
            }
            return data;
        } 
        [HttpPost]
        public ActionResult Solve(SudukoModel data)
        { 
            Helper obj = new Helper(data); 
            dataq = null;
            dataq = obj.SolveSudoko(); 
            //return RedirectToAction("PopulateResult", dataresult);
            return RedirectToAction("Index",new { id= JsonConvert.SerializeObject(dataq), session = "" });
        } 
        public IActionResult indexchange(string value)
        {
            dataq=ConvertToSudocoTable(FetchDatafromFile(value));
            HttpContext.Session.SetString("indexchanged", value);
            return RedirectToAction("Index", new { id = JsonConvert.SerializeObject(dataq), session=value });
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private List<SelectListItem> getAllSudukoFiles( string  q)
        {
            List<SelectListItem> ObjList = new List<SelectListItem>()
            { 
            };
            string[] Documents = System.IO.Directory.GetFiles("./SudokoData/");

            for (int i = 0; i < Documents.Length; i++)
            {
                SelectListItem t = null;

                if (i == 0)
                {
                    if (string.IsNullOrEmpty(q))
                    {
                        t = new SelectListItem { Text = Path.GetFileName(Documents[i].ToString()), Value = Documents[i].ToString(), Selected = true };
                    }
                }
                if (Documents[i].ToString() == q)
                {
                    t = new SelectListItem {  Text =Path.GetFileName(Documents[i].ToString()), Value = Documents[i].ToString(), Selected = true};
                }
                else
                {
                    t = new SelectListItem { Text = Path.GetFileName(Documents[i].ToString()), Value = Documents[i].ToString() };
                }
                ObjList.Add(t);
            } 
            return ObjList;

        }


    }
}
