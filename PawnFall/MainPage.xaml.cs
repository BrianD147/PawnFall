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
        Rectangle[,] chessSquares = new Rectangle[8, 8]; //a 2 dimensional array of all chessboard squares

        public MainPage()
        {
            this.InitializeComponent();
            setup();
        }

        private void setup() //sets up the chessboard
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
                chessboardMap[1, i] = 1;
            }

            //set knights
            chessboardMap[0, 1] = 2;
            chessboardMap[0, 5] = 2;

            //set king
            chessboardMap[0, 3] = 3;

            //load pieces into correct positions
            for (int i=0; i<7; i++)
            {
                for(int j=0; j<7; j++)
                {
                    loadPieces(chessboardMap[i, j], i, j);
                }

            }

        }

        private void loadPieces(int piece, int x, int y)//loads a chess piece into correct position
        {
            Rectangle temp = new Rectangle();
            temp.Height = vbChessboard.Height / 7;
            temp.Width = vbChessboard.Width / 7;

            temp.Stroke = new SolidColorBrush(Colors.LawnGreen);
            temp.StrokeThickness = 0;
            ImageBrush img = new ImageBrush();
            switch (piece)
            {
                case 0:
                    break;
                case 1:
                    img.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/pawnWhite.png"));
                    break;
                case 2:
                    img.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/knightWhite.png"));
                    break;
                case 3:
                    img.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/kingWhite.png"));
                    break;
            }
            temp.Fill = img;

            Grid.SetRow(temp, x);
            Grid.SetColumn(temp, y);
            if (chessSquares[x, y] != null)
                gChessboard.Children.Remove(chessSquares[x, y]);
            chessSquares[x, y] = temp;
            gChessboard.Children.Add(chessSquares[x, y]);
        }

        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            tblGameTitle.Text = "Test";
        }

    }
}
