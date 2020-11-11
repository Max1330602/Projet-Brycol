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
    /// Logique d'interaction pour UCCMenuPrincipal.xaml
    /// </summary>
    public partial class UCCMenuPrincipal : UserControl
    {
        public UCCMenuPrincipal()
        {
            InitializeComponent();
            DataContext = new Projet_VM();
        }

        private void btnTeleverserProjet_Click(object sender, RoutedEventArgs e)
        {
            ChargerProjet popup = new ChargerProjet();
            popup.ShowDialog();
        }

    }
}
