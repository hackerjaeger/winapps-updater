﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

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
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "114.0b9";

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
            // https://ftp.mozilla.org/pub/devedition/releases/114.0b9/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "31164c6fb2419377f15e1bce5586b062a87281a617c2707fb2c57493948a487b34f6d365cdec62d5b4e54860cbb7b67426bdba4e99c1c7826dfc3cd0ef395cb6" },
                { "af", "42ce55b371d9e630920c1fc0b6586f96ac9872231b5c446c57094b0037a1cc26d4d140d4c1f6c3dcd6d8da112e55763579e162c43d8cc0f836090f2c8c051031" },
                { "an", "a1dd5696309032e0359347ddd981dad93d528ae084fbc5c6925fa01de210bb8187655c751a3049e93224f4eff6f5fc2aded64c183c93b0d13e3511333cb932b8" },
                { "ar", "3562e4b59d9f893bab74124c7224ad78e4255e1b763ff0886260a3fa1bf2a5b42434edfc1238fbc3f2fc92955a78c53e3fa68d96d1093ecc0a4fde5c2c60b1fa" },
                { "ast", "1e1a8e29f8bf3a21a61acddd160c1747c3641ff8d8160b09092c57c8bf2f0c9e829109fbb475147ee5bf22da7e9d353798115fcddffab9acf270da86279da53c" },
                { "az", "abbaf2f21722f5d56064b10d351d299782e264d7881e8cf6ea3f6a6442a40b9e8a786751697d42a72048ffae8fc47ff89c1b8ed6e13a4340660044f46b00fe26" },
                { "be", "56ca04c13c1ccb905a4481d6b6a324ab1c4d80b08117e123e29cf1e7dc908a473546c6bd35f62f4aaa1eb24a5ee62022c6aad432da830ca70fd31a3189a988fe" },
                { "bg", "3559022fbda5a3355e0eb0dde7337c15237c8c2fba912dfeb82f00e58286c5e0aa47251db0979952d7bb5cdedb7624c0ee2e6efe2e9cf9844eecd062e933da50" },
                { "bn", "513c7199d84b4da03b71349668fa3ade50bac373c05fe5e1ae2da8ef5497ed9e7a270f480792cec3701e62b9f15ac62ef5fd3a0bd5fe70a14b745610a73ebfa0" },
                { "br", "9e4a9f7a1c73a10667406438bc6273e4d934bc62b67f6970c84c73fc2ebd1d7cefd13e59e1ba0323b0fde2fecd88d611a68a79a26b74166aee7d103d731285fe" },
                { "bs", "1cbf02c13f401c397ee6fa198715c0f5a3d64f058240650b09e1087f19d78821ca45d95a7c8119cffa2ec433979862ae4f0f79db451e6ffc32d3fcea99e82b23" },
                { "ca", "0addad08829fde55f465b0784ef6de206aa286dc0cebb09eb32ddba2cebbde8bfb363b3f9fe9d1508fbcbb0743ee75f29d29b5e5764f96d3c9b6b0c9dc4a0976" },
                { "cak", "5b8d1ef78c28cf86d2f2459b693d42e27b0d034d6168c47ebaf95bee9039d932a823b663b1a5143428517eae9253ff15154ff8d6e4484f13582e4ee64bf01926" },
                { "cs", "e9fc4fdfa07e75d8da706fa882ea6110887aa4fbe7be1dbe955478f4b40913ed23a1be027c6e11fd2a814d1702817037c9a12f45ed30b3c08599ca37e55b01c2" },
                { "cy", "a37cb852b8c64b33d4e36d10e9ee72fda2ca455f24252e90ae61bd3a7a3b713bea55fa3e01399fb734e26ea2e4b610d4867f5ef16563e87dd27cd67dfc88032e" },
                { "da", "9ded98163b82e1684e9a78136b90ae6f23ffed7cb04b2c33ae744a25969362aed69349e9ea74766d82d4b32ef38d699a3f72f2cb60d1674906e3edf683667b73" },
                { "de", "afc8702a93f2656b0c7d87928a2b87dd710fd8f06c8e4691548c2e59b667e681dc89f077d33453e56e87405283a4976dd478f4f1c5a7ea72bbddbc6db6f7480e" },
                { "dsb", "2024e9de5ab5cb533f95b73fff525bf636e705a87b438f16348b138b58aa998fab11ad57aba690464fdb00acd2541b9976198dfc135f6b5ffa3f2497b5f2b050" },
                { "el", "0ad144a9ef5b8b7ddff695596d4da42dfa6f96964e94b586f8a9b4bec2ff3f1aa79e50a3fb582cc3b4b96b1b53e1bbb614bec1ce9dfee2a6c993531562d2452d" },
                { "en-CA", "e1b30b43b2f3333fb97e0ddeae62846ecad26bf69b99da80c5cd695cbf938ddb557221b52a0528218fb16efa4f21dcd29acb294a2c079fc28b362047789e45bd" },
                { "en-GB", "620591bd354ceb71bf7501db4cffbb63fb6000e88ffa8cd0dc85aad669b6dc96383390c778d4ee33590e52c494e98ae8c4605bce5ff5a6ff29f22b75c185d098" },
                { "en-US", "135389f6d4f4e060fc004a3e66b2502d171775066e1459131ef8165405eb9f5270470ce90d78a04b705c474cd211fde51f561fea9cb762d4ec4ac9c7fff951aa" },
                { "eo", "08025633517a20a7161dc0d4fa8434a44a95668fce65db7ee4f2fa0f382bbc22c299f5ef1952ba6845d60298b461a9cb46136178b1a947fa97c3a96d27584f15" },
                { "es-AR", "47b272373247f83043cb9b586a0004bc06a273359e5bf3a4d8464a8515d373695f41bc27eb9238affd6d4ff00c6912401b16dde2ef6a9bc63a428e69ecaf9883" },
                { "es-CL", "90a8e52b52e51bd99eeaead2c7f255b410cf16a45bc265301bc5e857ddcb4c5b2946ecdcada983dde95ef65cbd77d81af7f47d9308ed3f44aa06aeed89e7d5ed" },
                { "es-ES", "d1c5dea5ae2028266767cca7db5092df2b94044538c512882d38689c5a7d425615ade5ff800891db97223f27e42ca43fc4038fb05e223753e354b65bf52a1276" },
                { "es-MX", "d71bfa5de47fcf0c1a705babd806ddaae70ec934e69b28732b34bb758fe8f10e2782822b972a8664136b5fb75c88b1a70b42877a46fa2d31ca4925f9437f448e" },
                { "et", "f70d59818838454ce0d0f1c84a29cad394df19daced26a0aefeb3a2e3489c024ce37fd4618587049905c0e57e6a5efe1b73f377d5a15b34cc595bc1d84ec1444" },
                { "eu", "61fbc47dbb033998a2168b58241a05866619c5f808833df9cdd150f1b37ce502d880aa957d5588b4c1be9f02fa018475f4c70741673dbc4003fa20df76fab18f" },
                { "fa", "406bc92aac07d36747540b7e8d940087186bae4bfcc8bb7ca5d5f6cfe385deebf01eb57ad70268ccd5c9d3619e4164a60c51c845ed8131e46a1398ca1738123e" },
                { "ff", "b5b8d010ff5a87ac3cee6803ecb70d32a116b75c8033b8a2fbae80e4deccd09e700a26e40fa748c6d599c9b997ced4133ab74c07a37328caf3da677311ea2533" },
                { "fi", "6bbabd1c3be6dc4fadb6528dac6a94e3796fc426492fbb48ef1e9abca626190faa9d771bf55eb3c24c3880c120673d05072a9aa841a9aa4ebeb416c4461a35b2" },
                { "fr", "bf53b220848a9e3480a36a027ad5cf2ae3340bece67bbba97c57f65decd0012adf89014026ffa9d41777ba596b35e821e810e00e8953dafa1fcb7f6997416b6a" },
                { "fur", "0caf1e7bcb90d9547de1ae21b8e80af8e3d8622151b398fa243594a4c40f161c99ee4f18b74aac36cf328b309334b1d31c86f853d916fbba8b9df121b5964326" },
                { "fy-NL", "187ad24c8be53a91193eb22094b3be839faa9fab430a0cc268238f42069c4d7f35313e347356821861aa88c7bfa81a69f587f73d9fc52371d60ccd769ee2e7b9" },
                { "ga-IE", "5214618e548978385695ee2188a3713061e9157afa98655054402b33663c61a5f4a9505e63ae05d4b37119f247a3db9df27667574ab04804644ddfb1973fe64b" },
                { "gd", "f39ece557e91f5117d838eda1b0b14cd9e634348eef4848f9791b9bb11a57bcea9e592d5c5d8bd8c406c198d513c36fe45c8aa189749be0462967c398329d572" },
                { "gl", "5c67c0864db35db2157bedf165abed88e92dacb6c2ecb0a78eec57ed1cc1db2ed58d5b54ee98f388cd7238b8a9e7623a2288cbd72299aeef459b25b655824bb3" },
                { "gn", "54bcee2a14629914f0f7aabb6eb0b3e8b29375f2f70e3c0dd98c356224368f2cd74dfe45241cd21687c5fca44cc903764aee0c7f21d4ddbf858709873866f049" },
                { "gu-IN", "d570b2d515d57368635730b89b3a41281d3f1ea2e4839fef83e36d367365428a7d77e75097a73192703fc15d79340a084fc4618f1f56e08ae0bbb16a35239888" },
                { "he", "ebea38cd31f7a09c94ecc14d5c8883f1d60fc2c5794c0f2deb3869dd01fcc765e73219c4f5e61e740c0c5b747cc35cb86919a7f54ea12006fa6e9021d5c3c0fe" },
                { "hi-IN", "cbe1de06cdeb85ce6b83b7e795453a2f91b1bf7a8742409121cb55d1de72d4245f2b29cbe1ac547646462e3cfe5cd703be190ad6009f985d311f57e3d2082048" },
                { "hr", "1bf972a1136cd5655e4ad3b2b42667c6ee0bab90237d88be3959a82ff050d320babba832450e5fbe23c6e9638b7ac71bf5e9ffe246e2590cbf601b71ff33697c" },
                { "hsb", "7bb762e5963e05eff3fb75394183aff0ae414fa5854226b226f56a30ff81773b30a452e952dfebd614c5288c27f16edee21203f5ef11917cec77a4215ab2c34c" },
                { "hu", "a2ca7cfab27441224d31e9fb0e65f2de2acae903f2acc06cb34115d056062a34d9e03602ac8896381b0bbb23146582be0c44cca38dbde0389735c3f0225f4eb4" },
                { "hy-AM", "74f68a45030420d5d738be5e07deb9ebf14c0debd742c575c9178219c62e1e4863dc561e4ce28f530a1bbdb3c09dedc94701bd285ab72fe9961b44d46cbd1ff1" },
                { "ia", "ac7bc875183650ec62688764b2fdb7ee0b177bc26060642db794c75521191d45272ec4a79ca7ff8c983825bb903cbf5272ac7f8dd3a6a1279465153065c28b6a" },
                { "id", "47b32f180a3ae1decd5b33c5bb0451326e4695477cc28a375e7aac5b788a64a0129b8d51bd793f04d44fb45031cc715d6dc097d513ba19bfa0cc0326fc735bc9" },
                { "is", "1a6c948d8de884930510ce32d9c492df16352488f97ba3dc61e5a8c6cdc70ced98df539207523ad78dfa07563e9f83e00d3efc21b2b93be34114cd71e4d1d107" },
                { "it", "eb9e4256341cfdf0130fefab3cedcda77a61baebf4fc452e15ab05f11abc9a6ecfb36f4a9c7daaf6a6c20a26fa133aa2c9cbe8ee28529b6bc342ac74b702cac1" },
                { "ja", "0963a1dc817d235c612fc47bde4e84cb4bd145db0fcbd310479fb3dfce3207c0e4c00c74956ddfc20944191ece3788a2e54bb81623769ed66c42fa88f5510fe4" },
                { "ka", "f1e116f0dd8908fcd1d2c1afc61c71e3edd867e3b56daf92bea4c1e85df111adcf5cf8c6d1a2b5166c62c8070b8953209ec8cb47e57e0899998d5cd1d8e25ee4" },
                { "kab", "880ad09cd9c65c522da25b9d2f6eb83d91752630169f462d12a91d298e11ac935da01d9ace232212dc1e6e5bb39851a56b7797da1c42dc07c482d010f0a9c376" },
                { "kk", "c24b528c204259e00d821d1cbbfc2a8bcc22938e49495e5762a5d55b382be8db64c91a1d934bf1001f314b5c3d865c382faf28adc4efad422e9f091567814460" },
                { "km", "45c740a7f29cdf730fdde17eca398af7c5c7b2fceb2ba96911ecfc19340b676e4d29f9230165a2b9f91fe4498a9a96d3dc5e2564604e5590678add9f1c9cd434" },
                { "kn", "5e21093a18caa666061d0ee68085412a0fa64b30d8645fbec243997487e9171e1b78bd486e6e457700a4f5e36d7f36891a60e728686ce3c351d771756e6adb25" },
                { "ko", "b70bc3710a74e8157e931a66b1a3bd68b336c06e3e6b1429eedd2bd943a0317920382a3688bd5cc10f0ca9369b2c298561411c98bee84785c8f0fe12f1d3f186" },
                { "lij", "498405668fa79be7cee9af485e059d0b5aeb181e7f541ff6a862912849c051da84e22f908248bf183026c17792ab556572f32ab8418f23e26b23885877f8534b" },
                { "lt", "38969672d5042fa450ec62c31df08e230904bbc47813875058c22d7beeb53a9416c5b65194a75214811271041a97a62e6273467bd569c3d1148dc30d1c69dc42" },
                { "lv", "c9c89557f3541791715c3c4ff73e71ba7b0d2139be7dfe3c43a0240b5205d7a96bc926d51c41239d457e6bcbd481777a1f49abc42bc3b9c7896bb913fd00c36c" },
                { "mk", "6b5135165b13af35cfc58d9dc0e7b688c5609543a59f948f48624d5d4072a5725acdc567a7806bf74aeb34ae6bba667bc7616f77328732abda5d933002824502" },
                { "mr", "4bb7ef150b80e98d5ef86d98ce04f9ec350c2eac20be72f804c0900dc61d2e783df959e0d389b9ec1254507e5d4334bd98a4e195498e3000143d2acadd0879eb" },
                { "ms", "78aa9764200a53b3722cfd6cba88e9262b4c1af3cea7fbb5ac9c7c8170c4768b19cf5e5088a0f697ef516f9359de4474e64e0ac5278a04e980c6be6bc7791dfc" },
                { "my", "f80dd7be0695f31a7a14ef27915014b83bc207c8255e571070556fca5093314476fc1683e8a34a65a1788edf6f9e6286359600a44ffd1dce92e7f969987dadc7" },
                { "nb-NO", "fad9a0aaa86e9b6e84d570ac89bbd29255dc1b5748950a9edc912f8ba4373c9e640c9bf214bf68537ea99053fd84db9be341822787ad2238ce9a3bcb5ad07b96" },
                { "ne-NP", "42f618ab976fce5fe90e10acd13858ff69da9ca115fdd780c343441a84e2c664f4530b9b38c68977fcae5c612f22311d276fcdb7303f3070d3271517edb566b5" },
                { "nl", "dea20eadeefc2ad6e046de670d641f6186302e23993bfd6999309786964bf9495f93e481baeada789e8f55d8a376309b940c222c8befd1a660d4259f741606a7" },
                { "nn-NO", "45ea199787b7f67070332d134c97f9820927e65071f34b722b38d11235ea908296893330eb1e1ff9a430171ba3d4636a97142d789530d258f3176da06a77de99" },
                { "oc", "89a4caf5246d08011dadc6b1845a678fffe9bc0f443347a0180ca086a8e4d3379c9bbd96a85af6737c277c8cad9a8d2571baf9606e4e93ecc985ff1b4267b1ba" },
                { "pa-IN", "d8bfe54fbb66adff446ce777755703944f467865f449bbf0b30d62912fec06c553ad74c589d31d9134236a7b6188d27852f481051e097dc175956ab8aae8a851" },
                { "pl", "f23028bf5633f7669ac94052c2b762837682768db6af19e6ddca5f9e94ace3ddb9e9bdab2174944aecea0f54556b5071af80ad2240963d986b08d86ba26cbfac" },
                { "pt-BR", "67efd52f53402f3051ee1ed197bbab488417abb008c55b031312f525d411220d28a4dc3e2a485c47383dc53dc7c64f3a20d941e614c60b3f55b6ce8753316930" },
                { "pt-PT", "27ab81190be8fee5a668b74a5c24000c6130ad3c10e881a301e671ed48029d4c15ae1e523442395f2daf293e9680cfd2f424bcd384b365e443422d6dac76a527" },
                { "rm", "100aa04dd31bee95d4f78ec3ddd33d0e82be2d0ae3eb4cebd551b6599fda104e55178901378f6871813732da82775e1fce8472ad5e869692ba0e01b4436cd377" },
                { "ro", "7d484dd3a9d62003a42741a909944990f692f1e107b3fcb32cb656e54a0a5f130e6c8a933a8e12e298b42d895dbe7476e0f2bd165ee9cd761ab251341a73ec41" },
                { "ru", "1892b13c62e467d82008d7e69afbd26390a7fc0f585bb00def55dd0aa7e4734881e079e968f31e69b6b9e5cb13805f944ae6d4cb3f0934cb52a14ddca6605005" },
                { "sc", "6792300b40caa8d4e4594c1023715b9786661960faa899d05412e37281106cf201798efd0a632d57d7cf242b9e425948b945551a09ee0710edbd2ed991fb345e" },
                { "sco", "395c8ba70769c3bbb39e90dba7faf07864ee35d357c9862ce198f8d0be552442518023221b8561c7cbda6f298f88fa5bad42530a4ebc2e8116aa939389ff5955" },
                { "si", "27cbce16aa69a7aff991db7ab6d0a31ce852653dad707ba408c884fe6a4028b349c93ffe2cdef66c46661429dffd37645cbb2c355bd416f5aeb9d062c5d8bcbb" },
                { "sk", "d0ec5126e7120e8e0436000ee8aefee6d0546afdf4a14b846c37504fb785d61e633166db6d91b833ba93e46401ff04461699dddd638696fc4f4f781af76fb1f4" },
                { "sl", "ce1939d1116884030b2e0fbf1894cad45adcf70956a678d8b6aa8a33f710abcd168a26e02b47186bc148a7d7f0fb2316936de3da3b0e099b12e47461f7ae1e16" },
                { "son", "f55eb913fb6cfc8463db99ddf55e6c123dc984f676a44749bc8e87bbbd7002262519387db852c835584bfa6e8fa4626d7bf956c432f013e6eff9e9e9d4177a7c" },
                { "sq", "80f32fb228a9ddd11d416e16af1750c9db90aab110b1beb24fa3370c4076143dc4edbeb7c1da6ec3b96e7b71e9ab3293840acf1ffa9c70b445d3165e730ab12b" },
                { "sr", "63802b5cb5189f4bd665e47b8fc28208add9558d8c3d51aa6d8719915c9bd054ecfe129d7c592eaaff5defd544a91da38cf7d25349b9b65000f1688fa3b3f5be" },
                { "sv-SE", "fe772fde7fe6a2573822f9a839246af5a28f9c420b4d84624adaa3eaf3618fce77b2552870d5db53f28012de0935f811b3018d356e9f38fba7f50a3a8ff7524c" },
                { "szl", "32e22e5509a7d88f4c06928f0c205870dd8d19fb07423b0dc6bde2c90af41c959265def4c18489b9b9c1fda786729a90fe60609e9fe783fd346369a2051b0037" },
                { "ta", "494812f7019eb8be102efeb063e6c265cef215e95206b0428e1deaaf10bf705f2b58da414f8eb560a03ac851044440702abebdde439ffb5f708bcf3612e181a4" },
                { "te", "28ae24341a011ee731f719cea9c867e4da477ae757c7ae1a397ecd042fa70d94130fd7c9979d8cbe5ccec9e7c5d944981d71bff5cfc4d1f550b4abdaebb8c7d2" },
                { "tg", "7e70bfedc9ecbe8e04ae31594893317c321e3965fd3dea1e8d9480d7e59e81bab5b29d1b1d6a62fc855f077b6c6d25d5e1f289752dc60b0ae037d9186b8c8f85" },
                { "th", "d44a1056bd8e3fbc5b15cdb1556aff99d38639ff64837f2ec8a12b10a26539c2c391090139607fff44eee8f91cd58401f5fa095bd1223e7bba1c81fd6ac12004" },
                { "tl", "39dcd0e84029c2d0fb6f301503c42400af246b66b6884194f05a2376f336a66a3b855e3b74a41d6a6bbc02c0cb9645cda395e34a94d0f0d82305c74eef6bf448" },
                { "tr", "bde91f88188c5ab753f14378fdc41a55bb26c8a79f8930fc7227ab7026c467908162e73e095e3218b03113208884a0b1a7d02ac01abace02ed161121dfbb8a49" },
                { "trs", "4478df4653dd62c26e9440363c40a72b71650e94ea0c903046cff50b3ab1c73e8a46c30d742b10a16c5b811b767b07e273b85386ae13110ee9c85833c2fb4e71" },
                { "uk", "49101de69767b21ff80147d2603d8aa06a661035035531c4146aefeb85b0a1f5bb66f28e86d8ba16db77a4d3dad6bd125d7897fde01b801d2d484e9e935817c7" },
                { "ur", "0069be0d43f5f908643a9bb0a3cb32758257d2071dcd2cc0d6e4c60e676b449a017c654fca05199137f8b841ce1095ee1d621c6aca6bd36b179f55b0017409a7" },
                { "uz", "69229bfbe4726925f799d557aa2c715e3c65fe569638369f8079011c09457f9729259992bdf62394fcecea3e550eb4bb22cae82b9ffd34428de6904825415e1d" },
                { "vi", "659b59ab99e3d8142cc2f77db4339a9b83ad12bc08bed97dcf166a1b1e301d5816db0032eec79c52ba3c7dd01a9123d2aba5c06405628052d665471ad7946160" },
                { "xh", "039ab5f63b154433465fe8e66c2985dcba904e7e5c00ea9a1925c046963aa1a11e98504f49389e2c41f33b74ef4e551ee38dbce08ec440a009e738cdbd9a111e" },
                { "zh-CN", "9f19b03814ba9f2a412048db7c6b82bfe914d3d2213f84daa299db6b8ba66063d02f874d4d6b663c8c1d25489c1462af71279d415efc726b16d3a9eac680a73c" },
                { "zh-TW", "2770e7fb0b8beafb537d3d8745dac98831a5bb342f1aa70a09e24ed3ce36b57ee9088958f9f7df6593466514fb0810eb277ed828a281e1276a6f04fbe80cb820" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/114.0b9/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "a337035ed5fd42ba8e77ea370d479a2244400060af63daacd56418762ce7af412d551e598456016441593930fe62be36db830a0acd5511bd1c41fb6e8b7552ba" },
                { "af", "49ebbadf06463e19de1f4104e8e1c4117612aa9518840e374b8e5cbbd7dc2a9871a27b3afa838ba237923758457c1b279569fb90edf9722b9bbd8fd52402bb72" },
                { "an", "fc2994615c4f57d2737a411bedef59fa55e13c7e974e591b05937151691e2d8db3b0e9dde0165883e9f4d5df85074bacd236078cd0f3b8e026fe82e3fedc42b4" },
                { "ar", "31186b2b894fdf982c02d70c7df023ab6a7966454afed2809d3e69dc309ecff8b89e0a59442c61156efcb213823fe69919afa07cb477e33fded7ec9c5bd89b5c" },
                { "ast", "89002bf24bde186c9e0f515a5de3439a9babed03f91326a24a4ebb6456d3a81436e6579b50bc42f981f8bd0f161c206e6706e81929ccaec2087212a0f8984716" },
                { "az", "8a0fcb1cec8294a8e20d95bc20e09d0e0e09589e30b7e9795ed452791c29bbf94b2f4a1c7cacfd3e047376ff01d0b382f533f8bb2eff07ab229ffa4dca023bc9" },
                { "be", "89a179799001f97da0f58b27c4d0e0ad5fcfea2b51b0a2a78223e6a6932045a56d208c2e6faace02ec13560a0bd7dc7dcb17c5314fdddf8e29728550c815e409" },
                { "bg", "d4deef58695ab3e5371656be242b70432107aed664c35a8862b1c769726caa5ec676e8c8c1109c1364a3bdab6dc2563fbeb0c2191e2a8d37c3a07e582db85b53" },
                { "bn", "30b7bebf47e348ae8531b10e23df7bdd683296146a306636d92ea1678a2187c74f5b918f0807038c47b19cb01fab79d1c356aff687db605096b1b349250024cf" },
                { "br", "5a4a3b62a60f87a61e7b3de96a497a69d76e9ae8c3d4bdb269f925be53da110f5987d1606971b0d94190e964b1ee5cbd86f6a44959c1a0389c9d6c430b59eb1a" },
                { "bs", "6f26c5fe75fa9a5c4e46833c42d7af4ddbad48a5b081db7926adb4f745bdbf83e3ff5077898241139d964ebef807eaf85beeb7b7817404f9d1429d10ae614aef" },
                { "ca", "ebe83b3873671ae5df25cbb4ca414ee89182920e356d1dbb6af63349590d5d71b51e0f2b4f1701afa470756fa575bd0f4488390e32c7df0fb51c33f527237685" },
                { "cak", "77badbf0a829c859edf9e8f0a978224a7da73684c27a9b4225ba0f43d961ab2300868d8d1d0c5997a27ad077c95632678247c31068c38186b52f8db4348ac4da" },
                { "cs", "4d8a1d544981d96eedabffdbd883d3ef7d08a691c3881db4506b7061d40b2aaf1a9e9e219c320e9aca94271db3b7d3a91f4030a5fb11684e5439d4b9704ccaec" },
                { "cy", "037b4a20c76194663de677eed6824199e7b0dc39db3b65918e52a84979d0e4971f3371da3bc5a85ce00c73aea6c26d42a9fea2c939ec84ebd2d68fb50770f1c6" },
                { "da", "a0cccdf6b327aa2b3d5367556103b724663af6cc7c1e8a62284cf19c96028d8abd08826cf39e4e14de7d7cca3187288696a7cc4362f0aa31257a2e7b01d6c8e9" },
                { "de", "b0a40f036a6294a5504b37abc7f89644a64a59ec1e227a5dc10a13a1d7eb4b97e0de4931af5de9adf274a4f2ca591270d29702b7b0bac425ee9dfcba1c5519af" },
                { "dsb", "cf545cfc320492e71427784deae00b4a7f6affbbdd4d4c3f9ffbfe6982bd4169a960842757685a2bdb14f68c1604cff22088b12842e80c57d6deec32a1e3fbb1" },
                { "el", "a35aa93f0a85be273a80eb6a21252d53dfa14a2ec2f78c8d8a17054d0103433cbd4f4a433c9a737c828cb0ef134506baf560a0df3b68c02ab0765ec3ee37a7b3" },
                { "en-CA", "d245366d861f8c1cf7c5ee6646c79806a6499628e4436e76236768c8e7198a8a252dfb467aeaebfb9986c74c2838def8239ca5e264ba09218dd4abec07f99627" },
                { "en-GB", "ccc6c0ce4cb85cefdcfcfca4a898cbbd5f17da9f96f4d7b3d6860f65cce5c6f9909ed4fc0e40c84ea923a6706a373374e8914ec82b80f9b2142a0fdffeb1dd0c" },
                { "en-US", "c1d1de6629fc2d12178b5471e7d4c23fa526986c7a8569b347d59b1e6a3a36e2015d5a73bb4efff68b6db039083112568294e1f4730b5b5bed6872efb7c7e2d3" },
                { "eo", "f8142348bbba734ac9839c4b726966f098acb135fbe4d6684263329cf3d6b82e844e9d8f026a520bac3c506e76c4d5801cb8d3afeec3185e859b0cb697d04304" },
                { "es-AR", "a53c42642b3d1ce2d08d8f473ee54223a030ffc39bc97bf455cbf2de2e9c67772a56dde167248f9cdeafd6bb82043252ae23109d753731e576700547a54bdec8" },
                { "es-CL", "672657bc9fa3805a4fc4286d7c93350788b2ef6e4f399d04371eaa07ab81ad34be9c1667858b3dfef7767776df965a7072db7a58e47a4b28eaec9985422b6b2b" },
                { "es-ES", "a60edfa27bfeb5fedeee700f1f355cbef47810c26cb08bfe1f0aa5602063522eb61b1157543ce8c849f7057658c0248502fffe5d2fca0208de67640e4b8b317e" },
                { "es-MX", "32dad7d1bd9b061376fa8de3d34769466f03c40cff95c1530e59be3bb91a5395c0d3de23f3a4d4bf22a398b167821477bd94c0aa6c50028fd992e9734b00b2af" },
                { "et", "1beb2b9e3df27fad0369797ef2bf60ee61e01fa9c0270205599e1a7cb42af7012a34decda02aca320099b226bb4afaca2cc5f735b5573bcbcdc86b37371c90bf" },
                { "eu", "fa9116a0751d360ca38f41796aac27f9cc8d54dc3660955f938728c20f6551b67be8b38ca4c6636a40cff9058639806c2d09c7a5cdc4c7486039ca053ec80878" },
                { "fa", "2848074751278b6a8c82293b9ee4a0a460038df951bbba16e5a31f364def6c0ffa64389a9b020b9e5454699f5a4be2c24135b4a2267216e699dfc4d1935f2853" },
                { "ff", "b4f4cc25bf4fd6e216b801d1607866ebd342fdc30707101f4128755172ad762dbb6f38ea9060fd244e9b9a8e6eee1323e20f29c596f8f1016fa51e69469c93a8" },
                { "fi", "227b35f4137876250a3f316c22c3fea3be691670b6f6e807ce19ecacf44e177aca71008c6de744e57e1069aafce19d016faf58511895a1ee5abcced231caaa5c" },
                { "fr", "a4d26b16a30303ad20c6076a7ca49bf0d2f0e222ad5dcca2581d8fc67267d4abec86cb7ea602c46cc584c218bff1d03b8499442a50659d9b607140f8ef06bdcc" },
                { "fur", "033c2419d8c740ba400de65c4e2f55c467fd80df2672a864467732952e110a4cde98864d5aad3e036b8af6c47a0a2c6b097f9275ad330a7afb13d58d55f9bbab" },
                { "fy-NL", "d71588597c6502469b41c8ea7d7cf0f954c4a2a8890df0a3e210d1ffb59ef2b8bea84887fada24e017b326f08868f86a600f88886bf7955defb250eb79c7349b" },
                { "ga-IE", "5839d087ed5bb16699c475a920463f5efd1f4adfb9aa914f896806e7e8491847d5acc3d784bcf90a26d6635408516a079559a6ecffbcf116377adf3946cc4e36" },
                { "gd", "38275068885f3db760fd147d435dc0ec9a42b41296b38e0427a211bacd0314b0d24adfb7997c65427dbe9412d8fd98ca46fd2d2505b1013d2996bc7b76686809" },
                { "gl", "95c55bffc78005927dddfb97a4fab9a1ada2c502b47a107ebcecaa8ab854cd43df2db0a6661f759a11261633edf2d4997f9abb7927c84f1353c47444cfe90c28" },
                { "gn", "29e60f67429e0e534ae0bf6dfe575f643229c51c33c343ab3fb8904fdfc3deb8e7855aabdb866f2f4b303d3241e73f08343ce60a4fac7b5712e002e30aee9b36" },
                { "gu-IN", "c06a88ff837ceeeba5733388de365aafa6e5008b313b6229a16cfabf6b99d5e5f7534dd1ec2912a417e791a8bf124a2a783ae90957fe053cf460fbc6dec06931" },
                { "he", "381b505d7517d20536bd89f081c9994dab3817e9973a81d4ec3ebb9be2dd3071a949f6b3f6383fea64d50dd68534fd31fc3fdc8ee9cb700bbdc6fbdb2c3aff98" },
                { "hi-IN", "47fe062eecb1554031c79cd5c9956439ecc88c68e999dd494764228f0d94d00ee364cd901117e5352548ec601afcafa772471d4a203f79ba458c60b0c7512e71" },
                { "hr", "dbff7850e7b137fa92792f62a72891a9153156ad1c87d91bb5d156316dee15aa3954810805b17c12f3205fee30072a45a28423f944098a70999a11180f7812de" },
                { "hsb", "5d706cda4b25538ee4c662eb1dae70967c5ab75e1bae86bffd97125c66e843bb4954d3b76182543023aac1c38d5c4925373dd7b2f95f6d3b2665ea7f220a6d7c" },
                { "hu", "48acb0d79b7eba79c3cd61ffedfbaffe385cf1e95251af4cfb799aabdf1fa87c338615dff484f40025159fea21249b6f1146a54424af2368f7f5a3ebf1eb9a32" },
                { "hy-AM", "8dc44e1fa8da9c1908942018157e03155d549ff9a8298476dcc41a3c45ae0a445668e507c73b8b821858537c4d708fc101b3e8a297f57ddcbdee460e052861d0" },
                { "ia", "1435e6aa6d14cbc6531a3cb7f79eceeeae64cfb5ea45ed7941aa2d85c5ff845d79f54d996cea5bff0234563ba4e3e50f3504116f1e1c80290a962a0c666e7fd5" },
                { "id", "62674af3f1d750890e9a7f7aa958ac23772880842d899133bd6eb353de5b6f1a4e3129c33a7be7f51f6ffd4377a3306c4cf13690e65cdcc09e6fa4a69ccd7852" },
                { "is", "46f391a4e1f55ee3d3f8b477d37c009cc179342fc4e6fe40240251b7a4ccaae663ea44187fbb6f70a3dcb4ade2f5fc66d740286b6a4742bedc7171a294edfc6f" },
                { "it", "b1ada185a9251e50cabd459b5dad6097de9bf494a91e2d24d645493b9d4ba0ad629e32e8ca2d7a2cfa27eb8f8e1a66c1e333e26a6c2b8e0f7b9720b42e2a2a84" },
                { "ja", "6795a3bfe9689b8486d11c39dcf11d051a4c1b1fda1613fdb430333bfbf760dfb146c65ea112ad5fa0204769d820cb9df94a2b48a622857c0998a6cdadb93eec" },
                { "ka", "96a176672fc0ae51658ff493fdf03a9f2c6a3d54d054facaa46d20a353b2d7a62ca8c5e06c2fe12ef0c7014bbf65c715c2988e9fcede30d1d2591a3b9f1dfd70" },
                { "kab", "5e595f5df3ed3387d41ee41a137bd6d62e30dcc53b25659f63db87eac0cbe94e22dc7f79d19f6d5e7194f9c6fae5a4cbea34b58500dde36a57e6d5a45b0fc8db" },
                { "kk", "c573ac0e624fefd5830ec0f26fe468f3aad65dfddb264f466a2f27545fb52589d2a29f1af593675939137d5e392735610a92cf9ca9f48b57e212446d729298e5" },
                { "km", "a0ca7a3e04689a47f26090530f72bcb4153629cc3e1c62b470c53e6db8f46ca944e389f1f5c6c1c694082b50d513e0585726a639aa7f06462bf6b0250fa97437" },
                { "kn", "4da16534f35aba7e8acec94edd338c05dbb09770a7138bd8daa6b99b10f37e34f82a23a019df9e8184de8b78ae2ac08dad4ad6c3cb21c12b77056333ac55cebb" },
                { "ko", "8b5279da40302ae01c97e4ce12fc5f86855308f445354dafd08061df8bc04ed5bea3d8da9ad1edb4ef35394aca0a3dccc80589fb9dcdabd6c6285fd67adb2152" },
                { "lij", "05db11c0ce4275c6b35f208d4ff2889d3ba0f8b2ceb8f945f5b229903088feea7cb22015cf652f712f862db7b32a023dbe796bf1a0b53ccea373e77c39f15321" },
                { "lt", "986f51ab12627195a19a491884cbaa500ba295a451ef22c1919704dd18e5ef657caa7f86c736ffd2c452656917e2eafc35ebd732ee8c558c87e5ad3557e1993e" },
                { "lv", "417aadbed3df6ed99f391d3c93054e6c8175136ec69ac00d384dd34b3cdd557147f1014ba4aaf7486eb6b6e6c9f0c5cbb168019e029fd28f9466ca8a62fb4383" },
                { "mk", "9e41de3d8b3f8c12500e612d9e3abd498c7bee0583c29d66994177ff81bc10bf4e4c76c9a46b16e160c48bd485740f5f819e6670184ea558456c6a74ce6eebaa" },
                { "mr", "10a1598bd7e7bad895c2902371ef2bcff820b783f5afdf148346f77d85aed8d013ce4491c3d3ee1a9c6b1d79d58f3d2569dfd5bd5c19f7a489b0bacccf4cd813" },
                { "ms", "81a45ebbc6deebd229c661dc54fd0ad565737984d63d763f3c3c3748a1289b35f06f05a40b7fa30cab2a721225792169ed90c474d536cc796d3c7b152c90f266" },
                { "my", "218aadb0f47d62a50938a320b94f3f7f0d68476d1be88a5fc504f15b44cdecef89b0706340c033842ae1a7e7aa0c196a3aeb59585ff523b4f581e9305154a3a0" },
                { "nb-NO", "5a628890c04144f3031d4ee2e7517aecdd20065246a2ba18c6877b92e71b5a27e71872cf7377b58c4e246bdd3fba3071cee88ad4ba26debc9446ee6e05eb9477" },
                { "ne-NP", "8ef0d31f6e03371265a6ef118d040b3ba719ab6a5f4b9c9f6a96470f68cd72827878f2a366a130aaa2fb7b46cbea3de680e57d6afac413a0e148c2e2af0eeebc" },
                { "nl", "e11e4dbc81ad6b970b368b3c00f3f09efc465c8a82348eb3387d635dde937770f0cded5284f5560a85d7602388edd98c76d839a70bae65948686a0bfc093ac1f" },
                { "nn-NO", "9d2d88637a279006deb7b236d62344919f4a8c92f4751609af83d05a5d9aa83781f476c0f96eb4fc73bdc226801e8a5fa835e4c9d910d51e5c67d8233b9998bf" },
                { "oc", "14cec0da0528abeb8f466c34ef87b358bf305c6c1e0fcfba48080f9a7633849bb125d221e3de3863dff56d8981d109ae7c4fbc93dbf86c032d91f4a322576863" },
                { "pa-IN", "524fa6909fd10f35dc25e59faee51f7092e4f041d965bc176ea497b4c8b381ba2c09866f891b2511c9e6580251c795c22529b1cd38a5876e687414c0b3c44382" },
                { "pl", "391bbb7030c12b4eb6854c4e5dac217dce83f5ec03c5ce27ab4ffb3290fc3faaf5f2867bb6b5ea0630f830c41e5a016e92d5761c27881273d85f4139dbd8cc03" },
                { "pt-BR", "773b7e779826a8eaec7a807c20593ee1ceb0325319f5856f35a3ebd2526ef7405d635d4639d596fbf2a501385e5efa87302c512bd63a636e4f0074251bb052ac" },
                { "pt-PT", "4cc7254900aea6020fc92f665724ff5abf5128768cc1232108e80f39944bd3744e679009c0608a68edba6999f6926bfa924b23ee8c57e9493895b1cadda6da6f" },
                { "rm", "a15906e83bb33173791a9048b351e23ea68071a03b7be026ca93879ba2d2c4a6a19cec75f060da59ec323819ad7f43f80d224733812db722fc64a6553eb0f507" },
                { "ro", "ee7cb5f0fc6d66fec2d4ca802c96e0e99143ca964a19b1fc1eac201341786d8deeef96c1dbc07bcf9e58673466ab529096000741814ca98e4db1337bb5cbeb5b" },
                { "ru", "6589a79f49922984f6c231996383ae4ca6c2cb92482ea794e897ab805828ae04917deb0d0cf1e04d387eaf7caf429c2169fc87296ee952afd0feeaf730a2b420" },
                { "sc", "cfd0c1bba18ad18a44c5bb329baded6ed8bc53ad85a3755d2ba0610898925357a27b387642f7800f2c270480b69be920fb8b7c6b982d102142942159a4cb95d7" },
                { "sco", "292972c77bbb87464c8d544d867d77925933e7bea51e360022727bc6b8a54aae4e9ee5b483c1aadeca18649add012a7e7224068e3ceaf04668ddafa0fa21dd18" },
                { "si", "b6d693915de3a69e186f5f5103e0343056b5328160bc9e70799511c00f4da955d4f4637121942714c7d3f0f438ca8f381d4a8ed747c83676760945eca49edac8" },
                { "sk", "183b2055599a8107021d1d34982ecd24fd729be2d0d5a305c2ab3bf35955490834e1c116736582edf4b5890a87d42f2904f22b36ecdd0c48e4a72f70b848fdd3" },
                { "sl", "dd0a1a1051d2d701b2a81c177382fd2d504234021235982de69fe41ae735fcffce86073e4c17df7d8add07d7789e4816e6340943d6d286d90f7f1edd7e0c7cf8" },
                { "son", "d47ed54f621ccdcfec32a0c76cc65fc939bc99794e5935d5c8815414a9ad33122242b0ec4c26f09ac3b2bec865da696ecb67914ad6d82a1208a60045d6479f41" },
                { "sq", "bda7c8ca39e1cef71620c8535d61076bdfc5c617f90ef75e9c21970773ab708b2241690400325f588f06bc99914315674b28d427f12f75bf5fa1a7f52785fa69" },
                { "sr", "fd63df123952c457246fe31540eec0b3a1348a15e19e21cb023d34033b2fd183a06bfd37311bfed2b5f00354b56580cecdc6a2d2406ebbf0440b1ae9667743d6" },
                { "sv-SE", "5542b5e5e10e832de86199e54f3f86bb8cb3b57906e537effea319e82a5a2f5dbbf5f4c0170c3869bd4c812d07492627a0e36df682dbdd995dbc7cfa5b9add0f" },
                { "szl", "73b430df404b7c4ef3f5b598fa8ea629af07aa8b1c70643335126a8ca6b7ad1edfd27ef6c0a5efdd8b4a6cef507720f01f29f48c3d1799721f0163e248a200f5" },
                { "ta", "97add40bb4827464d4f91f94b05b5531d41f3b382b7963919f3fcfd42c70da4a2e24d852b84929fd8d8727b9737e53b3e22c8b28bd24c3d4b36f462248207f01" },
                { "te", "cce22b814ba7da6cc321bd8f7e0571cc8a89dfeea78c4382419b7c21931c1894847acbf006b281ba0ff26b470b17dd72ce6683c2f14f4dd0da23020b0da92a01" },
                { "tg", "e2c9bf7e0b66e243e9469ca48e81e0d5200cad2aa3d6528a7eb74b104c2f7cfc00a4cd8e63a24339ab72a33e4c0c14bf0bfc39e6d6f916300d840dfe47f1bf60" },
                { "th", "2bdeed79b7807432a4798d3692e2b46cfb1702a437d40ce34aa3db2ebe96d0482864050da7c68ad65b2e9c0d77b7a5d343ec9e200dd8c128bd7d83279781cc39" },
                { "tl", "b03db7a46d85586ec5baa1bf3d2a2477d853f7bd89592089595f40d3ead1524c7688a2af530073baa2e03634f0519b76a749e40c44ffe10d347b9a0bfc223a08" },
                { "tr", "9dce887140775ade8b135f7996e39c6c92fc3a720e1d9d4a7b138df3be0a22cca26a320bd31397852cf7ab988e76ef48c46e466baa5f834d88f4e9dc22132992" },
                { "trs", "66b686d106a441bef1454eaa5fe65efb23d02c6681c206bb9a0d6e79b4d9dd496b87cb12a0f5e2ac516684c0012e84fb41bf8b58db23ec1f4b9be4707456085f" },
                { "uk", "4f6bbb206b98f5503c72698d0145e90862c7260aafae2675040f309406340b264d708ed3dee1e18dfb58ed03892f49e28ff62e3dd1632d70afc44d07aee6dfd8" },
                { "ur", "34f2dd7c2f0e8646b5e7b89bf015e783f7c5c8d2861721e5c381729ba965c56f495213ff1d2da15ab0a8358b23e7391c36cbae3751aa4a77918f541a11928c92" },
                { "uz", "75ff479045fd4caec32a1623a3581fa271f0673022ba143cd11b86da2dcc4853420f9df73878f0f7700721cf1a654818dca3d53b9e80ed82fba89d1a67522c9c" },
                { "vi", "f39d82e6551f5018ec3848e93f99c8712a5f6bf582019a22295db9dcffa99feafb81cbf38e374deca119dbecdae5fa6988379eb595cc587d49ee32891a13dbcf" },
                { "xh", "fc382bbe028b5fe86706c1a209889551f545c077d22106f6ff1fd9b7b94f6a26ac6c0f688a579e65eb12381577454266fe137788614a1b24e7687e1d252cb714" },
                { "zh-CN", "f2911f9d26aa077f3bd5b9aa0928d87a3143fa94ab72989e4a8d8584a0760a44833681534c37ff013a163ca6fed6bcd118fa14bdac61875e226049bd3a3e3ad1" },
                { "zh-TW", "2d8fa6662e6444101884c342dcafe9c1cd91c1fec2aac76559a91a523feaf6019302c4b78642fb30aa8455f148b8234c4d6376c49911356f59f80dbdbb7aed0d" }
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
