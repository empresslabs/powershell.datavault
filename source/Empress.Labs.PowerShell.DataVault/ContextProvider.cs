// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Empress.Labs.PowerShell.DataVault.Abstractions;
using Empress.Labs.PowerShell.DataVault.Options;
using SQLite;

namespace Empress.Labs.PowerShell.DataVault;

[ExcludeFromCodeCoverage]
internal sealed class ContextProvider : IContextProvider {
  public ContextProvider(DataVaultOptionsRegistrar registrar) {
    ArgumentNullException.ThrowIfNull(registrar, nameof(registrar));

    var options = registrar.Options;
    var filePath = options.GetPath();

    filePath.Parent?.Directory.Create();

    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
    var entities = assemblies
      .SelectMany(assembly => assembly.GetTypes())
      .Where(type => type is { IsClass: true, IsAbstract: false } &&
                     type.GetCustomAttribute<TableAttribute>() is not null)
      .ToArray();

    switch (options.SQLiteConnectionType) {
      case DataVaultOptions.ConnectionType.Asynchronous:
        AsynchronousConnection = new SQLiteAsyncConnection(filePath, options.SQLiteOpenFlags);
        AsynchronousConnection.CreateTablesAsync(CreateFlags.AllImplicit, entities).Wait();
        break;
      case DataVaultOptions.ConnectionType.Synchronous:
        SynchronousConnection = new SQLiteConnection(filePath, options.SQLiteOpenFlags);
        SynchronousConnection.CreateTables(CreateFlags.AllImplicit, entities);
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(options.SQLiteConnectionType), options.SQLiteConnectionType, null);
    }
  }

  /// <inheritdoc />
  public SQLiteAsyncConnection? AsynchronousConnection { get; private set; }

  /// <inheritdoc />
  public SQLiteConnection? SynchronousConnection { get; private set; }

  /// <inheritdoc />
  public void Dispose() {
    SynchronousConnection?.Dispose();
    SynchronousConnection = null;
  }

  /// <inheritdoc />
  public async ValueTask DisposeAsync() {
    if (AsynchronousConnection is not null) {
      await AsynchronousConnection.CloseAsync();
      AsynchronousConnection = null;
    }

    Dispose();
  }
}
