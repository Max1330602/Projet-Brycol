using App_Brycol.Modele;
using App_Brycol.Outils;
using App_Brycol.VuesModele;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace App_Brycol.Vues
{
    /// <summary>
    /// Logique d'interaction pour Transaction.xaml
    /// </summary>
    public partial class Transaction : UserControl
    {
        private List<string> lstFournisseurUnique { get; set; }
        private List<Facture> lstFacturesCrees = new List<Facture>();
        private List<ItemsPlan> lstItemsPlansProjet { get; set; }
        public static UCCoutDetailProjet uccdp { get; set; }
        public Transaction()
        {
            InitializeComponent();
        }

        private void btnConfirmer_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder MessageConfirmation = new StringBuilder();

            if (validerChampsCarte())
            {
                RemplirListesItemsPlanProjetetFournisseur();
                if (lstFournisseurUnique.Count() > 1)
                    MessageConfirmation.Append("Transactions approuvées : " + lstFournisseurUnique.Count() + " Factures\n");
                else
                    MessageConfirmation.Append("Transaction approuvée : " + lstFournisseurUnique.Count() + " Facture\n");
                foreach(Facture f in lstFacturesCrees)
                {
                    MessageConfirmation.Append(f.Fournisseur + ": " + f.Montant + "$\n");
                }
                MessageBox.Show(MessageConfirmation.ToString());

            }

        }

        private void RemplirListesItemsPlanProjetetFournisseur()
        {
            List<Piece> LstPi = new List<Piece>();
            lstItemsPlansProjet = new List<ItemsPlan>();
            ObservableCollection<ItemPieceProjet> LstIPP = new ObservableCollection<ItemPieceProjet>();
            ObservableCollection<ItemsPlan> LstIP = new ObservableCollection<ItemsPlan>();
            List<string> lstFournisseur = new List<string>();
            lstFournisseurUnique = new List<string>();

            var PReq = from p in OutilEF.brycolContexte.Pieces where p.Projet.ID == Projet_VM.ProjetActuel.ID select p;
            foreach (Piece p in PReq)
                LstPi.Add(p);

            foreach (Piece p in LstPi)
            {
                var PReq3 = from iP in OutilEF.brycolContexte.lstItems.Include("Item") where iP.Plan.Piece.ID == p.ID select iP;
                foreach (ItemsPlan itemP in PReq3)
                {
                    if (itemP.EstPaye == "Non")
                    {
                        lstFournisseur.Add(itemP.Item.Fournisseur);
                    }
                    lstItemsPlansProjet.Add(itemP);
                }
            }

            lstFournisseurUnique = lstFournisseur.Distinct().ToList();

            CreerFactures();

            foreach (Window w in Application.Current.Windows)
            {
                if (w.GetType() == typeof(Cout))
                {
                    uccdp = new UCCoutDetailProjet();
                    (w as Cout).grdCoutParent.Children.Remove(Cout.uCCoutDetailProjet);
                    Grid.SetRow(uccdp, 1);
                    (w as Cout).grdCoutParent.Children.Add(uccdp);
                }
            }
        }

        private void CreerFactures()
        {
            Facture factureFournisseur;
            foreach(string f in lstFournisseurUnique)
            {
                factureFournisseur = new Facture();
                foreach(ItemsPlan ip in lstItemsPlansProjet)
                {
                    if (ip.Item.Fournisseur == f && ip.EstPaye == "Non")
                    {
                        factureFournisseur.Montant += ip.Item.Cout;
                        ip.EstPaye = "Oui";
                    }
                }
                decimal tps = Projet_VM.CalTPS(factureFournisseur.Montant);
                decimal tvq = Projet_VM.CalTVQ(factureFournisseur.Montant);
                factureFournisseur.Montant = Projet_VM.CalTotal(factureFournisseur.Montant, tps, tvq);
                factureFournisseur.Projet = Projet_VM.ProjetActuel;
                factureFournisseur.Utilisateur = Utilisateur_VM.utilActuel;
                factureFournisseur.Date = DateTime.Now;
                factureFournisseur.Fournisseur = f;
                lstFacturesCrees.Add(factureFournisseur);
                if (factureFournisseur != null)
                    OutilEF.brycolContexte.Factures.Add(factureFournisseur);
            }
            OutilEF.brycolContexte.SaveChanges();
        }

        bool validerChampsCarte()
        {
            string patternVisa = @"^4\d{12}(\d{3})?$";
            string patternMaster = @"^5[1-5]\d{14}$";
            string patternAmex = @"^3[47]\d{13}$";

            Regex RegexVisa = new Regex(patternVisa);
            Regex RegexMaster = new Regex(patternMaster);
            Regex RegexAmex = new Regex(patternAmex);

            //Vérification de la date d'expiration
            var aujourdhui = new DateTime();
            aujourdhui = DateTime.Today;
            int anneeCourante = aujourdhui.Year;
            int moisCourant = aujourdhui.Month;

            if (!(bool)visa.IsChecked && !(bool)mastercard.IsChecked && !(bool)amex.IsChecked)
            {
                MessageBox.Show("Vous devez choisir un type de carte");
                return false;
            }
            //Validation par carte selon le modèle
            else if (((bool)visa.IsChecked && !RegexVisa.IsMatch(txtnoCarte.Text)) ||
                ((bool)mastercard.IsChecked && !RegexMaster.IsMatch(txtnoCarte.Text)) ||
                ((bool)amex.IsChecked && !RegexAmex.IsMatch(txtnoCarte.Text)))
            {
                MessageBox.Show("Incohérence entre type de carte et numéro");
                return false;
            }
            //Si le résultat se termine par un zéro, le modulo 10 est validé
            else if (!validerCarteModulo())
            {
                MessageBox.Show("Le numéro de carte n'est pas valable");
                return false;
            }
            else if (anneeCourante > Int64.Parse(txtAnnee.Text) ||
               (anneeCourante == Int64.Parse(txtAnnee.Text) && moisCourant > Int64.Parse(txtMois.Text)))
            {
                MessageBox.Show("Date d'expiration dépassée");
                return false;
            }
            return true;
        }

        bool validerCarteModulo()
        {
            //Validation modulo 10
            int sumOfDigits = txtnoCarte.Text.Where((chiffre) => chiffre >= '0' && chiffre <= '9')
            .Reverse()
            .Select((chiffre, i) => ((int)chiffre - 48) * (i % 2 == 0 ? 1 : 2))
            .Sum((chiffre) => chiffre / 10 + chiffre % 10);

            return sumOfDigits % 10 == 0;
        }

    }
}
