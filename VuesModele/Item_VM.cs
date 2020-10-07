using App_Brycol.Modele;
using App_Brycol.Outils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Brycol.VuesModele
{
    class Item_VM : INotifyPropertyChanged
    {
        public Item_VM()
        {
            SommaireItems = new ObservableCollection<Item>();
            var iReq = from i in OutilEF.brycolContexte.Meubles select i;
            
            foreach (Item i in iReq)
                SommaireItems.Add(i);

        }
        #region Propriétés
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

        #endregion


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
