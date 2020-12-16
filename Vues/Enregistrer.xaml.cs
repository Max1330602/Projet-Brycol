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
    /// Logique d'interaction pour Enregistrer.xaml
    /// </summary>
    public partial class Enregistrer : Window
    {
        public Enregistrer()
        {
            InitializeComponent();
            DataContext = new Projet_VM();

            if (Projet_VM.themeSombre)
                AppliquerThemeSombre();
            else
                EnleverThemeSombre();

        }

        

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnRetour_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void AppliquerThemeSombre()
        {
            BrushConverter bc = new BrushConverter();
            Brush CouleurBouton = (Brush)bc.ConvertFrom("#45463F");
            Brush CouleurArriere = (Brush)bc.ConvertFrom("#33342F");
            Brush CouleurBanniere = (Brush)bc.ConvertFrom("#84857D");
            Brush CouleurArrierePlan = (Brush)bc.ConvertFrom("#7D7E79");

            Banniere.Background = CouleurBanniere;
            grdEnregistrer.Background = CouleurArrierePlan;

            btnEnregistrer.Background = CouleurBouton;
            btnEnregistrer.Foreground = Brushes.White;

            btnRetour.Background = CouleurBouton;
            btnRetour.Foreground = Brushes.White;

        }

        private void EnleverThemeSombre()
        {
            Banniere.Background = Brushes.LightGray;
            grdEnregistrer.Background = Brushes.White;

            btnEnregistrer.Background = Brushes.White;
            btnEnregistrer.Foreground = Brushes.Black;

            btnRetour.Background = Brushes.White;
            btnRetour.Foreground = Brushes.Black;
        }
        
    }
}
