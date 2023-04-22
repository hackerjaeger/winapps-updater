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
        private const string currentVersion = "113.0b6";

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
            // https://ftp.mozilla.org/pub/devedition/releases/113.0b6/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "b297a70a5b83a83d4971ca23c2665e6567b92242d50ea28feb62ce5a6176bd375d8c3adcec5a2db8700307e51d57fe9dc18be21f7832de2648fb63f33f16877a" },
                { "af", "5d84266b2be8aff8ea7523ea2d5f43eb88a57659a616d37f07f50716cc4f92a8b2dc1d370e15bfc6f8bf51b6ef22af1f40c7bdad93e0a2ce076807a36405a969" },
                { "an", "df64ae4f5e9c50163afa5cd761f7eaf5002e3251ca7725e8e59bb4949fa62ff2a74f64b8a2ad8588dfd9a057b148114f8715eb217cf628f5e679c99004710345" },
                { "ar", "628b1d7505e47dd99874af1663ce453f8af98aa729bd12e0543e1774c374d73d1ccfdf53ad6604d5e6818e96ed4ae4e5069154e8440cc1b049bdb254d7a8beb7" },
                { "ast", "ea998171cd01194747416367a80533a3e885b2be1ca2f043b6969b327178cfd6219ce7cded167548cc0fd91fb8bf9407b2c8aa8d29800874e8cbae1a7bfea3b2" },
                { "az", "f50054a199c3cd961d412ba0c3571464e97bb0d14a2800347effee7d688a652d5213884585805261ed7a86a8814e435559f699d47aba96abe968b346917870a1" },
                { "be", "c75a626d0b659afb03b384d5d7ad220f217e95323ba3fbcad5c48d3b9e1c01a0a7153466696bc1979829017513aadca76e50d895694b7b290e4e7ebb02b6e303" },
                { "bg", "965f13a36ba34c9ff82ed30fcb29f5c1d9a9e89047cbf4c1d6859f90d37aac019bf112b2d8e56a1e72e228bafbe64364dd76a8b99dc10086ef529761e6db6d5b" },
                { "bn", "54bcd76daf730dadc31a4f071be6316e2a6c08b15a4da7d74b8e3c74c4d5f21287fffe7d6ae74dd3086b6466e05f6752a73b5b1dfeeeb7b85ae4961c34bc20fc" },
                { "br", "dbddd25cd60d08e270b108c6eabae46fbd69f41eb6c913425d9c4127a80a32867147e9f1c94f97f0c154dbd9018ba2f5f3f20e12191ce794d0e77bc315e8734d" },
                { "bs", "eb5b446cc2589bf61083bb49a77faadd140a4b5a516542a536d1b4efa3be425cbc149ed9593026cbb9476cb88b186d803f66168d756b579db164647d3d080e13" },
                { "ca", "f2590c025ef85ef813d7eecfd62a681b2be7ec0477192992329ca0baae08296e8ea092f3bf286ab24c056e5d3133ac7e7659a55846a2a7f65fff8242d0f4b021" },
                { "cak", "75dcc9edc21ef19574bae1ac6ded7a9bd9b53e5922e0b1086ba95204e05df2f73088ebfeacf633e0180a9d954ffa05fc219e407cafe967c9e0d9331770aa2e3f" },
                { "cs", "58bf0ce54430d8c2f7819a470c5a7336ee3338bac0c99d6d5f1b0efe63a0029bac7b410794cd93eb5260a7cb12c76faffccf8cc66beda00d98104c38634bf309" },
                { "cy", "63b5e95dfd53b9102f78dbe85ce4a3ad7c021775a817e3be34eadd7dd75fecbca51d81ca14f9a7b112fb49de14f5b32dae9e9f1f5f9f3d433a489f48650734de" },
                { "da", "9573e76376ed43f86c3d97a0084ce00dc00c7687217ed85a942c4cdc1440ea3b140600d14f87c3f0961478e4890a37de5019b8e2c16ae37ce551a1dd20c1b56b" },
                { "de", "b9186ddeb0af711f796f4bd20140023299f3374ca3c0f975af2323e8a84657cc360965e29d1235a94f86c6d3daccf6dcccf0e99ed91665becf1714fa38938cba" },
                { "dsb", "eb240c581a6c5c833c4dfe6e8cfe10ba5b6d2dd22377888cc4133d368338be60c155bc4134d77bee47110a74b9ddc1ba8ae67e1374a9b12feb319d953f4e9246" },
                { "el", "ecd36c3a24343c9f7280435d6ad678c802bcb60429a462dc0981abeaee17b0556096abaa9aba9f5f6f964e4f1e91c657e5a2293555cb0293456302fe9ec4a7c0" },
                { "en-CA", "77df36dcbe403ab56cc6abffc207b3358f5133480b9e489f056005545a4aafa65ad0a6fc8063667c32e1063bf748cebf35afe96068ab2d76c03863180560721d" },
                { "en-GB", "1e2dcc9e901eb5de0e85344b13dae02bf694ccae1cc3213827dbcb093f8963de7154b92ea4c73087429f31902987011ee77d363d1a1d1494d8500175c3a28374" },
                { "en-US", "052eded839a8b5ae05be5aa61fd327f41e3a69d1ff0c906bff941f29c337548890ca09bf7ef449c81efc6a8d40d5b26a0261d32158464f7c62bddc512ace5bd7" },
                { "eo", "4ac3d8e10bd850cd235a004a7c62f4d26b179d7dca5ca6aa2d13af9225612af2ce279d04967b070a95c95fbf285f74c0a58f604f62adc32343535a830af7ce89" },
                { "es-AR", "02e3e483af3b9b9f6b56a5edb35657e05a887f508d8e26f65882cf84bd12b82fa9a7b3e34f587a7c6a84cb86befcfd012cbd9978fc3735516ebb1d22ead5c6a5" },
                { "es-CL", "b208c71dc8fc6656cb1f70594f9adb6a58217a17edc36833d948bd354d1f232e27f5e18487499535e6c7b530e9c05d1d26a99458c06bbbd33def287ded67bb1d" },
                { "es-ES", "96422632858c7fd668d439b5cc3c8803d1175837cbf9bb7714b678f6cb604cfab4fd6102bc33ecb686cbbd1f2c33d0d8925c5030d6571084616d781b082ec222" },
                { "es-MX", "d646933165f0861aa154ea0108829dbcae1e5f1768b105c99918db851d2201e22a20e446714cf8d6e3cdab2f1e3ce01fa91bd19925dba09d48797ea9f7a33487" },
                { "et", "9af60b5d17cc90a5146ca8803ba7372938eab52bb6ea6308fc5a1c69a179ce67c49a45adc9b40dfc565c24cb785b6bf348065ac7f54984edea4e006b0616353d" },
                { "eu", "9b956671fc1e08c3247e5d63aafc9037de87dad5de31d0435a1fdc0e7a551eacba023e37f5f1ab07bd925248680150fd3a42c8e5ba73de25bd2a2550c4daeae7" },
                { "fa", "69b0270bf2331df5a32e3adab0ab1091ab641a1ac0f5014ac5b3534bcdddb39f4e31498a4687cc579664faddec117314c3ebee8d9ca8e8494a73ac3697f2c5a0" },
                { "ff", "543f793d5c92305dc9e5344e76cfb0a8c259a4aff6f02dffb3275bb059e663c9ca30b063349bc92f291d9fec78b210865ab2f545121904c596e79d69f8c1a815" },
                { "fi", "12185e00ecbae948ce9ed409b210fbe0e5af6359f77194dafbbfb21ceb1f1b98e6f7156e110632a87d31e7841f4f91dcdefe89f33be057ff6c623c47e4d77ca2" },
                { "fr", "581817c4037893e112a9c87d63207ddc1dfdf2b835265ccec4753d4d21dc09e73cf0938d2722770f8d29a738e4ac5df1c132ffd6b9d5723773d6b01a125b6579" },
                { "fur", "2cfc5c7f7d3afcb2bfcdd9512d54b5f316debabf4364a588708e2180f48e152143e95d196393c0e16b84f4274b57d12231889b27d6dbc4c3b7272df44d6b13f3" },
                { "fy-NL", "eba518743f4f4cdbfa1fa5ce53dddc8bdc74029c78a2476f2c44e918cc35c0108e1bb67f07d5197b2ca199d7edc856f8cbefbe4f708cd380901582aac5ac3a45" },
                { "ga-IE", "dcd644439202fa026595bbbcf14851226f907f7996a86593c087e262119156475726524deb172187b09eb4a0e0745f01515359bf1f7d87220b2d74a34678066b" },
                { "gd", "bc45d965b36a94fe32d0ec23f757c933b9902fa060d706abf1856e660e2495ae812475f7cc2b1b9cb21f6401ef0aa9142ae9eb65e8629969206d60aade3c3836" },
                { "gl", "c927ae855741d049f07962f69fe8bc04ebc738c56f13937ebdba9d71a4c4628eaded22041e5cd60736690c3a93b84d4e2a0ce58ebf66d8a2ce7fc8cc3f1404ff" },
                { "gn", "67598247b3456320793826189f920ca76492213387409682fe8a4c83d278e41ea59dd2cab09cf8e8ddb415eb2618fde74a72700b7e02532d35264491ec5eee4d" },
                { "gu-IN", "aa78907bd8e93e932396e34c1b5cb3c38a345e88e50c99ec3b241c0a22f21f29ffb26c3f7eac505703b3e070015edc1ec97cafa98977b33b70ec717706b06c3a" },
                { "he", "350c2c5ed08831dc8421e51ebedf26cb0d52261af0a73f3c72126b84d1651c98b3e3cc932ba84e9afd5cd6d28e553b272e885abe2eef95ee7ebac833dbe595c5" },
                { "hi-IN", "9dcf660c8891dd6f275e6e8633c7d21f79f597fb8ba66680d71781f5c5e63d1ae25c88c2c56c57a6e9ef6fe6d6258611593d05e433b602a5d2a72e7f3c6de3fe" },
                { "hr", "af97f1bbe3c6de0cf0eff67ca4c35b1dc77e59eb1f5894a867e000fb37bc181a5be6e44a9af54eca0ad24c3a8d0f36955cc9efd1346a35629c90f77acfb89056" },
                { "hsb", "be654a925de789dbc1b9747f61b41d9ed216144b02b15608a35341fc38d95b770142dfa02f507748589fd7857242d7a794adf5c41c0f8aeabad97078e4b7dc55" },
                { "hu", "f84e6774641ccbac9c673059a2e695bd4d07ae56dac414010791bbbb76b986e6749a0ffe76299724f971a1b76117e6c2abde5b2873418a5e39892948064bd6e7" },
                { "hy-AM", "69222b6c458c58d796b54655924a3aaf3e6ae9e102dff17c7c23793b5a281094ab6c70348c5385b8e334d790d37c19d0646c6ba2b96c3738471e49f8ef522cad" },
                { "ia", "6dd8f8cf0951f7e8f60cd047cc777a01b99e63182edac716c241c1e9e8647c628a5f03b674cb22003034b5b283297aac700ca60488a2d5681f78651bfef9a476" },
                { "id", "417e81712bba9a23a36102d7240823fc3476a2238f09e257d44d29c6b0e0c2b7e3672717ab5b24631e9c606b5c9f5801c9bab1c4ea78c2564737a3fb1bce39c1" },
                { "is", "8c360539e21fb562c0de7b84ceba123d3a0edff8e5dd4f85a1497ef46ede8697c8ae3eca0a2ac4b6d699ca02f8b449f7cb68d2748675fc7e19149f9244be116e" },
                { "it", "25ba2941a88116a1e8fa6ab55fc6254fd56391c7aa952cd695c66634efc630063a833ccafeaab3e24a3439c581cf1f140d9d8ee4028ef99ff6e3b3c112692386" },
                { "ja", "d5ad53fc3f4b9cd36d5d432b0401f1903e47fa766bce9acee1b1dc28bbf8cea915c2f354ad930a4c2a7e44cf7f9d28685c124a1d2cfd168983016a2ee4e24005" },
                { "ka", "0db17863000f59ed1a56e4d0095dd83cf5c546fc83b3ba9b7fdd459b2df88d7620e5c51c6ae6bc4b4242303a428b869c3242db4f98d7b5f1ce1278f7ebbe3f5f" },
                { "kab", "26225e126d4e688ae111782cb57c1f499903f5d0f3dd98ab7ce293eb0346e00ddfd985a606368bbfddb12c90897ac4a802899d269fd6d6522ca93d7f321397c7" },
                { "kk", "86268afc3e4905222103982636fa66cabcb28104801fb2c803bab57776d0b32b0dcb31e687d1e20029a86f182fa2c839cd0949e117d8a4b11aeffed625fef658" },
                { "km", "41ebdce41b152df1ad94c142811e6e226a589674c0c5807fd505ab56c2161bdc86df2a747e5495e10451f24a1938fcbdc7ecdb40b859f9cc659354379561f5c2" },
                { "kn", "4fbc0ba6d8774dfca67bec5a1e34c0c6b686f76049b17386f7d943c7ffa05f1fbaaf669c8a0c5f00154f20e124c8eef99af1dc5c384d5d1df2eea89feb33e7dd" },
                { "ko", "5cd142263f3c0dbf5b310c73a960a4f746a2078bcf0e0a1951444e91ffc9522cc2398eeeb995daf3eae5e5dfaf101d3486ddfff39bbe15ce2b1910207c4829dc" },
                { "lij", "46240c2af04c67c2dc8b161b518266de14d02026b431d97161ed355bd9debf54a7497379c210928170748458293b9306440fe3ef41b1fed3d5b6a82ee830241d" },
                { "lt", "2886a10184c113c0b1f5030ccbffc68af97b1813c2877bd2e245713fe9ecc51c1a289a50ad6fc67d050068a6276f19e0fa3d46e57bcf4f1a5ac84f3146d6128f" },
                { "lv", "2cf74491d071b0e02a375ea9262f9109c85b8e072d2c0ae74696f473bc03f2f2be732f0151c7e45a4a059ee5e9bef876baa3415a25a4aad16c0acc326d666137" },
                { "mk", "3cb12995f04705d1e7992e1b28706b507e6abcbddb289273614773bc9912ca4b36cb35a0c27d5a3650a4fc023b447a7b2ee242b48a819a968de51bc4c0c89193" },
                { "mr", "036b0f4e107ee2fa1cf4e3c19cb6fe68d994a2bf6193f383ae60c9921081990fbac11b495bc4e72559dba1607ebfa8c95ecd90c2d520a943716d035a3fc3774f" },
                { "ms", "f4eaa840d78d6aceafb6453b7595447b8f4cd22b86a3983146eb11a2f85565b19fe922d2a0bfdd812e2fcad113422d4d44bfd6afe3302957c032587616f66108" },
                { "my", "8fda8ec7aa3a21f2206042b8dc9dd7f66ad114d7a0cce54eafb63275bcf370220044565f8953089c709811e038d373e39c21172810df4ae1fe6e6b9ca1ad21f3" },
                { "nb-NO", "d1a1a8689684434a1439d3b004462f1f171182727a596c7786dd2596df86c647b33454343acae28e34100fe8e763e0f92e938e2c178b6bc9bf107a830620ca2e" },
                { "ne-NP", "0287f5c4defeea8e6e1c790a6634d0967b6cfef0ff0a678a6d868be5bf5350902f6bc563a6fe23fc1bc93821189d836dbb869c68f719ac712391bfbf9e3ccdee" },
                { "nl", "8870ff0541ada9e17e5e00ae31e730db56569e0c721c137175e7c48d8131d4d80362cde01027d29367e9b24736bfc09672a712f73c502425d536a5273102a669" },
                { "nn-NO", "24d0b183eaab7120fb6fe4be39165a3d7974f3c09ed80ac1d6fe850bdc302780ff0e16aa8cdc4ab555e6fd3fa85591bae1e6ad2d5cc87125bd2d1e7d53407019" },
                { "oc", "3db8c4f4b6254d34fcf78341237c159c9b3d3586355eaa644d16408e06fa3e2b5a97bd69d6a00450ee8b4321d58a126cb63d902875637f193a0a5db7b6a8091b" },
                { "pa-IN", "320db4c78ba4399f0fb2fa6812c4aebeaaadbfb36f55578eb17ccddb142251c14b116234dc782dd92e85ee050c830147d23b0b4b2f4c12c3894bab114037b099" },
                { "pl", "db616b3583bc3730644dd2e8760a118f8b34033288d0db3e2e2a9ae72566c25d140ba9278b53d6d332c03b50fb81523a79ab281c24a347bf62f732a56d2fc184" },
                { "pt-BR", "ffbf9b38b49cbaa9195d5af9235378a4a048cd7136ed2d652b0f959e1f5820f6b7d7c5cca84d536ea80871d3233e868b523f09df02f05426cb6415f6fbee3305" },
                { "pt-PT", "8f653dc224b4ccd63d6590d733c3386114741873e7c692c8db411f784e4f0c09f5d2b3cdd7b4fbe0887bd6d500549d81d06e0f6bfb72bd3b200073d27af4f6cb" },
                { "rm", "802ab74b648150524d92716e7d9097206287c81bca09ab4c077ca8b710be4e97c0b7cc9d5539f2920d91371ffe7cca9d4069699f58dfdb306d108cce1524f103" },
                { "ro", "e60567ea643acfd2c0d03276387682cb78e5f7843330243cb3f334b2f3e42e38f54cf1f8bed6823dd0db6bcfc031e6d4ae8125425d2e4c6c580a209828e1cbcb" },
                { "ru", "09be2a35f78bea08561604d2e73db38f13c0aacfd8673a641bf0ce8bb81b836389327636da5554b9e64632cb136f4041bbce1d9af30c93f456b30272cadd41f0" },
                { "sc", "ac21acd8b187887acbb01fcd43f4771218ef215cfe1b995fba54068c2f9d4a57708978b08d7a38ea37eed28b65fe5b7f24718766c3422a2400e57bb023deb3c8" },
                { "sco", "b60a2b0f926a2e2ac09612bcdfe20358ecf6b25d2ca774aa37eb004a37453eeced20a1ad71a5e36aecee8ff5372e30da64b853c6d335bfab27c5eb0f0083d5db" },
                { "si", "a24df522596e3e401fff2a5620387786d93c6ed7c161f41865890a3fc16248ab8c79aad86aa1e885356cc3b0830df7b97607373062b812207920ef246707de19" },
                { "sk", "c04085bb3d2ca0e65d5f9c8f1fff2412728eed9d7c53bf2f51d068b1d3a7f93aac740e1e71801746dbe5e117db8b6d1958696546e4cd95d15c38a66f448c3c43" },
                { "sl", "7263c70e029a46a77838b411bf24d34a382691ffcf6f68bf4dc4ef58f4a6145bb16c2dcf214e4e9e644b787c081bbb737483a65e353e121786b8985c6c1c9747" },
                { "son", "37974b85a0fbd270d377a6428f3fc69e6d442d3f08f26b7a6ec96bcec203d477712a6ca749155d20ceef268e225929b1d406513a2d59c34c6904df832f68bc23" },
                { "sq", "511a0729e8cbc33a686f83b535f02ec692c654e5db3e31baa499f6cfc334d738944d12167061f2f1b723b18bec698787cd93fbb8a3a0d9f28df9a0492715ba02" },
                { "sr", "f8d129dd9af170156351e6e6bb8a2db0167b6c80bdb609ab8f063cff526054b804d7e5727653eb6cc96db500c1942f9d9933e3983dfd7c6d0e753552ca859d29" },
                { "sv-SE", "35702e127e805c74f96eca49444d861825f4c5b75fe9e45eef72a525c0612e5c6665ad59fc41b945bfc696001878fa3a6ac91b7fee2e639549b6a87dd16bc372" },
                { "szl", "ff733190206a809bce6fa10b37d0d9cbda447040d1de7b7eb1b3dfe213fc971724a0a3a5d24ff57218af610d0f0ce5278ac131c7f56aea02e6cdda1e4bb1c6f6" },
                { "ta", "317592afbdc51158292b7c38e6e8b4866a474e7dabb6cf3449e40345e67f0e66f83b2e61db0d91a6ba11ca917306d34980ac06af3114b13a386cab5665570e1d" },
                { "te", "9e8f7b458329d0f77de7c53e0faea1d4cef12308bb8e693e323831aeae82e312c5ab2030af1a856dcef56b9fbbeaa489dc52703774b6684a0fcafab12e33d2c3" },
                { "tg", "a017dc477a9bad340d0b6a337d927de5d745ed6b0f206665eb33e9c93627363c5e4a5f225cd3c6443c968675c26d48cc294fa7e96d475933620c135f5c9a5ccf" },
                { "th", "f5410894482b725071cb9fb15db3d6f6f39b444b8c72909fa19a40bc3f5927a4c35f2af961fb5260e17fc129fb3fba0b4b20e05a28d00e69bfb34c2f29c1bcb1" },
                { "tl", "ed66909145ea2628fd8168c9aca4a8f88f440a0cfc5044bf84dad50f60a79c99e68164dc7b6bb88c8d44580bb8efc3e7c08a9e8f97e2df86018b472d59b32c47" },
                { "tr", "7dd905147ce4884038e7c04dfdfd12881fd63f80621bbea4b299610c9e13c137549ccededafa550aab397f6b4979223b5fbe9de04691494fdb4ad1d8b30ada8d" },
                { "trs", "e959cecaf53a7cd1c0ddc56c2a2db77e45eafbe7ac92f43c95d5bd377ed049c545a44de61ea126ee8e90285353d753966e38a827d586bd14045252e8d18c642c" },
                { "uk", "3fafa06aa1c25af805818c10eb5fb684fc2d1466ce6176fed91c66abb8b0a7c7dcd307ef2bc962dec0914b7732f522f109daea639dfefc89044f5598c55a1a9e" },
                { "ur", "82d9dac8f3449c1314caee0b8ea04865f46fd6f479698aa012bf997f2adfcc066bde3b88839e38b25beda9f0e2ec01c7dfcd95523de613df1ce0c055d3f54007" },
                { "uz", "0b181bb682e5d301d8a71aa1c26643a219e77b2a17a56db00466efcad82b2275653487add8ceb516137df521ebb0f3da3bc2f1df8e1e8cd13100b2dd50fc256a" },
                { "vi", "715040aa73a065a4618f19599dd35003c1b6d453db04f979617d67457d7559faf906ce941526bf769e3581f7e0a0ddce200b526d49b1b0b9c5bad111a3be0d21" },
                { "xh", "3e61e2afa6af61d2163b7a26e71166e18627577dfcc2ce2aafeb3ad1af972f0ddaf7ee818aa21ad14cb80514478afd37ae66d2e3ff4e53d3876f053ccc9075e8" },
                { "zh-CN", "c14896766fe72f4869a6f7d526c3fccc3f02b1c52b60a1483d7db1b6c16da49071d2ada5c40181c7a0bc11323cd0168c4dc7e9056d7c0e93f4254362d889ac1d" },
                { "zh-TW", "ad0c7a61e68589bc9ddc53b369dfba2cfc0ddd0cd6e7af3f306bcd62761d354d7d03a8158baa605ee2e71ffbba92786e88c3b1799535274226a401ed44f5e571" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/113.0b6/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "7e44b04028de3919423445e13760aad0fbf09644a5bfba9289e82964cf8e236342f5325c31bbc8d26693c30270adf69f5edb8073438896acfde99cd7c0a916b9" },
                { "af", "74dea00cfccc444570ebc603bd719a4b406a9285360db67be2edf024e65acad980a1000c1bcb43aa2aa4d55cec0e8550c6b19327152ed606b9c73aca5c1a8f40" },
                { "an", "8c99aff8c07d5e831fe383ca9108c1f9017b23f984db4215a9189f847b2e550a02c2df7c2bd7a45dcf72a9f0cec7e7d58f3bcd25f557688bebb3ec850662e51c" },
                { "ar", "ad7f11f1b407172106be719bab8770cccfde4603ecbd4638a28ff7e22f5805e561c12041c01652e389fc4b48ea1bc4223967647546d8c939b9f33fb03c5a5e9c" },
                { "ast", "bbe406c21afc10528b21c090b5fb7f371bf3bb5cf78d9b77138cf887d6b2cae8886299d192f121cb00667e94db0d6ebc0f88f421a50d438e73e2f4b688aed8a7" },
                { "az", "b94362edcd5f2ca9119971d0cd84c9f548d8d3791e2137c009cf647033842e4a4bb48e07a64feafb8c48d4392df34d63d2d0a220cdbc97ab41e06a000b022ab1" },
                { "be", "f47646a80eb81885352b32436679dbd59392e89abd0c26b571453d75b37ac472b16533560d49c13d2536d4294e3936a25d7d1c56b37c00ce5c68e648c3aa9276" },
                { "bg", "01f72a50b6483b30dfc1f2e410e4a7f4928b9c4ff1d2ad10a0cc754a39d360502819b9e73a8c6da255d9b7254bced550d319d5294b40482f090a419f4b78a642" },
                { "bn", "5593a2e0189eb1d88bb88a30e3bc55a832779503dcef80a23869697846a3b9ccd8673ac6a45ee426f6ad0592c065951262c2187ae1bb2d2bf06b3451b417489e" },
                { "br", "961d6192adf175a55c02cd8127a67a1faeeeddf757ba5231a5cbf3b5a696a6f08681e90b30dc056c29f9e6d6c124ef42c7e0b7ac25fef97a7def03fcbd130192" },
                { "bs", "d6f593aefa67074ad62d0be44388ba6e108100c28afb75e2521fae0785aa23d88d58f16d63ff1965a65e2e3b68dbb921e60e970289cdda42714b379bd5e5d585" },
                { "ca", "b7113991857a6a1f7decaab88d0a1c2e12b610149fecb3fe3cfcb2c6b0498cb5bee240e5b600550cf9cbdfce26329f8c4c1aaa5cd23356d48a64ba2af798b6b7" },
                { "cak", "3edff40dbe886e8e9e3d2f6de3957a5df20559f436cad6af0967df798f00593385ca506835c5aef25cce027f990531760d336001ee8a82d2fd4907fb70187960" },
                { "cs", "e900ea0159c1f63215a8d4080d64de6f9672e7dea16265766c5eddaefaa6761ff885d773c47f33a6f2f919a4d87dbc26a9a15e4ec21f23c69eaa34bad2969ed6" },
                { "cy", "4ccea2e4429eff3cde657006f9ad0e344be62a8376ac775e57428e98eacaa633d228fb5073f30f4b9c29240e6c887e533bad4588b00a0106e498d94b0313a6e6" },
                { "da", "7c4494ec598924a4509f09b0b917e365c3b8625b12514919ce376c6b3a8c7a555f343d75c9f8f6a062d6143286414212b089995b25e32bdfe98f24859e0d78fd" },
                { "de", "910428346663948927e78d75c01de92a766495169c8b9718a21211c90a6e851314a9a7742c1988e1495ffddcd59118aca9de966566dacb841e600311457588d8" },
                { "dsb", "bcca0d8fba1b56064f5f1a25d2821d81068c573c7f3c96e2bb8e02e831b8fc1ba2b9d38083627d59437e6f500a21a760c942ea2a48475f2ea12587282ff3ed2e" },
                { "el", "256776b393105c8a5a501f026ca9b1d0a80e5d9014b1065e9a305be39af7742d31843b65d7b288c19bde50964b415cf0b619a8f3f0339f0508253cc18591ecc6" },
                { "en-CA", "2481fe1e9e593b019cb8fc62a29a5ee3b711048ec37abe5f1298c1811079fb1ab2e19d43421bd5d8fad79f693f793a8d3165201eb257fde61b5a95dc67d1d0e5" },
                { "en-GB", "3bbea45ddbc9fcb5e0f5788ac63fa5a07687a74be54cf456c4d3818d9e0f674852d8899684f7b36d8c6a975a30483aab0cc3421b74b8a8e566f781a2ef53ddce" },
                { "en-US", "54684b90fcc607ec2517c95c195eafdcd8d6e165b797603c23376ec22a3bd03e068b4d16426711bf5915eb0d1a8897a547facfafacebd55eb05eaeda89bef0f9" },
                { "eo", "a355026499e8ce413e64a407f8ec92291fe618c80b87f862ccb5b0a03d324f06d2bc1a90aafa0c9cce00215544b14a7e2e7cb95d513039542009bb05390e4594" },
                { "es-AR", "df14752baf969ddbbb426a4b95309ff9afce93e70b1de2c600bb8281f920ddbb9bc93d373639a06c305140918da3ed3b3390c8c773f9d62743735940a3bac899" },
                { "es-CL", "187e05182d2e09109fdda6835e93fc9921adeb054e1c489c9b58170c1a86d9baffe71ffd575c134a608dad2eb189d2a3db6ab6e587e95acf2058c635d94fe223" },
                { "es-ES", "8bd1a925a67f8ae3becb8e8926d9b534e5c53f9d76ac56fb1a11d5da64886d343ea8a950586cb95cd418a1dc4a8ea7f5caab1cb590f25c554c4ad3b14f5b3e75" },
                { "es-MX", "83b958a39eb39a1e568aadb0f59049aa83b998d3cedc292ca7f3112aa8cdbf170f1d59dcecc6090fd2a62db4808f7fd744ccde00fab3f26ffc756b0848d5b05b" },
                { "et", "0da85014503aa0d7119ee46b554ec436bda39b400b1f7d8e85714c39b1c12b5761c5942448ef8b33eaa55f1213538fdfd0334aceb11b00326c9e48f4ec8316b0" },
                { "eu", "19454d8b52495c500bf6bea997b7ec0fb4cc223b3f43db603bb85684941ef2a32294b002ecebd11818eb4c0115464b0b4c730c25e490df1938c7848dd7f800a3" },
                { "fa", "65ba9ff080e4b5435db49aaf62177c5cbfa6eb157b3f65739544e3fd26acb89ce1e309e790cb5b460178c967dbc547bdb26dd49ec77004a559884a6aaffbb708" },
                { "ff", "d6c3ed6f90a57f37e80106afc97bad2a3e7b72e6d05edccb0726aeb60f221ee91716d1a49c1dea6b185e79d3c590e33fd69a272650b45fe72841b2d870432175" },
                { "fi", "adcd61f39b59a46a9892ce8888d87c969fa1e25d0ee5b06769da270a078c74f4eca680f66741cc973bc263c567c461a9fac1c69db78f197dc4903e1f6be567a7" },
                { "fr", "c252dcf9565557448ba68f23320f47a049a9dbda5081dbfd9cae5563412258384f5d600a038b50e035888c6cb638d487f1aa8b37737b7a9b5febb66ad7ede476" },
                { "fur", "c6093fc55c5b52872f0f06d83fc34abfcf209a2554c1f85c56872ef10906bfedd8fe2a9f5f6cdce8725958349f3c969d48d2b7d8a286f75f01701f6b963fc0a4" },
                { "fy-NL", "e65328f95f357fd6fba59041ce85e73e1a0dfb66f8e371dea8b9f582921e7ded320ca458cec4b05dbf69d80d135c54a3693930e16a2c4bf6acce2580f535bd97" },
                { "ga-IE", "def04a1fc00e2168d15cd8e8d97305c057536b0b175932ec1552c671f70df7374f7b440b905719f8b382ce4b87605ea69667645d3453a698f6e9b25a6fe9db97" },
                { "gd", "67f197df0701257c418dbded869592ecf02bea20305446f446efc1a35576d4214ffe2904bc60bce6e4231e531d8aed4a52f7f8e62ee3db17ba3ebbef9a61a806" },
                { "gl", "b70e447f0f335a238652935b34b9e04be112eefd3f389ab3c19eb1b19483ed336db9e0bbaf0c73da5a8a5bf27a4267edf1e1bf01d7a96b45ac5f14faeb7c4632" },
                { "gn", "7c6e5a426b4e19cb62a8e39ef8007244fca43a414d389e1f0043b657681f90a8d6bc4035230c5358642e6b063c9d5357e92042a68da748aaba99e17d8c3d3126" },
                { "gu-IN", "7c0803ea5bc6361e41bd3167b0b3393d711a6e4efb3fff84a1d1e995fb1ff08a30b527c52e2779c23c2eb2080836bdf9123510d6064832e3b4b9f78a107431b3" },
                { "he", "0f5e93511ce4dcbe85013fed9521e6abd535baea0e3f0af1068b873a1739a3da826c7f8432b9ddf90d3056a54516f4393733d0f6035660a2cc29f7da1ab08811" },
                { "hi-IN", "b9cfbd7fc58b588f07903cac3c7e1d4ff3b8b9d39d8722ce44a205b57b441ecdf2a4581d47b7d2ec221dc6093a0a09634de7c53e423157d67a4a5a9035513878" },
                { "hr", "3073cfb497c291dec10e8c9d3b8682c94aa21676586a87a4b7ad509972b7de337e2728ee9d8c7ad623757def3e492cb2275676142a7c46ced560afd56d2f85c9" },
                { "hsb", "270774ae573e761413f6022a558b149e5cecab834bc353e42462ac76d4f5ffe96ba52176585438da37411447148ae985b9e61bbb8b2b94d6f53746d0e7f18142" },
                { "hu", "3a22bd4bbe49e76ab9b17e15b11669a3d6c7354676733d6cb86df27713085487e1c8b1ced5ce7647056684e3181ad105de5d9140a9806f1246554140777d7685" },
                { "hy-AM", "22ba26a68d45436fdfdf31003bfb2b1e4e86a3934487117967528693349b0f38c95aeb3f0daec43b2bef0522f08e751500acf9e151dedc68290594844ffe9c04" },
                { "ia", "a48d433b1e137fdc601237333c6eda60b7562df64ec6a75e558458bba23b1a1bb041a9a4571fcf63572b547fcebb9302298ee3d247351a07885d0ed9b5b510b7" },
                { "id", "dcd9e2d3952c135a2ca53541972f8e9500543d47c7f4f4591e01c43aada06fdb310db4e19f7b05db2cd195d3bdbe1a79a4de1ece5d3abef3437b53dce5f84b88" },
                { "is", "e60a068762d536f22ed5acf6cc94ecd5133bd69f8b63f2031a0892969660d0e322f320386ca16b51beafd0d15339517d88dee1dc184bd4eee141ee5da6fdc13e" },
                { "it", "389e84dc2e228463058e8a54690b13b9bc7b5a9cb3907710134f39a581bfe88900c83d146332e82f8ffe8fb2356fa2d6567a95bbaabda60791f1b96de9455704" },
                { "ja", "aaf06ebc624b73d83cb00b4b7231f26c9aeccf77f7860a2aabceeb7adffeea2cb3e4dfed92fd668c1295b72eb50080fa19b26b8826c46d8c0b993727000afaa9" },
                { "ka", "eb781a49f74f6b6fe374597c7529c5dd71ca800546e61489ec97a1ec794b37dee45d29eecccac809b64fce26d66fb344f168e9b5591b3e1952e6b3606e66596a" },
                { "kab", "56c79838dc615e576835da80c05dd811b09b399e42a9a16b018eefa61ffc67e684bc2b72af76f66c98746724b02d4f65d04ebd6f24637e9c523ca1e9849b1abc" },
                { "kk", "ecfbb409730c5dcfb7d1bde4c7a5d3c964a0fbdcd110110adbd9863fca7d68f934efd38d9bb2941a112675c647345287f4b5437632dd5bdd0ee9ac24176f517e" },
                { "km", "17c14b271728a27addb7c4ba705bc9ea36ad331daf2bb7e9f437de93d6447e6f4b816ac4a01ad233b01e3f3990f4b6a95ae5f846270be6fdf9c2858825658a90" },
                { "kn", "7428e7c142f664cd421fdfe3ee64a183595408cd30932605372d2fd19644f04b613ca68281a505bc679172013bdea07ffb6e54d234c5b0435ced389661086d89" },
                { "ko", "414c24d5fa87f07816aea47953801f77cc9c31905e389c734cc9f1ad2fc01550677ffa687dc4a63fd5b748a35d0005c8b22fd7b87e5bfc63126b429c7f02aef5" },
                { "lij", "3c8e416f72421d4e3500f72ce45ecbad4b978c24bc1616e3888f676c5c58adbef75e12a51b64016d2704d2d732e7aeea3ba7a6acfe343a0212db5128ac86223b" },
                { "lt", "a86c72d9f4c131b4d078aa7f60fca7d65dec8de59947a1e3dbe0c56258cfa7a48a7a4dc597010c3a0621d6263caf5f0ee6bf1d1356b9868600ba328457aa54fd" },
                { "lv", "797ff1621848d41063ad01ff601920c4d61197650f143ed9c7168c87a16e960b27aee629ab21d9333c4f2f3668f3ed91088d46d60934bbbca1b930518d378830" },
                { "mk", "544ef56b6509e288704f7e8b330b0831d60b087bf0a8586b72486f6d9b9f42f144f5d965b5c3d3b0b322e8d828ecea7d1d1c0eace393033889f028a28fae97a5" },
                { "mr", "380cb87fbe771d47d4817d598190999350df3dad2278c7207681d25cb6da7f3e32b71077d904e1ef3d5fc6b8b0552b9baef64dda7d3c469333df75e6b8ac6b0a" },
                { "ms", "47c56df2279d79c84637e9a1aa698dce0178addfa1ba2833a6ffc972c1fb781c6984df184c7bef9a57192c3b17edab5b5984c0dfba0d395bd9c51194013ee5ff" },
                { "my", "fd0dd1ca99342979699ee7ccf1b03a3e7ea56763f19861a1a729cce01da842003a290d603c611261944ade2ec30bc27448c28f7e6ecaf69c6e0c186868cd6722" },
                { "nb-NO", "7f501dddda5face0b7ef96c74ea6d9503fc177cc55748cb890d7979c45680b7088742290832db0f4e997e7f02195717bb9a09a1d5bc22f976f934ca10f590ae9" },
                { "ne-NP", "2fa9091f6f3d45e3a41938e3b494cd16af67b4ffcacdc4c1e7d1033db5aebf7f854d1318e08ef1b796af61fa9672e4bb2e0f52b873270b98a3575822c69c461e" },
                { "nl", "991b4939e1ff2b46dfd2b4bfa4bf51cdb59c2fe5b8a4ce7aba7b66df9064b9b390b02f3b61e2c8076cf4dc872de00d1cd4b8f795eae9bc230554b400fa60f92e" },
                { "nn-NO", "115b31c9321155219a99853d509f1377aa98d6c394b7539641145d994303f4521a2c6821b48bb42f521b7cc095badb2c9dbe9378b69ea576cabd600f4d04201a" },
                { "oc", "2c06323a73010c675151456cc6f1677aae1b359e301861c538264f672dda6f02627706fe98bf07ed68a7e6cd1dd8cab902be04e422c60a8c78c1cffd14ad8223" },
                { "pa-IN", "b6771cc13bd9b5709524672df46a8c5f5a453933117d8d395292916a02a5aa9180a93bf4d0de8000cfcb3561aa13009daee322f12e616f40becb6bbc4921a035" },
                { "pl", "88d66315340eadbac41470caee6b1ac63145f50028c6ea49a5f7851c2221924473e8ef8fd6a2ebf1c788c09ff97aa9a591455a9db0aa9a453b7bc913b42b8e47" },
                { "pt-BR", "2a85a70655868f09b5f1f10bff8c9bf344d6873e3dd9a18edab1db4f9e2abde808f0854176d87ab237d585fe0d8707c853096a0cb34ead0d593aae4aad719d69" },
                { "pt-PT", "1d2355ed93fd33ef38463dda95465795f4f047e57bcaa7effddebe7ecebfa7b137f3c3b145976f220083ad12775e359c990289a611873760100f8024735e25d9" },
                { "rm", "df350028dc270e9320b6d75914047a85f51e7b88d87fe4ed584e2eb64dd6b9eed4fe7b4c3991a35cf1f55c19bba7002da91089813aa17157fec51357d270851e" },
                { "ro", "ef777b552350ae01e7dbac7aec4209cdfa3c07b0614a3e7619f9fbaeef3908a8a22e157550bdf3a23a434e2ecc12fc1daada8f49887f3e24d0ac042a8adaf608" },
                { "ru", "3dd9fdeea8d10c7819e0120a89e894ed9d49614abe956272fbece86a61adeef1fa240dd12a5f90e08194682e5bade06a605fc584a09a5aa34ca7a96966200cc5" },
                { "sc", "f463d920d8c4440588bfc1a1add21513a8395ebb48491cc80faac06a0640150d3de64457308fe76ccc5e01f5d69930a4822ee42992c4cfc7ec235c06911e42e0" },
                { "sco", "0f63065c508967bbc5e75071067a23735b8f20c2ce648914e3bf6823bc54521a4a538ba3104c75c1290b1a8246212298bd0af8914aa876f587e4aa15b12c9b56" },
                { "si", "6dd8b52d2fab0c63d8d856dd2a95d8d06158f177d89b89090a3e7f5733eb09b6a38d823b1d325cc50838de66f924e548418f16d685836ed2f802cc83b997128d" },
                { "sk", "6811f8e0fda209d3910a5d2c4cbc7b5b549e284bd82a0a0d015e2a8d41543eadf4d5b03d3b1758c3c2b7bc919ba0d13106df4fb70e3d336ed424630619b53f25" },
                { "sl", "d462dfddc272d3b003dd2917f2acbc3233c9ec89ed5d97a4d0aef48950145cab9fe0c8dbfd0d5fbe754e2477c9f3adde7f59938d3fdc2b3df8c56a4f83de2765" },
                { "son", "0dde6da5ff6302831930dc6f511b1968a25120720716c79326e5d07d3a069c5bc697c0b5b47960cf6cee5eb21ad281cb0b3a79c412503f1488c6b4a257d6f415" },
                { "sq", "9052dd18be97143c962f6a55cd6b14e4bd24f912780577e19832c695daf3a44052f54410499788f67ba756791bdcfb4488606a4a3ef8a42ea6bf1dcf246521bb" },
                { "sr", "b5e8c98297d200588423da9e07b362a74eb01e01050f25e91e4d9ed4a395fb485d67873786935238f6f8922dba1c18f63ca61ad17160b7af142f232a26dab7f0" },
                { "sv-SE", "cbaa63c8a9d8a844e824de13c3af86d9e52313daf3f31265eac9ea2475fd4e0a1d2276f4eabe1d4b7b2985a1f039d1c87a33d8458556e0cceb3ebe54667e10bf" },
                { "szl", "1907c42ec5ac7829c8afdd36d36b1267d35ad1ba5f784381e60750bc42b97753da40c424091f58d6c8a02dbe257710bd3d79a428b82299a4e1ac80774cde7d2c" },
                { "ta", "e3d7fe82cd1d3773db78549d7ad43364383354f921a493f2bac1ad93ebad239ea0d2ed47a5c2dad1da1e843d56a6de95b63f0a1d9d63d0ff7e0c510a9916667d" },
                { "te", "1dda979d07157b819fdf652cdd872028fa25a237449d84b874bad2122ecf0bae371d1859e4260e158132ba24f40ed30e32e0b47a07275dbea846d53cf98d1589" },
                { "tg", "7812838b110a1c71ce41dfdae6aadfe06eafc0a01a3ef97017c1889013f16fbbc2ea2fdb082817c2382590b2a02afae6cfb3b872ae9294bb45f405b389422bcf" },
                { "th", "2ff16520762e108af766194f83c7dfc3699ba585dc5b13186d2e8be3098ba6f6806da7dbe98cc93108ee81ddfb8544cee9fa7b8755689a2a5827e73eedba3629" },
                { "tl", "c43bc5950a1f1dc27d579a70a60a381acc0ee3104fc5965b76ca8a28833110dc87079a973103df61dc90181108c59e185e1ef6531f22665badd9c5ddcb9317ba" },
                { "tr", "0f3fb0f42ccc1297b2a19c5a905656744f63823606863cc55014b13d4186fe79f37e678e841dbc25385c47dc7d1af3f02e8e94729051d0cdd29e9f5d2448b80b" },
                { "trs", "8a4f0251f77eb7c79b7d2b205982fa7a4ca4db0038d78bc25ec8cd6dfbdf9c5a77decf92b9d684ebc6cd0c46f24dce80cd79e763e1141e228fa0a14dc40e84ca" },
                { "uk", "f80e19d172f9d3e7888e7873c5ebea8f57fd5e8edf09c2ba309068e5fb2fd5ebd1a87601cc198fb5ec2912a5e810af3f64385d1e2cc35bb250a7ced73c540ccd" },
                { "ur", "c5856851d2c57ccbeedea6c0682d27516adfe7a3ea32f4ab3862821cd37b8ef0b7c4854020934238e226dd22b8cd0377076323a9ea461a9ee8b67dfc27d2902d" },
                { "uz", "c06cc9d395a72a457c5a9b3b620dbf24203213538ea748806d6a598dda200aa27e72778004741d71dffedbd420c3a81022965bc49ab72860521878d5b6c976d9" },
                { "vi", "22b0b9b8d5016557180038fbf6067b2a74c5840df73f625f33da2027ddd64eb43afc5308101f67397022254aa956fc160f7bcb2a11429aea0508cd786aecc9ab" },
                { "xh", "04cc3613e9caadae37a04a744f99c8412dd4b4ee33bec9f35334d8ebe33e5f80283b4b3e1a71206d482a5d9b5e32b018c8aa0b86e706380ac80ef044d4008a08" },
                { "zh-CN", "06874ba1eff972ff0fe65ec525c1b76497ebbc9baceb9164fec77f9e8daa4d49ef222247b69d6995d165321103c660f6db782402d019285d20d0dae599de3ad2" },
                { "zh-TW", "cc933d405357479d266fd23c99f4ccbe55dad2940d3fee6e634c696bbfe774031ad3af573257d3dde2d291b3909851f718508b7bfdddb9cc38cb36b2ebb44df1" }
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
