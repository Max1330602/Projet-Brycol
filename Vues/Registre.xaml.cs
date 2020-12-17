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
    /// Logique d'interaction pour Registre.xaml
    /// </summary>
    public partial class Registre : Window
    {
        public Registre()
        {
            InitializeComponent();

            if (Projet_VM.themeSombre)
                AppliquerThemeSombre();
            else
                EnleverThemeSombre();

            DataContext = new Utilisateur_VM();
        }

        private void AppliquerThemeSombre()
        {
            BrushConverter bc = new BrushConverter();
            Brush CouleurBouton = (Brush)bc.ConvertFrom("#45463F");
            Brush CouleurArriere = (Brush)bc.ConvertFrom("#33342F");
            Brush CouleurBanniere = (Brush)bc.ConvertFrom("#84857D");
            Brush CouleurArrierePlan = (Brush)bc.ConvertFrom("#7D7E79");

            Banniere.Background = CouleurBanniere;
            DG_factures.Background = CouleurArrierePlan;

            DG_factures.RowBackground = Brushes.LightGray;
            DG_factures.AlternatingRowBackground = Brushes.Gray;

            Pied.Background = CouleurArrierePlan;

            btnRetour.Background = CouleurBouton;
            btnRetour.Foreground = Brushes.White;
        }

        private void EnleverThemeSombre()
        {
            Banniere.Background = Brushes.LightGray;
            DG_factures.Background = Brushes.White;

            DG_factures.RowBackground = Brushes.CornflowerBlue;
            DG_factures.AlternatingRowBackground = Brushes.White;

            Pied.Background = Brushes.White;

            btnRetour.Background = Brushes.White;
            btnRetour.Foreground = Brushes.Black;
        }

        private void btnRetour_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
