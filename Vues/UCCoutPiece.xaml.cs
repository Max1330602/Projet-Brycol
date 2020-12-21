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
    /// Logique d'interaction pour UCCoutPiece.xaml
    /// </summary>
    public partial class UCCoutPiece : UserControl
    {
        public UCCoutPiece()
        {
            InitializeComponent();

            if (Projet_VM.themeSombre)
                AppliquerThemeSombre();
            else
                EnleverThemeSombre();

            DataContext = new Piece_VM("");
            lblNomPiece.Content = Piece_VM.pieceSelect.Nom;
            lblSousTotal.Content = Piece_VM.SousTotal.ToString() + "$";
            lblTPS.Content = Piece_VM.TpsDePiece.ToString() + "$";
            lblTVQ.Content = Piece_VM.TvqDePiece.ToString() + "$";
            lblTotal.Content = Piece_VM.Total.ToString() + "$";
        }

        private void EnleverThemeSombre()
        {
            BrushConverter bc = new BrushConverter();
            Brush Couleur = (Brush)bc.ConvertFrom("#4DAFFF");

            DG_coutItem.Background = Brushes.White;

            DG_coutItem.RowBackground = Couleur;
            DG_coutItem.AlternatingRowBackground = Brushes.White; 
        }

        private void AppliquerThemeSombre()
        {
            BrushConverter bc = new BrushConverter();
            Brush CouleurArrierePlan = (Brush)bc.ConvertFrom("#7D7E79");

            DG_coutItem.Background = CouleurArrierePlan;

            DG_coutItem.RowBackground = Brushes.LightGray;
            DG_coutItem.AlternatingRowBackground = Brushes.Gray;
        }
    }
}
