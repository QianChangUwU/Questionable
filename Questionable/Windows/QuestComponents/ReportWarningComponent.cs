using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Questionable.Utils;
using static Questionable.Utils.LocalizeShortcut;
namespace Questionable.Windows.QuestComponents;

internal sealed class ReportWarningComponent(Configuration configuration)
{
    private readonly Configuration _configuration = configuration;

    public void Draw() => DrawReportWarning();

    private void DrawReportWarning()
    {
        ImGui.TextColored(ImGuiColors.DPSRed, _L("Future message"));
        ImGui.TextWrapped(_L("As of version xxxx, QST includes a feature where you can click the " +
                          "! button next to the quest progress buttons to report an issue with the current quest. " +
                          "This message is to notify you that if you choose to make use of this new feature and submit a " +
                          "bug report, QST will automatically capture and upload the following information:"));
        ImGui.BulletText(_L("List of all enabled plugins and their version numbers"));
        ImGui.BulletText(_L("The last ten actions taken by QST"));
        ImGui.BulletText(_L("The quest/sequence/step you are on when clicking the button"));
        ImGui.BulletText(_L("Your list of priority quests"));
        ImGui.BulletText(_L("A short configurable message from Settings, if set"));
        ImGui.TextWrapped(_L("This feature will never send any information to the bug report service unless you click " +
                          "the ! button highlighted in red below. If you would like to opt out of seeing this button, click the " +
                          "orange \"Opt Out\" button below. Otherwise, click the green \"Dismiss\" button to hide this warning."));
        if (ImGuiComponentsLocal.IconButtonWithText(FontAwesomeIcon.ExclamationTriangle, _L("Opt Out"), ImGuiColors.DalamudOrange))
        {
            _configuration.General.DismissedReportWarning = true;
            _configuration.General.ReportsDisabled = true;
        }

        ImGui.SameLine();
        if (ImGuiComponentsLocal.IconButtonWithText(FontAwesomeIcon.ExclamationTriangle, _L("Dismiss"), ImGuiColors.ParsedGreen))
            _configuration.General.DismissedReportWarning = true;
    }
}
