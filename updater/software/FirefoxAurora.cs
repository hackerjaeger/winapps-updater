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
        private const string currentVersion = "130.0b1";

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
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/130.0b1/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "082d7f8bfb7fa13752d7405f8ac1f3ce04f0fb850b261117751957a7988157440c0cacd5f47864feee46ca517b9e1816e1762266946f390aa4d6c14d9949c4df" },
                { "af", "3ce0fa82af47a6ad20787516fa7323b97b0d0fb41883f6c660426c226ecd24273b7cfa7904ef84d21c8cc474b86985fde5d7a48c5aac6ca0f378d7116ef76228" },
                { "an", "a05111ba5c05b2fcdbcabf8570a4a6ca98979010abc29baaab659ae07fef4743bb59e1c156af4e8b9ffd13d5a3f6fc55e944bdf725a7c4d1a1887b15ffa74075" },
                { "ar", "a939b7bac5739a8efe1e0ba1f05a902609a93f6eeef8abe8c5ed1dfdd2c2f3e5a5e423b30e56fbf3773a6626ef056ce98f176cd26aabc5d1ad79ac8eec66740b" },
                { "ast", "2622d49839c8949c14bd7cd6d88fd2577f86987fa4e2a1305bbc8d3bf603a1bbfa2bba77f779e7e7c85d5950429aa151a806ea73b0a3b1720ee25893400c0a96" },
                { "az", "e0a029b14cb1ef273da0aceea08997cd6c7681159a8c14d9a7518133e015e8c3752c3e0e2b56e8d252305100124f8135fd2a66688f50994b2b2b77d5607a361a" },
                { "be", "5a317bcb7500345eac5497a6628ba29bd91a8c03c50742ca27548c2b68398fa1d3754e6f4ed09eca33af6b67d95f1e21eb5d57533221ffc219d0f437566c1d0f" },
                { "bg", "7840f33bad9e3a726c06c76b2373dc01ed403b5582b08f83da9312c82a2b178e1d3f3590e5b88d805974c326c3ed572d6c2c2e1ff3ce931544582468a9a4ca85" },
                { "bn", "e5607319f887c5b0857be36c8e1bd879d898e43aa725619d422ec41206a5e4309f40f6af638d01a10d39346d8f784e4f700a2cc89987f979d7ae7db3938f8a69" },
                { "br", "6de34b45f4c9a8f9a90885e48231b7da688ec5deb70ffddaac50da07e07bafd1c4c136260a834e604f1ffeaabc5543690979b099bc611d60e76e370ee259cff7" },
                { "bs", "16325061fefea35e007d13834bed40065f5aed483e70d10d71ca634d3ee7e86c31ab0db0a9640c82fdb1fef4dad005234f5de4ab64870c173d41201e74996477" },
                { "ca", "383c05795b340a0079d673ccf8019f82bd6db9433d0b7fee1697f1f0e057df0c15ce1405c098b44f09be1633378d5b6220cf765a1d9c2ab2ee1ec53159784089" },
                { "cak", "aa918711fca63be7de0a5533d4b0f15c4a151b120736dec59a3df27dfe90dc16c0e464bccc5880d0adb034171cca524364a1bde191692c3cd5aef169f24bceaa" },
                { "cs", "277709608630ce34cd7bc7b86821ce3bbdaccd2d0c08021fed9141a33f972578cebeed385b50127b2a26534ee3362e715f0fa1d6863850cb343071a1cf6dfec7" },
                { "cy", "41a9449b485720d176541d649a4826942a31f8c5dbae3b409f11f1dec898cd57a06c6a06cb92612bbe412787d7641b437571a375603ac48d0f7110ef7d010bbf" },
                { "da", "4427e267081b4bea2b4d0cd3f6d1204d1792eab70cf251f26cb61d4c7d7a15d89bd42324f7a7e9b44400f3d852851717a711fb7acb341e005b0a98f1db6954ee" },
                { "de", "7bc0727361baac8c223ac38857bbd83cc311bd18889409eb2cf57e5fe93591078b134fa22275fecbe72e64edffdee3d202e83d94b8528029947444ae7e26496d" },
                { "dsb", "3662af5caa71e5a6ebb8f2f97944c0124c9670413717b4020b4d39308445cd7101389e52d0b374363ee8c771a4c27b74e53dd7ce7565dd3487024f3ec20be07c" },
                { "el", "14fa687e1aa2c90d79591a02fedf48b14b18a5d1f8bafee4691cf98126d3a0c8d285b2d435f2f4348e46d3b7618686b2c10256a9cb621c18488fec76ea6eabda" },
                { "en-CA", "0fc434ebbf648c3ac42599fca1f0e71fb4be9cbe33a919e1f01d00df9001dcca9fa4b124d76dd5a22c643a3da7598e0c396d318fc7c041469d9222d211ad2284" },
                { "en-GB", "37adc4e3146464c195c4a4a86ea49feddcd279f86f1943b3b93b353cbd92edbc7d6779f2540b193ffa705678776e00f4633eec029f0df06abb38d978a117bd9d" },
                { "en-US", "d8d6695275965a848cf67b3398151347c108dd6b404336a62362fd6ab1ee26e95611f489b3f0f5a6cc346aae5b03efc4998459ac64b732d49821162bda641ea8" },
                { "eo", "6b5442dc4169f3ec6ada0c4a120c0895e505c948ae454c99c0521580ede00822ac1ad5722d7d61e9e545fee8c2d827de5722bd24b3ff005b295a0da2cd3c0183" },
                { "es-AR", "73273498cd9d41550dea74388a70a20b473f98ae394b78019b270787fd8c243cb2172ce9a58e9fb5ff2d33840de4ab82e309aef06c7ea6d4288f21ebe5672525" },
                { "es-CL", "b5280c73601c4585a0fca096150d5481cbc610dc8cd2523c768214c67a8d6068233ee2c43c0e6ee411fc24492b6aec77c262bf0acc4f65af1a899b3043bf1f4b" },
                { "es-ES", "66f6b69af161fd44fb63a1f0cd1fd1134ee4126cfba971ae58907fab2b2c1c93b3cc1104e59167d0aa48890a70fb70bee63067494e515c9451bdf155d6ec8244" },
                { "es-MX", "dbd08a92389c6dbde8051afdfcfc370b3a3f8bcb5be5937dd3dc6b2cd80f8b7502b355a1479f2a0cc52ceac60ec3b48cea0fd53c61b499a7214f228d35d20493" },
                { "et", "800f58edc45854c98d31b96c46f5594d85b46d5ad8dc85cc4f64226e4471c14121cd9da8f439db514dbe03c2a4f7c2b7142432ba413d454097c2153cebbdff7b" },
                { "eu", "f51e9e46bd6c2a346602b6e70e4f2527dd4e3e8856a4a668117832c8932ee8ee023590410224fe5aef7c6b3a696787bfef52ea1279603118a491fb95d47b86ed" },
                { "fa", "ed2bc05a4d0bcd77c706253ed2a4cd12bf908646190ff70611305a1e60a7401cf3ae033d1dd264e7129dab9c8966438124e3e6ec0fdc3b20c29f590df21571ed" },
                { "ff", "b3e7784d4ed3012ae7955fed388dc188bd545c2846644e4cd3445d883ebbcab0ccbf8adbaa88bfacad9d8a1eb1c192f56d090aae462c8c8f9def0fec642caf5b" },
                { "fi", "ab6ac4cf059f6db9b09d4b3e5fe45eb017b8ba0ae19adbdc65f957ec24162468c6ea9c1e130fbc7ce0a86606e1a329d2e25d72561a24a5f0bdbb424b81138bdf" },
                { "fr", "d57efbb818503e8a74875efe6d32c860bfdd1ec9c1b3bd3909b63478a945061785975b5af2a9a27e56c24256cbf862e27a48aa03a5de34608b876fbee05cb773" },
                { "fur", "bbcbb097b78f0ab05ce164efe9fa11426e7b728b9a5be0799fef8dbb14213d63af71e94d033ecf71980eae904b8c15ba1da7dfbe3012e0af58ec75cfefccd053" },
                { "fy-NL", "c28b0806a7fa78b26ea30dabfb15a33876c5a137c3bba6c516300a17dc77c213e84387f31afec3f7de0641c787c3b412a87f337a4324d55707d33876a58e62bd" },
                { "ga-IE", "388145c8656c98b9e8c731eeb34d8b621d548f1fcd21ebc8c629f05603abb19de1ef47b0d89793860b6f6a1a78601618e6695eca487ded70bd070b32dd175914" },
                { "gd", "a9bb7e4d99757341275c0fb3f5461c8da58dbab829f083d00559cf64eff603f976741f26a09c852e190e716413457aec930d64f0de5cb5d48dfabf0ab50ea623" },
                { "gl", "5a27e1fc13143c9b15493f0b5308a72f44c8a5fb0ede414393b2e7882bfd57020d2e3f6f35e8d2c8014b6042eff849ccb5b142de44b455176cc31bd521f88b48" },
                { "gn", "b2fbb0f41b70a76e185bf229d77cef9bf04a9be67fea9422955e0e9890fab9d9bd59903b990c90ac5080a3753630cae619fdda27117e12a2a3d3edf702090bb8" },
                { "gu-IN", "762479810fb331870517d2aceb908827718b0c3600baca603a686e9605e585c1522fdef4957fc14b1fa491dd3950dddf73eb1b8dff693cf3c5f80ec480ffb6dd" },
                { "he", "4215b254b945f856ee8c00c624ca5fa81f021a91e6e57903409d29209e4f12bc8388e76b118684c9d96d2ba89b3d937f4bae79fcb5e3bc4e4959dafc738fdf8c" },
                { "hi-IN", "acbfa7a0f00c044cb1de143e489d9f72cf39d700afe8be639c83ee32b5fe7d3ed42c4fc55ef0746a6e496dda512ef345dc1c4483e79e7c24f2da6cbfbbb11320" },
                { "hr", "4bb7a474467d0782aaa06cda0b43b77eced1a157293dfd74794dd3d308db446742bd8db9f7ff6b88d5368c4103351cbec80fc07e4d553bd4b9b655d52f26b921" },
                { "hsb", "3d730558868029088a25d674708fb55893a1af808f29abf4804f3a3e1e8b4ebe2b67f91693bed3272f650d45771125c0f7a5dd3b16e1e156ce120566bd84c55e" },
                { "hu", "5084efcbfbec1f4723b5d539f24de91707274cde0fa58456aad300e46feb822405c9ea493b62243d7f0fa9113e7dd8e37a8037265ea7070d2cb2ea7ee8042590" },
                { "hy-AM", "a18c27911cbd4d46f22d613b7f9d7d67a489af7a25cdd7e9e2b7ea77b3299677a778e5528f14b7d795f714d449d53fd7a1a4897d17836bb6e0e410b5f933ef52" },
                { "ia", "65355f9c2e09cb9002848f749ac7dd23ef17412984015c130cf32bb674aed9a1aebeaab638d6fa281df51db79392277f9e93c4e0cddafd88bee040a8309cd53f" },
                { "id", "842652c36f239fa8a1b922a17c83f3b98c6ee8181c1fdffe5ba7b90e0ecac1f6509a42cbd80890e96ea3fe76aa72c6211e2a8410c01e60352800c99b2398d381" },
                { "is", "632c28d90b675b6090b4f4d3a91abda5d4622ae0412a9245274faa0338a1049c615fd29cda6d537edfa0755f8b401ada09f7ea69b305c6f535aca580d858d672" },
                { "it", "d06121c6184daf084a7ffcd45a221b038e86ea39c297b5756dacaaf073a52c6270c1179c8a55c2ca51ba56b3afa15e1ec47ad42e0430df2c71deecfe6e1ee529" },
                { "ja", "b5ee61107b2e7eba7475afd9387cd19280b70c7bd59d7dea3583bd4762008a42edf6d68e0849dade01cf2d91143a471c0fe6e3a5a8267adf65c35aa973514a0e" },
                { "ka", "9a308c55f7ff439dd133f8c8a987ce7203f733e63f1c939fc8978e91b72c9fdf5dd81bf3228df118388eb2510cd63effd5eb4f8921c711377ca257c2319def69" },
                { "kab", "6a0ef26e09ae23451a08eddb2b6f36a6dbd65cbda57f7855380ffccb4eef839688c420af4fad7d65c5faf99c4f1cf6a99e2524b32c95c0b36c96279c66dc2e17" },
                { "kk", "2ad3840e616ba61505c683ccc877787d550efff98517a55fcd3ea31aa16844abbe21d3e88714a85730feb420391d646c259c07c79df5a2aa15c6d69910501c73" },
                { "km", "97a2f4947029870baac95e6372c577e5811246ec99b41d0e968d64b864d243413385ac057cfe750361322fc5d3daedab0f08f6e90a9538c6528b80817b7fd05c" },
                { "kn", "69d873092890b6ea308aa6b444e890a65bdc57e2a255a92b06e62b8f909c0a438b70db355418b15af24da7ecd49466db7ffe868fb38cb9c13e4d0e2325e7f816" },
                { "ko", "672f410e8a4f11b562b7e95d40d0a7238c43e7026c99665e37846768c7cc498a94644aaab339c7390100993b3e7d9276698f8b51508359e35948748449210afd" },
                { "lij", "34faad9d81e332fe0939538398dfb6d295982cc9fe7c7a38d9d1854c1af4479f6907f0d5e71ada110c2cb56029601f925a092ed9c0c2f8694ab1bc34b40bf27c" },
                { "lt", "226ff5376bafa2057b5fa0ee0cd2d59bdfdc023b8103559166f417a730c3298caebabac46786db257b1d1569f9fb13fb004ddf1a9fdd46f230a1a4c7737f4c49" },
                { "lv", "fb15077381b56d6d5394d46ff3f24c0ce497cad71e52a81340ae10ff603022903a4a0bddf98c89fdfdeba07044ec244d92198f56ba0da460b46e3f7b0b325f60" },
                { "mk", "07715d4fdea0713175708edd4f20476b17199d75acda724fce735eb3b088738989f0789d951df5ee6869daad5f2d3f3ce7ff7eb77a982a5c4a8f65486ef42a9d" },
                { "mr", "1f0c8b17436f2599adc6369e4a9c67060ace04af50a1f1fbf3b66b4083079cd90a0ef1e3c60086888948ab8052581060cf22e51be7c854e17ea896a0092d51a3" },
                { "ms", "45b8a140d13a88b27fc73160bbcf91e0fd67db4ab29addf55d4f3bfc3dd63c0f4d933ac40c8a77ae159929b43f6000622eec2adeb088b19bf48bdb5136ac5dfc" },
                { "my", "12ada34e9ce399de6ec8663ac191386b395e7e86e929e3bf68384996c99ec384cf2f5cd7ae15cae6be7c4567a2f9ee64d91d7622cb78d8e3ca8dda7eb1f68a93" },
                { "nb-NO", "703f923ea6a796a07523793f73637719350cf3ce9146fe9008899457d08c3401d063f9a9e0a4e17011cecbbf1454c3b5093479268f3a462834bf253f3ce53062" },
                { "ne-NP", "7d1dbe2a0396d6314a64eb2bbedc7bc0a2bf6854ce7a248f38e4f7c2f051bebf50620b1c31ddebafb9aae7c9bee3e30dc3af738406920ba5373dac2729d816cc" },
                { "nl", "88b76bf8ff6a99b12f047d058fe1c6b8e5b1cb08933ba73c41e100f8b7cfe1a2f933a61e10880beb715645c2f3df5971a0bc090fd4ce49e150737747cd1b8412" },
                { "nn-NO", "4ae6613f5bdb19cf953a7ea1ce58f10a2d2af1ee9ced4927139ff4c0f5a9b13e5506b8f268fd726024483f988f6917d1e037e2a21fc6a2cdd18b00448e4f92f9" },
                { "oc", "d9a7455f21ae50a4ff123c5af269ed38691ee17e88aa915e2d9d629f18047c9bd0b42dc37f1e0b5d0d2a79ea0652bdaa809d624fe8a8bf94cda1dbfbe95ff2a4" },
                { "pa-IN", "f0d799c248ac129b077785f9e6a166e75595ad0cb2f356cccd42a2d26791ca838d308109a8759da8cdd3049821a138c272ce3454f57631ff91a6bf181eb7b81a" },
                { "pl", "09a68253fdbd8121c472f978bdaa4ff56a7613c46fb499492403f04cc23295f75871c618114808de84031bd958c843dc03bdde32923fb61c2da11c21db1e0018" },
                { "pt-BR", "54b029c66ee8e44ec1ecdd3e14be5e0554405a43111c033672638866a5e17ff832fe809715557d758f3c311596bfc033681b90838733105a0dc178f896ae287c" },
                { "pt-PT", "3741c935c168316a3ff3f7b3e370f5c029cbc2b73a20311bfcb28009ed857167df2bf65412289465a92a6f9cfe63efd118683158b23074d2bb77a4afa4fec0ac" },
                { "rm", "01762a06a8af86acb9710c8ebfe920069e883be80f562d37241637abfe9fe6da8c1ad54652fd1f04dd5f08f1fcb6c3e70b9494acb3791207a0244db2b8020ce5" },
                { "ro", "d24880c6eda08db6289ecd3d50917f657d9a1faae53fef166154ec2e1e833c574c49597c4df83092bdd45a787cad5490a22c10a61d89467b33b44254d7393242" },
                { "ru", "8da4b086c4101072d043b9c19e208ac2a4d01cafe7382e336f5152894a9cd9b9fab99b5433ae927e3f787f13e786d5abcbffce3d81e135656fff6a12ea12602c" },
                { "sat", "d8f264fa65acafbd99571195dc753542dbdd840a564135309dcc05ce54035392d10cca95ddafb26054d978ba7ab7750caa2ce146edb6d2168abc11637f102c6e" },
                { "sc", "1f031cd45ef8cfe3d77792eb9958ca4aebe8aaa7c10889cea749d81a42a0748db807ce196c52fe5cf5975568ac03806d7703dd0223d2b86161e3d3a6874d6b73" },
                { "sco", "cb2d2db6b2aa2f5ba9ab04d0b60216697b781c9ae9c7b3296cd6635cee7a799d2c21dd7bde7af70a8f595d4c8255eb9cb7a9c7228592247adad459e91e27a2d3" },
                { "si", "cd71e436c70c4c14c9d87b42093e8f5b85aceb4cd6e230695c1ab967cef22ee9c72477477cc58e2d51a477f211016bbf56be37ac1805cf81e355a7e277bff96f" },
                { "sk", "d932147bec3c83a87639f3619c0793759dc30992ae0949bc2f8281245fc8ed8740044545c64ef9b383c16d3355339f7ba08cba82b0ae86dae0804a53361bf7b3" },
                { "skr", "a9f6b4b5515bc8a13dcc98ec54e05050a5168dcd30e1029032c24e767a34da6a0009e620e59ffaf86e2b6304bd6206e6b46376a681f4dbade15afd7cbdad345a" },
                { "sl", "f9a0d8ff33407bdfef2cbe49b019207bca1acb4bc3ebdb8c702a4c733ba6450683aa35395e63d7a2f200be67d76fbc775fc8eb9e0498e55e04a4a2463e58c0e6" },
                { "son", "9d1368b23e463e798eb0d268183e5f12f6c75c8505818325d568999cc0f2a62f2214f93141d74ffdc53c9be149c1a83b11d3bca60ad9f7ca8d61fdc2468ed77b" },
                { "sq", "5c95017b920dd55931d75d8b22b996ab92fec704779291828c9d679ba7c7681b3ed18c6c6fa5ca80ae3b8cf77a10fb163fa4c910f9c3a883a283140be2a6ad92" },
                { "sr", "a1a3ba876d17e6eb250070d5116167b7002a761c3d57ba013d789ebeb3f5f832c998ed5a41e6752d23d3affbcb9f8860b262464f3c5497a0478606f596bc6bef" },
                { "sv-SE", "c8da6d84fea39be8edf7788941b5c843bc9de04f1c2c40185a0266596a94e4a40bee852a739f74b83032bd298adad5522663c0c1f42b19d8d08b08ab131a2cf8" },
                { "szl", "9aaeb364d0c42b98dfb7b29f6cf2ed2dfe1891d355faca64e36bdb13e99920eec4d3237334c28c976c99949fb11b35b223fa17a987a5e880c0212125e3068e6a" },
                { "ta", "f90837428f36f9a0408a3d719ccac3552ac53e4e4c97aee61f3bf339fd30037b27fd365cbb8ec08747d0da579e43606cf3de22f5516f45cc9d9661e41c068b78" },
                { "te", "e57fab63369652a4e87dd90c96e456558c6b1c04243f7f0ea0e051d9381dbf8d98825ead1c71d2d1da1370276874cc76ba64d6c87b8ad573b802a8b9b7220da0" },
                { "tg", "2e095307489f572f6a95c53788830497b6198c80c0d2c04a95b554897d9e75d443c2707cf3452ab1c78bbeeba47c3dc172a629a299a2efddaf7839abe1b5f8a0" },
                { "th", "c1d201dace1714fc3058f612f5b3df50b05c419d8fac740f553f3b8585f3ec5be306a32ff0b138bae7d62fb7c11e78d570d3ac269ce438385f89835b90911470" },
                { "tl", "6620e56e1dad83247d26f7f43782b54df50a76fc061e5dbe32635008de03eb800e857378946d4537acb71069ab05655d808e58fe1999642da2ffa4d4c2b0db4b" },
                { "tr", "2d3f6bd809823865c66ad0ace955a2d9c6b75f7486902f182d584066c00edc2a1daaa2ff8ef29d7a24f73e744d5223a689d8fca0e37cbb3812cb1ace3a681c95" },
                { "trs", "5ff868b30e4462be96117f2639bc3a2144734e469b485473fe34fc86fe3e2f58c50b1e3f56be894ec338c4e3356efc98daa272d6cf057ae27e288a73fb4d9ea4" },
                { "uk", "98d60b7b90cd5bc604e2b4318f4d0d48ec46489ff4b3364227e552aebb58d6a9dae335f21c8f3fcb9f25d169a4b9ed13e85b57817eb1587cb5db60d2e33f783f" },
                { "ur", "4072ab8da82534d8927cd4c2408b8ba937c93e746d61c829c27bb61bc2d2ed435b13092b150e4a501363d4200aae41d3bb6a4d9327f4f17775b39c7421f0be10" },
                { "uz", "411a70acaff05562f0be924d3bdaf6aa593fb469ab90f867d067d13b715b95b8b15bb4466c83579b787ecd65626989b740e581e104a558ab09c5c499244100e1" },
                { "vi", "43ed05499c05b437f9c97c5cc733375d3c9eb1ae4de120950c22999a15eeb731aa0c703912024a85a999a3906a2973b85a39de8a0302c01eb33641b518b9d9e3" },
                { "xh", "1d1232bbec2b7bcfdf915f0b531148d3faee992a8d711ac9044b309c87403292619e61991d35b5a96591c4c2eb9e04ed6e7da6aecd4101d241f84c21a6118bff" },
                { "zh-CN", "58fc49f695a2551169582b08677f30fa49764404ad6c12f8f5f4c2f5160654bd0da739ded88ffbdabfe2f295cab84cfdb90b20b25faa5d7558f10ab302a3ef0a" },
                { "zh-TW", "7167a52be134d90da29c27bfe8cda3edc15ff472b415fee8ffb465ed05af717c67e8d271745abae505c172d5b78949a39db874ade1939134525e9deda10a7ec2" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/130.0b1/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "c9b02063495292db74faa1793951ad832154d3f732e548f09a200ddcb8bf0dc4d6ff9f174f58a40ebac44e05d95cf76f3f197f78ee67af59393b7e6189d1eae9" },
                { "af", "e969be317e9054cab4d8bdba40d33ac6651990c58f1c63d584594b291ea559f5f0aa13a8042b63f81713f42e6747e9e048999e82115c0af2aa3dd40761dbe218" },
                { "an", "a71c3f0a53ac2e801450ba548608c74a6e99db3e1796c342a3f3df287a5df77cda87abe1eee965e2969d2038fa9d4e74f75fb9e3dc3c6188d4dff8c954b1e6f8" },
                { "ar", "8a1c9b9625a33c79ae5ba4217b674e566b8a31b4805a549bbc65ee723c3ce013994b72818ffe46c7219b54738dad78b5f96b88e6a66e72e813aae5f5c311ae8f" },
                { "ast", "28f5c53d95808bd0278efde30e04b4a5839510f344b8cafe65378f7821a4f8b095e1a3c7f1ed8dcbc97cc527134e70700166bd3b8f3466d9d8f7fe87778d141d" },
                { "az", "50f50710f9698602a5fa955a6243ebd191a1f688f675c2f729b79131908b34f3dffdc211860dba57af18e2c917d66e66c44d4487488a904e908c8da134f96890" },
                { "be", "27d26dd2f173ab5fb32f423d6101c91afab226c34bf8fabfa402856888ce78323e417ff295f875f39d24f09f41b62d4cb8d0007b3000889afd813b6cfbb9ad7c" },
                { "bg", "43ed05de2a409c1a642398d8c5f8401d0c3131cb57b8ecba12a2a69095215f2b3e02daf5bb353d7e78057974f55b6e4b7970908d038b04fed0a31b7cfc7c9cdf" },
                { "bn", "81e0d9ade4fe32b00db214fc5069dba6829bcb598fcfd0ea04564734633cf0aa2693c3707149bf2f26a545a3accdbf01e0be8bb821cab1257f97e7cf6788486f" },
                { "br", "ac13bc4ff7e5a45605a7d48cda77eb33cbe1f7887c62cf6ac6ebe1b5ccbb7211dca8c8e6b756278d223113d51b3d568146880ef718a5f1bd28447273263ad401" },
                { "bs", "0beedee96286805ce1a3ae57ddae62e123136e72fae4f67511cfabe2eae0dab553d4a2ab282254c094148e5a59d9cebe6cf0fb0052d1ab8b6199c893430fb15f" },
                { "ca", "f534c8bdc441139f5356166f4de00d3ea84732dd569f0c30cc64e0a82757422abde26fc9ef1a4614b5cd0baa65897610868f23d66a875dae538ebfa93fc48551" },
                { "cak", "61c3b19e0058bd118734f5202275d904903c62ed15bf478b5fdef4ce0b5b811b24bf59ed929e2a0fbf1cced08757f8735de600af0d42e02cb47c06a2e0fd0402" },
                { "cs", "7e8326368772020f8a4264aaba00f8510cef5fe1191386beb34df46b3319cb597e69e7c835ed43be4031ae6d51be33164541fa0dc2ed162d14147a8801063ee8" },
                { "cy", "74bf38f3614398055448a77028dc079802bd2d4b9fd29102cab0608b3725f883a3fb7727bbc406664741dc1eb18b0c55dda495e3c0d344be0f5f50e054fa85a9" },
                { "da", "0edc243c2d94f5f54ddb022ccba6a1dca44041c1fc6988ed8f04103fe8b42a0060ca69629d8a20eb3e7e0376d29534e472d1f5ee64651e10587a4a6b2dc3c4e6" },
                { "de", "8b079cdef361181719048eb74424735173106484c5f452a7a23c99bcbdafd399c6416ef0a23f60cff99dcca1e9b0a9ff173facf5680ce371f40827007cd27c41" },
                { "dsb", "4a6a263875ffddf23490bc6a484ec26901a63803293c62841d9456060076a7c3aed8963a4fcf7d2d25a53eba3e287065826dbc9e8b0d215bb7f94b06f88307d1" },
                { "el", "72b5cedc2c7f79016c32b5fbf1f6772fa922d0aeb89323ed62bb15424381ee1d38c0600655bae43a2e5066b6ecfc5605577542b1543f59fc6268af6cb1f20246" },
                { "en-CA", "9894f25aabe2df19ce50063e5362505a6b7571b57b555490f7cdf3faebd68b31b24b3b8b9d42265666506ae4e022cf9243083fe007f3636b4836361a43e7cb36" },
                { "en-GB", "9ed344584b8439729a8c6f3fa88a7116d8d60e5d6eb0581c6daab49603f416801112faf7152b1cf1da6b8d344f1f66c67cf02c944d4ff616188f188a637e7749" },
                { "en-US", "391f32ced3cdb466dd0d9faf23e3565c9b796ab64df3605bbeb68fc9729d63c1e3d055229ed6bba664e6af280cbaf0e9da07e8a194c931c80322caa35cb09c11" },
                { "eo", "640d01b3e8b52b489b0effc689bae63e57535ab5ce255890d30226026f13c558df71daa01149555e26f8eabbce9eaf36289f1ce106d0d6640ca8aba44b158b14" },
                { "es-AR", "6f40e5745a735bd17ec2d666d3f70344af182829f34410f4024a104697e07fa02327a73843d4037ed886e11c6b439fa75aaa2b51d2b111ec1368c3a4267299db" },
                { "es-CL", "ce2e8f8a3b6158d936d9194895ca46b9a37ca3de577290241ee3ac4e9bc3d513ad9f1263cbad5164ca1f8fb35fa00c17022bb13df9165772a13a4096c346d570" },
                { "es-ES", "c1cad5b9a94c050588687e9df8d0c0fdf8d4134e03bb8238e9a7c90138a36c94e387f178827b259c74116806ff102f47df7392bbb3c0291cb04315a9b8ae886a" },
                { "es-MX", "ab17659103dc123ea0a769a92b1beb2ccff49006bb395612bcba3117f5060d6092d1cbfebb248aa2ad1a3d9e3086317cbe7b1275ca6297e431a804368c1ac512" },
                { "et", "16187edf98e8e7a0bd3b66f34144e90c8dc6357ba22d73508fe234f35defe1ec831d02e1f191bbeda2b7b327eb45abeaa3c547b1f80c97908344d95f79159560" },
                { "eu", "2927fcebfa9f7e859ef8f2f065d7abf3b8e5d3d16b9dfd2a4be72cab963cffa72b584eb0ef555a7c730dce4cb6e25aca5a935108a7e8eb77e7495e059713cff9" },
                { "fa", "d8a218341a0f47425e432b55d02764949aef16cdadb01585c479d6ebb6e8e4c32fd56fda09ae502f961b411464023d23a9491a577f88b7a5f0ae6e5eba453096" },
                { "ff", "fdf84817f4f0a3f6de5c19e422a2c660718b270fe9237a4e51adb4486e5f860be5fb680b39a05e7cc50e34b03e4dd7e98ad4f55cac0baf2a35da268d98286b6f" },
                { "fi", "c93b8295ae5d5f23dd958e8b2b1fe4f574187b22f99e7d4c47a0a1cfa9c788197c81715cd434162398e7409424cf8d6b9670425d29471e15d63889c3b5749119" },
                { "fr", "a5fe2c32494c9a3718e6de8576ffb2ea0621e3b6084a0bfa6c8a326f278e7493ea33174855e33bfff89b4d4562065d92d7a815cbd8b9dcfae61cdde44bdf05ef" },
                { "fur", "3a8d1f8fb4b384d70bb0a5e825c22b914d30d52b3263a97c08cbd78fe09e03c8261edfe4988e8bac49df356f407902ef3ca4fa59857cea73b369e7c69c81886d" },
                { "fy-NL", "c4d3eca2f4adc9785ba02260dc848f8f8f05c4c854589f222894d375e25f03caae6497129b63d660d088e57c405d0579b7e489e1d641b5fa0d901c2d17c9be08" },
                { "ga-IE", "b6bb1a9c0e349aca35b3f8877754a7a205db684e96983faa3f1f3d858a8c2608e4ae0c050bb3237967cb05e801e2c5daabd8ef567f94f4f91892ecd590c08d2e" },
                { "gd", "24e9edffbd0d2c7bf07fa66dc715c7f447e1d212c2a2c81a87eb21944b2d2bd5ec292eafda8484b77d48089aa4d2180f58adeecda5963472cbe9487d29a80411" },
                { "gl", "96fb6bef610920f018745740df8d51255166a97db0190ed9671a4a95d6efc8dc2864c9aad98edf44f0ad6a2ad03e4faa304bf5c4ba44ce0167597c99493f5628" },
                { "gn", "abab057f418354907b65da363a5ab814fba4c8f974ac251a4c270ab4fda16b496a8a9355d3d95cf6899d2b00f873c7790d4ab13e2ca64ebca9e566bf423b9d17" },
                { "gu-IN", "796d53bb052ed9c2c1eda7c8036f37a89602a07b0537e7997ff300fec17c2d9d5e88d4ad64e11d6a908a2e5d0a26375f258d2edfa0d4108a845883157ab2a84d" },
                { "he", "9ebee9c0af1e0168a453d4f75ba93cae452b3729be08f2784d0463e49748eb164c68a4aac24d9d7fdd6f85c6cf9ed7c49aa59110a8cc8e10364c5ef118e582f9" },
                { "hi-IN", "907319efc69f9bd06ebcb735c084f97126e42be916825b70a774e9c89167b93d640f582d6e229e99005bcbe852bfa545ed8987d4b55895c8619bd70985fe22ca" },
                { "hr", "d9a7ae23030675502ab74e2878799df01559d75c29712763b8945702309e8898bbcadb89ae7baf54ca658dfe07788d876358724d3f540f739649a654c8eedf92" },
                { "hsb", "dd859e75fbf003398e0101b6b332183e064a0fb2aaa8de6c5024f071e42b6497f3aea35bcbb0ae097f54635532700f75ad867eac852db987da719781e984077c" },
                { "hu", "1e66d21b87e4acb24ab3bb8a0fd249c63461621659d0e0a2d1b990edd721ddea16fbbe760f87010c4150c0dda3a0e2d8411ae56372b448e26aee762c000c7ce2" },
                { "hy-AM", "236979e12a49ceec86282e338200d0cb9c554157f95a823c481f96bd001dbe240bd110bcc6b50d0e311073bc7e746f88b52808d88df6e57a3cecf21985a11455" },
                { "ia", "ab97f7e69084dbd28a80ae898f9e488fa3a3480dedf808803c540e68517802cd0f70f7ad5a27a83cfd76fc6555a3a6088f45aadad084152a326b508b3e1b449e" },
                { "id", "1cbacfa9dd098d15f96f2c90e6e9e54736758d0eba166c43811ba65b0e94ce266da7fbe867dec4ba79b2a9908e3c2c990a99dd44a0b34beab4dc75d425df9af8" },
                { "is", "f20687f5626f83735d5a77dcfd6241a100f428dcaae66f4b17bf54d7c4d514f6d66d7bdb9c31d2643a3dfd1e980b3c184a241fde99bfe88189bd88ddaff0d64d" },
                { "it", "be76b3c27817cb9cb5c01d6c0209ec1933a5a44b6acca721582d9c84aa9bf1c55b2d8ba181ea00ac3f3a71f1dd312728168473f0b345949c028137575c1ae21e" },
                { "ja", "bfd1456e11bb7bbe00dbb0c733e36c8d4eb52c06fcdcf000306671015ef32b423f5bfb0f5b7eee1595444a153fd4fb785e7559554d02d6790b1ffad113909763" },
                { "ka", "2495817c18f53883f7fb2a51160bd2caf8a1148fc344ea5227047289dd6d1a8def06b4602bbe1198ea2a3a28c3a4d5294e018c8c5f440b2564c4cf43e5a9e74b" },
                { "kab", "8c5b02cc1588bbcdd80515039187d45a16a5eb52585466ad4413daf1056375f1481a63b194d6fb63ed51a9b9897f6086d40c40415d4f1c463087a4a119bb388c" },
                { "kk", "e8a13d7410a64073b5dbc9094a6d5b490f936df4263a99dd0165a28e5e65cc85da6e75ef4ee1c3837af79ceb61881618a7fd2ca5e030e8f43cd35decdbbe2b06" },
                { "km", "d7a875684d0dbd9a770819aac86217d64d71b0d0beda10375fd1c2cb982c4700a5e9b00817792d7e0469e2339c60a8c1e27f46b9cd33104de059face3e2fafdd" },
                { "kn", "8b1ecdef9bb0b1368e3e39029a4deb291e4eab1808e98740f694406628ed99082e9573fbc496e383e545f6094637617818e2893d85ee1bdb97c93040b8f3d460" },
                { "ko", "b04227d809aa130988a1243b5af416d1cc927bee04e2f41065c45996781b20917a5f037272c8f2124e6b5152384285e64ff99882738885716bff693a9211a54d" },
                { "lij", "a06bdd5c02a5adc04319f2c51bcdffd34bec6b44f32a2f9e6b4941b576e8e9bb2d17be5a5b8972956fe354b0e88c24895bcf9af9264e43b186a991c41a96497f" },
                { "lt", "0a79daeac27d29414d9d674fc1e21cdae16839328ec40fba9b623b49842df53ecc89d462769d012069462aa425da5d192d167132953c243053f5272a5d32b5d9" },
                { "lv", "8c574a139542a2c479dd5f6073b3dc0304611e50b701275f5275217fe2fea457f9a0dc3b46be6ea9fa7f14c771e8e7bb5a23ef8f0e9129da0751d60a363edbc0" },
                { "mk", "cabd9b2ce97531bb122a352a2171ca847b090acf38ef1e846d1eed229733314468b6c310de229d3ead2d4662bb31b21e02827b6d03174e5b745a3b5530f5ddc7" },
                { "mr", "baee574e9961017ae67adf6fc65f6398f75f977df43a6fb66afaa881f616a5d913e0dfed400beb77970c05986b22d1862d57f978af7c6d78dfd1aef9dc12689a" },
                { "ms", "b85583917b2da8a8cb401e4335b170757423298119362bcb55246126462426369be11dbe4775537cab8cfc4a5d8a1a3b8cb071eb78d8bc70613fb248978b8ef8" },
                { "my", "d7035f43f0db7d05da898c59decb2b130896c191b4de32d87e363eb901433175f3a89682d36104d9ead9985ae69b21e66da979f70573169f48456f02c15436bd" },
                { "nb-NO", "b43a02fe150d8eb9adee2e1b8366925194e970c9c8e236b7ca0e90ae156e5888e59b3dbc285e867adb59ac0d5ca8c324660a0016d65ca0d86fcfa9725768a040" },
                { "ne-NP", "f0dc6cceb0baea3c67c3d0fb8f961ec848d0cf79e3a782edd370366adf280a1a065e2c6da78f9371dbc55ae3ab90c0182418aa1077df4dc7bf2ad8ca1ef50880" },
                { "nl", "534aae98e14d922df54c52b911404f86924735a687a9cbdce14af272dc14b7690830cc23571b481de03d691364ad6c005ada636325ba01ab17f0b9770aa3093b" },
                { "nn-NO", "e56556ac258c834824bb5c9f6cae1f85615ed8d76ba9a5e9c5212ae9f5eb8ca3e3b26657a4ee82f5377c5768285768f2260a295589c4ed9c35c6ea7b633011c9" },
                { "oc", "2a2f1f7242b5cc57ca04837292bc7699672d68a83cd13bc60ee31a068c79b6e997a905afd258627aff28143cbb9b0c3983f71ec2750441272c9ea146b550fe2a" },
                { "pa-IN", "36301a37da3a979afde2ad16e52c1c400583e3e30dee041150ca5175488a918d2b14e342d510343f72663c06766dccb7f883b5367e9942fa2cd983facf420f08" },
                { "pl", "7def7851b849f5f5d4efddbae5599387647088df8282e0e1ac5a8fc38ef9831df4a775ec598c68be5a79669b234d87dbd50a3549120dade66486f7c67dd5798b" },
                { "pt-BR", "7bca4add0bc3bee362b5c7303eb688de1a1dcaac7fc952a0d764c10fe011961ab4a830247af4bba4a604514fc49f0e48ba13dce837b85914a4382f7dda83c2ab" },
                { "pt-PT", "82e372497c01e3567c61a3516114d8c5848a132a0da262ccbae89bcb6be10d2aa04e6989c03a37ccb9751a5ab01996bf0444eceb58a0cbeb8f32146d60f43a31" },
                { "rm", "8b7cc874621e68e9baf46a9a134243fc10ddf263aded58c243c73022c733a90b8f76519854d9bc8a6b1b27ee9b8e0bc0fab48aeb7919ce8eaa3296553ee1bfa1" },
                { "ro", "3dfc034cede33532175f5fc44e358ebd5d6699fde30e834e9be1e2393724a4824b0d86746383e8b4972b5c867108631b8865a81a0e0c66f109746e9bf3f92c90" },
                { "ru", "3addd8d66dac946c0424ffad874f68ad488fbe7917642dd6125ec9adc564d708fb028f195dc84dfffd5cd6230632018bcf6d76f8d6b540cbde093d9271b7732c" },
                { "sat", "b5ec03c4c780cb95f33594caa8bccbd85167343fbce016ad72a5b0c7511c1d1b12da6c14ba051253a181a3150ca72fa2690f27274b766d3c898aafdc51b7faf0" },
                { "sc", "b1023f3d66f6fc8b46aae83c5a06ab4ff499e0a46245d30a2f8183eff58d08cae48b3dd911299b504fbe69911c126e498b39b071520dc47c5f6a95fa893f7fd2" },
                { "sco", "b83005b64b7aab46e6e52366b0178ca70dc01a998cf1777cc09202f576c184b6bfb4e5c55b8f600dbe189ea269bd3a8953bdc703dba779a012429997e0b9aac6" },
                { "si", "c9f3c34065aa79d8abc3c93d3512145b1e151e3e102c47637b60710cb09f51ce2d4dd202459c60166929b772797fe328796e912aaf19c3b9fdbfb1021d7d9b13" },
                { "sk", "434baf520f902f2ce133bef5efc304790b37daae3297f5dbd726f30fc8016520780b6877a100aa3bc59ba478f5c551a99366cd88b19e02915f4c6e01b729cb7e" },
                { "skr", "7551e7b7d5d5c95a51634a3f2dc18c4a2de3c1a50e706dab267b770481675b246826a3d867e6bdd3ddcd2382db6658f77f33ed1e1c3a61dbc18518f5d6957f15" },
                { "sl", "828e7217135cfb2d90a873fef68bd300ca0d8085b82e7800162a98cb5acdf929b20edbfe40549cf7e1f168669c9881eedac0d1e317a31b303584cfce465529d0" },
                { "son", "3b67a7dca3327ede5d440a1fc1217df7e0c8270988f4b119ec73c492d89b31418b08f060a41b4c76575bd021bd1c3ecf15e4ea9fdb49b34d73c27a12a486f664" },
                { "sq", "4a31ec579d40eea5bd0c0610fb25051456cb204c9854682a5cc82ccc5281f92a0d28ce0d7804782861ea399d7e29f943708d9896c91beff5dec167697dfecebf" },
                { "sr", "7bf13f8716125968636df637b4882c20af460f412d509b54b7c8f114841ecd052dfbdd69566112d9c1fcad22cad51b7d08935d7d842285f02a26e9a450c14aee" },
                { "sv-SE", "31a78263b4a3174f5706783fd2d91a6d350463f04a122610684cc727b57b9ef2a189eae606d47c2368f39744958cc729d41a624849bcff6ce2812694d3c261f0" },
                { "szl", "879b93d6d1eb28d1ad95bdeda013fc9bbb91b560efa63cb38cb6ed6e4bc9ebb3d0913fe6f1d89e760cf307071184718720ead318a35c636ebeb0ec293710bc04" },
                { "ta", "68b248b397ba549b0777ca72c50566cad25930377ce946ad380bd49b3972c09596212e448d1bf87bbe1c099ba0edb3aaa2fd34022c35add6db1475fb7c156e66" },
                { "te", "d49187dfac6c565c43bdfd2494f10c83618dd02e66b43017bb66329d68be9c4582e3deaa4520ee52a336567ee0ef8e071db417c95c79071b84bd05eec6195e80" },
                { "tg", "7d32d4220fb0e4b641e26882c36442ac1c66480209782977c98e15faed1dd3421cb2761223296b85163ab17d1ab1f96d6c85ced83f426e88e7d9ef7216483e9f" },
                { "th", "ed6c2011fb9295cf54973c5079d862a05e0f0b9dd074fd1f7550a785156b889eb4cc48ccb80654f3a903933433ec7af6f2f9f664f58320fec4d9c7b944fbc3a2" },
                { "tl", "4b0c32e8fc8819d0f3d9fc63b023bd1aa6ed9a88ae9515be958da9b54281311163a54dc719193c0cb710432ff08737a2d89261aae9388a0b895b73e3a5c4611b" },
                { "tr", "8714a8188e173bf03c411a2f7daa03880f3045485080f3e99ab9b3992c5f61137dcf8383d613903807227878a21773f440424796ed54190ab2b05efecf210bd0" },
                { "trs", "4524f5c10bb0e6e80d4405bd5a296279dad1957c1733e5a7da1c7bcc202375c65b1b373942d4d9ab3209cd79e6d4c04b18cf59e3cecca988cc81aecef3763fd7" },
                { "uk", "add2d8dc61121bba80e29c9f7d94041c20ea6df10a13b632b630e1f095a88dbde6517cbb93102d02c9e3716acde6422cea21b737e2a0b84d2e0b334a8cfae986" },
                { "ur", "db80dd2f08717cdc59300095051eb15f2137fad2c075e98b61c3415cb989c6b63625314ca152a3f91908bc7b3153eaaaef81d9906dd921048aa591f111a65080" },
                { "uz", "0d98593a3e42aee3515079cb8774f50a4909e155fc6c6f3ea023f39cbfec5df76904658c38c75b781a2f9c6546561e62b375d4eb4b054709a0dfd1118528168b" },
                { "vi", "d4810c917217da0c7932d548a17cc0430d7f745535148a812b2211fa66dcc816c7eb04639af78fd80a5695747a9c3ae453e417c2c73ab81eae5229cd6dfb7ecd" },
                { "xh", "c46ad71197bd8f7184bbc89a4dae09182384f0faa62096ef70c642158ec072b7cfa88bad54549499ef2ace9fad0296d066674462fbca0a83057eb68d0d791906" },
                { "zh-CN", "993f7156b03811e3cb5728a6f3ba4029621866f7a1495bc9dca7ca72625c3442005e32b348afb20d221a998551dfbce365bba32cfb3d6e2273f9561bb5675f63" },
                { "zh-TW", "017a5a2ccb22d845c51a5145b720d855191f56f37b92808030402063a8f3210f44c82b33db6e54651255576a75ce89d056ea1e1a0c9db6c5551a52cd983b6462" }
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
                // 32-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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
                    // look for lines with language code and version for 32-bit
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
                    // look for line with the correct language code and version for 64-bit
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
        /// Determines whether the method searchForNewer() is implemented.
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
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32-bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64-bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
