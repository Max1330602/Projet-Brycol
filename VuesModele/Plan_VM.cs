﻿using App_Brycol.Modele;
using App_Brycol.Outils;
using App_Brycol.Vues;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace App_Brycol.VuesModele
{
    class Plan_VM : INotifyPropertyChanged
    {

        public Plan_VM()
        {

        }

        

        public static Plan PlanActuel;

        public void InitPlan()
        {
            PlanActuel = new Plan();
            PlanActuel.Piece = Piece_VM.pieceActuel;
            OutilEF.brycolContexte.Plans.Add(PlanActuel);
            OutilEF.brycolContexte.SaveChanges();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string nomPropriete)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
            }
        }

    }
   
}
