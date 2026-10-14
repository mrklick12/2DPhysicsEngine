
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;


namespace _2DPhysicsEngine
{    
    public partial class MainWindow : Window
    {
        private TimeSpan _previousTime;
        private readonly double g = 300.0; // gravity constant

        private readonly double frictionRate = 2.5; // friction rate for sliding on slopes

        private readonly double restitutionConst = 0.5; // coefficient of restitution for collisions

        private GroundSegment ground1;
        private GroundSegment ground2;
        private PhysicsObject player;

        private PhysicsObject rectangle;

        public List<PhysicsObject> gravObjects;

        public List<GroundSegment> groundObjects;

        private double width = SystemParameters.WorkArea.Width;
        private double height = SystemParameters.WorkArea.Height;

        private bool _leftPressed;
        private bool _rightPressed;

        private Ellipse visualfor1 = new Ellipse // new ball
        {
            Width = 10,
            Height = 10,
            Fill = Brushes.Yellow
        };

        private Ellipse visualfor0 = new Ellipse // new ball
        {
            Width = 100,
            Height = 100,
            Fill = Brushes.OrangeRed
        };



        double Lerp(double a, double b, double t) => a + (b - a) * t;
        public MainWindow()
        {
            InitializeComponent();
            Focusable = true;
            Focus();

            player = new PhysicsObject(Ball1, Ball1.Width, Ball1.Height)
            {
                PosX = 50,
                PosY = 50,
                VelX = 0,
                VelY = 0
            };

            ground1 = new GroundSegment(Ground1, Ground1.ActualWidth, Ground1.ActualHeight);
            ground2 = new GroundSegment(Ground2, Ground2.ActualWidth, Ground2.ActualHeight);

            gravObjects = new List<PhysicsObject> 
            {
                player  // all objects that need to be affected by gravity (and collisions)
            };

            groundObjects = new List<GroundSegment>
            {
                ground1,
                ground2,
                new GroundSegment(Step1, Step1.ActualHeight, Step1.ActualHeight),
                new GroundSegment(Step2, Step2.ActualHeight, Step2.ActualHeight),
                new GroundSegment(Step3, Step3.ActualHeight, Step3.ActualHeight)
            };

            CompositionTarget.Rendering += OnRendering;

            

        }

        private void SimCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(MainCanvas);

            Ellipse visual = new Ellipse // new ball
            {
                Width = 30,
                Height = 30,
                Fill = Brushes.DarkRed
            };
            MainCanvas.Children.Add(visual);

            if (e.ChangedButton == MouseButton.Right)
            {
                gravObjects.Add(new PhysicsObject(visual, visual.Width, visual.Height)
                {
                    PosX = p.X,
                    PosY = p.Y,
                    VelX = 0,
                    VelY = -15
                });
            }
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

            foreach (var obj in gravObjects)
                obj.isGrounded = false;

            double deltaTime = (currentTime - _previousTime).TotalSeconds;
            _previousTime = currentTime;

            double horizontalSpeed = 150;

            if (_leftPressed)
            {
                player.VelX = -horizontalSpeed;
            }
            else if (_rightPressed)
            {
                player.VelX = horizontalSpeed;
            }

