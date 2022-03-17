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
using System.Linq;
using System.Net;
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
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "99.0b4";

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
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/devedition/releases/99.0b4/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "4f908e367ba37dc95dd9ef429e049d386c11e0f6f96f928598eb093fab86b4f72ba9b7e60cf1f0e432bf797cde21b3691e916ef40417a3595421d183d4b543e7" },
                { "af", "ec8238513c1db21862c1845427682b7ba2867716c597f813ad9b8e0fdf1e98dcb94d43838ca3e41766adf5d8f000c3e65f47139a97e3f5178c42886b97c165d4" },
                { "an", "eef7774e7af3609a4e3f4f7cab2a2bcc8094a907890d7db3737c81c45750d2d0dc75666348ef8ad9780ac81aea69ce95e7951b232b11d576d969904291f78511" },
                { "ar", "c8adcd816c9adc6ca68dbf05e1967a073e65f87308cabb9dc8605e46f5d6bed12f5cc256ee57c71a1d7f071ccfbfde8359f5cba9e98ab0e1ff2855088bee82d1" },
                { "ast", "6caa2cbf2550a8402080a9a9b91b50dff6f6e9ee3eb5b0e43430bb5f79aac7bd26267f0b93fb4d2e0d594ae74372fb0e9c9984ad374dacfdfa14ad3f672d0398" },
                { "az", "9e44ea5c9b7e03ca174b54a9d4304c5d94ce13f66bb4a6ad4ab65a0f3d94b67c7ee96c0791bdb522effd280b4c3f4543461259a249d649052f34e484ce344137" },
                { "be", "e1028e6f2376400ee1c3117332cee30f56544a3516cdbc601d0d3d2d030958a1dc15846c724540e3bc559b2e0468c35e80a16a8077605c5d268d262092bd365d" },
                { "bg", "bca7f30a4f4556007b0675427ee594ccfb2d486b530beacc71445825514594e5993145799309b610e1e4f13e2d931c35001dcbdf08693e3ce489433fbf0f6a78" },
                { "bn", "eae6a7f96898c394a518594242c0d9e18a31c48ce37d00a624a489f044922ed581b81d99f2db143544461fe2d81325e06cc04011a436f138c9c5ffdbcbe352fa" },
                { "br", "b31f3b5085f43253431619cc6d6ddf91fd522993c57247df22db264cb4166a946dfec1d9069e6790e1d8f1e92ebd3727950ea3767f7832b05ae7d95a7489d20d" },
                { "bs", "1eed672fb08d1eedbc84879d122f0d5b0e6d61432b8811bff2d8662605c2e533864661e1571eedfd4510c6c3d746254bb88ad187502061b46fa4cdaaf3ea2e4c" },
                { "ca", "0eacad12fe466a5c7ad1a40a5158b52847ecc76fb64172f9a7d1b0a30350a763aa4c60fff0f9594e797fef4cad31cb4f3c6ff0ffe92eda52ac61442e5abe8cf4" },
                { "cak", "03d46cdfc7b9b492853cd2d587bdfd1507308e418bbb05a58344a53880ece8cc394eec42ad9fd89dfe94423056e08140f67c3d8b66cace4343ea51561b66a86c" },
                { "cs", "28da4d43c56352178e2945d14bee79eb9a41402830732b2109e83135dd5c2c29d3d9d6abf54ffe99820392e1074a5b0960c7dc1441842b957edc5c21cd567bf7" },
                { "cy", "980619ab0f34ac0803d5b2d6f111fa447d0331b913ee4f8b3ad78ca705c87ea5cdd8f784ee6b5fe20350525aab9c740bc6e1f24e89962763027e91a5a5d5bd56" },
                { "da", "8c28f10db31b75df946fda36c25ba7b1a99ee774c228adc19ec9499cbf464bc3486043ed7775e03b4076d276455b4c32bbedf5c0988b0fb3d9ac6fcecbf32d07" },
                { "de", "88af791b87656b64ff6b306f9c8157edf3712b4de6f619a5fbeb95749604f154e1815df4fdffeba4159b1cf3709b5771c758c00c3db05b37dfa5c6ec758b34c1" },
                { "dsb", "2e89227827a412e9c6c2990531c078ab5c2b7bbea2b7793ceeecb35d65277c540f0d3938a4c4d71db26ce1ec62abe4b3b470b19d483f3d1e70193296489e36ca" },
                { "el", "a621c63afcffed42d74a94085cbb9270b19ebbad9596a0c7327ad8c4c69bd59ed283c78cec2fea7e4041d18e5dca285a0b42e3d897a7508ddbb980d62da050f4" },
                { "en-CA", "3692821405e5e819ef66c6d01cf58e4b31e1e058ecae07935f8aac0c3fc3bdf6a1db023b794c90494ddb44f39a3a98bafed4763785401540fbff1323863019e8" },
                { "en-GB", "01e52d98e2194752d0a5bfe15bd46236e67b1db4513fa794ecf46c5626f181f41885d722eb0af6aabf2c874b5bc39f500e201c925e93fe10d5e9206a20a1ba29" },
                { "en-US", "8108d393feb94dacafebbe97cd914c643060bb0596e49fcf0f7bdb821cfe23d4aa43ca100251e548f5e81a2943df7ba378edc6eb172c3da1ce3d39bf606678a7" },
                { "eo", "546b0d71906200b74318b3656e76d07b082ce43681badf2705556bddb4d0a93ab2c2ec15c2bcc5335514748a5fd5751f7117dbe816d4ca903a3c39191bbb2202" },
                { "es-AR", "47677ce62dd305a260f0843868d9ffc39efd2da5669196966f3997eea6635ce3027bd178302d2826161173bcbd398959a20f37cd67c22f87ff97e4538860a3fc" },
                { "es-CL", "7392eda5a44714b63c787c9963d40d01fa22401382e36350f897d61668103a65a2106dff43d6ed459a382d9d4771786e7feb17e2e70b01945ea1bc9c85d20b3f" },
                { "es-ES", "6cb90e3a178a19ed2434111c5b3d6dd72a59175539fafd2a3cd2e539dec57ec638200d69fd52668bb60dee3d6ef52c94a534ba7fd8f7b283d15ee547233651a9" },
                { "es-MX", "cfcbfb4b8f787aac47f4f5ac6c28ea6461699bad86f3d3b86fde53c777c50d873af47d6a06766b9bf676456e2941a53550fb75be2dec50f654f1f86d94041d33" },
                { "et", "fc6379948ba5f5bcad4ead006f44bd192bcde6664eef2cb457894dc91aba18aaeabbe806762148de918af9c60652c7cfc7a7419c2226f440b1569f81b8bdca0d" },
                { "eu", "90221db0eb269c420690f201b93d44f84ff8d92e2c959aeddba66a95dbbdf668d4ea131483ede052921c6a4d3b0d1ba782325996a2324d7c8912304faa78fbb4" },
                { "fa", "2cbbacb6e7cb69380a46af51af1c38f32fb06295bca97eb7df15ed05cee3ad2af7bb68ff966b2e67fa34320f02036788abbb3867c21e1839387cf03f30e6923b" },
                { "ff", "1dc75a08e052b8a1ee013a01312b0dd3c5236b74a627bc06e0583a248d5751af1613aab6fe69d7ba707c08698e06f62290f48d73c74448be94cb318f9bf7798d" },
                { "fi", "aec8ee8c7c407dc1ad4fb4ab247136a8cdd94dddd18ad03f7348767fbae8b4fd23c820d4b520ebb221e6fa6aa2d849e3b70a8432d21da82dc615f9e234792c4f" },
                { "fr", "26208567616714ad3516279ff043ac821f588837415fa4132b99783d1d2090af372f9913183b44733c8f94646950a87418bb4f1200e000f746509443618b129e" },
                { "fy-NL", "3476747009ffb99186bd0442edad80e075eea2b33fe528ca859cf9a6b4f94c00c1e36ca910f3b2bc9e8a6cb4275c3c5d83a2be8265c99e9a8c93f698c92a7891" },
                { "ga-IE", "e489abe37fe7811bfc807e90bdc20eb0e991e44de4e2309ae89fd504ebf3f70d9c5401e49ecf34498d5c30b0ea4daed23f09b01d32e7e31c1ba6df81d9025f3b" },
                { "gd", "64cbb88302af32598880bdd6ce0eeec0fddf0f6e9297d52fe38f936a0d190299c66968a81df89648adc5501c131749e8a9ae8be0a51ddd3161753a6c54e3e172" },
                { "gl", "8d51ce7385476a40d6021d43547bbbd1443dd0083664ab99faf1e9c36cf7ab0f13271648d754f851e19b72a670997429e89c2f82bc6b88012a5b85b8118dfb40" },
                { "gn", "975f4f2405f58ca3af78de611ce8dae09ff357d404d21fe23db14379bf373cb20bb8f81570df4ebdfef6f854ec6587336ea3fd214e61bc0e77f8b77083eeb7a7" },
                { "gu-IN", "e13deac79c68037a24172de2ce5558fd693057f8d9f5b8a8e88fa7dfb8714975b498f88e1f2e977e280395efd721ffdf483304882eb81a0fef0c4d32ea5407d4" },
                { "he", "729a02422846a6f124c76949beaf080807a248877f5a346cb9375b8b46db1252cc44d331b4c079dfe8a5496d5031769bf680d7f9c400c1750a3bbccf0285386c" },
                { "hi-IN", "708f3787765dab64a3d17b8d39bd1c7fab9d0a70c72d6cddd5bf88d49f154a067579d82083a010672a3aac600dd6268494b546e072fab24cbbd9ee6b9065c126" },
                { "hr", "04782a240340af6d947b0fcaeb67a61848b64cc7a5901cabd663b35da1cf5a267eb825076de44511c46164972a22afd0bc4c019a3d3d7a8527fb2b43c313661f" },
                { "hsb", "8ec323c395711c80deeb65c53230212620efe9b38e9707a86dd4082ae3f91682ce22b879276ec1308d7dc84de076d3cffce261235744bcfe5ea8627940964c9c" },
                { "hu", "72b09614749c2501358d18b06e1d7db9f560e1e6c6f170554f9fc4f28554cd058c56005964abd7ddec5c7a584a38ddc1dc53e35688435b85e8c26d03b5305e18" },
                { "hy-AM", "19e1f7eb5a50cefacd5cf1a0723218facb63d12cf85108a411a495b59cf82ec59d4f8aec805c7e228edbfc4c8c6a56f9e51b28172f1142c601dfb4db6d7c384d" },
                { "ia", "3aa7eaf340bd0def2f3a1715da8a4bc1d8b1d30681ee39624d105f09f37da6def75ff92422df3ea71d8274ed667e904fc6fdeada9225160007b18a61afdcabc5" },
                { "id", "b570bacbb753a4f8c1a1c9a3c1551274cbb99783d4809baafb96462a5032df9c1f8c16f952b659eca8880388161d03ef56d3c37fa5ad8febf455f499bdc8070b" },
                { "is", "277f7ba16c82a1898a3042c30b726dccb48a1d9100c9621e59e868e357c7db37d44e94347fac420b410e44125d5e609d57987de527844c19b0c99ca8af5f97c7" },
                { "it", "49d33458fa908d9b457b97ad3ccca767e30dc98443240faeec38cbfbcfb3a9e26750f95af9501e0534e330a54a0112124e52579a3fa1b8f332d3cf8295967c8b" },
                { "ja", "94321f5a051b55eb87deaff263adf1fd9723bd2dee181c39658a931b6c87bc8b52a2cfb80c80d6255b837eaf22df08e05bc09025338f64243de60be24e9f2b0e" },
                { "ka", "5380182bed4ad0c02c9ae9abe8afeb113d620ac4cc1a79ede6d4ac40c5f3f39d0a1b332359e5edb2e9975374c9c8b0ee9d6c1ed7d50d58385e8c8f831606b6ea" },
                { "kab", "7895e0ed25579df0c796922a60b32fb68324d2fda0ef1c4158cd7456ec609658e431fa66e08a8a521889b6503fd10025053a972658457751094c33c4bd9540ce" },
                { "kk", "347d1d52ee225c9193608c1faac652beb8c43b3b651c70e04bd40dc1bbaab3d95d7cb898b2c92dc4f839303064347823768749928549270a5bdcc517c03ef6d9" },
                { "km", "fd11d01444b370a497d16e20d98f49083bbbbd4242fd58a781939633a2179c382fc61a169ddad2472b3d76640e88e42f478c27f9fca76a4260cf32cf059209a2" },
                { "kn", "122fdd90c4f646e50cb9ad73f00cc7bb2e12a7712c07c76cddb9eb5d11f76c5d623a0495dbcd3e547aa3f2d61aa8c2e93dd4780058f58aa4d80047d4de4679c3" },
                { "ko", "94f58f1bc1e0a19f4599dc47291e3ca730648a8ad10fc5d41c7d9f35bf81b7a2dcf8a6e982cd9cfc0bfbc34d19a1de55cd1054f31a69a364e89e18d134b7094b" },
                { "lij", "26b614e66a7ed30be06b62d6457fe0437249860ec11c574071dbe4bc2dc5042563cd3ac11f4d4357a0f3b344417cb939786ac65dfa5a48a85dcf92924dd188ee" },
                { "lt", "3757ab74f203d071b5c9953d8b2d54159982242d7ca3e4a5dd1c4c9bedd987c42d0f31affab75c3351197591ee60a4cacda27e15fc0003d0f9e90baa4fa8f165" },
                { "lv", "911bde4a36a843ad714cb4293a9422cadb5933dff6eebe4fbb5add0a6ff4a49b9e3fc2d22a1ef65019206540f5eb9cfe934d5bc762f047794519a5d09e9f9cc4" },
                { "mk", "b00935e6d07002fb7be5629500a94fc0840f04124b84e3c3e239e724df7435e49f39c021ae61262690a45f568a347403abce3cea9300608eced60796a5b3f775" },
                { "mr", "30e69e2b7c36dd0596527aa1afbd6508db604c59269e6b743a58c4154b316ff9bf7a3bedf81c394236eed42333482ef85e78c420b0063a1615af442ef88a5db0" },
                { "ms", "e0641fb5a91afa5df423a44a0c1638ad1d44276c9f6703ca8aa5d333293524ffc3eceaf26f89114dae81e75c5d8a18a7fdfc46bf279c1b4986d6a0d2d358e1d0" },
                { "my", "89e9a8b39cedc8513eb78b4b806cbb1633282ef08f0e1f9ab01cf21d8c72e0ccb090610a1fc7b99e117eecaa031d6918a0c4e6b617901dfbe8f65044f2d8adee" },
                { "nb-NO", "68ed9d77a32d7a36e579ebce4c8e6a4e7524b0ee531255a75785d626fe0debca9f9a5366c9d3db2a45ce9271bb37a48dd279c4c02a6c362a935ab35918e705f3" },
                { "ne-NP", "fa306a65069b7aa16a246a79cfb3458db5e39464cb9ca570d6f504232ed2741b26729aebb16d163b184708a7b673feeb3dddd6662609ec92d21646279f23d695" },
                { "nl", "dc939fa75d504cff02eec015b4085b14b241d1c7edf0e9d4e5a4b231dcdd4f592261bd541ddfcf24014127da1bedd37381af3c77adb84a932e5a5a54f17a01a8" },
                { "nn-NO", "88d6f11d5f8c5e9ef3ee2f13d97688c96fdf1e44245ada4ea22bd2c7997ff7c6786d5f4cd30dae529a2c465de63aa8a97ffd0e09fd6ece9531eef6214880e944" },
                { "oc", "377d9762614ccacfeca9ccc746b29bc81eacd8600f029ab9c0e26ed41ac61fd42b88ad858dba6ddc6ab42da80419867d3ebadf1cd59575d5aebaabf94e51efb4" },
                { "pa-IN", "45ab9b266131c3248621e7acb236fd539204eefeb21fcd7bf9c3ac9b75fdbabf8a91d8454de1f19115de0da728d4cbaa15263a1ddf413e52e76e952e10bff85e" },
                { "pl", "f0364d96e24358e3ac078b91d79a034f38cc249cdfbeb37c4e19d94d2411f17eec5e1136e55501048f066ccf233af2aea9f588001bb3eb5e96f2dde9b6e0ec7a" },
                { "pt-BR", "3125a78667cff5d81dbf0c540fead2e1d2307dc627b924c09f5a735599825117a637b4b9ae6ea11fcfe033e3bc71d3fb47d1fed1ecf19443d7be60acd8fe4c94" },
                { "pt-PT", "e78b32f8f79139874b3f5d9fafc551f70d5641d7fe4d8dabb5fb80bcf3003c14ef300c2a5af88511819e821b785c8c1fd717f0637915e5f77906a13dc417e617" },
                { "rm", "75f17f727f80f4c74f286ea659bae08fdb7ebcdff90e1443793bd18e88c32ad205ad847314aaff75bf5767462791cf311cbc59865000c69c2a88e79179c50ad9" },
                { "ro", "519b814584e21799d534aa6911d526c02e96638e16d405d29bbab110542a0bdf9c0dd6a344bd79ad027c9a0b49ada135f19f07c4525453f74c2bfcb2811c5f33" },
                { "ru", "258a3c2018bba01ea51c249d3e0ea0fb1ff5a6a9e3d025e7b54e2df030f4020689e0521bdd74655ee3b0f2ddefce4232915409bb052c7b9a2dfeef31abe39b5e" },
                { "sco", "9b49c911e57269b46de723138dbd3d556eb774a81e84f9499712a1bb23bee54bfb09ca0ff5cdcffaeb6298e6cf0703e1d278d5ee07e13a0e3081fe81ff69ea7a" },
                { "si", "219c652fdb117fd99b2740ab5395f3adc8b1702feaa04d4b12788117c3161aa26e63a2f0f5ba7f1320c452002a24b1a11e1ee64eff7915d26aba3debbff58e8f" },
                { "sk", "8c55c8582e071f8dd9b9d6f2fccf4404b256828ca42b768aab27c768727de8ff67f9ffce82e12367552dc3b483a6b61589643f353ccaed9fcce91fcae77934e4" },
                { "sl", "902775570fcc33dafdc2039c2d8295ded1d6ca33b38b7dee391efa1478cfb13d2a4f1d744076362e376ff86cb96ac16fd7bd8928f4fff7beb088ea644d44db00" },
                { "son", "5083e7ae738ca4e3d702f7aa992684a569bbc567ecef6abf8e671d2978dfbc6228f1dc4507676ff7db8d5627f66d7ce81b5c044cd6a35cb093770478c050a5b5" },
                { "sq", "e0e6e47f7db3cad3f769fb7ad0e6ce8f019bc4fbea87280ddda9c0bf7782b5d09d0181513f2f06f5fc0163b64732767d468ce2d132ddf2e3fd1fc734ad8ef930" },
                { "sr", "7845334bd7af2195d90fd046aa11d303dc4c3909c523a00c2705ad75860e95b564850f12684e1aa30e91656f569319f8d5a6b4b0b152931215bb165d36a0dcda" },
                { "sv-SE", "3b914f6bb796c60b94a2162af6f5be42e9a3d2ac3822af2396f59183699460931ee5dcd3f6de98b4af0888ba0545cfbe604bca2fabddecfb42e3c7f2b07e4305" },
                { "szl", "5df779bd1fa563c698a8b29783a14adbc95d4c89638c18145193d4f53a2a0bce01f9ed3f7c1bda100cceb37342929417fb55bfd09c3d4c17c72048a6cc0cc49a" },
                { "ta", "50ab8877a13c716aaa3f73194dda0683cbb53eae3b018f44373b6755681be9a7fab56f1819fa205be2652ada2a172e53b5c4bfb450b33f1f44a0bf23c397e2f4" },
                { "te", "7560aaee59f4f483d52838d7d6afd744c786f5da92c675ec2ed70d6327316f2bdae2c76ee102cebb3c8915cc0cfb8df77abd13385819a526e36aa54cc06616ab" },
                { "th", "8d76226194819ebd26992430ac416204716a7307a64909f079c2fd68107d6e4572f6188dd0ec85c67d9408418cefa093c7bcd6e25cf01472c62594d6c667c10c" },
                { "tl", "1677823388b4b69b9653924b6721b16fe680ec901ae9b9be3643e02561b67b9a13709f4e1f237ce353776a199d619c4a2ddb99bd98788dde43839e4beb7a8c9f" },
                { "tr", "c67d615da880bce775c761e16ac7ec6d1e7e6fed63a30437174eaba50336c7aff58ea88c38b81453e7685ca82c7299e516c8f817b36e6eabb03ed0d227e1df3e" },
                { "trs", "eb31f91e9ae36be44b2124625d55964f191ff5ca00dbb408bc4fd88fb0bd7fc6dc48f7f8a3612495bb2cc5b35dbdc7c84de087df8da72394f39831dbd50c4d61" },
                { "uk", "f8b559f02cfcfe7068ffd1824793a7920695baa79e9b7c4585112620ac3e806d651aacf328af0468462db1aaf8fef42bf502e86f4c32491e1b20e0bdf80a6a0c" },
                { "ur", "e001bd2a2a18af0153ce2a06ea6759ecae84d15bf8b31a7001a07ada9e4689904a984ff91910af5784cdb5ca3f49c3e59a3556fe028c63586f8f602e856f80fc" },
                { "uz", "0ab33bc1164fd2ab59b5e00edf450a309933acb580f8e9f90c9a100ddd80980d5ddb5fff871b934938c4bc0e1c625d99a3e107edb4f11d6aa3e9f223f0e6aabc" },
                { "vi", "8f6344f1e82ab9ef38f42907665781525441a0361b096213f62fc7fe3145672b1fb1e4a99c7f752c041c1ddd217b38f961d7a411c26fe6aadff48ea2a16f9357" },
                { "xh", "b896d9cb6296da3dd5f287e96bdeb3028367986eb40a05c7919532f57a2f3bbc2ece96c15f60a0724834b553bc3260cbf9aeaa68d50087c0d767ac1b7d04ae30" },
                { "zh-CN", "d6de1fdf9e5eb1178076caca1940801756927f3c8a8f94de7571ed244f844a82f0b796af0debe3629ce67eccea6e0851daa59a665373b65fa7461deb2316abf5" },
                { "zh-TW", "6c7d725324e1f10d2dcf3450253f456e5ff47686a4be5887bd498ab456265f6449772dfc498861f9336f462ee300bed6992fc181f0561d843e0aa351af768f38" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/99.0b4/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "c0177033b698e1914f31619eee72274c8834bebc7fa4e9698aa9d987bc6abe6eeb53646aae9577214455388a1534fccaeb528aebf441a9d6bb8079efca18110e" },
                { "af", "7faa45f08e15c762b9b99803da81ea94dc6982a2483908761f28eacd6bebd3ff0fb7a46284945d63b249c12f9058cc9df9dcb086b3982e6d07ee52ac6c1c800d" },
                { "an", "358fc447e8c65b884ab6daef4ac52dee030a4c77f6dd34917cf7912b4fcc27c300a22a64f7b98daac5a72a82e127237111ff914f6c002443cc815b2d0189d4db" },
                { "ar", "f05fb561f674d58b5d7c4020fdfe3e4f317711cdce0b4f7e91345e0dd8497de9e46b45c0af83fb29786309627e90efd2702890f2bc6d6a503b6ed9fc79d63cd8" },
                { "ast", "28a693aab393466b7a168eff86821da60cf1655cada3592f638b9b3dfb17e9582c76f30dc89487887dc9e7e8f200df3872889673adf2a9152f70be5505c7fc99" },
                { "az", "77343e420cbc70c1e6400b999f0acafa940b81993260a8e319977d97ffefc4cc174e88c1d6bdbef27a1cf40401f5145a87e0fc10057811024c8f807cf6cc66ee" },
                { "be", "279fb6bcaa01ebfce8ec086973d29771d655bb341d37dc969807dc366b16e9a7879403d93f9954f33f3884906808723d7914c219fa4ffc54dbb5fbb383084879" },
                { "bg", "552b48fba2dec9d4ce06ddc7c2937dc185a5847c6ce39ba4251fd2dc939ccaa2b033e5dc2bb45e9a49d999c2171728f593f8c99c9d5a5978d4de1301c587dff6" },
                { "bn", "032f3af6a090afc3ef798a3f13a04989f996122477906324c1645c897b889e63095e1a1b0607814f723d56a92215ec85cd66443e5fa4717041eba7698720f098" },
                { "br", "d6f18922cf95d371bb665276dcf529b2c1e3b6a9f9f812df6ac708821001337c92c1f768a5e6fc0ae2ff59e975a7e0c3ed77446afbb82f870a4eef4592953970" },
                { "bs", "2f4ab87e368ed72503b0e727581c63535192370bc3c91a8e1b62a7fdef9d245ad168a835b9746995ce791a81cf37f88e3251fe8903cd67ee29960a7d62849495" },
                { "ca", "f4b8f82d072ae30c4ac75c3622bd7ea69cc39a8c9ef1dfc3f8fa50f30076143f6ada68a6097bdc2fa4f4787d1becda74582e7a0257c0e232f5e5b8868e97c88a" },
                { "cak", "7074514530de266bdf7804bc44e6f478aad5b8322a882cdf44499e3f533eb60b8db6b152df72343ff57eee3dc65db1fdf50b41bc0e5e13d60c26372b76a9f563" },
                { "cs", "61ccb8ceea93d9454790dcfabf038f5443c3a523db1f9319925f91fcb4c5c7b53799644a617379c982be40cb07edfa8530ada9ffdfa0aeac968df5071e22013d" },
                { "cy", "37ddd06ac8a2af90b48ce28d20b3c8e6492c57b8aa4d0d253287f6deddccbb554c07a001b3d7a7a9512f603b54f65ed80fd3aa577dd961b092608ccd6e5e01f6" },
                { "da", "d83a54a0203acb5fdd816c6b3b42dd8d85981ced2427ee7a0b3a0883f12bc86437191d204e0ccfd3c6c0e050927c1f1f2baa79218c8cf3fbed06cd4626915f1c" },
                { "de", "4029847829554c141446aec9b089934f39f4f88c3ece96f8c151456f67d69ac48d04f964b79d596290c4eb8164a67bad6b553fa3b34e0039444d0998bd563ace" },
                { "dsb", "9f18e0431a19a5f09354674fdc5ad5c85a7755649731d51977ff47db4792e1a221e5018cb914af48f3a48cc23719d3023be035e6956c705d828c9c114a8a251a" },
                { "el", "d3062d86200bdad2feaa7fb549589a736249deba92f609acb877490d00599a0c9f02e5daede5f67c6c6075f0622f8bc2318d3737af8a42ca34a37ee753d630e5" },
                { "en-CA", "1eff994a63cc6e40a83b978e6e9ba1da93149905cc33b4733b3b4e15782ebab41b6736b3375e79967d8a6f1e296c3ccbd2b915829d2bbe7bc69e7eeea20e5327" },
                { "en-GB", "23e8548a35e48f8c4a7054f91b1b032d4fc7f0955b531172bf784bf72c1164a056725f9cfffde8ba3ed7a4c9e48427700bb27e88ae73fa3ff235014467472497" },
                { "en-US", "f1528e447467d60a5005a7071e876fbcc50d2584622e768c56f11a4877782b8dfede681b736148bf369f0cd8afa435cad9ec069db2c11158a44424e158ab11c5" },
                { "eo", "620b8aa81041310eba1b8d662655a7d7bd724de19b2068d3795bba18d6c7e7f1b00779a37f4a4c01f0683f17ec51a76b751c86a33f3fee55d72240d71907da6d" },
                { "es-AR", "db4b711278f6859dde47ae13e81bf05ea52cdb02a1e0a73128a09af941c116364df8fbb7c5b833b817f19198d026404d4ef627730526c37b7a4c1bc4391c59af" },
                { "es-CL", "0b13b37e9e953e2a14ba74a227d6ed5853dbfc4067dcad1ad9ec190b0b6b126d1e3b72cd7c439a8537fa5e2a27de7404eca6df6c188ec0e678eb8118841a4b97" },
                { "es-ES", "06884f47bca586568f31851b6f1988be8be7a37c0e87c5eca71512e19b18360a7c5e20a0777c189871a5beecf15e36ac4483756c90ffee9d045e0a0d4d27aa13" },
                { "es-MX", "eb82c50c606fa78baaba42df763a096d003c644636f0c2b67da1cd04c47c1a7a8f3aca700790086e76deec78d2a275b89a12d8fcc8c0ee7952e94bd0beccdc40" },
                { "et", "326626d765ad74cb56745f9d7dfdede764ef569be29358536bb6600e3d6b838f3e2656ab792d671c855841b112a41afdb18e0a727385d2576f1f665bff373454" },
                { "eu", "45442303d8430cf418aad14362b4d5109200d2034a5543d41b3f2d3c9815be1c8a1894da51534a65cc2beb0226e46e83d7a4ae178cd44412234cb5a677fb1c16" },
                { "fa", "06de54d81fac2a254dfe86b601219ab764a2b7cf5ae30da1716c0fd0a50406d8023ed805e1976276cc5243985b72a4ba2d07984e7a775900dba61b8b4d78d8cd" },
                { "ff", "0c7c1158565ad977b7f5becf0daddbbcbf07d6f0f0ab156ef9093ee37daaf18e5e1a270d7324f76a18124f2a4182b36cf6913533e6ab5a5cd3adee79b02ae6cc" },
                { "fi", "cd028021ca0561b21e4efc21eff76d975ab8ee1a65df7996bfa6465d2798d5ff219b3b5162f675bc8ea5444b3f010f9e73b609a02e96f05dc35974f50c5492b7" },
                { "fr", "96539c67682f5bfacf9c58c53fdcfdf7557416aee4bf7c3a27a721f3176ae4ef8efe09de8dce3accd519037b95b28b1edaa7f30d1fc2e1a462229ff9a95502e3" },
                { "fy-NL", "598a91fc194400a37fad615d0af8a153c17e6e528544890dd72a6326d13dc34a4fa821a45870a877906f29e7fe8c42a9471c03dd2fc0c5273d485c87915ceef5" },
                { "ga-IE", "181a9b991fb52527a40d91f4049cb1d59344f77f9e954d5997fd7fff33b98fb568bfff5382c0765bf7a1f49e4fb2721714de5a3c15f5839ed04edf13ebc9f1c4" },
                { "gd", "12a5c7fcb93df17d96bcf7c7e72b777bd80643416796316dee9a31f654117c6f811f73dc461227e6e4e6f94f3165ac5ce5ca103381af885ad1b78c50d18cb929" },
                { "gl", "23c3b76a69a89ac4813438c0e1ac4e7ca41af34fa9f88446259f91c7d613dc80f0e5e6f15eed5c613b19b189cf10dd6467d9fef274ca0af1f4520bea4ddd354b" },
                { "gn", "0b79734c95bec866355d3204f0414e39f26d88ef4157055d0c426bef15d1d1df3cb9ff56657dfcc7c94c1c9f946702e82684cbc5d165624b1a7b22b7424965f9" },
                { "gu-IN", "f7a70157420ad504ff7b1978c8f305a93bfcb63626038093cf93939e8c6c51c94590189e37636d18f9547b2699bebf7c2af2113f893c7af6885ebb1554174ea1" },
                { "he", "ae0eb955cc661511c89dc1fab73f45cf916266823043f441383245907e29f5d033802bb344a677126a930cf28b8e2cd4fc3f7d3294f7439762703fb5ab23404a" },
                { "hi-IN", "7b32b682d7d34ca4185fc086fbbd6106726facccfe639e9f84f701e3206bea3f6e1226564e2df855c82863f2ad5c9290edd8ded9e899aaf7a0dd74df53f685a7" },
                { "hr", "efcad4b3a0b31c954b2bac91c73ec0109fb50265e4bf19d27b67fb21b34a473a28be54db8c6a37185a12ec790b84c4dba7c5b5f9da99500f371f2ffd60d73ae2" },
                { "hsb", "8959f721844c957e5aa0c7fd597223124f413418481f940e65a06b40fcf5277774939f08bf6b7c0b413b29d2ba47111c990bbd696a65a9135ebcc1cbf59bb2d8" },
                { "hu", "9a062d9b7cb436c3265cfd0eb01cf1b85e84accf6c40b9f4d9b89072d163a1c19cd9bf46732eb16693fe2b7a388ee01275fb0fa3deefb15f61bec74a97824f77" },
                { "hy-AM", "c743f8f7f87a84c6969cacd1c695dd6d24f2c2e335a4a73267e7f1596819eeab8d790bebeba380ff914288b7bbcb54457a037bdf93ff836c156f865a13f60cb0" },
                { "ia", "58201279370cd6d883f56c407f9496d546e7abb684fbcb8708ca70f7b7d9d063877082fd356014e7e0d2d6a14d8df1d6025c30e02d089d8cfb7c9c63be828465" },
                { "id", "4b36d960e53e988892388018918a8549724e9cce1cbe01560a98ffb1da8dcc682645a086d81e32c9be001ac08e9d6f723088f950add5cea21b51e97ddc05cb5f" },
                { "is", "8c32f9f6b017b56cb1725ea813e0d3b6f46519e256261304783a03c37836f8fdb2ca94a36992939bd49e5e2d52ccadccb5b0b4f8c341771eb668c28cff7811d8" },
                { "it", "ac4ebda6dc0a8de1cea33197e7fa6f2aa649203293ce9c9b36d4513c7bb4110361ce2a8beb6c1c5b4bac6ece34f560dfde7890174e1c0a751e88210c70af89b3" },
                { "ja", "cec6290d2e1cce63641769410ee9adfeba381bfc5901d291497055aa976f0ff409dec5403cfbfea801d537fb8e378773328f69eb3497a32ce9fc96b1799c808a" },
                { "ka", "3a8c99f723832629a8342c08bd0c76e0ebe0b7c27ae12cbdb4f01c72bac0977daf00b520106436b0478efd32f255a107c9c32b239ab0afe7c66dbc0c2be0a40a" },
                { "kab", "a802541dab4850e1ffc289176479f5007f1c1a565bbbf8d68a66b4ee4bd57a044a613783f1169c10c941fbd93d7d21525290b19df87dcd4c81a4959f5f83ffb5" },
                { "kk", "a729ec515a78a3af3d10c23ae799c67440debbc2301fb043797b1d6ab4e2fabde718ae08224290d178a378fd31199cc7d4b1021027370201cb344e3236e76353" },
                { "km", "05210f7b223a9b56292e4f593a06e8ed1591b4c77fff0bc380b2fd2d15815d7640bd52d7203efb1cf2687e12591684a473262b351f042a70cf8ba990c7b1d2b1" },
                { "kn", "9d934d90b9ca47b78722651c673217b65e2b6fc4dafda27eb63fb34a2db398aeed068f5c7fc57a6fbee2f71d67ec9c4a0939447b34eeab1826112151479151c6" },
                { "ko", "6620e30b03a28f31383add8eee14a6b2e39d4fa140c76d5c7ed07ed085eff4c24487098102c8943e84ca8deb7b1960f46475c98b6d867e7abed1aa5556b18792" },
                { "lij", "f59df943fab64fd09838c9eec1f235a1b182befb5dbfcc1872dfdad8ed7a42a4ec347091e253e9e8d017068a090de31d1a094f094555dacc9525db57a4d01c19" },
                { "lt", "6d152762f7190abb59255be1bcfc2d9fb9a4b3c24a383d90f4d832fead80d40b8c17cdabf4fa5e162be1b18701cf2caa1f5295cc3c48932ad41f29ee4236dbf0" },
                { "lv", "42d66e09e3d4e09e67d0c303b4688f2196bc51d7cc0b83659f6616ec135b1df58e0e976bbb88df29bbc93572914316a782d699d786b82d2c8ea71480c1c0279a" },
                { "mk", "96431fda4c278e5b285325fe9011cf3c5071bc8856eba7a13036ca4d75a746314d35261da8c3db42132215cf2b34d6df62172fa229744dcddb80390905a0c5fc" },
                { "mr", "08f7838225dab5b7c227053dfd6f9ee2f5637fd169d6679431b0ea02504d13c8122c6dae5c1c1066ffc9f15077fa1a4d6d7b337cf7bb52626485f547365fa697" },
                { "ms", "6300cd1627c7c429f533b789a7b9e92e1c296c11e019ac960a970cb145f18b2460e78d7e6b5ba0b6c8d44706df1e640597521576f60c7c76fbb31576444a5a7f" },
                { "my", "8c9f18171d5fbf2c43b156dafb554aa9dccea43af7c97cfe8bbcf5de2c0f7a20a60b28fc91dc4d466c3f9572c93c8ce950022487f960f39f23bbf78b7b74be13" },
                { "nb-NO", "260c0b8ba927d670cb919e0b9a90406858f17dbb3f5965fa6bc2c5f16cd124ff1abd0c0c993bfc1c9beb8712d5fbcab2c6386a231ddabdb25b13840e3185a61f" },
                { "ne-NP", "2fbda32ef2943719e7e4484cee35fb6d1bf204c63feeef51d5fc444f1cc8184815e4483c3bc65177aa6ad89bcdf412db37a6b2e0c46cbe2fd7523514168bcd90" },
                { "nl", "08f932fb0d0162f1eb3e1352765cf2093dccbf7ee2fba05b7517f85589019ab679f29574b57e313189cff37b7b36e891ce9bd72ca69893d6d72f11eff7d71b30" },
                { "nn-NO", "b2113bcafc9534e311a8a45e8cab4b6223458bc01ada4d1335881e3c9af1be0718965ba8746860053c8917cc3dd0e49435b5c766d29101fee4af9425ddf04b93" },
                { "oc", "f06818258aae07a4cb8c1ef341702ada9d3628f7532974fe6519671511b45c50c49704ee63336f555389865acfb70625b59f97db48aaebb1a7bbd64c48179c12" },
                { "pa-IN", "bf073d1fbeea6227e93415d693e580313bf5e7fc07ec8288bf434dc17900095c5ef08a0d4bd88666501a332de79f585988fd1844db51810839d5dab7b128d085" },
                { "pl", "4c7a62c58d95e8618f5aba931fb5cdc2cf2f95def33721f883e075044f0d0d406df719470fe301901648d3a9f9a4f4ae20d3b0925d39f2a1df6e778a10fc969e" },
                { "pt-BR", "014eea311e0bba669218a9019d918931410be3c3afd5611349a9b7f9bee14fa40e6713ceb7d2530ad89a6dc38c41b9d0b7dd1fc155769c0b14bee47164db6cdf" },
                { "pt-PT", "ce42087ba032e1d5fc52c43ce42f48d6ee557ca707a5799aba8cccfde5c0ed158aa85ac63f789f67ad41b4fdd7c2bae1b468b36b6390e8b004c42090a0581ea9" },
                { "rm", "d2624e53e8921d0802dbc47f71994b4cd24d58d21a4cebdb7f41114423a20389a0eaea1199c516b42e6d626dcb0aa3ee6b8e52289683da15428f6e3691d5329e" },
                { "ro", "bf84c026ec326ebe13b8ea0eb8eda087da3a2d4e148f86fcbcf5d038591f2579945793ea7df9b363cd1ffa3cbc5ff30a18cfa1ff0ded9751c997cfd56e1c837e" },
                { "ru", "c3aca20e195ee8ddbe4b8ca12fe5a6371a44077b073b7cadbad858a1849c8df7ff1dced86a039d978e9f0c378e18b22167e5cffc7a331b4f3cf481cbd7e5027a" },
                { "sco", "9db91ccf4b8d508d61b1ba12e3c961d08f8e37478de1d74cf7ca580781fc08d81bc1887319ad85fff4a2979bc53e1bd7bee004397877e5b311ad6beb51b81cf6" },
                { "si", "ecf3c5ed9462ce57d55abe547dced28c31c03a324251fb3f7a9869574ab5b046096486bbfcff620e67fe875607e2a9ca7efb50603399491f4fd3a1d791965d76" },
                { "sk", "c70290d24cab68d9bc0fba65decdf2ea8233b01a1bfe62240ac72f7427e7055b0510cc3ead55aed34f07615f9b2589d38cd9bcdc0d89b978359a4de2b3323fd3" },
                { "sl", "f84b955aaec4fd61da5889c01118b0f4f5e5d2cadcfb2fc1f9b3bd5ce93e6d37592df91154801a4ba67290b162b028c5b53c9354cd3080a006bf9cce5ce9a9d1" },
                { "son", "20d04de7181121e09f4e31c93437e1078312b6e1960afbcf66ca7df0002affa664d8f6b8b2ddbc1a269da9b65a117384050e1cdb482e7f385a9a45c474c7f896" },
                { "sq", "8acac33d9f7df1eb96fc50f37834d3179d79b36143ca2b961d47bab179c34c9ff0459fa4d6edf3210475057bc968583cacbb7c0802bfe77eaeb3cd835536c3a7" },
                { "sr", "f2827f602480d32c84c251e98a33bb5ded9ec2add71cfec492ed6414d477426bd8bb976dd95cb6fc502daecb4d1e07bf5c1cf7c6b85a05b73935d621b1739600" },
                { "sv-SE", "5a9f26e32f4f5b5ecc245c65949a3452146c55b531dbba9e41d50a3b3f26b4cff2e9bf90d63a956f16c42af280760aa53593787f600c947475af8058f93d3e4e" },
                { "szl", "530aa2b3fc04d50395ca5ec0ce8517bb41ad76b9d26163e0716e96f326a8e660d74abd6826d321ea2b69eda855c31cdcf1933530ea1438f752a1e9375575bb64" },
                { "ta", "1283cdd234e370315d9c685a6b75c3b1d27eb3f2df32d6e9de1cff0b44c05bb7365ace2e2f482486777026fe150bc85ca0aef84ae9d8b8571a62e03104edac63" },
                { "te", "bb1106abaada8ed1183464a4bb6d5831c47903375b6ae0e5eed343e8736fee50f5331743b4a379b516d3e28f3dce7a958ee42e1a468b9e13ccf8678ea44dffa4" },
                { "th", "18840e3fa52139b1550bb687755b05871d1042354593f12cf48c5aed472f09277c7e7a6cab1517dcfc1a2d4f3bfffe7826330408d8853012034a97e3daa37e6f" },
                { "tl", "0915bcbcc5beeaa795599710811f07e199d1c318f39ae0b31c0c274c26ed1280e2f11d1bb2b9715d45e2f0d4e4db6c998812cb42cfa213dfea6faf9995ca5637" },
                { "tr", "786939485afa0979ffbeb1a5910289ba184c54ffb3885838ef072cc610b538de7181879c03e5c59f07a9d9c6ed4a31af20c2693456eb29b5d8eb08aa5efd1163" },
                { "trs", "311f0c3c39e392e7707f3b67920f08a2d410d7c84fdcbcd0763638e2e6a01591374954d6df13cdb4e756abfd496292d25428c4297b6cff440f0f1c050b2fae21" },
                { "uk", "3bbc1089cbde578066f6d1bfe29e98edd8fa71bd2dc7dc25e553830bdfb0193e64135e21d500b3696787270228e386bb5e402ecb2794c73c9849e42b2e4050fe" },
                { "ur", "c41bb22bc086d6f2031dcee999aa293da66ea729ce2bd2aba94c2ac44228122fe3e110b9bfa5f36cb6f04972ddd25d6daffd59aff70b27bbd1e109c47e17bb63" },
                { "uz", "a7d5b0b3df4bd277d4b070ac393e6f69eb604b5b1c528bc0f9593ebfb6495d7f55b63b2e864d5d2bc19bf58102b9e03f11670c55ab6d5016e3b83d8d2583bb3d" },
                { "vi", "8fac1d01a8bbb85150e6159cc8fc4f1e40c18abd171aa103a59b19d532b22e317ec71f6d8749d9642b456731bf589baaceee7d61c1860d691527b47c1e5a11b2" },
                { "xh", "b1668e075d64fe1be1a4d3dcff37cd304eea662787e09a5a8b0d5f4e176742675f2838fa8bb6bc7eb5be34370571cc1690823f99d01c0ddd6d816defacad23f2" },
                { "zh-CN", "15f885247cd1f15b1c11868017bb6d20f430fd01eee40c674e0c2304145b0eec7da177d2ad27da28ae7d0096098767d98369c25acefacee8dcfe486c1ca7752c" },
                { "zh-TW", "d3a5532734bb560f7abf449d458d3e9208743cc92ac730ff2fe8af48037cdd174ad5adf5ecb5cc8cfc9bfc448a4ae858ca6b16f631551531b3083ef5bbcdbe06" }
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

            string htmlContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    htmlContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            List<QuartetAurora> versions = new List<QuartetAurora>();
            Regex regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
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
            string sha512SumsContent = null;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                using (var client = new WebClient())
                {
                    try
                    {
                        sha512SumsContent = client.DownloadString(url);
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
                    client.Dispose();
                } // using
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
                Regex reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value.Substring(0, 128));
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
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
