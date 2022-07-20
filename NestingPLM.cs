using BasicSqlService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelPLM
{
    // Grupa rozkrojów - tabela NESTING w bazie PLM
    public class NestingPLM {

        private readonly int _nesId; // NES_ID
        // NES_TYP ?? typ rozkroju?

        // rozkroje z tabeli NESTBAR
        // private string _typRozkroju; // => Rozkroje.First().Blacha; true - blachy | false - profile
        public bool RozkrojeWczytanePoprawnie { get; }
        public List<RozkrojPLM> Rozkroje { get; } = new();

        // detale z tabeli NESTDET
        private List<Nestdet> _detaleNestingu;
        public List<Nestdet> DetaleNestingu => _detaleNestingu ??= WczytajDetaleNestdet(_nesId);
        // UWAGA ! Wszystlie DetaleNestingu są potrzebne tylko w niektórych sytuacjach. Przy wczytywaniu rozkrojów detale są wczytane do poszczególnych rozkrojów jako 'DetaleRozkroju'

        public NestingPLM(int nesId) {
            _nesId                    = nesId;
            RozkrojeWczytanePoprawnie = _nesId > 0 && WczytajRozkroje();
        }

        private bool WczytajRozkroje() {
            if (_nesId < 1) return false;
            IEnumerable<string[]> rozkrojeDB = SqlService.PobierzDaneZBazy(BazaDanych.Plm, "NESTBAR", new[] {"BAR_ID"}, $"NES_ID='{_nesId}'", out string blad).ToList();
            if (!blad.IsNullOrEmpty() || !rozkrojeDB.Any()) return false;
            List<int> numeryIdNestbarow = rozkrojeDB.Select(r => int.TryParse(r[0], out int nr) ? nr : -1).ToList();
            numeryIdNestbarow.ForEach(id => {
                if (id > 0) Rozkroje.Add(new RozkrojPLM(id));
            });
            return Rozkroje.Any();
        }
        public static List<Nestdet> WczytajDetaleNestdet(int nesId) {
            List<string[]> daneDB = SqlService.PobierzDaneZBazy(BazaDanych.Plm, "NESTDET", new[] {"NED_ID", "PCE_ID", "NED_QTY"}, $"NES_ID='{nesId}'", out string blad).ToList();
            if (!blad.IsNullOrEmpty()) return new List<Nestdet>();
            try {
                return daneDB.Select(d => new Nestdet(int.Parse(d[0]), nesId, int.Parse(d[2]), int.Parse(d[1]))).ToList();
            }
            catch {
                return new List<Nestdet>();
            }
        }
    }

    public record Nestdet(int NedId, int NesId, int NedQty, int PceId) {
        public int NedId { get; } = NedId;   // NED_ID (NESTDET id)
        public int NesId { get; } = NesId;   // NES_ID (NESTING id)
        public int NedQty { get; } = NedQty; // NED_QTY -> szt detalu w całym NESTINGU
        public int PceId { get; } = PceId;   // PCE_ID (PART id)
    }
}
