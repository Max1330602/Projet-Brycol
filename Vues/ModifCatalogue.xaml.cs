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
