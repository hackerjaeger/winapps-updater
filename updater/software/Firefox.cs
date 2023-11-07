﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023  Dirk Stolle

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
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
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
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/119.0.1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "03f4562af541dfc09ff0759d815a717e791e0dd90a4ec1cbed75bf87b32fa5ddb8493a4654fc6c7cfe83bb0349e36e1053662da5c66adb43f20a82024cadf44e" },
                { "af", "8e31e8add6a7149ff3a064d5948859c33c97fc7bb5c068456ede15b7024f3b43526638fef276824f1eb0d90759eb1147301d821180e7376f59c7bb37a451fb53" },
                { "an", "71631e976cea5410e4e497f1d34333ebcd0f7c2f49bfa4fad1465e87f0acf9c1921f9b4bb5a417e46b8bb4e91a1f9d78d787dbc046614c8e78c1372fe19c6093" },
                { "ar", "c4ebfb49754d881258c920a84dd9e231801e476752c1bec49765846db68e9e982d85ebc4c2f694a152ef1799d34c760a8d94ccc71025e0f1197e73195bcecec4" },
                { "ast", "a27c697d36c92c18d39c2719ca4f583c17e500211c8962c51f5640cd77207974447a192113bff10d00898f8e93f444d23821af74e1d97244845d7e7346a0e083" },
                { "az", "fe114d50e0c2ed4339a393093e8fec29522f81ecbe3d8aa54ef8d03d36d36d6094a174499a8328a86c422ad30b8ec8917472f8a5d78b9d31ec02a7219d341c80" },
                { "be", "6b5c3594f304dbb75c73853b59c615f1328141a63287d80415ad37584ee6fbb8f49329d08aaa72c4efa88ef6de028a252af590ceb442470a43e388305e9c13c3" },
                { "bg", "484b5fdba2b039c0cca4a7fc23eb462ef5cee5b5eb5b78f73cff28a52bacdcbd196dd1b42a5db36177286dec932596cb6553bc7c6260619fba5b2f6967c8b6d6" },
                { "bn", "af0cc18b006f78921f542084d5edb5828241a2d4cf37ad773a79d41675ec277a7a5e1c8d72fd8ce6595f470713ed898ae104ebcd198d174c0584ec95d108e980" },
                { "br", "c474a23f233187f71c25bedd22f78731ce411994590064a79723ade320be8fecc49aa8bf1617bd90ab0669a50f6133e4144be217c7322eb06968c90282db74c8" },
                { "bs", "24cc360c987738d1c6e01a39da39337a60fdde067d3b8921cfd9b5d764aef041c3931f255bfa587b1f262caf6d327f1b2a8c6882f3d6e91e85ebb4e58a80e0c2" },
                { "ca", "42091d2a7c9f3998dcc8ce115e3329b5d8a0c7fdf305b904b6514d6dd78fd231e0fa0a3c8af36aeea0e6f6489a9bd93966aea0ebfdfada94a24d7f70cfc42d72" },
                { "cak", "58025ccdc268ef5b8d9724ec9709ba193d79a04d39660f63b2f657038d6b54e80e9c61fd395b790b31226ef85ef7c9528c6e4f7b810b93b24eb65daf2d120657" },
                { "cs", "d8caa042c48bbe204998d3d778e06d081c82d9cfa76c457f4f4167e6d356c46c0598791c5ea2ac3846e4aac67ad8cf53d5f484e7af32fa72a9b6f06752d9b770" },
                { "cy", "69c00c1a036f6c134517861f425707e6a84ce21551b546fee6efdb0b22269322baf15ec413a5d617499d29baef457463977599855e1b4a7bd868ce317932bf50" },
                { "da", "68dd78acd67a470d80deb32c2c0fe2724d4ddf7f37987b0ea5a93c606dd7b3364f9a2bbad5e0b948c16a423c06652f32162673e62594d3b4946c0a6a729fc4b6" },
                { "de", "72a41e7e6482de02247240e062622b11e152acbc21ecb77ee98e0d4705d13854a4df64a7600e255b7d2c421c4fe4a5f8aebda4122dc0bce1a0b19443986e5d0d" },
                { "dsb", "c1c1123ace426732995aa90bef08c81e2e9ed063bb390efae65591029aba0f32305405605a0a98429776dc720331862d9346c0de214eaf544b5a2a6b566495ab" },
                { "el", "0a7712434a49e8df441964e9bab29edd6753441d904cb34b4fa05ecc0dc5cf09b46fee493e5ba175ccdbcd02caa0a3f896bdec718eba25b969d1eeb508c878fb" },
                { "en-CA", "d2c491ffc52dae80bd7357a0723afed11081ee3dbe97d0d10b1f5d475c53ef5763c91f4c67eefecbe8af51eeaa91a132b8e9d5f592c698ce8521afb812d61f20" },
                { "en-GB", "cd8a5470382b5a591bd341b9b9c2b7333721dcd1f55ec71f9a823bdf1077ebe7080d3138ea35454948b30149c7c2ef4a8d660d44b396700be09fe5bc82023334" },
                { "en-US", "7b03ca2c560ae2fbbf3027530a5f1b99ae1c814719d3c0143bbbdf895056fabb085fa0216101a53a3dde9d8b5f3d89e5209435b2715e4ef7c2bde0486ba08d73" },
                { "eo", "ba25dbd4ec1d6c4ace2ebc8f343034335210c811fd33d80b090b2be7e10f2d73541773560c802199f18a86c73089075137a916bd71824e6c8943da69082a7faa" },
                { "es-AR", "830bca027e0ccd0ae69d420f741dee57fa252c3e0d8df39ecd5eae3e0e589640a3b999f6c9dd20452ab95e6c2dd0e861d3bd0c056050a8f0eaa9c9500a17d0db" },
                { "es-CL", "2edfae25923d10c25a51beb70aa2e93132145d2d428eeeb1db2f963a5f9d040f94d8a160b62af1917c9243497f30c64f3331080b2a74cd387b18da12da3199c2" },
                { "es-ES", "0471bb0742b1a6de4c3a6defbcf6f2bda08dfe054ab2d879d47e24a86144a0070435a6877bfd6ed89bc5175f77ecc68ad227a1ebdce0e5a4f248d06029caa058" },
                { "es-MX", "dabe175c74934eeb7fcb10bd6f64877dbba0f35b28690e138efbb1f8a64bbceab50c7e4accce1bc936bb535099e4a5e6c5cf7519a6acfb910f2e6341d43ac958" },
                { "et", "21bc5eb31d6c4de10c87b4552fdcf993fb43fa99ad9313c1384794187198056129e9248086d3bf12fcf48b2c7f8bb2aa1476f55d2bf55fde72497b06a5780285" },
                { "eu", "1668432de89d171e3f45796bf39ebd9c27e08cb0fd9db296d85b91f9f53cf2e2a350c956c9fe72be2db574cf74ac19f01182cce971fc5771d05618ab1d5d60d4" },
                { "fa", "ec8cb0de2d21cbc161e2373336d8de6cafe20e4355d8ca0415415dd9fdd8c7eca6df6d76d03e9ecbe777a89f1abe0a8f247943450f54b2f6ec03225f7c591e6c" },
                { "ff", "a6dd4b07ea3860e76389d7b3c587dd5475d1c6212eb99e087d7fc23ddb5eb98f7b55d7e059ae2a3bfa98c4ab8f15feeb84bf8e920087b42e2fdafa48f92a8608" },
                { "fi", "dbb2a972b258225aa446db4cade5115a7d5633f1831b7c6ac351b315ea33af85aaee58a8bb00f0ec296ad9ac3c78cff4b03f948af5014a1dbe28129c4908d895" },
                { "fr", "778e35db532abf73fe6aa5e5fb3a30b253ab1e01ec3d9f05b5f6d8cabcb8ef6f6d4e71164476e5c57121446364a5770fdfa813ef51eb4f218f3db5427f24a23a" },
                { "fur", "6d3828d1dc4364a5ad17c87d426bef0bb88fde83432778c94cbe794b65d1bf8179975b3ddb90de6ec97757ebff599093fa44c307513847aad164d3962a6855d9" },
                { "fy-NL", "2ce958074b3729ae6412d9752bdba7983b633eedaa06922a0065d7371c2394475a611c5e3f94e346a9134f709e66339deeb217eae19628377300d17408286434" },
                { "ga-IE", "478a50e5c98c214eef25d036d2fc970c52bc15571dcc4bcabba90a1f4026ace8a403dd6c81564737010490afd5e06eb9ed50d9192581e1c37693d696dcb59a60" },
                { "gd", "d069deb33f7e7ca33c7d224254161cf01f8423f540f05c6d4050eeecff5b8e27fb25c279d0d1dfa38d16218839cacbe38af1d050a3b0c6b9926a815bb8e35120" },
                { "gl", "3a29f0bf9e4100d93eedb9e8f4be2dc6f99df7366caf6069ff3cdf85c919b1cd5ddbbe1f94fecefae57b2e23ffe586e434805ff663a4be1eeb0cb430f7c9e46b" },
                { "gn", "4f2f55944b4d652c2c15ad9723a264f2a827ab79353104de6fa520cdd6a9184b83990b10fea5d7878508b00dbce2cabde5196fbc22e103154277e3582d509193" },
                { "gu-IN", "0dac50a04308e3d2763787433b473429594e563dfda94b0908e334b0d2c07cbe654d86c657decebd7633f8064916f3f201c8fa56c71791ae40bb07af2af286a3" },
                { "he", "8fa4b50d1dac361a5672897b28b7b58163378353efe3c3639f0a44578567f3b9bb74ebe00a5fa6fad249c262a75c06a7f4a5db9d5b8c14e5252949d6a676e66d" },
                { "hi-IN", "4c8173f1fcf1ef0def5e10e94945cfcfa73604509d80456ae910050b6e68f964b203b2377e57a28a93c199d043e8ec783e2ac9f70ca3cd86a19c63a7de15752c" },
                { "hr", "7285f6106a5e219fccfbe33fc5e49fda42eece2d18c5c0608b1444c4814d3285591673c889762cb55f0bf6d2761ccbb51cda4254cb9d6a6328705281704f170e" },
                { "hsb", "f8d37b06c78552813179eb182e6385192eba851edde7abd40e2f18c3e993e3c890c3a575df99b98917292e6a0eaacd61b6c89005859ad995776fe1434028b385" },
                { "hu", "01065cb57ec321679958e89df4dc0d74fc1e3198832b35b3f0c210e27ed70c9cf9aa5cdc7a8d4ab501ad0b2266d106fc4714c38326eed587b5e3dc64499dd319" },
                { "hy-AM", "0b56623dafe710bbbe04d043e7bf6fefb26d3cc5a27defc0f5cbcf8db0f34e802a82d69da659e552f21fb9b92d16c8f135c87e9ca8fe21323cab09919a95d56e" },
                { "ia", "43196aa2eca78080b40faf899b450c02bafcc430f2dae17d7c634de714987d39bdbb3149ba1689361bba4ced72745745884392633c7c03386dbe630eeeb999c1" },
                { "id", "c58fafb1ccef0c1c1c614f63f21c0e9337e5cb98c232056c8bde4ec513dcd7e9fa67343d0c27c86fc6201ae35e8cf2cdf0cf39c9e0b695a73daaf737ee442f93" },
                { "is", "51a12ee3fce1e956605be59c2de764137557d497efa124120d8218b529a6635a3f812dbd1e696e9076ee44e0a88aa739be6e4ca27eb7f8596c1b92e81d284528" },
                { "it", "5bb68f65d83acfdc2379f41bdefc16713296a011a9f984d6ad4a9f3ae89ccdc4ad366050fa54737395983f84a4ebdc38f7ca20145a16e5f447204712aa3bcfb0" },
                { "ja", "f5df05df143a2314b86d6fbd97b89d4eb6a1a3cd86de1eef1ffa7500806b8398e93f96ba6ba6e7652b4729613d5030a7b0c8242c358b1b8feb52a7629e6dd454" },
                { "ka", "c2443ea08524c98b4a231d61b450c753dada546697ca2c3d0164adbd5a403176a57c863a280eb4a9e9c00cd4be5da995388d0915d0a175cdc38efcb4011cdca9" },
                { "kab", "0d94c304d7e7b289fdb83a3b8c923cd1943764b5c0b7d33ae590b7d96ebfa909e8e247b0bde41291db80e4a18d5e19074610e757ac7a4f589dc994450ae76fc6" },
                { "kk", "f3bce6b539d2f41107d16bdd261f3702cbbbb066c416ae288a6b46c316fb69ea6fdbbd6bada77c501364b61f97633dc757ffc77f2dd3b223ca0381650ac8f5db" },
                { "km", "8c6f497185eff70c298d17691ff812b59582cb72a079f54574c90a3a196eaf445209cd3ebc88cec84e48f2fa238b0bbc5b03dd014907e05ca324dd9d1e90b33f" },
                { "kn", "0e2022e8f7ff91c43d1b801fd795f70680c60e253bc6cfcf93357cb599a240ed198772f293d4f892c696cfafaf401e747130a96a15394774b2eaeeec875bf1fe" },
                { "ko", "2ba9607901d78fc4131e06954aa5b18bf9e52ada2afdc44e0455e02484aa0bac7338ce5315f125d9512e93b04187181aedc7ba5801eb9b59663a11b81f536c7f" },
                { "lij", "07a8d5276d5dbbcdb5713cba0080e945e5f4bc09b59467b5ef01aaebce72c1d7b2727f46028d16cb65947f136903568f72fe9f00e1e7d371a40d75ac53f3d0da" },
                { "lt", "10d094ba28dc4274af784846ae022e3c1679c48bd5642a346ca9492c64ef84727faf81104021373d87dbd5e4703bfc4a8a27f013d9dbd770fbfe88026ee8bf94" },
                { "lv", "0e00beffa11e575ef97510cb489a70348ee2d0283c9561be3509401cb8d2cade11a863900a40fb0764ae20174fcd4cc4ddf4871aabb1102275128bc90d4227d0" },
                { "mk", "f41f8fb25df3ae1896e1b1645e42d8a23eee8c38e810c8e6912e5e705ffb2c5ac145b62cf3a5b7e36b43a25180521d0d39b6164faadc17f0fd66805b9d5526fc" },
                { "mr", "8b6d0901cfab31dd86ff8b69398c8502e8110eb37b7d0ac3726ec64ae4e94168e27511de9654aacd62e98f1f8c098c8410b3a6057a93e5c26e7ac0c0f5371fe4" },
                { "ms", "41125f46b78fd31908be1dd406a7f69abf2ef882d6f716a7e8b1f8bf7bc1547f523bec6fe5cc1fbb6e02d93f2d41ebd1b91e84790a4130b0dfc74f496b659726" },
                { "my", "57463109ca9582818638c27fc20866888f0d5fb49596035bf0042b3c7a39561bf80e95f437ae96f65e47b264f7a13aceb46d1159c92d5a49cccb00633390b2d0" },
                { "nb-NO", "a33bc584395f94b69630ad95229888a4a9ee2c4ec72ccbfa0033ba9454102bec6b91c8aeb2960f00478ae5a3211ad9d8a3cc15b2d1182223ba7ab177f5f1a1b2" },
                { "ne-NP", "2b61d4a7c8fb5ccf4bc1f587a10b6b519e216a86e4a895f3cc0a65a6e7177bd73d4fe4022429953311566f283c70a76b3fb7b2b825b8b00c9dcd3573d2a1d0e1" },
                { "nl", "0eea7204aa7cbefb87db5c6622a2e85116218f6eddff1c0c1ef8eceb2f1531b767fe5afc5ec1a14dba31b58322d97d75fdd25ef674c02eb3b87f4e500d00a1f0" },
                { "nn-NO", "676693244923876037a2a55d685e8ade2a0ed6510951a9e5ae1f0ecde6a30e30b69483b4ff210cddcd21691e9c3131d8cb2c5f938c3253c9d8889e1093fdb61f" },
                { "oc", "64ad5ee25395b4a94ec19404514632e593c08b7f670689068265c0caf4034c08f6faeb9bc9600fb16943ae90bd93ae6d52c93e2f458fcdbf97dad45dce181801" },
                { "pa-IN", "464a8f473a7ae4c4ef2e824433c7dae606f52f015421709793d383e370ec07c0a8353271bc213e5b284c3f33a9e4e2f46b90333d40a80c8753c4af4f37b93579" },
                { "pl", "be32d4f743791fb66c83b4e08e4d058bdc2135ad2d3c99ebe0a33856c6eec851b033dc1a6f0a9462a1d6e1cf1393fa2456c3d6c68c2490a2d6b430ac96a3471a" },
                { "pt-BR", "9b8a72ac8d1492332917403bcb8a77f93544ac767b289e6199ac655ca3dfe7a62915f8a49d766c432def3216729c1489ea014e9b6f730b3ee2ede8fe5583a00b" },
                { "pt-PT", "eacc29ec1953084323dc7a20994cfbd83ad7e3625cec7a074e36331d8521ba63d8edc51bc5ce79979c996be5f73f43adbe8d9ccda024712041068946c207fe03" },
                { "rm", "d7884d226456ca986d06f03b6c9e212c809a3e3698075ff5e5ffad05f79c504205d364001962f1f7900b9fd886e3fa466c0764d1bbef31975bebf449ffa386d3" },
                { "ro", "8d59a777f688075ab1e2281dff2e7cf91d462025ede4e9f73e61aa8273b4b00336717e5d0f11d92068aa2ebb3a0b3a33f5f59bad4703a3436a751e51ee749a04" },
                { "ru", "f0b08b7f3bcda041171a9ccaea22c640e190a4d56f15f20c3fa430443a5ffb9ad8e46e8f71a43ee43fd9b22c4e7be42801e3839805af9eee89c2526fed4ed8f4" },
                { "sat", "ef8911cb2f13304b6b00e0f830bc73b64ff726f683130bdd7b03cc0abcc8f158ee540f7678159fcf8dcc56c5b6c18f4fcf018072494dc03439e89bbc622fe0a5" },
                { "sc", "11c1ce823a5f57684f78580e8e456beb526ff52c96dc1be7fc0e8aaf9f524c16ee40891e651efc74fdeb54193824ac39957d8d480d6376ffa4ef45e0adff53c7" },
                { "sco", "37ffab48c1095c40786d5c4f46b821fa4382f8dbfedd7121c731be86a63ae41611afaf0f2c82c9828ecc67e705e1845d555bd6c6bbc133936208bb81b39e946b" },
                { "si", "51c033bab9789bada03036fd4916ef91ded80e78af9bf68d68a5d41953fbaf72430e89b464efdfed668ac51ac0c0c43ba2925da397464669f8786cbbdcf0bd80" },
                { "sk", "8cbd2f3e9ba50f263935851d915c9b8f5d823f7ad6a3252139f3d921be02a5f4d9b0fee4d3c5a590970567bb24ee65b757f5ad43fcd08eb7cdddac1285bef4e1" },
                { "sl", "d8a78115257f2e9647cc657efcedd83e0318d3d3f5c21b391ec80d8616f4fc78d0ba2b5b01d4ea4b6182c6418508db02e5fa2f2634d915d1482c6b2f9ced444c" },
                { "son", "06a85ad9b81c23f4099ca201c63463fcd851182dbe4d20b0c61205e6fa8f9e3abe6615bdd758a54d570a358e2367465aab020b3535908c1628a1bbfd6c97dd57" },
                { "sq", "b7955da361abc9574b022660fc39425b607777a844ab45920457837d472c11aab4c2ef4b054f44b398bc1e75c825f6e8a771f9f6998d647447b08a7c1ce064db" },
                { "sr", "f7cec688f285ce2710008cf8042380855e4fbcc196a9a16868db154607f5fa02b55764b195a42363ba604671cb76dc2c9cb7427531beedaff68f23ad6e560117" },
                { "sv-SE", "c428ab10875010968f8f12bb571def691cba20f4c81a2d1cb2a5c8d791567bfaa2c359ebc34e129a13396fb2f3a362998780108b86672dcee54f361abdb11b49" },
                { "szl", "f2e5ea7a8557cdadd75770ddda721c5d49a98e82c93c76e78906fb3d8cb988a252610e23ac9feae83e461d479acf75e22919683e6d21385f0994d7d6032bd46e" },
                { "ta", "4323dcf2b9e9db4d38288edbed395bed0d20637e882080db494709262963431a93c64713bea4f8d193ce131b603ef1505543cd0c2c05e1448d59964a0ffe0673" },
                { "te", "3615b301e9523bd8ae02150e2cb6411e882a358ad82f7a3a8931911a6ff818b6ba880f282408f7ad953849685b8d82683465e52a2191d08161f7682e2e0a6d32" },
                { "tg", "26907c6e0a63fed467be43ea07f5beeba8ebd43403031410fae9270178f340090e1324a635cbaf2915a411d9004978f2f66e9cb9a063645c4c06b34d7e6d53ba" },
                { "th", "7494e008949a9bb88846f17ebf11abd46dcb893cdaf1bfe6f8297cf64e544c15561ee8b42571b4abd71457e01c5d381228a8fdfd922382d362de450c223a8656" },
                { "tl", "97edb4392ed87ff06c46a7edafb406014f47e6265774d26f28147a0d5de8eac5c6ac7cdaf29a31ee3f946779c480ab5b153fadf442c6348e660083129dd288c4" },
                { "tr", "78d811c3b4a02a217fe1942fe49d1092fcf18661b25d17babeabda0e25bb816190b9a5a17ceb29b5286771dd75361e32c853cd1e6da15544c52c91ddac921c1d" },
                { "trs", "2acb8fb2b23b0c6a16742f6579ae5044b40947de98548f8b209c0f079df58a6dcd1f589840da36dec6098a1addf4f553468ef7e01b2ef1b92816d899efd8f57a" },
                { "uk", "09ff4e29459d16ba32caaf2c0cce323b09cc141ce6f782a830c941c4f924d8063b905d21a9ceb6564a73daa057e20f848dca8eadd9601e27b0c8dc29aa3674be" },
                { "ur", "4c73304f934f35a6ec66c0c8cad31778ca5d731215488e7fb6f0b56c983aa608987dcd50513516e20ccf36934f9e736bc43eff09988e977d8b5d3783c7971eb7" },
                { "uz", "2e3cd9147e9f56a7365379e6466dab2318bc3962b8cb4d05704540da249fde49a58072b3183e32f7162064121035244376167376ead60dd6c63afba65a87bfef" },
                { "vi", "9b157b85ad3d164d50e696af8dcf027e2f3b4d44f0b421eee34822b99176a03c67a24d95bee4f9563ad2e9e8b837fcfe0ceb093e4fdda273944f45ae712cb307" },
                { "xh", "9b71e6bfd33c58776ec0f1ae74144f8f7f76e3a571054111811c616bb81e5328162a4c6c828731ab2495d4679efeb9351f4613740ae9d105376eda5f3b5269f7" },
                { "zh-CN", "ee41f41d10f8829b59ede61be512292f615d92d3c14785849fe4a8db72ababd4b1c85e3c1d458bf22dd29513d8bc3a4900e0ae33f7248b8edbfa69a511131c2c" },
                { "zh-TW", "24354b05490cf0163ace4d14cd509f8d9a17e4d50fd08d9aea3b6b57de1f785b0e5087118cf9f68acfeac91142decdac29415cb051061f5ab8f1a13c38b26bda" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/119.0.1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "4541559ef2d61668d7926ef7bed97d89021c30e5ac400318d33a0b039eebff1a607e475616471d4c6e4428b256f55e7dc1b9c720c57f676b68aa27d69d2aa9ba" },
                { "af", "f9043f1f650382fd57f8c49451a99a9d0b4ad0b8f39f89cf76558a002bba2ce75a610cdfa98178d2d9a7affce88226d5f3f07ba02ad573420860e40d4fbc40fa" },
                { "an", "4ddb554dc355185fc9901f89068d26fe93a14fc70ba34b7b817c4275d7e966f2ee93aade0326c7cee2a3e39798ff303a400585baf4bc4365ee2ef00e2a75692b" },
                { "ar", "9491cee1853966d65e47ca773c8dc0769bf45009958fefb23a530053cef5972e42b49eb0087ef0a186579bea1781dac936af43567bbad74db29e764cc81bd4c9" },
                { "ast", "b0ef0f311858ac5010aed468c5d453d6e99eb9bbd9cd409fc57d7b04f3725f3678d42d3f6679068261706842d5268476abbde0e2b4416c3eca686a6aef67342b" },
                { "az", "838e2f469fb612a22f7ea81aa61f6a7a085e8b23d6ce8333eadeaf99980c8418807fd1cbde2e359b150b7892098659d0aeffc85d6d5145daf5baf789851b3710" },
                { "be", "0aedeb9e81eb9f7859a7323b27fde60860636d72dda0ae076e08c4e3b1df6a7a59e84b8d82bae06d07eb15d0f021f595ea236b0c84f69debedd291d642789431" },
                { "bg", "19dd5babbc78f4bd6097a11d7b0845aaf01c94e4a57f9ae16599545e0689801a92fa6c640e9299d34fcb4d5e16be393f6515bc228ad09ae3146b5367fd0344bf" },
                { "bn", "e615a97832d5a10c0144e1f0a89e02a1a56fdbb6971a8f974786a57f4b895fabb7be4b3a4d66ddd5bc8c6a971f8c3cdc66025c8ea7f2b1e49cc6738d0530b299" },
                { "br", "8309fb9e8ac6eaf4b481cea84766daf46cb03103b37b82da1c018b88fc6984f501c72f6aa73cdd99f39ed5d8f49a578291e490f27fb37b59c1d3171c2fd13017" },
                { "bs", "cd642e37036d7d14cf56f37eb6830afc17d803d19b96c5120d64f96c53e95a466fef3fd0edf11c864d408a1dc4c23772a913fd16bb580ef646ff792221677d13" },
                { "ca", "9b62c705a97fe2be5659e2706e7c27febc65d1ff97e9e09441f3ef0cced89783864c08d46ec9fc16bb83e436bce15325f57a16db82f15754127390c2902fd259" },
                { "cak", "23f324c8f2686fb0d5d7d1dcf45545f645045616bb3b7f6680c03808990ce67def3712fc8ecff844cb6be317463ebdffb385a346971b07b0ee0a786c53052e89" },
                { "cs", "b26a686a882eca864d9558373be756577c0cb3cc81860d30803b1047a335a486ddfc042b545e338f56b3be78dd123b34fe72584b810845eff97ba477e919a12e" },
                { "cy", "d8025679f13343da131a6684905bccbfb0ec53175ed732aa05f04de83eb9995990f5956dac3ce892c67778989afcf010884e4abb19fde764ecf84e4ce540f687" },
                { "da", "bbcdd67ed1f90b91a78e164397a3b93bcfffd98939220c352f9bf5cd28ecd2774f16fb1ce54870560b427f97d430c783fbc9f39f7a8e545f2acbca8777363fe6" },
                { "de", "97538b63924fd94f618b9dd706a4cd08f1c2a8148516d335c1b1df5bb3bbb75412e72cb9db8cc47a17b64cceba5d8a4645304823bd873de313079390a37168de" },
                { "dsb", "c620526e8c00206511d74198b4c8a956bb3c6b4639c19b2e725573344c1e4120c7fd0c0a480d372d98eb006f058d501aa032de10eb022044854fba08015a2f95" },
                { "el", "9e013cd409c6554d182df0b71262ea9ea76ac6a4ffd1752648f51824d6b2939c75a565df44ac0bb3faabf8317170d6bf45eacd0422266ceda6f11fa6994f3c7c" },
                { "en-CA", "954bcfa11ae4e1aba3e0d9c8ba993dccea0c29e18ef6dc68d95df54a81378c05730107a4fcdb9fab384d1ba754682056c8bc17e06be11c13c900f0fe1d3b5703" },
                { "en-GB", "76bf877f75742771cff8ab262c25c635fc51185afabe5c4aeae5bac36aa5f550719bb2909828274d61612eac23b22f00a10c82761a0f5a6ef1cfe5c0063c8738" },
                { "en-US", "de8fc0c1b56b5a4069ef9f4ed81e6d7d5cbff0ed3a37503b4907206e5e75beb6f199837dcd6fe4ff1283afc12feb76f487630ceb59e40625b8d420dd31a33733" },
                { "eo", "e57efea2dc9f9f476d4b78b700ab402f513dc6348f744cca23054a942c46a5bcb2b56be44c3adcc365d6b5c0d6352d61aa10624c55f3d447c3a3ba75357c6abd" },
                { "es-AR", "eec5da183044311c188db3507381ba991721d4fc1b6e0a3f660ad361d632d03f202a68eb7582d520508ebe07fc492f0e4d41daac8d183a0b1e9b830ee87533a1" },
                { "es-CL", "bc3b2c65c8f2ebd0526a6acb5360b4f1616c25889a464ad9f2cf8659a65a05bfa1378c24d30691b97fefdc409dcecc7616b7cac1be88dacfd8154f2b67f37a19" },
                { "es-ES", "11cd73bc9e75196f1d780f45289f7cc320720f1e08fc05816f9d634c3708cf91cf6d724b47dadd6b526edfabe900a07e290de6c606ef8165753f4b5c07bdae84" },
                { "es-MX", "89f52c2ad18c031f7c80e22afea3b48f8be64720b7dc78a5fe9b7bc842bea626df5c9ae12909b8ca89bf0082f9e5b9d0b252f8c861d9e6ef0484c7a7a11c7093" },
                { "et", "4a29c23b75e876e5b9d3a379498f1f212e55e285868ad119ac4f73cce3f20cca39bdd94168e1e4fae9d53f3c5772b203c8d55bcb6a786e30e55bdab80f6d2f2d" },
                { "eu", "b1b9b98d583d21e78af25703159a540ddbcff3c320b859a75a14bd6461e4f4a536d1266cbc0d98fdd2966ffbcf36ce7f59c27f2d0d8484ff2e501895638d1a5e" },
                { "fa", "c0c933a7620e6f9c6869628ed4189964a5583fa3e7b0d4ef5f40e03af7e9f627824ac0381a0562352fae962d7cc73fe3c8874cc44f186c6ff76cad91779da8d0" },
                { "ff", "6994d53b79d39426f760ab4214ddf731174fcdcd3795bb67b8324b1de3a9027c23353814bd2ff46ab49df1c927b7a5635ed518144caa3db873b48bc8fc9e115c" },
                { "fi", "be64ad041dfafb5b94be3a2dc89bec6aebb0811b154c1da32be99c58c18aa088e287273538ec97dbb75b6e23e051cce1207249727d7b58760ce3e111833a7ce1" },
                { "fr", "53e9aa72c1a2b0b1c26159cb4cb29861835f679e44b8c8f3c5a5eb52109b31b7dfebb51b35d4deab3a26c871a928df6846ce53a2dce8904b6dfded2c46b9214c" },
                { "fur", "a3b1bc8edf62cc5d9793f8aa3e403bac8bc76b6fd9a6cb71a9269f17794f88d8b4319b421cb76dfa589c3380c46238308b189e344ef254c1721bb2f2c1e8c89b" },
                { "fy-NL", "a315cbe6483521ca9154b1d17d1a43383c2cdce15aecf7fbde40858af21780b498405e3ae4f11a5742afd4a18038bbbff34d81d0a88c05c9c17aa143f4364863" },
                { "ga-IE", "a3adc96eb89b6493227be61a9c628c672865b9f619a128a3a79e457fd9a2a5f991838a3ad3e5e68d8585d27a07b53e78506cbe0ebea1a22110aef737b921598c" },
                { "gd", "59bab8437cfcd48c8e7da604007cf8e68472764d778c799a87e4027a057ecd28bf63ebd79d3cf54fece0a7ba947ab341bfa5b854a144ff692111e897619a8bd2" },
                { "gl", "9ac23b868f66c4954b042f9db9aa15e0e5b5f28787e054b85e22dd8567ed52eee7fb46d7a71915a2f536c4235c4661bce7c94e81365f35cdf6eb55ad1703ba7c" },
                { "gn", "00e52fbbe46febe624a6d42257a44784cf60dfefd0850fcdbd11115c3ce34ea583b1c7912af225bac798a4c85b0fafaf018d5ca8c4a676ff97fe668a9a51f682" },
                { "gu-IN", "c731e2be5e4a300f165f2fc9ac238ce4899e04da50e5bef60b1257de47665855d19df1af32e80c550357c38fcc2980aeeae334c2a72b02ad6d00a908f6c7c605" },
                { "he", "b28eba110ad38382f6a82fbfe669dc8db67b8221b5f6cdf87d912c99d4b334e76fc3476c7eab3a5e86a93906a1121c7a391bf2b83d07a099b8fd9ac19cbc9c75" },
                { "hi-IN", "73d419a1395fd65fa5039022956af02d80731ca5aad5d26723dfc430d9b678c1f99dd776965f863bc4752931dac23a3ba03193d897294b391b956fff6c5f4c10" },
                { "hr", "ed951eb87ee0a1fccf57c5a459bbab1e46072fea9ffa3bee464fcae330bcbe0c0c51f398f0f453da940449061449cda692c3886f1019d6d604538f0c851a3bea" },
                { "hsb", "786dfb17bb1f9f29b0e18ebd446cb0d66c2686080e14776ea2ac4dabcd50bf83b246f24381acaaa51a7c2753d9770a95ddf524a2a700834db939fc914e911f19" },
                { "hu", "87e729dcac468516e473c4c211de6d6d706826793ceed8701dc31caee598e1bd69b72aeb0dc1696e8944a276f58dab78960b013df85a5a725ef4dea2f0e2be42" },
                { "hy-AM", "2552ab2026d8f1df52b41742248de44184ffd954755587191e3f1a9e53560d42201be39d1a2caea4ac585ba3860598ae08a43a16671889b55cf39b4478611087" },
                { "ia", "f2b37fb4350ad22c5c2536ca8468585d4249048f7cb5037741df5b8971c96c5a2085867b3518f903980a691e73514a28387a35171bc3fc0d816d55f8a7c5d7cc" },
                { "id", "649ffcea6510c2d37aece0417e824d4898560ca3bca7a15e50193ba1c0c51d72d69382f3dc87b19574c292f4e10f0909ce231b33deec37bb886ffb7cb6075731" },
                { "is", "fe30447806024018aac1a11a1657ffdd5c5a828ff73ed04e0bd2b991a6969498a7c1624199c54faa9898b2066217c2a72802199a690880751fe975b74e75a28e" },
                { "it", "53b48b094ac272347a562b905aa5b1d1389f522ac85f746634b0ce294905e9b6cc4062b106fd7c99575740e96617826f4ee4f0473eb056007414a5fffbbd499e" },
                { "ja", "dd5b84da946ee5bed03986a3b4c606e772fd6856c96d08cd542c1f6022b8efa8cbbe2534791001cb8d93716ed725675e6a5467584e4c4db74bbb9600abe21189" },
                { "ka", "9bb4e52f6d46c918ac3d1ee58ee965e804b8d4c8ac2edae3446bc0607a656fbaa92d6e6688fbc3a6daf85df21d06fa551ad791ba356da3a7e19976924130a8fe" },
                { "kab", "5ee8b00a83c12fc0010991332dffacca415c03f51d40a37ddc0c4b73e16045842773d5db60a306c2932c95630bafc11ce7a9b24d0722e21be8482760cf15fa74" },
                { "kk", "a252baa66af5059ed47c9c3eb67cd2a3ba0ac17d82eb61af9cc925f87f525df7427c5e754173c23d32fa2d5e2d4d00141eb0430be01ceff6b4f3d4426c9156f9" },
                { "km", "9f5926c6e940776225248014cbfa2552673b7099a2b2b949ca0045cad1fc41ceefc364b60288a4aaee4632fab02c9e8aa8ff1b593c953dab980f438b2afafcaf" },
                { "kn", "82e576dcbceb2d92a985fe3bdb26a4a7a6b02a6aa2ce6393c79264d4a899394b2280cfa2e29ef482cbccbb5b847f2095b08fb9a7516756abfe7452d6a67d8ba3" },
                { "ko", "b6a7399a7210f2c981f7e45f4ad79582d6d2d058ed03754aee9fc433af42a7c19c74f511120943d34a63d9a4fea52ad1a3636abe1932ae749597e9b32599e2a3" },
                { "lij", "7c15da0f6ba6f52f2cf29ad1c36b535b554e50dea497d93b25385eea016a0574f0efd50a94f4ff28380805a14bb39130baef889627a68dbf230b877a960c53c1" },
                { "lt", "c6b1ff66117d2f91bd1b1e0e672498753d8b6bdb71068cf0ded746b86caf33ade458e678ed714459840cffa3a243373d797b605152ac9c6a8aa6d91c3661ac71" },
                { "lv", "1d451956040c925ff6ef0a6732955d2c8dae2396f466a4b23886482cb63fdf87f17ab19f820d89f0379e804c0dc729c0e8e1e72addd5254a6b1913df49e21dfa" },
                { "mk", "bef31cbb1b8216ae7b73da5eb6d82c83c7b7c7bc7faa230cc8e7639adf0f0ea5f1a0263dddba82bfdfbb714a51ae9bb1a40fc25f0b1eb43461d2fa27fcedb8c5" },
                { "mr", "09e5e9aa4d9da4fd69b94e9f6c0c7b6454df248607c69009eb98eb26348f50ce1b800222f3e7269f70a4a0b9d8c9d629330c644d81cb53be5c4d76c3e2c2e09f" },
                { "ms", "0607e204a1ba182677b58b2b1bfb9b1a89b1231cf5dde9b2dee9fda23252b550310d71ebe18eff09f445e8e32f6618787073f9ad98beb9155487361d6986ae7f" },
                { "my", "087a5e50d123ff116303bd310118234eaca68890aa444b8f1e128c7871824201e0844243c1f6ac0710d39ea0175bfc652092bb39de1ea2760bc18e1a0e6205bb" },
                { "nb-NO", "8ec029387745d3bd9908ec11a1132e8c1dda5df8949a31a69f04a19cd09095764c6ea9d4009098ffeeb260a47b458d0e5965896f4a0bdfcac65a772d4a6fc121" },
                { "ne-NP", "3c6b8f4ac8b3a5d4ff63469465a69b1e7983d6fa451d2fa9831d58484050c3ce82b6a64c0a7ea535cae91e068fef4299f9654a727391369d78acea1e23ca1849" },
                { "nl", "fe19fe23ac72f88c92a1f9c34a618bfa4c65c70de05afb6406147020c4a9fda7423455c769a1ec14230607083bab161534fd6682656e33c0d6b60bb18bed64b1" },
                { "nn-NO", "c827b00a307c8d6c1d47e39385f07cabe4d409d653ffee6ed5e2bc3ec1275d224f41c1c6b7a3a9ae3c5baeb38dc2d0ee6a5c4384d9262d5f8700984bbf872e44" },
                { "oc", "4252655a0b456a81fd3727411bbcaa63b2c87ee41056fe8f446dc4973c335d28ba4d7d283eb2b45520bc6aa5ea76bdb43d8e3b172f2d476361571961ccc09105" },
                { "pa-IN", "faa052295002b1b9476353eec9f6393c0e5406c06e1cb35510e93b3b930f6be1352e4dd05c39be387143a837b8a7eb379cd6b659f60af7857698293c09cff516" },
                { "pl", "cbfb373c8a9e309ee4a7b3cc11cf155c70eac0918ef7e91b1071ae671d6fd9ca1a0ada73a6f020f5e6b6667ad7b6de14d4526b337fca8d99e3b9902490acfa0b" },
                { "pt-BR", "68648f3d2ca14b0af02d55f65ff206906d0ca58dfd97ef915338bf5f9a3e4de68f2fae6474b5d8eb62ef7c2e4f655819eaa54aea349cdd57f64c44151436792d" },
                { "pt-PT", "67d56dbca23dd0c6754f05e54a854f205d2c49e0b98081258216cd278e7cf73067793623830470b8041ec7784d837c804c3a22f307efb717543648629145ba78" },
                { "rm", "aa118d1d16409899e9ea78029353e9db5a148badc8bd04c1f03b480d3b68ac4505f0cb2a0cc0005da5a3e525d9ca71f6491e26197f1432e273e92bec44c19aed" },
                { "ro", "b9dd89e6d3c55155c3647e4c1b732a0e461622c9d32f14cebf3bc79e6a2e5edc032ea2b2d4922f21491737c439ae05f027d6d01f09b6fe9bc27794077007a8bc" },
                { "ru", "da0e15a7db2f5dad68ae76a435073dd91c584fbf811edfef0281a2f595ac90f557f41c9c359cdcbcde75a283a2b9a25d9208a35fa34adb401152ef95984932ee" },
                { "sat", "06591004f35731ec27f8fe16808cc622c826c79047939d423c12387d1f7dec39f1e45988953110da87d8718405ae3fd53aee989983d653940befc3b220e352e9" },
                { "sc", "a06a7b2454858e6e0ced3eb6cf036126a3c14076356fd53b7aa57f9ead71c3efe23ba47466aacc4ea5ba316d42f9ead5ce724c96ad4bea70e4aac097a62c40f9" },
                { "sco", "a0412a35c400badc44be98d6cb0676f3ad8a500083b61996cdf87baf6e0644c4fdc1229b6599a1c11b52cac5bdbd4ce63bf4c9992cb77a3bc9662586db1c294a" },
                { "si", "adce8c748cbf8af86ec5a27423c2c1ce2608b5cab89e1ea74446217b18a9e617787db125cd767e194ae591d22feb928d63ed9f7f1ddf62e2a08db3b8cd24b84b" },
                { "sk", "6ef41d2441c56616d5af98c6b56555958674ddd8719beeb60b5d32235b1218680d9ffbc5fef7b767c4d5c53d8f76d00542135f26c88d2215b1d0b1b8c2680bf1" },
                { "sl", "e8ff9e92be8d30e218aa7fef4418040f783e3bee5bc445a6f64df5dc0d32acec4165b6e41abe1017c83bf5490e3f3029ee94b48db070b0fa5afe41929515a07d" },
                { "son", "c73446f07a4611ac3950c7b50246822a858a4c9b7a1a7b0eca5cc36ba217e1290a03a61090e320f6fbb67215900846b964e28d59aab99f6dac43c86da4ab67ee" },
                { "sq", "2f9297e11b2e1afdb5e9c7ef98e354bb8de252fc856895aa61fda1feec3209a7b1f9e328edf1a00411f8fa992e3721ebac37379310fdfe29acfe33895a6e1611" },
                { "sr", "968e801e95c1887c4f935498b57815344ad4942a8907981377cfd41b74ff1e317c2f26a80de1f8ec13a006ab366ea6108107df43291d459aecbe060873619d43" },
                { "sv-SE", "c596d04469851a63291bf58cdcf6ed820ff545d7bcc1d3ed632fb6f41455f690ddf34fafa8af451b30f1d3a5d5e072687ff01a1d607750449dddfe3326526ef4" },
                { "szl", "fc8b02620fec06edae2fcb3231b33a9cf8e7c048e29bbea17c182dffcad0a291d8c6fcd5421b655fc77982eccdf35a2bacd5f8838ff2d792f327dc186fc1168b" },
                { "ta", "acb6d94761a3934a98bcbe52d57433e4242ef651e4bd97ad8797e3c7bca86e46ade5fadf9a0c9e55b9fe02911973bc18f94acae9957279d513867405ed2804c1" },
                { "te", "30fdb70a123db9e1cdc4fe625833e38d4268f54de7fce4c8196aaea838cc74aa85b58c60eeebc919c881adab76af2f3de3aed862f451bd6e7c84090182076b10" },
                { "tg", "0d824301da34086990f255fd811aa223387a640dbfbb4fbe352e54a82e0e3dc27e857b1e0f30a7a3a52f74d124db905074fc866bd5047ae051fddff4c45388c2" },
                { "th", "1a1ee2f7cddcfea5d80ced64dfacc4f42e03a4236abbfc38c416627c56eca8e0afa56ea8d1e7d507ce6d8222ca9a5d7265a5fbb4218217d95efa76e15549b0bc" },
                { "tl", "fde630c9b9bbccf99dc070eeab882ddd970ea171faaddf1834d62941c4f245a13f51f1b6dfea1a6ac7f1c0f84c05ea1a42b014e3627e6f94773bdb3b65e18ca4" },
                { "tr", "acbd6e24731a9189971d96044018f554509f3c79dcfd1b9d9b17859ff5bebf7d6d457bb5bb13beed6eedc9b13d7336dee0e52fda163d06515010a3cd02063444" },
                { "trs", "6a8277ed80344912cb6eb8718365b7b81974d4df716dd33c45503ac9e5eee402dc3e1192f8362d2237a0149045f62ac21e172f407dd8272ce81eaa3420e7696b" },
                { "uk", "3c4761a9bccaae40d008de4cd53d8764829e5104d77bc2529325df0c90bd8f5adabfef145d9589697d84a4e0244f893ba85d51bebf6de66cc1e7a10fd0b818bc" },
                { "ur", "affdfc3ae22225ef6d8346d3e491fb0a01e0dd27d4d75521afaa268c6653c5ce9bd79a18d0d89ea333d21a15b31d98252a8c153dbeb3fbf2f134fae898adf847" },
                { "uz", "9758853e5b0ed603dc9956dd5f02d45dbbe2b44891815df1136ef38baaee14af35a71f933cccdf369c356a5435a9c4d7b8a1e01ed2b9c139fa0f3a6b65961994" },
                { "vi", "43511fb2f2953c81be53137b90c8ca2a1410b8510416a389ad469392b7a57f2329f9f4ec52eefbe29a139dda5db9e199e72f266cb9ce8a362ade094c4176dc7c" },
                { "xh", "26ad71562023302f30e6e3cd8ea46801067d96c673bbbb2663ea0c83dcad5a9c49ccc1e6c270ffba53b016cb22ddb763fe0b515f777ba0c50a1dfe84c03a9607" },
                { "zh-CN", "03e2a12ede3cb9615fda32d76af6e98e4c3070b2820bf4bef231ce41aca05e6a86eec8e5475ce34c9102995a12e1a89c82efabbff8c162dfd7b50cb9a9e5d424" },
                { "zh-TW", "29966a596e0ab9756b1491bd9dfd07e7b5f6e74ad21a42efd1268abb8e4a88918d8dcebe8f021110c75366c99dca1426abc10c524647dacb89c47ce02f153552" }
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
            const string knownVersion = "119.0.1";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
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
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
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
                client = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
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
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
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
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
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
            logger.Info("Searcing for newer version of Firefox...");
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
                // failure occurred
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
        /// language code for the Firefox ESR version
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
