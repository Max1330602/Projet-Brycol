using App_Brycol.Modele;
using App_Brycol.VuesModele;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Resources;
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
using Ubiety.Dns.Core;

namespace App_Brycol.Vues
{
    /// <summary>
    /// Logique d'interaction pour PlanDeTravail.xaml
    /// </summary>
    public partial class PlanDeTravail : Window
    {
        public PlanDeTravail()
        {
            InitializeComponent();

            DataContext = new Projet_VM();
            lblProjet.Content = "Projet : " + Projet_VM.ProjetActuel.Nom + "\t Pièce : " + Piece_VM.pieceActuel.Nom;
        }

        public static Catalogue Catalogue;
        private void btnAide_Click(object sender, RoutedEventArgs e)
        {
            Aide popUp = new Aide();
            popUp.ShowDialog();
        }

        private void btn2D_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn3D_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnCatalogue_Click(object sender, RoutedEventArgs e)
        {
            Catalogue popUp = new Catalogue();
            popUp.ShowDialog();
        }

        private void btnModifierItem_Click(object sender, RoutedEventArgs e)
        {
            if (Piece2D.draggedImage != null && Piece2D.draggedImage.Source != null)
            {
                ModifierItem popUp = new ModifierItem();
                popUp.ShowDialog();
            }
            else
            {
                MessageBox.Show("Il faut d'abord sélectionner un item dans le plan");
            }
        }
        

        private void btnSupprimerItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Voulez-vraiment supprimer cet item ?", "Suppression d'item", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
           
        }

        private void btnModifierPiece_Click(object sender, RoutedEventArgs e)
        {
            InfoPiece popUp = new InfoPiece("Modifier");
            popUp.ShowDialog();
        }

        private void btnProjet_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Projet_VM.ProjetActuel.ListePieces.Count(); i++)
            {
                if (Projet_VM.ProjetActuel.ListePieces[i] == Piece_VM.pieceActuel)
                {
                    System.Windows.Point PositionAbsolue = plan2D.TranslatePoint(new System.Windows.Point(162, 125), plan2D);
                    System.Windows.Point PositionEcran = this.PointToScreen(PositionAbsolue);

                    // Catch width and hight of scatterview
                    int width = (int)plan2D.ActualWidth;
                    int height = (int)plan2D.ActualHeight;

                    // Screenshot as bitmap TakeScreenshot(StartWert_X, StartWert_Y, BreiteBild, HöheBild)
                    Bitmap capture = Capturer((int)PositionEcran.X, (int)PositionEcran.Y, width, height);

                    // save image in stream
                    capture.Save("..\\..\\images\\Plans\\" + "plan" + Plan_VM.PlanActuel.ID + ".png");

                }
            }
            GererProjet popUp = new GererProjet();
            popUp.ShowDialog();
        }

        private void btnThemeSombre_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCoutPiece_Click(object sender, RoutedEventArgs e)
        {
            string UCEcran = "Piece";
            Piece_VM.pieceSelect = Piece_VM.pieceActuel;
            Cout popUp = new Cout(UCEcran);
            popUp.ShowDialog();
        }

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            Enregistrer popUp = new Enregistrer();
            popUp.ShowDialog();
        }

      

        private Bitmap Capturer(int DebutX, int DebutY, int Width, int Height)
        {
            // Bitmap in right size
            Bitmap Capture = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(Capture);
            // snip wanted area
            G.CopyFromScreen(DebutX, DebutY, 0, 0, new System.Drawing.Size(Width, Height), CopyPixelOperation.SourceCopy);


            // save uncompressed bitmap to disk
            string fileName = "..\\..\\images\\Plans\\" + "plan" + Plan_VM.PlanActuel.ID + ".png";
            System.IO.FileStream fs = System.IO.File.Open(fileName, System.IO.FileMode.OpenOrCreate);
            Capture.Save(fs, System.Drawing.Imaging.ImageFormat.Bmp);
            fs.Close();

            return Capture;

        }

        private void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            WarningSupPro popUp = new WarningSupPro();
            popUp.ShowDialog();

        }
    }
}
