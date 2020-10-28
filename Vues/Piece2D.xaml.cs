using App_Brycol.Modele;
using App_Brycol.Outils;
using App_Brycol.VuesModele;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public partial class Piece2D : UserControl , INotifyPropertyChanged
    {
        public Piece2D()
        {
            InitializeComponent();
            initializeItems();

            //CanvasBorder.BorderThickness = new Thickness(1);

            DataContext = new Plan_VM();
        }

        private Point clickPosition;
        private Image draggedImage;
        private Point mousePosition;
        private bool move;
        public double Zoom;
        public const double pixelToM = 3779.5275590551 / echelle;
        public const double pixelToCm = 37.7952755906 / echelle;
        public const int echelle = 50;


        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private double _zoomLevel;
        public double ZoomLevel
        {
            get { return _zoomLevel; }
            set
            {
                if(_zoomLevel != value)
                {
                    _zoomLevel = value;
                    OnPropertyChanged();
                }
            }
        }



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
                clickPosition = e.GetPosition(canvas); // get click position
                if (image != null && canvas.CaptureMouse())
                {
                    draggedImage = image;
                    draggedImage.Opacity = 1;

                    move = true;
                    mousePosition = e.GetPosition(canvas);
                    draggedImage = image;

                    Panel.SetZIndex(draggedImage, 1); // in case of multiple images

                }
            }
        }

        private void CanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(canvas);
            var offset = position - mousePosition;

            if (draggedImage != null && move == true)
            {
                canvas.ReleaseMouseCapture();
                Panel.SetZIndex(draggedImage, 0);

                //var ireq = from ip in OutilEF.brycolContexte.lstItems.Include("Plan") where ip.Plan.Piece.ID == Piece_VM.pieceActuel.ID select ip;
                foreach (ItemsPlan i in Item_VM.ItemsPlanActuel)
                {
                    var bitmap = new BitmapImage(i.Item.ImgItem.UriSource);
                    
                    var imageBD = new Image { Source = bitmap };
                    imageBD.Tag = i.ID;

                    if (imageBD.Tag.ToString() == draggedImage.Tag.ToString())
                    {
                        RotateTransform rotation = draggedImage.RenderTransform as RotateTransform;
                        if (rotation != null)
                        {
                            i.angleRotation = rotation.Angle;
                        }
                
                        i.emplacementGauche = Canvas.GetLeft(draggedImage) + offset.X;
                        i.emplacementHaut = Canvas.GetTop(draggedImage) + offset.Y;

                        OutilEF.brycolContexte.SaveChanges();
                        draggedImage = null;
                        return;
                    }
                }
               
            }
        }
        
        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (draggedImage != null && move == true)
            {
                var position = e.GetPosition(canvas);
                var offset = position - mousePosition;
                mousePosition = position;
                Canvas.SetLeft(draggedImage, Canvas.GetLeft(draggedImage) + offset.X);
                Canvas.SetTop(draggedImage, Canvas.GetTop(draggedImage) + offset.Y);
            }
            else
            {
                if (e.LeftButton != MouseButtonState.Released)
                {
                    Point mousePos = e.GetPosition(canvas_Zoom); // get absolute mouse position
                    Canvas.SetLeft(canvas, mousePos.X - clickPosition.X); // move canvas
                    Canvas.SetTop(canvas, mousePos.Y - clickPosition.Y);

                }
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

        public event PropertyChangedEventHandler PropertyChanged;


        // Zoom on Mouse wheel
        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            zoom += zoomSpeed * e.Delta; // Ajust zooming speed (e.Delta = Mouse spin value )
            if (zoom < zoomMin) { zoom = zoomMin; } // Limit Min Scale
            if (zoom > zoomMax) { zoom = zoomMax; } // Limit Max Scale

            Point mousePos = e.GetPosition(canvas);

            
                canvas_Zoom.RenderTransform = new ScaleTransform(zoom, zoom, mousePos.X, mousePos.Y); // transform Canvas size from mouse position

            if (e.Delta<0)
            {
                slider1.Value = slider1.Ticks.Select(x => (double?)x).LastOrDefault(x => x < slider1.Value) ?? slider1.Value;
            }
            else
            {
                slider1.Value = slider1.Ticks.Select(x => (double?)x).FirstOrDefault(x => x > slider1.Value) ?? slider1.Value;
            }
           
        }
        

        private void btnDelete(object sender, RoutedEventArgs e)
        {
            if (!move)
            {              
                if (Item_VM.ItemsPlanActuel != null)
                {
                    foreach (ItemsPlan ip in Item_VM.ItemsPlanActuel)
                    {
                        var bitmap = new BitmapImage(ip.Item.ImgItem.UriSource);
                        var imageBD = new Image { Source = bitmap };
                        imageBD.Tag = ip.ID;
                        if (imageBD.Source.ToString() == draggedImage.Source.ToString() && imageBD.Tag.ToString() == draggedImage.Tag.ToString())
                        {
                            
                            Item_VM.ItemsPlanActuel.Remove(ip);
                            draggedImage.Source = null;
                            OutilEF.brycolContexte.lstItems.Remove(ip);
                            OutilEF.brycolContexte.SaveChanges();
                            
                            return;
                        }
                    }
                }
                
            }
        }

        public void initializeItems()
        {
            canvas.Children.Clear();

            if (Item_VM.ItemsPlanActuel != null)
            {
                Item_VM.ItemsPlanActuel.Clear();
                var ireq = from ip in OutilEF.brycolContexte.lstItems.Include("Plan") where ip.Plan.Piece.ID == Piece_VM.pieceActuel.ID select ip;
                foreach (ItemsPlan i in ireq)
                    Item_VM.ItemsPlanActuel.Add(i);
                foreach (ItemsPlan ip in Item_VM.ItemsPlanActuel)
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri("pack://application:,,,/images/Items/Top/item" + ip.Item.ID + ".png");
                    try
                    {
                        bitmap.EndInit();
                    }
                    catch (Exception e)
                    {
                        bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri("pack://application:,,,/images/Items/Top/item0.png");
                        bitmap.EndInit();


                    }
                    var image = new Image { Source = bitmap };
                    Canvas.SetLeft(image, ip.emplacementGauche);
                    Canvas.SetTop(image, ip.emplacementHaut);

                    #region angle
                    if (ip.angleRotation == 0)
                    {
                        image.RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 0 };
                    }
                    else if (ip.angleRotation == 90)
                    {
                        image.RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 90 };
                    }
                    else if (ip.angleRotation == 180)
                    {
                        image.RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 180 };
                    }
                    else if (ip.angleRotation == 270)
                    {

                        image.RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 270 };
                    }
                    #endregion

                    image.Height = (ip.Item.Hauteur * pixelToCm);
                    image.Width = (ip.Item.Largeur * pixelToCm);
                    //image.Height = 100;
                    //image.Width = 100;
                    image.Tag = ip.ID;
                    canvas.Children.Add(image);
                    }
            }
        }

        private void canvasResizeFull(object sender, RoutedEventArgs e)
        {
            zoom = 1;
        
            canvas_Zoom.RenderTransform = new ScaleTransform(zoom, zoom); // transform Canvas size
            slider1.Value = 50;
        }

        private void btnZoomIn(object sender, RoutedEventArgs e)
        {
            zoom = zoom + 0.12;
            if (zoom < zoomMin) { zoom = zoomMin; } // Limit Min Scale
            if (zoom > zoomMax) { zoom = zoomMax; } // Limit Max Scale

            slider1.Value = slider1.Ticks.Select(x => (double?)x).FirstOrDefault(x => x > slider1.Value) ?? slider1.Value;


            canvas_Zoom.RenderTransform = new ScaleTransform(zoom, zoom); // transform Canvas size
          
        }

        private void btnZoomOut(object sender, RoutedEventArgs e)
        {
            zoom = zoom - 0.12;
            if (zoom < zoomMin) { zoom = zoomMin; } // Limit Min Scale
            if (zoom > zoomMax) { zoom = zoomMax; } // Limit Max Scale

            slider1.Value = slider1.Ticks.Select(x => (double?)x).LastOrDefault(x => x < slider1.Value) ?? slider1.Value;

            canvas_Zoom.RenderTransform = new ScaleTransform(zoom, zoom); // transform Canvas size
        }

        private void CanvasZoomMouseMove(object sender, MouseEventArgs e)
        {
           
            var canvss = e.GetPosition(canvas);
            foreach (UIElement child in canvas.Children)
            {
                Point childTopGauche = child.TransformToAncestor(canvas).Transform(new Point(0, 0));
                Point childBotDroite = new Point(childTopGauche.X + child.DesiredSize.Width , childTopGauche.Y + child.DesiredSize.Height);

                if (childTopGauche.X > canvas.Width || childTopGauche.X < 0 || childTopGauche.Y < 0 || childTopGauche.Y > canvas.Height || childBotDroite.X > canvas.Width || childBotDroite.X < 0 || childBotDroite.Y < 0 || childBotDroite.Y > canvas.Height)
                {
                    child.Opacity = 0.3;
                    
                }
                else
                {
                    child.Opacity = 1;
                }
            }
        }

    }
}
