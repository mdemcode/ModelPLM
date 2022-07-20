using BasicSqlService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelPLM
{
    public partial class DetalPLM {

        public static class Factory {

            /// <summary> Detal PLM </summary>
            /// <param name="daneDetalu"> { "PCE_ID", "COM_ID", "PCE_NAM", "PRF_ID", "MAT_ID", "PNT_ID", "PCE_LEN", "PCE_WDT", "PCE_DES", "PCE_CMT1", "PCE_CMT2", "PCE_CMT3", "GRP_ID" } </param>
            public static DetalPLM NowyDetalWgDanychDetalu(string[] daneDetalu) {
                return new DetalPLM(daneDetalu);
            }

            /// <summary> Detal PLM </summary>
            /// <param name="pceId"> Id detalu w bazie (PCE_ID w tabeli PART) </param>
            public static DetalPLM NowyDetalWgPceId(int pceId) {
                bool ok = SqlService.PobierzPojedynczyWiersz(BazaDanych.Plm, "PART", _kolumnyTabeliPartDoWczytania, $"PCE_ID ='{pceId}'", out string[] daneDetalu);
                return ok ? new DetalPLM(daneDetalu) : new DetalPLM(new[] {""}); // jeśli nie ok to ustawi DanePodstawoweWczytanePoprawnie na false
            }

            /// <summary> Detal PLM </summary>
            /// <param name="idZlec"> Id zlecenia w bazie (COM_ID w tabeli PROJECT) </param>
            /// <param name="nazwaDetalu"> Nazwa detalu w bazie (PCE_NAM w tabeli PART) </param>
            public static DetalPLM NowyDetalWgIdZlecINazwyDetalu(int idZlec, string nazwaDetalu) {
                bool ok = WczytajDaneDetaluWgNazwyDetalu(idZlec, nazwaDetalu, out string[] daneDetalu);
                return ok ? new DetalPLM(daneDetalu) : new DetalPLM(new[] {""}); // jeśli nie ok to ustawi DanePodstawoweWczytanePoprawnie na false
            }

            /// <summary> Detal PLM </summary>
            /// <param name="idZlec"> Id zlecenia w bazie (COM_ID w tabeli PROJECT) </param>
            /// <param name="nrPrzewodnika"> Nr przewodnika detalu w bazie (PCE_CMT1 w tabeli PART) </param>
            public static DetalPLM NowyDetalWgIdZlecINumeruPrzewodnika(int idZlec, string nrPrzewodnika) {
                bool ok = WczytajDaneDetaluWgNumeruPrzew(idZlec, nrPrzewodnika, out string[] daneDetalu);
                return ok ? new DetalPLM(daneDetalu) : new DetalPLM(new[] {""}); // jeśli nie ok to ustawi DanePodstawoweWczytanePoprawnie na false
            }

        }
    }

}
