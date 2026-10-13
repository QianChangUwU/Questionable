// Authored with LLM assistance, changes must be reviewed and owned by a human.
// Initial version reviewed and owned by @QianChangUwU
using Dalamud.Plugin.Ipc;
using static Questionable.External.IPCUtils;

namespace Questionable.External;

[RegisterSingleton]
internal sealed class DailyRoutinesIpc : IDisposable
{
    private const string PluginName = "DailyRoutines";

    private readonly IFramework _framework;
    private readonly ILogger<DailyRoutinesIpc> _logger;
    private readonly IChatGui _chatGui;
    private readonly QuestController _questController;
    private readonly Configuration _configuration;

    private readonly ICallGateSubscriber<string, bool?> _isModuleEnabled;
    private readonly ICallGateSubscriber<string, bool, bool> _loadModule;
    private readonly ICallGateSubscriber<string, bool, bool, bool> _unloadModule;

    private readonly Dictionary<string, bool> _modulesEnabledByUs = new();

    /// <summary>Modules that must be turned OFF during automation (they conflict with QST).</summary>
    private static readonly string[] ModulesToDisable =
    [
        "AutoTalkSkip",
        "AutoCutsceneSkip",
        "OptimizedInteraction"
    ];

    private readonly Dictionary<string, bool> _conflictModulesDisabledByUs = new();

    private static readonly string[] ModulesToEnable =
    [
        "AutoSnipeQuest",
        "AutoCancelNPCEmote",
        "IgnoreTransparencyWait",
        "IgnoreTurnAndLookAtWait"
    ];

    private static readonly Dictionary<string, string> ModuleDisplayNames = new()
    {
        { "AutoTalkSkip", "自动跳过对话" },
        { "AutoCutsceneSkip", "自动跳过过场动画" },
        { "OptimizedInteraction", "游戏原生交互优化" },
        { "AutoSnipeQuest", "自动完成观察任务" },
        { "AutoCancelNPCEmote", "自动取消NPC情感动作播放" },
        { "IgnoreTransparencyWait", "自动取消NPC淡入淡出等待" },
        { "IgnoreTurnAndLookAtWait", "自动取消NPC转身及视线调整等待" }
    };

    private static string GetModuleDisplayName(string module) =>
        ModuleDisplayNames.TryGetValue(module, out string? name) ? name : module;

    private void NotifyUser(string message)
    {
        if (!_configuration.Notifications.DailyRoutinesModuleNotifications)
            return;

        _chatGui.Print(message, CommandHandler.MessageTag, CommandHandler.TagColor);
    }

    public DailyRoutinesIpc(
        IDalamudPluginInterface pluginInterface,
        IFramework framework,
        ILogger<DailyRoutinesIpc> logger,
        IChatGui chatGui,
        QuestController questController,
        Configuration configuration)
    {
        _framework = framework;
        _logger = logger;
        _chatGui = chatGui;
        _questController = questController;
        _configuration = configuration;

        _isModuleEnabled = pluginInterface.GetIpcSubscriber<string, bool?>($"{PluginName}.IsModuleEnabled");
        _loadModule = pluginInterface.GetIpcSubscriber<string, bool, bool>($"{PluginName}.LoadModule");
        _unloadModule = pluginInterface.GetIpcSubscriber<string, bool, bool, bool>($"{PluginName}.UnloadModule");

        _framework.Update += OnUpdate;
    }

    public void Dispose()
    {
        _framework.Update -= OnUpdate;
        RestoreModules();
    }

    private void OnUpdate(IFramework framework)
    {
        if (!IPCSubscriber.IsInstalled(PluginName))
            return;

        if (!EzThrottler.Throttle("DailyRoutinesIpc.Check", 1000))
            return;

        bool hasActiveQuest = _questController.IsRunning ||
                              _questController.AutomationType != QuestController.EAutomationType.Manual;

        if (hasActiveQuest)
        {
            HandleConflictModulesDisable();
            HandleHelperModulesEnable();
        }
        else
        {
            RestoreModules();
        }
    }

    private void HandleConflictModulesDisable()
    {
        foreach (string module in ModulesToDisable)
        {
            if (_conflictModulesDisabledByUs.ContainsKey(module))
                continue;

            bool? enabled = IsModuleEnabled(module);
            if (enabled == true)
            {
                UnloadModule(module);
                _conflictModulesDisabledByUs[module] = true;
                NotifyUser(
                    _LF("DailyRoutines {0} has been temporarily disabled to avoid conflicts with Questionable.",
                        GetModuleDisplayName(module)));
                _logger.LogInformation("Disabled DailyRoutines {Module} module due to Questionable automation", module);
            }
        }
    }

    private void HandleHelperModulesEnable()
    {
        foreach (string module in ModulesToEnable)
        {
            if (_modulesEnabledByUs.ContainsKey(module))
                continue;

            bool? enabled = IsModuleEnabled(module);
            if (enabled == false)
            {
                LoadModule(module);
                _modulesEnabledByUs[module] = true;
                NotifyUser(
                    _LF("DailyRoutines {0} has been temporarily enabled for Questionable.", GetModuleDisplayName(module)));
                _logger.LogInformation("Enabled DailyRoutines {Module} module for Questionable automation", module);
            }
        }
    }

    private void RestoreModules()
    {
        if (!IPCSubscriber.IsInstalled(PluginName))
            return;

        foreach (string module in _conflictModulesDisabledByUs.Keys)
        {
            LoadModule(module);
            NotifyUser(
                _LF("DailyRoutines {0} has been re-enabled.", GetModuleDisplayName(module)));
            _logger.LogInformation("Re-enabled DailyRoutines {Module} module", module);
        }

        _conflictModulesDisabledByUs.Clear();

        foreach (string module in _modulesEnabledByUs.Keys)
        {
            UnloadModule(module);
            NotifyUser(
                _LF("DailyRoutines {0} has been disabled.", GetModuleDisplayName(module)));
            _logger.LogInformation("Disabled DailyRoutines {Module} module (restoring original state)", module);
        }

        _modulesEnabledByUs.Clear();
    }

    private bool? IsModuleEnabled(string module)
    {
        try
        {
            if (!_isModuleEnabled.HasFunction)
                return null;
            return _isModuleEnabled.InvokeFunc(module);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Failed to check DailyRoutines {Module} status", module);
            return null;
        }
    }

    private void UnloadModule(string module)
    {
        try
        {
            if (_unloadModule.HasFunction)
                _unloadModule.InvokeFunc(module, false, false);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Failed to disable DailyRoutines {Module}", module);
        }
    }

    private void LoadModule(string module)
    {
        try
        {
            if (_loadModule.HasFunction)
                _loadModule.InvokeFunc(module, false);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Failed to enable DailyRoutines {Module}", module);
        }
    }
}
