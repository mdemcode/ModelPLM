using BasicSqlService;
using System.Collections.Generic;

namespace ModelPLM
{
    // Tabela PROJECT
    public partial class ZleceniePLM {

        public int IdDB { get; private set; }      // COM_ID
        public string Nr { get; private set; }     // COM_NAM // Uwaga !!! Stała długość pola w DB (nchar(31)) - spacje!
        public int Kod { get; private set; }       // COM_DES
        public string Sekcja { get; private set; } // COM_OBJ
        //
        public bool ZlecenieWczytanePopr { get; private set; }

        private ZleceniePLM() { } // tworzenie obiektów przez fabrykę

        private bool SetProperties(IReadOnlyList<string> daneDB) {
            if (daneDB.Count != 4) return false;
            if (!int.TryParse(daneDB[0], out int id) || daneDB[1].IsNullOrEmpty() || daneDB[1].Equals("{NULL}") || !int.TryParse(daneDB[2], out int kod)) 
                return false;
            IdDB   = id;
            Nr     = daneDB[1].Trim();
            Kod    = kod;
            Sekcja = daneDB[3];
            return true;
        }
    }
}
