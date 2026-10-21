using System;
using System.Collections.Generic;
using BankApp.Transaction_Layer.Infrastructure;

namespace BankApp.Transaction_Layer.Service_Locators;

public sealed class BankTransactionServiceLocator
{

    private readonly static BankTransactionServiceLocator _instance = new();
    public static BankTransactionServiceLocator Instance => _instance;
    private readonly Dictionary<Type, object> _services = [];

    /// <summary>
    /// Private constructor since we're using singleton pattern.
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    private BankTransactionServiceLocator() { LocatorBootstrap.RegisterAll(this);}

    /// <summary>
    /// Responsible for registering a service instance with the locator.
    /// Stores the given service keyed by its concrete type.
    /// </summary>
    /// <param name="">
    /// object o the service instance to register with the locator.
    /// </param>
    public void RegisterService(object o) => _services.Add(o.GetType(), o);

    /// <summary>
    /// Responsible for retrieving a registered service instance from the locator.
    /// Attempts to look up the service by its type.
    /// </summary>
    /// <param name="">
    /// Type t the concrete type of the service being requested.
    /// object? o output value containing the found service, or null if no service was found.
    /// </param>
    /// <returns>
    /// bool representing whether the service was found
    /// </returns>
    public bool TryGetService(Type t, out object? o) => _services.TryGetValue(t, out o);

}

