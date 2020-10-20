using App_Brycol.Outils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Brycol.Modele
{

    class Piece
    {
        public int ID { get; set; }
        public Projet Projet { get; set; }
        public TypePiece TypePiece { get; set; }
        public float Largeur { get; set; }
        public float Longueur { get; set; }
        public string Nom { get; set; }
        [NotMapped]
        private decimal _total { get; set; }
        [NotMapped]
        public decimal Total
        {
            get
            {
                decimal ToPi = 0;
                const decimal TPS = 0.05M;
                const decimal TVQ = 0.09975M;

             /*   var PReq = from p in OutilEF.brycolContexte.Plans where p.Piece.ID == ID select p; 
                foreach (Plan p in PReq)
                {
                    var LiReq = from Li in OutilEF.brycolContexte.lstItems where Li.idPlan == p.ID select Li;
                    foreach (ItemsPlan Li in LiReq)
                    {
                        ToPi += Li.Item.Cout;
                    }
                }*/
                    return  ToPi /*+ (ToPi*TPS) + (ToPi*TVQ)*/; 
            }
            set { _total = value; }
        }

        public Piece() 
        {

        }
    }
}
