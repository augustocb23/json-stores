using System.Runtime.InteropServices;
using Xunit;

namespace JsonStores.Tests.Helpers
{
    public sealed class WindowsOnlyFact : FactAttribute
    {
        public WindowsOnlyFact()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Skip = "This test is only available on Windows.";
        }
    }
}