using Content.Shared._Redshift.Sex.Components;
using Content.Shared.Jittering;
using Content.Shared.Nutrition.Components;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Server._Redshift.Sex;

public sealed class ArousalSystem : EntitySystem
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedJitteringSystem _jitter = default!;
    [Dependency] private readonly GenitalSystem _genital = default!;
    [Dependency] private readonly SharedStunSystem _stun = default!;

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        var arousalQuery = EntityQueryEnumerator<ArousalComponent>();
        var now = _timing.CurTime;

        while (arousalQuery.MoveNext(out var uid, out var arousal))
        {
            if (arousal.ClimaxTime == null && arousal.CurrentArousal > arousal.MaxArousal)
            {
                arousal.ClimaxTime = _timing.CurTime + arousal.ClimaxDelay;
                _popup.PopupEntity(Loc.GetString("arousal-popup-climax-start-internal"), uid, Filter.Entities(uid), true, PopupType.Medium);
                _popup.PopupEntity(Loc.GetString("arousal-popup-climax-start-external", ("target", uid)), uid, Filter.PvsExcept(uid), true, PopupType.Small);
                _jitter.DoJitter(uid, arousal.ClimaxDelay, true, 0.5f, 6f);
            }
            if (arousal.ClimaxTime != null && arousal.ClimaxTime <= now)
            {
                // grab the relevant genital
                if (arousal.Genital == null)
                {
                    if(!TryComp<GenitalComponent>(uid, out var gen))
                        continue;
                    arousal.Genital = gen;
                }

                arousal.WaveTime = now;
                arousal.ClimaxTime = null;
            }

            if (arousal.WaveTime != null && arousal.WaveTime <= now)
            {
                if (arousal.Genital == null)
                    continue; // this is handled prior but compiler doesnt know that for sure

                var empty = !_genital.Emit((uid,arousal.Genital));

                // apply stun if moving.
                // this is likely a terrible way to detect if we're moving.
                if (TryComp(uid, out PhysicsComponent? physics) && physics.LinearVelocity.Length() > 0.5f)
                {
                    _stun.TryParalyze(uid, TimeSpan.FromSeconds(1.25f), true);
                }

                _jitter.DoJitter(uid, TimeSpan.FromSeconds(0.5f), true, 4f, 2f); // arbitrary values my beloved

                arousal.CurrentArousal /= 2;

                if (!empty || arousal.CurrentArousal > 20) // 20 being the threshold where an icon (ideally) appears
                {
                    arousal.WaveTime = now + arousal.Genital.WaveDelay;
                }
                else
                {
                    arousal.WaveTime = null; // we're done
                }
            }
        }
    }
}
