using Content.Shared._Redshift.Sex.Components;
using Content.Shared.Alert;

namespace Content.Shared._Redshift.Sex;

/// <summary>
/// This handles...
/// </summary>
public abstract class SharedArousalSystem : EntitySystem
{
    [Dependency] private readonly AlertsSystem _alerts = default!;

    public void ModifyArousal(Entity<ArousalComponent> ent, float amount)
    {
        SetArousal(ent, ent.Comp.CurrentArousal + amount);
    }

    public void SetArousal(Entity<ArousalComponent> ent, float amount)
    {
        ent.Comp.CurrentArousal = Math.Clamp(amount, 0, ent.Comp.MaxArousal);

        DirtyField(ent, ent.Comp, nameof(ArousalComponent.CurrentArousal));


        if (ent.Comp.CurrentArousal > 20)
        {
            var level = (short)(Math.Round(ent.Comp.CurrentArousal) / 20);
            _alerts.ShowAlert(ent, ent.Comp.ArousalAlert, level);
        }
        else
        {
            _alerts.ClearAlert(ent, ent.Comp.ArousalAlert);
        }
    }
}
