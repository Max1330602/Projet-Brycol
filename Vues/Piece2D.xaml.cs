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
using MessageBox = System.Windows.MessageBox;
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


                zoomDefault = 11 / Piece_VM.pieceActuel.Longueur;
                zoom = 11 / Piece_VM.pieceActuel.Longueur;

                

            }
            else
            {
                Canvas.SetLeft(canvas, 0);
                Canvas.SetTop(canvas, 0);
                zoom = 1.4;
               

            }
            if (Piece_VM.pieceActuel.Longueur > 10 && Plan_VM.uniteDeMesure == "Mètres")
            {
                rulerText.Text = "0             2              4              6             8             10            12           14            16           18           20";
                rulerTextY.Text = "20             18              16              14             12              10              8            6              4            2             0";
                
            }
            /*else if(Plan_VM.uniteDeMesure == "Pieds" && Piece_VM.pieceActuel.Longueur < 32)
            {
                rulerText.Text = "0             3.3              4              6             8             10            12           14            16           18           20";
                rulerTextY.Text = "20             18              16              14             12              10              8            6              4            2             0";
            }*/
            rulerText.Visibility = Visibility.Visible;

            rulerTextY.Visibility = Visibility.Visible;
           
            ruler1.Visibility = Visibility.Visible;
            ruler2.Visibility = Visibility.Visible;
            ruler3.Visibility = Visibility.Visible;
            ruler4.Visibility = Visibility.Visible;
            ruler5.Visibility = Visibility.Visible;
            ruler6.Visibility = Visibility.Visible;

            canvas_Zoom.RenderTransform = new ScaleTransform(zoom, zoom); // transforme la grandeur du canvas
            

            DataContext = new Plan_VM();
        }

        #region attributs--------------------------------------------------------------------------------------------------------------------------------
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
        public double rotationPiece = 0;
        public bool movePiece = false;
        // Zoom
        private Double zoomMax = 1.9;
        private Double zoomMin = 0.8;
        private Double zoomSpeed = 0.001;
        private Double zoom;
        private Double zoomDefault;
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
                            if (i.Item.Nom.Contains("Porte") || i.Item.Nom.Contains("Fenêtre"))
                            {
                                Canvas.SetLeft(btntoolRotation, i.emplacementGauche + 105);
                                Canvas.SetTop(btntoolRotation, i.emplacementHaut + 45);
                                canvas.Children.Add(btntoolRotation);
                                Canvas.SetLeft(btntoolSupprimer, i.emplacementGauche + 164);
                                Canvas.SetTop(btntoolSupprimer, i.emplacementHaut + 45);
                                canvas.Children.Add(btntoolSupprimer);
                            }
                            else
                            {
                                Canvas.SetLeft(btntoolRotation, i.emplacementGauche + 5);
                                Canvas.SetTop(btntoolRotation, i.emplacementHaut - 19);
                                canvas.Children.Add(btntoolRotation);
                                Canvas.SetLeft(btntoolSupprimer, i.emplacementGauche + 64);
                                Canvas.SetTop(btntoolSupprimer, i.emplacementHaut - 19);
                                canvas.Children.Add(btntoolSupprimer);
                            }
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

                        ClipperPorteExtremite(i);
                        ClipperPorteDoubleExtremite(i);
                        ClipperFenetreDoubleExtremite(i);
                        ClipperFenetreExtremite(i);

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
                    if (movePiece)
                    {

                        Point mousePos = e.GetPosition(canvas_Zoom); // position absolu de la souris
                        Canvas.SetLeft(canvas, mousePos.X - clickPosition.X); // bouger le canvas
                        Canvas.SetTop(canvas, mousePos.Y - clickPosition.Y);
                        // var offset = position - mousePos;
                        /*if (rotationPiece == 0)
                        {

                                      
                            Canvas.SetLeft(canvas, mousePos.X - clickPosition.X); // bouger le canvas
                            Canvas.SetTop(canvas, mousePos.Y - clickPosition.Y);
                        }
                        else if(rotationPiece == 180)
                            {
                            Canvas.SetLeft(canvas, Canvas.GetLeft(canvas)- offset.X); // bouger le canvas
                            Canvas.SetTop(canvas, Canvas.GetTop(canvas) - offset.Y);
                        }
                        }*/
                    }
                    }
                }
            
        }

        private void btnRotation(object sender, RoutedEventArgs e)
        {

            RotateTransform rotation = toolbarImage.RenderTransform as RotateTransform;
            double rotationInDegrees = 0;
            if (rotation != null)
            {
                rotationInDegrees = rotation.Angle;
            }
            else
            {
                rotationInDegrees = 0;
            }
            if (rotationInDegrees == 0 || rotation == null)
            {
                toolbarImage.RenderTransformOrigin = new Point(0.5, 0.5);
                toolbarImage.RenderTransform = new RotateTransform(90);
            }
            else if (rotationInDegrees == 90)
            {
                toolbarImage.RenderTransformOrigin = new Point(0.5, 0.5);
                toolbarImage.RenderTransform = new RotateTransform(180);
            }
            else if (rotationInDegrees == 180)
            {
                toolbarImage.RenderTransformOrigin = new Point(0.5, 0.5);
                toolbarImage.RenderTransform = new RotateTransform(270);
            }
            else if (rotationInDegrees == 270)
            {
                toolbarImage.RenderTransformOrigin = new Point(0.5, 0.5);
                toolbarImage.RenderTransform = new RotateTransform(0);
            }

           



            foreach (ItemsPlan i in Item_VM.ItemsPlanActuel)
            {
                var bitmap = new BitmapImage(i.Item.ImgItem.UriSource);

                var imageBD = new Image { Source = bitmap };
                imageBD.Tag = i.ID;

                if (imageBD.Tag.ToString() == toolbarImage.Tag.ToString())
                {
                    
                        i.angleRotation = rotationInDegrees+90;                    
                }
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

            if (movePiece)
            {
                canvas_Zoom.RenderTransform = new ScaleTransform(zoom, zoom, mousePos.X, mousePos.Y); // transforme la grandeur du canvas selon la position de la souris
            }
            else
            {                
                canvas_Zoom.RenderTransform = new ScaleTransform(zoom, zoom); // transforme la grandeur du canvas si la pièce est clip
            }
                

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
                    if (ip.Item.Nom == "Porte")
                    {     
                        try
                        {
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri("pack://application:,,,/images/Items/Top/item" + ip.Item.ID + ip.cotePorte + ".png");
                            bitmap.EndInit();
                        }
                        catch (Exception e)
                        {
                            bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri("pack://application:,,,/images/Items/Top/item0.png");
                            bitmap.EndInit();
                        }
                    }
                    else
                    {
                        try
                        {
                            bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri("pack://application:,,,/images/Items/Top/item" + ip.Item.ID + ".png");
                            bitmap.EndInit();
                        }
                        catch (Exception e)
                        {
                            bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri("pack://application:,,,/images/Items/Top/item0.png");
                            bitmap.EndInit();
                        }
                    }

                        

                    try
                    {
                        if (ip.Item.Nom != "Porte")
                        {
                            bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            bitmap.UriSource = new Uri("pack://application:,,,/images/ItemsModifies/Item" + ip.Item.ID + "/" + ip.Couleur + ".png");
                            bitmap.EndInit();
                        }
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

                    #region angle-----------------------------------------------------------------------------------------------------------------
                    if (ip.angleRotation == 0)
                    {
                        image.RenderTransformOrigin = new Point(0.5, 0.5);
                        image.RenderTransform = new RotateTransform(0);
                    }
                    else if (ip.angleRotation == 90)
                    {
                        image.RenderTransformOrigin = new Point(0.5, 0.5);
                        image.RenderTransform = new RotateTransform(90);
                    }
                    else if (ip.angleRotation == 180)
                    {
                        image.RenderTransformOrigin = new Point(0.5, 0.5);
                        image.RenderTransform = new RotateTransform(180);
                    }
                    else if (ip.angleRotation == 270)
                    {

                        image.RenderTransformOrigin = new Point(0.5, 0.5);
                        image.RenderTransform = new RotateTransform(270);
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

            double zoomm = zoomDefault;
            canvas_Zoom.RenderTransform = new ScaleTransform(zoomm, zoomm); // transforme la grandeur du canvas
            canvas_Zoom.RenderTransform = new ScaleTransform(zoomm, zoomm); // transforme la grandeur du canvas
            slider1.Value = 50;
        }

        private void btnZoomIn(object sender, RoutedEventArgs e)
        {
            zoom = zoom + 0.12;
            if (zoom < zoomMin) { zoom = zoomMin; } // Limite le minimum
            if (zoom > zoomMax) { zoom = zoomMax; } // Limite le maximum

            slider1.Value = slider1.Ticks.Select(x => (double?)x).FirstOrDefault(x => x > slider1.Value) ?? slider1.Value;


            canvas.RenderTransform = new ScaleTransform(zoom, zoom); // transforme la grandeur du canvas
          
        }

        private void btnZoomOut(object sender, RoutedEventArgs e)
        {
            zoom = zoom - 0.12;
            if (zoom < zoomMin) { zoom = zoomMin; } // Limite le minimum
            if (zoom > zoomMax) { zoom = zoomMax; } // Limite le maximum

            slider1.Value = slider1.Ticks.Select(x => (double?)x).LastOrDefault(x => x < slider1.Value) ?? slider1.Value;

            canvas.RenderTransform = new ScaleTransform(zoom, zoom);  // transforme la grandeur du canvas
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
                        Canvas.SetLeft(child, canvas.Width / 2);
                        Canvas.SetTop(child, canvas.Height / 2);

                    }
                    else
                    {
                        if (!((Image)child).GetValue(Image.SourceProperty).ToString().Contains("26") &&
                            !((Image)child).GetValue(Image.SourceProperty).ToString().Contains("27") &&
                            !((Image)child).GetValue(Image.SourceProperty).ToString().Contains("28") &&
                            !((Image)child).GetValue(Image.SourceProperty).ToString().Contains("29"))
                        {
                            ImageBrush imgBrush = new ImageBrush();
                            imgBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Items/invalideItem.png"));
                            child.OpacityMask = imgBrush;
                            Plan_VM.validePourEnregistrer = false;
                        }

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
            
            btnClipStructure.Background = Brushes.White;
            btnClipStructure.Foreground = Brushes.Black;

            btnClipPiece.Background = Brushes.White;
            btnClipPiece.Foreground = Brushes.Black;

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

            btnClipStructure.Background = CouleurBouton;
            btnClipStructure.Foreground = Brushes.White;

            btnClipPiece.Background = CouleurBouton;
            btnClipPiece.Foreground = Brushes.White;

            btnEchelle.Background = CouleurBouton;
            btnEchelle.Foreground = Brushes.White;

            btnmoins.Background = CouleurBouton;
            btnmoins.Foreground = Brushes.White;

            btnPlus.Background = CouleurBouton;
            btnPlus.Foreground = Brushes.White;
        }

        private void btnClipStructure_Click(object sender, RoutedEventArgs e)
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


        private void btnClip(object sender, RoutedEventArgs e)
        {
            movePiece = false;
            if (Piece_VM.pieceActuel.Longueur > 10)
            {
                rulerText.Visibility = Visibility.Visible;
                ruler1.Visibility = Visibility.Visible;
                ruler2.Visibility = Visibility.Visible;
                ruler3.Visibility = Visibility.Visible;
                ruler4.Visibility = Visibility.Visible;
                ruler5.Visibility = Visibility.Visible;
                ruler6.Visibility = Visibility.Visible;
                rulerText.Text = "0             2              4              6             8              10              12            14              16             18            20";
                rulerTextY.Text = "20             18              16              14             12              10              8            6              4            2             0";
            }
            else
            {
                rulerText.Visibility = Visibility.Visible;
                rulerTextY.Visibility = Visibility.Visible;
                ruler1.Visibility = Visibility.Visible;
                ruler2.Visibility = Visibility.Visible;
                ruler3.Visibility = Visibility.Visible;
                ruler4.Visibility = Visibility.Visible;
                ruler5.Visibility = Visibility.Visible;
                ruler6.Visibility = Visibility.Visible;
            }

            Canvas.SetLeft(canvas, 0);
            Canvas.SetTop(canvas, 0);

            btnClipPiece.Visibility = Visibility.Hidden;
            btnClipPieceDeclipper.Visibility = Visibility.Visible;

            
        }


        private void btnDéclip(object sender, RoutedEventArgs e)
        {
            rulerText.Visibility = Visibility.Hidden;
            rulerTextY.Visibility = Visibility.Hidden;
            ruler1.Visibility = Visibility.Hidden;
            ruler2.Visibility = Visibility.Hidden;
            ruler3.Visibility = Visibility.Hidden;
            ruler4.Visibility = Visibility.Hidden;
            ruler5.Visibility = Visibility.Hidden;
            ruler6.Visibility = Visibility.Hidden;
            movePiece = true;
            btnClipPiece.Visibility = Visibility.Visible;
            btnClipPieceDeclipper.Visibility = Visibility.Hidden;
        }


        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double zoomSlider = e.NewValue;
            double zoomSliderTemp = 0;
            switch (zoomSlider)
            {
                case 1:
                    zoomSliderTemp = 0.8;
                    break;
                case 10:
                    zoomSliderTemp = 0.92;
                    break;
                case 20:
                    zoomSliderTemp = 1.04;
                    break;
                case 30:
                    zoomSliderTemp = 1.16;
                    break;
                case 40:
                    zoomSliderTemp = 1.28;
                    break;           
                case 50:             
                    zoomSliderTemp = 1.4;
                    break;           
                case 60:             
                    zoomSliderTemp = 1.52;
                    break;           
                case 70:             
                    zoomSliderTemp = 1.64;
                    break;           
                case 80:             
                    zoomSliderTemp = 1.76;
                    break;           
                case 90:             
                    zoomSliderTemp = 1.88;
                    break;           
                case 100:            
                    zoomSliderTemp = 1.9;
                    break;           


                default:
                    break;
            }
            canvas_Zoom.RenderTransform = new ScaleTransform(zoomSliderTemp, zoomSliderTemp);
        }

        #region Clip Strucure----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        // BD : 100 60 60
        private void ClipperPorteExtremite(ItemsPlan i)
        {
            if (i.Item.Nom == "Porte")
            {
                if (i.angleRotation == 0)
                {
                    if (i.emplacementHaut <= canvas.Height / 2 && i.emplacementGauche > 0 && i.emplacementGauche < canvas.Width - 74 && i.emplacementHaut > 0 && i.emplacementHaut < canvas.Height -41)
                        i.emplacementHaut = -5;
                    else if (i.emplacementHaut > canvas.Height / 2 && i.emplacementGauche > 0 && i.emplacementGauche < canvas.Width - 74 && i.emplacementHaut > 0 && i.emplacementHaut < canvas.Height -41)
                        i.emplacementHaut = canvas.Height - 10;
                    else
                    {
                        MessageBox.Show("Emplacement Invalide pour attacher une porte sur un mur.");
                        i.emplacementHaut = canvas.Height / 2;
                        i.emplacementGauche = canvas.Width / 2;
                    }
                }
                else if (i.angleRotation == 90)
                {
                    if (i.emplacementGauche <= canvas.Width / 2 && i.emplacementGauche > -19 && i.emplacementGauche < canvas.Width -57 && i.emplacementHaut > 14 && i.emplacementHaut < canvas.Height - 61)
                        i.emplacementGauche = -51;
                    else if (i.emplacementGauche > canvas.Width / 2 && i.emplacementGauche > -19 && i.emplacementGauche < canvas.Width + -57 && i.emplacementHaut > 14 && i.emplacementHaut < canvas.Height - 61)
                        i.emplacementGauche = canvas.Width - 54;
                    else
                    {
                        MessageBox.Show("Emplacement Invalide pour attacher une porte sur un mur.");
                        i.emplacementHaut = canvas.Height / 2;
                        i.emplacementGauche = canvas.Width / 2;
                    }
                }
                else if (i.angleRotation == 180)
                {
                    if (i.emplacementHaut <= canvas.Height / 2 && i.emplacementGauche > -1 && i.emplacementGauche < canvas.Width - 74 && i.emplacementHaut > -5 && i.emplacementHaut < canvas.Height -42)
                        i.emplacementHaut = -36;
                    else if (i.emplacementHaut > canvas.Height / 2 && i.emplacementGauche > -1 && i.emplacementGauche < canvas.Width -74 && i.emplacementHaut > -5 && i.emplacementHaut < canvas.Height-42)
                        i.emplacementHaut = canvas.Height -39;
                    else
                    {
                        MessageBox.Show("Emplacement Invalide pour attacher une porte sur un mur.");
                        i.emplacementHaut = canvas.Height / 2;
                        i.emplacementGauche = canvas.Width / 2;
                    }
                }
                else if (i.angleRotation == 270)
                {
                    if (i.emplacementGauche <= canvas.Width / 2 && i.emplacementGauche > -18 && i.emplacementGauche < canvas.Width - 56 && i.emplacementHaut > 14 && i.emplacementHaut < canvas.Height -59)
                        i.emplacementGauche = -21;
                    else if (i.emplacementGauche > canvas.Width / 2 && i.emplacementGauche > -18 && i.emplacementGauche < canvas.Width - 56 && i.emplacementHaut > 14 && i.emplacementHaut < canvas.Height -59)
                        i.emplacementGauche = canvas.Width - 23;
                    else
                    {
                        MessageBox.Show("Emplacement Invalide pour attacher une porte sur un mur.");
                        i.emplacementHaut = canvas.Height / 2;
                        i.emplacementGauche = canvas.Width / 2;
                    }
                }

            }
            OutilEF.brycolContexte.SaveChanges();
        }

        // BD : 200 60 60
        private void ClipperPorteDoubleExtremite(ItemsPlan i)
        {
            if (i.Item.Nom == "Porte Double")
            {
                if (i.angleRotation == 0)
                {
                    if (i.emplacementHaut <= canvas.Height / 2 && i.emplacementGauche > -3 && i.emplacementGauche < canvas.Width - 150 && i.emplacementHaut > -3 && i.emplacementHaut < canvas.Height - 41)
                        i.emplacementHaut = -5;
                    else if (i.emplacementHaut > canvas.Height / 2 && i.emplacementGauche > -3 && i.emplacementGauche < canvas.Width - 150 && i.emplacementHaut > -3 && i.emplacementHaut < canvas.Height - 41)
                        i.emplacementHaut = canvas.Height - 10;
                    else
                    {
                        MessageBox.Show("Emplacement Invalide pour attacher une porte sur un mur.");
                        i.emplacementHaut = canvas.Height / 2;
                        i.emplacementGauche = canvas.Width / 2;
                    }
                }
                else if (i.angleRotation == 90)
                {
                    if (i.emplacementGauche <= canvas.Width / 2 && i.emplacementGauche > -57 && i.emplacementGauche < canvas.Width -94 && i.emplacementHaut > 50 && i.emplacementHaut < canvas.Height - 97)
                        i.emplacementGauche = -90;
                    else if (i.emplacementGauche > canvas.Width / 2 && i.emplacementGauche > -57 && i.emplacementGauche < canvas.Width -94 && i.emplacementHaut > 50 && i.emplacementHaut < canvas.Height - 97)
                        i.emplacementGauche = canvas.Width - 92;
                    else
                    {
                        MessageBox.Show("Emplacement Invalide pour attacher une porte sur un mur.");
                        i.emplacementHaut = canvas.Height / 2;
                        i.emplacementGauche = canvas.Width / 2;
                    }
                }
                else if (i.angleRotation == 180)
                {
                    if (i.emplacementHaut <= canvas.Height / 2 && i.emplacementGauche > -1 && i.emplacementGauche < canvas.Width - 148 && i.emplacementHaut > -5 && i.emplacementHaut < canvas.Height- 41)
                        i.emplacementHaut = -37;
                    else if (i.emplacementHaut > canvas.Height / 2 && i.emplacementGauche > -1 && i.emplacementGauche < canvas.Width - 148 && i.emplacementHaut > -5 && i.emplacementHaut < canvas.Height- 41)
                        i.emplacementHaut = canvas.Height -37;
                    else
                    {
                        MessageBox.Show("Emplacement Invalide pour attacher une porte sur un mur.");
                        i.emplacementHaut = canvas.Height / 2;
                        i.emplacementGauche = canvas.Width / 2;
                    }
                }
                else if (i.angleRotation == 270)
                {
                    if (i.emplacementGauche <= canvas.Width / 2 && i.emplacementGauche > -57 && i.emplacementGauche < canvas.Width - 93 && i.emplacementHaut > 52 && i.emplacementHaut < canvas.Height-95)
                        i.emplacementGauche = -58;
                    else if (i.emplacementGauche > canvas.Width / 2 && i.emplacementGauche > -57 && i.emplacementGauche < canvas.Width - 93 && i.emplacementHaut > 52 && i.emplacementHaut < canvas.Height-95)
                        i.emplacementGauche = canvas.Width - 62;
                    else
                    {
                        MessageBox.Show("Emplacement Invalide pour attacher une porte sur un mur.");
                        i.emplacementHaut = canvas.Height / 2;
                        i.emplacementGauche = canvas.Width / 2;
                    }
                }

            }
            OutilEF.brycolContexte.SaveChanges();
        }

        // BD : 100 40 60
        private void ClipperFenetreExtremite(ItemsPlan i)
        {
            if (i.Item.Nom == "Fenêtre")
            {
                if (i.angleRotation == 0)
                {
                    if (i.emplacementHaut <= canvas.Height / 2 && i.emplacementGauche > 0 && i.emplacementGauche < canvas.Width -75 && i.emplacementHaut > -13 && i.emplacementHaut < canvas.Height -17)
                        i.emplacementHaut = -13.5;
                    else if (i.emplacementHaut > canvas.Height / 2 && i.emplacementGauche > 0 && i.emplacementGauche < canvas.Width -75 && i.emplacementHaut > -13 && i.emplacementHaut < canvas.Height - 17)
                        i.emplacementHaut = canvas.Height -16;
                    else
                    {
                        MessageBox.Show("Emplacement Invalide pour attacher une fenêtre sur un mur.");
                        i.emplacementHaut = canvas.Height / 2;
                        i.emplacementGauche = canvas.Width / 2;
                    }
                }
                else if (i.angleRotation == 90)
                {
                    if (i.emplacementGauche <= canvas.Width / 2 && i.emplacementGauche > -35 && i.emplacementGauche < canvas.Width && i.emplacementHaut > 22 && i.emplacementHaut < canvas.Height - 52)
                        i.emplacementGauche = -36.5;
                    else if (i.emplacementGauche > canvas.Width / 2 && i.emplacementGauche > -35 && i.emplacementGauche < canvas.Width && i.emplacementHaut >  22 && i.emplacementHaut < canvas.Height - 52)
                        i.emplacementGauche = canvas.Width - 39;
                    else
                    {
                        MessageBox.Show("Emplacement Invalide pour attacher une fenêtre sur un mur.");
                        i.emplacementHaut = canvas.Height / 2;
                        i.emplacementGauche = canvas.Width / 2;
                    }
                }
                else if (i.angleRotation == 180)
                {
                    if (i.emplacementHaut <= canvas.Height / 2 && i.emplacementGauche > -1 && i.emplacementGauche < canvas.Width -75 && i.emplacementHaut > -13 && i.emplacementHaut < canvas.Height - 17)
                        i.emplacementHaut = -13.5;
                    else if (i.emplacementHaut > canvas.Height / 2 && i.emplacementGauche > -1 && i.emplacementGauche < canvas.Width -75 && i.emplacementHaut > -13 && i.emplacementHaut < canvas.Height - 17)
                        i.emplacementHaut = canvas.Height - 16;
                    else
                    {
                        MessageBox.Show("Emplacement Invalide pour attacher une fenêtre sur un mur.");
                        i.emplacementHaut = canvas.Height / 2;
                        i.emplacementGauche = canvas.Width / 2;
                    }
                }
                else if (i.angleRotation == 270)
                {
                    if (i.emplacementGauche <= canvas.Width / 2 && i.emplacementGauche > -36 && i.emplacementGauche < canvas.Width -40 && i.emplacementHaut > 22 && i.emplacementHaut < canvas.Height - 52)
                        i.emplacementGauche = -36;
                    else if (i.emplacementGauche > canvas.Width / 2 && i.emplacementGauche > -36 && i.emplacementGauche < canvas.Width -40 && i.emplacementHaut > 22 && i.emplacementHaut < canvas.Height - 52)
                        i.emplacementGauche = canvas.Width - 39;
                    else
                    {
                        MessageBox.Show("Emplacement Invalide pour attacher une fenêtre sur un mur.");
                        i.emplacementHaut = canvas.Height / 2;
                        i.emplacementGauche = canvas.Width / 2;
                    }
                }

            }
            OutilEF.brycolContexte.SaveChanges();
        }
        // BD : 200 40 60
        private void ClipperFenetreDoubleExtremite(ItemsPlan i)
        {
            if (i.Item.Nom == "Fenêtre Double")
            {
                if (i.angleRotation == 0)
                {
                    if (i.emplacementHaut <= canvas.Height / 2 && i.emplacementGauche > 0.5 && i.emplacementGauche < canvas.Width -151 && i.emplacementHaut > -10.1 && i.emplacementHaut < canvas.Height - 20)
                        i.emplacementHaut = -13;
                    else if (i.emplacementHaut > canvas.Height / 2 && i.emplacementGauche > 0.5 && i.emplacementGauche < canvas.Width -51 && i.emplacementHaut > -10.1 && i.emplacementHaut < canvas.Height - 20)
                        i.emplacementHaut = canvas.Height-17;
                    else
                    {
                        MessageBox.Show("Emplacement Invalide pour attacher une fenêtre sur un mur.");
                        i.emplacementHaut = canvas.Height / 2;
                        i.emplacementGauche = canvas.Width / 2;
                    }
                }
                else if (i.angleRotation == 90)
                {
                    if (i.emplacementGauche <= canvas.Width / 2 && i.emplacementGauche > -70.6 && i.emplacementGauche < canvas.Width -80 && i.emplacementHaut > 61 && i.emplacementHaut < canvas.Height - 90)
                        i.emplacementGauche = -73;
                    else if (i.emplacementGauche > canvas.Width / 2 && i.emplacementGauche > -70.6 && i.emplacementGauche < canvas.Width -80 && i.emplacementHaut > 61 && i.emplacementHaut < canvas.Height - 90)
                        i.emplacementGauche = canvas.Width - 77;
                    else
                    {
                        MessageBox.Show("Emplacement Invalide pour attacher une fenêtre sur un mur.");
                        i.emplacementHaut = canvas.Height / 2;
                        i.emplacementGauche = canvas.Width / 2;
                    }
                }
                else if (i.angleRotation == 180)
                {
                    if (i.emplacementHaut <= canvas.Height / 2 && i.emplacementGauche > 0 && i.emplacementGauche < canvas.Width -151 && i.emplacementHaut > -10.2 && i.emplacementHaut < canvas.Height - 20)
                        i.emplacementHaut = -13.5;
                    else if (i.emplacementHaut > canvas.Height / 2 && i.emplacementGauche > 0 && i.emplacementGauche < canvas.Width -151 && i.emplacementHaut > -10.2 && i.emplacementHaut < canvas.Height - 20)
                        i.emplacementHaut = canvas.Height - 16;
                    else
                    {
                        MessageBox.Show("Emplacement Invalide pour attacher une fenêtre sur un mur.");
                        i.emplacementHaut = canvas.Height / 2;
                        i.emplacementGauche = canvas.Width / 2;
                    }
                }
                else if (i.angleRotation == 270)
                {
                    if (i.emplacementGauche <= canvas.Width / 2 && i.emplacementGauche > -70.6 && i.emplacementGauche < canvas.Width -80.5 && i.emplacementHaut > 60.5 && i.emplacementHaut < canvas.Height - 90.5)
                        i.emplacementGauche = -73;
                    else if (i.emplacementGauche > canvas.Width / 2 && i.emplacementGauche > -70.6 && i.emplacementGauche < canvas.Width -80.5 && i.emplacementHaut > 60.5 && i.emplacementHaut < canvas.Height - 90.5)
                        i.emplacementGauche = canvas.Width -76.5;
                    else
                    {
                        MessageBox.Show("Emplacement Invalide pour attacher une fenêtre sur un mur.");
                        i.emplacementHaut = canvas.Height / 2;
                        i.emplacementGauche = canvas.Width / 2;
                    }
                }

            }
            OutilEF.brycolContexte.SaveChanges();
        }
        #endregion


        private void btnRotationPiece(object sender, RoutedEventArgs e)
        {
            rotationPiece = rotationPiece + 90;
            canvas.RenderTransformOrigin = new Point(0.5, 0.5);

            canvas.RenderTransform = new RotateTransform(rotationPiece);

        }

       



    }
}
