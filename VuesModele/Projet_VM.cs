using App_Brycol.Modele;
using App_Brycol.Outils;
using App_Brycol.Vues;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace App_Brycol.VuesModele
{
    class Projet_VM : INotifyPropertyChanged
    {

        public static Projet ProjetActuel;
        public static bool EstSauvegarde = false;
        public ICommand cmdCreerProjet { get; set; }
        public ICommand cmdSauvProjet { get; set; }
        public ICommand cmdSuppProjet { get; set; }

        public Projet_VM()
        {
            cmdCreerProjet = new Commande(CreerProjet);
            cmdSauvProjet = new Commande(SauvProjet);
            cmdSuppProjet = new Commande(SuppProjet);

            ListePieces = new ObservableCollection<Piece>();

            if (ProjetActuel != null)
            {
                var PReq = from p in OutilEF.brycolContexte.Pieces where p.Projet.ID == ProjetActuel.ID select p;
                foreach (Piece p in PReq)
                    ListePieces.Add(p);
            }


        }

        #region Propriétés

        private ObservableCollection<Piece> _listePieces;
        public ObservableCollection<Piece> ListePieces
        {
            get { return _listePieces; }
            set
            {
                _listePieces = value;
                OnPropertyChanged("ListePieces");
            }
        }

        private Piece _pieceSelectionnee;
        public Piece PieceSelectionnee
        {
            get { return _pieceSelectionnee; }
            set 
            {
                if (value != null)
                {
                    _pieceSelectionnee = value;

                    Piece_VM.pieceSelect = _pieceSelectionnee;
                }      
                OnPropertyChanged("PieceSelectionnee");
            }
        }

        private string _nom;
        public string Nom
        {
            get { return _nom; }
            set 
            {
                _nom = value;
                OnPropertyChanged("Nom");
            }
        }

        #endregion

        public void CreerProjet(Object param)
        {
            Projet p = new Projet();
            try
            {
                var test = OutilEF.brycolContexte.Projets.Max<Projet>(t => t.ID);
                test += 1;
                p.Nom = "Projet" + test;
            } catch (Exception e)
            {
                p.Nom = "Projet";
            }
            p.Createur = "Utilisateur";
            p.ListePieces = ListePieces;
            OutilEF.brycolContexte.Projets.Add(p);
            OutilEF.brycolContexte.SaveChanges();
            ProjetActuel = p;

            GererProjet popUp = new GererProjet();
            popUp.ShowDialog();

        }

        public void SauvProjet(Object param)
        {
            Projet p = OutilEF.brycolContexte.Projets.Find(ProjetActuel.ID);
            p.Nom = Nom;

            if (Nom == null)
            {
                var test = OutilEF.brycolContexte.Projets.Max<Projet>(t => t.ID);
                test += 1;
                p.Nom = "Projet" + test;
            }

            OutilEF.brycolContexte.SaveChanges();

            ProjetActuel = p;
            EstSauvegarde = true;

        }

        public void SuppProjet(Object param)
        {
            Projet pro = new Projet();
            List<Piece> lstPie = new List<Piece>();
            Plan pla = new Plan();
            List<ItemsPlan> lstItPla = new List<ItemsPlan>();

            pro = OutilEF.brycolContexte.Projets.Find(ProjetActuel.ID);
            var PieReq = from pie in OutilEF.brycolContexte.Pieces where pie.Projet.ID == pro.ID select pie;

            foreach (Piece pie in PieReq)
                lstPie.Add(pie);

            foreach (Piece pie in lstPie)
            {

                var PlanReq = from plan in OutilEF.brycolContexte.Plans where plan.Piece.ID == pie.ID select plan;

                foreach (Plan plan in PlanReq)
                    pla = plan;
                
                var IteReq = from ite in OutilEF.brycolContexte.lstItems where ite.Plan.ID == pla.ID select ite;

                foreach (ItemsPlan itPl in IteReq)
                    lstItPla.Add(itPl);

                foreach (ItemsPlan itPl in lstItPla)
                    OutilEF.brycolContexte.lstItems.Remove(itPl);

                OutilEF.brycolContexte.Plans.Remove(pla);
                

                OutilEF.brycolContexte.Pieces.Remove(pie);
            }
            OutilEF.brycolContexte.Projets.Remove(pro);
            OutilEF.brycolContexte.SaveChanges();
        }

        public static decimal CalSouTo(Piece laPiece)
        {
            Plan plan = new Plan();
            decimal St = 0M;

            //****************************************
            // HARDCODE LE ID                                                      ICI laPiece
            var PReq = from p in OutilEF.brycolContexte.Plans where p.Piece.ID == laPiece.ID select p;
            //****************************************
            foreach (Plan p in PReq)
                plan = p;

            var LiReq = from Li in OutilEF.brycolContexte.lstItems.Include("Item") where Li.Plan.ID == plan.ID select Li;
            foreach (ItemsPlan Li in LiReq)
                St += Li.Item.Cout;

            return St;

        }

        public static decimal CalTPS(decimal montant)
        {
            const decimal TPS = 0.05M;

            return decimal.Round((montant * TPS), 2, MidpointRounding.AwayFromZero);
        }

        public static decimal CalTVQ(decimal montant)
        {
            const decimal TVQ = 0.09975M;

            return decimal.Round((montant * TVQ), 2, MidpointRounding.AwayFromZero);
        }

        public static decimal CalTotal(decimal St, decimal montantTPS, decimal montantTVQ)
        {
            return decimal.Round((St + montantTPS + montantTVQ), 2, MidpointRounding.AwayFromZero);

        }

        public static decimal Total()
        {
            decimal SouPo = 0M;
            decimal ToPo = 0M;

            foreach (Piece p in ProjetActuel.ListePieces)
            {
                SouPo += CalSouTo(p);
                ToPo += CalTotal(SouPo, CalTPS(SouPo), CalTVQ(SouPo));
            }
            return ToPo;

        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string nomPropriete)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
            }
        }
    }
}
