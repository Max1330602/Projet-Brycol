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
    /// Logique d'interaction pour ChargerProjet.xaml
    /// </summary>
    public partial class ChargerProjet : Window
    {
        public ChargerProjet()
        {
            InitializeComponent();
            DataContext = new Projet_VM();

            var pReq = (from p in OutilEF.brycolContexte.Projets select p.Nom).ToList();

            cmbProjets.ItemsSource = pReq;
        }
    }
}