            for (int i = 0; i < gravObjects.Count; i++)  // apply gravity and collision to objects
            {
                gravObjects[i].UpdatePositionY(deltaTime, g, 1);
                gravObjects[i].UpdatePosition(deltaTime);


                double groundLevelB = MainCanvas.ActualHeight - gravObjects[i].height;
                if (gravObjects[i].CheckIfGround(groundLevelB))
                {
                    gravObjects[i].VelX = Lerp(gravObjects[i].VelX, 0, 0.1);
                }
                gravObjects[i].checkIfWall(MainCanvas.ActualWidth - gravObjects[i].width);  



                for (int k = 0; k < groundObjects.Count; k++)
                {
                    Point closest;
                    double dist = gravObjects[i].CheckCollisionDistanceToSegment(gravObjects[i], groundObjects[k], out closest) - groundObjects[k].Thickness / 2.0;
                    double radius = gravObjects[i].width / 2.0;
                    double cx = gravObjects[i].PosX + gravObjects[i].width / 2.0;
                    double cy = gravObjects[i].PosY + gravObjects[i].height / 2.0;

                    if (dist < radius) // in contact with slope
                    {
                        Vector normal = new Vector(cx - closest.X, cy - closest.Y);
                        normal.Normalize(); // becomes unit vector
                        if (normal.Y < -0.5)
                        {
                            gravObjects[i].isGrounded = true;
                        }    
                        double penetrationDepth = radius - dist;
                        gravObjects[i].PosX += normal.X * penetrationDepth;
                        gravObjects[i].PosY += normal.Y * penetrationDepth;
                        double velocityAlongNormal = gravObjects[i].VelX * normal.X + gravObjects[i].VelY * normal.Y;
                        if (velocityAlongNormal < 0)
                        {
                            // double  = 0.5; // e
                            double restitution = (-velocityAlongNormal < 20) ? 0 : restitutionConst;
                            double impulse = -(1 + restitution) * velocityAlongNormal;
                            gravObjects[i].VelX += impulse * normal.X;
                            gravObjects[i].VelY += impulse * normal.Y;
                        }

                        if (normal.Y < -0.99)
                        {
                            Vector v = new Vector(gravObjects[i].VelX, gravObjects[i].VelY);
                            Vector vNormal = normal * Vector.Multiply(v, normal);
                            Vector vTangent = v - vNormal;
                            vTangent *= Math.Exp(-frictionRate * deltaTime);
                            v = vNormal + vTangent;
                            gravObjects[i].VelX = v.X;
                            gravObjects[i].VelY = v.Y;
                        }
                        

                    }
                }

                // for collision with other objects
                for (int j = 0; j < gravObjects.Count; j++)
                {
                    if (i == j) continue;
                    // gravObjects[i] and gravObjects[j] are the two objects to check for collision
                    double sumOfRadii = (gravObjects[i].width + gravObjects[j].width) / 2.0;
                    double distance = CheckCollisionDistanceToObject(gravObjects[i], gravObjects[j]);
                    if (distance < sumOfRadii)
                    {
                        double cx1 = gravObjects[i].PosX + gravObjects[i].width / 2.0;
                        double cy1 = gravObjects[i].PosY + gravObjects[i].height / 2.0;

                        double cx2 = gravObjects[j].PosX + gravObjects[j].width / 2.0;
                        double cy2 = gravObjects[j].PosY + gravObjects[j].height / 2.0;

                        Vector normal = new Vector(cx1 - cx2, cy1 - cy2);
                        normal.Normalize();

                        if (normal.Y < -0.5)
                        {
                            gravObjects[i].isGrounded = true;
                        }

                        if (normal.Y > 0.5)
                        {
                            gravObjects[j].isGrounded = true;
                        }

                        double totalMass = gravObjects[i].mass + gravObjects[j].mass;

                        gravObjects[i].PosX += normal.X * (sumOfRadii - distance) * (gravObjects[j].mass / totalMass);
                        gravObjects[i].PosY += normal.Y * (sumOfRadii - distance) * (gravObjects[j].mass / totalMass);

                        gravObjects[j].PosX -= normal.X * (sumOfRadii - distance) * (gravObjects[i].mass / totalMass);
                        gravObjects[j].PosY -= normal.Y * (sumOfRadii - distance) * (gravObjects[i].mass / totalMass);

                        double vNormal = (gravObjects[i].VelX - gravObjects[j].VelX) * normal.X + (gravObjects[i].VelY - gravObjects[j].VelY) * normal.Y;
                        if (vNormal > 0) continue; 
                        double J = -(1 + restitutionConst) * vNormal / (1 / gravObjects[i].mass + 1 / gravObjects[j].mass);

                        gravObjects[i].VelX += (J / gravObjects[i].mass) * normal.X;
                        gravObjects[i].VelY += (J / gravObjects[i].mass) * normal.Y;

                        gravObjects[j].VelX -= (J / gravObjects[j].mass) * normal.X;
                        gravObjects[j].VelY -= (J / gravObjects[j].mass) * normal.Y;


                    }


                }
            }
        }
        public double CheckCollisionDistanceToObject(PhysicsObject obj1, PhysicsObject obj2)
        {
            double cx1 = obj1.PosX + obj1.width / 2.0;
            double cy1 = obj1.PosY + obj1.height / 2.0;
            double cx2 = obj2.PosX + obj2.width / 2.0;
            double cy2 = obj2.PosY + obj2.height / 2.0;
            double dx = cx1 - cx2;
            double dy = cy1 - cy2;
            return Math.Sqrt(dx * dx + dy * dy);
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
                if (e.Key == Key.Space && player.isGrounded)
                {
                    player.VelY = -200;
                }
            }


            
            

