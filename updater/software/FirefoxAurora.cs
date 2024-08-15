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
        private const string currentVersion = "130.0b5";

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
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/130.0b5/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "71eff5e66c88646e8d041ee34e4245ee9dfa14e5c0ff9f09f06fd89032fe8768dc882cbfeecc5629cd6d1df47a6495cb2558729b578eea560fd07324f4659c42" },
                { "af", "382b99ecdb0614e5999b3b73cb77964e0b4c78c49c00f37d0e8fcf4d60c70c2178b2129edd0a2ea2fe84ff68af997408fe2662f80de0723a43dba2fcc6d1a59f" },
                { "an", "b9654bde7ac2570b7a3422e563b5d9e70efcbfd432efd514ae7319f32d0c80740dd0f705be016008582348835a08d94529743b38c972eb16dc3386346fd18a39" },
                { "ar", "b3616673069753742caf68f3dd183d205f7f4e12940fc8006808957d9b8453c443e4a3020f1cffcba14b37e1f7795da0a2ae2efbeb1e54121791a8994f1d57b2" },
                { "ast", "7edec15eac4ba6d75501be0609a9c82f6308b08b2794e3352938f70b07747e0c33c42df803bdb49afc271149a8ce21e3c90cbcb0be8b373603e27c13abdf078f" },
                { "az", "8ef026faaae5f96b2cfa9e335e3688debc626746e5d2864a0421abf0ef6ec4d7c06b67c30bab90ae1efbbf7e04c6e27a7b65bdf90ae02c9460fe3ad46af417c6" },
                { "be", "e9987f26868de212fe0f3a603bf7bdd1637b2633f32be9062bcef14861f3701eaad4f6013cf9049bec9c5530a566cf851643fe4cedd6b655102776850c4154a8" },
                { "bg", "d0981722f19bcf874ba91393e391020c06947f0d1596283c436c9a2417b6de2b9336d1ebb71509fc2f088abf4b0ac3a7e696e0dc1683e1306fa300f9f1d1cb07" },
                { "bn", "9005dd235bd062d4ba8d61370d9673a38898fc10af2872e43ebbd76361f567f0b4705348783c78311e43ebe9b9b93be0084e6d633d873e082005c320e39943e0" },
                { "br", "0b61f714131bcde38322f6bb772be1ae0dd9297fc4793cf2458bf0ecbf8e5d2442b8b1a738b7ade105b76789b663c560730aa0f9839e93fcddf1a35fbba52665" },
                { "bs", "75b98402ebfd8cabe10c5bd041d9530e4c5219db8e43575995d6fafc3132a3ade43bccacf00c124f45ea1b0cf64c3de71f602d93c28c92e00c85c8d386093848" },
                { "ca", "ce877fa82472f08d395827f7d19bdd8d6a7aba8ac3ebd55e4646634c3f69432f18f04f808fd52c56fa3a8ffec1a9c08a4ca93aacf2fd583bd26de1eec9cdf695" },
                { "cak", "94c74ed707299c970f8a62dfd7685cd71e94d3f2438ddb49d7463d042367c13236c6763231500e62f378e188057064db77681811d5c61cf4cc3147672c4b1cbc" },
                { "cs", "8ac1f686be966f5467fa8e9a1dfeb8aedef11ea95ec099159a0fdafd13321e164b6d601ccb8e2f82dcd2ed45d0157a60f66b8deb27fbc191b8a312bb7ad90d82" },
                { "cy", "62fa7dfd2bf962654bbe978178ffda37cc657b37a4a829a3387f7459c9eedf898ab163ee5ed54722cd7fd62f0d0a0b8a8af5715e68516bac9f2e96af5e2d4bda" },
                { "da", "257153dda17480690f99cc6a578a3a49a8f69720a04fee218b4049ecfa0b0823e15f5251c1cefc861a6ab35d752d7128247354bfa189bb5047296a60d549b44e" },
                { "de", "1b56c10d44a4d4218d23501288729928b11bf6730773cba2e1505a6c76940b7d319d859c4ae8dd2dc943d04464eed37af423dcbb331d30c678bd21bbdefcfc36" },
                { "dsb", "4190d54f282f2707610dc22b00c53592f48ab55972bebf6df9737735610343a08f0f5ff99603f18252e71eb9c53f4868f8eb632cdcfbc8a7f4d2359414b7b7d4" },
                { "el", "ddc72e5b809c09c04b453918a4f63b20a9a10485ffa893ee655df286086c91ced8be42fcb84dc8d1218df8fc77868da86942af85a3c5249dd841753c4bfe9e84" },
                { "en-CA", "9f85c76b451268dcd3c38451ee604ff8207e3fbdc1a709fe4ac4d225918ffbcd99ed6a4d571807e61b0565d255133cbe570b7f1e9ddee89fe5860ff2c5461150" },
                { "en-GB", "16d08c6937f88d09403a4702c46830e2742487333ceb2972ec377197ad40edf618012848924fd94aa47a3bfdaf1866341595677e8b9df49c70c1c7213cd17f41" },
                { "en-US", "f7c29110b61db6b3c219cd2b18eeee132dae06a89b29664c6af916e483a9c0054a82881ad08ae3ad32d27a8ed2e941e1fac8136e67dceb869101cc8991a73681" },
                { "eo", "8450243bbc756183e2a89dc08baea43a9225ed019b4419fb77b4cdd16698d89301f05db3b97d64ad349de5dde4224edde8e9b4060140f4862588f334582c9c07" },
                { "es-AR", "506f88f8ae900917b33751f4b69f83a642c304e33c67489b73dc40c2c4eba9405ac55286c448f37bba454d02770a76f36901bcfa921f47368e57c4b65251b0b5" },
                { "es-CL", "11bc7477ba1b2351f506b62cc2dcef023126ca726a5b840611bd806809621752120cf6dc6568ffc03bb0972b6b9635ec7563bc3c070d1b6edaf09497985ace81" },
                { "es-ES", "b4f7e9e5d469edf3be084415fc736e02d1bf074f9968da25b593181d47dae2c35497d27eb7caf87ee4d14b73d5426996d5de751218cbfde1e0273eaff6368586" },
                { "es-MX", "97bb217a7a11bda11c312cd4291d5177fcb2f04d928d151563f89b48291366dede3c46efd771d2ce5cc8058f3bb939ce1b1a737d70f63dad46e24bbaadaca1d1" },
                { "et", "62a7d497134353ecb93bae73600719081dc8d2ad764186ed40af02e7d6d1af9c4488061083a1f7c4d9b2849f4b7a645fa5bb29c095f4eb1cc0b5e3fd7837acaf" },
                { "eu", "030efde1fcc9be65261851c6503c8a890d7ef88b6cffb8e066a8a2dadff1a3ffbf4ef1914e1f9b2237e23518bec2afc6c156ea56d524b0fc72171413d157d1ea" },
                { "fa", "77dea5008f7add0fc1bbd0f017eef7ecc50df8d06f7082bfbdbf38925aae233241ea46b9ca075e7c3f0b2f32f4802a0f903e433fe35af2699d2d9911eaba44a8" },
                { "ff", "bbe1641b07e4116f387658303563c5bb41f46d43772f882c0a3a8aa5bed83af70b98d6ebfd39e9387e9bec220a0fafb2995149311d520757029cdbbb8247dae4" },
                { "fi", "1de9e6094e9fc515d7de633b3cb6ceafbe73c002d57fa6d45081dfd3a028025bdfef57e05e2f11d2db831b283a6d35e54135588133f5fa228b1a1b8f86d95a6d" },
                { "fr", "34d17cb730259178f95053f3eab571e08fb8ad0a93dc9932e6723a0e92f5857b62b2d998f26176f179492f43baac5bb4bda9ca99f20b0117c053a71be81cb71c" },
                { "fur", "24b4eb926840886ead2011556b0c01e56407109449deaa75818743f3ddd09ed915156f9b9641321ca6d4cbf6775b367156aa95fafe80a2fe72e88469cfba6259" },
                { "fy-NL", "29d4dd9233ad6e3c20fc2f5ffdce45fa0221e3e12c30587ee841456fa0151d01684c682fab06d4ea2d0b295f7295247905a95a21a996b9091042b05aefddc688" },
                { "ga-IE", "8af4a8df00c3cfc3361dc1395ef2f2da7f5facbcf765f0e781813e7049e01fd53db70009bf8107b37240e11da4ff4ad50d69b6f1d26f1028714cdb327813b9c8" },
                { "gd", "6f73ddf389202a979928f17604bcef10604d9aeca1545b9681f501f5b96d676c1e240c0aadf2a733945c0b9c24889eab6594a424a24c64b4c894114064124442" },
                { "gl", "1dd09529e5c088036986a8a609c3b721bee38ba2e2740631c88240caabdec7c2ac362194cbd625607c2f98425e1f8c2e029d2b68d4a5f7d7ac010a1c6ef2c932" },
                { "gn", "a375683dce55f10259c563bd64fa7dfe0dcd54c3b6f325b305915ea8bf6b01ceeda4b69fce4679edce8abd525c1831f4a38fae6e8d66aab527635f89dfc68ad8" },
                { "gu-IN", "95977098b509be05db7d136230409e6eebd1224c44aa73746a34c3a865741a3dc79fc4d74b748e289e9fbfc9c9c8fab051a828977e2d92bd3a5f2a6597f0ab95" },
                { "he", "364b438f7fcf3d38594cf4ee85b19a0f747efab60d45ed991a184262986f653daad2b3c34fd3d0fdbe2a385821810feed8b8e04779181a00eb345a14fddd5a41" },
                { "hi-IN", "3a52c987f910fa0e14436a68d306ad2b0c63fcba4fa2fe9c6ff7b42c469cc74ce0640a41668a12b8cc538e0389a0ade11aa7fb641ba7eebbcd7bae520e738ff6" },
                { "hr", "1d5022e40ec9395a119444b7ee1bb6d727671af2d47bd186362c13d2ba6e74f2404e9a90dbc927d04aade666ff6b8ada8f46588aa35e0a0ae7413b2fa953fe9b" },
                { "hsb", "3d214d4b67d99b4dd13558deb106758cb1c89ef943b99f1b1fadce0b11d1bd24780b8d665356640f88533e338b4b29ed9b3f93dfbdcaeed99479834de43af059" },
                { "hu", "8b1fb8799b9ab3d8e7f051807d20852f07162fe6bc82d9a6f2b54cff0d5b876e6dbb57dd28cf8004de020a6e4f5386a22b27ecbe7f2604ca8179d197f2b87780" },
                { "hy-AM", "dbe78fa62be51c7abe895f7518093861a5e455b0ea2e7ddb3e1325f13570049bbcf7272fc013ffdb4cd9d58e1b9121edaa1bc130b455965bf4cb6782e6d39e1d" },
                { "ia", "42f352ede82fc6351eab3c665e5c4490721991bed4ca355efb03bdbc43377acc79a636ee85b1cb162eb8d5557145dc4e4f22247e61010317c22a7808b18298e5" },
                { "id", "7208287151d226906f5cba72dd99e1962d0d325133c435c36b8beaf8988f9d9f494e224b1ef665a1c91c677c8dce4dbab2f01bf00fd1ec2ece90f0faa91a619d" },
                { "is", "bad4f4ae671dee864633f2d2e4614b0614052653a3968d4e0ac8197fa0dfd450178e03cae14095faab4e6aceb4e613a19d88de09e3413f07b4f9575285a37bcf" },
                { "it", "a509110be37f2332431848457752d9a737b597e83f424ec98fc3b600af812f24d039422bb24ca62ce65c47cb8efde578d971ad4eb0b03eeaa04f3bee634a531f" },
                { "ja", "d98483709f1dfe6cc28e7c3e0ad5966b7fb82506bf09537d031bdd18b6bd47a01b91a3866a5dbf11c8edf9a1cfd6adf424a720c85caf0efbb24327b54f009151" },
                { "ka", "67ffae18ae7f290db693c4e04f9d183442ba3f71925af46a6e5cdb5059d5c4806d6550051303014d8f161e54bc82d80cb3dcd5228bfe8db55d345efd49d07a1c" },
                { "kab", "19f192520e89e24cc18638084d34288093b1bf7878d4fbcffd268e07c48fe1a8ab24528c24031d647a557f887b92f09a83c318e065fb51c10dbebcfd4ea091da" },
                { "kk", "6a2dc96340907342bef34cd9707eaabbaaee787db5e181449121e36162934aeb6648890be107300dfd4db83d458242a9f8c75b2caa5dfabc997007ed1ab5445e" },
                { "km", "24742285559e90f298ca1167d028c441ca153eaeb9f5bbde9ded6b71f97e60b21a337616ce33b77875251c5a7d2546e19204a7246f38654ec49449abe8147c18" },
                { "kn", "c3a0c93184454092297a320b31f440eb9936e0fc6d562923a1dccc18370c2001d1124a58d1b779914f5fa10de861127949dbb9f974f73de05fd19b9453dd9c47" },
                { "ko", "65b323ce9ae6ef165bd7720ae50dd346d08a1df51e63fb2e40c7f4a3da6e331e9e450440ef4a911e9c3ebe797b66de366dbdc1538b223f35105dcafbd8012a76" },
                { "lij", "ca61fef0f91c5e0a6733fc4c05e91a13925e9d9543606827a2e3bdab42d426c4d01b9a4dbaaea05fe365560204a9bfb082c890c560f976e672ec8e3a6f19a338" },
                { "lt", "52db7e7da2e7d3b105e81efff00fad50a7e180c60e42e527a98f26fe2afdd2068d0dc3367ce773f4d68bdad4523c3b272f3ff040041c9e2ee4f32cd1b1909706" },
                { "lv", "df2bfe01a1cd5bed11ec71d5cadd7eca484ad7528f464811ceb4e17d976b7238224d82da6d48156592e67f18f2d6bf0de79eb55183caa3aae625a37006a53a4d" },
                { "mk", "31dc5dd1267d0cedf9b3986648eee36ba4cf0dfb61297c5624c3d64ae3550ef02137e46ef22b6b9dcb5dba8fbdd7b281de1605502bb8c81a93c36e957dd95579" },
                { "mr", "cb65cb2f789aff9f831dd7ed94ed0d0619bbe54b5ebbe51bd1551cc56199b6615a2f7b4ee5074e415bee9b44fc6da21d6856cbd1475634535326aca398c25ebc" },
                { "ms", "5fcc71b0d2abe35f0f834fc93ea330f7097251e9dd974013d507d4ccf6cc057bd6b862eaddd7d8308d3591366d059e707c5c7c5c687dfd899a377dc25853c7e0" },
                { "my", "9076d7e9c9a7ce48289541f55ba7a2e4a3e43fe94b05ad56b4cb37103478fdb0eb99b150c606a4714a3bf0db7e8d25accc69a90cbe209b97ebe40582b6df5917" },
                { "nb-NO", "dd5d35957340fa6036b527795f27385179b447116e907e52064207b57f1c565d8897ffb4fadf7cba6a9fdc8a96829d094af410768e57b35583e42c1000011849" },
                { "ne-NP", "7dbe4d8d3bbd3d44e95b77acd6af74087f703f745d249d17f517dc2918518f4a439c40acdf7b812b4f8afa34ffa502a4e48caecda776261d5f5f680e98d338df" },
                { "nl", "dba9e490314b08e46cf0b34a607e25254d4f162f8b33e9d72eb7c7bb797dea5db5bf1c82fedb5ebaac022f6bfe329e71d8921aab333f2d66c4bd3f3199ba73e9" },
                { "nn-NO", "20515d7536f7cb8d48cd3045222dd61cfdae436e930c1282635d0892606ec88bd24c937719bba6cba2391e48d7d3de31571a702c12ccb1b539abd27bffb86a95" },
                { "oc", "20a3f3dc4e32b7fc8433fa997c85d3c677d4668db3b9fe7506ee9b667a39211e0e052cbd43696c1967867ef58b5397a518d6a18d52b737b4d0da53c064bf9015" },
                { "pa-IN", "74619d4be424ac05f8f895817a839f146b688e8da6e148f8f17771de432b73052b29ed8369b213d2c4ff54fbef46bed85b5d5ab43538e1586b7242afccd026f5" },
                { "pl", "38d5ecf7f1646791c601afeb3945888f440583d30599b21f290ae414d14f5d18c0a99216b9bb57af6f38e457c4d7903c102193f61ca90555492b77c36dbcf8e9" },
                { "pt-BR", "b34820ba5b5f70fe83a8fcd03eef9009aba952c6efb6ea6f0d249cf4b4fec1c380cd8f4219e1ec8343f0018e1fcdc6ae80ad0455bd406e8619cdd2198e491834" },
                { "pt-PT", "d689bdda35e6f968adadbe6794c6dc0601ea6314fd920785d594658761eb286f15b1171fa21fcd883a2983b6889f81513686915b00eefbae10dd5ca6c03958d5" },
                { "rm", "8bf5ad96d3343345d4215f50a7dfeb5a1bca69ffb6074b61b9fa2ca9ea6fc0cda8c9af462f299ee63496680ef08ff4b74b2cecba4a18123217c6176386e3c919" },
                { "ro", "536777fa70f955e456fb87c09837d81bda3d3f9f73e4c4804361fa438bcb2481f52349f71e59b21946e94ef074cefa0a715b48be5ce94d8e8f994176ef4ecbe9" },
                { "ru", "32d48d05ad8d540642ec6d3a2049be3a2e42f18046f10ac12f1856e286a3a78f86aa4ff29ac94c724d275849a359d29df25a613cc385697f1a7902feda317e34" },
                { "sat", "120fbd17a732963d8624565abe3900d495d871bdeeb9c7e2598f4fba91ef48c01908b0195f0c5b4f0c3fa4956e9d1b4a575e653c7a67e69d31b4c0717a63f360" },
                { "sc", "52250d1b58ccfbe703e740d8ec7c35f75dcc5b309da44207045540a899807d5c31bec1f6b76e824acc744b6fa433d348b8fac6d112376bcea3bba023cc3732e3" },
                { "sco", "e73e367958cccf99a5c6c12cd2820f29824236f8bd85ef31416ded19c75c2dfba19c3674af726e73a32a1a2695c0b29d39942e6c81e06551862949796ebd1d3b" },
                { "si", "14f0b7ce3ed77ca79d1b2c536f3bcff17a92f441d40ea2a80fd3085ba101041261b79c554f32961fb3914f2d2380a1d3cf89ac8d1e8f97ae85cf4ef261f0d553" },
                { "sk", "2fa7961d9a84b4c80cf5e5b757f3694fa2cc4d165742713aeda99a0bed835d9bd76b2dbbea61c6ccfe286882883ca5fbd4ce1ec3e633f6b626aba423d66cfeb6" },
                { "skr", "5f36ebd298edf97835c989969161372db0e1c6f2e1a45eb4d302337af27d2abde13154d5103708349170b3884a78fa160e9a017230cf4774660f74e1c8ad2085" },
                { "sl", "dda224a870feaf3b41881a6e05bf09ae0ba2eda6f425ae6dbad56c86b6921cbf4475287052ee25903ecb3cd6de529a1d042fa5ab5b01a37d073c22b50b2db315" },
                { "son", "e6639680e56d7563f3b1ded2b0f4e17b87c562b98a930162970c2e80a4c33fe0da0fed2d26929149f8ee6704e10e7ace9189705f4663687be083e30c279cf92b" },
                { "sq", "78b7b65cfcb3f394ca25be0be7bf581584503df2a14b8e8fdfb473288b3a979cd8fd78331ad8249e263e340f189586865d36f8846f47a76ef5ab7ef84911a108" },
                { "sr", "9948d4d5b13faa0d84dd00367a84f000b188d9c384b98d6079984dd8d5ee74a2b0b3bd3533878e35b24b67266df75881fe3e55e40073107e3530d0312558c759" },
                { "sv-SE", "523fd20fffdc6595839416f3f59e3a8255b5fbca79743b0f48392e2e60a71e90ae59b149d8510d20343a3b544c89088668f0fe1647cf975201a900ae8ab1a4b6" },
                { "szl", "65421554c11bba930f8bc4e632cdad4df7f7f43150886c9c471cc0d32bce03bb5eb44f13388f78dfed60944f926d769396480a2acbbd64d5413bea451f52e257" },
                { "ta", "dc04226086201fa43b8e47267204f49a2b9f72894b01cd0521b759d0a2839718b9a0520b7e12e8b32f93223379eab701e048f3630715619d7dfcae599f638e99" },
                { "te", "f67181aa7b662090169de079fc9f466f6319458054640b830a777067dffefd0171079733663bce39ff4913f7f9648325ed196692edf7d8fe7eee5d1a16680a21" },
                { "tg", "d79a9e5dd9cee4a8d3432d384a5fb8247bf370aefc49dc3bca813b9bed0e5bfc57c21b586d70c110cf9a2f3b9bb0237925a26ba86704dd50fd7b5b08407f1f3a" },
                { "th", "27540ac97eb3238a11b26908b258491b2f7abffa744b8d9337a0d7a6fd633b9e82acbc2f45f66f486edd0a61979bcc7862c09382ae101dd73b38d4900f1fab09" },
                { "tl", "03799a4e2d99097d1c6a37092a9f81a5692860c9b5b50256e6d2daf97125439f91bd5f8511c9087eab9e1b7e95d7b7fa6553dc566dc53a650564511a9db8fd3b" },
                { "tr", "66c807933ec9f1bd6dc589379fdcc3c27d7fb947bde5d49c2dc75642bae94bb051637f50b793a531fbaf9d84b3ea9285a997441af7702a24009044ff217de114" },
                { "trs", "9f3c82a782f511c01b30cab28ea5c6a45fdabe220bb58f37a0b4585b497159619b2088b328c04e26ffe3ca33958a4096985a8a4e04e15d34730bf577f9b9295a" },
                { "uk", "942175bb9225d4bb60cb6928091ee346b8017fa29b8bebbaccda75df975fd531dea6e60b7debefb25af352715c42dd9e286f2d1be928fd20b17742d69d560b7c" },
                { "ur", "de919f9800aa1793eb9c000348b8fed01e03af3f5446b76cbc9516e93be5c8a0da5407d6be7adc6924f3bcba8bf1ab833434b24680d60af23067bc0214d6cc0d" },
                { "uz", "2fb6b95c3b56c7abe52a7fd8fcdab8f2ba9dee909e6647b6b207320651a4fb43a1845943a0edf9953a664f21b5d30c34e6ffafb47e82740f7b2e476661880e48" },
                { "vi", "d29f65ea0d179580c72b8b946a36780de5befb59ca86860e6572270f4dfc030e612e8dac88eb9958f16fdf537b4895941ce7050fe4f335cbc58568435aee0961" },
                { "xh", "b48680b197acd81733c8cd759df6e9ae91d4ac9f0a0c9acef9fa96a8dce40a5f576e7513fefac21b803a17cf4179110b2bc9e8124498c156bbceacef2b3b895e" },
                { "zh-CN", "88ac56b1484d47c9585d09f55be804f4174c4b69afff304c4f1c9415ad190a4adec30c23ee680e19ee97ed6a7366be239e957c5d30dca773b1368e06ac874f2c" },
                { "zh-TW", "60de5205c514a78c55894ca5ade88c0da5478880870912b5e5c050d0cdabf7c0c7c30d13f0ebcbe967b10b5ca7a77dda7f21630d26922d9954c92bf88b554f4f" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/130.0b5/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "93b283e911b9b92a7f68d853478181a98026aa7e7e9d18842d1165d678780c18cdcd3ad69babc69698eedc0a6dc44a0802b0a63a7de810c779dbd76b679dff3c" },
                { "af", "e5512843640b3573eda7f8f21358861570f6cd4e8375963c50daa6a0aad6d2a3db6bee7bb5642339e9930cfbe82edea787155e4a94377067adc0e53a07bbc77e" },
                { "an", "ed8c64189ca75678eb8719e992e3215896c5e41fc8764dabcd1e3793e0d8d1a22155880c7bad1741aecb3fdac064d126ff7b67d710e12fb1da30787fcf081735" },
                { "ar", "890bcd384e8409456817fa55b515736ed55880e1665413deb63473e667cc9323f3047bc5328b62a2ee5dbd50152f84acbf3357bd09e73ce232ecd5326c8cb709" },
                { "ast", "471046a35f0446dfb8c2c555316faee9bb64948a8f4227fb982375357cf8108cdb46bb4cbe6ab5917e9d002b718696929d93f657ebb6a798afb14ff2ab78de01" },
                { "az", "49f9a48b36bc2b3903f2760e3a4897597861b56ffd4d6217efa40c1b683d2a96a608021d9737395c99f94787fc7084fbf7ef5d96fb3ec9133125e81df43ed576" },
                { "be", "ea60b2df3bcf18786d151a204ba0ce6e339b5761a48f658850779953d201da69fdb8dbc42f947d55e8120ebc1d04bc23915f06a08ca772b20e89e6efe48d69c7" },
                { "bg", "a205114644ecc1f233d17d0f1060cce66d78ba487bd1e6eaaaf266d0b964a0d1e30fea3a3a8fac2e265b5331d1a212e150952abc71bb663d9e199a0a67c80454" },
                { "bn", "e9f23f9718e0e26056a7ff79a06229fa67c35655e82ae64c418a54e287f0fe026baf89b2b762ca06ec64cf6e5d9751edbd418e87a5298d6a741d67ad4d6f1f5b" },
                { "br", "b3893a5eb8781926bfdc5d937b71ce5ddd99c05ddf4a47ebf0e12daf3f2dfb744d3d64283d443d19055362a4753e7504580505d6fa8fc92fcd22a3dc96c8c462" },
                { "bs", "0fe1f11a923dbf195d9d2cf2ddb211b6644109796f8b0e2c4cc691e1d38531c2e91693103dc23c80902a5654bb60c3ae37d4820e9784dc40bdf6cc1cb5bc871d" },
                { "ca", "a4382ea41941124b3ec25679fd33eddc7a3e36d598d8891e80c406ae0b92ee2349a07beed2260c5dffc4f54a197713558eb53fa476919e27cbdb4f31e0ed976d" },
                { "cak", "66cb26a57f1d469114586d78f26d0dbcb8dfc74e2da174862efd44c4f87176861d8f96c79541b9f1ad229f179e3a8516a0657328c700da725d2e0da944b7db3c" },
                { "cs", "16b8fe1898b9d4a62544198b7d8b4d416b47977a09f18d4523264fc9d7eed78d2638a46875315d6124b80dd1af4951c21cb98b26d5f032d5db305ad5d096ee04" },
                { "cy", "c985bf14523d01f5f473424c290de280f92c17126daf20bb86bf602f75f35bf8ba5fb040a7f93f95e2d35e993fcc0481e4a5256bbe79246bda8f501cf0cb6cf8" },
                { "da", "ae3703844094bdf7c4d1832f7811a6ed66d343c4092cbae8816d0ff9581887756d83dffd809607c955e37469545720da829d50cb6025879fc70ae74e0613080e" },
                { "de", "850009e544dec13738b79601a0a0e525537facd621cfd5a33ee9fa80ded73fdbe9708371df17d5dee63c598de860a3bd98b5b70051dfadc19321859f80f8ce39" },
                { "dsb", "c146c1c69e16c331a8ac6a95d75b5ff875e3eedbbe006c15f97f4081dd5f2ce49b7e82723706bb27b981b9bb15e8c7810c46546311191ad521a68b35e1302907" },
                { "el", "063aa16114bf6bbcad77e5fa969fa77092011aa87900c0a71c8f0041b95b471e892add490ce93d74b031e8216c53ca399ccf2c8b2682836974b587673025ba9f" },
                { "en-CA", "ea5d8fe55641e87bf6c7e6db7db09a41af94d133a1ff9acd02463b6f0570b576736fdc2e26aee62c393e2555095e2f998fbd50b85b7e1c35a99c65354c358c8d" },
                { "en-GB", "b8b963f0e4c33a8f36dd8548519dbddc41440b2b6417c1beacfa7345daab98ec01aef9d0487cd66628d0acdac876fa9b79f11609731d91864f0718fef7e61c1b" },
                { "en-US", "26aa89b5bef35a4dbc9c5d203ef2b8970395f2de0e6d5657f151fc49c23f3ec2d4b1ad7423255a7c0f0bcc577eed3e5c98d8abaaec88cce3ca506455906a7cdf" },
                { "eo", "3de2ddd013e091726597c3a32a45f2cf3c1dfd30361da610b17037f7bd4f39ba178ae4e1531b7b969da96d69f60acc053003c29d2972d98ed269219866f7d9d7" },
                { "es-AR", "4350f13087f4b8751a589e4e83c1ea4f5efe34f8236d6c19abd61535f8c5af40ea8066ed201f5c9a5f7b8546976efe6ac646965169fe20a528fbccc36f64bcdc" },
                { "es-CL", "1f6f7bc5b66a78a5de7c0bae2546929e66c5376890af4eb67103b4d08be95472e8773cf9b5ad71938a211ce71c839e60c0848e976e042a0017b7679c7d3e94a9" },
                { "es-ES", "fe3f13c809b822e97119524febe9a5347de3ea327d8d8b409b657df90da56af97f064d2976c32bd535496c892cbc6383e38970faf8c01d97f36ca056f34515e2" },
                { "es-MX", "cbecbcae0ab375ee28ef30a6ba8d0d16631c2bf698383d142f3bef8535cf861a06a692d534ea46b45e0304ec0ba8e5b92f5a9b823297664adc72f0a60a420ccd" },
                { "et", "c04ceee644d74c0535b20a71f4e50b072832834a7572b51cbeca9e6691559c88e151c49b6db310032e7e8de65bac51d358a17e4656baf99d3b787a2ede14133a" },
                { "eu", "61f498d974868ad558aecf9e8b18d5976f339c3ccb77375482545947da917efd6319f7f471622c3bfec34bd7f5ddc40b6592a3c94d4216c65775954a76879641" },
                { "fa", "fa121c6a34e0a14946a91abf9a88edf586a88264db59324a0bda107f8fbd2474c4d70648e14700e1264dc61c85363a5356da0b37da23bb50b110bbc58b3c27da" },
                { "ff", "285a2afc61b31b3971b5953c910f61f149972d51e242cefe447483e8518e557d2c01a9ebc8427d89e87a9fb6a645025378978195ebf35b77a9891bb423534ad4" },
                { "fi", "8af2cdfebd335ef29a5aa736e5000c9cf18df9cb05fd9895dcf949bd0dc331d18995d7882a6b5df9ff25f1cf11e7ad22dad4c1fc941ab300c38977ce15723c06" },
                { "fr", "4b56fcbe3888264e102f0a322697f0bda398b54461a654b31c9e182aee53502155996b4cb20104489f3cc1ed90c9b52bf128c58de7dd2bdded5ebf27c88436dd" },
                { "fur", "914266f2e6ec13b01b03b184b7f09834ad9b86301738e9092d2096f89e4812eaa6dfe7f0f2a8c572af816aa22d8288271ac5404842b123505a405a95a77f8f47" },
                { "fy-NL", "dbb61e5bc9bd795aecfffd29e220b791d78b5e6c823c9862c701d73a03a8c3f0ff93c14e2efe25fdbd755d3e987f52cf2d7cbe66aa9b2afaffb537ba06dc9771" },
                { "ga-IE", "2260a1e332f131aa8c2eec085250fc7da6d05e2cf886cef3966bf9906116bdf674194f56c5fc1206d1cac2af3a10fc3c8da2dfbdfa9011f1c92acd10171224cd" },
                { "gd", "423c2cab164bf35ccfaee59de3f6cdcd3fcb1531c51ed188c0f3cd2c6a74f62c0788b67afed9652deaf4e91c57fc8f61a50081db7fd264e9bfd7978eb53eb7a5" },
                { "gl", "c7c6f0c9a4f5661b751a60c8c5159804903495c7602c12d9caa092d977b71541984cf942d0726f6d73d6e974276f8fcf5237e1e02a5fa1ecd8ddd236030b0e38" },
                { "gn", "f20cf43c173323b5a95a6d9c9c5e0351936d71b86d690f4fc04acb14319837bf55d8515189c9a9ee64c3ff1d13363006f84e7d39fba905c9f4848cc141dbbf4d" },
                { "gu-IN", "31d37ed95dc810881531a880d687e59f7cb7febb76439c07f17ac183f67e2e9cb8e9d2df736ffda3151b3ed68986f61c07f5769a007543e45c4e41cc0d20e94a" },
                { "he", "d5a4aac28948de855d72809ac85cddaf44ac8779b05f5f454eba208222293ee57a258e0fd6aa42d367982d89d581121a1ace6d9e7f268e106ae694b6673c7954" },
                { "hi-IN", "6791c4db657d6fd704d45e92f35e7294296711ab8b29f6baea7a3916afda9f2a20af1eae915f223bcfecd3b427c3aa844532c1264b54206be3c80b003036536a" },
                { "hr", "e75d51bd259cc1950ff7f1fe9979e2be49cb6c9653e399753cd9f1b88fe438472cf34b6c7d3223b5686349b3ef3203065043edbecd784061bbf5ae141b17c7b1" },
                { "hsb", "25a4284b308bd9a439dd1af626bc3a290458657f108ef383b06a3b91ee26ca168b5942c764e70b5fa079be8ac06f5ac7fd162fb63d635bd3a4093f3eeba8df5b" },
                { "hu", "61ff85bbe7f371d9c1025b496c3da1d3287cf6584648f332d64f8c1a8deb722fca16164a23fe04ab05035034da226f17ba15b5139e47de5abe7c807ebdea468f" },
                { "hy-AM", "d4aa7642b29f601b338c50f21ae89ed85d05b3e13fc449f620d9f1a5624c91015199e1bf63b8cb18a6df6ed9b740c48be1504b064f2ee32942be7a8fffb2e71a" },
                { "ia", "fbb078a541288d91ea0010a835a7490da53e7de9e88e9114938316700f4c2ede523d4eebf212fbdc9eb063d36fb6c70c827d6e17130d50354f88488200304ed9" },
                { "id", "51185c78528efb93045504c621f51262deef6228c7fc67f836bf3b549fdd582528617f91584dafb7f0ebfdf8754887a0429f722513e7dd0253acf42ca1296b2e" },
                { "is", "77479cb8d7d839cad5cba63e3db1a85ceb276585f15b40f146a19f1bc3554772405c7ec08554affd9b777f43b4c70d6e5d89e3171fb658f36ba82922ad17c094" },
                { "it", "67a98a2ab87fc66dcc02094ff2180d915dd19a798b326d3e9a39faad174302e3a1d9526626e401ee2769faae91fc053017adce12484f155edbda8f8371277100" },
                { "ja", "70b905d2c9ae3e4cab68be19042ae00115980d335e80a68a33fa47651cbde956250ecf8bc4a70ef6a42e17a67f040d332df27b1cab0dcd7e4cb5ac1aba08dc75" },
                { "ka", "ef67389c1211e195f33e31087fa5057310decbdae567a0642cb5e8667503e77081c6201351edcfa45ee03b5d64b526a24bcb37ef5308744ad800005e4dc9f380" },
                { "kab", "168c988c83dea250812a46dd397236266c8b69c9a3ff2d6ec853bf842639ced141122660b1d6e33bc1222c8310342063cdcbb2408521d80fa26c24af6a981c0c" },
                { "kk", "30d4468f395707767ad3745d67189220aa682d7978638d7ec5ac8830f2379796409371c841faa20a0274c055869fd1ba3c7866b1d4458c3a4bda617713807c19" },
                { "km", "d660eb7d9af4470dd46f2b4e1f830cb937d6ca763a98f4256c246360225ba0a4bc36ad5f25c871207fd959b2c71ca8f5175d221117082a9d48bdd4619cc64161" },
                { "kn", "0ef4d0732deabbf60465ab3f3236e5bd6d5c3d876316b3b696165e98a4281399f3d92b392cfa7e6a589e58a160fad2792e366eb1186c40b8e73afd136c46539b" },
                { "ko", "b7ba123cd348a9b187d974cdde6449fe58a0a1a7bf245fc4a819ac3de748149f61855fa8a72f6ea83e91311d09049da12575caf7399efaafdbd48e5f74467eea" },
                { "lij", "63ac356be2db6015b815e73752e50840f0e6ba17755cbac629ed8c7da0c9ba78e5353b9a515ba0cfd83da90e2bc5a9375c5d40306446fdb1e1820d7e6ca77ff6" },
                { "lt", "7d3f88047285e5ef8242ba08aa4aeb3fcb4ca42ad21bd157b2f1358c6924e406c4020fe0d1652f3d71cfae9f603a6d09b26c9fbe9728a38e1e23911082da3022" },
                { "lv", "3774f3d38b3a16c5bc0fc1ce1ce1c7d15ae56b2b75dfa25b2735ed9c698b8f4829820d3093397b4f1c661062e52c83574e2aa76b18f2ac9a288d8dce79154f62" },
                { "mk", "ee27e167dd572525f92cb3a2ae636d116af889c6ae309c8742ddb03192cd549930521db72d2c1afc11f9538a156e835b9f77da7b81effe22892019d603025f63" },
                { "mr", "37d71207fffe1e59cb23b061aeb427fa0f685c11be99776a01ce832a58cfdcf33e263119ffe25fe2b5aeda6e3773945ae03d49f52c57e0352ee76b4b2f74026e" },
                { "ms", "285c4f21dce9c5d6dc1321edb31052bc218a42c18ad2a8126f337f151e7a7864c486693f27bd4cd5eaaac297d62068a22f43ed8f80b3ceb701ad3ec7a4a84844" },
                { "my", "83b664c5b5cf7dc536399b8f15bd20445cc132625e03892e79c6990c7ace7e8b271304cd8eb620633904ebbbb0608040e1db830d74a5de993cdd752ba7dd98d7" },
                { "nb-NO", "d83daff49978f69bcb82e84df7d6d86e6fa00a50703af549000afa22884b85b553673e5d541396fad76d42e5db2ff5755500d3705d8d1757e7a5bdc091fb8260" },
                { "ne-NP", "745c4b3b99b560f3f31cb95f05b6c9bcbcece44a74c1ae913277661ec1d4b988481bb4aab4df3ba663c9318ac75da2bb07ce25182af8dd27c12fac8419b6e6e2" },
                { "nl", "8e23b494a654345af73ca9ed049cc3650028141a0fae7af2535c9759c7114f1d089ac3e5e85a55548f02f8c3c7843d03236f923b5ad779deec508ccdf3ee5eb3" },
                { "nn-NO", "87111b8f3e68735cbdf9c340a33da70cb58375810ea8be8324f95a54d4e6fcb0b2cbb48f9a0b803b25f8f7664e29ff361ae60ab86637621c22cea92ae30dbb13" },
                { "oc", "981764b1e33b4a33e4aed9219bfc7d84417480e5895e4e3ea911ac030291fd038f615a6daeb5a60adfc4bcb94e7b06e4ccbcce25c4d08214ac11a0b333250bfc" },
                { "pa-IN", "5546eebf60449e685adfc884c98f4e72fdb75511b57ca4d6cf6bd302b108ec5f639daed07b9d49dd1d691e376bf8e1e81bb0dd2259284a3151becc053c583c19" },
                { "pl", "a9aa1d8654d4402492216535cb2c484a2f5bbb52a4930a089670b2fefe1bffb5e8f68768f0b39c80ff280e2ece2edcc759a3593f9dbfc0fe21bc7f77dfe41005" },
                { "pt-BR", "7fd32b35cde13be075156c065a1e2b572ea112653893077598f71cbbeabc60e31740c2a6a162c9e21b369baa045407b14f6a98fce6feea36e55739cfd3f51761" },
                { "pt-PT", "e8be446f0a5576808836e600b25652f61a6c44906c182ac00a2ec9914a09dd0760151c4561f2a46c0583666c915b014c03cc57454251ae0e33278f02d390844d" },
                { "rm", "d15918320633228a666bc90616c03fd9134619fd2825eadec934f057b993224894cbf60af9252bac9a101e1457a9882b01d8ff31e02f7a22d675f33a4abf3e33" },
                { "ro", "e3423c9a670477b4d85e7483446cd4d97f89ddcca47474ab7a93ace546ea4d1725e08ca6d69d36d7b9705488df81b6fedcfd867a723413b7889860b8b2f6bb31" },
                { "ru", "2b48ac6732895cb17e160cad04cc4f9f9b19a6c95eb267947d8f02837aee0bfc7e74cdeaa7381cb0a227537ec859e1465b9f05fb9734cdef0c67f7dcd7878a3e" },
                { "sat", "fd21429bce50646bff91e70e65fc31a14c829dd2cf6167e0921a4933c2f8028e2bc0fc06059e733dbf12cd9f8450dd2e427ea4edf3ca35af3a55cc458a690b5b" },
                { "sc", "90616f7123ba1aba039096bf01feff1ada989e1b316017a02622ec583024fefb505682995a7808f62e3de0ffe45ba09b68479f10ebf45f5d8ed78c50c42ae18b" },
                { "sco", "ce4eb38bf446816e2e0ab7b5addd1223c8b28ab123553d4f2546c64f25e9b581e836693684024747ecbca6b102237ba866522512f74ef502d3349114c18a4b96" },
                { "si", "37fd862d9be7d8a1e276fdc1a7982faf253a07d2ad0496a3a6914e0c5809895a0171141978f759bdf019dd35b1ed8eb26fca646775888df4116b8df4015ac303" },
                { "sk", "93a79ae46f17a47abc29e826ca3f9919dca4f392791669545defeb117c4e15bc5d4bd98b1fb339afaae0b00a4238a4e744d482211eb9093da67b6944491eccab" },
                { "skr", "e03f48237a3c19270efa1ffad7e9531ac5bf63953f8c69f2c746b72eabe7ca119f381240a05e3052ad1fa8d67d653631d64b81d28dd7ce044b382f516e074905" },
                { "sl", "c235c92c01b7b7977022b068c05432bd0e2d8f3965a1170d7217f5f9d50b4ab073e965586a457c9cabf1e31d909f8c8a9fb73715723533060c55222af3ed3404" },
                { "son", "1ef2b4ed62e5f80e4f3942b1a0733cf958da60a13c33c3a2b98e70fdb623754e605789c14a16ea3811c15a1a937ba878d3f466f93aa46cceb9a954e026372a1f" },
                { "sq", "751087da3e2eee2411b84d9ff593f54bb89645cacc69ef79ab7db0c196cb4da39b6e0a013e40db3535ad04dbeec81c55f124c736fe5f99c0608962a2d2e25ed0" },
                { "sr", "2f772ae93abcab47139c15fd2d899ed9a8697e62da4876e213d60fb79b356c7ed4330fe79c6f2cdb6f277fcc996626beb4afac62e54d9bfd3769ed2e0a5f51d0" },
                { "sv-SE", "2ca100f7307199fd1720bc3c7f03d1479907cf9e0a8764cf9111bc9a01e9fd89109b75cab0ba092f171c958e6f91e4d048274fd1286677d4dedd9b20d0593478" },
                { "szl", "b05ea0cf83e4e3f4786da188d59e4768aa5f545bb3a675ef32d677efffb17201ff9d3c5a80856fbe233ee2a6e1099dcf48a0df4a99eef8e53b576a3152ab1f09" },
                { "ta", "bf3134763a45a114276f6d01b8b30581f5a5890b9a7099ce23648d6276bd20c6437e1544e9972e0a32f7d6154c685c5bc127e80d84ed22e638a99f973f82dd52" },
                { "te", "c7dc8a3696368190d7d61ee60798b1bb314d18c6f597aa20be9e543d704a3b15cb661e8bf24dd4f4d37c7de1fad9eee2f2d8532497cccc1f4ff56226eabefec3" },
                { "tg", "71c8ced4499b9e9fe32ca94e61c608dbf9d201443dd3349a4be91640cc9f6570628106a7d55bd516d06240e2fa2d015d7e2e387d1ca559dcb885b9171fd78efb" },
                { "th", "59816cb0732d626dbbfa5d191832a79620384b02af323b953a5a65c1e75ab62dd87284102a632231cf767113578c1c630025aaed8f0ec9ae219717f082460e6a" },
                { "tl", "d66f2cd27df60d3c959d36f4f24c6a1ba3ba659c2288b0300532aeeff315ecb564ab05170f23c278de2691d7bd048c43596fdcc9da81c82f99de62dab8bc2667" },
                { "tr", "b8a90fc208f37a5c76a5ba471475797b387ccf28221da483ece28a511e7859007c9699c49e877468cb9673e502341bb9cd19027a99aeef444911395747d390be" },
                { "trs", "d54fc9890850f019eb0999fb474da65bcbb75509574a0eb297c76bf8465bd798a294830a116acfee4982cfa010c91cc1366523cfd99b158aef1428cbedb7c84f" },
                { "uk", "814f5eabe25f55265b45aa086661abdf00f23b1fdbaf9ca58199deab5d0bb8471ce67ef22d3034792024a6a564bbc3c9ced0ea556c0afe33fc4af41d87920cf2" },
                { "ur", "43bc12bee6e4f4f5fe4441353d16e33d26d1aca0d849ce41bb2e49b53096adfe5d7a772a16a79305dd1129e078fd59a089b0e34a387d4f2c5f1b2b8a077e96cf" },
                { "uz", "e3427c82ab7190c87890c1b53044de2bc0e0984033b69e30b6b02beec86f95973a4905544d8864a873834c4fac0033839f630f6ed57a0c3a7d01c94135f487ec" },
                { "vi", "090eb42b40204b08286edc88aae6e656e72b170ffb0bc830b893cf41b59a9cbda959b8711347c3bd294a253a8dd7d247f3ea604db3901ddabac1be6a7bcdc18d" },
                { "xh", "31eeaa90482506388d97bd7b5972a8b1c1d6f2fc3c8c6df18f68c22516d4759172fbbd86171b75dd46214d5fe89f73053bde141f5fd8c6bf639d93a22c1b1e77" },
                { "zh-CN", "4d9f6b97e5aae7ed39cb8a082af0aa49263cc231747848f89f5071e1413fdb2d43eb3202abc85e60043a9c40b7d8519d91774f43522206460c032a1521214f60" },
                { "zh-TW", "1928ab29e8338bceff0852d7f8060653fd3fcca6fc21641d61891d2934cc27442f8f2d88fc8c771fc48dc27740ccd7e32b1e70e4e6319748139c4b9bdbd4c310" }
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
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
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
                return versions[versions.Count - 1].full();
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
                    // look for lines with language code and version for 32-bit
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
                    // look for line with the correct language code and version for 64-bit
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
            return new List<string>();
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
