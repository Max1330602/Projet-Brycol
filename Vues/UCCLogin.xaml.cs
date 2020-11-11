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
    /// Logique d'interaction pour UCCLogin.xaml
    /// </summary>
    public partial class UCCLogin : UserControl
    {
        private UCInscrire uCInscrire { get; set; }

        public UCCLogin()
        {
            InitializeComponent();
            DataContext = new Utilisateur_VM();

            uCInscrire = new UCInscrire();
            Grid.SetRow(uCInscrire, 1);
        }

        private void btnInscrire_Click(object sender, RoutedEventArgs e)
        {
            Grid gridMW = (Grid)Application.Current.MainWindow.FindName("gridMainWindow");
            gridMW.Children.Clear();
            gridMW.Children.Add(uCInscrire);
        }

    }
}
