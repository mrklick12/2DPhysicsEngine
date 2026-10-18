using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;


namespace _2DPhysicsEngine
{    
    public partial class MainWindow : Window
    {
        private TimeSpan _previousTime;
        private bool _leftPressed;
        private bool _rightPressed;

        private World world;

        private Body player;

        private Dictionary<Body, Ellipse> visuals = new Dictionary<Body, Ellipse>();

        private Ellipse visualfor1 = new Ellipse // new ball
        {
            Width = 15,
            Height = 15,
            Fill = Brushes.Yellow
        };

        private Ellipse visualfor0 = new Ellipse // new ball
        {
            Width = 100,
            Height = 100,
            Fill = Brushes.OrangeRed
        };

        public MainWindow()
        {
            InitializeComponent();

            world = new World(MainCanvas.ActualWidth, MainCanvas.ActualHeight);

            double r = Ball1.Width / 2;
            Vector2 centre = new Vector2(Canvas.GetLeft(Ball1) + r, Canvas.GetTop(Ball1) + r);
            player = new Body(centre, r, new Vector2(0, 0));
            world.Bodies.Add(player);
            visuals[player] = Ball1;

            world.Segments.Add(SegmentFromPath(Ground1));
            world.Segments.Add(SegmentFromPath(Ground2));

            world.Segments.Add(SegmentFromPath(Step1));
            world.Segments.Add(SegmentFromPath(Step2));
            world.Segments.Add(SegmentFromPath(Step3));


            Focusable = true;
            Focus();
           

            CompositionTarget.Rendering += OnRendering;

        }

        private Segment SegmentFromPath(System.Windows.Shapes.Path path)
        {
            PathGeometry pg = PathGeometry.CreateFromGeometry(path.Data);
            Point a = pg.Figures[0].StartPoint;
            Point b = ((LineSegment)pg.Figures[0].Segments[0]).Point;
            return new Segment(new Vector2(a.X, a.Y), new Vector2(b.X, b.Y), path.StrokeThickness);
        }
        private void OnRendering(object sender, EventArgs e)
        {
            var args = (RenderingEventArgs)e;
            TimeSpan currentTime = args.RenderingTime;

            if (_previousTime == TimeSpan.Zero)
            {
                _previousTime = currentTime;
                return;
            }

            double deltaTime = (currentTime - _previousTime).TotalSeconds;
            deltaTime = Math.Min(deltaTime, 0.05);
            _previousTime = currentTime;

            double horizontalSpeed = 150;

            if (_leftPressed)
            {
                player.velocity = new Vector2(-horizontalSpeed, player.velocity.y);
            }
            else if (_rightPressed)
            {
                player.velocity = new Vector2(horizontalSpeed, player.velocity.y);
            }


            world.Width = MainCanvas.ActualWidth;
            world.Height = MainCanvas.ActualHeight;
            world.Update(deltaTime);

            foreach (var v in visuals)
            {
                Canvas.SetLeft(v.Value, v.Key.position.x - v.Key.radius);
                Canvas.SetTop(v.Value, v.Key.position.y - v.Key.radius);
            }


        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                _leftPressed = true;
            }

            if (e.Key == Key.Right)
            {
                _rightPressed = true;
            }

            if (e.Key == Key.Space)
            {
                if (e.Key == Key.Space)
                { 
                    player.velocity = new Vector2(player.velocity.x, -300);
                }
            }
            if (e.Key == Key.D0 && !e.IsRepeat)
            {
                Point p = Mouse.GetPosition(MainCanvas);
                var ball = new Ellipse { Width = visualfor0.Width, Height = visualfor0.Height, Fill = visualfor0.Fill };
                MainCanvas.Children.Add(ball);

                Body body = new Body(new Vector2(p.X, p.Y), ball.Width / 2.0, new Vector2(0, -15));
                world.Bodies.Add(body);
                visuals[body] = ball;
            }

            if (e.Key == Key.D1 && !e.IsRepeat)
            {
                Point p = Mouse.GetPosition(MainCanvas);
                var ball = new Ellipse { Width = visualfor1.Width, Height = visualfor1.Height, Fill = visualfor1.Fill };
                MainCanvas.Children.Add(ball);
                Body body = new Body(new Vector2(p.X, p.Y), ball.Width / 2.0, new Vector2(0, -15));
                world.Bodies.Add(body);
                visuals[body] = ball;
            }

        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                _leftPressed = false;
            }

            if (e.Key == Key.Right)
            {
                _rightPressed = false;
            }
        }
    }
}