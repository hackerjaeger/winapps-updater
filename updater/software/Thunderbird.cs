﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021  Dirk Stolle

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
            // https://ftp.mozilla.org/pub/thunderbird/releases/91.2.1/SHA512SUMS
            return new Dictionary<string, string>(65)
            {
                { "af", "3d1c96ebcf049ecf1550de9d43487e2bae3308a83cec3d109352a624aaf6c885a81c32a713e6d67c29774ad1a10dedf7bcfd07c54b3d28b9f9aa9b32fc32df2f" },
                { "ar", "5d21eb9cb40601ec8c152736affb1fc1ac1cb05fd4c93bb726909924549519228f76ec85eb15469f8b6fb297279edc2b48bfad80cd0039ebc10fb72278c24f42" },
                { "ast", "7f51d627505c78f83527afaddd4795e5c8b5ac4d42a14575486acf01aff5d1183d88086e02ea8999f28eeb7b414cfb619fc781597a80539eb9625787c915ad6c" },
                { "be", "1135dc797a5fe90da2f8e69f4995c1701b3e47567bb438ee1b51e0a6aa439e935c739446e198357b5d81e0225358d8766c996bee6bc0f2532bcac8a2fee8d21a" },
                { "bg", "522a5ae33c3282bc4b22cb69ea49f71e89250e1dce1842dc62f932e755133e390e428baeda0efee55090806180a1e401e8c8752b57701d131a073d0fa03cad37" },
                { "br", "ba6f358a2f8683bfda695bfdd54d533f7cc6c641e827298a5aff7d18942abf8ef33d20978fddfff803cd20f84694feff638e13a4430489f4b1afa7b75fa4e568" },
                { "ca", "14eeaa401291cdad84d3eda3eb96a4e14e33b848624e17b47b5cb6d0bed9d1e71490df0c0c33ecb3c38f62165903af09206a4339cc7b635bc0b6592709f462b1" },
                { "cak", "6c4f17ca00ec0dc05659e491315e5b36d7b7e998c760aac8d48a2261d49f008e6407c2237751140f08209f492be091fecd8d3e859808d059df2d53b0363f24dd" },
                { "cs", "ad34d495a0ecfd4324a7809da384b2d9e02e928f0bf0c504e704ac838f9ac9e576f474c6f3203dbc6c7a995aa4fb97d8a0967b5543a4395f4ffc7f56ce8dee84" },
                { "cy", "ced7568295999cf2ca9f8c6ea0b7d9080a5aec1041d5df54f2ee3925f8228d3beb39c40d2cc21120d0aaa5d679c126b529d80d48c80a897117fba010edab4558" },
                { "da", "71ffc8f9470458dce5d679eee7f7c25282369ab9081ef9571fbf0c335eaa5d2d5d412066863a8829985af89ae44e4a05433e63bc5cd8174ab17c280fcbc56a6f" },
                { "de", "47102692139a222f5738396719468bc678cca71f5d17bed7bb6c99dbcf545932a1148cc9f2ce0bc0f361f4dfa98e723ff10715c0406772caae059c24f5638ae0" },
                { "dsb", "3b1c1b4b2bfbb4f906874fdf754ca1a87e56b80637bf0ac61c9345a10778fbd19c5cc758c72c6a0f09c24cdddca652fdbbc1908a4542d58ff75ffba446ed2136" },
                { "el", "28dc8994131f8d1c4c1f6aa1fb011dab6b6f78aa0c98ad4c181181d74fe61e7d7f6cc5c714eec51b15fa9d8d655492b3820abd54bd0e78228825b3da1d5f9c58" },
                { "en-CA", "ff09b2f96d2703ff0156e1ad6ee015d562b1b33fc06dcd6e03f929987fa5ad2b6780cfb9a3c6515ef21b77645d70c5b10c11688207c40c2a296b170efa023663" },
                { "en-GB", "80b236c880670039ac22adef14886b37a30e7349b8c99fb09baa77d3bdcdd3ba7ecfd26e3b09279e74452156a577a367b11138624d3f9136a8f99afdd4fc41a2" },
                { "en-US", "010eb043052d16cb8275d257942359c3469c4e5383fac5c8c5a562ef26d2bf8d8102505dbd6c4e8927bffd7e4cd676d6b4f183afcc0ee8f11d41473b1b4765a3" },
                { "es-AR", "149ba972dacf30e5395ca987e4418a3bb9c6a7ec96e9695963ca89e85cfae8ff202ec5163477aedf716c052afeebe173cb84286cb312220b912cbbfb677a6637" },
                { "es-ES", "fe0602d2068a66fcb2801fe7f9c092d53275b43c214fee9c7e494d965c5b79f9fe166564be585c3c25b9ddbfdeecbebc44d877d0f990434a314096ef63bb04bc" },
                { "et", "ecf4d979da12824d6369fb4e5dd06cd7d5fc16ed8b0087f140a271b44cb0f72b3d8c7e6c34f52e209e23078f34f0d9d61b40f02a2cd2e7b7d7d7f12e20cf48e5" },
                { "eu", "f46f3f6bb5ed64cda44ff594726692f402a6b2484c6f6517660b807eb529a26a3edb00dbd093f41da2277970c5883a1f198de03ea53a38b9e34411a05250d1a5" },
                { "fi", "21386ac7141e28c3a73e9046a466d08ff9abfe5ab4a90a40034f72ce1f197ce2f0c403f804057c9ac629a9bec054215edcf5ca436efeea832e6b193ab2e145b0" },
                { "fr", "3ed1ab69784dc03fc441499530272ebff695d5875a341a4486beee7a603e26802a9aa4829d39458c4e742466639cfa6799626663d1c2137a618f0a9dcbc1a090" },
                { "fy-NL", "256ea5088d65007abec3306201a7d789b6d136160fb9561c52454b4a37d940c3c37980a9658263cd0179801608038961b67c01dc4a6b01c59b09ae758b5ea0b1" },
                { "ga-IE", "6d9dec08ce119c002ee3d13c9904df00116295e2782c23350281ac401a29bec1933b0626bea244ce32666ab8490199404bef25c200963b2d24c07b6f0c9610f8" },
                { "gd", "e344aecdf57beac724290efcdf7cb1ce2de98f89e17fce13c27a08d21d0836ef1ac58fb0cc591175bbeab44ee30441044dfd81facb1a3d6656a24f48890b7e12" },
                { "gl", "d4e31df63d986c50de6eb5022ee55835223a6de5324d6f6d04c41df843315eded067926488776cd901e1523d59a6640b8cdb3824159aa98d58e31e991da87ca7" },
                { "he", "0132a711c925f2a23df065d0854069c9066053c333ff5375c2950e8c155b14130296af3004912c610dbe0713e923ec5d58097747761a8cbc6cf3646911847a03" },
                { "hr", "c6622e631413c861606ff629ca1c295ef6ab5b7dc27e8b027050763def0488db9212eba6420b6806e7abde2e4b2ac7c59fff3865f904fe0c758c9471d5a2d066" },
                { "hsb", "601a2e35be6cdd59717318b07bd7d097c4b2e6e35eb267b78fbe9487d7ed55b99bd0fe1c055055ba3c4d47a7520f8b4564b3a6e00f2dda40257b0a22456b4517" },
                { "hu", "e0635c60a8dc8d6bfb36069c6c9547fd1625c591c36c0cd444467a1393c948dc67c329370748d80540635496f0197bc6877ee2ab97fe710a634effa4d6288566" },
                { "hy-AM", "f35bd8161122fc3fd80e2918141db029bb81e8501061c45adbc17cdc417830bb652be6f1c5fd2349401cec73ea4e92e8f802ab605db618d1f17ae78f183d16a5" },
                { "id", "7680df03162f7c2e3c8f5d03445b0bcfe2ee89ffa807d537c74a0fbb1e00ccbe594738acf4166625b8d1cacfad085121328daf9e25ab7a449a0aa837194d70ee" },
                { "is", "a39cf88c59f62c4b9cf86e8c220a563369db3ec9e78be2f9c82b56e755859cda5d15922e13cabd0a86ef964ababad6496abd0a60f94b8e1801e5a2b6e4e8dfd5" },
                { "it", "5b66dc04ac75f462084248583683aa6f52f101b9c8d0f15292fe9e2abbf1cee1338956aed52cb82e339b7e73cfc47cde7f69c7943fa2a3bc2166655256819ace" },
                { "ja", "ee286b53fc98c0742fc657f47287732c5649ec79e57b8d155bffc71d854a5db1fbc81b361f36f9d4db3535674fed81d9dac77010ef0c77104ec03aeb2614d625" },
                { "ka", "a0b5279e65baa06a1bb5742b366213dae9db366a0abe84524e8f50a7230c9ff8fe65af04850ec7a0d905aa08df5219f504ef14b6bd3dbc35d69ca8f1d7ce1424" },
                { "kab", "a1c66efe51500ff27ee591acb5cad651d7ac8e7a39d448cd4bb9eb4a0db4e38a265ee67c3a791e6cb22853a8fae511dce07d5e3d8d331537a87af07b728a4d2a" },
                { "kk", "0e33b60863abf057f23b44127a4cdcb127b7fb1496d8a144119795235c52c3eeb3230963ad6d4d41c08ca94105f07d620d95a02e95a1205236c2393d3744b4a7" },
                { "ko", "735bedd74084962b79b5a88c26ddafc2482da4ae85b0f3cf724a8d9cda1d1f0c69c79af94ed87cbd78c511c195d020477b37af20fba1adab4e2abc9679d8fe73" },
                { "lt", "4440efb8f9fc997e07bf35480a93d032b7afba18702d1c444886ac7dcd1ef1dbe5aed61ab689f46544e86e037bf392dc42235085049e9befd3a1b25cf473219e" },
                { "lv", "27f059a10924e7e6551b9e3f2a5ff7a43e97d6900fc0aeb139500c8e893897c8b6152a3481494baf37b9f13a409f8ec9c5f2a6eb03a2ea02e818f8526dcec07e" },
                { "ms", "b0adf90b8d2ec40699638ab1efd11c1014835db3506f6f0ffc8bbd38159bba1dd2eec237f7f2409c971a431ee39beaf57d83895d0e0a0bc0fb13c4cd26e05a91" },
                { "nb-NO", "b0e31ba73e535797d3af463eebff887672becd53a7d9373a9847c0f60dd994a63177e6c095d1a2014903c4c969ec56cf00f91cf8a18174c6293402b65a45875b" },
                { "nl", "a5849209e216c2187a7d38049bc240330d3458bddc654f53d8955ab5599d1bb5d7332afb409539ef2353698080c4a30e8bd9595881369538652dcdf5d0a5ed68" },
                { "nn-NO", "4e2098c375c22fccfde33ab6605deaddfbad5d57a3ff26a1290bb67b1c9d9d38cc5f526a480a52dd74211f70bee230a4d560af8f76f25de3de12b439ca4d4a1f" },
                { "pa-IN", "8f166ed29de776be3ee6e73b65d9dcf12aecaf947fb5f58950a8431b9a19cd5a89d4cf253b836efabdc8702d37a53be74702861cc456b67695f6b78400df07ea" },
                { "pl", "3fb00ef108f1b2a05369d082c84294942b761091dc2ebd3767f8d523de4e6285288b4ba47bd5dbe95d105f938734158655c07e574686c6c94f550d0571b0b3a2" },
                { "pt-BR", "c2b64a47e3053b897e91760960070083bd374f4b36db4ab45298f41119e91b22e72a1823d1b661e4de1185a48186d90620c1c00a88560c9aae2db63730856eb8" },
                { "pt-PT", "b6630e2e3c208e65e24d5fa34fda2227fbd59ec450904812030580d7bff86a2eb58cf2418ae03d8e3eb3403ac7ac2cf07eeea37a401e372eabb0e86dfbf90c1e" },
                { "rm", "bc2019b25605545eda90fb3bc67e67e1680e791bd84e99e5a8920214b4b58d8d73bc03622c63b70b385eefa4585c2b10b12df55f1657adba601353ea61154802" },
                { "ro", "42cd9dc371b35b08eb6e5667d06402f7651a1d48cd2d01422d3803d6616bb0e1ee33c04cb022a73e48bd031609cafc692e52a474c63634b58547f06428a593b7" },
                { "ru", "bd40ff389a45795a415dc01b2a743340a28bfa00da0b4caa1c5e56eaa296f93b0aecc7bcae9a7692ae6a69227e4f3dc22bc14d6eadd2344900648e6dfeb3315a" },
                { "sk", "bcfceb457a10be848b542bc891a9637f7644554cb1d76a5cacfb59922c78051e16bba608c0011e846f7e2f5fc3f1b2da35474d6568b7946b98f670eeb37559e8" },
                { "sl", "aa0fc40ab9e658d226fc053174a550eb15b488e60e45cd8dec27602888ae79b6b1b3e977072704cbb8fe07d62e699e92122ab272914d5bb62eb1bd7421a5c988" },
                { "sq", "d11e504f80204ec3de7711ee898c3646d18efb7fbc6a28058deb8ab34b0c6d5e54c9b05545eec809915c1a2b68219c51ce424cc56efe442b36bfc3d4b21cca8b" },
                { "sr", "58f0d4ff31e5874ef134e032e6cd56a9c8dc5ef014ac0d4e74a9cf9b54ac75b8bf937ff6c29b2da7a641424cf5e1ff3743a40a959cec1cc3d36838ae949897b7" },
                { "sv-SE", "6ca8a04d890f0de14b3b0e00342543fddfe53591705d84cf3b68bb8aa784a2c0bfd1b9bcb3d9eeeb9c48f451e2d284a106ee12e27d6762be4379c21264aec51e" },
                { "th", "ca99efc5ee9fae13048397344749233b540d850cae501a480c59eb07df43ce2de99651f84c62f373b2c1dc024e2529f8aa883d515473240f0311d4aa1f90f6ac" },
                { "tr", "74f5eb809a1d18da5fd0a1c2d85f2710a726922408727971194d5ac72614d6f2ad00e7245dcc4fc9437135d42fbb9912d4f8fa8da58c1acaaa727193ef700a22" },
                { "uk", "ab33d2e0bf9711b0a58ff266e0269cd0495a72f0fcaf7a146994d2a034f8068a9fe6005769e0756734366e558da8ba7d3d658c1d16bf447e6c6ebd9a741b783f" },
                { "uz", "530392aaf3e36998c461eb112d45426793757e5a0d3b2af2cd45af591d56d5a817781be36453a6a0f94c0853105a6223ade432577524f70d98569b607cb66c64" },
                { "vi", "e2d813f9d536d83edebb17b8c070d29387005905d6141b66bc9b061f9f7eada37676a49a46fdfbfefcf7273d351197ee663cca6563156bce12794737dec80e95" },
                { "zh-CN", "92bdb0766624569408dfb4e986618be0b175d458a33dbcf328b66393a4c945b18efe77023d1dbf8e483c0e571c4d72dc8dc839beb4491e3f98df9d2962319555" },
                { "zh-TW", "3f694e413d1975a6b4de4c9bff373e1af70fc1578fa7d958eba21d52448e450be409b13deba09a7150bf55ec153e7ddb810f3e8c824fa005cb27f9e574dcd72b" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/91.2.1/SHA512SUMS
            return new Dictionary<string, string>(65)
            {
                { "af", "d35da43e29136974b6283a7d86b21fdacaf4ad740dc4e2c10d2941d645e18c7feee6171342dfb817d663b5cb003b6005b4d04a498437935e6399a90f1049f122" },
                { "ar", "30099b9f40e7e91db5cbe7fe15f27e4908cbe41d03e869dc5acf067c2fcbbf155a3c66228534ed7fbcd12ddf5b59dcd7e28a86b06d458e9b40d4a0906ce44de6" },
                { "ast", "c0f9c9ecd18a3d4552c64d478f6478b925bb37f37b3d62d1b909b3c6dbc66af4d8adeda8021497feabca767d8941847d630d48fe426cffb990cc31386ac95687" },
                { "be", "5ca57be20f120810ad4fcba96a413f37a3ac56f760e7bc7701ff9b4ee10fe2323de4e2acc4014ac5822e3d0ac53da595973b82ae71c2a2e7903c2f0f26fe3cac" },
                { "bg", "49444f2804c3729b647fe3c0183cc46fbfc47a19a60024f8e5eb8005a6bf6f62d401f00c8df436cbab6807529acec83bb9e9779c4c07a08f4bbe8108a122ac47" },
                { "br", "283d7be5a12b45c1b192c72d546910e5025909d3e083423c28d47fc9f5dfdb79f65f7a26b6478136ef066d83c0f56002988b4974f5c7a0e8ff068e28e0b0da77" },
                { "ca", "1109d9d38df6d5bddf3fd19e95623287d0d98be38494115e6848ad124bdeb2e23bbe0280ae803858244ccbf7db6184f365ea590132a8701a30ac5b394763b988" },
                { "cak", "a1e9a64ccc4c87571acf47c602b694ec10087aec304c7da39e362ab8ca68c954fac1467712967af79bddee24bbbb8869220087dc6e1ba7043416ecc4832f2770" },
                { "cs", "3807491287d73d681f3d335d35d3b56609488a9a164a9b8442292146751b36c41212d739c877cbe87f0fba7d65c8f1f38eb8bce8435f5dad594217ebeed32eed" },
                { "cy", "9013fc266cc3e55e191bf18a67dc85843c6b79a3fc592ff5c2e83ada796b80de6673e784c4ab6aaf120b6deb788a36aa775e41a76f263c1a00aa9d4f8168f849" },
                { "da", "57a53fd97c0a279f3ed6a27345bca37ee4a7909ac34efedd264a50ec587b07c64c62023816b2fb1e8c46997097af8dc83e16f0af8fe7ad3170bb84b2da1aae5b" },
                { "de", "678cf14329cbeaee449526620656fa6f8d9ab59d87020bdc31389087db59a025a027a582b2c0a26e7279a0501be30c6ad0407e98a5004c07ce9401333b3db572" },
                { "dsb", "ef11f7e28493202feaac9c49419cf77be6da9b742d38662fbcf6a4a33079c4e7c187ac34ad9b70ca0a97c493b25750bfab1b7cfe8591df5fa535dcf58e1818f8" },
                { "el", "ad569700bbca776da21942de94278e8c1448032768daf9fea2d9dc5ce9b5508ad794babe3230d364952d3f84920aa32118f79b0b7399dafe0d044f826b184d6b" },
                { "en-CA", "5092fcfe26f62e27d0e98b083533142f68b7e0780057ae2e067efdff20a6a7cb386f1531e9f32157397fc474caa11dbcdbeb2634a87bc4da8b949f6383620c94" },
                { "en-GB", "1cee71df5e5e75dd4a19f0489fa500430c863a6b2cffccad2bff24870207df07aeb82db0c969b87d365485de96028501f784fabaa04153499a1f9288511e9b0e" },
                { "en-US", "3f52d39b8e5947f06cb3aec8fc1cd88f2c44cb60f8c91c273126112121ff50c9bef8fc63e58d5973f052588e77e2124a7db37bd1426d669d6eb8e5606bda6673" },
                { "es-AR", "4b8416e0ecce4b35db89a9dd4bc0c5bfda853206ad8c9da1f1d5891fb234e551ac264f4fb2e7ac3c08fe57c9fe9adcae8e747bb8a2ca00fd1263c5f6ec784b09" },
                { "es-ES", "95d79b3f78d90c5f5b2241931421dff028dc79a61331531699c9605cff41d368ccbb2ffc8bf4b77a81815c1541ac2b03c6e1443097815bc15e39338e6963b392" },
                { "et", "9c6521990db712041b07558195c5326b58a7d6da625e693709821b5e416a1f26e2195f458e49e3bdb09ade186101098c82783cfca56bb4494b084bfa15f27675" },
                { "eu", "ff29df4d7b07f8084e39690be3b98196023fdfb3c2fd327bebf1faadb975142b879aeef841a2f2c3087a8f5a792a865582a7d212229ac70e1a1a2e43a2076275" },
                { "fi", "dfeb716ca001452ada4fb02de8afc565de5410b04f11775ea39734cc70616f0eee644b33e2a0b6b6a6318ce4c426839fba5911efbb80dd08e4973ffa5ce3d8cc" },
                { "fr", "538a596fef8d27e60bf8f8d8c79846694d7e6bfa505e6246a65040d411898577203542bc199ed7113c2242a494c4229f8d85732f92aba5eda94b431375b70125" },
                { "fy-NL", "2f28629b7741c873ce686bc7b3784691c6b16e0d2cf6edda0dd7c5fbc79e83d845713c26509d8e1fed3b4422b088d25ac3c38e766629ebdcc9619165d42c26aa" },
                { "ga-IE", "7c7edbea8de4f6e769e5c6a87529cb0b2c9e6f1c4ec6498b2dd1e5afbd38b2807d4e7554a5765f167d9aafbcc23ee08d822bc5d2ccfccd47c182923d777699ab" },
                { "gd", "9658404089ca6f43e9966d6579731dd6e032ff942a7e19ea764e1322d14b6897f67a151148ee2dccb57145beff46aa63caf435ae069d4768307a719c51f539f4" },
                { "gl", "6c2b3c3de97f072a4ff6635a4491745020c99fb13aaeedd707f49c84a9dc49cb30707d883e86685251cfaa3a5e668de344100b00a87eeebc120ed8cf32f2903a" },
                { "he", "31e7e416e3b2a505a9ce7062ccd751f7ad590bc538ff497c8adda7c8abd97543d9a838cff55310022d530012d9964cb2012f3314ef6420c78c7f422651f23c1c" },
                { "hr", "baec763a97f3d4c95bd020c6bb7adeee5a4c339a4b46e9002433f7635fc85aee466a6195a91e3a2cf424364b4447fcf05900ce0a3034010ec4f1f9d2d6c6c407" },
                { "hsb", "435fa49435fb9f93ad0223991bf0889df63008de8f4b17e6fa8991c726db520f8118b6d0bd29d9274e70c6dbae038a62f633c07d32c754a293f823458a722599" },
                { "hu", "ca0a7eff060dae5ebfdbeb83bd993878a192dcc85222bace5fe7213db3cf1ee548a4726dc64948edba775f8a1c4f34aa96e8aaaa87ee77ae2ee3053c48cfebc6" },
                { "hy-AM", "4cc1df42890134946d294c5b06982d3acc873ed2984b22cd5ea49b0891648a3900abf7c165fde92ba9c6d28d34c6bc247313ac1fbf6bcb3715d5b9cde06026ce" },
                { "id", "d6ed73add73dbed62acf0e4771de766b3c222fc71ce1bf8955ae8a198654ab3e10ad818e9afa0e05b1fc0f7be42e96f806c4fe4a35d0310a0d609dce3a0660df" },
                { "is", "8ec2a3b5033cf0e74728b0e9b760393cd3c79cec3db9024a6259e6b67e6e35f657393e37004375592480c8f160ae03f4391ec98055761d16c2f07ca68557c517" },
                { "it", "b00839ecf3b4efdd1d42974f6023171671e39ff5c5cfa6a3056382a8f5259151f9a0be3a434fd762205f6704c457b5b9f8ad5a60b7cb8e588f070f0ed4d45ef2" },
                { "ja", "41a049470fcbae7ac711f3eb3363202d62d5dc7c847b799da7104dd0cbb8589c5719ae1808333b1ec8191e3778dfe24e79765cc9288e1f2725fb130d4766e839" },
                { "ka", "c3bd1db41e19bbca2007b00bf5300bfbe1db4398768e1a8dbf2626fe7c844f67d12044d2964b36a0dbb5640102444bac348797087083493813380b4019f5cbaf" },
                { "kab", "387b0cd33c0c7d860452a53a20f9f8d27ad6e70ebd45ba922452f65c86eac7dc63832796d0042c0df131e889a355df04a0045c273136d7c8f73f6de8f336ee43" },
                { "kk", "68631a7d23cefd8882ce515d8dbec89302fbe34719419a5a12c2d77b2ce94d3656c0d0584be68146163068f838ab77ad9cf4f765ceccbf60f2d07d6ed6144c6c" },
                { "ko", "49b356f180a4212ce00dff93ba80b9b5a4288007823ed6f29754bc1c845819e193402beee93a2a3ae5aae12295af48041be7e8f63324a99f4ef9da9e9c27ea18" },
                { "lt", "3cf0e7380a7fa4cb35f99a3adcccdeec6ab7ab0d0a4d9f237ef6baaee2fbec76058762c5b02442a9f753de3295442bf53dc0c25b203015b1f07202b7de6add94" },
                { "lv", "81c2b24464e6f18b49168776bc0a18d254095cfe660a41acdb26fb7d606db6194021adb62f9f70d69ad5959bd40f57f56f98c2cf65fb09f6b5bc46289f93d642" },
                { "ms", "841c71988923f5446f677539e16f9e40af6991ac406733d01abdb29458002f22aefe99e0ee3a35787586188a3b73e942152e4a42f7b16cc2f70bc8e70fcb3f04" },
                { "nb-NO", "735b0e01bd2740d92ba759bddaeab5e641f529170a03258f114527e9b885e59f8bbb9f88205ead9e302a5bfabddb36419490fc6f6bd9b048001125bb91d26aad" },
                { "nl", "10928ac85a598f64e0487f9dcce5bb97a9928d16d184c774eeef64c9dbb0b01c9916fb78f00fc79877ad08cca6c72158e9b6843c2872f2c6e32ddd7f55d3ec3d" },
                { "nn-NO", "523e87c7dc594bd20000bf67525e8dbf7a76d94912d603eaa590f6530bac3cd687bbf782c714cc3fceaaed29b88f53ece078142ea44a7e9943ab5a9eaf74caed" },
                { "pa-IN", "472b04e586dc202d11fdf430f54a7ffd0a403a2a2e090905ec4b590ee5d959c3d826f37cfd7f8a35a05bb51c126549972c3478f5549e60fc471845634a211909" },
                { "pl", "0e73f9633b257c19fdd4f51a4096ab9b6a3bd79804508fd1865fc88ebbb81968dbe2b594860e35bde3157aafb2ae6cec6b55ee1eb494e73d3571e059b84c2476" },
                { "pt-BR", "f467b19b0c1ff8dbe454eefba1ae49044e946287959a92b623f3d5816344b1d0f3ade51a9439a986d73eb1628835f89bca2f8ef827423ff419b83fa265de76e5" },
                { "pt-PT", "eb63a21b21248bf0b3b78eaf5e58d0e9295fe61562f462edc1f4fb13cd27fb9d75e00c68864f12a08f92d63c7ffee7cb1299e6a25c2b56c17da4f8b9c52678dd" },
                { "rm", "d1a60bd1a1bde74ceba5e512150cf2b16b62871b417009dacd41ff3aa80bd1e5debb65b48ea78d5cf01e0c87aaf6d1a3ad26c2e877a505a56e02a65c6750a3a5" },
                { "ro", "2bb5b9ee3fef7bdae636d244a40b41dbf4951f4214b7009576fe1b727436e1746d479f4f7de2e55ff823346858259bbcbefb98f6c210e20bae9f86ad1f09bde2" },
                { "ru", "b2a482601771e71249e185eb7684a3b2a0c545273b14c6fd9c2b2e086d2f82eacc765ec1beab68ceaedf52ec418fc1072b04397555fc239315ad9afe30ff061b" },
                { "sk", "b7d860b5a5508fda58324b8c23914792b9a0bbcf9ae86a871de667ca6fd552907ac1483362c38c197e1ce1721914b72dc89135408ad85f5269cb71152a066771" },
                { "sl", "6ac44d6013fd2e10b18182e429eb1c46012ad13104193ec3113b56b3e2ed3f9433af71057b18df6466d588afc44eb6d55e6d0c846649c52d09da3e738eb765a6" },
                { "sq", "bec3d925a3add4bd13096c4102d7f48a6cedce8edb477868a3a737838759343be171a26356b1eb0f9d5af2a95a6ff4a1787c4e4e6a48518376f5c04924d42736" },
                { "sr", "f3105e520994ededd05deb5606c644aedc83f60562ba94d924bac1e1ed07f2968d1689b23a27b9cfd921c17c72368213d7cbda3ccb9c892377d5c9317113a7eb" },
                { "sv-SE", "d36adb1b52a815fb99d818085fec812c429dd08554e20a3ae119a7b268302fc40458f707c8559be7ebde3da8364157f71585dd46af107ceb25aae1440effc6fe" },
                { "th", "96b15a68981c2ed5a8417b13dbe1cf46357506167f851ccd5ab10985ffc446e35b830a5112ad6829c233f03a621e4c1aa3c90092316d92293fb2144722c89238" },
                { "tr", "47ff8695f224201d959418d3d2562a8b8d30796cf5f1d2f6ca0cadb73856dea840fef5073ecde1360d64e152241da7a47888e1b532cb32c3f9a996e8303dff91" },
                { "uk", "513daa2ab963df96d776af9272a7746a9001c154386f8a19080394bd46f9480cd78a340cc3ff9b0ba9407ad0af89ee7899edd8b483a500bc414753acea5b6e18" },
                { "uz", "df7e7aaed11c2c12f4047096da911656182b0529fceabbef0a66e344a9db252fa9eb24de425bd218f9c14823f795d0e1b051cf7474b1893566ca7eeab1608341" },
                { "vi", "84dd575ef5c00c281ec0daabe17d40cc1f8ce601d722e8ced9cd2649b95013d6bb82a24ebfce159d68b81c1c539543da4389c4b3225136566cfd95c4c02c95b0" },
                { "zh-CN", "737a97a093ebadfe282aa0e6df942832717b1d372ecafd680beb40c86685f10f0df25721502bc648c1841e97da0674c205fa1fffe6179e2b2c3506cf383508ac" },
                { "zh-TW", "5d6adfc9f9f1eeb1cb1d6fea23a06fa36f9c367d341def78228f99bdd5f7c3934d5a395d0c090cec85af8212066a7dadca7a9f63719928896fa10f5d29d66017" }
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
            const string version = "91.2.1";
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
