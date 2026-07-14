using System.Linq;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin;
using Microsoft.Extensions.Logging;
using Questionable.Functions;
using Questionable.Model.Common;

namespace Questionable.Controller.CombatModules;

internal sealed class AeAssistModule
(
    ILogger<AeAssistModule> logger,
    ChatFunctions chatFunctions,
    IDalamudPluginInterface pluginInterface,
    Configuration configuration
) : ICombatModule
{
    private bool _active;

    public bool CanHandleFight(CombatController.CombatData combatData)
    {
        if (configuration.General.CombatModule != ECombatModule.AeAssist)
            return false;

        bool isLoaded = pluginInterface.InstalledPlugins.Any(x =>
            x.InternalName == "AEAssistV3" && x.IsLoaded);
        if (!isLoaded)
            logger.LogWarning("AE Assist is selected but not loaded");

        return isLoaded;
    }

    public bool Start(CombatController.CombatData combatData)
    {
        if (_active)
            return true;

        logger.LogInformation("Starting combat with AE Assist");
        chatFunctions.ExecuteCommand("/aepull on");
        chatFunctions.ExecuteCommand("/aeTargetSelector on");
        chatFunctions.ExecuteCommand("/aeTargetSelector mode6");
        chatFunctions.ExecuteCommand("/aeTargetSelector NoTargetOnly off");
        _active = true;
        return true;
    }

    public bool Stop()
    {
        if (!_active)
            return true;

        logger.LogInformation("Stopping combat with AE Assist");
        chatFunctions.ExecuteCommand("/aeTargetSelector off");
        chatFunctions.ExecuteCommand("/aepull off");
        _active = false;
        return true;
    }

    public void Update(IGameObject nextTarget)
    {
    }

    public bool CanAttack(IBattleNpc target) => true;
}
