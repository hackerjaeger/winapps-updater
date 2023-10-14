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
        private const string currentVersion = "119.0b9";

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
            // https://ftp.mozilla.org/pub/devedition/releases/119.0b9/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "ada333329e7294314a070630216240a884cead76a5b76c6725e225a2e822b3cb426b98032a23600acd0caec4c994d3747a6b52af7ebca1713fe8770375a41c40" },
                { "af", "e65e40643c85575c8abe57831b38daa7f54afb04ace13505bcf1cb65034b5637ad1f272acb142a3d41d7f89a1a678fd39d477340b3a4d86ab2a25df5bdef721f" },
                { "an", "919ff29caf90dd237b0ce5b8af547a66aedea8951575c7b9269935f8fea1e198fbbb752659fb580e429c4b6878061fd759cc3e50e6f0b5258b7179fe3aa50207" },
                { "ar", "1c383cedfa3a3a2883db8e06d2c7248fbc5b07ab2546576e0e9733ab0b2d37bc48b4fac06ec72856c1781516ba2e3fa081d3fc70db99c6dc08d95ee3bd0cb1ae" },
                { "ast", "abc31ab28ee00b43c82106bc5eff72a9cd7ebff64668fed3cd045a4aacf6b17cc9972814c5bd9af518a309e12c8afcf816d225b8d74751835cb7c809ca143ef4" },
                { "az", "9bc4da213b34b1cb29ff6454dfdb3a30ff7020e5ac8667ed0c7347bc25419abeeb8a2391f37fce0398924ff50d2dcee7dbc3f530946e5381596d0cda6711f754" },
                { "be", "9b26f380be4b5cbfeb30d72d766d1f4e1c8ca6333d4ddabad7fedaa1c277fc8eb1e407d7bd01854786f78b2e7c590e746492d2b55d7c6d8ab71dcbaa1dac2d9c" },
                { "bg", "c07e9ed36886633e0169ca3810c58d30e46af694b353e267ea456f847d64b27d0ed000b907896891c3c59399d61a6df5fc7062504ebabf95d78d83e7bb1fe02a" },
                { "bn", "d656ca7a059658784f44499375e24f530b0d7262bd7ac9ba2675bb512c24559351c78be359f2b5652955224b7807edf6add5c45a91021d84a40a8c242557ff71" },
                { "br", "3714ab5a3f7f4085428dde037663bb73b0eee2ad6828cc59d63543b0b5579e3358b89f7f33f8f98d2387d7f5b82bae336ef376b697c9453b5dfc66d1f428ad94" },
                { "bs", "eb6c1556d9426482269fbb70b25f7c750e5349cdc2a65e5a20842d1095fa2bed3460b9d9b95e84302db25659a085a1797175d525d4b9c6eb25b6a392b10ef574" },
                { "ca", "01858b4827adb630c844f7197b47f5e3e1f38eaeb083d40d42d0aa9cfc7e965cd0ab98570995f3f6cbaa8d8892ba8c2dcfa6be4cb4ef081aa76c7b5c33efc14c" },
                { "cak", "354bcbe7d75e2c8d2dcea677bd3e6d0d524cddfbb21005a10210ad0a5f2c14e95b73da889604ca86f390493f6eea296a8a62302b44f73422540234106415fc1a" },
                { "cs", "ae9b4cf15c4dceeb5590bd6098c46edfcbb74e17d0fc37b699a9d51a6e2b39d04613733d72a74c7c6ec3549cb792c18def685806906088770d361421f82569b4" },
                { "cy", "7981b4e07c175203ea3fea95a0e677c8bbbe292e8059f4cbf6fdf64a2d7566fda36d105da184df59f9c6a487acbe1894b9c923d8173e1274614ab50d54607027" },
                { "da", "099e2aae0215a384ffc6b7b663daf024230aaed24edb3b7965c0f3dd92b8aa8f1bcf4e8ac08e6e32143cfe1d5649dc5ed2f7f401aac1f611ea2686da57ce152c" },
                { "de", "72fed8acd8f1748361fa20666a097f5d7e5be7499ca14ddf6e9ab441698200fcd49f1bbe323a1edd791fd751a8b27ece03011c75e05b8b36890a65954ed4c171" },
                { "dsb", "389759855ab1e7718d05655c19f9638d5d608637ff6f75d7acb0c2d0f15a3e6684669456d5c4d0f8118c19184b57adad5e193aea806233f8c3a5561082ef908d" },
                { "el", "64fa891d8b2dd4aef43a0451b64daad5adfec92bf75dbe7598057015e1ec852e931cca6ab390321f0fb48520c7bdbc57874accee97cf7d5d10e84493692cea40" },
                { "en-CA", "e9debdf68d57ca1b14a1ac9b503507fbb64d153480eb81e28a216677c4f39aafd3c3febe22200d7451c1a6005ef67be999e96c30bfc45b79a1333c8a525513df" },
                { "en-GB", "00f8a37942ad1c9c5dd9e5866afd97fcb2316e0ce5202dc40af01f5fe3a7e39022ddb7e38c1fd5248105957c9ffaeebc82e680ad15ce06b315c274c933d3804f" },
                { "en-US", "99447c7ad55f2439f3fb43494a08f616344fa02a7c6c526d574702bf2e9fd11e01fb18563d229ea28d4c869b980555a3490fc03453b98fe025cb060eec1f770f" },
                { "eo", "201347ba6bb3464b6065cd0f4555d1206cb683154ccf2edf9da5dee4e22a483b60e8180601f7bbeb4c6dc651a7b6ce7bc81a645d2ce0cf76378d7d51c822320b" },
                { "es-AR", "b897f3ce8708148323cd239afa4e88c3c269284263b75d5a4d5306e30b7182422cf85b7bdfafd47a7315ab3269060e633bf31c0253245671779ae22d54d3ac81" },
                { "es-CL", "e5afabc8e3e87abf5809d829d3affd777b09a04525f0c712cb54eda7a0f594b560a4477a3b41beda6f11c9461e8490f612c9328e724a2697fcf0e6714dad2dcd" },
                { "es-ES", "1ab6fbb49ec334e8dce327a13987eb34b7399ebcc7ec929426e9d7d9da6f89276d1118285122c396b8e7e8422bd22d8e27e4e4b58bc79905024bf8ac491f4683" },
                { "es-MX", "a73127a58ebb726ffb8886b1e850ef4e5bb7c888ee51bfcaf1887febfa92f3255a761aefecc7040a36052904449a6fc26ba7110c7eea74743d76bd1750863e2c" },
                { "et", "7ed682c1a3d220b14739a747d586ebd72fb6dc119fdb59e885702f4d96621d2351faf233a6621fd02218ff0fc621d2fa891933ca2454aee7897f1a1d2cb7ebc2" },
                { "eu", "ab5fc8e5186cc3242b4affca6ccc993246f6843a35fa23904b8481a539343c2bd5659a26e4941eba3617027cb3c782c17fb19fd63069ff19ccc5975b35637497" },
                { "fa", "457adee0a853facf3f7a73b45175a048d8f09b441f2b562c7d6d2e7d3f6471c2655d4d072ea05aa179c64432f2536da07772ade0de5f880e1f2ebabf3e24d03c" },
                { "ff", "82de2169357f57f5bb122279c419694c1e499e970fbd4056a71488044b36c1a48da25ec4ae63a5a1349e824bedf60e77ac811fd43a8a560c81cacc391b8658a9" },
                { "fi", "d5eb01802745e2dd34154e52331590b3b9cacf09703be4bdfd9d81c9ee99088fa51ac5d302dd74fdc55db5fb450d00f6e28140159a9c38eab7da122ce31a2466" },
                { "fr", "151b4ae4f00ef99e87612acf73766aef5fdde6f26c8a611bf0f5523e733e06f340db2be12e865a4f4bc328aad817b685a8f61d75a60efe37da5ac7ec803043d6" },
                { "fur", "cfb67290693b987873a13409cceef5b69b7ed4e524d26ba59309971e45ca45f660f61b5c7e72c3a0cfe544a2f29d0c8d2cc466daa2a9d49e15f5c2856494c6be" },
                { "fy-NL", "3a4b5267e1caf4c2201cb68b27893b64d884c9ee14cd93fd83c70cf270171181e87a5d43fae9fdbaeb9a71f16c2b5e4d1b1ff4b15fde63e6a79b9e81080a2913" },
                { "ga-IE", "906fc08162aaeff7c96f5e7609562e8910ce5fa1eddf88d9a7419f1d3ec14fc1e90732542f5c3b0b1586126cb473a57fba17cffff917d7e41ce6e1cb0909e834" },
                { "gd", "371a08266279b5170b4d3dbe66de1e0d66915562c520912855ada3eceb72db15407dc0f2ebd41e725a10af17a6c19faaf60f9b75b46be02fcfe0efbc48b4cd75" },
                { "gl", "4a223e9a3072dcfe292b95388af598685ef807c35e24408aa703e8a55c57e49be15895a6453ae59b9a1da56e87d2921506fe5e65ba52a3a68366d480a39779f6" },
                { "gn", "15eeee205c3e2f19e349959fe4a2964563bdbfe2d01edc3d7ef49bd1a14c7d00ece0b9b4c3a90fbefd88566f1088f8b828a7b26ccca2e87603befb78f828c2a0" },
                { "gu-IN", "47b43b82da20d699a3996d7778cff1ef4f89c76fd3730702c34f0b6ec7042031c3f97871720c4e50bbdf34dfb5b2c04a354ee0e209c2a26584f873f6412415e8" },
                { "he", "6e4792d165b83b490e966afb726a5c74f1f366c6dc4d4b3264129e7dd2c554ffe58a0bb315c20172baf67e1eb8674dae55eb10f3ed4a0556e518d54497ee3053" },
                { "hi-IN", "99d779bd72a9ee7701695bc58f34975d3675a52fa7f8b533aa2c83940d125b67f6c087741434c00222c4c568c5725eb9e6ddba02d686bbfa8bc5119ade31aab6" },
                { "hr", "2babbb0ea339efc78ae0cbea2adc7c0801fb3b34fd72ec5f995ff0b6de2535978593bfa285c1f3a9fc60c248cd95933dc4e70eff8e9058bc523e5027acba64dd" },
                { "hsb", "3f3ab261603cfa867b2d27d0b725d692ab45a95cc1c857383adf905353e3500eb5b486f9dea88862b4a4c62ac547e4f18a7f69296c7151391babd61a360dc5c2" },
                { "hu", "0969f4de762d2f39884a58d3dfad0dabf9a90b0e5a37299e218271cb755386b833cb2880fbfd0f3d7c8e02fe04cc2e59dd66b587cfc112b51e874d96a2caa4dd" },
                { "hy-AM", "d1dae1984177b01a43c087ac8d1a49272a81a75dd69da2596a7f9bfa5b20a5ee25323043ca7fbdcbe78c98be670816ad809f1876cb571444367cee942ad527ad" },
                { "ia", "d8947be1d708638b2c66de70779ab47dceb8639e732c27bb5887c09cafa24579031e127f70d4c67cb7da0b362d94e70b9fcad6dab9368efc78e11b6480f2eaa2" },
                { "id", "9a24e69f48d6167d47056f70ec3b4bccb20291917e1dc648b554d57198362eadbdf5af0a2d00612aa19c7f8ad2769569f6d28cd8742e6a17ad0394d1096f6684" },
                { "is", "e8705e4f7eb40e05430ae78c50723f63e2726a92a30a87c2024d1f3c17c4e3e4f0b44d6891896b5f271628cd90e547c2ef9387751aee0eb2944d612dd922a203" },
                { "it", "f348985ce405e82ffb098c13d49e39f78bb263624750b5111887334e79a552180a401b0d5803e8da058def89cf410d3fe735c36f240ab2d82c898153822d91d8" },
                { "ja", "d0250450cdda1cb52f89816037abb6dde01ee537e19bf344fe7b3b9cc6912bc46eba6b1f1999af260325b30f5edfd11a85e7e51c8e3dc7da8d9077cf4cea643b" },
                { "ka", "2bce3668d789bec5c6fe729571efade43d72b2c51ac44f4b5cd76c1a392bf2f51b96d59f57f012c98a1f1de9b96e3738d5f60df2dfc261e3003a0813d1bb5d4e" },
                { "kab", "c903e1b0fd4ad29a7f448b64916a6a49a28387aa5cef7c602c0e351893458470f39b1351613e89f354606f9be2227bbbf35f98a8ab430e0e339423ae29d119e4" },
                { "kk", "2ddb629b76f4d4e8cbd82d18028eaede4d6490df45691d75147c551ecd0fd4807ac06a66cd968eb3053a0aebea6037d8a9bc757ec962fe50fda4829f5c2f2aaf" },
                { "km", "8d7c67cd05bcbccb4dbd1605bd2c4c1ad3eae2fd2af13de8633ea602bc3029b55f166a32553c21db8f60b44d661514478c4b4188dee949debc36f6e033789957" },
                { "kn", "99281076a3d6e86c3057baa7ddf4d3930502ea040086d3b544a2e13b4b56cc0c5e0a808d6498fbba7d5e2ced5c0517e5e86125baceeb0a66a8d0d1d03cedca05" },
                { "ko", "db3895a1bc7fe0d22aeea0f16fac15dd0515c05130babd00bd6bb3dc253626674fae066fee78173e303bfd85c226168e5caa25462fa55cf81322c5f91422d83c" },
                { "lij", "08ec14048d48a4d4813a127a418a2a9068540f44cf5d2a25165195c17ec6a0c1b8c57817128a5f72ace4d5745f27a326e285cd41710eeb2ef5b8bdf0b94a6718" },
                { "lt", "fb9dfec03707655d1617ef4004b98f9cb7ffe32620bb68864772bc1315b01364916056a81272aa6db2af2f52c629b676920b41f858826a5b0c6377923c081dcb" },
                { "lv", "b6303460b3e434ed03fdc76357210bee79a4e46f28cc1d191d138d23d7fb2771f9aa8ba5348f9caf6daa0ae69796fcc3ef6d4eb4de5a5f50547648fdf9b0ee8e" },
                { "mk", "8e870936be352a71921270a0da0e1779a07a90469b2c9c8d17e4ad47d2c79f89dd89d7a3ccaeb3dd5b33ec8d14ec159e68dcec50173e3d4fbd3e51eaf828a1a7" },
                { "mr", "f17992be7935395550654fc415fd8b8f003c724e0f9f5db832aff5ad518cca258fcc06ce6446b9e50a630bca07a7e34371a9254bb3152f76e80d19fed4462ade" },
                { "ms", "5f1542ed31779562eca2525ecd61996b4d190b249dc4bff93efdbd1c8b621a5bfb44f88af13cd149035bf1ca86d11f8ec67ad67dfcfefb2c92b3b074d2acd964" },
                { "my", "1b3141a2b36c070821636aecc35ade047b30c94cad57540331b463e1d4fd3b1a04d31b8685b67e31517b042d69880073376df73a1fe5a4c3658cd3bf00430377" },
                { "nb-NO", "d8e118e95afed8c0fafa16c2072c330c678fd20ab8ccdfbc47208284875e1116046ff88507a2567f3f0fba3e3d9fba47ab71155202f0823f6d8ee338c48b586b" },
                { "ne-NP", "fc7a26379077d9d8c5020c77281561cab1585d355a80425f6c5a62bdc66bcd509d51979ed0659b83cc9e105155b231327702f775a78479de11ec9f5a9d546e4f" },
                { "nl", "721cf67823a89429a081e6eb2232ef722825d515455f22a4d8d94ea87cde9da462b0498ba25e932b5a1aa0858930d6f05dc0b9533f0de868844e455d9753cb65" },
                { "nn-NO", "992cf00c774d5a58f2c3a475ac6ec6355fa0605b2fe5eed6fde8336926aa9fa4b64600bd27b629ed5cc70ac63b8b21272bf0531bab6e3bf68163199de6cccb81" },
                { "oc", "2a16df7a5f270dbc1ad9c6c0a19157bdb168b4a457a783e8e15d4d0727ac2fb897cf2269d1d8ea7081bd2fb0f628092ee59ab3716002fee93a232b7542b393bd" },
                { "pa-IN", "9dd40eae50a260628c6c15470392db7a4ddd392fe34aea660c7ff163f4cf5056a04157b6887cffd2e482aad68fd1cbd49ee65fd5d342e1b8a59aa92d6c6a2aa9" },
                { "pl", "af8acbf1f28c5250a5e4056af2da6c551fdccf30b1e2b51e6ba351c14cd12026b2963189432adf4782d75b53803e743d4ecedfcd2951af41b84581857ea9090e" },
                { "pt-BR", "b3af3e1c6304cce8d69ffdf97c82edbee7ad372b95067acdfd72c30a62cbd222231edeaf3c6b6d91998173226946b5329c02d087175b00ca31877d52990d6af3" },
                { "pt-PT", "ff115e2539ede312382bbd4103785b745fc3aa2c4491ed2891f94d298f9a816024f3442a2bc5466e1dbdadf0a407bfa858a5a2bdce8f5903847eba9da60b28cb" },
                { "rm", "270dab2e4c09e15c818fd71e4609c6b8941e7e15ff717b3bf1ed0f702cdbbc4f25ae63f9cf47bc55242229cd441152ab4cb54627a2e5022846a05be85c62c369" },
                { "ro", "524289087c8fd1aa1069fc3beee2db68090e3ae07808e25887cf96d019300e2dccfb29571d8035237ff59db58200091f4b649e11bcafb8ffe3e56058977f3d51" },
                { "ru", "a26918d04281d0b3a895788ddbc8fd0e94e18574958866ce68ba90b5ff0a9cef07a75fbe3c201e59a9b498ea9c0478130a238c9aa4e30ea04cfc52e65dc90f28" },
                { "sat", "733577387c9dec6793b665b0f6ba97a70dde0c0687dccd06f9e91a4e6f2709b9dd256195be283511b9b80e9ce3d7a6c3e15253ab5eb119b0ac4f79ed8e194ba7" },
                { "sc", "9b9a1c4f558162c29224b4d1be2016d3e6e57c871a513f4e89ccec43c37dbfeea9407a79d3b1d38bbbad78701520bca5726ec0b0931e781e494379bdcf9c1c5d" },
                { "sco", "b08a7dd586e2eef77f678a8f7bf5d6d8a41b03d42640b54dfd59b50e554b5e6d59dea7347bb4b02ab8ec96da6f310d47b294c3f21882749b1ba1aae6e9d20754" },
                { "si", "11ffae5ee5f8a5308d201d503f69ce3b79a67edd972d859840bef9c7596c5963de1a301c1b645eb399e0ef43dffc411fe64f9f1d3673dd5fbb9b15fb7df7753f" },
                { "sk", "3737cf98388310fdcc642031118ca061809ff4ea92799fd3ff94a462488e2c14122f87e4eade0990b02f11b598d72025646f9c57e4c190530d549be70efebee9" },
                { "sl", "38480846b9aab2a75d2c4ec28177fa8e670e99b2e4488049016cfebc13b76ee71d23c37089ad159a8e788e94cff85cd9f283a31d280a823a09cf07af0fbdd7dc" },
                { "son", "45cec693b763544191e23b6d47591eabee1b360c1232f0ad8cd191b1a214add7a38b5ecb2fe6a310fd8d76fea8cf1f4222b0457a0025a34019ed06af8c6ec7c2" },
                { "sq", "76163e8d48b21c21bb94c2627e5c596070bce6cc8d2add8e59d47f03e3e364a15b6bf11deedc4b9a0310798322700dafe91b05e5392c3411ae4b6c486284a680" },
                { "sr", "8201cb9e2f38738e1b6178ed041ab32c28920b946857898c713418cb48893879bb43557b942a782a71df6d49dbe678e30f7232c68ad78220be20fbef50c54a7a" },
                { "sv-SE", "180b755459db53aab62fc8e21f664c0b702c0bf493dfa7cc19ca4da548ddea3a44e085dff8aa675c04269e6cf65e448d3b4493a8ef423e2d733c24cdeb36ee95" },
                { "szl", "e425854bf972f00d464096bbc62f65245481a8ed82a8312c09d0f61c0d65cbe0daa91d23e3a722448a9655ced3b533670a418d7f87980fbf9301a67547ccefd5" },
                { "ta", "9edd9147039a5e882f2a926b58523a0646da54993059084a9657ba013f99bb01e8e560bd7e8b93f15ac84db8edc8dbc156ec54097a83059af0dd0678bebfda0c" },
                { "te", "ab113c3244b28086d8e246e78f8d57044789bd078701658c474752ebe55bd451c05346d3faade26fd8a9e0f2923ebdeaa833988adf58d7181e93dbb9b73f184b" },
                { "tg", "b08d932a47b754ec53443724119e8145b282a90f6f5c80751a1c4307c99ab2c8eef5c928d4f7689a90a68da7a6aba660cf2a058f2dd526ea8f10167410668434" },
                { "th", "3f1d19629fbd90b2c1086bdac010bfb138a8f74a27bed0820ffdc7988c06a3ced76f49392e8780f8144469885bb083b2504ebdcb6894801a0fe91c95ffdc7448" },
                { "tl", "b91137c9f263f7401ddf0d436bc5983770828fb3f6556057ee413a45fcb6d6a2aeaa314dda5b642557e4318d8e5e95735da138fd63782bead2af30b8a7639d95" },
                { "tr", "b11cfb9153ac2315d947b9c98130f6fbcce335a3d76f7cf8f84503960b8ed34fe2f1f7f5f2ab4088a052b879bf82af677a517cad63955c64852d5d76532146e7" },
                { "trs", "2d204f8dbbed52e652748c8d7404dd0e4494baac4c0b3d95276ed06264c68ed9f5e234cd2a92c535a9c65c5ea477a4d7c4e6b3f3ab7574f5d466d18f2e982f4e" },
                { "uk", "d41ee2c8d02049eb4502fa0b3229160503fe2e975dcb78ead0bd884ae9c5e62a90a1fa1a38e8a3812192d3e9f12fd81094e06833dfd66fdbdf173896767e3538" },
                { "ur", "bdd1988c8ed1cc22467fd5b87094ab9f2b93f02328cabcfa50024b03d9e904d3d56785a5a1a8359662cca6afd7a593b8c4cdb3564e103309c83871a85c3ff823" },
                { "uz", "b0d014c7bca5f33b7a56baa740bb0245cc18f2b7baf9027cd9fd46741f9054d02e98e762e0035fcb1a1dbc38edc57037f772efaccf3b7d1e1bda86bcb4d94526" },
                { "vi", "72a4fd48ffac72c19d99b609d13d1c6a4610d3a03a5ed5c84a709116f1672975c18fed06ef75bfba0930c2c8325e54f35fb899f45c3476d35dd3ad700fc6ffe1" },
                { "xh", "ab5a1cc3a60fc20216f733d217ce8262203e937d353afa95ba66170447f61a2b4a0d289894aa2fac13a23b8d141e6c13af71c1e2dad95d3a349a35d7ceaf00e5" },
                { "zh-CN", "5fec2ddf22826d61b09155e22d0a8f7ee42c5239f5930bd62c7b235f26abd5caebfa6e70cc1107a65ac11150bba0410f01230efe9220cae99849126d7358ec52" },
                { "zh-TW", "edc33941fa5e8bbee2a746a6da3cb3f40ecc783ac86d7316e99f76b52ee26cbec97deb6d188529589d4ccf8ad31df894112ef3f486f1b32ee7703568c13a1b56" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/119.0b9/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "35abda06034be6e3e12ce596290abbb5d46b7fe9166297fdd2d2c5b0ab568d3a697fb8b47787d839314791557a0865697bbf185857ffb0eefc40b43a2f1b718b" },
                { "af", "67039853617c91e7010ec23d70011d0fc427355bf60ff70861f4ba16b6b37df018dcfd32e3f53451e0e1d9d84709e6d8f23d00070837965aede80f451babe4be" },
                { "an", "6024c24eee90e96f96c8fea188a8543d7bd105ba7aee035aa2024eb9810406afa65305bef1b8fe628ac4c27613217ca0ad21e16bae7476190573adeb6840b927" },
                { "ar", "dcb5de144237b7da3f8870bc885b3783e9435121c424f9409c338b4c3b0f50800ad1a2c8752398a4ab8d01e1cfa2dc3769e8dca36207e6ff454737a57c5f3ad5" },
                { "ast", "6621f1071e217253a42df222e3d560b8f68a820034457460ae1ea1f42b646cfa89bf794fc5d6ac6911b52ff5057992255b5b477064e3cda3636b797aa15e338c" },
                { "az", "e185b93085574eeda4d7fa08f07449f4c227f30bb6ff4d6e3d910cdd4e2d1938281c6c68adfc766a32f83227fd2a6f6264b3f0cfdca65cb634626ea8843e7cd5" },
                { "be", "fb2ae5f6b936f3532d4dc8a82c92ef08db2e80fbbf79cabc729495a06c541e55d3e6b65e8c4de81c906dda206307551ff37ec4af6aebe8746a001bde23fccae7" },
                { "bg", "ab979292dd7abeadcbb0eef1b63a85e77d0de7b59b346513feaac1ef8c8e5785003214610591dfdc315860ac7539ae621f857ae1ec5f9353a7772b744e027e19" },
                { "bn", "27ad291ef148af296f11ea79264de6f3ab46066bbe92dce9a65f49c0f8ba2cbf6c860eb904af6774771985eef4fec0405249a910b4145a965f47338d21049150" },
                { "br", "32a1c65517b827a0eabb1ed188ae843f0fea9c0368122edb1104798be69ab9d2d50dfd1ad122a99df8f517ccce362bf965b4f8bfe0a2cf614c049841a212fcd2" },
                { "bs", "6aa83ee61870444955b6d05ec3ae74d562cd181a88003f2477263573ba7fa2a7bb15a20d69463b899b74a6e5efb4585e92978005ac01b7bf21dd0e072958b866" },
                { "ca", "60592fb355af54659e4c43ef7a1c12a16a74be8104d31548ee357d1922992e1239348097d463c9c4e10d6214010e52016b61860e918736f0529bbff617ee112f" },
                { "cak", "9adca1a8e52e14267b970e274cf718e9573a86c53df50ca635d8d0a221db34b9f938e766d32c7b735d4efd2d089a0a4c2989b4e03457852d46d035ea5628ac54" },
                { "cs", "84c13d2fe73e96295e07ca37bffe5a243a8757ed09cb9f664510ff91f8cdca3aefef1ea1d69133aee8a662cea6fe69bad22d696bd27231f8c02400da6b87bd09" },
                { "cy", "abbde1398e0efa559ff77bf0c6bb85cfa4355524d1158de5149d956cc966c43599566dfcc29b68177fbfc7585089cf85bb8bb49979ee2a0cb6a2f193a1367cc8" },
                { "da", "2ac19e888e0a441f848fa69cb0c88a1b2cadb495f118c5c162612e9a187571bc2a38b49c94c8ba9c826c7d8df833b421492988736ccffb24db0dd7253292682e" },
                { "de", "6382941648c5ca5bbd26fbc452e1e20f80d60452eef438342ed74aa3f0bb2e034b61ab3096a48cd73af5d4e5eb0426008ee136338816ac4b8e954115c8fec40b" },
                { "dsb", "0a03b2afd0e85a630981ed0ee185e43b91d697d5ea412c14afb8b1dc0569aa31fcbb3dfc38fb2480f01025d73e9b9eaaa7b5b6c1b114c9d4cddd9850858b016d" },
                { "el", "6d9d63a50d83f66b43366ddccc7b6b1ca07352e90901ea530e16d86b1f2f337673529a1aef8c7083e9c2be0b9996716a8f57392bbb2e153b0c3c077e825882bc" },
                { "en-CA", "c4403bcb4777edcc94c76b1775a19151b124e56f782f69e37a85fa2190096ea84fccf5ef7415ac747f2be664dd8d571f920ffdbdb0b552b048e13ffdbcc42d2f" },
                { "en-GB", "bcaa57575a3be9405abcd5b81705f7134c2d88f2137aea60d5ccfe6057f2672560c10a984d94ce0a8d314c0e2930116f0275df81e1951b0736ed94a498e0221f" },
                { "en-US", "05466de06d4022d9d61f53d2b9ab15cace4f63167b2e9d9c25eab502bd5677e8d8640558ffc7d533a41099192556e5c2d07682001c173fe460a1a28a27afad9b" },
                { "eo", "1e971a383061c74c94b701a437d0558d5ed6a0b5edecfa41fc86d086f15aff013d22ea85e2174678993b392382db50e8831b3a365c09e0338254b95d6c33e4c9" },
                { "es-AR", "90e5d2fc572564a6de6129a56b39cbb8b29e587bfcb6c9eed1079b526b36cd74c9edbf65a6e071e21477c741b38f769a8510c5147837505bd30989759ef42611" },
                { "es-CL", "fd53f21edb957077511ef78f2d5ef92c311918022065dbff35bdc87f9ed12f729dbaf06446d8a6fcdb1d3b478c424437bfef9397138a5ad078642de28b086f56" },
                { "es-ES", "d8dc1557c7efa25db40c1f65daab42b804c6bdfd6ee72d012f4cdcabbc2cc41c27966155e12e998d70a1b0c42e85c18d429d1e11ad81b2b159ce7c8f843acae5" },
                { "es-MX", "fd9db7fb7b47586c7fbf4ea0a10d7716b37b374ad12b5b738678492e8e961315fb3c8970897e60c97556dd469e309264d9024860d0c942f5e4f73e07e7f08a72" },
                { "et", "28bd26fb02f89aaf29a4d9b97bd50390618e312ad5b591cef049ccfca051db8e7f07a856b3b9b2feef9a29d678d0c341d43022baac020f9beac9b375d2f091fd" },
                { "eu", "700a3d4ad544eeeb0d23bbf710bb8d2a061721fa6a8b16086d0d32ef92333b1e9a6c231c1e1ab822c125166243265ed824b99f79c6de1cfdee47af637382c6c9" },
                { "fa", "2c2a105d41f8e121c9bd7423a5732b314439f3bb934eda9664418f8486425d3743898b56a169cbacd338aa1f8dd6f63d19c343bab6bd6c38955384f43fb2ea77" },
                { "ff", "950ec09d26ff9718718439e8b49d181d3d8d2567313a2f5b9c30a75c78290d676bb02192a5cef81cf39cabce236196b7ec3be31bf387c8e000bdbb409a6a6cf6" },
                { "fi", "21bd86fb0ecb33df772f09dcb143594ba815e3ec64289318b33c2f0f37c162a03145a22dc39abc3875cb0c9cd6dedbb627e6756cfbfb0960423090c778abe620" },
                { "fr", "94e98d9acdc20b051d860357bc60a0ebe62a9ec916551658601794c336a1588e26fe415ee4826f241a60cfc29d1ac5d92335790a1cff718cd4acfe0b95541a90" },
                { "fur", "5d3ad878b74faa7a45d5101d345b13d845c54b8db8b7b23d4c1c305eb38cacd30e56d2af89812b49f62a34f48b7625feaac4b94c221afee1d6c8250a52c17794" },
                { "fy-NL", "b8c726e8983e9ab933b517d6f6278c1aa495f85916d12fad19c63363e5de4d16fa6dbbfc2ce21711a5c6dcf3e2354946a2fb15d5abf0f43088caa2e346493604" },
                { "ga-IE", "7133162b695fadd59f42594886d5c4b39fbf2fcc63116a265c0d25202d373fa7f0a66f2ebd85e9d363888cab82cc79118daadb2fa10ef556c5f1aa10ef491eeb" },
                { "gd", "52d116c077166353fe0da418e9d14c73f078fbacc24833502f436bc862fd28f0370107b3b4bf60d199d7c297709dfa3a64040eb15a94051ce5fba7251ee53958" },
                { "gl", "bf69afba0baf25a76b5438746ab330bf26447113c91b0211d4106dd1dc87f458536811390f23b8c1c372cd80d9c45b62cf8e05d71cf5d7f29d0f2eb10ada227c" },
                { "gn", "339ebd5eff14e9a6ffb0b702033289534e13c3f096d26c6a6bfb385383280ac497e53907dabe0741aecfbbd757388706ac1492a9020aede114257282545fa423" },
                { "gu-IN", "c3e2a2d8d633c8b0b6e3be82427f10234743517e1ea22f920009fee8ef2613d91a1b714b64387d0c0de241af086f16bbbd0084eab3c3f033760b1f210a43a062" },
                { "he", "076d1b114b4c6bf62fafd92883c3778d9475a25e7581e2062f7eb00502b4d01d35574d76955f4e461f2af43889a3fcc8475ee56659c064273e90fbb03c0e2915" },
                { "hi-IN", "61813c58eb4c534680d1760619b27dea4e9e8522bdeeeaf5e2ecba41519249df38535cd341a3f46fa1ae0048e8b7f74904176a28abfbbb9b9d5eb9ee34982a1c" },
                { "hr", "046a00f8075fdb5b224c345befe69859a36a7b1e0653e40dc705eff18f86e5c0e4bc323403ac4877e6d8e4a1d81b34c7c4c5679ef77a8f95d88c588e34eb4a0f" },
                { "hsb", "e3f995b705ec7f578b6d252121c357a497e00b4aa5317caae8af86f9664c548f7b3f0621ba21bbc04ad346c5e61cc9cb8f04d7de5d86e679576c1c696912f5fe" },
                { "hu", "782a5e18486e47740c21c09ecb269d5e656a0181636f15a230062dc177cf672633523f6609f8f4dcb3055f75bb0ba1476705cb1c5a422ac9382b350c8ce450e5" },
                { "hy-AM", "4cc8a3ad1096aa21426d48a141fee02dcb6eac4119d3e441a5ce77db8ec3bf6ebe93bfea38467dfad52966b80197d3dbbd886b7fadd87cddce57ad573630e961" },
                { "ia", "ae584cfff9b5c914cd10239bd5e542f303dce43d6f002e95a1d78c45137f9bb2bdfcd75f64e5b3fdd6c84ee47f40d36f462377c04c548e6e302e417f1b590bc5" },
                { "id", "d8953a58bdab247795bc8e24d1c057a0a837dcd34a2ad88051c1899948c3ca7dd4d54d25aac29363a8c258445d4f7caac998b39c727ff4a039b98d0ae01b9580" },
                { "is", "670104adaeed47d197435355212feb57ce862aedfa44ebb1cc69be8e02699f518a36afd269b8bcbef85c2805cc361592abacbcde227d8fd128cf83ba7bae9cde" },
                { "it", "aa82d13f79c6d6776443b96f806de02a549e3a9ff3820c3fe00b19b95ac242d2bf8c36c42dabdca957a4545b761dab2ecf0bdc41af177e4268078392ad4b0232" },
                { "ja", "4de0443fc17be6c59469074d6c7721093460292c3d0aaac216829e2d478728c83341a9cbb5c178e16b541a9923c21a8702f8f7a3ada6fd653fb3463287bebb44" },
                { "ka", "ef92873f3e19b0e62c2746ae3d812237f9f9ba8770105ab6f5528ac57d45e9ae7f2d725d6d82e00139fc044d00ec823a32f8761fb9e829a166dd1eaf4d482a4c" },
                { "kab", "3f3c3ec0ed11336b5967ef9e3f263580c72350c71e09a2e75fa0f29bb66247e4fbf6bd5b10112b46725d0376a5fc35ae9ca8a9d94cfab86926cf32dccab046d8" },
                { "kk", "72d938ab4b083026fe926f482dc78ce1b03a9e3c205e1f9b57e1307665f8f0d0892680c3eafad8154ad9310e4803fde09636929acf2157506f748b76904c90a3" },
                { "km", "7f65ae7f6c2cc5d763c66e2688842df8551f48422972ae75ae2719d72b64568bc05828316cccd6154fbd61c4bbb0c2c10ceac229f474aacd8de9806c99714787" },
                { "kn", "b0578a7e42c9f004e4f6ee33836752d1b6903c8cf5b5d58db3952e965ba0a86419cf65d5e0ee84b64f0318bb20d1f4c2b9efef425f354cc81fc6e7edf99248f9" },
                { "ko", "a1e61f8d3ec5f8f2d7278089cba325db58659259395d66acca531a912ae9c942f6b00d6c416d25ecb1de4ec3df4db077e1dadda62178010f4a70fb93353f740e" },
                { "lij", "54f3939565edb130cb25aa197dffcedaf8281e248f03848837ebf63c892e5e9b7a1b136f7ed8cd3aeb8ed2bab136c464ddc75b96f24fbed30b6695016e4700c7" },
                { "lt", "a2fd9aaf68aafbd8756bc3a9a9b24ba1c02ec1d8b4a9c2d8cc9e34b8ff9f22af8b1f10670759cac4e6c422f7f2ea7093a04a9927040dbb64895b4e014e11ee1d" },
                { "lv", "3d544be9698c7d25c9bee039be8d0dab9ff06a05626faffa237d0e77842584bafbb8b2ccbc276d8a6d89a5243de29ec265196ae9e52978a93b78199e021d9a67" },
                { "mk", "914cfabf855f6abfe5ec3b06a4cee2a2596fe67e8621cde0380c40888ce214ab75ef9d7483472294bf9528cef539073031b2b645e8d8a7dea3a0f1b7380b831e" },
                { "mr", "86e236d85f4159e2708de36ce8c7ca2156e25354756f22a75cb97e86e162b3d9c013fdafc105c1469aef2886fb503e89ddc1b0a7e86b7622062e1732eeae85b9" },
                { "ms", "705c28f5517d1678bde021352ca8693848aaf42972f5f52851192ade88dd79f3123758e039c4cc781be5e4b3a734e85f70b4302e5f1baee8348de3972bc678ae" },
                { "my", "247f1b8b2beef3ffdf02d828ea78bef7fc6c9b81122c1c0ecf0aa15fa65f32bfa011826bffbcace73066ff107e6f0cdcb0d2731690c1d628467a7c85867e539a" },
                { "nb-NO", "ef98edf78129ce16bbd52ca6ffd916332cb3bcec1ecd2c0af07603eef1d92f0397e82a03a5d16f8da1801d09db0bedf76a21d5d996cad50824bd05263059b921" },
                { "ne-NP", "84e473fcf373996ef1726360b5df153a60d19381733b8c0ee153f26041a8583b5ecdf7a53c4fc6d362a0f78ccbf6f4f82bccb65de721018aa9ba73c349711ffc" },
                { "nl", "ce79d15a3d285394e9f508fed1b97721714714dfcf588cc015e639a41b709cc8be42322cb5d1e7ef2ba806c4471325e36e41db4fce8aa295ac92f349b2b126f1" },
                { "nn-NO", "ed830bed5f16f712eed0aa2f87ce94168e427fef2891d926d939dd14770977de8bb2c9ac388fcc9bf7fdb9dcb8f92134497b6e3ca71633a7aaf9d315769e08c7" },
                { "oc", "215fdfafa215df4fd8115fa83a403471459e7a1e8c63365afca7b4041602b62a95c90280ab0dbc72b61f609d8b2ce02cc863b467973b3dcb92270c9a44c4f25a" },
                { "pa-IN", "b670867d4463bb18fe41cbd41574959aa406930623e641b48de46ac446561aaa1f7792541bc6a93674b905502d3d4f27817925af16430ff19e54fa07efd822a0" },
                { "pl", "ed2bc87443eaec0bb0ce2e7c0551470000bb1c9002967d60cf67808f286127b9e9d45e95c25ba70c80f21312147b074d62526adb6a7762c29cbfb686dba50261" },
                { "pt-BR", "ea7f097184e7dc0dd56c44a00de6e2f93e6e653ab00c9d473541e776d51da140e6611d51a762cf7de4ba9f983b707033643b2b05114cfa69677101a71cbf8b08" },
                { "pt-PT", "f09f2a4a6d38b3334165c10e6681a1ce5a005c76a5b9018c45e02d4500eed8cfab9ba7de080eba4a92ce54777d9e1aba5931dddf731de7fd8c0f9126efff43e1" },
                { "rm", "af2e229412405e2d6952dbb4b5ff1e0e1110f7702237c10686c6539500ca717aa0d30fc36df4e8b7bdaa9c0f03bdcff723582f13066eec82f2a649f01ecebc5e" },
                { "ro", "762b1ec109262526d55b2593593dd0f84b7e1b79e127c037664c87b0a2aa79ea5de618b4771e7fea627a22d3583bdb1e19ae7a5d73e2d51414daf5403a72361c" },
                { "ru", "5cb3e26b4c7b9a0a2f552d960ec91dc200926416d9f830d995d0eafc83fe65dcc4d0ee2249f9a284c7aa9c0f9428dc13e98c3644128c071bb0e767ba15b5ba13" },
                { "sat", "3ac7776f0222cdb3cdae8fbfa8833be26fa54e7fe106b3244553885eecf8ac6d08e49190027be9b9a70b7b078d5561d2af6f11235833bd656c7a868053eb4671" },
                { "sc", "427f1e9c066cf3345b37399d96bcd696fba230f5b9333020ad8f856006896b3616b9a95273bbbb1508eb7049cafd340683ce2d87c1a5674c702dabc907a1bae2" },
                { "sco", "a1a859b408a16536f84fc1b5ec3f57662f0ac92423b73f3b1bea602f487ab0fd98bc15035eb468b2481e18d13c73c37806f5b982eca3e8989489b65689a2509a" },
                { "si", "370aa478eaa7af929224d18d9b19bb0e05de8fb874f4330a710b200c7bc2a34e243fcc8eea19bbe36099e547e092b82984942367047f0afd223f09d5d4009a03" },
                { "sk", "9a55ad735c1a617e5a4fd4ff43299caeeaa9035779730de585add11dbbd00d9ae6ae651ed8bf59aa82fb53c1e63237f37c6d91ce581d3e62b6bf4caf94784002" },
                { "sl", "3cf08c7918b7359fdcb2f7816c326d3feb1e5a07f5743bd38f4bc589eccf160852a3b110fd60ee417eb205a71e038075ad2470c1a5326ed0f3dac6b79e98935a" },
                { "son", "210db4b3a7028ff10e5dd020f556545e8a5d8edfe6b9c02e72850d09ae95c6bbee200bd90e6102656fd2fb053ed6c1c5759a50f79328049230cc847fb483eaca" },
                { "sq", "0a6ea6345c1f063ce320d48c0b46d395a94d17b869268b85a2045ba34b06b651bf3d32e0120a2771799aeb8b4ae0281fce186a0eaa64090a5695c4821e3f1458" },
                { "sr", "2b3fa0f51aa8011bd9802509f09e8ccad68530af1c02572bdba365c409c4012b51108e007cd69e98057ba84a20b2b6b831169f34a2917af0b854874437550e71" },
                { "sv-SE", "10063fb984dac8a6549a390b5c72718f7e8ba797bd9d32510a389b4f0c570aebb20fde376bf0ae33fea928eacf4140bd4ab66a69b709020c61d099d00f2c2fab" },
                { "szl", "05debd6019fd85d8bb7ba34748bbc2885721b744195fa137b05fe02079bbb012474e51ab88e3c8b75c6f4a6ad3d052d70bb131f6df77e06cef514a9b258050a5" },
                { "ta", "597a5cd8cafc746e43c751f9e5340f254d6d0f42df9415a62ad5cc5e3eed81ffe4c978e90a1b2abc458b88f8e5404ae93e16c9d4a9f667c069a672aca68621c6" },
                { "te", "5a3aca891f3df49bb034493d95b9d25b6593a82575ba8b340e5d4c820af018a2add93c4d9563bda403f30a12e60c531d1889146a8e8dd5e278523f948123cd5f" },
                { "tg", "0469b7aaf267af915241a9d969fb268bf9c11d876696f75be7ffb7b2a1a0b44ab8bf97a84f5a09819547a6b34f712f00896992b335f70a008d6d5e192f86ae5a" },
                { "th", "0f6ecbed2acb96ef016eeb747a592b30f25d306bfaae2da43739b45a66908512469234399e4300f64b89cef004487a2f8cd326a63bbb86452eb18a70fd814d02" },
                { "tl", "56386033491f0e6e84784d696dd0f21888046a49ff1556b683e0fc2025832fc50e22a8632d5851e1ce3b64317f9a72b8478aaf99e4876a3d94a21d7ae502848e" },
                { "tr", "1573ab93e8417ba31d3b37a9eb30a6431763dc18335f431bfb3d781f69e6707f1bff319731d0dc841528117726297f305a63117958f4bafd1f2c30487368a5e6" },
                { "trs", "a97dbe9c379ef54da9656a47b71ecdf3c60c008c862e08f5aaade40c2973f78393bb84f04f0411596e9e0d4d443b212db280962ca67c1b2aa156d678c60e4e24" },
                { "uk", "adafb8e9425a6b38111ddb6b85e40cadcf803838c9d1c934cdb33f9926d2441f1cc667ff92a7e935c3ef547a173f6788d2919dc3e6ed5cd1f06ac562c944d7ee" },
                { "ur", "3e2a0aac2666df982108db8df5e30f772066b95dd5579183faa20c449b6e3da73314d8ffc7217d4bb6be10ce8068ccf0ee489d72eb00d39577567b0d2a18dafa" },
                { "uz", "02899c5243400dea0ce275b2f8933a8acd38b9d86c9dcac3007adcc7ca82ca7db9941d1db0e7082ad190c6a252ab0b90ffe29d551fa03dfcec65c38d44965421" },
                { "vi", "ef430fe09322b36fdb30418008678fd6facf4081e26b2a7416d879496e09a00fd3c1c639186c85141898cc27aff96c9eabb315719cb2f7ca1b266a3f6aa84eed" },
                { "xh", "aa6111564173c56b726e89d7a4cc387311f16334f81a064bcc68c12425a00a5092f91a5ff247dbb32203b86f628d7c0c9fabe2db3ac1534d00f67c5481364d98" },
                { "zh-CN", "44104113d91b695065b42ac0be5b07ba3eb3043d73e8408a50ff63c5b323a912aabcd65ae0eb41a31483f7c8e4c43dfeaa94b219c23ce1d3a7c1fd26cedbf419" },
                { "zh-TW", "a6ab622ba2f1c1d7f26f2487f8a450efb7b33cfeae2022409e03fe1ff67fe3367d42a98fbccdf0a15216f4e4becdcbbc5cc156038a5ddf3e40456201e1cf7cb4" }
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
