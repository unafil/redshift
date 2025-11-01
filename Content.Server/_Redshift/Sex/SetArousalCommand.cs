using Content.Server.Administration.Managers;
using Content.Shared._Redshift.Sex.Components;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server._Redshift.Sex;

[AnyCommand]
public sealed class SetArousalCommand : IConsoleCommand
{
    [Dependency] private readonly IEntityManager _entities = default!;
    [Dependency] private readonly IEntitySystemManager _entity = default!;
    [Dependency] private readonly IAdminManager _adminManager = default!;

    public string Command => "setarousal";
    public string Description => Loc.GetString("set-arousal-command-description");
    public string Help => Loc.GetString("set-arousal-command-help-text");

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player == null || !_entities.TryGetComponent<ArousalComponent>(shell.Player.AttachedEntity, out var comp))
            return;

        if (!int.TryParse(args[0], out var val))
        {
            shell.WriteLine(Loc.GetString("set-arousal-command-failure-invalid"));
            return;
        }

        if (val < comp.CurrentArousal && _adminManager.GetAdminData(shell.Player, includeDeAdmin: true) == null) // admins can set it lower (we trust them)
        {
            shell.WriteLine(Loc.GetString("set-arousal-command-failure-cannot-lower"));
            return;
        }

        var arousalSys = _entity.GetEntitySystem<ArousalSystem>(); // can't use normal dependencies in interfaces

        arousalSys.SetArousal((shell.Player.AttachedEntity.Value, comp), val);
    }
}
