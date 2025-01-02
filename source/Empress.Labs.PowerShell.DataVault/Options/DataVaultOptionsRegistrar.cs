// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using System.Diagnostics.CodeAnalysis;
using static Empress.Labs.PowerShell.Common.ExtendedDataAnnotations.Prelude;

namespace Empress.Labs.PowerShell.DataVault.Options;

[ExcludeFromCodeCoverage]
internal sealed class DataVaultOptionsRegistrar {
  public DataVaultOptionsRegistrar(DataVaultOptions.Setup setup) {
    var builder = new DataVaultOptionsBuilder();
    setup.Invoke(builder);

    Options = builder.Apply();

    ValidateOrThrow(Options);
  }

  /// <summary>
  ///   The options for the data vault.
  /// </summary>
  public DataVaultOptions Options { get; }
}
