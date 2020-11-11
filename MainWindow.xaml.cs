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
        private UCCLogin uCLogin { get; set; }

       public MainWindow()
        {
            InitializeComponent();
            OutilEF outilEF = new OutilEF();

            uCLogin = new UCCLogin();
            Grid.SetRow(uCLogin, 1);

            gridMainWindow.Children.Add(uCLogin);

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
