using Dalamud.Plugin;
using ECommons.DalamudServices;
using Questionable.Windows;

namespace Questionable.Controller;

internal static class ConfigurationExtensions
{
    internal static void Save(this Configuration configuration)
    {
        Svc.PluginInterface.SavePluginConfig(configuration);
    }
}
