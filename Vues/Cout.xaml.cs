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

        public Cout(string UCEcran )
        {
            InitializeComponent();

            switch (UCEcran)
            {
                case "Piece":
                    uCCoutPiece = new UCCoutPiece();
                    Grid.SetRow(uCCoutPiece, 1);

                    grdCoutParent.Children.Add(uCCoutPiece);
                    btnVoirCoutProjet.IsEnabled = true;
                    break;
                case "Projet":
                    uCCoutProjet = new UCCoutProjet();
                    Grid.SetRow(uCCoutProjet, 1);

                    grdCoutParent.Children.Add(uCCoutProjet);
                    btnVoirCoutPiece.IsEnabled = true;
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

            uCCoutProjet = new UCCoutProjet();
            Grid.SetRow(uCCoutProjet, 1);

            grdCoutParent.Children.Add(uCCoutProjet);
            btnVoirCoutProjet.IsEnabled = false;
            btnVoirCoutPiece.IsEnabled = true;
        }

        private void btnVoirCoutPiece_Click(object sender, RoutedEventArgs e)
        {
            grdCoutParent.Children.Remove(uCCoutProjet);

            uCCoutPiece = new UCCoutPiece();
            Grid.SetRow(uCCoutPiece, 1);

            grdCoutParent.Children.Add(uCCoutPiece);
            btnVoirCoutProjet.IsEnabled = true;
            btnVoirCoutPiece.IsEnabled = false;
        }
    }
}
