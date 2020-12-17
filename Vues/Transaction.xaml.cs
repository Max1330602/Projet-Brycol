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
using ceTe.DynamicPDF;
using Page = ceTe.DynamicPDF.Page;
using Label = ceTe.DynamicPDF.PageElements.Label;
using Image = ceTe.DynamicPDF.PageElements.Image;
using System.IO;

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
        public Transaction()
        {
            InitializeComponent();
            if (Projet_VM.themeSombre)
                AppliquerThemeSombre();
            else
                EnleverThemeSombre();

        }

        private void EnleverThemeSombre()
        {
            btnPayer.Background = Brushes.White;
            btnPayer.Foreground = Brushes.Black;
        }

        private void AppliquerThemeSombre()
        {
            BrushConverter bc = new BrushConverter();
            Brush CouleurBouton = (Brush)bc.ConvertFrom("#45463F");

            btnPayer.Background = CouleurBouton;
            btnPayer.Foreground = Brushes.White;
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

                if (lstFournisseurUnique.Count() == 0)
                    MessageBox.Show("Il n'y a rien à payer");
                else
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
                    (w as Cout).grdCoutParent.Children.Remove(Cout.uCCoutDetailProjet);

                    Cout.uCCoutDetailProjet = new UCCoutDetailProjet();
                    Grid.SetRow(Cout.uCCoutDetailProjet, 1);
                    (w as Cout).grdCoutParent.Children.Add(Cout.uCCoutDetailProjet);
                }
            }
        }

        string GetPath(string filePath)
        {
            var exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathManager = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathManager.Match(exePath).Value;
            return System.IO.Path.Combine(appRoot, filePath);
        }

        private void CreerFactures()
        {
            Facture factureFournisseur;
            Document document;
            Page page;
            DateTime date = new DateTime();
            date = DateTime.Now;

            string pathDirectoryUser = "..\\..\\Factures\\" + Utilisateur_VM.utilActuel.Nom;
            string pathDirectoryFacture = "..\\..\\Factures\\";

            if (!Directory.Exists(pathDirectoryFacture))
                Directory.CreateDirectory(pathDirectoryFacture);

            if (!Directory.Exists(pathDirectoryUser))
                Directory.CreateDirectory(pathDirectoryUser);

            //   "../../Factures/" + Utilisateur_VM.utilActuel.Nom + "/" + f + "_" + date + ".pdf"
            foreach (string f in lstFournisseurUnique)
            {
                document = new Document();
                page = new Page(PageSize.Letter, PageOrientation.Portrait, 54.0f);
                document.Pages.Add(page);
                Image logo = new Image("..\\..\\images\\logo.png", 0, 0);
                logo.Align = Align.Right;
                page.Elements.Add(logo);
                string titre = "------------------------------------------------------------------------------------\n"
                                                                                  + f +
                                   "\n-----------------------------------------------------------------------------------\n";
                Label labelTitre = new Label(titre, 0, 0, 504, 100, Font.Helvetica, 18, TextAlign.Center);
                page.Elements.Add(labelTitre);

                Label ItemsAchetes = new Label("Items Achetés : ", 0, 100, 504, 150, Font.Helvetica, 16, TextAlign.Left);
                page.Elements.Add(ItemsAchetes);

                factureFournisseur = new Facture();

                
                string infoItem = "";
                foreach (ItemsPlan ip in lstItemsPlansProjet)
                {
                    if (ip.Item.Fournisseur == f && ip.EstPaye == "Non")
                    {
                        infoItem += ip.Item.Nom + "          " + ip.Item.Cout + " $\n";

                        factureFournisseur.Montant += ip.Item.Cout;
                        ip.EstPaye = "Oui";
                    }
                    
                    

                }

                Label item = new Label(infoItem, 0, 150, 504, 30, Font.Helvetica, 13, TextAlign.Left);
                page.Elements.Add(item);

                string sousTotalInfo = "Sous-total : " + factureFournisseur.Montant.ToString() + " $";
                Label sousTotal = new Label(sousTotalInfo, 0, 250, 504, 30, Font.Helvetica, 13, TextAlign.Left);
                page.Elements.Add(sousTotal);


                decimal tps = Projet_VM.CalTPS(factureFournisseur.Montant);
                Label TPS = new Label("TPS : " + tps + " $", 0, 280, 504, 30, Font.Helvetica, 13, TextAlign.Left);
                page.Elements.Add(TPS);

                decimal tvq = Projet_VM.CalTVQ(factureFournisseur.Montant);
                Label TVQ = new Label("TVQ : " + tvq + " $", 0, 310, 504, 30, Font.Helvetica, 13, TextAlign.Left);
                page.Elements.Add(TVQ);

                factureFournisseur.Montant = Projet_VM.CalTotal(factureFournisseur.Montant, tps, tvq);
                Label Total = new Label("Total : " + factureFournisseur.Montant + " $", 0, 340, 504, 30, Font.Helvetica, 13, TextAlign.Left);
                page.Elements.Add(Total);

                factureFournisseur.Projet = Projet_VM.ProjetActuel;
                factureFournisseur.Utilisateur = Utilisateur_VM.utilActuel;
                factureFournisseur.Date = DateTime.Now;
                factureFournisseur.Fournisseur = f;
                lstFacturesCrees.Add(factureFournisseur);
                if (factureFournisseur != null)
                    OutilEF.brycolContexte.Factures.Add(factureFournisseur);
                document.Draw("..\\..\\Factures\\" + Utilisateur_VM.utilActuel.Nom + "\\"+ f + "_" + date.Year + "_" + date.Month + "_" + date.Day + "_" + date.Hour + "_" + date.Minute + "_" + date.Second + ".pdf");
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
            else if (Int64.Parse(txtMois.Text) < 1 || Int64.Parse(txtMois.Text) > 12)
            {
                MessageBox.Show("Le mois doit être entre 1 et 12");
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
