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
    /// Logique d'interaction pour Aide.xaml
    /// </summary>
    public partial class Aide : Window
    {
        public Aide()
        {
            InitializeComponent();

            if (Projet_VM.themeSombre)
                AppliquerThemeSombre();
            else
                EnleverThemeSombre();
        }

        private void EnleverThemeSombre()
        {
            Banniere.Background = Brushes.LightGray;
            Corps.Background = Brushes.White;

            btnRetour.Background = Brushes.White;
            btnRetour.Foreground = Brushes.Black;
        }

        private void AppliquerThemeSombre()
        {
            BrushConverter bc = new BrushConverter();
            Brush CouleurBouton = (Brush)bc.ConvertFrom("#45463F");
            Brush CouleurBanniere = (Brush)bc.ConvertFrom("#84857D");
            Brush CouleurArrierePlan = (Brush)bc.ConvertFrom("#7D7E79");

            Banniere.Background = CouleurBanniere;
            Corps.Background = Brushes.LightGray;

            btnRetour.Background = CouleurBouton;
            btnRetour.Foreground = Brushes.White;
        }

        private void btnRetour_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
