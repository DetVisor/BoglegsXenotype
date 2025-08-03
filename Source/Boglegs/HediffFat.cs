using RimWorld;
using UnityEngine;
using Verse;

namespace Boglegs
{
    public class HediffFat : HediffWithComps
    {
        private HediffComp_Fat cachedComp;
        private HediffComp_Fat Comp => cachedComp ?? (cachedComp = this.TryGetComp<HediffComp_Fat>());

        private float cachedFat = -1f;
        private HediffStage cachedStage;

        public override HediffStage CurStage
        {
            get
            {
                if (Mathf.Approximately(cachedFat, Comp.storedFat))
                {
                    return cachedStage;
                }

                cachedFat = Comp.storedFat;
                cachedStage = new HediffStage
                {
                    capMods = [new PawnCapacityModifier { capacity = PawnCapacityDefOf.Moving, offset = Comp.Props.slowdownCurve.Evaluate(cachedFat) }],
                    statOffsets =
                    [
                        new StatModifier { stat = StatDefOf.ArmorRating_Blunt, value = Comp.Props.bluntProtectionCurve.Evaluate(cachedFat) },
                        new StatModifier { stat = StatDefOf.MeatAmount, value = Comp.Props.meatCurve.Evaluate(cachedFat) }
                    ]
                };

                return cachedStage;
            }
        }

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            base.Notify_PawnDied(dinfo, culprit);
            Comp.cachedNeed = null;
        }
    }
}
