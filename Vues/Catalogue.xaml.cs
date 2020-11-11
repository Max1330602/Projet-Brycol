using App_Brycol.Modele;
using App_Brycol.Outils;
using App_Brycol.VuesModele;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Application = System.Windows.Application;

namespace App_Brycol.Vues
{
    /// <summary>
    /// Logique d'interaction pour Catalogue.xaml
    /// </summary>
    public partial class Catalogue : Window
    {
        public Catalogue()
        {
            InitializeComponent();
            DataContext = new Item_VM();
        }

        private void btnRetour_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            foreach (Window w in Application.Current.Windows)
            {
                if (w.Name == "wPlanDeTravail")
                {
                    w.Close();
                }
            }
            PlanDeTravail popup = new PlanDeTravail();
            popup.ShowDialog();
        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtPrixMin.Text = Regex.Replace(txtPrixMin.Text, "[^0-9]+", "");
            txtPrixMax.Text = Regex.Replace(txtPrixMax.Text, "[^0-9]+", "");

        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var vm = (Item_VM)DataContext;
            if (vm.CmdAjouterItem.CanExecute(null))
                vm.CmdAjouterItem.Execute(null);
        }
    }
}
