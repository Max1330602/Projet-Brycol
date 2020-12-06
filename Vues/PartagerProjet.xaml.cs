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
        private UCUnProUnUser uCUnProUser { get; set; }
        private UCPluProjUnUser uCPluProUnUser { get; set; }

        public PartagerProjet()
        {
            InitializeComponent();

            DataContext = new Projet_VM();

            if (Projet_VM.themeSombre)
                AppliquerThemeSombre();
            else
                EnleverThemeSombre();

            uCUnProUser = new UCUnProUnUser();
            Grid.SetRow(uCUnProUser, 2);

            grdPartage.Children.Add(uCUnProUser);



        }

        private void EnleverThemeSombre()
        {
            Banniere.Background = Brushes.LightGray;

            btnUnProUnUti.Background = Brushes.White;
            btnUnProUnUti.Foreground = Brushes.Black;

            btnPluProUnUti.Background = Brushes.White;
            btnPluProUnUti.Foreground = Brushes.Black;

            btnPluProPluUti.Background = Brushes.White;
            btnPluProPluUti.Foreground = Brushes.Black;


        }

        private void AppliquerThemeSombre()
        {
            BrushConverter bc = new BrushConverter();
            Brush CouleurBouton = (Brush)bc.ConvertFrom("#45463F");
            Brush CouleurBanniere = (Brush)bc.ConvertFrom("#84857D");
            Brush CouleurArrierePlan = (Brush)bc.ConvertFrom("#7D7E79");

            Banniere.Background = CouleurBanniere;

            btnUnProUnUti.Background = CouleurBouton;
            btnUnProUnUti.Foreground = Brushes.White;

            btnPluProUnUti.Background = CouleurBouton;
            btnPluProUnUti.Foreground = Brushes.White;

            btnPluProPluUti.Background = CouleurBouton;
            btnPluProPluUti.Foreground = Brushes.White;

        }

        private void btnUnProUnUti_Click(object sender, RoutedEventArgs e)
        {
            grdPartage.Children.Remove(uCUnProUser);
            grdPartage.Children.Remove(uCPluProUnUser);

            uCUnProUser = new UCUnProUnUser();
            Grid.SetRow(uCUnProUser, 2);

            grdPartage.Children.Add(uCUnProUser);
        }

        private void btnPluProUnUti_Click(object sender, RoutedEventArgs e)
        {
            grdPartage.Children.Remove(uCUnProUser);
            grdPartage.Children.Remove(uCPluProUnUser);

            uCPluProUnUser = new UCPluProjUnUser();
            Grid.SetRow(uCPluProUnUser, 2);

            grdPartage.Children.Add(uCPluProUnUser);
        }

        private void btnPluProPluUti_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
