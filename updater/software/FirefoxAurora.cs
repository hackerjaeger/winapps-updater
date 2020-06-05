﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020  Dirk Stolle

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
        private static NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "78.0b3";

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
            //Do not set checksum explicitly, because aurora releases change too often.
            // Instead we try to get them on demand, when needed.
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
            // https://ftp.mozilla.org/pub/devedition/releases/78.0b3/SHA512SUMS
            var result = new Dictionary<string, string>();

            result.Add("ach", "4f5e02ec4db68373a5841a15005ba5cbda5537632b2005a8c3e452f411b4b993c179da4a6be8f83cf385bf071289795d0d91c2e107eac5470a6b766d34ad38c6");
            result.Add("af", "d76562410e7650428bf09bdb4110d5d8bd3145229ab9f19eb97e4293652377f27d5240b8662975600fdcf8209da072354b183ebf782ca3990aa580204c8c80ac");
            result.Add("an", "8c6ebf96f2178b1f3ec7ba27ed38b6a7fa5b1ca4ed65b23df17080cd987a23b922700bb0db0bc95151b6be973deb052a695f30de67a65b7d8a7c6c5fb60835bf");
            result.Add("ar", "8a2a28c3f2c415eaa7e243169e165016bb0ed4dac643316a7f71766379772e9523c43cd86db53371efb0d3c0a83c4f81e2b408c86ec9628740f9d59fd022fd20");
            result.Add("ast", "7d27be5efd68baa652488d1f697aa82e7373406883ce13ef52dbdf8802c364e210ad0387465ee0da364f072b14971f9799f6c7f57812cf277dd3f118611e61b3");
            result.Add("az", "77e1ee850df63f917b931d5013a66def871ffa2460d67f87089143e26bf31f364b605e706b4fc1182daa240fef4b1f3225be1e6c2d0841a493cae2b2416bb688");
            result.Add("be", "b4bb974d8dda8b198b57e5ea5e38d44d37c6bd8b212c689024591656af9d5ad4bc5eb5be16f4bc3f48f6dc7d6eb9c025ce8713c782605af7ae898269157eaaef");
            result.Add("bg", "4cacbba100c6c2ec4878cc7dfc39b81fbe7cc72b38c6e246a03a8789fd85cdcd198dc916303ec1f7cc283a73b269069fff8bf74214c3e22f53ef348376ad9717");
            result.Add("bn", "0840bbca735aa88aa784709a4819d1be6b979d35977a22e671e1701718e7c180144d64f596fa23e6108cd5df67091f84021d912583c794834d051fe3ffe3cf08");
            result.Add("br", "6b47e09a1e009794d0dd6cfb351b3fab2d3a0ee87a025c7b04cb0b3b016d67e589d9ab829d159a8e442982030e0f9dbaff811cae01553a8a05b400053e29fd40");
            result.Add("bs", "f504c6416871ccb44bb2e625e4a8b56de7c61ddc7b6e6d7f614c8d890f89ffb3c134de3d13b9961b814ea2873cc653dd559a37842f14c2e0f2b1db23b87b13df");
            result.Add("ca", "160f772e601328fced9e3b8808ee3ff389aa48b88889965cd8cbf60fba09e3fe2eba94375ebaf64ea56986a8cb808dcd7ad791cfa9ddbddab353a62cea90bf88");
            result.Add("cak", "44a2110dbb95b65bab22355c54697b39ddd2a36116eaecfc3af8b1a21fdf9c962167a3310eaafe90c553b77cd3a9d50636aa93ac2dfbd0f58aa77cdae50f05ee");
            result.Add("cs", "7597dff68dd774eb812f5305b8e303f360b6e971c24596c886e963fda7492e57ee3fe1e96f00855269d04c7de32cd71f7ba46c3a0f98e272bd173092fb9a481e");
            result.Add("cy", "3018ad50b30742aa0e360c397ec1220ee40beb61762ecf30addfab16c3296e68a1535410fba58fb7fb53374aa0f9b5c7f87dc86ad62f10dc2a3b5b1ebff347af");
            result.Add("da", "50ca10194008fbe17e6dd9f907ba849c5c11c57537a71d8c2e93c1e769a3984069bbf2cfcdf3e7029756d8389c34c9f7d9889dd514b372edf1483fb00d245295");
            result.Add("de", "7ac7b47dcafcb19c8c0bdc91c8a5aa0ff1efa07d2d3ff2bc544917fd1fafbdd0a424ae77dd21feb3517368a372d42c3e3a98f76f0142dccd3ece05beff594f75");
            result.Add("dsb", "ac87469c3c01196b9f6607fa5edb8e84b60d947f2bed297bc45190161f3b43baa77d9dfde2a383143116dbd2fd0bb8e36152ed714aeb972a8b8213e9ad1416e1");
            result.Add("el", "ff6b9ed2b7c2f36a83009c9c2f180d4059d8757d93aa6d05ecf4cbb9fa3094af486ca194245b11192453598dcdac1acb33c95e80fe6a4a2ddee0754fa10ed8b5");
            result.Add("en-CA", "5e0c6b4f2b1c75305992bbf35cd3c8b94e7870d85b37640a84aab355690eb0096466eaaa23af07e10a4ac9fc66e333c74fad6ae5e3244524708409d5e47e7c68");
            result.Add("en-GB", "76611431a3ef4d2c02f571de7ea01089dabec2addf2db48c8ad63097f211098cae076f615b6e9b98bcb3b52d47acf1cefb304c3ba9e996624b5eeaf8a0f92872");
            result.Add("en-US", "a9943d01eac89cd0adadddf70b5bd60ac98877d778e27ab0790cfbadda987093c53a6f9fe9f8b3bbf36542b74979c451101406e6baca07dd3f0db1bb0339c7ad");
            result.Add("eo", "1bee917513e2233163aa89efa835f94203fa999b02383b6ff840b7e17d063f0edb6fc7af6679061760b318ff91613976fdd3ce8bcea986c7f350e0898f8cb696");
            result.Add("es-AR", "4b7e144f03b4d79998bcebfbe11bf71d9c667ac9cf051b01d5e7c3dcf91c562eeafc4adc63fcc8141a786035fe9b548c40aeb395ef827f81829b96d77381fcbc");
            result.Add("es-CL", "766e83572ce658be519783a2c677323bc15c6da23a02f73b9aca0f4927a9af83d365b61f072f059f9a9b4be319dd76042a2c9e63bbe718b7237e9c958dd0e32a");
            result.Add("es-ES", "4436d096bb701a27fbb58f8b73815d586c5985d8eda71e8dd4058c7a4b7bece858ae1a49895fcd5b6d393cf37c93660b6806ee478de11fea44d74df911dc67f7");
            result.Add("es-MX", "1b3ccabc0318da3c9e850620204c3a1d28a8d831107f1c8f86a88659f965421e331ee6dd553dcfaeca4660c506b4838b660790c5efeb005c7af972c279c77262");
            result.Add("et", "7351cf78e3a3d086908db7b08b9aeec1323a0a5cb6c9eebf89e7f56ef752dd2bb2f87f26359cdd089eb587ae27dcb7fafcaf0bd6174efc8da12d268ec321743a");
            result.Add("eu", "576ac9edcc4495715c1d3904b6f160f08d616d30a001db4b58e71da53f321aaac5087adbf816e02efe439a54d255171a6624255b65f305271e5ee9214aaa4785");
            result.Add("fa", "ff259dfabca069b9884dea6599c278a828e18ee73e55619a2d5ee51150ac2ef092d32fb818765887de6e5e092f30eeac3fe8a8085e7ee4a0b8adf92582f5cf10");
            result.Add("ff", "48f7baeb8f66a8a98f7bc2d44ae124f3f2951ba88c7f523dce1e2046e182fc223e504e3832e7687054dcd168cb794cc57f508144aacfbe4b55080cd1911ff661");
            result.Add("fi", "f72d3205ee310fab8c206cc8ad3ac1c02a3bc4280170a46188be45d51fb4a2c462ce8af62ea4c3b7888c6d308837796e4f46659cf6c2a020ba13c0bcd403b6ad");
            result.Add("fr", "eda56e9ce9c7c337db798c6fc0543b0c600c28a8ee46ac9121a4ff9c603b9206c852774a7e02cbfa438811fe839ffe08d1fe5cba9c51b19b8dead73224565dcd");
            result.Add("fy-NL", "067a3f56a353e563018a2c835012d6ec8658928a40014a4ff7b8fe0b1d298a9cdad8277e4404a48765865af17db5c8c39a2f9a327c4939b163d6bd15bae6372e");
            result.Add("ga-IE", "bbdcd5ab23724ef68de608e6a18956d413894612d514b76d0cbc8b0af29e0223bd9ba2039f3f1345dedff8e47afdf38b6eb4b7a4c96c06ce265b4a0634bb2341");
            result.Add("gd", "6d7551474c6be42cbb0d1608e0938905aa9565ae20bf8a73756473c2bd913c5c831460c2542a581f293bf0d9b073e69119c0967b1caf1dfffd159aaafe62177b");
            result.Add("gl", "ca48c64a77c8b24583a4fa881cbcfb37a4571e3f3bab2dfc4354bc27c5e4f4080fe217466e14a7ebd4a52be7e6d81fb6173e7efa3415bd0aecd65095bdd1bc23");
            result.Add("gn", "38f2c495b9d99dc81f4333886c273debea8746fab8d135b7df17adc3021cc07e86896c53eeb7a371d62eaf2a27cae222a736a777f5c7c046581703b410835b15");
            result.Add("gu-IN", "9c9cd4cc8db457a06c23ce391787403e4f5cb68b48e58d77d3b26521c833824a644b87cca6e865616db2baa39b979c6fd3fd6627ce5212b7488741a570d15394");
            result.Add("he", "5ca2754a17bcd2971bbd77a4bd80fb5028e5f3b91133aad641120162972bb17296285472bb293fd59b2b4d004d2ecf7916c8044ba6ddc5fec8e2531380c39726");
            result.Add("hi-IN", "64165fa1cb09f789f34ae43856b433157a8f30717e0d03d085953ef93dc97fb2fbc550ce8048de1e66f1f1f02f6fdac9008cba0d69b207c3bd3b22dea0cf4cef");
            result.Add("hr", "0bcd404e0fb8a8f8bee20b579ac09978ce60d381c19109692ee25ae6f98016b26e1dce53bff00d966c87b27b4df251466b93198c1040d5db3ebfb0bb3b50264c");
            result.Add("hsb", "6d6a7d243e6dfdd63ed67fc8dfba24bab4cb6bf197fe476c353f503e6a59744cb38d224a6a270dca4f632580d6d135a942cc69ee07d990222e2b6b1ac64bc35b");
            result.Add("hu", "6cce10a65db1bd7c7be61a4750bf6137208ae4734d5e8ac7c59e3d976280f602cc4043ccbe9c78a31a82e65e38ff4a8e74bb6e94f260305d168787b4f828e81b");
            result.Add("hy-AM", "bdd481d35bc662660a2a39bf7936f9b1392eb2ddeaed877374bd4c08c5e4f0ae35cb64cb78db6bcbc46ffa0745c45cd7dd79948284f4f323a66bfed22faf7847");
            result.Add("ia", "5900adb7da11a31ae8e88299860108b103ef337b3ace5b215bb95ddad132a36f24a6a310655f393ebf788f774e68e15f44103c0073735afd90a62b1f7fc98dbc");
            result.Add("id", "f36f9fed1a45197eb608d2a96a86be6c1626186d0b3956a04b1ddfb19268b6e76b59119572253de357fd473290068f1c13cbcc6ba63b05f98df2b7c4105aeb8c");
            result.Add("is", "c8f97fa15d5bbfd81b55ef0975c88b1239d3ff0c67bf721373e5952ee6bdfbb6e462337e764825abc50c2288621042d4962aa2eb5c6b813dd3f96bad3cf6af80");
            result.Add("it", "1f7739743d85679310f2fc4b6497bddb32d9571a62a8b3b355308c6bb59a90c0b4679de5e8d8b7488ee90a28f949111c92caa187eb2e77ae0576f87399774132");
            result.Add("ja", "5a4e491a4a9c64d65d0c9923ac2d8a807bd4125ee1570c7ef6fa10f5f08799dc959f8b09b8b21473f9b3ff45385a3f6f51676e064a3caa92a312694c53417c88");
            result.Add("ka", "397db38e85fe3a10cccd9453b0c7cd27c4e13496fbe6779b3255ecfee3c014b30e8f55ec7caeb6263b2036f7ee4f262f648a4b6bb4f45495c5affafdd33f80a1");
            result.Add("kab", "7b7431f08c3ed16bb2e0a0d066f02ef97beaa2d527dbc47a0c067d35ede9d0de90dcb9a1c13c52bedf323ef050c4d4ac3d7e3e82827446e9b2141734c8fde57b");
            result.Add("kk", "bef34d168df3a9fd51d0b2e6668f130a2710085290a95e9015e2f864e2d2274d7bb4ee4cc53f1f5ae13438c4e3efec1f6326be78b778fc5f9621c2f0ce5ec68b");
            result.Add("km", "956fb3eec45b50003e8eb1f04f026e719db820d46473c7b685397dda99ba703694f2281c3fc7e2118cede27b82d6a5dadcec5f8337de65546e098bf1007cfbef");
            result.Add("kn", "1a69f438d4041793faa5c392b2993d7d6bf383a9147d1a80b68fc127da2fa9d0d34618c371193e677a839cd5f557cb92d494d18e832c9bed2a88805dc4b9c1a3");
            result.Add("ko", "98770a94226e2a3ebf23deeec9015fa0baebaddef8ac0335ce073acf3bcd469b7dec617f970a3b4b44ae8e4429dc732f8e9544ff89807183b44af389c791e3e1");
            result.Add("lij", "d609b1f606a9f891ff4dfa31c6c74bed9fb24064d5461303daf597e7b072ebd3d7c9f9de709755fd23ad03134bf1e33f48bfcbb61e2f4bc087f6ef77116b401b");
            result.Add("lt", "fcbe94f61d6148c04eb7947f2b0c8717e54d152a4d3cee0be58f119943dc03079b5550405a0a610287301f1176fe0f7eab14150478c1ad2800d936469252eedc");
            result.Add("lv", "85931333f78d9c9c146bb48e2653f4c9bdf4e9b0ddce0bfbb56ccfab6ef11525bae18763ff5840b11c2e9d2212db0689848610c8ea4f8fa2f865874913e717df");
            result.Add("mk", "0f9fd8fad21de31ef774e3f9f735e061b4d1e0cbc5ea4e849a20fbb79e5adad9ca5374378f9fc5b2f0ca8ccbde2739c847d3da1549c3aab61c197ffef9dded5b");
            result.Add("mr", "9af682ec4ee7b3a55d9b6e4f840d94728c27914920690221971bec53681fc94180e123afa152fc1a3c3ff47291018b14c11c5a16e1e9568b6dc7e374d14c6158");
            result.Add("ms", "d953f935470b444bdc2273373877acaad6f9deaa79a27ecafca1caec5ce874fc3a5abfc5404cefc07947ef5f18ab0b8b2bc6fb84d4f18e11273445b034702a3c");
            result.Add("my", "bd0b7a6f1c304d9c948b5c808fbc034ada63ab9a9eeb7d9ed0b3a4a30a8e4e3a4b4b3a535ad0efd35a8fc9db6cb683a696f4eccce06b8774fadf05fb2514a3a7");
            result.Add("nb-NO", "ad3b76509998e08cee06605d0b71b04630e1adff9db23cb3b5ad1abcd0e81c34031757a99fc833e1c1d0c10d2839f9143077f065868f246c97618483362542e8");
            result.Add("ne-NP", "eee1c7b97c802fa0315a1416b1f275f0660e3f894226329f6849108c25be606d29c9e66a56cd3ec4271bda43199c81829e556b257df45f5fad8d494bf200b800");
            result.Add("nl", "357ecb382b56cad149929c10940537f057442a97607156fa7c8c888b830a1d4171c22f37b7447de3c84dd287adc8ac6b002b8ef164c47ed900572653226c856d");
            result.Add("nn-NO", "312d3574b6a9eaa2b3d6894f1c4c78d09d67fa361c15237c0ea10b579c34955f341e26c9ee52e0cbe5d1c3d0214f519cae2bf68aa802612bcd45b7ae8de7cbc4");
            result.Add("oc", "b9e88495e6f8d888842af6a3f5f2ea11e6ee1b2e165b6338b08529f2cbe68a04533544725196815ecdd45729e9dc04c0669c943c78e6ebce801c57e6d73a7458");
            result.Add("pa-IN", "2accbd6590e44e73140907ab3e1b34e0f386070835ffa1dcfd4cde2ede7ccd0d4e5f741a1811dca97563dc835bdcc189ce4387068e75b3e7c9b57321470cb2c8");
            result.Add("pl", "badc662c6bb1614671043749d2afafc9541e447bc57d47e8e56118f125d0a3c0488fb3054976100110ce2e5adbf026093bb837fefa41187d4a29e1769c5ed70a");
            result.Add("pt-BR", "7cece3f944b16bfd60ec976a3a9f87a310a22995d68c18d254c7416334444eef33bc0ca5e125f4266efdafc683a2857eb2c61a0c7cef75ea8a7c8861712b66f8");
            result.Add("pt-PT", "b7b04d644363b1706d076e9dd60d9a54107b950e8d1a7244592bef476b57680a28f79e855d4d7f58337c00ec79c2be2e0038b01c9f0c8f371c8a7bbaf28ab77e");
            result.Add("rm", "255715fc39130191f006033e522e7e52dbf7fbbce0e091db034ed94df4db42a8d6a2c0b23b6fdcc1083e5b4de5d2c5ae297c7e0bdb485a34fb6f5ef9e638ed8d");
            result.Add("ro", "f79deb794335e1ebcf0be94f5c1a62743d25c20b73b11ce52468bc8d8c1e3af89b89ee93f9b97373fc537e957d80640b71329d6780e18dc7a928f8ca54b7f143");
            result.Add("ru", "953d94bb93ddb8f16dcdec7908e7f5a52df0116f1fca7fa2fab4faf3bf0a1c0ec0a40d8f5e7574f446cca8af70f38cacbb64a3a49cb3d61ceb74bb329d1e341e");
            result.Add("si", "20bf78cdf8f141cb54fa934853e85a03a648029f236f45f4824e6c3662e56782a10c4a5267fff70be6ce56f1a4f2d18d7e6302f0c56d7befae4ac4acf4a78e0e");
            result.Add("sk", "a2239860adcf70483d2f13a297b9e6526ea6da47f88cbd794349bfa2de1a7c156c7d48f078b090aeb505294b16e3941a36ae1b64cfa037b02e9d3d865e07a505");
            result.Add("sl", "b72801fca2cb463be9e913efeb84a898788f33f344d97418b7f1871651171554dc5c795dc0f936427793a25019e4d0faabca976b12506d15587b1bd28e9e021b");
            result.Add("son", "98b1de9b523a61830e5fd2292c0685c64bc9e5adf49ebb0dd77fa62bdf6dd22a08992ab8a96184579230b0c7816f6ca148be94f37d9922d3a8c80c0fecc2af72");
            result.Add("sq", "ac4cd7b516a1bc2a1b11dc0a83b47b978ba46509000449d0886b04fcfd861f8505eb70f6d0e67a904f9bf7066b3e207abaff94f0eac967753a548cd724075309");
            result.Add("sr", "75c77765886507f063c73a357f8d76994d38ddad3ec0eb6648bbbb4e19b3df471dbe9f306ff3536fa7ac995d9703318e1e968d21eef7fedfb309ad92e562e1c1");
            result.Add("sv-SE", "5d7cf0af7dfdcfc841d0ebc661a2863fcfa5210ef494e536b527870107f5b58795a71b1fc5a1d60ecab9e3b0b239e92f79f5ec8539825132b1ac3744f7a34579");
            result.Add("ta", "d6c09c1265ff2977fa10081b6ed956d61905aba84db2dd9f091573d4d3afee5a6e06dbe239cc0039d67dde0cd28bd11e4414dbd92890649df075fc5786a9ea0f");
            result.Add("te", "00bba19aeb9be0738f56fd8b021a7d391a1707fab9fb7f17d08d7a8994caa38cc5854115d0e2814e5a6cda4ac1bd4b358ba7c9c38942ee081a3e220665505ebc");
            result.Add("th", "8cbd16f19e1abeccd985ba0457b7830f43d8308c47ad4ed1e10c528511faa265b7564c9191178a3fe657e8cbc4bbeb5c4552e045e18774d4838e783096f27306");
            result.Add("tl", "a3c641e7bcccec246018db17077c49644d026c8fb6f3105bd063ab9dfbcb146ac0cc628cbdd0c5c979e8ce7ad8fdb7bbe49496e0d50cb9bf29888d5adea0f731");
            result.Add("tr", "89ccca3bf27bd314e9d57369e016fa28ef72b65c0c52d6ffb07ffde2124e9e9fd13cb9188084e02525d66e2b16e67a1d28ef6e0deec56d5002bce58e3f1b8c44");
            result.Add("trs", "dae0b0c45e34acaba39d9a9ab41edecefafa212db2ce4d924b38800334276050780ccb89879e247efeb6ec523ccad85de3e6889eccb05f4d1a53e42deb9d7ec4");
            result.Add("uk", "ab4da287193598ee6f3604c76b7d2ae0ffb4dc17e83d223f54198d1dc2dde9c69a9c4ecb0b0932282f0e6341671ba8e957af5721f27a8afb865451b16a45edb8");
            result.Add("ur", "ec4300bd2a72d32bb0d5c117f24fa7768cf3d7b6e88de6619ed9e96a3842df58b5ebd6135f141eb04c7265e04f7ace01cd70798bfefb2b5865bb34cc252f34a3");
            result.Add("uz", "5b116e0b6285b32f29c27ad130101e3973b2fa72c01b70f5cf2991c84f643c7eefe3e303103344b2ee1a122569e1dd540bbb5af8a1ce8b6218ef8bbe598ce28f");
            result.Add("vi", "2770787200809c8ea082c715d95504ade9b6dd6d58783fdc618a394f341817e27b772bc1b1d41025f7c43dc07d1f31222b77750980b9b65f4653030d6cac13f2");
            result.Add("xh", "5b6cfc3719323f2438fcfb59bade192f93512e0beffc3efd64129ec3caa08ea77ed9584a223f6e4a33a094bb46ba57e860e842f77682ee85c2f9404b07f494b1");
            result.Add("zh-CN", "4a7aa683fb9669fda02da4d6c2316626b8af51799b19879ea85bc6ed07178ad7b284b1e70ccdae0e0b3fcde4f048affcf5813d11dae647bb4baae6f3a4589814");
            result.Add("zh-TW", "abff68224bc470bbc5bfa21cf2f1e10cfb3662064e7fec1c250e7ac62f34831ff87028ae5c2d88de831325c997a3489ceb693c15fffee072000ed50b008538cd");

            return result;
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/78.0b3/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "d83dd3885433c03d88eef0714ef4b0b6e3088fe203f95c70d33c9490257d454f2503feeb5bc2c7b02e750ce96b10f8d0e2de97b26bf5641b725427d583e23d1f");
            result.Add("af", "7873812f20a42d8772c9a042755154314f40d7e71940a35c016afc25673c07415f1e4846b6f998d4317fa6c64f93d5c18d11c26128942ceaa13b885a4b7416c5");
            result.Add("an", "13c569e913027d7082ee19a6263d1b589a3053adc0e476f167a22b71baac209fb2c980eca27735274f124fcdac7bb1ccc2745252d94f9a192d37933c98ff4d30");
            result.Add("ar", "ee21de1e419a89bc9dec32594b6a7db658abaeec38f1dff70b057434f130ee11fa06cb8e421a29ca92f1de59d6ac459cae1ae9c1241418d8887230c51a629199");
            result.Add("ast", "609751b45ffe8124c975564d774bfb29e5e6bdd079b0a2e829007bb1e5577576ce7328e66feea52c7b39bed847202ae3ec9ca3177b1bdc780dd7f097d1ce6d57");
            result.Add("az", "6960f81980df62e790bb007ae1a0479951c5d6e763fe43f1f49d41de5c43c3fc9fce8c40d270e6f3da70c427a271d7ed07b39c610d90ed8b4b6e6927df085441");
            result.Add("be", "08556e1e86c5ab3296b92b1cbcd1fb1444816a6819a31306ff36a9e9a7336e567d89e313bdcdda4f44d253e4a916d7d022a8966e7524cd4c5b2847de1d55296b");
            result.Add("bg", "aa2d055193bf44f42cf5dd2a0b6a4892484a425ee302af862173f9766c8bc3a4c84a74d7c4601d0507b5ec02302befd8cd9c0c2d5d7257e453b7aaaf5b0c4a01");
            result.Add("bn", "9ce8ba78956ecbf32bd19129034e33d317d3df27f5c31e4cad11221dec564b9b7d275178602cf4f177f79516787da3036299d5264bbdd91cd6ce858b338aa5d1");
            result.Add("br", "e76419ff4a7f35ff65349a7526440c4d5159beaadf0c4585cd70b117c422698a6bfc8951c989b0c17bd9d689c2168a8669668cbf69255d6253156277113debcd");
            result.Add("bs", "43c2fb2c7aa9d13e546915a89ff765558276e26dfbab86cabe9228ca19a6193936d9e4d290bd68239fa2f2c8b49951fbedfc2f1ddcfa719f03b24e4a8b90feae");
            result.Add("ca", "de118f246d43efb5ff33c46081765a48c1a1eccd390a2d61b56c879e4c09b79ed3cf1b0e6050113e922c5fec771036dde0e84ad9e1647563ea5c98538848d5a8");
            result.Add("cak", "a5a70fb03c8ea892b7c9d100c73bfc8b95ebead113c8a1c88ef7ff2b4a3e28d2ca26413c931bbc6c5b3983f101483f1f28f2b36a4c535961d24d186e8e237fb2");
            result.Add("cs", "8e791bf302c8e336aaf56812e2ac50f28ac96c12a54c4098e5b83750fba1e120b1d24675384e46146cd6faacb323929b0afe6ddf3438e25465ee69fa33ff1267");
            result.Add("cy", "f5d82da981476c17e8b915f515952f562fa861722901f93348d40a898bb92b6c749c49c58ff4a64e942a51d745899446b27bcf31c2791a94d662f0bda2cf1a9c");
            result.Add("da", "670126588d74f7d01d90f37a591144246e5fecdc671edc9816f95649368fce9eb0ff81374b066d14e6dc54ca14e67339717ca623468a1fa99c1abef282dd9c84");
            result.Add("de", "aaed8302221fe43f4bc46fff29e07a798eb954278f01803d3cec00ee00e8529d6a59d32973b2401b5426e26cf780e7fd8edc1a342ca75f1defc0d025648c0683");
            result.Add("dsb", "14c3f962343d000d798d66f794d7e8b3211d0d4be12b44c8970ba2e0d46c816c7a15fcee6e4253b0844717e71c94098b244c1f7e3e18f6ed39f63d9f78ceba8c");
            result.Add("el", "d677434a46abca1ebe88356fd5c8628e8e2223a8bc5abacb26d74a4e10fc227403f8976b21ec11f35a5559d1af4d9f6989b78471fe09687b3de97be50e99f7a3");
            result.Add("en-CA", "8d05eeac19a2981e9afce828d34e170707f9a61c812167cbf639f7aca511da084f725bb310f099ba9087707eff60cf507ce284bfbc0cda28466a349a44c8edb0");
            result.Add("en-GB", "929e1b55a3a7f7927f2a12dd9ba2e075c9307181516102b352fe2537a54be37435b4cd1563f96eb2272272b31ec6f2ee63667fc0753dd9da0b8b334ebb85e3b1");
            result.Add("en-US", "c1bc340585b2a2f438d8d857dfcd7ae66f5f1e17331dce81c59216e1af3679e1444152dbff6475e905b4e6acf43833d7d3394992dc588da8096502ff1c03ac36");
            result.Add("eo", "da19f51987af6cbeb8cebfcf797ce0cf7ac729b3b579fcafecc6a268b32b70efa28cb8a220a0a1dd9eb900e29dcacd83a5ca4dd0b1f6682015f611841dc60577");
            result.Add("es-AR", "a1d3fbde1a31e8377915ebd5bbadb60c6cd20b8e2c4a80ad0c8cb3d0f47722592bdb9a54cc0f336b4c4491a80dc5d677e31a9d1518bc88671fbb3705e3b4d262");
            result.Add("es-CL", "91a89a42d6dade7d82fd57a2626e78d396f1e04591c80984244d99b7eee9beec34c0bd111c3f44546751120b85c1df0760cbe10a841e3452a4c9436ad7ca61eb");
            result.Add("es-ES", "d4bc10032f16f2cf0ff66f7c740df9828bed9380dfbe5bed43574901015e1e5fea93fb1018d3b0cd51307a71326abb72df10a91da126ca60bbc2f8a7bd8eea89");
            result.Add("es-MX", "1357cbc44a76516d9250673ecdf87dc9fd24e82bed5ee98f6b738d4cd42bd889c7daf15113aeeabdf593e2c3049b661f5305eee62ed0f2d32a83beed2ab8dad2");
            result.Add("et", "d9407cd6eac2c9d25f2d3e48580463df00aeba1f5e8e0febd3968f35489472ff9ab7e9be0c9d2a83f22c7a442b354aafde5c4a677974177571a4335597cd4652");
            result.Add("eu", "175da0e11924e274b2bdf2417e125bb705511d7064297ac73da30b27889cd7648935686002a9da16fac3cfdba6e10610a073bfec7c5cd91e519d4553dc544c6f");
            result.Add("fa", "60ef4afba8dd705d1fbcd3a8729e58e36ec8ac550fd135062b9ab7042f9630211700a2d58ed3bc22c44491bfb94f54c3b78e825ba9416e39b6327f57d4670341");
            result.Add("ff", "bce620a55e46ff9b0933417f45289392c3c261a4bf00a2e11808661a77a3dcc9943d46e6271f9d98781b34bc498479269c9769a31111cb136d54a59d64ab6e37");
            result.Add("fi", "5eb3bb8e5fb09337b152703ea0f5a21f292d3fb20c52bacdaa8684c29a35f6380940716e6037d6b83daead4972eb8de561d7d991f33aa9733306c6d548326d0c");
            result.Add("fr", "9daf8c5ce848f66c93c544ff3ea99bfb871f2683ebde5c2d300c0c4e64b0ce28c2f43154a86215b135a9cec1e590ce4f511f16eb893ca8c9c54c5b18cdc89818");
            result.Add("fy-NL", "60757a20b8f38e40349075bf98348ca25b5e5ad11babea02b2bb84582f744efba382f2ef31220c5a21acf263cc7f729df08e91c3eb65e5556a3b9e5abb00b7c4");
            result.Add("ga-IE", "9ca1c67d12c4d8b6c73a9520e7ea21738a93aa80762a7d02ce3f6f96eda44826a2afd4dd9c0e9aeeebb234ff18a5d6e81ba7c979381109e0020ebf5b3d8374de");
            result.Add("gd", "954898484547f4c4727092572865154c51de7f4b777030a39ad4a62fe8ba4e47e5bf2d6513da8678cf06d537a298bdc0eafc38eab5cc7b902d317dbf5e5a373e");
            result.Add("gl", "b23c74f5211543841e419f819f52325f4931a0abfe002e3dd2b90b73a4eaa5d2241776de75f87e322b008539faf93cf7bc1a8a990a2b344f49e5feac8edc9754");
            result.Add("gn", "68526d43b55bebb9c40d165a87014663497a779b6e8b4d919579252665deac64a42b88957e24b6841034216101d896ec478cfe85368a3c2bc3acfa34867e1cb6");
            result.Add("gu-IN", "8b8c80f2529c88c9de27b7064971d81aa622a80fbca2d50ed9c25f84f8ef637507d936c45f3a178c61c0c11047ea44b0d196fcc76bea44031187ac658003fa5d");
            result.Add("he", "c674a995921c3d11e52c9fc92ade5dfab4ebd00bd42fe61acd958779aa44d4efbc4425d106ab2506a4360ae2290b296c8d5bd07b66c6194cacf553e4b64ed579");
            result.Add("hi-IN", "262f3cbd16e46ee2c66c2b5c48ec17ff0d180bd812aba357d2271eca7ae6a9d1d93f32b05a137726aadc6665c62d1ec042863f8f4428e5dabbf181f19f60073f");
            result.Add("hr", "04486b74833889a659abec363c8b9ba5f3db761c3a0cbe18cd1a6dfdd20c346c815de9bed10f2cc38e61fe05a975c81af2a81452cf448fd8d146e4f66d4f01d1");
            result.Add("hsb", "6b87281d532aeac6bb35c181f90d11aad2f55d07f7379e1bfcda65c97dae59c5647b050ba8ff03dffe13eef12552994b514aaa1a5a154d96bea7113265814715");
            result.Add("hu", "555c02a0c9b9909b1324999e12537b6d53632f485e0fd39b7fa80f8f086c5d39cdff43fe58e8f75d0eb14f6a6beaae450d6e438b7dd7b95a5a85c2c52183483d");
            result.Add("hy-AM", "f2276f9db4b2153af5db193cc7c598b0f89fc1e5d875e480576eaa93fad8d2fc9b8e3341271826671c8e696498791d9f4aeafc69a9e59bb318379d6ed99ba3f7");
            result.Add("ia", "0391d07daad12c14628c8a71321566d0f7c70a63d82bd4b3ef1c60170facc2ece88ea59dfd267f68946527b1ba1f85ed1dfa9e058505ca4d61495e614b80d24f");
            result.Add("id", "5715e0286c30ae07ac21d6e1c6c9a3384cf76004a8bd2f657a907cdbd085bcb9f463f5e4f50c4303425e2b70ee07ec6b5f24962e7b6ce656115c6f1405d68154");
            result.Add("is", "f9ae642211fdc23926c341c849950f7df93c8752eb5056d6f49a1425f10dad41829eb5081f5ade61dccbbcbe1ff82c9eb41e6677a6e680c67a1f44dc1620f2d0");
            result.Add("it", "9701983fa0945a6df8951330cd1abd7cde7b3ffedf773d92cf488530ebbc8408c1269b8cf3901c6a1af3099868b339792f311c69c187328592bdafbf253bfde9");
            result.Add("ja", "d1ef5166bf842162d3f5219d77653d6638c3d57968230a664dad6d5adee3a423870c5626418aa5a44cd55f8088b48c24c7a3697af981e276cf784a507c1ddf59");
            result.Add("ka", "937d89e051cb09075a27be1abe91a7cf414337120f4247d03f480530f42eb332180b75ca3d6d69561dfe9c820cd1e101d95ef1a8a11c58aa44c1faed631af120");
            result.Add("kab", "1cf9bc097b229c1dabd56b1bf092f18419ee226beadeb921f99276e29aa1e28b6bacd059c7800daf1331b00371f2f0129861a8852232a282116da4af69610f5d");
            result.Add("kk", "79c2f469f0fae4a1b30ab9fe628c3d8907016f815bd3a3ab50798f7504a8fe032c6f59a2106ffd8e93926b1a5f09a65c8e665634d179e298886305bb1762664b");
            result.Add("km", "a2df20b791e2f4f30b052a405ba83f1060ef68890ad6271e7e27e278adf5a89daad3428f5cca6c1805f27a173570a81774e402d20acbef5c0142e8611626044e");
            result.Add("kn", "0e8938b370612cbd3a841acfe7412532bcec3c000529de26e9984e793918b668507ba278a9813d43c278018d42fc1ce0cac5ceb570fde16289e5c71fe678e233");
            result.Add("ko", "9b87053accd798c8f490b003fb0629d6e5bf4d34fa64244ba98b4269eff08b1caed29816c5756d7ce481e1c5c94bc9f099003bf363f93340495a5efabd7b3ac5");
            result.Add("lij", "76ad6d6951da92b562ff735a230e1610d69f731363ee8b9097b1d3b3bb4907d581ac4ee99dcf88a657b07324d77c59183afe62b97b7c9c90e665c1fd3fa9c01c");
            result.Add("lt", "38e994ab32b41db2aa592fddfb2bf95f7a555d5070c022af948413db6b3fb44d35cfa4cc6d87bc243394b4b75a46cc56da2910991382f971a87d6868498b82a0");
            result.Add("lv", "98e6d7e11688a715897b4e58f5b580c6c5413a4b149777960e89e38c50180fca6120b7c7cdbedf8130367ee8c469a3ccf02ce7867eb815445f4b6dec894a727d");
            result.Add("mk", "58b7002c8969d1d3dcc559fb6760254035b46be6df24b0a0d208a60cadc013eecc459ac876a5ad75dc4074bc8c34d4a93ca8d97c638fef3ede1b6ee758cb4622");
            result.Add("mr", "6baf9fb49fa887a1ba321c11d20214f8bc2f157af8f02482fe07cf031e70f4294a7d7247032cdb0dcebc9532981cdcbe5df939d94e30794d810df5b31eae600b");
            result.Add("ms", "2d73d02ef4fe0107bd8de5ab79221e8a63070d3b11d4c4f4c363fff5ee056ff08ab2ab6a161683f14827e8642d53044836bc3989b57d372e3000e50f063c3387");
            result.Add("my", "9b2f0bd3d7f5dcf2d7df0cd756548a9b35393a99cda05cfdee2896d30db6ec7de6c72e3428004bfb2e5fa2803dea53fdb3361a6aec39e11c40b122b83f364133");
            result.Add("nb-NO", "a88704533ba3b65e77d6c8cbf005ada5641a9827708deacc8cadfdbe61c735c007c0d7081a1b5f81366dc7f120004c41b7f088d405a9a8e555efbe1aedec129e");
            result.Add("ne-NP", "84c4974dba12df491ab72c54002c8023a7349988a3fc4c138e561a236a061e4a65f0ee1981d54488c83b281e506719fb15af53a4a8566b1160f0ac666c8b62ed");
            result.Add("nl", "f9df8e7018b7d845e27b3f840a0ffc3ce0faa6b639d1336207d84c5c33c9d2e09753c7153a5c192cd68a45dcffb3f307b65c0d312a8fcec6e8a4ae6130da73ff");
            result.Add("nn-NO", "b655152d996522d0bf560b5e25095324adcd199b88c42a37b04c23cb56a089341b7f609b2dc15116faf558767b32baac4c611a2f369121a6c3b1bc037fe2be7e");
            result.Add("oc", "92882838d75ac6fd71ab57f82123015ebed71c12316a3afce9e5e9584ae0638e66f08f435f294cc3432bfb7d842897c221266655c7991447e7b3c2497a49ae1e");
            result.Add("pa-IN", "687e5b71c23f7777c4190020b4b2642e761b9741b06d5484188e1c76e1c8ef999458e942852354a92b056afcde6009cbab770ed8064f00800905d83ae35b0ef9");
            result.Add("pl", "e7b6864836c4e7473bee49551e95af2ed5630454fa959cf681f982b214dfe18dd609032ca27b366cf8a8155844493283438e9cb0f411a66c04dbd18a79703d27");
            result.Add("pt-BR", "fb6ce192abaa3ba24a7009c9f6e693c200491553e7f6b14d42c777062bf945f022abd18cb82237b9bde1f21fba358580d075aba07ebf06b670bd625884f06e77");
            result.Add("pt-PT", "8f6b448afffb0622fff4b7a9e309875b035ce9e9ffd20ac872e9ee5fbf22b0ee8b0cab70b31dbbeef3fbc362d8ee2159e8e068f337f86dc2935edbaf87332b88");
            result.Add("rm", "86bce99815e6245614aac83f4ae475a401b9f0042ab19338137dc4830be0e16e018577e6fa9d41ab48da816b1fd6dc6d13ffb778b6e46fdb7cd212de51985eb4");
            result.Add("ro", "4a58907ae963ec86d884c8a25bca7dd7d61c53e56579ecba1bc33379207d55651b4b661b139c093ad1865bc338aa7b282c6e0c57546b1e2dcc7c11945fb97ea4");
            result.Add("ru", "c1013505b64949a637ef027d74dfa3527b943767cbd8502f123435c3df5986a9fa8868745f6e15ad2a803b38807c8a6f3dd4697d8aabcf85a9511a9c91e7d258");
            result.Add("si", "30fe04bc1ce593318591d403a07cad222ab9af30796934622deeca78e17af2d69fe8a555461bde56fb9251c4cc6e1d9f17ff4e8c98fb1d9d3a7d3697dbf70189");
            result.Add("sk", "4014905bfd8bd7e0d0d55afc3e4b156ce9ad79c2fed3af2370edd5fec58e79d435ebd6790230708058da0ad5963725f9526640761c3781721f7c227fc558761d");
            result.Add("sl", "e1c28d89f2189dc8b8175307376926b22e5d2cf6176244aaf9bfc53fea9a13c19197d8ebb15458128fe556eefeac302eb1be1a2883a102a58dac44f8550698ca");
            result.Add("son", "9a537082789f8cddc3ac71175753c6233033f68bd5473fd69ebcf5fd6ab3ee92b7520f0a41ca211e879bd56e83e47a45371baadc221f9e0eacfd40fcb639d56f");
            result.Add("sq", "c3ef60661f972ae5ab47d416a834da35aeb8df100a8ab79201ffd0edd98f261713d366f0a564ab45a1b4c4016cba8346ca5d39ac96ad2bd7a4288277ec9fa000");
            result.Add("sr", "51ba23c43b03d071a6b756a3d771391a2d653f7acb28f9feb82539f9753b37076c967b17a83b289afbdee0ad5f901081791fbd95298e479fe6750d559f40c58b");
            result.Add("sv-SE", "3066042cccc7eea8ae975beb492f69b22a375a493d96eae1133b327ea19215a053da0e970d76f2f4e395b394fd92add2ea4b14ec2f605e4c5827453f1ce38420");
            result.Add("ta", "f701f57a1a35e6fc43af966156836f38a5208408a9d2ff40d1bd98ef75323cc25b52364b7f56d5c9f69050bd4f5cc1943ad201135cd7c202d16db9fc7bc8d321");
            result.Add("te", "635be3593949e2b4e1f0aedff92fdb2abad56493cbd930734d0cd1b4f263c11fec34a6e68a2ea9ed75a8bce0d36e39c73114934f1c087fcf3e9a543e17ffc49a");
            result.Add("th", "373e30e8544a7dc628c141915c0de2da17ae5bf48139aeae90df841aa82f129f5a6b3329da1fa87396670a4abf81dc17656c9b84f67a3e723f6bbbdc2100ccb7");
            result.Add("tl", "e03fedbf221fc20a3195a6ce2432ee0ef471c5a56ac794a56f160181bec36fa4d5cf45aa40b28191f7d74ff138634afcc32121e0fc5362df5a101eade7dc3f84");
            result.Add("tr", "67461b80e7adb9baaf3f3a0cca71a9a56ccd99e90915a5b60e9c716802aab299ef736601381da7507a0fd382c937dce8ef82497a772e8610a6e29542bc332068");
            result.Add("trs", "7b921daed08ef828b47816b60f4d0c42c6a48a78a2b4618f05a4652cffe3f7b978c26b0aecab08b1df55c7f57bde1ccd62671b772c60b91d37c77b73ad711db3");
            result.Add("uk", "6c736a59ccb9a49b3fe1096d815577311a284efbdd9f12a02b9940ffd9f49b5a7e6280e9aeb8d4630a49f255fa37135fa6ac6f2a7121cb0c9172884f9e9c86a0");
            result.Add("ur", "1189ad2aaabcfa3e3f63f8870d5e0c3a944f057881261e569d3942a1fab978becd88e426243c3104788369bfb303151c0ed579d7a176dbb84e56fc3b1426f4e8");
            result.Add("uz", "921e3f79136814fd7b589b1f398d42dfc9e9914dcd61c22f6fd7f4d9d2112d64d237cae43b0924d1af29d19779738dd20cfb577deeaff03a6310c479755b5f71");
            result.Add("vi", "acc089d6a384e2c3d1bf5af04b6346aad572eaece2f8b3f25ae4dc31c8c46a7eb9049c5294b66da10ee34357d470a612936767b4243de44e1cb702a6b1c16296");
            result.Add("xh", "611a99d6e8982d7b01ba73f80792e6aaa9d3ffece7ccf14c4f69182050ea92de577f74a0e0669e405e5e77d071e875cbe985d40084657aa32da95ceb1d6d76c2");
            result.Add("zh-CN", "ca2b23bb554300415bcb071bae7285c74b6201b202a3064edceda68493c5f0f2bc3cc74bdaa9ed57223b3ce884b54923b5a7f14b898d8975d44a478f1e345b84");
            result.Add("zh-TW", "82ef7270354bf938309824d71ecfce2e0372af6aecfb3c86871823cbc785df096e39265b1b0d7d912fe905f9e777710da27b3f6f15616a51c55b70283eab15c8");

            return result;
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
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    null,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    null,
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
        /// <returns>Returns a string array containing the checksums for 32 bit an 64 bit (in that order), if successfull.
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
                    } //for
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    //look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
                    } //for
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
            logger.Debug("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
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
        /// the application cannot be update while it is running.
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
        private string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private string checksum64Bit;


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
