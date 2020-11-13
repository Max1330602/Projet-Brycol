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
    /// Logique d'interaction pour Structures.xaml
    /// </summary>
    public partial class Structures : Window
    {
        public Structures()
        {
            InitializeComponent();
            DataContext = new Item_VM();
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
