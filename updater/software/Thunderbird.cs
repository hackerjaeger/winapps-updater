﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
using updater.versions;

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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// currently known newest version
        /// </summary>
        private const string knownVersion = "128.5.0";


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
            if (!d32.TryGetValue(languageCode, out checksum32Bit) || !d64.TryGetValue(languageCode, out checksum64Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32-bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/128.5.0esr/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "8cb3cbd93882a92bd2653adcfd762c2c0f78226a29c43d4441e699dcd17aa9a7bf82f85c70072919280606357001af31515abd7a30c64aa85f94c39c51adc445" },
                { "ar", "33702134b89d673c139af517bcdec0fb1afe19099e39bbbdf878b5f84a7376b3986ef8ca3e64b4430aea5ed9c3dbd53b227b6bac3ad3e25d8e4ce114ce28164a" },
                { "ast", "6789831a70c59794aebe01150d131531b6050f45148c9c6328f4dc9369dcd8cce16c8fe9f4359dd627b5183fd51db279e0906fbd5a87b99770f6a20d1c1d3259" },
                { "be", "c40884c771715b81dd5176ab359389183feaea5ba422cd0eb40dfa972b7941e78f5c5fdf2ab02ef80c1f589f0aa72c7bd621af7c8c89485155c1ea5b7b5a5820" },
                { "bg", "034e3a5fd902b20e638ed23b0d45a43762bfc0f442ab7734b622146ff4ee986226d42c5119442b18e595cdce02e41112516e412ecf086888c8328009a2ea8f9b" },
                { "br", "4bd59cd3e6452b650355ad7f8abbb9a7a99ec2b73a9fae68e9753868182b8a873cf456f3aa40c736cd3b010ee59143056a3c5e6ace3d46c677934392279228dd" },
                { "ca", "8a490e15a0369c654f7647b125df7b73cd959aa56b6ae2bf337a321196a2f633919c3fdb1ad2684ea6a82c53fce712709756308acecdc4cd943c2ad9c3e5feef" },
                { "cak", "8e306a22e82617d5496058bc99c2f3bfadbf8bbbb9600f2fd61b0a61badf2f48440d7291087137541d3b3a45ea2fcb9fa924c0c85285bbd07898c1944ed742b5" },
                { "cs", "1c3e262badccf95fcf1b677c9cc2c3c9405d79980cdf1d65b8ae5cf85a164608c3b2f107c6768bc69893c802b0f1b1bc8306aa4314e5525f1c4d09d30f5e23fe" },
                { "cy", "3eb7e7e0c15eac081b72230eb4164859cb9227ab028dfee3301af9307d3e3c23776dedb3499818faee50e2933e65f7cbbec00eeb40a45b1c4ca0243492dbc695" },
                { "da", "f502c0453450482c0a1539e3c5721175a34df705360d9e23dfa12cc4b1cfe70e447173487f3be1ea9a54c7b15e7bc4fd491bde3dd087dc2ac78e2febd82697f0" },
                { "de", "df499d7f3f6f3ce5ebbcfe102c86ef0c9062e214a997a8b3f5b135d32c9d629b033972bac40f6366b44e8666475b8e9200b89ce8632a61c5b3fcc9618a29d0fd" },
                { "dsb", "440740524f299dcf0c2ca4b576394f9d06b456102e309daa9746594d120c62ae1cc94888e8a874ecacebf16b494f236b358151f3ce04c51935ad7c60d8dfde43" },
                { "el", "f643d1453f1820e9c89396bfd476126d09d1dc6d2be44b76697be030d3cde83b86d7814dba920d340c95aeb11bd6bcde867ec018a79380e59c7b91d5111ba296" },
                { "en-CA", "a9a3f75522f16aa1d4ddb6360bb3f3aa21ef951265f912204ae52fb97132463c53ada404777ab1be1bf0b25119678b1db25715040cdfec89b0d23efbd1ef55e5" },
                { "en-GB", "4334d7b0dba58d6dffd09a28009133c09ee785762f5dfbcc12543c61913ff0dc70b9d753882a21efdfb726544d1dceaeabf350240fdccb97bdd77755a2766f45" },
                { "en-US", "1c7454f67f9a65bb10acc661fb0d115596bc470489b731c92c6b5c034b1c52228cb6676942eb4534df350376d11677ba7b51b0e13137837cc2d7a85d9d82f736" },
                { "es-AR", "056c4ba805ce461d4df645a5205d18611e7eb0cf32408f81e9a7b80a69f3b7a4bb697f94d70eac5d81c149ad2151e4b6926a2a86095550ea947aef9c8f91da15" },
                { "es-ES", "8deb43066d2e81e05df38137b1d0e414c493b957b8b6b22c457d408b07124bc1b19ee5a12325fc97e36d2d0353da89c599d978a41d290b634bcf51e0b4ff4f11" },
                { "es-MX", "681bf83c2c52ca01a92b84ff2e87f3754ff1456e8194998a393eb0fb641559359f6173ffa57f6e8decd4ee2f1c24286b4634b815a05f2e767d8ecac741618f2e" },
                { "et", "4a36c1e492fa534ddefe63d9f0e58183ab80bbd478f667df9f19266f4de7232f4879eefc2758ff6c71fefb4f93c17355ee1b59a4694c92112ac681c39c102ca3" },
                { "eu", "a22bceaac3ce561adeee82021c3517143027fedab5f1f1fdc11691bd3a838947b9f43fa74202319ab388b2040757a00b89adc4612a29e6f028870e9f67061a1e" },
                { "fi", "ad9eeb988764b46503d64d92bc47deb3530fab40d22c781eb66b74875ab962c1376049524a17990d5aefeea21132714e586ffa5f48dbef9cb9623540991aefaa" },
                { "fr", "d6ad1c8bc39bd399c2c20d0c1fd1e2c7fcaa8ad8a3aa968bc544b1b290fc67ad3957d6bae8d0f8a6c6834f47904676b80f07595f0a55139cd3cb782c330b8daa" },
                { "fy-NL", "0d7cc54cd1968b51ae52ef5f4bbb703be34d3e0a8c86c3bf66e0de766106867a4e42f98c4579aa5e9eecd02bba3012b9f81cb823d3de43085505de0aaf36e541" },
                { "ga-IE", "209505157a15d435d0ce13ac7eddafd268f81365e3753e88edab0829c9cbf0acc0115c1c73d54555dfc52b7d133828b0827e6737c5f9d49f27a00a359edc29dd" },
                { "gd", "b26927e1112d4785fe70b75cd00f8ad82cfb6b5d776f4941063c9934e9417e2cf2e574939a2b0efdfaa5e81b8d26fba77b206e436719ba4b8ec03a5b5ff7d986" },
                { "gl", "9cc1ed7b9df263e4de3529bd10c9939da7d1554e07685ec1e85694ebf88b1fbbf996d30b72475add7872c2d1140054c90f0635f58731eb06d27e0d3760ec12de" },
                { "he", "cc28e32e3fb1fd277118514d05575c3b2766f5237a5600f4cb1fd2043a55df04a5c657f5ea77e6bdf875eee687dec7e19269acdb1f854ee0d1df85d36dd0be5b" },
                { "hr", "7de8def31dbfdf5a288971d3bee2fc2870b722030535676b75399e1ed7ff80f22177184f03b82183665edc4d37c877c85c65c5cb119883de3adbb3a97d9baeb4" },
                { "hsb", "d53f82c3a7c2b4c340e1ca676c4fb4ba34a698d226d18e9c0215e0a8fec3da22c7e2b7176e3d1f5bcea7937adb50207427c317272b79c115b3eade0d0e138c87" },
                { "hu", "65a401ee372b54b8a4ac1adc27e311d5713f2397bbee81890f16e8e11b3ee57ed9d2873b48de8e97403121eb8227f91b004290dd52dd978aed58b6e3cde82a59" },
                { "hy-AM", "3421c6f664d07826d4c346c9517d28ba790327fb52163fe69f80821ff4fee0ae82249058537f70a54aec6bf54f05f3cff27106896b97f8099721e80158a18cae" },
                { "id", "6f37005701d61017690faa59b0e736bc8277964139810780688f9635d69faca2469baf64c93667ec616aa30cad6ec4e8ae985444edaf7351d9b4e34d0ea60192" },
                { "is", "3e66e3c0acd565293293fe5397ae45288fffdc250a5fdd855debc894dbe9f09fdce579f54c27259fa69d88cc837ff71418f206fe175b98a6f143680804cfd899" },
                { "it", "98209237e27ac1cfeb0c78191d45c2364680148d8f02926dc763a4016617695e1117ddd2c7da51c44118f02a7349ebc4771982fd1058e045730bcc3b14f1bd6b" },
                { "ja", "cc4f48143610b96061a82a133bb143eccd64b372b03c957c1caaee8743873e716013c65ed892c8f6b4759c4035ae4b8fa345303651f31830f7b9ef26544ca6ea" },
                { "ka", "07139b171eb99f9769987ff60510f0691e28fcc31584002859e605f0cbc67bae29cc15fa44e178ccd9aee7e20ffafdebdcbdd1bca153053a4d9227927ed4c967" },
                { "kab", "322af3a7c2a0f627513506b7f7fdcd70c88e0b368e386fadad77b3f93452c123007511cccbb242dec60cf8ddf646582c4bd8631eb7f6a2b6b3c2dce1cd8ae95a" },
                { "kk", "f2ae3cafc3ff86388dcfa31a5fafc81c26f01205cd27b6c5ade454e01c9aa911282c84318c4873f3a2c83fcb016f80de40c73e7838ad1c6e7b3b8c9147f04a88" },
                { "ko", "cd0da1eeefe74c5a8bf57a46189c6b7a7a135406362fa0d427c0cd3cb53463d7d37befec26b63f5ae399dd637b0480c87b6dc532bb8a81facc69501cc5225afa" },
                { "lt", "896d5699dca7fa320826dec7e114e633250c71e0586d79b08063b7a6842b5a0179d97ac20cc88e8d5caa0e43c5aa77f1e0dac02c5446a86b75a9e8242dffa32d" },
                { "lv", "b790c4ea9ef7929391e167b0c085dd9b7a641f963529f46b774153759c8dfe1fdd70c83718561a11f5f9b4a7372079c9674742c7ff163a042124a86d4d306354" },
                { "ms", "9b8ee0ab6462e50146bfb2694a09c598e6d5519643ba6e592a0a3695a30eb1fdc70922cece677ec6244e299ce73cb59622267026e8bf213c4408b12a1e367c8d" },
                { "nb-NO", "62a6f55c21861709c999eaab7fb2528deeb44b9d4f1427b780dde85ea65e0306688de9991c0062fa52f38fdda56185c07343f60402f3a6e34b21e87e793432c9" },
                { "nl", "ce5f5267956a58a9d728eefd1743f45d6c731433f2c60eaa5a5694f6e96989e4be9b964acadcfed35378ff41956faf5e58f339961678b39d978f82acb2829ad7" },
                { "nn-NO", "048f41a41b75ba896e743fa6dfac9e0a3343eca56757481a4da4ae89be16f81bb5c9be4ca4b3458c114371c74d34430871795cd98218dd90742b6b127e97daa6" },
                { "pa-IN", "9b58fc7e1e12cb83a35ef8d19e5506fbbe947078be907cefa30a426d2df451b987a2595fdb574f4e91e61b0fb9ef9c402368bb7dcbb033763c20098572ba5c1f" },
                { "pl", "22a75faa28466bd5a2973c18e351d206d34da1a08c00c2e9671c288f4589343d9e14bd11a555913557eea579da7b44fbce4ec6f152d752470808ec751d1da2d0" },
                { "pt-BR", "b408ee7779e715a8b112016f7201aab554356994e033fddce3559b73cbbd4f9705aaaed2efe055ae75ef31a0ecb4a30b80f686d6a82703683cdf12d7d6aabf87" },
                { "pt-PT", "32603680406b40368c1ca74d477aacf4c0a6b58b9c0a618b5b6e81a7e71ad7cd864e244cc193cd40c9b1bc781467c19154cc25a227d466867d0da5bd098ffe03" },
                { "rm", "97ad976e005f94c77d6d6e235c3e8d2246e18f68672f9c1903e36403faf22ecaa1f98411e3be84548897c7000b3d129639c6aa179451105720a8d207ca151d7a" },
                { "ro", "f2ec4d2913cf95cd6b812b890eda64a7d934bb41f8e0a8e1cd2c04f4372d8c1353767eaa16fcee27ed49e16b16b60b4e08874953e4eff30f11da7beac8d0729b" },
                { "ru", "4782db503fb79febc2f9f12d17cea05d9743c085b46c457763fdaccb2d35305ddec0b8c02085cbba77ec7a8dd48f843cfa4d414c76bc175fd5ae5d5d0269b842" },
                { "sk", "b0bd2220895a7328d72e2e094fdbc8a524c42eaaa7812027ad15ecdeaff98744db2bf822c980577696b106bc3de83862a3657271f61670fe59757d2ec4ec7deb" },
                { "sl", "bc727dcbb12bba671abe8e31d402225b43b7880e9af47bc6142eb30018c6796210155e7d83249f6db62340559f2e2026c8262f2ad276b6caa88f663be3762655" },
                { "sq", "2aa0fb0ed3ceb967cb661fec18456d50e2eed3d2df50de22467bd3e63f4295079806388a96895ee541b31b3b91fd5c3077b286d87a0795c3416b1e25aeb249f9" },
                { "sr", "1d2fc593654dccc7de570cc863e5e2482f6104c91d1eae667aa76f28c538e7bdcc3cfec5cde5cc83922d29ab960136493aca2dfa3aa236d9189733cf100ba158" },
                { "sv-SE", "38806539a98c712ae54b07dff2284d6ef715dfba32ffdfe099762f7a36f1269e769764962db5d482eafb3a227326364795e97edac11a0a5f41c28e9dd671f80a" },
                { "th", "9b280fb8c3bd8ce17986290df76690690560e2a1254eb5edfa8f26c077ada4182b6dc27aaa9ea70c9fbdd30de6c6394050944399be62abd0ada7332432f144ee" },
                { "tr", "cbb30614a29ec4185f85f68affcaed98a539de62cbcf5c1387ac99705bcd729a8035579b517b8e72ed80edbd9450a7fc82b166fb5d284674fb32bc9e260d106e" },
                { "uk", "5483b383fd3773e93009885dc5427431bbc47430966ca57ffc8b73b92589a23474c71cdc581da14d297dd7762cb0e59e280824981f431f79341165d4056598b3" },
                { "uz", "ef7b6c5cbfebce85305f651ac383afd7b92ceabb30421e810c00a40907ae83cb3a56086d1e28cf4ca7677c79208ea437bd76679b91d94a3a173a794353e267f1" },
                { "vi", "0d46f8ec0664fd3c699dd9d608d5644ea6628de3929ba8b4da6a3943df1370434d18c30c79a40aa2acc1305460dd303c2a4cfa5e561c46fef28e282414fb8ff7" },
                { "zh-CN", "6dee48c5f70511edf549e4667e4c547cc2e7fbf58ba0a4813a8a1aaf3a98a094917e894250aed87b1997bcddbde94498a4be32939b4e2dcb1188a9b5873cd55b" },
                { "zh-TW", "e922c2a30f860b5ac9fc24020955a02ac4a43a95e5b1f96dea4b950aed887e1e27034b6504e836c91749e09859d4d06d899bef25818db9e439e99b30324c1a47" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64-bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/128.5.0esr/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "146c8234951abfb53e169c12c54ec2808d1264f6cf6e487c45904d4ce916041dee6388584525e82bcbda0756a95a99cf3bca17cb301716456c6053617b3e31e2" },
                { "ar", "d5192a4d348ce6fe119701373f88984a127d8615304d28e7e905aa981d8fd79b537f3ac4541f221b4e18cd05ad35aa8c0b380c225b0762a140658c735ca77f84" },
                { "ast", "c6d29857fefab49c2e365defbfe6352c1c8ba83af08ef85a59341de7b810e3ae121adfb617b5cb874af9bb8fadcafd1c558ee9f1f1c9fa91e5f8a00e52083bd0" },
                { "be", "844cd62db29dd465ec3e8f512143ede0ebbd7a2cc11d8ec487f387a3219dcec0a10776e97f8a518c3180d94784031e9805dce06cc9a8a8f5e1f47ac6526c871c" },
                { "bg", "278eee583d88e74a7f0f7271066443849e959f353341e3a63352ac36dd9fb21b42559ab9056e5330f3da14792534ff5b382847aac495688af8e230123db919ed" },
                { "br", "215eb03a77fa1dc555e8a188dc7600546390c3a56854c017d8e5c1ee5c7562f0640e97ca5588270e447e700b790de927616b3619784ebcc9a4f3a69fbd49e9b4" },
                { "ca", "f7e20d09566d1e36fe492110980dba2d42deca05e2b2607e5291b74d74ff169f62c07f3f641a83e07628dd53c8f9a608dd65d871ca94b6ec893be6ca263f0574" },
                { "cak", "d45bd602ccf4d9fc4bb7fa29b0131f737d68d529e2ccd74da6279c22d18d7491bb8e53c8d42ece4f6703e95c2eb3904d016c3014071ff5b59aeb13ec5d1fb2ee" },
                { "cs", "2e6a8eee94373507baccfa97a427e08c4bc420c0d92f2a42676579f96e170fcd2144d67493557b9e8c0dc4a9facf7c6e41ca4436eecca477a1f78d7f7acf7362" },
                { "cy", "7fa169d7c09a826f06040fbe14cce180c41a3c5cdae069dc7061aa98a35858ca2dd8239dd89cd11d9e3d43b597a8903fec3017a7de1bdc911ce41ace6c41db06" },
                { "da", "b563b6391e343f1cfa9a96f0191a0b1fc482d8cbbc7499b161ece6a55e2614bf142479c78737a5f53d1c0ced0f3e6cb87040327e9c9246f3e02d7b85a9ff2eaa" },
                { "de", "a2dffb3c79fbc184b3e236055338d1e6f0866d1c207474558306b704e0232e7fe27dc3fb665bcc2289cb037adc11028b86a39c5cf27f30608ad59324b82338a1" },
                { "dsb", "f69561b881cbce23aabd4764f06909487754144655bd7667ee3b22dbd0e897822ab5a61171e75dca2702507150767f831fb2551d966d365af854067b91833d3c" },
                { "el", "eeb9c47b46966a252aa0cd8560473c9a177daa5228e524f5efc4e9141104c913de3f9805ec8b3dc7e0885e6b1d9957e1080faa9e706b4e744df1b1def9cd8df2" },
                { "en-CA", "1f75571bf90ec94221a5d9e2a9b3fcdb8bd575823ad5144334bb6a6b6cd5828d086b3a095af4b4fac84798e23939a1d60f78cb00ca3dc765673e746014ed6e17" },
                { "en-GB", "11a16d83be47026d0f2dd490ee66c0738cbfcadcdf7c182822b6b6de9e6469f33d2134eae80fcd6730c119ef49b4054ff12251e90e88320e0ad7759e18f2b2ea" },
                { "en-US", "83ca5ece8036f1676d5b6e4c793d20094131ff8a1d986639f2a7b81c4a613a134e0e9e5923d9beeff031645f6284b94dd08f616d2e60b2c2e1d25a95f4350af7" },
                { "es-AR", "28a1df42bc8bdb44ea138abdfc479a9aff348aa1e1ccc5952c39e7893cdbb6cde080ea070d996093596375c0f33307958473137daa7dc4ed515bf73087b86284" },
                { "es-ES", "f8e0831a834a09a0c45093bb775a0adb66a611423366ff026e31ce1da0d077340d4e11840f3dce16d1a45de7b4e3947f9d3b02aa6785b878c1c0889707f2f7c4" },
                { "es-MX", "f20df6b4fff8d3d3645a4cf5246c0b39bfaaaea63243c604267b43bd374990e670ea72cc9da766c0cc1558e92965cfb80d2626a2bb3b5aacece2e05e33bd7043" },
                { "et", "0b33de7806db62de1ede7bc8dfcace5963d59e105392dfca3cfbf918b37bb3bc70da3d0a2f98977f26228a565d79667371e2e2816c5b2e2a88e7685bc5810aaa" },
                { "eu", "eb08d8c1b5ea870d2bfdf612d93e3507d9121c8609b8735f597ae7669559fe9ff91e578ef7e9462d4b5fec975e935f48c1b7a6b36a23ae492ed0e2bef1bb1fc9" },
                { "fi", "97a045889f4d3eaf8aa5f244d05997f40a7d5f54ccd74ee2e5059d618eb6185044a7d3a976f4c775bf6990adce40c279058d0d00168fac492e76754f1023ecf0" },
                { "fr", "6db2dfca169cf38cb3b41927c918660428b82f4a15f41622483b183daf3c4cb2bd060d47d0187e5b975decf7be104eb2feae0761e1202f58cd0465d1eaa6c986" },
                { "fy-NL", "d8f3292665a0eb058c874e15127dfa75c83ea5438846020572aac3e29b559aee3469c6f399b1daef7392344769f8ef89d51b4b139a4017a912a7502ba846d664" },
                { "ga-IE", "8a72ca82cf0c1f939a6e0e7df0c5bb58176bf9a7c6f1e24a9e2c912a9ad39178bc8972fbf63af0e5e277a3cb2ac2f937b9d7298e14cde33a1d3d94281d8ac352" },
                { "gd", "2d19f219bedd46cfa5c6bd918b62a370c7ffd7531189502100aa8f8fbfc9cdf7499c9efe1824ed8f39faf919077954aced370a6a7d76219520ddd095ebd98387" },
                { "gl", "5e6500c10fae65c8d56cd03395418de21e88b8ee9eafe1161af6fba2007ff608efe94f00956ba59c31792374c37709fe29e90740068109f074a985ad02fc5358" },
                { "he", "745b789224162207635ab468534d1c75582109d00e29f51ca37565bb269e0d490cf8951b236e609f7986ce287aabaa3e94e115fc82b48ee94e01fdecae277d2a" },
                { "hr", "11209170f39fa14e32c8d46dcc05df2322f056e82528fcceab9747fb4722c273c3c8e9bee8b4b16652d2d0c244e3494c7c458d1b80f78b9b1d2d04adc89bea7c" },
                { "hsb", "4609688567a580223fbdfc6c173eef82f4ce24c969b69e6657f694f4bf12cff94af42504be795c641858c044869f87a9a28cda90182a4e5db4320ecfee9f3ee3" },
                { "hu", "b1657e102fa0fb17beb2223997bc25a1c1a0d287cc0b300c87e82e9af27987e7c08d80e8e7d95b7220f3b974ab634b530507a449d17c9be0bf46e3a0d7b58cda" },
                { "hy-AM", "211270073f18b9d9c5b2ada73ebef5231173d02ed34f6e78430a098e28603c6cbf1ac93ce21c9dca5d59a6376856e3cc612b881b7d1f9e9b908f0d82443f5c8f" },
                { "id", "f18f59b1f40d1b4a63c710a9fec6ebd569fa2a355042cf9808adb9c16ad94840680c23432a7b0b7232cc375a35e5bee549700318f28cc2bf4791e44d99097d73" },
                { "is", "67f68267b52161e7f356b4b3dac8dde239f1146df2ed4d6041acbaaef37bc2350a77093959e3d7be9b438de5b296c71206872e63a2408e6062d6224edd3615fb" },
                { "it", "125722f0a8c7e2b3a7130ee06b09a85ded9dc059ed8cf488bd2f26b54870425906caa37c589fd8e658274334cf87f0ef60107c9aedc07564c75c86e1dbb34004" },
                { "ja", "42dec095a5048a71e2de7d305ba55568e7ef02781f1ca761c585491b3cb785e5180681b0ccf9ad38cfe5ea6d520f473d181e0520975498bc46c723dae263c026" },
                { "ka", "2615c6e20ac4ff9e5477a763814ef889d73dfefab842f4c90d9dabfa4af15633fd8ef4442c5db5a200222c8b027c0137b3a54b709d55871e5a793aaf20fb1bf3" },
                { "kab", "bc13f158145243e490edfced435552bfad8ee255da1eb81cbdf6ab83339a536b837ad6b3852f1e7c36821353e11f2ec22d43030df296b7059d38234091dfbd7d" },
                { "kk", "13e65ab7f3f3e5489659780e9fac0bd8664008bd8b0b28ff8111289daec73ef9d613070f5650937d91236ea878de4613a112a3938bc3b88802548f1c5c149e2c" },
                { "ko", "b75358a1bdeab3666e3cd89131b435280f24238f7010af08414ba7476cd605e10019ff8be0e4ee441ac99676333787a29ce57228a4961f7de1d31a2b84430697" },
                { "lt", "827c876d96d60201dd6ade281e2b9415b013496c1b48a3a9659bd01a97818b690facc4cdb6822803cc6be8d5a32fee151cd7df3471ab18d0e0ee1cc65dc22c17" },
                { "lv", "44134db05acb19618b9f87a2dab014f6d7d13f7d975a139a1821f0ffb0cbeb1c194db72b8849ff85abb1ef1948769eea0df0ee0d02d7ae487a713e7a6993bd8a" },
                { "ms", "b0ac249e1671d484ffd4a1974cb86c36e9d789e788f70b2ee1fa1973748461c15119df029fb4a75881e937067620fad98bdb799ec40bbdabb877e654f3fae971" },
                { "nb-NO", "9c396a16b82490bc13c63e2dff6e5ebf2f710a7e3a430147ca69277c412bd345c625ba9156145a307ca2f2ab40250d3ddc885b19ee8c4a7ef4e0de6edc036d7f" },
                { "nl", "b974b09d38371e150645c8458b68824603338e502f5a69512267f3c9f7a51f2582d7d4c0c9fce3eb7d1ad5b9a5dcc6de77e59a987dcbe03c79a54d10985269b4" },
                { "nn-NO", "f5bc24391f6ba865998c471986316dd5b12fc8701edc1de8f464bf126a60e13119c1e5be8841431e830b26425c2c2cb7c76cf6a50458fa0ad564ef9e4c719efc" },
                { "pa-IN", "f69914aab6edc966684c32266c22e0ac5dee08a75c1e233793cc0d44075259f9f6ad2dd01adf617831d5776ee834833bb5cf1d21b4acf95a66a067ba811b5de0" },
                { "pl", "cdaf36f6977ad6f32790a718edde82f8bf5883eba935c73c80caa8123f50e2fb91c018ccc514b48621c8a92cab26a0da5c465960fe604fb3e260ab37b78cfc91" },
                { "pt-BR", "ec3ca8f0cf0e8b04ec32bd423a5cba35f0e3d9f5d7861604c6fb700c7038e4bf6237931d8480215896a003af3c1172953f23c46b0025bd5a1b39ccb060a0d954" },
                { "pt-PT", "e658b04c474db368d3836b5daa53117a1ac72c306bd9790b4346908efc59c1cb7c26922a7b90896b665800beb5f9a59fccfe40860a81d0ed45f56dc7bf96ed16" },
                { "rm", "f55f17bd1875705bbeecc2dd50b90ddb1ebb896d07e4b666daf44aabdd0cfca68df01de6c0af8991132705d29500ef62609866aea2631cad722d4079befecb02" },
                { "ro", "a885e69e6bbb795b34cf85118fa1cd5dd11faac58794a93ec5b58c47045a9da184989f5346942dc948de4505b7125c2166cae78c3a3e8900e5b339e35cbbdf22" },
                { "ru", "281f6ec2a7aaac4fc5099776e691eadcf860b3a1892bef4ca838c9e5057b74b8d393c31cbcf8668e9e835d473bab788f61235a8c31fde035f6d1eb4b03b0f011" },
                { "sk", "da05c4ce69ced951cea825d76fedaca21678703a34361f509c9998d7b672d6491f26b4765ae487deccd9ff77e005e4af5fa5119a45d2c58ebc2db2c92f1ed102" },
                { "sl", "c5013b428cd417acab8222c20764a8f5e4e67aa87b376e81b56d8fc12809a188ad164d5059af2dd2aafa78025afb731571921c5a2e2bb96d30cfdafc647a2119" },
                { "sq", "7e0fef599af5775d1a578e5259ffb1843633fdc24c478b604fee5b98c226e537df3aa499acacb4332d3fd22652fbda5a35c8ff9cb28d826562e5d67d221b8f01" },
                { "sr", "5b895498972d9c99e132372b44aad50c26f2fa536dddbca17cb0f4e46e7aefdabe5a0180ae256a00acca1c002d85edb37f3501e1392cecb8f160a043b4589d44" },
                { "sv-SE", "71b8138900a4a37dc79407b576dc7bb8543e6ad8f76c7eb4ce33c274c5e311fc37fb999d52ff952cab3bd27ab9c688110b060ed14e823128c84f0e06abebd050" },
                { "th", "7bc0c79ffc47be70d5e1679b1c21642cfd41ebf15a68e81417460f86360dadce1a8285c79c9ff9f8a049de7f3e53cf4871c35a8a1d159a8fcbb585d494db6dee" },
                { "tr", "9f55ff76931b0188e6fa2f6f02120467036dd4f081d542e67d17f394210c99506f5769a59c08eca000b9ac455fad88b19a0d139c75ca0b3ccafce0e6f4e08d20" },
                { "uk", "8e1826398c2112c27800fa4c45d80573b2dca7a8f05cb42bd380543468444052b0b883ee88f96134ec2a4ca4ed746ba0555561caca5813eec89c7df56930fe76" },
                { "uz", "48e0f1b18c188d2219c07ca1c5952f9a267903bf0d3f5a7505c4caf8a7532f4d3ece2213046b5d10ba59a27edbc93e6c8a0155b9282c9d881a00f388ba2ff9cc" },
                { "vi", "373743a357f2149d9ba8bc83774f44e6a99075f7fc305fc7db2732231fc85db0256c97d45d978bad13e276d67ed81f2463277860060b02c3d3a7461a84f32844" },
                { "zh-CN", "d6b2386524ccc407f88bfc5ad4bd19bad5f13034d48ae593bd6f7e4389ca981dc659c51144e33fc1fb05bc8335a1de33dd658ee62db01860a0ff54f50c436b21" },
                { "zh-TW", "65d855b7638caf9eae0925c1841e6cd202eb6caa2da375154e017ff24ec25de633108edc707858a404a34b89136279ab4706b21538112e30794d32391d8c3e1a" }
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
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                knownVersion,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + knownVersion + "esr/win32/" + languageCode + "/Thunderbird%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + knownVersion + "esr/win64/" + languageCode + "/Thunderbird%20Setup%20" + knownVersion + "esr.exe",
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
            return ["thunderbird-" + languageCode.ToLower(), "thunderbird"];
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-esr-latest&os=win&lang=" + languageCode;
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
                Triple current = new(currentVersion);
                Triple known = new(knownVersion);
                if (known > current)
                {
                    return knownVersion;
                }

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
             * https://ftp.mozilla.org/pub/thunderbird/releases/128.1.0esr/SHA512SUMS
             * Common lines look like
             * "3881bf28...e2ab  win32/en-GB/Thunderbird Setup 128.1.0esr.exe"
             * for the 32-bit installer, and like
             * "20fd118b...f4a2  win64/en-GB/Thunderbird Setup 128.1.0esr.exe"
             * for the 64-bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "esr/SHA512SUMS";
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
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return [
                matchChecksum32Bit.Value[..128],
                matchChecksum64Bit.Value[..128]
            ];
        }


        /// <summary>
        /// Indicates whether the method searchForNewer() is implemented.
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
            return ["thunderbird"];
        }


        /// <summary>
        /// Determines whether a separate process must be run before the update.
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
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
