using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace CMouss.ENVs
{
    /// <summary>
    /// Static environment variable manager that loads configuration from text files
    /// Supports Windows (C:\ENVs\) and Linux (/etc/envs/)
    /// </summary>
    public static class ENVManager
    {
        private static readonly Dictionary<string, string> _envVariables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static bool _isInitialized = false;
        private static readonly object _lock = new object();

        /// <summary>
        /// Gets the base environment directory based on the operating system
        /// Windows: C:\ENVs\
        /// Linux: /etc/envs/
        /// </summary>
        private static string BaseEnvironmentDirectory
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return @"C:\ENVs";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return "/etc/envs";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return "/etc/envs";
                }
                else
                {
                    throw new PlatformNotSupportedException("Unsupported operating system");
                }
            }
        }

        /// <summary>
        /// Initializes the environment manager for the specified application
        /// </summary>
        /// <param name="appName">Application name (folder name under base environment directory)</param>
        /// <param name="envName">Optional environment name (e.g., "Staging", "Production"). If null, only Base.txt is loaded</param>
        public static void UseEnvironment(string appName, string envName = null)
        {
            lock (_lock)
            {
                if (_isInitialized)
                {
                    throw new InvalidOperationException("Environment manager has already been initialized. Call Reset() first if you need to reinitialize.");
                }

                if (string.IsNullOrWhiteSpace(appName))
                {
                    throw new ArgumentException("Application name cannot be null or empty", nameof(appName));
                }

                string basePath = Path.Combine(BaseEnvironmentDirectory, appName);

                if (!Directory.Exists(basePath))
                {
                    throw new DirectoryNotFoundException($"Environment directory not found: {basePath}");
                }

                // Load Base.txt first
                string baseFilePath = Path.Combine(basePath, "Base.txt");
                LoadEnvironmentFile(baseFilePath, isRequired: true);

                // If environment is specified, load and overlay those values
                if (!string.IsNullOrWhiteSpace(envName))
                {
                    string envFilePath = Path.Combine(basePath, $"{envName}.txt");
                    LoadEnvironmentFile(envFilePath, isRequired: true);
                }

                _isInitialized = true;
            }
        }

        /// <summary>
        /// Gets the value for the specified parameter name
        /// </summary>
        /// <param name="paramName">Parameter name</param>
        /// <returns>Parameter value</returns>
        public static string GetValue(string paramName)
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("Environment manager has not been initialized. Call UseEnvironment() first.");
            }

            if (string.IsNullOrWhiteSpace(paramName))
            {
                throw new ArgumentException("Parameter name cannot be null or empty", nameof(paramName));
            }

            lock (_lock)
            {
                if (_envVariables.TryGetValue(paramName, out string value))
                {
                    return value;
                }
            }

            throw new KeyNotFoundException($"Parameter '{paramName}' not found in environment variables.");
        }

        /// <summary>
        /// Tries to get the value for the specified parameter name
        /// </summary>
        /// <param name="paramName">Parameter name</param>
        /// <param name="value">Output parameter value</param>
        /// <returns>True if parameter exists, false otherwise</returns>
        public static bool TryGetValue(string paramName, out string value)
        {
            value = null;

            if (!_isInitialized || string.IsNullOrWhiteSpace(paramName))
            {
                return false;
            }

            lock (_lock)
            {
                return _envVariables.TryGetValue(paramName, out value);
            }
        }

        /// <summary>
        /// Gets all loaded environment variables
        /// </summary>
        /// <returns>Read-only dictionary of all parameters</returns>
        public static IReadOnlyDictionary<string, string> GetAll()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("Environment manager has not been initialized. Call UseEnvironment() first.");
            }

            lock (_lock)
            {
                return new Dictionary<string, string>(_envVariables);
            }
        }

        /// <summary>
        /// Resets the environment manager (useful for testing or reinitialization)
        /// </summary>
        public static void Reset()
        {
            lock (_lock)
            {
                _envVariables.Clear();
                _isInitialized = false;
            }
        }

        /// <summary>
        /// Checks if the environment manager has been initialized
        /// </summary>
        public static bool IsInitialized => _isInitialized;

        /// <summary>
        /// Gets the current base environment directory path
        /// </summary>
        public static string GetBaseDirectory() => BaseEnvironmentDirectory;

        private static void LoadEnvironmentFile(string filePath, bool isRequired)
        {
            if (!File.Exists(filePath))
            {
                if (isRequired)
                {
                    throw new FileNotFoundException($"Environment file not found: {filePath}");
                }
                return;
            }

            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                // Skip empty lines and comments
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                {
                    continue;
                }

                // Find the first occurrence of '='
                int equalsIndex = line.IndexOf('=');

                // Skip empty lines and comments
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                {
                    continue;
                }

                if (equalsIndex <= 0) // No '=' found or '=' is at the start
                {
                    continue; // Skip malformed lines
                }



                // Everything before first '=' is the key
                string key = line.Substring(0, equalsIndex).Trim();

                // Everything after first '=' is the value
                string value = line.Substring(equalsIndex + 1).Trim();

                if (!string.IsNullOrEmpty(key))
                {
                    // Add or update the value (overlay behavior)
                    _envVariables[key] = value;
                }
            }
        }
    }
}
