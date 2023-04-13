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
            // https://ftp.mozilla.org/pub/firefox/releases/112.0/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "3f458707c27a246d54c6c6a3f6f893ee7add26ebbba002957f81a6eb8f4fd2adee82375c73e1b56cf95b9ab5f1f3667569a2c81f7fb562e24dfb78f606ca54ef" },
                { "af", "ffc3c268f6091fd9f4664266814bfa2b7689a5b21ec1cec214aaafe28e646a86e7cc257c7cf44ebf99e48f6b0725d2a59e311dd10ace26f784a96e21923460de" },
                { "an", "34b5144e15b66e606cc1438b42f05885b58bf71cdf197797002bbcf2089bbcc00b1c54bb742e51bca939d87578c6bcc633afcec80817f26b2c665d81079f45b7" },
                { "ar", "be598e8b2d3362fdfc73721a10bb7c24faad45aa75ca1c371795165880558894dfb3aad5aa64aa7b1b5be13e7939661a15bc863903e5531e4bd4991db1b6bdd8" },
                { "ast", "75a33cfd9b4c0d1a53198cbd557784390edbd27bb1e2792c8e11b41d08b58149080b13a1e315a471f94fc1731d7b31e09a53f0cc0b71e97bb8773939b6595a71" },
                { "az", "2925441f89cce434fece8d9e87363dfa302dc5fa11221dbdd285039a32bc6aaca17c2849c87f8980a2a6e300802c242e23d8ab53368159cbf7b86d602872ea51" },
                { "be", "f3b97a2382ca89bc3008a0b420eb79781549221334ec6130bfe5da429fcf2c6f54d390b0471866fe0291282e307e9c840cfd1c35297aef34f2b41c0bba917d78" },
                { "bg", "83f80bc0cfec591de62c4716d4bcead19b6c652258731f44006bcc2a421d60fb7ed787e7c710a6756969f1b47c9f708b9645f5dd0454940a2d306a6007845158" },
                { "bn", "97a9443c0174d61ae1b4c2f1c95f2cdcbfb0fc08790530d7acd4ac2dd91ea99ef080b3ee73d3c40550ecd624023101241c88a46d5d588b8bb398916f3db523af" },
                { "br", "f8e3b371e4d5587b7ffca163cecb0e7bd6b117dd261589f41a71ca26fe62edce4df781adef68ae8aef035e3c2ce69d1e3b09f3cfda688519d4a67efd34eff335" },
                { "bs", "29ed69603db0a834ad91a284256a5d7adbfd0eafc64ed41c04e7aa6cc40e8c3cbe5ba2c13b925a58a19aed09523d8d8c127f2bc0569aae56dd1ecb286b64cd9f" },
                { "ca", "73bf273e5cad0c935f7092ef56f2ef2ceceab6ce422fbef86024e19f0e8a4af94430488378e21e037e56cd44c1fd7e115ce96325e00bf21e9207093eea980263" },
                { "cak", "4eb2ea61ce585cd505266791629cea83ea4a9bcb6b9cc539a6d1c454fe5df7a6c8fee3b30ccc7ca7a4050af8a51fb572f5a0ff7e491cf6fa33ef2ce7f787dafe" },
                { "cs", "9d905dc91f87fe7ec56c35bb1f2e6fbe82b159da0750a0b88bf40b2284031cb7859e6ebc644a9a81d3c140d8e56c90a02ff71465d1a0dc7a989eb48c3d3a49d5" },
                { "cy", "5eb95c9e0a698734a891d25892326b2747cdf0be958be1a8ec1031cc3cfc4a7bc5e56d9b8f0671495d205162e4d65a0599f80b577f6162db3393133336fb73cc" },
                { "da", "1acf52aa19d193a0bdf2511d0b1e5d6da34e47d18a313f89a45399a763870e80106c229e64102c5ace2a63746f2959ef0d0aba24829f801386edc6e5e06cef71" },
                { "de", "6b8d2e657273ea6b1ff3822d98bc3c8e06e5206d8c585873945189af902cb8361044d7834295657c43a7b3d43df5a8c194eb1e0eedd80215370f01ec13f5a034" },
                { "dsb", "fed8c0a2bb7979b01ef36e2d263d9b9fcfe7a2a95232e2552425d6a93a24e5c6ad5196b187f6d0d9bff905b66515eccb0734ff9b9c4bebc1bbf0cde8fa6d77c2" },
                { "el", "435dd45c8acafda70fb116557e242cd822875782694b3de407b28846e929c9a25ac449d39ce8238135052609368f5ea1e32b7c27fcccb294104692fdba69b4bf" },
                { "en-CA", "8f8accb20965ee8f8a1587d15c86256572aaffe2325db58fcfe3745034a6fc48bee9c6e2b7d9eab274f1f6452237e3e300fa34c0d7fd1124648a98cfd46d61ea" },
                { "en-GB", "df09d3ba82a719448bb2554aa45486e4b881d057112dc54107d21d048e25d92f54b7dce0e36dcbc81bc8b5548e3201be1db728ed868575338cbb6ef179f3d44a" },
                { "en-US", "a443cde5510aec9f24a0eef0dfc947e0cb8b2e64d42dce6c412f6fbbe11de0d7dd6ab594daa0989272c4d33c0d63282d10cdc25ab71bdb5ab7506a880c0cf202" },
                { "eo", "8d36254eb53c86f6e895215fa8d48d7c6a8a573567e7d40cf2643aa41fa32d0d3a5a6b5d378f20b3c15749574ce268677ae793d264df469f15e9c8eb228909c8" },
                { "es-AR", "1db41561483362607d43f372971c500074a04010d3b3610e115930abf7fec5f6d56d364b3fb38379bae79c3590e6579c30a3f48798d78890e686b03376bf4239" },
                { "es-CL", "321c85407cb9d7791b5006b99cd387570ff24f9c13e66fc36b1c9649b09bf58adb848e4679b5cf84f00872635940939c71b35c76b0effd7fd2fc755d84ede249" },
                { "es-ES", "9570e68098279fd3de23357e3080fa0e5de59d2b3dd8a0c74716a9bb4b2add6c16758260405abacd4623fa06fddd95fbd9b468fa6dea3b4f0e3aa9a71bedff63" },
                { "es-MX", "c05e29cac228333a3850ded578c56860361c9e2a53384a9fcf9aa45be0cb3b5d33329ddb26472bde4623852b0c500030f0e4496695524a1cac557a872668a9a0" },
                { "et", "e7abfebd7bccbdb01a224bd020c963072d9e6366c6be36bd7c1af5f9dcb6ca56085418dac605cd96f1d36560d8c0972bfcc67c34b3449f838681f17657f8ac9c" },
                { "eu", "5c379f4b46c659cc32da5bae8844ad9b62916c733eb1436eca55ff76ba94047cf0f18cbc7c19c8ab355b3f32a0510b9e8dd84112b01df8dec0cfe790ab1eda35" },
                { "fa", "ff4738d3ad9b4d2452e0d43319fda8e7031a530b46b0b62690cde1572da7f5607e4bf3a7036cf22ac2799f10d923dea468a1e4fb0f1b7535264f83f897fff5a4" },
                { "ff", "9c4a2bf4cce913bc9f79befe07d9f0236043ccb8472cc94561c2f8ae8b46957ea7b7c680006399bfa54f28b8d8ddae54bafae9c179720219358fe9f6db600a2c" },
                { "fi", "ca124d123333c9e74bdabb9ba9e61c7fcc595613f8057215a322adfe27cbe7e4ce8741b231d77e2b92d69825ff7b0bac57aee1b8c598bf20992c9848817e3971" },
                { "fr", "78b5912b330f3ea5f3111406a7bad968359808b48ed47c8e0a0170636a5baef40ae93f3e32120e49c97f0c23b6e8e733662e8823f7ae174825130fe11c8ddc13" },
                { "fur", "37d025f1dc210b093915be66eb8dfc86b301552f84477984bb6eed14e4dd7382af01519e889d0014a29ffb0d69b055111cbff27c0b28add0b183fc5b6f062ae8" },
                { "fy-NL", "806cd78b4fb1649b6e239f66df2aa7c9514e570cb64883e6ffee018caaf0ed80f868662b27119ac843d8865ced36e4d0929478b06da7e64db9da4e784bdebe9c" },
                { "ga-IE", "2b53e7793a6a069203ae4870a720ce218b95896b640e0c933a5409c5683b0501e12c199e4d7bf88611f934d7e8676a1cf8ed030149a6f0c91017617c34cb5f2b" },
                { "gd", "73fa8b47e2c2a850a021877275f05226dfa5259261ff303c7ee3093034df96fbd375d26ba1347657d74742e8f3c141a86ca5e6d1647aa314a4b01412c6a6f50f" },
                { "gl", "bdc44aa8e229da69182c6d6e8245d87fd2564b0350c6732152533f62a458bb89124b95b9cce5524f314fd523036b0ca4f4d7d7e22e7ee4eadfc69465e7bf14ac" },
                { "gn", "dd19b2177871daa7fb6d2efdb2e9ceaf20589d29e0b7375791c6adee2eee604083256f96fc71e80010f634dbf0a34131ff33861c926a76034126f819a365feca" },
                { "gu-IN", "c2b8db4d10c89a525101248d3bc60146eed06de26450cf7098ac038411f2dca8e9032d7e1bd9435574f89b1802ac5fb26c3bba28e4565bac2579b84d32acaf81" },
                { "he", "79e2783306a13c43f9869cb3578cf0297e94481b14150843c7ee029d5ec566bc668660e847be30a9fd3ed353e5472c3bc43587790970798ab91fc2fe34b9be2f" },
                { "hi-IN", "49c7aacde8293883f757014825bcca200702addef4b61fdbc9acd1d40616fcdb2995cc0622326ead3b7929a70cea852e0d9e8fe48bb1a4f41fd0f929eea735b5" },
                { "hr", "94cfb9872d721d51f674549a2e120041f144eab062ea1d48377363546d1c89e58b74ccdae01e9c16cdfa768673493a65b63c299f0eb8594143322efc43cc7644" },
                { "hsb", "95060457e697b9f59b2311cd9c57124fde14800a810872547d7466ffa637c3b9957ea3b96d6be1aef6f725ec8fbbae75ecdbebadd2ed9279a4025b7193e6fcbc" },
                { "hu", "4b281555753dee78a66e498039a847d2a5533e4518060e436b25ece2bce02c0f09382fccb9c4202ba80e3559b42af1ac98e004db579203a3a47dca3d07a6b940" },
                { "hy-AM", "c6feab2c81a4b326c48f0fd5667c3716d7c9eca3bec0139430897d85e60d9f4879133375f6bc8c60e4b78689fa6aa253461cd1f17bdc7a72a9d262bf0ea1dde6" },
                { "ia", "bb7a6184fee243b36cbebdd151070fb68100f9e8e62373b39c9c69e62dd0ebf0ccabf6ae3bf66c8a65bfec27573d8e859ef68d932e4531bcd594a1faf96340e2" },
                { "id", "4719c185fa721bc685f1da0d9f1183a444831852f35472a5593708c3f6e135e9d7ff83a4ce9fd123000dd3972df430c3f08e4d3c3d6915b083adfb6e43796428" },
                { "is", "0adae7dec7b0bf797fa6a78c4ee1e885f6c51ba42523ad6be25bb39f8febdddf91fb9f96e83be87dde0e399435f71025ba4233cdd433891e94c19a26c34a0909" },
                { "it", "a2de10e93b4c9a6c729b5748cde8f3ca5dd4ad4ecf56b23a19cf183635fa33ad28eabd654385a870e945f760e5c86e1160320433b827f76c83d7edc67c733b5f" },
                { "ja", "53e778187ee1171080b5b79aea77371995e852747ebf0b4198dad8ea8cae731c0cde85bca1a98dabf37428ea8634f9c7d06786c6c71ae3d5c49c20031df94a76" },
                { "ka", "1c09a856330129faf1f399b8077aba30c78f9a47e2f31d0f8c38cd0b409fe4d370c87990c01917effc858b6f35bc6d2b2dd7e7091fc6c706ca8e043c8666565d" },
                { "kab", "2ad98be76b5b217e8aeef3d39b2d3b6cd4fcf94d06729bd3a7fee9db6ee8a8d2b8339007d8326e2f8df5c120279f5e66844d46ec4e02fbc367985ca8dabbd140" },
                { "kk", "d4e9934544bb2f6b882294adf8e1f8522ff3197d9265063292bef7b25aa688fb1404426013a87a2f048e4df0031b0aea7e242bc890f2035d90f63dea725e2d6e" },
                { "km", "7118988380b4b1b26cb77c4e6bf60544d127be4ce6be2cf5ebbdd0b39d885cfc6470bac5f621b6850cb2e65c15acb38cafb8414440fa878d7b17acc1de06f8d3" },
                { "kn", "344b96122d3644df2524f35ea024da2b41c84c1091ad62b36951f8d25a52b5ac278c5270d40a7456890b0c5d552fca3b78af828d642ea066ee3792b1a915a65b" },
                { "ko", "c646c42e0fee77798d11a36a808fd25506d30d81e6cbcd75b575ab7265d413b5b440b791610ae6cf99d79ac25983f37206c43a000f716358b965a677e93a59fa" },
                { "lij", "30400a5361f9fa8b33e213104c0e15f89e69c82e353c209ce5f4a7663e95a86ad58b1d2805fb6408b36cfedf399e477614927d9873d166f1846858c6e16d560c" },
                { "lt", "8cc83785c7884cc8f3342fc45a108c13f133c8cd009d2bac134b5a53afc8f2345d8cf217ccf81b3923e0d91cfdd43cfbe5db2cc343c798d1e9b98d3c7c62ae7b" },
                { "lv", "2a7d7085d19e76e9158bbf0e828af01717807393f9d0425d3bf28b8b1426397d6fc5c7ab83bd1a91b6cf509895f5d0d63620ddbd981d2ccb49c5b24ee073b1c9" },
                { "mk", "4ef7257aae61b2fa9923cd266dc93624d45251eea650728740c4364e8cdb0576667e7b31f995af11273a07df31f9d96f7ea5ca1e41c2602e492c30770aeb75e5" },
                { "mr", "c135a11967c87bc6655ff44ceba43ee3b449234345c404bdd7cd8ec3eb378786a8ea0a52f14f34fe388c26d11298b21de6d15f5c8e97a5b6043a59bf98caeb24" },
                { "ms", "1f2985204d247a773a7aeb1c3d9bd13422c2ef570045d1da95c3da69e698bba42b785c4b3ad1c56c3609792f77e6ec243e215ac56d29c126b334b50c019f5207" },
                { "my", "e599fe8661034a0026653ce8e7fb90e0fc5486c441880a2d4ffe6249c48892a36ace6917eb97785dcaada055f5a14d664866657dc7499a1335f934c41efe736b" },
                { "nb-NO", "d83e262a7bdf64aeee5359285025df3c1eb3c0bf33213b4fc2ef8eeba4d2f66dfe91a8ba37d03bbcbda193174bb867f7f3e6f32314d63c981b82815ec65db757" },
                { "ne-NP", "2d82ed0d4c487c12bb0bfb3c5bc7f13a797bda350a46a3904a43b52e4390c751fe2628ef55705d07931fc6a722d0441da1f7137c4f9dfe42c84263436985d582" },
                { "nl", "e6a859690b439058ecff4705015995f1067dbba1d5abeff591452a1176d7e94310b13ed709ee87622565f71f005c0680a52e5312b1445b65f8c95be3a04be122" },
                { "nn-NO", "1c390f93f5efa48742ec13748a6f020526a34395f04f90e7f294d2f6bfa2dd5207d951f0fde110ef1ffd53d86c2295f1035bcaf475c1d104f8389897d647c0c9" },
                { "oc", "4d69dd0029139cc9eb0e06ac8985d7ca09c2c97524210941693c251ebdacc22f55878f2b1ff58c3ab1d09923292b4987986b726211b2a5adc10d211b4ec78c30" },
                { "pa-IN", "de21d566b72d8579db46a8cb12ec0152fb802bba6bb6283c8965143711b63945ec67d40ec5fdb95a6637344a1d3f53e42fdff0359b16f4ff7f216987d1c20977" },
                { "pl", "33c60980bf78320e9c71b7400bf998b1b9f8baaa24f665d5248617a11603871fc83ac05d4e9926c6b4f83222b6f20ef554b6ebf35cd820661a511b85414779a1" },
                { "pt-BR", "303fa87a778b859bc410de12e3ffc7ddd77f92817ee73c4e8407141945e642c8e4ec52e9a95116e71b612038831330ac47bcc023218e45882d2c2c442654e87a" },
                { "pt-PT", "444b98ebf43411f05cbd7caf27eabbfdf7a9d7be64d4e1de073af26bc059a7ea054e7c08095b6ee1127a54d262f18173cdec90b44b4c10886e62e0e155db3de6" },
                { "rm", "92775a95f0abd539d1bcef7a4b2eee49c01d825f2a859e55cb74b09801033162b8b2829c4d7ff0e62227dbdd813a0014d5b037ba76352d52bbe4d9a9e8389c32" },
                { "ro", "df1891f9bac862ad9cfc02eccd78b047dc8c1459a9c9febcc717e36ae0fafcbf3c66e97e80f115334feb142e3121e2b2c47ef126e36e9716d858ca82019600f2" },
                { "ru", "0685a90693e1f6f31b506ee47303a23ba63384cabf0d317b68a30bfdb9504e34db85cbf8a84f8423c68fcccf2160fd1bd8c95bc7c504dd6bb91e4c1e9e5abb36" },
                { "sc", "0baf74fa020ee70010fc4dd2b25e39b0b23cc6c5f8119bd5724108f4afb592d85ac6623eb999d33f4ca881e9e020f5782d7a6e50d0eec15bdac5599cdbeb6359" },
                { "sco", "756b0190b09d78ce4e61ceb825dd22ccfa4353c9a07cd0e31828e3e186fa3f154ce9853b347188c42d3254cf5ac8a600696e381d55aa2675773b4de401467d06" },
                { "si", "49fa87cad3eef67d3e64bf813a6cfca8257cc6d367237e7e6e822b989d0dee1426ad26b579a72ba5f71aeeaecb5b6793b999b6eb4ec20c4043de6dba93f98c28" },
                { "sk", "42935e7f87b05559524e3ee5912ba6ae5a4ea0ffed9e49e045605c9e304c207f9a9c2911f9e07ed19b5f3d35058c0f8608c40be6b0f8adb3ccf50e3b38ed5131" },
                { "sl", "56971bac6f57dc6783820a9269c0fe6c561ca8564d051229377d6ddc5992b04bed3333240aa3cc2ee7572b3c3f6b8f2cce3ae26b65c69a59b6005a16217b1262" },
                { "son", "692f6f5aa328677be194702487d71f656b6d78e90d726af210acda756059fc01e605d5a66c05fb0150aeb8e865249c283ce55c74e2a8901dc8fdce0834922394" },
                { "sq", "17b5f5fba835eb6d81e025533881295b1ea2e57518c9e113b2a48a5f767b254f5faa1be6dc3f210a52b0d05e5e210df00890fcb74d353fc6618ac4cbb567a5d0" },
                { "sr", "00df1e28106b213efb590f45370eb18fcca5ed894a63e8cd5235e80717dc9ed61397effa209742ed94a9c3e7adbd7b2a07316bff4ad674b9cb0263c5ba938c93" },
                { "sv-SE", "b8547eb74cab8f753f638c2cf6dddd671e559e7c5437322de54c467b5882cbfeb9eec0dd1d8887f02aeeb7dfebf9f56c7beffafdf0ffb5c1886d7bf16830c479" },
                { "szl", "6f7e903c957a367bb224f06ebd6ca9e46dd9aead24db463aa7590a5fc9342f66ed63938317d1676635d582807696dafae2d4f1aedb9a7d340ee78aadd15fd904" },
                { "ta", "c3f4363da5343d483281a28ac9caecf07e86ae0b5bb3c61a0b04ab4505809106296d6fe217350de4ab0b148bc58306e6ff86fcc8b9838b080bb734b5e83d35eb" },
                { "te", "cef941db2d4cc23bf8cf13dc01ec66e971c6c284d0a2ce53893e324f83bab214262e77a2a195db39c11cfb98d242ad62c5006d0dc1d3b2531ab690adeaa48f79" },
                { "th", "4bbfda9100b1535beea5a9dba6b059c8c269fbde2ee07498a3eefb1bd997b2b83de134d72f1756ba74e52740cc154640c137c3902527b19ede2912115c8aad04" },
                { "tl", "5ee2dcaf22854fcfc439081c2e7d34ff3e07b96267140f559402b5f9ae4e1783e201fd7bbcc1b990160a807fb17e222605254543d980417cc90f43b10623ffb2" },
                { "tr", "bd1c0281a8254bef245b4c65fa6638fdda8c230ae21082d50c6b257c1c776136fda973004d5e4af9e1410ed19f6a8c3a7f81a6db4ff04666d011daef94c0e7ff" },
                { "trs", "d9b90898b2292ab9d0399888a13c1de292a77fb65eb442cebd638e40968d62b4b4148268e3d1e0f2f86b23042c00d5f3b6cd0f54c80d929db676d8780951d540" },
                { "uk", "02940c9c76a54e3cf1a253c20ac1a227a59b2a5c308d70912039c68db4ac140173ee933d42b7c06f27c64148ab22bc6b5bbbf43f5902fbf27d062a637271d2de" },
                { "ur", "4217c336da08cfd6a1db0e17604b5f7db7c94bed9f61dcfde8d94d7201a1a4c85d1c58156a33904eec996f8f5837f8c2738a19d4ac0133b161cd69a685864ac7" },
                { "uz", "99cac1a1949ffda6cc05c63a01f20d4ecede651dd74e5935a65a77c80aaa8cd15d16e1cec7c3634bc7594e537b547cfe7311b0ae776d2d85d7c57e5fcc4c2a78" },
                { "vi", "999bdfcfd0e61f1e867ee8d80d473dec774c11977cfb1853da1bafc34413ea42a008fc07aa97b3aa0add26261572d71b9aeaae91a391f92c06b5ccc65e166650" },
                { "xh", "0ba7e969e225be179f7645b84a96b73e9950993f33c30ab87e7c2a37a343ae85f5750a2f1b9adf3635bd6201be06d3817d00ecb16fa7e9ee8542e760d5cca66a" },
                { "zh-CN", "e689fb4f251fb0630350f0cbfe91d4dc66792f0685e8b6963e0910e0fdb385ff82f2f8bb5d702c801220edf3f4914fdbf5ce95b3175efcc9077a158b35481ddb" },
                { "zh-TW", "6c19fb53cb6bcfe1117b18c5363db95f16f4c2e83ca597d4c477acc48be1d07d8e82e3b725c57cf3fc287b3c7164227a075fee89d17cb3583a57c42adbc56465" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/112.0/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "1c265b8d22791ccc6202e709bfe640fa8236fdfdda2432eb19267e18000a35fc2dea95efd65ac9af4721092295993188ba8c4ed39e8eb97bcd76704460aeec57" },
                { "af", "19680d7e8de06fe92ad0880d44825a177de65e2ee872dec74cb2c1e0832e3c7107d59162041d1d1cf55b40c5fe35ff7ad6a1fb37568a0ff16f9192b2461a13d3" },
                { "an", "aa2d443d4c1b5a7e8fd2b2c2e48c1532807acd83d3e9f19143ebbf41e7306927468c8c088176541b367f935a0c6270446a38a50e9ab9dbdbde1404acf76666bb" },
                { "ar", "e3eb36a3766470c33db9fc412dc2cbbbc78bea737bbe1a3f175de5f36a3100f6999c97bec2ad9b61f7bf8a930a15910062e124754cf11bf00d16b83a6006220f" },
                { "ast", "676f759506ba997764c3a324a3ee89df8394446af1bac9fa06d09e0616a7c66eeb160afed2a659d2f32f0e559c8819d7292e8da8b551940323520af173b363b0" },
                { "az", "35115366641b7de303ccfd2cebc28a29edc17bd2b96a0dac53396387cdc4a19d63fc46271684dba4d5906bb70a9645330a96941b7916a228aedd605379810629" },
                { "be", "4c5fa63d9f4749731aaa62d4c972e68262bf71a91782a7e3813b238653f89e0758e1a01d7f5e926eaf3c6792db6d533b04e063e2a20d2b61333cd889325fbf66" },
                { "bg", "d700ac0d55b90d5176051d4b4616491ecdce8a419974df7bb63ba8bb712bfa9ed5065a91736699500379454341bf7c8033f66484f8c010b8c12c8c30f194de8a" },
                { "bn", "a19082ae5c3586ddf14d91d51335cb37b654e17fe9f161620ce870c06ccf53bb0f0528f0cdfffa88e0b3caf09f962501e50be82215f4165f68127ebbc3ec528c" },
                { "br", "781785f8aa342140ab381ec0a8f64dfdff28c1d37224e4e7549467209842ecf4c1cec0f98f95381105feba18aaa71fd1a8684163fbe9eeca4a5e9205a80b7fa7" },
                { "bs", "2e96aa91c5da13ce35d00757e55f4c4b7b96e52eaa8df75b298f8edb155ab17c16a18b4077e8c24a0cfd628f89ba835ca16b9ba685f1420fef9c1d501e627851" },
                { "ca", "8c413ff0268994ea1c4d1c01255ee3563e561504244db920c7d6c6ca947c850774dac77bea07b05ffe6e7b78460f13f31aca42c5516ebc5cf62a33ad5e9e2099" },
                { "cak", "0b6890c64447d9fbb7a2f830400f0579e032a01bdd1d22380946bffa4bb9b2f085b2af8d869604095852e7f659f966c5e605965794f4073827faf9ddbf231c05" },
                { "cs", "a9b13566638aea7b623604af616a61f0fb2d037667b1c4462e0cea852c3cfae54368870c6f5c1bcb2dc599d3e093a544ef362db8c2cd4a30aebee8175ab6ea8d" },
                { "cy", "9e99fe684d58a3e5de98031ed6fb80d233e8eb8f2927e5d158b76254edd3e9928f41cd5c75a6a4c65324773b38e2262e8e3f784970ae805be8fc97f0f120a7cc" },
                { "da", "3a0c3026d48d4e79a58551a0216cdf56db2c2daf7ab6f544842dbaeed6e52be60f66679375d11a3d8b1a056a133fe1a4b0c20573ef38d0e2c2d35247dc7711a4" },
                { "de", "8977e4aefc3d03ba385e780ad1a6df95feef55469b017b56e10bc40c22468080e8924949ab6cca0f91973ab9f4963306b6203f35b8d0e48453f537476e04aa47" },
                { "dsb", "6ee524fa5f6fb79b7f7217fb66f820c814799ad95ee1cb0dd43ab566b949575827d87d323484bacb9bd96d6a81e87052fe4037a1463a7b15ad4983b73d129875" },
                { "el", "68ec800cde4ecb09ccc402afadc8a90947438b8661af7b41187b69b0b50943afcbfcb9b6c71b4348c66c9802c1a091e910f58ee84f302091c5aea180657924c3" },
                { "en-CA", "550b1eedfa94ff033d80f0bc487109f9b00363785571ad103829ff9ab48ad36c0d562814d8257a24e6084b50dd9dbb894be036e3f42daad0c33d7c3dc0e0c95c" },
                { "en-GB", "78763f1f0992c6edb60fd6fcdca1916b1f9c700369751bf046a8572f010973f6607232bdd16a4e37dde97f2f56d05d7e591b28f6369d2e02a9b934ea3000a2c4" },
                { "en-US", "6f6a4fbd91e921d81b7992eaab9752eca2a81e8be13a82b7ee73dd2419375edc497f1ad32d0dcf6460f4ce8c97c6edd2e02bfd07b3ee49809c17e11f4b0e2fc7" },
                { "eo", "a410545011226f220a86492be231ed53dfb9d8752ca6d3442b239e3156acfab42e6874fd227b79d8ba611e96c76f0e410f9069347abd42d7c1096871258a1d80" },
                { "es-AR", "b1dc5107fb9b3c53adb414cb02bf44d65e40999c6924b51dbac087620349e346d7083625b2fd7f7538f5cf39e07d25836f72cb131c865098493696ea98024010" },
                { "es-CL", "145fcc16e26eb65aa8a307d4bef73314b1a41c67364dbde1fa94861030ecebe0f7fd34b32ac88dc10dedb7a1898307feb656156c50ffb6479dfa70832ef6a8f8" },
                { "es-ES", "0c2e3e091a23d188e8b4e979d4678e6e7fe2405abd0de38c7d45b01d3f6debad12f9716eb7bb28655dfc8d96980e09df3fdd66910baaa769a56bcd4d992ea480" },
                { "es-MX", "4c665b369b4f2ceda5439bb64717cbaad5c3404a9f28cc1fed5c0c1ae886399765e7bb0185ecfac6344e56d3b3cbd686d7679e39692774b9948c3a2152689be0" },
                { "et", "89d8ad6826f3b7e4067c8805780eb2f7f1ab62c852018938696ae0963b238a79756ff4b63ac17af8d20e409fda3762c3ce54f49e059d4aa2bda38638628acbef" },
                { "eu", "ef59e5225da32a60547efb2c7970264c9c0ec95ca4d5688038ea4c05445e4fb002a87d8fb0f7df4b83d8049a0b735fcac1ef2b2f69cd735071d7cb6e0d7bb8b4" },
                { "fa", "b908d6eb11c8c4d96e1b1a7b1c2215b1f5af494aedff82c5543c34d78960850944477e82842f3d208bd9e83caff7213597d5851e54f079034ea8382f11636483" },
                { "ff", "e64933ba9947875d769b2c3dad3d179956836994f2bbd9f25be336f8de6d1766968b0d71c005df427dacc1ba3b82cc646d5325c73a9e96bdefd9be60c200581c" },
                { "fi", "a77484a9dc044709394959ccaba46acf37db48e4a7307c3a52b07c37ee1b700ba116c29f2ba368ac38fd34ef0376af7d48105752ccc9130db6322b9b057a40d6" },
                { "fr", "d43ad10f00324a37b51a7a4f576f8e9218cb07e06da6efafb319ee566d45ddfe3727b9d6d733d0a628559b69c2de5ed1dc8946d6392e54ef9ecb2be0e25f7f6b" },
                { "fur", "58083e2db7827e8c92e23e9da7f1b06902780a3352a478bbafc5f1af4baf29cc4218c506e3d2230facae2902adf093d80217415383a9d90a1013ee8972075255" },
                { "fy-NL", "2599e6d33fd2b36c661485e2067f8cd6927429c27d6a5b169cae2c309c4bc759611d81e2e2f43560f8b3f83655148c55ed61b8edf5dee89d4cc866bf33fae27f" },
                { "ga-IE", "694b8b0cad7e32ad1ec384f0f8221bd73c732d17ddb6fbb7a4e6ea99161046192556f1ca61c8bbffac4b2ad1d4c2df91c182f5837d2edafd3849177384ce66dc" },
                { "gd", "2e1641361d40d5466085cd4685778ec6654ebdeaa6ef06647e2be6fe73ff58e2e681e7a631fb4a90260b79c16c0054a5f4869bb33b4ed9351cb6da2b7c74cee1" },
                { "gl", "af63ca1a2f4b4a1c391ce012efa268e016f5823a01f19794bf6eb002cc59fcc6dfec9934a78d21335396bf0768e1caebd0c6dcd88c60fed43c37f9db5dea4a9b" },
                { "gn", "b64997e7fec8f88cc19c718045c4832890725cfaba02119bb1d3936c0a179d43890705753e5ededd0ae95781b0028fb24a3ab0d1497ab235256c1c0a2a9c5a48" },
                { "gu-IN", "132e76774dee84e6de58bb5db628dae9d2f37345454fde3d9cf57747a3e6305c2d52c6465906085dec1c9cad47a6800f97a089ffb367a164e7846c48b35d9109" },
                { "he", "8a7f559cebf3c7b21de88727b4f6b0389c8e813124797ec0e670b03dc33ce5e4cfbd9199e7fd2aea2737e566fdae059857c627604d30b674aac73447a92a7bfa" },
                { "hi-IN", "d5b82bed00e0790f8d10f80791d886d129bac85821b90dcdd3d4a08381e8ee3ee7d6d20904d87d3526b73961b608e6bbf7d52f02f0b663fe91e33a3f67d87725" },
                { "hr", "001912bd63b71cd4d884db2b78e67f8c0a917d177ac4a25722c8f0546b3d6f904492ee709efe5f5e9fedb3deb31692a9827e20f82bedbc888fd1b86acb3bd94d" },
                { "hsb", "ea815b90e1cbd6fb6bc52801afb3cfecb4747614750181410a613b76817d51d3061bfb50e74718fdc73d012af7a31f105af0b9d0da4529afda85fea1b07c9d07" },
                { "hu", "a85c6914eb625bfd539a70980090fa05d7c805498f3b64eee1095705379a3a2582e4d196f1ed6561701c8f18d4c4bdd4a2e7289c02105714ed8fc8f833512baf" },
                { "hy-AM", "6286d064799832bff6e5e5e376068d2cac96a10ae1f344db653ec12d164bbc0c8f341abb10be2ce411a8bb704cffbdb94965868571256225e5e4b4606b9e8e08" },
                { "ia", "79a5851af3c1ce4a2c8754b997ed35d71896f60efc38989d61622d937094c56d7e4246c6f63fa1bf0967f03795d2cf95cc24760cc14d9d11fd0c49acf0a89af7" },
                { "id", "3abcf55218f6e3cdb0eaad98c26084f17258794e9947dd7dac3b27bb4aecad7ef6452106d2a9e7808cdee6acd952690440b6b9f9e3ad7136206a7444ce9623cc" },
                { "is", "8e77ffd214c249e4a36476916598fe7324ddbfa0db88ace2652c8230999984b766c92da79c9bda42b3b19998eadfbbacf78791831aa415d8ff00982475b060f4" },
                { "it", "41ed95fff45f5632025fc77bb4b66d82b2f44f7db2b252f2c6ae2b5ceb035d0e1d275ab1a624b3d4cfedb074688f93df483635bc531377091591ad826aa8f68c" },
                { "ja", "9e2b1f374fa94fa14eae60d415377450fd5eee484b2898daffbfa4075b40d71def7b5ed71172a75ee49bf0e19185315bea51dd81a34e2ecb5503ad0d19a547a3" },
                { "ka", "b0fecd3499abbf3156fcb0a4a40770568594145e950398c246f5257d622bb31839232b2d7337caa58aef85c79e375645357ce5de573811162d02bcdb0a4521e5" },
                { "kab", "2da62d6d6706cb20737082815a82e7263adce9d6290c5f7eb38dc15d235e4afb564653512236be9c82941977284520a9de0b98a983eca6d136c29c8878ea738e" },
                { "kk", "8242d8b9c78977a902c3bf5c07f12c8bd1ff4a08a8d2ead232e9bf8789f1324724929eaafd64f6bb582303d56529127ff0484cdc6383f91120f04e269d472e94" },
                { "km", "d91082d0c65c194765b1dfdaa30b78725d0547881427cb980238db53815eb88a9e4ffd86a0834cd4e355c6ba78dadbac8ebf0bb479e27514b54a5fda170fc90d" },
                { "kn", "7b086ace2d22d560fdfb3eea022bb4f433cd8cb4e853f36cc78af7a7d53078b1c5d084773be8b9c38cb8ee602e0bdab3337f93aa9e43d77de76d24dcd7fca2cb" },
                { "ko", "fd83b170db35e81b26f4c82ccc46dafe4129d9a4906dd4976e1a92e4804ef53ffc41af560edd29ef400c287646c82634c8123368a73d87bda42ac2839c3c45ec" },
                { "lij", "f151752646ec4598720fd876f33e955dffee0b3000987b381b0ba70ab25366b491df1f9a72d42915ae18e292492775fa298e4efbfa4ae52a4f17ed97662d72df" },
                { "lt", "36e0d6af49924dd747dd88ea6f76c0aaa7317dd3131e3948b34e26eaa53fba525eb970616dc5d223c7d91fef51cdf4134be19d11942f474730bb504e40e565e0" },
                { "lv", "970f4e7e7a1c98a70854b5212966cc37487e98442ceb46908ef56ee904276f05188e3a8f5d6b7f9180bd94b23e4ad78608b28e1b47dc29e222850015dd3ae29b" },
                { "mk", "5dac8cb062e105f43d9acfb5613fda4b5e6019c4fc57d570fc001e07e06245c26df519c464264d169d314e23593de1370703a2bac40eb067fbc1c810524f05b2" },
                { "mr", "f9a555ee51d2328ae0912d84aaf31c82548f5c3e5f0cce30a69b470e6f3e788347ce89c4aa41e2a20a14d70074e8b37232c54040c801f265ba1c15ba40c25e94" },
                { "ms", "219815b98781ad08fb5cd1a46052bcafce7afcf04f7593314649bb3ea9ad37737ef3ecdc841f4f152f844bc21fdaa3c6af7899b20a1063dd2a47884afcf17ab1" },
                { "my", "0c3690e801e7a3b5f123bf5f91eaeed257a7ee2f990c36eedb3889ad2fad6bc267543d16ff0b06a59ed02f487d0e7581b0d6ccd8b6dae76f1354232bd7939367" },
                { "nb-NO", "875be1e94a18d9c9d23df53993932f33ffef31cebd3bba9f60bc5be4a1ffa3be671a224d099408392d950cef23be30467e8f6e38627e79213ad64f343014a09d" },
                { "ne-NP", "2503dad3f6d78e6f5b8d8fc2f0cddadeefdb6d17f1b439c9c7861fea9fba8377050546247558bc28348aac0c85a5ffaa77a5033671362ff4252a5ae3c209ec0a" },
                { "nl", "fdf42b2124fc3cca242a432668ff33cc85795e763c4f8b17eed7657781ac9dbd6695ddb0db8abc17f9cd869d85d5080bde347ae5dc9f606a0d1f5f6cf6a2c467" },
                { "nn-NO", "fb671801fdf433c579290c220b0bbf6e2c2f6d9efd21c05d71e6dfbac374d594210000b93ba9d7e23fbf5b4c135394d9134439ef894c73dfe7a64ca7192ca8ed" },
                { "oc", "0f9a9d187039cf8dd562dd3a981a107638ed4ac05c1c19dd644bc911bb06fad8190079773635dcd64f913ac43a8d6a32e32714e487fb3ceee94a5517fc8c9bb8" },
                { "pa-IN", "d808ba107a1733b262d94f1869daed55ac771b9ba7425dfdbb5e364ad4adc14bfc548de6346f34437642fe300e3dea27a91f5b86c1c4b80cbf561424fb09f264" },
                { "pl", "31d2f6faa0fd3e769ac8db4f03005f8366f9902564bc7a8a62ab64680387888db92c7421b0d792761d23417441a30ed7d86a92d7b685a814fc5054ceccaef8a7" },
                { "pt-BR", "11ee55eabd3b78fbc167a4c078ede84ba04635285d21b7a822e9e4a9a645ec4504fd79e0ab03755ace2fe113c03c74eabdb9041f0344f58ab2d1cdf301ca01d8" },
                { "pt-PT", "23107d9b82c30ea20ff067908fa935b98d58426faae47e01bfd72053e11021fdebb7c1a1e4bbb115607ff679e4259dbb65ba9fd2407ac17dd17d9ffcd54847ce" },
                { "rm", "ccc09df79f46c6f266a392e2e298d240b050bd3f7199ea3b4aec2ab91e2f26585636508f77c7800841abd69a6080efc5551e5ea7ecb1593b3a2cf0e3dc445256" },
                { "ro", "9012deb50963a0820ff05bb26c2e2b5ecd045327e19d9b8363eb087e2b027614ce8e0a1cbe8dd04e9e65114ad5fe73b6114ef543e8ba6350848739b41997af01" },
                { "ru", "74777b6eb8f88f62727b5efb495a9e584d17ff67316be7ddc25f378e1994a432f9d1d4b15c2aac33acd26176310052927c5e73c41c0514f32416baa7a3fcd486" },
                { "sc", "b35fce75ff4caa4decceabda61e73361ba8e7b9964b9353894a67bfee545a21ca55c3268aaca10ab70229b5f5e7e3b12ef8785d210dbd5b4c613f94049ae195e" },
                { "sco", "404f613c0dba5f058eafdc4f126985a17e1665b64a66142d5ec96a61b828131204374364457b442dc1c1e84477f694b2e1dfd95ba140f633e4fe1fe78a7dde61" },
                { "si", "180fd0843506a4c57fb3768780368fd8c70d3b0c6783c68c66b1616d95ec1dcaebfc6420868fbc8d7bf434601f83271efd58d86b26b8381f771e70992c28cc61" },
                { "sk", "21e6b5433a228e1fa28c042c7ebf3f99b1acdeb613f417480dae3efbcf4fe762dc7b7c64ea76cc60707ad970702d67620682ecb3f67b9e5065d94d1eb282911c" },
                { "sl", "5e70b3e429a03e12ddfc97e422d03ab9b041d574948ef7d941a6ada85c12ca9f4576b6e5ff0db720e9c7e2fdf25ff8f12feeafe8aab5342f440044368aec6de6" },
                { "son", "1b69d3dd97910b56f9dd94af2a170b0ddcf880f6e85eb8150125c8049d4d25a93b5bff2227dac6560838553a82d8f120f633c7e2ff4a72005e5b510ba4fdbea7" },
                { "sq", "b64f8d0b97e08ee7593ca4b430032879fe69b137537e89585069da7fba715b048cea7e7feb8a97b721b73c9ae7cb442f223653acdcc17e1c84ae1edc29cabb0c" },
                { "sr", "3efa1493bdc622597e636ea3ef0337023691eeb0a94a94314d4aedfedfcc76766ae51848e9d2c435f82775e781e99d878857fba23c09d99db5965af009eda052" },
                { "sv-SE", "c23c5c15cd9aabe9f48f0524a463e03e7c09dabab3cdd905ffb1fa40c8e1e47e0a18ce5c5f44c99f38ecf96dd3379f50994a777310b216227fd0ff5e03169dfb" },
                { "szl", "4b5c283c33f971347bab0a3426dd7e403e3bd38a4c507012239921cb09f6f48b4ac51a4bd0de8b0c6ee13756290edd4a7711519a4edbce4007fd9b8611f1ae31" },
                { "ta", "622164bd29944dfa13cb14e8e5f892aad5e9bc1b60baf1bbb95e49b8d1188b8ef3edc63e0b705b2ca1e2bdd166f05541d7127f0c75c7555b35bffa2ccf4d2031" },
                { "te", "2102ec0e28f67dd32ff864c5dc7714aeb2f0ee1a2a613e26a26b3157d34b145755095b00410eb54b6065bc6af19389e827691c0f71de0b4e6fc49da9a2e5cfc1" },
                { "th", "81df6ecfaa53a4c9e48bf50deed69ad1085a70d7c67bf6038f70c811bc16a4bb7e81e09146c100072b9ecc0dd2ed27a9fff40510c6c38a60b48ef6ac3ed537a8" },
                { "tl", "a7e78543adba62f9bee66bafaa84d472fdf546360002e1f70c9c946bc69849efa9e938896345fe68e6dab1ad0c5de16840db9742b01b7a6ad2beb8a4b446a9a4" },
                { "tr", "d307caa68a0504c115af8c5e8ac455a651d20570d18def70e913855517e2327720158ef76818ccfe1692bc630b6296debfd9757eaa0a902b2da6931c1510fb0b" },
                { "trs", "bb1e3d64ce42df538836b26511be33d5c17ba2a89a9613650fbf48de5e8a145396db0111ebaec979dfcecb7d7db03c13a2a747a83e2268332831ff268f4cb627" },
                { "uk", "266af5b38c6725dd72de1d0cfb6c263356cf116b7033192af97b6528d3fec802a58dbb94b0bb70ec605e1d8df10076ae09410b95a072834ed0447e80fd231920" },
                { "ur", "80b503dccb90c9ab693463aa69ebfb66e4a0485bb70c7fcffb70d17a8789eab44fc80cb7354274a7a03d0821550d5566625f926536101ba59c5ffa0dbd65c02f" },
                { "uz", "bc53339ad28230242dc3e42a9d35df7b30be6159c515a52cc5bd62663e4e9917ad24ce2f7f4ae546fa8850a839ae7b6f685ff94ef3e3a604eda70d7193282a60" },
                { "vi", "0ae5308e380475304b5aa4babad7c7569d87e462507bd49289767cd0250a361aaf3400b283eae1d5d68556424e2f5df865c57f7fa96fecab0bd2b37c5030f59f" },
                { "xh", "fc84c1afe48aaa08d1817f8e337404bda6c8fd86392a445030be2df2c1ffd0e50c14babf349c3932ce9e7635c13d99d0b92b5f8c9f7abbc761666e2b912c0dd3" },
                { "zh-CN", "aa0e257a089f392b779947fdd10ba81b6bdcb3a28f9b6c6c6e5c49cf457954f5d6a5f1e60bc602e9c82a8f4670fc3796ee9fb8bf1a035700b544d3df41873317" },
                { "zh-TW", "982e3e3162f699d834b4f2cb5def8c8f485ceca9ef9ad2c1177b64ea9e157cc2bd0b13d49b510586ae8f446d08a774f3036305072e81f19b5c629794c0c6c47a" }
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
            const string knownVersion = "112.0";
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
