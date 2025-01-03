// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using Empress.Labs.PowerShell.Common.IO;

namespace Empress.Labs.PowerShell.DataVault.Reflection;

/// <summary>
///   The SQLite Portable Class Library loader.
/// </summary>
/// <remarks>
///   This exists because PowerShell modules are published at most as Portable AnyCPU, and the SQLite library
///   cannot find the native library. This class is responsible for loading the native library based on the platform.
/// </remarks>
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class SQLitePortableClassLibrary {
  /// <summary>
  ///   Tries to load the native SQLite library based on the platform.
  /// </summary>
  /// <exception cref="InvalidOperationException">The root path of the assembly could not be determined.</exception>
  /// <exception cref="DllNotFoundException">Failed to load the native SQLite library.</exception>
  public static void TryLoad() {
    try {
      var path = AbsolutePath.NewPath(typeof(SQLitePortableClassLibrary).Assembly.Location);
      var platform = GetPlatformIdentifier();
      var parent = path.Parent ??
                   throw new InvalidOperationException("Could not determine the root path of the assembly.");
      var nativeLibraryPath = parent / "runtimes" / platform / "native" / GetLibraryName();
      var directoryPath = nativeLibraryPath.Parent;

      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
        LoadOnWindows(directoryPath, nativeLibraryPath);
      }
      else {
        LoadOnUnix(nativeLibraryPath);
      }
    }
    catch (Exception ex) {
      throw new DllNotFoundException("Failed to load the native SQLite library.", ex);
    }
  }

  private static string GetPlatformIdentifier() {
    var osName = RuntimeInformation.OSDescription.ToLower();
    string rid;

    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
      rid = "win";
      rid += RuntimeInformation.ProcessArchitecture switch {
        Architecture.X86 => "-x86",
        Architecture.X64 => "-x64",
        Architecture.Arm => "-arm",
        Architecture.Arm64 => "-arm64",
        var _ => throw new PlatformNotSupportedException("Unsupported architecture")
      };
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
      rid = osName.Contains("alpine") ? "alpine" : "linux";

      rid += RuntimeInformation.ProcessArchitecture switch {
        Architecture.X86 => "-x86",
        Architecture.X64 => "-x64",
        Architecture.Arm => "-arm",
        Architecture.Arm64 => "-arm64",
        Architecture.Armv6 => "-armel",
        Architecture.S390x => "-s390x",
        var _ => throw new PlatformNotSupportedException("Unsupported architecture")
      };
      if (osName.Contains("musl")) {
        rid += "-musl";
      }
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
      rid = "osx";
      rid += RuntimeInformation.ProcessArchitecture switch {
        Architecture.X64 => "-x64",
        Architecture.Arm64 => "-arm64",
        var _ => throw new PlatformNotSupportedException("Unsupported architecture")
      };
    }
    else {
      throw new PlatformNotSupportedException("Unsupported operating system");
    }

    return rid;
  }

  private static string GetLibraryName() {
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
      return "e_sqlite3.dll";
    }

    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
      return "libe_sqlite3.so";
    }

    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
      return "libe_sqlite3.dylib";
    }

    throw new PlatformNotSupportedException("Unsupported operating system for library loading");
  }

  private static void LoadOnWindows(string? directoryPath, string libraryPath) {
    if (!Directory.Exists(directoryPath)) {
      throw new FileNotFoundException($"The directory {directoryPath} does not exist.");
    }

    if (SetDllDirectory(directoryPath)) {
      var handle = LoadLibrary(Path.GetFileName(libraryPath));
      if (handle == IntPtr.Zero) {
        throw new DllNotFoundException($"Failed to load the library {libraryPath}: {Marshal.GetLastWin32Error()}");
      }
    }
    else {
      throw new Exception($"Failed to set DLL directory: {Marshal.GetLastWin32Error()}");
    }

    return;

    [DllImport("kernel32", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    static extern IntPtr LoadLibrary(string lpFileName);

    [DllImport("kernel32", SetLastError = true)]
    static extern bool SetDllDirectory(string lpPathName);
  }

  private static void LoadOnUnix(string libraryPath) {
    if (!File.Exists(libraryPath)) {
      throw new FileNotFoundException($"The file {libraryPath} does not exist.");
    }

    try {
      var path = FindLibrary("libdl.so.2") ?? FindLibrary("libdl.so");

      if (string.IsNullOrEmpty(path)) {
        throw new DllNotFoundException("Could not find libdl.so or libdl.so.2 in the ldconfig cache.");
      }

      // Load the library using the found path
      if (!NativeLibrary.TryLoad(path, Assembly.GetExecutingAssembly(), null, out var _)) {
        throw new DllNotFoundException($"Failed to load the library {path}: {Marshal.GetLastWin32Error()}");
      }
    }
    catch (Exception ex) {
      throw new DllNotFoundException($"Failed to load library: {ex.Message}", ex);
    }

    return;

    static string? FindLibrary(string libraryName) {
      var startInfo = new ProcessStartInfo {
        FileName = "bash",
        Arguments = $"-c \"ldconfig -p | grep -oP '(?<=\\s=>\\s)[^ ]+{libraryName}'\"",
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
      };

      using var process = Process.Start(startInfo);
      using var reader = process?.StandardOutput;
      var output = reader?.ReadToEnd();
      return output?.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
    }
  }
}
