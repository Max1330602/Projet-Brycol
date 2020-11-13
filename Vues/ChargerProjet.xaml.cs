﻿using App_Brycol.Outils;
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

            var pReq = (from p in OutilEF.brycolContexte.Projets.Include("Utilisateur") where p.Utilisateur.Nom == Utilisateur_VM.utilActuel.Nom  select p.Nom).ToList();

            if (Projet_VM.themeSombre)
                AppliquerThemeSombre();
            else
                EnleverThemeSombre();

            cmbProjets.ItemsSource = pReq;
        }

        private void EnleverThemeSombre()
        {
            Banniere.Background = Brushes.LightGray;
            lblProjet.Background = Brushes.White;

            btnOk.Background = Brushes.White;
            btnOk.Foreground = Brushes.Black;
        }

        private void AppliquerThemeSombre()
        {
            BrushConverter bc = new BrushConverter();
            Brush CouleurBouton = (Brush)bc.ConvertFrom("#45463F");
            Brush CouleurBanniere = (Brush)bc.ConvertFrom("#84857D");
            Brush CouleurArrierePlan = (Brush)bc.ConvertFrom("#7D7E79");

            Banniere.Background = CouleurBanniere;
            lblProjet.Background = CouleurArrierePlan;

            btnOk.Background = CouleurBouton;
            btnOk.Foreground = Brushes.White;
        }

        private void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            WarningSupPro popUp = new WarningSupPro();
            popUp.ShowDialog();

            var pReq = (from p in OutilEF.brycolContexte.Projets.Include("Utilisateur") where p.Utilisateur.Nom == Utilisateur_VM.utilActuel.Nom select p.Nom).ToList();

            cmbProjets.ItemsSource = pReq;

        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
