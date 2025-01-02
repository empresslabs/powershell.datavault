// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using System.Diagnostics.CodeAnalysis;
using Empress.Labs.PowerShell.Common.Extensions;
using Empress.Labs.PowerShell.Common.IO;
using Empress.Labs.PowerShell.DataVault.Abstractions;
using Empress.Labs.PowerShell.DataVault.Exceptions;
using Empress.Labs.PowerShell.DataVault.Options;
using Empress.Labs.PowerShell.DataVault.Reflection;

namespace Empress.Labs.PowerShell.DataVault;

/// <summary>
///   Methods for the DataVault module.
/// </summary>
[ExcludeFromCodeCoverage]
public static class DataVaultModule {
  /// <summary>
  ///   Sets up the DataVault module.
  /// </summary>
  /// <param name="setup">The setup for the DataVault options.</param>
  /// <param name="customRootDirectory">The custom root directory for the database.</param>
  /// <remarks>Should be called when the module is imported without <see cref="Microsoft.Extensions.DependencyInjection" />.</remarks>
  public static void OnImport(DataVaultOptions.Setup setup, AbsolutePath? customRootDirectory = null) {
    if (!string.IsNullOrEmpty(customRootDirectory)) {
      InternalState.RootDirectory = customRootDirectory;
    }

    SQLitePortableClassLibrary.TryLoad();

    var registrar = new DataVaultOptionsRegistrar(setup);
    var provider = new ContextProvider(registrar);

    InternalState.ContextProvider = provider;
    InternalState.AsynchronousRepositories.Clear();
    InternalState.SynchronousRepositories.Clear();
  }

  /// <summary>
  ///   Cleans up the DataVault module.
  /// </summary>
  /// <remarks>Should be called when the module is imported without <see cref="Microsoft.Extensions.DependencyInjection" />.</remarks>
  public static void OnCleanUp() {
    InternalState.ContextProvider?.Dispose();
    InternalState.ContextProvider = null;
    InternalState.AsynchronousRepositories.Values.ForEach(repository => ((IAsyncDisposable)repository).DisposeAsync());
    InternalState.AsynchronousRepositories.Clear();
    InternalState.SynchronousRepositories.Values.ForEach(repository => ((IDisposable)repository).Dispose());
    InternalState.SynchronousRepositories.Clear();
  }

  /// <summary>
  ///   Gets an asynchronous repository for the specified entity.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <returns>The asynchronous repository.</returns>
  /// <exception cref="ContextProviderNotInitializedException">The context provider is not initialized.</exception>
  public static IAsyncRepository<TEntity> GetAsyncRepository<TEntity>() where TEntity : Entity, new() {
    ContextProviderNotInitializedException.ThrowIfNull(InternalState.ContextProvider);

    var entityType = typeof(TEntity);

    if (!InternalState.AsynchronousRepositories.TryGetValue(entityType, out var repositoryInstance)) {
      repositoryInstance = new AsyncRepository<TEntity>(InternalState.ContextProvider);
      InternalState.AsynchronousRepositories.TryAdd(entityType, repositoryInstance);
    }

    return (IAsyncRepository<TEntity>)repositoryInstance;
  }

  /// <summary>
  ///   Gets a read-only asynchronous repository for the specified entity.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <returns>The read-only asynchronous repository.</returns>
  public static IReadOnlyAsyncRepository<TEntity> GetReadOnlyAsyncRepository<TEntity>() where TEntity : Entity, new()
    => GetAsyncRepository<TEntity>().AsReadOnly();

  /// <summary>
  ///   Gets a repository for the specified entity.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <returns>The repository.</returns>
  /// <exception cref="InvalidOperationException">The context provider is not initialized.</exception>
  public static IRepository<TEntity> GetRepository<TEntity>() where TEntity : Entity, new() {
    ContextProviderNotInitializedException.ThrowIfNull(InternalState.ContextProvider);

    if (!InternalState.SynchronousRepositories.TryGetValue(typeof(TEntity), out var repositoryInstance)) {
      repositoryInstance = new Repository<TEntity>(InternalState.ContextProvider);
      InternalState.SynchronousRepositories.TryAdd(typeof(TEntity), repositoryInstance);
    }

    return (IRepository<TEntity>)repositoryInstance;
  }

  /// <summary>
  ///   Gets a read-only repository for the specified entity.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <returns>The read-only repository.</returns>
  public static IReadOnlyRepository<TEntity> GetReadOnlyRepository<TEntity>() where TEntity : Entity, new()
    => GetRepository<TEntity>().AsReadOnly();
}
