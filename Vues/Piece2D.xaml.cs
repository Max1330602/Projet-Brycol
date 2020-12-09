using App_Brycol.Modele;
using App_Brycol.Outils;
using App_Brycol.VuesModele;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
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
            Plan_VM.zoomPiece = 11 / Piece_VM.pieceActuel.Longueur;
            DataContext = new Plan_VM();      
        }

        #region attributs--------------------------------------------------------------------------------------------------------------------------------
        int compteur;
        private Point clickPosition;
        private Point position;
        public static Image draggedImage;
        public static Image toolbarImage;
        public static Image imageSelection;
        private Point mousePosition;
        private bool move;
        public double Zoom;       
        public const float pixelToM = 3779.5275590551f / echelle;
        public const float pixelToPied = 1151.9999999832f / echelle;
        public const float pixelToCm = 37.7952755906f / echelle;
        public const float echelle = 50;
        public double rotationPiece = 0;
        public bool movePiece = false;
        public bool pieceCreer = false;
        public int choixMurAttendre = 0;
        Point start;
        Point end;
        public double rectanglePieceWidth;
        public double rectanglePieceHeight;
        public bool validation;

        // Zoom
        private Double zoomMax = 1.9;
        private Double zoomMin = 0.8;
        private Double zoomSpeed = 0.001;
        private Double zoom;
        private Double zoomDefault;
        private Stack<String> stackItem = new Stack<string>();
        private List<Image> itemSelectionee = new List<Image>();
        private double _zoomLevel;
        public double ZoomLevel
        {
            get { return _zoomLevel; }
            set
            {
                if (_zoomLevel != value)
                {
                    _zoomLevel = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void setStructureCanvas()
        {
            foreach (ItemsPlan i in Item_VM.ItemsPlanActuel)
            {
                var bitmap = new BitmapImage(i.Item.ImgItem.UriSource);

                var imageBD = new Image { Source = bitmap };
                imageBD.Tag = i.ID;

                    foreach (Image image in canvas.Children.OfType<Image>())
                    {
                        if (imageBD.Tag.ToString() == image.Tag.ToString())
                        {
                            if (i.mur == 1)
                            {
                                Canvas.SetLeft(image, i.emplacementGauche);
                                Canvas.SetTop(image, -5);
                            }
                            else if (i.mur == 2)
                            {

                                Canvas.SetTop(image, i.emplacementGauche);
                                Canvas.SetLeft(image, canvas.ActualWidth - i.Item.Longueur);
                                i.angleRotation = 90;
                                image.RenderTransformOrigin = new Point(0.5, 0.5);
                                image.RenderTransform = new RotateTransform(90);
                            }
                            else if (i.mur == 3)
                            {
                                Canvas.SetTop(image, canvas.ActualHeight - i.Item.Longueur + 17);
                                Canvas.SetLeft(image, i.emplacementGauche);
                                i.angleRotation = 180;
                                image.RenderTransformOrigin = new Point(0.5, 0.5);
                                image.RenderTransform = new RotateTransform(180);
                            }
                            else if (i.mur == 4)
                            {
                                Canvas.SetTop(image, i.emplacementGauche);
                                Canvas.SetLeft(image, -17);
                                i.angleRotation = 270;
                                image.RenderTransformOrigin = new Point(0.5, 0.5);
                                image.RenderTransform = new RotateTransform(270);
                            }



                        }
                    }

                }
            }
        
        void setCanvasPiece()
            {
            if (Item_VM.ItemsPlanActuel != null)
            {
                setStructureCanvas();
            }
            
            try
            {
                ImageBrush imgBrushMur = new ImageBrush();
                imgBrushMur.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Items/mur.jpg"));
                canvasMur.Background = imgBrushMur;
            }
            catch (Exception)
            {
                ImageBrush imgBrush = new ImageBrush();
                imgBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Items/plancheAucun.jpg"));
                canvasMur.Background = imgBrush;
            }
            

            if (Piece_VM.pieceActuel.Longueur == 0 && Piece_VM.pieceActuel.Largeur == 0)
            {
                zoom = 1.2;
                Canvas.SetLeft(canvas_Zoom, 0);
                Canvas.SetTop(canvas_Zoom, 0);
                canvas_Zoom.RenderTransform = new ScaleTransform(zoom, zoom); // transforme la grandeur du canvas         
            }


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
           
            if (Plan_VM.uniteDeMesure == "Mètres" && Piece_VM.pieceActuel.Longueur != 0 && Piece_VM.pieceActuel.Largeur != 0)
            {
                if (!Plan_VM.catalogueClick)
                {
                    Canvas.SetLeft(canvas, 10);
                    Canvas.SetTop(canvas, 10);

                    zoomDefault = 11 / Piece_VM.pieceActuel.Longueur;
                    zoom = 11 / Piece_VM.pieceActuel.Longueur;
                    
                    canvas_Zoom.RenderTransform = new ScaleTransform(zoom, zoom); // transforme la grandeur du canvas  

                    /*
                    if (Piece_VM.pieceActuel.Longueur > 10 && Plan_VM.uniteDeMesure == "Mètres")
                    {
                        rulerText.Text = "0              2              4              6              8             10            12            14             16            18            20";
                        rulerTextY.Text = "20             18             16              14             12              10              8            6              4            2             0";

                    }*/


                    /*else if(Plan_VM.uniteDeMesure == "Pieds" && Piece_VM.pieceActuel.Longueur < 32)
                    {
                        rulerText.Text = "0             3.3              4              6             8             10            12           14            16           18           20";
                        rulerTextY.Text = "20             18              16              14             12              10              8            6              4            2             0";
                    }*/
                    montrerClip();

                }
                else
                {
                    if (Plan_VM.clip)
                    {
                        montrerClip();
                        Canvas.SetLeft(canvas, 10);
                        Canvas.SetTop(canvas, 10);

                    }
                    else
                    {
                        cacherClip();
                        canvas.RenderTransformOrigin = new Point(0.5, 0.5);
                        canvas.RenderTransform = new RotateTransform(Plan_VM.rotationPieceVM);
                    }
                    
                    canvas_Zoom.RenderTransform = new ScaleTransform(Plan_VM.zoomPiece, Plan_VM.zoomPiece); // transforme la grandeur du canvas 
                    Canvas.SetLeft(canvas, Plan_VM.canvasPieceLeft);
                    Canvas.SetTop(canvas, Plan_VM.canvasPieceTop);                   

                }
               
                

            }
            else if (Plan_VM.uniteDeMesure == "Pieds" && Piece_VM.pieceActuel.Longueur != 0 && Piece_VM.pieceActuel.Largeur != 0)
            {
                Canvas.SetLeft(canvas, 10);
                Canvas.SetTop(canvas, 10);
                zoomDefault = 36.08 / Piece_VM.pieceActuel.Longueur;
                zoom = 36.08 / Piece_VM.pieceActuel.Longueur;
                canvas_Zoom.RenderTransform = new ScaleTransform(zoom, zoom); // transforme la grandeur du canvas    
            }         
        }

        private void cacherClip()
        {

            btnPieceRotation.Visibility = Visibility.Visible;
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

        private void montrerClip()
        {

            rulerText.Visibility = Visibility.Visible;
            rulerTextY.Visibility = Visibility.Visible;
            ruler1.Visibility = Visibility.Visible;
            ruler2.Visibility = Visibility.Visible;
            ruler3.Visibility = Visibility.Visible;
            ruler4.Visibility = Visibility.Visible;
            ruler5.Visibility = Visibility.Visible;
            ruler6.Visibility = Visibility.Visible;
            btnPieceRotation.Visibility = Visibility.Hidden;
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

                #region Affichage des murs--------------------------------------------------------------------------------------
                //Pour l'ouverture des murs--------------------------------------------------
                Canvas.SetLeft(canvasMur, Canvas.GetLeft(canvas));
                Canvas.SetTop(canvasMur, Canvas.GetTop(canvas));
                if (clickPosition.Y < 5)
                {
                    btntoolRotation.Visibility = Visibility.Hidden;
                    btntoolSupprimer.Visibility = Visibility.Hidden;
                    btntoolModifier.Visibility = Visibility.Hidden;
                    btntoolMurRotation.Visibility = Visibility.Hidden;
                    btntoolMurSupprimer.Visibility = Visibility.Hidden;
                    btntoolMurModifier.Visibility = Visibility.Hidden;
                    foreach (Image child in canvasMur.Children.OfType<Image>())
                    {
                        foreach (ItemsPlan ip in Item_VM.ItemsPlanActuel)
                        {
                            var bitmap = new BitmapImage(ip.Item.ImgItem.UriSource);

                            var imageBD = new Image { Source = bitmap };
                            imageBD.Tag = ip.ID;
                            if (imageBD.Tag.ToString() == child.Tag.ToString())
                            {
                                if (ip.mur == 1)
                                {
                                    child.Visibility = Visibility.Visible;
                                }
                                else
                                {
                                    child.Visibility = Visibility.Hidden;
                                }
                            }
                        }
                    }

                    if (Plan_VM.PlanActuel.Piece.UniteDeMesure == "Mètres")
                    {
                        canvasMur.Width = Plan_VM.PlanActuel.Piece.Largeur * pixelToM;

                    }
                    else
                    {
                        canvasMur.Width = Plan_VM.PlanActuel.Piece.Largeur * pixelToPied;

                    }

                    canvasMur.Visibility = Visibility.Visible;
                    popupMur.IsOpen = true;
                   
                }
                else if (clickPosition.Y > (Plan_VM.PlanActuel.Piece.Longueur*pixelToM)-5)
                {
                    btntoolRotation.Visibility = Visibility.Hidden;
                    btntoolSupprimer.Visibility = Visibility.Hidden;
                    btntoolModifier.Visibility = Visibility.Hidden;
                    btntoolMurRotation.Visibility = Visibility.Hidden;
                    btntoolMurSupprimer.Visibility = Visibility.Hidden;
                    btntoolMurModifier.Visibility = Visibility.Hidden;
                    foreach (Image child in canvasMur.Children.OfType<Image>())
                    {
                        foreach (ItemsPlan ip in Item_VM.ItemsPlanActuel)
                        {
                            var bitmap = new BitmapImage(ip.Item.ImgItem.UriSource);

                            var imageBD = new Image { Source = bitmap };
                            imageBD.Tag = ip.ID;
                            if (imageBD.Tag.ToString() == child.Tag.ToString())
                            {
                                if (ip.mur == 3)
                                {
                                    child.Visibility = Visibility.Visible;
                                }
                                else
                                {
                                    child.Visibility = Visibility.Hidden;
                                }
                            }
                        }
                    }
                    canvasMur.Width = Plan_VM.PlanActuel.Piece.Largeur * pixelToM;
                    canvasMur.Visibility = Visibility.Visible;
                    popupMur.IsOpen = true;
                }
                else if (clickPosition.X < 5)
                {
                    btntoolRotation.Visibility = Visibility.Hidden;
                    btntoolSupprimer.Visibility = Visibility.Hidden;
                    btntoolModifier.Visibility = Visibility.Hidden;
                    btntoolMurRotation.Visibility = Visibility.Hidden;
                    btntoolMurSupprimer.Visibility = Visibility.Hidden;
                    btntoolMurModifier.Visibility = Visibility.Hidden;
                    foreach (Image child in canvasMur.Children.OfType<Image>())
                        {
                            foreach (ItemsPlan ip in Item_VM.ItemsPlanActuel)
                            {
                                var bitmap = new BitmapImage(ip.Item.ImgItem.UriSource);

                                var imageBD = new Image { Source = bitmap };
                                imageBD.Tag = ip.ID;
                                if (imageBD.Tag.ToString() == child.Tag.ToString())
                                {
                                    if (ip.mur == 4)
                                    {
                                        child.Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        child.Visibility = Visibility.Hidden;
                                    }
                                }
                            }
                        
                        }
                    canvasMur.Width = Plan_VM.PlanActuel.Piece.Longueur * pixelToM;
                    canvasMur.Visibility = Visibility.Visible;
                    popupMur.IsOpen = true;
                }
                else if (clickPosition.X > (Plan_VM.PlanActuel.Piece.Largeur * pixelToM) - 5)
                {
                    btntoolRotation.Visibility = Visibility.Hidden;
                    btntoolSupprimer.Visibility = Visibility.Hidden;
                    btntoolModifier.Visibility = Visibility.Hidden;
                    btntoolMurRotation.Visibility = Visibility.Hidden;
                    btntoolMurSupprimer.Visibility = Visibility.Hidden;
                    btntoolMurModifier.Visibility = Visibility.Hidden;
                    foreach (Image child in canvasMur.Children.OfType<Image>())
                    {
                        foreach (ItemsPlan ip in Item_VM.ItemsPlanActuel)
                        {
                            var bitmap = new BitmapImage(ip.Item.ImgItem.UriSource);

                            var imageBD = new Image { Source = bitmap };
                            imageBD.Tag = ip.ID;
                            if (imageBD.Tag.ToString() == child.Tag.ToString())
                            {
                                if (ip.mur == 2)
                                {
                                    child.Visibility = Visibility.Visible;
                                }
                                else
                                {
                                    child.Visibility = Visibility.Hidden;
                                }
                            }
                        }
                    }
                    canvasMur.Width = Plan_VM.PlanActuel.Piece.Longueur * pixelToM;
                    canvasMur.Visibility = Visibility.Visible;
                    popupMur.IsOpen = true;
                }
                else
                {
                    canvasMur.Visibility = Visibility.Hidden;
                    btntoolMurRotation.Visibility = Visibility.Hidden;
                    btntoolMurSupprimer.Visibility = Visibility.Hidden;
                    btntoolMurModifier.Visibility = Visibility.Hidden;
                    popupMur.IsOpen = false;
                }
                #endregion

                if (image != null && canvas.CaptureMouse())
                {
                    canvas.Children.Remove(btntoolRotation);
                    canvas.Children.Remove(btntoolSupprimer);
                    canvas.Children.Remove(btntoolModifier);
                    draggedImage = image;
                    imageSelection = image;
                    

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
                                Canvas.SetLeft(btntoolModifier, i.emplacementGauche + 224);
                                Canvas.SetTop(btntoolModifier, i.emplacementHaut + 45);
                                canvas.Children.Add(btntoolModifier);
                            }
                            else
                            {
                                if (i.emplacementHaut > 19)
                                {
                                    Canvas.SetLeft(btntoolRotation, i.emplacementGauche + 5);
                                    Canvas.SetTop(btntoolRotation, i.emplacementHaut - 19);
                                    canvas.Children.Add(btntoolRotation);
                                    Canvas.SetLeft(btntoolSupprimer, i.emplacementGauche + 64);
                                    Canvas.SetTop(btntoolSupprimer, i.emplacementHaut - 19);
                                    canvas.Children.Add(btntoolSupprimer);
                                    Canvas.SetLeft(btntoolModifier, i.emplacementGauche + 124);
                                    Canvas.SetTop(btntoolModifier, i.emplacementHaut -19);
                                    canvas.Children.Add(btntoolModifier);
                                }
                                else
                                {
                                    Canvas.SetLeft(btntoolRotation, i.emplacementGauche + 5);
                                    Canvas.SetTop(btntoolRotation, i.emplacementHaut+ i.Item.Longueur-15);
                                    canvas.Children.Add(btntoolRotation);
                                    Canvas.SetLeft(btntoolSupprimer, i.emplacementGauche + 64);
                                    Canvas.SetTop(btntoolSupprimer, i.emplacementHaut + i.Item.Longueur-15);
                                    canvas.Children.Add(btntoolSupprimer);
                                    Canvas.SetLeft(btntoolModifier, i.emplacementGauche + 124);
                                    Canvas.SetTop(btntoolModifier, i.emplacementHaut + i.Item.Longueur - 15);
                                    canvas.Children.Add(btntoolModifier);
                                }
                                
                            }
                        }
                    }
                    btntoolRotation.Visibility = Visibility.Visible;
                    btntoolSupprimer.Visibility = Visibility.Visible;
                    btntoolModifier.Visibility = Visibility.Visible;
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
                        foreach (Image image in canvas.Children.OfType<Image>())
                        {
                            if (imageBD.Tag.ToString() == image.Tag.ToString())
                            {
                                positionnerStructureDansPiece(i, image);                                                                                                   
                            }
                        }

                            if (i.Item.Nom.Contains("Porte") || i.Item.Nom.Contains("Fenêtre"))
                            {
                            Canvas.SetLeft(btntoolRotation, i.emplacementGauche + 105);
                            Canvas.SetTop(btntoolRotation, i.emplacementHaut + 45);
                          
                            Canvas.SetLeft(btntoolSupprimer, i.emplacementGauche + 164);
                            Canvas.SetTop(btntoolSupprimer, i.emplacementHaut + 45);

                            Canvas.SetLeft(btntoolModifier, i.emplacementGauche + 224);
                            Canvas.SetTop(btntoolModifier, i.emplacementHaut + 45);

                            }
                            else
                            {
                                if (i.emplacementHaut > 19)
                                {
                                    Canvas.SetLeft(btntoolRotation, i.emplacementGauche + 5);
                                    Canvas.SetTop(btntoolRotation, i.emplacementHaut - 19);
                              
                                    Canvas.SetLeft(btntoolSupprimer, i.emplacementGauche + 64);
                                    Canvas.SetTop(btntoolSupprimer, i.emplacementHaut - 19);

                                    Canvas.SetLeft(btntoolModifier, i.emplacementGauche + 124);
                                    Canvas.SetTop(btntoolModifier, i.emplacementHaut - 19);

                                }
                                else
                                {
                                    Canvas.SetLeft(btntoolRotation, i.emplacementGauche + 5);
                                    Canvas.SetTop(btntoolRotation, i.emplacementHaut + i.Item.Longueur - 15);
                               
                                    Canvas.SetLeft(btntoolSupprimer, i.emplacementGauche + 64);
                                    Canvas.SetTop(btntoolSupprimer, i.emplacementHaut + i.Item.Longueur - 15);

                                    Canvas.SetLeft(btntoolModifier, i.emplacementGauche + 124);
                                    Canvas.SetTop(btntoolModifier, i.emplacementHaut + i.Item.Longueur - 15);

                                }
                            }
                        if (canvasMur.Visibility == Visibility.Hidden)
                        {
                            ClipperPorteExtremite(i);
                            ClipperPorteDoubleExtremite(i);
                            ClipperFenetreDoubleExtremite(i);
                            ClipperFenetreExtremite(i);
                        }
                        else
                        {
                            ClipperItemSurMur(i);
                        }
                        

                        Canvas.SetLeft(draggedImage, i.emplacementGauche);
                        Canvas.SetTop(draggedImage, i.emplacementHaut);
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

        private void positionnerStructureDansPiece(ItemsPlan i, Image image)
        { 

        if (i.mur == 1)
         {
             if (i.Item.Nom == "Porte")
             {
                 Canvas.SetLeft(image, i.emplacementGauche+74);
                 Canvas.SetTop(image, -5);
             }
             else if (i.Item.Nom == "Porte Double")
             {
                 Canvas.SetLeft(image, i.emplacementGauche +150);
                 Canvas.SetTop(image, -5);
             }
             else if (i.Item.Nom == "Fenêtre")
            {
                Canvas.SetLeft(image, i.emplacementGauche + 75);
                Canvas.SetTop(image, -14);
            }
            else
            {
                Canvas.SetLeft(image, i.emplacementGauche + 151);
                Canvas.SetTop(image, -14);
            }
                                    
         }
         else if (i.mur == 2)
         {
                image.RenderTransformOrigin = new Point(0.5, 0.5);
                image.RenderTransform = new RotateTransform(90);
                if (i.Item.Nom == "Porte")
                {
                    Canvas.SetTop(image, i.emplacementGauche + 85);
                    Canvas.SetLeft(image, canvas.ActualWidth- 57);
                }
                else if (i.Item.Nom == "Porte Double")
                {
                    Canvas.SetTop(image, i.emplacementGauche + 200);
                    Canvas.SetLeft(image, canvas.ActualWidth - 97);
                }
                else if (i.Item.Nom == "Fenêtre")
                {
                    Canvas.SetTop(image, i.emplacementGauche + 220);
                    Canvas.SetLeft(image, canvas.ActualWidth - 80);
                }
                else
                {
                    Canvas.SetTop(image, i.emplacementGauche + 195);
                    Canvas.SetLeft(image, canvas.ActualWidth - 80);
                }
         }
         else if (i.mur == 3)
          {
                image.RenderTransformOrigin = new Point(0.5, 0.5);
                image.RenderTransform = new RotateTransform(180);
                if (i.Item.Nom == "Porte")
                {
                    Canvas.SetTop(image, canvas.ActualHeight - i.Item.Longueur + 17);
                    Canvas.SetLeft(image, i.emplacementGauche + 78);
                }
                else if (i.Item.Nom == "Porte Double")
                {
                    Canvas.SetTop(image, canvas.ActualHeight - i.Item.Longueur + 17);
                    Canvas.SetLeft(image, i.emplacementGauche + 150);
                }
                else if (i.Item.Nom == "Fenêtre")
                {
                    Canvas.SetTop(image, canvas.ActualHeight - i.Item.Longueur + 17);
                    Canvas.SetLeft(image, i.emplacementGauche + 150);

                }
                else
                {
                    Canvas.SetTop(image, canvas.ActualHeight - i.Item.Longueur + 17);
                    Canvas.SetLeft(image, i.emplacementGauche + 140);
                }
                
            }
            else if (i.mur == 4)
            {
                image.RenderTransformOrigin = new Point(0.5, 0.5);
                image.RenderTransform = new RotateTransform(270);
                if (i.Item.Nom == "Porte")
                {
                    Canvas.SetTop(image, i.emplacementGauche + 85);
                    Canvas.SetLeft(image, -19);
                }
                else if (i.Item.Nom == "Porte Double")
                {
                    Canvas.SetTop(image, i.emplacementGauche + 200);
                    Canvas.SetLeft(image, -58);
                }
                else if (i.Item.Nom == "Fenêtre")
                {
                    Canvas.SetTop(image, i.emplacementGauche + 220);
                    Canvas.SetLeft(image, -73);
                }
                else
                {
                    Canvas.SetTop(image, i.emplacementGauche + 195);
                    Canvas.SetLeft(image, -73);
                }
            }
        }

        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            Plan_VM.canvasPieceLeft = Canvas.GetLeft(canvas);
            Plan_VM.canvasPieceTop = Canvas.GetTop(canvas);
            
                if (draggedImage != null && move == true)
                {             
                    var position = e.GetPosition(canvas);
                    var offset = position - mousePosition;
                    mousePosition = position;
                
                    Canvas.SetLeft(draggedImage, Canvas.GetLeft(draggedImage) + offset.X);
                    Canvas.SetTop(draggedImage, Canvas.GetTop(draggedImage) + offset.Y);

                    if (itemSelectionee.Count != 0)
                    {
                        foreach (Image item in itemSelectionee)
                        {
                            Canvas.SetLeft(item, Canvas.GetLeft(item) + offset.X);
                            Canvas.SetTop(item, Canvas.GetTop(item) + offset.Y);
                        }
                    }
                    
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

        private void btnModifier(object sender, RoutedEventArgs e)
        {
            if (Piece2D.toolbarImage != null && Piece2D.toolbarImage.Source != null)
            {
                foreach (ItemsPlan ip in Item_VM.ItemsPlanActuel)
                {
                    if (Piece2D.toolbarImage.Source.ToString().Contains(ip.Item.ID.ToString()))
                    {
                        if (ip.Item.Nom.Contains("Porte Double") || ip.Item.Nom.Contains("Fenêtre"))
                        {
                            MessageBox.Show("Impossible de modifier une double porte ou une fenêtre quelconque.");
                        }
                        else if (ip.Item.Nom == "Porte")
                        {
                            ModifierPorte popUp = new ModifierPorte();
                            popUp.ShowDialog();
                            return;
                        }
                        else
                        {
                            ModifierItem popUp = new ModifierItem();
                            popUp.ShowDialog();
                            return;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Il faut d'abord sélectionner un item dans le plan");
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
            Plan_VM.zoomPiece = zoom;
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
                        foreach (ItemsPlan ip in Item_VM.ItemsPlanActuel.ToList())
                        {
                        
                            var bitmap = new BitmapImage(ip.Item.ImgItem.UriSource);
                            var imageBD = new Image { Source = bitmap };
                            imageBD.Tag = ip.ID;
                        if (itemSelectionee.Count == 0)
                        {
                            // Pour le simple delete
                            if (imageBD.Tag.ToString() == toolbarImage.Tag.ToString())
                            {
                                foreach (Image image in canvas.Children.OfType<Image>())
                                {
                                    // Pour le simple delete
                                    if (imageBD.Tag.ToString() == image.Tag.ToString())
                                    {
                                        Item_VM.ItemsPlanActuel.Remove(ip);
                                        image.Source = null;
                                        btntoolRotation.Visibility = Visibility.Hidden;
                                        btntoolSupprimer.Visibility = Visibility.Hidden;
                                        btntoolModifier.Visibility = Visibility.Hidden;
                                    }
                                }
                                Item_VM.ItemsPlanActuel.Remove(ip);
                                toolbarImage.Source = null;
                                imageBD.Source = null;
                                btntoolRotation.Visibility = Visibility.Hidden;
                                btntoolSupprimer.Visibility = Visibility.Hidden;
                                btntoolModifier.Visibility = Visibility.Hidden;
                                OutilEF.brycolContexte.lstItems.Remove(ip);
                                OutilEF.brycolContexte.SaveChanges();
                                return;
                            }
                        }
                        else
                        {
                            // Pour la sélection multiple!
                            foreach (Image item in itemSelectionee)
                            {
                                if (imageBD.Tag.ToString() == item.Tag.ToString())
                                {
                                    foreach (Image image in canvas.Children.OfType<Image>())
                                    {
                                        // Pour le simple delete
                                        if (imageBD.Tag.ToString() == image.Tag.ToString())
                                        {
                                            Item_VM.ItemsPlanActuel.Remove(ip);
                                            image.Source = null;
                                            btntoolRotation.Visibility = Visibility.Hidden;
                                            btntoolSupprimer.Visibility = Visibility.Hidden;
                                            btntoolModifier.Visibility = Visibility.Hidden;
                                        }
                                    }
                                    Item_VM.ItemsPlanActuel.Remove(ip);
                                    item.Source = null;
                                    btntoolRotation.Visibility = Visibility.Hidden;
                                    btntoolSupprimer.Visibility = Visibility.Hidden;
                                    btntoolModifier.Visibility = Visibility.Hidden;
                                    OutilEF.brycolContexte.lstItems.Remove(ip);
                                    OutilEF.brycolContexte.SaveChanges();
                                }
                            }
                        }
                            
                            
                        }
                    }
                }              
            
        }

        public async void initializeItems()
        {
            imageSelection = null;
            canvas.Children.Clear();
            canvasMur.Children.Clear();
            setCanvasPiece();
            
           
          
            if (Item_VM.ItemsPlanActuel != null)
            {
                Item_VM.ItemsPlanActuel.Clear();
                var ireq = from ip in OutilEF.brycolContexte.lstItems.Include("Plan") where ip.Plan.Piece.ID == Piece_VM.pieceActuel.ID select ip;
                foreach (ItemsPlan i in ireq)
                    Item_VM.ItemsPlanActuel.Add(i);
                foreach (ItemsPlan ip in Item_VM.ItemsPlanActuel)
                {
                    var bitmap = new BitmapImage();
                    var bitmapTop = new BitmapImage();
                    if (ip.Item.Nom.Contains("Porte") || ip.Item.Nom.Contains("Fenêtre"))
                    {     
                        try
                        {
                            bitmap.BeginInit();
                            //bitmap.UriSource = new Uri("pack://application:,,,/images/Items/Top/item" + ip.Item.ID + ip.cotePorte + ".png");
                            bitmap.UriSource = new Uri("pack://application:,,,/images/Items/item" + ip.Item.ID + ".png");
                            bitmap.EndInit();
                        }
                        catch (Exception)
                        {
                            bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri("pack://application:,,,/images/Items/Top/item0.png");
                            bitmap.EndInit();
                        }
                        try
                        {
                            bitmapTop.BeginInit();
                            bitmapTop.UriSource = new Uri("pack://application:,,,/images/Items/Top/item" + ip.Item.ID + ip.cotePorte + ".png");                        
                            bitmapTop.EndInit();
                        }
                        catch (Exception)
                        {
                            bitmapTop = new BitmapImage();
                            bitmapTop.BeginInit();
                            bitmapTop.UriSource = new Uri("pack://application:,,,/images/Items/Top/item0.png");
                            bitmapTop.EndInit();
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
                        catch (Exception)
                        {
                            bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri("pack://application:,,,/images/Items/Top/item0.png");
                            bitmap.EndInit();
                        }
                    }

                        

                    try
                    {
                        if (!ip.Item.Nom.Contains("Porte") && !ip.Item.Nom.Contains("Fenêtre"))
                        {
                            bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            bitmap.UriSource = new Uri("pack://application:,,,/images/ItemsModifies/Item" + ip.Item.ID + "/" + ip.Couleur + ".png");
                            bitmap.EndInit();
                        }
                    }
                    catch (Exception)
                    {
                        try
                        { 
                            bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri("pack://application:,,,/images/Items/Top/item" + ip.Item.ID + ".png");
                            bitmap.EndInit();
                        }
                        catch(Exception)
                        {
                            bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri("pack://application:,,,/images/Items/Top/item0.png");
                            bitmap.EndInit();
                        }
                    }
                    
                    
                    var image = new Image { Source = bitmap };
                    var imageTop = new Image { Source = bitmapTop };
                    Canvas.SetLeft(image, ip.emplacementGauche);
                    Canvas.SetTop(image, ip.emplacementHaut);
                    Canvas.SetLeft(imageTop, ip.emplacementGauche);
                    Canvas.SetTop(imageTop, ip.emplacementHaut);
                    image.Tag = ip.ID;
                    imageTop.Tag = ip.ID;
                    #region angle-----------------------------------------------------------------------------------------------------------------
                    if (ip.angleRotation == 0)
                    {
                        image.RenderTransformOrigin = new Point(0.5, 0.5);
                        image.RenderTransform = new RotateTransform(0);
                        imageTop.RenderTransformOrigin = new Point(0.5, 0.5);
                        imageTop.RenderTransform = new RotateTransform(0);
                    }
                    else if (ip.angleRotation == 90)
                    {
                        image.RenderTransformOrigin = new Point(0.5, 0.5);
                        image.RenderTransform = new RotateTransform(90);
                        imageTop.RenderTransformOrigin = new Point(0.5, 0.5);
                        imageTop.RenderTransform = new RotateTransform(90);
                    }
                    else if (ip.angleRotation == 180)
                    {
                        image.RenderTransformOrigin = new Point(0.5, 0.5);
                        image.RenderTransform = new RotateTransform(180);
                        imageTop.RenderTransformOrigin = new Point(0.5, 0.5);
                        imageTop.RenderTransform = new RotateTransform(90);
                    }
                    else if (ip.angleRotation == 270)
                    {

                        image.RenderTransformOrigin = new Point(0.5, 0.5);
                        image.RenderTransform = new RotateTransform(270);
                        imageTop.RenderTransformOrigin = new Point(0.5, 0.5);
                        imageTop.RenderTransform = new RotateTransform(90);
                    }
                    
                    #endregion
                   
                    image.Height = (ip.Item.Longueur * pixelToCm);
                    image.Width = (ip.Item.Largeur * pixelToCm);
                    imageTop.Height = (ip.Item.Longueur * pixelToCm);
                    imageTop.Width = (ip.Item.Largeur * pixelToCm);
                    int choixMur = 1;
                    if ((ip.Item.Nom.Contains("Porte") || ip.Item.Nom.Contains("Fenêtre")) && ip.mur == 0)
                    {
                        image.Height = image.Height * 3;
                        image.Width = image.Width * 3;
                        popupMur1.IsOpen = true;
                        popupMur2.IsOpen = true;
                        popupMur3.IsOpen = true;
                        popupMur4.IsOpen = true;
                        Task<int> task = new Task<int>(choixMurAttendreAsync);
                        task.Start();
                        choixMur = await task;
                        popupMur1.IsOpen = false;
                        popupMur2.IsOpen = false;
                        popupMur3.IsOpen = false;
                        popupMur4.IsOpen = false;
                        ip.mur = choixMurAttendre;
                        
                        canvasMur.Children.Add(image);
                        canvas.Children.Add(imageTop);
                    }
                    else if (ip.mur != 0)
                    {
                        image.Height = image.Height * 3;
                        image.Width = image.Width * 3;
                        canvasMur.Children.Add(image);
                        canvas.Children.Add(imageTop);
                    }
                    else
                    {
                        canvas.Children.Add(image);
                    }
                   
                    
                    
                }
            }
        }
      
        private int choixMurAttendreAsync()
        {
            while (choixMurAttendre == 0)
            {

            }
            
            return choixMurAttendre;
        }

        private void btnMurChoix1(object sender, RoutedEventArgs e)
        {
            choixMurAttendre = 1;
        }

        private void btnMurChoix2(object sender, RoutedEventArgs e)
        {
            choixMurAttendre = 2;
        }

        private void btnMurChoix3(object sender, RoutedEventArgs e)
        {
            choixMurAttendre = 3;
        }

        private void btnMurChoix4(object sender, RoutedEventArgs e)
        {
            choixMurAttendre = 4;
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
            Plan_VM.zoomPiece = zoom;
            slider1.Value = slider1.Ticks.Select(x => (double?)x).FirstOrDefault(x => x > slider1.Value) ?? slider1.Value;


            canvas.RenderTransform = new ScaleTransform(zoom, zoom); // transforme la grandeur du canvas
          
        }

        private void btnZoomOut(object sender, RoutedEventArgs e)
        {
            zoom = zoom - 0.12;
            if (zoom < zoomMin) { zoom = zoomMin; } // Limite le minimum
            if (zoom > zoomMax) { zoom = zoomMax; } // Limite le maximum
            Plan_VM.zoomPiece = zoom;
            slider1.Value = slider1.Ticks.Select(x => (double?)x).LastOrDefault(x => x < slider1.Value) ?? slider1.Value;

            canvas.RenderTransform = new ScaleTransform(zoom, zoom);  // transforme la grandeur du canvas
        }          

        private void CanvasZoomLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Piece_VM.pieceActuel.Longueur == 0 && Piece_VM.pieceActuel.Largeur == 00)
            {
                start = e.GetPosition(canvas_Zoom);
            }
            
        }       
       
        private void CanvasZoomMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Piece_VM.pieceActuel.Longueur == 0 && Piece_VM.pieceActuel.Largeur == 0)
            {
                canvas_Zoom.Children.Remove(txtLargeur);
                canvas_Zoom.Children.Remove(txtLongueur);
                end = e.GetPosition(canvas_Zoom);
                end.X = end.X - 2;
                ImageBrush imgBrush = new ImageBrush();
                imgBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Items/planche" + Piece_VM.pieceActuel.TypePlancher.Nom + ".jpg"));
                System.Windows.Shapes.Rectangle newRectangle = new System.Windows.Shapes.Rectangle()
                {
                    Stroke = Brushes.Black,
                    Fill = imgBrush,
                    StrokeThickness = 2
                };


                // Update the X & Y as the mouse moves
                if (e.LeftButton == MouseButtonState.Pressed && pieceCreer == false)
                {
                    if (canvas_Zoom.Children.Count > 1)
                    {
                        canvas_Zoom.Children.RemoveRange(1, canvas_Zoom.Children.Count - 1);
                    }
                    if (end.X >= start.X)
                    {
                        // Defines the left part of the rectangle
                        newRectangle.SetValue(Canvas.LeftProperty, start.X);
                        newRectangle.Width = end.X - start.X;
                    }
                    else
                    {
                        newRectangle.SetValue(Canvas.LeftProperty, end.X);
                        newRectangle.Width = start.X - end.X;
                    }

                    if (end.Y >= start.Y)
                    {
                        // Defines the top part of the rectangle
                        newRectangle.SetValue(Canvas.TopProperty, start.Y);
                        newRectangle.Height = end.Y - start.Y;
                    }
                    else
                    {
                        newRectangle.SetValue(Canvas.TopProperty, end.Y);
                        newRectangle.Height = start.Y - end.Y;
                    }


                    double rectangleWidthTemp = newRectangle.Width / pixelToM;

                    if (rectangleWidthTemp % 1 >= 0.75 && rectangleWidthTemp > 1)
                    {
                        rectangleWidthTemp = rectangleWidthTemp + (1 - rectangleWidthTemp % 1);
                    }
                    else if (rectangleWidthTemp % 1 < 0.75 && rectangleWidthTemp % 1 > 0.25 && rectangleWidthTemp > 1)
                    {
                        rectangleWidthTemp = rectangleWidthTemp - (rectangleWidthTemp % 1 - 0.5);
                    }
                    else if (rectangleWidthTemp % 1 < 0.25 && rectangleWidthTemp > 1)
                    {
                        rectangleWidthTemp = rectangleWidthTemp - (rectangleWidthTemp % 1);
                    }
                    newRectangle.Width = rectangleWidthTemp * pixelToM;
                    rectanglePieceWidth = newRectangle.Width;
                    txtLargeur.Text = (newRectangle.Width / pixelToM).ToString("0.00");
                    txtLargeur.FontSize = 17;
                    Canvas.SetLeft(txtLargeur, start.X + 10);
                    Canvas.SetTop(txtLargeur, start.Y - 19);
                    canvas_Zoom.Children.Add(txtLargeur);


                    double rectangleHeightTemp = newRectangle.Height / pixelToM;

                    if (rectangleHeightTemp % 1 >= 0.75 && rectangleHeightTemp > 1)
                    {
                        rectangleHeightTemp = rectangleHeightTemp + (1 - rectangleHeightTemp % 1);
                    }
                    else if (rectangleHeightTemp % 1 < 0.75 && rectangleHeightTemp % 1 > 0.25 && rectangleHeightTemp > 1)
                    {
                        rectangleHeightTemp = rectangleHeightTemp - (rectangleHeightTemp % 1 - 0.5);
                    }
                    else if (rectangleHeightTemp % 1 < 0.25 && rectangleHeightTemp > 1)
                    {
                        rectangleHeightTemp = rectangleHeightTemp - (rectangleHeightTemp % 1);
                    }
                    newRectangle.Height = rectangleHeightTemp * pixelToM;
                    rectanglePieceHeight = newRectangle.Height;
                    txtLongueur.Text = (newRectangle.Height / pixelToM).ToString("0.00");
                    txtLongueur.FontSize = 17;
                    Canvas.SetLeft(txtLongueur, start.X - 19);
                    Canvas.SetTop(txtLongueur, start.Y - 25);
                    canvas_Zoom.Children.Add(txtLongueur);

                    canvas_Zoom.Children.Add(newRectangle);

                }
            }
            if (canvas.Children != null)
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
                                ImageBrush imgBrush2 = new ImageBrush();
                                imgBrush2.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Items/invalideItem.png"));
                                child.OpacityMask = imgBrush2;
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
            if (canvasMur.Children.Count > 4)
            {
                validerPositionMur();
            }
        }

        private void validerPositionMur()
        {
            foreach (UIElement child in canvasMur.Children)
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
                            childTopGauche = child.TransformToAncestor(canvasMur).Transform(new Point(0, 0));
                            childBotDroite = new Point(childTopGauche.X + child.DesiredSize.Width, childTopGauche.Y + child.DesiredSize.Height);
                        }
                        else if (ip.angleRotation == 90)
                        {
                            childTopGaucheFixe = child.TransformToAncestor(canvasMur).Transform(new Point(0, 0));
                            childTopGauche = new Point(childTopGaucheFixe.X - child.DesiredSize.Height, childTopGaucheFixe.Y);
                            childBotDroite = new Point(childTopGaucheFixe.X, childTopGaucheFixe.Y + child.DesiredSize.Width);
                        }
                        else if (ip.angleRotation == 180)
                        {
                            childBotDroite = child.TransformToAncestor(canvasMur).Transform(new Point(0, 0));
                            childTopGauche = new Point(childBotDroite.X - child.DesiredSize.Width, childBotDroite.Y - child.DesiredSize.Height);
                        }
                        else if (ip.angleRotation == 270)
                        {
                            childTopGaucheFixe = child.TransformToAncestor(canvasMur).Transform(new Point(0, 0));
                            childTopGauche = new Point(childTopGaucheFixe.X, childTopGaucheFixe.Y - child.DesiredSize.Width);
                            childBotDroite = new Point(childTopGaucheFixe.X + child.DesiredSize.Height, childTopGaucheFixe.Y);
                        }
                    }

                }
                int canvasMurHeight = 282;
                if (childTopGauche.X > canvasMur.Width || childTopGauche.X < 0 || childTopGauche.Y < 0 || childTopGauche.Y > canvasMurHeight || childBotDroite.X > canvasMur.Width || childBotDroite.X < 0 || childBotDroite.Y < 0 || childBotDroite.Y > canvasMurHeight)
                {
                    if ((childTopGauche.X > canvasMur.Width && childBotDroite.X > canvasMur.Width) || (childTopGauche.Y > canvasMurHeight && childBotDroite.Y > canvasMurHeight) || (childTopGauche.X < 0 && childBotDroite.X < 0) || (childTopGauche.Y < 0 && childBotDroite.Y < 0))
                    {
                        Canvas.SetLeft(child, canvasMur.Width / 2);
                        Canvas.SetTop(child, canvasMur.Height / 2);
                    }
                    else
                    {
                        if (!((Image)child).GetValue(Image.SourceProperty).ToString().Contains("26") &&
                                !((Image)child).GetValue(Image.SourceProperty).ToString().Contains("27") &&
                                !((Image)child).GetValue(Image.SourceProperty).ToString().Contains("28") &&
                                !((Image)child).GetValue(Image.SourceProperty).ToString().Contains("29"))
                        {
                            ImageBrush imgBrush2 = new ImageBrush();
                            imgBrush2.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Items/invalideItem.png"));
                            child.OpacityMask = imgBrush2;
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

        private void CanvasZoomMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Piece_VM.pieceActuel.Longueur == 0 && Piece_VM.pieceActuel.Largeur == 0)
            {
                if (pieceCreer == false)
                {
                    MessageBoxResult resultat;
                    resultat = System.Windows.MessageBox.Show("Voulez-vous vraiment créer une pièce de dimension "+ Convert.ToSingle(rectanglePieceHeight) / pixelToM + " par " + Convert.ToSingle(rectanglePieceWidth) / pixelToM + "?", "Création de la pièce", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (resultat == MessageBoxResult.Yes)
                    {
                        pieceCreer = true;
                        Piece_VM.pieceActuel.Longueur = Convert.ToSingle(rectanglePieceHeight) / pixelToM;
                        Piece_VM.pieceActuel.Largeur = Convert.ToSingle(rectanglePieceWidth) / pixelToM;

                        canvas.Width = rectanglePieceWidth;
                        canvas.Height = rectanglePieceHeight;

                        canvas_Zoom.Children.RemoveRange(1, 3);



                        setCanvasPiece();
                    }
                    else
                    {

                        canvas_Zoom.Children.RemoveRange(1, 3);
                    }

                }
               
            }
        }

        private void EnleverThemeSombre()
        {
            btnClipPieceDeclipper.Background = Brushes.White;
            btnClipPieceDeclipper.Foreground = Brushes.Black;

            btnPieceRotation.Background = Brushes.White;
            btnPieceRotation.Foreground = Brushes.Black;

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

            btnClipPieceDeclipper.Background = CouleurBouton;
            btnClipPieceDeclipper.Foreground = Brushes.White;

            btnPieceRotation.Background = CouleurBouton;
            btnPieceRotation.Foreground = Brushes.White;

            btnClipPiece.Background = CouleurBouton;
            btnClipPiece.Foreground = Brushes.White;

            btnEchelle.Background = CouleurBouton;
            btnEchelle.Foreground = Brushes.White;

            btnmoins.Background = CouleurBouton;
            btnmoins.Foreground = Brushes.White;

            btnPlus.Background = CouleurBouton;
            btnPlus.Foreground = Brushes.White;
        }

        private void btnClip(object sender, RoutedEventArgs e)
        {
            Plan_VM.clip = true;
            movePiece = false;
            rotationPiece = 0;
            canvas.RenderTransform = new RotateTransform(rotationPiece);
            btnPieceRotation.Visibility = Visibility.Hidden;
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
            Plan_VM.clip = false;
            btnPieceRotation.Visibility = Visibility.Visible;
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

        private void ClipperItemSurMur(ItemsPlan i)
        {



            OutilEF.brycolContexte.SaveChanges();
        }
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
            Plan_VM.rotationPieceVM = rotationPiece;
            canvas.RenderTransformOrigin = new Point(0.5, 0.5);

            canvas.RenderTransform = new RotateTransform(rotationPiece);
        }

        private void HandleKeyPress(object sender, System.Windows.Input.KeyEventArgs e)
        {
            
            if (imageSelection != null)
            {              
                if (e.Key == Key.Delete)
                {
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
                                if (imageBD.Tag.ToString() == imageSelection.Tag.ToString())
                                {
                                    Item_VM.ItemsPlanActuel.Remove(ip);
                                    imageSelection.Source = null;
                                    imageSelection = null;
                                    btntoolRotation.Visibility = Visibility.Hidden;
                                    btntoolSupprimer.Visibility = Visibility.Hidden;
                                    btntoolModifier.Visibility = Visibility.Hidden;
                                    OutilEF.brycolContexte.lstItems.Remove(ip);
                                    OutilEF.brycolContexte.SaveChanges();

                                    return;
                                }
                            }
                        }
                    }
                }
            
                if (Keyboard.IsKeyDown(Key.M)) 
                {
                   
                   popupItem.IsOpen = false;
                    if (itemSelectionee != null)
                    {
                        foreach (Image item in itemSelectionee)
                        {
                            if (imageSelection == item)
                            {
                                foreach (ItemsPlan i in Item_VM.ItemsPlanActuel)
                                {
                                    var bitmap = new BitmapImage(i.Item.ImgItem.UriSource);
                                    var imageBD = new Image { Source = bitmap };
                                    imageBD.Tag = i.ID;

                                    if (imageBD.Tag.ToString() == imageSelection.Tag.ToString())
                                    {
                                        popupText.Text = i.Item.Nom + " est déjà ajouté à la sélection multiple!";
                                        popupText.Foreground = Brushes.Red;
                                        popupItem.IsOpen = true;
                                    }
                                }
                                return;
                            }
                        }
                    }
                    itemSelectionee.Add(imageSelection);

                    foreach (ItemsPlan i in Item_VM.ItemsPlanActuel)
                    {
                        var bitmap = new BitmapImage(i.Item.ImgItem.UriSource);
                        var imageBD = new Image { Source = bitmap };
                        imageBD.Tag = i.ID;

                        if (imageBD.Tag.ToString() == imageSelection.Tag.ToString())
                        {
                            popupText.Text = i.Item.Nom + " est sélectionné";
                            popupText.Foreground = Brushes.Black;
                        }
                    }


                    popupItem.IsOpen = true;
                    return;
                    
                    
                   
                }
                return;
            }
            return;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.KeyDown += HandleKeyPress;
        }

        private void CanvasMurMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var image = e.Source as System.Windows.Controls.Image;
          
                clickPosition = e.GetPosition(canvasMur); // avoir la position du click


                if (image != null && canvas.CaptureMouse())
                {
                    canvasMur.Children.Remove(btntoolMurRotation);
                    canvasMur.Children.Remove(btntoolMurSupprimer);
                    canvasMur.Children.Remove(btntoolMurModifier);
                    draggedImage = image;
                    imageSelection = image;


                    foreach (ItemsPlan i in Item_VM.ItemsPlanActuel)
                    {
                        var bitmap = new BitmapImage(i.Item.ImgItem.UriSource);

                        var imageBD = new Image { Source = bitmap };
                        imageBD.Tag = i.ID;

                        if (imageBD.Tag.ToString() == draggedImage.Tag.ToString())
                        {
                            if (i.emplacementHaut > 19)
                            {
                                Canvas.SetLeft(btntoolMurRotation, i.emplacementGauche + 5);
                                Canvas.SetTop(btntoolMurRotation, i.emplacementHaut - 19);
                                canvasMur.Children.Add(btntoolMurRotation);
                                Canvas.SetLeft(btntoolMurSupprimer, i.emplacementGauche + 64);
                                Canvas.SetTop(btntoolMurSupprimer, i.emplacementHaut - 19);
                                canvasMur.Children.Add(btntoolMurSupprimer);
                                Canvas.SetLeft(btntoolMurModifier, i.emplacementGauche + 124);
                                Canvas.SetTop(btntoolMurModifier, i.emplacementHaut - 19);
                                canvasMur.Children.Add(btntoolMurModifier);
                            }
                            else
                            {
                                Canvas.SetLeft(btntoolMurRotation, i.emplacementGauche + 5);
                                Canvas.SetTop(btntoolMurRotation, i.emplacementHaut + i.Item.Longueur - 15);
                                canvasMur.Children.Add(btntoolMurRotation);
                                Canvas.SetLeft(btntoolMurSupprimer, i.emplacementGauche + 64);
                                Canvas.SetTop(btntoolMurSupprimer, i.emplacementHaut + i.Item.Longueur - 15);
                                canvasMur.Children.Add(btntoolMurSupprimer);
                                Canvas.SetLeft(btntoolMurModifier, i.emplacementGauche + 124);
                                Canvas.SetTop(btntoolMurModifier, i.emplacementHaut + i.Item.Longueur - 15);
                                canvasMur.Children.Add(btntoolMurModifier);
                            }
                        }
                    }
                    btntoolMurRotation.Visibility = Visibility.Visible;
                    btntoolMurSupprimer.Visibility = Visibility.Visible;
                    btntoolMurModifier.Visibility = Visibility.Visible;
                    move = true;
                    mousePosition = e.GetPosition(canvasMur);
                    draggedImage = image;
                    toolbarImage = draggedImage;

                System.Windows.Controls.Panel.SetZIndex(draggedImage, 1); // si on a plusieurs images

                }
            
        }

        private void CanvasMurMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            position = e.GetPosition(canvasMur);
            var offset = position - mousePosition;

            if (draggedImage != null && move == true)
            {
                canvasMur.ReleaseMouseCapture();
                System.Windows.Controls.Panel.SetZIndex(draggedImage, 0);


                foreach (ItemsPlan i in Item_VM.ItemsPlanActuel)
                {
                    var bitmap = new BitmapImage(i.Item.ImgItem.UriSource);

                    var imageBD = new Image { Source = bitmap };
                    imageBD.Tag = i.ID;

                    if (imageBD.Tag.ToString() == draggedImage.Tag.ToString())
                    {                       
                        i.emplacementGauche = Canvas.GetLeft(draggedImage) + offset.X;
                        i.emplacementHaut = Canvas.GetTop(draggedImage) + offset.Y;

                        if (i.emplacementHaut > 19)
                        {
                            Canvas.SetLeft(btntoolMurRotation, i.emplacementGauche + 5);
                            Canvas.SetTop(btntoolMurRotation, i.emplacementHaut - 19);

                            Canvas.SetLeft(btntoolMurSupprimer, i.emplacementGauche + 64);
                            Canvas.SetTop(btntoolMurSupprimer, i.emplacementHaut - 19);

                            Canvas.SetLeft(btntoolMurModifier, i.emplacementGauche + 124);
                            Canvas.SetTop(btntoolMurModifier, i.emplacementHaut - 19);

                        }
                        else
                        {
                            Canvas.SetLeft(btntoolMurRotation, i.emplacementGauche + 5);
                            Canvas.SetTop(btntoolMurRotation, i.emplacementHaut + i.Item.Longueur - 15);

                            Canvas.SetLeft(btntoolMurSupprimer, i.emplacementGauche + 64);
                            Canvas.SetTop(btntoolMurSupprimer, i.emplacementHaut + i.Item.Longueur - 15);

                            Canvas.SetLeft(btntoolMurModifier, i.emplacementGauche + 124);
                            Canvas.SetTop(btntoolMurModifier, i.emplacementHaut + i.Item.Longueur - 15);

                        }

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

        private void CanvasMurMouseMove(object sender, MouseEventArgs e)
        {
            Plan_VM.canvasPieceLeft = Canvas.GetLeft(canvasMur);
            Plan_VM.canvasPieceTop = Canvas.GetTop(canvasMur);

            if (draggedImage != null && move == true)
            {
                var position = e.GetPosition(canvasMur);
                var offset = position - mousePosition;
                mousePosition = position;

                Canvas.SetLeft(draggedImage, Canvas.GetLeft(draggedImage) + offset.X);
                Canvas.SetTop(draggedImage, Canvas.GetTop(draggedImage) + offset.Y);

                if (itemSelectionee.Count != 0)
                {
                    foreach (Image item in itemSelectionee)
                    {
                        Canvas.SetLeft(item, Canvas.GetLeft(item) + offset.X);
                        Canvas.SetTop(item, Canvas.GetTop(item) + offset.Y);
                    }
                }

            }
            else
            {
                if (e.LeftButton != MouseButtonState.Released)
                {
                    if (movePiece)
                    {
                        Point mousePos = e.GetPosition(canvas_Zoom); // position absolu de la souris
                        Canvas.SetLeft(canvasMur, mousePos.X - clickPosition.X); // bouger le canvas
                        Canvas.SetTop(canvasMur, mousePos.Y - clickPosition.Y);

                    }
                }
            }

        }

        private void CanvasMur_MouseWheel(object sender, MouseWheelEventArgs e)
        {

        }        
    }
}
