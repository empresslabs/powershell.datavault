// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using System.Linq.Expressions;

namespace Empress.Labs.PowerShell.DataVault.Abstractions;

/// <summary>
///   A read-only repository that provides basic operations to entities.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface IReadOnlyAsyncRepository<TEntity> : IAsyncDisposable where TEntity : Entity, new() {
  /// <summary>
  ///   Counts all entities in the repository.
  /// </summary>
  /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
  /// <returns>The number of entities.</returns>
  Task<int> CountAsync(CancellationToken cancellationToken = default);

  /// <summary>
  ///   Counts all entities in the repository by a predicate.
  /// </summary>
  /// <param name="predicate">The predicate to use.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
  /// <returns>The number of entities.</returns>
  /// <exception cref="ArgumentNullException">If the <paramref name="predicate" /> is <c>null</c>.</exception>
  Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

  /// <summary>
  ///   Checks if an entity exists in the repository by its identifier.
  /// </summary>
  /// <param name="id">The identifier of the entity to find.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
  /// <returns><c>true</c> if the entity exists, <c>false</c> otherwise.</returns>
  /// <exception cref="ArgumentNullException">If the <paramref name="id" /> is <c>null</c>.</exception>
  Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

  /// <summary>
  ///   Checks if an entity exists in the repository by a predicate.
  /// </summary>
  /// <param name="predicate">The predicate to use.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
  /// <returns><c>true</c> if the entity exists, <c>false</c> otherwise.</returns>
  /// <exception cref="ArgumentNullException">If the <paramref name="predicate" /> is <c>null</c>.</exception>
  Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

  /// <summary>
  ///   Gets an entity in the repository by its identifier.
  /// </summary>
  /// <param name="id">The identifier of the entity to find.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
  /// <returns>The entity if found, null otherwise.</returns>
  /// <exception cref="ArgumentNullException">If the <paramref name="id" /> is <c>null</c>.</exception>
  Task<TEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default);

  /// <summary>
  ///   Gets an entity in the repository by a predicate.
  /// </summary>
  /// <param name="predicate">The predicate to use.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
  /// <returns>The entity if found, null otherwise.</returns>
  /// <exception cref="ArgumentNullException">If the <paramref name="predicate" /> is <c>null</c>.</exception>
  Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

  /// <summary>
  ///   Finds all entities in the repository.
  /// </summary>
  /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
  /// <returns>The found entities or an empty collection.</returns>
  Task<IReadOnlyCollection<TEntity>> FindAsync(CancellationToken cancellationToken = default);

  /// <summary>
  ///   Finds all entities in the repository by a predicate.
  /// </summary>
  /// <param name="predicate">The predicate to use.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
  /// <returns>The entities found by the predicate or an empty collection.</returns>
  /// <exception cref="ArgumentNullException">If the <paramref name="predicate" /> is <c>null</c>.</exception>
  Task<IReadOnlyCollection<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
}
