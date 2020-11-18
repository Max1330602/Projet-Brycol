using App_Brycol.Modele;
using App_Brycol.Outils;
using App_Brycol.Vues;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace App_Brycol.VuesModele
{
    class Projet_VM : INotifyPropertyChanged
    {

        public static Projet ProjetActuel;
        public static bool EstSauvegarde = false;
        public static bool themeSombre = false;
        public static bool planOuvert = false;
        public ICommand cmdCreerProjet { get; set; }
        public ICommand cmdSauvProjet { get; set; }
        public ICommand cmdSuppProjet { get; set; }
        public ICommand cmdChargerProjet { get; set; }

        public Projet_VM()
        {
            cmdCreerProjet = new Commande(CreerProjet);
            cmdSauvProjet = new Commande(SauvProjet);
            cmdSuppProjet = new Commande(SuppProjet);
            cmdChargerProjet = new Commande(Charger);

            ListePieces = new ObservableCollection<Piece>();
            ListePlans = new ObservableCollection<Plan>();
            ListeItemPieceProjet = new ObservableCollection<ItemPieceProjet>();

            if (ProjetActuel != null)
            {
                var PReq = from p in OutilEF.brycolContexte.Pieces where p.Projet.ID == ProjetActuel.ID select p;
                foreach (Piece p in PReq)
                    ListePieces.Add(p);

                var PReq2 = from plan in OutilEF.brycolContexte.Plans where plan.Piece.Projet.ID == ProjetActuel.ID select plan;
                foreach (Plan plan in PReq2)
                    ListePlans.Add(plan);

                ListeItemPieceProjet = CreatlstIPP();

                foreach (Piece p in ListePieces)
                {
                    Piece_VM.SousTotal = Piece_VM.CalSouTo(p);
                    Piece_VM.TpsDePiece = Piece_VM.CalTPS(Piece_VM.SousTotal);
                    Piece_VM.TvqDePiece = Piece_VM.CalTVQ(Piece_VM.SousTotal);
                    Piece_VM.Total = Piece_VM.CalTotal(Piece_VM.SousTotal, Piece_VM.TpsDePiece, Piece_VM.TvqDePiece);
                    p.Total = Piece_VM.Total;
                }

                Nom = ProjetActuel.Nom;
            }


        }

        #region Propriétés

        private ObservableCollection<Plan> _listePlans;
        public ObservableCollection<Plan> ListePlans
        {
            get { return _listePlans; }
            set
            {
                _listePlans = value;
                OnPropertyChanged("ListePlans");
            }
        }

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

        private ObservableCollection<ItemPieceProjet> _listeItemPieceProjet;
        public ObservableCollection<ItemPieceProjet> ListeItemPieceProjet
        {
            get { return _listeItemPieceProjet; }
            set
            {
                _listeItemPieceProjet = value;
                OnPropertyChanged("ListeItemPieceProjet");
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

        private string _projetSelectionne;
        public string ProjetSelectionne
        {
            get { return _projetSelectionne; }
            set 
            {
                _projetSelectionne = value;
                OnPropertyChanged("ProjetSelectionne");
            }
        }
        #endregion

        public void Charger(Object param)
        {
            Projet proj = new Projet();
            var pReq = from p in OutilEF.brycolContexte.Projets where p.Nom == ProjetSelectionne select p;
            foreach (Projet pro in pReq)
                proj = pro;
            ProjetActuel = proj;
            ProjetActuel.ListePieces = new ObservableCollection<Piece>();
            ProjetActuel.ListePlans = new ObservableCollection<Plan>();
            var pieceReq = from piece in OutilEF.brycolContexte.Pieces.Include("TypePlancher").Include("TypePiece").Include("Projet") where piece.Projet.ID == ProjetActuel.ID select piece;
            if(pieceReq.Count<Piece>() == 0)
            {
                GererProjet popUp = new GererProjet();
                popUp.ShowDialog();
                return;
            }
            foreach (Piece pie in pieceReq)
            {
                ProjetActuel.ListePieces.Add(pie);
            }
            foreach (Piece pie in ProjetActuel.ListePieces)
            {
                var planReq = (from plan in OutilEF.brycolContexte.Plans where plan.Piece.ID == pie.ID select plan).ToList<Plan>();
                foreach (Plan pl in planReq)
                {
                    ProjetActuel.ListePlans.Add(pl);
                }
            }
            if (ProjetActuel.ListePieces.Count > 0)
            {
                Piece_VM.pieceActuel = ProjetActuel.ListePieces.First<Piece>();
                Plan_VM.uniteDeMesure = Piece_VM.pieceActuel.UniteDeMesure;
            }
            if (ProjetActuel.ListePlans.Count > 0)
            {
                Plan_VM.PlanActuel = ProjetActuel.ListePlans.First<Plan>();
                int idPlan = ProjetActuel.ListePlans.First<Plan>().ID;

                var itemReq = from item in OutilEF.brycolContexte.lstItems.Include("Item") where item.Plan.ID == idPlan select item;
                Item_VM.ItemsPlanActuel = new ObservableCollection<ItemsPlan>();
                foreach (ItemsPlan i in itemReq)
                {
                    if (i.Item != null)
                        Item_VM.ItemsPlanActuel.Add(i);
                }
            }

            
            EstSauvegarde = true;

            PlanDeTravail PlanDeTravail = new PlanDeTravail();
            PlanDeTravail.grdPlanTravail.Children.Clear();
            PlanDeTravail.grdPlanTravail.Children.Add(new PlanDeTravail2());
            PlanDeTravail.ShowDialog();

        }

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

            p.Utilisateur = Utilisateur_VM.utilActuel;
            p.ListePieces = ListePieces;
            p.ListePlans = ListePlans;
            OutilEF.brycolContexte.Projets.Add(p);
            OutilEF.brycolContexte.SaveChanges();
            ProjetActuel = p;

            GererProjet popUp = new GererProjet();
            popUp.ShowDialog();

        }

        public void SauvProjet(Object param)
        {
            Projet p = OutilEF.brycolContexte.Projets.Find(ProjetActuel.ID);
            bool validation = Plan_VM.validePourEnregistrer;
            //vérifie si les items sont valide
            if (validation)
            {
                if (Nom == null)
                {
                    var test = OutilEF.brycolContexte.Projets.Max<Projet>(t => t.ID);
                    test += 1;
                    p.Nom = "Projet" + test;
                }

                if (Nom != p.Nom && EstSauvegarde == true)
                {
                    MessageBoxResult result = MessageBox.Show("Voulez-vous sauvegarder en tant que nouveau projet?", "Sauvegarder en tant que nouveau projet", MessageBoxButton.YesNo);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            var pReq = from pr in OutilEF.brycolContexte.Projets.Include("Utilisateur") where pr.Nom == Nom select pr;
                            foreach (Projet pr in pReq)
                            {
                                if (pr.Utilisateur == Utilisateur_VM.utilActuel && pr.ID != ProjetActuel.ID)
                                {
                                    MessageBox.Show("Vous avez déjà un projet qui se nomme " + Nom + ".");
                                    return;
                                }
                            }
                            SauNeoProjet();
                            break;
                        case MessageBoxResult.No:
                            p.Nom = Nom;
                            OutilEF.brycolContexte.SaveChanges();
                            break;
                    }

                }
                else
                {
                    var pReq = from pr in OutilEF.brycolContexte.Projets.Include("Utilisateur") where pr.Nom == Nom select pr;
                    foreach (Projet pr in pReq)
                    {
                        if (pr.Utilisateur == Utilisateur_VM.utilActuel && pr.ID != ProjetActuel.ID)
                        {
                            MessageBox.Show("Vous avez déjà un projet qui se nomme " + Nom + ".");
                            return;
                        }
                    }
                  
                    p.Nom = Nom;
                    OutilEF.brycolContexte.SaveChanges();
                }

                ProjetActuel = p;
                EstSauvegarde = true;
            }
            else
            {
                //Si au moins un item n'est pas valide, on affiche un message d'erreur
                MessageBoxResult resultat;
                resultat = System.Windows.MessageBox.Show("Impossible d'enregistrer, vous avez un item invalide dans le plan de travail.", "Sauvegarde impossible", MessageBoxButton.OK, MessageBoxImage.Warning);
                if (resultat == MessageBoxResult.OK)
                {
                    return;
                }
            }

            foreach (Window w in Application.Current.Windows)
            {
                if (w.GetType() == typeof(PlanDeTravail))
                {
                    (w as PlanDeTravail).grdPlanTravail.Children.Clear();
                    (w as PlanDeTravail).grdPlanTravail.Children.Add(new PlanDeTravail2());
                }
            }


        }

        public void SuppProjet(Object param)
        {
            Projet pro = new Projet();
            List<Piece> lstPie = new List<Piece>();
            Plan pla = new Plan();
            List<ItemsPlan> lstItPla = new List<ItemsPlan>();

            if (ProjetSelectionne == null)
            {
                ProjetSelectionne = ProjetActuel.Nom;
            }

            var ProReq = from pr in OutilEF.brycolContexte.Projets where pr.Nom == ProjetSelectionne select pr;
            foreach (Projet pr in ProReq)
                pro = pr;

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


        private void SauNeoProjet()
        {
            Projet pro = new Projet();
            List<Piece> lstPie = new List<Piece>();
            Plan pla = new Plan();
            List<ItemsPlan> lstItPla = new List<ItemsPlan>();

            pro = OutilEF.brycolContexte.Projets.Find(ProjetActuel.ID);
            var PieReq = from pie in OutilEF.brycolContexte.Pieces where pie.Projet.ID == pro.ID select pie;

            pro.Nom = Nom;
            pro.Utilisateur = Utilisateur_VM.utilActuel;
            ListePieces = new ObservableCollection<Piece>();
            pro.ListePieces = ListePieces;

            ListePlans = new ObservableCollection<Plan>();
            pro.ListePlans = ListePlans;

            OutilEF.brycolContexte.Projets.Add(pro);

            ProjetActuel = pro;

            foreach (Piece pie in PieReq)
                lstPie.Add(pie);

            foreach (Piece pie in lstPie)
            {
                var PlanReq = from plan in OutilEF.brycolContexte.Plans where plan.Piece.ID == pie.ID select plan;

                pie.Projet = ProjetActuel;
                OutilEF.brycolContexte.Pieces.Add(pie);
                pro.ListePieces.Add(pie);

                foreach (Plan plan in PlanReq)
                {
                    pla = plan;
                    pro.ListePlans.Add(pla);
                }

                var IteReq = from ite in OutilEF.brycolContexte.lstItems where ite.Plan.ID == pla.ID select ite;


                pla.Piece = pie;
                OutilEF.brycolContexte.Plans.Add(pla);

                foreach (ItemsPlan itPl in IteReq)
                    lstItPla.Add(itPl);

                
                foreach (ItemsPlan itPl in lstItPla)
                {

                    itPl.Plan = pla;
                    OutilEF.brycolContexte.lstItems.Add(itPl);
                }

                lstItPla.Clear();

            }

            OutilEF.brycolContexte.SaveChanges();
        }

        private ObservableCollection<ItemPieceProjet> CreatlstIPP()
        {
            ObservableCollection<ItemPieceProjet> LstIPP = new ObservableCollection<ItemPieceProjet>();
            List<Piece> LstPi = new List<Piece>();
            //Item_VM.ItemsPlanActuel = new ObservableCollection<ItemsPlan>();

            var PReq = from p in OutilEF.brycolContexte.Pieces where p.Projet.ID == ProjetActuel.ID select p;
            foreach (Piece p in PReq)
                LstPi.Add(p);

            foreach (Piece p in LstPi)
            {
                ItemPieceProjet Ipp = new ItemPieceProjet();

                var PReq3 = from iP in OutilEF.brycolContexte.lstItems.Include("Item") where iP.Plan.Piece.ID == p.ID select iP;
                foreach (ItemsPlan itemP in PReq3)
                {
                    Ipp.NomPiece = p.Nom;
                    Ipp.NomItem = itemP.Item.Nom;
                    Ipp.CoutItem = itemP.Item.Cout;
                    LstIPP.Add(Ipp);

                    Ipp = new ItemPieceProjet();
                }

            }

            return LstIPP;

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
