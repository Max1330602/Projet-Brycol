using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Brycol.Modele
{
    class Plan
    {
        public int ID { get; set; }
        public Piece Piece { get; set; }
        public bool est3D { get; set; }
        public float tailleZoom { get; set; }

        public Plan()
        {

        }
    }
}
