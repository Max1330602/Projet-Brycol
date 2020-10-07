using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Brycol.Modele
{
    class Item
    {
        public int ID { get; set; }
        public int IdTypeDePiece { get; set; }
        public string Nom { get; set; }
        public float Largeur { get; set; }
        public float Longueur { get; set; }
        public float Hauteur { get; set; }
        public string Couleur { get; set; }
        public decimal Cout { get; set; }
        public string Fournisseur { get; set; }
        public string Commentaire { get; set; }

        public Item() { }

    }
}
