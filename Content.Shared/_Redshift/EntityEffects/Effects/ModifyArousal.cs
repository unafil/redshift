using Content.Shared._Redshift.Sex.Components;
using Content.Shared.EntityEffects;
using Robust.Shared.Prototypes;

namespace Content.Shared._Redshift.EntityEffects.Effects;

public sealed partial class ModifyArousal : EntityEffect
{
    [DataField] public float Amount = 10;

    [DataField] public float? MaxThreshold = null;

    public override void Effect(EntityEffectBaseArgs args)
    {
        if (args.EntityManager.TryGetComponent(args.TargetEntity, out ArousalComponent? arousal))
        {
            if (arousal.CurrentArousal + Amount >= MaxThreshold)
                return;

            //args.EntityManager.System<ArousalSystem>().ModifyArousal((args.TargetEntity, arousal), Amount);
            //ArousalSystem is server, can't access it the cool way from shared
            //but this needs to be in shared for the guidebook text
            arousal.CurrentArousal = Math.Clamp(arousal.CurrentArousal + Amount, 0, arousal.MaxArousal);
        }
    }

    // TODO: guidebook text, see SatiateThirst.cs
    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
        => Loc.GetString("reagent-effect-guidebook-modify-arousal", ("chance", Probability), ("amount",  Amount), ("max", MaxThreshold == null? -1: MaxThreshold.Value));
}
