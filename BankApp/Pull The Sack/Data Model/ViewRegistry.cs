using BankApp.Pull_The_Sack.Enum;
using BankApp.Pull_The_Sack.Interface;

namespace BankApp.Pull_The_Sack.Data_Model;

/// <summary>
/// registry holds all IViews for slotmachine game.
/// </summary>
/// <param name=""></param>
/// <returns></returns>
public class ViewRegistry
{
    private readonly Dictionary<ViewType, IView> _views = [];

    /// <summary>
    /// Add IView to registry, the params are selfexplanatory I dont want to write more comments :(
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public void AddView(ViewType viewType, IView view) => _views[viewType] = view;

    /// <summary>
    /// Remove IView from registry
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public bool RemoveView(ViewType viewType) => _views.Remove(viewType);

    /// <summary>
    /// Returns all IViews
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public IEnumerable<IView> AllViews => _views.Values;

    /// <summary>
    /// Returns IView based on ViewType enum passed in.
    /// </summary>
    /// <param name="">
    /// ViewType viewType the enum of the IView to return.
    /// </param>
    /// <returns></returns>
    public IView GetView(ViewType viewType) =>
        _views.TryGetValue(viewType, out var view)
            ? view
            : throw new KeyNotFoundException(
                $"No view registered for {viewType}. Registered: [{string.Join(", ", _views.Keys)}]");

}