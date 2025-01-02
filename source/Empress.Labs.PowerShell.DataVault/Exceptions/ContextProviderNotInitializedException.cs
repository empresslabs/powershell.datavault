// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using System.Diagnostics.CodeAnalysis;
using Empress.Labs.PowerShell.DataVault.Abstractions;

namespace Empress.Labs.PowerShell.DataVault.Exceptions;

/// <summary>
///   Represents an exception that is thrown when the <see cref="IContextProvider" /> has not been initialized.
/// </summary>
public sealed class ContextProviderNotInitializedException() : Exception("The context provider has not been initialized.") {
  /// <summary>
  ///   Throws an exception if the context provider is null.
  /// </summary>
  /// <param name="contextProvider">The context provider.</param>
  /// <exception cref="ContextProviderNotInitializedException">The context provider is null.</exception>
  public static void ThrowIfNull([NotNull] IContextProvider? contextProvider) {
    if (contextProvider is null) {
      throw new ContextProviderNotInitializedException();
    }
  }
}
