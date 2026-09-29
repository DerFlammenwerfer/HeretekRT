using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;

namespace Content.Server._WH40K.Act1.Commands;

/// <summary>
/// Receives the lobby request for an authored Act I start without passing through a normal job-slot selection.
/// </summary>
[AnyCommand]
public sealed partial class Wh40kAct1JoinCommand : IConsoleCommand
{
    [Dependency] private IEntityManager _entities = default!;

    public string Command => "wh40kact1join";
    public string Description => "Запускает доступный для аккаунта Акт I из лобби.";
    public string Help => "wh40kact1join";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 0)
        {
            shell.WriteError(Help);
            return;
        }

        if (shell.Player == null)
            return;

        if (_entities.System<Act1PrologueSystem>().TryJoinFromLobby(shell.Player, out var message))
            shell.WriteLine(message);
        else
            shell.WriteError(message);
    }
}
