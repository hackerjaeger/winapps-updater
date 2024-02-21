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
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.8.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "88814b1b6fa1f14a5085b050e0f41c303f255bbb8fcb3cc5ecba0fffa1080946fd7c1cf0d0f4a000008a16390b53e892f0e3a205e126ca62e16f2c15fc636d3b" },
                { "ar", "bfc864e639d28583326f54946e3320c022a32fe8f2e8415c0e886311b47073a13cac3aabab30f7468a149d2c54047a68ca9ecbe0124b7cbc31128b9485f69670" },
                { "ast", "3181d61933b07811b3ce016362a56fc3872fdaaf9d9dec54c4e78d24301ccc64124147facaa32d0f6183c07bb96127ebabfd41bfb60ffa663de09224e341c0a2" },
                { "be", "bfe144fc03702eb24a98f7fd8a5a1a3f1be801a3d3d1f2bb1d854520905eb46a81020af2149b9c74d0e4de794143318dd67c5fb5bb955e86edb0a7ade6adcacb" },
                { "bg", "c51f20f9d80ee8df1faf0282f4e85ea305f00cb5a8882c2311166c0f1062549626204443e3d64133114941c99493cff8bb9507b66072289a77b71e35058e0057" },
                { "br", "f41faf64465c1677d48bd8a8183f1516f50871bfaa713e26c64a57f1025cf15f4730d0cb2d6fb046bd91c602a3ea75f3b76f829fd1c27e14b6cc13942380895a" },
                { "ca", "86e18328894cb41944ab258393f9dd033d2998f51b398f7c78dcff740f32551beb23fb6188c7e961c562b2d53e4d12bb318d17a382797202bfd6402b371c3821" },
                { "cak", "ad9ff88df5c75294d013ee5638eb3214c67ee995511c5ebf4ef48fbf24380ed3ca9f538cdcf37c52552b1d9624fe0c6e80c2ae9adb23a1e2ceab7748807cf49c" },
                { "cs", "ce3f4060f5d6e99e5427771bbb4723a476a3b1cf38877fd1b3e4c0e17d92c59538b2362759e40d8de04dd68f14372563c57481303c8e9122141c490dedf19872" },
                { "cy", "0f1739b1990a192ff63ef601ecc5767e26425ca67339b27de0c97747d41e2ba87a59d7e751279ba0fa2a4a45404a2845633e84be423c623dccbf73feb9587541" },
                { "da", "381c25c9db61eb6892c75328703a0a6f30d24b55dbe9871d229484d5fd548f1e7850a3d747bba56ea02e69017c2d288a4b953181626797c5c39955bd6bfb4ed7" },
                { "de", "977e952c9df66bcf373be3c956f923caa0afea64d69436fcb0796b04021d61a15644c20c736e775f7f1b45f1871a50fee9ed70435b28c71138a459c699642295" },
                { "dsb", "cc1feb423b772150bf469bbbbe156927ac54e23610ceb1ba3e755112b3fb5a96881ac29bdc9f3c2f9c3319d439b9f598f1fe206aafb227e525d86c0628540e82" },
                { "el", "23538b18e5256d62ee9365927e68cedbeb7112b7627747c74ab556bc8728af55fcec9fbcd1f7eb0940c71b8738f59aba993fa6f95cf355a3198ed234fc89c8ad" },
                { "en-CA", "b3c414040ccee6d8536c736b8162560c27a35e9b8f63806f4cac637a10e02d48f110b6db21f8996a010809edd62013e59df579b84931eb4c73258a6908a5172e" },
                { "en-GB", "7cb41cad8ec10e0b9c8cdda7b1fe3342abf7162786dca5dc54bb67e9423cd12b0c3eab17978eb61668fe8a1a802ba8637774a86c01d8e425bcd9e32295fecc2d" },
                { "en-US", "ffc346d65c7d453d309cf59415ea10e13b6f68d27b6c4514938b9a2044f248c87325aaf821215ed6a5592bc3c7067ff2596c19bf989be486be8de2d7b0b1d03c" },
                { "es-AR", "75b7dde50ab9a884d27cda75e3e7bc68522e8d1e8030f324f1cc4cd7458d7b8a4b011d094e6419c4aa2e62588b6c4de80a69cfe753c269b3e821ed758fe5e782" },
                { "es-ES", "4a619f1713825060f11bc87f08bd1238b62193b98a7f62b40d2fb124e71ccdfe8ebd56687ae9daa0b22ca1c0d808619786d97abb55ca15dfef296b23cccafeb8" },
                { "es-MX", "317653f2101d1febb9e58ec33bcdec010c906bdcdd03bce011da66b43fa3de2eaffec8cee29ecd32c31fcfcbfa9c7afbad92c2de9e4f6d5ee01f7de054d277a8" },
                { "et", "fb79eafe8b2fc43697537ec1be622706d40b792c863c12f7278e8dc51c88f5b5291546161df3533a07ba987667e2d228b939abe0de8d23a324a83b0ae72439b0" },
                { "eu", "d2fd381f100bea3c2b778999d077bdda93490dd763df0a52f3185d302d9d4729d4679f030e2b911e839f4dcbdcf0cb958a06e1d2680d3eeb51a03026287e4f9b" },
                { "fi", "62704b2a44dfd92010dfa227bdbb50a8fbe0dd38d2bbb863345e6fedcf8cfec86d3475782a915e3db1b3848584e1a1389ce091130c7d8287cc5bdc4cd0ff63f2" },
                { "fr", "d42c4f0a502297a78dbdbd765df69779f32a227ff067c0a90551c2a00f332c7866e6000e40d92d290f80e908015fb1b127ffd3c566fda22153248ec2c308b1a0" },
                { "fy-NL", "2944d43f1002b6b725306fc4bebda2f5db9851544a3838dfb65f2c8bdf59869cc3d9fb5635486f955f902090998f8ff2d3fc1b7ea87b8d98786f6c6850509cb1" },
                { "ga-IE", "9be6ff03ed1f8f825c38abab59ccf330198658cf12c4d8f605c6366802e53ef82f6d56d0d02fb08682db6947a4af703d4483c83d19b605eb0a2e2834abc28716" },
                { "gd", "c4dcfb75a2e344634726168e3be94b9ef9af5d72240f59adb7b2de4bdd042f54f22a558dcf1739a0e12d446c1792c823dbdd6bbce997f9a40b70f0caf4be58c6" },
                { "gl", "46b072f14400a60c13c5c20f0857b6a41f6092c2293265b392e11e279e827b573d83d802c67e3a26e8f1f696d15efa57fa7d9da551c15d0c5565228b2df738a8" },
                { "he", "a2afd70c6852767460c0498d30111f9be1c8d5781546c76efb5eff909ccc20771813a3e68bd1fb8568ec8f2345563f67d94f2a71264ef827b17765ddae12b726" },
                { "hr", "7fb008d88c85d571cb8cbf379e1bce51750c862e9462264519fa905c84f2faa1a565d0e38bd8f6afb261f0fafb2b518cf72396f33bcd638b6c96ecad5376bd40" },
                { "hsb", "1e497c4a5d81f71cf80e4306fe8b2fc302df105458d968ebd7a91b6a65299853fe4e3490b7eab92c80d445fee48b40aa2229445ab7230d68d63a88dc2ec43774" },
                { "hu", "1087ed7a395cf82c47135fe8f3b6a8badefa16bde451ee0e3dd8b3c37cdfd23e284301b3adefadae821c2318dc105a6a5ed314754d1442a4198d07fab1b53b93" },
                { "hy-AM", "e8d9fc1ac3a22bff1c74e0ca4af8953010d521c19c93cdda77329ee75c118156b6b2bbfe3e0abf8dc414da38daddea9b068cc312a7acebbea90169933017d8fe" },
                { "id", "215c3752e92dc2fe9a58aaeb9c3d0b7a68a0bd09032ecb4c9166ab6aff85b0390b707e111f69da3c8dc48c0d4e43a984f99c988954ae2c092b937ca1bb2f8ed2" },
                { "is", "43ce5b2bcf9eb7011a497f2a4eccecdeac131992e8ac2e4886f6d7e51b033c514d17485f6f246060e430e4b8b121296d7d4b777935b0d13f343249dc776b1a71" },
                { "it", "d53d5cbf8ae8495cd8d35f187a57030b1c7a846a711ddb5b1de1259f8f68be6c28a064a9c0d540bd01a82bb9d9661b84c02af6baafc02688e673fbd456e52433" },
                { "ja", "2284170b4d16c739720a43867635a002dbe984f67d45d3b88334871ce60376ee855df0bb90870c57aa6cd6d7dc194a5261db029de6056c8a4279fb8ac1a33d35" },
                { "ka", "4621e001152b20bb98549e0afb984a1c36e8fe00cc1efb3f22d4154aa4639d2faee7db4baaa7f79f8d4f4ad6d87c46f15e217ff8094c1c87675607cc18a28904" },
                { "kab", "71f184505379103c848fef66dcc48108159d6c7b33b3d8ff2290e77fc23b93e9581996bec6f911e3d5e831fa1a4c655671bd1402764bacf2b7cb717732ed67fe" },
                { "kk", "59de94851d7ecfde86c7b412a5f2efa4606ed2166a6094eedbf5cb627b814bb6a106e7a03e06c392bdb48ec7f49f4769f296d7aae4cb6edb681adbd384935b28" },
                { "ko", "ef213215e108b070a04fceaa24817f474ccd0ae7982ba9a5f2f3e886fe7cd41e8648efdb4624555a4093fc92d1b4786b8823ce74a7a24201906735f5e8e1246a" },
                { "lt", "19a92f5c014855691a3a494958d357047af1bc5f66d2bd03557f0e68909ba61847116ff5069d7256f25ffe6f4f8cb7a6f9c87c57c48147e5377f9c4064fd1a37" },
                { "lv", "098ed15dc82a3d553660810c6daec011d271491b5c50f4b4d3f81ffe21eb1cb46b031267da0090f3213b9ab23cd74a20dac7e484a025d83c1802e5319c303afb" },
                { "ms", "07c19a25b5723f03b91e728ad7e878efa6180c273c8324f04db19b533dcf432fdc737d676719485c0f68d2c4d89b6747975f9138de89a471797a336a6c9030f9" },
                { "nb-NO", "c5ce978341a647dd91a2fee94ba48f46941e14d6b906f0fbf1b1fb3f0edc0e2828745aac213246e006db4dd888a47da49008afad5a688b83ae68412b20ddb758" },
                { "nl", "ccdc6895b3ac70a57ce3c3beb6acc2d156f0b2712359cf909410569000e4c52bea9f51e1190ec3c84633d3eb47e1f22206b36633ddb851a70e04e55a3bbc1b6c" },
                { "nn-NO", "7452ac11300afd17bec8d07c450342f5fc6c2832e01f2116b0f0d9a27d750b81dda0c0669d520097daf81c2952b5f1bfcb312a4926ab58fa6adb8d5b46764b77" },
                { "pa-IN", "a99f08b879277972c5dc00b67a9187edaa7fef4a20c5a15eb3b32be0986a6cb0d0a522f531c30bdaab292b3dc7afa4e4a390b42452e488d04975f10081de4887" },
                { "pl", "c7bd2d5af31eeef261b8bffd19d666be9c369c314326f7ea1093893732dc14b853f6c5e464fdce606d81d151deecd7e5c696ffb052e4129d58b6f452e83b6d2c" },
                { "pt-BR", "bb0e4867995746eb85c913ac53cc71565c5fdb229c3b6ffd22c3a0c92698992ab58d8f7a67dc658e3865b148ce37ab2dc6a58bc104cdc44e5223b71c64e3e752" },
                { "pt-PT", "5678fb691cf2fb5ef66b3983fa19228e61875059b96e3a3cc715e4068a24cf34af58f5d48517e454bc6885a2c93fefb269150f1967c14d605aa8494823273ee4" },
                { "rm", "7b86e18bd4f0b84e0ad485f4f3f6b15708b502bd8877c7709636908fd34e4aa038f16463e6282f03dd4cc1b85310d208a0bfb826841c3e31532df76ec471cfd8" },
                { "ro", "322970660b18d4c23777075a5e90ad4de8f14c04bbd9249f7b90d0e1e4f981a0e37597e456ecbad2ef55243c40c9540221449171f1bfeea643b7186ccc7a950e" },
                { "ru", "08367edefad6a7ed0e1b06dc2b8a53c5183ef01d344aaac82dee897b6deac31cfb1ab3e6f9314c7c5c90a3b67c15449b237770817a6a0f2e999c03eda03a312b" },
                { "sk", "54ba7d4ae21a622f1efd067c6261d28af93e8ec14e219fbd88a4a62d15e7eb51663c3258dfca28be352f72ea5136333f83f8fe2e9f0f5ab39d3229bada592fa2" },
                { "sl", "598d6c1c81d832ffc132e0160a67e4b3e321e98b850c7177e6711421279d88609c8204cc571eb7140436188c6fc86a4a07dab8e3db0da011115986bc47be6ee7" },
                { "sq", "b9fc0ee291d89b51104882e5a563c794a61acd9a7d78165f7d757277b32500b9a83b212e03e1c5eeb2ce2760e300de636e13eab4752b2b79a627d5f901392d8b" },
                { "sr", "71a9323c4297abc120f64946f3b44c39f5d79adae02ad3196a17e4ac7b6ef05848a69d54d185624190a90ff21aa892da1d5c9b33d04a951169bfc958dc181e86" },
                { "sv-SE", "e0d2cf8d80c690c7d887d991788d53ffbfc2bc1bef1e75ab2140c8585ce2122d17ac075dbd5d664c59ecf7559fabe30e3bb3ef01e61c883f49dd28bd11c6f2c4" },
                { "th", "5bd00a472388c534af49dd30d683c8c31634f700e2c0124677b1be00ecee980d7eff9bf09f0283ee87785743a26a4af56d78eb619f1006e43679e6edc91a9c8d" },
                { "tr", "7e5c209a7b1224239f8239e460653bdfe2ae5d773600635e5783f366accd1f269d367c70f98bc20cc25063b3ca2be739b9903da7a87ebca37855574bb28907d9" },
                { "uk", "aae2f1474519a9209fefd5d9884c3890a622d05bdbebd51b125daa397b3eedb8c7e10a33b43ff0e4b72cced42ba00782c54e7e2029442c36112857ad414f7659" },
                { "uz", "b234341ca16363b03b463bb955c2f403fc4a8fde6cf2ac8cdb6a35ada5f185510ed39edd34734364a1454d9d7ca5ae946dca5575be907a4728a6039d4e779bb2" },
                { "vi", "9718901d122194418095ba0366da32cd8128f8fa9debdab7fd6a05de56b148e3cbe29c7c3a2436b10baaf7ff8bc60ca86071ad3a10b0b4c0285bf5725f9d20ef" },
                { "zh-CN", "f519f08b7fd9a3ae95e98680edff8529575190d18966e370d79d4318178f672e4d92fa6c482bd58d34a17ab801724bd0a5a95a5ba05bc1d018c21be6611f671e" },
                { "zh-TW", "dfc4edd838b176f2428d44603055ef1e7d32efde55fe89f21e6ec85802779f498b9105913583d193d00212ca2bf91f50ee9bd8abd8dff1df8a61f888ab56d511" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.8.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "42de83540bc8c2d5a622e622e76df53699e73b5c7ec47d7836f46ae961d459cf6968d39453d895680296c513e5d9c8667b6a4883089badcd4ea30328f37f35ad" },
                { "ar", "cf6d926980a53350167382795005040b5befcd326e475b4f28842c81dfc401d89c30a9024b420458c5f4d7a993f10c1cf5355725c91ac6b98e5c7d1247ec43d8" },
                { "ast", "2d8de7eeb0f8557793a0a78d12932858a5ce6b53bff4db8e6c3b7a8ad414f9651545245badd3064eed6ca98b3c4f40cddbc1c523a27052cdc51cf749ce6629ff" },
                { "be", "88001eb56d8ff498ed46738003eeae4cb9363109c4d71588c1d3c2b65d027001aeca7d4325cdd0ffcca7ebfc1936e4747526cef8cd7c1f4f7dc2e2bf8bd38af3" },
                { "bg", "06db3a3f1fa9b51e87f7f4cc28f88b8311e44321b5d479d81fe2b5f51112309ebdf246a24311a83743896a89ca39594629367463736e4f4698787cb8a910fce7" },
                { "br", "5f06944ceac910baa396fbfbfca3cc273bda2f6c195ae05a01fc98754ad3643179866cee6f7187ebebaaad83894cd7d3a2628dbb051de39acf5d8137148571a4" },
                { "ca", "59d2c73dab35cc07b591b87ea2290a800dd3ece3d1808743eaacd030534b090249e2f5cd7a73684819ad9d5061e5dac1face7c3cfaed0987157d0fe88034bfe5" },
                { "cak", "c8bbbdd986290e7a85f09218734ee78a661fb902092cc6f6945f86075e8068c90866925cffab9b8cf0cac01feb80132ddec5d1d601db739a0a61e511a380627b" },
                { "cs", "546627f79bf920b5941781a4e2192914785902d5da2a4b4f26e58edbdc82366e191d9f2a557565a168e823ae95285df3ae11f80dead42ff841277d5c55e53f93" },
                { "cy", "f75ff98323245cb67640e35e5d6dcd20f38b4bfb4961717c7ef034c6bab97240cb16e2a4f332406075f94ecb743575b2fb64a99f35849d36663d6ffcfbd1c866" },
                { "da", "2bdbf997327d1d0209b139230de535cfe2ec0c618c60c8b671b1f2b7cc02798c31ebdfd9fbeb88603f8275d60bd840939218d5a4ea59d5c16cf3e104240f33c2" },
                { "de", "2b507ea340640cc3b27022195e618b94d21dddcde911f734f8a44d847a88c355a696489e478b860182ad7776fa3c5f86909dbadf55ac0fdc3d0b05196126d81d" },
                { "dsb", "c202338a25831263d832ea5e89d011bb165728b8a45a5efec35a222d3862c480a54f5505fa9548226058d6e19790765babb69f9651338bb9782700e935294a5a" },
                { "el", "acb3610fe631f3e0f65f46d0620039ae9ffc5d43f29200b7f5762728882953921a8efdef254f6ec03a8cdd5ab02fdcb8c820ca55a0ec8af0be343bb5e716902e" },
                { "en-CA", "70d59460e709a128cb16f985bef901b6be29d610b609ebab19c73aa08fac9fc1462d70dd6a277937573f793a561ac5cc6a637ddab753f34307234dbab176ef5f" },
                { "en-GB", "0809a29f1e9b3f0ca85099f8fc0365bbaa97fb1a034c13b1c3ca079d9b0455f2b74bf129727c2037bc85572cb300c3e835541a372a2e7be16f7994cb93068c52" },
                { "en-US", "14777873d10374413f1ff95ba3dca237cf6d7edb02a9ed045ece612cb0aebd7acb4e3d438c0c7e430ea4e42c11ad3f734fad8351c5ccfa9d6f7097328aa81adc" },
                { "es-AR", "6d82319abfde00d009887fd5cb6045fa7fe1302f2aab8d9e2d3c8e24feab2c0e6bd9054136efb59a7ab60f44ce1a3310313f25a8b42530c66058a514c7196bb2" },
                { "es-ES", "77f5f0158bf4fc4f91acec69c2b3e9a33c8941bdcae248be4a67f6925d2150499792834255253a53f28b3f17dace77d5a9249c90defd02d42a83654fffd98ff2" },
                { "es-MX", "ff2560ee3de825f0ef950b06ed35540e7d4535703997a314d5dc02f2f93b5d36b2866a64767a575bd56883c2c100883e3c35103f22d68156df698c16fc99521b" },
                { "et", "31fcaa97542d6051930d3a46797e0b2cd657c3936dc6dea9196975db1b2ec44c32cbe1e8fbc9fe3dfd77f725c65d6cb18fcd080fafc8857c7a69ca00778a443a" },
                { "eu", "66137acb6846b2e3468681a8e508881a3cc897bea97919e5db525815dd7177266a0d18d0cf4544542835740d7930661e1f3287fb3b7494228b9bfc97044e1c90" },
                { "fi", "7a6c1e5a151ba2eb62faed65662e9a6355af00ccb516e29cb65a2c61a4d9bc674996819f698c03bdebd1c21f21563f3ec6f34ff46b78b23b7c6c50405d1f538e" },
                { "fr", "f7244cf6fb337c73eb26b92225e29849459d07a63bb11e042c4bcf657a36b24b40fd6084649cece273ce407cc2ce7f62801b7c46875e7c8b2fcc2c3ddb30be39" },
                { "fy-NL", "76333f075a2e7b049c1756d40ec8f3bb3c8b6e6cec83b6804b18d2b53d39ad178e2137286558eddf4838bbe3e42e5c00977fe25a1d575b37f9384c433d4a7328" },
                { "ga-IE", "129b8f435a335f55feec085b86a5e260caec6b7cf6bcfac47ca85d02c533f470f13dee2113358254cd01a9c10105076437d02ddd69d5e19bbbed704c6d374fb6" },
                { "gd", "a7231834424c6ba0de5e121e2002de73146ad9e3b087a5beb634574f140b7d33d964058cb3824553764dd24e5711e8a315741b5997bca5740a7e5bd96259a76e" },
                { "gl", "ed1ee3631099e83cae9f6007e0cae41497336630ba5776f8bd6535bbccfaf459134ed041f26a047d13f7db8304c4f0abed3ce5e3df12ede43c4eda8643676aff" },
                { "he", "e553315ceb5f5e4a0315107546a3f2219d8291edc6fc003c6978d026649408edcf3575ba587f56a26918eeb0b8a5533dd1bbcf69a945f4d84e71218f27fc99c7" },
                { "hr", "08971e812bf9f9f8921daca9fc9e129e1fcef9ba29940aaa6547704f9182bcaeff4fdb445eaf99e99d5d3bd39f47e3c29ba63e65b1bdf0e664f8e76e8217bf3c" },
                { "hsb", "7cd152558cc866961b2919b785d9604faabd8a8fd3c964802f2e0ba34b0e8eb7538f8552d91246b8e6053aecc17b9b506e0042d8fa723f70fb91a0fc473502ef" },
                { "hu", "64e1cc17718d0ed59d2ab4f6c2eb4b740a69f125b47fa91d9a0c7c2d04f44280c63ebb8f5bd6c579ada70944ac2647e11fea290ab39b7858213ba0c98a5d3568" },
                { "hy-AM", "a710641edcd33f6e887a5ae08b7c7e108ea9d24f8ab1258b1fa860fb0885cdffca24ed89ff9c0a868f3e17ef336a751a43c669be3e416519edfadfc41b2fd290" },
                { "id", "592dad54fafda17bada9a791c813e0357f421164b59b133b9a24803d57ea874124bdb83c089215192b5bd2fcbc68562fa122f5b6a9b93c945c9265451ff53e6e" },
                { "is", "119752686114ecfec8fdc3dddf187862616d7a61493884d0fbeef01c4a76f9dee481cd55b8bc3731fc9ecec6a52b6cd4c90e7d8880e4b3c822fd619e51cbafd3" },
                { "it", "9d2f9bf1314ebf73dd21498a7a62926a88bed908278b5c39d39b42fa72906977cda2eca12874bb70c4d52612edaf8ea0ddae68177f6e2849093cdc0decb21b03" },
                { "ja", "17cc53c710c07d5a2c087b86d8e07bb4a6dacb7ab4d11fbbf6277bbedb69fb068794a9da29fae4cde36bdf0db548d26b315fafb21d729d88a5db7b410c9842d1" },
                { "ka", "29bac1548ec359fa876b75797449998f48b57dc532726d13644f6de63eb3bcca96e603c171d0f9c1a3ac5821b353afd8a5f8614e2887e3db50da3f262ccff606" },
                { "kab", "208a0d0785cca40ca44f743edbcea819a9400779f7c6151f43120504ac6743d4aaf6b40f3e16532377542de7c73e8e1caf649562bff03d3915448974c48f4d09" },
                { "kk", "7d303177f02cba091425fb86ee2e48e38b3056073e5eb5a0640a42bb0b3bebabdb708b23b793c82305ab09477082b557dd9a45b58665c424c67362f9ac5ba7b7" },
                { "ko", "eda41c50d46e4b1b7fa3d5a69c990002d59cb7d64f8db098ee3ccdfe77b25a72262c81c6dff4eb911ecf7d3471c4b139f47e7bb25a4b0966fb56a174f299fce0" },
                { "lt", "15d58b00ed3544ac4db4b565204dd8f70afa4c4cc38564efde766679e06d120f62c212f6b985bddacbccb2182d4e59b4eb871660a6f234489034a56913aba159" },
                { "lv", "dfedacf8bfef91be78043e2ccf8e43da670ebb5fc6aa5cded4b27178d14ee6436e42135ab02524ea8b46e0a9b626e9c43ca81876de7407105c270174fb97249f" },
                { "ms", "97fccbd96178b0aa2e1a469f5677ce76f70fb2c3f3528d4ab891247a89c00a79adde9dec3a23d34700cde45604cf723f6f9ac143a398b26a186ab86546d6f600" },
                { "nb-NO", "46f818f4195e8038d495a3084d8a8b212a1ba2d63b73c5b66b1108bb5113d7fba2db0f9218dc79dcfdcb9e1f768ea43ace8708eb766f4a393a144791c97ef5dd" },
                { "nl", "50830edb5100067553f3822883522f41adb74f57cc19959a2734de1c1b97864b31e2ab5ae3258d9136a35445d07f42fa37d287f9e2ab481fbef364c1191e9213" },
                { "nn-NO", "e0f10fac264ce08d58bff53107c0603d79a4a7cba32f1ffadff6c1b428b2936593c29784ae21fbc76cac5489e466e7299ff1ea945e64ae8df5c74336152ac67d" },
                { "pa-IN", "ffedb8bb21c98d3ac3683120502d4c1f082aac5ece7d0d4e5175c3bcb461d5d7aed83993fd180c7b63382edf220f81fd03631d497cc4c08fe436a5fc54bade09" },
                { "pl", "2c76ba17ca4537562daa483d7dff8f49582190b03989717a847ff54b1cec7f7c6dae48dfc589ff7fe9070fbefdd1fd06223abefa5e5e826bd4df93dc64b872a9" },
                { "pt-BR", "1cde87b4cac14cfd228c24cd45c035f16f8c56bf7428b3e9863bb7bbc2e299291836d421727317f447bee3a7388fa49f5e1e7e6433043cd3236320cebae59605" },
                { "pt-PT", "f9f7bc017984054466222f8adf728513bf27424b00fe73801ae12b15b4744ffa30b4c2454cce3e841abb5d0bfa06c17d352a56519ae378dbe2b4f30d46439e6b" },
                { "rm", "03f7d5af07d83f174cbbeca042ba9ae62c0d4bb24d3de9de04b54d894bc2214b4d384de9172dd972cd9dea216e4a1a0a0c45908f6aa30fcb01b037a7a24b5413" },
                { "ro", "99fa94ced5a014aa55bb9a34d75608a88ceb576896b33150eac64a21837c45e52c696d84a00cf8ab81a87d9d563949059038aa604fb1fbc2f9b14e3c708fa535" },
                { "ru", "c40c817fbea411ef1b9d89af9f2c425778a8380bbc668947a1cf2b3aa50ce974cce70098ea36e894e5f319f60632b2cfcb348f5cd79f8c54cb38d947dd1de8bc" },
                { "sk", "e896e2a731d2efb2b9d4dec58395fe55a0fb7f0190c883cc2251c4fae8821c2e83420a4796eb9de1f2f641c706373aa0cb15fe8137610a76c1d6d184e44577f2" },
                { "sl", "8009e14721ef1a9d650402973b0587011aee7606b2278b555b003a103a9575ab285d1aa8931c90e02927bc84276206ba327e83ff21babf33c1f2fcb28f882662" },
                { "sq", "2ea9f424d4de4064492ec3a0535ced1ce65bed689c20c1c9b25b6ea09bbea0c6e8a339286618d24a4cb5e62d566bb94f867694993a504dfdd0fbea27792f3107" },
                { "sr", "f5c7b4cc3824ebc43f0f9f99273434659e698fb8c9071fcaa8412a53a05d493e186e9a82b799fef8889d6ee811ec338728eb3f2a1563c35f4444087cfa83d45e" },
                { "sv-SE", "08395564497c32c392cdfad3f504675ecdb6eebd9cc370b943d02363c70ec58f7c80a2cc493b7c1ad463094d5761950a156d3200f7728bc346da54a42fa8a993" },
                { "th", "88aa80c3be79d585265f6ce60e278662ec7732609b67f0f621ff28e75441d91324ace626449f76708b5c41332d53ecbfe804474d69db0f1ce4603ec288697b02" },
                { "tr", "33f2859470cf71e70d32bc6610f65e1161e39c102b8b47f885104cbc7afb6bc30bd7eb316db8a610162bd5f9aa18c47f4a52f66ab2401b2772748e8034564ece" },
                { "uk", "8c61ee5178be7f613de1a595697fb4d3202d70cfc344362604f2ea46abe0a77927220a57fee183fc369d77c9629a2c218905807ee79eddf590a632ef34d40b40" },
                { "uz", "20e76caf3d4500f5487390030f3b554ded17500c1efa564b36ac27f11ae4d5cede22174d2822163b2400269c87ad12dcc6c967f5057d684d9496860aa587efb6" },
                { "vi", "1d1722150d6ecd4b36325c936101ad85aefd9d6bcabaaf8cff336473f19d0718565d305c4078e547ff30219dece1dda81a7330bf773f55c5aa7fc2532226c45d" },
                { "zh-CN", "1ee7d8b0af1391f2fb3db76aa54b81d7276fb7dc91ef5e9a3597c455bd1ce868114ac4824b0b1a01328a1ab386e436e3680a60899d075e302b10da452a12b3ec" },
                { "zh-TW", "c88ce6420a3af5f8beeaa4bcfffcb7af18fb32c9152fcd5a2c599bdac0f90a59e12c2aacc0ed811a970e4d55ee2b41d9826e305082e706e9c184e7cd8de8cad1" }
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
            const string version = "115.8.0";
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
