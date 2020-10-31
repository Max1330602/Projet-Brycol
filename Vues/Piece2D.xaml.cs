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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Drawing.Color;
using Image = System.Windows.Controls.Image;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Windows.Point;

namespace App_Brycol.Vues
{
    /// <summary>
    /// Logique d'interaction pour Piece2D.xaml
    /// </summary>
    public partial class Piece2D : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        public Piece2D()
        {
            InitializeComponent();
            initializeItems();

            ImageBrush imgBrush = new ImageBrush();
            imgBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Items/plancheWood.jpg"));
            canvas.Background = imgBrush;

            Canvas.SetLeft(canvas, (600 / 2)- (Piece_VM.pieceActuel.Largeur * pixelToM /2));
            Canvas.SetBottom(canvas, (800 / 2) - (Piece_VM.pieceActuel.Longueur * pixelToM / 2));


            if (Piece_VM.pieceActuel.Longueur <= 8 || Piece_VM.pieceActuel.Largeur <= 8)
            {
                zoom = 1.4 - ((Piece_VM.pieceActuel.Longueur * 100.0 / 50.0) / 100.0);
                zoomMax = 1.9;
                zoomMin = 0.7;
            }
            else
            {
                zoom = 0.9 - ((Piece_VM.pieceActuel.Longueur * 100.0 / 50.0) / 100.0);
                zoomMax = 1.2;
                zoomMin = 0.3;
            }
            
            
          
            canvas_Zoom.RenderTransform = new ScaleTransform(zoom, zoom); // transforme la grandeur du canvas
          

            DataContext = new Plan_VM();
        }

        private Point clickPosition;
        public static Image draggedImage;
        private Point mousePosition;
        private bool move;
        public double Zoom;
        public const double pixelToM = 3779.5275590551 / echelle;
        public const double pixelToCm = 37.7952755906 / echelle;
        public const int echelle = 50;
        private Double zoomMax;
        private Double zoomMin;
        private Double zoomSpeed = 0.001;
        private Double zoom = 1;


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
            btnDeletePlan.IsEnabled = false;
            btnRotationPlan.IsEnabled = false;
            var image = e.Source as System.Windows.Controls.Image;
            if (e.ClickCount == 2 && image!= null)
            {
                btnDeletePlan.IsEnabled = true;
                btnRotationPlan.IsEnabled = true;

                draggedImage = image;
                draggedImage.Opacity = 0.5;

                move = false;
            }
           
