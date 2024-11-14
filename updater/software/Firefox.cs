﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
using System.Net.Http;
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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


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
            if (!d32.TryGetValue(languageCode, out checksum32Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.TryGetValue(languageCode, out checksum64Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/132.0.2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "4dada91ca35d7e79ac2085f560c9fd7e9d96eb319bd6fba6a0387029c2ad71e1bdccb11efadad4cdee34d7b1dc4277211ec531049fff2283c0746ca67746c0ca" },
                { "af", "5a0e5b505a41476a5d0484d07e6bac5a26bb72bf22c539d99167971b5590bc59d354fb1819a30ceecd4e2f09270414a9017a06438b28a903eef1d8f1ae467e69" },
                { "an", "dd8ab55216417beaf12c716c28d152a28a63c4dbbd2705fcd2fceb3959d3b4196d33d972568dd2cc886d3dfb4d249c9682c3aca86a48a0994d40badba83882aa" },
                { "ar", "4cca6074246eba68ef20f9d11f11b19a5e10584fe57f4866c4ebc9ab72e97f4a0a135af0cf0b5da167a47f35499db1dfeea7aedbe716a3dc9819d84a9c5c4f55" },
                { "ast", "d98a31abcf20c4bc3241592d88f95e1b1e6fc021a42e66ddec3caba130ad1f98aee44189e4599252a5c87ea004201d336ccf42e350feaf2db55c20fc0c9fb524" },
                { "az", "55c0b0b20fa5e557e0b2bcd793827f38299ae0b45e1c3a212c85af409366eb2a84bb9d0d2ff96fa292b34767c5402f7e4725cbe0d5b6319dc71b13cff4d6573d" },
                { "be", "6b011bfe72cfd03b7a0a7e467e23942b591642262168f90c1de7b52b1988ff7e906e9ffba43f6ad81e968c7e7471d8bbea703995d732a02e375ae4d6ffa8f6f1" },
                { "bg", "23fcddd42797d71a579dda62a14daf5b2421d4464e0429f605d42fbdd16e493b6dc808c027ebe8b477938754098430a3820584d67b58de3f35bdbdaeca24059c" },
                { "bn", "f84588fff82c9537299c6a906474fb4d7ddc3e5bce5fe44a9ea4043dfd9efd85bfef9968d61ade120623cfb11cd2dab6609a59bcde8e5729eb98a09744cecff5" },
                { "br", "0711e9eedeae2d4aeff574f7b455206b6cdcc83890bc3957aee38fe5e585f227e4fbd1ef0d66ded33de9a4db0490e8d9a0d9693c192e4fa29eea3a713f239bae" },
                { "bs", "fa2c972e530db89e1d061ba17457ac65d82b74e08a756cf52825b73301154c2f046880983f3ba84e9f08b49fe8989fa9f0af6d68f89814cc047225b8f846f0ec" },
                { "ca", "71e152fa42d3e56a6be5525d82c91f03d7794d8f7d39573ae3f5eeade742b35f92824387173cb0a56d371a53465c6484d9d6d63fd3512c94a2c911977e550c9b" },
                { "cak", "08ceee3c42c9dac677546f84d1d64411a8d0fad026691c9a96b43d180c4b6d41262b45549e64ae12db9a51ad16d1beb7a6a878ba9231116969a16db48657891c" },
                { "cs", "50b25e42b59e25beaea3e789c9878bbe618e28244114b01177eecc2f67460767a65f7d3f93aeb5df7d676b1a0a685d912d582433416e95b10ce1f21f8386c210" },
                { "cy", "9daf959886b6699a58a10c70ff7da5649cab98c3aa7aee875f41a9d1f8084ac1cbb75e36472db72fba38474bd951d97e883d3bc9f38d6563a22c9a2d180f98c4" },
                { "da", "24ef80593198b45fb81684a33cf6049ef0a063517f7b071c3f2b0170fc2536ebf9e5892369e1a63fce695493a0f00024ba3235470638e13e5cae97a31d79f285" },
                { "de", "1eeca4321cadf5a7f3baf6dff5cfd0f486fb498de58dba60c3a94ec56f6da28ada121cb319106bdf60f3cc434559c83d97ef86505b0adf51bb7c306ac49b3e6f" },
                { "dsb", "ce13794cbb90108062c5bb8efbbafaae8a67f4e5257e14f75b95570e18b908fa0778235d09afee2feaf9b821b1cefafea6c0ebf3fdf9f4e2c48012c622bea65f" },
                { "el", "ad29bf5cc0bacfaa50ec96fe8ed07028527f565238673b14b3caad88c137b75012aeee074702ba8c6ede04d7f0802c3374f86d4cf08bc7428c0f0ea9945ee61d" },
                { "en-CA", "2c849192b77ba3c9b0219bf258d0e5f80d360bf096e7838298d26e4fc73828bbb752e58fdc608d2a9a12e8167713e5428e182564d5232f89925f7b3685df9572" },
                { "en-GB", "b8faebf980801b2c79618e8014c4afafa388c37100d7fc1552e926866e8c89b42d29a366054f01d769baa39fb8fdc4e5b90e9416ee72c84463ccaa1401af2cb0" },
                { "en-US", "75488fabe4fd3c44bec7c1e57e3f2f65aab64d638ce161fa2bfbfaa00dc76ada9a6a169ce7a0694e339b4d9ba5b7289512f35d85cedc7e340c4f951893240455" },
                { "eo", "adfe648c41d26cf7014f2562fe1c9d3dd79726af15b0ee990bfa8e042d67228fef94f504c659fac09024213c0f82d4820a6b8290e4600e788829e9e8519aeb77" },
                { "es-AR", "9ac1bcf246f771e331da689af0f4f818a04fd3447e237c27a9a55e0bdbccba3f05acfd72b994e94130af2d175b39ebe78f706556e552aa529b47dcca7c9ac55b" },
                { "es-CL", "78eba7401bf34ac15a4738cb8930f9b4424ed49ff65b7348582baed590f182b01dfb30ed51ac83f87004d8200f87759b7dd5de5d4530143e8a01d12484d4402b" },
                { "es-ES", "55f59c052a38c06dd46f85044607bb3cc5552134d191a10e80f9ca01d77a1761ad2b13933023bc2fb76e4cd58a0831065e0c65ca1e64672513cfdd6c278f5a9e" },
                { "es-MX", "a2aeb31db2e4d2fa01382148dbe413d6da41737d0a104a9d34b674b0ed7f58e158f7be143c3a028768c41bdec2ff38c6e283a7b0b8fbd209af50905c0adbb051" },
                { "et", "f102c3b96ee1073002b899e8881c54d42276c4ee8444a1df6655eb554b99417a25a6453663aa57105766f84b2f68eb37d9cdd02299a470aae95acd2e446819f6" },
                { "eu", "0a4b5b9c5fa05294f904b35908296771715701d9f197948230b2b4293413a49878c6062ce0e3557de9b57435169b3016ae15fc5c6c424aa9ec105a3857e5162c" },
                { "fa", "5a4c784b3efca4de06cc07723b335c57a3f79bb603620979951d8bf2a27597f33f2106d9650ed9c9f63aee225eda0cb611ea80a00fe5d4a565d4ce9e7a986ef9" },
                { "ff", "2646559fdeb3c7b14fee2bff584b84cb210bb3d69daf204de8fe412772e9c582d3575c21029f902f7dd2cd0e183921602b1b7dd3c1c8969fb715355981bd3bcc" },
                { "fi", "c57e9638428921a7d6f1e0b54c831498bf3c93d825dc8596d75e30ce8ae8bbd743fff833060e38b0bc948718b207438ce1597fc94edf61c87e22d39183f431f9" },
                { "fr", "2a057bc21973da21eaeaecc399cd38a40ec9d790535de550cfe6735841307ca31ebedb5f822489e1fdd4777dbd2c44ce0ad90ac7d25a1560ed4ae685312b0934" },
                { "fur", "7c15186fa22671c71613dc3d651740f1981549dca66b5cec9d44ca821076316d34bd5d64b30463810ca452f7222b570f4b46acbf468d6d98ae740ac5915b285c" },
                { "fy-NL", "6a5e00db3fab899e6c175990aaf5a7482b8cf83df0a6069d8f3c4c561d5a0a324e8475bcfc12931ca1930a0a4b77c91a6b60f175b72aefcf5f3a3e8d1e3f7b12" },
                { "ga-IE", "e3479d1652170c0a91dfb1fd9044eafd272171b537a2bc8acc80f4f55fcaf3694b6363b073c0b37544d9e928732f76e9c0fc1595ad0a4b69e44ad3db76482047" },
                { "gd", "905e7a851c431ce48b78a3c53c8e54308e2d804109ce7fa0420b55e65a1e668dbde0e1dec9d33337fb0aac5d972a74a35e9ee03001bbc6073313cab1b7e15dec" },
                { "gl", "b933139edcdfe516297ac186101605a550a064c6edf0e8dbb81034ddcb6591f64800be7a0c93a6f741c015581087824b859ca53f97e4c8fde4338b88577a0896" },
                { "gn", "a3778580e285f5bf8fe57a743e9735733ad61b6897ef4a886b514a1f0e57f0aebdbdcd15672274ec670952f2b619feeda67b5df3a565ac72bff45e8e914c7a4f" },
                { "gu-IN", "9ee66f8a960cca82c890ec1d36e3265d6cd4a2f11e8f64ff7c21b7e61143edbd003d6a3c441576185b54f7ab9266ee9ad1547db2bac737f0c9c3ad3d25dc6860" },
                { "he", "0ba6b80fa398024c5b47a9c829a4b713d435da3a3e3dc5c252a0f7df04fae901eb9cb885355d3368d06cea7bf9b7f918f4494a354c6d1578b6597fb4bc784b5b" },
                { "hi-IN", "e39090dab33a8f69185eeb79101ba6d56a377ea83ec34524155f0f9fed1020661f27b5469dce24b548ebc9978d34ce72e4bfe27f347b72e4657b5572216bbd91" },
                { "hr", "8d492de08d0a88a67cb9f690b231521ef03d9838612ee076871333ba1348633c3e203308bedc6afe501d705eba7f4a705fafb26bde10beab93692004d403748d" },
                { "hsb", "781f8ae4c7a20408807c294276d51080a6dcd7dedd5b646955e8e084c4a4172e46247bd0b452ec8930cc7e88bcc6353ed057e776b43fc07786cdac86d75d2f6f" },
                { "hu", "f9772c8a5adbe1853c2af41819ea7b179101d36a3cabc9dd270b4ff3b5510e49bd996a4d35dbe154aca959b029946b8e805cef37a8c8ca1ab4d582dfe0b8719b" },
                { "hy-AM", "d9e9e4de139781ca8907e7b2cda26e1ce234bc2400c2620a8c6f7fb6a935b539aa1cb663849b65f5443e8612ac094e300d309a2051a79a4512348c4c078e68d3" },
                { "ia", "d4a3fdf8f0241bae61927ee6ac6acccc427bda15dd81d7c03bf9f37abbd25596abac45d470a95edb37a3b8553935f46954cae92ebfa112512a1ee08670ee0abe" },
                { "id", "f6b0299afee8766892f39ed9f42f287e8a77e216fecedbe89dca89a7628930afa8ee76a5bce4075e6e4ec61c2b7f533a76f53b40cf7a139ae81363fcc68be6e0" },
                { "is", "63e3c4173c46d7b16c968976684bdf6c282eead4428dc1c9fc22a70fada526ba5410adbaf625f07268d0aa3c26ecfe82f607c8df1b62c4a2f7fd066a2f04e97f" },
                { "it", "abe4c18f5e030a58d8d385712d5843bae3e628ab5a1890cc6ce1308bd0b7694031643f0246a85ff1355b4cfb9fc3ce0b8eb0890ac6d5f323e8f75a2fc527fe92" },
                { "ja", "fa6684e3588ac36a032f890195f1efc1ad034f0f438d60e84cc23236d9b828e7ba01e6f2158a1d514f31edf4c45886aeb21920bf45f663f7ca064e2dfdd832f2" },
                { "ka", "101eade9207a9056dae768eee18fc646ae1de19d3df74d2ebf8034648a20aa292e8b7d6159ed33af1106c05751df4122583c0f6c7419ed3d2ede7743f95a778f" },
                { "kab", "08d890da1e40f78f75719d19d3f4448d3ab55576bce80a68b0870dab16b1f809ccac02aa71b551c9dc6784e0abd6ae8f5a57d92befbb3e94e02a02405de200ce" },
                { "kk", "82e4e544ccd21ab33cb404a493f4594a47b406b38fe0cfead895ab9e34ccc6cefdb5c29f84636c4eee33f029e3889b01c5eb174fd5de5fc0bddf24ad9ca9a075" },
                { "km", "89887e4e10139bae4529fc341f4aa80a71d55d492677376572910468ccbef8a7b32e465c885f86151db4188b74d23bc5a038e6b953b261f5a62c47d3ca190f02" },
                { "kn", "7b4206ce34e0fe3c649ccbd3df1682d2736a9fe386c58a7d3d6eb461ec721a4256250490db51c5a2289b51900b71e98073da3b094ff0b5d7e6e34f7020f11105" },
                { "ko", "df9590e4bb67c2b720f59fe2a7bc51a6e5b26489da2f84503799aed3393948085fcc137a789f3a037788f6f8ed5eaff36b4004d4b5f3b4b4f0e8f80073894722" },
                { "lij", "b5855190f4ff11e9c4a22ebbdfa754b97a14be5d5d47d7eff29e1c8108ec8ad6b1beeb6cccb2362fe4ee29c17f44cc6569d6eded385375130dd5e1d2e225aee9" },
                { "lt", "3aa03c7bface0ea6072e3798cc467e99d49f78d013cf6cb13a465731a19aa05f8b755a7d68a25cae6f184641d987c196be4cba757d1187b3db4cfd70513fef19" },
                { "lv", "19a7f1ec0eb4e67f37bf6a4bc9e3771a2326a2b1a99d2bb4e1fe7f23d229e83dd55cc72a6311b23f6f69c90af463fc1e1f849609da621833f8a0aafccfd73284" },
                { "mk", "a6b0cf0b5ff3c031c74341b6c556469d0c9a3ffbebab53e5574f86c7974f10a68ccdc5323db113bb630c57b5309b9f79ac87c673b85507ca1079323198f07576" },
                { "mr", "3410f80e5bb8489114cde0f3e2543bdde415bdf786aece28a832775103f565a211ac87e7dbcef920b7ffb27190a90d6d7de3bf0073f61867cd3fdc967c4fb86f" },
                { "ms", "e3088e57d238c55667987eb7c1305ecc8604e7c02d16e872097b4dc4a0f29257773255cf8a12d695184b5cdb213ad1af25e78057b960fb4a2ddc21a65188f013" },
                { "my", "096a5baa590473fce4b3c2a884503c397ccd5394f1250787c93156597a606c5e73bbb1e4e632dfd92d7fbb4b192c7d9f40aa676bf679ea8e92a13a8c97e868da" },
                { "nb-NO", "e4564ec15601d5413e6f6f30d24c31d2c1ca49cf01e1a8c04c726314917a93179f54974e0409027216f527726a13124ec2c07155397e59ba128a48ed454748ae" },
                { "ne-NP", "13bae9453932b60d4e90ae6c375103d97ddc441fe16f9c87e07d6dcb915c0a8035e983968221a692b492dd631322e6b81c36f5f2b77656e15bd15258467d08ce" },
                { "nl", "9638ca3c0ac501d20ce0201ffe44fc108bd1309364621f6e513477e2e6c944cd3269289f15759a78ed4062f200a3bdaccea2b4ccf222300d832b60f53ae9943c" },
                { "nn-NO", "0302d90b6bea4a0d2caac01e5bbd550acd4035c03548bb058f2bb9050113bf6a6c311e4ff0b8f24e28959a555b7f70447807520db0e68786eb0b45bb86de7cfd" },
                { "oc", "6697451259bdd60d72509531e243730d32d5eb95af2cbcddd4783bd2532b3753f188f8999630cc5d86c997ab400eff293be444d0b85923193b76274b4327794b" },
                { "pa-IN", "9a83b432855f7686888451f9e5f6f1833983fa0a55772b3d6df9dbec942f6ca003d7f9f7627b1335436a632bb14c663dc41852e02bbd37897d435010242a8558" },
                { "pl", "8fd88a6e10cd49dffccf2b2f6e0fbe3c284790f0e936249704783fab4d46b83a41564677322dff54782730d28a079e381089b2f13859c99161e4cd76d8bb1809" },
                { "pt-BR", "2ac2f41279248af59813e9c1d40774d2e8c418ebd978e065194f9b3be4e62c158f069411204b178da2c6d9d10d1f477247c10698ba0f509fef1816bc297a0d61" },
                { "pt-PT", "f268bbafc591711a08485835c54dde8cf8a6a80fffe4195eaee61243f5ee080d9b372e48f9fc464bc28253956f2d5310269a0a72e0f410e2b87f15998c5bb049" },
                { "rm", "10e9c4d4a08df61d799acaa6fa5760bca001efb5c35ed39e33a06b3b649fb22c82578d91aa9883fea8b1152c0f26c066be5f7576dc6bfdd9260fd9d8fcd5e9f7" },
                { "ro", "27cb184a464cde6a8b23d6b5df108e7f9fbd18a7b517c9b209d1741934105f792904dc485be29ced5676dafe5866c92e40828fd5fd8b15d6636e123d9ee28236" },
                { "ru", "01b4be892f6b51fff2aadf6c35bf2809239ead3da180c30ee8db4afac916cd3a329992c57365409ee159f97307843c11727f9ecfff55ffeb5e169bd7d8be0cf5" },
                { "sat", "1c128afa0b6f67b5ab722119f740df669c5e4eb6954416851037539ee10837674e2bc82ec813174e3d4835065eb30c13dd99ad43711e498f2814c8e70bf4db50" },
                { "sc", "440c25ea0bac935fe049eaf57f19f8ac7890cf0743de3e9fe60bc0b9b820892193e8036ad32e818b6ffc6415c198373ed5b899d6d1e54e620a3c27bbabccd981" },
                { "sco", "66cf9fcca01939dd80637d9b18ea5913509c203ec2345706a7df8192dc1f92f6cafdb778fcee181bf4dbc6ffb14fde3d6621be63941000103bdcb54c3b2f3d64" },
                { "si", "778af1176617ce54148cbc5243e3705532c92fdd4e9d235296b9f3b234056248710b463630cb08205b56e38e03784f52a652498cf89fba42501c756945614795" },
                { "sk", "7c01ce1f4e71b5360c96792ebc2103aebf60b7c62e4442e9536f905c14477a4b1c10cdfa3a6bec8dbb8dab2eabd2f3060ec1c3edbca815f1dce216f6a1e86ba3" },
                { "skr", "018e1d33491afb62800e095ac3090230735d4f05d8a457f2968ce98d430ff25d9c228c0e9815d0e12809b3e3248e8042dddb0f6fd1e2507278a525039a9530c7" },
                { "sl", "ad307af54c6cd970dfdce4a22dec3b3a5710fb3ddd84609c4221a36ca58cfc6b73d678ba29b6d070af1f8e7c6ae8dc4c38f8cae097f81815d4f9f687a7c3de65" },
                { "son", "de0dda1b1401f410171702cb8ad8711d87c7db4206a8fac041cd19e348caf12ebfc0d4aafa200aa6d198775af1cca7fc0a4004ec825c25ac22519bdbf4ca146e" },
                { "sq", "9dcbd72159f9617f56a5ecbf5ea3e2513825cdad7f076b1d14149a7ef2afc38ff4298a59f0474e836e8f1356f3693f037437a536f64300c322e2de66c742214c" },
                { "sr", "1f29574f7ea4d6aa3acd8bd5c7cb9a8114bf8ca29b9ad0d5a039df692944cdb7e55c31bff9ee2a32f3dbe11f455db728c8c1eb37f6d18fbcb85f3954d30af7f4" },
                { "sv-SE", "7b1b4871873f0f559eae00c30d51e01b88cdf2ecea82019534a8fc918ad27e0a0fc957d98e714d21c5d045540b2e6e705f6a30cf34f587e9ee8bfc2e416d3dd5" },
                { "szl", "1163326ffa930c961d221b28d836e22fda72f68daeb51b3e8d6c641599cbd7c83d7474c9df6852a6b85bb0379cc529f855fb8ca36da38fd06acb9aaa3f0ba433" },
                { "ta", "8d44dcebbcf041d26eadd42ca2fc13b0407d160f2e33bf9bb3c4536520b31080cb4f56b76f8aa4e0c5a97dc5f6ba918c10e75d33619071c17e2bd404109794bc" },
                { "te", "7b055dbfc62169e36436dde6086ef9e8719af720f45516490b4a2a9461b4e1991ac88cea8e5db697c1c7c8c26f4c063c308927326d87e8876cc6f5e946b0b66b" },
                { "tg", "9d7d4e1dfaeca57804720372d5f2ace1b3dcfee2777f40282ca7aef778604f3b9bf044e253b7f6ddfcb3237fb2f9fb4266b31f02c4b80230a28afc216703ff12" },
                { "th", "acec4ef4c1d4bd7f5b67bb44f07c7702e0ead0ea069237161363a7ab44bceb14739322979736accc4919dc86e4d26ba245638ecc8c036ae1aed53e949fd0e6da" },
                { "tl", "65ff93cf8d2945630ac4fa36bc2518252f0b27fe4502fc41668a7485a96482eb8bb3501098217579b1900988c554e3238433f6e4ccce5024a0f5ab183492e428" },
                { "tr", "2a77617094d1d0a12a941af2275202b99a0fd2e49693f67bd2dc1120c9dbe690d833bacd9a9856a365b0db1ddb0334a5b7d4c69562398e1b4a2b6a89fc8b7365" },
                { "trs", "0625df2c0bf119b470728567ae9f7a9f510b8cbe247e0a18334085cf58a585d960e73d8113c1f5cb678534d4092c68a57f502b7f15b733b9546051bd178aeec0" },
                { "uk", "6971e53c307fecefd969924b041580766b73841a7eabcc5769d4ba910b23c104b001e159700e19355df8aa447bf1bcc69be5595ee713d32f3ecd02ce48fb9a39" },
                { "ur", "f0adb0b768a9e59c2a443a4fe463ccf4684c42d7fcf83482aa9f03a3c99b5055969a7b99dbb366f0ae01d2b4396e53d60fe3825144845b754f1fc527d0d7f48c" },
                { "uz", "a860b06c82ff16a3f66bef3afc32f0e57481bf1c8a7372a6d6e24c2de1830d506c8f05dda26b47caaa33aea1c7848188b34c3e562710d728a72d61d4d1752233" },
                { "vi", "4037ee9a8bc6498c584abb9c35602832b7b5cb4aeca3ab69959d0df812feae7b6f65c35cd379e52c043569ab7b0688bdc0012da1355d21e0c0ec0b1f9d155c7f" },
                { "xh", "75554d61fa93f53700b9986eba0a0cefa97bb05957adf1c745cabfc71583aca5457d6eea23d7c1feae14d4cb24000cd744ffd16954ea7180f76370ca14df5e9b" },
                { "zh-CN", "f0a810613512cce05e8c2b6cba9499b48e79bb0796711fa6732e63923862e2ede827f905024a2dc7a6999694d9f3eadc66c49cce003c769d217700b331b733d6" },
                { "zh-TW", "f18d73ecd699cb2043a8e8a207b4eebbb6fd9064fc877f2e9a4225c32f802fa4e13741cda7c51d969b6a5ecf3fe08e6c37f9f16ce3385459a5732f9f9a53dfaf" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/132.0.2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "88d24a3a156b3e83c8248ea0576e376510b1f6ad648366512df7ec39da91835b165cd4c1332fbbe536458babf6ee998d938e5e279440a784f75f133e932d9649" },
                { "af", "d1fdc2db5719765aa9efb6123293b7a00c4264e4b892538f3ea735a0c727fb9725cc25caa55c22acf77431ce05c119fe1c883a645c3e066df0329312d1a56564" },
                { "an", "a8cf07e865c0b248436bbb607e6e476ffa9c30d4f7ed63b24d7c85f5aff9e9377fbf0d57a93305a3d830313f56a9dbb7d6d8bb3babbb3e4d45945de96e24df7d" },
                { "ar", "81a7b387a472884ebd7bde2e25266bfa7aaecce7be98753db2912eb90bff8d4934d8e283ea5fa53c1f8f5ff3a0204d052be8988d6d68b6f5d323b7089ce4b0d9" },
                { "ast", "3ec8e5424e570ba3d36b5bf86c824b016595fbbf2d49e236f7a31b0ce6d380cc80776ebc9ae54ebd4db40203f11adf440adaa64b042eaecc3568582646ac79df" },
                { "az", "68935733e80cdd8c311560a2dea0b59fa2cf5f8133ad9926e19db8470fef46102dfc013b3d7b596de231066fc2fb53532cbbd7e6855ec8cf16e674a59522d748" },
                { "be", "72ad2dbf2a876fde4238cbc0aed32a0a8dbf5e9085f2d6e93f1d8a7a1060a202a8b9c148fd6a2bc76e15ace323bf21a36eff1e14826e8aea121b40b22ab9cc1b" },
                { "bg", "1697be930cf09ac0c320a4a25f917e36beae966be75858e56f9c00333d8ea42c7f8eb6ad1ff1bbc3c8d50da6098100694788a309c3d3c4d9ec5c7273a308be3c" },
                { "bn", "b0a85a1640a3d47750192d374a51d5c63f924e9ec8b1906e33f7075468ec79eb116e47f2ae9b8de2c86908558dc0f23fcfbbb8a5376a3c20636a799fcc8790a7" },
                { "br", "cab7447123f8a678761c94931cb0facb0196db8980901e81e33e372a47f9c558884bf9280d30b2a69b421f375b1606775b555fe3904d096d748833be14ec45f3" },
                { "bs", "47b8d509272f754515703ccb71b086cba24e1ef9454ac3e7bd64afcb465404ff683176b928721a91520d40b0672e5b9dff1d1e11b80beab1b7678db869822681" },
                { "ca", "4717fe2025a45aabf2b75140e897cdaabae5fc7e998d873eb5f264532aa4ff24d9e29f49cedb59aeb67a7ee639c04848769c2d90893ad168b0c2d4881dad7f00" },
                { "cak", "1e82bbbbbc0ccde198c7279469d3fb727ce7cdd94257300eca21ee3a001f23fd01409cf7bb3a637ccf7f48e81a5d1dbab34901394b8ca7921de9ef0e43052a2b" },
                { "cs", "1845b92b5e6a2ae9d0198cc07643e24936e011e9559a49b2f1e3428423ccf3236262337b0ca8da3174fecb5717473fa77b43b7874703c5fd0db48acac109a1b9" },
                { "cy", "5d91a00722bc09e2e8e7eac60401236e8dc8e53e159800f58334e57707bdabf348b8df45843fecfed47272546df222705eecf5b7ddcf02a6df69f9b897f7c670" },
                { "da", "81fef0653d1f1c5daeb71d37f96e9ec4cf150f6b16d7bac4acaab1ddd9aae0ea23d9ef47f4027b3ec82ce0444ca27e5c9fc1373ee7f1241d010ff834f94a3c73" },
                { "de", "7a3452c43f74ded377bd543b92a6e618a3047ba18d868ddccb41ccde04c30c607f465e7fbad77cd8ba5deebe655140d61a9f854b1b47743314fff0ebebd1cd10" },
                { "dsb", "55ed3d1f57cea5ed8ff63e75b0e5cca95eb3353ad6b0f2f4b47e18f32ebea3cdd26eff5535b1f17662cdeae1a4e44db3afefa8e4497b7d53e5745e374c1dd458" },
                { "el", "84b297121dc721e2a3a0aab7e65ed5b6e5e2a9948aba243e49df4ca8b9074efcd2b6e96f9cf8f1267e85ed9213b3501791289096a6f74627373805a16674fad7" },
                { "en-CA", "06654d62ff6b10ca27a19b3172f01d5ccd3400787ee7f389fc033aa84c098dde49ad34e1c1b7448f780640ce2eda2532c527dd71f21c2f998f1cea003e45b01e" },
                { "en-GB", "6a72a03bf5c4733490fcf0053b5b5846f779c72c7a781aa23deeed580506d67c52ed48a21de504437c0f9b1302112f4694ade87f209f2e3efda48b8a3e82077c" },
                { "en-US", "9f6047a0752bca24d6af1a21b0165b15694300b7d860468e9128e0b653ae53f66bf9e5421fb33cc93f852c7edc4b43f35858ca1513ceb699e3e1b8071d0605b1" },
                { "eo", "c338cfc52a5cb23b535c94f2385e81e95c41d1c5c3f7a5366a6896d03a4819f11518cf647e60836b8b5e4f5a6b2826af69f1caa7ee133c1926967c6bb067a172" },
                { "es-AR", "f63b365ab5ee88e2ea3a187f75f14eac6ea52377d417d35e7a34af53d679c917f45780eb91b8c04e89bcbd88c714592a63b453993916028a54a9217c1c4a841e" },
                { "es-CL", "4b498efd8843c2099371c90af430828dba01fe5f0755050018f555371a0f05e909ea927ed441b55b949b4ce412f5405726ae12b7af1b4e9af576736a1b600a9e" },
                { "es-ES", "22da857ef202400db80a7eb312b7f6b82802f09d58fa65f98b6bb055ab65c1578ba7bc9ba90cfc0661f1141a70e2c03d833b416cb08d98d23608109ba94a4e50" },
                { "es-MX", "b8210e2c1255f974be6bcfd02c3a485e1dfa7df36ef43853a436a42928c4611e08ead96e93afb8a0bd941aa5f2a044ac31654e788b9cb670188d72edffed3608" },
                { "et", "c6b5114ba2560f436817307621edd6384fee13aa6df058ed7248ee991561295d64923f604148029b8edbdc8c1d66226f11521d5c75074dc610bec25f7c51011a" },
                { "eu", "3743bfc47c6472ac83cc2c88d6d569cd3f417ba7ab163ff6bad2f2065f1e205bc3e732c9a4fbba64b2ef18f5e8f93a937df7d415a82a19cf54b136471809f52a" },
                { "fa", "091354f332d056fded3c96ef7d59f2cf321eb806a07e5dd611d844893f56d2ecfc27fa1ea40aa0de3e6a62919f8cfa373f0ec78f6cf952eaae47aff9e5109edc" },
                { "ff", "2e25fb3059a86110e38f96167b8f7f62b2a0e1c6411774f0c3cbee6540c95f78c770bb51e47400646617929f8ebf98c6df02c6d75e785f70b3f29f5ac4b014cf" },
                { "fi", "1ffa6d1d883155e212ab1207e9256cf6d892e46fdc088b9829ae33bc8c31bd78acf6ca1f86d26baad35fa5d355d42c1f05fa53f9b6f01c116648fea6864a03ee" },
                { "fr", "c7eeb190d245fd7b392f432e12b28b7fddd4470cde1006a122f504b81d8b7b08c57171cbfd3b2f2d1eaf8a082091c38c55a58748db90392ebfaf7ff931d7a1bf" },
                { "fur", "54f7f7bee9fbd340d8631c783e8c4c8736e6801caaed7e3a95fdf2ac3c42e7f661149eb93ff56d344aafb4ecc1d6dd231a6efddfd6bfaf7dfd845810c2d6e8f5" },
                { "fy-NL", "1288cf5b505b207545c0be35a2e9dd856bbb9089ab29a584fe6176912c522bb2fde52707f0c7f5ecfd629d3017dda60d73979147b10832ed3466de66a748fb59" },
                { "ga-IE", "7ab52b4fe4402fba47bca2a57c15daffe1bb6c4a04a534aef55d96b98321ce8c1e9d952cf263731fb90f2245e48ecdaeafb7d0dad9bd2736a84ab7d0964a0050" },
                { "gd", "5fd92fd151627f7e9990ba13b1db5693603ac6af557447b2f83abfbeff8aa2b2349a1c50e833b66e691d068d413f581d1557830f8fff25aa299a4f8edd9d84a8" },
                { "gl", "ff8322a4d85dc180cd1a791c1bff5e667d45954728422824830a7336eaea6f05d9b92e449946a483c8517eba14be9427cd350d46ef0ba450ccee332b9ad8dda0" },
                { "gn", "94c06587277244e4be4af169cfa5ba22b1d6a5607c1ea6a2771e77e6022ecedf0c2794fdf598169577e5fa090299c52f711e3bf216456b25041d8d042bf3aab1" },
                { "gu-IN", "685d281881599b863533a810cb110a608b5725fb0e3a21e6d3b85cd7c1e6cfec1158b6661c3e036bc7f849a0a4e4af24c9f9f8de5153d6e0cc2e2ecabebf40b0" },
                { "he", "84ec797a2ce2b901de00e7c6c08574657746e7b34735479fc8dad230efe99d81090763c144deb06f6658f39243abfafec9242ed01d465d844225e52951073915" },
                { "hi-IN", "b9fecc1808ee1b0ddd4a772cc68844cbc687da22dc852af9c123376dcf1d4f1e2881f39723c4584d1907b55419c458035c065a00230557d6e8e95ded1fb961ba" },
                { "hr", "9e64c635db2818a5fa4e9d0974b75387af5696f815d3bcfa90506dd2804fb89ec789da14ece6ab748338e30f25d38f919604b2559cf5cb4e79c59133ccfac6de" },
                { "hsb", "f4562aeae143ada41572d9340db774c168cf7068317bd7d8a6de61220ef67e8ac39a1781d6e28eb9deed0279dad13de02fc53600a43ef4fee02030b4dadfb020" },
                { "hu", "3a21cca3bf19dc3e0053709ee703d05bfec72b2429261b6bbc3bb0567686e40a15c2e481db5231843f1f29f22bac8bd4754313521f1605df58ab9f763528aaf9" },
                { "hy-AM", "cc0516805529d0e277bc10b58186c767008eac903306bf2563f2db1d456bcd79ae8397ad9f8452a81fd995b028e5fe2d23f96d0e2b22d0a95e2bf1258137199c" },
                { "ia", "e59129db03a5d0792806d86e765acaf401b013bf2ec92225e9d03e79558067b7893bc336a5d744cfeae51453f86cc587549594f0439988743165feaec3417ede" },
                { "id", "fc7914187dff80caf9d0d63a143eac18eea9ac6c48349b7fee97367cd8597e44689ee9d7d9c74553f67eda3fe4dd2d10d7ad6c235c834ece13688045558f882a" },
                { "is", "48ec173dfcbff1d2aacd200efda61d1c392edf4402df2e5be8cb26bb6f5c5704655c64f8c06a8dd9be29cb19e0b94d0388220e7235805e0d131134cd697bc106" },
                { "it", "4aeb9030f4570dab50465d41edede1ead5e3da37822166ce89525e5b60bc49f0de5a25c83aede42e449608c399dfe03af439ba399c61527f064495404931986d" },
                { "ja", "7f0c7562bedff8d20b880a363a4ad800213f2c1b7cab07ebfbc855b05bfd93b42df0749a88b85a46738d25250e08175b49a791fe0b069261601493fe235116a8" },
                { "ka", "d3d3c52ff9ad46e5d580ba89f922e711722fe2ec054afeebd3c56b64fb6ed8fea7f7724c29de1e361a1f8f6cb4d4d72a7ee695e37873d033217bba34b864783a" },
                { "kab", "05d97497f9895846ea8a4680e59b3b5a6c88b9d1cca45ce2886a3f0da9a2c027700f2f9def4a6cd9ed9304a214cb83c5ea9b7d1073bda05ad2a484f2468de2a1" },
                { "kk", "1b29875316f0c6ef82938fd0f6a4d93dcab45b5a80b8a76ba56322da5d159656dbb029775e1375523f47546ca51a1db7e9d30a47be73878b75485cf7a186b4af" },
                { "km", "2c13e334d87afe295204be85214ef01d6bf569dda8a741514cf00adad1061bc417edf6f8008968624c0cadd5e5bae57645c40eaa27acde18e137f2f08fffe06a" },
                { "kn", "280a095e82fbd2b26529b0156ce258915d152a30ec5951599443d92f5170ac0462267088f3c886ab0f35234a900fe1e6c9377fa70db0f2eb7d61f094acee6782" },
                { "ko", "89f03a85de5127a817ec913f334c4f1bad24bbfa33d0f0079faa08773cae4733139e7086509872b13a7814ef33f2e44f4c460e30c3dde27662e079853f5b8dcf" },
                { "lij", "f1647165bcfc9ca928a0d58fcf19b4f390d7752a6e40bf7f6521922fa0324bf198ab2bde39aa12ac199cbe44b88b553a609b67cd858292f81b6386372515c187" },
                { "lt", "e7e1eb2cd136b286c9ee826550036c03962edbc8d473ab1942d60a9c2b3557521f59825faf44c9fd04464e608b012463dd08497d528d1beae0d841ec224933bd" },
                { "lv", "360ce42323ebe68a099dedb91e8ff1799b520233e6184903dae4b2f4093de84924f0b3eb55def2d27e11839bb9a3d319627e37d214a8a83890f8eaede6847ba2" },
                { "mk", "e569f9b90ec8d4a8a99151afe0b00f7ce27e0ea3799a8f30f3f289f2dc3cf7a2686580a441dd3533cadf32943b48a83c1467cb5cb24d09bf8827a4a64180a7b3" },
                { "mr", "5e27294fca110829e2ffc4a0e16cc17068b02cb1bc5b267577a46c8f90aab744b4556260cd5c09bbc1b4dfeb2705b31a177ae7de35b0ba1f7459b4a579b3aba7" },
                { "ms", "32dc76f73d54120a7947957c18d091045cbe8f0e442fd9113974624d26e82bb710a115551927cea8854f1d11bab7aa7d1b030c6557579be89b3a243f50fe6710" },
                { "my", "79e41943c490189eef6f4e4fee5ad4a8d8904eddd6e59665117d6de3abab9f99389ac03a217ba47e68a015aabf3c18943dc6c08eab25eb4c631ed6e6bf11c8dc" },
                { "nb-NO", "992f1583076005fd8eb8e83d6aacbd07d001eb56679ab093494bcb94d63c695cf2f35103fb1502a70d55d960effd6bf4b6d6018532d8b9b674ea0ebf8c9b57eb" },
                { "ne-NP", "a97a75c1abfac1c92ea606a970108bb644793c249ec1199cb6f306dae5dfd406f7d374856426061efcc15a4ab4d1e9a683804f77793b1e1ad8750e9c767b172b" },
                { "nl", "9c711fa2bdec62cd11eb4359c5f07ec82fc5fc05b1a96d0142eb220081b45c4f0ab8aa078e352bb33fa04743ff256a027edc2d5c2ffe860af18dd51c55c3e89b" },
                { "nn-NO", "67f09e7dcf8baca74beb727a53bde3a63340250e889a61fa7a7ef870898f4bb1c5db7bc1d13ab24cbf0fcdc3a7bec81d69749e8fe342167d8dddac106260219e" },
                { "oc", "4668c0dc4b7726fde0af70f7b1d83a8dcad7684f1558dbc8da234bdaf950589f06692fb8a1a7a1042b3e4517268d4e01c392fb40b7eb3977661fad18271c4df1" },
                { "pa-IN", "3cd35edd23b8c802abb209aafcee9063b0c6539aaea403cb6412f708315b4a92c1eb6ce5e84049daa186836ac0fd18436c9336b541dac5c175981d1be77e9807" },
                { "pl", "290c587e3d8342d217b36a3601b0910b95b0de8f8f86c7f10c7538f7b520c07bbc58c594486356120cdcb3ead46770eea1e6ad1174dd4394a8fb8e601526d591" },
                { "pt-BR", "fb229158ecef25e0b3aee88edcd0198560ccabb5e1dc75b88877b558da9748ea54a7ddc2dc523f79b7cc7ed792cab671c2758b3e9a7eab0b2eab2f984bacf481" },
                { "pt-PT", "949ed9f9115a468370800941f181af50a54f6ea0290f567a98dd0d41147a983cf3055ae1ee779b5e9748d6b8c8326276bfa29752f9729737bdb98ff1c749b63d" },
                { "rm", "5b0ca2ee26ab3bfddfd9800167ce54020f14ba8a97c14add5d7d850979a4ae68c94f0cfe06350047991445a11dcc0158235afd3584d5591a8c981ace0c50d927" },
                { "ro", "37df911a28307892a5737cb6aa1cecd3509b54f27167537cd07c7e0c466985a743fe0ffe6df9eb71c75e9eea33219b58f0c08ba213232fa7c7079fde2345f0c1" },
                { "ru", "965cd37e2e5de9ef0f38d2aa691ce09a49913d616efd09229cce29a5aa14c48566cfc938a7c636ae4c6eab063c32750cf6eb4e28c909580de50dd53a96d4a26d" },
                { "sat", "cda73403e8a2ae651c947952c93ab6e48a8e88ed696f9df2022a2e06ad8d5758fb10a52e08aaf190f6432c41db30416ff2f36e2039b9e7d0ca3f83fea50516b1" },
                { "sc", "1b6d868fd78132a8b489638ea1683d41e823b370f093c79a352b3249d21e295b5626c31da46fccec0c66374b6f4a9c209d792fc747f175adb943b739a6a1fa61" },
                { "sco", "23fecc4db38657978dad7cf48ad4393fbdb6204edd86b7207cc14ff7eb9aca2bd3dca6507049b8965ce8a726b7c1dc1c3cd9a7d4e84a589ed14ae304dff66d75" },
                { "si", "cc2735758207df573ac2f7f2a78394283c06ed8263e7d4a01fca09b67ab7fa76bf17d5d3b24ea0d16af77e3cbe9803dd3a1fab956f40d1e31e0d7ef201778856" },
                { "sk", "24023a030c30d1825681f7140332e2873af271cd500a0963cb3ce2497ba3278bf17e11a8368552b75005f6f11386dcd368b57874394e5eb1b26e51967ae1700e" },
                { "skr", "bc6cc326faaaed11df4a7724ae7e5ec9667304519008457e0008dbc56cd816f7d7291568f0131a530c2c071d1c2f4cefe2228d0e050ff008092d1d60a1f4e2ff" },
                { "sl", "2181810e31bcac2d9f8d07633fb9fa06de5d4bf12335352a8275809ccb3e52c955225eff2f5cdcf15202a1a0ca3d3b9751de9236ffc63d99a2a1a626eb56655f" },
                { "son", "a5cd61921098084f0ad478a9954772bf4fb28b8bf58e6377140315686280f10ea9dc847d3cc0114f971302bd86b39b0ca02a00e5150e2c35c5930507e2b2ee42" },
                { "sq", "6635307e6dd8dc12a437c37a783c5eb50495a8c63a3c15c8f2993b4830ee81558a84524ccf2294a352c3671d015771e3416a299bfa604dc41139fab132a371c2" },
                { "sr", "741bba4460e1272cf3ae10d1924eb0c1a858ae664b3a024386ad8a060db3e6a1cbb7f36a0f34f65b19583cf0f4b6d7d70afb63621586642302db89928ffcf646" },
                { "sv-SE", "ea664a72c94216f57f00c40578e943f4627bdbad5776f0da3a99c7cbd8456a94631e40a38a8feb016e48cf5798d00c12daa94fe1a046efa67c474bdb58974a98" },
                { "szl", "da0397dc22a565af732dc60c861e22612df77dfa498051925c6f3ec0c07acb944a1c34d8fa16f5e8931bd961b8ed72e4d8c7d4d1ac81ca93ef51480177a64462" },
                { "ta", "dae50f5626f74480a86d449ea4edce22fb6e1c50f42f15ef2d71923f7b09b5d45fcaf3111f1349a52412383c0ebba1fce699160d6622b8c74a9902bcd41747ae" },
                { "te", "48c9b17117dda96ab306a5be7118795a6f3c031810656b7bbcfa244119d456ebf46ee5c39c40b1de1155445e890a59cf5c7ff34f8e017e334f2ecfe3519af95c" },
                { "tg", "8010e26a611b47eca0ff787d1176d1cafe7def24fa7f5fc9025da05a59c42b337285a2c52db17e58a8c766f8004d97a867c713b72f0d78bbb8021fdd1400c79b" },
                { "th", "6b2d49cb0c53e2994862419c75a49e6814aa30275de01e5a8d9a96b0bf5c300ef0a211e19fc6f77960d0c6c981560ff0efac746128604a3170145f8bbaafb3ce" },
                { "tl", "fab1b8276605e434f110d1bdb13535cea5c3a202ca2fe6d779f2863733c8f0d47e2226b6b5fa8a4035663ac31cb90e207d956cb458cf06dd233a16c24ad3751b" },
                { "tr", "47818d36c51523aca29ae45b0a1f4ba77ce4e46ae5daddb1fee4e98e0602ccd3176da99e21b9795b2e26295357a019e590e24e4be2626ec0f429f469f14f3bce" },
                { "trs", "84ecba8af5b6d066f69684c8fe897ca0d54527ead8b5535c1893d3f1522657de73d1906eba3908376bfb2bfe37a999acd9740985e554d8d1f058bff0942639b5" },
                { "uk", "27e6e917f0f564d686f66ea7edba9bedae8e08482666f0c1955132954d4082c9bff91e6aa61fdfc874a0f217d39e27d21e07c9512fcae7fb4bd70fb324da7eed" },
                { "ur", "c72cbab8916e19429648257e7b9e536ac4800f21782c0089d74e851913388e15c16e4c647dce5482e3fdffa76cb40567333d042a5b5fc97b5d9b97ba0d9db5e0" },
                { "uz", "36c14bb1cf008345dc9002365f8f3072aece605e0d7b5bffe277aee792346b6829140212a35b21b5fedea458546c95aa5d71e24d506eafdd796c6cbe2e2e42db" },
                { "vi", "f40285499dbe6d8b8e93a4558868ed20d14337cad003937f879aad155dc14137cb79dfa37c39caa2cb218bc0f08c3162cfdc7c3f85fe6cf5c53e4dcc84d43699" },
                { "xh", "67ac0b875efb484207151eb6fec6068742d5e0050c8dc2a685216df753ecac4d2df55a7cdc2e752918ed34a40288d5040456e0570399d2b6bc12d5623d54fead" },
                { "zh-CN", "8e574c0069b8d3ebe8e43dffa3de6a9becbfdf3681e88801d93fb81ad623490eca7852da933198e40f10bab9e249d8e3509d0ac505575fc151d768e799f03957" },
                { "zh-TW", "ca652b64636cf78141846dc633cbccc00b781f9aebc2382870c1af2332c02ab305520c8e7dc3d5e68264ead0bcbf0d989b0008e3bbdfacad65b3e1a4d41fda58" }
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
            const string knownVersion = "132.0.2";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
            return ["firefox", "firefox-" + languageCode.ToLower()];
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
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
                client = null;
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
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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

            // look for line with the correct language code and version for 32-bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return [matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128]];
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
            logger.Info("Searching for newer version of Firefox...");
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
            return [];
        }


        /// <summary>
        /// language code for the Firefox ESR version
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
    } // class
} // namespace
