namespace ModelPLM
{
    public static class ToolsPlm {

        public static int? ParseToIntOrNull(this string input) => int.TryParse(input, out int result) ? result : null;

    }
}
