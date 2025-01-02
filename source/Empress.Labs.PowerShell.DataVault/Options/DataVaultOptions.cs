// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Empress.Labs.PowerShell.Common.ExtendedDataAnnotations;
using Empress.Labs.PowerShell.Common.IO;
using Empress.Labs.PowerShell.DataVault.Options.Abstractions;
using SQLite;
using DataAnnotationsMaxLength = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace Empress.Labs.PowerShell.DataVault.Options;

/// <summary>
///   Options for the data vault library.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public readonly record struct DataVaultOptions {
  /// <summary>
  ///   The setup delegate.
  /// </summary>
  public delegate void Setup(IDataVaultOptions options);

  /// <summary>
  ///   The connection type to use.
  /// </summary>
  public enum ConnectionType {
    /// <summary>
    ///   The asynchronous connection type.
    /// </summary>
    Asynchronous = 1 << 0,

    /// <summary>
    ///   The synchronous connection type.
    /// </summary>
    Synchronous = 1 << 1
  }

  /// <summary>
  ///   The file name of the database.
  /// </summary>
  /// <remarks>
  ///   Everything besides the file name will be ignored.
  ///   <br />
  ///   The default file extension is: <c>.db3</c>
  /// </remarks>
  [Required(ErrorMessage = "The file name is required.")]
  [NotNullOrEmpty(ErrorMessage = "The file name cannot be null or empty.")]
  [DataAnnotationsMaxLength(255, ErrorMessage = "The file name must have at most 255 characters.")]
  public required string FileName { get; init; }

  /// <summary>
  ///   The flags to open the database.
  /// </summary>
  /// <remarks>
  ///   The default flags are: <c>Create | ReadWrite | SharedCache</c>
  /// </remarks>
  [EnumDataType(typeof(SQLiteOpenFlags), ErrorMessage = "The SQLite open flags are invalid.")]
  public SQLiteOpenFlags SQLiteOpenFlags { get; init; }

  /// <summary>
  ///   The connection type to use.
  /// </summary>
  [EnumDataType(typeof(ConnectionType), ErrorMessage = "The connection type is invalid.")]
  public ConnectionType SQLiteConnectionType { get; init; }

  /// <summary>
  ///   Gets the path of the database.
  /// </summary>
  /// <returns>The path of the database.</returns>
  public AbsolutePath GetPath()
    => InternalState.RootDirectory / $"{FileName}.db3";
}
