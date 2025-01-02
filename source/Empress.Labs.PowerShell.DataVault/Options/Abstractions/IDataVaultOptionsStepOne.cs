namespace Empress.Labs.PowerShell.DataVault.Options.Abstractions;

public interface IDataVaultOptionsStepOne {
  /// <summary>
  ///   Uses the provided file name for the data vault.
  /// </summary>
  /// <param name="fileName">The file name to use.</param>
  /// <returns>The step two builder.</returns>
  IDataVaultOptionsStepTwo UseFileName(string fileName);
}
