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

            if (Piece2D.draggedImage.Source != null)
                imgItem.Source = Piece2D.draggedImage.Source;
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAppliquer_Click(object sender, RoutedEventArgs e)
        {

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

            Bitmap rbmp = new Bitmap(bmp);

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
                        rbmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 255, g, b));
                    }

                    else if (cmbCouleur.SelectedItem == RougeFonce)
                    {
                        rbmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 139, g, b));
                    }

                    else if (cmbCouleur.SelectedItem == VertFonce)
                    {
                        rbmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a, r, 100, b));
                    }

                    else if (cmbCouleur.SelectedItem == Vert)
                    {
                        rbmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a, r, 128, b));
                    }

                    else if (cmbCouleur.SelectedItem == Bleu)
                    {
                        rbmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a, r, g, 255));
                    }

                    else if (cmbCouleur.SelectedItem == BleuFonce)
                    {
                        rbmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a, r, g, 128));
                    }

                    else if (cmbCouleur.SelectedItem == Orange)
                    {
                        rbmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 255, 165, b));
                    }

                    else if (cmbCouleur.SelectedItem == OrangeFonce)
                    {
                        rbmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 255, 140, b));
                    }

                    else if (cmbCouleur.SelectedItem == Jaune)
                    {
                        rbmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 255, 255, b));
                    }

                    else if (cmbCouleur.SelectedItem == Violet)
                    {
                        rbmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 238, 130, 238));
                    }

                    else if (cmbCouleur.SelectedItem == Mauve)
                    {
                        rbmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 128, g, 128));
                    }

                    else if (cmbCouleur.SelectedItem == Brun)
                    {
                        rbmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 139, 69, 19));
                    }
                }
            }
            string bitmapSourceSave = bitmapSource.Replace("Items\\Top", "ItemsModifies");

            rbmp.Save(bitmapSourceSave, ImageFormat.Png);

            BitmapImage bmiItem = new BitmapImage();
            bmiItem.BeginInit();
            bmiItem.CacheOption = BitmapCacheOption.OnLoad;
            bmiItem.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            //var bmiItem = new BitmapImage(new Uri("pack://application:,,,/images/Items/Top/item" + ip.Item.ID + ".png"));
            bmiItem.UriSource = new Uri(bitmapSourceSave, UriKind.Relative);
            bmiItem.EndInit();
            imgItem.Source = bmiItem;
        }
    }
}
