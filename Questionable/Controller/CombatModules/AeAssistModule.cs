using System.Linq;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin;
using Microsoft.Extensions.Logging;

namespace Questionable.Controller.CombatModules;

internal sealed class AeAssistModule
(
    ILogger<AeAssistModule> logger,
    IDalamudPluginInterface pluginInterface,
    Configuration configuration
) : ICombatModule
{
    public bool CanHandleFight(CombatController.CombatData combatData)
    {
        if (configuration.General.CombatModule != Configuration.ECombatModule.AeAssist)
            return false;

        bool isLoaded = pluginInterface.InstalledPlugins.Any(x =>
            x.InternalName == "AEAssist" && x.IsLoaded);
        if (!isLoaded)
            logger.LogWarning("AE Assist is selected but not loaded");

        return isLoaded;
    }

    public bool Start(CombatController.CombatData combatData)
    {
        logger.LogInformation("Starting combat with AE Assist");
        return true;
    }

    public bool Stop()
    {
        logger.LogInformation("Stopping combat with AE Assist");
        return true;
    }

    public void Update(IGameObject nextTarget)
    {
    }

    public bool CanAttack(IBattleNpc target) => true;
}
