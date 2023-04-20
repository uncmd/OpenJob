using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Runtime.Loader;

namespace OpenJob;

public class AssemblyHelper
{
    public static IList<Assembly> GetAssemblies(ILogger logger = default)
    {
        var libs = DependencyContext.Default.CompileLibraries.Where(lib => !lib.Serviceable);
        return libs.Select(lib =>
        {
            try
            {
                return AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
            }
            catch (Exception ex)
            {
                if (logger != default)
                {
                    logger.LogWarning(ex, ex.Message);
                }

                return default;
            }
        }).Where(assembly => assembly != default).ToList();
    }
}
