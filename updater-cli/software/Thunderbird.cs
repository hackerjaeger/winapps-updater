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
using System.Net;
using System.Text.RegularExpressions;
using updater_cli.data;

namespace updater_cli.software
{
    public class Thunderbird : ISoftware
    {
        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        public Thunderbird(string langCode)
        {
            if (string.IsNullOrWhiteSpace(langCode))
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            languageCode = langCode.Trim();
            var d = knownChecksums();
            if (!d.ContainsKey(languageCode))
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            checksum = d[languageCode];
        }


        /// <summary>
        /// gets a dictionary with the known checksums for the installers (key: language, value: checksum)
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/45.7.1/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ar", "374cbf2619b3bcffc104f9156e56ccfd06c7e12f7aa953a58a6acfaa19e6f929e2ce4a8aa6ea754f210d4378fc63517fabff92b21e47a64d071b99c55cb72b6d");
            result.Add("ast", "47b378a20f85cef1487fdd81732eeb43ab8ebf796a5c674c53e8cd0e0f675a810b4e5f388132acefa0575ff9f22b981ff3cec69c362ee24d0d75fa8d7b2b4221");
            result.Add("be", "f9e668d6a84793191a83d2d53c17fd360b2270ffc230c5b94dba203f3461fa6f8eaca9b9834e1e87b7254c2faf08120cde57da5803e93e1c8e599117cf725548");
            result.Add("bg", "3148f3b71be8b87a2a1ba808c4f38db8f75988432b275ea3ce27d543ebdcab6f5df990965352a951bee6e6fa3c5932e4007b10da0f96b1f332074c44db35e0ce");
            result.Add("bn-BD", "e07ed67bf2e1736f943151c1c7208bd6bae848268607b1df5d9486b093241c2a35cd4d4a3e11e781e12d2363691982813cac2df82e750c28b1c8c6702014619a");
            result.Add("br", "1a0f63f503690dd6a8b32b699c5b071ef1b098f59a72f6cf676098ebdb9d41fbdaa9d4918b3e17bec2e7478f3e658421e0491e4a9c65d28a965e719ff62d7a2a");
            result.Add("ca", "4bd757bd5466151b0e3118646cbde803f542d97e5d5ac8fe45a77dc0a9ede76ecba860302e814758dc748aeffd1afa843155376021a7cedb3520ba54bf7d2de5");
            result.Add("cs", "202a3879fb752ce67e467e0dc8e4ccf09d2a0561b5f635d0067e3654fa92d38249909aba07ca1752f2b950b37c53bb30e19b8366ef8d693443b5edf3912911a0");
            result.Add("cy", "7a845a5b7d392722c20ac10ba84029404f21fc2c2d8e670eb108e897bdcceae869172f1c64a73886816152eda44e51e722c8393fa9d8e1cae1b72f27b2061f0e");
            result.Add("da", "1010dac2c30b08ec6867f7b1f68fadd45048814b19562341d4b41d4829f2f9808b0dde0b5510a98f34326ba6be91eb773827ed9ac58b2ea8bd580364eece6876");
            result.Add("de", "9aa96f74067523a35468e2160392c740bb19d59576284d68561d70fdd7226ad2a3263ca72dc1a72cb27c6b2e0c6b9e322d7cc3248ea7995d6b006366acf7801e");
            result.Add("dsb", "3f6dfd3d6796ae3c79551fb3cb5d99266bcc57a921d0dd5324c263114c9b48706d7babe59894b24f3b505100ca53c91d1677903ff4b4fe2e75dfd86f4d909168");
            result.Add("el", "44ede49b1517454dec0f6881bb783d46c9bfc8d80aee1d9e38cf93fd988fe017c1989d997db790d6a7d0def988b67e6fb7733c74daeb929af5fb2308bb812600");
            result.Add("en-GB", "69d1192446d7dbd62251e824ee73ee8cba751d1bf13bba303406ad5e0bbf3c882e885f99eef2a9ecbc94156a40481b81e02bd3880e5a46615820253d163f7eff");
            result.Add("en-US", "f87799180918f6030054a193b51dc90cf9b2824b5c6e66a158400f1e69bb24bd86e0dbe07d64493701ebe7dc59371f5da40ff9e9a2996c0ea196024b13cbb60b");
            result.Add("es-AR", "8fcccff0c0d06e8366a5a2678a073297440de3e857a45fb272f2ed2e8091f6393a2f83e7cecaa5bc89ce539f0d547de3b0871e1fd758e137a19a3d1d626db4b0");
            result.Add("es-ES", "0b85f23cc2b4afc92b2f82be3fdc149df07d4e10405b221498bc96fc09a09212cbf2e5ecd440905b7b7908bb710aba8d79c6dbb026ef22f903d9564df2cbfb6e");
            result.Add("et", "4d02a17bb63f841c566826e100a5a34dae29467ab0da680edd345d56837ee4022c62be7a5a9d510cea30f545020ac34a5629e5d4f6df677fcb7a03a389aae988");
            result.Add("eu", "203292f04bae9f2174f8c356a91149677a38a60ead73e595d14bae85c612449217a8845f617cb3e239c5118798615b1238e2ce3a4af26006a9a05bd6118ca400");
            result.Add("fi", "f5e382c829d864724a25fcfefc0bb7945cd52dce4e52c62c0ea7b16006c7d2b8288922aa6c505459605ec14bbded54d3b4a5282a377bc5d88364d608e0468a7a");
            result.Add("fy-NL", "f758cc6455881aff1bd95d8bdf31344d01b3d8acb68193ed1ad5fa15ff1d0f0c224ddc51e4324fb0fdf3e4a1a7d8ef23abe9ef7ea72d446314f1c399918b944c");
            result.Add("ga-IE", "a218aa3e5f17a418e713c846c41239448e29c3ac9a32d71867852c52122e13e7be592b44b4bf9b464018470be20f2323b8ff680ff84b748ba77ec3315b0bdc3f");
            result.Add("gd", "8861e101675404634ceba29fc4aba9b6ce415fb1eadf8068452d9788b990ba4cd75c3b15447478c6edf39bd2d915f00b5b016d78bdb3feed3a9434a05ab940e1");
            result.Add("gl", "9891d31cc4d27c6beed30d91f4f78f38e373ce24b021984c5199f1a7f06b3dad2395ead75facb3f36aaed3c9a795bc475cf8fbd6d7a47fda3a25043f84a5a7c1");
            result.Add("he", "6977f84ae5a19a0eaef67ff280f803b48014d8e972b31d5d07fd7751b740efbd6d73590b8d268790f8b7e02ca048f7ee1c3333540f6af56bfafd6005bbfff4ac");
            result.Add("hr", "a7ec28799d1337842ae045b7a9d3dfe4ffa5a76c4abf94ece5457d477e7df2f70170b742429b92ffc6e9ac9952d51a4fac9a49468e0375e1a313bec20b7a5337");
            result.Add("hsb", "0828e50fda61a8559f9f24958ee97ffeaf6b8bd185ec1db841334997769771614082a9c8a137c90b9698d01220729f26acbcde5ac8ddfadbbc8dd4ad9d5305ba");
            result.Add("hu", "0a5ff4443920be3e37e5a99dfc80d26e8d952bce767099ba46d528f8c3bcc3a653a1189100ea65a954a042cdfce4d701c2de23fcac1f6d06a1e726e17783e484");
            result.Add("hy-AM", "8416c07d8bccb5b1156d4b01e270ddac670ede8edaec764066eec409f007be6cf4d0a9a8c7851e8745655c32da08cf28fa628514a1e16bb0f15a02bc714e2045");
            result.Add("id", "a4af1548c4e653f4ba69b7332d087997847dd215317a505576097d452aed14004887f4cff8fc504f3c186da619677c49be1fe8e3e0aeb8b2ec910b8dd929a4bf");
            result.Add("is", "7b66ccb13beb6fcd2fedb4437f430592c66ddba61f0ad3196b4c0613281abd9ed0504c18427a04ed8ec7bdd9484b635bad6c6ecdb1e7c181510d7285bf24c441");
            result.Add("it", "e5eaad465cb04972cab88e35ffba3e863dc0e9f9d4f706e3779befcf8c5436c22bdfb827277c6d01d194004b4d36e20872730d4d3ddc3eeb309c09ec3cb3901b");
            result.Add("ja", "98619abf63a2a0cef6b48025791beec57d3c4f67f8ee1be267e82c215c5a78d976fccad54772186bcf325e3f37d55f836ab9088ba0ef846c0b1f86f23de8e50a");
            result.Add("ko", "23ebc82a77523589a0d023edc283c702f0ac09c7da598f683fd698cf19772c3ce7692584d552c5caf4c8b35c994b73f631c1b3a43bfb29bf2efc091ad00e5b69");
            result.Add("lt", "2d4085e3c1cc3c92f91c352afcb1e44dc0f96bab6400adeb8f03eac0e6ab47e0dcfd43dcd8ce77127b4957e59999b5f574987a94c728baef1747b7ea62d02c87");
            result.Add("nb-NO", "1ea834e43969ea27995c71b98c0b279559dc3c7d4b5d573b1376817496e318d9e3107c64dea7b7d3ef1dc11a249c5262c192cb9e3f414cae46b813e28f330649");
            result.Add("nl", "351089b9c31d609e7a86fadc72ba1981b27fc66d795f30f38dd7b056c905f30ef67210dcfa8b367bfda3b5f400a1bfb01c3652516bf608a99eaf04a45692bfcd");
            result.Add("nn-NO", "1aeb3b0f7d1a1e1a68fbf6a8d5774939c3158b241d7c4e25b07e42c88bb9ca5df3600a1d0ad5956f9b37d7d521d84a87906d27f26100febe3000718cf9258ded");
            result.Add("pa-IN", "2adc664d840adf119dfa7e972d0e5eb4af530d8fd86f7280a4c6e0b7798b3ee83517120a6c6483c413bc62adaa3a4a535ef5743b679dc012986ef4cfdd811d96");
            result.Add("pl", "dc59798cb981b215be73f712a2965a157249562cda572c3eabbde1d2e0fb0b3d75a2c1b72a44c65a3758de797d73f7f8db301569c925c786f150f190ffe87f61");
            result.Add("pt-BR", "55a4c2922b527c408c1cc5206476302c9633cf92f6d38e6fc140a220a2a14b77cfa5884dbeca5b0a4ffc11684f334627ebb43564d428c90562d78e062a03e327");
            result.Add("pt-PT", "c934791a34d48ae9952ee3f21b706041609a460e441fa53eacba0713fcbd726bf1f9eeed926c75cda1c8400f005f9fac3de9609831f7846a03b2255377d5f2b9");
            result.Add("rm", "3bfe6ead598c46dd0abc6763c64a1002f2243ed838a19248592ff98ab06c0c88fcb0aa1d20beb499b01da3f3508d6ab0f15fea5f2d7f014ed9de3a0052f7dfa6");
            result.Add("ro", "eb552e34e6b13121a1fb951d8a9a72f9b7188ad82153726ed7489bf73b0918eae53e9b7f864994737db0a80cb7dffa2678e2665b2730e8697966c456c03b8275");
            result.Add("ru", "5c93c9757744797ef95c0cdc458454e1a0a4fa96d1160ff334903628f89bd61f2416c2d7691f876b46224727761c72aea061f803b2ec929ccca8943c14d39237");
            result.Add("si", "e9b7871fe5b781847f57a51aea43dcc6296626bb3ea4047e384567585cd6b73e717756ba99a21ca22d80d7733755cbe8bc485f8e774e068506ebdfc18b6a8269");
            result.Add("sk", "f9e93b56e16d4f96c6862f2dff85ac765bcb182c54577eb1e503472e589d810f840d340718d29ca312b4e58ce8459cbb0e26f9e24fdd804eb6fb27b4c493dc77");
            result.Add("sl", "c73e63f55bde97d87d540de8a06e53ddb708981424b4e58e53617ddb5989a486b5c22d82fa2337eb62414912b6e412542992cf6a6005c1f80a87892b99adf533");
            result.Add("sq", "bd6d65e33dcec42f14eb1dc6f517a1c222b293352eb242e16d30a4df7d3c6cac37822f5fcd18fcd4f1ff0a6346d73d78bc33761147d8744a75c48674ebd675c6");
            result.Add("sr", "bf73108578648e052a95bb59cfc10c5d8ea8a23ee5c273441b57bdef4750820bac4ff525345fdd070507f9f64ba14402ea79a0cbcabb1903f6252583dd4cbc8c");
            result.Add("sv-SE", "03f3a5dd6fdee0f51be7ca08d7caf531ee845ba8eaf59f02dd080f12f204ac79a340dce2356cca9024b88bb5e2d3268e9b2389be053d14da219a6a6dd031d6b5");
            result.Add("ta-LK", "47cd58368d4b8f1dd700141723ed5c62e7ca7f16e67a58be540e929d6d8cf6da7106f2a9ebfbf833760d6f0d85c4791baf811cb4bfcc0ae16da3eaa1382a4919");
            result.Add("tr", "dd7956b57ea45641563ffc1621ffec41db4c2dcfc6e6bbc9d59447210fa20d06ee739b5680497bfe00812901f7f0d39bbf8027869f9d6b1812ed572ef47f5513");
            result.Add("uk", "8cac59a5ae7cb57d0b9ef47271c7549519c2dd56ce535ad833a86a1c4d3a7713ea7efc2e9904afb592f49e6e89275f35c58100a64c462050d0ecd270d8fb0fe0");
            result.Add("vi", "8eeb4fc19404265904ff6dcdd2bef183a2aa3748e26932e3306b5a047a6b326b8f2dd50e0761b1641c4a111e75068f94474294807a7fa7407f754e464165647a");

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
        public AvailableSoftware info()
        {
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                "45.7.1",
                "^Mozilla Thunderbird [0-9]{2}\\.[0-9]\\.[0-9] \\(x86 " + languageCode + "\\)$",
                null,
                new InstallInfo(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/45.7.1/win32/" + languageCode + "/Thunderbird%20Setup%2045.7.1.exe",
                    HashAlgorithm.SHA512,
                    checksum,
                    "-ms -ma",
                    "C:\\Program Files\\Mozilla Thunderbird",
                    "C:\\Program Files (x86)\\Mozilla Thunderbird"),
                //There is no 64 bit installer yet.
                null);
        }


        /// <summary>
        /// tries to find the newest version number of Thunderbird
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        private string determineNewestVersion()
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
                Console.WriteLine("Error while looking for newer Thunderbird version: " + ex.Message);
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
                    Console.WriteLine("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
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
        public bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// looks for newer versions of the software than the currently known version
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public AvailableSoftware searchForNewer()
        {
            string newerVersion = determineNewestVersion();
            var currentInfo = info();
            if (string.IsNullOrWhiteSpace(newerVersion) || (newerVersion == currentInfo.newestVersion))
                // fallback to known information
                return currentInfo;
            string newerChecksum = determineNewestChecksum(newerVersion);
            if (string.IsNullOrWhiteSpace(newerChecksum))
                // fallback to known information
                return currentInfo;
            //replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksum;
            return currentInfo;
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
