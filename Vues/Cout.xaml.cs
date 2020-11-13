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
    /// Logique d'interaction pour Cout.xaml
    /// </summary>
    public partial class Cout : Window
    {
        private UCCoutPiece uCCoutPiece { get; set; }
        private UCCoutProjet uCCoutProjet { get; set; }
        private UCCoutDetailProjet uCCoutDetailProjet { get; set; }

        public Cout(string UCEcran )
        {
            InitializeComponent();
            DataContext = new Piece_VM("");

            if (Projet_VM.themeSombre)
                AppliquerThemeSombre();
            else
                EnleverThemeSombre();

            switch (UCEcran)
            {
                case "Piece":
                    uCCoutPiece = new UCCoutPiece();
                    Grid.SetRow(uCCoutPiece, 1);

                    grdCoutParent.Children.Add(uCCoutPiece);
                    btnVoirCoutProjet.Visibility= Visibility.Visible;
                    break;
                case "Projet":
                    uCCoutProjet = new UCCoutProjet();
                    Grid.SetRow(uCCoutProjet, 1);

                    grdCoutParent.Children.Add(uCCoutProjet);
                    btnVoirCoutDetailProjet.Visibility = Visibility.Visible;
                    btnVoirCoutPiece.Visibility = Visibility.Visible;
                    break;

            }

        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnVoirCoutProjet_Click(object sender, RoutedEventArgs e)
        {
            grdCoutParent.Children.Remove(uCCoutPiece);
            grdCoutParent.Children.Remove(uCCoutDetailProjet);

            uCCoutProjet = new UCCoutProjet();
            Grid.SetRow(uCCoutProjet, 1);

            grdCoutParent.Children.Add(uCCoutProjet);
            btnVoirCoutProjet.Visibility = Visibility.Collapsed;
            btnVoirCoutDetailProjet.Visibility = Visibility.Visible;
            btnVoirCoutPiece.Visibility = Visibility.Visible;

        }

        private void btnVoirCoutPiece_Click(object sender, RoutedEventArgs e)
        {
            grdCoutParent.Children.Remove(uCCoutProjet);
            grdCoutParent.Children.Remove(uCCoutDetailProjet);

            uCCoutPiece = new UCCoutPiece();
            Grid.SetRow(uCCoutPiece, 1);

            grdCoutParent.Children.Add(uCCoutPiece);
            btnVoirCoutPiece.Visibility = Visibility.Collapsed;
            btnVoirCoutDetailProjet.Visibility = Visibility.Collapsed;
            btnVoirCoutProjet.Visibility = Visibility.Visible;

        }

        private void btnVoirCoutDetailProjet_Click(object sender, RoutedEventArgs e)
        {
            grdCoutParent.Children.Remove(uCCoutProjet);

            uCCoutDetailProjet = new UCCoutDetailProjet();
            Grid.SetRow(uCCoutDetailProjet, 1);

            grdCoutParent.Children.Add(uCCoutDetailProjet);
            btnVoirCoutPiece.Visibility = Visibility.Collapsed;
            btnVoirCoutDetailProjet.Visibility = Visibility.Collapsed;
            btnVoirCoutProjet.Visibility = Visibility.Visible;

        }

        private void EnleverThemeSombre()
        {
            Banniere.Background = Brushes.LightGray;

            grdCoutParent.Background = Brushes.White;

            btnOk.Background = Brushes.White;
            btnOk.Foreground = Brushes.Black;

            btnVoirCoutProjet.Background = Brushes.White;
            btnVoirCoutProjet.Foreground = Brushes.Black;

            btnVoirCoutDetailProjet.Background = Brushes.White;
            btnVoirCoutDetailProjet.Foreground = Brushes.Black;

            btnVoirCoutPiece.Background = Brushes.White;
            btnVoirCoutPiece.Foreground = Brushes.Black;
        }

        private void AppliquerThemeSombre()
        {
            BrushConverter bc = new BrushConverter();
            Brush CouleurBouton = (Brush)bc.ConvertFrom("#45463F");
            Brush CouleurBanniere = (Brush)bc.ConvertFrom("#84857D");
            Brush CouleurArrierePlan = (Brush)bc.ConvertFrom("#7D7E79");

            Banniere.Background = CouleurBanniere;

            grdCoutParent.Background = CouleurArrierePlan;

            btnOk.Background = CouleurBouton;
            btnOk.Foreground = Brushes.White;

            btnVoirCoutProjet.Background = CouleurBouton;
            btnVoirCoutProjet.Foreground = Brushes.White;

            btnVoirCoutDetailProjet.Background = CouleurBouton;
            btnVoirCoutDetailProjet.Foreground = Brushes.White;

            btnVoirCoutPiece.Background = CouleurBouton;
            btnVoirCoutPiece.Foreground = Brushes.White;
        }
    }
}
