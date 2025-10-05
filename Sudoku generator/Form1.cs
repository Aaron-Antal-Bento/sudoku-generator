using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Windows.Forms;

//problems:
//not enough seed numbers to cover all possible solutions

namespace Sudoku_generator
{
    public partial class Form1 : Form
    {
        Random seedCreator = new Random();
        Random rnd;
        Random fillRnd;
        Cells[,] cells = new Cells[9, 9];
        int seed;
        int fillSeed;
        bool quickGen;
        bool showSolution = false;
        bool generated;
        int fillPercentage;

        public Form1()
        {
            InitializeComponent();

            seed = seedCreator.Next();
            Seed.Text = seed.ToString();
            Seed.Select(Seed.Text.Length, Seed.Text.Length);

            fillSeed = seedCreator.Next();
            FillSeedTextBox.Text = fillSeed.ToString();
            FillSeedTextBox.Select(FillSeedTextBox.Text.Length, FillSeedTextBox.Text.Length);
        }

        private void GenerateButton_Click(object sender, EventArgs e)
        {
            quickGen = false;
            Generate();
        }
        private void QuickGenerateButton_Click(object sender, EventArgs e)
        {
            quickGen = true;
            Generate();
        }
        private void Seed_TextChanged(object sender, EventArgs e)
        {
            if (Seed.Text == "")
                seed = 0;
            else
            {
                try
                {
                    seed = Int32.Parse(Seed.Text);
                }
                catch (OverflowException) { Seed.Text = seed.ToString(); }
                catch (FormatException) { seed = 0; Seed.Text = seed.ToString(); }
            }
        }
        private void RandomiseSeed_Click(object sender, EventArgs e)
        {
            seed = seedCreator.Next();
            Seed.Text = seed.ToString();
        }
        private void FillSeedTextBox_TextChanged(object sender, EventArgs e)
        {
            if (FillSeedTextBox.Text == "")
                fillSeed = 0;
            else
            {
                try
                {
                    fillSeed = Int32.Parse(FillSeedTextBox.Text);
                }
                catch (OverflowException) { FillSeedTextBox.Text = fillSeed.ToString(); }
                catch (FormatException) { fillSeed = 0; FillSeedTextBox.Text = fillSeed.ToString(); }
            }
        }
        private void RandomiseFillSeed_Click(object sender, EventArgs e)
        {
            fillSeed = seedCreator.Next();
            FillSeedTextBox.Text = fillSeed.ToString();
        }
        private void ShowSolutionButton_Click(object sender, EventArgs e)
        {
            if (!generated) return;

            showSolution = !showSolution;

            foreach (Cells c in cells)
            {
                if (!c.GetValueFound())
                    c.ChangeNumber(showSolution ? c.GetSolution() : c.GetValue());
            }
        }

        private void FillPercentageSlider_ValueChanged(object sender, EventArgs e)
        {
            PercentageDisplay.Text = (FillPercentageSlider.Value * 5).ToString() + "%";
            fillPercentage = FillPercentageSlider.Value * 5;
        }
        private void Easy_Click(object sender, EventArgs e)
        {
            FillPercentageSlider.Value = 12;
        }
        private void Medium_Click(object sender, EventArgs e)
        {
            FillPercentageSlider.Value = 8;
        }
        private void Hard_Click(object sender, EventArgs e)
        {
            FillPercentageSlider.Value = 4;
        }

        private void Generate()
        {
            GeneratingText.Visible = true;

            rnd = new Random(seed);
            fillRnd = new Random(fillSeed);

            Button[,] buttons =
            {
                { a1, b1, c1,   d1, e1, f1,   g1, h1, i1 },
                { a2, b2, c2,   d2, e2, f2,   g2, h2, i2 },
                { a3, b3, c3,   d3, e3, f3,   g3, h3, i3 },

                { a4, b4, c4,   d4, e4, f4,   g4, h4, i4 },
                { a5, b5, c5,   d5, e5, f5,   g5, h5, i5 },
                { a6, b6, c6,   d6, e6, f6,   g6, h6, i6 },

                { a7, b7, c7,   d7, e7, f7,   g7, h7, i7 },
                { a8, b8, c8,   d8, e8, f8,   g8, h8, i8 },
                { a9, b9, c9,   d9, e9, f9,   g9, h9, i9 },
            };

            cells = new Cells[9, 9];
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    cells[y, x] = new Cells((y, x), buttons[y, x]);
                }
            }

