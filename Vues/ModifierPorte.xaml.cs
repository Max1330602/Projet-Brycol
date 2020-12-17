using App_Brycol.Modele;
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
    /// Logique d'interaction pour ModifierPorte.xaml
    /// </summary>
    public partial class ModifierPorte : Window
    {
        public ModifierPorte()
        {
            InitializeComponent();

            if (Piece2D.toolbarImage.Source.ToString().Contains("droite"))
                cmbPorte.Text = "Droite";
            else
                cmbPorte.Text = "Gauche";

            if (Projet_VM.themeSombre)
                AppliquerThemeSombre();
            else
                EnleverThemeSombre();

        }

        private void btnRetour_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAppliquer_Click(object sender, RoutedEventArgs e)
        {
            foreach (ItemsPlan ip in Item_VM.ItemsPlanActuel)
            {
                ip.Tag = ip.ID;
                if (Piece2D.toolbarImage.Tag.ToString() == ip.Tag.ToString())
                {
                    ip.cotePorte = cmbPorte.Text;
                    OutilEF.brycolContexte.SaveChanges();
                }
            }
            this.Close();
            foreach (Window w in Application.Current.Windows)
            {
                if (w.GetType() == typeof(PlanDeTravail))
                {
                    (w as PlanDeTravail).grdPlanTravail.Children.Clear();
                    (w as PlanDeTravail).grdPlanTravail.Children.Add(new PlanDeTravail2());
                }
            }
        }

        private void EnleverThemeSombre()
        {

            btnRetour.Background = Brushes.White;
            btnRetour.Foreground = Brushes.Black;

            btnAppliquer.Background = Brushes.White;
            btnRetour.Foreground = Brushes.Black;

            grdModifPorte.Background = Brushes.White;

            Banniere.Background = Brushes.LightGray;

        }

        private void AppliquerThemeSombre()
        {
            BrushConverter bc = new BrushConverter();
            Brush CouleurBouton = (Brush)bc.ConvertFrom("#45463F");
            Brush CouleurArriere = (Brush)bc.ConvertFrom("#33342F");
            Brush CouleurBanniere = (Brush)bc.ConvertFrom("#84857D");

            btnRetour.Background = CouleurBouton;
            btnRetour.Foreground = Brushes.White;

            btnAppliquer.Background = CouleurBouton;
            btnAppliquer.Foreground = Brushes.White;

            grdModifPorte.Background = CouleurArriere;

            Banniere.Background = CouleurBanniere;
        }
    }
}
