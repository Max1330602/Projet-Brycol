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
using System.Windows.Shapes;

namespace App_Brycol.Vues
{
    /// <summary>
    /// Logique d'interaction pour PartagerProjet.xaml
    /// </summary>
    public partial class PartagerProjet : Window
    {
        public PartagerProjet()
        {
            InitializeComponent();

            DataContext = new Projet_VM();

            var pReq = (from p in OutilEF.brycolContexte.Projets.Include("Utilisateur") where p.Utilisateur.Nom == Utilisateur_VM.utilActuel.Nom select p.Nom).ToList();
            cmbProjets.ItemsSource = pReq;

            var uReq = (from u in OutilEF.brycolContexte.Utilisateurs select u.Nom).ToList();
            uReq.Remove(Utilisateur_VM.utilActuel.Nom);
            cmbUtili.ItemsSource = uReq;

            if (Projet_VM.themeSombre)
                AppliquerThemeSombre();
            else
                EnleverThemeSombre();



        }

        private void EnleverThemeSombre()
        {
            Banniere.Background = Brushes.LightGray;
            lblProjet.Background = Brushes.White;

            if (btnPartage.IsEnabled)
            {
                btnPartage.Background = Brushes.White;
                btnPartage.Foreground = Brushes.Black;

            }

        }

        private void AppliquerThemeSombre()
        {
            BrushConverter bc = new BrushConverter();
            Brush CouleurBouton = (Brush)bc.ConvertFrom("#45463F");
            Brush CouleurBanniere = (Brush)bc.ConvertFrom("#84857D");
            Brush CouleurArrierePlan = (Brush)bc.ConvertFrom("#7D7E79");

            Banniere.Background = CouleurBanniere;
            lblProjet.Background = CouleurArrierePlan;

            if (btnPartage.IsEnabled)
            {
                btnPartage.Background = CouleurBouton;
                btnPartage.Foreground = Brushes.White;

            }
        }

        private void cmbProjets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbUtili.SelectedItem != null)
                btnPartage.IsEnabled = true;
            else
                btnPartage.IsEnabled = false;
        }

        private void cmbUtili_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbProjets.SelectedItem != null)
                btnPartage.IsEnabled = true;
            else
                btnPartage.IsEnabled = false;
        }

        private void btnQuitter_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
