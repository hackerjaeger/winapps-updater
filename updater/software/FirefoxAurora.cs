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
        private const string currentVersion = "130.0b3";

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
            // https://ftp.mozilla.org/pub/devedition/releases/130.0b3/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "00e9d48a822dcc84c83f439016fd3b3f74e2ad059e1322b67302f536d82f37cee4ad09ca453cc87632718d97c794d8fd04fea8544878c264cafc0ab0d2f0cbf2" },
                { "af", "e0f142e9f5b1da102b625da3a1324cedf4c54f3bd2c73d72966a8539fc21dcfc081de0287e2f1c40cbb28581ddcadb89fbeabccf45e9c704ad1233d52ba77e76" },
                { "an", "00a3d4e73287b8f6768b2b425d451ade8554377330b4a36439cc29d3c1e2b787c4bff8d4e911e35a0cc14a687b2e81c7ba8aa5492340e6e556e3dc79e2649841" },
                { "ar", "064a7e97e2eb350ff8951e52d76af1e5534bf961c1ab706f82d4289dec1cf241341b302397480b277591c09cff4a20e4bd324c7e8a274cb39e6195cf7b20534a" },
                { "ast", "b00522285cab0c4b82e53b06e5d3da155ce37c258782d4f7cb256e90089a398f10c3b5e13f30f5859454a35997f4a1398abd5de87e5a328bbfba695f5503dfad" },
                { "az", "fc9f047cccfdbeb4636cf8c10c12be54a2ca2474364b0e4f71bf622b43326c2575eb9b69b341efee3c4964be196b506b6d2d191a70baeafae64665efed00d418" },
                { "be", "9d4805cb37557d687e4348d4464cb861d9f2f7988a1c8033d11af7fe4909b20bcaf68db9395a7faab7249447f9d2c5ef0d00fdd7d7b47ac7526baa1355c45546" },
                { "bg", "6b9ba3601f930c4ee838b05c6834e5d8f8945834fc556663c039727cfe79a2cb278253ece07f4014ff5eab561df21ad881d06a8ba39d7ba33b0389a1228afcf4" },
                { "bn", "649e2db5c34fec664a8c3c39b55cae71c2ea8915a21933e48231e9a4632a569428fd4853c6756e6dc02bc1948433a6e4852c373e2cc0fc8a03cb14b2cee4b466" },
                { "br", "2f9b39d932b804ac6d96b9129bb948b1ca65ca739df4eb6bf5188ca1e37073e35e9d947f243d8d6b21f5917eb80bd5886eb4dc2fb6c3674ed13c66b38c40c5ab" },
                { "bs", "ee7d899b6d0556b3dc1226dc630548005442726977e563a778efd390b584f3a2fbf29a377fd7fe19b2189fb627c3583b4f7ea9909897d1657e18ef058f7668b8" },
                { "ca", "fd2090dca52245caeae9776054ba7cce952f5aec829de2eb0e2651d27e030e10e56eacb2de456eb922a33181580e8e19b0a9d4d866ed6e6c523e2009fffa98db" },
                { "cak", "3e478331fd15ba529dc5934f18859c86603f39331d81dd627322706d6f1f5ad659509953462bee2053848a583ac216dcbcf9da9af17e408cb3ccc893ccc094a6" },
                { "cs", "f591b0c14224c929c2ec418ba15adb2d209eeca2985cd4adcdc3233c8fda83fefb78ba65d4b92dc423a6ffc7c05b9d9fcb4191e72e303b54dfdc7449333be285" },
                { "cy", "d8ba49b5c4fee95467a2fcf611014a60483c7bd54b36c36474307f2a51a7fca7049c12834a7d4261424b9e3396bc33b24147ceae080f76440db825311560fc5b" },
                { "da", "3d3ae8bd9a2cdfd7ff8de427102536cd13955f092b942b83a57bfb38c0c23f263d296da27fa0aefc38ed61acdb3c16e6e70ed8ee68801e867debaecd4aad4778" },
                { "de", "3bd4cf49eeb0cfe42306202471c0992a35c4d73e02f556176d2fcc2e07b9bf8a2992625b45693bb14a316067b3eb0778b011e17084c6974092a34a323b9b4d71" },
                { "dsb", "c1106f8d13f22b5dd342659ef99fb1e0a09a19d090a8cf492d98b6fb080f1d8d16bc08f12f8d8e2d816aeed60d3f8642770ec164540fbeb389ccabae1ee8590d" },
                { "el", "350de0c87c75053c4337600038e6d3fc83f2c0c628c1fe855fd211a2c3009264cf24e29dd22e14295a670338e4159df14a954f3d15817e5d83cf3f978d57afe2" },
                { "en-CA", "2a75a1ff85a7db0052572d76e8bde91b04a9436a7c6996bded88178fd173a9cd702edc158569fd3ebcfcc88425b3d25ddc045fbb7cc364ae0256f91f80c44b9d" },
                { "en-GB", "7007aeba9aa835b2a5983e5bc189316e53cc7c25f4dfece4f1e08e7ff700a02564eba5dab3cd49cdef38f9a96bcab28da2fe4dc1f7cdfd50d87e509298bfa6d0" },
                { "en-US", "a9ba93aef5e14ec04ee45ae486c8a903adafc793fcf1ace80e694ec720adf8fb40c42ad1924943de45912298ccc0ad2755a7cb1bb2ba99e07e0c466682670cf9" },
                { "eo", "e004a70e85e260e33379ef9320956918d04b369f9999c538a5b16c12de136086ad9541bb32ba898d897b70c7d0dbab5995d2c66a58ab766db38a2d14d0e921a2" },
                { "es-AR", "4f3255aa7fd36f3b6f1e48a95abb7cd36dccc87d71f3b04926eb29d299c974413b7d958f715a8e94edf6b8dc06393376d6b7381591c55e7a3b3320b9b6d508cd" },
                { "es-CL", "403ed67d565877d4f9824f1baec4a067ce6a8eeb5c55748fdad9a9a28cf92e6ea5f01c525d2e8bd18b696eb1659f93a96cb7ae5e84dfde71a1088f295dfc112d" },
                { "es-ES", "71366afaf186b24f1ce452a2a670222b38ea02689e49a0b9a0bff9bb59f8bc99df032389e8a0341b4d72c2be073b2bfa125eb2d77e82641da004d2b6b14b15c0" },
                { "es-MX", "5289c6260ddb58fe369b02f08f14279498e73d2a269acbf0e900f340c90640c26f3ee1ae31a9d146eebd811252cfcc8d06fada5c3a1db166db03d0cca2ca2220" },
                { "et", "3553395563f9c0c054d3a54b0b3fbfed62b3e3c223b81d3a63dba3592b8f5d4fd689a4077cf8856d6156c150671819e754e2883e235a05dee83906b54732700b" },
                { "eu", "68998429fdb030391f3420f24761e4cee4a3bea3594f3a79a5c4a860c7c1941cb406c6af0a4ceaa2c08f6bc2bd67a269f827194e7c89a3bcb2fec500a56b9db8" },
                { "fa", "688e68ad56e507c3321477be7590b44140cc46196084da437b474b5ad5cb01ee1de6e7dbbc36f5317f4848e2bf50a44d95e1c25cc38c6803c63dc94d85858ebc" },
                { "ff", "da397bd41a00ee09d0e3c07a4e9fe9e740363d3dc2c93a69be6754aeee386c6650e5ba6949025792fea9980af3fe8d024fe192994d1982e0a90a1e5eda294402" },
                { "fi", "664c78188ad3d71052de07d0ee56902816deb72b1f58f8bc93f9689e173038765d7080d178b1cd6bf0b4a48024728e8060c30bf41262574bb05aa8b4f554a2a1" },
                { "fr", "dc957e121a7a198a106d1a783b370fb59cf14339a97b84cf783049b2a240903f2ce9216c58e532b940c5831014c472134f3c140d79de4b3affc534c36233eb3e" },
                { "fur", "a3392a8f410f61e1f4adae0f9473d5beeff63eeca1b5f550625e3a3467794ec36bd7543654d010e225a5b0a68a9ded56cf87c4e0d4f00ae737963c020357f86e" },
                { "fy-NL", "3abdcc06f0dcfecaa07bba958f1fb17e3976497b59e5a40d05a7cbee7b67673a12822167e6138c308a0a30566584e0bcc1b6d5dbc32e4578610e9e656782b076" },
                { "ga-IE", "b7f9408209e65fb64d8622ea3553e8a81a526724bc689f442555df48c980fb374c157d043dffd9e66c6ef5945845708325a1eaee3ef3ba7b64c58c1ed962e5ee" },
                { "gd", "36398fef8d7e03465ad4fcde5733c46373427a888f7b3d4a2cd81f5429f653d088880b86bcb77a75c4aac745c59fe2aea07c11721826d3fbebea42583700e118" },
                { "gl", "fbdff81b3d007259066b38b3f4f7495cefcb680a9489deaaa86e1b0388e0e2c3b817d094cff2037d8076bc7833721b4eab649e80d4c9db1800959ae0503ef955" },
                { "gn", "6f8d21889d4247e17ac0958b280bbfcb55065e7db8725a15885f1de6f5ce956a8e243d02de7173dd20781e7a0f32d90023b8de08e51b105092bfdd0f27dc0ac0" },
                { "gu-IN", "0f965b074777ab24dc798738bcdedcad00d85e6d5206b81ec2ab0006104a39b2754fb7d31bdd3557c5a5b594feecf919c19ab8fdf8a27132fe75596ec7c4937b" },
                { "he", "9ee901e763e826afca51678194b32f552c4357e0b8bd58f263a8dbce4bbf8ea02613f941edc4493f33203f619e63bc79bfcf848918bfe8f58e7737e4c530e682" },
                { "hi-IN", "f775939a4ab6480f63249378533a84d9bc4e66df0989cdb63e6b794a029f760f66c87d9d0d2ae5b4d9da141799cd42b755575c5102ba3483bf10afa841ee30f4" },
                { "hr", "219e0e2383c8931d7c334d9dbfd206ccce860d8d2a2645f7154514dea84c59c750e1603f9597c3029727d60e77ddef8fa1e4bbc6db74c392eba087e0df98f0f1" },
                { "hsb", "9a28f99f9433a7d17ba61fdb3c0dca6fc20a8491bb4789e5d0e10014bacea92f3dd406011f1ae66f84ca28faa26be5e828fa1956495518a9bd16aaca16f0cf93" },
                { "hu", "e8c142fbb02b9583eb77236b0ff3940023b9a9ad6ca51f041212bec2cb3c45f5efaf7d678aca2b2e8f7c2522e6306055a70aa2f711f094ddd12e8dc96d127ac3" },
                { "hy-AM", "e340c64c958d4ce3a5bfcec547300b871bcac800165e7eab102ec804ab81746bfbabde2a32d6b2adbc2fcde49bee08a2a5982d72228dcf980ac093675820f738" },
                { "ia", "9768b3c043654b48be55f52cecd74200e83c558b3a3453ae286f3c35efc639db421c8e5008c622a12a5a6ee7b281138492c407f8d2dab882fe6906c54b76bcce" },
                { "id", "da1a1d0178fc8922cdd07342e3a2298c9a6e0f7e7704e27886064cb79f0987581a3d6da1aea1c404513f6de956513cc82d91a2191f69cf27757ebfc754751bdd" },
                { "is", "ce8c4faab01bfabbcbdd0513f51dcf4d3fd8275789afb6fa52c909275b9c7a0371da9ec1d8aa7bd1b1b9f440d963b9b1c56c6a4f546fbbbc911ec9cdd4cace6d" },
                { "it", "c00a9a41df0b342131cbfdd87cedeea5a0c5be4dbae5fd6de205b2f6d12d82cb5a2f45cf75ffa898ccc4eb72785704360daf3d0be4f7fb5287d2c4531fe46674" },
                { "ja", "978acb3ec1c3d6b54170318f626e8ca5e9ab2d124c7d08f1a6e83818c20912c16223bb58da8668fcf34e083a15bbf210972b657c8dde65a139b1cc73dc8f5ddb" },
                { "ka", "886b64b74b42f55b6b087bfbbfc7f599c86bcd6ce62aaaf291ff7cd4f580b691f1fd9f2772ad06f6654f47b7e0bcc25cc8f24d32ecde12c999b171574fee5e63" },
                { "kab", "ecdf8090d6ba54ae9b8242dced16ab345ab1b1b3270e3eee735ddbd804e692b5dcd4d840ce20397789aedf6b92ed230a3a45a1540b4080f63a8c76dfd7720fd8" },
                { "kk", "d6c9f1899b5f7fbcfab85cae017bbaa4ae3a710e6a618cc70d569f7d74fd12930365c4db07c281b4898668c7de1a8d7e7c56c9981b8c79092ce09d72f132a579" },
                { "km", "46986cec81eaab3ece38851a593fccb2c27713e9405daa6eacf3960cdd6d1397363c72665a9c3972c0d92d5098a03a34a9e694bcace0dd53ceab88f6f70fe26c" },
                { "kn", "00fe476dfd6f289c025dc69f408d97f43ae7291fefeb76941cf3341a3759917fdbf53d01528900d45f4c44c12bcdaedf263f11749d295f7101e1bdf03e377c42" },
                { "ko", "f9a81652669941fa372c6e7ed48d4aa48729b508eda88324f6d1233975a28555c75aef6cbc0b737f77a2d3f6d8fce5b4bdedd3360ca9677efda41cb78c92f255" },
                { "lij", "47513c06de14afcb37b8c3aabf085532b9fe8d2d74ef2db39aa0071009015173b314da73fc9cfa831bda22b12138815e548fd392bbe155d0ceaeac0210810e7a" },
                { "lt", "143674c51c6480105f68b0643d8257f029c5755e03c317d449e99292c5c39de5ee0dff1cbb6a123a02b3b5376d1e8d1e368b69e49af5828213597bd27ac04566" },
                { "lv", "bc48166446610bdcb9d159800138b1c137cc032523252398fd0dec8747ffa81ff92f6f8a8c1ce4fb4699ad4608816b427cd1045d0905aada23bd17563c5fce57" },
                { "mk", "7f32a15126debaff6a5e874a180ee364918a96a7250efb08e3a755a8b7298f272c1767429c4c30bfb34b892a061f1b6d970eb4e24f28c3f43ada3cd0399883ea" },
                { "mr", "2b4ea0af99b261f8fca335d894c0f8b0fd1125d5aa4ef4ebcb7469f3a1976c3148bf0fdb592af568b09f125185222b02ad6500fda932ba7b48591b583d5add78" },
                { "ms", "2f975eef996a7974811c454b2dac61dd57a11fe2e4f67fb0551969e293398bc16ae4091cf32371c76bb5ff774ad786ebed95845bcb1a0d9169ba75fe8650a54a" },
                { "my", "97710bc5ebccf443f23b2f6b586011c722b32d113a99efa5bec7da05e9868f3822ab1cf52ddcb659c51b8919a65c51102ac67e01032cea1ba75d4d07604feef3" },
                { "nb-NO", "76a5a0e1fd7a32b9b57f042d33d82b598b4e6e6519957914f5c8854e3aea41b6c1c2929c86e87225d4d666381aed8b234e110c828a942f73cdbbf76573b054a6" },
                { "ne-NP", "122f328c0044afdce98706b4c285627db8c02aeea6349dcab9dc16bbf5d152e47fcdef00967ef535a2881b335a42d57e6e27671ed5653a190c5c007b95de206e" },
                { "nl", "1627ffc8847cbf3debf43ebd05d822f313db67dba6a0978d0b587008238527c693f1e0b9bb8e110361ad0788c4f98a426ad050198398c48d079f55f5fd9ffa7e" },
                { "nn-NO", "db7c0c7ef8579e420b546f9d999e469a0852800313ee95f145385cd2b6a84e49ccf27973024b8f0d2e5f99006715dafca4f78459ebd2e97df3ea7d6998e6e06b" },
                { "oc", "3456a4be2e15c16d17b962b50892b28ee87819c1509a5927eea1b5e1455a10274de6f589e198b0d7904a5840811c3b7bc9402e2aac203c257012fa5fa5f2ea47" },
                { "pa-IN", "6db9bcfd0c3495d390cf1d8eba5a4c0975125c38d51beeb274048c9efe00a3adb99baba3a776c555f06ed1c62f57dca250bc30faa667195c1678c404088edbf7" },
                { "pl", "d3436e9fc9c2bdc964d3c737b9b53ebf8ce471e614cf0f1ec87ea71ba3c2b8105536cb3d93b0623cbe3fe9a498cd088896b270fcd1f5da0a0b247ba3c01f0dab" },
                { "pt-BR", "af8b3a9cb1c4e56b0035d117e3dd0387932a7b916f63825edcca0a2340553eeea28a596e41ca9136f191f9bbc34fb3048dbd8c7b9f77d52645324e0e0952efcf" },
                { "pt-PT", "d9e93b001fa2f4d7c6587baec5d42af8713f7cb388c11d60a46eb28f1bffa37a40ad8126232a6380ed1d9226d546f09637b57d8aa14005662503d490fe0d5972" },
                { "rm", "d29882bf20a69f7042a93757cd7e3386e9d3719f393351402dac5629dba7f235046e2a842ead1818e674163e4e7bbf226362c0762584705e339f0cc2222f86d8" },
                { "ro", "b7312a7a2b9c4f33cc2c0857dd0acd0793aa7368b542763b138834e5438dd4f5262c5b2b7f337cf151e387d7da9675602694a1a1a411f5288b7d594161ee1a68" },
                { "ru", "e3624e9b0db15faed1a3b3b0fd4121eaf8800bdd826258e6bd4790568c3fbfb5302b0f44a4499db24d07e9cb4fd42db645cc3808068551f94e1a4b9ff28a2645" },
                { "sat", "40ef50662d2bd7a3ed6844fdbbf9b6107f3d43ecbc75e33ab3224ebe6afef5f1f991a609ae2ded4926934186a8eb2763add6eff2de6faf78bf9bb75f2032602b" },
                { "sc", "f74fc1b2c7af51007b4fcfabd52c3f9a74ef14e71f0f0245bf07789456b8f0154416c57e7efa349c6d3293ec824ebf5718250789c76578a802ef03a3cd625c94" },
                { "sco", "ca68e83d4847c085969d5a8470fa6fae4559c159d2a87cadd5f09168a008c68b38662d3dd632709b9d13421355de7e6978b25fd690f59531aee0ebdfdb368d38" },
                { "si", "76406ef262f88891b18487d01b13c38e58008f28c6e98d0bfe4435546b3f1892cb095b2e633c0afe14f17611a387aef536527383d7c77d08d33e1006c44adbbd" },
                { "sk", "6d58bf1707aaf117f5087c6239fc53d2e48528c5f4e854018ac9801554fa57df3f9f766a93d59a43c389c51133f09dd372ce3a7baa3e98947f55e71f3287e2b1" },
                { "skr", "b6e0c649d45a193d7a7d9c4090249596e959b3eb73b62890b70022a34dc8c488e28490aa53708951453f1c7b34d983513a288612f7fd6b707317a7cfeca402b3" },
                { "sl", "ba89ae01a63e247f0d6d6ec569f0c2fd0a8478d3d816bd19920bf1038f98eed29069e434cb4ed660ba6094d3487fa5daa2e0ff732ec0e388fb5f8fc3398dd0bd" },
                { "son", "9276cb8f032baf0296dbe28ad27b685e67921e90df54cdce26152f109a72f59f3c2745b597152c9f20a2c02fa269179e83ba2a8aaeab59db145616055bb939b0" },
                { "sq", "2e7e73a52a31d008319bf429ae5b6b7fbb63bae2b456d709e6cf67d26e7f3fa288f75cf29c512e9fef5bee19a5a65a3af57aeb8236cb803ebbe175a4cabf41ef" },
                { "sr", "45f20bbe603b66f2302dc352bcf86d30081cf0625e47069b946fd3da80a6961b9abb50ef00c7bff889ff8f8c1184b623f51c0923a5eb262cf15f3f3629ea2457" },
                { "sv-SE", "89a163f9141cea4c4fe60d2e8fdc627b3d1562c140799b8a019d3da2f6cb79065373233d3ac3a93075acb01a2277341c61fb00a6fec74563ffdac2b2d4720657" },
                { "szl", "f74b8e37de690f348779ec7436e9967b918a767ff06c9abfee2e1c6a694db59be3c2a506e4bff02566eeb2ed0209cbefbd90be176debfcca31d42238a0d0227e" },
                { "ta", "895dedb94e01fde16b4baab4c30b41bce0c305961bc28172b5c1ab05a784c1c719f4e2186807da20d4b58096a4455145b8a5070624a8275ec23a90a97f7908bc" },
                { "te", "97e89ea8f2e9b72f4f12c08b9a0dfe581021dd0fd32c4341642ec128e74cc4272fcde95ba5e8442a71fdc714111eeb0f6bbe5900bcdb0ebf4ed6b9dd96c17b97" },
                { "tg", "792bfbe36f6d44f167a729fb115d0aad00c7a398622147bd12ef91e31fd122be33e5c0daa2ce7fe68a3359cb02c0a540399f0f31e27328d38c4b8bb6b53c4bfe" },
                { "th", "28ebc6b6156af6ca79050d5e917c7701fce7800ac333f07ec40eaac9486ce7ef2a6421bd6d0d0e0bf981bdcac09718753b368f3795992654a5f934552aeee792" },
                { "tl", "3ffc4ce51b9918a246b025f44178ccd9ef8134f36ce4eb424ccbd4272b2d2b4c00c6865f56b4e1a9aba5c4442351b0d2006a2d3a7bdd9714e9a9b4582bb95472" },
                { "tr", "6779c619d8e738c71d552656257d670a66480a63a452e187d38a205f5c35a9498d49e43775c3ee3a49d2b4588762da3516da5ba00551ee892edfa9f0357ed680" },
                { "trs", "100fdb45e02a3122514de20dc2def5883c33427ccbde834a594bcbf4c95eb9681e1534d593b052aa2ddc87176deb4ffa572ebe0ee3c02775127488d1dbc0a74a" },
                { "uk", "168cb45e598bbecea8393f297265d1cedd23f5e81b9c581f731d8a170b817bab4f609cd625043144f5e625fda3896193082ce4fddb1545cf711b5aa88b9986ed" },
                { "ur", "c4814210db56333643f6b38394175bdeaf2c8727180f40a356d5a6e069e3fc8247444de93bd26ec9fffdd122ee067a10901e117cbfc273a351e78cf6a71c1abd" },
                { "uz", "b1eb690be21256386064bd62821538358e9f5f35fe456154e3b5bd85cabc3d7c1f782cbce8d5e87af7a67307256e011478c867a050bfade83a29d58fbb12e18c" },
                { "vi", "09598a4fa25ad569e6ac594e9cf318ff0378c70253afe722c999c03669c659377f8c43ec7dcfb2c3bcf091b5c76a75efbaba706e99513a63d3570907064dbbe6" },
                { "xh", "689031fe5496be37be4a410ff2daf1d397fc88abe459503dfc7163087d1220c0cc4bc2fa71b4b4da0d109758dfcc4174f73703e65b1b7cc4b3e63e23fc80c9e5" },
                { "zh-CN", "798413eeaac7f5b3163956aef53c6220cd935d667d64ccd2e150d9f80df6a1e4e5b54e8fc76c36e8137981189a9e1ca5a82a3c237fd92285b04053f8402abf1f" },
                { "zh-TW", "d1dc14dc7983e959f37f209f4772dc6944902d2543d86a31ca3ef59940bb41c2406b4722cd9226103daf89cb6a90ab44e8515dee973a7a69d5efd071b9eb1a90" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/130.0b3/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "49f0a0ab778f8bf04519a5aaf9aa37417c2e1756499497932779fde71b0c86de75388a7090a1598302f572ec5602679ee2c5841ca2495ca9d12f0e83877655e5" },
                { "af", "2d3b0777452d1b97ea139a07d9309c8ca0623d287611d20f1aa86f8d5512331dbf6f1b1e32097df60bda0492837de3f54ad1f449b7af26629a23969b8bbf6fb2" },
                { "an", "8a9fed57b558e7b5a77cff65f71c0488690e68fcea134efa286fde80725df86815f3389e5ea195ff5150796556601d328ab15b306c04689ea01d0cca18a482ad" },
                { "ar", "39f4ffc66841996323d166d88b9874ae56a8908deb36edf1b44a0bc05798001c19f12fcde6be29a6e3462047f759cdef55368b428162ff5348a176e3dbe104bc" },
                { "ast", "f93920e869c3c118cc721e86695ee70febdbb9305e8a60da0b86c58e27d550d3321316515902def74cb1d56d282a81ee185e6230b69f6d21ec9a4d23bd0dc978" },
                { "az", "47c6a584ea7b56a5e71853b41579263a8bbcf2329f44a250a58d64728b9fda1b8569174ee2ba87383891cf1e753f9f5b2d9f6ab5df22e77dfb8cff5082782ec2" },
                { "be", "c46afbd02ed727ed9cb3b573d2168f6adbe56d3572480a90fd0ac8fb655b2c64490d35676f31f51aab37c31f260ced6a065200185a06d355dd616b83f74097c6" },
                { "bg", "191f94beb1410dd13948d33e91cd3d16755179b0a02955973990d82c896cd385bc613678d880d7811877d0c5d6875545d6ae4fccd2dd4c68f31f6b3bb68a3753" },
                { "bn", "03c2323857f49fd9c409b5079d559d9002b0d407bed231a5acfe2077c4125529bec5ec47a7bc70d95996547477f02e0e50cd34ca7e5a69de35da144190a8e38c" },
                { "br", "5228768f83b232dedf6f3277ee67a7914818768a4c5015285c276090b18aa0d1ec7eea6e2b32043a1712a7f4f97a8930078a01e4bdbd10c6b8668d8bfdb6a2ec" },
                { "bs", "bf4dcf6b5657dcd1aeb6e317972f3fefe8ed9d5270ab4ee4497b4664619ed85c9814a220763858ba9a6e629b289762378f7caa6f13f643bc5eb20552d634936e" },
                { "ca", "e390efbf968da282981a3ae6a0ea59caa3a1257df28842f304bde378af5fbfe48e5bec718e488bd06583913f5c5eccfb593dee7eb4213b6634b3a73105478ddf" },
                { "cak", "9f2c625d6bb10628d65bf5da72a7bdabcb763bab732408c671b696ef9a7f5cb9597c703eaea49610f858c68536a7f912e91b6fb1b16dacb76c3f173a5fe0c4a9" },
                { "cs", "961236ac572d982977eb935727adf73dea5c73aa00f3311e38cbbddc93cc3f9ffc9d6543563c8ad54fad26f7fd5758245b23169fffba3a48949639d65ac094a0" },
                { "cy", "a50bbf16ce57839a1bad2412ebe9daff6c17426f893d97993ef8b8db55cd7508430c90ff20845e13b7ebc83c67b45fce0d61086bef507f6e4d26fe5eeb15b6df" },
                { "da", "d25b4cb749c529e36d251cb9b62e06b91d75728ccd05888d3600ffef067d3e6bd00aae1a07629faa1aa23ca34a34326813e242be284c938def61025321e68021" },
                { "de", "e1c9169167d8aac553ef78a11205b5c38ce92de0726c7dd164dd399322ac93eaa29ed7f6b2ba926aee034992f5e48d3a35872e9d0484f2f3480177dd337d379a" },
                { "dsb", "1bbe8f5ce2ebcf201c52ccb40e45cde3a061750461cb10e5d1b1c92e473ba8777c15710a5d37448742b208293e1d803cf8c3cbcc5550ef371066b9565eb43d05" },
                { "el", "0eae44987c7b57abe376cde1e6572f080d9a92cd02cccc278a2edf349e247b686fc8758c516976c4368e5586e2af6498d5f35e76555f2b1251b78f9bbcb11a48" },
                { "en-CA", "c2f2ff59deb40731f5a09703d7c9450c7c4185b9660a4a3fd82da7a9d0769342083a0341c1882ff5d6b908974c92886a1c21d32bc1992c9c0e292e3fe89575e9" },
                { "en-GB", "07aa6358c26e9bd1455d4f5ef1af63d61cc48d49b0b98fea979a6bb68459839669739c5bcdc15b89c50752c454666ec8344f27c6058a0e28b19f64b825499690" },
                { "en-US", "ef5f441ecaa1dd495ec9ba1b482d9d301c5a57741deb2708d254c47ab45b7468b0dfbe36cb13056db60bfcb2347b0d4aada663928318326a7b26cc9c9deca7d8" },
                { "eo", "2b32eb3de2742437aef9ad9a618c11b4deeecdc173fdfe54d42b2197dc7189e6b88c995a727ad3870b541cce0820c05c354f5b8718c1dda44eb3c52a0ff86cfe" },
                { "es-AR", "f56e7cb2e2363dc672c9c4516b9bacbf9b64524afbcf69566bcbc97205c5495a0bbea265e0bc0559d50c74d5e6b5e65a89674df5a0a5b19a289b7c5d9b04a6b4" },
                { "es-CL", "7a6b37c08076f04d9384d321b283c9619aa1adaa7aa762d7582a102cca7b92e7baf41799e0c789917aba01ea3820d91559b90bd07d79e7dc0692638edf4a1d51" },
                { "es-ES", "df1b591e631a2ec7d6979a1c4788ddce2c5e06bc0208430b3634ec7ece8762a8f15a2d3ac2dd20464c5deaa410f4d2cb701111737755a419cd921368834d07b3" },
                { "es-MX", "f848aa64a19d1e3203c418eb22b4860290f8aca2d0c8659d24c56b183a0ee4c851bce3457ed1965cacb6879a5fa6492f35434edc24817be77004be838874c8b2" },
                { "et", "003644d8e4bc7a5c60d132697cf1b702f8bd5788c22573ee7a84e8beb129015822da042e40e02608b70faedbfbcde52344ff1978227ae87323301692f949be38" },
                { "eu", "4712514c9fffe5aa9d33286c2b2a0e06093f81458a1493f4af1014a6828bd2d66aa4bfc499d755c4d02e5a6c40c2a5879f9b4040b90577b3f05ee0de74264ef6" },
                { "fa", "cbfc16f7ac5433ad86c9c426534c16cd96a6047702c0d425bfcb0482b32abe8e1a0c0715f73216296f08ebac4463f15e25bf3c949c523fb3d225b8aaedf27b11" },
                { "ff", "efde0c36f8222be03d67c9eaa8973b6beeda16d763269f1adfc8db70fd22fdfe3f3907d42653700ee014bcf868630b801718d1ed7ddb226c290c40e09332cfdb" },
                { "fi", "9a486ecd8e8e490d9e59afe2c19de474ab1b93075612718f13fa8d0a57896a489c327fe855aa4f469d2bdb550085cb67d357e656f7b804eb9e95321caa1ba661" },
                { "fr", "8fb8cee6cceda44e4d2f0ee2b3074c3ff70759b3f370382bf58dfe6bf9f278defad8bba1f5238cf81878322c1e89768b43315d1a31da9ac2eb0755c052d205e1" },
                { "fur", "0452b9eadf5776a609b4e506ffee4f361f09b3d6a128b8127e1a5ff43ee418ca29f03e2069d06d5ee792844ac029b83a3ec9fcfeb3612ccabce9c5904c5f49ee" },
                { "fy-NL", "8983abbe8ea31b601df7d9fa29a3938564948cb84a1978505a2fdc17a36ace2796fdc5047420fb9737f4032bd1cb1ca62569ac2479e9c39d9744498507c24de6" },
                { "ga-IE", "544d9e8c874a00d5efe1bc71e72ded1c93b23c93612317d413e0ba9f8a37bb1192e975dc343925c260c48868269c676dbf252fca38f20dfdb893521a1fd9bb3c" },
                { "gd", "09766db526cbda2d925bda20753333dff29dc0835a29719b97522c4aa7962513946ae5f7901406aeefefd36c9ca5143ed1f12a48f99952693ade2f9f73d7f710" },
                { "gl", "d32a169fd41e04c78f635f49551c267932c6699315d20630669af8b915995253e6deb0664120ecb52db51b30a333bc1e0f9e11670af252a9a2f26e53fd5e0626" },
                { "gn", "92e0cd2212d258d78604add023088c1c3effcaec282c3ea0d0eaa5aabd89e28aba7bd41e713175f0cabf3dba11da4e1e34a987653a12602f91665017e74f998a" },
                { "gu-IN", "0b387fa6b6989a6f0eaf15ca0049a016204bd2d4be0229118af2fcab571947540b0e6559c23fd8110f8d319c5546b2d2c6361b89fec4e2d46cfebc288d90340b" },
                { "he", "6fa8c1864bd719e5d51cf3c486593a18891c249c225de430c0b3401efb1b04847df498fba109efaf20465beccd498b9859800ef99b340d7d380e78dbd1316367" },
                { "hi-IN", "5f8d768bc5f9bd94cb1268bbf5cd4b200e2cfe7b16ba814dbe7a29d6eca8d945218188a7ad026c886c8da3486c2516849abdaf1d5df74767f60a152ea270613f" },
                { "hr", "971e9502a02402bb95253f522a0afff74df35edd23227b8fdc9807396ea80482bcb90e1619484bf9d3b1b5cc69235441b94fcf0a41840ab05326f8a5a59a9158" },
                { "hsb", "e1dbe13cf304b2ba1cfa947ef256739116fd6746bd798c006b5e729d9d445998f21f763384869ef586e66e8613f54d2a6b68e8d3618ce60add28389708c54217" },
                { "hu", "b00b573121d97373b55673b0885c6a76bcf6a81b074c49f684b0f20f014721e42769ce0e25e21b8a109d315dfbf48c429e7a81287728709e2717e14aa455594e" },
                { "hy-AM", "5979223702889e1c9dd2ad89ca45461182267135f1c7d98e744637c14057a5e813cc27f6f9a472fa0d23e39ad2bb4ba2cffaa9c14b06bd17877c28366c9d68f7" },
                { "ia", "bf3b566ed12b0d9472e10927d359d5f05d1670c1e22e373c657a82eb37628783c7bf3372bec5408452e69a9f4267676800e007f520015ff39353aa5bb64b65d6" },
                { "id", "1d295f7dd2714292dae5590e5bc58ac4d7a618c50c612d93d5f4796d708a1c18b051f7df20b74d4459d0abbe547de51ed403f9a8e6e83e3edef61f48ce588d6b" },
                { "is", "5313261f736d7748ce98245cc9fcca54b1c2a2736601b763e60159521b14feeabe733b87bc8724c58d44f2558a577e4177104bb01a4f0cfbea237cc3a1f6164a" },
                { "it", "9867bf07eceb5623c951a3d791bcb1b50e8fdd7ad950fe438c4a0c27bcbc875e5213e02e3e02eebbe8b7006225ff0e42f7d238205b199ad6aa5c28fbf33fa989" },
                { "ja", "bbecb3273644cd9578b307f304b352b73688ced2aab15cfd3dd00358e2459a70e8a36dd8132beb3ae96478dc8ad276d0415008d3a5f691fd5f96a69274e89f62" },
                { "ka", "0fb3888655843bffa0b47bcb61bcfe5833ba3e05daab75effce5d50b09daa64d96aa4d6498fa3cdaad247a0e4a86bbb45fa73be04653e38d14e1b2527632c03e" },
                { "kab", "f2620ba721cc04b436f373a6577e76825e447f9eb96869c3a0c0905ffe81a552d8561f0c6cd0df7a28ef5207c84e80fe23e9e2b3d61a85afb9445beef89bb923" },
                { "kk", "87ea5e11b5093f79b329c608aa455d04b067e8cd21e5bfea62aa03e4cc6e80f74e332d2bf58353439ab8f947fdbd8845638cd9f262d1709801544546fdc92530" },
                { "km", "d0e18bcabf5dab49f4db109b29de32d60d056e925ef7ded896ec4cd77dfe110b70690c4b1cb8c601002fb09074d65476fb96bc0dcf2ed059b731a287b422c3d2" },
                { "kn", "169aba52334dd70e95d97495b5b2e19dbcd3f242716dbfeefc26a057766f6eff5456c50c5a0eec695dc6e52a4b66480d7901dda6413224010623630229c5b15c" },
                { "ko", "43eff7049afc3eb15c841560d88f5963369f331eb81447d4d5a147aab30329d5c70c959c298ed21a8bb32a317e05c81a0fa071e9adc35dc9c3aa6103d0da3780" },
                { "lij", "d3c834919f846f3eed153930a715869610a29b894bae2c225e2de3349f060879e7706de9069a7894f24deab537107953e4c8df6fd4a2780c66b0eb888a06bb86" },
                { "lt", "d721b29839e600de33d5ec94cec354f6d216907d651b899a67ae3482e9e858013897f068a8b42a9fea1a31605ed93d23aab24c5987ceefaa24f783e9af8e567e" },
                { "lv", "e9044457cdcca20845ea85a06b2652188ce03570837a0f7581c4629ca3f91c1a3c1355463941b6188b396dcbbfb13d164be747004c63de1db1a66f04b4520aaa" },
                { "mk", "0fea3bf8de4b74893d6177272588473e3637f72be55cf7a6de10cdf24ee7ad7f3e863321362fb218f8f3c2c88f9a0748ae6ba2c023ba0c1aca66432d0a7ed7c3" },
                { "mr", "993df58a3fbb6473bb6c7607f902986251e8b95cd843e860fc28f044e1be08b7aaf49e32fd829487321b1f13154ddb14597955957eb05545f762d593142c8033" },
                { "ms", "4acd58ae651cf7e2c42afa93ac998208e46c6bae0e2fa8de48a78a7c0d5161b1501858324609a6081013e5ff913f144ca54cd3ef2247c3c57c67391555c3101d" },
                { "my", "cdcad721c16ce3838743a4c796ab3b7457e791649d4c4a43576e8e5051fad1ac18f6cd0c7384b4068fcbc285c4e8e82492a318d034acc0ec7cfb27cc28c177c6" },
                { "nb-NO", "0f9f43f596c2760d93979ec883e174a5424fe6018d2be43a1eb19aee5d33aee38fd28d75c4a15048eda68c1c8c221cc441efaa5cd70705d1837b25151e2a48e6" },
                { "ne-NP", "3cc7dc962492af778d2b974d5585c23e342666653fbfc1c63e751756d731ca87f4e6115e35a831c709afce3cf2a39c0fad986ca250625187e756821807017f73" },
                { "nl", "a5efb988fba117867d1118b2dac367d3ac1a8c7557ce2dc2f297b2bb92d0e0d00f540e27348bccf8b27143820e75f5ac5df5416c418b1dac39073a329fca5dec" },
                { "nn-NO", "8675d7f4179e08ac85e3f67bb2a9b5b2fdfb05db97457c73e59dfdc21579f4ccfa98e672959c4e4bbb77fdc7dc6a6404f270b783eac6845dcd9956ca802a377d" },
                { "oc", "8b3fe5063c2026ff602f7e2a6b5b1293e1ec44b114c7dfec4e7df773d8c8ba029c1e3095a742854d861d35b9fa6a7e134b6ee1a0ddb613c0660f1dd8d327fc48" },
                { "pa-IN", "24d3fd93cd58a5566e28497d528769fdf6b695c1e94c099d9168b98b581542834ab09979f04eb541aa848c3ac3fc3a615e2a43e2f718fb169aff9ba34fbefde8" },
                { "pl", "d44a622f8fed5bc90b22705732d77fb622fe0cac59c73d16ca2c058275cbaa6ebafcd666797f0b5a4ba778553127c0d995e1841c0ad1dabc3af58dfe0600cead" },
                { "pt-BR", "a5920cf8ea47dbb6dd78b25fd174663b65b42d07be9f461f493c1dd85b9532ee7e490aa6443888888bdecc7f9abd2f72fc30858f13f5714ec479fc2350ec1964" },
                { "pt-PT", "1492be47eefb154b5283b7f2c0877a76d108dcf6e8b8181e40dfcf715e7e6b3f5cc73107949f78461bb855a6430ac20b447bcdb591b93a5aac14953d16d74bcd" },
                { "rm", "b6a3617ebf23495eccd5a616d2ca73e9eadb64304b86daa2d29ac1a05250f3c146d423dbda83a4f8380d9c308e21a520484fa392fd5f26aba6d7a71df3c31124" },
                { "ro", "f5f5751ff4301eb9bc001ab67220cecb6065103df8a00bb48353e40d998e5204a3c775adbdca44eb918aac84fd7bb5c60f3102f0feb8c724563c0a1133c54790" },
                { "ru", "6dff8a39aad2600adc03aa3917c493344cbb5b04d3a84e0b78158a018cfd6a6f173ca47680a478f98d8d4765ce8d622137d094d89e96111a7955d146dc925f16" },
                { "sat", "3c29f2554738c7612909a0c72418dc41063e890349889db3fc47031a43847452b92f8c45eba1f436dd1d86acd839ec380a4839dab9a0f345d0f27450249b596b" },
                { "sc", "17869f176fefdc8e80b19aef21ec55c54064ffac5ff9736750e4f39a726ef571ee5226348e3c33e13e0581caa0add7be08cc15af23ee1147dd48af12cd3db7f8" },
                { "sco", "856c943f607e0e6ac9b603bdc4153d687ce35d67b8d25c6d9d7b5b530a45f1e295f56a0726a119324e7e52cc35c848e50fd74f42eab6524d3c5315a7494fedb8" },
                { "si", "aafae8e600836398432edbd1dbffa7d5816fb55680d10a64e242b6a69ae1c8942bd4809982fa1f01d6c0c39a946bcf124bd97c0957e2572f8fb91f4d2a2ecc41" },
                { "sk", "5c23763da3b4a2ff5d410b7e2870faabbb041a8a5c5254d6355b4a43a05803daff577822ccf02c37795eadc9549bbdb5060d4234d7135a51178944bac755e635" },
                { "skr", "cde022970e0581c57e5b064205775463c4a468873001d26b044874b4521d01af2ef2e4ce160071ff7c5a85629d941196a5496cf5638dcfd9a3aadc40aec1ff2b" },
                { "sl", "56353f830f831c4ca28126fc2319be9f3adf2e5f087e661d53a0bff956e7cb4302568b1edbde95d106403509f761fe6db980e51c23b12a1964924fe8e7bc330c" },
                { "son", "bb47351bfa8e66fdc6c79b950e394111cae554d31a84e4556806e47ca3ce8b05d3f347065b84288e652d3ab92e44c8046275e21088d809b513cfdb9a40f35105" },
                { "sq", "3c703de2df6c51a79620e260c23d7ca8bb0dc6c78a7e5a64f0248fe200e36f384ff2655dfc8e1ad378389adec0c0da64fe037b6e50e1c1219e0c9bc980a22334" },
                { "sr", "4eea5c3d89aeb907e7f064404365c622ec0a8bb9669ebfb9ec8f768e9a050f0bdd9ddf8332eaf13ba7ba756689da5a024be17d7389458fd07a97955793bfb4d6" },
                { "sv-SE", "f434cf0327b3910d1d7d78439b07a181118d6715eee59534009bfe80c6d7c4731a1dc4e5ea4e19ff0066649275318e06d5bc7d77c19c40739d17becf67b9038f" },
                { "szl", "514f6c6d0de90bf088848cfe3084fbbfc21814f4eae0b5ca8d713579488636cd59bb797653d825954b6f0f3f768fd4046d55592d1431447850d21048c22859e6" },
                { "ta", "d59ee9966862f0cc9b91ff0d7f679bbb919651f03ac4095f2001107cacf9a3ee74c9608c7300d13f0df8dbf5cce0b86d54dc9fcb63e847b80d4a9d04b06280d4" },
                { "te", "ce80021c5d15e93590f20c3226c0bfe5b2c2cb96058c4c825b0008e4de1635338efa07f9c41bcdd59ce8318bdc8bdec927bcd1dce58aa9e8743db0556bf05a53" },
                { "tg", "4d96af99c8cb3565fc1d14c1e8793be9c83a00d715a06b6a44158790ff7720653830fa9c8e412ad8f1ebbb9b2a57b570895d3a991d193739592dae879d7b025f" },
                { "th", "f75e53c1a29d7a482a30c0baed5f7a65625591d8221fb0ac32ac020f6d0455fed71b5461a917b173df98580e8e100e0286cdcade9a44c2e99de9acf921dbf1b4" },
                { "tl", "1ca3d8ebe7e23e9d5395937bbb05ccd79e4bd6165f2e1b79ff734abc014fe93f7aba24fccf2947a7a4579f67f212fcca395b64a259305887358e8324d5a42009" },
                { "tr", "af30a1671221d4c92196d778191edabb7159e5424cb0800ec2001f89a6aa52ff02dc9151ea2a60e4d7f9f809df569efbfe1cd0f9c3af240902502c712ad9f742" },
                { "trs", "c0d2067dd486e609a9b3bc54f24d2c8a9338d1200c5f406ca9c900de24d00cb972d32794a4157b729880273a492b4cca6e0ff3c30538e614407361d5252e819c" },
                { "uk", "8c7ec836e5afd09056d4e27388381a6d89566d937219e68d5bd820f0cc24aade641db75bc2dae5736ee6da3f1051c2ebf6f42426c5b7428768b577dff3310892" },
                { "ur", "cc1743f160602eeba26df2d6ad9952fe61e22111367bf707a548e76b2e9aec08b5891c9fbeaf1e4d254143fe875006e6a766bc00b9eb6dfb1a251400fe852433" },
                { "uz", "1fc4145098965ccfdf407c2347342985b40aea495e1ac3cf381251e98f9619483236fd77dd25e942f9365e8f9221ba47daf86d0d2a4fa31d729c72a2a18ac204" },
                { "vi", "599c2738cb6bf8fb7011369ef77bd25b9844d2037b5f1bbfcda46a1e45bd8278f0424f3565d54b2819ce9d9e9554587822069ccc30302209d794ae6553dc9ae1" },
                { "xh", "161c38214e7915ae147f891cd1873cff7f056837c73f610c7b3de03e8d8a209797d89ebb8243ed35640654f009083473f557e0ba64a500b1763ae6b490de614e" },
                { "zh-CN", "2b5110c8a756e474876f3712149f5890e39bc13fc89dee850f71aa803abbea3c4e634231eb18a96ec60efda3c45cb9e2dec1b75e0a2ddf7b3c529df65696edf0" },
                { "zh-TW", "f309d8bb7861798c566c37091b1fd7e61813d84636dcbafa8a2aeace562958ff40717894955591027fb3914ac2b35fb1ed220b874eaa3d6cb22c30238cb7900b" }
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
