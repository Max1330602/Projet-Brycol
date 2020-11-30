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
using System.Text.RegularExpressions;
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
            StructuresItems = new ObservableCollection<Item>();
            ListeItems = new ObservableCollection<Item>();
            Categories = new ObservableCollection<string>();
            TypesDePiece = new ObservableCollection<string>();
            if (ItemsPlanActuel == null)
                ItemsPlanActuel = new ObservableCollection<ItemsPlan>();

            if (ItemsPlanModifie == null)
                ItemsPlanModifie = new ObservableCollection<ItemsPlan>();

            FiltreCategorie = "";
            FiltreNom = "";
            FiltreType = "";
            FiltrePrixMax = PRIXMAX;
            FiltrePrixMin = PRIXMIN;
            var iReq = from i in OutilEF.brycolContexte.Meubles.Include("Categorie").Include("TypePiece") select i;

            foreach (Item i in iReq)
            {
                if (i.Nom.Contains("Porte") || i.Nom.Contains("Fenêtre"))
                    StructuresItems.Add(i);
                else
                    SommaireItems.Add(i);



                if (!Categories.Contains(i.Categorie.Nom))
                    Categories.Add(i.Categorie.Nom);

                if (!TypesDePiece.Contains(i.TypePiece.Nom))
                    TypesDePiece.Add(i.TypePiece.Nom);
            }
            Items = SommaireItems;
            Structures = StructuresItems;
            Categories.Add("Tous");
            TypesDePiece.Add("Tous");

        }
        #region Propriétés

        private ICommand _cmdAjouterItem;
        public ICommand CmdAjouterItem
        {
            get
            {
                if (_cmdAjouterItem == null)
                    _cmdAjouterItem = new Commande(AjouterItem);
                return _cmdAjouterItem;
            }
            set
            {
                _cmdAjouterItem = value;
            }
        }

        public ICommand cmdAjouterItemModifie { get; set;}
        public ICommand cmdInitItem { get; set; }
        public ICommand cmdAjouterItem { get; set; }

        public const int POS_PAR_DEFAUT = 0;
        public const int PRIXMAX = 1000000;
        public const int PRIXMIN = 0;

        public ObservableCollection<Item> Items;
        public ObservableCollection<Item> Structures;

        public static ObservableCollection<ItemsPlan> ItemsPlanActuel;
        public static ObservableCollection<ItemsPlan> ItemsPlanModifie;

        private Categorie _categorie;
        public Categorie Categorie 
        {
            get { return _categorie; }
            set
            {
                _categorie = value;
                OnPropertyChanged("Categorie");
            }

        }

        private ObservableCollection<string> _categories;
        public ObservableCollection<string> Categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
                OnPropertyChanged("Categories");
            }

        }

        private ObservableCollection<string> _typesDePiece;
        public ObservableCollection<string> TypesDePiece
        {
            get { return _typesDePiece; }
            set
            {
                _typesDePiece = value;
                OnPropertyChanged("TypesDePiece");
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

        private ObservableCollection<Item> _structuresItems;
        public ObservableCollection<Item> StructuresItems
        {
            get { return _structuresItems; }
            set
            {
                _structuresItems = value;
                OnPropertyChanged("StructuresItems");
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

        private string _fournisseur;
        public string Fournisseur
        {
            get
            {
                return _fournisseur;
            }
            set
            {
                _fournisseur = value;
                OnPropertyChanged("Fournisseur");
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
            if (FiltreCategorie == "Tous")
                FiltreCategorie = "";
            else if (FiltreType == "Tous")
                FiltreType = "";
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
            if (FiltreCategorie == "Tous")
                FiltreCategorie = "";
            else if (FiltreType == "Tous")
                FiltreType = "";
            if (FiltreNom == "" && FiltreCategorie == "" && FiltreType == "")
                SommaireItems = new ObservableCollection<Item>(SommaireItems.Where(si => si.Cout > FiltrePrixMin &&
                                                                                         si.Cout < FiltrePrixMax));
            else
            {  SommaireItems = new ObservableCollection<Item>(SommaireItems.Where(si => Regex.IsMatch(si.TypePiece.Nom, FiltreType, RegexOptions.IgnoreCase) && //si.TypePiece.Nom.Contains(FiltreType) &&
                                                                                        Regex.IsMatch(si.Categorie.Nom, FiltreCategorie, RegexOptions.IgnoreCase) && //si.Categorie.Nom.Contains(FiltreCategorie) &&
                                                                                        Regex.IsMatch(si.Nom, FiltreNom, RegexOptions.IgnoreCase) && //si.Nom.Contains(FiltreNom) &&
                                                                                         si.Cout > FiltrePrixMin &&
                                                                                         si.Cout < FiltrePrixMax));
            
            }
        }

        public void AjouterItem(Object param)
        { 
            
            ItemsPlan i = new ItemsPlan();

            i.Item = _itemSelectionne;           
            i.emplacementGauche = POS_PAR_DEFAUT;
            i.emplacementHaut = POS_PAR_DEFAUT;
            if (i.Item.Nom == "Porte")
                i.cotePorte = "droite";
            else
                i.cotePorte = "";
            // HARD CODE
            i.Plan = Plan_VM.PlanActuel;
            if (i.Item != null)
            {
                ListeItems.Add(i.Item);
                i.Plan = Plan_VM.PlanActuel;
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
