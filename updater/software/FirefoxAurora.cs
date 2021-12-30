﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021  Dirk Stolle

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
using System.Net;
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
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "96.0b10";

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
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/devedition/releases/96.0b10/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "5c4be31552b3ad2023a96ceac0f390bd46f33cb01b7d39c948e69f369239acdb9b27246a4aeeb371877314bc41c00db24dddbce0a5d77c6d68a9b51558079ff9" },
                { "af", "e2c5044ba26fd3e0bc81076533030808764f0fd4a7c79c664baa7fc0de227b5b04089c7ff62ecdd1d252fc99990b7e2eec1e940442408ebb5cf82705bd62686d" },
                { "an", "bed81d917def921674d9dc1910e92f78d15d5ef53287df46aef14addb1c1303384d1a362c2271a85c0b02eddb772b4e8d2a040519f4cfc73e38dd2fc99aa7b98" },
                { "ar", "a7810e134326de853b76c9872a31d1f8f9a5ea5c97ee6a84b94b49d6803f838b9ab9a9fdc737864a424ed7d5397196fcca4120ecf03375b19b62a0035207484e" },
                { "ast", "1831a8de21e9721d59cb5a39fc086a264ba94589835cb7ca359889ff33ffcea29c01c4986ab69acec22e7891faa7de290c0bf1e8bafccb048bcc874429a128c6" },
                { "az", "fcedd30184531b3259a87b28f88986ed7580085b40573b07d73316e851fd01cbef0ff32382f7ed46f18bd75fb3483ebb80f0e8f80841dd9b5cbdb015308ce029" },
                { "be", "bdf6744e77b12435904a3cba0653251ae01d07d775e1dce7e28cd147d9c4b6befa877ca26d39ba3f3c7b89d60ec247ae9197295361edbee58f2efc919214885c" },
                { "bg", "7ced73fa16e6ab0485dc38a18e8a06e7a7005bb61f9ff6c9334fdf1f123bbb8a7b7b8c9d9d467242cfb1b698771f2fb06224e2f00b7e6a6a794e7f3cdc23d6ad" },
                { "bn", "4fc4c696766b85ccf6e2c0a017a7a75b4e859a77b36688bc1e4739394377603ad322162300095f8696d938097e699239f8cb5edacd82a663000be4c7f396e315" },
                { "br", "ff4baae27e791fb87315cf3522e90c27a7258af8a28e2f882e45e8b9aa7ff4f8874cb6573b9cf5b64755dd28bbd2ed25a9c88b1999759936cb14034f26606957" },
                { "bs", "4bf771917fdab380e7ae00ce6c14b5fe1bb1764e80223f6086402ff4c41f64c2048ff538413aa42616d65a746a9fd2699fbc09d63fcf9e6b58c9d5429710e2a6" },
                { "ca", "8f1ea72e31d060b5bc01ed24c1c56c81902b8616e4a8cdf42b21e05d90d3f40e16380350f3c2b8ca7b2c6e9415fd3997865e1527fb2155a6f600d69983e5aa03" },
                { "cak", "7eed1a15df247f0d099ddbbf590f013cd87383839068b24745741f087acdb0a735c32ef28c8d3e995c4358e5cb7d0dbeaad7585c3996f7df891adbb2dddf66d1" },
                { "cs", "81081298e88147af9f73eec86cddd1e51b421d89c7bff078b0fe158eb5dd42b636bb9e51918ceadcd9896804c9855145d0fde8cefc8d2fa706e42b23b41e9967" },
                { "cy", "419c6ba1c4f937c607e9ada64e8470da9ba790257f650d019fdf2ee26d4228c16c6874b83906c22a9396aeab8a536c5f56516516bf731c7fb46065e89c8be22c" },
                { "da", "0ea3487895d7ffa122e5575710d09c7d9dad2e0b86fb1d352e1cd62f05b323b0421674f11710c59139a7dd1b903f94317cfb46f3bd17ead9de9c1e70ba184762" },
                { "de", "ab11e87fb4c5fcfbac06b127c6d34ec9674d8f0845200b70627f59d94077e027b198a5a66d8f6367505ba3b445c1ea2b4b5617e7b07dd06639ff1d1ed62d35fd" },
                { "dsb", "25ab70c87a438cbc18a689a6db441eee26600c4a37ae4a8a6402e7caa190764a1b3687b64641a408b3fd23fdd614982d10a2f0a09bd07bc755eda385b6bf22ec" },
                { "el", "ecea60dd33dcc2946e7174133518e8b50f37402aa00e54f7b683af8600443cf73e1d9072d3ae5641835b26866dbc981b4bf1d8d57119a8564a32d3ded618ef69" },
                { "en-CA", "ce869a84b4f7e1aa9a8bc206ab7656341758d82dbe848bbfa0203ec4205323720c3e92dbd4ff20c75239caa946fbe93d72684865969b9f052adeaab4eeb8d45f" },
                { "en-GB", "f7916a69ad7ec3c72009650cc0d5b2a2a8d55c7d27ebffe0cecabc6554f0d574913fc9c4854d4ce7f535e1cd32dd5c024e0241eb97142e9918be03723c92c107" },
                { "en-US", "166cf83d9a51d970f9c8e9812b904207bcd049834f231aebafc6e73e8c2d03c223371637f8d97443a357a58edbd326ed8ee6f8fc908999f9d527073c333144cb" },
                { "eo", "d71006de2819e11694b24ef2af794407cf003cdc75b13d93865a4a1e4b74943ff5c33ab8570190209c2764e3c4065fa0d4766159efaf67f080cf7523d89b59dc" },
                { "es-AR", "71d6b940287328030e024641c67bec31fb33bd4f633d318e92dee7c4af59847da3edea0443833c92a7d9481c28840de9e94aebefadfdd451c773935d51f543a1" },
                { "es-CL", "70374d5579daf230748c02eebea393fed29a3bef298e3ffe7c374bdbb18e7b19a689868c68273b9be339001841b0d0bb02868808b1c44db2e52cd13ae886a506" },
                { "es-ES", "7173ded949174f53e667f28985c1f48a510e971afe7b5808d153b92d9981cfe940337d6cb576bf058454537ae72818246ecc6537858009613fc39e5b7c09100d" },
                { "es-MX", "d9a397878e286580fa9f830ff3923e1f40f7569c597b87fb7168d42cb97323b05ac8412c56209ac68d663bb6707b97ae59996c5d8881f0e3828ca2474ca80a43" },
                { "et", "815264d58420b821cc46896c89481f7b84b7d16fa6b50edf25cad574c36d09d869a3a4afd38650b2cb25a7d22759c424cdf0d8938390dd96b7d46d02f02f247b" },
                { "eu", "c84ce63464e3fcf2aac6d49f95f357d0060c4805a4a2faff6a0c1602d5103c00acd40a8f7bca62e7755ac874454d01c1247983dccc547b3bcc8a1b57a43dc225" },
                { "fa", "af2dd60d03134faae17fff4c7e0153a82909c322c8f0ddb6a46e9498f65b622b446283f2623ee9943d70a28463256e2640bbde3f2fd3e4211bd13b46243ad0c6" },
                { "ff", "9352bdfc65eb81148ec5320357c8883ad18b2bcd885583ec4ed9193403205a22d22ae82ee00db1d87995bb116d327bf4561e665ac319a1021bf5d92adfda84fb" },
                { "fi", "9651dd7ad684a628c5456dd32c3122061bd8da97c8faf80dcff35cc394949a6550f297ff8dd54602345633e4c60979305b08d09d788a4a14853c47288a9f0c09" },
                { "fr", "32cd1c13c82f76cd1daba14d45bfa450ec336f8994fb5ac4efe36ce25034b296cecd892b7f8922452009c8473612279da756176a5aee86ae32764a67b85ca6f2" },
                { "fy-NL", "0df1e21d32f907fefc6ed1825aaea8a97f83a6c8a722293cde31f5b519a53d35217510f7afecdb95898d996159d8c3279cd225138bf2cb07f8454c740d6ac017" },
                { "ga-IE", "63075eae5a70a5933d2ad4f14e568e78a5f2c23fc08d9b3af759a19511103492660ec8083f1fded357195ebc51adfc259273c8ef78823a032b6147aac89306a5" },
                { "gd", "b73fd39bf716f0d9e9014eb84bf27cdc8c098cd40c43d199ed59152bff4b175ac05fea8b4f26de2fa2c084f699071e1a3cb94ee0072db406f49b548fffbb80f0" },
                { "gl", "1f02770442da17aeb805e5b033bc6f3bebd5919242c957b164e043da6f59b0c0caaded4d011dd902e16f1dfcf0842eef9c885ca1743845f0b8b892080ea6b4a3" },
                { "gn", "b25f469279f88a6492d76cb173b9bf1cc1869ff1750d46bb030cecd8b466e76f3ad2c71572b06ed46e1885fb8f764f1ee471f7cd092c92af923a68f3f5fd9a40" },
                { "gu-IN", "1093899723d567beca6e345942f7b2e71dc2d5f0b59e09bed3ae59f49eb3f4ae23c90ca939faf6de865853c00e432aea5a468542da4d7c05e09533bb3fa79cbc" },
                { "he", "273eae3938f81bff5a3619bc57d88050d5ca2de7ddcf00bb10ff0c722cef5bfd49c9e3f13164dbe8da1975f527396d4d2e537d62b84f4be0bccff414fb9b8779" },
                { "hi-IN", "4fb62f9c1431f0c9364ccce32c1bc2ac3a2448980b96cfdd2ad928af9a314acd72509bdb2bea56015d0e2a19ca54e90670d590d33e68dcf83a5ff08b6662bc90" },
                { "hr", "7a99aeb270145b0d88801b22cf725cd116e6aa67b903efaef35b39ec564bf7ee29ab4f9fca83867a458cd1f0e1677232a077b1533dd0e58a88a716c5885ba2d4" },
                { "hsb", "089edb36cae1c6dcd0568b03ad5f11d57504b6d4508918097b08488eab885621a66355bf6bebdad8af393ee0b09888e53dd4ac11f4a628adc73776aae36c1d38" },
                { "hu", "678f1de427add0f555d09894b4d72bb6b9f1bfd9f9f2b1b93980533f86883213353b18824564342683f977387af0bd7ea56b13c5c79009b9d1a7b87051e49607" },
                { "hy-AM", "206f82363634b36b4a456b2774dd9d646e1eb8b9096e4253598319ef0180bfac564ac99537c7364fe6cdd49433b513ed218949b7a67295d68d47d546e7997cf2" },
                { "ia", "450f9ede3155768e8fad116c18125ac91cc971c36f3428f90705437c7e25a5ff40efdc5b63dce6bf4ae590830fcd55e4e4876cfb8898988f18c87e651d87ee1d" },
                { "id", "582df5764ce5f06a87b9c78ead03f8a515aea771e53b6799e9da53feda3ac219904c6b1a2760de4e5366b19265aa8babd448af1cdc0ca1f6b15703b997260190" },
                { "is", "aa07abc04bd706b38b57fe9d2d585935c89b48d077c7068c1fc426e7e6d0b76d82ad38007459182e4f404c9ca610713f9bd296b6185416c0bf8ee7c90c09524c" },
                { "it", "0e2384834f08d55de990537043ea549283f6cd729707337afdf02b1ea5c75f0b3951123e9d20b31597f1bc16e179bb04706c37936da238dae8ad9e873a75fe44" },
                { "ja", "e331706364fac17405ce9aee276c95c01aa06bd839d27d236bc76ae9a3132b2fd9a592988f301da409fa5c8fd58ce2c9e3754bef85dc328e512162707b6a7eb2" },
                { "ka", "dfd3839999c59fbe6ec0eaa16ad53f75f5a6855099bae0bf8120f478d377b6ff79f7cb6cc060a2e2ea438d1a728925daceab2b91d9eceae9df7f730fb659dbf5" },
                { "kab", "260f8e8d6bd925855cb57ba874bdf5a3098cbfb54fb4a24b98fb34a3189a3417ee233705b9a1fa97c0bfb6a24576b0f9a8d108297981fafee3fae4c81143589c" },
                { "kk", "963bd857519564f2e52f054a2806b914d6091c034d9c5dfff4b66343f80b854bc0b8ff06fb93afb39d6ff66e28d82bcfbb654dee26e64b59b58e0dc345cc3f0d" },
                { "km", "2ae12800ded550cd9b6cbb5c0db3d57ed74ec694d83e8467502310b2a948845b487b18c01bd3386feab3a62d95918af50633c4336aec0ba73c3c9b0b3d341fc5" },
                { "kn", "08f1ee8b6144226eb4185fc0b8cf30fc021829abe3a90d6c1539ca1e8823403c948cd272e668b8a84f5ac09b6e0441b6d21cac4024a5993c677664fd1d496d41" },
                { "ko", "57f4f4a18dddd990e644e97e010e6c73d83d5598a923236012b73b775e15064a9ebe8b51ad683f5e22bf08b2bad5075fe3c48dad29ed0e468e5990c1e280257c" },
                { "lij", "8f992b870d58ed801d6619abf53ecd68d5889984006d9d84cd602498c52b55b768a6e52b0ab41d9f4f9d06cea3748bd21ab41473a5631adc31a1e461f3c2fb37" },
                { "lt", "fc71b8a7aecab6e0b545ac4d8269425deaae59ed6a4a66922cb6a3ac7f7659ea436a05e177481ac1df8cadacb264ae6e9a0324a9ce56d67894a9de303179301d" },
                { "lv", "3fdcd91771213f4443179ff005ed8a7a1c8d0b8a11cc0a1fa0e618f00509c5156b9ceca99a9d40440b988843c3f02459928c0bc87587a1c0047afa093bc9fbe9" },
                { "mk", "0ca5f41da82e45578881026f0c1bfed6acc0debddc4251b4cab8661ea7672b940a5daf108eed252a859903f65a39a6102be8bfa0ec79c6f42d251a5d8a84b2e8" },
                { "mr", "bd9bfd32b0bd1d2a886ac7aa7bd6ef3d090a6c309b2dba96204614ddc3154c30691f602fb7bc00a9d7fc2442d7a89dd1f6014f3067b3f5a6e6c56e4cb1e38184" },
                { "ms", "03e8c8b42ebe86e54e374f2d9b05fd6155f503088a13e18a65943b867f645cb99e4d2bac964c3dfd1c8de9932f5d46b48e2f07ffbe0d8f42f59403a2b6befc65" },
                { "my", "30ae6d7dc6547d1e2fbf60f6bffb7da764e443e99656df76894f3026506b9eba33b8d3f5e711e4e688524643a4c3b3ee1dcfffb2f4b850a848b67c04de001b81" },
                { "nb-NO", "19c32cfa5ab7c77e46ee423d3b877a68084ccfed2cde8ce933f0e70b0405b7da4ff7a31cb0bb57521043afe80faefa374f0cb453fdbba3674e89c49b12a2b82d" },
                { "ne-NP", "a28c847d4a4135a38f03a8c8f1ce901069fc910328e98bd1dde9f90494563edc46479024e514aa44e2795b6ea263439b0477c1baeb3cba79a4df60cfe4a36def" },
                { "nl", "a2fb59a15bd988fb06066a7f1a82a2d80883780fd44ce37e403ee283dc519c2595bf4c11da704db25c8eace11495d0db8132abfa7aee4ed8f38ad685e983a87d" },
                { "nn-NO", "04a3564720d4a63b468af31b87828610ed581f0bf869270d8cb292b9c428c177ee3a43559be4061b1fcd1535f043771519e342c326fe54327ba8580b08af61a1" },
                { "oc", "a45f907bbab24510d18de95acbc93cf2b009a05f39c8701eef6d3310dfcf98bc1e21a9c4bb1141731984587856a65c1c9971aee1cfc00cfc642a483605dde614" },
                { "pa-IN", "3d1e2dfe08414000d540e6c178c43d4e54d24727f0159f568618ec3aff47e289ad34500ea53848cf03468191cdc92b14c1e76b39129bee81a41641a26e0be72b" },
                { "pl", "e3ce9e4b5ebb4fdcf1b69586fccdd6353c006f9f3771a88f855e1391048411b4e04fca7c2af9fab5b56fddbbfad26f30ddc44513103c96e45773031f8c3bf6bc" },
                { "pt-BR", "162eed671c2d3661cdf48f399ece18e7d944270110625faa75dfe024ea9d21d418eaab751c68a66fea8fcb8fae4696e3c268c041cf77bef528be42edcf415975" },
                { "pt-PT", "50f8b025e116ad68bbeb720317c392d92574bc73163536d91da006172ef68137e9c9b5006ac894d52380203d69f68ac60b4b059b2e04c834979a2cf06f26c454" },
                { "rm", "6959b4ce00269af630b266601d8cd5a7f94a2c1cc90c00be212ba2ac36d29121c259859c4f3197b56237535691315534c6b829744c01736ecc83db6d28fa6dae" },
                { "ro", "1ffda02f9beddb6408e838cc46215b6e3580970e5a69500ddd7ce6438d314294a28e027ac0f6fcda70497748cb387bd47364a639e2f3a63c6997e49df2c6fd27" },
                { "ru", "32d57a0bb4e0adb27b4fafa063bfed0f43953ad0b26c52791a2593ebe019845a88424c06e8f5b2087e690839c7a7f3c8514bf3093d17aa1195d718db0e8be538" },
                { "sco", "f424e9b64540bf7328daea5dca31487f357568d110992d0a2ca31ba340d5db6a6510346878a056556d6c8d85ba31875f3086f10c1f7f0cd8875c3acbbf768fba" },
                { "si", "729e7d630544f54c03588e5a9f7e443b08266f6d8f3c2fe72a3099f419bf0a32fb6863370d9bbdb078dce08d1b45bb9c6c1baa31a5e266bbd093518e1c9993fb" },
                { "sk", "4fce08a5d98a9daa44047badf1811efcabc06987b26fe4a11c30ebbff6b90852b9a46469cb2a634751d2d3dc8587da92201fdc4e3a80c0fb4d2ab57146fd26b4" },
                { "sl", "2eee2a4bec2b5c78b3a04421e3c741b3ba59c619a21c9243a7f6af8ec81b5e5be576f34515b41d6051dc322dcda4f56974edb18d6e72da2ce260b233fa8d2059" },
                { "son", "a2b3bbb95a36c036f6c35c3fb22ea8e75d825361bd957144ce9171ff9b73e2560162206f35045bd9193034667d981ae9abb879747c88eb7d3c05a5961bd2b03a" },
                { "sq", "bee5f9a827c28258c76a92b38145e05ad82d29f4ff75d01e032ba85d4bfb67ae8280a021753fb51129ebb5e14f885cecaafcfa9a6059d3ffde1e8df6401dc569" },
                { "sr", "4d9002119a172dac159f58e0972b33b0b5208517a0c65a5d7c4fd774f681194d1b5517961c52ff144475ed2ab7653e1deb44dc5cf956363d5b9d57be4e53d6b4" },
                { "sv-SE", "7c41e98d3c42e2abee84cf68b9a87ae930d6e7a55fa23e9b02ca5c6b9b4829f9cc8201fa18e8e136d710cb90f9533f198958e8dc50f783b8cb639190838e8354" },
                { "szl", "0da13644a1af4f0548d5497d1e836bc80bd8765bdcd754a0e1f04191a94cde5ee13c980d7548cc3c84470e73176b94bc36df6eb86065200b6ce27107c95dde7a" },
                { "ta", "57c6a63754c29f58837114a1baade53b73bcfb282c1c00d7dbccd3ee57536871a6514769741a4549432293466e38d8ac516950cdf6d9e7346dc347978d0321fd" },
                { "te", "737b6bd630a49510a53451009eb7bbf67fc1336b0dcd541d753f47cafda355ecb24c28fc08fcd964ce6b98580f6b70868d5eb7e10fc27fb5b5bec7d9cb5a9e04" },
                { "th", "dc202ecdf98a07eea05cccf1264bd9c7390a533ca8d64ae258ad9c044fe04c4d1e6b886ca2468c0d3079846a94893b0b596a443a1da7cac6f40757cdafb1fcca" },
                { "tl", "9ad08c30a6d25ef264e2065f819f6659736dc2909ff7783726b5131cc8996b260ef6c57a833450cbeec09e9400de4366e0469086d96dc9a3658dcee99574bbf5" },
                { "tr", "29273515c12741affb78edb4a89341de501625ac16b5037b6eea93eb7e4af1c5ced55f80871c16bd784d6e31dc99b07725b82b88426000dd1eb504b96101eda6" },
                { "trs", "1503b0cd90c47c6b72989dbb65e2a41111d33686d5eb4f5763c767f75cd6b1f2ab8a4a893ddabb94b79e0f1df79f01645e161ce67f7d40de508955119bcf245d" },
                { "uk", "cc8fa9f0d695026363c2a66f3ad6f719e4e78f82e906fd60ad7f234fae3c18148b36dcbf14e94694996347127149d26d2882240819616cb4c01908d718a34206" },
                { "ur", "fa8fe19529c609ec4ab804fa9211da266e9c30e635468a5f83fd3b3fc425d02c88f325cdb952f42efd82269816a3b11dab4f7140b0fe5bfed312ebabd8d567a7" },
                { "uz", "7993d3eb4ade817afa4aa5a9a8114d45ff257441b307852bff8c7c3120ed4fb8278109640dec70f857612144eb4f213e8039f9c504d997a137235d49171c6320" },
                { "vi", "e45aa70fd4444ffc3ad7c6d73fc8993f95f3a1d5699ea9e9363979802fa7cb39e5790bfcff41899501e48f811153664939b0306593eeba874136db388b8eb7a8" },
                { "xh", "ebe0002505dccc8359a5cb79f02f5b0860a9e325090160d6d1080db3fa59757151ad8b60b514c9d37a1502250084c08697d9ab081e4f930f6b8e0f430c7fe42e" },
                { "zh-CN", "f2b8bde149ec4bc3b7d2801557a91693b2558459af351c9894aac605fb03d46fc788fbc8ad734511685243dba246956e9f67b26db4682cea480f513b489f6e1f" },
                { "zh-TW", "cc090d34a33b2d998972b077f8ee99321c1df4d894d4813d430afa8a690c25ea24e6bcfa5f47cb2b6ce99f5f219cac510e0a6cdb235986d33e82fcabc96fbf88" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/96.0b10/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "a70a688046ec457d14b01837750705ef552692803d1aa6815080e0418a3a72b7645bca275249bbb3efd1547a81de71979d7e2a87d8bff715eab64462690350e7" },
                { "af", "4e3fe26415359aa0cd162e7cc6227021f8d904a9211ea71d0d5570158bf8642ff4d5880b789f1fd8cdc8672bb9ac7d504156015493f200960f2cb6167dd2fa72" },
                { "an", "7ba109a09aa4016165be518623fc56b34914eb1203e94716a0cb434bf5ddc4650db0a8e3701dd41a6ec467a4fff839cf7096a1bbfa381643188bb1aa5391732f" },
                { "ar", "d3a7e401d60466de6fb894dde14f1b20149df3fccca11ca0e78b9dbd99cbf859145c814d109690165794707126e0460b3bb7271b6dcf69650f566b8e6e42c90f" },
                { "ast", "1a5951e64af53828bc47434c3a7baaf42349fb2a21865b2fa39f65c65a8f0fa5f0d95c08eb43523285a1f99d3f9fd260b5d88d4803b60c36c7bfc4fa51826311" },
                { "az", "0ec1e79dbc02d0e66d0275c5553b3489a279b1c39248f2564e345124e672f19255e5248dec1ec2666618e2fdce99f5da2b340104f638bd7a61bc940339a93b04" },
                { "be", "a9a45149154c9a17111964a516dbbf5912eaab570565edbb638fffff57de0ccc791e12d7f7cedd5eff46c4e6b85f034e10dac39b462abf9c37db66c20875c09f" },
                { "bg", "27253ed88ea1a4e80c0cfce31e5b2a831c6805ae81c7767ccea7f0a0a7e494db87ae994cab520c142e1c4e9be36348ad3a629f5abc2e776c86b2318eb6904b80" },
                { "bn", "c2293e87e6eecd9090fc4ef5a40c70a5e4bc572f03960d768210269799cdcb7c1b989f063931ad8a51e0f906b9815cb270e8593f05688bc443e008133bc05db3" },
                { "br", "d0616c46dd6bb66f7906eed95ef5035f43b9de5764919beafbd9f3b6ba273903fc3b9b81a15651b97dfa9733f5efeb7c39bd1ce4300271f37ee0cc07848d8e0a" },
                { "bs", "1cf2a39eb8572ab43634e4a731662ed872ad41462707ae4d930ae788e5381be315e8db17d1cebab15dc05c1a7a95ee5033fac47ee7ac223ab98e70aea9d8dde3" },
                { "ca", "2ed57585d376d70450e509d984864d9acbc5f746af152e52cf4cb9bf474d82cb273d9fa4f4e0b75ad93552390e63da68da98169a844d0d4babbcbfb55bd68328" },
                { "cak", "f8559b05b528b370c041e2a0264f5059694ecfd36cabc6d0bce6032d308b5cd229d4c741c66c4f4d38fefae1563025d4f1996babf1b6ea44c906e7487b543624" },
                { "cs", "7dd5ef3579b15ae77bbff1a42e0a6c26bcbde591d3e04a05e6b82fe894e48ba3418c80f4dce0e80a8a71030b9a455307a6078a9703fe5898ecbe0353f0471160" },
                { "cy", "4d4a7418c84cb0714949a1153f234bff80eb33620e7a17cf2806ef94961f4693265ff04bff7c4e7cd08150cf7cbd4e49ec6f48f2e2de285f64e1cdc9f060fb21" },
                { "da", "e9360968c60c36ab0dbf42a40678752711deee32e46c111ee760614fd6d53e3578319135dc1f673723b47375c1b64a823a6e402742de65b928bafcae877364ce" },
                { "de", "c70cf60ebf0adec3ab365d01592a692dc4932682c52a97ba6a2b931b85facbb0438a2789ded3552c7954892a50af511be9a1f9f51ff7c3d1dd56cfe1f58d736b" },
                { "dsb", "ba759d15da79b06e96cc82036e799b349710da903f1cd72d203bbe2d7e3df5570b093e0e95c6d86644afca645a0584548dddceb0f421fb9ab6dc180ca90f523a" },
                { "el", "2d786bcfaa34f8ba58dbdefb8ff01a025c9a94340df8acefb8d803b105baa6512c6f9660652f2b0802b94438374292ec2b35280c1daeb45ae1cb7a71022f0ae8" },
                { "en-CA", "bdb04dd494813d25b6d6d8254a7ff6537c7d52db024e0ee0ca3b5b8147e2b4d3d03bee29680ede2703eaeb43a4e8399fdeb51df2c9336496e5bfc90a7aa7d04a" },
                { "en-GB", "8b883bf7367b8106c21ff199dc661656c07b5973e948702c5f4603f735f41616b6d255c324b61f998f1cfde59ee832caf5d5c1864638e5bf6e861b57b378d072" },
                { "en-US", "9c48dfda65bef0e28f609c81a272ccbf8ea42026b39f724b8fc8b473569f1e7dc3d0e3b7fd37af4094edc4feb9ef2cfbebf0bd4bc1b02037813708b16e9c40e1" },
                { "eo", "115e6229c3c9295287211bd26da1fa280856de2d882cd84dfae28a40bcd500ec77439d7738f9473a7bb3ea78014979936cdfa5fa8edb98fb527f31bd7b173a91" },
                { "es-AR", "aa817b1aa56320a83f5a8691828f9307ae4550856ba3ab044edca82e2d0a45d251f83511328ba984fed5431c26a2181100387ab68d594a2557adb2eae96978c7" },
                { "es-CL", "b7d73312b0b88a6c5f86c5b55d6c4922b04ad112c442d217a6ddeab85fdfc0354975c49a527dafaa20f09c46f38e3e3a90e71946ef9c5ca0836d11e100c1a30f" },
                { "es-ES", "d972bb933ecc4063341ea0b3297de7160adb0ac0b15f5c5303e01e55b1d0cb97402e70832b05336ab0edc0cff46ebfa32f6e681d1735090b10133da0a963f6b7" },
                { "es-MX", "d8920928f9cd802d165e09df095a40e958e546a754070b5233c46ed711557028ec00e6006ce59bf64001f6ef11c8c3af7b29d53040e4d7aa3aaf537bfef0ca0f" },
                { "et", "ef5d309b77f484f3336d553c2ddb1256d400ad19084eb8f4c4b3813459f82269874be5ab3f679bed141d75ccee306b91896c9e7873c108f9650253dbdfcea43c" },
                { "eu", "4bda2f25e08ccd1002db0e3fa1f54183b2bc5e5dfccaaf52941f40d9d323a6204065df378045d917055d45d4e4db7cbf5b75641e205c03f8c88282382e71ea4f" },
                { "fa", "ce8d5b250d572a031a48ece5a786116b5ef5fb93b9f789a000955ca8795e771e534b6531f6b7c9b9726b93edd0ea078780cfffd75332722be743dd329cd707d7" },
                { "ff", "d58d4615df7261cbc640899eb4b07a2626f3436ae21fc600cc565dae9b4fb26e33051bc79cad16f10f1001e196d2b30e9f9a6ce49b67095cb8d3d3b630cf2317" },
                { "fi", "b82790f56d74accb5e3247650b997c52b06b2c5ce23f4492871e0e93ca952988fb864b431620a4f5f84f17ec699392f0026925951aed87715e67ff754f7310c6" },
                { "fr", "69e4962ce3fa43e50b2f31ef083e0613b5e0b5db5ad47a2ad42dd77fed83198424c7b481b26e642bc4e490dd1fe419ec9ac0fcdab50f2bfd913a5fa20e7ef3e3" },
                { "fy-NL", "a87aebdc8ceb64cab0224df51130ddf24dae585a04c15494b0ffe8487b3e0aeca5ff71cd9010628158c8b243c47145098f615673f03d1deefb18a81449764483" },
                { "ga-IE", "c59311f46bad8f27ac0f2cc7a1c219e1c3ab4519579af62b375ee25a7934f5451f60b942a0ed47a637cd66c262361932b7a6fc7fb20b7744d478f677d3bd15a1" },
                { "gd", "646ee389396d1c1f56bfa6673c23306291862ee79e67eafee1b50e85ed66471b701f6cd0a14703b90c7957f648e45d135ee0bf02fb50c4403b7713130e533c38" },
                { "gl", "2850d607546518221b6732b0789a80323cb234e35254c54e622577487e007b3965cf386510d17caa7f71c2c37d9639c2928d6ecee90f013d0d31c6e5358246c9" },
                { "gn", "f1192044e9fbe006e8f48daa54fec8b072dc09ab040af905af7143f2abd3490ed9f24c63f06738d1c2a24e6e870deb14b0f23079c46966c8f809db1c6f92be8f" },
                { "gu-IN", "c807bdb818a2bf1c64c893843bb79fc7275426affd1ae566d8c7dc4b450f6130445ac8bbd9f312aea73b42af867385fce8d800c939b59bda0c3f4463a07a804d" },
                { "he", "62c870ee1370e0f2a759dec31a0576a7b0f2f6ca1a4872a0636f30e437319c8a3f2662d3d0990073306aa59d04125c7fac02c2b68736e315272a892ed10ae09f" },
                { "hi-IN", "60fd97428632332eaaf4c9d08cbcba9be55499b46dd19e117eb9be82565d4dc686998a8d6717be34a848118e970c2b4284243257444832a1ba3506e4fef25a71" },
                { "hr", "d08befc677c3326814704e7e1c4a0f90ebc87c88ade47d5a5ee3ac60ca3d2d62448ee02dd55b4881e1fc20acc2a11090cedaef08e5370e883016a1a282f50e47" },
                { "hsb", "83d3972b68decba1260ba6320d9ab483e7636000a3363a691f2ee4a0e7d5233bd102fb901e93b5be954490bb0db69fb55d4c0de5de1e3b455349df11574ad8d1" },
                { "hu", "4dc76434ac8d1adb25a2ccf70729a0fc42feb974537e197bb4ee1a3fe7d6570e0ef55de9e4793539e1e0811dfa93cb09e0c39389e56e871faafcf7224b90ea40" },
                { "hy-AM", "01df1d26996e4109bccfc910fca90d6be928aecbb0a8a745c949343709012e4f932a93e6f66707c52ecde8c985240541ade4af5c40982bba732d3ce01181cc20" },
                { "ia", "65b89afea8dadf3b4a15674eb92b7edcd0a7e84b1ab464b0de83040222efb6fc34b9aaa20328d53c0f857e1d6a7a52e7a4c6bdacbc004e62ae09735e346e3347" },
                { "id", "d5b81445a656e3c003887f77183b85635a778e7e72de73d2152032266a2381b6aa6223c5f1b31d89c577a7d6270aef3a8d5c26f827f573f1a22c3d61aba10f70" },
                { "is", "b0ed4f187053bd3a121746b809351eaa43ad1e22ba211c1cb32c2bdb4a99375cb89e24b56bc99da83d905898882d78182d3968f74b736f653699326da7173399" },
                { "it", "86dbe9470cdc39a225fd7ebb455d3f4bfa4e71d10ea7c5c5f82f2f10fa7eb9ef5e322bb0b180d069b6acbb7da678c788ed0a15de42b1e264835715bdfb029420" },
                { "ja", "dcf085eb880ed1eddf5abcf50be97a3f0a6257502b840a8e5ea76697075580fb076c09f2f13aa215aec06578b0499d80156b3f97979f2fd1958667c7e5674b03" },
                { "ka", "9cf98def6053faa988593129d60a04e93f8bd4c26236262edef7424b4d2a3ce3c42c5666c632787da7eef4c3396e1544145550234e559dba5e406da22bbe468e" },
                { "kab", "e1e157ac434a66cfe6a2eb5dafdb9b711fbd33572824918449c996531e2e523ba24ce1539e36e03e6faaeb1ff50f16433bdae91d4d979302f9cfd213687a4338" },
                { "kk", "531bbffa7bf399ea0f46fe117f641bbe7801bb2437e38490ef2f104230e21762a31b313602406749a254e43611cfe8c979c4f06cd34ad53682e364f3d39eadb3" },
                { "km", "a33eeca13d78cbd76b091a5a630e261ba9772cf9236f7ef5fd561c0abde17189a2b4b5fe6a134ea37002818d14565bdb0e7d844c763b23244778ceb37812b628" },
                { "kn", "76dfb80d0918cf9cb5eec2f3b9c0c23aa3e4ed6840874fc6e62833258ce01620f8870fb49477b1f1e70c6d549f86a31c4be879b78be95effe90920ebbed57344" },
                { "ko", "f23eb05a72b8d1f338a9b8b5bd1ccf0dfdb51a6b472e7ea56de1d868a0c63dd1cedcd71a485cd6ac7f5f1bd5bd2672af42a4defcc3c625bc15a2f4fbe03301a2" },
                { "lij", "5334e14341fe4ba5c76c797d8b0dd8e4ca4a9a5b9f2c9582e35bb6a02be26aef6e7f1b8d5b68a9bc0303bde5728abbe4b618ed1d7f45145d2f9b6a3711321ebb" },
                { "lt", "0c479fe3dcaf621fed7601e7d9dede27c429ecb87cdbf41882e97862c7cc35d06456aee56369ee07f79d8a9af725bafa8e98de8ae20c9aa3e76d768e3cce8547" },
                { "lv", "c99d44a72269306163f482faa0d054f0686871d3451a993afa37ac2aa78b726c1dd58b861ce8d16f4956ecb04a11af54f552cb5503c61634da6101a4cb3fe8dc" },
                { "mk", "02b80c9530b7e7a2f8107c4de7f269b541c4f6699e8058e81cb4693b966f574a341efd7806f69be2b866a1e2d212c31c6bdf4363c7a972f4318aa6b8c5297149" },
                { "mr", "cbc1af485a3919c5a63ea5ff80c3dfe7169eb42518a254d366ae08862565df366baae2c2fde088509e069a336dbe4c1c75c4f032bc621a126b2340f0bb4d0e90" },
                { "ms", "ee4bbb8fb3fdc0784f204113b25417aa169e22677c39f34b55dd9bfa170d22a7c5811d6327cc1a8fd3512240365bc002a6c55fdc9df9de4e240bded9ca7243e8" },
                { "my", "9e74a06a7d3eb55e7272c64dfad0e02f81ef594db8b442b50c1d29df3c49e6355f533eaad41b4ad33c54510ba0e57bca439110d4c1ad823f595ee23d9db73aac" },
                { "nb-NO", "a4a67dc3adb129d79b30f9b1556d8b6e5e50bc2e7efc4c1f5d643d2a30f8a163b41a61bfbddaa4fca6f8be2ec1beee449fd99dbecd6a2788aa4ae5a41410fbe8" },
                { "ne-NP", "ef69997ebc91911817b0e6c285e88a1b0892ffb42e29c46e1eacf91c32bc7567d3b78576e27971fddf571e1451668701e73f9a880b86592dac180558dfe44b45" },
                { "nl", "6d6556ef80af439fee30f8cce1e39380acd4f5880fe6eb92d803fdc20d8b04c87a86aad8136a29a25a219cfcecb44cb679c621d115aa245e9c16099d871e51c5" },
                { "nn-NO", "c0b7dcb6700d7f81f277d41744299bad457b565dff9fa8b38db1cad981cc18d2647454bcced782f14a375a0a7e88c41ee3ec61cd4728f5e6c64db1b0d5305132" },
                { "oc", "7f398f68c6a8df4481dbe3a9e5f486846de5cdc74c9617a41cc39f1a24e98b8c9768c71bb43bc6dfb7d0520d072c8f6ff9c4357090d22b403b2181a832a7cdad" },
                { "pa-IN", "e3d056d8212bf9464c429e4d21c77d2d6f2439845ae72a683751237192d0628630a474675f85d536e06d7e08bd54739352b66d6e116fd50f5268546798d783c1" },
                { "pl", "bceaa564373952a37ae9d93e852be3604063d5b7e2fa48a1119bc3caa4669f40369f3617e02197a5a98a7b6d979cf9872ee1558430519cc63c910ce370e91b25" },
                { "pt-BR", "49c7b8d3b62b29e2dbbefbc9a9661704757ad9201e3db4bfec4061c66c7e614a1b69fcecaac7123fc9a3bfa42785052a273709f0d4db14cf0553805d8d5677fe" },
                { "pt-PT", "16f28e55254be12678640508ecc6d8a7e6467b22cf309a4d82d9a55e4d137e5f38b1c2e311eb111f65616838831466729af9311f50a9ae082942912e7f9f7216" },
                { "rm", "e189d44f2ee92761e9dfd14224702734e0514d9fe1c47089f99bc4dfc3ce0944e1cf2b0a2eb8c2ecfd17d28df0a4abeeeb14bdfa3ff20cc26f089fbf6c3d2b39" },
                { "ro", "b31919f0bb67c761b8615e35c992cf967feff48f9ce99b72e4d59d6e8f502c6d1bb77b08be936df645796feb4600b75319d0fa7070ad3592fa701b9b573f7452" },
                { "ru", "8b590c86d9720ca8625a6b918f4140ab3527cb3493690622fdf1beec044460d007377cb1fdb2662fe1ed535f54101a579bd68a5bde9368f9c3c26e91b9aecacd" },
                { "sco", "4efbacac7271d92b5cf65601ec94805704679d5e2f942cceeef1cdb57a117d05b24df18891b892e1312376a0d4464a67bb9e9f38e33331a9310cc99dd7763e98" },
                { "si", "fa33bce238e3b3709757f8de50547310b768cddf7c54c24d568445bba1d8f37186299c3b33b5fed0c3de78a1340170a488096cb030f301bfdab65e217a0eae58" },
                { "sk", "95cd180fbfa194d2e814655b4f27a88e39f52d05db6cc8f1446a4a5b22cf942eaaac694f18524d0262330a2d5455c301be7810ff915c742d940adbfc9a63a9ff" },
                { "sl", "6bd2d8fbb02606eaf5d613fbaff014f37e74dee9175b345084f91684ce19c60e12ab464083f99c78cccd01d8b9ca64874a34d02c044f83bee392eb34649b4248" },
                { "son", "c46bb7b15bd318a579a475e73c744da3315bf7f4e95834b7ba9971ce8e2814d3f3a7818d0b00d1c66c21a16633d0228da6f37ec7be8475aa939a04c4f576eb39" },
                { "sq", "42e8b0f306b65b26d731b9801e0eb6bf1c39bb17fa5dd937308925d920325f3d33aff9220efe4ceed8a56505524c9be6bb333d664d62e70c4f0a5082acabb871" },
                { "sr", "312679d5efd8b02cb84bf09c8f8e058893bd341010ce48ed4ca0a3a4bd0d31553a116b4dc47f5d5ab2a03b0e9f89254faab7944f03d03fed5f14a2d81586f9f6" },
                { "sv-SE", "be0b66496a8ec89507493aa87239e92dd38dd8d0bd69f285475119f676dbf9b558b62ef1a8c57c9ad16292ae0626743ee9421360d66235e7c22e99544069348c" },
                { "szl", "db362da0831a3013816b9f71809d3b951a4b7fb05006db7d9a356f3d5dfcd64766b08a83fee75046bbdfa5ab7448f9451854f1ff205b8e32421c6527201baba8" },
                { "ta", "978a841041c9b9aa6da5c3c97cc52d40c0b3247b3a61c3325421880f85b442a94697ad4c9085ed54c15ee4706c90f90290d95d06a058debc9faeb01e614ae355" },
                { "te", "dcbf5791007c3c204144beab07585cefb561c6eda61bf9791fca16fff38145f478a077254ea245f86aadca8a10c154f053d3b5e543ca39a83c19c7ce2e615433" },
                { "th", "08c732ab77a3631d31bd6d1844abae1ee263f88b3c84eb657b8b7212bbe215adefa6c837dd6a6a4ffe2671a15b31b283900270f59f1eaa434a4825d39639c8c0" },
                { "tl", "5cb9e126ca4981994f5122f4c99a3a76c432fa261dc5195bcf07464f82dc5ec71112cc1a6f9a101b10e24ae1fd54eec49cfee319d98eb2d4218e8ebaf2010a9f" },
                { "tr", "e2c9ffe3ab736203fd12f553d9e65da1d2f6325378590eb906005661feffd2b2649b3cdf78ca349c7daa76aa8419a900bf6f5651bd6c4c217bb6fe17b82230a1" },
                { "trs", "278c0a190ccb172d82e7a5d97200bc65a37888dde96370407b9e7e99978f1a39e650f4dd1025510c9ea1c6498170ceae07a0551b36b7baeaf9d5ecdeada1c2a1" },
                { "uk", "1b0706c7b2d4ae9a0d8c9184c87754c33b253fba67d2fd456681c9e1b6da864a4837d746fc775648d6bb5997bc366acb24873995a6a2858f56432de36d8d6653" },
                { "ur", "c47216f6692a75cadfbc5c7f65bdd1b50f82abb586cf876006d8b5f40ac6fbe652f480d0e9f5900ff0ba7bd5359d8d8a8b319a8a754f122a9079259dac9e41d0" },
                { "uz", "3c7bbad96af19fc9a57dec31c730ded3d266882bbc10b8b7667ff5b347e41b5ec352a4484ac4012b0a3ba26d3d915182b534f2c4fb568f654a97eb719f9be131" },
                { "vi", "e7699166e38619d4d867b08886fc0ae75d610aca7c811dbdd14da38d204158a55462c046bc80242fd0820aec2282e910094b93ca66e6b886d1ef0a3de3519f3c" },
                { "xh", "bc933868b1b4daadfe73f126d531acd85b96351cad8240d6943b7db7dd4e6584633648e95f070cb99aeb4d37e4983c10fc9055f6d273769103fc53334ecbaddf" },
                { "zh-CN", "2cc83afb90317756206433220103e2d122b566e8a0dfdb51ccc2efc38831fc6673a047853914a98630fcabef682fd7666568cfa78166440f0f71cee9d569b161" },
                { "zh-TW", "49c7774a6506de8ef144306e4e2992230581db1cd8494f1d36486252815f9d6f3e12b7260946809aae697420114af428a4b39ef745ce594f902d4995cd75a201" }
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

            string htmlContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    htmlContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            List<QuartetAurora> versions = new List<QuartetAurora>();
            Regex regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
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
            string sha512SumsContent = null;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                using (var client = new WebClient())
                {
                    try
                    {
                        sha512SumsContent = client.DownloadString(url);
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
                    client.Dispose();
                } // using
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
                Regex reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value.Substring(0, 128));
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
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
