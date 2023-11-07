﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

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
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


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
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.4.2/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "380065bce50a7a3644338ba66bfec1e64b8d8749a739292b4faf145c59063fe4e3c2d3d0a970f474dfd0d90bb1035150487c8ef37f1a454bb2e5705fcd95206a" },
                { "ar", "4385584506f37697e990aeae141bb9b45d0981e0b99ddce849acca8f3199339cb878f8de99e52cb7618bc055984d00d6da45404d1a06c8e00de2f65d1304e5b1" },
                { "ast", "4582dfdc1f789eb9009b8d061133930a3d16813ac1aa2a56b83edfe89fa86befab9da740eb95d79f2d8ac609feae3715f862f051d05c7be6e6d1f8b1e09a9553" },
                { "be", "0c7ff015d4fdb86aeeaac70521b2e11afd755852340b6dff60718c59cd7fe503897cb0b456079a621d59cccf96f8584ca0926d13546969437fbb6e681c48c841" },
                { "bg", "4cfe4a3f9c8429f65e0595e9c1a0a1493ada7cc513a4ed8b03e9b33ba4e7f5f1807be78c4b442511aea517b8116fd98c4c67889c4bba8e4fbd6b77bd66ff2caf" },
                { "br", "5a427004bb3cee49a3adbc5827a1ea4a9f301b158132548dab837e4fa840bf20fd1fb55ffb925ee384e42e1db2378060fda914de758f78bfd5b16d36e71764ef" },
                { "ca", "f110b9e8e8620c50078e08203b51c841a4eba12900ac5c29a270fff9be051cba0af292f55318a1f76128b99383f748491dd6ae2b4d25695213abd3cd308c4df3" },
                { "cak", "339bc2a03b125e6be93284ce6884e0f22ee3a701be63bca78432fb45eb5c4720ab8ccf86218de4976f5b5a3164dfd833290e3623f97ecfa4dc8fd637860a16bb" },
                { "cs", "df553cdb33f2bdad11a1d94764a5d12cb2cd07c73f9c75f7c7af156aa6a63d94531b1b7f8150d37d0b23daa21a8fd5879d62580cfbd3f3b05c48520f229950f9" },
                { "cy", "71ef9a7371c3d83f601298e6f8b4d5cfdd2b2e231e114d1ff1394345d1130e969a761b591bd183c62452693361c7fbb220c6f973a93a8d6ea772ede9d4668d02" },
                { "da", "bf308b618b20d73aff411f8de20b6a3567580149b41cc2d643932c4bf131381400ab29ef1c50335bd18a8215a1ceda3cfa15066a2a865454b66cf805b43d06b5" },
                { "de", "2aad009cf504d5e4d764b488520cfe918de4e6d8bcf9170ea73e70bb940413a66eacd2302a85b85580a1662ae0ff73bec48e1ed917ff63d2e0b61065063a52f9" },
                { "dsb", "b1bdd50ff43bc5f9b0eca2d865076b812da76a26dd9d354ef0a6faef95fa507db482810ac236829b6b023cd6a2470b7d5f3e0abd8d20094df0199b88d0014b03" },
                { "el", "faa7c74a927679feb3ec4fb87bac9c0d98023e593b3b57e4a3dccd64a6aa3780f816c5f48a23be5b7f9f090c76fa044b870ad02ce11d1ace05c00cfd24c63aa9" },
                { "en-CA", "dcf63f74b3508bca22f9c45142ee850fa7eacf14c901ab577946fb1be65273db1eaf14d0f78d04307a03c108f4223b8930adab00ac6368cdee9b72a789478f5a" },
                { "en-GB", "8112ecffb1992effc01b9168716a67f8d1013f04728e593d75e9d3945711d0aed86c3a90d69677a99d174e634333507f98f7b33c7d28893f27b25e3f5a16db7e" },
                { "en-US", "66cbfe1d0b0dddb7c419f417a6307b49abd88ce8b473cf8944b3c39baf322a9e72b888bff2853a4c5e0768a9f30f5671650c767799293052a1b435a21ebcdae9" },
                { "es-AR", "ce15d1ffb5c0979a1d9f3edd1e5ce89811ed4c8e21d448c881ac378907e6baec7e4de23baea159ac8c6164dad409b12ff20a445544e1d3ceb11f9d6cbaaba1e7" },
                { "es-ES", "8a5cba2161f026883f1ebb06cad5ebe6380968a6bfb752ba1ff5c9b77cb0ed64e91c58d30bb5e9b2927a37c6c18b122d4b25f2db8e5ee407f23b1453767ccce3" },
                { "es-MX", "ad6e9e76c877d2dbe41ee6ee6932fd88775c7f82097938e63a57a1339c1d5eb0951396c552c4b17296be99c95ee66b680e1211c403203c6639312c3a018b6e06" },
                { "et", "620195e1384c75218f23b58e733bb773b8bd1e7776759f2b705fda25768a76382528d7d87295523e1f58a6cdc31fa4fb94abaa3933703024e1514175d8a65065" },
                { "eu", "92df8bf29b6ae8899fe2adbe44ed6d48fdbb2aeca2812194f60e26ccc07e5f5dcb40122974c0eac37f31c7e7f8603daf94f1883b31298e9d80c0b76dec2e1df2" },
                { "fi", "3fd0659f518ce0b299b6b473d6f4e8e4ddcdf33b63fad3fa7904bb49b6e4ad8dd37501ac9214b455610d1e3ec83a778e3f998ce61ec31c2bb1e62319daf41cea" },
                { "fr", "dc6f67d6c2e4c01db42d6fd7c040e7fa8ee299d7be8ff793b39ef0d6c10f9d683681fcc22fda50d04bff6eac5ccc61cadb7212ecc716f988f9a5a2f0affee1e6" },
                { "fy-NL", "cdb7634981d293bfed757cb0798b93cc8ab7370ffa9e40e0c819e298e833d5fd26f7478bc6a3986e30d5361818be48e31b6adaec9724d4302e9b1c1842cfe799" },
                { "ga-IE", "d93c663361d2c4a905c19b555fdbc67f5892a362f040224f8ca4d657ebbed3af71706d030d8e0559b955e039e37dd8058f514183d6aa3b27b7abf8e0689bfe00" },
                { "gd", "58a1b08c0ee09d9275611a66cbed663175c28e64cc32c57445acfb9db640d9089073fc2ce60cef9ade9c76ecb1b707a6feb02b194c0ba269297682e55bcff2ba" },
                { "gl", "e36504def0e60ac17b0134c7b26b9747a073e5aa80b80c95c703a849e7b91c421d57d6d0ec8aee6b7c640c0e9c55afaaf8b92fc7bf3a4d04fc1183f1e6860a2b" },
                { "he", "b38d29dadf4141889be82b324aa60388dcf4dd6ef77bea59995625ec07e0f959819d3c04cd52d8dd264ce2e1f933fec038f01809b7b1a9ddb9677c92a2a16fe5" },
                { "hr", "5a66210c005eedf9c7ab3e520a5fc43f261121ad1edcb1fd5b95fbbecff23c11bd43959e8de608fb7b621d5d00f6111dbb325169892cbb50c731eb28d4279d9f" },
                { "hsb", "a932122c63085692b69b3050afe24e42a7cb8d8af7fd061729b8c12ffde662ea68fe0f6e4ce87f62be9c48ada4b5055c8e4601de836283547f0f3019d2baa855" },
                { "hu", "64f8cd99a1bb084def2a43d02f929988c1c7bd57912ff196a6a4e70acf73d216db27d73e85d397dac71effb978ffdd5268fe465911127d58678f2ca9b5777b4a" },
                { "hy-AM", "6f5531af3ddd10cfc67d02f9d98a06c836426a4511736ab61466f06156fe5d52337bd6e7e6ebcd7a914fa20801cb13a4d80ccde27d2ce478dd0a33678d62c20e" },
                { "id", "8de039f27b2bb4bc7b9c7e57f1169ccb630eda351b5a7eada4224fa9988be110baf048297d2732de9ffd490f274392abe1c9fe380d44c7cc368c9c5c0ce527a0" },
                { "is", "a28774f7d699f3be36e6071c45ad4e4a6305311dd768a273da19d32d0b57f3c586ae7608daa894f39da48afb8ed5c1abc4743da78656b05d9297bfb0c89598fc" },
                { "it", "30c7773f5e02b2baf0e3793a5728a031df2e4813b089f8c61bec53f8fb091274eb57248bce7bbbacee542e221e944ea750a44194f730fe0301804acd96356a04" },
                { "ja", "2dba1ee466f0b5185ae4bf63d7b08b2ed755f43730b4ede2046f762bd463183e0a171a29b4b7c0c7d80475d1989e1510fdcfa624c93df48da12791835de2828f" },
                { "ka", "71d13eefa6b25b1b500c5fbbc562925058f58ffae147bf085b76eda5aea24e25e018f45b1165d0d63e2e3719932120b2bdc23636a1afd22723a8c7f28c4513d4" },
                { "kab", "1b76d9d9512b48f0de09f3a5c6544800915f8a0b77d1d38d454e7a9c3759d0c3fd2933b3a16eaf5331f56587061b9a719295e3bfa8d8e9a66f74e77fbc306e7d" },
                { "kk", "05052aa0d9b82e514864cd824db29732d3cd55c07855a51086a1bc823057875f9b3ae1d242ac9afa62ac178dbbe0fadc08aade2ab0c74012de55bd1048e4fafa" },
                { "ko", "594dcb98d33bd43f6c7a75f574d5cb6daa84d6c9d4b3ad183c6a586fbbb8d6d7f0797f450624d85b8f9748fdb7b0d5e0f4800d3554c334d740ec5804b34f1ac1" },
                { "lt", "191fb2264175371303ac65a13de3266bc64d975060a978d615560cfd9ae993da7a688ee93f17b0b77d17b80e79eb94f6004e58ded3fdb2670b93abdcbf80f59a" },
                { "lv", "ce91cf33f3bffe7c5da0f0682acb0207ade1500bee59a81bc632146be58dab23894b21ba060fa7e8ec8ecbc72a6a98f760dc3ff907776960b9355f49e489aa43" },
                { "ms", "528fffedbd5f2832fb67a8359c483faec6094df46ec2bb705df491013cbf43f522d8b9532d2c0139d1a37c24bc935625a3e4c2ce2d101472017ee259f5c6c1a8" },
                { "nb-NO", "d7cda4659cdf29abcda5dbe17ccb71b0ccb0ee27a5ff66b8bfbe31d760f67bb5535ead550324b1cd11722d313487d4693f6e163a6ee2e3b965ace5cfd714c061" },
                { "nl", "c1f32bc2a9ce3660a22afe4c88ff7daa3eb517c24fced91d5306391f12c6d8b6d7b6fea87ea18e71af8492aba10b5360a27bf66f8949854a6cb7528488d928e1" },
                { "nn-NO", "34c50dd389bf82e04d968552f53045fae74e120bd6dea7d11f69397cf74a2e7f1bc0ec0471d61531698f5ff6a8e144b4a2b83493639a5e83ca9856aff9f16553" },
                { "pa-IN", "ac13b2dc0ab340d496fe9355ad084d3f8ec7e24dae1927bcbc82009c10c8ee2fa9b7f5728be57e4ee5aa4b26a8a9021afbedaad64c070e89fded104e15244ca5" },
                { "pl", "bc43f6138c5647501ed9178668771e7b3bae04128f0642b84c7898817b7f0d8f23deeb8d61781f5c6101b90604bd892e286ac220ead1d5058582ee1b5e4fe73f" },
                { "pt-BR", "0123715d6b96f28a1eb32e496420e17f33ecc29de2212f87d2f5ae86b6cefeca46b915f9a9672ff958fa9f1ee3f255fba9ed0e0274134af5da52db4114ca4b20" },
                { "pt-PT", "f4b8e0893c310f0e52a595d263329dd777a70b3a075dceb494edb613da55b72bb78cb51655adf36df7f57551912a1eb8a1d99e7d74e0293ef5265ab95807be9c" },
                { "rm", "b7ad463d52c8e49a0240c79ab9a1a2d8fdd8ddef95cf2d3514eaf39b52f62808d18e81f529f019566563cfb933031b066c980df998bffab313899420ec2a9e26" },
                { "ro", "2f01469efd923209e55bf564a08543a09708e9ccc28635560c7a6ef75ff0996121f4d87f04edb7cbbc4f78e0a7e801d3eff5c15117d3e0edfbceeae013e911a7" },
                { "ru", "1d2052c872e6b4b91f42abc2babab240a373d8d9000f9016a9cdafd5831db4d39b1d023ee95fbb2cfe647e68dcda56dad83d6c1b5e87c1ea42bfb26465bd2675" },
                { "sk", "ccd6874784240c6711a066c7c4e84d4236d17a528a8b5a388e9864a1203349445c38e156f1c5e056509a846f1df2dc360f9a4278e3dbb3127c037bdd23bfc8a0" },
                { "sl", "6cd02c7f94885fd618eacd7a097c4bdc94a5ae318440849b19f328b1d786691d529d2fe9f5d0e62153b0289eee966505eb40d172af894a1b4fccfdfebe36fdfe" },
                { "sq", "5521fa0cf99b74b668ff0f6e984dba1676f89191b10929a8fa0a6cbc462df743cd892de7825ac5c1bbc2441519a2eb002ccf790ef023b9730357dd29b7c2c465" },
                { "sr", "5da3a7aa68adfcdf03df859419d4633017c587893e25dc437f3a86c6a25ded000de84f0a77b97c37ca8a2bd005cc0dd7a8e549074b8961e640c37c5235acce7a" },
                { "sv-SE", "9a8741a9ba215dbadd962a6f821ed61095054f39b37554be01ca7e6c29130f43d913279c0c8826abafa7f2e4b2ff5df018ee9b9c8cb5d4818b4e36538395843c" },
                { "th", "e3a381d09be9eba7e60d0b10bdcf832cce30991140e7dc804aa8ddde8c5ba2dfcdd53978524ca5d9cb3294bdb4d82e2942f883261f6e9166ed686660c7f8d206" },
                { "tr", "59cd852db56773e46bf1a0a96ce208c7ddcaa2e66929b638abe706cf41c3b5538dabedbe9b1fc454dfd7a33aece8269fbfb35823577171e840780131492047b3" },
                { "uk", "b270bdd5b0c233b71e89c9946022405e2c7ad0f7a4e3bc30edd2c7a60d43ef5bf1b349a3a6318cab7e05b37549e99ebf5e313434254e5e05837a8216efe292a8" },
                { "uz", "402f456c29be30a06f944ac892e1fdef9367f4876e4847cf28e00825cded6db5aace19c95814992c879e774cdd5acb37361190f8f7219fc2e0319afdf7a187d1" },
                { "vi", "17f0b37b6af0b99981b3ac731fc7ad0b95eb6b6f6de48c7638d74a462c91fa6d142038ff87daa396ee667a517b43568ec2122f95eab35608d713ceaf1114e7e6" },
                { "zh-CN", "429915760eb5606b1332d8a68e71db3389bbb781558316c30bedd608ab6d8f307b13d2bfef8a36c39ae09b506aeb8e4690a4c6fcafb287b2538ae8a1b564276a" },
                { "zh-TW", "a6cccb1a2efea03a4af8dfc4a92130c377f65bf49bff543546e7d914575522990358db03fa0b487bf26ea2c7e284a07459569211e604cda33b83d55727350801" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.4.2/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "df97220ae4ad6ab5d575bc426dbeeb51fef09fb36e32ba7645d43a73ea57e89b0f2c0a12fd2a30d911546fe68b527adf731ab771e20d0cd6338fb9380a2fb711" },
                { "ar", "209d919e30ae7855137d97efb154f2b1913c4ea7773ae978d7566d50ffb5bf0c4618e76dab5af37db4db3c45b6b3628ae51bc1e5649000cbb1703b2b14c7e9f2" },
                { "ast", "69334cb75c73d0434859b635312badca112fd6534a4b4a0ebc0dac591fd2bc9c2eb91e8b3a91b38afbe42fd0c92e4735db6ef1a3459c5a2f00f2b2944cb16a92" },
                { "be", "e572e9117888afb32a7a141994950d126714e07dea9e5b39b771b5684a8f6c848803c2654cdc8d888557ba8664da325665b4a9bf38066d5f6d7073187f179a99" },
                { "bg", "c83ce8b3aafae1c2cda73b50dcdcb62f535d08e16bc6854881fef8316b802adb084aedab9f0c35ef2f413b1fe055ddf526d82139258315417aa3b24d11d1823d" },
                { "br", "1ec61d8d636fa95ce1a87c98d7f3a75632e7d1fcabc3bbcf32d25cfd6cf1961bdee5c5f9143b4694812d19d660f421add1febbc2bec9ae4991efdabf23c578e6" },
                { "ca", "24d1a6f2e3a1134c124ef3dcdc1604eeb10bbd921f360ea50fc32da3fdd3229822e2112991836dd33657e0fe60c180e14164a5d623261bdf3b0b228573a8f181" },
                { "cak", "2497548ee00fa2f4b2ecb2efc3e4230af6325621a237ea5acebeda49188ff5b2feb6ceaf227c20331f152f82a278791ab8f870209e93793aa231a5a80fe93860" },
                { "cs", "59a7ac75125daf440b106366492d04a9d0f86e22d1c1ca9537f49b1568c8acfe07898b8f1b8f9896f0ac6d173571a1ce27307b411b81a9655385ba0f644a9e64" },
                { "cy", "b94feb5227efdaaaf5746f6d91c9ebc89d88b2f3916adff72fc9664598976d28d15295a78ec372387b5aad947cd0d1310da25d892f75da9635d71668891f1c86" },
                { "da", "84f2a1e97e1a947ba4f6c9f23f5b0b8f49f40f5e134d80639f7de3bfd807f575c487ae1554ea22c7a1468e7cc2c8ba75bba8b244e344c9312fce1fa05f03bc4f" },
                { "de", "1437603493513bf4efcbdef25250b8caed3092ad5c11dc1b388981a4c42c7b0617abcf22ac8d36c50360cfed432bdf24fbdf3f93e0b09f404c89baede10988b7" },
                { "dsb", "861d3ffe3dea0f37033379b0d55e378bd175c07cccf914cf9dc336eb4cc1f77ac319852a2847a249e643f6b568ffcbe35c082ddeaa29a8a3e0ea64b1e5597f83" },
                { "el", "dd1506045e83c2d9c0a31a1f119c3d32f59d43e6343024a9bb8b3b6d2dd2ec4e944be7ecf52337fb5c036be2e86d5b3a25daa5bdd8d070086c7ca845bc15bd5e" },
                { "en-CA", "51919704cdc576a19fcd4ea6362add7f19e95ffd78d28da82f5c7d50b094b85767b0b20fa9dbf805d05562382d22efec48556e6cc8fcbc3f0ed0cd1b97a6a416" },
                { "en-GB", "cad5df64802c35a4a510bce4415c949acebda35a62338918b8d3f39ea5134c5c1aa66ab1ab9a97d150b0c1f1796646b3488ca9cab4f82c934f85d84e790e741e" },
                { "en-US", "866ea92515a842f8841d396b1363f020a13d4c9e376995ea6283ef5b5d17f09a852b89ac713d14cda38f60a1d6a69c59c3d61e79db10201948fd73fbb554a641" },
                { "es-AR", "05f8d60e4e231fa691203d8e59e3fde9affabc628d540fee5cc4fd0587046743338aa043d0c3a926c4389e92ea4fc60ac7db0503fdd2e5805e377fd025828aa5" },
                { "es-ES", "1b30e4f31a4df56e21d068b86c4e1413a1de1e4ed1c5adc574172cfedfafc29a1137c497bc222774639873e7ac69874036d57ea799d707a4272e420df3e09a51" },
                { "es-MX", "464207216d5920f5fef13bee562d4979a6bca68b8659b6bfda490b228b80f75299fe9391121fa782d6dfb52675b4a674e9dc7cf568cbe3d6a3325bbfe21d4c1f" },
                { "et", "c55430129aec52a0416ec5816910e3052ee3a3a9c2609518bc64dd0aeaf0d69886fd7ad849d93fd6634238340cd9431da40c6636fc817ecb31a23d7dae4745e8" },
                { "eu", "c1ecd58a07a637ee1e6abf370379efa0dfddf9f5fe2930ae8d2e1c6dc2a3df69d1a98764206da0b3d867162a2011f0ad2383cd39cc0945c4daa8e852a9e9ea2a" },
                { "fi", "41d23bd2c6ac194de28025f200a5706cd1e9d5d06888cf6c435806da13fe4dfe8684316ea16bf6ba81b4b6f3c98608cf9668ed6147998aaf1241cb9883afe8f5" },
                { "fr", "af974f74680517e8cefbd5bcc9907c6ff5853779d8c74e41f3c0593995a48f7f6a67d46353c328ae5c20b5c92962313e52566597b2859dcacd5d9f227eec9b7f" },
                { "fy-NL", "073861598b987dbb4c81d209a3c5385bbe9ee9bc0f9bee8d81c7a4a2395ccd37aa678a007ac97d271b1c904d6df66dbc1043fa404a725231f7212dd0628006c6" },
                { "ga-IE", "5d49929bdafff23f43a1b7ccbb805741c4aedee7c2eac4c7b0d0e0d56305bc11ae43c74088936e9356acdd8a90909266fe89432a8304eda195dd19b9f18b302d" },
                { "gd", "6396aa2edc26637d69b7af5b872d904cb68b072e85297f0e6e9a8b42caacc4c88c712d3494f5e7714eb1a3d958444f9e1e011a3145c3bd58c3609a1a5fb69504" },
                { "gl", "a6314d151537208271f15d1c414b2d5b2c5641258d98e3abd9210a8e0b83cdac619a624b910f1f9b76186d54ae014673bab93cb9071237a991c11fd625cce30c" },
                { "he", "a9a12835a05ad066d218e526c85e03aa6764606e460ab45accd53fa61f3266377c0c44f9d3dcb005fa05ad635fb099bd37119323fd272c21281e0eb57c6982ad" },
                { "hr", "802e59cf3261a57b03648bbd14f6e78a39d3614947420bdd2e82a9267eb801e904d4564d4fde79e22ea7e4eb1211f92fab68e1009b0452d7d58fda9ff1523678" },
                { "hsb", "fee700e3895993f441018452d1b0d9176350a4cc40bbaec90ba547e09c270343eb941147881933a320330b73815fc7b3712e600d5e11e98a695f322c4c5f2358" },
                { "hu", "89e79be44d38f76ef9e6cae755281377c7d2273d9c46f41d27dc1f753185b9c3d4c0dd33bd5e6631ddc17ce38b9136b8bef972fe80a397dd421ee2816a87d6b7" },
                { "hy-AM", "5559077634854cb1b40c36a936bc39647482998b8349b0bff5c6181bd3d560b81aa51826e61f6612943d63414ca6afe92b4c9f7596b60a251201e2568f2437aa" },
                { "id", "f3d609e38fcb22e1ebca972c188fef84afb62708528c8aef7b15f85602ca88a59d126c2e8c7e1efb41f392507fe6dbf7cc4a67c64de8c42135bb6340c127326b" },
                { "is", "fdf40e4c79bd5c9a4ab5e873970260bdf3d3ac468d861c802b948d2e4df337a488cbbb82ed5822d6aa0a2727df4e18cf2d49286a5489d1fb40eb8d4ad70cf435" },
                { "it", "f5825ad665cdf00c030c5d5c68180785c74e02541b5f8ae094c3bbd2fc9391467be9954741a1b799fbfccda3736ffc7f77c686de2bd28ba270bbe48929e50ebd" },
                { "ja", "6243ca497fe1ba44385e5bcb212557b89a0787d2b69a783275d6d93cf45ef59df6a623e2d49d10870ce357af96f1ec8b74e439fac81c443fc68aee4dab0d65b7" },
                { "ka", "e43d58a2c63ed60046caf4a99d374bfd5b568eb1739a0b44831af2a41decd1a21392a073a64acbfdffdace845b44c61ba0b8702526be68c77b22e8addbc3987d" },
                { "kab", "a7fafb6a25be250f6584a63fbf313c4450b5b169d97b541d21fd238748d82fb6b00066dc94ec23d7a23cb39d6da5b8ea00137fb75383dcddbf7512c2ae80f21f" },
                { "kk", "e667a52363a2a570014c96dd3830fed08bd542c29413fba6c01ae3f0847470354e59a40973747188bc7b75454beb0f970a43b9136d949c53162524a3e07ddccf" },
                { "ko", "c1efc72ddbcba4c21b0d1ed119141627fbe54e3a57c3a5750b7995694abaebd212759779016538cfbda43bb0885c79f23e952d2fdc28bdb8041a1fd06e68bb39" },
                { "lt", "e55905245172caa5e457237e8f20498e0336d35d6d885f96c0d71e30d4f40fbadb131f2419e151a51dc0d9151e28a0fb9fe04121737406a79db64f4b15759326" },
                { "lv", "7674bee29b54bd28f4062a9dd9ebe2e1acba698aeaeaf057ca44fda5be297c0c962cf62ba86f105dfbdfb1d396ecf308289ffcb296b19afd3902ade742c013f2" },
                { "ms", "5cb40c614f83ed4765018aa66cb02c845fe1758c3816268f7e9f10762d5a931c56659eb4724494e125e26b391a04cbdc417ad945956682458a1f35d359f934d6" },
                { "nb-NO", "75ded8bb7009685570a21f9ced4b4d45220d93f7be77dfae8cddab74def639649d30f3fe6e80d0e8c3560d9fd8dd23361c7049e6e7ad0c56945e5ab6245afac9" },
                { "nl", "bda82195653eaff8fc626e3d41f055b1286b2c671360f25428502e0f9cde3939887d718c2ab86d0334681ef8d2b3626bef6458ff88f3d638ec0578958e139ae6" },
                { "nn-NO", "5830d5952f78dda0d304c78107b412cd89939b7525e5169cb711f870b7c8901d337f3eea14e8bd3d395222cbbcb536d202cf078e182b387e859e9940f3008a70" },
                { "pa-IN", "efd325ab60b5da5d6f57ac27bb8f33c45c80e29d90caafe7ca0e312ef918fbbb601b0e0aeba824b1fb8f017be82c60bcd8ffd81de26c561447c62d2ad2b5d26c" },
                { "pl", "d061c3baa33ffb15e5f93e9f7ab9beb560a213c62cd32cf9e2eb161d070a67a9ee558a19033028094ce8ad69c9a340705bc90e0dfd54ca63f6ca3b896553e301" },
                { "pt-BR", "b5224b31e8ffd116b2ab90e90ae3dc14ea4ae596e515764d27b6f031be4912ba265b2a86ba727370c03f0e90f852e7828268ce43e67b86a77550580746f8bc69" },
                { "pt-PT", "a9383b6c3ac23589cffc2099da702ed8b873d07fbb6c7607fc82d31a9d6b47ddec5c7aabeccd9b81211a4b735ab33422de8a3e85413bcf02bcc07c57e3172d96" },
                { "rm", "8271168a4702f7d0cd79ef1521bb0936ba2a77811c1ac00a890e1f9f1830c7b43aa02d6bfbf1850b4cf9313ad094bfceff86b3e2360afa8a9ddd1f27237007bc" },
                { "ro", "fddb4eceb4bfdd56a32cb958b038e60b49c83d47a88b5f9318ea78629b9a7dc723f9d7a701bd884ab5e25af25043b77e469b3be9460bc6c65964062ab9c612f5" },
                { "ru", "13495ef6723770d3c43756366841780ab17bac9a03bc1b31879f77dde0f3e3c4f1857d1d99a7dc0c7c18bf6087da47e4b213970a8ae8c5f738f31581d19951c6" },
                { "sk", "4a3ebb8d2764dc9cbb0ba1a0ad550530a754d886da9c4e6cd42ace87eea697224ad4ba2c8c75265d65e9a233c128ceae87de7b951faafe038a2423db7a2a76e9" },
                { "sl", "94b9f08fcd808d835f1855ce5201fee927b7a87f35ac81a1a0d0cee93dac7f253d0ade29bb4838560b81a04cba848d6c6a37b9db8e2d942be69760b05ac15b65" },
                { "sq", "a39173ea1d2992b653c2f7ac5fe98e14e8c5d3cb617e7b28c627f8b9d3a8e795e084d5d516b8e4786cea62b87db61d5c6ed64c694d6042717e7c6925e9f3b2af" },
                { "sr", "1d710259c3f31c84233eb9ca26a6bcfd56024759d6d664158eccf29afc83782386270752047e860d56d893951f8a167a17f0d0e94203757d53b4fc1fbca1e12f" },
                { "sv-SE", "54ef1b29d9f5b81226121cca4d3d802bd6a24b30e625dbd36b33f225331dc7b9c6535d8b19d07fb30b080cceafa2d2110b17bd64f94e585c98185fb919523be3" },
                { "th", "364448865964cdd623380045a55476b6b586ef2dc312dcbf78475dc40e10ad3fa19a337268b18541a6399b0d8866caee4be815904daf0d98ff8720efa8d815ed" },
                { "tr", "c6ae427764883f6ff909f209bccabd1456f2e88b5504211b5028e53ad4398abcad53d1c6ff49319570e71f30bec9abf8d90a150a5d669236965ded0c54c10724" },
                { "uk", "27573161e9892dbca73bca7a13b3ff01e21bcea6a6af48c9a3fe9a8a8e19edfdbec50bc8e6a3f4977e000667cb4f47be1d47d90c0ce015f0a6bf151588eb25ed" },
                { "uz", "1162059565c2d313dd42509fad5ea878c797b1000ef8da87f40d2b189215ebe1ef18a0eb8074b26c6e8ae9cf8dfd6a7ab673d5a0341fc01845111d3bd8ef33f6" },
                { "vi", "86c77cbdb405c1bf47ac14490dd6b0de3772a762544760858382fcab657a0db8a1869f285f9385122739d21879875a09f8f87851ede3674826a50db0b5969568" },
                { "zh-CN", "17fcaf3cd7b047eb39fb7ddfa0b6c9ec143ab48bc6cfe18173b42ef75ee8feab2af7ac9362306c69cedf7f0953a3dcea811599a02311027c6f1a15a9ff147810" },
                { "zh-TW", "cb566c7859d109d61162c1ccb67f1f9245c1438826c053ca2cf7d9db3685df19c4a73fce143a312be9b19decfbcf2bee92fdc0db9e4332747c24e110ecd9fe8d" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "115.4.2";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers.Location?.ToString();
                response = null;
                task = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
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
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                return null;
            }
            // look for line with the correct language code and version
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value[..128],
                matchChecksum64Bit.Value[..128]
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
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
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
