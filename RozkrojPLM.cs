using BasicSqlService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelPLM
{
    // tabela NESTBAR w bazie PLM
    public class RozkrojPLM {

        public List<DetalRozkroju> DetaleRozkroju { get; } = new();
        public int BarId { get; private set; }              // BAR_ID  -> tabele: NESTBAR | NESTITEM
        public int NesId { get; private set; }              // NES_ID  -> tabele: NESTBAR | NESTING | NESTDET
        public int NrKolejnyWGrupie { get; private set; }   // BAR_IDT -> tabela: NESTBAR
        public int KrotnoscRozkroju { get; private set; }   // BAR_QTY -> tabela: NESTBAR
        // UWAGA ! ilość detali w NESTING = SUMA (SztWJednymRozkroju(DetalRozkroju) * KrotnoscRozkroju) wszystkich NESTBAR`ów NESTING`u
        // !!! W ROZKROJU MOGĄ BYĆ NIE WSZYSTKIE SZT. DETALI !!!
        public bool DaneWczytanePoprawnie { get; }
        public bool DetaleWczytanePoprawnie { get; }
        public bool RozkrojWczytanyPoprawnie => DaneWczytanePoprawnie && DetaleWczytanePoprawnie;

        /// <summary> Rozkroj PLM </summary>
        /// <param name="barId"> Id rozkroju w bazie </param>
        public RozkrojPLM(int barId) {
            DaneWczytanePoprawnie   = WczytajDanePodstDB(barId);
            DetaleWczytanePoprawnie = DaneWczytanePoprawnie && WczytajDetaleRozkroju();
        }

        private bool WczytajDanePodstDB(int barId) {
            bool ok = SqlService.PobierzPojedynczyWiersz(BazaDanych.Plm, "NESTBAR", new[] {"NES_ID", "BAR_IDT", "BAR_QTY"}, $"BAR_ID='{barId}'", out string[] rozkrojDB);
            if (!ok) return false;
            if (!int.TryParse(rozkrojDB[0], out int nesId)) return false;
            if (!int.TryParse(rozkrojDB[1], out int barIdt)) return false;
            if (!int.TryParse(rozkrojDB[2], out int barQty)) return false;
            BarId  = barId;
            NesId  = nesId;
            NrKolejnyWGrupie  = barIdt;
            KrotnoscRozkroju = barQty;
            return true;
        }
        private bool WczytajDetaleRozkroju() {
            // wszystkie detale NESTINGu (NESTDETs):
            List<Nestdet>  detaleNestingu = NestingPLM.WczytajDetaleNestdet(NesId);
            // detale rozkroju NESTBAR (NESTITEMs):
            IEnumerable<string[]> nestItemsDB    = SqlService.PobierzDaneZBazy(BazaDanych.Plm, "NESTITEM", new[] {"NED_ID", "ITM_QTY"}, $"BAR_ID='{BarId}'", out string blad);
            if (!blad.IsNullOrEmpty()) return false;
            try {
                foreach (string[] nestitemDB in nestItemsDB) {
                    Nestdet nestdet = detaleNestingu.Single(nd => nd.NedId.ToString().Equals(nestitemDB[0]));
                    DetaleRozkroju.Add(new DetalRozkroju(nestdet.PceId, int.TryParse(nestitemDB[1], out int szt) ? szt : -1));
                }
            }
            catch {
                return false;
            }
            return true;
        }
    }

}
