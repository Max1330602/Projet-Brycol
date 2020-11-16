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
using Application = System.Windows.Application;
using Brush = System.Windows.Media.Brush;
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

            if (Projet_VM.themeSombre)
                AppliquerThemeSombre();
            else
                EnleverThemeSombre();
            try
            {
                ImageBrush imgBrush = new ImageBrush();
                imgBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Items/planche" + Piece_VM.pieceActuel.TypePlancher.Nom + ".jpg"));
                canvas.Background = imgBrush;
            }
            catch (Exception)
            {
                ImageBrush imgBrush = new ImageBrush();
                imgBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Items/plancheAucun.jpg"));
                canvas.Background = imgBrush;
            }
            

            if (Plan_VM.uniteDeMesure == "Mètres")
            {
                Canvas.SetLeft(canvas, 0);
                Canvas.SetTop(canvas, 0);
                
               
            /*
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
                }*/

            }
            else
            {
                Canvas.SetLeft(canvas, 0);
                Canvas.SetBottom(canvas, 0);

                if (Piece_VM.pieceActuel.Longueur <= 26 || Piece_VM.pieceActuel.Largeur <= 26)
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


            }

            canvas_Zoom.RenderTransform = new ScaleTransform(zoom, zoom); // transforme la grandeur du canvas


            DataContext = new Plan_VM();
        }

        #region attributs
        private Point clickPosition;
        private Point position;
        public static Image draggedImage;
        public static Image toolbarImage;
        private Point mousePosition;
        private bool move;
        public double Zoom;
        public const double pixelToM = 3779.5275590551 / echelle;
        public const double pixelToPied = 1151.9999999832 / echelle;
        public const double pixelToCm = 37.7952755906 / echelle;
        public const int echelle = 50;
        // Zoom
        private Double zoomMax;
        private Double zoomMin;
        private Double zoomSpeed = 0.001;
        private Double zoom = 1;
        #endregion

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
            if (e.ClickCount == 2 && image!= null)
            {
                

                draggedImage = image;
                draggedImage.Opacity = 0.5;

                move = false;
            }

            else
            {
                clickPosition = e.GetPosition(canvas); // avoir la position du click
                if (image != null && canvas.CaptureMouse())
                {
                    canvas.Children.Remove(btntoolRotation);
                    canvas.Children.Remove(btntoolSupprimer);
                    draggedImage = image;
                    draggedImage.Opacity = 1;

                    foreach (ItemsPlan i in Item_VM.ItemsPlanActuel)
                    {
                        var bitmap = new BitmapImage(i.Item.ImgItem.UriSource);

                        var imageBD = new Image { Source = bitmap };
                        imageBD.Tag = i.ID;

                        if (imageBD.Tag.ToString() == draggedImage.Tag.ToString())
                        {
                            Canvas.SetLeft(btntoolRotation, i.emplacementGauche + 5);
                            Canvas.SetTop(btntoolRotation, i.emplacementHaut - 19);
                            canvas.Children.Add(btntoolRotation);
                            Canvas.SetLeft(btntoolSupprimer, i.emplacementGauche + 64);
                            Canvas.SetTop(btntoolSupprimer, i.emplacementHaut - 19);
                            canvas.Children.Add(btntoolSupprimer);
                        }
                    }


                    btntoolRotation.Visibility = Visibility.Visible;
                    btntoolSupprimer.Visibility = Visibility.Visible;
                    move = true;
                    mousePosition = e.GetPosition(canvas);
                    draggedImage = image;

                    System.Windows.Controls.Panel.SetZIndex(draggedImage, 1); // si on a plusieurs images

                }
            }
        }

        private void CanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            position = e.GetPosition(canvas);
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

                        if (i.Item.Nom == "Porte")
                        {
                            if (i.angleRotation == 0)
                                i.emplacementHaut = -25;
                            else if (i.angleRotation == 90)
                                i.emplacementGauche = canvas.Width - 2;
                            else if (i.angleRotation == 180)
                                i.emplacementHaut = canvas.Height-30;
                            else if (i.angleRotation == 270)
                                i.emplacementGauche = 0;
                        }

                        OutilEF.brycolContexte.SaveChanges();
                        if (mousePosition == position)
                        {
                            toolbarImage = draggedImage;
                            draggedImage = null;
                        }
                        else
                        {
                            draggedImage = null;
                        }
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
           
            RotateTransform rotation = toolbarImage.RenderTransform as RotateTransform;
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
                        toolbarImage.RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 90 };
                    }
                    else if(rotationInDegrees == 90)
                    {
                        toolbarImage.RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 180 };
                    }
                    else if (rotationInDegrees == 180)
                    {
                        toolbarImage.RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 270 };
                    }
                    else if (rotationInDegrees == 270)
                    {

                        toolbarImage.RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 0 };
                    }
                             
            
            

        }
       
        

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
           
            if (draggedImage != null)
                    draggedImage.Source = null;
                MessageBoxResult resultat;
                resultat = System.Windows.MessageBox.Show("Voulez-vous vraiment supprimer cet item ?", "Suppression d'un item", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (resultat == MessageBoxResult.Yes)
                {
                    if (Item_VM.ItemsPlanActuel != null)
                    {
                        foreach (ItemsPlan ip in Item_VM.ItemsPlanActuel)
                        {
                            var bitmap = new BitmapImage(ip.Item.ImgItem.UriSource);
                            var imageBD = new Image { Source = bitmap };
                            imageBD.Tag = ip.ID;
                            if (imageBD.Tag.ToString() == toolbarImage.Tag.ToString())
                            {
                                Item_VM.ItemsPlanActuel.Remove(ip);
                                toolbarImage.Source = null;
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

                        

                    try
                    {
                        bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                        bitmap.UriSource = new Uri("pack://application:,,,/images/ItemsModifies/Item" + ip.Item.ID + "/" + ip.Couleur + ".png");
                        bitmap.EndInit();
                    }
                    catch (Exception e)
                    {
                        try
                        {

                            bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri("pack://application:,,,/images/Items/Top/item" + ip.Item.ID + ".png");
                            bitmap.EndInit();
                        }
                        catch(Exception e2)
                        {
                            bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri("pack://application:,,,/images/Items/Top/item0.png");
                            bitmap.EndInit();
                        }
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
                        Canvas.SetLeft(child, canvas.Width /2);
                        Canvas.SetTop(child, canvas.Height/2);
                        
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

        private void EnleverThemeSombre()
        {
            
            btnEchelle.Background = Brushes.White;
            btnEchelle.Foreground = Brushes.Black;

            btnmoins.Background = Brushes.White;
            btnmoins.Foreground = Brushes.Black;

            btnPlus.Background = Brushes.White;
            btnPlus.Foreground = Brushes.Black;
        }

        private void AppliquerThemeSombre()
        {
            BrushConverter bc = new BrushConverter();
            Brush CouleurBouton = (Brush)bc.ConvertFrom("#45463F");


            btnEchelle.Background = CouleurBouton;
            btnEchelle.Foreground = Brushes.White;

            btnmoins.Background = CouleurBouton;
            btnmoins.Foreground = Brushes.White;

            btnPlus.Background = CouleurBouton;
            btnPlus.Foreground = Brushes.White;
        }

        private void btnClip(object sender, RoutedEventArgs e)
        {

            foreach (Window w in Application.Current.Windows)
            {
                if (w.GetType() == typeof(PlanDeTravail))
                {
                    (w as PlanDeTravail).grdPlanTravail.Children.Clear();
                    (w as PlanDeTravail).grdPlanTravail.Children.Add(new PlanDeTravail2());
                }
            }

        }
    }
}
