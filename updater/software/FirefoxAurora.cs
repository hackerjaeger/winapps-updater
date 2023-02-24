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
        private const string currentVersion = "111.0b5";

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
            // https://ftp.mozilla.org/pub/devedition/releases/111.0b5/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "39310974e26ab56131f5052bfff5394aa39a2866059d06dd5b393dc8f5b60308ca33e6e1872637f40e5b344bada758a9199bf42307954cbff5af7e57b21ef0ab" },
                { "af", "ce9c09a1228f901f0a53e0b6576630b39f3ad3faca75bb839ac2e300d2b6c5f576488218c1a446d63242f3815a2610c3916373f7c24f43af20a9c60fefaf5089" },
                { "an", "7e9797e2114e9f43495c90e16c3eb27933c14437eb1d6f1c7bbb997c5345f201623bceaa4f3f0349fb6aa782b456383a8ce4221eda356fc38b22974ba6fec060" },
                { "ar", "711e4288f3bbf9531791aa37eaf068846493cd8d9c317e242d267a950df2a2860a75c517c66210a076e277add96794243fe67e187ee86bdf4a4f262d67a38cbe" },
                { "ast", "c1233dc5a36b942e6544544274fca9d1ca35c08366eb7c0207c8303895263eb71d377541d2b5f363feacaf2188cce25647cdf0ba8a6f2d36678ceabf624d1017" },
                { "az", "aebb38089572e565f3c7aa2984ef4d5306a17051e7d82130b1b89d4c7deb4058b6946f8a90694aa64fcb290d5f8a9d997e5f72845fe03617ff869d76715934e5" },
                { "be", "22199ef0bad8a12556f1a557fb632f0ccf817b839db22ed5085927829825fcfb3634b9ee1219562c1da7348f93b1eeeba46c8466f066a837522b626cf7c99f5b" },
                { "bg", "fa76d16a4771e078f6f18ad50337d6b87a039542f5c2bfb35f4e1efe2dc81b4fc60f89f08f4326221a21c47b3a9c55d0f31af6dd3d48415e2b899184a71b8e93" },
                { "bn", "f167c7763150618f92f2c3e0a0c33e5dad3e70b8f2292669af97bcc256849dcb504a7b2baafd4d91098bad195ac6174aa52baca0f65863029b47aaa50fbf37fe" },
                { "br", "c939735ea1b8d40560d6a93b48dcb0484fa99b9cbe61b00726a91dbe93f9ea7bcc20640f23fa2d17efd062fae31874dabe4da48a07470fc5743386342ede002c" },
                { "bs", "ad386afb02ce37d9b140097e8160225b0d7b35447dbdf476eb1aaf685b0d430462cc5896cd207ef886a2d71adb76f287eab2f7170f41634c4463778d9cd76600" },
                { "ca", "7d9fa21de51a961fdf518f3eaf7cb43971146d59b89c11d99119f15ceec6f1d03acb3d68ca395dfc57fb5d7308d1f55f62b14cbad3f36146f1d41f46b0280afd" },
                { "cak", "7e1fd2888ea565925bd3ded1ef37ae719a751f46252fb47f1d12f3a6c57b7eb3ceeae670c7f31def8adf6f3fb6875de81f729fe851f9a4890552df843f123cd1" },
                { "cs", "13509b8530d18ec601ad31440657e46b7175ae93457455939e50b11adafde2f9699e0a559d75f2f5d1a26c6d69abc0535a78c3c4d590dfcac2dc98d7ade8705a" },
                { "cy", "e75da6073823336b86c3255e23498012f1cd01ac5f273e33c212fd1ed36ebe62829093630a688d9b67d12f770ea2697eb0a21b219d9882ba8fd5e5dd92e2a807" },
                { "da", "ac3a4f31437c1092469795be0014af8b1769943dde67ebc9f04956db55de14d8922673b5d991fcc80d151153d4af9d825d7a89947d190caddda63b31f3900841" },
                { "de", "85e23c45e3c2ad92382082694eda61dad23aff1d15955921ec3e081fd86ecc4e11dc746e7a0cbd15703d2622a0751a036ee45978cddf83e109f2ac0304a5acd5" },
                { "dsb", "090265d5b005bbbaa8fca8438c2f43d571e0bc86620852b645bc7dbdfee850097cef384ac355c180c15bdefc7dd97c6f6882b696926d6494062af5fc1ee18930" },
                { "el", "f59b7c83cabdca2e2a6b9fed9a8aed0842aef4add5c98930c7c36a4b7cf181b55246a638d63d6faab34ac3e734a5521055e0c0d3b10182a40a33476cc5c6f8c5" },
                { "en-CA", "8375ec0a4e4d09f03d557e24fc60d6e80e63771917517e34862cb0b985c19b2247d4a79157af285e3b1f1472a039d441bc399e203afbeaf62980904d8193bf84" },
                { "en-GB", "36449d5c39476974a87366f313bd56d5ad69afff17278c378968e1f74ffecc61a6f814284eb492d52a7b81f143e2861f74ab1c0d02980b914f76fb89915e0ed8" },
                { "en-US", "27d023e2c9d8f407aad1dded9c7a17589fb02ed3de102edae6ff312314cf63f806046f9da944094c850588941df32081da28e849671dc283e4291d118c479771" },
                { "eo", "0e9830c45460475d4b1cefed4a5df0be845335aa40bbc65657397683672efb7453f036da6bc0acf6f757b7350773d00160bb78384ff66f382902b2178b141c4a" },
                { "es-AR", "0cf60f835eafb7550adb1106adb2d5d3948720a4abe4ba392ef0e4f02b43d0de723a941acb8f2844fb03e7abe672ea604d2b21507a826e0a6d9df23e0821ed18" },
                { "es-CL", "6ae6aaee0e441854b1c492c4a49123149860b2e8f24e3ea2cad8cb5bc1119ed70a0491b6d52d6a7b9a317c105a6e1ccb5bfa3a0d70e23f8a993559d66d0fab9b" },
                { "es-ES", "e38f9d0bfea0c6b91bffe731ebd99c7457bc717af8d6eded1cd36bc21121cf2ce6588180ebc4333ac2eab425baf5921a7fe326153d3a63ccbf626b9a4419555b" },
                { "es-MX", "3c917ac4c824610f4de987b433c2b17a08504f66a0695be7c3c3fc2015241d125250d3865479371a9a6a4e52dee36d2eba23b07c1445cf421a211d6c9dc052fe" },
                { "et", "158cff0ad4648cf126f37705c73db5f138475f3454a39ef1a1ef83fb89cc6fc131f1963d8d4dbfb2e22e10f10eac628a42acd62c2dac4fc39f0abd0ef793695f" },
                { "eu", "a4f8f811300c6247ac00c27d403f30c13913e1b2ae918d845097290dd2dd30668a42bac0737ead0be2bdd65994312d15aa84b78631320af0df686781e5fa7ba6" },
                { "fa", "7c17ccc9059cff69d16d9215a2bca40376890945e51bb3b108c004cedb8b703d43943a194def879c2336ed6b58fc1c35efe7c63c5c6af1360d0522a61bd54c8a" },
                { "ff", "5e221c0f49b47a1157a97a18b1ccff0256c8d953d23e4d3c0f5528f9197cf72ac9fad804d5e4cc333dab1b9d552859e6db43246de5338f4ab3f7a2a7f8a73f9f" },
                { "fi", "78935666ba31770aacc359b95738f5144bc065468fb99434b3b746c40a3d493fbc79a73a618024b9ed60f5086dd9d27bbf13f50f669ef6520aa801fd8166f135" },
                { "fr", "cb24e367c0b6b2d254509279f320f6b2b4124add1aa1175c4713b3be1bab28377a190126381fc00b771964aeb298ee85992846275a7cbc9f033de65900c461e1" },
                { "fur", "7c54ae5a630ea947d0e1d84f59e27c5b18f15f1f6d2d5507f77df74e8ea052202dcbfc5bfe3c3abaeb582a15100fae6d2fb7ed287a6ae9c89a654dffb95fe036" },
                { "fy-NL", "7bdba41b2640cc80ae7ab60363d18045ebc9f0915026cc140d6a6a3ed624272869ee9e31cec7db8cd7bf7d4e70e0c9eb86b31bd3465ce170aab3bf87a263ed4b" },
                { "ga-IE", "4daac7eaa90e5e631355ec5a9737fc22048cfb31dd10c7c1512f146a6ffbdc89089567cba7ed8e50b896cfeb98dc017d853b6f242d911e64c068908c198d16b7" },
                { "gd", "92409439ebbf315a3527b53acba438a10a6cd4603acaf0de17b45fee39848efc857cbbc1e383d8bb999e5506db5f2f89b48091e352fcb1b34b3070799e0799e8" },
                { "gl", "c5430733b3aafb0cc0f05c738ebd2fd14e01b5310f9a51d6793c2e9dd1d2b08abe9a9f57a00e5764f8cc8d7ac85b80d6e780bc63569d333ec0195db0b5899349" },
                { "gn", "fd772d829ab18b1c4c2f04a1b1f7c2a0cf726368608bad339e48d3d4b63674e665a3638c563e861172c3424ace045c46ecbde4343bcb3062fb9336b16f5da517" },
                { "gu-IN", "c289247e66a635480edf4eda6eb17e1a39e3ed00fbee0e77d36191d9a05d6545d68510eddc8a35cc80c11ef937eb1ce0b1dd922441af3b287e78d5d0142ed655" },
                { "he", "dd3605a95c20b4e0bca1a649ce48dc4fe0498c36e5c8834db51eb5b3bbe2735f58a849542013f4826fa0bb20050391e5710bafa9d7ac8dc23b5a29dbbe40c2ee" },
                { "hi-IN", "de7857a7db345a92903a65a415b3abb8e2183acdd9b91d4bcaea2288efd18b052a54e0af92a97175592a4dd58cadcf59160521203422066a3ff9e06fa3b1b4c9" },
                { "hr", "efb9cb72372b7eceb1450e456c9c7fb4247f3e7e5cc5f5915d999d87b311deaf7bffdd29cbd9f15c0b29bbba110909a8c3f8bc863c15b13dd27c02285aa1baf2" },
                { "hsb", "fa10f97bb8061e7ca7d7f86a6799b90c48757185854e69bfb8e361f56abcb3b0544aabd66181e1c882dc679cd9fe4c1d70d96a12a79355fcec4459e5b697c277" },
                { "hu", "7adc7658200af2274b5e71210661574cfffe947f8842d3755959cb0acdd522ad8d400808d8b16b934c9a370f96c334e1c51ddf2a42fb990897be44a887b4411d" },
                { "hy-AM", "d1cd5af45f30e2437e87de69592412058b9584a4176a8ccb573b963da1192b14833bedf800961529c1c5093e34e210fc55dedd19a209ebe00a377aca46cc465c" },
                { "ia", "2f667c57821dcd5f019637237a3899553bf6af24525b7277ac64cca6ac9c905a7f91b6c63bd90f59021b8e5a046836c8cf77a3f0d841f1c301f421e3e399bef7" },
                { "id", "c485565aaf1216a2633f88ce789da4e3ece3c1cc5166402ae77a94ae7269ecc81e8da1768f9c332da6d42472c82a0c6e9db72c4fa31aa16bd4373d3164711888" },
                { "is", "b6873f5a66df97dfe0df583a14f92d6ac4e1e6e9418274eb5c1cc2fabc21395558daed9dc94a21b3877e8a8643c5b1abaf9f157f5bf1bfa8a61f574aabf24b16" },
                { "it", "871e399f49e2b82b9596fee76f5c75015e402b5fe04c95b79db70aab307cc230c1e7a629a9759365c5ed1b3ace05d935aa151a1341abc6f05a0abfa6b444d50b" },
                { "ja", "e388972ed39cf6d98b3331ef349c34f2a3d748275e28cc0329ed1bf43e7429d26c360294c9e2e25fb6adff8acf9515d4419b5b25e3e3185c584cafd116ffe659" },
                { "ka", "11f0e7a31567f32a1880eabe99d231d2c439939ef672497012badadae687ef2dd41faf1d612deb3dcb7ae0df0c23a30204f06413c6b00ec421d10a568d398a0b" },
                { "kab", "1cf84e0423b986226119b2220370ecabd18bf06e1b81ca044b45e58e2603a7f96d5941b523bb4465ef5ee8cbb9b5821375a1d646904a97f4d24a5f3701f19a04" },
                { "kk", "afbd53cf4d5790298c783629f66ed4d29763da2cfb28380743cdb1ad0d5e18625d4744708c05e7ee54e9d6a32b4fcb3857e5a61684600561fcecbb634620dfec" },
                { "km", "aaebc23afd09b2537104d85f3515473fc1d73575a62a98512226fdefc74ba4a9ddd608c5cdbba3c40fe911e161f71fe662436ecf33a91cedc904cab98a4a6829" },
                { "kn", "b30f2e491ccd1890fbcf4124094fe2d68cf8c6c5bf9822eff166d1fd8df0383ca0605de1951af136577f566787526a007b37da090701466eb8a66feb4a196b30" },
                { "ko", "3919ce652a72ed16869f2f1301c13523375a6ec4260861cf14e98017a2bb922908bc5b7a61af026585e2bceadf3fb66215428439715f84b87e908640c2ee5914" },
                { "lij", "7f7a6761c33fac889e83fecde3abdfd230707279e084e741743bed7e11f6832955a9fcdc4a3e9171653f17d3dc594c47f5ba9ef5c120453a99c6822dedcd806d" },
                { "lt", "4ecaec2a944194e3f952e6b52c254d5c323f991dd361b9405fba7012fbfd1fa74cc3f026a71c384906788cbfb2d7c6a33c2aa0dee1cae298662d936aa03e126d" },
                { "lv", "302ccce65563dd7965201d210476b7ff09740b415fa5f67d09d649ac6e4d9360f5aae6c26efade74d45a8372df007146870d0bfc8101ec7b1d0a4e563ba0a37e" },
                { "mk", "5b0cee4b3d795dfa73710fad02851f064136d88ce7c16e702aa98f7db4fa63038642982b56f30af05a9e7b2dd5ffeb836d70f465ac5dd106f063977765a729a9" },
                { "mr", "ba73c3d6478680285c61bb05f88178bfd11835b534754b955153051091ea996045accaa3ee4a2a0586818f0ca54e628e1961dd195972cffafecf0e45977da1ee" },
                { "ms", "6d6de5275b5e4def533691e4f3f897baa0fae11b5638605d30d5a57ab18b338775ce03967020890ec03d3a45d6a4130482c6e6de2e5d0fab6c13878ae5db136a" },
                { "my", "3eaf40aa7e854b445ae5e1ad1d7f846c63e2aa246b40fd639ab934e57bd5a80d8cdda0212b11723aa12b3306b523ef7223dbbf5ae693c1f476f6a248a5438d19" },
                { "nb-NO", "ed5cadda938b7bfd277d3895ee27a5e61eea8ef28dfd6d0ae59f0019d308f83ee47eff81280d2a1332ac887de2ff7a08d5d6a36c637bcc18fc37105fe9a925dd" },
                { "ne-NP", "9d7c1c8926097122fed044be92617f62fb2c63e1185bf0dadcfff163058a81edb6e9b38dcddf07a3b29d14f67d6a538e6e85e369b1188a7786842d6061acccae" },
                { "nl", "61fc6c3567973614f6527bd9512c9d901004af69bca9fa097d64ca41ee8bf62b24130e04c98fcde4b612b6d57e44bfff92cdf46ffd73ebde5bba59d5d4b48d12" },
                { "nn-NO", "e4d2e3b03633fa7a3f0d71ac4f15199b26480ab8c46bc9b6c5952cfb7ea7f960c8a2e649ecc29f8b1a0b31f61e721fa5fd48373fe9993370af91641c887100bc" },
                { "oc", "abb10e8bb4f441c2fd0ecf6e74e10bdb158c509ed54d620040600ee1851940086b13d18f251d10a3c1f2a938b9d6311b29354183a1dd85308f85ce284c7f8b8d" },
                { "pa-IN", "8d2a020dbab06731f58f989e456e7bf854b584e35195dc4f4b2758431587ed4c7f3ad690c25ae794aaaf0f0ae4355f5ee67c988929004d789651c70703d7391b" },
                { "pl", "0249cb1d4850d11b8ec63f9c4eaba3dbc897e76aa9a249976b23a44f7a77c61ac4827b4e804408f5d2b3238f86ae6d68adfec349c951c745e5778d4f5adec537" },
                { "pt-BR", "6ca5a3f61b30efbd3a9b66ea781bc4e774e853fdd3d32436e18e82c99f2084b87dbafb50dde56987b5e9e58aef39a29aa21299edaed1c07f3ba30a518baf0c7f" },
                { "pt-PT", "12319cea6b3ad56d9f536ea889d3b2403c916adbbe70765cf47063443d28bb2a7df26caa9cf58bc9f12542d596a2547cacc523358ce7516cffc1f117367e2b9c" },
                { "rm", "ad0d4cc0bf42381b3c9565842dbaf05672195c20c39ea67808e54b1d7b443bc1383fbe51462d099c32ce750786c66eeceb29a59237946e10208ae19f837dd8de" },
                { "ro", "64ae60da297e2cedee37f343334377e8476f094786054f4dee631caf94eb4afd65669c1bb19923dfb93caa522526342a18109d30c9c3182be38a9d46a22a50d6" },
                { "ru", "851be9a905689862b76d6cf68c791c2d43d5d26b92f1aa06e2e568872bf6890e70c6665e3e01de4d1e5eac7bd661dfaf12f272f2b47a2875adeb0813ce61f639" },
                { "sc", "d06f87f0156b4e1701ea2d6f2b23eec4890c026bd561babba265058ab13cf2c9ee4f833049d1e11c2a48d81514fdd21ab2434de3044625d9f01ac318af6fe53b" },
                { "sco", "a7b3b1d4ee05e7e1505d46521f056217cdfd03437fe7f0111554e2f68e3028ab753253413b6660bc9278e86d9f8aff15298f95ce90072d8e0eeb88f7fcedd6c6" },
                { "si", "5eb8e70b82d5e8bdeea6308d91327a1ebd464b487feb77fb09f1e8780b8a0a64626737cb6d7e00fd6d306d316cc9d8240d3baa8f8052c579dc8f9b71c455ec2e" },
                { "sk", "0615f712c73a36478de6f968b99decee1f09fc2d6c0403c7def21be30ab64d26a319113495a5cfaa693ef99b560e15dc07316bbea11d12f23add15c700af81ef" },
                { "sl", "3000215ebf2ace3dd945003100918fb956b6373e6b0dc05a6a28e6f34fd89e144851d8ba888f2fd069800c7801f39cd85b0e83a1992cdf8be6f84e1ccf6ca492" },
                { "son", "867f18b4457f77b617ca50909ce5b4d937365ca1cb86c00675b6f640822a903fb21e2e0957e6d89eb2f20140209277e743de1dc208bcdffcea220d468feafd97" },
                { "sq", "3e52755041b6e26023e87b99f265c6db12cdab8bc62d498fc659ea24e017aaf045641f8fcd19756685a2fb75da737fef00971e1b080b1ac587945a3c3005db5a" },
                { "sr", "6c47dd88d16689d5dae5b521ac1753c413aaf9bfe38c10e3ffb3642dce972faa378e31eb1e06f94ae909376280bdb4720dca9d7ea1d1f7b0e8d882a05541b2ed" },
                { "sv-SE", "fd713f5499c9fbeb659bd9c539d238e89e9bda5dc94f9ccafbef8130c968333fbf1b0f986bf9ec730541abacccef4731790d55f5384ec601ed45e8ff7f028a63" },
                { "szl", "7f1ea15801d23a9c7dec563c614a9a93982a19eb75dd4a701397f0fd0956a4377c8bbad2ac6e3f7f204f4be3b9ed1e7629da8cf0baeef5048538ebbdf8223a21" },
                { "ta", "e561940ba7429b4ff17de67e8bec9b65eed6ba62577be02fffb91478ab6db2d08233fcd47b037700b1b92a5d24f6da28ca57e3eb348067439faa63874625efe8" },
                { "te", "fa91c92e6adbad5457599bd494f388cf2dec1fc1cef4f8c7f6cbcb2edfe23a5e3deb76800e925df76885cd5d1c632136220c6111773272851ca8dead254d5d64" },
                { "th", "1f4130ad13653f12ffac3b2183bfa0679090d48f76848c187579e698a4a784707b9ca97c5e7b3f54345ea1e3ead0610ab8b9f68a38898bb4c190082c9e1c5c8c" },
                { "tl", "38b2787bd6b3b139fdbf7222b994b9767c61b9a903840a2ba25b7767c11718b153326324fdcad31a67bc3b708b0506938515118ae3708119f977c4315b0c2e46" },
                { "tr", "d756916ff5ddac83f13743052538bbfb39c48ed2e44ef52f295d271b390052a96342cbb7ba2873d1ddc367e7d0d360fd1341ced7c5f2c0d868f89e4e9e8db763" },
                { "trs", "26678751d4fff0b95d9ef816fe6632302413f74ad2b8ab93afa759ef61f237e250e005358213dd666aa09d50cfc166c96b0103ed5ff32c062c6c41aaf072aacb" },
                { "uk", "f808c442fbf8199cfb0568a24dcf6f3b86e3c1e7865e287dc04bcbc4375a2c876880aa5e52ba0a982f3346a4699f2d9f24852b483e5ac8c11e8aaf401a6c84ef" },
                { "ur", "d1e2402fea1da9bbe833255b0bb2e13d49c6d29cd3fa4bb4ed4a9307acffc95413cbe507c223080959d5f89bba1418fbd0e4a30647788bdb5d452426540ad86d" },
                { "uz", "0b83a2b24134e6bbd0d38517642554f4b139c62dcd9b3b5ab572f8c43da1de09873c75e45d74838a926d3962fa2061f684ac3493a5e9bf20357b9cae09edb8e2" },
                { "vi", "f7f1495d36276c73b6bcb97b2e9c5d64958ac437c307c8f622a015ae987219aba8d786181ea2a37efb5a442bd3f23a0a6ee2406f39c544573a51f76e1fb630b1" },
                { "xh", "7617d28a125ef8754e2f13ca15440077c3844eb50a79c519d39feaab15f572f605f568bd832d427cf6463114a9cb01b1d7c7cadf87997790720f8558420917f1" },
                { "zh-CN", "c58c520107f2a2bf865540b199a7338a02b267082cfee72f32ba44a672f804ef5cfb9a8f756457d006aba278d655cc07936ba0191e2c8af271c3620806f4fdcf" },
                { "zh-TW", "8c034b8101b672253d022ddd8a4228f238122951ce9b9d33f7dd8229583ea0b33c046c366ad869ddeefba7d75401bbc673f20ce18085fc3453a6c48a9197bae4" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/111.0b5/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "a6bf90a135e75a85d233e144cf6d516c3fbba1d4af47dbe054cfd1e8cd47fc957a07bf5fad9018c48e244543bea252f90f8bda83ce41ed2bf7e954cd5fc2bc4d" },
                { "af", "15ff1c502b635c3b48222544f53a7352bab36a9b8ef3139578d5f7e4624e198d4f7432643dcb65b84c277aa07d14e23531d4b7d0fefff12ff20f89a78678a9ea" },
                { "an", "d3421c287045c39e072b74a43b022538bc3e201c5ec285f2a1da17ae35573268f6ed2aa42957829023019d3f888e3293658af122d86aa9a3b19c7322799da365" },
                { "ar", "e1c9eed9cdfe98c64be14a775efc5e5c14ffd2b4de8308215f16897d81bb8f2960848aa8f4363b93460b981b0250a1ee89769a2561e1c3e4f94b80f246ad473e" },
                { "ast", "969349d3828accef64a92fdee0c5689ac12323b46c051859cdb9fe5facffc88e876fdcde14fde5c804e1cdb0cc939e3c35caaf8f7b5f530d0b488bdbcfe2fb2b" },
                { "az", "bed152f9b4819b90b4cb57cb72b298edb90954de6ab49e13a9c950d2e3699a7e74d1d8e56dc5e68374b58ef3ca2379a025bae654867e988cb5521981ad7a74ca" },
                { "be", "76f40f3f370fdf19dc429700ceecbe00ee97c94d50e6faa4c28ca935b9e4986ee4b4033e8a1302d16131ecc0439fe9eb939642532784154e94ca70ade8cea732" },
                { "bg", "f913557756008ad3b25b90e5ae2836964d529dffb7b04a5a06fb8c4d78ad77643bd3a8864b76e790888f5f42b1118083616359eed77dbd677719044f57fc0f75" },
                { "bn", "e8ba3ce3f69069c1c818d4786f0ff9e31a8cdefa5325a9c7d71536ef7c8caa265d9ba38c6d775a2e4b70fc90f518391b6e000cebd5557752d2a70d399150951e" },
                { "br", "19791202a7588181882132d93e81b30c4378dd620b6de7cf478d48dcab07cc5a0b1c35081c77b09d473eedfde1f89c3d3f7501230a335f1913333371008122c5" },
                { "bs", "812ae26cd6f879aed6e28f6593ccafbc97189c0d395e774aad423dc1f47c8bf0003b48519ce5e9cff581c530f79842ca082e0c8a93ab3953f3002d15812f381c" },
                { "ca", "dbdce65e49e7d22ebc44587ca51fa423ab4df7f67730b3c66ac55c94139fa79db0d0cdf7aa3c9cc5aa431bb0527333868aef017eea53ac58f93156d8d9bce013" },
                { "cak", "ad497839cd95dd8e42e06191858ac5a4ca94d1f56b9b68f2119e5cfd24e6417edf82118258a00cbf23596f5b7c25a21deb3b0aa87991b858602ca6d75ab8d59f" },
                { "cs", "b931acece7acc35486c124cd640ab365e959e95723019a2d4953559f87e507a42bc9dfd81443faee0231b43049790117f6077ff71d25329d16da41385295735e" },
                { "cy", "7462c3b51d6c6f25e069f28d7a4dcb8668e34b6e8e96e4dd19bf48014818973319c62156673ebf9118a18100adebf57eb6569c21bcd577d5a738bc5d7e20e0ca" },
                { "da", "afed779f33ac14cb0e3de330a016059984019f0af89995580b19cbf331f608c1fc5d072f42c1604ff32c6482a01cab25f9b4e978a65fbbc23f42dabb7044e258" },
                { "de", "e5e70f6458f202b339fc743429c772411221da25764ebdedc20fe6f7ed4f50597bde6b960f275a241625037293df1966fe2e5ca681006b21abe87f8aa9cd2564" },
                { "dsb", "75bb00408ea36efe58f54322c3723172b77d1d6da5636f1cbf2916f6ad4817f296ebea34580db8e0546e345d9ba1d4928e78447288920666daa1b47da588942b" },
                { "el", "cc77a8b7cdf519ec1b3f66c9cd2b8322dbe98c5c4fac635f1fcf6c378149c59badfa0ccaf85c7e5b1511db92f77027c672144be71a044c80cc9ce522b32b3426" },
                { "en-CA", "1858008494404b1b7405d2b21058322cd1ead6b3c06d954af6cbc0affdcc89d632f526f3c42960e9af769a21c3e6264b66ed1aff98a1303010cecf6a857c4414" },
                { "en-GB", "3fd288e67b8bdaced310a245cd32913c7d95b24cc08e3899e9a0c6976fc400bdeedaa2f7ec6115186b609afe03ac9e468ff7edc6d92d7da3f7f7ad6585911615" },
                { "en-US", "26be659697f851117859cdd482ebc874fb5176362430a1a0274ffc2408eed0e12bdaca2d62d83ad7e77b010dbb8c784b9beb36618e2fded685ec7641e8ca5157" },
                { "eo", "be6e99483290c832937fb99b451512666fde00f0622139c806fdada0407a37afe236bd1f32840ebc794a64c0b87381e04cc59d191113a23ed3cf04794478b8b9" },
                { "es-AR", "4c8dbd17ac4f8c952409c466e6d671434faa4f3f8c603aa0c5465c945f7eeab4acebb774f16645d4889b926666214fb97139238d3b59b4cb20ec6b6ab57cc070" },
                { "es-CL", "00b4e95459097bc4171cdd495e19d3f682052cb29a05052427fc1b438485f5c7e1af45e7b85b57b38ed0cc3de4aa4bea4f4a7946de9d8fa50756fab04474ade6" },
                { "es-ES", "31ed20d54aa4ecaaf6f38915bbaca27d1d02e1f0cf269f939fc2817ced3926c6df0c64e7d849a44c8a82f75621711affc3f8a780486c9f9f6d61dd377ca04b1f" },
                { "es-MX", "1efe45d5ffbd55786a2bf0324b72f633946d9882fa4c68a61ee738cd4c1b1a8f0449464bd27f44917018a489a0314642fa4b7e00a226efb8cdadaaf7a77de3ab" },
                { "et", "a2dc13ca17dabfaf665e0c6415b769d5a72e64bc162999885d800476eb455b0a65afb90081b3036d8eabc1447ba2fb01c0351c2e014e6375163a875d12bc42b6" },
                { "eu", "fe4b119e56b5feec2acf88cc269bb5e63fedb554c5383fa72a67dcc80ab4a08660b71f731e4a0dd2702aa0e27ba27b0a0cfd3eb6ecf696f81aed8ad6645f5500" },
                { "fa", "27e2208d68c81a4a64138df628ee7b2b3463168d321e0688131e1c68bbcbdb915bc6c4785b85c9b2410ec6927b938eb680ac13a59a9cda9a99e9952262f39be1" },
                { "ff", "8356b830554d008b95f09d7d36065317446dafd1977deeb24b2c2c7c2a2e3b807b897c7c63380b08a582ca5573cf777766e137cbe2bf6904002ba0e05d007f59" },
                { "fi", "496ac76a48b181fad282261ece09f86b6bef8b8372ad70ecb88c86cda65a3ec80d1ef8aac59c8cc476c2b3ce70c918bcc3316b69a86a6270382c553a4670aeee" },
                { "fr", "269d506a65454536a83bc41a6c782da0b2bf9bacc67b0612e29bcc73d7175c7149d88db0c292fc66e774117241055ffd392bdf0bacbc1895868f13f7638fcd79" },
                { "fur", "d6fa60256868c10e1c557fd030462fb89ab975b067c42bb22f9ea20942f20033ebe9e627c2988546027e6fde2d51e5fd705d0e66b1c9e08813161af6edbf83bb" },
                { "fy-NL", "b00362e6ace4b90ef8756713010d24de5428a813128a98dfcd845eec1abe689a569ed5499cd613b7876e27cc0c9909699bdd898acc2f25bc54b0d87d95c1c41d" },
                { "ga-IE", "f97e049f517c44ad9b39316d702e761f3b4d1c765f6fb03c8dbf768de361d0fa9a5e0595aa87aba862eb0bcf63761dd1f07e5cefdab3ed6529a62be446feb68a" },
                { "gd", "f0b3ef00a82364715e10e6e3ede1586fdd32d0fa6352f96874e77c0e71537b48da0e2b701397e06759f950a2ff0e737afd44a7faadb429bdf0f1550e8f131737" },
                { "gl", "4de16a006e2a0eb07b0897e263e495cafaf5e9925758d6b238847cf95db6500b6913586fce505b45de788ebdfb6255c78159a3b9160d87ad4c089c06c766eddb" },
                { "gn", "da9abb2b10284a8ac09df18aaa84b3785bc7ba732f9eba0b53b790732912d34c454df97b164c3fea133ec545d10f2564c9bd22bb5d8180f6660d211d8bc7e683" },
                { "gu-IN", "ce6985b7424eedb79d38d5be864166f1ef0b5e9993f34adba42a22f40295f3e98f1a1866bdc091835c4a90b281d66a9602e95105076f3f43b510ed69a1d89983" },
                { "he", "2ed91082558f6e50c42440ce7a9a237b3980c372abe6aa5275172641e7a4508d45f3506528e2f4c0d58225c4dbbf0d4c3e852f8f268b94634471e2bcfbdf7d8a" },
                { "hi-IN", "d4d29bffbef78b2fe750d1611f56610e642613e75c9d8b9822acc1c80781070bb46fd93e3961dafc50b7f671cec61c0857f70451355855f5bf0ab19210a05972" },
                { "hr", "151fe1dfdd835480310af0d631a7d2df2d942b5db964087a646ff38e067082183412158abe154c047bea4a12875a0a67fc75f727c373ebe8e3e72f117086a054" },
                { "hsb", "3568491a98e121e0e86d4e5341cb0cd03381022d25ed900759bbb153444f44c2da3f9fd4c464debe0d405ac608462b7af12aed9bd3f43b85bffb8fe1f39479e4" },
                { "hu", "bbc5f3bd6bec1b0c838e66c58f941e98dfc867305a07e0e252160983917832ad0177a792e1df37908c51510861d7cb022092a701cc2ae066fd4c6866b7ac5d5b" },
                { "hy-AM", "7074a6252c1882e36cc8bea556e8ffe166ad7513891300920f0b4ee73282f84140a61993ab436aa4e7f264b5ea195cd0b0bc9a72b74f1f567436b70cb744f194" },
                { "ia", "78241f5eac7eae9548e027aa16466821e5440ed5422ee8fcd7f5722fb6cc7d6f046fa6bc934ce9bba4df05eac8427b345f2113dbf3eb501617df6305b1171c05" },
                { "id", "48da4246d8891b580e1a27179dc0f4902fe9caa6d88839dc8f0622bc01d13cadb91d5373f8687153b69de1f792884e95aac5276d708e98638d6ad3793041d459" },
                { "is", "dd7c265e6c5e35a0e4dd6c38ec228c6cce37aa9155730ed1d08eb113acadf8d1e3abc9ca56826990ddaf9f2588ff06eb8e43e9d817b2ab09fa69de02871cb421" },
                { "it", "c2c6e7d0ea59597280f525a10d8387096d45a34e813cf54a3d5c8ae13c0747c70d6edd323778d14b7258fce2be27a01cf93f26025ff06bb8de9e56aa29b529e8" },
                { "ja", "6b267dfae5800aad44582c476a02bed8765c9ba04ccd177d6bae4dc39a78dc1a454b3ac8550b067db34d6285986fb3063fa6e7d831fb6579f51390398e3a0936" },
                { "ka", "a0b2aff18d1fce8a88c14b22842bee389cdada055ace1462f86d35574a5e79ecccd264a39401682180e7d954d6ef075ea87f58f96751bbd73b1625651817b23e" },
                { "kab", "7c57354db393c2d37776fcaaca3ffbde3d7d2a2bad62be85639f3da61b94bed1fe42163cfc4f22bfadfdd027a768456ce0e63b43974d0e0a4c094e2cf6b014f6" },
                { "kk", "cfa97c26314f67141760aed94738598ba74596d3b4fba2a3dd52312b9ff4a70204bdb76035fcbd6e55f1ed80342f1031a3fc0b3647fc914531937fc591ea8140" },
                { "km", "ce774ccd36d890c9651458fbd3c31d2f8820222b5a38f29a098e5ea4abea2586e39a05a8045676379e514aa768a3c57746464cb1c709b9575c4a6e6e019bd1c4" },
                { "kn", "3270c332de1730cba4db67fe4772afc5a50f0d45915c3cfe84e55e5ae7b3d46a7965624026bffbc4c6e3e80a341b2645f355ff9a653a6d401a9e0bdf372673f4" },
                { "ko", "615861596ed812307257691f051a3fcd161f83029818aac473b7e54b8addb92fe37a0979a4e3354232dd28faa4be1b51d2557ccfd31c860a5c7e6bb708dcd66a" },
                { "lij", "7cc84ca36ee2f8be1d4376fb3e55d1104fe187650da29523ba769b6a377f3d78c92b3e07614d7cccd2aed0df34be7724562991ba92d6056fa4d0a4b461aa42c4" },
                { "lt", "bb40ca101daa4802134610b85b2f0b2d474933177f8fb43775790c9748a6ceac83fb4c0e878b40f8ce736d1ffba28a4bdaa864a5a58bb8db34e7fc54d68b7003" },
                { "lv", "4aa825195ebdb5c6afa6dc1f2effb2ef68acc62d7a7bd9148a781433d4dbcc021528400ef28001fd67bb34fc240aebea183cde4a421b34be01035efe9bb5e213" },
                { "mk", "82eb6115d0287004168c89dfcb50a49a4c53ea7c5dbb2568487d34e836553973b5d0abe40a2fe2041528a29a8176949e24ecfb2ee0a692e220329bc61c53321b" },
                { "mr", "9d7e7949271074d8b813bea5cfdee82c12d3835f78ae9a73a3af01da2f090c3394590e8b7820e03bcfed565519c1e75fb85e8da3bd976c28ce617021f9c4a2c4" },
                { "ms", "4c8ea82fe55845a8a68665d2b6da7f6158044efd7aba20a3c438859a59e19fe881a2c903d10d4da5640ca55d26c39de4b44f6dbcce5c77a82633cdddaa53c24a" },
                { "my", "752ea1289a10bab829054bcffe8cf35db21b0ab88e1053f358ff16a7db39b8bc5a0b931178c5a54e06ae9cb2051f8fb171d7de70aca8aac883b69c906e3650ca" },
                { "nb-NO", "e890eddad9b2db18c3759e19b191193f0e3572da1e89a64c538fb5b85c811f848d346a1c07c99be8294ef80425cde62b5bb8fef40d554be9f2c021d63c7e0134" },
                { "ne-NP", "dbd9c16866e2cf0a0fbf7683981f44a34ad950a7f2975e585f78ac1461cbb79107c6d795164cdfa150ca85d7ea23daa5eda3abca37194cffca7dc1c710128942" },
                { "nl", "d3dc4f18f2bd1e89ee9fe9e155c45cf6df62bd39b08d0b26574204422cf6dc87e41c12f612553969299b082f16781a9f3f06a96f4660208274714c1d62ef98d2" },
                { "nn-NO", "af7c17280bd13408e9131fa45952e885bad3d2365ca6bb4e017d374649924b7eb592c1783dc154aa7f8dc46a1bfe06f392c55ac9073878b14bad17afeb591191" },
                { "oc", "59156f83655d8505588979aed19ec2b516668a524a24c3178986dbeff7dfe2e826788e5eb8a1200f17f451dba3cda1a29eeb17dc804b03e3d07d043fd9695aad" },
                { "pa-IN", "5913580b6c3f72fc72dd4cb5a1632b79c500703244a6dfdca547ff7e70f5a60fd89fd3633d528cd645c39591e2db34adb22ffe6a7047591ab61f2adbc53adcd0" },
                { "pl", "ba3993156b3426995ce6012fe43e44b84b09a5f4c881f4bcbfc2f3908c1cad592434a6cc8e565f284a8ea3d24acb9ffb2e259d5aaa32353a9965fde1eef633e0" },
                { "pt-BR", "71b47ddaa39860c017a6408e01eb16def46753440e15da2901cc886287b7da70df6c87a413985f52a9930641632a1b0cd83a9bef4dea75cb0e9a390f80c9a0a4" },
                { "pt-PT", "2578d0b94b16385f1a149be3623292f256d82feb95f6e884190903f972ab01a5274469343968cd9acbe8bbfe12773e99d1a6bc203e02ea7cf2124ea52620af3c" },
                { "rm", "71486dd51415061bebae5f0b35706dbc49960a7898c91485fbd813b26cf5d82cd9afd47e3dd1c5bf96fc506bdb0ffaa24bbea75709b5e63c39f1a484e6d0fb2c" },
                { "ro", "b8fab7e00c281512e4179a88d5212c0add53435f1acaa38bf2c59352cb3ee3de0190100a5f1416c93adecb3f64bf19a4b019afe3a2635f84ea94f76cf664d313" },
                { "ru", "29d585ac31113b57a9b4a9af9f18d90f02d51b8363e3fe37b0cd8b9645c13fe578784482ad7b0ed86b8806a5e8d55b33587f09bc5e420ec5b0f1dca327447bba" },
                { "sc", "b8b1447c228ad7d93b7671e1fcbca189aa79a1061937f54ee41f145863b805789cece67db9fd18cb59d12bcefa7c2a3ad8e5a79b17c1cff61fb06af1e89f2704" },
                { "sco", "16171cf217ade80081ed0ab3885727874d179b0d3b9d9496656a8a3f043c840d0827841348508c832e693a46a70bea62c7efac83ca8cf2c7e446a3c374cdf083" },
                { "si", "bc0467172c6dc52a798a02c1c26bae75c29397ce252fcb253dda741cc2d200f5b0b65e54e544a6ee6221d1adc3bfaba23c7757e643c911b7ea7e547ed3bf6362" },
                { "sk", "a5d00ebf666a2fb99d3a6d4721307de577bb1a4343cca1c969932aad71345e86a46897b3a78b61bfa9eb5b8589ae91e7f18aaea0038a9bcec194aec7b8eb6008" },
                { "sl", "10a8646fdb84039d9c45f3bbd0d3174df2ec7d4f11d3bc97c2cf03c633e9c8ff67086ac169bf6dfc177521346ee4cec932d7cd07c1c8e8b06c662fb46120f780" },
                { "son", "2f526a38fe0136addf62f7f1b10bb00dcab478434518bc88c6cce9fa3e05d45e194f2e9d6ecb5566f366c80e124a6e289ff40a57d6138d41c612bfaf9ce6eec5" },
                { "sq", "a3a46987833782d3f529891956e1736d060b0bfc0300f3950bab8c68f88af71f09e3878e181dffdb29d250c9fdfe55645a0dbd83fe0b571d4bd915ff38f7b0f0" },
                { "sr", "e5a835308de22500e84178391c706e5866580a30d5a52f185fb923d7b906824db90705ce4efd84b645d49ae63455e670fe67c91862d263433d38fdf1f343d9d9" },
                { "sv-SE", "1923f85330c10f9b2c16e0abb1f7861922941e44a61d399c860c7ea11219b1551bf820b3fe7d2e4aefc35a937851fb937947650b6196796e4bc9cd13d86baf90" },
                { "szl", "657e884b15275c3576e0321d30067dcfadbed4438b514e1582f295cde247779dccc9ad3717ddbae050574e0957bf4683dee261a6622be99d120e5140eb794920" },
                { "ta", "f7f11b3f6c3ea1f163fb866364a6e58a30a57945119578781c82e9c83ecc7ef0955000060fbf09a3befc2363a3818a008b49b16a222c4c1096b21542c7e298e3" },
                { "te", "4834a8056764b98f748d8c768227685c4700d60b70a49436f3cfbb6475f321381ce735d0eef7e23838df2613937ac8d4b24c8a0de110ce69d53dcb3ef4f5540d" },
                { "th", "da776b327470d796e4e16237b9dbf7a89bd8d84f8bf63fd5aae99d7a9876c8aa1e09f0b658c430a7f2febc2c4af7d4808d0e1eb19a714fd29d4d5cca7f46a055" },
                { "tl", "5c0ab6c32f6c05834a7c93c9223ecc8654a6d16546d10d6517369f1593715fad5e2280393255fbda8b9557a36094b626b0793ea7044ce4a18e62552dadc11ab0" },
                { "tr", "f82d288ed7e4c006d0700b946f2cfd041727ab3969dac0871d06fc7fdaf4b0a30e9bd0b94a152dca05c5812ab266c82b5c68af8990640bc6ec8aee7b61992639" },
                { "trs", "d7d2c6816d187e654c19dc0e76f4fa6de3ecd1850e8c22afe61e828b638af77928529175241ea0628e7b78cc4499b813e07552e5aad5263744600b5e0bffdde0" },
                { "uk", "c4ed252eec4d7876cd34826f3f83a6db65c4c33304b3572b21d2a28d3b8681cec9bfe92795e74359c18ac315b948b59ccfddc123c698488ff2140b07948320a7" },
                { "ur", "c49aec9a62853c921a78a4423233bce9313d200108cde8f26b4efba8b92c0738d418d3a394e076bf3f4cd96ab347aea2d6d90990df2bb2e8d30688caee649138" },
                { "uz", "1715e63a581bd435048edca1418198c3a57111717b6636507df299314a84f9097bf652296ae78a16dea407568b2066be506f1ad240ce95b255ceeb6938b831ec" },
                { "vi", "af795db4e6c6ee93873f72d6d16401c32001775c7ad2d98a498c0c725fb8c930f1043a7330f6bdecd264184cea5d32f01c38fc6961615dbbf92f6741b9ffc110" },
                { "xh", "786db021a8512510e681786f1e5e8dd22c47519f11bc3c3ceb6995e27870810b44404cf8dad60d03e1e4ace96aaed841142ceca36124122a3141d4b67e8379f2" },
                { "zh-CN", "10b95868ea97ff6121267195cd8b71d965d59cf30e2ff581c70ae91f7d67c5bd506477afea569c3148bb44396057ae3645c270b2d2385d5c40df516541618fb8" },
                { "zh-TW", "ac3dce40c5ac7c62d735901c8c0f64fd2fe59b5a652a21bcb9fa8829b12f0be83d6413c3e615e26441786a65a5ebd7730c77962da4c7b0ccd75212426008f516" }
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
