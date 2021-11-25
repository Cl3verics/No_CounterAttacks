using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace NoCounterAttacks
{
    [StaticConstructorOnStartup]
    public static class Core
    {
        static Core()
        {
            new Harmony("NoCounterAttacks.Mod").PatchAll();
        }
    }

    [HarmonyPatch(typeof(JobGiver_AIGotoNearestHostile), "TryGiveJob")]
    public class JobGiver_AIGotoNearestHostile_TryGiveJob_Patch
    {
        private static void Postfix(ref Job __result, Pawn pawn)
        {
            if (pawn.HostileTo(Faction.OfPlayer) && (pawn.GetLord()?.LordJob is LordJob_DefendBase || pawn.Map.ParentFaction == pawn.Faction && pawn.GetLord()?.LordJob is LordJob_AssaultColony))
            {
                if (__result != null)
                {
                    var distance = NearestDistanceFromBase(__result.targetA.Cell, pawn);
                    if (distance > 10)
                    {
                        __result = null;
                    }
                }
            }
        }
        public static float NearestDistanceFromBase(IntVec3 intVec3, Pawn pawn)
        {
            var nearestBuilding = pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.BuildingArtificial).Where(x => x.Faction == pawn.Faction).MinBy(x => x.Position.DistanceTo(intVec3));
            return nearestBuilding.Position.DistanceTo(intVec3);
        }
    }

    [HarmonyPatch(typeof(JobGiver_AISapper), "TryGiveJob")]
    public class JobGiver_AISapper_TryGiveJob_Patch
    {
        private static void Postfix(ref Job __result, Pawn pawn)
        {
            if (pawn.HostileTo(Faction.OfPlayer) && (pawn.GetLord()?.LordJob is LordJob_DefendBase || pawn.Map.ParentFaction == pawn.Faction && pawn.GetLord()?.LordJob is LordJob_AssaultColony))
            {
                if (__result != null)
                {
                    var distance = JobGiver_AIGotoNearestHostile_TryGiveJob_Patch.NearestDistanceFromBase(__result.targetA.Cell, pawn);
                    if (distance > 10)
                    {
                        __result = null;
                    }
                }
            }
        }
    }
}
