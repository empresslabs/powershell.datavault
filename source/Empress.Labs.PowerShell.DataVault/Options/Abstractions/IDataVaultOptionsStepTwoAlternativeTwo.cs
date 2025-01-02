using SQLite;

namespace Empress.Labs.PowerShell.DataVault.Options.Abstractions;

public interface IDataVaultOptionsStepTwoAlternativeTwo {
  /// <summary>
  ///   Uses the provided flags for the data vault.
  /// </summary>
  /// <param name="flags">The flags to use.</param>
  void UseFlags(SQLiteOpenFlags flags);
}
