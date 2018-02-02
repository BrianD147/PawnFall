﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PawnFall
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        int[,] chessboardMap = new int[7, 7]; //an integer array of chesspieces
        Rectangle[,] chessSquares = new Rectangle[7, 7]; //a 2 dimensional array of all chessboard squares
        Rectangle[,] backgroundBoard = new Rectangle[7, 7]; //a 2 dimentional array of the chessboard background itself
        int color = 0; //counter to determine wether square should be white or green
        bool isPathHighlighted = false; //bool to tell if a square is highlighted or not
        int[] pieceCoordinate = new int[2]; //holds tapped piece coordinated if player wants to move it to another square
        int[,] blackPawns = new int[7, 7]; // integer array of black pawn positions

        public MainPage()
        {
            this.InitializeComponent();
            BoardSetup();
            PieceSetup();
        }

        private void BoardSetup() //Sets up the chessboard background grid
        {
            //load pieces into correct positions
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    LoadSquares(i, j, color);
                    color++;
                }

            }
        }

        private void LoadSquares(int x, int y, int color)
        {
            Rectangle temp = new Rectangle
            {
                Height = 40,
                Width = 40
            };

            if (color%2 == 0)
            {
                temp.Fill = new SolidColorBrush(Colors.LightGreen);
            }
            else
            {
                temp.Fill = new SolidColorBrush(Colors.Beige);
            }

            Grid.SetRow(temp, y);
            Grid.SetColumn(temp, x);

            gChessboard.Children.Add(temp);
        }

        private void PieceSetup() //sets up the chessboard
        {
            //populate array with piece values
            //0: blank space
            //1: pawn
            //2: knight
            //3: king

            //set all to blank spaces first
            for (int i=0; i<7; i++)
            {
                for(int j=0; j<7; j++)
                {
                    chessboardMap[i, j] = 0;
                }
            }

            //set pawns
            for (int i=0; i<7; i++)
            {
                chessboardMap[5, i] = 1;
            }

            //set knights
            chessboardMap[6, 1] = 2;
            chessboardMap[6, 5] = 2;

            //set king
            chessboardMap[6, 3] = 3;

            //set black pawn (for testing)
            chessboardMap[6, 2] = -1;

            //load pieces into correct positions
            for (int i=0; i<7; i++)
            {
                for(int j=0; j<7; j++)
                {
                    LoadPieces(chessboardMap[i, j], i, j);
                }

            }

        }

        private void LoadPieces(int piece, int x, int y)//loads a chess piece into correct position
        {
            Rectangle temp = new Rectangle //declares a temp rectangle which will be given a piece and put onto the chessboard position
            {
                Height = 40,
                Width = 40,
            };
            temp.Tapped += SquareTapped;

            temp.Stroke = new SolidColorBrush(Colors.Gold);

            ImageBrush img = new ImageBrush();
            switch (piece)
            {
                case 0:
                    temp.Fill = new SolidColorBrush(Colors.Transparent);
                    break;
                case 1:
                    img.ImageSource = new BitmapImage(new Uri("ms-appx:///Image/pawnWhite.png", UriKind.RelativeOrAbsolute));
                    temp.Fill = img;
                    //temp.Stroke = new SolidColorBrush(Colors.Gray);
                    break;
                case 2:
                    img.ImageSource = new BitmapImage(new Uri("ms-appx:///Image/knightWhite.png", UriKind.RelativeOrAbsolute));
                    temp.Fill = img;
                    //temp.Stroke = new SolidColorBrush(Colors.Gray);
                    break;
                case 3:
                    img.ImageSource = new BitmapImage(new Uri("ms-appx:///Image/kingWhite.png", UriKind.RelativeOrAbsolute));
                    temp.Fill = img;
                    //temp.Stroke = new SolidColorBrush(Colors.Gray);
                    break;
                case -1:
                    img.ImageSource = new BitmapImage(new Uri("ms-appx:///Image/pawnBlack.png", UriKind.RelativeOrAbsolute));
                    temp.Fill = img;
                    //temp.Stroke = new SolidColorBrush(Colors.Gray);
                    break;
            }

            temp.StrokeThickness = 0;

            Grid.SetRow(temp, x);
            Grid.SetColumn(temp, y);
            if (chessSquares[x, y] != null)
                gPieces.Children.Remove(chessSquares[x, y]);
            chessSquares[x, y] = temp;
            gPieces.Children.Add(chessSquares[x, y]);
        }

        private void SquareTapped(object sender, TappedRoutedEventArgs e) // Chessboard square is tapped
        {
            Rectangle rect = sender as Rectangle;
            int row = Grid.GetRow(rect);
            int column = Grid.GetColumn(rect);

            if (isPathHighlighted == false)
            {
                ShowValidSquares(row, column);
                isPathHighlighted = true;
                pieceCoordinate[0] = row;
                pieceCoordinate[1] = column;
            }
            else
            {
                if (chessSquares[row, column].StrokeThickness == 3)
                {
                    ClearHighlights();
                    MovePiece(chessboardMap[row, column], pieceCoordinate[0], pieceCoordinate[1], row, column);
                    isPathHighlighted = false;
                    OpponentsTurn();
                }
                else
                {
                    ClearHighlights();
                    isPathHighlighted = false;
                }
            }
        }
        
        private void OpponentsTurn()
        {
            for (int i = 6; i >= 0; i--)
            {
                for (int j = 6; j >= 0; j--)
                {
                    if (chessboardMap[i, j] == -1)
                    {
                        if (i != 6)
                        {
                            MovePiece(chessboardMap[i + 1, j], i, j, i + 1, j);
                        }
                        else
                        {
                            GameOver();
                        }
                        
                    }
                }
            }
            Random rnd = new Random();
            int pos = rnd.Next(6);
            chessboardMap[0, pos] = -1;
            LoadPieces(chessboardMap[0, pos], 0, pos);
        }

        private void GameOver()
        {
            TextBlock gameoverMsg = new TextBlock
            {
                Opacity = 0,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 40,
                FontWeight = FontWeights.Bold,
                MaxLines = 2,
                Text = "Game Over!",
                Foreground = new SolidColorBrush(Colors.Black),
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, -100, 0, 0)
            };


            Button tryAgainBtn = new Button
            {
                Height = 50,
                Width = 150,
                Opacity = 0,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 20,
                Content = "Play again!"
            };
            tryAgainBtn.Tapped += Button_Tapped;
            tryAgainBtn.Background = new SolidColorBrush(Colors.DarkGray);
            tryAgainBtn.Margin = new Thickness(0, 100, 0, 0);

            gGameover.Children.Add(gameoverMsg);
            gGameover.Children.Add(tryAgainBtn);

            Storyboard stb = new Storyboard();
            DoubleAnimation appear = new DoubleAnimation();
            DoubleAnimation appear2 = new DoubleAnimation();

            appear.To = 1.0;
            appear.Duration = new Duration(TimeSpan.FromMilliseconds(1500));
            Storyboard.SetTarget(appear, gameoverMsg);
            Storyboard.SetTargetProperty(appear, "Opacity");

            appear2.To = 1.0;
            appear2.Duration = new Duration(TimeSpan.FromMilliseconds(1500));
            Storyboard.SetTarget(appear2, tryAgainBtn);
            Storyboard.SetTargetProperty(appear2, "Opacity");
            
            stb.Children.Add(appear);
            stb.Children.Add(appear2);
            stb.Begin();
        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void MovePiece(int piece, int x, int y, int destX, int destY)
        {
            chessboardMap[destX, destY] = chessboardMap[x, y];
            LoadPieces(chessboardMap[destX, destY], destX, destY);

            chessboardMap[x, y] = 0;
            LoadPieces(chessboardMap[x, y], x, y);
        }

        private void ClearHighlights()
        {
            //clear the current highlights on board
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    chessSquares[i, j].StrokeThickness = 0;
                }
            }
        }

        private void ShowValidSquares(int x, int y)
        {
            switch (chessboardMap[x, y])
            {
                case 1: // White pawn
                    HighlightSelectedTile(x, y);

                    // If pawn has space infront of it
                    if (chessboardMap[x - 1, y] == 0)
                    {
                        HighlightTile(x - 1, y);
                        // If pawn is at starting position offer a 2 stepped move
                        if (x == 5 && chessboardMap[x - 2, y] == 0)
                        {
                            HighlightTile(x - 2, y);
                        }
                    }

                    //Taking pieces
                    //If pawn isnt against an edge check both diagonals
                    if (y > 0 && y < 6)
                    {
                        //Check right diagonal
                        if (chessboardMap[x - 1, y + 1] < 0)
                        {
                            HighlightTile(x - 1, y + 1);
                        }

                        //Check left diagonal
                        if (chessboardMap[x - 1, y - 1] < 0)
                        {
                            HighlightTile(x - 1, y - 1);
                        }
                    }

                    //If pawn is against left edge only check right diagonal
                    if (y == 0)
                    {
                        //Check right diagonal
                        if (chessboardMap[x - 1, y + 1] < 0)
                        {
                            HighlightTile(x - 1, y + 1);
                        }
                    }

                    //If pawn is against right edge only check left diagonal
                    if (y == 6)
                    {
                        //Check left diagonal
                        if (chessboardMap[x - 1, y - 1] < 0)
                        {
                            HighlightTile(x - 1, y - 1);
                        }
                    }



                    break;
                case 2: // White Knight
                    HighlightSelectedTile(x, y);

                    //Check for 1 oclock position
                    if (x > 1 && y < 6)
                    {
                        if (chessboardMap[x - 2, y + 1] <= 0)
                        {
                            HighlightTile(x - 2, y + 1);
                        }
                    }
                    //Check for 2 oclock position
                    if (x > 0 && y < 5)
                    {
                        if (chessboardMap[x - 1, y + 2] <= 0)
                        {
                            HighlightTile(x - 1, y + 2);
                        }
                    }
                    //Check for 4 oclock position
                    if (x < 6 && y < 5)
                    {
                        if (chessboardMap[x + 1, y + 2] <= 0)
                        {
                            HighlightTile(x + 1, y + 2);
                        }
                    }
                    //Check for 5 oclock position
                    if (x < 5 && y < 6)
                    {
                        if (chessboardMap[x + 2, y + 1] <= 0)
                        {
                            HighlightTile(x + 2, y + 1);
                        }
                    }
                    //Check for 7 oclock position
                    if (x < 5 && y > 0)
                    {
                        if (chessboardMap[x + 2, y - 1] <= 0)
                        {
                            HighlightTile(x + 2, y - 1);
                        }
                    }
                    //Check for 8 oclock position
                    if (x < 6 && y > 1)
                    {
                        if (chessboardMap[x + 1, y - 2] <= 0)
                        {
                            HighlightTile(x + 1, y - 2);
                        }
                    }
                    //Check for 10 oclock position
                    if (x > 0 && y > 1)
                    {
                        if (chessboardMap[x - 1, y - 2] <= 0)
                        {
                            HighlightTile(x - 1, y - 2);
                        }
                    }
                    //Check for 11 oclock position
                    if (x > 1 && y > 0)
                    {
                        if (chessboardMap[x - 2, y - 1] <= 0)
                        {
                            HighlightTile(x - 2, y - 1);
                        }
                    }
                    break;
                case 3:
                    HighlightSelectedTile(x, y);

                    //Check for NW position
                    if (x > 0 && y > 0)
                    {
                        if (chessboardMap[x - 1, y - 1] <= 0)
                        {
                            HighlightTile(x - 1, y - 1);
                        }
                    }
                    //Check for N position
                    if (x > 0)
                    {
                        if (chessboardMap[x - 1, y] <= 0)
                        {
                            HighlightTile(x - 1, y);
                        }
                    }
                    //Check for NE position
                    if (x > 0 && y < 6)
                    {
                        if (chessboardMap[x - 1, y + 1] <= 0)
                        {
                            HighlightTile(x - 1, y + 1);
                        }
                    }
                    //Check for E position
                    if (y < 6)
                    {
                        if (chessboardMap[x, y + 1] <= 0)
                        {
                            HighlightTile(x, y + 1);
                        }
                    }
                    //Check for SE position
                    if (x < 6 && y < 6)
                    {
                        if (chessboardMap[x + 1, y + 1] <= 0)
                        {
                            HighlightTile(x + 1, y + 1);
                        }
                    }
                    //Check for S position
                    if (x < 6)
                    {
                        if (chessboardMap[x + 1, y] <= 0)
                        {
                            HighlightTile(x + 1, y);
                        }
                    }
                    //Check for SW position
                    if (x < 6 && y > 0)
                    {
                        if (chessboardMap[x + 1, y - 1] <= 0)
                        {
                            HighlightTile(x + 1, y - 1);
                        }
                    }
                    //Check for W position
                    if (y > 0)
                    {
                        if (chessboardMap[x, y - 1] <= 0)
                        {
                            HighlightTile(x, y - 1);
                        }
                    }
                    break;
            }
        }

        private void HighlightSelectedTile(int x, int y)
        {
            chessSquares[x, y].Stroke = new SolidColorBrush(Colors.Gray);
            if (chessSquares[x, y].StrokeThickness == 0)
            {
                chessSquares[x, y].StrokeThickness = 2.9;
            }
            else
            {
                chessSquares[x, y].StrokeThickness = 0;
            }
        }

        private void HighlightTile(int x, int y)
        {
            if (chessSquares[x, y].StrokeThickness == 0)
            {
                chessSquares[x, y].StrokeThickness = 3;
            }
            else
            {
                chessSquares[x, y].StrokeThickness = 0;
            }
        }

        private void BtnMenu_Click(object sender, RoutedEventArgs e) //Menu button click
        {
            //tblGameTitle.Text = "Test";
        }

    }
}
