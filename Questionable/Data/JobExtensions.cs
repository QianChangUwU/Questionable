using System;
using System.Linq;
using ECommons.ExcelServices;
using static Questionable.Utils.LocalizeShortcut;

namespace Questionable.Data;

internal static class JobExtensions
{
    public static bool IsClass(this Job classJob)
    {
        return classJob is >= Job.GLA and <= Job.THM
                   or Job.ACN
                   or Job.ROG
               || classJob.IsCrafter()
               || classJob.IsGatherer();
    }

    public static bool HasBaseClass(this Job classJob)
    {
        return Enum.GetValues<Job>()
            .Any(x => x.IsClass() && x.AsJob() == classJob);
    }

    public static Job AsJob(this Job classJob)
    {
        return classJob switch
        {
            Job.GLA => Job.PLD,
            Job.MRD => Job.WAR,
            Job.PGL => Job.MNK,
            Job.LNC => Job.DRG,
            Job.ROG => Job.NIN,
            Job.ARC => Job.BRD,
            Job.CNJ => Job.WHM,
            Job.THM => Job.BLM,
            Job.ACN => Job.SMN,
            var _ => classJob
        };
    }

    public static bool IsMelee(this Job classJob)
    {
        return classJob is Job.PGL
            or Job.MNK
            or Job.LNC
            or Job.DRG
            or Job.ROG
            or Job.NIN
            or Job.SAM
            or Job.RPR
            or Job.VPR;
    }

    public static bool IsPhysicalRanged(this Job classJob)
    {
        return classJob is Job.ARC
            or Job.BRD
            or Job.MCH
            or Job.DNC;
    }

    public static bool IsCaster(this Job classJob)
    {
        return classJob is Job.THM
            or Job.BLM
            or Job.ACN
            or Job.SMN
            or Job.RDM
            or Job.BLU
            or Job.PCT;
    }

    public static bool DealsPhysicalDamage(this Job classJob) => classJob.IsTank() || classJob.IsMelee() || classJob.IsPhysicalRanged();

    public static bool DealsMagicDamage(this Job classJob) => classJob.IsHealer() || classJob.IsCaster();

    public static bool IsCrafter(this Job classJob) => classJob.IsDoh();

    public static bool IsGatherer(this Job classJob) => classJob.IsDol();

    public static string ToFriendlyString(this Job classJob)
    {
        return classJob switch
        {
            Job.GLA => _L("Gladiator"),
            Job.PGL => _L("Pugilist"),
            Job.MRD => _L("Marauder"),
            Job.LNC => _L("Lancer"),
            Job.ARC => _L("Archer"),
            Job.CNJ => _L("Conjurer"),
            Job.THM => _L("Thaumaturge"),
            Job.CRP => _L("Carpenter"),
            Job.BSM => _L("Blacksmith"),
            Job.ARM => _L("Armorer"),
            Job.GSM => _L("Goldsmith"),
            Job.LTW => _L("Leatherworker"),
            Job.WVR => _L("Weaver"),
            Job.ALC => _L("Alchemist"),
            Job.CUL => _L("Culinarian"),
            Job.MIN => _L("Miner"),
            Job.BTN => _L("Botanist"),
            Job.FSH => _L("Fisher"),
            Job.PLD => _L("Paladin"),
            Job.MNK => _L("Monk"),
            Job.WAR => _L("Warrior"),
            Job.DRG => _L("Dragoon"),
            Job.BRD => _L("Bard"),
            Job.WHM => _L("White Mage"),
            Job.BLM => _L("Black Mage"),
            Job.ACN => _L("Arcanist"),
            Job.SMN => _L("Summoner"),
            Job.SCH => _L("Scholar"),
            Job.ROG => _L("Rogue"),
            Job.NIN => _L("Ninja"),
            Job.MCH => _L("Machinist"),
            Job.DRK => _L("Dark Knight"),
            Job.AST => _L("Astrologian"),
            Job.SAM => _L("Samurai"),
            Job.RDM => _L("Red Mage"),
            Job.BLU => _L("Blue Mage"),
            Job.GNB => _L("Gunbreaker"),
            Job.DNC => _L("Dancer"),
            Job.RPR => _L("Reaper"),
            Job.SGE => _L("Sage"),
            Job.VPR => _L("Viper"),
            Job.PCT => _L("Pictomancer"),
            var _ => classJob.ToString()
        };
    }
}
