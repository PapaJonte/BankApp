using System;
using BankApp.Transaction_Layer.API;
using BankApp.Transaction_Layer.Controller;
using BankApp.Transaction_Layer.Data;
using BankApp.Transaction_Layer.Service;
using BankApp.Transaction_Layer.Service_Locators;

namespace BankApp.Transaction_Layer.Infrastructure;

public static class LocatorBootstrap
{
    /// <summary>
    /// Create all neccessary transaction objects, bootstrap them and register with service locator
    /// </summary>
    /// <param name="">
    /// ServiceLocator serviceLocator used to register the transaction services.
    /// </param>
    /// <returns></returns>
    public static void RegisterAll(BankTransactionServiceLocator serviceLocator)
    {
        
        BankTransaction bankTransactionModule = new();
        #region  ExchangeRate Module
        ExchangeRateRegistry exchangeRegistry = new();
        ExchangeRateCalculator exchangeCalculator = new();
        ExchangeRate exchangeRateModule = new(exchangeRegistry, exchangeCalculator);
        #endregion
        BankTransfer bankTransferModule = new(bankTransactionModule, exchangeRateModule);
        StageTransaction transactionStaging = new();
        TransactionExecutor transactionExecutor = new();

        serviceLocator.RegisterService(bankTransferModule);
        serviceLocator.RegisterService(bankTransactionModule);
        serviceLocator.RegisterService(exchangeRateModule);
        serviceLocator.RegisterService(transactionStaging);
        serviceLocator.RegisterService(transactionExecutor);
    }
}
