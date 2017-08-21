﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017  Dirk Stolle

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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d = knownChecksums();
            if (!d.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum = d[languageCode];
        }


        /// <summary>
        /// gets a dictionary with the known checksums for the installers (key: language, value: checksum)
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/52.3.0/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ar", "ea88fe7f0d25b6a8b49f35aa28960f71c61998997ed1ee9ac1840babc4b527549830c952088714c9d167b28d39d5593272fa3c76d223087a76d6dc6216b6bb03");
            result.Add("ast", "5dd3a19969d51756aa2981bf9524619f7338e692c8377e1f7ec51bda6ca00da48aa898bfe5ff5281201b4969ef3e38a44bb025db648d7f4a6deaf70699361132");
            result.Add("be", "3c63322e9c65e827b8eadd7df19076f98877068c2f5697954ff8713b57fd5bdd178d5e090c4a30afd8b17ba63f8c3170838a941cd74ff79c39fb12b1a48fd6e3");
            result.Add("bg", "2097b759cec304e129e990313a3ca6c1d5a0d20f58f97fba2d44f37674a1bad90a909cc47b54d4c24743c1ac10615c87398672f0a548944abbcb4e9d7ab55e7f");
            result.Add("bn-BD", "848466f9b04b0593aee493c47e008e17d06d3e45c10dd080f0d6882a4d0dde67de77779e27d5faf76c743e201b7e5098f9b83dc0499c1212540e7da7aa9fdb69");
            result.Add("br", "16386e7399ed108613cad1f81e5f296593db41622afc041dd2199c2e4f0d85e9ff8c6ed32519a61b60ffb0848a14202395472b541ec47b20d4428c15191987e5");
            result.Add("ca", "cb4ae2019a77e567cba2d7f294a6ca72970306fc1a4f320854a0b588e334eac8aa3945416566bc19bf2192ec0efe01fdbc1760a610046682607ffbb82a5fc1cb");
            result.Add("cs", "3b196afddfd7eb98b842d7715428e0e88ad7cd16ef25af11d20afa8fe331a7a0d028ef0e4408b062ce35b7704f5564ae46ac18aad2d3ec8895c639635ec24cbf");
            result.Add("cy", "b3729296ec7121dcf026aa95563a83bf4ef65bab62560c3577d87e432e947bb06ec4272ce00dad310c3ee5bb6a16b18bbdca27ec20ca021063e4d428794a9e07");
            result.Add("da", "49ba732d72ef6e6afe4e8f3d54c0e5897605a119d9868d819e238c70db55b8e18799191c5a13c91bec93cfbeaddac3c253f64d666b4e5d2625775cd5084e5298");
            result.Add("de", "3c67797ce5af9b6ceba4fbf150070dae1aedac28fd5c40da698d7a28da17556e193c33d0f33789a7554fcbac8ae85efb00acc90abf850f65fce8133a48e795c3");
            result.Add("dsb", "7f3d49dcae41a8596ad2fc2b895fdb12768449e2367d6c44d10c8d876649a1bdc2f0d0db7ad71a0ca2034199ad5381d403fabf05b6413d3c14c6a6dcd66f4735");
            result.Add("el", "47e62d85d3e852898b5e23ca5620f25f38d167d3853323ce540892839a85f4a1443c0b240cc8eee28f8582877974ecabfd7121f1abcbfb4ca04312d178cf8d10");
            result.Add("en-GB", "b5a4706aba92c343a1d8089b24b28e9d453f587fa59127f23a69b8b9265c003059fa8cf63e0d6c3aaf04938c5a10f5203130bb6b926a1760864272f001a10928");
            result.Add("en-US", "bf701345e8c8d38b12c48715a26256f2d40ae9763cb34969ee30cc050fa3a3806eabe96e6ddc2f380ffd1961274f8906774e85819e0b077342d06636ef84bd74");
            result.Add("es-AR", "dafec02036a2c8281c29cd5f503b0048445e293357812076050bd7acb2d57f6f80083367de007a838015ffb7d68c5b7442209c9d7b6ccce2969fdd7d90c49f56");
            result.Add("es-ES", "b8b036c76541803f76dce7aa4b4b0b485116067720e5c097eee1e305effb0ca604a6f85c8edbcaaa7b07d1af674932c82f3ecb31b8e0803d4c4aa7eb0fa84dd5");
            result.Add("et", "17a7169c89da5f7ca3b16da38f799fae5bd8fb9c16afc3a5d6e5e3ddc36271263c5b4de38a51dfa61d04c484126aa02f7503716d902ea6662e7551ea0b311d49");
            result.Add("eu", "21c8c684c04cb30b9326c959af116f3e511feb4560b2b88483b2e2953a030ddcf5c53ed4c24ad54d5c3d8568ec50a1704de4dd2ae2ba3d191bcc065993730ff6");
            result.Add("fi", "93550bdbc3c43177d26aec5c278a1e3610f1bd1f8fa3093045517542391a1f6126657933d36c357f5aa56d009a8bedd5c8fcec0693c95f49080a0e01479a9709");
            result.Add("fr", "0102b789154398b008854d8098b38d455998456733f1c19850f496459e5f3c4af220f6ad4fce647f3f5e804d3567a28e9590f429a0caf6a624a7cccebf8396e3");
            result.Add("fy-NL", "bd39e65d20003c9a09985fd284aa10f6f2dc4a9544dff64d919d9b1c0e20e3d1a476189c766b038f18f68285558100b54aa743fb86bc0f95fe364c1a61f332d9");
            result.Add("ga-IE", "39f1ca80a82b6f6bfd09bf95f4159a7234bf63c5355563e6945bd96e75964ee5ebcf7ed49865fc52dad41f71578d1e213865fce02c19c9ec2ae43bc53d4ad340");
            result.Add("gd", "7ddb7604cb1bd81b3288e95af8cc482f67e58b4aaa2d6963b594c2b937cbfc718026ef3260ab8f883ddc5bd4f4a719600d80161c5e476c6ae4d6b519b65eba1e");
            result.Add("gl", "c939d8aa3ae9b942d6af544276d04c7c450a0476270a2cfb35199114319310a3b18426ec7e875348fb1b20ccf51205049b15fe083652a7facc438bf720060880");
            result.Add("he", "0550f6101240d3e2d608d900345c37b9612ff64ad1ab2b1aea7021c8461b46b70d41e1d262da1809845cadbef78a1a1d2708a8ec06a31e41eabfb9acf8e7f378");
            result.Add("hr", "6a8bbbcc1ff45e307531f0743d083a2869038b29745cec6bfd1a37ad0e53844f20891b9923319709685b7283183bc01fe08273ee1967c729d55f1928f96ad332");
            result.Add("hsb", "01e46c5e5393a727aafa9f8fe7db3a36c445a0d453b1a5948b63acacb1d922780dc5053e9a7a48e7e4aa82a21bc44a6438019a09470999ef8e0e0aad9311c18a");
            result.Add("hu", "c70aa21a7a90793b296a50c96bc07ea9b57e80d294a95be96f6df2bbe0d7a7d6ced2b24846bfaa6ce890dd115411987f6fe8fcfa1ba8146f2cba97ad5c455c58");
            result.Add("hy-AM", "3990f19c908733a1823062f966a429fe9cbf6ab52c07e1705c3a42d62a41217749483fc1ef9d06532670b66d5f7d572bc553d4284457f7c3cd10799bbf3eb597");
            result.Add("id", "8273a082a8bb3417bf81fcf3a22301368eae7bc81b0a810b2331c53148426e04c41a9350be36ec23fe61c2a0c5fd3fe9981219335d490198d1d6e708190c7771");
            result.Add("is", "dbdcb4d29d5c1f4ed6cc6db7ba75024ddc91e1289daad09a9842bb628b5e52724ddfd5e4dca117b29294bfdd7889204cb43bab1a90c47c03b0cab3adadd007ed");
            result.Add("it", "4e0f92f4cfbbf685db7e23f9aee9a39c93d5bfa80cb8424b36f4c6043ac9061e59f7a30467df35a84c02702bc34c1bbf912141cd15b43ae77632fb5832799e38");
            result.Add("ja", "d0bd3a914c6b395680f28ccfc4de1e07bccbef1dddf155168fdfa17d78b47bd7c01364f8757fa22aefe888b14c1353391a7cd9725aaf6e51c0cc16e8c4c7052a");
            result.Add("kab", "f593694bbb30039af22e8d0bc110d43b836091a1035ea6ffd81b04c5068418dbf7e1ace456885bef598712c75b53e94bdab38d11ae1c6719ed38673600cd5633");
            result.Add("ko", "d4b40e423192f39fdc6c83945cd0db988d273111f6362b0e21c20e403e37a9c32dd2c0af11141f6c14c5df69573b495e96e9851ef82a28e3313ddf7f46f03824");
            result.Add("lt", "ecdbce195d3a3fdbd43f79de13f3665b2fcbcf0a1443d6b599053f6a95c9bcc2a2aa1fdc926493c8e93d50ab81ba4b187f011eefdfbaff58f18dd3f2bfbd059d");
            result.Add("nb-NO", "bf3ec7b88baeeb7b72b63f22d73c0eb1b4a6d675b0d866a03e550ceadf39602a5db3ed8267a820544201d264937cf70b09cdaadfa1fa12eb0211a25dfe291046");
            result.Add("nl", "d96ad9755a1211e00954dd3158a471f32c3a270a6b6bed015cc4fac20ba541eb0c790efdc68285c22f3c0f19b3cf71e5cd8996eaf00bee8123c2555a29d8b4ca");
            result.Add("nn-NO", "88b937206906b3d92c2d424303a8707470e8121926c1131eff974d7d8ffa258a356c9d37cccee6495924ea5855b2c6fdbb7836f1c64fc152620c02998976710f");
            result.Add("pa-IN", "e2dc60b635ee2b72edb0583e3a2622b2809994e4bce2f2f0218b76942a320d61bfd59b32a8fed54d1eed8c2b24aa8450436f0b1c0ff4bd7bfd18ca5c4bc05b25");
            result.Add("pl", "507880e4f949d974519f7c1532c7e1ac4d0c900dd2f10f9befa25c9a5d581054f06149ea07e155dfa5bdcfd2e941ff91721f84db31a9c8f9aa7997d27c58d10e");
            result.Add("pt-BR", "71ecbd7ce8dc83dabac4c247f176de620194ec4f46df3a63c6703a35020866512c00785253d5bd6c3568c2dbb2655970bf4b542b174c212119c7cafb4e3f21b9");
            result.Add("pt-PT", "51475f291a931a878bdb5bab80c2d6200e38c4f81375645aba2e1d01811d3ca82324f3dbb5188263d1e4dd71ec9307f0cbf051d8a1976fe6ef8147854d218589");
            result.Add("rm", "3a4d1a5a5b62abb3972c1da2be074b39f1da98b797672cc9d46d1a2ed952fa076e015299136ad3ab96b1864738cb47cd253ffbe1605ca3d38d7e231401c89a15");
            result.Add("ro", "481c85ffcd27a91cab2794a3f2b859b489f0dccde073149b1c9d91a7fb270192e8fab16f308d1e9925bd98049c4f360e4c4e895e18ccb5b3c2107f537062fe5b");
            result.Add("ru", "e859df67550a737dce580b28c65f00d9cf85e70f5f8383d2632cfd9d2500860ca5b25d7bccea1dfac85128db67135f37030844c65bf98766f79280a1013c6c46");
            result.Add("si", "1efe6f93d56f736f19d79f8e154b18df8b343ac552dc440d8403e94bbe821144c42bb7341fc8d191a065c2a005ea0e8424b284195679eb1f82782dc3ef10bc95");
            result.Add("sk", "aad730c117f8b793c00427bfcfd16bb85a004b00a1229c45ba2b6032732d00ef9b0ebeec374d7aed838b08fadc2147d4215661655f5ff65441c3d9a21c24eef1");
            result.Add("sl", "a5c562b84c2e63c510ac55e716ed8b28d702937d76f6345990dcaaa978a75d1ff1399e40acafd79ef8af69f08220576302ea362d53bec5ceebad5ee974b85ba4");
            result.Add("sq", "49785c5e4c6a99070c8cc5db92484a7500727017a145bc1100f3402d25319cb3c083d8ef4a4af295e2da5bc36f54c00de0f7683f083c70bd1a1b753ddf8f90e1");
            result.Add("sr", "4f97f7e619642d851c3b7a97cf44cba12dc4f26b3053936148587f3fb645c9543f69301d1a7b17efba5aabe936a35a754b0c658212184fcc76dd3ddb5f86e4b7");
            result.Add("sv-SE", "1bc533344a12953b78554de6a4f9933b2da03ac72e368d6faf05f33f0d977a47da305b8ab09447b5fd5a40524eb27d8844a9a6b1f21cf087c427e884e2120bcb");
            result.Add("ta-LK", "f2b330c9c9ea6d3943156863d42f7aa0b4e4ae61ad599dd239907a10a31a174737c2e50cdc97d168182fa75f6a6c185ce4141956c97a3835cb1f4d01880a9074");
            result.Add("tr", "73623a2658521fa7a7263a3f9f14f87eb06f0d32a65de316453d477d8731716314d8d2e196959f517353f7699751dfd21cf96f1ddc2ec61908be9f292d07785a");
            result.Add("uk", "c798f4651c8a73aea2e4cb7ce64ce120a70762206e3b59b7bde03066cf64c2c35f99ee44e17db741a5784e2ba318d9ef7d15cada69902f6e5e3ffb7e6cd97b29");
            result.Add("vi", "c4b52abd2b58665f43673d568b1f625949250a8ae917a6c70131ddfedfc019fa81042346bba2ecff2ea6471a99f78a9272423df2cc259386c0c3eaa03a40ff67");
            result.Add("zh-CN", "48be3be4eeebf6c7350a7d1cec5ab714278726b51e48602874ed4e839a1832ce260d8477556f191c984208d40cce2043143336ad190284d5c956f6346e5e34b4");
            result.Add("zh-TW", "a1d2afd269b625047662d8be5e09532499e3d96d50e821c89d7d64aee88a923d852ccfbbef092f379854a5999844c26e2beeb34508fb2f2f81b117740e719776");
            
            return result;
        }


        /// <summary>
        /// gets an enumerable collection of valid language codes
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums();
            return d.Keys;
        }


        /// <summary>
        /// gets the currently known information about the software
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            const string version = "52.3.0";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird [0-9]{2}\\.[0-9]\\.[0-9] \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                null,
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum,
                    "CN=Mozilla Corporation, O=Mozilla Corporation, L=Mountain View, S=California, C=US",
                    "-ms -ma",
                    "C:\\Program Files\\Mozilla Thunderbird",
                    "C:\\Program Files (x86)\\Mozilla Thunderbird"),
                //There is no 64 bit installer yet.
                null);
        }


        /// <summary>
        /// list of IDs to identify the software
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// tries to find the newest version number of Thunderbird
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]{2}\\.[0-9]\\.[0-9]");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// tries to get the checksum of the newer version
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successfull.
        /// Returns null, if an error occurred.</returns>
        private string determineNewestChecksum(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/45.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } //using
            //look for line with the correct language code and version
            Regex reChecksum = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum = reChecksum.Match(sha512SumsContent);
            if (!matchChecksum.Success)
                return null;
            // checksum is the first 128 characters of the match
            return matchChecksum.Value.Substring(0, 128);
        }


        /// <summary>
        /// whether or not the method searchForNewer() is implemented
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// looks for newer versions of the software than the currently known version
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Debug("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string newerChecksum = determineNewestChecksum(newerVersion);
            if (string.IsNullOrWhiteSpace(newerChecksum))
                return null;
            //replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksum;
            return currentInfo;
        }


        /// <summary>
        /// lists names of processes that might block an update, e.g. because
        /// the application cannot be update while it is running
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            var p = new List<string>();
            p.Add("thunderbird");
            return p;
        }


        /// <summary>
        /// whether or not a separate process must be run before the update
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate proess returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// returns a process that must be run before the update
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            //uninstall previous version to avoid having two Thunderbird entries in control panel
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
        /// </summary>
        private string languageCode;


        /// <summary>
        /// checksum for the installer
        /// </summary>
        private string checksum;

    } //class
} //namespace
