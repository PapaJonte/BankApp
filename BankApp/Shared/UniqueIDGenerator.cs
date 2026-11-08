using System;

namespace BankApp.Shared;

public static class UniqueIDGenerator
{
    private static int UniqueID = 0;

    public static int GenerateUniqueID() => ++UniqueID;
}
