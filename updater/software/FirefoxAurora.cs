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
        private const string currentVersion = "112.0b7";

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
            // https://ftp.mozilla.org/pub/devedition/releases/112.0b7/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "d6fff4be9d55228370407509582618db9edca9369b66d2bd59c84448728d1d6b87a919961cd58f97ea58002d0b7fa7f9d9ff287331e39ab0695c8547f1cc6e7e" },
                { "af", "12d5265f175ab552041f7942c2500f0ecfe66524d3449a60522c45cf07c2ebe5f96d5b2355b7a9ff0adb7bfe0e82d537abdf0e506769ec8a4ae1d46a8b880b7c" },
                { "an", "a41419dbce3b8643e31144ed3644fdbf090fe697bdfeb185010f019a904f1afaf1ffc78922beb08727ed2b6db212d72a24d7f2c85befd55062b1153205a62544" },
                { "ar", "c1f86c470beb930153784fd5c5e28ee0614a55b7cb077a122d0b9e67fcef34f154104f9fc1b2f92fb723c39091700af751026ee0775a22f55a127e1a1926ed25" },
                { "ast", "c279c917a63ab67e58e85c239ae72a94861b787d86beafa191b668fd76483e85ad7bd09affa32067dc3e41ecdd3832dd0de3d2cdabe79fa213ebe022725eb1c1" },
                { "az", "168d956db270dea276635a9c81b04f0b7f6844d9955719b07ca453e8385dd3fc973c3a41f88cf29049dd97ab895b519fe87bf6a6b9fced1ea33a98a67929e68e" },
                { "be", "81ee32427a2f406f09e0d827e0a57fd5e237795b3426afd0d8ed397e5a829eda79ca73e28cb1a3642e39f806a1b9d2104e329405d61312bc8f4a4aba25e3b4aa" },
                { "bg", "89859bbf63d98899d549e33133f2bfaf4fef298b0c76096f39f6cf4c06fef20fca86628664a1833b2585fcae318a8f1e450169a1e6f93b7883c6d19c2c1404b9" },
                { "bn", "7c3dd3edbe83e5868bf4bea054a7182b958a3162a92e429d15fa7008970eef9a1af61d547a5263c1e11a326ac1b4c2c93b3f218269dbf78b64652c7cf6f33b5f" },
                { "br", "3e3e0cef24d4092912f8aa2766b0d3c4f99fb9f5856d6c0a054ee21b712b87e4bd2bea3070696f5af8c5d59543821e3acb5c991e9ee4512acde0d72e4cf08708" },
                { "bs", "be848689ac04a436772185945f3c84d98916a04bcc2b574d0833783966cb844b5a208f22f2c91e70dfd5aca2e3e66e2dac9d81a0f7a7ad42400a8c93c0756a60" },
                { "ca", "8c9bce09d97f1e2c5ab638d4174f023059021f65bd07c480682a8e6690d7f46c5dae316955b857fc4584a856b31869a0dcec0e5b8885552c5579ce07eb382049" },
                { "cak", "73b6791b7108ad09a4d26dc15f98e3bd45bbaff11843b3f6be0cee4403af23f6b2c76c19c238361a81eccf197bb95858e324921a568f4c3c20d8dfba2680ae57" },
                { "cs", "fe7513c883837973185b3c25691fa4bd1fff43d3e440e4ff38034340b86274fa66d08b0b446cae466466288054ac826e5ff6e5939217d72d03fe79a4dd4e9611" },
                { "cy", "2607db5afab3abf9532446811460e3bc7a3c880df9316668b431e7828c4780cb43fb7dab401f1223d9ffddf43e99c2f64cc280c06228a4a119026d0324538122" },
                { "da", "ce05c49627a2426504c971ae33267304e08ba48dfa6b5114e76e5ea031430eba3dc48b77c03b0ee1b24bf926505c77cd20741f7ad0dcf5f4d388bf3767e5622a" },
                { "de", "0973c08c7d74c9d2718dc149a18088c10e4034de84847ee7a1ff8fa803b91150c12c0965b4383f4b60af0366b02acd5909e0e8fa1762143a9d99e79c1aa8efbc" },
                { "dsb", "3a5ae49415d76f4149ebd84800fa02f4c8618cf68a29d7b8b46a92ce35c87facc17f54e577d056d332b07b2b76678e55529dd6581e48f86961d0d49e94281e2e" },
                { "el", "5a4e5bdc388c8d6e8e07fcf18a7417041fb17ea14f494fe91527d773aac1e30902bb7a4d4465ff3583d446130609a1ec3d27e6897e0c343b8d60f056ceab22eb" },
                { "en-CA", "47411cccc943efe28e300bdebaeb3ded83996adeed39fa6d7d0ff30f51e17eee1b9089fe67a31fe5a483cd121d7c050d0861bfe33fbfb713c408cde1592a2ad9" },
                { "en-GB", "05c4a5b59584c5b76dc78b1d8db635ed53b97223ffcf4417382ab9ad4748c913b3300ce588a266586a95812de3a75064a8a44bb03a1573f288f7c349a1919b18" },
                { "en-US", "f9798b88947f71f2c534f7efbedd1e96d969c99016a2a3bf98661af31317aeac825e4f1d25e4b9d61252bf7492ae531f6c41e24f2907e282881e78daf44eaccb" },
                { "eo", "27e920c7faf5145b25d95ff9293f00605c88bfe58fa4d13d49a37b6752e396c4a2ce32e3a60f2d0a0f4955e633e6b9a46745051fc95e33ed39860988f8dce8c1" },
                { "es-AR", "b9f3405e708008ac3a83413370b2a8b0292f5a358b49c163ff4d6f60a908dcac557a75389b6b497c0f2f3b2bd49343a64c39f761c19ff0cf2425d982abdc9391" },
                { "es-CL", "7699c55e480f538c2f32c5f5e4b30565735cee996582a2a7d2099c1878f5f0d84e911df87a5c08139de34ecbe2797887659505f5ca341510731723af623705ee" },
                { "es-ES", "cabc24815224a67d6c922e6c84133a65c86c6b574c40e088ceb064e5c727fb65f756e9ca777eff519e46d2796264b9a6511ee9ebb4834e4e4c931db79c9828da" },
                { "es-MX", "4c680dbce0f4c538d32b94e3a5ac3f16f6b23a03c6bc8a32e265491dd6e03c38161a9ae203d5996eef668229da1cbcfecd05fc54f5c04d769a0574588f447e40" },
                { "et", "c6c0d93278e0560f6b8b57f32efda82534ee17328962045d31ce7ceca35a3ba225c592f34130d68f01219c93727892ae304d78db979a5e2a2847faa01af68069" },
                { "eu", "4f7ce21e5c92f6dab739fd88c50b7a63a0cac7f29b2658d5b6ae69f1e6843e064ffaa64d295e5191f88cd54f142710dbd5490bddb7f45e83128678a5396e4f1e" },
                { "fa", "e97790164ca373900f1a10e4b25a23f8d001b106b22af6225afd4585b78cbd276a2cee4660bde19b9a50365ef0b0f3e08a0c937fec6b524fc36e6367c2782b6a" },
                { "ff", "56eb04817ede7710800b96791ee0c3ecbdd2efe33a9e441c704c0fcbe69d487e5c1c4ba8f9bb12782ad2ae3111c32c6d0c29c9fc26141693afe16c8f67a4f134" },
                { "fi", "8a42c30fbf1f868e9eea67807931cf0839431c3a9a74247ce21ff19c747bc63d65aa673056362814c7397fb3e022725bfa8547721df84e646bd965d8c516d368" },
                { "fr", "f1dc55e96fa5db65ccdaf3ceb75b23abd6dd1d17042730c1e196290eaf257b284fc5b83e238fb9fc50d81acbec366338694ce48a881ddc21ad646757220fd5ef" },
                { "fur", "146e1d2d5e0f070220e7912b84caf33cb2232f7ea32074b59365f9e815381f19b3a4a441e931e52e146509428863dd8f3f8a91825e49c4bf66bd20cd19ad2419" },
                { "fy-NL", "08e2cd2a0522e07c91391b582a3618cd7b59e266238865b390915ec421c3ef52348cc35affe904325e8a05e06fafc0ece7c2b0827fa00e86a8793b08209de47f" },
                { "ga-IE", "4e5fe4d0f4dbe70d6356965d28f940ad37f3f71cc50cfd9b0557e7631d6e93a8371ac65ad4b67a1bd1306fef7c0e49d95e7644d195d3a21c23d10f50a64dbee1" },
                { "gd", "65418fecee36c2a6c6fbb7a3f7884dbd9a363f18804892c4d417474779b3aeeb623a37407cc3271172a75b20b991528d7b157049291249220173403d21fa4e33" },
                { "gl", "660c2172e628628af2d5793882694f12262d3f8819638def24e9ed700a5e6614c1d87305494319db50ef2e77572fa737e27826650924fedda5a4705c38fb925f" },
                { "gn", "353fdedc77bd2785f463b7c634a5cebba0eeeed91bdc93ad0440da9bad7e599c2135d3277f916a47f7605ffd434896001a3cd64599eeea68be637178764e0257" },
                { "gu-IN", "efbe9ecc01bc3351787f9e07fc210aa9f1c9588bba0b01e20f7dc4e48b0262830327fc6736f1702befd6c587e552dcd2261216d2c140163fed6d5ac0afa9871d" },
                { "he", "fbf29d356a4dd4e42ea1f57d7e72f6bb7eec0c4490317e788ecec580db399c973207f95ab018f835f65cbc25a263d330f0bbc44d37f3a2b40f4e8cb1edc1e2e0" },
                { "hi-IN", "a06bb0471c1cf176702d0c1a19ae3776b437d49d242386d7fd6535727047dfd05fc706e4e5e4f6e099d5a249e9bcce67cebf81e584e75a1a1e43a7727002ca63" },
                { "hr", "83c266ef33997b0be98dcb6f86dd622539bc542a4607d7500cf9155f92ab0bf836a09038f719b83632a896a8e072a54e67c50cd0ac71b850444f0e63edc76f9e" },
                { "hsb", "3299b8efffa17b1d13dd793ce50ccc95097e9796f7a369faa261c0e2f9f917490d9db71fbb4a8c24a50c908c04f217bf903a1214ba06abc1fbf9fe12b04f8671" },
                { "hu", "f53373ee5c801c771bf92f47f5037fcf47a47951f785bb272e510d869f109bb677264cb025e693bf42f08599292c3cc14e532afbecef719936960aeec7098913" },
                { "hy-AM", "6841c915f3a2189691a5abefa9855766a0d31b8eb80b3bf79ffc9c4f078a3f17aa4650f54b9c52b15523dfa53fb3fd24bb2a252ee14d7c64a9db874ce6cf48a1" },
                { "ia", "e41551674c963823dee20205efe0cb6d18bc71cddd100b783dd9a3b700fb8c99c13b4969d852122defef17ad8aef15b22801174435735ee15de774614c8faea3" },
                { "id", "10febbc7bd44a66609bae3a075fcdceba6db966c44058f029f25d18c55e9d067b50e44a872f298693e9e8d4d681d29cc02d5a16a54e2f4ffc1be93e8631230fe" },
                { "is", "0be3bbcf61c8dbc17f7377b39353abf56daa09e5c4369b2b9119b9be1fbc5c4d275f7828b6a3aa632c0e58d20c1715918b1987c441b3b53bced6dc2cdeac06c5" },
                { "it", "5643e3d4565fd49a12664887aa7c16c0443b16a727cea039a57a902bc9ac8fafbb1fca4d1bfe234c3b3969dcd5a5b36d49a49a33dde5a4fbe885ef9b951d7ad3" },
                { "ja", "5ea6c227d34bff280f4d39c71ea646af468a5050784a69f16d5b1f683e606c91e547dcc24310b6d8883254ace4ec60b0cc451012f63d447e900da11ef5dfd268" },
                { "ka", "6df80e903df8fa2c3cf3f3d81e735f2144a57b77b329c0e588dbccb5b61e6af8afbd148882cbe450b7ef1bf0fe11ad679458dbe6794700b00ee4c201890f3564" },
                { "kab", "9047423756997f1caed3f0236673aceef7304f2fb38ac417ee51d90f82d544c72b9d7cf05988f9726c9b2850361d978519138950262abcf51576cc4ea24b5586" },
                { "kk", "5c24a2f4774d95083219cb6725261e7b83eacc69b5d246398fe62cdbaa0c4040e6f56503e21141e677737690a15c4e88cc1cc5c065ef63ba6354b3ecdd40cf08" },
                { "km", "1c4e879b9859c2e824304e81fae01b2e4c417650fc5818cfb35dc1a712143e6ea61b9fa78094954317f3796d33c875510dde6defcff4574732ee14da42cb5a4d" },
                { "kn", "3e95633915c8a2ae2ee6ef2190386f97503824f672dc24fa84f0b3edf80b2103df3468595fa4fb50a2b05405990aa56d1a7dcabf8aecc178cdd9085c6eabbe74" },
                { "ko", "0fdbca677872ea3f9abd178e18550463cfd6ba9139a7625044f93832e7905319c6e19185123a09ad90fdbd4ae3c316b093d11294d50deb975142d2183f191352" },
                { "lij", "20e930871c938c5516b13306c32d6a6863c59e52d69761f0f5ba5e8e7646d069951d0572a3064b876c6c6031bec15f113ed00b282fcaa64f84b09d086a9caf7f" },
                { "lt", "49abc3ba5bc82bce2dbac4abdd512f9453c1cc0f002d076a92cc2378e66251bcd30601283efee7b233395a8b1d401ad969aee1abe818c0e25132d43b04543e84" },
                { "lv", "36a29d0eadfaf377e1f33420a34f8f065baea564a09caada69919a9fbce903318ca6ff6a41f3ca814deb110349442ee317100b1b7df834e369b45eb667dd1f8b" },
                { "mk", "d815f4c6a46151a60a15daf04a7786ea7a492a614da56143c30f72dd66a258154c5f3465145873fbce0b120346727638a9bcf380eade47738fca7d75ec493fec" },
                { "mr", "9dff6e7ddefd2ffae353c83b4978bcf95a1fcd10d6848dee2bd62e899e30519ead9321acaff01e1e471ef0c524b1f3525e1ca8115cbce31a13bbd94f49408e81" },
                { "ms", "47667d58551a74beadabb5c3d6caed14ea8c0d573f932be54f206a61ebf210e75c71830ab644ab476c53a4d3ca5dae21a30dc0da026e115424539ea4f8defb04" },
                { "my", "33aa68bfac6490f01ef45114c2d766fba20f8783f9b1b6ed41fc8c36775ec117134dccba4c12ea4c893e32b8d38e12a64947a8c3731f4e86f31fe8486c46faad" },
                { "nb-NO", "84faf8de179cee8e8a03806f9bb216d15458024f880215d3f919a23a7c62d81882069b2166414021f3a97ba5fabc176188b39c9eeb735440538c431266f41a5b" },
                { "ne-NP", "6d80fdd0b1164150ed04db972da3cedf809e48214007d413d7d9648ca6b280fea9ec9fca19c765da277cf052cda17cb9b536e6329abc9c730c111d6b0506f2a8" },
                { "nl", "63840ea040a17eb2c49945c72b70adddfb09d8b255458195bc27e723345b3f238abfd986b7449e473c9a059e6df7bd42f1d2924f52a6609da2a2fbe3a3088dcb" },
                { "nn-NO", "ee52d421cc0ec1670e5e5e1db34a225d2ca3ea0da1cf84f726d33e2021a2ec0b24ff9b060f49cea1a8c1cc22cd891d1012fc118da46598c9dfc68796355488dd" },
                { "oc", "9939faee1cfb1d43687cd2a22d9c837b4db5ce42c28c568669b923831efbbda60d2cbfd8b501c7db3053020f1b44bfd6382a7919eb9e75393e087bc8dbd4fcf6" },
                { "pa-IN", "abfe1db61da50c55b11fee345c3be12090acf00e5c0321b6afbd28ddfee44e0d3f88f9b2b991a9360961d9a33c1ccee7c3f6e5563babf452b8d234cc45af1b22" },
                { "pl", "cb47a1db72da473a146024329d9628e61443443e9bd12ea5b43d9a5cad9b549f424018f565717b9cef3722401ab0b4a491db9afbb09eb03f9811ffd18cb239a2" },
                { "pt-BR", "ec8e92547d307e2b60d0a3ccdff31d5f98d5772d1b8f4f89d3a7651b9c6cbacf1d3e301d3a427fc996cad889c0f6a9799f46e5d4693bbfc8707e30bc723afe13" },
                { "pt-PT", "764ae152b8a3cdd809da9f590d05c97f79b14b58f01c78d2e1d672859d12b5b24a312018c68fdd725df97f5c21427c716af6a812293824965008b39bcdec8b90" },
                { "rm", "a4756b278a7ebefbf37d0bc49eeaf92f73ef1a7e54b59c476e841fd68c716d25dab94248172ebccfd8da6b959e1b6a825496a466f64cf32a819213905c6792da" },
                { "ro", "f0553f56a96f40aca865aa15bc5b0f03b32479cf4b6fb3da24ddae0eb46ed3d7e05c78d0bdb4d14e45070d775b669421e8183ec0afc3fca009296aaaad684f05" },
                { "ru", "232a1e64a9b0b5c9ddf50152615cb5d31712520c3b253eb756a1418495afdc4dca443cfb9502b5a44dcfaa0f14cf2a0efd25b492ec406c6451242489c64f6b3f" },
                { "sc", "b8a41c18e01d8061fe0f5a99928e29fa30058c4545b37dbae8f9263fc476d3e59feb348e19e33686ce2b45968c3bfd030a4f98156efd14d5e2b5bdc36cd2c3d5" },
                { "sco", "66bdd162a8e97089d811e07bcfc98bcd25ff58f8517c8e2216c05a04a870dc1a777f7b7a3da4891accdc612bc996cf60830be8f2659f489c605899ad62785172" },
                { "si", "b0802e9a83e0a1c438c21717f0edf77dc576ce2f1e162d287e4b46fa7195007ea5e6a04c0889f512e43507f4fb90c7cbe7814a6700cd0e8a090e8bc0a809a40a" },
                { "sk", "bcf2f0ffd3438885c8498574136b5c1e444380d260a0fc69b8091452475587aaba71be990e390730ebf320cb3cd95dbf85ec4e49204a07878ab70afecb1cab32" },
                { "sl", "856271a6578abb0a495a8a1e7d2065c617818251306f610475fe940bf0ad6e1f9d392c675beb33626dd331204103435fca067d491f87e9576b4b5bc5f44220e1" },
                { "son", "e40e23024a65d39cb0841e7ec1f9510f003e26c3ef83b45e51c90caa16dff801520d263169a9fbf8857bee45e125811e2c803460eba9fd5a1f5d4a3c99f3180c" },
                { "sq", "a326e10662516ab550491e9109203bd6fe995544516c9cb002f2892861dd8f9a8c5a3b1d0ea0c0ca1747a5f368e85234e310fc0dd42b36912282079e11f43220" },
                { "sr", "090ba8c9599eca65988f8e5b01b2d49f8dace2bab1d7f05e534de7e6cc4d3e991f6b8760310b002db72ec97870ea27b12599acffb8f392e077028406a353d07f" },
                { "sv-SE", "23fc68fca263d434dba361cd7e75712dfdd1fca8a3a39d91b27b506db1edc13d469df106178d71f69a205a4daf6d8f931745d5ed728082267800810337cbe292" },
                { "szl", "03e350c2dd59cbc6f78362d42deba86475df5eca7599db96fb132269ad91c570c884c3d5eb10dbc8e0819a623c32f0b7212962bb9df91acb06d7a4edc13d51a0" },
                { "ta", "c5d6f5c78544d00ef618432b36c8105b0708e6ab0a7b9eab6eac909ed1b6330cce9b19c6789d2e725e5d6ca000e852002fcbe283338d18c0bb34845535e17fd0" },
                { "te", "21c432449a3b3273ac8a7bb87ba9cdef7d668875596fa17741ec99c5a71f1700ef61da3252a98896c3c22bc8108a74e9d6d0a0e118c33f4deb890d7066ff627f" },
                { "th", "1c83c163aa8097bff38976e9c0d4e5a345f05661f49efd4382685548b88d8f9e5f1e9ddd08fc74618c5f0a2a870d5ebb98b03acb6e20901adedf11cbaaea61d7" },
                { "tl", "86bd3928b5d59d2d5d07334386d7c21e4661c49d20e1744e6923ee916cc0bdf275b673fffb2e5aaa7ba362da49dd61d92714f8085b5c4816aa57a5ebb7024081" },
                { "tr", "be6fe064748fd9af874629577e3e40bbfc14c6e5d707e06fda168f8a5324601a2032df039e0e384ed0fe586f56d8fb811111fbec0c09adbd8e2bdb4f07b89c7e" },
                { "trs", "e3a3d79941b9ad1832c4c2e15c44ce65e9476cf954827fa5686294556b841a3e940768f18f763e49914f13b8cea314293c8cf5ef638c1ac1ad97b32a09854716" },
                { "uk", "81872d8a143b3596cbbb94f463a9a2bfa0884857de5b836b2e16eadff95f9d73b69af0aaa27122655912cff8dea654328f126c9ba6cd295157b88554bee7305e" },
                { "ur", "9f911cc6e1314ab781d7c362394d423774250620791f65d4b73a1756d699de5818941d8e844b98f3af819fb69773ad593a07c2b80132caebf26c797ab7e62c3d" },
                { "uz", "eb3e6c570962fa3564a0b73c5fa041d35efe1b7e668b6e08114688996ca1432470ac0a2f7f2b659c42ad0ee0c3eba179ae0c1b54102d630465c6629dd76eac8f" },
                { "vi", "8e3d3ad3251e86671e035f4cac7cac598d8e5ed2955c439c98e613b9c052a4f84dcebf48ac24ebd5d2e9f8d5263861adbfb8b318713b328c9f1ccc846bf5c16c" },
                { "xh", "8d98ff8735cf45987f24de0e03ba51368bd54b71e8ce454b87890b412cf80d4c5a46797af8dc60232475cde35d7db83288fb70de79ec431f1b2785da6a14651e" },
                { "zh-CN", "ae789272895c3a355a6df9cf7d679a74c8fccecce5635c556c444eeb67530c7fbeb37efd46d0cd795792abb02264e6ead1f6a1a99149499cd7a8aa16f492fc34" },
                { "zh-TW", "16a7e62e454e2c761c7c1b0c596938f38f0fd18b0396796747b372a05ce9f0a7488fb86d80526c1c4d2d506184f42ca2bb6f5b8c68c7d4e647ef8a682b0b66d7" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/112.0b7/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "64a4fb21643caf6389b477ed0bd11d94a2ceb03a47c01e33849a34ad584c63c848abf81acbefa74f94b5cd7532e6b49051f22c5e677ef12c5bb3c9bd44c9f193" },
                { "af", "5a865f4c23e0f038522d0a817c1e9b65d77c48b8e62262bf0e6e4d1bf567084fd44ab943b0d94837cb1897a92cac6c22556c3ff28dae01b19ab3a65653bd00b5" },
                { "an", "07693fba4c1dcafa274a71f67d18cc44d4b5638ac1e9700bf2930087a90d4959daaa9a3e6ab58a3e8f039d3ae0d5f6ef3b8bfe93db3b76fe743d5f7eb20c631b" },
                { "ar", "0cc960256bac6a5d08b8ba2dfcb5b4c598796040406ab4365214760b51d29968f85ee5fd1f440429dfe849e57c71950d2a5edc3e8597491a0201fb0713c157b4" },
                { "ast", "79e0caf3282a9eb884c75321f93f86bfec0b881a0b00849e0c0560a5ff4ec9f8b1df9667480a51886cafe0a74f109a50d4eafbf82740d7f3c8016f80c6e2e93b" },
                { "az", "c88b70f5f95d40d8018b9678a96a6b81779791651abe6e78edd4e8b3b155135eee7e142ed9283b3535b3a2eaa64a23c6bf3e772bda3e7cd9ac0cea74d3e6ce1c" },
                { "be", "f00b1a3509ebb3496c73d8afe3ef427eb4ec3ea9ba159505e1ae413c18c32ebe5e159c1b1705291aad0bd471b4a6890e46483b704c90d85a1a09a6a85107605b" },
                { "bg", "d03692d43abfdee7dc4cba966293f54c321655290798d3e86e59ee6732d94dcd79daeb78d9e8703b237b687dbcb458b6fdde71301e680064623024bc10bc37d4" },
                { "bn", "1b598b0503813661b3a16ce470f68626144e223c782acd9ab92a0c3ea2b80e1d91f88241a5439777903129832a2e9563b85fe0cd5e8e3354eb0bc236161444db" },
                { "br", "384cc3f56532a765fa6cb3a506750e69cbbc45b3733d90a0aa597ff550e57d3bad7096aad090f55c72e0ae09f51279600055f2dd5762cb56048919d361661dac" },
                { "bs", "fd79f973f750fd950ca4d395dfe123a721eb465b77cc6253ebd2da6d079adaa01e0301099991a98a2eec2c2772ef0cdb507cf49ae10aea9526935b3b01ba747a" },
                { "ca", "91c360b238ec4442526e3acf33f7a0513ec8335c2c2f833ccc981f3a2ac61c4d3692993ce712569e10e09f8e703b2497bcbccf69bb543c3424f0c3a2699fef1e" },
                { "cak", "202a89178ecbd2dc48c88c9cfedaf021c86de6e7aad764157ba19f4b2ff42c766c2cae06be43c364de067d58cab1a85674cabc42c6b5ca58221fa973a542353a" },
                { "cs", "dde87334dd496badaa71438cf169d8c496f26804772dac85acd4795d00272855395c335c53a3def18dbfd30115b7a3b805f10ccc891e374b57975000cd2acbe8" },
                { "cy", "8d2391fc0d9f996d407e1eb90b7ee19a4d2ca3916f836144096979ce079d4e8ec86e5e91978d77061aa4fee843678da4ea7fd801d17757e0c465ea90951036bc" },
                { "da", "d55c41919242adb6143f1fe8d8f85c94aa25a74425fd7f6d6cf0eff68a3c914a7cde1bfe381613a49dc0ca5e6ee604c9a2024fdd5b56fe980b115e6268c075fb" },
                { "de", "80f706538680dc1e2d8ca0c8ecb8cdf5769602caadd64e72bc8bea8335c70ed1b230b86b30c2bcf160d583f3529e0928dc029ac979605abdd2ddb3b1280bed50" },
                { "dsb", "741b168772208059f84d8cd8e9ede9544b43826d7a5f4ee78eb8be6840eab114061042e6d63d3b1a715af86e0d951d569ab7c15166c9a058d9bfa567d898db8c" },
                { "el", "a186e0f71490612bc2f8bdb980093cad6cda1a7b08dc4eb5a214f3ea3c41f9f554b3739e09da225103d00c60ae61b209a664ed21f81534bb27aa2c0ba96b98a1" },
                { "en-CA", "bfa8bbe506ccdc6b3292c50a21b004a2824463cfeafac39add3058efa97d119155c6579ad6d1a19f479e8ccb09ee96fd6e6fbc6d99ec51ea646915e4887eb5bf" },
                { "en-GB", "3697e4505c2178bb3e102fa5d9758df573a81d6bebee8769234dde75dafd6881a7d4d574c6a0449c0c2e58113e7ac6d2e011ac27441578c3bfd3abb034dc6977" },
                { "en-US", "235b2c15ef2747ca36f822cbcfdf33c2b4179beeba6ab6510a9d45e05ef9c26d5d18937c5c18f4d5cd6e11d13a033b726dc0a0c811ee12831f82e67a3a50c008" },
                { "eo", "03eec7064b8c029780e4a2f196b9885b61d310e2516cdfac05520409c1cf5e440cdb6e90b379dbb6646165a37c47588b19f9baf30da7884345b5f9c63ad9502d" },
                { "es-AR", "0d6e08de9e53ce7e091129973716d42356f7d6befe164eeae44a254a52cd7863fabc3893827e49c9921c92a035d112edf723d4368d04cd991c08b4e446babe3d" },
                { "es-CL", "a5ddad2eeb79102def3d52d07c33fbe11078cbc379d80b87858b2b2546366973b103bb56795344018ffde0593a7d725561d31b95ced0ec2a9baab480217d6a52" },
                { "es-ES", "11a066cf6d36f549277687c3604a0c3f58d0dd003933117c58fc39faa0123a66813a007850778c310c0222376b845b32df7c1d0944e42713b75bd4214b337bb4" },
                { "es-MX", "376907ea5e6f49cf41491231f44907fdc7c7e70e437d66d95b346b6da62ca20e531f57587baa60a04a9fac7ba0622f81eadc190fee88e25836ad0eab84074c5f" },
                { "et", "4498dbda741f6710c2e08c502a08328f9022c731546378fe2b11e8e62de9895e26671fc4abbb0d681433de8835deedba5f6e51cabac8098d9c5a0aac2aceac53" },
                { "eu", "5f65cd8b75558527a8d2f47659017962fa40154cd2ace725db85d0c6b9e46bcde7f9331d4f1f6e95a037f03ff7d4ebc9a18bdb861a43a93d8b97069ef1e54e40" },
                { "fa", "2254c7ed4e775c2890649443f779f028678e7d9cd6b89e2b6c8f5c3cfa1cf5bfee5107cd79f41ed169500e2fa097fd8dd6f5d0e694276182393609e4bf5abb17" },
                { "ff", "d761c7ddaeb1d5fe95550249761554fbad11b044edbdc45c4dbaff42f983d4ae0ca4d350eddab6d5b693b292c0564460b5b0995ec251f24a970da739f317c94e" },
                { "fi", "690c36ce51fae257a5f03ab3e0d59a1212face50cd8bfeaf4ef24604a0f527979f9380f4177bb39e2bd21a3cfa6660124e4c5dd762b3e2dadf5821defdf2fe1f" },
                { "fr", "58c7d406d50d8571470988a91470d8f0ba77866667c5a04dad530c8a68594a062a9c00349825686afa62b1d92f69657abd65eef6e160c27909564b63dffce0ef" },
                { "fur", "e731f167411f0047f0c101d20f58a47f88d1d811205819ec3cc9b7d89eda5294388a8aeb806a33ee7cfc97eb77e93f45d73a8d7a90f8013cbb76996d5a313e5e" },
                { "fy-NL", "3f2a88e95c5d0186fffb61e4505c3a54c8df968b32d8824973915af0e773b560cb4cf739d9cddad4fb057a3fe7d26515bb3bfa7aef117d5699c4f4e98cfeac55" },
                { "ga-IE", "9762cee67da7f4a05bd6cda9208f4802e611a7607a8b2cad8f8ea3092bbce56f4cf02c070cab8fb8e2f320e2745061fc8fa4573a08a7f15a770122c91db51431" },
                { "gd", "e677c2979abd52daeb2e22ce11cfc36647e4ae57aa11da3a01ed482a912115c65bc0d76ae2226ba6644172416645726657ab468b8445303acd63b499b478f48d" },
                { "gl", "41f0cf8b2633248be8c715b32e772966accf2442ac716a6f629fe694289cc9e6d2bc0bc8bc36a0fc0415ad673b05bf73dd3dd2009e3a621d25db4964fd26fe25" },
                { "gn", "62bf2ec8c7aa98fd500a2eabcab6e688a47a60ed7a06017e479b01ede964096167ad816281af3fc7ee05c9850c0039677d19c0b9d60478424ab2b4ecf7a49d53" },
                { "gu-IN", "196900d442b4b591bc5d901c0d85b2dea298240964b678fa20f26000b423ead7c2f9506a7a0917da72975064ed2019e2a00edec3e2c9b5d491d1d3e1d507ad83" },
                { "he", "f5f9313176b43b96b52f2a97665af56c1d142e7db325df6f711f84abd127bc2a98cebe93c0c4ade2bbe96b972c4762873861ff8e4ecb74b252107b9be58d3eec" },
                { "hi-IN", "0e2637b1459fc8c0f54bb08f37fa3d04b1ed549f68cbce96702e1692ae18344f91c4d174d66832fcff8b50f166fdd6c5f3529e07b3747c8223438bc0a89deda0" },
                { "hr", "bdc24efa650f500c3188b08edac7f2224606380875bceb591de5e9e52389658fae5859ee61351ee728a7d02335a931c00e95542a078fe14116f49e7054d9f33b" },
                { "hsb", "9028e2f5fc92f445500bbfdf33c71c758a9d51316d6b9413b704f09216926a9f42c647d03d5c3969c1d2a90dc6051f2b1dcb0809e02628a27383dda172f46635" },
                { "hu", "db39b4a7a4caa5066c01376cecb61c5feb1b64c113d59bf02f1fb72ae3ab55b5820e3c10ac65decf9c19d7917064516a919f914a8c240217e9f18df2674b727b" },
                { "hy-AM", "69bc3728538ac047096ec609d57f31bc4a0441c95b253f603dd49adc47a0f4707bafa1c5210eebf8ca187495afa4195822d581d02248042a0ab23eb1bc6dd41f" },
                { "ia", "77bb96368efb8e3a1324f4919b776f0e549c1e653620a90adacabd2a9b40e2123703e818aab6398aee0bad20c797ec6c0ae6937568a5abc28010a03a240e23b8" },
                { "id", "9bd19627a1f7055bfdea54d22866ac7f054a0c3190a1727c4972db439a4cc643b3d98b479bb9d278963427d4f2409be240c978f5369c03a7eee7c792724b2e20" },
                { "is", "3a3e255fd61c33c12dc166c4292ccc335e6a497391b8c4dce51fced589be70b417e8d201c7e4fc942dea07ef7d351a4d6b7dced21c8b2a5193b7b2929155e0b9" },
                { "it", "1eef931a46208c5ae08dc9e7421d4fd95883dd5b48ff720451a913dbebace0a76c875ab00e8d505f26a3fef0fc29e8241765d063cb8a7be3fec2140497e85f23" },
                { "ja", "e618764ceda3f90fc72f939f9caad53f0e2a441990fb655455f2c5488a95a91318d531b0db3c796c6a479638642950e10327129574bfe6a086995343064d90c6" },
                { "ka", "fcab4248fcea79e24686850e028b5370e0d48acb6b2e00da538999823b024f5a5eb527593284ac97aed7ca3f81df9d6610d739490361d53a62204ab4de707f72" },
                { "kab", "66ff719cb90f031e7ea32945af30f0bd1a69e79d9a18ae4a7808c1771040b6bbc7d8399a3e9f7c03c469e763d34cc5aa259e1c274ace2795ae6c33b4bc1bf0c6" },
                { "kk", "c70ada4573b222f8ffa9474acfce08d01f6de4c667b4b36b9950290b0e1109c244ae9b5522910e1cd0f3255d188b63eda5fa63cf60c980f0f9d3caecc01b38f2" },
                { "km", "0cd11ba9632d32be05980bed657b006b2a8dbefb713420487931ff745178bdab6b582144295562cba35d934ae9fb2c0169459a774b6f43f1785787ae82710717" },
                { "kn", "c142048957494128babb0838bb6c0daebdeb7e688dac539bf16910df906c1a7066fb49b2e0e86840b29cf66c62f39f485a1ad26cf932d957a191e0b577f0a4bf" },
                { "ko", "cd55674199d401ee07da94244a91d81c8333f27dcc9676545cec934583b6a534675b3a95d2bc4d2c83f8cbf5315a617e8216252d1270e9438618d04d7f5aafd4" },
                { "lij", "8a5dfa3d76abddd362e1d9ceac15b534430249bd927f04f24580a1b330e5987f1d59618e20f37e74869a657f6983f5158b2e91cfb13079ab8019cc6c202ce8ad" },
                { "lt", "8c7595db0ab64c4a802362099a6e062308f3df9a26fc547935cf49845c3f00497c2cc69dfe00278e4aa6b54c3a73727ca3853f0c015d17df850c5ab5b3b6a464" },
                { "lv", "e3a5d4e3c705c13344b5d8f44ddd6673a54f533942f6e0c64067e012bb4186e6cca631d0b02982b261512424fbb52628bd8951f88320a361236e8fd8cd289d0d" },
                { "mk", "86892bff665aeb718a66b7178832ec4c1a8861280e291ee63bed8ecb6887726d0723279ada2bc7680a815f2ec18caacb875433e241f05db102b4e7fb50ab0fef" },
                { "mr", "7ed6e3359819a0c3405ecc22b2f83ab6436e4d5e2a043ab6fdc0602ea7db5d4654e8199bbdfcf7472a3e3261e2589c00a43a36369036e7bfec9f254621b0bdc6" },
                { "ms", "6b064f85b84031ed08334d9c93165cca206d42987f6febdaccb6bd2a4dfcb6e36405f0448e00a90280c34c1e2c264d01edbad94b0e18a17b1773ed68a333f469" },
                { "my", "e634927161e12eed7562017df48c77a65ad4b071ede377359ee82011bab31e26151e4863d44806e518b247681e4b95c26f2ec964477e0a165657d0c07d740a52" },
                { "nb-NO", "165aaf241dc08fa5d0e2bc9d51656421992cfb9de3374d5180337915182e5298ae93b70bb9bea7a511f3d19c1b5ff573b9001227f58feb77d3e8bf0032c50981" },
                { "ne-NP", "97c75929b49453220f6246d03ef59271b52cd2f98f6bc52dc69160748a92dcd289e79ed6a58777f55289ef641f22e56f87d2104252427b391cd3b92cc7e62fad" },
                { "nl", "2c2b0505f784a810e262c5a05e286e444c175d596c1ce2ad33cb2bea97e94c93e1c3d37dad0be2f2c79e1feccdaaa09b2fbc68859e4f30179b04749569b115d2" },
                { "nn-NO", "f0b4f7eabf966a6fa18e37ceed375014e4d3ccc06c7f061c42ec3a56a16b7454780f81ce9ff4a0cc7f5a910f3084e075db99e9feefdce0c9b5b930eac375711f" },
                { "oc", "0f1cee218e90ac0212b73e807a7d11bb8e5df4cf9af2d111c606ccbab9f97d11db19ab7ba6c6d05b666f1e5fe8cdc58f8e208d41746d8fff2f88e4c1169a6305" },
                { "pa-IN", "79622b100ad3c368f99d2df00b49bcd200f19a690d80c1e329cfc9dc7a0672cc95f902104a10b5a0d220b8d21c22f878282337a07f9e96c998bb29c5857c66f6" },
                { "pl", "19e60d95ff4a18ad91a22b742d001372cbeb1c1bdeaf123db6e3d4e8770442eb73f065226d64cdf177bd3fa3ba42e4d97003404a3c5fcb779077f8d9948dee2a" },
                { "pt-BR", "8aaa8baf98f7d0a9b995d824e07cf684cc258581c50f8e419e9af63e9aaf359c5329ca12026fa4d811307f501192308e38eefd57de363668a7e7d5936f0cabb1" },
                { "pt-PT", "8b232c999af46ce19d19747fcca0292a5388b41c3a806b208fb8c47e2b3f45e7d36a73e4e0cf8776c0f8babbbf6409277493cefcc8399af93f257bb30eab05d8" },
                { "rm", "388f4783c25288976e63376d7b964c28c86ffcc7de16961f1dc343e54b0248ecc3ba411b92113c3d7f16c21c98d02294f883f15346c6ab18a6db38349d337cd9" },
                { "ro", "0822ac815be531240ac9b5b460df154f9e4f9881caf3af0f9dbc9861a8e3d94fc37b252874297cc8bfd743cde0d06e5ff716573d9f3c434dda8b6a299cd6c282" },
                { "ru", "0b22c9a11627b5fc3331ada1bdf06607ef83f3e45327c6d5360a3ad9c1bfa7bf255e5cfe4218dd9e96758f7549af6659278bf3cc339f107115f4438483773aa5" },
                { "sc", "034ead8b745836608cafc0cba0ee275ce32e30787245e482232603fcad8b326e5f5412033b81c840232956423f96e435bcd7b49b7ee32032dc67a3c9af2d54df" },
                { "sco", "6d797dc9e438badb1e8af53d34f03bb05e93d0fe94c042acf928993c24773b50797d9ea4fbfdb8e7d170654982559ebc5120d413058f36cda4c62c5af2f4e47d" },
                { "si", "0a67f7fa8c1a70c7ccf7b38a7a829751297d38794495bc14022a2be80838db6df427f286992c1f88d56695ef27d46e4860d5f571255b8df4dd17f6ec70243a99" },
                { "sk", "6da8adec93777d8bfe463b90759f469481e138fa7a93522e7c3723a8ffe68b134e5f77fe6a4f13de7f81cbc880ae4c6cf05dc04b39417997548ffc1b9397fc14" },
                { "sl", "08ee59941bd1f6cb799c58bc0a40690b2c546d31180b0138c43c60cbdad5cc281ec9c5d5f28a669f482b4429729af8935419d4509a3529a91c75ff97add00dcc" },
                { "son", "7a178829999799ea554228fd98f42fbf8e480b889c701162e68f715fee63446298c17f85bc03215bab34f566743a96201dbe3e3ccd02f86a341a86e8ef2ce047" },
                { "sq", "64a6005cdd07a93cccec736b17a8f2386ade4d164a00207f551914456fb4afd601fec4b52cfe119b4f9e698e9d8be481b86a1d5c7297e629d386d0ee60665894" },
                { "sr", "a902eda3f1ae75ec5be84ece90500c9b338fb8994618f555bdc18bd7503d44970d59cc73453795245b47ecced4388d56507fc8d47e98790df18822a4b3643d7e" },
                { "sv-SE", "10d0a1e7671b015b8af5cdf7d59c8623765dc787ea057a5cbed3909acffd73fba66690497e57bf07bf8bc0c36b5b732c3a4ff91260784ea0038afbf01d5a3061" },
                { "szl", "5fef187cb38fa67a3e75f844568394f28d832858c5e6fa830ca68684d0e8d3fee82c3b135856456235316b17a454844add3d5d3998b0f314726b4e1021f0985f" },
                { "ta", "09a63dda5c200d2823d4a59d51839deece437c03212fe06fef1f0ad0ea528c8b0023adbd97327a740e1704077a8d70d4990a30da52f37cf1f4d7fa3464a165d5" },
                { "te", "7774e8c24a4e670cbaa812ebb47de1ff35009727601ece9776f5df5842e33a3e1b19bf050c88fad548f20a49b3bd35484de65fab0a08b2baa79166ceed8b6d05" },
                { "th", "3a1af1a813105f5f14a581482506dd85ce0b5ad02367223ee0d358614b7bfbf784959bcaef2e0e89113e31c8b218dd4765ef866c9baa9292352e5a6c3d1e67ce" },
                { "tl", "a7e78ff42125be78d2536508ca8997745bd5da3eed903458ea20628978b85f76c9665be0bda8d3af8ab4859ad33bdbb4d31dbc84d69a08979877402fc5bce2fd" },
                { "tr", "0fec6114f0ca3065d270a3a5dc77d836c6ccb19f9f8e57653cf8a4e756f1b745f0ef5979ce91cfcb858aaee09f749728e5b17c23e788219a6fd5444442520eff" },
                { "trs", "19606e6605bd62bcdab4e2e98ef27b35865d6792a492e92ec1fcdb66318e749ce2c812a82c3d51d6ada137453e0daeba8b0b2c9348fa0852a8ebba3de2720ff4" },
                { "uk", "a58dba313bdf9dced76fa1d4c6bcffd1d7e9d2505b69a633b72bf7f6f22bc8bbeb1e4458ca0ecfb98bcdbb4782a41cb4ad2af7b43b89da463da51c8ec9d310a4" },
                { "ur", "903f92c242ef1da7516968ed2a2757cf8c61b90f81f6c3793bb9e702d25d8f9a7decce78d3d9dd4e0fd7e21204e31ded6612799143c100f24aca37c7789e315d" },
                { "uz", "bc8797a5e58b314ea66ef5f9f3366748b80c95ee60c5a247bf3629b86c011416a2096c948ea196b67408df443620431859344121ff2a957beccba1cdab8e413a" },
                { "vi", "8c0e49ab8002c8380da8f1d44c2d2eb884f3c81a83a2196f8cf5b8d06b77ae38a6f2be46e054fbcb78c2038d8edb0ac1d7a0eaaa1ae2f50ab3d5ae3b14acd0f6" },
                { "xh", "7bb3e8a008bedbffcae8fcfc1e420c66c1584e3ec927b3237fa748a1ccd2df198ea797c97ba41f1ba1aa4fca78024a6ebe9a2478f4fdacd35fd62b1be2dafe23" },
                { "zh-CN", "147cc4963738f2e62863d13923f5d59e77a2abf2a590c8f3ec1857a4d356aab0ca33ff2cdade8de2d901b741cf5b26a4f99aceed721fb78903acb83aa44bb791" },
                { "zh-TW", "2be90196eb771bbfa13caad52790fe0554159caeff0f085f2bf17bd25756b7357959beeec30596aac8ef59e80955d183e25c72635d49bc66e4e2081a8877f752" }
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
