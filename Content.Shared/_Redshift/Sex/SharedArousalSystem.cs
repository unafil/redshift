using Content.Shared._Redshift.Sex.Components;
using Content.Shared.Alert;

namespace Content.Shared._Redshift.Sex;

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

        //this is in shared but it SHOULD only ever be called from the server
        //so who cares about dirtying, i'm not networking the components for this
        //DirtyField(ent, ent.Comp, nameof(ArousalComponent.CurrentArousal));

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
