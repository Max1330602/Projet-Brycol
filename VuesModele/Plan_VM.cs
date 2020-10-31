using App_Brycol.Modele;
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
        public static Plan PlanActuel;
        public const float pixelToPied = (float)1152.0001451339 / echelle;
        public const float pixelToM = (float)3779.5275590551 / echelle;
        public const double pixelToCm = (float)37.7952755906 / echelle;
        public const int echelle = 50;
        public static bool validePourEnregistrer = true;

        public Plan_VM()
        {
           
            if(InfoPiece.uniteMesure == "Mètres")
            {
                Longueur = Piece_VM.pieceActuel.Longueur * pixelToM;
                Largeur = Piece_VM.pieceActuel.Largeur * pixelToM;
                LongueurAffichage = Piece_VM.pieceActuel.Longueur;
                LargeurAffichage = Piece_VM.pieceActuel.Largeur;
            }
            else
            {
                Longueur = Piece_VM.pieceActuel.Longueur * pixelToPied;
                Largeur = Piece_VM.pieceActuel.Largeur * pixelToPied;
                LongueurAffichage = Piece_VM.pieceActuel.Longueur;
                LargeurAffichage = Piece_VM.pieceActuel.Largeur;
            }
            UniteMesure = InfoPiece.uniteMesure;
            
        }
        private string _uniteMesure;
        public string UniteMesure
        {
            get { return _uniteMesure; }
            set
            {
                _uniteMesure = value;
                OnPropertyChanged("UniteMesure");
            }

        }

        private bool _validePourEnregistrer;
        public bool ValidePourEnregistrer
        {
            get { return _validePourEnregistrer; }
            set
            {
                _validePourEnregistrer = value;
                OnPropertyChanged("ValidePourEnregistrer");
            }

        }

        private float _longueur;
        public float Longueur
        {
            get { return _longueur; }
            set
            {
                _longueur = value;
                OnPropertyChanged("Longueur");
            }

        }

        private float _largeur;
        public float Largeur
        {
            get { return _largeur; }
            set
            {
                _largeur = value;
                OnPropertyChanged("Largeur");
            }

        }

        private float _longueurAffichage;
        public float LongueurAffichage
        {
            get { return _longueurAffichage; }
            set
            {
                _longueurAffichage = value;
                OnPropertyChanged("LongueurAffichage");
            }

        }

        private float _largeurAffichage;
        public float LargeurAffichage
        {
            get { return _largeurAffichage; }
            set
            {
                _largeurAffichage = value;
                OnPropertyChanged("LargeurAffichage");
            }

        }


        public void InitPlan()
        {
            PlanActuel = new Plan();
            PlanActuel.Piece = Piece_VM.pieceActuel;
            Projet_VM.ProjetActuel.ListePlans.Add(PlanActuel);
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

        public static void supprimerPlan()
        {
            List<ItemsPlan> lstItPla = new List<ItemsPlan>();

            var IteReq = from ite in OutilEF.brycolContexte.lstItems where ite.Plan.ID == PlanActuel.ID select ite;

            foreach (ItemsPlan itPl in IteReq)
                lstItPla.Add(itPl);

            foreach (ItemsPlan itPl in lstItPla)
                OutilEF.brycolContexte.lstItems.Remove(itPl);

            Plan p = OutilEF.brycolContexte.Plans.Find(PlanActuel.ID);

            OutilEF.brycolContexte.Plans.Remove(PlanActuel);
            OutilEF.brycolContexte.SaveChanges();

            Projet_VM.ProjetActuel.ListePlans.Remove(PlanActuel);
        }

    }
   
}
