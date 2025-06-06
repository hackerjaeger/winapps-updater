﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2022, 2024, 2025  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Text.Json;
using updater.data;
using updater.software.mariadb_api;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Handles updates of MariaDB server.
    /// </summary>
    public abstract class MariaDB_Base : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for MariaDB class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(MariaDB_Base).FullName);


        /// <summary>
        /// publisher of signed binaries of MariaDB
        /// </summary>
        protected const string publisherX509 = "CN=\"MariaDB USA, Inc.\", O=\"MariaDB USA, Inc.\", L=Redwood City, S=California, C=US";


        /// <summary>
        /// expiration date of the certificate of MariaDB installer binaries
        /// </summary>
        protected static readonly DateTime certificateExpiration = new(2026, 3, 21, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="autoGetNewer">whether to automatically get newer
        /// information about the software when calling the info() method</param>
        /// <param name="_branch">the MariaDB branch to handle, e.g. "10.5"</param>
        protected MariaDB_Base(bool autoGetNewer, string _branch)
            : base(autoGetNewer)
        {
            branch = _branch;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public abstract override AvailableSoftware knownInfo();


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return ["mariadb-" + branch, "mariadb-server-" + branch, "mariadb", "mariadb-server"];
        }


        /// <summary>
        /// Determines whether the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searching for newer version of MariaDB Server " + branch + "...");
            CheckForEndOfLife();
            string json = null;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync("https://downloads.mariadb.org/rest-api/mariadb/" + branch + "/");
                task.Wait();
                json = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of MariaDB Server " + branch + ": " + ex.Message);
                return null;
            }

            Release_Wrapper wrapper = null;
            try
            {
                wrapper = JsonSerializer.Deserialize<Release_Wrapper>(json);
            }
            catch (Exception ex)
            {
                logger.Error("Error: Could not deserialize JSON data from MariaDB API!"
                    + Environment.NewLine + ex.Message);
                return null;
            }
            if (wrapper == null)
            {
                logger.Error("Error: Could not deserialize MariaDB API response!");
                return null;
            }
            if ((wrapper.Releases == null) || (wrapper.Releases.Count == 0))
            {
                logger.Error("Error: MariaDB API returned empty response!");
                return null;
            }

            Release release = null;
            int idx = -1;
            do
            {
                var maxVersion = new Triple("0.0.0");
                foreach (var item in wrapper.Releases.Keys)
                {
                    var version = new Triple(item);
                    if (version > maxVersion)
                    {
                        maxVersion = version;
                    }
                }

                release = wrapper.Releases[maxVersion.full()];
                // There should be several files for download.
                if ((release.Files == null) || (release.Files.Count == 0))
                {
                    logger.Error("Error: MariaDB API returned empty file list for release " + release.ReleaseName + "!");
                    return null;
                }
                // Find the appropriate download for 64-bit Windows.
                idx = release.Files.FindIndex(x => x.OS == "Windows" && x.PackageType == "MSI Package" && x.CPU == "x86_64");
                if (idx == -1)
                {
                    logger.Warn("Info: There seems to be no matching installer for MariaDB "
                        + maxVersion.full() + " on Windows. Trying next newest version instead.");
                    wrapper.Releases.Remove(maxVersion.full());
                    continue;
                }
                if (string.IsNullOrEmpty(release.ReleaseId)
                    || string.IsNullOrEmpty(release.Files[idx].FileDownloadURL)
                    || string.IsNullOrEmpty(release.Files[idx].Checksum.SHA256Sum))
                {
                    logger.Error("Error: MariaDB API response does not contain enough information for installer download!");
                    return null;
                }
            } while (wrapper.Releases.Count > 0 && idx == -1);
            if ((idx == -1) || (release == null))
            {
                logger.Error("Error: There is no matching installer for MariaDB " + branch + " on Windows!");
                return null;
            }

            // construct new version information
            var newInfo = knownInfo();
            newInfo.newestVersion = release.ReleaseId;
            newInfo.install64Bit.downloadUrl = release.Files[idx].FileDownloadURL;
            if (!newInfo.install64Bit.downloadUrl.StartsWith("https://"))
            {
                // Always use HTTPS, download server of MariaDB supports that.
                newInfo.install64Bit.downloadUrl = newInfo.install64Bit.downloadUrl.Replace("http://", "https://");
            }
            newInfo.install64Bit.checksum = release.Files[idx].Checksum.SHA256Sum;
            newInfo.install64Bit.algorithm = HashAlgorithm.SHA256;
            return newInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return
            [
                // Technically, having the MariaDB client command-line tool
                // running is not a blocker, because the server is forced into
                // shutdown during the upgrade, but that will also kill any
                // possibly running queries, so better don't do it.
                "mariadb",
                "mysql",
            ];
        }


        /// <summary>
        /// Gets the date when this branch of MariaDB reaches its end of life.
        /// </summary>
        /// <returns>Returns the end of life date for this release branch.</returns>
        public abstract DateTime EndOfLife();


        /// <summary>
        /// Checks whether this release branch of MariaDB has reached its EOL
        /// and will log a warning message, if that is the case.
        /// </summary>
        /// <remarks>For EOL dates of various MariaDB release branches see
        /// https://mariadb.org/about/#maintenance-policy.
        /// </remarks>
        private void CheckForEndOfLife()
        {
            if (DateTime.UtcNow > EndOfLife())
            {
                string msg = "MariaDB " + branch + " has reached its end of life."
                    + " It will no longer receive any updates or security fixes."
                    + " Please consider switching to a newer release of MariaDB,"
                    + " e.g. possibly MariaDB ";
                var current = new Triple(branch);
                var minorUpdate = current.major.ToString() + "." + (current.minor + 1).ToString();
                var majorUpdate = (current.major + 1).ToString() + ".0";
                msg += minorUpdate + " or MariaDB " + majorUpdate
                    + " - whichever of those versions is released for general availability.";
                logger.Warn(msg);
            }
        }


        /// <summary>
        /// the MariaDB server branch to use, e.g. "10.5" or "10.6"
        /// </summary>
        private readonly string branch;
    }
}
