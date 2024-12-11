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
using updater.versions;

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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// currently known newest version
        /// </summary>
        private const string knownVersion = "128.5.2";


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
            if (!d32.TryGetValue(languageCode, out checksum32Bit) || !d64.TryGetValue(languageCode, out checksum64Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32-bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/128.5.2esr/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "278ba120bded2575bcc00a6b2fdea18b249475bf68c34e060e9060088623383e2c8f5312704c12bf267d058b568a36ffa2243873e7bc4d59bd290d4664e8c7bd" },
                { "ar", "81553331c9bdf6e7ec6aed94fc985744c8bfebdcd8bef500af468d0cf9e0923d3ca53bdabf0a13d2b5fdaab1302e1bf24fe14c13c1d43d1af77ec8ce92f48379" },
                { "ast", "39d2b3f548d43ecf848752816b300bbdfc326e9716ba356c66ad6604977ed456c426cd034177ecf09b59eee2a9a4d30da3585fb0347ee274e0888f1de29570a9" },
                { "be", "87acf9a35fbc3beb06c1aebf7dd8bc3a49a14715ccc439faad7789a127888323222168982bb452611ad6ed1c89e7e38259950e32b81f5b0029fb1024ccdd7f1c" },
                { "bg", "551130f3df49b2f569f9af9b37ac2f3b9677b975b545ffebb65e76221ed9c18184084acf0118c5e113ffe966c489d30dadc64c240e9763555356209730ca4aa7" },
                { "br", "4ba65b393c70f20f9dccad5bf708674ad197ad376386871385886e0de83cc0f53390c41863e9b4aaca273fde26b2381ac242ca883ac73cafdb3438f5130fe59a" },
                { "ca", "4ab3ef078152068398dcbdc89c02b6515fbc09b124dcffcbe4685c81bae03f6a5b340e5611af533d974f0b5e410b957931936e6cfb3664e70d3b33a8c7ed6156" },
                { "cak", "d79a0b4ac5e40a2792007270b0b0d16b1e3e19368020682da8556235a96e0e533807a5d2728abb21560499337b9c5fdeb3f7fc4718c9e3ce1e3d9c7e1049525b" },
                { "cs", "8c4ab36e87194d40e31ca7525fcae4fe08ee5ce49adbf486cdc636c59083d43f94b1aab8156850c3e6efa413dedc1c1b60ff0d47feced06314022b28a88ca329" },
                { "cy", "95e0876b55107fcbb2e20a1efff141e9d4761b8d94152c92bde2c258d929927faf6f6596da00ec689181145ac0023da80c398fde3cc39491ecaa0aef99ac9298" },
                { "da", "865ada5097f8a54f2874b3d66bf40326259e0f1ad6a1d1d855a6d94399b7a7a5d281730f4ae7135f66acf626a577c8a65ef5f8e68e70a13361a494eeabecb2fc" },
                { "de", "b6136f35b6ff5ce627e2c4622c10bce366723dbb769f3c9eef499ee2e309893894c306dd7e1b5b63e197e9dad336915e7eea3fba83ba0e0732900707d219c074" },
                { "dsb", "8f45d313cd5531687b26a4e1d1ebc44998f382dcbed94b4a2f8d109cc934cb2d37c103eb91a294c981a51e45ac95a7fa7fb3ba1ff49368729c88b984410278dd" },
                { "el", "c538e80fe17d4b934c9450161573baac0efb2e6d305726a2fd29a231bccf3731bee165ba6df6954c9b8dd9d5de451667a3f8296dd9f8ccf4523d8b4cd043b5eb" },
                { "en-CA", "afd1dc0a89168ef46020e197e9882dd2b8e58a4419af9f761c5f47f258f0d82f7e89675f54f76b61f90ddf77fd7d259c26126126c0959752f57dec46cef1370d" },
                { "en-GB", "dd9bfafe69e50fec0893f85e3fdb297ea2623ccba0f018d4fce76ea18fe73c841c7346ec337604b603a21216948adae72641c0b2dda054a46b6e402d49e5c1dc" },
                { "en-US", "11c8ec444f69e301efe11454382323e849a08a1e612f144477188278fbb45f9d3fc2740bd429b8030eb51bc25cdf58b4ba3c8277471a7e9063cb6902ad8d0480" },
                { "es-AR", "e1088c5f8d4b9d3fd65c28ad0e321828582d201c891a198a1b574fb7aecbc96eac32c89b2e3780d55ad57e34feaf389556c975c4d64538c2cc01d480548fa26b" },
                { "es-ES", "d7eeee2628ea968866a1b95fe88285db50508473bfc28e0371b5d558098a9bab966acc76b9da8987485a5b49318a5e89a9b713a42eb16a97f4f9a5b6cb51d89d" },
                { "es-MX", "e72b485098b35c7669fab2d8e8bd510b8782d1b22f4bd85862e305c17150909502360355a70c1b1e492b787788090c91a8713cd7b69a9ae1b011e35f04f4d887" },
                { "et", "774cd4db34a5af8dc5a219c91940ad0ba7a11fc070251e847a4f10f795bb7b27bde1aff0bd122dfc946671ccedb13bd66c91a93d6ef198d9b54b55ff03453bd3" },
                { "eu", "9117b8aad8d5ec3772d4512000130f7256069533f0e11c23ce95f11c6bbd515ef98f2ab39fa5f58fb951e2b9b7ffe1c868057677ae703345c4d41893afbc3f16" },
                { "fi", "b68719633b8cbef89a2d8c2cc3808d05ed8cee19ba89aabdd02a23dafa4631694b22a7119336ffa215669df6b9e6cbcfbf7f3e17efd3afec4b5ab6af9b2b8090" },
                { "fr", "e88319d7251a149f1db7411b3dd8e3f95f3bbfd59d8657c826e4f0097c2f32566442ccca993cf21516ab94e51dfb1be4537213c0e1cb25ada5072ef5ad47999a" },
                { "fy-NL", "be3dc167ab813c72748eeb4d2e3a378ca2c2c6424b0c6f3e4f4d4b5ba0d002816f65c28fc3a23edca898cf163e636a8defea136de63a30b3b1bc2483ea6072c7" },
                { "ga-IE", "b107e38aaac41613269182d7e42df5becd3a0f3b414a6139f9a1146ba9aa2675aaaeb662b1911f474f4e44222406c303360604951e8b7e4d2c4fba5cb18caf7c" },
                { "gd", "23cfa341415bc4d7252f617e197503e167bc5274e859db9a8705d7a00d3f8f704dd1a52b0f3d7a2ace44754a0b1598a3c7f4bcf853eb7abe361c54601596621f" },
                { "gl", "192020dd19c6d9e2bb2c3fc6fa98210107c36db49d0a05b61f284656c99981726359534ca8088d641fbe8703f4777d3ac93368c9b338bff59e77515dbfc3471d" },
                { "he", "78b0618f63d41bf10b7c9a8edc9ace008da241245aa3e3253d5409a3519604c2d83aa97cf5fa43a9b1e1f3cbfa710bbe496471e0b3d3e78e8b7d6b93a1e2a087" },
                { "hr", "b0f7aa62659c4e14c24de863fd1a7255ed7f4f47dc41ec9ba120a4a623c4985b15a6aed7f81c5ae40e8bd3612d22eceb364d5a35cc75d902d08e4780e06b5740" },
                { "hsb", "c7ec6e769c1c80876e211a367d7e95380cc9fae984517a4547fa38075d3151bae60119dc3794dd9fd34aabcff5fe38878a137751ef04f1493c9f284dd293c0b1" },
                { "hu", "466d4f66bfdb3c6fb1ccafba06e8aa920ae1c58b82a949e0ef59b1e3748acc3aaae7651ce5a20fba8a76ec9bc2cc8ca985f6e4bdf51d772741f99a7a9d13f79d" },
                { "hy-AM", "30d49bfae5805e31dfa38af3054954e61a42e8c192324b033d9ac88d4f9da2f536a51dd4fad0361ca052b832d975ba80c007747ec0cc254c943318b6e8a8a26f" },
                { "id", "1876ec8ddd18cedbc882e42d95d63d2c4e48256a1c151dcd8f21260c23f7cc86b2e59730ca5bb25dda676b701fdb96f33d8bc431107a565d28edac7d524458e9" },
                { "is", "ba00c479409a0411aed1e7837cc460645e4ffc812eb8c7baf3d1e4f9e79185c51c9d85bd823f66908915d8b7aeb6b55b6a53b7d32b8c96ef60357c05138ce69e" },
                { "it", "bd0e9a09e1bb1401629846a9ca92060e42973c9a8835fd3d6c3e13e23c6a01f78b3c39d0a1605c4c1b9ad8a8556cb4de5bc9dfdeda62d36fe51f9cde7a4eb078" },
                { "ja", "ab33097613d1f4f44c0745fe8d321e3bd81998bed07ed005f9fdf9d37900b04e74d2ae7d08f84fc31669c9f87f8e1729e6190b2bbccab6ae1f19ca793411575c" },
                { "ka", "102930da2dad2e0e49ca927fdbab84c009ab93e181e08658d6c3e58ac1d129a16365e777e8e3c5c1b8a574c3cd6f26f4e32581f2ae97777e72c31728817fa5af" },
                { "kab", "98dc62f87b3e5d4bb771d81759b250f38a1c8a69060385125802063f4d062959f92cf107256d16a720709bedc848b38a72d024fadce3f49a603c2d67aac81d17" },
                { "kk", "5b0a593a650488d3a608dc2a383772bc0ce78e537e34d1abd5d068313ba0f9c38fb2203247ba18e5bcc14204d606f16668b8a8c775b62188864d56d3080fbabc" },
                { "ko", "30cad4e56f68288b3a061e38b9ee4b11b8878f7ffa30356a88ad94c8a62564e90ca1185a564bd5c3188ba9e71cb9d528dd470370ff08f76c4a0810e0fd8556e0" },
                { "lt", "ec5fd24420ac546789e9f351c7af594d9c06bf42178a136930f417dead930c2a56323c9d4f863c03915f7c1da4c365dfabf7bf914811d662a2e8dad84719b623" },
                { "lv", "544669aa22a6b9a7ff51db86888fd3f5a36bd8b23e52c30b1f8fe9833a07964d71840cbf8ff4c228adc1546f72c2e62375d0c8b325ca5b012231f041e590d7d2" },
                { "ms", "78f3dc6abf1372d72eb562db949d0187a97ca551b31db794d7c32496f9484d1650d049581dc4b81eece11894b4451ce33fe128901a7c0f8c9cbb8e9ae92d7a1d" },
                { "nb-NO", "d4d3d3451065ae4697f2b23ec797622520d8a858221d7f9c18fe885974cb52c6a1e5b27e1a2b66b58120c630af6c2643d5db00893e49a0675ec808f5a3fda747" },
                { "nl", "c48bbbf49a46b912d7101e1e1900540fa50734fe4b4b545a84be82a07d7724c625216df7160984a4dafd4c4bda1566e63c41913f5eca04dd0f701278d087cac6" },
                { "nn-NO", "72b110c5f62429302ec43cd26cf7e3fd6fd9347e818cdcb2a387ed9c26b15d0b4faa3bb4a5f36bdecb8425ac1a795b393f0b8feb7ffff8bbf95f871f85f4b86d" },
                { "pa-IN", "5a95d511f80ce467745bf30ed42ebc07617c952839188349e1c69f4e3fbf4ad01feda58f034da6e13346d8dcce4861fb27d692261ee05d2b45409c80790d9b6e" },
                { "pl", "053539550eb3a01dff425cdff6077e05e06870ffbfe972ad25fdeebe2d409d484ce55150c1278ddf2472de6b1a76874ba49585a7842a248c47f7fa50e73c71dd" },
                { "pt-BR", "11227a503a8c5d3622aae46a4c30e9c7b75ecd33236f92cb2deb0b273b9d4152cc7001dc91c68376ebe23d115fd6cd5f04c301b4626167229ece3515ed823e5b" },
                { "pt-PT", "a63e3cdb22277d3c595ba945ab0f43557b6434a479fac2314e44cc76eefdebe9e3e52e750a156f13444e774e7f8ce6ad2e46d904c6441c837984756f086d403a" },
                { "rm", "f062d0bbf8c045f866d6aa707051346abc8d826e5ab6aaabdde23d82043d1ed795361e94b9543f2a1e73af45da519ae2acb811fc8c3cc34abad064bfbd4d59f1" },
                { "ro", "24d17367dde41aa99dd506044e3d5bea518d934809b5384fd698991d98fe951c479d4f4a7edaacfb444e93d8c79ab4782453d8ec57a1d014e8406fc9c55ae704" },
                { "ru", "6f9ede66d2351782d1adc5a36f0070484c293640f6c273083f6777e4a0d1c5e0ab19220fb874cbfbe84141a8b94e27c2c09b85b6448fda255cee2a140463fbd1" },
                { "sk", "ee80af531f9031964cd59a1764ed934c6988f5ecac10e7c45c45c5818b9ef0c931400995f8180519ab082a062608d816107e17ae3564d0c125183d53ad36ca1a" },
                { "sl", "4c91fe9c5f129ba796e5ce7946ecf2bc5350300c6230a443e86731c1edcf568bb70ba9d8dbeb564704dd3e1bc6849cfcc0d1b4ed6c6dcc405f54da346ceb7962" },
                { "sq", "76251cd6825ad03286ac0b2c5322258e309d26023eddc9e5efb66f59190e6addc6b11bef5452e3aac1b715f272a4dcdee4126a788601156139b9cbc29c1034c6" },
                { "sr", "3a13b0eda63548320b4e8e4ad77611291ff633c41480e0e34c5231918d537c849a4c17bb4fda2c32bdbe1f4221c27a9815193c5e8c4455422ce0a1ab6bba7d02" },
                { "sv-SE", "936e5e04806b773407ecc0c4671214db866e496ad6522c56ad422017b9372c8b5eaffe88fdd94c5805e64ccdb5bd4bf86cd7ba00c54dcc5940729f8ddc39de15" },
                { "th", "2e6b283b412f66e921e016f2e4b9d3518117950a8a455b67354b54954427aa7570032559831798ce291dd0ee63032e62d526536fb806d4c75e9ffc6c418a89c7" },
                { "tr", "60c9252171f47b9e3446ed1c3b797b5fdb1f766f54930b5537767bc0f01a5b1462aaea53b132b020afc2596cf3371db2783b737aea82afd0ab2ec62f49fe872f" },
                { "uk", "846c18235f970c82c1255b136d2e43111413003e66035e498985155e7da36097b36668c01cf0a8e22f0f634beb7fdcd9286dfdeba26475aede3442d2a2e6f4f1" },
                { "uz", "1d80bae584036d032eab64d672398105c9c5b9e60d6ef969860f4ba10001462c852e4ec6c9a3b300186152b282be67a759e0768cdb4806b14679649788463e46" },
                { "vi", "95f27e2604fcc7f306889ded408ed69a8bbb2eb1f9fce651d8213d7fb80e0d7b1d5581c295d7c94a1afaa188f3dcc52a581414daf2080fe873da6a7788731e62" },
                { "zh-CN", "32dd8cf305e7b24a7c9e6fd9250cca3b695759b2a89e02d3c89287fa35688262cfc9c9c5eb3f402c7fb2fe26e61a25eb3ef0b1d3a4a4e21020075cde3e89fd3d" },
                { "zh-TW", "cb261b2064ae81cb0bb45223baef999d734414ec6a4bd30909ef51211179bfde486f1e721883fa4f223fb3e12c55a54ebf964e265743d15f790c651fdf5092a3" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64-bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/128.5.2esr/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "7e308acbc68d282cc94ae8671a448e6423b5530475ce888173e5ce8339ca282f3b8325113de5c9fab449f30862ceddac813b2f089c8f609172caf6cb63c8945f" },
                { "ar", "1fc1fea20ddd04348ac2c98339d6e63e9434e5a9173abe9f61eb03d2cd4281e066ab85016d5a00e5a50351d1b92c96e0771d5d92a1b5810673dab504d3181b42" },
                { "ast", "23a32f72ae9f7e5718744f169b4fc18a3be7c0a26b60444a2b3bd190713389bced9ccbba739310761b053c40308f7a53be5242bab6ea2f3fa6bf5bc86a829302" },
                { "be", "4f540ba35a274ccf9d2f198cec9f764236b490f0af2ffc05f22fdd6cb4afa9d8c3e40a0ff132f82da8bed24ee8019dfc94187da37b3f0dd3c0d288bd6ab536e4" },
                { "bg", "fda766becdf49a0d5c3ae80eaf4d0d57d57d0a932526c1c5f7293ea75c1398ddbde27332da22f53e32e59d7621f6c2f9ccc4bcb37513b43a8d8015cd5a92eeb3" },
                { "br", "6ca603e2eb7be4571ed342f31dd556ec76423190d1c64bb55875b29d745eeaa354037168619d5dc7636d1b6e3ef4e5d68b1ef50de13f5fa0d62c8ca0ce6b8b6a" },
                { "ca", "6a4a713b006b6c7a2f6efcc04f6714c1132d0cdfe86fe709dd02164357cbe31c4a0fd07058b87e8ee49b30adb3aa3e37dc64059c1653cb4f7f71f9c47943f14e" },
                { "cak", "703c573ddbc1a80db229f5335c62bf13c5e7b2c6195f3db1b15369d14188cf31aa0e6860ff4a5bb2f373d9fc910b078cc89aaf9308af6789c00c8cb0798a5c4e" },
                { "cs", "a21ab0e651bb6461ed11d46f9b4a4df72c826939a3e2b4886829a66b0404b8919872833ffc38ac9c8d8301bd51ce039221d3b9c4998075aad93043e980a09543" },
                { "cy", "46b55bfd1b30bc720fe9cbb9b5d5b06c5b0f15d0364ff7bbc75aae55cca5407239d58ff2ca60cec7fd62ec8bd452cfcee93e901de6849b752b3fe9f87a0c0947" },
                { "da", "b5180905ee4b5a6a7efd1742f708b796cffce2224fed5cb613660513c9784a58428ea8f8847feb4b978f4cb09f10b4e48388c9f044ec82e722db81411b28bec6" },
                { "de", "f5c6ac8526cb09178ef1a12292b5becf9950c79ff06849a237d8066d193c86f433ff13b100affbafc7d79ed51ac83e65ed221d6db1e3bd34948814aeb7463f81" },
                { "dsb", "d61b2e7244cfaeaefdb60a53ae1202fd0e57be307e88943e3d5b84fb44b57ef14dcee5df722619c8299b4e7fdae8142ffbb0942a9a2ebc048bbb11c69dc077ec" },
                { "el", "b32d7d6f59440c1a80e4e8a2236a0abc99d7b4729d50ac440cf0d8ceb907ec85d91505022ea8889ee88828f7a4bdcc56bc72bcb3e308ad8635b7c3f808997a85" },
                { "en-CA", "d12740243d208f7696a27470bd92df3b134cccea9dcb48ae4d8204e4a81db54b753d3bc5219ac703c81755e49536baa8828f75bc3293946d2ef4e63c6f04f8f5" },
                { "en-GB", "e3c5a45025e35235f99ec94d960756381986a9cbe8f53d2801a0f86bf6eb267d72af02697c6d976b683145970d0c66f25b169a5ebe6ae19c1e372c0bee361145" },
                { "en-US", "beecc1d55d461c9dd7f603047a41c69c95eb965bb0977840d149f7adeea726214820d1c1d7bb4621c09323348b2e3af381eb15d5a6258949d8f41a087d706729" },
                { "es-AR", "8e27538ffecab30b2fedbc3637bc2db0cc0f1d9103854628f056c87c8be41542976594cf67119e7ef5f955b2e6e835cd0785698e673ec6ec745f323236c3fd3e" },
                { "es-ES", "9fd17b9c006b33cef045c81fc79dfc80992b129a6a2d8bc7195f6126a5bf419c5ef715b4bd8c0c0b06c68b94ba5c197b74c8bffd18c19df8c9f9f18fbca2bb7f" },
                { "es-MX", "a254702bfaf6f671e54f2beab3e1c38c2aed7c3f2072fe73d6a55383e1489d769526a721151386d14de9263014f3654147c5e68fbc366fa7b8808c6cdde1da2f" },
                { "et", "1afed9be296b548c68bffdb97eac806ebcbae05fcaee54b1dd967984b6fcdfc74f1ce006d04411862eb5f234b953fdaab1d2cdaff897559974fcdf78c864ae75" },
                { "eu", "90bead1cd2a4c072b5fa0b892f90b0016b8eda21f108ab513d21f4ab1414002b4f5bbb761e98f60f190b4ad5a86ad301b5a1371287752c99a2b2119a584e3110" },
                { "fi", "f15a2a378c334e0ce2fa4b5f7765fe499c00015a2709a4b0610d1b45cb265a179a00ed2334b9c98500f1a590094ca5b56ae6af5593710385ed5e3963201c4547" },
                { "fr", "93e164d735ac5f43e46a59ea91f028187b1744ec098fdc6fa90c8532f69920f39a2427d5680e6f688db927f394fda4d03095bc751ef2c46bc7c6aa30fab5a73b" },
                { "fy-NL", "23f9e7a957568e1b183c7a6aff6feafabae5aa8fd00eb87cc040cb5f61a9ecf8fb63e27103d2061952248f54e5fc9b618367da9d8853ea81387479ca61a66f13" },
                { "ga-IE", "054c92a9538ae3fd6db2d407eccc4ce619079a2d65a5e51ec81c186defab364b8057391e62ccb1606c4d3e89d08837a1954db8c4d98115471b32a9230da31690" },
                { "gd", "ce98bafb6e128a9de0c0b7b0e9212a357d648c403a100edc94153499057fa7e85e3fd2057d85e8534b1332d02a598223f487512b42ddb6832ab363921fce6a3c" },
                { "gl", "a03a3f6fc8ac8107c99c4b55efeaddbf3a1bb18bc30583695a0d77c2109934b2919300eb4a4e7732e0541cb740c91913d99000dcd07b54d27f0fc2c8a7142a7c" },
                { "he", "3adedb02095e2ad0a031d97b75eb8e95338026af5621862001c6ad9671669bd2d29f2753f51bc60a3c4889942306b9d136b3939b86b726572b57b535d2c1afcd" },
                { "hr", "f174a96eb926eee4a60ef322964605f330778133e5bc104de20c99d467adebe4a1e96bc49f2c26d36713cf8ac4e213234ed11459321b091c44c7fe5c80879571" },
                { "hsb", "df9572921dd3af84ee91d4a314f9f74ec1f60bfd8f37c065c34c79dc050a472a7c303050abb181d9ea29e31715e75877cb628f1cb34c2bd9acb1d3e75d2b6734" },
                { "hu", "a5a9acf6ec1c790115bf3d30d4ce8eee9be21ffe54da5286d9801746419df84293c6ec457156be4b2755a39059dbd38c2a4c5eeda5bbc99dd7250eb465e00822" },
                { "hy-AM", "055fa2af7067cb74d361ab1fbe93f074ad9a2348219cfee9caeaf4648ca15b68214cb75e7d7a90899ab13fecd7e914c4538e9cefd76c04926f5df1571a6dd818" },
                { "id", "c4ba5769352912baf6880a8685a219aca50766e10605448a5ba693fd6bdfd79b969576dc2b4603ddb4e857d3647acfb553cd1338740cf6282ee6ddbbe6f3481b" },
                { "is", "e69f0d8138b6a8fe6f8807ddedc141ae28e8f18b16642950476c599c5fe0ddbf62906b1ac165c5c4752a8826da1b0f0b7a2d4fb94087c704a52d4d7540a92cd1" },
                { "it", "bef60f3794e695b96918a7992da2069010aa7076971b93aeddbee41e5e4afa103f8cc4d2d543e4fabc7776699d0a953584f8e8ff79b27c0e36307e02a60e38a8" },
                { "ja", "74fbf0eda50362b0a67676c76beec29b88f5020574f136d15a652eac3ebad6d35d9c293b41c6aaf623b15e4764ad8597b994a2f16021b09a29da0e192e4ea659" },
                { "ka", "00b42fa7dad6094cc902ff388e050e185298e027da5622c407843bfac5a6a23dd41fa650de4a16dfe64235352323bec243cd96bb1852bf2ae3795574fec16426" },
                { "kab", "9117dc6780c4f8ce0632bec1dfbdd4faf7587c951a78d6cb392c51767b76d074930c4486adf35d1ed66afe9eb502dc988788443c8829a35dc2834120dba68882" },
                { "kk", "0390b0ad4357d5dbd5125bf619790192b0831f6fa3ff7d8936a5ec73d1de97ce4eca9b17573fa3b63683aacf79e78329624c6766ffc0f6df850bfb5ef3e7b90e" },
                { "ko", "03dd5f2f197a4aa070563fe04e32d4018b869b32ee0f13e0c525f001c1aea98a4d92ee5a8f6509022ff72183c57d11eaa9f603f656977f3f560d325d2e637e1d" },
                { "lt", "151163c9d2b06b6c1f12ddc379149154144436b30d4b80c09da02a8fad3bd45e0d76b6c1a13dcc936a881c87a5cde075a88210315e0c793eeff5202790398258" },
                { "lv", "24587c9da7e986becc46b559ef63f275ffe3ed678adb14f54d201f8377d00f648e706fd40313a267c5365ae14a1907d01334da1c82ebf584b3b19b0117c77c65" },
                { "ms", "eb53fb4f2b71a550738597f2386cc463a3ddfa26ad70a4758ac2a1706dfd9dafbb5bae84013fc85090062e2aef05477059151412dade531ee643edf17ee59ce2" },
                { "nb-NO", "06fa243af24030a06023eb2befd6ecb5366172125f3b63c1cda761c50d13319ff103cc0bbab9e8d41ed4b3c70f5b3eddac277e1ae86448db27cd21aa059a8dd2" },
                { "nl", "e98a9162745ae1375c0e68badfb1b681a68308d89c687db837ad67ef50481308d4d10c7b16ba942e6f37b70c510ef532e81cdd7c62ef3d80941cad45ede47659" },
                { "nn-NO", "37caaa7d6d8f569eda73a1de5d3f775aab215ff180327f46664887d28bf156aa1452d93d75187b27028e27db288304cc5a3a046daa4ccd80de7adf9c79e6a131" },
                { "pa-IN", "af8ef7f2d50fbffe876539982a969f459612b3d52277bc2fe84d956548e7140dc4c94fdc2f32108d9703bcf908643337335200671f2d0cb0f756d84662f3bd1b" },
                { "pl", "df70da5db86cb95b0614d0c7a819b4685a66a09948de186d365993f4b2d01f9b4fb0a8a26e3faedbda5970b9e512e88c0aeccc0e68d3dbedf2a6f591610a7fa4" },
                { "pt-BR", "82b445ff58ad26341319382689f10d25ac61de472cdd1b2d9e3f67741315d5360e297d8864b150eaefee9b768a0653da7f58c06c79b12bd7536c7b7665720ce7" },
                { "pt-PT", "c213e6b45cd07d3cd3d32369e8abecb0564a528aa6c804ab22e10ccd0f6473fb1f48cfcf8c164a2d06d271155f3bbaf1116e6dc5d52b134317442fca23174249" },
                { "rm", "5d2d345a52ea4720c115bfc06985e13b7a64ccaf87e2f42c0456753691be4db73e3540ddab9f1f8ce48edf93b1c8468ca16851e1fa7e033a290ec8c5ed50cc08" },
                { "ro", "d691d7a19f10d24e1bce4164338ff025c56ba3b9894927f1a05a06ca627147ec253acb4d15f008d62fa8bc0275ee2a0493456dfdb3abe0d5991157fc962ccfc6" },
                { "ru", "480c88cc9c96ef7780de6714f6efd6f65a502016e110ac50c268ac172b27903c2caf5baa127e1b7d00fa85fd2189dee3f542e01421da30bb947c5e2db98f51f9" },
                { "sk", "be9cca7b92c453f91f8634f912af170eef554bff1f7678860b4ac6f7dee7bac107b621320a4b283fc7e883eb9e1ee8e790af9f860c5087cf8692bfe9ef15f548" },
                { "sl", "bd95bd8f8de146160b68c48d932c07312c8c7e9e82d8e108adac0f3554f430c31a1c6a6e2bf76157335c9eaac7660efe95c5d3690c568441b7fa3d2e711a67df" },
                { "sq", "de83d389f17d6e377e74400e16be258585f128c92e41c1647ced517ab58cb28abba3555b5c62797e025f29a4300e0b96fe61c0c3e49a5b02d5a72716897d09cd" },
                { "sr", "a7780245a10630f9b2232d470a6acb009edaf1f6f741533b5625b7ddf56aedd9d1c55ae5a29bdddcdbafe7523351add581c37eeac2a0befb3c9ff18492bf17ac" },
                { "sv-SE", "11078941c5c88f04f1017a39b7e6a89682dd2833857b1174bc718e8336ae31c3baba6b3fa12582604612167688541f77db4f0cd241b6292bb40768fc25d234f6" },
                { "th", "029d1e31f5c0752b0ec7f0bc5fdc7e5566880034854a1b9a4069930c4ad2152a8da9a160524b39b22bfe0b6c7bd0a69ef19aaf7e7fb19c97e1bf5227b88f8958" },
                { "tr", "f29d35c4359f7aec955a248c6a45cae2c986682c49e41679e17287c8c0a41968c3515169cd883d337e53cf5d4e20b223680ba3ae2f688aa3e515bd0b38073602" },
                { "uk", "51c6a364b3a8453ee4dead7116332ed6ff2f9913240f0efcb56803acbbb8ecb838f4a6431aa13d00d725a209de032a82d6edb6e3ced7ed41cc66935d88e24287" },
                { "uz", "a901e3a38c5f3cf3991edf6e8f2774d66164efd65cd3922feee430cbc1fe8793c59455155a86b255146b4c5fcc9661594315a0c7ffd575e9fbf9321c8bf721be" },
                { "vi", "b6c004922039d89679ef69ecc20ea7a4c6ba1b697f5b294bbcab9162a2d02cc88148daf4dca0306819a8cc498568a14dfa29775ad8232a1cb0839626bfb4398b" },
                { "zh-CN", "155161910a9fde5418b8772b3b9920d805f53d0bd5a8ab933e9d3327bde86447a560ddb3d20bb8c2660bbe47449396bc8b6315cf94b0072362d4817f769434ae" },
                { "zh-TW", "aae96249736775892c3dfa2cc802106a5c7d003172063188447c2a98074f3c949eb70e09fc02f4f288749dbfae9ba847d0ee13a7771c325aa7b1f10e82b2e7be" }
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
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                knownVersion,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + knownVersion + "esr/win32/" + languageCode + "/Thunderbird%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + knownVersion + "esr/win64/" + languageCode + "/Thunderbird%20Setup%20" + knownVersion + "esr.exe",
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
            return ["thunderbird-" + languageCode.ToLower(), "thunderbird"];
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-esr-latest&os=win&lang=" + languageCode;
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
                Triple current = new(currentVersion);
                Triple known = new(knownVersion);
                if (known > current)
                {
                    return knownVersion;
                }

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
             * https://ftp.mozilla.org/pub/thunderbird/releases/128.1.0esr/SHA512SUMS
             * Common lines look like
             * "3881bf28...e2ab  win32/en-GB/Thunderbird Setup 128.1.0esr.exe"
             * for the 32-bit installer, and like
             * "20fd118b...f4a2  win64/en-GB/Thunderbird Setup 128.1.0esr.exe"
             * for the 64-bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "esr/SHA512SUMS";
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
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return [
                matchChecksum32Bit.Value[..128],
                matchChecksum64Bit.Value[..128]
            ];
        }


        /// <summary>
        /// Indicates whether the method searchForNewer() is implemented.
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
            return ["thunderbird"];
        }


        /// <summary>
        /// Determines whether a separate process must be run before the update.
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
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
