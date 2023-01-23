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
using System.Linq;
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
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "110.0b4";

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
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/devedition/releases/110.0b4/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "bbd042da0e3baee53065c64e8669b2da6dc1940b939819d3ba71fe27c44be0e8087b1a9271b79a0c52e4906395a300977868be295546635cea7ddfcadd80a63e" },
                { "af", "d34a6c4954412258d1afed664c1e6c8dddf69ea0a192991719c4c91febed9b0c1fd1d371d038c37d32d2360500d9aaf668a433af2bf93c48bea483723ec4ecd4" },
                { "an", "04d61fc21a99460977c2f923b8fe4072fa6002e9fe044ec6e4a8dd7e7f43978cfbf8f4f547b48764086a6367c7edffeb5f1a8ac91de3a5b6fedbd08198831c02" },
                { "ar", "d94eb4fb7f114b8c31c5242872686d09996d98636f1e1342759dfa61ae905369751d81dd6a123352098c502499d68aa53406082edf57b09780a6b9441c7f9a97" },
                { "ast", "969c83c01cadcdd56ca9b6cb8b47ee19c631a76bc22a0c675260d6ac94dbcc06134f400f37fd6333a1d23060c0f7b64d822b427da01a09335ca6503a822c3ff8" },
                { "az", "cb0a86deee1884ab8c51f67c6f309a603e4398cd421fbb29210174bf1ac439a0b041b7e600deb2ed594a3cf0521cb1bb1971ddfe8a5952c674daa7c21141c766" },
                { "be", "cc462a4306cb5628e1eeaecfc8e41b66a0c70f167937c76e2fd5a8d6b6dd4f7b5a8def385d178ffed778e396e7c8bfcd22f81f50b5c26ddcc6562da4188ad707" },
                { "bg", "502cb49f1320a4be090d70c5238c7b1a68fb0abbbde07cf23721102d42b06ec24bad0fd3a61f85b82a1155d4d446d0375d11892a04f5ef3a65ac9df943aa7704" },
                { "bn", "05e3a0b6cef820aaa79d9a854b11151de23e0b1edba59f826857b362e50a45f6c8a3794896aa1915908f4a8d0022d1002da2b275b7421dde38311f00b8cf075d" },
                { "br", "24e123e9836668762f9e1380c2f731bfaebab20cd99320367d4b4181463d38c957079677ad0c85ade729c8c10f780fa13badf051bf42e72df43e3ec21aa56873" },
                { "bs", "d43c62dba102fba402ca212c9a71f626a8156cfa0b5fb49fb91e8319b1df77d065fae4c5ab96054c5f085312300b9ec54addafa1990fcf9a8e6caa10d32b03a5" },
                { "ca", "620ea5aaf28130ae6aff5c64f3279770111d6e4c392e9587cd326b4fadd2785aa35e33eb23b779150448c4de86de7da6c58bdefb4ebeb3de5fd5ccbd0465ad27" },
                { "cak", "348d2a88f643270300b7f5d430e24f80ed06298a0cd1fc699b95646526cd36ce6732c825a9070c5ca091cbca2778326876a3792f41b07d2f4ab301400e1a4c6c" },
                { "cs", "18e33cfc13da978504aef7d5b57bffbc2851c4f6dea32c99dcd91ccaaf58f6d6de1d1de91d5e637b14c44705ef02723ddb08c937dbc825908a80eee1308a1cee" },
                { "cy", "d9e7e482bacd9caacd082e6d290ee516a0b4843538b678714701cb9d673c92c7538d975f5493bfb657daabc70f06c5ffe47242c53ac06754f9901f7f4a2456d0" },
                { "da", "233590403be0d3a1774b9375fc55ce1102a0ce8ebf40c0dc6f3c360b67e040445ffeebcd19f71f631bbffbba3af36cdd234557cac3f6943c81e3a22bf4c46a58" },
                { "de", "91523719f6fa349a2387ac7a90750a062d8a2f1008c08234199cac465abf1f32941c98d9df4961046b0205f9a070d531b2c478c527de33355120bb93d77f2512" },
                { "dsb", "6449f26073a3969c422b5deccf29f03ea45abda465cc53ff8de280d2171010eac1bc0e575d286106edba0ba9edc970f79deddae1722a3551c4365c699ca3e348" },
                { "el", "94f8f1c37cac3cb19e44f1dcb7cc85e1e34302f7ff1de76779230987db238d17725fdbbc76f5af2e150dff69eac09c29b853c3e3f002940b39fc170f19cea2f5" },
                { "en-CA", "3562a5e7c284c933f855f0a346a8beb17314b4a74462a3c778975665b632177df8ea08c707181b3249c4db3ba1bae5860489a55a90baf74a0132b9e204b04242" },
                { "en-GB", "5cb8008b8b3b62095a678b28dc1de4192ced4def8d92dd54d5973d26e795adddb0536a155759a115b4b530f082720a018639732bd2443c205f2edee44826c976" },
                { "en-US", "852a66133935f8c38308741b75204c2446235a61db4c2046c737dfe4d5d15f89a4f25992b56e51d6510f70499f985c100c51dda13807473e523a92f1e1595c94" },
                { "eo", "d5451bcce9cbd80729fbbc4dcbf42a0db2e8c002a8187e2fa9051c5eee3ce83de9a1053b4efd44a663a71ad5436ca15321f7e6097e4ad349a46460680c00a0f7" },
                { "es-AR", "37089a1af8b8293408ea08b01129a39221c8c96b41c1153c856642b53c7aa0cdacd20d7a63f4180c64266f59a1aa5167dce4edb7deea4ae44777fe699d5d451b" },
                { "es-CL", "08a1ac623d59f58a82820469041ee6d2f6cb59bbe784b08a5653e9028db540af42821bce784b44f2abb9c4b3a88fd87b5931e110b41e9f05773832118c388025" },
                { "es-ES", "38423c36b71b41479934c98c70328fae4c5480121d8e2d46be7d4c0ce6985ffacdde73df53aca5ec9e9b925b7652a5c2d3765175833f7912c9291d7b5cf88f13" },
                { "es-MX", "dd682c942e02dce741a74e4464ca2ca7cdd7ef06239301cda025259d4740d87092e4ab3ad640e2e99509bebc5c8e0895945f4364ab2002a1156e6b7b3aa7363b" },
                { "et", "bf8c6d10bcfd82b793a412674102a5df277f22f21d437e378e2b2f1b36e3780f1f8308e255f2e90226b1d56759f5aaaf9ed83c4adf36635e4f21c54842a340b0" },
                { "eu", "cb4a3ff9431c95ac61b0743df39f1a27c0ac0a89f7cebc14cd94b1c7d1eced6f7b4b1408b7ad87b9776e83482ff2047becf089da0847b067318d2d0338113c1d" },
                { "fa", "ed011247567754c2731e8d6be5684d6f0eb1ac9b918b16985ba3c23ae21ddbd46dcdecf271d25239d8099ff7567b30f138630e2216abc193186a234cee0a6abf" },
                { "ff", "d3db95399cbfdce8d56336b158cc1f61b10fefda887395712d016432ac7160a9396ed70f75e2aa019cb8eb490f1fb5efb6a88855fb66efa8bf5d3c9dd84145a0" },
                { "fi", "74bccd75f841ea595b67be38dc2244e1f91f0d9d5bdb84f86033ed08e661c35af465b5422885b0234890b3ede94ae56d08d97ff22f181748791fbb20780aba10" },
                { "fr", "e681cbfe88922736e38f8ba0a032c3ac215554a35a7f4fbf4f4498f727461816d31763a43ce0b881d054cb587ab67f61095165eaee7ac7d82cabcebf85e7fba4" },
                { "fy-NL", "3799956965ff07d129af71d475468e25419180c19a62f41caa94a4b55963d9af7e4cd82ac7e0a0b91cdbdd22ead5ad7765c03f34d81cbbe8464221fea8a1ead7" },
                { "ga-IE", "b03ac4297f38ec07a8b0d111baf5f811cacfd635ceabf2503aa87920df733aaec01c41b7a20384f15d2b612ff5641c781f5ffc43fd7969ed7613ea40e58c69a5" },
                { "gd", "b1eb4bbf955c8fc366f2d44f247e3e65d8c647aaf9eb0fa3d627a8f233edda9ef57ce2fb0b1fc89a1dc64cc7b80a0d0227b6c23ba1beeda0d734e3745368c697" },
                { "gl", "54be7ac4bc834c4c35277ce2a51a0a62ba094060a3dd45c549194dbe9ba4612e5991c82de8dbed01fd45df952f6eccd6990ba81daab2fd1d94c6f95a8915628d" },
                { "gn", "5d610a5d97e1e96a48a605b117d1610821762edd1f71c26b92e0c6139a31382601d7e81b99c07fc53e443de0761bb2b7e06a7e3bcbf93a8c35a97893fadd3733" },
                { "gu-IN", "3eb54262f2177374f8bfcb4986d0b6c842675dbf99aae3983012220caa08fe8de1443869959a35d2107428b8b2ebe128bd691009659aff258b9fdabf8941e5ea" },
                { "he", "18a9cc74711540cd0636819755ba998a2459840694d438ad3f71815bc56211e7abf0e30719278c23120fd8f6e1e7553461e0b159d385c9a40ca9fdf9c1d18eea" },
                { "hi-IN", "bacd57f5a4b89ea9ba2389bbba2bcb7b957a5ad85be595c962d3a69a7f09d5c5ab0cfbf11b7fed30317f0b4df2a993d7a23e53ba3a0576ceb3b24ee046abff89" },
                { "hr", "916707eeddb5c3e9554ab800cf4caf0851fd462c2f6ea8fe7c0b5d82732bc3e28b2b707b79c21be48fc43125f87cf04b6e1a986d29e6e1f7bacbdcd4d5165856" },
                { "hsb", "d8cb6b5aad6f1ae96651518e95900c0ba96a98ef426a9dfd389906e04373661221d44e2bd56a0d1356fba3e8b3854050d5236e0c229a62342d1be5c8b6f7adc6" },
                { "hu", "bf9b73d106bf7f33d37a5ba335876c1da9aaa20f265cdda5c4c6473c10fa2dd232e25ee0299e7d02e4bfd80e5e498af900c98c00c719150ef0a237cc6633b03d" },
                { "hy-AM", "7e674c95cc773946525352e290e8525715b4660176f10e4538c1bc3c2f63bdcb0c6cf333d7ec485a232728b0c8c19cb69602f7cef708acd740a8c53dfab528ac" },
                { "ia", "37ff9d07aa5a67773ec92239db98c25dd7b966443ec0e085265713f343c5261481c633447fe2541892be04c55d4d4aebe11bc4a4feb51dd35050e9172fb91484" },
                { "id", "62cb97177bbaeb05ecaf0398b69e1970b0da3db4102fd156685808d17ce38636a73b8b56eb71b78649cb58cc9c43e0da1613f6e5afe0c3103d3c7f5d190d5b23" },
                { "is", "49b69578fea0e3868f37ed53db686827e3f2a03c06a81428ffa3d388becbd82ef168feadea2e6bd75416d75f6699e52bcfe63dc55b536376283412c96feb136a" },
                { "it", "17d67b07473a95a5bdfd22ec3289eaa977327ec7fdeaeb498f711e574ed796f50d44864be899ce7ca4cfced65e8c9fd43986c70e1a5775aea3f093ae124f1f36" },
                { "ja", "9e2a7327bb88cbf17d10a8322f9aa4f6f025358fd7fa1779eff8946f3c0bfb57b90159194bbcef448611b40cd31a0547be7802318aec7c7059f5d5be2f4c9b2d" },
                { "ka", "dbc06af306c53f0b84f5eefaf4fb82c3a58702baa3fb2f28c844475350b9a7e4b5aa00d0d36bdfbfc6da2e18feb7538bb1eed9a581a822604d200aab9b4aaee8" },
                { "kab", "aa3e2d79cd4d3df752ba0f8b361b9ab70782960d74cfc5421f191817ab6352353fdb3336546e5f4a1a51d3afd665c7d8fc0b99c91b985e9db161ffa7591c6959" },
                { "kk", "35b1e17029001089c1a6f722a738f5904670f0ecc5fea55678bd16c811f1efd7124d3eea5d6617c9efcf6ed799b61c8b35acfd580ee865f302d0bee8e9831c04" },
                { "km", "d3c1203744af21d24a9765673c446e38cfa642070b663e1649dcc47d3d1a489c1b5e4c2f4a68cf750d7bf608e568ea7f0301011435f2833e82d5c7c5fafbffac" },
                { "kn", "ea6c9962db953b4e5fc1a3de6d95493ec3be25cf458c63f545824b911c0a40b6364aa8227a2d98105d2087583297efcd15f38be9b05e51495aff1cc9b39f0af9" },
                { "ko", "658d3396227a762def2e464a4869ad851cc325dde7c37d1c642e60e89744bbefd5bab417fd296530337ff71b4f64d9e563babdf1456689339388e72a9588324c" },
                { "lij", "18a251638c576aae06c0a089a312e011463935da87a6c8d439241c5b95e93683d691eb047579ab893e8a8f5255afcaf8ac75025c770e5447cb932d9385a61934" },
                { "lt", "ce3eaa3ab6fd76522c8047567167c4689c3aeb773b94f4aeb83d047f180cf8da770b8cd2a62e3b6fb805559ff9a040f2aa5d8520437ac8eda67fe040a0d71bf3" },
                { "lv", "42835280c676bf94d80d0a1529d0f1440222434b0901cd824c63fb318c6638fa7d395013e418c615d8ee2c8d968da82832839d29bed583e29d7b1a0259b11e8b" },
                { "mk", "79a55a4d4df6161d2c2102b3816605b43d4ce2b8b23ddf2d93fd9cd3c352d763ee56825cc9b4beb694f98345ace3c7b34c507a9813055a9eacd05ba7aef8c563" },
                { "mr", "e09c63473988ab3078c126acb35904b8d4513baba8139382bd27b755918986f0945b6d39c8aea75afe19c788f04ca9cbf858a0c5c068381b3055cb20a19ed4a7" },
                { "ms", "09e94c0685016fbc6f4553a7a8de49f268c75e33b45f7537df0e9251941e2b9cc29eb8150aa1d78e545344ca837b32f4535a633b6b7bc5d89d6ee3134b9b5ff6" },
                { "my", "518b78ec978fd89fa7716dc19301b6e409932e38359fef9ba002a95763a7c949967b63e6955f1a2e71c4cc386d97bb506623f4da64ce8ae6eaed54a680632933" },
                { "nb-NO", "992fc352bd3a610c2eb3599e9b7e0e9525164dfff0597d9f130b77f1365a54bf6620fff632c580284fcf08a2d7e2cadf31231ed3479ef04b018de5874f7477f6" },
                { "ne-NP", "bff09f444fb6391089bda676990c94b83d7e7aef68166fa61aaa70da24c57a66e68f0c0d908d6d3d6f68eee9f4b617d9a825b7ddf48e0a7945305634e4ec7498" },
                { "nl", "aae87d75d1ecf031f805c82e79c636d58cef9224dd85ca03656f4425b49e7999291ea2d6d2a81f7fdbebb0f1bda68ff32c7e59399c6dbc59da53c9a238d40261" },
                { "nn-NO", "99e1f535e195232cf5302c19b2040769a086b77590de5578d78ebaca6f6c4956341a0cc0551affdb5800d557208622e54c79ac6e853c764ddb55aa88964ccc0b" },
                { "oc", "f4ea16d1b6be63e87793c54ca2f791d00eb1569c59864a17d4c35691edbe44f4a991dfcecdd0a93d1e7be76ca60028456a70e36457d2c03b7f9432ee0edb72d5" },
                { "pa-IN", "e0056495cb4dd45633d64fdd3425af43b5bc87adc76491fdb698708552da53f3430d973a01c2c1730906839afffc665f4fb3f45bce7e752e77c5808ecf96e871" },
                { "pl", "0b11eaef9791033dab2744faf29a0ca7f366f974f2720bd645ad0475a7c540b2c0e0d977b1e3be6d30030d0fde33bccf2cb2220e19e17f4d2ff1b65283732dd9" },
                { "pt-BR", "1adb139c54b6a9293a643016484a8b7daa9138e1997a4b0957f6290b7df44b96caa7cd89184bcd861d999b0e981872e0a9b00b4ad29322b9ab1916ae87433912" },
                { "pt-PT", "7f1a88e2693b1b39daf7d12ec70c6514026729bec1d9e7e4a9fc2b483445c001556c6de6e47c13fd6671af56c226edc7169c4d848e365eb410d4270335a3dbf9" },
                { "rm", "ab6e9c4f2665de5d3768b28a5ef72ce3ebfcbb69bf98d7092ad76d8e2888bd9a11d306b3f29879fbc255f8622408d4eb0c4be3f233d32e0eb1a7f7a140a54ce3" },
                { "ro", "fb9a2388fcd9317b93b151b4332a6fd56945297f5e1dd9e01ac9608c36cfa2e0ca554b7ecefa70708f59e7a99972d51c28d31806cc79a7fa5c97c53306141474" },
                { "ru", "1c37bc96dd2a647db0c656c101c458c6fe51d0e03322075acfe05239c8c3e2eaf2f5289f6db4c23987753d8ccfcaa2fcce1f312452bf7f646613ae53a70ee302" },
                { "sco", "cdce732b5e52787042ebd98b1bed6c10a6adf87189e88e2ca1daec798662309a448e7a2285d142a80828db9f9230600cadb76be7c9a2e54ae9d709ab26e1a986" },
                { "si", "0cd577dcbb60985aeba84310875cac00d21dc00bfade6684f313add116808858960f00499c4ca095600bb3f23b4beca173ef2bc6caec34fc94d82625b6082342" },
                { "sk", "8bf8779473da652801823561cc3e26960c3a1ff6d9eb4938040dba4d55c9c17cf7cff05fd9b8b03769f08afcbcadecb4fb0cdec3d165d3ada2b560c53cd1739b" },
                { "sl", "5e3de1481219a51da4d14fcd1447c0158c5add96180b2badce1623eb8e10e70a7145a24723d247ebad8ab7e1a17903a7f2f41552b147afba24bdda9095c9f62c" },
                { "son", "2a7ee70df330d9125cdbdbcd535215dfe78aadae6fa52e03339eb9454f4aa9029f08a2738d4de7495b577e2afaa1695249ad1f655ebebfbbdd0abd81ab2f645b" },
                { "sq", "6fa1cfc4ae9dd846a78d4cc9f08063dfc14b7c0370e8b994f07a4bb700b45ea753fb6ae8cf49373f124892006249135c376eb78d9a26a51267329b801f3ede84" },
                { "sr", "06e58a851ded04025510fb1bcb4639baf4c5fd60d53d19b55c198e7d853d0829fcc5a38449a88931cb759d0d486116a28aef57d449964605f307e880fd66dd2d" },
                { "sv-SE", "b88cbc2000dcabe7641e895b1544248ad0c798523b7c8bc58518a20dfe1eb893a7723a63ff4d69a1711473d62913c4fe1d7e0c6c388350ce382875754a2083aa" },
                { "szl", "0900d908685e01bd413c47e83702e4814c4e7a46422993c6699d52fbcd1e3b3a2e7171dc9bf696dfc07a3c71014e0a68eac3c97cd687aa99fbc20dafb9c6fe20" },
                { "ta", "87b03f58dec26031abec50b61dddf57b5a4236ac8ce8bb29184990da77def32441e14a4a6d14084a3b1b6b4306f626cda90b3c6a9fe6dc280642ff2b33dd1980" },
                { "te", "1934031f14c036246b9d575faeca41b46e3e5466da0fc111ef3650689932d41d47fe9b7787d3abe6e1af8451401d782dbc972fb7b0cef2bb40a4fc4a1f8152c5" },
                { "th", "0e550aedc3cde555aa089fe4dee4c930ae23172a3519af5e05e9a1595c625d6bd2af7cee3d936985e152173ca5cfe0aa3cfa0727fa4577ab784d7692cb3f1d9d" },
                { "tl", "de602247f51211bdc992876507e88bd05634a95e4f77844da742822f3f7720e2ca9bbdba0fb07d82f98931872fe93f737fdb4454f7639e23fce6b1ac31ddd24f" },
                { "tr", "bed2b8cd3d90cb7aad94450d20730a72d8cf1aee15c88b42d4dcc93b7cce4e252b76287950ba0453cb16b74269675aea3047667103b8fd7b4453eadc16fbbd99" },
                { "trs", "a6c7fde16cedc98ebd443317da94e1b322e1e53d15e3c743986d2bb6bdda0de657b7e884a2014beaaf02921dabf4b14cdbef35d7ec3dc725d416507c3fa894de" },
                { "uk", "54a365b8d480bd3762ebde15947ed05930e035f2e9140b57e073d81f023bc72a758b830b0381521e7fb34c69a8cf590833c1ce7e5f3bec68135805203b9cdc40" },
                { "ur", "e20ebe140bf45abff0421cca50e374877833dd74c2cb2a11637c445cd1806d33d264b955a4be969b0cf2875cdd144ccbfdfa60c2c415654274ac48d7a1809eb9" },
                { "uz", "2009ace96bc5c9d56058d0a6d08783295df1d994e22479c5b6219b919e24136a14d1f36d2a9c10426f682e262320ffb3af53150c4ac4de55fd5f882b9d3acaa4" },
                { "vi", "20176dc0fd70c5d8fb3a090ac4b75dcdfef2cb722d9a92a9cbb3fb2987829851e5d915c82654714817c64bd34ab25db8e5c10bf5dde81cb617e966132242b3e2" },
                { "xh", "0803796ca7434a604800d7dcd281756333ccf6ea81d996b6f24131d7b9dc214d8d3d91de4374de974b8e4facae724eed94ec362b747d8debd6a7496f0f7c07ba" },
                { "zh-CN", "d63caf926c9b8b1b9cfacce751330520471a4b2c099b6aaa89f861da596af42513e2da899e9f14df0515513baccd16cc2cec075cff28c6ab892c700004f7ca53" },
                { "zh-TW", "3d66307ca24678242d0d83dc0194a41cad186b52a08ac0e36b47ceb741ea94e74f332ac37b9fcdcf205066bcd787247731d3ae41c6f09e7b91bf2c7c943cbb38" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/110.0b4/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "87572097ea615233d1eefa0ea0e4613faed945a79328e3dcea76b40f37b01a4fc59ea60e6919398801a8fb640017c781ff32a96af32ab2b341521331cc290aec" },
                { "af", "132a02cdb92dceac5395b5756591d07f5ae1ad188b3ba7b43c3a5ec850a6e5aec380bb5e3c238536d0c04b3698b4804d504afd08885bd7cf84616e0ff5d5707e" },
                { "an", "e8662c4337fab1b825386587864f6efde49b1f8071136f8601fc6f98c22e1cb8e2791991a4252b0744c38a694d708baee2013e79a0f4e92414b1e68357dd38c2" },
                { "ar", "2240046ad1839eda0fed8c1b3792d7c81419adfa3c40b6e8a323ecd17beb2665fdd5bd2d651d289f733f1108998295606f116d37756b2237e76c45e252298680" },
                { "ast", "d80acd29da972f9345999944d6fba976267800a6e2d777224d9afcd62cc9d10cd450c5b8b3eeb9e20a868d0999b774c71f7124af2c3cb2ac5bdb7646c77f72e1" },
                { "az", "c9dd5039bd5ffca7661bb13d76679181f05809f1ddb5161eb5ed6cd3f3fcf41889960ae76dcdac9297440f5a5c2422261ec4ffda0bc127bdd9130d43f24c96f7" },
                { "be", "4f29fa68e81f3ae358a047e961f114fd9139d156ed798916382f836470dc8928ce422ebea23936f8bfdabb32ae1942c3442d0ffa092c00e0b5302fff6106ee79" },
                { "bg", "5e7f120b1cf558380d5fdac56565c97a098cbecc6530353f67fc9533af5f490dfc1cd5c6ca3edddcee1f7f8bad777aaa60e4f05935aef2bc59173e012cbb568d" },
                { "bn", "fc4db572705abf78425779ae027235e161a4dcfbd8bc1c85f0d423b54dc810c2b13633e5ebd770832a8d5a3114cd95650f5cf49e020bbc0a35fc8ed33e56f2da" },
                { "br", "b6eb9f6d748b405eb63d3db6f3fe2d3ffc41d1cf3159a003a16306e003f49f7109553490a2f6c159ee025d311a0ac53d3863ec2f1339908e7ba8d3f6ee5e7e41" },
                { "bs", "8b76b090159123dfb82addf8794e11b9ca936f098cee0197723132db2f225700ac88e681b1160d62e41dc50736f4319ca18be67bfcf2bca5be2c8318431c5b56" },
                { "ca", "c2abcb5f709c766bdcfedc974f9f1947f4979610aab3b32c263cfa7ac8a3dd812a6674252bad59dfe7da181031690e030cf1e97873b660cd88ad3d4e817e5ff7" },
                { "cak", "0bf61dcfa2d3ab7ef18270c724c92d4df3a67ee5e227b24f839335bcc7af22d9fddedaed2095103dd8c9e52b5f0e5fc3cae019fcaeb7dd9196e18d4c26ffad9c" },
                { "cs", "d624689e5dc68969ab569321224d6dfcff153859b57bf6e085d82f9d0e5486e8aaccd2220bcf521c2dbbb6704e299b5e8c58dbb527921ca9b1f51740c05dccde" },
                { "cy", "fa9351ddce6107ed9f924e52c93d1f84068f345341206a549ea9fb80c3188a0699ae4a7176ea22563f98651ff22d3866e5a979dde6dda0d7629f750647f5d4db" },
                { "da", "0720021cb2d6f3d22f54696cd94656664235d43872cac5eb5e01578c02b5cfd1a158bf82b2e4f854bcd573af4d110defcc9d10a908ef992fd6830bf92e553873" },
                { "de", "713a37e45a4d6e103451d51995ea717d07cbe42912f2367a82291d39599be2feda1c5f5d66a9c93da7180650ce160db0a0dba5680842b5fb5c26a1714130c7c6" },
                { "dsb", "9c1dab5fb691aadb22837c6e49d6415b12ddeaecb7f86f98bf814b57cf73ec184fabf0a0067fd65141be182837253559cadd2538f1b1274a3beb0e171812f8f2" },
                { "el", "1f34b66b33701ff67d22c7ae8aba7b81c2e9014bd7a3bd958756f74f8ed962da7cf5d18d0411807038cf7e9b02e721646fb2712c1fd561b95975d58fcca629c8" },
                { "en-CA", "3aa543999478684d84f7e7f1c1541fbaedea72fdbb0b74db696a47b393f40b60337481f6e26cda126003fb560fcb7f542badb047e0f74406c5e9968ea2fc922e" },
                { "en-GB", "71bedadf42196b839cc7e58321a5497a287d2d97db1a7dcbd9087e646a4cbdac6d4a1d1cf1600d29df122ca601097067dc734135a4bcab52654972bef265d043" },
                { "en-US", "c9412e1892947ffb3e86af0bc44306ffa47015877e6577ba1eaa5da7b29e4535e758803137036e9257b8442fe200bf45647fc0c1892a34fad85c96d1c971c0b1" },
                { "eo", "4139b33f44275366ba0f3dadf6b08d5831cfb6e2fa1656295f588f60abb78fa95efa0ca95086f2f419dde6975512a0b3ae2319a626d861ba389361e599418d3e" },
                { "es-AR", "6428df26a01df27de15d6961ac44c4f4a90204bf756ca37721df1ec5994f17f812ca2b698ccb9ab2aafd7d17c984677205f5e8d42d9ac400680ca06046d02dd6" },
                { "es-CL", "a15dd66939a721f854e5ce7163c0d9fd732af5364f0698a8eeafc0c5a66a86e37117bcf21f69d10e6a97fead412bd1afabdd50ddd93b321a090be82532658e50" },
                { "es-ES", "c813b5c3e75c25bd88f6b28038e43a686ac8e4c13beea38eee9e4c2c11b850a739fc1b80c88f9bd52de4e384ae6c195b6311fb1dc9157b7c25398d91e109a97d" },
                { "es-MX", "c261abb5304b0774077cf8b72e2ee3d4cb8ad4fe6f3476ea94fc73597617f34c9fd6891f4d6679c4a451e1df3e8be257e195a8259111ed040427e1c860c2012c" },
                { "et", "db5649c6942bd6fbfddde8058b363e800a8079c0638f8ee6e03df20efc63d8d7e239fcac48ce1eef6bb90c4955a2e38c324e28e5422448bf48a99e1d6f81bd81" },
                { "eu", "e7161654067f23a6a8102f168bdc8831d939214ed0d89b112b963b1c73b8048ad9833dae7b570c259569bc714c3376b890bae441f54fd40e87d5708ec8e18904" },
                { "fa", "9bc0f3736bc9c7d8aa8cb073fd08e85f334952847667504d337a21d7166674e6acf57a1a180c5f283b914a765168d3f2757b5239dcc26f5b4a4615caec646400" },
                { "ff", "384ca5b9a903245842d1727a00d3dd2ec80c0670156e52176af17ac8917b6d58c6dfd26b6890e0e5292280f0511aa1555a62b023d013f6fe354b90365fe8f721" },
                { "fi", "6a7ae8917153f82d1b8a5648f691d36c1a756b831fee253c711686a89faa6af3814f421064ebd91747879700a6dc4d377fecfd811dacb452a3a2f8270dd31f99" },
                { "fr", "b8c57ed32479150979817dc93d0ff2053f3b6baaa8e55bc56221a8ff38e850d6c475af344a0b0cae39a2fbb6e9768cb46871089826745502cf9116b9a75d5f69" },
                { "fy-NL", "cc4c93118de04bc38bc333f3db70456bb601390062a62a3d1c9b4cdacfcdae5102f1b2d7427406e1ebaf719e364228bb8af7f4c701a75e85968b1b141c89b965" },
                { "ga-IE", "80badd650de88fa72472acc9eae44ac3ebf7bed80fdcd0c8a917a084882168dc8cd3df978900474720cf1d65659a90a879dbe010cb3ce90da70936ad446e9e6e" },
                { "gd", "0be227fca9ef0ce3fd3001da870c8de7dd0f14a8c026bed3c30911362a6b6e40fca8c9394cf3db4b1fb54cf0544b217cc661ae413c25328c9cffbdb283c908aa" },
                { "gl", "ea824093bfd08a24d807a762f242cb1e3edc0a068a6ee3211e0ca19f2f4de87a875a124c3fddc5f9023afcfc0373fa0b57c98b08b27e684c1c9eb9fa689ba198" },
                { "gn", "e1168ebcd33ed44052ce7f395b63f2336b4bba3d80abbff52910bb99ec2e9aa21e8d5fdd7c1ed973a05763c1735f7577fd91294db61682fbdd70b97d2c092932" },
                { "gu-IN", "8a5b59a9c98c469d963d977abc88c694d7864a0bde74a00a547c6b03bb035fd42cb16e8c47e96a472f0d5f82be3c197712e9a1377725745827120a821680f31a" },
                { "he", "2a2d161ae9883c1b54d3706d085f6c5bcde9add240a7e7536994339b4ae0e3a3439adce4066c2494a09dcf0e4be56a265b3241d670f68e41900995397e0acfd4" },
                { "hi-IN", "6e00d70089ac1a66091650579749c8b22a89f7996a40cf4edec37277dea91aa5d7c4e673f60c9e2318daeab5f93672fbdd0d9f4210ffede91cd1076f3428ecee" },
                { "hr", "1e625801b7bd03c8ea961f3a991a72b0b141168c710997102dee3638509b7bb254a10e7466debf6f6af6b2634cbe2951d52d3ced6f36d18365f6435d842964ca" },
                { "hsb", "5c5db077d725a5abb537452ad1fe0d24bca80003b3a2aa8f13f0b85bb8b92c39f2ef98c7e865420c299d66cd004dc7017a6543a7cf7123fa123cc7f9959b188b" },
                { "hu", "e9e00c44f85286c7db41f922aef10064cdc9ae9ccf4d8d223ce013454717943f3dff9a606637ea59babd990dec9401382b8919a5ee3b092aca239f129d63f941" },
                { "hy-AM", "16dcd24ee1889a42977c99849427178b9f9788cb0e62e6cf2c002908f944410ba370a616277510ec6cf5d508455aeb71f74da5f04edd2a05e54f41b774d0ff2e" },
                { "ia", "12223058db532c28efdb21991c776a4d7c2820c476e7f79840e6341f1b0c8345234140135c217ca94f33d03a054d711b240e98c9157793752a7efef2d11692ac" },
                { "id", "85c9f0f2d32c2615de62165ef9cb6b01286a69faddeffa5e7dcc0253f01c3cad130c770ac74f20dae35dd382f59b9ab4ed089c8ae881d049d7fb868ed7ed11d3" },
                { "is", "9f40e762a5b4b9fe5f712ba1f1b87d45bdc048d99336a609bcfbdb5e3799a02a599667c50617a56815dcc409505dc68bd0b4d6267663c67fd6e81964da6acfee" },
                { "it", "2a0cb599588d550c95d37c1f59f3f5d980256685f6ccf97170fca3a0d9502abf83751a71e9c130bc42055f46cfb57e2d96819c0e3993fb47509fa2f781a893b2" },
                { "ja", "d56f2a16719436fee4f5e02d26295b82cac0c1874420db0050f9ab8495fec6436374235426df3f271194fdd12ec48ed87016d0ecce4580027c888e250d53f216" },
                { "ka", "79d83b4aba5f09c40671be000498993d5847ab733b63f4724e1ef249c3a98524e5bc78042497cf3629ce4f2ed0835bca44c9b01d6fbf69a43f6bbc83da5c265e" },
                { "kab", "8b804deaaf93a6c73d51464405e9f444360564730c4ab1ad68519e0c089e44260ddd98718285d0f6995b67408c38b88b09a151a0fefdc58aba79596371c0acf2" },
                { "kk", "dbe20b1ebf241673b091b6b4a0422e802149281417684d2eb58adb6f765c2543d0e2d462daa3a69c7ce4123bbadb8e6de40af78010293aa497c391ac5b46bfce" },
                { "km", "7406f4dbdc09dfd5a9c8eb0ae785882be557009b361ad42e2232ea7c9457e675356b8f1c1140d420850063da3b3dd7eb769b9ff8659aa58320369d26d4a99737" },
                { "kn", "5975b38ce27338532aec6faf135e0be3507b65a84646be11f3276a56c8f27a0122dfc87b342ab364c01a081e1d2db610668ea3befa6c5cf101d27ad9a437acd1" },
                { "ko", "78d0914f9af47ca877d36567b32d5afd2655062387874ed9118b072b73f70fa3b26468b36caa64793fcd33996fd908ce221c3b1f4f2ff4aac5fe15a92837692e" },
                { "lij", "8d805b0fa424f9e98a8ed8ba5cf616d1e4793b1eaabc8874764e4d7400dacc14836cb6f8eb78007a30c75fe6a20dd8802d76ad434f170eead7f6d6d83402c158" },
                { "lt", "50039b291e73f0ced6be4b74a6b74d02aff78e30043a53d5465b927c48c39c538a7281ac66ea0ee6637423493f88ffb0614d350cb0d503042d57f7141728ad47" },
                { "lv", "749c9feabaa3fccc2bb25874825d93d510497193f181b3e7d60f1995ddd4acf60e2f3dffdab867089d3213cd6356108b9865143581885b00279541806341c5f5" },
                { "mk", "e386543f327388f1cfa4ffcef00c86bbb4b841dd8b02329d6b2e18a8b507ccb47b837a069672c51982fee163664ca5ea7e2da4fa4d16116a317993a6858bc20f" },
                { "mr", "cc584972372165195dfcaca1c1caf91c1dec43812316f7595d6233177eb7b3090160158d1e6f927fc7689fd445ab44c91bc326abd4418dfa4b5b1b86c5b0f224" },
                { "ms", "eecca1f72534e162c1941ec1a7a46dc0170bf96fe76bd5e53a8dc256fd649b8e7f30768f82ea19632a25818c91fcee91e6ace5046085a0a40f1bd49c0fbf96e0" },
                { "my", "f79017b17365c997362272394d5bf11270966d04513c2ea79651cce9c46d083c4d200ab4fe4c23ced0344fcd13e6e0ffd817f5b70b95d7823b767a449eca4781" },
                { "nb-NO", "f001fc12c491e429bd2f65288065059dc49b36b202e925c65733755031b57fbdf1e2f7cf866b8b209fa53cc7c9d91ec52974cb3451a14bb02b798fd3fe5617a5" },
                { "ne-NP", "92529005c3266125e590794c08046ecef20a05e38e5d5f3ede10f5c04c3caf442eae277eff92e94cd4cee766eb42322f6b6e58880dba3a8b191ba4d2d2254ea6" },
                { "nl", "221a50a129690b3dab21b5e31f9fe9eb7112a18ee77aead815556f00439850b577cec265c199f702ccd1c018f4a48cedc7ef3d89680919e419988ec6c2859a84" },
                { "nn-NO", "bd01497ddfb8c6ff104fbd8bbada286b43bda007c735d87132aa25d649bab0411e8b72056138e8d48ee412fe3e966c83ca38497690d0df20d60226dd8c1fe4dc" },
                { "oc", "6cdc09919b9b0654d3d5c928813caba9adb8eed638f1daec7b1bb6fed1b65e098c256dd102d97d82f11e0bc6e52fc7bb598c3650f8f09912674657954ad1e5b3" },
                { "pa-IN", "a5a86b79cb77f0d85e88227b9b9e5b6e4bbaf631efb24013fbde90a60c139f9d6a6504fccb7df893624e505ed82cc2e23b1420c3e7639950b5150feef678a71b" },
                { "pl", "e6ba9556350cdb1cb4441accbe8a9a2ddc2fb6c3d80d7aa6805fcd4cf91c1e7d75db82a44eee72ae6865cf46cefb81f1bd4572834c70cbc82eb6f0dd2f85f5f8" },
                { "pt-BR", "83258fae4becdeb5c2bae8102831202e609d51825c19e5716fc81f7ba1f631c63c1583a59c30546f887e0cbd1262d539667b7889bace35fb3802307e4d374359" },
                { "pt-PT", "eba102bc613c57bf2ded9b961c7a79b300a31e6f27ac0ba80b498851b64143e9c4333e3f3a9a13640b6c4453f6f58e05ae3d01dfafad5a1ae73eaa80397f1ebc" },
                { "rm", "f1a7dc47f29619e6e55d11df5530388dd29392f6d84d286bc5232252ce5ba136f16ef9f0008c5e114812b6d379460e9e978ce9e9e2df5fc2b2d617b127b10695" },
                { "ro", "5e4e2a93149d80e004df82f916357db86fcaf13f14bf509da194023d09a7b5163bf517432f912da117e778a9e21f60f8700e31e2eec97506715e32e4d97eff90" },
                { "ru", "4c69e2c7256dfff7a4d10f93c64aba33ef89642946711a84b98251a67b7ae97995e01c17c19f415726c125f8698edf9e4e09480eca8c9eec12ae437b85a2dfde" },
                { "sco", "bdc35acca7a93e2fd1d03f301deac15240e528395a027ebc1b7d810ab3170c240fe8f36ff11f8aa035c45abef3b806896718cf146b824423d830305b38dd45ec" },
                { "si", "b59456f4454aca150194edbd325d4354f176724012e4a8dd07aa377b00aefd5c22cb1b71480737bb1ff25678c1d232238bd83169ac8b06b3fcdb2a86d0cfc08d" },
                { "sk", "0c5b36650744f388d2ae6a2a7a7bcab14b5419a04fa39cac3f798049d08eafa09488de8de3406e066952c00ca157b9d4e999f23518bc60655587d03520afcb1a" },
                { "sl", "972b3f364eb89ec716ec9b7093057fe7ebf0b806e3cabc93c470c6c859cc7563944c721a7d5e0eeb25bbbba53d07266b3802b04fe9ff3fd450e4d12e3a9d95c2" },
                { "son", "9b32abd83b748d96bb29c00f534bb2612488ba8e7c6cc4ae9f1a7b1ebf1155689c7420dafedc4dfe24b63e34791dbbdb9f055d302a7c2e0cccf3648bfa221d53" },
                { "sq", "d037e0048b1868d19220e943a0b1f4c4b22a6c019be5fbf1bb4e65fa1fe1d069328790fa8a556625c86f06110a398747894efe01007e58a6e976e6ab73b5a601" },
                { "sr", "80d80fdbcb5eb1c32f52926fc7a366bfd2d5e6c12200848fa63fc281be60d89dfb54b728a7158804a54e5f40a7708937a90133f7234447ee8f0a10b163618324" },
                { "sv-SE", "2b2b437e09a253759292742ecda24759f74a87d6968f9180db7c6fad653e0f6d3bdd6538451a9382819f1bf054956e938c9242a721c50b08310fb7d160e8b6f5" },
                { "szl", "b8c0d0a58d23f2a8060dd26403b63802872957572bb09b677448b9bb48f823242814acfb56c47696219ec6e8febfe38743bd9385b7023ebb18e2b5bb21a4ae5b" },
                { "ta", "9c3e0c00fb39c5a8bb56c7fcc66b512fd260b2999d08c18c7a12c8fa1c71b19f04c60bb0ed35282cb553715c749d11ba604381dfe931bb22f3321cf51261e6ab" },
                { "te", "4fff3bfe1d6fa48e74e36e4733a75448792176973c9c51c14168a24d2c9ea92cc0df9b4ec79fcbaab3e755e521935472a5e158dfedc856d1a562766a58dfbdb7" },
                { "th", "838a395407d5507a5aa598aa440749f8b39f2caee6b9dbc23c4b528e2a03f35536ed6ae6ba81abedd8a9db27ad5475b84db718f26f6817b9e266371bd2cb9afa" },
                { "tl", "112902bf1fc6d07ac05598558790a14fd109e410783b1b480179c0264eff5511a38b9a328bf4a3e3b70f4b8becbece53b7849747ff879e3d4355cf8402d93d4b" },
                { "tr", "1ffe6e8415e063ebc1985712ba0967da10d4052653d41b761ff3d46187e60c5419099d60b6c070abf9f7a203bcaaf576764e1129f9e0c0990c7da701e5de2980" },
                { "trs", "84e7f628c27703742ffa4be246ee24ecd5be5cefc821512b64b91ef69377163647622a40c49b07fdef51ca21712bf22c86e8c57e76af0621a275c821e3ddaaed" },
                { "uk", "3b6f34298266bead8e9fac068d8c47cade237bb7d33d29db0e45d7d55ea31d8dddf87d99b9790ae5a7f4bba40056e31148d0e6cbc55986c3129662dc6e2bb850" },
                { "ur", "fb7ef987eec1f7191774a872046a1c87c8ab1eab3305724a3320cdf8122f29d8aad791519e83df2f7917b27d61d32966e9492b53c72b4f475bd9accc329dcdbc" },
                { "uz", "c0c9b3742940d6502586781e00b28c30f0ee602c025409ce6c9bc6b93091f110624877a77c9412866e9d1083f413e35d8d0500a9ac7f1605e3c9962570ec0e9b" },
                { "vi", "3e0afb7a7d3c366681203d80b43eb7ed4ff6a81efb8aabe7a4e3b0fad92ce349923fb6b46cb9179a14fb2e64dee113c0492c3059a06eb71bd3881cc2af6282d3" },
                { "xh", "7840b5fbf27c4b678e2f0beba9bcdcab6cdd1c763a74aaec3f40fb3e66c86475164db170999550ace73b7e9e4b92c5fc0ac236f854adc5aab0008c100ae0c036" },
                { "zh-CN", "e6f555565c1d55667d063690ddc88bf80fafdf01ae3c1c4ed48e8b934abdb05d3c92c25ed267be688328d7f8fdf530f4999679573f08d003ee560c130229f0fb" },
                { "zh-TW", "0042c5eaf33b286460bf807f63fdb0b23a9ad247ab9202dff0199d0a21340d1bb3aaf24b4be43396d04922ffb6730f452e7a9fb95009ad0974b549584a9701be" }
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

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
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
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
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
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
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
