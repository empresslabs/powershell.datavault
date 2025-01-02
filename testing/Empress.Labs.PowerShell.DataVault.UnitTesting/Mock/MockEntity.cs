// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using SQLite;

namespace Empress.Labs.PowerShell.DataVault.UnitTesting.Mock;

[Table("mock")]
public sealed class MockEntity : Entity {
  [Column("name")]
  public string Name { get; set; } = null!;
}
