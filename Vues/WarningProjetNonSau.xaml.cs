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
            DataContext = new Projet_VM();

            if (Projet_VM.themeSombre)
                AppliquerThemeSombre();
            else
                EnleverThemeSombre();
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

        private void AppliquerThemeSombre()
        {
            BrushConverter bc = new BrushConverter();
            Brush CouleurBouton = (Brush)bc.ConvertFrom("#45463F");
            Brush CouleurArrierePlan = (Brush)bc.ConvertFrom("#7D7E79");

            grdWarning.Background = CouleurArrierePlan;

            btnComfirm.Background = CouleurBouton;
            btnComfirm.Foreground = Brushes.White;

            btnRefus.Background = CouleurBouton;
            btnRefus.Foreground = Brushes.White;

        }

        private void EnleverThemeSombre()
        {
            grdWarning.Background = Brushes.White;

            btnComfirm.Background = Brushes.White;
            btnComfirm.Foreground = Brushes.Black;

            btnRefus.Background = Brushes.White;
            btnRefus.Foreground = Brushes.Black;
        }
    }
}
