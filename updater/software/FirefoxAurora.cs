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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "127.0b4";

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
            // https://ftp.mozilla.org/pub/devedition/releases/127.0b4/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "0668839db3e2a583e3c59773f5ae729920fabce0656ea04b7b3562069f06a1c09d8da49e1f781a59cecf36c2b68d708d3cc102e6586274df7d82cf20a09c2359" },
                { "af", "569f5466119e2ba6d4218a8a5afaba3de9430c899822646bfe6b7dfb3ef8e763d222530f457258dbc820757a1699a5d9fe82a494559f4fad12c9099a15c2f270" },
                { "an", "7a0b9dafbd4cd273b34b304a43b743580e913893035a61407c7098cb09422726f52a41a36f86cecf219ec481a05b9362a13959beb4bee01798c589f40c364a06" },
                { "ar", "4cc0b24d4302d355f1deda1103b19e00f64253303f6ef55dc51eade4c5858e5226ed4e0872d66418d8f14f87101ad3b302dd99ae25c6ede2aa82912d56b2f033" },
                { "ast", "0f2b00c73af3e92e050ab3ef7d5760a7488f760598ee6c2605f7594ac5b333794ffe634611677cfef82ad42fc0c4f78246c38b6f60adf9dcbf18d2181b3cc75f" },
                { "az", "22aa4c4054dc879f72d1de5390e1ca879946a5fe84fae9bf50c7b104b731669923497b6ae7567f23782894914b596e560381467730027e74fe0e2a82e0663af0" },
                { "be", "5a13cb8b653962b8d7645a92c0eb99f0891a34ac3ad0c4771bcf9df7a575d5e049b03d1b9f9db8083c0bd8cae78ae873b85663a4ed4035259652ccde3a511fe6" },
                { "bg", "d36887aea7c6517cffa685715263c5095aaf9511148ba2ec612d419d54bd9d165fac4924ef2ed790811a45c81a6bed3666f7c162c17ec854691e6cdb4c691e72" },
                { "bn", "be7434da6601ebc4eb2e6e493759b19c2a0871be78098a48a2802494db93b7f7d008f4dd074f033ab2c0cab88ba66c68f0b5e3901bc923d8fcf7ed9ee8a45cdd" },
                { "br", "926eeef1b659c825e205e097753066b2947d337b4231b7e3db25fe8d8fe00c005fd3a2418e14150380e95d0b416e29af0cebb3d58957eae09d8c335b39e930e3" },
                { "bs", "9262fe6063f1bb3655e846bdecd58eaacfd67d0237f1df3cfc411cf69cdd2f17c4db42a0dd7c7d3e9aeeb702cc08e173c7ce32714a92658a159661e26285c724" },
                { "ca", "915f3b92870283ef08b6aa78613c06242ac4ca7ab28c996003ee42bb4ea43258c7fc61adc261baf0977b16ce41087f42ecdf5142ba017e85a4e6f985dfb3cc3f" },
                { "cak", "e80426e14149ca90330c50eaecbaa273c8dbf434b12de62ace7012fcbbd0c81f917a850beb8a929986d9cfa203ca659e3ac9434feb463177cee321494d8a8b38" },
                { "cs", "6f09841f6d1c4c11e7ffcb4290b734bc4e6e2cea84bb076603b2ff430fd1b98f7fb96ddf7a46b78a2459d15cc6e32c88b626d63387d71a84a87b9e207280d063" },
                { "cy", "3be6b95ad15d29de09d4892565b7ee7413e4b47b40404c9d313c26aaaee25909c8694aac6a1ebdb25fd96ee15649c09415dc7b88ac199bd10e90936ba8698249" },
                { "da", "97a1eccd301de4bf17148e2a4cfa721d865ba8aeef8dd0d9651bf8c0495114d7b8e207d3c3f4834c8be25b4aa56719041811d83cffcd528c3e831155b5e5b118" },
                { "de", "5c5264270c86c7774a36b9b500eb44cc3c785a9ba5d78999db7daa0b18874ef0d78a21e83e2efe51542410112d0bf3e89bd4d0e571a842a9ff325ae0f52bc655" },
                { "dsb", "507c2b9e6044dbd17bd307903fc9949083a4769f161413e2fe2c7b82d8650219788de50964697c0d95c277bc7f2b3a4306b39fbb4dc3ca3b1a6a37a601f01362" },
                { "el", "aba03f2ce354c2f3f0805e3bb100d1af8f657f977ab753153a931e8430ce11ee23e41149a29623f0759fec2e4c5deed9b1d9870f1f12248404fdf6c5575d7a6f" },
                { "en-CA", "2053e8116b70eec903cc866a4c2b07119ddf5975007b3e93e272e6677702693321e88669a829cd540027d283b71447a14e60223e6ebc37fbd79b024d30e6388b" },
                { "en-GB", "95b703166daad73c5eee367d2327b67c0812e0470f5793af2e3811804986fe758163e38f0eaed91118d2b0f8f696e69e2b8bd64e0db776a8c9929da798721c46" },
                { "en-US", "d148bdd9469d1ff57aa91008c22c3134af7bdb9db1258eab9d1c6a31c10f4c9849a39f8530ad318c208e98bb8cbf7f7a0b535de60feea4ffc4af159cb1351366" },
                { "eo", "ba095773458b5f5929853734020b27fc7b7b7acb3687ba60b5800882e7e8b12e28f2c114093e387005408f84b4785cd3bd0b82b62f732e4200840b5af6aec95b" },
                { "es-AR", "931ca75d0e67db67841b890bef24244d6a4d3f4cfe84a088b784cc72143da6bda0b94ae7f0ec009cf3dd191c00b4f5dbb97cc170697202c4fdf99a0db430c9a8" },
                { "es-CL", "c80a0da99fc893877cc8bf94c6dac903af945f557ae25bed5afd1bb5f8cd37a0ecbc8c9775c2d456b6312d104f20b1f0c87a97001ffc752a38d47e30953fd5f8" },
                { "es-ES", "2432e96a5c71156c35ab2df425abd1e112be8226095f5d5b2acc494caf61bb41c17b3f00779e62dc4ddbb3e36a145ba5144b7a85814750e4846eb8d944eb16ca" },
                { "es-MX", "110f7eae6f0548ce0acd37f995edca7cb3cbbc9ad2ef86ac58e0f6fe3ddd75278af0fa0bf51dd951c10242e90106e2243e94d56157d429a1431a320fb0209bac" },
                { "et", "b0bead2713017c9d2078691f6612449fc0e7a0b7cfd6b9e4671c0de9c89342395e65ce7205fd4dca9bc6b710b82dc8dc15e263fd5c77256ccaebac88d93fe6a2" },
                { "eu", "b02e778387f216ea8d8e764b1a528092159dd3165118ab90dc065e842a12abd0f72030ce9df01f7ca9772fd2cedc5032b1a668598642001ea385ba12e0158478" },
                { "fa", "45c65b7c8c4ac055e88bd94c794d7a6e07e60d8bf52d2805dae5e879442e29c542f7b8b69f11b0968415282500bc2e3146afddfbe435ad9f02ef79991bf9d108" },
                { "ff", "1c6295935a0906c86c3550bf012bebc7773a0aeeadb6cd444d694087cdde15c7fa72ffe32cdf65fe25f740d5cdb7ac650aaa1244826fe93808f619a044ffcff2" },
                { "fi", "a5abe257c9e661e226919bd576e97b8355bbad99d28ae035d3e43a978f06337e607234d72691c806bf1fd7d71341611a8f8d6bda465dc2cc9b1a245d7a764263" },
                { "fr", "595898c8504062218d0e4a3d70d7c9fa43da4979b2bb055807d6c36e3c68d0ca2712b47145ab91b10c147777a3699d18b78febb207ef10e6d3893de039319a49" },
                { "fur", "5acc04ab93e42a03cf332e7db294672a718bfb2f4c543de422ed3d45342708d1114bfa68d50b17672d2ea7033ccc130fda56c87a7dcacc34aabbef175fa5fd56" },
                { "fy-NL", "7e9ed25fb459a7043c985172a1cf446495ae2cb99a311280f525de7dfd683a88c459046971a117ab2472224e3349cbe469b7b1be4c2f0f70d97dbe146fa7a85f" },
                { "ga-IE", "be8efaa1334d4e675453faf67dcdd5ab705dc03722002cd6fdb73820b61d4e9eae2039d83c1fe4b4ec8e25f603844c3865490233e5bc9169cb549beeb580b857" },
                { "gd", "43312321f51439fd2c28803ee58aee7e2bc88816d606d7b39c10a013e6c60e02f0c2cc6b5c48de07f359dcf05a1cbaa6f60ccbe05272925775f4924cc46bfbb1" },
                { "gl", "85eb816fe8e78351421715123edfedcd8750d1b8917823574682f320f0ca5234a28cd7c4c46008b24b216e3f91ce3cc2c1b2619bf342e1ad6bd4506692930115" },
                { "gn", "57693d1aec07dd4b4094aac27c42673979a080ab83d16d20e087acba7427dbe2a14d0e16a12df0a3cf9f0999a76c51c93f2383451167346a7d3b549750c871b4" },
                { "gu-IN", "54f0704dea177021132a7b4b1c48ec422bbd09eb1b7dc814c2cfdd4b5684e11709bffade4c06ffcb051cf7a7d9ba6ed658f4d72c4929df9ed899c97dad549a4b" },
                { "he", "31aa1aa9a385677889e08fc9ea33b702ace5ab97eee9e470dd8ec4347a441b32a115066cd33a546e3ebca0ca2a3afdcc277d5e6987e98ae8c636661f8a0d19d7" },
                { "hi-IN", "b1bf83226e2b08690149e5b7651e603f72ff64dd04355b2aef198672c0533ddd1302645544547fd73a627ff2eede1d62f51e6d2f7b782051f32692238c0761e4" },
                { "hr", "2fe3f70ddcc93b02591475d5208b84c403dd1fe9e0b793b0058fb4cf7a539623fb27106190d5ecc60f7683e19d2ca1fda6f1604ebfb92e9ee1890177512e9f93" },
                { "hsb", "f77ed7e9e982efd5be0993b19f160afa710b08a25735c9d8087f352220f4157d617308b8fa43f69a7d29cb5cec890fd3f68f2d5816a067026a870d08918adb76" },
                { "hu", "db07d031009a221c1a9deb948900b30a2f430e81240ed0e8cf4e8b8be7ed03bd138a437156d376704f88d13e4dcad7df37ba5ec7aac993de27ce8d115903d765" },
                { "hy-AM", "421f612c8d34b5a8db5ec7b2d19290e3460ad332af110283a9cfe6cf48f02ab884b1354d3b46e12b8321461f4e396cf0dd0f0afde238d080dcf64217abdc5415" },
                { "ia", "5ef3f89065ab02c2312745d374a6c84cbaafec34da0be5d496f0ef8d4f29e8df56a1f9077c9362357352c19c10a822d3ec23c78bf08631d301fe8107c17c1b09" },
                { "id", "eb17d816fa1f4ebbc6761477d7285802afa0995005dbdf1a28af759421cbecb842267759dc6f12e9e9c7855e5f9daefd0ed8452ebae338ee95a07f819701a9fd" },
                { "is", "f65d1a0940f9c61989e0afb7fe9a4b2c40c5881dd1c75ee66643b4261fc017568d3c3803547b3ef83c200e32c8769838189b108af57664eca280307723aaba6a" },
                { "it", "9c4600f441b1603cbddfdf200d988007e22f7bf245bca62129888b0fcbe13d9b260379ce293e36fa9f67f21ae4acaf6db23553b59bae849d8c4a43d45df3aaf2" },
                { "ja", "3064221922788481ed314d04b616cc2ebd027507f7e46fc4799e4a31fe65591ff273d16dd02a24d0efa95181e09f000ace67502b2083597c601eef74cc6ca5e4" },
                { "ka", "b285e62ef936a106e09f8c867c2a980543f40d5b80f7b15467b6c423fb16b7d938aa14c32372459ac78171fe6a5acfca4c69fbbeaca1974daaf4fa1fbabc4b7a" },
                { "kab", "47a539dd820f2c9826830c9e51e401e5c56c5aab4a9d477603170ba2d0e35cd948420903e2d5bd1faf194384d2145b380bc27f7cc0c10285cc05d30536c62f38" },
                { "kk", "ed9ba5b7ed03b05ca9dd44f6a598c30407ed9acf112c1ff3d7de64e6d5785da41d937d6aaf9e23bfd6aa7d483071b2da1d92f0b446d3efe057381f405197f86e" },
                { "km", "d7ddb69a80901bec7cea973785c15f22b88b29e2d4ded03860ab61d5ac677dbb23cc7e725576468e7868d84c540a5e938a0efac50c49e4eec99b7fe0d5a2fd0e" },
                { "kn", "9af5ebbeaa0223a84209437f9395009a8a5ddd6f4070e2a5d5ff8ca67c1cd200b1b5a96371865f70f83dcd1b7ddde402045875c3aba270e13fdde9c951261efc" },
                { "ko", "9b496eb7daf857abfa258e79efd2f2a23eaa22266434861fbc8f3f17d2661ed48569496f0ed0b61470483cf5ec4acdb8ba0662097ed5d4f826ddb5dbb9445b62" },
                { "lij", "19ccb8e6e30f812b5cce5bf8b2a95c19c92b367dbbdca1ed4882e86b1936d222a49e05504afc32cb27f54bde9cf63a372111f83d633dcb87cce4343e99650614" },
                { "lt", "ecd66a846d0041c724193f05f68715667396e512e4c66815ead5a28ba42df6cbffc4204e5d2cc90c6e5ca83d55f7acedb2968bf10573715f651b363e31b81219" },
                { "lv", "2195e298542aebf0de00c9767c7e76278d7d10d7cc8dee0494a11de7659baa3bbbcdc3b1219b7d69d47eef1ce7f579d48daa1b3e5cccde642d7b9788ed51cf7b" },
                { "mk", "6dc975fbb2e7f9ac3edc4cb90402ab39b2028daafaaa01e0f0a702c1ecaf29e8a00a9a7fd6bdfd5d3506178b7183318d4615d882b883d58ab760aa7d1e892c87" },
                { "mr", "7dd1a24a76d9e627d3885272a89e5eb716839640fd4c3f18545c36298dbc046fa642148c919d4a75221a29e66dec9e08754f28a18ddad117ec04e826885a453f" },
                { "ms", "017777cebae0ca73103613e86107d7c713455061786b146c12f958740402b0214b22e9718a0c04f0968cd0bb22f88f1beee8aebaa901ef3079a1dc554be7929d" },
                { "my", "2feeefd0d96046b87cd1114f614818f52289de8782d0ccaf76341941f26638cd46432e447d46e68c22230ea654c4bdd159d7c0cae215c7ad668b28230f0faba4" },
                { "nb-NO", "412c53a2c3c1cb941a112beb3883a7e9709efb2e13de7fd2072ede2a4856d0c73b8df1072637f712b598c3ddddbd9c0d18e57341ca46ad4518189453e84d3162" },
                { "ne-NP", "168d3a5577fc161274b0f4dd958ed5b60fa7ce69dea9998ef13fa3c6c0fb4edd270be02bd2c2fed9989e3142e8fc96aba4d942cdc637541bdf5e2a60e50663ce" },
                { "nl", "84e92c0972abef18e65889536eeb5eae19c0df30664c64b490789c371045d052839ac9b215f039570db0f1fdf0c94aa6df8bc786e0cae7e0d5cec5b8ea846431" },
                { "nn-NO", "0116f7efa7e2bdf2bb8d6cf141fd0d319ebbc1c408057073fd7024d5b92deb3426fb8e4daf605fd04de780b29ad0a70b326b3faa0c1f60eee94a554ad77facc0" },
                { "oc", "e2f95500d1091b67f40045ec56846c57face38a2f90725419108b56afe2f99fba4e0730fcd48d8320f7ca56d2aab65e34e5782c7983a72870574fea0a1b5aa18" },
                { "pa-IN", "79b89aa0d0977fd0e7c8729f8f0dc7d81b279a70ffcdaacaf0b9957a5be507be3bd051e341dabe99fa7c8c147ff3d6044ab6cbfd05a820a2740f8499287cf3a0" },
                { "pl", "2996b8b2a2460059e6f01dc3c021122ae5cfcb7ccafaedc304a4a43ea2e0fd8d0a6255492cc7d9ccae7268ba0fcc84c35d0065018c636060ab0c20c4cc31491f" },
                { "pt-BR", "5fe8d5a8e86b39517a1b6a7cba3573667a19ce5fa05e46b36d3b5a8370728a8ccd25805e189b0c6905dd15412066b6c7a780411075a90bcca6e0e33521e70a85" },
                { "pt-PT", "256be08785b95aeae00ad3b78418e9c7ac27fbd1518009ad50704d6591f09c4c558d6e1fffc8874f0b77e1e8810d9b6342cace5c473490265def83ee679c44c4" },
                { "rm", "57e5d9c3f89abf49e5e1d8d1f58fec024ee897abe8dd65fd67af700c77518f1023d7fa68a040ed11920d5e7aca8b7e4a4eb23da9b8b5fae4acd3dfe39f8f21e6" },
                { "ro", "ea8301d7636b1a40e43f587272dbc5d3d4d53398a6abab4ce3010a2fa3dceec5db8ad4f9190596749962e7476c82cc6081350337c593c23819d2a6880e66ad2a" },
                { "ru", "c32b98e609c3a797efb22040f9a8848c0d0c4d90531e983e8283d1d93199bfcf5734021f95bc21966398311e35cbc229a32cd64a6c16998dd3035d5528139970" },
                { "sat", "0c575836f74b4e7fdb9e8c7832fade57d864247eeac15b884601fa91bc98cffced0e85f2493b942624ef9e678f773a374d92427f43e08424a96915dff916f678" },
                { "sc", "e7ee16691c5076ae006c9107c7a8bd84277a1ca9dffa0316fcf0d783d6444fde24d564cd27c8ba462b3b3c6b5513000680bf28a593a3707416e678251851fc15" },
                { "sco", "aa3b37192519a55df116627ac5f489bd7a38bb03fb954722ba06bd4d21dcf0abb6980539f09193dd17c590a9ad4096ed283b0dd8245faf3100e056b66bab0222" },
                { "si", "3d651fcf40fd34196bb012c42580fc0c6e783ca833a7db83b66c5d748792b7659ffb8a976843991072f42cccbd613cc30e2c1ea947e15e6a48792d2e08bf5fd2" },
                { "sk", "7f0592d05b802324dedad736b6c9561301f63cf349c2f7fd29e71b974a223dcb07e86ce29999b044ac4fc46d85bdb0117de49126100570137f381b89c270a8cc" },
                { "sl", "af4d76a2f0a1edc60110bd2ac8b811dc3ccdcba2615462aedd63f7998fec88e2d9afc60670732ce536977e067282e9fe55d8b0c925998c7dff73d6927788bd84" },
                { "son", "119ad5b2bdb9bebf07fbf4e23e6826cd6e501829733514385e980dbc2ff5e97ba9045f95f1f7e28bbecc4fd37d6c11e630142396052e87a93327cc80bba95168" },
                { "sq", "69b598e58c67700e98c0eb7a7857d80bd2e0de1f5b994d93ab747520488e0c01416f4f75f78c8debe698e1106a9a314f826f8ffe6b1abe9834f476f0656afbf7" },
                { "sr", "8fdc73457145c668b0141af3787cbb31966123b9e79e50df8064b8eb37cea4e75e30866c8d0a8a72e8202edfdfab02d7406d22a802b4d43e108ec9b8838f3a9c" },
                { "sv-SE", "185a0724734b3fc9bba8e067c34dd53f7f8e2b1fd7210264335a858f143ae915400cee978d066455f5fc74977c3e59c0f167f64fb32b737f020cd1245df30221" },
                { "szl", "9f1d3ba692b40c4d671135b414cd9e746a0beb1e8f9de2a56bf1126b25e6ea0e6ce6d9c4af1c479178b594274edc4ddcb7a38510f7cd491a9cfc6efdd4d2f4d9" },
                { "ta", "09ac8889fede70ab437dcf0563b47a415d8d26e111b0ef275955007d8e3a20c5d6793f630707618c473c2600392350c49a6a89c4b4196a953374b757bf5879bc" },
                { "te", "fe71e55af936736290919f8391af5f5666a7a546160d37a0a194ef9d3eabf13188e34f8b2efb6a3f1b6a2af355851e28664c11388bb475fd3dbe9ab7be7893b1" },
                { "tg", "09e83a76a0f7cf24d9904f18276756f2d3d7c10fb387b3862951b76474d24eb08d95db957962ce0195f14a097e6b2e2516b28e4897effe449fde51e04e0af1a6" },
                { "th", "877592b62f91bea2bd618e8aa012eccaa51e27895606bc8b4a2dc06cf94f2693dfa023fbce91ce14c6822b2a0a3a9dbea4d67a0ab10bdb6362775caac1eb5f77" },
                { "tl", "0b3cb8e23a61e94bda50bf23c942d5f434d3d062c631d9d926153c7070da5333eeb749fdee557d31777dfca5c0954dd9fa828d3dff8ec5312570e6f700f8ee3b" },
                { "tr", "0d9b91225af2e6a40259a50f5a4a6c80b81c68f1929701196f19578fcc0d3d076d4b1368b6834bb9866e8861a586101a956f04163373634e88e7aad88a403224" },
                { "trs", "9cfcb0f0354f51089793f62913625e472b3f4f5d0f14a04e171ad1ba49ec580c2ed41202113ccb6c65acd0eb9c7d75711cf48d928b561a79d8164b53a486f4cb" },
                { "uk", "0fa7ad37d58aabdc8f4231fe6d621f4a12f745003a1efb27a986fd7df18bfa7fae3085b6748830083358cacc5d9c4d390629e81d05c1a3f594b362e83b894bc4" },
                { "ur", "95fc1869fd4f72a84ff3cd9ded088ef1150fc8400353dfd4dcc43f99057ed6c5afa4851dee410ac8a08d9ba497c09a611b073dd236fc6ac27f98d5de56924129" },
                { "uz", "ce9dc9324a62a50d842793090cbc1b11e06453cab22e63cc4e7015917b88166146740ce148cfed146103e8493ef4774eae441a259570c9108be5d941e43914cf" },
                { "vi", "6e323888ab730d36a5f1573a89cbd69cbe64c0f548014632edc1c83f97c7ee6fec2dd54197715e2d9d86225bb2366218aba64faf1bbcce9379ec3c76c3e02125" },
                { "xh", "0b3b0d3292c290493c5695e1869d4cf95d59dbb2ab16106cbd95cc17c2e7a397c666123e9887ee5f345dddceaef3d955612da6340e659ec2920a1e94d4de6fab" },
                { "zh-CN", "5ea769cca651e162b2331ba02e778c2fe5e3db4325d3c84dadaa4ec8d0a6508f88163b9813440b6f6c55a1cf611dfc21c7112d2b2feeadab6e61d62741999c36" },
                { "zh-TW", "d9e8fd053132fbd862918ae704c298c3cc5c719a3008cfc4b6042375f1f2f6601618f0d018c322945ead6754fc1c3c943b2cc10fa82f8b8b8f319fd41026ea91" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/127.0b4/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "17c2535fa4705ff1b083cae6bd126c95b2c7c5481f5678510c6f531d0f14a06cc7bf37947a42f557a262d624b84c2d29f934b5884f23318bf0503d752887ee8f" },
                { "af", "91e118683cb51439c284d828856572ee15b5ae899da491f43cdb44a61032d16f6a64764ca503a00980305e9c8425972fcec6cb4ce57aad2faee630bcaf11156b" },
                { "an", "06882318f120475839f893d8c3cc521629c90793870563cf18f4c33b32dfd47a568573771592fa38fdb3f70e5e706e26e433b09dda70274d9c6e0308384c56b0" },
                { "ar", "2251f1d6dc2267d5c9aad8379bebf8db47f701c2a0704809503e255f712dc38375caf5b662f6c7176d3b4a33fe8075f2baa71db7cf3f5d1afafb07b6ac535025" },
                { "ast", "9c5f7c89c63b13e7b8fca2eba1b29ed7c5838a17f08201a8c26a322a887402e3a0c8387c9a5b2455ca5ed7addc0774d7ed776d0464da74770bf6256f369f7ff0" },
                { "az", "98eab6055a2ad9456362d12c8d3bcb61fe08edf9cb74e7f95362f463c3ae0d65d3e826c7ce7e3bd53993e4428c688199807b890129d4efb3ed2f11b5dc33c9ac" },
                { "be", "9a1d1bb2f18949f80f0ae08e7e2897a415514eb1265831cca5b87fc58a67d83f05b91aa6c081d006e6f6b41edbe7e4d4fa6ee8c4327af00ed9520b6e594080f8" },
                { "bg", "9818c4f4ab3c146593b32f56b04c7e2e9fe0985ae060bf544a8ec98ca442b04c8c6b6f23d34eb0d7383e0db1746341f760d166c35b98502f1e003d367e1d3899" },
                { "bn", "a84a124844b9d9d8adc08f44e3f52f021c7b39466d0c7cf00110f579306d326920c9c2187f4caeb0ebb68058041d667e3ea30c0f0fb2a0aa1268af1e3275c8be" },
                { "br", "015c876b2db68f2079f24602ea686b5697006ed0201da43dbeed0a3fe58c85b8f28741dc7cd82da32196a362b3fc05a80ee9e7b16af6c3d0db568ca61aa7d114" },
                { "bs", "d16fa7c33f6de947c572d24ca0a86156c2dfdff7362a97122e6a17be848d8f49b1b276118ec39b4fbd5dd00339a78ef0c55d9c60b6156a52f1b65bd8f5ddd408" },
                { "ca", "4e589e960722c83d5cc0dda1a2ed53282bf55a0a95ea447bd13afb3f957e86a5b0d625be171849b858355051b1d627b93e6fa62ea0cfbf9d4e2207fc8e71190b" },
                { "cak", "f0712cb9299a587aaec91ff9e1be089cd75f8cdb777632de3d1eedb1275ddc82ad7a4c6a507736db77498fa3e609a0dd08f72b603c67fac284e65e7ec749d4a9" },
                { "cs", "c7afc0f48c14ddac331434dca3492833b2e6bcf5522b787435a007ec74f46f619ccf52aabe2a25266d7be576d49d2813edc23493fecde2652dde4b6710759189" },
                { "cy", "eb8fe1122c1af8d083ee430b977d46d45d2d37cb6883627ffa52edd420e261f6dee10a6468ec8788e70459329a5e379445217fe3d26612c25ff41630330908b2" },
                { "da", "bb550f9996e79cfa94cd0e33bcc5fc4fd1d150da6572826e88f9436ff3fe0df380ef2853828d2a4240e40e582955101efc2d6f880d100eec12ba1ac8f2436c0d" },
                { "de", "edf1a0dc370cbbed49b67e460c59ea2263297b6383bb961ebf84fbbf6a0918f1e343514fb83fa3c0e9de9448b4263e6d5e61f23421efeadfae73c543e6cd430e" },
                { "dsb", "98e96f4eb3a8a876beeb35763c5a4bd54a47858f7f22a4bb1b6f3f0745e7522e2d41ca142ff867868828a0084a2840e0b295f1c862b44450a74da5f84a61a89a" },
                { "el", "a7ad4616b38cc6078cde67f15ec4339b17104d52a1f90e0d7b6313658a061f61757732ccd402614a9f201a81000222f50793350f17c1a87bc742ce8eb060172d" },
                { "en-CA", "cdacc5db75e47cf7112429578081ccd6da83fa97a5f6114fb09120786fffa4d37ef35cf2b28d2300cda378ae6f56fc9672c3d5cc4e3c6f26e854bdff7e84a689" },
                { "en-GB", "c1ef2d0d72e3527e52dc710c27e92db5be27b5040a591a46945a7c5ddedc49f094fda4185445666b54309c621925fcecb69d0712fff6894d6794d3833601a906" },
                { "en-US", "ab648bd582381532bf17304af92f9f7f0b731f4a99b9072d39abffa21df75ed19fabcf1c7c5e4e9db0e69bcf06ba6309abb76e1b04023b21a6cb487fbf44c45f" },
                { "eo", "274567efeb01ad0ba63bd50f455a0d094dd22ea53d938163c3e2eb9404428edac5a80c2033fa0bf06b3023a16758f67d59ec231f7568dff540ac703451fdbd23" },
                { "es-AR", "540e8570142aa211fef57d0a8644b4ec251b0297365b4f1f3581ab0f63fc35ee851bd66795f7b54bd965c5e8407a0f9958c9afe47e643ad0609bb2f226ee255a" },
                { "es-CL", "2c90fd05836b556b7ebaf0b55bdef3206ce47852aa2616a1683428d0e3a5d4bbea46c01eb740b4786119cbadab6b37a0a46bf6410b2af94a94aaecaa3d250c95" },
                { "es-ES", "38492556a89b1074a1f144f4e56ae95e877fe9f2f8282d1fafe61052b9f1a145b6025974c5bbbabc3fb116e55a272aa6f939140ad2c0fd86cb2f849a1ad64391" },
                { "es-MX", "937260aae6f289c2d6105e6cf14040484833782f0b7f322a5ad47f745b025bc1d09e59d1bbed6f39718946d83e52127bed1e375b3c97c597cae4a7840d700f61" },
                { "et", "d8724fb90f42572c73beaba585ad5f36c1e19357f015a706749a882bcddb8b9282495def0dc08f4c55a18dea8fd852800ca0f3969fb5488032c3b7ed13590f1f" },
                { "eu", "f6ac4fb4d6472653c93a7e42ac0a625f73c1977089bf39aadbbade9a3fd120e72c490a7dde6e9ee246fbf801a2c0b8d55eb2018d4b06a46af32e73506037ce52" },
                { "fa", "86086b29e76c343034511ae7d75a2c857cb4bd18b90b9deea5e9ca08e9162c1de0891277c09c6c1998e6c68a38c4593da89d2ae7873799f520b0115f3489f89a" },
                { "ff", "78a7f2398a2a5a987e958b183f99a5e5e9793072a82b77fcfb017516a19e64039b5105fd13857224276858fdb69b36ebe8acb439f4905717ae7332ad5df04593" },
                { "fi", "86ab2a089a26b3922cbd652bacef59a14274f1fda753d7c81788c200e77066f41c2e7110723742ae0168eae13f9e9cef76ba94f74d30da622bbe5fc3bfe7ef31" },
                { "fr", "1faa2a0c8065f58702b0ed0dec2768aaf2ebc0279630450e050d924022b56a30e049a2ef8c3f6fcab6b21b4c95580e2137fcde5b346c2992ea9eb92acf4c2e1b" },
                { "fur", "6d5552e6f941578f2c2719182beb8006e523aa90be62bdc5505443170b3ed9457e74ac88e1a9e2485b1d10c558d912c253c036766ef975d6d545d33a98e948a5" },
                { "fy-NL", "fd59d7c045918ac02aecacef3798f47cff7c0e26b98bb4752af5aee599387c036c6ad8e5b03065a9f9d7717e8d824b7d422df2c42cd6c9758a7c18a7848ebb3b" },
                { "ga-IE", "55cc8ab61857f820071f5bedf9c509f0120fd872b442603bb590b84b17d606767f66b0ae0d08eb264599df30017eeb0e458fae2a7dd99cafed1d1a40a291dba1" },
                { "gd", "9144299e2eaf5e9f06b0e84497f1f2db6f11827efc3d0d6d8e337a6caa9525ff74446735d3b1117fd64634974dabab2be0ac9c91afbef673faa52a611fd5a317" },
                { "gl", "fc54adfb85852eaa3471f0ff7eff8cf05b04770679453447c071cf2cd8600ebbe70cb66f37314af5925161c741f520c85cadfc09d24d08b1949759e996a67224" },
                { "gn", "a874773eb450676641b40869ef554b32234f3bb138fd397733fa7a1223d96c72dbe808a4f04ad9d9a07b472842f7ad569a6b7dd90769f7df5646a9e7d1d65865" },
                { "gu-IN", "d67aa24255811b1e0186110a1e5cb2d4ed479c1fb36a911ceff36a1cf26ff7f68ae9ecd969297e3916acce3a7d829be3279238ed4f0f44fff26b68a6020b3502" },
                { "he", "ff077b7435e61887959781094c47d20502445891a92714c48331c0618872db8a5b6aeaa41b7757eb68747cccfe416726b6885e2b0fbd36049b11b57b6e55a5b0" },
                { "hi-IN", "8ef5b085536e387eb9fb7fff863a86fba0e6f72fe98af55f321921e52adab5610f1f76243e241008dd525603f036ad129500c3494b6e92dc2110045e332056de" },
                { "hr", "29647cba1bff1ab4bf713ce11bc4773bf06d57ebadc3d83fcd1bbd540b02b16514ddbcd1ff4cbe1cefae81dbdb28b6658e6c7a58178d10614dac9263003e9368" },
                { "hsb", "fb7f14a8adcc10a5fd91652614642debaae6d98a7c812c2f6cc6a6c1b0d08c21725bfcddf135578d04eb32dcb3b431cfbad8fce8a6fc48f21d0584d8fb94c84d" },
                { "hu", "62aefa9eb6aedc684f7243d715e122f010b58e28f1edce9b26f30a0cfd22d6f0d198240153cdb97abeb5f0c91cae410e36fe5071d39b8fa56ca4f4807b0f1f29" },
                { "hy-AM", "158cbbc43a6eddb95105b6499eb674095799707464266a1bbac1a14f0c943733c1cdc6696adb9af43634fc9d9744a300cd71f89d5d6c0b4134f19d9ebc13f92e" },
                { "ia", "fd6ba3dc42ca56e5281bbdd607e1bcdbebd930de21ff563ee3653dfc11484ecedc92f5de121bf5aa1b8aa28510ad890b0899be03aa2b2e2ab868291522c49fa4" },
                { "id", "587be5bac66aaa67c0a9213e8aae80fbe9a6223ef0560fdaf42ae3273cc5a70ff884b6be3db4d211170f73353a05833ffcf1cafe716947fc02f8d26b39cb2a8c" },
                { "is", "82f72443741800c6667b98b061aef718df79dabae00abec92268294df2259798a578bbf2ba7c8fb558ab366c9dffaaa7563167944e5c9d3dd8764c121986ba52" },
                { "it", "a0a31eeb809a1aaca14424ac75240d77b97b4916f03e92347732ee09752322d09754bcc04f64c98fd2f1868def24287a81909b190364e4de45137ba03ffb3ba6" },
                { "ja", "b4f9b21c445981f511dae814087fa0f39743eceb6b3643b4020b22ec7e48129690d05b2e39ca171bf84440cbcf7bdfde7bb56bfdfc8b33309f64e4ae76e510ed" },
                { "ka", "c405c06d27b690ed9ca7eb62d49701a41f6fe6ee86d402e7f86ddfd5ebcc5e96cac456c26c21fb76c087a6460a00db5b1b5647e6aa1ae870077207985feb90f6" },
                { "kab", "89dce5286fc621d3e7a2924902842cc961bd1b3dcca2b9f6fd01f6684a2ee2f52dbbc14d579787a3e615bedabaf981168e253bc1272c712e4016b14535495f04" },
                { "kk", "f8bafa7fe23db8f6b7193c4ca9763f3a34ffbee60c07a7cfcf0a73892dceeb1ac672a86468396acd26b567abb6c72e316b021145f8d598a1d1b60e94bbeeb013" },
                { "km", "0a9fc2a5325556044da25b52407f005fd66300e46db4412a7c193fbfcd9d416a1bdb51275e809589c763df83e8dfb38851c12e9aeee59eb31e17cff9ad8a0416" },
                { "kn", "325f7574090f7fd59803f683949c4e8d052759dba99c5616ccb6726cfd9b4f7b4ae7b83beacd7cb90845bfe0e7146ccefee0a5e216f730783b4af1d504c741cd" },
                { "ko", "1efc18dbcf008912550f2d628fec9c587cf5df4f936e20ab8f9365d26706fe17eb97bcd7af2e171cecc88f8547de095e8a4ff41e7d1249e9b85ff9e1a22c59a7" },
                { "lij", "a4301678cdc471b6a5f1fcb90b5a53a71cac0a63098e18429b724f9fa6e84501fcc5445e4a1a6fb2c950fd005af981c3322c9cfb6820435297c9125ded430486" },
                { "lt", "33984ce2860820ba4916d0564c4c3d5155860127cc731e0feac2fb8377210d1f7b413089980dd9d9648bdb04673910ac4e09b40c96262f15d4ffa4102032feea" },
                { "lv", "5cb0eae551bd7471dbb345bc1fc62b0078b06bbd580b9de1b48c6a4305b7315e9694060dacf3c48516f35309443a4f750238947c03f469f5ef6de7bc95a259cc" },
                { "mk", "673b204209a07cfba3856dae49a07b7c2785a77ec1626d381b8fa18ba275576274aa8e7ad3c1e128e47ea83df04cb995c61b5622b9dbb990fb10035f44d57be0" },
                { "mr", "877319638deb2ac4e4761ea77ab0b2a8a1bf0967fe0d6704992576e269b44c7a9c61c4728215b6ed4707fd29ce73b80e045501d8aefc7e77516539fb703d2f29" },
                { "ms", "a3ce0587e69a15dea2e8ca2c74247c1336531b9fcdbe332dc32d0001d84f528f90bbae2afaa8b512ce42f90ce820d55c73f1988a9a1c0c2f969feda942d0a2b3" },
                { "my", "6c866ef4f3dc4b1dc0a50baff4f3a3f421d442ae853062fe79e33211587b6e7b3b7a577851f6913a6a76a15bc73ada085b15b631117fa68dafb6228052c17263" },
                { "nb-NO", "ced58b53b7959e3cb95d0d06f484e477f0e143e09710ce7ffaf75d9a94a572b04d52a897b68b1a1a4b234427a9cc41475674f03e8f080b6c656f548633bb1214" },
                { "ne-NP", "fc2c489713995dd63f12effd957f8fe91632e3b224dd9d4a120b4401ab64c3a52d6f938f179b24d0624e68dbeca6db069cd794b22419717da05441ef87342262" },
                { "nl", "004ed6964e9657bcf12548e921b04e613c123483a17cf8fb70e336a9be6ad88d2c2642746c82af924c078bbc3b32168c29bc4cc33aeee4d3997d913a7873e389" },
                { "nn-NO", "0a279014fd4b8eaabc28c9aeae10747338afb5818e99c5335bd7b899e8394aa7df60dfe85696b1e1bb06a2ad415d5d37ad778ae6601c6f287e562492083386ae" },
                { "oc", "eac2514e9f6a0fab45a628fd31631edc5967364545ecd2cc587a47972b1dbff657b53f9c5d1f34807db18380df89ca80d04aa0b362ace5f291c445c22942d873" },
                { "pa-IN", "e2b770beff1beac46bd7f97872bd7a3c1e36b65725ab0cd22ab744d415ec5f51901a62d23f17572e1d851843d0c2d90b7cc804bb09826cae73ca6a6b75ad3e00" },
                { "pl", "01e5a4c772a8a266f6562376448e93a4e8c4e384ab112056d984c6c9716a5a417efb43e2884fb74239cc5322eaf7a315532532185bc4cfcd6322f7a108a8a026" },
                { "pt-BR", "0685bda65729a801df28d5ffe81b0dd5236afed88dadfa2edcecf2de09e77549d9d0f144c04c535bbfa5378b5b5c976688292c94f111d5f6569a421b903c5592" },
                { "pt-PT", "fcdacfcf0b3d3399af228de09d439d9f017ed665f54179f30de5bfdb346a6c4e0aaa9cf9eded9e0bd5adf2721990ed348af2b78d9515cf494c2290ea9c16fef6" },
                { "rm", "e1b8e0310383036919bf214aa243ca0c8e1d26a9244e3adffc90fc0101b52d509341a67d39e2336e883a51c8206f6d6e45a029bc7dc361197e76fc3b0b9ad40c" },
                { "ro", "caadfc9c5be0f72ec1b3d7bff94feefcf8d8676ca7df6412baa4234abcbc95779528e6e1911551080a7ae18c0a6ee29fe288978003cd46e15963981c25ebc4b7" },
                { "ru", "3fd4bbc7e05960f370a348adf30474f92429f2c58f2dc8592653d86008f063d916ad9a449633f2acd990ab38ddcfd3c14d102086bd6c5f38f11efbb741f265c0" },
                { "sat", "afe7a25227c48fc2e1ee92530a71faa8bfbf96fb40b5ee229ba6a433c8cc8aaa7887b53d965b5a0128682eb0cb2050dd34381fcf2bc8aa717518e94549c69656" },
                { "sc", "2d25611833d6f42bebf39bc73c4aa47a23eeeee5f34f0763620d78454a58091cbb98fe4a9b4b0e4841ec1778b0b425fc46c199cb83941e7d2ead84800913fae0" },
                { "sco", "86047660e43c34d2506cc87ee242a430391da05016c74e3fe445b44e9598d50a7d631691afe4a376adb9cf1d8cf00c07ed2b45499c63b4e8131d6af84747c8ed" },
                { "si", "cb8f1faef5b216472ffc4021e2e663340eb1dcf2c55ad2297a2bef0eedcfeee9b5de461406ab4c92cfcb446359e1cc2c0d0a715ab74dbdfec3a0031cbaf83853" },
                { "sk", "239a625104f5c66e77831269f6fa42edc220a23ad3800f751ea4338fa81f6b08cf3dd56dc857a660dcf6d27426ae180cac055bf047b4f1bbe6ffa7c04371e67e" },
                { "sl", "0b4fbbaeb65277a91c884f414e244fa97755966c91216484c562dbaad431375da43ede1b0718b9047ab9a53d715d3742c960cca8863e9c3ed9c58da3d45af31d" },
                { "son", "0eb18297510e1906df294865472359a8c8fb26921005566eebbef6f439a6217e072ceeae71e6d44251a32712014b3f5f2247fc775821411b34dbe872d4eeabae" },
                { "sq", "4a7f32b7a91cb424bf91c2c01dbd9c4e16e13c0813c3309f17ca234bd627eb4f1546bc34195257b995835080fb39ab45f164042348a627d61feba29f68e0e213" },
                { "sr", "10a907f0aea47ea5b56c0328ed9ecbd81d9ad98e4df7b0f2f3b90077ca93fd1f6b2df8e0cde8548d3b45bb25f7edbdf560ae5412218e40a64b8763caef7cfee8" },
                { "sv-SE", "de3932fdffbe9e8634b6ae049da123105209932f0a6275adb0b214ee35ce758b06a07ef6b3866493017899e07589a70e20a7682188919d63c61eb14227f47a04" },
                { "szl", "7d51f99edee850296b71d60a18586c6f3b741db086a1e05c6bfb97c921af5eeb3acb74e8acfafa777d5eb3aff5bb807fb28fcd7852787a6d44e37d71a6205acc" },
                { "ta", "d74a3581573d424d86f3686ceb77a74cff6eed4a80be008b9bf9b663daf2d147adb8593211094c9db7cb5b63f4dbc5303d95b12d93636756d92a8eecc4058078" },
                { "te", "6ba6295e4bd9298a4303606db32d4de69b05a167bab301b686780b67db15847cb049d06738a8d35d703f95c3263caa11a9c16683162467c4e0b3fe3b8d23d627" },
                { "tg", "eb429f5a26993fa83e0ea9cf948b63a7fac48609c122368ec02bc1eb47794f7f0f8d6922ee10783f46c57ce00bf4b35ff1736a36b9cf92c9b9a3bac00bd4c1ce" },
                { "th", "ea582b93271515c9635b32d4b088a810911a754c4f347a311f0613b5a0fe4e299ec3c4bd94f17717cee24cb4234d3f5fda6d1aec3813ea85963662aba32477b2" },
                { "tl", "a31e8315a8c2dee367a1a8d8d6f5871554efffe8b4efb963cd15b6137a87a30ceb16567d57c5c19bf439b99dbbbcfff7c9b47507cefcd08f17add241a76cd726" },
                { "tr", "d9ca3eddf629705515f52040a8281d8e176d0532ab9c9c184a5ac1d77725c4cfcedd75a60b08af80fd5729c39176464dc4e28e5f6e0d16ee14fbef4413056d02" },
                { "trs", "da972f0b16bd0ea99f126f81c5a46fe112357f937f2674915c5f263b2e41bf31892bd7819d75091e810899b8d06dd5f46b6c3b12d4fd9344319a95a533120ad2" },
                { "uk", "9d76e8c4d1d10ed2eb8467589da974562e59ed06ad2584b8b2c11936dd4db3710c625ab33550c91d81f0a56a657aec00b1bfcf14b8974a474160950bb92c8197" },
                { "ur", "c69970074018e23778285f1c5aa76a85f75d0bf82b290eda6876324ac090c67babb206540db74a48c1d4f1e47735d6ab82d779588800165a67a2c3d8d46986d1" },
                { "uz", "77ba53e04af7434962d6f3d31e2adcb053e84c5eb0be485177b63ebe98f67864b3d558b92dbcd90f2a690f210dc9f5fb157d22f5dd80d665dcd83eed95d394f6" },
                { "vi", "d3f5e3e32a36719af64d18d98d4ec85d3dcfca7ec26cbdf1e5594e712cda69071d21de96f4f03febbd2f0abe5cd3a730c4e73db6043036c04932df5a82fbe918" },
                { "xh", "b3f24d3ddb2284ac182dd68b3ab8b415d2cb892acf2923f7b423d64c832f6ae3db555358045e43cfa29d18316018391e10a79e84bb90ad1e57770ea27f601b03" },
                { "zh-CN", "4f9cea81fde1f38aa6e1558831955e646f6568440eb07a9019c4bdd5c810654a2d4a305972f78e7087f12b182da042db15fe4bcbc3b1a8eeb0035ebf437ae9cb" },
                { "zh-TW", "9fcb043f0a9de5861580c4145cc7c90fde35e082d0d5c471181259a270da9ed57f1f64770ff0673e27c4af026d204322ab16b3cff2fe15453283f18fbcc4c00d" }
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
