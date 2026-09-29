using Content.Shared._NF.Bank.Components;
using Content.Shared._WH40K.Merchant;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Client.UserInterface;

namespace Content.Client._WH40K.Merchant;

public sealed class MerchantBoundUserInterface : BoundUserInterface
{
    [Dependency] private IPlayerManager _playerManager = default!;

    private MerchantWindow? _window;
    private Action<string>? _buyHandler;
    private Action? _refreshHandler;

    public MerchantBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindowCenteredLeft<MerchantWindow>();
        _buyHandler = product => SendMessage(new MerchantBuyMessage(product));
        _refreshHandler = () => SendMessage(new MerchantRequestRefreshMessage());
        _window.OnBuy += _buyHandler;
        _window.OnRequestRefresh += _refreshHandler;
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is MerchantUpdateState merchantState)
            _window?.UpdateState(merchantState, GetBalance());
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing || _window == null)
        {
            base.Dispose(disposing);
            return;
        }

        if (_buyHandler != null)
            _window.OnBuy -= _buyHandler;
        if (_refreshHandler != null)
            _window.OnRequestRefresh -= _refreshHandler;

        // CreateWindowCenteredLeft registers the window for disposal in the
        // base BUI, so do not dispose it a second time here.
        base.Dispose(disposing);
    }

    private int GetBalance()
    {
        if (_playerManager.LocalEntity is { } local
            && EntMan.TryGetComponent<BankAccountComponent>(local, out var localBank))
        {
            return localBank.Balance;
        }

        var uiSystem = EntMan.System<UserInterfaceSystem>();
        foreach (var actor in uiSystem.GetActors(Owner, UiKey))
        {
            if (EntMan.TryGetComponent<BankAccountComponent>(actor, out var bank))
                return bank.Balance;
        }

        return 0;
    }
}
