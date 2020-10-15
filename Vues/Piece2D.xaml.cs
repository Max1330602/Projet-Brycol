using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;

namespace App_Brycol.Vues
{
    /// <summary>
    /// Logique d'interaction pour Piece2D.xaml
    /// </summary>
    public partial class Piece2D : UserControl 
    {
        public Piece2D()
        {
            InitializeComponent();

        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            //dialog.Filter =
              //  "Image Files (*.jpg; *.png; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";

            if ((bool)dialog.ShowDialog())
            {
                var bitmap = new BitmapImage(new Uri(dialog.FileName));
                var image = new Image { Source = bitmap };
                Canvas.SetLeft(image, 0);
                Canvas.SetTop(image, 0);
                canvas.Children.Add(image);
            }
        }

        private Image draggedImage;
        private Point mousePosition;
        private bool move;
        public double Zoom;

        private void CanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var image = e.Source as System.Windows.Controls.Image;
            if (e.ClickCount == 2)
            {
                PlanDeTravail xx = new PlanDeTravail();
                xx.btnSupprimerItem.IsEnabled = true;

                draggedImage = image;
                draggedImage.Opacity = 0.5;

                move = false;
            }
           
            else {

                if (image != null && canvas.CaptureMouse())
                {
                    move = true;
                    mousePosition = e.GetPosition(canvas);
                    draggedImage = image;

                    Panel.SetZIndex(draggedImage, 1); // in case of multiple images

                }
            }
        }

        private void CanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (draggedImage != null && move == true)
            {
                canvas.ReleaseMouseCapture();
                Panel.SetZIndex(draggedImage, 0);
                
                draggedImage = null;
               
            }
        }
        
        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
               
            }
            if (draggedImage != null && move == true)
            {
                var position = e.GetPosition(canvas);
                var offset = position - mousePosition;
                mousePosition = position;
                Canvas.SetLeft(draggedImage, Canvas.GetLeft(draggedImage) + offset.X);
                Canvas.SetTop(draggedImage, Canvas.GetTop(draggedImage) + offset.Y);
            }
        }

        private void btnRotation(object sender, RoutedEventArgs e)
        {
            
            if (!move)
            {

                RotateTransform rotation = draggedImage.RenderTransform as RotateTransform;
                double rotationInDegrees = 0;
                if (rotation !=null)
                {
                    rotationInDegrees = rotation.Angle;
                }
                else
                {
                    rotationInDegrees = 0;
                }
                    // Do something with the rotationInDegrees here, if needed...
                    if (rotationInDegrees == 0 || rotation == null)
                    {
                        draggedImage.RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 90 };
                    }
                    else if(rotationInDegrees == 90)
                    {
                        draggedImage.RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 180 };
                    }
                    else if (rotationInDegrees == 180)
                    {
                        draggedImage.RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 270 };
                    }
                    else if (rotationInDegrees == 270)
                    {
                        
                        draggedImage.RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 0 };
                    }
                
                
            }
            

        }
        // Zoom
        private Double zoomMax = 1.4;
        private Double zoomMin = 0.3;
        private Double zoomSpeed = 0.001;
        private Double zoom = 1;
        private void btnZoomIn(object sender, RoutedEventArgs e)
        {
            if (zoom < zoomMin) { zoom = zoomMin; } // Limit Min Scale
            if (zoom > zoomMax) { zoom = zoomMax; } // Limit Max Scale
            canvas.RenderTransform = new ScaleTransform(zoom, zoom); // transform Canvas size
            
        }

       
        // Zoom on Mouse wheel
        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            zoom += zoomSpeed * e.Delta; // Ajust zooming speed (e.Delta = Mouse spin value )
            if (zoom < zoomMin) { zoom = zoomMin; } // Limit Min Scale
            if (zoom > zoomMax) { zoom = zoomMax; } // Limit Max Scale

            Point mousePos = e.GetPosition(canvas);

            if (zoom > 1)
            {
                canvas.RenderTransform = new ScaleTransform(zoom, zoom, mousePos.X, mousePos.Y); // transform Canvas size from mouse position
               
            }
            else
            {
                canvas.RenderTransform = new ScaleTransform(zoom, zoom); // transform Canvas size
                canvas.Height = 450;
                canvas.Width = 800;
            }
        }
        
        private void btnZoomOut(object sender, RoutedEventArgs e)
        {
            double ZoomFactor = -1.1;
            canvas.Width *= ZoomFactor;
            canvas.Height *= ZoomFactor;
            Zoom *= ZoomFactor;
            TransformGroup transform = new TransformGroup();
            transform.Children.Add(new ScaleTransform(Zoom, Zoom));
            transform.Children.Add(new TranslateTransform(0, 0));
            canvas.RenderTransform = transform;
        }

        private void btnDelete(object sender, RoutedEventArgs e)
        {
            if (!move)
            {

                draggedImage.Source = null;
            }
        }
       
     
    }
}
