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
        private const string currentVersion = "109.0b1";

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
            // https://ftp.mozilla.org/pub/devedition/releases/109.0b1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "3d153a50176beac1241c381f95ab2eab8a74ae686fc517e3f875f936cfd44244a099793a835cf7f416e33124939707cf9e16808160a5d3c13268dc74a2436c17" },
                { "af", "1a8596cced4de8a315eb7ed4d21de2272f04de6c9f7b2f1e0aab24139cb426503be8f70680ce1c5bfb1980f919b8789277cdf1bffebb84d157669b8307e8af0d" },
                { "an", "1ba7592d43de75eb6fbda2b6ec53daa5e7091adb2b48abc4cf77b6c73086fb02acbef617a0cf35663ab5055197c74256c238d8eb862f1f4bf9f44184a94d32ff" },
                { "ar", "4e17dee2a8839bd9b1e5b93e4a125bbec2e3f2a67ddd85091c55e0f19943260f582fc2fe205b0edd00162e0e5b5cf2f44cbf3987b9350f8a5cc51df08dbaeeb0" },
                { "ast", "6ca18feed54386c43c751e181849024f90356824631294bbf1d8404404a7f8742e6d03cef2c17df2e8221c9bcfce60c913799ba1ccaa09332727370fc63ce124" },
                { "az", "01b949fc738cc4b140604c9a6b696b6cfe22caab062d999ec66493c3451b300c35685f48d1a00bf60394ab1412b6b35423eb44f2c1eb91feee614832ada86fda" },
                { "be", "cdda72d91741de88b6acb06f17857818b64aa285e602f047c7d7bcf42822d256931ae76becd9047e14503581122d1dc351a936ff0a4a5b353c3bef7011d6982d" },
                { "bg", "2449bf96effe1c532e9da77d1326b3803efbb3ca95f4f7d32df3057f239780870b4b79054c32b5bd474ba9d6cac1537bf99cd67d4dee83888cf236350640a54f" },
                { "bn", "e5f5e72d2dba8d8d21b8c43aa0a165bbd1b525f8ef4ab7607aaeb49244f818262b1dd5c671a2e0b33273ee75e40978ca92e993e53285817315c2f30eaa2a3f92" },
                { "br", "6cf612225cc294cbd5b6308d3d727353796a954b9ad05b796547541fbdcf541bf6734378eb7f98e0405040373004140ea94ac1a286f3ea730cd183a161036622" },
                { "bs", "729a66d4b4a29edd703fde100053e99152c90346bd03fbc4c1edc06e74918df5f0f3de478105b2068fbdc51aa2ba2e3cfcdc386d27c07e76f3e67f6c1073e92f" },
                { "ca", "c3c05eddb26dbe47951adb72f726d0d995debfeffb0b75b99a8a96da13887107aa86503ff753de87c04b36aeef4a085832f7b0c0315be8545e04624c6cad793d" },
                { "cak", "e2cc03baa19670d0dfd1545f901b707cb3f1a389f1210e2c78e28cd84b04191eb70fa110c32af56272053e948ed92abea51ee1fcc1143803c421bef28795fb82" },
                { "cs", "edb0d544a12b78c3baf4e2c63d041a07f049e5db78244e2ec9ee0cac515793b6cadddcc6b0bf5b59dd594a7328d9eae07fd8acaef4b322290b06eb4b5eb9cba6" },
                { "cy", "b3fb9942eaec13bff33feb8986fe97f4ecc338fc95c62b13baf7e166143ae3aeb7b93380ce602ac970d7e708959b3b5fed6e1556e4cdcaeccde365a33c79b10c" },
                { "da", "2cb898b4afdaacf1ab9255c12259337d1305d77282aa81086efba7e941b3cfc40c2d31688e8782de313a3e2f1eed21796211457a2a82903a22504518dee7b533" },
                { "de", "d12f577ea1630923ba56304d5cca18af860d15c41e202852ff41cf4fee0ebfecb5d7a100382b14ec780404a398090cff95c84de1053a0441e25f2a99ccf6bda3" },
                { "dsb", "3f721b7bf20f0daf9cee1a51af00b426660905869db87e3aec1d0eab1b8b8cc283431b909dcc2af1641b59f31e00aa220703a0b54b32475aedac513d7e17bb23" },
                { "el", "2d2586b97b70302f83ac41cd15350ef019e6b9b5717592b5527076d5e744ccbbfd71d2a0340204be74cfff34beca91c54462e06e2afdd4ac1e9ed4483bd3f5f3" },
                { "en-CA", "7f50e08b3e29cd96eb0166484109105ab385d80ccb069d68bd1e0dfa6cb02c519f3a65d24efaced58d8ce58ae9634e7d2a09601b2a3c6d6a66fa714bfe4eed9f" },
                { "en-GB", "8a6aaece9cb7b537f40f01b49d3910fa035f24505ce724d8a4b2eaf27780d7c699132e09717df559dd7662ab6270fdd255fac6565c06633cd587e906bc3db840" },
                { "en-US", "47d9f0b1a7af441b55e0b7ec5adf45c75c37d6202263691ef9ef370a99d972afb899182db652f0a9a43d76d76e42c22431b60919d879066e0368c6f2fe2155a6" },
                { "eo", "4391e0c2e31a86dca88b23a8658dba2f916488877f80ee55a8afcac2c1b4bdb73f604ce289b98b5b50dbf1466f321ca3fcf116a415b1238915f17ec829cdf9c3" },
                { "es-AR", "9188686ceef1071a8673fdf532e9f6a81150297c25dd36a248c4d644be8fbf1e692f37bc0259e20b0d3b612a6b875ca67e6b65666528ea403e3a5734f57add40" },
                { "es-CL", "66bf41531b11726062dd60496882f94a7eab2119a55dc1be20204bb03209ce61f7e471908224e2d8db8219578155d8845d66b8c72f00637d0f7d73d2d2cc88aa" },
                { "es-ES", "e408b64ce02df55b46172e7bd97fc12234f173b5bac9f19b87567adb80917c1bda0b6c0dcbd7fa97f07998aa667978b5dbc0637589fe4babd5767f1f39f61795" },
                { "es-MX", "986889b2e840c9df8d10259acd61683af2306e0250df3d9ac6171dc522b142be15be8b4f63ab93ade4aedbf4d5673e4f27ecb8cfd0a4e40b17a3c0928db95078" },
                { "et", "ce49a380f8de14157b85382940b9d83ab5d8a6b3aae18f863f9eb81de4e6ec8e5a06e206ec9668c1276a87a7ceb0ecbd41b9730e86e199d0972943463a1aa952" },
                { "eu", "a734b2b5b3842e741c4b15808eea4876c3aee3228d7f9d5ad1b34569e109f60ef036086ab2785eda19c5c29d25505effd5848cbced82cde36bce570eaaa9009d" },
                { "fa", "914efcb910223457bb3dac591d482f4fa097f07049fb0497ba9cd938a741d4043565b73d0c2aaace31fa050b6c0bcba76b7814317ee7492dad585cc6c52697eb" },
                { "ff", "51b80b98dc423cd3573ca5027f8fc42c409cfbb38c1bb80c7fd396c5338ca71bae82d20bfc8511730662ba7cd8416dd6f9df179a0c67629f346730cdb47f05cf" },
                { "fi", "5b38390352efdabf5fa137a915557d9f17616645e5e020746a550ccccf5066c8d674da64fda164a8318360f398f3ef67d44a65f276af8aea0b7a6654732da4c0" },
                { "fr", "d56deb7cc17f8ada2846932e166ce1b449448105d7f9a24605ffe1fff4d956c49db0ed1b2aa129000c61209c8678a106e06e38a70c4b95ca75d0c57cd044e06e" },
                { "fy-NL", "a396ae49ccb36bee14b6ddc22e892dcbea8f36192e659f2fb639872bfbcf8dd2bc52779aa575d052f02ae5cd08181607ef55eb586513794cc94bb8ab7f6624c6" },
                { "ga-IE", "ead8438fc8d04565f538f44fe6c15816ff7b7a5a84a39b0e6d28d0b2b5576ffea69e1cd455d7a73da3a2f6e8c05b73fa467a885f08b47179d1fc824d0cc9b6c2" },
                { "gd", "8f5a57dfcfccaa223caba6a5673ada2f4a8974060bfd8b5dcd3b63e7bb7d794db497f073a05dea2a19d0264c219182fe877d35811d273b1d9d436cefd9715a7a" },
                { "gl", "58c02b580fe710c6e1ed97bafd74ed77e94816738dac5489d3433e5c436772d749d98e7343189e5aa0821a72ebb9d6adfc1bef423255236d86c7ea629c79b666" },
                { "gn", "d77eeea1c1568f4d1f039bcbe5bae5c12abf7d6007f4b83c29dcc39a85d47c015724b20ded531bb7c8afacfee2f6cdc0913d78e0df2a3b4751f19f29a6de9ffc" },
                { "gu-IN", "a3deb6f321f0b89dd4eb76232e3ef15ce7557b24e3536420d7c92b2804c9800296ea83dbb8fd20a257ac725dec1e3c8fd16d985ecf2f2ec0ac17642b87dbf780" },
                { "he", "7fc520c66b50a40021b8743875a337f06b5ba745323997b20acf9a1ba14f5685fadca8e379b28248bee2036b10ebaf27e8e9709a859ec99e39de890bf756530c" },
                { "hi-IN", "ab3d38e3bb6a49fb0d3a2d6455528212edf2c875848bd96a12496c4ff9f7c281e848264954f8b64174d4f1ebb7f67c98efc359f954f81062180cb8f79ddbb0c5" },
                { "hr", "cd423d31daa46de0045894bd3c3b9f429609e6e9044dfdfba780952232b2b6f23e0c2f30e6168cf28967f46060b64587d0131d9fc8fe2cf91d02f8fdf57ce3b6" },
                { "hsb", "0c5e936815f3db3aeed056b71e9ce9981aa398ccb744e510418b96f3bc51d00fe78f2357822d22809f3006bd3fc8ad662537b7fe792e6b9adc4f868cb70741f6" },
                { "hu", "c5c3ec02b9b9cbce72d484e94d40ed108b4fa36df56d91701d399b69789a6779ff3f76e93f3042950a2906a7b0b78e7dba04cee5a7dd4d7f2346020d079af264" },
                { "hy-AM", "d8896a4ae03fed3f6f7dd66a2fcfd25a06eb493ae93488ba1453ec3ea445383957325c93af7c10084f626ee62339020140b038b203a93cb164574f71729955f6" },
                { "ia", "7ef564ed1aa66226915154187832dbbff66b6540decd79a62b796704798cbb5f5d356e9f590dc27d5f11af1562421bbe0656cdf7abd2bcd751d8ff229383e963" },
                { "id", "9dcf9bd87aa37b6a01d9d9768297cb75cbc7a6d1fc090f505b4dda279b8233f4fa2965bca71948a41685212d4a297578b4b69aa3a0604062451aad480f013022" },
                { "is", "a85d6725be69361b68b2d10bb6bffe750f61a14e426319dfa18bc0058edaa2ff3c25255f8e3f218579092ce48f3d92f71520b4b4cc9d716fd3ad7255c5a2ee1e" },
                { "it", "e4a3b94069175ab2eaf11da5196fb0adcfa862dad9fd5526f94f80087b675397340b3b3c483fc804081affb002641252080488f3bd69668a8fc54ebd4c7fed12" },
                { "ja", "50c7ab38cfd379bea9592ffd3fa509297a6f03ed70e55e7ab498e8cb9ff3a4648f3c0ada5fe9c2a778fdf23dbed973d6d3a715a008db5489b93cb380fae7c050" },
                { "ka", "a144f119194eb9246f419a42ea857b0d46e46e0ea627ccace2b9620bdb3e1169b962dd92fdea7eea86abbe69ec2bcc0522cf8418b3b98736715be41b44207fa2" },
                { "kab", "612760dbe16c32a108e63d287948183854eb73a259e5880e4529a5136c86220285b19de77e28c614021fe0d5b49ca055918641d39d43b5302337cdb6aa19437b" },
                { "kk", "6670359483f5512668547180429f94fd54ef4b5ef21be5607e245369d29b1e3af71bb065d2dbae3c6198b0aea26bb6c25b351c683d24e0e6f391419152376615" },
                { "km", "cb2cf53d3b4d34096b29cdca8de56f223f813198b45a8b16d7709d6d3d51648120181759e972271bd2e54051815ae66e8301aea45517ae97fc6e9596f275fd4a" },
                { "kn", "f477a1c514637421b59b621e885fa06fc9857f7b926773ff8c2e717fed3e1d3e61f2bd6f6e7ec427b3a692efcb19f6301e048d2afefdc5a0454cc06a6d3be01d" },
                { "ko", "688b6028e2fb633034aa8d5801bded8768a2e143291e01a811d2158280c0e5b12508f6de9346186b79399c8a0b8f577636a2eeb0d576b9a36bfbfd133d5675b2" },
                { "lij", "858f1a9596b3274b219422b20b6b85dcdd38f86dc88b908036f73b5941da5d8bacab403d380f37f4d8ee8e9ef056f4ca144e62b350789c1c5b251edc68207f38" },
                { "lt", "8c46d905fcfee185cf8b76e9185beafb5c4e3bd50416ad6f98d0884c3fe75e8d42338cb242a7e66f672650a61d109f11baa5189dbbabe1562dabf2e61ca829f8" },
                { "lv", "73a8d331a344dfb215b7dc9503b3351e5775f321a9953955c23814f66ec30b98ca2ad1b66e0c5615580da8e46e248c8d930272ff10320704675d1e2302f5f0f7" },
                { "mk", "5a36648ce265ec558ad36816cd10e38a4778901be3edee944571cb0e425d1f383f02016121189cd2bbf2c826fa0f489ef4e09ca46e95cd55b451f6eaf4141708" },
                { "mr", "094d6527e8b841e105636689ca3bc569b7ed802e93ebf5e6cfad61212e992f8a5afaa5c530942d73baeee70d6133a6a810a339cdb9d6b5352dcc9a656609c237" },
                { "ms", "f421fd81aad9f37aade0cdf74bb9226cd331813ce14ab04e48b08c3da51f797e667601b7cfe61c2ef325aed03da697f92291b6b87dabfb67a1b2bbed16dea134" },
                { "my", "a5c2b883fb8cdeaaf6930410a66be22a9d64bf8283703c1490f224eacecf46e24994cfba502c2451cf05e312dd90b96a03ca099aa669768b57e16bf496c8b1c3" },
                { "nb-NO", "df93870cba10cd5fd4649a4b5c39876530fda83758030114f556121b41ac98e557c22889db4804e0b81a8ec9aff48af1438b7a79fe59a7bf28a09779cca2032d" },
                { "ne-NP", "753b65c26d1e1b8bb11cafaf27eae654746c2b8af28a0976162d77c3dca81ae91f833cb500107285405e6e0843797ee803bb290eb2ead2a37bb57a1f208c82f5" },
                { "nl", "e8056abffdae493c2c5d3ade36485763833df69789eb3226e35a9f06c39cd620b07e4bbc0283b7e147c8ddac617c9e332b0b35cc3aacf3407892da3531d1f811" },
                { "nn-NO", "49a9f4b2b05c099c4a6f838ece10f5d4886d61e85d428cf4459d097e2c4feaddd9b3fcbac046ff1d8b8483e8664b8db25bc2a4ae08b2acae0e91be8bc9f42e2e" },
                { "oc", "b9fd6aa356d09f841e73c31bd295c64d3222350a44b057ca1ab91ea152e3ed7010a4a1d9f6adbc0d3987799a54f5e6196021e56348972bfc9534e8425f90234f" },
                { "pa-IN", "5846a415a798d07b740824a4f77764bc2ebab23f769fb7461acb34c18145b83f7100625e2f0274643851d77f8922ea346d8577491c565e1fe9edebf1ae329436" },
                { "pl", "f82dbbb99c5e87d09988b8fa4a6243e0d7e2507435685dc69f842f1bfe395bc149cbf389e72aa65d5cc0760cca65a68e8075accfae46497ecace9329fd72b690" },
                { "pt-BR", "ad55fd327fd632d8fb068b6e8ee37076fa4d82894789b8667ed985c70b274e022512537dea75069dbede10d3da5b3ffb731e175cf2df0b50b17a3b75538461d3" },
                { "pt-PT", "6ea8bb7970674f2636260ed1579c879385822aabb296250a0a239f19c9a580ccff87ea001ac7ef41ee0d77b84784c2479552b392ac62a7d2581da3ddb1e86427" },
                { "rm", "acb1a6f4261e0a7bf50add38d95ae39f56acff2585ad1273c066d86ef7769ee4aeae002b96ab7e277bef31fcae31c404b640f33cc7846bf059356476c9f38b5b" },
                { "ro", "dfe84ec0954a762464ed888849b790cc5341feec30b2ddaf4cccf32044ae3f6cdc8d9882fb317762bc3bd8846bf8a51305b30d3b569cd9a2eae63a5473de9de6" },
                { "ru", "bc1e02d89d5eef1f69a9f2303cf2c11771d5046e2e9220a1b3b7bf7f71785dee675aa7ffee2a5ca666ad84168fe8c7fa4ffd2a1f07560a7e2185f21fd98c4b03" },
                { "sco", "7848b485804d22dbebb7dab0e0221e65918a6660cad57d37a5cbcc0e40ac770fe49bc8626486ca86033118d67d3d1c8c3c27fca7ea0639817f35bb4a1d9014c4" },
                { "si", "9744a46430c90f076a62f0bcb8df5390e1e6129707a32d3ab8d4ead4615744dbc1d739c341e3c65ed70cedbe076856f8f6ba7b21da7204e32c3581a7c3327a9c" },
                { "sk", "2c67c9a6dbb0e3d44f067c66a4b1fab50020ebc6f84353b5fd93014647924a4f93745713395b0326e9ab5550cc58f476ce49036eb91267e39f5c505383c9d5e6" },
                { "sl", "f144222fb6edc691cec9117c766378c9d38f1d76df9cb19c4501e62cbf33b438c099011000de37922be1cff4d4012a52d6f93da2c3dd1e28907f10f8343aa61c" },
                { "son", "6922f153e1b345a18404ef430129caf5533c86004503f0efdc2429fe46e77ca96f35477de342a724c928205771d52a80e726cf8faf63f993f1f0f27587110de2" },
                { "sq", "b09b9c6fedca0edd455ba9844c6e59f77dce67cc6fdeb54be64e881dc8e305256fa5624f7b196cc7914a267f9675010634e97f284145e4f25bc10e35474c9b35" },
                { "sr", "575c4153da70931ded1d75a081ace037f436bea2fc94cf48f51e9d0dbf113cae4d37a792abf520380f09f5f69af03ed2d9f938a974ba3e83aff8b7d55873d02a" },
                { "sv-SE", "bc2ac8c745f74341207db6f81520b6b07036ba4b5d1428a65a2d3b31117ac57e11a005e3ff2888e7f6b5f05d891559a9ebc9ac51dac49d0e9a00d72b3a8c0ec4" },
                { "szl", "33ed1c7e2e0046b8cae16a7085cda7b3f0009d534761d9ecca006777dfa04c5e6e79ec39fcb86f74d16521ac218f5ff16b108bd5db317065333a382d940dde81" },
                { "ta", "d525e1b4d6900898f1c9963d05ccf7c1289ad85519baefe2894e2b073362437b91e36b11a8373e419e503a54913fdb9f2f0294e48f972bc5d35e76bcb570c589" },
                { "te", "9fe54d2d4e39bb51d652663593e42106dee97c073d9d81726601c5c6335ce9e0504f0ae1ea92aa47719d97489966a940335b9f65de6aef5a9e55d7f53fcf1e9d" },
                { "th", "92292a35d6860dc4cc63f79ce323e162dd40930fdd55d16ecf8251abd24b886e837aeb094584a3ace47a433697669e7064d6da29f67fd0370ac6b5cd0fad78dc" },
                { "tl", "25f5ad97a1aa26e062d163a6e2d994065ac42e4426c7dff248f449f6791977bab82ef47dccb8ad3e829ab1f17689c2e6fb10816a12c53b8105aae9c23a992c46" },
                { "tr", "57ae46e2bc27a6dce0e6af2ec641bd042abc2e2bc943d038c2dc6793b628bb7b605ccf787f9ce733a579a03f0253b842931b8209f7f6003a3c37403ccd18ad01" },
                { "trs", "c4daa030a6507d1c518f61b1173fb6d358c56b57ae16c092b8d05a518495fd296cf59a8182f8dac77ab2aeaf62af452017e8480f6e2e5b5a291dea6c4b89a9b0" },
                { "uk", "8f1c46dc39ddf3a1ddcef290c98bfaf0f7ebecb63cdfce1706dcb41f4f2822894af1af04efb5f12df893c36f65d5e7b7976f8005390e254fd1f4c9b8ce5a5b43" },
                { "ur", "b9a3f8d8b7c609b5ca635a1c4721f3d6d8ac805da91658a1691d09efd22e82e5b92014f18744d78a4056b7fcca58180473e6202a9698383f6e3476852e573015" },
                { "uz", "da44117324f612c1bce29ece4cb8b45b9e41a2e2b98a2c6072209b5785a30fb59b4162111c43ab07a7b60768b53e37f86b6ac5b4c410b26b986b31b6a291e047" },
                { "vi", "7f40a5d3aa38fee180f336ffb2920bfbe7c3218e5517689435c24f4eb467e8e0a3c37bbf63047e4b13e9b9966c2b54839424382064ea60ae59a93e7d6ba13949" },
                { "xh", "bd471f3818b2c57112164b8d8374351db46cb56b8235edabe29e399a7751784b024ad2e4ab16d202f44a0b703d1bed6a9ad9fd5ee00b6b2d8b53a44ea7faa633" },
                { "zh-CN", "122fac15bcd244de6c5c6725eabcfe2f08d9d90a7e4ed752b5ed4d980d5c9cd93d1d975921fdb98ed900764cd54f681d05051d07f645dfff0f48ac53634de495" },
                { "zh-TW", "ff0fdcdb30b1e244794185198a4c2c7e6c3afd9fdbb043a96c9239d60a7a697445672cdf4edaa8dd4e43ba3ff0f95f340ea54d24f700733d78c3788d433020b7" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/109.0b1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "3087ea4f81d5891039e3b57f9957c8dd8daacd25fc53a881215e60cd369c4823866da38023637b97695f2c207dbbf18abb8520715edad6e4dbc360c7b6614250" },
                { "af", "84f9207597115be5cd2fccf2c7a746824c831fdc3bc7dc2fe23ca1fc339889cba044df54ebd767b9c4129d8a2eb42b654a3a8d6b2efb063dae6579c390055329" },
                { "an", "5f6d4f021f51795dcddef92b120d7d89b4c34214a889993277d92343375a63cae07df9bd2ebeefc8c4d00ec8d5ba82bbd1ae2ca30031aba8d734792c11518bb1" },
                { "ar", "c444d4624c7b0edca2daaa70fac3d0fae9b020fc34a639ecfe598f2830a31d10055bb6d74d6cf44cee20b4755f21864044bc384775ca784d48e8b67fc883e780" },
                { "ast", "af0d03ec1aca1d0b523083f321585ecb7cc8d435d77a5a46eef0de39775772cdcc18cc5327c64b28281029487406c4a8fdc8c0cdfc6f348e8cd1d1ef716bb150" },
                { "az", "f69f24f2caef25ebda878588e34acc2cf9727e5c31b99b5f9c46d035af2e50ed2beb7c62ddbfd6127d09fab057369213eaddc25eb3f183e7467dee86e625ad3a" },
                { "be", "872c1276463a8e733094aee5dd900b4ec5d85ede16f9d07aa3b02bf7b11a80a12042b127e6332987a5d4088e147d171fa92da2aeff8f2bbb5b4cf81fcb6c599f" },
                { "bg", "4f6eb22ae61dd20cc56eaeb35825c930a43e6d476d7c515f26d752d2fec364918b7502db11534a3454adfb5f458fc0ab2d60edf341d89405bd8f7578c7ae421f" },
                { "bn", "e63d4e1f1dce0aeff9efd4d11bb949b3237d90dbcbe9d947331645c38828adfae7622ae77da2af67948e8964d4fc5716559d6e49ac0f25df1b9d2884c9348854" },
                { "br", "1f4aa3791525a7de69bd0c0c50c18269d9a150ea0a132c59e92aa0218ff7fbc11a599e6dfe70d1c70ff5f94f82f67d590073d5ec2346f3423933a9a1c686994e" },
                { "bs", "acceaa0a43a5c94578c407a0ffc12628cdaed762c63321cab728cd14ca9bedb0f94c857a8eb8b9f5bda6aefdb25ea89f4a5328cd4f0eeb5fc09d3c4ffb175f34" },
                { "ca", "c67e80dc9075a6d81b3a7bfc3af542f18299ac5c22f9f807da058befb6acb615ae5e15856d5f9fae6f3b7f9ea8961427d23092df6cda09b2baac396ab6f7dcc0" },
                { "cak", "abe35d14675c1a04fe47c1896ebae26ee1e08d71684eea7f2637b0cabd95809c76af4ee7eece31921245f3fe861445fc8ba5a5317dc4bd15f515545e632c0ccc" },
                { "cs", "cf07b259d6ce6cd05b24d82f98bfc3f407d69f4321ae10e9432a3693a0e49adef72667d390f50232c085d867597c7f8e86fd21d9ab83fa787a2f7b3d8570151e" },
                { "cy", "83bb26db78753e09f32bf8e8557209dea041fbd24671f63d62191f2633c371ec7b6f59e4dbc234ad3eb92b5cb03ad736d854c1ae76575f131b8ade81a1f86366" },
                { "da", "4332e3f686508f6efd994a2ea9a31ab9b3d09e11a08d7baede279e91f056a4af948e1b785935cbbf86e1cf8bfb9e34c5d53e62f40587e876feb126c1d40817f5" },
                { "de", "306e0b8bf7b31433575d4efc78213d5dd36658ee2572f7dac447353e05b782b88e0210176e02f583f85aa905c024a20b9c9ed82bda98fb49000b087e486390d2" },
                { "dsb", "386671f16aa0d71c365bef3a973e60b3902caafacc4197c277f0bdee8399b3170cd56a44a4dc940b33c5a61d96735d957411a3236efb0bc4e71a657e6f363e99" },
                { "el", "7b02d900d3238169a27305761d962d734becf0526d8232185f6a946ac138fb5e246ff3a516bbebb42f9cd77085bbf5edce94db46b160404d85f5341cccd1f2f8" },
                { "en-CA", "02251ad2513b131fb072b9aa04b369a11baef29411f6d08d8cf41e8f93d3a464b9c6c9c2376a9c4f6d99966594390793969b1e4e9e5897e25078251e618b7dc2" },
                { "en-GB", "9749032997f1136ce97213ec52ceb91a55a1d79c38563a0119e99bd5173f4e0f024955a8d2be199fd069dd3285f8960e38ac77587ec3a12e6b74c42bb5362a31" },
                { "en-US", "6b07aaa53a99df1acca61c4168e202b44c48b83ed4d1753e0a1ec8f86a85b9a08c43f9db57daee5f82aa46a59b6c8c9330c26bec635690ac825f6a23b5080a80" },
                { "eo", "dc4bf4a3b10317dc35396a1719f322b34f39c88565a0bebd016d0cc7b3323d2ae1127e8c98400177e512f3dbef71e459e97dbc6564664f0f9005f916400769de" },
                { "es-AR", "675a8e3dd5ab490f0c932781dacecc1849e042827e662e1dcd3f1297a345ce55e56216a6b596781b0c7a844c23151e4b69c030c1c44946ca81c52c7bab330cf7" },
                { "es-CL", "1050b4ee9d02ee82e57736151ac44294d5470d3ed69ba3cab1a85c2e89786b398453b9e3bd88a94544077320644ba57a32f0ca9f5f1a4b98af105f6b0efccc8e" },
                { "es-ES", "af451c04877caf6ee7fde263c7e02a526eb065b882a0b11308e9dc552b2b0023fa7740f887aae4a6e3d8f59f25c949380d46960bc69586ee42f2e1ee1ff30f18" },
                { "es-MX", "f00de44cbc5668d3ffa03e7e64650743e8deb90e1040664deae06e5c6dd7497def41b73542c65fda28af673c16666c351d914a7fe2e2385f61e61249e63abead" },
                { "et", "39df7b8cadbb4e499efdcc0a325673b53577141ee07d66f340f992e77ace88ae01bb6ac5f7077f7b6a4a984477d780bbffc95fa692d833c922608e414cb09b36" },
                { "eu", "0a19e6155e12ee66ab99234db034565e407f5fbcf10e5a4f9dcd19823c86946e360f7ef1e2fd40c55400e307878292c6e5a7695b21a4d96d0a6630926ee737de" },
                { "fa", "7bc7eb238a1ab0e7f593b94c687ef696a3a2520dca7b7a0e137153b61d2e65be20d97c966f138879b0d6cca524eff0a64666c18a5a98c46c7fe1e82ac1ba3fb3" },
                { "ff", "87f84145ae8752fb1f921724594f7f6cb4e1c9f25943230d860e9406e9457a939bf011a6ae1ddcb3cea4e71aa626fd767c2c38f62ab207a1f396f0d3edb3379e" },
                { "fi", "947c5904046bced3f1475102c7991391a2e2d2495df372994de9f7832f2501313b982804adcc2f370a74bc9296caecdeffd6d9005302606a0115b0cf28bd779a" },
                { "fr", "5013edfd1c6ba3002f367489141a10dd47ef78927b098c01ef5ceed8929796ee16f88f74f12e064a4c374b3ff2509c437c56effa22d7173743f8f7a285dc9083" },
                { "fy-NL", "f717da0ee4be5c16cbe5784b6808c2384a0ce6c89bfac6b4f3aa18c0822d33897db450b001393d8e0cf0b230c0f8ad50a39b0e15810d376e4b97e8ceb82a2552" },
                { "ga-IE", "364e5c29f72295ba59ce6f63d600f1a386036b7843995f7006566b34b2e2b85a16641c57256f7a9c766c95d4ca00277b8b1de30573e3194bf9736fb1382182c4" },
                { "gd", "570f6a5052266938ce425947f60f9606b98ac9f29b259f8d1aaeb6b8c17561b71691a97f8d44e26a6788cbf805c919845961a77c91afb9782e2fc242856b372e" },
                { "gl", "e56da7bc4e5b868b6312675f2c1354f019dab34a099cfbb9ecaa44bab763457472b1527d0d78425eaecd966c9b466d31cf783fa303362b6e79c2102ba1e88b1a" },
                { "gn", "ca2a855ee8f22998b55c479d6524ede16cc2a052a7b8b1cb5d8ef2b821b5fb2ac19a63c1f6e17c601463c6efb40a69d4d07dc6714f79e3b579506d0c24ebb106" },
                { "gu-IN", "beb637bf92a80dd61ea06365463edc84d681708a5cf7e41e2da575c4eb8c797c39621fa67a3bfcb5720c88d9dbea8a906a40de3e66105565ab5995d3dbfc306c" },
                { "he", "899505a90d9fca3eef87ea415826f67d7fd3c126264e9b461ac4077969735a2552a7ac1ffe1bdbd5eacb113c5738fe8a4f07cd79daa819c4d8e896cf094d1a37" },
                { "hi-IN", "39df837d5371e84f54b27b26865200e2efc5ad6f1add5d0e4d275ee13b248c032e5c3af7f7ad45c47d82911bf5f6674c9d240e6d87008db6ee6e251c15513d2e" },
                { "hr", "01985bd7abb5c56b17020e72f6b84b4b8239c89f70378ec3840d1486d2507381ab548dabd30c8a14a82ff733a335806e6ebbdbae94b87358f79e6fb587bf9b7e" },
                { "hsb", "9aff66c89ee08d0c0e3f63e2c94084a3b4a316bb01e5c8e0c70ec7053c76e1f25ee08b940decd4d0df6710d67ecb42368b812967929da55f680e78f7f6195f39" },
                { "hu", "18b7bee0c306c924b5665bfd0cbe3410a98175f99a9b11e77bbf0a8eecb860d3fd792b3bd57dccdc3fe7b84653cd1f2187de62dcd9751e69152d036bb177e82d" },
                { "hy-AM", "6ab36f2dfc47d8fffff80bfe6d990c834dfc81265ce1ef0ec829fe1c62d410a11452153fcf42acfa9e7afd622e4f3a1a8942d831b3a23828ce4d6539723c1741" },
                { "ia", "3a47a06382558d8332fb7d7f8ad5b2e3e91f6af9811e64485040f3eab7e4bbf677085ba415a244554c3f111fd674d9b1132d64a8b9b80605caa42023018cda36" },
                { "id", "25f7f381f8d8d3a530b3fc037760b7860e857c0824702f3d1e6efa977124aff41359adc4934a8640f92447adc6d3b2db879b6e94c8e9a781c19655b81a297bb6" },
                { "is", "8720907343aec97c82fde9ff392fb2bbe33f4cb4f385e4649991e4b229f141518a5b8d1bd16d281c7f1d6c8c1baccde8794651b340cfa38f51d5ea68392f0c74" },
                { "it", "a4c15e28ff38fadba4a634a7df0e8dd742a5e8e363081246c4c0281b6078e98add306d4efb0a1491c741f0c676ed33402ef29b5b491801fbd71c53e551e56958" },
                { "ja", "04031a9e2bf043f5faef5da05bba14cb3a2c50284f8796c66efcd64a9c1a1a01976762073671c2ecb0187a35f7d888273f8797d4623fa662927dbff15e9245ae" },
                { "ka", "2da31a9a90eb67fdd82ffc913bd1cf75c3f4d02384907eed905d2897c23abf721ad6577c6198b9ef751c1968500fc4879d2b85ea63eddf24cb3cfec5ed1117c9" },
                { "kab", "40710c95a4ccd71f65e237eb0077bf0097560830c614b622c64bd7a334ba5b4f7cc2cf68be144176aed520de5b7054580acb52c31b0895322b2b1e1b6339550d" },
                { "kk", "5247dff7d6bce7a2b8aab530363546435a5e6128609a84f0e5e1f2132236ce1d0c8a53902d35f58af73e23504d60a7d0f216a60111d0acac9e23825ff2898abb" },
                { "km", "def0fee15f4ddcf164c8f97816a3b91867b79536add410f3a2f48dd3477841961732a1404ed6bceadbc6a10420972ab8a730eea12d628b0f450e022bfbbf19a1" },
                { "kn", "bd4fb9e2446d0212388b2ed88950208cb1caac256689f5ffaf52368f665e962239d64e93622a3ff5f6dfa7e0d32783a095e2956d0472540d556801e130b85be1" },
                { "ko", "a3fadd94729fe3c90a912ba16da6499e3384d8b5200c32e3ab90d2179103c676549d7110f3f438e5c5cd309895e5dc9f3cbfecb36ac3f063f6d1c583048792d7" },
                { "lij", "f235dd8e7690788108734aab7430c56da9ebe3a76ccfebbfe7b360a9493d3afe841d10af21a2da297d21faba10e2f55c81b83528ddf589c93a7b44fc46520214" },
                { "lt", "b98b4d2423344dd2070b171764535533bd10309bf68d6a40a42d9eb82b1ca37ab3e8bf31f072e90d601743757ef19de8623f6a8d5284feb6b0becd7a95f60746" },
                { "lv", "9d98b4dfccead249701bbe5d58dd1f16873b5c096dae91069cc7fcb485c4826ac79e31070b30bbdacb1184866017e563cddd8e62e47eaffa2a877305ab406484" },
                { "mk", "db1b3896a702bff13f0aa5951774bdd8ce3c1c1407c5a74a4b716310ea1814a59af2c01d20c8f8b40ce64fbee0e660f67c2665a4d6e32b4f7f70a45c1d7112af" },
                { "mr", "b091d778791bb1193bcc987a9a635bfe7a94579cdb59b5ab1ec479a578fb48acdb21a273b2f79e0252ea274e3963d9e2971ec0355f0fe2c98aaa0c754d8f4c0f" },
                { "ms", "dd76da90a0efab214e0408835946c33df6f069ad58deddd6a31681fda99cab7a69b83b582bfc86693487d1d2ca63d7db214c8e8bf5bed1e1a17b8737a4236eb6" },
                { "my", "9cf4df6b992f0cc2205479808f7fc7314246a113735294c40c0d89363b8543ae3ba049ca9f8f34b6586626cc8450d824e7083ab1bc34371c9c18893d52ca1d30" },
                { "nb-NO", "da3085e9c449374d2cdec5311c9b7d884de8158594dfe5f9b6869f6e319695453510ab636f59911b3039794658d49ddc0c45ff0ff7f98a3d09831daeea79baa9" },
                { "ne-NP", "6c096126322276c8598783f96800a804329ca4d6e376405f01fe0988fa36c220c7fc420add623359ca97e3da2f7c7b2b054203cd5bef6a10c75ee74b48e85856" },
                { "nl", "b91585779ff88717bba7750577052ec343d5970b72e0f0e87c0107593b3bb2250ae7652eb720dcf2382fcb8f10b136e29f7521f609dc6ee365e99757f42d482e" },
                { "nn-NO", "7ec024d9673af535bdc1c0dc9e3c62a7b8b7b3378413f1da01c20a3ffeb3cfd51412334ac1f96c586e562f1a9105672a4f469913cc3b744ae8a508be5d886ef6" },
                { "oc", "952d50858b47cc077b4d327a04d88004097b03c3acb64e03431df58ad34c0c7531508a09afa6bb56234ed20b7e69620746a772d752be04708533c2ecebc76b7f" },
                { "pa-IN", "41e05f05fa9e01b2779d9861aea8da31b8acea84ac1a449b978c56557e33d4425c00d5be2e9daef312e34d81361004dc3f6da3cb06d9c75e255ac414f6d8a45a" },
                { "pl", "34b2ac73a33f4c61cfeeb69cb472f5be3b91b1536e42175712f243b709e62b46edd52a3c760e455a1a350cf155773d3e4fba93c0ad6710ce5d1bc7c3071518f5" },
                { "pt-BR", "e264301aa05ade01e2b5569eb9a8b588001f9f94d6b8ffff89a151a97b78df367cef8d120283a9542c8c777fe9b8ab0860ad9b498e17b05147c12c5efd96a273" },
                { "pt-PT", "9ed84d810abf2d5431b9459b2a2149e229294342298189101e0b7d4b437532db17f6637a3f27d391c131ed4b50edebe8c464ae1cab83edacf086ab61318ff1e0" },
                { "rm", "a39457cb79a98d33d73428d90746379959e067ffcbfd8b4cc95ff1a171251b7a3337dd030c107d364ae80d354b05fd0adf1fb682ffea89d9e779a83168785b0b" },
                { "ro", "82e074d53a6a5c90b1ac40255f50f4f695c318d043826e73d495258e1db9074bf2657f036644d27a85e8f2e0627c3811a8174d390d7422f9353c00c02209e874" },
                { "ru", "db26943ab2444eae1eb1b71a0c59602a9966e58229aaceeb0af896fb4195a526135324abd7c307b1987f6df6db7171c30fd31d54f20e4b746575446cce0a4217" },
                { "sco", "2ff209c1448e09922889b3f83c8cd115afd4ea13d35e89bf7bc6eb1b31e709538f89e35814ea9bc3ab294d6c30e376395e9f56e48dab1638b3e1315d5e9adc39" },
                { "si", "6ec941b1bef76e323ddabc8470aafae6daaf7a6c01b024577c6e8af159d47b3454d29d64544e026dfffa8d68010d87f9f8a4818f60b928c334e4496f98d44829" },
                { "sk", "76c4c6c7c43c1b6eed6cd8d465b7f91c556cfd80927c8ccda24f891f8d5827dbc7ac0a2e5668c88940381eddb714bfc8b88e092c57fb3f2d3d10dc94dbdee0ce" },
                { "sl", "9e4a0c8b0133a6cc891cc10d2d50bf6f1302832c28947ff69fe1a7cf7069917953386a7226248f2dfd4110edd89be5c8daefa547c49b63c62d57ecd056d27872" },
                { "son", "4bf9f6b3dfc2d6d555de435d90542d7180c2ed6d500a8b368217aab2adc65eb262a19e4946020a72bd070151e1a1d7a79915bcda5eec88861d0585933a49fced" },
                { "sq", "6c9b7055c79ab3c4cfca0935c27daca02d1c47bce8d3b479469f89826956c468a32a66fb7702d4eff2841ac090d6f7ff774da40403c79e34d75d20b3612595dc" },
                { "sr", "20d5ce99e78c6bb15581afce03069179b24c058ea34d35665c661350272e95b15c6652d543b12a751e8e3a9091926333684caf8016a028fc29209ea83ebbb519" },
                { "sv-SE", "c39a4d2314a6facc7d36d85b511ac5baac6fce70d7ce33822564d534af9728f07d9a064031dfc03267901e9319fadb195c96f6bbf6e368d2187ff63629e568ae" },
                { "szl", "3ded948c6f1b55721203330357368ce5bfc4cf737ab0c629d363d90ef7bb1bbbc380444b962818cce5b0f9af3b2ec7766c6debb9504591cc877d358e91db1f75" },
                { "ta", "61db7c52eb6838e6fdd0dd3bcceaa2d5e60786d86e62223d66805d4ebaf52b69712cfd0074d4ba041e5bb562e1cdbc0e35f8125507c1a95f52610069344f0582" },
                { "te", "81c3955cea949e1c31c9ede85536e82f9e9a78b367c43a87538812b66a027a9ad69c78ebfcb8fba78ae6e4f1d1c2307a990cc073e1883d9bd7c6430b5c5cc60b" },
                { "th", "dd20cc28ddbeab939a2e8e474efd16603d67a7b9e8f75671ccab88a7179cbd0ff28642f280fc16c6d2def28daf875933d09989889e93ca6d6ee82d134bc3dc5f" },
                { "tl", "beee1ef7806771c77a75e33fa2039ccd4c16e70d2532cf1d51aa97fd7f8d4b6479809995066222bffa6a70f8feec447412e96e3095e9e8a8fdcb2af27113e7f6" },
                { "tr", "674f714a9d0a1eacb15e03aa22a3ac56d5799fd93eef118db929c164774d387f09037aa9eb0e8a61fc2363f050976b0b012d33766f42ad8b9a133ee862c11fbe" },
                { "trs", "15ed4ef28cc568bd8698394e5e086ea0360ad6868c1fa78be9b4ac2368cca9b6e035d288324a40d36aba67713fb2d13ae18a4aa8a50141752ca380d51d48085b" },
                { "uk", "4c253d6754c95b47b5d8794b8e012460edab62987ff5f6951615b814d4c0d49ca01999ebc77979f0a01301da76fb1d72cf81bccbc9451ad06b5a015427af1fd9" },
                { "ur", "75ad751354a503ddeb9897cc1e08b38f4c80eb4ac95a6e593b2d9289b5dccedb3b8809f7172b43ded93e13319a9ea93a47c7e857b9753328e7838cada56cc43f" },
                { "uz", "e751fae19cca266feca26e4569e1b4d580e4bdb54535be670d6cc2fe84fbe94b70a85891e76a37114759fd7cae60c0657eb748c2cd24e3a5f2e11bf8f96393ed" },
                { "vi", "0dac3bd6946ce6893c98f73a24f9025190a5c5a9d84acfe5075224f6239720cc1f0020d3e7fddb7530138b916ca1e40f0b522856a604b54317428459b467871e" },
                { "xh", "2286a752d5427ac6b69ae2e4b0e81d0a194f16e4ebb8e9a18cd26f7fde44cb88a374ae06ef2111a5e78d0dcecd590387e8fd1e51d8568d3b24388789353c0ef5" },
                { "zh-CN", "80820831fdeacc1848cda8f61389a8884ad55107f45d8dc2c83b249c5fdaf6c163db1335158e8ef56b6b61fbb0f56980eb6578118459950043fd76b390606459" },
                { "zh-TW", "6fb77a8faa8fb5afab0ecb78bf57671b47a2e5b19bfdf0c24ddb6df2b2320e6afeb50a4d7c690336b6fb7e292052be0a2438b9f8ec9dad1627741cd6d66d7171" }
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
