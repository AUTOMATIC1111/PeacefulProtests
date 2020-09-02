using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeacefulProtests.Patch
{
    // fix to disregard xml and make peaceful protesters permanently allied to player
    [HarmonyPatch(typeof(Faction), "RelationKindWith")]
    class PatchFactionRelationKindWith
    {
        static bool Prefix(Faction other, Faction __instance, ref FactionRelationKind __result)
        {
            if (__instance.def != PeacefulFactionDefOf.PeacefulProtesters) return true;
            if (other != Faction.OfPlayer) return true;

            __result = FactionRelationKind.Ally;
            return false;
        }
    }
}
