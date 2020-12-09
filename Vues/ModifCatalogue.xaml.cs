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

            grdModifierCatalogue.Background = CouleurArriere;
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
                string pathCorrect = path.Substring(0, path.IndexOf("bin")) + "images\\items\\";
                var iReq = from i in OutilEF.brycolContexte.Meubles select i;
                int dernierItemNum = iReq.OrderByDescending(it => it.ID).FirstOrDefault().ID;
                dernierItemNum = dernierItemNum + 1;
                img.Save(pathCorrect + "item" + dernierItemNum + ".png", ImageFormat.Png);

                imgCat.Source = new BitmapImage(new Uri(pathCorrect + "item" + dernierItemNum + ".png"));
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
                string pathCorrect = path.Substring(0, path.IndexOf("bin")) + "images\\items\\Top\\";
                var iReq = from i in OutilEF.brycolContexte.Meubles select i;
                int dernierItemNum = iReq.OrderByDescending(it => it.ID).FirstOrDefault().ID;
                dernierItemNum = dernierItemNum + 1;
                img.Save(pathCorrect + "item" + dernierItemNum + ".png", ImageFormat.Png);

                imgHaut.Source = new BitmapImage(new Uri(pathCorrect + "item" + dernierItemNum + ".png"));
            }


        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnRetour_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Catalogue popUp = new Catalogue();
            popUp.ShowDialog();
        }

        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
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
                MessageBox.Show("Erreur de chargement, veuillez modifier le contenu de votre ajout.");
            }
            

            
        }
    }
}
