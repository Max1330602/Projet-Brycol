﻿using App_Brycol.Modele;
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
using System.Windows.Controls.Primitives;
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
          
            DataContext = new Plan_VM();      
        }

        #region attributs--------------------------------------------------------------------------------------------------------------------------------
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
        private Double zoomMax;
        private Double zoomMin;
       // private Double zoomSpeed = 0.001;
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

            if (Piece_VM.pieceActuel.Longueur == 0 && Piece_VM.pieceActuel.Largeur == 0)
            {
                Canvas.SetLeft(canvas_Zoom, 5);
                Canvas.SetTop(canvas_Zoom, 5);
                
                canvas_Zoom.RenderTransform = new ScaleTransform(1.3, 1.3); // transforme la grandeur du canvas               
            }
            else if (!Plan_VM.catalogueClick)
            {
                Plan_VM.sliderPiece = 50;
                Canvas.SetLeft(canvas, 10);
                Canvas.SetTop(canvas, 10);             
                if (Plan_VM.uniteDeMesure == "Mètres")
                {
                    Plan_VM.zoomPiece = 10 / Piece_VM.pieceActuel.Longueur;
                    Plan_VM.zoomDefault = 10 / Piece_VM.pieceActuel.Longueur;
                    Plan_VM.zoomMax = Plan_VM.zoomPiece+2.4;
                    Plan_VM.zoomMin = Plan_VM.zoomPiece-2.4;

                }
                else
                {
                    Plan_VM.zoomPiece = 32.81 / Piece_VM.pieceActuel.Longueur;
                    Plan_VM.zoomDefault = 32.81 / Piece_VM.pieceActuel.Longueur;
                    Plan_VM.zoomMax = Plan_VM.zoomPiece + 2.4;
                    Plan_VM.zoomMin = Plan_VM.zoomPiece - 2.4;

                }
                canvas_Zoom.RenderTransform = new ScaleTransform(Plan_VM.zoomPiece, Plan_VM.zoomPiece); // transforme la grandeur du canvas  
                montrerClip();
            }             
            else
            {       
                if (Plan_VM.clip)
                {
                    movePiece = false;
                    montrerClip();
                    Canvas.SetLeft(canvas, 10);
                    Canvas.SetTop(canvas, 10);
                  
                }
                else
                {
                    movePiece = true;
                    montrerClip();
                    cacherClip();
                    canvas.RenderTransformOrigin = new Point(0.5, 0.5);
                    canvas.RenderTransform = new RotateTransform(Plan_VM.rotationPieceVM);
                    Canvas.SetLeft(canvas, Plan_VM.canvasPieceLeft);
                    Canvas.SetTop(canvas, Plan_VM.canvasPieceTop);
                }
                slider1.Value = Plan_VM.sliderPiece;
                //canvas_Zoom.RenderTransform = new ScaleTransform(Plan_VM.zoomPiece, Plan_VM.zoomPiece); // transforme la grandeur du canvas 

            }
        }

        private void cacherClip()
        {
            btnPieceRotation.Visibility = Visibility.Visible;                       
            btnClipPiece.Visibility = Visibility.Visible;
            btnClipPieceDeclipper.Visibility = Visibility.Hidden;
        }

        private void montrerClip()
        {
            if (Plan_VM.PlanActuel.Piece.UniteDeMesure == "Mètres")
            {
                for (int i = 0; i <= Plan_VM.PlanActuel.Piece.Largeur; i++)
                {
                    Label txtb = new Label();
                    txtb.Height = 30;
                    txtb.BorderBrush = Brushes.White;
                    txtb.FontSize = 12;
                    txtb.FontFamily = new FontFamily("Century Gothic");
                    txtb.Content = i.ToString();
                    Canvas.SetTop(txtb, -23);
                    Canvas.SetLeft(txtb, i * pixelToM - 10);
                    canvas.Children.Add(txtb);
                }
                for (int i = 1; i <= Plan_VM.PlanActuel.Piece.Longueur; i++)
                {
                    Label txtb = new Label();
                    txtb.Height = 30;
                    txtb.BorderBrush = Brushes.White;
                    txtb.FontFamily = new FontFamily("Century Gothic");
                    txtb.FontSize = 12;
                    txtb.Content = i.ToString();
                    Canvas.SetTop(txtb, i * pixelToM - 23);
                    Canvas.SetLeft(txtb, -17);
                    canvas.Children.Add(txtb);
                }
            }
            else
            {
                for (int i = 0; i <= Plan_VM.PlanActuel.Piece.Largeur; i++)
                {
                    if (i % 2 == 0)
                    {
                        Label txtb = new Label();
                        txtb.Height = 30;
                        txtb.BorderBrush = Brushes.White;
                        txtb.FontSize = 12;
                        txtb.FontFamily = new FontFamily("Century Gothic");
                        txtb.Content = i.ToString();
                        Canvas.SetTop(txtb, -23);
                        Canvas.SetLeft(txtb, i * pixelToPied - 10);
                        canvas.Children.Add(txtb);
                    }
                }
                for (int i = 1; i <= Plan_VM.PlanActuel.Piece.Longueur; i++)
                {
                    if (i % 2 == 0)
                    {
                        Label txtb = new Label();
                        txtb.Height = 30;
                        txtb.BorderBrush = Brushes.White;
                        txtb.FontSize = 12;
                        txtb.FontFamily = new FontFamily("Century Gothic");
                        txtb.Content = i.ToString();
                        Canvas.SetTop(txtb, i * pixelToPied - 23);
                        Canvas.SetLeft(txtb, -17);
                        if (i >= 10)
                        {
                            Canvas.SetLeft(txtb, -23);
                        }

                        canvas.Children.Add(txtb);
                    }
                }
            }

            
            btnClipPiece.Visibility = Visibility.Hidden;
            btnClipPieceDeclipper.Visibility = Visibility.Visible;

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
                    btntoolDeselection.Visibility = Visibility.Hidden;
                    btntoolMurRotation.Visibility = Visibility.Hidden;
                    btntoolMurSupprimer.Visibility = Visibility.Hidden;
                    btntoolMurModifier.Visibility = Visibility.Hidden;
                    btntoolMurDeselection.Visibility = Visibility.Hidden;
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
                    popupMurText.Visibility = Visibility.Visible;
                    popupMur.IsOpen = true;

                }
                else if (clickPosition.X < 5)
                {
                    btntoolRotation.Visibility = Visibility.Hidden;
                    btntoolSupprimer.Visibility = Visibility.Hidden;
                    btntoolModifier.Visibility = Visibility.Hidden;
                    btntoolDeselection.Visibility = Visibility.Hidden;
                    btntoolMurRotation.Visibility = Visibility.Hidden;
                    btntoolMurSupprimer.Visibility = Visibility.Hidden;
                    btntoolMurModifier.Visibility = Visibility.Hidden;
                    btntoolMurDeselection.Visibility = Visibility.Hidden;
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
                    if (Plan_VM.PlanActuel.Piece.UniteDeMesure == "Mètres")
                    {
                        canvasMur.Width = Plan_VM.PlanActuel.Piece.Longueur * pixelToM;

                    }
                    else
                    {
                        canvasMur.Width = Plan_VM.PlanActuel.Piece.Longueur * pixelToPied;

                    }
                    canvasMur.Visibility = Visibility.Visible;
                    popupMurText.Visibility = Visibility.Visible;
                    popupMur.IsOpen = true;
                }
                else
                {
                    canvasMur.Visibility = Visibility.Hidden;
                    btntoolMurRotation.Visibility = Visibility.Hidden;
                    btntoolMurSupprimer.Visibility = Visibility.Hidden;
                    btntoolMurModifier.Visibility = Visibility.Hidden;
                    btntoolMurDeselection.Visibility = Visibility.Hidden;
                    btntoolRotation.Visibility = Visibility.Hidden;
                    btntoolSupprimer.Visibility = Visibility.Hidden;
                    btntoolModifier.Visibility = Visibility.Hidden;
                    btntoolDeselection.Visibility = Visibility.Hidden;
                    popupMur.IsOpen = false;
                }
                if (Plan_VM.PlanActuel.Piece.UniteDeMesure == "Mètres")
                {
                   
                   if (clickPosition.Y > (Plan_VM.PlanActuel.Piece.Longueur * pixelToM) - 5)
                    {
                        btntoolRotation.Visibility = Visibility.Hidden;
                        btntoolSupprimer.Visibility = Visibility.Hidden;
                        btntoolModifier.Visibility = Visibility.Hidden;
                        btntoolDeselection.Visibility = Visibility.Hidden;
                        btntoolMurRotation.Visibility = Visibility.Hidden;
                        btntoolMurSupprimer.Visibility = Visibility.Hidden;
                        btntoolMurModifier.Visibility = Visibility.Hidden;
                        btntoolMurDeselection.Visibility = Visibility.Hidden;
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
                        if (Plan_VM.PlanActuel.Piece.UniteDeMesure == "Mètres")
                        {
                            canvasMur.Width = Plan_VM.PlanActuel.Piece.Largeur * pixelToM;

                        }
                        else
                        {
                            canvasMur.Width = Plan_VM.PlanActuel.Piece.Largeur * pixelToPied;

                        }
                        canvasMur.Visibility = Visibility.Visible;
                        popupMurText.Visibility = Visibility.Visible;
                        popupMur.IsOpen = true;
                    }
                    else if (clickPosition.X > (Plan_VM.PlanActuel.Piece.Largeur * pixelToM) - 5)
                    {
                        btntoolRotation.Visibility = Visibility.Hidden;
                        btntoolSupprimer.Visibility = Visibility.Hidden;
                        btntoolModifier.Visibility = Visibility.Hidden;
                        btntoolDeselection.Visibility = Visibility.Hidden;
                        btntoolMurRotation.Visibility = Visibility.Hidden;
                        btntoolMurSupprimer.Visibility = Visibility.Hidden;
                        btntoolMurModifier.Visibility = Visibility.Hidden;
                        btntoolMurDeselection.Visibility = Visibility.Hidden;
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
                        if (Plan_VM.PlanActuel.Piece.UniteDeMesure == "Mètres")
                        {
                            canvasMur.Width = Plan_VM.PlanActuel.Piece.Longueur * pixelToM;

                        }
                        else
                        {
                            canvasMur.Width = Plan_VM.PlanActuel.Piece.Longueur * pixelToPied;

                        }
                        canvasMur.Visibility = Visibility.Visible;
                        popupMurText.Visibility = Visibility.Visible;
                        popupMur.IsOpen = true;
                    }
                }
                else
                {
                    if (clickPosition.Y > (Plan_VM.PlanActuel.Piece.Longueur * pixelToPied) - 5)
                    {
                        btntoolRotation.Visibility = Visibility.Hidden;
                        btntoolSupprimer.Visibility = Visibility.Hidden;
                        btntoolModifier.Visibility = Visibility.Hidden;
                        btntoolDeselection.Visibility = Visibility.Hidden;
                        btntoolMurRotation.Visibility = Visibility.Hidden;
                        btntoolMurSupprimer.Visibility = Visibility.Hidden;
                        btntoolMurModifier.Visibility = Visibility.Hidden;
                        btntoolMurDeselection.Visibility = Visibility.Hidden;
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
                        if (Plan_VM.PlanActuel.Piece.UniteDeMesure == "Mètres")
                        {
                            canvasMur.Width = Plan_VM.PlanActuel.Piece.Largeur * pixelToM;

                        }
                        else
                        {
                            canvasMur.Width = Plan_VM.PlanActuel.Piece.Largeur * pixelToPied;

                        }
                        canvasMur.Visibility = Visibility.Visible;
                        popupMurText.Visibility = Visibility.Visible;
                        popupMur.IsOpen = true;
                    }
                    else if (clickPosition.X > (Plan_VM.PlanActuel.Piece.Largeur * pixelToPied) - 5)
                    {
                        btntoolRotation.Visibility = Visibility.Hidden;
                        btntoolSupprimer.Visibility = Visibility.Hidden;
                        btntoolModifier.Visibility = Visibility.Hidden;
                        btntoolDeselection.Visibility = Visibility.Hidden;
                        btntoolMurRotation.Visibility = Visibility.Hidden;
                        btntoolMurSupprimer.Visibility = Visibility.Hidden;
                        btntoolMurModifier.Visibility = Visibility.Hidden;
                        btntoolMurDeselection.Visibility = Visibility.Hidden;
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
                        if (Plan_VM.PlanActuel.Piece.UniteDeMesure == "Mètres")
                        {
                            canvasMur.Width = Plan_VM.PlanActuel.Piece.Longueur * pixelToM;

                        }
                        else
                        {
                            canvasMur.Width = Plan_VM.PlanActuel.Piece.Longueur * pixelToPied;

                        }
                        canvasMur.Visibility = Visibility.Visible;
                        popupMurText.Visibility = Visibility.Visible;
                        popupMur.IsOpen = true;
                    }
                }
  
                #endregion

                if (image != null && canvas.CaptureMouse())
                {
                    canvas.Children.Remove(btntoolRotation);
                    canvas.Children.Remove(btntoolSupprimer);
                    canvas.Children.Remove(btntoolModifier);
                    canvas.Children.Remove(btntoolDeselection);
           
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
                                Canvas.SetLeft(btntoolDeselection, i.emplacementGauche + 280);
                                Canvas.SetTop(btntoolDeselection, i.emplacementHaut + 45);
                                canvas.Children.Add(btntoolDeselection);
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
                                    Canvas.SetLeft(btntoolDeselection, i.emplacementGauche + 180);
                                    Canvas.SetTop(btntoolDeselection, i.emplacementHaut - 19);
                                    canvas.Children.Add(btntoolDeselection);
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
                                    Canvas.SetLeft(btntoolDeselection, i.emplacementGauche + 180);
                                    Canvas.SetTop(btntoolDeselection, i.emplacementHaut + i.Item.Longueur - 15);
                                    canvas.Children.Add(btntoolDeselection);
                                }
                                
                            }
                        }
                    }
                    btntoolRotation.Visibility = Visibility.Visible;
                    btntoolSupprimer.Visibility = Visibility.Visible;
                    btntoolModifier.Visibility = Visibility.Visible;
                    btntoolDeselection.Visibility = Visibility.Visible;
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
                                i.emplacementGaucheTOP = Canvas.GetLeft(image) + offset.X;
                                i.emplacementHautTOP = Canvas.GetTop(image) + offset.Y;
                                RotateTransform rotationTOP = image.RenderTransform as RotateTransform;
                                if (rotationTOP != null)
                                {

                                    i.angleRotationTOP = rotationTOP.Angle;
                                }
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

                            Canvas.SetLeft(btntoolDeselection, i.emplacementGauche + 280);
                            Canvas.SetTop(btntoolDeselection, i.emplacementHaut + 45);
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

                                    Canvas.SetLeft(btntoolDeselection, i.emplacementGauche + 180);
                                    Canvas.SetTop(btntoolDeselection, i.emplacementHaut - 19);
                            }
                                else
                                {
                                    Canvas.SetLeft(btntoolRotation, i.emplacementGauche + 5);
                                    Canvas.SetTop(btntoolRotation, i.emplacementHaut + i.Item.Longueur - 15);
                               
                                    Canvas.SetLeft(btntoolSupprimer, i.emplacementGauche + 64);
                                    Canvas.SetTop(btntoolSupprimer, i.emplacementHaut + i.Item.Longueur - 15);

                                    Canvas.SetLeft(btntoolModifier, i.emplacementGauche + 124);
                                    Canvas.SetTop(btntoolModifier, i.emplacementHaut + i.Item.Longueur - 15);

                                    Canvas.SetLeft(btntoolDeselection, i.emplacementGauche + 180);
                                    Canvas.SetTop(btntoolDeselection, i.emplacementHaut + i.Item.Longueur - 15);
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
                Canvas.SetLeft(image, i.emplacementGauche + 150);
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

                    /*if (itemSelectionee.Count != 0)
                    {
                        foreach (Image item in itemSelectionee)
                        {
                            Canvas.SetLeft(item, Canvas.GetLeft(item) + offset.X);
                            Canvas.SetTop(item, Canvas.GetTop(item) + offset.Y);
                        }
                    }*/

               
                    
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

        // Zoom avec la roue à souris
        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //Plan_VM.zoomPiece += zoomSpeed * e.Delta; // Ajuste la vitesse du zoom (e.Delta = Souris roue à zoom )
            if (Plan_VM.zoomPiece < Plan_VM.zoomMin) { Plan_VM.zoomPiece = Plan_VM.zoomMin; } // Limite le minimum
            if (Plan_VM.zoomPiece > Plan_VM.zoomMax) { Plan_VM.zoomPiece = Plan_VM.zoomMax; } // Limite le maximum

            Point mousePos = e.GetPosition(canvas);
            if (e.Delta < 0)
            {
                slider1.Value = slider1.Ticks.Select(x => (double?)x).LastOrDefault(x => x < slider1.Value) ?? slider1.Value;
                if (Plan_VM.sliderPiece >= 5)
                {
                    Plan_VM.sliderPiece = Plan_VM.sliderPiece - 5;
                }
            }
            else
            {
                if (Plan_VM.sliderPiece <= 95)
                {
                    Plan_VM.sliderPiece = Plan_VM.sliderPiece + 5;
                }
                slider1.Value = slider1.Ticks.Select(x => (double?)x).FirstOrDefault(x => x > slider1.Value) ?? slider1.Value;
            }
            // Plan_VM.zoomPiece = zoom;
           /* if (movePiece)
            {
                canvas_Zoom.RenderTransform = new ScaleTransform(Plan_VM.zoomPiece, Plan_VM.zoomPiece, mousePos.X, mousePos.Y); // transforme la grandeur du canvas selon la position de la souris
            }
            else
            {
                canvas_Zoom.RenderTransform = new ScaleTransform(Plan_VM.zoomPiece, Plan_VM.zoomPiece); //transforme la grandeur du canvas si la pièce est clip
            }*/
                

            
           
        }
        
        private void btnDelete(object sender, RoutedEventArgs e)
        {
           
            if (draggedImage != null)
                    draggedImage.Source = null;

                MessageBoxResult resultat;
                if (itemSelectionee.Count >1)
                {
                    resultat = System.Windows.MessageBox.Show("Voulez-vous vraiment supprimer ces items", "Suppression des items", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                }
                else
                {
                    resultat = System.Windows.MessageBox.Show("Voulez-vous vraiment supprimer cet item ?", "Suppression d'un item", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                }
                
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
                                        imageBD.Source = null;
                                        btntoolRotation.Visibility = Visibility.Hidden;
                                        btntoolSupprimer.Visibility = Visibility.Hidden;
                                        btntoolModifier.Visibility = Visibility.Hidden;
                                        btntoolDeselection.Visibility = Visibility.Hidden;
                                    }
                                }
                                foreach (Image image in canvasMur.Children.OfType<Image>())
                                {
                                    // Pour le simple delete
                                    if (imageBD.Tag.ToString() == image.Tag.ToString())
                                    {
                                        Item_VM.ItemsPlanActuel.Remove(ip);
                                        image.Source = null;
                                        btntoolRotation.Visibility = Visibility.Hidden;
                                        btntoolSupprimer.Visibility = Visibility.Hidden;
                                        btntoolModifier.Visibility = Visibility.Hidden;
                                        btntoolDeselection.Visibility = Visibility.Hidden;
                                    }
                                }
                                Item_VM.ItemsPlanActuel.Remove(ip);
                                toolbarImage.Source = null;
                                imageBD.Source = null;
                                btntoolRotation.Visibility = Visibility.Hidden;
                                btntoolSupprimer.Visibility = Visibility.Hidden;
                                btntoolModifier.Visibility = Visibility.Hidden;
                                btntoolDeselection.Visibility = Visibility.Hidden;
                                OutilEF.brycolContexte.lstItems.Remove(ip);
                                OutilEF.brycolContexte.SaveChanges();
                                return;
                            }
                        }
                        else
                        {
                            // Pour la sélection multiple!
                            foreach (Image item in itemSelectionee.ToList())
                            {
                                if (imageBD.Tag.ToString() == item.Tag.ToString())
                                {
                                    foreach (Image image in canvas.Children.OfType<Image>())
                                    {
                                       
                                        if (imageBD.Tag.ToString() == image.Tag.ToString())
                                        {
                                            Item_VM.ItemsPlanActuel.Remove(ip);
                                            image.Source = null;
                                            btntoolRotation.Visibility = Visibility.Hidden;
                                            btntoolSupprimer.Visibility = Visibility.Hidden;
                                            btntoolModifier.Visibility = Visibility.Hidden;
                                            btntoolDeselection.Visibility = Visibility.Hidden;
                                        }
                                    }
                                    itemSelectionee.Remove(item);
                                    Item_VM.ItemsPlanActuel.Remove(ip);
                                    item.Source = null;
                                    btntoolRotation.Visibility = Visibility.Hidden;
                                    btntoolSupprimer.Visibility = Visibility.Hidden;
                                    btntoolModifier.Visibility = Visibility.Hidden;
                                    btntoolDeselection.Visibility = Visibility.Hidden;
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
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;

                            string path = System.Windows.Forms.Application.StartupPath;
                            string pathCorrect = path.Substring(0, path.IndexOf("Brycol")) + "images\\items\\Top\\";
                            bitmap.UriSource = new Uri("..\\..\\images\\Items\\Top\\" + "item" + ip.Item.ID + ".png", UriKind.Relative);
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
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            //string path = System.Windows.Forms.Application.StartupPath;
                            //string pathCorrect = path.Substring(0, path.IndexOf("Brycol")) + "images\\items\\Top\\";
                            bitmap.UriSource = new Uri("..\\..\\images\\Items\\Top\\" + "item" + ip.Item.ID + ".png", UriKind.Relative);
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
                    Canvas.SetLeft(imageTop, ip.emplacementGaucheTOP);
                    Canvas.SetTop(imageTop, ip.emplacementHautTOP);
                    image.Tag = ip.ID;
                    imageTop.Tag = ip.ID;
                    #region angle-----------------------------------------------------------------------------------------------------------------
                    if (ip.angleRotation == 0 || ip.angleRotationTOP == 0)
                    {
                        if (ip.angleRotation == 0)
                        {
                            image.RenderTransformOrigin = new Point(0.5, 0.5);
                            image.RenderTransform = new RotateTransform(0);
                        }
                        if(ip.angleRotationTOP == 0)
                        {
                            imageTop.RenderTransformOrigin = new Point(0.5, 0.5);
                            imageTop.RenderTransform = new RotateTransform(0);
                        }
                        
                        
                    }
                    if (ip.angleRotation == 90 || ip.angleRotationTOP == 90)
                    {
                        if (ip.angleRotation == 90)
                        {
                            image.RenderTransformOrigin = new Point(0.5, 0.5);
                            image.RenderTransform = new RotateTransform(90);
                        }
                        if(ip.angleRotationTOP == 90)
                        {
                            imageTop.RenderTransformOrigin = new Point(0.5, 0.5);
                            imageTop.RenderTransform = new RotateTransform(90);
                        }
                        
                        
                    }
                    if (ip.angleRotation == 180 || ip.angleRotationTOP == 180)
                    {
                        if (ip.angleRotation == 180)
                        {
                            image.RenderTransformOrigin = new Point(0.5, 0.5);
                            image.RenderTransform = new RotateTransform(180);
                        }
                        if(ip.angleRotationTOP == 180)
                        {

                            imageTop.RenderTransformOrigin = new Point(0.5, 0.5);
                            imageTop.RenderTransform = new RotateTransform(180);
                        }
                       
                    }
                    if (ip.angleRotation == 270 || ip.angleRotationTOP == 270)
                    {
                        if (ip.angleRotation == 270)
                        {
                            image.RenderTransformOrigin = new Point(0.5, 0.5);
                            image.RenderTransform = new RotateTransform(270);
                        }
                        if(ip.angleRotationTOP == 270)
                        {
                            imageTop.RenderTransformOrigin = new Point(0.5, 0.5);
                            imageTop.RenderTransform = new RotateTransform(270);
                        }
                        
                        
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
                        //MessageBox.Show("Veuillez sélectionner un mur");
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
            Canvas.SetLeft(canvas, 10);
            Canvas.SetTop(canvas, 10);
            Plan_VM.sliderPiece = 50;
            Plan_VM.zoomPiece = Plan_VM.zoomDefault;
            canvas_Zoom.RenderTransform = new ScaleTransform(Plan_VM.zoomPiece, Plan_VM.zoomPiece); // transforme la grandeur du canvas  
            slider1.Value = 50;
            
                
        }

        private void btnZoomIn(object sender, RoutedEventArgs e)
        {
            //Plan_VM.zoomPiece = Plan_VM.zoomPiece + 0.07;
            if (Plan_VM.zoomPiece < Plan_VM.zoomMin) { Plan_VM.zoomPiece = Plan_VM.zoomMin; } // Limite le minimum
            if (Plan_VM.zoomPiece > Plan_VM.zoomMax) { Plan_VM.zoomPiece = Plan_VM.zoomMax; } // Limite le maximum
          
            slider1.Value = slider1.Ticks.Select(x => (double?)x).FirstOrDefault(x => x > slider1.Value) ?? slider1.Value;
            if (Plan_VM.sliderPiece <= 95)
            {
                Plan_VM.sliderPiece = Plan_VM.sliderPiece + 5;
            }
           // canvas_Zoom.RenderTransform = new ScaleTransform(Plan_VM.zoomPiece, Plan_VM.zoomPiece); // transforme la grandeur du canvas

        }

        private void btnZoomOut(object sender, RoutedEventArgs e)
        {
            //Plan_VM.zoomPiece = Plan_VM.zoomPiece - 0.07;
            if (Plan_VM.zoomPiece < Plan_VM.zoomMin) { Plan_VM.zoomPiece = Plan_VM.zoomMin; } // Limite le minimum
            if (Plan_VM.zoomPiece > Plan_VM.zoomMax) { Plan_VM.zoomPiece = Plan_VM.zoomMax; } // Limite le maximum
       
            slider1.Value = slider1.Ticks.Select(x => (double?)x).LastOrDefault(x => x < slider1.Value) ?? slider1.Value;
            if (Plan_VM.sliderPiece >= 5)
            {
                Plan_VM.sliderPiece = Plan_VM.sliderPiece - 5;
            }
            
            //canvas_Zoom.RenderTransform = new ScaleTransform(Plan_VM.zoomPiece, Plan_VM.zoomPiece);  // transforme la grandeur du canvas
        }          

        private void CanvasZoomLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Piece_VM.pieceActuel.Longueur == 0 && Piece_VM.pieceActuel.Largeur == 0)
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
            if (canvas.Children.OfType<Image>().Count() != 0)
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
                    if (Convert.ToSingle(rectanglePieceHeight) / pixelToM > 30 || Convert.ToSingle(rectanglePieceHeight) / pixelToM < 1 || Convert.ToSingle(rectanglePieceWidth) / pixelToM > 30 || Convert.ToSingle(rectanglePieceWidth) / pixelToM < 1)
                    {
                        MessageBox.Show("Les dimensions ne sont pas valides. (Maximum de 30 mètres et minimum de 1 mètres)");
                        canvas_Zoom.Children.RemoveRange(1, 3);
                        return;
                    }



                    MessageBoxResult resultat;
                    resultat = System.Windows.MessageBox.Show("Voulez-vous vraiment créer une pièce de dimension "+ Convert.ToSingle(rectanglePieceHeight) / pixelToM + " par " + Convert.ToSingle(rectanglePieceWidth) / pixelToM + "?", "Création de la pièce", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (resultat == MessageBoxResult.Yes)
                    {
                        pieceCreer = true;
                        Piece_VM.pieceActuel.Longueur = Convert.ToSingle(rectanglePieceHeight) / pixelToM;
                        Piece_VM.pieceActuel.Largeur = Convert.ToSingle(rectanglePieceWidth) / pixelToM;
                        canvas.Width = rectanglePieceWidth;
                        canvas.Height = rectanglePieceHeight;
                        txtLargeur2.Text = (Convert.ToSingle(rectanglePieceWidth) / pixelToM).ToString();
                        txtLongueur2.Text = (Convert.ToSingle(rectanglePieceHeight) / pixelToM).ToString();
                        txtUnite1.Text = Piece_VM.pieceActuel.UniteDeMesure.ToString();
                        txtUnite2.Text = Piece_VM.pieceActuel.UniteDeMesure.ToString();
                        canvas_Zoom.Children.RemoveRange(1, 3);
                        montrerClip();


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
            Brush CouleurArrierePlan = (Brush)bc.ConvertFrom("#7D7E79");

            canvas_Zoom.Background = CouleurArrierePlan;
            dimensionAffichage.Background = CouleurBouton;
            dimensionAffichage.Foreground = Brushes.White;

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
            foreach (Label item in canvas.Children.OfType<Label>().ToList())
            {
                canvas.Children.Remove(item);
            }
            if (Plan_VM.PlanActuel.Piece.UniteDeMesure == "Mètres")
            {
                for (int i = 0; i <= Plan_VM.PlanActuel.Piece.Largeur; i++)
                {
                    Label txtb = new Label();
                    txtb.Height = 30;
                    txtb.BorderBrush = Brushes.White;
                    txtb.FontSize = 12;
                    txtb.FontFamily = new FontFamily("Century Gothic");
                    txtb.Content = i.ToString();
                    Canvas.SetTop(txtb, -23);
                    Canvas.SetLeft(txtb, i * pixelToM - 10);
                    canvas.Children.Add(txtb);
                }
                for (int i = 1; i <= Plan_VM.PlanActuel.Piece.Longueur; i++)
                {
                    Label txtb = new Label();
                    txtb.Height = 30;
                    txtb.BorderBrush = Brushes.White;
                    txtb.FontFamily = new FontFamily("Century Gothic");
                    txtb.FontSize = 12;
                    txtb.Content = i.ToString();
                    Canvas.SetTop(txtb, i * pixelToM - 23);
                    Canvas.SetLeft(txtb, -17);
                    if (i >= 10)
                    {
                        Canvas.SetLeft(txtb, -23);
                    }
                    canvas.Children.Add(txtb);
                }
            }
            else
            {
                for (int i = 0; i <= Plan_VM.PlanActuel.Piece.Largeur; i++)
                {
                    if (i%2 == 0)
                    {                
                    Label txtb = new Label();
                    txtb.Height = 30;
                    txtb.BorderBrush = Brushes.White;
                    txtb.FontSize = 12;
                    txtb.FontFamily = new FontFamily("Century Gothic");
                    txtb.Content = i.ToString();
                    Canvas.SetTop(txtb, -23);
                    Canvas.SetLeft(txtb, i * pixelToPied - 10);
                    canvas.Children.Add(txtb);
                    }
                }
                for (int i = 1; i <= Plan_VM.PlanActuel.Piece.Longueur; i++)
                {
                    if (i % 2 == 0)
                    {
                        Label txtb = new Label();
                        txtb.Height = 30;
                        txtb.BorderBrush = Brushes.White;
                        txtb.FontSize = 12;
                        txtb.FontFamily = new FontFamily("Century Gothic");
                        txtb.Content = i.ToString();
                        Canvas.SetTop(txtb, i * pixelToPied - 23);
                        Canvas.SetLeft(txtb, -17);
                        if (i >= 10)
                        {
                            Canvas.SetLeft(txtb, -23);
                        }

                        canvas.Children.Add(txtb);
                    }
                }
            }
            
            Canvas.SetLeft(canvas, 10);
            Canvas.SetTop(canvas, 10);

            btnClipPiece.Visibility = Visibility.Hidden;
            btnClipPieceDeclipper.Visibility = Visibility.Visible;           
        }

        private void btnDéclip(object sender, RoutedEventArgs e)
        {
            Plan_VM.clip = false;
            btnPieceRotation.Visibility = Visibility.Visible;
            
            movePiece = true;
            btnClipPiece.Visibility = Visibility.Visible;
            btnClipPieceDeclipper.Visibility = Visibility.Hidden;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double zoomSlider = e.NewValue;
            if(e.OldValue == 0)
            {
                canvas_Zoom.RenderTransform = new ScaleTransform(Plan_VM.zoomPiece, Plan_VM.zoomPiece);
            }
            else if (e.OldValue == 1 && zoomSlider != 5)
            {
                canvas_Zoom.RenderTransform = new ScaleTransform(Plan_VM.zoomPiece, Plan_VM.zoomPiece);
            }
            else if (zoomSlider < e.OldValue )
            {
                Plan_VM.zoomPiece = Plan_VM.zoomPiece - 0.12;
                canvas_Zoom.RenderTransform = new ScaleTransform(Plan_VM.zoomPiece, Plan_VM.zoomPiece);
            }
            else if (zoomSlider > e.OldValue)
            {
                Plan_VM.zoomPiece = Plan_VM.zoomPiece + 0.12;
                canvas_Zoom.RenderTransform = new ScaleTransform(Plan_VM.zoomPiece, Plan_VM.zoomPiece);
  
            }
           

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
                    if (itemSelectionee.Count > 1)
                    {
                        resultat = System.Windows.MessageBox.Show("Voulez-vous vraiment supprimer ces items", "Suppression des items", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    }
                    else
                    {
                        resultat = System.Windows.MessageBox.Show("Voulez-vous vraiment supprimer cet item ?", "Suppression d'un item", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    }

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
                                    foreach (Image item in itemSelectionee.ToList())
                                    {
                                        if (imageBD.Tag.ToString() == item.Tag.ToString())
                                        {
                                            foreach (Image image in canvas.Children.OfType<Image>())
                                            {

                                                if (imageBD.Tag.ToString() == image.Tag.ToString())
                                                {
                                                    Item_VM.ItemsPlanActuel.Remove(ip);
                                                    image.Source = null;
                                                    btntoolRotation.Visibility = Visibility.Hidden;
                                                    btntoolSupprimer.Visibility = Visibility.Hidden;
                                                    btntoolModifier.Visibility = Visibility.Hidden;
                                                }
                                            }
                                            itemSelectionee.Remove(item);
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
                    itemSelectionee.Clear();
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
                    canvasMur.Children.Remove(btntoolMurDeselection);
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
                            Canvas.SetLeft(btntoolMurRotation, i.emplacementGauche + 50);
                            Canvas.SetTop(btntoolMurRotation, i.emplacementHaut - 19);
                            canvasMur.Children.Add(btntoolMurRotation);
                            Canvas.SetLeft(btntoolMurSupprimer, i.emplacementGauche + 109);
                            Canvas.SetTop(btntoolMurSupprimer, i.emplacementHaut - 19);
                            canvasMur.Children.Add(btntoolMurSupprimer);
                            Canvas.SetLeft(btntoolMurModifier, i.emplacementGauche + 167);
                            Canvas.SetTop(btntoolMurModifier, i.emplacementHaut - 19);
                            canvasMur.Children.Add(btntoolMurModifier);
                            Canvas.SetLeft(btntoolMurDeselection, i.emplacementGauche + 223);
                            Canvas.SetTop(btntoolMurDeselection, i.emplacementHaut - 19);
                            canvasMur.Children.Add(btntoolMurDeselection);
                        }
                        else
                        {
                            Canvas.SetLeft(btntoolMurRotation, i.emplacementGauche + 50);
                            Canvas.SetTop(btntoolMurRotation, i.emplacementHaut + i.Item.Longueur - 15);
                            canvasMur.Children.Add(btntoolMurRotation);
                            Canvas.SetLeft(btntoolMurSupprimer, i.emplacementGauche + 109);
                            Canvas.SetTop(btntoolMurSupprimer, i.emplacementHaut + i.Item.Longueur - 15);
                            canvasMur.Children.Add(btntoolMurSupprimer);
                            Canvas.SetLeft(btntoolMurModifier, i.emplacementGauche + 167);
                            Canvas.SetTop(btntoolMurModifier, i.emplacementHaut + i.Item.Longueur - 15);
                            canvasMur.Children.Add(btntoolMurModifier);
                            Canvas.SetLeft(btntoolMurDeselection, i.emplacementGauche + 223);
                            Canvas.SetTop(btntoolMurDeselection, i.emplacementHaut + i.Item.Longueur - 15);
                            canvasMur.Children.Add(btntoolMurDeselection);
                        }
                    }
                    }
                    btntoolMurRotation.Visibility = Visibility.Visible;
                    btntoolMurSupprimer.Visibility = Visibility.Visible;
                    btntoolMurModifier.Visibility = Visibility.Visible;
                    btntoolMurDeselection.Visibility = Visibility.Visible;
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
                            Canvas.SetLeft(btntoolMurRotation, i.emplacementGauche + 50);
                            Canvas.SetTop(btntoolMurRotation, i.emplacementHaut - 19);

                            Canvas.SetLeft(btntoolMurSupprimer, i.emplacementGauche + 109);
                            Canvas.SetTop(btntoolMurSupprimer, i.emplacementHaut - 19);

                            Canvas.SetLeft(btntoolMurModifier, i.emplacementGauche + 167);
                            Canvas.SetTop(btntoolMurModifier, i.emplacementHaut - 19);

                            Canvas.SetLeft(btntoolDeselection, i.emplacementGauche + 223);
                            Canvas.SetTop(btntoolDeselection, i.emplacementHaut - 19);
                        }
                        else
                        {
                            Canvas.SetLeft(btntoolMurRotation, i.emplacementGauche + 50);
                            Canvas.SetTop(btntoolMurRotation, i.emplacementHaut + i.Item.Longueur - 15);

                            Canvas.SetLeft(btntoolMurSupprimer, i.emplacementGauche + 109);
                            Canvas.SetTop(btntoolMurSupprimer, i.emplacementHaut + i.Item.Longueur - 15);

                            Canvas.SetLeft(btntoolMurModifier, i.emplacementGauche + 167);
                            Canvas.SetTop(btntoolMurModifier, i.emplacementHaut + i.Item.Longueur - 15);

                            Canvas.SetLeft(btntoolMurDeselection, i.emplacementGauche + 223);
                            Canvas.SetTop(btntoolMurDeselection, i.emplacementHaut + i.Item.Longueur - 15);
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

        private void click_deselect(object sender, RoutedEventArgs e)
        {
            if (itemSelectionee.Count > 0)
            {
                
                Popup codePopup = new Popup();
                TextBlock popupText = new TextBlock();
                popupText.Text = "Succès";
                popupText.Background = Brushes.LightGreen;
                popupText.Foreground = Brushes.Black;
                popupText.FontSize = 24;
                popupText.FontWeight = FontWeights.ExtraBold;
                popupText.FontFamily = new FontFamily("Times new roman");
                codePopup.Child = popupText;
                codePopup.PopupAnimation = PopupAnimation.Fade;
                codePopup.PlacementTarget = btntoolDeselection;
                codePopup.Placement = PlacementMode.Right;
                codePopup.StaysOpen = false;
                codePopup.IsOpen = true;

            }

            itemSelectionee.Clear();
        }

        private void click_deselectMur(object sender, RoutedEventArgs e)
        {
            if (itemSelectionee.Count > 0)
            {

                Popup codePopup = new Popup();
                TextBlock popupText = new TextBlock();
                popupText.Text = "Succès";
                popupText.Background = Brushes.LightGreen;
                popupText.Foreground = Brushes.Black;
                popupText.FontSize = 24;
                popupText.FontWeight = FontWeights.ExtraBold;
                popupText.FontFamily = new FontFamily("Times new roman");
                codePopup.Child = popupText;
                codePopup.PopupAnimation = PopupAnimation.Fade;
                codePopup.PlacementTarget = btntoolMurDeselection;
                codePopup.Placement = PlacementMode.Right;
                codePopup.StaysOpen = false;
                codePopup.IsOpen = true;

            }

            itemSelectionee.Clear();
        }
    }
}
