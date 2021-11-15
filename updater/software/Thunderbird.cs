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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
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
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/91.3.1/SHA512SUMS
            return new Dictionary<string, string>(65)
            {
                { "af", "7b613451604dd16cfe3d3ab9f62ac7e74bc9c0d5e16da9eb64f656eeedcf57bc4a5a99999ffe654b782fdd75e5f167808a63c629b4d414ff0557be89cb9b1c43" },
                { "ar", "a4d57cf68533100a44adcd0d359dc98c6d1de5b242e7d10583bcfc59269079e07aadda01ffb070c9adb2eb5c6040eadce673a753fc8d3c1adda3288571987d6c" },
                { "ast", "a763e81e87816afed41244b6876ae55d0313b8046eff2bec6d665dabee190ebe39df4673bd1488fab62ef19f8bf503aa59d95bedc672842665e221037c0edb09" },
                { "be", "efe97ee4cfdb68a5bf62fbd752bdfd372ccd42db0c2591ad427f391fed73658edc1c340efd6be9a8682187d4db46d2c0959abd11ee3ce9608243f734caf3aac1" },
                { "bg", "1264e5f47b821c251cb08c9ed56f97a17a4668b46ed95e8fc0bc2d46fb13003b4886ad1935f2c54fa19eea2687d63fb291441e8d8663d5a8f960dda976a7f51b" },
                { "br", "a40f0516c52a17b4bc6a9b197ef8dc5638ecc0a8f5a70811bd7e29230b7d6a88e6e346a0d497f70f8886ee2832bb64b8aecd8ed91f21fb3df5de6518ba6e6174" },
                { "ca", "eb3d4d6f0150fd4316b66ba1a0db31e53bcb6c75d1e40ba0e21f53b102b9dba62f9532ea4632f4166197a90192af91755019fd111e135b57fe73711166b43abf" },
                { "cak", "9e01e3d87e88b5755a6f4e3fe1b2737053b3784453870ca38dcc53a00b418e30337c24a2ebd3c20c8a66bd64df500ad21041e8e8cc95b2c1e54b82cc848705d5" },
                { "cs", "b379021abc999a7f4cc6a43c7a4ee0b818385d2111a995ac46ecf075c48ad2fb12108e96d70f4228bb6b91fd8643295e34e0d02cf4ad7b6a08c80188828046a6" },
                { "cy", "9c547c675b44709e671dbf048f85d95037096bd2f0572c5274fee096c9c8925604d89d1b5800be981f19955f38eefb3e1e901fad8261d0229b703ecceff0a500" },
                { "da", "e610e2a9bdce80bebecd173f9bcda94ca75fd8974d0cdb9ffe4a80dad70290da44b1e9b26a380dfa5c6c2b41a9781d95ac2c156d6740051fcd34536b794c9740" },
                { "de", "4b556f3be5588a14c3d97794ea269d9900f1c11466c1d8780445661278f2080c3febdbf282c0af44f090ee7d1bad27ac420233a0dedb3e9b5deea69192739e5b" },
                { "dsb", "e6ca94bcd1ff2330b62bcbce759dba459ed593a9268e92918730416b6cb648fad3708876f7012b0fc3a374eb72e5c9dd686fad2f6d13931e75f2d3da9c80c117" },
                { "el", "07da12ba81cc95fe63fe058ded36011a19818e30717abe131b79fbf8f04878b10741896121f08a7e54b85774cdbb5827b91682047c3f4959b3eff01af748a11a" },
                { "en-CA", "60fbd6ba7d8b7e95d5d38f94a1942c0a2a859bd745397c958be5de692f6066e16f983e4aa54caef2d66861386ca8bff26db86cf15d9c107d79e2be9650f6b1a8" },
                { "en-GB", "6223984de1c4cb7036ba1e0f3b8aa881ecc19fc4cd5958307614d793a4b3caa4b8ae80d623026f0fc295df87db5631949478f40fbca44c5e8d3cd07d35359af5" },
                { "en-US", "498d1203a8849cfbfcd43fb68a10ad2a63a2409acf8a0e1a1876ab95bff24d4734a1fb1a0f79c3cbb4d25ba1842877e86bf7d620ded4eeffe1c00715ce8ca837" },
                { "es-AR", "b5350392376968082b129305b1f8dce3e0cbcb5e252e38a60ca08b5a9c9b143c42345f44a74d56b5c791bbe23ec429b6c0d64b76d233eeea7a19c9f3f47da38b" },
                { "es-ES", "2184871a4aa5168c2ff4c315b605511e934440f73a4669794b4023e065866826ade35fc2c4c02f3ef185c060da787ff659db2ea18d515861102bb8bbd4aa26f7" },
                { "et", "3eb3bd863b364198eedea68e40f7bfbb2e28d0221f16ae71c150d494741ea6862c7d524fa7b255ca24ed89541a86cfd1bcab5f59b355f5c5e79da6267cc0a4b6" },
                { "eu", "3c18e8ec5b7cb40c5d5f665b0794ac8d883ee71bcda6360ce4988101c93fc26b952e372fe3413a94d928bae3644d649ebdf16e4a8eed3db0e6dbb5cd5449e2fe" },
                { "fi", "21a9adb176d1a9a2d1e043f51f1ff3cbb17f305474dafecbb929a83a2712913c777dcb031634d66aee5bc1f1e63af7e6bf43aa26534f0a6bb2bafb96e799c91b" },
                { "fr", "3dfa3963cde94f52a75f46be392aa42031e99fe024538dfa2a9688251c94413c41b642aace7bcb5bb00eb0a6edfa0c2d9ca3e308716201b757b3078c45a8ff86" },
                { "fy-NL", "1098cb6abc22cdd0126f0162508a86b07d37922deab0c30bed7e439d6ce001e62838b9650228ee1f677cf8dee7833c544b29d343f10395c54ba59ab929c51970" },
                { "ga-IE", "ed1173d41acf31fa79e61bf6c2f82e545591f5368ae8bd18218852fdd90b51d55697e1b815bfa06284cb6018a22bb5ad1943a97799cea221193dba88bd6362db" },
                { "gd", "b11919993c2596dcdcdccb4958d09bfac8f0e0193be02ea978a5b64ccbf40d3ce52cf9c4fdff8e5c262aed270a43e6d8692fd4673aa35a755a0933ae9130ac59" },
                { "gl", "a32c0fbd1cdbd12bccdd9b85dc51ee283c96153ed13198040f91f2fa1c58c89c393468cd8675b19fa621db99916125a95346321bcb0ae51410086032c5a59852" },
                { "he", "eb661db2a9e402569b852127ffef4aec305a07fcfd3b10a1c22da370ffafcb705a450fd87ad609361a57fef104e532508366333bc69df7b364e5ff99815b7ddb" },
                { "hr", "e5d7028489601706414dc1c13f80db83f164cb77c7f61e677b3d91bff9574d137399d945c9e91284576f2792ced41c960fec9051d07e4ad1035187ba24313771" },
                { "hsb", "28249056bc1d66e93cc644716e1d33296a3ea84c4161709adef831123a318fb5af6c88be23864df388fa7124e713e85449afa4ee7874a146cdd07e0aa86d60f6" },
                { "hu", "fe55f97d6313533b9c673035749507ab7b05f0ae90b09b2548992976e72558dae7ef0512b6fc8e167c5f83afae8249c9b563545589dc18220fa50f11e9eb7bc2" },
                { "hy-AM", "344574affa62fc03ad7e98d48a04d794d8502feddfe2b5cb25a96cb6c823c5785b3dcf3a0ed75c17a3ebf28b41fd833e6dc4c38eb79f90e5603caf7a4b736ce5" },
                { "id", "38bc116b4090bceeab126d05550e131445fdfb291216a6f0289916d6643049dc9161917830b5096ddd8d121f8693d88c9b01f33359bbc8f58e7cbf8fa9a507f8" },
                { "is", "f433fa55de7fa3a03752862c5a4e49a6ec7875e1b3f50978e682658923b572d2e542500f245b394a1d54ea16f1f1d7e6ebcd7abda8741402ef04f524cc87214f" },
                { "it", "c519d1a5b6fdfed95e8d9840d9010bc1f701842679cdfa1f0a21451b7e1ea76e78893a6a5625fdf7a6e357823522c21eb3f6067d872374ae2b490817fd144b94" },
                { "ja", "31fc35a5062cfe799e2f70efd47c274b982fceb07a5d3d1e9c5278f92640523d38b65e307c92dfcddf033453bee188d088cb7097e8d5a7c3ea7b91feea4ebf46" },
                { "ka", "2aba5c593584206d7960951de35879f799a6e1565d5a5d4f7c107151e84c3c35b46a21e08e0cc59c6f22e3d8aa0410d83e57aac386767cc718d2baf3873f26b8" },
                { "kab", "05570215d0315435d0ea18899eec32502a21c3a31cd25d0a50ad33693039806f29229c098461ca18e9eb4fd774e52203b66ef94c620cb9e0d3854f6638857d38" },
                { "kk", "ba0cf5345b75da01a081dee739d9d76857e80753f54683b88c3baf540815100689989cf06a9e6e3a6fe2afa681f3b8488247011ff6d7765b84d6f4c974b29001" },
                { "ko", "35456ecfc27c2e29b01d1cda8fd8d71e735baccca5f8013645773e1b9115a6512e16c7c1968944253444e89d6639cc71c25402f218f2d5fd840a6ba4e07081c5" },
                { "lt", "9b0cacb76367a8f29aab5ab9f4f74e24b84af58deaa52dc409fb8c98fa15e30dac1e85ab6e785b62c5221963ffe1767504db61ab025a531478efced76382057d" },
                { "lv", "6e9e1113cea25a9004409d7127d19e74fa382e1dbe8e4002c02d14ea90be8e9cf80ec77296bb2ae2c1807d79711d69be263c572df69658e63297e7ff666cd44e" },
                { "ms", "cf1ab2906f53379884f5d0bdb4c9a5676cdaab07fdfc1f171730da78837292e2eeba1379c5b2de021294d4a605f5af943a08c0cfea88b571edb9495503fdd1b8" },
                { "nb-NO", "16cebd8215c87dde037123ab33fc13fc5dc34f6aedd7b33f214055dcbc87792eda2b16b2c56b695f7a3c34ff095b5c0ee9083d44eae0dba010dc911c867c2a69" },
                { "nl", "493a08e937d90c6c6d998a0921ea2d7749c746d7ca2d15ce8b1e499a357f01234fa389785c837973eb2dcc4f6b1af7c6e46178f8d714617b8003adda18ea40fa" },
                { "nn-NO", "6ac43f60431231c2721e8bd6b26efb5b50ff70c6654c7f2174782479ee9d05d80c5d3c7679033fe33ef75cff8eccdc966f1240ca19299eb411d3ba29f94fb397" },
                { "pa-IN", "0600db1ea8164ac55e329a913c1c6a1bae0c1aef8fb8e445736b0abc8c408b98c778fd8372e14e7ef9956364ef99cf6b4f66aff8f4486c50b9f29c7f09b2f286" },
                { "pl", "e11b8aed3293eadd1d81873ecc350973c90ea2536131cf8d0b789106044fd9d158e3a041a19cc209a2fc51467cfd665d6a359345e4ba5f61f9cd2b9c08c33623" },
                { "pt-BR", "f8e6797c5386e1d11119a89d6edf04620802d8d17952696537be6287687a52a731110e0a3d9b7ed84a3c9c4c417b0adf61f2f53f7c1c2a862a8c3e9729824d29" },
                { "pt-PT", "16dee263f75ed8b6344ef92432374a9f4fc71a6b6e91b1c45149c70133b97c92a6bfd790725b43b1f44535ff6c457d55853aea078585c00550417fc58187d85a" },
                { "rm", "839c026aa9a2495916fd15d136b5af4e660b9735caf97bc9af1ab9c8316c5963d1617612819d821a50cbbd8d5c48c4057ad292b0958ba9d843ebf37d066c86e2" },
                { "ro", "2456c2d3352593346fe3d2e315f069146c052bf80fb7451e992916f71760c022c6ae35a96a14d282018e30f78d956f2fb05143abb24c8fe9eeb0013bc2892161" },
                { "ru", "b1b10a1139e69b2297df02c13bfaf1b7ac76762dbbc56497f6c3b0dff426dd9a44ea6d233768760789b16c95f46aae99e67301801edbc062a4c00bf4f712c9bb" },
                { "sk", "43ce202cecd6b337205a5f5a7a34906f07e4f93ea65229418b13e9552be8a732c2117da778d5c1d4031982ddc23937c27094fc6f1878c37a1b49cb164c3b705b" },
                { "sl", "2d08f46abc3e2f0f0f349c21192d9df154b1e6cb76f844398e05eafebd32e90db061d4b96b857ef401e77a2dfb5907d71e94e16769821817246e840a108a2214" },
                { "sq", "7074138cc78d9acc178c82ce7c854ff2a4ac3aac4eaf7e07e9857513349c04a3f71a3c152c9b8aaaf0b33553bb3b7f60312d27e32b391aab787a8039f969c3e6" },
                { "sr", "3156a8c7d222b79d152f3afa6404d25bf94423f62bd227e8f9d9551eb068dd092f60835495c1abcc5b95de0109fd3ba125dc769f62f26c5f3f17123a09684acb" },
                { "sv-SE", "cf0aaa7b40c5c48e1acc5d5b2b36733fe2ec94c4b038ea755877a8e1c8a80f3dc15354141a7d5512d06d119c6019255da067bcd5a985016317d083f4b2308091" },
                { "th", "e6ee9ebdac78b64bc3671e73b7350c56f8597aa064f67e0fbfc6ac4672ff95a86dd8fc71919878b9eaed9fcc9d6fa5f57090f07ce4cd9b924e8b8cd47ddd5c4f" },
                { "tr", "c4d4541c373ca45aeb37afcb418a21be39979ac3f7ef6f34e935f5eafa33a240b99addd2be1e6332c862c3ac66c7dbd44f09cc0109f558c1b45ac4f1e915b6a3" },
                { "uk", "d0145bf199413c9e6cf32cc21c91e92acd394f785f4d276b1bfe57b9133ca68c56aed03924e07175d9bf71281f289f47dc0f2db1d08aab7b11a3f484a42713bf" },
                { "uz", "5b9abbfadfee567fe18aa4a694b9428d8cd9893e3e88e69c8c4d01fcacd8b0bfe36c0aa8e82cd86afb900184622ea679618649bbf5b6b67f7adfadd2c7124004" },
                { "vi", "9790ee7193fb69c061a0cf5ba8af785416069f282d1fab2594e3caa590aa05e76ee6359286d7e128f2888c9d4f10cf35ed83e4e7ba6c3dea6ec5d9602c4d423f" },
                { "zh-CN", "dace46e9ad2bfcb63bceafec120bc987cf7084a9bead88d058c8a9495088d4d744b5b9a20558a36af538582b910f6bd0871f1de1ad4e02e4302f77f32dffbafb" },
                { "zh-TW", "8fab73c88c1f05ae54d002e67292e3a0e518b8ec1bfc699ef89dfb81ffeeb585b418311ce0b4c4f18aad1dd435753acd2fd58adf791558f68c7073dad8a3cacd" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/91.3.1/SHA512SUMS
            return new Dictionary<string, string>(65)
            {
                { "af", "0c8d068a3741caa5ebbe7e1751571ee734493372cddd3ba33ab486df74473a46d93d0179baad8432009de0ad3cbaf4cee97ee0b5d1f6f1978a6b0e2a3354a3c8" },
                { "ar", "6341dedc651a84aa85f7a9f0669ffc1ce8a1e3fe5e5dba3076599cc917f56ca76d65a11be50ffbd2c1d78f6e62f4a25533b8d87d4c3170de92981f5eeaff7f1b" },
                { "ast", "865645c5d31ea5048608ab1c5e5bad0ae818f5efff7a411ae7a55397e922df3b54a3d66df7cf7d103673ec7013e612c7b2bcd3fc638c37b8e8484ccf43fd06e7" },
                { "be", "0677ea0ff849d16b9d49136b47784f303409d3be6b729ac709c9e83a32ba320fd072c96284c426081b59a6e4203a4847681b998f1cc5e0b57a62c9b4928e1ff1" },
                { "bg", "8a072da2be7b4a3d0b577d792e90f62eb83d04e106a6a409a821053417b50974408428712c6cdba82dc1aeb8a169d669eeed7c0647a593b17d47fe875aea8cea" },
                { "br", "77aa1cebff43452e1ee365b279daf8c15b04d6af1210b16f05acd722762cafee858fbb5575f3cfae592290598ae9598fa3b4178e4bd7e01b7df259249173c838" },
                { "ca", "2a63f7f145c526f9e50a8e48a6db54358cfff11b933fd81d627bb8b5af6ca495ab5d67edc4bbb987fdb7f89ec7bc9e5776ec0f8cc62d35e7d5c9c1076378b9fa" },
                { "cak", "9cabbbd973dac83c5f3f543f4d4fa5243e51746e7436d3ca826afc8439f765ad0a2356f2b3f221b8a6afc6099d6348eb8faf8f42762d95113453f11752d0c24f" },
                { "cs", "23ef65e113932daebf51d09838763013dfdabbb41e4fc35b657e8144035c1c78f3c239b07c217767d79c3e01cba2fc851e5b6bfebe4f1c874cbeb5285e03e6e8" },
                { "cy", "bb4550fdc0f4e6f17ee7fa855512531ef01c4db76e2b2bbcef68fc0c7afdb384867cff0a43d38fea7d45098de2ded9766fead415c84db0ff283e4a3956c8ceef" },
                { "da", "93291670bb94c218d6a1c5c48a346688e0cca0a106a772bb8c94b63e5882cff57c9f5a20199ba976ecec614bc1807e3170c3ecc7faefc749e590888a838762c1" },
                { "de", "c3a095e0693f0dc0df862f20cf5809e1696c1014fbdf0a21414ea9b77c157e99e5e1052512e253d7e979a7016d80fe43a151eca2a40a8d48ea6f85caba25ee63" },
                { "dsb", "86a6b15817fe56bcf71339ec785ea92be6972357ad826926ea2bc4534d7cf4991224ae2adcd46d725dc38dbf4eb6438f59a41e94a28d2738cf256f6a8eb93b1e" },
                { "el", "cfba1c7217b18a39e49df22259c405d01456b0f5ad878790b960f0dffaf537a5f078ada42f41b12d797e1fe9f5150d5e4ad6d80c3fed3bcb9da8c85138124237" },
                { "en-CA", "94a6740300c17a516e073e74b4e733ac7aaf6554f100fceac3a8e79ce6d2c04747e36d406ffaacb863e05ae1f045120e3b0fd29cafba182bd525f62838081e81" },
                { "en-GB", "72ab3163fafb388889099ebf57bf2e45f4a840007bc843c3e3bf88583a703782dd3923e7941a002f916b17398e3a614e92adc95bb5977d1ff6f0867a0e4b0e70" },
                { "en-US", "df74773322500106fba033b9ce331103b8eacb31368f8356787591d130928f39603532d81f30015d1233ea790d050bc804e53e1c2a55443f2bfc1d07423ebefa" },
                { "es-AR", "8cc0a71ef4e0b009defe234f515615811a2625665d4a64d621c8ca063b526f1d78b58720ae6862b800d313816445f640aae1dceef42dc1a2e4ea8601c54a78de" },
                { "es-ES", "3056db732831e11f14b4e77ff2d83a7107b1e583beae48c3827baf8d93948fca119168b8d8b6cad671c84f17340b21d8689024ee4bf7a5d5cbad4c72298b4809" },
                { "et", "7d47326aaee35427e05a3869bebd9385f8b4bf7e4f726a02f36a0dbde4becc42e590b1fd13cee0760f2520de6d83ac79cbfb807e92c34e867b81a465304236c3" },
                { "eu", "354457b5479adb3e992340ddd4531f461a4935848b59dc48fcae8731b810961f9a05353af2e4d70ef82ea4787706169f1b7a53152f8489cab0510ae405f9ec0e" },
                { "fi", "93931a5997330d22be30f96db453d2b8d926f243292479656b7e51f2e462f00913263a939df2dc34f0f49e2bf7bb6f3c328c53fc37f3302a84154b9f5d6a08a3" },
                { "fr", "37135a82180d556d9b6a81e73806140c2568aa5cd161d779138d2626a1e6be4ec916733aa4453c8b0c50a45c43f48f2f5bf6d6ae6b829d24e198b0652a22c1bc" },
                { "fy-NL", "2ea3541646bc918c9236bdf913bc9b99f5ed640c7b8ec5080fa8d4c9cac4e65745f2d1e9cac04406ede4692cbf91e3a886dc62976e219533f4c2a3836e2d8195" },
                { "ga-IE", "b1d75ccf93cd37882ce572a4e82ae8b21bbedb595f242619202ac3abfd02c1d26f0209ab00746001e27c576048ebe5d7d9c42d80f413c36e79d38e2ded01a175" },
                { "gd", "217cdd739bc0cb3c558a44bad3a39e99d76f42d421c09f1929b0f19ca56497390465920f42dfeeefa836f8da187298d99bb3f8cabf1aedc346d89206904eb734" },
                { "gl", "e318e2c07f4e3b604bf1ca70a651d8c7e11bd34d9e9cdb91832888b9477d85d388ba780ba3e1675d6b0f262df9295c69066030a64830694ec7df67a8763b10d2" },
                { "he", "29e4955cd3549ff08517cc799787fba3c6cfb9a1386324a2d907e05fa199827004cea17e7359baace8255869bf2e09389335f6530317c1c890f1e7db532795bd" },
                { "hr", "52fa9d2321b5981414338ebc5ac69a12ba50268c69b652c9238ffea1af2997005012cc5a3b3918f3df471ef6dc4e65ea08b854544b4032374d2dbf9259b2dd59" },
                { "hsb", "c9d38759d17b2a37bb912a150ef7fc8a25f8c2c40bc855f0a7ab75f84d1cbcab96582b3f4828b245ba0a02f1f769fb1b1b4adf928c965b0929c7e6ecba06c726" },
                { "hu", "153225670e101aff92256438aef5e2db4e8258cfeb6ae6a4e2255e7a178415119dc6a36aa85690c3931645bc925eaf40de4fb26b635dfb35a6cadddc6fea8060" },
                { "hy-AM", "2227f2d008919fa398ed9aedb88b4c0a1e35e09846adc0e675ef14389fe970fa283252657ff3bdec7c05f542ee711be56f6cfc27cdf632569a4efffd82630b0f" },
                { "id", "44df2bb7703ef0c6c2a8a899ac7d5b3e184613965dd423556792c9c36ece55434cc1c0e357dad84de93d021f1ef5e365f41685ab310aa9f3f3008145053e5f0a" },
                { "is", "821b646f32fb9ac92e24b7fb0b90a6036b79989de748f2f9d5eb671c69a49101fc3047f5a084db3a81d94c72db6d5649206d74c5ed2f7de5f04d1a3b04a37d7c" },
                { "it", "f06801973e3045bf196ccf882e1d7f6101993cedd10eccafad8fd30e19b22aa4ca12686173b45a924b6b2630b6c6b733a890871759a94973d03ce63521d67918" },
                { "ja", "8a2ba253a216a097930d3da54e125386164e45216ce132c371e9103f6b69141438cff5ec2235547029c8ff581de72e5df27fd0de5b34cbdc05f25da35722da43" },
                { "ka", "118645063cb600ff753a8c3f44b9081695938f9c3ea1a0a5688ef53c5a8a72a183f4d249bcf392f891b1dd27db32ef9a9c99d8d28096fb1d80044294477f1b1a" },
                { "kab", "1aac5b07125adda2873aca1589b67d9f2ab2e8b7da7e43879a8eeda364fc818ccd41cc0fd7a288c3d6f51fd7ee9495125c0e19d0bac940be9a9086dd5e716f98" },
                { "kk", "647ffcb4a8e822e63a5d11836b280274d23b61071bdc366d02d687cb3d49ff84c0a81975a9772b2e41ab61b63a19e907423cfeb4a541980ddbe2da7fbdfc346e" },
                { "ko", "996d169a77b2cfeaac908c1a715e97ee106950a20811c6f3cfe761a5f2d792a3eaf0f4d8c0ded3db1a2954ce3110aaebf1ffe3153dec88e408520d507c3d015e" },
                { "lt", "0a0f7260dbc08f105edcbbd464918e7cdf00dbc15015df34b9ed217b94fa24a6a937302fd4b4560927b29bc8b3d9ce06386511779163693686c04c495416c5e7" },
                { "lv", "ccfb4f3fb06512132f65f61fa11b2a2570cd177b9f16fa641744e91dd8b03b26bd93eebd4d797932dfb13db0558f56678cab8caedcd1a64498b57f43cefef932" },
                { "ms", "7bbdd77c3f9ceafcf9536a7417d5d34101247f93894e872f158bbc81ff50e80b18bdf058a315b2337fcdeb3fbe23537141355d7af9f4d4b73f6de7d32e06d189" },
                { "nb-NO", "37f54d5a2447e2d9c8a803e0098a85e39509c5f58316f58953b99a03715e22bb410b8277eff115557fa08f49ccc9ea7f1b6e7f282a292b8b4375eb2714485626" },
                { "nl", "89f08e1f549559909d49929110bf75cb119bff19191e5f8bddfaaf2affda930d6525a73340882d3077d68f4ed90c023de29dc304058650ddb28667bfb4d1f140" },
                { "nn-NO", "78ce4e85670cd3c7b98d96951fc27ca9977edbb48869c031fef2e41a894cda496038cd17f2a99db457a7f1eff2d2971d66e5d2167335b3cd79435b58cc5aa562" },
                { "pa-IN", "1a2237ff173a48da1274ac5e9e4ac8ddca37ba6fbf96c430fedfc594bd052cac37c9dce21d422519f534ffb8e6cc17416b20f1257c7b6d4d192ef08585c78048" },
                { "pl", "94a3aa87d1b6a5fe20bac6daf8168f90c6c9c14ceb0606a565ea0dfda9d6e430148112749a83887235b6b4f299d5436849f12d8edefda0b602a1e638f429df9a" },
                { "pt-BR", "7cbbff256db479e5359f0dbe4aba4b1992982cbe2c148a13d45e240829243be7aa363d76f832c1273495860dd30129e356cf3181f86ad938e8faa713cbd8cbe8" },
                { "pt-PT", "6fbf9075586b856fd48b36fd4051a604a73efdaebdcc35197bdac94cefb0cf69cbdbf1772dded212aa351ae67485084c1e8bf131607ce1492e2e8eca11f8ef4e" },
                { "rm", "85ac3f9a9406126f986a3e170ee3069417b5c8b9b47ff20ebbc1e82d06e5fa79a8d6bc841e1647aa9d8e9cd4ee46a9ce78cd539e3e95a850cf9262e6d439a938" },
                { "ro", "2f8f7f96a35cc7cfe8c5213a250485306f7c341723164ee34cc26c6eca69766df782e49c126315b5f1ed1514970c87cfb1597620c1035f80ed898648b2fdc347" },
                { "ru", "87f4a7de2ecee0aa1a3f8a1605919d4e8ed7077d1d1f3dce5df008a1d6b461aca2f73847a3a6dca57b454fc4c8e0374af32748982feea315a42ed50cfe709192" },
                { "sk", "6019932fb06b0bfc035f5ed91aff573421cad296d23f34082c7c3efa9870f84137f57ed68669b7c1d7d6863b1b36b5ec946aeb16e841b9f3705596eb1bad2661" },
                { "sl", "e774db529a9b2859b29155069311c40ad9465c10a7956aafc5c9467b7cdff27b2485794fc315951f1e9a4a8ae1ae9487733eba5077bc88c8d802a800eecd4d52" },
                { "sq", "f0a0604839dcbb16f8352df2d4ec017a7eca5dca3183903411919b2c072bb41c6f1813ef69301ea92919521d4de4afd0616cf07660e8b7973fcac8be5c29041c" },
                { "sr", "1f0a5c9b1f7d558fca785d3bbc6ce39ef2f5b86f53836328ac1bdacc2932546c252ef5b4df2264453d0a8294bd440c64a95c95662bc7b649d3ffd026ac94954b" },
                { "sv-SE", "c8713d0165aa9272d81980414155a3901f3f322dd13662ecf28df41a37b2bef6c53af812b426978bbf3fa3b8ba1218f3fbd40783c90efc62970465eef1bf3e29" },
                { "th", "f606a807b851fa31ef071842b36d608ceeb3d18ca513d96b05a64ade146ba5a0953784314cdf01dad53449247256aa40aa0ce2dcef7c84918027e382d4ee024f" },
                { "tr", "378deea1f2d9d7a7e8f90887af3498521f0c7d0336a1070a4ef3bfedbd3395632ea6f462063bf8686bcef1f9aa33363e9aab5a4a6088a7b0101650ab2145d0e8" },
                { "uk", "f877b3beab94ca2f3271ccf0a0e9a3372fc2972fadc8cbd9f73bc228160e0f688647a563b8f86be81fd404772cb5ee7f0a436dfa9946dfa3c7072d7b3f291af1" },
                { "uz", "c5044c2c7e8d6293d4b2020dfdb256d3550a5ffe42fea5ee46a41361d308d6e53843f67feb194471d32837b1d67ea1644a07ecc9c5cf6495b457b3d5d93e8d8e" },
                { "vi", "3f9d71a60d413f80c4d7d5be300f8394a13863c5c4e77882a1a29738d61920ee15ef842c875071e3a9bfc8e99b72d539dd3f87db30c62b494fb8ea89a4efed0d" },
                { "zh-CN", "5a7b7c242ebe598513b5a69a35829203b4dbb46933c7c990da7684dbf367ef5b948e187c772dd2a45eec0c75a2ffac206ca1eb4c03015c5c8ac79e546b782e0f" },
                { "zh-TW", "47b6a6cac9fcfa39833798d9bb1677c6496c2f026fd14c0a71956b23f3f295f31c9c332c38acd1f6698d3a788ff94ad974f08e73b030706859c4f58b1b7a8a9f" }
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
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "91.3.1";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30_000 ms / 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using
            // look for line with the correct language code and version
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value.Substring(0, 128),
                matchChecksum64Bit.Value.Substring(0, 128)
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
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
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
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
