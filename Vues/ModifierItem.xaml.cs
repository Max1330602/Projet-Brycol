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
            string FichierProjet = "..\\..\\";
            string img = FichierProjet + @"\\images\\Items\\item6.png";

            Bitmap bmp = new Bitmap(img);

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
                        rbmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 255, g, b));

                    else if (cmbCouleur.SelectedItem == RougeFonce)
                        rbmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 139, g, b));

                    else if (cmbCouleur.SelectedItem == VertFonce)
                        rbmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a,r,100,b));

                    else if (cmbCouleur.SelectedItem == Vert)
                        rbmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a, r, 128, b));

                    else if (cmbCouleur.SelectedItem == Bleu)
                        rbmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a, r, g, 255));

                    else if (cmbCouleur.SelectedItem == BleuFonce)
                        rbmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a, r, g, 128));

                    else if (cmbCouleur.SelectedItem == Orange)
                        rbmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 255, 165, b));

                    else if (cmbCouleur.SelectedItem == OrangeFonce)
                        rbmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 255, 140, b));

                    else if (cmbCouleur.SelectedItem == Jaune)
                        rbmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 255, 255, b));

                    else if (cmbCouleur.SelectedItem == Violet)
                        rbmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 238, 130, 238));

                    else if (cmbCouleur.SelectedItem == Mauve)
                        rbmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 128, g, 128));

                    else if (cmbCouleur.SelectedItem == Brun)
                        rbmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 139, 69, 19));
                }
            }
            rbmp.Save(FichierProjet + @"images\\ItemsModifies\\item6Modifie.png");

            BitmapImage bmiItem = new BitmapImage();
            bmiItem.BeginInit();
            bmiItem.CacheOption = BitmapCacheOption.OnLoad;
            bmiItem.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bmiItem.UriSource = new Uri(FichierProjet + @"images\\ItemsModifies\\item6Modifie.png", UriKind.Relative);
            bmiItem.EndInit();
            imgItem.Source = bmiItem;

        }
    }
}
