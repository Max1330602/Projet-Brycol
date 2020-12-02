using App_Brycol.Outils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Brycol.Modele
{
    public class Facture
    {
        public int ID { get; set; }
        public Utilisateur Utilisateur { get; set; }
        public string Fournisseur { get; set; }
        public decimal Montant { get; set; } 

        public Facture()
        {

        }
    }
}
