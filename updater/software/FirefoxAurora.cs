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
        private const string currentVersion = "99.0b2";

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
            // https://ftp.mozilla.org/pub/devedition/releases/99.0b2/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "35fd34b62c69fc8334a32203a8a29706a44f3b0ebe1f8626862196cb026ca0ec74f0ee0533bc8fbbe1a7a18b94773ab1e20ada05aeba4a04ef3c5e832b04689b" },
                { "af", "a3bd76bcd139875a663d2b1161d8b5e7770370b55193280a43b9c546476aa62534748a52503e7989a965d38195ba561ae03fc13f1e9ab9918583b6553ed4d8c7" },
                { "an", "21eea4cae756d17efce7d6a105a68e9905ad968391f111a24a2bac5c52bfcf6f8e175e12bd0565452ef901a2735720790d614fdc343f7d63f8b446662f87d250" },
                { "ar", "910496e4e3c84ac168d89ef4f92840b57449224807a38c674b853438aa3015e22628d0d31778a3d82cea54535254f576e1f64a7faaf273bf6518b9ae24a971dd" },
                { "ast", "817cc9b42a564e47ba194720e0d968d2f934eafe7980214c2289fa6597da8e5d4602428472bde0caa17f61b443b88428e5b5bc13ef0a45e73d82824e94580dfe" },
                { "az", "321cde250718af508c41be66a88abed0053a58a2e60b71b3ad0db86811162d5f8e50e4a7cde38f333fbd52627c0c63c5da661414e03b85a4e7d282d358c343f5" },
                { "be", "fadaa1fc9d1556f86f296a64faf652253414a081c0d6426a334acc8b3f6416afe95ad22fd2052f099c7c22d8a23346e8f9768a0067a55d0229461ec6e92bc32c" },
                { "bg", "5d26e30eaa4db12b1725f53a731ba5b6afda7bd846785849b895906809ac6e58693545eb9e61a6a0aea4315376a1600dba9fb3e66391231ecc2df6a3935fb7d6" },
                { "bn", "51fb29c9c31bd3a9021e4fbcbd8e2b823a4a8b669c9a49edeac4ad5633520d57e3e66804da4c67d339424e18207fb36bb17119fc949244d17a62d3668e65d18e" },
                { "br", "f15bafb5d5060f7be28d7eb90f87f44bee865982d2adb09835d67b9859ac1e7a74bbd24ba57e5b90c52cb11970bb98647b689e46194859dc46ffcff5b94f9e6e" },
                { "bs", "c45321cc9a1e59d1909f1e75a741fc871459d537adcb259dde34dfdaf1f9916b1e3fafb9fea2145275766a47c569319690a7e8afd4eb401006a38529afdb1e4b" },
                { "ca", "c92bfa9a0f7ad2e636efa1dab23ae0ee8b80617a30906871da3460be6114faf230832cac5f8441e452dcec7e76d494afa56021898b4103be9602648cf486eebe" },
                { "cak", "19060c9cf3d9162f2f4721e11551ec022d5adaf413b93230803941756e2c70a9dbb5ce5b6eb96b4f7f544da5dcb022b45cd4ccacbd62b0125e5df135c1863184" },
                { "cs", "d1f1e3882c189bc95f82beb74fa6e164c2765a600344805500b51b8024e9faf20719c1b7f15e8fa1960b5ce9a83105f0d7282cb6e037d13d022bb274d4d5b793" },
                { "cy", "630b536c2ec90299b0afbf0b92e67a6a2f53ad1c6a309bca930476280dc817c062d833a7eca1d789217a1903a9bc1bb549378df3f50d27fac0c79170e02e61af" },
                { "da", "fdfc6c5520f440d0ca2e5a224a028c887f4d66266a856bfa96f42a27a26cf759e413ef34fa2cfa097825574eac990c6e199d5d57a6720d1f60715d1a2d872830" },
                { "de", "6fada1695b8e5c084afb653f39cd495111e5dae144a7790378874060a4d8f06cb417a5ab3e104e28f6eccd0fcc6e246aade6a11ae3162bc29abadbd7cb76844e" },
                { "dsb", "08812bd6ceeef974f77f90271f9087f51fa15060f63372e9cf41437621d28f42ef6bd08d049608b06ccce01bdb5cd1fd281749bc268ffec7a38a83b6460eaeca" },
                { "el", "868c436495210eaf5e08a792536d1651b46046b27cd3da2ff6f047a4aaf8bba68d9b9380f1a593eaedaf63716e85b4f3a5d4941eae29a446ed7b55980663e9e7" },
                { "en-CA", "3232c527b083a8a98db94abf104390f7cd1d462e5b054449e13d207005977065b2c91aa7699cf15e9584b86d65a030c11466f8e79bc55e1a85c7b66589b7f192" },
                { "en-GB", "3243445414294de748696ddeeaef4660e3094c21083b5150770cfbf379d15954c509eb92c1cb6a9cdb3213ad6df75fc7a964b6370ab797e7b290b08b85e128b6" },
                { "en-US", "bda67955f88b57ba2e425444bba7f619f2cb175d3cd05224942213296f08ab773ced3fa2a68bebb70649b6b12c33e91534f346d81d545483d97e70316c980f87" },
                { "eo", "ca41291ad9e8c8c64639d1874dc188dad176b0071bebcf0bd13a518beef0b63fa484e2e0db4b276c637bd9fd984d787d1a5b88d2360be251754c37e7e76e7781" },
                { "es-AR", "a00fc2940aa44847138a1a86858b828b211a31881238df6db17ab48221e9086a0c42257d8d1de8fc488c58c318da54d83a27a942f3a6af6919d88bee9cf2a627" },
                { "es-CL", "3ce370196087c87bb6b4e4409cc7bb91ff1572e54f6414fda6b55c42170dee80179f4ad6a7d67ffc2fb427c2c29d4645821472f5bcbce9a7d9e38c3eb9b34d96" },
                { "es-ES", "f060ce8d98a3d51605c77c6e3f15ec1b81ecd2cef8a5b293ee71a3a95c007ace9c09ec930f806a4f825b5c674cd24bb5f32f54969cf9812c7d294b8488b94117" },
                { "es-MX", "4c9c3ed6116c8914cb980c97f501b4b6af0199c92119b1375f78091dd96e0697c7086cca51286a5b0600894e423e6a1a461b393fdd733939848f33516e6f57de" },
                { "et", "7b4f444a007084d4616ccdfdd8813290301f4774c332ad0be70d8975d7311f1507dfe2e18f9b671cc021e7bc9afe1ff2e43ae1cc4a71bd0930b3451235ec8276" },
                { "eu", "e80c74b5dea0c3fb96c01c27c2e8d653bfa555c5b7ec79d5a610a05c036d64d2ff0c5cb9d8df7a93bcc5456279db65794a6108bd4f7e2c85817672496ed762cc" },
                { "fa", "627412c64cb3cb60d38b52fd12a61335689d63ef5b286c174fe09488ef4d06af3e41a441e1b41d2846eb2f26fb99718889d8de2f3daeb69852666eaa4e61e153" },
                { "ff", "888571d933958a81dddc1fa6ccca920548858462fae367e1c4ebd69d014f36f1619763f7cf3468732939d4eef200b2728275dd5df5a5430a7408910a355d193e" },
                { "fi", "fc154a43443047e104543090316f4cdc9811340075b3e93b3f89d4ad6e2f2602bf49c6670aaf4fe7c432ffd135f54526192d19b36ff868e21c8afa6041686d2d" },
                { "fr", "f53ff977109778cb5f8e736d2f85b084cdddedeab59b66e3e429d492d6c3525dee5779f482bb4b8fd2b48396b51dc007f7e756ab079a749977a3d2a2c1092900" },
                { "fy-NL", "87f8a0070db32652829b380b67b6d79b5456a1f455f7d37502a69ba289c7e98b8b5b07e129a12bece779d1c19bd70cee015aa272fffc2c49f38159f620592701" },
                { "ga-IE", "cb00dc5ed15f52836f4a86565c399090405041d52565a12006d131f40bb51121e513ace0a5df24692de9ea2feefc086889f7c0edbd7d19463ed51fa856b9f056" },
                { "gd", "b2989ff556efd88493b45b4d26198b3cf513b0169fda6b2d87d5b8871670e7ea06eb7e02ffe5b0bca0cd4af612077f13b7ebe503b35c72ef565c485f8b898ee0" },
                { "gl", "322f31ad6b50dfcb44d7ea05f8e0e20d064ee60682825bdc3e16acfcd72a4800c25138a99469ece2e2d4f7fc7e1691b80ea001b7d433c639c252a79a45bc56f1" },
                { "gn", "3e0e63948851024fc39f96930aaeea303a20eb28bc351fd27a4e95252029c050fde18a7963db7ed391bf397a8fb77ec293541aae37ae06411e5a38a75924f2fe" },
                { "gu-IN", "c56e39871141be31d0a629540fc67582578fb80109b45d2d6acfa6cde16537654ce794dac7c96fd0aaf32505f2afcc511c774402a9d9160f12de8127b4d10593" },
                { "he", "80a99ef3b20364de1cacc15162d0e74e42e55a33978517ee13ddff85c69a8235a9e2234f4d467ebaa3f4d02cfead6481130afaf808b1b810360cd5f49429d3ae" },
                { "hi-IN", "884aa823d12c7f323efbe208eb91d6221b35112f0e90b90ecb057d291773acb5078bbe7dfb03e456e24f6b54270692f4ec3483cb820a37d1031298e634aef1a5" },
                { "hr", "f8e1771350d995416a68d68f6cdb828fa5d663bfc62d72f51b8b57deb5514cdcfa94735ad6c2defef5b4603e68304e921f866b1f460f464215eff249649663bc" },
                { "hsb", "843fd11ad525e73e506ca1314cc903d430c5f4a5314c6aa38a72eeb9da6db18bc4179b84a83a115e3b678b64e465251774b52af42274de56abd6c846d60ee09f" },
                { "hu", "e13061f31be83fd754062ccd1a37e2c0e7287868f8e96584912935a83a72ee00c3452e8555ae75fe5b63319b23968d218bd09e68008e3878c61b6f823e9daa36" },
                { "hy-AM", "8d59e686f338e7cdaab51d9b5eb6c1c29a63fd43937da4e49ae751cd35657a69a3d929a6666ce66c1daa64901646eae3cdaaeb71a1315aab3f7176678bfb723a" },
                { "ia", "edaf5f3f6137e235fb17362ee4d415fe19f11a2a1f4b1a3471939bc4029438550eacc55f5f0af2ff2c8f23b8e4229db5b53355de406e66ce55de7c02ca289730" },
                { "id", "b2f26fb5ad47f39a5eda3d5147fc37e7c4725f77c79831b312ba1ae246ef25b46ad7d3a7ffc320dfc95aeaf6277fc99d18def73f5d41c3c87312feef7c79476b" },
                { "is", "d6dbae4ba3ca4940a3bbd469c52fee48e11d0266f0b6e8e6d8f8052d4ba637ecd29c5c12ae38255f46f1045c47e2ba460344f641ada0344380d70deb0596944c" },
                { "it", "40d9e06a99d60a3998bb3a7f1fcaf67b7eff13ce73cf79814b7fe78f9298bc2e832c3052fc82aed9e6c9001d8d2629cb325ac132edcf17919fb62e0bc4bbb74e" },
                { "ja", "c4678c658b92f6654ad85b15cb841c0a711b3ea775bedc7ab3dc55026a811a2decb3c74030543d1e71bc34fc51d38d7e2c00316963efe4a7d8cfb47acca778e7" },
                { "ka", "ae7f5042ac6539d5faae1b9dae98c66a938be21dcfff364dcdb740c90421326741a11fce0e7ea4e7c927b3a13d40dc671f1a5d3d7baf0f93afa7b4f590472699" },
                { "kab", "23c88c7629ee11cbb9098c80980d99f858f684ea6846695a6dfb3ee1247692fc30238ffc4d6794c7aae32a466031c692ee90a3c3f060b56d27de7aaf439e8ad5" },
                { "kk", "f3710096bca701929da9549fdb8d9373564736e4beeb068a8f3e64a410ce8be4b08cb0b2f0c5a004347d7a4fea1aa27fb69a9de6fe9cc9a03074271ceba09f13" },
                { "km", "62aee1879a60aa98b76e280b0c294b70b49d5221558acb6bfcafae701cf6f273551e48ea9b8954872be8557e26bfd4edf955319fd0f91e7b95e3c4cabe7259c7" },
                { "kn", "eac04629bd6c0e410b86990690ed49e5de5ac4b414b5630c739a9f49d922673fb8e0a40ab1f71614dd2731300469b09da4a7d5fe65bc6ffea55db584704a4df8" },
                { "ko", "cb746b364f50767c29d9cb7fc30dbf05a29012669ab937fe5bfad5910c4daf4bd20ac8b309c5949620aeeff832e7f1de2dcab34055bc5c2e38370269922357be" },
                { "lij", "5994d85cd1e5c90f87bf7b82a0b5d0bd4212f7fd86d266ace283162a58d5218583fb1f5b406640306b301f012d4206da25f3e155b40697aebe90cae549bd9904" },
                { "lt", "6f00da19287b8fd957123e84c57b25604fc3f76e0df49729233a0d1c081a350ba6486c76446a4f59aa99caac6914aa00429f22aba36f986344b51f45d61e4c4d" },
                { "lv", "887eef0ea0895cd6b137d505dc56e5470e15020c367de78349f2e6bdcbf5323427c865fcacdb95dfa749a8c05380531fbb181a9bfc58ebc74f0caa4ad2bca16f" },
                { "mk", "42022f36ced6547f3c45bd4de64a9645d738ca8ba112d4390e06c627a1c3881fc21316538545b806706cec2353d89603cd2d4cc6017a5c2ab754c53e22b595b1" },
                { "mr", "cee26603e399387542286f42dc57314b4520758c6adbf770c0295b9f38228089cb6e02dd773b389cd364564aa10b9946cda77a1075a233eff8066ca1aa985f65" },
                { "ms", "7b8e432e834a476d49d1ea7a3572bf0f4b125914c9922d770936fdddfd03b39778376ba47eaa46e09310c24ba1bb2c0d3cee28c70776ce05eecfd42acdd185b7" },
                { "my", "ea34e8055eb727b0904ca0386674499cfd791b1e35e1fa2f8415f17db4a5c16df3176aa1dbc684eea1130e6a4557dc1c6183d2a476c418a30ed50aef835c7cd2" },
                { "nb-NO", "32df7043c4f1564137c175d9c39e28cede044787d948492f5a457f6defcc3692b16296423296a0d763851e43d9ea599db0e78d5d78d7776e57ca6d4b84ccc032" },
                { "ne-NP", "10e9ff65af87129932dadc024f849002d8ddaa3259cc3e2ad6c5a31e2a56e6a6cda77a50fc3ede598a52487f6235a1094683aa2896ed85c1eaac27811d8dd103" },
                { "nl", "f149a2ca199a66f3fd9650c85ba4143d1fac05293e58d52c16cb3a1473a3bdeaf35ac17ff6ca553f4c9b7c3f1707a7cfa88bdb20bcf674184f550488b1bd4678" },
                { "nn-NO", "2803f41110456d604a7d7cf24ae8536cecf4008f18c784ff3d92febac7dc2ad81bc19bb6a3b6480bc3822fe0c00ac186b9add53ef0ed176de0b5c98f0b467563" },
                { "oc", "60950c37b3530fe02be879603b2eb6c0c5e7679fb32efd67a415ece62f6fe6231b7fbcde204685629eab2e111d0462dc48f3011cfad77515e154547eb658a814" },
                { "pa-IN", "97846090cdf8584c3a6f643cddaf86adbe97112513df859c5b101396c5c1ab0e5160b6b95c27a289781b41c16d06f4c39c4b2dc3f4fe316a706423bcb9e42678" },
                { "pl", "c94ce98cd3f766551007eab5e48c90e3f77b5c75d86a3a1971c034bc897cec3252dc27fd102cde7f526f32cf3c342f3cdc8ebae33f3f18a9e31c9d6ad2cec00d" },
                { "pt-BR", "2031a0a980b5f15d56286eaee29d69a617eeb5be62fe308fcf66eb4b3dac853a8bd36a9f897c8eaf7d902875b24444f1b625aa52f97f67932890818c8f2fca0d" },
                { "pt-PT", "01468d65588aed52ee074641759c8b2e97cafe1c52d127d0d4dfc250740fc2ec50e171d947819823fb0c2e23d8c93637cb7f262f461994efd4e131e57391e1dc" },
                { "rm", "d7337f3997947a972c99e733fcf5e8c51514ec0e7237cb6f043c258eb3f32730c532055a2d3d6ffe5b0731ad4e18d43ea430314a40b1cd625512c4d7e9e26203" },
                { "ro", "c6890f8a33036aa733bede5427c5b9a600d310cbb784fa255012c4f95f7166d8bb811ed2719404495c0e82807f8fb92a3bcf6d53512a5ab5a2756664d3d7f50d" },
                { "ru", "d2beace746cf9e20e24b03475fdb7fed9f786f173cfe5a81aac398d403eaae3292fddb0fa2b61cf43e6686695ab479b3cde93257523f9180287ca3883122c81f" },
                { "sco", "15a7ba371d926fdc97622f676523874646bb5581b69ac014f5c3d919fcafc95df807d4450d3dd172c8ec9b4a9a2c614fab7984b2a767997841d1e4e5ed9c5de2" },
                { "si", "093939f1ef8ed9aa2bfce364562c8c4737dea0e99162d074eaa5836991883edbe9f019ca02c87a0f645cdaaa6663adeb1cc4e226dbd26436f40dff28ea5dc9b2" },
                { "sk", "788195a42784d65f0ba062c36b8b44f1e707e6e7e3266d8f5be690ad664fa69a96f63916af1c36310192338fb4ed10530b80462f38543680c902efccb938d2b2" },
                { "sl", "e8cf2d98a4e79e372206e11424f2714d49e6321b01fbe095f127b1b8a052e85cf57e07111692676ca6316510822c5b2be128b98d1384fc4b7f59b86485100fad" },
                { "son", "34dafffe6914e7cfba2cc0bd658a794a96330329de931d18299fb82549c093a6d80cee2d0af8413c2ef65f4673c79e465f91ed919b64b0819ab971d48b94c20c" },
                { "sq", "035f9e5196106f095681d2f73d81dda29a7f4593867857b15609a59f222fce4033c365a5f4cc3de003f0deaf9d9ff8fb3fca6ef003736da6818bbeb3efc1d65b" },
                { "sr", "fbfbd286588d0db38774fcaef6d1e480fba9d82d3080cfb4b4df0e47fe7ec9f93f3bb439af4da3a955e44c48c3c2641b9aadd85faf30e8e0ee290e570dde60fa" },
                { "sv-SE", "533ec3d22c3529e2cedf23127d358b79ce867c3a988db17580b0e5a2ebd7c38077fea03a4e8b1839a12d0d92a3d1ec1ad3471dfc2dc375cc35cd382f6d1b14e4" },
                { "szl", "b45377c937ad8b1eef30a995ecc28261c0af3caeca9150208c32b8e2a1cf2151b1afe484b1917feefd3f9390aa032f31df72f70393ff54aaf4d66e761dd3fe70" },
                { "ta", "37396e65e1fbdd70cad071dc38bb722eb56ab78fc566390048d1c4fe74a6af61f39c899b9fc4d1d363e6872ba9f4c67f736fe5170e7b241c370c32920d0f5dff" },
                { "te", "118c8b64185976a388c2039db4a9623cd245ef3f00e40bdc0390e1e241d5b2fbf059a0049f3af0096b9d5bd5127edd6d125ddc63a67ceb8962629739ff6eee54" },
                { "th", "a2991917d920e3210120efa1c2e6b411da7976fbcec4cc139ad2bd7a8bec0414f1a9d02cdfb6bc1d8a9c1b4e51d67aa92c75ff1790faa21f44f4a4a1affd2f1d" },
                { "tl", "2ada0cf35b1f67511c3e1ffa0862f66f7f25ff6a3ea951317e2ec7804ca155715fc5a900df1bbd2b5766d7a56d9826115b777e418285a72a190d0b36017d712c" },
                { "tr", "40e09005a5c4b1757c059bf7da6f60c22b2312bda94e9750e0a0a416e79754ecebad4531dc12ad83d5babcae0f78abe2a9b8be3be1b86a196edd617b68614bd9" },
                { "trs", "c11b57a6d0255cd656faf431067b089cb122ad33cec96c878e194a469546655df011ced1faa3da53d02ac1b894ad43475ddced23bdf116036dca9a6efe611d80" },
                { "uk", "4ee7fef7b60d7e3dc00ee0cebfa41125c56c82adde1853a7a576a2cf5fbc502bf12ced591fac5342469c82144e0d54a6a95cd90bfcfaeec22813ed77c93f5bb2" },
                { "ur", "911e99da0fa29bc8dcde1afe822c3a1be79c46990e32f0c8974afcc0f83eadcdb434836c6f4df9a737db414fc0d2f41e14a0156d78d1fdf2f7ee7c58eb540e6c" },
                { "uz", "88a8f271eb8a1dbf4fcfa662e2ffb6174c6288f09728a2417dde91a526a53bffa514c08d8b1028a98d117df7bd9ed4d67e49225e2efa577b7c1e31b67346aecc" },
                { "vi", "e67f47863035beb87a656d21ec16c69b24d40d783b353828624ad626a35647e796c2b59fd90f5dafac283ed214980cf1186265a070430f4a9ac587bc3416ee3b" },
                { "xh", "2511af2fd423271a782d6d2ca2a092c4677c9f87bd91e61eb0fe2151261f4e857329c915fab2cf71c4f90b449cbd4d810107d644970a28c541200ac95abbc045" },
                { "zh-CN", "5fee911ab8cde9272ebf4fa5f3e90a202f8bd81a1be319455014814c26d6143a39812ea12e2e48fd6ad0e928b8de4208a8f140d1c99fb2a9a3243e810bb8ed8f" },
                { "zh-TW", "5be7fdddea7a931163ee63f9e1ccddcb2b11720a58b4a2cdb452ffda0f7a9fa21e173592103a358a7e4c315f3243da1a36067ddb5d944d28461fd2c0a1560222" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/99.0b2/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "5907ad49f138edcc8b15c58ae7ca8e326ca3c9122047df090b931a8cf4f1d9421ddbffef4706afdecdad2cecb6b48b39865d578ac3adcf9aa502b8923f9d1e81" },
                { "af", "1f3bddbe5fa7f9d4e1d1373ff33b6a6c9e3124287d4d1b818b0bfdd5f4f477d57f6032b561f6db8300f7e20a20d825ddcbd2dcd109a2b519ed7a1140c52a26c3" },
                { "an", "bcdaadf6ecf999c3e75445fdddb6ab99caeff818c3bb00b032ede26b20dc873e7b7d616d199cfdb2516eea7236de1de23d80668d2f9464c2761845aba806c520" },
                { "ar", "e1a6873036f4ba9885451bd5ee8847feecc8ce2344a6f90836aebdc843b083ee71b2d11120a24037ecbd8363afe9670d21f05629181b6c5d08ccafb74143ddf6" },
                { "ast", "1e45a3668138b12ba743da972b5c2c6e71135136b5e517552cd4c9ba0f28e1d98a35c2dee6b6fde2c2ffddf2271a691c6df8a4a6be1d44746b5ef2ee8a82158d" },
                { "az", "c297f91430e765c54206f02166777329630d028b234ade98a94e3917088fff2dd103ea6fbb3586c08f1f68166271dae8e236ae213c1a9c3b55560c5f30227019" },
                { "be", "1fd51b4c2598790bfd8f4382caa3ee1cc1f4a9e5cb68d9d85bb13ab4034aa73ce54981c8495b29f297d3c17bf800c6354e76a8f223bbeac78830aeedf8102912" },
                { "bg", "91851314ddfb1f3052cfdd6102ecc1a8a5bdcff0859da0371e675b2fc09a1dc95c3c58bddfb6dba1f8e005f30671d68b021c69b910397e16b8e994ca9cad780f" },
                { "bn", "91cd0ee45a82a73591bc5ef15224289bf9dcf9a34d5167efdefa93e4225d84316073e35e24e2e1cc1d42db8afa0657e14f457d87890b2bb5186f8e070ae13348" },
                { "br", "dfc51385570ee2317fe155431cd66c8e5dd8404bb1531da56c6dc1d4737c46f06247649e320b1652c11b664d1c8ebff0b78647f35c348487358d177f73cd6443" },
                { "bs", "1bc25df96b9b27bb361c827c0ee47f9cfd2b3bfa406d08ccc8e2c508674db9e5a29a61f7ab84b838a1fa23877a5833726420701d8cd5a8f6e37bf9f1a4cbea77" },
                { "ca", "665bd67de36657ecd1ca7977e61acc15490386d31c0f9c88d1ef785b7d40b90603dd8d151620c4c01c7c3c60f7c677fdbd453794e4f4fc72975ff12b8c051091" },
                { "cak", "58871b178e8467f39cd15e6f534aa9c046b51d64c9c31f50abd53e6b273ae1a8e2563eb8b64b41b1a7d4e29b726d5df750968a51a67fd071fac0f1e30cb92ef4" },
                { "cs", "cf78a1ba500537194f6e682cec94661e859f01fbe7ba4ec6be281b688cf4722966e75e81487e19e1d120db7fcb192905273d77dbe9343619fb98afa568daad48" },
                { "cy", "a571a78e3b03abfc9bda129c80fe9a39f76e77f445d09a23718e6515c72e86cbc621fa82c485ab821055195894db3e589ece332e06c35e14966f6a5137f54435" },
                { "da", "4b2b3c91fbfbfde5ff418ba266b56dd81bc34cc2fab3395af856afd2097600cc16745148449f30800cd407535ddd1c92f7911e55274d602513cdf4f0c351958b" },
                { "de", "9e64ab929c83f7fda489fe61162628b819687a0156ecc8d4a07b6ccc626b7b9beaa79422f5b9cd40b5eb06bc4a3617755d775030daee93c845d4681571936abd" },
                { "dsb", "438387abb8d17d6420188ad8601c68127fe3af744393ae3b31142741e274aa08ecca6be2ca2f9441ebc4bbcf4b4fc568a8e128ea8cfe957637f6f256c7fb3d5d" },
                { "el", "f988e13bb544bcb3c81916d2a3f416d4802253d6061f6ce72a41ba1aadbb0de95b50003372136b7a6f27edd84c19a03ea3d5f9373e8427b5cefac538873b87c0" },
                { "en-CA", "79c2656e86a2eec88d6fab9e0cea0035f56d757c0635903fff140cc7a98dbbb58a189fa1fe7213a8bd800531070b893518482c2deb97a37cab6368f9ae589cba" },
                { "en-GB", "c78a95b06eba6d5006b8645f640afc08f232d420bbe9c6e6f1f4a35168ae0e064af2504c3d02c94750a983e846c3f9f6d53c7da5a563dfa755a5d609d4610e4d" },
                { "en-US", "cbb60fb73dc489c14cde77a2b92bf61286f3d822cd1da6a3f8e3ceee620e8533a27edc409d414c0627e13c2310eaa21bd509c045473d3c063ba5b049abbfdecb" },
                { "eo", "0476a621ecec6841867bfd6f7eec86676356fdc37f45a9bad99344e427af774643471d4b5f870c4146402fa1bbad65fd680d61804535c4ffca24060b1a1f70d7" },
                { "es-AR", "7acda1972aabf220ec3fbdc0606d16544206c243315b3728c64b7aa8cdea94b948b964eaf11516982adfce02580e1236763670b4b4761017b42158613d310247" },
                { "es-CL", "27fb0bde15748afe0a3d9dbceda3151bc08ff1a0dae27308e04f44e20c0cd243a15cf2215f89fb5656738d9a355f87bc874cb43fc64b6f6dcacb3f0439e9cc49" },
                { "es-ES", "9c5d395c41a7a2cc6c3daf5602f7854ce76d3a7771484c9f6f1deb445badb84ca4b2176d4f4cd7f616ac5e02e44cea36ee9dad5040acfb8cdbcdf6372886dc75" },
                { "es-MX", "f7127f652aa3459eab6dd58d9c3c0b20fa0e388a34900c88ac060f03b2170824881592344a0de7b227580859a39508f5b68fcd67a3a4c839c07bc00f3538d5a6" },
                { "et", "323954d68b227368d6c431981e3345fd4aa896aa73d038ef3e49852de4b711a0d7b03dedcc57791e62ff0574de70bab72dc1cd3c43811ac37067803e0f10bf17" },
                { "eu", "e7a27cd406eeb99653332533fe89566fdaf92f1a5a56f745f575fcb07daa1259700a3c4c262d774af86097f15dd4b070a31895d4068f452158debc2135aab58d" },
                { "fa", "4feda18fe4623a33aef030de0c510297233311e5a512ab36da079a930c63e95146cb0145b23da3640feb15b546660bd0bf187a51292902b90084a3eda8f168d9" },
                { "ff", "cc22ef62e9721135befede211dd5302c00273b9dd3c3a2824ce87cad656ba8e67b630a90478d118e2f8f41ebb3ef81c527724307d1e7ec291288f1265e81e759" },
                { "fi", "df6b3169ea7326f26506794f516ddd700fd6ac5e0c17158cb848abf6c7c7b95711384fee1b315965b62fc4396abe4505d122bd795e5d80e3e7495d5b14dc7795" },
                { "fr", "5ab60f8f2b0a233f97f0dd3d0cc5a9addeef47f72d1a01d90499dec9dca9b09a79fa9e2489ceb130a0f1f31eda993634d0960f823f4e27ea55778f1cbf048326" },
                { "fy-NL", "a90770bbb74f35eff3d202337c0a1f7556048d301908bdd9a7afa180a2f37356d6ce726fca74edd1d474e342f1110c6e34bc5e05a641b19827e2ab697884d746" },
                { "ga-IE", "df178c5d1567a155e914d8092b993e8e7d25cd8f7f07026c0ac488bdde28c3f9c5988d5ee72046016aca76825bb90527b13ee1730ace5f76dc59d3d877b633ec" },
                { "gd", "b12d6c0ea9733192ef71bf6b0a7c3b258bad037e108ac71ea732db66d2c09bb09072e69016d3a7c6d34a68b0dad5e857f859bdfe077e378f8b0d03e71c2eb78e" },
                { "gl", "913509be2e7360956eb7d6094d376998e1280ce14f34365bb6ee7b4ee69dd3482675069f9602be73bca012eccdec9c0080ce866548071716bcc342c4b02abb2b" },
                { "gn", "1108bcaeddfb16c37426de41bcd636531aa009dfffd861b6ae7f72ec1aa53a95a9cd4d83a7d78f74c560a45ef90c7592e8d0a3f1da4b500cc2e83506615b83f6" },
                { "gu-IN", "318247fd4b804ce19464fed34988f057627ce11a2207cc8662c96cbd4f0632aa147f2ae00470e761b39e885cd9c71c1296d7aedd6e74fef54dfe70a2e4dd8894" },
                { "he", "ca6bc197a45c1381883635ee3c0c6a3e6de11061ae7241d34358e8bedeed106964705e3b31f078b2fb8612703940d18ecdd91eafd5ced5048f6ab4747497a819" },
                { "hi-IN", "0a40e9e0b2af3b2d745fc815a25bee1406df32b71a3b7aab14296e71896353ad758a8b58ca693a74fb1eef49ef0a0173a3174b46397641f719488a23fb64bd71" },
                { "hr", "898436f00580575491e45a7cd1ad2887092babffdee39c13cba3cbf1a990ec07697f9dfbf11a706baf4116d0e28440f3a87f31cf17b1160a36edc2918f3c7393" },
                { "hsb", "9e6d975e1693c33755cddb0eeafd23a6f51b4a9b446575977cff57332a1fc85edab638b41f547420bb8f3855cf016f5a2f517235b91217e0fdadab59eda3d9e3" },
                { "hu", "918bd517ddbcb6d3f78b44712cc0f9e36acaecf2f407a1dca38f6563331f1472c8107a1d017e528c88bf566ab5e42ecc112f299f20b492a09f525ea7b3799cc2" },
                { "hy-AM", "7733a9bc80daba621d083eaa3396d0554c67bda09f401eb5b2948c76c303e46d00c9de3f9c3477887cec388638ac2e9fa457bbd565757501b08a9795f760f427" },
                { "ia", "5f19e49613a775a3875adee7d5889ebdf49a5c7d2a33ab34cf90128e43008d5d821145fa441947cfe79ba28e1772918cb2cf7c33775a0c09a896d63edd5c985f" },
                { "id", "375f30426aa17a7c842e0df747ea5bdd903bb7085d79d5194a7a712358b285c21de833ed5a480b65ad8e89c386cae042ba61b7aeb4c59d49c9deb7b0d7766197" },
                { "is", "ec1244235a4ed617be9428d734f9dc951c538a0b089cd2dce8a4b48eaada98ff814488e175d4f00fe18489037f869b0821bc470d809d1681197db2bdb9c35fb4" },
                { "it", "6583d1d6f8dc2600ff0761877cfeb9be401130e7717e24585e4f42bb3da139b4a8e16cf906fb3c3790e660c87550e7d75a3eb6aea4664c6841433d878ef84bbf" },
                { "ja", "73ccf75e4e00775957a58cb355bd99199e2e26292251df7cbb91f3770cc2d56c5dee47c8ace872b357f95586c20cac067398e2e3917bea23b0b64317b15a83d8" },
                { "ka", "6962b579ff9b3188ba32ac849d3a8ae51d3762b6c83dcc94053e45dffcfd2a1f3cd72a0e3ee23285e3e360e9f3caad807d0847b4b764cbb9ed316207dfdbfc40" },
                { "kab", "dda555a70db5619d8867d61d3cc8f8d4399d98143f3fead035b81c9812a0ca7a82a6b377d9634b16bc87d74dc056541bb845e84b0fa31562c2383c6c0977224b" },
                { "kk", "b815e6071e31d1726819a9e9ee33390de5553b0f5f59fc141c17aa535d8284287a5ca3ca75b940b5713c68419f9b25c3f412a0931efad52946a439e2440740b3" },
                { "km", "d7b8ddef9cf626406fda343a2f8918a84bc4cb19b8b378e4edc4e9a3622d6b075009f7971a6f65f314c3acfda93646e92018c9838ff5aae21556cb5d91792aa5" },
                { "kn", "d623e5a2bb0261a13b90c9a7ae16703c086628509a12675a591789080cb15fc11fb028e1fdadc521410bdf91141439b6c749b34fccb0f9eab6105b5c00bebd34" },
                { "ko", "35e4ea8980b72b2d7bc67fe8b4c0e50ad0bd5df97b5e8f3eb7c1135a8345d21bda9096dba32460fe4e3e283ee6186055b64b6fe44ec5bc21180a97261da86155" },
                { "lij", "af88d5dc808a5b939db1a737f9d0f20560dddc9b629872ba1ebd8ba1a50b90bc7578cfb9a90b60e914d3ce9f440426cdef76f918b22f66841c7a779f04610158" },
                { "lt", "e03ddc1fdd12494f7d5a5b5987509dde6b33a9ad36ac583c5c61e6ead1dfe2a918bd2a1f2d4e1a9db27bea588d073629078202382d60e6c11ddcc1ddd3aed891" },
                { "lv", "d6458bac491337484ebb56bbb88269190ecd05c30835ba36e952f782480d1ff33789d718ebdd6688be012e3d33ab823ad777250769be0d840c5292b88f2cf813" },
                { "mk", "1bec31b0ac05271eb05b8926e3c3a4313a80a4d8892222948ca44358ed41acfad2cbd7091914c3a9e27a8618475a3206ce61daa80b0fa28fba6e877899f2bc88" },
                { "mr", "3e120c75d4195d50d2ccfa88c1a588de62e290818e31b8247f61bf9b2efd0cb49adac8ee4eaf573b300d7b1a9f4db080e82777529e596040aebc499b2a235417" },
                { "ms", "5cfc78977c52a05f5c76cbd58895bc8ca052fb42baed1a3bc5233ae2b738454020558b48a57102e92ff41aeb8e2289121afe9c77ebcca1e8c8b943a1313e25f2" },
                { "my", "004a458c6ccecddcc8567ea9a207ddd26aad66c8462c0f92719094fba43ee5cea66d1b53b83822e36d0098b20ae57f3efb12e906b87878fc59cbc419fcc75288" },
                { "nb-NO", "2ba51e54ba8d5277ea211d29ce38acac20f879eb953a9831894e5d2f808090fabc677360206c9193f73139b8a880bb6f4c88d6862cba749511ad16b422994302" },
                { "ne-NP", "2d46491b1791d47aa690da89348fef257b19273fd9ce563d196d54f9f7c8ee76f4e72c8b6120a28385699971f9ca91a959695d98be6fa6f4f17e7628f20d81fa" },
                { "nl", "009d7bd9dd18744f28cccfa9b140bcb6947633c659f34994ffc06a7b313ae410e3b309f13c9ed3ad6b6eda1e26453bb891a7906ddd21bbf51e0771544d61d4c8" },
                { "nn-NO", "cd798a8b54926284cb6c0905c869f8bf2011691820b373d94c6aa76b4e10e9919d3522703cf7f0e697dbb251010e8224f4e5bc520383f13f4428f49f8da15809" },
                { "oc", "491301b6ffb122b5c21bbeba23709e6f0d33c119b8b991fd9f2e52c61fd87409b9e6901ae0a7be920829eaa178534a3fbf67e7cb0296243ad19628c2f09d7751" },
                { "pa-IN", "ad5557773a0ace0122e3084b3eee69a068cd8d740e2fa66d0d6a85005f65f0e38987046ee4ab0299fd0ae81475c036235a5280391d9c7863f4f62884641d57f9" },
                { "pl", "a80acb073d395fd90a0c4c12bf9691eaec8eb2fef513c3f7e16f1b7b0c1af0be1be21d7389ff9791d7fe873b4c8c6f9f8f568d7578beece1d7b805824649aef9" },
                { "pt-BR", "e459a5dcc541e41b1fa19e4850e628dc333a880b61b3c127ce262663f1b4cd63d254e85f7ffc5c7aba6e776150c3050d9ca124f40e23d827b17a3acbc455c6dd" },
                { "pt-PT", "7095aba6e68bde2c6899e3990b59c467c6466247b6d3c7999c8a258bda23c30313e821f93195ff24685b51cb24f4d021f9f76d1d93383c970e6ffe3115cb7a0d" },
                { "rm", "cef84e569da83e39c7f0bd628aa64881702713fd29b423ef918dc6956857061675b0a3ab54973849a41d1e48e63f8f907efb3af3e7104d7b645880401ca2ed3e" },
                { "ro", "1293f36c5c344c0584b5e57b47d1213e9ffc7398b02fe5e5553dc390d743d656daa72b1e4066d168f8d562773d03e7f71638942292099681f3ef8bdf5e85971f" },
                { "ru", "b16acc319d3bc6d727f02ead5f3b8005075fb663faab66464465002e43ff48c8e8b633a28a3b316bba53aed535f72114681b758c53b4cddca432514a79c485c6" },
                { "sco", "8c2072fb539d88b99957cf6bc0b426dee096b20c0509105b868ad9f2591583f2d305dc7f232b39276205a16bfe4ed520b4031b5617cb42611dcd96957f0732b0" },
                { "si", "3dd156eab596dfa9943787151f6c47a258616ee705d134d086bace176d689681653ce4d601f0dbb6616b9f9fdb74b0e91aa5efbd0b1016df0b857555fbc053a6" },
                { "sk", "c052a0b90243dab9916c8f67a613a716686b57ae3f883bc6b10004b945693eee350667e21b1cb972ea3f0c5febda546e6ad6bd1b773515b651d7c18e71f6cf6c" },
                { "sl", "b23062a6f43c7bc25394b5034da5645a569300decfa4bcdfcfef0eae9262bd3afa2eabbc5721a0aa6e130362c0869504622881b6c6122da7efbc42aa1d85e50a" },
                { "son", "0b5b7d06ed604340c6bd4785b79183b35809dee007f6aab33531ed0ea9ed5ac884e33499224d76ae6aeecc3202d10f46e619f62da2649a40a3d84571767d888a" },
                { "sq", "0393f75f48a09f82dc2cfa00f2190bf1f9dcfe00203d617d7b0174eb3abf960c30b8f6b1ba23fef88ac97822ad738fe0557a686646cf0a72fbb4c30870818d7b" },
                { "sr", "451cb29c590f78172d40b07b1194ab30d1ab99d4a27676dc1dfd5d4a69c357e9e59ea4b52d68e4c2502ff3b7587bf077d298ac9c5effcebd89b0ab2a93310727" },
                { "sv-SE", "354581f23f46623fb72fe4e79c44d200bb971546b3b9dafdbbca09d81974f50f47f9b5c9dbcd69562de803b944a79b8a77d88ce602f3304926f434fa52e22bee" },
                { "szl", "c5fda7becea08999621ab0d36b5617e910a996e8b12142c902c0a99a8041a182b2e2803cfc051edcea346eda2b60bb5a8796152593b78964a6e7450083385696" },
                { "ta", "71971eadf587e76386d2ce7c95b1cf4e2b003578054ae330c43f85381a7b3afbc94ee398cab0a49c7b81d3fb3e8776aa19b8b41037fce6f88eb7607d7b42b739" },
                { "te", "62834350fadaa20304a42606bef08b0e87e22f713bd86eaf23b8c5b27d7b510a2425dee4e4adb3380b7ec2c2d836b9b89f8dcdaf2fa8fe5d6b4459a4daf50562" },
                { "th", "3c69916cc43d224097429917189884eeba6bba3480802a23919b4f7a19c88f04f2b0eb95ac6010696fe8bf2f7019223f2814057ede4aff9fbee679b9e5ffd651" },
                { "tl", "154f6628165249ba0ada2dd40f0143d6bc567ffc61a6be2bec8988863cce1acbd9cb06ea15f880924813ddba5c769052517f0f4cd2ec033ea0b08885a0959a35" },
                { "tr", "df3f24457d2bb517a18abfddbba540cb75a9d2c85656a68af3942b9b2d2bb7e0d643e0e4b4b87a9bb691f8e8621853aad22145a1e0df20c67b1fc56e406c3191" },
                { "trs", "fbcc46695d83c1da520b560f8d943ba73e3ac78b91d13f0ce07981d3a171337297520770b6b8008f616e04f179ca71958a993479cde789bbee5d4a82531eab93" },
                { "uk", "799ecad50fe7835b0fda185f47b4087cfd73ca3f94d1e3b570afbe28a6699f85d461bddbb7f26c351f60d543f053b2bffbbc0f44c5e3e4ea8b82a683502a86bd" },
                { "ur", "73315e32c6a7c05c8b14057827f280ae4e71c4be783375a3aee21f2a9f6cf1bbd563b7e8b9056f88bc0bedcc702606c8b679cd875d93387d42ad2b3cdac487ad" },
                { "uz", "dcc6d3bf18ac645e32129c9b23643964616adda6db8fb339d2a139047558518a6e253930199ae9583342a63f66014435be0bea45649286d2ebef7b405f1a4b85" },
                { "vi", "c34a157a4383140cf37772e6d1d711e337264198183999bac15c150e3f1dedb922d4377d9ea6d0c056a08d54d8124568820a63c3113111e4ab968dddf6a785b6" },
                { "xh", "8054a726fd39f719a1995d4858a6d67c54560ccf942320c976e22d8fe73d562992c5f86d21792953b8eff8339d77dbece803687ca15a17357b6545e57f979380" },
                { "zh-CN", "2a40bb6573102ea89e1468d7e739431b620021685455ebe958a90a0ba50aaef25e04079ea7db45dd72b94f0fbd8c0aae78b63dcc0fcadf5769f7acd9666dec48" },
                { "zh-TW", "5b4b32b6d2b73c7e6bab04c2b5a4565bf1f5585a1163461550df177221fd9cc0deca97d9c3ba86b1db6c7d712aae19a0779d6ce9b98e21dc2fada59c0acead16" }
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
