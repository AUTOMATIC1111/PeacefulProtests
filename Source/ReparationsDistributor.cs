using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PeacefulProtests
{
    internal class ReparationsDistributor
    {
        public ReparationsDistributor(IncidentParms parms, List<Pawn> allPawns, List<Thing> loot)
        {
            this.parms = parms;
            this.allPawns = allPawns;
            this.loot = loot;
            this.unusedPawns = new List<Pawn>(allPawns);
        }

        public void DistributeLoot()
        {
            recipient = allPawns.MaxBy((Pawn p) => p.kindDef.combatPower);
            recipientMassGiven = 0f;
            foreach (IGrouping<ThingDef, Thing> grouping in from t in loot group t by t.def)
            {
                foreach (Thing item in grouping)
                {
                    DistributeItem(item);
                }
                NextRecipient();
            }
        }

        private void DistributeItem(Thing item)
        {
            int num = item.stackCount;
            int num2 = 0;
            while (num > 0 && num2++ < 5)
            {
                num -= TryGiveToRecipient(item, num, false);
                if (num > 0)
                {
                    NextRecipient();
                }
            }
            if (num > 0)
            {
                NextRecipient();
                TryGiveToRecipient(item, num, true);
            }
        }

        private int TryGiveToRecipient(Thing item, int count, bool force = false)
        {
            float num = 10f * Mathf.Max(1f, recipient.BodySize) - recipientMassGiven;
            float statValue = item.GetStatValue(StatDefOf.Mass, true);
            int num2 = force ? count : Mathf.RoundToInt(Mathf.Clamp(num / statValue, 0f, (float)count));
            if (num2 > 0)
            {
                int num3 = recipient.inventory.innerContainer.TryAdd(item, num2, true);
                recipientMassGiven += (float)num3 * statValue;
                return num3;
            }
            return 0;
        }

        private void NextRecipient()
        {
            recipientMassGiven = 0f;
            if (unusedPawns.Any<Pawn>())
            {
                recipient = unusedPawns.Pop<Pawn>();
                return;
            }
            recipient = allPawns.RandomElement<Pawn>();
        }

        private readonly IncidentParms parms;

        private readonly List<Pawn> allPawns;

        private readonly List<Thing> loot;

        private readonly List<Pawn> unusedPawns;

        private Pawn recipient;

        private float recipientMassGiven;
    }
}
