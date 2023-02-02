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
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.7.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "eed44b2546bb23702e4619c217a88e9749b01d0f4e0152d07270d9611b7a46106cb1bc1788f403b6f9f89e2b418fed227b8a8497e66d9bc099698aa781d3b9e8" },
                { "ar", "638f63afdba0a67b708419add530547a25bc8d000064955fed74ab6bbf439f235e0edd23718e832dc666e8d3717fbaed7d36e710eda448fca893348159569378" },
                { "ast", "4961e32761dc0ad6c6645a0d44b09f7530252e3b4402228d3664e655c78afc65f41ed2de3eade4c5c52c5fb6c27f5a3158d62cfa386c5fb08511051a8fe8b21e" },
                { "be", "0e8085307a09151a74f1dc1cbcc8d4f217f5e8e614902f1f3c57da19146e9823859258e47e7611ac7f204c1eb1e5bbf26ef4e656b226a655d2121ba95d4322e7" },
                { "bg", "b7fd81faa1ac9b96a9d699a78f4422f5eda0ee02326ac443dd109f94a0bffd9b8d522fd477f1c6f3f6ee4be7f9576f2ef2aaa8fbf4093c173f015b1d000b5427" },
                { "br", "6ebc5c16251a8c501b9d66c889909d8c8dd05a6889467ff6018e9134ec7991133857ac88b0a924cdfc82663e2743ff4a055a699e22aa8dd597eea42bb1da51fc" },
                { "ca", "73ed2806f17b92eeb8b554202c5595625ddbcc4f3ce60e58a709dd1e0d76aa606d51e08fdfdb3e84f8a5c72f3c4a790c45f0a4bbf5dbba367966317f0fd73ba5" },
                { "cak", "519861a8ecc59588124c366c371924890ce2f7d195529676d60d025ead5bfe188db189f81a2923403760667eb2c4a3a768b51b1a77b00f94edac212c1f5f49a5" },
                { "cs", "2e60a707440a59aad21976d7b85966a5711d80297a8cda4498e039baeb4f727bf945be65cf8201bf642fddc237d0cd3c0a2037b62ce52e3904914b5e5222cd92" },
                { "cy", "6c6ba34d9fec925f47e3eff73bf08d4928f525d81681b7df68eac70afbe3b65370582a3b60efa5ab79ecfc10e352120096570c5a840e023fdf4a723a7e76cd7f" },
                { "da", "ae576881547ecb8ba9a7fd648808ea652490596723f1bbdf24960225ade747bace7f8eebe7e5aab810d018fc72789ca04e1e806419e54795b7047282e0d33074" },
                { "de", "0b5342a93c2c6b67b1943ef83cdcff552a7e981c71e0fd9f763ab29a1744ee189f476856f3b0ed827e857d9af33316c2f7eb2f4dcb185645c6e7bf612f222c67" },
                { "dsb", "7a5e565718e630acea50a0af4c0c4f7c87c345740fc409332811f20bf2bff72530ffadcdcc770e728cfcf7028db6b3c0fc09b54d2f62998389ec27d726172819" },
                { "el", "cfdf49f9f619279d968d44221a0caaf05b36fcd445e9bb77745c2b38bf922ba04754ed8e0d2630a863363a29e06ef15872ef55e631f23013f2bcdc8292edfaa9" },
                { "en-CA", "9aa7c5d2dc4f51a2cbfb2943977cd5483998a787e42c412b4b4d4c3c6a04441040b9d489be1efabc899d6abde110591640c8de249919d8db78fd75b9e02ccc52" },
                { "en-GB", "4b8c5746a6b12993213dc30bebeb05c29cf00683c428319f6eac4872132d9b1308d36698509cc1fadd22359f001cdd1bb88bd7f093ef7bc77879698371fcbdad" },
                { "en-US", "2c4935dad02bc6857d1532c55971f62d31a83dfbb7a62c50428d747a032ff2ccddc8960aa826043208c8365f6aa5a7c88fdf16e50780bb32797b180421347e8a" },
                { "es-AR", "e35a1f38ea9e23cd59a0e47da9cc5750e9de7d9408d971a97d7c000e324c1301d9098dbbdc8b4d48aca2802664418ae2f01a2d57128c296434c8a7764ca4c718" },
                { "es-ES", "80688b4ccbf5005bdcd548c56e7331a19f535b2139107e5864eedbb15ccf17085c645ea0b3d361ee175f40f0864e47f61e30cbbe633a8575eb5acb699f6d20c8" },
                { "es-MX", "2b65171c8f9784be4ab8e171ad74b746e89b316a024a9ae36b5a6ebf2f702de425517fafead2480d3f24d330df9fdbc3b96005eead09c25c550715a0f19a4c59" },
                { "et", "69aa9506fc9cf79135435bee18044638b22edf84201e38404eec5705b946e8612b11c130ebbadc4d59590f1f5252c0836dc2aad59d2e332c3594786878f65c24" },
                { "eu", "f8519fa92492af7b7c334ddbf2fc6bdf1aeab2743492f9b6626500745754a5ac6f328b3821f84a7205f8c415447226387bf3b5537cd2a54cda3818c6f6d2a19c" },
                { "fi", "57aa2b8193f461ace54c542f1d4410311d12d1c25aee540ff51a8ab8d76a6ad091d8e8579e1c75e3cb82e018a4e584a0d6bf31a91a7e5add12a0fb343eb35232" },
                { "fr", "66488b49259b985a8de2097f27f6f67756003b07c544b7827cd232436bd1d16c441919c5dc9809a7ac5094e6e0cb15b8dfccc5e4057e80c52dd79014710b086d" },
                { "fy-NL", "b5938c093de8ce22ea67c757ac5c05a8a413dfb3e3df969064b0c09e62f4fbc04dcee6ab24e1ed713f92199546df6f26828acf420de324b4818e28680d94d8bc" },
                { "ga-IE", "8ed46a11ee72dcb6c1dc5c8b24cb2d10b236194b0fedee8161012ffc95757498ae5d211d28507f402f63bc9cf62f124505045cfefc4c2e5bb0b86be9b10efd0d" },
                { "gd", "e0e2fe63152dd841d2294ba3c01294bd3b8199298039871559887ff2f75a711ac0043faa1b0a1f141f9545b06cab050c16bb069df3401dda636de16d357451df" },
                { "gl", "e4e134bc12781d4e35a9c633cb6611701bd7248f50b76afca02b01c4677996338cb880c801ba4ab22f95d6b092ff3e2759a9824041854bf63586ccba7afafc87" },
                { "he", "08bbecf7e48ef78b82e89e83450b6572954b79b23a5e25e257d8a65a08f64756a664dfcf859c79705a42c8c6903ddd3c968dcbef4acd4bb4979a2ff99ff30819" },
                { "hr", "c8095fc7b97156aeb049b120bae3c064fdc0f35d6006dd450b187279c31522a144ddaacccb12776744c5d610ec0ad8d3df711d8ff723a44b7a92bf403d4096b3" },
                { "hsb", "a7facae265bbbb012991d377daec27d73227c300f00b4b8c6cefbbb51b847151dbfc84f0632560782f4cee7c5651b0ed733a69e0cd504d6da52a096ef9cb1f19" },
                { "hu", "591f19f85b84d91acd9cd81fae57602981f68101f27495dc89d27fcc1f9f15798237af67d05979d388fe252cbcead7be9680eb83b3d36e8392294ef27e74958e" },
                { "hy-AM", "637999a68aecb5d64aa4053f04f829f63cc0d9577460a5752bc8ed11afc33aaaea913571cb7553b23067ffaa8ddc9b64cb2089d7f63decc1df14949c84dc056f" },
                { "id", "26db5eef4c1de8915f06eb853ef876ca5b41250ebabb7182e40adc04539cbf7c46cc34bac6fd5aecff319846cc1f65e0cdfd31cdd7f1df933b3a5e9e17c03e33" },
                { "is", "8fa35e3a649edba3ddfa0f555d345e66e709a3484cb0dc254dbd6451d7516c1e9bf021e4b3910866c33202b7da7d25da084473d8bed3ccc193090dd3e0efc7ab" },
                { "it", "417167e844b299bde9e0f54011c0ed4fa93b1159e3b9db2ab4ca9716ec4d74c8fddf306da362a1ec8c52b16b3525f32e0aee5304559a341fd715aa4bd30b7505" },
                { "ja", "e3a0ef22f743d7f2a0ee12603264c125c25f44eae56b21c1265a4d6aee50f224be547ee43d32e208b81b7ed4dfce97f6557099cbdc2c07ec3b249efcc8c9d031" },
                { "ka", "a8019dff79b1547ec84fbf2f95d527924e170ba1232723441bb7a8135f18bcec0cecf1ad796451532d130c3ba9e053d9ac4a64bed1630012cbb391a8d5819e83" },
                { "kab", "945f2f6ee509b9687e1b0e93781518d2019fea02b3de09a125d0e5e69d0786784ab1cfd46c8d4515dedd7110799178b7a62aa3035b277186b298bea4830e80f1" },
                { "kk", "4c3b7551147647254de65bcc3986b957f40ee7e42cdc3aba528daaf8afade36db11d302ff066d9cef696c574b44bd22df5762731ff7a34bb78a73d3097690b06" },
                { "ko", "6c1b5c77db6217dc16aef9be40305f8752596cde60b48b4bf8b3687c945a0410472bd45ec1163127898ee67e99bcccea212ba1c302de6191df76661fcdcb930b" },
                { "lt", "3ee81539721204b0c52adcb8a4f4e626547e686aa35b4d53ef852112d810f0de18bb9bda5dcda5f3804bef79c1e29e6cd424c5775929fd505fced56b6d1c5b83" },
                { "lv", "27b2842147705f28c7fa56017d2f61aa358664cfdcd33b6c2cefc70db42c827e4b6433cdce1099079ea4a952ccd5a9d4f7a7ffe3a5c46d4211084347840c3529" },
                { "ms", "97204e80c7d022fb28b5c44ed5a8cc416aba40ec085f63c8b9f5cc0c35ab05ed3305e5d10d2e2a31c51c16658ae586ce72b5638e259fb2a939b3b49eebfeea15" },
                { "nb-NO", "f3b4495715222b94f878e9965140aa47939e6984346df0ffcc452697b665f0ca46fbefcd1d4b43775654e882610589d04e3380e27119e66aee8df190298331b2" },
                { "nl", "9e47c4b56960456628614d90e027e40fc0f9ccf1cdf7491ad7da8f81a2fc4c96243f5e95d6d2c746308c282d12a16ebf8605b56922b7ba867d7fca70c813034b" },
                { "nn-NO", "48ea807d6c8721621bc3c050730b75e5c309f776d9817169f8df8b2fbe5014effd1327d5f78e4116ce623174816b22428a05a22bfb011f04997d650d34d66e61" },
                { "pa-IN", "e325eb69ecdf1cb5323888b54f705e2475fbdbb6b79fe47b7c13d0698012e833e60759860002b6a7a8bb1f05319950ae3f0354e8e665c2ca51412911d109c589" },
                { "pl", "c42f6162a7c7656eeecde53decf6b1813cf2d712549644769234bc8f80290f28dc93fc681931b01756ed78f4012c2ee8a2ad25069d79619abeb8feef5a4030b6" },
                { "pt-BR", "7861eba20eb12e31a577a9f6f43217f89aa8e7aec429622deea65f0f7edae0d4ebfa5df762bbe5cf14ea609a28954b20ca18363b10090380121fba859509c423" },
                { "pt-PT", "7d49099457445d8df1e2352ef0098b4148c5251867a2d5b2402287b026408b2c881ae160290d0530b585e7186afe432520957750c9f75e218d7290647b318887" },
                { "rm", "9634f4e37d809716702124af952afd67d730071aa17f3cf3f3bfda704229cecb6fe8109c89a6076ae45df51a014061064b1188a371c271344aeb9c75cac9f1fc" },
                { "ro", "0e1af7653576de5b0aa0de40ac68c8796efe2ad568a62335826cab84986139a8e305720da3d127fe91600355a22b18cfee5db71d930a2cac1f15c0ffd5ef880e" },
                { "ru", "33f189b279882b317fa79d874737671c977c4f69ea3ca4707217e791cb73cb420cbe08b2bbaedc1d0802dedc193ab69fc9ce390833bd1e021c325cad6bd5577d" },
                { "sk", "8eab12a702f9832130a5977a3ab2ab8de9e2a25af7bc78bf673a28225a9b40ea8cc176354deeb7335d7d158c65ff43449c2a69980cad3d0abdc8137e104f158a" },
                { "sl", "54806c32bd25a4ff9a81ab77b57d03e542a0873a768f89301f02d9d55e8c966e0523b41c341248659e0a7e3bff97e6359d642742b41ae5024f372f6f346ff836" },
                { "sq", "76b4ac4c359ba312262e2b3509c21005523143699b2a9d3094fca8aebdf861b0e3ab5c0be87a6d8bc29fe635a05263498f5bba4fd640329107df6f386682633b" },
                { "sr", "f46d35ff7660faf25acb836c9da0ec1e77d7cd1a4ffdba2bee59171e250d7af2adddc50f5ca2d89de00df320b66448b113ec88364961291f55f8d175c282d800" },
                { "sv-SE", "0256020b427a6a4d273aebd06e4cd3e205abc492016c2dad06c46180ec7da396ad03f373d2453c2afbe3f0c93df11f1a40d49ccda00ceef70f3cb21dfb1dd2ed" },
                { "th", "d59d7c6f51b7afc69b264d8bcae2ce626f6188785d7a036ed0d9fb9a6afdf64c827a846ffc9347712e1a00ecd37d254d67bd0c577785352e26a567196527e3b4" },
                { "tr", "2b4c181e3d9b778321ef5f65f9d7093f56dd86493cf173d4359c2b074bd36a1cf70127bf23266ab92909227408dfc81506a450fd4362d26a2c3a57eec5bf2387" },
                { "uk", "bba2dd0eed3bcfa10eb6a388ab479cf55b0adde635876b3c1b842a202231a32c93435df7c60d9f721f3b26c5be75e4e2392e97396f0d5d69ab51ba8ccd8e243c" },
                { "uz", "ff566438d47eb5e3f40c8c326749985ee42e8f87da5dd20f2cdfbee7ecf1ca6c38b5bf2b5a50b78547446c6464f45eb9bb159212d561c47f60fabeca0e72d5a5" },
                { "vi", "6c2c6343ad5c7d8e517650883c15db3f712ab36a60422fe737c8577125787313d70afcf7641bc005ec8d84a668b54178ebd19f94e6e9258630f81991214199f6" },
                { "zh-CN", "aa5c217ddb00a74c8ee314da6a53e5d7995a6734411db369869f1722ec9221e90b6a3004ab5ca6ffd16e3b742ad22788328dc6a4def27470fce839b024bf9354" },
                { "zh-TW", "09be854725ee35ba28d14557eb6d2538980889774def8d86049ea8890ea0b39fcef5f6ef027b261d068707da1e176b982575be2c76ee4fad8b02f8859ad9585d" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.7.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "f17d58cce57466d100022ccbfd9d1180d11d203adc718d83b42f1685ac9179722291d78c98442a31cb55fa6c2db0d2cce553b38b6a29f02e4f6b659db137ff2c" },
                { "ar", "7cf1e042cd7f496fe933503eeeb27b6390707d4df9514b3d329571e692ff6d678fdb3486d79a6d4df8793b966175b9f97532011bf78461bde72f93284b9aea79" },
                { "ast", "896e783bbe9e8c0ce944ae891d58891206dbf2d23ad937fffd77c2edc5d6d36381ee9dfd71fad290209cf6aeec0b55027e6d29a162cd8fe8e578e37d050df855" },
                { "be", "7a7c8d022e2f9c2df8f99dc7ec219478ef4720a455bc6c6bc0117588039b6462d1ee0e96c3a4ba6c0879b123c8ada3d11ef7da7b5566b32e6c6b16d54b97a52b" },
                { "bg", "924fc4258ba08312b189ead12f39d1800d8112c47691f064d89cae8c1a074b9dd73b452437f2c0eeb57ecc16cfd65554bc5091f548d304a6e0c483a49d1026b4" },
                { "br", "05986c5a8441f7a2546b1da0d7de4b4d45d78b2b1eb1810b05d5333d6d6e61632397be259af81ab4261b6a56806ba702f39fb781fb7e33683b8a6df8b6c66c65" },
                { "ca", "f36c318c44f8331055f65935ea5a668e735ee6b7d57d7a9e0bedcd756262b8142a8a3c1e44a876ee7e969961e20424515cca491acde64b9cafd6569f7e0393e4" },
                { "cak", "6e0192c118f63076ac5d622a91601c267e753b39facd90c989a796a5a6ed6297ab664e865347ae1f2a807e0800546fc2666e30b1e19e70156f3122ef0a999260" },
                { "cs", "fbe6542d56a7567c428efe8bcde238cbaffbf4f690e8eac038b9922b28e257d44f056d3dab02a9d7a558fc7656ba86ec2ce884033840eae04e4a613cee5de238" },
                { "cy", "32aabb9e8e9f478d78fd21b561f0d0bbe89c04a50e43d4ddc88f77790db9f0fa541c2c1560aa406b3ce2a55a115269c9bcf60af783ac2f691f595d05ba21ac77" },
                { "da", "5b42f50a3236b85d25aa4ea65529852f20b5f70e27330612c2af4f056942517475cd37ec2cd9734c0cb84399dc7856a48bf2a29d468d709327a9b74c928708b8" },
                { "de", "374b781948e48f43a3f9541c59cf06220bf3766b2026319cfb2ef7e123f6ccfef5a47011d61cbf299bb8801297531ced4189b2f4b3a24e89398b11f04ac51c97" },
                { "dsb", "e01dae183656d33f12b3885e14267a65ed07a389cff00c09ee4d47ca99907fa86c2619109a11806d2514bdda20085d3181fb94de7949bdf2c4da7bc9b3e46ed9" },
                { "el", "990a86871a99cbe525c038a3b02a48b0f76e56cdb9f54fc6864ad22c5e806a2a69803c5430ead01560dc7136ef1330a940974739d570b45b3dcd32a8bbb92e80" },
                { "en-CA", "1a3cb48317cdc440e87feee8ec0ba3c1d429d6f89c9348998ae5922195f6ac65f40a6a7a54567ca62af0f59eee217b71276b0f9e287e2a1553ba002739a973ec" },
                { "en-GB", "71adbefc02f249ec02653ba6bc9244371a2d7d9de0358dadfd5b3310a2b7c1be2f957c360c86da4de0f7e24e3825c42b3f95c91b7bf0e8a0ed2d1447f5b20997" },
                { "en-US", "3aeda72f5cadea48bdae68f5141bfe52048c40eda3c3298ffe095ce57f61079080b5849db0f6465cfe356a2e3798d6363f963797343e4ec4e5257ccb9b56f3b9" },
                { "es-AR", "73e3724c9e75ceb645cc7067822f931a6412ebc36701829b583e85eccd08304c7185d3415bfa36f03695a69cf284433bb5471df7c37c8d5721b53397d0df0daa" },
                { "es-ES", "4ca4e4b8be30a005b6359aa3dec2ca13ae4b3484e75968c2b2eb2ef43a77d49fec7330d5122b4d5af4a5d07f9b14caa7a566f67bf2b167c0f72275663c7aa0de" },
                { "es-MX", "0dc172bfc021e3792286e6aaa1d279c2a5e11f04bdfbe1e58bd43e4f910ee09605659d81bedf577ebcca334aad133716bef26b0017ca1ed66e243d2e7499ca57" },
                { "et", "b7843b60d0a184a30dab8215d5d9a5a1685573ebbb96f52f93d824bdc9f12f316c1d3d9771bcbf5537373e287baf9401f30e4d33b3a44188264f6afa4ac3317b" },
                { "eu", "1e1bbd5e6c077f0b2aebf4ee5f75b077841e564a469d67d0c6e215fd74c3ecf8e5521bd2e5528e60feaf1a181502d381d3b1819e9341fae7f614f631e5febcbe" },
                { "fi", "d49e8adb90ff3c2b7dd63a4bf8c5bbd834599b05e4bf1fdb1db97bc0062b89dc0b6c1f26f144671d193e8032bb0c3a013392d989c600c74b78483ba24107a5de" },
                { "fr", "5ba06ff81ec50652015d7a9786997de7c001751a81b34c77c8f6f9dd5627d006429dae8106bd30a802588abaef3d52858b4dd71ef8fbd694b04b0a52bad9e3b0" },
                { "fy-NL", "bbf66eaf692fc9e8167f8a4649e6d18b881923f55cbdc9835e5525432dbed96772043bf54c33f01ee4011cfe5eca48da7c2a0ca58ca6dd6046ce113c96519f83" },
                { "ga-IE", "4276072dc6a33cfac837338f8b97345a3375289d6c4a0815f5b37ed8a03a81b9e2bfaac282a1066a0a39aec1beb4d4a01db0d05c09a457381eeb4f7f191a9ce9" },
                { "gd", "ef213e8a7a0b2b5cb688ae89ec0e11846a6b72c92b0e72198edbe30cf63a680f8ffc7d3b97c9ac761ecc7cf318e146b53d5dbb133d2591775383b035210ed6dc" },
                { "gl", "80d9fda23093df5cc4d0e4422eaee483521ca35202f5de6def6fea73c61dd70be540d69cee594d264b60ff62a528e8ad90e0357a9a9aadff21bb56e515f04d67" },
                { "he", "9b71a800c4f7837b458895ddec9cc01206ee038138534a15d6a7abc845910006ed002bf8a030e9acca84f7520a8590718f3107f9c5ccee2f25626025efc08e11" },
                { "hr", "c0c4817327cfc00d1de71ca01ebed484074ec6a9fc7872fa74b0a9bd8c4f5b89041ba8b49951effb7816faff0021840cbee5308eb78ba61bdc84688eaece1efb" },
                { "hsb", "abe72ee480e302fd24f02076e0ce5a3b8e706e57a93dab53944859120a73400de61fceb7f84f68a2a2c3a8ff01f4a5fe4c8e815326b814f3a77018debd014e38" },
                { "hu", "c8f9c3c75e269506b21095f4a1edae409c37e421cf9e4d92d2e831a211cc3b63eef4daea07fe697fedca50adbc959ab1f5bed2dff91e6e047055773bae182178" },
                { "hy-AM", "3b0ab26bb0b3ce71813ece60fa77c587ac7f2fdcf03305f5030638a78a76ce10e690c37b1514816f7efa215aeb9427116e06c21aaee97b45a0f74ffa53bd1a34" },
                { "id", "196fecb0ca753b0a5dd104d7bd43a33a8175efa0b4387e3a7e1ddc9b3deeeaec108415d962919cab0a7ac3e71a5f802c11bee30a00c02242fec83f59e5e459de" },
                { "is", "6b92c7f9320af0245c7d92d8241cd9664bdf3d596bf38149d0728093b44ebe033b21f8a35970655bff5445fa6a688fcf3d1a1d4802a9ce3257f5c7c56a4b4f41" },
                { "it", "6b56617437c16cdfe37a9f2e6a6711183716a57ebb1b8d2be0a235ce9b9bdfbba1ceab5dc8fce820715e7b3789944232a3d238653cda86c3780fc7e90cd16439" },
                { "ja", "8f61f6de124a26f79fa44af9c11019ebaa6400cf78918bbcd85d3aa5433c253f89eb990d9f7e8e6e403d354fafc0d5aeadb88fd501c911b6a042612dc37fcf85" },
                { "ka", "4552c5bc30b8631d66a9b0dfcba61678737386cfc9c07e0e14b216c73c718e3a9147d85b4b40c5d570a76ffb7fee2f57480dd1df2a57f87aefc8274d9f717a65" },
                { "kab", "ba353b75ba9e34e897a4de834ba09ad8fc4bc9235c3b0351fc125321fbed542365a3bdcb204566877309f7964bf5b81d4798f99e8920fce4426c39da8e59e2f7" },
                { "kk", "a1b939186f736993be1707fc3639ffaea0ab64fe545861265e63d3be3a77603e3269cfc81f025267a845832209c4fc4101bcc652965bcbdc812b69f3500db007" },
                { "ko", "66016b0c8f976bcceb5f0af43913abcedc899a64bcb9ca355272f9b28dc31e5d3da7a4c888f29e4913015fd06fab52bddd1f365091bdf213a5070ab4ff69ba81" },
                { "lt", "774eea4f826e37be2a7c9e029c12a935faa6d99e65b3939023ffceea06b47d49aadc8a8d213a2b2176de2758f564247000f558a556b45032edc35e54aea2fdfe" },
                { "lv", "b9f2e01d47d544132adfe7c528db61176ca67b77a74f8f0a83661941e32cab5d49a59b9f70d9e04f1da674f8efe749d6b641484fbf84e570247b0f9f701a1f0a" },
                { "ms", "73e3754e3a5f28d4b6615c5caf5b0a146ed40d793e790288e56c85858c7dbce0105dd76332ced0eaf5b0ec5594b382cd1389c4acc07085dcc21bacdaa1ed901a" },
                { "nb-NO", "4650613ef2706cc968e03510c438a54f57bce7f6e49e79647f8d3937f3c9fd8faf9696adfc776f87057d4bcb0b9df2dd4ca8644478dcc3e7529c17c8163e355a" },
                { "nl", "33ed1745f18e363623b2e690ce5c19f3d2a14a43a6342b89ec4099765c2d08547f4c1e08212385168343cad54b5e20eabb23e33848e0326ae6b435d70da2b839" },
                { "nn-NO", "2ac881b7b5b0583c182afb35963920ba5d8d36bb3dec29f0f7456d2a22dce91bd8fe010da67602cfbccaced56f56f8f96f2649c2f9a9d371864208b43ec94500" },
                { "pa-IN", "2850227b02cbb2398b673fe9ef1983801cbde0aa22847594b0fdf3291489e23f17881d66a4fe94cf1203c1f7ba3f1c50335bc14f0ca46475d177778a45ab8665" },
                { "pl", "cd0df0b8d1e23f4543025a730adf7196d95f0c9664f39db9d9c5f0551507678d40db040b643b2870565c78578d8c531098fc48a27c7492b8d03b81d37fe0a2f9" },
                { "pt-BR", "aea90e6864db1938350b7dd9270575a9b474d7fb0162765d77b25ca9c17b246d3ba8de56852c5414ae15fb6f580aafbf4d56914586bb627b9a9e6b20ef9d7e9e" },
                { "pt-PT", "1bde61c89275fa23738196fa751bc730f82db62394e585faa534bfb1c0311a216102812345ebbbc1eb3666c30b727e88a0db1ab341c4916915f40a2032588a46" },
                { "rm", "ddff2f5226e63743feb8addf1db4d27e846ea3a3f6725d55ccf1b195c80daaf3e69300bc04abe8a97c54827356e01b7c0a66acd0095cb3e54e2d011d758097c4" },
                { "ro", "b0a476f3a67c9a4a4b3365fad8a9d47233cbcee1619ad0ad1f58dd1da78e31f4ed466efaef93f817746fd8ccc4fad083c1a3583df6fa75b8ffd2809570236fd6" },
                { "ru", "bed3b5e176573a5ab1391b5fc2129074bc1f8b10a8ac948d8974707f36c742595d149d58f29e152f867be5f27e0406fcc9ce857261b0a3afb5c86dfb07fa080a" },
                { "sk", "2e334a3b2961bb253d58d9c02e5ec60833beb7f069203a9826a184a9144c9e520bf4e834c44a68b493bd148af89d641a98fe4f11621ae17b95afdc4099235046" },
                { "sl", "f8882dcf112b78d3d7476e6770e32b05b58a779afda4f2c5fedaf215f5450a1de650f9cb82b4a6ea304c31357ab7fe48a565294d8785b85c3ddf3a31ac024bab" },
                { "sq", "dc0b981c68ea32cd5beb5ad7c04d621a73c432971a82d1d0ffc1cdddd4a3878a79f204ab74a776fb04b1f9900c4fe147adfc6524b0280b096e1a31351bee4e32" },
                { "sr", "e40c72e90dd8430d5e9ff37ca408cffc697df27acda385b0c7f894e81b398c56b78f174e81972f1b8c8d5d0bdb538667b159e25c6b1727e57ae20f2b491c6ba0" },
                { "sv-SE", "12c4a9929ab5d9bc16bc57470e499b266f265e6d46e54bae6eec89828fc098ae5774c07c852ed83f80fb00eb37df560f356cf7e6d7a8cec5cfa7d16fa4c4e6ab" },
                { "th", "824262145231928463fe05586fb967173ecad3dbdce176519033066de5ee82a8ebdb95385e57310d53b9ded4ef63c4170894172fccc05ae1da17de2ee5f524b1" },
                { "tr", "838c1ca38b0dd748410b351d28b0fd72877707bccd3b9a418f7a31ceb3ac1eb7ed0673ee1277ecc6caaafb6a77beeabae9a7897e1cf2505df7c4fffe5d99466d" },
                { "uk", "0bbb35d67176a875d21b7d92dc052fe5d2cbd3be8cbe324e2ed4e97ef9fc3038c1df965604fa493619d0147da32408d40ba4a7e2eb4fb6430643f3c7660f47f3" },
                { "uz", "bfef5ff625bb8153d64280fd2c3c6f163cdda8a3935c1e8403c4c3ce87d04125f99d817b02327b3b03520ba712871791b7d66d1c89b26062f559f01119fda4e1" },
                { "vi", "259e32d64c6bb41a54afd175ab0f823f276e88f81dd23f6f5e56ff10e003639fa343868a8189400b479ea29b8c12e8d36f65aae5d35cccf86eaa148ed80e66c6" },
                { "zh-CN", "51b2e949d28b8622321055b4dedff3741edcb980bd1c952f73fb6929f8d760a7f4c8bca2f4e3b73e680a0487612a964f0da464193fc0ccbf42055f78143c0692" },
                { "zh-TW", "c7314599ed8bd581890d43f3361e6b7e30a93bc0f9c223d74ef85d3d188dea3935f44dc90761fd2052ba8240687e3e177cfb1bf7368859b8c31f915cdfd01cc1" }
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
            const string version = "102.7.1";
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
