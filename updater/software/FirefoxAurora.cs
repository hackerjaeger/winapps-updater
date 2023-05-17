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
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "114.0b5";

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
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/114.0b5/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "5c87b520ccb188f640d14e4fa0eeeae9895dbb981ea7e42bf110359a4324a3e5b095ac3f35bbc34bc53091a16a74cf1a4267bddc0f07d7632ccecfd5e69d5631" },
                { "af", "222c4b8bed6b2635a52dfb1e62c0dac260eb26e58cd39485dcbbc1a6351bbd297c579c42a527bddd88a521b703be485f8527442f37b438d2202e6cf77d946392" },
                { "an", "5e6c1e85cd1e5247cc237c52a2553ca8263dfdb2536a392e6061146f133dcd320faf9d44be804c0bce8cb6a3a152a3e835d12ca2e2cad31f7a879a320efffdd4" },
                { "ar", "ae20ff208ca51a3cf70aa82566f86ef9341d816214522f857afc1030ec26ea391f4bafee0ad096d8d57787c600e843718ec604bc703a3b8ed93ae60e8d79fd5d" },
                { "ast", "20a7700ef510901b907bcb3ae2c27bde201221d657f303017396e346c88a2a06422976db4c14c33c0808b4098f230d74639a3de8b0e6800bd99d1624ed6df40e" },
                { "az", "83687d21278e68c872c04f319ca8da52e14c5f7cac050664d5df6ee4ef3da3c7bd46692324c761569d79bae4e9d8e9ac207d3c49638e60c7c109db63b98ab8b8" },
                { "be", "8c8bc16cec4667710e9450ec54b9aceef421d34ecbb12137e913cd3d42e1a9623d0fbdf093550e2f5e25d60adb12e00d6ce196a3529a3d2a2c7938e86636ef99" },
                { "bg", "a8ce61cf7978a9624bc2b0c9cbb2e2269ba0502ec1b1f8db7e071f3d04e19032083ec30743370c6fd57ca98bf420c497f6ac7174543b48d7d623c2cfb8097786" },
                { "bn", "e6f1f044fb34fb3dd14103a66bc55553b58e0715736e48029bf5924268b58f0a428ac181eb1784a5d87d661a5d42031d9a9b70a217cb811a73c25bbe41b1179d" },
                { "br", "42f01034844df70364ca9a5f84e6a06e0c8470049b0669c163b861418d99c8c7f42513a08f0951c922d2869fee6d58fa6959a2989f848eef3d63dbe084821ca4" },
                { "bs", "f23e1fa481a1fb36760a0213f519554cd5cd471604107ae4a24fdaea433522efd699d3e1184a1b9a502e3b2499146153748b17e388b682057c4a834e7fbe3ec4" },
                { "ca", "e636328dc860130ef3f717235952fed919dcba7c1e4eb3bd2f4964f01269aeb7311e4c34e1769147c887ed4585c8c994672ef21b082974cb3c899b7df278de14" },
                { "cak", "e36589f742346f2883071c3df5d870a87bbb89bbf0eabedb83bebec0050e3929130f35580958f2bb713aef7c5b374966b601ea8d32a5fad14894958a8f08d344" },
                { "cs", "dda9000c2e4842944e1df1d3c947c4fe822b4393dc9442b349064466e4a9d109216fc758751ef83739ce9b40bf6970a3d83688db3b9614be35c51ca308fcae02" },
                { "cy", "2ce550e27345558b743488f0799debeb0643f7df5ae71f29aaacbb86f04ad64845c4b344d7edd83e0c473c5f3b1973b9308f9b250143193321120c5f4fd0f23b" },
                { "da", "eeb1f6359b1e6d969e4c2f0e3efb3568d56ae67642ae75fbe0cf475c7c689d3bac2024303e60c797f98d78e83ae1f6aa64a0a68373210f36317ced0606264602" },
                { "de", "3ae1d5cbbfd2b9fc158e67b5d451dc86113d3d43489418c8c1d2a7b992d8276c72ecdaf1f48a6233fc649534a208f3f68bad46b46cb9b4652ffc92fdf77e1041" },
                { "dsb", "938ecfde992414f7751863d33ee57aadd33b335c37411660d21b57f4fdc6b2156cb3bd91ae66a35634472589e4267f3e386f56ca860a86f2784d1d1d98dcc1c5" },
                { "el", "df5fad1abec4252c09ca49cfa093caa9c7a707ad4608dd19d26f255e108ae44fa83643c18b87d0fa2ad28c25410a3a6a2e39f9059e115ef37b4aa5f1cfc50dee" },
                { "en-CA", "d9a03e1ea4a0c7cb9528152c9304b03b5d911951ce5bb4579dbed8ccd210ff1410e2553788f38619b7b9e073edab58c1382fb0af4565866c669507a9c862fbf4" },
                { "en-GB", "92628b8fe6f2fb4317019984a150615508d33d305806d45bfd3e9f0dc78feb38bf485f0be27255f0235f5eaf44f034d29ccde625dbb9fd6e03879aa1e22e1643" },
                { "en-US", "5f3e998ea9e7459ff62223b68e22f81ce84b1f4d9b86d772b66d3cc0f863418e4a4b378dfceea19904f49c9ef82b7f328c41a651168a30988605ef91c0a7c5d2" },
                { "eo", "47fdf2f31bfc9e2ff59ed0d71f5180f6ecb91b910916cfa5057df97155c501de4cf4696a956dd243a496f3057e8e1123c56f33cf198f25de95396e5e189dca08" },
                { "es-AR", "899208e843cd0fe80ceb3482a3c7e71820a17e84b3057b7a8b5361d6f7cd520bfe590d695e1305ff7da7264bed9a032b85ea2955236f0b31ad394c9d06ef1005" },
                { "es-CL", "2d922c8fccabc8aae019ddaa0fb594b6ddafd8502c6d8f4c542c732c16ed54c942a75b6edb609103881cb9ea0542c02ad09263999873f512d3df51d0818dc060" },
                { "es-ES", "192c646080dacc30b1c151ee3b7a1fc6fbf5711668e54ea21dce49dd84383b37f08c0aa34ef7087942b67f16d4b537be722345178e67ef4a086db0d14fe31676" },
                { "es-MX", "90a04d8d103a9d0e2e5e4db377cf94d29c131b8129040eaa2ba6983cd9b5a510c78a15fe823a75ecab3d7ccd070e0624fcf2102bb927e86c6017f0cc9095b8c6" },
                { "et", "e29fc216c93b693728a7e24d71ebc3ecead5433d997a829871ffe1bfa715cb83513f92ffa70803ffa12affe8e5797b76c273ca83adde727a170f274498aafb8c" },
                { "eu", "6697e3e0e67f26a48f9cbeef6903849df89a4af78062fe9f59fc204bc0dfb6e9beff07eff2f26d74c6489c277a3df866f7cfe500aa7990ed0756ebf73adf48a4" },
                { "fa", "e34576f8c01ec150acc9fe9df09cc90bca91af1ae51ea9616b739e95cc9a97b749e803aca9cb1802618d00f7e746ed68cc53c8f217eca3ae47eb07baab95ee04" },
                { "ff", "b780d7c18c9f6df2e57050564929fb0f2020f6339890a1b32d4902c050aa56f3fa050660347ca45043d7ed5917e70f16eae99e553c80d32698bbb124efe33390" },
                { "fi", "2a8f24a98bc8ffb2d4aafaab495bdb1c63a99618f3bc09253b0a87c5c75e3738d2ecefd718591fb4049589913cf79ae750a18d7056179babca301be131d216f7" },
                { "fr", "7898d4cf13814e449f0edadcf733a0d9b3d994d6052937f454935b3459ce0d81b3529bc0694649284d33efeeff96f436e659cb656cf9e8df3d3d37b40818200e" },
                { "fur", "e35f8c13a55fb5321dcf5f77b68df5f3c5e34d9adcc8d64a6d9f2cfbf9e0c1ac8166dba95257dfa04b33cdfdc36a7d217d95c82047bc18634b42a67898fc4815" },
                { "fy-NL", "2488c71b97bdc772545fe3338661bf46b95b6b75e66ed359620d2ce7729c46f25fad7499dbf261dbc068d26a7b77e010ea54c2e1c5d3953d8490695e64248ca2" },
                { "ga-IE", "0f27ee25ecaf2d0548ceed4fbfbc50fd1bad8b0838dbe3ba39293906ac74725ec8985bed726ded6c0652e1fe1980f550eae9ed9a78db6690522a58fab9a9d348" },
                { "gd", "4f8fda73b3f92279b7e2dc622edd063951f66886b7f8e982f5138945036e8e2b86cc82d4370020c370556ef36b8e347f95d1f16f174716eaa7f469aaa28ebfd0" },
                { "gl", "3f9f5b9860fdfb8d964df218db4749f3a1084e292f59de1b38554f1cba1c654a305502fb507a3a4c282c9d876faedebf7891e19c692a3ca9e552c1b75f73bc67" },
                { "gn", "2c79dadd97d481d11d8dca26bee74e6015cb14b856025e39ed370fe498ecba50d628dbf11be53941150178c5374c23bac015faa8f5ce338fd2479c6a52ed0fad" },
                { "gu-IN", "4d2b3fca70967077a031b1ed1a2c5b26af99b7dbf1cd6cff0971d682d34783e5a19ab9f7c6b731e99be1d808c47698697f0f1bddb7d921de03f038d5226b1bcf" },
                { "he", "732b1be61d99f3942cb6a317dfff8110dd0ac5a34e5a18277cf828c78809e5b5a740b341e58e9ec0d120f67a20b5e2cc6da6a350e4e60e22565d9ca09ce86b1a" },
                { "hi-IN", "59bd37cef6a9fc97bb6128d75105dedbb8affb233d43110f1ac23498e2aabae1a3cbca2b4e5cd16b45bbf1b824d25ac9ba2dbcbc0b0939395a193522f0e0ad0e" },
                { "hr", "3978ac1432d398eb4e8ba61f643fac9d6b8de3c3584037a64157f792b5ee430cc352786341bc7e38683dd0120f524bb57a468d4050881bc17714a799bbbd18e6" },
                { "hsb", "da2027a024f341f7919df4bc5c6948be7ffa89b97a213e5a74187f3a9e598b298fcc3ac300798303aaa61105e0f76970f0d23e43b208cb34a99242b7b6773b8c" },
                { "hu", "3ed84c220b2bc88820ba3bc355ce8f9a6dd97cc3058a164eceeed82465751772fe25b15ac001b79d59be2593252882d9ae61debdd32153e365364471c62111d3" },
                { "hy-AM", "36c23ef3dfe003fd040b41fef79ac90b5cd4ff415826897f3f58147f05a21ffca5addf6dddc3ae11b9e6c27c99721959f20cb3ebc9f43c50a33b43b8fe126453" },
                { "ia", "6ecdd4f7062188782381ea47ed788abe0225e8cac88747c20d166efdb0938f85ecbf6ffe368120a54426dfc10e5e555085b263ccb11a9b6ba4df760ada899b73" },
                { "id", "cd4f5d42324ec41e30215b8a5564bb67582615a73d14cd33a357af6beaaf66b2472a1ff7384934955f82f000dd45bc3d330ebf82158d0235ef228afd6c78f4e0" },
                { "is", "5f5c0392d37464d4865fbe43138e41dec74a8a1db226e1aa37db9d3fd08050f3eea4d96222e5bef4e50b21b3554e9fb1f75a85f76fdf1b8d7052f34582e41fa9" },
                { "it", "4f4da417661fbe72708c6ef28c258849123bda0e4102b2739c6439beb111f5570618e9aa3f515bcab82df89d2f5425f931c231a59aa6c0b54b2253c95c0b0367" },
                { "ja", "0a654a03e364b68956dedaee36b8a65911d484dc56eae503cd4ebe8bbdff87e4c7af03194afea2aca3391ac7c193a29a865cfa68835c23301b0da744a5538796" },
                { "ka", "7569b2b12d27220a13459529c1049b3925f219b2c39fade7a7e2d9f5326fac3e95ea3f51002621c2c83b2d83dc9ffa51f472e6a4e4d97f93ff91b971e9801461" },
                { "kab", "6c0b9823b302e6c0fba907df32b2518c55c2791a8ac29e654ee78a0fb3e948d169c77d3d7d39e54d39b82ec856e6f594b0c1500a053dd369c64677aa4c722afc" },
                { "kk", "676fbc5686624209d83312155421c69623b3db681bc32ad2a7f4790e86bbfe24376acfbbd9e8a102ac111b0f8b4949fb00ce03d153ece1d9d13eb51bc34ce7de" },
                { "km", "85bb4a9ffdfc80a5247c98f29e45e047a9ada34230572f66994603068e543cf10f648044003919865e7dc921d5b6c46c5bd759518781584e09c22ca9b8d14c7a" },
                { "kn", "f1c19416256d4b33aae2a224325a402b8f60c2c9b93e9581834cd7f3ee836c32db6a700569443c8778c005a393ff0e8f39faf42e2a11836766d65df82423fce4" },
                { "ko", "9dbe3d581cf78ffc5ade069649e36ae259331ca96ce7bf6c91fc0ecf4a20dbf49579f356300a507b0ab7125703d98f59b1fcc31a60a54f4dfdc02cb84319721c" },
                { "lij", "24fad74000133bfbf99376685bdbbd2d7aaff00dc243522bc1413dd73f9fb89dfba608bd39366eab40cc10777597ca88ef6bb58541dea91fec57d9ff23eddf2d" },
                { "lt", "1be4d4ae01460e2f023be218669c2fd07c9f8849de3cbeee289f9b155e6b737978476ad898dccbab49228f13d7b8809db85814e964defb7973c1cb9774c42fd1" },
                { "lv", "2a27b998b9544714a1be730c80490502c87ce0acb940439c9397e269395c56fb86e4c2866faf172946fb265d6f1e8792d6b8d7571c6d10c16457046384d3730b" },
                { "mk", "10373b03c3cd3be3645df922ef358ea2249b637c90c00c3f63794271d97b2f8996f2b281eef1f58d0f9a1e5ac30dd4ce7d6fd615ea96f3086d9fbb035e595d3e" },
                { "mr", "a53c12b9f768448e1dd4d1c33c7618d902576a03a038a09d79a32ae9b1cab40b8b7d24504b30a70c479459cead860d2ef15b95801947decc94a84903b946c5d7" },
                { "ms", "576fb497be2456cd556be424c4dac0ab906911d88c455d6482985272725f24f525b43b24b755eb0ff3805bf1d035ffc3f33ba84811fb42104fec7c90897d4c80" },
                { "my", "f6fb6dc9f1bedfd58fa83d7efa58a9e211b173eaa7ad3e35f13416037b6539a4d67252c4e1f4b8b5d438a5eb696251dcc0fcfcfe7f0415c0004488558633b732" },
                { "nb-NO", "98b2e240e280e51741f258724220c8f9127d721e0bb116f790ebf77955b65c941db22b882e3b9e5fb05fd008a62fbc7b421f87d27533bd304c7ed87ad0707eee" },
                { "ne-NP", "577652de0df2251705af93e9bc7bbb09572bb86b4f4a0b2467506f8cc8e9e9af2933ffbcd1cf409e9b9799a6c48c67f97f3099ca63677d2c8feb0d9c3d700d03" },
                { "nl", "dab75092db3bc2fcecd3772fc1e02f5581dfcc7e20055896f6ce98ec246976447c11177d793b8548b59f7012986aeca28e68407acaabbacdf9c8d81b9fc02021" },
                { "nn-NO", "1f42438a40d7cfffacd92d55f1be3c0570f0fd6e64893d0afa70fe0b34d1ac15bc0f36feb56b9ddfb4b3264d8606722ca5ce30f520a4e85e30a14a5fbefb4c59" },
                { "oc", "c5dfbb39caada6e9bae3c78b86142af13b423a9ebb5fbae415d74d8ff220a4f4be472807d1c9c6516decff4c23e3c4bd5cc26fed036d67fe52119e1eb55a60a5" },
                { "pa-IN", "713efdea94d03791cfbed8ee1c687d2d5ccf81c61db0bd1540df1d1afc90543edac69ae1e15b9bd1ac7d1626dd33e6941d1d8edc0216a089383839ece4e815c4" },
                { "pl", "020456e808aec9c21775b7fcfabe0772c94f850f08dfd011781f19dff50c7816826db47df169c82f76d6613246c01c50d82b8018c60d412283b2cc617da812fc" },
                { "pt-BR", "d57e65336545f0e1fbbe54f6488a4a646396a4a2da4cea107d4031a770fd3b593cc6c46cc4415984703ccfc344a9ffb83bd587b2be9254c31635ebe52d6ea7f4" },
                { "pt-PT", "11abd38a2d1e249576a551667415a464e03a280128100672eec2281ddd3d4b3157d24445859dfb694d4fd3a1dbed03013fd121cc1d067760aa463756fd9618c5" },
                { "rm", "c4213a919359177c4b03444340595c48f54eb3efe952dd56d442018a5ca96c467e8a66c5c1b1a8fd5f674e8bc9991b3139e950a00dbec3cad136a9f72e29f388" },
                { "ro", "8f43ec929332a0f9fb6022ba41c1c76f13dad2b690113039c330194ba3917ec5e5cc720f0c432257a728752d8641cfac1ca0e489eb3c7bf7b15f9df279789aa7" },
                { "ru", "b18ccd37f3b870bac928678dae12f5fb0190df6f2fa44a6e3bf4e3d2edc01abae30058431f379d56893535df8f4ad54e3ee3a44fcc02d5495faef1a05532ad66" },
                { "sc", "c0f49d3b6bc9dd907e5ba4ed6c66aa9f2376fa9b3f2ac8f258083224e83e8f576f3ec1b7b833e182463e0aed531b728a6195449cc5eeb23774cfb6b675eba2b8" },
                { "sco", "8418dd4476b12ea08189351c2bafff4cd7c970601f9bcac461c867891fb483f021b8acbc2f7dfb404b10612a99077eb33d2c3f524dcb1cc0e45809f75b21bc19" },
                { "si", "2c9d53de14f49ab41211bcfe8a84afa51deb1b406954c046a0f521b81dc9460785f05f33b25a319e703db00e2eb13439f4c48f4e900ceb407b02e7540da86a46" },
                { "sk", "5995d6711536fb5ceb83f5b6d0e86b94ec9d9bf84ef0b0fe47d5cf6664edf8c1e0678e7addd8996a9a8027d35a8a8aa6ed7d0928155e7b045fa99ddae130521d" },
                { "sl", "1b8d2025b510412f8272fd2875d52094070d7ff208a8615a72eab35cd1dc75c3cc024ec8ac54fac95766a009cabd3ddaf0f40c83a8884a76133b740e14b5daa3" },
                { "son", "f9e55f9244725bd0972d0aa3ebc3682cd6b1f4a9ce75dcac7f3cb3a4942ec5273e9c8beb972d5fcbb963192fbeb5906a873d55a6a33048a285c6951d5066d54a" },
                { "sq", "6fd812bde83aa2660442da07fb06ff36ba5fd1647a7debceeb131061e02b7348c6e1aa6fceff30f67add4b3d6c71101b1ba5d2e841b610a2b149a49166637c54" },
                { "sr", "ed645ccad972ac1e271ae30951681fcfa01733a638b32ae8588df0c44c432f83a7b145874429b5acbdef7c012f829b8b1c9583b3ef4c8cc950fd5e445fa27474" },
                { "sv-SE", "4a23a99f97cdaafbfbb06396b91728f49bb181a49e53f2c8e1eeecd1168bc12473ddecba8243f1dc77d937a73f9c02a280a63a4ddedb55606b78937712897105" },
                { "szl", "fbcaf6f17ce602518f0eee39a9f0c2a0482a60ac6bdf509eb439facd8a24af02767cd283113949f1cb677391660b616cea0cc4972888f6883f55f58b3bf41ab0" },
                { "ta", "13f50130b37684a4e72c82b0d8f06bfefbcc5624d11e4e321d35dcf6951bc5fe540a57eea52d92aaef894c2c4bfcd71b996e1e33afb77b73553fc17af840e820" },
                { "te", "58960600e75c077816dc5424e21a602f7b7747f399aeb88062880223bdfedc070926964a4eb8887199ade9d9b0128ea8cbbc29844d01f58c121b5f7153530a27" },
                { "tg", "0db9a413b2ab578b9959607cefe1be7664ea546942e3a58834baee0f2307e426f75bd174703bbe2cc7dd60120e77e0feb4ed370a18b0d8041873013e3858c3db" },
                { "th", "34e1b0093db0f44525c2e4c1d3676470082c57abbc5e3987ce1ad80f0b8be595e594829c268e32cb26157a1a58207d0a8ac16d4da0556e649f7bb065d8ac534d" },
                { "tl", "72320f1f8573c2f8b516bffdb184a7b09ef79ec365b698f972c217d55df8d91e0c0811c8a3fdf9b2846e932286643fda59df3553b30bdff9448f337fa153a140" },
                { "tr", "6f92d5027a9f97b44cd281320241cc8ceecc4a32196be2e11cc3ba207fa62b86307fc077669fd5e37ca7f20834b426a4c0c7db8852ff295ad28d15198c1545b3" },
                { "trs", "002e687d616ae4f7edd523b5199b6220c069188030749ac3ba3f1f68aa0e19ff7f8ea0e6788f1423cda4b2408067e0987b028914e794838d376b686de1475af8" },
                { "uk", "281f3b7a0742f45b62652ae124211fade57e9fee6e73e33d893ebb9dbf27a2ca3462bd9bc1e0d1ec05cadc860adbd0cf1c78136c532240b97597ae2eced13fb5" },
                { "ur", "cf39a3173ca8b98ebf75e30816ba72f722381f67a1635b6070400acdc3828c06a1a84668c72b4cbbc96af89b6742a1c488b362886249a56d9bf606b1b443b6c9" },
                { "uz", "fa98cbe434d5300745372cdc6c4a1defb3416a2a73ef50ed4ad83098affd3c40e431f448e9db5e9303e7bb51db8e873293013d733517ab38053fec9e51d8b788" },
                { "vi", "57157da966378cd59346b7d213feeb1bbdc13b6e7e75638983024dc38509de39256004978d3916b124cb08816a56f02450384e48d371fdbf4271deadb6338303" },
                { "xh", "9a5713cae030d302be881c305df974a4a76b2f8665dcb091db9bdd7ec7e75594544fe92e6f548f35aedfe9b1d7caa7285c31b0312f79d555e73be2d0de128aba" },
                { "zh-CN", "e949a497b57108b3dcf62810628272452944dd0e27c70f7f3146d72c832f29691d884ed1e072e4222f8f213482b408c9f8010c28de49534e694a0aa985f119a0" },
                { "zh-TW", "a6595c5409bdfc2c8b99ff82f7828bd60435c3ab0e7d4a626cb376063a4ca3f2937b03bdaf7c6bd13e2d45a26609a929e8124ddbabffbaf125a04675043c694a" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/114.0b5/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "9f974c53155398e30f07769b361a1c56c0ddd640fe123da0b4c90c67e02b191d1bfda3f5aff6369d65eb3a3a0fa79a5c8ccae1fc0c765eeda3751c2ad7f09ac4" },
                { "af", "074b6485542796c0fb17b908ca25aba03bd7e7b543b8ed56ea0ed546741b3e5526d7d372585c4365b689a54d29d1abae117b6647bca8b0e8b2d4cd7a5b961871" },
                { "an", "40006a59afdf63c13575a7cdd190d276116be0efa09a6d26ab5558c17e1a529691f9cd71adfa437e79cb351a0196b80679f4f908f680454f4c361748fd0b6e91" },
                { "ar", "eb85a70c4293edac73c65236dbfd7b9c2d5501a1da6c6dee2d47513a7990f8ed3e5943a4fb0294d936be67beae259f17d832ff47f940e63b2859a98db6119726" },
                { "ast", "c454d18f79fb3277b8a5619ee3955259d415c9fad75cc954d10713101221028b87bcd0112017f8d9b9c306ec7a28e6fd0bf45c712b35aba2b1db35db15db1fa5" },
                { "az", "faf1dd6c45e8722a9af3a4e43735677d7f438be9eeb505728f1d13f28b0cc0bb390f2ad874592a964a4bf932e4ff1c0eb6adfffbb351007aeaf644028e611eae" },
                { "be", "3a56e24eb04be6180710fcea7c4f8e776efce341c05b5d1dc38df9997c0bea168b7ce05e666fb64df89d8f2bf3676875a780ec2ec5ca66acabc9adcebcc5894f" },
                { "bg", "f27b6c88dd649e457ee57dccdec8206fd01eb95495a1b3aa171e6c38280f47b489e4f1d87e8c4cc87476779f56d070c1da5729c7e047b481a14a1488e5195ffd" },
                { "bn", "721375a9f991c187c72558bd05697792f24717b083056dcd5b7d155635d4a6df30f1fed49761279ca30063a390775a5b66981dc54ca47107d0dfc79cfe0675c2" },
                { "br", "c73c03ae63203c260a1eead3b4dc627e3fc1ea5ccebd1cc8c91e954f5bd5f47a3c5902f721068c13dc80cf2777a4100a12d178ee19e39cb9de262e53d822bca0" },
                { "bs", "a40127b01eff82a3d62f969eaf3d99bef2b2ae521b89a99c8d74f20268e337aea0900de440cd38d3f3deef26c18429721a87017d7e51e9a54666ddd9312eea6d" },
                { "ca", "6ffcc14f0e57e32cbdc921dcfed4f9bb8c609a7ce4f133a60e36e884d76f0d34a1e5eb87369731d98c50f9ca2cb32682ceb45dadb0fadfe3e14358a9d6ec9611" },
                { "cak", "69551d760c5593c1611d0ac90deeb5e6fb57e563c0117f1c804bf80ddf1d35f51f96cf2c2417a28bef23fa3c7e5c40b486865674c472ca9017ae535c15700c1f" },
                { "cs", "e5f42d9f37b8bc2e90aa6b7efa77eceda528281741bc5d3d741d1d848f0de67a11351ce4bcf1cb67344c92a06fb2a99556bcfd3c2f65fd9b24af5e3b4942be51" },
                { "cy", "9dfffd3b94156d8c505a403fb295a91689f74aea80b50cf35364498cf40cb986f13b69f4950ed26febe25566de10c4a1cd2c2d1481c614cca99ef448f1af50be" },
                { "da", "f54ffcc7b173230dcd48af79f819f5bbe857e696f6eab2e5f3a5f5747b848e10bc658f5b77508f096f4972d04996898b137fc9d8b65e4fe0463211d55daa15a0" },
                { "de", "9f9cc3ce3baac40c040e1a8089d269e2a4a653b0cf5d1b918de2c2502fbf631bcd99e552f8b644536135fb87c0d3fa53d27b56e06c9963d0e6b28b8deac0d044" },
                { "dsb", "f2e29499e1fe1a136f3513d370ae4062ab9fccbe22a155f82346b83a5124a3a9aebc452a503c4bc4b77bd99b5b3bb2a3555f1ba107821a0b575447a2e37debf2" },
                { "el", "0ae6fd355c830782cdf4c2a9f9a912391e7d3612e34ea44c10c110664ce21a04d686f6f7ed152806399d1ca8f347bd68273fcd219ce3c7e4e24f92a7bf813bff" },
                { "en-CA", "1c4c55c3cf708036173cf8b0003ab78aa0daf876a02035082b6b55813ca2389006019beb9bbf7d8bbb6604693a9794df5fbdb06e83b5c582a7494203641d7f1e" },
                { "en-GB", "cd7fc7bc61f9fc56128ccbe19f6f403e81a72a2904732db1aafb8884de377f05d1496641e198aeaad1fcfb46647d3d69ae2ddefd406353e0aa64f879cf18899f" },
                { "en-US", "69fdfca8b422d964fae5684b0f3783581d7c00bb48dc7df22b922996126655d903078cffadda3ebfccd9c957fccfc22474d5f81f23698177456ea36fae8c6244" },
                { "eo", "5e236b910160d7e2bfa1e07cf38e55183c37e4b4ad74753f3a36448e7bb20caf617dcfb03c51b5ad405c6359da79c1a0bfb97fa62fb2dd48581cf6ddb20568dc" },
                { "es-AR", "337c6f5fad95d47904775e9417109b72fd2a8d578bd2fd9b940c3c22b03587595879a1321537afd71ceeeb932b9eb9b52dd38d417fbe3c6cd28019e9be00024c" },
                { "es-CL", "cab8ff1d630f66f0cf3457ad9b91db889046d042b8545017872877c6df47475be328495bd3d918eb81f2aa5fda920409464ec92d7adceeebcc2eb8c5c6ab1baa" },
                { "es-ES", "e79c5ee35c1d97d7920a31f7b1550eed9f9d02eff9ab72b98122245e44f0a2b8e911ba4aa3ed370154642ba2aa128870712b16bae1365a4aa4edf2a3974b0cb1" },
                { "es-MX", "5ff8a252c1f5bc0a8aa2b605dab37a06e2c57f200c341721ccf675f5aeebe4bf76419e32ff0ce84cdeb5fba43eb19db2f77d1efba732c6ff5f59a5265ed69c10" },
                { "et", "1f1d013499bd57aef4218f6676ac18b079a9b5abdfd3152f5069737add998fceacb4f9d92f8287b63015abd0cb86e592590c82ca1d34944374c13c5c8dbbe4d6" },
                { "eu", "8ec2505d83279750099b624579e940180d44280603a936c8abbba3d1975918acf115301f483fd3b17735ea85a909b0d13582372af33a15505b59196684492441" },
                { "fa", "dab63a8d5aa069ac0ca9130a4d6d35b8c3420caa21fbd6153e364ef3f1d5de416ed51cb87b46a64374afbcf7bf78cc3592e3f88febc411b431b0b5aadf99df9f" },
                { "ff", "2404788a3496c581160b20b1a38cf16fc501a35bb44a97883ff0dae9ee04cc0b450219f92a36540175700da5e62f322a9bd708c909956caa7c0d87085cacb073" },
                { "fi", "c7775386550c622ba30abac5281accd89cdd71c7ed0effe5d1caea0d326c111894bdbd8681b39066f5464fb5550d65eaa2cb04a8bffa5565bb991c1f418e87a9" },
                { "fr", "130679103d1f5486446c24a123ec9beb405aae350f92b9c401b3d719a7d972ee3e1ecd2f2726391acc34fb3c39d8408bafd176dc4f37bd9b7492d888300c9203" },
                { "fur", "4fd1c2f0bd818e4dc9a1bef74d46cc5d1939a2020cde7fb08164c68da26465096e39b95a85ca77f244d17d762bcbfb662cf5ec0d266c27dec02e05bd219666d2" },
                { "fy-NL", "c54360a30bb3dae9d420eec59bbf29eaa223ec183ecad1f8d48e507e016f53fc4cd24f0c48a84eb72ef61ce6affb349b9c37ccb9306e429fd40ed050dda49f4b" },
                { "ga-IE", "82a9bcba5f631717d76273a7abff647b0314e563c7221665e3d358bc60cdcfdbfa0d0ac7eb2edaeae8662bc3ef4e700fe89c1c68eea2a90f8ff313b6057efc79" },
                { "gd", "83b121e80dd456d5458b141b4bdb5bf53c12113be115955f749bfe440a529dba07060602fe7274dcd84e51510b1d339158a60bb91eecac3b3a1cb91e0b44efd5" },
                { "gl", "0326be2daefa0941bb65074b48edd577392e5afe9f4046b24fd9fea9f256f34f6f3254a0cbe51828e15c98a31477c7931e4781e409a9a88aaff0d529ccfa1d19" },
                { "gn", "3e6ad07fb7645ca368ad06940afd6bb43722f0b6e7c14277410a65ced2e200e81a1c49c9a5f6d38f79c18d83f31adba8c79aa64b74214445d6cd89b94b52c080" },
                { "gu-IN", "ee42c719e21569772e4915f145e26a9fa24592f1349b7b7c0406b45235dd155ddf4a11423110ca0d808be5fc2fed718cefd52afae5500d7ad2ba29c3efc58345" },
                { "he", "2d78217949aab1e3f4fedc990e8101341fc09eae766a59a0d81949f768be2056dd721bd8d00978e48f58fea3373e77a04ad75c5a480cca36d30db1e1a1c5e49f" },
                { "hi-IN", "ca0942a25e5e2eb1aa5ca244755823e40c6dfa68e6d7488e1b83ebfd278108f1d057ce2c06874d4417450180ab25dd3c50dbfed5926c11e4f70cda5e4cfb2d2a" },
                { "hr", "dab55db4a2a11f4d5f118dda4a5c441d9bd1e59df9c514ad4175bf3d326778ead4104d1de9faff86fbda6d397eaa61683c6f14afce9a83bf79a7ecbb47763e8b" },
                { "hsb", "9c02a8701a2802426f6d21e0a4b8c1ccd1c72af12861c1a0e623244d61741a8140a6b8a9657a4b736a58f0272d0572d37cf5c44634716370c85ee67e2d8e8c2c" },
                { "hu", "000ade33275daf7895052dbf11068ee3b147417f1c9ddad63aba0dafb3c60e761f4a61b4a5eaa59c859316d7c124328263f473f5c53d4ab8482ee63f2a4f336f" },
                { "hy-AM", "388a8f3e10c77b7aa2344e8586239b8cb2dedcc7eca76b1e9ab70db26248569acb17caa078c68ae24b5813c12d9f73762202737cb1e807c252ce9ab9fbb82f6d" },
                { "ia", "8c8592162c849c83ddc037f60d85a3a91d702d1d1b33c297f5f0921a499cc602302c11f953357cde5d9f7d7926226333c086382e874fc9c1edf9d5f9c629642b" },
                { "id", "1c27d3270de0ce204efdf338b24304e684b3bfe69bc998d8761eedd4c45d2d448b5bdedcb6eb00e865c8d444b26a665fdd7440271264385c3349d158ede906b8" },
                { "is", "8bdd95f79cfd42b8768d5de0f5d02e0d2cc4788900a914e55b11a2f95ea523ae72673342d84c76f7361c5407d8ce870f9de71f51c2b260d4f1d80987ae733bab" },
                { "it", "a6f73b40f828f1038de4742b2adc5c57d843eb940ed46f633e6ac79125b7b6462790ee6bd1e0a0ade8becdbf918c91fd8068e69dd0d1a53c4bfa43d285b775a4" },
                { "ja", "5f5ee18848fbd7a5e3265732163e70036bd2adf7260be47f6c0a939e1d525f6bfe96ecdc9e2d4cb1f6e0908efc8115e24c3a166fd5b632a9185c81aa8fd71e13" },
                { "ka", "654342d389253077246e2def831100c5900b7bb283dbb3ff902d069d295b35ae4c046541e080ca507295dc700a8998c7024615334c48740cc3f7f5ca26feb1a0" },
                { "kab", "7598a76748a4743c494a3e5c17d76199a7efccef04fa6a4338af487482d7e2512f647dc628a9e3d7148cca2fec616f2bbc10d70111063fdddc18cd4d2347e9b6" },
                { "kk", "3fd280243b2a1b85cf29e225ca14a3f81e0c8b88f700285dc2e9310bcef60479bc33e479617869b3d22947582efc5806eb27e33467a26406eba21473129c8ef7" },
                { "km", "4e5e999c39d22be767d1359691a97e4c991218eceec8ff4c05e87bbe4d3427818d35dab41ae5011c05b4df055f2a1534bd12671b252b8df9c2a047604746998d" },
                { "kn", "5782efc75412962c5bc059f5e6876afb9036c6f6fce5611655ac98e8db3648fb86b2897f77f2ae29d823da4b34c4c87e257265c205e76262ad27a4b7fe6a6310" },
                { "ko", "9c6faa38fe2ebb1318c6d0d948cef311b6b350e2552a1160b360d6961af6d99dbf5e25450c9d59e71a4cf750ab784f459adf4d53774d1c4e3f1a29b6d2661251" },
                { "lij", "0211fa12010151c05416a1a0ae46dc9441f7cc182a9b9bc6830a305badb1019905e05496e4ac801547a9d88423926fd50efc579e6d46f1b50ebbd91d6e22fa29" },
                { "lt", "f72ca806d33c82b2e0b9e41de495cb4acad0c167f67f923cd0398d9884eec5b97499a495af1e97aaa046247ecb7ee0a39dbfb19a037c1dddb0b6e54e790c3bef" },
                { "lv", "d96e176a3f752e3396b21d9eec1d76bbc5a0a87ef2e36aac831a154f21351204557033f3b0ead634c409129c29375368a3ae0f612292f1b4dddc1b6cdef243ff" },
                { "mk", "30d1d556bb0db87d3342e3f6d9e75981c3ee096a4df402cd87fe0800e75a1e26e84675e64336f69729af1c726fc96dbdd00958619ce1231bc7f48461569feaff" },
                { "mr", "3f583ab9716e66df6cef386eb118f75c9df8d66722d462c085c3a03437e2474643c6afc6743a1bb44696b0d037ff7841f7857e3024c0f0f7fdf3b9ece8949e53" },
                { "ms", "b2f3c03a137ccd3969f3d7a3731be8454c401ea5a79cd41be3d28129a66dbce64dc63f9527ad9d69ac1975325012d54c4cdc506244a53bb1ea7f00b86dedd5b0" },
                { "my", "e2e4bb1b5b0d11bee6674bf2ff20a699dc9e51357b133ac7d1b249f90f97bf6ab3e1b42142e6aa24f4d24199d54948bbcbce00a0a732fafa1115249833f553ba" },
                { "nb-NO", "dee5db15637c241ab81bd770e56e1db0cef6692ca0251d0a735215caab0cc80dfb605cd7ccc3ad167a8a9561ea81f50bded9817e572b6997def52e0ba0f13cd7" },
                { "ne-NP", "189a96524e61d4dd36028311cde30c0b9d733fda37b0b5db67dff0598aa02f922bc17390e72870e6636881a1dc3541df10018454d859c861ade5a88affc77159" },
                { "nl", "bc93ca45283b9b6e70e9d2da1fcc1b36d272ee89f817e397b1e947cc5990d67baa46243d49a4a2df589669c104831b206bb28b8b6a2f0ac91de81ff072ca6c4b" },
                { "nn-NO", "3d3bbdece689efea6cd752cb8f5cac0c8284cfa674aacb79a9b5b14c771d22e97ce49eb63fb2d64c9ca77e3898c52458d600d272f49eecb460360ba327f194d7" },
                { "oc", "7464dc8bae6b0f22298e6fe899745893acd54ed9cfd1657ff88c4cac7c95294d6a1c33f2a222354333358e4d00fbaeb3d250422130b7122bc60e5625e8db2749" },
                { "pa-IN", "7cb3824667b271181956fac01895b369d2be53a92b0c413bfc99ea008ffee93b1b452f0f2e0cf12be9ab6445c97febd964b4d0a6904f5f0fbbbd669036952251" },
                { "pl", "002929a27e10f9afd6bafe98bdba0d9074171107f0503fe3d8570ccad581493ef41f8e732a85e3099aeddfbcd949a4102578826fc967880e1ccf9dbb3c60f603" },
                { "pt-BR", "22f9b1cb29d5870c22b4fdb2a46cf316e78addb66c02003c50fa070a470d69a4536806364a685ce4296b1d94d564e6f1b477237e5088050bb51464ff97f3e644" },
                { "pt-PT", "b689eb14ea14bd6ca3b89561bfd8761b83dfc21500eeb4e1cc766e5bab590d8c9b0832b1f6062053bcbcca57d4c6d1f4edb4be48acd6c62e0570f9a8e6eccff1" },
                { "rm", "267dccb768c2016081c321f69665a0a36ade52b967e12e12a52c7659c0c88016668706abe814a299a46e035da73a320fc1e4774c6bfe32849447c99314988748" },
                { "ro", "e9d11ee9c110bda4f201b15648c6910afb11696cbf0388559f4d09c0a86253cc58551c27a6c936578888c6c9242c4e28240393502409163ae3ec62729a01eea8" },
                { "ru", "66929b6d75e050bd298d09fa310b6d27400689861bb00756bcd094b539bdc4ffa28d2cb87a8723c2da5ecba22cc035121b06e81ed1b13ea37348b68b78561446" },
                { "sc", "6a34a1bb2bb84e3e3158ef6364e6311d911217ada7a45b0b51b1cc87a008903e3b5eca417d5444ba38cf38f1b58ebe4ebe6f226dc4f3691a19b6539acb4790d1" },
                { "sco", "e96073a603c113ae0515b53033eca5371280fa8f4cf2f8bae85570a183b10fae4e9ea8506c3af3aaa74ca42d0863f224338fa7c2d92a369cae529f42bba3e8a2" },
                { "si", "2ac0c861ef2c84ccf51fc1a0428d7eebe6e8506bbf2c36fdd6e546ae8c5d39931f5f66c14eef3682cde2acae87e38e45730aa14992a9834c953651d44eaf25bc" },
                { "sk", "0995786260ab87a029bdf5e638233a55304f6f03a1d108e96b2a3bb631bbe5ddb46ddb8e438342ffe6852a9231a73866606400593f2315857a7d05d3146503e0" },
                { "sl", "f7c239fda35fa637560f469ae95f3753419fc41978057e925e9859845864145421cbc61613de20234d0f77d5f5ede956e6146336e4d1313e9ce134a28f2a040b" },
                { "son", "6f654c316819166e560a7f5269ebb3367123ec2de1f703c701cbdb1476cef57afde912004d7ec933eba717a41c3b0b78d841442e8f40f9d27390a33f3c16310f" },
                { "sq", "d99369be8bc4649ffa4f6b2e8f888aedf8dc529903698c8e7ef9dfdbc74023d2f640e832d99f3894973e0cd990880c650e2d90c1707b83502267316b1e69a647" },
                { "sr", "4a135f72392f5570d83168887f30d9fd7ede6773006f4dad7a9468973b98cd487990262c03fe429edcdf25f4af139640f994a743d555f9386647369dd3dbc25a" },
                { "sv-SE", "d6648de516a8f002564f4d1d9f50f69cd6e5fbc40f5a060a43289700f3978e6a86280d2d31cabf6bd44e73f0479e28139b189ba66558fef6a2a9b5680b8baaa5" },
                { "szl", "6eb29681af28fdd71f963dff7f56e10243aea501a2e81262b1f6124e2605b8f3bcdcdd3bb1f352fdfff159f458f76e7bbaaa51dd3093f5d1e491ede44a1c6ce4" },
                { "ta", "ede6632ac7f9ac6bfdde7d59f554bd55a50459be9b4a06f8083b7ddc8736b5164ad4c6cbeddac3eee0671141f560888a67f54593b4f53669effb763f21848cf0" },
                { "te", "8ed691f643295879413ffcc8618dd47f45d673e7c42b1a50dd3e2e2c5dd0986cfaf6eaf6cd7ceeb5f8157cba12d572be47842cc9ad8d8e07e4a1cc4e0efa3295" },
                { "tg", "b84ac71b4e6ebc1753960047fd97cee0feb33d492c12489c8c3772d81aac7bf755580ddf5a937703b4c3d0c7ce86cd8cabff6214607f6975fbb748745b9a5c37" },
                { "th", "ee2b1f42555a61a995f477c9c1859d9d342ea75c8ae6a68d88cd8547ab8044bd8531a9f9fc281d2215a4b4ef10598fd8a37695c4fc01ab7a107be6346d88241c" },
                { "tl", "392065f65121e54197cb88c3b04dae48410df4e3f3832ecf87543bae68b0f9b9318d19d54c4d351123c1981ddaf10ebd50124ef2084183307365d5c649707833" },
                { "tr", "da8c27ca927bee1cee9d15c61e840eef7033aa5d8fbef561a09b920fc07c1ec55662bae1bc88b2fbe2a06cdb706df97dec5b5f3aee4b3bb42a5f60713ad6f302" },
                { "trs", "a84636cc26778184e379a8b24d8d2808c1ff2c6a36042a87d7a2a6009d59e5c770bf79624ddf2bebbf1e80c4610223d5cec95034780ab8b0515dbf0933244a2b" },
                { "uk", "f3f5e67fb3f7d78f95834b6c4b8314f3c24697bd53c2fa2a8648f3201c31502afe331c43668cf484f873c31902ce94dec1c6b3027de5d752556d2a4304c47040" },
                { "ur", "bee801652f97f543048da261eeafc817bbe56f2d80f40e7f19c111c92cb8a5535b6c4de98848294f1efbf1e4ccc16794d450ec9e576b9784b18f59990cbb4917" },
                { "uz", "c832fe72ccac983dafc01c45ef1de514328866ab3b0f62028e421a7d6321bf7547017948c81ab3e89f010a46f857a780fc6836f56aef9f832c1439e09a2fa4cd" },
                { "vi", "50f4228067a5d1020aaa18650dd6947c60ce2187d054b600121f0d399d5a6c3db4c812db06ed9bb2b15abdde77e7fda66aac7b368bef320cb14b26225e2bd013" },
                { "xh", "25a7afd58edfed4faee5fc8b8abe5452949d4766bfd6eb569d1a5226cd25c0a11dc61244abc01300c261899a1e04f970091c0df82dece061c1e07337a3be57f9" },
                { "zh-CN", "7f4e391131db3b7234c126415146868f728b27221cdf2c3ec208a555b426d8d870cf8b7ea87b3fd5a411b8c72f43696de8e98874c0ab936679040fc1f3d4f182" },
                { "zh-TW", "dee3af24b85f3b9cd5b558df9d6ff1527cc404d47fa7d0f89b5484fcbf1092d37125f136bfd8973c8955e4c1bbebeacdd59803a9f852f6ca65278d6bf2a612bc" }
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
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
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
        public string determineNewestVersion()
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
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
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
                    // look for lines with language code and version for 32 bit
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
                    // look for line with the correct language code and version for 64 bit
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
        /// Determines whether or not the method searchForNewer() is implemented.
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
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
