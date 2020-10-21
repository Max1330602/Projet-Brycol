﻿using App_Brycol.Modele;
using App_Brycol.Outils;
using App_Brycol.Vues;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace App_Brycol.VuesModele
{
    class Projet_VM : INotifyPropertyChanged
    {
        public Projet_VM()
        {
            cmdCreerProjet = new Commande(CreerProjet);

            ListePieces = new ObservableCollection<Piece>();

            if (ProjetActuel != null)
            {
                var PReq = from p in OutilEF.brycolContexte.Pieces where p.Projet.ID == ProjetActuel.ID select p;
                foreach (Piece p in PReq)
                    ListePieces.Add(p);
            }

        }

        public static Projet ProjetActuel;
        public ICommand cmdCreerProjet { get; set; }
        public int ID { get; set; }

        private ObservableCollection<Piece> _listePieces;
        public ObservableCollection<Piece> ListePieces
        {
            get { return _listePieces; }
            set
            {
                _listePieces = value;
                OnPropertyChanged("ListePieces");
            }
        }

        private Piece _pieceSelectionnee;
        public Piece PieceSelectionnee
        {
            get { return _pieceSelectionnee; }
            set 
            {
                if (value != null)
                    _pieceSelectionnee = value;

                OnPropertyChanged("PieceSelectionnee");
            }
        }

        public void CreerProjet(Object param)
        {
            Projet p = new Projet();
            try
            {
                var test = OutilEF.brycolContexte.Projets.Max<Projet>(t => t.ID);
                test += 1;
                p.Nom = "Projet" + test;
            } catch (Exception e)
            {
                p.Nom = "Projet";
            }
            p.Createur = "Utilisateur";
            p.ListePieces = ListePieces;
            OutilEF.brycolContexte.Projets.Add(p);
            OutilEF.brycolContexte.SaveChanges();
            ProjetActuel = p;

            GererProjet popUp = new GererProjet();
            popUp.ShowDialog();

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
