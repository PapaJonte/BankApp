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
    /// <param name="serviceLocator">ServiceLocator used to register the transaction services.</param>
    public static void RegisterAll(BankTransactionServiceLocator serviceLocator)
    {
        #region  ExchangeRate Module
        ExchangeRate exchangeRateModule = new();
        #endregion
        #region Transaction Module
        TransactionScheduler transactionScheduler = new();
        TransactionRegistry transactionRegistry = new();
        StageTransaction transactionStaging = new(transactionScheduler, transactionRegistry);
        BankTransaction bankTransactionModule = new(transactionStaging);
        #endregion
        #region Transfer Module
        BankTransfer bankTransferModule = new(bankTransactionModule, exchangeRateModule);
        #endregion

        serviceLocator.RegisterService(exchangeRateModule);
        serviceLocator.RegisterService(bankTransactionModule);
        serviceLocator.RegisterService(bankTransferModule);
    }
}

