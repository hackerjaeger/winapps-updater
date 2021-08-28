﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021  Dirk Stolle

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
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "92.0b9";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/92.0b9/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "10de332164523c04f9e2d4078c88d0add9089c8292796df10ce044b00d0575b09038683b4e115258328ef0ab1cc4a3170f35a80129e2daad9f2330d5792b7a9c" },
                { "af", "9ce316a7f5d8f2666fb4323c9873f3b418e76a775878ea72b5db7de562d0e2d49b8fb282708cbdbe6d9dce2583ec58b839326a8943c3d1dcf7590dccea45280e" },
                { "an", "52907ee71ee1054f86ffb32bf8922001b21a5d7bfa9a761cda5316ec2d818160d587f9b6d8d87f8a813ce2b74278985d1daac50517377054a69222c9f3105c1a" },
                { "ar", "de54c45758549956b63ee9b27e56191f1828c0c46c293489231733344da505c38184cbcd3d22e098703c3ae3cda8016f75de8c5184dbce6143444a8c99d9a5d3" },
                { "ast", "27b8306f0a8e6be1acbec9485f92bfcabe38a43a449911bc9f6dc14840c738fb23e2a0331c978a0354e21f6af51cc17b3341fa90c9d46998c07e85384510ab90" },
                { "az", "6e5318cf21320f30f49e53c9d02a87340daf3261e68fb27b5b92305699ec4a0c52110a8d775eeb4b450cf040819c79a11faa551950dce526e4d963d36f44f68d" },
                { "be", "815a6cb5c06693fa7fd4290c40bf9ec829c8d943557645ebc7159761510e46d58c60aff7f74583076495a0a8296977ec48a4f986eac2adfbe0bec33fc7bb8e78" },
                { "bg", "39374dfa90a6b25976ae43701852779ccee89c602a81619fffbcb7f2c086be9c7caf1bc7b9a87b61db8727ba12c91a9d21657f5a993bc302421135260260c8c7" },
                { "bn", "8d6afc1269959bf7d640bf4ef8039cdc97244f920877c9c38b14f1c658f80b9c7d9adb591a96c63ba1e9a72bf002b3fa1da9dddbaac42b8c79eeb7095088dc4b" },
                { "br", "0f92609063a17fa7600e43d083862dcdebc044031604df9813ff0322a9b789895919c47bfb0474c91d64a170d9de2e89387e258e57294e5f13f3ff7f717f1386" },
                { "bs", "c0ad9c9606ef344b40cd78b2be7e89f9b8ce2688987f7f28d31d46cb0f4f00608314051c211f8a8ed569e9342eaafaf8212ca7c64c9dadee3ded67e6afccdba2" },
                { "ca", "4d757d8400ea8196158c9eef76854b9093e59a4e6e25ccd7ccd7931122850bdcd93ef7bf9fa94e0d3cac6ecc612fa9608171c5e7b1a26fba7c86a559560b1c12" },
                { "cak", "d8959c49ed613255efc965248a3686646b63f98c673fd0ff4e66c5d644675778c836889d99a8039e17246ba52689e63690a33a0d705f79e0a6a5d042081ef343" },
                { "cs", "2a3cd0375d116f36e05074a7073eb37c33652dee1cfa2c33758e5bfcaed3ce4fe6486c24e3fdb68ab9c1b03f860f21a30d7c6ad6a55a84e0c398895c96eee009" },
                { "cy", "615c709ae685130f2a0a145472aa6905d9ba3a18289928b77787476a1b8028e801f66a3f7bc3777b109d069a435a3d6da2c3faa6a060a2f28b6c7d6bd5ab4011" },
                { "da", "6fbc3c3ebd87c420ba16ae2c861bab871e9ae39e78ac7166e29d2cee354f94fb4c363c909abc9843db39eaed95ea2e1e92efd59cbf5468201e55b0d76c97527f" },
                { "de", "f0ddcde4be19980ebe2c8c578f548e1aaa62076c3671f1f6c35d7d5a68d6b36f12d420527933cb88c751e412ebbf46d36fc424a31f559a25ddbb72f7eccca2c9" },
                { "dsb", "bb6bdee33f8dda7447c51c7c2f6179e267f5900b52a7c9bc238e8877afb55af589bbf1e7b9b1ea8597af04cf93f1282bb4fbd7d51caac322b8d09101c31b29b0" },
                { "el", "481e830a3a7b4bc735d4e642a595fc5bcb0bb739d4ac6c7c24c7fd65b4394c80ec4e5c2ab1d2a58c7101e231738e103ab38fb970c5605f0890f1afc481b37a2a" },
                { "en-CA", "99aea4f3d7ce6de7b84db9280fc7d0dcc564f746bdb467e28bce44a068ccb619f4b5c95fb58a040a6e74444ab0d2a04cdcb00deabe3592861d0e6bc7854c3cb8" },
                { "en-GB", "a1b4bf35e5e414d6ac0407d51167126623d23cb17ca7f78bd7e5f6582e20b83b5a7d952cf3dfefd1a16127d42b1b2e0688aa3b55b90bfe5ec80979fa6f524bba" },
                { "en-US", "dc82d205d83bb6db51e697ab06b6ff2cb9f0188c21b76b7840d622388627e0bdc3ab90f7a0ad1cf3f7fc18006f4a62f6b63144c6546535d830f6402015421f6a" },
                { "eo", "77a9439f402eb883038ec94a2118c9d9d8f1000d21cfd2f4d94ef16d49337b4c784ea0c469062c6099b0ce2113e02db1693edb70b8798e00eafd3ff4b2a89b22" },
                { "es-AR", "ed06f6c8402c8deee121cae10bcc5f7bbbb2f3b6c42bace5a22c44c6bfd4af6e34cfdebbe5cb249978ae90cc5dbf0afe88b8952340bcabb5a905dd7b4d9d9490" },
                { "es-CL", "4fc697a3fd367a36159fa00d2803fa9619c65e8a97870b399693db8cc58fb31282a5bcb0c4bf36b4d0c7a6e1e4b5fbcf71d1255b566d53ca79025a1f8455d2bd" },
                { "es-ES", "bdc64d76a1a48ba8e024483da4486b05dcb75fcf13467ab2371632838341865bdeff19924443d37fb979dbfea73b1d32c420a16a85d1dd264d3ec95146d14164" },
                { "es-MX", "94082edbbf7b3ebe684ff81ae1fbc5df5a6f70356533cd9cb8db8b1b4649fb8f1660997d8177f95b52a240e9114bb90beb9494b0fd4b053468a65caab2860d62" },
                { "et", "3aaaf806c7a9b09a997c38f7efd1d02a3ba563cf629843b396fc68293a2fea7dcc1c96314b198a84fc609cf23d30814505054bd32ffc48870fe69bc0397b2bae" },
                { "eu", "d1fe34993c572012f32086a56d9e37cbcd714352ff282614ccf158e84a12647d255ff00f4e0ce4b1b45cf5359895b50ed03766f16988731b7db9f1cc604f5e55" },
                { "fa", "0be04211e8598f76e195904041b728d8f64c9694adf6f1bd385c9ae169ddf6b62806e88c98c34051baabbaab18390d589b77c6fd938a22f19ab583b2fb497891" },
                { "ff", "51fc2050e4833d2e4f6b30e069f21c0f1547089678d9080a320c640cb60bfad6ad94aa138cb4d2623a234a4191b1c2c58552b76b4b8edbd977bd2ac085fd5584" },
                { "fi", "19233e65256fbaf791b56d1658fb28ed2880b348f5d6b6906995bca524ae3548545a34cedaf9ed528e3779b6322cc9bac488dc1bd12836e7dd0152a97478831f" },
                { "fr", "bb8d421c914afb8a81dc4c7b48d5907225a244ccf1a84edf252a09d16c5eb4b56129aa84c3ba24630f7aee02935a05cc8db38f87c551575114120c9c2688ff04" },
                { "fy-NL", "60f8b32c4ab5b46c004799bfacf4c7cbe2140fc9ebff86df3be25d48b6eba10e90ee97491b9104ead9b57aa5e2af35866a1216d63c824aa5142b64f7bb94795b" },
                { "ga-IE", "bb077703d3ad693a9400b9dfd783d00e3350a8c66c672fb9bd35e6918db3ea767a4ef0bbfef22c090ac4a7b498ecd7c1e0addd9be2c5486ebd948ff8b94c8c3f" },
                { "gd", "8647786b90c54a432bd0cc184962dc94c8c572671a2f620c50c655131479cf1dd9ee367d47491d22b5913c2f36524bb66c948a86d7ddf612c0dd7d332c236d72" },
                { "gl", "3af54967ae663397a130c1b8ac321f865b7a86d1e5fd0837d8e114034df5f4105fa75a83877a9365aedd0734b1c31abc2b0beb2153d703b59b17cd936dd3c2f5" },
                { "gn", "899ea108b75f83f1f3d8d9e7af338fdc788d9aec3b46e95f36a9352a4a4f12d04b28eff15c365ece57572d531c1e240ac121c4552f5eb1b301f65fc91686e420" },
                { "gu-IN", "5be414cc28cd83de24fad15c36d63270a4794874b3b987ae8a1bcf730d01ed72028d15a3cc75f6cee614fb258123768eb205438a6a1a4754ee373ef26744b603" },
                { "he", "4b031ee2cd95e59bb7899d70cefbce40b845e454778e422021666e224b186c227d531152f561fac1e57eff35e1bd00a571d36b6c8fe803fc5adc85a0d2cda4a9" },
                { "hi-IN", "d4656503d0354e732e9c7768f2430d8118b2af36c6c57966b5f86c58a94baf64823951988d692ba1923119ee8c5dbcc078fc4b6ed514c64342f7b79d135773c3" },
                { "hr", "fcbae7b93f98c435ae76904c52307a9141b78c9d0c2f28d17981d7daba906c3e8d0113ec6b611223851a732eb54523fc2d3cfdfc417aee8bde3b153c31fe54d4" },
                { "hsb", "334cd57213ea7a4ff7c7c563fa6362fc240a0d98eb3da88fd3757f50552ffa364eca9266e2064cbf96f22206caf469b1c1af81abbf5a545b8b325afc3fba504b" },
                { "hu", "1b9f7be1c36dde82cf5cc3e00aa3b3222ed6b18062399f863ac9881f58fd3c91b28fa13fbc5c69174f76ac887e0ecd07b79acb028ce88d4c8b93eee7b7ade479" },
                { "hy-AM", "37ee3abca12b1789cd18f21b45556f586a38ee011a72243a8ec958957dccb38c250d452c39335aa6e6457fc82c55767c513726b0093a5496f944142d8e2b2045" },
                { "ia", "7ebe2edbb4f0b33857e6095c5862ac8c5ef846dff599dde000d0a668f091f3de4c4cb04db7e8e8100b23b134515d9da53ab6dae5bd399026f30dd57322c1149e" },
                { "id", "cbfebb8332be41e4c9c46173c8ae8afa359b82a1018cc0a1eaccba816ca5ef0e93f09a37d98e416fb58eab00b2b72f687df657649850cc866464f2490722d0e0" },
                { "is", "194df806c08c48b8cd39fc3b17dad2eb6ae74310f27544c15248620ac7d8bfd9f9b327059be5e8ee56f6e63fe1d34535d63f408fb48301730859dfc8ebc5d59b" },
                { "it", "f6fc09947a7d529cd1ab1896556f0718b294a0ec0065e5969be0c6980a92b0857f2da0525a20a17690287a690b58e671326b96929c73e894bbbabd3422f454af" },
                { "ja", "ce855cb94cc8590a68dc6ba18d3328031c927e3dd3e87b39aca05ec2d2135aae501ed7b623f4470a520f8cfda065fc38760b54136581ce6c54e16aa9cf91888a" },
                { "ka", "0507eb7b16020d80ee3ed2403f06dd836fcc6ae129e3ec7550b7e2190bc994226bc81ae93f2ba251ada19eb3376b354710bf1125a41b8db80c4f7107f862befe" },
                { "kab", "36c8e259d55a79c592464ed08d9e45254333228b0a4179b2e65999fed8364183d2cd1b01718629cd526be89762f0145c6db7c03ce248158d16788970aa58b231" },
                { "kk", "3d9882d16202321c4342ce2200384d501d419d78545067298d84175dad5c49ed86cf4fb92e15b9aa0bea2662c3a480f903b880c59f1aef91135174b5ac559239" },
                { "km", "6ed6741ce7e6a1ea7600647fd49d1990ec84eedf2b0f97482b52a7ee5f4dfb62f725b8efd2eb07005c61ce4ea717faced3d1389afa772fec3b933fde6976738d" },
                { "kn", "4911272898965c8b548590604a3b7da020defaf83fea1d0c3a2579aaed54e4dcb9c1eef69190eab0c2d1ae31412475609d568d534d5944128395aab055a3adde" },
                { "ko", "438227b21c084c03045c38d400f84249827d47ccaad4fcbde6383b26c716c52e2d43b8a6b7affcea284ba6940de52d79ff2cb1bcf06e73527fce4bcbaf17099f" },
                { "lij", "fbd23fae1207933e27d4087bf83268825d4458718edc229c3539fee972b4fa2f00e77a6c940dedd400dd24ea9e1e8c2589ffc737b8dd1e770d6b9832be6b3946" },
                { "lt", "5eb364d9d05ebac2277b79a9d4b969b0bda1725ce92366e740b0837b97a724452033621ccdf695d21a7e55e5aa0cea3e8761bdcd827a2f01f375d69153bcdf8b" },
                { "lv", "2e63eb6eeb4ee4dec62537ecb774916326ec2e98bdfec1c7f839c3ffbabab5f59f253739edc299456c0f69ee116dd43531e4bc1b101d3ff545b6fa367e77ef56" },
                { "mk", "ae11f2e45c8a4356d5514f11bc5122d6097d74c2c7c1c8e00779841d5cc48e7c43ad47fa4625e16d321ef6c01618855ea9c487fc1f7853fe622f9a640432a10d" },
                { "mr", "8a5fc7963d101389699c58bdbc5d9ab0aae485057f65136adec4554df63ae6ecfd1d8ea411e39ee2254b761924f19ef188e26efb00e563201583b519e0a0533f" },
                { "ms", "0e3a5635c34bf553f778f7b8663db06358ddf529d44746469f2f9a362578c0c60716d4e8413f251a4e4c211ca7e84e8a3e3a88cac8bb0c74cb34c7afe74041a1" },
                { "my", "a690b7c2725d0a768b5cf17dfc050ab47dc6deb2cffeb8f640eba6f9384213b83696465361330055dc638cb5f5d1b3d5bc8aa1be7213feaeddec76c2d4066485" },
                { "nb-NO", "b2d7fd29f1833a9186a338086b2babf837287c751801adc9c18db6eb20df504381f2adc40cee963bc6ae4bb3cdf6399d9b95e2610699f7f2cf7a3fd8952d8b1e" },
                { "ne-NP", "5f42b215d29fb5690f423a24cf427cbc060b48617a50f491982fa06f24fb6bb75543cdb53ba0817c52b98fd768b0285082cb064ab03c53de06fe5ca66d13f65a" },
                { "nl", "24e1b7eea828cb113c969f66ddbf78389704ca85be5f2452a95978f058edebf55b13918401de8ffa3ad7d340d17708f16fd862a16ef2ad94c511b6958ccbac6b" },
                { "nn-NO", "26209f4f7e1c9066f73a803e8828fde16d71438587ba90f82f6af290397ac3526e5ae3e8499d273eafe90c50eb99cc3bd30a5aec85ae6fe1d8f0b28ecc5618c5" },
                { "oc", "0fbe6122ef0dc67ec3312337791486d656bde7fb638eff655ffdeff3072affdaa9e0fae1953112677d92f6390aff44818a6713b4a0083f8c91e2b6e8797b1552" },
                { "pa-IN", "9ce363018c0b11818e397142775c5e3d02a73d90eeec028f49309624e85a5a683cc58db74f0b845cf8bd251f7e0ffd30bf8c74c3223affc433af0d11e38f28e5" },
                { "pl", "7fa0bb6d1f736eac847d351e80cdaf8590c85782e46a2ea1c32ef68afaaf5b13993679cb8db7bd9aa6d0009a23cc8a8a5f14114a0db12c68ec714e05ae44cea7" },
                { "pt-BR", "9bd6205908b1fce3f3d9e8f536f745f68c8f631acb5e1ecc343edb58e57a26a5a267b6bac28795dc65d91d9c2480c2e41be07a819be73f20d1b37d84d0286c59" },
                { "pt-PT", "8181540279fddc74d3fa372b549c75fcdd0b3898e162e07c0e1728b57b5b48b874e207786b9b0327b2290d6e4ef2f7be9a3a2dd27af548ffe46f8ae9c259bba9" },
                { "rm", "dbc11b329c557df4e430815073b096699a3974f9761984a568c151ecb366a2d4b6a782ada53f3d0a86fcf0fdd3ce499981b3956b8784833123af0470939ad7b3" },
                { "ro", "4ce9d1ead87b883a64e9faa95aa1381aa934ff6b13e91d5634ae723bce6ac38647aacadb9cb35c15e97042705fcc6c4139f779a669f7f1507347a3df825d3ad5" },
                { "ru", "4c415605055b971306c09c90a1f42cad2eead35b762cb3d0fb976a3ebbed0690d4c6ad0bc6e791f7765237434b5d617c8d6ec518c2485a95965f88cb4e842e1e" },
                { "sco", "d3da1556c37c26a6bd2b865fd6a8e6ee980779442b65a6a76e2942cac7cb164dc0d2b1d25b6d788dcf59531619a58a34647c2920901cbe6ce4a9d45b77770ae9" },
                { "si", "11cd2a7d745761fe8195a39290c2288d62f2dd5a931e5dd12fe442276b84e87b1171e00032f25cc5c4fd52566dbea84725f78c0aaa9b57d17ddca40a5e6f3852" },
                { "sk", "ef018c3ebf1a7afdbf9c1c87c2bb4028e73f0b9f89f0c68fc67883b56d2a04c5aba4939b5780439a33f0e0bab71b4454d9140266974693d77d8f4cef89c87baf" },
                { "sl", "f517ac390d76863fba706c72f38d36740e0bea7899c4e19f9e201ac0428a1d41569865a97b91911a810193d229c4b4d16b50b21ca56d825d622e4b6a64cb5708" },
                { "son", "08c79ed8b60bb42ebe5920236a20bfe148d169bced1ab7a021a07cf873fd2a040d11ce9abe345721c79e8bd0e7dc890c4699a56339ffee66fb5dd3943c914923" },
                { "sq", "2b1f0b639eac238d70b967429ff64e114ed57f3e8e8fd8d91497d40a1669dfbf8b24f7ddfdf84fdce6d98457fc3446c52659ed615f9e8e9dcacb872bb6844a33" },
                { "sr", "72d9272aef9cd8cf96a6e6eadc99154bbbca4a1e8a9ff1c7ba3bf2d3fb42eb8c4b47261982077bb980ef8a413b81da745bfb8e53b26c90974d681e208a8b5ad8" },
                { "sv-SE", "4049d180a743f38851e4868f771ad0f777580d70055a990cf2f9a6586ffd85e90253d0b0d9ba152bc2311d847d8bdd8fe0efa0f3f791f4b0bdd6d6baae3fb1f9" },
                { "szl", "411ffe57fdc7f8f43cdb2076cc210dda3c5061c3b65510f63cc6acfeafca74991f8dd248cf1cd2cd4896854bf8829f286a469aa2d88adecd7486ad1d76f9784e" },
                { "ta", "4cc0e3b91cc3ed0f39555da23c27768c767000748c41a827906ae829fac82a0fd820a49b9991b928d855d75ca1656c17b931a784e4228d1453d34bdb92db98dd" },
                { "te", "553011721fa893d2de03e440ef65cfc37327dbdff78e0358748552836b02345e4d7b714190bc502d14d8549b5e5bfc7ec934193c294834d198a04bafe887abdc" },
                { "th", "ff69210983747aaea839003f31ca95c91a962eb94a0954ce1329a77e6e747780d722abf5ff87621f09027083e773e3d3e999d527fc0e1dab40d2009cdb4461b0" },
                { "tl", "8414cb43b0f33df17fee1b30cc60d5912c073bcecd91a02807ed896fda75e4de213f45cafd980bbf7f5bb3039e994c6ff009a73d9ca660d0f2a0b521007ac56f" },
                { "tr", "d01d06e8f93bdd61bbc4432548bb3aa84b9b18e0f29b5d7a54116e56ae2f4352a4c644a185147ba88efba3b64bc06eab5ee3f29e1997c527fa9d9cf2292e9d75" },
                { "trs", "7118df3b3b61efafa558bd6f347e97beefd1d40c772efacff24154982dc54c54a2036089cc96f9a601e86a379dbea0f0435180e1625ba52fcd4d880ab7817944" },
                { "uk", "57be71510cc43e023b6beb5aab192cfeb5cac3ebb4205179973c70f46d93177e357d8371cca45f455436a81bc34d874ba765ed2a9c5d35fa6e60b671daa49894" },
                { "ur", "b1e2368362e3720c12260b8ee1079e8b0bfc1c3146ae724827766f5345fdd4b33fd4fa217cd39b55c551602e76f485e35a9dd6f62a0bb4f03aeb326913cb8a48" },
                { "uz", "cd8cdcbdb5cf4785c98bfcc7a5a523cf0f20bbf21697a990f9b9522ac699ab2424f575ea8b667cd375c72bfdc90676c1823639ec1c9479e41204137ea69a1af2" },
                { "vi", "2871e11700a105d8401e6fb11e7db51957b7cd3bc21e154408f37c1c00f5f019d2ef1054de6dbe22c32186645d2955378fe28c0735874f6a0eba43b57e0c4f3f" },
                { "xh", "ac7f4a9f569bd80770b1d3f50f28df94fb90f6e28a59a1d770b09ca4f33267d44353e150db4b33771b015914a378ee47728ab818133bc9be305616eebfec0f1d" },
                { "zh-CN", "a719c43bacbd498893a47b2683615b604af0b1caf61bdd88d52e4aa7d656903f0b7e4a7fc62775346eb9d57e803a8d82577744a52066527e1ab16d1e5f21c7db" },
                { "zh-TW", "712016a519795816f3c320b966be00673b943da02580ce72ae17628f2e774116fc272fbc5d2431a1cf65f6c1a4bda7f5e7c2842985996b1dcdb40856c8d9b32a" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/92.0b9/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "3feafae1af9ba120f299f8326cb6b04f9eab05165d7eb114183d2b97a3c19956912e444d20d925c39b15c370f971f5176addbd0394aee70efa7792856c8e9eab" },
                { "af", "d4c757a40a5973c01463b1362931302ebca73df9485999728bab43f22ebf7ffe8b43dd28fec6490f44e306aed318715b5fa3fe9bff665f2a65b3052933ae448b" },
                { "an", "4d685f3561f2d498e05f241429bdf61270110347674aa3728c4ec2cca5e66d2a4fb5e84f52698a01fba54c1e3455e540cecf11f564ccbf60be757313a0ef88b6" },
                { "ar", "5fcdbd771156982ef48027f61941765129dc5f312e25c96cb66557e5a7409588621fb206b82bf47bb23ef0bfe57e4ab9cd87ededc9d9e5638950ab91b77b533b" },
                { "ast", "460169c92fdbbeab3efb49e096438a6c56240c970ba9b7f273e28eb711a155b6445735e3abd3897ea873678ee1d5d6b8eb67145b9509faf49bfceed16535b106" },
                { "az", "5ab415fd53c76db60b6d040e2b6f0f55bc2781a8c5cf44fe7fb1840a81d16df826ffe581c1b4a8153f9aaeb99c9aea01ae1c23d7727b9d4086b5b7d0c9dd53fa" },
                { "be", "ff19a4d7fdb24aa92594107a65c7c6370b6684ac63e1c4be19e65c5ebf405d4931499ddd2d872177db7651e0df50f305a334ce84ffe709109ee44775b2a356b9" },
                { "bg", "b16579baaaebb7236866baf9b812ee2dcc81cb18ff5107feb11a3e0e078f0b3dad9a3a728edbe10dc8557c8697fef9edd8655e1ad899b3450b962976271c59ae" },
                { "bn", "b23246a93e2aa2faf58e4a8e1e40e7cc4d759a4edaf9cb3d0749548e873b1f91c7ac4c619e973b4306258842079828d00bd4f102be9338a7761ec6c88f7b5ad7" },
                { "br", "8539ae4b4a072396c168ae9d12cc0c97e6f1909ffb93e4f3aa4b74cd6f51db249aa602bb71745bb7bcf25d46e0de3a103fd08e73644ca31ca39345c018d6d101" },
                { "bs", "8a2a8455dd454cfb4f8899f93c2e9a1ee4e18da495ab53e131677194cb8f006d8997f9ccb1d42ddbaaca81dd72337cd051d3069e11c61212787fefb9f8439fba" },
                { "ca", "24e659d041ae38949747136b68807b20b9f995244d53b94787cb8e9e23c087f97216fe6df09016a0e164ca9ee49c0bf4aacf7b8d6d43929c0c02222d72965b81" },
                { "cak", "f8465ff6697cb9903436ce87dde5b2be312f1973d7e3abb2f486a18d4e73ebc2d7ced7a1c4eca5a3211ba4ff6079dd63e304f3ce152a8ac72ff829aeb9a2ad21" },
                { "cs", "6335360b3c5aad697bd19ffd01cb189f528200244db9924bcdce5c3e749b698b9612275ca0ea562d3d5661fc99fa129eb019532dc3470cd522aeee10f581e480" },
                { "cy", "d464bd316f4cf158dc88db0415d1331ceaff00dd9651a9e085fdc991cc7aebac650f1b5d7605cb512f2561c5e8442745c2f41e44d112239d7e1180b2685e1218" },
                { "da", "e6669cf61595e2b7cb232214825b696240ef113d44efed209c9b92d3097db8e3843b2b7045a97c5cface569cff3d1bfc9bd34cfb83634825f3b0bfe0c62da583" },
                { "de", "83bb0af381752d8d6be3d06f4c14d8bdb5bcc01bdb8d9d0c1ee06eef17cff80bdd2632d84d9c8d1bb1b7c051ccee43a38bb389158bfcff5976b66dc41c707c88" },
                { "dsb", "4272413ae9db09a41096c7dcf3a515b537baa416238a0c177c2b2cc2577c16b149a92d5174f6cf7d389e4fc28b1b79407a1c2a82298a36eb8f16e38bfbdadf47" },
                { "el", "ca4371857fe546b97f341b70d129a0240df3e1764238b22ab3189509c1af3068574bb405308451651ddcf060d3a19b69fcfcb8740f30ce1312d2714f86100e29" },
                { "en-CA", "1b0861a64d6e2af9b34586b5e6b40778fdf71bb1ce64f4d535b6ea17a30916b2844f6f801c09d4e2a86bb206a4fcc45989c2fa83d9ff9c0063a7eb6ca30d3e5a" },
                { "en-GB", "60dc70a6bb8dc90d9f0b0ff9a459b709b1b188a16ae31d95b07dff1ca87bb202da23299f8d828c83db851100ed7926dc5ea1b03c00f9398975a1058623ae2637" },
                { "en-US", "5e32c9e3bd781c5945efbae71dafa568dc108ee80bfa31186b8e17b2a4793ceab752bf7050d11fd1c49e8e7ac31930f87acd9e01fb551a63c2eee5acbad30fcf" },
                { "eo", "2ff71ac7dfd130ddcd19df486cacf9f670aae6b635118b58a76b3eca26075d2f1d8d7a3f48fe686aa51cf309537f85e99a4581095beecf0ffe4c3c965b0ccb68" },
                { "es-AR", "5e256d31fd265413edba01bca9143499718d9464d06c313aced6c720f686f977af24699db304cce34f27c03ba4ba94398209a0e59d8ca1bbe89a440d769f911c" },
                { "es-CL", "58735c34aaf775628413438e85c9dd1b3b4659e0ff49050985a411dc7b1fd1a8414170794d18eaccef565eea1e213993cabac1e6cbca973e6cc48afcd1353730" },
                { "es-ES", "cff9c7ad6770269c2455c7509d95f0ac63f77ed2b9b82ad6051fbe57101f7c1926b257f8cfca8a7bd1b716046b7372a844f0f468f8a29d97f6705e054e3c5898" },
                { "es-MX", "17f95c6cb56b0d45cfaba0a35caf457b457d5cf319a5fe327f567b1c5f489c46ebac4b3695c1be6ac1298b440b92a6bde2c19c3e0bd938344ef970e550dba062" },
                { "et", "8a7601382f362c1f447b4d2553a7265d0bbf6308f4af63e848e5440819474d509de57c07d69571610d8cb9a660802c6c9e94f9f1e81bcffb2c10163361898bfa" },
                { "eu", "d376ba99c843c5895ad6d54724141682a52f4772ae4c886eccac80e583878090338a81e739aabeeba1150ff05dc3b6af6ad6102c55a2ffdf1635f54e372e8fb1" },
                { "fa", "79a9a189fd945077bb0da0de52555054f9a05e2fe47f647e2e640069be5e01abdec376d7bd865adf0adc175edda443e1fd444daba6922e3e766228007ff0b159" },
                { "ff", "a521fcdea0d1f269e21afbb305439c3bf591bead20e6f85efd486d07b108c69e353f2feee3ada5a85c2690773f8186f7a269c60b5db3e7ecefdf541e1fd886a8" },
                { "fi", "fa5e6969f1e2c576b9c790f97c8cb2e1d135126b235be11d0def49316cac53946e05ab8f824ef3330d7f77023cc5df494edd58c9b04ae2270c7366cc166db4e2" },
                { "fr", "85e83a237d5aa6d7b038b8d550ee3032cc182418a4995f1a14e6db54fcf7f06ea9e3d532609c320cbf4e52b9f83c8ebd9a3b84784deb9a366bae6b9aed6fdcb4" },
                { "fy-NL", "77700b9c2da6cbfbf68360aae6be1a25318ba85d9c3bd8af5cdc555c176cb096beb26445088b9f7738794907f6cd76192ebb48081048ae78808fb9f8e970d42f" },
                { "ga-IE", "e396a3c3e814e2220dc27297f4183fef2cebf7aca4aa122fe2f0fb337ae4b92082390cd761f51a68ff24cedbf2a4b3f6efe4ad4e4e09ca7b235792da9fc04efe" },
                { "gd", "ed5db16ba6a85c16c69007f1f5ffe91409f249fea31b4e009aa6f213e37c95ea5f4c66c15410f5cc449ca830b3d9198bb683ad3ad930faecc72834bbfaea8f9a" },
                { "gl", "b318573cbe95de246c2abd6a4f2cb16135f2125dd093c9d23eaebdeeb6312385205e9713eeac55ecc3f8599922ffd71f8cdeb0a474ae302d894c5e557c568ff5" },
                { "gn", "15a7006e8c69ebe759dcfa21f7a5e86e91173f74323055f16d218aa805ef90a665da255aa1447875191e07b3e237dd8145c76b555b94d205c640ac66bc74bce6" },
                { "gu-IN", "9e3d5c123e5d544b1506863cb2c959516413e8b0b43d1bd6563b24c4ecbec529b5f112db8ccebfc0272a93f2bd81ef9baea897abd1bf384bcf6eae52e67a8699" },
                { "he", "628a14593370f37d2a9a459acedd92243ddad10b616dd2a8c8d90d9107072e53dbd86632d55432a733613e3fa70d0406f70603e6ae4408cd5bfddcf8acf9e5ab" },
                { "hi-IN", "4cfb55f5b6c8d9761e66dd579fed539b3293ae3f36fa344aa1c8e5877b398e1fb63df5392e29554f4fdacea9de937dcbd8185135e34a39b3c1df00c500aa4d52" },
                { "hr", "2bafce4e5dd8fb3aa72ea0d03353482e1d979dc15f81e8242c03970232e8b5e11e1012213d606db7e1a611badd4e171e8976bad347d204d2c0e9895bb9ec0295" },
                { "hsb", "8231e48da07344ac045d25561d16ee25ab84537535f239105416ff482d27d164e15d58d8b2291e243fc0c1973d41bb4745664b6be1c003ddda743bca17112fa7" },
                { "hu", "e877ff547e8cb505848ecb8b7cb5dc81c9652f4f65341ebb7c969e964860df32d398e858f1324d21eaa4f0786fc40b181d00426625f39e9c45376ecec7c4c98c" },
                { "hy-AM", "ff622b34fd7781467771a1e942d28381793aa125e250598f5fdbec93986dac93e41a1a73dc0139f771fdbfb06a49a97e8d91a339f74adce0c1e7b4f091c455f0" },
                { "ia", "2415f46b3393f77acefe820474c48e237933e638567b976421563cadabc7153700a94dff91a5a5e56a3a871d742363cc1c4aa872605c59550422626a20153b27" },
                { "id", "c66312b73e780fc5f5e61e3e9383f1af6cfb5ddd7a9cd465a51af49c77f64a3b9c4bc414dc7bfc14dd7cc2a413df4d234120df5a6e499bff37223b98b6715939" },
                { "is", "114b19cb28607f6902f57162560a039356985db0a6e41728f0a1f6e6e19524c304498df6da0584cd13d25d3d026b8eede0fc77ee0abcff574a763982d810357d" },
                { "it", "bc9f876f83efd46056f670dda89f7339e59a96a85382792c3534a6e8c6911fed3128b6e9be8bb593afea9c79184d52b930acedd486e39751aeb7b9c080eb12be" },
                { "ja", "daabfa7d586df2649737c3b727328248f1488b17014fde59a6e2d30461284ac0eeea40ad5d227b0342c3371aba8240e286bc2657760a3477ede1bb6c0531b61f" },
                { "ka", "aafac7c8d0a53f15d63feeab0339acdbd7a98fcf5a97d8286565984151c7f2b16258a7b0dcc0218fec6eacc5ed9948abb8356a051d1961e364a9a4e84e84d0f7" },
                { "kab", "fac76e7fa5dbce7dfafb712c3953dda9f35f7df1071378040122bd0236faf29e230f65e2d62ac6b852cab98cf3ce05bd0923fcd7455fa62c5099744f11e7ee18" },
                { "kk", "87411e26583ffa63e10bc0e507aad62a8cef4c34584693e6305c71f993d2d2a91ef5e6596ba32f5dac3a2f19a9711467ba1709f8e22dae1cb4b22258ed6f7208" },
                { "km", "373f2af23b9656ae4d843e3d6666bef002c5ac8655795f1f3e8fb919fee7f0800ba048babbc26622f7992fc789d6422b6c1f786cdaa695dbd69dd428977b4ddd" },
                { "kn", "88d2ebecd6c17254022bcee6065ddb3c74d29b86ed6ac2420b1681d1b2758fc45bf8648a8869afa13a97d5ee6de34d25a98fb42f35be0877a3b2673e2ad2228d" },
                { "ko", "08fcfda0c24af24a04668cc3c3ccaaa9fd2fb791fd2ab56843c3517bfa9313a5906527f5fc563f710b5b6421b0ae3260ec5d84b2819bfa0a6ca13a3c63758f71" },
                { "lij", "bba92ebd7317269ef56e8a833c880206ca0fcf0b820513f2332b70cf83b39f9b51d13dbae9b5aed6414d6ce18d76a4eb1fae807d39f8c325a5ce4af4f9277c2b" },
                { "lt", "b802cc2f57d53464cacd147fffa3bed679e215858ac39ffb8e4a0f7870535e3517d702c076c8e23c70035a31bc78a4bfee71a0f267df37ceead1a5bc0a4d3fe6" },
                { "lv", "1806d985f3d92219b82042bf5863e3fbfbf9d1bf412353369da90f233abc2681747bb362611fe790518a02fb99b13249f6dd0cebf25806cd86a5cec20242ddc7" },
                { "mk", "ed4c6bcdfc0e487291ca1ab138d6e14a6582e56df7042cef8b761a1f0cb8d256a2f0aca6a6c99b2de7c4902c111dda13077fff4b28f12f3cf15aed8fe9901730" },
                { "mr", "0eb0c886b5a267ef0e705229dacb63119927574a1554666a3ed66d98c89fb9ce67ed54c09062ee409cc80fc9860fd3e4ec65c5e46aa8dce8ca0aa17df620bfc0" },
                { "ms", "148add2e46d0c9be3783e442af70d59dc95d2a627d4238fc9b230aa467411e7d62e4f1f94750fb8485c5cb416213eb40a54cb16720a74442fc727ebdcd5a2b7e" },
                { "my", "dc3cc0c8cb8c8a5dc96c34d62d095d16a6da51d200b3c0e2291b431202b7bd84f2d2dfb5a5405a805982637d7d70edd8ac405558b6ad4059e50cdcaab92dfa9b" },
                { "nb-NO", "513a2c4022741a272a29d98d4267c959d75bddc43f1bdf2f741afbef320ac96c13e9c537824356bd0d37e1cc3f29c3d33134dd3825d83dc4b8047178c0731165" },
                { "ne-NP", "ca58bff9f674d0adea05e33f5d9e32623d38203597e066c14ef2733cba08d630f82b982b2bdf40af4a9d5150de3419e475f3896eba1c18e1080b0c94ef8ca5b6" },
                { "nl", "420aecf713597c06331de01aa936eaebe73c40acac4d874395e96cb0ddf2416d51e5af5a0bed023b3610d493c12f2ceedfaf9c2aca1cdbdf870331204ffb8279" },
                { "nn-NO", "588191152a1bf281a0734c859743efeb8022a9f748663ef95366e5218c0ac119da4bd558a29ae3ec219da9519606b7f85af8390c450d89e466c04dd18cb70ed3" },
                { "oc", "cc638155b691ecc4ebfaaae4cca343aae9b96e4dfae017e4d9ebc50db192daf73594a750fc555e2dbb5ba464634944242dd38b8410f7646b08396faf68dab4f2" },
                { "pa-IN", "d2f0202e386ff78dd1c299713a7bd0aae2499dfb9374ea29fabb76696c96ae06240c0d082c2caa98441f0345ecdea2deff0dc9cbf2385997b53885a73d04640a" },
                { "pl", "f650a4ea89c9aaf5d73fba185f51c0d134731d69d0291c231d54caf81299895b244601a08935161ce9ac483094e78919dee9bb984c5385e5894fa869121e2435" },
                { "pt-BR", "e9082881dbf015e0da143ed0f4fdb67f47f596125f8cda30484b299aa18212e076d54268febcc54fb9355ca5c2ef07ffaf4ce09881f9595fea0297397fd65299" },
                { "pt-PT", "0544eca02980b65cfec8d9ccfadbc973ce44503c68b964df5be0a90cda54e60ceb40305ea77a18a8b13496f2d38e7fd983c762b1bf415c00da40dd1f0ebe8774" },
                { "rm", "b2a4cf8a9eb0b8580e1ab4738d6c928906f451576058474f75034c4d5c07794f66cb95537ede429cb9f21883b957741e920f9b29f871995da12bb5ea1cbd9222" },
                { "ro", "fcd50c3b351d646dcef94fbe9e06f1f6dc30bf002ecc075f36812018dae22f9cf5767bf7dab97754ad23ec1500dbb6757da2fc6ece8b22c67a07c9a422c4fa56" },
                { "ru", "0afd9dff41350cef3199c3c856c5af1aaff3d629f24e6dfd1a7b0239ca73b6b823af24d3a673b59bbef2f852d3483c4037e10db1ab977e98d8d2da274848c55a" },
                { "sco", "2364e421838e519ea61507e305caf0a82f4d0fdee0b506fd62ac6c7aba23b762aa9fbff311fd7be6695901654b25b678d359df43344cc6dee78e3c8145f2dc28" },
                { "si", "4f6bca1f70ba42293a623fd99959f7de50a5cac5637e08c29d2c181e6f281b0bebde2c0261963d57754fc5a7e5086685b81c4949a697df024ecc07619e7da349" },
                { "sk", "ff3cd5093a0ce94be2657cf0ada7cceeea99c5e3001ff90f1f73b7c37d4797b2d718691e79216fd73287227d2a884ff4fa98a7dfc3a7bb1487161a82e6fd654f" },
                { "sl", "ebfe66c7c4b49c0eb420349ec40bd22992229ebc949cab530cfaddd460eb666e67ab109a71e751ac588bf441f39541a08c604af24e9b7380b8a71a8014252e22" },
                { "son", "bae1f03b8723d1d9133d3ea88d431ac3e147c7e69449707827f9feb4cfaf1283c558979ffdf35eb1a280590ec0a9279c930ed92b652892e703c7d3347664f7ac" },
                { "sq", "bd3ff6b16b2925ed64853d03e8f241272fc1862698258f7ec9f268c2bb2f2624c7bf08e5e1430740e3dba6fbbacffb264689baadaa40da5b4aff09909a529bdb" },
                { "sr", "b9059b1bb4554a823ba004214ac6698a8ade42d80809024934dba3d509a94f9010f87dc8bfad0283ccf487d674f30dc05b0ad3179e7f95e0fa2d20e94a36c920" },
                { "sv-SE", "b029842bd81555147d213014d7713ba81554f0b2923aebaf357eda9c6566f3a9bb8acca1e26de9f955897bf857ccaf0910b4eb94b9029f446bb02a1b70803093" },
                { "szl", "2590397625e82d1f982d77fe6907170a6c9b987abdf6249cb90ada75e823cfb7b3b388ce9f7ec85f6715ba68b4750c98b33cdc1e0b257f0931fa7874fcc0f983" },
                { "ta", "68b22875b6d7d4ded29f9669bb6b32cf4876241409ddcd7d8929360c01e569ec6792926fbfbc9cb1c5f4cf6fb676210ba061cd94655cb1615a34dea201182136" },
                { "te", "d1ecf70f359a1f4c073706ee7f958de297bfc07eb100424f95708cfa077c0ff43c833827454538d1a62175496b63ce3fcf8bbb113cac33a1390146d167db05c2" },
                { "th", "b8fb8dea1f0c3c704021f18f1f45ea6b71bf86a649a1fe4a803b7e6d4acdc3ea51b58570b89aaf39081936e0e9cb14f7b8c39aa60cf31280718da82393347836" },
                { "tl", "da65d66138de88057e9b188660bae8f9614bfbf270c1ac3e766f130a2b3deaee7a22e49d7b54750d974a24d4c6bcc84bf37ae00c79d3174759bf1232d90e5ac5" },
                { "tr", "538386c2a59df67b1115c0fc6f31bf44781221e7d137afde396665d215fe43a0bb3c208565ef6e09556d94ba788654e68ef2dcc8cf9a9431ad8a26d4d85179fa" },
                { "trs", "08701462e0b98cf7341c4e681cc631d8b5badc3f7bba6fd06d28560abe42180d232a1bb7804e3410d72ad1dfc7b4b1c2ee485610b2d30fb8271a0b2dabad82d4" },
                { "uk", "3a09e20ee00f0450706cdc9b33b4a4e3d34e55a69e818d6fcf2ab70cc370c076e0b84f6a93ceb6d02beef16e80c69189c5fa14b238d83ecce1bfd6c059533e49" },
                { "ur", "283f011376d293380d3d4a904b9cf8259295c9f88971a493aa4b4b407992cebbb6efaa79b746270ef4408473ef05397c3fda86b282582740456d9de26811aae3" },
                { "uz", "0fecd502a994f06c7395f5062f234b5753cb2c7edc1e71e3b8b90c86115ab95516c12ff7b258aaf4cf12300b8fcd77ebe9fe227f98170df897f76c0c9f847f6b" },
                { "vi", "5d7eae7fd62cab6f2ce2f94ece3d05b10a343c262d980bbaf898fd0ec4fc36881fbefe6c28122721237f35d98950f76213d2eb9bc04a660274ff4a53e4f3a9e3" },
                { "xh", "a4d8ea2787acae70f58b0dbd744396833b4c34245bb405f41e1d010cf76f6fc8aad8f5baae258dc056f64af487ec7504157fed10c389b87449e6980109416673" },
                { "zh-CN", "a9b5696b46fee9a983a7baf45e5b0dabb6c1f00a57b3db9fea1a9453b2bc06129c3c4c403ffb96ae140650fa7d72893703cb9b2c97ce1864f8f4caecdcd8d9f7" },
                { "zh-TW", "0da4241db9ef6dbb977b2b1869a0de7c1c86576b6afe53ffd14bf775b0e19f9e531c70386e26b00e7d91488f5d5f2756bc953296230e54fbfc8353f751905ce2" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    htmlContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            List<QuartetAurora> versions = new List<QuartetAurora>();
            Regex regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent = null;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                using (var client = new WebClient())
                {
                    try
                    {
                        sha512SumsContent = client.DownloadString(url);
                        if (newerVersion == currentVersion)
                        {
                            checksumsText = sha512SumsContent;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Warn("Exception occurred while checking for newer"
                            + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                        return null;
                    }
                    client.Dispose();
                } // using
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                Regex reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value.Substring(0, 128));
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // fallback to known information
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
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
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


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
