using BasicSqlService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelPLM
{
    public partial class ProfilPLM {

        // Bufor danych z bazy (tabela PROFILE)
        private static List<string[]> _daneDB; // PRF_ID | PRF_CAT | PRF_NAM | PRF_H | PRF_B | PRF_C | PRF_A | PRF_E | PRF_R | PRF_R1 | PRF_PDS | PRF_SRF  -- UWAGA! NAM - stała długość pola (31 znaków) w bazie
        public static IEnumerable<string[]> DaneDB => _daneDB ??= WczytajDaneDB();

        private static List<string[]> WczytajDaneDB() {
            IEnumerable<string[]> profileDB = SqlService.PobierzDaneZBazy(BazaDanych.Plm,
                                                                           "PROFILE",
                                                                           new[] {"PRF_ID", "PRF_CAT", "PRF_NAM", "PRF_H", "PRF_B", "PRF_C", "PRF_A", "PRF_E", "PRF_R", "PRF_R1", "PRF_PDS", "PRF_SRF"},
                                                                           string.Empty,
                                                                           out string blad);
            return blad.IsNullOrEmpty() ? profileDB.Select(p => new[]{ p[0], p[1], p[2].Trim(), p[3], p[4], p[5], p[6], p[7], p[8], p[9], p[10], p[11] }).ToList() : new List<string[]>();
        }

        public static class Factory {

            public static ProfilPLM NowyProfilWgNazwy(string nazwa) {
                ProfilPLM nowyProfil = new();
                if (nazwa.IsNullOrEmpty()) {
                    nowyProfil.ProfilWczytanyPopr = false;
                    return nowyProfil;
                }
                try {
                    string[] daneDB = DaneDB.Single(d => d[2].Trim().Equals(nazwa));
                    nowyProfil.ProfilWczytanyPopr = nowyProfil.SetProperties(daneDB);
                }
                catch {
                    nowyProfil.ProfilWczytanyPopr = false;
                }
                return nowyProfil;
            }
            public static ProfilPLM NowyProfilWgIdDb(int idDb) {
                ProfilPLM nowyProfil = new();
                if (idDb < 1) {
                    nowyProfil.ProfilWczytanyPopr = false;
                    return nowyProfil;
                }
                try {
                    string[] daneDB = DaneDB.Single(d => d[0] == idDb.ToString());
                    nowyProfil.ProfilWczytanyPopr = nowyProfil.SetProperties(daneDB);
                }
                catch {
                    nowyProfil.ProfilWczytanyPopr = false;
                }
                return nowyProfil;
            }

        }

    }
}