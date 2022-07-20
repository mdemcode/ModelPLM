using BasicSqlService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelPLM
{
    // Tabela PROFILE
    public partial class ProfilPLM {

        public int IdDB { get; private set; }                     // 0 - PRF_ID
        public char Kategoria { get; private set; }               // 1 - PRF_CAT
        public string Nazwa { get; private set; }                 // 2 - PRF_NAM // Uwaga !!! Stała długość pola w DB (nchar(31)) - spacje!
        public float Wysokosc_SzerokoscFlPl { get; private set; } // 3 - PRF_H (szerokość blachy / płaskownika)
        public float SzerokoscPolki { get; private set; }         // 4 - PRF_B
        public float SzerokoscPolki1 { get; private set; }        // 5 - PRF_C (druga półka profilu / blachownicy)
        public float GruboscSrodnika_Blachy { get; private set; } // 6 - PRF_A (grubość blachy / środnika)
        public float GruboscPolki { get; private set; }           // 7 - PRF_E (grubość półki; 0 dla blach)
        public float Promien { get; private set; }                // 8 - PRF_R
        public float Promien1 { get; private set; }               // 9 - PRF_R1
        public float MasaJedn { get; private set; }               // 10 - PRF_PDS [kg/m]
        public float PowierzchniaJedn { get; private set; }       // 11 - PRF_SRF [m^2/m]
        //
        public bool ProfilWczytanyPopr { get; private set; }

        private ProfilPLM() { } // tworzenie obiektów przez fabrykę

        private bool SetProperties(IReadOnlyList<string> daneDB) {
            if (daneDB.Count != 12) return false;
            if (!int.TryParse(daneDB[0], out int id) || daneDB[1].IsNullOrEmpty() || daneDB[2].IsNullOrEmpty() || daneDB[2].Equals("{NULL}") || 
                !float.TryParse(daneDB[3], out float wys) || !float.TryParse(daneDB[4], out float szer) || !float.TryParse(daneDB[5], out float szer1) || 
                !float.TryParse(daneDB[6], out float gr) || !float.TryParse(daneDB[7], out float gr1) || !float.TryParse(daneDB[8], out float pr) || 
                !float.TryParse(daneDB[9], out float pr1) || !float.TryParse(daneDB[10], out float masa) || !float.TryParse(daneDB[11], out float pow)) 
                return false;
            IdDB                   = id;
            Kategoria              = daneDB[1].First();
            Nazwa                  = daneDB[2].Trim();
            Wysokosc_SzerokoscFlPl = wys;
            SzerokoscPolki         = szer;
            SzerokoscPolki1        = szer1;
            GruboscSrodnika_Blachy = gr;
            GruboscPolki           = gr1;
            Promien                = pr;
            Promien1               = pr1;
            MasaJedn               = masa;
            PowierzchniaJedn       = pow;
            return true;
        }
    }
}
