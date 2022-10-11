﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022  Dirk Stolle

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
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
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
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/105.0.3/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "f2a099b6f3fe835805f403eb57083467fbe7791e8ebf324f302584a44e3524a907cd2fe643654d41b97b590350869cf2c47ac6b44f8d5652cd1e6b9c85d4a84c" },
                { "af", "4d853aa89e9b7b1afec3f779a659b219e72ebf7c927a1ec44951d4affa9046abe09fcb9861c7c3b04474b278729719492339cc5a40fc8c7e8556603209983754" },
                { "an", "2309e9e8b8aed490a5538a4b8a17458fcfc2d3d395f37f3cf46cfc61c2b6be294d8ecebc7e61ecd39f15e8c63d6b717333c018a5dd5e9404a7587d3b09333bbc" },
                { "ar", "0e3e1bd717027d007c9d0e39fa36483659cd2eae30a268764c511b52a8e70b48fe5872f1cc608971a262dfec5c145b29feb78c42f0cd7c5eb64f73452fbd8693" },
                { "ast", "79b81f7498887d83a5db0a79ac532c567da634895855c7049c1fa671a043da2b90f1ac6ee1a748a2033f96c41fb83cc2aefdd8a3a7292a67776ad51a4559a779" },
                { "az", "7cf36ff4aa71a07eaef4b622b274a12c2f7ea883afc5ae8b5a211b2d03b5b65231e9024d9f8d1c7f64711be8028264f155787cafbc48a59e6baffa265b41aef3" },
                { "be", "03a54f1d9adfaaa6cc093088aaabb56baf85297c2611f97bad203d5fb43efeda12efb859c856da332c75308720acdf3cf0434b7139808149afa0c949abefe8d0" },
                { "bg", "03adc5df54bc36a2cf91ec84f0757787ce7da883f35e9a240dbe9dc218c1e2aa6abf8e8e040f287a615f048787e19f8b8df399987dfd387350623ce3d144ac37" },
                { "bn", "db8d0ef3fd7f70e4d5e7f169a95684710a89a10e18dd013c09d204bdf1480dbbfe8aacbee34dc454ad2db28838e7b2bb16928b8fc503c39c283a2a791b59bb69" },
                { "br", "242d140f319cf0096ee3b9d6240102602a1a2a4ca372fd8fa8e14a571ba335276dd6515957f9a4213413e677f1fb1636dc22ad8550c002ec8b26138c82f6e373" },
                { "bs", "9f83573d800da139793b8f3fafb48e2cc1344ab841c2027ac8c43cbd2fa77b8fcdcf61d80ad33223ae53501385f51e75d7e74cbaa759e37893b2090b5f4df2ad" },
                { "ca", "44790ed73544cf54720ebc0cc79d4ea521300d9d3b38b78dd06c610622d140d91b0a90a589223a08f3bc06718d2244323bf1d5f699a9e7bd687eaf9777958d81" },
                { "cak", "b32dc15462f2ef6979ffd89ab434a51a8984017b803485f97bbb892586f7d8fd6049d0fe99b8295840a7a2e1a317637b308dbc91dcb7101a5401a31f076230f0" },
                { "cs", "90fd3d8784792fa66ec97ba675e55e770f0300ee59113d54e8fcbbd0b61ad739af1f665b9c0dfcacb2047b07d37288607b4358b7817ed8f9f33a88761e7e4de2" },
                { "cy", "01c2b98b63705cef12dbb49efa2bf805d71ae92643cc9e73678cd4dea6b80f4ed0a71728b7ea990c4fcf6dd8697d1d2aed532de4bc11ed6402042cd048e1d302" },
                { "da", "6773d250afd16220eda133e1409ad9cfcf879e257c7620830c20e2548897e2a463e05621e921be4f4e16debfb5ced83e75c6275979c4c927d732c74237bfe8e3" },
                { "de", "3649837486ce09a35e12e49106c621a193ea55e60f029397cdda174390a590db148efbd0c8e2126a2979cedae37367b67c9ddfbcd3c3b8909af05cfb69860b3b" },
                { "dsb", "2cd4a27e6d39b2796e7eaa21332c84e1898a746418ddfd631f170522170f0d198f23ec0ff780794f1696bc01877a5e529689c0e0c95d0340187782f6ceb7dfd8" },
                { "el", "28454fb3b1de1afba204da1b520beb7cf0a28e3afbe847be5dc932bb67463555a266613c534ff94118882801eecfd5b38c46ea6fc82bda4394490c7fb01b17aa" },
                { "en-CA", "f60b2a9db4825ba73175cc884ee58855d7e9bdc331ac42539f67ac30ec4352164b05ffcc18368d40e2baaf26dffca4955e2323e39c772128e0b116adf249f237" },
                { "en-GB", "512250ea1d1a0294ee9a7288b0cb8fc411ebee5c9fe45a93733e6270d7a27d4ae00b6706729d26cff2b4c082e900cd23926351e9fc730f4f87ba71902329fbef" },
                { "en-US", "fa5cfe3388cc1b44f513c625f15188a48ec9211a6d3ff40afae52f8ed379468f6227bfee5c729f09147074c2d3162945796fd37b7f57f0dfe90f3633af0c833c" },
                { "eo", "7382ffa714a85117d7e318bc2c9a21a38e3a3e19885254b1807a94319cb0744e2ef7670a39cb0035b7e80c7fdce0b036bbd4274f2cb3d9537e9e1935c121946d" },
                { "es-AR", "112d872bf1a436943173811a1cc1f9dada06ec30f035975225983352964daa632b4d67c1f1cdc476818f542bdee0574f0c2214325fc854374b77f80f99273075" },
                { "es-CL", "8da7b47f8a2bdb1ae587ae607b100942fea1251c0e6951fca6f426e4303441c6bfd6db6908fd98abcf406c75265fa832a4283826a767c6a170795e9691842060" },
                { "es-ES", "05cb4e20cfd6570130f671712cd8ce42cae25c3b91dcca01cd42ea53287f15ffcc178ad5510bfbb95faf7ca5026ac5514c1c50ed993385d51d2b776c595e4859" },
                { "es-MX", "cd0c8f6b517ab95731c2ac0852ae61e96def6a6fc989b66920b20daa92994268f2d211adf568b4aa80d8d8fdc410cfbe6bfcb30be5cc9336fcd105693d888889" },
                { "et", "0784d4c53d899760651758effb32c32c50287579de62d0a4e6903dd9afd91672bda3c4204ca45a9fae9d0de56b434a5206e9be826690005b6a6ffdff3a885af8" },
                { "eu", "86c7f1674c1443fe50b6b614dcfe013d683cdb6afe0a2f1d5f0f833b0837034d2e8707d8ce750ee47bb74537bbe047d5085926212c0ce2442ad7415d82df20f4" },
                { "fa", "3a0ba3df00c013a18d6342e6459751ff0cf103b395e4476a61c8379eda183d25d5b32650419c1336652d9f72b35d710fde0421698ab706fd2dd5ecc6238120e0" },
                { "ff", "e6fb6e97913c831cf6783c21e45b9a4b2fdcdafe3e9ef018179e436d5acec039e149bb4c4f1859248023ecab52e47f5432488945cce67121ac7f19e3f536f344" },
                { "fi", "755268359a9c478fb4b05e407c80c03256c50bc18c918a0d8d35d0ddf0bab4bd4f2baa35819e2f258197640868390152e4ea53ac29594f1d6e5999c163674c19" },
                { "fr", "81bbf32333b8a9dbca5011e99c9505905744cf33edda9577c0be9e4ae35523e5b652dc59b859ebb155e4e7cb583a0f6efb28eeab1d73ef07cd04cb54ae551011" },
                { "fy-NL", "1175d811722e7bdd1e1d337575675e0f442689b012050d513a5ba666113bc089fc42adc1fc263fe25a91d631973095e8f3d8757d1cce977eb8dac1a71f5f57d0" },
                { "ga-IE", "c708fc4a9a740c946ebf1c5022782932358e2b1acec59835f8f5c921a31b772e7abd777ecb20444541a35a99065f183440688eff7008aefa0382c56297cfe7bc" },
                { "gd", "8728aaa5332121ce48cd117c048eac92a5c9f3db62b759409d2fcdfea148ee370e4b4f598a163a45316dd2630fc1fd4b69d7a9f14fe93aa8cbadf58d7facf2da" },
                { "gl", "8351b663978600bb3121d9da04581e5978f33f2aaf617c4f12e4832cceb39203d17cdb1b175ae0f34950a3ad5f8ce46bf4bc231c8dd594692c52143982b2f65d" },
                { "gn", "bd4c76118f4a94d6483c0030a5c254e7acc07ab52b82ea56bfbfcb40cfc11b444231b8f4189be1ab853ed758ed1f603ab78d32cebb37774d0201cdebb6b314f5" },
                { "gu-IN", "47890acdcc934ddc717ad6212ed602cc679a39ef361cfc89368a03d6ef1fb4ff50f40faeab987e42510eab4159dfe0356d8ba70eab14f1b370411a7b7fd76bef" },
                { "he", "4ffde3bfb8bde3bba8250559fb18e221eb88ca92efbce18e23834672c23a2631209cdd3a51d31c852449147ad3027b31ec5babb0b0b709ce2bccb0ef8d9b8367" },
                { "hi-IN", "4969d78b7ef83c67fe02f279d0ee05e9a8945bb543943d7dfef3a017d1646074786509f20dc5479a1e74dc39b8124184d5526b85e6afa352ba833898de18f9d1" },
                { "hr", "717997ae2e4a2fa4cb5907fb4b29de081df6ebc3f8f705557ccd9c1a4eafaaf134ad40677f1d9d5f23d4519fcdd63b444e092aeffa6b6c1a559b4f66e72f2ddd" },
                { "hsb", "10ecfb4e91c562bff99b0397a706c459d316a8d0e339e9e78e8bd23d1f4ccd7bed3829d2f2a92ea785bd4c14e7a58380f4e5639610a1c4cd52fd35fb91ef0adb" },
                { "hu", "3c71f7c460ac496adbdd1fe1b548f635b88dce799436d2192f987470b0b92dfbc23deb4e79497a37afc320a53e8d30c6d636a47ee354a15d3c7de62e5b6f150e" },
                { "hy-AM", "9b0ddb6277f3146b05445ed32a206555af65eea2653bbcd03aa9247809b215d9819059b9349869d43543d2a11adfc3db4343f8e155700ae8200230a627e43f4e" },
                { "ia", "1f3a2fe9084804fa4999117134950f5377830a246c0739da2d0ebef90c46c9db1cc7c115c96ed5294d86895f55c1c033c3c42075e62e17b96e1e96e533561b5e" },
                { "id", "6729d31eba1ea62ba1f16d8c63be8d250a7f8f7a688ee9aa6ec631ad1532c68461155e127d14d7b6746381d27bd482795153891f6a562765d8193924616224f0" },
                { "is", "c447fb9bab053469a6ab04e0aabfb75d76a8123bfe040604318b87d27211b667ce9b5cf987291fa75ed50c4e0a0850df0b6d348bd37073c3a1a6e6646a6e2286" },
                { "it", "21566b2c1a59206c01b63b04600c86050eb4c57cabe04bb0aa5107741d7af3005d5cab92cb18bd335adc0cde03132ea82da7254e7eabfc424732353e070bc43a" },
                { "ja", "180f47ea3290019a533a8a360b241b6a10d5957aecf7f7fe491d138d9a1365886709425a3508a06b59bbceea109a8fc4e059658568f23fc31e250dbeafe63a28" },
                { "ka", "52089ae43140bd838ef29d97cd479f3b0e831dfa917e5be35bb3142c8a806afd6b39370ad9f7758072ff85e496382e83a0551a2897150e065f2e628462789fe2" },
                { "kab", "b57e3ba22da353fcbe4c19388f5a1f4945030e09a4b54d7d76b259a04d2356d86d2343712e04891c25d7b418d80bd21b2033dc74a8d201af8c647f93ae3fca52" },
                { "kk", "f71fe7aacd807c97cfb71c6ab03cdd71e90ca599dd0a8966ff4e2953e60a5243a1d9aae9f905f2651a77c053ca973b7d31cfe604b5b0a381315c9115f6969e2d" },
                { "km", "e13d86d97202f44ff4dc10f74ca3263adf9094cf99ffbe75ecf2c3e3bbcd627f0729549765aceedff677aba324f2000b7e7289c9d3109de3460e7d6a1b2c8d5a" },
                { "kn", "bf478092704dc6ef9f48602281b2a90b53be008250b084b445fd21223bb1266aa97532dbfde3f80f8d83a3831c862e993d8bea1b682520ef6da2920fd5a3f7ab" },
                { "ko", "529780c41a14a7027c6fcc2d5d169a45f2026744352712f3a83bf08c0ccb1505d1675afd6fc8b6bb17981e4f1bd207ec89f225b87aa1f2537598950b53a4bb68" },
                { "lij", "44ca61a92f8be2b7cf87263cd780829898a3f94df0e10ed5fb7dfebfbc281f7ec55a01c9a8bc3e8159a89df202bd45bd71b4c92bbafd91d89ac8bcfc233b8964" },
                { "lt", "c92768d113d29ee61183b35b8db95a2beef54853dde360683d741098727e4af6d811deb8eda9aec05fdcf4050f0cb12a6cce85a6c96c96336624361e181fd85f" },
                { "lv", "caea07f37cfdf06c9cc8176f016043d99876c2d77198376997c39f9befed0cf875a30457668e2e9d255f07b298402b27c3e5c804621cb4a6220974f2d692fcb8" },
                { "mk", "bb7d15f47b96fa73c2fdb0ef3ea61366341de46009c119b3f6a9755b56b5b5dec7babdedbd998f245158b4c2d300a0e3bf6628ad717a4305c4e82df5964ec965" },
                { "mr", "b8852fe37ebbdf48ade44fb469d3da1867fd20d941ca2d348abcdb17310617f740da9db558227cd064eabc64ebe01da877e37d7eb02b22ca46037357afa03157" },
                { "ms", "50fb058f151508c701874119f2ac06a78983d2c4bfef6d219d10908835acaabc5f03b3af8e28b2e518b4c88c9f9cb5a148230d596a7df8f4c797258cb2afc851" },
                { "my", "1dca11f390643c4865716948644eb5c2d837bca216a8ef688345960afe62dd10bb8347195c8d12a54a313dad5e7b93a1df4390c4c730aa07d89150405ab82e24" },
                { "nb-NO", "a4749cfe5dc31d5c90822191783c268fec4125f56680d47d43e946b943c85babe6c20c168edc4519bb9af6aa80b96a2c56a7d7082ef6af82c19cb96a6193495c" },
                { "ne-NP", "a64e4afa79535617a6ebe13ec3400794c683f3221ebf63820e0ff65b049e3410eca3bbec08c7342daa06904825688b579cfdf407dc4d750af88a901aec669fd5" },
                { "nl", "6051315b17beb21e1cbef24ff5d5f78da5b945ddaab851781c74adb3f4a82439c7e5cf5a3c52f8d31a8d5d887a9b6f9ce4d5f398219cd8517c39118f2fcbaaf9" },
                { "nn-NO", "e4f8ac5fc670a6154ac4a7d16d089e256d8eba4a4e0ae992add9e7d07c381507fbb361fe7938241cc839414c6afde6053ea57867cd4b10d71983e195fddd184b" },
                { "oc", "581aa9b9d6ba97157f8b47e8e37e8f4c2dd13c54bfa9019808404e394e6952a3f88c16343ffc412f3f97f50032e23e2f51b5754384ecbc74c47a04f1576555e6" },
                { "pa-IN", "cd80a28df8d6fbd8a67873415b1a2dde8526bc477032d677b5f2ef5e15cf2f18c5e410d30016a4e73353cb2b37b29e366a55a53fa85e37b08eb402a0c2f00e63" },
                { "pl", "f4ccf6df213e47febe3faf12962a101f6c63052be52cb11170cb80036603b45f8576e77db423e1295693681453eb676c6f88258d47aa95026e22d9c0f1168785" },
                { "pt-BR", "1820231225f4b9e771f2fc631baebb9bddbedd6fbf13406d1d6bebfc34424225b9d6cc1f6630e6c0e59643f43b967fe9330341474cc60be4bbf0598b1add4c3d" },
                { "pt-PT", "aa6a639ee7815516e5379bb877cc9359bf0e578644ea38becb6ff59ec0107295985a50fe5f45e463cbceedf9932c932495473d5824c638ea5272df682dc2e643" },
                { "rm", "f8e64bcc0c58db46b14513525d53ce7d54cf900a8b8b3ff7f71bf446c3e5031e1796ffa250e515adfc046fa934a7b08b18fb26132370680f30fcd847fefbcc3a" },
                { "ro", "987a5ff3bf06840040e8b8692b938019ccf8320744c13547ba3dbb9fbd20a8af9727035bef1ce8419f6a86ec3198107ed059e4f00ed5f493fc677150e16a6dd5" },
                { "ru", "71d1939e4025d984ca8fda4294a04a5281d59ac72b4f329ba9829dd7b1366b83015adfecd62ef9bd03bd3250305121248d7c7a609483ff3c37ca908fe4ee5b53" },
                { "sco", "df64fa1de628cc4e8d0f2da80ce4a85db81943c2539a04b330bb8deabf8bc742cde9aa3d4372cefd7e537187cdb887cc11fc9df1584c43985bef8c9bde918da8" },
                { "si", "aa0f624e9c5691edfb20717fe0cc2c2a023c26cac9d3b30495ba77cd7cdfc71537b439605bf62c81340154361d3b8e14b38ee8dfb28e7f2757884d6282bfff2b" },
                { "sk", "2938586e3c0719a17c08ebc570dc1c5325f15e1379cdabfe1f121c1aa1528108fa66c6183ebf0451d5154097b977d7b808118437d63a764ea77550e484ddafc8" },
                { "sl", "214174705ed90d68ec7a886d1a2310b407ed2ae7467b6fa50c84964512953ec02189aa8a67431ba6e068de72c71b5a9e6ace3bdf42b4579199d6e5ecdf972d35" },
                { "son", "929e1dc1d25d7b27738c68169444cfe4275bdbafc7b22fc809d716cddb5c1221f4b6541fea4be93b191a6de85dc05fe58c552e7aa8877eab21a0421028367213" },
                { "sq", "e9b05a56da63101761ab110fd9bd6280f2bb15a8168a268b2c2feaae9bfbda376a57c1217576455fc8a6ed9f789e69b3977409f48e1416ff23f822af90c3ca23" },
                { "sr", "0aaed723c9d8d24d3656834f4a6b0e4ed749137574557634590967250bc0145f1bcbf28a2baaaca33190512b563f2af04f9ee71df2819620249d13f374a6f661" },
                { "sv-SE", "bbbefbd9841f8ec89f9d31d31f6696931a9995a928b2acaac7e3be04198698310ad9654fd0a385b67913b3e612748598dae552c682fbd9e11c56852b3e6a050a" },
                { "szl", "bc736d888e8adfcbc78270f9f637dd40d7b79cede7bce649d498b2a9412c8a3bd8793e91c2b41fdceab29a12b994ac4cb15f7968ba61b204229f4868000eb1ce" },
                { "ta", "376e2020d9729e264a57fad48582ea2947628093ee6fb8f4b4a0c6f9be3d3e63026a0094c14357f91c9e6bf0a31adff24b95dced370a13093d1a16ed6a1c99d8" },
                { "te", "156c7b502ec8bbcbe421a4967f20af02fb815e6228545cf6010eab245120d6b66375c5092134360218dbdacf9deb47d0e9aaa4e128382cca43a2bc594e35ddfd" },
                { "th", "6f1cef990261bcc875aaa1fcf17d473102c58d3be4b13e44b67a1a6fefd49f1f1d20246826212cc18b02f9799869dd913d271bb4a5881f2599ba70e6447d74e6" },
                { "tl", "af4802dd4ec08edd9e5d1ea50bd60115bc1566f2667dc2aee1f50d035d6c917856def1153d558a96ed84baaea12a1b2a1fd8fa9f5d03b632ad430c8ca1348739" },
                { "tr", "8f1239b8e54a1576760aa9d2d15b566f85d25ea0353f6f18675dd1fc1c4ca99561446d77546c66c30900085d962171deafd96084aeb2935bc703ae249a959236" },
                { "trs", "f249d3a9a2e81b83884fb89738f4a1eb0f62c0b4b863a81b7d0bce61fc7cb393b09ca4297f8ccd1567cd672a24675017d8d1ae9ccd42ee11f226a1d399f4dea0" },
                { "uk", "8fe1070c92fe49bee27cd22baa2023d248e27129e29787880f47f9e814d8bc6c2f323252954a6af5ccca749d91c9f3c0014a05dee643045a51944da5815dc2e1" },
                { "ur", "f3212ab0eb3acd888954545c2f37a6e5e98bf23457eed6e7be9c59f8b0b1cb1c0ce3477fd275e316fa01a13196a88a5e48ec94e1df614694e22f481f7393573d" },
                { "uz", "ef2b935fcd93fcffcceef631c7f837161049ed5361520e0796a9f92548a16594bc44a937dd1cb6b64dbd9376c150bd493d27625e55b9697841399cc6d8e8de71" },
                { "vi", "75595a0a82958d101e68fcc9adb54e7ac6284b52939336114e41847b5c22efa28d70ace91da69d384a6f8d1f0628ceb4f128d619feefe5482330107063ba655a" },
                { "xh", "2ff945a695128b7a71cef8a2231f04a9dea4fba877d3d8a1a029260b27a63eec5f01aa5880599ce20be2b2e48f3a6937cda3716e51fa1ff5caa20b2eaf6f08dc" },
                { "zh-CN", "b993ded2bd41a1fc2b5a720b55503192b7c255540cb4fba63b830bce67acacf53903bcdff91bfd16feab4aa94c73c404157dfc8a43422e571d430bb4054598ba" },
                { "zh-TW", "2199f513ddbae933e6744c6129cd7f15f81be584ccb47e02703822f34335480621fc27106826e52e753dc32cec91c1fbb0866de29710d07368022667e73ffa4c" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/105.0.3/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "6b7db277509bc230d45555be940ed31e5bceb0db8984f5381a63ec1fe6c62b6b55dcca98557dcbdecee959c984ae4f6284a03e395d06d162432a2b3699e2ec93" },
                { "af", "b0f828fab25226c1a8e03627e6c6b6a0746ad93d165bde7dcc0d4799d80710782d90ef30aba3f6f945f6710d974bb1f3aefaab646a17ed7fa04258d24bbbc557" },
                { "an", "bdb3962fa724651b431f3d1b88c085f062de625bb05379abbcd8dc78d62df1bc53454ab9948efb967240901f09493c630a90c4540e6e387d2a8f16a46885a686" },
                { "ar", "328492663b92a01c6db49f3d55702c1e5bffe2384f83f6130be6a3ef57c51983aed047f8bb94f5a7b9ba227bad38457273ecaa89971ef73020857a1e419cf5b3" },
                { "ast", "e683da5c4abc170ba9c139a5b950872bb8158c361c8499a8be237e5572855e7049d579af1e11bc5c0dcfc44a38bc2545f3ffd9560e37da9e120aba55a39afd16" },
                { "az", "c276185cf85e1f2071bcd639167d6cb681d27a45beaa647254af0ebc2f78f26afb33cc010d1161e90ff517405d63b71455d658a7cf47603917898c7c0199c4f2" },
                { "be", "3071a91084d7a7e1304cdf5840e38e848ee6064df1a0f786b921415eeab6413355ac02caf109d497fd2f0ef87b89bd5ff72cbd913f216969c6fd0f29a8bb69e7" },
                { "bg", "085dd54f99737d920307102c08c6936e24ed5b97d5750f28ed15ddd178e6e55e772e33c323141d456c5ec98ef6c39b34f86f090a4c9059cca167d6f5894fe82a" },
                { "bn", "e3abd485e982bb2be46b4d0c535e17dff1c052bf7a5a20ad3e2b65de05095df8a6a0dff37fccb53dbe8a267854b7ef9cfa4bd1f0ca4766b7eacdbee8aff8cee0" },
                { "br", "f1f95fa23ce0954ff39813f6ae8ab830e2880fe38dd58fab04b371fd15b09ced2d4fe98d0fb98d831e3192ded38f4dff905d8177d40907449c570651884b0889" },
                { "bs", "07e413df186f74513932d43f5faab418162adf0a6fd7c0ee9c4a66140d9c267de1c35cbc31e6fada5869984adc1c15e9ed79e0fa808b20f31ea1b1e88ccb4746" },
                { "ca", "1bdedf12df059c5c88eeb9a6abeecb9e30275b21fad7f3f30e2581ecbf26c42e12547f8993c7f59864df1268cd1560f68806792e0364efa98be11e38eb1b1186" },
                { "cak", "c0d16fda6293eee28f5c221af6d1f7519fd2faffd200e89ba8dd8a7d6aa093ca8ccb01e4eabbdffa1b671575035c49ab48652734b67bb5417e41deb642c6f9f7" },
                { "cs", "cd1b70f6a4cd32554c6c8774e00a7d9f2092ecef10141ff48d5e6929734a29107b974a465439d533927b0156bffab100fa50fc75e5f08135a3fd52ea6c5df2f3" },
                { "cy", "7399dd2278a2455d3f121baf4c02187caa8a0b30f70d2c3207055f2f963c1d740cf030b4a808931d726529150f98295617482e0cb54adbdbb9ac5e511b157cee" },
                { "da", "6763e75c4865c9d65bacd6eb95a94f8cacba35bb63c8deeccb9e22347ad2a36769a89190128b603517f762209d86ba7681aefde5a2e563fbb3308925cbb60082" },
                { "de", "fac732f5c76f5a4b615c6ec7fd083ddf86e88b8e37bec429b5c44d3cc92f84fa93bef28d4752a0f836757febf305a8c7ecfc4529122623fdfd12b1db7d4655d7" },
                { "dsb", "b1108b0bbb813f92a3fb5cc5b60cea4c0c579fd43909297adc88c09d47c4411ff56ffdd9a52b68a918c1d02888ad8c6ab94e92e04edb219e021e123a047f71bc" },
                { "el", "83010169af7befd7b1d10b8ee95d50e028896c20c0f9ad4f645a7165ac73ae1a8dc12a33f5dbb49406778cd88bb1caa4a5e1e5a8e7cd962d3909e2188a8dc2eb" },
                { "en-CA", "36346a188d8f86d02c20d129693135ef74637494916a77b9b1607dffd17a875c69ebebe31f34871093137331d79bf4463e9795c0da20841534225be6a5c96778" },
                { "en-GB", "877d947e25343b0b7ad520859d5ec2ef56bd2b7c5c05057ce26ce9f7fb9828ead8f3c8faca9f3412bc5c0f3eb4e361100131199b249608a8e90c65de8f86828f" },
                { "en-US", "008a2d227ce96c3388fa96adefead942950ace638a38b9f6d9b6a61303a851b1bf2b7103041f96b485f555ec16678bdbf000721888488e271a5016b1d8b35d37" },
                { "eo", "24163ab26f123cfa17fd3828141b339ceb7c055f3f28b8861c98c57c807f36710d6ce5743886630e3c5d7a17fce557ea42a2609f8ec7f0a3258fe7bfd5259d1f" },
                { "es-AR", "5487b25cc69c02ff09d6e7e9933a41770fb37767bcb217088acdc3f87ab3be60e16a16d1c155b01ab1eca6f053728595aeb694a523be51c9dc2073caffa9e33e" },
                { "es-CL", "b7c2ad86c305db123ccabe69bf1f23c6b9ae98697b6770d53f4062fb2acaf4a869586d7cbb18f4f4e3af066ff249e90538f2608bbc526de68b86253212ab9422" },
                { "es-ES", "1fc15ebf9d13d367570b17e738a3cc51ae3fc7f51474edf151dd989dc6f0a2d7386c2b12c1758b84745923d69d342c46252d2c6a6b5d86031d4829766bd9b417" },
                { "es-MX", "143901a89c5a8a383d7f9d025cb7c2fc3d35ad9b2cd660fd4a5590acf9c5d466b62f7e163c836ab1cf513824d6b3e35fa8d2b5f575773851f84876f9098cbc4f" },
                { "et", "4e0eef5e70d1a6e4849666cc71cd5522b20cb202edef9d05c78c772ad8169518f202b74f23bbb876d8a93f4fd2e74dd636207da5cd57c17ca153802e2c0ac06e" },
                { "eu", "d2126843647fdbbdac0cae1e7796ee5870dbc73986d5b613cafea904a53754b1d374340d61f693741dd57504081c06f1e494ee94f8b8ab98c6573f4875fbc45b" },
                { "fa", "b444cb2ccb24235b2712c5b48abdd67be91742701ccfa505a5c6bfcfb0345a50dd8c0577c7e6f2cf3de629909686e89ba941967cde018b3fde6e0167bfb56047" },
                { "ff", "4b2908e2cb00260a82f82657b9bffc2984bb86960d38bccc30c27091b1ae2ae924c065610dc82bca4b4a98d1a0ec0ff7c9a3040add38350fca48542e3ed3f06e" },
                { "fi", "9d78ec55a0ad8708d2c80ee3db0e519c2f14bbd9382399c072c1ed1b2c2dd107da489dd5b3982272ba87547c5f7f59f8648233b62d0567be6468a047d170e6c9" },
                { "fr", "68b34d7fa048f3b5adc042f50fd735df90c6ee3cf2d49012f3582c3c7051592398a1e3319b5a9c8a1889aea2039615b25ed8001b1fce24fe8d1f46ccb3cf1263" },
                { "fy-NL", "ad508274747da4bc674169faf7cf4a99a88f6706fb958010378aee1bb0a0665b9f6e6d4a8877c460ab3205519e77bccb52260fce4fbcdd93f584d2e494ea1d95" },
                { "ga-IE", "5e90e409b0529a29d9e9f91e32844ad49e15296b747339d824e0c5272d19e67c8219b1ae08dfd18a0d796df20f92268283052dbe2dc3bf0bc2009890d2e54f5c" },
                { "gd", "2b47f37bb84c491a65dfb172a02689afa6db0abfea0dda0233a58337657cef9a3978cfe4e29694979d8c705a3cbc52ecd05fab2f6fb9b7ac260c967975cd46b5" },
                { "gl", "9ab421af631cc57a4d1cb4885444497ae1e6433820e90d85a43fd7f4ace78dea7e05d7a0697597bd19ed0c7673f05594bdb44e37a432d55554722133f8a2c83d" },
                { "gn", "237cf5b170a36bee24b20472ec8a326f331616889e11120a5324f3957f118f533190d836a038362dd7000a6d6afce8622da61231ee6786f113dc09d749542f64" },
                { "gu-IN", "fe8cbb3fe4ea9a571415416325e33b36fae218ef387d1ae1564a37b94dd61a54941345d171c5ada9822ab50edf38e431ceda7f2593607bc7bcb6ea9c94c45b21" },
                { "he", "6668e539f271ce24c25fdcbdb7b8dab77fb58b2dab9be7d0ffc96c8cc5bdf246b03621d0b6e8c3debd40bba0d914f61f68eed760cee8b16f3a8bc61cdcfbca37" },
                { "hi-IN", "7948b2bde4a9c711e1681d1ed9d201ebd80e9c75624000ba68ddce3b0e2bd5522314191dd4931b6b24db139e16f1047b5f7e109fb3ec99bffbb59cc74cc950b6" },
                { "hr", "b463864d3b36e506110297e29a761c63291fb1c700b7231813bf7372ecaff2d27e9735175477157f45cc8fdba10594ca87ffe004b39e05e078c9cac8d91c6e60" },
                { "hsb", "0655135cd13306428440701a465645b787bce753052fd027d980a1ed72100ef988d6f99ecfb3202d6eaf6093649b9ad0f5e1fa8e971acd780d28950dab69f0dc" },
                { "hu", "72fd0d392f6fa745cca847de014b477e04d967616987f43f91dae3284a47410fc62f2cfa34d3225fdf7d1d035a7c099ac07fd5382d11b3fe366ecb5cc592640f" },
                { "hy-AM", "ee7b739f61e41e877d7bb3f152e946665006d7550ef0a49cc1d980220c99ec4bdd32d369c22d439abc143ad60d519d81666e4519e884b93d3a5e89cba6cc580e" },
                { "ia", "8f60b4f55c9e52b9a3254e3a37bbc3be8e92e148166e85399cb1269cf7bccbe693ba17323c8cc159dc77514a607fc47c14de13199f83abb6c775e07f048a037c" },
                { "id", "52a3a54a1ac5d505df949bd69c49b3ce2b68ecc12bf3c2fccd3a35d5ffe13371462a99c2336321f07b36daa03f3d49a0cb37fb211a81b98e411857c8fe22ff28" },
                { "is", "f24a0c5863d41a107b7edf33f6d19054fedbe1906de069829b23a5af61ef7a062f191d6f10e2f1a75831dca9e5d1eb396f13015388d4fb5fd2468f69f32f85b5" },
                { "it", "df0466fd9381d06adcaa9e90a4d9edd0944697d25b6729db8c80c61bbffda5c683d706c9a041c7efcf0dc37408220455c98507cdd7283da25b8cde219ae9198b" },
                { "ja", "c1d5796de616a8ddea3dcadd1ac14cfb7b5773afb160d92f131d83f4cd3944cec25f6cc35cbedb5673c57add4603055679439b44b09c6afbf9bb0c86b34f964a" },
                { "ka", "4c851f241d76999341157ac7cc884b499e4c9311a1855848fc33a74da16ec6597687659f3cd27e31b9b6322f06ed8547d8fae1366543ca7de6a5f5b70220763e" },
                { "kab", "0c6a937d16fb917e6d685ee2be510f29147aec75376a9bd72ace63a008117de078a99a71fbbbea10a5cf96033214be42700a50ef1fd99ed972828828a6466b49" },
                { "kk", "1b9a64e9745c794b60b0626b4c6860bc26c2865d2aab0ed2706570f064dc2e685fe7aaabf72cdc9aa79c60ba44dd1ae739ccbed7379444cb8f92579eadeab061" },
                { "km", "9f05542a9b5ac0b6e64588232fc088034ead0f9c4975f64f3272633938da0f2842ba5b6e8a65ea8321407a3678e0d3267f14dc74f5e21ec81d4f38250161d993" },
                { "kn", "fa6aaf976d5017c0d04a64687be8b10ce1d847ce149f6bf1f6e1cfeb7b58e636e951ea3ff3a49e1e7f4de4ef6e2b16d3c81aa10c030ae611ca9a9d93472a4b41" },
                { "ko", "b4bcd1aea5d8232ae185d9e3a860084ede36c3411bcafe0161882685eed8982fae71edf70db9fdbaf919d2b8709454d65339afaa681659b98e8c4ca32bc49ba1" },
                { "lij", "3af771e4683c5abd9e32d7ec2971405127a193f1340113f734a5aed902aae2cd830542c074c16c96b6cb1d2271c7c9e466d6629ec82b558fa2d32a7fa1ab6661" },
                { "lt", "5abbd0c2bf393a934f3393157f51c36a8eeb1ba6cb5210ca46c2827e6b0ce484d0390cd32717a0668b0b079b1c22f1e57027815d068887e6acbb9343f9e29815" },
                { "lv", "29f558b3adc38d629c85c8bc1c609138c90489d49f979f510457da390e07e3db2fd3a245fb995e89cb92dbde46f09d234a0ed4a1db8e94eee976e9b02689a25c" },
                { "mk", "3a07af0c0c7574874b243db22ce214e2dca0958998415e0fea53ba0906ef99fa484ff3d0807884d8b91afe0a597debd16772554fd7c7c1dcc9da0569fac4b0b1" },
                { "mr", "73bbed47dac9779b6bd3159906b3ee27d244333500aa88e9f450ce33c0e24b3421347141ef640a45c6507a1c7a5425749bb05f2be4db183f6ccfcca28af8bc0f" },
                { "ms", "e1264b79ecf34ddb206ed0e65babb9a709afe5d845141014c155b2e51c1e57d6499051d7ff9148a2a3333edff51d94ff7c455b03c2df83874486bbbf049d77cd" },
                { "my", "90d8c8bc50ff891ba7daf2d2df197433611fdd55c389b478b1d446c69d83dcd5f226545bd2b2543af4e7b6ee27b6f73456a02049c5ac0a10d749acf3a20d68b3" },
                { "nb-NO", "9751ab9aa408b98993569118adade800e4f2748312a3a7111d821ffe65328fdbf4fef0ead0d4b6d1417a585d10a156d31aa37194b7de35b91deed6df78a009d3" },
                { "ne-NP", "7db17031c2972527a36fcc515d44a8443c12aba0eaa608c0b3eb84017229c00a54086fbb47b1e26a7c77e56d0195231a541fa28a92752b8526fc12d5716a5d77" },
                { "nl", "2ebdae980599a3062bf4b95ecf12a5b37bd5e6e8124a695e08515819c4e8a6f5324127b6479632c94f9f4be027e9fe272480fb610fd2bdd071bc187097215705" },
                { "nn-NO", "b46f8e144fea0e1e3a2226dd1182f901098fa30b9d07c96edb973982257f124b407d7ea07e9c4c9029ab09466edba598f463e07c076221b7b340e74437b976e6" },
                { "oc", "69d6adea4c8056ef349f6d4e88b3a98a20068d7e6a834428b44bb65f7c0675a5099cd58145dfb9e5ee84b7fc65ea96b51059b2241ef2e2ccffa8bd1de2cdf3fe" },
                { "pa-IN", "b959fa716bad9d50a6274533fcd783ecf745bcfc20383e1950dc0090fa1dda2d0d7dc6690038d7be11747b673bfd1fe00bef7fd0e67e14ba938030ce07ba2f0b" },
                { "pl", "c25e7917bc347124f52a758f67be61af0d9f239db75d3d042e4a1236545e399f13bf10042e215f0a7de0f6c202bbcd86a061bc622a2ab1ed4a829ab45f9a8268" },
                { "pt-BR", "b42112920f66352a1c742d1f3a9e7f650a32fd0c9018accb4f951c431f4850a01706330beb3eb8d75793baf6d0e8fdab7f542243f0097a70e7059f30bf4d3160" },
                { "pt-PT", "e5898904420b0b266337dfb56ad208a0b9c7097a3f4c521da04876216b1e5e7e46d845e4a3155642a1babcc69f40dfae362b4c31f788f8d93fe169f5c4896320" },
                { "rm", "fd979c046f247626453d593b64620bf1aa326383822a366d461dae70b15317d262e6761a552143c3ee836acefb879810cede93e4c12af5a9cbdc49dcb9a30f6c" },
                { "ro", "0932ed815889aca7e62f4014bf088c90f5d90674046106c759ccaebedd7825b6d776392a0311219125cce24fa7e9d2e269e20c086d021eea87653f7827d921d7" },
                { "ru", "743c702f97755fb83178c769a9f4b8fbc2a5fe72a34e66011a8643b86e6b0e04b78f6f707d2c5acc2154174986989a7d3279b160d5348cf34fcb1f6cc702052a" },
                { "sco", "cf7bb9a0270194794f6b7ab03f4cf7f8c344934c40dde574eb3ae056bcddea23e294849075bcaa86d367da294d6dca6c1a75a43a448c241f886f67dd032a270a" },
                { "si", "ba53bd2fe1b2a14405c2e173d8f452c66205fe394a50b3c174a0b08e39fd638a197b8d00fb8a9df05b2507002a6001c46a4d17130973e8a71539f885404c945e" },
                { "sk", "524b19b252245707afc7d3323cb53cc03ebaff51d3b3573d935ca444eb3e6ddbb10dc6b7e935856b8e3356eeb527798ec1eca63960297f89cd07c95812ec317f" },
                { "sl", "f6de95419d1014fa0eff2f8103f1733450eb871f6c7f958285ffea8f5e8324ba2afb3ec8239278aff653a26fc518304efffa225b909c5d24e0a1ceb187e92e61" },
                { "son", "1e7abbbd2d1ebb92cdee80af110a756ac64141dc30965f5f425a43ea8e3d07c712aebcdd4dc8e747897259ef681e8c6e18e639a955bff96644b8bf6b4fcb62a6" },
                { "sq", "5022fd23df04a09db942f05e54e57f3bbb48bab00de16f8af59351c587a2559bad13e0a28e7488b922f506ab49a3f4a1e42a2a803ccdc4be4c64bb97e7563fb1" },
                { "sr", "2f61428bf3faceedd5eb886a32b772ef86265cdb73b817bf4a6bb227990540164d322c71b9b41ff4caf6cb1b7218b8822e7b0d6dc5a2f426a8c358cdc367aca5" },
                { "sv-SE", "06770b62c012d00e7f6e763b133b138e6207f84d907632fd25bb7c98fbd022b2deca57fc236d968a76e58afe7250b697c8369130e4421675c96b1452646e7def" },
                { "szl", "e2734215c795d1358c03d2c0054b4071c1d1a5ceb0d21048a2f6e773cff7685cb465a75704702ad5ccc08df84cb29a185fa3bcb4a6504d10f26f9f00758c1a42" },
                { "ta", "37904b7a599552abc0b8bd1c2244e99da1f4c17b710892882c24a994ad4baf9eec7a512b2ad020afee5f0cae3fe4e012cd8ec7cc71e09f192d621a9efbc9786f" },
                { "te", "4cba3f783090a4ff63fac08459129049b9e3ade9f94cf5aae22baae943762b6013e3aba4600883f7bcc5e46dc9233bc59e5e7671eb904ccd983d63d956da921c" },
                { "th", "ebccbe5fea7cf942bbd21de769f0e8ac70936c861fcd147a993cd3efc8c106ab98c5637cb512ec76b6417bad7d58bd9ab02addd070a249f9348ea2ce5ccd3658" },
                { "tl", "cd7c8ba9942adf57615ef524f7ddd1034e6f21d428922df9437ae7be9b4b9ee6f13e146b5091e4358d4fa12dedac695ed8bb55551b69910d7a92820497a55349" },
                { "tr", "a1dd2223cb0fd4080fc05246bd90aded411fabbb8663b015f85a846eb317df605d4616df91e97d771f249c84f20149c75a54f12db5e25d2a03c7eb5ada2a990e" },
                { "trs", "36c772cee1e3c02f963f4a311266345eed8f70f875fad39c267800a661a8b112230580280a4e4d4bd592dc61e8c29d91a1ba9b1589f4e2cda1817d6b0072e0c9" },
                { "uk", "32047031060eb932d3d178abef90b52afbdad16159f688a9e2886dc539d3df31a9399d0681e125a27e8094c0f0f7192d19b3eb781b94e423ee7de7a7fe4d0796" },
                { "ur", "32d15a8a58ee7c7159b57d9f0ecc64846ae324bdecb1e23dbb52bd63cd3bc3bd2216d1e7672033622f2654b15514fb9b9d569b5ae3bc27ca3956d08ddbcbdcb9" },
                { "uz", "f3faf866a42b8c7bd4d78779eff6ccfacf6cceb34768b18270fdc341db86a7bb8169dabf07a4e040e0855392364c7b3733e936f427a96561878c427b8e543568" },
                { "vi", "fdb3a224b1af68a6be1d7a698d8d951258a56401c49d1cde46015cc5baab3c689c7a16e5003aef4a7c32287ec56b7c73b579be3041fe1953f1bc10048c3391f8" },
                { "xh", "ef2f27c0578f976a6f7bd6030a97ffae87c28559b4c7de6b7656d751241d9523becc199a5fdd45f79997509473bbdbce4b15d8a5559d004db9d55cc4d658ffb9" },
                { "zh-CN", "39f272a8c451c28196ad7cc2c77bc042d7b2a8a00b4383f64bf0c39f54ee341efe5b815cfb7dc05bcc58cd78c65b4a9fe5f227bf186124195a18e2a74aa9c6e5" },
                { "zh-TW", "d8de197aaafa563fa5d9fa90284719ae62df75c1a0e7669f726b2367e7a98203a6dc3874977b741ded13cdf5f8e5ef8f3ece7f3552c308ac10d6b888b6826bb3" }
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
            const string knownVersion = "105.0.3";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
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
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
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
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
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
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value.Substring(0, 128), matchChecksum64Bit.Value.Substring(0, 128) };
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
            logger.Info("Searcing for newer version of Firefox...");
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
                // failure occurred
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
        /// language code for the Firefox ESR version
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
