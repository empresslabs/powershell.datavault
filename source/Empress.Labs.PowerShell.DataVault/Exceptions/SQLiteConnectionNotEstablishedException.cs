// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using System.Diagnostics.CodeAnalysis;
using Empress.Labs.PowerShell.DataVault.Abstractions;
using Empress.Labs.PowerShell.DataVault.Options;
using SQLite;

namespace Empress.Labs.PowerShell.DataVault.Exceptions;

/// <summary>
///   Represents an exception that is thrown when the <see cre="IContextProviderder.AsynchronousConnection" /> or
///   <see cref="IContextProvider.SynchronousConnection" /> has not been established.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class SQLiteConnectionNotEstablishedException(DataVaultOptions.ConnectionType connectionType)
  : Exception($"The {Enum.GetName(connectionType)?.ToLower()} connection has not been established.") {
  /// <summary>
  ///   Throws an <see cref="SQLiteConnectionNotEstablishedException" /> if the <see cref="IContextProvider.AsynchronousConnection" /> is null.
  /// </summary>
  /// <param name="sqLiteAsyncConnection">The connection provider.</param>
  /// <exception cref="SQLiteConnectionNotEstablishedException">The asynchronous connection has not been established.</exception>
  public static void ThrowIfNull([System.Diagnostics.CodeAnalysis.NotNull] SQLiteAsyncConnection? sqLiteAsyncConnection) {
    if (sqLiteAsyncConnection is null) {
      throw new SQLiteConnectionNotEstablishedException(DataVaultOptions.ConnectionType.Asynchronous);
    }
  }

  /// <summary>
  ///   Throws an <see cref="SQLiteConnectionNotEstablishedException" /> if the <see cref="IContextProvider.SynchronousConnection" /> is null.
  /// </summary>
  /// <param name="sqLiteConnection">The connection provider.</param>
  /// <exception cref="SQLiteConnectionNotEstablishedException">The synchronous connection has not been established.</exception>
  public static void ThrowIfNull([System.Diagnostics.CodeAnalysis.NotNull] SQLiteConnection? sqLiteConnection) {
    if (sqLiteConnection is null) {
      throw new SQLiteConnectionNotEstablishedException(DataVaultOptions.ConnectionType.Synchronous);
    }
  }
}
