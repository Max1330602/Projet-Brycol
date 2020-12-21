using App_Brycol.Modele;
using App_Brycol.Outils;
using App_Brycol.VuesModele;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace App_Brycol.Vues
{
    /// <summary>
    /// Logique d'interaction pour ModifCatalogue.xaml
    /// </summary>
    public partial class ModifCatalogue : Window
    {
        public ModifCatalogue()
        {
            InitializeComponent();
            DataContext = new Item_VM();

            if (Projet_VM.themeSombre)
                AppliquerThemeSombre();
            else
                EnleverThemeSombre();

        }

        private void AppliquerThemeSombre()
        {
            BrushConverter bc = new BrushConverter();
            Brush CouleurBouton = (Brush)bc.ConvertFrom("#45463F");
            Brush CouleurArriere = (Brush)bc.ConvertFrom("#33342F");
            Brush CouleurBanniere = (Brush)bc.ConvertFrom("#84857D");
            Brush CouleurArrierePlan = (Brush)bc.ConvertFrom("#7D7E79");

            Banniere.Background = CouleurBanniere;

            btnAjouter.Background = CouleurBouton;
            btnAjouter.Foreground = Brushes.White;

            btnCharger1.Background = CouleurBouton;
            btnCharger1.Foreground = Brushes.White;
            
            btnCharger2.Background = CouleurBouton;
            btnCharger2.Foreground = Brushes.White;

            btnRetour.Background = CouleurBouton;
            btnRetour.Foreground = Brushes.White;

            grdModifierCatalogue.Background = CouleurBanniere;
            Pied.Background = CouleurArrierePlan;
        }

        private void EnleverThemeSombre()
        {
            Banniere.Background = Brushes.LightGray;

            btnAjouter.Background = Brushes.White;
            btnAjouter.Foreground = Brushes.Black;

            btnCharger1.Background = Brushes.White;
            btnCharger1.Foreground = Brushes.Black;

            btnCharger2.Background = Brushes.White;
            btnCharger2.Foreground = Brushes.Black;

            btnRetour.Background = Brushes.White;
            btnRetour.Foreground = Brushes.Black;

            Pied.Background = Brushes.White;
            grdModifierCatalogue.Background = Brushes.White;
        }

        private void btnCharger1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png) | *.png";

            if (openFileDialog.ShowDialog() == true)
            {
                Bitmap img = new Bitmap(openFileDialog.FileName);
                string path = System.Windows.Forms.Application.StartupPath;
                string pathCorrect = path.Substring(0, path.IndexOf("Brycol")) + "images\\items\\";
                var iReq = from i in OutilEF.brycolContexte.Meubles select i;
                int dernierItemNum = iReq.OrderByDescending(it => it.ID).FirstOrDefault().ID;
                dernierItemNum = dernierItemNum + 1;
                img.Save("..\\..\\images\\Items\\" + "item" + dernierItemNum + ".png", ImageFormat.Png);
                BitmapImage bmiItem = new BitmapImage();
                try
                {
                    bmiItem.BeginInit();
                    bmiItem.CacheOption = BitmapCacheOption.OnLoad;
                    bmiItem.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    bmiItem.UriSource = new Uri("..\\..\\images\\Items\\item" + dernierItemNum + ".png", UriKind.Relative);
                    bmiItem.EndInit();
                    imgCat.Source = bmiItem;
                }
                catch (Exception)
                {
                    MessageBox.Show("Impossible de modifier une image ajoutée au catalogue.");
                    return;
                }
                //imgCat.Source = new BitmapImage(new Uri("..\\..\\images\\Items\\" + "item" + dernierItemNum + ".png", UriKind.Relative));
            }


        }

        private void btnCharger2_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png) | *.png";

            if (openFileDialog.ShowDialog() == true)
            {
                Bitmap img = new Bitmap(openFileDialog.FileName);
                string path = System.Windows.Forms.Application.StartupPath;
                string pathCorrect = path.Substring(0, path.IndexOf("Brycol")) + "images\\items\\Top\\";
                var iReq = from i in OutilEF.brycolContexte.Meubles select i;
                int dernierItemNum = iReq.OrderByDescending(it => it.ID).FirstOrDefault().ID;
                dernierItemNum = dernierItemNum + 1;
                img.Save("..\\..\\images\\Items\\Top\\" + "item" + dernierItemNum + ".png", ImageFormat.Png);
                BitmapImage bmiItem = new BitmapImage();
                try
                {
                    bmiItem.BeginInit();
                    bmiItem.CacheOption = BitmapCacheOption.OnLoad;
                    bmiItem.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    bmiItem.UriSource = new Uri("..\\..\\images\\Items\\Top\\item" + dernierItemNum + ".png", UriKind.Relative);
                    bmiItem.EndInit();
                    imgHaut.Source = bmiItem;
                }
                catch (Exception)
                {
                    MessageBox.Show("Impossible de modifier une image ajoutée au catalogue.");
                    return;
                }
                //imgHaut.Source = new BitmapImage(new Uri("..\\..\\images\\Items\\" + "item" + dernierItemNum + ".png", UriKind.Relative));
            }


        }

        private void btnRetour_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Catalogue popUp = new Catalogue();
            popUp.ShowDialog();
        }

        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            if (cboCategorie.SelectedItem.ToString() == "Tous")
            {
                MessageBox.Show("Vous devez changer la catégorie avant de poursuivre, veuillez corriger cette erreur.");
                cboCategorie.Focus();
                return;

            }
            else if (cboTypePiece.SelectedItem.ToString() == "Tous")
            {
                MessageBox.Show("Vous devez changer le type de pièce avant de poursuivre, veuillez corriger cette erreur.");
                cboTypePiece.Focus();
                return;

            }
            else if (imgCat.Source == null)
            {
                MessageBox.Show("Vous devez charger une image pour le catalogue avant de poursuivre veuillez corriger cette erreur.");
                btnCharger1.Focus();
                return;

            }
            else if (imgHaut.Source == null)
            {
                MessageBox.Show("Vous devez charger une image vue de haut pour le plan avant de poursuivre veuillez corriger cette erreur.");
                btnCharger1.Focus();
                return;

            }
            Item i = new Item();
            try
            {
                var cReq = from c in OutilEF.brycolContexte.Catego where c.Nom == cboCategorie.Text select c;
                i.Categorie = cReq.First();
                var tpReq = from tp in OutilEF.brycolContexte.TypePiece where tp.Nom == cboTypePiece.Text select tp;
                i.TypePiece = tpReq.First();
                i.Nom = txtNom.Text;
                i.Largeur = float.Parse(txtLargeur.Text);
                i.Longueur = float.Parse(txtLongueur.Text);
                i.Hauteur = float.Parse(txtHauteur.Text);
                i.Cout = Decimal.Parse(txtPrix.Text);
                i.Fournisseur = txtFournisseur.Text;
                OutilEF.brycolContexte.Meubles.Add(i);
                OutilEF.brycolContexte.SaveChanges();
                this.Close();
                Catalogue popUp = new Catalogue();
                popUp.ShowDialog();
            }
            catch (Exception)
            { 
                MessageBox.Show("Veuillez remplir tous les champs avant de poursuivre.");
            }
            

            
        }

        private void txtNom_LostFocus(object sender, RoutedEventArgs e)
        { 

            Regex reg = new Regex("^[a-zA-Z0-9]*$");
            if (txtNom.Text != "" && (txtNom.Text.Length < 2 || txtNom.Text.Length > 30))
            {
                MessageBox.Show("La longueur du nom n'est pas valide, veuillez corriger cette erreur.");
                txtNom.Focus();
            }
            else if (!reg.IsMatch(txtNom.Text))
            {
                MessageBox.Show("Un caractère invalide est présent dans le nom, veuillez corriger cette erreur.");
                txtNom.Focus();
            }
        }

        private void txtPrix_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            int number = 1;
            if (txtPrix.Text != "" && !Int32.TryParse(txtPrix.Text,out number))
            {
                MessageBox.Show("Ce champ accepte seulement des chiffres, veuillez corriger cette erreur.");
                txtPrix.Focus();
            }
            else if (number < 0 || number > 1000000)
            {
                MessageBox.Show("La valeur doit être entre 0 et 1000000, veuillez corriger cette erreur.");
                txtPrix.Focus();
            }
        }

        private void txtFournisseur_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Regex reg = new Regex("^[a-zA-Z0-9]*$");
            if (txtFournisseur.Text != "" && (txtFournisseur.Text.Length < 2 || txtFournisseur.Text.Length > 30))
            {
                MessageBox.Show("La longueur du fournisseur n'est pas valide, veuillez corriger cette erreur.");
                txtFournisseur.Focus();
            }
            else if (!reg.IsMatch(txtFournisseur.Text))
            {
                MessageBox.Show("Un caractère invalide est présent dans le nom du fournisseur, veuillez corriger cette erreur.");
                txtFournisseur.Focus();
            }
        }

        private void txtLongueur_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            int number = 2;
            if (txtLongueur.Text != "" && !Int32.TryParse(txtLongueur.Text, out number))
            {
                MessageBox.Show("Ce champ accepte seulement des chiffres, veuillez corriger cette erreur.");
                txtLongueur.Focus();
            }
            else if (number < 1 || number > 500)
            {
                MessageBox.Show("La valeur doit être entre 1 et 500, veuillez corriger cette erreur.");
                txtLongueur.Focus();
            }
        }

        private void txtLargeur_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            int number = 2;
            if (txtLargeur.Text != "" && !Int32.TryParse(txtLargeur.Text, out number))
            {
                MessageBox.Show("Ce champ accepte seulement des chiffres, veuillez corriger cette erreur.");
                txtLargeur.Focus();
            }
            else if (number < 1 || number > 500)
            {
                MessageBox.Show("La valeur doit être entre 1 et 500, veuillez corriger cette erreur.");
                txtLargeur.Focus();
            }
        }

        private void txtHauteur_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            int number = 2;
            if (txtHauteur.Text != "" && !Int32.TryParse(txtHauteur.Text, out number))
            {
                MessageBox.Show("Ce champ accepte seulement des chiffres, veuillez corriger cette erreur.");
                txtHauteur.Focus();
            }
            else if (number < 1 || number > 500)
            {
                MessageBox.Show("La valeur doit être entre 1 et 500, veuillez corriger cette erreur.");
                txtHauteur.Focus();
            }
        }
    }
}
