using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
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
    /// Author: Brian Doyle = G00330969
    /// PawnFall - Alternate game of chess where the challenge is to keep the pawns from reaching the bottom of the board
    /// </summary>
    public sealed partial class MainPage : Page
    {
        int[,] chessboardMap = new int[8, 8]; //an integer array of chesspieces
        Rectangle[,] chessSquares = new Rectangle[8, 8]; //a 2 dimensional array of all chessboard squares
        Rectangle[,] backgroundBoard = new Rectangle[8, 8]; //a 2 dimentional array of the chessboard background itself
        int color = 0; //counter to determine wether square should be white or green
        bool isPathHighlighted = false; //bool to tell if a square is highlighted or not
        int[] pieceCoordinate = new int[2]; //holds tapped piece coordinated if player wants to move it to another square
        int[,] blackPawns = new int[8, 8]; // integer array of black pawn positions
        int score = 0;
        int highScore = 0;

        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public MainPage()
        {
            this.InitializeComponent();
            Windows.Storage.ApplicationDataCompositeValue composite = (Windows.Storage.ApplicationDataCompositeValue)localSettings.Values["highScore"];

            if (composite == null)
            {
                // No data
            }
            else
            {
                highScore = (int)composite["intVal"];
            }

            BoardSetup();
            PieceSetup();
        }

        private void BoardSetup() //Sets up the chessboard background grid
        {
            //load pieces into correct positions
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    LoadSquares(i, j, color);
                    color++;
                }
                color++;
            }
        }

        private void LoadSquares(int x, int y, int color)
        {
            Rectangle temp = new Rectangle
            {
                Height = 35,
                Width = 35
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
            //4: queen
            //5: rook
            //6: bishop

            //set all to blank spaces first
            for (int i=0; i<8; i++)
            {
                for(int j=0; j<8; j++)
                {
                    chessboardMap[i, j] = 0;
                }
            }

            //set pawns
            for (int i=0; i<8; i++)
            {
                chessboardMap[6, i] = 1;
            }

            //set knights
            chessboardMap[7, 1] = 2;
            chessboardMap[7, 6] = 2;

            //set king
            chessboardMap[7, 3] = 3;

            //set queen
            chessboardMap[7, 4] = 4;

            //set rooks
            chessboardMap[7, 0] = 5;
            chessboardMap[7, 7] = 5;

            //set bishops
            chessboardMap[7, 2] = 6;
            chessboardMap[7, 5] = 6;

            //set black pawn (for testing)
            //chessboardMap[6, 2] = -1;

            //load pieces into correct positions
            for (int i=0; i<8; i++)
            {
                for(int j=0; j<8; j++)
                {
                    LoadPieces(chessboardMap[i, j], i, j);
                }
            }
        }

        private void LoadPieces(int piece, int x, int y)//loads a chess piece into correct position
        {
            Rectangle temp = new Rectangle //declares a temp rectangle which will be given a piece and put onto the chessboard position
            {
                Height = 35,
                Width = 35,
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
                case 4:
                    img.ImageSource = new BitmapImage(new Uri("ms-appx:///Image/queenWhite.png", UriKind.RelativeOrAbsolute));
                    temp.Fill = img;
                    //temp.Stroke = new SolidColorBrush(Colors.Gray);
                    break;
                case 5:
                    img.ImageSource = new BitmapImage(new Uri("ms-appx:///Image/rookWhite.png", UriKind.RelativeOrAbsolute));
                    temp.Fill = img;
                    //temp.Stroke = new SolidColorBrush(Colors.Gray);
                    break;
                case 6:
                    img.ImageSource = new BitmapImage(new Uri("ms-appx:///Image/bishopWhite.png", UriKind.RelativeOrAbsolute));
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
                    CheckIfPiece(row, column);
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

        private void CheckIfPiece(int x, int y)
        {
            if (chessboardMap[x, y] == -1)
            {
                score++;
                tblScore.Text = "" + score;
            }
        }

        private void OpponentsTurn()
        {
            for (int i = 7; i >= 0; i--)
            {
                for (int j = 7; j >= 0; j--)
                {
                    if (chessboardMap[i, j] == -1)
                    {
                        if (i != 7)
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
            int pos = rnd.Next(8);
            chessboardMap[0, pos] = -1;
            LoadPieces(chessboardMap[0, pos], 0, pos);
        }

        private void GameOver()
        {
            rectGameOver.Visibility = Visibility.Visible;
            tblGameOver.Visibility = Visibility.Visible;
            if (score > highScore)
            {
                highScore = score;
                tblHighScore.Text = "" + highScore;
            }
        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PieceSetup();
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
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
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

                    //of pawn isnt at the top of the screen
                    if (x > 0)
                    {
                        // If pawn has space infront of it
                        if (chessboardMap[x - 1, y] == 0)
                        {
                            HighlightTile(x - 1, y);
                            // If pawn is at starting position offer a 2 stepped move
                            if (x == 6 && chessboardMap[x - 2, y] == 0)
                            {
                                HighlightTile(x - 2, y);
                            }
                        }
                    
                        //Taking pieces
                        //If pawn isnt against an edge check both diagonals
                        if (y > 0 && y < 7)
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
                        if (y == 7)
                        {
                            //Check left diagonal
                            if (chessboardMap[x - 1, y - 1] < 0)
                            {
                                HighlightTile(x - 1, y - 1);
                            }
                        }
                    }


                    break;
                case 2: // White Knight
                    HighlightSelectedTile(x, y);

                    //Check for 1 oclock position
                    if (x > 1 && y < 7)
                    {
                        if (chessboardMap[x - 2, y + 1] <= 0)
                        {
                            HighlightTile(x - 2, y + 1);
                        }
                    }
                    //Check for 2 oclock position
                    if (x > 0 && y < 6)
                    {
                        if (chessboardMap[x - 1, y + 2] <= 0)
                        {
                            HighlightTile(x - 1, y + 2);
                        }
                    }
                    //Check for 4 oclock position
                    if (x < 7 && y < 6)
                    {
                        if (chessboardMap[x + 1, y + 2] <= 0)
                        {
                            HighlightTile(x + 1, y + 2);
                        }
                    }
                    //Check for 5 oclock position
                    if (x < 6 && y < 7)
                    {
                        if (chessboardMap[x + 2, y + 1] <= 0)
                        {
                            HighlightTile(x + 2, y + 1);
                        }
                    }
                    //Check for 7 oclock position
                    if (x < 6 && y > 0)
                    {
                        if (chessboardMap[x + 2, y - 1] <= 0)
                        {
                            HighlightTile(x + 2, y - 1);
                        }
                    }
                    //Check for 8 oclock position
                    if (x < 7 && y > 1)
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
                case 3: //white king
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
                    if (x > 0 && y < 7)
                    {
                        if (chessboardMap[x - 1, y + 1] <= 0)
                        {
                            HighlightTile(x - 1, y + 1);
                        }
                    }
                    //Check for E position
                    if (y < 7)
                    {
                        if (chessboardMap[x, y + 1] <= 0)
                        {
                            HighlightTile(x, y + 1);
                        }
                    }
                    //Check for SE position
                    if (x < 7 && y < 7)
                    {
                        if (chessboardMap[x + 1, y + 1] <= 0)
                        {
                            HighlightTile(x + 1, y + 1);
                        }
                    }
                    //Check for S position
                    if (x < 7)
                    {
                        if (chessboardMap[x + 1, y] <= 0)
                        {
                            HighlightTile(x + 1, y);
                        }
                    }
                    //Check for SW position
                    if (x < 7 && y > 0)
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
                case 4:
                    //Horizontal movement
                    for (int i = x + 1; i < 8; i++)
                    {
                        if (chessboardMap[i, y] == 0)
                        {
                            HighlightTile(i, y);
                        }
                        else if (chessboardMap[i, y] < 0)
                        {
                            HighlightTile(i, y);
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    for (int i = x - 1; i >= 0; i--)
                    {
                        if (chessboardMap[i, y] == 0)
                        {
                            HighlightTile(i, y);
                        }
                        else if (chessboardMap[i, y] < 0)
                        {
                            HighlightTile(i, y);
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    for (int i = y + 1; i < 8; i++)
                    {
                        if (chessboardMap[x, i] == 0)
                        {
                            HighlightTile(x, i);
                        }
                        else if (chessboardMap[x, i] < 0)
                        {
                            HighlightTile(x, i);
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    for (int i = y - 1; i >= 0; i--)
                    {
                        if (chessboardMap[x, i] == 0)
                        {
                            HighlightTile(x, i);
                        }
                        else if (chessboardMap[x, i] < 0)
                        {
                            HighlightTile(x, i);
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }

                    //Diagonal movement
                    for (int i = 1; i < 8; i++)
                    {
                        if (x + i < 8 && y + i < 8)
                        {
                            if (chessboardMap[x + i, y + i] == 0)
                            {
                                HighlightTile(x + i, y + i);
                            }
                            else if (chessboardMap[x + i, y + i] < 0)
                            {
                                HighlightTile(x + i, y + i);
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }

                        else
                        {
                            break;
                        }
                    }

                    for (int i = 1; i < 8; i++)
                    {
                        if (x + i < 8 && y - i >= 0)
                        {
                            if (chessboardMap[x + i, y - i] == 0)
                            {
                                HighlightTile(x + i, y - i);
                            }
                            else if (chessboardMap[x + i, y - i] < 0)
                            {
                                HighlightTile(x + i, y - i);
                                break;
                            }
                            else
                            {
                                break;
                            }
                                
                        }
                        else
                        {
                            break;
                        }
                    }

                    for (int i = 1; i < 8; i++)
                    {
                        if (x - i >= 0 && y + i < 8)
                        {
                            if (chessboardMap[x - i, y + i] == 0)
                            {
                                HighlightTile(x - i, y + i);
                            }
                            else if (chessboardMap[x - i, y + i] < 0)
                            {
                                HighlightTile(x - i, y + i);
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    for (int i = 1; i < 8; i++)
                    {
                        if (x - i >= 0 && y - i >= 0)
                        {
                            if (chessboardMap[x - i, y - i] == 0)
                            {
                                HighlightTile(x - i, y - i);
                            }
                            else if (chessboardMap[x - i, y - i] < 0)
                            {
                                HighlightTile(x - i, y - i);
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    break;
                case 5:
                    //Horizontal movement
                    for (int i = x + 1; i < 8; i++)
                    {
                        if (chessboardMap[i, y] == 0)
                        {
                            HighlightTile(i, y);
                        }
                        else if (chessboardMap[i, y] < 0)
                        {
                            HighlightTile(i, y);
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    for (int i = x - 1; i >= 0; i--)
                    {
                        if (chessboardMap[i, y] == 0)
                        {
                            HighlightTile(i, y);
                        }
                        else if (chessboardMap[i, y] < 0)
                        {
                            HighlightTile(i, y);
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    for (int i = y + 1; i < 8; i++)
                    {
                        if (chessboardMap[x, i] == 0)
                        {
                            HighlightTile(x, i);
                        }
                        else if (chessboardMap[x, i] < 0)
                        {
                            HighlightTile(x, i);
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    for (int i = y - 1; i >= 0; i--)
                    {
                        if (chessboardMap[x, i] == 0)
                        {
                            HighlightTile(x, i);
                        }
                        else if (chessboardMap[x, i] < 0)
                        {
                            HighlightTile(x, i);
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                case 6:
                    //Diagonal movement
                    for (int i = 1; i < 8; i++)
                    {
                        if (x + i < 8 && y + i < 8)
                        {
                            if (chessboardMap[x + i, y + i] == 0)
                            {
                                HighlightTile(x + i, y + i);
                            }
                            else if (chessboardMap[x + i, y + i] < 0)
                            {
                                HighlightTile(x + i, y + i);
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }

                        else
                        {
                            break;
                        }
                    }

                    for (int i = 1; i < 8; i++)
                    {
                        if (x + i < 8 && y - i >= 0)
                        {
                            if (chessboardMap[x + i, y - i] == 0)
                            {
                                HighlightTile(x + i, y - i);
                            }
                            else if (chessboardMap[x + i, y - i] < 0)
                            {
                                HighlightTile(x + i, y - i);
                                break;
                            }
                            else
                            {
                                break;
                            }

                        }
                        else
                        {
                            break;
                        }
                    }

                    for (int i = 1; i < 8; i++)
                    {
                        if (x - i >= 0 && y + i < 8)
                        {
                            if (chessboardMap[x - i, y + i] == 0)
                            {
                                HighlightTile(x - i, y + i);
                            }
                            else if (chessboardMap[x - i, y + i] < 0)
                            {
                                HighlightTile(x - i, y + i);
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    for (int i = 1; i < 8; i++)
                    {
                        if (x - i >= 0 && y - i >= 0)
                        {
                            if (chessboardMap[x - i, y - i] == 0)
                            {
                                HighlightTile(x - i, y - i);
                            }
                            else if (chessboardMap[x - i, y - i] < 0)
                            {
                                HighlightTile(x - i, y - i);
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
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
            if (tblScoreHeading.Visibility == Visibility.Visible)
            {
                tblHighScoreHeading.Visibility = Visibility.Collapsed;
                tblHighScore.Visibility = Visibility.Collapsed;
                tblScore.Visibility = Visibility.Collapsed;
                tblScoreHeading.Visibility = Visibility.Collapsed;
                btnNewGame.Visibility = Visibility.Visible;
                btnExit.Visibility = Visibility.Visible;
                btnHelp.Visibility = Visibility.Visible;
            }
            else
            {
                tblHighScoreHeading.Visibility = Visibility.Visible;
                tblHighScore.Visibility = Visibility.Visible;
                tblScore.Visibility = Visibility.Visible;
                tblScoreHeading.Visibility = Visibility.Visible;
                btnNewGame.Visibility = Visibility.Collapsed;
                btnExit.Visibility = Visibility.Collapsed;
                btnHelp.Visibility = Visibility.Collapsed;
            }
            
        }

        private void BtnNewGame_Click(object sender, RoutedEventArgs e)
        {
            rectGameOver.Visibility = Visibility.Collapsed;
            tblGameOver.Visibility = Visibility.Collapsed;
            tblHelp.Visibility = Visibility.Collapsed;
            BoardSetup();
            PieceSetup();
            score = 0;
            tblScore.Text = "" + score;

            tblHighScoreHeading.Visibility = Visibility.Visible;
            tblHighScore.Visibility = Visibility.Visible;
            tblScore.Visibility = Visibility.Visible;
            tblScoreHeading.Visibility = Visibility.Visible;
            btnNewGame.Visibility = Visibility.Collapsed;
            btnExit.Visibility = Visibility.Collapsed;
            btnHelp.Visibility = Visibility.Collapsed;
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationDataCompositeValue composite = new Windows.Storage.ApplicationDataCompositeValue();
            composite["intVal"] = highScore;

            localSettings.Values["highScore"] = composite;

            Application.Current.Exit();
        }

        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            if (tblHelp.Visibility == Visibility.Visible)
            {
                rectGameOver.Visibility = Visibility.Collapsed;
                tblHelp.Visibility = Visibility.Collapsed;
            }
            else
            {
                rectGameOver.Visibility = Visibility.Visible;
                tblHelp.Visibility = Visibility.Visible;
            }
        }
    }
}
