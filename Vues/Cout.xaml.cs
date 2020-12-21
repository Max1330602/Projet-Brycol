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
        private Transaction uCCTransaction { get; set; }
        public static UCCoutDetailProjet uCCoutDetailProjet { get; set; }

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

        private void btnRetour_Click(object sender, RoutedEventArgs e)
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
            btnTransaction.Visibility = Visibility.Hidden;
            btnRegistre.Visibility = Visibility.Hidden;
            btnVoirCoutPiece.Visibility = Visibility.Visible;

            if (btnAnnulerTransaction.Visibility == Visibility.Visible)
            {
                btnAnnulerPayer_Click(btnAnnulerTransaction, e);
                btnAnnulerTransaction.Visibility = Visibility.Hidden;
                btnTransaction.Visibility = Visibility.Hidden;
            }

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
            btnRegistre.Visibility = Visibility.Hidden;
            btnTransaction.Visibility = Visibility.Hidden;
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
            btnRegistre.Visibility = Visibility.Visible;
            btnTransaction.Visibility = Visibility.Visible;
            btnVoirCoutProjet.Visibility = Visibility.Visible;

        }

        private void EnleverThemeSombre()
        {
         
            Banniere.Background = Brushes.LightGray;

            grdCoutParent.Background = Brushes.White;

            btnRetour.Background = Brushes.White;
            btnRetour.Foreground = Brushes.Black;

            btnRegistre.Background = Brushes.White;
            btnRegistre.Foreground = Brushes.Black;

            btnTransaction.Background = Brushes.White;
            btnTransaction.Foreground = Brushes.Black;

            btnAnnulerTransaction.Background = Brushes.White;
            btnAnnulerTransaction.Foreground = Brushes.Black;

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

            btnRegistre.Background = CouleurBouton;
            btnRegistre.Foreground = Brushes.White;

            btnRetour.Background = CouleurBouton;
            btnRetour.Foreground = Brushes.White;

            btnTransaction.Background = CouleurBouton;
            btnTransaction.Foreground = Brushes.White;

            btnAnnulerTransaction.Background = CouleurBouton;
            btnAnnulerTransaction.Foreground = Brushes.White;

            btnVoirCoutProjet.Background = CouleurBouton;
            btnVoirCoutProjet.Foreground = Brushes.White;

            btnVoirCoutDetailProjet.Background = CouleurBouton;
            btnVoirCoutDetailProjet.Foreground = Brushes.White;

            btnVoirCoutPiece.Background = CouleurBouton;
            btnVoirCoutPiece.Foreground = Brushes.White;
        }

        private void btnPayer_Click(object sender, RoutedEventArgs e)
        {
            uCCTransaction = new Transaction();
            this.Width = this.Width + uCCTransaction.Width;
            ColumnDefinition ColonneTransaction = new ColumnDefinition();
            ColonneTransaction.Width = new GridLength(400, GridUnitType.Pixel);
            grdCoutParent.ColumnDefinitions.Add(ColonneTransaction);


            Grid.SetColumn(uCCTransaction, 1);
            Grid.SetRow(uCCTransaction, 0);
            Grid.SetRowSpan(uCCTransaction, 2);
            grdCoutParent.Children.Add(uCCTransaction);

            btnTransaction.Visibility = Visibility.Hidden;
            btnAnnulerTransaction.Visibility = Visibility.Visible;

            
        }

        private void btnAnnulerPayer_Click(object sender, RoutedEventArgs e)
        {
            this.Width = 800;
            grdCoutParent.Children.Remove(uCCTransaction);
            grdCoutParent.ColumnDefinitions.RemoveAt(1);

            btnTransaction.Visibility = Visibility.Visible;
            btnAnnulerTransaction.Visibility = Visibility.Hidden;
        }

        private void btnRegistre_Click(object sender, RoutedEventArgs e)
        {
            Registre popup = new Registre();
            popup.ShowDialog();
        }
    }
}
