namespace Empress.Labs.PowerShell.DataVault.Options.Abstractions;

public interface IDataVaultOptionsStepTwoAlternativeOne {
  /// <summary>
  ///   Uses the provided connection type for the data vault.
  /// </summary>
  /// <param name="connectionType">The connection type to use.</param>
  void UseConnectionType(DataVaultOptions.ConnectionType connectionType);
}
