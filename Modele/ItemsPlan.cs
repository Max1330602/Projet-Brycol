using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public double emplacementGauche { get; set; }
        public double emplacementHaut { get; set; }

        public ItemsPlan()
        {
        }
    }
}
