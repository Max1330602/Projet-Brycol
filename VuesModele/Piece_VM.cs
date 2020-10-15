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

        public ICommand cmdCreerPiece { get; set; }

        public Piece_VM()
        {
            cmdCreerPiece = new Commande(CreerPiece);
            TypesDePiece = new ObservableCollection<string>();
            var treq = from t in OutilEF.brycolContexte.TypePiece select t;

            foreach (TypePiece t in treq)
            {
                TypesDePiece.Add(t.Nom);
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

        public void CreerPiece(Object param)
        {

            Piece p = new Piece();
            p.Projet = Projet_VM.ProjetActuel;
            p.Nom = Nom;
            p.Largeur = Largeur;
            p.Longueur = Longueur;

            var treq = from t in OutilEF.brycolContexte.TypePiece where t.Nom == TypePiece select t;
            p.TypePiece = treq.First();
            OutilEF.brycolContexte.Pieces.Add(p);
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
