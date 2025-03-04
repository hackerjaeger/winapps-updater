﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024, 2025  Dirk Stolle

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
            // https://ftp.mozilla.org/pub/firefox/releases/136.0/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "fba5ad08a7fcf2c73d2e2f37263868900e5c3f0533e80616e42d93f4bf3b4925a7edf972df2cbe00909bfee1ad716b16603f571e4a5ab28ecada6aea0e6d4967" },
                { "af", "3a1b1d2feac45fdd2b21ec18e7e8fd4d7cd92a8f4f93773355bb9109ad39584af45844a3f70ac57876e2e7024b3bc4d080cf1c6cfb78b2c00b8e1957cff44c41" },
                { "an", "5d458779c84caa87c4c084e6a97aa2e525f8ca5db083b7b6299e7efee8eedc061e456b2356b582b82fe953012eabdad2705b29fd0729838de49fe7f7c6937521" },
                { "ar", "b34b38a8379ab2dee4555e158380d68fd93798a21918a791170a17d79eaf4c558885cceb0fcd80975fc5781989534af48c1d9de37bcaabe1678f9436ff96bea9" },
                { "ast", "d61384302a2f4d73b343cf71f462237ed79ae4563cbe30c8c1e67dc43a219fb5306ce8b749484381124deb3f3da4eaf740f13257fb0a9bbbf4883c23a455ac80" },
                { "az", "0587be847892f1da9e5b7978f29fbef878aeb7f8e19ddb8f2971d270052e684ba04a143c1394225f7735c46dbd1b5dc201a2882760a24bfbd8dab68825c99120" },
                { "be", "f6413d3f1614516c706e2e7a0c91c0fcb41425f08ec2a1c0682d82047c1a3509d5ed6d1f191ddf7f90cd98d8161ca209fd757eba37caad18ffde85f36465c394" },
                { "bg", "386ccb195c51331795f24e4d68f325c88f83c026c61c54b715414d1b80596ffec91df429eb82dbc4227b1a667384a6377401bc56fcc868435ae16f599e0c5fe9" },
                { "bn", "3257f2b9100c7cf899ef746a2e6d8a9e7e56cb68bc9b53847266a112795c17d60212d869f8db8883cd5ad0177d3cc09b0df4fa726043af67dc5226bfeb9cee83" },
                { "br", "8940f19d233b5facc57cfced29096563170a515892ecc94130915534440d5485765f5cb05d700ff86c158b64b9bcc98ed1f963cb8bcf51963051db44464ccae8" },
                { "bs", "2492d2c32fa33ef8c445079d82e4d5da209c805a09e705df55c797500629eddad026c08aeead14177acd5d675abc856400846f59022fbd6565dd1c61b9a36e04" },
                { "ca", "6eb929248bb3d0d7570e2a2cea9f28da05eb9a5a1f5c52bb9ee07fba84e526cb78976373fc57eafecd2b413b1b542947111e6b8f387197b64ec0fc9fd065203e" },
                { "cak", "4dce22056b6486badd2c8849cc16f67003afedfcdc8e69ebc52a110e1b31789e638daa8a87b8799996a757e7bedea313ba21e7a7caaa0d67e367bc0448736f5a" },
                { "cs", "5ff1384e2ef80e23e307d5a50dbcef8613149e2cf66b9f790d950e2731888067cbf7ee2d11995759f5c0339dccdb003030c9de7d04ef45eec08337dd7bdf8c3e" },
                { "cy", "6b25f1048c9f214243b1856bb2ed795093605670f3bcf2d018cf929cce76d2ad20287d62132cab9f0be89d72ba54aeaeace9b22f9708a6f0399c732fffd01eef" },
                { "da", "fcecbdd78b1732c0c951efebae78183fd03348c61bcf9c6f37343ab8845237aff029bda9c835512b1fd8fa614415f5fadc566dba8e1ab0002741dc61b6d493e8" },
                { "de", "b4b543cd9fd0b45022665e6281930e629a06db3a3fa8e255e7e81de11e3e17558c2232348b8c3022ee868bfb4fbf9df001d779bda3714eba5b3157ce26727d15" },
                { "dsb", "1b516c3daf1adfb2dde157dc531e94d01c53396a25a335315c56c699a2ecee5a8c59fb69b1718f0312176a6bffc0d59218f69bf0b3abe1b2e43d85795993fe96" },
                { "el", "06d86962334d689ba701b5624950e686e5e5da526cf5e36b5f5e934439850e7cd9973cbbb1009f8f1ae178d81c5cf4ffcff945baef2b3f0f0033c5c70dd39874" },
                { "en-CA", "9c41363570cbd98b1b95c5a38d9c8bebe5d1599b917cb7d6b2e14614a9736dbbbee43c364880db1d7bdf4c81d7c01a7cb2166de160c0cb0f1234ce755055b4fc" },
                { "en-GB", "eef3d2e41540ff9ea56ac7f18b4a2fb7b6cdc555edfd52bacb97f330363c70b290b8d7b351b2e9224479a825d0bde83bd888cbdf1df02a6cb38e967b153febe3" },
                { "en-US", "d69c32fcdfb0785181020714f5a9bae413e4312e7af88f47f3f160ed44d668fc50ce0c3616c0764de4cced8e8f8a868641d185c5a6e356d06d73d936ea4839ec" },
                { "eo", "59d2b89289ecc468dd0d6a6481e9657d07e2880899a738cfa336da9839c2e6ad546dc0dd3ef58f3d0c1c0fded4d1dc79164149f7381e962020db48387025c762" },
                { "es-AR", "a0d29b377789845680dcdd1f06fd7059b969b7c739c10820d2fb005c9d981349086df3bcf3dec5d421200c335f1f9b33d8a2897959b54a741c2131cd5876672a" },
                { "es-CL", "7441eb7b0082b5edc3d29873ae73075a3fe982684d7129e1eb24c1737b4c377207302b0f80343ca39f38f7b97722d9fb4b8797cc90e3b0326a8aebda32cf692d" },
                { "es-ES", "e50766a24411d6d340496ad0f4900bbee92eba80f94654ce4153277ffa0651f650a5f1265d6936482bd0389e5b96fdeb9b6a218f1ae19e7bac89a9a0c01d181b" },
                { "es-MX", "ca25a928fcf0554d87b0aa8d379ab054ad19454ea1b93d5d2c063deaf73e8203dee1140123febfb474851b8ed82e2a4e41f54b31f273654297466985f9847336" },
                { "et", "7ade3ca568bdc17361f11da231c50f46c2f9bee3c71058d6be22e76b6024a23c20929680368c48a65b6b56c77d65d8bf2dfb5a88775f6752d0e45973accdf6ef" },
                { "eu", "c8bf84e4ffa565aca5c0ec707eb87af9feefcdc72cc1b6f7a7f45d0ae9433871e9f437a3dc1af8bd46da539007ec53d84b3477047373ac5b124dbbaa8b39faf1" },
                { "fa", "d2d01640b43dfefc98d3118567cb9cd6bfc7ead165cad131dcf2eb86401fb45797c314e8434cc1a71a2788be7e4c4d70c562aa35dd230cf899b5af4567266a23" },
                { "ff", "466b17ba37c78807ea82749399eacdd6dc17b9fac9d9b88061e8c3556dd8c428978f81c67e9b35b132f2f4c5711564492aa3f635542545d3756b734519b5f799" },
                { "fi", "96c74c313496071a7b67872057a2b1f43c4fb0c018fd4853c78bd650319f91ad3d78d2d6649c84e33441c3c923c513cc9d27ca5137cf8a51f0dd41ab7d10087e" },
                { "fr", "46609ea2f24fa33a339a4e1364c552799f0817e6397ea3a7e8047d752d017a84331421c11c9e4afbb49401c6f9985dca8dc0792e5297e7a86ea2a38c268a3f8f" },
                { "fur", "c2d26b563bff2cc40f459b4de58bafa451d0c1641f20d6297656d961f24cb87b6397dd317cfdb4856a436c3a106120aa0a37469dbfee3ab083484e2907aec88e" },
                { "fy-NL", "b870ade4016d983c3f4c7dcac310d32a7d96082e2b6a8d1708f5124fe9ea463c76da8ff6544f1c1b214c40cb67a53cde47d7f16405d30a8e0107bd95ddca133e" },
                { "ga-IE", "54abeaf7f10f34610d39fbc6252c701234dce21fb6c90cfdc4c333ee89d5fdad1a426fcfa865e1823406e0d076a0a4e8fe137eff71a4302cd67b0af8064686f6" },
                { "gd", "f2a340c84d75fbbdadb26662b41299d36d42b2c0775089a8f89bae2fc44b70c0dc226594424f7793b7331ef7ea0d0982aebaf954679a02631678e16d7da3c9a3" },
                { "gl", "429e4f79378c01abadae08ec9f5d86cf455b8d71ae2fcaf3d4deedcbafafdc9685d1bdb5a579375a0e14d5cd3fc5261105799aebeb4853c77c2d5daab8e87b7e" },
                { "gn", "1fcd9a94dbbace09ce9cf10fd7c15d6cd6f5aa8747893b58d9035d1e7ef17ecb79399912cf3e6a3771f4f82bb4a52e94864cfb610761a494fa2d9c3953eaa549" },
                { "gu-IN", "cab79c5b985f2b0a7e495368bbf35c963cd1cd81198a439d07d9d98323c3f6adeb9f25afcd7d975329e43c13692ba610b4c16e927ba61267ac8adb8ba878b902" },
                { "he", "808242278e9c6b0de7d5191641f89a7ded0ce8aa74d8da3177660cd44be9f69d77952ea24179179cf1bd5f825d3f9e5bada9a4bbd793b79f8fdee12a6d51d97a" },
                { "hi-IN", "66acf2aada72e6ec04cb51fb48f604c927f80b51c636ca9ac1ca6ea2571f84b0b565294112a8df6cb14a6f0a2570033d4c12726aff776fe82ee455a6f55fbb60" },
                { "hr", "3adced2215ca3e7d1172517f5d9af58d364c5f890991d41054e08fa5ec252d39a0e37f71774a101c1b0153adaf4f512fc415e8b29a209b202e3f40084f041b88" },
                { "hsb", "7e73f1d15da1e9e4ba7e26d518abf273eaf8cdb863d3f1bba2a279e104a32ab2320c9a5a681367dd116167d9836e823ab61fb09e6a6f94fcb43a92a9e4180301" },
                { "hu", "c77f3c4743e347ec1250125cf082a97d86c5af55569766bd7cd382da636d10e1a4b2345efac8b69d29c2e9bd4cd66a82e3358f7dc2c868aaf8893f1af0796be2" },
                { "hy-AM", "1ec6ae4e9f8756203debb845c93a248a02dcc59c9b569e54e45e6bd8e1516bdb7bfada841c460d0a562b76c170a125c147aed49018174c1e9be27c2337ca4677" },
                { "ia", "72844add49fd338e96224cfd4550d0c4d09e878c7140336642192d1f03cee4de56106b6e086dc607f1fe274d5eb5918a49ad680939783a8a5407a2f4136c4dfb" },
                { "id", "9bfe0c71707e9a40a0e556152b88635ca379fd3e505485ed49e41a6a09231bd60d4490954051160dbca1238d42fec5e10269c94a5b8b0d4714dbd654fc94ff11" },
                { "is", "2db3d3ab47a22352f002a960268eb0db5c19d27fde373f32a88134127ab460a19238b1f9cf0e4d7924e2af3389b3a375988bf31da876ae3db09fbeedfb3f17c0" },
                { "it", "297f343902a5e5fde1f0b90e747120b896485721366201bbd19d56a89ec672fba28db664c070d77c2ba63e3f8a801c42f821bea8e7e577285217b4ff469cf60c" },
                { "ja", "027e6015fa0faf93061223e8174549a802566f048a5b242d9071fe07991e19a6b551f9ae0e2722adf3d6b161c52702fd09e4fc8f1eee47660481c1c92db835d1" },
                { "ka", "64fff1f7354b5c4e71625775b1d33070f19863727e3b063dae10e1e47c4f11df6592fb472486769ac2d8f9a622a0f127d32e54c31f661c16c31b1f5b02c3507a" },
                { "kab", "a3dcc1f93937de4b8999d089818053c8e0c943198cb051a89c4b23ec0da99bacccad54d27b0105942d2370fb25a9cbce3dd3cf88492e710803d9fbd21c4cba4e" },
                { "kk", "2f749b3559890b713a3b1aa83bdd99473e711bd8aebe34d5761ec0cb9ccc60e62f572214dd63a6909edcdc5bbe48b6e6ef58a819609172878e85fdb85343c5b2" },
                { "km", "a52ddbfc3e77fa82360f433ab204b03725bbb2b19bdb3d84d6bc492166f24c4b300400d7f410b3c064cbaf6c5c006b8b5320c8cdbf235986790d6069bb0ecadf" },
                { "kn", "a85d2b29ee28493ddd646de554042728ff848ed9cc5b95d4ad54973676f7e7538e0f4d44d7879065e15132d30065af7b6cd98edef8136378668206f34b590879" },
                { "ko", "f6e432278341f0bb4ca0e7ba2637a28f3963044236e39e186297e3276365015daee6dd033bb65855cf4dc78f7c0cadc5606261ac36a10f96db1924ec9a6a967d" },
                { "lij", "1853811f1e9006fa47c631d660e766c02d40cfd7d82388d8a1f616ac40c790dbbd49cbd852da292d1651bd98c18fe40314d8c2ac0a53d9b1b46905f81199d1b7" },
                { "lt", "42931b46bf7e282ac804f23d7e337780d973a6b7d5f9d3a9b7412876fa6e0b57b0ba8b470861e32042a9f660495a5cc7bc97291f8191e7c87395880ee3834f2d" },
                { "lv", "26a960036e8cab056096bde34735617d1f9382852fdc8bfb1fa80c257e8c88dcf8fb42883526ad3479bc8f065cbac5e23f00dda4fb1390a791f41ff2772c65a9" },
                { "mk", "91d13ff2df66f195d768cdf6016f39cd8d3742d4f4d598429cd83172679df9adea79d6578b743324b70753213635d4640cf0bf8cb1eeeeb2e15b8486a5ba081a" },
                { "mr", "eb4af67f6f1b113bf8e94cd684aed5d0ee7809806fcac45887ede39472e1a362c54d9bb13395da624603f6e104b5c91621581af84a9485b3d40c5008cc54976c" },
                { "ms", "a835a89bc271c9f909576de56aa8fc75469245cba7a926e73cede5440720dd7b11aeac6502cfda5df53df20abb325225d8e16a3bf27615b54448e4c4ba58adfe" },
                { "my", "5bad9fb8cb82931ea1fe89b26695194ac52340049173608a60b327ba429d2d68ee4d3942fe657574fcd08e4fa410da99008a422640f06ab306239da70cc7240f" },
                { "nb-NO", "91dc2732f360751dd73391f03248ff5bc0dbaeaa6e8ca8db38d90548b5419bc4dc3bb00884f77472f9a752f8249e8de40108657a6bfac5192e685edaab859d45" },
                { "ne-NP", "02cbed9db4f2be70c740d2689c350f6a0c2450ef8d2c5fe865349016f24d31765a0b9a4b3ac8ea643ada9d1cae81d5487b6d152a94c46fa559964966b0f1e7d4" },
                { "nl", "a7b89311a806bc6c0f40c7cf7c935d6549b4be10d6842a0fe902446d2f3625b0bf2fc42e55bee4847d3ff02bcc2c5eaed2cc403c826bc2010873f4a3c644afc9" },
                { "nn-NO", "d2ba2a7a083d7353dab240eb3423ec257c52b475cacc50a049ff3da3919230c0f75d49472752d142c8fb0af2d872263e170f99110b921f22d049e2b25c14a7f8" },
                { "oc", "7e2e6032102000916feb63829c239a826a660bee0af7c985481a864e060277e26a389a964e8123158b15c30ec1b2bb97404b1f08cec6baa91fefccd4c37ba743" },
                { "pa-IN", "d3e0b4992953aa707de1fc8cc17cf00b35c315f1aaf786f2d8df7f2f2cee8fd1dee1aac32034f5f1c03ff3810b4ed9240bb17cfc5704e8e3cb37f2edd0077a16" },
                { "pl", "9f51e4d15245ff5d1d9aa427146e106259cac83dd17e775d8594395d1b6a4f380cc4e0fe833bb1bec03ef3e93757274c2c223a3b3ed96c3bc9494aeca8070d28" },
                { "pt-BR", "8ec211d779ef22a33b3aaaa37e7b333688e04ae06397cb4ff0654a61b7ac5ec5d5071ac0375e8e3e3fe6266a83fc70a35806efc4cbde8d60d7741118c9d4939a" },
                { "pt-PT", "04f6b49f6394dba0a66c82dee9f9e09d73858db69dd9c3cbf1dd42d4e2aee1d03768e6e5042db92fbf423ce61f48c374fe79a65b0036a2030cbc3a74e91b39e5" },
                { "rm", "f4e3bfdef37aa1b21fc84b4fee73095ac94b52741c2fddbf7dd391ea0b2092dfcf0e016bea96ead2b6d77be0bde4db48f2efc8fbc2a9e51d3d1461576cc6795a" },
                { "ro", "4405c5230e6c897e2ebe13014f0c330b6c14317341bf19a3d4f7715df400e8adf83bae2d5d6afea79d3fd916767c5841f2df41c9d8117b6a1b9927111d1c71ae" },
                { "ru", "e951fc9c1db30b9d232bdd0a801d0faf81ec1643c96fa375cac119563845704f6880f9f3773d27b251cb414cd486a67c51e9b935a7a19b0cf060bf9f0667baf6" },
                { "sat", "439d803ea8662dead8fd6da6e4a0d3205b72864f509613edc27c14d025939ee245a6cc855fd6f5dd3d8e317825fb23743b8f86526587ae58d3555a719700d6b5" },
                { "sc", "ae4a23260a5f3cf9ea8a777a8c5c161623937c7672701836ced3ece10b2e34a6d473de52b9dd0d4db82abe04461b0f4fb2b0b7a7b00099a75fcad40c1e34b1a9" },
                { "sco", "b2137ad3e74dca6ed4cec453166558a3e5cae302f1df7a1ad2043eb1f4b535f3a2fa13d83b2400349441bfed53087e52ca7a52f0c63c57c2f6e13b4ca8e0b15f" },
                { "si", "dc16528ca158b8d33eba02ff60f226de83472d0c932ce0546806908ed90f105e91ff6710cc3c7f1f18cd3077111bf018ad339f54b16607ba5c647235a1be3380" },
                { "sk", "74ff48d5654bb88d43dfe919c35e4fa7d07d495c69c022ba549faf54789bd77357da40ffdbac2a98d49ef131ba1458f685a709c8f34e7ca64ce2a56575c86357" },
                { "skr", "119d3c31fe0b6a4efb8416286f74a8ae1c215678cafe68f0c90993cf9deaf642e7e9b1a16ffcc591dfe1ae79e5c81edcf295c595d018d528aea6fb008c694d0f" },
                { "sl", "e164dc68c347d6fafeaa1218c8aa469e1f20c6132af5ef1f223cb381fdb16834798d99cf2129f94ac62efd61f909820e4a23c379b28bc951c2e3801e1cde3212" },
                { "son", "49101d6beee9719dfac49dfb44a5e16f0681f0f3e623dad90d02df840dd4dac48295fc0cf31dc65dbb17c3aee13e53f81000b3ae822ab2fe9690ce7731a3c403" },
                { "sq", "6065c9e329c7f5b4420359bd1a0a60d0ad32744c75555de5862bf7dd83efaa09ecb707a384ca13d5ba327e52b6014b5ade698fb1bdb525fd04bfafcbde5f6218" },
                { "sr", "7eb9ddcff86ed61209316235af1982a636ee888813e2fb5f2d6c810495008621ba3a246d5a4e8ded2483e0425454556680191bdd17ff3a915acd2ec2b73db25b" },
                { "sv-SE", "a99c3bad83cc783dda99b968faf38cb7ef0672318bed400f63b1971547bac80ab19b119f45bdad50cb8497e3dc54be3e606b2fffa34d9ca218ef905d574c373d" },
                { "szl", "b54095e516c15597b875747ccc7e470354b61c97e06fe155bb2e7b6ce948705bb6e8ebe50b4c1dd0a2ecdfbbf4ac1847afc1cccf3008304634b3931a2c82c108" },
                { "ta", "ee25e4f0b4c3601e6bb80f8037699641145b5b9e689aef53238bae57456b9043f1d9bdf651ee896e45e1c4f2719c9279f2447d0d1d2cbd3f420f256bcae2fb2b" },
                { "te", "0de5678ee931fd6b0b8585d8a16a2568917474da55a205ccd212fe11ffa483fe77805de8bbd4a837e8148c37b903a1fdc3fadb64523c57c91b7fda5c356eb076" },
                { "tg", "db81ef8af0bc827b90ae43a6eabdfeed02045efb8dac4c26e2e16e273ac2fda1ac094821fd9b8704a4248e325512a4c317e3f429a0e2032d7d84eae42f19d0fe" },
                { "th", "9a7ba4091738b4570ae82fb822fd5d8f4919629891084266968a6dd95950202756a372f2df7b9f1af3b4d44adff20f91e7c5e2fc5275b4dd84870b19ce78bc5f" },
                { "tl", "050fa8af8423f11624cee30401d0c2797c83beed9a9d332bb8c11eb9f1630df9b6ebe5da5ef1ffe47323c313c4d29298c7eaabc8da4c5fdf8663b292e1d228d5" },
                { "tr", "b8a457d7b5871d8cac03f2fc1e51ebd259430832a5b9ba4338803165b86d7527a35cf4eb1d3653beef031821be9d5d75c6b6b903ce224614a12998933012c405" },
                { "trs", "97ddc4f4bfbed16dca8a8dee5b1c51b7be06bedb48846f9ab9ff9983bb7cc7ddb8801a200f73967a3162821b43175b9166ffc3cfc2ec31e3ca7bfadd86739c70" },
                { "uk", "8aa03dba0763c03dc874097d8cdaa534216e6fe14ee56e94f62be11e22a0262f2f9fa1fca2878d7955255d9955ffa46712f733062e6344eeec50876b1dd86f58" },
                { "ur", "db327cc11d0769a20fadbadff96e0a78547e56ad28bf285e201a481223f70ad5b996b21e60d74eb439b7c8b4ce22b0c10d12aa61e63acf8c28b7b879c0f722a8" },
                { "uz", "ec69b3b627e85f4f26555e87079602bf786ea6cbaf4ec0cf47de8928c7b30b96c6a3937d062223c86ac1bb6e306ea631c255b292bcb84025df73e74a4c8e39ec" },
                { "vi", "d7700a91af1122b2146e5533ae132410005489028c48cd26eb1d88c22c2851e8aa59d95cd52074f442e35045986679aac3118d614e8e44f263ce6524a80485af" },
                { "xh", "717375c2569ac7fcb38c1fcd4c07eeb2c92da18a107a6d8975b304462ad8d22f5b0db6677f48f4f05de08c65d39588ed70032d2bf76719d37defe4ad6484872a" },
                { "zh-CN", "ab4aa925ac55ba801e4f3505a77cf239c42c43b399b98333e13e68d8234f5224fe4708930d8b642e6ced6d2b246939603c07935d1f40b7468b9272926841e62b" },
                { "zh-TW", "fba04e8932393188b7e4bbc38f7d548288b3daab5c8c1df574397d5bc4951f39e4866b9f125ceed82e593fb98f3144ba8c8eafe3f58f5527e386893cfe2152b9" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/136.0/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "021dce72f08dbdda201496d2bb5091d4b6521fd5d3c579362cc4b9de321701e3c96f0411498938fde3d9ee918adef8569ed01405e82c8ab02b3f45fe43d90127" },
                { "af", "ed3b9f72ebef444bb07e25e3cac1cf927d5e4315f993317692c262b69a9d2c4d2cb4c371301190b6f733fe4203820f2cbe1f29cd16dfa708b5c45ad12f772c19" },
                { "an", "891ee65bdc27df79647320e66a6ec78172db16bca94e159e10f7cc9b86bd54d6730f3b9c59140fa924282d9af6698afbaa285e42c2212feb2f623c63e05ba797" },
                { "ar", "3b5da8f176ded1b9adb5e05091c3bf0dba6c04dd29198d6bcc36d49cdc078aaaa2193cba5410a71e41eec50d0c73f116ff5ae0be575e6c560d5d6c785b6e78df" },
                { "ast", "fd846d2762ed9e5dd7db423f5f1d2257729d1173d0d51c7ace7305d55280551fd046c7d2dd7c6b86fa982601b8b660d6ddc5d1be228801f4b53bc1f4db6d0854" },
                { "az", "8562b8229875d4779d5d40af47d11a29a585eab88cf9ee7fef1500826eeb1765d6936a5b092b32d28b00b50a24f30168d851fd370fbe6e181c05b3994da08a8c" },
                { "be", "62985479d7d640b096400ce7c761d0b1090fd170a8e497296bcb2fed6bf8148ba376a3755b9a8fb64de383424e4758ad3344d0d0dadb4fe2786435b955e1a5f5" },
                { "bg", "fa26bab79742e561ac08292b7657063cfb3ea57d07c80c367125e74aeef2aaebbab9115cb57b1ee6a34c2f08223a56bf9c612b7b3fab975e42fd883818bd130f" },
                { "bn", "d959880b1168e59a224ec409962278b81cbc7586e4935b85f5c0cad07b3df0a70f3911de5671a2fe881d584d99f94ed1b54c1b0396f30d41b16156e0d2d3c794" },
                { "br", "cfdba7dad8d93a28c0080bfecd3dc7d2c164ef10115235c5676e0ea99248ac57fb905e038e32955665beffe684d93fda27f2fbed5210f713ab37ed172b1b17a7" },
                { "bs", "1714afcf7580361691a2c186e3fab3b9561eb2e4f95284aff210dc00b518b782f198295e891b162d7d4110b9dfcd332fe91929e68aa3553820d8f98f872ce3ea" },
                { "ca", "bf1030e44fff46ff134154ed9be65c06f203dd36925eee0ab4c4c949dbca549227a2d01f8c80c93a530f3f18b1b79f6a6e12e45c3b0650b80b314bb3a5cba649" },
                { "cak", "0e099bfcdb4f32645314eef3563779f49215083d5a38fd8045b32a4dc0f2309a5b8f263daef8cc098ab888a261c9186c0e4e79e0f1f401a6c33cf20a08019f23" },
                { "cs", "5380ebee3654c7050163e41421c01c53144b0c17793f212689bafc51043eb9e11f0ab3e15c3a9afb7670a66b2440d0d3cf6b39ae829d6870bf4f784df0234f4f" },
                { "cy", "58c37f4d46cb5a9914dcd7e705a4f888fb4b217128d8e5bae30b77100e318c7b8ba5c6141a0713441ef7d3ce0e3d4f91ec350165214321ce0067534cbc360253" },
                { "da", "8c1d70f9505f73fd55bb45b13aa81dfa3f8b9ea82364eb76de1139c03e0be2fa57ef8b5fda1cb5d40c32c80867a07d0aaceeeeea0d2f4d38e79c0d47a7c6ea3f" },
                { "de", "b813bfc60d5adb7a075467f14ac936e2f05d04d343533ef3eb207678ed0b7fdf14d10934651bb6177fe5221533dd1b98f2089bff7077d79acab56a1e29fd450d" },
                { "dsb", "2a25c6626267328e320ba9083b994c15a3de86237ebb3c120768c1a0f1ea43a4d2be20437881e05b80e0165defda6a9a7d9b480babe2def8956823e8cbf2ee6c" },
                { "el", "5909e65587c7c6255db1c6ee2df472af5fd4a534b4899f9451f380700d3306f483a35a2404cb5b2f01d9f1194ef2a07d1a2a56207fed3614731381983f886c1b" },
                { "en-CA", "34ec2c104ffce0184931c9d17edb5d6ef6b9c1d82fe9b67192fdc7df995eb1bc3f0954364d24bd2343715e79896b6fd24534c4443c817d3fd6ac4838b7c6b1c3" },
                { "en-GB", "41297e21590297f2c29a75209425669dacac384abe599606d6ef59fea7f38afecb16042ac8679d0fa124febc5f6ce24e37efadc3e18c726cfe9d19c799b2b3fe" },
                { "en-US", "d6e85bdcf1cc65d9c6bfbc41a83212eba321dbc493894f407a3b69409002cc4daf82ccc035c9eb2c2e2d33be8c2395eeeafd9a3f8112c9a1236d7710ef8dbd62" },
                { "eo", "f141a6a2e3f03f40a904bbf8ec960e05b12a10bee5c20306a73de5704f7e190c7ac3880de044dfdcdbb653f9cc6725771eb9b50309d7cd3f247df39fe22f7115" },
                { "es-AR", "2a32b2c63a2e49a1185a285f0ee961514f1b605281de58e5113ae5d2709263ea1d5a29534b1d37d48dfc92e0b42f995d07d2d16232618cfe8c19855339aa8ecc" },
                { "es-CL", "d0acd1301c4bc10fee99a1d90c3c53a63117e52a2226f5562ca910547f8847fdaba0f0b601690ad60634c9f3e30989135c232621cdef833d571f8a562043b5bf" },
                { "es-ES", "04780e4a78f1d12e1a6038bfb87e9c0a3a5e99c31f170191ba6fcaa22b11f799ac0e3567395c2c9ac2f9a0bc9860dd0eea3d6c892b07cdb442361e4c760e29a4" },
                { "es-MX", "cb9fb257818b6e7ade2223c506ba225985edf1bbb296a06c3e95d10704c0ca78b8bbb1b53213f981e8cc90d7234e42abe59da38d264f3de38b023d22cf58bb53" },
                { "et", "c9d7cecf354176bf906e8ae99fe6146d5cfdf9855fab5c12298f0e497c3be6f4fdb99aef72277dcda0a349e9e4c170fcd2ad9ebe8dab8602edf5d3ae4151b470" },
                { "eu", "b006f7264d96a91733222be25a1a16b687ce807f21a8bce78f1f63401bf5d643bd2ba5c9f9cdab4dd8acdbad1692a656e8245c24c2cc7d2ea122213cab860ab4" },
                { "fa", "659b9597ab1e4909769d3336cbda7fea5ceeb23edeaad35802542f150687b16a41e6aa15f57411fcd39d2bf364697711f62e735c19b181f4471e481f8a95f107" },
                { "ff", "1f03084edce4cc7d3e73892a33a4cd7225ba2096dd866703448d8c44fc57483a43254db53b618afbef1bb62d66111c052b7471ba743409a121c61fe50a5073b9" },
                { "fi", "b0de2f0a44be382f36e3afbe33ef07a08910ac00c4372cd2f18cb6535d138f1ce035229a2dccae90163556fe2efb2863a80e5106acf7622891a3e919fa59fb68" },
                { "fr", "77baa8cab75a21b863dd1e324c5759da2fba65532d91dd9f88e74f760294f32fe26d221ab98fbecbe35c1f10b6f7ccbac0c9689cd2a1bc5d6356e3b99e52e545" },
                { "fur", "3f59226b2c96be3e0fb8f648ed0019fa001af4838b01ba479f86378490958b153ec774e9d94775edb957880fdd7cbea201d7885492dda23bc698506c73f2416f" },
                { "fy-NL", "2eac8d0e257462aff60efa7542c419c24aaa2d963822e122ff5a46480bd6b46248977b7c3f897929a805b39389bf101272f9f6808cdaa2e5f835998523f7facb" },
                { "ga-IE", "68c34211f9ecdf5d17bb67b24ece3727a0c3d18ba5e2bd5c595a6b12e39cc94a94fceeba62383e521d02c55e6d0a2a6e4e093757bc72516a054923c10f57863b" },
                { "gd", "9809d50275aabfaf39955e26378e75b6b148bc5515c0cede8e08f615749d7ae96d82d737997e3e9e2c7181885818b3f82e96428edf603427ab491729049108f4" },
                { "gl", "a7b062c61190f3369768d8d41a5b61ef65347b3cd51a25d076ca58346292732259b47290346b0021b6bbff3bda32f5f048d8cd32d1a9958fa8e3b43d9e3d86a2" },
                { "gn", "1f447b0f83ae81ac26bd310252d3a9ba3ce6c2bf993fc9b25f6faf2f44f3e3dd77f057c6d4699ff6569b781ae52a52a6fbe900411f05a6f1e2df9d87a607a657" },
                { "gu-IN", "f6fe182116c5edfdb000deaeccac279c7d126355a56bfceaf6a383ec37c200ed5fbdaf339b87db405392c920157801ad035c600f171e16297e25108d156bd50a" },
                { "he", "a1b88485346af4396aa172d985b2d1c63500f52505f2520267a804c28ca1b3964c1e2acc1edfd7c8e060416364ed4f55d471b1e294ba59e22260311ccca6af99" },
                { "hi-IN", "10dc94638798c1a2918ff0889ce49640cef294ca6a176de0733ec832c3d2922c6a11fafc16f00160696d472661cb8eccba410ead1a4cddf94ea3193f49a16a34" },
                { "hr", "4db56493fa3242baf916f5747fe7dabefcc43f17514e3637c49e99a7df30a218cd69a97b47b379590527659298343e206eba49ff9272aeb099b879ee87055244" },
                { "hsb", "d56bebe765a8f0299afeccabce068e78de2ce1764cc2d3ca5787b635f6907aabb11fdd3f047b2327b18a69d5790e3e954ca10710d677225950a4a263dd1e2eb2" },
                { "hu", "40788f1b33dcd5da60f987fdc89b8732821d2c4bb8bb971449de8b42ef3fe8346c8dcc8f7ba543a3fce269276ce3ba443311888a0c3b26a6d14ffa037c93e54a" },
                { "hy-AM", "049de78be6b2f78b36fbe244adf10f25b21f9ba5d5bc301f2fe14203d96cb155b33e5d7f1aa4b94f2e93a87f795410c5aa84f105f735b611c5f2871c70687673" },
                { "ia", "a11c93ff5fe9e3b3d25c548f3e8a5c9f1ea0392ff50aeefbf48d04ba3dee74d65b15ffaed7ae9dbd58b47c8c7f29c55d3ec43bf6ee73c90bf05e76c5fce81f22" },
                { "id", "00541ead139f88e96ee4a64b2d1a564fc0ea65c8a8a047fd9f9c62eac9a6e7fe7dcf9186167a03c5e773ac053918e9c505cc99b1bb925dff3e115718ed651322" },
                { "is", "f3791900f4d7283b8c67d7dbdafdb50a6de2b6c29e58875ff12adf421dfdce4c08a33b4b915761305658108edaa5874835f38eb8e0327bb0687184853d6c297a" },
                { "it", "71ff6538979052fc86fe51324feb723ade64e51f59d11ecf2d8c13ee0f5bd13c2425b439491fc324e3e1089be386dca2a3fb946068243e0e227f80d08ba2f235" },
                { "ja", "8278f6d74420ffb3bbcf01acc56aefc8fbff9f0e4a205a083bd4b9049b4137c8778f2d20a137e6a0035c4b35f0fd7d6e1e5f4a207ba19eecfbd0769c85031f65" },
                { "ka", "03612849926a1ee4e2c428f954937890f0294c52b831a22cd78050441b1e3d25d8f4474735d924927c1347af2cede5731eadb99b5dc2cf7b6ff9dac55121447a" },
                { "kab", "62d29fd20d791b8b59b4cef70a5e4d6e86576194e723c40ee378a2d0b24cff0cc0712cf13fe6a9f6d669e4b709a2d96e78b16db2c9197c8684792d885c9e66c8" },
                { "kk", "39f4dfc1af4642d9b8ad5ff730c3f50e2f568d9c94a386965b9e97136f6f0adf4e6ff2eb08af1f1f8a358af954fbca0e2ee555f2b0bd4a33452f8c883a4fbab2" },
                { "km", "80841f4b89c7a66e8cedad71b099838599b810c0de077c0a3b3c0d0b5c91b07de729bfe1882099788de69307967b6592680e90d64a32ef7f1bbcd54243ae6d67" },
                { "kn", "fb6be8a0aa2225052f626f98c4abcff03228da81ff739c0830019fc2c0147fecfba60574b7d191cffefadc49f1b73a33d8af947890f66608af5190ff3d063729" },
                { "ko", "9ae66a8190caa64f606807d2452e1a058921d37a4410ae42069360ba8eed34fbf3b131a5bdf4b62e4fc7e4193c65487542938f5222e0f2e942430167dc7c8f9b" },
                { "lij", "b7cc1d4d13c8014e0485b8804c9b38fe00afd65abe99c20b80e0ffeaa119b98cb2a3bc980989e24069922f53279ea17d986762e2a0acd59748dfc737932bf403" },
                { "lt", "08cd3537d516182a664bb29b927e73060889e8dc6d2cfe0d0632df02af8f1b17b1fa11fe5a06c1e96c9f217815dfe91d259908dbe0afb66ec95f319a465fa280" },
                { "lv", "981f5948d879ee89f50bdae94c97454ca9df2ba98ad44a9849856f384850127a6e27e006a98e4b41233b2c3b12413eca27efcd33c72cd7c25c27f4d1a8441cb8" },
                { "mk", "320de5b8eac83e6fd8d5babd69821d544b223cc510d23b9d76b5f6d6a328180218bd12eb3f0aba24fd67091e8f934ef5de711c87a7a34c53bef868eab12f59dd" },
                { "mr", "50f391e4c6f81ed6eefb279295be4bd3bcb2e03570796c1d88538532cde70f814cc9db993036fa575f62a103625b26d3df018e6950b9775907625a45d3306457" },
                { "ms", "023c69fb7d8f242ab4f43c2b879ed76e3afe5ea6ec9d9e6003ec6f238cd6fe0d6f0337a68e012365e195dc647fc67e2e9a37b87d1933845246c7ac68b2bc832f" },
                { "my", "cef386256d896c0fb07876321d886582fd9a28edca6900e5f509182d2a70fd2bba1d26d25464f2f12eb7d82cdc85786f521edccff41c111513e5161df2e61706" },
                { "nb-NO", "577b47982ac0c5876138d6cd6cfeb35523575f166836ffaa9e7f3363e41687e204124ec6784c2291d7a09c5678b2eaa42e88282841697a1cbc304495c3adfb5c" },
                { "ne-NP", "72dfba5060d5c03aefec10bde7c72636535634456c764a32c16d430cada8b38ec844f7dabbd4f27722a13c1a73192622d5b53dfbae86c73758efbda9b1984c18" },
                { "nl", "f771f9387394c192d901fb88435f5f721bfc1987ea3b5f29422a6e7b6543d49132ea26848e962a55970a960564132e77d55becc1825b6aa115c642efcce072d3" },
                { "nn-NO", "e3fe476cd377343eb972994b7f67c701c5825ff658bac0c60d9323794a5d37dbf503279db07ebb43d417d9232fd2372ddb47c4556b3639491bbc723133bb9c06" },
                { "oc", "1f3c17e3548c41abc43903b3c58df47574a965bc82d76bc4042ab4e9055d23f3825e2f95e9f333cda29a7369553b3135b0deed10ce9f3b9a60de193a28c66fc2" },
                { "pa-IN", "ca13432af53fbbf330201761f050ef6cddb8c6f2d93709d441a5880d8fe1e910cfe664d52f2fd40ea44051dfdc48869c4b89b4a82d0385b273099d88dc753558" },
                { "pl", "8743470740cd487f2dc93c45fe28caa777b152c515478ed53b64f5f1273e67976c1ed73a3b3926e8d312b84f135183dcc83852a3a18dee4d7e71b68b74af183f" },
                { "pt-BR", "a3ae337aff64fcd449ea99a7f1055ae4429833b38161c00770259ea37b731c787f7c197a9725512bb093b57a0e8ff4075f6e2440c33467d4d02676188483ac8c" },
                { "pt-PT", "930b763eed2a6479a1c674a807bbfc25709ff7e64745480da457aa0ea0d783b080d1bbc7e87c70d814e0cdddc37cec511061f5bb6050c6e498c02b478acd4606" },
                { "rm", "5e2e9fa25ef2bb8b5d2899564a08c9f6399ca244c9f830e9cfb882f051b03354054eebc10ed7c772e17bf03519e56cc59e8367bb9fd2934a556fecba506f2342" },
                { "ro", "2709b574ff2324e8ee18a82186fa931111f5ab462835c7141dede85c5d3264acb7128d3f2c4dbe0d51c31545e29a99e11ef6173af2815b4087ec7f478f9e9559" },
                { "ru", "8b38449010b9be53b90dc57024aa4d2aee12ccddae1d9aed5c4f5be416917b225e6606f57c0131d0393a06c4823a39a536d86974903980a5e2f66fa56638e7eb" },
                { "sat", "482f5552694e513ebade762e40d6452acd9e834deee23a4d513636d9a43cf20036392e66bbfb97b8a420660671f0b9d9af9f20180781c3bcafc80284c0333835" },
                { "sc", "5a6b5c4b4402eec7f23b8d083ba4cf3608db45b0a43081897c1ad84579bf553e96749661d1da9f6f3a15f11167d06ce6307a9e04b55d1e3ad426a015aa7d4555" },
                { "sco", "f707f858beec18b608915abce1c1fd44d0d51054d2467c4f4ad1bf3376bf4aa96a29df9684262f5ed1fb9ab4bad61f77ef75dc062415ef0e65de07307ec10a3b" },
                { "si", "e756111112ce611cababeccc155b3ca6db0b74dd695585ed381f83cc2e5c02dcbf3ae840ed207ce6cc3e58f16fd2d6a4572c06afb728c1fc2f0343a02b02e8c8" },
                { "sk", "d86b7e352364f1713fc95d3f4ee30ce9cf52f89f7aa70921b3f50d8463b8ad6e4ccedb702751eafbb0c593de2c9c8717dfc76fe73543946e702abfed45b9aad9" },
                { "skr", "767d52550e60e63490bb842db0b7dc6d36b71568a5319ebd0b9fac54400f35aa4039e8148a5ea01eec14488bf92f9f8562ebc9a6e436cee7a236a3a0b4671aa4" },
                { "sl", "1d9a72f36e4327056304beefd843a2c690627c5a4868a8d40e90715aab6f33799629c35addc8c1a274420ddef7495935c2c63957773f7c6e4b3be07cbb5fb7f4" },
                { "son", "b27de79dcdb9bd091a54833e9760eb0ff627a4e07109c77ef36db7b1087976ac9bf3f992580871cde5bb8b9cbf63808cab6bea285a7126509929d193eb461e53" },
                { "sq", "900b7049624c99c8cb0ed1aff178cc2015e38a300c9451af712239508c68e9177e772665d2d23196f3bfc85c333e9758cafc44d093b6306567d13389f29c6766" },
                { "sr", "128588497b4988bca27bdfe8cf3385f1414f0687b875fbf0684b64819f609a71d1ae39605ace76e34cca83cf3d3f9bf65520877f0e9b59fc5fdcc42b5b777987" },
                { "sv-SE", "6293ac095ea55376eadcf50888976cfce40da0281d0c6cb94e0bd3d45ffdef6671cc672c3d6ab6b4ab60f338d1fb3b577769e838965595781e79f17de8d1f018" },
                { "szl", "84946a126ab77c56dfca30709b2266e0c2f8bbe7cc2b284c526578e57a64b4b5308d098c1f7ea9609e979839df7e512894c6fbd45b9bb38e6487e0bf2933ab51" },
                { "ta", "e7d7ec8ffc4feed9d8be7d87272fe3088b7ed37facee16a449925ef979afd9b9f139f70d139a95410c17e6d724fc2eb4025ba853b795acacd2e402177c0224ac" },
                { "te", "a240a051c92fb16dc6f28a4863df259d009020b14e9dce418f91d70ed4b825db52e62d4a9efdc1fe7eb97864c5f68a6e6a3ce8289cfe4010877269f55d624c65" },
                { "tg", "07215ec345488a31f7cc2a1c9f59c791dda965bae362a16003bdd0b1b306966e29b52cf1e4175b8ee541b4460087d76ef8154a8d87261e6fde47d85d63d1b902" },
                { "th", "5067bed61ea705ac29b7abda2adc8929bc394b1877e6a0b78d49fabaa0e7429884b072679396068b64ccd34cdc49cb6672ddacaecc3ab13703c3a34dc7a84344" },
                { "tl", "d396bb81e603d0dbe898eec5311d6575136a6d005e2bfcd55ff57cb7779032b37a69226b9da12a3933836951747872044b95c412ac0a1a8e848d9648ff045513" },
                { "tr", "62d13655a1acda69689983f7f1f20302a48c14256779f75ca4a4efd31faf2d4287f4b0fc339fda185af7022574fb0b78df4dbb94f905d22a932cca6d185ad040" },
                { "trs", "3027916ebcd36528ed32a1e86fc7fc45e6887a7794aa42b286f765bd25ecd06aa8b1f3ae74eeda52e26dacd6aa2c6912bb06cdf65682087a1882f8e47158e922" },
                { "uk", "01d3c5e9c1df673b203d6b76065ef580071fa158a600d623cdb4b28cf17539fb76dd33c57cc2cc55240dad72222f09b302cfc0de0decb863e2135f1e18107118" },
                { "ur", "d9b9dc9c72c71c6120e618c09db18bb3b114725075774ddfe356e1b484298ecd6d5a4857e367c4a78353abfe17b6e007fdd5463499da8de4c66ffb5718f2c879" },
                { "uz", "3402218e3f6a5163239a1130bca2d1aba44c6ccf50d0d2ebdcc983da4b8e8e45b4dc8873e55de52bea174280ce52993324c40bdff1a888de8d352d617575ffb0" },
                { "vi", "442c580c82132bbd7be104ff97ac7aff55a24e6333fda91c99f0c8749bdd4937b71ab8173d0171c9a83f71b3acc0bf31595602ce771cb9bbdd8e9bc43da4cf22" },
                { "xh", "e83b2f1377dc037d239384abe3d44850671d314e329c259f73c3b2061c33160e8ac846cf087273fdd229b6c04da9f0885244909f1c6f845fbc1a66ebd36feebc" },
                { "zh-CN", "7f2130b15e8ed1f8b24bcf54a7a5dc2fc086e8abde0230b25a237e17738666b4dacf5a67a816cc557fdd28933e9c179c7dc8f77527b10eacfd47ee90236d95f7" },
                { "zh-TW", "b2de3d5f25e0e19cc146db21b9a0316d4861307b21f20c59e9b56a7a5ca17919b0f56ec48386dcf05fc593d88b1df2c34551cf080f47b7a4b6ac6f32d1fcdae4" }
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
            const string knownVersion = "136.0";
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
