using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 

namespace Suduko.Models
{
    public class Helper
    {
        protected SudukoModel _sudukoModel { get; set; }
        public Helper (SudukoModel sudukomodel)
        {
            _sudukoModel = sudukomodel;

        }
        public SudukoModel SolveSudoko( )
        {
            int previousValue;
            int currentValue; 
            Func<int> SumOfSudokuValue = () => _sudukoModel.SudData.Sum(t => t.Sum());
            do
            {
                previousValue = SumOfSudokuValue();
                SolveArray();
                currentValue = SumOfSudokuValue();
            }
            while (previousValue != currentValue);
            return _sudukoModel;
        }
        private void SolveArray()
        {
            var list = new List<int>(); 
            for (var row = 0; row < _sudukoModel.SudData.Length; row++)
            {
                for (var column = 0; column < _sudukoModel.SudData.Length; column++)
                {
                    var currentValue = _sudukoModel.SudData[row][column];
                    if (currentValue > 0)
                        continue;
                    list.Clear();
                    for (var i = 1; i <= 9; i++)
                    {
                        if (CheckCurrentFullRow(row, column, i))
                        {
                            if (CheckCurrentFullColumn(row, column, i))
                            {
                                if (CheckCurrentFullSquare(row, column, i))
                                    list.Add(i);
                            }
                        }
                    }
                    if (list.Count() == 1)
                    {
                        _sudukoModel.SudData[row][column] = list[0];
                    }
                    else if (list.Count() > 1)
                    {
                        var posValue = CheckNeighbourValue(row, column, list);
                        if (posValue > 0) _sudukoModel.SudData[row][column] = posValue;
                    }
                }
            }
        }
        private bool CheckCurrentFullRow(int Row, int Column, int AssignValue)
        {
            for (var i = 0; i <= 8; i++)
            {
                if (_sudukoModel.SudData[Row][i] == AssignValue && Column != i)
                    return false;
            }
            return true;
        }
        private bool CheckCurrentFullColumn(int Row, int Column, int AssignValue)
        {
            for (var i = 0; i <= 8; i++)
            {
                if (_sudukoModel.SudData[i][Column] == AssignValue && i != Row)
                    return false;
            }
            return true;
        }
        private bool CheckCurrentFullSquare(int Row, int Column, int AssignValue)
        {
            var rowStart = (Row / 3) + 1;
            var rowIndexEnd = (rowStart * 3) - 1;
            if (rowIndexEnd == 0) rowIndexEnd = 3;
            var rowIndexStart = rowIndexEnd - 2;
            var columnStart = (Column / 3) + 1;
            var columnIndexEnd = (columnStart * 3) - 1;
            if (columnIndexEnd == 0) columnIndexEnd = 3;
            var columnIndexStart = columnIndexEnd - 2;
            for (var curRow = rowIndexStart; curRow <= rowIndexEnd; curRow++)
            {
                for (var curColumn = columnIndexStart; curColumn <= columnIndexEnd; curColumn++)
                {
                    if (_sudukoModel.SudData[curRow][curColumn] == AssignValue && curColumn != Column && curRow != Row)
                        return false;
                }

            }
            return true;
        }

        private int CheckNeighbourValue(int CurrentRow, int CurrentColumn, List<int> PossibleIntList)
        {
            var rowStart = (CurrentRow / 3) + 1;
            var rowIndexEnd = (rowStart * 3) - 1;
            if (rowIndexEnd == 0) rowIndexEnd = 3;
            var rowIndexStart = rowIndexEnd - 2;
            var columnStart = (CurrentColumn / 3) + 1;
            var columnIndexEnd = (columnStart * 3) - 1;
            if (columnIndexEnd == 0) columnIndexEnd = 3;
            var columnIndexStart = columnIndexEnd - 2;
            var applicableRows = new List<int>();
            var currentSquareRows = new List<int>();
            for (var curRow = rowIndexStart; curRow <= rowIndexEnd; curRow++)
            {
                currentSquareRows.Add(curRow);
                if (CurrentRow != curRow)
                {
                    applicableRows.Add(curRow);
                }
            }

            var applicableColumns = new List<int>();
            var currentSquareColumns = new List<int>();
            for (var curColumn = columnIndexStart; curColumn <= columnIndexEnd; curColumn++)
            {
                currentSquareColumns.Add(curColumn);
                if (CurrentColumn != curColumn)
                {
                    applicableColumns.Add(curColumn);
                }
            }
            var resultArray = new List<int>();
            for (int i = 0; i < PossibleIntList.Count(); i++)
            {
                var assignValue = PossibleIntList[i];
                var newIntArray = NewArray;
                //Copy all values from exisiting array
                for (var curRow = rowIndexStart; curRow <= rowIndexEnd; curRow++)
                {
                    for (var curColumn = columnIndexStart; curColumn <= columnIndexEnd; curColumn++)
                    {
                        if (curRow == CurrentRow && curColumn == CurrentColumn)
                            continue;
                        newIntArray[curRow][curColumn] = _sudukoModel.SudData[curRow][curColumn];
                    }
                }
                foreach (var applicableColumn in applicableColumns)
                {
                    for (int row = 0; row <= 8; row++)
                    {
                        if (_sudukoModel.SudData[row][applicableColumn] == assignValue && !currentSquareRows.Contains(row))
                        {
                            if (newIntArray[CurrentRow][applicableColumn] == 0)
                                newIntArray[CurrentRow][applicableColumn] = assignValue;
                            foreach (var applicableRow in applicableRows)
                            {
                                if (newIntArray[applicableRow][applicableColumn] == 0)
                                    newIntArray[applicableRow][applicableColumn] = assignValue;
                            }
                            break;
                        }
                    }
                }
                foreach (var applicableRow in applicableRows)
                {
                    for (int column = 0; column <= 8; column++)
                    {
                        if (_sudukoModel.SudData[applicableRow][column] == assignValue && !currentSquareColumns.Contains(column))
                        {
                            if (newIntArray[applicableRow][CurrentColumn] == 0)
                                newIntArray[applicableRow][CurrentColumn] = assignValue;
                            foreach (var applicableColumn in applicableColumns)
                            {
                                if (newIntArray[applicableRow][applicableColumn] == 0)
                                    newIntArray[applicableRow][applicableColumn] = assignValue;
                            }
                            break;
                        }
                    }
                }
                //Check all square having values.

                var allHavingValues = true;
                newIntArray[CurrentRow][CurrentColumn] = -1;
                for (var curRow = rowIndexStart; curRow <= rowIndexEnd; curRow++)
                {
                    for (var curColumn = columnIndexStart; curColumn <= columnIndexEnd; curColumn++)
                    {
                        if (newIntArray[curRow][curColumn] == 0)
                        {
                            allHavingValues = false;
                        }
                    }
                }
                if (allHavingValues)
                {
                    resultArray.Add(assignValue);
                }
            }
            var possibleValue = 0;
            if (resultArray.Count() == 1)
                possibleValue = resultArray[0];
            return possibleValue;
        }
        protected int[][] NewArray
        {
            get
            {
                var newArrray = new int[9][];
                for (var i = 0; i < newArrray.Length; i++)
                {
                    newArrray[i] = new int[9];
                }
                foreach (var t in newArrray)
                {
                    for (var j = 0; j < newArrray.Length; j++)
                    {
                        t[j] = 0;
                    }
                }
                return newArrray;
            }

        }

    }
}
