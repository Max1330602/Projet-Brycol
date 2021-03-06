﻿using App_Brycol.Outils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Brycol.Modele
{

    public class Piece
    {
       
        public int ID { get; set; }
        public Projet Projet { get; set; }
        public TypePiece TypePiece { get; set; }
        public TypePlancher TypePlancher { get; set; }
        public float Largeur { get; set; }
        public float Longueur { get; set; }
        public string Nom { get; set; }
        public string UniteDeMesure { get; set; }
        [NotMapped]
        public decimal Total { get; set; }

        public Piece() 
        {

        }
    }
}
