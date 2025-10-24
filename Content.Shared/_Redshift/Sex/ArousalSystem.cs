using Content.Shared._Redshift.Sex.Components;
using Content.Shared.Nutrition.Components;

namespace Content.Shared._Redshift.Sex;

public sealed class ArousalSystem : EntitySystem // TODO: put like 99% of this shit in server when prediction inevitably breaks
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();


    }

    // i :heart: stealing multi-year-old wizden code
    public void ModifyArousal(Entity<ArousalComponent> ent, float amount)
    {
        SetArousal(ent, ent.Comp.CurrentArousal + amount);
    }

    public void SetArousal(Entity<ArousalComponent> ent, float amount)
    {
        ent.Comp.CurrentArousal = Math.Clamp(amount, 0, ent.Comp.MaxArousal);

        DirtyField(ent, ent.Comp, nameof(ArousalComponent.CurrentArousal));
    }
}
