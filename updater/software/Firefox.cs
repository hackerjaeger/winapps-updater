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
            // https://ftp.mozilla.org/pub/firefox/releases/114.0.2/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "063b0043d60396c121b69f9f181eaa3c63b03c9283977b3649dceafb49fe7143a49496d052c8473fd49a484f920c8ca23656cd3333c74c5d47ac1eccb98458d9" },
                { "af", "e8be1ef06599555dc2214ecb59bdfbba8720830aa87d2ded2d7ddb4fe209ea123b5bc572a8e1ff5c0a98f16ba5e8cadaee15de3d3d6e4f851c250b601590f923" },
                { "an", "20b55087babe244867bbf35c670f07fd7615dded505d6184ba8ad00af98de38804e6246d8cea5d7fa463b707a669798e26d187c3cfc4970c8be22da4c4140d8f" },
                { "ar", "3b65a143441ca65d39364a0edd448a0e90879e275f6fba3b90976b5b5bc164bb2ff0bc11e598c524656d011f5ac02fbe4dcf1bb60519426507e01e6b7b90608d" },
                { "ast", "b4c08e22775a8020daccbd090da5f5eb7f2b4c4007da2ce62861b8ac7fb15435326bf5681bedee686ca1c59692a35de2958595722be91ac3dadef1d429ebfd47" },
                { "az", "2c33cee44a579e8ca3ec3515fc2e6d84d2177aa89ed1c7103fb05312c1f4585afc4cb1d24696043b6a77b9fd0890284b6b782e10eab0c7dca12a69305f1a2ce0" },
                { "be", "04a8f83e621e1519f28bbb9d6bb5e560139d9a926e56fbbc516b99079d63490d5c2b4033efd42eee6a2211f8140b737052c045c51cdb51e964e6fbabd8ab78e4" },
                { "bg", "94d30e9166085291c79087ed4e47d0198355d4f407d9c79fd3ee992ddc08d18267f0535d8bde8a64f554ed7af6680de1d57219cba467cb9d173836e84329eafa" },
                { "bn", "5f30dbc46a839bd450769b06d58297f39479b4986178f56ee974e06bd4360863954d6701638a76f4a49e6b41e3d6902fa882a312da14b61946ad56b4c8fbc843" },
                { "br", "20a65f8e4f027cd977bd29945c3fbe613321f2efc59bab48628f0f25e8a3d6a451dcac2c5fa28baca999cd28f7f20704ae52a59782855198158564bd8fbdb0e8" },
                { "bs", "dcf21c23f9a5981c63f821a8f9387d809ca1784a92fd0180434c359c1a1badcea31cc511b6f0f9decfd1bede78f1529693ac3ab10434ae46ceb66e060eabc2d9" },
                { "ca", "e5fe2f6b93435b0d5c95d42f19c33bce49cfa84e169c5eae8b9779c783b693ae328bd47eb9a3854a5c781b58226ab7c25e9bb1c06d258c909bc3cc0a3d3b5c30" },
                { "cak", "fbb95a62fb0841a38acd23fa67f4cf2656b302ebd820e1ad652c48ca75a13b9be5eebf8077e1e3da2b5c66c8ba66c804f136bb6eae61349ef271752d7133577f" },
                { "cs", "c51f484c45bc7da06ec5f3e771384864d64e89aa283816fbc39bb798a22f511b8cf24936fd1fc26cd2bf7b58f00bdbcdae51da18fc051f3c1a48e67e1100e080" },
                { "cy", "92857af66680efb2941086b330ccfbfeb186ba0830b21cb01a6e43b34ac75cfe33333ebb74eb90c1c366b76c7a1ea8abffdd37538ee11c4069e6eff7064938bd" },
                { "da", "28b1771555075db28996120f2f131069e6a933a4e595803750e9aad870943e44f7e13237678a15a7cb8e8209a9f671c1772ef76bb3883d3eb4fd29df0cb89d7c" },
                { "de", "02d18af96beefa835b70d82db851bb53472bbd1fc84c86c102f219fa6e44ce93c2b05e05921c479eb0d3d16ac3707a4fc22be63a7fcd4051328fee31cec8a061" },
                { "dsb", "38caeb3fe1979d016434e36ed9d5ce2b9b7c9fc335d899343bf1e67abec4eb464855ddf7b9d8f989f812ee0e5ab8ed89a6cb5b022a7727d8b7dba3b7e76e5cc5" },
                { "el", "2dde3769cf24796a21f9c4e460d61d0a606537d236899b736a9094ed137c87a228d65a1c50a16a7ec979497238be3cf7a739275dd5575764db1bda951585bba5" },
                { "en-CA", "9a48ea7d0ec231a74e664876cb537c11722d9a0107ac6e62c2eda1da10d64d948cb280d23969e35e99b4d95bd6f6414646d9eedfa1cbfab47869ceba8a2f4ac3" },
                { "en-GB", "c101a8b17939467fd2d03a3419fc5a63cd93f4f39cbc0e9f8e89e42ee092b707653d615f4460697400bcd0f0b0b3265899b2a9f5541537594e9e0254f92dbde8" },
                { "en-US", "a11e94593b73ff9cda1d35369229c1354c1ea57e5adcb029578614e23313654cfdd86ab8dc2dea3404919cec3c9bef716b096f270454974c5f0e12216a9da761" },
                { "eo", "4bfe771b03677ea5875e2da3855310deafbba7c46792eb64d2b46cc3cf46b74e9753b07f3ed7ae14b743bfcfcee331ff965f0c26acce83859bdfc2ec9cbb5ec0" },
                { "es-AR", "71accb3bee688f70592d48ca869aa88fa5578086342bfb815b43380ad43c49244bad873ceeef966d67e5d92b2f320154c37eb4fe37a21a8b9be429a9671bd116" },
                { "es-CL", "42a5f0ff9a62a2e0e861b7794744b519cd42789b4cb893c8a31938a4a9af4ec00d8ec5abe1052932f09800ba291faaf4e64632a4b46a1abe2a7b5917bb1176b5" },
                { "es-ES", "0dfcd63a2aafd7d1876cabfa1ffaa9261dce37c06f694ff25d0513267ba262a834f282bfd168022f3de41c1a21eb80ef9135e2e7c5a59d6ea1c55d1521b17e51" },
                { "es-MX", "331cba3dfeb6ee90376735817b15c4e8c108a02ccbd5fda41caaa13b0db963332043bfb645245cffef1a2f67f1d3347b23bbe44295f1522de2debeda57b3b480" },
                { "et", "6966fddc77d607bd1ac4c0e54d5b8d1d2076525889544b81d491215fd059fd5866f03b69c67cd20d7ff66d1fbf280eed1d4ff78b126051af3ccc93b368e3373b" },
                { "eu", "2c7bc11e33a7fed2ba0b88f6c6474af81580cc4578e3a260f1a9dd249e5c860ee0181f584b53684bf5c96f038bbf8968f02b9c561bb587c44861a8f628e61d8c" },
                { "fa", "fd31ce003d374e2a975fdab7f9a96f3616d47ca49660ec50817fe9af5e5d4d062fd5bee5982d0c82c13b0d687a7465c495556531f60a5b3b145c6bc8d215aea9" },
                { "ff", "f30ebb666dcad45d337d264caf601d28a898f3912f2e03612c4dc47b115789e81c40dfd0e351788c48baa32fe085e126700718113346903db7a5fc33f8cb923a" },
                { "fi", "78095255b8d860ad35dd341d1706eaa2ded142629de123ebc1c6f95d0660d400009522b88ba5b96320bfdfe435cc227300e73595450096eabc0084df73844d08" },
                { "fr", "bf0d64d02c82433944b7ab9bc0be3544bb7777c5433983d905299919eac8bc89777147ecd7e4f05d8fd444858a4e44fb5216c4bb6375837447f83af4a07602e9" },
                { "fur", "aae652f204dc52f662c445e2c9a3f28384518a4346790c8a2e99590de24314c60bbe268ed6159c7f04ba45ac20f450168f745e88bf2ab188d64d60730532b97c" },
                { "fy-NL", "662ba39be898c369194c9b0326b3ab0e56d19b087b8eb494f98a9eb89f2cd58f12bb2d626ea1861865e6a414e3d51bddf9ce7b91ad4efe0c2aab76f32dd49c27" },
                { "ga-IE", "8d1011e6e572a61c70657fe1a4b30349536cfe72bc0474239b6372f005b98b515bb44edcd10fae7d96c6773181d5585ea6ebc769921aa5121a19e42518f403f5" },
                { "gd", "fc5dfbc2eb7163b142c0efcad59a82bf6a9352a3ab056071a5443e91e9d182db71e8b94e1a658e29991365bc65ba67ec18ba3989a15ea29a72344327249d286d" },
                { "gl", "9b6d9926b86b0f744494c03715534f5b42a87dca9d083465ade3a85c9fb4b0ce9afb5617dae9d05092285c1032da5b6c95c6473074097f7f7dbbf94e70ac75a3" },
                { "gn", "34203dc743391b6c41042d243ad47392c22f9acc2de19fce037aa0947cda61c785c097cd399738a8c8bb18f143e03c53a25d2b888b13f81aa516aa1af18b3a4f" },
                { "gu-IN", "35ee7ce67036f41720b073ff2c720c69de7d34fbe1e10498d6d769a01ac98baabfa28ef9eb44ba894c201b456f97d9d95646504e4aa73a7aa307bb46b2b1565d" },
                { "he", "c243a5d36e7ef64f71d71601f4c88b5be2e3cb374016208973749e867fa0a2dc4d0f8915c7a4391e74c11adc8e2e7d6ee4ea7d128f68eda3c4df2f639df948f5" },
                { "hi-IN", "0e128c80336adea3a9aad76a8e2a35c6eaaf9e502b3a3c3551cad146eb58ceef698178b2c449e279427785d9006f5cfefc69f272dbde25c62d555cb9941d2e0f" },
                { "hr", "e38897057e42b2e3678c9bab06d9933477954bd2a6100c93b75728145c2f364c616049847f28c78cff05bad00112d6d511d1154d10fffbc6fabdaf63983ac81f" },
                { "hsb", "67818b313d6ef11273d0ec2868edb9eec1a975933971d0576eaa1621d31ad62f3a93146f0db685a401ce5067338574516d4e2151792bac2d2279fe2944a7538f" },
                { "hu", "bbbe98760494169620500fe21b4b1e002f405df15ddb42a109db128c3024a4f94e52a5f720adba04506676d8f0b9f221ae06b7c1e741ca7ca8e41a39385e8bf2" },
                { "hy-AM", "d9ee538957e95445fe459b8b089bf31941ef49760d3d4808d098102730164aaa067d6ad9c59b7bc432a1d8a2c89e434844136017c53206274cc42c0511dda501" },
                { "ia", "cac33c0f27881ff2dc9d1c707a9c68c7b840c008cb22305999d0f452e6d3355c31b6239a591bedf22f1a14e80382d9cf4e454828ac420c11fe1c647021941c07" },
                { "id", "27f2f4f04c8b1c000345c65d3690b8c60878946b78fbc04d68f2d19d0788362c1c5094ccd6f0b799cc15953af2908960de2fa3c4913dba92d0c4dbbeca3dbf58" },
                { "is", "6d1e8927e7bcac0bf63d7ed7d296859fa7e8e62b1eeed95ec1d3662a78ce2aca520c4afaa0a442c6750c13dd0c9e421e6fdfbf71d841355167829d104d7f4d53" },
                { "it", "93c402988a728c84cea278b18cf1d9f4f771a18f0e73dd3e685c8c52f2be131f1c754e2bee056db01e38e3bca064b0db6053c8e152ee7fb84e64d793b697d18f" },
                { "ja", "7df60917d79fe2fa3a534dfda6907e224807ac11b5e61a7579518d67e8f3addd65a43c19bb2e3b1ec7fa733e80b2db782eb8eb9cc8b3e3ad28cd9c72cd266e64" },
                { "ka", "5dcce1207ac76f6115d652b69083e317b6aa40fe91cdbb4d10d8212ebed28c6565b608f39d55132c73608f0351ec105586072e3ad1f47b290a19bbdc49ef0fdd" },
                { "kab", "d76990937c254ae7f1e22091b670e199e4dbb856c1b1a222db53b0639e13e9fd1a6e7292ee72416b242edfd5b1b810e764e3efb415bc264d26cce5def9d21f3b" },
                { "kk", "b015e4e95a25f88fb63ffba6bcd7b771fa2a953168b1aaeae35dde956250e91488d7c1654c7259036f11225e1e46daf8427eb7c3a6734c5b245f95b9b5ea4fc0" },
                { "km", "fd8158af92113fa71aa6fc5c3f232c178031a94cbf194705b47a0243871d167ddbb69a5d94915e0da0449762364b0f7135f3cb721bf18535a44260de23611464" },
                { "kn", "e6971a7474abe05d477191216899870893c56a5807a437845745efdc99728e3dee9520552af85b5486fb1204969b58b1c79e1f9ec9dda99657695c6c6bd7b9c8" },
                { "ko", "55f5dd1440c2264b75faf4f31ddb4bc6b86b384c2ff22521dd49ff05f9d375b00e38af98f88602894e7be77ac995b0047a8257b52e7ac7225a2574fcb9145f9b" },
                { "lij", "7324a1125bf6fe86ffb06a4993938139829c6d9373361f181ff0f9322f09b3b156026e4720eecd89dcd4f59f699a683f533e051045fbfeaa3b3615a63669d237" },
                { "lt", "9cdf8b84ab6c3cfa463b558069f4413beb026971748309472fb139d5be37e6b840e3b47a03d90a6796a5e57241d50c509369841d539808f46608eb567f49dc09" },
                { "lv", "cb7343b58e93939f60ef739dcf96a33787b92a8bb530773de845fbdec0fcb534586cc64c044bafc881ad71eeb754531c00da9784298c7fe9f15a6de4b5c84d85" },
                { "mk", "a2615ab224bb06fb4ccb3ba52eb22769524ef450de03e7452d1bfbfe1c7a574bae2740c095553b375fcd8aefc64a770401e8c1a98fbb358bdb12fc29d943d2f5" },
                { "mr", "e2448c8c5954703a6ee547ad3aefcd92f2032c2ab5b6008463a0e3944204085fe873f3d97a86944de8136dbeeac465f75e15093c650dd3b2044710444d2ba6db" },
                { "ms", "a5313bae59e345b17fceb5ca2ee697215b497660408edced278b31dff11f28e357160f8c7e2736b1cbdfa4ae7cf0376007e7091d4ba7e5d7ccd5a8d9338c4e87" },
                { "my", "8315f02be180b04680bc478b3e60085da481a538b7cabced215e940ab35c37e12d8c2f5ab7feb12396b2804d678c851ecb6734ca633dc5d2a221a4db6d02597b" },
                { "nb-NO", "6e8c2ecd71fb48d213ef27a8a488e376e161d6b517e69557b2f1c8f2d01e6f3e0bfc0cac4280274fe73b5ed74404a723fa24eb833a2e3b295e752fbac73eda28" },
                { "ne-NP", "4fc5a619186f3210104c79e26f674238b780ccd31bcdec379f8e2f23fa5d55050898e9422997daf2bf155cb5ec615896b53422e7df1f8027de9fdc2eb24b3c3c" },
                { "nl", "5290e3f4e07a0357c24a8f28fddc646654c67f7d8d0e9750c4c78f60c095aca1c028eaea44bb2001cc2bc465d48c32637fd25f5b862b5edac7609ae744d72caf" },
                { "nn-NO", "f3fd8deb17d153cce0331fc8f143be21d61081fa131dbdb02eaa016d8e552201e8b5df7b899d9b89695bb60e3ca670670de9c4471533967a95c2410df48850bc" },
                { "oc", "6ca9511760757f004e2137a4b74560c630da67494194a7c53a3a2e2746b372364d8bcf5e8d11e727c941ac7609659a67e8e624e3fef652dade7afaf32aef74c9" },
                { "pa-IN", "b981b16c396e73f6940d4634fad8de95849de0ab55d47c09abb2c881de4dabafecfb517d20fbb16b79d503fa3cf969b7203ee05635b156f44b4bb5d02761470b" },
                { "pl", "969e4693368acd5439752e92535553d7cd697f6c3b02ad012a365198115790e3c8c060087a5119ae56fb52df12a6b7917903d336bad5bf9578be6fa69a1f4855" },
                { "pt-BR", "12b91043d2cbd8377f3a783797d01ac8e220b480b54d21c46e54bc1eae9ff57118c85333428b41ffaf2a7779384d3497993ea8bd88d024dc41523e940d82f486" },
                { "pt-PT", "630efaaa2ff8a67e0930d7d416afb3d3a642404287cbaa5a5abbf9016b954921aaaaf43345d7089f08ca26e6a30e181e535d0749c2efe3ac31d235e46d3f3f78" },
                { "rm", "94eab9cc8d9ee71d9280e62957eddd61bd463aff91c7e40b730619b4c4d92cf3c8d5e1160e23a244177b15009e846707775cb15cbf4a8e88ebefd7154cb7b5db" },
                { "ro", "538f124be2e05ade4b7f7829ec3af0302fa9878c218fcfac8cf480f456a7c80ede1f031c3b53bbc48fbbf99ce69b851f905ff1b4c130e2800d3a6dc452d5f4d9" },
                { "ru", "31bcccccca5f4903b900b7bc44c805babaa1b2d2b45b07074b57b1ebc490f19da4d737b89b327430359267c6102122d3b7f4ae5015dd81d82078b71fa2f45b72" },
                { "sc", "22813d5eeddfb09e9d078f0dc36422ec7a3f38124f08ea8806b9174f912a2ef11d3aef06778ca58b61d53e2c3606911af55182660ae6dc2625b3d342896451c4" },
                { "sco", "c857d963cb44f6a0a68111000470ce173ddb907f774393b3ffe74fada37f1af678079b25f802fdc8f4340f544cb242cf2529eb2c8a13905efdc8d859791b8b4a" },
                { "si", "06b00c48c28c6d4e9414904dba583340180b05047977fae3439f041998e6d1f5beee62749ff4e9b66dcad74209bff2020a8138b89d5b9715e2d86d49a4603db7" },
                { "sk", "3622d4461dd1853f8109cb9fca3365d46fe8abec31025bc27808a5ff4b2b10829abe080177b3227485060d828fd2abefc72e6b1daa9f11805bdcf5924105306d" },
                { "sl", "7e04cd8d9d23a1ff1b8227b959928b73863c4eacf378f18136125cf4c84e862bf2cacabfef750768fd91d32a86715837c50964d4ff6f77bd4b8dc5da25c7926a" },
                { "son", "341b35e3ad13ffb920e28e1831594d2ecdd9967f23a12e37da39448e6b3536ba7ba2f4875b5e433d7a3945e2748d9997fcb3716d9f36ccfcec7aa9cdf25bb02a" },
                { "sq", "cb84a7c8a050951e2489f14bffbd9f780cfa46a5379b05daaa907c67dfa399d89b20b1d6aa40c8873fba16e146e775fa438fc1f2119e01cca82ad4fdd6f9dce0" },
                { "sr", "29f8fd957853a76b461a05eb31c8d4612556a89302ba2e9170f64d30c6db782704a03dfc0b853900172e7dcd6891c2ca7fd1f2d9132fcedb99e260d69f0ab629" },
                { "sv-SE", "ec8b81a7bb6020193bb3166092190bc158320f55a0df4ac33333c988a622ae270d46ab19ed7a484f3d2bfa06d1b1175c26a0b609ef61f80d33b16c0a368126bc" },
                { "szl", "8d784eb961609c6c1db4f9897c417e042b71ff33047764bfb5fb2f858257ddf415dbaa75c13d2efb80d716626337151c1625c569919adcdc2fcfeb0408dc561e" },
                { "ta", "eff8586de6aba405457bf995213b0a9c7fff6d3061f234dae6d6a023b867534dd2ce3d1ca7d21410881e63089cfb1ee7461ef0ab4b55a87691557e43d1a13f47" },
                { "te", "596346296080ff3bbafa4bdafe3861b5edb9caa2c9a0ea558f77eb96bb5ae93ec8c9e8c97b8ef818d65ff4912dbdbc10aaefba2bc85a77b453691ca0355a0015" },
                { "tg", "ef35f894834c6dd94ee2615176c29ac1d916250697de31dc976f905e246514d3285ec511820151e27cc5df0fa10eaebfa33924ce80a0d3b0da8da58fc3a62861" },
                { "th", "4db62da8ed8e913738fd1b8a99f62de1f242ce66311c928cfdd582737bd30a2fe4f1f8959f90c64df4fa14249e83a626cc292ee4a65c5e8711c945b4da67b675" },
                { "tl", "3132c2891fcbd5bdbd25824a0cb0592844ec5b7b1950083b1c78fe50a69abc042eb1fbdf5679c98a5058209c346db5d4d5e833442326e2e6f3eb4b95f01d2a6b" },
                { "tr", "1b01697335478121d278b36dfdc622bee451e824ed6b9cb35370fd3b8f4a33166089e10e39ff12add610b556687d8bdc94f9b47901acf2432866b91e96fb5885" },
                { "trs", "3a9f52a0e5939be1ecb915683b4f8c6672eb03e6075d0d434a5e471f0d4db5b054161bbfb572ed4e35064b79d9a09288d3e507843d22c959ed7aad6d83baa7ff" },
                { "uk", "790467580110ec7c207aeddcae73a1638e5bac52c168ddde0da3c636cffa46a2ab6a66dcebd3807f07c3733c51e6b33dc50d5cab4ffdc0cbe1aea97e160a2a6c" },
                { "ur", "a88da9eb8555a2bb92b1669761e36a90b8220ce624561b78c9183558bab846a2205d1f770d152afc7db20689bd5f4a3a7191cf3aae915049a4f2aad5005b8d53" },
                { "uz", "2d4bc851ce52289705d8ccaa1326f63b33e431f92c6f4fef26b22f9b6d2b559f483ce96e8fd5aee5ee965f23478c3e28ef180cb86289b8f98acbe1ce1b956469" },
                { "vi", "6991421bc6aec82aea15efef11c19a9e1edc9e7f498c956b182d6453639c49cf291fa4355e7c0576355b68ff27a5904d21179f407d5e2182bbe693a9b1c21544" },
                { "xh", "332d41c8aaf7b21a041c0bc3cdff07eb6029e364264c3b82d715d4a9781230d6c3a6239e82be3de61c6a21f3dc675d3748274f3380da119e810b917b61f49125" },
                { "zh-CN", "555a2643e36a2bfe8073d87dae49f260c5f37c3a2e212f5865cfda014b65b66682f7637cdb4672a89b666c4bb1d4b6638b3d43235c5ac08c11c46911ce023b15" },
                { "zh-TW", "55cbadce1a3f3973c06a071b7c15319c183806a598d77b63270a221bc8d65a36571146d85a44dc670a3ece4c543fc7457d77d849914d1776970c1ec8a0b2449f" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/114.0.2/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "80616137703a15481d904b6f1df0f51849ac98102f93580b02c9fd55c4a283ac2ae24229a830bca0342445a0986b868452d38672a7c89febdcdaa4c3aa28fa1d" },
                { "af", "314bfd922000d10f998d1810e2e262b76421561c6ca9980518d109d87f19bda099b1eb63fef2a253338024d67194b4f0aba27871d313d926ea2002d5a7d45289" },
                { "an", "225cc5c6fd211ec7f678afec006381183666069bae60bf9e7cd3cda7f46d9a15a615e41e710f3f7a1cb2910d5e230bc37c9796790e2e693d9c7c1c0cbe25d388" },
                { "ar", "373a10e13e8efbb1cafdaffee48fb6de98ddc746f4f93414767de207e6412da8e9081ef91c7d0c47efd6c27bdcb5e4d2c40dd769a35ac35d08cc87013f41866a" },
                { "ast", "791725abe7d9b16756aebe69b0486c6574ab0a7c6f802d9b94a261e55faaa1527f897d84f9b84f478689f9c1faaeb2086f971bc5b289473ee69aa3617b01df70" },
                { "az", "57fb0858bb56631b678c0ca429e4529e9b1b4c3fec68d2ac4f2571c95b9a91deb20cdf3ebcce3282757c426c84b9de6701599421d269a4e4901c00a2b00c1309" },
                { "be", "61924e4264534988c4cdd016ef08c137d607533cb2f347ceeced8c2e0c32e86e1cf41b50cd499e52b3de67ceed51cfa411cfddd47eb8b3f29cc38f4331086192" },
                { "bg", "e0df902b565becdf327b5c761ab2991cf60686f035a1da53c838e3d60cddcd85e91ae15c66ee0825aed7fa02ac02b0e75ff26c5bdcdd12359f0f6767eece681a" },
                { "bn", "c4b5fdda26af803a8c656b4900288286cc85d9b3bc22dfbfa81cc25f696ec2be8b321aa0d4acb2eebc47754295a67bb5e23dad8e219ea21d98f0fbdf0ae8f137" },
                { "br", "864d794d28dfc94922923ffcc15bfbcd5e511f8fffb19bf41e34ded0836360561803831c1a57f8bbf8dda325e26a5db22c1aa5fbcb6af723ee76fc3114af109a" },
                { "bs", "38e47ca7ac139d42a207f9e1d3044600396655607d98722e0bd1a6f08a29a05353f2a6f6de45f8e02f265e8d3abd4dfd8fd6b7f01d7541f9d6e25bf212871665" },
                { "ca", "feb7c8fdae289a5672b80529ecfbc346d3f9131ed0917e77280f0acbc06e9ba9c72eae7d4b398dbb84599b06e059e92f42d68a38551e5a09b18bd630231d7192" },
                { "cak", "8a1009307c31a4cbcdb03ec126b55b2f25b537698a07b0fb7d0a72c89e3543c424bcfab1e8a42a0b614cf91a3a88c4a7a58362e80b35a849c27c197cc4689af1" },
                { "cs", "be15b55fc51a404ec3e5f83433918c2e91d7643afaeadb600cf80f7b13872c0f816d1d7ceeb31b6492af65f3e8f81e502d7b2a39c3c9129e51470d3c0c4cf869" },
                { "cy", "6cef6b5a54ef37eae3264ec59b53860b6e0f558bc405c6eda5bf47cdaa05ef458409ec0ceb3ae0c01181882401d29711c16577ca96b7fe3e34ac2748c100173b" },
                { "da", "cfc2e98c5777330a5c381cbe26e566c650c788cb69fce910202e60ed818f4b1e41f208121e555266d63de1eb017547f326cdf70b5dda8ce643f07dee7ff1c42d" },
                { "de", "c0210c574d8bef18f9dc83a4cd70c52250fda11962e440ed4552980b74383504174d8032d373e61ececb80c4a0da80deb473fcd55b5f26973c419b4fb1c8c9ab" },
                { "dsb", "1b45983ce27bd5401da56f9bb04c49a208ae8bb2504d3e86b58352a83578cc8fe76d6f0572ef110ae99c35052b14327b8b6485e5e268fcdb7dcd49cf5fe96fbd" },
                { "el", "33e121e6fc394907cc55cd5b335e8f9a6cb086e77ba1804231ebe2da529f81add5f4321e6a005f0a054c936a8c93021c2232901fa5fa659e9414e8edac6adbc8" },
                { "en-CA", "9d048a0b936acf238d804022f15e60c4e33110089c40da10ed3023436645183d63cadc4c3c5df083a318f136e5595e2a6b461c3717228b9bce339b90597bff7d" },
                { "en-GB", "9831cf1c224746584f13baf87c2264b545f338c1f3ab3a4f760a439d960a20e247e2e25d69e4c2481a04c73d6e0bffb7d326505c37b3fc0f5f9ab4acdfde053a" },
                { "en-US", "ef404d39e4852e1af66ea1c00f3a4756080fe0ffc73d38273772095df97b199e0781c2fba22d7baec400b8079e1403a2809b09c039db996665c03c4684e20c2a" },
                { "eo", "4a274536d16547cfc03d302dcbf3a82568ae7499633dd0c7c86f2e6b8132a6c7fb6ec4dd2d610ee47a5b08188b78079f22e998c8199f336b9b7a4076cce89461" },
                { "es-AR", "a53a12523f6e8b3edd65f302a4e4ff7ea04bb7de72398e1c480af7c4470fadeaa5f5f55a87b1b2a7a4317896f5c3602871a57f6bf835b9fbe190c22c7981fb08" },
                { "es-CL", "9cd750f2c996f6863dd2ad65a49a88d7b88c9abb029d0b038ab3bbc79798e4b507e05fd34271bb09d629dd1f93d4e4b393f7ef6048533d60c46df36e7c168d7c" },
                { "es-ES", "e7b30a049f426748a0c23903220bf3db71c9d5a499079ea5a30b5bd9796f2571d94dff09088ecaf2073c1b2e82491f0a93e8c896fc5c969764a7a4275f52c2f2" },
                { "es-MX", "8f70976badde6181ef769dd38c808aa4ec20eaf4a51759cb65b04003bd1f2fb72baba17add98a0f744bf44362053db03b6ac337b918760eaca39f1fb0552e1b7" },
                { "et", "19c2092a7bd21cf5981d654543f74801320caf41e451fa1cbc587add01c2ce71bfc1bcd342adfe0e2c00115b85ba7e339fa99ab543db5e44192d4bd59f6a4b23" },
                { "eu", "154fc09769b70fe1e796885a248c433c4211e2bd8bc7a708a408e2c5ffcd141696bf2b93ed0ae093f00a5a7ce8a500d5e0f72128ec6ebf3720cf143edc57ebc2" },
                { "fa", "869527c4464e686f09228ea33ee909b446a6d9267a46c88e2757a450b24c34f043d1e5661c546cfcd2d5a99371517ea20f922d774af16a2d830229ef05964321" },
                { "ff", "9975b6ea3a4090cdb15b564f5f09231faae39784636e76ac3f330acf3eb4e0fdd4c7bb8d4da8824d28136c575b5ff5e0586b7f99447d0004ceb120127f16fd4b" },
                { "fi", "701376fe6a061cdc5813fb561e8435ad98392edbabc2bade39061ed0df03e1fcba09ce925cfbe5e9b7eaf997235684bffbcc14670cf5a2cd98690705450ee2f2" },
                { "fr", "02b522fae72524ec90263173ede62158c1487f58c647457bb5c1a9039d6c26cce3bedadb1d11746d5646a1d48010d852d91caaf6132a0ba4dddd048aa1141992" },
                { "fur", "ad2972740fc116aa0f5dc6406d023f864a000c8e51b31c7c5ab89344d4019c234f7e92d4d0da28567bb05fe2211038f78e082bf03abe1718c333c4c5a2473d8d" },
                { "fy-NL", "a28cb7cbacb7bfe7c731645267e47f1bfbfdc24be2b7fe75f7a64d5aa41087ff6341696393d2662e7fdaa3ac0c442229bd541c0a98eed619f020ef64437551b4" },
                { "ga-IE", "accf31b94474ea0c1789aa36b2acf367382892265b8cec3931230b781ab95bce520a8bf71b4b95054fad018a235ed38ded05af43d5aedadf2a0881fc0466de0c" },
                { "gd", "c9277b46266e4fa2e57b6dfb4f2f9d81d337e010f6931eaf5866b7d606f64ccee0950a6eda5697217ef75652688c55262e8870d489fd96ee721888d8cedc3532" },
                { "gl", "68e92b5ac0a3ebc78c6809635a91c18685f4004cb7b243e4274ff81e8a29bee730bcb63af55a5ddbf0e4067346aa6f66f6b5c7cc1d99e351487904d4f5ef5cf3" },
                { "gn", "b7500dc16f1ee20ede2efea6effe5fa2aea0d49eefc4adebcfc54eb5d858ecb9177d337707f7c4903ee8fbf49709085941d77d020fa1a49c39f60148a44c2d9d" },
                { "gu-IN", "f0de0466a41200ab407e35cc1f9e935cbf40549021dee6d237779e5faa8f748e44090e2e44b8d6311525f557f578e67d60ab93cc514e1197bb7551baf46814b6" },
                { "he", "20b5ffc8b5c6aed4d8ae6d69b5a6ff7534e7c475e7c07597202c774d913f4cc5df7c2ce6a1c5967111dcd893ed8bc4742419aac71d7fd66d0522e9abaafbcad3" },
                { "hi-IN", "20048b0861ced8da60556cd99e9ff87c8bb1081392397e6e5baa98ccd2c7cbb2343292f93af9526f954e8ec49fa321437e2aa4a74f7ccb90e7332d9476e4dec2" },
                { "hr", "fa806ff89182453a2c0fdbcc6e1ebeb21bbf9d672452a668517fab15f8e963d3a17b7b7c6cbbd4cfe5b2997a76181c8b051760c05c22b21af9ebddea92e11ce1" },
                { "hsb", "56827cf71224aff946732c843d544fdf9a82400b2f90afa66fcd1d3028ae080a5a6a178618148b2d45e94c54c0e2c347b773e980ad67defc1549aeb2814c562b" },
                { "hu", "141758841282cee5f4bfc92d4689f47191252dfe972f86d8513501e1de0e3fcc6ada3ac6a4df767a98ccadd6dab250ce62fcbf8cddc225eb4824454a76ea63fb" },
                { "hy-AM", "2f9cac88acd3a84e5c366c8d82bf63ca9162af7013c180520025024d60703979ce137b0d5569f04c43d7af435533e74de249f015639d89a63d7863944e8a3263" },
                { "ia", "f236ad9b304db0e99f5999b2bf8853c8a5d9e112408a3d692c0c7709d94808d32a279c1344fa8475e8cf37cde9b61513f412e3d2fe05642a85da773cb6869a77" },
                { "id", "ba1cb4672733c6f90b77f23704b100c2bde64ab7bc9e3972f06a66b76a28b87ff80c5448bdf0e59878aacb9b57bb839c3ad647ae2baae8373a0365e137baad8c" },
                { "is", "c0783e5774d1d5a921df8b52a6a9c62f7504e10e2f531de15e3317e6d6105bbe97d01923c1aaffd6c33e2b57efe1669684692f6559825cce9b456490395cb718" },
                { "it", "0af2ea559cddb766031b3c1c05010ceadf94554e61ad6391e2859ba8d7c8fedd283cf333a241ba0905138f5114e130948c7c9f7e18453fe632c625716530b6d1" },
                { "ja", "10c86df187d2785544d4d34091fe95107d146915fe5b53c921199fca234c3cdb8ae6e0d8372516f7a715dad41235130b6bbdaedaabfd61c4ae071d10cd38c8a0" },
                { "ka", "e9504886d1134790051447b9027f2e7dcb65a11aaa19fc5cdd274868bf4ff0021eca8f8474fc7a4a15351290585d0bc5ed3626553587ea3a4785f0cadcb945df" },
                { "kab", "7afaf1f67f914e0ad101dcb58bb406f229e0b6224f3fffeeac6ca9cd4f65f73e3182676e55794e1e648eb83765e496be6c8c394bf9d4f59bd9033ab016a3e76a" },
                { "kk", "4acd27175eef899a571244f657de447053d42a4365d5667b0788eb54158379d7c55cc90529c0713f9bf73027239e829116d814d4046dcc635d3d5d5dbcd9d09a" },
                { "km", "15251a4636cfabbc7c3656e74edeca63d10cb8036c35427b9d82f90f6f79fe9504871f399d02cb4689f20397706caeee1e335c5a0478557c7ee09d46766cd591" },
                { "kn", "f42cf269aff0c84887db175502b95be5ce959e9919740dce31cb3775a3ddd99590339333b1159303b8112d8526bd0d014a2e2405e9762e55ccd32f29bfbc8f79" },
                { "ko", "094fda61b39f6fd032531a45d05d0f7bcb707d24e8989ece1451d4eb6fa15e1d6c2ee9604eb86d3d3865ef6b6c311f10698d91227532ebd6bb580c5300c585c9" },
                { "lij", "bd59c372f2e56b3f6fe83e31816d15a973f009ceff80faf0cd1cc3bf59be93e53d7631aa9a652952b3b8b4c79e887776c3cc01a1123c8b2f0c9733c00d8d5244" },
                { "lt", "a13a2664dc2b00b0c5ee1b3af28017ee3d9ec0354d28f1a619d27a9e57909f6e830fc69682219513eab488f7807cc6a20f75a91b8b62c2f703225f2fdf85febd" },
                { "lv", "a41a81f9f5ca9176e2e4b08f1e0fdb17ad62893a7e5dcb3cbbc9aeb7cfdd461ae49cb2c0e73111cb71f7a8aa37babc31dce801e7092263d9e71d7460fc5dba98" },
                { "mk", "261c265bfc283e80eb404d135df003c637c21cbdce8325fff5d03c87124a41014003ee289744b714173dce81a6bfad69a3cbac0edc9d1c3f46b5e66454bab936" },
                { "mr", "9180434185c46a6bf08361c93eb3a6979dada7a6bbff96eb722ff3e03a9ab5f0757f307ad70b2b1dd21e8742b6677597b723cd0530f3fb818173093f1d085e0c" },
                { "ms", "5df655515a30ea73cc847692dfdcbef030942854ac5e392228e7faa3cb37691a7ec185f0240457d55c1c8f7b2a78d4c8837ceb3fddf24ec831211cc1e7235601" },
                { "my", "692705549ae2616f358d587b37297ab640abe5479e9716e449fea78409e557645bf7b64558aa2be0597e08f6b6ccc81a65397fd42e10f44b0f600cd03abb9bc8" },
                { "nb-NO", "4deb336ec7a20d2319975bcc6b2989def098eb0d1765323213e2d7939a2772bdb921dd5aaaa98c18a2753601f2cebbab3412f48654c606fc15ae412554d518e5" },
                { "ne-NP", "84fc9b146b403ac4634508d25bed6ee30b894e4e1b445300bc66adc7fa7803c967ae751db37b08960c78cb3fb8d8c02f88718549a19ffb444ddf24bac4046652" },
                { "nl", "5359daad8ce72d46f7058ff02444a4a32ac3477aa6e6430f4b8a782fb1f201ab9b3dff90aa5f798b0c6d0559dbfe8bd1032aa8c58b8c0c4895fafff0189cb0cc" },
                { "nn-NO", "fb5a86d158bee735066f5da79188d1676ff737786a92a9d6c25303839cc2cfcb6e16f006225c21ce503d8ab8cf00860d88e3d19b1368bfe29a085b95aa2ca0ef" },
                { "oc", "f6175504a037311152cb1cce9538c7b5d77a47dfa3eae482e0b329031acd8b188927cfefe957ca5a6751b416653192a79fbba9018b2daf4e901b53329c6c8ac1" },
                { "pa-IN", "7463df347bc6d978d7e97fdb5e8b5cc5807df7d3df93542f839bd08457b7bbd606171b711789f553b3fae5cdc4fe9b5b478483f7f54db7a933c2640c58f0e4bf" },
                { "pl", "9b8af315dd06d7040b77e58f4e96bd2e5839fa720e83cf418b2c500955544764f787e0e6422e688f654b3e370a21fdc48eefda6a7d9196a68fab3d06fad66ca2" },
                { "pt-BR", "c8cf02611affed5233db72e6444ada04f3d1fe995a568379717d65368067d00ff015713d28089844b4ab31b35e7410e341c99a8132391f7bc499cf7b4db46fca" },
                { "pt-PT", "a7470b0ec92a03b7a446ffb7a3998268401880fac77b7aeca354ac6336107b7330b034eca21363ded0fff0c09620345e881ef4b431915a6264f60585d7103797" },
                { "rm", "63bde22ea42a03ca60e6e814d439814ef80b9459f93e4e19051d470fa513bfa0ce0ce279ef5c84049eefa9b0d7e7443afe49f21e0364ae4dc40fa1d65b43ca3d" },
                { "ro", "c25e10d04e0364177b0e48e57c217ad32f4ee984ba8ae16eba6be3e5ee16c2e3f08da11836b4edca332a913dcf2989cf56fbf80cb6cc9cc042e8bc94a80ec620" },
                { "ru", "e75af09b3891ff5e4dfd0ddb0aeeef628a7287c052727109d7cf2cdd3b6fccc671277fb1224b63a3de8505b8ff66ac3858148e9a91a3416201963fe193993537" },
                { "sc", "b93fa7320358fd40043fc2852ff94ea8630c1a57cde686e6c445c2222f928b31d508bfd0025375f66f9cfe491692fc4bd81237718c05a9684cf6f38248c55471" },
                { "sco", "7860a61f688c3578dd617c13a7ffb226cade5d721f7d568f660b3f8541c0f83860073e3250b17c1878c09f32d00aa5fcea0754eee31abe858521e1063d86a7b7" },
                { "si", "fb1140ef4f7c32d25e12546b6b033772293a48edad48c2f73a82fd0021358567a097638367456d162fddb9642660fe32d0f1ad6050740cb961852b9523c0e3e7" },
                { "sk", "725157deed5b6286f616af995c80b351d169658b97136ea09c28b80be43b896103ed21d85660bd028e78e67c3d799ce9bff27ec6230c6b2bb1d7bc3fd356873f" },
                { "sl", "de1e416e945d1f09989c9cc7af6e680ba419a1ef2b49ae76921712b96a370ca67e92d4e4c1dffce7dda64f6797359c7f3295af906c0f847b05df7143630b0963" },
                { "son", "7dd4680ac2cb82c2c6d86ae8609eba947a8e675b606392eea5ef28a52c5c1c1d5895e8f940a5078eb4e297c7cb3a3d2277a1a463ec8909cded178aa86b707618" },
                { "sq", "a2d8a3d1a925f8286bdb51280c4cf6aa4721a2c6d6faf56c24f68928e36a822523295551cb1f644612b0976ea58657abc22d7d634c411fe4d6db78905028373d" },
                { "sr", "1952aec0254df666b3b2519daebc927c52bf757a81ae127aa34554145a28bf075ec63e1a8841629f02631fa469f34ec9b2e456c41527f961188290cbb2f9e0fa" },
                { "sv-SE", "f08164b6b77a6a2f9cad05f5356c1b37a099004eeeed4c542d161fa1e8e40cd11b35054c8fb530162d1abd075cb53268eb70cff1441e007b6caa91d17c3f41cc" },
                { "szl", "870b73a562dbfa001e1ed469da069451e0f1109e2880b15a720b1b15dff106da71f9b07fb49f9f21578f5b019c2b3f2bdedc8ac2bb288473c344e4c8ad4b84c8" },
                { "ta", "607bbf76757552320d5e49f27953922ccaa90bec5259c86945a361081d977d88f8b42085d9a9987db8b9e75035646b56e2880d3a38024814985d9ee26db4a2cf" },
                { "te", "39733f576aa6e78d9cbd9a1dc3ed2392a33e13c5ce8832c0cbd14f48ccb76ee37aa31afcbc57023b88fa16daa4123b69064074801e6934b68ce9af736c6f820c" },
                { "tg", "b1dd992565ba248f01364c03d70d9ea92f9884545d6e6a41b525338f59f82e0abf66fb73f4b63b08cf91565613f8518c30ba0455615fea630747d94c98e0f0d0" },
                { "th", "df4404b689d9f3f3dd7c7df4c6fd566855a2d2623423d0ec04417fe07e9d4f7fc1fe424b04b891c95097e3407709f6309d186575e47209209d26fa42bb349b58" },
                { "tl", "19581c8847d79ce0179beaf4136c3e4133570a915a300757bea3fe97b1c6992a899ae4ac69199e411891896d7b57efddbe6c6c22d558cb7b589c95e08319d115" },
                { "tr", "d9878ec3eabc9829b5becf3c1d2433cea176fcf3f7accad3014c7e622cb9e6a81521259948fdda5c6a4674b239d2e279193a9ef32163d57e3add112974c7bf22" },
                { "trs", "842c2a9041cd385728f9251a771cc3a45614f638f7325aba15df727b5add4c2daf154ce800f07137a9e473c68a7df2c15436220de527eef148e0e97f4532edd8" },
                { "uk", "3cd3df6739df91acd8da93aa5c68afa56266d11fed7e2dc8a091885e70a3d9c70748314d0f52ee4ce3d5da7b044733d8ada7666d6f07a061605155d33e2d63d8" },
                { "ur", "176d3d794d07d94514d3524da46ef432a8d58ddea49da27791f9fa84b7791560b086fb51d90f03ae2df4c09e0643b996553e10bfbbcc70b1b138ba74d687e10b" },
                { "uz", "164656fc3657601e45957f7fcd8fe249e482dca742dc0f51d995a3b87a78be2e41f0bd0bdfbda38fa0b72202ba654e0e1c39b262bc8adf88030fde45a3370a60" },
                { "vi", "34cc63a281c2df3ed00f87bb534584e2d61103c402a69c3ac5747f02f5756609a0ddaf610c430907a80cd742a56c3b29e1a547634069676dc740406b6c42a4a4" },
                { "xh", "7ce3111ee9b75bc4108566603440815308d2d00a09027ba32bfb7727c07e7f66927e1242ab49de720f21aa6e25ff195961efa065aaa6fcb14c33ecc03cd8c522" },
                { "zh-CN", "c7c71d2106ba37271a77331a6ce45ba61e7a683bf49b51a42f1c89d7a9a949b894fc35686cf30c53f3da5cc68d99a4840324183d612204aff91f09328017ac21" },
                { "zh-TW", "1838d2c02c4b424caea3f9228137152ad3b607879724ff81f2d6a4b8ade5a070f97fe4944f70accaa6eb529ec02a2cc45b623899157a907911c0190bc1256d3e" }
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
            const string knownVersion = "114.0.2";
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