            CreateBlocks(cells);

            ResetBoard(cells);
            Application.DoEvents();

            FillBoard(cells);

            foreach (Cells cell in cells)
            {
                cell.SetSolution();
            }
            ResetPencilMarks(cells);

            RemoveNumbers(cells);

            foreach (Cells cell in cells)
            {
                if (cell.GetValueFound())
                    cell.ChangeColor(Color.Gainsboro);
            }

            generated = true;
            GeneratingText.Visible = false;
        }

        private void ResetBoard(Cells[,] cells)
        {
            foreach (Cells cell in cells)
            {
                cell.SetValue(0);
                cell.ChangeColor(Color.White);
            }
            showSolution = false;
            generated = false;
        }
        private void CreateBlocks(Cells[,] cells)
        {
            int blockX = 0;
            int blockY = 0;
            for (int i = 0; i < 9; i++)
            {
                int n = 0;
                for (int y = blockY * 3; y < blockY * 3 + 3; y++)
                {
                    for (int x = blockX * 3; x < blockX * 3 + 3; x++)
                    {
                        cells[y, x].SetBlock(i);
                        n++;
                    }
                }

                blockX++;
                if (blockX == 3)
                {
                    blockX = 0;
                    blockY++;
                }
            }
        }

        private void FillBoard(Cells[,] cells)
        {
            CreatePencilMarks(cells);

            List<Cells> cellsToFind = new List<Cells>();
            foreach (Cells cell in cells)
            {
                cell.ShufflePencilMarks(rnd);
                cellsToFind.Add(cell);
            }

            List<int> solution = new List<int>();
            List<int> potentialSolutions = RecursiveSolutions(0, cellsToFind, solution);

            int i = 0;
            foreach (Cells cell in cellsToFind)
            {
                cell.SetValue(potentialSolutions[i]);
                cell.ValueFound();
                i++;
            }
            if (!quickGen)
                Application.DoEvents();
        }
        private void CreatePencilMarks(Cells[,] cells)
        {
            foreach (Cells cell in cells)
            {
                if (!cell.GetValueFound())
                {
                    for (int i = 1; i <= 9; i++)
                    {
                        if (CheckBlock(cells, cell.GetBlock(), i))
                            if (CheckRow(cells, cell.GetRow(), i))
                                if (CheckColumn(cells, cell.GetColumn(), i))
                                    cell.AddPencilMark(i);
                    }
                }
            }
        }
        private void ResetPencilMarks(Cells[,] cells)
        {
            foreach (Cells cell in cells)
            {
                cell.ClearPencilMarks();
            }
        }

        private void RemoveNumbers(Cells[,] cells)
        {
            List<Cells> shuffledCells = ShuffleList(cells);

            foreach (Cells cell in shuffledCells)
            {
                if (fillPercentage >= BoardFillPercentage(cells)) continue;

                int currentCellValue = cell.GetValue();

                if (!cell.GetValueFound()) continue;
                cell.SetValue(-1);
                cell.ValueNotFound();
                if (!quickGen)
                    Application.DoEvents();

                if (!OneSolution(cells))
                {
                    cell.SetValue(currentCellValue);
                    cell.ValueFound();
                }
            }
        }
        private bool OneSolution(Cells[,] cells)
        {
            ResetPencilMarks(cells);
            CreatePencilMarks(cells);

            List<Cells> cellsToFind = new List<Cells>();
            foreach (Cells cell in cells)
            {
                if (!cell.GetValueFound())
                    cellsToFind.Add(cell);
            }

            int numberOfSolutions = 0;

            return CheckForOneSolution(0, cellsToFind, new int[cellsToFind.Count()], ref numberOfSolutions);
        }
        private bool CheckForOneSolution(int cell, List<Cells> cellsToFind, int[] solution, ref int result)
        {
            foreach (int pencilMark in cellsToFind[cell].GetPencilMarks())
            {
                solution[cell] = pencilMark;

                if (!CheckSolutionValid(solution, cellsToFind, cell)) continue;

                if (cell == cellsToFind.Count() - 1)
                {
                    result++;
                    if (result == 2) return false;
                }
                else
                    if (!CheckForOneSolution(cell + 1, cellsToFind, solution, ref result)) return false;
            }
            return result < 2;
        }
        private List<int> RecursiveSolutions(int cell, List<Cells> cellsToFind, List<int> s)
        {
            foreach (int pencilMark in cellsToFind[cell].GetPencilMarks())
            {
                List<int> str = s.ToList();
                str.Add(pencilMark); 
                
                if (!CheckSolutionValid(str, cellsToFind)) continue;

                if (cell == cellsToFind.Count() - 1)
                    return str;
                else
                {
                    List<int> solution = RecursiveSolutions(cell + 1, cellsToFind, str);
                    if (solution.Count == cellsToFind.Count())
                        return solution;
                }
            }
            return new List<int>();
        }
        private bool CheckSolutionValid(List<int> solution, List<Cells> cellsToFind)
        {
            HashSet<int>[] row = new HashSet<int>[9];
            HashSet<int>[] column = new HashSet<int>[9];
            HashSet<int>[] block = new HashSet<int>[9];
            int index = 0;

            foreach (int pM in solution)
            {
                Cells cell = cellsToFind[index];

                int r = cell.GetRow(); if (row[r] == null) row[r] = new HashSet<int>();
                if (!row[r].Add(pM)) return false;

                int c = cell.GetColumn(); if (column[c] == null) column[c] = new HashSet<int>();
                if (!column[c].Add(pM)) return false;

                int b = cell.GetBlock(); if (block[b] == null) block[b] = new HashSet<int>();
                if (!block[b].Add(pM)) return false;

                index++;
            }
            return true;
        }
        private bool CheckSolutionValid(int[] solution, List<Cells> cellsToFind, int index)
        {
            HashSet<int>[] row = new HashSet<int>[9];
            HashSet<int>[] column = new HashSet<int>[9];
            HashSet<int>[] block = new HashSet<int>[9];

            for (int i = 0; i <= index; i++)
            {
                Cells cell = cellsToFind[i];
                int pM = solution[i];

                int r = cell.GetRow(); if (row[r] == null) row[r] = new HashSet<int>();
                if (!row[r].Add(pM)) return false;

                int c = cell.GetColumn(); if (column[c] == null) column[c] = new HashSet<int>();
                if (!column[c].Add(pM)) return false;

                int b = cell.GetBlock(); if (block[b] == null) block[b] = new HashSet<int>();
                if (!block[b].Add(pM)) return false;
            }
            return true;
        }
        private int BoardFillPercentage(Cells[,] cells)
        {
            float filledCells = 0;
            foreach (Cells cell in cells)
            {
                if (cell.GetValueFound())
                    filledCells++;
            }

            return (Int16)((filledCells / 81) * 100);
        }

        private bool CheckBlock(Cells[,] cells, int block, int num)
        {
            foreach (Cells cell in GetBlock(cells, block))
            {
                if (cell.GetValue() == num)
                    return false;
            }
            return true;
        }
        private bool CheckRow(Cells[,] cells, int row, int num)
        {
            for (int i = 0; i < 9; i++)
            {
                if (cells[row, i].GetValue() == num)
                    return false;
            }
            return true;
        }
        private bool CheckColumn(Cells[,] cells, int column, int num)
        {
            for (int i = 0; i < 9; i++)
            {
                if (cells[i, column].GetValue() == num)
                    return false;
            }
            return true;
        }

        private List<Cells> ShuffleList(Cells[,] array)
        {
            List<Cells> shuffledList = new List<Cells>();
            List<Cells> tempList = new List<Cells>();
            foreach (Cells c in array)
            {
                tempList.Add(c);
            }

            int n = 0;
            while (n < array.Length)
            {
                int r = fillRnd.Next(tempList.Count);
                shuffledList.Add(tempList[r]);
                tempList.RemoveAt(r);
                n++;
            }

            return shuffledList;
        }

        private List<T> ArrayToList<T>(T[] array)
        {
            List<T> list = new List<T>();

            foreach (T a in array)
            {
                list.Add(a);
            }
            return list;
        }
        private List<Cells> GetBlock(Cells[,] cells, int block)
        {
            List<Cells> c = new List<Cells>();
            foreach (Cells cell in cells)
            {
                if (cell.GetBlock() == block)
                {
                    c.Add(cell);
                }
            }
            return c;
        }
    }
}