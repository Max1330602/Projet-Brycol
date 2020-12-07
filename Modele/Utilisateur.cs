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
    public class Utilisateur
    {
        public int ID { get; set; }
        public string Nom { get; set; }
        public string MotPasse { get; set; }
        [NotMapped]
        public ObservableCollection<Facture> ListeFactures { get; set; }

        public Utilisateur() 
        { 
            ListeFactures = new ObservableCollection<Facture>();
        }
    }
}
