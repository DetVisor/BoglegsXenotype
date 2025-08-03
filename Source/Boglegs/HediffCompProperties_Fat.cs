using Verse;

namespace Boglegs
{
    public class HediffCompProperties_Fat : HediffCompProperties
    {
        // Amount of nutrition past which fat is accumulated
        public float startThreshold;
        // Conversion rate between fat and nutrition when accumulating (when using fat, conversion rate is always 1)
        // Higher number will mean that more nutrition is converted into less fat
        public float conversionRate;
        // Amount of nutrition below which fat will be spent
        public float starvationThreshold;
        // Rate at which fat is accumulated, fat units per tick
        public float conversionSpeed;
        // Rate at which fat is spent, fat units per tick
        public float starvationSpeed;
        // Maximum amount of nutrition that can be stored
        public float maxFat;

        // Curve for slowdown, 1 = 100% movespeed
        public SimpleCurve slowdownCurve;
        // Curve for blunt protection, 1 = 100% blunt protection
        public SimpleCurve bluntProtectionCurve;
        // Curve for additional meat amount
        public SimpleCurve meatCurve;

        public HediffCompProperties_Fat()
        {
            compClass = typeof(HediffComp_Fat);
        }
    }
}