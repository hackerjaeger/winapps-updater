﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017 - 2025  Dirk Stolle

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
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "138.0b4";


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German, "en-GB" for British English, "fr" for French, etc.</param>
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
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/138.0b4/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "c4c79885a76c40f6dec8b9d78e218b0e977535da206dcf402b8ec55d47ad7b6320f68ef2c666c0f84d817b8532e572d192678ba7813dc4e95ce0dc75589bd26e" },
                { "af", "298e7c6dd489c8241a6cb983654bba653d63860931975287bffdc9b01b519684265f2125690207f3b89701f2ba9cced814c8b1b7962e2d64732138f474e71914" },
                { "an", "2e3cf305ed3953bf5afd567de0d94a9a98d53c6a46df553476a8fa2bba16c325ae071d2e75f1b383f6429f96cb4491602711c44ecfd158f3abe3310248b329cd" },
                { "ar", "798cd436d440afd55b0a39cec80c4e20e64f20a48ea9631c685fd3bebf5ff98b5d723fbd1986e353bfce6ebcb55431199aac1f655782d3794dc71c47bd3c15f5" },
                { "ast", "af1f05d5b3db312f41a880c4cd11e28f3ce2faba44ccfc7d9d86cc6bafdf6ffd2e70ef1c7650817a83595b2c2c1aff5b2234dd495c7d2847ade8b18f1bee5be8" },
                { "az", "04b2e332c00ca90cd6ef29daf3dbc9d771285b1575cc91b3e92dafcf7c08e833a78d19e9debc40a6273234b0493e3dd09ffbccb8b0b5edd0f3e175514632752c" },
                { "be", "72f1ef04171efffc899561e1fddea29a65265df123aff4a9fdabd99bae3e2e5f0e443523d895aa3b8c9621060e14fd29d61af474f3ef39274c0572cdfbe2ca4c" },
                { "bg", "192c9e386951ffbc689b9a63bf12317cb84290699ef6297ded7869f300be248bad404d7a7c2bdad9d891287a049d4c001d7a7485bce9880ef290147eb8b2c18b" },
                { "bn", "006c32ddbf7502ebda0e6cfb700068da6e190aefa3a075e1d40520d674ca241ca4c0c4cb98b31f56eb826c630c82a440aafa3bb80818e962577473849ac669fd" },
                { "br", "ee18f589c899b006f2df7a034ed93752e3b35afb476c9e20735bf380f04c7ebea80ec9b4501c3128e9160fa3fcfd294257f6bfcd839e2443b94830522c1e2659" },
                { "bs", "855ee6fcb871968c8b72449e6fa3ccc101def8664fb8735f3b144a3d1a520cf9dc19e44bb05bc348d748ab7e759ff0bdc0ec24f282b6a87370ac87d999b17228" },
                { "ca", "50e71cc47c1bbe03d4ca2ff605aaf341e684409609a26f326eab19a6f42738d30cd00c16b92b577a61331e90db7e9deb6473ce2db9ebb06a4b42ebf3805581e6" },
                { "cak", "2bcb8a3ac5a2ce6a2e92412b04efdf9d269d1d9dfeab44d34a4fe4eb78b927008a8bffa018ede5831f93158f90bef655ab13ce1aaf16f7c6faa33eec6a9035e4" },
                { "cs", "9db1302d0bd470e6ee066579ce937ad11277fce68bc555e5ac26935bafb8d08610737737a966423cebda1a30322c56abeb18ec58ff026ed3bf2599264bb87ac6" },
                { "cy", "9e05e07159eb56b43e8d5502740fa1b3b137c56ee8f554a2d47e5f9226c227202c9b5663f6a01a52782876568faff22de2e20a1c98eec085db7f4101135796c5" },
                { "da", "9f10e2e6cbd9276944392c72b720e1754fbe0cf38d4529bcd3c6e4693151f393239c70b5c017828b861b41d6502b3250dc8da1dcc29da2a7206e69f208fcc068" },
                { "de", "89047bb7cd2d2d2c7b78289c205dcec7f5f5cb0134503165d725e04f962bffac46ef4c60b77f37a44c50bf7659896728a97f9f656e3d5f4fb884f5f9b519f2d7" },
                { "dsb", "743da47dff3331471b1353a28bf8f9db83ada26d7b2b243a81340c6c7773d8a0a32d457b6456cb70f94072dad0512c19d97ef96f40d2d965ce4061dfb0ccc748" },
                { "el", "151419d704948e77755ec3c7d9dd1fc7d36bb1daea450f33b03440bf226111d24e8e641505dce2b6abc89266c1e9ae5f2e7e6b0227a7b971a600f1df285fbe77" },
                { "en-CA", "28bd88c53b2e2ebab99444e115417e8e817ebc04fb2c7b7e98df84a3f4101b9b4fd7cc1375e7bde15e5c0d5e472b842fc079124eb5c7083207157aec7c2979ed" },
                { "en-GB", "fde23d67600d6a011b8cc10cfbdc1ec3bb1207396e7444a3559e8bbbd5a684e3d1fdd5aa0f26fc094b629fb19326ffb80277947cd0e05144d19a4ceac6ef333d" },
                { "en-US", "dd9e84ee82ba8a216fdf75d9d8c8ae0f36514b1360d6f7533eab1259499c0d36afc2415446a55de3c2268999186e2e1e49f3251a866b453a0104ebed85926e07" },
                { "eo", "2579877d58fba4d5f7f94fe99d9deb577c557e16d89d10309e60dbb0c4ccf05482265b77f09713e3ba4b9c866032e371ed656e1fe8c16dbdd7dff4b6689054be" },
                { "es-AR", "08f05d6abad1b21a6ac73522eaec35d8e7492327a2c8f017ff9ef30dbe6799aadfc7db286eba761d2e6f80842cad88a5dafae3457269acc06af33bad1d248caf" },
                { "es-CL", "d063f5a7b2bc9c9b42f81fc8710d38a05798e9d664869063204e467ec605137151b35fdabce2c8dabb23bed6f6e856a15ba67b2e4f83c807beb2af7279bb4187" },
                { "es-ES", "d3f7dcc7a6d0769b37c8b46ed47a77c76e9cfe84059ac266577979f016fa0f9755515f1f3a408913edd254ad182879b291c412d450a0bb42aa31b66bab0a5615" },
                { "es-MX", "9d6fe33bea052df12e6f4ed81964aed01a1c8d53d64a0bddf9f0d2bb99167997454db54a5b71679cbddee779ebfa17a0af056cfd67b93eaa578c9268f1ab72ec" },
                { "et", "389c909416079f3102fb0355164968b82e6446296ea41983ca5bac02f011e165dca90b1a715dbc21bf2dd8cebc64ad18594ddc5a80ade05dd01218246d60c203" },
                { "eu", "92309a7f28fbb8a288819cabf24f3babb325d5b2715ce976ab27140d2405c1412c684ea793ecba09ed427d87297e5eb5f69e39048a3fb770cdfb6bd418f141cf" },
                { "fa", "9b80892007bd0b33bcc577bc8c68ad9cf9aa72dae82eb57df7cdf990eae2185342f5c5de0e122c6cf5c2621acbc3550200f1f79a7df36ddefb67a24c7ce9abbf" },
                { "ff", "3c36251d3dcb8fd14933d976234a53ce109327ebd787bf239abe0ec46efcd1332382eb8c28b062514ae84708947296ba5ff50d60ddd77f90558da37c30aa0c06" },
                { "fi", "02ccb83e3730922381f958160012d450c0a99cf98a4be3df59a86dfc4806d2b96898da06db5ad7259a272ec2391c90e0e6d9cd83bdfdfb9ca7fdfc33f57f7453" },
                { "fr", "ce3327d446d7bcaa751e96e0b5d87eb48259f58abbbfeb01316a8e894cb92f569954fa899e21ce614d9800a8ce734b4c57f5fed8d37a4972aeeb380176b72b3a" },
                { "fur", "55279724768ac0cd1d869fcf0b0e3a5d6df02979acac1a218aa1e11ed5dc74eb0ac032ee8b0e350bc886779aae894893beb50e5c8bbed36e18df9100f6c31976" },
                { "fy-NL", "19cac3bcbc9e7fe52677bcf02c8ba48f535c55a8d0cb3ee3852f691c38c5a8c119a89be750cbd41c0bc22447b732bef9e1531c4769483b3bd50b0533d69e7ad9" },
                { "ga-IE", "37cf6ead165dc277af0ea320b2ee9218ffa19d360b6e61decc069e4123bb5830dfa537c0d3796ff9399337a1d2379b4490c5fabe87a5c5a734930065e6659e00" },
                { "gd", "52cad3dd849d699f22357dda22a26570e3c47504a61d48402216efd48323e987ea041dd2fa0e17417cf49b7893b0ff67801c260bb2fa140160181351bd6b1162" },
                { "gl", "562b1cef4217ef2d8e22fa29b49790adb37e3f50146c00ae0d29cfc4329d271868450951cdea2eaecf735b24d3b3d792a9cbc167641ce89dcdbef2255917a405" },
                { "gn", "02326e4c2cc03f402469cbe6e20ceae770b3c22def261d40ad2a0372c4c24e204072f9b32c3168a48df49dc9aac026c96295cc1bfcdc6ab0c27505e1489aa8f0" },
                { "gu-IN", "a065d13a19c36f31c47e74cb0d987f4b8513c8bac071a44617f7fdad1e06437cf4239eba4d573206e841938c243c19b05d6a3bee32a36b498b2ef81f45046dd0" },
                { "he", "63d90c00c694b23b0ddebadb6f6e0edc0d042dcf83f9cd5c27f73d785c1c62948ec19accc3edac04043096000d48208b65f1a01db5ef1fb604c9f5b1813d3085" },
                { "hi-IN", "099ac688ab0d14f80c934eb8b890b8d92ae81ed462b9f9147d1b7103fa33e43e2ac3702045ba94d8baa99b88106a065ec231f53817eee44bd35acfd799ea5ebc" },
                { "hr", "918b9674473ff028027d1fe7cb4064bfdff46d636ae1550be086948ca3ca37b750494048ad7a31f84a6a204be4b6fb46515747e34bccf118f057e979cdfd0bae" },
                { "hsb", "b2529d8262ee7ef4e47f4876628946f3bc3fba75a2928afba1a11af14a70f220cb974284da5ffcf437fa752d7593cbfe9abcdc6b410fb68e2efd47f2ddd62b9b" },
                { "hu", "7accba501fc5237cfbf7a2d55cad52ee66a4afe8e0dd8aa081b5c1992d2611df62fe70de0a53995a6447ab46ae58f2039b3b16d788ab41af10ebab009473663f" },
                { "hy-AM", "786ddbe6c7f77ee2fc045bc303b002985f70ccd0cabc0e1fae3a4d2f7e69985178b34f9e92ec33a97fc17719e8d3732fdd88d9a983e241885f770b2ee57c4954" },
                { "ia", "24ac8eecea99ff383e9d929e2b263ffe1178d70ac6edcce8d303ddd400a271c9a822debf7b83b74667913e73d277c58387428328aa9fd79b21ca0e098ced0648" },
                { "id", "1920a059fac5bb74a9a57e589037f445a0176138622c9aeab3abd967b4dc41f945141c4d65136a35adeb0cf7aa4ed024206be9d80c2adba577f99906495102cc" },
                { "is", "48462ba5f5f5072f002f3d007ba1fb1834efb0b0bb19dfe1b3261ab3ab3abf1db00e5f6cc33e8c7ff8424312fb75c7a17b41b6e4607e1e9b67a627f9b785d112" },
                { "it", "c206dc52a70edd0a43786b5516e4e07b5ad031d078f8c3f4922383d0d7e18d8f1847b3d810e711d916bd514409dfeadd0c66234ce8dc8859bbb55cdc5e3acbde" },
                { "ja", "7dacb3968b46ad1b270213c2baf3c69751f68a7ecbb4fe6a5b168aeeda16cf0bb8f45a9865e79a1a5cf30bd6f9cb775f5bd758ebe3f03703310c6af556e4b3d3" },
                { "ka", "ed4c6d755ad66ddbce6bdc692faed98d926d293e9537034b2b4c8a4bb13977ad1d67c01e500d8cf2a2e496c5507865fa850c35217d498b89cc08f634e2ce0a3a" },
                { "kab", "e834210969fcefcb3172e25f5c0a64d0ae3ca4db33315b7d038262b28314355555e8e708dfe1916c56d96529ef9e3e07bf4f9cdf2c6486cf63c0bcf452ce44b6" },
                { "kk", "6e72e9bd731e7cb9a933de77bb7ce10e8ec2b0c265706dfd7db4d90f735d7eec62be13116749de181d4a69d7fb2953612d8523916eb440c424e6e9b163af47d8" },
                { "km", "0bb5c9ef6fe9746d889c6ea543afc6e69f29a4f130b9a984c2dcac5b20394605279407aaf4dccbbc5b00a0674bade9694515afb7f6e6eef7ddddf94a850e35e6" },
                { "kn", "56121a3e667efab8269038f816be005584dda63eaebef8a229b01a3b2b1e6c07f50607ca25d224b54fb79d766843b2b7ff3f7f9d7e12ffcc229f454df35633c7" },
                { "ko", "45328aa4aa3a6af2ef155a49bfe61fbe905bbae3f26ae173d275344081858e2f387f68dc83e085abc8da5b8fa1cc5b115d07b599764800350ffbb30dd7a4d21f" },
                { "lij", "ea0724124a77c9dde71d1ce0e4ea300cd0e83619f8acd83db3ba5ec4d23920fda29c1df9f7a8864970ab1af7362148fea8bcb8359228584a0daa157aafcd501e" },
                { "lt", "ecd8f28c974a474f827d75b4b4aa085f5bbfbe9daa2ac7d572adb8a4e733f5257daa3e35565be03d97b2f322c978c0a90b3a19e8a525594f25b63f4a308a8885" },
                { "lv", "0b7f42483e99eefdee9d24cb6a282b340dd659751f9114459669975bbb4eb44ab0d03c0a12543e42887bb758e1e04b302f9feb9342ceb97699e3994d4143c1a3" },
                { "mk", "76c954a4ea13165d3214c4418cd0f5170d438ea3df2172b26b73211796b4c542502900db5767c66820364894771b12936393d0ef6031c751633d2617a843c0b5" },
                { "mr", "fa407e417e66653edc4acaf0c5b234760f9109838be5215de7789b9143f11de9501ae7dfce3f8f7d0f57cc63fe83ed82c62da1ba1764175b10ab2f2f3b4d6158" },
                { "ms", "7342592e84ab9ff177dd8b3a9f49b9a3010560e5fae1840c5314163837f1f58dd4a3b565a9343efef293ea8e256d320fef3205570fb13685fb904195458c414d" },
                { "my", "a1c69f54b410ced0346a8a77d3278827258335d2455076832fa5ecd7aef0865732150c19bd9bccb5f6a0380d62cb9740a51cb7683f7914f39c821fc0d5e99d14" },
                { "nb-NO", "217ff06801cafe49073697b9a8a33bc00093c48f2a8197999470cff80e80362f02a1294b2e0f958250fcb69747d2e17fe14c59be1c2a5732a19724e75182fa3f" },
                { "ne-NP", "6b578b276f86a4fa0e406d1d5d3cd86c5a3ed7e2c9abc8bf7c6f46053d6cfe3d4ad9563c9e4cc511b7d15f66160f5a1a901f3881638bd5495e3609caa1f8c0a1" },
                { "nl", "25dd803a6673a642e8c4c19a102428b4a6e8683f4ac55ec0075fc52f2d23c65969f79e7ac4de5a12fede06abe9cbf4400832d85be236e7672939790cb92b6e4a" },
                { "nn-NO", "d4c71711991baad6a5784d67fbc11a76bee0520a1ccc1ca8a9595a4dd6c77cc3ee28550cbd926a3f745112ef00c6c6012e1e95b04eac9824a80e2bc4ab94f2e4" },
                { "oc", "e6d37be6b2fb02028ba6163d8c49fc5f01a79c898b285b61b6124bc596376d06606a3969b2fde1bba2ed9992b968301d051434bf9704f17bc9dc1785f4516951" },
                { "pa-IN", "cbe4e16c0a0826ff4e61b0142f8b833ab2771088f2f6968d1aa267cbc6894f6a0c5351fde64a4ff2a0a9310c1f5d635b1e3649a0bcf2157b2a8edc67ec6e5c4f" },
                { "pl", "02e3dafb278424c45dcef2845ccae9b9f8dca4186a08bc5fc76dd3028851f7b4163b8d38dd0f8466aa164048abd6ee2a0eaf96a57872695fab3febf9c8f0f542" },
                { "pt-BR", "9ee96deb9c97242fa7e1f2c682f72462744efc8d1c825c101b0d6a02362f6e670dd7f2a59c5c154d62380504268769df6495b629347a21c3c33dee5ff3c0da72" },
                { "pt-PT", "d7c812f6d35b58609faa8e6b30a895531300403bcb2223fb55803f1b0002c29495c0cc56b729fb8a24859a08297659fbd8590f0e757eed6d98c5920130e93883" },
                { "rm", "af7c67dbd1161244cf1c97f3da4fe4b6799543d7bad73992fd8337a5cb1ae9cd9a577148a8fd112ea955ff8279969f14ea4dabf396d0dc2a177b9d44a34053df" },
                { "ro", "61d29efb638d0312469e0047e55fe5402058cf5d5e2fd9aa4a11642697e72ba66968d228635e867e3c1c85f5d0e6506d60372f4a63e2d460319f56303e0f5ce8" },
                { "ru", "c058b8a0a4b3185bc18cc8035c24af6c5046f70990fe071d53dd2f6104ed58c158f04212634b054cea158da229b45c6199a209df0b8f62ac5134c98f2f084de0" },
                { "sat", "b663acca796495ec1d59f907871709c561bdcf347bdd1f5f24b4fa366efacbcac3e266bd5fb56bfc2cb1992777b4245e90bcd71590c42e22b116da1b49bbafb5" },
                { "sc", "9d5f1714f2c45d145c2ebdc13722f4ff3b488861761606bb6590b84583442c9f28a54f22ae0e35800c44e5bc4e2dd0b04016efd061d74b32753b85c903aa95cd" },
                { "sco", "cf9a856c4d7f057f44791fc1285e0dd5a718dd37de4a8bb47798749fa9ea0591cc27272d65a2e2c288d50c00ce4c7ff61a25c626f56ac739925aab848ba940dc" },
                { "si", "f33a3fd88cfeace023947d2f152811d0ab64520dbaef41caecfe0854acf79e015733c377c8df3a2fab991ec93a22ef955b365fa5db75d84f7595f63313c24233" },
                { "sk", "a370fdc9a9ec80afcc95b41340cf1035df6b5e3dd96df2016793da0627e29daa4bfa9195786de4ffeb3fbb28bbb2eeab3b526a36043e825e1f30cc4d7df29b85" },
                { "skr", "30b8c51d96892bd47f0b4712e0fc2366d2e3a33f966088802a15fa86016b2c2efed626e3761fff3cdd71e1cbda799063a8326b62c5f66d5bbc39bf91d6707ff5" },
                { "sl", "429e9ab1bae80559cc1bde08424a37157026ee82a3b9c677f7bad108dd84fcf352d0b054375831de24c398d76b12376b9b99b5e903699605905c56c248044558" },
                { "son", "94ff0d031644c4c539ef25615968bb9924f72ef941d6aa1e028b8b0ecefb1867db228693759843ee279e5d6cb7e929c1305cacef7fdc1728e6ebdc925bc2d0d3" },
                { "sq", "517fa206978c654175cf3cca1faa49ddb724e95ff21ca96e2e58e541e2d7c2ef87cdf3a5396223ee4a31f19db9fc3ff0773a0c52c889fce5cba77cb5e5ea09c4" },
                { "sr", "f3236de158d6143239df74a8fc40d0122568842e5e59e5b9ffc609417b14c846adac16ba52aa58cdf338eef025cdc9e1e97473fe9dd4433e902b639ae7aea481" },
                { "sv-SE", "9012d2dd82f0740d9d5ef17b8f274f315b61c7de58bf8737b8e3f184d9997f67e18c703c9ad748218ee1263ed746b8f0da36bd704d854d3c53822aab7dfd5c64" },
                { "szl", "3ddede225f94d05afd79619e774836b7ce52a41950b9659f0f2f7f040ab6fd511d5a312e7dd876e28a8e464d9766a663d5bc09c82a3a008ec85f307d41324c12" },
                { "ta", "7f3db339b8352240444e72d138763f7731b2bfa2e9228c9f1ea552372659c4292b5dcd23fa0de60ffd29aad8aad9b4422f6883d4d81ff5db7aa9a56ade6daff0" },
                { "te", "9339fa2f7231fbd6f97241c7ee37a5ea8dbb7654c9135f1169190884ba474285a5d2e4371d2f46a87544cae0ba25efdcf7609edde0b50b43b5b7897c96a043ff" },
                { "tg", "b765777c05ade4c46fd8091f8b2856a196d62f7b384d9dfae88578b073058916423801cb38ea5489e604258461107b72f554b9174dd1976182216c73abbc6f5e" },
                { "th", "d95ccac81aa31ac9cf9b6dbaf9112a258c3fd47ea503a902b3317380810037d71c86783aac47182643b560147fbf5c003b5ca4d419681127ee493d855c06643f" },
                { "tl", "920b4f0d50f741cbda48c63fd1cc04cc5fb89e5c3811cacc4286c3214858208f52ba05e3e268310fc3da9581fa2bf82b73f334dfc23a0f9102925a764155cb3d" },
                { "tr", "457e3ff2176df7b519083b0a79f610b8fc93305e0b409df8004d9efd0380375950b64e67e7623ef82016aefbac020b3aeadc019c0c8d52ef7cd3794617a0b4a1" },
                { "trs", "a32946eb1445aab46030fe7e70a0b2cdcf0735c23a17f41aace6922c44a152ec383513afd192f7c0c47effd6f1dc108ee8f161749d48c781da3c2ccc3ec77031" },
                { "uk", "5918ec53ac4d66f7d3d27470a64dccddf7e48a46e980c3c36ab80daf4830cf62b5c3254f0bfca4c93cdf91b236182ac12493c788bfb60500eeb9a5f98299b306" },
                { "ur", "6129f980385509822e6d99c5a5134fab87abd785c1f5be3ab9b69c392933f330a743992aacb628a27e7c40c2d9c2562c5db741841c9340fb755e35b5ebc569bf" },
                { "uz", "32b433dbbd9286ce0aa931f63be90265fdaaeb2d1ff82393b7f8cb9d0063c6ae51068d06981237dcf3c2b2545f9916f5aa60f96cc97162172bfa39ab75c32ab8" },
                { "vi", "e5ce9c6b37e6b78ee844a346ae15bb380c798d16f909b7dc22a046b6b03db0ab87b24deb9c860b2bbec0aab76b96b3892687dfb15872677829e3ba2ee204c586" },
                { "xh", "8b49f2408ec6eb3572ebaa795b7eadbf54a36aa8d510585dfd6837ae2afa595fe0c1f346c328297b92eac579684d4f9037efd2c737e5c09dab78285529b86848" },
                { "zh-CN", "ab340da2f99306008fbc69d13b19dc1ab16132f8cc940567c6b5b7dea82cc08ab36802e8c285cacd989856500ae7b382c3d6a09b65b8377e6533d0870ec5968a" },
                { "zh-TW", "4c0211bc0e4896b4e602668dfd30dc1c1453fce6d0880f7e0d0458349268f4e9c40d7aec8d5e350cdcdfc92789dd3dc59bbba8259e9894c6e6506ea848f4e291" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/138.0b4/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "168681dae5a63a978903327e91ff97b0bdf43e0eb7c8e337ba8c9d03feadb2b8ccc9f3924b6b48bdaf742004912580553be407fcfdd8cb4d67276d98b8c4cc08" },
                { "af", "794d9e48eb4e7807dcd839f36ca50752c4625f815f2b9bfb8cf76bcdc2fbb7075911bf20214afe3df459446804abc0ac081def586fc32f85e41c3334c49047f1" },
                { "an", "8351457105fe10af16e6879d5d0d9b6663e2dbd342c514b51f4231ff13ab4efb98034dbf1f5f86e54b7759f7095f56b6d6fa8f3bf206c853ae0f83081a15d747" },
                { "ar", "4890a8bb936a5b5a97d83e31b710530ca38cb5764ea2e60e9c213e070c3aa09312e5208faa98f864b517ba320f42fa50b1ed64b6eac74a2e2fe2f5a242edb638" },
                { "ast", "5623dd95ba3ffc5987fc0356120c28b012e07b3bb725dcc0ad54a854d326d1c70d7645f3400f4ddd9f12d17eb39f133fa7fd8967cf0ac63d0c65c9419695e0f2" },
                { "az", "5cb9f7c68bc8e16e7d3ec1eeb2da712beb4759011d13cde6abf6ace3d2f7d92ebb196cef251ed1d07de3adaed6a701654f59c69d218093e6ce9c82ce7d1163fd" },
                { "be", "9827a1a59e41295d38917a820d7eca2e74d127819932b514e8165441684adf07f99c207b3ba79c8ab645c731f827498911b466f4d91a2d38d1567706c3d4665e" },
                { "bg", "0701907b91a0e89233a6606d2cdbdbfd5ae971bd4aadab99548f8d20a1f4aab01dbd81dcf009d6f50c762916d2e741c7b9dc90c4bdbef5bc245af79e5270bbad" },
                { "bn", "a7264b0fa3b34b0ad05cdd7e5991ceccfde5e2c7955f8a1f544cca62fafa1b45255750069bcf62333ffb192e21a14161dc512e25fa580fcf84c86077203e8a9c" },
                { "br", "1c5bf18f0e26849b6cff760a149efbd205115fbeed1c673e945fca65ddbc66be9150bbe6e1a27894a145db620c3f5deb6eec8a7b9823104ae9befb4f98b269ff" },
                { "bs", "120c2748c0efa47767a4c1c870a45588dc8db451b3d2f7861d858c12bdfe4e75c9e19feba45db893acecbca39e81300f66bc0a0f2bc02ff930572f5d6286a3aa" },
                { "ca", "b0f6746e9088843391fc8767a6b66cbf9b9dd0af5529ee77210b755ff8416fa330859064ecf1bc5a4050e0d3f32c01ea03e89c4873d828351ba4e3c35a950ddd" },
                { "cak", "f53cebe9ee176c3dc1a3750ac104c4749ecd5c0aacfe4ce0b15bff3c09541e4ca6c079e5df83ff684817c2542db727640b6e28541842891e864b012f10a44063" },
                { "cs", "ea9f83c02c3d11eeda2cf1b1bf58f6110fd864f3bd2eb8f4afa08ffbf21fc1e2ec6e10599e27b2599bcac4b2802cfd5bebb93cf4f99ddf48ca9a668cf3603723" },
                { "cy", "7a21b501dfaf4c9be1ddb17c8cded220cfe0bcc5ba8d1e85c13734f6566717eea3e59f411247b2da1d51059be82ba270e3556b91c3dc8aa1b24416a1ec8c3246" },
                { "da", "1206ad21545ae28e6a3e1f3ea000f63e3c4c8b0bbd5d649f2b1c9b07ddbd00f6f7f62aea699b459c6f2f7a5c46066fd984ee9421a72d00d4cdfb516d647760c8" },
                { "de", "e45d10c99a1efad58a6ff115d808ad43b0a8b1b7f8986c4ef47b18c4c65d13a00aa0072a1366ab5c6efbbbf49da2b76be11f9843e2e6ad25e1ef51ab61f342e1" },
                { "dsb", "c7321c9914e919d7d9098af5ec4643db939d382fcf77f0a71c93fe1cfde2bb777fd828cd8ff2ccbb969ef2f90ed650a453569d45b624586783e1d3832397afc3" },
                { "el", "f980ec62b916d9f9aebc1f8bc3dc987b5f585493e63ebe12ff321a6db8d142f709bedf19342ec17a76b1dbbb92631978b74b8219c0e77f18d4e01d6c66c09904" },
                { "en-CA", "306e2d989547575920e1fbab3a55d9d9157846358b8284fda18a147d271bc8b827e865e441f73ce5a9adc9e9d6947f020d33e601aebcc7ae126dd1b0e09cff28" },
                { "en-GB", "ee22dcc3a0835b2fb944ec040cfb32a78df9851001f31c17ba6b658c6d13bb9c28133cb5131c9179d54b5671904f2236159925faae7c775c67983d8c32e7fe2a" },
                { "en-US", "e56fb02ec17b419e845e0bd100c149c4370d6faf6d8882c12688819820979ff40ef4837ce5ca7d71ef88b7f411ca9ae09c1154f6ae9c4599ade86ffe2c852d12" },
                { "eo", "d149e3c48088f205a684075014170153ffd00e04fdd018460bf50592c0e2231ac6a40fb78d9a77b7e2540f194fc26bacb1a673417e8f8aafb633c5267d0b027c" },
                { "es-AR", "5558423e421b592629249a7eb27a9263d41670531ecc51c5022f586f7fa93480c1c141e931701130ba405eccf4a4fc3f6ab54b370a96ffa2090ed03d1ce0f920" },
                { "es-CL", "ceca3b95e8ee37441a7cdd060052e74892abff0e9538249d60df6d7d04dd623a84433da93aae18ae94184b4491cfeb53402cc473fa420c9178de22192de9695f" },
                { "es-ES", "684d4eb75a7eb4b5e3812297a38bab1c4403f4bb398727729fbf8793c7ead11b059c1be8af521a3e4998838236bbb583395ecc66d883e75f585ec4d528958d51" },
                { "es-MX", "3657b1fa8ca5f6309d85b3065f54425b2f25fd8c58547eb2f2b8232bc6bf0d0883605e4a7a78cb93184f600382c509d5fb63b8972c5d58fe6a340317b83b490c" },
                { "et", "59681735d7a5441df471ce30ab2a574e795d4707de7a3f6c14f8e21294745a457e10ae54212d8859cf437190ad4c59c374584403e70028e919101f36c8d8ee33" },
                { "eu", "89d6344523be825bf58ea72f6882679adf8c7954ff88b37e1efa4c143e502053b1fd275e8030008d20d494ee5f8dededaa6037281740caafd322c8b92568c55d" },
                { "fa", "5a6d98d7e03f1b97104985ef08692d37af46ba31c5423dd2d8202cb55200fd048977b523d6b3e44d0ca697ae5175d34e222821b5c23c0b081c06289e35df213d" },
                { "ff", "3166f223767371dff67ed44f4bfe0b720169876c144d76f01d8f9230d728207cd03f3504d6d556a182a50804b4587d25fa04c29d0dbd9d8d47a00d8f328ceb6c" },
                { "fi", "fa1748382a8657c27ad6360ec15ef2270bdadaabad326941f62e3057ade857af4cfebadd24baa625add9a10419e1c8f6135645a27a13b9bbc92b4e6e0830ce8d" },
                { "fr", "e6ed85824c5938afdef35bf3657f90f3568ea5f647108af0986302cb8ca3c79a36eee0f589b9e42101739f03d6646fd2777b4f563a136ef3c431c804b24f409d" },
                { "fur", "889de545bec13f3b12106b042fbbebf03e75ab767c87d876183ae8fad36967e8474355a8c5b8ee10b12b64bdeb5cb9af257e289bffcffd46b63af0535de36e0c" },
                { "fy-NL", "3cb9b478f1691ae863853caa4e5a3bf3096f3d897cd16fd986fef512a29f3b864f7b3f972f02001651b4c2f3e89ecd844d9bc3ed5a42c068dac9fac02b0b2bab" },
                { "ga-IE", "7c1cf23ffd2d6aaa733733b4f6230fd3e4610187f0a0082e66822ef49c995132f052f25ee549af6b05d7bcaf30eb8094ffe7d5642e1b1d828b2874a6155d516f" },
                { "gd", "bbd95c35fecec4469305162dc8274c121c9b7175c543f05d2d7134a86a7f7eef2a56db98840afde32d543e45dfa62604363daaef0ea217f1f6e3ef9443627af0" },
                { "gl", "00b40dcef4f9292c4efa68bf2183e7fdeae336f1a6ae3ca104c0bd3e6d41568b4dddcf9e23d24e229ed6ffd7d0f881ac7b0ec0f29de0776f7488c26e36feb51e" },
                { "gn", "e5c7c50622388691a093ce6245652911be153f93b24af12edd73faafe9000b58b496deebeba54c79dab08608adb87a9ea547ab4f603df741a39491769b2b4d74" },
                { "gu-IN", "8aafef433e2caeb27cc0401f819e909c5d05cb590ab6a518f203dce94444e2dd5f9d8f466dea1f3b163327ccb7aad9b6785af37793c8daf985ef1183a67c2bd2" },
                { "he", "a904914ae7f31f5b57cc7fe33ab0ebaaf081a5d0ea289a40124d172e742b676602256641e96aee3e9448c18e35e0a2ce24f2b9e8cff98120d4bfa66ded5d570b" },
                { "hi-IN", "477b1a29d5eb30e2c174498618cedaae1424a20253b5e70439f45638b4aa585c214d5bfa0fa8d947503cecb0ce4268fbdf0e778da85143918a2b3ab3d3fce321" },
                { "hr", "59546d9c43c2c459d9661b03ea46f2ae8216b384301077c29673e2c5dd38207bb8134eb9f7fc45ccddfb4e407fb15000cd0089ba6530e5b6e73432d18db74f56" },
                { "hsb", "bc36c37fcdd4d81d542221501cb87291a3a7882966f2396dcb79dd02044028ed6b208a57dbaf358de1b5d965174e277b37ec3212cfc4c60c08a4f856502e96c6" },
                { "hu", "bf94617a0c28f254a68f7165932bba4755d8d4815b6a4c9ee1f6da47da3da05dc5067b67acb07cfd4a221cbb25d5265ef4d74742c03fbf3359502248075e2fd2" },
                { "hy-AM", "e29ee603c89b4cfe081ba5d0163bcdbb324cf4d5486d9f3396a5e0f0d10cef09b175a89a03cca7f1be8957adb3d95ef8955eeb2beff2d4233a89d9df186b782f" },
                { "ia", "bcb0916887c421fece42a877113176969bf9abb7777b7e76968fe7169a47da734cd4ecb3ca230d67b1032b66b7874a3c0ec172408aeef924c7c90040d930ea9f" },
                { "id", "dc7cdc73528eb4eae102328291b2e32824eacb6653fd377924ac1a55bcc3d25f8c7156caa87e927c0b83a8731341e6c838df7431f0e379f2668e6215ffb6afca" },
                { "is", "e604b77f18ed72aa3024c334036066a26fbc698e33aedf3cb3ba350a36a7a8937b99481b21ec786b66068dc660395e3957589f0dbc573dda979ee653d9d5813a" },
                { "it", "d1529f403c3b6a512108d1cf04f1c3ecc0f5dc14e1465aaa95a84c71ad8b39b8183db57774de470811c8eafc2bf00b88c7aa6c854e6df6f20016b7f062de4d7a" },
                { "ja", "e774b336aaea554ba7bcc457f53f70d0793342bde5870d3a049b75d1d3b4a7a54a4fbfc482a32708d568b452667a4d7e07544db1d9618b404fa78e33ae10cb96" },
                { "ka", "752f9563e9981e9fb15f532d96502cd8ab2804c0b82abfcfd1c1f7f3abdbc5c755b50e2c05fe3d2c531a1000e6500036d3bcca6ac2a4fd2d9b52acc0fbb72547" },
                { "kab", "5858af36e3054975e161db681f7b16b445c96d060bc50cde0332dacefd8f603c1e66a06de5e9abb8203cb3a2481d247a9641c0eb101045f6b5e27931d46e578e" },
                { "kk", "4157a310eedd7bdfdd42423b8a37e9cc5d98aaabf2b67baa3d0951f9e92b2924662927e59378f39150f2dfc9cb090af80fc93207e053f5874797f769ddee0c41" },
                { "km", "383b8cb92593c1de2f90fe202ee4133226ecbb831add314ce678f2b486d392dd7173d05ec95a366a182a84eb086bd869b5d5fc59f13831407b2cf435dc7c3bc2" },
                { "kn", "dfdf11b05794ddf89f2b1b7f9cd5afd5751829962c6b1f5b60d33d775617047dd9d409bd5e7853ba61aeef22542eb5dcbc70d32131417d723c5addbd307b2234" },
                { "ko", "5e9cb804fc39c889d9b9ec67fe587751d0e5638af7ffa0a996ad7cc4aa2a8699acba3ab76f3803d030b74e8198bc409524c3322515d1bf734d76d4d3b1f2a21b" },
                { "lij", "ec93de15f6172137d60873207ed3bb7b980552b89a28d71772116d95b0a6e9b3095fecdcb1148b546edf86bdd3c13564c234bc6bde00c8c1e2a313be3134218a" },
                { "lt", "60e2ad37a5bd78594b2c0976ff39b9e769aac6cf4c9d9f711aea642b95796cbcd57d4a6b0d9a2753810259af81d1695290a66dbeb2c975c21853124d61e604b9" },
                { "lv", "fd53dc94ff5f082121f049ee2c5192fd8a0928ca253a1a49c5b8f9c55e4464af0e304a5e703cc62b2ef4375f5f87e0f53f6d4b2ee21c3cbd5b1dc800db6261df" },
                { "mk", "949ac20be1c603c41b77685819b4ef9e386bf9e838a03ad457bc30e015de72dc969c726eecfc3c8e934218d9d50730daac066b9e8cdb921c653697f8fbb76ab7" },
                { "mr", "ca6989b58cd9e614c898ed745792d726a9308af1b1639f3dbba0ddc1f653162dcb7396789a925c5b981e807e4e4cad90185f578685b09c59227fe92cbf010c98" },
                { "ms", "a9d40486dc7124d5f1f8eedf0c3f4c9df35babaf271c107ea41734437ef99daf31d1848e194497d686bd5db32110174536827884644c64ff0b9f45e08044622d" },
                { "my", "2a991e15760e9d0641be47c76db57a9ff028c19294e758e4bb8a16f85c5decbde1310674948670cb9b74444b99656a2a6dbe55a2344fc7ff63dcf988cf06e2a1" },
                { "nb-NO", "68d2846e056150aa4b5cd66cf17dad74b52de931efc0b6e72e5fb05130fbc7fd8c96ae70a33ab72d7ac26e953008f8a787f9a11ffc3a514e598464ba253597a4" },
                { "ne-NP", "6722e36ba09529ad49f6164c3f0e337c657575605b725118a063213cd89ecd767179be958ac654fbd160fb6b575ab5a8f5cd1f7c2fec9de25d89662be5b9b09b" },
                { "nl", "12f50e3e6c58c260cfc67047502ecc6be00a415aee50ab532abade4f0bee435beaad4f5dd96d67e93f4ca4e14efb059dbe23c21d41b7929da195497803f62c98" },
                { "nn-NO", "7370f84975199f064c129ce7886711a349da69798d7fedf784e138ef9a1f5341362864acfcf66f81410597bfbf69d9c117a71ed2725d939defc1314fa4f2428c" },
                { "oc", "6c51853886b09cc48d554dfd8e6b9ccee95858c1e1b64f210775ea0cb98aaa68de14e83baf2928b79b4701da0797b3b0b1d991958f5ae26f4a0213a7a138391d" },
                { "pa-IN", "3ec04cc75151cb9c664f472a018e1bba3dd8226df416edca0d1d175b2b8bc629ac6d1bd7ac6dfb918c6be6273c00533f0794042eca0c3122c6d955cf8033e8f6" },
                { "pl", "d31814e4078c8b30835da4ea7490c7b5275cbc2b35f1855f92dd34c26b02083f95f926dac28959c9b1187b6836f9db2d3c3e3ad0fb73e3efe848cf79a875d185" },
                { "pt-BR", "523b290a002d937ff4929c09570a8f45d5be70d1a6158adeb361ee1be196bbc960cc0426ba53a1d7c7b935cb425f8c4fdff9b99c3e1c7f9a70a1f605e35d765e" },
                { "pt-PT", "b2f769c1c5f0190ad1d64bd3479b60e8a90d519dea7f06c54052ceea7e287dd751a0de757e737716124ac857d1a4262f82ddb2b8c910d639fe43641816398a4f" },
                { "rm", "77451f29c11071170792df1b05e9dfff145222537d539c3ed2cf76f32df51d439e8364fa493874984db83d1450638de3b73ff2b7f6c02339782d21908f4f4d70" },
                { "ro", "3b84b9ee43982d8895f98ba78ab00f6a207efea3a58595dd3ff899e0525d84f9e9dea3accf0986b1ecea3b201478746329f3ac602b8b2071746516d4b4aea42c" },
                { "ru", "f15da82d2d720530dc525c74258bdb51ae68d315688c0b99fdf16db278650c4bb1369706de2dffe20b7ee59c863724fc48052007d4682846d07a153680223791" },
                { "sat", "be17807331b9bcf62e5677a0a65f1d44ec161b6eb1d9535e03176effeb5909fbcb81bf1a2b4ee44eaaa2b6c365042465cc4c5d2907e448c5f948fee409088f16" },
                { "sc", "a61f826f60bf3b4afaa9bb9c001e2136b307bbc9bbcf97b5642d4161f4496811d8fd54ea017269ad91a94253a13d4bd8362ee06d35e3907d6b383398425ea9e5" },
                { "sco", "d153b238245e4e293c8fec3f55f3a0b6416a0cde17b71eebbeb031a6ca8b1522b5c4c497e31a788fbd23f8e484ea2c6e49169a63e4e48b6c0251a140c86918e3" },
                { "si", "d28de025a99bb74b1fd81b90c552e2d473211745f85bef4cc0c7daf73ffbe896e8bfbf40503999476c0fcc59938d563ec2eaedce99ebb7c77ba978be1cb4ade4" },
                { "sk", "a96966171c3d63b408a5438a9e39d2b77e91766eee0e2646f957874b439ed3a98e0e91f2a79cff1e7b203b514881832919f8adb0e475282ffce9bc44ca84e261" },
                { "skr", "96f5e3925a670f7e49e653236efe138917f9ee43268bdd32c86a208c7a499436ba880492377bac56944ed4252ff49a46b44b2fc93db8577b7f61f768d0cc8675" },
                { "sl", "304b7c3085372a00d563c0bd5c291fb9f910c07b2821e97f01abf2f0a345df037d0a4b19cfc7b2dfda451e9e3dd7d2e8ba5bb6c07736120d85743d38ca82d118" },
                { "son", "50a2ad05aaa9235b1604a60f0934a0b5b677bb94e301936cc76b3fabd2b60a4e9a2dae03f3f834285a6a65fb5466be6ec8ed2a733039f78c4404017b18843d38" },
                { "sq", "9b0cb9623b15e85d6e96d4631bc457e2bafbadf1f522f412397a797c385ac69cfb8096c0ca41ed8e7c5cf5d5f240d19a8491705c3fc5e36caae89db5b0ace40a" },
                { "sr", "8d30b1dd13bf1ff6193fef5c0dc86e91400edef18f1f3cd75f5ad7adaa0dfababea3e573b79d71d7575f0c0dfa054f1f60a84cce218ea84e9845b582ad75f137" },
                { "sv-SE", "969a1e15e2e1bfc249195461bb14f83abc63ccccbd5410ef0b73d9ca6b2ba087ddc946fa1c37f38aa5ebd213d4be2c319e7d93001dc8a8930ff9c2b119e07e52" },
                { "szl", "c4d11ee6b3d3e40c37b2af4881ddb51657cbfc701b2d0ca877081b8e17b5081dde89f253413c31e34c3c076f46f4c9fdd12f5aca9c735d68ee2b15eab6efc2b2" },
                { "ta", "7d9d722318367af60893d480f8c21505b3d92b9798f604916040f412e65ffc80d84105b3ff5c34e3ab7dd4318cb91c7d7884d85c7b47171ba9315d82e61ccd86" },
                { "te", "d62eb64ee5c29f508651f25680c4686b71510f2f35beed41430ce925c067445ad899f05a6318dd6e32999dd2111c91ba5c1699953520bde03722d5373acf91dc" },
                { "tg", "b4d7bbf15d1c0fae7e9585cf7f44eb89158479f30dfd1009ff7b00f1c3a04a7f0837efbb088c69eca439528417a25c0132f5aefb6dfe80ecbeebe43a3ddf8c87" },
                { "th", "803d82bd77567eb4dcc7c3276c34b5792f90cc7b8f5b3b26f837015a56991451a0a8ff064b4ac9ea0ba7a55a27816ee4dcbc13f6333ba17410d38153341e5732" },
                { "tl", "e2e768566aa88951dc9dc46ac7ebd9562bd9fb5ccc674b4a7cf5cdece34717c59897ecbdb0ff0418f3d311f9b9f4add366312ab0ce8971896e7de88450ddfd6c" },
                { "tr", "a82ebd9be0b9624281f0ac60625f496ec808b3f578006f5cd5bcb2c5ded399c7b4dd5c443e8467a7b0d3fa3c96eb478317bfd9012152d3ef7cab928637426c89" },
                { "trs", "a82ec830ea227cdcd033b6b8f043b5a08cb15a473f5fac8f1eeadb599bde8ce43cb9ca4392fbe7a5808a0df5380f2ae7079b5bdbb1a2b97b499112b4a204adab" },
                { "uk", "dd498db994c9e21712cc00b8ce29c8619b083e1842f7e5d74e8df667fa1145138fc2dbbafc80e51a3a52b81954f4be68fabae714583fd29f944bee736ad11155" },
                { "ur", "1b6d16053a57eade372e84816006edc15aae179187a036e181a6638f7af9883c674bdf227ae0eee40c8aab03ddc9cafffcfc8fb9f0a5bce6094be7690649757f" },
                { "uz", "c098e56c578540bb0abdf258698177ae29b634b6fb14f8d371edbd1adc6591da1089af76b6471c926ec510725407dd9e41bab0bf4c22110a1058f43abdc74919" },
                { "vi", "3e7e030abfe4e3a5b66c8bd7717310e018d7b4d157adcf165e5d3e3cf781f41408b96bdbb6a4449424a20f87326fd62dfe5e9c733648a86483f836cdebc43f45" },
                { "xh", "e02de3aa7b11639438a1b26e10a9277f6d40aa840968e2148536534c8a9d8b89dc0503d230ebb01efd2a36257b16dabcfbe69ac7ca16d881a459708b1d3c2433" },
                { "zh-CN", "1fdf4a7047db3d88b324ea11bc888f8e41a250fe83c6495518b19e31ca8a58f768432c5becd04574ab4fe65189a3f419462d527066fb8d8d7320c877cc2d151e" },
                { "zh-TW", "b2549d818fd01108d3a58c77fe3d69c3448193030a13cdc5e4e169c9d89a4e2f3f2517e03a381c404943cd92c0dee75e01076bcc73c2d48034bba6c8347941b5" }
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
                // 32-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
            return ["firefox-aurora", "firefox-aurora-" + languageCode.ToLower()];
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public static string determineNewestVersion()
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
                return versions[^1].full();
            }
            else
                return null;
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
                if (cs64 != null && cs32 != null
                    && cs32.TryGetValue(languageCode, out string hash32)
                    && cs64.TryGetValue(languageCode, out string hash64))
                {
                    return [hash32, hash64];
                }
            }
            var sums = new List<string>(2);
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
            return [.. sums];
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
                    // look for lines with language code and version for 32-bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = [];
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64-bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = [];
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
            return [];
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
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


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32-bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64-bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
