using App_Brycol.Modele;
using App_Brycol.VuesModele;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ubiety.Dns.Core;

namespace App_Brycol.Vues
{
    /// <summary>
    /// Logique d'interaction pour PlanDeTravail.xaml
    /// </summary>
    public partial class PlanDeTravail : UserControl
    {
        public PlanDeTravail()
        {
            InitializeComponent();
            DataContext = new Projet_VM();
            lblProjet.Content = Projet_VM.ProjetActuel.Nom;
            lblPiece.Content = Piece_VM.pieceActuel.Nom;
        }

        public static Catalogue Catalogue;
        private void btnAide_Click(object sender, RoutedEventArgs e)
        {
            Aide popUp = new Aide();
            popUp.ShowDialog();
        }

        private void btn2D_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn3D_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnCatalogue_Click(object sender, RoutedEventArgs e)
        {
            Catalogue popUp = new Catalogue();
            popUp.ShowDialog();
        }

        private void btnModifierItem_Click(object sender, RoutedEventArgs e)
        {
            ModifierItem popUp = new ModifierItem();
            popUp.ShowDialog();
        }
        

        private void btnSupprimerItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Voulez-vraiment supprimer cet item ?", "Suppression d'item", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
           
        }

        private void btnModifierPiece_Click(object sender, RoutedEventArgs e)
        {
            InfoPiece popUp = new InfoPiece("Modifier");
            popUp.ShowDialog();
        }

        private void btnProjet_Click(object sender, RoutedEventArgs e)
        {
            GererProjet popUp = new GererProjet();
            popUp.ShowDialog();
        }

        private void btnThemeSombre_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCoutPiece_Click(object sender, RoutedEventArgs e)
        {
            string UCEcran = "Piece";
            Cout popUp = new Cout(UCEcran);
            popUp.ShowDialog();
        }

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            Enregistrer popUp = new Enregistrer();
            popUp.ShowDialog();
        }

        private void Piece2D_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void test(object sender, RoutedEventArgs e)
        {
            btnSupprimerItem.IsEnabled = true;
        }
    }
}
