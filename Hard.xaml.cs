using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Minesweeper_Kitty_Boom
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Hard : Page
    {
        static int dimensionX = 16;
        static int dimensionY = 30;

        static int parameterX = 168;
        static int parameterY = 64;

        static int bombcount = 99;
        static int flagcount = 99;

        static int SIZE = 40;

        Button[,] buttons = new Button[dimensionX, dimensionY];

        int[,] board = new int[dimensionX, dimensionY];
        int[,] oldboard = new int[dimensionX, dimensionY];
        int[,] statusboard = new int[dimensionX, dimensionY];

        public Hard()
        {
            this.InitializeComponent();
            StackPanel gameboardGrid = new StackPanel();
            gameboardGrid.BorderThickness = new Thickness(1, 1, 1, 1);
            gameboardGrid.BorderBrush = new SolidColorBrush(Colors.Gray);

            addMines(board);
            addMinesIndicators(board);

            addMines(oldboard);
            addMinesIndicators(oldboard);

            initializeStatusBoard(statusboard);

            for (int i = 0; i < dimensionX; i++)
            {
                StackPanel rowStackPanel = new StackPanel();
                rowStackPanel.Orientation = Orientation.Horizontal;
                for (int j = 0; j < dimensionY; j++)
                {
                    buttons[i, j] = new Button();

                    addMineClick(i, j);

                    addNonMineClick(i, j);

                    addRightClick(i, j);

                    buttons[i, j].Height = SIZE;
                    buttons[i, j].Width = SIZE;
                    buttons[i, j].BorderThickness = new Thickness(2, 2, 2, 2);
                    buttons[i, j].BorderBrush = new SolidColorBrush(Colors.DarkGray);

                    rowStackPanel.Children.Add(buttons[i, j]);
                }
                gameboardGrid.Children.Add(rowStackPanel);
            }
            signBoardStackPanel.Children.Clear();
            signBoardStackPanel.Children.Add(gameboardGrid);
        }



        void initializeStatusBoard(int[,] statusboard)
        {
            for (int i = 0; i < dimensionX; i++)
                for (int j = 0; j < dimensionY; j++)
                    statusboard[i, j] = 0;
        }


        void addMineClick(int i, int j)
        {
            if (board[i, j] == 9)
            {
                buttons[i, j].Click += (s, e) =>
                {
                    for (int i1 = 0; i1 < dimensionX; i1++)
                        for (int j1 = 0; j1 < dimensionY; j1++)
                        {
                            if (board[i1, j1] == 9)
                            {
                                buttons[i1, j1].Content = "x.x";
                                buttons[i1, j1].Background = new SolidColorBrush(Colors.Red);
                            }
                        }
                    bombcount = 99;
                    flagcount = 99;
                    LoseButton_Click(s, e);
                };
            }
        }


        void addNonMineClick(int i, int j)
        {
            if (board[i, j] != 9)
            {
                buttons[i, j].Click += (s, e) =>
                {
                    var button = s as Button;
                    var ttv = button.TransformToVisual(Window.Current.Content);
                    Point screenCoords = ttv.TransformPoint(new Point(0, 0));

                    parameterY = Convert.ToInt32(buttons[0, 0].TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0)).Y);
                    parameterX = Convert.ToInt32(buttons[0, 0].TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0)).X);

                    int coordY = Convert.ToInt32((screenCoords.Y - parameterY) / SIZE);
                    int coordX = Convert.ToInt32((screenCoords.X - parameterX) / SIZE);


                    if (board[coordY, coordX] == 0)
                        Fill(buttons, coordY, coordX);
                    else
                    {
                        if (statusboard[coordY, coordX] == 2)
                        {
                            flagcount++;
                        }
                        statusboard[coordY, coordX] = 1;
                        buttons[coordY, coordX].Content = board[coordY, coordX];
                        buttons[coordY, coordX].Background = new SolidColorBrush(Colors.Green);
                    }
                };
            }
        }


        void addRightClick(int i, int j)
        {
            buttons[i, j].RightTapped += (s, e) =>
            {
                var button = s as Button;
                var ttv = button.TransformToVisual(Window.Current.Content);
                Point screenCoords = ttv.TransformPoint(new Point(0, 0));

                parameterY = Convert.ToInt32(buttons[0, 0].TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0)).Y);
                parameterX = Convert.ToInt32(buttons[0, 0].TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0)).X);

                int coordY = Convert.ToInt32((screenCoords.Y - parameterY) / SIZE);
                int coordX = Convert.ToInt32((screenCoords.X - parameterX) / SIZE);



                if (statusboard[coordY, coordX] == 0)
                {
                    flagcount--;
                    statusboard[coordY, coordX] = 2;
                    if (board[coordY, coordX] == 9)
                        bombcount--;
                    buttons[coordY, coordX].Content = "cat";
                    buttons[coordY, coordX].Background = new SolidColorBrush(Colors.Yellow);
                }
                else if (statusboard[coordY, coordX] == 2)
                {
                    flagcount++;
                    statusboard[coordY, coordX] = 0;
                    if (board[coordY, coordX] == 9)
                        bombcount++;
                    buttons[coordY, coordX].Content = "";
                    buttons[coordY, coordX].Background = new SolidColorBrush(Colors.LightGray);
                }

                if (bombcount == 0)
                {
                    bombcount = 99;
                    flagcount = 99;
                    WinButton(s, e);
                }
            };
        }


        private async void WinButton(object sender, RoutedEventArgs e)
        {
            var messageDialog = new MessageDialog("ALL YOUR CATS HAVE BEEN SAVED! Pusica will be so happy to play with them :D. But we can still save some kittens ! What do you want to do?");
            messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("Restart") { Id = 0 });
            messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("Easy") { Id = 1 });
            messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("Medium") { Id = 2 });
            var result = await messageDialog.ShowAsync();

            if ((int)result.Id == 0)
            {
                this.Frame.Navigate(typeof(Hard));
            }
            else if ((int)result.Id == 1)
            {
                this.Frame.Navigate(typeof(Easy));
            }
            else
            {
                this.Frame.Navigate(typeof(Medium));
            }
        }

        void Fill(Button[,] screen, int x, int y)
        {

            if (x < 0 || x >= dimensionX || y < 0 || y >= dimensionY)
                return;

            if (oldboard[x, y] != 0)
            {
                if (board[x, y] != 0)
                    buttons[x, y].Content = board[x, y];

                buttons[x, y].Background = new SolidColorBrush(Colors.Green);

                if (statusboard[x, y] == 2)
                {
                    flagcount++;
                }

                return;
            }

            if (statusboard[x, y] == 2)
            {
                flagcount++;
            }

            statusboard[x, y] = 1;
            buttons[x, y].Background = new SolidColorBrush(Colors.Green);
            buttons[x, y].Content = "";

            oldboard[x, y] = 1;

            Fill(screen, x + 1, y);
            Fill(screen, x - 1, y);
            Fill(screen, x, y + 1);
            Fill(screen, x, y - 1);

            Fill(screen, x + 1, y + 1);
            Fill(screen, x - 1, y - 1);
            Fill(screen, x - 1, y + 1);
            Fill(screen, x + 1, y - 1);
        }

        private async void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            bombcount = 99;
            flagcount = 99;
            var messageDialog = new MessageDialog("Where to?");
            messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("Restart") { Id = 0 });
            messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("Easy") { Id = 1 });
            messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("Medium") { Id = 2 });
            var result = await messageDialog.ShowAsync();

            if ((int)result.Id == 0)
            {
                this.Frame.Navigate(typeof(Hard));
            }
            else if ((int)result.Id == 1)
            {
                this.Frame.Navigate(typeof(Easy));
            }
            else
            {
                this.Frame.Navigate(typeof(Medium));
            }
        }

        private async void LoseButton_Click(object sender, RoutedEventArgs e)
        {
            var messageDialog = new MessageDialog("ALL YOUR CATS HAVE BLOWN! Pusica will be so sad :(. But we can still save some kittens ! What do you want to do?");
            messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("Restart") { Id = 0 });
            messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("Easy") { Id = 1 });
            messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("Medium") { Id = 2 });
            var result = await messageDialog.ShowAsync();

            if ((int)result.Id == 0)
            {
                this.Frame.Navigate(typeof(Hard));
            }
            else if ((int)result.Id == 1)
            {
                this.Frame.Navigate(typeof(Easy));
            }
            else
            {
                this.Frame.Navigate(typeof(Medium));
            }
        }

        public void addMines(int[,] board)
        {
            Random rand = new Random();

            int i, j;
            int nrBombs = 99;

            while (nrBombs != 0)
            {
                i = rand.Next(0, dimensionX);
                j = rand.Next(0, dimensionY);

                if (board[i, j] != 9)
                {
                    board[i, j] = 9;
                    nrBombs--;
                }
            }

        }

        public void addMinesIndicators(int[,] board)
        {
            int i, j;

            for (j = 1; j < dimensionY - 1; j++)
                if (board[0, j] != 9)
                    board[0, j] = countUpperLineMines(board, j);

            for (j = 1; j < dimensionY - 1; j++)
                if (board[dimensionX - 1, j] != 9)
                    board[dimensionX - 1, j] = countLastLineMines(board, j);

            for (i = 1; i < dimensionX - 1; i++)
                if (board[i, 0] != 9)
                    board[i, 0] = countLeftLineMines(board, i);

            for (i = 1; i < dimensionX - 1; i++)
                if (board[i, dimensionY - 1] != 9)
                    board[i, dimensionY - 1] = countRightLineMines(board, i);

            countCornerMines(board);

            for (i = 1; i < dimensionX - 1; i++)
                for (j = 1; j < dimensionY - 1; j++)
                {
                    if (board[i, j] != 9)
                        board[i, j] = countCenterMines(board, i, j);
                }
        }

        public int countCenterMines(int[,] board, int i, int j)
        {
            int counter = 0;

            if (board[i, j] != 9)
            {
                if (board[i - 1, j - 1] == 9)
                    counter++;

                if (board[i - 1, j] == 9)
                    counter++;

                if (board[i - 1, j + 1] == 9)
                    counter++;

                if (board[i, j - 1] == 9)
                    counter++;

                if (board[i, j + 1] == 9)
                    counter++;

                if (board[i + 1, j - 1] == 9)
                    counter++;

                if (board[i + 1, j] == 9)
                    counter++;

                if (board[i + 1, j + 1] == 9)
                    counter++;
            }

            return counter;
        }

        public int countLastLineMines(int[,] board, int j)
        {
            int counter = 0;

            if (board[dimensionX - 2, j - 1] == 9)
                counter++;

            if (board[dimensionX - 2, j + 1] == 9)
                counter++;

            if (board[dimensionX - 1, j - 1] == 9)
                counter++;

            if (board[dimensionX - 2, j] == 9)
                counter++;

            if (board[dimensionX - 1, j + 1] == 9)
                counter++;

            return counter;
        }

        public int countUpperLineMines(int[,] board, int j)
        {
            int counter = 0;

            if (board[0, j - 1] == 9)
                counter++;

            if (board[0, j + 1] == 9)
                counter++;

            if (board[1, j - 1] == 9)
                counter++;

            if (board[1, j] == 9)
                counter++;

            if (board[1, j + 1] == 9)
                counter++;

            return counter;
        }

        public int countLeftLineMines(int[,] board, int i)
        {
            int counter = 0;

            if (board[i - 1, 0] == 9)
                counter++;

            if (board[i - 1, 1] == 9)
                counter++;

            if (board[i, 1] == 9)
                counter++;

            if (board[i + 1, 1] == 9)
                counter++;

            if (board[i + 1, 0] == 9)
                counter++;

            return counter;
        }

        public int countRightLineMines(int[,] board, int i)
        {
            int counter = 0;

            if (board[i - 1, dimensionY - 1] == 9)
                counter++;

            if (board[i - 1, dimensionY - 2] == 9)
                counter++;

            if (board[i, dimensionY - 2] == 9)
                counter++;

            if (board[i + 1, dimensionY - 2] == 9)
                counter++;

            if (board[i + 1, dimensionY - 1] == 9)
                counter++;

            return counter;
        }

        public void countCornerMines(int[,] board)
        {
            int counter = 0;

            if (board[0, 0] != 9)
            {
                if (board[0, 1] == 9)
                    counter++;

                if (board[1, 0] == 9)
                    counter++;

                if (board[1, 1] == 9)
                    counter++;

                board[0, 0] = counter;
                counter = 0;
            }

            if (board[dimensionX - 1, 0] != 9)
            {
                if (board[dimensionX - 2, 0] == 9)
                    counter++;

                if (board[dimensionX - 2, 1] == 9)
                    counter++;

                if (board[dimensionX - 1, 1] == 9)
                    counter++;

                board[dimensionX - 1, 0] = counter;
                counter = 0;
            }

            if (board[0, dimensionY - 1] != 9)
            {
                if (board[0, dimensionY - 2] == 9)
                    counter++;

                if (board[1, dimensionY - 2] == 9)
                    counter++;

                if (board[1, dimensionY - 1] == 9)
                    counter++;

                board[0, dimensionY - 1] = counter;
                counter = 0;
            }

            if (board[dimensionX - 1, dimensionY - 1] != 9)
            {
                if (board[dimensionX - 2, dimensionY - 2] == 9)
                    counter++;

                if (board[dimensionX - 1, dimensionY - 2] == 9)
                    counter++;

                if (board[dimensionX - 2, dimensionY - 1] == 9)
                    counter++;

                board[dimensionX - 1, dimensionY - 1] = counter;
                counter = 0;
            }

        }
    }
}
