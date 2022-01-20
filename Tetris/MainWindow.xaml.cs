using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ImageSource[] TileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileCyan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileOrange.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileRed.png", UriKind.Relative))
        };

        private readonly ImageSource[] BlockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-I.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-J.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-L.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-O.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-S.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-T.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-Z.png", UriKind.Relative))
        };

        private readonly Image[,] imageControls;

        private GameState gameState = new();

        public MainWindow()
        {
            InitializeComponent();
            imageControls = SetupGameCanvas(gameState.GameGrid);
        }

        private Image[,] SetupGameCanvas(GameGrid grid)
        {
            Image[,] imageControls = new Image[grid.Rows, grid.Columns];
            int cellSize = 25;

            for (var r = 0; r < grid.Rows; r++)
            {
                for (var c = 0; c < grid.Columns; c++)
                {
                    Image imageControl = new()
                    {
                        Width = cellSize,
                        Height = cellSize,
                    };

                    Canvas.SetTop(imageControl, (r - 2) * cellSize + 10);
                    Canvas.SetLeft(imageControl, c * cellSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }

            return imageControls;
        }

        private void DrawNextBlock(BlockQueue blockQueue)
        {
            var next = blockQueue.NextBlock;
            NextImage.Source = BlockImages[next.Id];
        }

        private void DrawGrid(GameGrid grid)
        {
            for (var r = 0; r < grid.Rows; r++)
            {
                for (var c = 0; c < grid.Columns; c++)
                {
                    var id = grid[r, c];
                    imageControls[r, c].Opacity = 1;
                    imageControls[r, c].Source = TileImages[id];
                }
            }
        }

        private void DrawHeldBlock(Block heldBlock) => HoldImage.Source = (heldBlock is null) ? BlockImages[0] : BlockImages[heldBlock.Id];

        private void DrawGhostBlock(Block block)
        {
            int drop = gameState.BlockDropDistance();
            int tmp;

            foreach(Position p in block.TilePositions())
            {
                tmp = p.Row + drop;
                imageControls[tmp, p.Column].Opacity = 0.25;
                imageControls[tmp, p.Column].Source = TileImages[block.Id];
            }
        }
        

        private void DrawBlock(Block block)
        {
            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row, p.Column].Opacity = 1;
                imageControls[p.Row, p.Column].Source = TileImages[block.Id];
            }
        }

        private void Draw(GameState gameState)
        {
            DrawGrid(gameState.GameGrid);
            DrawGhostBlock(gameState.CurrentBlock);
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState.BlockQueue);
            DrawHeldBlock(gameState.HeldBlock);
            ScoreText.Text = $"Score: {gameState.Score}";
        }

        private async Task GameLoop()
        {
            Draw(gameState);

            while (!gameState.GameOver)
            {
                await Task.Delay((int)gameState.difficulty);
                gameState.MoveBlockDown();
                Draw(gameState);
            }

            FinalScoreText.Text = $"Score: {gameState.Score}";
            GameOverMenu.Visibility = Visibility.Visible;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!gameState.GameOver)
            {
                switch (e.Key)
                {
                    case Key.Left:
                        gameState.MoveBlockLeft();
                        break;
                    case Key.Right:
                        gameState.MoveBlockRight();
                        break;
                    case Key.Down:
                        gameState.MoveBlockDown();
                        break;
                    case Key.Up:
                        gameState.RotateBlockCW();
                        break;
                    case Key.Z:
                        gameState.RotateBlockCCW();
                        break;
                    case Key.C:
                        gameState.HoldBlock();
                        break;
                    case Key.Space:
                        gameState.DropBlock();
                        break;
                    default:
                        return;
                }
                Draw(gameState);
            }
        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }

        private async void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            gameState = new();
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();
        }
    }
}
