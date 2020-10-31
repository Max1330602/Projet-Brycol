using App_Brycol.Modele;
using App_Brycol.VuesModele;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
using System.Windows.Shapes;

namespace App_Brycol.Vues
{
    /// <summary>
    /// Logique d'interaction pour ModifierItem.xaml
    /// </summary>
    public partial class ModifierItem : Window
    {
        public ModifierItem()
        {
            InitializeComponent();

            DataContext = new Item_VM();

            imgItem.Source = Piece2D.draggedImage.Source;
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Piece2D.draggedImage = null;
        }

        private void btnAppliquer_Click(object sender, RoutedEventArgs e)
        {
            Grid gridMW = (Grid)Application.Current.MainWindow.FindName("gridMainWindow");
            ContentPresenter cpMW = (ContentPresenter)Application.Current.MainWindow.FindName("presenteurContenu");
            this.Close();
            gridMW.Children.Clear();
            gridMW.Children.Add(cpMW);
            cpMW.Content = new PlanDeTravail();
        }

        private void cmbCouleur_SelectedChange(object sender, SelectionChangedEventArgs e)
        {
            imgItem.Source = Piece2D.draggedImage.Source;
            string FichierProjet = "..\\..\\";
            string bitmapSourceInit = imgItem.Source.ToString();

            string bitmapSource = bitmapSourceInit.Replace("pack://application:,,,/", FichierProjet);
            bitmapSource = bitmapSource.Replace("/", "\\");

            Bitmap bmp = new Bitmap(bitmapSource);

            // Avoir les dimension de l'image
            int width = bmp.Width;
            int height = bmp.Height;

            Bitmap imageModifie1 = new Bitmap(bmp);
            Bitmap imageModifie2 = new Bitmap(bmp);

            // Tant que toute la hauteur n'a pas été toute parcourue
            for (int y = 0; y < height; y++)
            {
                // Tant que toute la largeur n'a pas été toute parcourue
                for (int x = 0; x < width; x++)
                {
                    System.Drawing.Color p = bmp.GetPixel(x, y);

                    int a = p.A;
                    int r = p.R;
                    int g = p.G;
                    int b = p.B;

                    if (cmbCouleur.SelectedItem == Rouge)
                    {
                        imageModifie1.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 255, g, b));
                        imageModifie2.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 255, g, b));
                    }

                    else if (cmbCouleur.SelectedItem == RougeFonce)
                    {
                        imageModifie1.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 139, g, b));
                        imageModifie2.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 139, g, b));
                    }

                    else if (cmbCouleur.SelectedItem == VertFonce)
                    {
                        imageModifie1.SetPixel(x, y, System.Drawing.Color.FromArgb(a, r, 100, b));
                        imageModifie2.SetPixel(x, y, System.Drawing.Color.FromArgb(a, r, 100, b));
                    }

                    else if (cmbCouleur.SelectedItem == Vert)
                    {
                        imageModifie1.SetPixel(x, y, System.Drawing.Color.FromArgb(a, r, 128, b));
                        imageModifie2.SetPixel(x, y, System.Drawing.Color.FromArgb(a, r, 128, b));
                    }

                    else if (cmbCouleur.SelectedItem == Bleu)
                    {
                        imageModifie1.SetPixel(x, y, System.Drawing.Color.FromArgb(a, r, g, 255));
                        imageModifie2.SetPixel(x, y, System.Drawing.Color.FromArgb(a, r, g, 255));
                    }

                    else if (cmbCouleur.SelectedItem == BleuFonce)
                    {
                        imageModifie1.SetPixel(x, y, System.Drawing.Color.FromArgb(a, r, g, 128));
                        imageModifie2.SetPixel(x, y, System.Drawing.Color.FromArgb(a, r, g, 128));
                    }

                    else if (cmbCouleur.SelectedItem == Orange)
                    {
                        imageModifie1.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 255, 165, b));
                        imageModifie2.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 255, 165, b));
                    }

                    else if (cmbCouleur.SelectedItem == OrangeFonce)
                    {
                        imageModifie1.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 255, 140, b));
                        imageModifie2.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 255, 140, b));
                    }

                    else if (cmbCouleur.SelectedItem == Jaune)
                    {
                        imageModifie1.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 255, 255, b));
                        imageModifie2.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 255, 255, b));
                    }

                    else if (cmbCouleur.SelectedItem == Violet)
                    {
                        imageModifie1.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 238, 130, 238));
                        imageModifie2.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 238, 130, 238));
                    }

                    else if (cmbCouleur.SelectedItem == Mauve)
                    {
                        imageModifie1.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 128, g, 128));
                        imageModifie2.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 128, g, 128));
                    }

                    else if (cmbCouleur.SelectedItem == Brun)
                    {
                        imageModifie1.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 139, 69, 19));
                        imageModifie2.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 139, 69, 19));
                    }
                }
            }
            string bitmapSourceSave = bitmapSource.Replace("Items\\Top", "ItemsModifies");
            string bitmapSourceSave2 = bitmapSourceSave;

            if (bitmapSourceSave2.Contains("(1)"))
                bitmapSourceSave = bitmapSourceSave2.Replace("(1).png", ".png" );
            else
                bitmapSourceSave2 = bitmapSourceSave2.Replace(".png", "(1).png");

            if (ItemsPlan.pathChoisi)
            {
                imageModifie1.Save(bitmapSourceSave, ImageFormat.Png);
                imageModifie1.Dispose();
                BitmapImage bmiItem = new BitmapImage();
                bmiItem.BeginInit();
                bmiItem.CacheOption = BitmapCacheOption.OnLoad;
                bmiItem.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bmiItem.UriSource = new Uri(bitmapSourceSave, UriKind.Relative);
                bmiItem.EndInit();
                imgItem.Source = bmiItem;
            }
            else
            {
                imageModifie2.Save(bitmapSourceSave2, ImageFormat.Png);
                imageModifie2.Dispose();
                BitmapImage bmiItem = new BitmapImage();
                bmiItem.BeginInit();
                bmiItem.CacheOption = BitmapCacheOption.OnLoad;
                bmiItem.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bmiItem.UriSource = new Uri(bitmapSourceSave2, UriKind.Relative);
                bmiItem.EndInit();
                imgItem.Source = bmiItem;
            }


            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Close();
            //Piece2D.draggedImage = null;
        }
    }
}