            if (e.Key == Key.D0)
            {
                Point p = Mouse.GetPosition(MainCanvas);
                var ball = new Ellipse { Width = visualfor0.Width, Height = visualfor0.Height, Fill = visualfor0.Fill };
                MainCanvas.Children.Add(ball);
                gravObjects.Add(new PhysicsObject(ball, ball.Width, ball.Height)
                {
                    PosX = p.X,
                    PosY = p.Y,
                    VelX = 0,
                    VelY = -15
                });
            }

            if (e.Key == Key.D1)
            {
                Point p = Mouse.GetPosition(MainCanvas);
                var ball = new Ellipse { Width = visualfor1.Width, Height = visualfor1.Height, Fill = visualfor1.Fill };
                MainCanvas.Children.Add(ball);
                gravObjects.Add(new PhysicsObject(ball, ball.Width, ball.Height)
                {
                    PosX = p.X,
                    PosY = p.Y,
                    VelX = 0,
                    VelY = -15
                });
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

    public class PhysicsObject
    {
        public FrameworkElement Visual { get; }

        public double PosX { get; set; }
        public double PosY { get; set; }
        public double VelX { get; set; }
        public double VelY { get; set; }

        public double width { get; }
        public double height { get; }

        public double[] CoM { get; set; }

        public double mass { get; set; }

        public double t { get; set; }

        public double minDis { get; set; }

        public Boolean isGrounded { get; set; }

        public PhysicsObject(FrameworkElement visual, double width, double height)
        {
            Visual = visual;
            this.width = width;
            this.height = height;
            CoM = new double[] { width / 2, height / 2};
            mass = width*height; // simple mass calculation based on area
            isGrounded = false;
        }

        public void UpdateVisual()
        {
            Canvas.SetLeft(Visual, PosX);
            Canvas.SetTop(Visual, PosY);
        }

        public void UpdatePosition(double deltaTime)
        {
            PosX += VelX * deltaTime;
            PosY += VelY * deltaTime;
            UpdateVisual();
        }

        public void UpdatePositionY(double deltaTime, double acceleration, int direction)
        {
            VelY += acceleration * deltaTime * direction;
            PosY += VelY * deltaTime;
            UpdateVisual();
        }

        public void UpdatePositionX(double deltaTime, double acceleration, int direction)
        {
            VelX += acceleration * deltaTime * direction;
            PosX += VelX * deltaTime;
            UpdateVisual();
        }

        public Boolean CheckIfGround(double groundLevel)
        {
            if (PosY >= groundLevel)
            {
                PosY = groundLevel;
                VelY = 0;
                isGrounded = true;

                return true;
            }
            return false;
        }

        public Boolean checkIfWall(double wallX)
        {
            if (PosX > wallX)
            {
                PosX = wallX;
                VelX = 0;
                return true;
            }
            else if (PosX < 0)
            {
                PosX = 0;
                VelX = 0;
                return true;
            }
            return false;
        }

        public double CheckCollisionDistanceToSegment(PhysicsObject obj, GroundSegment ground, out Point closest)
        {
            // object center
            double cx = obj.PosX + obj.width / 2.0;
            double cy = obj.PosY + obj.height / 2.0;

            Vector m = ground.m; // B - A
            Vector ap = new Vector(cx - ground.A.X, cy - ground.A.Y);

            double mLen2 = m.X*m.X + m.Y*m.Y;
            if (mLen2 <= 0.0) // degenerate segment
            {
                closest = ground.A;
                return Math.Sqrt((cx - closest.X)*(cx - closest.X) + (cy - closest.Y)*(cy - closest.Y));
            }

            double t = Vector.Multiply(ap, m) / mLen2;
            // clamp to segment
            t = Math.Max(0.0, Math.Min(1.0, t));

            closest = new Point(ground.A.X + t * m.X, ground.A.Y + t * m.Y);

            double dx = cx - closest.X;
            double dy = cy - closest.Y;
            return Math.Sqrt(dx*dx + dy*dy);
        }



    }

    public class GroundSegment
    {
        // public FrameworkElement Visual { get; }

        private double width;

        private double height;

        public PathGeometry pg;

        public Point A { get; set; } // represents the start point of the ground segment

        public Point B { get; set; } // represents the end point of the ground segment

        public Vector m { get { return B - A; } } // represents the vector from point A to point B

        public double Thickness { get; set; }




        public GroundSegment(Path path, double width, double height)
        {
            this.width = width;
            this.height = height;

            this.pg = PathGeometry.CreateFromGeometry(path.Data);
            this.A = pg.Figures[0].StartPoint;
            this.B = ((LineSegment)pg.Figures[0].Segments[0]).Point;
            this.Thickness = path.StrokeThickness;
        }
    }
    



}