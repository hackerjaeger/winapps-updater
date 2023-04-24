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
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.10.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "4961dce263ed275ce6cd7be6861c9db84a201945a0e29b2349d175da3b9b399ed408d842f8345079327e1b142487a6fc5cf98b92983a425812766b104e75d519" },
                { "ar", "a53573daa8462341fa0cb1b70864153077845d28a6e2d563cd66b8a3f75c6f19f46405c09026806c4b156d1218d8c194816543fbf0f0bb6210b328d5f776a182" },
                { "ast", "bcdfd9fcc1a11cde44ad2a6789233af8f452a3ef62400aa52900288d882c6710a48909008b8f7b15b7ee9403aa03ea79c1a5f2f83001ec3456992c38c783ac88" },
                { "be", "d44086fb97a1f02b36e8018983ad1ea626f0dd1ad3f503791da1fe11aad8ef8b1b6e1fa4f9faa3c5691206f7da8a5b6354b5ad2530b51cebfc8e5d46f26def94" },
                { "bg", "9b31e97ded99c294761ab694e80b0e0032426be48d255336c8640bc3bd808595d4aa743129ece379ecca731aa022ef9b3692259d385005c57518388a2c680896" },
                { "br", "158a995d99185dedeb0178ade8e43c250726f937d9c1322b14d264f921e4c0670def5d6a35e80930f4035db7d9f557eb5772fe29cc44659886bcc953cb5acc66" },
                { "ca", "1d1323a3065fa1bd57fedcd99d41fcddd1422c3cb3457c074f6ca921468a92120ed756574bb6664a209eb374866beba07d9f59b8a0bef9aa7e437309b4375143" },
                { "cak", "2e0114a73a05e95eb010d104cf5f4a5dd8bf58ecca1b12e4dc2f63f7ce199289f8f31c209550c5c525d4c61a9a903df4c840b1e011356834a70c7a3cc65e8e24" },
                { "cs", "6251677d7f86de799f40021c3ba8a0312142f8922e8e24c9b2a9305c8b4116c6f9c0d0c8c00e20e2fd5cdd91c5c185193d1266a9520d796b94adbf89dee1e382" },
                { "cy", "2ed9acd4e399704ba84dd5bad63b755c383fdc8c3fd7de5043ed300c5ddb82c7a79992554a9876aa15a46d97619c94f26b476cf0a37d084e5a8d4428079f65b6" },
                { "da", "0d9afe38133b409ed53ef1492139734a6946d696746af89a4febc2d098188ea3f47937ea9071a539ab08854a0cbde919ceba116602fe3208c13079e2986d1bc9" },
                { "de", "05d0de42951ebdb341388e4fb96eb4f304940e7a4f7da4107c30d159b697085797ddcf7e2d70a48bdc11f4cc1e71175cd5778a4fe77b30557f540e2ea97cc457" },
                { "dsb", "92873af42eba915f39278bc220315f55358b8caa5f037583927d86ed1741799150df1ad320eedf208e024eac65e58cd27f4f310b84e0ff9d89e98ed01bbd5929" },
                { "el", "d593a2c63ad2f84ac4ef3018eab578939e4834d5ec7dc25ea30d0cf955898fa7903e4b7e1f9f680b95c26a7abdb3d9136a9143d9a5be58f1ca58284073bbe00b" },
                { "en-CA", "ab92319d97191b27ed55991b8993a9b1cbd2430f6703139dd0a067498d6885528670321b8d88e630c67ef603452e79aab71365b7d134ef975433fd6a4e456977" },
                { "en-GB", "372bb34a6ab2bb5612b9158e4c848a924e32b0bc94566a684442141b80b4f61c04d483ebabfd511ff802a70a83182ed171c8afa55825828bb30fdc6090a04c23" },
                { "en-US", "c4a008c57db761cc3f70f3bffd33a6f481cdfa303782093a63fb83f8697f42896e44760cf46f7e4a30cbd1eba46499f8aa56faef376beba4cb2828ecb061794e" },
                { "es-AR", "e7fa3bfca98537cce91beab3a3374631afbabce1315f93d9833cdca22c0e03f654b1345bfff7964f1d57c4b5c51d8f0203d0a7d69d1db700187fb6bb523ac13f" },
                { "es-ES", "aa617f92a80f5a5c824e473011867d3c45da1629565deb387c66c2bb3426941585ba7e41706c1f511bf4e4662df9be4a853f64331ba1f57b3a579be995317e81" },
                { "es-MX", "9e7ca1104c7f17cb94c7c2dafca38a62c7dd10211b4327a8b7bbaf10da898ae1227c5261666ebf567e8dddeab70bbad6182371602eda0362c091fc81d142f32c" },
                { "et", "92789ccebfaf5ce53cc67aeaaccd3059ae45be43c0c74b44597f78e8a45ef33245b6b448731cf0c0eee4770d5d7d4727109657b0fac3cc1452a7ee5e4409478e" },
                { "eu", "5f4cb1b4cd933f182ccb600e3003f9b50499112c1a1a055cf4d2d1a90bf8328d066eab75d42db3d62a8ffbe229add2dd85c2e1fa5c25997eb5fbad5fe42efdf7" },
                { "fi", "e9db67b61d62211ad3a7b259c6d6cce286efa3fb41a3001ea70b35b03ae8d204547424f287b43c16a482d842bde72d8a6fe703ad40958c85231da86b7791d77b" },
                { "fr", "f662aa746b4dafd84fbe1985b8be1914543a55161e8c05f4bf7c26bfdb1e142a0f3274adc19e5aa912c86eb1635dc2c26b89f55f6d5b85eda93d75ed3b229d6e" },
                { "fy-NL", "722a1579ef5e6398fcf219023f7045ae9a36b2165203907ea514f8d25efa002ca30a39f5ee396f814c1218176081890bf14f9680444ad873521ad68bb3847bca" },
                { "ga-IE", "ddc30c2eb1e385dbc28b87a926ea4e1204740733329c75b4a989913293ed9d7bf12ebc2d0f639291aaae18b01320251295423d52051ea312830a2a64a51c8f74" },
                { "gd", "6e6b12cef0d6ccd781eea28d1566a563cc68500d39ac028af36900dcc91081b9b8064a358c7256fbd00c96f4adc88adbb93fe38bd433e9904c7113df95e46914" },
                { "gl", "2def4a98ea6eafa732b58d3ec8ea1fbe8a3c88c57cfe600741b8ac6d0ebe949c54ecbfd02e55f35a1bf84b515b9616ea566d068c7c8174427d480bce6882dcf6" },
                { "he", "8f734990290272d0e901fb03c8adb5854397f63db108cf9a48b2b9acc0e99f461badaa9d894aebefc7b13dd38fc1f20c96ec02b5eb79c75bed095afc8d3bcd27" },
                { "hr", "17fb9418ace89e6cad81649f9b1bdf9f48964ab0418571cc0da1423eebe23ef51acc58f7d07ce29fd481befcfbf96ac3e243363c73b5968839d8c9214e323dd2" },
                { "hsb", "eb11468d5f889229a684e1e95a27d1f0b9201a0edff2f87b8322bff05035b7d1c7d0f42c5322328e51629b172ffd64ceda76366892a6355cd549a89634fde976" },
                { "hu", "529f24e52b319c2ac087ce4d58b2ff38ee7ea8ffc7d1ec4cefc1dea878cc0b43c2fd32d1dcd0c5cbdde753d55025fe3fad096057127d05d375d4810384af2bd8" },
                { "hy-AM", "8fea3f319a4dd545f0910af3b15bff70b9d5e2e0a8bff1cd686506ad17dacc6d6466b081c67646a26e815ba1033d4609ef23bdfe7ef2b501e36daebc1c81390e" },
                { "id", "74984725602e1df6202d72e2b1a70623feb5dbe9a89c4e955f8e4fb77aff69b908a0d45f0ee769723d8b4b590effcaa4821845145108f478c3b1551de078e06f" },
                { "is", "4ba924e933542c13d9b1fb1865922df831b758c3e2307348be2d9b74356b65c825b3635973c72493689ff0ea2f4324abd7de9193d04d8ffd504bb5ad16c10d8e" },
                { "it", "d2dea6b28ade7041bf752f36e55a92ded84d5bd5c97d79c29bf06e1562bd4ae5a0351f7edcede5006b4ec1017dcdef7b72f729bb4211700784d8446ef9a98187" },
                { "ja", "34058caac07cfd7f40f03e8a2078271fdedd812db21e8f73062cdd06dca699c3cae36300fb02194df30a9fd224a2a142003efb006b345a839e32c6de43afb59a" },
                { "ka", "59f43c83b2d2c6f36bdd08ef035755fe17220d82ca029ba73c886a66ea50f18fef10073166ae1ea62fbc3df278e2e7f0d03b5c90028429e49614a60d15367d0f" },
                { "kab", "873de5d5682213c5bb86c1bda60d5a1eff3f5f179f2dc056283e52ca7a5f8fb446765abfcd083c28b4a12c0f02f0178682b350ce64d47296905250637d446b85" },
                { "kk", "543e3fc7005b59ad9cd6a138859316bb9e2cef0513d53ec4cf92ce9b3a760c10ad090f20566207ee315ca1f8d2f00d014caf1e99c0c80c1516b343b8302275f3" },
                { "ko", "89b5444c93941c9f70c7a5af8bd0cded1c578b885707e6d007a8976f485af1b1805189efb3d6480620f9e5513c929c2ad13cdb4ae5d35c64052ed678f9863493" },
                { "lt", "1b05196d81547cfff2de1b471bd6f0bdb997035824ad061f4470f9e26a566d9a398d255780fc5339e5699790fe9f9bf6049b2228b912bfeda0d1796f6f42bcd1" },
                { "lv", "3d7aac002365d633986626e6dc276db9cd28dcd1b6ba3843521b01c52b44cd275a1f4f0c3ac4466177ac93f6128272cdfa9ae75d33ba4fde47e093ee70267d97" },
                { "ms", "5cf4bd790c3b7bb5bd4663acaa23302f16a21de6c6fd1e04b32e470e4076c5e1df44948feee7b731b7664d5ee58c7f1566770d14c9f9b34440325d10d707faac" },
                { "nb-NO", "fc36845875a17cf8c318eb141339561ac5f884cb0ce435821cd6dda075da4e5efbd4a42af5afc1ed959a7177ba144955ca4ae4021c0e683efedf5b5a7a3bd635" },
                { "nl", "8a6b9b82d673a74fe331b75f634b0bbd56666cc5dae6438e20f911ec41a6e7d9d74ce3a01dd324d0a2df53ae5688e0a783d337cf2a26907f8fbff509aafb3bbd" },
                { "nn-NO", "33e750721566f6fa237e5c24a67872610458539983b858d2eba3c913c14d8f014adae95c5bdd617db1dad9de17245156ba2cf399bbb5843954e9d32075b43d65" },
                { "pa-IN", "f6074401f6523f144a764ee354f7ab7ad163c1b10a95642cb4c4d489d87b7d935533f46ad4a250071841ac684dcc4f9b662c9fdbc8df9710c403a40c66b8017c" },
                { "pl", "f5676368d6627374668fdd79df28fc2ce5b1690024a526e4c13b989816b74ad2f0dc8fa116203460c8dfe4db87f9cd8c779d17bb799d0cf58fa931cc0749ce7d" },
                { "pt-BR", "2b6f268bf2bc3d22f66b190bb4136d517db464b154e1dcac129b679cf6aaa2abd9af4a0ce71d586d20887cfc28176291f7cfb9bd2a11a0bd097089127ae9d3de" },
                { "pt-PT", "bf10b2cc38d165dba15c7b4a1051e65b91c6e9fb599101e0df3293409e0a24eeb80ea914f195ea153c9b14d040ffb1314d96d26d16a650f6e85a268ad6553741" },
                { "rm", "ea7e93027e483bc0ea264607a747cda17cf8fa6a25037374c1fc53fd89cfbaa48774c88f939fce00254afef7ff0bcfe19a3dccc63f5d47e0bc02bc46caec735d" },
                { "ro", "a36ca34c61b0262b296635724b4f8568e9126fab2062d6b80f847e00da637115f915a5b6d35db012503554fbbdc24c49ba2f0c1763ca334d75a90992175d6bd4" },
                { "ru", "58b2dece94c01edca0b147925600d1a5be5fab3810e6ae409e3a145cc3ccbdee8fb17605fccbc9d22b060da16fc452bb3191367b1d573676da1720e1ffd9c5dc" },
                { "sk", "0995bef6cefa9d0ce93a6dc6c127b5433098071b3f3ed6a5bb7b3c824f3a58dd50c2a525aa2fec8e9856524cd1f0b1671ca8ae25fe6f0e773db2f98e9a7da2dd" },
                { "sl", "8743df71fd2528007646f54cc75b001a443473f6436b5b09298da64323edf6cc93fc619c35d143bad0cc0c64427a92b20ff72dfd5cd65fc47aede1115ffb9b4e" },
                { "sq", "9274e1b6de7e936283cab96ccca595d368d9cefdcd7e498c4c8084187be27e3fe162653156244c17faa1607d01599e4043b57948fc2d612990b6789757f0b262" },
                { "sr", "7562efb2f4ffb7c46daab68cc0be1f60c121d31a61096ae6486fe36cf9f3d55c13a4a795344bbcc6dc90ee38d3b148d12e7324fcf352c45785f727db9de8b1bc" },
                { "sv-SE", "868c214c9953a87f7e4185acdb8aafd62e76036e5c79e2261a6bfbb5422af77d7e7c8bf90747dffdb26db8b050ab179adc51749bee2a8b5f7dabf363daf25dcf" },
                { "th", "a9e0057395c4199d2dbc93860c7bc5e79da19ad7590ad363e3eaecbae9a87424eb05c59bf9848694275506ee71fc84136c8bdcfe6c00ddc55e39b88af316d233" },
                { "tr", "5ed690757b2b5fde7429a75cc3d67e1ac4bc72e1de20718242ce5f33550224a78e95d75806f7001d5238345edfd8a1480d1f865cbe8c9428c683d27f6a6d7fed" },
                { "uk", "d5738416f5db56150fd1d11f6e13ac37462d064135dedd7648cdfc644a72fb88f98a00b37b4456e08966ea9a7ed2bc2c02d55770a44c9bea70721672baf79123" },
                { "uz", "010f243e43eed89d3a252ade523d2c253dd16383b1e24cf6063ac191d221fd0d31bf9a2b9502780cb70b922b1eb160c587c5eed72d52b4a8e1a3adbda5e6e62f" },
                { "vi", "ae98d8a9de700d6a779c8e67efc40a753dcc136c9f606574a8bce719daf54708004e72cd72a4cdac85108dd7db0d252882a9d83721cdc51256377fe4bd496c88" },
                { "zh-CN", "13a9aa3f95f634ee406b3f3e567ecef54c020cab32b4993b620b5c7941bc50b9fbc485cf1a50e2033de9d8bd0373611bf1b675a0223cf9cb6a978d3e991f25a8" },
                { "zh-TW", "b80c6ae59e9c06feca3b83707fc35814bcbbc32e2143acac27bb6ebed5fcf73ecde87aa52141d911b57c21ad4bc3de6a437b98941c391099f7cd54e258935b0f" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.10.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "eaef85c5ba0e8fee7474f942e0e1f8bde27be9c8e0705707453c4e6308330dc35b90982bf4b0f710221a1443eb02f60c15eca0bd1d25e4a358f927ab0efe0639" },
                { "ar", "cf91c48db675cf34b3534ba62177a820e9938a35ee5de5b39210a8ddc225b6c1521c3db347f94f6254894a3a38fafbcba6def0f34194ea8d78f663540f13d28d" },
                { "ast", "f3687143e219ae41b88681cce05f2a6254185f4ef2bf96f205a69b2a51ed27ad1b6d8abbb2919ff1cd650a415af0131d7c74fd4334b059df1dd97d3e4203b158" },
                { "be", "72b960381624f002dfc33aef3d1cc269de69f485b77c070bcf12c790a254f1d6fa5a369c2b711b00894ca3e495e8927aab93bed6a5c6cf19cd3cfd81e30f8e7b" },
                { "bg", "e060dd9ad514d37a353602041f77177703cddb9341bcfd38339208d03cdf80eea91a24dafb2706d57ff3d1832e7f52a41c70589a023e4594d8dba248653db24a" },
                { "br", "ea666586394c1cdaf7ead28bc5f2c7cf7178123533f84e3cd88bf3914e1ad1cea0ef572b92abcb164ed1be4e8f4cdf74e3f0fafe9a64da748d2cda8894105ced" },
                { "ca", "8031c34080a70a0e74b03691080ebc57b1f4a3e57a0240ee0a2dc76fe50ce15f34b2ad41691f07849686176d2dba84e30b4c7322aac0999aa20ef41b97139977" },
                { "cak", "966c43313afa98254952e70f5481c81bbfad0279ed37bfee43c291990543a4f4b7ac8906a969e1ac00c0360848ff259a329eac4768d880a1fdac6748792ea90d" },
                { "cs", "449ec297203bcf0b298a868e2033977f13abfd0f91e1a4e286c0613f61e7099faf606fff2ee2edb9b349066e1933565695af81ad926ff38db074933ef5e050d6" },
                { "cy", "2edbed4b34d44f62e9dd94770fd9fe7e979e8e9bf40abeafce2a42552ecd894e1699cd29209e6d2bf19b7b4cb6d3afd8cb8188491da20a850727995a8d188c77" },
                { "da", "488f04345cf9421fd812c015bb72d554400105257fe418ceac6361090eeee3b953bf8083144351568b1dcfd05db85ee39b6e669e43ab06f3627da747529580fa" },
                { "de", "f08000b13a11ff02de468970c3bb70702cb96b708e855568930dc216851154c9386b4e1e147d418d87448c3841c4e0970683ef0f8af8047527aec1bee3960413" },
                { "dsb", "d06fb8fedc326ca3f073a782e64516a01d3b439b1fee6fc44050e9fe37da4f25d4556e7b8884a683e6fd50f80480f91dcb6962acb7c22c468ec71f23634857cd" },
                { "el", "273e22483e74f8923320de0993c26e430e32b478eb3062ecf2aeae27dc1a166d09bb846f3191e2709f94065e41bb717bf7282db0ad6298172510c96ed9df21e9" },
                { "en-CA", "d50fb85f516dc85b1e390b32de4a5e5254957a26cbc022c6f3bbf0cf46205feb844d487a609aa141e94bb18f1276a8b303b17984f239d4178988477806d9519d" },
                { "en-GB", "d61e0a0d90f322e3defd83a8d6a7755eadb55e8b13cb29b6af9b3f079923e575dd349ad07d2a35051e70e0b8770faff5e27c2d205401e67836262b567c8651d6" },
                { "en-US", "652c706a1bad8c679b184b731b73b3e65a8f390d9537451e5abc7b3f4ad35d25124e6bb868ea83139e0c1101eadd7c73c87da094eb2390e89049fd706c954e7b" },
                { "es-AR", "ddb900ee55693e676fd5f218f88811180a489e84e5181cb05a9b9f17073c889e9efaa86f6975fd6714308548ca7cef827d116dc92b825b34eb803771b2b6a805" },
                { "es-ES", "8f546793ded0167f738d6588646fd5b4d9eb9fbb2b4b543b2a2172a83a39439aa4d5bd6c69feb9b0d50da05e5805ecfea973c5ec66f690f4641be495651fe90d" },
                { "es-MX", "55d5cf80472006a9c8086a1b68e2d2b85be064dc38c71539f1dd02dd33623e127ab2c7e0ab2a086573499556df7bdcda6da9c55ad501c25c5442c1e94af5a75f" },
                { "et", "4a2c24a50548bcf121cb7d11e6d7a249d2e3f4c985559e4b9323fa6a57aa86a8b416548fe5c982bbdd90f67a9b790d073e5e16298e8da7c7aa4880440a248a3b" },
                { "eu", "11c73d02dd997834b17f2dd2c1c70644f5088eaa2c38f85ae8a5bdfbe3324e34a227146eb201c91b3195a757784d40ef8a3a1e9a4b57a357f5d35f08b4eec988" },
                { "fi", "99574cb08962b94524a62c36508051ecd145819ff1c800ec7af29c50bbedcc465093b68091ee17f1e484e994b99d056d0a6f47b333f1186a9e1630615700b329" },
                { "fr", "70ec314b3412b078a472e911d300c3973e7b174dc454a75d840b5d1bcb74f8746f322543f6461195bad4b6cac8da13212851131e7fccd4a89d2f2bbc6f50ff9e" },
                { "fy-NL", "ab588946411f9ec8fe1b38727bac08228b7edb0429bb7a66598206fc1e37590b23e41b110b139e0fb363415c0fce6f193e194eba5c49d8a79a1a8eb68ff31c7c" },
                { "ga-IE", "07468bc0292f86bc8aede2c160fe1b17b5bae5c2dd5d58ffc3b0fef471ae3b579a66b82316dace8c225bb1edf1c17569ef00d430b4d4e7a07a5a38efc84799cf" },
                { "gd", "587175a8ae95648b30cb580dd8d59be99eacb17b6f2d44c0bfcced3cd5454ed96c74601c0f4124c6234883f6f613ca68447f4076b313f164612db55db10e2aea" },
                { "gl", "740a4744c2caf77492e6ebaccea154117b3963a8481017809f88a014df6f3355b51e04ca1ebf2bbc28c08dd70db87548529b4248f3b8a575b7c620e96ccbece7" },
                { "he", "8f2736acb94d9d4325bed86a8bef16797d94483e7f4ebb6a3ed18fc19dfe6eb73a17fe40f8a7c089eb628e69a5c517bc5228b34569833d0b6fe5030b0473d2e2" },
                { "hr", "fbf89fff0393b1d7025d254c7a4c078ef5d9f8aba154db5007e5d4d5655117eecd74682cac4544c7543d6fed57941705461ae40b05742c902be8334acdc61f5a" },
                { "hsb", "bb4282816c15b58975f1668b6e6ad2677d1544118c8301e75905216e35de1bcbaf15a2799860294f7fb5c6c7e7f4c71811cc39d4cf34468f885f3bbfe3791e29" },
                { "hu", "dd3fd7f25b187e5def4806b9649f7be5c2276515bc63c68247ed465f05e90af67180a66e8fc641e77899aa9261c6835ae264720c4f614a8a04eceb6451a93c77" },
                { "hy-AM", "7f927e53b3ad99b0eeb6faade95b99ca1dd0169ed1fdd554c5712b4dc5e5597b0ea76c8584703ace67d6b2bc97cc581b377d4e7fe506962819efb567dd847d19" },
                { "id", "3cb463c5afdc38a1a565ec642fea5836462567a1838243ad7cb04ceea49ae84bad471fb5c515527e5e725d4ea8dff166fcd512646ee516dfd36398032cb3c598" },
                { "is", "e69ec040f9a0eb1d5a143a15989bec7d08af554e7e87c397e45a2ae451a33de4cd080db7f7e3c11c184c52dc1266c224517b6446fc68b985218a41eab5b60846" },
                { "it", "bb542957a6f303dbbdc239938f47f62d0f90ea870a05b94c23a4186345fd1760384c995e3de5ca1bebd5817567b7ed0c594e7ece83f40b821bb613f848a71898" },
                { "ja", "2efc5b60142dc6078452926dc11762dbc06b45d829528a3e0f3180d1384fbfea750debfa6e09849289b396a87af3fd71172780f5322b0a8790ec2ca9016cbaf5" },
                { "ka", "ae49f15b84388940ed5c7e7f0d2e8a52c1ad0a85f5ff834f653c605af86802064a8dbb2c142e7c533496efa75184dac6d9e632790f670b6a2428e9d7fb764cd2" },
                { "kab", "83103a9bbf577cbf9c5d021938b8e84438a4456437c44b64df8880690559e7e57e1842b38a4cfe5cb9480499a916a922cecbd77c6ce6c58a1a40e5e3f2045075" },
                { "kk", "4afb4a2078248f1a661559eeeb7d01daf143b1358f63e0ae3c571cfeff35df6bea284a0e10fd14e6666339419bbae724d34ecfcecd89865bfbaa5290458e0f3c" },
                { "ko", "4fe79bd4b0d9da8122d70f7a5bf5e37e03d702b2629875181973d4ef4660d3d3522482376ede2273e634eb48e9dc0ba536e4123ca401d2192ac0105bedf70800" },
                { "lt", "f8ecdd418853df10eea8211affd8d42315e75dc5ea2c119ded8a2d99b46c9e1c44c68eee74e7690ae3b4f902c2d85e3fbaf79f804d3c97bb00e7e74d92ff78b3" },
                { "lv", "1eaf494f92d7a5b1804e8a0727ff5bd1f4ba0e6c3b9b1ecd6aba3b792067543c815c38d7b765d0e3b208ef41cae1938186fc707803e429443eb50958c5bdff58" },
                { "ms", "04063e3f180bb6216d6e152de7050695095206767a492638e51a70443cd3754f4a3f09dda97985012e2f2b43491901e77ec3bf3433867bfb5b4dd8a5326965ab" },
                { "nb-NO", "d4255379ad94393e1311815b56175edd1f5f4738a8ec8fee360876fac4dc025e64f1ba4075eae5a004cb200ff5ff44f540e884feefcf263348c74c57f792bafd" },
                { "nl", "ff4c98f5258f76ae5f93fbc9a25ee2c25a504c7074f200424fe239e00543ddeed575441ca735cfc84bf8a75b99e6cf92c81adbf81221069d2abe5e4bff6ce3f6" },
                { "nn-NO", "6c8bf40928272465b6f4fda467008973165e4a8c221b6c2a75c23c2422c646a0a412a3f028f7dbad0cc3c1a9ee84f4cfebc8bbc5a7058a08f7d40e5396ec7df5" },
                { "pa-IN", "a5565da42af41abbdd3d800c946ee2041bdcc9c8f41892ccfc1cbe73144af8d58bdd4892c28ce2e7ef078b0910caabe1521a07dc99d4ea6b68dd1d703b983f30" },
                { "pl", "ead0c1ec06262183896b749c4f423be6f991ad51f92ad7632ec23fcc557b636e851e514efb2317262ee0b52c3eafe4ee20c89b6f67b0d3b784d803ce1500ac53" },
                { "pt-BR", "767340ebc178f619f285ab58544cb004320961a68a4fff7ffa8c56e18e6d0f3d01f38b01373045a090e362212ac79f55c5ab33b72f61b4c7be0489daca508758" },
                { "pt-PT", "83d4da0c8381a153469c8d66b083234346dd6c358eff06d3f18796f35e796568a3fd984b10a88ff4c411988160d14e4be3639c6a3d89bc33cdb83e3afa5c0e6b" },
                { "rm", "53f53936b49d82d20dbe6e903160eb3964de7cfaffc336c709abc1dea68382330fbc4ae08cf752e25587007a0835b77fb7c80a0e16527527823389bc33a80de3" },
                { "ro", "b7f1646be65bfb6e2f09658fd3599f0b178f56d1730030c4a0f642316ae4e3fd5737ba55d13c84d594166b135411c10283b71a144e1b2232e9cac945c413c51f" },
                { "ru", "c9c4dde196ad3ebc5488fe0c7873799297b7ffdadb4fcc92a01675de54e3fa93b8865612a125d3bcf2bf79f5b27655cdf3a503f4c6c6627581c8219db5895cfc" },
                { "sk", "965c2872911c330448abe05385afbb0429c7c9e72922698aaf1d8aa5e38a794737f56320ce3690dba00f29fd8d97262dd6ed51660494e799a8dc46ea3aed2597" },
                { "sl", "89d8cb3e64b9cb60edfe19eda5395d0371238b58f9de359b95dabb5f005ad75d276dab5f903f051912fa78a7ba734bf9f93fb711172656c5d61f246e273ae452" },
                { "sq", "7e4da8e33c072e3db505000a27aa8fb37f8b926c4e09b84460d513b88afac9e77159945f70f0e50e58ea7f31325a6cca56c68e52bae624e63bbf312522f65fb5" },
                { "sr", "ea8b29667811e213a707e1dabcd94aeed99896917fd957c72b54789a51065a2b759795abbf9f0ee09ae65cbc7842f19f9e612cbd4e708f17634431944e3f0d10" },
                { "sv-SE", "a2782c538cdc37ea39cddbfc09fdd30cce54bc678dd5237d209322b24c751a344bbbe20600209789bf9095d2ea11ea3ad0daaae2983ac1ec613a418fd8378bce" },
                { "th", "9f63e1e4ff269366be892b0a7b05bee5f6380f9a2022df63e79c5773e78074dd55ecf61af493e4cbc5dcf326f9beb9b20b5badc89762d6dc9fba617e083610e7" },
                { "tr", "cb34cd977032d3321d13b9579356f4f2a960c41e6cb5f85ab5063f21045bae8476b02b957a19c7fa6db24bb645c770f1c3752bb1448fd3b9258871549a05d62d" },
                { "uk", "28306f1aae061b794e1a8e8b52d44840ad9e667830603d16cecd470f43ee1a53ad9a34de17e3451a9568c0834eb712155cc827412d6a2ed5d1d89246e745f771" },
                { "uz", "cd8146a8cc2a47a9be74378c48794ea2e3f35cd178394b00b71f86153b450da96a554191b6be4ce52a2007872773e6dbef7c5350344f38f88c0f3ca9691ea455" },
                { "vi", "420a6273d8827f40917a585721fe0fdfc065b28a96b959b27eb92a958bbcf251272002f8d4323e5e5fc2f3d6a5266065678a7f840ba21a0388b369f72c50d625" },
                { "zh-CN", "99a9accc3c4c455cf28cc86667d982fff2ea714d77b9e53e137d575ad25c17f516675391d3b43d7f8debeffe8907b21b735e1936b80dc946380e9396019a14d4" },
                { "zh-TW", "a5273d189501093be79fdd30e354826af45d28ac2ae36d2e952f86ec5625da39f429d51a80d0a4940f5a2d25262020edc9883a6383b66303a6e21a4a0b6d1e4d" }
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
            const string version = "102.10.1";
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
