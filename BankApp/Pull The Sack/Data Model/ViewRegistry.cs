using BankApp.Pull_The_Sack.Enum;
using BankApp.Pull_The_Sack.Interface;

namespace BankApp.Pull_The_Sack.Data_Model;

/// <summary>
/// Holds the set of views the controller can serve, keyed by <see cref="ViewType"/>.
/// Encapsulates the underlying dictionary and guards against missing keys.
/// </summary>
public class ViewRegistry
{
    private readonly Dictionary<ViewType, IView> _views = [];

    /// <summary>Registers (or replaces) the view for the given type.</summary>
    public void AddView(ViewType viewType, IView view) => _views[viewType] = view;

    /// <summary>Removes the view registered under the given type, if any.</summary>
    public bool RemoveView(ViewType viewType) => _views.Remove(viewType);

    /// <summary>True if a view is registered for the given type.</summary>
    public bool HasView(ViewType viewType) => _views.ContainsKey(viewType);

    /// <summary>All registered views, safe to enumerate.</summary>
    public IEnumerable<IView> AllViews => _views.Values;

    /// <summary>Looks up a view, throwing a clear error if the type isn't registered.</summary>
    public IView GetView(ViewType viewType) =>
        _views.TryGetValue(viewType, out var view)
            ? view
            : throw new KeyNotFoundException(
                $"No view registered for {viewType}. Registered: [{string.Join(", ", _views.Keys)}]");

    /// <summary>Looks up a view without throwing.</summary>
    public bool TryGetView(ViewType viewType, out IView view) =>
        _views.TryGetValue(viewType, out view!);
}