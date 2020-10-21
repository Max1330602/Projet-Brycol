using App_Brycol.Outils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Brycol.Modele
{
    public class Projet
    {
        public int ID { get; set; }
        public string Nom { get; set; }
        public string Createur { get; set; }
        public ObservableCollection<Piece> ListePieces { get; set; }

        public Projet() 
        {

        }
    }
}
