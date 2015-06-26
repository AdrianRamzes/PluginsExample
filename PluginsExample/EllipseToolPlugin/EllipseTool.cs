using PluginInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace EllipseToolPlugin
{
    public class EllipseTool : IPlugin
    {
        private int _thickness = 3; // default value
        private Color _color = Colors.Black; //default color
        private Canvas _canvas;

        public MenuItem GetMenuItem()
        {
            MenuItem menuItem = new MenuItem();
            menuItem.Header = "Ellipse";
            return menuItem;
        }

        public void Initialize(Canvas canvas, Color color, int thickness)
        {
            _canvas = canvas;
            _color = color;
            _thickness = thickness;
            if (_canvas != null)
            {
                _canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
                _canvas.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
                _canvas.MouseMove += Canvas_MouseMove;
            }
        }

        public void SetColor(Color color)
        {
            if (color != null)
            {
                _color = color;
            }
        }

        public void SetThickness(int thickness)
        {
            if (thickness >= 0)
            {
                _thickness = thickness;
            }
        }

        public void Dispose()
        {
            if (_canvas != null)
            {
                _canvas.MouseLeftButtonDown -= Canvas_MouseLeftButtonDown;
                _canvas.MouseLeftButtonUp -= Canvas_MouseLeftButtonUp;
                _canvas.MouseMove -= Canvas_MouseMove;
            }
        }

        private Point startPoint;
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var canvas = (Canvas)sender;

            if (canvas.CaptureMouse())
            {
                startPoint = e.GetPosition(canvas);
                var ellipse = new Ellipse()
                {
                    Stroke = new SolidColorBrush(_color),
                    StrokeThickness = _thickness,
                };
                Canvas.SetLeft(ellipse, startPoint.X);
                Canvas.SetTop(ellipse, startPoint.Y);
                canvas.Children.Add(ellipse);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var canvas = (Canvas)sender;

            if (canvas.IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed)
            {
                var ellipse = canvas.Children.OfType<Ellipse>().LastOrDefault();

                if (ellipse != null)
                {
                    var endPoint = e.GetPosition(canvas);
                    var x = Math.Min(endPoint.X, startPoint.X);
                    var y = Math.Min(endPoint.Y, startPoint.Y);

                    var w = Math.Max(endPoint.X, startPoint.X) - x;
                    var h = Math.Max(endPoint.Y, startPoint.Y) - y;

                    ellipse.Width = w;
                    ellipse.Height = h;

                    Canvas.SetLeft(ellipse, x);
                    Canvas.SetTop(ellipse, y);
                }
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((Canvas)sender).ReleaseMouseCapture();
        }
    }
}
