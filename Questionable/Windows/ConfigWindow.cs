using System.Diagnostics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using Questionable.Windows.Common;
using Questionable.Windows.Common.Ui;
namespace Questionable.Windows;

[RegisterSingleton]
internal sealed class ConfigWindow
 : LWindow, IPersistableWindowConfig
{
    private readonly Configuration _configuration;
    private readonly DebugConfigComponent _debugConfigComponent;
    private readonly DutyConfigComponent _dutyConfigComponent;
    private readonly GeneralConfigComponent _generalConfigComponent;
    private readonly NotificationConfigComponent _notificationConfigComponent;
    private readonly PluginConfigComponent _pluginConfigComponent;
    private readonly IDalamudPluginInterface _pluginInterface;
    private readonly SinglePlayerDutyConfigComponent _singlePlayerDutyConfigComponent;
    private readonly StopConditionComponent _stopConditionComponent;

    public ConfigWindow(
    IDalamudPluginInterface pluginInterface,
    GeneralConfigComponent generalConfigComponent,
    PluginConfigComponent pluginConfigComponent,
    DutyConfigComponent dutyConfigComponent,
    SinglePlayerDutyConfigComponent singlePlayerDutyConfigComponent,
    StopConditionComponent stopConditionComponent,
    NotificationConfigComponent notificationConfigComponent,
    DebugConfigComponent debugConfigComponent,
    Configuration configuration) : base(_L("Config - Questionable") + "###QuestionableConfig")
    {
        _configuration = configuration;
        _debugConfigComponent = debugConfigComponent;
        _dutyConfigComponent = dutyConfigComponent;
        _generalConfigComponent = generalConfigComponent;
        _notificationConfigComponent = notificationConfigComponent;
        _pluginConfigComponent = pluginConfigComponent;
        _pluginInterface = pluginInterface;
        _singlePlayerDutyConfigComponent = singlePlayerDutyConfigComponent;
        _stopConditionComponent = stopConditionComponent;

        Size = new Vector2(400, 400);
        SizeCondition = ImGuiCond.Once;
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new(400, 400),
            MaximumSize = default
        };

        if (!_configuration.General.HideSponsorButton)
            TitleBarButtons.Add(new()
            {
                Icon = FontAwesomeIcon.Heart,
                IconOffset = TitleBarIconOffset,
                Click = _ => Process.Start(new ProcessStartInfo { FileName = "https://ko-fi.com/alydev", UseShellExecute = true }),
                Priority = TitleBarButtonPriority,
                ShowTooltip = () =>
                {
                    using ImRaii.TooltipDisposable _ = ImRaii.Tooltip();
                    ImGui.Text(_L("Sponsor QST development"));
                }
            });
    }
    public WindowConfig WindowConfig => _configuration.ConfigWindowConfig;

    public void SaveWindowConfig() => _pluginInterface.SavePluginConfig(_configuration);

    public override void DrawContent()
    {
        using ImRaii.TabBarDisposable tabBar = ImRaii.TabBar("QuestionableConfigTabs");
        if (!tabBar)
            return;

        _generalConfigComponent.DrawTab();
        _pluginConfigComponent.DrawTab();
        _dutyConfigComponent.DrawTab();
        _singlePlayerDutyConfigComponent.DrawTab();
        _stopConditionComponent.DrawTab();
        _notificationConfigComponent.DrawTab();
        _debugConfigComponent.DrawTab();
        using ImRaii.TabItemDisposable tab = ImRaii.TabItem(_L("About") + "###QuestionableConfigTabs");
        if (!tab)
            return;
        DrawAboutTab();
    }

    private void DrawAboutTab()
    {
        Version pluginVersion = typeof(QuestionablePlugin).Assembly.GetName().Version!;

        ImGui.Text("Questionable");
        ImGui.SameLine();
        QstWidgets.Chip($"v{pluginVersion.ToString(4)}", QstTheme.Info);
        ImGui.TextColored(QstTheme.TextMuted, _L("CN adaptation maintained by QianChang"));

        if (QstWidgets.SectionHeader(_L("Description"), "AboutDescription"))
            ImGui.TextWrapped(_pluginInterface.Manifest.Description ?? string.Empty);

        if (QstWidgets.SectionHeader(_L("Information"), "AboutInfo"))
        {
            DrawAboutRow(_L("Author"), "liza, qstxiv, & various contributors & QianChang");
            DrawAboutRow(_L("Upstream"), "PunishXIV/Questionable (alydev & contributors)");
            DrawAboutLinkRow(_L("Source repository"), "QianChangUwU/Questionable",
                "https://github.com/QianChangUwU/Questionable");
            DrawAboutLinkRow(_L("Upstream repository"), "PunishXIV/Questionable",
                "https://github.com/PunishXIV/Questionable");
        }

        if (QstWidgets.SectionHeader(_L("Support & community"), "AboutSupport"))
        {
            if (ImGuiComponentsLocal.IconButtonWithText(FontAwesomeIcon.Heart, _L("Sponsor upstream")))
                Util.OpenLink("https://ko-fi.com/alydev");
            ImGui.SameLine();
            if (ImGuiComponentsLocal.IconButtonWithText(FontAwesomeIcon.Heart, _L("Sponsor QianChang (afdian)")))
                Util.OpenLink("https://ifdian.net/a/QianChang");
            ImGui.SameLine();
            if (ImGuiComponentsLocal.IconButtonWithText(FontAwesomeIcon.Comments, _L("QianChang's Discord")))
                Util.OpenLink("https://discord.gg/K36BTSGGxN");

            ImGui.TextColored(QstTheme.TextMuted,
                _L("Join QianChang's Discord for CN support and updates."));
        }

        if (QstWidgets.SectionHeader(_L("Required plugins:"), "AboutRequired"))
            ImGui.TextWrapped("vnavmesh · TextAdvance · Lifestream");
    }

    private static void DrawAboutRow(string label, string value)
    {
        float valueOffset = Math.Max(110f, ImGui.CalcTextSize(label).X + 16f);
        ImGui.Text(label);
        ImGui.SameLine(valueOffset);
        ImGui.TextWrapped(value);
    }

    private static void DrawAboutLinkRow(string label, string display, string url)
    {
        float valueOffset = Math.Max(110f, ImGui.CalcTextSize(label).X + 16f);
        ImGui.Text(label);
        ImGui.SameLine(valueOffset);
        ImGui.TextColored(QstTheme.Accent, display);
        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            ImGui.SetTooltip(url);
        }

        if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
            Util.OpenLink(url);
    }
}
