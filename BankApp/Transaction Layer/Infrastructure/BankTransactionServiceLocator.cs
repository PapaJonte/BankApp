using System;
using System.Collections.Generic;

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
    private BankTransactionServiceLocator() { }

    public void RegisterService(object o) => _services.Add(o.GetType(), o);
    public bool TryGetService(Type t, out object? o) => _services.TryGetValue(t, out o);

}
