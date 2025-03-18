﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024, 2025  Dirk Stolle

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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


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
            if (!d32.TryGetValue(languageCode, out checksum32Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.TryGetValue(languageCode, out checksum64Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/136.0.2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "2836b1a04ef446aa3e4e2153c134d029a005e870661647f35ae97c85a30bd6fe94f8ce135f858ae6373dbf632744843d894d88881493b3eaf4497b650150fd98" },
                { "af", "12533eccdae12426f8c3b9839c2b102104e289a43abef0c3b22a1bade3d180a6ec6a5575d9c6ba11a0de17530e27670ea0f036dc4d41ba7fb95a01de838f404c" },
                { "an", "daf104655cde0feb312371774cd33624751c2cac3ca3ed49c22bba9a4a525b4fc5db796a050e2770db81cec1dfa07162b71a2b33b351ee30c4bb2cbb5aba4209" },
                { "ar", "0f52b9c64b99248bd62a1b3b2bfc47980a10c697d029ca91e8b693b5af4ca3eb4c7561d81da5ce086ac9796aaf0ac518606fad45082e1c5c8b3ade8b6fe26359" },
                { "ast", "18a3836c0c005b18324e81ecb1b22a6423ef7f92260707409dd69361d980ded1217c0b3c3dc68237eba43e83633f71c6d023319bbff33d461552e27cfc15b9fa" },
                { "az", "323d61b39ebc07d6d0d3ea51a3c35a964fd7553dcaea5ca909a52e496dc7d8402e59a72ba16c9894c8ec8f9d06cebbaad90f0e18aa775ffd9cf2726d60535588" },
                { "be", "516cfa263ecf81f58ea6c159be64a5a528dbb1014867764ad2aceef75070772e270383a913592f3f412762b7e2718aaffea1d6c79ddc3444f6a0e14434baef0a" },
                { "bg", "da1a836600717060d7a23c6b26b053799837f296d68edb50b7c287b95d5f50094b6d765f37bc577886f19b76b6120c3b70d5678401d910eb84711797e5bf69ca" },
                { "bn", "f7e052c71fe82816097f12e03b10e6604374738e45c0ced33a9b000079c0e3579fcd4c3a9aac30859f30ecb9a80adf8ba859ae3852ba2d90d8a567ed34d4406e" },
                { "br", "f7740a11083f02b270386474aae74cb3c078ff89176c2bf77bc968c92276c276e30c4e48e0b45de36a3c552e131e6042780b8dcb61e051d5c5e245407d772d0e" },
                { "bs", "04fb1759aeb6a2c1eedfbffb671646166e9d870ccea90b3b2982f5719a1bc46e591629437ae25434dd91e46ac6c79df6ce9a9eb735d9866f6f3b3ae4213ae454" },
                { "ca", "6c8c487a8736abb5e14baee802794096d917eb5e2de44ff854858ead902eb1e132e1251b9a609ea5a2ca69a79f6e24476d7178c01accf49b1e5ee60aa2ff2052" },
                { "cak", "4c597aa266ed0be82e7fe17b02065fa93b244f2280c8a652aa9c03e1851cc58ddd60614a7f95e437ea38cb704b756d45d2e528dc7630749abaeefa800ddf6c47" },
                { "cs", "f5773649157f3c7547a92e159aa1896903cb88eac6c9a519472ecd32baac04702e34583804126d28d64340843b8fe8308b7b2c2e3ee12188e701d6084276c51b" },
                { "cy", "bc2442332b7c617504a3e3cfeb1b223ddc5db367f8b7ffd2d65e7b3a8893ac3fcc9a4e68e76f7cca987a0f050b3ca56dd7ae4c7593ea170efcc5511647519dae" },
                { "da", "e87c34aff7bc3d12e0583bb6588fb4633865afdc9b0ec080e6f6584da24a9c539a4a9269980b45a88b7a391a1c458527e355c912bc39979f554184c8292ff3a4" },
                { "de", "7834be0195c5ae1c3c177bc6970f540176ddfb59d8bb0e0c807cf937c1c100a48150823fb83087f804f1adba3839feabbf27ee84c0274a7b472c8d6bf43143fb" },
                { "dsb", "c7a9de487a446f303dee1794b390118c8df6cc34c945be4b6fd0f4db60f4addeaf00385ec3f229f9835d9da6cf7a391b43832ad21d839bbabbee9ad86985fe8e" },
                { "el", "ce9b47ded3b01406451d02ddb330554077da90e4cba2599632ae855b520729cf462a01c1d92cb54412bac047e9874f074875fa8a9e964728841f47d54a9a4083" },
                { "en-CA", "ddd22e8667e0c53554e275fff438a8ba954f2671a6ed6380946b30d318ee5655f220d5603d1422d9887ad802210eb1ddb36e6645edbb49cf65dd902502e15ea0" },
                { "en-GB", "392d7a37c75fa58a73990433062668c52caa936fd61a79c06fbeb1b260b64fe0fa393afc7de956b13ba97a2995ab2e3ed9d95baba4f307647f34a8a5984dc54b" },
                { "en-US", "bc01461666e6c9def82e77b4e2ae6df1c911078cc44f93edcdcf621b29dc494c6ed200e1d555018f2d415c1df0744e1dab811a7d5cc1f47e36b6333fcc9b1961" },
                { "eo", "d1b2b38477525e8586b8586e0e1f8935c190df20124821d69cf93abc5ca92e49abe9c4e4d374a8fd100d06ec2f5d45e893904f30af7614a8ba17f360ac5e930d" },
                { "es-AR", "8dc759208e1f896c398c1f84e8105152bae67ab29fdd85c63d276c1374c1cfeef7a6c9c47a7804aea4932f32adad74ce5d07b8135b0cdfffd1517319a9bf7221" },
                { "es-CL", "40c22783d15fc43e0a56bbbeeca9543302bed41784d8cdff721eef290523d7ca0afaad8156d5904d78d0c2d94a4788f1dc83fe6dd0e9bfcae33c2f57c07f7e83" },
                { "es-ES", "a5806d5a1b486931b670560a8d6bcaa016e03458926ca543d7cb4e7ae387548435dedc766be15092c40a35519545b1d664462af290e0c99fe3e934574c5251d5" },
                { "es-MX", "bfc12ecf408f3bd8706c5cc45524860e2d676bb0e21587c3b0995caa8004c93e4bcde288cbb44ac719be1375cc48550cb8e51441d26eed846ba3f8eabf43c72f" },
                { "et", "41b4ec9ee6b5b8b1437e28548dd0798cd8e82f8819b716b3a745eb7e9fae9596a2178eff6966994090722cd9d2d22c0cffac8c6e41f6b9bf8b5bcb464a777c20" },
                { "eu", "a2a34432a0c077d612a0e8b843bd083c837fa7f69177220e40e42cd52f477568a7e029ff392d8940d1467d606ef5df83d2cc20ad00570013832e0902de37f025" },
                { "fa", "233a400f0bded38f2aa9c97885c3f4e41059077261a0efaaecc689e0d8ae0d25d85e1df9223ca66ae741b274ba7cc6ae69b8150907102a1029debdf49627817d" },
                { "ff", "8b7a5038c4f98438b681b7a1a6d9164b0928a72c8b08931fbc6233f9b725c0c98c63023e075660ac8b5da54a16933ee2fd7ad5b5bbce668f0c20cad1d34062bb" },
                { "fi", "05edd51e0c33faafd5604f4291a1bfeab7769dd3062cc1b19e4698acd90f0de0638d92a202e384ae3d58f5a8681489cf249d49e1aa6bf985118e3c0cc1c29a8f" },
                { "fr", "2dba77e115b11ed9ea496fd6df91b0c3a2ae86af8c2b9abfd05182542e8ac8f3dc46e08255568e3cee04236b28ca1df69ac44bb70194fa4b75394950a858a427" },
                { "fur", "2d75613f406bea5953db12aff3e62e2770b85dd08c7552ed9b9365f51ee7461e4bc15dba4b7ff4af88acbb153a3a0757df3c2bc271861cf8c385e4bd3b837f54" },
                { "fy-NL", "5dfcba592693ce786f94f0717a28652df3fa7f077b7fc7d30ed32ef31b372e4d029054d45e97151f74079ee2022c1353847a943f4450b3fc5db5b687b96af481" },
                { "ga-IE", "3cdd2223dedb2c8b2ae2e0180921d050faefac25a65f6e396e997ea51b7c4b226fc58b0b76998cb1f82901a42418d23737881cc7d97633316e47b5efa300b2bc" },
                { "gd", "f0e8c4d6c5d13058171134be42bf72e768a9b997e2590f4b94d191fc035046efb3620b2d8b46c4e4c58b38e4eef49ddc32b9813489a7b64da2c7604702b2b319" },
                { "gl", "7c1cdff8e0a46272a339458b1cf51cf46796044988a2e6ea04f7046da66a07435be73e383cb93b1a09bb2b92b0f3e803616b2b73965bc5370ec93e763c23e93a" },
                { "gn", "05988462a94e704b2f05ccce918fbf8bf88c1f320948b7947d83b4ce222a5e812e0c19ac22308fc25f9624775b6ffd83cad7d1490c434f7057e89a578ee7e0a5" },
                { "gu-IN", "9b65a808f1b60c6e572dafd59f1815d274454353e579c6e9fff4ecb6e5d669d447515c4598ba61618c21638cb1a82191b2ff84a6583af14f7683d4ba96dd0603" },
                { "he", "828889809cb094b26abee96d403b88ee87083cefa95833d7c670f3e2ed8d6535ceef80a58c57207566a4603c13fe00e325beabf4a8c99ecd251c834dbd035613" },
                { "hi-IN", "c4b39d6a4c8a9c8d1da85e1fe799b7b1d3c39fe3e4e35fd1353c85f12e08db8b92a2b887eef4ab9451d45bf388b4c6febf33cec2baa5a8c34f85c5fb89a5a3fa" },
                { "hr", "1594aacd19538a1611ca8f5b552bb986ac56c1caf2ccef273d20e7e33472ed5c310a0fd47f39e7908130b958b6cdfcbae4d99a5e2e5d32af6d3ab1e224a4f4e8" },
                { "hsb", "3116122c4712b969dc8066237c2e17bec9b38875b7d71c79cf4916f549c3f0f3c920a7ee5cdc7cb63ce71d87b4b9e69e9a9fa62ae97689fd5972396f0c9af679" },
                { "hu", "5f5d4a3a7a1c04cf83e46b9dbf41f8884d601281d155b50191dfb482781ce4d881840d3b35c5e901378b1695dd132f79427fa8a677748c4754afb5aecf8a9f61" },
                { "hy-AM", "c8397cdb1b20ff70c60af06809c78978fbdb306818a2444df75b60c8c1865d5f996a5d69165eecb3162203e5e639ef06792fa63b809f3dee8fa24243eadab606" },
                { "ia", "2802d88da5a90f2744793b93eb35a140a1cad3cf6fea492ef2c39e65da16db069a399bcd1d937558c4ae0f581919122f0fd17ea2f268b9c6861f572214595695" },
                { "id", "19e34a6a2d7e083a2e44748d96c3084bbbaf836bb3b0b45b28ce99d81167d0c9e6b31254d2a43defd363a9074b44c7319b1ee4326c1b8cf78bf2317a6664e081" },
                { "is", "598847c651995741028edddc94ab8cf43ecce174f3e502ffe5eb5fe75c369af7aa211b93f966e942d50a38c8a68cafe1697091e30421d9c313d082a212d6237b" },
                { "it", "ae7e0b9f4f33cf20a68e43cf1bd1e383d77e7bc3d1424b0eaf08ffdeac8c7e2b387d07c2a16a56e9fc6b51278afe9b252c679b64e120536b1c245532c1ddde2b" },
                { "ja", "51c6674cca5f4d74f027029525f47bbf8ced824a85cec4e012da3aaae31c4edf8e4c4df3d0d6405fa481db07d7d683906e2acaa2d41bb7fd4898ae2b6cddf560" },
                { "ka", "83a79c55a4ef05d24df7f9e56c2df560c17a5ed457dfef8130ef83ba1227e76f0acfbf533346d80533ea7473acc42aebccc9e3f893d01a36ccfe51047ffb7975" },
                { "kab", "322500d03bd7b13ff1e9da547f05100c3cfc15a97e50737fae4e1a735286966299023b8fbe2c905ea1f483b578d83c8342214698df3c86567779d1b0451c64e4" },
                { "kk", "d7f777c57d59569ae44659569549c0093e7f30832e2a0df61040ae9559a3195cc553446581e74f65ca9cdc5eb6ba6b69d13b05e2f3892702df0f70d40b799e8f" },
                { "km", "653588e40f216faaa0d7271b91f2117164b3c37e2db53d27b0023dfe26b6168ed25fad064c2c70c7bdab4cefbc64897549cee45a980fdcd10e63d1ad7e10eac2" },
                { "kn", "66cbba7991b3cd132b551b395f5ccd067136632d4df867e37a42165dc7ccd880291b513a7f211e658a9438990ad796b94a43e8a119c5b86834bfb9a6b14f972f" },
                { "ko", "8c1bd117f554e77c14bf547c4f2a021205e6108ef40556c401407bcf7fe56d6208c5271364c4f14cd750c9520aa28c6c41fc952def0ff9d46a8c85a3d2eadd69" },
                { "lij", "81f59a69dca1473d5c74103ca991a7e660334f31f2a26190785b6a3b69ac361a8367cebb3e37c8a46b0ead4ffeebfdebfa93a6c199a561e63ef39d3c7504b8ac" },
                { "lt", "01181f32052b526cb25f4dba8dc1d6c1acc157f8c6f273501ba3bd367c527e296466212238ec9d8c398b9be89700f15830444d9d8590110be028bc9e1cf2f889" },
                { "lv", "269e0fbfb5e5c87849bca3b8fefd85a68dd09a05105842898bbbcf088cc717b57d6f04076a584172ba14dd20d87fa9645ada84e6506cac8cc4a7c1f869b30ba1" },
                { "mk", "b00c8433f9dfa6b24158200287d1b619dcb965ba78e3db6a6da44fb4c873f7ac0f36a4fe892d06fcbb29b4e63855490a86aeb4d7edd0d46611c9942a079b84ac" },
                { "mr", "a7a7cd1d60147e197d03ad1591db90f0497c950ef05df6c4757993a8a86445394ccbbc256237192f072fc9dea2fc22dd676ac70bcaae8141ecc901d3c2e09c89" },
                { "ms", "d828a0d834a3f82e1d3f9afed91f9ea541368f00a6e1a601dd9c98c8c757cca3a6c3aa3a2714dcc3db3b443051cd80324c3722f6dc3fc10b046ff67bd87edb9b" },
                { "my", "a00c1088af590480306254230661015a90d254b46e71d42a4bd1e80190310a1f3c96cec63435f67d10695677ab9153db744e91ffa47e228f0793361b6466a155" },
                { "nb-NO", "c7ca8fcebf920d134a499a7d55e714517057dc8a5899550e890106d16dbf80ca354ebef6d1fcb3101a89189ee39ae3e6f7a309cafa4a471e3c5f04d2c3e1c6d3" },
                { "ne-NP", "4b16854e84786e8689a3ec2761f55d7c1add2cfb0c7b3821db011c4f027d572aed6a49a8688fd7a5838b3c023df46cf0744ba09d903002a88246a4b7e1f58b84" },
                { "nl", "a37bde000fa7718495f75e5385bf72482ce7c6da08e84c5ec959deb11dc80e855bb8818ae0bafec12d4f39e9c1377d8fcd83155eadf90d644bcf141e5d729585" },
                { "nn-NO", "912a76796b7bcf2964c117a232062c3c3dd82a62d9af206a4732cdc221f0bbf2b48cd640fc8db888f3c96fc320e5cb1add4c89b903b4ee986901bca00c513340" },
                { "oc", "068be4b08fe8265ead8768b1de4f8f311e9d14bf05f731834ee6636fe20ec6b6d428629a548066c6976af5b90d6acb1ed351445c66bc62bfbcdedfa7d694a3de" },
                { "pa-IN", "d5363914a24ffe0f413af7261fa3fc97f228b19f89fd4c446a13dde3f262b96c65969e759d128ad2307a7a23afa60a16d2b878c21a3ee310024112788c934022" },
                { "pl", "12ae49ea11aff12df6b452b3cef6d3dcf3c3d1d3f4d1a4bb8cc0768102e07e625d54829fbcbf9a3ce8e0f42fa541041974510207ed60a32f9056e7ed9745c85c" },
                { "pt-BR", "8b9b237f7a9f8d0bb0b22bf5a24baa249a002a61abdc5dfdfe4fbffc77d43afb3097c2b5a7c77677c879a4faf80da380900b8d1c7b5958b570f32a17fb97921f" },
                { "pt-PT", "aa9db34a9fbb76cc8f93cfd91dd03c937de336672dd41c84f89f194db78f9f652f7264532a8d7f9dfa1eacb01454db4ce1ffad35d5f3248094cfb4ff89fc2ff6" },
                { "rm", "246decd49855037f0c79431c9b8c2458c41264ae966f989a92ca79737a50253e7e446a42a189bd9d396cacf8f2334b30dc770070dbd2353878c4ef533ecf96b5" },
                { "ro", "617a4607fa9a8da59c14e70538e5a77ca54b1029f9b04ea973fa205259176ddb74e841662d0795170483e0e3130c914bafbcfcf352b224367ebc7a11c540cc51" },
                { "ru", "284422cb0b8cf0b32d0be55c82782cefc93b0f9eac66d5183902a7c96efd9872241cc4813a1305d04b49e930b0fa3a9087874fdd6fc98740df1445f65327554e" },
                { "sat", "60b3d7367e81e854b4119d7a51d7e59e885849a0e6adb6362661fe1bb911fe01e217e4c6809243c340ca974f204e761a78ed8c7b566cd516fe7c3e337fe3ab55" },
                { "sc", "7c6f51736fb33b2b19563527a101185df4759cff4070740ba3a2bde98d5da74016e16d70dd3018359e6dd0843ced413e2880092755f1dd4743a11376542abf75" },
                { "sco", "c9a4a394547e2bae09b1a531e8e286dc734bbe5006b80f132b11cdeefda2859ff11cfd74aec453a04509e20bddb803a738c34ddf442b139c67b8944d0c19e158" },
                { "si", "0a7badf0f921a63d87aa7aee5991fb1db8da83ce5ddbecc3c259bbd8e4b0006266defce2dc2bb838b91204ddbd0730b2d593dddde5c9ea460300851d77fd69c5" },
                { "sk", "b775f967706f032288888b939ad9680b90c310dc3aed41ba5ca619b57ca194eb712ee4f95fb3119859d6dd53a58e051d2ae3741166415e706be165ec737d65f9" },
                { "skr", "2cc3132e2682f88531eeddec2984c2cf0542ed8a13431a3ac9d86d080f95f04463ffd4181ecc163d052c79f6ae167716ec1e25e437525648bc846aebfa5777c8" },
                { "sl", "2569ed57fb5f09e7b34bf5221cd3070f08806e69592fa49e114ed1bc97348a9cd58385804e130d94a0cfe1d15c37b439fb1a1667f3ec4073b86b964b61c9f017" },
                { "son", "769f0f4bd6e6ccb7d91977273d3b49e8a4845647dc93fdc9676427f818af2b2e73e0410b9b6398457d1669ef320311b2af973153e9f28f46891b146f8873a0fc" },
                { "sq", "3abce480f322c7f95e77ca43d25dd40c6d648a4731894735b72284d5344b0d1d849a9fe58c80bece19e7755ba3e550fd40f79a946ab42a24af6ba0bda766d256" },
                { "sr", "7771c8251f6c50f306ccbd9c517f73abd2a383eac8135aaecf2fc03a7cc7e2e750acc6979e6bc7f36bdd301d060fdee8abdc7e3aaa9be2c62e3b10962b72b71c" },
                { "sv-SE", "8cc8bb625158b5b0211b1a2d307957d202273db5a268ea8fe401df6d84d434561c82f611a285702ec3137aab596ea608d9745b47a6bbc1f8804509a5b0bf68a3" },
                { "szl", "db34353a7e5f6fc06b57b862b15ccef18d7c27f892c791543d88bb9a4efcf734d74c51367fd07c04e8f02ef35f8e71e679eeb1a3ea24d42f7139c969f68e327a" },
                { "ta", "7dfd6dcdc73bd130c9692fb4ab9dabaa6ed02cd176e7f314f8ac6cd18652a654156d28f23b4399cd8f6fd5b9d4c701d29d478e4d1cf6aa7e4fa39adb9d8b4893" },
                { "te", "bb9dda45812bd18d041e3c46f1f699c6806bcd682c10aabfa0533fbc3f623942fd9400cf9abda566da5c01cc01cff17a878cf3e33fff155ad5d65f763fb9378a" },
                { "tg", "b60011d1cf24b1d88833c5b242b01d41d4cb8893a2d8c51897dc03e9c103315cb28c9dd23ea06719e6e57e2d370ed2657245ccb41ebb0320b6eb0d28eb06924d" },
                { "th", "8890643cf85294eaca08a59adc971285fd6d8e6806b7b643898509c97225db43beed413fd4de84dcbf867d817c96d9173a8d2c602fae72a342b3c98c5bff8342" },
                { "tl", "fd29891fcfa47ef1108f14277157720476b1fe5494f237bb7525d6fdd9118ad283ebf77364544c6972a9d83cd1b8c06ef630dd79b88241a1d7748a9eb2c3125c" },
                { "tr", "233742477e7855bb7d1744271b659b62f7ea7205fe8ee1010dd00be1a0bd399f5d6a3dae24a75d40ebd9bb2f0d23b5e14dbb66314b8e31ba92bf7b0dbae8b9c1" },
                { "trs", "8cecbe14fd9746758cab4831109a441b8d0233a35414d5129654119a3ee1a8a010f43161bf79e723d360f69797916cc614006fd8e70482ae97185baf7eb1901f" },
                { "uk", "1d4d8bc3a3cc0c27523bee3b86e6cc6ea06aad6da62a93557cabc816d33bd3a2a12c80f0a085b10e20e3d25a09df28cd6af4ff767e2585be630934ec32713e7d" },
                { "ur", "d4ab29759d60cf60dc5046f778798349934f1bd7783db41a0b4a8b1985cc6fed290eb04da7d9126039d0fe3984d7a556673f97d1c36e7e6d500d1e3ddab48936" },
                { "uz", "4778e210672e2b26eaa6f6857423963108c532b3a59b39e45efc1ed66ac7a92316b15eb378f2c85381b1aaa35cf60a1c6956a8a40d86caa1daba52358efa36a8" },
                { "vi", "1d663e262dcf37ca5865eba616285c07ca39e9f03b338d2b59b78473f292da749c65d057b60b223ca2bafb4a4ea990027766155c97eea84cf4ab5309305abfe5" },
                { "xh", "a4f2f2e6d6381b1a6796bb87467589acf9128b56227f21bd5252912941e664ff73f847c338185aed273f595e8e4c9594978326e0680ede803c1e61990d594123" },
                { "zh-CN", "396ca579c4e3545deedc573a13b62c694759317bc80fc39b31a0ada20b6d310e3e774389ace368a5ce7f8798ded09ac3210ac9d30d97b1bbb9ea4bb601a5f03a" },
                { "zh-TW", "a8eb8772b2432f5a5c6861516691a2ed02a52b9e101143a7506d13b244cff79a92f7700a693b11dc2cd8952c086a190efc2c3e67f4c7f2b5b41645a11c4b17bc" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/136.0.2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "2c8e339a04ec741e92927c57d43cb4ddf5d6766d5cf9816a610d5b5510c0e1b97218e656380664c506c31fa1e1569142923fc7630f5a3d962ded6e249559eaef" },
                { "af", "99a250d9bd9b6313bfa544252971a35b1313c66248393212008267a1302390818d8a9ff5a42c8a37a4d3385d017b021db1688f6997f0f9918087facf106f841a" },
                { "an", "7a7625455da930cf70f1c3c76d6d119556a16520343889a6492fefc9af227fae95575e256fc9ff5acd719bf2677a651b5655011fa272adb26d690c921aea2cff" },
                { "ar", "a82be158adfff6096037035f11adce730470f25bde86da74b81d0f56ae62784b74f65ba2fa20f242b5416d8c90ad1c2c9ad823762959669af29b711ca961d10e" },
                { "ast", "16969f289b9baa6ed39475107f79f85122a65f02acd4860d5e8aa24be96adcbda245581d8e388ea5f52ae2d6c39cc654e080efe30e7773486361b8e296441bb2" },
                { "az", "08c50ebcb0fb0e47527b7cfd06aeb89e71ce0e67427679502c0f4cff6ddf58464675389cd040053142bdfe21309e11f8ccdd2d8d0a99437d01cc44249c64914c" },
                { "be", "144538b275a0f5266128360ff5e0bf282d7a92bf290096e154b861b02ba1f99f790b5a78850d96596a50e2c5374a7157fa21d4e1b7716ef8d2b6695f3dd220dd" },
                { "bg", "7501248273cd7313b6ad990cf7eb5befff1b843caec17a8af61ed210cb9aac00dc8b16dff0e35310246a813ae00c0e4edd067876aedf74f85a9084e1dd53b6a5" },
                { "bn", "d2097682203aeaf00d67497d72c4e93089726959cc6e9fec75d8e93d16c530a21c091ec8d0849e5e3723326df0daa5fe0b18c28d32adc3d08c44bed3f58cad2f" },
                { "br", "cec0a91df56607b513ca2e5c23cd3a967e96c1610f5e886448792d696492847ae2e89edda341d7033501991e1dbbca25a6fbdbde7972718113c18d366dff2f11" },
                { "bs", "dba0977f68e905dca2578a82ab10e3891959a621bd6353e0bdaf5045055575e4296855a40c57c8af526550b276eb22b5d007d7c100ee9011894577c1599d3b81" },
                { "ca", "dc3738e342cc8943d5295f146b25a59c876fb7da49accb52afae0929acae46cbb62281ad1db6cb76e078e6428136abed28e41598a0f1eeaaef5679a33b328fc4" },
                { "cak", "a688f3838f7b4ca510bfd3e7eecfdd591ac7ccbefa19a07f3c458b2e46d0e2f34f6a92e6337ad96e69a1138734e6b40b15d5cec9f81ec90cc3acbb45a111f665" },
                { "cs", "b8ceed35951cc5046fc0995eeff102f32e614c73f6923b6f7601e21b43c107597899946426b9de05f3bc835203575cecad0ac1eaf71983a2eb42745b4737ddf3" },
                { "cy", "2d3d896be0b6979bc8eb1cf2baf14f0fd36753439e28cc978d2369a14b641014a8158f63629ba501255229d82dd2fde001895711e9af9cbd0cbac86297d12c32" },
                { "da", "9ce133a07210200f4a1004976ce99c6b408b3987936b09d3e2b3c2d8487a5dc159a553f214b5e3e52ee30e84359d8eba944e7910b029514b753b05273c74bd6f" },
                { "de", "4ffc53dcc7d89233712d41390ca8051420d260376b125e932e7864774f7fdb0f3f526f5ea9cab5e66eb1ff87e4a56f1ee9f75e52f4750b9f7b5ba87315595d34" },
                { "dsb", "3d07dc176ed19d5188e765d4d4a3e73c29210c554875b1ad9b2f885487b7759f3112b1af2b19fb1e0b515e61ee2b1100935918568901bb2d7817079cbfc537ef" },
                { "el", "19d04c76240f5b0f460591771e2e76d7586f73d13b23fb579b8a4925ee4da3433b630e00e88c80e97d53b0116fde257eccc68992df55843a575b1fe612422b42" },
                { "en-CA", "18e8439c64e63e7c072a56a9eccfac22a54acf3d9425f997ac2ae1d00cff956f17f6a6c06714d5fed162ace1a3b4a7967db750772175050e913ff36696845f76" },
                { "en-GB", "06e6b61aef15fbb4960e9f2701ecf05bca86e2f2c528aadc4ae8e830bd15aadd7946f08cff148b7d800cd1cc3bbe066e81ada85d3c9b00dd916a1c97cd263918" },
                { "en-US", "36a60c039d7dad2df9aed878c349130cb2a867f0db3437d5087d396a7cdbc0eaed0d7d0862423512813bebe99ec99b2a03934be1d419315e4512f3d1a1f1c9d5" },
                { "eo", "738d23989059ee3da2860efe902b964b88a2d555dc555c42470d12f7224fb12e5dbc6759b9d769c1528c51473cc67ff3257de45e8cc9340b75d5a8fca83d1c5e" },
                { "es-AR", "ed83231cff1053f907a00c48db052d724418aba8a21cd4099bcdcfec0135e36f835d4863e2196272f36a4829a15ca377aa32bc57c4c620917901b136fd02f3f5" },
                { "es-CL", "5860c11e5ace983b98e3bfd2a2b87984f6bf08da199f9f649d830afc133fe1a09749f8bd2eb7a8ace628d1c1e0b9ad104b6e90bcd0de1187d971de27fff6e456" },
                { "es-ES", "88d0b65589eb490849a7d51fbf7a273c2a9534784fd8aebbc0fef020ee0d878af991aef68f63f1f71ef1b9c46d005d8c6441c111b4e2e2a06bc9c7d9bd17a151" },
                { "es-MX", "1429bfae90de7ae4dac68d7e869b9bbe562e32ec046b7e68dfde008f248b2ce51f3716893a02a4b441e8553a0b1b8114fe0069704e1972bdefdede2a2f5acac0" },
                { "et", "a618a6941d6485b79b86a4b7d1528bcd96e782f7b0038bb410ab9bbe20c558f4c979110b4da7b3458090f84e443b51d89bc25ed1de1db10c9b909ae45766dd11" },
                { "eu", "6e3c876c8dd384185ed969e63d55d61e0f5a0aad219a1fa1f2de0a4e3ec3d64ea5b4ed06a73ef8542d26b513b81af4f02a5f76c09d7224c4cbe40b0456f5ba32" },
                { "fa", "21a1188ecf8c69c726be8ff04b29825c88b3e3f0288bfdc16649cbe1093454557dd2815a3cb82acbb3325ddb49d96e7db2991c7bc5ea5333112d292a3e8ce20e" },
                { "ff", "c20cfc88e5e7ba256203ae140d7cede7338721ef7af1d2ed57ffcf8de43abee0edc3c95140f3ccb76c216422ca965cee93b3ca07c65dfbbd9e37e684c71b76b1" },
                { "fi", "42d74b4ba58820c7821fb94f0f72798309f083bf8822baf55df807eff41f3656fee8366663a7f6da82c2f0f778ab0c2b264322cfef2a46f294c52678e3e9ae45" },
                { "fr", "ca28a8f36520c566e6fac6e85ff8ba81433f5b46cb850f255d9c0d91abffac3e562dde790cb36a3336af1a61d2cbd58e8af54874b41ae8c57d52e70373a4ebca" },
                { "fur", "ae97605ed614fd744b7dc0152d09abec19c8651c00751ddab9ee32bcab491fa66b96cf8fdafafc476516b3d7a5dd92236b795282d2649d377d0add82cd844759" },
                { "fy-NL", "290ff7ff2f6829a36366ae37d4fc7123f22246abae41dc5d73238c137987e9b936a512e0f539263d9571e004b42d4546da18bf5a48534fb65bc9eeba5acd4caa" },
                { "ga-IE", "ba0d46e7ccb424d7998e8112f961199f67d758307da6f093e608fb9b72a320b78451f523174b7e70def556691486ab4383e7ef0d933b8585052d0b8c57a23cf4" },
                { "gd", "d7dd60b69361457278d87e50945a37fcf6ed8a23352fbf116108a4068fdac164c23d29798ad986a1bbe98913738d6b6f391fc63f97b40bde00c84307859773f2" },
                { "gl", "80ca2d8a94e545f142284394aef2098639492e35a75ecde22582d1bfbc9d7d85d5cab990fc40f4571b3b7d814006e0e5cf1be92966e78a73b598844a0f853bee" },
                { "gn", "02691187e55549cafa37c6406bea9037a29b688f0ad5dd56a1bffabbd832f672cf36b3b52ea08d6b7d8be6dbe057de5d8e353b71d7d4183a32ba9f6e47db2fc3" },
                { "gu-IN", "326ceedb49b2d24a9d1864f45ecbadb303900a2c42b99a433cd47508e300873c2e26b8cba02e129eaaf99543e0cef709de1cfcc4a4ab2b189c5042e8f7d12d2b" },
                { "he", "9b468e16d4eaa42b9251484d6fdcfc0d1bad246efb17e3911712e7e2f3200591d5f9fade66051aace6f269b55cce361bcf9f4809fbb8e97bbeac5dc8e8a2d1e8" },
                { "hi-IN", "6b3d16309796bec09f2eda1ef8ce7dd4aa7c1e413072ce0f6b78809deb958e32ad824ad599b950b97e5c992bb1336718bc9d74feacbb424dd9d92d698534dae7" },
                { "hr", "c946baf0fd23ff531029db768d5be09b851ec91e9b4decac1d2142c5003d8d5d4b93da064795e42262d2f75543fbd6d5c5ce6ef6429e3b294416766369acdae0" },
                { "hsb", "71a8a72307eb8d8b8b10c932f8e3bf34d4202f061ace083cb6e507f45b047fd6c78b027adfcb5fd46762535f45d26266c655f46a08b6293636b84f01188b5546" },
                { "hu", "67d03e6c8c467c1d33ab1d344f86b68d8c5ac33d57bf6b1d50565cbaf3a7bf196a23d27438d2cde5bbaf566d91d80dfbcbc8a4fe965a75cd5de83cf4adfb4035" },
                { "hy-AM", "4c6319c028cfbab4a7c3c255cf1af6189722fd081c4b06f2f7bcf2b2cc9b20fc3a001da7e1f39dee138b3eb843a7ce6815a7c451226162803539528369cd078a" },
                { "ia", "7bfe0809ebf1099fe57cf7d649095a95b909e1e06a1bc88fde899334560117a06645260e4fd84fece9f578a6154228e5ff57cd3c4e74e8bc3fa028ed26c06756" },
                { "id", "ddf83a9ea44b1e09cf955d1ad91cecbeb9acad675b883913783a0d23a6a388f7dd35f29c1cf8a0dc6678e3044ca7f9d14771ef8f087c19b9438c19f184267064" },
                { "is", "99a56eb96a44501f9347eca7e25c11d7edaf51f107d62dbc5e2c70fcc343a617c76bfae1b4c1316f90fcc9170662fca6882b2b3724691f287373fef64e536522" },
                { "it", "ed9a403416b9d8884a88b6444959b2ae168dc15813c1abe912f4545082f68d30bfc204aa1757956c17168b4f98d71206d2814a24ffd315a40107b90c99b16d59" },
                { "ja", "c51afc37f01582ae3840dc6723aa9c0c79a73214491430c3340fa843458295de226fffdff199697df479b09c418a81f373b3ce862fad502d8bda774f6be18e5a" },
                { "ka", "67ba980edf0358af51e1706f61302e2798090892d932982e6944be54879e2d49a7a9908484cd21c7804ff0e077894d83c7e2f01935ccfaa03b4ce681ef8455db" },
                { "kab", "854855516229854f8db9e7e01b6f6cef2da87cd5b7f37833af1c92b208bca7ba8610d8f800b0e1496652059ca0ded4313bb7c62b33d58d07ee4fc6314e0290b1" },
                { "kk", "77043f39ce2ebf0b52e6363d4a8294988561b1bd9ad6e8289a2b664a5f7e7f77386ea7dfe12e1aede74545b0febd8314c625e2a56e245d69e289b62be41fbd64" },
                { "km", "40be51e137e70f395d25f7caef4d8c4de6d8b45c9a282d904a7a0eeacf532e47b6a62fe77b639ac407b1ec7578e84aa1a806bf1f26b7e54f95bc08f060b2c702" },
                { "kn", "41dc9251207adf53dc09fc74aafa05a5f139d6e9ff2042177eb9249130d8f8f50380cc14a055ce87788763387bf073bf6efc3ce4b3893ad170ec1a9618664641" },
                { "ko", "5ab532944384a4948f89495ea4c14438fd410a7c4d5d7feb61b90deca0b5688f3f255bf2b512a3871b1c05c9ff2ca7025adb400c569c786db4f8ca21d01c156c" },
                { "lij", "096d6c4d29a41d56ae7a766953cf8f9e7d86187a2d1c1b6d73315f36270fdba3c1bc0282495dc6faff4bf8de8b191cdb991560a1d32a7d11d8c2c300586b3a34" },
                { "lt", "2558f09684e59347cafa9cf7da73ee40ebe0923e1f7141363ca7e0735ae7cd155335562b7ff12f42d579bdbfb042cc6e786b3cc90d65a328377a52b7ee446308" },
                { "lv", "897fe37348ab9c4bbfdd27e71f3ca7010d99e94797a50094628fba0be2015087ea33298b25d88c04024e20a4305e7ada5a950b7129ccd16b1211cf5730806bb5" },
                { "mk", "cff37e343d77cd5075c80b3c667e5bbcb8206cc44e7f18d416b16efd047e3437defae02e55d0acb9eafc6e62c471296e89212ae63f5509f83c08aacfb26e8e99" },
                { "mr", "e9bf786b9b554cc152a5c349c30d9617fa2d2946d1a3b5051ed27b1ced2cdbf634c2b4fbd941c537c397421a5aec8ebf48cbac841be6699659cab8899dee3cb5" },
                { "ms", "73bdcda8467ecc0a4aa733e34b3aba2b5d448db2a807ad698316e0a8fa38849f346a8e63f10ea86f99d75cb8032185825f6fad67d2033ca95bf0edb09ae6a077" },
                { "my", "6686aa424c1c4047d29f041ce76eaebd7b52460b0fcdce8661405480556b983fc6f2f07697d2613485deede5da77a7873c722d3ffbe8b822712d2e3f3562f135" },
                { "nb-NO", "734e9b799d6cfdec9b543b435f5580a2af6229cd36b3ce1b610eb1849d09b1a36da69c3d26a9fb41ac87daa2a591e79a24eb37ea8af2de7d09e6bb6f752235bc" },
                { "ne-NP", "0e33fd30440c74f265bca9b7630e3391a399b50dbef0e8313ee49f4811609c39ffd8a76220bedfd4bf77186bcf79c9d125d919f63948a75366bf4771a52399cc" },
                { "nl", "2d5019d8b88bf0ce7557f0b408d22ed1713e369475434b559ae51fe7548db05af744c59d304bec6b56a49848a71eeb20c2485fd3fbe8270df41c0188e7f676a5" },
                { "nn-NO", "5501c99f389f304b01ed72de5e4a3df0455fde704cf70b9424128b9df8d29a049e13ba6d7430ee831c2d5a3f6ca5473fac0750081b9b2026f743c452ed8f0af3" },
                { "oc", "01ec6fd1a6e612581c6c0bd15b924aa11c0238fed6680d5328f1a9ebab1b84ba461dc627cd153c595029578a85865bd68f39cfdf3d0ef6d1a96bd0bc149dcc8b" },
                { "pa-IN", "4cd1b50d008748f6928053e9df4284ea31fadd2b944002555db987c678525d836727fb9b88b9922ac5dd9241ec6acaa974e187cb6a5892026025c38d6a17f2c0" },
                { "pl", "c729ab749e2b8bfc85aa4549b4413c398ec600d6bd5d4f8ee3cdaaa59021d846c3da7d098f200d6f84db8c32a0cd5d98602cdad93ea7048b94b44ba4a5338b6c" },
                { "pt-BR", "20638cbab9487752dddeaa69d7962dc38a7fc1f77032647a4592997344c83929ffd8d379fc0d40a91f82f3b86c289f2b2f248caa839fab15f86951ea77b93a50" },
                { "pt-PT", "acdb6939d1878a5c27e527c5625a5ba37d17778973e81d0b7dbc70b2269b97c4061c3c0c87c88c5695d0aa5f4f2c59fb586532ffc2405bdd6f752e6ad13e3b34" },
                { "rm", "ae24b6743b36391399d618b1bb15c9f899af356a2592ba51ae3cec9d6201361493cd562183aa22cc2636be01922e6fee9cd267cd64aee455f0bd96f26d208f6f" },
                { "ro", "5832aa8e1e38ef9bb244770d93902a74102a4756e7fa6c9a07db238ce3e949f7bf8d5bdf59f22e8702ae5bb0792ea2964106391be0cdfdb0541504432e3ea6bf" },
                { "ru", "80823393db12656855610a7ed05639ddc9429c28afa584ddc381d31cd198a4a74ca146156f22c35352a218e1ccbd67d5d6fe9c208c4e173186839656d3b78afb" },
                { "sat", "e3e8e1bf8142c3e0c1847f7b789d7ef27919a7ca65eff0ee9007340d50465e965917b61dc099bd412ef32d299896d4dd5d6cf402c627d5f6e022a26fb8b44764" },
                { "sc", "4c5261d4ddddbcfddf6abc305b3a0afd2e5168072b1f74ffce4051a4d72f2c66a4f4303677e381b7639bca62838b7c07cddb4a0cdb0e1c27659a098c99550499" },
                { "sco", "c63fe742a1d0e8506d84cd1b3b87ba5f6c76c573438494ed083fa5b1e561d60ec2c6cfd47ba48d71d98d7104ca5cdeccca4d482d8d16d0787b7c97b73ec4182c" },
                { "si", "371daea25bf44ccd584221c7def77661eb6dafdf4ee85a46eb96995af240e9a2afd0ab58f72f70d9edad854e1fbb491f2335ecff454dd59f0cbc4cf0eeef8946" },
                { "sk", "bd9e8842a295667645132704aaf6584d61fc2b5632f40c9ef4764bf8853672a337f7707fd35aa66e1a0f0ebefebe364097f994765cb07435584574cc3024a60d" },
                { "skr", "3beebbf460c78463489ef3dc98140d84e1cdce124578430da9daa60721201c38cf4713eb0bf496eafcb7a52e89a5a076c20f8f5c7e942f3f4bf2297cb71eba25" },
                { "sl", "7cc80523e21f2809c4bd931296f8630615be150e489857f1be12485b5951eed2a626f069afd5ecb12850aecf5976e2df4439c46293f2d08337bbaece3bc88ecb" },
                { "son", "7bf39154be540e6da2eb2cfdcaf476a5928768c0a6aeffd0eeadbbfb9a8b7765f96ab7c7d07ef87689761fe0516f111376ba1e9342744370b98fe7d03b18f671" },
                { "sq", "33126445a65a4aebe3832f1af16b01a44847ce3055ca5a0f128eb0a28e165dacf06b335989e60a4f9701748e8ad4b456a7ac37527d3782988f12fe78fc75680e" },
                { "sr", "071bd5a0f6b858aa53c45480252d4ad07262fbc9e1982589067b519e85a05b7d2ce7cc2234be428b3040ae4678ae4764ad654ed53511fe46de30a7e57a2ca00a" },
                { "sv-SE", "229601844afcd0208e072f4f4f2214515fa2ca2fc776da55a608a95c658b647c769e75eb4b8d3a58a0af99b5ba471fe5561900a689b11ddf3b10df99b6846471" },
                { "szl", "c28ceaedf63adf0c2abb35ae375d6e4212006ce1426a37207cd47b71e7bae34438bfff50822c091856fa3103dc45b3607f315ed8c68a6471586fd866ea8f00eb" },
                { "ta", "f39b9f6faaddffb59fbb3727337a05616744d989cfb048faa56fea2a5f47ee07273db2306dcf35f13ac5ab360e6fb2adb743c2ab8e2de61a35bdf16da4a4bad8" },
                { "te", "0e7408236969119d810f05f6b10bd8ca20c1675318d5172f0d675fe783896cc9386a508a5b482e0793fb23aef4f36e2db8ca83a24808ab66a4938453eed64eef" },
                { "tg", "e5dda459da16745b3b04644a30f88e27b875ea0c107bc10ea8028a74a2581c64fc1f0a569994e1279653a807ce9911dfbc485c16d019cf87136c871b58c2e774" },
                { "th", "0a70bfe1c451f2abbc7ce895f6f811d6f3e436b4a10ecb37c9a2287007eb8f57f6201895ee2f9cf986fc4ac3f66b4e198bc8d6ddc344517ed493fe41ffe89b7e" },
                { "tl", "ba79f2e1cbd3470064021f6385316c42c1a7de4a73698e089ba200cf8a5274421a435b4e7ef8c2442bb71cc792a2ac5327dfa4e3ef8fc6b3e7ceef29652e61f9" },
                { "tr", "5a8d283d0c8e0480f24ce1c023f3d3ff2ab825ede7a6db0d14ddb0af4fd114916e5528c28e7458a516a7ec63004134335876977986bf44093b751a212c4772d7" },
                { "trs", "ebdc49a25bbdefad9d2b81ec6dc7c5f2c9162b34380a66b92591e2d27129d2df3314bfdcf4eec0a1584a336aa5bb8f12d9a9e56bfc12a954a8d33becefc6e55a" },
                { "uk", "8d2d4e2ce68568b8913c14044f4079c89f699ba368fde0539015f4a9340bf3bac8289a048fd7de1f788b9e349dd07c77274bd8f46276d7c5f8227aba9dc839ad" },
                { "ur", "4f4a2859372df9815d4f4b3c2b63bb239887708075b9e337a5c99c4f06e3407160a569f6d0d05947cbcfefa747bcae042632202a9353235bac613d6667f25caf" },
                { "uz", "eda94453568b959198c6cf0a85b100f12c279060105c753513c222dc1c184aff9da30226ed677286562c87c4b7880422ffd84a27561c7a14383bfcbff8f881a7" },
                { "vi", "40f0855bf6eb4d4ef24dda00586bfe74213eeb2b3b36263c910aef34182801c4fc4585cdbb4d2b3120c2fe99704cd376d63e1ea4268b1b6f2ad4efcdca83c30f" },
                { "xh", "0e809fde755ccf1b203ce980fd4f0087a38aab2aceaef61133b39bda12d853057c0a6b8a053973bf48b12dc07a7fb9dc230383fd81c96e588af9845f801bc284" },
                { "zh-CN", "a6fb7d542ec336e736420f184f0e353c96a0290971ca9a1866f2734e916dd6cc946354878068d42fb938a71b93c40ccb1148ca6e7f96e9cf24b03534b82f3157" },
                { "zh-TW", "17df57d26f59d7548c4c159583050a2a8bd9497d480784c065c71c01e2402e7216667a03e82ca766772968fb4e6e88e8a083f52734fc11a98018ea38f90604a3" }
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
            const string knownVersion = "136.0.2";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
            return ["firefox", "firefox-" + languageCode.ToLower()];
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
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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

            // look for line with the correct language code and version for 32-bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return [matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128]];
        }


        /// <summary>
        /// Determines whether the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Firefox...");
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
            return [];
        }


        /// <summary>
        /// language code for the Firefox ESR version
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
