using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace PeacefulProtests.Patch
{
    /// this changes pawn name colors to blue, to show to player that they are friendlies
    [HarmonyPatch(typeof(PawnNameColorUtility), "PawnNameColorOf")]
    class PatchPawnNameColorUtility
    {
        static Color colorPeraceful = new Color(0.4f, 0.85f, 0.9f);

        static bool Prefix(Pawn pawn, ref Color __result)
        {
            if (pawn.Faction?.def != PeacefulFactionDefOf.PeacefulProtesters) return true;
            if (pawn.IsPrisoner) return true;

            __result = colorPeraceful;
            return false;
        }
    }


}
