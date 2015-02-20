using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRJ_AnotherSudoku.classes
{
    class Cell
    {
        public bool isFinal;
        public Int16 value;
        public List<Int16> validValues;

        public Cell()
        {
            validValues = new List<Int16>();
            value = 0;

            isFinal = false;
        }

        public Cell(Int16 value)
            :this()
        {
            if (value <= 0)
            {
                /* In an empty cell, all values from 1 to 9 can be 
                 * valid values.
                 */
                for (int i = 0; i < PRJ_AnotherSudoku.classes.SudokuBoard.SUDOKU_SIZE; i++)
                {
                    this.validValues.Add((Int16)(i+1));
                }
            }
            else
            {
                /* This is a cell that can not be changed by the algorithm */
                isFinal = true;
            }

            this.value = value;
        }

        public void setValue(Int16 valueToWrite)
        {
            this.value = valueToWrite;
        }
        public Int16 getValue()
        {
            return this.value;
        }

        public Cell clone()
        {
            Cell tempCell = new Cell();
            List<Int16> tempValidValues = new List<Int16>();

            /* Cloning list of valid values */
            for (int i = 0; i < this.validValues.Count; i++)
            {
                tempValidValues.Add(this.validValues[i]);
            }
            /* Cloning cell's value */
            tempCell.validValues = tempValidValues;
            tempCell.value = this.value;
            tempCell.isFinal = this.isFinal;


            return tempCell;
        }



        public String getListOfValidValues()
        {
            String tempString = String.Empty;

            for (int i = 0; i < this.validValues.Count; i++)
            {
                tempString = tempString + this.validValues[i];
            }

            return tempString;
        }
        

    }
}
