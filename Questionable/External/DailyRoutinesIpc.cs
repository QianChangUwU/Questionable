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

    private readonly ICallGateSubscriber<string, bool?> _isModuleEnabled;
    private readonly ICallGateSubscriber<string, bool, bool> _loadModule;
    private readonly ICallGateSubscriber<string, bool, bool, bool> _unloadModule;

    private readonly Dictionary<string, bool> _modulesEnabledByUs = new();
    private bool _autoTalkSkipDisabledByUs;
    private bool _wasAutoTalkSkipEnabled;
    private bool _autoCutsceneSkipDisabledByUs;
    private bool _wasAutoCutsceneSkipEnabled;

    private static readonly string[] ModulesToEnable =
    [
        "AutoSnipeQuest",
        "AutoCancelNPCEmote",
        "IgnoreTransparencyWait",
        "IgnoreTurnAndLookAtWait"
    ];

    private const string AutoTalkSkipModule = "AutoTalkSkip";
    private const string AutoCutsceneSkipModule = "AutoCutsceneSkip";

    public DailyRoutinesIpc(
        IDalamudPluginInterface pluginInterface,
        IFramework framework,
        ILogger<DailyRoutinesIpc> logger,
        IChatGui chatGui,
        QuestController questController)
    {
        _framework = framework;
        _logger = logger;
        _chatGui = chatGui;
        _questController = questController;

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
            HandleAutoTalkSkipDisable();
            HandleAutoCutsceneSkipDisable();
            HandleHelperModulesEnable();
        }
        else
        {
            RestoreModules();
        }
    }

    private void HandleAutoTalkSkipDisable()
    {
        if (_autoTalkSkipDisabledByUs)
            return;

        bool? enabled = IsModuleEnabled(AutoTalkSkipModule);
        if (enabled == true)
        {
            _wasAutoTalkSkipEnabled = true;
            _autoTalkSkipDisabledByUs = true;
            UnloadModule(AutoTalkSkipModule);
            _chatGui.Print(
                _L("DailyRoutines AutoTalkSkip has been temporarily disabled to avoid conflicts with Questionable."),
                CommandHandler.MessageTag, CommandHandler.TagColor);
            _logger.LogInformation("Disabled DailyRoutines AutoTalkSkip module due to Questionable automation");
        }
    }

    private void HandleAutoCutsceneSkipDisable()
    {
        if (_autoCutsceneSkipDisabledByUs)
            return;

        bool? enabled = IsModuleEnabled(AutoCutsceneSkipModule);
        if (enabled == true)
        {
            _wasAutoCutsceneSkipEnabled = true;
            _autoCutsceneSkipDisabledByUs = true;
            UnloadModule(AutoCutsceneSkipModule);
            _chatGui.Print(
                _L("DailyRoutines AutoCutsceneSkip has been temporarily disabled to avoid conflicts with Questionable."),
                CommandHandler.MessageTag, CommandHandler.TagColor);
            _logger.LogInformation("Disabled DailyRoutines AutoCutsceneSkip module due to Questionable automation");
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
                _chatGui.Print(
                    _LF("DailyRoutines {0} has been temporarily enabled for Questionable.", module),
                    CommandHandler.MessageTag, CommandHandler.TagColor);
                _logger.LogInformation("Enabled DailyRoutines {Module} module for Questionable automation", module);
            }
        }
    }

    private void RestoreModules()
    {
        if (!IPCSubscriber.IsInstalled(PluginName))
            return;

        if (_autoTalkSkipDisabledByUs)
        {
            if (_wasAutoTalkSkipEnabled)
            {
                LoadModule(AutoTalkSkipModule);
                _chatGui.Print(
                    _L("DailyRoutines AutoTalkSkip has been re-enabled."),
                    CommandHandler.MessageTag, CommandHandler.TagColor);
                _logger.LogInformation("Re-enabled DailyRoutines AutoTalkSkip module");
            }

            _autoTalkSkipDisabledByUs = false;
            _wasAutoTalkSkipEnabled = false;
        }

        if (_autoCutsceneSkipDisabledByUs)
        {
            if (_wasAutoCutsceneSkipEnabled)
            {
                LoadModule(AutoCutsceneSkipModule);
                _chatGui.Print(
                    _L("DailyRoutines AutoCutsceneSkip has been re-enabled."),
                    CommandHandler.MessageTag, CommandHandler.TagColor);
                _logger.LogInformation("Re-enabled DailyRoutines AutoCutsceneSkip module");
            }

            _autoCutsceneSkipDisabledByUs = false;
            _wasAutoCutsceneSkipEnabled = false;
        }

        foreach (string module in _modulesEnabledByUs.Keys)
        {
            UnloadModule(module);
            _chatGui.Print(
                _LF("DailyRoutines {0} has been disabled.", module),
                CommandHandler.MessageTag, CommandHandler.TagColor);
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
