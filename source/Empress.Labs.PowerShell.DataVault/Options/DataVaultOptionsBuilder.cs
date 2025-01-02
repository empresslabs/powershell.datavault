// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using System.Diagnostics.CodeAnalysis;
using Empress.Labs.PowerShell.DataVault.Options.Abstractions;
using SQLite;

namespace Empress.Labs.PowerShell.DataVault.Options;

[ExcludeFromCodeCoverage]
internal sealed class DataVaultOptionsBuilder : IDataVaultOptions {
  private DataVaultOptions.ConnectionType _connectionType;
  private string _fileName = string.Empty;
  private SQLiteOpenFlags _flags = SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache;

  /// <inheritdoc />
  public IDataVaultOptionsStepTwo UseFileName(string fileName) {
    _fileName = fileName;

    return this;
  }

  /// <inheritdoc />
  IDataVaultOptionsStepTwoAlternativeOne IDataVaultOptionsStepTwo.UseFlags(SQLiteOpenFlags flags) {
    _flags = flags;

    return this;
  }

  /// <inheritdoc />
  IDataVaultOptionsStepTwoAlternativeTwo IDataVaultOptionsStepTwo.UseConnectionType(DataVaultOptions.ConnectionType connectionType) {
    _connectionType = connectionType;

    return this;
  }

  /// <inheritdoc />
  void IDataVaultOptionsStepTwoAlternativeOne.UseConnectionType(DataVaultOptions.ConnectionType connectionType)
    => _connectionType = connectionType;

  /// <inheritdoc />
  void IDataVaultOptionsStepTwoAlternativeTwo.UseFlags(SQLiteOpenFlags flags)
    => _flags = flags;

  /// <inheritdoc />
  public DataVaultOptions Apply()
    => new() {
      FileName = _fileName,
      SQLiteOpenFlags = _flags,
      SQLiteConnectionType = _connectionType
    };
}
