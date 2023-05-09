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
            // https://ftp.mozilla.org/pub/firefox/releases/113.0/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "62b0d5745da69b8b546f7cafe5007aeb55c1c9d3f01277bf4785ccd99690bebd20eb08026d29e3a32beb6e569a5f4189799d1afff646ee23df3e81a91e26cf5f" },
                { "af", "a3d6d48b67fb3976687862c1a46753f15a1bdd55c80ff4757ddb385bb24f1bf8e2a8385c63e02ad660a3b776f887e9209fa6c54c7b53fa65212fc4d50fd8d0bc" },
                { "an", "c2028a117aa9e14d5f124a0cbfa4561a0d86481376e9ca267804091208ce4b089baf4e3c51d2d4dac4b2495de6806250ed2d70f5a7dd54860cae5495d71c3cab" },
                { "ar", "ba0dab53b38094d65b8c183df57ff29726933eefb73c37b070b4c75d3cf98ff39d7d7ca66bc384f45d4a3e9aee7805fc39baa784a1046f3ace28b86f40d85b3d" },
                { "ast", "9a338b4b0f33c0bba6d034e1f3b1b1ccf9e17346ae50fdd419a945c70f1e0c8dbf99705a253efd024b9097f8c77c9d2c948d2b6583ab8db9936ed0401d1777a8" },
                { "az", "830c898dfb7b1e070959d6fa739afdfcbe8466b9cd26de091f0c6d3901448f492b0eb84976aa44ab3aecab9732efbf46eea7fce1c921ac59d615b1d41991fadb" },
                { "be", "60175d3a6a62337f67f46b9b4a58633b7a8b7b6e81ca7bb6179ac5c09a1e6179e056885f3aa7dd2772a0faa2bdf7656746be90b5619d6e0771817e003b2e795d" },
                { "bg", "1d39674d0724dcfaea85c1df9eee79253e2a78ba24d17de18d2d2e3e59336464e5742bdf0326a5d485f7fdf5f8c285936a144e47d043aa0f9883118b0d56afae" },
                { "bn", "3de8cdd59fbfb4eb287189f71337e5cd19c7c39598e683fafbfc709deb13878eb8da19a38f10eac5658132f306593efdabc946b911abe264c0d530085451261e" },
                { "br", "1296f1c2790d5785b8bf417aa579c39bf7441a8f9ae2444f8747f5c3564264bca4b046bfbd2332afec7781c3b62ab13518a3b8c09b9d08969aa2195363e52e0a" },
                { "bs", "dba85ed7050b9ee4c8ed95a9f0004e6b3dafcb94da0c9fc9476d125025dfba4ba777cb13eb4fab0a41f7c9f94fc6ff8057876e9b3c2df6f52c4b8a50c64b2062" },
                { "ca", "1915298ed618a1fe5c054c45b193127890f18290d2b4d513d92f26cc4e4637f45ac6e5ee1c3c4c3bff38a660c47f3fb3b43b4f825cc8ce4329427c084dbfd201" },
                { "cak", "2adc47c20199bc93fdd417ead5e807a646f272be5215bb63bba11d202c52e82211ddd6d585218ad1dec4c51217585fc93e37eb479688e0cd06c062ed90ae7e20" },
                { "cs", "3a6c53a9ece1b9f74c511d3e22efa60a8e25aa18cc213c6614c71abea18f0684bc18e635f6b6b23690e148bc73e5f00d5bdd0ec784db2762ffe69a52d57ef7ec" },
                { "cy", "90193e4917eade9c03a7c7b2ad0e57a6e48ed5a75eb4277a164032cce24ad424f34d79507584c00872eecf34b030270e122253c327e34110f81a83acc5d01845" },
                { "da", "363fb8ee61b310129364841587dc0a32808cb73f71ec2f3c11a5be697c5034f76d2882af7fa0a6a6cf3e8d7daa53968d213450b582d4e317496f6a2030567fa4" },
                { "de", "f87d51c71547c388955916ce42cb2916bbeca764c88e71dc6b51caf61e4baddc6a6b45d9f2fecb9c21fcf1e878a1d67e044f64fc4fae1c5c2ecb24bca1b1baab" },
                { "dsb", "fe495c14649f5ffc27b4edddbab8d669fda6c569ed545492fa2eb4c77b767cf0f1cb036e10cb2d769ed0013d1b904d0103b44e6e4132ce8a03ba612c4fcf2326" },
                { "el", "ae96a3413c1f7a0b2415aea1bf543ba593c2015aadc356cd6be9f9eb84de0ed9a75635d9b9cd44c422e75609dd211b28ec5368e3ea8d4000119a9c1d03ddf6bf" },
                { "en-CA", "41a1a131ff9a7301739e32ef01e6df686ea46e50a5359477093af282c2720e9a9e7ddb38dddd66aba8bd071e4ce0beb8547fc9334a1f0499c4c73f4b128d427c" },
                { "en-GB", "6177572cfe5bc0621d0f0c30e4dd09418d67ef79b0905f20fe97aecdc1b125df865f38e2b8ec6263df94ed2ca75521c4bf15e917865acdaecf4b328f8cba8bba" },
                { "en-US", "a81fc538f2a8630c3cd10cd394cf3f4ca47f5faaffd02b1064e9bb4dbae1bae634f596846ba1d0d0d7a276af4388dab4efc7d27d2a1c696f4cce14aaa6e5276c" },
                { "eo", "cb986f46e8605aab232c6dc8d01eb435b59a0381f785039bab6112ef45be2107a3cd6914f6a45ccf3edd1109d79afbe8c14a9a4d91450de3a5d5ad0b7d1187f1" },
                { "es-AR", "613ef9f77b41d5112a04079f1948bcf9664d9b37b5b50157f8fdfeb5d682938e27674d5aaaa5c8f30898a4fc1dc2b07ce7a5541b9e83b10ed46c8c42ebc9ab8c" },
                { "es-CL", "0ded5aef9f435b8ca62e4b0ec57a90a98979b6b942e6084e394b35090ceea01e78e8dcdba362c08078bd34ff7499ceb7ba1f752b8764121615941d75fffbaa6f" },
                { "es-ES", "868a83efbcbdd9b39526315ed3276840b23c5a37890faa7c8bd45de65bf6bb68ad6660e52f8b3a0c9f2be9b0e092c90beea6e92e61e9a2d640fe0ee99f0f10eb" },
                { "es-MX", "321f78597d963161809f68d9deef39e4846fe2ea1e9978920952761df35bd8bf6d62006257b97d475a11de453f61c84ba7c791c77fb66de5a28c247991556b26" },
                { "et", "60ed0b30a619007d03222128d7d77ce4d7c902f12ca3c95d696c84a5ec9c0be921d52f2c1a2a9d7114ac3eed835887a90cd4fb677ad5dc2e30199a34cb73e62a" },
                { "eu", "8f6028cb1f6868062bebf8a122d9a60276b9597c0447cb2154faa7617f9ce9abce041bbcff6422a88b187b7737097dbc07b73e5a10c470a0967fec3a3f853bc9" },
                { "fa", "6162259c7d48fc0005a680fef652532d058f82e832045fe1595204a9f996e0c403796cc9f6e196423c7d7d683c3460be8d2a2d76fb94b1bb27d67d0245193cbc" },
                { "ff", "c5f9cd0fd1366d30b0129883ce9caf035eaae1c9a295360523872c31cbdd8568629e2be9b8f9da851a021bbadfe0890009d9475353df22514d85d63a22d5762c" },
                { "fi", "6031d7c1d7c7063e5e56b6da4da242744856fea79f29b8a65d278f6709ad631f68b02e8f58867090d59e155edfd0c11fae97bda239b342f5771eb5290e4bc48b" },
                { "fr", "a790144f4e5c65683d42ea53f55512e8f5664a3a5770bda3f54827c5fd33d4f4b94d9ce246c2083917fb3e9bf95544642b1fecdcc07412fbe230ca9ba3756a6c" },
                { "fur", "25770606ac019ce611f786e695dcc594828d8b502afb4a861cbef19f7f4b11d4ada2ab4fe8ad1227ab30ab673af63b10cdc243137d1fbabd1ea887db39cc6721" },
                { "fy-NL", "55e2fc996fabfc965d9253c0014687c1d413769afdd91d6396ff6c2553665d1445fc5447b2bf3a9fdf6326f4a5c4ef385558c41ed3b581ed799e157ed1242308" },
                { "ga-IE", "976d94bb83bf944ebfb2dc06ea707646a1bbe81aa60afa16a3937567f3c15d1effed3fbae0a99ee157b2033fb59e4f343f040da70b82a10a565606a801146a23" },
                { "gd", "81ed2b54bacf6fefb5f15f959204b503cc9ff411f71f469892618542c52c5f56423b44f3d2d136e0832a2c51daa77b21d46e4c341c3fcfe8dd8f277bad691049" },
                { "gl", "14b98dfe8dda22889cf18d82f30fe9049d07d8a615f616c8b28fb0ab21dc7f15e6a9a1470ca666319744d68fa3fc3815746f315bbff1397078689f7b0f011a32" },
                { "gn", "26afbe5eba3761bd3dc24920c7b204363e33cfa2453bbd76132f68745949a6598dd91887e29799069fbb4fa745f4aee97582e1761b5b259a21bc2e80333a37ef" },
                { "gu-IN", "e6380710051c346b48e454f784c39a176975717bc665f06e998e0d59c6d7acfa280181b074bfd274a485c1d9968cf1a0774cbbdf25579ff68d044cd0ac1fe49a" },
                { "he", "82da6e298c6e2c8e9b8e5c3c1532d4c0e13519b3c6ce7f7c79a187ec6d33d485ee2ab4b564386d77a8060a52d9629fc8efd678d4c6d0e186a8ca89efb919bed0" },
                { "hi-IN", "ba8462ed7e1009082d3627b09224a94cc7f29c2b84216a87be8a2da9fdbcebf53745b8c1edc65a152cb42e96797ff07814d7229ab08df83ab233c057e9ab9feb" },
                { "hr", "70a7c24d48fc9a906c76f44c1c3631f4b54cc52f6a346f3ca305ace4e1971bc3927da2aba61116fba626aeea5c2c33eba9571b6a981d9e1b7bb83647976e31a1" },
                { "hsb", "159c4ed871bc4626d026c5a99cdb1dff0414ec3644113bc571079fcc19da2c68ccf340f52a7cb3368be5c11ad8973732ba19cab8d5c4fac0ca8612fd0d0df389" },
                { "hu", "5362e7acd28455ac26bb53cd009d7986409276c7fdfa459c47c0a4a1e65d60e66e5e679ff703fb250b4e421a0e11eebd79fb99319b6c45a60cc5bc040e33be37" },
                { "hy-AM", "adbcec11cdc33b3db1ae169989ae95d5cc1a33679c88e4c6fd7d4bfccc043c1f663fd66b6c951ed8ce88508d9a0a03e789b98776d2a8d3bde9175aed92256787" },
                { "ia", "99c15b445eeb70013767918c5b7470e613d6a525681ee737984cf75df38ca56e112602a215ff1d26f91e59d7213c2b5e01482deb721b2753498c2cb99cec4ecf" },
                { "id", "e1a10d91e1abd2df215e2f7529684414f3df232ac7a7b33f1468f0fcee04e0b0751c6157ac27f54b4f5bc1a8f2b47bef08d778d8f3d9a95166b8d1832f1cb5a7" },
                { "is", "4baaf5c616e8a2ac494d466d438e60861c753d7cbee62061f96b629e94263d0dce2001898e554afc9f40e8d0352b8ed422460607b0640b7c646d197a1837f574" },
                { "it", "6c1871e795a0e7215c35d9d9f70bb5c2a34aff300527bc8bcd23a26c69f3a08fd33e3f13a8a7ba52da75a3f68d1e512ce86daab13ebbcdb06dde38e033f13ee1" },
                { "ja", "b4cefaaff60224a3ec4eed381c4b6511eae0b686e105a31a8f35b28b4b5455dd85fb9690fd031a07ce498e98d72a6c4faaa7ba5dd43c496b2956429325bb1b96" },
                { "ka", "7d5f7d4a339f260916c959dc176fab88f5246905bca0fbf053ace9e11aecbf700eaeff1a777b30d8aac06db958bd0c39ae6c4481a645d9cc9f47ce6b7b7de4a4" },
                { "kab", "0b8b53059200b798951cd3a964d3922dafbdfdd15c7a36f85d0bab90ce0571cff4de2a36a6dba8e6627cbecb0a8b95ab7f15a84aff3d03e4b01945b5ae8442f0" },
                { "kk", "7147f86a1a878cf524497470db8662a4d2e100af14d9d26146b9d768506756428f8153909541f3fe1317bb30b5efec05d08637908eec23b9ba4e06d37a5cc192" },
                { "km", "ecd40e92c571b169c099739d5ec3d2ec72c42764bb3cc2ffcdce6cdb4a35405ea3ab749352f6f4e5c3efdcc45bb14d9f8c3e123078c14ec033fb34b59a176dbd" },
                { "kn", "a8edfa34c65babe59992fa54feae5c97f15f088388790a4e56e8b8a52a652f44665cf258a07f9bbb01c70b11d9e8951bd7a256ab770dba49f3953b560cae02f2" },
                { "ko", "cabcaacb83a1f68cafec1c916710888a3f59b7970817bfe7eaf09d3ab8470348b6213c11c814f693489b8812bd8fe516281bfe28c594d4115bbbad710b33c82d" },
                { "lij", "040cc0c56530e2ee499332b61885da66cf275a94bb72bd935c132a5721dbfc723af7722d504d17b7277fa674142cb120fa271ef52a4b51d9bcb0ce5ac6debf19" },
                { "lt", "ad77be36ddbf05b1a917a54deed85f7ddcad5d03e4b4713824baf67d0bada5369d4232d691e8f2fa39fa330c1ecbe86c76bb952197d19332d05583a52d9d3e2e" },
                { "lv", "d3449099ea54278aeedd75d0779dfbd411de1c3d9bad7a19f0af93419bf8a128f7d323e74db7ae56e690c9da0f846c1092d8c4071dfbe68c02a3fcfc9457030c" },
                { "mk", "9b586b0af67147818c17d867ba84728ac7bd3f0c10f867ec7b38794ea4a861772edae3307338522335e41999c904ae7e8d55ddb9229cd5681e6f8be05a3b06cd" },
                { "mr", "87138f316217fdf96b7a24062b1487f38ffadf291476887bcf9d86a063439e13be733cd8e9f0b3aa2de424aec90266f6d7cc59155eb6014bf74be2b074dc3125" },
                { "ms", "d50ed39acb7a94dd6f63e0aaf4858d9a6a16db0a226a4f74bc7070754d8e8113cf590d7330b03808a2972c329966de5ad96870a381c1ed9adf7a4b7159cc1b31" },
                { "my", "7103bcc6d8f50fb60f91ea719f34e674cf2825a8964966dd1f5944573f966256163beb0249135ff0805cf02a820821d408e0a6e59b3ae9851bdd17173044abad" },
                { "nb-NO", "8b936e259ceb323c74d0c05e032a32fe052a9f6662b739a7bf7b876fd47abff6ab8faf21ff6cf8cf8a6e75670fea896671f2d617e7bb72b99a4de466ff8afbe2" },
                { "ne-NP", "d996707f43c53218b46353c01e621b6cfc032f8bfc103aada9ceb6a82140e4dce07ec1e9d33cbdb88525d2920f0a29e20c793a89ac6fee08e7ab0d8d36b15015" },
                { "nl", "d88fc5d21b1439be9f8db54d7dbc3bd16fcb30e6f9d3e24ba76fe8d70f5fa873fc35855a32930709b52aa9d5c4d0f40705a7c0dbe50965f34149ad21520e3311" },
                { "nn-NO", "16a5010ad992b0ba3c2726d1d2d1aaf9ed34f3245611996801dba2d4459e2f9673b36c6c1335d43f62a00bdf4e0adf24c690152c5ef51eef5940af8e4ffeb466" },
                { "oc", "80db97083a252395458c67913f5334b7155cf641d1dedcc10c8c3000b11cfb6445ba8e293303a8b5bf39eb4fbba2e068dba604380b264bc1baa9f80df0e4dcb9" },
                { "pa-IN", "a3c0006cf8609039ba0c587cd3d495c0978e2067fd54ac79fd999f6bb88e8957a4d7dd75562de2ee7707ae0d040a2a68da17decd7f9005a9d282100292a6012d" },
                { "pl", "9e6ef33312bca8ff2eb10959af0ebd8b56a701b7d4399a0c3c005a9ffad6add8f36ffc20359c2514fb3b59486d5cfad11c8b3114f18dcf2a14e9ed3735113ebd" },
                { "pt-BR", "c5a6518e1c61a693051371ad9983f6dfa2e6810fafeb7b0444c6d8cc8f8f4b285b6ac7135b4c5de88011a717894bab3614dd3afa3af29d0bbc1741f87bab340b" },
                { "pt-PT", "555b36125d60cde9495b7e47b88a96d0ea96a9581bde71e178b28b89560af6f07697441f5d9f25e28673688f041d995bd111c2902e44a5fcba8acd25aeb421a0" },
                { "rm", "454c930f64870e9c45ba5df341af09c6ebf7a19a2686f67ccfc7cd3abb850c3a0177c99ea8c5be4e77c024f271a19a3e0b3694f1814cf7b77ad8d8f6e9b84d8e" },
                { "ro", "6d95dda1d33313115c4dc197fbbffb43f9ca3f46e23ad6722a75f10c03ae2cc37774420537fc2bf31a5af033f8b87aa3afb5665b33df9485f5771417fbf270d4" },
                { "ru", "56dc126f77b9522b22ecccf1b816018df6d709c20adfe7dd6cd76974d002bea5d997288dfcfe1e4699a7965f4fb5150bc11fc222e9799524352a7fc909157d36" },
                { "sc", "ee481d4e4dbdaf22f862fc58c850876dbc35a8a09426f521c0f7ecbd76334c9b4c58f47f7784b640cc5db5b8bd058e3e8df5c2facda286c7800fce5e69996c5e" },
                { "sco", "9ffc266e35c92d3086e56adc8c56df94d8138b2a60c65354646585df42d8e2a032a8de5e37c69fa98620b76ca7007ec7274784922d1f277f04f5b940108ea3d0" },
                { "si", "92622c1dff1f15130e687adc24a91bb3461323a0357c5dc4ffd2a1cff3b75f34e5b6d45609d2f65a78b1b47c0aff2efd1dcfee204fa494358bb06abc6dc21493" },
                { "sk", "279f849f9fdd9113d5a220d1a1d5b596047d7d804d093f4db9472b020f5a966bfb31fa23a8ac360edb686c14629f4fe5e87fa081969ee57dd520fd0e5f4390dc" },
                { "sl", "ee65c87c2c38ff9e1baa11d9dfb20685be7732e31967236c63e1631ae9c5c46d724e21bfc26a771fcc6d5447b3bf13496e0074540ffd17a0dcb998c49a74591f" },
                { "son", "d4a0384afaf9e272da4d53cfc3e10a9cb92db59b3f4c60cc5c6dcbb85ce967bffd8bc7fb99812a10977aba391b8571aae27ff5b5f175060ca5d17c5c8915ce3d" },
                { "sq", "33429158d34184d9549862e17fb07f259c1686f7f403db590c491d19219ce768808289f2caa5bb2d4cc69f22edcc6d74d0798249aca6898a272cd856f3a1c51c" },
                { "sr", "5623b8bb877a8b5af3bb3bc253734519c2bb82ab05e6373ba158f73f9adeb344f67039741bf4bc537ab58e7ab1335ea71208b0db4b938729c7727aa15995bb6d" },
                { "sv-SE", "41a4e7b8f3fabf3479ed61ac2b53130fa1bed4d52fe71456f58cdb1e02e77744a8ab387ec6c5a5c9b47930ab5aa2f1b00ae6a5d850d45f2617dc6b785e6bbbd1" },
                { "szl", "4daf4f58b5a0b6512a11996885ce75f25c115a0f4d75416029e16e2d31ef93680240830cf114f3d01549cbb21bb4cd02cbae94e7155773afaef5e147c1df77e8" },
                { "ta", "acc09c23cf421bdc1ac4a6f1a36f29929f68055c6757c66aacc7a1afc07c71c823adeda96bcfaa6d8b082f541ad61eb4db8d48c4bc32c58e7b0a9c61962b781f" },
                { "te", "e3a2e459f1f32f2e1f84c274bcba6ec70c84203a7040fed159b509ea46bdcf61841b49586b2b782d0dfa3a917879ad54bac019aae41195d182eaf9c046026fb9" },
                { "tg", "3b2a6c8113db9969920a011132930986ddadd4d3b983fe827f4e0f0c832eee2bb95c15eb4c4b4491197e60c9ab5330af50e08dbf0e2211e60df3e3308976ac57" },
                { "th", "5b129a8bd5607484681876b0a8d0dc350e15193f70a26b7238776cd5ce83d4ffd1177e172ffef6c5e20411ebebf3b3c04447dcdd192eb4d0018de037ffae13aa" },
                { "tl", "3bd1c1827febbd8150758b41a3a077b8f653a938f3fef0773c07ee64075639d0c45572edf60539aadfdc4639f38a2dbf9471852cf381c173a745d6f947cfb46b" },
                { "tr", "e752d8ba40955625306f3ede73ec94f80c49b342199b463fadd6e805be453ac1789b15f069938b03d9d8f5f9f8a389fa15e1aa9f3d5de8e206e487295284e088" },
                { "trs", "cd34e637051cc9381044f725ca14a4b091fa6b737b4e1ffa5c83d28b81c24a026f6efc4ac7e4c3403448166758692b08fb0927ca3684147900ae06331e22aabc" },
                { "uk", "36e366ccde603d048f74b5d124b0db7634d4dac4151826b746a4035db97b90eaf050afe1cbe62522bb3b95296dea643ad59f199e5b1c376e93e9988db68a4a82" },
                { "ur", "21ae8521592f581814905700f713291b3fec1f274c7869319da04d3611632a40027056902ad168babbae9bae9a660b65ee6beadcaa49961e8d3363ea00d16239" },
                { "uz", "0f486acacbac47ff4a9af0a1c6af534bc682edbc0aa61bb9a2bcd18ad5dcf57713e4d42b44dcdb0e76bbcecb281d78670b999114d212e59c064749f8f731efa7" },
                { "vi", "97eb443963d05c9e39a7dff153f527c73b0e9e65fdc4f047835aa6e2897a8397b983bd4d26214f9edce909767bc294fa78230e2442a9c714273a0a6296385675" },
                { "xh", "b34acac147b9f8965c147be50ef69fbc07e871412b989eeb0bf100c4149cdf544a5deef96b33794ae7ca1156639508c45293f29d33a219a0e88351cfb43377bc" },
                { "zh-CN", "55f5b4c9474bdc1231634073e9952e289350cbf76a088071d78907499d2bd72cc2f4ecf03e3278384f2fc38f74946ecf07f523103cbbd9a74bdc6dc500520dee" },
                { "zh-TW", "2e9bca39f2126159a592f5a69cd53864b9f265e25ca49dd670949f55e8ce40c3d38261b9249ef4e0fb4792b8d2df0a947975f4344b64e6e9a59c1ce04d173c07" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/113.0/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "b8ca681bd01e3346ab29eaba9eba1931a407f521a9318daef3e5bd4679493a627f5efd18e057644ae91020533a8973cb704ab03bf7e4f08647d00f752a247681" },
                { "af", "4ee691fb0b6b55c190071053775fc04ebd1df020d17d3c53b318be44e10fb96aac27a3919b5444bf285f5f3a88c37f84bbbd6da81d8909b6902265bdde206ede" },
                { "an", "82722a5632c8a3408ff9d8aa0c95821748f5c01bd837129e24c35b2dd9ad2d3caffab5906044cee5ee2f151cc24a9d9a16fff3431ad2f4791f0a53d4255f9d00" },
                { "ar", "165f9aff9577ed6348270f750c8f0779d6b0ca08d993c06c244c40279bbc0e51bae68f04ac8a5c22d9b5486fed07d4e5c629076e2cecaf1784095feaf1fd1ce9" },
                { "ast", "9238e78c605228a54356a51abc05e948311b2662e805587a059055893b708058383c7cbd57c3494e7fe25b39cd88e79204051e81011fa54850b361e8c7f95931" },
                { "az", "ce8a426655e812d796d78b9b13738272abe1d13064ec460576ce6fbdad22d065f0248a78fcf922b4d16a599e6e88fd5135a593a810802f643bbb06c67e2680ed" },
                { "be", "5feaa7da2fe0c89618b9f695661e76f848a9a948cf45eb391ab98fa0915142c1e2810131a625a01d6e43a168fb178aedf712bed580c30d365be7d5e9afab19cd" },
                { "bg", "cb0b05588d1dac75bdca9d4409e40b6270164d450d31148408852a492bf091e3c6c5f0ea6e9c8c90ce0f68329528148bac7de0263f80004c97ef7d4d179e75e7" },
                { "bn", "4b4e73c245a659dd3091aee1a678e62ee1486d11d02a86a490a63e7c45323e763ef8ae4dd2d3fb2fef2225ab3dd8ffdbbe77254aedcc616bb0ef0875a86b11fb" },
                { "br", "0f742d3a46072e95d62fb5d62eeddadb1e41e48e59bb7e72eaf031a050a77d1a27ff1b169cff2a0a65fecc9b9c557794c775bb63550744d7c5b320db038a24d0" },
                { "bs", "0c273b34144edef071c65f78615b432b5d7f4636f4aa4f3e68d4dc921dbebc322260a4b02a245e1848fbd6dfb362da3072d5e807888b43dff5d6a7962a3a8337" },
                { "ca", "50243af4696480c674990e7db5caa7fca0494b08f59123ee84167706389a2674fc3d3bb77e7b4882f203a1db83c4bc5741ae675aab36f0819ae899294454293b" },
                { "cak", "2aa22885752b3b0e0f7d8b2681bae932db01fb47991ba33656e165f375f793c2532425d3875be1778be1e6df575ce2cf31b0472b02aaf17119a81faa4a7baa5b" },
                { "cs", "c621bae9591d4cf54cdfab4916b7c0097717465e3ecc0e2b2dd28da5e903921095681207c1e70027ae77e45700a2af2247ede46690db838cadf3160c1841a1dd" },
                { "cy", "d8628e1f55cb463e48e3f2cdf00d6deb1616b40ed87aed5e2c91849fcc44d355412f18943d8a23c2a3a59c78421b9ad7574c687529ba3f76c728a5964faa4fc9" },
                { "da", "dbcae810d4d197a47039f1abc5aeafc48d66e3c1840d20769910202cca9243fe18c39610b895a72a498a09f206d96e65138aa6fb1d7f8e67e45bba844120e2dd" },
                { "de", "1eb8a06104efa963d43595f98d0a3d8255269df6adce1cd03f50774cd61e0db7fa7537cd1e25eea69c0d3a6c9f60df822aa65067525cd4465acee6be7a6f7af3" },
                { "dsb", "33a661c84d3516377676a3614e911945ab00c4cd9681dffeb1bae7a29b6cf5f70246acea682a781f7b8f45453ee33890dcb03ea4f2c9f6915f4f4d3db45f47ad" },
                { "el", "81f3d054986115f36d90e46e324a54c4ef16f0457faa68ecae553f1f36b17dc3812faf623a546fdd7f552f8878a6d676b06f24304820764b68370225197ccb9c" },
                { "en-CA", "437b9fb18bc3d12fa7369c4692ca09e60c2fa5ecfb18fd39ab8793439f07157226c57e83c40534b5503b9b8a0b1db3a2470549bae7f19c3727e7142e72e8630a" },
                { "en-GB", "aae8bc976527c9e25973c32f87596f7181f53cc41d4e023037881d09774969e6b91aa8e213a4babb8764bc357345c367870d07914f5084d5cc7fadd1af0cb201" },
                { "en-US", "fbbda493b56361da85d3a55688dd94e4a6ba215111ec0d67d3845aadc9489a7cb287deef1d312b8bdcdc553b67e15096237c0e46f46a38df13c547369dd7bd64" },
                { "eo", "e93aecae5315879f57b640e400eb62c34fea333baa723748bb5f6b3c8a096b8bfc392730de607abe3a63fcb666c5d7cbcbbb574c1dd19d80849fb0d49b252086" },
                { "es-AR", "9da3d3223e8e5cfb23d9b536052ba951c5db0421b5559e178e285fc6ba999300a44abd8e8dea4a480b16a56280dd6522fda989d48fedb15aa637ed8508b9528b" },
                { "es-CL", "9a684d6ed55c3845376d25f4af04d5bf9155fe284ff73115677ed56d31704d0d572bea692718492e8ab1ce0eaa1026af3e940956009ca9902583c3fa29798cfd" },
                { "es-ES", "c115ea97c8fd20d3816ae321f54a95de653e1afaf0654abbacae67c530675cb0a9aa7c44dedd60464390e49fc7f1b6f0c82f31ce535e8992d250cff1adcc249c" },
                { "es-MX", "c5f3b50b0a9c5fd62affd5adddca6eba86c6c55dd7bb7a45d485973ed8bff7f4565040958ade4d9ab88142363fa3f7b5aa6c619bf80c6b76623dc8868a4198fe" },
                { "et", "9fe886027f3821c2ae5ea05bbe21eeb70bc1c087e64b00b2574484bbb6ef767c4f41832d59bc6f3f673012e93ad30a2323f02353579652974f93472370167d51" },
                { "eu", "e6e87cd00c6e9888df1612dd70596ca76b2348eae0d5b542b330bf0ea43af800ff2e5794c6baa6565d421433bc5f27a2f99c15763be161ff7a05a8c35dec5fc8" },
                { "fa", "60a21b211859e3178d7796c3839e3b16e3cf38a0aa9deaad0b9953ed09b30a2f9ec381f81485a2918af10f3f696540cc11be247d605a1604c935b96a529fc662" },
                { "ff", "15d7bec6e6b249eac595a8242d0bc5a703334445aca62949aeb63ef1dbdc404ede7dc271ad31a39d2c61ff93f4bbcc1b7f7d7cf8c7a5252d31d1371044609f9e" },
                { "fi", "77c63ac5b47a66b8e024ba3ed2ce7ab77d9e96bc5579137b721c4756005d70921c20f51288d8784fcc9d2c52c883fdb311f5368364bcc1604d20609e18c3d342" },
                { "fr", "7d211873906a7e1246619c3c4649a4c3df2e6447b65679aa1f590bbc7131fb742675a73be93534576d09d8f4c23c23b56a6c70f349ae125afa29e0dc7c91c40a" },
                { "fur", "9454a8305b1523375082b3f2031c2f6e34037c519d9063b4f8d3b4dc4913aff5050b7ad72dc6af826194045d70327c582946ec51e60f465dd3c1b1cfb94b0c76" },
                { "fy-NL", "eba42340af6a5693d3f7f389d34177693e7ef79f474a6a416ae8549b6a00a3eeaeaa342dd1a835a44198e4829a40377ff28e6df8070fe792561ba23bb961ba08" },
                { "ga-IE", "ab1993ff182d23d7d6a246c76ff8d551fa541d5388d92a5699ad8a1c221f2976efd50764dfbdaaadae05f520100bb67ee87c1e74dde93679db79c0eb5fd16b40" },
                { "gd", "8420b2ca3fc8c4c9c859691e38cfabac05d859db789c2fd93b0221894adc93925e03a1e462a7d458c8d783736667d537b14c9c4e43b05259618efd742d23dbce" },
                { "gl", "84d173a8cf68d75ea17c942d302893e4c3089e23f4e5117299c5d4c3954b16d429cddb52a9e1990f0084a7dd1def836d792d955825a56a88f1e10db16d538fe6" },
                { "gn", "687cd9855b262559229da4c583b49b1c0c7e13c696131f014f38a51cfb8caf5bcab5c226aef75619b868fceb200276481831b0881dff167d850125a7923788a2" },
                { "gu-IN", "8928259a56560dd1623267aad36947bbd7c196905cb997d7314c16325149c79d49180468954f190613fd67590316b9d9bd048d6f2d7971f3eed90a3766f69392" },
                { "he", "6026f7aade4e8d8279c6e1f9a99cf6084325b846b1cc2e92769011573c5548e0845bd3a1792714088da49dfacf958863ea77489fa377c73fe8be5108e9684609" },
                { "hi-IN", "c5b869e65e71ab65b14e60f32993f00b7b6936d2bbcc56dd7bef72d32889cb114067cba58f881de526cb17390cd81f0eb5bc30f4f974eef09ccf463647b32a40" },
                { "hr", "1281f76c3f6c2024d11830aedb8b41046d575435ae971e937ad42cc0e9f1a392af074fc952ddb3862db5e8ac6f860e665813c16352a2142479b596c738f59aef" },
                { "hsb", "66f254067e32e5e94a3d518bbff3476951a6d8dadd53dd0feb2b3bc304367a393ac9d4e98f950f37027822f4f0109a1d0d7a2476af99f8eec90afb83d7489f12" },
                { "hu", "4af505994e06bf9f09e8783db7b8d89a19d5cbe9eec4896582741e102377cb7a57e993be23ea5bd5f5c2e06f52ec798e61056477a6fbfa945351aace30265ac5" },
                { "hy-AM", "b4320bcba93c43000ea9c137dac42233e23e30065d14d38d42a2ac5beb689b000151aa2fe74762d5efd1d6065c14ce96d2173ea7891e0a4e168e48df854a04ef" },
                { "ia", "69087ac07d67791cba395912dccf3651630f0b74b023a97ed4c2b43bdbeaab0df5c199a4b0e4138f86c86af83ce4c7c0e1299727c7abeecceeeac38f16690bfb" },
                { "id", "d22cc6b05311c5ff3a0f9f7c30813fc03e460590905a8496defe4f4aae8ed4ca2a525f92e8d94769d002ed92b05a537b05978c74c7251d48c9ba983bcd737558" },
                { "is", "34e61098166537eac112d49f1768a0e95ae40d35a89b8684e042c3d7201e406f06f81024d47dbe751eaa3e652290baa7df87722a3f33d99343775a5bd1bb3421" },
                { "it", "8974ab18c52d1bfddd5338deb7b79277eb3aa8f773fa04b2aa12fc8c681cdf6f4c23f92846943a9a273b3adf0b9dd2190f50ad548d7290d95972a578b305f5cc" },
                { "ja", "410ec972d3b65daeeef09c12a8ef653550352837908e214b9fefe15c37e78c967dcaf060f0869b21246389da7633204beb315ced1a5d4b867cbeeddb3c9d77f5" },
                { "ka", "8c4758d63ed25ef94a9ef9ef5b42e221afaf4918177abfb399d5f7e1f1fbdd59eba20195d167360952a2f1337ba86cb0dc258cc116bbb1d388aa9cb07abd717d" },
                { "kab", "3c9e2ed1be31495eccef702fd9e37349a28d6d27bf3c6c0223ade09ae14c6af97086188b60f997fcbdee10695051856dbc15fd74f2fd9b94ec5484ca70cdbb55" },
                { "kk", "42296a79be024c8804333fcc218af45bff4edc7ad5e258a4fd315285a964b5ff241809e0bb6d8d4f5d12958891176cecb50a0f24253e764983504f77d00c3c6f" },
                { "km", "0a8f13b950e423c5c7029b6ccc40124ca0ec9bfd78e1e710b7aae95c80b91f6a83c7392c20c11e74685e5ebac468a1bb2bf7ac8186db49aa55f765b85547b620" },
                { "kn", "c0b2dd2980dc6f52fb5a235ca990d84a3715f840addb03f4fcb9ff906f8ca1c48e045579a8a214210e0d2c1480f0aeb1d01581bb67b76365de08b81429082ea8" },
                { "ko", "5d71efb75c2e7889184740851c3fc6ad2b6b6a509a7334ca65339c244657313601c154db66dba4acfef1fba3fc60676997975dd86a842cf1fa689f6cddd33223" },
                { "lij", "011625e1c0b1d780ef96a5cd0400cbc9a180bc8537f91379c2263a4f351dcb93e25181ac6ad3020d427d5e15b933b96f542d1fe3bd13f1d628286510dab76e5b" },
                { "lt", "eb8c88c384a9a47f203d75a6194bbde320a416438d1fa3bda89d5584abc9d82357fc8267d4817cf1ce6582ce8895f2e9f4674b806aa0b84db3db5f6f2ec25819" },
                { "lv", "49497aa9f6ca37c25d78a62df66d0e7ac5c2449f592d3e2ce669626d30b508588df6bc9f4244816155123b3bbe923a52d88ea63322031773700e61bcb34fe835" },
                { "mk", "73dcfebf4e45d3493c5ad1f7a6d4aaf57bb41674a83ca955d5123a3296cca917542b64e9aebb9c0197c3976880058c0df63e48fb75c48b457dbeb7f932b46040" },
                { "mr", "707949b188cc609f5860156d494e6a8e7357c075e571b1d31a377c49b3fe55b0e273932faf3c317446d98e9c6fba6a49bd54dabe71960214e9094d44586f1556" },
                { "ms", "febb7d775c631299561c6d786b91849e96c2bb25190ccd35a7784709a197bb6c730538372969a4482447a45d82f48bd07260d2075fbc3030a5abd662fedff726" },
                { "my", "c115ddb05e2261c470cb6285602eced515c95ed5865c74e47e0e63c17a7895063869e02316a620595021697fdc1d7b23fd12b017071e0cdf6adb5debffb4d2a1" },
                { "nb-NO", "371d7648b0af20601d996acb3fb47330dc7cc67614c7df60ae6fb3bf6ce98da4ad1643b5ad8edec8b1d71969821f75cb824782937aaff706e2538dbb557ddcdd" },
                { "ne-NP", "f5e06d24cc6591b90621aac13eb88b8db08d3cf3d6a3ca8013a35e3ac80aee42182a7d68000aba8998563ddfdc244817f26b38552e159c9fefea51a50fd04d19" },
                { "nl", "2f6599fdc50e609fa74cf320c5bcb7e33682024069ab9a21a3e6fbb9c52ad66c94c4de1b9dad1e4d056e77728f455ef99fe9fb58ce87a60a107c6e2f88b743c7" },
                { "nn-NO", "584cdc177d69d35589b3ae2a668791c03597af5c01d269067bf0c100c150a8f84d3343cba4d71106403c58441511e488981f2bde77845012a6900f81073d9337" },
                { "oc", "6eeac0e448dfd5c1999a854fea66064441154324228b4954c3749e97addc6a701fe0cc2c5047357998bf38f233429357fde38bd44ba52f0440368781e1af8ed3" },
                { "pa-IN", "74a815cd55728a5e05d7b7e513101e7920920331c74232ccc2c84338f50ab5aab6fc124cad1ab900c78bfa31fc0f4762cbb1804e4afdc6eda1099959a23668b5" },
                { "pl", "5911b0cb5685cc8a3a79110de00b8a5deacd048a265361195eb231e49a3858a37c62421838824cce733dde03434aa8e11e5fc019fe20a5a22a2f62679ed44e4a" },
                { "pt-BR", "11fa9107a8a3d71127790dbec286120d5ea451dcd3235a93bed15e946772eaeb02442200a9bc7fd9bc1fe3fa8148600e3b92698d13e7c12cf50949986b5ea062" },
                { "pt-PT", "6b9dd90137eda80a1d5384b28655fb8ba4df5f0c5cb91f8ea21a690ee90ffb72453cfd221c63079c26f1a3a0551d29ebd377740d2f62f2729aac541d99fda813" },
                { "rm", "86e119c4e90c9d471cf93307bf7f0eae8c836e453cb28c89e71e2396d897ace9d29225bdff0ecc37317991af248ec6a557d6ca9e3359b8700c7358bd2d3aef97" },
                { "ro", "9dff80f0f0231ec06e2ef43d75db4ea0aacf35a50383afe745ea7e08cc2c96911bc49fec6a35de6fa66f8144df451d5f1caad6f466a0d40ea389d4a07838d70d" },
                { "ru", "8bdc8c1f0624f17329ed0b296491371f1c7f486d58289f4b22570d98556f33181d41e8ee4082b6192e47af788fdb5390a6c93b7e1f39ef21e5c6b4a964ded336" },
                { "sc", "e2da84544f24374bf6263855de44608a1db68c9ecbf371461ea41e9c7d27b52ada14929f9f475016c53cfdb45dc261614adca2a757695edb6872154e7b8f6924" },
                { "sco", "ff3eb648d7c72fd1057d99964552c4457eba72eb25812daecf4accfd6493d5231b7894e84100a25fe5939ef7500595f443bcec6a61b525137a0d7b3c225e9d53" },
                { "si", "d3cce683213667dbe325bd6509aa793b4451a1671bc5a62a9a32eaad1a76f5593ef551409a598baa3f74e000c146d14fe661b9ea1333d4aa5659111e37eb18c5" },
                { "sk", "a90f14edd50f3fb752a11aebc9a9208741c349dd9370bf7b59cce922289bd572669c612a059663dc164d73a4e0c2e8eba5e09bf9b65f4714d7a5dd6af7473613" },
                { "sl", "7630420bc50c7edb80474a1f7379bc968ae0f13823f2b8c785a551e022fda1d32dd2bae68daffd05cc8f206f92dd951eda27974e488e523167a76d95feabc47f" },
                { "son", "ad5a598496b18d39c6a2d7536ec7857900f520bed8bec52ca143cbea2cfe295f06908abe859d1ac95ebcf49d165f5488f6a40d1857b3ad691f0b59026dc5c0c8" },
                { "sq", "aacd2bfddd4f416d4c4bd2677bb9df6a2388d7e5ace311e298f95db55cfa78c8fa6f61b7d1d706fefc6c3b687a0846a7cf21f5c738af53ce491f30adcbec4348" },
                { "sr", "d7ea12befb75ff43ea9e12f065f349ae5848f158811833dac96744cd919f7c9595c6810fe72e9d93f0d54d0824489e51dcc73b4631c8fed02169f15f55f751cd" },
                { "sv-SE", "bfb345a17d6455c552c5ee3b48e0473efaae4fae3d3e4a2f4dbbf8d73145f5ee5ac33b04005962035840817f9845a5ca58165bd6fde0aef1c11a67f4478e5a08" },
                { "szl", "b51bb78245f740e59ac33d6dd2f94434e4a3e2a54ea4bb81ba4d6ef4f150377dfc85497599a5c4826c415d29b65d1d8cbc6325e239f5392e7083201f67e17a83" },
                { "ta", "932412ca98e45c7dd7aabf74c92756df540b6ee851bcc22de0dd39d943e7f97336dd44de27c697b4ebfa621566fc005d7958b1e0be37f8ae2f9b3126d47f555c" },
                { "te", "5cbf2d481b936bf3caa53bef9cf975832957796f7c4703fdfdd13b4860f1d6764ee6355afcb7512463901b4f388d04fa178e7895bc3d20e23c739615ccacfdec" },
                { "tg", "64c316c18d8a9855aaf8bfbbff5b0bf06dd1432f2b02a5cb712310ccdf56c1cbe5149eb43d2fc46652fd42105662ce02f106ddb91d5862df7214ea5e0592cc73" },
                { "th", "fb096a37dc3457c7e0ba33c986783c9072d55c1323f118b27cf492fd1987bd84cd6b2aea462e18498873501f3919b95ef4070428f1d9a1d1946baafb81d4eaa8" },
                { "tl", "b0b7b60f30fdfdf09f24bce5ce5927d5c2d0d4bcf056464400e8c3ec535589313eee4515d0997b771cc21adb1d538e37535e9c37b95ffc36639f0def5d3f2564" },
                { "tr", "97af39782d3b4f02a630ce45edb831862367a1b6c98888dc1e03ddff5514f78de24c7a7112aff7bcfafecdc6a002784b5a6909753f1a55ef1dc4fdfc51f481b7" },
                { "trs", "967917fc258464aa01537d88ab4491e1ae3af3033af1b9628c064899bbb5fc84385c8266ac097530dd4ebc2e1f9e83a4abb1c9c29a1d6d5220e3083a0ae78d14" },
                { "uk", "d52fa5b1e437d9f29f7b5611c1c97ac2cfb7ee7bb4ad3a2bb4137743f08daed6ce43459d639a7da07a56641f29947635d673e46c2d08a8dcab6ca1acb3b57f32" },
                { "ur", "16ba60ca7155e1e3de58416b81ec05d1b37ae5e071e983d4f215c98e3ef17538e6d7a904495aa4324118568348c523cc730eaaf97fb862d61c6ef82f57eae13b" },
                { "uz", "92ce0d34cab41b182a48e8915727fc6be15346c700888c798fb69fa6582524916d3581433edb700a25ae8a2e05c1315df51424b68fe0c24b3da714727f8f96eb" },
                { "vi", "93ce8f120cf8dcae6ab7d5357e4b10bf00466dfc319a13e34f28b732bc777ad6904f220e2f931bcfc73d7a5be6c77abea5ed2cec3f4d1326311bc4aadaa86143" },
                { "xh", "1d1611f2af9d1a2ea7dfbe1113d9f28e622d2452b625a6c69e3a34a9449c2ce35166221d1d7fb5600318d4e35030db5ff5c689522f89033e914d6ebd014b53bd" },
                { "zh-CN", "3040618861afd526ed1e84e79af68ef8ab73178587780b63b5a85cb8ee7127d4d3c7cffbc66e61eaafb4594113ad8d6c435eadbd85454d196d192347b847266a" },
                { "zh-TW", "88e5aae9fb0f25158b73121f17c8bf43c11755e1bf590a356503e61402da66141e52982a934ad7077aedc1fe67dc9581f0385219bec6d6c5518fd7618626ad19" }
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
            const string knownVersion = "113.0";
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
