// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using System.Linq.Expressions;
using Empress.Labs.PowerShell.Common.Extensions;
using Empress.Labs.PowerShell.DataVault.Abstractions;
using Empress.Labs.PowerShell.DataVault.Exceptions;
using SQLite;

namespace Empress.Labs.PowerShell.DataVault;

internal sealed class AsyncRepository<TEntity> : IAsyncRepository<TEntity> where TEntity : Entity, new() {
  private readonly SQLiteAsyncConnection _connection;

  public AsyncRepository(IContextProvider contextProvider) {
    SQLiteConnectionNotEstablishedException.ThrowIfNull(contextProvider.AsynchronousConnection);

    _connection = contextProvider.AsynchronousConnection;
  }

  /// <inheritdoc />
  public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    => await _connection.Table<TEntity>().CountAsync().WithCancellationAsync(cancellationToken);

  /// <inheritdoc />
  public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) {
    ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

    return await _connection.Table<TEntity>().CountAsync(predicate).WithCancellationAsync(cancellationToken);
  }

  /// <inheritdoc />
  public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) {
    ArgumentNullException.ThrowIfNull(id, nameof(id));

    var count = await CountAsync(entity => entity.Id == id, cancellationToken);

    return count > 0;
  }

  /// <inheritdoc />
  public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) {
    ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

    var count = await CountAsync(predicate, cancellationToken);

    return count > 0;
  }

  /// <inheritdoc />
  public async Task<TEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default) {
    ArgumentNullException.ThrowIfNull(id, nameof(id));

    return await _connection.FindAsync<TEntity>(id).WithCancellationAsync(cancellationToken);
  }

  /// <inheritdoc />
  public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) {
    ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

    return await _connection.FindAsync(predicate).WithCancellationAsync(cancellationToken);
  }

  /// <inheritdoc />
  public async Task<IReadOnlyCollection<TEntity>> FindAsync(CancellationToken cancellationToken = default)
    => await _connection.Table<TEntity>().ToArrayAsync().WithCancellationAsync(cancellationToken);

  /// <inheritdoc />
  public async Task<IReadOnlyCollection<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate,
  CancellationToken cancellationToken = default)
    => await _connection.Table<TEntity>().Where(predicate).ToArrayAsync().WithCancellationAsync(cancellationToken);

  /// <inheritdoc />
  public IReadOnlyAsyncRepository<TEntity> AsReadOnly()
    => this;

  /// <inheritdoc />
  public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) {
    ArgumentNullException.ThrowIfNull(entity, nameof(entity));

    await _connection.InsertAsync(entity).WithCancellationAsync(cancellationToken);
  }

  /// <inheritdoc />
  public async Task AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) {
    ArgumentNullException.ThrowIfNull(entities, nameof(entities));

    var nonMultipleEnumeration = entities.ToArray();

    if (nonMultipleEnumeration.Length != 0) {
      await _connection.InsertAllAsync(nonMultipleEnumeration).WithCancellationAsync(cancellationToken);
    }
  }

  /// <inheritdoc />
  public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default) {
    ArgumentNullException.ThrowIfNull(entity, nameof(entity));

    await _connection.UpdateAsync(entity).WithCancellationAsync(cancellationToken);
  }

  /// <inheritdoc />
  public async Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) {
    ArgumentNullException.ThrowIfNull(entities, nameof(entities));

    var nonMultipleEnumeration = entities.ToArray();

    if (nonMultipleEnumeration.Length != 0) {
      await _connection.UpdateAllAsync(nonMultipleEnumeration).WithCancellationAsync(cancellationToken);
    }
  }

  /// <inheritdoc />
  public async Task UpdateAsync(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action, CancellationToken cancellationToken = default) {
    ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
    ArgumentNullException.ThrowIfNull(action, nameof(action));

    var entities = await FindAsync(predicate, cancellationToken);

    if (entities.Count != 0) {
      entities.ForEach(action);
      await UpdateAsync(entities, cancellationToken);
    }
  }

  /// <inheritdoc />
  public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default) {
    ArgumentNullException.ThrowIfNull(entity, nameof(entity));

    await _connection.DeleteAsync(entity).WithCancellationAsync(cancellationToken);
  }

  /// <inheritdoc />
  public async Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) {
    ArgumentNullException.ThrowIfNull(entities, nameof(entities));

    var nonMultipleEnumeration = entities.ToArray();

    if (nonMultipleEnumeration.Length != 0) {
      await _connection.RunInTransactionAsync(connection => {
        Array.ForEach(nonMultipleEnumeration, entity => connection.Delete(entity));
      }).WithCancellationAsync(cancellationToken);
    }
  }

  /// <inheritdoc />
  public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) {
    ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

    var entities = await FindAsync(predicate, cancellationToken);

    if (entities.Count != 0) {
      await DeleteAsync(entities, cancellationToken);
    }
  }

  /// <inheritdoc />
  public async Task DeleteAsync(IEnumerable<Guid> identifiers, CancellationToken cancellationToken = default) {
    ArgumentNullException.ThrowIfNull(identifiers, nameof(identifiers));

    var nonMultipleEnumeration = identifiers.ToArray();

    if (nonMultipleEnumeration.Length != 0) {
      await _connection.RunInTransactionAsync(connection => {
        Array.ForEach(nonMultipleEnumeration, id => connection.Delete<TEntity>(id));
      }).WithCancellationAsync(cancellationToken);
    }
  }

  /// <inheritdoc />
  public async Task TruncateAsync(CancellationToken cancellationToken = default)
    => await _connection.RunInTransactionAsync(connection => {
      connection.DropTable<TEntity>();
      connection.CreateTable<TEntity>();
    }).WithCancellationAsync(cancellationToken);

  /// <inheritdoc />
  public async Task UpdateAsync(Guid id, Action<TEntity> action, CancellationToken cancellationToken = default) {
    ArgumentNullException.ThrowIfNull(id, nameof(id));
    ArgumentNullException.ThrowIfNull(action, nameof(action));

    var entity = await GetAsync(id, cancellationToken);

    if (entity is not null) {
      action(entity);
      await UpdateAsync(entity, cancellationToken);
    }
  }

  /// <inheritdoc />
  public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default) {
    ArgumentNullException.ThrowIfNull(id, nameof(id));

    await _connection.DeleteAsync<TEntity>(id);
  }

  /// <inheritdoc />
  public async ValueTask DisposeAsync()
    => await _connection.CloseAsync();
}
