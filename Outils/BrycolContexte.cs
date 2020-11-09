using App_Brycol.Modele;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Brycol.Outils
{
    class BrycolContexte : DbContext
    {
        public DbSet<Item> Meubles { get; set; }
        public DbSet<ItemsPlan> lstItems { get; set; }
        public DbSet<Projet> Projets { get; set; }
        public DbSet<Categorie> Catego { get; set; }
        public DbSet<TypePiece> TypePiece { get; set; }
        public DbSet<Piece> Pieces { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Utilisateur> Utilisateurs { get; set; }

        public BrycolContexte() : base("name=connexionBrycol")
        {
            //Database.SetInitializer<BrycolContexte>(new DropCreateDatabaseAlways<BrycolContexte>());
            Database.SetInitializer<BrycolContexte>(new CreateDatabaseIfNotExists<BrycolContexte>());
        }
    }

}
