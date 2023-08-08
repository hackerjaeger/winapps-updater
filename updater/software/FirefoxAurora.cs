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
        private const string currentVersion = "117.0b4";

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
            // https://ftp.mozilla.org/pub/devedition/releases/117.0b4/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "9ffc182d44ce06e7594cd013aa28e8fc17fd1eb65a88ea70d9175f5e9bbc9fd06ebdb09640c9f50a65cf837a9ec0e8f91a88fbbe0e17fc91436457673123bb39" },
                { "af", "383d209d345413a4a423f4570e1bab7006a5a957fbdcb98f9b33980e81c0bb84943b5908f1b5698ce295c6e1aa0c38d6e069769df61b3989b885dc8a4ae5ed5f" },
                { "an", "09d9905712b4095ff82e4c60146dafc6ba59caa89f6fe245fc8107e1c012954a4708c98d4e375eb0847e230214bb4e2d38799c482c29e653bb03afd946ccec45" },
                { "ar", "63ccfb12ef2ee608cefc2b81093b414377938f646b884d9494d8af0ecd70bc146cabb875191dac94b962ebdcf7126bef15f1b231bef2455337609f1d36ffd631" },
                { "ast", "cd4e79437c355f123a7c79c273cad1bcf40736c0088c401d4411f9fb722e7ec8217774b10448e3d246aff0d7aed93813996ed2dfa606ef9399122651a454992d" },
                { "az", "3fa08d084f670560b52ea0d3fdad904d462b3325e93f99357aa703f2f63f27026e39e3f04433ee135888337d980ed1016a078d60bcd316ee082661ff06e2b329" },
                { "be", "03c0ffe297fc6fc601debb62c7db0034e603e2c11fd32d6f1d2de438ec4a7ae182cd325e59a92b8fe497e1e60cb9d59c016f93d62c4e965549b116ee973e9654" },
                { "bg", "65bdd4e67efd4f992c658061c0ae10c3a0d2c4d069cea105281bc5fd7c2e9034f3530e825a7143efeaa1a0d53f7083455351d3e74aa8be2ab561d93c0d9e0776" },
                { "bn", "31471a864864441125b2f8e414e75a3129da09804cd7862ee515e2311546ffb39a80ec22aeac890dcf6ab5e596b1e78241a259e5346568707f8c575ec1afafa9" },
                { "br", "24eea939030a7b782894ec2a4e54946734648678cca9e88be02e594d1b97e703d726a8f6d378c456942196b3ad5d1525861d0e3885f650330ca190f8ed58379c" },
                { "bs", "47db9a2b2365ac330ef922a1dc90ac359b7a2134c9a05217beb3135c9359b00eeaa0abc003e540e55217409bb0d4458b90004b709c5b77ad601f9fe584b9dd08" },
                { "ca", "23e0f971b2a9cc5977ab58b2349dc283e09884d62e2caed81fe6679a17228f0e24e9d4d76dad2394462baeed138f8252b36446fb509d4f3df7bc00b0d9113f31" },
                { "cak", "11bc4c1ffeea6c50204be132a7494d61028555f04893cd9a3dba098ced44ef6aea1c420beeacc46870aea95b36954a4d548d45216ef530714372cd3d2ebb3a63" },
                { "cs", "e1e164a63cc88840c23a8afcd73eef56bfa8a9820a83fb409692e1221a13f526aa80c165b0fb01167bea635daedb54ac87ae6aeb318def0c4a8ae2d473243cd5" },
                { "cy", "c4d2fe344720f351b4abcebb8458dda29249bf29502715c94d523b37402e6fe92874a0295c3bc2442a30d940604bd4fa2c873e9fb98b22ebb1c5f1f02cae29bf" },
                { "da", "8d0a64977d8e955dd816a61190d5d895141294cde21310934177df2becc11a9cb97fa1ab5eaa30648ca5b79e1c17b44c510180baba9afdd208d4baae54b72543" },
                { "de", "4192d31ccef141f136b4e09a66592a5d6b22bec3c10e91b2f8ca6306ba7cd137758a15ca4b4f234699fff16b79d0ae4981cd4cab737b3db3748d626005d300ef" },
                { "dsb", "775112b1733f20c8e326393eed6f20b7d75df199068623706cd7dc065a029ed0df26f77383c05f59fa6da4ceb6c719e046caf504ff32cb9b95e1d9c5bed6df89" },
                { "el", "f99cb5d6795ac2aa8aee5146b67a5f1bc1e9235f650e7e63493a41b5029800dfc9f4fa9f40f92ea71d2a2a63245f03d016b4b6ab21f00b8e8d6e93ee3ffa2013" },
                { "en-CA", "f48dd00921c8d4e4784ee566763a4cecf1960797f668dee4f75dda1441baff498e154706cd34a51c6609028a8fb8d1a4d60ff0b9b495521edea7ec46977f78eb" },
                { "en-GB", "5ca30818635e4dbbe9c76f1429ee67710b7d9b7dda6c8c25b741e3131b13f08c51a23b411bd4e80eb85b7e430dd28770e8a20154739e56fdfe5ff6a31d4303f5" },
                { "en-US", "f11ef312a701785a98c5945155235ca1136fb8abc9ac69cd8f13b43c366bff816ecea5ac8bd9acfc408d829bfe6f00ba3ab8e8c05239d7d5bbae4c74cc4c1dc8" },
                { "eo", "3c853e0870696eb4b58829bc39d28d1f7f11c54ce5659c0384134e28a6d8e1c63349d8cd9d5a37f3e14fcdacea483e73abc9aeddee3b082b3c3c2cb7113dd728" },
                { "es-AR", "271a9ad23a149880391c11dbe42df9693ae9842b1d846e39b8318715b17e89aa82c287ca5f04d799b85d4775c24aecda14082ecfbed1988fae44e54e28c07ee7" },
                { "es-CL", "788dbce35ab656a3dc362d10d0b6d475ebdcc0241640eab0cc8f6352061ccb12494365366f5459222a991c7b166d5d175ced53df727fa9ca2bc834de80849e16" },
                { "es-ES", "ce6e1854c983fe3314dc1f848655c9ee1f3b1ccf07cec381f2f9082f459cbf2d26121f2499accc0366f3126d5681e6f0ffaee30a006b0c205670425395bb2dce" },
                { "es-MX", "8ec3882bd744b4500846d3d42737c513c533ebe08245f91cd918e30604c85d6a56d8eea4700ebeff66b981dbb9f454b2eddb712e6cd39917517f61f690ac2a26" },
                { "et", "9afcfb6a6e43f2594287d32b4055050b0fda360521a774bfc0cc8ac5759679449067d8079e653aacb93b33ad7885d3c333c20d0b54acf4153c91d7fa7bff5baf" },
                { "eu", "ca422820423eb6269190da9717a117909d7de1643396c31c44854dde536bacf1e8dac93fdd12d73fa6b6649a8b4d284069d00b13b0c484cdd0228fd267f5c6b6" },
                { "fa", "5af95dceba48a8113d626d146e574be90833d0bde96595ce95ac87d9f6d25d4cd48fc68a1edf546cbe2cfd627b3f80d234afc5df46c41ea6033f8fdd66ee7198" },
                { "ff", "1a7386bb43f93dc23daa9cb7e8d7cd712052ecb6dc77f60082525d4c67a8aa22ee9a8eb304ada604e9fd18f17224a8c14c1646de67c35748156abc9b5e961130" },
                { "fi", "3ae1fc6adc10c56a951358fa2245e36e660a291b6a252bb9de0e1fd37e003690ba3233eb2d9880776fcd63069b01c064bdef5a989eb0d9990d4a66c867ef79b4" },
                { "fr", "c72b25f8e104012e83e45a956cce6a4f0388743a7ffd1a6bf0f39b6875a06be6a664d5889f112ab854fd5d11bff53c045ffa2699c9a6997285087fc95e8ef432" },
                { "fur", "ca93579eb8d7fb12161bf87cf9179beb538c60ec3365a5266eb116f6471cd766a801b4779c951124c6c2dc74aaffaf099ea986fd1a1448464013ad7443842469" },
                { "fy-NL", "2ee7607691fbc8536da1bb1575efca8ceb05489d03bdbb287aedc5b66f75515503b421a5bca2f26fedb1ff22cfcd886a9f0e9c5205b04d6fd3b046f7fe0e23a5" },
                { "ga-IE", "63f15c941325172abedc93e84e0286754919c293caef109f94eead32460d8d89bd66b1ca9f7f85b7cc558cedc3f770e71dba4c6a1f8ae990c853ca29c0d122ca" },
                { "gd", "5378b41c63d39317c64213cf8514240a9c5dfcd6321f373b3f0733602d69041369105d42bb1450c4761880f075a59823fba775c2483e5ee15fd5869e2a248772" },
                { "gl", "fe5e35d114b16d6cd9a8790aa55b1378e2cd1ba492dbd39d4a02205d8823f1b593e0c93c40a9b4015a44261c100b634f32ecb5563ce4c4f674fcec4d9a5e055c" },
                { "gn", "2709af33b440232996494e19f91951fa983546cc437e38db7ae26256c177e16e986cb5bf9362eb3eb6c46d17b3fe7054d3f3fbf84b44e2ef14feb1a37177903e" },
                { "gu-IN", "38005785dfc315bba9a840629574a7f0a9fd9c4af94fadfb20ad46cde85cf8ffbc10ac187675f6ce310b5bcd492e2c9ee985f8df21a7295ef0e6614b020814f8" },
                { "he", "483a677dc1f78e450d965f11c4e9722fec7915140782a1d28c65e31fb1e1d0c8810f0fcfdf439f4cf70bab16d8411e6bebc6f44a524d63ca7688a4ba0741ae42" },
                { "hi-IN", "631cca3c63972cb3db514ee450fcb2eed0f55667f9914a6e4df49e91bf699112e84b22fa67f99f46924dd10eabb7902a299a9399249eeb849d27f82ae72941a6" },
                { "hr", "41819b3ded98b3e3eece751fc9c5b783730ecb446eff23b88f849a9d64b50ca05aa38d90246a6229d3abe869b1cf30b854480b993c492e84cbeb751e411e0bec" },
                { "hsb", "ab2a2507cf343149cd5bf3a45ce2dd2a2f69b952cc9d7c80c40146c291b6b25b5b41121965a18ec704f7e98e0c874724cc4069011ea38a8a9e2983d6f4e1d77c" },
                { "hu", "c2ecfd5654e24a3f09d32c7bc2e2e7b903fcf94efa71303b31ce10215209f7a69d877f370206d8f644fb58758bbf5dd15b0ee3b0e905a8f6564bbf5c6efdf787" },
                { "hy-AM", "3b58826d568a0e1327aff2967da9f8dd8dacbe4edd4bfed1272e47b64f1963edd43a23215c9889fc2a17209079227c9497e593863351c6c9e75c7cd17ee447ca" },
                { "ia", "3a100ddf2c65882bdaceb403a956203876239210bc4c9fe436dc09433a80c89f5e00d50e36774352806746051e22d3216af5e94b818c58e0ffbb9d83436eba85" },
                { "id", "1f63f1193cf6bf7587217fbd0d92c2debf6a08c60e4213abd7172ce4385ef0aa6d06483504f354e361dfd3a154f58f425edb068890275d61b571c8eb7ce2ba36" },
                { "is", "e69a163db1824ed88843196891d0441354124d69c0731ed18b7f7c1b8ab3605f6d603a77857f88810f8e4f2fcaf8c2d788626683707ea65659c580fe96d4b8fb" },
                { "it", "393233ea5bb4d1faaf4bbeb1c5afb3b2c9c90992f2767f05873b73b93653bd0db9f4b3a4c424016e47f24e945219fadbf91fa86105581f33ec25ffd071efac27" },
                { "ja", "6378ae17e10697c4ea27daf94d632b4c3a6be2e9baa0e5849a013d768f92008e49718fbd6007a088502a1dcfa9681f2720dc1218fdac6c3299da41e0edbf3e25" },
                { "ka", "906a89dcf3a6cae5c3467c98db51a13adc79e3e62c16cb500d59414a31607046eddd4bcda7fbf7156e3d899bc21c3844b19448e6207b55880c1b3c0a6abd5768" },
                { "kab", "30c1c6586ded0cd31e071af4ffce2354f29af2f4451620396ba928e3e76d271305527780cbd51edf82ebc3f696241ff1aed2263588ce0716faf7b5391bb76aca" },
                { "kk", "afcf15e508d9ef396a5043c210e6edf18b82736735f87da835de113fec9fc5d412cbb910db685fb4d3c1de394c159c3a24fe2e70d57bb996ccd46393b97783e8" },
                { "km", "1d539c1fd70def0c363b451be06f4cd3bc1a4b36f8928d862cd30762d1e9abda55313a40f0d56280a1b4c82151ba267b5e4adc91e93ac2617d130089a4181369" },
                { "kn", "b98cacf780dd087d090b80989f3044cf5e346e8af13dbcf504f4e36bec1318f379dfca2e9553626579e97719db3f3aeecb80d0fa287ac99101d5684bff236a91" },
                { "ko", "aabfd825cd660e4834437e92d27ca5014276f2fdf8802f08fb03fa6f40322ee8502b341bfc80e592651517db731d5c53098bb581ead3710b9efa220f0d09cb1f" },
                { "lij", "68d12d9736a8b398da47ae7b664a31f52a5895a27f59a2951167a65add7702409e84d6a94013f3ccbc8fc663b791f47f7e962e848e0a0ad4305aa86af26c3c02" },
                { "lt", "447bb7eb7c1e733bea5bb2ca85c26b227cf848e6d4be0d97a2d11a3abf2f5dcf30a37cc45b4571fa5282ffc0a8fcd24572cdf56a156f1de87970d2551c90e913" },
                { "lv", "d71f9c8ed08dd2ebf7fdd98e12ea1dde636fde5e81fa0370e1e6e463aedc7cf6dfc45c57fc5a051b984dd2ca2d155d961e4e861dcd0cc9dcbe2c1c9e03cd2c40" },
                { "mk", "74160d2e368a030dc83ca54da9ebba4fb8dbad7c2b190a75eb2f7a1ae8d7ba67dd8cd5ecded2b6a6a5049f6c9cf4e359758b71fcaf57c8c1e2bcf04ea0e816f9" },
                { "mr", "00ca6088122b60a01633c5ffb27025768fe14d728721387d8557481d1512ffa09cee99d7325615bafa3f8fc9cf549dcb8b4213973d5a0c4056ead41009e5e492" },
                { "ms", "91ee1cc604a8d8d0e8dd77ed7cb29bbf2a9222954977d787b32a01967d50c64e5abbc66b39b7a079c06f788d2925df0d0721f41358e85f289a59c42269cbd543" },
                { "my", "2fdb0cd48e1e77024d2be7d5ba987e03cdefdff2e01b26ab82c39bccada7f7b627d18878857026be2b389635e66b4da6f98142502108eba7f76af5779085f840" },
                { "nb-NO", "888a0c608294c4078d6aa5e8503630496bc1f5c97296299038f1f0e75aa10c03d9a0545a09ee8c945edd8ea58be700781b2c12c36db9bf89db86ddf986bf2a4c" },
                { "ne-NP", "a0c6f6a550d12c6ca10e803d64915ce9cb6ea5f50cb077ab956b5c2e228f65cdbf395e8bf551f652ccff4256ffbd4847d8f6cadd0e856d6dccba51afdb5e50c8" },
                { "nl", "91d1ea90e841af060c0f245e2b7a6dac8c9966f08fd36112a11aedee88bcffbbd9b17c78b31074b5ccb34fe484e13ee8d7a073b2074f0a395d9db96979a5d0df" },
                { "nn-NO", "eaaf319b2bc4b1c425a704ce88c9f506bbb69e2878332f4938319e17c0eaf1b441440680a9a8343c7a743925862e1760df494f84fe0b6cfefc22404e4f8946ab" },
                { "oc", "fec7e164483d0bdd6799301846d3759e77fae35121cd93c1aa2397e8cf29c3bfc64b84d9117dbafaec6cce88a250504604e1c88fb0dc3c8725cc8173dbf2a060" },
                { "pa-IN", "5f00318b4edaca48382d22a6faf0004f0040bb89a93ebaaf8336b863809de42057722273649def8cc3eb76b2cd02c477768ec57de9fc2e12890ee7bc228d4606" },
                { "pl", "1f9e59cfa8b6d6279df1e0c569b1c2f3d3c206b99b980ea6c0190ac9c58440f14959d99e37e1accb3ab2db073ed2ff2197decc320c5d6e1472a5ca7e89c2496d" },
                { "pt-BR", "78b52d00b1b8b7eb465a214008c55be6dcf4fa34fcd34828e667e7bb060b90373beddb1fcbf4bf80830c4a8625151ea2a40d9ae3e75d7cbc62c33abf668c63f0" },
                { "pt-PT", "cc0b1e371b21e3b4497f81ff447d7e8fc9614e367a79e3b6c8b7c9551e38e17478fe500fe2ba1e3a93b3c9e44d91cf30421bc281918f1e3ac4a767a94ae837c6" },
                { "rm", "d5001c564f88d60a1252303bbfe5adddbd0467ddc4fa8f2716a6058aafdb900367652ee34098e69902bac485629ed72342ca8adca09d68a4fd7152a3939b818d" },
                { "ro", "a8e4ba7f0b195c6e5ccfefb942cecab5b45b5634e39894bbd37b36f1a1bed9f7425a5d10d5205c12759bc098f19eff1cb7dea75408e1ce0f5eadaa10b0f03f68" },
                { "ru", "2b1da0b8d47ac0e8dda6d98da2d9b4c77d79da8f1e888a2e34700bd9f6543780300bad5175bd73bf53e97687c1acccd8f3dafe6f3891ccecddffd423952a6ea7" },
                { "sc", "7cd52094ccfd84edb6aa6dea9285dbbd47c8c1694fbd183111dd753d3e06478bd903abe8181bbb834b213d5611bfa5f919ef0e28853a2df5060d888043ea9ac9" },
                { "sco", "fa296a475328325c9833be7ea5cbaee28f19151844215619ce93da2cabd18cf05e0e791e5e0ba9764f4771a205ce435ee1e0a4e7c3111e38ca2c56c2f7e759e1" },
                { "si", "1b715bc19a0fb76a4762865e54d2ec71892be9d1b217c3083293ad57dd9a990e9bb9832cea1533aa3b11ead33d8a1af3e8f9c921e264b99fa7b764b7eec8f3d7" },
                { "sk", "caff2ab904ed006152882e0099d25bd52bc689018a9bf033c391eebe568c9411e59c18e07087136b4aeb7e8cda96f40250f1a00a0a18853543f54b36c1204bb2" },
                { "sl", "ae6c30b83e1715249bd51cb6a207954ca2aca244dea3a618657ef49cc24696426a27aed2ae927bc3eab0624c6a8fc8c4ef810e6975b85b7b707d648d7a88c83f" },
                { "son", "112459b4687ce3cf55b37b63a24229d4c98fbcf4c086969fb0a78db8022ebf9d632a93ed09a1a74ac4988a139f5ad569cb47f4fcd13d2f60daf6abd452881ba2" },
                { "sq", "3bc7f1a0d546f6363cfad1a24d8aef448c424ccdf76d5088bbb7fb0eb62b117728d4861c7875c659190e5a32d561d6ff4f5f646e21e5e7061003681578faad1f" },
                { "sr", "e495fe4547b77a858cd0d01771d32d41afbe7643961844f1fcbfda04c39c4f981d29c196c4852ef70ec0bb4384aa484c7915fcec8e814e581782babf70f1b250" },
                { "sv-SE", "f82db985514d71b8709b66d526e67de20d7eeee22ced88f564d747ed246f83b8ebc0f15f6a309ccf53ce9228fc7060c6cce833b3e5ef7ee3e2732d7ad0c241e0" },
                { "szl", "c57210863448c360e81e2a36e91b67337b95cf907399a82c7baec860417f2afd9ed8ae820146909763425be7fdc81d3d5b0eb3a0fa9cea29c2d8b0e0a206ad63" },
                { "ta", "94dedc37475df14fd4482576f16e6aefc274e537cd8255c377c841ea98ad87fa3f7922f554656717eef35ca642a3dada4d4c46756c9b60a5d81536e091e98e9e" },
                { "te", "bc159aa2288959006fb02ca215c061d4641ab7e4a5b019b1552b11dcf894a49fd396d698ba88b60e8973db53d6715ae5df7cd19db1828dac79083322b3c9126d" },
                { "tg", "77d914e667ccbd325e38f364c792737b729d2e20094e2ed22dcc1cbc2f2c3c7238b4ebd36fae12bb9627c60f1e470eb47d3ecd4d203fb300126f4e18ed8e0bb0" },
                { "th", "4d5f83006f726ef7b48206b9632aecaa513288a99e4ed5709c229247701ab055889cb071d354fffebb122ab7f16b7d95938f54c23bd8111af5e48c48841b62df" },
                { "tl", "7af5a2d2ace65da054921311b5098f521c1e4200a192be9b352876ddcb20e380662913d79e340b2442c779b7a16752df52050abbb680ad8dcce69b36547c9cd0" },
                { "tr", "aadb8ab2684641f94c69712456f1b4d064440d5fd655ed4696f269e5b4ee195bfca5e79a8c4455032bb688346e16003349b990783e01f8354d62624aea776a29" },
                { "trs", "742a55d274809bfd65b49f2bc84a49754ea22355db2c8536ba85c669aab9fec12b545ba5b1fc73a4b55ce7e5f48778cb2249d16a2e12a0e45a642e41e1c1f672" },
                { "uk", "04080028ace1caaaa2bca5e7be88834d73612a0d353235b66ddf47b803ed8d5721fc701c613eca9ffd8fd3693009515ee93941270b79f3521a24b931997a07e7" },
                { "ur", "eb3ad85d4cc25511b7ea3cf2c526f2362d806775d1a2223da3605396bcf822043be2bf98d9683023fde3b4f630b0c0f5af19cf27d928897f5fce547a79846998" },
                { "uz", "1b1e826c1198432cbcdba9d3a15547b7f7dc2166486fc0a8ce6a4a1079e3ca9e275a6dbd3e6e81b048baec405679b8d494ccc54050224865a4ffa4bd60c2f846" },
                { "vi", "57d9fcb29506f8bb2c8d30ab4bb76a17f6dd0937244d568d2adce7ae6dc71eddce6ae5ce713f9c0695f7dffe7a24331a58f1e4790417d766cdafa23c10506f57" },
                { "xh", "ac391c594a50549c3f4c0534a4b0e674a6840ce03eb86197827e66e05008b60b0716067225d2db629b0f388de0ab4e7b947e9c43268fe8335b2404557fa17659" },
                { "zh-CN", "8638aa692bd47cadbb6fd81eb38ac78c4a6a38c39d1a39ff668ec29530b6774a0077a75fb76e7016822ee341d01c03bbc657557deb15aa0ab43c083a896be874" },
                { "zh-TW", "065d83fd47794a0d0f7758a359a195499f63a79f7990af3b28086238e2f2b0a177ac5a1f99628ead396d5d30a9b898488af0538d146a3025e6d7c36cb69a4156" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/117.0b4/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "f0085dcbffdabe9aeed9bd29eed49e6863a6c23d642bdb68657c47126b8c024d757951cf0557970bb0f0507c5f072de0f186d1b89ad6afda80b47ccb1f6e6ef8" },
                { "af", "5560d786162ff48b2b7ee11d48497c4c7ba2dc4c6c611b39c90f9fef7d8463949253347491cc96f120d26fd077aa5ea57ad9aa5acb91075b5e840c278630658c" },
                { "an", "c71b23790b754dae4b1ba5a45b9b0bfc918a1ed96b0ba1c71e86c60078461bbf4ec31cef565d17291d4f3d98598323c89dea769e3206106eeaafde3ca1642807" },
                { "ar", "c1ee0dc9c2f6c562fc51f5cafd03d3c56e4d45b7da0a35fc05baad5e160a8533d32a26e2c4ce5cf5d6d7db914163086aa00cc4f981693ee9767d165150ca08f3" },
                { "ast", "cbb8cc135b9b8599df950c0b7111e40166eb63c99c953e744bf5ce262eaf0d77d1062f1f170958f1ba807b922af89586a8ea16bc30923298b222837f3d031768" },
                { "az", "0bf15832045654f33f92cf7a0ca041079dc2a85594029b73cbf6ec1949a9e64c66d622efbf7287a331a0ce9219cbe84c941b0d5cdf2290729ab7ff7f92fc1368" },
                { "be", "efada0d6a93ada62fdb02281e609806a8dea74b7b2eca8e2438f7a563e246a4ca2c2fd637a5041e63912301d1c0c7d4dae5b85e0e86f7d87fa09fa44de224517" },
                { "bg", "d75d2f4a3753463e8d582834c72dffa936508685f6ec32cc8095f39de4db0acce640c6ba686835573ca9d9e45179e9da334a5418dd760806f8129a82e4a71d6d" },
                { "bn", "b0f003c6776b6701a785edee7fd826aee03574ba33f3369db96249d2e712743b896a9e7ef322774251f8d80bc34e243d4a2142699e8d4dcb65b8f08ef7783551" },
                { "br", "628d8a1986f5c5b6e035b83ef69549f31f6496ef6e9477c9d7e0d1bb730e8936cc488f68e7e0673f80ab1b7f641ac56af8ef3cafbb698dd4387fe58121c66051" },
                { "bs", "bd2e05a9fded1cd2917a8cdc7bf7fe8b641af44f5903d2401df8f60038a918a67dd35563216cb39f37284139a9678138235fb64b76b7dc2476b96efcd6f97db8" },
                { "ca", "5cb3ed28ff2e1031474525e95783c0489b478845717fed17740e1fd8a3095f8dc4e9e879515160c45e456236e717dbf973cc6769b164e2ccc3ba7001ee2a215a" },
                { "cak", "6044590c2a41e54b76274d72a93c7442984275036ef0a9b67ecf24945cc27b643c969b01e3fcdbf5f9073363c7d61a43e7527d1e0cc6f2f0864d0350e3ca305f" },
                { "cs", "0e9b836b074db44b0c3cd6650f988d73a8464778da0c8e35bfd147d89b6ed6f6b317928306c3e1c8829f2c359e5881bfadcfb69c8ad08a6489f646464d5492a0" },
                { "cy", "9ff8e4233b4dd31f07c1e549e7516038d8bf97c58968a0add2c550d3c92407d009408f0c8448d647bc67d1f9566db8fbe6f6c38d6b21bf824c505d88e36abdd3" },
                { "da", "cd675de6976b87b8d38c82ec859e10a8e61ada247de8e7c614acc708bb79afecf19690d4f1ca9328dcabfe010a3f8dd31180bf18458ec51ae276f4db0a51dc38" },
                { "de", "16c1dbb4ff5bd5e59afaf3f62c4797297397efcb2d36ed531f899a48cb0a150ba1c9e085f7ebaedbf06cd2c6caaece17157e9ef5086c39899b999cfdfe340d0b" },
                { "dsb", "30ac8ddf94349f3a9633520ba4dcbe35a5e8423c220206a40a79ebdd21deed3834e3045ed5e1f2afccf9ebe4cd932f3dced70b9896a613a7c79cb00489446124" },
                { "el", "5fe9d3f4c6c7ab8101e9b80310744c9821040f6e75da66ec6291c08e019d6be78343884b7dd27d552bcd68f9d0ab775498009f28f31363f5af32a1e4e86843f1" },
                { "en-CA", "4b34496c2227b12eea1f15541d232fddb8efefe5673672425b386bda32cfae0de6a4f01618124d787a5740287240b7463aee41691d2f58715d44d1a4730b8533" },
                { "en-GB", "d9142b62f7cf3b9016e43815bef32be6260dff3d5a23b9335aab4fcbf8bfb589eda117afc810bbfd3294ae5a3e94cca42d09d119b92c34b8c26101a0d6b4fdb3" },
                { "en-US", "4dd57262473c37cb87b8c65860226d8d7f3e9c3262a0833d4fb2c6fa80e1e8ee091caba4cc0dc7e862afca984626a40710e23cafb82a035d8cbd0eb7eda55b05" },
                { "eo", "e874c1ee795a51e0b2e9a473ff6046a9c90acbda6297812d51c214d72a1e1a11d78adaa6b3f7163de93878730de82da5720c3827457ab03f758d3bf1a1117567" },
                { "es-AR", "58e96c62eab4a64fd3fb63e8e45175d3c04f297c33653909a85fe85308a857201b55b5aeb645e7d03fafbb1661b9efb4ab662f89799880bad17d3edda57232e8" },
                { "es-CL", "05d257b645f437ca9474cabfcc779922db6b0c3d6feab4d8838b476b9185b1e25e8595ffb935bdec63809469eef19ba11a920219babe363df0ad0d6fd2acd001" },
                { "es-ES", "b5ae281162bfdf1f28d469129152407a153e235613a43a90dc2393ca587186cb590a3511f040c1045c4b08234bb0624e91c2b9d85a49620c88871d1c21a43f44" },
                { "es-MX", "8b9b388babe2f55723ddc252f54f9d902650dc1e8a1cc65f83ae3fb9f6a2a293c4e254ff3e8433a28d98664d906b7c9ddea451e18ef1099eb78294e002e142f6" },
                { "et", "caae838fd10d2673c3cd5664e86ace741599a8b8fe337dcbfe23b9b676ab482d3ca05c4a012f0b5776f4e335748ef9af5220a92ba52741948f32b8cba883bf90" },
                { "eu", "e43be1e2f403efaa871dcf119819debc651f13807d534ef125fea5309913bad393eccc167f92c0328e05006f1ae385dc875aa898eae0cd22394457ffabd34e7b" },
                { "fa", "591362b48b99298ca170ce6ce2727917718a2d78e9f6257ef995f5d868b4d3a6b21170e66f500a982fa3091fc8c34f8efa5c5deff2dcea1aec76a875c03f7773" },
                { "ff", "b5036546a955499d30ca60cecc91695aca5b27f6c7891c82ae0ad5e14d4efd57a4b116e5c221df86eab2d51a953d1cc5a8a9048e8a1ca31017557eae9862c86c" },
                { "fi", "6864ec4661b6d5943fccbf319484785ce50126ace286dc3427d707e97cc88705b0d6bd988e38dceca66aaa2be72ef0e6684aeffee64ba9f25bb45181153a41a8" },
                { "fr", "a3603583bc992fbf6229fc2783b951fdb6d21d4388cbce7eff2ae2ab80036ae430f1f365845b815ea89d9872313f9e3d85229e928df5efebf9fc1a8968c8081b" },
                { "fur", "c0cf5fe567903a5c65e27bfa506ff403c5c82a2a270d486d19da030b088d25691f5751c41877b845cb02c1d8ddd67797b234a1d625f253d020170d8141c97080" },
                { "fy-NL", "f63bba76b16b615321631e06d40de7f6836e5de1eb84a3e2162089a5553289f42b22bf611a50beb2e8461f055eb129e1be5ffadd93bfdbaab03b884cbb4dd446" },
                { "ga-IE", "e8e01b90ab70f5d6166980c2642c31b04151ffdb6f136ef58e439193453dcd304c6724ef83f278b2d66027b55a39676c68576cda579b6a2dee8d55006c6ecdc7" },
                { "gd", "e7b6e9b23b4a6fab595bc19b2105f3366418e275cbffe7cac11c781487855034eead86896ede72cdd8f6aa26b83694405c098b33f9b7a42da20713bd2dfda030" },
                { "gl", "d482a6928a868b83f8a3cacfb0e0da712d4e3d9db1a5cf9067e6035c2eb836eace9ab14a53c4ba5d1f943243e0bae67069162d080a9072245109008fb00d16c6" },
                { "gn", "2d947a5a9cf79f35d81530735f95d6b0ea538e3cba9ebfee5f9f5235f4c3819c975c0fb52db6245d7d9dc82f353a6185ea1da61aaabf9cb4f7415093ff54fa6e" },
                { "gu-IN", "f5d8e8aafc6108e9d92d8b3e16825a24e5490ddd9363d301810d9688eff7b5b0d5679ec20ca078e6f034c08c4f94a3e3835d8ebaedf2148d4148a5d232b12b1c" },
                { "he", "b1441837ab7e86a7f48671ad356523f20563892837f06b561e07b2aeeee23a794bde44b193308978ab5f13e9080b829d63d9b16d2b947c283a93c238e3775e66" },
                { "hi-IN", "a54cabe547cd31edd9df0c8b72d36b0044b9c330164d969e6218dc49842ef69288c58ee620ca59b7526f2958257ada0695378e67df8888e2b956c3f33d506218" },
                { "hr", "b219ff934a8951c1b2c47a01b8d2dd9e9b91fac83d828e679f6174f2fe272b2eb8f97b07cdc89df74d682274540f5b87b9b1beebbe2d0f1d2eb2b05465c893a0" },
                { "hsb", "0fd0395d615626f9871d068821463400a82c812a3edcc035fe91f9c1f27e1ef241372f7dfd14a9bc3ef308ce742891337c6e7602dbb482956cf2a3aad0512d48" },
                { "hu", "8a7749feda6c3f3c826292b42787a3650350f68858317428f0ca9ee7d89f40976f1ef4a41be2a0823330b85f4d21f6e5e2eecada84f9f5925b1b784e5c15e183" },
                { "hy-AM", "2347299c3d872241fd63973558ea1293fe8cef8b13f10ed3e8dcf16e897e798fb18eeed65e636db371feb0089154ffd4a852e085a1fc9dcb96674797d216f4a3" },
                { "ia", "bc08825628cd4447d77670cf8adc4353da171ac1e6108f3102ee20b03c5f90e65c7fb9924608df4066e12cba11098dbbea9ce61205db221548f6178714a13526" },
                { "id", "52cd83779d7cf67a5a05e8abe29e4fcd5bdd717bf35f98ca60d3ba4953bafe801b14493eda619e566265771bc191e1f1b2dcd783c004eef6a8b9cc2443853087" },
                { "is", "972abd1c04ae2e1be3c0b64a56a2ac8ab912fc7e23ec72773c1592376e97ec2d4e0cb65233e1faa3b4098315f410e6033fdf82702a7521e911e59a03bb506d4e" },
                { "it", "0d75f70bb03ef398c15cc02c86b28d0bdf7d39f383de5bdc74d576deb5a0adc751d61601379331109e95e71389c3513199e8121729453460c5cdc7c7c19c4be8" },
                { "ja", "200ff17aced4c92f0205f97e3c94dbf50330f3fcebc98a290817c430b30fcd1f574dbb2addedf2792968cbc20e6a9a8ed03537d92e7559ca0fd707376ead3e7e" },
                { "ka", "691a45d763b393647c5e40446240564c5951eb24e14c09571368c52a78c786082475a00210c0dbd1be0a473f01b567aefda5baf8619c8a289a3193e23e643d4e" },
                { "kab", "33790f568c0db8d627903558289245ba233d7925435e6e6da518d5d48323d32b044d7be45f8e9e8f9db9394d765494ebd8e20a6f1e7e4fd474bf0ec5fe268421" },
                { "kk", "43b9c7f5fe68971ec0d5c90775cb303be299dcf546246be61fa255d61ceadf5bd0ec1eac1a57c1c1ee2f5fe344ed754bfefb7ca293c3206e9b1b25c7599f43ad" },
                { "km", "79862bd8d50c14834b6bd6c8860505744aaaf6e1a4c8076d2624367a0c8321f722c47e297a9020c9b73f6017bae9f9f8359d54237e196a82813248a540b45b98" },
                { "kn", "025f7faed3fb25df4879230e40b8322f0a5a3fb29e47e4d8f6cdcdf7630b4864c97e840847e486523ebbf536674e219bf7f4e07197ec2d0f2bb09675f34a6c4c" },
                { "ko", "8c2d48b9b8082238fb3df1358239cb84125cda1c7a0ba89f1f20d721a132abc5a8c37f9adc15e5a79be8ca45d78368dce858fb432ed76d2efb4578b7a26ab05b" },
                { "lij", "010856780ae995aa82ba1679ee15ccf728e4cc626e4073005d106993047a79f3b86e2b2a54486d8313e470b78ce0892b25218936f7cf3921fe2cf4a72e161911" },
                { "lt", "9ed3f2d9a1b07193868bd5e8326f3530c3580cd8392077092d6f4b35b595b8c9605b99129b7094f730e5602163602582f5056ee6ba9639051d2f3778be2fe66b" },
                { "lv", "be3d5a605a6ebb7faecab94221699163cfd6c4b2c949c61cdecaa10708add55bd3347e28e7aaa413bfbe33e76dd3efec466756bcccbac0f909d75ac298cebf6f" },
                { "mk", "b629301dc21ea0870089674ad54dd991ad33590981f1a17341fdeb45c9a057faae25bc59ea6d00a5ad6d5868777eee4b328f6dca532bfa0e0252ee355c58079f" },
                { "mr", "72ff846168cd505aef4506090be65cafa67a19eaeed787eae53d76398adf5c4d27e75369ff83cb7f2e7dafc070d5ca0c1e8febd688f32a48fa6abcf8c2a21d8d" },
                { "ms", "59549dbd115cea721cda3272b008e4f1d78ab209191320efd2a104f19f02f206cff6eef2bc5ae649cede5bec3eac853c23f8ee52db2d901973f79cabbdd0c060" },
                { "my", "55f05b255db688f5e5e9e4fba25d24568104370c5896956d30c2e662c95b1e73b8e4c285a2d28e1b9024a50b33eef304130140f4394125c449be6a48b0414394" },
                { "nb-NO", "7f97813dae55e6b18f4324ca3da3707c9c75505847c05f3511993730395fb7c8df05c77b450839b905bdd7cefa02f906d428cc399cb89fb3b48638b00b94c5a4" },
                { "ne-NP", "8b2f35922ae9c1ef06b58ffdb6885021b85b9f6f879dc0d844f218f9a804b15292e07a77476a1c66cd6a5a23dffc3b30975e87681f4ea922032b433cad21b495" },
                { "nl", "e0da852be262b9db3b27f4c0fab6d152260bfeb9148d5c107175fb4675c76d3efcd2839baf3eaab326b675a6e3a062b4945ff540760de1e1549e56599ca5d423" },
                { "nn-NO", "5b28442448e09f0acf652207fd1708f368805bf7bc488c052cc049031d43437c56a9285656939fca1f5d095b4bd6c7b75d83a8855b3069ea970099dfa6211eff" },
                { "oc", "936a3d8a325a0e5db80a97b25819526f33db32d1b29a94f50ff5db39967e9fbdebb395244b5ee5a0b9b65d6f9ff134d006a3baf48eb5206d6af360dc9f192eef" },
                { "pa-IN", "236f4bb9f662e642564c1894d149fa3473000bf45d8f657693ee34245aae4967e0f5979cd7c2f2b0bdd001e0dda888cea6aa4b18e89f92775a94e253947c6006" },
                { "pl", "3b475a64ebf88bd3a630a99d1479062b8680c5fad82760dd6ebbfade7a1184014eca306efc6cec14b2fa282ee73ded393fc0211dbf83957007fdcbd17bf8ccde" },
                { "pt-BR", "a91b133de4e9ef89c38be51826e84f2b3c86609f1cbf1e6be97c1a957c226ff4a47a25790e7f836d4929e337fbbd111f661616f1dee5cdee7a0c0e30e7d4d48d" },
                { "pt-PT", "2eea8d077b0e141d30ef512f410a30bd8309597cf1e566983fed4917554e96062e9f7b4e5b5035a3fad39dcef8c199477e290d727f786e349eb6f57a18c7aec3" },
                { "rm", "6a1bd456a80897797b1bd2a37141c687e460df9d0fce2b4991991003ed879c9d7956885c8ada807f4462f4ec8f95aec902986652da20be14e3c24d867aae7cca" },
                { "ro", "fcaa770fac0474654ce5853e2dbf4e45ad40100c07ee8c314369e9c19455a93f2301f8cdbaed346e982e59d7b66199f0cfe53ab2225407062fd9085cee37e17a" },
                { "ru", "ab87582d0dfa26045388723180f7bf8d4fe07f58b3973997bf92507dc4db00345acafa48a015939ba6faa65910c00f63a05fd9ac606d6020599b3a16eb76e4ff" },
                { "sc", "18b3c8cd7cdeb506a39ad9e7517bab435271eb05972063d82ac0e31ac091b4f16886829b29365e53ecf91a0936df0c765e9ec02e53cc2331d5cc82d40e21826b" },
                { "sco", "4981ee00d87ed9c849bc6c2b905c6aedc08c881717852fee8f25484265996c4d5797f7194be3615d9483c972dcc97bf9a4b8d958684332d221db942685c4198d" },
                { "si", "02dd4221ca8ffcfa7f81c08c02601f8b57c8265fcac5903879a76f5003cf739e756789a56dfd5db679632d6f5b3df5ee09d6f623db106e593afe35009a8e1848" },
                { "sk", "80df14491ac689bbf70702a4a0eb0ac77704cc8f40d17db76815f77c835f5731dc47ca69654dc3be499bb3f9f7f2b866b1ff0641ac4a923f051bae27f62bd435" },
                { "sl", "26fb9dcca305187f9c5c8df4adbd284dd531924af5272341bbf96149503a94609b0f019fd11c3f9131e39679d98ba5871cda5567cc3350741424c838b1fdf14c" },
                { "son", "202635cf34a8724bf9910b219ae070f9312affe757f33034b951e53d04cd3ea326a53248a18be37f0aef3b91acafa8a1386fb1f018b639e55f0e1df5af62809c" },
                { "sq", "0776ad84c5c5602d022cb11a0a0dda7662a26a9c6441fe65de524529b57d250319e43dad982e4d6b8d249d3a884f033c46fe688cd204d20b6dfeeee7c2635c49" },
                { "sr", "9c399c2579338d48a9371c30f0c0aa3908bca3b4d4ecbbfa8bdb21ae3b3288a1561d8428ef05aa3effbbc78662db5782ae646c2e9a6aa543d137ac9e93855121" },
                { "sv-SE", "57de20dea68846e9dfba1b94cedff9e5ad8e429519c621dc50043c2c6a16a74558653c05d4800bce4553438602811b765fea00db523d0b818bd531ad54ecff12" },
                { "szl", "f8568ea80ade6087a31e935b1c2726cf23ac18fa2c53d9a9d5a81c5afd8c70acb32afbf7b5813332447945e6dd8f045b49c15e6c590c2092045840a10e6f1915" },
                { "ta", "bbe392aa28c503288ccc3e47f1b6189306267ff0a8cd348d352f896c21f7cc5fff7528f0b4b8103ee5f7ba7d683e54f72df0ec5bd55fc45a539dec6249becb7b" },
                { "te", "f3ba5fb58e1cba5c705293d92340c124f27138fe3bd5213676222d75284ced93ea11e930b62e182b7f17d7db07f33dcdcd11f902b8b10b66ea20b19bd940def0" },
                { "tg", "386500d3aa8dc6510098d5fbb3082adeb83121a4669c0fe01a3544b865aaa93f0051e0c1415925c91b0d8ffa3a167749f0190b7d4560a09571814fa18bd78390" },
                { "th", "16f42792bddd24951a9619d7e6e70dd935b45d50ff01a74250db60411163d2204c14c0bd3e4366779cf40b3ee736dd85f12f991a747de897c670b9ad3dab3f1c" },
                { "tl", "9f2e12cfd670600f283b6a6e3a41d6fbbb445b3c8c9b53c4713122e07d09c6e9d8a91d62d08ac7e77ab9149df8b7ed5a1c972c2a8ee12b8371c648e64b5ca564" },
                { "tr", "51d5b7f5f45797e89915a1d534bdf9ff13bca20c00adc208115199ac707c361c5a78143691fbce23623364711f2267ec37972a05a321f9eef3c2ce58a6b5fb1b" },
                { "trs", "fe9fbc4eb866306554276009471bfe9ffa34f473d56584e123d99fea84815f26c8588664f3e371d9ec90faf560b0fd008f22e887b0d7ac7ca79212b55777a323" },
                { "uk", "5e176c1c7c8fd4167e4f8ad13befc2b8c5f0d210470bc70f57a2ba40c3a815fb0bc63dc821bc41a2d9e2a506bffd894855130b22a091a8ec725e0b8378283a69" },
                { "ur", "78377990a29645c6281d4970d5b79c3a7b1477472e90205ff48919fb8f84781de315994b6c414ceec979a15d037e28cc5e2e0be702d0044f5d69a928bd143c1c" },
                { "uz", "7f7b310dcb88dc2d2b952221be587bc32760e16564754f019e6bb78408dd3c74fde34475a5c19abcad795a3dfc4bfa227301d9822156f693288e94a0859686b1" },
                { "vi", "cd44ae5a6ec23ed9aa7a8c09928ccce15b39495baee39733d147cf31048c4b482bdb439776f28fd30495263feb1e7cf6facd1c47ebd3f7c2d8f46a5e1af848da" },
                { "xh", "38e2da423995b26792b437acdf114c2430e3cbcfc16d35bf029392fb41c2de2a776e38c00d401e1bbcd6622e497a41de58fc1625f3529e0caabd7c7e061069e4" },
                { "zh-CN", "a2bb47279499a0d7bd16c54b37745a7bffd85e127a800020ca2f7d90eb87fc3ffbc4a4f4983d0f8bf840830e36545becee810dc6ce8a226e170a18417a339ada" },
                { "zh-TW", "10cc1f4cf271ee653858562593f59cc15931487f2a8d6991b0413eb0222003b6cc6184b5b7ad927200e636da7a2205ff3cd05934d4d3c15a8d555b2b83bcfd5e" }
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
