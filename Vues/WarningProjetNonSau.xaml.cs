using App_Brycol.VuesModele;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Logique d'interaction pour WarningProjetNonSau.xaml
    /// </summary>
    public partial class WarningProjetNonSau : Window
    {


        public WarningProjetNonSau()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            DataContext = new Projet_VM();
        }

        private void btnComfirm_Click(object sender, RoutedEventArgs e)
        {
            Enregistrer popUp = new Enregistrer();
            popUp.ShowDialog();

            this.Close();
        }

        private void btnRefus_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
