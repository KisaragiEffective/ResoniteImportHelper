namespace ResoniteImportHelper.Internal.DependencyManager
{
    public sealed class Args(string From, string To)
    {
    }

    public sealed class Program
    {
        private static void Main(string[] args)
        {
            Main0(new Args(args[0], args[1]));
        }

        public static void Main0(Args args)
        {
            // TODO: dotnet publish
        }
    }
}
