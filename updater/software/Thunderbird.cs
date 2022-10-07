﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022  Dirk Stolle

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
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.3.2/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "47fb4828ada2dd3d8e5ea23dd385706f25021f34deeb9d4e3aeb665904e1d911beaa7152b6f582bf1edcaba5ce8982e5c568e6712dd2b88eea979767f7012be9" },
                { "ar", "d0ce0f232c0ca336a6303278f70ab4318d7cfbeb19af10231cdf91415656140c3037a0f6c0c15d4232a87075a9f4a85f86426c8f48d137c9a05d2de669a2cc9b" },
                { "ast", "ae1e0e1d31a4428aac992c013423baebac0afe344f0e098c5cffead5001c522a2a1496e62180a8a678e6648e155c761c7c5c188dd86f84a8f8a9709a7b8ea054" },
                { "be", "9c7744d9e09fee925f924d0e1eeddbe09f02ff67fc6e23149ced544cd32e47b02d1dc52fd02d3c9d2b647266cd026a70491c7cd584a58a0c6a32ee478970c480" },
                { "bg", "6f962c9d89e50314f1e252eb7b0442a792d25e3722ad6cbe4de20e0a67cf1f3787ce47e6d87c0b956df4dffc284ebacd5e406d713ae468e7c6b3417306d0ae74" },
                { "br", "af31e774a5dcb04372d7a50b2fb6b04fde0c632ef796d9810b23e9d0496f1a4460a4ed1cf8e62b8d6ce95cfb5388b69543acabf3d0143cdc1925eabfe4137fd7" },
                { "ca", "cb7eacdef8f71fd50a5a0f2bcbcd83ed6e72db38f6f1cd8bcdb5679018ab238b03ac9c049cfa843994142cefc83d0b173de660e2edfb092d5124832b4422f697" },
                { "cak", "f7c4f849d9fdddf7b9303b1bc4a4dd9922ca62b1e68344c7bacc2e8dce71a5c50397e7beca8ab2809065c5dd584e4596b1ef878ad3888a9b4705496386ad30eb" },
                { "cs", "6a66ba26ef9858d0856c65462bc1a6d33302caaf8e53cafc4d39cdd4b6df19d85be8ce96592484a337b21566434423c460992848ed5f2a646b0e917156c66d40" },
                { "cy", "c1238d140fc1ca8c95976915b6f25d3273762216b0c92a43ac6a15ad84fbf580683ee531846a47cf648de42b304f49d9dd9a988b610e878cf39f1ad12afa807e" },
                { "da", "356221049d68fedc2692959461329c36035c5d9e028f229859ca65404d4ff9c751a22570dd593dbbe1b18a94410b8430653800843de51b95371e64c68312e26a" },
                { "de", "cf40e647524375484b3a4c1811162e69d3a9bcdf37ee001ae07760094ab7241433683c35e20f6fe499a75bc5ac2c293ef4bd9d4ca7fc72f8f473c340600c9c4d" },
                { "dsb", "8212b7c5e4502e42bc94f2e159923136013f8cea836c429ae7930c7d705b1e7910afc0a2f5fbafde6b0e264745b6552b1a1ee82f10eb1389d4e41676790cbce3" },
                { "el", "43f79b574c678102104bc67563aa8ac81290c94bd308db3662fd9e6db0bf748b15b3b958544dd51ce5d646ce2a5fe1cadf528463e342ec5bd4f1107215ee2577" },
                { "en-CA", "d538ed301bc516f22d56e44f9a1305c077e1d568eb995c0d4b01d88ae24d1d31c512d135243c40f0092d66edcb960deb20a8c455064bb3c00737ea72b538309d" },
                { "en-GB", "8b0bd97a1adbe5bb5a4f64f78ce97fa7a42077ea97f172c83702da724416aaa431d4f62b28adb3dda3e6dc2f11dd539991bfa5a7e398bb1bf0a2659ae5365804" },
                { "en-US", "ccbd95efe9947e93f6e576e398e68c931aaba141fff142f2641b05c54097e759d824abb3c64e37064fb14c9e6f13b27764ae15f77d156ec47898587a2b2d861b" },
                { "es-AR", "fac33c7dc1ab19851941948b62b8d9b94c2675b3d021a8efd79660f5119c9d08b201bb64dd2a9ae7053cdc84d824212219ca582941f93d2d52c8f052f827b76f" },
                { "es-ES", "4a1576b83c33d46b2f273b59512a0142e6accef67cb00e608e9add583edc02e8e4f38606d8e128e9857ac2dca7514b714d1c12259613cfbf402790dd03d99ab2" },
                { "es-MX", "7c73ab23aa58c94cd1fd0472c6e003b394914f7facc0ff470d68b960f996fd7d9aa80a9e7ae18b6a0e22ace38e3c644788c9f233b18920ce9ede1e038734e6a6" },
                { "et", "f0fb19b89f6de76deee274dd35f926b807e7828e87412677d7901b1f49a95e602d4fecb8654bad059b2fd7ebcb0942b05a846fe0add2fbe416daabf0e6c77155" },
                { "eu", "6331cad7c423e60add292c90407a72f3939357662a1d83919a58b704e351f82cd8964a863ace7f1b471d220acfcff6972e142dd1bf066aa835275d8bef09e91a" },
                { "fi", "ddea279a84557dc0710248d8c83b1fdf03a07e2cac463f66f2e2f846a732ac8260aaaf4ecdcea2dab22fd78aab44861f72afbb7d92bd79cde45ebad7cd3c12e7" },
                { "fr", "880bdc59ddf523563258741e81989c05f665cd92c6a9db3ebec93b62fbaaf0224e150db4eeabe701b77f5076021ad6a46cfbb52300f8e36766289fe749dc3364" },
                { "fy-NL", "9702989bea624ce8d320efd327bd1026e73794a05f790c9d6048c593524cc72e14268fdcb4c554b3b1e43d374eea1ff6a2cbe52c5b7d55a6bd77827c78f44977" },
                { "ga-IE", "3e173abc500ea64bb4b08bf11b03dc9aa91ce065bcc621d5438341fab0323bbce69aa913cbe8c274c3d6852704c10548ba290c776f26b550ccd5df45bb62a02c" },
                { "gd", "a53996d94aea90f3addb2d80f0b35169f5355edd3ecbff12bb9e554bae845a55b45d02a36efba8236565749492dcdc2da85f5800043dc54665b625613d788cb0" },
                { "gl", "d195ea8c68de0c3f2c33068e25c48bfa6143924c810045148c911d8c440fe107971573730e37f6b2b1ac127ce393c33fb95328396171cc8ff1b9cbc587e11d89" },
                { "he", "16727ac9710273e49ccbfc6063500689fb01936aa3a9d52799e3bb4e08987cbfccbcf5d0144794a1b0e57e86a3e4f4fa78765293627f98ddb8e722b8f6561d3c" },
                { "hr", "c01c1377d5806cf3c121aa88098b19dfc1e22d59e2c6920c63c926643a95f78462ec3f92986c07e79560f067d54d108a7d3cd504301c74106c598b818752034c" },
                { "hsb", "f1d91a2b7ed21239cd64a8ac34f4ffedb0b53637c694517b640629ea6b9118582fd7a66bf74b52c61257139fa1eb152c4e64e09a0e7f50513de26858ca622119" },
                { "hu", "b241e371f876a7e89fa4781abffd7c256d6bf02b38e55716364522663c367e70a8f7b3bed16073f70be9623c47a03ea5ca90cb7ef9825def6d163d8e81feded5" },
                { "hy-AM", "b79e6c9fb74cb47b31676373123cad7ba72ed0341e91246949df6520b233b9e3aed4d6174867638a98cbd7a1b33e11650153007844ee66b41722c84a6c4c177e" },
                { "id", "a9602d527967ff905ddf61a7148170948d3a610142c0e8f1d1f20ab203cd3e4d86ea2c0ad8cb7869599b81af2247d38bc35cc0acab9b7b6d403bd2b5b194bdcc" },
                { "is", "092415b2a4256d803169c430dd2077dcc57e627fb0da66befe0c0f503a3bb26f61516a86aa22ea50ec098015dc3f12dbf1dbc625cb18b1437b5e678f411d3af5" },
                { "it", "7f7e3f9803ce4fd8aeb2ee1d77fd1250ce7f61a7f83027954bca40c2d409a6976343b62177b8ce0ec3ea0b219e8db752934aa064dd8eb71c5992121114510503" },
                { "ja", "ee50c4abf34bd2cda02efe07aebc8afb67a748b509b50814e442c04b3dedf3e0354e5b0c8b57e0aaadcb1680c1f4fbc0806d13ff890f72526ed580f1bf84aad4" },
                { "ka", "8077a0927c868424d1aa52f9dd5099b19b124b7886080ef2b7f48d48aa69dce3adec472842dc4dde8f68d11394e1967cb265be4e1df8cfd90f262f34480dbc9a" },
                { "kab", "e0e51045c60c5ffff1720f7f4ceb46f0ef954e9bca37fc83f7300537ba53673f9705a8f0d9f8d3edd699ebd9dc8553f5a7f8e37c9a73809b1c87d34974acd70f" },
                { "kk", "4f9ca9f54a6c00f58482729a93aa4c60ee2553e41a3bccb9bf625f4e022b9ad2a763c2b512ccf5f25801798968a00746f58d94d4f0eb9bb7b2379530f889df3a" },
                { "ko", "45157b4e32c143fe286cd6e9c2c156997f0dafce630f8bd209dc8d4b8ee30a3515e3248da647411ed7ab6b43b481aa7b71cb40fddd1229a0a6eedb221673c7b7" },
                { "lt", "c0174a330231ea63eb81bc67c23929299674f6d381d5d0ab8e28cc3782033496dbbb809bb56ec9976c7181e0e97d960570838be1eb51f9628b31018b0a883b95" },
                { "lv", "a3b951c720594730c4d67459c9a673be545929226b84b6d2af11da7d12b7274108ac9685b7a97b6798fc92b4042c6d1701d6e5e7e950a6d677267c5cac8e8654" },
                { "ms", "dfe77f614d57ebdf622379d551199f6ed44dd28e1802d0542cd8142fd99483bf846c857583375c6b3a52331e03d94fe9cc6fbbb633e9d2110c0c8308442b10fa" },
                { "nb-NO", "f3aa9a7379824af37e87ba9374c39752c8c45cdb45758733299f987164c379c9d427d66e279f75e47c8368250da215b3a238d47aaadfdb9f209be02605d774e6" },
                { "nl", "35b14d06c4d9958c77fa210d2d673ecdde11e0d2a8e8d4530985d765fc89a4bf25857d66d47716834953877fae7f978d9f9fcee334fa5c4576065b8badf15a02" },
                { "nn-NO", "8306f77a26f56c7ceb11f0c2a89ea3ddf4a5ee12192bff45b7bc84ffddb396ea850c32a2f32a39f1d1b4f180e58f4d503d7f459acb5c024d23ae111ba98a681d" },
                { "pa-IN", "444cea7925be8c066bd3c95aea144e3c917520912841486a126bd029afd028de4392e33a17d1d6bcdd7a69223ae5e1ce6e84cbb9994c77ff82ee6bc86104f881" },
                { "pl", "e34e71cedfc3d4eb554affe95b11a0f0828ccf41faf9c804408239c2d70f5590b4e50c67db237010255d9777b8b9ff9106935392f041b4299b9056aab75ff836" },
                { "pt-BR", "bacc9f9a9200172a4898b3fdae5d99115106c273b0ed46573a7eb44a4511e1133c45a09dab8e453a7de74e80596e1d65c544e4c1f10721120cdc5cda3f60393e" },
                { "pt-PT", "59ee416a5eae5a99099dce0f18a0134c6f01041e63c10d7067234587055d6d85115ee7a2f38dabdf3aeaf681278876fc8a61aa990f75c16ee9e8f411caa7f30c" },
                { "rm", "8aabb7e548ee402787b13f9c54750e3a5d17195d510a2959a2a766fa7890ce0ac1667761534462a8733b09b9b44ce4ad2b1527debbe4422d39ec2367fde9f5bc" },
                { "ro", "0c199dabf8fde18ca82b4b884a2de5e7c44330dbe993d9949d174259e12ed73724c5a5c8f2853568073dbba213d59762c1bb28e05b99f4e35892089a225f046c" },
                { "ru", "650f3a25bee8b2b6b4d0ed4c482430cdd7fe6ef4ab5d6f302cc79944cdf0dceb8ca6aece9c5119ae9dcf525dce9f19cfe9ad0fc7baec733c40811f438c3d3292" },
                { "sk", "64f02f218efcdb38f88cfbd7c6ceda26dc27de9ff4a49c0d33a0c9f7a56613a06253c55b7a5558dee962c05059c136c2e93ff61343bfa2b86dedad54ac61e088" },
                { "sl", "da8f66e1175cbff95260be461936f9615f12eb1d15130e91299b2a9473ab4b9f28c2895f2eef3cc0da593f588209014b7c7079af6b40247e8e31537c072242ae" },
                { "sq", "676058c81269f845b47dad6b42cdc439d13abb713df6613df473277c3dadac7d6cda1ad10be4f5e992e091fb21d17b17b38aecc462d7f973253bedec6510ed87" },
                { "sr", "76f8f54c1383adf06e9ed6e5cc0a1d4778f84359eadfac50696c5ca968682a8eb632ef97ea67df20cb35f5155c4831a130fc9b94c266608332328b5afdc6209d" },
                { "sv-SE", "0a2ad5a17de6568239c51e6143eb7cb54131839fab208baf307cce6be44b56c8a3ceb4abc1563b12ae171d563489a0f28c2cbe970cb65a8c8c3814873ddc56e6" },
                { "th", "e6ec95492fead8847cda319b79c12d5d96014f2167ea602737678b8114cf9a6528f85a9211b25e51d7bd3136a2f50d6ff8bffbe8f36f8c3da40ce6336adf3ec7" },
                { "tr", "b229de734880044fe7907e327802406494d86254b54437ba634c26b0e7370d79156098f65f68cc30a805091eecff3386f56de3cfc526382d7cc86dd50ab4ab81" },
                { "uk", "a0d2d5b8b99632ef570c4dfaa87553824604723d8844a0236beac20b0f44c8b14105cc5ee7f7c7b94fd2fe5639027a5312b3d14a91e78b46d06b77d29fbbfed8" },
                { "uz", "7d359ba538c6be244b7a77aea4b2d6e9aa92d80728f208f89c9c8ae6f3f09c9beb43b31f24dd0682775cce9d4d6efed2935015e4b9bdbfb1257eecfce03f9bf2" },
                { "vi", "7fa56a3fc5e83f787a947bd54d7a81f6d6cf2951d4386fc73493e1778ac03ed3f0ef93213eceb21376fcb76b5750f3c04770003283f8b5b36e3feabdf08691bd" },
                { "zh-CN", "003f09ae91300838f85c3755e236f592dba1bab1707bc387f65b1a12140d34a9e5cc93d4f37929bf495c6f690ac25cd11ee512a2ce37362e10599780f125a1a6" },
                { "zh-TW", "bff488ed454bd9eeb0d76630baea599863e017ffe07304d351587451ba7c5dc9337bb839f72286075479c6e6ff66c54baa4114f38f2bf2db800b1c5de589320f" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.3.2/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "d5951be87a04d8d4dd505476bfb9351edec5509ec6ffb7c31afc0f3475471740a6979e493b54b5ef9f08909285e0b472b0f59d505d676b6a5918b9142480485e" },
                { "ar", "196d3251705d28e972617914841bfa614b1febbcbe7774f78160f0cc027f7c44c4a7ff00b59b4e6c7cdf50858f67a1413246c51eeb7c47e32962412442ba64f5" },
                { "ast", "611a389987f752d5cd58fb7633379eb21a61c1ee01a5c6530e3e18eaa9519f7bc027656b1b12ea314c94a80e0b0799a397d40385f2c317cac3197ed92763db4f" },
                { "be", "faa5b707dcbe5c621fe5b96360489fef8e7c8dd1eb90384d1c74daf231bd34c3453acc664eac7d4c99ab1d8de8c6ed79fd366fd60f7d38bfa985e227cc1bdde3" },
                { "bg", "0848077466833df76da3c8c2440c74c667c87ceb3111cfba6341840b9b6889f2e02c920f95c58de63d8dd5944fdf8efcab423c7928b4357b07bc723ae26a239d" },
                { "br", "5c9a7d8984f434a21ac64d00f08363f97a57533da278532fe06533610e2cbe14dfee36ffffc3d9a9218ac4ad5fa85341a3663d5ba231cad32475106dda8e457a" },
                { "ca", "5decc7289b06916eb6696a11ea5b5afb382db64b728fa284f6cdcca37d18cc7670621de3170f946e5dbb950617d9e9eec65a97f8f10e0e7ccbb292ef958e23a0" },
                { "cak", "93780239cc28f96c8701a7d1d6b2b154a52c9bf937965a623627f23a3446a8f1e75853c71148d30e31a4707382cb89b6b8c329066fc8bb85b4be33657ca6983a" },
                { "cs", "523f46b62cab1c5978bb7120f62b1afc24311c261e89ceb341cbae3ae584da4c2f03bb5302b8e2bc1b96fd04b25bba2db9df0add2c7288537ef059c0d481a663" },
                { "cy", "b49b7a2b8afc05117c142dbb12c03afefcb947f075c4bd7d6ed19b36f6895c4ba71ec54944bfbd4dcf3a05cf25701cda79e935ede004b6c7e1f7bbaf870b686f" },
                { "da", "4ba225d3b4b4f8addde3d8b8fe01ff3a91750bf0d06c4d913cce9e3a34c04a02a44d63cf4568884de72af55d82e2e64acd9fae15c9f5c1edc6ff32d1f609c95a" },
                { "de", "40cdcfd67c4f856d2808136ba62ad75ea717b200f41515d8d21242efb9b3f24f75c588bce00949d6ebaf63a7266378cc81908618731fe374770d7a72abafb856" },
                { "dsb", "2df85073e7556e0713eed6b24bad001f8443f61c0c7ac5b7f7b66c03e8f6ab58399e111cf477d399e0154e7b9b3a7850b8d47ddceb72bb86c46e550b4f3ecdaa" },
                { "el", "3e140068f0e129c1921b0d0632dfafb670298bb2624d8f50cef18219c5d161086fa0dcf619f4a423d63f6a5710a055d12fac56de3b726c2d4ee9cbae58012990" },
                { "en-CA", "eec59870c7e0c3d4fe923916f7cbdf04acf68268097b0271610041bc5c5065ae86f8c47147aff9ded0610784e6af72d8a3f1a3da342420f47eab008b1f390bd8" },
                { "en-GB", "afeb2ef32ee633e74ccd0ca6bdd14f82487abc40499fa0fd30792a2d133453c26363f7c1298af0ae7129becf8a0b827b88400745426d0d1cd660970183de06b5" },
                { "en-US", "a03cf9c80aab28f51c1fa6154dff0a7bced89a3afde24c10afc2992a6c0f0c66a8fa7e7f511ff6af106ace5261a3d1dbc90b6f581aa692b3bd3c1023892028a9" },
                { "es-AR", "b7d26e90a33a635cecb859dcff92e243b9bef1b0ad70a3c6b62e76dbe42716b31380f7fe04b74849da6471fb59706e48f4451fa2b795c10cd6f35b821d7c7c76" },
                { "es-ES", "cc0e5f9e28ae0ecaded8de17863f190fe34f30f9b01c0129cad4cf0fc47154a3909547d646ff165d2141049597cd566bdaed8d2e680b1d3c717310ba761021f2" },
                { "es-MX", "28f5afe65def5965b6a081cf3dbe195757fb17a042ca43497be1e6f4e4383002af5ebd487816344fe50095a861ccb1c49689631db16a5aa4979ed2c918099464" },
                { "et", "a530ae79b75580dee27a2604100139a89ca0f0f13a99eb5b80e0a65192b8a5c4f6328f2527b1289bd833c2a5a89e2e4f99e9e3e5de1a2d28e8af086b6fc03218" },
                { "eu", "242c99b66fe1b2f118c53b948609fba9e531039a9dfad3fc3675fed82171fd034163eb70344b7b03421124d28d5721a1cb24e3cb87666ef9cb866b12d70be497" },
                { "fi", "5030b78f0ad7cab13e0e501f3045cebe230bf15470d391dd8479763859571e737850899dae726aeaa71bd5940fc7bc24e7fa36fe2819fe6d8c367c030d65c3c3" },
                { "fr", "d14fc84eb1d5c8c58d66fe05553804c1c7c0ac1c7f95e5823f2bb77f8468f2f1e0b569fee335229881821f96cae161af5cd4c71c10666e7bb3ea8e64d388438b" },
                { "fy-NL", "bcf6f25a59804ce3746073cd8ea3a8543dee457a8668235b4476951fd044a7f535d090111a2b3de3969d446f4f7eb18caff997c44c07b16780a238c86cd8ed6f" },
                { "ga-IE", "0f5962b33a2737558c6925d5c8e78332fe75f99cc51fcd645243a60962d4204b5bd9c8c966b56e5f7ce80e2171dfd7d114e4296f86f749d6c22959944a0fb0b7" },
                { "gd", "07e48895917d5354d8cf82214c59f2d9fde1d1c4099a7f392784054ad290d526472faa0980f3b8740cc9786576e37d28042865ba3a8003137ebae0772a48ace4" },
                { "gl", "38e1e8b0c2106af2508bc5bb453d2d108e1796b62c1cbc0b77fba1768fea5472030e800e84910c3d95e2086351332a907219438e25447183c3107ca3623182db" },
                { "he", "3a71ae6c79d76425be52ca20e70facff1edbce1380578d976e75a3b0f41a45b4717a4933b25359d81c412f17f6a654ec48ed8acf7d5f7d3547a21c756ac92349" },
                { "hr", "c2d60a1d2ef8aed3bc892b32b403a50eb8e238f6e0912c767fbbfdf21d64605d215ddc2101068f468b25825a99b098ba9f7b751e20fa4adf9958cd9bdc522a0e" },
                { "hsb", "dbee0d0e6911a06946939e3ecedde10a49df76681970540f1ad8e7ca7e8207e95021a0761f94c7b61c5c5f898bb5fab917a5f5990103d97efde98eaf455b42eb" },
                { "hu", "8561bfd1828a74553221958e892b22e73da9800ad01bcc99396c492af8c78ed077e13b850a5ad50f641d9764e408d4f89fef309b9ce68e53a90786ba9adf32fc" },
                { "hy-AM", "56d0c25686dd30813b53821b7eb47c0fc167928927e2895d2cc50fdc9a83a89a233aaae61f9b94998f10ef25388a21718909a11f9ebc99022f752791dea88850" },
                { "id", "e38fca5d06fd4644c8eb9c266a68dfe3f78b1089bf3efc78256efb535d009f90cdd61cdafdf4cb8a7c593762ecf9a5a04632b762a63ef732100dd7c562477b6b" },
                { "is", "c50f71bcad3725049b420ba03c5a27c92a53dd7c2339b37ed26199c103eab20b3d9a0ad587754f6412bbb95c6819466d16e8bd6fda9c06e10389ba4229932528" },
                { "it", "f44655c99111f6233c01523f18f6bc70f885930a8684027419d4054c625d158bca99c7abdc28e322c6343ea6664486532a2b041f183eddb17f62a40b276af3b9" },
                { "ja", "7b2be8c3d797debe18e5efc91c3a7ffbb6d9cfbc001144733d541ca80edf38431a76a6f5863a8a8d9010c025b7bf2ba7eb972e6c042a0038636aa632aca60287" },
                { "ka", "0aca0ecc6b1229338046892a47a7c8469704b4e38bccf70276e67d166a193da5ea6713645420ea4215d001f934af571b60faefc797e4ef5dbb960ccde5ad7d41" },
                { "kab", "fad6c85fda93bb565df9d06946a40fb9ff49dd2ba7d2683a510094d907aa46f69ae6b94e8f637114ba24a1182603ab685253b8e27cfe4ee5f02af439670748dd" },
                { "kk", "616c8ad51f44635c8e93c88579674ee0630f1518ea0a3fda6e6790a2e960183225f08d6dfaf51958d4aca0355950926270e9521b45fcee8d04dcd47525395d64" },
                { "ko", "97bb8b838fb6df8fc625605171fe8aaf343ad7be0b78807508026a6c873e7bc014d759798a3ae4a62e8fa0adc071e84fcf6ce0af8af7def3c5c8c47880a44c04" },
                { "lt", "04dd03a6df0a31faedd3208b9c0933f23d41b7e69851b7bc830b9e8c556989538948b778e15c7912c912696851b6c5be5d489901fafc5a5fc9c981852f82f159" },
                { "lv", "98332d5143d12db96d6f1a4f74f6f1a9364dff405f46779252e4a8314442c26bc7891229ddf556e68231c3c56617504a82be17392173281a70f370c5066cbc41" },
                { "ms", "a9839a4fb8dc1da02cf6678c57db42f0ac379475c4976ca81ef3ac6985e6d33889af0ad78525b80120d71baaaf50746bb8dc2e624f78d7073a1ee43ec8dc6740" },
                { "nb-NO", "b00470c03ef7f2787beb98f1aa1c4a7b60f3cafee66625f756e5b7e7c1a728da1f099a1799d8b398aa5e3c7f822a705309f2fa586739c8de3c36a20bb72baec2" },
                { "nl", "b50204e3c045aae6f28458e04cd5181fd5db9239db3b9add30ed7f218aeaf4b8a371d0962ce26d19fe8bb045e14d671d9235c79bd94b2a98e00626ea28c8f3b8" },
                { "nn-NO", "a855262777e60dff53679fccb0a3b1a32ebd59ca4e993f38d6421bb165b746f58c40733509cbce6a86b3070864b4e072ff8438afbacd79462814a331990eac95" },
                { "pa-IN", "6a9dbfc3cc4a634899292744923786af578223ebdfb1634cc1bc8f9e7ee2ed1dea1e86f2fa826e2f0ddf00a70a57c0f42c6b67758387eb956a8eb6a4506e43d1" },
                { "pl", "8b9c4c8368e2aa0300da3213aa47e5b1060d530a0cc80dddf32f5e8e6c4b36653728b0d73e841e80443cbec3d0155ca4dea7a45320f3e9e6667df7f649763193" },
                { "pt-BR", "7dffcd495a5bd862cc4a057b44bebd186a29c9c4b3b5102095fefdd0d5d9289613adc8c9b131743ee328388d7ad7a59e2ecd1ca2c9561489e61a69c76a2c350c" },
                { "pt-PT", "5f7fea2a4fefda9abfcc97d9d6029dc253b4559e3270269522a7b6b2d5db21e648a545f63e549ae5a41ff42d04b80a3768f2f5d2d265b5c7daba55e4e5675fc7" },
                { "rm", "1712b82c3f7bb660d43f55a0361f4050d84aa8f253ffe31cefa181281a22498ff836991e1ad71a36bd281495992b3c6527d8faf957c88e4592da99b8aea21cd5" },
                { "ro", "04aede5d4a8d3f3b24ef78a514a66c9eb8f98cffaaf85d11786691c0140cd076e2c6fb7a4cea349cc0841a3ca83e2d1b9e9b96a2a4b34143ea192677d36fd1c9" },
                { "ru", "b7fb3b427a556d881085dd57971b33e45f9a035bebafd011a22087da35732ca3d3f36e93736692783c1f80e5190100cba79268c8af138c0395a2d97281c8b913" },
                { "sk", "d2f131b9394b231d8466575d6ec64561cb5544356b6cbcec2b46d1c2a32b0995d2b3e2ddb7ce5cb07eaf5827635e55a0aef0a42496c8274371272d79a241ce97" },
                { "sl", "d8da7b8c8e2e827e34a98c8e37245b610017f8ae7fbe5f8cf909bf5a71b61683f952c510a6f6a29cdc377a7465eafabc1a763ec767b8d4e2863d7af56241ad67" },
                { "sq", "2e51cdd2c0c9a20172c9200d546f1a210bc6498cef31471414f0c92d486a54f6b452004fdab5c9bfc27683fa6f5be55aa527d2b1cf065d1c8234a84251ffc55f" },
                { "sr", "fd35165c8751a80f8ba1599c0fd5a6ccf82021f6a39bea1391f0e5a0b421e862676f8f484a8e4526e46ff1726da233f58505a5b96647499be18e3e008daa42de" },
                { "sv-SE", "2e90e7ba83de06ef78f056ecb54b1b0083917ac8de2c1ec5e874dc4f20fde502c3e66017e223e1721d7c8080906c0574ec2c305aed17eab6e44e165c1a26769d" },
                { "th", "5e8cfe89fc20e6296dbb202052083d233c1fcd3c5688929679ec89687cac966333ea897b66385fa580170c672e828cac477ac73869e45ed64dd6ed514b40f6c6" },
                { "tr", "c0f89f8cbda77105ee4985471ea76e822f537965b419507ca220ae16383e33c49b765f05a6bc0396e027fcd8c026ae3a54f0e18a9b45cd84e021b590b7c63193" },
                { "uk", "f07e129108b7e8e60dd2d398faf6309f9b9470d231fcf9b35a5621ae8fecba8e0d87d2f1e7bfe0c353d7dcd05b1296339a75f6fa5620f19da90e764989bae81b" },
                { "uz", "37cb5df587c70a71db639af23410bb7011417debb65eb127dd44a4d8af3c939b81eb69f5a38a4f50dfe624ef5316066c31da72e475610efbc115d7280a25dfb5" },
                { "vi", "fc00d7957114847561d1a7d661d59892ce3dcbb7d7282763bb1a5ebac34e98a055d4d6f0253f000f81972e93e38224224c4f801a7db72c0f1c7883d811c8fc24" },
                { "zh-CN", "d170fb0fd5383b71862f5cb7b179f9a33c6c16c8984020cb7e7babd511cde84ccff463cf27bc38fd328cb59ab4a682183d8a779242bb9ca5a28455fd30b789a7" },
                { "zh-TW", "d71f124ebc0dd2fa1e0ddabc7d8007bfbbd6ae0c88b23c0f731ecb518190c2c8d7e3f79b5620840615aed2fe28be0a3bb10aea525ba5db342139db67d366e319" }
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
            const string version = "102.3.2";
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30_000 ms / 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
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
                matchChecksum32Bit.Value.Substring(0, 128),
                matchChecksum64Bit.Value.Substring(0, 128)
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
