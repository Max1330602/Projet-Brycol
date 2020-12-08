using App_Brycol.VuesModele;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Logique d'interaction pour Structures.xaml
    /// </summary>
    public partial class Structures : Window
    {
        public Structures()
        {
            InitializeComponent();

            if (Projet_VM.themeSombre)
                AppliquerThemeSombre();
            else
                EnleverThemeSombre();

            DataContext = new Item_VM();
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var vm = (Item_VM)DataContext;
            if (vm.CmdAjouterItem.CanExecute(null))
                vm.CmdAjouterItem.Execute(null);
        }

        private void btnRetour_Click(object sender, RoutedEventArgs e)
        {
            Plan_VM.catalogueClick = true;
            this.Close();
            foreach (Window w in Application.Current.Windows)
            {
                if (w.GetType() == typeof(PlanDeTravail))
                {
                    (w as PlanDeTravail).grdPlanTravail.Children.Clear();
                    (w as PlanDeTravail).grdPlanTravail.Children.Add(new PlanDeTravail2());
                }
            }
        }

        private void AppliquerThemeSombre()
        {
            BrushConverter bc = new BrushConverter();
            Brush CouleurBouton = (Brush)bc.ConvertFrom("#45463F");
            Brush CouleurArriere = (Brush)bc.ConvertFrom("#33342F");
            Brush CouleurBanniere = (Brush)bc.ConvertFrom("#84857D");
            Brush CouleurArrierePlan = (Brush)bc.ConvertFrom("#7D7E79");

            BitmapImage bmiLogo = new BitmapImage();
            bmiLogo.BeginInit();
            bmiLogo.CacheOption = BitmapCacheOption.OnLoad;
            bmiLogo.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bmiLogo.UriSource = new Uri("pack://application:,,,/images/logoclair.png");
            bmiLogo.EndInit();
            logo.Source = bmiLogo;

            btnAjouter.Background = CouleurBouton;
            btnAjouter.Foreground = Brushes.White;

            btnRetour.Background = CouleurBouton;
            btnRetour.Foreground = Brushes.White;

            DG_items.RowBackground = Brushes.LightGray;
            DG_items.AlternatingRowBackground = Brushes.Gray;
            DG_items.Background = CouleurArriere;

            DG_ListeItems.RowBackground = Brushes.Gray;
            DG_ListeItems.AlternatingRowBackground = Brushes.LightGray;
            DG_ListeItems.Background = CouleurArriere;

            Pied.Background = CouleurArrierePlan;
            Banniere.Background = CouleurBanniere;;
        }

        private void EnleverThemeSombre()
        {
            btnAjouter.Background = Brushes.White;
            btnAjouter.Foreground = Brushes.Black;

            btnRetour.Background = Brushes.White;
            btnRetour.Foreground = Brushes.Black;

            DG_items.RowBackground = Brushes.White;
            DG_items.AlternatingRowBackground = Brushes.White;
            DG_items.Background = Brushes.White;

            DG_ListeItems.RowBackground = Brushes.White;
            DG_ListeItems.AlternatingRowBackground = Brushes.White;
            DG_ListeItems.Background = Brushes.White;

            Pied.Background = Brushes.White;
            Banniere.Background = Brushes.LightGray;
        }


    }
}
