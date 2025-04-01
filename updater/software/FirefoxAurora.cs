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
        private const string currentVersion = "138.0b1";


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
            // https://ftp.mozilla.org/pub/devedition/releases/138.0b1/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "0f3a66c4c398487eada64e981a67b55fe779ab195b5fcdea5bffe15fa62f266d0fbca315c54b095794f0f49910d336daec4ab642b540e79e6e91b02ee82c85c9" },
                { "af", "f74e1b6c09df380a37fa8d39add0395c0bbeb4f1d0906075d13816161a9b0538130eb92a25b598f485e69edf9d8310dbaab627d09b5ed36b10c093993efce9a4" },
                { "an", "cae12c8e272576c0d66e43bf464ab88a40b8b5921124cac8e52668875723ba2adfd125d95794756c7a619a6f9ab64710f8382b2c49f5d6e9f58f07e7619fc8c9" },
                { "ar", "c76420d9c1ef86a3f8af932c4246dc2ede016af7243cb0fc96c238a6ef870a53645ec79db029a1625516466d59bb09b65d71c61eba0db8d8bfb726cbe13a5702" },
                { "ast", "813a5f5d699dd1cde135fa3d4e164441ac22643eacdf8f4993a0e340bf3295e1475ce63e692c393155e4736b318b41c87310d4cd458636656df4d53a1bca5ca2" },
                { "az", "e6559984255f252cd8a9e1c28976957fabb6acebebeaec781079096c36458db6cfebadcb270955b5d46d0bccd99545d713dd3ea9a7ceb9f7f5658ccc09f12c88" },
                { "be", "04d6c5446034ea058d351549379e6526f2b16f76585be6ee5cd89ad88187f03ab36cb7ff2ad5dd77694ada48051be073a8f022b1449d7d6e20f63cbb40f51b37" },
                { "bg", "4405e997a117a1fc61b5d7e3014f95b818b755a87a7c0ef3cc9b96b952b1364f5b00f7249c7c7b9fe8579e409042c2618f574bd2b5a5e65efb758e4b59e2c025" },
                { "bn", "63f36a6656752363ae425c6e83dfe69643b77190cd003c41bfbf1bbfcd1056445e191ab66bee459ce0fd674e726cd206a0be0f87ec3d866edbe5614a4db95e53" },
                { "br", "639eac6a3165d29313b8d87889ee518e8381927e46f58c83dca2bb2f1a1c48db84f1c746efe35eaccc426feac77b68d56b7d8c73f130eba222dc292723a18bfa" },
                { "bs", "4f27f6d90f673edfddf9d7611238f38a38c86ad754d5e229cd84777af6e657c8170061c3e33d5a09df6e3e97695e65b3cc6aad136e3311c9ef4006c106222bbf" },
                { "ca", "ab5ae54409d87744855bf377c3589116db2d55d298400c8e6526ab340b6f649d14406e118516324a7ed28958271483ce1c571005bbea5323f1677c5e4c34740f" },
                { "cak", "6e41848786448397aa0f0b07f935a412f4a8866ff086d542ae779f1a0e747c01c54e829bac86c9204274494349bf98397f4fc059cb819f3b747cb0a3f4af2787" },
                { "cs", "bfac18427629d408b1dbd5d67a53edfdd0c7bd9729f9e86c57bf7692f6e5b30e2ae37ccacc812d0fa0dcda0230a87d80a6fe34ae74d58952a53c0feee792ec0e" },
                { "cy", "3796dd1fc2902f2277732eb344c11686768ab7c2c9257fbeda74ad35184d58b56be2b4756fa378a300c6a9beae48372625d28be214c739ed0a29f297e12ecc05" },
                { "da", "eba6632c9b08f97b35fe7f3245dbfe3055d9b13ab794a8f00736018cbdc012e7ce183ff374af65e20f495c170e874da6af1aa19bae55283d83192f88839d5dc2" },
                { "de", "9d612c4181f801df4e203f0128ec49b3af1e34b3d1c8a56689e868e0ff00f0bb86ac2534d80b35afa062925bae3708bb085768467e8da13f206801ad6fb50460" },
                { "dsb", "2b29709d829a1e809d2a5f83b564e74628d9c93cef851d4ac40a64a054ebf7bd4a16cf86203c8e901e0fab2d4a957596df640b18aaefff35807d9dd58d4570d8" },
                { "el", "34d5f02f24e0c83bb80aa002e3012fd6873389d71b8cfbe2b5320be7f43ffa2eec1fd06721564e02b9e77ebd00713989a46d12959928e7ed958718701cb8419c" },
                { "en-CA", "3686aedd1e29a482966124648ea49ad95358eefe22f99b9b29b3d76e0ea03b163015ce36a2f186075959e602bd0051554edc33b1944a4edc8b8aeaaa1fca3e0d" },
                { "en-GB", "f5d6753ceebfcab8ffffa2095c7d9ac2bd55b04db88fd91e9d1d99e551ec96d528d4fca3e3739a9f6be65870894661c8ebd44f616d0c8733dcf36283f6c97ea3" },
                { "en-US", "b393eacf01f4d9b7cc979683fc5e16e6c75e3557d246de81c0c342c2438fca6f6a95171f8430b8a58a0526b2664fe6cb35e2f5aa6b9174af070dae8325aaae54" },
                { "eo", "08d585d8f6de31a240598d1f30c6d246b0a1a0eae5478f1fa9ab1d84e3004143d379faa5eae3ce4db22a387c13a8d7e0f3cd2a1a52cbcc83201970c1b3ae2563" },
                { "es-AR", "402740d4080309a3dde59fed3cbbb2f85380e7c85d7a041fe7d8c34056e114608deaeacccbcd70839cf56779e87b8e0705f6141874fba6e61aaa592ae156d666" },
                { "es-CL", "db7f18962624b4936476074078f9f3edc84844a4f5dc53d02f9884ef54ffdc9d5566dc6de8b464a36332332272e188bc261cdab6d04a01f56ea0c2c21657d822" },
                { "es-ES", "2f98f412155c3f4351336a29baee6d6eb6d99c413daec4995ce9db4020209ad30f7848b403e2b77471956bc8b2ad219ac58e87fef37ff6752a8e01d1373d4594" },
                { "es-MX", "d8c7109d07ce0ec15e0dae62c87180135b85400b3c97a8cb54c3ba94ad34c57252ab40e6d83b9babc9e30f30492e6d59483590da6e2450f2d559dd1c59d4b714" },
                { "et", "60d73d46c040ff181ece7bd6b334f749a3cd0759ce871f2aef83a6651d0f064456e9b68c415ff81d56894556bc032278d325631284637d2a264b8c7989521a2e" },
                { "eu", "0dbd9ace539b0a42fa339d494b96dd44a793433f0c779b8427c8833270255c7d5c00762d810c2b6ff2a876397f882afbb72153eec7ee30ef89d7001af704630c" },
                { "fa", "77f9f8a7a6db15afe08d3fe55aad81eb65a8304ab937d67b03a3ef1d68048bcc9b896802394a1d7e7449bb59c5600c64242c70983fcb1b3811eb4f63e2644526" },
                { "ff", "d2e98a6e75b99faa40992b0f86ad8f346b83dd2267c0b302e797b542c1e676e0dc7314587e49a8cacd3a24acfbbbb01ecfc140f3d184c5172039dd67a0a795c6" },
                { "fi", "b22a2cdb1ab0759432019e17e14076bda6e5b47f8e7ec9c26f3b17cf8a1f84ec5c6d252dba83e8b208e41c46fa6d49080e7d8fa9536fe93089498b9dfcfcec58" },
                { "fr", "5f89a8d224cbf27149cfb3ea301ee068a8735f00549136c0978de2692bcdb3511af2b029165cb940a7ea7a94d6dc8ead08de96110602928f9ccf108d5ac00dd6" },
                { "fur", "9ae056ace116a446c77eb6c2020eef454db017af99eaa4d9ce2ff8c7cef974d4703b163a4e566f9aa49379547dbcc5a50aac9edd3daa6f4ae9565a66120a8ecd" },
                { "fy-NL", "b28d4456f16794ee98347614da6c144e375d7df012c6671d7c32def6a604504dde86c1b80b31c2c6b3fb56173330562f067f3dd0c67918f92bd0ec13298b3886" },
                { "ga-IE", "b00c5175fce9d5053a88130ee1e0b779cfa1415610487d31710f386aae2529fdd5d71a731b45ae8887b17070de265fa825d8f65f8a3a43264c727c9213a5c6a4" },
                { "gd", "8ed1ad46aa79450896ca74acff6a3dcb31b66581394b156ad6f4ac090a56b5e17173bd73e5afb1f41b7dff7a8bb4dc434658495c82aec7d4be76eb8f4c75b890" },
                { "gl", "6e56aafc4845966802e41b668a2fe81682a878d20ff21bc7c3b981630bed413c8380e855df0bbd2703c6077f9065ea7338f48b3759b4da589a968a7df83892e9" },
                { "gn", "6870786fe7f393a6664ce631f7d54e135c568b2b0000c991f7352393feb9fd666316eb1e9d2d69d2c6daa035b7dc4600269ed6111f88ffbee649790da3ba8515" },
                { "gu-IN", "f3a0f17e1a2dee4f447dbb7cef98ab1fb5cc088dc5e3c7977e7eb816f1b26d861fbcda39e1ecf2193b1ca96d8928b15e5c27e3cc945218c6a4e514f91571aae8" },
                { "he", "65186d1f6e46a0779013b5e72910bdead7168708e379b65dc6ec10fba5dc8c0690fc9b0b1d1aabfd1307002c1c865aff76f8dd5257e01ad240d5330c4dd99991" },
                { "hi-IN", "d119a99f7a005253f4b61465eb8d53f60086662f94fe7c991ef724660bd0385494193c9e65734555155c7a6106184f9cda47f4c238fea606d64e9c7ad960c247" },
                { "hr", "3e5de1d822239529876178e9909b8a6d15c311eba391684b302086d2dd4878c10773793718541b0103abd481b09e866741459a0bcf0e0f5d1a0777f1efbc0197" },
                { "hsb", "dc2bbed7f99bbb5eb326c666be5e4dbeaa29562ce84cbdb400b720eb951227523e02b8208b67e303ccf828123e528d1c1548cca169feeac5548884555c9bea61" },
                { "hu", "5924bf1c593f81e2fe59248d9761ce7ce819af02871f694bb61c0281e64ec5a23773c6222d29a175242300f588e83c6274fe392526960e24880163d64efc6b3a" },
                { "hy-AM", "b0cd1afedd1ff50a6f1818c3e02e6a75711aa9019a854de52ed9b0cb68049d9ea60b017ac6f20581d9b5f3662c397a4d1e28bc59e6ba6483ab39852b77fac471" },
                { "ia", "61dd3ec85fe6c26c3cc86aa086d142d56de4c2533ae69be03220578096b96218a1da6bde3e8b62ce4ee79fe63d0eb954c3216f10b0040200cd79331b5f96f646" },
                { "id", "27cf3a8f614b634ecad391b3c41e2870ada057d1b49866defb880af1e754d7bf67e5d21fc0106974bab9ab8ac3cf68ea51a73cf5e492c8584cde2eaa83d68d64" },
                { "is", "22a1a96036f5394191d23f2afbb46bc1c63a3a5e17db263efb07d54d43150eff459725b2fe46be855add2738227506bca8f33a71b8e9c17955e9b5e9ee6637d6" },
                { "it", "27fc62196216542a3af4eb17100d7948496443bbfc4fa402b673f41a709fe38c2ae360bb58bcc9284b273079bb8485f4c0e356781f57e3e32e5b0bc85cfb4c96" },
                { "ja", "fcd931f48296986a3c407a6d5fa042a70f2cafb0cbeb070665b4cbfb4e850caee316350dfc07351e04cf63185f99624b92e92bff530bcb8ad658721adcbd5452" },
                { "ka", "39d7af0c4c95988fe259980c82c5c8613574127e459d1f560bd80ad3b49b7e43a107737ad296e0fa7920cc678d3ca12a8bc59b6105a95bb0d18ea217244f2d78" },
                { "kab", "1beff9726b34614f5c1da56fe2ead7dceee0dd1a5dc5f9186b0470d847a45a50afe78c950ae9701863ddd765959bd6453c65d33eb80509f6fb6ee92f637a9a69" },
                { "kk", "e5e1c1424a778d437d06c16390878879cf6d57037cc065b38b6b4ad7e42db2cc72dd5f2906ede4f394c09a99db3e34fd1472114321d479b725447f61cbc82478" },
                { "km", "9d0d8992b0d1b48d36ac2dfebec6382bb584b353946493369b80fb666b9d36f24cca2f8c25f18f3d837143eb0674cca0ba0f0953c21ff6c19ed67e54b1620e96" },
                { "kn", "45356a06ce35d3a31c0147116f512ed87873b2fd06ef64ed7d834e9364576f3dd8535c54287fd5ced8f70e3ae6fb45c35ea9fb3b6a5b7459c7abd23b16311a89" },
                { "ko", "452214ed8cf3b0c9dd3a614398f07fd877a27bb9425829fb339a589e65e108eda9ce7dccc3a2823d946f6b971024a3090dd1fea433951803a8acf2bfb93fe267" },
                { "lij", "42314b2a5fabdde4c55ee63f8f068277f0d81eee2cbcbe057a3c709d8e09fd4c48b21ee0cfeee5cc9d1fc016438cab48d12dccf96d3aab62087b650727918d9e" },
                { "lt", "39ac30031318c3bbc5a52930e468f88862fe3eb95d361365c5143bb87932c7d51c772ae82a33b884d653e4074e8fefaeb9e99f5131b3fbfe6d10b9c868649a2e" },
                { "lv", "c3a8a9389a5d8a2fbe1ccb9920c7d786bdb9940b551a9d1943c5ba08264a32a536d400797f24d3bf5386782d15d611785d6ae1ad6fccffd390cbc3564f6c5965" },
                { "mk", "e1bd260ff8d8aaade1e8501bbd8a09a8618faa9fc76cbee401d3d751ece0200ee402022241f134e9a59ceeb2abd04d125738f085b44e466fd3109268b1bfd57b" },
                { "mr", "3741f7f8e7def4b5baae94625a1769ec2d3cea51a833185e733121804a9a6d41cc7a13252561ab8c3e35746a0ba52f1d9d53d307e098aa71041397c79d48345d" },
                { "ms", "38360f977fee91570e7d336d11eb398555544cd9f6acc56fa931981d3ff3086c7f09552daa73b142b396786c6b5ae54298ad409ee426f4297ba2c970f5004888" },
                { "my", "f5e74c0c99f03444edf4bffffa519cd9bbcdefef288850bcbf9daf8b72b910cffd8da2db37c1689dbfdc7adc176cd37b79ad4333cc59b96672d9f3aef28adb6b" },
                { "nb-NO", "5dd75a9523c4928b543076fa7989fc65f2a9c71f18821ac85e416662645fd26937676a5b3789389c55f2cf79f8db860597ae80431a0eb976895cb2dcd9965f7e" },
                { "ne-NP", "db42cd3b4e1e158c077d2bf5c8c2c1f228415e88d15ed94668f9c97da82a149ff10f7256c06884c9c7d3aeaf2877be9406dd7d2795fac76393cce28a4f42e74c" },
                { "nl", "d5a3cc2da2e85fb11b0db956402a47c56530b3812a9c705916486c0bd3512fa4dc65cf87909964bee7ae18d47fb0835a9f5cafeef170ca2a61ec8b1af5fae345" },
                { "nn-NO", "2617037009541e99b1e52caf81af436d7ced19476c5523db5ef7219bf891339220b34d21b6a192ed1920be8bdded851020b6612c3673f02ee3fc6567d7e97376" },
                { "oc", "ff04b2a832e47402162b793e0619722c1c34bfda2a819569e738ed0f477133289aae003c23c6f94faec4a854762084f1c19389ddac795084404985f4e4666374" },
                { "pa-IN", "97c60aabd5eab82a8afa969eff835b2d4f3f6a8de621227abb9e3e630dd2803713635a8b75ae9d77205bb2622d60db29b8fee403fbb1765e19f3cbf3cf3f1ba3" },
                { "pl", "1cae8c445a98d512a1771be285e16ed3e84f331eea826b51bbf3e99e1067405e6913ace6f377c564c38dbef5de77e98fa5084090db493d3ba6ac8b47f388e393" },
                { "pt-BR", "c82dbf9ee33694a14904aa4ea9d4e16681fe44839c5d74d6c234ee115417683be4d08f037bab8beab52f440c1755478816c2374a33c2ecd98a7662deb79ea0b8" },
                { "pt-PT", "af79629c905f6eaec0f4fddd0fa49da14d2640eb68f2a65069267aae5c0ff0bd3043ba5f044a214aadfb1ec8d60506db129eb9ee23660baaf65b97071704bd0a" },
                { "rm", "cbbaf602363e843a1fe8328c5d0e01af80626b7282e908bc6959acfc316bacd184469ce34982f09be1ddc59ec506b57ea84a690e0f28ccb6752e08818b7a22e9" },
                { "ro", "7c46b8d62c5692e71be9929021512c8a522e08f36b9e8112ed40402a5f489f5b07030e7c1ed4f78ef715e01ba8af175f716790e71498dab8bdc5dc07a580c9f5" },
                { "ru", "f991f7a4950732fd0d13af7966df43e71667d1c0cc48508c86f38222557b8166ed190a36d54153fa14160f52227f48a9737dd311167d8266d6129cb6ddc979c4" },
                { "sat", "05d921e138ef720cc35a21eb35e93b4508cb0fc553dea6f494ef738d89f217cc8f0b1215d1f0e035e6cd922ccc58c80e5ac89ce99297ae55820b51969dd8fd22" },
                { "sc", "924a8edf38495371013637c9538b357b778106dad64c9059aafa99d5eb7ace34c9179e08530aa1cdc93ca5d831e79cb52698de0d9cade44b6f7f323f4cb621f5" },
                { "sco", "62fba97db7255b4fe813ce3fca3569d51ab4786ec0397d218de6aa860b120c666e4b9c5d30cc8e7f268a4f8600095f6f380f4ff7ec37331119907e5b3ceb8285" },
                { "si", "5421c3facd067f1e417741d4698c813571c8db50407295d951156360873d6e57347f74ef1f1a6a3167a25b4401a5802cdfa2ccda6d258dff300219329cf4a910" },
                { "sk", "dd4f352f65899d15251ab1c9c7680e68770b5f45171fbeb170d107043933131867d82c500685b3016c6491c47f3942615bbd3c9d7621bd27a9e0281638187458" },
                { "skr", "4f57d40dc3f8dc5b154dec0a42305cfee6a10a8893c7114bccc23b998bfe9408059e44f61ab0cc9eeb7f07b6588a43a1057fb15d1f997eae08f536dec377cdce" },
                { "sl", "db4e43d7f82ce4329ce1bd6ecea331349b0ed8db9ebc313c00b68440359404f7471743c829a0bbb89b4a307b5dba649bbc940e2461aaa79958f40f7eea5dea29" },
                { "son", "5de3b77c1061e4ef55094bccc45c720a3affce0488ccfbc39aa41dfbfb2221550e162922ecf51d06a657e0334be487db8aafbca0fe08d533b82066f427f303ad" },
                { "sq", "12964fd2160dd0935953f0796b82501fe6a6a5a2e2103a1b0214209f6f62ea56e27f87e9d4e1fcd9912fb445f52b16b48cdc2194c3c903d69d9c7fc99593b54f" },
                { "sr", "cd773c21054ae23a130a7776b3d4b9b65464b310b35bdb85bc52517249245459e0c5e4158e232c9da3d9b321fde3b78721542349ff385f68c42264a5885dc22d" },
                { "sv-SE", "44a8481f95f56bf2edcd2f90a8f218d1606e7b1917535314565fc14d0a98ed42775be6ad112be41c4c2dbea655d23adaa58b352b937739d71330a451a93296e0" },
                { "szl", "0c2e7c4eb5c261fcc60f525e5338792eda0b37215421fae7c0786f3c4350045c0290b150475f10a14f6fcad530f4396a42190f50fbea1c8510238dde84b48f0e" },
                { "ta", "15c192be07c2a01244d161a4b3a10c3e6037f1eb807196441f3ebf4ecb8557be37d85576ab0d605e056108862838d29dbb76a89236bdc70f8fd111f9b3c18d0d" },
                { "te", "3101b2da3de1fda465be239576154d3033564e7dd0cb9d650bc0049178bf65571ca27d5f70a8af6427ebac48cb204665ecd9e32de43f59478c5ffd92b27888d7" },
                { "tg", "8d2934e681e8126a7181ae94bc40dda3ff030e8d513b4e0d2254acfba539816adb3eee5a0e3ae43bb638904cfb5dca54c5871b4da4b8e0aeec24a46b625adacf" },
                { "th", "3087202b4aede90717873ed46c0b962180573ef8b1fba83663d862f6ff1ddb024ff2444e353024ed2047f44de312e52bf33db40b0b0889e37edada69aad2d5e1" },
                { "tl", "ec0c4d4d17f4f4322c19782df59f1b9acdaeec4ef650a34130d5cc43fb5b4400119a6b921f094a21ae5cee46062a6bb3800a04f557f533ec9e47409e674bc73e" },
                { "tr", "6bfce4f5a6360ffb1e209752753d1750c348abeb0870094b7b5ec9ea3a40ca3b9b9d38ddd7bc9ba9b5445f0c16c5bbd282a068548f7fa355b26f1f5eb65ab6cc" },
                { "trs", "32ac7a4d66e12377f41a96b3af1778e2d20cc22c20a5e68343f2684772c4321379d0c1f495b3669573a6275eb7f8616a887f93b131bbb70080f7c264e7aa34c8" },
                { "uk", "93cc7a5db3f56f521b3636166c40418c700a46f232b2453c27fa971d995adbb67aae5cb16fd7adca6d3f4508eccc1c125c2b36c01d80e3d6043618849e2c21ed" },
                { "ur", "0b97b582cc851d53b78f9cc0470ab0f770483f27430af85066e197d694d2dc2cd2577501d417771bc50f96208607feea3e656f72cc0d5be60c1638c8f492b114" },
                { "uz", "c5e79a0b14643e4c891e59d0605f12ffb9b1d38fe171a53067d2a4406cee1676696cab80c58a906ec35d178fcbb1aa048683716b5b0f59546df6f19883b8081a" },
                { "vi", "332fa3e96fb293a11fbc8b12ac1ddbf83adb1516ec8f8d9a222ed2856bb140735042adb25001b2dfac5d86e4637fcbf3e6db649b2631423a9ab5ce2e064eda86" },
                { "xh", "16cf94e8cccbfcf0fcab7ea015726039e1799d1cc64b0d8a9a1586f36c71d5163901c971681e0d0265305774419ca9cfc409df042026c7642586fa19632b254d" },
                { "zh-CN", "154d168213df68bccaacaa86b26ce122fddb4c68b2ba86cc07241c80f52d4540a1e75f476787b48d7b61662133874f83d471f49fcbce50b19bcdf02449ee6aac" },
                { "zh-TW", "98887c7653e1d98bd1349deb4ad3d9198199429de775636403175c3937c62db7d1860e25f1090a16325b57a4e2bf6a9de317b41d11fc72fa6231cf797776e7a5" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/138.0b1/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "be4918b85eceacf9ea1ec16f289300ea2a35b0d007a37e6c361dd6344f9ec7a53383a06aeb9114a2b1197ec0cf6f9eea8fac4b3c2e7ea48730e6e0f451ff55d1" },
                { "af", "302460f9f89c6b144010c4034b0758f7eef0f69bfffb9ea26975dd74d238bca24e70955e02f316e6a621093b44980b51f5c907be408dbbac12a64f181a139843" },
                { "an", "f2cf97a8c141af02cdf2bed5f4c56a0bfc95546ca5a68df1a0bef855e92adc3ba0f1fa11d9dd702353ddad8c892dc0aefb74225a16a8c7254d4683cc603c5b19" },
                { "ar", "d37ead48fd0da102a1385b85bb1c22eb274e15fd6d3c6e0c90d500296c3da4f83a9c82501230f25581c3298b878c3ed2f9cd3f958c27e4988c3f481f47d41bfa" },
                { "ast", "2ac3bf139b67179c56aeef71642595b602d040f43988ce991ae1a35b8e59816831320b2c35f4c9fcac0a260208d10d6cbf83fe338d01dbe02090aab458d4d10f" },
                { "az", "82ee471ce755d182f6246687d96813991eeb625a713a0973cc1c4615fe3bb7a02c1a6ec6652624976109b81054bee3813cf86b7289a1c2a19f06782a43e45924" },
                { "be", "429d1c13205d8bfe89f915f51e948e8c4a8712a2346c85481bdc4cbbd42f22a7865e6529d2c4a0e42fe84e29635a3bffafc1824aad1efff673fbaa9f6bb1a547" },
                { "bg", "6267fe0fb6d571b03eb1738142f31b44ab736d7ec5b2d513743b7d3a2d42475a40b9bd504bde27fca03dc06331ffb42c84b39386971241582bd9b8c61264dca4" },
                { "bn", "0ec99e99afb0cdcfd0fe01c7f32894ad15b7c0f73364fa1964fcfba5e484d4fd18abc8fa11ac60b9c39fddfaf8c23252480423e89787d7b9494667f711d487cb" },
                { "br", "670e61a7769b2899e2d26fdc6ac753555876b6408ab25190d585c6a21ebd85c73a27fdf3eb317b8a668da03d53bf845a5ac6375e3fbe96e1b47c0b4ec026a627" },
                { "bs", "8634685566ad346bf44d371b748d3a3f09986a572a698c8746fcd59fefa4fb11815ae5ff512f11774157e1f38e5d256e8263e1dc39d5a33eccafedcde127f4f5" },
                { "ca", "490c2f994969b520f04944f369e7117a5311eba62a261786890defa47088832b0a8522879e03d9c020064e8a110130aaf20980985c22d06baa48caa72e8dc0c8" },
                { "cak", "99c6f945749670395120dc68f89d4a57152559c404de037168c228b72653d008f469e739c0f8d4e8c7cb25250f90bc5502435d1a75f991ef1cc32155afd536e4" },
                { "cs", "4265ac219656cb321c0b9d357086caf75470718402df92914916ba1352dffa1fa685b34d8dfdb4ea801c93bec2d59070a50e50316bf517188a88adbb7dbef347" },
                { "cy", "dba857be342d6627f87c10b1e94e7ce4c6f027bee86fcfe6d6ab569e0c775c648d9b4b95088494d266e37bb5235b31815422e8492baf596f489c1c7fd371887e" },
                { "da", "21fde9af8c3447bf6ca086f9719a1bb2fbf4b7f6a08190fabd409dc8396d5172a18a679b8e6863957fbfd7418271463ad0393c5667d43892901eae74f20b9de0" },
                { "de", "d8d5dd67cf7361c83f70f51717ca70e7e748897f6c6530d482a325ff55df62a59396ed617d0d429e95fdb2077eb2a76c026493118f008fa24ec2bb7a87f1877a" },
                { "dsb", "b4240092440d5b535922e1e674a8ccee5fd3ded8041f5a95b2d840e0d7071a172ab295456856e1d1d03677408984fdba353e4a63f899b108ad045309b8fdd14e" },
                { "el", "1933c371f72eaf0e568fdf1e836c4c62a14597c93845d6d282c803d0a304f49d5808e8742e65d8d8e268f7de2f8575fc84f17550644576b8d5d31461e2ea478a" },
                { "en-CA", "a75c8dca4e97d7608a51bd3757f598a2bc969d709fbabe7d84151958cb918cc17575e1f237cf421b6375426b7cfa5206f0a9bcfef885c50310918c2858c22f53" },
                { "en-GB", "20e749fbd4fad86a2223bd25d211da47aac3070941de9298eda0ef55114614ac1418f6eb9ac0bc3f8438e30edf7938f4f1d3d2344a76173563d9e0b4fa5c94b5" },
                { "en-US", "b628bf3ee396639fb0e759080d3767c10719e0e00171dadf45f717f8a453ef451ba553ac4436f7e7b301eae058bcdb4582837fc954c16ee4c03e6d9811701b3e" },
                { "eo", "09bcb89b1839b90fef590e30ada542528c4970dddbeb59f4110e6dc5a36ec89a3d1468c8aef24633103862f4c9b595a49defcf9f5d75146eb39df771ac3aeef6" },
                { "es-AR", "3e0890b6dbb0a251d74dfb201f878ae591a9a200eba779f558de5c63cff1911812f162b91cc4d3ed5c4eab00c1e70447a74a9ad8532aae5ea6771297de2f50ae" },
                { "es-CL", "f3bff8e443485917b3527e9e44355488369cce1052344b99434e6fb8905018b75aa01dcffa95b67ae08827981fece88dc7edd92aeaac1165067fb0b4d0ba86aa" },
                { "es-ES", "c6b91dc96c3eee073e86f5439c98455a265cc7f2888475a540a25737caf5673747b034ecfd5eb4bcf887e255a897c9f886271c9621fa4a91563c535239f6cf72" },
                { "es-MX", "9cb9f03a4f0d6a67f7219bc841df91d01399e14fce105bcfef9195c76c5461edb78088e7625a2967e4dcdd8e4db9f601ee1edfbbeff20adaefe10405ef9eee72" },
                { "et", "9aca1451bb949189de9a00229bf7bceaf2b5ff52bdfdefa28c7983c7bc3f69c0012e9f0f2a8a39a72df70d33ce2c651755c700dd10c8a715d9c30de5061b61e8" },
                { "eu", "75c2b63fa07e3f8f75b9ca939d398775c7af2aeb09951b5ffe21e031a063770a16c586899a06a06531cf2e1db34d4dd35b7b62e53d8cc6c39d2d96c93a320db7" },
                { "fa", "c989a411911ed64e3bfea34eaf32b5b065322c4755de7403510ddfa13a99d5e45e8e22b74e88e9af17b397e983de46dc9667dadbd019c1ecaff3f99652882a71" },
                { "ff", "d6b07214a0760fea1dba2a405190d5742f682c9491e1bb2e525ca3534022fffcb093032f93267ace36e83fe3a3e9652b096641830bc634a4fd0ae0a8e05937e6" },
                { "fi", "3de4aeac4913d87ca406667da2881e987076ae5396013fdd9d383cd8a775fbec2f88cc02e46300608dd61f6aa88fcc940d0f87438333b44175a892cc45b174f3" },
                { "fr", "4d49cfdfa8028953a91a0397692e11410737be9983fd084f18bf1b08c32616cf7a7793312bdda7ea1288e6eb9fdb796780238970eb0ba0ee35a9d66e1698d0e7" },
                { "fur", "41e3b584ba8885c33dd2136b95de3f692da522630ce89390a62789a4115ed4de8642fec7cbae34b0d2f5a6da368bc73cbf74402d15f8630706a6b21289d55cf9" },
                { "fy-NL", "1fef4bf908ee674837d62c42ffc3ff8acf640d9737a80d225904168920c5d2a5a74d3f52362eba5b692065d4748a6c1fc8a38597f3401eddc5fa3c9542aada00" },
                { "ga-IE", "df325d91340275769ebe56c95a690ec081096c15bc2d79c4d0f3c21bdaad170125cf60b68fa07470262db219410a6184ba4e779e0910dfc80a3527d90cb132ab" },
                { "gd", "3dd9e373d6e87c899a82d70d7b997d456b63bfc95a42415fb14d95e56cbafafd3c2dbf7a1bedcf7ba9a4045d154aaab551d81f1856326dbc135d4130c379d544" },
                { "gl", "2a6e8a76d5b8bdce1a896472f7aea9273da307cc1408fa87df6510cecdbacd00e82f52108bcc957964ad90199615623f27636b14a202d3fa58af98d8f284f06a" },
                { "gn", "b15b50ea3233b2dd0a44df23e672ca30ff37874ad6f92ff0fb5c2a79512c761b36ed19b38a7981644676ce309a6d914f765ff6f9876794f8f396ea42626d3cf5" },
                { "gu-IN", "3a26f76f442b5bcd37359c3019dab6b63d16d342487aaf03e61118cef141ebe8c335a9f8c70bc3e35cf27673c49360dba9554b3edeada17da568b571e1773504" },
                { "he", "cac026e510c1c8437d84470bb723977d5d24b33a8079289ea2d7b15feb628fc572c2d1577ccfde9cd57d358606b9506409d7045aa7eb0380cd0b8a97a4463b54" },
                { "hi-IN", "ce9538c37a7bcea0e0c320a42229df359f7db2c91473227ab519f67003a6077903074efedc5c69bede8fed1ade61d84714a70ec50ab7c35b2e418dba1cc1827a" },
                { "hr", "ca02f547588ca7fc1f8572285bbcb8b03f37c6eb602bd30bb7c0331523a7b86bab796bcd7eabf24f0ebdefb412ec2a7888825ffecd71c0a9383328695a7569c8" },
                { "hsb", "62077d076cacb8b2856c5fc87da0cb8b30d6bb6c0d84cb64fac379ed234931ae52961ea2316835016d2da9c6532e9b0a3a216c2c2596100075dc838ea9ff32b8" },
                { "hu", "d041321531c2ea3f29c741748c27bf42b4951802ee7e3f92a48338c98efb17552ae344a65e222c00c51ae62ed43e79f383a921eaf0d480755a376b1794bdcbf4" },
                { "hy-AM", "54b47e8555b31d717114c9fe3283a0d2cf52aa03ebb4c68c09f9b268603d57f72a6850513675d688eedeebd96178b56d1fc47363c7acb8d39351cd8cf7ce35f1" },
                { "ia", "0b142eef7cd4aad85542dfbbdcf9e54cfe0aef47fdff14b7055ecd4bd85a40cd16497d6ad2e2100d208eb8dbf45bea25ff6aa43d3c78006232d69ebc062955ef" },
                { "id", "77c5f97a420359466e702ab6446ed4d8ed631a564c09c3c198a9fd39541d93daaf5be7c486f9e749e96c8aaeb7ffa7690dfa2f3231844206ebe5247932f53042" },
                { "is", "8692645c0d7042277cacdd351776a11ce33cd00c6ae96c4150950f8055318649fce682a5b9cf238db2037b0df6ba3dcb25be44cf5447d98e7ee03d0e4d71b10c" },
                { "it", "de33a8377a33d6b6cc461ebbb131baf94d9c462bcfe580ed2b8d386623b6045ce22b8e33f734c4f3356748c4ab7d5695843806a8d67a578066c730d2f7ee41a6" },
                { "ja", "07d35437d093b3effd4386a22e47be852e8ee5ad90eea2111f2b4d0d0ca04b48c6002333288846c046c96ca38afdf2b3f743e399acac9a512fa4ba574e288ae0" },
                { "ka", "681b10e75464318ef68b504741afc6795a6ec547d90693ec64fe2df50ebd482b749541efdf8ef342d1f7a40a83a6027a30a5f6d93b0cbc7de8c12c17d1da9f95" },
                { "kab", "7acecab6748030ccf95fed14b1e3162dbcf541834a2f0e0009c2ba6bb117f9ed92da701649521be66be932703ecef64cb30322eb36d6906b269ed88d9a1a5ff2" },
                { "kk", "095d4d4cbc3b2a84ee49151e82338dab52acf44c129ce320824917b75edc4a183df21f038c48c11214d8de29a9459ebd47b5e94589f4d6013cf129857796552f" },
                { "km", "778fc16f152a18880518d293ef55db60af12a8122af4abc8a732e86524b57cee1757fd0c5a6500332a049d9d681712089b0e3a2ae9fd0fb7299ec0decd0ccad4" },
                { "kn", "6b4b80f162e9b20f930070b19c476235a8311d22f4ac0232e598bafc2251a0fde0d8b57488151da775e84630413b5bc6913898a6d05af8267638458b213f25bf" },
                { "ko", "9b291e788ac71ca40ec28a6dff7ae919eac5d969f38443a9dad9f47a468205c98d47a1d4648776061b816b8208165245c7a6e3658d1e93e5bae42a95bb69625b" },
                { "lij", "2f57c0643a76379d096d67d0c994ffd9971fe36bbf7cd1d15238a0c1bed7cc109cb0818216fd5908dd0fe7766c09db2015c77bd2115e835615a61bf981855ce6" },
                { "lt", "9294a714dd98923fc502088bc699b3415b672666ed8f34822a76d8688ebd31911f5e7a65f73832b9abd117e8f099814b587f1e6a41bcc9cac0751c2e0203fecf" },
                { "lv", "f2f8ffe1c459abed7c0ac8366ed61c9db492596a4652dd29258176a9ffa656558ada66e9b1ddafb98aa4e9f5ba884a70afd3ef19e74adabc3af7c578b3679803" },
                { "mk", "22629d49de3c7b1caedf846d218dc45de40ddb6fe297e5b4da9b9a2a8fb3cc52ad7bb892fea90d378b5291c4732cf65f7f398f27a16114a6194ea7cae0c22e1e" },
                { "mr", "69b82030b95a3a9bad9cfd9af54bbd2c62dfc9a95973b60155147c3a489af058b22d51a5c592af43f43a6ecb0abd4ed67376d7a54ecf5cf81276fe75fc75167d" },
                { "ms", "51965e053b6515c82a2c5b48f6ae4a0d4aa0644bb63a07263c5a82cbd34570a93d75b44b45d7f1a33519658abf9df0a15674b04c09af8a837ab0f945351e88ce" },
                { "my", "65f56b339e8cb0c1427c5f2b64663486c12867fc23721e5a412ad59e9a833a8ac5c30f6b0161b0ed42015ddc3f08a596966d83e525de1a46b131d6847662e71c" },
                { "nb-NO", "5242fc7c59db251970f3b2ad4e98b6513bece909cd452857005bf72468d53e4fa41295728875a04b164aeca9251a2e4f2f52cbe07d25a962ed6d5da30e3cef3c" },
                { "ne-NP", "6f46166d099b7014b1516ab2e0aa0f46447e01b1bee9815e79dd39f5961c428290f803b3e0eb526f9f598951b8b1654fa172f3878f402e23811d1036515b8d69" },
                { "nl", "710fa6c3fdecb6be50972112b1e7791c52704001bace779d71f4d87f30ee23c99c86ae3c5a9e713b1648e62ebba506eee2f37d6983e1828f5ccd14439b2b9f9c" },
                { "nn-NO", "897c6039d1ca0b1bae3f792e0933f1c2956c2dd7584a6fa1b09855c55151d94e4e088bbed63cc51ab4f40c0e4c43f7b9ec017c097dfe1ae57d6ed96d6e4bf580" },
                { "oc", "8e9eaa37f6f9969374c30782a5e1573e12c51c2d0dada1f0225757c22bf21fda63a2d0835704741d11321bbeacd83d70021b92c8c5508ad20aabd62c36c7155b" },
                { "pa-IN", "d9a06556f8b30fd8c686a21909265a7b5c76395fb8c51eb76edd8f2d2f1d1b2f4fa845f79babd3152ac2a20ca701f444c9ac3c5173b6e24b9f28589aa41187ef" },
                { "pl", "0d045e56e35069c7f31c12c34de907342d5b056471ddeb8c7973f1cc6a6b5b78614a7e69e105527e2cfc7080dc0099bad139b3be63f67c09d4d192f59f748df6" },
                { "pt-BR", "c1fff700152bc73783594d9bc2327f8aa658cee57961aef730818a72cd874065b594789a5e12993431a4b6a0122aece63f27787d38dab89692c3e6849befebe7" },
                { "pt-PT", "cb5cd9c67e4d002ddbee81afbe959f933a40c5ad5e98a39c483dd5591bd8ce5de5f1b95c2702fbdf4c3a788e2e0eb5f90cf7ba02607d10695ce50cc6a99da60a" },
                { "rm", "77baff87061693dc475cf78fcf81c53a0bceb50bf6490a728e80e43fc571986b4a0340c88daa207427a27afca255c61fac0750162f68c50768a7952fd9c8b336" },
                { "ro", "4c24793ca39edfaa1576a1ce05758abac89c0c9ccb98d35c43b862569a97b3eb232404c77957964f01d7eb4440b94a94cf29ed395b048636d5bedeea25041ed8" },
                { "ru", "de0b4a7aec8bdfdded5712a83fb97fa4f10ff9379b2a93808b12b6fa3af76ef55d11502bf97fc3ef07a7331a82d91731fced90e3ba7310e864b2cb98c0dd54c1" },
                { "sat", "3d3530d3deac9ef0184798ae14a615580c565813f4252c38765e1bcad32a67bd8a01ae75a93e3610aa6596b2563e7c459d375046459979250d28a37dc1153c48" },
                { "sc", "43505493ac6d04f200ee964168a6593562a240543c6d344841f45dc9f063b6a977c1f4d1ea630250722f5b0cf113b58f12fee5b8b37b7cd1786fc52734dcb692" },
                { "sco", "1fcec6e08e61f05ed6783d3119101a2060344278dda0be291881a8240691d9b91dfd8d0b3f3c487d06e68952d6239f33195daf1034146db155f2a5582b668ecc" },
                { "si", "0d01a70c8490e32fc504afa0202fdddb015efd0f0fa0ff235b7f44383b3780ca48e26cd24d23fc6be60372592fcf25a90d5575658bf5557be38a76480406f111" },
                { "sk", "61919b7aa0c7b59c309a1f2e98c60dcfec02389da5563e52683c4b073085d8316be095b4848232669d6ec3ee55bf03e7628a1902e9d4cef0161e56d4a5f44364" },
                { "skr", "75d2ae03d17ef4341d0c1a0a5bef87ee4608523dd689f501370f1c8bb782486a1e5762995ae5a13249be816fcc3bbf70a4b5df14fc79cae1717b978278f3175a" },
                { "sl", "36a56d4819e6643140881c94dbe48fd0364274e65211affd12f33567fd37dee618f7d83cba4ac486fa9db8a70429e09f73bfab6b623c83ef5e4e0fa4b92be770" },
                { "son", "4854114b2bc742fb1b6bfd11171ef701afb566a22ce0bb5b1958fc160eb34c6bf502f686a8874d41f9c9e4c07b24c544a07403ff8e99d5151417e1298ca55ccd" },
                { "sq", "11ea889097c6bbc5204f8401ab74158368c9929fe08420f69f52cac4066179887e92ac01547bf23a84ac5383598aeb1928dc331dc8cb316bfdaa13e97002b221" },
                { "sr", "8a20a0894976d4cb2be9bce7749b8f3965d4bbe07fd6538a0d56b7eb22587e2cc2a5e80f7d5eaff213edfebfdbc2e7c839010672833e251a604556aaa96a327f" },
                { "sv-SE", "29771587ce9028878bc6e4a32bd94b446afce49b2510169a5521e01622ebf527f8e4d156b34e629cc73a3000587b7f9235a0678fc8988d4297d08fd2e116a6b3" },
                { "szl", "9582e2ad2a02e9e68f7fb8a76606cf2875552e0ce7b2c4cfa930596c10bf1d0d897e57ad87798abd8c7a3cbaa269569ad1e3d8979e73c1a1e0ea4fa93ae27ef9" },
                { "ta", "8ac9fc2ebcaa9ea3af248e12aad18ee9f74ce6b2420d7b9ae273d61805f1d3f743b3ea0a822b2fbeb2f357e70c3373cef84136cdb1c87d4efca71b9561fdc737" },
                { "te", "1e7d01202523e827dcbc9686457a1e9c1112898b4bc07f032d4ea9a7dc22cde595d33c8ff433d6b49d0a0b64a80a758d58899fbb2df75445c1f7852fb3b5d759" },
                { "tg", "4f3bdbc2d005918233331417a8ab5daed9343e331b24c5c934fd5534c924b10e9e44c08bcce12534761ea2c81fb8097f6e509df9d2e0fb302f8c78e641e83c64" },
                { "th", "1dbdb5bdde176881038df9ea4ef7ed7ff097995d30ad9d51b0f101408ae9daf43d5ba0aa7e3f92060ba36ddf0436d5fbae6237ed00967e548382241422d34d6f" },
                { "tl", "8b8e15c59192b5353a07e528be0914ffbc834d96746b6cb41700bd9dd3c3ff27bf36a9d1ee327505e4acc81dae49772c2842d2ef719be4e8e5f97d3309eb4e52" },
                { "tr", "677af8a965227a9030bd5d246a7c33a6e4c03bb740f51cf38e937f32c6596757479a130edb3da72194d67b412346ca5670ebc56b189355a2bfc5f6846cb449d4" },
                { "trs", "6ba663b463aedf707d022e8dcd6508a5167dde8ce7e40c047c24f92704bb600efef67d631be2935f765ea78f7ea48d700f2df07020dedcd8e431f85dcd5a7b13" },
                { "uk", "f9bc2015b42174d40b9feec553637fed35da1227a60a4c9194783b57b3a6dd2418aa19d53c90cee8dc5f0ba299adc6a23bb08b7d541493bd0cbf2d865dc52367" },
                { "ur", "346ce984b451bed6bdb6a2ea297fff2890dccdb3fc09679be93dd8131dc09ecc3923ca6afdadd26762ab651f5389d8239421a89e61babd8e44f3f0fc0e55a227" },
                { "uz", "19256e04e1cce528280f5c95968c4e0b5792a35ba919a7d0f3b12ea9c07f5ae17d525fcb075349ffa79d783fc81899dad9cfe19fd48fd95a6afbc32e5bcf41c7" },
                { "vi", "326875415c00a8aed2fb90faab1972a10801ed143227fcd80ea1c29d7829828159a410c5e06bfdcf3b3d862c9f2378408e8ef12c250d2c48871a5d720d88cc33" },
                { "xh", "c7cc5b19ae57cf265eaafd18cb7eb252cc23f3da47e749bef6da6ad6a812b663f0b3f87c04b3aa13338e6c8113c893fd9397d6006cdbb77763b2ddfc31f3624b" },
                { "zh-CN", "e96d0f3056c621cf1ff0305901b76c466ebdc43a8eb6ea26f280d6d7e0db18b3df7e63f710aebf8259f769c3bc43c5ee649710d2ca896d978467729f8dde7f04" },
                { "zh-TW", "ce36f373282e01a5da5bdddcf9b1be21427e988ca481563594492dca70e8e238e79270557129e54991a24316007e972597ae55f1542956a790958e72e4c69cd3" }
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
