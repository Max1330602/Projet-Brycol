using App_Brycol.Outils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Brycol.Modele
{

    class Piece
    {
        public int ID { get; set; }
        public Projet Projet { get; set; }
        public int IdTypeDePiece { get; set; }
        public float Largeur { get; set; }
        public float Longueur { get; set; }
        public string Nom { get; set; }

        public Piece() 
        {

        }
    }
}
