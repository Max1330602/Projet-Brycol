﻿using App_Brycol.VuesModele;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Logique d'interaction pour DimensionsPiece.xaml
    /// </summary>
    public partial class DimensionsPiece : Window
    {
        public DimensionsPiece()
        {
            InitializeComponent();

            DataContext = new Piece_VM();
        }

      

        private void btnContinuer_Click(object sender, RoutedEventArgs e)
        {

            Grid gridMW = (Grid)Application.Current.MainWindow.FindName("gridMainWindow");
            ContentPresenter cpMW = (ContentPresenter)Application.Current.MainWindow.FindName("presenteurContenu");
            this.Close();
            gridMW.Children.Clear();
            gridMW.Children.Add(cpMW);
            cpMW.Content = new PlanDeTravail();

        }

        private void txtLargeur_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtLargeur.Text = Regex.Replace(txtLargeur.Text, "[^0-9]+", "");
            string LargeurPiece = txtLargeur.Text;
            int LargeurPieceINT = int.Parse(LargeurPiece); //Success
        }

        private void txtLongueur_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtLongueur.Text = Regex.Replace(txtLongueur.Text, "[^0-9]+", "");
            string LongueurPiece = txtLongueur.Text;
            int LongueurPieceINT = int.Parse(LongueurPiece); //Success
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            //RETOUR ÉCRAN PROJET
            this.Close();
        }
    }
}
