﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023  Dirk Stolle

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
            // https://ftp.mozilla.org/pub/firefox/releases/112.0.2/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "25307b10f1802ee58fbada93446cbcf42a75c83c15ea0934f8c024a3e48cbcae9276b428d3552426cf1f3f1e46ebcb984178a55d9928505fe5714cdbdb8fd19b" },
                { "af", "f528cad1701ccdca4c3a7995a3861e9cade757c89d18a8827dff1ad2bf6215f18d6f887caef38587a4e2fba340525fe3f7326acee6f8b6ea4ad2364508e8c7ed" },
                { "an", "a66f9d66f82a393d634f25476fb2b7f3005efc118324448c2c7325538b7b96f377760e5e9529f76c70876dea1f5e1accfb3fd8ed437e7c3454c8db3962829eb3" },
                { "ar", "b0698cc5f39b07b73aefd5da094d957e6598f57bb5645c487cfffbc7bd6a92ce759a5dbd9a3b52e6e728b67cc68dbedd0e7faf0f3269890c1eca2c64f1714166" },
                { "ast", "5e6d09428ffe5f171d5570e66f1a1c2e2b798ec3b08fa037325612ff8499db8caf445aa2d5ece43d2d8032bc834e8a17d3fa8170608b270ac8cf706054293cf9" },
                { "az", "d265fcbd6f21f7d598deb2438814553919a710480f43666bc3c0c1af84306d55b53d4591e9a50f69a78a8e9e54e1e2ead999269cd39a9233b1adba8ad2af382d" },
                { "be", "d484ccb08449022a1ff1ef157fde37fff5ab57df883b90388ed38426471243d7ac4422a5c84a1fd6b341863f306165ba8b8907346a9c61aee4fad53c529fa84c" },
                { "bg", "3bf1462ba028377f5928dc11dc050b15b8179092e9c123edd43e20771267c535ec291dd1a12f4462d92749a035c7b0e6cff8071a40ccd45297c5945ddacbe647" },
                { "bn", "b1b2e3d22de4c0bfde4a89ce7fbb6d7dd0ba806e3a52359f7fbace766a82ce3dacca72513471e17c2a36d6786fa2fb1d82c6d6e624760a8c6e310c1617d72ec0" },
                { "br", "5959eee7e68fa3233fa7ded5d7669b046c49aef02198364498246a25b7b56d53dce5819efc91508f734411aab46e62218c8f589a8acd21c2fbbd6f5bb461460c" },
                { "bs", "fbe5be41b77e1d2db75192fd9646b2ed9e4473a14b99d5a02a7c2af538c647927ec1b114140bc6ea294dadd228cf5c8cd230df9ece44e1681cb03a5197822848" },
                { "ca", "6fe3bf394a355645f1c55f783c1988beafff34af0750f9c97ab2e1e246d47a211871e805d35c6e126f60a6266c42822eb1e448ab47e38a7a6544659c4f4cd765" },
                { "cak", "087b755c32a73a05f2d003121ae35409a98f2ce449a029acee4a4e5697b8fe5b52ddc59926b439bef476e11edae4723589df9a76f70a8c49ec694545a220229c" },
                { "cs", "8917dad24ca4e16cf914d774f7d283ea7a9f45c913d31f10b9a73dbf0ffb0db1fa1183eea4539a2a679dfde66ae4f1c73956655d83dfadd21481c0cfdbe7695a" },
                { "cy", "97741f052a31e07cd490aa4f0f5bd0d5824e0d8f2044a9f345fc1c29d6bffacf82d3afe2b2b21436be8f2c332d2b70a122df5bb01a4f9025de113180583af1aa" },
                { "da", "4d2400e4b7ededdf9e8fd91ddb79ecfcac64f9ce89ffac9b3e402b5a40110c60d57e191f1b1806ffb69673a9b2536968cac64e1b9ae10860747ecdf4eaa2c543" },
                { "de", "4e911a6d72dbff1030bfd66a63d7de31325d4811a8204d9c21aa94fd300a8ea2bae200904d873a8fe8a03b30bea05a099d92e9a558fcedd3090e0cb4d91f433d" },
                { "dsb", "ab7256eb8c2313316a54512943b382f1465ce88aa550545455702650ad3268d4745ff171b1f9d647521b21cc68dcf6886a67f430fddc5cc0fd39c616441777f7" },
                { "el", "5fbc124ae6d358368d9f77533b162656d266150b9843bd0359ae6f25b930c9eef4032767ca8f5b8700b8a9f8f142056ea24acb7852cd814f0f91693c88e6a533" },
                { "en-CA", "c08854e2851499735f5b7702dfe5ce8edcf4608164b07fa8c4c9c0b8a674831a1b3faf90f9773565d11a4b9a086a54014f96a2844ba56fa7c75c7ee96843b051" },
                { "en-GB", "9fb84e4dd250aa9e34638ce6331b75f39e3cc379ca5b37edf5ea80ef6056e16a18dca44aff005336bace0561275a3a76dec25488e8f3297bef1e2bc89ea1060c" },
                { "en-US", "e16ed3b15ea6ec3de59ff8a4fb57cc2f791d3b7217fc62e3c1c9930240576dd2b3513890833c0830405a2a9d48091da039e018cf799495161c0f6d7a28d6c621" },
                { "eo", "3856a2afbf8a3b7462869c7ffae10cce5729ad35bb5619854360a1851207526c5bec2f27022d7c2964274010e8038b583444b8e5d8477a678eefa3f6342ea284" },
                { "es-AR", "2e0eea4c58d39fcf70ba656c1b24674c8959d6e1696fc7c00354fdcdbb9ce96f3e4558e942322a5eceb7519f56d5f5eeb4adc55f25516204628610c150feaaaf" },
                { "es-CL", "73df5e577e75117182833f71e41744d87f2a35350459462fbcb0d9d3156d37e78182ee4d639699adbf70fc9ecebcfe0112520dbef5103e66938921c232efbea9" },
                { "es-ES", "ed841c280681d47b910d6bd323204681fe0552b623407eea11784b3c47b0937b9b5cb8f339971fabda66b2b9714299941a619a8e13b2924d4a3a511b0ae6cbc3" },
                { "es-MX", "1607fa7705480bab05c07edfd60e82661dc3af813a1aea04613b10c5305688a16b57c354565b6e6881b29d40cb2cb7f0420f37adc6a76ec51c6da97cc69b7157" },
                { "et", "0755b81c450a6205b9ae23f7188379e5ff7e5691f3279286ecb589e8f8dbb7f33244941b94f526e52b35b1fc21ee3769fc7ee5c688427d161579a79806804887" },
                { "eu", "3ea609de2253bc78fc0523b131cac3a1d8b30c3dc22b0879fd8cb0fbe5e1f686cdb3ed598b7bb5db25596de7231257fc227d92a2d61804eb367ea7a6a661eb34" },
                { "fa", "631ec53ad624fd23fb4f2e11e4828e76e96559f33d9e84a2a4188125c03814058d1429e4fd0b054a248a60649f284141e1dcef559770e0f2dae7e92db966462c" },
                { "ff", "7152c381a6d978f0021e91b97b5076923c5e41ddbbb3c0d1c75ef8233071e68605810f9915ae065fdefddcb8e30b9a244a0bcdbc7c1982fe54a18af392435775" },
                { "fi", "8c67b2145dd8c9068c0175afa765d9d4346f4c0b5be4a31bc12dd44ea4fedb200ec4af2719495934e22e07f4c77bbc17e8a0790359f5e38b0eeaaad761be124d" },
                { "fr", "e951ca65327fb0ccf0703ae543713c2d516f0c7cf26d0ba82ea9fc6c82646b4a2b16e3326d80141835c87877ffe0a1acb0d255becfb1d9b849ccc87dad01996a" },
                { "fur", "deb383336a06502d534b60aff15ee002d076fb6c37dd44d0938a8e7058507e25b1166d5b5745ed2ef7be8885d7ba3b2f7fb1d3f504c860d8a15777a61957a24a" },
                { "fy-NL", "4b2e02e047ae8930a850d4a1dee8ebab82a06eec9b7765a9f9bb0e8462cadbafd99e8a66a24a4893801c65a6e071f4c3fc1a617de53a56e6818beb935cd25c93" },
                { "ga-IE", "86f173872f0dd915313c9975758939293e410cab6dcc6cc1fa6872febc716dd7780511787af0dd754f495bf0dd99ad63962bcfe716ee57efc578300533aa4356" },
                { "gd", "65c8bbac02e7078c91710123b2b8672b9a454fbdd3697cb136fd939337165d0265ed01b6bce40ed54e405f1fe99a37692aeafa0d9682bf99075c52d40967b8ac" },
                { "gl", "ec34ebaa94b40380308c3120c07a22edcf838f48048facfb7e4cd0371d94fd12b91c1032793eaa439240db17b2c740ed3bc14fa67f9d11ed0e1300f9c899bffb" },
                { "gn", "2095bf70fab72363f87ccfc9b81c74e94c1a2c23684d2e3051df06255fa302c7d2ab0ee16f7ece824405f873b5b8a7ebc6d69b3ed2b1f48055941e69fb40b645" },
                { "gu-IN", "e5f396d869826bc4508e2a6638eb5df2f3c873f365fe89e34bdefbe8c5543f43db71f23c0720630f3ab00421084ffda7ac4b6d235374da8428627fd360fa9621" },
                { "he", "74d97990d2feaa295904363d146a12b0d87a50b2fb79f08e2e5119b4d84b0760c9e0a204dcf3559e49cd0255b3f1d0e56117401b298e6d0959a9d4c34f382b65" },
                { "hi-IN", "16cd24d27bda1eab9e5fc278d77dac5b631be9342b71aa72076395bab547d65bf608fd0f3aaefe49648df1d345320fb011d76c9530c793b7a3156aafbcbcb697" },
                { "hr", "b39f11dea40644044121286fbd632baf2b77a8899574540ea32c294b50cd22038749f3b01c1fd185db4bf03f766882cec8f0cb601585f57a0bdab9c26d88bdda" },
                { "hsb", "8ce83d27ab85b48c027342471e45400e43a5f2c0f292fa3962b89737be4881043fc2d3a7c5ae7091d66b0c926f95d3cd25a46939331baad83388cbec588ec77e" },
                { "hu", "a5bed3c32662571056f45749232333bc7dab9f01dda1b90af65b0150c71ccac5173e200467ba24105d1723be4511a3e9bd7214573b752ae3dd9f159981555212" },
                { "hy-AM", "e42901868426d18f7d2c1d730dbb44fce11b2e325706582540b86a40a0382b964eaa2447b7412ed8262b43d00e0da2cb78d536491dadc03c43b3f2833169cb9d" },
                { "ia", "58059e90ef703960ae72e4b73c7ca3057c240c47a21fb6cc31cd35ffdb2187d29b41cfc0ad595f4307b7ba0333973f5337108f4e0da411f87d4fca051e5477fe" },
                { "id", "a253625ded5641fb53967b36ed47c688106ee4b895b1b162df8c5eddfa39a8a70762e7efa0f2ed51ac840d97c97cec7d47d7104a1524a6e334a4bbe89746a171" },
                { "is", "8eab62c179c0475ecaa4184616076e5208009d5d41850cf6e32f34e689663216fdd03103fbee869a3be23433889158576f8d79907272bf55ab5eb7a429caba97" },
                { "it", "699ff1099e254e73abf530097ec572dceb6f5ed78a87417a52c70dfed4a7837deb049247723c91fb363f56d9fa9c99648ef4976d8cea27dc4dcf449e1fe80585" },
                { "ja", "ea01b1056c46c8a72071b05b959ec8452c007f54fa853d851b1e81eeaa9c0dad4d67b53657c1d1a9471e1b19d49ad311354498dc6c189eeeea1100961a60465b" },
                { "ka", "bef7032d092d0a92fcb1fd0456e3d1ce869ee4e6795f3ad8d5e7e4277220303bd505322e47a397b92f4afdf66516b126df93ddb869a7aa8e3703defaeb3c342b" },
                { "kab", "f9872f9d4e5826cc7da1db259f985723a963bfa5fe0984960973b1faa8abc8f8280460c218de9287b77ab40c73a6aeedb2cbfeee73fef0d4902faaab1386f2fc" },
                { "kk", "8066cb1dde4070490493dd334716cf1e2fff959d21faa68324645a1957a00844060af00a963a7f16f68196111a85222f8b984a21f13797baf3e81ebeb83e6331" },
                { "km", "d8fe6a1113db574b42b65cd440c29d68da50eafa54174d6e1059c57350b454668bdf4405f4757d1e0ce13e8a0670ebc7972bcaedf9c463c623e1825162b7e7ef" },
                { "kn", "57dbaf1047ebe1350b23d8f8409c5df3d4d8905ac64e34391ca3dae4ca8e6c5021e55fd77a8d258464a7b84f002eb8060d7beb365743c971206a35096f33fcc1" },
                { "ko", "77fed35b96e8db545e8d62ed4f45cce0b6514e89e65fcc26ebd8ea124a9df7cc57d55f26f0c45d4b6e44713a2d975b412bbcb276ca583acfafc3fee6040e9945" },
                { "lij", "8c00b136f864948c6788bd5670bd3adccffb7c5a6aa6d23728ac7503ffbcfed8f108959de44e73b4756682c4dc98e65c627876fbbbaca26c3aa6c8a92ea2f180" },
                { "lt", "95fb0b2d5ed54a01c77d2a2fd55e3f7c70104724e8c10cc7a936c14da45a30d01976d7cbd573a3ff56e0b62e334ef4f0a0f9b463470e2e020c72734206f40690" },
                { "lv", "fc9397380ed21b72237e4cc768713abfe9a6aa601b627e5eb996b360fe1a44217502e73a891f5791e384bbff2b104a935a5bd9ea4ba5cce3db5d787c0bce4ff2" },
                { "mk", "e53a3d0403ae1c408bf1a75070c3d8daa7e78035626dc6b9b8bd9f6797cc3c5adfde6c3d7875c1ffbc620ace3640ec2806882990f58be5d59fb209b22aea847a" },
                { "mr", "d9cdeae6707ef73e0580cf564b6bd0e16c8a1f6d99f1c04d716875eba1c1491a305b729efa142607b69bbb59c70ea2ac936526d845151860db0fd2ef24abd0f8" },
                { "ms", "9034a0760872a5579a38c97a0e2b04959b4a59bf98fcf41139d1e5fd0987d86053db3d43b9f1022161adcccb008cc075f74d6169e1b0b92632ced1a8b109dddb" },
                { "my", "61a7a57b52cd2ae6e11f369827b23bc40ceec17499c3eede8151ccf4e5a4f631d0ab43b9b117d145d183885b41b2cf634d4c56a45c46d7634603aeafef72ff38" },
                { "nb-NO", "db2bdf2a9762f8545837909c3124ebda82ba17758cfa764e9eff743d6f5cba1526295216f1f71a2bc40dbcbfa096e8d00159575cd524d8a372177b962a7c2246" },
                { "ne-NP", "ab3b831f300bbfa9f7c211b5c96deea9ef9f1c52d0193cbb92a823874e5726c24cc2169e8766e8be78438e9df61c7ce2f9529f78994ab7f51786c6387a79162a" },
                { "nl", "dfe98bd7da24c98c43ad2389842755780ea097164389b582e325b9d82c14acd5d3bc4e99315ef08e6fdf411bd4357d546a21e8ad57a103b72d6ba0675848ab8b" },
                { "nn-NO", "79e4e1fa52bfd790bc6c277fad2e894fcd45557d856f2888345432f16e840ed23048c1a101152dbcc4c7c884eae4bec1623ec9008872551dac3cd4c6b6929253" },
                { "oc", "75600492a99d48b839c7a5f31462aa06cb9876806eb68a34b2befae15fe75df6e87a1a25da723ceb8c8f77f54871a96273a958a7e99a80343cc0853f757b85de" },
                { "pa-IN", "301b9d73fc96c12865e705ab4c2a6968901e2356a24097823aaf866dbababdeee574009933357f5cda14bb5da9a1e7e02bfd0e576e534c39687cb872acea7f82" },
                { "pl", "960051cf4b1b1f89d731d30663145e383cbe12c55f94cdcf7ce65f672139e5beb9cfc2e296a586316da2458428a1528f9d6a2f61c883d86e798b4fd9bb4cc00c" },
                { "pt-BR", "24882d3ea6dda235e585376e1736a694d31f41246bd7a6e169c981aa1437c280a54721bc1b151fc24e5a1b2af9a50854d19866cee38ae1b1dd9cd3996a57f838" },
                { "pt-PT", "4f169446099394747bf5cf53ed3effb0d4be88977bba9bbf61ac1eaee5a731fd3a13ffe49e956ea1c8e3443e45d3ab4c88d88e7d309b71badf67a0eaeed1ee0b" },
                { "rm", "7f46bce350b79ae37126c52b20e20e115ac4c16c7b54f685847e90d7927ef571d0c87c1b0654692e76736f8b21259580b266e92182085b5e085c8004c6b9506f" },
                { "ro", "5902f5ceb89fe24178cfae5b2757076597f095e0ffe26077aeb71b0c40e493b5d8a6c2c166403d960c979b99954e7b0b1d5da7ec3a8e24634f4f3b81158637e9" },
                { "ru", "8a65cea9578c08b929123ee466f4e08966d4b7aa79e5e75169cf342d74e683c49c13aef00ffcbc7264c9dbd5e1c3bb9e25c11930b30497668e4aa775408c57b7" },
                { "sc", "a01ad416131c3b748ddac95b22dfcf4835f4439cbd60803447f3afab3f945008635f39db051294fc3fe7f1649b099676a1bdd6f0a805980ae1bb515aa196ccbb" },
                { "sco", "17e7a460f2e8924553e5f2bac3963ad8d84111cb2f8067080260060b1da8f438263d070e545f9f1633d8828d56ea71796761e7c932e8e48c6a86d49e28f8d5f8" },
                { "si", "33d9fec5b77be80b21c02bf07ca3e6962e2ba0643f1c4bbc4d7cdab344e8f17a4f723c563094bdb400e511a922b0ffc21fb0b0df9976b0fca2b4b93e6d0d5cba" },
                { "sk", "d78eb4baf580d2283328c57f115eb82b12e90427afdcfa883f7faba75a0fc43e3706c5f1e8244a1e4d5845b8cfffcd54e29c334cb5d8d0db9a69ed23dbb280eb" },
                { "sl", "757a507d460cc66f57ede05e93f2c292d3378e8eb950cb32e4dbe2d5604640513e579e413a361ddf57563acf38889228bd05fc4a2b36e9f62ac92d5516d36e53" },
                { "son", "2e54a4b87b20ef18c25a89b031776da4e7051d785dea295077be7b21e94cc0483a1aa4b7c8bced7307320096f8ad32c9beb16bedbc0462ef9b4c9fd18a680cdd" },
                { "sq", "62bd08d27afab53f6e256bb5206c2243a5727dc4da81ca4c2ec21d740535b2f65364ac557a03b9ed0cb8b232cec19e047bb7d9fadc90d2e59ee078ac5031da52" },
                { "sr", "7f41b5800f377bb0b45c60b562d3ebcc479e5491978df9ffa5568bcde4db58c7cbfedfd0dd893e0aebe5ea56eb044cef1b53c695bb9d8db70f99918a9d27e54c" },
                { "sv-SE", "0ec8952a6e7e947284271955ea55532c62729e44a6198e5c0b27b3fb0ec9c92aba68cee882141ad04483fb8ab7cd713a2aa7b7b713a5a55923996ba404d0ac68" },
                { "szl", "86add0f911ba2598bb18fddac4ca4c71c52d37d0e74d4a052dd4c3977228b3ac44fe23110b86eec7aec14a11128e95c09b78f9036c20671820e5c367d6aaa8e7" },
                { "ta", "ab48dc357747065c0868e0f9137c80241e46545c8693d3bdfba7d8983137947d995e037f97f5ae5a0782a9d193c5f44abf64d9949e16257e871f037c1d504ac7" },
                { "te", "7bcfff3304f76b77b321cc417d94341381b3c167b0b24796f97218dc559fdfc39482859453aeb7eb357d8d9045dfec4fa6aa1d4d1a3b3526f7738d6febcba258" },
                { "th", "42ec326198ca588a208916c1b653f424ee3ca1eaccb76339b917698783ed6ff61c406a93287928169c2adefceb353013e9e92f9f1977913e3a72a22bb22ecbe4" },
                { "tl", "8f73072cb9bd1747197aea6e46f6f9ecd90d5a6ee751faab23f50ef1f0355cc6cd62b9fb552b272d3cf859af2c91b8f5f5c9ee2d345252122b2ff8c593b717b0" },
                { "tr", "fbcf51c7084efacb46a0d57934ea2db8207d6fe6dbcd93d25b0023309d5bd714aafb60d4fb892d05c1bc54e35376b643d0069b10b088db51da33cbb686944ae8" },
                { "trs", "328f2533e6aa90775d0528cfc51a6aab5b66b6e79a161a89efaa5aee0fbc703428f36d3878e254603ab325081fb0c0779a746adc894b92d6c3cb94ec08b078e6" },
                { "uk", "b6bab582a92463415fa825a16efd2caaadcebc525b8eee6fea539631aa6b5ed73ddda0288bb54b50af5eb453c90adaed7ed926c91676049face46df04b1cc612" },
                { "ur", "a4c16c61f64eac7c365841e40e901740aa3d86c9f7d93317f4a4ea6dbbb7b95d35342d927feb4d221386c168832242cab10f8bd07fa87ae8d39848a5e11eadb3" },
                { "uz", "965d93cef7a5bd80a828174db4c9117399e4f69372a3be8879629d5e488324bba4c4a899822942c93d6efcb0819e67247f22a2bcc6a2e683297e0a92675aa7ad" },
                { "vi", "69c4c7e7c578346de5933cbd07679214330898ab8f9dcd0463d0e37440a2e20fdea0a129d86309a4cc1cf0b1b16389430bd6b0d80d6c0be8a333a604d042482a" },
                { "xh", "ba9c8be93a831e72e1715b73dd7595674ad438f12400575d709f4310a23cdf1ce551e766af1e286e18971d9a722f617a962b9b68ec000295231f3bec678b737b" },
                { "zh-CN", "89e84b8db5f64295b31b37e77383a0f19bee3ae6fed0e94f3506eaad49f8a53111efe7746257fcaeb00fab37b08df5784e868960a95379cd7f59d7726aec9b5d" },
                { "zh-TW", "87b395854b665318d94fb0ca72f9b7b43d628812e7d9e51cf07cfdcfef36e4cf57449efa8caed951fe4beb3d2b65dede2d770fe47ea76b31cf079aa75e876e4f" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/112.0.2/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "0cee6eb1d574dd0eaaba53565ce8b25e806c0c96b47e31622246d1d0d233d892e3ab6edf79e251f6910ae1d6c1ead35ed27c70f5a359d4aabf4ad7d942a75b3d" },
                { "af", "606c5978d3cd47259967a1dcd3adabbf4602d5d0fb6a16a665ff600cf0c42b14d2785dfe2dfd686c067f12fdc655145922eca83a32c836fb9374a274b64f99dc" },
                { "an", "f16ec2b57741ba0f0d321673d66609d0e53dbc28aa5d0048b98ffeb8c34f37885d268c1e1e40241ee035ecd6fb2f8260eae6551bbb351efd7c30588f587a136f" },
                { "ar", "f93f6e48c1537ae1dd83d16aa44636b700435f472b2cafc33093bbb1829b2cf99d95cb1385c3e2d6fd707a9e8b4b10758906112587b0a8f40c2215306c524b54" },
                { "ast", "4fab30c63bde25076d0775a9b19feaa9781b7b032f0476a13997113dcf30e68fc1d5fda5d99c2cba05a4d28a9941a27490540fd5ee02837f38eb9006dd243c3a" },
                { "az", "7acea624e8581b2e6cd02f694e4e74935b1a196682b8caff9681de5dff594ccf04e3e3adeef7998be114946542d0b5ec8805fdf248639a3ad2acfe144f6ff3a6" },
                { "be", "b2fe21f0996253045a5f6d1edae76e427f27f3cb83f56da4f569eeec562ae9e94621c681e0ef1c08f6ce2cd31797bfec1a621e2deba55b974d0305cf3e8a378d" },
                { "bg", "a725aa81780cfadb447b26f9acac69eb31d49b54fc0cc98307ed66daf5390915340da3258805d4aa35e195a40ff15cb6a00da59b31287aba9667521c903a1966" },
                { "bn", "91ab3cedcc75d21d4e12900a19c08e42b47fcbe60f7017c502fd220486b906d96892082b4e42f43ea27fcee3539bb48f92f9c543497ca6a62a6b5d92846b8628" },
                { "br", "5e55e9061aedba843e653cf17671b8a2219df419e3b50ce2db488a1e305885fa6697586dedab4f064c96d4fb758cad76c04aef602f4d1bcaf802d46877fd6d44" },
                { "bs", "395decea4bf37fa647be48515f26d6b5cc6a312c5a1e355585caba44919faeea1edf0ad97045fed50565a80bed955d002619aafbb56f914c619fced8494753c9" },
                { "ca", "2c7bb0ead95cea063efbfadd9f12c7054a805af2a1bc2567eca6b27d51814a41ffe8175fc58468f20d5dd8d35064b03bb8df8d628ea3299bbbfb078b427b7332" },
                { "cak", "f117482ae427aaeefc2d5e4a7faa7073e1b7a9bbb0536c548b29ac12c61fc420e824cac895a8fc6f20e70b783d51f37679b9d90db6bed7cefe57c577a9858585" },
                { "cs", "6061e4d813c5c0aae03303ef6b7e8a3382e3f1ddb1593aaf72b18aa6cceba81164445b29ef17e4a40ce0ebad97fa47964f659b341fca25c0cda8d73c96dfb450" },
                { "cy", "86b1e43ce6dffeeac9283c524e41724a3e08cbd67ece79714b0c947c0e71ec59ee979478ac65c132e8ad566c69dc6971131de27d56d662fe5b8127065f7a6e5d" },
                { "da", "84198184c23034ad8043dc05c938b7b162886b8758d7eb6c7f7ee12b1b1f29697e0928862b195df82ec6cd576bb345402ac847ff60f2c55d530a245805518592" },
                { "de", "53fa03d992b41afc967b65aa4616d474439a54ccec74303217c34387e67f359922bff3b7b908320fa70ff87f8c2482d498ea75b5bd8d7e5e58f0749aa24eda99" },
                { "dsb", "ae3ade2294eb3481d1a81e9dce103c87f5b39d4246fc07368ed4cd3c3a35ff3ce59edb107277bcac48c36be57cd5a964c6297626c790c9895b1c40b618ab5cfe" },
                { "el", "a479ebc7ec36b07419ac70777a2d101f59dc3b081fbfb73c4ea82491d6f91079810b1626dd513b004b77180dbcfb807d36ec615abbaa1658b921aee26a86940e" },
                { "en-CA", "5ee2e6d967d8903956d4823d01d2c0f27183c0aedc1261f71c844a28698b8b5ce606ae3e4063c4a49f370686ee17ab12f51790e5371571246227afb1ed2db6a7" },
                { "en-GB", "8de3d0475e0b64401c95c7974505913e681fcb22c456520fdbac68b4efb0d9ec66c6f885252b4e7b414e33a29fa9854959c948bc94edba8851761f9ce5037875" },
                { "en-US", "ee284821bf186099c3d60abb6214725c2567b4ca6ae55b4660af13db90d58e427636e7b6ff4532de11bc0c3fdb4e8d755685f6d4543a9da538b7432d2bc2885f" },
                { "eo", "15fc1f0c06a658fe5a8d7df35122b5ed6220cc3260c4f60497f0d4d83bdd89a9dd682ccfb33914359bddbc36eb0b929ff096acae3405d2beb59166705228e03d" },
                { "es-AR", "7b1cd0206fee7aecdfe3a49c3dc52fdf3c664830ec6d757b8fd7304bf8aba7fd5954171532fea6b5634bad63a65e35c3ba9d961f34b8b93789128a6a30f30e5e" },
                { "es-CL", "7bbd02de02d4051240075517e2f44e99e1e6641e134b1238ecb9466fffad4cb87219bcaeb457f68e01087fc8c572b60e4b4f2ba4462c49b0204ce791631229ed" },
                { "es-ES", "fe3f868ea3af22aa17b144af8d3cb9738847091a3b118cecfe18040686c7485290e0057c9526af9347d6c39a792fc19f30c3c0f2db1086318b7a2ae472097a26" },
                { "es-MX", "dce71da0fa2672bbdc1c23ec9fbce7521eca50011b2e6bc0df1e14976c3ef0c2c31d7080e73bddc43a0844280cd641470676543dc2aeeb7f9419c14e8082d38f" },
                { "et", "52a5a2d7eef0d177d7f4a8de7e5ab40d14dbf2f408eb6193f400c7f14d5a7d236aafb863765b30b2871d03ab8d8c31248d6b98045d9bb68f83f1ebd1295d0e8f" },
                { "eu", "0cc826483535123c2d74a1188bf8dfdf0e11cfab7be83dab600a1bf8c66c8d51b7288f29509533734112c746a8079edc188dbef5d7263546d30b2390ea1b61a2" },
                { "fa", "faa31cece6e10c955001182f437cbb1a19bbdf2693fa4844610b3ebedd195670ecffdfb2f37b4cb8c79259436e44ff732c601ce0fa037e4b76bcb3b0a63a1bab" },
                { "ff", "849b928110abc38622aaf6c3e1eaba690d5f739487445b039e9d94e44439eb88171f15f6407422d0d43ce35957c909a243a393daa1e90576ee710b5197697e0b" },
                { "fi", "0cac74db5ddd141441fdb352373e31e696d27580f0f3a95061335433afcbfc5ce267f8e7c9a17910499a191d1dd52e11950a39d41d8dd2ed4288a3bc6ece228b" },
                { "fr", "d7f0ed9bdb8f60e5e40027796dff60b54d3f175a9b89cb7b8fa7b0728edf9b29dd8c530bc562711869f22ecd74ad4f35f886bc424a1c3dd53b5bd28d8a8981b9" },
                { "fur", "1d2a5d0a113699f91e88d3dc7b0882c019d217cb30ded85f0fbda9e90342d1733409cf879bbe6ce01c34f84d85f530162b32156d206cdc9e5a60d39fc757fd43" },
                { "fy-NL", "492c575e86d7d10677b3980eccde9757cb589393c2a395f7b07c4f273d1c8d50f15d2b095ddea46ac435fd27eeb96108e801561ba3720983e33ff3f304f8f597" },
                { "ga-IE", "b7fa6cda0a0e9571be6f1c3db66669921b4f072a5a47a30048b39838e039e6099de2b4972acb42fdea3218df0725a7ec35c53971c1c68f5dead53a550176dc74" },
                { "gd", "b2a4ceaa7a4240c5318ecc6c65894456c3ce781dafc285021690e4d142ee8c13d39b9a53de853ab5798f80648b100405b7e767186e40acff740f224cf41cfabf" },
                { "gl", "adbba776ac3823bf2a311cc30f2643ce602ba10c6adfcff637a6e762220fb2659675ffd20f5949a186677f0af06d6c7188d1e06791914269479b9192042290e7" },
                { "gn", "3ba42e84b21c9af051041e57c68f80bae3cb2e9eb492f04f05e17b503a473e58e1b589e15a67112324728d69960628eda1d53c1ba10459858f9badcf8d4660be" },
                { "gu-IN", "49159359fe07f46d13967b36d3797840d687caf677a6b170373b56b72118c8dfbd9f409669f3a858b91d18667048f32a41336e6ec92ab7189b198f13802cac4b" },
                { "he", "d17dc0d4594ac5416f35540843a1502e5af6d5ba49df2ad050e7337bc1961031fd3f40761daf87120f1254a62be4aaaa280f46cb351c5102a9404c79da4edb1e" },
                { "hi-IN", "fdfe15bbb8cf0e0a35e66db223a30f73da0e91b6c0e09c53d881457ac5317d18aff639af186e0c048b34bc87160ab57c144abe9ff68c42e845c14dc62a7e95c1" },
                { "hr", "bccaa74e9d25e547b706270b07c36d774c4c6d337a7d5801a10a127e20020f2839bd7ba47cabd7707bcc4916245b760ac4081bbf53708e14b4d9ebda88ef98c5" },
                { "hsb", "e7429ea7a8b0f64d3ba3c331636ae709aa9a6da9610a5c3c2f0e9425d03537be55867fa581130e27be34540706f1ee26b65d5dfc2fd2f9559e89a46ed1e7b665" },
                { "hu", "9e3e1edb40c51a5203fab04203175304ed3bbfb53ce12730741d11cbed1e31a3add949e5a451d283daea0f2b8faefd962e774dc2dfbb5ede2a7ec1328e1799c9" },
                { "hy-AM", "4c988e4380b36a45ad8b8259951984ab5c586c96fcdb6d21ef1af32425380e08d854b5dc6085e44b72aab7806b5e51a510ef302cc800c44dc459c49fdba2e821" },
                { "ia", "76520417f5d9a164b5c9fbc8d4073704c6e674452cf4602f7d9ae4a205c7d5f0689b4c177ae0f504f94f4babd25ee9619781a0cbe228b9e1be5968c3630b54a6" },
                { "id", "ffd89d919d59b80fdeff921ce5ddef622aafb1aa086ad92d9af8243d2093bd0407ac555e81fee2e02b5be091767541e91599d9fd035de0390a353144655d9ff7" },
                { "is", "3d55199310456d087159da1d65696a6cfe797650425b2a7bd46270924995450855e9c913c0bfc2ba362879791ebdc82f14148b1c6de48b4a008152a93bce9ce8" },
                { "it", "03bf83a3b930016de3dbec48be107318013298c26e5662cc03de5bd4d4eeb375f53a09fc43343b1b9bcfbdf8f703fe0b4cc12ada5d41bab909d7c73a024e4c9b" },
                { "ja", "e0c2e13dfcc701d6d95f65a7a1a8d178cecd091e10b5a1bdd37b681a39e0ed2498999d56823fbe1d5ae651e75d8df85083ff0406ca743656ebb546e328f5b311" },
                { "ka", "4be0786f2fe479ebbbd9c2e8536331d5e39618605928dba8de4830460143328d34954cab55255ff0576e52ffd70bb6c4ebd612e7ab912b81c4522611146b933f" },
                { "kab", "163b0959679e051a524c88ac8252099ffdafd1c83270d510005bf6e4a326024a7a71b1d97457524d8d5fe38d5a88fdbea449dd57d5906a1865824826b9b8e21e" },
                { "kk", "07d962deed241c99f1a42170258d705ad22cb89c2aeae9d51cf49538eabb327c5f17dfef5dabc39e31b5a0db4935ae7cad3469d8d6a651527c4792ad372fde8a" },
                { "km", "b86f6ead44a70be871ebc47d3bc650d833cf63c3c9ec6ebb1844de57a0047d10c33be5941826953374e32c4d0f2014e2443016798cc8d3911aee59f37b40f37f" },
                { "kn", "d801ef33360cfe5889e7d5b3ed35223a508f7785fb464f1ae5202d0c12a5ffc960c289432cdf0573a5120d40b944ef752b75f507f1036fc8ec1e50138e94e122" },
                { "ko", "6816f7d669ec82f4ae2f44493ac8ba4f8c80656358e5ca487a62bee4829bcccecde5aee31ad06db35b9bf1e63969256e0dd1fa73d783d84929ff452b0092f86b" },
                { "lij", "df632894ee042711540ac68a2bdbf3fd6ee591367cd680377dae814a65e53023f880e9291d35872719f1c755478de2f7476110e191251557d5361edd42220201" },
                { "lt", "d9f4841396608fdd3c1a655bdb13d15b318392ce82c473c71de7ed7849f7596085eb5af55cead486a24aa1d342ee797fec2cdbcb006e265bd07e8be5ce3331b5" },
                { "lv", "84bc4c088f3c2e460479e5c180f2985fc28a4f35ddec5a711589c549b2de3548a1ce01e1cf10d0f7cbc3859206a5c433202f1d7056ca073593edab0e5f632323" },
                { "mk", "201687864b17e9ebb5a7bca44c1fb6255680778df89c2ed40e4659bbf2af93c8fdcdb6ac3930ad4cd6595cb107b96c1f62bbc3e78a898ad16ef8613e1c6bf3e3" },
                { "mr", "92bb335236e3b0abe223148306d8f8c95a1814acd298b926ae427dd4a3b276c73cd811e243ae95711c9a213507f8f06362f87269156f9f6793c5f44b60f73673" },
                { "ms", "3128042ec0a0cae338ee06ebcacea87c30b9b92c36ccbf32912f2737ab1e5ebcfa2247f593d556dad6731256c84fc5108e3568cb8a2856e9771a45fa37d35df1" },
                { "my", "b06c79f7eeadbfd36173c33c50bc08d29f928c76a6f593ab2a20162e868f4cbc302f63b9dc1cfacb460a5edfae4406d5ef3ae9a6bd00fa5fb24ea76e7e6dc3cc" },
                { "nb-NO", "1799999be2ab15302418261e0c3b9cd9db1c790de814c906e42e2dfcdc303d5657b98f1aa7d147a318baf7f0b1d6209bb93cc93821d9ff6db63f3ae2593861f3" },
                { "ne-NP", "fafe72ea16e7b33b7af658846bb9f6e6bbd21bb175dbc1d9613916b6dd79ff446a4e0f4b30ef4260c4c3aca9ba25d9e49b75f5dcb50ebd4f63e5a1b43bbf2407" },
                { "nl", "d805b5975de346375566ac5ae26b26bc21f84b9eeedcb17a9aac0d9a89f667a6aecfb22e7dda4d1537c4a38c08bba12b8e405ecba0a7a994084b895d98c3d5e0" },
                { "nn-NO", "48ad99eef9b535a68b0c668b4c55a3bb385d0666e8ed18298714d7274a8f0627e9667e8bf68b5aeb1ca55fee282da5f448a46ce5e486063233a470e7ef5a420e" },
                { "oc", "462f26524631950471b29fe524e1feabc360a1c6e6469bbebceb8fea7c1fc8b404231d6c06dcd2def5e789e180b7e687c3847e551ece99c838f576f0c40edabb" },
                { "pa-IN", "3300aabeaa15c20da5cb184a9a6e99ed1931f49e19aeee838244fece8bd1b699374ff4e36cd52d522c15640c74b8c38a30ee92593f8d442f1b2833939397e1aa" },
                { "pl", "80dd826fcc38d1c0750efdc6877238fe570391b193b5adf43a61d7120dcbb172972935dc10a60dd1b595d547bd106aa2c283e341d5244d5ee11c7851a6d91399" },
                { "pt-BR", "440b3a3ceb61ea66678bf5c8847fb2455e2d9b46dff21a517904749b200eec379c95d9268d8b8c9fd32a232bad6d0f0c138fd79d6e942df21d731e63f1c54813" },
                { "pt-PT", "be7583122311a7eaa3d5911f3c560aa05ca1df4d04ed06701575140b0ebe4165bb5f6f29a44c5a47c17d4e1c818f8b274563113ec246600d7bb6b054aa4f3dfb" },
                { "rm", "3d73a15581a204d4b39ab2acfd4099bfaae25196b91b61b6be66f2fe2051f6fbd6828f51031fd4d1cf5b484359bd7d43360e73c4b4fdd28a7f7e3717c81a0683" },
                { "ro", "f8c1afb2302b95771c7a0f8b7695264840c341534d95840c4795d7b3d28c73a75154025ea0b828cff62d9f609d3f216f605fc3eebeb99e843bf0821031d6e5de" },
                { "ru", "4342d39984fc4dcebe72db875c1328ca26047913c56fcbb1b5f527cb450fac80004258c9bf976f00e48e9b18c5f084ceeffb2569a5738c203194f989ff1a9420" },
                { "sc", "2662bc410df85f87a5ae61ab24d59120d76fdaf37c64b460867bfd20b49322051e119ea028e8536565b6b3c248aa9b395deb7125511bd38aacfbb457ee282588" },
                { "sco", "359ab63aa31584d3d7df25e0330eda214fe127ce944f6f6ee4ca76849779c5153e83244ae204649394e79dda81eb1ea69bde685cf27c5184bbb21fc59a1e7517" },
                { "si", "c9130aa2e82f56856dea8f80ae2d99e0fdad9eb69a31075660fd8cb6c60dfbf33858e39527186070668aa3753709a7c76b2d292f09a9482b1911ed7884f1d919" },
                { "sk", "ecd263d386bb0054091c1a6b6da728acf9942a9cfb70370664758ad81cd734595e3c4c6d9f536093c5e303c280fef5db099167abbcfb1c43bcf45ae0c18110aa" },
                { "sl", "0bf326b776642d10bea359b503cbb7fff363288ba636893bd3048c84645ed6c5f79353769d9dbf189d18ba9caf96521b92cc83d0db32541e38ea3b8402df2734" },
                { "son", "9d83c23da1d1f1dbe72bf0363ba49c101a6e0549f5eec08e8c0cb0401f0a9f0d13aae2b0bfad05a07500f708dc366b4a262380baa312b697cf2b82458643623e" },
                { "sq", "6f0b063576d7ec36605cbfd978dc2ec039ad9fdf2115651904b6f9986b6a2e46d29331d1573fc5d0a02dc5b07d9521d25579e4bd8c4a23f9f9057c9a865eda8b" },
                { "sr", "63c3c67924bab14c4e3420d827c744e2222bbafc63a936ca902b49c16c5ac187bc34df1ebb8cf0a4f20956be5024f2674529e8f9ca388476efe241fc1eec799b" },
                { "sv-SE", "a05716393f1692112498fd235a460bfec0efc9daf6218824317c1578b125bf7f025015ff0339e832b31a9c13c7dc49b7f9dce57dc9b397ffc519a4d1a38e6e57" },
                { "szl", "9dfc008143331c192a9f9f2b110d78793e137eafd88e4e8092b07cf8277d90694b304db05603b5fd5b1f4d8abcf0781c7b73163d4e352e5b16d2644e555dd4b0" },
                { "ta", "84a54a4e12faf18b16a249f8acb20fd1828bd962ec2f77f6471df13b85e6f4eee7bdd9f5c467183464729813df5b7dd6d2c13a9bbbb16f50f64673f588c3f3cc" },
                { "te", "87f5b60ab1b57a32da012cbaf099e446b23dc5d7cf2f16f3378f4e3a2878c290eaad441bb865afaa1b9b2e2fdc21dbad59fe00d8ab84e885c1652d76c861c977" },
                { "th", "e684f022a699ad9a1fd0f79ad4c4d7c36467cc94b799ec1bb67a90c26d9c7866845e18e9be02cb87e65a981bb83531960cb33c7fbb644bee9bc6e6b65d6ec211" },
                { "tl", "11ab815b23da6b24e57afdce0dcaab1cbfa93d9aac4b50cfd3c8a2c5f8077ab062ebc5f14e783fa38867e98d6c40355821d3011baa5fc2142d8924b953a1bbf2" },
                { "tr", "0c35bfc91ed0aaac76b344fc5badd4af10265d75a053290e13972788ef541cd052f5c52b1c8d8f8f833bad9c08a7a70b46ee92307477f6faf532c9a3936faffc" },
                { "trs", "cba6a7f22191e3497e6f3346dc6fd0bfe768ca5d77ecd4ef9465aac5656e5c84aad28264699f85e6d79dfa2750dad19f3f69831bd93d81d2d21b2a18249a4ee2" },
                { "uk", "9fbeaa458955711a7db0f9652a8f65eba23f86f4f3f8ccefbcd35a526cdd3a7965c674e3de50d1c96cff7b200821c460b9608709e02e9f6e5569f8df7d0b6aba" },
                { "ur", "4a4bcdfc15f6a84a508fa116276cec803c88a7098cd8902ed494ea0fb01cbc8bf85743156da57fa5e5df2f9f8a0adeddf0a8da52490230ab837a00696f3ebf5a" },
                { "uz", "96787be29db4cf2c9f8cf598f9b381252d2f9512c5d2ea03341adee09035adae11e76b72f6d21b2c9279fb5bb537d5e09f0674e88ff1e4fff2ac7df577340f3d" },
                { "vi", "1de3afe11a40e8230c95155bf6b0e75223f5dad2b8cc2f60b32eeda3e386d0eeea4c322e1182f2eec1603145e7d60f712ac917fc617ac25322df7bec6dd5bca1" },
                { "xh", "dbf8d505f9b9d5819a1748d86177bbc2b85a8aaf375cfeb85619caa5a3d499f04705765946a9aebec38692260f2a2ca559e2bd145adf2a5fb075384bbae71a2c" },
                { "zh-CN", "4e596040f5f9e3e90a620dd7292bd8820e6fa70e83aabf1eeb9f2bbf9ff4719cf6ff1a52c46dc61018386e9c55a0bbcb65a76107562c028d8810fde6aea52afa" },
                { "zh-TW", "96ec19a24eefb5171d466fbbcb1d78b6a8ef6a69c5e8170503147e2c795f6ba88ea5f9d593e0a14fded5c2cefb75d4c064383325158f90a6840a0dc9bf0a95ad" }
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
            const string knownVersion = "112.0.2";
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
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
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
