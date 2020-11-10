using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Brycol.Modele
{
    public class Utilisateur
    {
        public int ID { get; set; }
        public string Nom { get; set; }
        public string MotPasse { get; set; }

        public Utilisateur() { }
    }
}
