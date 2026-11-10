namespace EWallet.Core.Exceptions;

public sealed class InsufficientBalanceException(string message) : Exception(message);