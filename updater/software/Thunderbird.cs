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
using System.Diagnostics;
using System.IO;
using System.Net;
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
        private const string publisherX509 = "E=\"release+certificates@mozilla.com\", CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


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
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "4b0e51f06efdbbe15a44d78d84eadddf39555d2fce6774a01bfca41a84654e71cf66203a371807987dce1f1f5c8b15289a300765ac5fb0aa35609af43df34f2d" },
                { "ar", "a178110cf21506e0d012d0ea07fc6ccfac71f16414fe525fb0a36aee5aa3d58d5591b257b0198d3755cbc78ef737e1401f1c295d6697b1a111e529a4fa998c6d" },
                { "ast", "4421bf1bc2bf423d0537c350436191bb6ef50806e4830ced6da61bbd8034491b935cc8e42b8567859c31621dc43d60e41b56163dce96e16d22df855ca7ef030b" },
                { "be", "a4a2686a62803bf05eaabd4723cfed4ac58561e6b98f8f5eb0c9a8ac0e097421d2d783bb9b66a6abc623375957fb566dc4e9ebac4bc3a2072a3a3a61a6ffa781" },
                { "bg", "390741d5afba7f6fdae4281486de43091f0741f415bc519adf633bd5a752340c4770d8b7e0c4a8d782841c52b940b0a9d03b31002d62013509479eb345991cb5" },
                { "br", "c0f1880be198adb0b2345e39836492578c63bb3a769c96aa1b365deb8202303cd3413c0368bfba13f157af27260dfd7d5636c327e03aaab9e33247c9afd315d6" },
                { "ca", "ed530230dc02b8cdd09d4656753493dcd195fe05ed3f02525138ff22d2b9a7f058f8091cdd6424c5dcc60b298a2266b7fb822a2ae189ca1b3fac80f2d5d72716" },
                { "cak", "adf01b49fbbf9dd12dec33014bce0acb9d8b8ff93db59fec55aedaad9d2eb82dcdc20cb68ab5cee24a5e3abac102cd08665e017ad4c1d1c4f5a69249cd7963a5" },
                { "cs", "a4ffa67ad81e029b6709771b4e10dd362f11ccf5ca32c8b147a59cd327e2549f039653b486aff202286b35b1983becaed485827f4d8bd98e5d6dd1cb546cda3a" },
                { "cy", "0ba6d578d8685eb59c2bc6b8f88f409c274b428d87e4eda18aa477e7ceee9cc7453206c9a5142c9fba49d1d138e41ba36dcef11e1479d3ff4a9462efe0e74b14" },
                { "da", "af5e7c6b91fe908bcd98d3e38ef563bfc6eaa37252ba5e1d38fb61fb4e2bcb6fa27ff5fe0014099316455d1f4a52a440e2964614159b3260bc395cb12685b340" },
                { "de", "da23bde733685a63044149eaa785834bf96c1c4a369c4db5dfa6748bbb7828273f254981dc3f18b2cae337d9826aa2cfd4c157bfe7e1776b3b4c074382acd9cc" },
                { "dsb", "ed3716081d99793f9a19a20c140fd312da13e11c8fe62ca5a1ac68e90a4164bb850bc43a5b43f5ed9aef000264f079a1017d68507f4c5cad8826fbc7a8ac65e4" },
                { "el", "8006852a650ef7b4246e8e8803fe9318e6a780825569683e7181a26a2346309c50e7d9c15e2feb2e6fe43564d67dff03f50673fc86cd3024bde9f1045faaaa7b" },
                { "en-CA", "5b1f26d9cfde94698f73d5aa900e294f44110278f463c9fa272c1d1f4345072b124408e141849f7e956d1cc1be0954040c2d7f29d8348fd941a610a5b208dfad" },
                { "en-GB", "c30dbc62a01cb1c6652138195716c9a0720337f0fffaa63fa7768ca8b0f7b780087966f346261d2d99a02d59f58650d5e2c2b6b7368f30331e4c138791950470" },
                { "en-US", "083758e2a0527a5cb94a63cde0246f68e564cd11a4b980dc9c55abb09bea04d0ad52ed232aff817386476da27eaf0680cbe81262c07ae216f6e149ece9f00c93" },
                { "es-AR", "7937ef68fce4560a42117fc75378ccd52ba9ee42322a909f7f373603539ca197b78acaed9590702791e4ae6eb8dcd48dba6d778e40fd4c187d3187a42c927ef7" },
                { "es-ES", "4cf4c15ce9e871de4105288778cee05e4d03fa3a26860c77d7dbde3ac0e7429ee6801123477bb1e578153571d8ee23b0a1dcbb854e444f87022b46d87e515b38" },
                { "et", "88e579e0b5722271fade0dde4d23106cb56bbdcc468e09c1508b3d3f8e4b6cdfd09c24494f5a978beb55eeeb5f7f7570092850f76bb67a5e6560b78b1f6c6a64" },
                { "eu", "9381a0cd775508c17eb9ef7205962662144fc88f6fc4d6cc162338531221f82122109d00c908f7c04ac5ee19f1cf1961f23bfe9a539290566e1edac17ae9a36a" },
                { "fa", "13d8b2e399ec0b1884fa3eefe42ee44ec1343be85aa1a6e8371a99395ff20fd3c3f354f84dfb867e4e332c6c00e2f1fbeb406261e1559e05d51eecba2e17dcec" },
                { "fi", "de4b53b84c1b1d056cc4a2ac43e8bd4dc1c2fb2621d76352a4e2cdcbe43c42356aa2d88d76c2d0316bbd3aabc992e898db0f0d5f94c7153803bd7656c6d7728d" },
                { "fr", "c5321c09569c949424d0292d911ee3c95fcde3a7d7296e643ef14dd798f6299000379dba4f0ed7a3cd75449747f9b4c3f5b7959d1aa08b384f9581752b522d7d" },
                { "fy-NL", "03ff922eb53bda12a9221172570fa640ca1daa4c2434b0dbf4e9d5b0f9ce87b0978dbbcfdd94d37d18ec21a3397a2d099609b8eb0659dc7b16971629ec0a2fb5" },
                { "ga-IE", "8ea90fb3869c15cac8a3be6f180c10c49bdf614d2d0f9f3a69401d5c7b6d4232bbaddbc0314542f6f162148f5a502707e5bcbe051b0c8319b4b1fa9d8341690d" },
                { "gd", "7705b91390f04b4808104c575c6d66b8bff679e0d5ca5cfeff4503d19fa72e56f8941f935f1c5cab26a071bbba25fe3dece8245b1360c86d9145d856cf31668d" },
                { "gl", "88ce3f9dea0d8f4d6c27ffc22a5980a07b604ca59d2924497e8050dc42419b7ca0e06d8e5a2707e03e4349d48aca30f094fd585864e82fcd03627b2a1704d568" },
                { "he", "b9818e91398dba47ba364969adb553904e24d1e7f2f9f8664e11d11dba6ebac5a5b61ec5a03b1fa7d183acd037371096f41cbf9738c357320956acbce23b6cdc" },
                { "hr", "1d08dd07d0b161fc7abd78da0feabcfaf64ba8e479b30c14fb1e790e09f049e9dc1675df65ba05df926addcf3ba8d847316b1980dad52d59542d73e038a254ed" },
                { "hsb", "faf89eff13b6d9c503d4054515cd46b883fc714558c8584887fde6bdab68713d8951d8d9b8de85176d0e81437fb8ac0b6298742a733bc0293b18e046f0a6be6d" },
                { "hu", "dc1c230384d128408e5ac4df2f8fe969076220ed5c11c3e11a120f29c291a1122084948cf19c596bb07e4b9d811fc37d6be9b605917c68f375ecbc70540b1ea9" },
                { "hy-AM", "6fa206bd9ea6748209718c7543692d11aa172496d7170eee91beee6bc92e6a85b79478db4597d2883dd70998b58f652fb4751416a59e641bfd9d06aae9a9cf47" },
                { "id", "3f055e62bf1bc904254074e79d1cdb70e96e063a47e3f43021b015b14039b30c4fae50a78527069ba38dd53320afa6823a4d907def7fff2366086c8af909a05a" },
                { "is", "5ca58f7f89805f2827d033d7c63c8fcb793b9a016d72f80aa644e4351c570e69f3814119501dbcab295af28e9c2fc21ea9a3d2f19aa7b8e04bbb92baf65c2359" },
                { "it", "45bd8b01bb6b310d0590fc9bd15ab401927e51df46f546e2348eb9e37600a34ab7079ea8d2098331eedbe940b92078a840fd2f9fc16578641f2c85e30c936126" },
                { "ja", "9ca1a3c16cec8477783b5794c17e7145d7c77e564d391406a5a6ed4bfd7bfb5c34c87ab971558a7c5f7ca33aa22893c25f897b166831337955e1a24d2e84b371" },
                { "ka", "519d376af62ebe5768c58d4d2b8f95b6013080cee7919de037610bc24ec1ef682700a2e3582a209c7108f3d8b4176537c89e610dafc8d4ec05ab916fb4163bc1" },
                { "kab", "0b554bd4ce4aea174ed2069186513f8cc0691f99745d620e8d49a16c128ef32905b140ef97848e98cb1232bac8e0e28ba4c1e0cacb702bc5842027f2d15ee450" },
                { "kk", "9fad0bef684143ec1f72fe10eca3a28a3b650234ea94b7b51447ad184d90954322ab7515fd799bb21408dbe3cd241ea3e8cbfea873fa291b55f4c99af061d79b" },
                { "ko", "adcd4a67cf216b6f6c8146faf63803a8280ddf579f3587f9d443f64d744bcbb8de2e7f76fa922541b81b3f58f3cf87e7a7f9c106e43ed63ea8f927f50d008653" },
                { "lt", "d4497cdc2b87779e244c36eeced0d6f536c1a17227a4e94e0346a54959bcc0ac51da4b445fb0c2035f2cd2c75cc4fe137790044763d3b8582cd4dd2c68ceb878" },
                { "ms", "24ba31c08f3d959009b95a99c85abd8ad0547a0649e31e18e3c570bb26e8d5b1752067e793b8b34902bbe76d602c067c3ab9582c5fcf12a8f05eddb8e04ae083" },
                { "nb-NO", "874334631443171864c116e5c13d890b06948fb5ccd968547bc3fdc62631ae0d851daaf161f468f8b872e431cab1fbcbb54aab000536b23fc9ccf175d75c3299" },
                { "nl", "071e7c930edfecbdf3a8c390707dde0cc49d5ece79607e98b2d6469e980568c8b2edccc200aaa0cd431a73d40ddc97c39b458071f62c37c38d0b9724e23d3503" },
                { "nn-NO", "b03023e96ece5c9ea9ec832af43dba3ee211ebcd2fc46217890d941f6ab2bab062b1de7e111361ffa6259e563f699ee4ee952dfd0a4e6de4f98e5c840477d277" },
                { "pa-IN", "bf5054cdb60a90effbcb39acbc849a31c3416b6949ba150137c7cadc2a2e6b7038787323e96ad7e30553e0b3ac538b34e1962d3252868c29da842437abd807cf" },
                { "pl", "899d44b83307ddeba7603d6fca79576fa2431e264ebd9bcc8bdeaddb930ced5705f9d27b22b17b4f84c4bc4bb3118e590c5ba0189de25f7dc5b2791d4e5fad5c" },
                { "pt-BR", "1ce81034a4d42c7127f45b96e5a40831898e53ea693321a07c2fe430d98302456a0770b40972695bdb5339f3136c68eac161f9bb838fe5bb95af761e08f310cb" },
                { "pt-PT", "0a89eda9996acc73d0eaa59ab758aad331530ea0a38a73232c8ed61043ab9e9a610e4969aec82e45613d78d5aa01cf70eb3588788bbcbf6dc780268bbfa07b69" },
                { "rm", "40b37c0477cf87bef547493164e453a92c3b759d14a624aa2a70c83ebe58eb47b3aa0a484e57c0f2a540f1455360b35cde5dc5691a74bcd9e6ec6768b0942cd1" },
                { "ro", "9b843bcf40168afd942b49b092ac72b6b33c612074e68fae85d5a8319b2ed039620ab17fb7e53db56ff28e0a04004c23a717158e35ee46fe2cddc8e285b9e6aa" },
                { "ru", "054ae951903d5c5ea043f1314be7378db7510e8cbc58262c9047c0ac0ebfb059e3203c617815de70cc1a097881e82de8cce97273766aec7961c7157614d4bc97" },
                { "si", "d71f928971122e3c82fd8285a2a8a851e36890babebfa6b44d3ad537fbbc0f79e653930c3ae3ae36a9b312119e702aa68a877495e1633f31ef73542a88d2a73d" },
                { "sk", "90f7292b07ae67450ea18e36f5878a6fe5acf2109f65f43378986b1633d9f2ceb0dd2da05b3bb52856412c7a4a1ff54a240c99c0d950df167f1ff2b7c65cf129" },
                { "sl", "60be9970a2f872379d6cb7e9e9f59e092b8f5ec3f0e51daa73479752473c0c5e264d43ce8ce20069efbc7d46fc5ed6694ec8fbe4c844041e3f621a31c9b7904c" },
                { "sq", "bb9019e3bd7d07dbf225fb5ed76211755e79e140a38257331c9b039a0a65c55349f6d8012305a26f5a1baa20ee881cdfd7717ebc072f11331612de03746caee3" },
                { "sr", "943f80ac285e98cbb78e4539d637acc6caf69df3c263343a811599485c028cd7d20281da64760cb7059f942a463856070b5498f21791cff7beb39d87579a0d9f" },
                { "sv-SE", "7687bd242bf2af136d5ec019c22b6543ba7d953b931868741546dbdbd9647ffb34773aa22c86a7d2aea50618efd3c2acbbbb7aaaef8226e8fe7faa82d3bbe2d5" },
                { "th", "6d2c718d813de8c474b8b6756143eca08ec856f9556480a5ac1a70128dba9502960cc1a3ed8e4839a2955e01551458b6554b4304f3d82f8674c9734be866c139" },
                { "tr", "bee44eab5ea9d04bdbdd25c657d253445fa1d680ac3e76c0dc7864464b86ba6f07fdca98a5ab657f22f6ce8d61e89717a973993f7df848aa5927c777671ae213" },
                { "uk", "6480aa82c2ffa0ab4d9f8959effc18a621dc3933cb116cc94a8c128e5a1a7fd770d60c2ad465c81dd0dbd0edac4b340207224e068ec52713a86a0534ad766b9d" },
                { "uz", "ebcdba747005529bc71b889d2fd17d03f3241d87411e47252a9412967abb0ddc452cb6ca1820b7e473404a285502d4eb4fdc598f8dc24798f4afe72bb791a8a0" },
                { "vi", "016593e2394365517be9bcbbfcc53a1ccaa7a5c7a0361d7da79b85d81a474cec9d11c40bf44585432b812d194b3f9106c5af3db43d74efb4db85bb385f05597a" },
                { "zh-CN", "27d19ff7b33282cb69b1223f423916c6d495c728f6de2513f5c4af12d4fe9fda05b1e472fe13ecf0c7b81402aa07b6f259ee68ddff22df8c818b624d40850e02" },
                { "zh-TW", "584246eaf5c5d2e774a58a3684e98c46d03b7e40d7573bfc618c582e27f4f4676f063529571b96cb2ad83bab6bcd9b4a586467dd659c893536742024f4c94686" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "eb05214c8697652cf40e25d0dc1cf24243ff1099a28f3ade86fa8e013cc845fcff2c5727036940c5730595808fde5eb7cb086497c06f29db9dd72f5a032e2a0d" },
                { "ar", "07a5646c1499f6e58cd3c254d89c4b8d8f2eabf8ae928e43c4fd76b0121b719e4dba54d613efba78f2015eef4771671d313425cb3c782863601623e9288672c5" },
                { "ast", "e4721e3adfd2caa150c7d8337a4f177732eabc9a0d7c7ef2cea10c72f3b1cd6d98e0d068a02f519974b2f8c6efa585d4d19cd95b0bd4b26ef99c2926517873bf" },
                { "be", "616a26447444f6281b26e2c84fe070ffa0c5b2fb742305708c19eeefce6def5de8a9453d774fb64ada8cc056a22d1baf0bfb4a29455d674de5dfab98981203d8" },
                { "bg", "7ba053c2fcc6c4dcabb352f446d12c28d99fc6265dcaaf5392e7a5bc711240bdaee1569eca1ea573a956d9716e19eb5598e8a52709e892d13dc8ffd703c794e7" },
                { "br", "48f7579a0f1871311010a9e2bafdbed50ab56f84d2aabaaadf7b7cdf93285b70a4ea0a27fe267cd4c180443f6ba24683f4da9500f75e366c7ac8cdb56f1e7892" },
                { "ca", "130c9f8a9d5f980a50bcbfdb63ca9ff0d99d29c6a13afe2f5d578d300af6232bfef3f1ee5da4367f7a4db04a23a5f1ff5f5a24aa306441a2038210ad65e8c92b" },
                { "cak", "3aff4050da448e482473867e5a193f44b95f35068083b22090d90e2c2f4a4b348714ef19d6c6cf221bb0a574f42f0d2055bf954f6f32f49d9379289950a22902" },
                { "cs", "002ffe3c82109d5e3de31d8f3766bcaeafa255877fca775e72b0b446a182cf6652070ea8ea024a243b501f995f2cd19547f25cc132eacfc7b3302bbb8e3a02b9" },
                { "cy", "3bab5f1cb3d05aaf14298cb74d7049aab25445eaf057674f24b5db97ccfc46a92d4a288ebb6d8671083788876736b53872f1171993e0b52c55fe39312df22c24" },
                { "da", "6e4dad54d0a1b415b4e17ecf4709853aaeb1424faaee53eac192300c9de43686d3d1e41e8474c193da16a470f786dc094eec3cd84d89bcd91bb9993bf23a00b7" },
                { "de", "f7a1be396a5cf590ce27f8aa6d408060084900adc2165b3301a694dbf123b1c84ef1c8abec7ea6b1dbf68ff4c4515637006cddbdfbaeef5dd1b2a703599722b3" },
                { "dsb", "eabc104c37299111fe7ede57215fce016d0a9e43c99e3da69e42e18635ebb8e7240d9ff4191f5a1d62ec26a708bc6a4a9675061a057d4a679f56167b3978b83a" },
                { "el", "d60b06a50009e3636ba326a85740dc4b69bd8c27cf7b86a4c3f0ac1a2a8564e195ad59223ba2749df313c2a44690eda481a079262a7aee91b34fadaa5669e6c5" },
                { "en-CA", "34da158c89d2213f9b5c71154e87e86c59aa180c7a8e5a7e2d962f01d510cbac33a80bea8b5a3ad1e24c73662e468267d76c69504985a66b27995318c9fb5170" },
                { "en-GB", "1428e70c95dcd7cfacce575624770a0ebd5d74bca2939394c1c61b4a933e40cf06a170e360bb4a49f09a8607151ca6c37e72ff85891cdd1bc3e80387f2fcfb3c" },
                { "en-US", "1fe8d0e64676dbfa1ebe768ae37896062400f5302b3e18f93a046502b27af11ed6beb25915711a36524ca275454d046ea458666dac0df71f53227739c186b544" },
                { "es-AR", "c7e1e2cdd28842aea498c2161866d008c6ace714f05f4798d520fd4bf69424faefc46cb05b00b40cfc8d87d3d8b655be03c43e9a3d2d9a9438b6db9ca77c3647" },
                { "es-ES", "7afafa945c1fc15e3050ec04514bff9c0e210cae95fe82e9536252ee7e0af6ca14f10c83fd7ebd91c7d3b15869c37f87227d7284ef0e17a175ef94f841e62df1" },
                { "et", "86797c8d467c0b1e048d9b3fb73a26ac78a2d0354e831264b3daee77b10199ccac4682eeb00a95f7cf95bc630724d039c7a3a0c23bb8b159693bc0086c73445b" },
                { "eu", "cc16230ffa8b9bb87225d61fb79dbd1bac4db3641f743bc31ea80b40d77ed999436e745202da7e352e6aca837f3da31f4b8983b537fa6a2b5eac348cf8e097b8" },
                { "fa", "36a871873d718d6710c659d24e2bb7424085ee8e0a98753d543198797184a4b38e41275468b9e0ee4546ff5e99f0d037ff9456e8e4944d98f978a35986940997" },
                { "fi", "1c97666bd2d0d9911a911c4a6d4304b393d3f39d578097311630c7a6e61db9fd3aa9fb2264381b6cc01f41f14108cef39da2c8030c72cebe6492ff32b6d24ee0" },
                { "fr", "a36c75268ba6ae29b2d541961d3dcdb67ee90e81b71ddbbfa70497b449c7a5455d30bdb86357037ce5fe3eff997614a8f955eace015641c3103dc12ef0e2c99e" },
                { "fy-NL", "20261c8712a693999578fb31cbdaba8fdf8726237525d46394f7e05e76bcc89ffc6caf49a78da251c65b0093bab259b31b326e3370f595e82ea70b5169d81521" },
                { "ga-IE", "05e479c258d4de3c9330ec5d4fe61af66f844bb58b730eea0efb163581d935d6f408de8f978d1c14c421f95de7185ed8a3dfbf46e284c4d7e83fdc24bde3d62f" },
                { "gd", "63ebed8a9b9ca15018a3c67f2eba4c73a61c86535c379cfa1abde2e8ca3aa9b858824837762330f840d3a922365315f48f66463d634513fe4a5e1b2178738305" },
                { "gl", "608a5308acd9adb38e794e39cb2c612c5aef1b1511cc6167ca63e33b8a7226623e1d61d43fb0ed09419683395abe384ddd7aee31ac3edd0503b96a45080523bf" },
                { "he", "58766409512c99cf30968c03db5aa30ed8bb1612d409d5a0216b22b91ae002bcccbbf31f3caef814b90597c5da102de1855ce7eaabaa79c72f2b5d38aa0004f7" },
                { "hr", "182a81f4dd2d21bbd5caea2da825f8cf31e66c5a3ca4f3bc5431d6f627b39f1e362d9f90dc50e1ec9b8fc13c594e06369d6c87e65925dcaaa18585de010b8a68" },
                { "hsb", "3e0c650ab3a21f78dc583227bc72803d00326d0c1cb727124c4f30354a17b361c0446a621afe487bbbda9005295f209b84419acfffd7d7b7a5869dbca19b2dc9" },
                { "hu", "53ab54bbbc28e56003f98140d58cd9e45c748d099ab8caf0fa72838755ac5470c7df489c153aa5272c20545fadd8a05544135d161e1ac460ba182da1b81ca289" },
                { "hy-AM", "95dbf118441f6599787a0c8864d67941ebe58e59173335601306b347a994e01d2f246abc746f51f144eb120dd907da3b6686942ff96e5c28c91e7bc33c3db034" },
                { "id", "89c8bfe445a0c906c5e6d930491a58a7fc6190b139fe71d741902f81f63ec5690a89ec1bd9fe009ab05e31e7638f60bc0a8b015a3d78e9a68df90ff019b3c0cd" },
                { "is", "7bba0d373cd9440f792a25d3efce7e5020c00eaf871aad8312d596b1b1b3436bbaae134de9c0d83155be1451d5e3a0c66743adf1a82b1986f9996ed0f758ad69" },
                { "it", "957b7a7af9275d28619cb33da992f43f9e05f7f2e184211b2988b4850ae5e02bb30ce3c207eccf928619bb8131def447049ca3a8bee61f6a85241375d70cf3ff" },
                { "ja", "1b87d91db12fc7d4486f5f2bab34fb5ce29e4865d4ba9d9b3a0d51df0e003961373d7bf036867dadabd46923f1de8fd2bd7d49eb4917674db461b9224fd3174e" },
                { "ka", "8b4f3818c7212554d6156c8108628cc0816bcdf929cd9ee2a4e335a4e425297997e085dfc644f7202df9fceb35577a13cb583ac8ae5d52fb7a9e8dddc4b8eeb0" },
                { "kab", "3ed64c8b96eb9189cb57bbba3e07db66ab2ac065ba88de43a71f9a6b1a08216743c7932f1be0e962d5074aa929c97bcf41ab844a746f1bd10ce8b8201ca90408" },
                { "kk", "0f2dce02f6f2cc67ef60c9d61b20206a509a1cf6cff761cd1834101d0e693c8382a029b439b816ac074da239a62a427a331701a588045fed503fb1262a5f118b" },
                { "ko", "bd5bfc91ae9b1fb2b6f465eb8772a028f6093718874cae9a2f21eb3022cd536f7dbe07dc7b2a570ad122d1ff114046d9bbc3cd47e77ce0af0a2b9255360c7ba3" },
                { "lt", "0d22be388847a51acda30777cf95a00dfe21d47644a1b9d100bffc9da6f8edbbf1d5808cc8223dc1ad1f41ef0e1fc80ba4008cd3c91da8a55c0553a827fae8d1" },
                { "ms", "a91800ed0aa07cb11fe4f5753ff36d1dad6f27b11fe8171dfc167d1c76698307241936b4540921e2d77e8701e52e054c66241ee93af9b513a6bf8b554bab16a4" },
                { "nb-NO", "92e84ff200bc1e1c8cd040377b404efab9c804d5b4d2767889e1224358bbbe89a36f0d5581e0ce12e49e96e6288692d8f43d50c404ca922675f69fa048c66147" },
                { "nl", "f7cef56186d7587542999ff05923cb70b0cd61bdc1de9275a4bdb0f8b40655c95bec288516d3742c9dba5866f344dec4025cde48a439e0c3395926830bea024b" },
                { "nn-NO", "cf87a593c5199c9940d0c998b0842c1f89e1ec20061e02dea4030ab7688ff80bda93b5d730bdf6c24e0cc6649b73d4cf2d1722f38d9a7ae4d877b1be86a92385" },
                { "pa-IN", "b445c7ed0eca3be984665514a6aeda1738bbdf535bba733122e3c53fc80c1cde4b5bbeefc9ef87a4bcd358a696fdaf36af68e3d8e3b278a06bd71d3636c5b948" },
                { "pl", "3a14ce8d7149935aa15285c8d564dcf681d7d22351ec9cf9a7511a8dc6ab015b1adfa54ba190c7f252d1599cdbcc96b1bd1ea0187a9922da48ec5e193e602c5d" },
                { "pt-BR", "5181e622f8d07bd2525d75cea346547352d8857daca1500a7ce9a54358f23517a881224688bb9199034a719299810f421766c11cd59b56b4de259aad69e4759b" },
                { "pt-PT", "19d1a10aa33bf98408a1cf6f8d9a11751aeec6969cb641139a0581865e7e84c093461ff656f3f383989a997cfc5047dab87f888ddd5b63edc59211c50443f107" },
                { "rm", "02f9a4f5f8c7cc26fa5d3a59d007fa683dbae0a0f30d275d2fd70889fb03b8cc9e967bb73301d04a4695759978d7ed48da96e7c68ecab90f203ea6e83bd758f8" },
                { "ro", "8f981350bec49ae12a3aa714c9f25e94c86e08788ed25af912987cbf0ff118b52500ea455a925582a771b4b279f7776aa72d0f897296539fb0921fa24bae4587" },
                { "ru", "f21aa44f2fd3380a720a8941bf1bd6b67237c0f79048fbb3083383049a975d49e647032b77c764a3a1862d36c5985cfc4b0ddd15baf603b7670b50db709ca9e7" },
                { "si", "e3025d24f39529b76ef11c68ba39f8e9a573a1e3f409c571c9d9c0faac3617f7af4c3850118b8709f3551ad8bfc49b94324b2196d50928c12a99b8c468115503" },
                { "sk", "863509b70199806b4ce7b775df838e917c0b52b2e6d96e835385b25712f436741e46370921416e4e4774b8e436cc9f13eb5e605dfbf8e74b0fc9d647a14d2c05" },
                { "sl", "98679baa89c912fe90c67f75060bc9bd7cd22062b7d6c7c675c4515247f52dfef68bac81640bf7802e338905ea77f13d2c4e63c868a510134c946323cb8d25b6" },
                { "sq", "d0659b8b2fa2818ca893296d4c8161c212882a06faf2efb6387cba818954adb221e2ae2451e6a5174fdb0c16cf430a7bcdb9f40c7082cc6e07f6f4a82becc8ad" },
                { "sr", "7b1870476b3e7fcf3399f77b7bb8308319f8839429f18c2ee59ee85154d9f329159a8e93f4ffdec53541d3e93b94dfc872e9695e78dda72ebfb2d7ba86841e76" },
                { "sv-SE", "31f658fd4c20ab3b1b56c88485bbb33c29249ca2ce6b445ffb3c9a4d88ee689e4edf74fa83c111e29a5073e2420f5faae7d42d3037035e967dc84f735a2f2f49" },
                { "th", "5b4244aa161c8597d18aa31894f2c7d5e0c0c73c0aefe01b0215ddd4c7862ae66798385bd3bc1a9145bea75af146d68bf8efaaa11e33a3c48a644fecb4af0ca0" },
                { "tr", "4111d5401e3ad80b0fed715d3f1c27786e7804ccaa99727c36d2a82987ceb2479b653c1e42c3d5c0fe92ac4ab1c724e05544d8cc2793d994bb9d9b40c45f6df4" },
                { "uk", "a517332572215ce47f09b76747223b87a9de5ccbaef344f86bcca81d762f33c7cddeedffc07e67a49b90bc81988e9040f3a96ab877e97182f9bc7b7c37f5c6ce" },
                { "uz", "ad0f9f0b5f6f833ba4e2cf8ca3a782101038d3627f5079585d021daf802ccbfd816bdd4883a82bf1b3c7fc944ce0c90f684fa01f53c9c5af9a0baa3143a2e4e8" },
                { "vi", "14fd3ce412b82ca5b44023daf3eddcc8ad727e00fbb96e4e4e5c28311c24bdf73672886cccac494567e2995bade96e8e611319fd7ba8ba5b79c636ce44f6ce82" },
                { "zh-CN", "b7dde27a2fc468a0f83f0437e879b1c355cba051aadb658b1524c8bfd9041b6f150bd20634ceb0e7f61f9cd3aaae050566d2b67907a38e4e3010a3ab27f43cdd" },
                { "zh-TW", "277feec9a85f02b1a66c46efac4a0901a750e587ae0d54f549eccb0cd6fa39bd4c8f0750dafc720554adb3660ce2aca633b19cacbfd068f8870f022d98f29a69" }
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
            const string version = "78.7.1";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird [0-9]+\\.[0-9]+(\\.[0-9]+)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird [0-9]+\\.[0-9]+(\\.[0-9]+)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    publisherX509,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    publisherX509,
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
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
        /// <returns>Returns a string containing the checksum, if successfull.
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
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using
            // look for line with the correct language code and version
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value.Substring(0, 128),
                matchChecksum64Bit.Value.Substring(0, 128)
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
            logger.Debug("Searching for newer version of Thunderbird (" + languageCode + ")...");
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
        /// <returns>Returns true, if a separate proess returned by
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
