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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
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
        private static readonly DateTime certificateExpiration = new(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


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
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.12.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "887a66ce99312e42b6c278a3d85a8ae96f9c05422187452d335bf675b4817c6f330c6fa757ab5fa58ef7937916fb56ada4a9ebaeb548fda6114332f86b5da7ec" },
                { "ar", "f073b8c6abde0189aa9f2976ed020c6926b2f1301463655122017146a31319b7cc9e22a9d88d690b435ba39d0c122349eb6352dc2d038305586f2782f76ddd15" },
                { "ast", "b9d3c7957a4aa248e534b27099b826c10c0dfba3b00715de39a2c853cc35a9bf154fdc4022fa50b4c7e2e9bb3696699fe211bfdf680e9a0d9e0e522d47cf788a" },
                { "be", "e77c988323ffcb25ae56ead6b3e6742fde61cabb059b8b122b36fc3dd439e0de0b5859a6051a8a27368cc843e2ca5de5aee5f6659f4325e3aaf0ac4486203920" },
                { "bg", "d91d32cf7cb90c1c5631fdaf58e2a4792d8640ce37eb8377c606058d5c72a04b830d2c96873a8f075a0a40c5f89d53d90412d1f052a3d2e31ee576cb21856940" },
                { "br", "4ab8ee3b8d2876c345946b5a16365cdce95c6c9c0ea41a85f0a5b605033311223aef9c06a528970aa4f43762934cffce1ac96f3eaa36f46db5d38fe3cc72e9f1" },
                { "ca", "656beb0f53c05c731a7c580ae59ca569504ada470aedb56aaae9ee3d2af0bb8fe8c9e8253ffc04f0589ef732c6025ebf4a0ca9e0a398832541b1fa31fc261688" },
                { "cak", "ade3a620f9357d27c7336314c7e79f52621e10e1b3c6659490c56a79881dd4eea2afa60005966d4750b58a7f46f8508a7b77cb947895c455d3ba7121cd74db50" },
                { "cs", "2932d16c91c98e90e054189a9ff6f85ce5b3318dfccbc08937c865f27a0721b4570868da8ed00adae72b4b58097d599b878eb8d51403415492fdcfa0d45d5afa" },
                { "cy", "43f1cbf94a0a681d81606a1ee0667b4160f19dd4db2a1619ed33469d4e789d2652144f58b9980908b0eb3ed5bf28da87ad73a72ead637e1edaab932440873a59" },
                { "da", "ee8fdee96a99fb50ca4816397520b8c5adbee95c16b7af5d00a517c6bc6a436df349e4e329f1f05f31018ba2376fe57065f3351c850acb99f8d27bddf40dc9d1" },
                { "de", "588948abeb740d01e44564d9a8af1354c45d63132286a79df28b2e671beeb4c71488fe7dab192cf8b8597556c6124c6835660c3adeb6f2bf672b6c3fcbe3cb8c" },
                { "dsb", "fc809371173f2a1b80485fbb2ea44c48a1e978b8d01cb322bbcb7788de31cbbcbf2777fa9a91ebfe9cdae0795edbd4d25ed95e7b82e326c96ef570f4fed368f9" },
                { "el", "083ffba7ea0714a23026cfd043e0b5dfdd088756f7bdcf21ec7899a8f270ab9d118ed370ec955060e58bff8a7580a9ba1a9e69d2070046b4389aaf57550debed" },
                { "en-CA", "ee261ea873c294bf39ee09f986a2cf82f0667817618112ce5827305c958b1f14764cc1b6bbc070671580b607390159d869f10605a21290558ff48d260eb2ac31" },
                { "en-GB", "564ca29081a903042db61e052e662d077db208d898846d0e9aa683d770fceda13e13fa890a96b1d06eb3c3b8ca2d1963a3b37909cb69737b5c31a6ed7807a018" },
                { "en-US", "cba03a45cfd486ebcb91cdf7bc9c9baeadbd056b0cff0dad0e9532d37985d0587193cf9edb8c0d068609c517266d245b66cbe4fb589f7440f18d31f1d869af7b" },
                { "es-AR", "9b65152d455c2a1ee6c82e7a98317b714a374b7ee432955aaf4c53c646bb77cb1631e94400ea20b07ee5c5e5fa173dcc614ac286bd7217dc78d50a10c37e3567" },
                { "es-ES", "59e90be1198be9c3982c548e233cff2ad0f8dd19dcb6ddf9df7019e0591ef75031c339e24b5e4e457ed4a7732744399061034d18086dc39d09ee8f1a6e2653bb" },
                { "es-MX", "eb4b0829a5e88ec5c9df29044ce640fdd8bdfe446d9b987936f059d7b5fbb046e0cc438b8956bf9f57f28a6c225759cec5a9eec1088ba2e42fb1e1772f2a59b6" },
                { "et", "7dfb2d4282f7c52476dab5ab0375a7e2b6c046b70e619652132a1d6df6c38fad86cc0f57a853c916eafd1c304f8d115809321b19b4c6b4df044bb3cd97014e8c" },
                { "eu", "c116d98aafc99ef62a454b951f00f8237653f6f027659e2484bad61ae262268b2e914492af0e0039c76536ec7fda5eae719767430cdd3678ade2a1842dd68b61" },
                { "fi", "310b0987c2ed8758958020ef818a67c40f6efdffc60b5f13b5a5338e3fe402c7310e17a859a09915bc6244c42d805d2b3ab965b0c749629367641f351dc5be8a" },
                { "fr", "b20010dda88a0ac1eea32e32c80c0e2a7ff6acf91c1a59592f4ebc493c699dbac7cdf98aa6cf30fe79163f1ed0c1ae41292e06f23a4ea532872a4e859e1cbdbd" },
                { "fy-NL", "66d378c4abf16bee9ad86b6678f5c91c7a4eee312840eb2c29cfc28faaa777f4df3c2192a8a9fe350aa94a9183b04a2b3ff4b41d1b0dcb94098d2372b89338da" },
                { "ga-IE", "6c809d734adc96fd69d4b0a472840c1aad5d9396e4beeeb002e1fcf545116d4509a918cf9033fb0cb98f43def0de5f3c7843a2d338c9605528af7deb7f3ba79a" },
                { "gd", "af1c5946001e459d13011b9084cdd405c2ff1e80b087648d4836f9dd8322882eabf29319abed0d2b8c54159aef34cef6383bf3f8669bd8b717408ba814fff694" },
                { "gl", "29a483c581785fc49c5b999dfda5689f3a63aa4d6387fc02e2edfe63bb8c435a50b2d3439c848efaaa49d53bcb44a2b6d3c6137317ac75254cd15752af228068" },
                { "he", "beafef06692bbbf7279819402d18e5d8f8c1e487970a13c4951fe7a40daf88e221370581d7198f9c8f6b73fca596e0c318cc812b5ae67a23541433a14e94ccbc" },
                { "hr", "4ce9e9793c80e5fe8b5129331d0ae85868b436ffdb4a4ad1c2dd02b1c277c1f31285bad659b17ded0c12a6a9fa22b658f8bd106c73348d8fc005590f39406c5b" },
                { "hsb", "ae5d89eb58321a633edc6813fbe9fee989ec894988c7835e1ffd86e5909f6fff04dc15cc59635326fda9cf6ff390ceff845a657a7f99c0950b0fc3a582c130df" },
                { "hu", "6e413e06eba4af5748511b9dd9e78c96865ac7554aac7fdd9d54f203f5ba656a89536f8be8a0b4ce4387dc849e69ae9db0e75b5a7ec2d65e4260b91da6ada0d1" },
                { "hy-AM", "e84e2234a9983650d9e4cf911fd3f45d1004132ed836774188ba2eb5e2a3944f364a6a53469e1cc6f05d74d9bbf134b71af51769cc26002058b7593de2239cce" },
                { "id", "90aba0faa76d816a6b011faa4f4ecd88af4d9b8222b9b58f6d6e16bfcc1e23bbfeed187874e326c53e4db6fc5d8871c00fe2fc86cbcb361ee39806f6f084df1c" },
                { "is", "16f33139314aadc1b22695d2cadb09a531d3457f4fb4c4f6f83edf0bccee3b63d6be065744e0bbf0507731b9f31da83d02d6839bb207df5413cdf6ec2996f2e9" },
                { "it", "9d917aae479276402554d70d6c73b34a6c5485ba784198dcb3afea53b56ea28e3a9f4b906a269db91a784300eac4585d54edf65303b5ed90111bb9f9cb9efd18" },
                { "ja", "1a54ecfbddc78451f5e6889b135034a7c2fbf2e3e1c710b96c929e99ba0ea0c8da7f6b17aa0ef7c2c036050416461cf95dd1277891dd1b7544812d7326b08c24" },
                { "ka", "5a30e43c5a5d391b03e425521a382a320492505a457d2f9f9017084716c5cffdc14d97af216155aa8fa16248dbbe92dd033f342123f5dc41b0a10738619795b3" },
                { "kab", "90965746b558069cacd5e61b35c2da406c3ec7cd86e722fcaf1f86140021c1c2dd895a393ddece2aa532bfa3612ae3b31a09f474f53fed1c022e1dffe00cc267" },
                { "kk", "a9b2736fe08f71d52be7db47205bf0ff8441c61ca7685e99587d6ab10547fb3ac4782405baafcee047baf80e7c816f0fbad5d47141de455eebfa02e30beb7d11" },
                { "ko", "e7953ab3a0b6bba4c6b83bc231795c6b00943a605ed49deee4a55fddf4f46956787a48e13f1b710305cdff01a1069840a58f7982eaf296dd9a6ed0a2663817a5" },
                { "lt", "62829c8622dc51adb6be8785b1fd79bd132f2bfb265ac3cb64ecdb349f3d6290f4e8b16d0f75abe978f098a7bd29b71287dbf635cd912592539d6385adf2644b" },
                { "lv", "a1878f6d439c29dd32c79c3499081c9f1f7a49cea7353b108c0aa3e22890485a694c7d49707e948bcf94dd0a90249da120f8fcd60dc837c7ace018229f116d60" },
                { "ms", "451d2c10edb712f5d3a44ab0f513004a9a5dd8b6dde7994de6467a00a2da9a4e3cda78386a17617adb9aace3330ec9340d54d7395e5bebbf8551f33f9db48d9b" },
                { "nb-NO", "f9ef74b1a60954926847fc393da7eb819af2567183f90dc6d69ac110b46d8e23f5f7c86db321e86ab73094fd32f42d81dd56165f9060fbf333db93a08410551d" },
                { "nl", "644543381ccddb1c4600b35ad2241cec491018b1081a54c06ca4f8eb0d9cc0b334ad0fabf6a70ccd55e3262c46e7dc85358752bdfd31693fa937088ba30aa8fb" },
                { "nn-NO", "832f8caca02e78ba1adc89e2654168fa0042b391874f674c9e8c2b7ec296d344f7fac5465c681e07c61009d2da93de24bd14f76111d14f2139a736ca9debd409" },
                { "pa-IN", "e0faeaf831952ec05f1840ad5d85b5c701920bfd8a5787cca11a6a09d117f47aec2fcafa20b2134abc2bc42ec5a0318576430e2a68602c03a4dacd08f2269dc9" },
                { "pl", "9384db003968d369d48c075750a13df74b506fb21341dea27c441181bdf001bf054334f0c3a73b1cfe4786703132bff3876f560b0db5ee8745c1354235a6e4cd" },
                { "pt-BR", "9caafefc2059515daf6c0ccd7ed5dffed0f9bb0b193e712b698978c08afa12e2a67f215d140fedd4aa03c85632e502702ff53497061b2fa08b90f4f3b7f5df95" },
                { "pt-PT", "37ac2520c4a8acd80619f24dc8034adc1213b23650fd822ecadbbe950cdbae97f45d17b6dfd869525b032b1ffb4fd40ffe8ab29104c5432c02249cead02cfcb1" },
                { "rm", "77871c7cf0e86ec88e929296989cf7fa50c409bf10c95004dcac475f6c3e7b03f16e723ecfb3d32352c5999c8774ca6adaaa203c6100f6cb545c6ae0c8c11648" },
                { "ro", "882b54724c7d55657b154dfa30e471ac0205b2d279321042e14639a9045fc89e37fa70a49d78a95ee2ec9a090b403b6beda01bf88247f53148209ae8f7a0de61" },
                { "ru", "b281b7c451678d34d005334f2ac7c0df02ce2ab3ef7eddc150820c240e7fbd9c5c6308f3c62ed90cd2c2a082e69c2b3ebc3dcbf6721371074af6a3e08a0b2d16" },
                { "sk", "d98fd00ef2d7b96230bde214f63c733a1024c042d59a90ba712e5b56b5bb91048b16a0d81f386319ddb679416c0f4ec466e8140c24dc52aed17a1aecba1ff5c4" },
                { "sl", "9736db0672fca7ac102e3c8249c4384b8cbc42d8d91b0d63a0a125684d30bd09b7220834db81bcec19be49c48ea7cf54a211aef505fc3bd0be56a797e8f7a2f5" },
                { "sq", "5957166d949550941597b8d3c32b2ebd8ef1e86416de3340b625fb949c337fcca90a9c39bf46d13147f7d4c4af61b4167996244c876fa7f2617093a43b5215f3" },
                { "sr", "ba991ca8143d264b9240b53b9f5f9559778a469d54acb7df88f57a3c4a6f96037f4a1aa1178ddde038d80b61c616552b2f3e48cbdb6cf36033fb4d17d3c942d0" },
                { "sv-SE", "44c19f520b8c623193aed5a12a2c529834958bac6c01053ea5724ca4bc4c0728ae220129f635e24456544fde043782d1e288c997f0314983d90cda2ef120833b" },
                { "th", "e884d92d716193544f12b310c065ae6aef122e5ec38bbc9f0b4bdc2eab7373c958cbce80104a3c84ccdfcd7e1f702887458cacd954686c2af53a76049e03df39" },
                { "tr", "ef7665d1cf3eb077952726861270c7ccb80a576fb7a3e3b39942144272ca3c80578c31e28039eaa734dfeb2f954c6336da1ad7ed1416c32c91f218fdb0eb6a03" },
                { "uk", "9ce14b34c5ae495686760f49a1ed8b95a4ede3bef031fc0a786c70c570bc26b642ef80074d6de158ec2018a76970cc7e7d12b05b0f23d023438320e396e8a899" },
                { "uz", "6e802c396361d699e6fa859a2bd999a8289eb8a9903cd2adc0ac267d8fe16ed5b34e9f72f51377e7fe66a521c95dc292840e2ffce5f88afefd3283987595e94c" },
                { "vi", "438e12cea8ac4a3e0fd6bc3e2896791466956515e18736757bccfd12be75cf4470626863b75d0051eb8e704ddb1fca26bf403c09da20c579146a5586291b7f18" },
                { "zh-CN", "4d4a6ba5dbc928831ae07ae364623610e5fd8d8c77cde1e30231af214a589be28f256f0cf3504930ec0903495be60c7ed65d96d0547193cb77d2cf77f7f0b02d" },
                { "zh-TW", "b355864e4dd60ca06cdc6b829e77e828c2b2fe249416606434db17c991f1654dfb2d1b75c8208f5fac24535c68801bb1cda62e58f6fb79071bfc9a4ce9ccfea1" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.12.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "940993f1e6d75138b27f54788bd80feef416c241bc44cd4eb4431052eb0e17bb591a857480abf0f251547d1acec166f4a821026dea78fdf3bce53a80d88aa7f6" },
                { "ar", "1d74a838217cf0a78795057b7203c432e93dfa9ec08db2a90578f8efb41f93de72c38a871e0f9403202736e6fc69bcaedfb7cfe23872324e83313d9f89f2c81b" },
                { "ast", "782d0ee5301520160792421b4a6cb3066ce3721853f0fcd1e73d271eb4e5bc83587b8032ca10844768a66692f088834ba6e62ed53370db214e0165bc466f2d54" },
                { "be", "aa6e3e4471d73de316e3caf18bd3044f4a27756a2cf3e59695d6e9bc331ad324b13980a3371dfd58117cbbacc4ebb1ff88393fc37412f113108c43ebcc5538f0" },
                { "bg", "4f3c59ef587d70c694eb1d1ca40fe106b3baf413208f73921edc076bbf93a7d099afa19ccd30514958865a45c9a7872ab779deff022e61b7e6fae108421629a6" },
                { "br", "117313994e4968e7e966405f71aae50a6dde72cc96b4a8c23b3747e6a8cbc750dcfb8d0673a12c2fe64360f0692e2a83656e3fcf1329bc8ab7c8c5f956afe967" },
                { "ca", "8604d013668286c65c610574c7a6e1ecb014f90e5c9d89b7849e933873431280881500edfe0f16c27e34efbae37b1d1922357dd686bc5bdeea780293bb60bc6d" },
                { "cak", "d813ba8e34182fb375a5e246da51c82210b38fb9ca2690124c9a879f006eee3ad2544e998c82f819884ff59e6bc982eac52612678c1860a69bf9ad32972d236c" },
                { "cs", "db0dd7386d7d07d475916c049e2db05ddcd1acf21e1ed20911a68b44a50f837decf367fdb5def7b7b82e1782e31e17e9e6303ac5b5c7ae24cd8fcd95afcad928" },
                { "cy", "1069353ae7d04f27797f01c0d1db9e575f507902b92ee4254f9adf831d2a1683eb5e443c8a2547ae56579ffd0ff1d62ce2c97262cc704d29df511b53b71e9f7e" },
                { "da", "a544d944e51033f5934fb01853089d0087e3fb2846b1786364ecf9bc9cbab743e2edd038fc546b87f42094c01bc2d2b9e5962faf2a00ee02774b2c09f3009534" },
                { "de", "f1490b988a437ac8b168a84ab4ba8a021ace24da11c678f99dbde0368edb4542487f80db6f621c6f69294d0a4c7fd95327c9b73524a111df953039663e615139" },
                { "dsb", "2e4135c75487f67afb45979e5380c81d67a3f72c7b346a0f1c58f89380d311cef5d671c00e41b0999652ae9e92bd4aac02540bb3e8f189eb5001eca6be8d8759" },
                { "el", "e0c40780b49a1ef91549c695d43530a5db6f8a62c3ad5deb49cb8a3431928a762d38d7589b5e6ca6d469f21402adf6696cf9b3c850a5f9f708188c7a6d959e84" },
                { "en-CA", "45d516357a9442f7dbd224d99fa0743e7ab0611a83aee0d4b428866ca14b3c0e012bf1ae979f338768d46747bd13f218338104551d21e612bf6ae4a3b04737c1" },
                { "en-GB", "53ce1174e353a3090038b0496efacae72365bc684f2180e4b05b458360f7a5c295687767c535747628bd8e3e22e859947f71f93e7a1e85ac047a27486e4c34fa" },
                { "en-US", "3dd211e4a3110c468b796a3fa3c1ed6f29e63ac8d44ed9fa3f11fc7fcff1c7bda4756e314508b160cbe4b8ce849345eeb96c23ee47a932ce0618209655c64b76" },
                { "es-AR", "fe583fb8aa0420336fdf7c28755d85990c14ceb88a0867c336231beaaed161c1be23f24505e0637d3c3ae623141ed34ad42e38571396c4a5fed670cdd7e8e6e2" },
                { "es-ES", "0dcce1799394435857907d187c7ac134d2ffc26d081d81a67ae6282af02d9c0896be89cf4d7b0962f27df3db015ea3eb4df4a90ccca4cc978a25efd71a932798" },
                { "es-MX", "b0be386025312df50fe785d01ea787e2fd308e7e98be964c2bf926f7efc51edc34512289b74d8f5e2b040ce8a3a7e0a31fa48fde68c9f625efdd0ce7fbcd4504" },
                { "et", "2f531230ca9efee7308444096a9e55b10f5c8a405d4942cfc81287ad344b023e7dddcca2fb8e6ba7c697e6ded2cbfaa90a1e54f75539d0838f7582d5326d2481" },
                { "eu", "f9877db93e99859e1a69f4b99698166ecf309de325caf2e419f96c0dcc5c026bcd12514579dddf4a2019837930fc79668360bb6023a96f4fe34264aeda5a4bb8" },
                { "fi", "587ae4cb4f69c1e50cfa37ed830cec652202d99c3dbf6002790e7ea082b6abdf547bb00b85b4a6a3a1816289efcb007f97f8b75eb61c7ba83910e98ee725d53e" },
                { "fr", "48bbbb7c0845261658fedbe92ccd82aa4e3dede51ccd0a04481e61c490f95192c46328f66a90544b7456d98673932e770aea7d21ffd8a37c2868787cb855b1c0" },
                { "fy-NL", "dbb34b1a8a115c76d9154abc73f90a0b7f45b44af2bc51bb89c3b862a4d4caddd76cb1bb3a341ec00b7259f7b20849a4d6ee2926cba406b1963e409cce88dfe8" },
                { "ga-IE", "dfb13b3110ef0eb487a20abdd88d49b92567f0cf52fe27d02aeebbf1a056c0846a7aad29e20d138824553de264d2b10e43452a1c4ae8301a2ffa92c617e70e1c" },
                { "gd", "3f55c89f8c89f8eaf00279f0ded255ad1bd9f5396767962c5feaf3c0b337422e99fe3594166f12152504547e6be38ca5fc2ca88a6d559f601cccbd9b8e0f80ef" },
                { "gl", "4a5b197ea91326cd7e3895e8eec3b8686ad7863bce6c505543de903e5720d025cb9df648e28e9e5b09a44d86cf5bdcf96a3ad6ce7953bbe1e1d7b7cae8964039" },
                { "he", "c47ec4060ac6090e9c27d926addb8d9ba04c396a9dc3cb127a14c84e858e1f0e9eca36d838efee3a238dd3b3376d4bf4cc039e84999b14e9d69a7f833be0f0de" },
                { "hr", "3e344407d57b14e93c2484c0e483c8cbb003dc2a33023a6f694395afeb09461c0613b189871615a5194bc60c810a99889570f9fd7bb77509a442a6a2a869d3c4" },
                { "hsb", "291fee11c89a85815dbbebf2bbdef7b123f4c441b0e4c87cad3d4cf92e0e0c43487094d658efc4b261c4738142f054e98e35872192872d2d3c9058d077f5062a" },
                { "hu", "99dcae7f8946efdf9be6eb604fbadfe1b286c6734703f580655bc4270a60dd5d23bb4e0fed8b25d172c45a535c9e4ecbbe2d2e4722236e2c1c9b1b24367d54a0" },
                { "hy-AM", "2cedd46691efaa5ee75b5670245babe9125f5be3429da19ad5b87d9b5955aec8ba8c65700970944b531444a5d5f05a2411f6683e65743f1116b6216d1565d0d2" },
                { "id", "c94ed94fcccc9520322ea3147c2e9f48b1f1631ff59cd9b48a5069266329c31ed90880555efda06d5f556aa24189e7a903760c2745ed1f9303f87d021cd2edbb" },
                { "is", "ef1057e2321e3acd07579ddf9c4b6f943abe9959bb4b38893e1f9c6b35bd80020449f36a4954a72d40dd6c3c52a599b254d045dbb78e77060dba214442491769" },
                { "it", "3d5cc4666b4e96e69c1e52fa68ded9827f0c716094de8028dea43b5e86174de951da58ed4cc86eab2a2e3446948b4e1e49b4f0854667a160e35bca28e6a2e58e" },
                { "ja", "7fb5e26c4c81334b33013d9bfbba2b77fb1e7ebafdf24ad0076c26f03022c3bd3cee9145ab7571bf4799907eaea9022adf0cf5751d4505a8c5ac532167a31a2d" },
                { "ka", "4d767a7b6a4cf662e06fc04228249b850e3995235d4d7077527a986b995501895bc8d9ea2899742f50b3d65ff01a1dd27c27bc75ba27de813dba0f9910f15b03" },
                { "kab", "af29b0d6c6adb2a679c061aec017aa4bab43950aca1ebf6b03694f30b0875c7d69ba5f19cc6d309cd55036de2478312f94a14991807b28e023c24d2002926707" },
                { "kk", "871eaf01178b136f6bc4d0292f2a054234723339f724289f62af2cfc91ccc231866daa98369a4e5bc605bedab3cb032275169a617d13bef9881fddc9928338bb" },
                { "ko", "51f2b8fde22c7802c3b2b23b2763ff9d1a39405f4b7b5865f9b0edfb5ba0f5e53a67fa74f2542a00dd01a1897fd65c1be9456b22306d987f0187a929de84852a" },
                { "lt", "b1ef989471ba9138c627b0ecfd2b205b244b1d6c1427ed86e97c7ef5dd7fb2fa62177d252cde21876f084f1bb3075c872316ca418c8f355bd5fef54db0351b02" },
                { "lv", "e05e21c677d901d7cf00f6f18f1e950f5576b8538ddaa69b67ad3d9d49519cb5cae9f4433623ec30a83983f693cd3f43df28bbcf8dfac8c10b4f1f811dd8398c" },
                { "ms", "a82b56586049c0a01e2e130d0fe5dd4997f1af0d0499f312be60ce26e1f512c6ec3816f7d7e59b5695e39f327a5b75025e35c5a5c5657b3ab675b09f8540c819" },
                { "nb-NO", "8c66e2736f488951a660c1351fce4d30a9092028948ea40a4639b78d45106bfe018605605035820c95dfb1db943f46bfdefcba4626c73f924842b15cbf93d7c3" },
                { "nl", "da71ec7974e8ec3d7b341cea1ce3158f92466006f64fb1fd7755b481945ac4d3debb2a677e8d47e386932596130ae5ca753ffad52f11e104b15359acae5ab3f7" },
                { "nn-NO", "33a4133d8133be39e53bcf8b6a75ad0c8b20a49f18ebe0b466165e3c979948fe9f79b41aa07a426a02d3000a5be2eee1bd2b35e2e2d8e2a1c56f1c4126a4c4a6" },
                { "pa-IN", "f32042196f42315f16499dd6d2e25c1d6793197c49937400aa454192f893adaf9eff1e7df448a2001962a59c44d3b9299d2cc42b992c79c9d5c601307439fa43" },
                { "pl", "9d75b3e595f2ed649ef249abf06db05e85e31f1081582aba64488309d94e71e8dcc75d4a1a4f72ff8668d8946d19b45b5cf10f78f57df3d88ddb074ea6eb3a90" },
                { "pt-BR", "cd3b13b68b0812ebab848f8245a2c13e8627fddb27769e7fc1901d43123d95414e25826f6e76a6e880588f716e44b8a2d5be0a2c52884fc878c57ce81cadb460" },
                { "pt-PT", "cc16d334f6374f5ecd4cb567b6749efd2576fba3f7d3bd19af3394a397bf1eaf4226648c8279c4ecc928d4ae0b7e394a18ddc8ad08ddb9c3cd7c5086d6936a1a" },
                { "rm", "7a0734c9f60d663d247efe3b7aba3b5d382d5744327ef7288df0b1d15d4ecb19c40744175cc553f380fc392d83a378daace099d1a0f6288cb44b7d3f6bdc0c4e" },
                { "ro", "3d009a8b7544bd97e902832b6a6b1ce8b67843bd712ca70559301781f680fc9453e501ce8f9fa04a2f732b4d53a9ae5992046cd92f72ba5ec169285de81955dc" },
                { "ru", "e627a21071ab6a7a3309d4d102a91c1336fd8312fd08a857805e5c219b8a901fad3677a6397d3c259555b88de23d7e1f66a184523f1d44ce97731732b0565e6e" },
                { "sk", "eec1f12b7974daf991c0d0d028c12ae0a4094767dab192529cc8917c6a16fe6e66cb7a228fbebeb3abb59eaca8465b957d2fda43c00f585278dcf4c18988d64d" },
                { "sl", "73940b70be5574001af9fd286a2728268b61e69245430bbd1efe15608c0a2e477009afffdeb4c327728598b07b6f472089f213930ebe0ba9c5b7c151b907fb81" },
                { "sq", "b0c3543481f37cac8e430aa2911cbe4def66d9da015a6948fdde77e9d8740c97660bcfddc284c52c8b3ba92c5f7d5d8ec4cb640de0583348a6ecd4b91a709897" },
                { "sr", "c729a760da6db95e27ad68de92272d4ff6145df3bc0a8496a4e47955e071dbcc9e1b5a83340806b3dd26753bc2f6977498fea1ab63594f14693d25e85278c30f" },
                { "sv-SE", "7f8944e9d592082c075e3603190682fba1dd53c98ea7598c9150c45a5a8db2311e3222855d4da2367152d7be152a77dbb2cac7bb185ffa350d257574147f8853" },
                { "th", "5be39a7bbab137617d7bfea1357f7325611fc70c62afc0aea3d490ff591f53a0fb693170776aa0fc1a0bfbf341547cbd37be0f882b1b5a75d6f88cd12f4df421" },
                { "tr", "5fdfacd9a373f592c33ff619ca327473e074ae695ccaa41839412341f38eb4860658573daea5ff159edb331d1493a47c43688a73da43a23d6ce482c539480109" },
                { "uk", "0e62922bec990b8d10d57809ab4d7e79cf9ab0ebc78f43835212e187b76e87662078afdaa9f51e2676a335c04afde90a20ef69e8a977132fa290d0c1ca419674" },
                { "uz", "7264424352a1438e95efbaa39f12acd0c182ee0c6dda9f42fb2a79dc22f49fd250f0b0a52d19c052c10fd5277b24c62fc8b5161c8e0f86ddb672b13869f7365f" },
                { "vi", "b3944643f9ef46e841e03987b0063b0bf4354c7d3dd65a6fb5a09f5eb2b4f8c717b7fd402e60bcc5c7f79efc3b299c646977e245ff8323533953d416190e241e" },
                { "zh-CN", "5e68ef4df322c2e910d69e8505f186db1421fd6146e22386edb256dc97bef05787ce36e7a749309ee7f53903c2520d24886a180f70561ab267302946c919319f" },
                { "zh-TW", "69f181e09013e5e4cf0dfec68153f69108841003f14ecaf124286a8c9af25022b4d80cab0aad4762e82c0926f27bdaafc0452681b5193eb6a7f6937bb8ce6e20" }
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
            const string version = "115.12.1";
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
                task = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
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
                logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                return null;
            }
            // look for line with the correct language code and version
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value[..128],
                matchChecksum64Bit.Value[..128]
            };
        }


        /// <summary>
        /// Indicates whether the method searchForNewer() is implemented.
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
        /// Determines whether a separate process must be run before the update.
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
