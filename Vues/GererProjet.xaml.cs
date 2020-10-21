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
using System.Windows.Shapes;

namespace App_Brycol.Vues
{
    /// <summary>
    /// Logique d'interaction pour Projet.xaml
    /// </summary>
    public partial class GererProjet : Window
    {
        public GererProjet()
        {
            InitializeComponent();
        }

        private void btnAjouterPiece_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            if (Projet_VM.ProjetActuel.ListePieces.Count() < 8)
            {
                InfoPiece popUp = new InfoPiece("Ajouter");
                popUp.ShowDialog();
            }
            else
            {
                MessageBox.Show("Déjà 8 pièces sont liées à ce projet", "Maximum de pièces atteint", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSupprimerPiece_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Voulez-vraiment supprimer cette pièce ?", "Suppression de pièce", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        }

        private void btnPlan_Click(object sender, RoutedEventArgs e)
        {
            Grid gridMW = (Grid)Application.Current.MainWindow.FindName("gridMainWindow");
            ContentPresenter cpMW = (ContentPresenter)Application.Current.MainWindow.FindName("presenteurContenu");
            this.Close();
            gridMW.Children.Clear();
            gridMW.Children.Add(cpMW);
            cpMW.Content = new PlanDeTravail();
        }

        private void btnCoutProjet_Click(object sender, RoutedEventArgs e)
        {
            string UCEcran = "Projet";
            Cout popUp = new Cout(UCEcran);
            popUp.ShowDialog();
        }

        private void btnQuitter_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnPiece1_Click(object sender, RoutedEventArgs e)
        {
            if (!btnPlan.IsEnabled)
            {
                btnPiece1.Background = new SolidColorBrush(Colors.LightBlue);
                btnPlan.IsEnabled = !btnPlan.IsEnabled;
            }
            else
            {
                btnPiece1.Background = new SolidColorBrush(Colors.White);
                btnPlan.IsEnabled = !btnPlan.IsEnabled;
            }
        }

        private void btnPiece2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPiece3_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPiece4_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPiece5_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPiece6_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPiece7_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPiece8_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
