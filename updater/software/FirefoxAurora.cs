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
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "128.0b4";

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
            // https://ftp.mozilla.org/pub/devedition/releases/128.0b4/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "5a2b045f638f2543585df8738c35722b76ba3603c91fabbc5bcc6e388a4e04c77995138a3e3358235877cc8026fe726498ad44bd9df0b06036202ada43cbdec4" },
                { "af", "9c3acfe3d5bd96409d4c05b33b9885110045feb09674b5e8ef1e24272e32c8917d14593ef7c9b55a33fad006aa405cac9234a7fae9c4d4936d12c54f77e07cfd" },
                { "an", "ef01759e12ebc0c16028b45b5156b354c3eafcc523782d9b71db68ad8c66e15d5e5c8f9af08451d881f04dcb39ccad65ea56dcbdb6d9cc41ceb2fb4a24003a4b" },
                { "ar", "d00748710bbc09ed9065e08dc590390c0607183856a56c5cdb712f5400143192a51e0a2b51c707c0df114b7733c650043cccb3cd7c887a58fbdec958ccde74b3" },
                { "ast", "4ce9b052eab5b781ee9d0f1cb0fa542e8ed6ac9948230ba0c3de0a53c498f6e07a4595a0a1cb897dfaeba5100bcd39b7fd4632589724ad583384eb4c26fa8afa" },
                { "az", "60e8279275ae9fbe3d2adcb2529040dfce8eb171c9ff04fbf3caf9b3bdf277d531d34eb12be85a006014872dd76e72cb8da92b5cadbb2a70cd7f4cba99878a68" },
                { "be", "fffc299d246b7095f3992a80ed0074daeafe4c5079989a371c0dc4ec6c0416cd705d9ad1c58856f1adbaa1a398b1103274baae8a01e892a9cb985b53a54e7d40" },
                { "bg", "fb61b54e853247fad4b22e6de5470748da8c5e7326d0ab79ef36d60ec54796c964519770e34d1b742182478682b7f5964a274027e371c5afa28bd1bc3db42871" },
                { "bn", "bf24a543b9e33b74a5b50322cfe07e952d9bf19bdf055b45675b5fcf1e4c0abe94953f9adad80c44c2f09db87b2a140aad5281d703f234de23cf24d5e08df804" },
                { "br", "abddb316eea236952f8c8ab2988adb56f3196400a102d58a170230d5071c6bbf6299e5ead90779e97aa0383e5b4918501550c0f43847ce69946a8b5b0d624e50" },
                { "bs", "de963684ab3f937635a217d7e71a2b6739590e39ee75bc5cf5496595cd378f48d1947280bfb4774646cafdd8f8a79e97c45d4d4aede0c1d9bd0d3fb218a848c6" },
                { "ca", "e02fa9921e0044842e0e12168891749eb5b81b32aaa3884bb41b463c288fcc018e6611d1ad7ec2685cf0ddd4cc73aa3053f68ccd539500d3009919e05485fccb" },
                { "cak", "1d46e55cfeb466e0bc1855e855766e17705d3b3c4479c337274131d846261746c62b12c0d5effd702b0c827fa49143806ea752323bd3922b010d6ed1eb64a043" },
                { "cs", "7fd5afc103ab89c1c1c7075f365fe24bab391898e1e4f18853fbd085a2e06999c13a82339d8bc6d36786116e16a2e19008e986ef827400c2beb3cf63adbfc429" },
                { "cy", "16dc486d989809b240055ef04206a11f87adcfe74834efdf1f9d144f2f04b34ab06ca2058c631d9d5824cbf1c8a5068edebac53f407fafe91a970be545b22b85" },
                { "da", "b648d3df6cc85019cc6a97a43dc95c35f2d88a497f9f29d8d21e000586b7492b34a4045ae8cb611a85ac9a50ef5aa66f94ebb32047f11571ddb1f4219e4e7e6b" },
                { "de", "7f67126e1938cab05151637a909ad9ddaa976d85185f2e54e93479efee2cb5f72fe43e9313be9e12da44ee05c8cd9cdf0edd482ee9cc568f897f8efa05cabc6f" },
                { "dsb", "53f43ba58e1230795bbf8d83eeb41f8855873f9b94c30ed191d29b9448e28d72c15e56cdcdcdde10d0826aeb70d1ee6403aac100c4dff926545e2d617e9c0280" },
                { "el", "2953dc9f5634dd1f1f65f4e4f64dd5b040d8dd8509dcec710e54631f924039438b18f7cceddc69529b8eabca8b3d9faa929741e97c77949832bd486babc850ba" },
                { "en-CA", "ca91dc0f0888e196ef5bc449fb611e886587f707c3b93fd29ee3770f6ccc6333132c09bac9de23cc02d3695963d276fc6a315e9ddde07b9845156495ae90e6d4" },
                { "en-GB", "5d36fb0766b0bbb50daa2ee65c92eb515524c5cfb0dd7540a5c5f9d1d83b50ee944d693c7f4959d4777d47ac8620e433b15d6e1c4c66a379fb1bc989b840b9ab" },
                { "en-US", "b203cf681cc711a760dd0e60e4ac00bf33639b739538aa5ecbec6bc76318520c2e42106213613fdf6e4a1915b6e20b0b72c9ba2bf29af51c4273bce160b964f0" },
                { "eo", "f2c36870b84dc605f49bb15e2f9b74e9f00989f3ffcf6a5273a6c91d684411797b162864ff56254742646dc6e6e1235f1f6a7d6cbbcf7ef4d1c598fd990db296" },
                { "es-AR", "3c818023ec48aa30820f095b916ee4c9ecd88776eac8734f0aa7f34a37da9da252078b8445d2209982c5970182fcc900cc2396042f387618c4f330005a830492" },
                { "es-CL", "ab84d4c91052679e31f9cfe8e3a6d93b9ac695c79c146754b27733729af72bc98be69c595a7a9cc22dcb276537ada8674b5c071f3af028a9561435ec38d88e06" },
                { "es-ES", "947508d17fe3e3788c466126ff017bd4b5f79bab57ed75e5035fd610fde40bcae138afa12dbb51b70814585f89f6fd54a458332cfaf673c68fe9d2dca806752b" },
                { "es-MX", "e0242644fae76260bab8435e6a5c3b62870c8d2cedbc0422e3affdbd2b10f9092e8644d35019570a1a6f5c18ecad3319b36a89e4ac0b61db589436ac665a041b" },
                { "et", "e68b28058c585d4a4f8d4d778f6add331548c4e8162ce538170617473be24f06705e9d32fde1fdbb26a1b0d05f6b71001035862bab99354475f774e7b68945d2" },
                { "eu", "dc218cbbcbe521ce79b74b5744bfc263ae95900c8fac3c28b208bafa4099892a4caec235fe93373b1f1a23b8dcaa73d8bf40c578555be0818764913c6b8babbe" },
                { "fa", "06659c6935e81dc19d511ad97c7b71f27e963ea8632a80c1835cd06de14b55a247120f14daf3da6ef3fe63db02e89e517ff63fb4ab308a395f5396a4127c703d" },
                { "ff", "c75399b22f073776842c43120c8899393e696380e291b4fce9e12575e7824276379d95591a36754a4bce63a17052297712cbdc8edc525c8280da87e0f3de2bb7" },
                { "fi", "62839cd668c454939ba2cb4fda18554cff6521ab1fbee954386238883941b74b394f185d89b07a2f969ebe682bbf76bba54d2267a5ffa3174db9714aa1bd9017" },
                { "fr", "c361d6fd06e66e375fcc298b0434f4ad97cd914a741a96ef934e39beb0fdfd49729d6b11f2550aeff53c5e8f7515482c826bb024d827887bc4275183118f00c2" },
                { "fur", "f61200d65103302b22f90e2291a1c5367fc40e43e2c02d2490487934fe8eb27753daad137ec2d3eacd391ec7c7a863144c31861e97d1054a3031d3f714fc3cc8" },
                { "fy-NL", "27e9586cfbcbf59fdf9c4da63837ffed536f5cef24df848de023694377c0d28f68fc6a94a6367c462a0debc9f522dd701160e207b10221d9aa6c2b9e7bcd4510" },
                { "ga-IE", "df22d10a6787f7df47d35d6ada5e8ef6c4503be345ab58d5ad3aeef9a62ef05ebb4ee5df4960fa32e1b2181e77fc7937127907719dbe4dd83d5fd4487d3c7158" },
                { "gd", "edcac9ee408e71c191cb8cd8d9f655166922e82bb2018e4bf1fdd7d1a1bc1f1ca84cdc7c8e2daaa8dc50c5d6a98679765eb9d045e6cd348085dce726f31f07f5" },
                { "gl", "e6c20f1c708cf9617de72251263de03739e34ae0c7041226c419d6ecb9c986a3e8c036152dc8a5fada97529361b2247d1605e65569158ffc78b105d5e62d19a9" },
                { "gn", "1d261926c76252df4ff8a478e689009cc2839c510a8da6dfc3f64cd9ef6c6e5a79cf4bfbb1298220bfe280f9a618f7046adc2baf62383bcd66f6df6f893456d1" },
                { "gu-IN", "956fbfdaa23ba45b7eccbd9e6de2f01a7beb5c9cc46b1ae2b7063cdfd8dcc7028aebd5f2ac0af2c07e3e109cee2577fced6c8c241a7fc50020a6606f0a648b28" },
                { "he", "e6a8242993075f308c231e8d4dfb08519550abea0502fbefac2eb6641c6fd06a7a848cf329f9f2f82babad81ea35a79fa767e4cf1b5211dbaa1d1038e7aceda3" },
                { "hi-IN", "7eb232362bb24f3af6045770779c5ff0bfea7072079993e2484eadda83fe3cf03002558427c732da7378945a495c13ff1b456ab947ecd0f8ce2f363d402b4eda" },
                { "hr", "23a100ca5ddf03b3a07020065ed1a44e1e6889307c6435971502f0baa54df5069cf6636cf9d400077b1a28caf13db2d5826dcfa3eba3ffb72b17f074aa4ce4ed" },
                { "hsb", "e0aca3e0f35c985531679053dfb547e653c10f814683ffa2aebf484ff594522569195e179afeead1aa715d88d28159cf2f0a9d8a25e677fe1444d780c9ab48da" },
                { "hu", "7ad88105c49c3542e4ce4526465e1d976b989abd3db05f0980336e978e8df7988f7d09d1902d45464a76e1f13f45547d61611547fb8853aaeafef8d98a0ecc06" },
                { "hy-AM", "7a12e0dc9ac29a4d8ab7f577691838c5f0a02368e8d0d9d4cf306aae4f3993746dfdf70e1071972aaa06e61458734a57cb230d00389f1cd72fa3f39b45a5bf2b" },
                { "ia", "21fc263c662e6c86e4baf7549dfd3b732b11609bb6c166821ecbf088942a92898c3fe9c777529be6c41c6391c270cd5eaaf14cc68b5b06bed95ef60eff04384a" },
                { "id", "8815a7d24970428117b8a4e15dd7226fa14247ceb0d236b26b3588758d12f15ef5d19a99f0f6f83c2913c8a429a6e46e98a1de78edd81f66084bc840614e9d18" },
                { "is", "660d9d791288844590ae1aa850e0ef23e91b823d353d619c3783662d93634897c0fec8c1db3af3913a4323e7e2421a170fda2ff3bbd7579e581f430803a63dd9" },
                { "it", "5ed415f51cf8140f2d5d0e81a34f9ad0ede08e48156dc1df85852d2814bc51da93baae47d187c350eedfeb1497cdce828cb5d9ab2cb2c45844b74a3fe41b2ee7" },
                { "ja", "cb4d8e90f054732e385dc86112d9494ca6a47dc3b41d46663626cf5e3b1bac5d3bafa9e3a3636a451b4d6b627ba1cd1ce8af7e45c187f117ce1c9d0b00dfb913" },
                { "ka", "1713a3d90de5eb965735b06496635ec1fee8ce9470aef9acf6491f0d2df2b3e4670987f8c4878c1bd4a6943f0574213cbf779ff6b74a72a664721c451a9a23bc" },
                { "kab", "7186c2a099ba295062219d332a175eb978eb6b66c5d8acf69b3da7a8e0f5ebdd88a242d831c2551af9df5ac921d004547ed651076b8a22e7dd4c36ebc2f2ce8b" },
                { "kk", "c99ed27d85b1c7db404c1e39be5abf93670ed1cba8cde3869742bb375ba11e2f6b6d07dd3740f298552ee2cef8b4dc857cfa8f1a1fd941b0fcf2316b7f2c0fa1" },
                { "km", "7df6db9c050c136571deaa485c886ccfbea954e56ffec28f05345d32a1ff343c18a6ed22fe9a450f9935325013d3b4d9a0e4fc23d463dca29314d9f3620aa7ff" },
                { "kn", "33bc9441f04d827f5f455462c2b2dc9b0e1b69e36f985086d5d0fd4fc4f4b34d06e7939aed8e5552ca733891a87953d21165c7f9dc8b0936e9e0817faa84c4a2" },
                { "ko", "271b62ea6122769b727ee79d73915c4a0a4d2bc38e8c40cf45a4a4a9616d5f7eb474cc3512e98b0920c3f2496506564f541998b2c6e8a0729f7db6af985a4554" },
                { "lij", "4dc947fac14d7a183a89f224de6321467349fa99366cfb91e2babcf9f2e28e8db4ffbbf3401988cbfea104708786ea240a98350983ca7e541b76a6dbdd6c592e" },
                { "lt", "3cbc64c870aa679fa7bdd0a6774fa798320bda90124a227697823746a20b8abe92a70cf64db40dfcb6bb06401dcf5c7143365f818e904a324c649b4ba9cc4d8e" },
                { "lv", "2ac33b367df5f1debec8026b656b4dd52627db4957aedb595154da990a0f734890f58152a650123b36e1f4c573389603489609f0621654ad64aa8d0ac18b6141" },
                { "mk", "8b317d9c2afd394f02cccae49397f7cd834fefc84d26454068af5722385546ba6472f01e479328e4830a2f34dda888a1ece244539cebbcc25c38181c900b2b49" },
                { "mr", "e6c9d6486852be7c4fdcf9d8891e807814b1723ec023b42e8483ce1f95b20cd5ce2e344ca69a052ef16107a30116f54866bf0e543c2bb0ba25d0128e5c4cc4e8" },
                { "ms", "bb167113559300a9729a11ddd4d325fccf23aa62ed66ba2155f956afb53fa8e755a048b14386b8dc2cea38f4105f29e379febea6bd600664e929208b442e71c2" },
                { "my", "9c7df81670e6cc7ebef15b4f44d1017f58e4531bb376b05149e336a63370973d1aeaf76aa29e86021423e990b20abd4a266284a6f3320ade55261636fb40c8d7" },
                { "nb-NO", "65723ea103abc399ccc276693c8f1bdfdbd32f99b5c0fae27c20d24a7b8090676cf4f4fbec7c120d24cb1c84250e0749101157bd8a068213994302c1616677c1" },
                { "ne-NP", "227abe537ffe887cd702328e3ce28346b6298831d646b3894cfd3a7706633bcf304b8e0f788512ff96624c0d38bf9bb4e9cf53335bcc979f6dcb591a26ef9094" },
                { "nl", "202d521942a053eb49b56dad9262aad932fd0de0dac55cd3c3aed2a23eeadbc1bc3222ad388a895d23b71e3c690bd233d61892a5ebb77f598428dac6b41e1393" },
                { "nn-NO", "a47ac7a3d89107fa6622326017b9bf240308a242c4bc6f86ba0a7b586f5b0156008d130a4b427e051404303d2ee884661e2e41150c1289c4a8fed6f26adad790" },
                { "oc", "57195dba16f02b8b634ee97dc9b97309e00acf8b10ababb9dd8ea6d5dd3b05460ba26036cf72aa887ba28eba4ca2bc3ca0e4cbcd561b7de66e62ca04967e5e80" },
                { "pa-IN", "fbf3a17bfc6547f4da4e067c354fe3a6cf8a72b4d81a5a12704d42ab08093d9f4d6ba75e94f26ea475ff954322e3ffd61d287490b175dbf9a52b15d433e0159e" },
                { "pl", "ccbb3d7ab7e1723dc094f8a687808f89352f8a7e5a387891d7ea6e2d5e405584dedf590f92953fb1a9d092fc7555a8757fdd13d3970c65fe67b7540e2ab1b033" },
                { "pt-BR", "d20b33aa883066648a05fac4d0a452f057ac27e028150b2c14ea12efbb9b6c72cbe6b4d8c4fc0731e299fe1111be26fa66c18ccce16dc9533d6e821fddb529f0" },
                { "pt-PT", "11c09b58dddf39302b85b4d01d072a0b57a468ac30dd8b59feb3dfb843f65f027d2bccba5b6ffa79a8117ca6069e8169e5491827acafb67fc625143881c4a778" },
                { "rm", "ad4403e9b80cdf99460256dad2e9ae5ec92119f938290246c36c563821d13e4ba2f13c009b8379cb86365118298309bf76aa4f30835494158fda73b395be73ba" },
                { "ro", "9feba1eddcd8fe8a1749a076c069b949ca128dc668c0604408240a92b75231a1edcf6b01994d8554dd151bbb95735a48ead893374828f5f560d8dad10008298f" },
                { "ru", "af286d762a624fd80f26e7b9e14efdfe04356ff4147cff7a780cad7491077225d28f6d9be004d8aef742c6c8a7096892e9eda4366943b493b340e028bca848bd" },
                { "sat", "505ce3639352c00548b890c8ec574095d767726678a0d1754885c87932f66f33fd133f029df380b053a9307e5e254118a4cdcc6e063a601d6f4f4668a4b2f421" },
                { "sc", "2bbcd5b1de7eebe21f4907ad49629cd284e9cd43836ab178c384060f0151c8e3dc4ee55163a1a24f8df4e54a1463cb8a211b64bc1f00d9aca9257750970dafc7" },
                { "sco", "e135d40ac92fab1820f187797dbfd1a2722cf1ee442830f4f4788680760a1eacecbe871a930a61833119192494cd57e39396366e70f0b0bdcc2c8ee73b25197a" },
                { "si", "c06c213cadd7b7cf6ac327b6e45d0bc9dc5c8738fded8a7c867e6ab2f5485d19518b8b553d05a1fc50a74d3694a199a76482835457e961cff596fb943eaf1dde" },
                { "sk", "876588c1d81d693292c49b17056b9e42d1b4a5b54faa5e061eabb424d7c5713b973a3966cfd4f970ad4d800c9b56c73ed34c271f9c566efa2683d40c03a0aeab" },
                { "skr", "6f3f8c4a42e8280f0e451e139ff185df3dd6a30ebcf21ab6e4b06ed9b783021eba0691d60ff7ec063b6f6c616a2d2ecf2060b178e76eaa697073d1d14491224b" },
                { "sl", "ca55b976b9c00704bc057ba2301d666485e1afcf9632ab09e00903da094a4d8ca81de2cff1cb60c9e01e96500d5b9af6a5930b685eda30300e4833caef1d1d95" },
                { "son", "effd3950130a44b0f146da6921e41cce3466015dadd7d71693e32f12bba862484e2049bb1bd253a6de34ffdb9a3fdd333e62abfca1bdf11b445aec039f5c0b60" },
                { "sq", "4b21e27189bfb6f0eb5d6ce4c2a4f908c6b00ef432d58e6c16273479f82f936e5689362be082f0c0259c4cf3527e1a3b6726b900d47bdc5c18e4fbc537422f6a" },
                { "sr", "6439b72f3c9f86b7714d61c9ddec87d91dbbfc3eb49367b6992f4a67822a681491e56b69b740818ae8a630fd70ccb77152322f1e4f7e85d7b24ac6bb44664558" },
                { "sv-SE", "26ab08df4af6742bed85a9d02294384a9d81838b22d2b02fb10fc88eb091c89ade721665d7a9cdb20b82c8474c9d8e2b5c1be71af972262c12fe17a9da53ccad" },
                { "szl", "f7bb40a7d36067342de9d171903ccdc31500f269247656d4ab701ffb0e4e8fcebabb6d17245f8f7f85fecbfbc08a51cc6dfbbe06199b8626cc36e6a26400599b" },
                { "ta", "4c3b7224c252680764e3faf793f45ba27ff0c25b8a3cb056b6dbed9388e36a2ccb03f77e163160a4795c8b72d7c8f99a33ef04e6cc60a4234d0433420a0a0e09" },
                { "te", "7f0468ee4fc7e698affc6cf747018f4bb1b9f31b54da52b6de1d5bc0b41e4b186fe13f442da522c308a33fa06e872271e046d7c511c30ee891d6080c64b8c7cb" },
                { "tg", "513f8b768a21048e0bea277a14d1f97c71d487081f30d91b07558ad57242d64ca0f4de335aa4b2b47a968401ed56568fca3b9e390aacf01edee4fe1e350b595b" },
                { "th", "9a11dbc8e993e59211e19cda81937ddead522c0ef61d45f2837473871fd4a48a1efbeff0b7b2e7ccb9698975879e237d5ea5383bfd0b2ba4de4da996afdf50cd" },
                { "tl", "2b742180fa6bf57a4a1c95f4e76a4cbcd6f4f1134ad4e6fa072408b03dc1fb65c2014ab13db28065700078f6f8559bb8b21068bf438487341ba83900afdefcd2" },
                { "tr", "b81fdd8787f2049b58808544a2c76d15cff253827eaedb5ae3c77f4706609a70e020c91cbb6b860294b220e59bca5764d71e4513448086d91e8075f50164416d" },
                { "trs", "2c146a4273c539fed3091d7d0d6130d7361fd7bf2463bb5998dc17810ed38e7f2449d028d01ff5502b3b2186f8762f4cebc610bd8b01e51981098a07f3699b75" },
                { "uk", "9fd8f6361551dcfa576f6b03a4ddb9a48bc5b7acbfa1889f8a738fd7491d8a75b43fa4339acf3585e104c5f570144cac19392cbdfaef8fe15f6a52f877de0164" },
                { "ur", "32121340e2dbc08d10d3d9c8afd75ae670594201c0aa00bd471991e2ce03b81936c8fdd63dc47b1a89935dd6fb0ad958a8cd7667b2e5200af95d2fcacb1f639f" },
                { "uz", "78d620173cd325f64b0bbc4563c3e7e3b7deea80456b1e631170a7a15bf71f990cd474b08226312eff0ef4b6e275bc332ccf40f4add5d9df9b9ba69494c6ba92" },
                { "vi", "10794446c00d23cdf90029f4e6fbb2acf0d00c571e163e0e1a9f216e887feab1ca931eb20fbd549ed6f8326dbab338b045a23eb7a4992dcd8fb6e84285242922" },
                { "xh", "e7378a136e1964e142edd2df89b9ab4c74ea8770d328835b29c4502797209227158e66170677396dc7c84b369e2d81e95cec034fdba9d303eda8a016288d4c0f" },
                { "zh-CN", "f1d20e099f57324f745e5a819439a6d045517ef55f35fd64c521e30724936fb0d692762db28535e3d4bb9ed0aa9e6aecaeda3f0efe3439aa6082a914cdc95168" },
                { "zh-TW", "3b3077da7a7a5fcee0cedfb46d53650d17d74d4dbaf8b372b1969d9efd022e528ee181de40412ce054817f6f59b754b3c3b2b0e1af8080d65a29c90106cb65bd" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/128.0b4/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "6d01fd52a3d4b7ec1a37abb9bb45bd69c223d69c5e83a07742483543bf0695fcffb1cda38e9c730cd91a9c29042758990541d3964dfdf10af7e1c1f0e0c45acb" },
                { "af", "789ef47bbf405d8896cb5424dd39ba75deb3af66082632c9448e389998184d70751c8cc7e75a194dee561a2f728b9567e90d723f1afa6295b1844dc17f9bf634" },
                { "an", "c38b016dbc25bedc61a5442d6f37f27f24b28bd08b5ffe7d12d6fca22099ebadf5ff88c5546247dc0b5b730230dc14c05dcad6b7a590a533cf1c5ebd47c28e68" },
                { "ar", "c41c510efb3628e28fa4dd63aff7a8ce5dd2bedf986a9bead4515ed1043313036d7bc4b71baeb260aa64b1f9c90eec4c7f301dd541858b74fef6a8a91a370964" },
                { "ast", "d778e7592a14bac7fed4f3377c3d74fe11d7d3b96355eb41d47d34f858633077affc64d73cc3844f8fe6fdbbd6495694266cd432b09e3c63393997afbf1499ad" },
                { "az", "54ba7a9c2bbb644bae273498987af7c6da3dd0bae83ec48aadcb270d6f6aa8dc62de3f58b82e8ffeb8a5bc455233ca9cfabdb23e8907feee94c4a5bac2bf331a" },
                { "be", "164b111bfb84758bba5493c90dbefbe0b1627a8d0bc008b0a4ebfd3b203d358d0dc844b49f644d2c3c944a6de64813f1fe311dabe8a6a2921f5c5f915d30c5f5" },
                { "bg", "cb277ab4b2c1937aec9c87780cbc42be3bf7de9ba07b6c1e820e55fb164dcf923da1c7f85394da1df1dd3cba76ff95669c4a4770b35ef1b64ed4c3fc370c9ffb" },
                { "bn", "ab3d5521baf7cfa7e701f321e17cda3663a717de01c0d042dd3c98c11d60da1639dc1780b5e445817a759573058936b497c08be3da6d867a65ca81bb80b83f9f" },
                { "br", "dd55b85cf0bf028487ee5cefa1aeb38f2980471035e4a228bed1b722ce2d2535dafe3f84f84d935d90d24665d56d9c369ae3d35d8bde78cfc889e9ac452796b7" },
                { "bs", "9fb4681cf23a6580fbd1df3279372e2fd0173eb5e36b289dea97277d9cd330143369cf18dafd69612706091551d839ec319b6efaa133fd300344c423da123243" },
                { "ca", "5d3d54648b00f6bf09fb9609709815e53b23187d9a80b6adb84009ed09ba416687c7baae61b6b3e5ef95af670a887e529a164200015b0b62c45a54e4d8165a15" },
                { "cak", "b1fa5b50f344d2db156c47a9d74c11a4472e115dfc6be446f180a4eb21f2f77ef4a21ea8b8fe456245342f2eadbc5bee5ec42308c15bae5fdaecb7109bd3e279" },
                { "cs", "ee79a7710774bbbc1e35704ee993d20f07de6da9bba99bc957db08f4c15877145aff59c38195a36598f483cc23d6e0a7e329984d5e5634c3f21a0331028dbeaf" },
                { "cy", "c24b9ea398eb82c8c4d8089c8e3b5d8dc4e7bfe63968b24df108d5dcef31509a1948d2198e7acccad1a1ef0c23f6ddfe8ee9134dd0478ae35605159fed486035" },
                { "da", "3d5e73f71d8739ff8604ef2407000416deff8c7f8bc258bf117f9a3637099521b98ab7dfe8b3b10420d1194e62f70e8294008f9985b0f1028ce665d96717e977" },
                { "de", "9ed73671c98b6cfe90b89bcc4ae9b906dba751f215ff45a2a3bbd4b7a5e1a78cc0f21fe709233cd41b7bb7a05f7a43fe119c1d6a0d779ae1804717c1c7021cff" },
                { "dsb", "01645136fe3ac5b4e2790508e9683eb4e56bfb3c713555ede818e428f3708332dfeb36054b8ed88c9229bc6c77e3449c48c6914781c8661d59982bed54ed742d" },
                { "el", "97acbccd762e8ae31934eb17d1f947284ba00c4908dbe2f10257bf9605ddcfc2bb0536a2bc9532984e718ec8ca74237b864d74cfdb388cd7a09d5836c13888bb" },
                { "en-CA", "92fa07e6b423163efb5bd71231b16940f58c492be4d604415f70d186a7dc7c484ebc9bb56c2565b4cc739620f9f1d2eeca912d5c8688743f819ab96f95d6fa22" },
                { "en-GB", "a10ebc9519f1d5885c9af035169f71314b2b9719575c2a60cc8ba493c27234ea81e3d3a648e068322383802e43e05b07c929701a4164dc672f5a8e5db8edb7db" },
                { "en-US", "525d412fb402161eb3974e06d532d9fec16ff2d6778c06bc6175271db6f1f22d46c53a6d3e638959eb59ec0692d536cd873da6ee5088d2d8f166fff639cd7bf7" },
                { "eo", "43935d332cc9e1604487dd19f4d3342edd10ad120ffde8c34ba33d89459bccf697e525358d10217e37af2ef2ac471cded0afeaaffba55636a15ad8270034ccd6" },
                { "es-AR", "3d05701927415cfb49c2b131ab3d8c6b061cff8087ce2973498be2fd7e604631868c5bded4004772e8b39631c4cd0e978b3a919df21cced64c107ec2162264de" },
                { "es-CL", "5ffd0b8692e5a8dffcb847cba7d81b7b741afa284cee0d98d7cc432b7c9beaa2edc902d0c8f513adc30bd9bfcbc302719f4a535e58d7e3aa26725461e5aa889a" },
                { "es-ES", "f1efb9b7db055548c7a79ae4a22c359851f57d6e23c28494335f098278ee5c306acb355f80eb7b39c9ff7e64ea64b8393654a604a090a1d9a25796a09b529f53" },
                { "es-MX", "e63509772f8ce29b815ccc188063b8f319df05e4c1ad7f3dcb1bbb6aedfe91bd7166b722a81dd77c3a9d3947eed427763087b0107ca8c795e4495ba0f3766378" },
                { "et", "90408933be0ed7731d842821bd7bdf46ca17a3344283411153fe6517a46cf56de36d28e7bf2df870d9f618a694b80e9e1dcfc28aca99a4d1cea39511c8da4ab1" },
                { "eu", "93d1c0cf03a23c7f57b536994ef0c96a2c0994b717ac26211899a6cc8480212fecae8330947039d69616702b689010331186fe8f30c9e88da5663376409f60ef" },
                { "fa", "f35bbb80d3d67947bc1102a8aed9999b064feae917ff9d65b18f8d7cc5481d34abc43a79d9b4dd26344a17d29dc38e71cada5da50d65374f67aa0c833f2614fa" },
                { "ff", "cbf700b771b9c0b7930b70f1dbd76abb1ed092d9b70ea97d215c1f7b9cf74a7704bac7436e62cdc59b046fdfdea5a63f416276a5b3d822e273d73496d17bfdb2" },
                { "fi", "174092e23bbd92fd674d80eca0b72730d460207b024e235122b443c13ab467582d5ec30b7827b9d198b3bcbb7b049d69b02c4b14cd6b723b3e054463349989c2" },
                { "fr", "32afb4944085812b013fc5ef750307c27bbd8abe29929ebbeba4104a803485a46482ae22418964dca96a7238d967d9609ba46d1fd3a258bea98f5c1bbb8b34eb" },
                { "fur", "290080a12b6d8e2a06301feb7f232c465ec1f8187db95c387ed61d74cafd29237b232e2c6e4e936c6a3383eddaa61fe58088f2a942864917d97fd221153d8b0d" },
                { "fy-NL", "17d33f76b227c8ea6a715e1647303d1637f7c1c6c57ce9f0f1e8fdf97756b594e0246049ce1ebb3c2d45919c37e8702fb08db233286c0d771675cfdf5511490d" },
                { "ga-IE", "51f6278bcb6ae4cf96f0ef6ef96af47da10e99507dc6a4511496930c20705a7d087c396a29b209d0a6ad571cc56f0b66c881406ed2f3f2a8e8a61f2ea1371da1" },
                { "gd", "26bdc82f71c93dc550b218dbf0f05a0c8bdebabdea2aceab1f56d9cafc834f85efde9e032c708237bf56f0cb41aa36b353a4b451736ba0ad3265dde269b02aa5" },
                { "gl", "ea1db0e243582406aa950171b731084a5079bfa7f338b2b7c8f94c86e0425d1a846242b6b184bdb524d3b73acd843c454c0045b17748eac71678805f1cd5178b" },
                { "gn", "7cd59ececf99755c864c75bcfc4c5e525d2e467701cb3466a4bf9e0c1dcf2fc40f28ab1f48b385ba0643107c97ab95494c9d922c1784ea34cf20dba493c528d5" },
                { "gu-IN", "4d19c0a2868924615525e3f1b35a3dccc1898634fc259f3f7be1f6402f84e32a2cf55e5a9697875a89d64ca1f486a95b312518c1539aff1f330cecc3c823c05e" },
                { "he", "c56a83010c5100f7912bd1cb4383427d61eda355ca87d64c70c215315ef71f8c800399efe1c65858216a874b0031533189460debdebabd13f21d2ff3217bed1c" },
                { "hi-IN", "e61a93863fe0b72d8a6265d018be6cac7aca01e57c508b0733de79c4fcfba5d358c9dcab8f4c76e841b6077378fb588709c34e5ac4e17e188d411dc8e0e633fa" },
                { "hr", "f29b1ecf7b42a1213dc8e233a06cdf7d2073847109e5b8f95db2e4edbf547c24cdba937fce189e3d7538a4be58cd489fb675fcb0afc8b2ed82954ec35c920d3d" },
                { "hsb", "290f71d0d1e4857d58485c793814ebc428aff3eb3ba995cc91b468ce18f7b6897c74ee76ab04d13814f51f16e18e5df2a195ae852b6a2fb76072e72cff26bbb1" },
                { "hu", "08e60af690dc14134a5a86678f11f27d6b9b159cabdd12f34a7de51def59e8caabb2504f6aee660653fb620af12c556ec8730f2caa2786f7edb1816b68dcc4a2" },
                { "hy-AM", "6da82ff878a95aea86e3d3b70183d831142701e5ac43ef5449bde80f8d9c8c51442270c657adebae03b8bc261aa5e7ccfdcf3cefcebf6467ccbac9e32872311d" },
                { "ia", "02d0078abca484b64a006d441faf38649382ee89498b99ebc23f0a685fc82de141a9c64b0560e4547c5a721be331a1c594328fec4c20cba2ba2f33775c714e08" },
                { "id", "58f3c6699eb823d1926956d5b3dd4977c77fb37cca1d63fcbb4a67daeab74eb068193088b258c3f121b045f49f826d8d301883e273a3597013cc936efd99abcf" },
                { "is", "cc9a2566004e98a80c7194e0b03c75128bcb85b5f312ebe2a08594beb8a1d190862038ccba054a26050e4d4f819571fa8ea118d78bb07d41c711f2dcaa3ef33c" },
                { "it", "75b53f823ffb0b75c6bb40a0d91037919b54b3db47df264ff9ea0daa7b7de0847ace035882492089d8d6ea275d2e7863cfa116992e8835de9891d201111e032e" },
                { "ja", "cd7cf6c199c77565824a01dc0aab377bfdc882e990bd7607c4ccf1011e1955030dd9455f98d09632915af073cda709cbb93407ae87d85e26f4f70d3ee4863eb1" },
                { "ka", "ae3aa9fd7b7332d2fc5c1f0cdebcf6e27c94289963f385cb790ab233ef8cf1dadf18dabb144b3b6dbfc74553aa7af945b97ad58c742826230c9a01df2a063593" },
                { "kab", "e1c3850608da94c1e996fb340f1291c6b532c48257b7c976fbaa2fefaa1bcc6125718c833ed66c4cc5cf02d308727ceab8339e797988dbf5b3756214e29532f9" },
                { "kk", "7530708f98ab662ddd6141ebeb01bb3bbde215b03848338443fbc7db497886630931fe2d35c5f2578cf8940c336462f3c0b41dba6e4f6aa97d003c2edae6a124" },
                { "km", "9c229c312ada497d3f4813d1887b948404cac8ca79e3c58c6c2774ba2c9e3e96c59e5035cc38544b50ec96f52217d3e989513cc91ad0545724ab3aa9f0a7f428" },
                { "kn", "c4fc5993a436f8443e81d10f7063d9187d8072b04c065b2b11a728d1f22da009e2f8ed20148b8ca24ecf94b27e5e1cc9fd62d2ce957db736e049ea5038a437eb" },
                { "ko", "4caa4a4f962f283ecae51e52b227f050fc360657c32a3d95b25a649eff85336104bccf88401037a86211980e059a202a2503ee266e6ebd57f3fa21e7c3de8980" },
                { "lij", "224f34eea02ba7ce1c8a8d7f12891367c5fc28f8f421adf2c8690f1599c47912e7f5e94804b67e7cef696824536d85a3a844267b174e0c81d7853efb26ad1ad0" },
                { "lt", "64e746bcc96b24b42207bbacc12b607e88075909693eea755c97f24724e4791c0a77160b5f73fff088cacda6cbaf6962647b48a8f7eb2338e1c133a1b743dcf5" },
                { "lv", "273ea2f48021b62f2ab3da56cd35fb251ac128a9bd336384187e58ed1ff4e5999f8010d7d73acce7811c23028d44dca643d2f410ffb968d87d648e542ae9f9ff" },
                { "mk", "589ab03f3fc5b279875850154b98f9bc04685a369404afcd8ef0faad194e2faa8b758092601c54aa82add4e146e4e8a3cb876fd7e44e730d33cc0a747acdad7d" },
                { "mr", "9190723282b5ea7881a29b3412c7c1fa0c2b8888cb2523940662fa75e11c49bbb92140064cf8fe04a29691adf116796bd7219e8b28293d8175ba8b7c7059ac75" },
                { "ms", "9142d9ad7be48e5beb7dd5894f5dedaf0a013c07e2d48f757cc715db6eb7dcb70ba8a8ecc71686a330a1328193f624c6f202857260bcd4b29bea5d29cc00c1fa" },
                { "my", "442f7ac7a52533baa29e93f95ae53c909ec8c6c38483fbb52f007090eb550b69cbf5159bdcd50a85c4f8ca669bf9526117ceed43cb153b41a9445e482a20c427" },
                { "nb-NO", "1d85ed3c9a4208cd9991673c50712ac77eabac4a369845b2f782c78353381703b46a62623e68c39a5139119dd4b58419b4b773c9d5e7199cf04ab80ef6e2232a" },
                { "ne-NP", "63a5fde45dcb140f46e7240f04cdd6c7ef3e0041dbb9347786a779a8221e5d1ecb0ce54db280149f4077f33bfd86d8559696c41518772547ff79887af3037484" },
                { "nl", "931de90184c9430274b29b72e19532fa8b11bef53e831403fd5a3bb151c091655162ecb1215322b1170f52da918389edf81acaededef8de5312a7d385fc73193" },
                { "nn-NO", "e4e692974d03b9319fb9ebcf8d5b6965255f3521c95d7d1e4439cc9af2e7846eea3625322ef4cf962adc01bc8b4873d808dece245460fa95e9914bcc290c2b5a" },
                { "oc", "0dedbcd11cb928f5c2a26dd68a18115cdec6f63ce7af75a94f30932ce08456fa93551853d9907cf04a2e7a375811bd456cdcec98b9864406d3f12359fd725380" },
                { "pa-IN", "965ffbb9617b40dc768a163c1918dcd926fe0db23b9d4dbf9d001a41a32e85b1226d6adac324ffe0bff51b753a676b920857c0a30fed3781e040051d54a926d0" },
                { "pl", "8c4b3b89f45cab080c0c2648da599622656bb238055ef23689a376ea38b803940980f4ef5d561fdfeaf09fe8cd0cf8b754336cb2e3fda2f82dccc46b76f6a3e2" },
                { "pt-BR", "59d3d0bee5e7260433f66679ee0aa6993739d1fb8070df7311df2387a73ffea6ef6e625866a7c0081400b1d01ab65eb73c9425dfc646839cee91beef85d8f5dc" },
                { "pt-PT", "52b98a78eb3040cb5cfc64d175d20a0e87994e9a9637a1ce74ef4e52abb8d92aad59d81ae350d2f8344a28b69af695b712ebfdf5188f54fc00ac71ada8e5190a" },
                { "rm", "634ea0bd47825db702d19767442473bb836b61747773366d3bafef445496b9b7377d2b5dc753282fd71666e212da064cd65846ce31d1394c8d2981a456c07214" },
                { "ro", "0d1c09a837ea2cb2e7b7a3f9b1545dc123cd294e01fa90fabb3e7a037ae818419f3ef38a056028bbb6cd5e5d9ea677fbbbf2eba4b07acbf55207e8425e9cbbcd" },
                { "ru", "6e663452f5c098e065eebccf7e382ab75604a6689adc8e0ccf530e4c54dbd3a829ed1a349a65247ac75aa6478414d801f5f986e930df449039ecf3b812a2aea1" },
                { "sat", "e9971cf1499b3a8427e0b7d63cbf54f06e466ce9bd74aec65cc15bc35b1788b8117c41cedc63ffdbfddf030de9709565b3407c9fedeae3ea4a23f6454af2cee9" },
                { "sc", "8025c72a644f6283f0656d5d5705fd1c59504f22300bbb2c27853d92ae77c73f3f3cce57dd47f137a4c8d54c5c70d184add82ab0389ccf14503ff40d874bbfe3" },
                { "sco", "a1d379010b380dfec13f0f1bf08a83cc3d4fd04ac2cd000b6d3f4d5ec4b9a43f69cb9abb20fa2a9ddaddad809ab4c455d664bdc63f90117339be8bcc0d7408e9" },
                { "si", "d855c28bdc0e07128e2fef9c5dc66dab2fd7c287d623968ae75339adcb7c34b98b1be9f9e56717dfbcb48b0460bbcd44991aa8379e8e70c9ed8deb38e1aadc67" },
                { "sk", "0ef9210947f37b5634f1856bc626c1d380e429e5111d9800dba01225a62524596f9a870b7ff471a847b8e94233e232e7ffd770a58a73dae46f652090e893881a" },
                { "skr", "9ed664444ff88b08ec8c754cf6b7909144aa61b47eb81abf9079610e78ff5fc1705d282afebf0b862e81406bd12157fa755d5e9f9a93a716bb183aa7a7f055c8" },
                { "sl", "cdf5690f8cdc48491413cbee182f18c24035122fc0e0df771b4cb2f77cffa7118c89af71fad4b7dced90dedc2da837c5b8e4714905098bc6579ae5038f2c577b" },
                { "son", "e24274b07e9563dd492b305a2e8f68d2c9be2cf950e09b7dc1a2e9b1b52e012cba71fd191f388dea8466238e589fa6f42014ab9b12556002712de917267fc3e4" },
                { "sq", "354871698b439c4898ba49b9df8849c4629de253f8ce17dcd71e405810cb623eb2ab85c95c7ca75277d6eddee2c1d61ae43550ad4c837ed5d264a7572cc69dd4" },
                { "sr", "91f2d2f920bb2cc78d37e9d3057989f599e68d2afea96f61a2803ba77312e41bb6718338b7d8ffd09ccb6fd83981a7d52920c69b1377f0efe7a7efdc4dc46ab0" },
                { "sv-SE", "fcb54a301c82e59b79bfc6a1edf677c3a4bbbc4fcddc38152f180f11e19f316667553769075edf8673e78d694791241959386aa06e254775651afa8826a6995c" },
                { "szl", "46f1478ee75d29894dd13d2fe335b55afc78df0c0ec287554536a08e933817b231b02d0d8f7f52bdb686e11b6b0583d32235ae920add030f0bfda901d0fd3554" },
                { "ta", "d2a9019599c91a1053cadc0b1eaa5e4cc2b70d3371a11315c0716c2d2f6dedde834a120676de227c58851af1215afc0aca56eb67e0c785a1eb2f51c8c6c2a97f" },
                { "te", "b4023937e539a1636bc7760c263aa57bf6dca5f7bce37d10d2edf21b64cc6fb8ecbe95743120a213d0e4dfa20841540db882c1293fc37930a93571bb4c0f2bc0" },
                { "tg", "7865b7eddb10d561c2bb09efff59dbf97087708bd05dce2ee3a82826e9cb3be832cf5c1069720f56ccad66b306a123c0f0f4ac120a300c598862d868c22b7184" },
                { "th", "93a6b5d921e613ac405544bce3078608b7b77fb7bd5465e91f475919c11583a394a2a731a2c41fb791151978c438f335037e6bc975379f98e810307822b4957e" },
                { "tl", "091e85785d3d8545a29a901c817bb22f9c332efa84d52db76303b98596db341aaf1e84742bf1007a36356d1e9260d4668e2641d67021bc3610bfba2e7a5b21e6" },
                { "tr", "354f4d0a5a3ee05e8399f6267c76dc1ec0f0d78b48c70183a746cedb1408cac9c76401c28541bd4d44dcc08ec33ace247b70b2fa287da1e03bd9a43104439b4f" },
                { "trs", "4c05c34f0e2d4cb544c583c5846b0119ddb3526117e8d355434448320fb23740ff85eb491d0c186c0bb2c3a1497528cded2f43ae055cf234c07d706ffa236390" },
                { "uk", "68c9d79fcbf20b454fbfb28064a077bff5869dfe81875e7d0f8001feeed5ea0783b5d9b2da21d77d3926e2c3c3c2376e05b3e8e6a1118ddab574206228584838" },
                { "ur", "681a1a8e1779ec8bf0afb0436f0e822b1d4bfea124acef9a672ea9cb874511d712a9645bd90d8bd17e00c8ab1e280590eded85c849dcaa571346fb2e3485eca4" },
                { "uz", "2f8bda4b70be29dbf129cbfcfb9c51e7993908d26a4c96413f843644967ebdc531bd1a24a263646654e97d042bfaeca903a70014df849b372eaf7fc96485617f" },
                { "vi", "20accdd1d55c2c20276179776f4c4aad5471e218b3a88387a95ef6f3a273d891f4009fb40f75ed693f7fc11408970fad5593f00cf5afe2da21b4eb0106d07546" },
                { "xh", "a689a9d6b913fe289cfdadff146691e239c5534cf183172533208497e7416c73955142b2f4adef04505366c76c4d74f7fad8289317700f4d71d3461184153a30" },
                { "zh-CN", "62d9de771be25e8aa5da70b33c7071733f06ba7ef6b6e62478b27eb3c3e8723161e440db34938d00a2cab33dacc5d2cd4a3148e12b9eba2bfaf58b7adcb988e0" },
                { "zh-TW", "de6a5b111a4f525d7425f8acf5f84b13bba6df58c671ce81a43506af73bc3c16e520cf2b39c12b42e8abb4eb7d8473f7b5dd3cddb0cf932efe1c067ef29aac61" }
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
