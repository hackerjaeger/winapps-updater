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
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.13.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "4192c02f683f37065518f65b8293bbc660407bc58046e1b5c6ec18a5d60f657dbb50033b2dca0413a3e8ecdf32c517021138fa5fdf43bef5d5ae6af5a8e66bc1" },
                { "ar", "d919e922c10c4cdef83cdbe110718b64cfd371d7d968081eef95b404b4619a42970c3e005ff0f4aae0a64371b6cfb949d5f17380f38079938b4c0acff585ff6d" },
                { "ast", "4cc2f79494640315512cec45816b5f325ecaf26ffe2227f6d36b15462119f5cc5e4d315fe7f2fbe891015205be5b8b5b77bd63d06561ca521c6f1e1db20fb6f1" },
                { "be", "9c14303d6c888924084bafce991212b081d0058a38363e6dbb68abf656536c87a1c5922006e48192cd1b9bc23e7c0fc0d94e5549936dcbe525ebc01c784b7848" },
                { "bg", "5374217a46597a4e0741cb80b266f678741695920571929256c3ee0e846e45150d4f1ba63e91dab2f702fbcc214fd6cb922c2a30944d610a4712231875ab459f" },
                { "br", "00ebc5b0b5c33f03644ae352e737db7e15a4896c71f703023dc05116a89f6743072da411327357c84783778757cc9adcd68d073d7a963261815a28ab592c6f58" },
                { "ca", "f78888b238198472c49e6bb75d06466789338394f078b129b81e5dac59e84ccda695b9d800cd8984a14263c09a861922b13853257a5eaf49ce72b584855022b4" },
                { "cak", "cd4764edd665d1df1d32fe708ec83b6d5e3eadb0fd89fefc00ced9636204838286bc1882edabd91de80f8da81d8e77d0de3bdb75fce62e81ceb769fa3c2b5ab3" },
                { "cs", "eff1e1f9b7d7611889feb3cb0899cb44827c68a7b7d7a247e9af5f75faf5aec47c47426c3b4439bae10145eff1ea64725d3b312b930902fb703b3f1ca0f07525" },
                { "cy", "29ecf10f739145db8f547a1a77b46293112bbd69d3eb3fd1a5c2e3ffb6e6543a5ea0894914d25e1586f0ff6c38fd4561b12d4f5bfe2238baee2aea68935be35e" },
                { "da", "fb21b6ed82ec167d63d3141b41465d91c90992f3f2851aee43b785a1466aea9413384b29acbcc63a4954eb826344ca1172bcdf30875e1f23ebff5e89f521cbb6" },
                { "de", "b2e4f724e4a1205b42c3abb14747fd95b5fcf84ba430ac95614ae69410614276cd0706c9b683cbef39f00c30c590ed93d87e85df30eaec9607a4e29516814fed" },
                { "dsb", "65c5d201b499ca4af099b3f70d81265546c703d0a2e4e5e34a502addc7749f3a6014891a28df1b2bd2dc7532dff792ca25c43faf4f497deb78e19e01043df48c" },
                { "el", "1b4ba57b30f4779032ed7478b755a1d780755a0f99423cc6e4f6c67a3f69c907c9f66a749fc79d63c12ef07a2aaf5e4dce7831e35462688cfdee65c60c25c542" },
                { "en-CA", "c48d43d95aef89745641779fd518a5f62801789e542f165795d7facde0e24040a985d931f7457434a17f3941f526e0eb9986fa39294e462f70d4c64bed294b3c" },
                { "en-GB", "be37eaae13f341d422a4e82945036380c83b8b575bd755881c4cfe3bcc05cb33c233cdec1c4147ae274dff977b9f916787960a7748bb8f38364a5035c3e02022" },
                { "en-US", "8872ca9e97ca794ad6d34016940076821e840c350eb04c80188fbfa1462b83088a5e40256c9b96460b0f13927205b24d3016927f50bad318c87315940f155cef" },
                { "es-AR", "8a42e9a2b5ee43d10765e69594c6ac420feffc5a2608bef1e2071c94fc935439ea1e1bba48342144c2ea533067f223f38009d0de963f92b0e821035948f16d13" },
                { "es-ES", "49ddabb19df2280607d327c06adc28790e6b3b68b8ea0d4da88b30a72a62be4891928673183054c5358842baf59a8d63022a425bb1fa33febf7f3f6a515a3ef9" },
                { "es-MX", "2e26819eaa0a66ad44e0adc230f89d1be437e5a5bf1a858ee5178dbe9359b1eb99d43db2c43df085cfaf5d2175f79ae9cc76943482168eedcea716945fbe5709" },
                { "et", "7220494126c01d9e216784d10f45cd0e0104d064c4028f326eb67ecdb771da8a137f1cd9f7b5fe20737fe99b81f4d658bce22a2375767d5bf346c16d02b17c8a" },
                { "eu", "281334cccdc40adf82b93acc6930e980a2d5504c03414df8927566ba933e20528f9b3334fc2bc4203c3e7656e128fb0acd5d90f56ac88711eac0e11a3539db3f" },
                { "fi", "b4c480ae03dcd646a24bc6b13657eba28e27c029f1e6966e94fae63ddc31e20a02cccbdb844e1b1495d7a2e85a75aeb6391b572e5f42498d0e90d7b58a08a5cd" },
                { "fr", "f7be04e32e248346bc3138b1204388d2d520ebba3bbbb621efb94162b8e290189e485906e6694e6dbf9742b2d8dcd253f275f552150330178de25e63a4cb7fb4" },
                { "fy-NL", "a4e00a5c0486e96b18bec99da853b8e619663d4aabaee8ecc03b416882f388a64e82a9a0f6cbd9e708268888f6edf2aea6ecfb6eff093500c7f0ba2ce629baf2" },
                { "ga-IE", "ba0ccda731ea042c9c613782f33c2d36c4f7c707319ffd59edf9dc030ddbab7c7d3ab2ee8c5c85a3f164afe0ba14ba18cfe5e95b346168b1dd16519e098b69f0" },
                { "gd", "e9cfb5376f0af9a4787dfc1612510d7ed4c12507c2dd53b8ce06bf514945a5739aa151d0dd2d0290bf96c34d3ec0d0162ad68286ff7c766455d7c4dc769f22a9" },
                { "gl", "f6de9112098e7e0475818fc9277f9826d4f4349ea4689e6d4b68b3facfcb1ef0f3d7ad86a796e50004aabbd06c475b41219550806cb8e00870c05f9936eb4d57" },
                { "he", "4ae84d6b520558418a50100e8b8ca4973e37c9f9c9cd95baaedc1c347c52eba6130d8c3945aa865ff1516aaba8f67359f0e39a9a680c509c9932b509251effb1" },
                { "hr", "41031f3f490c766df710f2668a134a33125c5d21cb1d7947e3724bc33ca4515dd3f7ef560cab59df90bcdbd9365e9ca25c386639e6ded807eabe051e773b8740" },
                { "hsb", "67583add9b7b456f74b4cf2bd0d1ba5ddd9620aff1e32e1e9bc45d89f06e053048bbe85221536204011101c7e62a420005f8a26514e6a0719feb8461a390ea49" },
                { "hu", "a2a8393574e5427b0f7ecc6ca8862f4f984fdc38d3dbc4ee7d99a0cc52b5213daeef01a3f50db4986d4d029dab76d25c93f20edce990ea5cd8a7c1cff8a99e26" },
                { "hy-AM", "bdc784ba5d5b2c01a865c30a2bd2ed50eb18c960c2569f2a18a2ed195561fb85b1fd65c671233e693d76cb5b32feb063122244e15d2fc482431b503e841ad6fb" },
                { "id", "10cdf4a99f814b836977523a18a4fcfc2a75ea2d8b1114ca373a8adbb2423941d840b36705d17b91b1d915692c6e79f9e51ce6f8e947d19997f03d8813564e1b" },
                { "is", "d4a921dde7f8d07cc6109440606684e01dea3f9c6a077791e099e0917d4edebf6f78622813cc3cb8e5bb4b7c9c93ac20ae6097c3988755f616a4e8b7dfe39896" },
                { "it", "7ca3e2c24edfd9a271264a0c5f897b0b49ef74a8188a3b036eb88ffda2d9efc55ee5653fd60ccdd35b8d8813210057e4d7283a4fade239ca7173892b7bb1d0dc" },
                { "ja", "93cfcb74d4bd54de21120431e0271af6ba173c8017cc443dddab31d1bc4a0a857e8b640bbd01a6b15aa638693b8aa71c150f02e01522c7dc93ac64f860297b0a" },
                { "ka", "ab71029155a1d26ab4de3aa03317fcae445e56e2bf22888cd25c6f41f9d2a3d1ff7215931a012d3dca6001d6c33749d8220934ec4fd08ebee5cfa105ad1801be" },
                { "kab", "bb8860ae54318bfe5477e17f1cfe6432b3349356eef1c3306736ba91d8c2634426752d42f2de33b3cbea9cf492739f549d469543f2c4d7a21c7baf3a61c04e52" },
                { "kk", "8fb9a0455d9581970445eb8ef536c7d5dccebcba306ca52667149f8d02f619bad94873b8c63ae9e2ce2832f617d60147a8484c103341e9febeb672cc84a161d7" },
                { "ko", "76bb0ac7bec7ad0c09c40f615fa6a2474d8f86e6b27c8fe904e074e4a66d088361b81c66d80b18e9615b583bce2154328fbf76c4743323762ac8edc55565b3f4" },
                { "lt", "10f3e439d3bae6b5588f304577f5895502b8613d73a3c12566b68f750022c84e070b84af12dd722df2f40c5f36d30fadefc2f6b2325c0457e727989e85f8512f" },
                { "lv", "e265f27cf34f593822d8619c7afdd6afaca261559de803614a57ed424fe7eb0cd963b92e4e39b70e56058367aeaf5fa09322abcce0c91d16ce25dad53c452509" },
                { "ms", "034be9b1cb5bcd40f8bd20dd26954d14a660713863fc44546fba90d9c90681dba78f3e7da920201be36dcb2643dfdf89ea154c054099b7a07bc3da0ee66bd1f9" },
                { "nb-NO", "226f4045f0ff18feb6b9c910309650ac8b408b4153ddd54195df2940b1b5b23fe388cba994e536a7391c040c2751a87e6443582c81662c75191bf32ada32a44b" },
                { "nl", "b6062471e33c75b5b57e0e30f514b85bab5eae7cb5b657dfa10f978cd28630356226e640e5c542ac2519c6f593e56cf9c1bc168d333078e6abbc0be65683c873" },
                { "nn-NO", "e74f6efb81d278c9d42636e7f411e2f6f8331ddade074eaed94595c7b14ce9fbc35b691f5c3a7e9ea584d6ab3e740ce513066f5d4e3a6c17ebd922ab2752443d" },
                { "pa-IN", "b1c803126eec654a6b855a8f2041aa7ef2794bc9e9219bdb2e52655ad2a2100a160603099c36a1a863930f1c6128418286c6713ade8ad9987f1671036469cc8b" },
                { "pl", "9c39c4263ff1e47f39e970550f01f79337d2a9c7305755d5bbbe1da7d3e63dd623503961d03b860f8087cd80ff6b959d0ca7d671a08620f1b0e68f2284174ded" },
                { "pt-BR", "bb3fce37ffb742e9bbd0ca1a5b406271b9e0aceebf93a42cd2b8b7111d911ee46f7c50ff037012682804f688c163cf5a1666a750da49bbb107391f10bb1d6d04" },
                { "pt-PT", "bfc30b813c5f75febda9a17b6dc319241eb30bd6b5a53fd09deaf98407994e058cbf877297d17a3c43e3a23acbff50b004d6ab483ec64b3144580f89ce938e85" },
                { "rm", "deef07d752341fdf582695879c6eb0e292edf147ca5338ea6f3d1851343fd8af89a8518064238926c1ae6851522ff0b18bbad7989f9649a1b3f5ddd48eb5d936" },
                { "ro", "a207e0f47a090f3d41c64163528434b222cbb570b8bb7800114906844bbbbe1ab8bccf0c027a9f4c4a4d31b142de55da694580d50d13741d06040b83aed763e1" },
                { "ru", "c1121275fc5a9deee6e0a5a64de0ceea092fb7d7928adb610ffdc462174f990f678069dc646072a941336989de79caf9e2d0a5b6b476c70a987f17a899e0deac" },
                { "sk", "706c7871d7cf83b6234b7bcf7ddd87ae0359b5cbfda55d5de2f26733ee530a8f32ec3fe1465f675706a6c2c0dcd4ee77c0af3f38e6a353ce07c79c94566f66e2" },
                { "sl", "7663260f815c3ea3feaf42b7be55dbcf70d05196913cf4b382bd144e9c5e6d8a7e55bf0942fbc9e2e10d8a00f2bf02dc398662767058edc658a597f21c010bc2" },
                { "sq", "fc036482f8c69a4078e1ec7991b4752116cc306c700d5e7c5c821712987cfa88617d5d2d3c61244af3c7c1faa3884d2d7d7bd7f8d5589b4e3f8d0f440ea05565" },
                { "sr", "77fc39473eba347625d82059e352560eb0cd7088ec7a513d4aad4aaf93d1610092c1a96bfd9e3a997d972adbeb75944e09e8e788ea7440bbadaa83c61c659b64" },
                { "sv-SE", "555ee8b56ecd61e11869eb0a24799b6ed9089ea04e5e9eb6ad12aa620c74d78a1bc499c93ada090f0a4648d0600122e2424051b83bf45b7216d28be2225695cd" },
                { "th", "899e42d694e9754c43345379b7bcaae9104489821d9ff0975b49930281e5191c2bce1c2e0b6752dc5ab499e56e66b2c922e1dd12df35b136f71353f5308241e5" },
                { "tr", "06c1bd642f99c35cf7645a755fa5d42ccbb7cdb8990907673619ff17bb7e2648bed9e28dcee88873f1cb2881500a3a7fb466dd81faff80b0b7750c096e5a878c" },
                { "uk", "2cc08480ac3ad69a117febf8bf4b781b0b9d89ef6796902ac20ffe63002130a59795be2c1b5a188c1ed35a0db49be649d207790df51b8144bbe6c98f2b4691cc" },
                { "uz", "a92ffcae35b5504e8ee3678eda15e26732673ec85a201e543d52d79123eba83c6fe726c2f6061e2179c866008c0c1d946bf229abb7d5b83e15309789c3ac6ed1" },
                { "vi", "45d452c34f3e570c862d3954766d1ea336e281153c435a2d038f8505200a171d4be1070cb02c6368dc27895cfcdcb126db7eca26310cd68605fc0283917c2684" },
                { "zh-CN", "b034ba4b138a546b4e390e90443a026b6c2cdc66f38bcbaa6d8ef94ffa0ef77078bd3b09ab8cd8ad919869fa0827a7258939c2247ed03fc7f6a25cb1b5dbefca" },
                { "zh-TW", "5624ebde248d8f5386d78635cf621b5e0b2fadec7b074e5c43b1ac4d99505d0e104483dd772801fd05ab6789fc5cf58be6642be6dd996fb3365eff642ed509e5" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.13.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "8ad2b273dd25851bb65ab36dff7a64a90b1b6ec76ca98f0e2b12c2194b07698d7a0acdb3f65e046623c1ac15d9811bc59e7850c2c8385dcd3df00e5d36bb3f6b" },
                { "ar", "86b217df8ef68edd3e6cb25bea2223b844ec2c3e75801fe1c15c7bde8b2bc32d3205034f01a4169f1c5b3ab2fef57ca6aafd6ea2ecb95f44868c9abda0c9dd50" },
                { "ast", "a9b3804c89dd21d184119a456665a3021cbfd881468d67592b473da3777f3c9da2c6c63562824f09ca9d7a35f5d0b1d881bbce32eece7164eda74391cd49d069" },
                { "be", "9c864634c9fa959871d752cbc38f2d23d67dc22ab9b197630d9397c0837a99ae9a38647ee86cf63989b0b9f9456803d28b0f13ffce9393860a9c130b28846232" },
                { "bg", "3862f1428607b1017b7cfb182486121398b981684a7540219e4caeef5d6305e187eeda730765eff6a78bad5dd2cd21faf94fc2a710bf43a683d351cae914df0e" },
                { "br", "5925dbae3c5f7e3561712a2c52163fee0b80cce12c409294f73a1eb1c8cfd3e4b595613309133767f580c579f8a02875997c19b1cccc2f90865a0ac91c816983" },
                { "ca", "aa891697558032b411f93893b681f176e6432d06db1c710aff077b6ee4e6100e06e5b93912b68de0574c0cc082058781992920066edb6ffba048449f81557eff" },
                { "cak", "1e81c686f22976e23805c11a8931eab89f979e901a5b2eca74ee60d65fa7f8a98392148862949a6c0124082e28f133f714740ad1e349ea0c5c5ca76911e7492b" },
                { "cs", "23d69448663aae86ff9d88ecbc030eff20f4db42644b39ad26ace2ba5bb109ae0216f51b4e4c156d413f9940444d0cbf137c4194967d40bf81a1fa240722b09b" },
                { "cy", "fcfc7ccab46e433d7022a06cea43fc63dbaec7a7ae103da8b6c96e42133ddb6ee23d446e50a7f511589570a18236d7fe08b234b577117599d1c5f858da2efb00" },
                { "da", "ed0d2a244048e2d57f744713a4dde96f4dcc59f48d672c63f41e3d5b9064e76b16ee56e539e912b3006d372cbf87e81c315687b460a652e2f866dc600f5def8c" },
                { "de", "229a7c903a59c0ee90035a270a394f849c3ec3f05b3bdda2bc66955e1bff44fb84d4c14398e865e3d6b26c96ac946da6720e544b0cdb382dcb514c12e705610e" },
                { "dsb", "da2c9af7b398766f24ba2e8454264777b439f4e580e3c07d906429e06d7763243da68a2f862dca26f7e801729596ff91b245a4f45c455378b02cbd4bb7bae0da" },
                { "el", "ae03ad336cf2ee0b456cf83f9ec65a9baa8524ae1afc42fd82ee13d531670747cb5b27306f1d4e9b16a62ef898e78b0e61f752194d14db1983d88d59124bb3af" },
                { "en-CA", "ce0f9091e2e15a59358115166189771347c2574d18a288a576f980289343beca6c65265de74cea10b0bbac899596bd1f1974e4676cf2e9361560bb380cf253c2" },
                { "en-GB", "9674ab0c1bb61b6bd839ad8b10c152ac35705de23bb2f5e39e2abc21706c56b0507b3b1d49151d2341d6952d0deb120eed8628f3fcf9ffdecdd560dd7c057b40" },
                { "en-US", "87c5e6f28beb91e34dceb3daaa446a8e72c239cc7bee3373481b28d1c516cd9416c42bf6e69c293b6c5932b1786eb3370f5d9dffeb94a47cfd39a59fc1f6f365" },
                { "es-AR", "d2d55fbcb1ee4ae64cd51634eca1b9c68ee7efffe1c770efc1e6cb9c958c79c7925b5aed25887a15908ec0555144d52d7fd2e924a6db688b6d8a9d157070f2f4" },
                { "es-ES", "2bae4be61c9a284082f29189e92dfa63c671928f034b32732e5e8e120308846d22e98a7374683be56a043c872587a60828b40390b534fc1e33b7eaddaae4c907" },
                { "es-MX", "84d03f7e9a281550e962ee2b949ba1f45340360c8a33b07f418450e393b08edb4d15520d031899070e0438f3a48378f2c06c0c547ef7cb7ec358d38a3687a725" },
                { "et", "8bd00e5ee4a3c004e7a46bcb215f1563f321ade12457825f8fedf798ad31838d49fea2f1c187a553c7af979a858e170d1232a4f476c5099e06f052c9940c300f" },
                { "eu", "d74dbaa51dce08bee19e6c55d159c3e69d4d0a80b0a7efaf8c99ff18f836e2f9939d3b525a632610ba659a177031738847838b7cc1e8f3f452b3df2ab69e938e" },
                { "fi", "3a637176e145160ac6a89fad4d09f6e8dc985fc7e92b8d67c297cfbd0d7c08972b90e4a8e5523f04027cfbb8af4283624db7a6fec328b0c929a4119b1d210e92" },
                { "fr", "d75b656348e2dfb5db1801ad07705bb17167f2385d807460ff54cff58e050a5a6286bfc11153c4e9cac138df79dba927b1e202310c9ad307c94f0ac0a71d4d9e" },
                { "fy-NL", "2c2a7113cfb12ad0f7460186e364c91ab81d49b6b44ccc6fd1ac1238b61b70a08e041a2fdd24c2ddf4022096a58012a1da197e3f979561cbedf6d8f6520cb864" },
                { "ga-IE", "e5e29429e786de0e77cef16994f012a528ee4da07843cdb840259411d9bd3a8128662a322ae5157eb16b64de48f6a6a27e80ff6e8bbe47407c0e6b915f9e4226" },
                { "gd", "99d329dbe21facac9fe2c32cf42ec304d6a9b21ab0a8c5029f0c51ad7daed971d3eb58eec9cbac6c492304845d07000274fcf8ebaff3ae2b944fad5bf03ffd75" },
                { "gl", "15a6409e18afbe5eeb1a7a4705b1e3ac4c48478723cbddf29b9a8fb23de72e5e7646905e5d18e9f8516fb2f85187c7df8bb9ea5726b67ea17eb90da8248fe84f" },
                { "he", "f89c190a16784396ce357026589b15b75e0325852981fea8ad1edf626ccc58737e757e1953811019150fe3c7b846af36a9e3c6a55067b6f5b3c3044c5283a676" },
                { "hr", "79ef44da8e492d95ca563ae28d4d1057ec27a8167c12b8579bb66b69152a16c83f644001a962f63b803c5640413f46fd237285e45c6771a4d515c91838afed78" },
                { "hsb", "dd59a573ee76dd4f9f6b2ee98c7a7edbf391cdd6349ef6735d6bd3d911bf47b1b6c65a9f5d41819933430ee256f34d1f4466fcf11e746c1e5a6ad59ba0ea01bb" },
                { "hu", "f5f3bbf04035c21a2f2c19e687f8a6f1bd9958e5dbd6e665b7bbb5827ac68d48479700cb6c1d49dff4d2ff1180abf6b7d8f29b149453e300057459798ded7cd5" },
                { "hy-AM", "802fdd9b2da7e5ffb1bb1fcea084b2b965c0a6bc661336eef06ce03f3637d3858bb6a9bc8c5a39f872d29efe35a6f70cf5907bd11335f454305088e63bb9f992" },
                { "id", "8644391580ec2556f7603ee401722fbf8057ffd3160c7c180245f7513333164f8eb5f3070e3d102594189d0acdc5fd990942a6564af3125367a3dc13908da1f1" },
                { "is", "57b6ab99feafcc7395fc1041aeed0f1199e82c312b75f44bafcb8aa54ce0b8dfaa8c9a4ad9e6ac573cd15813d13ea88f492457d01a729661c1d8b8531b13c0e5" },
                { "it", "84f5c4cf34f95b84cc385b2d83bf72afba9313ce61366e5f811c45121c13ba03302cb5997a46ac236dc9ef76e1f588d0978e3508b38ab0d822f7345e99cc4d8f" },
                { "ja", "dbee4b22bf81541c5a96f4ba7235d7c446512c426e19169533526b9dd1da480d2643aa8e4a5ff7781d870ad386222501b18c989f70558805cee3450e1ec6e109" },
                { "ka", "09ffd5872c32990621792850631f3a71ad1e42006de21bb719671772c098333cceae3c084b1312939fbea25715de03bae978a67ead591bbe7e679f4ce75e0e86" },
                { "kab", "16bd8ba246e5823cb1deae0a9cc9a2c66c9c192f2ff0ef38b83c78b475f3b94b49a50aad88695a05a82a55f1bd18d1d8b420601c7a3c68fd78c199e940f04d13" },
                { "kk", "c9dcdcec258d09ab084469786786212c7539a38c4345c7064532f185edade8a888b1e143b93b246b92360fb93db42d98d8ab4081cf55efa29e27484a21452f80" },
                { "ko", "73192d267dbdd6d4246bdfccd7c563706a1fc21662013fee6e04c542f36c1e6d7235e73a1280f25d15f2c66ff3b7cd458bfe662f76b14c960bb934072444a436" },
                { "lt", "d1ad34c89dd551e3d9be2ecb19fce75231e9b8fef3bb62980e9138eb421edfec81f9dde96bb746a0d6e18097db4ccbf8e28c8e778668653f4253cfd8ca2ee0d6" },
                { "lv", "b452beeea84cf6dd80035dcdea98d59115664966b65af16d833fd4bc1a9c2c25a1effa3fe2f07c1af95fab839058b15c1a68e8791f5b8e2826e3b70ff81cfa25" },
                { "ms", "035b51567e1257195c675895151eb0b1d535fb10353256ffa9ec5a8aab8a938d31dba9773a3df8bf63bd304797a0e1c7d5a0b21882db80fa44e5ac607542796c" },
                { "nb-NO", "90683c0242682c01e5824df37fcab1b9e33114cd00e6aa0c5df5e6e87aa53b066107ff31595ede83f7bd60a2c1cd1438b92aa9151c718a6b666231534209918e" },
                { "nl", "e83a77b170b9598523d78332aa79dd0af01eab1fcc43db71121a884ab10a69d95bbb45ee1832245b64ed0f5357c6cbeada3505a32e6b5e1fcfeeb633de965bfb" },
                { "nn-NO", "e01e058fba16ce3369011bc3188784370df7fb8d5c28c94873b8f1fd2425e364dd1cf5934a262de5d210d364b81e1e6a123756e5acef8fdd793f1a8729e9ca43" },
                { "pa-IN", "5cfb1cedfdc8c4a8a9ecb58768a8073f5f3a6b12e70771c7616bddc3a2ef7c5de70a3610e28bab510daeba4a57303e7efdc73ad46833cf35632049888b99d352" },
                { "pl", "1800bb89100fca5c726d297e41641fe7aaf576c7fc0f028ee9f9af956ceec9c3a9ee26b29ef5cb2943e4e86afc3a142195c6bbd84bde0a77003b2462f3f2eb5c" },
                { "pt-BR", "d3a609baad525adc20cf66ca0506d64ff25f8a15489e2745dd0f3303d2adfe58140015eeb768707ac509ff7dc18cd56f728bf07a513b47ac6aa5d47963814e72" },
                { "pt-PT", "0866256c042051c7e87fdfa4a0a519359381acd00d19ac6330a136e7ec12d561b1dba79ce44191176c4c9a51bf5fd8ec6d88712bbfce0135450798000ab281f8" },
                { "rm", "20a728f7da84fcec26f45184d3e7d39094586f568dc5daa7b5ac1e2d00ecd44fa0e53bf562d2b3e0c2f776a63bc6a81b99ce2669704417cd5138622e72985b62" },
                { "ro", "76b7222745269241e4eb7d217948579ad3b5b78fe4f6aeae69fb4b64732f1d81be52064f338bb9b3a50e05ff094e7153ba45d9af64c04ba5a58375b783438846" },
                { "ru", "b92c6671155949e86192469f98fe69b9fa81c6e81912a3c705165f4399b48997c4443715e67788d0c6e9f284b3fa018ef8c0cb4b35822560f775b51943c482dd" },
                { "sk", "c5555c19894db442f1461538ea1c0f9a767596e24256dbbcb5e5d5fe815946166c98ea8670fb60a17a6ed6752f58b1381df6b80cdd8a158a1c5bcc0d9644c4cc" },
                { "sl", "bf0e13966e139b246a76395763134e2790820ff314e2b149c0f7173f7822296b1d4347ab68fffb8e7641fcd4f09d49771b4d15a9e2dbfbaea17064ac851a6d2a" },
                { "sq", "e1b9aed5c9c9bd2273cfc7c12d0ddd69fe8a833dcc225de062f988e06e67e6fde80c74449001ad3359d8710331be69a4ee64d2eac732fbac43a159f0a2d0013d" },
                { "sr", "bea02c2c83d7636cde33f6b0bc4ac0cbf1192658851e473b6e1f5a2422dcd44debf5d09420eaa1d360d609bdd519122c7ae1dafc1392a6fc2b81709b9d308d45" },
                { "sv-SE", "58a6395a7c50a3cedad4346cfcce7a50a80c81aaea6561af65c73582aedbdf757c73e64732136bf022ac0e6d7ee932b646493e5d04f0ca64940de5c27aaafdd9" },
                { "th", "31b59ba7112d4db0926cf8404f4f290d71d7d5514fa616339a057342fa0c201e063cc33c4beec760401fcd83d32795c11c202b1583eaf7455e832392549f52d6" },
                { "tr", "b1105e309fe8f7bef4bc8848a16766c0d85eeb6b31f4cea9cf1ca100ed78dd1563b92a29731f28dfacfc1d78605b3fc22364dbd74211fafc851ca95ba1ed9198" },
                { "uk", "a8df4bbe27f519f0507f134059ee7321d104ea9690b65ebfb64a27420ff12774d9ead96ebb67439687969d538f5a768339298c2f8efb747c888bc937f755b0ad" },
                { "uz", "690e078d60e5b598d7158f489f0ab55e99f7a27a0c4a6441ce175b4c290fb825f9ff6cc9859675454950cd75276efc416762d549870d9a3afa2a790ca9d57f99" },
                { "vi", "f5c9bf5cd0c52d0ba37299a80753cf76e4d76637eff72d1cc798b1cce033758a2d9056b8b5370bc60e6dfe3dc7b3332c44efbad71da52c145508fb599261aa46" },
                { "zh-CN", "77ad656e5be6a9f39c84247625eb5dd1c6f10fbe9c7bff7f02bd9b9c7014ff2758c186acb0e40d33915be5431dcb007998727723c56a85f747aaddd73701a8c3" },
                { "zh-TW", "d6362a9e718286397a82dc2dae63ca3010a5ecffa88053bc998b30f0f0088f8b00a1caa4fd4be9c903dd9482fcd7fc098e704b44d8af2234a8d8238aaa8f570c" }
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
            const string version = "102.13.0";
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
