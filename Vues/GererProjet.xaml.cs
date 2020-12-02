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
        private List<Image> lstImagePlans = new List<Image>();
        private List<Label> lstLabels = new List<Label>();
        private bool Encreation = false;

        public GererProjet()
        {
            InitializeComponent();
            this.MinHeight = 450;
            this.MinWidth = 800;

            DataContext = new Projet_VM();
            txtProjet.Text = Projet_VM.ProjetActuel.Nom;

            lstBoutons.Add(btnPiece1);
            lstBoutons.Add(btnPiece2);
            lstBoutons.Add(btnPiece3);
            lstBoutons.Add(btnPiece4);
            lstBoutons.Add(btnPiece5);
            lstBoutons.Add(btnPiece6);
            lstBoutons.Add(btnPiece7);
            lstBoutons.Add(btnPiece8);

            lstImagePlans.Add(imgPlan1);
            lstImagePlans.Add(imgPlan2);
            lstImagePlans.Add(imgPlan3);
            lstImagePlans.Add(imgPlan4);
            lstImagePlans.Add(imgPlan5);
            lstImagePlans.Add(imgPlan6);
            lstImagePlans.Add(imgPlan7);
            lstImagePlans.Add(imgPlan8);

            lstLabels.Add(txtPiece1);
            lstLabels.Add(txtPiece2);
            lstLabels.Add(txtPiece3);
            lstLabels.Add(txtPiece4);
            lstLabels.Add(txtPiece5);
            lstLabels.Add(txtPiece6);
            lstLabels.Add(txtPiece7);
            lstLabels.Add(txtPiece8);

            if (Projet_VM.themeSombre)
                AppliquerThemeSombre();
            else
                EnleverThemeSombre();
            


            for (int i = 0; i < Projet_VM.ProjetActuel.ListePieces.Count(); i++)
            {
                lstBoutons[i].IsEnabled = true;
                if (lstBoutons[i].IsEnabled == true)
                {
                    lstImagePlans[i].Source = Projet_VM.ProjetActuel.ListePlans[i].ImgPlan;
                    lstLabels[i].Content = Projet_VM.ProjetActuel.ListePieces[i].Nom;
                }
                else
                {
                    lstImagePlans[i].Source = null;
                    lstLabels[i].Content = "";
                }
            }

            if (Projet_VM.ProjetActuel.ListePieces.Count == 0)
            {
                btnCoutProjet.IsEnabled = false;
                if (Projet_VM.themeSombre)
                    AppliquerThemeSombre();
            }
            else
            {
                btnCoutProjet.IsEnabled = true;
                if (Projet_VM.themeSombre)
                    AppliquerThemeSombre();
            }
        }


        private void btnAjouterPiece_Click(object sender, RoutedEventArgs e)
        {
            Encreation = true;
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
            Encreation = false;
        }

        private void btnSupprimerPiece_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult resultat;
            resultat = MessageBox.Show("Voulez-vraiment supprimer cette pièce ?", "Suppression de pièce", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (resultat == MessageBoxResult.Yes)
            {
                Plan_VM.supprimerPlan();
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
                    {
                        if (Projet_VM.ProjetActuel.ListePieces[i] != null)
                        {
                            b.IsEnabled = true;
                            b.Background = new SolidColorBrush(Colors.White);
                        }
                    }
                    i++;
                }

                foreach (Image image in lstImagePlans)
                {
                    image.Source = null;
                }

                foreach (Label lbl in lstLabels)
                {
                    lbl.Content = "";
                }

                for (int i2 = 0; i2 < lstImagePlans.Count(); i2++)
                {
                    if (i2 < Projet_VM.ProjetActuel.ListePieces.Count())
                    {
                        lstImagePlans[i2].Source = Projet_VM.ProjetActuel.ListePlans[i2].ImgPlan;
                        lstLabels[i2].Content = Projet_VM.ProjetActuel.ListePieces[i2].Nom;
                    }
                }
                

            }

            if (Projet_VM.ProjetActuel.ListePieces.Count() == 0)
            {               
                MessageBox.Show("Vous devez créer une pièce pour visualiser le plan de travail", "Créer une pièce", MessageBoxButton.OK, MessageBoxImage.Information);
                InfoPiece popUp = new InfoPiece("Ajouter");
                popUp.ShowDialog();
                this.Close();
            }
        }

        private void btnPlan_Click(object sender, RoutedEventArgs e)
        {
            OuvrirPlan();
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
            Plan_VM.PlanActuel = Projet_VM.ProjetActuel.ListePlans[0];
        }

        private void btnPiece2_Click(object sender, RoutedEventArgs e)
        {
            selectionnerBouton(sender);
            Piece_VM.pieceActuel = Projet_VM.ProjetActuel.ListePieces[1];
            Plan_VM.PlanActuel = Projet_VM.ProjetActuel.ListePlans[1];
        }

        private void btnPiece3_Click(object sender, RoutedEventArgs e)
        {
            selectionnerBouton(sender);
            Piece_VM.pieceActuel = Projet_VM.ProjetActuel.ListePieces[2];
            Plan_VM.PlanActuel = Projet_VM.ProjetActuel.ListePlans[2];
        }

        private void btnPiece4_Click(object sender, RoutedEventArgs e)
        {
            selectionnerBouton(sender);
            Piece_VM.pieceActuel = Projet_VM.ProjetActuel.ListePieces[3];
            Plan_VM.PlanActuel = Projet_VM.ProjetActuel.ListePlans[3];
        }

        private void btnPiece5_Click(object sender, RoutedEventArgs e)
        {
            selectionnerBouton(sender);
            Piece_VM.pieceActuel = Projet_VM.ProjetActuel.ListePieces[4];
            Plan_VM.PlanActuel = Projet_VM.ProjetActuel.ListePlans[4];
        }

        private void btnPiece6_Click(object sender, RoutedEventArgs e)
        {
            selectionnerBouton(sender);
            Piece_VM.pieceActuel = Projet_VM.ProjetActuel.ListePieces[5];
            Plan_VM.PlanActuel = Projet_VM.ProjetActuel.ListePlans[5];
        }

        private void btnPiece7_Click(object sender, RoutedEventArgs e)
        {
            selectionnerBouton(sender);
            Piece_VM.pieceActuel = Projet_VM.ProjetActuel.ListePieces[6];
            Plan_VM.PlanActuel = Projet_VM.ProjetActuel.ListePlans[6];
        }

        private void btnPiece8_Click(object sender, RoutedEventArgs e)
        {
            selectionnerBouton(sender);
            Piece_VM.pieceActuel = Projet_VM.ProjetActuel.ListePieces[7];
            Plan_VM.PlanActuel = Projet_VM.ProjetActuel.ListePlans[7];
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
                        btnPlan.IsEnabled = !btnPlan.IsEnabled;
                        btnSupprimerPiece.IsEnabled = !btnSupprimerPiece.IsEnabled;
                        if (Projet_VM.themeSombre)
                        {
                            AppliquerThemeSombre();
                        }
                        b.Background = new SolidColorBrush(Colors.LightBlue);
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
                            if (Projet_VM.themeSombre)
                            {
                                AppliquerThemeSombre();
                                btnSupprimerPiece.Background = Brushes.White;
                                btnSupprimerPiece.Foreground = Brushes.Black;

                                btnPlan.Background = Brushes.White;
                                btnPlan.Foreground = Brushes.Black;
                            }
                            else
                            {
                                EnleverThemeSombre();
                            }
                            btnPlan.IsEnabled = !btnPlan.IsEnabled;
                            btnSupprimerPiece.IsEnabled = !btnSupprimerPiece.IsEnabled;
                            
                        }
                    i++;
                }
            }
        }

        private void txtProjet_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtProjet.Text.Length < 2 || txtProjet.Text.Length > 30)
            {
                MessageBox.Show("La longueur du nom du projet n'est pas valide.");
                return;
            }
            if (txtProjet.Text != Projet_VM.ProjetActuel.Nom)
            {
                btnModifNom.Visibility = Visibility.Visible;
                return;
            }
            btnModifNom.Visibility = Visibility.Collapsed;

        }

        private void btnModifNom_Click(object sender, RoutedEventArgs e)
        {
            bool estExistant = false;

            var pReq = from pr in OutilEF.brycolContexte.Projets.Include("Utilisateur") where pr.Nom == txtProjet.Text select pr;
            foreach (Projet pr in pReq)
            {
                if (pr.Utilisateur == Utilisateur_VM.utilActuel && Projet_VM.ProjetActuel.ID != pr.ID)
                    estExistant = true;
            }
            if (estExistant)
            {
                MessageBox.Show("Vous avez déjà un projet qui se nomme " + txtProjet.Text + ".");
                txtProjet.Text = Projet_VM.ProjetActuel.Nom;
                btnModifNom.Visibility = Visibility.Collapsed;
                return;
            }
            Projet p = OutilEF.brycolContexte.Projets.Find(Projet_VM.ProjetActuel.ID);
            Projet_VM.ProjetActuel.Nom = txtProjet.Text;
            p.Nom = Projet_VM.ProjetActuel.Nom;
            OutilEF.brycolContexte.SaveChanges();

            btnModifNom.Visibility = Visibility.Collapsed;
        }

        private void btnPiece1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Piece_VM.pieceActuel = Projet_VM.ProjetActuel.ListePieces[0];
            Plan_VM.PlanActuel = Projet_VM.ProjetActuel.ListePlans[0];
            OuvrirPlan();
        }

        private void btnPiece2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Piece_VM.pieceActuel = Projet_VM.ProjetActuel.ListePieces[1];
            Plan_VM.PlanActuel = Projet_VM.ProjetActuel.ListePlans[1];
            OuvrirPlan();
        }

        private void btnPiece3_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Piece_VM.pieceActuel = Projet_VM.ProjetActuel.ListePieces[2];
            Plan_VM.PlanActuel = Projet_VM.ProjetActuel.ListePlans[2];
            OuvrirPlan();
        }

        private void btnPiece4_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Piece_VM.pieceActuel = Projet_VM.ProjetActuel.ListePieces[3];
            Plan_VM.PlanActuel = Projet_VM.ProjetActuel.ListePlans[3];
            OuvrirPlan();
        }

        private void btnPiece5_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Piece_VM.pieceActuel = Projet_VM.ProjetActuel.ListePieces[4];
            Plan_VM.PlanActuel = Projet_VM.ProjetActuel.ListePlans[4];
            OuvrirPlan();
        }

        private void btnPiece6_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Piece_VM.pieceActuel = Projet_VM.ProjetActuel.ListePieces[5];
            Plan_VM.PlanActuel = Projet_VM.ProjetActuel.ListePlans[5];
            OuvrirPlan();
        }

        private void btnPiece7_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Piece_VM.pieceActuel = Projet_VM.ProjetActuel.ListePieces[6];
            Plan_VM.PlanActuel = Projet_VM.ProjetActuel.ListePlans[6];
            OuvrirPlan();
        }

        private void btnPiece8_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Piece_VM.pieceActuel = Projet_VM.ProjetActuel.ListePieces[7];
            Plan_VM.PlanActuel = Projet_VM.ProjetActuel.ListePlans[7];
            OuvrirPlan();
        }

        private void OuvrirPlan()
        {
            this.Close();
            foreach (Window w in Application.Current.Windows)
            {
                if (w.GetType() == typeof(PlanDeTravail))
                {
                    Projet_VM.planOuvert = true;
                    (w as PlanDeTravail).grdPlanTravail.Children.Clear();
                    (w as PlanDeTravail).grdPlanTravail.Children.Add(new PlanDeTravail2());
                }
            }
            if (!Projet_VM.planOuvert)
            {
                PlanDeTravail PlanDeTravail = new PlanDeTravail();
                PlanDeTravail.grdPlanTravail.Children.Clear();
                PlanDeTravail.grdPlanTravail.Children.Add(new PlanDeTravail2());
                PlanDeTravail.ShowDialog();
            }
        }

        private void btnThemeSombre_Click(object sender, RoutedEventArgs e)
        {
            if (!Projet_VM.themeSombre)
            {
                Projet_VM.themeSombre = !Projet_VM.themeSombre;
                AppliquerThemeSombre();
                foreach(Window w in Application.Current.Windows)
                {
                    if (w.GetType() == typeof(PlanDeTravail))
                    {
                        (w as PlanDeTravail).grdPlanTravail.Children.Clear();
                        (w as PlanDeTravail).grdPlanTravail.Children.Add(new PlanDeTravail2());
                    }
                    else if (w.GetType() == typeof(MainWindow))
                    {
                        (w as MainWindow).gridMainWindow.Children.Clear();
                        (w as MainWindow).gridMainWindow.Children.Add(new UCCMenuPrincipal());
                    }
                }
            }
            else
            {
                Projet_VM.themeSombre = !Projet_VM.themeSombre;
                EnleverThemeSombre();

                foreach (Window w in Application.Current.Windows)
                {
                    if (w.GetType() == typeof(PlanDeTravail))
                    {
                        (w as PlanDeTravail).grdPlanTravail.Children.Clear();
                        (w as PlanDeTravail).grdPlanTravail.Children.Add(new PlanDeTravail2());
                    }
                    else if (w.GetType() == typeof(MainWindow))
                    {
                        (w as MainWindow).gridMainWindow.Children.Clear();
                        (w as MainWindow).gridMainWindow.Children.Add(new UCCMenuPrincipal());
                    }
                }
            }

        }

        private void AppliquerThemeSombre()
        {
            BrushConverter bc = new BrushConverter();
            Brush CouleurBouton = (Brush)bc.ConvertFrom("#45463F");
            Brush CouleurArriere = (Brush)bc.ConvertFrom("#33342F");
            Brush CouleurBanniere = (Brush)bc.ConvertFrom("#84857D");

            btnAjouterPiece.Background = CouleurBouton;
            btnAjouterPiece.Foreground = Brushes.White;

            if (btnPlan.IsEnabled && btnSupprimerPiece.IsEnabled && btnCoutProjet.IsEnabled)
            {
                btnPlan.Background = CouleurBouton;
                btnPlan.Foreground = Brushes.White;

                btnSupprimerPiece.Background = CouleurBouton;
                btnSupprimerPiece.Foreground = Brushes.White;

                btnCoutProjet.Background = CouleurBouton;
                btnCoutProjet.Foreground = Brushes.White;
            }
            else if (!btnPlan.IsEnabled && !btnSupprimerPiece.IsEnabled && btnCoutProjet.IsEnabled)
            {
                btnCoutProjet.Background = CouleurBouton;
                btnCoutProjet.Foreground = Brushes.White;
            }
            
            foreach (Button b in lstBoutons)
            {
                b.Background = CouleurBouton;
                b.Foreground = Brushes.White;
            }

            foreach (Label l in lstLabels)
            {
                if (l.Content != null)
                {
                    l.Background = CouleurBouton;
                    l.Foreground = Brushes.White;
                }
            }

            btnThemeSombre.Background = CouleurBouton;

            btnQuitter.Background = CouleurBouton;
            btnQuitter.Foreground = Brushes.White;

            txtProjet.Background = CouleurBanniere;
            Banniere.Background = CouleurBanniere;
            arriereBoutons.Background = CouleurArriere;
        }

        private void EnleverThemeSombre()
        {

            btnAjouterPiece.Background = Brushes.White;
            btnAjouterPiece.Foreground = Brushes.Black;

            if (btnPlan.IsEnabled && btnSupprimerPiece.IsEnabled && btnCoutProjet.IsEnabled)
            {
                btnPlan.Background = Brushes.White;
                btnPlan.Foreground = Brushes.Black;

                btnSupprimerPiece.Background = Brushes.White;
                btnSupprimerPiece.Foreground = Brushes.Black;

                btnCoutProjet.Background = Brushes.White;
                btnCoutProjet.Foreground = Brushes.Black;
            }
            else if (!btnPlan.IsEnabled && !btnSupprimerPiece.IsEnabled && btnCoutProjet.IsEnabled)
            {
                btnCoutProjet.Background = Brushes.White;
                btnCoutProjet.Foreground = Brushes.Black;
            }

            foreach (Button b in lstBoutons)
            {
                b.Background = Brushes.White;
                b.Foreground = Brushes.Black;
            }

            foreach (Label l in lstLabels)
            {
                if (l.Content != null)
                {
                    l.Background = Brushes.White;
                    l.Foreground = Brushes.Black;
                }
            }

            btnThemeSombre.Background = Brushes.White;

            btnQuitter.Background = Brushes.White;
            btnQuitter.Foreground = Brushes.Black;

            txtProjet.Background = Brushes.LightGray;
            Banniere.Background = Brushes.LightGray;
            arriereBoutons.Background = Brushes.White;

        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Projet_VM.ProjetActuel.ListePieces.Count == 0 && !Encreation)
            {
                if (Projet_VM.ProjetActuel != null && Projet_VM.EstSauvegarde == false)
                {
                    WarningProjetNonSau popUp = new WarningProjetNonSau();
                    popUp.ShowDialog();
                }
                Projet_VM.planOuvert = false;
                Projet_VM.ProjetActuel.ListePieces.Clear();
                Projet_VM.ProjetActuel.ListePlans.Clear();
                Projet_VM.ProjetActuel = null;
                Projet_VM.EstSauvegarde = false;
                Piece_VM.pieceActuel = null;
                Plan_VM.PlanActuel = null;
                Plan_VM.uniteDeMesure = "";


                Grid gridMW = (Grid)Application.Current.MainWindow.FindName("gridMainWindow");
                ContentPresenter cpMW = (ContentPresenter)Application.Current.MainWindow.FindName("presenteurContenu");
                gridMW.Children.Clear();
                gridMW.Children.Add(cpMW);
                cpMW.Content = new UCCMenuPrincipal();
            }
        }

    }
}
