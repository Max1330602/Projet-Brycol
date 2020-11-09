using App_Brycol.Outils;
using App_Brycol.Vues;
using App_Brycol.VuesModele;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace App_Brycol
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


       public MainWindow()
        {
            InitializeComponent();
            this.MinWidth = 1000;
            this.MinHeight = 720;
            OutilEF outilEF = new OutilEF();
            DataContext = new Projet_VM();

        }

        private void btnTeleverserProjet_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Option de téléversement à venir!");
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            if (Projet_VM.ProjetActuel != null && Projet_VM.EstSauvegarde == false)
            {
                WarningProjetNonSau popUp = new WarningProjetNonSau();
                popUp.ShowDialog();


            }

        }
    }
}
