using App_Brycol.Modele;
using App_Brycol.Outils;
using App_Brycol.Vues;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace App_Brycol.VuesModele
{
    class Projet_VM
    {
        public Projet_VM()
        {
            cmdCreerProjet = new Commande(CreerProjet);
        }

        public ICommand cmdCreerProjet { get; set; }

        public void CreerProjet(Object param)
        {
            Projet p = new Projet();
            var test = OutilEF.brycolContexte.Projets.Max<Projet>(t => t.ID);
            test += 1;
            p.Nom = "Projet" + test;
            p.Createur = "Utilisateur";

            OutilEF.brycolContexte.Projets.Add(p);
            OutilEF.brycolContexte.SaveChanges();

            GererProjet popUp = new GererProjet();
            popUp.ShowDialog();

        }
    }
}
