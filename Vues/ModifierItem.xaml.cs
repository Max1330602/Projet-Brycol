using App_Brycol.Modele;
using App_Brycol.Outils;
using App_Brycol.VuesModele;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
    /// Logique d'interaction pour ModifierItem.xaml
    /// </summary>
    public partial class ModifierItem : Window
    {
        public ModifierItem()
        {
            InitializeComponent();

            DataContext = new Item_VM();

            imgItem.Source = Piece2D.draggedImage.Source;
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Piece2D.draggedImage = null;
        }

        private void btnAppliquer_Click(object sender, RoutedEventArgs e)
        {
            Grid gridMW = (Grid)Application.Current.MainWindow.FindName("gridMainWindow");
            ContentPresenter cpMW = (ContentPresenter)Application.Current.MainWindow.FindName("presenteurContenu");
            this.Close();
            gridMW.Children.Clear();
            gridMW.Children.Add(cpMW);
            cpMW.Content = new PlanDeTravail();
        }

        private void cmbCouleur_SelectedChange(object sender, SelectionChangedEventArgs e)
        {
            imgItem.Source = Piece2D.draggedImage.Source;
            foreach (ItemsPlan ip in Item_VM.ItemsPlanActuel)
            {
                ip.Tag = ip.ID;
                if (Piece2D.draggedImage.Source.ToString() == "pack://application:,,,/images/Items/Top/item" + ip.Item.ID + ".png" && Piece2D.draggedImage.Tag.ToString() == ip.Tag.ToString())
                {
                    Item_VM.ItemsPlanModifie.Add(ip);
                    ip.Couleur = cmbCouleur.SelectedItem.ToString().Replace("System.Windows.Controls.ComboBoxItem: ", "");
                    OutilEF.brycolContexte.SaveChanges();

                    BitmapImage bmiItem = new BitmapImage();
                    bmiItem.BeginInit();
                    bmiItem.CacheOption = BitmapCacheOption.OnLoad;
                    bmiItem.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    bmiItem.UriSource = new Uri("pack://application:,,,/images/ItemsModifies/Item" + ip.Item.ID + "/" + ip.Couleur + ".png");
                    bmiItem.EndInit();
                    imgItem.Source = bmiItem;
                }
                else if (Piece2D.draggedImage.Source.ToString() == "pack://application:,,,/images/ItemsModifies/Item" + ip.Item.ID + "/" + ip.Couleur + ".png" && Piece2D.draggedImage.Tag.ToString() == ip.Tag.ToString())
                {
                    ip.Couleur = cmbCouleur.SelectedItem.ToString().Replace("System.Windows.Controls.ComboBoxItem: ", "");
                    OutilEF.brycolContexte.SaveChanges();

                    BitmapImage bmiItem = new BitmapImage();
                    bmiItem.BeginInit();
                    bmiItem.CacheOption = BitmapCacheOption.OnLoad;
                    bmiItem.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    bmiItem.UriSource = new Uri("pack://application:,,,/images/ItemsModifies/Item" + ip.Item.ID + "/" + ip.Couleur + ".png");
                    bmiItem.EndInit();
                    imgItem.Source = bmiItem;
                }           
            } 
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Close();
            //Piece2D.draggedImage = null;
        }


        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {

        }
    }
}
