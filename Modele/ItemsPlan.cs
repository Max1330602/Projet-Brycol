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
        public int idPlan { get; set; }
        public int emplacementGauche { get; set; }
        public int emplacementHaut { get; set; }
        public Item Item { get; set; }

        public ItemsPlan()
        {
        }
    }
}
