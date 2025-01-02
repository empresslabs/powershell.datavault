using SQLite;

namespace Empress.Labs.PowerShell.DataVault.Options.Abstractions;

public interface IDataVaultOptionsStepTwo {
  /// <summary>
  ///   Uses the provided flags for the data vault.
  /// </summary>
  /// <param name="flags">The flags to use.</param>
  /// <returns>The step three builder.</returns>
  IDataVaultOptionsStepTwoAlternativeOne UseFlags(SQLiteOpenFlags flags);

  /// <summary>
  ///   Uses the provided connection type for the data vault.
  /// </summary>
  /// <param name="connectionType">The connection type to use.</param>
  /// <returns>The step three builder.</returns>
  IDataVaultOptionsStepTwoAlternativeTwo UseConnectionType(DataVaultOptions.ConnectionType connectionType);
}
