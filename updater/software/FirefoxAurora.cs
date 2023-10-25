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
        private const string currentVersion = "120.0b2";

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
            // https://ftp.mozilla.org/pub/devedition/releases/120.0b2/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "d5d44f77be959e3a3fcfc89c093164e8440aa191cedeee4decb3e621763ef51cee570ccab0f0ebb30ce6c18e1972bc5705b0d5c0d9712e547214154faf31b04b" },
                { "af", "d106b86a6cee5bc11dd5af8c05e5634195331460330c149cc5cdb0ecb2cd0d636c5072d9135b17a4d49cd2283294473c0b3ed4ccee31ea608b492aedd14d35e6" },
                { "an", "864fcbc1ec69fefb456509a6e709b5d9d9e0d66794f1b5a98f9919fbe0b8350bdab5ee76fc64c7bea0fb46694a40e8b819522dd538c30777738a5e3d16ae2147" },
                { "ar", "06badba082acf0cc66a778516481745a025fed1017acc8d7d7fb5dbba82db6e4fbf771518c77fc9bcebfd3af100ac5caeeaeb459c4b7433159925f3015fd765f" },
                { "ast", "ab7148a67c7ca9a6871d6177d6cf657a421d893ce49664928993e99bfc34244f22e46e992e0d5c5dad0d59c0015bea2e805baecdd6e2688ced1748b7fa3f83b2" },
                { "az", "315b93f4613b9cd14f4adff40922e59d57179061543bc35c978fbb18880247674e816503798d7a520efcdaa885426a34c3fc4c89223d10eae6388f8fec77ef86" },
                { "be", "89bbfeb35a13a06c4246f958c5e58c5bff79d5a0d899915e8e5c72abfe14cffe2e726a009c21f044f1158367a283ab0eae5f51ea755b015877af2d454339d84b" },
                { "bg", "2a07a2970ebda2436ddd227fd2b92e40c4f61ad23463223dd35d1fc92e4572d2f78c572565046d423c325d9255a0d44b0cb6f4bcf74cb76c40187e8b61871faa" },
                { "bn", "a356a45be01d377233d0c36fa4bdcf7dcb9841db4c38d22b5446be4b25d886bbe247f7de14409b6fc576a0174166b66a2b2f0fefedd7fafc1508253938660b52" },
                { "br", "8de61364016b8eeeaa1f53fefd1bdd215dee4fbc884a88535b2b8e3b09c8ee66cd5f4c3016b4498098283d9564e854fad300acb4cf5510494cf05cbebcc6324b" },
                { "bs", "3ce7d737714d75a87bf6428a563daf797d7c9800ff865360f6e4e68a625873eb47dc6408ab4326ae1f515f8fecd942f0642f23a347ea2e7e163aa13dd31c38b3" },
                { "ca", "a95eb4bee9dcc761d9e132f817bf0a88c33537582ab778799304dab1cbc348e41f49e9d87167f054d676b1c521ecfe102267dfd7b202f00f28d8ecae8ccb0917" },
                { "cak", "0557fd10f1e894fbcd9bdf1ce12f4e69a54e0a173f9278c29203cb0dfb072b86a6c75522509233d7bcd79124e7ad1184fc51b257523b11b4d1c0fe52374ea46a" },
                { "cs", "ea4208ad0efd368c8bce087a91dcd231fc0f627b79725056063504ddb44c435281eb22d6d60e51b1748457de75d35a0d2241a99a35a383ece86c9eb355462428" },
                { "cy", "76eb0075a0f97f8b325eb11bfce2c78b6dd3b07d1dae0aab8d573a357b63d6cc0866cc7e742ab0c69f3150147d041e104af00adc7a7326137b925db683983008" },
                { "da", "8576bd2414219ed555845b91c939622e9166035234551b3556dd1527213350c989621e35ec850b90629e23c4483b1424f440dfe17afafee62532b84cad656f19" },
                { "de", "d569cde9682fe1f9765b99294ddc61c3a7702d6d470f339064ae35891ac9fe6e3ff9ce1b1c0c9010e2fd62d38de14e2af04790553196ce2c2074080bc15b8784" },
                { "dsb", "8fde4f4107c7f9da2fbff549a6263fff45592e7d43b50362d261347a93683b6078e3732a52c4095897a5619d423bfd618028e8a636986c5cad0ccb8899142cbb" },
                { "el", "95d45d6166c352fe5760e46917333bcd624129c24052f2d9dd24d6a75a0ee679df8abbbae3f6613e1b963a9c30a97a9e3e0748a31c79bd4ed40c93230a6470e3" },
                { "en-CA", "85e4f25679b8f2a342296528c07d6e98feba68e267ca20d8afeb8b76fcbe34a753998592bb7664261c3a499a493742736326850720b65d261be344a97627a97a" },
                { "en-GB", "887d48dc84b20b620fb20debee323cfc7693c7475c71ef3d77f34d5515fe5b29cb4ec035ee26705f09deb9335f1b459cfe27ea4cf86a5f2fba681709aad1ca9f" },
                { "en-US", "55420f2e7f0adeae255faac120dc252b605f362920d0604cff963ab1c4e3d8dd576ae84836a5f3a85212b05fe472e0cc2cc51e32e200e38c1b54a1cff89068fd" },
                { "eo", "0724e3c6a445032d91be7e60df3fe9d79d43bfb43e59dc2441a61589746a48472e94dd4d2418ddd1e81fae91f32b8d72946fec63e6da1cb1f2dd733ae637c8c5" },
                { "es-AR", "e30aa6eb7b045de19ecbdff5767f4b6857f6e1598b4dfa262a2bfab84428695f988b62d345075c3b9d9a20f449fb05f6a809d04203d3f96efac12c27e6c8f2f2" },
                { "es-CL", "6a7954dcdebc1f5f0b6240e2514186aaaabcf5a2b6779abed20b3dfa394983c0669b4d532e324ecd5b25fb2f1c5fb8c43703b2d322eda5e25ff73c5831fc5529" },
                { "es-ES", "d99c0c88e38062a43af940cfc30ae7bb6f932936e297fada8b62dcf499dfb2a71a2e640a4ecd6dd62365c0909649422685deaedb07120da3e58913e320998a15" },
                { "es-MX", "886fcdd83e5f00414339d14c64ce41af32689c3e400e02be9fee74555fe52054dbf7dd49967fc65caef846b5b3b1c15ed639d07bcdec94ebc71e9f04929cafde" },
                { "et", "8357d28464ffbadb6b804c9e72926468d6f1c214a81b9848b8e7b6612a45584ce2c3ee6661cd0c4107069eea27bbf17f51fad95f266eb96b41bb09f0ff111577" },
                { "eu", "d9278f9ee54788fd4b919505f8ffdb990466406d0667359b07ff5307c864cbc834abe2e1a113ed6d3bcc07927f27a18327e64b9a97c2b56617893d210dfa39fa" },
                { "fa", "a208aec804b529f2fab75cc79b7dd1410680db7cf00c8a5ceac2a292e77d314aa4a381015683b3543df00afc8d53495d69136db7b5b680ff9ce49ac3aa311776" },
                { "ff", "45d8201ad48829b0621c29abfb0063c044711da35b49fd347aa974e4d0b8cbb09ded04a41c7ab8383090b191d00c1469bfdf74db33b54b27b490310fdf7a5b23" },
                { "fi", "192f1778a9cc7e859f6403bdc94f76ab26f0d77898d21331afc585d6f4d06fc9a2f732e57dcea027734d237061a387b486b43be6ffae5f2cbe47ecc67e67f72a" },
                { "fr", "3aea6561c3c667909e9300d53e4a5a2425259664fa235ba45500d2befd7a01e979804a2de2e613ffd970eb6d2806d68fd50e6416fd9320476a1fecc4d7b314c6" },
                { "fur", "a602174631756c00d412084059545b650f19b0abff932bda958c253beb0f359aa56fb4a3462a435b799e6594eb0f632a912682e4f405036dd6de376272f2f68e" },
                { "fy-NL", "891a5cccf14ae607015f8df7afd4cbbce0ff10b7a128cbe4f28e1afd8d795eb7b55887c4dfcb20f9c08058ac9ce45ec9b9bdf1c50746c7f603f3d1a4fe144849" },
                { "ga-IE", "b3d39f54770cd53c47c006be489a46e2d854bf45cc3dfa24b89562711f6307edadab386c26d7231ebb7c07f0d5c33b06564044ab23c98297e969da645a47232a" },
                { "gd", "146329e167919bbb5c50dc8ec1f83d1cd63d70ef7be6363ddcf55ec571d6dc83a971a90412b0de3c0590ab9348559fef191871317b7ad1d535d211eb42ce9e65" },
                { "gl", "69aabacd1bdc4a055e17697c5848dfd58f08b0bee9499bd9d22cdb25263226a2649053ffc044cfe2217316d54e3206dd0fa77fa8d2de1c28f3de8244b11d1140" },
                { "gn", "8b898d77ad1a003bb28d738c5c242b99a6049be7c1aec35150499c03ebbb7a7d3a5d82792a69c14fd591006641c1a0798a09f544b4c12daeed2cca80f8cc73b3" },
                { "gu-IN", "9c78c805f52fed6a5177c8a0fa9735025940b0379b16f32212632b843cc50e39032a3698de0539da8a891e8770b9952566a4257044608fd8a05c9b4959673d80" },
                { "he", "d23abf76c79bc898c91fa5bb455bc57a48101f781db7fa2febd6b130acf71477adf61cde556ac1b645d7153fd263f48f9dc8488be309304f48d0b7c5b7034ed8" },
                { "hi-IN", "688d9f98c0789bb8dd4b821ac460b2c3972e8b526bc5b011cb87410a22cc78be1d94e28fa582a2a290a6d00495f65c030ad7e9f4b121070578ac58127a73d391" },
                { "hr", "1bb70234114ba74e1989c57a8da6b9856ed4a6067271c82d299283b8d8d0cb03f8cb4e53e8d211dafa23e0e2209b860ddea794728159ad5f2052cfb1f9f91862" },
                { "hsb", "64abd810c50298304e1a0e99b33432f411eebeb32f0906ac1554e9cb3204eb129845927a9eb938e2e9a13d666b632e3a609c9ddc84ea8c821583a75a1044661e" },
                { "hu", "1c6c9fc5755ffc12f66780ffdc780be0e0ea308b199351de4f004fba66aa8f1e67774ac2ecd7c6ee52cee80a021c9be95e95a2b41da6e7c4eb2743f752f94b81" },
                { "hy-AM", "cf9a1482659a650c244837c792a55130420d64dfc26be1c85bca44991eceb3a016c88e63ffa110a343dfcf2bf562ab893f267afc1fc2461dab6c39910bfdd6fd" },
                { "ia", "3292d964d91b7aad2255b322d77f02e5665081295f7b54d63f2b8a7df887f22cc42de0e55909489154e4763d5dac78367407d4c378d8c6bc84220b82143735c6" },
                { "id", "817bdd4a5060c9a506622150296bde5b6cfcfdcfb4f22c79597f36c6ee0df4e829f20d7981cca6980e2ff156258edbbb0205bf34e2a4ab7c40be2093cc8ba9fc" },
                { "is", "07c2536fa94913ae4b42b7ab9751c14fdd33979977d46fc626c4dd8d0083e43b7633f4cd0a45cba9c8af210fb0f480384b4006e6fdf9dbfe43a9b92c37c60fe6" },
                { "it", "3c8eba22dfcb7519e887fb7217630c9e03e62a4de12f4f80096bdbff6ee8b1d62a292e34352c83e72783fc91fb9cc35d73cce82586a90258210ba84d0c2d2262" },
                { "ja", "62dc9ecc1489b4f454c16d0f50d1db58238c2dc5c8dbff8fbf206139acc3f9834e6cf7e034d3c1dec2c893af80da10ff18de56d8c59f72614ba44949ddd6cacb" },
                { "ka", "8a443b2940f3e18d26b30170b909604ee67e708fb1ff60c93a8797f0c99852d2d838a951922a4f582408105bd58e8dc38eab86cd0b3a7bfffb2d32a51c2f0fb3" },
                { "kab", "7dd607db923881dc9ca47cb175e4e7472168ae36ae69e9f07d5088b8bc608d49d3b637ebcbf1d450c3eb41cdbd5c6d465d88cd9f51a2596433f726a6d0bf6460" },
                { "kk", "e5c4ef8749c84ee2170e4b126032fff5debf629f1d32f52e0fde87815989f2fd991c3744f52e15ca6ab563e002162bae7187c953bac2a01aca3c15ab0a808a9f" },
                { "km", "562c7a20e61406261b6a3bf50db5bb0feccfac0d41dff4a3382a1bc8aa443a067e5c934ed39a1e3b31d80f882803c76dfad43d6f57737882a9b8a53cdc43d7f5" },
                { "kn", "294434d98e4075e0b00fb9aa10fd8e4e31d85aa70280df5fc2ec1ff129dc23067f3c4f721c3d2f42aa5be43b6943a54bd5810d155b289da1904c843fa1fff590" },
                { "ko", "3cabcd5fa780f24a6ff90f11434bbcf32a362d1c7a7d0c534533fe195a00f7357ae96e6321b12a28b46ecb3ffae3af992a6b3d78d32b56218369972648406a08" },
                { "lij", "fbbf642d3ff1319b33cb4d728dfc2b20e8a26862ee96ce09ef65699bc70d513d119f15b9c646f532be7aa3eac7b7f1bd2c98f63de635d0a455f09c26dfa10da6" },
                { "lt", "3d5d61ecebe418b564ffe76b80fb5051e60ec5134eb3467149c600547d2882a6d5ea782e167f7da286351f70829f0cf078f758fd3b77a586e7769c93c3b29b48" },
                { "lv", "8c3e4bd535364fb396a2177a6d43180c8b641ea04d94132dc1156a5525a5be3482e0267887afb4f62b48acf816d543c2bf8018a2228ec75317770b050ccdfa25" },
                { "mk", "e354382ddd05a19781d115a12e9518078076378af3b1775184c28e57b01ce55ec797f2d7cf77e9df11d74a5fe343e83bce5bad82a44d5883bad170c9926e20ad" },
                { "mr", "d7032e5644fa4ba4fa7465992347a671c8e192c3521c43001eab4ba393bd3b1a11042c20c4c31b5f42a6d4b1da7b5bd50cefaa75c7a94dd0fdb5016a32312a52" },
                { "ms", "9db2325a148e3d97df962a03638be7eab7eac45973df4836c63d1bb79fc41f15b8f42316db04b4642ee183fd791e803428afe11c5e9d4160003b873a65be56f0" },
                { "my", "b812c8c9280ec6a68e034ec3b7bf56c59330f58b714b5af051ad12062d8a376ab45753056ff7548267b54139ad1a8e3e056137ee200572e32fd38bd013f98d45" },
                { "nb-NO", "6550580060ce655cb38b4c1de007c460106c6eb7a929ba774a7af3c6020945c7af47e345932d414fea9db7046a2b5bd39ef673e6457c2ff4db7ead1c583cd34e" },
                { "ne-NP", "0df9e21a78c53795e28df1416ac87eeed699fbdb6dc1826c9cf9cb9967bc99d81e311f704cba395baefa95e27118820baf91e8b1a85635a43cebca975cf9f4b0" },
                { "nl", "999ec6b87a5a0fd4a9f26c91c4949c606d07b096ca9f90ccce73815d9b025c39b9f598c7d72d92ac003f7239d1b3e30030b2b44cdb45dc0accd547e3be60e646" },
                { "nn-NO", "90154aedd425e4a3d4264d18b93202528d18ff565b9b0e7a5816cf704af34816458c98cd134198d5cfb3ebfae9dff32014fd7293a37d40894c494514b979bb4c" },
                { "oc", "4671a58beb4c4867091a2d670d0e9765a56b7b50f4b05ffb7ed6364f008fccfbcc4276a8c4f2aab8ceccf1ec5fd6d2b562e58ffebcf7752a5281875814e3fd2a" },
                { "pa-IN", "8aa1a70875379f6dafe4f01527172c94501c4c9c07e55319c218071a961317d4561f171fbdeeae62881d8c7f8d662fff27f9c25e6183c7f700acca24c7b12426" },
                { "pl", "372ce520772312baf1b19b8488e104a3eaf3ae5f6089e12fa8228d24d458e88c80d09ea651c0763b81df1a22604fc85e5a2cd3541a98d2088ddfad9ddb324490" },
                { "pt-BR", "e8f002c4c1149304798f0aaeeece90281cb398a25e5026544e3783ce592402762c5ebfbf4b54e11dbb41b3d856f239f64c9b84e933c6d233b811047af7459737" },
                { "pt-PT", "f583887ac186850bc87bc3a34304750abbb822a542ad6351ef27894d25b59000433bf8d8469021ea39ae9a4a587d1e4df65173fbd842320d3e9cbe844777871d" },
                { "rm", "3f125db43c311ef30345d9d6f6ad3681c83af92aabbb795aa307c012da7b24be1a720c16a719c203da6514a21d43c0c49e8729f3d449571b2f2a350373f28874" },
                { "ro", "fba347858b6bf2037ca7d29df77d04cd19c285b6124405de40672dba5d2b0795466447afef8254e44c39cd310326dab18b11aad38220786a70362cf9701a7914" },
                { "ru", "ba0e11b50a0b4049032a977f5b93a73cf56ba02660a5cb423cf49ff720b9aa09cd3e8bc3377a642435ddc94867e950e49115a01285ecd43d6819806059f404e8" },
                { "sat", "a5683155dd22cc32318dbd77468d9173f8761321d4f6b490de56e052e748185d1b7cb1ce86236911a73e3f33c55debe4faf6116dd5edcf446ff7a0ed2f9c2802" },
                { "sc", "4c963a0519b4cfd39210f21daf8a709cee3ec5e7573bb160ef8d5d59dcfc6ca410f157b7221a26b3ef2007639f2b7fd6198315fce486e9b043cd29ec40fba3f6" },
                { "sco", "31bffec9317049fe74796e6dab982e2284ad9af2c0f2aa1079a9eaeb6d8ca5f459c455862d5f608b5b707719007636fe8c5201ea7c994338348b97b7132234c8" },
                { "si", "e5d4f90aac0a8459ef49dc71982c5f816622833c291b974b214d3af6b6292e3112e5069af200b2787bb578605a842c0a822ade2d4970d9daf2ab5bc953b2d9b1" },
                { "sk", "00736b35253a58085fa72c1552d0098e13fdb11a704d8cff732b0aec0ddd3b2c6bca891d11f50f4ee5a0cfc193ebd52dce823e95ee22b8238a93f1766bd48334" },
                { "sl", "47c64af1f755ec0e85829dab7ea9b7a4839db19766766cbc478c38b9e566c1c1cb2b1fd22b8a456fac10d462fe5fbac4162da9109f57f57bab77862c2ee477b4" },
                { "son", "b54ae97af628e7e5fc12bcc3e314b5c41f508700e022ee9a4a9be39863e492a560eacc1c818c5a615644ca0f5b8866b194a4fc2230ba90878a40907aa7247e91" },
                { "sq", "75bc1e371be8a4ad9ac5134ed39e368d21a06f599b974f33c6e8143fac54a12fbc3be33da741add987ee3c396e32afdc688ebbccbabe3e40c22f271394453f68" },
                { "sr", "43537a9c25457a73dcb3433d151c4be0e7dba0136ba838f4b7bf9475a9d58333b09073fcb8f67393d3ff2d5003fd5c29c328b29d0dc6321ca0b4899e8bb3bd3d" },
                { "sv-SE", "77ec3894de22b53c6d566126855913296728efd722dffed5e2b6d95a4e6448d3c23d2e65f070b9ea5e31493fe537178110d347534905862e45bcb33966019e2a" },
                { "szl", "859c944bc5a1dd8036c4a031e81a9ddb5373152e6132bd5236f465854eb1b9eaca31fa467480a1510d51c82b7ffb163134251ea6f4f07a5e6869106533b18a82" },
                { "ta", "87e586693911a82661df23928802c80e8d57ddb2afa883d375d598c2075b21dc224e721c194608296a74491e9df89f7cc8743d41c7deb9edf0d3be8bf2b7491c" },
                { "te", "539a244f99d053e1684dad92751963c5584c4a2c1f6991c32e4c1c24d3e26a765c347986aba0ab2dd903c5dc5ef39484ab6987cec4f1f7d0e3216e97db07fa1f" },
                { "tg", "5d627f13ec7dbf6d59e76e47e4ebff9306b5fcc49c78309ce44f4d731e489b100e4e512a2757defa8e64a59cb4d1a89e70929919089a1d3f1826ac223d484db8" },
                { "th", "2843110a051cdbef3e295fa4716d934c34a795985d629f1ce22ec449e5aff7ae48155942d39f198ed50dc97e8496cc9ddede58a9110d966431c590394d940762" },
                { "tl", "3256fb4279a1b4f83e134dd992a061ff5699869de15875ad456a0e9d3a1957d06f39d9318c023e65155053b8914aed1f9794d3e604760efffd911fe352d9a0ec" },
                { "tr", "7dfdf642c1519586949246637fa7703e2437c52233df5bd2cd2f90758950d0837c5abc6c098c8ab3e51a295ae52e2d18c1aa40d197642e3ef02bda53a4cd52a1" },
                { "trs", "380cba6071fe47c2369fd936da65b6e9d37ef43104744f302d6e7dd54efff008a065222014c662fb4b123d63302364cd7719116eab9a5c59ce281d0b0388b57d" },
                { "uk", "2e6d9d65cfe5f254320aa0b0934e49ca047c1080dac73bbd586724331b10e1068191adb4f8aa247617a6240abc6f73175e7b7d2afb9a4a748da38cbb0b001c0a" },
                { "ur", "bd8e03e9a29e7f886cc5c477309f304393adcabe2066afb9018b6001d66217f7cc976546f3cc14623a858edb3de25489053712448c1ba476f4bf963c42a90e2d" },
                { "uz", "48ba3dcac3a315d69d179de62bf306a929ad30c2924d581f07855e647c9f879f83eaa4502b262ee3e70b4e6e2a4ba735a729a3cffe319396412b81218e4b0750" },
                { "vi", "39b696200b65e743191af4f395d88da224ec0753069e51ac07ccb3e44dd44e95edb46fdcb7dcec0904ac96582330cd70b1eeb10d56056957a02f732dd523b065" },
                { "xh", "b6eee5b8a9c1ceff9c8f60a267b472b84acea128da9d3f75c233c27246554d66dbe3e27a88837093cec70b1b755fe8a1a99f2ec7ce536320de103a7fe14e8ac4" },
                { "zh-CN", "b177551482dabb6e1b46c7a2b34cf6a4652513f934b9721339afd2e41e78508e9810d49ac63e3f054660251388f328fda18023946796991b96f86b876f7d99bd" },
                { "zh-TW", "744d641f0b0229b05e802559e627415979b7ad1e46e87b9ba595bde3aac489397b8d6e129dc515e35bc365da85480dc782c1be793f61cfcd5673b411a312c96a" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/120.0b2/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "d4fb621ae79b5482b65a0aa40de4000e180f70f4062b3ea1b9233339e1b6fab85631dc50488cf5007be858d2e1815b2b38da0bb98ab5c2aac4fb7dfb4584ecf9" },
                { "af", "f6e6431ce225ee6ab6e6dc97c02c01650612865bb68585cb1b01ae04f178ea31e24446ff835ebd9d9430a78e41547600353d42ffb409b8d2a79c525adb1adf7c" },
                { "an", "2a671dc8f3951fe08a98a89acbb1519e5c72cd0e2d296a1938e9d9fd3a269313aeb515b98e5c21d92ca7d339415d1e660a5d66d34879dcdae552f0f5f66e081f" },
                { "ar", "b5040973744ab1e3329fc3eccfb0164c513aab8ade6a24bc7068326c4a5025edfcaba091ff873ab0b79827557e722f4585ffcfc2391e10cc08fdfb871cd1326c" },
                { "ast", "97a2bb05329687ba44d2063a98f668db99f38051b1bcbb886d080d425b477ee0c58468b7d4a8d23200aaaabde8262c7ba73362f945b63bf4c9d14d7222fb96c8" },
                { "az", "5a81bbfb232153593ce9cc74276fa54f7b12648a0c69bd138ef6a3e6bc11334ff52bba4b8cb580b129236582eb2a728433874a81f836330f73dd2e7dd83296f0" },
                { "be", "b05d38b7b3f8bf97f7d8423ee40640d3646ae4d0d46e9bd6b935a6ed42857720aab228d5dd7bd4dc552ad97a4148f12b97079a45977f375ed3fe4c72b0dc2083" },
                { "bg", "c3acd2b36d1d477f7931ef56cc4c5c0c5e7e790c4b7ac9936d2150096d365679021d7cbc9cdca64c8ed89b8b792c14f359c8da8777de56fbd35368ec7d56254c" },
                { "bn", "d08f92dad8dc94047b8f3b45e9b23f2876094b43d007ea418dffcc4f1ee8489f1381f10098f8473df2236324614f0a786087b75eec40dcd335460b6e7eec56c2" },
                { "br", "065e9640855a89b3b4bd5f8229d780d42ee17b5b43355c8fa8b44811c7eb68fe05914083cbc4c40e5fa79fcf00d0ff0db32330916de840e0d1e74f46da1c20bf" },
                { "bs", "8d5b8be1e4dbb5458e8a3ed5d3f86d891048e321518a56bc7d41270a957e4daf37f3855561d69cf266c129504c9a0bd9fc5d48899075f8ca22ec3e52b1c8a3bc" },
                { "ca", "b1381890d224ef1b92b921b8eca38b4113a2e68e96f5a9bd8798a35bf468c0fd1037e2efe76d61471efe0905fcb3dbbfd33d0ad9d8000e83aa08eb952331dc9e" },
                { "cak", "7ca792f9552a809c902999498bbb9d7510131cc453d18f64df97c291c6b300b0bd6b30ec12b8770383ca1a06938d3c8087344db065c7eda29eeb29386b460ae0" },
                { "cs", "972d512654fa7a2bbbf7e4f56c60c98c1ee8fa4a554b1da49eea956d60b94b4367dacd9f6965ba4253f27a782751c4d6841a33a0a08781a6e58d44550815c626" },
                { "cy", "2ad7fd516cbbfe0f46371a27f54d11eeac84f171dbc92e9fbf16d91cfa5314f1757bc30207ad121df80dcb8dce43c61c9cacc7250ce5a57a1222bd8578cb874c" },
                { "da", "7a233769e633a7923005593340b07b6e857041b60c9684810b16a3a1bdcbee4e489597748413e49ea7f064f0953dfcade349ca096cdeb636103fff70e02ffbe4" },
                { "de", "8eb1531feef68390dbc007d4bb674e3c26b4ef13ec2f165500658d9cf59e0bb001c7a6ce1cabc1100ba61208b5cbdbba0d726e0dffc924f2b981dbf59d2b73fb" },
                { "dsb", "a7e71641007b516fdd28a98396bf46c795e8cd2cdd74776a49294fa5d40990fc9a4778735b816c4841834a78b921264ae0524ec59b55d911460384202f7adfb8" },
                { "el", "536e214609be8c092d19181813780c684c49a0b16338e70dcc5088012638d2418a959878381a1ba31b6abbffb7fec6a811d46cc5cd9eb008d244075a3ced4341" },
                { "en-CA", "c8619616b4773d6497ebb3c58a9c3b137f16c065bebb50892889179f163ecdec33b283bf7211d8da5544279fadaaa9d36679091c71a85770ed5a6400db3af41e" },
                { "en-GB", "0aa97c4e6fffaec682727b52b5a4af0d66d738ca18e28d24e3d318525ccc6d5a0697708bc2a65eef7d94f3415f998d21a228486fd1610e8ea3bd5a831ca5ff5c" },
                { "en-US", "9f0a964ab82e9c1f9d837492d1e9c67fdaab2ab720ffeafe135262cc485ede517689e44313002fb5083d8c4c1fd7f5901ee4b8d8890c10850a14ddfbb399eaae" },
                { "eo", "cd1b3a9ede3c00cea6654dfe08074ca5f4097be3da2b88234fa48530a1d1457a7ca352cad40c3583f08fdd6eda5802976345b69a87893211d9235c4bc83303a3" },
                { "es-AR", "1712525265457ece86b9cf661e8a4bdd87fcb650e258c88ef58a0b75579ff889d2aaedd606dc2a18c2343b52477cd22fd3e2e99a2d2ec9e03b84fe4a24bfeb2c" },
                { "es-CL", "d4e681516e8c5bfb0b2e299ae66fa3bd8cf054e3b7527396028ce00b645aae5db6aa26d928fcfa774d6d8d4166f9007e16fef8db8307b549ca5409cfcc160417" },
                { "es-ES", "30b4b72a90ec4b0bda4830851bfdc0b0fd1061d32462ecdedd37f03515c514b64ef7d33af0cbfeae60ae2c7cc0b5875a7e0ad0e511de03e81c668a605e55d6da" },
                { "es-MX", "5e6aab4e0478d257a28c1e9ee33af932207b794eca5cb1b23fa2e0f3f4683ecedee4673104065d6083524590f1c9a07c6e2ab82263b70206fa3b6be3de0d7abf" },
                { "et", "4fe2a0b8844c1630e3c1773ad400a935e5bd351009e703548fd32b9d845102e2632dcbaf5d91396eaf98b87b83beeff29a0b28b57b30dbe778ca582105edeb70" },
                { "eu", "de6b0d6af77cc1122441e5e44463fbb882d86dbb931c517cdca611baa115ecac8c329b76d7b17f24f9d6f833a68e16b1c1a3add4ba40fdc21427d39bd72728ff" },
                { "fa", "f06c72b8f33316f819ef8448e30225a6820e14fb6ecb83f090a16650af26dfb599b85932a8d9ef5a9915034a69eefc64299e78093be953c031413d56556e3f9e" },
                { "ff", "2b8735eea44daa661457fea5035d815182cda771c7554066113d8b33a20b01b719ca82dcf38b386cc4cd81d74484c8e89415eb5f243e45e7e0cea3494abb5e3c" },
                { "fi", "218464aa919ddf4101f808ffa07230a6a3c47862b1bab8d834740ec8cf225af5dd388eef4e6379d8ab00a80111ff9bb475e6224dd976e0a5be44b3df4c266616" },
                { "fr", "4a18d6e7859657765b40a140e9c279313aa3c03605f0c66836609a363bbb84d72673e462d5d103bbaf3372440f177da6be9c9a9d3dbaf114548c877d5ad4eec4" },
                { "fur", "78f54e87075b43fad91decc5d067e8e3f77debe3465774699135fcbf60d181932b3ba5c9dd2cc774a565e6d9ad2bd117ba3e2ba320879174abe44e6651eb3ac2" },
                { "fy-NL", "20016faa7f8d77d788394ec1e076b68088af7e03c121f0a4c0c0029cfcea31e122c8fc3c5a1e5ac85360de96014ca72608f0b6f29f7d1fa78e75e75937ef653c" },
                { "ga-IE", "62d2d59071837b2287e8aa643cef1dc708d6742ab111092efbd876c06a7858c34e9bbd68cc325c30a4d3225853d24c66d8bf2509b98c57b0ce9eacd2e0bf01ff" },
                { "gd", "81df16d54fdbafdb286094be67542e0c8b83512cbb23c26602db86f4507c0b40538ca25d959e55ec6e9105236f1a7274c3fa6c39405f5b1d21f0b6a84cad35d5" },
                { "gl", "dafb2c92b0103688740f813012db025b3869a5e5585328f1efb26f00cb15c48172834c403fa1869199b4c61c987de1c5f86f4d53c162e3413b3533fe7b4f0fcc" },
                { "gn", "2ef6b9e32122a8df007037ea9b92a51696968bd43eff37f6fb3caf0fd6e7551965962d1f4514f2574c2ead146a3fd59e0236c6194082a5fb73466f6e4b31db2f" },
                { "gu-IN", "fcf4f295e77de8267997d2d7621161652076fa1d5fb7da7375f632a5fc850907e4027937167ca589c5a959d83ceecc1b05b90ff2e7be2eb386d5edb2aab58f36" },
                { "he", "bb2a27e1fe7b0b377bdf037ab622a92824d1f1f60f9c244e0eb1369825302b36839e415f4b7e99ed1ec6a16298079766b940b9245cff2fc2c121eef0289b5451" },
                { "hi-IN", "ab120fba7bffcec186dce40d65d0b1be3635e0f006b8da4d1e8bf842eed12e8a1294f9137445ebca4560d4137aa71e4840a1ffc3fd49ae413c336e044916c388" },
                { "hr", "cb1bf54238a0c94797bcff85b4e965db228bc3fb3313d16bee705b4f862b15d30f01f977ff0e3767a5dd513ae85e55e3bce4e91efb550179cbe68538430eb8e2" },
                { "hsb", "d3116d69d6b877c9a2f65af18898593cbd08ee358794b5dd4282daaf439b26485c9021c2f97d50f8bd3edbe38ee0800b2de6e0bb90399c48b53f02f6132a760a" },
                { "hu", "a26a776da2a09ffd044c38a9e3a1891b7b38e325e005081d5653a8268ad3db793c034be43b48185616df9198e974be99ec7f4438f84112964da8850e54b2eed9" },
                { "hy-AM", "7c29aa190e9c5f79886c87909361fc297fdc9831169bd9f727387f3331b8b7251ca938d4d6639e5314377a75f7fe22b4960919308eca9179b3a5ed040b71afb5" },
                { "ia", "e21c892ab2e681bc747cd4fe5e22c78485e9a313f5c1251d0a8ede38822fd68faed33bb7b2c3abec82367ab34d86d8016d2e93a9b0e5a96f3c6980b784fcd64a" },
                { "id", "7c4f095496c5de296e3fe8d1c76223ecc0b42a7248a1e321576ee2f48ca39bc077ab9250fb6363496ed50a3d3c114d5ebc4facd72827d15edabcedbeeae349b7" },
                { "is", "0b349efe8147eb929d245f08da7697eacaaaf9fc7e2ced5cd464efd8e1c62413b66cb1683c6159c0353554ebaadce289b7fc7506917221c404e257ee2c364110" },
                { "it", "f55a75bd8de8099082064c7a08c51afcfe8c7850d62f49aa7d3bdd8a4e5fc834a574cc33e0ca92e50ce9b31d5f31d587149d50cee7001e8431149825d65335bc" },
                { "ja", "7dbd1398c4e6b6f22a0747898084883cad688f1e52e8c24fc366185105b6148b23f75f5515af591b054c7ed4b1029da6d8f3980f72c64747fb68159e940f8dd5" },
                { "ka", "c253ec9f460c10888f1a3d18a9aba8a0cce234d6901406db407d5f7a0ccb1f79f5f5b23b4fb38dca1dfa621d5c56d2bdf9c003977ded2ccb7160dc83ea15a794" },
                { "kab", "f3f369888ea79c3763503e1c1538e43c2d61baf61759b358b9194ec28b07ba68eda51e6c19ec66a85c9eb4a04e2fcacef51be75d02b81755b6f467af582e1a91" },
                { "kk", "b88a403f8491245c7a44dc7f1c30696e2b9d10962e7f7c284f1e394bf2c27d57cf42f13011aed3e18b0fa19120dcfe2242cf0b39fb62fe40500a748463e12766" },
                { "km", "6fb56f3de4690e15745883d29d2a6f47f4d270afde824f7c3a89d62bf8c21858839de8179890952cf7696b5bf071fb0ce5425758e496a378d23098fd678ddbb1" },
                { "kn", "c7b4f81d0ad721f41885084fc825b7cf946853c2143f31eeea1894dbf1cf8a47bb53e7fc5ef8aa05d598c5963267aa81968da749ba8bb0163e7d403eb360f177" },
                { "ko", "3259d73463033ffa67fb371ece66c4d9aa0746849dc0172255f91e931a0f627819d570318b6feb67f564da757cd0abd4d4333b7f6a97ac35f3dc9feee98c8a97" },
                { "lij", "1081596c13295659a7205eae271f8d6e4b5d5d7e5d4e71f4272cded245927b44b2275404111b4cab10d05e9b8199d39634420c7695e85269bc6a2b933db8862c" },
                { "lt", "11d9e4ef74c4db95d48aede20a8d4e40ffc06dcab2c6b5faf6c8ed1bf8af9bbca1ad032aed325a6d725b0ae2a2f79a232c5997e0c270da547c4cecbddd5a2a4f" },
                { "lv", "1e221d892be921556e59f7276f7b49a2956734b2189f37d66a35c7f98642fc78c98f114960ead2190e735a58b4e96336060c36a32c2508b69f544ecdf7bb3216" },
                { "mk", "4b2adaee8f7756171e132840f89c9cfcc92e989401af8a24636f08d8e646a5dd20d6460a812fc44a10a3ead2bd50cdb432aaad32a6a470e8be17310c9ec1a010" },
                { "mr", "31b59fdc9bcfedb50a0617a32de83f4b6695a93dd2d9c1a6f3fce2709dec5e36b8b8f847469e42ed2a07294cff6a8ce29ca4d8b3b4916f6b5a30fde51e46ecd9" },
                { "ms", "e24d310789ee80c582f89476852afd7f988031d731b120648b304b284b559d279cdfdb9df2b8479dffa5206239fae032a81ee6c51fddbf2d3ffc9eb12fcab559" },
                { "my", "8e48def7aca0d9d33876b00736b3207058acdbcc45efd07e18bef6dd50dc7f80a1c4e6bda4c517439dd7d6e772805f2dc210d4fd9365ebb6514b7daf27d3c966" },
                { "nb-NO", "32e1704e6994fcc3b59abc5144cfb9fc7c893a3ceeee346725b18b62ddd3a6014238a1a2142742dd786cda48d33ab7ee54564ed0be7ced0b10721c4697597622" },
                { "ne-NP", "2fb959d7ec1548ae71447ab6aabe98ace96ee9f99cda56542e7c799b98cb9dc1c1a1817cac117acd959a965021453a4f6e236e02bd7c9ca8e3aaac0463656373" },
                { "nl", "8d263ac4ba7bba350953b16e3c9aec913950fca6496f80135659a07f6fd27f441104ccf4859f4199d5524e302bda925296a923a9814819122f0024c6330d0fcd" },
                { "nn-NO", "007142f2fe8ead3840a49cafb055ca1cd5efe5a90477894bf6ed57e405795695a57dfcab704adda8e3dc34642e8a30b9782af1e0684fbddb889fb570ecc3c024" },
                { "oc", "f51ac6627ec3c37aeee39fec6d6657fc268eb472c24cb0230670f762204e05911ed76ccb6855a7f3029c726be43f1abdafc6a0db7db656bfac80918f581fbf12" },
                { "pa-IN", "d5443e2cc9cc31426015554ebd11b5dceac4b0f6cc81535fe2049484ba2c5ee0c5451fe2974e154d52c4bbec56ab5d361e17a56d1ff18a8e04bd4bae2249ecff" },
                { "pl", "d67807d008c9548725382b2bc970dc5709f735978b0e9292ab2e0dadcf404a530ba5a49a300ced02c275b2f5fa6a79d708ccb32fea01320b4bc899e2f3868998" },
                { "pt-BR", "4933a421f702c296f27756109526c32cee4473a983a71db2eb8597972b33feef08cf635ab16c2eebefb14b32fde6aa93b232208f77d22ace1056374cf2d61b21" },
                { "pt-PT", "3f8ea262b875449235e5d1d30e5aed51d4b8e9fa66559afd14c2faa0de7fafb81b257efe17955cebc810aa0fe73b50a322aed17429e39bed7ef6319791f018fd" },
                { "rm", "2daf9027295f6b5a41e556f90cbb975f691001a538c7180fe7ba8bc9f2b4ff54ed12327624fc0d7fab567921ee64202a4ac44380503bbc99757048adde1fe237" },
                { "ro", "bbf1b608cc6fb222adade60d160780879eaf403e655443f8e84132128a304b17b87dbf7b50386230994efa7319c69f0e78bc6cb38afc3c8de7571ef84135ba70" },
                { "ru", "b3adacab284349fb28c3810ba7edf2db2f7f4fc11716863c3ad50b46ef31cd2c229662bbebb9aa8f56b23a8d5714dbecdf868c69e6e10ac1aa9f99214e75e153" },
                { "sat", "bca170bbf671eebc566fe5da7182ad017b9a7ea62376304736c5326c00b1b2a722dcbfc552eb8b7ebfc7318a4d4b8ffeaac756920e6c23455da12288eb24fabc" },
                { "sc", "91485fb28cc4fc3958313b27d4eb52f23551bba8343d8b7bbcb1b3746f1c99f0edcc4641d21aeabdec71f7ff5f931eb8adbb1301a7cd8e5ad59a58543a55f436" },
                { "sco", "dfa6091f1bdf652a8b854b8dcff36631045021d603bbc605b96dc4c1614782d0b7b6e44bbd478ecdfc3be38ca8a762ce7952ba5f0a31517500cdf4e3024d5d13" },
                { "si", "c3cb48c694974a74ac728e2d66364d02ff3d9e794c89d99e951ce5bed644e735281ecd1ba0b344e794e06287459dd6403789e15485a0c8de912ef807788607e7" },
                { "sk", "2bdac10f13de4b4d8232f11eb0b5d45af6b752724ace56b08132571b7a8f332602de665fd4e44fdf8a63b0c5e8f66fcd54e2c0290891b5ba4262fc9cfc4b8501" },
                { "sl", "118827e418aafcae3a29f04a352e817507f02154d7e87a88cb1bb5134b812b6b1b994f9a216d683eeec1a14acefa2dcc2fe675e1492009409d0c0e299ba06cdd" },
                { "son", "0210bcc1149c991e3f28a034f007ae96cedf712ceaf35115b18b54d1c220a71fc1337803a6c27ab96ca63beb528c558df97bbb59740a4b4c855bbd2d5ee5252e" },
                { "sq", "cb2ce852eb278cc1a86b31a1c6f8161a90421be7af56c2a91919b4323af69d854aebc729e285b5d58edf97706c732515444a5fe6cbac692c9160e660b582d55d" },
                { "sr", "6ac705c6e40be2a4ffd3efc1eddf039c8a63e751610f08a0f9d885dcd4e767957ddf19f6db4fe69839d524bb1bbe62e709b8bc41cc9a344ee3a482511a9c82a7" },
                { "sv-SE", "82feaf94049361fc22b488fca52564e6e885435237f08b7ed690f8fc6b450172a6de2abd6e9896134cad4d4d1e80335dfd501924298425db182f9368c65ebb70" },
                { "szl", "2e8e953158a0b45b91b55956d27b2a77e8498fcc795b48004db8c01b5567cacd023b7521c71d6e6e16a7a7f216c7e045a3d4e831186502e73e136b500fcfbac4" },
                { "ta", "7af840d9c778916b30cafcb96a0fc5fe834ded8908b502389830f84cd808a23328d6ad97a41b895ccf57f771dc2162ae76df7602eb59f394f1575e9f056ec615" },
                { "te", "aad6f253023e483b66ae86d7650abcdae13ddc2387b03431b630f10d532d9faaf39aa591a401c33a813b687af3fa2b0e89662f6155f52513beabc37fbf51d881" },
                { "tg", "d1afd2fda68604763a32426396813fb1cd48873067030d9bc0e5da95a92c612afa11c0def33529ca871ffcc0357bfe8c264d65e39071f89e06b73340ea4f3a1b" },
                { "th", "9608d4ed04eca50c7c7d4aab7fe897658642deba40d5e128dc29eb8368fde25b7348c7a173766e4c34d1f95d707fb796873cf99947ff52a74653e5b654b204d7" },
                { "tl", "5c745888a672c5819f1b99d59735d3389319624d529179f5557b3b116f230f4b67577a633e7e78ea07a4bea9e14e2059bd9fa983a34f64a7ae50cf2ccce8077f" },
                { "tr", "d48da33b39668a5315086af499ee708822ba97d222f0f559686e552fb4eb17ee7ec5e2932aa256914d78d7361327c6034350cee4794de7c88fdf5658af20460f" },
                { "trs", "7f8e596520757214a8f19167c334e452ad34cf56b66e549cfb0a8e4c17a6fdf660a3b5f4203e3dc0620c19f04eea57bfcf52e08ad59b34ca3885a851346d5a3e" },
                { "uk", "c0695e83531de0b49bfab55e0a63e4d857a32bb9ac5789d0a8ffeb91065c15f8c337397495e34ce8a968096cd201fea787da4d1fbd641fcdbaaa416ecf30089f" },
                { "ur", "41586b70472cd19173caf9c6243780b57d4bc3c7d2d401a2a6745f8cff9c1b06ddb95e94c57388bb776d490f1ab604401e29a6c10056aade4c81269e27d68ac8" },
                { "uz", "29bfc35eb38c8c1461f1d663cc3ff6961debb5e26e14a089d12f2f49ba4503081c6b7eca79789cee86bf074497d4a463dc798f6501b4c229d0d84c356176a65a" },
                { "vi", "961acd8dc3227df56b71f2472381b61eaa42ad9c1a56e2afac40c008ec8d1cbfd6107ca65dbfc24c6dc0a2cdf6d65e269601735f33c7b7504b2a9206b81a7ee8" },
                { "xh", "d0a0636f359f60b6dcdb5bc64e0739ff9578957ec6a9ca4be6185299927846ef1574cccaf5ef7191ccb303584e752dc3107c8c54b065b16cdc67c4df93ddc1f0" },
                { "zh-CN", "0e2608c4057ce17285c6b55172eea1597d60b05f50697ee4520561a3af5a7365130609785fceac7ae09553e4f2b7eaa1c5bd43e3c0bf4a4f6235ed9309fdc6d7" },
                { "zh-TW", "b02818f0dba84351663249060764f02a91f149b9cdba06251f5884a48f6b15f31903145e19ccc9da6757569332f100a210b1539226814a13c334d59a01d706ee" }
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
        public static string determineNewestVersion()
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
