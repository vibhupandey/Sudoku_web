using Microsoft.AspNetCore.Mvc.Rendering;
using Suduko.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Suduko.BusinessLogic
{
    public class SudokuInitializer
    { 
        public int[] FetchDatafromFile(string filePath)
        {
            string text = System.IO.File.ReadAllText(filePath);
            text = text.Replace("\r\n", "");
            text = text.Replace("{", "").Replace("}", "");
            text = text.Trim();
            int[] input = text.Split(',').Select(x => int.Parse(x)).ToArray<int>();
            return input;
        }

        public SudukoModel ConvertToSudocoTable(int[] ProblemArray)
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
        public List<SelectListItem> getAllSudukoFiles(string selectedValue)
        {
            List<SelectListItem> ObjList = new List<SelectListItem>()
            {
            };
            string[] Documents = System.IO.Directory.GetFiles("./SudokoData/");

            for (int i = 0; i < Documents.Length; i++)
            {
                SelectListItem lstobj = null;

                if (i == 0)
                {
                    if (string.IsNullOrEmpty(selectedValue))
                    {
                        lstobj = new SelectListItem { Text = Path.GetFileName(Documents[i].ToString()), Value = Documents[i].ToString(), Selected = true };
                    }
                }
                if (Documents[i].ToString() == selectedValue)
                {
                    lstobj = new SelectListItem { Text = Path.GetFileName(Documents[i].ToString()), Value = Documents[i].ToString(), Selected = true };
                }
                else
                {
                    lstobj = new SelectListItem { Text = Path.GetFileName(Documents[i].ToString()), Value = Documents[i].ToString() };
                }
                ObjList.Add(lstobj);
            }
            return ObjList;

        }

    }
}
