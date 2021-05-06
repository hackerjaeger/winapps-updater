﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021  Dirk Stolle

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
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "89.0b8";

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
            // https://ftp.mozilla.org/pub/devedition/releases/89.0b8/SHA512SUMS
            return new Dictionary<string, string>(96)
            {
                { "ach", "b49f0e40d6e2e31241803b5edcad083b7f6fdb5d32112853a6e022433e9eb172cede43cfcbe0b57ca90f1a61aa6cbcf322bb7d8933dfea158246493aa447d214" },
                { "af", "ce379f628e303acc9bcf31751abeeb2b3fcf8f92be2698253f6fb6d11bd563a7e9bdae37c0eb6cbda3bd6cd9501395d2fd7f9a43476b9782fdfee9af12929b1c" },
                { "an", "c59021fb655bbb86df1622cc886dd3fe3976c8497a1270f9514f35d2e75c47351eb8c30a959ebd3a821b04b9de7990f11df7df458ec7f8f97cc9004ae205ca60" },
                { "ar", "df6a38186ac9dd83e471e322a8954d1092511a71a0bfee3a6f18b5d55f5791dfe3fc759b4aa2ac43cb5cbb4dbff09abb7d37156ee2eb70132500d1b4305d12dc" },
                { "ast", "d62fdd53792fd7b819485733be3b82d54d26ecd0df5e85ffd4e144f352685cfa38a32e9319cf7a13712a8ec7c1639d2876e3060e78d5e193b85c692afcfbe61c" },
                { "az", "1b9e1fd39d5bc9b110576e38cebf908bc96d3c81a5bcd924965f917be3df3c03baf0d65d38fa4166b330a7754a6476acba1e83f4213c6254215237a09a69d27b" },
                { "be", "38d916e9164970648edbcb988e82406c1eddcb1275026989016e4c9a537a74ab9297ece3d910ee11fd5c66e6f51117e589399ff257b0a6891cbd9761b650c123" },
                { "bg", "f9d72108ba80281e564660bcc1cb4dc32cbebe8e0afe9e9446ef1e3fee68fb074d1d70c168b8b7f16410508d7d190aaa0905abe934fb05994f432f284e640fd2" },
                { "bn", "b9b6b83d2bcf859b73d7a39d6cb7fbd054f5dd56cb8397741f10865c54a511c6dac4f7bfcfc7708c1586709b486b77c9ca5c8574f82e38cd1c3db86c0c2f807a" },
                { "br", "f2c1447b04c3ca575683a2183253159ee82e2ee52b7eec51799438e8eb5668c224e82a296ceaab760fa939e06141b8d8a1f6c041f46cbeb5456fc30a26610fa5" },
                { "bs", "328e5b73d15c6b7e35dcbcaaa8a5d05c8e9b8fa3788e94828ac709e148003db0e466788d590d8fa48dedbaf17f85d3d30b80693ea05929b9347afc8d366c6e6c" },
                { "ca", "147d62d298c4203d6f9a555babb7c4808bd241eecef98c5719dc744ff0240f09649fb5122f0d1a80aaee6ebc25c4477020215cc4c1994c04915efbead7f9f284" },
                { "cak", "6443d7ef84bcb01ed111e24d7c189bcd2d9cbe11b36c23ed3b18f50ffc9514a2dba726643ca8510a6b73a687b41d261d889593950fc2481515666a5385ebf078" },
                { "cs", "16d56c9032cada797c20709434683a000f91f69fc9f3124cd8890e508d9efb040dbad452a4f39002a0b9ee5609acf1278f397a5629b4d372ca653eda1d18dcf9" },
                { "cy", "aa74e8302ee66ffccdf8f05919bf6a5039e5e5ab3aaf18755e9aaf5f13eb31b9ff102cf5832a89e5c8c559f8a375d4ded33d7560e60be722758341f4d51c1aa7" },
                { "da", "100766e137e585ec023cf0d5669ed0acb50cd082f0b73d810f4f5fbfe8983c0ce8dfba73d934e3ca0ace3e2ce930386591d325ea3bf8a19731387ef06dc3797f" },
                { "de", "9ddce481abb80b3596a8b0bb84816bd962d0e269a4fe95e5d0fceb120a1a605be6de5b851f164fc055a91c29ba4475c47839de3c30e95271a9fe18da866700b8" },
                { "dsb", "c71eb0590b0b331c8ff297a739c4d1281439528fa28264fc87d4aa469d80e6186bc260a95d0da5f7120fbeb6cbc30fea899fa8d0cf53d28de250c1c1c4db032f" },
                { "el", "f6b7f90a5052fad79f316223c7f146c45802c3789dd378a4095d419e38f53300f1beba735040c1cc004328eab30c4cd0754ac7297d2b0ad9bca2bb16932e4acc" },
                { "en-CA", "5738f6b7d853f95d63a54b46d12d200b2cc75268a326d5ba0d8fd8b69725e30079d3400ad3b10bccb4583dd52321f3b094f7501bafc88965d79342a32fe92d8a" },
                { "en-GB", "1111cfe91b19001df61e04fb62fa024af40ab2174cd7d58166b1c9a45c1897e8337a8fe7aeeae5eb1f1c6dc3ada7406438de7ad210ba44ede82f0a1206d72e42" },
                { "en-US", "fd0bc87da4ea5cf4727abadba25d3303259285216abae9dda6dae4c68eb97239c0618a741647d0615b3cdb9bd9e0f853da2c16e143c5781672fbac24280747e3" },
                { "eo", "38ea5d0ffba64e00acdb0adce5a53e15c2290f789f91995b27947be771456b8281cf5fa0e9039dfc1b40d9c73f8d9ec492cd4c47dec363859bd20ad054ce21d2" },
                { "es-AR", "709b702f4612d3a925ea8a13360d23c3c62865a3a16afc6fa90ad6e454d74db8a5c9e09213e2ae6e804b3caa3933b3f05a465bbb165deb3fc1050a98b01a7cfd" },
                { "es-CL", "ac85d7f6ae83d12f3b6892060b8d4a8035abd6582c9a9d450627bbc9608c9c2de1dc1de26fcb3ae61b9bfad23fe9e02548212ed35cfef91db8e5e6f4f87bf7a8" },
                { "es-ES", "1a9a8fdb29a264cdbfb74465c0de408e866aead513af35f24491c7cb08c1797ac789741ebb83cb58f37d84d9d7667c927cc1601856189a0bd3d53c1737c46c70" },
                { "es-MX", "162bc9607cb3176417e9be4a30a394a6db40305969d7ddc0f9bc55bd95c05d1a5e5454ead8b86cabd93fce4e0bbc0639b297107e1c8bb6ebe05c7b1aecdf2407" },
                { "et", "43d8712088b7d43b6383c21c8f5ffcb900c313a0af7363f2f1c2666eaf6c81b3ef00d3616cc8ac558716d852f7c42ebb1ad1eff82851fb7b7ffb2f3444510d8c" },
                { "eu", "7abd6c04994f4e4906cdc82fe0af8d1c9eed79b04e717dd05847d96a47b8885b573c965bd8dff447ce8bf18f3f51aca4ae887061a429306fde76c71c6bbf7b7a" },
                { "fa", "a47c1d6acfc466139125909f56387c626919fe7d2a35fb2bea7303426372774dfce0babfd3a418ea52e9f8dc74c084c56227086ca1fbf8588b491d2081dd135e" },
                { "ff", "18f746c60f15f1948d322539f273f4f0fd87d05ce029f98063342b08237bf75334ae065de968b4149cee6205f89434ac1b9b803938a56e95e567cfd8d849f7ff" },
                { "fi", "a390102ffd11eb89c4588f8c6e9121d00ebd2fbef21ea96e7a732d68a3a89b84a02c19ccf586555ee44a50a1116d7f42129bdf3939858311f4e60960ae643a76" },
                { "fr", "652cfa9fbe09f1bf630743472372bfb1a97f43aae9e83d103b719b95e0a74600e4969534ee824496e4ec09e04554ce32742b3bd3f9f0a941ba880c280a852c51" },
                { "fy-NL", "034e60bfcd0329d7c6573693670ceef8fac48f2c26c2296097e0bde542ca3e31798ec8ad55406fabeb2be6b22542aa80348430d9c9620c81ef0144ed50a41108" },
                { "ga-IE", "2b7fbf2e802403b1f2829f5c16a8a71e6fd360135dc21be039110b3737066ebe66e52bab0d4452dbc541dcd2652c3e2fc7b2ecd178d644dbc09ae429714bef3d" },
                { "gd", "82b4a53b51dd4abb66bdea76514c8cc68e00dc762442fb713510055d491965ea9f5af5e814da1ac4a1d62f40f2a6c1889c78bd86f5161c6a76ba26defc0fa4a8" },
                { "gl", "2228059ed70d8ca2fe4f0ffc323d4a898f331cbcdd75ac3a7fdb92b8b465ffaff26f65f52234386b4a3b37284d33ad3e176a511cd5140a45069fbfadbf46576b" },
                { "gn", "d8cf95f0955b9ccb80e67ee940b7a6fbaaadc45197ab3e64d2b3200bee1b462e9de728501a6a39b154ec3b41eae8698fc09f316535d2616508122e473b01751d" },
                { "gu-IN", "f0513bc46d21c913901e3cd470835f9dc8d56162204a539527f8f0816782438a3e7cbe9d11cbf9a39b7ba2f3981981ef817c29c29f008624e41c5999944b31a2" },
                { "he", "c077175aad02099c5a295bf797e8d830da7c51e5c121b33e7e5a44f407ecc2802efeb07087f243d887d5ff83970e25762c550155cf57888cc20fbfa76e8e2205" },
                { "hi-IN", "77f48fe5f0f04a20acbfb4c0c0dfb663866d7d201e809bcc85266616802471d35ececb9a91fda6cb22c437658fd2f63732ed1911df5aa9aff9508cf49c67585c" },
                { "hr", "5bb66cfffcb814e5b08fe28fb45abd90698e8e2808b567d56b705f979f27af2b277ad31ed09b7498fc4154434aeb81dddbb42e5f790256f012b054a2fd26cb4a" },
                { "hsb", "9c2ecccc78ae438f0a7e4f645bf39fa719883c7b6ba7d530026fb3195d653db0c23bd68eaab3f6e8e3ad2667765e09addb5e795e2892cc61ba6c927c4b1261c1" },
                { "hu", "68608c0181ce805a6e9aee0bb35bdee20aa37549dd9f6088b078ddbcf6f98ca669c7130e46e3b1ea6b270143dd7919b7aeb1155000b173559fb09c0c8f31b7af" },
                { "hy-AM", "0d5060639b34d4ca0f5d0ff8d0375ed84b92a75fab4d2cc276b516f93b7a7c7d517b373da1286703d221fa5e89eb4c67cfd63f06a6f496d79cba9cd1f922f2f2" },
                { "ia", "64e44a95631886576a7a61749e229ea0bc2f81ce9b2a2509b9790d09f575a1ca0d7a72672d28f03b4d46c3eb0e50b34a1783469ef7d6afef4f40b201f7b0391d" },
                { "id", "5e52d0f6597382748049ce1076b15c74a1839fd4be801af69b893e05d9262794d1f53d2168cefecd0f3940e630ee2e2e849776c0c19dd07bea615459d52031df" },
                { "is", "ce1756b1ea5da4c69cb6d7d4a79248806b9374060bdb7f7f42cff366b006f126b952475718220f8e38935a506cb19257087822d1789b9d4ac1ee061d0a332f4e" },
                { "it", "cf34aaa67209b77314ab162bdfb19a67cfa55d52cc2a1d0eea2706b58291f5ca0e8775e9a05bce682faf56106db01a4bab97155460d3f2b4f1b4280ffd62bf89" },
                { "ja", "e46b90764083f59881570574be8e38d2da08107a6ebd1081f17634967652e0b059dcd072b2b0e336a64dde5936f0d86b8b21ec7f19c1a7bdff50dfea7ecebf4d" },
                { "ka", "ac6215a6e41ec543a30d9ee006b05201b7511e454615b9ce1cd941ee5bd53a0b6d45bad47e3417046deb17611063fd34342e42c9474b35ce7b04a0b69c7c24b7" },
                { "kab", "da7f2ec81eb03f33f04780180232b79fc9bc727c2f9eccd73fe2031e9c70186e19306bff91efab0403b6ac900572336e236ce412a389d0c857f344f9e5e52d72" },
                { "kk", "1c36040f82340af0e6b6b171479a02af6623b2951d35bbd6c8e65a0fad250b46518c09c1748d9a9d47d6ba49cfdeb3276337fc200e604f04555c029584b90cb0" },
                { "km", "62f045af8d64dd8ce7b497ec000544cf8abc7568554c702bd2863400bb8202148c197f894b26bf9e0545afbfe2e4408d97227b80fe38f56cce0b27296fad65b3" },
                { "kn", "5dae52cfb836505b204fa417b81f5257f078d30b6818436387872082401504045f9c4999154b91de9443cd4ea44a090caefddac97c2df542b8fa068bf4888ce6" },
                { "ko", "29ee2c48c85f5bec42ed768d58843fa2af74d97b411f8b6f5ad91fc0b0130bab0e77ee6dea8fb8a976ddc024edd58c9bdc6ca2946548ad8519b608d5f6c457fe" },
                { "lij", "1d5d596293ab7917c92d91482acbb835cb8265d916333cf262be4b205afdcb946a7eaaede09562273e747d8de3268d8bb36ec16b3d8ab84dae5e44384c7d433d" },
                { "lt", "a77655c31d6b5d1a6e6c80980cbc724d8c17a8cadc56396d408ac29b3e4709a56b434c0c41a5dff488a6bc40f03eb05b6cbe8a9df501a06dc3f7e1f2e2f5280f" },
                { "lv", "79c912757b7bbe70f2d64f32db292c5bd3b4e398ea29abc519d267d6652586368b904d9e26143efacff8b7b4d379ac9efaeb2427223e9a74885bb8542b60a1e2" },
                { "mk", "c9c25fb7a08ab9464ee3935d508702acc0363002a891bd084144757de667d0c056fc75d399defbbb90f7e94458642e3118ce347a5e8540ed1fd66c65eee3b778" },
                { "mr", "9b42a4fe7507504895c0e5dc9c5c9c67398018ad60903371bc27ca6ac4af19464ab0080ca164145894e15aee1b274fa910c15f5238d1525886e895cbe80acbe2" },
                { "ms", "55b9423c9e5c5120e41bbd90f8cbbd73dfe86879f4c93b1e8198ba6b5420eee47c5e92d155fe3eccbd58bf9ad413ae11db9222eb5332ca657c921ed453860eb8" },
                { "my", "813f469fb66ec116d7e8e9d6463a237bc3c298e6ab60507dd7e0e1850f31c0d332f78a941fc749eb40fbb523911193350760942af163c0fe9e041436a751331e" },
                { "nb-NO", "e47f001e191018b64411619015a721cdeb51a0dfb7c38485e5a536a3c8c4813f1ba64942911e50e1219c27a197868a42aec3e53845ceca8cdf9ebfd3fe7aff8c" },
                { "ne-NP", "eda52ebbbf53802030e0b8b0e0c91f39bc5e7f3b5f2785ef40bc72f31e5d8a600f4ac949904e66bedf98edda97fd540da16072d1b03b234f44bdc4c2d63d57a7" },
                { "nl", "8e99172408b7816002615377d297fd18f551ab7f7b56010a95321c5ffe8700a15c8789c299ade469e64bd0a46d206bb24cc1af8204d0d36a078019470fee8c49" },
                { "nn-NO", "88a0103b9154458fd901794fe9c7f6471266d14299882d08ce79d7742d7d6966fedc12f2bf169f586b42a65a886f9f18fc720fcae2b005d84e1f5e6b803fa09d" },
                { "oc", "6534055ab98c714025239ff8ebe45e004e48091246cd5c6e2ec33f42ad562c779b2e06d74f72b19e918db2056f80f71e2ddc9492f562f72cc5378934fde55190" },
                { "pa-IN", "66a3ae923ba746fa6d5c94167bdab2155bad28059966d3ec2e36dc0eca508c95c8189cf9020379cb407aada2538b88920ded45a545a265b489204d505b435cc2" },
                { "pl", "ff229d97251f2c85b05fe8070040341a631a5cb0c46f16f514cf204a657fef14004e43462feea8185d07bb4da02edf085bbb9bae238852a77a064b3601b8a23d" },
                { "pt-BR", "dda965a63cce1aaaac1952a3b47d078c9a0cfbe274e7717b7b95ff338f9bdbf0d1f0cf58668bc148b7c4638a6772b5d2332384dd88f08f98857dbf2e0c454525" },
                { "pt-PT", "d8af25d0a9326ea569266937380fc3da1e354b4b4ecdc37b703f185903dd3e933780fc0a68f09dda31ebac98e806cc99aee9d05d289377d58949f184ba6b2f4a" },
                { "rm", "104a099f4df17a04e9bae4247f63533ce477da7fc4a406039c81a104d263f11ba24d74bdcb7f5762bd71848f17a21d94272040172908bf31e941c83e894f8527" },
                { "ro", "46d96211fb39062c7e3207d08ac2700c7dd6229016de3fb7869eff5be26ef458ffb54e86d1c2913f8ed309cf7a45fb21012756af6b3027604bce7d230a6f9776" },
                { "ru", "26689a5a324518659fd624a6d7de2fad8d66d76e7508ee6711e49ea8e401da1ecfd6815c5112dc8e5b29200f35130324428f927d9846350cf693ca0858dd11ab" },
                { "si", "55c0e0c58ec0bdcf8f9ddfa81fb9db80b3bea431a5fc2d3c5bf8e23f50c40e829cf153ca5c9576476738a61da3fc303e602f03e5dca829c25c95208590a3246b" },
                { "sk", "a3b7097cd932fe98b42ae1fff78bc55fcee84076ebdb6ab79b52e4a883763e518da1c167046bdaaa25f49d42fe37ac975550bd949e789b5fe88e65f11634681d" },
                { "sl", "b9a547267f361393540a061065642c729df798ddac135b65db25a87ccfee38e36a4805e583e0c0ddef39b0dc0c0b99549afd3253f20a2c83353f4ca1d932bce8" },
                { "son", "2baec7e702aaf103f4b83db998e7ab88d8fe532b723e85fea9ea7eb5c3660fbf77b18b9a6f73d9c5fb30a150a4ce9645b17672275e9f44292907004e83fc4243" },
                { "sq", "cbbc8b8605dc8315bab0344315b82caa8c7ca1a135a767a480e198aef44022118f4574773a724a6a3f4b3b512b2901a522a5927f7b60d56f9fe49c56c71f7bc0" },
                { "sr", "925fff7f09ff42528f9d9aeea4ec011e1766f301b6c2d3f1894ffd165d8425a3ee44939696c93eb5fa373de67968285bb4b6d75d76b32b791a752e1920629dc7" },
                { "sv-SE", "2ebef7f27e41a5a1f5c9f92434bc18f192c91665dad27692a25564b06203487ce42dbf5c3b91738554f8d2d433c64bbb481921f22b977a28e827a091e32a93e2" },
                { "szl", "b9a88bfe6ca05085eef638f0b3609c7bc76d9e777e2d6d8b165fb08e1ceddb213e69d0a9233c0810478bdc4bee43282d1a99079133d6fb90b9669c157d268763" },
                { "ta", "bfc11143757b713e09f633cf0025b08a3510d9deaf88baf7a2c914b47dbe1c519743a3cdb182865e2d083ef619dba189059372fe39c14601ce5726cff2196b12" },
                { "te", "65d06dc4ee82cb03a12106e708d3f86b786d39212788b97c4a03f62b789aa3f462072c186eaff6801c682954c1a1e0aff46848727d9586183e19377dd2b686f8" },
                { "th", "6843241cfab595c29eb10645c6cf7c2f8ef782add0830fcf2d9ec4802f9e48ecce83d6757b102c2b85fd60afce8914bc65ee2621095b9893d8c1f1ee1ae99730" },
                { "tl", "e5fc3270e48e2be174bd467c8a36e94cf48b0cc39d61452ba241714348f9a03208b7a3e88f66e1874dcb899189e0c307bfd41fc1d9aece95393bbf7dcd9fb32d" },
                { "tr", "1dd065a08e6062d3433cfa54bf6f1e2451bdf03421b326641fdbabe63942d907be2202083031f4e015c32abbb61c9c9d334c92427bdb9285fa3907e60a85f003" },
                { "trs", "5df47a17eb852a9124b7517d9ddabb78102a4d838738bb4aca53ea29b15dc1d84ab0c16080c664134536a10bb22ed041efde545060910c2aeac7d5e9d02817a2" },
                { "uk", "be71eefec1dd2198c708a0c3ec694edef1a285faf07f0fe88af814fbb4d63b5155882c49916796859770fcefcaec5d163fcd99d5f33b5a8aba1748b1d17f780f" },
                { "ur", "67a085b411cf8745970cca046b1378718b60141a2dd6361e1715ecbbbfd549c49d87bc3960ceace3099b8eaa54e2f0f8b9f4f7c7dbe1fcc568d01f3dd8fec211" },
                { "uz", "517266654a61e0abb1840be2495a7e4066270f2f293931b34de02786aab0d12ae726e7ef80b7bf0e370bb80cfafd36be2e2ecc3e9eeb3704830ef2768cfebdc6" },
                { "vi", "1a351495654592440c5f70e575b0871c5425ec6aea1229d7e472cd4b68676fc83c1d35a3169d7cb2b742f3e57b05f1dcecc3183d38914df238e130faf270ac32" },
                { "xh", "187e3b0baf2020d4d48ff5558d8e719565b0ddc3cb119ee9d0e52d12d7e9919319ebdc4aa7f1a0b4c8dc516f4f362473f7d5e524d1a2ddbf793a292fbca56228" },
                { "zh-CN", "950b6e6a3385a37a58cfd7c8fa49c990bf6784802b605439dfc3e5e85c4f171cedefa3aa91293bba21b266579a6f6fdc63e88d321705455b6d2cac01daeef13a" },
                { "zh-TW", "893090ab4b095e4a8e5e0e818cdfb9add1291a9490cd662daa778fb0d818dcd70083c541dd59d42e6cd053bce75c1d4344df601daec3bc5495fcfa8084e7453a" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/89.0b8/SHA512SUMS
            return new Dictionary<string, string>(96)
            {
                { "ach", "16ee8bb14a556f0837cf6e2a26e9a0e83681d24dc33a3929f32aa5d93be00f110ee74d13c4f28aff6fb837cc7f63e0b315151eb894b5f98e5dce5296be088ba3" },
                { "af", "d159fa42f4b02678cf8c992c3234e1db6d1290c6a7b3e806a43709ef7e02b0f3f11f0dc10af1262ba4f28f7e1aea8087b1f0084b4f9fb5e5a18243c6da7181c0" },
                { "an", "1612a526899978ce0736e491a41e2e9000ae8429ba919a17c48de65114a4c2fb6e1d7ac23c278090f614fd1049088ce9ca6ab1bfb38a092704ab97a5fa94d406" },
                { "ar", "9ce2aac0ac33eb38a1ceec4739c16d53ccc3215d8d046462547e1fe7db573fa5215ce1601c7df57af17e7bd6a2094c9b6eff02f83258b51843a9159ad55c30c1" },
                { "ast", "a51453c7e2e81d04a0e221fed777a7b8c4158fd7654add60d02f78628d970d8a4dc53d6aaf05eea4a299045b900cbe58e7ca1fe5f65c98267eaa3b7e06a6fcf0" },
                { "az", "1e5393274d15de934dcf0757c4b5430bcedc5e60e52c85d1dd45802a9d71baa2b5ff734f444e72f9f17930bed4be31913344d914c58a36ac076867e48c44f43c" },
                { "be", "664bff4856e5e2b897307dd191f8ebf296b8956325f5ceba518b48b4f4f751a8ea9b0573e3a905d92aabc6b3b511ca3acd59a4254064c8eab9d8ff9057feaa49" },
                { "bg", "fe4ccf9f5ab130bc12c080c0f2088f2dccd4e4dd1fe8d09325714f76d9b7ef907f8aff2ff3d819ad20e7702218776e0e5f5730bc9ef8af48830b1944a625161b" },
                { "bn", "5394fde72671c47e240448445596c29cf4261815b436df62318b9cca92e357cc2d69b5d200c90f166ebbcb4a6c09b52be3ecedbd01a9e3b73b7b034bbd85daba" },
                { "br", "2ad16a1c69fa920d7c60076563595502c7119585c1cf672b6b3ed5633d480571432f87cb44e14790176505d7c357e7b303db43001da0670bae035180850c0514" },
                { "bs", "472f1480e1714dcf68668ae1b36dba9d3c30474b47bf2b89b6fa99898eb20f18953ffb69d8cf3d850aae0dce86b9aba4545955d70b9855ccb88d40982f19e668" },
                { "ca", "140dbd987739a0f5a63906741627bbdbd8efdac170eee83c9803c7f341a2140f6868d6ffc5d2e386fb6879f2c3a8e771998af4f1571ead1cbbae811809e5c570" },
                { "cak", "39e0819f88b90d8aa1db38e0fb6893e061434265ae31beab50dceaef88cb527ef13023f91b6ed9b33696a2de008c8e539c93d8434b75201e2e93e7b3a4b665d0" },
                { "cs", "c6f63c33cc6f86219cac4b572c17df391d31ca0445a72117f1568d8433035328a5cdefbbdf62f7adcb0f6c9d2e50ff5a355623bab432e3fffe8f79c44ed22b89" },
                { "cy", "03b53040a3ece3db653bf69e06e37d0c7ab6e5cf68faac9e7fc1d55761e2d3599d48f49eed648a72bed9b259b6e1d96cb7a5f5f0a9377270b8dcded9f114bb1b" },
                { "da", "8808cb69ac2de47e67cfce1783a037cd3d87cb3d61565f459e143658332f46e1c98acb0847e79ce18412c7cb526ba319d6571fbb02e6add38a716d9fbf717a85" },
                { "de", "9732befb0a7bec0e21ae3b0cd0d95d7f9161f8b22616745e64d640295e23dcd447cc1532218dd5b0e363d05ceb48c5337a79473996f63aee23c2bcaa3d446aec" },
                { "dsb", "c33ab5a5379278f5008347a9c520144248300659fbf96f99ab03105285b3d0744b9178657126ac8c7c63485fe59af2e7b40be5ed6919351e9eb8982505a34df5" },
                { "el", "dc7894b3fe9203b9e79f9e8bf2a38eaf85148579d9b8535f7b946e7e81cbe97d822551d5a56f380b513455d1a29eca90d7e9719d1e6745bdadc7c598b0d34842" },
                { "en-CA", "4d957727555f73a149e029c64d357f9b596c2d26846d9fc38ee1f8e3cd4e7e8dc5fa99ab4de081ab7e38c24bc3e756ef8f102ff94b484a6489ee0dfe92113f03" },
                { "en-GB", "eabebce46eaf441feba8b4e281b6db00d5ec3bf8abcec889f2ea71c183cd623dd723d2c6f3373b0f1df11ec1a5b464a930dfbbfb1ec77840eb16272fbb025d96" },
                { "en-US", "7fd50c94efb7028a0e8f088aa887e4bedbe67d273ac8758bff5bb1ddbddcf52900daf629523f6edd930e0f9f5f3b5ef9f56c855037a133111bc0df21671a9261" },
                { "eo", "946744ea78082945f434ded88cdbcdc8a09f02f479d217faaf9e7e873e5c6cc966850a35bc1805400191e46a67070ddaa3b9b781263010c3d184a56cdba77e13" },
                { "es-AR", "c172af9d7ef1fe69915c034e400abcebcd564c9a83a4f0922359a3b3f63bb42ab6e3dee86b91eef38df157bc37513767fcf5ac6c3dafba1e6cca65e9d8b6d4f8" },
                { "es-CL", "fef3191b04f183414736d1ed78c8c1c4398ae11c36919e9e8f5b75d9f88d2209e049eb15cd58e1bf4e1e5e85a260947528e94849cf01010dc20097363aae2eff" },
                { "es-ES", "26b55870f1437ba8436fa08e514eb889c353ebf8db13399c441d25e286a20c49e5e4c85646e07329495ae94bad232794ac114fdcab4dfd1f9f0a7b75dff395ed" },
                { "es-MX", "33b326c37421f391c01a148ed063a0dfee6559a108f44624ec3ef7ab2b9a5764eb319aeff54a45e03042ab38a0208a226d3f75b6921f0c83b1ea9b1d212d3fd4" },
                { "et", "3c7521f902dd02422df184bfe86ecd849b72c43def92dbdd77c245a00d4a708259ea043442dff33b552ae8d524854094cd4a7a33537febba64a897ebd4f5ec0d" },
                { "eu", "526e173eb49aa7eb17fdf77dae23a65727004e285011cd9b09a29f1cf721f93c80f76449e290d9b6bf6e96d691e7a0d3d921ba674701c3bb85d16b3f336bb860" },
                { "fa", "fef801822e670284786d975c40ce5fbfbe3f2b4452a25a074d8a93e022d0ff77155f76be475d781d3690c5c1eb7704c88aeb327705c1d5d90e5b55355fb801b2" },
                { "ff", "0b61e7fbb0ee81c28a1832278f2363b779bb9edd859da38d432caec904650760c3c5859a64967da852abaf278853645c32f77e75dc3a57008746ddcfd05d4aa5" },
                { "fi", "973c8780e25a537c46f0f37244438fd5fd3474d6c4ab9c37874e1ede1087db2140ca71750050d663f442ca6a2b301851af0f5fd5be58c08c857f8aa30c660485" },
                { "fr", "4eba1feaf97563578c1881e54f278a2d299dafa16f8f3cc146724a6f394f04dec9e087bb2f9da1fbc21518cc7c904a333d467b9cf0095073a60546bec6442487" },
                { "fy-NL", "03cb91d75b0d5bf7e6a8b6c72fab90913d4dc2743f0145b0b7d9e5ca5d2de91ef82abc6cd293e3fec8bb92ccf89a0ac12484ee8c50acb8a7e20f3cb843641337" },
                { "ga-IE", "d9ebf23eb550dee18e5ea766177076bdf1abc160c2bf6bcaa84811b443e2ad83432f826a1d08d00e27b9227263d735201df297826e521aeb2d8a5c05018242f9" },
                { "gd", "a789bb293f18e79907d42032720bc60b361c5f20cf6337aa71f8db5f11b62db1dd78c2b163ef06ea7b70757627130cdea48b8464aef476b706215df9cabbef0d" },
                { "gl", "9709f93e3b247ba5b50f35d258732807348cac2d331724c6c95c02f7cfa1f03a5aed0263aa613d70861c7e1347d1e2ab641b6e8f9c77cc64edb7ec01dee8b2f1" },
                { "gn", "8e709c973cff2180f5b97afeeac18576c73097c607e93c41685c73375820a85b95c58a924be425902afbbcdb15a2bf1368dfb62cecd293261057f530868d9553" },
                { "gu-IN", "251a829907f43643d35a606d58bf93cf034b91a4bdc97205dee8a5f2251ff968c408d71237e5431e648a05983859c797b39f6c704f0a57da2c490ea42ea07361" },
                { "he", "a0156e0d8b2d259dc32cf71da6c922a3078e1e813a645fac2abbdb54e5dde373d69b89591a0b0fc805e4099df527f301fd4b70fdddb93da46203ea47d63b54cc" },
                { "hi-IN", "56c1f0ac78ad0eb418e727b17270dac26de6dd717d930bd9d3e260bebccc694bc5dc8bc9f9ecbc99cec2bf67701cd7c09b92e3c0866d1ab2dc16a54d45bfd461" },
                { "hr", "536d5d5e0067fae79454323dc2eb90897260a9e9980d7dd9f5b46c487269bf2f9f68c4e78a730f30c0e10066f70620c9365df4cf95c3166d09675d7e570d1955" },
                { "hsb", "6d969bebd3aef88b8f734a2fc409125af7fd0972030e29d8270cb73440a2214d1534f023d83957961f0cffe732fd713aef1b24f11cc465a0420fb34f3647d727" },
                { "hu", "725ac044918160e405ce09ded92a7e28aaa79fa6f268900e89305f9f4c14d68ecf4a10802f8bdfef2736fdb8a2bf815d4df6cd33166ffd33ba721cbfa7e83554" },
                { "hy-AM", "34b5bb0965e8e5685094c673c2a58d7b5a3aa4987be2ad00ac32b105ae79eb70333bc795e635133878bedf218c5e127d2fbf0d99921def9a4f86188ef01c323c" },
                { "ia", "183fc462a7551da5250a8e5850838f193e3027437b739a7f5c3be8e365c1efd7daebdeb10cf60f8a77a3db33bfa76ae6cb812e8eaec81123be0e5f1f82ba8e0f" },
                { "id", "83e2612d0d7266e7d4d17c5d74c38f909f3fbac7d551f64c5525f2b43b417d7f04d95ee5c16b487612680cbda6d5fa065ba745c78ce481ee34197c99bce9c9cf" },
                { "is", "650ab0e62d0808678a5cf25005f7a23d9933e8a2b7d3e197989e3fad1e003d8b8f782797455b5c823b6cf1b67323fcee4725de6b0d3ab3fa0ae663c3a7f435a3" },
                { "it", "2211a5d5b8b2c6cdd85a424c310aecb1f68d99a68ec367c74e05b82699c81253e46f09aafe4834539cd895fa636764e46bf8b98e610bb2d39326e6d6b0e5800f" },
                { "ja", "c792b429cee797a641b1b1099236144fc6759364faffb056b4b142444cd1d772aad8604856638a9d49e0a8eea93f933f70db06ed274913df90ba17bd1ebb868c" },
                { "ka", "18b831361dc5428b7f72c448fbc8de6f163c5f3e70ac240705d404386500ef62d1fe8ace95acd4435db7e5faafa1948e547d23823834148324b98c2505084c03" },
                { "kab", "828d0cde0d0cb4dc830dbecce9f5f8b38bf0332e462597a819959950744ca75dff85728f465831b14791aa37de222741d9ab491ac95c9d03b8bdf2abd4c70433" },
                { "kk", "b78ebbc671b92e96556f4eb7c3af4565fba1611049e8e1cc9922f8342a197ff34f885cf4404a865f1a97c67c55e0bd2b69cb7a00e54aecb1bf223ea609479ed0" },
                { "km", "0faffde63708367fdd4b0c0cfe6b33a6ab1e8b9ce84c0e08fc010478ae04292e30959f1ab6bf8dcc8c6ab72af4d91c32a6d4e90e37d33b397f0c16b51ed7e886" },
                { "kn", "7c173b4089c0cb1ae0ea24f871f88007a885b5636cbb93c6967602bd107be4ec6c76099ffb6d72f1ee415b873fd81b87341cdd4cd8da46feafc13ed05096fe32" },
                { "ko", "f97d680046f7058250cdcb6146b12a428b9853edc1cf4d26cd79742ec26dd2c0fae03bd894ab1c366a098a7f0016c389f18cd5acf35a3ef72d3f00f2b94e90a5" },
                { "lij", "60ec87b56c4ed466d5ca3f6f3bf6b9a6d426c88117b0f72f10ce44f36910e46c29b20f9b91d32188b9ed10d63e071ab52f3988d3634c7ab7a85aea118fa59032" },
                { "lt", "ab444a391eb700dbc273a2343b6260f9f33f6bcdb7fcc0cf59f9f78a8b5507a6f17276683552223d65d8f3c1e36118b8e68c8952e5cae32668454d34761f46c9" },
                { "lv", "b588e75bcec007875870926238714ae19f124d4bc88e18ddde8dd799590f86c48700b13cc4c43224011e80f69964722828497bb69b2a0abe8002024f82533f2a" },
                { "mk", "5afaf98a0fc1ecaf174105e74a7bda7060fed4a6ce2a1c3f07bdc2344abee4273c3746e96006d431c2bbb8c12c337a28c33068e32dabfee02724fecb6f0db96d" },
                { "mr", "86c31ee06c72bfc135f83921cf600d105a428a52e4595271deab3006f018abf8eeae253b31c5308d4dad3e35e55ec7f7922f6f54b0170e2db58b4e596ade902d" },
                { "ms", "1aa76b8d26072de226b7292eeab261850bfdb77ae002d80988cc3c4a332d7c0d8460bfe6d987576d307551bff85f291fe9f0cdf9b76187255ad16f1b406465a6" },
                { "my", "391abe0b4746707b1f42b0e747bc186977165f5611e4175f8d610b9c1dc8cca7bfcbdd10e7c1c7c6c77c117943a133ce501c6b0aacccb732e87324025553eb93" },
                { "nb-NO", "ff95381cdeb30eec9901945aa8c67410187c39a7e7283f89a9d11b0ef5ee277b31ab14736a51ccfd792706111b14c1c7edd1de9c420679e776112f7b81b89dd6" },
                { "ne-NP", "8614847f5d3da00f037d6a5fcce47d98a5887586cd48f2202d4dc1b9d293aea29730c1cd9206deaebb0816e3f6a600b97d9e90d766dd66a6b981320736deb5ee" },
                { "nl", "69c592ff1d5bacf9d7ef90b24e17ae0280b9b340de6a7cdae38b8eb10afb56334befd7d79ccbba552630afe9b18faa15745918736892fb431bd80991291bbe23" },
                { "nn-NO", "5211cd950eeced7a1c7262b612f3fdc7ad1a313313ca44ce699b80f9689b8743985d9f6a965f3ca698d6866f50c1ae2712f7b2d9a4a72bfec22c3a4fae999a74" },
                { "oc", "45d92e5162cdecc234598f1437638ba8208d4800e382f12a5054f512b65d1f4e0f8fa133b9af89e9a19548a8ef758f95b8e4c1eb90d8c65249fccbc60c951d0b" },
                { "pa-IN", "0fa3a89f4234388cc8a551da59127281eb3837ef2492e5e21522eb572c1705baa2c7dbb29878bbedb31d29af908c18e34d4d2e45c3557d1fe762ea9fdd86e752" },
                { "pl", "06b3bba9cc7fff4699b134c15a7e2db621fc78f7b8c8648ca523fb65d7e66692aa77bab803d83a543169643906f3912013225aa3938490b533d940f5de1909d1" },
                { "pt-BR", "77bca142fb6d36d002053cd54c057dedc4a2bce5ca541fdc3291447993ef0867e123e2e44387978a2f290c7abc8b6a51b11142df624945c3adf08867fc2c5910" },
                { "pt-PT", "d6e9e9fc742b6931455116bc34fbe60c5c8e72bbfdc189c4afc00761e961ef8e5d5dbebacafd76cced95e19d494328af0e909555d48a7aebe1cca8108f860c66" },
                { "rm", "60f5f1ac9e09551387747884e884923cd198d0b8fc2ce8a80aebb9bbbc2b7129bbacbd81b1cfc9bc40ad36592fc59ea8993326a79a583e8e5a578303ddc34555" },
                { "ro", "f932ee0a8ac95725627cb82b5794bc526523e863deae838075a7df4c7ac9ed39d1c46a2603af83b37684e2f8c57962fc4a3b31c1fc859c3abd4d1bea716f8475" },
                { "ru", "25e5082333dd7c0b625fd9c3031aa0ec9615717174dcc6ecd2f2e64848fb0a31d3cf83e7f92fd0db1cbf8fcc488361a607d16c352baf83057659b655abde599d" },
                { "si", "d2b951db45f1b28cb1fe8a2f93066e918744864d8affd6f866de3934d0d2aa35ebefb7c47dfd21f9fae9d8a5da25ce4418bef98c7ddc3def36e25b32df011188" },
                { "sk", "0ac116d2f8b3953ad6365fd29ceb1da0a443a2f0bafd8647ade61b7159baf71066f512a394acf578285ec31c631da1709fbf755bffcfa910ea2ea2fafeff3865" },
                { "sl", "58ea5030c2b982fd7b51f797e27bffe7e03a7f8e3e68c16e1e56229078121598a3bf93e64a9f61e9b818306e9e38c485469466bf41d1132bcdfaa83f33915c12" },
                { "son", "a1119448d0fdbf0500aaf0c69678d5c23733e095d604b14ca672227b4754323adf3848a2c0a48ae1fd050231205b741814bd0019c36e73665efbf59c24b9d160" },
                { "sq", "76ab88b9432ed7fbe3c65130d2d30ff417a0d06f1a629de5b240827b9005616c83e3fb02c3afa240a934a2d9b849d125aaadfcb68c6d91f7f12139925abbbdcd" },
                { "sr", "c97ee52200105f5408e7b3e1128ade6522d46e091dd4f5fe373672932e0498e4f48c78d62b64b598a36587e9c097630a1b375dd68e45b49f63ef3f5c1eb55fc2" },
                { "sv-SE", "42a392580013f1e6ef5b508550c7ed8f1da0755c33ead97428d7082f594af06d369a829b50e6a956d57c83e622efd056db3eddcc4a7c2b039fddbc297a043269" },
                { "szl", "3636fa8d75dd8431a5785be2030047f279e7437de643e18000b9f46d8159a4feb7a6a5a2a9a003483b800190808fb6a32a977fb2f9fab47a83c7b8af37a9a646" },
                { "ta", "7c95699b8b5120bfa6a673f05b0cfbc5640f7a04a0f9f91cbad0e9f3baec0e2ef3c9c9c66ca8dc07c1e601e9165b563647b684b7b6cf40f7dd70360bf2ca9677" },
                { "te", "f7ed70a8baefc773ae30443b6c5d384acd089e56c0254e9cfc75245552e92bfee7399a74e98ead8255da99225f48cf819979b339bdc207697778687fbeb25f17" },
                { "th", "f2fd32334a7522a3230778d056b1232e6e0461e5bd90519e329345a58ccf8c70c442203a7e47b2abaa0ad40d135046e2ec1c40fce412bff028bfea409f5d2973" },
                { "tl", "39581e7c8220a4876baf342504ee83b195df37a257b27e4661a72aa6f8404c1afa1ee957daec89c7285f47d3171c3dc32091b471911abb56ad4a22101686b427" },
                { "tr", "75b55d5ee4e081a1d59da4797e446fef6baadb790b0c5759dde22a27f11e4a2a22412e8a8fbc4510501464b17d0525be9d72db5f138735cda098286f3c09eb4b" },
                { "trs", "06351abf1ddc7c917d3b389bf04d6d6ee75a5fc4643b88e3e2de38040933d1e1c4538cf062c497cf51ad2c5884c0e72a3a691fc7ff1bf9f437a115aeb80f2086" },
                { "uk", "00cda5bdf3f47d5bb0d3c1799cd64416d831516d23f373d71715631534a48c3fc032e53d5a3387ee74c6caa1480a7ea17795d2f1416d9849f96997100b037aee" },
                { "ur", "1cdb8a565926f2aa7ab8a32570c5e4c6ce51f2c5617479c69e52a097491c0eb82dd0afa650c124079c6a502f369bcaac92acad1f0d44cbd9d905cf08c49c1cff" },
                { "uz", "b75018be13c889e7fa10c5acec8f3acc8f7fbd742eda937c09c33b6028e6e8a7ed9faba04a173664ac09242c5c9bbd9d230d3729e8adb448722d3fc241ab415e" },
                { "vi", "cc302f582605e64bead06fbac93e23934d8e258e40650ed0e1ed8d0d5468c2dfe46f68552965d3efc06120ad742aca14f63495922f70ad2095175325945e6c89" },
                { "xh", "4f226903800b4bb5972b052c8113ffd93ca96e8c666ad828ff15a7a5a3f05edfd15d23a2dd755d4cb62c5835ac4d39127a2aa3747e302becfb44e77113c6c3ee" },
                { "zh-CN", "64e640c6c6525fa3b4fd65025ef19a4a06b7bf6f59a85047b41954c61c7d5a717b52001dd2cbd37209f43e7e8fa1ee3ca6cea2351bf9e1bd6c9efc60b5ab5905" },
                { "zh-TW", "bef3ba1402aff0aa46c815bb816f048fe56083d3a3699c02a83cb468c63159247f22ced7da5f671aae08166aeb8e0aadd090373b45aec1ef31ffdb2ee62900fa" }
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
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    Signature.None,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    Signature.None,
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
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successfull.
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
            logger.Debug("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
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
