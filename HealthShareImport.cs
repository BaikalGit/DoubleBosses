using MonoMod.ModInterop;

namespace DoubleBosses
{
    internal static class HealthShareImport
    {
        #pragma warning disable CS8632
        [ModImportName(nameof(HealthShare))]
        private static class HealthShare
        {
            public static Func<IEnumerable<GameObject>, int, string, MonoBehaviour>? ShareHealth = null!;
        }

        static HealthShareImport() =>
            typeof(HealthShare).ModInterop();

        internal static void ShareHealth(this IEnumerable<GameObject> self, int initialHealth, string name) =>
            HealthShare.ShareHealth?.Invoke(self, initialHealth, name);
    }
}