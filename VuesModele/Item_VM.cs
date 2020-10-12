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
            ListeItems = new ObservableCollection<Item>();

            var iReq = from i in OutilEF.brycolContexte.Meubles select i;
            
            foreach (Item i in iReq)
                SommaireItems.Add(i);            




        }
        #region Propriétés

        public ICommand cmdAjouterItem { get; set; }
        public const int POS_PAR_DEFAUT = 0;

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
                    if (_itemSelectionne != null)
                    {

                    }
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
        #endregion


        public void AjouterItem(Object param)
        { 
            ItemsPlan i = new ItemsPlan();

            i.Item = _itemSelectionne;
            i.emplacement = POS_PAR_DEFAUT;
            i.idPlan = 1;

            ListeItems.Add(i.Item);
            OutilEF.brycolContexte.lstItems.Add(i);
            OutilEF.brycolContexte.SaveChanges();

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
