using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace App_Brycol.Modele
{
    public class Plan
    {
        public int ID { get; set; }
        public Piece Piece { get; set; }
        public bool est3D { get; set; }
        public float tailleZoom { get; set; }
        [NotMapped]
        public BitmapImage ImgPlan
        {
            get
            {
                BitmapImage bmiPlan = new BitmapImage();
                bmiPlan.BeginInit();
                bmiPlan.CacheOption = BitmapCacheOption.OnLoad;
                bmiPlan.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bmiPlan.UriSource = new Uri("..\\..\\images\\Plans\\plan" + ID + ".png", UriKind.Relative);
                try
                {
                    bmiPlan.EndInit();
                }
                catch (Exception)
                {
                    bmiPlan = new BitmapImage();
                    bmiPlan.BeginInit();
                    bmiPlan.UriSource = new Uri("pack://application:,,,/images/Items/item0.png");
                    bmiPlan.EndInit();


                }
                return bmiPlan;
            }
            set { }
        }


        public Plan()
        {
            
        }
    }
}
