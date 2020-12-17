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
    /// Logique d'interaction pour ChargerProjet.xaml
    /// </summary>
    public partial class ChargerProjet : Window
    {
        public ChargerProjet()
        {
            InitializeComponent();
            DataContext = new Projet_VM();

            var pReq = (from p in OutilEF.brycolContexte.Projets.Include("Utilisateur") where p.Utilisateur.Nom == Utilisateur_VM.utilActuel.Nom  select p.Nom).ToList();

            if (Projet_VM.themeSombre)
                AppliquerThemeSombre();
            else
                EnleverThemeSombre();

            cmbProjets.ItemsSource = pReq;
        }

        private void EnleverThemeSombre()
        {
            Banniere.Background = Brushes.LightGray;
            lblProjet.Background = Brushes.White;

            btnRetour.Background = Brushes.White;
            btnRetour.Foreground = Brushes.Black;

            if (btnOk.IsEnabled)
            {
                btnOk.Background = Brushes.White;
                btnOk.Foreground = Brushes.Black;

                btnSupprimer.Background = Brushes.White;
                btnSupprimer.Foreground = Brushes.Black;
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

            btnOk.Background = CouleurBouton;
            btnOk.Foreground = Brushes.White;

            btnRetour.Background = CouleurBouton;
            btnRetour.Foreground = Brushes.White;

            btnSupprimer.Background = CouleurBouton;
            btnSupprimer.Foreground = Brushes.White;

        }

        private void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            WarningSupPro popUp = new WarningSupPro(this);
            popUp.ShowDialog();
            cmbProjets.SelectedIndex = -1;

            var pReq = (from p in OutilEF.brycolContexte.Projets.Include("Utilisateur") where p.Utilisateur.Nom == Utilisateur_VM.utilActuel.Nom select p.Nom).ToList();
            cmbProjets.ItemsSource = pReq;

            btnOk.IsEnabled = false;
            btnSupprimer.IsEnabled = false;

        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cmbProjets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnOk.IsEnabled = true;
            btnSupprimer.IsEnabled = true;
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            Grid gridMW = (Grid)Application.Current.MainWindow.FindName("gridMainWindow");
            ContentPresenter cpMW = (ContentPresenter)Application.Current.MainWindow.FindName("presenteurContenu");
            gridMW.Children.Clear();
            gridMW.Children.Add(cpMW);
            cpMW.Content = new UCCMenuPrincipal();
        }

        private void btnRetour_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
