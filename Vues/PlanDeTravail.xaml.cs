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
            Piece2D Plan2D = new Piece2D();
        }

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
            DimensionsPiece popUp = new DimensionsPiece();
            popUp.ShowDialog();
        }

        private void btnProjet_Click(object sender, RoutedEventArgs e)
        {
            Projet popUp = new Projet();
            popUp.ShowDialog();
        }

        private void btnThemeSombre_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCoutPiece_Click(object sender, RoutedEventArgs e)
        {
            Cout popUp = new Cout();
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
