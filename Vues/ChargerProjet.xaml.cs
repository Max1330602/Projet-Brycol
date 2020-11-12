using App_Brycol.Outils;
using App_Brycol.VuesModele;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace App_Brycol.Vues
{
    /// <summary>
    /// Logique d'interaction pour ChargerProjet.xaml
    /// </summary>
    public partial class ChargerProjet : Window
    {
        public ChargerProjet()
        {
            InitializeComponent();
            DataContext = new Projet_VM();

            if (Projet_VM.themeSombre)
                AppliquerThemeSombre();
            else
                EnleverThemeSombre();

            var pReq = (from p in OutilEF.brycolContexte.Projets select p.Nom).ToList();

            cmbProjets.ItemsSource = pReq;
        }

        private void EnleverThemeSombre()
        {
            Banniere.Background = Brushes.LightGray;
            lblProjet.Background = Brushes.White;

            btnOk.Background = Brushes.White;
            btnOk.Foreground = Brushes.Black;
        }

        private void AppliquerThemeSombre()
        {
            BrushConverter bc = new BrushConverter();
            Brush CouleurBouton = (Brush)bc.ConvertFrom("#45463F");
            Brush CouleurBanniere = (Brush)bc.ConvertFrom("#84857D");
            Brush CouleurArrierePlan = (Brush)bc.ConvertFrom("#7D7E79");

            Banniere.Background = CouleurBanniere;
            lblProjet.Background = CouleurArrierePlan;

            btnOk.Background = CouleurBouton;
            btnOk.Foreground = Brushes.White;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
