using App_Brycol.Modele;
using App_Brycol.Outils;
using App_Brycol.Vues;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace App_Brycol.VuesModele 
{
    class Piece_VM : INotifyPropertyChanged
    {

        public ICommand cmdPiece { get; set; }
        public static Piece pieceActuel;
        public static Piece pieceSelect;
        public static decimal SousTotal;
        public static decimal TpsDePiece;
        public static decimal TvqDePiece;
        public static decimal Total;
        public string ParamOption;

        public Piece_VM(string paramOpt)
        {
            SousTotal = 0;
            TpsDePiece = 0;
            TvqDePiece = 0;
            Total = 0;

            cmdPiece = new Commande(CmdPiece);
            TypesDePiece = new ObservableCollection<string>();
            var treq = from t in OutilEF.brycolContexte.TypePiece select t;
            foreach (TypePiece t in treq)
            {
                TypesDePiece.Add(t.Nom);
            }

            TypesDePlancher = new ObservableCollection<string>();
            var tPlancherReq = from t in OutilEF.brycolContexte.TypePlancher select t;
            foreach (TypePlancher t in tPlancherReq)
            {
                TypesDePlancher.Add(t.Nom);
            }

            ParamOption = paramOpt;

            if (paramOpt == "Modifier")
            {
                Nom = pieceActuel.Nom;
                Longueur = pieceActuel.Longueur;
                Largeur = pieceActuel.Largeur;
                TypePiece = pieceActuel.TypePiece.Nom;
                TypePlancher = pieceActuel.TypePlancher.Nom;
            }

            ListeItems = new ObservableCollection<Item>();

            if (pieceSelect == null)
                pieceSelect = pieceActuel;


            if (pieceSelect != null)
            {
                Plan plan = new Plan();

                var PReq = from p in OutilEF.brycolContexte.Plans where p.Piece.ID == pieceSelect.ID select p;

                foreach (Plan p in PReq)
                    plan = p;

                var LiReq = from Li in OutilEF.brycolContexte.lstItems.Include("Item") where Li.Plan.ID == plan.ID select Li;
                foreach (ItemsPlan Li in LiReq)
                    ListeItems.Add(Li.Item);


                SousTotal = CalSouTo(pieceSelect);
                TpsDePiece = CalTPS(SousTotal);
                TvqDePiece = CalTVQ(SousTotal);
                Total = CalTotal(SousTotal, TpsDePiece, TvqDePiece);
                pieceSelect.Total = Total;


            }
        }
        #region Propriétés

        private float _longueur;
        public float Longueur 
        {
            get { return _longueur; }

            set 
            {
                _longueur = value;
                OnPropertyChanged("Longueur");
            }
        }

        private float _largeur;
        public float Largeur
        {
            get { return _largeur; }

            set
            {
                _largeur = value;
                OnPropertyChanged("Largeur");
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
        private ObservableCollection<string> _typesDePiece;
        public ObservableCollection<string> TypesDePiece
        { 
            get { return _typesDePiece; }
            set
            {
                _typesDePiece = value;
                OnPropertyChanged("TypesDePiece");
            }
        }

        private ObservableCollection<string> _typesDePlancher;
        public ObservableCollection<string> TypesDePlancher
        {
            get { return _typesDePlancher; }
            set
            {
                _typesDePlancher = value;
                OnPropertyChanged("TypesDePlancher");
            }
        }

        private string _typePiece;
        public string TypePiece
        {
            get { return _typePiece; }
            set
            {
                _typePiece = value;
                OnPropertyChanged("TypePiece");
            }
        }

        private string _typePlancher;
        public string TypePlancher
        {
            get { return _typePlancher; }
            set
            {
                _typePlancher = value;
                OnPropertyChanged("TypePlancher");
            }
        }

        private ObservableCollection<Item> _listeItems;
        public ObservableCollection<Item> ListeItems
        {
            get { return _listeItems; }
            set
            {
                _listeItems = value;
                OnPropertyChanged("ListeItems");
            }
        }
        #endregion

        public void CmdPiece(Object param)
        {
            if (ParamOption == "Ajouter")
                ajouterPiece();

            if (ParamOption == "Modifier")
                modifierPiece();
        }

        private void ajouterPiece()
        {
            Piece p = new Piece();
            p.Projet = Projet_VM.ProjetActuel;
            p.Nom = Nom;
            p.Largeur = Largeur;
            p.Longueur = Longueur;
            p.Projet.ListePieces.Add(p);

            var treq = from t in OutilEF.brycolContexte.TypePiece where t.Nom == TypePiece select t;

            if (treq.Count() == 0)
                p.TypePiece = OutilEF.brycolContexte.TypePiece.Find(6);
            else
                p.TypePiece = treq.First();

            var tPlancherReq = from t in OutilEF.brycolContexte.TypePlancher where t.Nom == TypePlancher select t;

            if (tPlancherReq.Count() == 0)
                p.TypePlancher = OutilEF.brycolContexte.TypePlancher.Find(4);
            else
                p.TypePlancher = tPlancherReq.First();


            if (Nom == null)
            {
                var test = OutilEF.brycolContexte.Pieces.Max<Piece>(t => t.ID);
                test += 1;
                p.Nom = "Piece" + test;
            }

            OutilEF.brycolContexte.Pieces.Add(p);
            OutilEF.brycolContexte.SaveChanges();

            pieceActuel = p;
            Plan_VM pVM = new Plan_VM();
            pVM.InitPlan();


            PlanDeTravail popup = new PlanDeTravail();
            popup.ShowDialog();

        }

        private void modifierPiece()
        {
            Piece p = OutilEF.brycolContexte.Pieces.Find(pieceActuel.ID);
            p.Nom = Nom;
            p.Largeur = Largeur;
            p.Longueur = Longueur;

            var treq = from t in OutilEF.brycolContexte.TypePiece where t.Nom == TypePiece select t;

            if (treq.Count() == 0)
                p.TypePiece = OutilEF.brycolContexte.TypePiece.Find(6);
            else
                p.TypePiece = treq.First();

            var tPlancherReq = from t in OutilEF.brycolContexte.TypePlancher where t.Nom == TypePlancher select t;

            if (tPlancherReq.Count() == 0)
                p.TypePlancher = OutilEF.brycolContexte.TypePlancher.Find(4);
            else
                p.TypePlancher = tPlancherReq.First();


            if (Nom == null)
            {
                var test = OutilEF.brycolContexte.Pieces.Max<Piece>(t => t.ID);
                test += 1;
                p.Nom = "Piece" + test;
            }

            OutilEF.brycolContexte.SaveChanges();
            pieceActuel = p;

            PlanDeTravail popup = new PlanDeTravail();
            popup.ShowDialog();

            //---TODO-----------------------------------------------------------------------
            var plreq = from pl in OutilEF.brycolContexte.Plans.Include("Piece") where pl.Piece.ID == pieceActuel.ID select pl;
            Plan_VM.PlanActuel = plreq.First();
            
        }

        public static decimal CalSouTo(Piece laPiece)
        {
            Plan plan = new Plan();
            decimal St = 0M;

            var PReq = from p in OutilEF.brycolContexte.Plans where p.Piece.ID == laPiece.ID select p;

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

        public static void supprimerPiece()
        {
            Piece p = OutilEF.brycolContexte.Pieces.Find(pieceActuel.ID);

            OutilEF.brycolContexte.Pieces.Remove(p);
            OutilEF.brycolContexte.SaveChanges();

            Projet_VM.ProjetActuel.ListePieces.Remove(pieceActuel);
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
