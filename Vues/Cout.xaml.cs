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
    }
}
