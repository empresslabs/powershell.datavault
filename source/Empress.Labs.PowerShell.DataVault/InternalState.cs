// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using System.Diagnostics.CodeAnalysis;
using Empress.Labs.PowerShell.Common.IO;
using Empress.Labs.PowerShell.DataVault.Abstractions;

namespace Empress.Labs.PowerShell.DataVault;

[ExcludeFromCodeCoverage]
internal static class InternalState {
  /// <summary>
  ///   The root directory.
  /// </summary>
  public static AbsolutePath RootDirectory { get; set; }
    = AbsolutePath.NewPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EmpressLabs"));

  /// <summary>
  ///   The context provider.
  /// </summary>
  public static IContextProvider? ContextProvider { get; set; }

  /// <summary>
  ///   The asynchronous repository instances.
  /// </summary>
  public static Dictionary<Type, object> AsynchronousRepositories { get; } = [];

  /// <summary>
  ///   The synchronous repository instances.
  /// </summary>
  public static Dictionary<Type, object> SynchronousRepositories { get; } = [];
}
