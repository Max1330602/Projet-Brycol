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
        }

        private void btnCatalogue_Click(object sender, RoutedEventArgs e)
        {
            Catalogue popUp = new Catalogue();
            popUp.ShowDialog();
        }

        private void btnAide_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn2D_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn3D_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnModifierItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSupprimerItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnModifierPiece_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnProjet_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
