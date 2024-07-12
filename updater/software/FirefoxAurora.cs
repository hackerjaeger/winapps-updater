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
        private const string currentVersion = "129.0b3";

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
            // https://ftp.mozilla.org/pub/devedition/releases/129.0b3/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "bfaefe783173bad7ab78ddb0c5c138afac7a55c98975ad3e7a9844a97c3f3206af582c1ca9160692939b292b1578912d5bceb8639432204c2a4690592873b819" },
                { "af", "49d596f7fe8782b1a37eb9a5a546d62df25521768d9ec1d50713cf33a43b4e96ccd135815b9ea447c886e08bffb4947a18db50781ede7a3d8dc9dc5f3914be4c" },
                { "an", "3e9153a2417e6928b9a432443cee2fd0780a6d641095c23f33b768aaa5999142e91f912ac37ddf775107ed9e73b7ccb04395d43f8b8926382b5e17494372e375" },
                { "ar", "84cd8ed0ee5cd0e8baee41ab034c115c7a8ebd55606375b41538f7b420c0c31dfc82afa97b900d7ed402f1baf2a235f53c49b849d1be2f02afba1d1b28552e38" },
                { "ast", "0ae7e09c130ae01465e7716efedcd166d2641eb8f7dae62bb26acec462752dfb11a99cfdb57bd2d2de82a6e4a66ee20acc4380c0374210ece25b0e469e0c3237" },
                { "az", "b3c2320176827f8eabd77ea037b2784ea782e3adba100730da9965c2bca9e0380e5406947542c0119b5ca5dd3bca685cbfb3fee5f6fae5c48d139d076dccc32b" },
                { "be", "3e65a289f19a46fb315089c160b436fea04aa8eab84c0024adcc19807adf2257280c857837d3d51ac31090b515c734611f9a80e320745e36756a3b78cb8de75e" },
                { "bg", "c52a3e3f48eef960956a1e158d393362b6d25109ffb336c350094eb2bbe4eaf7768e349f37385f60ef4e9659c671db29e2d0ed16435cc50edc7c53d9e39bf4e9" },
                { "bn", "b3cae447a2c362067f1de9297f7e656ce25cd63d415936e903eb6f20d7b0114e2592250a65e931abfc1eaf8f4caa6ae3da77034aa2356a4fdd73fae6d6422061" },
                { "br", "0c57fc18102e9f0c1cc4a1fe4714d670c0b0576ab948b960f26266435816e891b992884e396e2e4d6f511113a8b00c01dd5c26997f84ecf68de7d4d97b36df8a" },
                { "bs", "7dec4702672d8520596ece4c5dd28d4d446875a0db66a92b5bf9a8892a4cb01930bc55e2dfad7113f7ce15582866bb5bb63be192b7c7c4d0b57617f35acc077a" },
                { "ca", "53291105db2a3d429d11c59570cad42b860c244740fb19737d37ac2008d6a281b752b45025045b6c0666d93100555fd7580a2d79db297e165d0f146eaf71d66a" },
                { "cak", "28f272fd6fb1c2cbbee153d7cce7b6a499ba89643a6ddac8fbd54a4f0f974e7a32e33d5b7b9316971795300886ca620f2e3516b59bcd16ede527f50b6a60e347" },
                { "cs", "1709dc6043e81705d1ef53420959520bf217335261a583b86a52675ec1dfc7d5f57fddffb4eb90f273f61977670ef6585532fece2b61f73972dafa1dfcf7a6c7" },
                { "cy", "be3fad29206d52765feccdd642db71234f1b5b7bc6894a7f87684e621a3257f12d2cffb34cacbff9a884b8934b19b8eee5777f256327e4c8865a45024cfdc428" },
                { "da", "dff1569154c254b809306cab9e276fdc62f84db2fdc82bdaac6e8519476a5c1ccf2d95266c9e9962180b1724593d018e2f1a2337e56e662123c4a1138eb0c365" },
                { "de", "dc79112e0c602150f9a235b4f6d03e285726bba9f5dbb02b0fbe9018c437aeebd0c45b159c834ca64ca5798083756249876b0686053354bb2446a6fc74baf091" },
                { "dsb", "8b509508d9f66ef34d20268af3c02e5a7968a7fd125de942c8c9e930d38bfee2b9441fce2f4a02083cce47e193ff5495ebf2bd80fb151280845e401010abce00" },
                { "el", "3dcdb4f3561328bdaf34f225e0b916899967e3685eb578dfda85157d82ebbf7cae722a489c5e087441e2da1dd90b546cd18a2a7ff508932bb4236b286151c290" },
                { "en-CA", "c17329e1f991335d8c898922cabbe8dfe309943f4f3392dc2b96ef8c20e8aaac284e5cb340742d7d675430ef721fae3facef5ec704a588851043863e1c7e8893" },
                { "en-GB", "85d2c3bc38914c7b23cec581aaa30fb9b0f4f79272733b97c774812e40a02fc6c936aa2dbe9c29ea9f16c111a4a14c8135554a2702b431fd7a446bbc5399ffea" },
                { "en-US", "4ff2d3e8eeda4b27844ecfacc42f4b854f8b91f66d4debd438ac39090c4a4ab54b487f1c2836a76a8689252c4c16bc97ec54c2a6d686eaaf143e69dc9713e610" },
                { "eo", "5b6bafe0630a404fc794f5f75c7246de0dfae3f3f584f2982e166c466f52258094b7dd887a186b5bb1b1ed7a15cc28d81a11986d32284e52a9977c78667ae13d" },
                { "es-AR", "aff674f45ad5378deca57eee3c137f5ac551a0f3020edd2fcd984bc0750671dffdf17da8ed107e379d5037c0f27f9adb5efa83cddd23b3a1f0f19796470fe015" },
                { "es-CL", "2bc5507c7231f1cab2ae10da9c5cc102aa8fd0c7d174dd600c55bd966a1834aa8ba9fd994a9f29bada363625f32d2561072c28c023a34bbf3d10894277fe2f46" },
                { "es-ES", "246e44bf8e59ba7594ed86962df5f2797a40af89dd4c1441180960fad0942fd7d8630d47b4ec2e121065a7464d90531041e7838dcd83b336bb1a9ff334f0f59a" },
                { "es-MX", "388cb58a2f6559b2d35ef44477adc967a1f126169fe8f583ed83725ae445130982a83e1fc53d5e9d4a4866544625a18e5002beeceeb005ef61d3db219567ff84" },
                { "et", "1d3a72e9395c60cefd6098575e2ab041a48a629e10bc4a69628afd743dc3c4448547623dd048a7efa59ae21db858f86a8b1c49d70b1d9a7a1bcc987fcad36f56" },
                { "eu", "64292d94dccb1720832b90535f92e8c03669d6e2bb63485a6fa8b8ced7e3f064121e5942b6e1c358096e9903a852c53fafbee73aba67ea97880c45542e2df38e" },
                { "fa", "7bf2afdc852db384f33377050d07d58db467dd62f99a925f094e41c9d5bd78a434a9c3a454fa3ca454179b537bc20ac800aa553f6561ce946786e594653cfcba" },
                { "ff", "366eafd80713b6a947c904123470da7f07216745075b86940f4f300916aa402b8fa578a9816acfebfcc5d9a457e4ca2537859b519b35e533d7438ebe0e00e938" },
                { "fi", "f6bab85aa0ecb3d98a9e92b7a90e5b39a6f7b247fc43d3964bf51a2703dc55c2caa794170bf4caa1c84bced82c33687ed94e3ae15a81c04cebea0b6577d9ff62" },
                { "fr", "dbd663c37c4c6d142a046a71eb5add5d70c3475e371d626d3d1bf9b76dd40b0978217c36fcef028f752bbf67a791287123c0e265ee891b5d812a662fbfea0a0e" },
                { "fur", "f802ad13d64e68946f38eda80d1b93941a384cbea47a8c2da3bcced3a1660d21e8bb5c7958eb6e1bf03334f2a211c28873556cb29e012474a783b16822a36213" },
                { "fy-NL", "3dcf8a49f380da8d04bda3211c596f6ca5b909483ad9f97480a7c417568517d7ae7a030844ea16b904289d19408521c01a80266bc4f8af0ab7d81562474577a1" },
                { "ga-IE", "f60af4d0d64528b1dda6c913e96e73729c124451733cd280d2357980453c9b8b7e6406bb148f5e1fa4fac31bfc636e2e5eab1a5d4689346c4b0d9d397f3846cb" },
                { "gd", "1c53299988d137acb13424bf2892ebfeb65a0d169e2ec316ed07804930d9da1dceda66d81dbcfed8dd0b5983ea3d9c39f9a4eafdad913f3754eabe9e66b4984b" },
                { "gl", "5900a638e8aaa3ae9be2b3354a8d0bf085896dd60d39c86713c5b1ba53080bf69da6f7b8dcde7ad6dcb5c366727ddcffe7405647cebf6695b75834f298d14cec" },
                { "gn", "6cec852b9301c871b2eb5fe82d8e33da493931c53064656c3ab9b4d1871a14fa2c7b0ce302b15d78fdf70e83b312d42b33bf421cd4eef6972899db4df1203939" },
                { "gu-IN", "38119bec00502ca42bc2520ebcb1fb565e3bc68ec8fe2e1943b7e31eb2e31410b8551cd64c6e901f5c5ef2d388b9bea75d9454d9216dd21fc133b23e0ba9a336" },
                { "he", "cde953a4eb498667415c8dcad1ab6afbafe6e9d9b340050da7f1b7fa349623a1056970253d102222bc81b3797bb118e9da90fe8a1af36ea03e9339fc11aef0dc" },
                { "hi-IN", "eeeb6c6097c8883b13f623e26c24f14fedb18dc975f795d95e3d9eeaa9f45ef840d37a0136756b439120a34bbd9a55259044b7f54532fc36c9d6615d7174e93d" },
                { "hr", "4df35e63e5a6a03abc81ccdc80d8fd4f8177d6a9d4fd785e7c3bc582ce0ecc36eab528623cfd5618d6cea74e0ec3a8ddb80a39da49f33d15b035d42a2bb540a3" },
                { "hsb", "9e8509fd24d241640558354c646ede5626bcbdc01ed5e0e379d6e6df11f141686bc6ff5a087359b59b41bbdbb6d38aabd9c17eb2cb536fa33c70f69e35de0582" },
                { "hu", "b49c6a613249ce668c34c43adc8cb44fa15ce7b5503bc0279bb819df299682fc65e4f9c43fdd9a29d97a6c105394e8ebcf8b7dbb7adf1c17d5839ae537581d9a" },
                { "hy-AM", "fbc63aacd788d077b1d2049aba2cbc8922648db7dace2d71d0b94e7eb00595115c2269d6b7e8a5c04ac65b7a1fb562f45b7ffa6972db1077ecbe984a92ee792e" },
                { "ia", "043ea7f85e57fff9f117c1537d1ff71812a0310d532d841852a8641a75a9caa91543bcca4c5b223ef498fa335a6e99da4ea30b0bf64762a9b50d4691a3d51ec0" },
                { "id", "0267a76a742129c4143e54f68f4d2e416ea8cccdd7998d372c783222692f7ce9591ddc90f18d53d35b6f16e63bec083fc12786f6d073046a2311101a6308741e" },
                { "is", "c995f0129615e32307dbb6b34c26c49549130c09c9bbf973155628f95156541f49a1a6e3598a26bac8ccd777a03f146d800466b9fe38b4c8533962fce3f89130" },
                { "it", "70cab18a1161eb9dbb0cfb3278e69fc863ce1ff7fb9b7e8dfdf585614205c46fc658156790806479bb22303e14f16779a4a9931e82e7264b2ca21b20ce187e7c" },
                { "ja", "fee4ac9273cc088defbc3f00ceb0205bf0d0cca4a173db00adea80c5474ef3b1582ba2ecd2591b612d16fd03262b073234c9d8025994ebfe6b96846e0e204fe7" },
                { "ka", "d4ab1d5aea48568ecb68a6ab336f4fe778734db5209e3565f036fa218c6f116f8ea1636b8f9598b91dc476bdf7edd90780606854722d35291fc7cff594335077" },
                { "kab", "a80fe11110af977f133a0924b950297200430f59d26f60d830bc47d333fdaf3a978a371e8e9cbd397c08457c2f6264df74fd207cc38ec7a7ae26cdfa105751bc" },
                { "kk", "1230a5e57469bcabe57c876dfb867f4d33d171caaa6185f77c760c4ef97752b566f484cd90b7ca29c6686de41d057157d92792cac6936c29bc36a4b265a48b8b" },
                { "km", "315ed12791090961dd757da34dfb79d5b907d9219e8754f64395cd777230fcc402f1c604a3477caa0c21fc9326e391c6c355e0378e562122f8ac0108714da7b3" },
                { "kn", "d4aa2ffb4f04836d63ed6e6cc2e2e8a14f9f31e953d20692f4d485ce94236d0f2ce3824af7e88a79623bd1cf0085d0f4f415a62786c7c8e7d9966a0f5c7b5b6b" },
                { "ko", "9ea67364ea36c7349709decdbcebcc4ada240fc5058dea571f6bee292e251a85bcd0badcbdbde12f0fc62b6d5d1bb0813197c88d09cc1078738f50543cc4fd3f" },
                { "lij", "a2731e8208320cf8dbc3c507d3614c3e0d32391b6a6da7f0a80fa967cfe1414a1838dd0b16ed74df694471d362a78cfba1d30dfb7d4a067a4fc8f058dda38347" },
                { "lt", "1bb7f0f05af5cae873ed2c843feb05c826bd5fe56b28356c7df24bb6f0522a9b41b9e8aa35ebf32e909dbe9bd9bb7801cbfc0ed6a2c540d064f5934c5e0c510a" },
                { "lv", "90132f55a99315345c738edc9cd0dd7214b57cda88800adfcb049d354d7d2319bf2e03447f2827cfbddf7727708960bc0110b2fe07631c6e3fc136a5a285371d" },
                { "mk", "4f090495b064a8fa66cde151da16c77ee90f53624a143109f41607b756a14553cd032a2a991f10ec3ac50d95106803dee0e06cc4d9a05ff7e6b2ff7048bf642f" },
                { "mr", "c795120262dea0674c0134ced7e86ebc57749f7e25c3a9a15cc9fb2854d2eec4b5b6a89afd6f572d69fee745b4f8fddc0424c28698e73af33d6755e31bfab8ed" },
                { "ms", "5a031a4a0044815452b3f89f14c9a13f67159d11554d4ba52921c1ff25ad7af211d0be293ce1eef3cfdba187c26d2bd23d901a25ca205a2b8357a6178f54dc5f" },
                { "my", "0104d384631996cf34a28c199fa933823fe8de079b2114748d7013f45586f1fa960f73cb93727a845d1d92a2e35866f2db4aeb975b7b1531576aef794b28e0ce" },
                { "nb-NO", "23a5d8892f00a28e0b6680daeeadaa0a3feb4bad7783220cd8fe887e406e5f3611c2ebfe7519b69b3e61ed85547425278ab80b78bf70b0ce0157e6fbeec32418" },
                { "ne-NP", "b800a03d9e8b3f49dc72565d7f682fa39f1f2ede91ddc178cffee4e955eddbff64593339f3cd9fbdd113c5271205a0c67e9d3e7b23348962bd489b2aef8a82d5" },
                { "nl", "3618925204ea83709de1b9357937879b949f8e22b41139d56391590f8cc2a7a2e6eca5169f282f07ecb91de38fa578bc819d51c4877dc4d99fee24868041bed4" },
                { "nn-NO", "19db532c0d467eb7c2fb9501abfbad8fab350b50bee39f88b6de8682e1fabb6bb84240394e3b173202354d54cf002063a9a2423088b06ccd52772f0305161e18" },
                { "oc", "cdd9f99cda037596bc19c0e7996f2c846399f5fa5e241d01c70e1de0d7580a251543e34850f4bb5ad14a147127ff07f8f50596ab43e9865e3af1cf4e819bc293" },
                { "pa-IN", "5e5f5a2b5e760c7d9c766d220ff54c8a6261dd765f3dc94f3d7d8084663ee316f3b1313b412c5cbd0e0ad48df737d4759b3a95b0ec62619cb04b2c4a773be5c5" },
                { "pl", "ae69aa6b2e8f55d5111a878ef6144988a61f1ccb57386137ff0b97e051705e93cb4659bcd9ba6ee88e6fe70d54a85249c410239b284fa0b6ec3f41bffad8588a" },
                { "pt-BR", "5bca81c8b674a47e582bc68bfd3eeb4b973be07c737d17032ff759d8008277fda9ab6f9ac40eccf83235177c040a99feab25c6ac6575c3cf8c20cd760e067232" },
                { "pt-PT", "425dba4b1425c46003d58bd77cbc4d7b52226ba6996a8cf225da0b9b355b37131c423857a5d9b690fde4a94a70c707eff47787feba401a817f81d76cd616dcb5" },
                { "rm", "294fae4f19dee198c3d9324049eeafa840c94346cb9cf4492d9f69dd51abbee5bdf3bfa02cc40b253624f8d10291c5c46e362e1ae508b9d0f4d82db708f23062" },
                { "ro", "74d0f992dc49e99dd59dd4af430573a69bfe7bfb43c7746361d515a5c47d2e69ebc25b1b6e31eeeb277b9c457b0bba56998bcf230fe41e30403ca9637f678925" },
                { "ru", "0634ec5b8ad7ab0e1270deab3efd181470265c3219307a06533c5b53ad6f1d58e78977f1d984d9a96db3582ac4b8e92ee467d214cb364eb539f30861e1307439" },
                { "sat", "516b1bbe3f4ffa796951d0325350bbc41a12303f51db447339d92b6b564bffdf38bed36297a3a890ff9435dfa4056acf1aeb076fc81d3d1914cf8d1ee6013985" },
                { "sc", "e46a55f62603660211a2f7e08e94b21687715c6fb695532b1c707f1ff31b355a46c7a3fccad140f5fcf40771c70c7bf294be8b89acd453ee823768b54ac208de" },
                { "sco", "303c1360b34c7eb904240819eb18f358e75aa0a915d6a4aaad9c50913abd1318f4ef394a81bc68a5ddd7816a0ad9ec90ff9b9d1a0279ae66d5df64161bd2b694" },
                { "si", "7c1a94b097b9d45368f276036514fe032ccf96544ac0eae9708998eb31ce07f0aeeaa061b08ff68605017b3e3bfe34cf0bdb9eb8cf189d351c5a8897e0ba6c5c" },
                { "sk", "38c2a4f188b07b0464a258a6d0ba824d33ad110ac06c0079eba402cc6e68490d5976c0c840ec0f7d79b68bfdf8abb491ba91afd52930f640dec6b656f07c6536" },
                { "skr", "b25e6b767568bdfdc3aa5780d27ba6eab1e7a223ac08cb1a4f1cce318759747c3272632e4d185d66f2fd2a2b9dfa0cdcc6ef72791ca18a313de8bce556318df6" },
                { "sl", "4cc3b8c504b12ab7e283226d903d396d70fa0c90278df9b002f89dad490e97bb889cb1a8d195bbd92e5656f476b5f5d4bc32f5e2d25263b35ed3bdb0b6784621" },
                { "son", "21062d90c6acd50155b85df56d32232a2acd5eb77d7d3aa116e26a3ffb3094c1c0e595da8e63912861c3f2353d874058045c597ec3bab7dbe101ac8776a97a97" },
                { "sq", "895701a15555178c543b3fc562cc9129662d6e345c77a87b08e512d19e757796170a0fa3525ee89c93bbfae7b324a249fde6d4891913a92ec2a2bca1be3ab7d4" },
                { "sr", "012a78f0bb4a32444bd28210407f7edab122f30ca2ee9e1797e990eb81f7d4e5ad1a70f86c9b7102372874c99f1059cafc74e7096b837bb120307082705e39b4" },
                { "sv-SE", "eb5ddc887ee236b5313788f970bddb39609713cf2997de6735d158814dfa7c99530803d0429f081162071ff5648ef44cf27034360a6358c2e7a29caf5b47af45" },
                { "szl", "495b336373299db4fa7634115b7e04dd9710a1dc0db37aae7360f2ea38b86dd66d024567aafe02c80c074ba005a6798039522ed61533d948a7ba127e1948653e" },
                { "ta", "e35d178743e44e58f8dfedf512643da7c876efbfa37058c1a1e4a3abd00410e55a5cd8e2c8dd8859b694b92a5b9550fd30f2fcafd133d9d6543a9393a80b4045" },
                { "te", "18547cf0a5422e9ca52ac37b76fd4fe60768b7bbec03f66a5896883af98c636480b768b2de3c7c0af02f0962c6ae8447a0bdb89c5cd42d2149a145b63ec79cda" },
                { "tg", "7729790ec7f51909444e376209c2cb40ce2b265236c9aaf04aa4511a1a9daeec40141e8a88bfd8d0080babea4e1e882fc4b9dcfd106f6bfb5690e957e2607c44" },
                { "th", "0067049f248f095c82d6101ce93d15c9d32c6165b2b46cdf51ac76f98906572d7a8de1cbf11ed69b0858c661f16beb6bebf88c6242cf9e70e0a826667af4e8ac" },
                { "tl", "cb221d7c5dc3d4ea680887eed9ef617fd7ee8ec398533998aef5199a2b054148d0a7e163ceca103bbc4dfd900b5f7106b1c0a187751a17f162a6538a8b1d6a1a" },
                { "tr", "e4f24025a1869716a8ac3e0945476331c4ef0fbf0088a3169aeeb99876103608defc87b9350e0759d9c2754ae92af2f173e378e5b047bd4b9ee7db44b370d50c" },
                { "trs", "b21914ccde1b1a29d5500a6ad315ce510a2f1187fa103eb2e92dcf30ee2b30fb277601dfc28e3ce7f482e201c4d0eccaceeb7b4ec713f08949c49945fe5e3067" },
                { "uk", "4e48ecc488def9bbf63fc402e8d49219a81ebec2ca526ac09b162441e4da9efc9883546537649648c8acf512aac01bb9e616b0cec9c72cb32142b218502cfd28" },
                { "ur", "bf7edf0f9f58917196273c4465a9c21a88118cd3efc457c6923bdda139638305bf6d3791b9c5a41fbff02d88e452bf18bf33b29b1065e6ec86460b8b224e3090" },
                { "uz", "222dd12688f9a694a0076dfa3dc8041ff6439e2991ecf9691579443c66625284e2353638d6cf88632e409cdd7691e23e8729f6a9c55ee1e7d290ecf9912d3e7c" },
                { "vi", "1016aa824c0dd4d1e97b04993f28bafd20fff02f85f8f9429cf31b8d8be5f286545ff7763872050179bf30b6765c9d457189d7a83355fb9d2712a59ff8dbdb91" },
                { "xh", "97c80e6fcb658b88ba438128df81466bf8b038953df717a44f66cad79164a74b6656f3e0360f83d02c342860de65ab2ad238c5deb6f8ff846df785d752771808" },
                { "zh-CN", "fa6136531916d900b0c78d8859f7ed6fc57cf6f48ef0115d707e95643307311b4a7a811e25152ac2e07281fe72ba7a729e6154c821acd162f3aace6c5b2c20ef" },
                { "zh-TW", "d233ff1dc6bfa5f38085c50ce1795575278d14a2dc1653bc0397ae9266bdd4226f5968bc894dd04c9581f01974fe5f0b8511444991b32af18d4e13095dc7bd6c" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/129.0b3/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "b471f8ead49471cbcc0973870c884a7a4733906939e9a6e568bc8b8fb1c27cd3a9455dafbcca9b66c39be55fe5c9f879bb875510ced35bc4fd8ddc145ef828db" },
                { "af", "2ffa783d7b1ac9dea9e63346a14c6e77450cebcdeb50ffbbabb626ea266485c1c953044304bb0f7fd5c67999f3437013b6d9c547cb1518af1c910fe0e28270a7" },
                { "an", "effc63ace35f12fec201a246f53a0a8ff492a8ce7c680a273312751e4a24921758bb890e6e3f96f33c521a3c60b76c6cdf7646b55d275cf6a1966b47e137d9b6" },
                { "ar", "27869424d60410bd9f7611ec5a617755c87cf11b2618df8af66a128fd6ce97a4861591bd313d4910b1b8fcd2013fc72d565b4874f31dc65ab303feda96af05a0" },
                { "ast", "eea927e9c9e7305f4cf0b840ede8c7b6d5c68af06e3c60eb146b40424ce7f6650987c82b498b7e2ae28d4e1f4de71d4eaf480a8dfc23c84d8a96cbf18e4e5665" },
                { "az", "51003875699621b7ee2a04104f9b9fe2d49a22abd94e58e00b5e87ec002f284e32d5403252578ca892cece596252cbe8762605fa679e22ad6f6644d6c568b767" },
                { "be", "2ac21e09467bcc29df5354b69c0b25c608e6f3febbd686092cceb2f40773c232a2a43e3a51e460e17286e4cca4730abedafc558c874212eb5f96286ee00bc7ba" },
                { "bg", "55d4e37978afec6dc21f88f55c9f415d5920b022742e534013a9bad716c9f5c62bb662ac31494f6a13fc654db38ad8ed8fdfc0592773a4ea036d6e8edca2e357" },
                { "bn", "d1b93979dbf82cbcf6c5bd0422caa211247424bfa1c58fa98c7a3a033498ef02e0820e907635fdd7561d9f870674877009a61df6c1db6ef58a3a29f0693f343e" },
                { "br", "f8c056fc54f86d8539131fafa2adb767e9727faac37e32bbf14eaa22aaebd60cfe65e643032563a2e85cdb37c326ecf5d131e7dd101bf6f7a1785e5b9ad09238" },
                { "bs", "58e664d53caa6453e140824da4b3a827a2c1578f4ed37e067f6f104cd546676a394720522f685ae594ffa950b8b7d91e09d2b1a14600eeb6e8a3cb8bb66878a8" },
                { "ca", "bc5a9fd527b1a0d6bf4c7e7de43e038d0c132bc83e1db10b689d9e5114d6beac3b876ac2548ea16e22c3fe9830b60296122987f40901135c0c3416581e96f4a6" },
                { "cak", "7c89a1ecc583a9c9dd0dc8694d34600ee289b6fb683711316e8b19ab8a1c25e8c502a7e952f43d3772353cf0523acdadb2430a99b55851681228230d5bcdccdb" },
                { "cs", "feddf25ec200476395e87348b3a053ee6bf08e5fe71d356904467363a52f82af53542399635c96fce41f116fb05a2960748dba07ce53dd9b145c4263e3d46cfa" },
                { "cy", "766c1688ef53332d309c554d8ed71c032f60e5d1ccb84ab8e2b57dc0917704748f648d9ac4bf006e1afeaae2e6151eb3f7c32e95aee8ee9c021d8261faa2154b" },
                { "da", "034bb8869ff3b76e43158b17fa84d44a9e8be77aaf310dd5b0a407d51cba17d4ffdf8af6005a3997ccae9ef1867f75538265b3025950d7775a1d84344bb66006" },
                { "de", "282c98243b41e0acc30515162b74a7edddb9ce32ae94b1a6c1ad2c93806396b5d26b5a745627bd28f898ab1bfd9b265daf3cc0785a63bbc6f90b92a00544765d" },
                { "dsb", "c202889bd08fc4c38e06467ea64545d672fa7a85b70a7480a7c45cf1d58d7b104eff07aa505d9a520f99ff556f056545c7b9e6fc18c1f2d16a559c8032e54c16" },
                { "el", "2315e2ae79a5e642a6847995eb872fdba7dd6e1570cb8000e0261d60adb008583e1c5b827e7a1c8971ca13492c0b923ea565d91196d3837f4160ba5d06b76d4c" },
                { "en-CA", "324a33199de2fc46ee175e9396f6bebbd664080f5168578d72847c734c704ec4d7dc74bee044167a776181f67cd4c8ba1441721c845a90ab1391d0a9833e82f3" },
                { "en-GB", "4c784a4a2c0d3af68b126220881a678aa24336ad453bc85f1c0f2b02aef5fec1c6c7b87733912e93d2e7cd8ec10c47a380fb40ace1057cba1651b29b2a99513d" },
                { "en-US", "9aad4cf34528656eb7f94f89b10272f94bc6052ea5ab4b3a44706d719b97a9d468050cd31456b6da29de657b53c91f299e4f2d52bb632f4a242c265b0fb6993a" },
                { "eo", "0160265a2ce1769b426ece67a8dc2774354792ca260a004308559597dc5ce0c375e4c790cd66d28f600097c4c51a1269603ad554b1a926056c7e7d73ce50e2c8" },
                { "es-AR", "9e4ac7563385c19d150f435cd930e9a2b34bf2c9facd9b64db605f1ed901e363ab2f36a5583ae76baea2dd88564b08085a6939bc8c5bed8c96b70b718db86ed5" },
                { "es-CL", "2ab0f7df1450c4c3c4b67f95529b755dc798d746e73b4fa43299fab32357c24bd9cf1f40bf6835241d5004fce794df2d3d406ee4b1f1ad5c7be81e1e41b607d7" },
                { "es-ES", "40ed2d87d1fb4a491d9e87c30782b1b3e08cd410dcf664c267be4104f3e6993f2de479e41c837f6e24d4b8332f6df8d7eefef68a09292c7a09c9afc251f8edcb" },
                { "es-MX", "6fdb65d9400da472bd8bc4ab607539cd5752d98994d703b12a3bd39da14d1c027cb803899f8d2d366d2f3818c07b7522704916b9ce649c80fa18063854ab8853" },
                { "et", "ce457ec5f74a18aed37c6babe55809301b213744b8af7177154690eee9a165e285d5c058ac7f694281a209cb3ec714b1935c68fadd40b7902751400d19c88ddd" },
                { "eu", "5e7d1b33d83fa8b27bbeef51c0e01e783a271c60fd278378e2f9780c1f1954e199a36fac39b7d6ce70a0c3cf6655728f52d89b9812a05a1fff092b4ddfbef67b" },
                { "fa", "5a65cca278b83336a3c4870797e8c87b3e492cb7d0cd7392a9b503acc1deac5e9a723c068b2ec8504c614ce0337859f44dfb84441f46793dc4feae949ed09939" },
                { "ff", "d0f17a50ab2c30fa224cb7dc9976b20f8aec631336b695616b9a866eeeacd4a5479e9536c5e950b51014a591a5f578f237a932e8a81d99b79d3e35462fa47457" },
                { "fi", "689f5ba7fb9836213dc258edc59f6392e0ee06e2a4ff57dbad7ea282a17345264da0f8b942ca07d8277d96ccce64cd87ead5db49913733ae491b83b04b59ad82" },
                { "fr", "8bbe67c0d3d38de3008620b2aa6dfcd633b514e168e5d02cba71720a535183abd8e98751735b5fe95cb3ca65b961e6205d5c2d9a15107c3bb1b83811a265a20d" },
                { "fur", "27648227607cee741f305bdd3002686f83132ca4640e281f29f732372cbb294049262fa72a3b2b82483e481f90da8d97274b7f938c4da4b043945a3107a15eb2" },
                { "fy-NL", "e5362b336acc5c5f70d5c685d8d691c16c6766463a23e4b2cfe6e199fcf3b16b099b776ec686e7edab1bfd18aca76a03f63649f10719ff1293144eeffbe3e6de" },
                { "ga-IE", "c76251962a33c43965ce7c7d5f0f78ab288e33ba54dc913eddd88166f1112750fcf6b8facce0d7a17cd5d66ea1554dc698c983ef1afc0da105de9825b4e209a1" },
                { "gd", "264d3e27fd58c008baec85ef466842e217fe7ff0693ad49d0a259d76ed476e8bcd3e328b5b382f8db1f11196f85f7414af15ecd48034fafae555b972adefb2ab" },
                { "gl", "f7bd6f8b5d7c170d2cce28a6445ef45c27d4179d88325a380e21c2d6f7eb5d13c78850750553ec5bcae9a2762e0b11ad979bbecaa3203b64180a44426149217d" },
                { "gn", "0cd677abac06da40bcd572bba60fee11fce6fb16e125267ce0701bf3cdfcd03f7d9b0b00d8736e6d2d25f4589cf55cee00b1542ebcde5919d63d80d7684499d4" },
                { "gu-IN", "63177948a2a1a0c7cdc4e6560f6d15ef7222ee28690c0e21a00ffd23dbc8d18d6dd628057c1312a10b8444c12bc60e1dd65fb784b43d27c9461b2c4e78fafd92" },
                { "he", "c7b0f6232354770f746ea5888c832e09ef010bf7bd96d66a1e5c2426c0bfb97f524d881e81ae583e0cb243d48d5ed22e2c56dd7012b17654e648e7686ab1e185" },
                { "hi-IN", "37d6cd51687757e579e7df80a9cfe7700797a4159a76a0c45d6f699caf935a5b49ae05bd1a24546fd50d47ed681abed47fa0327e9665e638881b8900d7dbf39f" },
                { "hr", "8c79b678ca41969e07296aad2ab3bcaee19e4822b10d9ff7f676572a2ee5a3688e61e61c854b5042976abfd26aed158be93efdc545c937ca99c9ee86e2bafce3" },
                { "hsb", "2a1273bc028263c5572232caa981665e79ae258c72f4d22d55e3032c32ddcfeba8262a393819a6d24042cafdc76b2d0ab883e1f26888be153fe2b4c3a7caa897" },
                { "hu", "242c905ab9bb7a407c6c3429694f360858a8549bf386c8cd47eaf5226437b1fb534130202179317905103bb36c426afd99fd9d02df0447535912be544c40df25" },
                { "hy-AM", "a259c629a938f6c54061332243deb6f82cc6b7524d2cd80411ce279fd7ead1e783d7d2f124b5f436ea1c19782761a87083c5cfeb04b46002298f0d95d65bce48" },
                { "ia", "44b3d6f85cd7d7edf7244b3d25439afb7e58a30db11b7cae587269151057a1358d288495fa4e19d25fcf4c95af19a9b3eae5d15d2ce7d8628cbf7525ad18858d" },
                { "id", "d4834bd524ead8120e671e8416818bb1fad15cfd16b01cfa57ef92e352d190cd0dd603ce9700ce6d8f27ea4349edfff2c8e58ef368418f7bbd53eafcaec1f7e3" },
                { "is", "ac00203d172f745c20baff113705ae8f278b476760e4d41331d7cbedd73a02f50281203389f299302cb4e857850eb3fdbfc7c0c2c96dc24cd36f32bd22bfa689" },
                { "it", "8f3ff70f0a4e164e8e51741e398f8c08e70fe0828a0bd62706d3c39c622610634745d6403f49b36bf7c3a875b5960ae9256c138b5cd34b382e2d6d5e118e0358" },
                { "ja", "9c997d34ec2c5808ccb4ade7cfab69c799061471fe3b3411828021f10520b9971766de7aa74485bcd255f4caa7cb03b9088f222f948428a4adcbe9c9d6bcbb3e" },
                { "ka", "da276a25f0b0f610fbd5ff2f5d6d24ce5ac5f5cc34b7c5b6698e8afc4cf2991a468d3eea93246a338a893176e2c58c2bc8686ee76f5442ddf8fd9e4c464ffcad" },
                { "kab", "616d36a8b0a0ea870ad547de41326af25a70db6739f7d7b354e6379528d8d14076f888d36a1b73a76bab8298de20c9640fe815b6dffe3a619101753c047fc0bc" },
                { "kk", "78113721a1a215ed2778b7e9584bdcf9e50449048e9a9009d419344b4ecea63a1e131860fc49ce7ec52e9ab0de790ac0a8c985034222bd9660496ae0b96d0c9f" },
                { "km", "3c08787786343b73de4dde93f763ecc983e4480b0a3d558c9348d4f11e63bbb73bbca2157673cc24d30fd1f01a9cf18603b3638ef4d3d66d919324bf955b0a65" },
                { "kn", "5e73ab31f36851105d55c9f68a3008974781684688554f4e4241cf9f48d9926c5c51febfb32ef35f070a6a075c80924e7a494be2f336167b3cc2924abe1b6d98" },
                { "ko", "96bff339ecaa642be7aecc6a6c18163adac0b0bf47afd5056479e3c0a15a089f5f4877d5050b83068945fdc1cfec73eef266c70f027ed7a97c3650adab1a4882" },
                { "lij", "1ec2bae368e73e3f7ff0f7eb4056806b937ab6c92550aac94037793f1e99bb359564445441dd12c1656e50d93c91de4ecdab5264be7149abe2066589f99fd483" },
                { "lt", "2cffbcafaa1496e2ab4595f2298502d661390f00c3c93b9987ae1a6c45d3be396f872896acaabaf8402ad75988826228283fd0a4d2eaa8f27b8af75dda6a018c" },
                { "lv", "1659f7c4e45e1c760547344b4a69bd88968a93321f993e9adaa54756c00c43d3ac838d6e5cfb783f776a3cae855d21f4e7990a18515043fc9895049b6bdffb1f" },
                { "mk", "4911faeb29561e6441994e925360bbe95dc37aafc9120c3e58bc95b371332235d3dc3cd3c12af84f9c442933f3eddaae744766daefec14b64ef0fb683987aee6" },
                { "mr", "e5459d3712a3e3d2f36d8ef32d21981bb8494e8c257bf297ea38256d27e81cb370a332c612dce53f9a9d16259e809f798bf3b9d82deba20f3698f36960e34391" },
                { "ms", "f141e67ed368d0b2b39f3a2bc080a2b60b56813a53395359affe40ecac5667b53824492f3bab8b17872d883d08d2ff25e313b87c69f84be3fa851194d002b988" },
                { "my", "39cdca65a472be07f5cb3b582c5beea30b3a212ed3b694555bf965393950f2064937a089ef3c85d59d108e52f5763c55eeca9f53c39950ad1e61d1ef03249e97" },
                { "nb-NO", "00ab2933ee9fc4dbf8db8a0f0573bb2cbbdb3585406b5a9c57c96ede6aecd02aef449e70598755909694045a80abe74bae81eaa750281c64b19e31cdc44353a7" },
                { "ne-NP", "ffe8e8bbb6e6af909728e90a917353064c59405a166117fbbe2258b1803f25d909dd4f15a91c3e23b7ed58581a8e57bb6393ceab0a8b9c1f958a839a9180fe7f" },
                { "nl", "4ffddd703bfa831afa7504c312e2e707553d0e0947ce538439b20984229c1b343dd225ac5abe41ca1c532534f18dda2592543ae0ce7c2821352d7d6ae79cc48c" },
                { "nn-NO", "d3ac9bc961e559f2f32d727afe3098c2928a6c8a316f268905992d4429291ebe6599acdf3a2c3c23ad83f8530ada35365aa129aecb945fcfa0294dcc4c5c8d66" },
                { "oc", "905fc63230aee0db50c02088071660c829d26b5e729f3ba85e21bb3140023b21f9687bb5575aea842db364064d789c6953315b54d9bb17f81d40d57f02d044e8" },
                { "pa-IN", "0bdc25723fb8133047e68d59e850b6efbc6500892bb6e2824d146d98fa0105d41e38e632a4bb4e5d8da687de90baf44ee01c73e48a2bc18d9cc6bd3208953b82" },
                { "pl", "bf9da950d1fe6af2ed5b06ad3aa94a97256a179871c35eda3467b2a27be2373738cf0121e928a77b2c94f9074567481098a4598d554715c8d6e7ae525e3d7000" },
                { "pt-BR", "34e4176538566288d760d14466a716495bac81753ab7726fe13ec0c2b8cd08aac4231e28a9d76c110e9208cd45b11e1bdaec352e00db1f7c3ca3d7ff5e239c5e" },
                { "pt-PT", "33016236337944532ee6209fd4fe94fb4ffb1abccce3f98b857e389379aef2528e125c9960980fcc63199665653006de7a2d30387642eb0737911af4f2f6f052" },
                { "rm", "41b167b085ff0a6a72e8a16744db3acde14fdf8a81313a56b3b147abf45ed0496dfbfaa112922de4ac54f2872842024c32b89dc07b8d59f33cc5085543b55b26" },
                { "ro", "986b24810c8085a225e2215b65d0d5e815ac6cdc5b8ca204303ab593d55b9c1050107958f7f9c478a237aebe88dc64147062dd08e69305ec0fad49984ead6499" },
                { "ru", "7e6b493002c851781de05ca202d134d5eef17e1fa6e2a9eb3f2e1a6a09bb66b49e1742225881a8ff3634c32befb0c3dc493684f2df0c19ed7caaf4d645d616ef" },
                { "sat", "24ccc1aaa8705d839ba1e5166fc647f607e7be3059fedf25725282063ad6facedda3a90d664aade2ef764d5f618eed763fac1c6770c98f7df6a3dd73b81d3b94" },
                { "sc", "76abd6ac505842e15f1e17c767635b922b17c480ab5bb47b30d5d657ec534d4af99be0dd6b17620e4eeadec9eef00ddb89a4e53eef147ffc37c74e12439ed584" },
                { "sco", "b6d4b832a906dee45aac9661319f7ccd1eec0c92d3edca8b6b4e45b990226c10e0ce7be99ae6fb7528c080b93df717b258d4263e8f8fa97e2e60dba6e7ffd437" },
                { "si", "a20eda4c1451073b795b967dd525888f42ef928af766feff95b7ac0b976ad6b60cf62e874b3f843a2a9086e9ab1f6cc72e71b807d405de0bc4fb4c1273182cc5" },
                { "sk", "58a01ae354f532338392ce5ba9abeedb03d1cd18804fedcac515e27882e88d5285c9dab4b4d58da9152315ded2372674d15ebc16d5242d9b079603e8fed628a8" },
                { "skr", "b7fa9c2743184a6a7f7834979981662ef3ff285a54ea7609824b264da60e2b01cc1da3a6198a3529c20335acdaf75fa01dff39d8a1251a20e8c1ce0bb8656bed" },
                { "sl", "6803c0a38b341871c6d34e408e8a6a176fbbdad9782a5b6b9e8d19029f6f5cd62463322de96827f2d6741204a5e7aa5b0190df8b117b78a866e9fc710f9f9c5a" },
                { "son", "c077d470921e50842e72f3d7c7e3e87df0df7a5f9fdaa13f141671a390eeee1ddb1c2ef7abe8a4fc340183c7912d39794f084f728bb3697920e00259bd1edaca" },
                { "sq", "f43aa228ead6b73ffa664afb5821b269fc98717813252df9963b34d7fe59094ef7fc39114b7f1fd5fcb46fb9d8f6fe633e994f73ab81e3da6409efe987bb0cd7" },
                { "sr", "583b335245b01f17a9a3cd5e529a0b11da2a617d87bfcfc4c132e3ffd71e822c8df1170479edaab6038f9135c519d4b34e505c5c4d1e4f239e179f5664f39252" },
                { "sv-SE", "278d74bd0056d03995e986d454660e1642fde595441371b9257fcb7bdb30c7f3d032f22c49287534fedfe8a9018da1e4b54b33c06b0b5276d7e3383e97ae8f20" },
                { "szl", "091c6e49e4d7a9b393072b8634a86a1f8face3e859bba2940accb19e76c16d5fb43b6c288c232b5f68f91b466c05612bdc9f23eeaef53074578ae8b19f509557" },
                { "ta", "ac233e885c5f3d671c9cd0d97a921b02062abcf8a11a31e3f877017d9d4fefef425e0ed3cf8d01e8ef9f30ecd306b6a2888130b32e7b05f0a35095b767cc1e9a" },
                { "te", "f797a4e397729c5c2b8fd110395a9a9cb5eda6ee7d1524659ab5b86887b77f453b3a617c7fc1240a5f97091e3e2d48567c635bdf0a010824283cd3df7fe8888b" },
                { "tg", "f5d1aa97a922550e4b117e054c6e56388e9bb0911835f8646fd48211b04b9a355a462b1b29eaeb064c8aa3df7ba8f98b3f36feaf3b984358fc6ae98ea022745e" },
                { "th", "edc18a76e13976f7923fda1c70c26eedf71a80893fbb3cc36bee9f7519b7bafc20a0830c018b6b74f2c78a6fa80dad222cb92739d2532894c21ec81a44d29ef5" },
                { "tl", "99315dd11227c2f5796c5608678fa5b9ff9c8119d54716b49c8a15977aa231d34fe9a28b28651285d782f5cde453bafae3b3b170ef4c0cb87ef7db3fb2d99887" },
                { "tr", "fb553b578e2bbb8deff2ed6becd3ecada2cbd43ddad22b66550f10da9c2d747c07ce7b4a9927aaab8f73de27123ecbb53fb8b8147d8338e53642ec86eda9025d" },
                { "trs", "51d2ea21b10516bcc1b1884d9065c08cbdb0fadaa02e99ba84e621754cfb87983b11a1d34596f2fce9ae508344218362c7bd78da507a6fe032e99fe47e221e01" },
                { "uk", "769de3b8575cca0e354ba8e8c71d1a275759710a41dab01d84ab62a2e6dc01924d2da064b98ed6699a10a53e6b6392776ea763904362841c6fb1bf1d7f7e673b" },
                { "ur", "8af4909b0a7ca2dce85d6a882a3c10cd8f5e83531683ac827639ec099e69d8351d1a01281209b00a434fc53c6a1c510bdf9e14139b0f6c4e3073632208c687a9" },
                { "uz", "8624378f42aa316fe77e87899f6d1e9c63cbb130aa4bf8d2fa64d2db51613f90b1da3ea7d115c1ca92ad70b991edf27dc6fbef00198436a472003f1f59acc82b" },
                { "vi", "15ae0efcaf1fa456042f02bea32551650e11b68580aa844a7e6a28718e3e90614f69bdba7779117df79c4a9f523b9321c63fe4a9a7008fc0b1461624acba3ec7" },
                { "xh", "265bc1c1a6d5a965bc3910c32dae1054876630c985dd06115683016cf8df241c41032d822d0e9b89b8350c121a2de735d85c023a978528fb553d66499ea806bd" },
                { "zh-CN", "7190a413575b37b11705de57051a209cc90129ed884e82f2de8589dcf576efdc3f1826a1eeab499d4bf2e8cff2d009ac282547eb7b0ed52e8f5da931bfe94158" },
                { "zh-TW", "c6af26f9a4a011ea6d131f0995497e6949d891d2a2304a28f50f26fd411435c1a462f35faa407dcba19ba18f1ef4c3ad6ac11b83b8afdf5ae0fcd7cc891482cf" }
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
