﻿using App_Brycol.VuesModele;
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
    /// Logique d'interaction pour UCCoutDetailProjet.xaml
    /// </summary>
    public partial class UCCoutDetailProjet : UserControl
    {
        public UCCoutDetailProjet()
        {
            InitializeComponent();
            DataContext = new Projet_VM();
            DG_coutItem.Items.Refresh();

            lblNomProjet.Content = Projet_VM.ProjetActuel.Nom;
            lblTotal.Content = Projet_VM.Total().ToString() + "$";
        }
    }
}
