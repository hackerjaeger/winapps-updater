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
        private const string currentVersion = "110.0b5";

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
            // https://ftp.mozilla.org/pub/devedition/releases/110.0b5/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "97ef7c45ead19a68508081d5903f99dff9d6dfb16ff5d3bddc90191767cc08b62aad6e14df1ea8ebdc52001af1fded08971178ce4f685c4c1e2a55eebc325846" },
                { "af", "34226c42281cdf12caa32eae473f432b5e9658342069138ab7929bb20305dbc24e70c612874895a8a16ff3b372b8d8e2e6db4792c7c5c1600ceb76a3b52f30fa" },
                { "an", "ecdb328cfd5c8aef9d4c43711e29401fc81093e0fd6b5a38948f3e023a3e862bc6ebf9687f9e3d72ae361e31550f1744f92108958b189e542d928354534513cf" },
                { "ar", "f7664f57700289ccbeee0177afc68014b18e7889f56234f5c6a1d532e0c88b175fb2550a4739479722892285d2b5c87c753c6418cf622bb02df8caff68615051" },
                { "ast", "ef0b05fdc82a60847360c473123d5cf7ec15acd748e21a7a466fee031f6b54630e23585a45619360aa5c70045fab416360447e6d2f1818f7e11a1aca35f559e6" },
                { "az", "3ee867f94458e91b67542183ad894c1347c57f9e2bcdcdbf0869882b71c28875968523f22fac4e066bf40016ef02540e671fa53efa8ef7472a1d264af22e77dd" },
                { "be", "65aece94db6775d744582287c3ec1afec826ab1e501a5468077b0f9d5e1b79d3b6c32d45d799807af4b61f555b2ca41a39e6b07751ce49901ed9e1ae10b64f64" },
                { "bg", "5113a35e8d0c8c777f4eda22a5d8c90efea1b67df6f9d1508ebe6a12cec044ec2497787f9393865376af0679eed173842564896839633bc96600095fc8bd71a0" },
                { "bn", "3f8a9566d2162bc6f033b32473ad404fe73f61ec3b85b61a37016bafaa393fcd872d5f9e62978d02ed2dad702fb4b9a0757a7562043c67c3ce94bd6fdd792809" },
                { "br", "7a55509edc71ac6a11f410be25dbcfb261d526c10f906b1b41a28e2cf472fb048c4d5ac84b25685bfc5a260fd65c6c896760fc19a3eb7ec9d7b63bd37b2e1394" },
                { "bs", "4c6ec1ad24468b3465dab0cb0f2af12ccad58d5224549ca72e4060782405944eca4b05e24b6c958b93bf3beec932823696586a8896d347f6041983fe083b94e6" },
                { "ca", "39a05b2077720f887fa8d8518fe8bd7a0762dd657e2a5220c2c3bfb15195c9a8a4e9dbf54ef3a60b08ff989f8472da7adcb37f296a7705799d8f16a3c6764f90" },
                { "cak", "badb086be89216cc3ba21322a31583985402b1fd65ae29b07630b2afb28d422780e269248efc819184b39d17b80740d720e7b47197015ad3c5d42913af4583b5" },
                { "cs", "7eea5f076b43fc674ee63318e47e314b824c65a640339b30f87b562f1272efcbbd987e80d6cac0acf3bbe8d966dab7a22ff9194af6ba76872676df525377451b" },
                { "cy", "5c549388580c7eb7151eecb387c0f8b2ba8b831414e453981ac7a90aff7d3a1a4370dbfbadf409219743ecfc007a8e928d19a5bd7fff537d9538f76cfdbcf8d6" },
                { "da", "d88e793f3cc253ea7f0a5e537cf37ce3c32fec7cec49371d399a1a9d3109c9d5099853c2e5a169e8a9ab01f8643d026ac1570a80ca3289a7a5db0fca8394c257" },
                { "de", "006a542a24ecc9cea425d28102952af4c094c142bce8949ab19a44de8b1c6c4718b2933d3de352a9b9003a4e2ad61d3a074fe783b42e6a04d2ada0f8d916c4ce" },
                { "dsb", "d8ccdc57fabe29fc665a0cec942efda7902cdb413fc76fb7d12bdcb0406aaa687c2a40879db13a7f532bdd19d605ec8cd0edc0821c48b2851bdc25e43d3dd232" },
                { "el", "f610d17ea5ff76fb0ed0b46dde1247b7b8bb128274b56444b51bd59fe66cd2c63d30366ccb8942acd470563aac735194e34f3a4c9d71a89ec88f3422c5bf2bb0" },
                { "en-CA", "ef4047b47c4641217bc82fb2ca992ef7f05004b1f89f829219f83a67e37da39ac8916e37eace6c30f653f70a0bd0eb2efbd74c5893dfd1bc366d6346a9ca3ebb" },
                { "en-GB", "6ca04b2765e2ca777978460598f16dc777b038a0a24d937b79c8ec9d3b01f5e100e9ebb00d04d36c11113bfe86fcbd44219b72f00abc6f879e2cef0915a4f020" },
                { "en-US", "43cd7ec7c78842063a9cff7993a0bff015590dc4a04ec8f71ca289442902c4421ef0f905aa8d254bf7ba25468596fc917fde4e884ada23bab40e12ac78706ba2" },
                { "eo", "82c8770be67105008784dcb3ddb81ba57f0db5d96fa7d55d4aea1dac4fa60ffedea8e8c59a0449776cf54370e369ba0d492d0a00c1789856b71b73e8f9c20a29" },
                { "es-AR", "7e5647feb212cb726337e7d4db1061b219372d6df39abe1eee1b865ca59c1719887b3ec64d64a24233c7a2073548d8c267e490559389c177b8489ae9952c36f6" },
                { "es-CL", "d27f0c89714e9309c2a51a68fc23a435b0f2cc782b63a277c04d24ad2200ca015e62b672f317c063069c1345f2b5ff4e0632418cd94898d65be5dd5dd9c7e6f5" },
                { "es-ES", "a2d7ca58bdcbe0440addaf5ab00efd65793393a61b4d4b9f591860a57faec13469d81f7a58c97cd39c8864e226092657df1dabf39f9d7a5792f1e2b78c38adb7" },
                { "es-MX", "8332b6d6cc69e995f43c5c22f1fea0f69b764f6ba5a232fa1ac969a3a104c770e43aa56f0a0495a1741fd26656e516173296c856fb32a761148672ec3359ebc5" },
                { "et", "c51da6faef4305509287d9c00638441294e0653a4ab89604f111a16a34bf7eef2d83e7f3545840e58d261894ee81674674ce6b64a946b8468c20647cd2ddf67e" },
                { "eu", "16727ac8a630950fa2f75d2b54aae280171537a61f02caa3198b9af2a131c06b67eac966a830b5ab7e6734ac39dbe381e376d7f78e08e3e3ab36c4ba8eaadde8" },
                { "fa", "125722681a5cf916be19778249ce43a78a8c0996693a918a691a40ed3167f0237931811df660cdeaad62e9d6b4cfc9337db6178b0a9857bb0a67795fc3a1bfd3" },
                { "ff", "1dd53f791fcc6d0042938a9af8181579fed1b8b8c124bb7a43672ad6234f8d08d5b4685bfe0f830eaadde20c034c2124995ea605d1df1c07938b4794c552df05" },
                { "fi", "16db03ac48a1290a5d0081dc1c1c087734210c0d50feed2ddb7713af48e614e73d05d545709a6ff01837a70dea81a9cd3304c6b52e7eca195d17a1d21403e3fd" },
                { "fr", "291991b3e3ce80edcddbd3c73721f4e77706b4b0f56a07d0d97b9ddcc36bfb645a2db1c13a38ae8b71a4e20f7f9abb1cd235a13355b22f1892714144d5ce35e2" },
                { "fy-NL", "b3b4e753eec4031c86dbcdf4c7a7267e0c29c1304d9ad9b5e99ce850a1d37b4bf7f5cd4447afba652f971059e593dd5b4a1e17ac927dd076ec0f8862f2bd9743" },
                { "ga-IE", "9b89297c7e7c57e7ba19ea3b2a4d1bd290738fc68374f40d31d23bccd0ca54fe85060b1d6f79eadac3a5567645fa270ac7e99d7ee1d559604512165efb50a618" },
                { "gd", "789727cd5be7ba0215bc0db4216db34a510ee21474662225ecb678171e5b58b28459906f218aab8a07f9cd1d57f8280185b40eebdf926ab49a736671c2bcac9b" },
                { "gl", "b1dadc0925af00fc4523fd3cdcc62a52ad93cd0219f07ef52640a500da972ebeafd18f7c70675f5e8dd2b04823b3af6ba162cb72ddc0fc0cc838c3368efd0b32" },
                { "gn", "ef9a36701f425ac9da166f0ec0603865045b1ed555f99232165488a0e39a1940073cdb3e0ae121ab00f7a0a1a8a034618fbee147466b4e684cc9b0448ae23f83" },
                { "gu-IN", "46bbc4daac7ede93327aed3946f2ddbaaf514fa0e23f6a41e9ad71dc6454315bcc274fb9031db057e99633ee5e73a8d48153452572f437dc27f665bb2ef2e45b" },
                { "he", "659fa7bd0b5c82d66c51bb8fca7f2f55c38530a34b2b47c55c37ccf12d64d0566f45e05a7d3de84e3e692e08779eff2b5dcdcf2f639901c0215279da82c00553" },
                { "hi-IN", "84eb2c2eb6311a7af5373ea5a6dc26222749a9799a2335c46af7b76d5eced2bd16b5e8dcb4ec9c31eab7942a5651322278939a1202e1407f6a5e2db65d2cd543" },
                { "hr", "f8bb1aaa215b598ea217ed1e842512c96e1f8b8de228040dba84b6287966d9d13d28f6d4b0dc1fc6f6446e0591f7e748bba7b2f9762de126d69a16288da7b12d" },
                { "hsb", "8b9ecf9729500336f335f8143a598a1eeaf4696bd908de989ee29501b365a150677959bf2d3ae167e2305717535570775397a9e68bd6fb8351536d305863c9e6" },
                { "hu", "2ad05bb214255bff08b732e3ecbf65c0baeaa914241e78f4a5c47fc49e36b7f693c6e252f801d5672fe3e6ca565b32187b97feb602203f219162f2851e204561" },
                { "hy-AM", "50602a5b83de3b7dfe91d0d4a30be800a9330e0a37fc5d9bc039a82b32911f57a09a432c6f724bee6ae06091e236a67311fa005fa7143c99b24130d2d3cb259b" },
                { "ia", "7c0b2dcf9589d977f5cf8366220ac9fc3c3ce141f378aff5a6d40bc037a27b99ab009fc2c1678bdc9c958faa1d715d941d5a44b83d974e51379f7a6d1e722535" },
                { "id", "72543145a1a1a54bc720af014e5eeac14355eed97ebe618fa0eab50e327621822c8942efd51fdf20724a7d708047c55354fd7edade4b251a55ebff78867a3646" },
                { "is", "3e1fe7f6eb7ba3dce30265292b926d2b7e53fa905a6c8c1798e930c80d67b87510608cbeb09839e892c7982fe2c73a8ba2873bb0e485befa9b14a04acca1fbbb" },
                { "it", "13c10ba372afb76a0a31230bf7e9534679c3ba3507a6bc9a5d2913ec45d254c2e3ca416062da4239258ad8e6b51ccf5dcddd5d2bbab153c92039557cfba85293" },
                { "ja", "60d7fe5afbf34c4bd858b0ecd03508e5936bd55290dabe8f0fbcfef32628e9ba97315d62d5d2240577f27d14fa6d4da4ed76d046793f524fdb3b1d43f8eff0d4" },
                { "ka", "dc7afdce082c7c94f07e9f8242bc9f15827917ab0966acf30c3ad9a4f69d45f38dd114e20774c244f307e29f8bd0cfe470e0628802a0a73077e354fcf64c028b" },
                { "kab", "55c896e2913259989ca315ec1b79d5aba08bab7b3b502382d53d60c9dc42ef701398d24105d81992df7547378e52c413f4e52079bf21564478a2a28e0bf660bc" },
                { "kk", "770c77d693ac24270abd75d032cdf116650b7542092969aa538a7200cee4bd7ab77ca1de90af414aa77f2bd4050ba04d0fc28ff55ff01dc92a8d0189e46884ae" },
                { "km", "b018c0e2bee0f784981144ec705edd18ebb988dfd90c79930860acd48a7677f572671b2371a08abb4ee899ae4ba6c6264034b9e60e4694a9d6a59d592afc734e" },
                { "kn", "c217de1ea0692f65d0f8cecf6a8156b4efde63c8b6923f354d40b697328592ba89e08ba12271bb41c892ac7353387074ba1705c2735f458d53f986c45144fb57" },
                { "ko", "38ad8ad1971e779f1996475d50bfe57e816772fbd6eeecf0a9a64c07325679ffa39f64679becf74994a64b92ffacda47dd471ec7c697759e47be0e3fb3713c46" },
                { "lij", "21fb0ce6ba55e4b51139d1035a55b7e0635d45c710ae81032eb3898b32eaf740b2223ffc742ee9cbb5359d3420709e456b0f0c0c3d7413f07ac464c89b7bfa7d" },
                { "lt", "6a710235ed3320a4a8374fdfaa218de4f929ead374e9daac140b8251787dbacd5da4c22f1145a5e5c5d20faebe7b52bae09d96e732ecd8a416267e98e43948d4" },
                { "lv", "e2912c7765ed16876937ea97a336e427a70fbc237043b01b31c26732d65e86b791485bad652cf9cf7dd8c56f574378303384e0800fe5bd543fec840dc38b9e49" },
                { "mk", "dbbe5b3560a61913d7938d8e11520bda868b71318ca6574e1e2f19fdfa0b1cfa391f028fff0bae53869135cd436fe3bf454009f30b978356d42ff06a7cf819b0" },
                { "mr", "e43e3b69b5bde2a2e74492e213488afdb463b9fc36b6939ce51693a8a85f666c36aff1807ed6d8992e370fa549818b30f9d357bd6091d9fd86956ea25907c1e1" },
                { "ms", "691c9be4f92744decfca72b0ad43959a0d5b13aaf9f05d4cf5c1884e8d5a3c47d17913e0c24b88950ade1650bdc5a31c96e185a4711c8826cc9ebf9b288c1d29" },
                { "my", "7423e323821df2796da458bf4de1f25ef410c14c814a93e2ac80eeba0c8562e315063fdbfce2178c704781b83a5827f62a04fc648722db9984a871c51792ce55" },
                { "nb-NO", "5a25ef2c18534c7c0d4004a42cef9f8e4440cb8c30a10d20d828930793fa09dfd3cdff6ab4e6b797e06f2ee10a1e5ac0681373759193aaf20e856f93a6893432" },
                { "ne-NP", "4d26bf7a5875db0f110c3e5c1ad318a53eb29fad8fe731a5f09255b78c56e09f03a530c91daeb97f1b2c36ddcdf04d4c81f7fa0833135bd0cd33a87c1a29bb6b" },
                { "nl", "5ac3b91a9466d2123e8959d932efc119fea48f36187fa57834ad4eea290eb110742ffc5ede95cc214c5aa6c607b8e84e403b28bdda44272780ac839ef2071302" },
                { "nn-NO", "08a42e156cc5afc6043ca9ae4efacfdf437b61b8a522184322ea6b5b5c6160baca96684679ba4d5c22b382be99bf57b6d6ad5ef0df4ab0a1a4cb42e640657c52" },
                { "oc", "81effd9d0b498481942d8aca2cc1ad1b61d330568147464434f2ecaaa859c1bd1ad26b4242d5726d0dde70869a25e2936b42cd66d7c509b2c92a20e7676df801" },
                { "pa-IN", "4ce973d5019a17e3b94f9bc808a5edfd00f30142b60551f23f567ab7f119616c9ae3f450f5b9806fe681f00470bba4a41e7e5a98e03f05accf4eecb1070cbb55" },
                { "pl", "48a3edfc4660dba0982c30201c74b7d998d92292912af8e3a1782c0538d65d4dbe687556b2d4e2611f9579aefa8f1bfdf1831c6acf44a60007c2348dbd12d8ed" },
                { "pt-BR", "a248895e23b453607a846b60867d4ed1d043529c83e13b095d9d25bed24b30253acc8016a034c4dfd238095166de37480616d1d75dad932efe8fc0f31a9d696a" },
                { "pt-PT", "7a481114c3cb31c2a303fcf832144ecb030ecfa7ae7bbdd1469aa9ba5ae085d5a781e308a2c1b725d2222884a3f21130ba6a1307bd18e9dcf845f4cf90ee3f9b" },
                { "rm", "88b7a7571d101bae17b8b9654b8ddfb5f70956b8350542cfca83c88c431f735cc7636c2d0a7032c9a3253a9dbbc398fbb6200514bdfebfc1066b349efc976b9c" },
                { "ro", "b4333a4169aee5f6f6f2bee4124f666b3fe573c904da398a43806f520bb07b30288a58942961a78b95de33f30b3797a9d82050443aa104e0f25b4cf3d9d91226" },
                { "ru", "8f5049919bb4915a53b7745fc31985ac4bb1d517b771583b33ad953a583b8345ccc3cffc058fc69b602bf59d4ad3eb5be5fe9af77b55b7fe7ffd80f7607fd213" },
                { "sco", "932a99f896ac335246eafd081e956773eda45be1f56251072cccb24a7490862317a8ec7ff8236d3e204154654a0b20e43a7674a3d6ba69fad48a421bff5b0142" },
                { "si", "ae8732eeb537afcc4ea9d9ca5bde3809a380bd60188af2f7f847648fe3039f8b7d9cb1b256d360407ab810bb681d2629ee70f128e19573e065f12a907ec81aa1" },
                { "sk", "f60ac7f9a068cb19ee05d8b3b4888d9172fabd3be6b0e81e8d1f1d8b61104fe1ed1716e2226f466ddd107ed80b75912f2302046d4c6e2bc319ac8d5e3c84037f" },
                { "sl", "627e0ac6b5ba09babd5a8f016dd4b55f39f1f1246c746179cb2ff41f502c8d996bcc9ee60b9ce604c31af5a82b3ca5bf11eaeb2e17ed643a8270d45aef59b43c" },
                { "son", "7606132332dee7e8ca7c9a5e7b802d3193f7d3640b55d0f8c7d018dabecd15077e2acecf5451391d730f9e5b4160126581058b31547a92ccb58680b027cbc198" },
                { "sq", "00cc7e00db5fd99b924c1f8c50c9517e1a9f860124d650b59ebd9b95aeedf9d40a36b711e95d88031b2bb87652d29f3ffafa91f593f0dea8d457e73589ee895b" },
                { "sr", "9ba81a88772c559cd0221ef854d15db28a8380e1c107b73f90c64091dfbfb28cf3483aa83183536688bcd0a9486a8e19ff87bcb6c8c0906bcc2f01c9fc66b1d5" },
                { "sv-SE", "38dfb6241a892ff0ff26c3bf976c1185e72eba67e8e86d5817581c6a00ce1e81a4cbc0d4e0a073e56c5abd2167e1c5f5fa3200b35c10a7be81fe7bb1699e25a9" },
                { "szl", "3a5714b76d887f5111a65d17b13493d9c0158a680ea9cd087a856127fcc832cd807e180f9f601ff344f7dc1df811c6cb930f6240a9d85f67dc5c220ff2b63f38" },
                { "ta", "5b41797f38da92ed4ab8fbadda4f7fdc3af89a94023c4ec0b159b49de2e89a0506391ff424eac1774f03cc8fb66eb3f7662e29e3c60f3f4238386d106ab02799" },
                { "te", "9ead69d8a708df652baac67270dda67de0e48f5a6628aba18bc2138958cbb511da86b7a5c43c8a639c6f0fa2f28749b4655533fa4b07ff65f1f7f4be2290fd0d" },
                { "th", "d24406c43ea497344228b1a8c7da84c742fd67e342d20c8cbf0297086d8d73bb1922bf54cadb7cccb9af5b79cfe078ae8ae9c97908f4e9fa90bacc7144fda1a6" },
                { "tl", "b8b4dcedc599256550fa2d6305766e5793194ea70613634910bf8e8ba3c0c50a9ec993ce80ffc9674f1404f1a4715edb22a49d886cda76c7b9e3ad5b5df208ae" },
                { "tr", "9373b74beb2825321e014e103b4a22997efbde4278aa7d9355b9c7405813bae164fa7191ce5139031af3fe90c2d1f842fc6bd416e0ea70ff0cb06b74b8876f70" },
                { "trs", "76cea63ad0bd9b583f2319150abf847b6298fb7fee8bc94898f7844b2c47fbd053dea8e69856ac02dfc183900a911fabae08e7e57743e4e64ae6f13699b6fab2" },
                { "uk", "7a3ce72c7f95f4add60779100d925a66f8fc168e61731ce8c3ce5c7948edbac9a4c86eb0992d6d0b3013eeb6fad6f6bab3c628f0b572bb557231c8e7476096fa" },
                { "ur", "51b20813db3becc44e19bc815bca2a647f48e4cb561b7a5e947670e6affdb3bc35b97d5ebf2114dd48137aeeb3be642d9db37b2ec251e4df6c6f4756f410ab96" },
                { "uz", "a995f14c46ef9a436ecf48df96e05c9753e03537fb0d5d6325d591e801d2e8779e86079612f29dd9d16da216ddc350353112f88d037325e5a075b7849ed799e4" },
                { "vi", "b1610dd8694c163c856bc542eeaf5c8853f9dab4d448bd6c48ddc37d48195ff085cf8859d7210b75c03677a0f39b5de05335afc2ca317c854b810c94dd1c9993" },
                { "xh", "d7278b0dd93830d391bf3f91b6cc4812ba01e75e6f6e4e6016e086e1da333064f3311af43375308c195289d81f3ecf20140e9e03d6e59d65b56ed16c4074cd83" },
                { "zh-CN", "2cb2c660320a08460d8a95a153a794eb2ff818051f913ae1ca2c24df7cd49e17349d19a9f9fe45c289fd461daa7424a8370b19414953663b03b324d1786bf7f8" },
                { "zh-TW", "15eba8edaa98f20ebd8b10245b11f00b8543da08c30f2785c3a6be218d21008aa4e6d3501efd4ad1b8be71c1d35f23a40304154945ca24987371271c03cd80c8" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/110.0b5/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "3254d702c9a61e5d6fda73905d62406661b96a20e5edd23cb14aef5c9527a219837c6a72969fee94e063f578775094669fd1865bba17d5cd676116a95b7316c9" },
                { "af", "a70483aa1185e2f6da9865ee2dedceb8ee78703814abdb7d557aa84f5f1f63972eef8f59de0359f64a8939e61262e9529a20cf15bd3927f96dc2984f7b1c4734" },
                { "an", "796b3c35b301eb47af8dcf914179aa3a7feb7dd8b34dbb7c1d35e5c80824602ff26203bd68d6ab8a31025b2c9be5c3e9a25832876eefd2dd52655247706c5363" },
                { "ar", "b998dcc4dd24f8f8537aa2bfc008f13c912dd18790ce6065ebeba873b84dd072f848b794c80ce4cab47bf9f6a4707b6efd6b6e258bc4f512fc6c6f1409bd0ad6" },
                { "ast", "11f41c896790031175a8bb64fe54f21c168ad9f11d40f2f74ca0e6ee38bd2f000cd85554df8a76b884684758ca856a97ce502fcaee7e68091c46435277b43fa7" },
                { "az", "63d8ba6a697188c644c4c3e1c102360d8f95efcb3cf949e5db427b6ba86d5e7ef45d37f91b89a6d7b505dc40422a6776e168aa5fcf9109826a040b1476d9af1d" },
                { "be", "b4e5391209b5b96b87853f43db48880d1e081f2bd3aeed0c337e86f4305f4184b4f8c8993aa625ce9c24e41bd4df24523f59da952779df6c47cbbbe2fe44894b" },
                { "bg", "47fa90a3b9df18243d30ae453c10ec00e794e2d2043a623542ffca959bca11a5916176bdd5114a9f4566c144f44a5b98f368e0922a76e646fb4f520c32230b11" },
                { "bn", "6122e0335b16cf21da4b2018b3acf88ab7606832ae3ab2545ec416bfd3a026353e10e55f9beb044c92735506897a7129c406f88ff7f60e8617f369c299018ebf" },
                { "br", "e9c46a2b008f0832290ec3c5d1f6583f7036a1db1fe09a19f4859b5118969c890bbddc26a151416582c4c37b2633c8b6ff33ec3de17eeaec2755ab34fb1d78bd" },
                { "bs", "e6782d04a2f3cfa4b3447982d1540ba5a9bac6eed382f76fac6511beca1fae09db4da1cef93170c2d6dba8f7542198ff1f15297a0d301d12fb96eccf0c56be69" },
                { "ca", "8a7c320be00814e63e704e900f88c86d55b21e888569bf20a941e13fd25c56b6f2d30a51e822b3a0f3719d8f089c840745d8315d86a0af5b3ef4a1a59db83551" },
                { "cak", "bc09115c5431a04a25d7bf75da0210e49d9a5cb99e79d24eaed9ef171acbfa9467070f7279d152e96b432f613b82f1d3a5bab2ecbb91b038c0103fcdb6fe7b0d" },
                { "cs", "cf64b3b8ff403981583f968b3a8017b6a65323b59f2363caff048d0cab8ae1aebd1cdd859379b836cc61a25b002f0a2afb715f685939f6eb94e0d264b1040c3a" },
                { "cy", "2ca612ceab63a60b90d12317f91e2ce62eb77ec18bf7e31d96ca0d417f2bca05d70dc2e71f47020aea8a736df08069f9c07ba6ba58c7eab1d4b0b127b90bafeb" },
                { "da", "567c963457c5b7e0fc9154e8840db70898d8bcf6279a694cea16b4d9024fee1f56a7cc7ede0cbcf1f3194d4de7e6ec5e259973b43991b72084ac38d774ffb64d" },
                { "de", "68bb06200dbe5c13353fd50faad6a9de4a16218eb9242d7a37d314e03b645d4462e226194c14106306a13c7014658a9b48c45f61df0b0eca62f21ab984dea934" },
                { "dsb", "4ca8e274828ac43d6f6191a42ee548096c6242f540d49183d1bc4fc06b36d60a747dfedab53d2356e4913967bbb781d6108d3009da620a83a3b7248fc1de431a" },
                { "el", "edbcf448048a26033105d6296f39cb1ad22845436e166ab2bbfa04a758c93d0317dbb75ec017563354d1e1008c7d1b1d8bdea4fe8514fc901490d74b59d34e90" },
                { "en-CA", "8071b5f75c6ae70cba3e1be0a7d8247f22c7dea32b826e401b579c21f43f52d10714ca5f5dfac93b0841bb09cefeb84681c4ad5a208ab74f99eac77eaf0a63bd" },
                { "en-GB", "f8c19df8d12f06090d2dec07d342dfd0396cad9abcb895699ae1d1723d883ca6a0a26a1ab10127862d94c6f43701ece841f98065ef3349770c16602e700df363" },
                { "en-US", "782eb6a3df04ffe7b469088c8b884dac427f2401d7ff4cd36af37123697c7caac05023bdc783726d78bb5203e4cc3895809c66daa005185510c02ba6e9ff4988" },
                { "eo", "37a5a0957947d64e519876f7a0648bb7f866fcd36f583f36df448f772b3b5b7ec97bbcffb97bfc5df2e6ae63e50f159030563bcc3b9bff08ec2ed3a50cf5acdd" },
                { "es-AR", "e633b602473c0755438e750e1a201a01ba3f30c622c8cf86d98cc1fd84a5ddd4f877fb2b99d85ec9acdac3acdb499e4d3ea790eba2bf58fcd3220c9471643865" },
                { "es-CL", "0c126cd4e24858e97039770cf6e300d95fc24abd9f483a761a98f0c48b2ee039239561e63496594968f3a7372a314e9f2400b058aff7272ea0300eb3b52b096d" },
                { "es-ES", "6e67b7adc028b46f5c0d2eb6679658c1c6861cd802af5cdc96269848c590e9a1da56ec1b05532b733d28da5e69c419fea3233e5558775797f42a4261b6a5daf4" },
                { "es-MX", "29dc11bd6c3f2c8106249c355af95edd21d066900890451116b13f6b65e6987813470e9abfc1a8011a00b7f50db35d787b6a42700e26a67348b88c9c1e6f63e2" },
                { "et", "2d1c8c749cfadafaeb5510deff768fac5c47863d7c9f82e77e063881e49f62d9a9f869c69e21ae2da82a8e1f038b978839c0398a4f2db0eccb5d389a1f4c9510" },
                { "eu", "f1296217233f81a68ff7a053994c36496e087f83bf47ad7d9154f4fac616dcc71bd60d3dbf652231cdf563d1bbab07a2fd3ac489883f7c1358f8b05c2c621668" },
                { "fa", "d189801bd20ca39598a287c17644722bd6738533ca5a8153ecb37b1a639a10425d9913f4c275c328510aec4627f09c59f6449939da966ec54897abe1347cc6c8" },
                { "ff", "833d3a5fd1a7e6ed82c33318cecdf0a0647edf9c1809a6e16c86b2df9cc70460a149c1b7f3e30f47cd4cb3c439e296cafa1d6988cbeb500d69bee8f8e6a4bac0" },
                { "fi", "55a871f1254a6b0542572fda856362000954bd3dadaf99c53dece67d7c020b18681dd04c227bce3c9a18003aca0bb08816d1b33955d6d39f87f41eb04c67f81d" },
                { "fr", "f22e4938c3ce9f88856eeb39e6aae3dfe27bbb8cd16a8a5f03a623822aa9e3f1ae03bbf68e7eee6cced899d42ca803ca4c2d65f506dfcae7413993b1d758cdf3" },
                { "fy-NL", "8a920e6b7ec14116ae062526ed044733502d1ab70c73627b61391e45e3b4b6955ea0763dec8c9da0528f313f5e68a1dbe0bb76888470b63ee14eb6d3e763b657" },
                { "ga-IE", "7bf9aef7484b6ac35028d9c6ab0f919317a3a49bebc911d76dca1d65f2f7fe35eb02272753a9824af954901e749d279402a5c52c98e9b1b468a25d0bae1720ab" },
                { "gd", "dda089ef2f5c84dcb97cff02b5cd9bf3146ca1d6ada898ab6f79d676480dc7efdb2ce5e5090a443bc870f88f85b8525db15782ae0e54613c42be76a547f6bc0f" },
                { "gl", "c0d3ac4e5ef9743d1b20dbb8649816694dad17887af1089ff2bb5026a47bc603a398378e175c070c35c35646b3394865ffcccfd39ca2367d338148f925b18555" },
                { "gn", "3e821178bceba1fd622ee7bdd63bfe1d05a855074d0abaad1ccf90f91e7dc51e6ca1ff2f0438ab7f6ccfda3d6126ab9aa136c60f8fdd86bb72727643a2d00ae5" },
                { "gu-IN", "c1396bcdeb5e7c55aed394c8b8ba57c9dcf97b58c823aca64f9c1779655af873a191792fd70ec8b1ab2681b7f22680d217673952b3b54d5e2e9cd2df12b7017e" },
                { "he", "f01bc43d5d868d9508419aaa59e252a5d1944f9cf729f929e36175557fb81ea55eb5f1212ab9a0ceb9e7dc47eeb9489b1426851b53d090baad7d2caf9940d223" },
                { "hi-IN", "88a78c897c4967b83d64c258786d6e707fad9b4f36d32eff0adc3b3828350a0a8cec76cf8fad2274056c532a829a73634bddf48d701258b8514cd14f88e64b1f" },
                { "hr", "e2d1e54c843ca539b5eb781625c94f8d6acc1fe4d6056c1f1e578cf74d6d423ff8279c66a00ead5656c8294219a2c9e7a223d694130165536fe18d164573ab4d" },
                { "hsb", "432cecb36693a063aea0759194cdf6637c1d9df41622daa8361a1962594b51ab952f2394017157f34db7c7460879de6c289e75559bbb3f6a188f422f8fd19df5" },
                { "hu", "8ebc5d7ac6ac7305e756b0055a0843dbb321f7bec2d80f92558bc67b4e336d95efc4a3eeac1ee2a13330dedaf7a0ef019094007465d497678ca2e40d1819155c" },
                { "hy-AM", "e889fd7a8c6434f9a311a639d0f94212d9e63872c4fd23f7a296efb64d2518dcb402e6583eca41cec62774083a75d771de3006d4c3d317604bdbdb0d58e010ba" },
                { "ia", "a957ca11b97a58e6e6d51fc2bbaab3257a98a18f57052bae5e2612b3d0497492185559db4e68414534abde83fc24417eb82f9e9183842a017c1f9e70c2c1383f" },
                { "id", "4df83c7c7afa48cd795ee9fdb37ef1f12d0941d874cd543332102f712572ad38bed04ae45692c685584ac6a1926b491ed34ad24e3a03a2571cd8d21d2cd470d0" },
                { "is", "08def6bc23b340a315f5aebe4a7d12d830afc445408214b559e828fd23a5731feae8fa7572f5d5bc36292166fc1c57b6685fa5171ab2d2084dadd98a249cc2d6" },
                { "it", "13e22539014ac6a8435e81a2e20f8e34afb8b3b46ef9d19e877c547795f277f9f60c9884cf04d853d1d9d6dbf59ef14fc9f95ccc112bd9842eef724974135abb" },
                { "ja", "490a5c14c16b86a8460a3e245bd52d6e11f104c1556b96434cc191474fa79f3cc5b2fd857e00f8516e05390f629556b3600bb923752fa6a205b29c390145477b" },
                { "ka", "6c8d2ac9621c68b900a747170f9f65fb11bf8a706e592efd44b438c02a2d2339e14ee207ba10694e05c3c120bbbab7eb27c7a90f33ed99cde747ddbb66bc7409" },
                { "kab", "c353c34f2db1536af8eb971c9bde90568be7242350948a85b0c42475ac287077d3d751ee2fe888d94c78ef684c2aaa17c6ed25401fa8214e5e9c0f420aa7825e" },
                { "kk", "7e87cf2228aceba941092b98441e0d702604010da046d486acfc66d7286614132b6ae487f06586e7cf3d8a4bd344c84d3d104a11b456d551ceb194ba4c9e17de" },
                { "km", "3bb8dc67fa5596fa7b648919628752c3f2c0611dc9799cc5b56c9361cf51695cb7faa569e9d06f413477c8412c1dbf9d8e52e9ead06a2818027ca8c6178b2aa7" },
                { "kn", "5ee479c44ce9d661290d4bf1c1d88223e119b0f83b65255cab6f3fbadfc742a6b37352a4d1635e179c14ca4796baff235bbb2019bc80c0448c320b852f643b3a" },
                { "ko", "dac39ace57333e8fca4eeb814081e3cd52996f6c358843fe62603be0a9ab2de698109d4ff99743e4b91185fc7f052b5c65e1b1ae4f5fb59ba44dada5f564d6a5" },
                { "lij", "a55cefdd084d5410a23836643ddee575456bd26898e177b1b282dd733019c3676278a72673463a7eb68ce8fd5e3f31a6e13af7fa6de3904efb776f69ccc70559" },
                { "lt", "932c613f0b5d0195b406a2c271793c665882b73581a30eda201ae0df8cbf9b3a183d521af5895d30e98411ff84c51368f58e8a381e98aaacfa1bdc86416e3156" },
                { "lv", "c1be9d6875a592f7f4f891919038012d1eaf3648ca5471c2ef294e4b2299b7b2e8c512c8099ba40654f37fc952cdd9dcdcc4e2f73ae1c10ef95789183d03da8d" },
                { "mk", "cbf1e7e60ef9cfa525f8620e247c0b875d377dbef00abedf3a8cdf274e8cdfca33a35c04f64497ef50a5066d91b5619027fef187388a86e8f0a3e94691c6edf7" },
                { "mr", "e4b8f5cfe1a36735898e7a377a97aa8fed8f7a11cacaccc479fe3f54d04a5d9ddf17ce0cab3bc3ba2efc2f79f4a0274325c6ba0963c3cfce98a1b5f48b4e2d70" },
                { "ms", "08b83ff797784781560208eab79a7e46864d7a6c0d553bf401283cec8d0246b1f0baa07407a517e12cd298e048745348599ec1cb6bface6c517adc579ee32bd7" },
                { "my", "0d94586c27801c1d86617d2eb3d44944d43c7a0b7ae12f4a90163aedf6e0f9c855c2341cacb1afcdcd9b540270d8a8f2aacc48b2a1b761aa54287acc3c646252" },
                { "nb-NO", "62f29006f88d5e7a51dfa42356b4544c50162bb5e28f668da4f28ce829c9add78cca358e92cae00f2fae6f6e5758179252fe76c6bd22e3a624a698a04887c7d2" },
                { "ne-NP", "84e972ebe7592b02a03e1b7aebaec61a9e946a99b453f56b8e05ff7e3a5de210ece0dd13ed5f250fa3fd20ea00e7ac379f2fb3784adfcb7043634a10558177db" },
                { "nl", "73adbf445bed24d1e69e30095c33a1b6bb8faf27e47c27bfc3340bbacb48779550999ef2c66b7fe9a7eb17bacb75a3a44a9a1164cac5be605be61e7f9637b20d" },
                { "nn-NO", "338225ae93d0684f08ba3e1dceca3c97e246be3bc3548038bc739a0f50732d4a68d24f4a9a178518f90e02f41cd5603c84134d375dae84c2801038f634017b76" },
                { "oc", "40ce44075603820285284cd1a4e8d79f582700ece0479cbb4af6b22ef4f75023657b12c117acd6d5c3e61d7c89477d0273518a79ad5142aa203747e0f056259e" },
                { "pa-IN", "2db6942d12416abebbb2f633c86fdbefdd94457e692473fd0853e9928cc3faf9cdfb65c9abe2d69b39b4e8587fc0fe07b22974a392ea6287fa95212f910e5026" },
                { "pl", "dbf34a9e2ad9e543cf72f2fa2e3e755973fb237e2fc8ffff8cee84434da1fcb0a1221e953ab73096eab79d5d7273c01716a3069d65953df935c54813b5b5e74b" },
                { "pt-BR", "2f0b06422fd6632408189e7fa1c05070f1a913a8df7b569a7f9d475780f6ad9ff26de993290a59e35b70e77a01189f4e591582e10a23b4ee92cb24e52a13f1c2" },
                { "pt-PT", "6d49846f70944497168db94a9643b8f92ca59f80c20995502a816e5aae8c532cf596b28ed7ee818c2130cb372377ccfccc8c042171177b9f9d8090cf18e2a28e" },
                { "rm", "84a03065e2be623262a290bcca537ca523400eb68c77edb9770fabdff1c917e06c0d11972f7aa38ea4cafc3658863fa185fd03c7e2df5ab4b64b35c0650b1f22" },
                { "ro", "4f87bba587f7fa5a270b2a7140c10cf53d61687e92f3e5e41e79009a5a0e8144ceca971871506be6b44000a3f5640b60869e77be5ca737339831e2bf2ba05ce6" },
                { "ru", "5576dfe6c891da5a52a4dd4e09ed5ea2ba3beb4f571bc484e46b6fe8a81d9011708159490b07b69c772d94deca77f59f1c900fbbde6f41c3d98179564aa9861a" },
                { "sco", "83f898f3c5ab8fc60372addfd7580dc42c3cd9722083d364dd001eab5b4b51884b70bf4d017580acca1e918e7821668ac1bac74f38970d8cb5e425a36af2ec12" },
                { "si", "b47e21f6473cef354e818ed7d118fe5dcd5b5899535124792608896d19d37768728ddcb93f7e04e98877ec1679dd054630db0715158a7fed3bc23daaea6d9953" },
                { "sk", "654e89307c56d7a750e792095a3ccd2dc729dc1a052842dc05c3966d4b1d68fe8ffbeca5cf298f427355cd7258275cb814e774040e0bb3dcd1cb2f378b7d9db5" },
                { "sl", "7633ae2bfe51ed57e2c4721b038fce5df5e1eb4249a944c32fff3ac25044f350cbc70d111265db1736717ea04e912f78d1a1a45dc12459939ef2621e92400dad" },
                { "son", "90368d5c6561b9e025d290cf00670709f9a8e6452cc1612215fa12901420c6fcb705bdea65cd670bd845638df1d043b9453f001542de4f87b8c9c199768a673c" },
                { "sq", "86d30c053dc2910b23916701908aa7d9fd8b92e38097a81bdfd24e25e4a94990d094129185d4ed7831427e770674e4b88828056dc1b5d5070bc6fc2aac1d9c6b" },
                { "sr", "283147ee8e955c188f41e554da8366c02c9f23563ba4af1675e38daf006257715171d3855338a1811dc6c79c7459995c52470044ab2c557dc183779c541b2cb5" },
                { "sv-SE", "d8c71f8826af529bd9ca88ae04bc5deaa42d4322ee122972c666ec77f1d9155e140a75eee8e7bb655604e697edb47c19b72aea51ad820fcafd45d2eedbfc8c76" },
                { "szl", "047885162025feee6c60c737ef1d0fdd182c9d8a687cbeef82da1ba2f2255f3d5e6db66b9e1ff4fe6bb392904eadb607dd87fca0edf84c20f79331b513f770c1" },
                { "ta", "3ebdbb56f6b16de85040b7b6691223bebaa81044fc0a8f8a94f9d6240e225e877b7d36df32fe7f45727e2b2f7e9c0644979adbaf9ee1afa62caf29f9a755225f" },
                { "te", "07b65f333b42ed1800d3b068b4237af0fd2d1c4c5fceb0ce2cfa0c624774e7f305202cead5af0623b8e198c8c22c1eb81657449dd4dbe276951dea7c64c0bedc" },
                { "th", "84392a0a4c92fc4079160381b33ff470e998cec7e62b3b13feb82339c893d3221103186b0ab6b70f01d788fcf5debdbcc9002b029f406373eb8d1e4dafd0a104" },
                { "tl", "483855a97f93421a0c58d2792bbfcfc5fa4f4fa756e7fca275f06bf0d980404d2bd05f80ddb956a91ede6979f91545aeafcc36c9f17a7f4e1d3edb6de55a1f52" },
                { "tr", "5a071889cf87a96e5645c36bb682b1c7cf7c15d45549b1d18562e0d0708c50dc89c51282fa0a9fbfc9dcc4198a3ad6ac5fc730635fa4cafe7e74fb3132e101d8" },
                { "trs", "1c6a412db543cff3e608f987b6339446d126f4eb1e5b45917cdacf9d022a66d98e0767135dfbb0817c80d97bc93a98a4750c265a8c25c834c73e43da2d83c0b8" },
                { "uk", "98b8550a6334fac7cb21a9bbb442576fc62dfbf8031527c1217b82be7efbe01b4e12344275ab97560af4792162777cb7ebb3548b14e125581966c134019db423" },
                { "ur", "4aa8150372ee21d181051babe920637e071d67c1cbc7fd4aed9a2e9096b75a6d87863f8cdb17eada55c2af7ea8130421d25eed98e271e7adf1cb1302b22e8d4d" },
                { "uz", "5f0a42653232a22538c8648f04d377734b66acebddbafd0f6b889bcef27b0bab6a16d121c1e802a915b3af0912e9474df43c3c589518c52525f71ed520fbbee8" },
                { "vi", "dae714c853ffb7ffd2bbd079d4a6a0f36c6a15cd2399edc41e9899f109119d6878471d46e30d52e711c06c16125e625057b1882d3cb0bd9e2ca2f99b63a107b9" },
                { "xh", "45d219bd137bc6d56517874e780cfb84921c9949237ea0a7458d24d0696ac47c20d317d127ac4542240fa5c5b17e9f80c0c8fe92ef8905dc076b7b86750f1c79" },
                { "zh-CN", "9e65e414cf3f93fc163b1f3d5200256472b8988603d2d3810e4348f3cf1278b61af617ccfd57a4b5f51c76096ebeceb4a6ada948c8216db37aa857786976c02e" },
                { "zh-TW", "64d2faaa7ccdcf1dbd91a8ba517f75992b94219bb557d81981ed48bacac8b98a8f521e2897bc11ab7b39b31c8f313984821695edc418eb0a83dd03e6c4257b8f" }
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
