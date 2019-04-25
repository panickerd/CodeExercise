using System;
using System.Collections.Generic;
using System.Linq;
using MineSweeperGenerator;

namespace MineSweeper
{
    class Program
    {
        private static readonly MineSweeperField _field = MineSweeperFactory.CreateMineField(20, 10, 25); // width, height, nr of bombs

        static List<FieldInfo> unOpenedCells = new List<FieldInfo>();
        static void Main(string[] args)
        {
            /*
             * You can play the game here: https://geekprank.com/minesweeper/
             * */
            try
            {
                var freefields = _field[1, 1];
                int rows = _field.Width;
                int columns = _field.Height;
                int noOfBombs = _field.NumberOfBombs;
                int noOfUnopenedCells = _field.Width * _field.Height - freefields.Count();

                int row = 1, col = 1;
                while (noOfBombs < noOfUnopenedCells)
                {
                    if (NoOfSurroundingMines(row, col, freefields, ref noOfUnopenedCells) == 0)
                    {
                        freefields = _field[unOpenedCells[0].X, unOpenedCells[0].Y];
                        unOpenedCells.Remove(new FieldInfo()
                        {
                            X = unOpenedCells[0].X,
                            Y = unOpenedCells[0].Y
                        });
                    }
                }

                FindRemainingBombs(freefields, rows, columns);
            }
            catch (BombExplodedException)
            {
                Console.WriteLine("It is a bomb");
            }
            catch (NoBombException)
            {
                Console.WriteLine("Game over");
            }
            catch (MineFieldClearedException)
            {
                Console.WriteLine("Game Won");
            }

        }

        /// <summary>
        /// Identify remaining bombs to catch MineFieldClearedException
        /// </summary>
        /// <param name="freefields">list of freefields</param>
        /// <param name="rows">rows</param>
        /// <param name="columns">columns</param>
        private static void FindRemainingBombs(List<FieldInfo> freefields, int rows, int columns)
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    if (freefields.FirstOrDefault(x => x.X == r && x.Y == c) == null)
                    {
                        _field.IdentifyBomb(r, c);
                    }
                }
            }
        }

        /// <summary>
        /// Returns no of surrounding mines
        /// </summary>
        /// <param name="row">row</param>
        /// <param name="col">col</param>
        /// <param name="fields">list of fields</param>
        /// <returns>count of bombs</returns>
        private static int NoOfSurroundingMines(int row, int col, List<FieldInfo> fields, ref int noOfUnopenedCells)
        {
            int fieldWithBombCount = 0;
            int noBombFieldCount = 0;
            List<FieldInfo> surroundingUnopenedCells = new List<FieldInfo>();
            for (int r = row - 1; r <= row + 1; ++r)
            {
                for (int c = col - 1; c <= col + 1; ++c)
                {
                    if (fields.Where(x => x.X == r && x.Y == c).FirstOrDefault()?.NumberOfBombs > 0)
                    {
                        fieldWithBombCount++;
                    }
                    else
                    {
                        noBombFieldCount++;
                        surroundingUnopenedCells.Add(new FieldInfo()
                        {
                            X = r,
                            Y = c
                        });
                    }
                }
            }
            if (fieldWithBombCount > noBombFieldCount)
            {
                noOfUnopenedCells += noBombFieldCount;
                fieldWithBombCount = 0;
            }
            else
            {
                unOpenedCells.AddRange(surroundingUnopenedCells);
            }
            return fieldWithBombCount;
        }
    }
}


