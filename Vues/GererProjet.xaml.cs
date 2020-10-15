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
            InfoPiece popUp = new InfoPiece();
            popUp.ShowDialog();
        }

        private void btnSupprimerPiece_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Voulez-vraiment supprimer cette pièce ?", "Suppression de pièce", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        }

        private void btnPlan_Click(object sender, RoutedEventArgs e)
        {
            // PlanDeTravail popUp = new PlanDeTravail();
            // popUp.ShowDialog();
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

    }
}
