using App_Brycol.Outils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Brycol.Modele
{

    public class Piece
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
                Plan plan = new Plan();
                decimal ToPi = 0;
                const decimal TPS = 0.05M;
                const decimal TVQ = 0.09975M;
                //****************************************
                // HARDCODE LE ID
                var PReq = from p in OutilEF.brycolContexte.Plans where p.Piece.ID == 1 select p; 
                //****************************************
                foreach (Plan p in PReq)
                    plan = p;

                var LiReq = from Li in OutilEF.brycolContexte.lstItems.Include("Item") where Li.Plan.ID == plan.ID select Li;
                foreach (ItemsPlan Li in LiReq)
                    ToPi += Li.Item.Cout;

                //On retourne la valeur à deux chiffres après la virgule
                return decimal.Round(ToPi + (ToPi*TPS) + (ToPi*TVQ), 2, MidpointRounding.AwayFromZero); 
            }
            set { _total = value; }
        }

        public Piece() 
        {

        }
    }
}
