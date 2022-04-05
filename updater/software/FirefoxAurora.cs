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
        private const string currentVersion = "100.0b1";

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
            // https://ftp.mozilla.org/pub/devedition/releases/100.0b1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "ebf0e4851576b4c7117fe90e0ecd8219dad1069bb9b55b992d624962e5b3da036558824930302267594a161e70013aae23bb040fbc0d88a2ca6fbf6c2d2986d9" },
                { "af", "a7c7d4f8b23793c9f196c6880999b36a83bee82ce5c77d25767dd0bf56fcd4842084be3b23386f0004549022561ad87f6ecacd92048c1906e48746a2e9a41253" },
                { "an", "22c9dde63eb709392ad36a3092252569f3957433cec9100c25f47193fe7ef601d9f6f60203002e3b60cbbbd98ea59d83a2f118799ac2f42ae6fb729d5e7fdc7b" },
                { "ar", "112b19711fc61284a6b3b5ec01ad710e51da23c96152410885234dd8de870baa297eaa381809462ce762582a4c707beab732c7f8d291650ebe799d30d3111520" },
                { "ast", "f313c20cec6feb142ffb0835746ac513e716a94de855042781fa006223f7591dbb8b15ea606fda63bdd081eaf1327b974d85aa4b7729331b9e681566bc191ec4" },
                { "az", "fc025202d55f94025c35237701f677527a66b6e98cbdfc3959417bffd4a091420100d086f55bd2089914a7ee26d5d596a7c754159e41a36fbfe4764358d252cb" },
                { "be", "8e7f5b03df12b882de3746ac7b899cc3a967e2b2f898a1191c3c7aec94e1ce8638867e8cb967fc676e79361bb9b7bd4c127c2d4bd6ed4a4f2bd2f576768d8167" },
                { "bg", "8aaf94c74c7b9d92912e130a33bb1916987da2016e31772bf8c8e915536325ccb669359d522c15efe39adb17ad2b062be08d53349e09b1aef61edcb6a7b5fef4" },
                { "bn", "0badd636f4317da1d607abc579267081b3a2ae0ef6d21114c693937a91fdd27723bc6e9f431f78934a3151c3f49270d32e8c1b41b2724a0875127a4f8810d453" },
                { "br", "27dffddf40cd028fdfaa9f16731cd7a737be921a1ce96c1880c046eb62695f4239f151607ef6e3fb9fca9741a950d0775df71a96550d8da274c5070f91794a1d" },
                { "bs", "b1ecec3117f7d00ad09bca08c80ba8bd78802bdceb74cdfddb1fbb9f7428eeb60546e74e7b5d384ba1a77d75d5bd7801ac5b29e773dbb845b53aedf0218ea4a1" },
                { "ca", "414d3c1d108fef0d720a9cf250c9db7d5ab105d8f1b7ef7b53080baab471430e737921ec34472a8479adc9b389246f6ce145a8d8c716c589759a55e3120c389a" },
                { "cak", "edf00b27083deb534dace185497bb41d6cdeb70d19199082793ead23aec703bade542ae6ab6b8ea2e63f3edb719f9f72ce63fae0a7b599cb5a640c82599f8273" },
                { "cs", "044de3f0b799c41d702918ab05203791aeba62e6150c704d348739d9e95c649ad76b37f5261115b40aaeb1e0d2d2383e196be69b39244f6b02cdf6d56176f431" },
                { "cy", "8acbf8a3085fe8ad70690d3aae58c124f2ac0691f5c632e496910eec1ec935ffc52d71f2710753e25daf700801d091ebd04fec818fcf4ceaf38b136dcfb06ff7" },
                { "da", "754b867902fea794b9d30b32b935145a7bc3c182334c2ea77d52f16dfe6b5bf07af36ccc2925300974a7e24e68c6bb28f62f3c2ad4ba1234982e9fc15b460851" },
                { "de", "998a8ed8d1cc23f285286b6b9c5736a2a64d57a92f7b58d49579715f3e95449ca529776cb423836cf82cb61d48bdb0a9fbd1dae2e2f681621c053d1456379428" },
                { "dsb", "5f64a648c93758b5fd960fe3ab4d3cfba3fac185d9325c446fa1fed1748431fc3553d5e3bc3f996732dc19aa42b2aedbd4b1992d278953e6757a53780808a741" },
                { "el", "7a5583754a44075820411df9d4a3bf056e2b1da7c37cc376ba0d9a9544768dd3bc6ac51c439c5ccdb29830aba248b234eb08269dc2d14b667bbc08aa7a69e894" },
                { "en-CA", "72747edd9ef134d1f12aead905a6cfa054507d37ef21188c1aa58b490a4b954ebd2395ab5740a08012a4c1ef5157b24b9b6de82dfe165be39ce1419e92133042" },
                { "en-GB", "60710c09c7ad060c5ed1ef2ee6d63142b86fa2865018a5a66c0bda847392531bbfb2615edadd478a4ccbe62fd3ac9218da22aa0b8f6ec94dea769861990ebdf2" },
                { "en-US", "fb3075cc7eb8b0aa5551f85981fd50e492e024f2570183b68c744b092ccc6ada57114a940b93531a3fb26ad418798643f3d2dee3a27d66dbc184aef72cb17384" },
                { "eo", "eb58c456a5f5defd2ec15a0768eace960a4e01bf0671ad4109e2a0443b611b2579d041afdda1a0624bf75d45f00cf3867c3cfd8bbafd93ddb56282dae472197a" },
                { "es-AR", "8aa91c813b9cddd5f16999e610d3e9a3b4c073f07393196668cce270036b775a2f2568c72b4a5b75d9647ac526ca8c5f6fa1b7b2547aca7e2c1af67282ddca27" },
                { "es-CL", "83c010960819a71e0a4a5868c5454bfb65bd594ae67ad9130b7f07932f50a2fd4a263e7243bc6daebbf999f5badb31be27af2d507991a55bc3a53ea76125bbab" },
                { "es-ES", "40bf843843e23ca7586987b3d2bcf067b01afc3474ceed8e4943d1896d587daead885e775eaaf0d796772dcb17ec1d11a1d054e2c25a4d37e54a7c686744603c" },
                { "es-MX", "92009c4d3aba78148121cb8668efdc7169242eeb6313c51d7c21c978475897ead5f7a52b2a961f93f4db4acac9da24ee5aef596b19bdbe492086d4ef1961a4fa" },
                { "et", "0ea67344aec2084be7be04b62a66493769ed713290bd8c52987a341047992c0d39776843e450d6a9708678e3a32615eea4d7721d78a5cf4fbcbf12c38b5e09d3" },
                { "eu", "d4643ac3e2d9cd92c2a802f22347681e3cc2dd764cba5f5123f54819a4f1c486c802b18b045abde8f2e7be543da2b8fd805979339f547766b67bf61dcc1ce8bc" },
                { "fa", "0f2036a2b5eebf71cce27f1c5212f928547fc27c55c60c155456879c0ffb548118ce8a529615309ea299e5c55724f1287a5e8790d1ea27a825978f821cc41459" },
                { "ff", "0786ec46486b6750306c526dca90443ebfc8ef2a1e12c9923e121c12d68be35e92fe87bd022613460c8be07642ea1d8c7f855cec8149ee9130d84ab7b8ab5cb4" },
                { "fi", "f668d00fafdec8747f9d248b09f358d65cef8cf140d77d05b65fa715851d6bdbdf8d259ad00c8d72a07df7212db3a61c61498119ee1208adb2791420bf6b6ebd" },
                { "fr", "f578c141871bcb5b11de9f38c673ccb737a7d460ae6155a83e2b15edb7a1a684d9617cb55f82a219858abe80eed9630ca093bc3fea3264dcf859d3962c9bde35" },
                { "fy-NL", "aaa7754999122d6bbd3746747da17d8eac786141ae64c5272229ea6a30f15f57253d14d994e9fa4f9859dcce06694f83cdf9d992fec23f3108a9cd03d5a062f7" },
                { "ga-IE", "5d17212292363e96b5550e7f9b4407b6553facf91347a389044f8f5cd24f3081973da9007cc1bfb12dd96505051245b6d289ca93e9f76863f1b250f1cb5af4c7" },
                { "gd", "6b0159dc117c20b9f8a6677e13565555924452400912c89a790ba89c42ab62de24549787b1d77be1c1113748d522ae2e1f7a22c0c544ded60ab8eae0fed8e9b2" },
                { "gl", "49f9e0e86a280f17020211be81452a8b72c784f24f614f2ffed0e5aaf2a92ebd9ced9d264393146d6ee788f050e5a00f4bb43745e13c6488cf23edfd6210af3e" },
                { "gn", "d414c9306be6b4b678738ff7d8b0052a9959b067327b2aae40d6381b90116f014b8f9549f3c7dc0ddee49dd64e55b61ab99c7a726351e5d4c36078660976854c" },
                { "gu-IN", "b2597d7578935b90a89c010017c3dc88244a450c3769ed573ab1b8feb4b58c617081af55ecd6c5e97c05746b7964bbee1a1ad6d3a1e0a4eccc54a3be60dc0307" },
                { "he", "c786575e0892fa9f14b32936eb4f28778ccc4b2a412164f5968342742290c9b3cfc711fa32c6031ea47a1b6e46f806c3cd7c4689481bf4ca646161384cb745bd" },
                { "hi-IN", "2506869f6fb7fa15ed5d617ef5a2c197122631a5f3611488076835873c023d3851df7beeaefe657b06a2319d4cd12b9d925caba7c00d43e51e4ef357792af3c9" },
                { "hr", "5a3663cfa9f019439b4390300c341cb7a42e0f869f7ab26bbb859e4c69ed3ac59d922682ee4f06f0b26c28aeaad759c6c805ee30593115ce77e43f7fd97469c4" },
                { "hsb", "dd6496d68c34adcc3e7992f9c2ae49e43ef1b5f0cf945ae6ce322a7f597da76cedf12522b2b1e4a27b8715b96e83cd2f59d04daf332ad01b4ab7bdf7d2a88fef" },
                { "hu", "c187ceda565d66ed02b0ea550a93e429f0125b1fffc051fffc702e0dd6d7a39f0bf5fa9540c973ad15bb92f4c60ecb15446cfe089dbcd43e45c52fff679f4c62" },
                { "hy-AM", "ee619774c63470ae2ea8753cf0f3a17984db4f1a4d339ad696cf3a14693a1ebe83a69f0bd113fe76bd1915bbd168e72d32146da1e4e1d3a7477315c803e64fe8" },
                { "ia", "adb23bb516ac311a0e371e3176426bf3db66f0d4da602602c6349a8f321cd9a00c17a7e4697666c01c9f4baec6e0c54af341dac233f60b757d50cdc3b520c2c8" },
                { "id", "4b1072250cc6efda685b675b0a17173fe507ff86fa1db83a8c2ca48caf17263899abc4a59af9d042e00b38f42f32855a0169901d93f8175e3cff2888df0ac85f" },
                { "is", "90da29f862645a7a24552d15dd8df21a31f826821d9e539a233b82ea33e3c3cdf790e9b02c1cf6f446f9c958953039ed501e32337ae3c62e0dd6618cbc4b83f8" },
                { "it", "db5f905d81d5b1e4b727b0400d4d35c2c2400bed5eb03991359a2025ce839b9d9d1b86be9a18638f7aa99716c4d75c912f33bd566cddac5bf8ac3686b68dddf9" },
                { "ja", "53719cd6801a2d89eee434fa0ad4fc52bc26528db92761de246cbaeaed1954b1652a4554382746fcf9598bc791a07c508370d3653ed370ef7782e761ffa76794" },
                { "ka", "05bdd18e6712fc6c08a8eb08c1127468ebda7715754c75fab2433c7e6aa2983d863886456f86a0e1702aa05295b092082a2245006edec99a65f523637150c25b" },
                { "kab", "72da56436e30b049bf728063efab9a3b73ada038902c92c7706572a6cdd85c6cde190cefa3804016ea744845fc3deedf230994beb96a8f160da0715458769d40" },
                { "kk", "392a04ea80bf67b039631135127fe1b8340e5fae2245a362af6d8cc7b320e2c646db178a4cf4c955b253738890786ab4a24a5afe15cceb31c26a2b26f8bb619d" },
                { "km", "bd6bdf620d785a493561cd3b173402039b1fc2edd9bf3fc247aa59318f97c1ec71779103e8e5cacab45a3e04872b874f36f95dd5bfafa4987bc3306f4587213e" },
                { "kn", "2116cd08db9e0ce2a2140f9bc02c3e719b61685af00c9e2b7638e93d3f14fbfa888fd4f28d3f3793e8e23da5f532bdea30e261637f33ac1ff632aa2c4829b1cf" },
                { "ko", "d19a36c2dbd8e995123fa0a4dca421c66cb1322ae75860c55f10f8e63f5dcb51cc5ed30292be1369c5d3db72e7aa42246ca9695c70b638116507f3c584123eb3" },
                { "lij", "3c26183e25ce31a5cc788c4c94730a254cfbca1fb4df165628fc31d805edc5fedfdb7e263b60af8065bbe6aa96c718ce0b67a7e2d323fc235591af93c5cdd702" },
                { "lt", "a3a8abb5c7f0348eda39f6b46a6b4ec0f5cebfaa5c8076e4c17824500abac636fcf9d01574a35e10e72518d4db6b8081e94e92a63d016ee1ab1bf786f153b71d" },
                { "lv", "0592ecb23769d61f8bc40671dd9dd85e11e015b1da5d7de275650a7d31de43f81da677fd3d9cd9f04ad394095ccf996285232dbc79349db588fc39d0713cd70d" },
                { "mk", "ba04bbe89bf88093c603e903590f740955b0a2f78da69b0e5cd571060a95e100517372fe99227977a16e9621bb60360e9c6c3da5c35feeabba691c438b6b926e" },
                { "mr", "01d66a31ae3c051a8f0fe99336c6418da608bbbcc95cae5a526ca1a3de1f5cb7862a910d434a2eabe690bf9b4742ae599333a38edf2c346fcac150e9781dadff" },
                { "ms", "7d52748a1fa14d6548c630c648466851bcbfe5181f6fd2ce777a94ccd613fab8c32555d581394ad76ac103c4ff20c4424b656ce097b8f8373243b7245da7c4be" },
                { "my", "6e91693761c3a76a84415cd6f1ca339bd041c3b82bfdb11e96dbd6278bdeb9000721a29a75f1c19381312016c98e6f687f3432442eb08619fe09b0327dd3d328" },
                { "nb-NO", "8754e812d2dfc32b7f2045a2c2e60b762292ab6a91cf9300c4df088e5d67d6b88c35a884583042b599c3f61f1359f52da525e33494257ff731586c3bc06c294c" },
                { "ne-NP", "5875b4beaec99320ca9f5a05038ea8ac6cbfff60d6a69efc43468617274044332910d7c2bbb10d7f78fc9b9e9cc18fc2dfd03e273a6a9400667175f7db34ec37" },
                { "nl", "5bb3493ca391ffa39020fbf934fdbdb1b1e8b7f5a85d9e3d4735895891eff78b397e790cfb648f1bec9f70851f20395c4e75bca2c054d3b8933efd63623010c2" },
                { "nn-NO", "a9ce0794ba655c7a33148e5aa4c4c82fdd316e3a382429a8f5a1650837782243386b3d5a1003061311120720e76eca63631f399762b50fe3dffa5b90f657165a" },
                { "oc", "05824238521214474c74fe79a34db58e96137308db91554d551d413d3df6acba52e2927600372c89da27a6cdd894031d0e7aa3f19130e11c10ad2a16ccc7aa89" },
                { "pa-IN", "ccb9781fb7badc983b3a04e294f2e04e2ba503159724d03c009a15f4a2063f242e5fc00c540496e9d3079bf0bb56d83770874be49965e3e813ea804b255a3ca0" },
                { "pl", "868da5a2b35a4ff18e29f06fc82a3b9d28f63334ab7979126906d1c472b15645099182ade4d6002eea38ee9d8df7c377a290141157889ee52ba506c0461dc611" },
                { "pt-BR", "b983636c2e010e0b22f8fbf997f8342de144b4f77f94558f8295ea1ef40c935ddeace781aa694fcbaee18f27f1b56261b1da1b8e689bc322397d8d289e3c6a31" },
                { "pt-PT", "4feae6fdc1714b5f6858c3d2b9e6e623b5c57cf4d5785f4e543c9c40b1df8e4ef6664d584cc4463a5c208df63d6b8b6427903dd55995293d5641e41bb0a082d7" },
                { "rm", "afaca9422b955bad4f481f51afc03b9d750f44692990c74657d3f372d91627f6bceae4d0c4a6be574066d70ea1d9e57754fd443e2a769071a207910c00f306b7" },
                { "ro", "0111e424e04ac58904a2e537a867781ae6d349493a57f284ba9e09351d273aa5cccdc5f82e99f699a18e25066250320e4e067d8e6d64b104905954a3d834f6d2" },
                { "ru", "0c887d9aa44d4a63f0d6dd93a5e7aa8691683d1fab6a53a24f9c26be8738bb318bea6c255e6f00db8252da0184bf0a1e7e1d6f42418094ed7c95060ee7b6943f" },
                { "sco", "06d70f991af3763103449205a8ba23811fbd394adac547509ad830e8a1238a3355cc22b457eb0d90daf8fa147226591573fdedf1856e1473325e952a821fce74" },
                { "si", "607b2439ea03383f9e09e34a47f4f8382672055de5e08a6afb51be6883a469efc6021a36495e5f965a7ed74da80b7f4aec16b78f6118966aeb3d7787b25af393" },
                { "sk", "6c1f2cd68db4531eb74eae377b07d12a42c228ac44cc20ac296897aa4bc36c0283a80138aa77f500c84ad4b5ba39d06437a89b3015530c17688dbee9554693ff" },
                { "sl", "498436a5c0dac0e0ce1dc0dd728fdf546f5dfdfacf3a3c8cb167aad91d90b7ab19fa99e194afc51b131e77b18ed17ddab159b0950b6ce6802a8b88685dc1c021" },
                { "son", "5a94f6fdd2a62aee8daaf23d0bb22c459d2ab5651e5e2a19399e1d98b576cc41fd19e2415b330108a7d0b2998ee60e1df84dfbb5623799e12d8c0e1dfb149293" },
                { "sq", "761a7db0426a62b1366f8a20c384bfad2512cb7ee2e3c8294d32f21908367ef2fb43ff83fb611e2ed7cc032fdfefe539187a17b362c9b795247875a62e52eb2d" },
                { "sr", "41feb75f1568824ce41ec997c3f81c51eb6c49b19481b537dacb0d3e34966d2add4fa397e2186e322e08bd689d233437843064a445a3365df6a048118cc10437" },
                { "sv-SE", "ca3ed64ea4a1b907c2fb7cfcdb8b8c7788b030df07851da03af95079069386e246dee7798344ad0849619bc6e9ed719b42a201768c9c4ef7178f33caee34c9fc" },
                { "szl", "581e232020a6392a8cce3d9ffe8c68145ca479cf292f936940fc5a5d36701b3a7347246b92dfddb2c5ef0a7cc277c6fbe42133809aec9c603261297208484dc2" },
                { "ta", "71ba6bcb1da7198d83762debda604577022992f9fe6ca208cdafc43c7a3f2b0353001040719aaeab7ddc61b86e1c72c631341e052b4079922d507e0af04853dc" },
                { "te", "bf26b77f330ad2fe6c54407353717cb6432faf0ca28c700db8fc2c645d66711012cb4907ce3498a7948cd81785589121833bb27dff7fec0f02e46e5bc60b6bb2" },
                { "th", "7d34737dfc6ab655328fad44ecea3cbe2fca7f07774c8af52cf60a8fb630eafd5ef32fc8be4ae5c49f4cba1f8925267180df3ef7e8716e84f14d9ea28b9dd10c" },
                { "tl", "9ee01859e02efac139540575fc0b8498401001cff3aa1db47b0392969d974ebdcb76d43107d9fbc215fa72aeb524659751b658a42ef77952bd46f2d1bc35d0fc" },
                { "tr", "b9ed7f25a39506b58ab618436ed36af299825aa135cc52a1e708d82c0e3c723feabb7cc2de2f5460367158313c659abc744c2c3208c55874e0b93a2f9d88af39" },
                { "trs", "b6d420fd01578f79bf87ff7609b44010116e0b12e7fc7380b8bae43e91bd421ec405ef1c53b69993db96fc2caec2d3e2d5b6b02cf366eaf22f902125d5afe4f0" },
                { "uk", "5f8b425a90a4ebeb31c068669a1384e7a43728b6d017593233f73b4787fdb2f2c60b1bc0c8024c4118955b84f3d9c04552d745209c5dcc27f5a2bed5e461eac5" },
                { "ur", "2fa65c6565f567b1f1ef10ba937a09a887f7101006bb1d26dd48cd42ee883a521765585d95e946b1a11742763c1767effce110d3e3ac5e4f177d418aa0903c41" },
                { "uz", "0d770538d96feba6cf897d57181f0d88bf93b0d1c0f1f8e0f2ff49858bf0cab84f2c7a19b19b20032da848f842e14980616860edddf6468d5fe3cca0ff8f037b" },
                { "vi", "e8466d7d5c7257a608f02437fd27caec622a0db37860c090d8070ac3767ac33862174a1be4bd5676d14f160a51e02e259ce2eeca122c040bce697d9bbade9130" },
                { "xh", "555d539bd1f28375d32f4cae1c66c8072d0d44b2280066b794ac8b28d624ae4c70f02570639982fe564f791837cd5844689879621924cacbf28484ebdca4e47a" },
                { "zh-CN", "476f838b2649b957f1b43dc21949b37a67a3792d4bd18cd84b573cd7c6a7a51b6d6268f22d95077f1cbfbc4915a5ec319cb5d5dcdd2e4c8f8cfa9830451732df" },
                { "zh-TW", "09a0b88b3f66e049092d60ac02ab24d42778bce990b93c57cda6c3369f489d7b5410b8cf69e74586a8428a02e5b44e32cb8ae80ab6cbbda30805c48a4481b2d5" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/100.0b1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "773eec43372c99f781e8f40a5fbf2068d69e1986dd6360b5aeb329fddb5421e43c1f92cb1a5361a574007c49ddb6db0ab97d0f39f8f3aa767e8fc89ef3191359" },
                { "af", "c89e8d84976ce7f218f4ef214d08efbe8509a2d930cb43b024a5e4ef5798778e33108ac28d62a60c83f9725e9bb33a9993de0e30c8d9b72ff4f16e46c797bd25" },
                { "an", "50695d0a795a4a8f34da1a8b2ef7a43bf9bfb24685d1a91502eb05959c981e38811366fc20150a4968eb581c4763e8438f5e1307ae75aa169633aa8acf84ccdb" },
                { "ar", "41b9bcf1dc918b5e5284c3c181ad1ca73c4616fbff36b9847eadc44b70393d0bdbce04e8ec6b3222bdbb5cb9e771fdef7f7ffa8d42db759fe86f25a776cd620a" },
                { "ast", "6617e895dad63d88efc1a1fe158869517fcdcd611086cf33c65863eef18d125c2a1e1447fe7db2f4c7c282ebe4c39cc12825d383d7bc53d4b5b31073faf9c364" },
                { "az", "400dcae0ba2a6209fca6a2ed9b2f13c638ef37e172f79de920592e347e078210077ddc268a309cbe5b40ba89bc879ff23a4865bcad905cdd9f203be436974439" },
                { "be", "000c68d7f0ca78a27e7e332e420941942844b9113872e393b25ba229aba390013d857c72d9c76d8ea6a3274b922f361ed68e6b50813ce5ff7c285b81e8b5e432" },
                { "bg", "f398cc1e8c1d51147e8608b17153b9f9fece87d702181079409c67f0b0636a1a7abf3e7b7a3449860ad9f3f94837c276f68856eea91661459a9b4514bc1d825d" },
                { "bn", "8c2412e527349d40dfe259e6035da26c18285caf654a71dd0f1805433f82519321d93953a83cbea5eafa4dda737a88160e6aa386bb83efc99356d8790c08cf0a" },
                { "br", "68cd386b5524e0930056d70bccc82f0ed46a26ff78a7dcf7466cfb7fa8db8fae94ecb4a9e3c0586131f1098287164d8d3f2c3b617a9f08404c269737ae8a2a6d" },
                { "bs", "72d79d017c607d4771359d58e47afa51b170fe7df4c01f302c355ca15d7c7fa379cf1c682f127078d2baaab8cadcc822e8e42fd6b19f4a2542a35cf933eaa154" },
                { "ca", "e7613942aabddb14901eec7b5682021b8d02bde73b791ec6f5aa6484d7ac5bccd61ba441ca120af8242c26be99ec30cba0d259fcc0cce05e5faf38b31f5f8465" },
                { "cak", "a8efa1e9edd06ba02beb86a8e8573458e72ce3be07d32459e565d778e7eab7bafbfd296a37567d1a78a31d87c2097b63b9d5eefcfa5314a0480b524c04615576" },
                { "cs", "47dd05403eacf719ebcb3a62fe0bd595dbac634c6b9a38dc4928981e22f6f14713a92cc72170b4a49d0795b6534e6079bef9b1d3b52961363d5b2a3cd361c308" },
                { "cy", "648ae52ce9bf920fba3dee538e98d43ca649d95ba55edcbba42d0b9fb6b97fe869591172abf1a781c3b4990fc187c028f0236fe31ec0bc39dc1b3dfcabcdb3f7" },
                { "da", "773ecde53c565913f16015d150d4139121c3fa0fb1ef81c05694313e6c4a733d5c0aff1b4b0b1d6383fd99f41dbb7847aeedbbb8a8b804f2db87d8940dfe619b" },
                { "de", "319b8cfaee627081cf562e86950ede11dc5526ec8c24f1028e031bd6f5bc0bd0c57135f268a15021a962e8bd9900ef57b462f2adeaad39a4e9b9774c0cec0995" },
                { "dsb", "df771a1b200c5d0b017d3691d54041fcfe4dbbaacc113579324090db1f0a5b941f6c34b4ac15ce254166ddd9980a647c626a50e6d8d75615c83aa2d6f159f72e" },
                { "el", "57af133a6f3efab64c51e8f711a83abd114fec327d944ed7226878152013a196b6fab517c3bd6d90ebb35e9f2bca8883852b4678047022be95b73c0e6d82c84c" },
                { "en-CA", "e3e72c8284bfb0f177a8c55498781ec750cea2e8dbe44466dff9f42cbe4a4c619685b2df91d155c00ebb3e630e01cbf4d1a6b07ed2e6db2b09e67dfbf381da5c" },
                { "en-GB", "f38c6f081493011e9e18835b00ce1d4f38ff1fc3542e3d211d0533593ba05c8d49bc76a82356d05dd90f627c4ccc417c28dfec8345ca0c43cf5879afe83d6ed3" },
                { "en-US", "ce7e5efce3f961489ea29caf7a4672d25258f09ed9b05afa8a62ecd1a691cabb12a42e260d5f066d68cb8053353d233e62843daad48190f4775eca176af7df7c" },
                { "eo", "967e154d388954d88542f67d9c258ce6d812a851955a698fa6414ca040986a248f7e28a7858b2a204299b2fba6567fde466886bc7311c8c53c79514506daf175" },
                { "es-AR", "e9634f07859512f9e52b147638230fec60ce3bc497028a1cfa5de20b6e5f540d221a13fdf931bd53ddf41d77ff7d1f12e942d87ae8e39f59f1591027f8a70780" },
                { "es-CL", "5b9c5b38ca4ec20ce80ce2b9fa527cc047901bd51a49f1a4b18e5d1ed032230e5c749792a83feffc073ca8902f16c111aad1f142d27a50c01523feaa8df535ed" },
                { "es-ES", "e3f87cc1890e05edd05b95887ac7da63e78dc02ba7981c99772bb75fa2adafdeb0a663cda8e3f6a93b4ccd9211a3a2d27d4cf9c17a6a42c5d3aa6546491e9d68" },
                { "es-MX", "de7818421cbfef37230ef4209d5a843c22662de6c35b57599a82b169885ad7f9e26c687c374595bcfcd3ce80c382e95dfb8157eafdb06374c6d57fa68c5459ef" },
                { "et", "986bf3709e2e8a2dd1556556de790c1035b80f299d191d3e720a386142e6a857bc09056c16d042f1e398a0feee550fa0a3e16234234653b15577c7d0bee150e3" },
                { "eu", "d13d323190dee6d3a77bc9fd113bc26168c0c543db43077d8a672384ac47f38bca697c1185e14fe2facdd01390f9f1776bba1f49fcfec630398f42bdc2ca60fe" },
                { "fa", "739b8bf0c24e7e7e82461a09b03e0e35fd9ed95439e141cadf0e4b79e2e6bf8d63b48243024a5be7c0bca2a7f174753a67309cc76dd48cc791470c138c3a99e4" },
                { "ff", "7261a0783b5140ebc33543faf3e9fcd21d39e6703f0419b2c6ff370b11850f59c1bea4ef114718d5b483343e996d38f2402a4c60f6c71380217a9aa468c97cab" },
                { "fi", "1d547c825b08ffb397b7d1202b09a9022ee717029276c0a6739a18635bed1d37fe09574cf886482ac537f0d97fe0864bbcb892ba751d61f7ef2b72f2edea139a" },
                { "fr", "e01e2c9a41997ade90f7094baf7b5aa033adb8b0886211965c9cf199f82e84c8b20bc363039231548afc63aef217560d3a77e5cc3a2c154d204b60abcfb6116c" },
                { "fy-NL", "60c143c98ecf5dc2c0bcd3dc845814f11f6da989e473e3f5c3cd22cc1c2cee5363fbc95c640d3131674505ffe5aba7e36e9331ae0fcaeea240ba95265e49cead" },
                { "ga-IE", "15f66f42c4d772078f4c8932b2cb9952ebac95992225f2c638d11d51bd20b3162bfec97f5cd8cfe8caa0a26a6a1278393e0c41ca39c3a612b79057415d7aac31" },
                { "gd", "f5a692cd2d9b40f8f008d30dc14f2dff7e462d54501950db1ea753f6b5287de96d8db7cbe8c4a39ea9b8e4243eea35cbb682700d22ea053f19e1a6f5a3d2aa6b" },
                { "gl", "a2d71e43b38bb61e30f5c3ed13c945ad02c7c462d435bf58889d812defc5f87cd4b92afef0093e4132d3a35ff069a75d898b9f3e54657e078fdd83b43d9852cd" },
                { "gn", "a9df5e65a089f521aeae53ef8654f3c8199738e96ebd104d778c0d12f60cad7cb2130e71b0e255983e4faeb2216f5aba4d8637d8ed2a6dcca09cde2ff0e59c96" },
                { "gu-IN", "bac9ed8a4fddb537b9e0d1a27354a43e36c6529c066a02e7f1c99eb28e8aabe8694706a57adc76d5326e9f2846f371913a34fca81b83c316ae43303c0a590d69" },
                { "he", "fa36ef4b76110b60ba2ff16ee08f0713e8eb7f6c7ca897e33283b2861ca83f2dbb86d35808e07543698414b67343be82ae8ae40fc6922d8690d9b2588b1bfa56" },
                { "hi-IN", "705f85da4442fb1b3073ea523bd60c1541d08ae56196c87c8d2444a545261ae061385b4ab839e136e2ddc1de57b80eee69360e6ef7f3cd415ec3fb089b0e1443" },
                { "hr", "981104813be022efd745ca4eac8a998be8641e98347cc7a4a84956d9e68afb6b2acb7ecf4230764a015f163827e3153e1ac88f6514a3e73752559cac8c4e869e" },
                { "hsb", "e20ea7e9653db0c31d6eac2f11dbb85724ddaea3b1828186268489b8e42f6643ca8fdec5dbcd4dd9230dd9ed5da75d95c6895ce0c743be242ee3962924679fc2" },
                { "hu", "3bd402ff746b31ce1ecee7c60ca3788d66229c0720751298d915789e8ad86b8c4a43b6eae5002a0d2af8784b7d98c0de82e4a1378e2f674b1b2611e1b5f21163" },
                { "hy-AM", "8829f0462091bd4cc5ef3c46037d6fb62f3a6d55bb1884b77f956c89f7c8ff2de64286a470b58f46927b4311e8197092c3a7cb7ce72bf7e4c84232094617a0eb" },
                { "ia", "40b58127e7ecac38a7dbde5a9b73652e219f581b9df91b3015fb2dc26ae541c03571ef566ab884da1a74e8c093459c4db86e3e20615fe62f5eb42b945c3df5be" },
                { "id", "9ce8ce2c845d15673a9f73642cb312d04243fa2d648749dcefb599e02badff1a8b30677a57f3e108340654ef7d76fe82bda5861611bde4835466639f6cf5247e" },
                { "is", "e7ac41f12b1c3ca0e6a89500f35e7219f0e5f2ced0d4169e85477aca05d6430015a403cbda89af74877aec0906f3a595ff5accd5376e763ed4c7db2aabe2647c" },
                { "it", "4513f751224dd09a3772b197004fe53f8d81e42ef88803a9b1dff6355ee75a124914f04c60e5aeabddc43a81181ead88088aa3d0f0711406754be69bd6fdf388" },
                { "ja", "01491840d3d4ef8849196d73652c706b705514d7cb2d7b08b7bdad2428e5508ee17b59fcadfbf250004baf978e4d2861a0b8e6cc374c394b50ae14b3ea7b9316" },
                { "ka", "a6886e57f52730df5459e9e7803863296502db6fbf8b22f872e15cb926a700dcbc9cafee6d7db59aa32d1f511f443fc2db7e8998495cef1fe385d99e6ddfe8d7" },
                { "kab", "7fd5a4a1b4f7ffc00928d075f4d3d9f3c009829e2e18b447705b15560588ef8d297428135bc133245b2b5db0eadf0dbbb7b3503c9ba01516c0be010869b16774" },
                { "kk", "b7d55f8d5c0f075b5ed66bebf0bc2f056ee3a16b6b216bc82a1bc89a4e09f4d90ef05996a00594001b916acca3b6694449115154fc557c977ba5f4eca159c356" },
                { "km", "819bad7b4220216826e9ec623bd068038b4e01a7f18820df47f7b8eb27ce451cacddaac9c31fdeff4d54df2e618919fbd42cc641efd8261c3c0ccd2158ad92c7" },
                { "kn", "a0aefd0b4c4990d7feb3ed3be50fb0612d178affa401eebc44d88dfabc3cfaddb359bdc93139840def4bee05be558f7a292324b987c5fe3b8a056c9b68aa6f62" },
                { "ko", "caea3775b2d8e106a730ebb4373f5a72c7269210ad24db408b7668a8c64b2f4466cf0d891519d66e0e692a67b2c2025066b2f75396bf1ebe33d44c0d8ffa8601" },
                { "lij", "a85edf23858b395fe6c02b4c0b32df7bf12555e2a39b4c67c9ee890df1fdd9743c95fdad7b220c443f508e135f4858804dae7c2a76140aea71565ac524ee7e75" },
                { "lt", "c36cd0eb5fbcaed9427c806726e8b80a40a49cb21056d3bccdab54f36f2aa2846b5f26c9e89fced83544bb2bfbf7a5f9aec016615e65a991782437e26d36ea6b" },
                { "lv", "7fc824356724ecd939dd815130d97431bf44664cf1e23c671f56302d8d2efd562e98c4c2c4edbc11524f12d6c948226d0207fb87590311e5bb4f42ba88e69fb1" },
                { "mk", "b349e5f212eccc8e2f56b5532ccd96a6629e7e033d5b578712acad278806c63072c953479516de4f4ab2518b3f4346b043bf4265c392f1e59593a6b042844cf5" },
                { "mr", "f9553ff87bf0ba8995fe7fecfbc86c7f766273211d5c3b8985f9215aba758d11775432e99c9811a5d7e5a7ba75dce3836e411ca1dc441fd33ea036ad8d9ab347" },
                { "ms", "e80c826c79968d3b2010c03e07dbfc99eb598e38d67981d00a4fbe4a8a31fa8193c651c1aa4cb39b8b571742985f9fc8c50274fd826e9be60d151b2a21a86a25" },
                { "my", "cf091fe468a3a484278e5f320629526ec2a9c2d70fea6ee17d2cfa344353691ce7112d670970071dcf8ac00ed2d6539d1d5392b3ee1f05cc07c40f3196495523" },
                { "nb-NO", "7ff95e664f915696ff579632359e1e9d4539e4a4bc104811c057de1f04d798e4b7f661c07004f3b985f7f8c829589279d73c765a2dc28d83e93df683fa2e29d7" },
                { "ne-NP", "e6af85a6df2caa61f1b30ffb4919c2ffba398be4f7fd477c427c4d13a99033f2f8fb14a68597a461806ea3bf0c400feb1746cdddfb77d823ebdbae811e4ca834" },
                { "nl", "bdec0f87bfa6e57c014724c987a2eb1d39792725c1e25f46f8261fac1575e15ee3dd6a5be1b8ece2598f430cde69fa8188f2dddca1910b75db704839455cf5a4" },
                { "nn-NO", "fd7a10d5ecf9f8d55486e03ac0d937ac31105acf302501d5c2db74bd6fd2d364c581d7a03c5c6784c8f220e1447ba163bb6bea41f3e276540dc472dd014bc12a" },
                { "oc", "045ae12d02bfca08efc65e282a55b5249b494828194540dcacff37d7c3e9873afe4b5ff7f3e40212c86f95bf4e076f549c034e901794c78b2a739d297daceff9" },
                { "pa-IN", "c989d51867daed24cac09554be80803088d2c0497da03a831389543bf4120a4ed2cf278fa72619d1d7fedaf45b736c2136ea9364aed1ad3a4432e2ad87092493" },
                { "pl", "713f1499bbde35e2475fa327176682bcbfdabc1d6781928d2b3eaabb1bccc33d86687d167fc36558edbbe758a691a7ee208c798dbbf3b88218a9f6bed5185dcd" },
                { "pt-BR", "76b083f5f57b1542ea99b1bbc8aa39737bb409911c4ddd439906a2e01c707925f8be7ab9649276f25101c595418576382460ae0ce93b227b4fb5bb4cb38a3757" },
                { "pt-PT", "ce40b52922e50f262d418b66c1b082b7948a5be3fdb25f74611f5bbdb99b4b924303a1d542e1344fd5e5fe8f802b1fe994eebec381939711d519b43d77e18f00" },
                { "rm", "cbd8241ad27f459310459ae26743ec2d4c03c0eca3076a981b08145b109abf396648fb5dff1fe202c6e1e815fa418a6407c22b0536f4864f6c48d62df1762383" },
                { "ro", "23f00ff3b5550128fd4c2e32886b5383bb4d80060b37677e67a0a5b6a99ebee3ccbd32eecc9609dcef3893eb35c704f51b2837a911000612c6c4ed7955848bc3" },
                { "ru", "891d72b3caa900aa4202fd371973f7937a211fc7cec1ec62b602676239a490a0a952e3e8b3a526d6d348976e217da47a2015bc09b593058b51f5f19e5d1032b4" },
                { "sco", "ef4343a1ec17c6a926f3a9534731beacd03a88688af550ee2fba28b6f1f62ace3d7b65e35f891f1210f4d41238293568e2c6a244dd4e4290b1ca5b924e9ec9b0" },
                { "si", "05ff57148cd8fdb3721d1701b5193dc0554547b838f569002dc6566f69828b7c7433dd4469303b3d010e87b936f1537e57c758fccd243dacb6ba1e8f802fda84" },
                { "sk", "d645c23006ceae08836de34cd6b36a098a6de046af994d3da8dbebf199587c335f8a5fbbf3f5bbc9349c3b676e8bdc214dd86ba16a6d40c1b7f84ba8863e7113" },
                { "sl", "8e2de7821730b624cf966c8b4967d0ff7a669b3dc758ee88686a428827de7cf91c68b17a64eaf17e85909c3bc4c0043e279060789b41c5bb194332d34adff5f6" },
                { "son", "31449a4b9aa203da8814b446c4891ac9e7cf728036a216f9d315a410954c4aaff9177705cb2ce61505d7700bb744516008afffc07a5a863a5f76d53b38979bc0" },
                { "sq", "2b7a314831dbd60c1de465346e02edc1381bb8803b2b5d19544787ab7745167ac04f1ba2686c77228d37bc8552060d25309bf08d9b5f139583e970da3fbc4ff2" },
                { "sr", "1c2210cd01a78b84e874b5049f7f4d871e941f02e52df99717c98ccb468550e0aa4975014ff98bdc093dde95e8015dffcfc764747c49785af4e6703260a16902" },
                { "sv-SE", "c8052074462a553cc256533c3dad61a0c104e94d983bba5f4f2847b8106cdfd0f3b0658665f16e42110b73b5115f8b5cb5c0ff13c84207cb502a1b74cb6c6ee7" },
                { "szl", "5b1252f44345e52645c6dba43710a2ca2a944d4829d95d07b272ffc2c0bff6af5ebf92adb39e1049a63ffc776e16773e0784f87d6b5a7e66ee8f4757c537ba1e" },
                { "ta", "375a5f087e02573465c1a22d3ffcd88946a6e033100eed6636a4ab20b75e544261d91972d579f1e72a27cad3d798c328c4fd9233881304aad7078f63fd8b2c31" },
                { "te", "1a5b82804dc7b1a239541a33790e691d75e15a9783cc90267cae98c8636ea325e159c8ef367fc1b81b6ba984a6f331fec58491ce0b5cd926de292d6b791f476c" },
                { "th", "5755f27bbbf0ba02519443236fb4723a26df3a73b5652d1fec46f401b260def231afd07c9170c333bc6c120bd9232ebc4b7996e0f45d4b68d03b9f80f22c78b8" },
                { "tl", "9a4bf4e4ac12c94a23afca7db7d4ae4481d8c1149fa2c1fb2286060b1d4572ac2ed1c3779d60bbbbad9a8029c14208bbefbc19de48cf4587eb661ba7cf0cba65" },
                { "tr", "aa711e7918897df2b50bfe19977e7c0bbb1f1377a87f302e8927d5d1dfd5ce042794098e40c621fe44d19a6bf9d27931c68251dc0aa1cf54e09ed439d7059bb7" },
                { "trs", "d4ffc69033287d0d49397c59c8bca6c42a5dd90642b86bbf738828819dd81df2f2851176a79fb4db2749ff571f3d1cfed37e53d5ade676d918b7b140d44bb8bc" },
                { "uk", "9692bd1c14fb2eca448577e5f3e01041ea55c2e77ca88cb4b7439ee248c29460d42ae32ebb75528ead1d69633b8385c3fc66ed01dcd44d022a6a79528e767aa9" },
                { "ur", "90a7fb367d2aec25ec26b11c6f51bef41d54bb85ce7b5f44aeb31229c59c58dab7c901508050fd7feebd4e53050a71fbe0c9baf9a761460e76297cbe59ac7885" },
                { "uz", "bbbaf2efb50c83be943d2d7fee4597442c4eb32b9a746667bd64416135a81c3feb24fce342f0579ff063d90d7155db1e75ed0c26f2b15ce8d4e8e8547baab68c" },
                { "vi", "d728576deb4bdbf40a6070be1bd571495f716c1f7168978f0e565f294287cd7a7219dd8e02cdcace90899bc03d9e4adf756cb3af619d9e0ccf6c7dd74b3dbeaa" },
                { "xh", "c8573150db9eacb96c8957778f09dc3db184fae8b5d712a6fdd2a2a5435d4a13e367cf4afe148fba34f6ba3360b85f37fa3a3aaf79e9d5714484d56cdc19cfcd" },
                { "zh-CN", "be49dc7102847768fe386551e91e9b4f2e1119d7266113a8e126f79cc1c4e05db90ba446ad1566942ee3040bdf62b862792156f745586cb2b6709c3dab425c29" },
                { "zh-TW", "c5cfd38e9da3c911a05796952f4e7c40137a9c461d51545564708a61a3db3a560ab854fef15800223a7d597e71b1b23ba0f4039d4eab6f4ad81ca4298d9db181" }
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
