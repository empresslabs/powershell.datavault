// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using System.Diagnostics.CodeAnalysis;
using Empress.Labs.PowerShell.Common.Extensions;
using Empress.Labs.PowerShell.Common.IO;
using Empress.Labs.PowerShell.DataVault.Abstractions;
using Empress.Labs.PowerShell.DataVault.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Empress.Labs.PowerShell.DataVault.Extensions;

/// <summary>
///   Extensions for the <see cref="IServiceCollection" />.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions {
  /// <summary>
  ///   Adds the DataVault services to the <see cref="IServiceCollection" />.
  /// </summary>
  /// <param name="serviceCollection">The service collection.</param>
  /// <param name="setup">The setup for the DataVault options.</param>
  /// <param name="customRootDirectory">The custom root directory for the database.</param>
  /// <returns>The service collection itself.</returns>
  /// <exception cref="ArgumentOutOfRangeException">The connection type is not supported.</exception>
  public static IServiceCollection AddDataVault(this IServiceCollection serviceCollection, DataVaultOptions.Setup setup,
  AbsolutePath? customRootDirectory = null) {
    if (!string.IsNullOrEmpty(customRootDirectory)) {
      InternalState.RootDirectory = customRootDirectory;
    }

    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
    var entities = assemblies
      .SelectMany(assembly => assembly.GetTypes())
      .Where(type => type is { IsClass: true, IsAbstract: false } && type.IsSubclassOf(typeof(Entity)));

    var registrar = new DataVaultOptionsRegistrar(setup);
    var provider = new ContextProvider(registrar);

    serviceCollection
      .AddSingleton<IContextProvider>(provider);

    switch (registrar.Options.SQLiteConnectionType) {
      case DataVaultOptions.ConnectionType.Asynchronous:
        entities.ForEach(entity => {
          var repositoryType = typeof(AsyncRepository<>).MakeGenericType(entity);

          serviceCollection.AddTransient(typeof(IAsyncRepository<>).MakeGenericType(entity), repositoryType);
          serviceCollection.AddTransient(typeof(IReadOnlyAsyncRepository<>).MakeGenericType(entity), repositoryType);
        });
        break;

      case DataVaultOptions.ConnectionType.Synchronous:
        entities.ForEach(entity => {
          var repositoryType = typeof(Repository<>).MakeGenericType(entity);

          serviceCollection.AddTransient(typeof(IRepository<>).MakeGenericType(entity), repositoryType);
          serviceCollection.AddTransient(typeof(IReadOnlyRepository<>).MakeGenericType(entity), repositoryType);
        });
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(registrar.Options.SQLiteConnectionType), registrar.Options.SQLiteConnectionType,
          "The connection type is not supported.");
    }

    return serviceCollection;
  }
}
