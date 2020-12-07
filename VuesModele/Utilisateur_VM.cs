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
using System.Collections.ObjectModel;

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

            ListeFactures = new ObservableCollection<Facture>();
            if (utilActuel != null && Projet_VM.ProjetActuel != null)
            {
                var PReq = from f in OutilEF.brycolContexte.Factures where f.Utilisateur.ID == utilActuel.ID select f;
                foreach (Facture f in PReq)
                    if (f.Projet != null && Projet_VM.ProjetActuel.ID == f.Projet.ID)
                        ListeFactures.Add(f);
            }
        }

        public void Login(Object param)
        {
            Utilisateur user = new Utilisateur();

            var UtReq = from ut in OutilEF.brycolContexte.Utilisateurs where ut.Nom == Nom select ut;
            if (UtReq.Count() != 0)
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
                else
                {
                    MessageBox.Show("Le nom de l'utilisateur et/ou le mot de passe sont incorrect.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Le nom de l'utilisateur et/ou le mot de passe sont incorrect.");
                return;
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
            Utilisateur user = new Utilisateur();

            var UtReq = from ut in OutilEF.brycolContexte.Utilisateurs where ut.Nom == Nom select ut;
            if (UtReq.Count() != 0)
            {
                MessageBox.Show("Le nom d'utilisateur est déjà pris.");
                return;
            }

            user.Nom = Nom;
            user.MotPasse = MotPasse;

            OutilEF.brycolContexte.Utilisateurs.Add(user);
            OutilEF.brycolContexte.SaveChanges();
            utilActuel = user;

            Grid gridMW = (Grid)Application.Current.MainWindow.FindName("gridMainWindow");
            ContentPresenter cpMW = (ContentPresenter)Application.Current.MainWindow.FindName("presenteurContenu");
            gridMW.Children.Clear();
            gridMW.Children.Add(cpMW);
            cpMW.Content = new UCCMenuPrincipal();

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

        private ObservableCollection<Facture> _listeFactures;
        public ObservableCollection<Facture> ListeFactures
        {
            get { return _listeFactures; }
            set
            {
                _listeFactures = value;
                OnPropertyChanged("ListeFactures");
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
