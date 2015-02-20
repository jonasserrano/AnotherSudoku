using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Threading;
namespace PRJ_AnotherSudoku.classes
{
    class SudokuBoard
    {
        #region /***********        PUBLIC MEMBERS          **********/

        public static Int32 animationDelay;

        public enum SolvingModes { INSTANT, ANIMATED };
        public static SolvingModes solvingMode;

        public static Int32 writeCounter = 0;
        public static Int32 backtrackCounter = 0;

        public int lastChangedCell_X;
        public int lastChangedCell_Y;


        public enum SolvingStatus { ABORTED, HALTED, RUNNING, SOLVED, SOLUTION_NOT_FOUND };
        public static SolvingStatus currentSolvingStatus = SolvingStatus.RUNNING;

        public enum SolvingOptimizations { SIMPLE_BACKTRACK, FORWARD_CHECK, CONTRAINT_PROP, UNIQUE_UPDATE };
        public static SolvingOptimizations currentSolvingOptimization = SolvingOptimizations.SIMPLE_BACKTRACK;


        public static int errorLocationCell_X;
        public static int errorLocationCell_Y;

        

        #endregion

        #region /*********    EVENTS AND DELEGATES          **********/
        public delegate void CellChanged_Hdlr(SudokuBoard sudokuToReport);
        
        /* EVENT-> Notify a cell's value has changed */
        public static event CellChanged_Hdlr CellChanged_Evnt;
        
        /* EVENT-> Notify current sudoku has found an error */
        public static event CellChanged_Hdlr ErrorFound_Evnt;

        /* EVENT-> Notify current sudoku has been finished */
        public static event CellChanged_Hdlr ProcessFinished_Evnt;

        #endregion


        #region /*********       PRIVATE MEMBERS            **********/

        /* Constant values */
        public const int SUDOKU_SIZE = 9;

        private Cell[,] boardArray;



        #endregion

        #region /*********   CONSTRUCTOR DECLARATION        **********/

        public SudokuBoard()
        {
            /* Instance algorithm-related members  */
            this.boardArray = new Cell[SUDOKU_SIZE, SUDOKU_SIZE];

            /* Filling the sudokuBoard with empty (0 value) cells */
            for (int x = 0; x < SUDOKU_SIZE; x++)
            {
                for (int y = 0; y < SUDOKU_SIZE; y++)
                {
                    this.boardArray[x, y] = new Cell(0);
                }
            }
        }

        #endregion

        #region /*********      PUBLIC FUNCTIONS     *****************/


        public bool solveSudokuInternal()
        {
            if (this.checkRepeatedValues() == false)
                return false;
            else
                return solveSudokuRecursive();
        }

        public Cell readCell(int x, int y)
        {
            /* Returns the object Cell of a given location of the current
             * sudokuBoard[][].
             */
            return this.boardArray[x, y];
        }


        public void writeCell(int x, int y, Int16 valueToWrite)
        {
            /* This function should change the value of an specific cell of the  
             * current sudokuBoard[][].
             * It should also update the list of possible values of every other 
             * cell in the sudokuBoard[][].
             */
            this.boardArray[x, y].setValue(valueToWrite);


            if (SudokuBoard.currentSolvingOptimization != SolvingOptimizations.SIMPLE_BACKTRACK &&
                SudokuBoard.currentSolvingOptimization != SolvingOptimizations.FORWARD_CHECK)
            {/* This optimization should NOT be included in a simple backtracking method nor Forward check*/
                this.updatePossibleValues(x, y, valueToWrite);
            }

            if (solvingMode == SolvingModes.ANIMATED)
            {
                /* Notify suscribers that a cell has been changed */
                if (CellChanged_Evnt != null)
                {
                    if (SudokuBoard.currentSolvingStatus == SolvingStatus.ABORTED)
                    {
                        return;
                    }
                    
                    do 
                    {
                        Thread.Sleep(animationDelay);
                    }while(SudokuBoard.currentSolvingStatus == SolvingStatus.HALTED);

                    //if (SudokuBoard.currentSolvingStatus == SolvingStatus.ABORTED)
                    //{
                      //  return;
                    //}

                    /* Only notify if there's at least one suscriber */
                    this.lastChangedCell_X = x;
                    this.lastChangedCell_Y = y;
                    CellChanged_Evnt(this);
                }
            }

            writeCounter++;

        }

        public void writeNewCell(int x, int y, Int16 valueToWrite)
        {
            this.boardArray[x, y] = new Cell(valueToWrite);
            if (SudokuBoard.currentSolvingOptimization != SolvingOptimizations.SIMPLE_BACKTRACK)
            {/* This optimization should NOT be included in a simple backtracking method */
                this.updatePossibleValues(x, y, valueToWrite);
            }
        }

        #endregion

        #region /*********      PRIVATE FUNCTIONS           **********/

        #region /*********        MAIN SOLVING PART         **********/
        
