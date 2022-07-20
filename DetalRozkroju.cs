namespace ModelPLM
{
    // tabela NESTITEM w bazie PLM >> powiązanie z NESTDET > PART (Detal)
    public class DetalRozkroju {

        public DetalPLM Detal { get; private set; }
        public int SztWJednymRozkroju { get; }
        // UWAGA ! ilość detali w NESTING = SUMA (SztWJednymRozkroju(DetalRozkroju) * KrotnoscRozkroju) wszystkich NESTBAR`ów NESTING`u

        public bool DetalWczytanyPoprawnie { get; }

        public DetalRozkroju(int pceId, int sztWrozkroju) {
            SztWJednymRozkroju     = sztWrozkroju;
            DetalWczytanyPoprawnie = WczytajDetal(pceId);
        }

        private bool WczytajDetal(int pceId) {
            try {
                Detal = DetalPLM.Factory.NowyDetalWgPceId(pceId);
                return Detal.DanePodstawoweWczytanePoprawnie;
            }
            catch {
                return false;
            }
        }
    }
}
