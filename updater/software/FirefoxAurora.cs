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
        private const string currentVersion = "113.0b5";

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
            // https://ftp.mozilla.org/pub/devedition/releases/113.0b5/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "3660f948fc82135f840e9e8418527cb552da3de55d973adb0f0aaf5c7249fb3a6dfe230ba88ee2a7e987c2a707864571179442c2d633b90589c899e0f6375e74" },
                { "af", "a8437386f46a0ce8e1aa0d6cc790f90a0851a2013a16ebef8a86e6215cd92083aa1fc79deff3bf79b2f396a9e1716593b2b7f53c7ac4608c38852aba56611e8b" },
                { "an", "01fb4ffd2057b800bdc6d70db4a3a2c1315297702d69966056b465fb2977e9b174fbeb304a54ab42457b450fbf266cd2719f894332f9d5a566dd0238434d3d32" },
                { "ar", "4ce3bc546bd76aebb7cbffe8e175fcf7077a7a6300228df0ed0053b0f7d6b130605df35ecfcb630d849c9ebf6ec544b0597bcdff520afe2b1ce4061a6477c0d8" },
                { "ast", "592be207d2730ab582f281aad33039a63f49e29aadbbac038e50996a049e13156c70d9cceccc160a22ecbbfbf7338f1ca30e5da744929295d8045d0b74ce10fc" },
                { "az", "b4546852587d68cd7dc93c4b7f7b2284586a9b516f76df7c674086d3c4f5d4433ee4b979cc8e7b0ca280ab8a1fcca141629fb72862334da1bea159d32d73df68" },
                { "be", "0033c8c3eb4785de0a5d009739c01aee2d378624f67786125db0d8932509121c5cea0f1605b5ed638c68cab173412e9950f9b1edc06c66e51f6d7e646bf95d9a" },
                { "bg", "8eea393a56c1517ce81443f35c496c5c1c5a6dd5e23974da3dc4c3679372459f20568f68fc061843bf0c7d5373d475490ff95188ec7eac2fba657264820929b9" },
                { "bn", "7e6aeb2a06b95f0e4cd98774aa608f82c9b0ac7b072818967be8f907cf9f0035e4fb5c807b1f67582319de9c36adc4011d36401c7af29d15bb40608cfffc5456" },
                { "br", "d2534221bee2294dcac7a9f2ba88467b1ddee6adec9642f7555604136405912085368c0aecd9705c300f93ac0b956a4baffba3143a70e42c7a54b61ff5b40172" },
                { "bs", "35a97fef73c1a46654730fef8ee7b2748e9fbb121181040916c0704be387f18152fbf8e1cad25698b00d526801a531958778df606372b5c2e5d0f2d0b159b007" },
                { "ca", "88b347db63291e630d73d0ea692af6b16a2bc8a96fadf91c00e732bf91fbd480f1ff14b4ecff9c0142b57ee8cbbcf68fd5e7fb0693cdde7665cb68b8679aedff" },
                { "cak", "b6b0eb2763dfc053291379ec7f5b6c35643f1c249b030eee5f93b89bb34ce132e6ab96940919fc65f0351849c698994c29438e1333d96cc25ae575ae511bc379" },
                { "cs", "d892e5fa9041301e72607c16efb4e722e8f1623e182857ead842723f0e35b8605d0be2c9955f1248039fb14f3d76411afe41fe720f691fd0033af874221a7bf2" },
                { "cy", "3be5c70aa768c3d397c049744b91ebcf5a66d1828433bfe7b2d2651da6bc30a503ca35fb2bd81f864a91c1f055aefabf63a33e09951845780e825f5c847838ae" },
                { "da", "a0a28b26430d2d30fa585355c5c547353fe0c663ede244bb6dad32e1e9955e8f9c8806e502bcda74e6b3bf682ff7b18717257a6fbb5c3206e94a29a3a8cde914" },
                { "de", "0173873e4aadf9cd121f3976cad2c200be839e948d6f3f46dafa3ffa7487d91d3a2d0bc190e1ece12b29b0064ea4cb597139e8b5a8d48ee36f2d3cabb9f2e205" },
                { "dsb", "e3c8d0280297208ed4a3f5dde42926fbcad5421f7a152e49ad38856e81582540ff0af9292814e74e908871b58e598a60565530dc0996a2010d4f0e122777c746" },
                { "el", "78728759235b5109a10c77bf14d7e44cfe165a3707796f647f9384e897520b28617dd3864f8dfc817bbf5b271782c86e869a23e4aae2986b25d2dad1bc153792" },
                { "en-CA", "d05dbcb3733fbc83321ecac6ec33580547df1e0fb293cdaca84a63267b5eb776c43db81b70e5ff739f36383e53bf1ac3d048ab0e97e2135b064a7e734fb791b8" },
                { "en-GB", "5912d5fb48cba6769a9862a55f71f28ea7baecd2e8733bd21c2815a84dea73bbb936fdd0eea9ac4c16c4e51cf266dde24a9134a020f0af0a830b52fb25a80fe6" },
                { "en-US", "ad430b04de8b775cdf8a3069e24822447f30ed3a8021389b30cdbb951694927879b80cd5f69ef6bf11306d0be96479ebc9f3a70d3492e60dac7a40f08e3bd819" },
                { "eo", "67d96453060310edacedfad43fb68e4b07aa341937cae512a27b350f1b8093cc331d453e6b7a86386a95c7c2aef94325af206a9873ffe5fea832240a919d8859" },
                { "es-AR", "a2f5f0d3028b7e43c01c4d84f6500ddcb8f4dc61bb79000a33fd469dac494b79846c71948259773a1ebc39650474b2fcab82502c153e7fe4043a4d5929714776" },
                { "es-CL", "14ce3abafbaa65d1b513b4b44b1b3adeeb604f54c9f5fb4f233377fce71f1ad78d06410ad947234dbfb59712cd51c6d00521c7bbcedccee809ee15785ba6901a" },
                { "es-ES", "51ea6e7b3f4d626679284fd3dd6bf73cc09bfc87676713b173824e5e8e5fb273e7a549b1d52d8f1646df1fbb9bdc120f36946c4e6f3529cf43e4b8c8847f5abb" },
                { "es-MX", "219c01e70c619599ab6f0fdb41b19ae84aae43285ae46245937dc2fc3e547a746f9822ee7c65cb1f64b5905b8da65a9476b26bd708e8070eec86a9299538b2bb" },
                { "et", "be6d4ea487c650c2138b7d0649656ffee7b74ff5a13f7477b7676c57547b5ffe4469cce106815d99b7f017d850e4b08b3b94942b9a1728f85c9d64c850858b0b" },
                { "eu", "f6919d58d4cb590826263bcdb71e443391fcc38bf28892de3b21bdc8910ee960ee16ff283bfd798cac276b75802cc2a7dab250b313bcc8f2a7d6e17114d06b67" },
                { "fa", "0a80673449741dbb581050b6fb2f73e7393fba0c3e5d2507652e5c18a7098bada4ba740440790dd0e84911e07ef5f206e82594fe2c343ed446972cc117fe1e30" },
                { "ff", "7bcafd6216a9a77eacd4dfb343b86c220a6c16c637f63d8ce4e97f2bcfd6a2997c13eb8d6776753fd5fe11abf50a88a473d78af7b1d6b128aeaee3328dd0143d" },
                { "fi", "3fdc410b4138df02f703f0efeae52d35f752d1efe9cbbd5c6589162965d0c6dfedbdfaa249122eb119e0d8b7cf62fbc93df4cbf4d86604c2f719a7fd559d2298" },
                { "fr", "5b3dd7b3860a543c0281dc72f1f9ac675b09977163a89ff68752d497e253b0e771d86d1b20c50292cc8d1704b37a802fbefad8cefc9297328432baf78cbd4747" },
                { "fur", "56f0008bb1223318cc203f745e92ec51cd7792bc0e9c5a70e97b0bf23f6c8615cc84dff1c772fdc6c2033e97f2ab124012f3d57d92631c8b4f556583680ef995" },
                { "fy-NL", "0f01d5b4c677fc3cb6c37354a54ad19ba26fb0cd7dfdf65879c7358cbe6c09a8b9cd40b109a53f2d9b62b5ad1c479fbc6379530b768b7296251a6e0a739990a0" },
                { "ga-IE", "f85c272836e809b7555d622c2025bda25e9a92e197c6ac7452d49ebc1cc65ef9c4700bd629770696d5c43c03facb6eaa29f064bd88b6c6cf7e1377dbd8510c29" },
                { "gd", "b162242d285983352cd94dbe494b17e289cffdb639e69ebd80a08e287b029ba30a0d4a1a2317f2c7d859678a6d416fda68fa99abc90a741643de4467068ab45e" },
                { "gl", "c64d824c8d0edc7eadc11b7fa524661924a298c219e215fb6140566af12d1ae86bc6f5a7b9bdf21338257bb7ca897ac3d172f8e2018767b1a579feb8868cd1f3" },
                { "gn", "e524275b15224c3079da9318967af720abafe3a99bc8ea427af47c4afc93f61dc67242491122d9a5853840599477df4e5ff3881f1e353c6cb6ca88f4be10eede" },
                { "gu-IN", "1992b4d81797d164ab846a024fe6f01a4d4c3fa96cf2381bc805c9525b6ba90973f5b77384143584f5adb9c0f05e098593be2c0e3a7a0cb9f14185173c886158" },
                { "he", "cecfc311c543f8b47190c4f07b53223a9aa70ee9d341af602e593999ad1e6094464e1143abb9d9b9acf1c9bcecff197178bdd72d3d89db1b9941cf89a21c0e1f" },
                { "hi-IN", "55a55062f81a83f33c630c9b362e8bfad9901b68b0d611ce74694c3bd1a42153182eeda8f57f9e1906f7223e3aec1b34658d17dfc305b09dd66fa74b48ee8227" },
                { "hr", "a5862fb572c92904fcdf91aff02414605802a024c7e1e0cd31662cc113bcad42fab972e9d0baed7f27760497d28fe57b7c9a1a0ebd97b2945264f129c313778f" },
                { "hsb", "ff461a91fd2b01fc339c2205ec517dfecb8e5d6e18ec27403d802e190298f43ef5796807340a65aaead765ef9b6d23b2be0129a142cce24000c5fef96ff8c5d8" },
                { "hu", "3efd271f12232eca5c3c91430d488ae70ee380fc8ef7a9089b202a7546ac819a3183ff7fe1c1523fb51b9a9985df2357f3ebc0bb01714712988dd154a2b9dc49" },
                { "hy-AM", "2b7248111dd6f37a0d42de6177de1a447f11375ec3d014ffdbb441684fd43101a3af8a39e63d3549cfc413e5d7f38720c62d9bf4a7156db9b67d4b2c612c6b93" },
                { "ia", "28e3c23d4e0784051bd1b4e8a1e13703461a947d43371561fddccb2b27b702b1137887a0fd20a75319564e57dcbd65801f4cf8cd58d40681c0cff4fd38dfb72c" },
                { "id", "cf5159e8f9db6d6164e351cde59c0953558c86ff5c5d6c8fa2155ba53919a92c4b42428e5039ea467230cd56e55b83cfa747863b58c21994be157587edebd3b3" },
                { "is", "5350346b070ebdfecdc43460b82901c3eb25f79c1dc65a67728f12660d397b0b02af81a2118617700c2b87e9f6f063a1d14ec82804cfd2715c90e955af60c944" },
                { "it", "0caf0f3224a7f30ed7f366b392429ecf75574acf96456c99369cce3952aac65b6551a82512b26b761140ec8dd856aeffb13cc1c1c9c52cf0beb0d90613b5d386" },
                { "ja", "719fc14c65533910cfeee5a30c64efa1a9be829369a0371240666c2eb97779b9db47878fbc1b127f2888d245ae30796cd55fcc02164f9e5771114ec60cb3b1f3" },
                { "ka", "0b456998354d7bead5c3fa9b540f7bc4cd90b4deeca29235b61f50348d29b0c0cca0a8c8d2c0768a7b4cdaaef74f7ec2f7c2e35f85562cc037ab107830d7ba8b" },
                { "kab", "5afe535c90f34adae683122d6891d3217bb038896d07154aef193ccfa40cce030910098aee0ba40099edc1160f82d797c2a21f4f7bf5de60bf8646d0c7f4ecab" },
                { "kk", "ef3f868e9dd915d791904b6824b41689c6614068e3e953d77c3cc515616bff7cb152ce15ec1b7970a2e9e5a990503d4cca133bcb0d4b6717bab40061989260ed" },
                { "km", "0715116eb725d8de36befb50de877420b0094f835abca71cec883e1d1debd0b55d91c3556bb2ffbb71f77326fd5c5ede7791f5cb603b8180d2ea9a11ec97ab4a" },
                { "kn", "2a8efe68eeb497a5d0ecf6a1e4ec4ce948185356321dc582901b2a1e3f0bd711b9730228215e04e40d0eaf271db40963d8d2a16ada799403a69747348028efb9" },
                { "ko", "2e14a869522451ede4a8d8070e50a976d923ed969a5bb6fc1b112ff3c91f40d1ef23b9de389c9f50fdb33a5c12d0095c6744977cff82105851486671a03b80f8" },
                { "lij", "ddfa724505b9a94e72ef9dc0edc90efaffbd8cf59fa6cd81163ed7bb8fe55ca5062a00f7f11a1be414b808dfeb3f19a5b13787ef02947749d34f86f2633506d3" },
                { "lt", "b645580854f56eae96c1270ab48cd11458b1448e33aa5c6ac32651387c579a6f62779caa1c6e96ff86ac8de364ee5177fad58f5b2464190ca416c05634549fe0" },
                { "lv", "fb20a12998e230d5300f08435f128c650d423a4b5983bf5ffed366b59800766fdba83e160d393462beec3e4452a498148a40e9b096bb8b66fd2e0e421281e7c1" },
                { "mk", "4972f3715d8f26a3dfa281afc3b05558df70531658d694b89aadea2a9dc0309c13a81389e947b6f58455af6bdabe8cc9de08f5dc569829087a8ddf6f2d99bf42" },
                { "mr", "4efa38a613fae77b876115170a5cd89456619e74e0eb9fd68b37907a36a2a950dbe02b5505b149b3a3a8680f474444468faa3b2a6df708d25085dde44a1fc8c3" },
                { "ms", "dc6100e87c129efcab798b8abf13e03ba68ec189272639afc13fda5ad1fe3925cb274e2bb7523a83f82f3a44b299106ffe2512f9c6e75a86b1e586bcd0306451" },
                { "my", "7acdde458f2b99215f19826fe61b6d391aa32e53eabff0524f62fa8c02eb0cde012327dfc8b67d715a265c669957b13c9b6bd0aea39febffcb63c26db3a15e82" },
                { "nb-NO", "957f0049d03b1707676ae78a0a7e38b0db7afda70ea88361e94c69c74bea9da6cfb61a873a9d1c051e1ae95e4e66c5e946b60f855fd38f374c2e01a4168e86f5" },
                { "ne-NP", "87c54511cecc7e2605e2c059db885549de408dc20ce654bca57b18d3678113ea913723db2b6fb18dc0dd752a1e7e65874bc7b8985078dfaa69ed9e84716b5f12" },
                { "nl", "e43de2c6df9fb7547e1ffabf2522a0023d3c0e20283879c881ea78b846403cc5849f77dcd8aa96d0b0fa247712556c5d8a941cbc1d610032a5350ae3b9cc1849" },
                { "nn-NO", "738c1bb85ec6bdea4a2387d02bed3712620a5f9e0b318d1dbe365f8685bd4817798ec7fe51dace150cf496d9779cea26a7c0cdae81a6aab97d13727888f040e0" },
                { "oc", "83a13fa6d6f097e1ffdab23bf37632d900ad30e9ad3ab790500f0bbee982609eb7913c9a8f9ec649313420fbc9c52698f03b3108b98a98b2bd6e1633cf227afe" },
                { "pa-IN", "862677220302a4aa4c9801bcdfad07b44f88492bdd35eef8e4499cfdbc455eaba9c5b87f0257c6238b4f576a97f0a9a1161f67099f9eb627ebe5e5d0bc7c4db0" },
                { "pl", "ee09e013f9d25bc957e0d25fad46164e7402747295dd0fada9b1845a0277a191cfb799c50c3ab7df3808e47d48c1901970405b37e23c2d09e51f272d0ecf18b2" },
                { "pt-BR", "3d843ef8f28eebc5bd9d3d01d5352654d4cd3ea54fa295fe3cfd374d11ec5e1321658b358d3051079433096fd1ab1b102ea1264902eb2b89c08fb62ecbccbf83" },
                { "pt-PT", "b8fb31ac5a9f6deddec8e12adfe0eff0611dca0776d7e2355a75aecc62226e9410bef6bc54f25ccd4de01ed3efcd8b7144f5b8d9da7f7c8ac1555750a970c988" },
                { "rm", "f7d01648a1e67da407b4a56aa1cc758929de55e176f5ce568573243ae38a47e5b288f6d923b5a7ec515a1b59f7e4b8ef47079c0c96441b2eb7e8bf7cf03c058c" },
                { "ro", "1892959faf0a31491e73abd441e2f569e42b9851e67fe4c65646329eae17336397fdbe4fa68f580ede6db5256327e152e52dea0cd55bebba07e9d9cf686fa0a4" },
                { "ru", "8a09be370680c7298d4d6fd5cdc2cd2c153801f831e685ce27ee6623ccc61372ff74977dfe75e101a591800e45b6a17a9084c208db734a79840d1c4729e70484" },
                { "sc", "3ebc0480245e9a20487fcdb90646194d15f00b8b26128e624cfb7914d8b1ca8d30113627b930da43a811373977dc60194df1c23e9f50ebf8ff223cfa58b32e70" },
                { "sco", "4a4c9206bde27a214fa4f639c52872ba20b43ed578aa07c59080651ebb7f1cfaeffe1d45e3313bfbc0c172f9e19815f337783a2d0ddcd96847b758b5064f08c8" },
                { "si", "c99765ab2d83863da02c8b3e3858bf5cad95b2e8b9dc55457c9afce6c8d88977b6a9a12bd75d9fe29f2a3c3d7d97e109d7070d39f47e405a0ae42ebb3d49f35f" },
                { "sk", "1e145fb1913324409b085cc23ce95acf1a5a0c254ebe523cf90468d45e4f39b65bef47642facaf264ab55dcd3dce4a3412264aed8e6b96bd6f0d445ef70cf2a9" },
                { "sl", "3125eadb1852e2d8f10c31e250ac0484718eac83e3b7db1cf3426ef5277df2b8b9b4e895b8fb8c29d7c996bde6105ef214c53c0f27940f56d8a0ab334aad87ef" },
                { "son", "f5cf6cc3928880fbed044201fed5649efd31fb73a47b1d9b06f736486a06dd45be33c9c9240814cdbab2780f71a443ee1ee41ccf946aef5f767ae43b0aab499c" },
                { "sq", "413f2fcd4768d17e2810f99d37caa2fcf85d9fc6d2720302d9454112945b721b1781f0ab8918de8fd1ffd4dfad8b9807f2d95cba4f57c3e305064773314550ea" },
                { "sr", "2f24fb28f956a6eebab8bb8f4cdd263f9c0b47f8d21508a828a2a2e6cc806214a2a828e8ae02b3ebc5853094df213f9e31e0386edd587be469049481a9735ebe" },
                { "sv-SE", "7f32980420ae8d292dfa0bbf0019596e56a1ea09db88e1d5d1ac406c9bae8d51f9b2094ae2801d63de24c618712ebaa23c7c902a84e6c48b99b9ad3373ad5e26" },
                { "szl", "88d7ba9000a6ca2f19dd2d653f8befe3bd9d10cf988e7d719554ec599de5620286cd9e668f40b43a2ec012d9e416499444c7a735889c5cce37f4c454b767e274" },
                { "ta", "5b95c1f1ba152414efe5cd9cfc2e261c1e4252b779f170687127af437cac65c8dfaa9453703ca51e73aeaea50e1fb182d0834d44b7003fd2be48569bb507b8ea" },
                { "te", "c81eb7519852d1f5d95f79329e11d0b6b9caea8bf58fd053bfe17a158869d7ec4b64e979a8e96f754d937083eaabd30d84244d3c7f7c627530be62c0ce38681d" },
                { "tg", "b8578d6837db2c18e7e6aa5485f3f2489d570e0a5f00f9f0fc6758d015fab01548a8476ab19643c8b73a1993003ab666ca1d8413bbfa7df455ecfa80356e5367" },
                { "th", "f410d93655aa735ee1e017edd8187cec62453d1b84b0ff051234de5433498e3fc29aa536d0410b5dd58a7f5cf0d0f81f7a862e7513f6d434108738e62763a734" },
                { "tl", "08660a2b6afe53d3cdb072f3df602a06e97c2e116a61b6962870f0f514154a22026235bd3b653d684387635f8524c4db315c1ed7f82b9f4933203907dd532a67" },
                { "tr", "c3777a3392457197fc897c78b93c5c4cd8432de5f119e5da4bd02316b89b9d9b6b9d4b815d63fd2e8fc4455692f1b8cb12fb2d8088d91bffbe4215efcdc83560" },
                { "trs", "2449afcd9b0937806e36ebad25320c4c669bb7962d466e4bea0b40a22b376aa9ece5be32c8203959ed7dcd0d39bd789b48f0dc2623f97aa61e09b2e3ac24aafe" },
                { "uk", "1ce6b52f138112a9dcef0c7df3ffd7fb314eaa0231909e56205513c74a7083937e2b064a67b4f8308b2c44e69a01666902ef8ca8897060eefb98c2063a68f239" },
                { "ur", "0cdb551253c1142549123a033e7c00b1354fc5fa6abd12b4f6422f224a2029b2dbc8275bcf15eb3783240d84caef94d143e2f68a1d2b50c608ef49f94737dfc9" },
                { "uz", "dbe7626b4588122bfe6a5cf1051efd1896011b90eae16dc68590ee6ce14310a07f2f82d8b5b4ad24204eb2d601066e91c045796f64946e13bfb3e863fe7fdb5e" },
                { "vi", "db44855681f0bf34a70924330a53b75a758e669696cc1f41c54e1cdbe8feb9ec27db81d3237007d67aae4154322dff4921522ae9d8f022d5b9f7de7f36a88301" },
                { "xh", "e85fab7a8d2e7763de76f9018b0d5c7cd58406f031e13148d933853c857c0845f1722ef90577d7fa485a8833ae1a859d5dedcf5afe31111e130fe331ffe5bf11" },
                { "zh-CN", "a70c359735cbbfb44f5bd2a6f4949ea3f122d8847baff6f41de0a5fa853bbc75d09318515be5d4b0f4d477cde62fe45a6fb3edfe8c005267d007683bb785994f" },
                { "zh-TW", "5dd0f2580901bc39c70c95cc83c5d46ac52a9c848e65100d85c5f1401610beb168a93b0c88775ba50cca947b142784992fb6e4d3d8627ce15fccca019ca597b5" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/113.0b5/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "75eee259dbba17509422869b37c9bf912ae7bb238ed542d3b5cd3c0ff130acb4c915658fde9201fd3c4d0b7702ec6d792a5b2aeab0163d5f6c2c93fb30b29cad" },
                { "af", "bba7cfb75eecac99fbb26919665fc6dbcef04ebe108327f55fc181d0ef8cdaf0c412c9e8136973cb4e1aaa5591305ba41d607d9db5f2dd0c25b77e8f5f19d962" },
                { "an", "6538e0480cdcdd5c50718ea38f66569a37e35ee941a9403f45adbe8c84dd194f8c336f6f123e33048f90febac59d4e22dc940d09777787b091ae30abf8716739" },
                { "ar", "5e37bc87b601aafd38d1238f43b24e32a63a3450d1841baf72598795d598edd836406e7f52f64d818d8641a9e95f47dd967c46f75873ead3f951a4bf6b18c702" },
                { "ast", "e4f3b10f1aaf7a5939cb8a285995dd3b2a3a7e24c6d687d2a5eecf0aecced57ca33f6eb419dab5ed479d02d0fef338fcd7c3a7d699cb8908fa5ce7ec460b087e" },
                { "az", "f3316d11dc88a0a71cbd3be5e5be6bfe94421afe59e649975a3daa39e72551289bacd91c0640bf7a2ba2e0cd80846325dd76fc3565fb56852dfa8e4289b79a96" },
                { "be", "050f92f1dca0aba853a8d467d96e75ce0d360b3c442490ba489bbc0b6e3464740971777086c038ac56141a8c85c7ecbd4d96bfb82d684568b9d6394451893c14" },
                { "bg", "01c06b637d4b217ba65d1aab249f31e1e80233f3e3da2aa9ff6cd57e986bb6ca55552f69316df9898fee8d67e0e32d6bcb5fb401135f588a98e05fa1afe5f544" },
                { "bn", "75d2123851a4bcdc84dc529bdf04cd3af867642a04ffe808c3c514ddcd7551f866668e17d6ce7fb91f72acd197e4072260cba8ecff6bb2610a942f95b3d4fd28" },
                { "br", "04ba7b0e9f6706354e00c006d5567871fa26b6e26d9b9299f1ce36ed02e3edd8b022a9aec63142e8a84c5c7bf94c28ce79fd37d6d9ed99e59edde1046f0bca0a" },
                { "bs", "e1a9e497cbe11da65e130dcff2d26467c9a6f02a3cdc16153b28c82687e7f3e5396fa008b1fc365a62af42891977792d642cba715042703ff448c5033feeb064" },
                { "ca", "201125a1e3b95bdd5a799e867a0befc5a33f214db89b9a248aa8a7c4d775b0a5c79cd1a0c5998ceb2fd9e599b3ea49006f3cfc407afe94c48d2bf9f3e9275cd7" },
                { "cak", "840383cde27cac3c47bf9152ac6841766f7c23dabea2931b30ea7a15d815446f52f16cba1a6fe411333e5a22a8ff128d9459b27ab073b218e643687ebe7572af" },
                { "cs", "9eaf4e6ba82b71e88fbb8d185a37ac79870e630ceecc8ba01c21bb456c899e3b48e01489dbbaa8926416593d6b89db423c6a84e83366e1362f94e27751c2bac7" },
                { "cy", "1924f6e3bd04d4b85b20a5eea81c5997f74406e70f1a72760971f15e76d02f0737ead09dc31e877cb811a9a876281ee32f80f9a3fa50c651b3f70cc91bcfb9ce" },
                { "da", "f8275dc1c7ea31671371919dc50008961e6db86c518206810a589f1aef212711550530c35a97cefee0048df4cb0534b8d7df9a8f5bf4cc854931b229541dbfd3" },
                { "de", "2cdfc9d64d60e4208ab228701e5ad0e457d5a590aca2e1b7d778f8c4cd50e362aa6eb395eab1410e40b5ca6a49e34e83af514b2c001a49c27ef386fc4f91cc12" },
                { "dsb", "c353dd34edb98af9bc39dd08a7034adaffe4d5f7677657f8e6c1f6c9895d505e3920a16466941ec7202eac2249cdb819e6dcea651cbf9ba8515778924d426556" },
                { "el", "4ecccf5ca26bd9edea1bdfd7509e1910315d986ad0ff9bd685d8a91759d0eb88d3188d526b600e1f31ff4a149c8994f09a95e85e6692ff1b423ac4cb3d366dd3" },
                { "en-CA", "6a54eb5eef4cdb136967d7ae6c8354dc3f7c59c6287313c8d4e288981215dc97f31afdd605d9f389848dfa1783abf2913a560462a9d0e2a79e46b0f1611dd570" },
                { "en-GB", "3ca2d5165b0e8a91025e7db5b4c8c91e9a3cdca02eca28f7a2ceefbdd995836cca88bafea43702c349c5e359896e95970121d5ed6fa5a9db776a90f86830ccb4" },
                { "en-US", "c97a89195dd5d12e518b7f92334db026f663d29d8ce6a8be9f31e2a50a9f13e2b5917230a6fecda5e6ab5c0fb724a0153377808509e0a02e950928606469628e" },
                { "eo", "962fd90c1ae3243b433b02f614b9e5f5196b2cfd5f22aca75f2603e31871ecc1f813095a44c386967e162838b69ea77bc10f86ffdc05bb8fb7ca2a42ff542d4e" },
                { "es-AR", "da3194ed23d4b0218ee5217d5481a6bc60ca319477ab08508120ec254b29149ccab63a43f8315764c98206fb3fbf5dfc624fae94d3e7d7eebd72b79ffb8fe23d" },
                { "es-CL", "0aa096e8814ac758881e1d8cb1e626df5c4e94cdb653f8a87af626573f016ec9c94b3d0fad5e347c28758e97d2c802efbdaf5026de3107f4d1c0d36b859632c4" },
                { "es-ES", "3b22438f9e2074fbc38b79f0b84afe0094fe57255fd47f60b336b46bf1829cedd50fb7c42a76c63415ac4d298694a9de75e9d68db89e869b209e0212eacdc27f" },
                { "es-MX", "cc5e48aa9aaf04f95f0f251886fe254167342444843e6a4ecb50aa58b21b49e48dc38023f2f70c40443655a3d00064f9e862ead61b596e5a3ec525551b5846d6" },
                { "et", "1c2fd95ddf76e87f949eff2432a023fdbd3d8c54346fb77db70842b1ae936c93391106c864b585b287b053eadd85f21be9a56705d698e5782e7772e5b417b962" },
                { "eu", "b01e0a0694572d737ed202878c41ee0bc89c3e07f0c4a4e810eecdcd4bc5ad42e727d0ae4a3da0a1c9c43fb806e8fde95329fceb7dc957ec0bc549937c0c8b4a" },
                { "fa", "7770f4a1eb843f17c70eeccaaebab991916a81fafe766658573b60d2afec7309948dc27168713526bae9e15f0c446d961f056da2fa0fc505995003d0af6ce39e" },
                { "ff", "a345db30c634dbe24fbbad3ce504cc23ed657908864132fb1d9bb073a3d6055f7a856478bb34e49c077e07665c50b5c413e93f6529782d376740c5365e1c6f5d" },
                { "fi", "ffc04ae95fcffc3bef7e0a187b85721d0c50c1d1973f4a3c30376200cf9b2560811048b59d1c629d397c83f7e5bdb4dd859e97cddc18cdb4df88c0c063cc3d3a" },
                { "fr", "915c7ebe40a1406a2f8944655852973c3382c01b911fce18260550363ec6cb765ae9898a007415fcb90c981e99b4bb0a5bc326d3c94e8ec86bb9cd2aa3c522c7" },
                { "fur", "0882c6216a33ae7f6cc27a45f72a0a51f546918cf4076afa2f2b14de03bee207e8e8e43356abfe4b305fe1c4c2b6e48a174b89e2fb33fd389aa26fc0008599bd" },
                { "fy-NL", "4fc4b272dfa44ce430ba27b2a5f1c8d287be9321a59665e94b093cdfe48574aee1adc7cbbd248858bab9745b8acebe7339ee4c2c3a8298336c3cb7143a06b985" },
                { "ga-IE", "9ccb9e3e323d70fa4157beee7da47df7336f6f5ad70986bda2e80782b44367e8f571819de04f46b742caca43f72506762e482209cfa9f7c3b999a6b48460cdda" },
                { "gd", "29c5d95bebffbe91a3855d1509e744eebd9ff46021a9ad3316388b88c935fbe1846a5bf505af4b8c1e03d62e407310db1db7da166be4ec773640cd63d0288817" },
                { "gl", "f12e77e0e5647a8dc7e7d34110564fe2e9457daae310e6176196a389bc5f7f03b6ecf91e243252c074a71b7b897cccba2576c18340c2a74a37290ef1cfb85d01" },
                { "gn", "edc0942d332a4dd0c0f5570e4549395139cfe3850dbab28d77d54bf911404e6c51e0663d670cd73edacedfa37e937ece9a967553ad0f9ea63f26e11d9785d995" },
                { "gu-IN", "98a4ce15d12da0ee924c8acb8a8007cb8bce21a8596403e63ee87c16bfb1dae6cb715226fb4f803da6cb55b1d0c3efa85d9a210a1289f54dc84fe7d46e02ccc2" },
                { "he", "1083c571182fc2663b766e58fceed793ff57e1f3be26a8854182826233e9516bd66c14ca313a6de6ebc5dcdddc7dcba48e28f36f866a0ee096490a4b0113b0c6" },
                { "hi-IN", "560d46b797c098393d549cb8e3ac2d9f687682f5932441616f9bf8f31f9d896c9cc81b85bf33ca6b15c7e7a5d3c6ad35956392c79255fd383813ae2ddb0ec230" },
                { "hr", "df3c19d236d85b397cd1e93fb20abae1ec1bdcdaadb0df496fe24eed742037287b41f88f6cbe7db03ba13e8f125fbc843f70c04165c9ff9b63f1b46d333c1af7" },
                { "hsb", "595893a20deb8396dc716b6889299ce513cfe557407c08937df56745d2a4faba61a92a972b339871730046c549f90a6b9272db13f36efb5cbd4b5f9ef0fc437e" },
                { "hu", "42509a4a8456027f713c314df5111afd59e1f9bddb72afa1618adfe07717b67c026211400c0b1dff13c7a6d459bedbe326ede78be24d3c48dea4d32c7e9c9e54" },
                { "hy-AM", "0f6bda296f3d7b5da5dc8bc1a53307d5c671d934105627c2d532b111a2af0a76a61a79332a2b84a05a8df71ed184c1078a6910ba5dff1c512b0758905d635293" },
                { "ia", "af156f2411014e21fc44672ed76150e8d59e40b5d75f5adf88c63c547b69afa7c6b8422737fabf3da034252dea568b1377be370dab3da43ccaa7a57ec520bf67" },
                { "id", "8b550605784c75ce8b239b270f008a137b75c2a740391ba511aed9cddbf622880656b54996a29d583707d1072f729e107360c9f9409efd93b2dc1106ab5b93be" },
                { "is", "1ec5ebe465107173590fa71d8a15398bd5f7690486acbb9aee012e70afe3701d50e3bad73bb576905a33c7b90f610db5a71ec5307b486ba14e12e384d1f383e7" },
                { "it", "66926a01991e0818ff45036dcd3f4111aa385b9f67e2cdf7c9c66ef0208bfbee3d9cd89fdd71df2dffb0f94981f9ef7095f925b631ffa56e097352f4f89824ab" },
                { "ja", "3b744bd81723a7a3e1858e31073e051b630a74b63ae861e48901c49ffa5634f99e5294a8891e82c7d95f559116de0792714e22dcd7e328288980ca9b4a20b9fe" },
                { "ka", "a8973944e0d5b542a4177fa93b6e99d624c99c53537c767ab324d43ab15f229f209431b3e95aeb05dcc7425fe5f27c4bb5aa1d5641339bc19c9fc7a97f4b0747" },
                { "kab", "5aca5ba860e5c3252e192f9025deaa0ac96eda1b82a5ed98da63bea985bdb6285930dacfb88df225b8ed86efe9f817c0e424f4c3e28817ac8ad97b1ed1d77ea6" },
                { "kk", "5a60d53e90124d675d11c7ff01fef3eec028e3e51a29a2b57821449f9fed0db7002d150064d1f1f95d89be64fc4aff1f5a7cdd330dbbde510ec56eb694869a3b" },
                { "km", "f7f27d1ee8ebeb30edcdc83829952c37d8b50a48ec3477847933d98aa57aac93fad781af0cba2ec581d297b1a5e19de93f1017323b17c7478488524e060f62a4" },
                { "kn", "6aa4798db5b53874fd00c5b8d926991dbb0883bbbb83c17e6393c669a1fe92a6aef2bfd3b4bdc71563cb2d5c708f32a34b78d55455b3051e30cec2a7febd063d" },
                { "ko", "b8e8867f273de8377271491fbed7acad59babf7e75922d02d1f67a4b5d53544a7366713b950f93d3f154b2c3e6cc4e7cea730bf60a87e50a2e4b00d5ce7fde06" },
                { "lij", "7118325ca3be0d9e8041e2408073489eb3c37d1826975d665158e247408b8cfa1b6d3e84da4fb22eac81ba90b19e2aac2c63edf6a0a1b5645d4f814429a7d7c5" },
                { "lt", "31ff9998b13377c41f9d62e22d5c3b71c034db00a70628a88a6b86a107594a7f4cfd7b61b7ff939af3703a3f4c134bd421a5e8b0d3e6a44feacd07e286d1105b" },
                { "lv", "a235ef29eca3f7edf3a82b66b917d179059c6b3f7839748a428da6d68a0cd19915d699f3d1803f4b8f185f38c925b7c8e63880e3e2023d2f42e9a0b99e9153ac" },
                { "mk", "f1d3850259329d88534a513c1d787b82071c83bb603449e42d8cd3c354f31462be8d25471c38e688e1df391fb7af339b5447fea1d4e46bdac03317495245b8cb" },
                { "mr", "599949b50a8bd4e671dadd35e15908c0854652122a3f0a27119894e9238dba58bed7ff4bf7fbb14b6a5ba9781aef35fb26a6b0f410100e261ae6d24422ca56fd" },
                { "ms", "c2667a92dd57ab15e85ab623462568c22e7b2cf72db496b68a99c3213140dabc1394335c8803b28fda70a8e5f2d300d43ef6a64ac69afd255cc43704bb72b3ad" },
                { "my", "a61146f6aef16e9aefdaea4ee68fbcb12eebd65ad7c06dde77d05b9b4a6973fc46fe20c389b7f594980464c17bf2ce07983faec63130946c9b15232197848d32" },
                { "nb-NO", "86244888036c3bad55b1d6ca8ce33b738c7d49888a4635985d1bf12ede2e2fa5e4a6aebabf26804d29bef15c187b10f5e4ae674bd5c62b3322ac8643d9b363a1" },
                { "ne-NP", "9903ed644223f6064b3389abdf35adcc4ffa7cf2af675c95eb66929e3aaee053df7a44d6946359c905bf3280e689550f7f2b814dec8a35f9d93d7fe076cc2e3b" },
                { "nl", "5fb84bbd11372f7c433021b9050dd8244480753727a2348ddc2dc8954b0815366b80f4ab531aa3adde4ce1cc409ab5542206261ed44e96d87054b9fdd0c388eb" },
                { "nn-NO", "c00aac02663c49544718745d3c56f40ca96322252db95dcb78288f1350c719c0232eb38574594fb1525d15cfd3494efaba78942ff310b2bae1948246018efb5e" },
                { "oc", "241728f41b1c023009504dab670c2e005b029ffe49f7af19c6bb43dd4801df21f69a6752e555ad30c94412d22ecc96327b6376cdb92a13f0f214d8105304ecd8" },
                { "pa-IN", "80b2363b5632239e0e45076de8afec07f6fdec065bc869e91afc6413c5b2ba2e93845bdfddf7949de490efcf5e8b2f6f8a313a2b870d10bd3a198222a7d45f96" },
                { "pl", "65ca4692af06f37d5f1eadc8ca2f5193f1386e4ff9bfd55d17e03f8cd838224cacc908fd76b3207393f0a7a9e60591702c882d6cde39d3e56f01bdb321c91656" },
                { "pt-BR", "80a7970f1c51c0bbd51eb2495feb0ca84e6513017e999b30b4c7f6034afdde85d6f2127cf5d8c9b2b0c9ebd02c7f1da22d5070d7364ae4979b63fbf8dd8ae750" },
                { "pt-PT", "ebbb61efac423c158aae7b154838ddd5f696806f5c322dc5d87c5a207361df6f852a4ac7d54228d3f2971b6c1c6cae1aea7255c88d656c490e498418778b8788" },
                { "rm", "8d5da964e2453be2ed216bf5ce6679a9744b6d14aa3e357e9a775cf587dc8c213d7a08a3c93010809550da0039f93ea1f86a50e594a40aef58c1302d33f53d76" },
                { "ro", "3509ea3fad70076456746205da7907181d4bc5caf73e27f3a980d47f08613da6a1cfe3fcd4c33e75bd8e5ce25d8fa229d15b9e05001dd63ddee5fdf000042b35" },
                { "ru", "e8bce19ccc4b9b49f17b48f7602fc755c663e7b93b07bc433af39263d5928b66a453d3320231eb478161cbf9d333bf7123203d2a922afc878cd5029a89ec9418" },
                { "sc", "131992a2c20674bc70a3bb72738b4686c202b5fdfe147fa0d77cb8b0783ec551851367c7fefdc7c6c94fb868aa87cc93009f09262da72ce9dc4c7148d5b6df7d" },
                { "sco", "49a6650dbb013f9e54cc059b5ae7c85d57277f5184c14461d3077ba5d4a8b6b8d760f6348ef7ba5b7c30ddce5af9941a6b0c5113c8541e09ed6ef5c2ac3ac19d" },
                { "si", "486a243309e7f8bf7210aad49c40aea6b5b59035bebb1da6f868de673458c5f9167e720a9f475b03ffa76082c189b3ebae1e6901cb0b94b78a78eb28422afdbe" },
                { "sk", "7594ecbb79642eed8566cf4850416ce25b910d824c35dd772ad6bee05e1273a1b95919eb0556c7190c8a9d601693cb75b975dc75cc25c993cb5a0d5856155c3d" },
                { "sl", "82944f4f69e510ab1b6bb6812c02fc4a2e32f2dbdf1737cf48d78a1e8be1d0de811827107a871bbb96b21064438dc92fd623f473bb89ddb583fc435d5d59867e" },
                { "son", "1d49cecea78f9fa3c659c972b3d7e08c8f519edda9a7b4f711336ea9a002248ea93af5162bedf36c7cdc22ea0f3d615c9e05a1784d03d6baa1aaffdd06b02d5b" },
                { "sq", "3e7ad7df14903711d39d0d16c75966515c7ee4d65b2de77630e0bc870f3ad1bef070a0bdb2708abd615620324e6fc7ead08cac2779ad1c1000a319dc8168866e" },
                { "sr", "8556fea1cb2aba149d5413e9fb63389c161aa52384fd4057702c44f846e28cc58a6df1dbc3fd2307c4851c9170acc827cfde4448c8f5ab2671daa0816b056b37" },
                { "sv-SE", "0faf5cc4dfb83ac0300a614cea19ebedf4cae8c02a9b8977f09fe231c9568448e53d629af9a4da6c719bf40ed9fe212af637cec10e704c95168cf16fd032ad16" },
                { "szl", "68a60d1c59efc241c42dd75310dfc4dbd32b12d68c4689b41e9064d39109b78e2e23a2530d9704ac9d616584c487fad985668109069c2185ed749d475dc6f706" },
                { "ta", "0e69a18f050c25d311777d45e54fc41d8cff1c89cd83980053e2c46ee373e4b90d0fa7bc69a769288338d018a4350f3a9455c31805925a30522af5c440a9270d" },
                { "te", "704dc6fe58d0dae9d527687e58b181bedd5941634907688370762a76fac34a5caff0d37f5321784b612fbc06b01481ef2b107af4bae32fdff226bb73e311e74d" },
                { "tg", "910ad950d2bd4af249103e784b634b60b05c85842ae91d44ffca7f230fc4377618ad2b9ff885ef150129722d22ebfcba07e54591f4390bb053750256637f42ce" },
                { "th", "5cbd9699f44d268456adfb19065540f88b5ae08a04903bc1fbd47fb5bba83eb599647ae947be1915ef0276f4a2cba76e89e5f09c4d7c0542cf26c14698233c2e" },
                { "tl", "a30b453ad7974fa99f6456f5eae1f3439cdaf6cd92862dc50cf348f58aed9a84b4cd0309a219d5b64fd25966d14ee481e5c360442cb872fc15780362ce2b9e72" },
                { "tr", "586d395494a9c8b411d323f47cbf7fb6468d594fe9219efcc48e9220c502e80e1fffd7bfddf329d724a30050924dd91c3cf6d701f9590a0301c073df9aa6d069" },
                { "trs", "52ef8d39c459b1dc4f3bd541eaadf3ae14de01b05908347310b5836bd7cd0c96e33a75c797770007e75a0edeed364f78e46a7a0a1766d86eb78897dfaf2752e5" },
                { "uk", "f28f3767631c84331fb42bb152bd34370bc28fe29f87940a75982c89e44530f414d61f7c47101b4ccad6afc0b5fcaa1eeadcae216b152df580c30347b24e3b22" },
                { "ur", "2ac6928a660741f74918e06d728b1faff8f165145a7c45180820ff7cc1eda175a6582a5787a4be956da8895c1690392e94de2cc8db74deccd939a746bcdbbea3" },
                { "uz", "873e001137d35642c10dfd3da0fa1246c6e9114a0e3b9f9b538b62d8ca779bb5773d18a97ce8bd5d547b3f33bc3b82099714a1591e95e7c350937c8b9d5f0da9" },
                { "vi", "15e16703598e416b491037e8e8664c0231d35dd081360da24e0db388fe2310b76c56354943766d234840058223585e4542f827739b0665caba3ae070195010cb" },
                { "xh", "1078a0b732b81bf368aebd648a952c2a3540214d3f3a6e475a058222ac3a5820f55ab18f9db36a499ffff2f5857281c869e36579b9eb0b0db34cadf4feb91de0" },
                { "zh-CN", "e497ebc2db1a3741872bce57042b29ebb275cf5f63ce182da4719e488366d975e8772707f5b220738a9ac07a3fe36a35a64810104d489b4319cbcdac11c5f820" },
                { "zh-TW", "514359af868a6a13fe8f108bcf41a043d354c9d0657a0589aa25262f9f7984ec061e71f73b0515b76f94ccb324e3ce888f8bc493597f321c286e652d15bb36c4" }
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
