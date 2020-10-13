using Renci.SshNet.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace App_Brycol.Modele
{
    class Item
    {
        public int ID { get; set; }
        public int IdTypeDePiece { get; set; }
        public int IdCategorie { get; set; }
        public string Nom { get; set; }
        public float Largeur { get; set; }
        public float Longueur { get; set; }
        public float Hauteur { get; set; }
        public string Couleur { get; set; }
        public decimal Cout { get; set; }
        public string Fournisseur { get; set; }
        public string Commentaire { get; set; }
        [NotMapped]
        public BitmapImage ImgItem { 
            get 
            {

                BitmapImage bmiItem = new BitmapImage();
                bmiItem.BeginInit();
                bmiItem.UriSource = new Uri("pack://application:,,,/images/Items/item" + ID + ".png");

                try
                {
                    bmiItem.EndInit();
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("Impossible de trouver la ressource"))
                    {
                        bmiItem = new BitmapImage();
                        bmiItem.BeginInit();
                        bmiItem.UriSource = new Uri("pack://application:,,,/images/Items/item0.png");
                        bmiItem.EndInit();
                    }

                }
                return bmiItem;
            }
            set { }
        }

        public Item() { }

    }
}