        public void solveSudoku_()
        {
            if (this.solveSudokuInternal() == true)
            {
                if(SudokuBoard.currentSolvingStatus != SolvingStatus.ABORTED)
                {
                /* Prepare event to notify a result was found */
                SudokuBoard.currentSolvingStatus = SolvingStatus.SOLVED;
                }
            }
            else
            {
                if (SudokuBoard.currentSolvingStatus != SolvingStatus.ABORTED)
                {
                    /* Prepare event to notify No Solving result was found */
                    SudokuBoard.currentSolvingStatus = SolvingStatus.SOLUTION_NOT_FOUND;
                }
                
            }

            /* Notify suscribers that a solving has Ended */
            if (ProcessFinished_Evnt != null)
            {
                ProcessFinished_Evnt(this);
            }

        }



        private bool solveSudokuRecursive()
        {
            /* This is it, the main backtracking-based solving engine... */
            

            /* Traverse the entire sudokuBoard searching for empty cells */
            for (int x = 0; x < SUDOKU_SIZE; x++){
                for (int y = 0; y < SUDOKU_SIZE; y++) {
                    
                    /* Look for Emtpy cells */
                    if (this.boardArray[x, y].getValue() == 0)
                    {
                        /* Try each of the possible values of the current emtpy cell */
                        foreach(Int16 testValue in this.boardArray[x,y].validValues)
                        {
                            bool tempConstraintPResult;

                            /* CLONE */
                            SudokuBoard currentSudokuBranch = this.clone();
                            /* WRITE */
                            currentSudokuBranch.writeCell(x, y, testValue);

                            if (SudokuBoard.currentSolvingOptimization == SolvingOptimizations.UNIQUE_UPDATE)
                            {
                                /* OPTIMIZE */
                                tempConstraintPResult = currentSudokuBranch.constraintPropagation();
                            }

                            if (currentSudokuBranch.checkStucked() == true)
                            {
                                tempConstraintPResult = false;
                                SudokuBoard.backtrackCounter++;
                            }
                            else if (currentSudokuBranch.checkSolved() == true)
                            {
                                tempConstraintPResult = true;
                            }
                            else if (currentSudokuBranch.checkRepeatedValues() == false)
                            {
                                tempConstraintPResult = false;
                                SudokuBoard.backtrackCounter++;
                            }
                            else
                            {
                                tempConstraintPResult = currentSudokuBranch.solveSudokuRecursive();
                            }


                            if (tempConstraintPResult == true)
                            {
                                /* Sudoku Solved!!! */
                                /* Transfer values from tempSudoku to current object */
                                this.overwriteFields(currentSudokuBranch);
                                return true;
                            }
                        }
                        return false;
                    }
                }
            }
            return false;
        }

        #endregion


        private bool checkRepeatedValues()
        {
            List<int>[] myList_X = new List<int>[SudokuBoard.SUDOKU_SIZE]
            {
                new List<int>(),new List<int>(),new List<int>(),
                new List<int>(),new List<int>(),new List<int>(),
                new List<int>(),new List<int>(),new List<int>()
            };
            List<int>[] myList_Y = new List<int>[SudokuBoard.SUDOKU_SIZE]
            {
                new List<int>(),new List<int>(),new List<int>(),
                new List<int>(),new List<int>(),new List<int>(),
                new List<int>(),new List<int>(),new List<int>()
            };
            List<int>[] myList_Square = new List<int>[SudokuBoard.SUDOKU_SIZE]
            {
                new List<int>(),new List<int>(),new List<int>(),
                new List<int>(),new List<int>(),new List<int>(),
                new List<int>(),new List<int>(),new List<int>()
            };

            /* Columns */
            for (int i = 0; i < SUDOKU_SIZE; i++)
            {
                for (int j = 0; j < SUDOKU_SIZE; j++)
                {
                    /* Checking Rows */
                    if (this.boardArray[i, j].value > 0)
                    {
                        if (myList_X[i].Contains(this.boardArray[i, j].value))
                            return false;
                        else
                            myList_X[i].Add(this.boardArray[i, j].value);

                        /* Checking Colums */
                        if (myList_Y[j].Contains(this.boardArray[i, j].value))
                            return false;
                        else
                            myList_Y[j].Add(this.boardArray[i, j].value);


                        Int16 xM = (Int16)(3 * (((Int16)i / 3)));
                        Int16 yM = (Int16)(3 * ((Int16)(j / 3)));
                        Int16 n;

                        if (yM > 0) { yM /= 3; }
                        n = (Int16)(xM + yM);


                        /* Checking Colums */
                        if (myList_Square[n].Contains(this.boardArray[i, j].value))
                            return false;
                        else
                            myList_Square[n].Add(this.boardArray[i, j].value);
                    }
                }
            }

                        
            /* */
            return true;
        }

        private void overwriteFields(SudokuBoard valuesToWrite) /* PLEASE TEST ME  -----------------------------------------!*/
        {
            /* Update array */
            this.boardArray = null;
            this.boardArray = valuesToWrite.boardArray;

        }

