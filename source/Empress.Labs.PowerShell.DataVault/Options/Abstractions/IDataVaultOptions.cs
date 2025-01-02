using System.Diagnostics.CodeAnalysis;

namespace Empress.Labs.PowerShell.DataVault.Options.Abstractions;

/// <summary>
///   The data vault options builder.
/// </summary>
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public interface IDataVaultOptions : IDataVaultOptionsStepOne, IDataVaultOptionsStepTwo, IDataVaultOptionsStepTwoAlternativeOne,
  IDataVaultOptionsStepTwoAlternativeTwo {
  /// <summary>
  ///   Applies the options to the data vault.
  /// </summary>
  /// <returns>The data vault options.</returns>
  DataVaultOptions Apply();
}
