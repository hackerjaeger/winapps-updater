﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017  Dirk Stolle

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
using System.Net;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox Extended Support Release
    /// </summary>
    public class FirefoxESR : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxESR class
        /// </summary>
        private static NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxESR).FullName);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox ESR software,
        /// e.g. "de" for German, "en-GB" for British English, "fr" for French, etc.</param
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxESR(string langCode, bool autoGetNewer)
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
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// gets a dictionary with the known checksums for the installers (key: language, value: checksum)
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/52.4.1esr/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "56f9c1192d27087ad7a4e079138b227a275450b3d86080c93ab620a4c2ca7fc44bccfdaf3f865e7e6c52104db0ee8736511ebf4c7cec800de39bf1afedeebf27");
            result.Add("af", "549c18cb997bed8dbd9cbcd5655d8bbdf0980bdc3ad73b1be51c9c46b2dd0e4e23722cc8365ecfc349adf9ea07032bab346be599ba6a6454da9cd85dbfb96504");
            result.Add("an", "12d8bdda68c0b40d9a741e9c13c8a1a1dbd9bbc6008fadcbb68fae8205f1f4b2bf77493a2bd13b15a503b5d1428b14ae8cbeea2c7e74e4efa23dec6e5ff966f0");
            result.Add("ar", "804951adea282f4e4e8e4bed5742255950881dd999d5bf798e239656be7a6bd66635fec101abfa81daa3fdf2e38e31f67bd5ba988cd2a4ca9c97aef47c4c8115");
            result.Add("as", "28cd602256d522f4b58dd1690cc25a51874a415a9a05943de6e0e9db962ac0feea0baf945aff24b253f9d89a152728705385a478410e2cbd3e4541de69e3ca13");
            result.Add("ast", "7bdbaeb70ad71dd10e6d0ce5d88f425f0a5a66ae961cd8070d99357a37675518b90ed8adc373defeb8f4eb2c66bfeb36c47ec0b30c6c882813858b791c161cdf");
            result.Add("az", "cab58413269a83648c2280e301f83d933e95ff728dec94f5470acbb368dde74d1c8a7b3a63d2eec0721d7338ca92ac8d6c07e52d879024c8265b88f09ac5e736");
            result.Add("bg", "a2da68ef5eb7dad6c0de0151df11fe6e7fb8c149d74ba5151b87fb0d6fa38e9ea1d9b2b08922744169f1f65a3ef59d23b1aa1411818aeef940b54d7777ec6bf6");
            result.Add("bn-BD", "1713d6ccb1034650977627946828e1ce6a91a65060d7c6113c57da7cedab188c8227e2faf6dc750a5c49f2a7c4fd10cb5eed0e81764e9246966f913d70d80afe");
            result.Add("bn-IN", "2f0c4960933008b7a07028a693fd91b185c84ae81d9eccbc212182a74cd871313198a7c4ee30fb082da4d15aebb9a47152e07d304bc6dba1583ccf4157cd38d8");
            result.Add("br", "9469a6cc8a91f110abcc3f02eb5b82816437a032692a5bbdab14361b64f5d10ecb91b20f32f9a76fa34e0477ee537777d44fb3c075778c64a16502d1bb3968c5");
            result.Add("bs", "a1227c432f3cf1b9a461134df456e25e33d3936eca92c5f50fbcb0d45a46841b9d7fc15ee11a75fb4d55ba9eff85a82ed5434567fbc3be2ab32649bbc8d4e463");
            result.Add("ca", "ef8c5bc0d5e75a032bb6eae09422c2b2980f88c4d6bea6b31a4a1deec93e9a3b855da5f0d69150aadeb269c584f2780bfa040f38c49a03ab04345d7c86b5a13e");
            result.Add("cak", "10903a67f5c4b82d6e22cb54fa882df2dd02b63d67292db53c910a385014005d6c5d6b22efe913258b832a9067c7a2adb5331710e4ace1f96b1a1799debe7e43");
            result.Add("cs", "71a7e82cb1c1d068e6babf9135ead3c1077d99816f5f323583a245dc9875cbf22df010a3e1cb230236705acde005c4013d47b235329252caf18c4fce0082ed1a");
            result.Add("cy", "536695ca529d01263f136617653ed7fb691bd4e691643cb89d252f4f9b66b40d28db9c82925a5a44e8b295378e1ba16db7c13d709963e8e8538675d4407a8bb3");
            result.Add("da", "d4b6db574cd932aabd878a41fad14af84646985459acd8f20fb7acab73560121d8f61a7538c3af64d8050acafda75cf300db6dae682841f7e047f0082355cbd8");
            result.Add("de", "0f4afa013cf9a86f8ad42b08adb7fb07f0b9f9c0d1aa51b20e3429aa343e402d655e2202f32cc14a13252770437a28ad07da3094bd1d76e3a2009052d0af69d1");
            result.Add("dsb", "3fbd74d1f811d687fa85ba3c7a407d449408aeefc251a1a898db454bb7a4850b9bba0438e4fae185815c87ac7cc848cfd15b3c6efb01b76939fe76e32a9bdfc4");
            result.Add("el", "a881d4ced27848fce4d94a8fcc91126171bde5a846dc29fbbe9e94cc95a27ec35741f26177d2b842eb993407e89c4c3abb78671adb3035a20a2d03959b41d916");
            result.Add("en-GB", "e0b983f181e9cf5aec44fd45ef49e89a817bec455259f7ae9b67c7a756b54a2c0a4a3f60639d557be739f2a2bd98ba149afc8279089335d0710a8e3279aedd72");
            result.Add("en-US", "27c62fbbd3a9d9f9da1910e4e50b2e45689cd1d2824d9de15720f9439c466c13f2ab590849fd008a7450f6166e2370ab97798c91a8eb27008bdc64d58e3d14cf");
            result.Add("en-ZA", "10179a3ba30539adcf46ce78393f9a018651b2991295af99baa511ce9d98b791f50abb3adf8f105f9f84287e0b1f495d8723f7da278c084e426aa670feebab44");
            result.Add("eo", "f15b9b8b8f3e49a2ffc977fd522365fbfe35f0fd30c57a1c63869a1528b1e104292008d00019c9ee271bd41ff027f5b5cfdc4aa9045ecae3d1439092b7d6d8c1");
            result.Add("es-AR", "a8259a4a92cb7d984a1118e750d6c641e58ba34c5613b482c26a4c04b1f04e8fd36a7e0af113d9dbd0a962aa45495bad8a1534e70a3fc93c46dc50ecbcd67c02");
            result.Add("es-CL", "fae04c5426889bb9db44418e8fde2862a1491c4297cbc5aaf73f5352482adf690ddf2a58a2ad7cd6a2b99d9bd0e84f08744ef03d1170f66afe011356e85650b2");
            result.Add("es-ES", "254b1e4147d76b8bd0385695a5361ac33ba0df2f7e6b7064c428499c7d771ed197c3b1b72b5919f455124842c13f3bad7a5094b820070ebd14b6c5b804987712");
            result.Add("es-MX", "28d36a7695f2b56fb09d985f9c1e4d640487b4d3593ee58abc16fc97dd9b3a66b7553d3975bfc221ca273c072c0e97cb5e99d028561b8942c9b58fe9e45459d1");
            result.Add("et", "2aa084cd5df5188c1f08d56854efe0facbfb4e7cf669c054e707405ad48ab81ec623cfb503f13e07c0f91ff0cea3f4843ceabf718bc16bb53ffe93640871173c");
            result.Add("eu", "db84b870c7ebe8ce9be542002fbe0b081296b09bbad4350c8842387f007169003bfac42c2dc4baf66f47c7e2e9aadd89b49b2f53fd44ff8aa97f68c3dcc5d46e");
            result.Add("fa", "916d704e7e4d8384a4c2bb7234a8b0832561b6dc473d1f5a52e19c3ce7f641494820a5af33e7e20112187ed625e1d808e3ec18ef0337dedb4d8f1779637175a1");
            result.Add("ff", "aab9e20f09c0b747d0577154dcf1e2e795154cf1e976df3c5ab237454f6629e803183e9c41f6f396a1027689820a6edd4d7de9a31e8e1fbfb85c2d0249cfe820");
            result.Add("fi", "30b2fbea4bf3ae06fb22ae690107c7005be153b720b6a80703d201af0c7dec6c370301fd3e62d707769ba68b998ffcaf42bb257692ac690a30f1ce52a6ea2a95");
            result.Add("fr", "a453881dcafecaf70cd9c6141bf748e8f896ae621137b5ca2d464f130341fad35a93744820b5224b68770052d106947d1468b7501b4a0bc9a111c3569954376d");
            result.Add("fy-NL", "a8c7628d29083a2076ed26c7e65e5fd631b7f2b2f652cb71abde39bc8ef73dc3cffd7dee60197911b7ceb17819e622e4c129f5cfd663edf82836c3496a89e3bc");
            result.Add("ga-IE", "9e31310306381a3185e89d75b7ab7a79a0c8857250ec0b7829c3821f847e191cafe96452de7b6a73c7fdfc894fcdb022d505408061262608da7ea2474dab757d");
            result.Add("gd", "c414cfd2b1bd9544d90726cbe44cea60d495a03525700c6b1e9a25aeb4dddada799fddec9a2d91779b0b2d9fe84d55aceea25ecd5cc4cbe8a295265443e6cfee");
            result.Add("gl", "ae0f74858d1258503dda83f082040aa3b51c16c49808aec7ebeab797e3933261161c9b679b587402cec0753d68866c771ad00ba61fbe967ef0811c9dc8aef055");
            result.Add("gn", "c5d6ccf30ebd337d06b64b4a244a16e356d69cc2655206f7dff41ee6e0cc125279dd49dc668884d0afcb60d6fef86acc81afe4b01afb43eaa2ac66a9cb478c1b");
            result.Add("gu-IN", "da4c5bdd89c56278456ce8caa53351c8a91ada8e9ff00b9dbd593efa36627f8953993255a3e64fe4acfcfbddf1f535a01891c274120820e5e0afc45e080b5711");
            result.Add("he", "8ec07786812d3d885ddc293f9eb3273b6d219c4a0c4ede81017a71ba855b8031f22a0ea8ab83fff8771157b0e07c1b59c12a2501a2583522f9081a0bac3e6012");
            result.Add("hi-IN", "5cfae37540ac92efbfcfbcbbf17f22bec96e434daf5e43e05896552db4d33d920a4857703d9cb50b9ecac5693cc2709db31c2f3fffcbfb32674067dde8996ae4");
            result.Add("hr", "b61c0b31100b1f5d8857b82b7dab78bf239bf932be2ede47cc05177c5e1db2d1fcdb6df854e8b2c207d32dd78276f10eb13130fb3c1eb54e8fc5d5d83efc9dca");
            result.Add("hsb", "5974c4be6139116f9cd01da9588bfe50339740d3056b56da5f1d322608c7a77c3a43fc903b4cf6624e927582ad042f525ab935310c3d95a79d512518bd974d91");
            result.Add("hu", "a6e035b81f85ab94a59bd70881f897b9716f400a7ee247daf885b3fd39e947b056e9a7089633b131a0af577858555481deea0cf89f0181dfd244315e124dc491");
            result.Add("hy-AM", "7ecca009c70333946373a052c75f4b5b8d54b2813d19ffd262cd849b2d672faeec9797ddda72075c155013aa31f9badca1ada00d94bd5e83b9f53736ae8edc17");
            result.Add("id", "f00f93cf247d2b3c85cdb7c4b3435f72d76283c9ca9c937cce58ea336cded322f12123fa77025b9fd492f0b5a24290aa15d2491717b9674eb89fe4aae74e514e");
            result.Add("is", "2e0c0ec10ceb0a2fa38088fc72c7e2ca525f5a6f49ec172bdd606c639d7f0d6ff288bbc49fe5edbbee0b994a8b547608d649a03c872cf7cf40d47a56c7b37316");
            result.Add("it", "563bbb1aa3f62fa8e4c9d1af4cdb66f9043b81589e833a0f14e9d3bbfea47288810f7d88856b1b765bcf422a1d428734a95949abcd207859e0edc6295bb4dcf9");
            result.Add("ja", "695baeeccdf275a915cdcb402839b39fea675b05074204ec8408fa638e462f33555ef8ad1dcdaeca90a87c7b35ba0d75534fee577c12e81285e539d1fa648c98");
            result.Add("ka", "eeac87a9a12878590c4a42e7636374b5bdb6e8ad5d9eda43b9f833e3bb88a5dec3cc5f1f2fd8fa5dfe5ea31b4bc81fb9cf3855631aeb7c6ead6b950e9255d21e");
            result.Add("kab", "17ed3d26e126fcf7aeebac3c108e8c31ee2ef4e9cc2a19d5643858d0691e74b6dbbbaf3ba2e54c539420df881e8999d0c8f86d84e94b4de5f3f7135ba38f3d53");
            result.Add("kk", "343e499c8f4fbae0929d51bda66a7e84a2b21205b06002f359d614e88fa7a5db0bb844f0d296b34868501bca8a5105b70a6b745fda824159643a9fd1204cce00");
            result.Add("km", "4068771672b77ea62e2864e890aff637b592435e2c55a82336b18aade967abc9505ad67f9f7b050504c07bc40b021d2087e1e6a4e5bce76be7e26ea169b52beb");
            result.Add("kn", "3ba29f9f616dda85d6169a63e5a8e715f17699ad9c8303a1ca201596769449851c3454a0275722a57dbf95cee6fae8614578f6ce51386efeceba561e187dda6c");
            result.Add("ko", "1a42b9e42cd4d3ebcebe05075213441b7803995d137af8a1e6f2d2a0cf5d6080eb587b9ae065027703e8cecc282798a40eefa11303ba2f614dcd7962b5d6cd3b");
            result.Add("lij", "4d2918419c21858a8d514112d3ea8ce11c21822bb0e6f57b5029eb7bfe95484c4e6d3dc0964a6446ba762bee4b9c780f75db38857d185def17a29191b2a04c1f");
            result.Add("lt", "abd9b017ff20babaf935e37cb423132cb5859d008a4792517a5fae6440192fb38d1aea43f97ed51a79354d4f8d86fbe7c7d3fd5324957d93aff252e917c7fdb6");
            result.Add("lv", "cabb9b656c3fa838e99f6f6fa2c8175ce641135b56e46a3b711e5241db60dc276271eb91fbf3a7d27087b4f812e0db8c7a283c044614e67aad8d8a0c238b0ed4");
            result.Add("mai", "80ea2d644924fb90d65f5d96f4f4b8698fdeeeea9e1469c22f93d27156f95616f4eb5f97533a2e88effe2befaab1cae8baeca79d5b476199cd24a190f413b318");
            result.Add("mk", "adee09bd7c903af1047779325f1e24ffe662cdc27249897e7702a5f712e20c253915378706dbfec5e9359166429834c67a9ff46ff1e733d0736a86ac0f69308c");
            result.Add("ml", "6520e6c01aa5a3496dbe99b842ec9f082b5cd179cf7bbca06597e2c43be4231534e9c8b4cbbd23da0b4340f30c243c53c94db5e5bc2091da67ae6c73f0b386e1");
            result.Add("mr", "bc70a3ac2657d197fc1a7d2a826e10887893ad346fb45e6259ff0531c5db864eaff56b87590b69734c98d2653de2336a0e0143acbf15465f67fcc6904945d568");
            result.Add("ms", "29fcfdd31a7cf16a5a2c482170b449e00230e8a4fd9c1a4a515f0a64aadfcddc003eb6f6e611af1fc61236da40c7d3bccb460004d76fe84d1a875dfb930880f2");
            result.Add("nb-NO", "855edc38e80d16b4e533133bf26a919c8fbdaaf8b9793ea62098343794944225440008eb35aea67dea6b1fe86c1b5219018da2f04596b65fa9ec1fb95a0fae85");
            result.Add("nl", "c66e79977be341fe0199ca314fd7908a2ae046fadd3a4389b9aafe265043f7c7351c0e937af6a0d0e36210c2ff1e641ff3d399198550f640580ad05f5c6714cb");
            result.Add("nn-NO", "88fddf258169ebf79f24224ae414db9147dd9334f937716ae861e813ba0923724fd287417208f0634f5c7bdd88e36f9057e610049fc6219973f946de681f0fed");
            result.Add("or", "57a26e35f9dd65230b06637846a6c95e60d422d5a40f28d2f3298d96ecfd8607982265b3b7838bd45955fb39783d789309609b82552ecda78bc2d276d97f2411");
            result.Add("pa-IN", "eecb40869bcd7edc91574e18b19c2f0ef10c5777b47a79c1d66f91bddb75c77f5fdec58300f60bea2902b18b83c0488ac6555929d5f97995ab076ac26c1542aa");
            result.Add("pl", "991616381efed6eb078908de5a2e15acac00001a735674bf33a82ccd97598ea4a5e6a72a3c4359031b51b4a9bd0a7148ce05aa2d4f6ab7fb7b22e92af2d2a839");
            result.Add("pt-BR", "f59c61df0764facf304d00bef6c62ee2c69ebdf893a5db2e5657cedc981e5e01154329c7d8ba8ee0740239f21f13f45faaf51e8659b2a6c752fc4772500bd0fe");
            result.Add("pt-PT", "2c14a36b4320003ebf13ff50c79c35d8ab2dbbf0ea7cf1dfe040c01d6bbe187184aa847cd706483484d792d4c3594a0c31e4931e436fb1c116f7449c54df621c");
            result.Add("rm", "a53ebf750272ccaa3327798c66b0ab816e06b509006f5b28e745cc71e9504745b5ffdf07f8fe974ccc09ebe9226bb4c0499683b282680cec9d5f94a5097a2459");
            result.Add("ro", "ba39bb3ff4112465dc36f466921dccce89bfa71664e873e61501330e261c06dedc26c6a1d84c7b341a27eda072d3834bfcb61615b6c3ae5ae2c9f117dbd83193");
            result.Add("ru", "5bb96ecd4d2e505ee644323955cf1590c63c5d43f8c9bbb01fa548fcbb52ee9ccbaa8b1afdd20c73f08a058db7a8b64546fe2885028efbd5bcdd9eb4080116c8");
            result.Add("si", "5c888bd7b6b8ca8f0ecdb553599c97dd0ae68b2df7c62467c99f4da2be53c51fabefea334d1d22bdeda5ffcba959d38fd157a93d2f83260364b456eba4b8a167");
            result.Add("sk", "a13a02b19b2981fd06ba600ade9f58782ab1ff84f0106e3b43cbacdebf3aeb9d1f452d06b690a1784c60fe177f6b50e73faf32b43fd808e8afbc89cdcdc24f84");
            result.Add("sl", "b84c0c75ad68ca87bd19e3be69fbec877934faf623b96336ecd9ca5328674390e6134559e2fb718ff167df84cee922807d3cb4405932a12bda0bc78cae4e2de8");
            result.Add("son", "11b6cd531d18597ab3f567833b4145518bad84159e2b9e09aa7f82e3346e30ca24097f81038dccd7120c420ae9cc34438ad37cdae7052fcc20c6f0a8a857d532");
            result.Add("sq", "3b3f1dded3299ad4a4377e43ebdb9d9aa2377af805b09471e7850387881a7fff76e31b47457baa559844923fd313c23c44d5038b59ea8bb9803f9dff971c3e4c");
            result.Add("sr", "d1d82074b22afcf6d80858033836478590697a141674b87ae371a89022501ae8b4707b0b17610619eef15ba508ece87d281a03be85391fc1e64354a8da663d49");
            result.Add("sv-SE", "5f5e0f7c4140b858f0528f349c7af5a2ef4e87fd228aaaef5fc436fca7985b57dc5fafc14b2bb2d84e1e36816926a7cb50ef66840465b91c39f27c131947ab5e");
            result.Add("ta", "6d7ddd77c0d39c2fe726c0fd719b096510a3fd537eb9504c44e0040edd1cd21427b4d45379981b47fbcbe8a17e8a7f3c92ecdb031a52118390e7e2b0ece89ce5");
            result.Add("te", "6eb236cea6768f2045d4b7e82b1beed1906a4e6748b82c2450afb33992fde0f140dd10ba61a0c59ae801a96c6c352aad03ccf1e248f5a473c329dfc84ac1f0ff");
            result.Add("th", "e0e13ec38d6d10efab99d7ec60a62b924d410c1307b9e9ecd9b07e42f05e5934589db0c4e544a0713e625285b730211e3609d2570173e78bd3211adfe9545734");
            result.Add("tr", "5815316e3503b64f3d76eadc98fdab8145cd3baa300dbb3932da94f528d85252fff4a813be5332247427babf3ea71f9bc425a76f539eb62c806238b7895b42f9");
            result.Add("uk", "9bd85a2b19440d1e6a770f32421ce1d9ba4426d7e416b7ad3456a5cf2660f01eda5a6e054b136c56c9ff3c4580d563456cf56223dad21c2db36209fb908d631b");
            result.Add("uz", "7426e18f3f0f54dbf38f4c6c4aea8ddf90256dded4d72fe44075f31a5760ba1170703af74e57182b7381c5e8546a7a284c45f04d38d0286cbf215ce7c85cfd79");
            result.Add("vi", "77985b40b201f3e40e01dca002d28219f763bb41c695949faa2df24dceb2c038557a4ad52b80678ea3edd5429a8e9aa64fd8bfc3d3e0fd165c90e3bbf0f9bbd2");
            result.Add("xh", "f521144a5f4a9cc467c254716944ab59dfa07eea9d2f822a4c7661bdae6e7c6e5e80f21ddd3e64a5f854772fbce8dd8d36a74d19a25df8e4a20cc972e71630db");
            result.Add("zh-CN", "be6bea743d99a785c2e4632a4ead9810f94d3b87afcf0c6b90cb893fee2d2fa1abb2ee201aaa126d7330f31359ba838fe3e27e52ef8c0387d4fd2092b7f5093e");
            result.Add("zh-TW", "cd7c409e4004ee57f9becb0850dcbfefecaf7c9906a8c117f212e4f4c11b38ae303c215435deabd878e8fe61b53f89d71f3aa0f34f4f5694d6723cecc720b3f8");

            return result;
        }


        /// <summary>
        /// gets a dictionary with the known checksums for the installers (key: language, value: checksum)
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/52.4.1esr/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "0c53e0ec132e81ff2b1566b68779ad6b66f48b4008de3a562f66bcbe09ae5ac0fd0f95b4852c36b1dc17d76fb46ea97c24b308320e6a7f0f4eb9240ab3aeb496");
            result.Add("af", "0d2b4781c0acaad90148adf8f0b9aa471ea7e070ddcbce4cc006d69964a0774bd0ac094512161ee32d590c4a899d018cbb109d84b8d059ef5983d43bca208b6f");
            result.Add("an", "8f8e671660a9cbbb027c32648c9653cf6ee5f2aa540c985c19dbd9032c7ad21a45ce1cac963ca6b899ee7edd94d1ebcccf53b84e3bd73774d98be2147c90d5d9");
            result.Add("ar", "6898250549b7ea314df967d7fca3a17c40a60b5dfd4d1c4a457bd13fbc846babc4c1730d0fe2d48a7f0dfed35df194a6af288e34c3f955b097b6f472f0284f5a");
            result.Add("as", "937af4e65faaf80ae109cd7bde96d61505925e1d2e01d1d25017ec5f771c5b452a8e730f4580be1407aeb590c2d3ccd6530e0db51838c976be01c8ce406e212f");
            result.Add("ast", "a421187c3c669f89ff84d4b74fb3c0d3237088e9286895c4c781511eb2bf547f48af88f5f7c68c89195032f2f37308b8e62e668f5dc116b8f037cc29d8bdf174");
            result.Add("az", "0b11a16d10e35bcd8e58561bc26606ba26920c8222f7305d67806224f9206803db9ee925d7f39f4f73a0d9165fdbf9c5698f8d15ea87287912a8c22b35a875f6");
            result.Add("bg", "e451aca7a6e2c562b005cbd02a4a0aabbfceb283fb0419a4219a86598ec26b128146b9bbc8d4767e0987189088bd1a74bb7427405abd0a60e681a5ee7116d1a5");
            result.Add("bn-BD", "7f9bfdbab2114fde58ee0a04a849f2f6760caaf7c28a7f0b8d2a0ebc80e3373a325477f473792a35cdd9fdead869f01a0fce4ede6d6a9fdc81e66a5e5d2a24b6");
            result.Add("bn-IN", "e5c7f90453fa9ee6b12349a00a5d664ee45db10ebf65d54625695d09c080c48b18f1b49896905f1012a14571b6da63e6b35ca513e5a2db73658d3bc7ac6919c5");
            result.Add("br", "db597a5fec08a5536380d6d1e6a9e35ed7204587d497c328c5625c3828fa62d604a97887972c70a293490e6b60446807c04dfe9acf37b4ca2eed7bb3d2efebcb");
            result.Add("bs", "b4a80e84a9450f581998c674e4c5dee7f741a5ab247392b6c24bd7a5ee119c91050b161f062ce84f9ae3e8e2156d0571e452e6ebdc3f037d43302ed91470b836");
            result.Add("ca", "72269a62cbadabbab9503b3ccb0d9216e5a554ef303ef5d92e21875b16da342d74f6ca23def836952e36fa3cfde36a872cac0326638ab0fcf78a4be804cd541a");
            result.Add("cak", "5634dc1a37b784a38e795a6950473202b4c8c52b8d65be0aa3fda8aa9182c5774779e07fd40e244184cbe7890af6460c97d317c833ec32cf577db9a02de0d504");
            result.Add("cs", "c05f8c9a6deb3f783dde3b3f8b1549bfb0160a66f6931438dc14c79af5c031b4731d0fac00f1d9873abc5d24afe345c7e71320cbddbc14c7e23521db038b16cb");
            result.Add("cy", "5bd0281c3cb43150ef3cb87572bd4c40cbd22c21876a5029b27df846b82bc5e38cd06452572ea0c407b3df9fb5e8e37c9b987358a92ff4e9e4507e3233aa0aaf");
            result.Add("da", "d20424269eba7268bc78f2907a82304f1bcf367833cc46cececb9c0e814890ccf3a5fdc1400d7fb657684a80565b0728306103b6827137fb7b68df6fae7d35e8");
            result.Add("de", "26663e1e16dae4287cf879d6935efdf7df8a4b31077a634c29ca5fb1535e670a74982fd1240c1af9759b34bda06e7cb1a5194577244142866c36eb9aff234294");
            result.Add("dsb", "ddd86d1a3dc85b30ec815683ecae7b7dbc67761895e97c21d0271b7a3fc4cc3b705213a4ce7fdfe803fb5fce044d8d1a9aead88a7a01c404454bc8764733cc06");
            result.Add("el", "a2a1c398661816a04a8e5cb633d0d9d2fc1132e2bd73a3cd2bd7078d57a0b11db2bec949715307a39c018ab60dfe1599d0886e7f9d45f13de04ed410e571321f");
            result.Add("en-GB", "3fc40c50ad3379f285d6f3fbda2b0e3d20db04cf0f3dbee6e6d0246e3decd7e27357ec3f8df61f26927d9131f728e6e9d76e016457fea24ff0ba11734eeaae64");
            result.Add("en-US", "22e379219fcf79c6b789875c9d904224c7e8bee9264f131c36b651abb4c4edd5aeaf20c8ded967ddcfb2b992b26ad75a82d813b89f783fb5a82068e97ef06269");
            result.Add("en-ZA", "dcc767ecd705bd6ff5c94331bc0c53226bcc87693fcf0ffc7a0e3518d1886b683ba0663d33033d9463a54d27c5fb86cfc55594b45294da59d8b238e813fb31d7");
            result.Add("eo", "f4c8e5d646dcfc225d3a07ff434406d40f3dd19e046c60660dbe121fbe9b4270e8173cec8daa7de3b757a836be57a3c86cf52ade36192e79eecb15c50b249708");
            result.Add("es-AR", "74884985b1ba0d9f303e297c29a0d7b1939daed5065b348f0e68b8f82fb60a64a9b7e8a0268a53c79f2e2af45a2c206385a7bf1687cd79bb4a16424a7f121074");
            result.Add("es-CL", "90d6f1919c922c7ac23e3aef747ea1c2526b9b9b396d93d9dc67508de48ef8efb80ceb4c6db10e2f6cdf69988494a581753707d725a57e54a90cab39d0a4e845");
            result.Add("es-ES", "2111232c4eddbd81ac844892fa6517104af9b880df07e6a87184a568fe3b529d4577eb01febf05d90577eab361f6d7ddae062ae4398419a82fc4b966ca1be36a");
            result.Add("es-MX", "1e83c7702694942355728562ac7e068c08adb76b27ad4af45684282d439b9e86df7c62feb0364132bec476d6565de53937d9236df3c8f1c311680194fb236adf");
            result.Add("et", "be9194f8174fb719fb80c522ad1b0c0a4849e04acd4913fc55990c9fbc4f2d2912ee59e0ddd3b08ade7cf6d840033e7f2714e97ec5966ac67d8fec34c180c2fd");
            result.Add("eu", "896844a0308412682a42785824626ef0e15d49cac260d08224106bbc85dc179b80178cb319ba0735cbc4beee21ca3103dbefa01bf58558ef58044ef69c42136a");
            result.Add("fa", "390ab12c69d3982cabea415ec310f3f37335dd75221f132b7086c4876ca9f578b9a5ec817d4773eb169744d57d1b6559695a046d8b73ab104e186af2e23dcaa4");
            result.Add("ff", "96f4b5af80541d7c3595f19d5bd2e03b3ab4218c55bdf48725f48920650acd5ebc78d721aff7246ea5d9a047cd593e93ec0de9bbacc39f9b1aefb97a0ee5ea10");
            result.Add("fi", "74e8f0dbdb91ad673101a05ecc81ad30d90e3127a17a541223e44aa4396800eb19e9214e271b080934dd42b7860f8f98a2a98b096e4c315c3016ede900f4139a");
            result.Add("fr", "9f6b789f1a5e6e375a8c131f4fedc9af69cccd1238aa477097e2183d99431932ee1de8210a034aa5ec83755a6bc383edf3057f6a64d47890f1f834977136041a");
            result.Add("fy-NL", "74f6ce258cd96a1f21691f3a76e07c9f6eca67172c89af90afa33f99d5a943a3f5c5ee00e8814ae637efab692fdc949f03f4ff8cc6307e6cfdc0dcb7dc07903b");
            result.Add("ga-IE", "957cf1af27ee4ad03c198df4c14984d3234ead2d1c1f2310ca3b2f2b57ffa9034d7c32e340a529facb5333c910cf961baeac6685ad8489ba158c799dc71f84d8");
            result.Add("gd", "e5b5e05c80b41c29f2dc22f450f59e80b5903a791d7893ed70f0825711941f8c7b22483e2623e8e7445b1f42127693d48af3e0d78ff6618c33539df58155d318");
            result.Add("gl", "ca3e48a33642f8d88be4ecf149187edf6a88e5de6afbe5c091f54857eade46660c1182e5f82fd37db59768d96b0e9dfbc2141d515a9298d82d20b0f629a1cffb");
            result.Add("gn", "1eb0f61e3e21d30081e49c0010e7365cab9c50f6bf0f0cb700163d2213a6b093a919dffa98edb20bae4fa2549141d6f8a12c43a9017448a3b743041d76fd2212");
            result.Add("gu-IN", "723b4e2bae31d2c74e1e9c3eb762f6c03eabd337b7ddbae8d7cc3de4a03f9d229f61f834ba5a664a42b91db95d45b662c451908a4fdfa6fa9a22ef2f1958b4a5");
            result.Add("he", "4c86558c15a541218e1cd3f3ae1b4b2a18438dc775cf50ee9e8c2faed8b756ca0d7471fe525a68feb11625934f63ec4f41e48ba3c60e797ebf9c5dd7b93c79b5");
            result.Add("hi-IN", "391eeac81e7cb6271f5582f0bc714c72c93308914f09c2e0be1b45d804e1df59b7045815112d840084a61db1322b19d356e7c6bda5b500b07b151f1653e68056");
            result.Add("hr", "fe5d7ab7df568a728300d3965116e57c358da67be5f98707858cc05bcf5e95da118d8acc19be2ef051fdd9e135fb85a1c784db27269df4c5f170f006efbd8edc");
            result.Add("hsb", "7ebc50655d417c52a7597d0930a660ea254df9ccb3111dd2a45028395c8af510db9ef5d89c19658fd226f43ece4a722a0d3033786098b352b0d51c6516e06d3e");
            result.Add("hu", "8f9dba5656e1921dd6fd8bf9d3cc6f6a89ddd1a683891609dc8b946175a3c530e484468f36c4454e72dfe61fe83058e63ab756cba4a0045efe76b5889d4ffaee");
            result.Add("hy-AM", "073e3175e4214813c5cb9a6435ea2f0a574efe2bf9bb3b3d5c7b8f22a5c4414dec9ec29c5258aa33c2da0d2c614dc47be8ac909935beae533be213e60d68c6ce");
            result.Add("id", "e824a3d6ba77a1f6d2ec4bf9d8a1fcf64752b6164fc8de7cb82cb2030872053f055a868e34c23ffdbf14742fcf3ce39b7ae9de7d6f4ad1496aeb2d857e096da0");
            result.Add("is", "4cf5778915cec2d811acc7a82507b1f443d9fb4bfc9bdcdfcb3a4a32817341e7b21975c99f8aff06084749223ee585b30e7045e24bf8908a29b4b15a3c83b6f5");
            result.Add("it", "f0ccfcbda33630d28a502ab0377dc0e8e8e0926d766a06a85490a939437652d25e29d5821df605306b479724856f6847a70268e77b1366651a3498c29a50e428");
            result.Add("ja", "2d1c4fc0d832e9387cfef13b4149100fb85920f24f315b48306ebed4a0621d901af6ba3c5ac6da683526be3de281d6bfeae22cd8397c4b167da879c34c956304");
            result.Add("ka", "4fe274068cc3d6402055199e30efc036f96966a7fafa7bfa77ba7bd643843cc845ecc49737c0ce08d7f4f06441cf281b93a0118a6b9a0afc9f3bfcc5b0a5f752");
            result.Add("kab", "05ddb8261ee956aa58c1efe07024311c66d488982c36495dcacf8aeae1d41f7f67986166675a673d7a3526d652381e53da5f9441dbb0ebdc4247b952cc0c3aca");
            result.Add("kk", "8a703e9cbae6e0cfcb4fbfbb116d1d27dd3484c115567cb782885957e7a0b7f2e8620e954c35638ee2e960ef25bc17301baee18308a4c07f665ee0674415b502");
            result.Add("km", "bd4cf269e4d765a9343dc2bc14c57ca5a3b48e0c4d788b55ca9e3a38596a1884966469fa759f1bf5ffc8c3e68c63ad004754cb1f229dfd637537d52ee23c0f49");
            result.Add("kn", "7d5370f3b0f0fdc0491f621ead8c905d5ecddadd24eebf7e7ec9517fd98c7088884855ad26bdfcd4feb55349e5278c651d84d65bc0632481f38ee4960fb3bdac");
            result.Add("ko", "1d115986ebfbeb139878ceb3ce2e65e16a322f9bfbb747c24787fa16817c451f84de35bb22e48bdf068f1d2129171df79a4b8c03a08e0f0c4bfdacaceb94b8ca");
            result.Add("lij", "5a4bb5cc42741da95936c560133c74910112f3a45a7fc74de77af101418c7f87aff376f99f795a5437d95ae43fa0746eb37f28794423a01151d73e513285a835");
            result.Add("lt", "42ea224c47fe24ccd9951ef6342f55c1725e12fe17a36afd43e94780d92455c731f8ac2bce4756beaa1f602f2c89cb624232532772d38d6014197d7bc727d4d2");
            result.Add("lv", "087edbb64f03a67b697f3a1ce97da8697d4cf2bd0bac1ca204e866355187f5e7a08ddac27c2c79465b78110382356801b012d1a7d1b953f77c79f437156687fe");
            result.Add("mai", "0cd76e40e899b59228db9d0896424bf8ae4acc5cc7de8c0f5f04b819fd7897fde514d3e75ce8ef71c37e4fa2790984f16ffe0e070c74c97bd91ee6bc4a05f9f8");
            result.Add("mk", "ad60c34ac1aacbedabc6d3974a1845fdd99ff83e3fc7348b820f63d6698d6fdc8b7f7901422362e756ca8749d324c5d06e7ab4740b2ac38429f772775b184bf2");
            result.Add("ml", "9868ae1633913fb080b50b6ef3e849355c30c74595c154cdcd771276232bb7e878d9e193f286442f0fe5538010929ad16a859bf31deec2991eb86bffb5b272b5");
            result.Add("mr", "6a63c6719bc2bca748978cd80ebcd66edcd89df3e8f97e4fd283ec90775a7778cbe6828321ae70cfe3a25933fe63cd448ba055a0c987b721ed15cc2d6ccdf455");
            result.Add("ms", "3914b39f3dbaef66d863bf026387be9061cc0b59b220584d2252785aef8c7cd5c825e451df11c1cc90314350788c378d867bd6cbb46b328a678e7d1c646fa426");
            result.Add("nb-NO", "3ec9d4be4badfdc6ae1975dcde4846006b055099332e3de215b48ab9c8f289da3aee300783b2170d9148667a7fdd66fbb03b2ef55214da1e8193579f5cda9801");
            result.Add("nl", "108765f942e999a4769d30cf90312714e7cc64709d8253a6414601e99efa852b1695f75ed02b5791902a760ce12566a0dae3ae072587d1538edb74e4e3b504e3");
            result.Add("nn-NO", "0fa696c306b309c42cbf6adf34c087d0a2a3e59a0cb28bd2e36edafd6c8f2b8c32f827ffb90e91f6cc203cbf0e0884f1ca34d2a617c2c2259f7fb072bc1acce5");
            result.Add("or", "8aad84eca0bce88f686bde66cbc1cab10aa0628b67bf27d433f2db9c25bb1a3b548e1cb57bfc802f269a8a17f25a48b05c7953d0a293f2367c408422cac94ce3");
            result.Add("pa-IN", "7df7d5f6003c32ad35ce74021a41c841d378ff3384dc3cb3ab9a0ac311fd99c28a625845086bd2bb8f970fc8003dd0ada0a5040ea2979ac76da561043e8cb3c3");
            result.Add("pl", "8cf0858908b88a8fbb02aedc04038dd9a6d00ccc4affdc27ad42620dea354c638f5cbc25c00b9ed0d4d9a1956d4616e30589c74fa99e9d1b2cc668610bc7cd8c");
            result.Add("pt-BR", "82770bef8eb12d2a0403cd34c04af2654458dcc252e9f79c4cd7a3fbd4e4594a2519418ef7fb59d2bf332c6146470abd717bda25eb1e7b6a531ab223441cabbd");
            result.Add("pt-PT", "caed1e388f2bbba0c8eb68f58dbdb8174d5fee98583d717041037a24f68ba5c70ef8835f135098293b7003617a67228f2a2d7e8903a1e571acfebad5832abedb");
            result.Add("rm", "632bb76dbb4bcbfa8dfdf8340b92127227d599d910eb8d3b5aba3923a4b3ddbfb23fae7f6793fb829e789a27523603a1aeeff1e33e32c8fed653ff47a418dcdf");
            result.Add("ro", "38af9741253cc05d3463cf198ab7f8c30169f54571a6414feb1073d99d94edbecef30e72ac3a53ab004eba4c91acdb49740ceb4d43b4d8be5de4e0b63c16dc98");
            result.Add("ru", "a58ae00888ac7a008e34a4df4010271d1d44d87b0fa0300f3d83786a45d28e30f1a5ab798b53d66e27208bab45b2df2ee7e1bf10d28c6b78a09efa16e87c881f");
            result.Add("si", "23b46b15f29a31a8c796efd8d07a5f1da170eea27306435c97f5da2ebd3a2071fdb5a7ec748308c9f85208331245c06ab1c938c1fa771a23c3a87f7c35e809f2");
            result.Add("sk", "26b6a77c443765c597727f1b3e44c412a9b65ed4ed255c3171ac1b3976d9c058866e726e29257171e0aa557ef2dabe87f6887e4222e73c18a4716fb3a15da4f3");
            result.Add("sl", "9878c10f1b1de2375c5e59dbca98f66ee6038799c1816a29f36ae7b05c4112ac75d867b821dac7a6d5c729d66ca96a0b8a52d4eed1487fbf1402ec8625b71333");
            result.Add("son", "0f00bfe737c3510622f972f4653ec84d9d04ede4e6d443fa8c245e0c181f1b6c226eb2c7eea8c14f440c9b82cd978c2bf58b9d4f01754be1052f643eb37cb342");
            result.Add("sq", "c0b04c3f6e041a0ec011c3ff27813a1fcb8fd028d0dc191fca2177c52b96fe823b632e611108bc0df7262c39af9952963d28840b2900a7f519bb6f7d9835b48c");
            result.Add("sr", "2c5197bc04f7c372c1b2c2effb0a0443ea2cb5282d01ad2a2e1abfd08c0e2c6b7c26fcf5b4d46687d7aa8de0690addc653291293d360cb6db6da38a01fe1e8ee");
            result.Add("sv-SE", "0889db7ebce8955c8032638ee868c1a28785f0b79e2fc40763974d91a4fffd5746bb0713cd95b26124a1796b3f401aafe0bb5fbebfccf6b43b83fcc1a4962f17");
            result.Add("ta", "90b04720351bf3eead6dd731bdf17f44d8e6f7d13ba584695dac43daee75ebcd9b7209e180473730d7d11069335b0aa1d4c0b9669ff30b8c5a66d4f89129b077");
            result.Add("te", "6cb6c9e34547886f208de5fdb0d9c77243fe2839af15b67de8ea168f325886e525a02add68018289154e2e711b6d5fb21cce54baa1947c6979f803789df82400");
            result.Add("th", "cf6f39183e958c2f7b620042148abb41846a9c3f81d6408914fd7f0e335268aaa1ca99f62c86875a4a5f7392462125e2aea79f445b86349c966e6a5c1c3efec2");
            result.Add("tr", "5a5eb95fb60e73b822c8332e9ab414f44cb764e282d5178aa2e9c8d0bbfce1a7a6add9a04daccc1866efff9c6ea48c446342488762ac95572020c1beea3e9fb9");
            result.Add("uk", "c48fcf3c40cc30882c1591b5db517a729dadcea97ccdebbd4b1573d6a7123d03753646eeb1943e9eac4bc3cc9fcdd8e4446aa0f6c09f809bc1445e752db33a00");
            result.Add("uz", "8faa23ac4cf6eda515d2ce2510d25c6fe6670b50a998572c686fc27759431c3d7cecbb941edc419fc742a2c1a33a707b84703842a3af7fd0c6cf8fc041678733");
            result.Add("vi", "60af91aa533f90e252eb9785f9fa49648a189652f7869ae54194d47669a99de09683c1cee4f37eb40cbd7364b9de1a0d65bdf2c3317104a54c4b913ac005eb80");
            result.Add("xh", "97f46469f403c3db51042da4da27e8a3a3e23674b1c158f92a1465c63ac9d18596b89a2dac31ec02f189814d8d487f32b87f6eaea284c284bafdaf57932c2c3a");
            result.Add("zh-CN", "813f4a9033e7667388a2058f33bc53d9cdf5681f801cafb9e582c4019577681f8cc23f671cb7c7000841594b60eaf9607d0e49e11b212411b12717a1127ad448");
            result.Add("zh-TW", "e008d594ff02f5d5a941bf4c730f2456799e1e7772245e591275ab7a097c4b6ef13db73d225abcdf9527ab968909be536a09aa056c606d2ba32511daaa2a7671");

            return result;
        }


        /// <summary>
        /// gets an enumerable collection of valid language codes
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// gets the currently known information about the software
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            const string knownVersion = "52.4.1";
            return new AvailableSoftware("Mozilla Firefox ESR (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox [0-9]{2}\\.[0-9](\\.[0-9])? ESR \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox [0-9]{2}\\.[0-9](\\.[0-9])? ESR \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                //32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "esr/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    null,
                    "-ms -ma",
                    "C:\\Program Files\\Mozilla Firefox",
                    "C:\\Program Files (x86)\\Mozilla Firefox"),
                //64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "esr/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    null,
                    "-ms -ma",
                    "C:\\Program Files\\Mozilla Firefox",
                    "C:\\Program Files (x86)\\Mozilla Firefox")
                    );
        }


        /// <summary>
        /// list of IDs to identify the software
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-esr", "firefox-esr-" + languageCode.ToLower() };
        }


        /// <summary>
        /// tries to find the newest version number of Firefox ESR
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-esr-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]{2}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                return matchVersion.Value;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox ESR version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// tries to get the checksums of the newer version
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit an 64 bit (in that order), if successfull.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/firefox/releases/45.7.0esr/SHA512SUMS
             * Common lines look like
             * "a59849ff...6761  win32/en-GB/Firefox Setup 45.7.0esr.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "esr/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Firefox ESR: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } //using
            //look for line with the correct language code and version for 32 bit
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            //look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value.Substring(0, 128), matchChecksum64Bit.Value.Substring(0, 128) };
        }


        /// <summary>
        /// lists names of processes that might block an update, e.g. because
        /// the application cannot be update while it is running
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// whether or not the method searchForNewer() is implemented
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// looks for newer versions of the software than the currently known version
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Debug("Searching for newer version of Firefox ESR (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            //If versions match, we can return the current information.
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
            //replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// language code for the Firefox ESR version
        /// </summary>
        private string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private string checksum64Bit;
    } //class
} //namespace
