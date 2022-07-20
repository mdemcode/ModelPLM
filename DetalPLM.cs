using BasicSqlService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelPLM
{
    // tabela PART w bazie PLM
    public partial class DetalPLM {

        #region FIELDS&PROPERTIES
        // dane podstawowe DB
        private static readonly string[] _kolumnyTabeliPartDoWczytania = 
        //     0         1         2          3         4         5         6          7          8          9           10          11          12
            { "PCE_ID", "COM_ID", "PCE_NAM", "PRF_ID", "MAT_ID", "PNT_ID", "PCE_LEN", "PCE_WDT", "PCE_DES", "PCE_CMT1", "PCE_CMT2", "PCE_CMT3", "GRP_ID" };
        //
        private int _pce_Id;      // id detalu
        private int _com_Id;      // id zlecenia (tab. PROJECT)
        private string _pce_Nam;  // nazwa detalu
        private int? _prf_Id;     // id profilu (tab. PROFILE)
        private int? _mat_Id;     // id gat. mat. (tab. MATERIAL)
        private int? _pnt_Id;     // id painting (tab. PAINTING | PAINTING.PNT_NOM: lp$uwaga$mod)
        private float _pce_Len;   // długość
        private float _pce_Wdt;   // szerokość
        private string _pce_Des;  // info o użytkowniku
        private string _pce_Cmt1; // nr przewodnika
        private string _pce_Cmt2; // uwagi inne
        private string _pce_Cmt3; // uwagi inne (zlec. short?)
        private int? _grp_Id;     // id grupy analit. (tab. GRPFAB)
        // pola dla 'lazy load'
        private string _nrZlec;
        private string _gatMat;
        private string _grAnal;
        private string _lpUwagaMod;
        private int _assId;
        //private List<OperacjaPLM> _operacjePLM; // z tych operacji chyba nie korzystamy w Skanerach ?
        // PUBLIC
        public string Nazwa => _pce_Nam;
        public string NrPrzewodnika => _pce_Cmt1;
        public string Material { get; private set; }
        public string KatMat { get; private set; } // kategoria materiału
        public double Dlugosc => _pce_Len;
        public double Szerokosc => _pce_Wdt; // == 0, jeśli materiał nie jest blachą
        public int Szt { get; private set; }
        public string Lp { get; private set; } // == "", jeśli detal nie jest luzem
        public string Grupa { get; private set; }
        public bool Luzem => !Lp.IsNullOrEmpty();
        // lazy load
        public string NrZlec => _nrZlec ??= SqlService.PobierzPojedynczyString(BazaDanych.Plm, "PROJECT", "COM_NAM", $"COM_ID = {_com_Id}", out string _).Trim();
        public string Gatunek => _gatMat ??= SqlService.PobierzPojedynczyString(BazaDanych.Plm, "MATERIAL", "MAT_NAM", $"MAT_ID='{_mat_Id}'", out string _).Trim();
        public string GrAnal => _grAnal ??= SqlService.PobierzPojedynczyString(BazaDanych.Plm, "GRPFAB", "GRP_NAM", $"GRP_ID='{_grp_Id}'", out string _).Trim();
        public string LpUwagaMod => _lpUwagaMod ??= SqlService.PobierzPojedynczyString(BazaDanych.Plm, "PAINTING", "PNT_NOM", $"PNT_ID='{_pnt_Id}'", out string _).Trim();
        public int AssId {
            get {
                if (_assId != 0) return _assId;
                string assIdTxt = SqlService.PobierzPojedynczyString(BazaDanych.Plm, "KEYASS", "ASS_ID", $"PCE_ID='{_pce_Id}'", out string blad);
                _assId = blad.IsNullOrEmpty() ? int.Parse(assIdTxt) : -1;
                return _assId;
            }
        }
        // pomocnicze
        public bool DanePodstawoweWczytanePoprawnie { get; }
        //public bool DaneDodatkoweWczytanePoprawnie { get; }
        #endregion

        #region CONSTRUCTOR
        private DetalPLM(string[] daneDetalu) {
            if (daneDetalu.Length == 13) {
                DanePodstawoweWczytanePoprawnie = true;
                UstawPolaDanychPodst(daneDetalu);
                WczytajDaneDodatkowe();
            }
            else {
                DanePodstawoweWczytanePoprawnie = false;
            }
        }
        // !!! Tworzenie obiektów w fabryce
        #endregion

        /// <summary> daneDetalu: "PCE_ID","COM_ID","PCE_NAM","PRF_ID","MAT_ID","PNT_ID","PCE_LEN","PCE_WDT","PCE_DES","PCE_CMT1","PCE_CMT2","PCE_CMT3","GRP_ID" </summary>
        private void UstawPolaDanychPodst(string[] daneDetalu) {
            _pce_Id   = int.Parse(daneDetalu[0]);
            _com_Id   = int.Parse(daneDetalu[1]);
            _pce_Nam  = daneDetalu[2].Trim();
            _prf_Id   = daneDetalu[3].ParseToIntOrNull();
            _mat_Id   = daneDetalu[4].ParseToIntOrNull();
            _pnt_Id   = daneDetalu[5].ParseToIntOrNull();
            _pce_Len  = float.Parse(daneDetalu[6]);
            _pce_Wdt  = float.Parse(daneDetalu[7]);
            _pce_Des  = daneDetalu[8].Trim();
            _pce_Cmt1 = daneDetalu[9].Trim();
            _pce_Cmt2 = daneDetalu[10].Trim();
            _pce_Cmt3 = daneDetalu[11].Trim();
            _grp_Id   = daneDetalu[12].ParseToIntOrNull();
        }
        private static bool WczytajDaneDetaluWgNazwyDetalu(int idZlec, string nazwaDetalu, out string[] daneDetalu) {
            daneDetalu = null;
            if (idZlec < 0 || nazwaDetalu.IsNullOrEmpty()) return false;
            IEnumerable<string[]> detaleZlecenia = SqlService.PobierzDaneZBazy(BazaDanych.Plm, "PART", _kolumnyTabeliPartDoWczytania, $"COM_ID='{idZlec}'", out string blad);
            // znajdowanie detalu utrudnione ze względu na dodatkow spacje w nazwach detali w bazie
            List<string[]> szukaneDetale = detaleZlecenia.Where(d => d[2].Trim() == nazwaDetalu).ToList();
            if (!string.IsNullOrEmpty(blad) || szukaneDetale.Count != 1) return false;
            daneDetalu = szukaneDetale.Single();
            return true;
        }
        private static bool WczytajDaneDetaluWgNumeruPrzew(int idZlec, string nrPrzew, out string[] daneDetalu) {
            daneDetalu = null;
            if (idZlec < 0 || nrPrzew.IsNullOrEmpty()) return false;
            return SqlService.PobierzPojedynczyWiersz(BazaDanych.Plm, "PART", _kolumnyTabeliPartDoWczytania, $"COM_ID = '{idZlec}' AND PCE_CMT1 = '{nrPrzew}'", out daneDetalu);
            //IEnumerable<string[]> szukaneDetale = SqlService.PobierzDaneZBazy(BazaDanych.Plm, "PART", _kolumnyTabeliPartDoWczytania, $"COM_ID = '{idZlec}' AND PCE_CMT1 = '{nrPrzew}'",
            //                                                                  out string blad).ToList();
            //if (!string.IsNullOrEmpty(blad) || szukaneDetale.Count() != 1) return false;
            //daneDetalu = szukaneDetale.Single();
            //return true;
        }
        private void WczytajDaneDodatkowe() {
            WczytajDaneMaterialu();
            WczytajSztuki(); // przy okazji również AssId
            WczytajLp();     // przy okazji również LpUwagaMod
            Grupa = SqlService.PobierzPojedynczyString(BazaDanych.Plm, "ASSEMBLY", "ASS_NAM", $"ASS_ID='{AssId}'", out string _).Trim();
        }
        private void WczytajDaneMaterialu() {
            //IEnumerable<string[]> materialy = SqlService.PobierzDaneZBazy(BazaDanych.Plm).ToList();
            if (SqlService.PobierzPojedynczyWiersz(BazaDanych.Plm, "PROFILE", new[] {"PRF_CAT", "PRF_NAM"}, $"PRF_ID='{_prf_Id}'", out string[] material)) { //string.IsNullOrEmpty(blad) && materialy.Count() == 1 
                KatMat   = material[0];
                Material = material[1].Trim();
            }
            else {
                Material = "[brak danych]";
                KatMat   = "[brak danych]";
            }
        }
        private void WczytajSztuki() {
            //IEnumerable<string[]> keyassDB = SqlService.PobierzDaneZBazy(BazaDanych.Plm, "KEYASS", new[] {"KEYR_QTY"}, $"PCE_ID='{_pce_Id}'", out string blad1).ToList();
            string keyassDB = SqlService.PobierzPojedynczyString(BazaDanych.Plm, "KEYASS", "KEYR_QTY", $"PCE_ID='{_pce_Id}'", out string blad1);
            if (string.IsNullOrEmpty(blad1)) { // && keyassDB.Count() == 1
                //IEnumerable<string[]> keydwgDB = SqlService.PobierzDaneZBazy(BazaDanych.Plm, "KEYDWG", new[] {"KEYD_QTY"}, $"ASS_ID='{AssId}'", out string blad2).ToList();
                string keydwgDB = SqlService.PobierzPojedynczyString(BazaDanych.Plm, "KEYDWG", "KEYD_QTY", $"ASS_ID='{AssId}'", out string blad2);
                if (!string.IsNullOrEmpty(blad2)) { // || keydwgDB.Count() != 1
                    Szt = 1;
                    return;
                }
                if (int.TryParse(keyassDB, out int keyr_qty) && int.TryParse(keydwgDB, out int keyd_qty)) {
                    Szt = keyr_qty * keyd_qty;
                }
                else {
                    Szt = 1;
                }
            }
            else {
                Szt = 1;
            }
        }
        private void WczytajLp() {
            string[] lumSplit = LpUwagaMod.Split('$');
            switch (lumSplit.Length) {
                case 0:
                case 1: // MOD
                    Lp = "";
                    break;
                case 2: // lp$uwaga - przy czym jedno i drugie może być ""
                case 3: // lp$uwaga$mod
                    Lp = lumSplit[0];
                    break;
            }
        }
    }
}
