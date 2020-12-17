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
    /// Logique d'interaction pour UCCoutProjet.xaml
    /// </summary>
    public partial class UCCoutProjet : UserControl
    {
        public UCCoutProjet()
        {
            InitializeComponent();

            DataContext = new Projet_VM();
            DG_coutItem.Items.Refresh();

            if (Projet_VM.themeSombre)
                AppliquerThemeSombre();
            else
                EnleverThemeSombre();

            lblNomProjet.Content = Projet_VM.ProjetActuel.Nom;
            lblTotal.Content = Projet_VM.Total().ToString() + "$";
        }

        private void EnleverThemeSombre()
        {
            DG_coutItem.RowBackground = Brushes.CornflowerBlue;
            DG_coutItem.AlternatingRowBackground = Brushes.White;

            DG_coutItem.Background = Brushes.White;
        }

        private void AppliquerThemeSombre()
        {
            BrushConverter bc = new BrushConverter();
            Brush CouleurArrierePlan = (Brush)bc.ConvertFrom("#7D7E79");

            DG_coutItem.RowBackground = Brushes.LightGray;
            DG_coutItem.AlternatingRowBackground = Brushes.Gray;

            DG_coutItem.Background = CouleurArrierePlan;
        }
    }
}
