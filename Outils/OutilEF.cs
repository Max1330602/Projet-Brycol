using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace App_Brycol.Outils
{


    class OutilEF
    {
        public static BrycolContexte brycolContexte;

        public OutilEF()
        {
            try
            {
                brycolContexte = new BrycolContexte();
                brycolContexte.Database.Initialize(force: true);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
