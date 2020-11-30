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
    /// Logique d'interaction pour UCCLogin.xaml
    /// </summary>
    public partial class UCCLogin : UserControl
    {
        private UCInscrire uCInscrire { get; set; }

        public UCCLogin()
        {
            InitializeComponent();
            DataContext = new Utilisateur_VM();

            if (Projet_VM.themeSombre)
                AppliquerThemeSombre();
            else
                EnleverThemeSombre();


            uCInscrire = new UCInscrire();
            Grid.SetRow(uCInscrire, 1);
        }

        private void btnInscrire_Click(object sender, RoutedEventArgs e)
        {
            Grid gridMW = (Grid)Application.Current.MainWindow.FindName("gridMainWindow");
            gridMW.Children.Clear();
            gridMW.Children.Add(uCInscrire);
        }

        private void EnleverThemeSombre()
        {
            Banniere.Background = Brushes.LightGray;

            btnInscrire.Background = Brushes.White;
            btnInscrire.Foreground = Brushes.Black;

            btnLogin.Background = Brushes.White;
            btnLogin.Foreground = Brushes.Black;

            btnTriche.Background = Brushes.White;
            btnTriche.Foreground = Brushes.Black;
        }

        private void AppliquerThemeSombre()
        {
            BrushConverter bc = new BrushConverter();
            Brush CouleurBouton = (Brush)bc.ConvertFrom("#45463F");
            Brush CouleurArriere = (Brush)bc.ConvertFrom("#33342F");
            Brush CouleurBanniere = (Brush)bc.ConvertFrom("#84857D");
            Brush CouleurArrierePlan = (Brush)bc.ConvertFrom("#7D7E79");

            Banniere.Background = CouleurBanniere;

            btnInscrire.Background = CouleurBouton;
            btnInscrire.Foreground = Brushes.White;

            btnLogin.Background = CouleurBouton;
            btnLogin.Foreground = Brushes.White;

            btnTriche.Background = CouleurBouton;
            btnTriche.Foreground = Brushes.White;
        }

    }
}
