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
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


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
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/thunderbird/releases/91.6.2/SHA512SUMS
            return new Dictionary<string, string>(65)
            {
                { "af", "a972e8be2b533c8b92ddcc54b6a11ec475f726aabfbeb6afd04f9bab3962ef4d30dde621d7962735752cb0b6b23d2fe79115a73f15a7f76270178dc51f18e3ab" },
                { "ar", "a1e21aabc86a7a02e6528a92714141778187130091b1d28ace802decc071876e7d356bcb79c887bb6c0fb6a41109fb5cf3cf1efbffe24940c7a4124078a08d63" },
                { "ast", "84041affde7c4678eba11d861689ef9d8c20f8230ae921d50913870c74f2452c4ae85d24c8b8323ca166677073a54f7914f5937eeba2d04d11d923ba0d121f6c" },
                { "be", "146032dd704045f3bec03c37a76ae1f845b478a6d5c7667e97d1083ae81c041d93b605c86cef4922a93d7f88774aaba4745cc4ff7674a71f24d56b7090424ec8" },
                { "bg", "5dc5b5abfd12ebb1baa4886e188c9abfd9785b1ced8327f93e3ec164dd111cc3936fdfd1b31aee74af0fb3a6831cb477d9461f137b46a91629571cfe1ffb1aa4" },
                { "br", "a4dbf2b149e5eb71565395119ce931442b1a48a9927b053004d5ee55c319d0c46ff76de4cdd5326fdf90b9401397dc30a97216a154892f2d19cd072ec06bd534" },
                { "ca", "77ece2a85b1a50f962d314fb691bdcd29e576302e2a745b649ddc63b89396edb219ef58c0a43d5b8d51af38be3837ec772dac5e6334e6c14ac1a8088cc293806" },
                { "cak", "980f069162dfa6e373c819f2b5fb0bd60fc1903289352ff7ec195c91c5bbcb72767277094d0814d4090e726f0b3118876bd3d574c377e3b7cfa603e0af16a9f4" },
                { "cs", "f167c2dc7161e653875131b2722a20bab9a2a6171f3ddaed3d2c355bf433dcb5a1a93706333d72d803f258b7f11fdea27cb3931090f77e373902f9c57a78aebb" },
                { "cy", "06af2e0dab58588439ef07ab967a130c19131bc5be8624e4e4eee013338819191e9f1a5d99e77d17b237c53b851f915358a2cb53a2b81c472e7978e29ad4d78e" },
                { "da", "326671a62bb315a0f0696d34bf8395597c747b3a1c99efdab7b6f1c2b760310ba69447cbd99b4eae3e677a933c5121aa76dca13003a9dd96dd930db74c4a133e" },
                { "de", "29969d86d67cfcaa065614eeeb21eddb3ec4db8625b82f31785e3b553b7d9c98875a9e7cdd2ebb1f785cd380e2d2049ffc95d353779e42d2e8d388cd4067f063" },
                { "dsb", "bad9e58810825d5c61dbd729f27c18b065e2c4047ec31e9585b22a95079aff0ca5a6bc763efc742afd1fd91c2e758cbaf60bfc549be8af449f554e6708a43aff" },
                { "el", "fe421544661e93ccd9e8963399f41e2662df778ceeeb24798c1324bb5df0d130066644c76e35b70b6778094fdd9ffd49cbaa8cf32a21f0ba950fc3e070fcde76" },
                { "en-CA", "3492609ddfa9b1d5c2222bb935082d37c9f5c42e590e7d68e0d252eb7dfba8d31389a0ec714d011107299ce3988c7c0b6fe900fb44280833444f2b7510fa108b" },
                { "en-GB", "db7f54e9bfce671b7460e6f23466cced6de2895a6db2dbffc8dec5d209880d813831ed6931dcf4ff87d75408aa8125c48ab1b28a38e4072a2e28509c2a237752" },
                { "en-US", "e08ce33d6bb980c6b3a264b12210008fbe63152beb66cea3a2bdd7f889df7ab730cf9d3f26394d751b0226e774175d33a26422bfe5ed950d4820192270f53e9d" },
                { "es-AR", "1a4ea06a79d77b27e455b8ab68c656a94698cb057d69703a9b78dfe8c01b3fa7424454181ae31a99116d764b4a4a0c476a3c87e46729a7f060c4eb5e0a684fba" },
                { "es-ES", "d5780c971001fd5c81c7048d87906be85b992e066753303bf18f84595931d929569022f2ba3f2cb912656a1fb67f0dca433fba7a7b87f48820f03fa466c6a6bf" },
                { "et", "078d2e8370ed69f8d0caad674501b2719b08e32dbfec3ae258212a4f8acc8edd22d019ca3f605d4c4ecee9fe6d985c21965a06eb671e3d43eb7831a4cc980513" },
                { "eu", "62538f0c5055437b8a08714201ff95c9787daac30cbb8ab670812adb102ab4d41fb0b2fadad59fa6f1b9d8c51ff45152bb4c0571fd706dbb999f1b625f793c4e" },
                { "fi", "ba570391b9c316d463d875ef30312497be1959b7d48f9ea17807bb5e479fa9af710eac69cc5fa75d763ca305cc3f708880ccc0e8525aced593d1131c4a2cd8cb" },
                { "fr", "b327078116754de7517f1a6a60807cd51185ce5ced2e32c0efd53fbf713c999ab57b131e16fd166a64812acd826571924233de9b683d794b67966ce54da4bac1" },
                { "fy-NL", "de2c454ade8043833d56dfcd4ea4b372a818c424888d3041b8b38bdad13483232c76f7f1dc85767ff161efcded9ef78b2bc2f9b8fd96efe073528de15c769ffa" },
                { "ga-IE", "b4214c6c5e12468c6345459d4a4897b38d5c0a095d16144f98aa6e20b5169e3dae55f2206ca8ea45f6496b4ea58b11a7512d0062d9a06a183ab213e54d151306" },
                { "gd", "87f118ca981877482ab0a3267c2f3348dd6557faeee838f7b7c5543b8e6ba129f0d19ed748dabf03c82c8cb2cf065a42fe82fad0aa76af57f08dcbe3a500f584" },
                { "gl", "4140b1fbac44a70e381cee9a9c4f4e9b2fddf5d5187e648e4dfc567c681b6b206d40007f25e591727f27d184e2814dba52aedcc92aaed6c8d359c2737d80e346" },
                { "he", "b97b63851391fe7013031a9f789f1d5f3f2365eecc8d7ede33636064e748ec297fc92beaa8f30d2d43bd22b57cbcffe117e3a791f09a2bee4161e1b3813709ce" },
                { "hr", "62cbcb8c272014592719b5981cada4d3769465fca0ceb8760b7d5c559ff0606f06a1eded903cb68ba00347132ea07e9493c30e68b90be3375c247c896c7a8c3b" },
                { "hsb", "2b87abf3cbf1693b493d43725b0c444076671bd76c34cd4007f3a8612c6dcb78b9583b4452da3fe79e2a39cd6f1c1cecce31ca4cbb5c64e79de3c810da520500" },
                { "hu", "845c0a114222e06147a835208365acb808961afc6451e4d4ec718ebe67f19a442bcbc0de0cbe301f497b5384a2d04ec0a085e53b4b9dc306d8141e6cc82d2504" },
                { "hy-AM", "beda1d60ec6254fd09f1cc1dd82062c976f6077d9717f90ff5d5c65f69fc6e26906584f3fa3c74bc2deb03da562766015475f38b81fa060f518442880b76f57f" },
                { "id", "c72b6a98d43ec00b889a7a160b21313568e6f2655c0baa4feee8a7989a26bc8020836c612622a42daabda25cecdb0bdd0ea2af2af55152cedb4fd2e42f3ee070" },
                { "is", "47c51e0eb63ce1900424f8c7726de497f06f18e4a25715808791d5d27f60507c372016db284b8c71b0b98b6987b9b4da51a97941dae520ccb3d38f6be2620436" },
                { "it", "1f6fb2b183d0c8966cf59e3bb3af0d1f6311192cc4dacead208519b90e9770cc69b422d38d31c6c1715984dec1dbf51fadd02c4239ee9bf0ccca90843e82e17a" },
                { "ja", "3f485e8ad9df52e51a12ce16d1bd195d71cf422263e69ff3123833f90bd9f1d8d5edf4681a96cf863c21d7961bbdcb32c7efc7e96a7d77f147f8088bca13d7a1" },
                { "ka", "a13f9305149c3ffe68a547e3430de167e12dd839c66a82d8c8bf8a38d1b4e322cee0fd49ea2ea0b1c5ba5c5c86f368df07ca42edccc8eacfb965aa597de83480" },
                { "kab", "553fbc4200d1009f060b522b3b3105ab1313e71561a956a2a573731a5fe68d1d5e6315f2ad07e2cca311ea1d5a8cebaf141242b161711c8150f2cd56b64c19b0" },
                { "kk", "33f833c9134cedf2d0643601e5c26e43bd50d6c3a3a444de9be976f30866ee3300bfd9443c71fdbe7aa276a01b8be94144d22a3f09f6919c11ed17db02045552" },
                { "ko", "e4eb12d4d1d7e815fd6d8ab673a299e349c2ba38f3ea260251af3ff61dea8684769863da825ce07f64749a7833fd6a5a46a2b7d26579f4bff55d75803ef34a08" },
                { "lt", "386f7a888fb6e33bb6f232d95931e7c4a619aa8abfc96eeeb8448b1fb0e5fa9c4d4c161447856c708e07cc598b0a43309f74b836f055bdcbc685adf767b13a81" },
                { "lv", "4c8352926d51f55d4149082baaf646b2461b1d803008fff3bc73630e17f486d1777c9a8ee9d7d88006a2dc898c63c0f92d6170ce63e47fffa9d7a700825e523b" },
                { "ms", "a8bdc7f30a57f2c258c4111454b9e62aa266dcdb6aa5c6afcc5f08f922f355ba222d5d5571942a33285c0869e295869492f73388b75261b684f9f076493439c8" },
                { "nb-NO", "cef47f1bef4b994a38db34978269916df0537b44c5f2228e0e8accfb94250b7427ac79f43290d3a877a54ef25aec9883fd98d8205e48738c101b13c32079ff2a" },
                { "nl", "e2da4da2b7411bfe7a495c4869167b41e715f8450907513a510994a083d955b555a2438858303d1808ceaf9513c53db29df24f01c369a434998f65dd8838da14" },
                { "nn-NO", "fc383c86c5b3d69d477eecf8e4451871d53ec0d458b9f692a5c69f0879e44daed93e5ba9b6193f9251e32d5f795de8813386c6222293d2dedbc67e3d7157ae25" },
                { "pa-IN", "d9d4a3540b11e11a1634bfcd1d87e55d595a50ec230c61de62ae977078241be409f09fd78c54e244576524fdffda0df31320367af3567f2bf327cc3c12ebb78a" },
                { "pl", "3ff297757b357b3b3a13a4f57a1a32c0a193cacf33f078e4754ac8e64b06ddc5c8fa767a959ad998ba78c12eb127b7b589df6bc16acd683da09ca9b5b7bd745b" },
                { "pt-BR", "c02e88e66c182b0107eb130a28bb50037d4dd3713cba0b503f22feb459a5a55db760e0c5710c84ea8f81144244c46051d87b2fb7bc82cdc8d947d1ec590484e5" },
                { "pt-PT", "2f53be0775db4bcf76145970a9d04fc944831f566502dd55b9a08fdeb8497a0bc573c045914482b6b9f76e683aa9cde01a4218ea9ff58394c4f7fb8d3cfac964" },
                { "rm", "300be4eb3bb6b93f4ac582a99e5b4cf2761af3bc45e21d9c7471e6f33419d4626a1150fb9b323cbb04c2cde0aa15aa82040845a27c0d7eb133fa7abc49bd8b63" },
                { "ro", "f2310f15ad76c7bdbf54693472e1d18e43c2948309b7f1ce41993175c2b62712711f9422c11bcfbb6a5e8ccb93278a04f08dc6c0d0e6ac6613607f9afa1f46ff" },
                { "ru", "04696681185b6980188d2c16e1387e658adf073f1e03b12dafd61c57244cae7f1a0fc09db6a8fbd4022e217f4a71c7822e76b2627e9d16a4b0462635cbd55fd1" },
                { "sk", "4225e2d677a3c622f2fe9502d2f2bffe66c38af63778273cf28b746bed4fb5d2b745da992ea23862f4104a90cd6a1a422eafb13add94f4faa1fd8d76fff9bb99" },
                { "sl", "af2c1000be4d959e9c7fc98be3bb5ea840e7b2fd46b377c8772eb4af3066ab8516e5c9ce7801be25f1aba14b52f28f8e5e94be6006e138ecddab7663164e7962" },
                { "sq", "53464c9b2f91ff040a09af58d79b078816d0956378e0e1756f17dabc174280770284d3f3ad203c8b9d1b23b40fe9552ccafe0b130120ee527a17067aa04586fa" },
                { "sr", "04c30e5da6c72726db7b53f56d8bb612dfe0cfbe55108cee50572a5b276d297984e9967e79a554c70e8e9a0952b23c05e2d05af7b2358314be0d90a8a0ab9910" },
                { "sv-SE", "1ab7d35b61f9ee3af8c21c143ba00e7d85274426673b19ceb98f7ea2eef28912526899859d78f8b9f9931e1a330e98b4d2fa4c2a3e42d0f1864f395ee957e10c" },
                { "th", "5aa432470f8961193d993e40a28704b577cc055a22cb05751cb7e2fbad0ba1b5b8f7edc1010b63cb48b7be0101be105f8c250bddab931bd7d0b9ff2a96a83382" },
                { "tr", "7c52d447f2204451806ee51d374412530e18af437e33c78d819e8d8757f49de7b1836081cd688ed07c263fba004adce1e0b95153262af0161f84121d29819ce3" },
                { "uk", "6ded2ca6f8111881c193d77174c24dd18975d9de9eb9b898e7839932a7d7e98742f5118c3633ba021da6c56cb6544648acad36a0b707c95cb293505d23c1e332" },
                { "uz", "4c40e04e464726eb0f189c976fd841bf757bd60c519af7044640ce25074fdacd52f89b549211ed61035fc02bcc62997d874e977026cebafbd327c1d8b203c0b6" },
                { "vi", "fea2a8ab2ef7dbf22db2d9291f2af876e044e3cb45fdc248728e5eb8006b254b954d011a3bc5a2f9bd5907164266dc3f340c418f9aba0efe0a8ca6ff6b560d34" },
                { "zh-CN", "487b269ed5b4830862f00a4ec3175f036152a611671a6785292fe1a5697a3736221e3ac663a6d1daba9320edb0a27653e232f71cadd6608e13cebfaf83811c15" },
                { "zh-TW", "65794e525eda7fcd8e8d11514eb25baa8cb0931b0ce71e25b1c6625f0db465c0cf8dab15f7f5fd896d230a141a7eb9131b794836d349cae7d87f93811e867aa8" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/91.6.2/SHA512SUMS
            return new Dictionary<string, string>(65)
            {
                { "af", "217b9d071c89b502a9c5e640a6359ef875983463be178decd436fd40ef4ccdfdb0dc5c9cd80cff5728f70829d3c643dc00462cbbd1e1f9d80287f7deebf5f40e" },
                { "ar", "214a03fc60cde08aff30b3cc891bff9ff4e8d3bbcc59092b8a2d2cb765ffbdc84e39af20f7294d8e6706f3b712223c2eefae45115b53fc1f22fb2b9674be7c05" },
                { "ast", "88eac60205d93d259ea532efcf1e41158f3035da7bb910a64c453b5a0ff432b53fcb737d7cff2dbd3caba8f29d4326e91fd63ee28d16967ef4f14758dec97100" },
                { "be", "d9c8cb355f3025bfd3a5f17788f0765fd6cd683f8f355819ca29906dfea490020ac2a5bdc5461ebc4d2cb23963b56df649d9ce04bcc7f5113a9460c1aaf91eac" },
                { "bg", "2d491760a18d36ad4fbba9f2a7f3cfcb891e1681e14f783c4e064bb001d369acb81b56ebdcd4700736b11df1be16c3b225d9ac7a43562ae1783af2267de6f93a" },
                { "br", "eb81866157f77ebe72a6e6ac4a79cf59d9fe6753613698e3fcc3c3a55a8a01d5a30a5db428fca8d99514f57c58d04825a2cb369070455ebad30ed34cf7814f3b" },
                { "ca", "10837d253d0182e2c622b57c8c54988b5d31b09041fccc8e083ad5db57872c3e7f3229a7808b49d54d6d92738f3ce8c715c0e2ddd6fd6d1487fc90ff7ce8d7f7" },
                { "cak", "e4b30fe24a164610cf546412bb9aebd4855f091167c339fdf3204b03483204c0692185ece5c1217fedb8b0336a36ccf0799c24c09e2ae07c6024fc103d7b4183" },
                { "cs", "5d3cd5129bc266e113b6f57e029bb23d8a84e0bd7fc23d63cc4bfa956058167578cc7497a33a8187636cbfd7339a915788168c09e878c0bd13274cb65344ea0d" },
                { "cy", "6346a08e5df13e5f9c470732c878b8be795141f9357afa8f30e1a67ac4c286d3b217472e575a0c4f5814f1750c9ca65f1d1509aa00145ad6ace9ba3215487279" },
                { "da", "0124a7ebd1a36dc5273c4b1ef7fd3fb95de84451bccddd05de54b5ed01a2a2fbfceb231fd8226aa849930576d0f2e8613a3a2dc9c266be79e5e62c930b5ca350" },
                { "de", "cd5c36c8ca981cffa01dad85908a41b7e29f789390de8716f312385af193aed33f8fa01b3f14d9ccfe15dbf1e9d8b798f3f85ea68185f81af2fb93ae8fae3002" },
                { "dsb", "674ad02acfec4f1099051fcbbf70344106a0e1a5fdad07f6af7baad554aa971a144098fbff8655e98f94383a4ea8c805891c73276acf3c6345c868fccd50b36d" },
                { "el", "d31888ea25bd5b3cb099e1b94901ca24f2a7fb567849cc1de15ea8b7ee17d8a8f95c1cb110d2eb96db3019f730a929aec7232cf74c17addc70e7efa0703ebeb2" },
                { "en-CA", "7983f3cdebea7b658566dae33ccc817b8c62330b6087c56f0eb43f5cbbefe177980bf1aada4bc3180ab11a1fe89b5e50c95d06aada0879e3a3c837ff8fc7f3e1" },
                { "en-GB", "f3ab1daf3eed867c624a44043209f72fd28783ebd33b17e498ecee22d9eff47bddb0c6647b77911399458e084806e5f6ced4a5627e1c13a274821c4285c7efc1" },
                { "en-US", "835ac1cac8bcd14c317f20cd0885fe29f4dd0c6d213008d0d81a088d8174ae8a45036518d16b850c3a23121b013f2fc2b32f9d752e0f831546d1b0e432def6f9" },
                { "es-AR", "6453e9577bf34feb92623aa206c6da676dbe33bc7e8d9fab9276020a34d282ec261770057e3f5ef15a80ee979393abfeabb26c463086f40e9165967708f2de65" },
                { "es-ES", "9f2a4db2b593b2b452b381d9a00e428871f3bf5a29c15a18ac572c30f1e4f814585eb4875aaa3ddd0f6653846b799f591d0d4eb14e3253cf06640ff064fdb976" },
                { "et", "df4d92a951c50a7a9afaf98473b0bfc433b396cc02c32e66f4132ed3afa15e616c021e9d9aeacb2c4d9057da4ab82ce5dbd187a72e5f5ce263dc6758429a44eb" },
                { "eu", "5371382142d7b9ae1f6fae4e65efdb6399e4359644b3e5589ba7dd3b1ec63f079235600f93671398f512db90e8773fd7b19a49c8476eee5802960509d255b5a0" },
                { "fi", "25c57bdc3e5cb123aec57f9933dc7b6f386815a126df2dd97c5bb2b76a6c3499961a2e9f0415f5ec96258e88b674ede44958b3f70eece067fc8b6d6b61efdcdc" },
                { "fr", "c9777581f41e92c0f37038fed95178ac2f462c0ef0c1f0795db0751e5314edff5d2e129a9b584b50091d698b2483fc0833c2a4c9029d8d251c74ddc5658046e3" },
                { "fy-NL", "0e4663a95819a8c86d3d4af9abd53a16c6e579fb70506a6c644b4dbd5701ad57a482c6f8a30c3d79776550ed448f41ab9bf88a8dfd9b0cc3599927e42c210946" },
                { "ga-IE", "2ecd1b07fdcd9c125f842b9eca42f004e7959a26e19781201408dc961ea3a867792ba86bf0bfef09035a508f0dd384ef8a483e2e9d6036543becc6f0b93242a0" },
                { "gd", "f8d458fe453b004deee847aad224740caf4ed2461eedf01e7e4a41407115d6a50d1a1af7f16bb88ce0aaa74cf01feb089d2f99e55169d80d11e6d91ab56fad0a" },
                { "gl", "cc24fd2d37949bf1280e20ce130e8bb30dbc25dbcfb9e36c625cbac0276cc59dd3846ba0e4679388bf4e3203776a1f07f306c568d7207c54cf7441bf87dbe1b1" },
                { "he", "9eeba0389e31574284d8f58c2f9a12437335b3848c2d0ce67aa012986e6e6093e106ffa45f5f74b1bb6e467c671a9fd33afb747e920eb81c6eae16e21c26cba3" },
                { "hr", "df7193ff7f10d7cc69e087d1b3e6615131e4a1e5a49f3ee011459ab8c12350f33c294cc0da63119f985ca924ab9b76003b4934a1521d9e54b39227c1c55fb1b2" },
                { "hsb", "a83fee48edb0428f66bcb4b3e75b9ea5b1f320beb42ec29934858a05597822019ae9b85cbcb3a04d3d8063271f6c5c0d5a76e2e4f7327e4cd2d64d3b3a32ec73" },
                { "hu", "d42ae4f143ac9536e8efd43f1dfaf143595978507ceca5dcbeebc8a0da3188b995a6ddd22f36366d63e0f505c072270059d887044dfbd7ac15ebff3518ed2812" },
                { "hy-AM", "478487e197c898d57256d4bca3ea595cd54cbdcc281582d4f7277e109252e910f37283fd5ace014d3347ce1c67f5e16ad9faac13616a073fe807c96e1335ed83" },
                { "id", "98780223047a440f0ca0ba520e673b627fa42a6b4cdcd40d441738dfd851cdf89e507904e752416f8cba2f264360e46ae743ae1e34aed499d804f24783bc4584" },
                { "is", "f9d0aafb41260406bc650aa60e5c3c4616eba3fc239b140cfe4afe659f8f3f2d88385cf47693dfdf3669328723b23884008188504ab91ecdd72b7b11a7b32a00" },
                { "it", "3cc4959c0c49ab7c1c1ebc1fdb0f5bb61e2a10557591fa9672e9cbd772af1fcf60dbb77af2a272f1183e16de996f454337014257af2ebf330ab41dad24914afb" },
                { "ja", "950f3e7ff9100d245629a012f357a9491b6b0154b9c83409ab4d33e52b393a299b68ceb621baa0a71192af7052537036780ad4eb70c140f194425d448d3d2e6c" },
                { "ka", "08703c1fb3357c95f7a3e5f69af90c10b85be5939d5247886ff329c4c0ac7ec506cfc6f12d1a0354f73baf0943b745680ccab965434c03e071f508345e39b920" },
                { "kab", "e3587fadcc6a95dcb169606fdbf8e71e4a9d365ccc7a62e93935483b767f8a5af3b5e313ae739799f9724982c5e9efa259ef0b25d2b415cfeed1dd4d695484e2" },
                { "kk", "1e96d8036b9fe37f5fe84933ac7d0bd604ff6c5ed87ea24d41645779221e06307e599d91ff0589fa4d713ecc54c826f8f78a5ca8a16991fab5e5f1641ae7060a" },
                { "ko", "249f9fbf81a6cea22249582f20a029af70a38a58ebbb1f30b9b9879c42ba5f5c1e025626808d3c55bf8c2f9a3e303d3a929db6ed96c833d35e01450746fa6892" },
                { "lt", "fd967f699fff289167d6595303328c815d82c5d408fc5039e62165f481b9a083fb16f6f30043d3a58cb26ca29eaa2ebb3b930b340f88790bf4fa7ff986f8ddfe" },
                { "lv", "d262d0b6ee1bbfbd50fb95bd3eda75c80798b999aea02dfb7d5359eb1c99d6744dcb6769840e7521588c469b4c53f72701dcdae32f6892523f83b44fa84e026c" },
                { "ms", "2f77e08d4dea21da4c84fcf41b7b208196712210f08180f431c33880052f2f33c9ee0ef3234755e9deabaa3ee760dcac5dc695c32977ea16e5f2a72363f269e0" },
                { "nb-NO", "09a2c6f348b47e542b114dd9c66e0589950723b87cab2bce93faf04bfa193f9cb6c73bfa51427caf5da2b5524f6c3ddfdf18bffa96858edaf7580df308d58b6d" },
                { "nl", "13952c1fda864ac1d169e26abac77ac7e997b0cb41b129a7034ce050575a15077e257f3179f6cecc711639055e3428beed97d0b15767ae34aa02e3a5a4181866" },
                { "nn-NO", "fe258bf8c7b0cc81bd6cfad6703b144068dc37d0e167c3b4407814989b8900a75deebd716bf14dab78cd69941ceca98ba8f83ebd1179c837d147802927fe1645" },
                { "pa-IN", "34b91962f28d9f2b65c2855f4843cf1c21ea0a92955d84c48af2ac5cd3c87f2e46c4d91d26f3df64c9c39ebad0a9db0cb6af1dd0c3b982de91968742528be7b7" },
                { "pl", "f599e12814a8854974b7a7367fc35ab35b0d10fb1c07f613353bb1b236dfd6a83ef106b756e0cfe5227176ac0124b01aea4864cb80962f1830cc2c5a7e9feece" },
                { "pt-BR", "ce084cc1254710dbee562ee1e4c5a92f9b87b865d9e1c9452119706aecd5d5aa37196d3641d4401051a1fd018670932fb22616ef3b5abd8d93a670176a939a84" },
                { "pt-PT", "cd0aea2a5cfd4b99d7cc0d7573d6ba2a09d594af99a4e2e13d4b9446729f6803d9e358492b917b37dfa37796fecb68993c0d860ff10eab3ce3ae3eab8b5695fc" },
                { "rm", "2411474c040a8db38c978cfba638dae2be9739c4866e8bdd8c783f8d56be751c2bb41b25e7738cfe4ec4973a2a0eb6d12e110d1e89407b3fecb89a330fe37dcd" },
                { "ro", "621217537c79483457e02d9b32d9a60d0cc7f95eb45cd131c43505fb1a48d4ceb13d39135e4bfaa3695d9e2e181b55ae35b167b116a4af58c92f9a078ed3f2aa" },
                { "ru", "fa1637da8e4146d6273e3416620b3523c10f366eeea6ead6b7709bcf9250b5cda2ad483e5350e26232f9d85c05e181b77faf01c004ecd083cc1a218198239477" },
                { "sk", "a5a62c1dc6b2309da2ddc310d226da86d532118a1fa31890276ab1ffffcfd907d7310af1affa7aacd73704769376fb8aa6399a366f181350f293781e1dd4580a" },
                { "sl", "cf656ad84ab09e7abf848a4f9773b58e8e90ca364b782dc55b96397d1532ad80ddc1b9f4a199771f7da25ed3acc4bd2a370c019eba6b50938e576f5427efb242" },
                { "sq", "b5d13c971bdbbe31629f8d79d1cf92e9017f0c884b569230a1e473ae3d3aa195de706b17fb2465da7bbe6061e8392554465c4eaec98bee043572047f9079e66a" },
                { "sr", "a9b232614955aac12dacc797bdfcb4b358a323b3d741960eb6f1f97eebac5af98eaef1ab5cead7139f582aa0967bcf5406ffb87029e96c5c4d56cb7b05791a04" },
                { "sv-SE", "ab88ddd7e5eb545f533b789188a1ca93ed8ff74ec94964e9fcdb4e6f2d005ed3f2d41ee120d0d6ecbc29e6d2a4e604b9dffcd1a8bd13f09255dac6b29f5f91fb" },
                { "th", "9c14604c5cbe8771141a5e7bca81e68ea1e3c9748354e2ba18a5977c3fa4f9f050a1cd83c2dbfd92ed5a2c45f90e1a4124756f560524babd8bd641f3b333fe1f" },
                { "tr", "87cf068bb59a2a4516e9763430cab296b7c5f99f87795fe8ab19da1cbf2d55d0ade41d012d2a3558c966c44bb7c7c80cc4cbaa33bc1daa16352ab937b504bcd2" },
                { "uk", "90e07bb4f0c1a175b5bcd87726d063a6ff4a358be1d28eff471e4c14b1f94f19313372c38511baff8678d0dcf3f83b4171af06de5a39152b202bf534ae039857" },
                { "uz", "657f2b07ddd6566d238c07058105f76884e81341ee619bc78b9860068698d61c105a9f47cbe876c4542ae85382a2f13136ebc94bed236bd3aab6422179215906" },
                { "vi", "07ec320761066506d427ec46c67ba997f0d52c7be9bb6afae466ea08088a4b789a5dd85d73e3f3429981847756e7a7ff7001d9ca15f1a197e74b13b3cfce4a89" },
                { "zh-CN", "30ccd041e3e72b9752e22be2a90ba59d3b604f54a802c7ceae47282d94d7f20783cc8f638afc817240baa962b0e4db1082e147c4789985ec2db829886f14e9e7" },
                { "zh-TW", "4f42d5275b6a39b5d43cc303db8498a9f789a0f1ad79db41847d031f972dd7ab214fdda53a3991bc4c328a2fc8a3ef77094de8f4fa92c15f385a214a0cce7d8f" }
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
            const string version = "91.6.2";
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
                Regex reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
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
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using
            // look for line with the correct language code and version
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
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