            else {
                clickPosition = e.GetPosition(canvas); // avoir la position du click
                if (image != null && canvas.CaptureMouse())
                {
                    draggedImage = image;
                    draggedImage.Opacity = 1;

                    move = true;
                    mousePosition = e.GetPosition(canvas);
                    draggedImage = image;

                    System.Windows.Controls.Panel.SetZIndex(draggedImage, 1); // si on a plusieurs images

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
                System.Windows.Controls.Panel.SetZIndex(draggedImage, 0);

                
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
                    Point mousePos = e.GetPosition(canvas_Zoom); // position absolu de la souris
                    Canvas.SetLeft(canvas, mousePos.X - clickPosition.X); // bouger le canvas
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
        

        public event PropertyChangedEventHandler PropertyChanged;


        // Zoom avec la roue à zoom
        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            zoom += zoomSpeed * e.Delta; // Ajuste la vitesse du zoom (e.Delta = Souris roue à zoom )
            if (zoom < zoomMin) { zoom = zoomMin; } // Limite le minimum
            if (zoom > zoomMax) { zoom = zoomMax; } // Limite le maximum

            Point mousePos = e.GetPosition(canvas);

            
                canvas_Zoom.RenderTransform = new ScaleTransform(zoom, zoom, mousePos.X, mousePos.Y); // transforme la grandeur du canvas selon la position de la souris

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
                MessageBoxResult resultat;
                resultat = System.Windows.MessageBox.Show("Voulez-vraiment supprimer cette item ?", "Suppression d'un item", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (resultat == MessageBoxResult.Yes)
                {
                    if (Item_VM.ItemsPlanActuel != null)
                    {
                        foreach (ItemsPlan ip in Item_VM.ItemsPlanActuel)
                        {
                            var bitmap = new BitmapImage(ip.Item.ImgItem.UriSource);
                            var imageBD = new Image { Source = bitmap };
                            imageBD.Tag = ip.ID;
                            if (imageBD.Tag.ToString() == draggedImage.Tag.ToString())
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

                    image.Height = (ip.Item.Longueur * pixelToCm);
                    image.Width = (ip.Item.Largeur * pixelToCm);
                    image.Tag = ip.ID;
                    canvas.Children.Add(image);
                    }
            }
        }

        private void canvasResizeFull(object sender, RoutedEventArgs e)
        {
            zoom = 1;
        
            canvas_Zoom.RenderTransform = new ScaleTransform(zoom, zoom); // transforme la grandeur du canvas
            slider1.Value = 50;
        }

        private void btnZoomIn(object sender, RoutedEventArgs e)
        {
            zoom = zoom + 0.12;
            if (zoom < zoomMin) { zoom = zoomMin; } // Limite le minimum
            if (zoom > zoomMax) { zoom = zoomMax; } // Limite le maximum

            slider1.Value = slider1.Ticks.Select(x => (double?)x).FirstOrDefault(x => x > slider1.Value) ?? slider1.Value;


            canvas_Zoom.RenderTransform = new ScaleTransform(zoom, zoom); // transforme la grandeur du canvas
          
        }

        private void btnZoomOut(object sender, RoutedEventArgs e)
        {
            zoom = zoom - 0.12;
            if (zoom < zoomMin) { zoom = zoomMin; } // Limite le minimum
            if (zoom > zoomMax) { zoom = zoomMax; } // Limite le maximum

            slider1.Value = slider1.Ticks.Select(x => (double?)x).LastOrDefault(x => x < slider1.Value) ?? slider1.Value;

            canvas_Zoom.RenderTransform = new ScaleTransform(zoom, zoom);  // transforme la grandeur du canvas
        }
        public bool validation;
        private void CanvasZoomMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            
            foreach (UIElement child in canvas.Children)
            {
                Point childTopGaucheFixe = new Point(0, 0);
                Point childTopGauche = new Point(0, 0);
                Point childBotDroite = new Point(0, 0);
                double emplacementGauche = Canvas.GetLeft(child);
                double emplacementHaut = Canvas.GetTop(child);

                foreach (ItemsPlan ip in Item_VM.ItemsPlanActuel)
                {
                    if (ip.emplacementGauche == emplacementGauche && ip.emplacementHaut == emplacementHaut)
                    {
                        double rotation = ip.angleRotation;
                        if (ip.angleRotation == 0)
                        {
                            childTopGauche = child.TransformToAncestor(canvas).Transform(new Point(0, 0));
                            childBotDroite = new Point(childTopGauche.X + child.DesiredSize.Width, childTopGauche.Y + child.DesiredSize.Height);
                            
                        }
                        else if (ip.angleRotation == 90)
                        {
                            childTopGaucheFixe = child.TransformToAncestor(canvas).Transform(new Point(0, 0));
                            childTopGauche = new Point(childTopGaucheFixe.X - child.DesiredSize.Height, childTopGaucheFixe.Y);
                            childBotDroite = new Point(childTopGaucheFixe.X, childTopGaucheFixe.Y + child.DesiredSize.Width);
                        }
                        else if (ip.angleRotation == 180)
                        {
                            childBotDroite = child.TransformToAncestor(canvas).Transform(new Point(0, 0));
                            childTopGauche = new Point(childBotDroite.X - child.DesiredSize.Width, childBotDroite.Y - child.DesiredSize.Height);
                        }
                        else if (ip.angleRotation == 270)
                        {
                            childTopGaucheFixe = child.TransformToAncestor(canvas).Transform(new Point(0, 0));
                            childTopGauche = new Point(childTopGaucheFixe.X, childTopGaucheFixe.Y - child.DesiredSize.Width);
                            childBotDroite = new Point(childTopGaucheFixe.X + child.DesiredSize.Height, childTopGaucheFixe.Y);
                        }
                        
                    }                   
                }
                if (childTopGauche.X > canvas.Width || childTopGauche.X < 0 || childTopGauche.Y < 0 || childTopGauche.Y > canvas.Height || childBotDroite.X > canvas.Width || childBotDroite.X < 0 || childBotDroite.Y < 0 || childBotDroite.Y > canvas.Height)
                {
                    if ((childTopGauche.X > canvas.Width && childBotDroite.X > canvas.Width) || (childTopGauche.Y > canvas.Height && childBotDroite.Y > canvas.Height) || (childTopGauche.X < 0 && childBotDroite.X < 0) || (childTopGauche.Y < 0 && childBotDroite.Y < 0))
                    {
                        Canvas.SetLeft(child, 0);
                        Canvas.SetTop(child, 0);
                    }
                    else
                    {
                        ImageBrush imgBrush = new ImageBrush();
                        imgBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Items/invalideItem.png"));
                        child.OpacityMask = imgBrush;
                        Plan_VM.validePourEnregistrer = false;

                    }
                }
                else
                {
                    child.OpacityMask = null;
                    child.Opacity = 1;
                    Plan_VM.validePourEnregistrer = true;

                }
            }

            

        }

    }
}
