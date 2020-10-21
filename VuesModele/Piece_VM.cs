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
    class Piece_VM : INotifyPropertyChanged
    {

        public ICommand cmdPiece { get; set; }

        public static Piece pieceActuel;
        public string ParamOption;

        public Piece_VM(string paramOpt)
        {
            cmdPiece = new Commande(CmdPiece);
            TypesDePiece = new ObservableCollection<string>();
            var treq = from t in OutilEF.brycolContexte.TypePiece select t;

            foreach (TypePiece t in treq)
            {
                TypesDePiece.Add(t.Nom);
            }

            ParamOption = paramOpt;

            if (paramOpt == "Modifier")
            {
                Nom = pieceActuel.Nom;
                Longueur = pieceActuel.Longueur;
                Largeur = pieceActuel.Largeur;
                TypePiece = pieceActuel.TypePiece.Nom;
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

        private string _nom;
        public string Nom
        {
            get { return _nom; }
            set 
            {
                _nom = value;
                OnPropertyChanged("Nom");
            }
        }
        private ObservableCollection<string> _typesDePiece;
        public ObservableCollection<string> TypesDePiece
        { 
            get { return _typesDePiece; }
            set
            {
                _typesDePiece = value;
                OnPropertyChanged("TypesDePiece");
            }
        }

        private string _typePiece;
        public string TypePiece
        {
            get { return _typePiece; }
            set
            {
                _typePiece = value;
                OnPropertyChanged("TypePiece");
            }
        }

        public void CmdPiece(Object param)
        {
            if (ParamOption == "Ajouter")
                ajouterPiece();

            if (ParamOption == "Modifier")
                modifierPiece();

        }

        private void ajouterPiece()
        {
            Piece p = new Piece();
            p.Projet = Projet_VM.ProjetActuel;
            p.Nom = Nom;
            p.Largeur = Largeur;
            p.Longueur = Longueur;

            var treq = from t in OutilEF.brycolContexte.TypePiece where t.Nom == TypePiece select t;

            if (treq.Count() == 0)
                p.TypePiece = OutilEF.brycolContexte.TypePiece.Find(6);
            else
                p.TypePiece = treq.First();

            if (Nom == null)
            {
                var test = OutilEF.brycolContexte.Pieces.Max<Piece>(t => t.ID);
                test += 1;
                p.Nom = "Piece" + test;
            }

            OutilEF.brycolContexte.Pieces.Add(p);
            OutilEF.brycolContexte.SaveChanges();

            pieceActuel = p;
            Plan_VM pVM = new Plan_VM();
            pVM.InitPlan();

        }

        private void modifierPiece()
        {
            Piece p = OutilEF.brycolContexte.Pieces.Find(pieceActuel.ID);
            p.Nom = Nom;
            p.Largeur = Largeur;
            p.Longueur = Longueur;

            var treq = from t in OutilEF.brycolContexte.TypePiece where t.Nom == TypePiece select t;

            if (treq.Count() == 0)
                p.TypePiece = OutilEF.brycolContexte.TypePiece.Find(6);
            else
                p.TypePiece = treq.First();

            if (Nom == null)
            {
                var test = OutilEF.brycolContexte.Pieces.Max<Piece>(t => t.ID);
                test += 1;
                p.Nom = "Piece" + test;
            }

            OutilEF.brycolContexte.SaveChanges();

            pieceActuel = p;
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
