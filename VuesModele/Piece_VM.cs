using App_Brycol.Modele;
using App_Brycol.Outils;
using App_Brycol.Vues;
using System;
using System.Collections.Generic;
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

        public void CreerPiece(Object param)
        {

            Piece p = new Piece();
            p.Projet = Projet_VM.ProjetActuel;

            //HARDCODE
            p.IdTypeDePiece = 6;
            //
            p.Largeur = Largeur;
            p.Longueur = Longueur;
            try
            {
                var numPiece = OutilEF.brycolContexte.Pieces.Max<Piece>(t => t.ID);
                numPiece += 1;
                p.Nom = "Piece" + numPiece;
            }
            catch (Exception e)
            {
                p.Nom = "Piece";

            }


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