        private bool checkStucked()
        {
            /* Traverse the entire sudokuBoard searching for empty cells */
            for (int x = 0; x < SUDOKU_SIZE; x++)
            {
                for (int y = 0; y < SUDOKU_SIZE; y++)
                {
                     /* Look for Emtpy cells */
                    if (this.boardArray[x, y].getValue() == 0 && 
                        this.boardArray[x,y].validValues.Count == 0)
                    {
                        /* A misscondition has been found, report positive stuck  
                         * condition.
                         */
                        /* TBD: RISE EVENT TO NOTIFY ERROR LOCATION */

                        if (SudokuBoard.currentSolvingStatus == SolvingStatus.ABORTED)
                        {
                            return false;
                        }

                        if (solvingMode == SolvingModes.ANIMATED)
                        {
                            /* Notify suscribers that a cell has been changed */
                            if (ErrorFound_Evnt != null)
                            {
                                /* Only notify if there's at least one suscriber */
                                SudokuBoard.errorLocationCell_X = x;
                                SudokuBoard.errorLocationCell_Y = y;

                                

                                ErrorFound_Evnt(this);
                            }
                        }

                        
                        return true;
                    }
                }

            }
            
            /* No stuck condition found, report false stuck */
            return false;
        }

        private bool checkSolved()
        {
            /* Traverse the entire sudokuBoard searching for empty cells */
            for (int x = 0; x < SUDOKU_SIZE; x++)
            {
                for (int y = 0; y < SUDOKU_SIZE; y++)
                {
                    /* Look for Emtpy cells */
                    if (this.boardArray[x, y].getValue() == 0)
                    {
                        /* Sudoku is not solved yet.
                         * At least one cell is missing a value.
                         */
                        return false;
                    }
                }

            }

            /* Sudoku board is complete!!! */
            return true;
        }

        private SudokuBoard clone()
        {
            SudokuBoard tempSudoku = new SudokuBoard();
            
            /* Clone ony by one each of the sudoku's cells */
            for (int x = 0; x < SUDOKU_SIZE; x++)
            {
                for (int y = 0; y < SUDOKU_SIZE; y++)
                {
                    tempSudoku.boardArray[x, y] = this.boardArray[x, y].clone();
                }
            }
            /* Return a new reference with the same cell values */
            return tempSudoku;
        }

        private bool constraintPropagation()
        {
            /* Searches for those cells that have only one possible value
             * according to the row, column and box constraints and 
             * updates its content.
             * 
             * This is a recursive method since new unique values can be 
             * generated while writting target values.
             */
            int changeCounter;
            
            do
            {
                changeCounter = 0;
                for (int i = 0; i < SUDOKU_SIZE; i++){
                    for (int j = 0; j < SUDOKU_SIZE; j++){

                        /* Look for emtpy Values */
                        if (this.boardArray[i, j].getValue() == 0)
                        {
                            /* Look for UNIQUE valid values */
                            if (this.boardArray[i, j].validValues.Count == 1)
                            {
                                this.writeCell(i, j, this.boardArray[i, j].validValues[0]);

                                /* Test Code! */
                                if (this.checkStucked() == true)
                                {
                                    return false;
                                }

                                changeCounter++;
                            }

                            /* Look for stuck condition */
                            else if (this.boardArray[i, j].validValues.Count == 0)
                            {
                                return false;
                            }
                            else
                            {
                                /* Keep searching.... */
                            }
                            
                        }
                    }
                }
            } while (changeCounter > 0);
            
            /* All cells set to its unique value */
            return true;
        }

        private void updatePossibleValues(int x, int y, Int16 valueToWrite)
        {
            //this.boardArray[x, y].validValues.Remove(valueToWrite);

            /* Columns */
            for (int i = 0; i < SUDOKU_SIZE; i++)
            {
                //if (this.boardArray[i, y].getValue() == 0)
                //{
                    /* Remove validValues by Column*/
                    if (this.boardArray[i, y].validValues.Contains(valueToWrite))
                        this.boardArray[i, y].validValues.Remove(valueToWrite);
                //}
                //if (this.boardArray[x, i].getValue() == 0)
                //{
                    /* Remove validValues by Row*/
                    if (this.boardArray[x, i].validValues.Contains(valueToWrite))
                        this.boardArray[x, i].validValues.Remove(valueToWrite);
                //}
            }

            /* Squares */
            Int16 xM = (Int16)(3 * (((Int16)x / 3)));
            Int16 yM = (Int16)(3 * ((Int16)(y / 3)));

            for (int i = xM; i < xM+3; i++)
            {
                for(int j = yM; j < yM+3; j++)
                {
                    //if (this.boardArray[i, j].getValue() == 0)
                    //{
                        if (this.boardArray[i, j].validValues.Contains(valueToWrite))
                            this.boardArray[i, j].validValues.Remove(valueToWrite);
                    //}
                }
            }
        }

        #endregion        
    }
}
