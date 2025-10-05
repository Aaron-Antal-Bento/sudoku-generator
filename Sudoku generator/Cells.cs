using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Sudoku_generator
{
    internal class Cells
    {
        (int, int) coords;
        Button cell;
        int value = 0;
        int Solution;
        List<int> pencilMarks = new List<int>();
        bool valueFound = false;
        int block;

        public Cells((int,int) coords, Button cell)
        {
            this.coords = coords;
            this.cell = cell;

            ChangeNumber(value);
        }

        public (int,int) GetCoords()
        {
            return coords;
        }
        public int GetRow() { return coords.Item1; }
        public int GetColumn() { return coords.Item2; }

        public void SetValue(int value) { this.value = value; ChangeNumber(this.value); }
        public int GetValue() { return value; }
        public void SetSolution() { Solution = value; }
        public int GetSolution() { return Solution; }

        public void AddPencilMark(int pencilMark) { pencilMarks.Add(pencilMark); }
        public List<int> GetPencilMarks() { return pencilMarks; }
        public int GetPencilMark(int n) { return pencilMarks[n]; }
        public int GetNumberOfPencilMarks() { return pencilMarks.Count; }
        public void ClearPencilMarks() { pencilMarks.Clear(); }
        public void ShufflePencilMarks(Random seed)
        {
            List<int> shuffledList = new List<int>();
            List<int> tempList = new List<int>();
            foreach (int c in pencilMarks)
            {
                tempList.Add(c);
            }

            int n = 0;
            while (n < pencilMarks.Count)
            {
                int r = seed.Next(tempList.Count);
                shuffledList.Add(tempList[r]);
                tempList.RemoveAt(r);
                n++;
            }

            pencilMarks = shuffledList.ToList();
        }

        public void ValueFound() { valueFound = true; }
        public void ValueNotFound() { valueFound = false; }
        public bool GetValueFound() { return valueFound; }

        public void SetBlock(int block) { this.block = block; }
        public int GetBlock() { return block; }

        public void ChangeNumber(int newNumber)
        {
            if(cell != null)
                if (newNumber == 0 || newNumber == -1)
                    cell.Text = "";
                else
                    cell.Text = newNumber.ToString();
        }

        public Button GetButton()
        {
            return cell;
        }
        public void ChangeColor(Color c)
        {
            cell.BackColor = c;
            cell.FlatAppearance.MouseOverBackColor = c;
            cell.FlatAppearance.MouseDownBackColor = c;
        }
    }
}
