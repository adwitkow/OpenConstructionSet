﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace OpenConstructionSet
{
    public static class OcsSteamHelper
    {
        private static string GetSteamFolder()
        {
            try
            {
                var registryKey = Environment.Is64BitProcess ? @"SOFTWARE\Wow6432Node\Valve\Steam" : @"SOFTWARE\Valve\Steam";

                using (var key = Registry.LocalMachine.OpenSubKey(registryKey))
                {
                    return key.GetValue("InstallPath").ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to find Steam folder", ex);
            }
        }

        private static IEnumerable<string> GetLibraries()
        {
            var steamFolder = GetSteamFolder();

            var path = Path.Combine(steamFolder, "steamapps", "libraryfolders.vdf");

            // [whitespace] "[number]" [whitespace] "[library path]"
            const string pattern = "^\\s+\"\\d+\"\\s+\"(.+)\"";

            foreach (var line in File.ReadLines(path))
            {
                var match = Regex.Match(line, pattern);

                if (match.Success)
                {
                    yield return match.Groups[1].Value;
                }
            }

            yield return steamFolder;
        }

        private static bool TryFindGameFolder(out string path)
        {
            foreach (var library in GetLibraries())
            {
                path = Path.Combine(library, "steamapps", "common", "Kenshi");

                if (Directory.Exists(path))
                {
                    return true;
                }
            }

            path = null;
            return false;
        }

        /// <summary>
        /// Attempt to find the default data and mods folders.
        /// </summary>
        /// <param name="defaultFolders">If found this will be set to a <see cref="DefaultFolders"/> containing (You guessed it) the default folders.<</param>
        /// <returns><c>true</c> if the default folders are found.</returns>
        public static bool TryFindDefaultFolders(out DefaultFolders defaultFolders)
        {
            if (!TryFindGameFolder(out var gameFolder))
            {
                defaultFolders = null;
                return false;
            }

            defaultFolders = new DefaultFolders()
            {
                Base = ModFolder.Base(Path.Combine(gameFolder, "data")),
                Mod = ModFolder.Mod(Path.Combine(gameFolder, "mods")),
            };

            return true;
        }
    }
}