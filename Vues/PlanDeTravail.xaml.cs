using App_Brycol.Modele;
using App_Brycol.VuesModele;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Resources;
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
using Ubiety.Dns.Core;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace App_Brycol.Vues
{
    /// <summary>
    /// Logique d'interaction pour PlanDeTravail.xaml
    /// </summary>
    public partial class PlanDeTravail : Window
    {
        public PlanDeTravail()
        {
            InitializeComponent();

        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (Projet_VM.ProjetActuel != null && Projet_VM.EstSauvegarde == false)
            {
                WarningProjetNonSau popUp = new WarningProjetNonSau();
                popUp.ShowDialog();
            }
            Projet_VM.planOuvert = false;
            Projet_VM.ProjetActuel.ListePieces.Clear();
            Projet_VM.ProjetActuel.ListePlans.Clear();
            Projet_VM.ProjetActuel = null;
            Piece_VM.pieceActuel = null;
            Plan_VM.PlanActuel = null;
            Plan_VM.uniteDeMesure = "";

        }
    }
}