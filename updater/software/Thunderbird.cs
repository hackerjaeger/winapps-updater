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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// currently known newest version
        /// </summary>
        private const string knownVersion = "128.4.2";


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
        /// Gets a dictionary with the known checksums for the 32-bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/128.4.2esr/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "a0acabaade7e04eb49867fed8210285de7b8373d0be53eab199504b6bb66f3ab529c06a04e6fe2633121c83a2b6c66cc8d2a0bf193c9e44c567d2e596a3f1355" },
                { "ar", "1e28dd7729dd487b62d6000c9f7f45b3729146ace72992d142baff2922d97de612a5d2385f22758707e8794ef673d71a73a2972e25d1eedfe0bcc17e5a8482c9" },
                { "ast", "77bd9bdff10bca305f2ca6e7afe8c8dc656b0cbae4efa710ada6e585e88b959e90226b20d09a77cdf96ba39c804c06a0539524898935d8b4d5666e575b426d09" },
                { "be", "2a8ec82341cc8e1bcec1552b90eabc11720791711e0602511c73256f8dc8ed7238e8f888c2f47c888245ff1d82ea9a2c6b180c5de41fd5681b0c37ac4cf08468" },
                { "bg", "09992bb24eca125e558d6e1257d7e0f9f6c4acdf82db0480e1711990cd75ed47dacb41b4d6204edcd84764b28c47689014e6d30b91e36596de94ee0267c17db7" },
                { "br", "09032d5b615e06ed43cfa51b8f29287cdf6551113168ab3f1b29535fd5c80666961e96768befaeb6c9f10b6c694568fc797749a5db3a1283b6fd2e3367e71169" },
                { "ca", "a7529835db39e477b7736127bdca7f236ecdb7b860b6734318acadb06ab020cf9dca780aad560ba780c4e75f9b063f45c696eb8d1383ca3cc9ec92d5dd77ceaa" },
                { "cak", "a30c746e0e312375ccbba81f9bc33129434d942999933d7f8664f805073e5c9f6d1b1fa307674a830b815968e260bd4a7f3d87259703e601a67bf5238b6c74a0" },
                { "cs", "eea59d7c84989756cef60e4ed8136aa747ea32f2b93c49c14b6dcec6267bea45ce6d1bf353650ef2dd1a83ccbac83070ef7d585ac8acaf166402bb5670177f9d" },
                { "cy", "b963d97ad5c6c38dbba62ece948422fd2af7e9b674513a7fc313e316dfe1355a1902ae842abb5a6df8c5aef3173617d40d383faf86e2a632fdc3fae4c86dd66d" },
                { "da", "27c3585808656185ffd2c39e9a2a0ba1ca859b3ba715c4a8f7bffb94c231c60a9ebe0e5fe03286786a30fecf8baea930dc37edf0bbcaf67b15390c71ccc9af22" },
                { "de", "b95bab4d0f5a0e0485da3b5263e955193a6b9a2ad42377f693c88adba44849fa5fe82b3bd153a6be5b0230b128b8d11c92f7dbbf60233451d4f64a8550c049c4" },
                { "dsb", "75fef12a59222f5bfbf34544af56838a21aab15139af758ae708bafb299d0ca0d6d36d59d2fc4e6fbf3e41a43dc5263f3a4c595042e730e8a1f155c591e5fb55" },
                { "el", "b3659c79698e7ed650369863964ae0ab692bab9df917ce167b668db583e2d539e5e2e8c2cbace89a22d47f7c34c1b4f6e15cd71c17fa42a494cb8ff2c8c3a51d" },
                { "en-CA", "2504b2bf2690604382bf1705ff390e69aeb1d86991de4fb5d698f944a0a0bc8f51983c691f08aabe30f6a914ccb4dbbe95265b180367790fca5aac535b4c1568" },
                { "en-GB", "be2d4669f9cb1d3d1016e0fceccd4d0f925007c966002b83947b9478fcb9dd96a74b2bf2ff25f95c2aa4185e0dc0e6017e1f28ed907b7e2ec0073aceae079fba" },
                { "en-US", "2a53b500ee5e2762c8ebc13514ab6111737759afdb85b6110c706a608933827aa82ff3d25611bf2192f0b5849bdc9bf20753a0eeb2f48c7b3cca21cbb067f9e4" },
                { "es-AR", "7658f96c93d5c31e17494411297829df025f76ed82f1490d30ca97d2ae6125944303488b825bd18140e9ffd40eb8cdf6495594c7cde74d024cd2337180bad107" },
                { "es-ES", "1e950589db9b82d76565a8d1ae8e03c1c8306b0390cb9034e709aa4df2f92c7b5149b3690edf29bcfb2d4f3bbe5e5ff4f937016f15ba6fc63b57f7314ca7e787" },
                { "es-MX", "67be733cf29a829c9a9c63780f9e5bd23d8dfaa5620a4071e4d16381e63e0cce4ca8ef52e15b5c75606532b1e64cb15f8b068cd41967750c22ae15bbe2b462d5" },
                { "et", "08ddd70761613a42bfe1fea69bcc1ce17c0e31cfad58607e1a988ad8a36ba26c8929fbe2c3231a3f7dfa4bf8bf33099d291ca86866e41445fb3a74bf54994988" },
                { "eu", "b1d4bf0db5613dd64399d3afd129bc2ee8bee786a942333848b94df02304395f4afbb0a69ef9ea04bb34b0593939ac5aa6cdf55574ef84f5b4c7c0f419bca15f" },
                { "fi", "aa200dbe6cf118b1b6aa6a94e2ef150100504b41a1992914177e76c19763af17a3d054da270118c5819e98f34b79205285f4323c0e77f51ee390ffaa7e8d3f6b" },
                { "fr", "b88bfebb06636dbe39b738ea226c0441a0533925c84d378019b9227044cf833b0a7f258156a85de07950a5ce436383a26d912dc567daa4badd442f1c353ca28a" },
                { "fy-NL", "1814c43315a843054e95231c503b4b76edbb4708bfd837818ef3bf1c59a34bdd025fdfeaf2e900df08b83aeed24214fbdf7fed2c2f2a56877027fa38da864b78" },
                { "ga-IE", "f82be083a661cc1a798cd5f292dace65cb9e87eb46602eb55159c81272577e1d2fb914104513df2f40e2ae67f355975410142fa60277d5f547032c4f3b90964a" },
                { "gd", "0f359b0c4a86d51d9a67a3453a94c40f47227f45e39d572ae3a3157652fac9683953810efd33890843a91cb8a344286df7d470d3871adb19ef39619776820c17" },
                { "gl", "49c7c494523e2414ecdc2e2ebd483a6f7f6b78289f428b0f7002675d95113f6883eaeed112730512dc74301d514016f7ef7b0dff744772a3cdc6713789b1a286" },
                { "he", "1c861b846e4cbaa89538f96f85f70dae464c06d236192ff292c48182c7bcf706aa5cea945690c4dd00b4694d9068fd907d51e77a86b9d06807b89e46e564d04d" },
                { "hr", "1b97d67e00be055ef19af1e505f99d6893466663ad675695684c88c10e3a6e3c8a6f9ff75843dfe06e7a48a7c5dfa02b21e38c4f44de3f72d0830cf3192d7105" },
                { "hsb", "87d66ebe49284f2760fab1658ce4a8eeba0795e869b0de649d8754bf136cbfda3d180f48fa77a841242704b204bb4ee5ffccbef15b04641a712584f9d15a5840" },
                { "hu", "b1ac2ca796f66aa6ec8a19e17dc35ce1f585f0fe208270078c7092c917cdae95fa27d042b370e78a4111dfa1578d4be4cf7f512864aea1fec7d09c5ad74aad64" },
                { "hy-AM", "382af710833a38ec99ef812be3ed13baaff0e7b479b1e41c056de31e60d8982d7aeaf1ab7e6f7e9b18ecc92fccbfbe8376af9c6b79279d9f0dc578c8f1dc87b9" },
                { "id", "389c3cf35b193de545b118f148825387d86161bc717deefeb05d53548866d4221202ba331d00de854a6eddf7549790c314cec42461b45e29382e480656d957c3" },
                { "is", "ffedca78e90ea98b692ce9e9df8913a53b1d6f492ead10707b9aecf35e7dee7dcf3b37539c60bb407c3f8b1e9e83bf7caea07a1593e52043b8adf441409c3cd4" },
                { "it", "dcbb9eab4f713ba295417bb9e3962f9e193908c57981f6384940271a44aa66b28fa48e8430b081f3f765ee618b9fa4dd0669212b35fe2c621cbc582fe69a7d56" },
                { "ja", "7c168023ee473fa3eaf89b3068f406c184123e9ff0becb118f2c3c607037d975f354ec2560f422d6749e70753bea49bd1f8cfc327a520c4e392689d4a6b976ea" },
                { "ka", "8e4e50b630edafb199c4dd1d06fd064e4e75f00164784414c74454ed7c443e837d75b2a98622db565122c84adfa5d4399a4b7b836530d20b8c223828f5b73fda" },
                { "kab", "51a7b8033f224aeb4b826dace1dc97c7349eb7c2c654b36522f3ba4232f66320191feec57f29c562a57dd0234e0f11f7a59a915212025c8c94f4387b37fff8ba" },
                { "kk", "efc56c21bb82c72a2574cf9aa5b1f8eca79ae887065a1201b940ea10f3668c87f3e1ff3fe803b787a5fbae32183a14cc3088d0100544232af5dc458fde767df9" },
                { "ko", "b087a33c0945f44dd7807dec1612e85f19d1e851a8c96b8f7434be3687ea4b77faea912b4b6fd5e32d859f5e09ecc16b530a2a9b7522598d87ba497d01fbc5a1" },
                { "lt", "a8ef5cc12386a320ad5c65b0d3628843a7e05804e1b1dee263de021010f489d7c77aa473ef0961b8b0817648d8f02cc3221d15daf73bc8bbefb18b3c823b2cc3" },
                { "lv", "26cf284a41e1df1fd50805112df88edad3b323ce5bfe88d5ebd563c25ac7d5f22a46fb204379e23e008c5248b3a367c919597123a8284243a29316f0b6cd44c3" },
                { "ms", "c160330270277d517ebdd284339f3eb2ade2c8205d56e82137f98769a67e87454f7f9bb8fc5199413f3ce5a660aa7b40850ac92b9456a1a9531007af57c02abc" },
                { "nb-NO", "2791050f9c8ceac3a17817429a6735db318a311a6a84a2fca10dd5d033b5aa6a8e0786660a5e8d991517f99878d72d79f9052fbe4d92ca80c8c5a908d1b9ce27" },
                { "nl", "41045d8206654ad3227cd04419403f8f90fd220170ac9b3015efb5aeaadaaa2fb74a9fc0d59924c31133e93d714662271e7d3357a7283ee8163a26cd16a8fa3d" },
                { "nn-NO", "483039487705c9f3f446407166539092832f826d6dc9f72d25e799d0c2598345a027fa88f562baf60334bb158b47a3be95db062352eb20867192393520d4c349" },
                { "pa-IN", "83e4ac318056b5503b6b341c27b172c2c745b8e40b831e9884d17daf8f5ebf05661390c7c44d9bd6bc54eb58269c2f8d4a7f4aeab35bf1761fe238f8bd4933ad" },
                { "pl", "481ae59be0196968dbe827134afd129b9ea65b440772f300f7699b12ae8759aeb0af614154ffca66a5033f18c7db84561f247f439d886898285057eb3c99576a" },
                { "pt-BR", "6e8914c3de196b92892fc1cfe454bf9997b045c5c94b5386f3a598598a45844bff38d64ead7317062c0b2c17f25d9f69ccfdbfa04ac755a9e2bfa04aeea8e6d5" },
                { "pt-PT", "0316a2b0ebe8d7c138e60ea33d43fed075eb2334d024f219393702513788655b60a46668cf3f0e1b5d407ae75ab7965c25ef72777d937b2b5f7b3c60b6d8548b" },
                { "rm", "400543d9ac9eb4f9ecd1bdfbd94341d86aeb7fee59c96562b13c1fbc986fc8a0007a871498a6c4e9d24a6aa621f76d79fc540b6f59ad8f9744c9845a9ef272bc" },
                { "ro", "88cf41433cf53e4924003ef44b52180ae33c5d1e7ad300e899c30457846e3f8587490c62ee8ec11fe99c20d5aa650939811ea133074d8697df145cb74f8411ac" },
                { "ru", "01a2b2d91a2654579e4d82793a747046ea8b74eafd70e9642bf0a43706d85d5c60ed484d04a4c2648be08b5a72b44b7916beff7417b20a76e38e4f024d034a62" },
                { "sk", "bbde3582fa91f14a88969968aa4b7dd1bbd515170df4bed9657fb1c0e55ce11dc4ddf896295defae65ceb6a23fcd506420922cc6c7e3f14a6de8855818706985" },
                { "sl", "3dd2f919af95b8cbf1f9254c10d557f5b1438b1259ea5da766d43d9acec4e6763726678f0113455c5c3e119c0af6a5267c0c57cbe0eb45b2f280c6aaa1c062e1" },
                { "sq", "9b59e4d64f08bce138ea77664332d900a7ca9aa4c8172ccdce2df0e367e82e8c25e888aca9fe4611b4a89f807cfdcbc7626ecca757da10f7084c692b2850662c" },
                { "sr", "cc778b1b4e129f41340b667cf3ab2d2f91f338d1d6cffade76e487ce3e0f6b341d152095a40ace3990c01ad9cf60ca5eb32dfc676c5801e99d9d054adfb4a6e7" },
                { "sv-SE", "1df7d871e462bd669ffe3d8517720e0dbee9dbee6e7f281dcae6ec82b98fd04b6f69b2313e1a05fe61dabfa4239c20d3fada17099c551cd8037ffbfa6180ea77" },
                { "th", "f06bb5046b34ef4d36aba43cae3fed19536a226eb5f3b4f88be914f13fce14e1e700f884ca94ae5b8efa842d594b9f17275ecbb0be640142310766217ae4cd6b" },
                { "tr", "89c4045d34dadf62a13dc12e8371d78d181ea0d8dff41b38a7654de4570a09ef93b306e961a53a326a93f9c9790d2f06eed73f04c7f651d2b51d68ec50847dc1" },
                { "uk", "d1c3f120578156a944393b1f8689e4ca59ebebbfcae470402925e4f59fa0adc9e6898ebe2be8e9d8a0e5df005ac9bcbc2d194e31195ab55cabe7dd12cbdf4e76" },
                { "uz", "0c3e459963b1c65f2b7ac6bd9f1efd0dbda899b1ce510efb2fc7ee16a429220ee0bca8578914c2d40c82c95abcc45523b28e1ee873d1144b72423b5a859bf74f" },
                { "vi", "905ac855cf789e91526e700b43aa1828a06839bacf03412b7417b82574db819518bc77ea5ffb2f441db01404fb6217376146b7fb20a795569a621006fc6557d4" },
                { "zh-CN", "f08c670adbc335dc422041aa50cec52a82dd2378f6f152557cd8533c6299b344da1dc0fc3d9b11eacb35cb983c4ab4abf41ec3134f6769f3f925c95794c1c23a" },
                { "zh-TW", "f02c919e42265795ac7fd7804feffe965b385a7759cc5b70cc0f6df508227c955439771d3221c03ad0280400bdfdbbe1cc090d0301a587c974eb0032ea949339" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64-bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/128.4.2esr/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "477612ba1c7d3f3fc44f90425805aa8e8a2e02447ce1498ed5255b1fd48b39eccf03aa18c396b1dc555bc7779598c401b1500184955ba5e0f45269d994292b35" },
                { "ar", "e8b669d45b2b87bce5b1f4d08d8490cd03d46cc48ac5a2917335f28d7d260664e612f166a496efcf9df530c790f5c89a9a5b5292dbada4d37ff8c6c224689efb" },
                { "ast", "3d3e25bff844675a76ae9f8451ba14e0a5e75dd67d617c96f58d7f53f8cdff367bde58e0f300d49d1daa6ade5ff2ef0aaf0b3ac2501376b61cf99991a5957a46" },
                { "be", "a5c111de5f532cb1ee49e368bb6ad38b1946063d064a55934baa8692bca4a04b3eb7eb7437c1e648b422a7cf4098dc5bed05eba15ee50b9fd94bfd708ab86949" },
                { "bg", "c9b43c4f13e1dabb9425943ccae27d064aa18f174e9697c1dd6360be0d9d79e024a51e69aa49eb2057c34a9217abb16dc6ae6cb49b9d6ea415a44d8eabf31e41" },
                { "br", "7366e789e434a7c224202e97cfb7a654da9f2ac36f5feafc32404c75ad31e138550428f6e44fd1007d764c01b44dcebd77e233321cb7b86fefd46c3c12bf1a6f" },
                { "ca", "a79dca6236ea275e17ff611a3db0a409af4fa5eca25c7927ac1c8080814af0f24d8a13439c986207bb99e2fe977e0d91ac00894b4e1932e92e42ca4045317be8" },
                { "cak", "4cca2420556fd72ff11b9a2270e7a2cf5508af30a34765e58bddbf797f9fa24c4181a119abec608962269da1e81ac8f608d8f49469a21cafed30e5999707b6ce" },
                { "cs", "f7a62e4d719d3bb3e86ffe5904617eb6759a35cd5cf532375e11dc2ce0a94f3d02729d4187a206b7acb0bd928eedff600185d642e38ff89639ab8ff5c333eb7d" },
                { "cy", "9d787da3519e4f14ba1629f65c0d4cedcaf1c90d2d334853bce0c9efdafc53fe5ca2b9247eacc76c9401822d5c083965db718a56ee970b5c3cc759796a1bc53f" },
                { "da", "fafca25c93b7279580d8f96fad2dfd43e441d4f6e9e744ae232ff9f7934b323bb9592016b9526cb756777f960773ef660764c2c6da7f48846010b502cbdca80e" },
                { "de", "a6bb70bd9c6dfd417ef7a42e8fc6afddd01a8e6dc275873e2dfe2a89f12a59967947586706d97c3a789fdb35550ca75f7e68c30f4fb2ba81ea7f9722fe3c3b19" },
                { "dsb", "f5fbf872893197233c01706fc0bcb980308dd2cf14ee26b926f0953b773ff35b49a406b3967fb921b50030df9d97f19518f6220c89e2d39994b52635a41b6a77" },
                { "el", "5ebcdf8b8e9430f9390226557d5eadfd2eb331798d1b24ec63e8fb86f22e208245511cbf23faaa86384390fc400e33ea3459c7c819fa4c4acb5601341e03e809" },
                { "en-CA", "c651656c3ad43dc2a8479cc9dffe9536d1cb1f7369500ebed7438af26a78f099a948cf9ef169a850aecc9a5cec0ea457fa196e12b5404052c5379c04866897d8" },
                { "en-GB", "353f1c3ff24a8c5ff2c17323f29a8f3856b072c9803e3aa03d763c3ff1f88acd327dcd15116fd2b933504250e2ab6706d2406f41334e868cf97c25dd56047c7c" },
                { "en-US", "9f3e11596029b4b1f37c6931822e22065fc9fb17a12020911185d673772953864f97b795ac1e0f26bdc02927f4870c7112ecfa3920563861822ea70b79143ddf" },
                { "es-AR", "9617da499288a615e73e69d47d40d2e4b0415028ee5d798ac9407139bafe8d87e0d4cd67255052cc044f2598e57b596a00531b345a157d593775b4a2284f6fe2" },
                { "es-ES", "52a72824b775e2309f4da3870438afd1a0c353e973ecf930ec7078324eda343c71847eae003a48ba6c60ae83d20005a1b3b4f217c2e6646f281415d7a2d6bdde" },
                { "es-MX", "2e758c0fcb656b8f44dab28112e3790173ad9c9d4a19dd50f7a665621a25c8bb4d9e0e35d6d11b5ff54399528f7aa3f575ba84651f890c9bd1df8993ab5917cb" },
                { "et", "f6d69291a3c5293e0ee1a5426b670ac02eff41d101d39aed0d046bf072f34b4b294a9cf6f3e9f2832cc3a201a0f29a483cf6c00199abc6a22f23f99e16551d73" },
                { "eu", "50220251685ef252ac316dfb69f54f77414c4cad139751b79d86f7a3b350ed2cf8286fbda25b32c6ceaef9c5a4f5664571af0047214ffe626919cc63cfc32187" },
                { "fi", "6b01eccf97232838eb073e64a0942a6e1b23149498d09558278dba3e36b20c017fbe9bdfd9ee5bd2c367a3be73ca86d83030a54ec3a9633ea6152daeaf938af2" },
                { "fr", "5e39067830e078fe6ef351091c7288fbab21df598fd59f1324a30c0c0daa1f6502c04875af7a512e1e59eebb90f0b84c1db342939498784804ab6bf44ee29b5b" },
                { "fy-NL", "e5b6abbeaf8efb3680cb41bf0a98e664ec845c2d002b7a70cfd62916224d7a36514ff6077089127b1ba631cba20deebe1e74c4ad8403fb1d2c562e298cae006e" },
                { "ga-IE", "5e85445d1e110347b40a372494142470b6b5850f0b082ca4249ca565959c1e2ed5e15e4b9763ec0f26564028cc0ac1ab521169a020447682c174528af1d8a0a6" },
                { "gd", "1676bcd0e333bd5515f9dd35fe8ee51649cca2974f2a17a505b1645bc672b78fb4093f117a7082270f6b83f4277a23e8f0f51c20e508b6409ac678e04957c281" },
                { "gl", "b7553aabc6526a941e0bad66cd2310966aa5c7378889f8f55279cee4e64937da4527861d716b31e3795edb4b9d4237bf4d7d1191f840f63af5d0b2347f363370" },
                { "he", "be764399caf74ab88a90fb7dd63272178725fb99e41b585e68dddb03d6fbc065720861c28653a6d3c420772a645704ea3d4dbd19501dce6d463eee31958bf9ad" },
                { "hr", "95de5814d8e3a21f832fe2c4a318fd37a9de8de778182f0265d2dd8c163119e5f8d418b20f26607fdcd839d843c02bf944afaeda685e4ce46a6b6adeacb26898" },
                { "hsb", "76973fca0d85aef13894763ca9fcc1ddf79107d68de4c97ca5f27e052b90cad09dd040c65153b63548c2c34d9a8e47daca02ed68b035c3f19570b24b8d787f7d" },
                { "hu", "ff3af071a82c5f12db481a8c6918e696590398385ce6dbd375f6259f7d9425bdafef31aefa9ed1e6cf3ba55945aff04d4f5c1feb2257f09d1c91457a86154293" },
                { "hy-AM", "0a07e78732624c6db5bae984f169a4dd6e9fdb08536b5fb0b9ef02a3c698352c262c745da3c1093605a5877b276740b9fe842eba5fef566cf7d349b3e3efe8f6" },
                { "id", "60ddb75253b120c01079bdf430c549a96f4de8db3ae8442cddb181abf30182fe60e56848254e066a59ccec628b7ac80fdf581fbbd91a02902bd966ba94659e7f" },
                { "is", "318c9b4f4fb13685d1f61916f82e4c3168e4be857a0ffc9b9f41b2366f5dae1dcd5807eee6702e8a4db79a8ee427401a56637e927feed6dd6dde372781312a05" },
                { "it", "3a958d90c7375673bf6827ba6f7eff44dccaf389b790e4392d86ed669504534c47d74deb2bd1150f2d9f332dd72b30bdfbd20c64aa978f7eeea20663e34c9143" },
                { "ja", "6f03d73cc6f472849ebb7a0116a4b3563f9d3c180b113db4d91943085153e0376932178bb59728e430629607acf26363da5c1b3d9df7e4d8c7a9da3ae826e33c" },
                { "ka", "825579c84a08bd95845f001211dd07ceff850399b984e43fb568dbf1ad3777ca520813196d9c6b71f2e0c0608a0192419fd8a5e878fa35ce879dd9f89df8d10b" },
                { "kab", "407a3bdb597369cd9b54d105e7da684a1dbea684588d99e5f9a82dd777036b76560476ee3258602712c2757dddfdae592422c584bd0e283d020931b40ffd7c0d" },
                { "kk", "a62865eb1693ce88af8e1945a2472690356135e3f7ddffdd9fc9d45fbaafddf2653fed5f47c2c790d215499a24412c9acac7346e7f80907f0e9a0a98aa969cd5" },
                { "ko", "7c7255213ec2cc1056edf2db7edcc682d8e18f98a5b6e9cbdcb2600cce92d8ca3b48dac7992dea3b217cf25dddad182be00708b95ad91c1af2c50b5b653a1e7b" },
                { "lt", "c6f06dd8f9be724229015797e5fd0c069e25e02c83b51bdb1dc3d4101c4881f2bf2eae93b9ad6ab6f31a7700896d2370aa9237191c8c312972bf8cb086943710" },
                { "lv", "054c0bbda3a2b2d8f9c5413f6da68df6af81d5e29f423eb6538d9a5622e11e657c371deab899d22f6c149880e2ff3161568ede37888bfef44cd9a97acc865527" },
                { "ms", "17e8d159998bb409744094a91ea428400ea936ab3a45881ed77bee8e3f7338fa2140a462a203f067d56db0a0fa672a8e1bf2abc7ba566dfc33e3e0a68ab6dcd8" },
                { "nb-NO", "8f7421128652c72cfc6404ac7327a9c7a5645001e6aa5fc9727ed12edfd7042c853dd3814cea5018ac7fdb0fa211567caa587c474c3cbf28a93b0aa003d4d7d7" },
                { "nl", "b2a8728efe4581525798ff361f930259b993d282a0dcd8df4a97d468d057d37e6f19087f8c3be0ba2e240ef4d566c09da858ebfd2d7e381775219c79ca58f4c3" },
                { "nn-NO", "1a96034548c01227e3f68af3ba7c2e9ead0b73f12b873b13661d29cfc314eec98a5e2d1861deddc8421a58f878aed6ee879522a38622e25747b361ca6f8a0c43" },
                { "pa-IN", "1731a19c1059070b7b281d8b9e3cdb4389196c45294eb5cd949058df46f0eefa34f8b970d44639329884788c8f96205841557fcffc9cfa25eff606ef8e11bc15" },
                { "pl", "0c0aaca723f0039c99ab204b45f146cb2a9a59f61983615ab1235e3a44bcc418dae0bbe857a935082dd3a5fd083e400c34d99544dcaf718f506e86b7ef351e73" },
                { "pt-BR", "e6a394978aff638cd96103c59e105e7ae052be11a288310a7a1a711b919434069f469d79fb0986428902d4133463d1b8851661aa0d6fd96ab403bec3c6412b08" },
                { "pt-PT", "1d7359ff85dd80723fd39c935dc21c87fe0b39c0e27ae03cdf536964ac48c1899da83eed0e4ee5650babe58abd40eb3c0f23054715dedc04738966fe71cecddf" },
                { "rm", "98264ab64b507ee11999eadd54fe3b5248d4a1ae5c3024ffd32824ec96567b93caf14201e12475155c940f1690861c9ef9451a914600788c25bd0235945f77d9" },
                { "ro", "3a4217383fba4a3a0318fbcf5302ff5631bea4311e054c8a182e6ecff9c5be4cd76f4a0227ba52552f24454a9cb017a48201938997def747a8b252654324c169" },
                { "ru", "f233861fda28d6ddfb19fb8505562a9f7a093dbb4633c67c4c536044052a56752ccc57a69c8161199d75060130847bb2bcff1232a9bc3e4fba84404040383eee" },
                { "sk", "250c6d3b916835fb43dea7aa67e16af81503fc0b06715768b311ba59f36975be3abb5e2759e764720deee65edccfa6a27ed2faecb45523543beab81ffaa3594d" },
                { "sl", "b53ee669f9d4251dfe47422878c09f7b6ceadbb3d8b375fe722b5a5588915f2e185d37b4a089e98dec2b949cd62bd1bc2035b1d4307990646abebc8fdfc0911c" },
                { "sq", "d28b2a214055584a22974aa9265cde5e17e638f6a652368343ce96e9eb35540189c8ad05d08f0861cc84bf4b754eb853a5516bbec51c6b7f8f77b63166b0cbef" },
                { "sr", "41a4279b030a404aedf76b3cd7830827dcf1dd7fc2cd17774798f63ca6a9566a95ba5b7ee7d78391df441008dc264cf115f01ef908ad5cfd6df861a6667f060e" },
                { "sv-SE", "d9cd9b3e47697e7776e167b8467450d13683b9d4195be3d924c0014794ff3da2d77cf505bf4a8d05e4c8dc5b0d2ba61ff76b5ae160f88093e9d8927f0fcd7ddf" },
                { "th", "e0962ef07c0bf0046207241296c61085d4e6c5e4fddba5480e3a9bc683b8203d873692bd64775c8de01a6547340b85d804d087c2bacb824b916e3a377680ebfa" },
                { "tr", "2a9ce0e3c84308d76b383ef8cd54629e36cb6f7cc4eb20264b57db313bb30d5bd795c4dd12959e1bd8fef4272a04d4eadbb509be7d6675fb55a0c873fd7d6b02" },
                { "uk", "6fae61d69c1086f8f70645bfbc54f1bf0330ade5e3714597a8ba2e0f2511724a41e3c0115a5eb2aeed477d59a750fe6d50bc67587f82d0d35585422896737311" },
                { "uz", "783dc16747f8ed6d7c98b2f82a70e3118b954734183953ffbb506fd5c935993b2b9a0ffb0d2c2175a2311cc736219a59f8f97345842dd2409d890d106103e55b" },
                { "vi", "7b5e116f5be134797e80a1f3f27d88341934dc93710c87ca72ecfa30233627779b3e0863e967a5bf5dea0e99ba14e4203b1ca156bd4dc99b6badc460080cedb9" },
                { "zh-CN", "96943dc3ed4a23d44ef91aa3d784e180e492262dc618f2706e7d141b2d30290ecac285410afef4ed1237dbd01feec20c8359945100d064371b9b58856591d166" },
                { "zh-TW", "d7be067857b51ab90f69a5d8bf5ab117d3a84401129364c807f72ddc6fd5d1c05ea32d346c1bade48543c215cef0111a160d265b53d2c6e9e43e2dde22a81eb0" }
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
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                knownVersion,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + knownVersion + "esr/win32/" + languageCode + "/Thunderbird%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + knownVersion + "esr/win64/" + languageCode + "/Thunderbird%20Setup%20" + knownVersion + "esr.exe",
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
            string url = "https://download.mozilla.org/?product=thunderbird-esr-latest&os=win&lang=" + languageCode;
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
                Triple current = new(currentVersion);
                Triple known = new(knownVersion);
                if (known > current)
                {
                    return knownVersion;
                }

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
             * https://ftp.mozilla.org/pub/thunderbird/releases/128.1.0esr/SHA512SUMS
             * Common lines look like
             * "3881bf28...e2ab  win32/en-GB/Thunderbird Setup 128.1.0esr.exe"
             * for the 32-bit installer, and like
             * "20fd118b...f4a2  win64/en-GB/Thunderbird Setup 128.1.0esr.exe"
             * for the 64-bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "esr/SHA512SUMS";
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
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
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
        /// Indicates whether the method searchForNewer() is implemented.
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
        /// Determines whether a separate process must be run before the update.
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
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
