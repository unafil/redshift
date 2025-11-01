using Content.Shared._Redshift.Sex;
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
        if (!args.EntityManager.TryGetComponent(args.TargetEntity, out ArousalComponent? arousal))
            return;

        if (MaxThreshold != null && arousal.CurrentArousal + Amount >= MaxThreshold)
            return;

        args.EntityManager.System<SharedArousalSystem>().ModifyArousal((args.TargetEntity, arousal), Amount);
    }

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
        => Loc.GetString("reagent-effect-guidebook-modify-arousal", ("chance", Probability), ("amount",  Amount), ("max", MaxThreshold == null? -1: MaxThreshold.Value));
}
