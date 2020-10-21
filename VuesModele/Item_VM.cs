using App_Brycol.Modele;
using App_Brycol.Outils;
using App_Brycol.Vues;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;

namespace App_Brycol.VuesModele
{
    class Item_VM : INotifyPropertyChanged
    {
        public Item_VM()
        {
            cmdAjouterItem = new Commande(AjouterItem);
            SommaireItems = new ObservableCollection<Item>();
            ListeItems = new ObservableCollection<Item>();
            if (ItemsPlanActuel == null)
                ItemsPlanActuel = new ObservableCollection<ItemsPlan>();
            FiltreCategorie = "";
            FiltreNom = "";
            FiltreType = "";
            FiltrePrixMax = PRIXMAX;
            FiltrePrixMin = PRIXMIN;
            var iReq = from i in OutilEF.brycolContexte.Meubles.Include("Categorie").Include("TypePiece") select i;

            foreach (Item i in iReq)
                SommaireItems.Add(i);

            Items = SommaireItems;

        }
        #region Propriétés

        public ICommand cmdAjouterItem { get; set; }
        public ICommand cmdInitItem { get; set; }
        public const int POS_PAR_DEFAUT = 0;
        public const int PRIXMAX = 1000000;
        public const int PRIXMIN = 0;

        public ObservableCollection<Item> Items;
        public static ObservableCollection<ItemsPlan> ItemsPlanActuel;

        private Categorie _Categorie;
        public Categorie Categorie 
        {
            get { return _Categorie; }
            set
            {
                _Categorie = value;
                OnPropertyChanged("Categorie");
            }

        }
        private ObservableCollection<Item> _sommaireItems;
        public ObservableCollection<Item> SommaireItems
        {
            get { return _sommaireItems; }
            set
            {
                _sommaireItems = value;
                OnPropertyChanged("SommaireItems");
            }
        }      

        private Item _itemSelectionne;
        public Item ItemSelectionne
        {
            get { return _itemSelectionne; }
            set
            {
                if (value != null)
                {
                    _itemSelectionne = value;
                }
                else 
                {
                    _itemSelectionne = null;
                }
                OnPropertyChanged("ItemSelectionne");
            }
        }

        private ObservableCollection<Item> _listeItems;
        public  ObservableCollection<Item> ListeItems
        {
            get { return _listeItems; }
            set
            {              
                _listeItems = value;
                OnPropertyChanged("ListeItems");
            }
        }

        private string _nom;
        public string Nom
        {
            get
            {
                return _nom;
            }
            set
            {
                _nom = value;
                OnPropertyChanged("Nom");
            }
        }

        private string _filtreNom;
        public string FiltreNom
        {
            get 
            {
                return _filtreNom;
            }
            set
            {
                _filtreNom = value;
                OnPropertyChanged("FiltreNom");
                if (Items != null)
                    SommaireItems = Items;
                filtrer();
            }
        }

        private decimal _filtrePrixMin;
        public decimal FiltrePrixMin 
        {
            get
            {
                return _filtrePrixMin;
            }
            set
            {
                _filtrePrixMin = value;
                OnPropertyChanged("FiltrePrixMin");
                if (Items != null)
                    SommaireItems = Items;
                filtrer();

            }
        }

        private decimal _filtrePrixMax;
        public decimal FiltrePrixMax 
        {
            get
            {
                return _filtrePrixMax;
            }
            set
            {
                _filtrePrixMax = value;
                OnPropertyChanged("FiltrePrixMax");
                if (Items != null)
                    SommaireItems = Items;
                filtrer();

            }
        }

        private string _filtreType;
        public string FiltreType
        {

            get
            {
                return _filtreType;

            }
            set
            {
                _filtreType = value;
                OnPropertyChanged("FiltreType");
                if (Items != null)
                    SommaireItems = Items;
                filtrer();
            }

        }

        private string _filtreCategorie;
        public string FiltreCategorie
        {

            get
            {
                return _filtreCategorie;

            }
            set
            {
                _filtreCategorie = value;
                OnPropertyChanged("FiltreCategorie");
                if (Items != null)
                    SommaireItems = Items;
                filtrer();
            }

        }
        #endregion


        public void filtrer()

        {
            //Si la liste n'est pas vide ni égale à 0
            if (SommaireItems != null && SommaireItems.Count != 0)
                filtreCombine();
            //Si il y a un ou des filtre mais que la liste est vide
            else if (SommaireItems.Count == 0 && (FiltreNom != "" || FiltreCategorie != "" || FiltreType != "" || FiltrePrixMin != 0 || FiltrePrixMax != 1000000))
                filtreCombine();
            //Si il y a un filtre actif
            else if (FiltreNom != "" || FiltreCategorie != "" || FiltreType != "" || FiltrePrixMin != 0 || FiltrePrixMax != 1000000)
                filtreCombine();
            

        }

        public void filtreCombine()
        {
            if (FiltreNom == "" && FiltreCategorie == "" && FiltreType == "")
                SommaireItems = new ObservableCollection<Item>(SommaireItems.Where(si => si.Cout > FiltrePrixMin &&
                                                                                         si.Cout < FiltrePrixMax));
            else
                SommaireItems = new ObservableCollection<Item>(SommaireItems.Where(si => si.TypePiece.Nom.Contains(FiltreType) &&
                                                                                         si.Categorie.Nom.Contains(FiltreCategorie) &&
                                                                                         si.Nom.Contains(FiltreNom) &&
                                                                                         si.Cout > FiltrePrixMin &&
                                                                                         si.Cout < FiltrePrixMax));
        }

        public void AjouterItem(Object param)
        { 
            ItemsPlan i = new ItemsPlan();

            i.Item = _itemSelectionne;
            i.emplacementGauche = POS_PAR_DEFAUT;
            i.emplacementHaut = POS_PAR_DEFAUT;
            // HARD CODE
            i.Plan = Plan_VM.PlanActuel;
            if (i.Item != null)
            {
                ListeItems.Add(i.Item);
                ItemsPlanActuel.Add(i);
                OutilEF.brycolContexte.lstItems.Add(i);
                OutilEF.brycolContexte.SaveChanges();
            }
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
