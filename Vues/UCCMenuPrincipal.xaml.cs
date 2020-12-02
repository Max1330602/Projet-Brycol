using App_Brycol.Outils;
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

namespace App_Brycol.Vues
{
    /// <summary>
    /// Logique d'interaction pour UCCMenuPrincipal.xaml
    /// </summary>
    public partial class UCCMenuPrincipal : UserControl
    {
        public UCCMenuPrincipal()
        {
            InitializeComponent();
            DataContext = new Projet_VM();

            if (Projet_VM.themeSombre)
                AppliquerThemeSombre();
            else
                EnleverThemeSombre();

            var pReq = (from p in OutilEF.brycolContexte.Projets.Include("Utilisateur") where p.Utilisateur.Nom == Utilisateur_VM.utilActuel.Nom select p.Nom).ToList();

            if (pReq.Count() != 0)
            {
                btnTeleverserProjet.IsEnabled = true;
            }


        }

        private void btnTeleverserProjet_Click(object sender, RoutedEventArgs e)
        {
            ChargerProjet popup = new ChargerProjet();
            popup.ShowDialog();
        }


        private void EnleverThemeSombre()
        {
            Banniere.Background = Brushes.LightGray;

            grdMenuPrincipal.Background = Brushes.White;

            btnCreerProjet.Background = Brushes.White;
            btnCreerProjet.Foreground = Brushes.Black;

            if (btnTeleverserProjet.IsEnabled)
            {
                btnTeleverserProjet.Background = Brushes.White;
                btnTeleverserProjet.Foreground = Brushes.Black;
            }


        }

        private void AppliquerThemeSombre()
        {
            BrushConverter bc = new BrushConverter();
            Brush CouleurBouton = (Brush)bc.ConvertFrom("#45463F");
            Brush CouleurBanniere = (Brush)bc.ConvertFrom("#84857D");
            Brush CouleurArrierePlan = (Brush)bc.ConvertFrom("#7D7E79");

            Banniere.Background = CouleurBanniere;

            grdMenuPrincipal.Background = CouleurArrierePlan;

            btnCreerProjet.Background = CouleurBouton;
            btnCreerProjet.Foreground = Brushes.White;

            btnTeleverserProjet.Background = CouleurBouton;
            btnTeleverserProjet.Foreground = Brushes.White;
        }
    }
}
