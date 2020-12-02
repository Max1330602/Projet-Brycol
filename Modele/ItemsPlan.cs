using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Brycol.Modele
{
    public class  ItemsPlan
    {
        public int ID { get; set; }
        public Plan Plan { get; set; }
        public Item Item { get; set; }
        public string Couleur { get; set; }
        public double emplacementGauche { get; set; }
        public double emplacementHaut { get; set; }
        public double angleRotation { get; set; }
        public string cotePorte { get; set; }
        public string EstPaye { get; set; }
        [NotMapped]
        public object Tag { get; set; }

        public ItemsPlan()
        {
        }
    }
}
