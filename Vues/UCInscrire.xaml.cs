using App_Brycol.VuesModele;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace App_Brycol.Vues
{
    /// <summary>
    /// Logique d'interaction pour UCInscrire.xaml
    /// </summary>
    public partial class UCInscrire : UserControl
    {
        private UCCLogin uCLogin { get; set; }

        public UCInscrire()
        {
            InitializeComponent();
            DataContext = new Utilisateur_VM();

        }

        private void btnRetour_Click(object sender, RoutedEventArgs e)
        {
            uCLogin = new UCCLogin();
            Grid.SetRow(uCLogin, 1);

            Grid gridMW = (Grid)Application.Current.MainWindow.FindName("gridMainWindow");
            gridMW.Children.Clear();
            gridMW.Children.Add(uCLogin);
        }

        private void btnInscrire_Click(object sender, RoutedEventArgs e)
        {
            Regex r = new Regex("^[A-Za-z0-9_-]*$");

            if (Nom.Text.ToString() == "" || pwMotPasse.Password == null || pwConfirm.Password == null)
            {
                MessageBox.Show("Un des champs est vide.");
                return;
            }
            else if ( !r.IsMatch(Nom.Text.ToString()) || !r.IsMatch(pwMotPasse.Password.ToString()) || !r.IsMatch(pwConfirm.Password.ToString()) )
            {
                MessageBox.Show("Les espaces sont interdites.");
                return;
            }
            else if (pwMotPasse.Password.ToString() != pwConfirm.Password.ToString())
            {
                MessageBox.Show("Le mot de passe et la confirmation ne sont pas pareil.");
                return;
            }
            btnInscrire.SetBinding(Button.CommandProperty, new Binding("cmdCreeUtil"));
        }

        private void EnleverThemeSombre()
        {
            Banniere.Background = Brushes.LightGray;

            grdInscription.Background = Brushes.White;

            btnRetour.Background = Brushes.White;
            btnRetour.Foreground = Brushes.Black;

            btnInscrire.Background = Brushes.White;
            btnInscrire.Foreground = Brushes.Black;

        }

        private void AppliquerThemeSombre()
        {
            BrushConverter bc = new BrushConverter();
            Brush CouleurBouton = (Brush)bc.ConvertFrom("#45463F");
            Brush CouleurBanniere = (Brush)bc.ConvertFrom("#84857D");
            Brush CouleurArrierePlan = (Brush)bc.ConvertFrom("#7D7E79");

            Banniere.Background = CouleurBanniere;

            grdInscription.Background = CouleurArrierePlan;

            btnRetour.Background = CouleurBouton;
            btnRetour.Foreground = Brushes.White;

            btnInscrire.Background = CouleurBouton;
            btnInscrire.Foreground = Brushes.White;
        }
    }
}
