using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


using System.Threading;
/* Project-specific Classes */
using PRJ_AnotherSudoku.classes;
using System.Diagnostics;

namespace PRJ_AnotherSudoku
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {

        System.Windows.Controls.TextBox[,] myTextBoxArray = new TextBox[SudokuBoard.SUDOKU_SIZE, SudokuBoard.SUDOKU_SIZE];
        System.Windows.Controls.Label[,] myLabelArray = new Label[SudokuBoard.SUDOKU_SIZE, SudokuBoard.SUDOKU_SIZE];

        #region /*********       PRIVATE MEMBERS            **********/

        SudokuBoard sudokuBoard;

        /* This thread will be used to start the sudoku solving. 
         * Using a thread to execute the solving algorithm will allow the
         * main Form to keep executing and avoid GUI freeze while process 
         * is running.
         */
        Thread sudokuSolving_Thread;


        #endregion

        public MainWindow()
		{
			this.InitializeComponent();

            /* Force total Selection when each cell is CLICKED or GOT_FOCUS */
            this.RegisterClassHandlers();

            /* Update preloaded sudokus */
            this.preloadSudokus();

            /* Suscribing to sudokuBoard CellChanged_Evnt event */
            SudokuBoard.CellChanged_Evnt += new SudokuBoard.CellChanged_Hdlr(OnCellChanged);

            /* Suscribing to sudokuBoard CellChanged_Evnt event */
            SudokuBoard.ErrorFound_Evnt += new SudokuBoard.CellChanged_Hdlr(OnErrorFound);

            /* Suscribing to sudokuBoard CellChanged_Evnt event */
            SudokuBoard.ProcessFinished_Evnt += new SudokuBoard.CellChanged_Hdlr(OnSudokuFinished);

            this.storeCells();

            this.storeLabels();

        }


        private void OnSudokuFinished(SudokuBoard sudokuToReport)
        {
            if (this.Dispatcher.Thread.Equals(Thread.CurrentThread) == false)
            {
                classes.SudokuBoard.CellChanged_Hdlr d = new SudokuBoard.CellChanged_Hdlr(this.OnSudokuFinished);
                Dispatcher.Invoke(d, sudokuToReport);
                return;
            }

            if (SudokuBoard.currentSolvingStatus == SudokuBoard.SolvingStatus.SOLVED)
            {
                this.Status_Lbl.Content = "Solved!";
                this.Status_Lbl.Foreground = new SolidColorBrush(Color.FromArgb(255, 71, 178, 0)); 
            }
            else if (SudokuBoard.currentSolvingStatus == SudokuBoard.SolvingStatus.SOLUTION_NOT_FOUND)
            {
                this.Status_Lbl.Content = "Solution Not found";
                this.Status_Lbl.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 93, 0)); 
            }
            else if (SudokuBoard.currentSolvingStatus == SudokuBoard.SolvingStatus.ABORTED)
            {
                this.Status_Lbl.Content = "Aborted";
                this.Status_Lbl.Foreground = new SolidColorBrush(Color.FromArgb(255, 248, 249, 63)); 
            }

            this.AnimateToggle_Sw.IsChecked = false;
        }


        private void OnErrorFound(SudokuBoard sudokuToReport)
        {
            if (this.Dispatcher.Thread.Equals(Thread.CurrentThread) == false)
            {
                classes.SudokuBoard.CellChanged_Hdlr d = new SudokuBoard.CellChanged_Hdlr(this.OnErrorFound);
                Dispatcher.Invoke(d, sudokuToReport);
                return;
            }

            this.myTextBoxArray[SudokuBoard.errorLocationCell_X,
                                SudokuBoard.errorLocationCell_Y].Text = "X";

            this.myTextBoxArray[SudokuBoard.errorLocationCell_X,
                                SudokuBoard.errorLocationCell_Y].Foreground = new SolidColorBrush(Color.FromArgb(100, 255, 0, 0)); 
        }

        private void storeLabels()
        {
            #region Storing references to valid values labels per cell
            myLabelArray[0, 0] = this.ValidValues_A1;
            myLabelArray[0, 1] = this.ValidValues_A2;
            myLabelArray[0, 2] = this.ValidValues_A3;
            myLabelArray[0, 3] = this.ValidValues_A4;
            myLabelArray[0, 4] = this.ValidValues_A5;
            myLabelArray[0, 5] = this.ValidValues_A6;
            myLabelArray[0, 6] = this.ValidValues_A7;
            myLabelArray[0, 7] = this.ValidValues_A8;
            myLabelArray[0, 8] = this.ValidValues_A9;
            myLabelArray[1, 0] = this.ValidValues_B1;
            myLabelArray[1, 1] = this.ValidValues_B2;
            myLabelArray[1, 2] = this.ValidValues_B3;
            myLabelArray[1, 3] = this.ValidValues_B4;
            myLabelArray[1, 4] = this.ValidValues_B5;
            myLabelArray[1, 5] = this.ValidValues_B6;
            myLabelArray[1, 6] = this.ValidValues_B7;
            myLabelArray[1, 7] = this.ValidValues_B8;
            myLabelArray[1, 8] = this.ValidValues_B9;
            myLabelArray[2, 0] = this.ValidValues_C1;
            myLabelArray[2, 1] = this.ValidValues_C2;
            myLabelArray[2, 2] = this.ValidValues_C3;
            myLabelArray[2, 3] = this.ValidValues_C4;
            myLabelArray[2, 4] = this.ValidValues_C5;
            myLabelArray[2, 5] = this.ValidValues_C6;
            myLabelArray[2, 6] = this.ValidValues_C7;
            myLabelArray[2, 7] = this.ValidValues_C8;
            myLabelArray[2, 8] = this.ValidValues_C9;
            myLabelArray[3, 0] = this.ValidValues_D1;
            myLabelArray[3, 1] = this.ValidValues_D2;
            myLabelArray[3, 2] = this.ValidValues_D3;
            myLabelArray[3, 3] = this.ValidValues_D4;
            myLabelArray[3, 4] = this.ValidValues_D5;
            myLabelArray[3, 5] = this.ValidValues_D6;
            myLabelArray[3, 6] = this.ValidValues_D7;
            myLabelArray[3, 7] = this.ValidValues_D8;
            myLabelArray[3, 8] = this.ValidValues_D9;
            myLabelArray[4, 0] = this.ValidValues_E1;
            myLabelArray[4, 1] = this.ValidValues_E2;
            myLabelArray[4, 2] = this.ValidValues_E3;
            myLabelArray[4, 3] = this.ValidValues_E4;
            myLabelArray[4, 4] = this.ValidValues_E5;
            myLabelArray[4, 5] = this.ValidValues_E6;
            myLabelArray[4, 6] = this.ValidValues_E7;
            myLabelArray[4, 7] = this.ValidValues_E8;
            myLabelArray[4, 8] = this.ValidValues_E9;
            myLabelArray[5, 0] = this.ValidValues_F1;
            myLabelArray[5, 1] = this.ValidValues_F2;
            myLabelArray[5, 2] = this.ValidValues_F3;
            myLabelArray[5, 3] = this.ValidValues_F4;
            myLabelArray[5, 4] = this.ValidValues_F5;
            myLabelArray[5, 5] = this.ValidValues_F6;
            myLabelArray[5, 6] = this.ValidValues_F7;
            myLabelArray[5, 7] = this.ValidValues_F8;
            myLabelArray[5, 8] = this.ValidValues_F9;
            myLabelArray[6, 0] = this.ValidValues_G1;
            myLabelArray[6, 1] = this.ValidValues_G2;
            myLabelArray[6, 2] = this.ValidValues_G3;
            myLabelArray[6, 3] = this.ValidValues_G4;
            myLabelArray[6, 4] = this.ValidValues_G5;
            myLabelArray[6, 5] = this.ValidValues_G6;
            myLabelArray[6, 6] = this.ValidValues_G7;
            myLabelArray[6, 7] = this.ValidValues_G8;
            myLabelArray[6, 8] = this.ValidValues_G9;
            myLabelArray[7, 0] = this.ValidValues_H1;
            myLabelArray[7, 1] = this.ValidValues_H2;
            myLabelArray[7, 2] = this.ValidValues_H3;
            myLabelArray[7, 3] = this.ValidValues_H4;
            myLabelArray[7, 4] = this.ValidValues_H5;
            myLabelArray[7, 5] = this.ValidValues_H6;
            myLabelArray[7, 6] = this.ValidValues_H7;
            myLabelArray[7, 7] = this.ValidValues_H8;
            myLabelArray[7, 8] = this.ValidValues_H9;
            myLabelArray[8, 0] = this.ValidValues_I1;
            myLabelArray[8, 1] = this.ValidValues_I2;
            myLabelArray[8, 2] = this.ValidValues_I3;
            myLabelArray[8, 3] = this.ValidValues_I4;
            myLabelArray[8, 4] = this.ValidValues_I5;
            myLabelArray[8, 5] = this.ValidValues_I6;
            myLabelArray[8, 6] = this.ValidValues_I7;
            myLabelArray[8, 7] = this.ValidValues_I8;
            myLabelArray[8, 8] = this.ValidValues_I9;
            #endregion
        }

        private void storeCells()
        {
            #region Storing references to sudoku cells
            myTextBoxArray[0, 0] = this.Cell_A1;
            myTextBoxArray[0, 1] = this.Cell_A2;
            myTextBoxArray[0, 2] = this.Cell_A3;
            myTextBoxArray[0, 3] = this.Cell_A4;
            myTextBoxArray[0, 4] = this.Cell_A5;
            myTextBoxArray[0, 5] = this.Cell_A6;
            myTextBoxArray[0, 6] = this.Cell_A7;
            myTextBoxArray[0, 7] = this.Cell_A8;
            myTextBoxArray[0, 8] = this.Cell_A9;
            myTextBoxArray[1, 0] = this.Cell_B1;
            myTextBoxArray[1, 1] = this.Cell_B2;
            myTextBoxArray[1, 2] = this.Cell_B3;
            myTextBoxArray[1, 3] = this.Cell_B4;
            myTextBoxArray[1, 4] = this.Cell_B5;
            myTextBoxArray[1, 5] = this.Cell_B6;
            myTextBoxArray[1, 6] = this.Cell_B7;
            myTextBoxArray[1, 7] = this.Cell_B8;
            myTextBoxArray[1, 8] = this.Cell_B9;
            myTextBoxArray[2, 0] = this.Cell_C1;
            myTextBoxArray[2, 1] = this.Cell_C2;
            myTextBoxArray[2, 2] = this.Cell_C3;
            myTextBoxArray[2, 3] = this.Cell_C4;
            myTextBoxArray[2, 4] = this.Cell_C5;
            myTextBoxArray[2, 5] = this.Cell_C6;
            myTextBoxArray[2, 6] = this.Cell_C7;
            myTextBoxArray[2, 7] = this.Cell_C8;
            myTextBoxArray[2, 8] = this.Cell_C9;
            myTextBoxArray[3, 0] = this.Cell_D1;
            myTextBoxArray[3, 1] = this.Cell_D2;
            myTextBoxArray[3, 2] = this.Cell_D3;
            myTextBoxArray[3, 3] = this.Cell_D4;
            myTextBoxArray[3, 4] = this.Cell_D5;
            myTextBoxArray[3, 5] = this.Cell_D6;
            myTextBoxArray[3, 6] = this.Cell_D7;
            myTextBoxArray[3, 7] = this.Cell_D8;
            myTextBoxArray[3, 8] = this.Cell_D9;
            myTextBoxArray[4, 0] = this.Cell_E1;
            myTextBoxArray[4, 1] = this.Cell_E2;
            myTextBoxArray[4, 2] = this.Cell_E3;
            myTextBoxArray[4, 3] = this.Cell_E4;
            myTextBoxArray[4, 4] = this.Cell_E5;
            myTextBoxArray[4, 5] = this.Cell_E6;
            myTextBoxArray[4, 6] = this.Cell_E7;
            myTextBoxArray[4, 7] = this.Cell_E8;
            myTextBoxArray[4, 8] = this.Cell_E9;
            myTextBoxArray[5, 0] = this.Cell_F1;
            myTextBoxArray[5, 1] = this.Cell_F2;
            myTextBoxArray[5, 2] = this.Cell_F3;
            myTextBoxArray[5, 3] = this.Cell_F4;
            myTextBoxArray[5, 4] = this.Cell_F5;
            myTextBoxArray[5, 5] = this.Cell_F6;
            myTextBoxArray[5, 6] = this.Cell_F7;
            myTextBoxArray[5, 7] = this.Cell_F8;
            myTextBoxArray[5, 8] = this.Cell_F9;
            myTextBoxArray[6, 0] = this.Cell_G1;
            myTextBoxArray[6, 1] = this.Cell_G2;
            myTextBoxArray[6, 2] = this.Cell_G3;
            myTextBoxArray[6, 3] = this.Cell_G4;
            myTextBoxArray[6, 4] = this.Cell_G5;
            myTextBoxArray[6, 5] = this.Cell_G6;
            myTextBoxArray[6, 6] = this.Cell_G7;
            myTextBoxArray[6, 7] = this.Cell_G8;
            myTextBoxArray[6, 8] = this.Cell_G9;
            myTextBoxArray[7, 0] = this.Cell_H1;
            myTextBoxArray[7, 1] = this.Cell_H2;
            myTextBoxArray[7, 2] = this.Cell_H3;
            myTextBoxArray[7, 3] = this.Cell_H4;
            myTextBoxArray[7, 4] = this.Cell_H5;
            myTextBoxArray[7, 5] = this.Cell_H6;
            myTextBoxArray[7, 6] = this.Cell_H7;
            myTextBoxArray[7, 7] = this.Cell_H8;
            myTextBoxArray[7, 8] = this.Cell_H9;
            myTextBoxArray[8, 0] = this.Cell_I1;
            myTextBoxArray[8, 1] = this.Cell_I2;
            myTextBoxArray[8, 2] = this.Cell_I3;
            myTextBoxArray[8, 3] = this.Cell_I4;
            myTextBoxArray[8, 4] = this.Cell_I5;
            myTextBoxArray[8, 5] = this.Cell_I6;
            myTextBoxArray[8, 6] = this.Cell_I7;
            myTextBoxArray[8, 7] = this.Cell_I8;
            myTextBoxArray[8, 8] = this.Cell_I9;
            #endregion
        }

        private void preloadSudokus()
        {
            // Adding sudoku selectors
            this.PreloadedSudoku_Dbox.Items.Add("Easy");
            this.PreloadedSudoku_Dbox.Items.Add("Medium");
            this.PreloadedSudoku_Dbox.Items.Add("Hard");
        }

        private void OnCellChanged(SudokuBoard sudokuToReport)
        {
            Int16 temp;
            temp = sudokuToReport.readCell(0, 0).getValue();

            if (this.Dispatcher.Thread.Equals(Thread.CurrentThread) == false)
            {
                classes.SudokuBoard.CellChanged_Hdlr d = new SudokuBoard.CellChanged_Hdlr(this.OnCellChanged);
                Dispatcher.Invoke(d, sudokuToReport);
                return;
            }

            for (int i = 0; i < SudokuBoard.SUDOKU_SIZE; i++){
                for (int j = 0; j < SudokuBoard.SUDOKU_SIZE; j++)
                {
                    /* Writting cell's value */
                    temp = sudokuToReport.readCell(i, j).getValue();
                    if (temp != 0)
                    {
                        this.myTextBoxArray[i, j].Text = temp.ToString();
                    }
                    else
                    {
                        this.myTextBoxArray[i, j].Text = " ";
                    }


                    /* Writting cell's Foreground color */
                    if (sudokuToReport.readCell(i, j).isFinal == false)
                    {
                        /* Green */
                        this.myTextBoxArray[i, j].Foreground = new SolidColorBrush(Color.FromArgb(100, 71, 178, 0)); 
                    }


                    /* Update Listo of possible values per cell */
                    this.myLabelArray[i, j].Content = sudokuToReport.readCell(i, j).getListOfValidValues();
                }
            }
            


            /* Blue */
            this.myTextBoxArray[sudokuToReport.lastChangedCell_X,sudokuToReport.lastChangedCell_Y].Foreground = new SolidColorBrush(Color.FromArgb(100, 45, 154, 255));


            this.TotalTriedValues_Lbl.Content = SudokuBoard.writeCounter.ToString();

            this.BackTrackCount_Lbl.Content = SudokuBoard.backtrackCounter.ToString();
        }



        #region /*********      PRIVATE FUNCTIONS           **********/

        private void readSudokuBoard()
        {
            /* Travese each of the cells contained in the WPF-Based GUI and  
             * stores it content in the local sudokuBoard[][] member.
             */
            Stopwatch sw = new Stopwatch();
            sw.Start();

            


            //int myInt = int.Parse(this.Cell_A1.Text);
            int tempInt;

            for (int i = 0; i < SudokuBoard.SUDOKU_SIZE; i++)
            {
                for (int j = 0; j < SudokuBoard.SUDOKU_SIZE; j++)
                {

                    if (int.TryParse(this.myTextBoxArray[i,j].Text, out tempInt) == true)
                    {
                        this.sudokuBoard.writeNewCell(i, j, (Int16)tempInt);
                    }
                    else
                    {
                        this.myTextBoxArray[i, j].Foreground = new SolidColorBrush(Color.FromArgb(100, 71, 178, 0));
                    }

                    //this.myTextBoxArray[i, j].Text = toUpdate.readCell(i, j).getValue().ToString();
                }
            }
            
            sw.Stop();
            Console.WriteLine("Reading time " + sw.ElapsedMilliseconds.ToString());
        }

        private String fastParse(int toConvert)
        {
            //String tempString = new String((char)(toConvert + 0x30));
            //char[] tempChar = { (char)(toConvert+0x30), (char)0 };
            char[] tempChar = { (char)(toConvert + 0x30)};
            
            return new String(tempChar);
        }

        private void writeSudoku(SudokuBoard toUpdate)
        {
            for (int i = 0; i < SudokuBoard.SUDOKU_SIZE; i++)
            {
                for (int j = 0; j < SudokuBoard.SUDOKU_SIZE; j++)
                {
                    //this.myTextBoxArray[i, j].Text = fastParse(toUpdate.readCell(i, j).getValue());
                    this.myTextBoxArray[i, j].Text = fastParse(toUpdate.readCell(i, j).value);
                }
            }
        }
        #endregion



        #region This code is used to force text selection when CLICK or KEYBOARD_FOCUS is achieved
        private static void SelectivelyHandleMouseButton(object sender, MouseButtonEventArgs e)
        {
            var textbox = (sender as TextBox);
            if (textbox != null && !textbox.IsKeyboardFocusWithin)
            {
                if (e.OriginalSource.GetType().Name == "TextBoxView")
                {
                    e.Handled = true;
                    textbox.Focus();
                }
            }
        }

        private static void SelectAllText(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
                textBox.SelectAll();
        }

        private void RegisterClassHandlers()
        {
            EventManager.RegisterClassHandler(typeof(TextBox),
                                           UIElement.PreviewMouseLeftButtonDownEvent,
                                           new MouseButtonEventHandler(SelectivelyHandleMouseButton),
                                           true);

            EventManager.RegisterClassHandler(typeof(TextBox),
                                                UIElement.GotKeyboardFocusEvent,
                                                new RoutedEventHandler(SelectAllText),
                                                true);
        }
        

        /// <summary>
        /// This method will handle the ClearBoard_Btn button click and will
        /// erase the entire sudoku board.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearBoard_Btn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Start removing values from sudoku board
            this.clearBoard();
        }


        private void clearBoard()
        {            
            #region Clearing Cells...
            this.Cell_A1.Text = " ";
            this.Cell_A2.Text = " ";
            this.Cell_A3.Text = " ";
            this.Cell_A4.Text = " ";
            this.Cell_A5.Text = " ";
            this.Cell_A6.Text = " ";
            this.Cell_A7.Text = " ";
            this.Cell_A8.Text = " ";
            this.Cell_A9.Text = " ";
            this.Cell_B1.Text = " ";
            this.Cell_B2.Text = " ";
            this.Cell_B3.Text = " ";
            this.Cell_B4.Text = " ";
            this.Cell_B5.Text = " ";
            this.Cell_B6.Text = " ";
            this.Cell_B7.Text = " ";
            this.Cell_B8.Text = " ";
            this.Cell_B9.Text = " ";
            this.Cell_C1.Text = " ";
            this.Cell_C2.Text = " ";
            this.Cell_C3.Text = " ";
            this.Cell_C4.Text = " ";
            this.Cell_C5.Text = " ";
            this.Cell_C6.Text = " ";
            this.Cell_C7.Text = " ";
            this.Cell_C8.Text = " ";
            this.Cell_C9.Text = " ";
            this.Cell_D1.Text = " ";
            this.Cell_D2.Text = " ";
            this.Cell_D3.Text = " ";
            this.Cell_D4.Text = " ";
            this.Cell_D5.Text = " ";
            this.Cell_D6.Text = " ";
            this.Cell_D7.Text = " ";
            this.Cell_D8.Text = " ";
            this.Cell_D9.Text = " ";
            this.Cell_E1.Text = " ";
            this.Cell_E2.Text = " ";
            this.Cell_E3.Text = " ";
            this.Cell_E4.Text = " ";
            this.Cell_E5.Text = " ";
            this.Cell_E6.Text = " ";
            this.Cell_E7.Text = " ";
            this.Cell_E8.Text = " ";
            this.Cell_E9.Text = " ";
            this.Cell_F1.Text = " ";
            this.Cell_F2.Text = " ";
            this.Cell_F3.Text = " ";
            this.Cell_F4.Text = " ";
            this.Cell_F5.Text = " ";
            this.Cell_F6.Text = " ";
            this.Cell_F7.Text = " ";
            this.Cell_F8.Text = " ";
            this.Cell_F9.Text = " ";
            this.Cell_G1.Text = " ";
            this.Cell_G2.Text = " ";
            this.Cell_G3.Text = " ";
            this.Cell_G4.Text = " ";
            this.Cell_G5.Text = " ";
            this.Cell_G6.Text = " ";
            this.Cell_G7.Text = " ";
            this.Cell_G8.Text = " ";
            this.Cell_G9.Text = " ";
            this.Cell_H1.Text = " ";
            this.Cell_H2.Text = " ";
            this.Cell_H3.Text = " ";
            this.Cell_H4.Text = " ";
            this.Cell_H5.Text = " ";
            this.Cell_H6.Text = " ";
            this.Cell_H7.Text = " ";
            this.Cell_H8.Text = " ";
            this.Cell_H9.Text = " ";
            this.Cell_I1.Text = " ";
            this.Cell_I2.Text = " ";
            this.Cell_I3.Text = " ";
            this.Cell_I4.Text = " ";
            this.Cell_I5.Text = " ";
            this.Cell_I6.Text = " ";
            this.Cell_I7.Text = " ";
            this.Cell_I8.Text = " ";
            this.Cell_I9.Text = " ";
            #endregion

            #region Reseting color formating
            this.Cell_A1.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_A2.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_A3.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_A4.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_A5.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_A6.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_A7.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_A8.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_A9.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_B1.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_B2.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_B3.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_B4.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_B5.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_B6.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_B7.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_B8.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_B9.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_C1.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_C2.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_C3.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_C4.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_C5.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_C6.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_C7.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_C8.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_C9.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_D1.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_D2.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_D3.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_D4.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_D5.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_D6.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_D7.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_D8.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_D9.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_E1.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_E2.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_E3.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_E4.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_E5.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_E6.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_E7.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_E8.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_E9.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_F1.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_F2.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_F3.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_F4.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_F5.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_F6.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_F7.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_F8.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_F9.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_G1.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_G2.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_G3.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_G4.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_G5.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_G6.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_G7.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_G8.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_G9.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_H1.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_H2.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_H3.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_H4.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_H5.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_H6.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_H7.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_H8.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_H9.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_I1.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_I2.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_I3.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_I4.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_I5.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_I6.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_I7.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_I8.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_I9.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            #endregion

            #region Clearing Valid Values Labels
            this.ValidValues_A1.Content = " ";
            this.ValidValues_A2.Content = " ";
            this.ValidValues_A3.Content = " ";
            this.ValidValues_A4.Content = " ";
            this.ValidValues_A5.Content = " ";
            this.ValidValues_A6.Content = " ";
            this.ValidValues_A7.Content = " ";
            this.ValidValues_A8.Content = " ";
            this.ValidValues_A9.Content = " ";
            this.ValidValues_B1.Content = " ";
            this.ValidValues_B2.Content = " ";
            this.ValidValues_B3.Content = " ";
            this.ValidValues_B4.Content = " ";
            this.ValidValues_B5.Content = " ";
            this.ValidValues_B6.Content = " ";
            this.ValidValues_B7.Content = " ";
            this.ValidValues_B8.Content = " ";
            this.ValidValues_B9.Content = " ";
            this.ValidValues_C1.Content = " ";
            this.ValidValues_C2.Content = " ";
            this.ValidValues_C3.Content = " ";
            this.ValidValues_C4.Content = " ";
            this.ValidValues_C5.Content = " ";
            this.ValidValues_C6.Content = " ";
            this.ValidValues_C7.Content = " ";
            this.ValidValues_C8.Content = " ";
            this.ValidValues_C9.Content = " ";
            this.ValidValues_D1.Content = " ";
            this.ValidValues_D2.Content = " ";
            this.ValidValues_D3.Content = " ";
            this.ValidValues_D4.Content = " ";
            this.ValidValues_D5.Content = " ";
            this.ValidValues_D6.Content = " ";
            this.ValidValues_D7.Content = " ";
            this.ValidValues_D8.Content = " ";
            this.ValidValues_D9.Content = " ";
            this.ValidValues_E1.Content = " ";
            this.ValidValues_E2.Content = " ";
            this.ValidValues_E3.Content = " ";
            this.ValidValues_E4.Content = " ";
            this.ValidValues_E5.Content = " ";
            this.ValidValues_E6.Content = " ";
            this.ValidValues_E7.Content = " ";
            this.ValidValues_E8.Content = " ";
            this.ValidValues_E9.Content = " ";
            this.ValidValues_F1.Content = " ";
            this.ValidValues_F2.Content = " ";
            this.ValidValues_F3.Content = " ";
            this.ValidValues_F4.Content = " ";
            this.ValidValues_F5.Content = " ";
            this.ValidValues_F6.Content = " ";
            this.ValidValues_F7.Content = " ";
            this.ValidValues_F8.Content = " ";
            this.ValidValues_F9.Content = " ";
            this.ValidValues_G1.Content = " ";
            this.ValidValues_G2.Content = " ";
            this.ValidValues_G3.Content = " ";
            this.ValidValues_G4.Content = " ";
            this.ValidValues_G5.Content = " ";
            this.ValidValues_G6.Content = " ";
            this.ValidValues_G7.Content = " ";
            this.ValidValues_G8.Content = " ";
            this.ValidValues_G9.Content = " ";
            this.ValidValues_H1.Content = " ";
            this.ValidValues_H2.Content = " ";
            this.ValidValues_H3.Content = " ";
            this.ValidValues_H4.Content = " ";
            this.ValidValues_H5.Content = " ";
            this.ValidValues_H6.Content = " ";
            this.ValidValues_H7.Content = " ";
            this.ValidValues_H8.Content = " ";
            this.ValidValues_H9.Content = " ";
            this.ValidValues_I1.Content = " ";
            this.ValidValues_I2.Content = " ";
            this.ValidValues_I3.Content = " ";
            this.ValidValues_I4.Content = " ";
            this.ValidValues_I5.Content = " ";
            this.ValidValues_I6.Content = " ";
            this.ValidValues_I7.Content = " ";
            this.ValidValues_I8.Content = " ";
            this.ValidValues_I9.Content = " ";
            #endregion
            clearStaticsFields();

            if (this.AnimateToggle_Sw.IsChecked == true)
            {
                this.AnimateToggle_Sw.IsChecked = false;
            }
//            else
            {
  //              SudokuBoard.currentSolvingStatus = SudokuBoard.SolvingStatus.ABORTED;
            }

            SudokuBoard.currentSolvingStatus = SudokuBoard.SolvingStatus.RUNNING;
        }

        private void resetCellForeground()
        {
            #region Reset Cell foreground
            this.Cell_A1.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_A2.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_A3.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_A4.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_A5.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_A6.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_A7.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_A8.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_A9.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_B1.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_B2.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_B3.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_B4.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_B5.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_B6.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_B7.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_B8.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_B9.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_C1.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_C2.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_C3.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_C4.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_C5.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_C6.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_C7.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_C8.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_C9.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_D1.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_D2.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_D3.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_D4.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_D5.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_D6.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_D7.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_D8.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_D9.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_E1.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_E2.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_E3.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_E4.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_E5.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_E6.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_E7.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_E8.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_E9.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_F1.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_F2.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_F3.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_F4.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_F5.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_F6.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_F7.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_F8.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_F9.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_G1.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_G2.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_G3.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_G4.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_G5.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_G6.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_G7.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_G8.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_G9.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_H1.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_H2.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_H3.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_H4.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_H5.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_H6.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_H7.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_H8.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_H9.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_I1.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_I2.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_I3.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_I4.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_I5.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_I6.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_I7.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_I8.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.Cell_I9.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            #endregion
        }

        private void clearStaticsFields()
        {

            this.TimeElapsed_Lbl.Content = String.Empty;
            this.TotalTriedValues_Lbl.Content = String.Empty;
            this.BackTrackCount_Lbl.Content = String.Empty;

            this.TotalTime_Lbl.Content = String.Empty;

            this.Status_Lbl.Content = String.Empty;
            this.Status_Lbl.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        }

        private void PreloadedSudoku_Dbox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string tempSelectionStr = this.PreloadedSudoku_Dbox.Items[this.PreloadedSudoku_Dbox.SelectedIndex].ToString();

            this.clearBoard();

            if (tempSelectionStr == "Easy")
            {
                #region Easy dificulty sudoku
                this.Cell_A1.Text = " ";
                this.Cell_A2.Text = "9";
                this.Cell_A3.Text = " ";
                this.Cell_A4.Text = "7";
                this.Cell_A5.Text = "1";
                this.Cell_A6.Text = " ";
                this.Cell_A7.Text = "2";
                this.Cell_A8.Text = "8";
                this.Cell_A9.Text = " ";

                this.Cell_B1.Text = "7";
                this.Cell_B2.Text = "2";
                this.Cell_B3.Text = "8";
                this.Cell_B4.Text = "4";
                this.Cell_B5.Text = " ";
                this.Cell_B6.Text = "5";
                this.Cell_B7.Text = "1";
                this.Cell_B8.Text = " ";
                this.Cell_B9.Text = " ";

                this.Cell_C1.Text = " ";
                this.Cell_C2.Text = " ";
                this.Cell_C3.Text = " ";
                this.Cell_C4.Text = " ";
                this.Cell_C5.Text = "2";
                this.Cell_C6.Text = " ";
                this.Cell_C7.Text = "6";
                this.Cell_C8.Text = " ";
                this.Cell_C9.Text = " ";

                this.Cell_D1.Text = "3";
                this.Cell_D2.Text = " ";
                this.Cell_D3.Text = "9";
                this.Cell_D4.Text = "1";
                this.Cell_D5.Text = "5";
                this.Cell_D6.Text = "8";
                this.Cell_D7.Text = " ";
                this.Cell_D8.Text = " ";
                this.Cell_D9.Text = " ";

                this.Cell_E1.Text = " ";
                this.Cell_E2.Text = " ";
                this.Cell_E3.Text = "5";
                this.Cell_E4.Text = " ";
                this.Cell_E5.Text = "4";
                this.Cell_E6.Text = "6";
                this.Cell_E7.Text = " ";
                this.Cell_E8.Text = " ";
                this.Cell_E9.Text = "1";

                this.Cell_F1.Text = " ";
                this.Cell_F2.Text = " ";
                this.Cell_F3.Text = "6";
                this.Cell_F4.Text = " ";
                this.Cell_F5.Text = "3";
                this.Cell_F6.Text = " ";
                this.Cell_F7.Text = " ";
                this.Cell_F8.Text = " ";
                this.Cell_F9.Text = "8";

                this.Cell_G1.Text = "4";
                this.Cell_G2.Text = "5";
                this.Cell_G3.Text = "2";
                this.Cell_G4.Text = "3";
                this.Cell_G5.Text = " ";
                this.Cell_G6.Text = " ";
                this.Cell_G7.Text = " ";
                this.Cell_G8.Text = " ";
                this.Cell_G9.Text = "6";

                this.Cell_H1.Text = "1";
                this.Cell_H2.Text = "8";
                this.Cell_H3.Text = "7";
                this.Cell_H4.Text = " ";
                this.Cell_H5.Text = "9";
                this.Cell_H6.Text = " ";
                this.Cell_H7.Text = " ";
                this.Cell_H8.Text = " ";
                this.Cell_H9.Text = "3";

                this.Cell_I1.Text = " ";
                this.Cell_I2.Text = " ";
                this.Cell_I3.Text = " ";
                this.Cell_I4.Text = " ";
                this.Cell_I5.Text = "7";
                this.Cell_I6.Text = " ";
                this.Cell_I7.Text = " ";
                this.Cell_I8.Text = " ";
                this.Cell_I9.Text = " ";
                #endregion
            }
            else if (tempSelectionStr == "Medium")
            {
                #region Medium dificulty sudoku
                this.Cell_A1.Text = " ";
                this.Cell_A2.Text = "2";
                this.Cell_A3.Text = " ";
                this.Cell_A4.Text = " ";
                this.Cell_A5.Text = " ";
                this.Cell_A6.Text = " ";
                this.Cell_A7.Text = "9";
                this.Cell_A8.Text = " ";
                this.Cell_A9.Text = " ";

                this.Cell_B1.Text = " ";
                this.Cell_B2.Text = " ";
                this.Cell_B3.Text = " ";
                this.Cell_B4.Text = "8";
                this.Cell_B5.Text = "1";
                this.Cell_B6.Text = " ";
                this.Cell_B7.Text = " ";
                this.Cell_B8.Text = "5";
                this.Cell_B9.Text = " ";

                this.Cell_C1.Text = "5";
                this.Cell_C2.Text = " ";
                this.Cell_C3.Text = "6";
                this.Cell_C4.Text = " ";
                this.Cell_C5.Text = " ";
                this.Cell_C6.Text = " ";
                this.Cell_C7.Text = " ";
                this.Cell_C8.Text = " ";
                this.Cell_C9.Text = "8";

                this.Cell_D1.Text = "2";
                this.Cell_D2.Text = " ";
                this.Cell_D3.Text = " ";
                this.Cell_D4.Text = " ";
                this.Cell_D5.Text = " ";
                this.Cell_D6.Text = "9";
                this.Cell_D7.Text = "3";
                this.Cell_D8.Text = " ";
                this.Cell_D9.Text = " ";

                this.Cell_E1.Text = " ";
                this.Cell_E2.Text = "8";
                this.Cell_E3.Text = " ";
                this.Cell_E4.Text = " ";
                this.Cell_E5.Text = "6";
                this.Cell_E6.Text = " ";
                this.Cell_E7.Text = " ";
                this.Cell_E8.Text = "1";
                this.Cell_E9.Text = " ";

                this.Cell_F1.Text = " ";
                this.Cell_F2.Text = " ";
                this.Cell_F3.Text = "7";
                this.Cell_F4.Text = "4";
                this.Cell_F5.Text = " ";
                this.Cell_F6.Text = " ";
                this.Cell_F7.Text = " ";
                this.Cell_F8.Text = " ";
                this.Cell_F9.Text = "6";

                this.Cell_G1.Text = "9";
                this.Cell_G2.Text = " ";
                this.Cell_G3.Text = " ";
                this.Cell_G4.Text = " ";
                this.Cell_G5.Text = " ";
                this.Cell_G6.Text = " ";
                this.Cell_G7.Text = "4";
                this.Cell_G8.Text = " ";
                this.Cell_G9.Text = "7";

                this.Cell_H1.Text = " ";
                this.Cell_H2.Text = "3";
                this.Cell_H3.Text = " ";
                this.Cell_H4.Text = " ";
                this.Cell_H5.Text = "5";
                this.Cell_H6.Text = "2";
                this.Cell_H7.Text = " ";
                this.Cell_H8.Text = " ";
                this.Cell_H9.Text = " ";

                this.Cell_I1.Text = " ";
                this.Cell_I2.Text = " ";
                this.Cell_I3.Text = "8";
                this.Cell_I4.Text = " ";
                this.Cell_I5.Text = " ";
                this.Cell_I6.Text = " ";
                this.Cell_I7.Text = " ";
                this.Cell_I8.Text = "6";
                this.Cell_I9.Text = " ";
                #endregion
            }
            else if (tempSelectionStr == "Hard")
            {
                #region Loading Hard Sudoku
                this.Cell_A1.Text = "8";
                this.Cell_A2.Text = " ";
                this.Cell_A3.Text = " ";
                this.Cell_A4.Text = " ";
                this.Cell_A5.Text = " ";
                this.Cell_A6.Text = " ";
                this.Cell_A7.Text = " ";
                this.Cell_A8.Text = " ";
                this.Cell_A9.Text = " ";

                this.Cell_B1.Text = " ";
                this.Cell_B2.Text = " ";
                this.Cell_B3.Text = "3";
                this.Cell_B4.Text = "6";
                this.Cell_B5.Text = " ";
                this.Cell_B6.Text = " ";
                this.Cell_B7.Text = " ";
                this.Cell_B8.Text = " ";
                this.Cell_B9.Text = " ";

                this.Cell_C1.Text = " ";
                this.Cell_C2.Text = "7";
                this.Cell_C3.Text = " ";
                this.Cell_C4.Text = " ";
                this.Cell_C5.Text = "9";
                this.Cell_C6.Text = " ";
                this.Cell_C7.Text = "2";
                this.Cell_C8.Text = " ";
                this.Cell_C9.Text = " ";

                this.Cell_D1.Text = " ";
                this.Cell_D2.Text = "5";
                this.Cell_D3.Text = " ";
                this.Cell_D4.Text = " ";
                this.Cell_D5.Text = " ";
                this.Cell_D6.Text = "7";
                this.Cell_D7.Text = " ";
                this.Cell_D8.Text = " ";
                this.Cell_D9.Text = " ";

                this.Cell_E1.Text = " ";
                this.Cell_E2.Text = " ";
                this.Cell_E3.Text = " ";
                this.Cell_E4.Text = " ";
                this.Cell_E5.Text = "4";
                this.Cell_E6.Text = "5";
                this.Cell_E7.Text = "7";
                this.Cell_E8.Text = " ";
                this.Cell_E9.Text = " ";

                this.Cell_F1.Text = " ";
                this.Cell_F2.Text = " ";
                this.Cell_F3.Text = " ";
                this.Cell_F4.Text = "1";
                this.Cell_F5.Text = " ";
                this.Cell_F6.Text = " ";
                this.Cell_F7.Text = " ";
                this.Cell_F8.Text = "3";
                this.Cell_F9.Text = " ";

                this.Cell_G1.Text = " ";
                this.Cell_G2.Text = " ";
                this.Cell_G3.Text = "1";
                this.Cell_G4.Text = " ";
                this.Cell_G5.Text = " ";
                this.Cell_G6.Text = " ";
                this.Cell_G7.Text = "3";
                this.Cell_G8.Text = " ";
                this.Cell_G9.Text = " ";

                this.Cell_H1.Text = " ";
                this.Cell_H2.Text = " ";
                this.Cell_H3.Text = "8";
                this.Cell_H4.Text = "5";
                this.Cell_H5.Text = " ";
                this.Cell_H6.Text = " ";
                this.Cell_H7.Text = " ";
                this.Cell_H8.Text = "1";
                this.Cell_H9.Text = " ";

                this.Cell_I1.Text = " ";
                this.Cell_I2.Text = "9";
                this.Cell_I3.Text = " ";
                this.Cell_I4.Text = " ";
                this.Cell_I5.Text = " ";
                this.Cell_I6.Text = " ";
                this.Cell_I7.Text = "4";
                this.Cell_I8.Text = " ";
                this.Cell_I9.Text = " ";

                #endregion
            }
            if (this.AnimateToggle_Sw.IsChecked == true)
            {
                this.AnimateToggle_Sw.IsChecked = false;
            }

        }

        private void CancelAnimation_Btn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SudokuBoard.currentSolvingStatus = SudokuBoard.SolvingStatus.ABORTED;
        }


        private SudokuBoard.SolvingOptimizations getOptimizationLevel()
        {
            
            if (this.OptimizationLevel_Sld.Value == 0)
            {/* SIMPLE BACKTRACKING */
                return SudokuBoard.SolvingOptimizations.SIMPLE_BACKTRACK;
            }
            else if (this.OptimizationLevel_Sld.Value == 1)
            {/* FORWARD CHECK */
                return SudokuBoard.SolvingOptimizations.FORWARD_CHECK;
            }
            else if (this.OptimizationLevel_Sld.Value == 2)
            {/* CONTRAINT PROPAGATION */
                return SudokuBoard.SolvingOptimizations.CONTRAINT_PROP;
            }
            else 
            {/* UNIQUE UPDATE */
                return SudokuBoard.SolvingOptimizations.UNIQUE_UPDATE;
            }
        }


        private void InstantSolve_Btn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var globalSw = new Stopwatch();
            var sudokuCreationsw = new Stopwatch();
            var sw = new Stopwatch();
            var lSw = new Stopwatch();
            var staticsSw = new Stopwatch();

            globalSw.Start();

        	/* Setting Instant solve values */
            classes.SudokuBoard.solvingMode = SudokuBoard.SolvingModes.INSTANT;
            classes.SudokuBoard.writeCounter = 0;
            classes.SudokuBoard.backtrackCounter = 0;
            classes.SudokuBoard.currentSolvingOptimization = this.getOptimizationLevel();
            

            bool sudokuResult;
            
            sudokuCreationsw.Start();

            /* Create empty sudoku board */  
            this.sudokuBoard = new SudokuBoard();

            sudokuCreationsw.Stop();
            Console.WriteLine("Sudoku Creation time = " + sudokuCreationsw.ElapsedMilliseconds.ToString());


            /* Read values of initial sudoku puzzle */
            this.readSudokuBoard();

            
            sw.Start();
            /* Start solving sodoku */
            sudokuResult = this.sudokuBoard.solveSudokuInternal();
            
            sw.Stop();
            this.TimeElapsed_Lbl.Content = sw.ElapsedMilliseconds.ToString() + "ms";
            Console.WriteLine("Algorithm time = " + sw.ElapsedMilliseconds.ToString());

            if (sudokuResult == true)
            {
                lSw.Start();
                /* A solution has been found, print results!!!! */
                this.writeSudoku(this.sudokuBoard);
                
                lSw.Stop();
                Console.WriteLine("Writting time " + lSw.ElapsedMilliseconds.ToString());

                

                this.TotalTriedValues_Lbl.Content = SudokuBoard.writeCounter.ToString();
                this.BackTrackCount_Lbl.Content = SudokuBoard.backtrackCounter.ToString();

                this.Status_Lbl.Content = "Solved!";
                this.Status_Lbl.Foreground = new SolidColorBrush(Color.FromArgb(255, 71, 178, 0));

                staticsSw.Stop();
                Console.WriteLine("Statistics time " + staticsSw.ElapsedMilliseconds.ToString());
            }
            else
            {
                /* No solution found, warn tester */
                this.Status_Lbl.Content = "Solution Not found";
                this.Status_Lbl.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 93, 0)); 
            }

            globalSw.Stop();
            Console.WriteLine("Total time = " + globalSw.ElapsedMilliseconds.ToString());
            this.TotalTime_Lbl.Content = globalSw.ElapsedMilliseconds.ToString() + "ms";

        }


        private void AnimationTime_Sld_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue >= 0)
            {
                Int32 tempDouble = (Int32)e.NewValue;
                string tempString = tempDouble.ToString();
                if (this.AnimationTime_Lbl != null)
                {
                    this.AnimationTime_Lbl.Content = tempString;
                    SudokuBoard.animationDelay = tempDouble;
                }   
            }
        }

        

        private void HaltSolve_Btn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
            if (SudokuBoard.currentSolvingStatus != SudokuBoard.SolvingStatus.HALTED)
            {
                SudokuBoard.currentSolvingStatus = SudokuBoard.SolvingStatus.HALTED;
            }
            else
            {
                SudokuBoard.currentSolvingStatus = SudokuBoard.SolvingStatus.RUNNING;
            }
        }

        private void AnimateToggle_Sw_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (SudokuBoard.currentSolvingStatus != SudokuBoard.SolvingStatus.HALTED)
            {
                this.sudokuBoard = new SudokuBoard();

                resetCellForeground();

                /* Setting Animated values */
                classes.SudokuBoard.solvingMode = SudokuBoard.SolvingModes.ANIMATED;
                /* Setting Animation delay value (defined in ms) */
                classes.SudokuBoard.animationDelay = (Int32)this.AnimationTime_Sld.Value;
                /* Setting status */
                classes.SudokuBoard.currentSolvingStatus = SudokuBoard.SolvingStatus.RUNNING;
                /* Reset Counters */
                classes.SudokuBoard.writeCounter = 0;

                classes.SudokuBoard.currentSolvingOptimization = this.getOptimizationLevel();

                classes.SudokuBoard.backtrackCounter = 0;

                this.Status_Lbl.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

                this.readSudokuBoard();

                this.sudokuSolving_Thread = new Thread(new ThreadStart(this.sudokuBoard.solveSudoku_));

                this.sudokuSolving_Thread.Start();

                this.TimeElapsed_Lbl.Content = "NA";

                this.TotalTime_Lbl.Content = "NA";

                this.Status_Lbl.Content = "Running...";
            }

            SudokuBoard.currentSolvingStatus = SudokuBoard.SolvingStatus.RUNNING;
        }

        private void AnimateToggle_Sw_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
        	SudokuBoard.currentSolvingStatus = SudokuBoard.SolvingStatus.HALTED;
        }

        private void OptimizationLevel_Sld_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
        	// TODO: Add event handler implementation here.
        }

   

      



        #endregion

		


      
    }
}