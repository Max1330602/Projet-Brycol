using App_Brycol.Modele;
using App_Brycol.VuesModele;
using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Logique d'interaction pour InfoPiece.xaml
    /// </summary>
    public partial class InfoPiece : Window 
    {
        public InfoPiece([Optional] string paramOpt)
        {
            InitializeComponent();

            DataContext = new Piece_VM(paramOpt);

            if (Projet_VM.themeSombre)
                AppliquerThemeSombre();
            else
                EnleverThemeSombre();

        }

        public static string uniteMesure;
      

        private void btnContinuer_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)pied.IsChecked && (Int32.Parse(txtLongueur.Text) > 100 || Int32.Parse(txtLongueur.Text) < 3))
            {
                MessageBox.Show("Les dimensions ne sont pas valides. (Maximum 100 pieds et minimum de 3 pieds)");
                return;
            }
            else if ((bool)metre.IsChecked && (Int32.Parse(txtLongueur.Text) > 30 || Int32.Parse(txtLongueur.Text) < 1))
            {
                MessageBox.Show("Les dimensions ne sont pas valides. (Maximum de 30 mètres et minimum de 1 mètres)");
                return;
            }
                   
            this.Close();
            btnContinuer.SetBinding(Button.CommandProperty, new Binding("cmdPiece"));
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            if (Plan_VM.PlanActuel != null)
            {
                Application.Current.MainWindow.Show();
            }
            else
            {
                GererProjet popUp = new GererProjet();
                popUp.ShowDialog();
            } 
        }

        private void metre_Checked(object sender, RoutedEventArgs e)
        {
            uniteMesure = "Mètres";
            Plan_VM.uniteDeMesure = uniteMesure;

            if (txtUniMesu != null)
                txtUniMesu.Text = "m";

        }

        private void pied_Checked(object sender, RoutedEventArgs e)
        {
            uniteMesure = "Pieds";
            Plan_VM.uniteDeMesure = uniteMesure;

            if (txtUniMesu != null)
                txtUniMesu.Text = "p";
        }

        private void txtLongueur_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (txtLongueur.Text != "" && txtLargeur.Text != "")
                txtSuperf.Text = (Convert.ToDouble(txtLargeur.Text) * Convert.ToDouble(txtLongueur.Text)).ToString();

        }

        private void txtLargeur_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (txtLongueur.Text != "" && txtLargeur.Text != "")
                txtSuperf.Text = (Convert.ToDouble(txtLargeur.Text) * Convert.ToDouble(txtLongueur.Text)).ToString();
        }

        private void EnleverThemeSombre()
        {
            grdInfoPiece.Background = Brushes.White;
        }

        private void AppliquerThemeSombre()
        {
            BrushConverter bc = new BrushConverter();
            Brush CouleurBouton = (Brush)bc.ConvertFrom("#45463F");
            Brush CouleurBanniere = (Brush)bc.ConvertFrom("#84857D");
            Brush CouleurArrierePlan = (Brush)bc.ConvertFrom("#7D7E79");

            grdInfoPiece.Background = CouleurArrierePlan;

            btnAnnuler.Background = CouleurBouton;
            btnAnnuler.Foreground = Brushes.White;

            btnContinuer.Background = CouleurBouton;
            btnContinuer.Foreground = Brushes.White;
        }
    }
}
