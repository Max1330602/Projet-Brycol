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
    /// Logique d'interaction pour WarningSupPro.xaml
    /// </summary>
    public partial class WarningSupPro : Window
    {
        public WarningSupPro()
        {
            InitializeComponent();
            DataContext = new Projet_VM();
        }

        private void btnComfirm_Click(object sender, RoutedEventArgs e)
        {
            Projet_VM.EstSauvegarde = true;
            Application.Current.MainWindow.Close();

            this.Close();
        }

        private void btnRefus_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
