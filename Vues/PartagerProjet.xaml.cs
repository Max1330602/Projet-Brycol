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
            lsbPro.ItemsSource = pReq;

            var uReq = (from u in OutilEF.brycolContexte.Utilisateurs select u.Nom).ToList();
            uReq.Remove(Utilisateur_VM.utilActuel.Nom);
            lsbUti.ItemsSource = uReq;

            if (Projet_VM.themeSombre)
                AppliquerThemeSombre();
            else
                EnleverThemeSombre();

        }


        private void EnleverThemeSombre()
        {
            Banniere.Background = Brushes.LightGray;

            grdPartage.Background = Brushes.White;

            btnRetour.Background = Brushes.White;
            btnRetour.Foreground = Brushes.Black;

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

            grdPartage.Background = CouleurArrierePlan;

            btnRetour.Background = CouleurBouton;
            btnRetour.Foreground = Brushes.White;

            if (btnPartage.IsEnabled)
            {
                btnPartage.Background = CouleurBouton;
                btnPartage.Foreground = Brushes.White;

            }


        }

        private void btnPrendreUti_Click(object sender, RoutedEventArgs e)
        {
            lsbUtiPa.Items.Clear(); // S'il y a des items, les supprimmer

            Projet_VM.LstUtilisateurPartage.Clear();

            foreach (object o in lsbUti.SelectedItems)
            {
                lsbUtiPa.Items.Add(o);
                Projet_VM.LstUtilisateurPartage.Add(o.ToString());
            }

            if (lsbProPa.Items.Count != 0)
                btnPartage.IsEnabled = true;
            else
                btnPartage.IsEnabled = false;
        }

        private void btnRemettreUti_Click(object sender, RoutedEventArgs e)
        {
            lsbUtiPa.Items.Clear();
            Projet_VM.LstUtilisateurPartage.Clear();

            btnPartage.IsEnabled = false;
        }

        private void btnPrendrePro_Click(object sender, RoutedEventArgs e)
        {
            lsbProPa.Items.Clear(); // S'il y a des items, les supprimmer

            Projet_VM.LstProjetPartage.Clear();

            foreach (object o in lsbPro.SelectedItems)
            {
                lsbProPa.Items.Add(o);
                Projet_VM.LstProjetPartage.Add(o.ToString());
            }

            if (lsbUtiPa.Items.Count != 0)
                btnPartage.IsEnabled = true;
            else
                btnPartage.IsEnabled = false;

        }

        private void btnRemettrePro_Click(object sender, RoutedEventArgs e)
        {
            lsbProPa.Items.Clear();
            Projet_VM.LstProjetPartage.Clear();

            btnPartage.IsEnabled = false;
        }

        private void btnPartage_Click(object sender, RoutedEventArgs e)
        {
            lsbUtiPa.Items.Clear();
            lsbProPa.Items.Clear();
            btnPartage.IsEnabled = false;
        }

        private void btnRetour_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
