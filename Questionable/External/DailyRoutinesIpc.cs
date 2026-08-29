// Authored with LLM assistance, changes must be reviewed and owned by a human.
// Initial version reviewed and owned by @QianChangUwU
using Dalamud.Plugin.Ipc;
using static Questionable.External.IPCUtils;

namespace Questionable.External;

[RegisterSingleton]
internal sealed class DailyRoutinesIpc : IDisposable
{
    private const string PluginName = "DailyRoutines";
    private const string ModuleName = "AutoTalkSkip";

    private readonly IFramework _framework;
    private readonly ILogger<DailyRoutinesIpc> _logger;
    private readonly IChatGui _chatGui;
    private readonly QuestController _questController;

    private readonly ICallGateSubscriber<string, bool?> _isModuleEnabled;
    private readonly ICallGateSubscriber<string, bool, bool> _loadModule;
    private readonly ICallGateSubscriber<string, bool, bool, bool> _unloadModule;

    private bool _wasAutoTalkSkipEnabled;
    private bool _autoTalkSkipDisabledByUs;

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
        RestoreAutoTalkSkip();
    }

    private void OnUpdate(IFramework framework)
    {
        if (!IPCSubscriber.IsInstalled(PluginName))
            return;

        if (!EzThrottler.Throttle("DailyRoutinesIpc.Check", 1000))
            return;

        bool hasActiveQuest = _questController.IsRunning ||
                              _questController.AutomationType != QuestController.EAutomationType.Manual;

        if (hasActiveQuest && !_autoTalkSkipDisabledByUs)
        {
            bool? enabled = IsAutoTalkSkipEnabled();
            if (enabled == true)
            {
                _wasAutoTalkSkipEnabled = true;
                _autoTalkSkipDisabledByUs = true;
                UnloadAutoTalkSkip();
                _chatGui.Print(
                    _L("DailyRoutines AutoTalkSkip has been temporarily disabled to avoid conflicts with Questionable."),
                    CommandHandler.MessageTag, CommandHandler.TagColor);
                _logger.LogInformation("Disabled DailyRoutines AutoTalkSkip module due to Questionable automation");
            }
        }
        else if (!hasActiveQuest && _autoTalkSkipDisabledByUs)
        {
            RestoreAutoTalkSkip();
        }
    }

    private void RestoreAutoTalkSkip()
    {
        if (!_autoTalkSkipDisabledByUs || !IPCSubscriber.IsInstalled(PluginName))
            return;

        if (_wasAutoTalkSkipEnabled)
        {
            LoadAutoTalkSkip();
            _chatGui.Print(
                _L("DailyRoutines AutoTalkSkip has been re-enabled."),
                CommandHandler.MessageTag, CommandHandler.TagColor);
            _logger.LogInformation("Re-enabled DailyRoutines AutoTalkSkip module");
        }

        _autoTalkSkipDisabledByUs = false;
        _wasAutoTalkSkipEnabled = false;
    }

    private bool? IsAutoTalkSkipEnabled()
    {
        try
        {
            if (!_isModuleEnabled.HasFunction)
                return null;
            return _isModuleEnabled.InvokeFunc(ModuleName);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Failed to check DailyRoutines AutoTalkSkip status");
            return null;
        }
    }

    private void UnloadAutoTalkSkip()
    {
        try
        {
            if (_unloadModule.HasFunction)
                _unloadModule.InvokeFunc(ModuleName, false, false);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Failed to disable DailyRoutines AutoTalkSkip");
        }
    }

    private void LoadAutoTalkSkip()
    {
        try
        {
            if (_loadModule.HasFunction)
                _loadModule.InvokeFunc(ModuleName, false);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Failed to re-enable DailyRoutines AutoTalkSkip");
        }
    }
}
