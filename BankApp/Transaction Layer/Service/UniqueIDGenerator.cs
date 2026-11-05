using System;

namespace BankApp.Transaction_Layer.Service;

public static class UniqueIDGenerator
{
    private static int UniqueID = 0;

    public static int GenerateUniqueID() => ++UniqueID;
}
