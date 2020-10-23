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
    /// Logique d'interaction pour Projet.xaml
    /// </summary>
    public partial class GererProjet : Window
    {
        private List<Button> lstBoutons = new List<Button>();

        public GererProjet()
        {
            InitializeComponent();

            lstBoutons.Add(btnPiece1);
            lstBoutons.Add(btnPiece2);
            lstBoutons.Add(btnPiece3);
            lstBoutons.Add(btnPiece4);
            lstBoutons.Add(btnPiece5);
            lstBoutons.Add(btnPiece6);
            lstBoutons.Add(btnPiece7);
            lstBoutons.Add(btnPiece8);
            
            for (int i = 0; i < Projet_VM.ProjetActuel.ListePieces.Count(); i++)
            {
                lstBoutons[i].IsEnabled = true;
            }
        }

        private void btnAjouterPiece_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            if (Projet_VM.ProjetActuel.ListePieces.Count() < 8)
            {
                InfoPiece popUp = new InfoPiece("Ajouter");
                popUp.ShowDialog();
            }
            else
            {
                MessageBox.Show("Déjà 8 pièces sont liées à ce projet", "Maximum de pièces atteint", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSupprimerPiece_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult resultat;
            resultat = MessageBox.Show("Voulez-vraiment supprimer cette pièce ?", "Suppression de pièce", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (resultat == MessageBoxResult.Yes)
            {
                Piece_VM.supprimerPiece();

                btnSupprimerPiece.IsEnabled = false;
                btnPlan.IsEnabled = false;

                foreach (Button b in lstBoutons)
                {
                    b.IsEnabled = false;
                }

                int i = 0;
                foreach (Button b in lstBoutons)
                {
                    if (i < Projet_VM.ProjetActuel.ListePieces.Count())
                        if (Projet_VM.ProjetActuel.ListePieces[i] != null)
                        {
                            b.IsEnabled = true;
                        }
                    i++;
                }

            }
        }

        private void btnPlan_Click(object sender, RoutedEventArgs e)
        {
            Grid gridMW = (Grid)Application.Current.MainWindow.FindName("gridMainWindow");
            ContentPresenter cpMW = (ContentPresenter)Application.Current.MainWindow.FindName("presenteurContenu");
            this.Close();
            gridMW.Children.Clear();
            gridMW.Children.Add(cpMW);
            cpMW.Content = new PlanDeTravail();
        }

        private void btnCoutProjet_Click(object sender, RoutedEventArgs e)
        {
            string UCEcran = "Projet";
            Cout popUp = new Cout(UCEcran);
            popUp.ShowDialog();
        }

        private void btnQuitter_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnPiece1_Click(object sender, RoutedEventArgs e)
        {
            selectionnerBouton(sender);
            Piece_VM.pieceActuel = Projet_VM.ProjetActuel.ListePieces[0];
        }

        private void btnPiece2_Click(object sender, RoutedEventArgs e)
        {
            selectionnerBouton(sender);
            Piece_VM.pieceActuel = Projet_VM.ProjetActuel.ListePieces[1];
        }

        private void btnPiece3_Click(object sender, RoutedEventArgs e)
        {
            selectionnerBouton(sender);
            Piece_VM.pieceActuel = Projet_VM.ProjetActuel.ListePieces[2];

        }

        private void btnPiece4_Click(object sender, RoutedEventArgs e)
        {
            selectionnerBouton(sender);
            Piece_VM.pieceActuel = Projet_VM.ProjetActuel.ListePieces[3];
        }

        private void btnPiece5_Click(object sender, RoutedEventArgs e)
        {
            selectionnerBouton(sender);
            Piece_VM.pieceActuel = Projet_VM.ProjetActuel.ListePieces[4];

        }

        private void btnPiece6_Click(object sender, RoutedEventArgs e)
        {
            selectionnerBouton(sender);
            Piece_VM.pieceActuel = Projet_VM.ProjetActuel.ListePieces[5];
        }

        private void btnPiece7_Click(object sender, RoutedEventArgs e)
        {
            selectionnerBouton(sender);
            Piece_VM.pieceActuel = Projet_VM.ProjetActuel.ListePieces[6];
        }

        private void btnPiece8_Click(object sender, RoutedEventArgs e)
        {
            selectionnerBouton(sender);
            Piece_VM.pieceActuel = Projet_VM.ProjetActuel.ListePieces[7];
        }

        private void selectionnerBouton(object sender)
        {
            if (!btnPlan.IsEnabled && !btnSupprimerPiece.IsEnabled)
            {
                foreach (Button b in lstBoutons)
                {
                    if (b != sender)
                    {
                        b.IsEnabled = false;
                    }
                    else
                    {
                        b.Background = new SolidColorBrush(Colors.LightBlue);
                        btnPlan.IsEnabled = !btnPlan.IsEnabled;
                        btnSupprimerPiece.IsEnabled = !btnSupprimerPiece.IsEnabled;
                    }

                }
            }
            else
            {
                int i = 0;
                foreach (Button b in lstBoutons)
                {
                    if (i < Projet_VM.ProjetActuel.ListePieces.Count())
                        if (b != sender && Projet_VM.ProjetActuel.ListePieces[i] != null)
                        {
                            b.IsEnabled = true;
                        }
                        else
                        {
                            b.Background = new SolidColorBrush(Colors.White);
                            btnPlan.IsEnabled = !btnPlan.IsEnabled;
                            btnSupprimerPiece.IsEnabled = !btnSupprimerPiece.IsEnabled;
                        }
                    i++;
                }
            }
        }
    }
}
