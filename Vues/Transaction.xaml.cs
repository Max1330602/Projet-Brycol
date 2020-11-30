using App_Brycol.VuesModele;
using System;
using System.Collections.Generic;
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
        public Transaction()
        {
            InitializeComponent();
        }

        private void btnConfirmer_Click(object sender, RoutedEventArgs e)
        {
            string patternVisa = @"^4\d{12}(\d{3})?$";
            string patternMaster = @"^5[1-5]\d{14}$";
            string patternAmex = @"^3[47]\d{13}$";

            Regex RegexVisa =new Regex(patternVisa);
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
            }
            //Validation par carte selon le modèle
            else if (((bool)visa.IsChecked && !RegexVisa.IsMatch(txtnoCarte.Text)) ||
                ((bool)mastercard.IsChecked && !RegexMaster.IsMatch(txtnoCarte.Text)) ||
                ((bool)amex.IsChecked && !RegexAmex.IsMatch(txtnoCarte.Text)))
            {
                MessageBox.Show("Incohérence entre type de carte et numéro");
            }
            //Si le résultat se termine par un zéro, le modulo 10 est validé
            else if (!validerCarteModulo())
            {
                MessageBox.Show("Le numéro de carte n'est pas valable");
            }
            else if (anneeCourante > Int64.Parse(txtAnnee.Text) ||
               (anneeCourante == Int64.Parse(txtAnnee.Text) && moisCourant > Int64.Parse(txtMois.Text)))
            {
                MessageBox.Show("Date d'expiration dépassée");
            }      
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
