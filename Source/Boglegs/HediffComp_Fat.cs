using System;
using RimWorld;
using Verse;

namespace Boglegs
{
    public class HediffComp_Fat : HediffComp
    {
        public HediffCompProperties_Fat Props => props as HediffCompProperties_Fat;

        public float storedFat;

        public override string CompTipStringExtra => "Boglegs.StoredNutrition".Translate(Math.Round(storedFat, 2));

        public Need_Food cachedNeed;
        public Need_Food need => cachedNeed ??= parent.pawn.needs.TryGetNeed<Need_Food>();

        public override void CompPostTickInterval(ref float severityAdjustment, int delta)
        {
            base.CompPostTickInterval(ref severityAdjustment, delta);
            ApplyFatChanges(delta);
        }

        public void ApplyFatChanges(int multiplier)
        {
            if (need == null) return;
            if (need.CurLevel > Props.startThreshold && storedFat < Props.maxFat)
            {
                need.CurLevel -= Math.Min(Props.conversionSpeed, Props.maxFat - storedFat) * multiplier * Props.conversionRate;
                storedFat += Math.Min(Props.conversionSpeed, Props.maxFat - storedFat) * multiplier;
            }
            else if (need.CurLevel < Props.starvationThreshold && storedFat > 0)
            {
                need.CurLevel += Math.Min(storedFat, Props.starvationSpeed) * multiplier;
                storedFat -= Math.Min(storedFat, Props.starvationSpeed) * multiplier;
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref storedFat, "storedFat");
        }
    }
}
