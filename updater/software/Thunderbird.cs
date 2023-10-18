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
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.3.3/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "ac1eb31901f167cb9fb898f41061f3c6e705778072ffc172eee8c59dcf26bdf082ac5a10a575f8733649ed101ec931fe519f98f5300f954defdd4e5ffc30d0ab" },
                { "ar", "f6471d2c48a2e2f94398bc92063ada156ac6a6525ad77bf69b159a3f28b32d13b54046264e63ec99740105133ae943e53f6d09bd3a2b30c6b809e9466f38ff10" },
                { "ast", "f1b8ad31be24a64effb7c77059144c5e7fa5f76d8322c26e3847fe2cefef76d4751e83eee068d8521641284ee872acfc3a35d52212e79a54ac8fe3d7e1b338b9" },
                { "be", "81227012a7bf64ab8aa41990dc09c2acdd15957db39e42a6ac101fc645439583f680c41a02ee973c3658874513d97a2066847b645c3d579cbddd921f8d333b83" },
                { "bg", "928e2b77529c0c45818be89b2fdb4fff11859eaa76f6b806786d86a8348332ce86c7b1259c9dd63530b5f882ce42baec7510a4c084661f5b762a7c09710510cf" },
                { "br", "8fc840d08f72f06bbe55fff0eb32a0b6889b116e6b9384d867bc27f1a2b93cb3f88ea2224855ffae960ab3afd06d3c8015366b63ed97d17f0b74c7bca1db4db6" },
                { "ca", "117a4bae8535ec7e633958d0240fb949381a64f0b19710905b227d480edc562040ac70601557c7b43c11b178f40dab65f2917463f0f3faca2c291a5617202d0b" },
                { "cak", "94ca82ea81e092c5fed07a7eda2f04c52840bacc16a40665d85ba22c66595a1c850419b4dde39ae17b39ca2db11a9f33aadb70b6b0a9147ec3b090efe926a77a" },
                { "cs", "27c02ec5f101e4ae334056368d5d05e6ea13fbea3cda2f19d61788a5001d46f83161398c135e5cb9bc34026fc18458a1c233ae23df9d3f32ca169f433d223143" },
                { "cy", "da8cc6ce25457caef1ced1b89b78613034d406cb2296bc4d21185ee05440d4c6b6b7a586ab9ce837497b548988b066066fd19d29d5d32275d2e117944be3d67d" },
                { "da", "e9ea3461faa337a656ced07a892414a617aacf89f5a5ec23e1da919997a7b47decd41412a509ad08d0e29342e4a641b20fc8d7002f20df0a18c2be7b1899cced" },
                { "de", "bd44401f45725dcb1446604dceb1316266ca33dbcc47987455759ee3966944f7075ff81ade96e7e48330b80282235bcf20b783515835de6390ad04f0ec8494ef" },
                { "dsb", "3057496c688f886e447e632d199458cb030d54ff65aa6550e329e5bb43155f378decf84fbe362ad1fb03cf9f4b2fc19348bd55a8531d15026dfea4f8f18707e0" },
                { "el", "f7eb287588beee021cff1b93a9a57899dbfa9726216bcd97495023a9fd09d3ad6b83fcba8373f5631081f7fd12a9259dcee0971677b8e94077bb73a2f96a41cc" },
                { "en-CA", "3540c0bfe7dda83702f368ed5908cb4a7790c2630c4bd0bd061d9f04d7d2685d7ba4cc0f11a4bad87c5195ee76d0ffb14b504e2f76722d9220df0442c80b72aa" },
                { "en-GB", "87f60b19215934794266133a082b911802e26a31e54deb8228bf0d4ae21e16a4673b99be0e15df5dc99f7ddd452ca3c03d29ab96651b5692f99c78102a154e1e" },
                { "en-US", "ca7be54419b2d1c797d9a059080bd66eac4b71223b79bce4243e29630ac023b04dd3e72320ff67cda2f7a55b475e988409c5b3f231619bcdab8ddcf2a4ec4a16" },
                { "es-AR", "4524965b478a6477c9496831aba31ee26014d2aedf76d84e20ecb793df0194344c0d28ec3f57c1c1c2c101cd1d0e97aefe59fbea3de80bc1f5faaf459ff59286" },
                { "es-ES", "66cccd6c2ac201a55a00a4fb0d670ed50cbbfeab34363017af565e9441f84eac452db4e59df8021d67b5102c301793ffbd37eef6871bda980ca27b6b38270bc3" },
                { "es-MX", "7c0d216696ee4dd910db799ade861d39f8ec2413dbc350f4399b70af7b338056cb0a7015938daadb0e67930b53ddb944eb3ebf94a2a974e71f6688b55be5de5f" },
                { "et", "1a831c1ef8a8b67dcc72ffaa179d994a29390dca31a02dfbfd7bcb9420fb798abe288b019b62d9d083d05a5a64d9af53bc65522cc24eabfae5d802a334926023" },
                { "eu", "85244add511d9ed816057ae9fc5f17c6ee46530a28b585122b8974cd8cc73dffb581853e8e2c9ffac5ad649d09f6f7efe0fd06dc123ae5d6e8fbc80504a18ee9" },
                { "fi", "e9e4f0cd10a94512ba91b2faa48db24653dd7aba4ff592671e5b56607ed26992108bdd714a53f75a67bfa0a4df204bea4f9d5892e09af0506d210469883049a6" },
                { "fr", "ce347d5c6fb637959448e939e3c6530fd1d7caae21df3d4a2e6501084c2cfc973cf32fdd1118c017365bbe86b65b62daf8e76d95da0ae50dad9f9a924dd4ca5d" },
                { "fy-NL", "1f463ea90d8b959c217a38bdad2501fc64f9d3b7ef253d6ab3c8d376bd3c64d3d02abb6f783314ee783c7694c6d3d4009f725767a5bea70e863528a44bca0b1e" },
                { "ga-IE", "c2f56b051e7fcfe837794b96f89b7bc82117fd13b8b8a530e54cdf3bf74ad7ecd7323498a52fc8948407cb67b5559cddefcdedbdb2b584857281822b18748c8f" },
                { "gd", "155a153c03613bbbc766380ac965b42db99fb4cb5a960d8b6fe7d28f7b19da0b78612836f6a916a6a1d5f77ac0499586481a7483d88d40328f4b859a1d559f9e" },
                { "gl", "cd82c97328694ef9044cb61f6ff3170dd1e781cd80e5df0dfe97c0afb22a893a2ab9046b7499a13f0f71cba4a7eda91d8ac0c1c3378ba86223854b13cad9d5d4" },
                { "he", "3662ae01062d98f458424713e49c3773d93b6bced44afeb7a9f5e9d7f6c1a5c5fec708e8f8bdfec24ba6d350eb4bcb5b7e931c6ab082a79dedbde1f39bea197d" },
                { "hr", "e6911e0690549faec054bde9a8021e7c504b4f41d132cdbfaa66688950056dd85c03fdf02921bc30573032532f1c5f83d3bb837b725c2c81ffff18050ff72230" },
                { "hsb", "3212c7123e1996da6c93384e8997fd929901f23026eaa5887ab10a0aa1ddf3ac2fd980ccfe858ca68d2d8ed8e02fce4d6edf1eb712400f5ff1de5810f1d802fd" },
                { "hu", "a87e6a1510aead61858426a2a761b2e9e856476ef4135a7445b4fe851a6829bcab5a9618df7e7a260fd8cdd629967fec9a24618c989ebb118dc991786e295f29" },
                { "hy-AM", "b8ee930d5e429c25bfd2895ddbb714a1dad1dbae731cd7847372f0bcfd76037be928923e8191851a7c8194cd27437636e64287d7eb966968d916dbadb9b2f26e" },
                { "id", "2713a82481440858c52f4df39afee1db166aceb836484771dc4c11d27ef62abc77dc9981674e5549a205f222c74cd61d097050c72e412c405399df4e1a0baf28" },
                { "is", "c321f4e4b1885203e28fa444503ed7871c7c6171ecb01a2bb754a6ff860207ba152e485ad75694b0513a5e637d9e781c8352494c71b09b4492fa23234ce5b8a2" },
                { "it", "db272fa1fc9b07b396a8f573be61bcf0840db4c6c05e56785b7062e7931a42963a39a74594649f2346284e3122abb84e7c504fbade75007571abe9a1fb3f2cfb" },
                { "ja", "fce1701f013d7eaf6c9dfa50af4c21373d91996bcf25b8c30ad3d18973de8dc732ee530c19335e651933cd4aaf6f36c4e360137a9bdb611f66edf953124ea2a4" },
                { "ka", "759076c55e487a53c6a791cc664a70fbf8d4615de395a494222ce89ce5e35f3b6ceb67782a73318e51afe023a20973fcbe36a116a8f6b5f896c40475b3c44afe" },
                { "kab", "316e917f50e4b2bc194b330ba50b7f3a5ffbb58a20f25808edd99fcd30c571413d0f736587c6740eb8da9184232b43ebbf0fa0c48987d497528961a63961a858" },
                { "kk", "d793f8d7ebb3b18b84cd7f4800f3e704649bbb83701d4a75fb2ffde66d7e1392697aa2dd40f07b389d3a9f553c834f995298d75846aed812e3720c1d02e29a0a" },
                { "ko", "7ec535c36f4574d2c05eddccaa47d9147cf22547db9e417bca3a8521ed60dc64b339c4653455aea606f550f8619842458424a30d517406584dc9b8b4c7f92494" },
                { "lt", "0232855722666c5a128d77b36cb363c9b2e79cd672a3f809efd4f3d013f8ea0d3002bca763d25468967cca627feae95962c40dcbe9307cb40a06e7df9ee5d6bc" },
                { "lv", "06ba5bbd4abaa5486f1a3e5a6e98916ecd2d13f5fde42cfa39b2f94ce37e1a7d4e39d503e1076f312bcf31d08a6f896c27e4bcb9cf2c99711cfd9f3c2d5447f8" },
                { "ms", "5aff57fc3135c46c143fe66d2101c65b751570a2119db1d40d7bbc516e9f0d53b313f515d01d1a9b9661143f9d6de69978eca2f57143309fafbbccbe96c1bd24" },
                { "nb-NO", "b1d2b3ab9a4c0c3b677938ea70407482e0044aa76fc5cdbcd76882724b59d5475ebaab4a4c678c5f13e68f6d14f2097539913fb528684e041951d63f1afc5848" },
                { "nl", "4763f8fda4d301190628f2234f34ad76ddf5d189d258a9886180ad21ae1ec82e8cede480262e5399646dd77c143ef1efcea19a24385e99094b3fbb83fc928122" },
                { "nn-NO", "b40e6e8d48a7b7fe6319a0626b1a3e679208e52bc405f34958eac45de3327a59d22af95e8bef95e9f4f18d2ee96a442ac1dd3306ca5d66800233ce3be2f33cd1" },
                { "pa-IN", "f828ea9cb0698d0c0d4e0e632f7b33d79ff9ce4315d81888b702c5e065430664653e4c1d698bc5ea473b2da19b558cb2cd3c43f1fa5aaa161bcfa5a09864949d" },
                { "pl", "5c45b7c3b019a354ea124f147bc1b030400dfc865802a4f97eb5547cfc555716e3e1077bdc2b78491cbb8d956bbe55f7dc976ea78a02dd1e56c86b1e1e03f519" },
                { "pt-BR", "0bab5cf748b7aabac5f8abbeb951ded8802fcb60dfa3e000733dc1bd3df52500e8ab5e6ab993e8664577e2b681e235db4349f095fb92a28b28b1407d63bda52c" },
                { "pt-PT", "ee49d65f00e4fcdbe05c78d4cb9d9ee09461e03e4fdccc3a1f618956479f277cbe870042a27909441b4f01dab3e35f6d32e5b53d3ba51788a52b7c57f84594d2" },
                { "rm", "2d13e913494f6537fb6d14bc0273b80b2aceed87512e006291f673bdb11a8256acb5906432fa72c49122d3075b24fc12fef6ba11ec15b3a8007533782bfec7ee" },
                { "ro", "3f5845c703a51590a0ca5ab0c0a1f19a066df4a5eff3d47bd93d963673e76a2339bb24fd92c825ea8f99a82b5a8a031e0d4833dfaeb7ce2acc7ab549b018f8dd" },
                { "ru", "cc4c0050e1bc318162689b71c489fb49360f1da9d356117b806634b3f49338b006da7e114f5924e0abe4ccdb1d771646552be78cfa504eef0fd055d47c18ae77" },
                { "sk", "fb8c878811840dff6d6d450ac95859e36c8ac3c77304c73acadaac4a40aa11c8a190b47aeb39de77917bbc29b06a65ae54595e9511426d1c83b865ada454f293" },
                { "sl", "cb08fdf43d35b644d52881ae8a2db3dd2d071f7ad5cd173d12c9e19f974393493d346fb50c638e9d9acff8154fd918aca8dc61db8fe94b99120fca40896bece9" },
                { "sq", "b560e7afcfe555e307a827f4bf04c183a3cd7c28809f651a6062cc975dc8c5efc3173f6ca24df88db9e7d61428a863ebf9745fac1f0ba4a910ca03f04d0351ac" },
                { "sr", "2158fef2bc2ed3b8ee0f4a1d105e7ef08d517b5e05d6e2e61721fa85b5a9ebb7a670ab870cbcd10366213b6b921086ef19a5c02993a59e07f919ac256ac5d423" },
                { "sv-SE", "83bbd10a34a955556c160622e4a85068b5510a356b4b37fcf7b87112a2fb0da641d518f8b21e0b43ec721659eaf5d323c6265917654c688a4e3bee0271299eae" },
                { "th", "f727dc5fe77c15a1de61afba41dbae74be63f6e4ac81cd7acb47bb29d54eeeea1c8fb1caaef240d703233222ef8a44da77818afab6cffe6cd1b72a1f3ad05726" },
                { "tr", "632ee129782a8b6d3a3fe8fdd332a405d0bd9a70982a50b9fe175b729ec2a854cb3e943df5ba6b9e7890fb8f6fad52347f00ac1aa0720bf207af712fce369547" },
                { "uk", "776c3510faddcef4c9d621db8ce12c6ea33802113c09efaa3ecc83251bf7c68868d4a234bc4b90c6497590a2daf459c71b32ee882328bf37874ed4f2e071bef3" },
                { "uz", "9e7264f11a9e54dcea90cdf08f351718de6ac277d83072c1ef8b9a639c1eae75f57cac33606d55d52de8c64e44a91a57843069f65930acc968c8fb1d77f37365" },
                { "vi", "296edca009ae440eec6246040124bb308f370d9d2239217f0971232bd6380b45b8c2ff879180684409aa97f67a89b37f7cc06266d5ca3a193a9e63e9d9c14cdd" },
                { "zh-CN", "0441720303a708d3f2ecf54b9e142ac4f6e383029a35ecf22311d7820d46c0383f11172470896439fe82fa27355ab3bf2527cfacd29b3b88cb4ba86d86b315a1" },
                { "zh-TW", "c4813528fa08544eebb3aa0e86cf75e0c954b35a8fc4df97fd9d0d2730851a3573233a25b78072e13fc00ef29b06126baa475a7058dbde1f961d69eec187ce3c" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.3.3/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "6f1198970f5e5ae4b0cdc9cbfc52b1ffe7e795173c54fdbcfc00624d19cbed429d631cdaa326a04f28690d53b97a79bd24d45426d13c25dd2c25a6019b7b3178" },
                { "ar", "bddd79bffc2b20a75c31fe5e527037897db4fe82d91592fc9dc637442ff339830fec2b1078265ba2c39806fd0b0f70dcf6c3e611f6a03e233332c6732c47a1d3" },
                { "ast", "40ea2caf3e1f3174672e7f6182d155352861bb8e62fcfaabe9f055f5b6e9d889222483ce13cebaae9f73a9972105586d6cc07dfb28c4d508712576b6eef9befc" },
                { "be", "fc11b266e9624d2971fe22dadee08967fd52cb88093a4c6626aa2de4f96f1b12ced61153c9f3102ad41657683ce3821a51e6daf604db754362f1385768686435" },
                { "bg", "0c39d3e101a2dbe0fb4aa1a575b68e3bdd0e0c15d604d7a451bdef24b1df1385ca7c0b0d66f7c9231c63081efcb13425d9d803b3027ac56a04228ba220b1ca41" },
                { "br", "43579f4d8ba53d616514ce44c62a10ced60f9f8d3a9ea68b14909ea8f4e26d58b2374ff38ec7fa5fd1936eaa693d7d67db7bcebcd46ea7f725b99d4c236b24ba" },
                { "ca", "726227c8e873b29cd564db62a3c92da4b96e44c2448a1eaefa399b1fed361cf4fcb46389be3d509acd1ebf346c18e0bb35671df4d6d3ba33bbae27040145df17" },
                { "cak", "e9dcb75a3c917432c7f8e8c6496715c17fdf8505a6b788048a34e3b10874d81cb77ded3eb1daf5858001d2fee7d07bd80c2a7586abdce2d9adc48d226c900e45" },
                { "cs", "a50b269c1bd623b4f3546f77d6f2414adf2458a38818c83bcc3528e4444a2a952571f3694d09165c56fda01b3c54e8983c9a9cc70a353fb1f0a7fe0c2f641626" },
                { "cy", "e1960f08136a5660012960fdc0e2025831a101bd31b8043ce44b5d7e23207a10b75fdb629c9c64ea51005c910f9e000f54a1eb42bf638444fec4caa2e1c087e4" },
                { "da", "a97374ba645350e72353044e23e50306577e9badc13c915992a87e374c7fdbbb4f4a98064aeee204b6c98a727da4f0b092fd81e8fcb34a17666bdd24a2e1bd65" },
                { "de", "29298e22550f2b12af675486c6be06cf74939f61f4ef70d2a6e00d2168c874f5c043b3f8ead314d20b704f4eb387fe2c795aaf5138841d94aa19f9897d3d2e16" },
                { "dsb", "c17a26de72ec5adb0ad16fdc21172d090c6451743bbc3173bab4549603dfe7993756fca23ca1b9d00da2e8c03cd980f9975fc38f75a59d4305c376c4c2adbdd5" },
                { "el", "3b9d16376f665f19363cfad23aa5b6916c5f9cf672ab45d719518df8d2eccece49350a66a8c4269c7de10b63b2ecafda005502bd1a77e7a358988dc29cffd61d" },
                { "en-CA", "f93c84fe3775ab855f264e22bcb0f3bc9220f30b840f290bf068b7729d97b27822f28581a872194ac2dcc5dc84671b2f2fab20e6678b96abbebe36f0b47ea0ac" },
                { "en-GB", "b8654d30e72ab23739b9547b5d4bd65e4f8980a4abbdba0279bbf5c44867404c2afc8bcfb4fec7c4514e79a76cba9eaf8f3559b6443073aba16cac18187f9fce" },
                { "en-US", "3c183f287963b3996eb01a850f1aceb56c80614422f5ccb3cbb85793336fec1e9da96ec39a0213b6fc76a6fe373ec79a707c39ed1e194674736edf5a808bfc95" },
                { "es-AR", "ee0afc73bef578c089817f37e2f31a09d90180efa21b9a3ed71bad9cb77c21332702fb7d5dc991c10182ccc0d2b7dc1c2947a53461bee6696e67f925fc0bdc86" },
                { "es-ES", "753f8ac8fe78ab3defe8a4f92c0c043728c9ce1f3d688928ac720e4d1b9a1cd6328cffcf0d40a9c6991153bd60bea53cd252f5866ddf01fdc3a015f4bf96f9ed" },
                { "es-MX", "8e376d9ace8eb2623e05a1c4d7d25a81036728cecc3101fccb27f51951551f8b53c1ba5b914efd00244b2c7eb7d81143b1a7ad100bd35c39adffc99d003a4f16" },
                { "et", "3d95021426c8da8f28a9b4dde4903333677888ee76fbe8843c80ed410f4cc5bbfb238f568b548ebb70e7e7355468ba2983f32ce656d5b4b864b16b3c9bd0bb83" },
                { "eu", "46c3bb2c5f07c4b523de7cd5111ea384cb83652fe6301b88acf8de8aaf87603edda623f81c04cc09a4b9adc4e57039bd535587fedaea7e626f0d3b9409385f8b" },
                { "fi", "b2f8737c3857458bd1f5d7e9b990d2d72cdb87eb3e048ab5eb9d07cdef18208c6c35134d6c0cdf70f9145db946974abaa2ad63ba0152c74d074f9d8a87d53e43" },
                { "fr", "4d3294e4de607bf63724f4f07fa18378202d132dbfd65575446539e640e67b47d836256e2e05ed2987b4cde41e82f9a0cb8222b2ba380d366f2622bc6cebaa68" },
                { "fy-NL", "2240b10c93bf4aba4f69c9912098a33b08a6384aa7f6b857a70f92113e7d1592643ed9436042b6b5bbbf26ee1d29cc977ca1b33207f4cb3a7b3ffd2b56575818" },
                { "ga-IE", "416b8e237d08f7ca032e31a8211a0d1d1a1e59b0c882e48b5d19e2f17fd30bb4c5daf3ac92a7b299c15dbf5eb27d18e8bce5b8c47682a6b67028c70a0a6ee76e" },
                { "gd", "985273f99ace0c59c2b2b098616a005fc1eb21ccd1de9ab39b3400dc5c68a30831b480f36ee83448c5095c3207e9e6dabef0c0b4563788b9ec9431c398f1dcd5" },
                { "gl", "950f36a1de95e1024c7077a5d2d0e472a275752a861897cb201505afad478a55c313b672a181b7a147e896106257edb39a3918b8f51d67f2aa60e2ced4aac36d" },
                { "he", "4fa51e1dfe9676d49e126e9c835c0aa3eaada31fa89a49130f8c24850a89ae503199a9f5b19cb7c973eabf0c265a969de540e7a2e13b71fbe4cda62ba605421b" },
                { "hr", "5d22d82837c76b0ed7fb0b7fe62c686951134efcfae90966a5bdf7c0ee0bd6f8a76e0cfa750032e1e3860055e9e19c32e5327213701b5699eb09aa7b0bc9bb7b" },
                { "hsb", "835a9c4d55505d6233f98647f7f3460ff370e785e2a7d8852de4e32e38de660937966405ca101fbdf19f2c496002cd1872e9c7849364973d0050cfd2318e385e" },
                { "hu", "f57d17f9e5a631c37b490065ce6754bb06bf66801fa1aaf0ddc1ecbd1fff472c878f1eca7375fa62d082e68c888fa6d5a67e05808e48011c229169bb53ea9a20" },
                { "hy-AM", "b40761276dcf7bab0eaa477efe8781563b2beb28d0255087f99b8ec08073096f0c20da613fd7cd1569e388184febc5795f60eb8b657e5b055fb67f650e98aa28" },
                { "id", "23643f1e1a8fa2eefb3f26b31e69b12131044cbb69e1271d33ad96f94bfb237a73f95c3eb1933a7e1e7bffb9889dd7af0315192e2e3efde825c5ace0ae117559" },
                { "is", "13dcd02666a6798551a9e66b40639529b85ba6f8efd3a91af47e8ff214a433aeef9f43347592b69988aeccae7cc42c7ec678b17547f1b7073cfa6294f4debb7b" },
                { "it", "d4246e10c6fdcf43fc85e263fa751af8ff9fc4604ea86c4790c6eee78cbe99ef19964b167b1ed7d2ef1d4edc44a2892ab8251931faf545b701abaef7280bac12" },
                { "ja", "5db060ce9500b7afe765a65b389ea3ed2f37686e9c431cf8aa1482ec106362c4d6f36cfc443f92c5bf199211f6dfdab5bd17e890c256c2aa69967fee06c1b94c" },
                { "ka", "91011409a737e14316712ca6bcc36293d0711ea184a8178aca9a38018af27a3bd20c39080e0986165fbf5affab4e5f2d63a2bad0dfe6c5eca91e8ecb82ad583c" },
                { "kab", "75f65c087e2f4e830430dfada45862b64d92a84e42b145b8fe2d7bab3678e6609dcdca50098ed578104502f18c2a60f9f50b01af11023011919d75c6dc4ff96f" },
                { "kk", "20f978556f213663c1d0cc5b2e661a6e76653f8301dfd3d8f3388536f94c318235770c591e9c1865cf2892413df4e532f5ea0daadb7fe5aa2ad450f3ee2afefb" },
                { "ko", "9be2b9c5c126993a08770a249991bf3a5de4360ade66e5d725490a7f336ec0a4d9d7a148dfacd6e408f4f617f04ce4624732c5d434ea3694dab34c5f684f1027" },
                { "lt", "8a0becf7e0f0875d4dfb7c58bf155321f729db82bda4977462f22f3cad1938cbfa72168b193a895c3a20e59ac2a6854acd4ac101b8ffe1e9d18161b09e0a2c74" },
                { "lv", "b33aa9c15673e3bb0ccba87859f61cfe20ab92933598f932af792ebe2aaeb9bcf062632587d1ccb6348e6407b21aa870b4ff6fe2fce040e0f7c55c05ad3894ae" },
                { "ms", "a74ed4f89e126091bcb5ef336b04c7af412d639fc378560452eae32e38efd52d2be94de5020f6989f341976e2bedb4aec88189eb4c2ca76008feee85ab043eea" },
                { "nb-NO", "7ec625146f705c68c1d47e621f3dff6c15d6968b64467551dfea643e970ac6803e7f4455e500b49b720d807da174dd840b80a7ce0e2e6a702632201a9b809189" },
                { "nl", "5e67a5de65e9c5c43d295400b8614aaf23c797ef27b9abc39ba93ddd7461e5e81c4acf223dce14607f62cc271ce3425dbe128d1cc0a6ccdf65c0041fc2b625f0" },
                { "nn-NO", "8c9a5fc1d5718c921401f8039ce92499301973e035913b69b3eb2da1dcbf83aa50a549163d5a7839294d54d548806a73afb0f30f6a1cf0773c9abcce02887336" },
                { "pa-IN", "8b4ccdb54bb5edc5ab192d1f9cada0a50587512f200c45fcc07900407e4eec036483821955b1916f213512aeabb946c0453596e7e99f01a854d71a4a592ed5ca" },
                { "pl", "413725e06b978d28df93580601eea4c556a4125ef6e8c880b184c994d1a1f4b1c404986d26f3c4b499b2dc779cf3fb39d2c03a8250ca978e9cdd59aaebd274f9" },
                { "pt-BR", "b6dcbd6fbda9f568e1916ab74606809d49bf11001683745bfe06028d85b61b7f0a4408c8bbc3d71a166d338ad85d973c2b72fbdf9587e4fd90b722f732d642e0" },
                { "pt-PT", "98a1fa2b519ab30ade7ec1b743d654f8c8970f5d075b3ef96343f266e939003db2f38cbbe90b1fb8a8f24609539166b0f80be7a2eaf75d0dac155b7f229a4851" },
                { "rm", "13d9d2ebc81f96c0fda166fa522fed0a6dca231699073b9225f0dee4df2e0e65aeb0bdc3e667603fe2af8915ff9382ba18b62693e9ec285800b96e00789e5539" },
                { "ro", "bd924276c95ac1bfbbe23c674daf4b611c7daae1f9616d338a5a7288621d6ec1aa550f57b615d4ad2d5295c9f969c34ecdf488004fc331eace7fabe66f5f30d1" },
                { "ru", "119f7fc814b133f88f7a93ce733b09591619c11844efebb676728a275ad2b7dcef45a2e17e70de4702f5f1dc6a3c819fde17d7eeddcd06b1d25dbbe29f17fc3b" },
                { "sk", "9c69820bb3a19032b14bdd2e6b6340f023aac88763cc203a1d0e955ac660134c2dad60ba8593ed27503986818173d0ee262d2102cf6a6a5a258b53be2a2c2c8d" },
                { "sl", "0f5ed27198d185a154eeb8ac4c84a2fb6303669a2247499f25c0a7bcc69f5db303c8dd6ce4c7cb419b1e25e853abcf364bcae271b21ca6d86869b97b5a2ae8be" },
                { "sq", "6fc40dc8dae333160d215faeccc9bf478278eec625324c43429645a8a16e39215e6c67c43b3e30a6b7a8760f16b6727f24c944c989b909b9cda32a73e5579691" },
                { "sr", "543083476eaa4302b41b0047e67a33053b95c0ad59ba9ff494f65cb10281d5c2ce004fb0dad8912d5228993bfbe7f9b779df84fbab70a6f3bd256877cbf774e4" },
                { "sv-SE", "3337b56e455faadd725a60d36bac29eaff81e9ad05fb49b1097c7f1c7c58c5240fc6205c642fc590d8260ea56bc1e64472931c3ec89e29a1dc39fe52ab323476" },
                { "th", "8f18b020264c40f93fc21e163ed630321c507f69d0037438416ab421893855d3266bbaaddc0b62f2bcffae126ef05d63f1ce76762d35004ae1fdd8add579883e" },
                { "tr", "725e87136ca8610989772ee2141fd5da7832bf0cff050610741a1cb65e67ad01dd5f7fac3655fd6e2fce9213cbc366e3aa64edfb186ee04e255bd88ecdddd5c3" },
                { "uk", "29d468d2053abc6cb9b5cb4497e88a30ee79ed5096da666109770f5ab71c586c70c2bcfc97188eeb6b91272e69131897dbaeedfefb95fcd1e548e0f05a6690cb" },
                { "uz", "8d61194ffbbc59cd09069579a467f016fe544639a7407c10e69047665b74fed433501804b903c8eb60dcdb9584635f89b0ab7a15d4c767db4c969c1b97c82882" },
                { "vi", "556f39a64eaeea4648cd13e5d955a85fd28e107889684b889fbe9338907d9c016cdb25f932d87fb2dc6706ecf9fd1af08b712f27d956286b69235e3870b7e9b0" },
                { "zh-CN", "abebdf26592d8167f33cd47c0dc17d5e2c19495757a298b558e8c42adad8810a3bdf2da6697a94e66dd114db4510858bdd710543b32fec46f8777c6e7f3ce8f4" },
                { "zh-TW", "ec9c3b41c92ebd1a88bfa777938bf97011e7308863eeae29cd9729a1744b7a5863b6b28d1f040eb880fdb6b8f3c63981bc4a7e3e0dfb7020e48b60b8f94760e1" }
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
            const string version = "115.3.3";
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
