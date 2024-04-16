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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "126.0b1";

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
            // https://ftp.mozilla.org/pub/devedition/releases/126.0b1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "570b17b67a27c591bab4dda61e375fc4ea645451c29b6061bd858f512d9bfac22434cdab6ecde498bb386779062d6bf71af52ec81b44a2e83d68d8f0afd67baf" },
                { "af", "5e1f448bd1a6eedb7c77471ad64b544bd1721f8fd4b5c7614ab6bb4731e8488c986c77b5350fcdc7086a2add574139a2ef5486430fdc504b1cc50c3cfa962097" },
                { "an", "43bcb326a11b9811fbf5c3787c5ddd8dc262db3cf993b1f5fb8e4088c8ad4df1986ac685e9a8ad0db9083198a2cdff5c4a8036a0ebb503c0dadf64dca6b73d65" },
                { "ar", "535af951775276c58d3ab391f5ac3143b4bcb2dc240768c5da32f0306a6cc8b1e84a1e83664f10e809ea46fea35509ebe56f31d0bcada467732e752f3006f8b4" },
                { "ast", "73cde5e72cc685fcf01a17d597456f6d91d8ee48a856eecffbad6c9636230b2100b3ee14c81d9bd5a300f589dae43960c823b65c6e63d95b281dfd208f129633" },
                { "az", "52541c9674ffc4efa5a1dd1c8e0ab763238e986bbc2955bf4a3e543e7b7e8123c500de16271a149e8b7dd05bac87e55b8b921f237fdd5c61e2e7e3e473fd2c87" },
                { "be", "e03721723021e8cf93e29dabe18591ed03048172255acafa6aa711591177bafb1b91dfaba91db618b09fb3fb6de3963e80eca6b80b81d3f2a5b6279058652ca5" },
                { "bg", "ae495e89c60526aba53fcb338a083ecbc38218d50d8fbb5f29f85752f69c245bb16898f939e0e0fd69cd0a76c499da38f8fcd1332d53b33d29d82a34a76ef12f" },
                { "bn", "223947c9a33aa538ffc1c331d76e1f71697776cb161f8e32411f75fa5c2f68fbb829c37850bfa1a0b19d4f318536605c46b31af7e2ada1c607dc3fe72a46d576" },
                { "br", "e6419434f1c3c140bb50f905a85dec2c44c80c4bf3227b093a5f9bacda09ec42baa24855a54fd4075e7f72c8a258c57baa9c874392a04a55aa33fb6ac7118823" },
                { "bs", "5fe0bfedc4d86fd9d5e1313810a8ba39561bdb4759f092f735465efbc49c90a78e6cbe963a8403cc9e01f9e5f9e367090b2ab25e54ed0e8dd718c07cbac0f265" },
                { "ca", "fca8e36719514dca9035e5ba753efa7bb8082da7e857272986779168a77ad30cd582fea295596d44e4d169455b9eafb5dbc21e52fdf83e94eef2d6529d51db16" },
                { "cak", "f8e26ad636106d939081040595b95b60cef1cfa67eef5a83560c8e0600572f0f5e1899495080ee8d0c2d3012fa2de1c4a56c3493ec6b3b9afde841939e4d47f9" },
                { "cs", "1d066f8943315ceb0001b16fb3127edf151047c07c18a1da2fbc022fe484e02600053f8857208af3763ae30bca2de505c89ab04c8dd2fbbfa50534616219fd28" },
                { "cy", "345432b6fe69fa7bb738c6dc138b30b14f4dd8b8dc5cbf32dee3dad3815ac6ad229e1e4873f222003ff19954f548e89179ab55c376fce5824c2d8a9f9f776e14" },
                { "da", "f1552ed370dc5366b8940f204a5e831d80c961be359dbe80d5437c8cabbd392820c6b7e8e771550fa78ae5fde340dfb7b5f13ae36cbfbcc1ca26a604e92d4cb9" },
                { "de", "d6038937b4a079ac5c55e48e5ab676459aac1f8c647dcdd0c78fd15719d83bd7c44a4694e574f33d94415da603eeff310f72fe082852be47d7724c7f42c20567" },
                { "dsb", "7b05b2dd1b20b1e35a8675ad984880cdfe62d067661a970a2c687d07b326a00d92b17014892f5e1af9bd059dfe049d660eb558daf868781c0007a48324bd0a70" },
                { "el", "3fe52cb7128bd97eac2978978a606124f3dc991f3c48064d18f3e6bbe329dcc462a7d7fa13f923da536d6180aba7136677f27d644b08a3c0ae81e4d54439d54a" },
                { "en-CA", "c842dc1dde80dbc331f38bd75f1c76af5ee9fd3ea0abb3506ad8547c4f26883faf1e26dda949b543a0d4237b34a49abfd8ad13a2d39a15b0f48a93c1ca54fbb0" },
                { "en-GB", "74dfb3a4157991a177ca50d44e8157484dcf5e03966ad9fa6dd35baf4270d626ca6d3d3f06ee71f0872c03d00270ab6deebdfefadd184be79116fe9028de09be" },
                { "en-US", "c4de9b51338c9c3793bfa895df0c741b74e8ad5e443a563c6084bd567219c7259690518e58d1db7e7babd69021013f8da93d1e9f88e85988b1527c33cc099f42" },
                { "eo", "7a570bf8fd6ddfeed91b9532234120f9211bfb41bc972b0c4993a318cb4ed29ebad7e06c76736a4c4b907bba2d4392891d7b5da1a6b8b9dbac00ac34eaf16c27" },
                { "es-AR", "603b0e3b03bf13b624d3c48838679c214efb2460cb2b76954e7b97a70a71cac876ea1171b8befda6bdbee13bafc49a6126b9d2654747041476822b984e8a2762" },
                { "es-CL", "b6744dd256e68d5449cf7b12eaa93e3fd7fe9e49225fcd1fe13f5550c5aceac41910018d49d19a4caf01d76822c7cca1e5a626a4e76b55ba13e9f0ff0b03454e" },
                { "es-ES", "b45bc784c631bd82c4f966863152907ac6ef09262fa652a77e0f48da9df6919f30644ff5a34041f09caec765a69c7edb5e73b51d793a7752d4bfe2b93caf0057" },
                { "es-MX", "03b1c0b69df2758a9abeaf994efd7e0763fc53bc487fd01520f532252f993ddee8bb80737c4f2de6cf2b5c9bac8921aea6edc024c6c784520bcbbee00884c5fd" },
                { "et", "cfc59ec61f72840a7f236b30a1747c28d8abe41728b0cba632b3b9eba76e21ecc974bc2b3a7afecaba7a980163ee990be777fc7911b99d7d6b4643098244725c" },
                { "eu", "7a2c82542751e501f460e5ce1c04c00628c71ed3dc3f50ac66f545e66e1c2f1ff487d31ca2ffab0a9a61093a47e4e5789e986fae67628a71cbc738ebc8a4acd0" },
                { "fa", "2b75b8d591abdb630dea307bb18933a1e02fa70592ab2805c78ca5954cdd1c9acd41b4426f023f161f6eeeb47fca0d2952eea28aad1b78f10a7159441b877e19" },
                { "ff", "dd038e5a6f992a2c78848804ac317fd0a088c426c9bf768cc748c0bfc03ff5419bc6c21421c51052fdd89b0c2839a455740cea0c4b17673b30368a5b9cb795de" },
                { "fi", "abae2d7d3a1418994025fa647fa61c5f5317fe958919ab5b6428dbce34208c8173808915ccf86302bd02cfb55fa6fd16d24e91028ca87e72bc97e039c73c1560" },
                { "fr", "c4b00f40d2b748bde5049d079fa26407c0acc526290ee2193def2078a9e77293aaebfb62857f7748c844903c42d7b6cc6426d25643b2e1dce073d605c9cdd2ef" },
                { "fur", "84f41d307c8c9f43339c500536a84eec8c20a7aad272995b8cace3752c1418c18aeec8728d5d50b939035b8cad80e6e10dfafefc39e96a6d350a1369e3e33d22" },
                { "fy-NL", "d54663a9cf49f1d0f9f5870d0d5f0b9a247d51841c942ec7fadb930d7f8c87406ac007d86f1cffc9c947a2ad987399e97212cb8275e028370232ca8d02df7997" },
                { "ga-IE", "2c972442a5f6199aff5d873374ac491356471d59e9a2f2ea19fc795bfddb027ed7e18454208f124f113ff088ce6375827d3ba7baceccda5d134dfa56b2378b08" },
                { "gd", "c6d5cd5baf6c65cefeab1c3a0a9d7467a6bc6ee2ca0ef49e065018ef8a6e6762d0e8684bf39327a3b462d3503fdbf51f1ef02c741954e8975b1d35adb5c7459b" },
                { "gl", "6edfb61a1d43f30765795fbf2285c00b5e11985a074dabfbe7273a04826b0c50192f605f581fbb73c4557137997b0e42fa0be771e6358ecf851cd3f348787ffa" },
                { "gn", "2ca9146afb133021a58b449fc420c97c55b06b0b2ecae274dece46578c8436da2cde945a83cfe93318c1ba7e7005e57454f432e77a09383ef52e93b4aab12a81" },
                { "gu-IN", "032748bcceae06c353d53169da75551dda0f8adc3b8aebc2e3c05448036745c108a02f682d51f36bcaaf4379bcf604f8ae54042af13413b6aa2bbc58edccb178" },
                { "he", "10dc2911dbec64b3936587038a5c9bbcb874a1abb8bcc5cb3b54b4a23ce7369d71fbd90b257e43bc2713121d514bf7439e4419cb9f59bdcf41e8c64f75ed63ba" },
                { "hi-IN", "5a65dfebeb7abea0c0b0f97a814f95b7f261b76d90cc53f4906f8001ad6f97809b4205d583bb01e9dd6abb14817387fd8f7c3ae9704fcdbbcecd8c8e1de1b593" },
                { "hr", "5155d58a1d328405237a264f54ec1fea43fc399b63b50e34f840f83978247e70e6b339cb9fdffa9c876f564d7d7d9a278e0faecf834edaf8d4c9423da19e8426" },
                { "hsb", "a5c5515a27184d9234709357b01fcd784dc7088b3b8443865844b0735e8d9d0d82ef085825ba7c60148e83a9b25b719f73068003b0e97d01ce4d0e5eb58cf7fc" },
                { "hu", "404d3f51314ebdcc60eb5adfb5cc97981a636b1c209142cdf6c1dff305a124b79871dff6bc45bd277cfc4c3d926103e8dc34d9ef04f84809422712c581b66c8f" },
                { "hy-AM", "711735678a51c63a2fe226d63fcd53c4d7cbad1119fe82a3c9080279c75ff28ce862661a34df32267832ee80972a322f9290093591cf2b6d21b073b4e00b5c0d" },
                { "ia", "c0f3df07cb6541adc113c73915c1930236a982d21e50fde5d4bd9fabbd3a82aeb29c56fac16ee3938faefbcd795d055cfb1952ba00ce93372f6489806698fcb7" },
                { "id", "8fe2ab2ae9cc0de303d6712af0344bac1c2456fb5ccade77e86f8f08751d335c8a18c00f36c6dc0d9d8e9f9a8d9f8f5da4a6323f786e416c932336c1c9733d96" },
                { "is", "1b5fee478665868b719ef492dd0d254c0deb57b910012ae99cbf8e3d4ab4a82486a379a9e822489d0f450bdb20220f16c0218ba5a3c2f419028ccca3719d4e49" },
                { "it", "a76feb327d89223d43a949cd82ead9e245a2fcc50b69e9c8d53a44c475519a5bda636003526c0bcd7946158bd0d8268f6fe82794b4c7e781d17371cbb5eb87b1" },
                { "ja", "8543349333333539932e1d817c0e90bd2730715f34347ddc6659eb5862506d66ffe041d576d070197cdff0e7117db21230cae5ad03fbdb089bfa9d09acb9367b" },
                { "ka", "a549c1d17c3589c9f9200923df89b0fc9ab35fdf13ab46fbbffd1edbb19f93f62c6c2c738fbd2c2a7c173432d8851395317d53c68bd7f720e01ef40ea4d68b00" },
                { "kab", "8b1e0c81def75564301de4701355f849937416815be5d0d4ed99a32bad18517f550fba62e3f09afa2d5b8329591b62ed184ac9410fa7b313e3cd3c8d87f53bed" },
                { "kk", "923442a98d02d7f40c10ba926f7645f0ac832bf4a06024c813e06f152d053e1b092f7967a821bf0acda5f83846a0111e7fb343891bbc9b84245bcd8d8a20e764" },
                { "km", "216f6942176a277126c0bdbebcaa5d026aeb496f39e3c21c89c2057fd585caa69dc8319204713e5253ba85068d463c5d961a396ffb3d5a90e63c588f4ae67a77" },
                { "kn", "87278306860408f1b853785c68c570370c65b1b0b295a4e294cfbf93b44f3aa530643679cfd8a1bbdec7f442cf97143d117de4255c9cc965b6c33792157a0852" },
                { "ko", "5012ba6e9bdcc2a780e1a2e370ef297e2768f94cbc258ab95d234bc06e02e9a66d026cff4bceb3727e3e70604a60843c555a68c0e2378a812e50757a921ecf10" },
                { "lij", "974e288538d4a3706b472dbbde01937614f1d2d37ccaed8dc4526264d9bca76d34bef27474d3bddbbb407a453cbf348e3bee0f5c27de430939acd5180d89445f" },
                { "lt", "371ba13fe107d08f255bd768ee820b3c0e24c604c3cbb914c8163d218da83c40444ede6a506d5da96cd991f319899ea1e7d6115df8e242c75771b1400e7c6f1a" },
                { "lv", "d7916201ddda03f244f88b1ddff54c0c8b224376c6ee5847449e32e9d18afcf73a28001144ace9afca07a6e91425703faf09aded2e87b0eee63cbbcc70b4a12d" },
                { "mk", "177e01069f8c3c73526e8d35c755e549b8b55c4b65cf12afedb75edf9b13faaf6dddbdd7e95e3fea90a260662fa72bd770a163562e649ef3f51bfe6133d7000a" },
                { "mr", "ffb8b8b57f06969e1bdb82fa6cf5e13f0d4fcf96e168dcfeeaab1cea3e4b9d7da4bcc0f030b0921613209e427f472d995df44253207ea4589145e8600ecc8956" },
                { "ms", "c932928c3aca610597b37deaf5c4d7487b49b1ee18586d9f26e8776b62043d82b6aca1f4709fb1622fbe650b9563c4e53337293ce34e692806fe4f18573d8ff0" },
                { "my", "87b59cd054949ca107e95078bc32f0916e108083ab1202ea7e4de46e54091aa47895801b602a5a234d66690dfb82457d43e214b113b39ba176031141b03c2301" },
                { "nb-NO", "8028ab241712560e514806dadc5e12a8802ad999e31588ad8d71c736001faaf0eeeaae3563b94479f5f08691b84a74046d59693cb29de5503c6e1b2e123bf44a" },
                { "ne-NP", "3cc77adb518d1544948e58b59750a80217a914841bc4317a0e55676d1dd6a70c11257e39924afec521a9c9661475db9b1e273b9ad5631fecbf088c62d1799099" },
                { "nl", "c1aed2370016613ee58148d8a228f8b81d5c41730b15e4bd796517f5a14bb34e06e3c39e6f278bf5df4e48e78bfca4f92ba90bb52d6796b06fcf751cf6dad8d9" },
                { "nn-NO", "23edbae4f9daeae5549641e3e2a0356564d4f4371aef60276fea89caf578c493671442b5b6328469f0897cf097118c3410add2d0229604056d147d34952049d4" },
                { "oc", "bfbde082708d7752ee59e532860cc7cde7f4ab6aa7140aa9aa018aee4b86ffd08ddb62ea85a472a5b2c9045ca8e2a7c293727a487eab682ce23989155fd0b279" },
                { "pa-IN", "b6ec99230d2ff8fe2edd7a47076f74c3a5adc510c681d7525161db856fc592137cad91c5fbeab9ba7b49b3f934a6c9c018d82ef5abac31a684078a478b2ad604" },
                { "pl", "1c4b4da12ef89b3e5cdce6b94c630e1f2bf04160795ee956ff93a7f44f964a2876fd5b53bd065f3f23ac227fab94ad4045b47628fc38412f9c1a1a75b9ad0e8a" },
                { "pt-BR", "b7f17e5fb5dfffb1402731678eafe1242f9aa016b750b4222983367e86f83fa2fcaf5608e43b5016b991ae4da0c74dc0d5e0c94397e5a911c2dc25ffac9edf83" },
                { "pt-PT", "1c04df5c34f4c74451bdfa85dad4784bccbfdc6df9f73e5a35bf829b4b6d61bfc496a13162fe720c5ac82578255dd3033f44420df2169d851616efd251e82e10" },
                { "rm", "b8011dc27d2a9e1b79b8a9a12a02a89b60313942954e3ba11fe2f279d1bf260db1fe3a0ce5b7c4d8f5f19a452c9f2dedd322c78cb13ffceb89428cd2ae9cab06" },
                { "ro", "d859a76bec962dcdf2329f7189426f3c7e69af740eed1b9930e9aca7556f5ee471627b5e8ad9431426a526b277f9a5e489859e7da849f22eae1ae19197861878" },
                { "ru", "2667e3677f2b12c9a634d78db63d40413b212cbb5649eedcf6039a3a73fb5f8521768cbaa8f53af33595c0f148399b6bcb85c11105cb5e31a8cbdd6e62b4810b" },
                { "sat", "da20f2bc3b5251b1069e024edf161cf6be5f9200781ff44681b580fba007832ae8537fbf5d65acc6432c3c96ad4f771cfc8f5294f7c1cbcd96d811d6596ac935" },
                { "sc", "1a77fe0966c1537fa538a51ebb75c8dfbe002fb59e0a2dcb769e391190f4e4304efc588cb516432240427883c41ccd71cd98b778b85b22e40b9eaa34fe16491b" },
                { "sco", "6ee44b51e868d09ff2838e7dd948c49d2a83ff036b68d0225c98da4653bf23e811f34ad3bab836401a7c60654cada63d97493e757b1317d233216d7506f043f4" },
                { "si", "edefa0ddb1a786bdc5bd2fdf629a93180b5a254199a51513cbe13ea631832149c46b22c295a98b7f29bda749cebec16dcfef2cdb47cab7ebeaf9e23bed7d2361" },
                { "sk", "4ac3f74c3380a3c52ab9fcae6a30b898e73c567a450d88db1c29e4850457aa070aa4c504f2c5f33485969fe6bfad010dd76bc716747a07c9e9088d5346342867" },
                { "sl", "017c4d9a070d080bd3d9c08384ed30b25c61a814fd8249b84524c2d3cef72588bbe90875eea441ab979a5db5d58ae2fc002ebb297ccb3f01070fb2b273e49af0" },
                { "son", "be3c6f40b59e0d4ff7791dba65d752d18e3426d413c1fa6b10f55eda527f0125f4a23b7f67716bd6c4d4f14c0a7d9056f43cf66bdc3b22620810d822bb005c74" },
                { "sq", "89c5784f4bbf4b9b90c0428db5c32fc7652e12288670fbdbfb4aac6c4e3df61fe5c6638d89083d101bbdb63a8a616c915fd87ac5a42af15c7a420f0240b6d260" },
                { "sr", "272b8380dbb49f8821e5755f813741ab37362b513d8b511fff018927548a61103e8a2116f0c43c89b694f51c1b439ed8ed1afbe201d198561302948a19749144" },
                { "sv-SE", "08a147b79f9414240bb167e08aacc05673ee4262d8e5846239f7e6a7f5af8c33fb22488fa0d2814033f8bd81b304db826fcf7675959f9cd67ddbba3b12d78760" },
                { "szl", "e98d4bb8a766244477613938ddeb950d34455f79991477b2dcd4c9388f6547842f560d25aa91a60ab70ae17af7ecc9613ef6f7bd72c4b93218c74fd651e37f38" },
                { "ta", "c21246e6c92f50c7bc7c4e044da03d703458d467a92aa2af50330ee9dd0dc17a30593cc1205edd5d53f8c3fde68df59efd09e3f2507b4a38ccf7894a1223d36d" },
                { "te", "1157b649c99d1c21ba88ef03dfad08402597b73679b4a6f6dcd46cd86df00482c9802d7cf22603274dd2f5c934f1d8514ad9f80a6cb3536e9f596296a479e68a" },
                { "tg", "d6d927c237a1eece60527840f98600dfe6d49467575db75eba9fdfae48a00f392a064f115af32e317e9aa7afeab3a89eebed95184d2c41aad556bf6113a0a945" },
                { "th", "46897825a31c78c814de9cf542adb8326480050214605cc5299e390f8da312c6a9ef132d33d7330c639d007e622e7e05edacd1d444f70f2329a20c1b6adf75ae" },
                { "tl", "d17b8510bf48222b5c77b54385e09edd43f6af15e7c2914f9014f2a0025bf130cdf884aef2663bc839f29726b62dea838b2bd15a7a5751c914b19cd0168e2480" },
                { "tr", "b06b26450d31adff265a20b0a13ee32626afb47b52d31871fd6cad45773f2e3a4fc905b2a855dbf80957e0b118113ca82940f042e43d4e706dfa565c6caa14ea" },
                { "trs", "fcb3d2889791bd9309e8592829cad924140ea78655f10df266a2bfd9b1c8813017206eb01e618831ce44a8b70ed64ba5a27b07804a6957773ad727576ba2a256" },
                { "uk", "fc559a457d50bd4c917412602041630a6dad24075883eeb03aa027118aa58e00bc105638d69613563cd4b96f01229dd26f595d03d022519b1874dce6ca18cc11" },
                { "ur", "c055cae1101b8275eba670e96570aa58a987037ccd752db757a37f26c52a7159d23528fb74b4907e38da3b6110700b5866b75364ce8e9b2a9ce4756f93d0bc95" },
                { "uz", "2d1d274d60f8e842bc7ef724f71eecb0ccead688870853f34f83d2bd1fabccec84c1e2c1ff55c56bee38978f418100b6ae81afb3473106aaf1faf02e07f66509" },
                { "vi", "dece7e6dd02159eb4c119fbc82aa89b5720654cfa3fe576abcf70a592e67e26424f2fa367d7489a0095584bf316c48d074dcf499d438883f52348fa4ab2edff4" },
                { "xh", "1be438609e8c9387fabe1b3f9cd38bfbcce1d316e29afd56800ee36aa24da60a4c55df386fcbd3e1e0c312bebdab90226adeec20f23079be6a57d302871e3309" },
                { "zh-CN", "9ca273e93d2df7f7d20adf53248e7c73b67efa3731c2b26b1c11b87f3037966ea87b6d29e327c3493b83823da19b7dc88b4aad2c1555fc731588c5c25f94e5ec" },
                { "zh-TW", "996a74872c2ff24b1ba0b5daab08cf399a65332089a56fbeb27c48de1bef6d1ac61d7bbe11a8a564e6bc587416b5e4a7ad3f1eaa66af9613a7f26d6b92f8916a" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/126.0b1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "de80b0d26d3ecc6e7b70e5ecdd6ac2180a7ea2bde257a035a76c0b71f0640e40ba6a823c535f42dac9a9c5fa7a5758fe45b3af083e18a3ac9781cb1ed3f7eb58" },
                { "af", "e6d76dfbe8a28e719c530b1e30a8fa33d1946cdcc0a394016ffdfef6f26091517a48a47404483bd8c3742f02168e332c7d5f151fab45b274541d624789713569" },
                { "an", "e0090427d111639802e4fb59eb271b152c6c3ddcf76c711f25819f5c671d4aed27ebe9953ec0b7c05606322975cc2a2ace6ab283f8644735fcf616518ecaa729" },
                { "ar", "25790afcdcae115212cfee48260848f683d3eb9fab8af7a02e5f5ae640c2e1cac5a75aa6e160328e708e64b2f2d401464e083f23635ce844a202a27059738b3b" },
                { "ast", "399a5fb6ac955f612f552b05d097356c0d459c5893d1319dc29e0dc2792acf2819a6e19f9eb70db9f9bc5186b2ab1e53b7745f9c5cf40f514fcaef885a8b83ee" },
                { "az", "8a252dc9dc9cf5da11bb9a494f15453a356fa4b0f0ad7ff0f5e4be688eb13bf0da58089f1164ac8a90da42f49fec22fc88229347ab01e0699d7b9e99bca35d75" },
                { "be", "27b68dacd0f5e1b9e3fdd431f2bdb1907f6429b1e02de59af212fc6526b00f0d6c02908eb4b0562f7de62a05890c3b621a35420ea01b53f3c5498e8afc89be81" },
                { "bg", "9de47d1e750dd110ba741770f22e854ccbcfe83689adbed92679f67a07e67ce229c519d7a9be8f040c0c9c577270a345d91e02e017c0b020627ef59a2c6402a8" },
                { "bn", "578744879b3acad75847c0b6888dcbf3cce426084a0814fb3529f16df90d63b36e50beddd551deb2e101b9dee56b6f9b40eef782eb8c9dc0d5959f2d7bc33f77" },
                { "br", "e9d04172fd6c3923ec0e45cfbd601c8e073e0c3c5bcc57fb7816322adc1b4fcda39ed64dab4ba55837a38ad78d9579b6522174eefd4f3e849263c5c4c929d2b9" },
                { "bs", "f3b6f85092c3bd6dbbb8be4c1112a1427b688deb3bcc60a78023f4c81f9245d3203ed4deb35d805bb493f6836b0e092d39b835f89d868590915b3d03cb3f6361" },
                { "ca", "9f838d5d279cde653b46af9d37f8bbf53267115404cf2dc71cc3acff526143d9cbdf9d441a344800a654398665e618a8e5286d200ee830f940b0100d15a0ea50" },
                { "cak", "66644a321a630206edcc421a5f2eaba2de87ac529d357026bc976598a90e6c2b715db3ee25dcfe8ec91fc5c0cb8e265a7e1514d48a7a6eb00cbeecc10198f338" },
                { "cs", "1cdaad6164452672bab0adcf22891ee4d839325e12ab5abe76a7d4a7d4bcf4ecf1f3a7dd80488f25729e64daa24d79134c3e257a1cd8165e7dd275f71d8f7747" },
                { "cy", "796754925cbb249d8c7916198e54f4c103cf942a48c5d072c5a940cbf9fcd0ee1712aa30b6fe38ba6d3054ff65577c40141170b6816130a6d90bf5b37d1fe4ab" },
                { "da", "f1ad67b3ea9d960f40b65f8bff93bccfaff00750e4ca7c6a8c9415b84a2412a5fb3fddba6a5e26348036f47c62b16cc637a4df10f05125ce35704838363c3b51" },
                { "de", "95b70880533becdb9d4dcb0bc14c6e215a82314ff20a5e426ec4d8cdb6b4941a687079ba9ff53be4556d232274a331c32ebc0fd9afcba0a59f050844a0c301f0" },
                { "dsb", "b6f41a045e033115df34c64d44265391e21a1087884ee7d3c67203e653605fec2c99bf1cdf7baeb1e9ae5b9c89591cc6611540b46479ae09245fa313f8afa21a" },
                { "el", "27d21ad1f7a9a13b5a79007512fa109a10e5b8547a65ae2d7190f1c5d2ce84fcab1089945df174f243b8398be15afb1e395de8f30d7f87386ffc77cad30f1f35" },
                { "en-CA", "f5d2ced33a9408e00398e108f62b2b0b5374811225d9e1005e535ea8236140116e4cebb4b6885fd2a3c1619605a8c3685d8fcb2b12d467c5a495a22a8667e51e" },
                { "en-GB", "645f4a7a69cc35f6c774df3a84411937f69d64bea23cab5f3a6401a555ec5e5bcca72709a8ea89ba4814a7a08978689fd982c47b5c8007b3d49b9d6b093f2db8" },
                { "en-US", "d7c56c7d6934491011ee646e01d196bf3b3e796559db7465e6a04b7bfc95f94dfed2f37e7c089d3772699c539e108e5f2924102e32fac87f5dd2ae284ed1b766" },
                { "eo", "163215b300d85f7da2915758f4b795327221c008050d171ce47a1f644ca815d447d449635b857ccce78c80a5e758c0a001d905781db2503be1dc521b5bf8bfc2" },
                { "es-AR", "dd2ce39ee4923e0590a0ce4d75f7c44c51a1239b92c7a073dc5eafd2ebb88cf36da305535f21ef9187516003f074daefd1646e81d7f484b5f66b6ad524de0075" },
                { "es-CL", "1c7aa11732cf9cc5233caf139fea70fbfd1f3280b2d886a9c0491fef11bbc867a2108c5aa5be345fdda3f7c77de9a1ea209ccc542b2b52c337ebc00c6ed171f1" },
                { "es-ES", "51c51af3a27e61cba30db698a56e15c520031c5a52b8285412f8c967951537a402227082b5d3bf20f3e58fa3ae641a0bf19af192bfeaecbef60d2b4f8e8f162f" },
                { "es-MX", "f5c6732c3cdafcf28f066b7d78ad9c815e944f6796d0254e5e177d584fc49a24419eba0e7b9ded0b0c6878497600574759f90c833d1ef2f8d3f6c9d800609cb4" },
                { "et", "20339e5b7a76f6ea601d8f906377959ffa0c26529cd1e9bdd023a54e6ff219ecefdf238aa6f3a9c0d9d94de85e0e635d50da3e98a7f76a7cbbcbab90a29c48e7" },
                { "eu", "7f3c1dbd67f13ef5dc1a6b5dde0fbe7772757d49709cfe37dbceeb21483d1370e02da48c8a46b2f483a473261355d76784802a84815944c9916edfa1d0299eb6" },
                { "fa", "db92f67afba67e5b899bb23bef954f287dd0848afe8c8add7fa9ffecbb334f86449583e91a769e4d62d43c54f8a875710a9a45cd37d27e4fd13615877ac3e102" },
                { "ff", "4f0212ae416f16bcf8a9d69f3715b33e3eb8eb61a96a8dab12d01dc84f4b4b1a11405634c8efcdbb704bf1ccbd90bc365d6a2b30b7ca0cd6c00dc9f10a8dca5b" },
                { "fi", "368bf50c459108cf6520d84bca8f6050427021fc4bc5f2e2644a7a17cb49671876fe58ab6be4a4b1397a75d715316f431b08a25c606061e41bd44e091b776eac" },
                { "fr", "ca75530fcfe8e6b63249b3f9e01e3e71a2d5fe076070335342d6efe7ca34a6613d69a225f7caf1daad510c10510ece7adff4452220da016005e31a636329c233" },
                { "fur", "99cd5cb98e85ddc76c213f36d00c91516757e9f996e2a9321c6398a750255cc5b3d9b52242c775052188ef1336b3cef34cd53bd5994bd0e11652e0b1a21f6b4b" },
                { "fy-NL", "08e6e3e6cd57216dfde33b5319bd769af94bdb099a2c8577465ba2611c0797689c2b97132d02b0104a5f056202b452925dae7a2dd9c912e766f38c870381387c" },
                { "ga-IE", "4eb26b404974a5a963fd8fa56c7cb5cf847375ccd273b3ef69ab2d4cb80f1144fcf245eb743863f17cf76ad22eca5dbf3c3f4b884d6a36e178c9f25e69045fef" },
                { "gd", "9f815b0d13dd85c4186e8b5183d96a9750875b1d91c128b3bb6a0f843fe5e425a751d0981a633c74bd9a6b3acc57ea3b65eccc060967ce67d683aa28ba9e1cb0" },
                { "gl", "827dce7735cf976ecc1f696034b4230d9e244580138a9f1c49a700ab3c4f85c1bd8ae899b76521b00d831db9234dac5e3c04c914ca3d6fc4527bd851320a42ff" },
                { "gn", "3a0bd59b40eea45d7cbb6a9e3fda7e2e3f691ded4d1e1ce41e4eb2cb9a7cd873af509cca6d467938d8fdcd7718b86aabe1e4fd148f817f4345400d4b944979a9" },
                { "gu-IN", "3ba2a313d2363239f6bc0ca36bde89c8d7d1e8e4d219058e82b74cc16a75823f645bebafd02924d77a1ae27a6a8d1d90ab1b78a69796c113be17bdf55f164189" },
                { "he", "6f353a0734ea432f6bbcf7bdeb1f519d6c275ea3af5af7e463fd140fbb91d3d8cdd6fd8bf779741adc83e5241d0bb4f6495b5e4cf751276ff09a326ba569097a" },
                { "hi-IN", "29472d59ff3d0cbd1616805d7e34f46b6cffb781559cbba40d7a5d047a39e6bfbcc898ba45242d9fa175050e5e1e359be5de1f19057e0564ba57223580c9d83b" },
                { "hr", "ba1af69285bc037ad331a0c758031c6413c6b7d9d4b869e5b29879bff6225ccc167c4ea1e7cd34805ac69170177569092b24c3c34397651a4c62555a7b64910a" },
                { "hsb", "68dfe3c8b2e6a53e99edc03613119578468f0825be716320132ff4b60ac9436fcca7ff560b1760be51cdee51a1703f02cff0a4762b99cc1c03381b1c295fc089" },
                { "hu", "558b07c1e755e086242b6f98e85d14b6886b029e999c382978077b7762263a5a367462e51c6c04dd680b7bc4ae61b55e4a26f39dabe1d5efc8a478cd275e2f98" },
                { "hy-AM", "1eacbcea048ee906b5295c818d739030697615c614ee246f9a16a9eb0f09776bd6398ed89482b1c0c39f23cb06dd6ebd64353f2e618a9a3c4587c73cf88c7ef1" },
                { "ia", "ef51a04a7acdee50ad22b860ba5ba383a9eb9501a53fd8c7995633dd88fa530ee9a2bf3d0d78d051726d11111b9c11564c10029c911e976bcc9bcfd26604f967" },
                { "id", "fba0838864a5a15cb6f53b3eea597981a979e42a25834bad56a9caf400fd303bd175b5a9063ffe24f26a754dd724e6a86c6786f638e2be77d4bad201856444d3" },
                { "is", "06474b8f8288435562bfd4a208e8cb31540d702067180d14682cbf015a0e6cefeeecc0116f804e959ac0a7085f61bf7ce1a453911982928b9dd72dc528542b54" },
                { "it", "e8b547111318f635ecc3a13ec808f24777ecb57262b6fdeaab9453ba8910d5f5aba4ed25e4cc9b4df40baf0a426f316ce9e4c36341daa0b1ac26e64e6e4c39b8" },
                { "ja", "516c8c9469c9d8450eafacee7c1e8e02214d8fe03f3004ad43dad8da62512a0ed03ae69f3bb6e15d169f1757ce12daf2104004f8d46c027f6599a2c60c432aac" },
                { "ka", "71f9683aed4068cdba7109bd226a4bb9faa8022bea6d24ab695ad67e33bbbe7a35cffae79a556e6962a6a9f19c9cbb6ec777478ce6ced936f979fdf04254d23b" },
                { "kab", "5773c200bd99694c00054c8721ccbd7c73ad44fd3e04e367afae85390c8ad0b5f9c23cb7e5500eb343f77ce2bc4772b69c71f7207998492008f63cdce814c887" },
                { "kk", "a564003ca7412ea8956f0c050bb0174960f7f5402221f14f731c5ee656295bfcbe90a97960010bff4580617e65342060fbde8709c06c44094b09f8716487fe35" },
                { "km", "0e21182f5f02a96cce76b9bcc266079102984d6711ba600f8e4b1b82a99ad74086d2134af0479aaca21b3c5fd9d4518b6bd41050a1a03f387d2f6adda8c26625" },
                { "kn", "7deeaf6f7503f3ac39d1115c0399c83e6b7a9214533f95a82d0ecb24d9c8fe675de4e3c6152d5940eb60f9dde30a35453a0c3d5d08f45acec574c4773a06ae37" },
                { "ko", "7423941bff7e9aeb306e0c88dffedffbfc0a4f9ca8a414520792af42ee5c40b531c78131450668b5803a85023a205ca98efa33f45955a8505e6f1330ac62bb71" },
                { "lij", "772456bf94ccff6ade0a2874fc7b02dd8a83b7a45644ff824f9ba672b711d00916900e23cbde79bfbb2cf9123c112275bd6e36f19353a083e1da880a4d616612" },
                { "lt", "3a3e3fbc444ff1ad40006bdf7ee4c580b830cafd3512b3661ea9fd1600258f77f2234e9d7485f589c910942dacf9bf11c42609cb68228504bef796dfbb0f5046" },
                { "lv", "6c45b632fd9fac38195d5cdf739abe2841f2bb8d9b166ed91f1985ae9cb886d402c65e0439cadca31efa02a50d0a399cf9b5e9036b099ac1690e0b642f55f56c" },
                { "mk", "bb4fe090e3ba5417c6ad5f085afbdcabbfaa2eea39fa6a1ece2a779d6e92c0bce2eeaa0ddaeb4a33b8f765a5bfb622aaf7e0c89eaa876798780b6ea8b4f295cb" },
                { "mr", "a76eb4c11267225006623dca0fdcbf2d6dfcf1a1f2346ca88e372f26a07ab6febfedc7c7b4008fe9df88dc7dc468c771cdffd2aff06c57c96dd9f7a0a64b4575" },
                { "ms", "d8eab3f593a21b68530f0eee4f87301e8b7711b7aa06b9b90a5d0d104c10dcce155cc8df42480b7496658e5525608b5e69748d7ed151efd129a9d48516e58829" },
                { "my", "96849e3be942ec5291713fff5170d8ec7666700366d0cdbe22e32fcf9038644f28d2d15badd5dd116c5d7594ee4d38e7e60cc0e0cf99d881ae986d45798f4ab3" },
                { "nb-NO", "b0c5e4e496b2339ddb78e70c20a17b0bd1ae2ebb83e6076d755cfcf8a3888f0fb81448cbb7c52e85cd70a5d30954f6dca1d5c70cbcdf64117175923bfb9c8869" },
                { "ne-NP", "48487ecc8281cbe451b03c7cdd397ae55cff5cbf6e73d0f62ae753447442e3f79c953801b4a898bb6d45f8589ffffbc6b5119acb461d4f306cf3d5cff3cc0583" },
                { "nl", "2ef079107713c4308bf005cecb3469764e1bd5191840fd5875eaf578448f7b5e80a818732e044da080e242fb25669df6c9ca06fbcf8272194caef7ac92c8c8ce" },
                { "nn-NO", "cde136b51486d242f529fc1eeedf7dfa06a84daff2e343c1c38e9dce2d6914250c28f3db4256876c212f5c4a633d8e74e0f90c5a3129b1bc54743e655b3a655f" },
                { "oc", "c5238bbc04ef123e0f192fec978225f4a2a8d8587b1d4b7c88bf47e9142678ff0d970cc46bd26fe467f7ec7cb7597a50ac85dfd4d456421ded88011b24097718" },
                { "pa-IN", "5197a6a6ff5906e93b3a9f298a173ef96fb77e52db67b7f3a66966c62a501505d97b18f9fdaaa4ca7b24126d1eb6c036a740274ae8942bbd9ee9d5d0eb46d7a1" },
                { "pl", "df40cdcb6a2d7a3a23d09a87882866f16720223ba11f4a0371608b929fed7dd215a0be88737de3ec750cd1e2ad7646b2df569e9820c6fc4573a5f0bbdac47788" },
                { "pt-BR", "b501c6447141d9a1cea41d0105a9f460a598e944b3a5821f3322ab0c340d7a156c28664d2f3942c6d35d01cea1268f8b98aee58d4cddd6ed130d5b87ad04006d" },
                { "pt-PT", "aedb90d4d8991763658a187cac0c3b2c75073759e217c109992f992f3c333c34355207e6531869ec56c6378b5a340c83a33445dbcf543624374bee93463dd045" },
                { "rm", "048e7b974c75035e1ec93d3a462189c00fc66ada886ce0800b2739cb8b780aceb5f8de1bf417f674090f82e76a8db31f346f8224e875736374ca2b57b9340ef3" },
                { "ro", "c3d1775b27ec0f2c9ce8e52a0e8fc344584de1cc7f6872e1515ea18178686afd0fa9aaaf3612852a43db816debe2f36bce237a00657d4784157114a88e9f8db2" },
                { "ru", "844fc650a4233d849fa5421beaba366b7f35bc39a8d57656a683b7bc6bb2181249b8772ed1ae18e09211468b47f39cc142101f2198000da4a1843996e44cd344" },
                { "sat", "0864cfb78f8d5bc850d31b86b916eb45d563c4e481498b2095519a7555e1ac2abf0e34f926ed331c9847b33b7ae6608b860a48afdb69b47defae34e099858e57" },
                { "sc", "eb6b0817b13728ed61ec1c4b7c58f8dbcb2f4ba30bf3642b7c5ec0719bc196dc9b8fe936180f93b1618a6c52c0827239cde56e4d2c5fa3b4c3533a6ea3618b37" },
                { "sco", "3981fa81ced8b4608bd8ed1a3bb3e0ef3103af912f08966c62520866ed38d29dc419adeb252698541c6aefa60e60c6567c79aa9fc1ad39ade720a3be0f1d01fa" },
                { "si", "3e0fd70a24debca27ab1c6b848aa7179dddcf21d74b8bb3cad6884d6e7f281e86ae63fb8f169dd25649a8925759180835cbbce1d8fd7b4d60b09dcb273d355e1" },
                { "sk", "70ada5e4d6224de7b4dc4b11adfb461e29dd8d963517ef93b3ef8c0548302f9722636437df5e8b1a06e0198d4cc22d95c706c6cc3516ed9338722b64f6088d2a" },
                { "sl", "8029791459f43e7d8c55ad6dd390bbd18aa8e1bef2e1a645e3276808e879b4430f7c2e1925db1ecb678a4c9c5ed98d22dbfe3f680fa236759c73f158934672ea" },
                { "son", "cc12335c5299e1e610ef18b6c3155c1172d8d1105e382442683f6f58c66a658b193ea4b4c51aec3c83d96dbe29ccdd340394a2ee6ecde9b267723c0f925e78d7" },
                { "sq", "22fb39d84f9927d4e76d11a5e3c3b0abc8221ea39fb54961f4d890956d1c58111e43ece6429149cdf427198ab8c2426d78b4598458e7bf9b925359ae282d71fa" },
                { "sr", "4f2061eae64fc1b892861ed40be060679b436bfc67f917c5c7673e852ca6fb4dacb886a37886a2733535e711cf63874158076aa40e3f0231c03c5f1d91ccef4a" },
                { "sv-SE", "5166fcfc1cdb90731ac49257bb48e5bcce5ada78731179ea3110b228ac74d12df4d2d5f5707143a2550ce5fe10a5828c489132c3666a4b8ebbb6a55a8e126dc6" },
                { "szl", "d842ee16387834987a30ee195d3fd181eca5fa7aca6e0738240aae70b13f04eb29f21508d047d20ad0e9743d90e650b008532e14a851c6c4f4ea157221e26e38" },
                { "ta", "9a815c63db058c5c18c4b43ebeb75af479be47210aeccb3e7ca297abf8c549f7c3ccfc937ccafa82794ac749f9258c7ce02df4b55618363f9dcb30e7a2ea4383" },
                { "te", "32289b7e14719065ade9018d54c350999d885492e2ae9aa5a3390a9fbb147cd317f4950231a2bc6d6c1c29356aed315842a89e5fda6054946f519daede8d6c63" },
                { "tg", "94d5affcf30c50f096986dd0c94471dbda08459ae6dc4722dc98cfba1e7269a22df889b8a190be8174bb3c60b2b5a90e9a9924e54629c72ea3fccbe0bd4ba38e" },
                { "th", "ffe029fa084de200745b6e2fa49717a2be456ea7c4ac5c44916f49e0ed3f5deaa8257a22d622ad4e70a7ddbb624f7dfdccd4a83fd92bf7ba9190e328c13b7b1d" },
                { "tl", "7b8f9a5229c733a3ad64fb4a03817d02178e8f0d224a48c07088e603480f7f3d07d6eba1659bbd91face3b3adb5e71d88b867ff2ed0942c8e7937d62d1d7f0a4" },
                { "tr", "3f96f8e65e061189048df7957d5f28b5c82f4588aa5fa3b24e306a7b0f4ec69053c194d151a79a17d046096dff1962f4262f4a550db9821d36acba0cefb5dfe1" },
                { "trs", "d5e59a2900ec0741aaf4d167481d8a7af6603ec48b8c1b4dc60f39a2fd8faefb10a04eab297dc5783428dfd5cd7cd0f228cc0db44cd5691540c0e35204fbc750" },
                { "uk", "0de680b9a7635620a59980aba90c71a10ebe47c1a8bb516a8106ccf3f2b40d2b0315767d45e1338812540b70d84aba1691886d95e70091746bde2259d07ba74f" },
                { "ur", "de975880e79bda432e4601d5bb24c05c38b9b3470b6f2bd42de2436dd0c59d612a75c8ceac6ee5a7c1fb42d5904f46c3a413598001931a1b206bbfb9f3e9df03" },
                { "uz", "4ce940b95b83bf2dc2bbb8b46c778d9a4ffa2b80da89a33e0992c0b1480883b247c5f76e5df93dcf62e418451649cfb2028af09660e5240824027d07212b1b09" },
                { "vi", "f640d67f9044efdff667041b311773c02a6ed466f70cf679686a414fddd5030b911acd92df78f0c9562da0eb110884ce0595601021dd8ebaf45ff7de545581b5" },
                { "xh", "929b486a965f11f16259692ffa35629bf65fea6d5008deb1be4646a668c1e3a40e4219e7f4dd0b27fbb136e0dfb0232b7165835bb0b8352dda660c34ca59b2ca" },
                { "zh-CN", "9ffafad2673a0db29ad25e013407e9d7b242bee687e05d4c9243e04dbb01953b568cfc2aa9286d099aab1fba36aa6e0c65723076011fe3408f761c2012cbfd44" },
                { "zh-TW", "eb50f11ce985a905471058de8be5dc39a52759bafb2687ea086b76f378affb5c6d3c584bf88211b9e9f503420c7be860be17420d5d231d7ad7d88c4943b56970" }
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
