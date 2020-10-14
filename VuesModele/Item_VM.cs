using App_Brycol.Modele;
using App_Brycol.Outils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace App_Brycol.VuesModele
{
    class Item_VM : INotifyPropertyChanged
    {
        public Item_VM()
        {
            cmdAjouterItem = new Commande(AjouterItem);

            SommaireItems = new ObservableCollection<Item>();
            FiltrePrixMax = PRIXMAX;
            FiltrePrixMin = PRIXMIN;
            var iReq = from i in OutilEF.brycolContexte.Meubles select i;
            
            foreach (Item i in iReq)
                SommaireItems.Add(i);
            Items = SommaireItems;

        }
        #region Propriétés

        public ICommand cmdAjouterItem { get; set; }
        public const int POS_PAR_DEFAUT = 0;
        public const int PRIXMAX = 1000000;
        public const int PRIXMIN = 0;
        public ObservableCollection<Item> Items;

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
        public ObservableCollection<Item> ListeItems
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
                OnPropertyChanged("Filtre");
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
                filtrer();
            }
        }
        #endregion


        public void filtrer() 
        {
            if (SommaireItems != null && SommaireItems.Count != 0)
            {
                SommaireItems = Items;
                if(FiltreNom == null)
                    SommaireItems = new ObservableCollection<Item>(SommaireItems.Where(si => si.Cout > FiltrePrixMin && si.Cout < FiltrePrixMax));
                else
                    SommaireItems = new ObservableCollection<Item>(SommaireItems.Where(si => si.Nom.Contains(FiltreNom) && si.Cout > FiltrePrixMin && si.Cout < FiltrePrixMax));
            }
            else if (SommaireItems.Count == 0 && (FiltreNom != null  || FiltrePrixMin != 0 || FiltrePrixMax != 1000000)) 
            {
                SommaireItems = Items;
                if (FiltreNom == null)
                    SommaireItems = new ObservableCollection<Item>(SommaireItems.Where(si => si.Cout > FiltrePrixMin && si.Cout < FiltrePrixMax));
                else
                    SommaireItems = new ObservableCollection<Item>(SommaireItems.Where(si => si.Nom.Contains(FiltreNom) && si.Cout > FiltrePrixMin && si.Cout < FiltrePrixMax));
            }
        }

        public void AjouterItem(Object param)
        { 
            ItemsPlan i = new ItemsPlan();

            i.Item = _itemSelectionne;
            i.emplacement = POS_PAR_DEFAUT;
            // HARD CODE
            i.idPlan = 1;
            if (i.Item != null)
            {
                ListeItems.Add(i.Item);
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
