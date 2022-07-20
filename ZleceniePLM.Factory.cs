using BasicSqlService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelPLM
{
    public partial class ZleceniePLM {

        // Bufor danych z bazy (tabela PROJECT)
        private static List<string[]> _daneZlecenDB; //   "COM_ID" | "COM_NAM" | "COM_DES" | "COM_OBJ"  -- UWAGA! NAM - stała długość pola (31 znaków); DES i OBJ can be null
        public static IEnumerable<string[]> DaneZlecenDB => _daneZlecenDB ??= WczytajDaneZlecenDB();

        private static List<string[]> WczytajDaneZlecenDB() {
            IEnumerable<string[]> zleceniaDB = SqlService.PobierzDaneZBazy(BazaDanych.Plm,
                                                                           "PROJECT",
                                                                           new[] {"COM_ID", "COM_NAM", "COM_DES", "COM_OBJ"},
                                                                           string.Empty,
                                                                           out string blad);
            return blad.IsNullOrEmpty() ? zleceniaDB.Select(z => new[]{ z[0], z[1].Trim(), z[2]?.Trim() ?? string.Empty, z[3]?.Trim() ?? string.Empty }).ToList() : new List<string[]>();
        }

        public static class Factory {

            public static ZleceniePLM NoweZlecenieWgNumeruZl(string nrZlec) {
                ZleceniePLM noweZlec = new();
                try {
                    string[] daneDB = DaneZlecenDB.Single(d => d[1].Trim().Equals(nrZlec));
                    noweZlec.ZlecenieWczytanePopr = noweZlec.SetProperties(daneDB);
                }
                catch {
                    noweZlec.ZlecenieWczytanePopr = false;
                }
                return noweZlec;
            }
            public static ZleceniePLM NoweZlecenieWgKoduZl(int kodZlec) {
                ZleceniePLM    noweZlec = new();
                try {
                    string[] daneDB = DaneZlecenDB.Single(d => d[2].Trim().Equals($"{kodZlec:D3}"));
                    noweZlec.ZlecenieWczytanePopr = noweZlec.SetProperties(daneDB);
                }
                catch {
                    noweZlec.ZlecenieWczytanePopr = false;
                }
                return noweZlec;
            }

        }
    }
}
