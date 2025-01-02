// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using System.Linq.Expressions;
using Empress.Labs.PowerShell.DataVault.Abstractions;
using Empress.Labs.PowerShell.DataVault.Exceptions;
using SQLite;

namespace Empress.Labs.PowerShell.DataVault;

internal sealed class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity, new() {
  private readonly SQLiteConnection _connection;

  public Repository(IContextProvider contextProvider) {
    SQLiteConnectionNotEstablishedException.ThrowIfNull(contextProvider.SynchronousConnection);

    _connection = contextProvider.SynchronousConnection;
  }

  /// <inheritdoc />
  public int Count()
    => _connection.Table<TEntity>().Count();

  /// <inheritdoc />
  public int Count(Expression<Func<TEntity, bool>> predicate) {
    ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

    return _connection.Table<TEntity>().Count(predicate);
  }

  /// <inheritdoc />
  public bool Exists(Guid id) {
    ArgumentNullException.ThrowIfNull(id, nameof(id));

    return Count(entity => entity.Id == id) > 0;
  }

  /// <inheritdoc />
  public bool Exists(Expression<Func<TEntity, bool>> predicate) {
    ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

    return Count(predicate) > 0;
  }

  /// <inheritdoc />
  public TEntity? Get(Guid id) {
    ArgumentNullException.ThrowIfNull(id, nameof(id));

    return _connection.Find<TEntity>(id);
  }

  /// <inheritdoc />
  public TEntity? Get(Expression<Func<TEntity, bool>> predicate) {
    ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

    return _connection.Find(predicate);
  }

  /// <inheritdoc />
  public IReadOnlyCollection<TEntity> Find()
    => _connection.Table<TEntity>().ToArray();

  /// <inheritdoc />
  public IReadOnlyCollection<TEntity> Find(Expression<Func<TEntity, bool>> predicate) {
    ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

    return _connection.Table<TEntity>().Where(predicate).ToArray();
  }

  /// <inheritdoc />
  public IReadOnlyRepository<TEntity> AsReadOnly()
    => this;

  /// <inheritdoc />
  public void Add(TEntity entity) {
    ArgumentNullException.ThrowIfNull(entity, nameof(entity));

    _connection.Insert(entity);
  }

  /// <inheritdoc />
  public void Add(IEnumerable<TEntity> entities) {
    ArgumentNullException.ThrowIfNull(entities, nameof(entities));

    var nonMultipleEnumeration = entities.ToArray();

    if (nonMultipleEnumeration.Length != 0) {
      _connection.InsertAll(nonMultipleEnumeration);
    }
  }

  /// <inheritdoc />
  public void Update(TEntity entity) {
    ArgumentNullException.ThrowIfNull(entity, nameof(entity));

    _connection.Update(entity);
  }

  /// <inheritdoc />
  public void Update(IEnumerable<TEntity> entities) {
    ArgumentNullException.ThrowIfNull(entities, nameof(entities));

    var nonMultipleEnumeration = entities.ToArray();

    if (nonMultipleEnumeration.Length != 0) {
      _connection.UpdateAll(nonMultipleEnumeration);
    }
  }

  /// <inheritdoc />
  public void Update(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action) {
    ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
    ArgumentNullException.ThrowIfNull(action, nameof(action));

    var entities = Find(predicate).ToArray();

    if (entities.Length == 0) {
      return;
    }

    Array.ForEach(entities, action);
    Update(entities);
  }

  /// <inheritdoc />
  public void Delete(TEntity entity) {
    ArgumentNullException.ThrowIfNull(entity, nameof(entity));

    _connection.Delete(entity);
  }

  /// <inheritdoc />
  public void Delete(IEnumerable<TEntity> entities) {
    ArgumentNullException.ThrowIfNull(entities, nameof(entities));

    var nonMultipleEnumeration = entities.ToArray();

    if (nonMultipleEnumeration.Length != 0) {
      _connection.RunInTransaction(() => {
        Array.ForEach(nonMultipleEnumeration, entity => _connection.Delete(entity));
      });
    }
  }

  /// <inheritdoc />
  public void Delete(Expression<Func<TEntity, bool>> predicate) {
    ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

    var entities = Find(predicate).ToArray();

    if (entities.Length == 0) {
      return;
    }

    Delete(entities);
  }

  /// <inheritdoc />
  public void Delete(IEnumerable<Guid> identifiers) {
    ArgumentNullException.ThrowIfNull(identifiers, nameof(identifiers));

    var nonMultipleEnumeration = identifiers.ToArray();

    if (nonMultipleEnumeration.Length != 0) {
      _connection.RunInTransaction(() => {
        Array.ForEach(nonMultipleEnumeration, id => _connection.Delete<TEntity>(id));
      });
    }
  }

  /// <inheritdoc />
  public void Truncate()
    => _connection.RunInTransaction(() => {
      _connection.DropTable<TEntity>();
      _connection.CreateTable<TEntity>();
    });

  /// <inheritdoc />
  public void Dispose()
    => _connection.Dispose();

  /// <inheritdoc />
  public void Update(Guid id, Action<TEntity> action) {
    ArgumentNullException.ThrowIfNull(id, nameof(id));
    ArgumentNullException.ThrowIfNull(action, nameof(action));

    var entity = Get(id);

    if (entity is null) {
      return;
    }

    action(entity);
    Update(entity);
  }

  /// <inheritdoc />
  public void Delete(Guid id) {
    ArgumentNullException.ThrowIfNull(id, nameof(id));

    _connection.Delete<TEntity>(id);
  }
}
