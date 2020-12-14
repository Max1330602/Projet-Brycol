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
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

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

            if (Projet_VM.themeSombre)
                AppliquerThemeSombre();
            else
                EnleverThemeSombre();

            imgItem.Source = Piece2D.toolbarImage.Source;
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Piece2D.toolbarImage = null;
        }

        private void btnAppliquer_Click(object sender, RoutedEventArgs e)
        {
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

        private void cmbCouleur_SelectedChange(object sender, SelectionChangedEventArgs e)
        {
            imgItem.Source = Piece2D.toolbarImage.Source;
            foreach (ItemsPlan ip in Item_VM.ItemsPlanActuel)
            {
                ip.Tag = ip.ID;
                string path = "file:///" + System.Windows.Forms.Application.StartupPath;
                path = path.Replace("\\", "/");
                string pathCorrect = path.Substring(0, path.IndexOf("bin")) + "images/items/Top/";

                if (Piece2D.toolbarImage.Source.ToString() == pathCorrect + "item" + ip.Item.ID + ".png" && Piece2D.toolbarImage.Tag.ToString() == ip.Tag.ToString())
                {
                    Item_VM.ItemsPlanModifie.Add(ip);
                    ip.Couleur = cmbCouleur.SelectedItem.ToString().Replace("System.Windows.Controls.ComboBoxItem: ", "");
                    OutilEF.brycolContexte.SaveChanges();

                    BitmapImage bmiItem = new BitmapImage();
                    try
                    {
                        bmiItem.BeginInit();
                        bmiItem.CacheOption = BitmapCacheOption.OnLoad;
                        bmiItem.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                        bmiItem.UriSource = new Uri("pack://application:,,,/images/ItemsModifies/Item" + ip.Item.ID + "/" + ip.Couleur + ".png");
                        bmiItem.EndInit();
                        imgItem.Source = bmiItem;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Impossible de modifier une image ajoutée au catalogue.");
                        return;
                    }
                }
                else if (Piece2D.toolbarImage.Source.ToString() == "pack://application:,,,/images/ItemsModifies/Item" + ip.Item.ID + "/" + ip.Couleur + ".png" && Piece2D.toolbarImage.Tag.ToString() == ip.Tag.ToString())
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
        }

        private void EnleverThemeSombre()
        {
            Banniere.Background = Brushes.LightGray;
            grdModifierItem.Background = Brushes.White;

            imgBackground.Background = Brushes.White;

            btnAppliquer.Background = Brushes.White;
            btnAppliquer.Foreground = Brushes.Black;

            btnAnnuler.Background = Brushes.White;
            btnAnnuler.Foreground = Brushes.Black;
        }

        private void AppliquerThemeSombre()
        {
            BrushConverter bc = new BrushConverter();
            Brush CouleurBouton = (Brush)bc.ConvertFrom("#45463F");
            Brush CouleurArriere = (Brush)bc.ConvertFrom("#33342F");
            Brush CouleurBanniere = (Brush)bc.ConvertFrom("#84857D");
            Brush CouleurArrierePlan = (Brush)bc.ConvertFrom("#7D7E79");

            Banniere.Background = CouleurBanniere;
            grdModifierItem.Background = CouleurArrierePlan;

            imgBackground.Background = CouleurArriere;

            btnAppliquer.Background = CouleurBouton;
            btnAppliquer.Foreground = Brushes.White;

            btnAnnuler.Background = CouleurBouton;
            btnAnnuler.Foreground = Brushes.White;
        }



        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {

        }
    }
}
