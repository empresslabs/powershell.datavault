// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using System.Diagnostics;
using SQLite;

namespace Empress.Labs.PowerShell.DataVault;

/// <summary>
///   Base class for entities.
/// </summary>
[Serializable]
[DebuggerDisplay("{ToString(),nq}")]
public abstract class Entity : IComparable<Entity>, IEquatable<Entity> {
  /// <summary>
  ///   The unique identifier of the entity.
  /// </summary>
  [PrimaryKey]
  [Column("id")]
  [MaxLength(36)]
  public Guid Id { get; init; } = Guid.NewGuid();

  /// <inheritdoc />
  public int CompareTo(Entity? other)
    => other is null ? 1 : Id.CompareTo(other.Id);

  /// <inheritdoc />
  public bool Equals(Entity? other)
    => other is not null && Id.Equals(other.Id);

  /// <inheritdoc />
  public override bool Equals(object? obj)
    => obj is Entity other && Equals(other);

  /// <inheritdoc />
  public override int GetHashCode()
    => Id.GetHashCode();

  /// <inheritdoc />
  public sealed override string ToString()
    => Id.ToString();
}
