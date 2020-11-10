using App_Brycol.Modele;
using App_Brycol.Outils;
using App_Brycol.Vues;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace App_Brycol.VuesModele
{
    class Utilisateur_VM : INotifyPropertyChanged
    {
        public ICommand cmdLogin { get; set; }
        public ICommand cmdLoginTriche { get; set; }
        public ICommand cmdCreeUtil { get; set; }
        public static Utilisateur utilActuel;

        public Utilisateur_VM()
        {
            cmdLogin = new Commande(Login);
            cmdLoginTriche = new Commande(LoginTriche);
            cmdCreeUtil = new Commande(CreeUtil);
        }

        public void Login(Object param)
        {
            Utilisateur user = new Utilisateur();

            var UtReq = from ut in OutilEF.brycolContexte.Utilisateurs where ut.Nom == Nom select ut;
            if (UtReq != null)
            {
                foreach (Utilisateur ut in UtReq)
                    user = ut;

                if (user.MotPasse == MotPasse)
                {
                    utilActuel = user;

                    Grid gridMW = (Grid)Application.Current.MainWindow.FindName("gridMainWindow");
                    ContentPresenter cpMW = (ContentPresenter)Application.Current.MainWindow.FindName("presenteurContenu");
                    gridMW.Children.Clear();
                    gridMW.Children.Add(cpMW);
                    cpMW.Content = new UCCMenuPrincipal();
                }
            }
        }

        public void LoginTriche(Object param)
        {
            Utilisateur user = new Utilisateur();

            user = OutilEF.brycolContexte.Utilisateurs.Find(1);
 
            utilActuel = user;

            Grid gridMW = (Grid)Application.Current.MainWindow.FindName("gridMainWindow");
            ContentPresenter cpMW = (ContentPresenter)Application.Current.MainWindow.FindName("presenteurContenu");
            gridMW.Children.Clear();
            gridMW.Children.Add(cpMW);
            cpMW.Content = new UCCMenuPrincipal();


        }

        public void CreeUtil(Object param)
        {

        }

        #region Propriétés

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

        private string _motPasse;
        public string MotPasse
        {
            get { return _motPasse; }
            set
            {
                _motPasse = value;
                OnPropertyChanged("MotPasse");
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string nomPropriete)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));

        }
    }
}
