using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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
        bool isPathHighlighted = false;
        int[] pieceCoordinate = new int[2];

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

            temp.Stroke = new SolidColorBrush(Colors.Yellow);
            temp.StrokeThickness = 0;
            ImageBrush img = new ImageBrush();
            switch (piece)
            {
                case 0:
                    temp.Fill = new SolidColorBrush(Colors.Transparent);
                    break;
                case 1:
                    img.ImageSource = new BitmapImage(new Uri("ms-appx:///Image/pawnWhite.png", UriKind.RelativeOrAbsolute));
                    //temp.Fill = new SolidColorBrush(Colors.Yellow);
                    temp.Fill = img;
                    break;
                case 2:
                    img.ImageSource = new BitmapImage(new Uri("ms-appx:///Image/knightWhite.png", UriKind.RelativeOrAbsolute));
                    //temp.Fill = new SolidColorBrush(Colors.Blue);
                    temp.Fill = img;
                    break;
                case 3:
                    img.ImageSource = new BitmapImage(new Uri("ms-appx:///Image/kingWhite.png", UriKind.RelativeOrAbsolute));
                    //temp.Fill = new SolidColorBrush(Colors.Red);
                    temp.Fill = img;
                    break;
            }

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
            
            if(isPathHighlighted == false)
            {
                ShowValidSquares(row, column);
                isPathHighlighted = true;
                pieceCoordinate[0] = row;
                pieceCoordinate[1] = column;
            }
        }

        private void ShowValidSquares(int x, int y)
        {

        }

        private void BtnMenu_Click(object sender, RoutedEventArgs e) //Menu button click
        {
            //tblGameTitle.Text = "Test";
        }

    }
}
