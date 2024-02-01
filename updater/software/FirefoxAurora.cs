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
        private const string currentVersion = "123.0b5";

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
            // https://ftp.mozilla.org/pub/devedition/releases/123.0b5/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "87f36b1147a2d4506b5e7e8311e46e5e92fe889a0e7efc2e0330a026a40d176b0a4f350391cf162d9721ce0ae153c5216fdafb47457953eb971b2284715f0d8d" },
                { "af", "006f79cf4f0e9ede0776f0c3da1553bbc8150e997fcccb4f2813565de9241d657fb0a0191def258eaa2650ece841a9fe5575b69456b565b4c8cc7daf07abfffc" },
                { "an", "505148add63d1bff517b4679a5f8b5a0ac61424bc24ae3b3fbf93a48174d5335eed217b4383b49f8b0f3fa79601f7ec26058db528f9cec9e2019fb9846294e58" },
                { "ar", "982a6345428a52c60753f9e929da90c8b6bdd598e48b46e5f8b8089479574448d3b4f52ad55a9867e85ffbf5031de360c340154948dd589117ba7a3e2b01805d" },
                { "ast", "dada7d16fb3de44369e0ce0e9c50f4661bd56d18d190dee7e9d51e40eefe54b49bb9a080a63d880233d7a47ed9b6ce87a2fae8df110fd6cf549f4e02d5cc46cd" },
                { "az", "8f1b2dcd992b5dd10799063c1f24bb12b24746c2a73bc20f3238061b9a6315ad1c2f9d2aad90f10fdff71dc209179cdb47aa35641b136c905e565b1db2bf2f60" },
                { "be", "8199bef80b0a1eef4d3695f87fdff5a5ce1109f7df1dd20f9818fac8d3abd2ae1b2ef71777255d6c0e8076d196b1e0b66d74e816b166cd9611fc330589daf9ea" },
                { "bg", "16c9591655ec457653f5760cba5527dc037671a522c9d906bb01b9625b885413990b60b43a01afcebe0c5a97b4e3571319b7c60c9b823b4ac93b577eb1821e73" },
                { "bn", "ad36771c940725f5952b5cce287f0009475537612f5e6286bc443859a50ff285f3d869eb0cea2120385c50a524cb57b150d0729bf24014c6f4c366c48c72a2ed" },
                { "br", "8183018387ca2e635b49ba2824424fdc3fb4705a98cb38366ecd3601a132d414a862380c9d01d73630bf6f0b213e6f9ad646cf78f8b119516a267c3abd5dacf5" },
                { "bs", "a811558bcaeb041ebdd163cf5188c4bb512bde484497716b98c7f3f96993683227278c8ad55004dcc3cda50388a6511767fc7e00b668c51505982f72df719bfa" },
                { "ca", "a5f029d5bb9f325ef06239d25845b2964a50ca291eeacaa593a9a72311a563314b201f093786f04949a165bd26e4e03035b684261b5c6b842e0a2d44952ff39d" },
                { "cak", "12c91f91c5cc593695c56d045d5bf3ae573520829054e9f4f1ee869285f41e10d314731807fc381aa2b9b71b5ee623c739399e2fd555dd90a05f702d7dfb6f94" },
                { "cs", "81615ebdf25d319cfda2dc99501952ec2eead42beebca4ce6a3d96d8bdc3ff386fdebabb60796b44eb272dcd0a620aa3568d679d74eae9b95abf874698e61877" },
                { "cy", "197c65cfd87eb0d783123b83acf0365a2888dc4472e9fcf32e4c249f34d6c57ef5d1bb36d10659d490c8c3bf7ebefe7473b5149c6ecb67b60ce9bc142f1bf8fd" },
                { "da", "dd881b7678ef2b1a448c4f0ca84e8eae62be891a40ad1edd9af739842f34937bfe321f05bd8fc6754977b9ec5ea4630c6a70bbd372f1f7219160deb68abb5c58" },
                { "de", "2a20c545dc19fd4e7c6da272396403a73d926dcffa61ba8c473ddc1a15dc1039c50a2f313d99eafca92618d0c59758628a7da65cc3d092544bdaf70870091f60" },
                { "dsb", "8dffc31ca1fad3446fc1e22eeae0647e8122e761cd15d41d83ce2c333a89d4a1bb866988ff50f1ca10213b2f25885d5260a0a045c7a1dae2f30c6bcfaf9a7341" },
                { "el", "22f60d9e27edb9117c95553756969ea2e62950388772e5dc566b4c2af40ce3866a76e67b733186ec37c342042a252d99a80e2c8adc739f170ed339ad188ba540" },
                { "en-CA", "5593d1d6a5b64128dace637fba1de7f3b8bd0571a442f21d07e26c3191d0debb8bce91d69dbd5a761bc7a3d53163f99978f75ce9683ea796bcf385266fc96cce" },
                { "en-GB", "45a7af7d80582e70693de7bb9d9289d6b738274c06919128a1de46932c9540a53498d5af38e0f5cef58859a8313737aed0ae6cc5c5e01efaf31786aa17117ad7" },
                { "en-US", "06304511423c8a2141a427866a32187617b0dec70938e8fda694ca8ff1bd4117f610592595c8eca5a3b44c7af846a01373bec27ec442d3e0c93c28ec1cf14591" },
                { "eo", "08e2c5a9377c6a35a43e908d8783162593d8e9430b6215db15ac94c6d9ce708fc7ba492220154178aabccdc014b2ed51065bc668ce05da6445d47d6785a52cbb" },
                { "es-AR", "e603bc683f4ac1e7c2d76565d6266699b59f5d39350a898deaed071629233651370bf7c585d4d84343b09cfc590320ae27fb215a86ce2cb9a7aeff0e89913b3a" },
                { "es-CL", "8fa8d4b3e83c75565ef6584663f41aaf6b675dc8222f5a1ae8604393dd6332fb2dce8805446c2025508981162bd2ccbf6250b115ea97ce8677d8f63594b21122" },
                { "es-ES", "493feedb82c18506c8310213437011d4a0b540c433e2ffaeea4325f0dcb3842f23cd0341f73bfb952c3c7e085553bd72238018a53053f233b7f661f0052e90aa" },
                { "es-MX", "b8726f8efa0b0d82d415eabfab96094fa2ecf22db541c6a0fc48628ca3b4577d7fce19ad6c9cb9240695d763d383f94996648888dbff039d32fc86e3bb9dc9ef" },
                { "et", "6d166c38a31c3bb2b0fc83edb49881d7a4119f71c04a8b2eaa89f7583c6b213b5a976642c12ced3a2252d2ab91ec3a358cc4d98ab11fdf6ad38dd0df7b34a74c" },
                { "eu", "e1f707064199db4b1832de2d114367f7e5076e331ad849919e0bc3b5928900958ffc8adf0d4f685bd4088ecdfd8bc5fbe5ffb892e2bd599dd27cd4274cc2c7c8" },
                { "fa", "628a3e543a11a4bdb0a4526369e0e8c642b08a1b44fe6f8c52228dba61dff78d43ac8cada28a5b5d9f0c37b81c6d2fcff2f18a30681c5c285cf06a59a49a7587" },
                { "ff", "d5c9b7e32afd479287aaf2d99ef8351ea1129f6a401da83eda7a1b07c10da51b3a634ec213182831cbc10e9dc28970cacd9de6326c3f0b2f0966e3f865033e07" },
                { "fi", "938e6f653b194f24fb06da34c9a0dd08615987f5832ff6dae17ba1a18c2c2fd537f2b8d140b701b0edffe86e9d1460b337fb0de6e508b9578a19738a2addb4c9" },
                { "fr", "0e6305ddf1d973731cdb1c81041fdb1b07caf7c7d04ffba65698cc1dc86f9aea6c72eb98ca2267b57a84687781e277be289df899f780b9a21173c2cb435b6646" },
                { "fur", "bd88db85535eb9d4dc8eb34e19e70fa29af3e6e334f289affe8566319cba3a63cbd4ae8a8b0859528642d7e1e21ab241cebcb40c97304b282b5467247a674cf5" },
                { "fy-NL", "2cef779f1dfe5427595e90a2fe4846098e24fc71ccf8c915297d1ed26318cec62b02d1c3ad3b2005b99f4e39619b5539cd494927dbe3e7158a3218a6ed294c74" },
                { "ga-IE", "c4453f69b7a0a0a0258fa9525cb44ef909616ffebcb9da48514ebb2fb29a3044c3eb20d30172b58093224eb839397a6228c3a5bc045cac093e5c96f04b6851cc" },
                { "gd", "6ae0927b04f5d4ea94b1243b9c6e28c2b4373cfeb55660e322e6fc581a94f8206471a5e18f6386df5cb285291d865129143569927716dbe0f095cf43f1336026" },
                { "gl", "477716a5540868e448424b3d077e1c5ce567756f6bded03a3481736047dce6db241457c007f6dbbba4e43bff4463c36d0030ebd1fd17cbe0ba37741653176639" },
                { "gn", "e2e90a587180ea3f70b0b0f2c23a6c14a35ba8094d5fd6991eae21b90aa919b74b8afe4d4e5a70344d0300b7969033211e829d2596d2d340d2ccc85af0dc3c36" },
                { "gu-IN", "bb0ef51dfa2571345981b8773f7ace26e1e00a5221b37eadf4cf13a9ed39a7cf9f2d22d0e5b2f0dabcc443ef75ab710a13d106126e792c95765d0160e572f412" },
                { "he", "1098a2637ea6991fc1931e6035bfa668244a8db206672e72d2ff307948e19546832ad49b599ad227535998bc3e9d40b93e09fd91b790cc30583be91690acf0ba" },
                { "hi-IN", "82cb6d941a2eb52fd3c66e77d7bb9f90314c616ee3fc45865e7e3328de2a0cc2d15478a4166c46b78e01896759fb115968e6d6ed01b167bd4423050a19dceb2d" },
                { "hr", "8a6dd0dca062fd803113721514fc6b4c9f60d4e76d7413d42b1115cbe313c89af083e2a295675102e2d3b6c20492589c18d15f0d08b0e3ade48d6777c3e30076" },
                { "hsb", "91a0062e0bc00dd831f8521ac0396ace9e67ddc692504fa8d47897972667a43bf11e54fd401d7b92fa10b5d1b1b917673db8b55ec6d84cae0da313aa95d64f1f" },
                { "hu", "16712871e641264f47b2e6e1d63a714191e8da761d02ee20bfa683a967c54661de4d1d4a07f43d18b3df87d5d548ea98c7e7207841afa3d2e7c7b399e4626767" },
                { "hy-AM", "be14aec3530994082a4964725ee09e0cb284ec4ea933bf2c3cc172f99121fbc076428d0435a8d91bbc598137880adf7c76e8677ce0e32ad5cb6503219e34d807" },
                { "ia", "70c9218ec2cd44d20db22d3565e11fa1bfd547708076502226ed4ea92a9b8fb08e228bc823ca359b3c4d76dc989a3963811ae98c917f302afc4067f7d7ac4704" },
                { "id", "04cf3aa9c235915c2ef0774c2cf41394fddb0fdd92713b41e910d9a3d88a826af4dd2619f88907f67d13048f348fcfe3a1475c79834134d0444dd46cec0217d7" },
                { "is", "a846ad7c69d4b6bac9576e41f929c6a12f4f2d5b30eb27a1dd1ccb14691bceec429f328ec4a0e15e17e467f617c157b234582a74d6ac5c0a63394537dd469ba9" },
                { "it", "4165643167dfb172a5d7c885590762ffe01d5ac6f72fedb63a9c3e559bc81dac4f164bd2bc2f1ebfae170fd1fec776c4672a7d9fbe71d7969a4ab9823b726111" },
                { "ja", "2d64bc202c507d047f7588b16c8a11d5cc7a7eb36e6659720c552e0470402fb1e17a8437eee14c16896f37cf8c31e1da485dd7ea97a1e722c48a0988bf10abbf" },
                { "ka", "1a7467db500662f17fe60bf3c6100a7388609cad46d055991917f8c787f25e94de3d43ade32f1dd44f1e41de4b3dad5b855c1fb710ddb7ee8b9445194650d643" },
                { "kab", "9d6502afdcfb2b5df63b671dacfdef99b0786705cf5e2d34d6f29f88ecc60b537bca1db075f9f5eecac761256ec8426f0732f12ac8ca22aa0e6317b9e6fc1feb" },
                { "kk", "19ede04bf3461499045dc3b7284178604a4f7f929bb222014b7aa1f23fd8ebcae8b5eae1e6d2d8381ba146f63bd2e67194355d0f2b1354d422901640a62401f3" },
                { "km", "7c0c622b65591e8bb24ced2303344e58356f6ad4876659c7b1b97a86bdaf8159f931cf94a2b7885c8a4c6b72f1ea9f1a95fc4dc581c950aae63242eaf64fbb66" },
                { "kn", "51a2b480281020ec8830901a835aa357537d495bb2a22afe4ed85417ce135ad4e58c941426d119f23240d107350c9445e7babca5d798b8a82ce4d23754aca8da" },
                { "ko", "484ce1764dd2d28617ea3920c1b63ebc92db8441009c226339aee1635a4591ec54406cd2488676ee7b12566c1b146df585b268715522bac34597156363b92df2" },
                { "lij", "0e3c7248d0025b488970ef0df0408fe5cd42c2993063fbe15123aeef65fcd0a0c3941a3f8d51ce43bebdd657d61659aab7dabd48b47841b3c51750290482bd6b" },
                { "lt", "65ddee80387017dfff8062c10f150f9a558f03c13c3bac9ee70916a9ca6ac2f2425ed4f6bac459b6569b92c395b20dcd89f3300dfcafc3126f17277952d1d3c1" },
                { "lv", "6621bfcd73b7f8bffbc2eb2a0c4201d17b0b6f9f450962f120bf8a4e8b8aced0168430d7f2113403958a67c6c279c1024418d2e5519aa32b9ef6e5ea6c26eca6" },
                { "mk", "e42a83acad4a38bd367618037f872403474e5decf07004eabc20a5c17b09a8072bb972a9c6a8ecb00d8e0232de42303c189856fec75dcc00a6c79261a551afb4" },
                { "mr", "854f012404a7dd0631cef60a43089180d78b2aa031f7a7627e7c75636c342404c107a9cafe4aa84282c285044496750acacfe40321283991f8d391494cbda49a" },
                { "ms", "1cb5e1b5fd29622000530dfa859bb81c7df48a43cfc2c5657e168964694329c920cafe29dfb4bd17d8560d212d7b2cf53417d4521542a68d57ad539abbd6c400" },
                { "my", "63fe037657fe6aaa035c766e1b6b95a6e79d749c81b133eb71b0524938b3ba3f6ca262ca278ced2e6b410ac8d5d1eac9b612a6e1dbd72ac907d7fd963e221597" },
                { "nb-NO", "e80e20da183011340622cc8c77c0cbd9ab83e40d7eecc334a75729b5bcbb2a64c1561ea79272aa5262c7691e1564f120a8acde45421acb8ad39cc73ed58e345d" },
                { "ne-NP", "0eb9070ca3c7741c5854b574a55db2605cf104cbbdc0e6ecb5ed90d613847d02284192b193dda65fe7e5bc5b89153b14f7992f400c31a4a6597eac02434432d8" },
                { "nl", "f532a82aa56ec70946fe924f633c536541d03347efa92c4c5806a0675a11662837d6b16dca2612e58fa20338ab27a6a512cf1e1a1118cee30f98d13f7091880a" },
                { "nn-NO", "7cd6534a78a213f27a3cc8abb721c11c00342b31b4d047d0798e5f54a1ac87840639128f7b0efec2dfb1c8d3c54b599091de4723fb4ee7946576916b7fc9ca93" },
                { "oc", "18c9a0dbafd431904d574eb841d13da77e47049f8081d7b78256a9b3512f41f10a9a58f72e61bebaf05ef0af9fd6a962632c6ff04473d4ce8ebe1fa32d3f5b4f" },
                { "pa-IN", "953702b9534de50def3adf52ea67f7064580fdada3dde78d8a15c8a608eba80635c659525f942b4dfbb3d25600b2deaf7edee2f50ef3ef4837fbdaee1b671f57" },
                { "pl", "62d7e47b7745397e06a867161d8b72af2ed70ac0d961f8c5edc0961af0674449863d3cafe84308767b4aa12d685810c01f95ede04b0fca2b835c60dea9ad59a0" },
                { "pt-BR", "08adb6103969d4b3cc662c80f8cb49d7d0cb058ad79a8d4eb0f78de706b81fc647711b50f37f616b0fbade6503739c272f59ec1d0d881535aaee18d374a83b82" },
                { "pt-PT", "66097eac64d4c995989bf339a432d884d39a38e6ccddbc39f30152a4c0bb6ac478be453070f4f1b0df1db6405af9bfa03caaa3022f24874a8648143920342bad" },
                { "rm", "c3cb1638ce6f858eeb0aec61ee90241dab1409c436883e8bfea55be9367cb9c272b105077e95b30a744ca7f42dc4dfc2ada6c914008448ed9fffd1d535de8030" },
                { "ro", "1bf3d1310c417bb65fe463132370ac510d2142c44d2aaae8ac8c63deb71f5c9debbb4c4437a43e763a51d375792cb6e078ca72c508c51c2fe7975938da1be580" },
                { "ru", "149c72b9777aec8f8cfcf713bbfd14c373ab3fb52b60d3ade2bc74256d39228d20bd380879147c7353fc04835bd4914e320fba975692b118b2f4fe348055dbb2" },
                { "sat", "529c0e24b988b5b9205d010222b11d77ecafe8af1174f05f652de8f50d58ac7feddcdd571c72e9cdb9327dacda224c8a2e7fa9ca2ce61b3f63d42bfdea98af1a" },
                { "sc", "a14510d0f3821fdad92e90d4d2b2a1857e9144d364c9bb1830602bd6d789919e389113a8e838ab2694e9906f9576eff10a561f3583b1575651a5de8f70444eea" },
                { "sco", "b3f7bcaa45fe8829196b906facc5d2dbd108dbb5399071fee759724a8bfacd9172cac0a1e2f37b7dc033895ab8aff8ac4e6dfe490ae15b01fe99725ee811f78e" },
                { "si", "f46b72a56b9e34db2451fd06c220343901824ab6840d585664bf93ffcf3fae6913cd95eb0705f48aaba507aa2f83ec77b27bca5593ba959443e74077b2eb7fac" },
                { "sk", "b5e86e5a795f85e380c975fc5265036d054145824be6e19bd41dbd2cc373fb6fb3ed15a6ec8c80cae1d9305dcab9b14b0422dea642143870c4924a8cd1e6c336" },
                { "sl", "4d8e53e8e8f574e21853a79c4cde9ca1c28aba9aee5514f0d7897104037c73cd58e7927081fca4b9de57ab0aea7fbcd6b2860d0736be47f4c9afbdf7f9eda08c" },
                { "son", "7fd1bf24fbef6544f3e927a2708a17bc29c6f6190dc801aea937161d4ab510660a44de5ab427984533b51ce840a4f8f48b9684b07573f2250ffef471467529ab" },
                { "sq", "e44b60bed6cdaf784edb2f5b9bf8fb101ec13962bec93f0e45873b9539e145f27ef82569d60c0f301fe27e7f716f4ec078678c8e2b5facedc51259147cc24147" },
                { "sr", "5c776186963f50e9892e4551f0151341e2bd1ee3f00b729bee2de9387665e2467e338fada0f482dabaed0b7d193ee4335f0827b6d579a3ef0c1d462eecfb2474" },
                { "sv-SE", "f9083c9e30867dd3ab40eb385419d292a5bed45d71555faeda913ad2bc6092bddd2bd4f1b7658410b0690d638166d72113db26b4f74ecc25476c8f840ca09747" },
                { "szl", "7073363116e3cc1e15c13614450b09e47dd939408d3fd973d3178d98d87b637297edb03cda94c5f3a292b55a4a5933c82196e7d58d5214ead4e98f72b6853e4a" },
                { "ta", "95420eff7fb5a17816341b5a36c929f80a1aea51beab255dd74ada6d9d71c51ef81c4866fbf6cc3e41636a1eba0b559546f550aa5fb988e63cbf8877a6a1b04d" },
                { "te", "050a8a32b02a76fa5b3fead2de09f4c9447b533a0f6d9e7c6cd61fb1d20165d8d7bcde89df5ad7571a4de9c10d0bfbba983f07a9580a8b43c8143617030d92af" },
                { "tg", "fbba851197cc26e73b3c5f0274379da61faea9ec5ac3036f9bdc336c6374967fca5803e06b879f257b3cac74293c265e71cc820d5402a3ee7d76fee63ce3d7c4" },
                { "th", "055fcd02aeafc0935e16b477e701c517fc8fcad553b2fbbaa0002bc88080ba3564d4d616b193a0e5f938daba0958966e3786e98088259c877ed0e19f6e70dda3" },
                { "tl", "5b5e28320c10d200254f2dc345c9223c2025f008280bae2506f253e164bc10750e145f25c400a85960b84901459ec72bb554f06aceebb9a769b75586558aca01" },
                { "tr", "975d3a9204c33a954f838739409504098e978ccdf22517bf72b04636ec6739a894f4197aa8d0d51bedbb4d699c6edda722370cbb9ee4d5a1dabb69d6955afdc7" },
                { "trs", "7cb5fa35e77dcd3d506d5ccbcef193d313cabeaaebc5f89091a433aeb569e1ac1455bc77bb8c8e1d2d53ea9f7bd185497b2f15351693238bad501701c7538884" },
                { "uk", "96d96252af7d898ddcc2fefe94517fc1c5dd0e54024be1409c51b05f8f0837e66600c6ed44846b3fb24b7600d451e5a542aba17bc744059c1f457d4ba1c1c8f3" },
                { "ur", "dbc20da2513fa446eb389684ab9eb9f07e34ccc698c8282ae1e748572680bad7980f3ff47bfd397e72f8bb2bb0c632c4872c9866e0233bebd21708894de13ebb" },
                { "uz", "303e7de246e6ba38cc6e95d8a532f21d6964282010f7cf33034eb3fbe731b3e274f8587dc41d213da99b67d048007d0fc39271be184248c7d60c7fbaec53ea6b" },
                { "vi", "15391039dff4e6ac80c52b326e5b7ea20ef2bef3882476b42bc78fb29c13800e30ef2e460ffe62b4793aa32dbc49b625607847f18c3651445d743a49a5130de5" },
                { "xh", "dee1c77655ecb16bfaf0e72cbcc4904d476fe7c1cee332735bb2e4edf289e020480af960996339132785a456bbacf09224e44d7bf1c6a7017722b2d820876b69" },
                { "zh-CN", "72b987b4eb3ed1c71277670ee01befaf1523cb7a66e26705adf9e5459c3fc3619829259f9ecf0af56973ba50b803793c7fe5e21dbd7ff68c850cf8877dc47bce" },
                { "zh-TW", "d399c37a7af8cf088d965556e7914b3e8ed1db1ffb42811b2a7e565a58b6580a806a6088692bf2952bfca9cf29ffdbc841ee7cd8a52ed9d12c54139e61f7a6ee" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/123.0b5/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "2e9b79bc0d86dc51620c57439434d9e10994cdb38632b6ed84cbd4187eea986162e6c28af484b7d836df721b423fca87c18ea061ed130caf8c3ad7ac90f77d8e" },
                { "af", "9dee38ce12bf9a1ef49b0680ad61abd2e3e2a18050a3b2e772709bf8c8797009b4479e5fc59c9a495c32f5811d0eca074c46d77a5771c6c518ac60823ac809d6" },
                { "an", "f5943070c52ac08949a731bb2f58055bc4d1f64b46c7da2000c910dd90782c9db6ddb2732cbdd62c3d3ee1ece58e8d3a38cb5bebad17b14b4acabc0061c1ae5b" },
                { "ar", "d0b03614ce95ea9a5078bfcc4eb8dccee58b3027fcdfde6588998298fb9bddb347b9f53a7ffcc6cc9fb814b15097d6b4ee283b654d1c25deba70eab664f4a361" },
                { "ast", "19fe4aae511f5d2f37f53b377312edcaf75c8916ef6bcd44334ec5ce2aa0a6c37e27605b35751d183616b589ff8e2c8190a9c911e04bff6ffc79ba53511dfb2f" },
                { "az", "8e962b1c21c961742d9e9b9dfaaee163969b64d5ed7f19866fb5e4098b1665c7492012d647152236c01394d217e18150dff74a07f986162e894918b7fe869e61" },
                { "be", "73665135839dd45b1b6bfc2673043f41a1dae812a03c5b96769e33eec928b258cd76bc2fd213e72d3b78f9af6e016484a2e7149b1dd7e35a36764f3fb4789f23" },
                { "bg", "443ffe3f7773ea457f1bbcf59f5a8ca31f97e62f7656d6813ec7fc6bb5d632977303bf58d889941c3d198c8c7d8eddf593b51cbce8d3415449c334fca7f5210c" },
                { "bn", "d53695b50dd3cf0f3bda28a0c1a7aa72538acb089640f58a92090a0517014cafb67bb10daffb4a1fdf1390495f77409f6fbfcb2c6f6ab8b0e78a59699bf07fe4" },
                { "br", "b2761d9d74ffc569c2ff17b72ec5b3df2af2d77167acbe1cb5ab322b34318eb9703e738cb819491ddf892c72edb69e78519603b57dc03689b322978468c9ac8c" },
                { "bs", "9f04849813d15f55e629ec22a84ca405edac95de1311a54aa8ae8e89bbd0d79b2968254405a792948008bf89cb9fcfc1331d2bb7abcb3fbafa9ce3bfaecda739" },
                { "ca", "852e2db964d71f188fe216f9c84812989fec2ae1873324a10d352d3aab6148fd16fad239056ff15d998e2356d8a59bcdd0fcb51bab4ab2e3799eb897a6b7998c" },
                { "cak", "5d5520d09ce08bddc82eda507501f9f607e12ac26fe864b3fada22dcb7430305f33eb23b3a0a611e9d328504e11bf1c37a6cf5270ea59b49e4668906e3af84d2" },
                { "cs", "580b3e02285837ebdb33cfddfba22ef8b9227c83323b1123b41afb37b8781b27ef4c08b85ba046862bf7a56d17291e5cdbc8b1e6ac590d48baeb05b484cf5e69" },
                { "cy", "a72ac471a78594424120eaebacaaf43aa5024e75e2a25f28c540ffff7e6e8d015645f0da03a6101c217aad95df1e462d56e6edbf22935bca4d843178968d8948" },
                { "da", "fb12bfc396e2bec01b32650cece39dcc988f249176958b8e2bf8e9a92ea1c305088406e96c30c88264c01fb2f286042cf52444106abc7ac04bc8fed8dac31bce" },
                { "de", "8f971f46812c0f15a8c53ea416e0abb3d70240760873f783854491655b272a4e9fe154eb53100394afd721002e860d584cf5757538d6bd8a924d4dcb9b66406d" },
                { "dsb", "2ff656e02df1d10d4c226e73c44a8f0e96d17a519434d33ec2cf6253596808497e2f6c9dd33a22406dbde98d791072012cd9928a7475b753e2767ded778cccdb" },
                { "el", "af0bfa74383bd853a5a253deb7a88911c23c7f0f1bbb30d540d3813478affc8d97bfe8ad0772acf83fdc4f3551cf85d147ed80ac46ace767bfe1ed9f35154ecf" },
                { "en-CA", "b2d1879b55fbfe48966f2736812f4fcb9d919246ead69ff7aa02fe726de2c80655ed316b32998233ac6471cbb20003541187d2759a0d4e74fef488ac0e5f8608" },
                { "en-GB", "e0112b8c1766b6219e5f28b34f46e5a20ae4d67487389efc10ccf9b1e14f1027be59a3daf6171ff4e99e15de3e2624c1b634c914a757b0117a0294f18250067e" },
                { "en-US", "4dde4ea43c5c529a2ae3363822170541f67de5ed0e7b5b0890fcfbe68833f0a287aa7c8bbb103fff901a5888ba20a9ab5922ca4da3810899bde38455a8c65e3b" },
                { "eo", "8ceee6b9705aed53cb30388c0e5bd951a2156090e128df5fd2af1ccdfe845114fbb5f26ec6efb3cf06712cc11891f853f90bc38d60abb99fe881fadbae44e715" },
                { "es-AR", "52b953f32bcaa95118d72f302e1f33d1ec6c7d4b66dccd293af86d8a82b3e8ff384df2e11660d13e611961c8cffb109aa0ae056e1bf64d03db22b1ff6b072d7e" },
                { "es-CL", "58f05edc5fdef843436e193a1477d69d92a7ac09b36c198ababb4b492a579d90fe178258f57357fa07dbc6a33669bbad83ff661b77d925e147f5a65b7632852a" },
                { "es-ES", "bab3d07558a13d8792141b75313e29b73205e592f12e7ad1c67890a28295dea2afe9020cfb47426db1656832e9293b1f68ef4a654efff771222f1b6f88fa2d96" },
                { "es-MX", "624919592bd68682b3658a19060a330846ae3588eded884f262de8d5c1a74dd220efd7b8a04c3d18eb6b8465e6307b003b591d6990b0b3a648e2d9f75aa56daf" },
                { "et", "89d6ef6d5bc490e830d2472cfc207db4f5d64a1a696e5248eaeef725da9b61d669812e23c19a8dd2e873b3896c8d3520bd77ee655d6a430c2f73e1fde87163d9" },
                { "eu", "71e5a07627e4bb89b26b284986d6afddc518f7eb144fbd97322dbe02a439b5cde9439be6efbc064e62170aa41d43d6bda1b0cbfba81985ee6d5e1884093bab14" },
                { "fa", "9984a1d10e125a12b18ff7f166c343da0225a7257e842c3ea2e69e3b5463bfb078278288b4f3e7a5cb426b6d8b1da901dcefc73116018e2e1c29d34029521332" },
                { "ff", "df1927d820f094f343e7280327e6af9be5efddcfa7c45d0242b5070185df3f48983801678a94da6bee6424d6f54270e243f30ab31f2d3036b193efa0326d3bb4" },
                { "fi", "fc8ad62c9296702fd7af6798524c70c43fc35a721985fe6936f900e514073ea923df36d52a2a2fc2543437e2c33165fa902e6ecd8dfef648764fed0acb93e3cf" },
                { "fr", "75d867897948d7469d3d37c112dc4c9fc0db7ec0780cf469454ccc8dda7e11ffdf9c64badd5633f38df23e400a9a5cdfb18b84b5cf5b94a04f3c40c1f0b9270c" },
                { "fur", "a9587d3190dde2af46986b83444440df9162c5c5180e67286a92132c6ee7bce10284bbca97b55ae49df5b9f9d99bef8892239131625b2b7cba462e18b0c7d424" },
                { "fy-NL", "147538234ef669fbb9ff4b60a8ef81e799df2d6876e4924782327bcd9d47676006b484aca65320a0f162895b18d0f4d4410a8da974d737fc6087b9d4721076de" },
                { "ga-IE", "272feebad471b82d12a1de519984878d1824541748e2a5fcd361d64864f66e89fa5a6b2ac16835bbb2cad11c1fb7105bad3998d834f98355da6318f9ff2e9f66" },
                { "gd", "1432b8e2a8e1181dde408bbbd7c2ed99d2200e56249ceb47b82bf2ff77794d06420ce72f4cdbff5487f04a6d82d4543b33e4ac85a4591dfa69e7b1d6386b4eda" },
                { "gl", "880438ed884d28afdfc23641bff62c784254f2738e68da247e48e3a263ff497b742a09cefac0d53aee57ff5145ce8f84fcfa6fe1e2d552541fd344dde9be6351" },
                { "gn", "e661d546b6c3632da33a11b825ab52e2e18f7863e68bb757c92f13506aa7f631021fd7aae0e9d522a9c42001176892b3c11d5bab154f200c53541d4b0264265e" },
                { "gu-IN", "b22bc674ba13c519e64313bf3055395113f74f36358f3aa32662fd24fcaea709527cfab244cd31d472c25b79e51bfad5c2204861c07959b8be9bd9a6f2a0407c" },
                { "he", "52fc29fe34e4df454711dcfea69fd5474623b4505f4902993ccf3e53ff320aecc91373b0bc0eba6ac5adad0fb17f6de50aac35970f8e0dae0a3a5b3250e70d6a" },
                { "hi-IN", "4e63483e7ffda4f40590d0b30972d245ea4136e281334ec74c295fa3c103e6200ad809549ce49a831733676c957a0ae645b3645c0bd5bbfddec44499298be33e" },
                { "hr", "889cdc2e00d31d25031df82e4ac72399094e81203507255759d971d43a6f5bf8f796349c7523748c152df1b00958ad044ef10e868a32f1f0ddf6ad8d50327218" },
                { "hsb", "88029636807b1edf33dfe4cab9e0421f9dd681a82b2a7cce6c250b1f4cb6fcdf02c2b4215b3741e3c7ae603041dcd199b64c2a66c386ec443247fe55d3b47082" },
                { "hu", "e0fe9ace6c2872e039e351cb4dd2e1099859b1d8e80f1af89400773c8472ebd97d37806356cf872ce34824352e611549ff363c707bb4976d00851a5b56b310b8" },
                { "hy-AM", "f851a81058bc977517c3ac3a4cab1c7d749b29920bb9ad9ec2a54fe7e56de5449f33a8334723bcbafaa1d7fb39e555f4ab14a394ec4a82a44ba09a4be0b9ec7e" },
                { "ia", "19938e2ff7e0dc1d91ddff95119b996820ff229d1b8383a266d8b106b616ff034bb497ef14bebf72f07faf70f511b7fe03b758aab05c2befb2c7d9b4842aabb6" },
                { "id", "e49346c48cc428dc12691ede50faf5ffc7c56fe79fd60e449774fc8e8a49309b0115149cc2de18b4fe2429f16159c230eae8a45968e101347b50536eb11d0792" },
                { "is", "4715124f16b8a89b4b529acce542aae39f5d8d46193d25ee6568875500046c44ee9632d07d1fe4c373cc47ba0da97b5615a7a6c73a59e246587a755d0c515b51" },
                { "it", "7e3b0cdf0543e7c67ddaa2f33175704bee250773a63892b74956f254cc75a90cd6ad7a76f08d953bd95047e6ca374f7fe33613cc8989c009ad2f9b533e1e011e" },
                { "ja", "db62047ee94285ae08c889ae768bae05b066c100ab511940ab00edbc689b8721541d85473e1ed0bdc19be8c37aff97e76934b3ed395651af01c2388021e54ebb" },
                { "ka", "ce23fe978d2fbb3af64626d1539111f325d1bb7d6fe7e72d49ae48fd215785e52e1e4994726b7545c4d1a12d3cde172e99ece5dd810dd8afead6d5714aa81b15" },
                { "kab", "647e49e902b56f22be914403b96f16e9f88c55eb526d9513c1f42f79ee07732dfd00555a7361baca0ec5a093f11062002bdaad30fabe390db68305e2a09332ac" },
                { "kk", "9426360c5978a3f58ffb5eafd9693398f891df3386e0aa3b367aef073392d20c9ce4c763903d31c9a773d496ce5133b9e020ebc2b705785c0750b470453aa52c" },
                { "km", "d8f5d960dde81ac6a55f10d0a1196178a98c92be9259b3385a706ff5f017d4c3c9915fbc2b4bf2f2d66f27c64bb2c5849a8cab3f046f1debd3099176f492f931" },
                { "kn", "7382e5f4ee73431a8dd0ce4fab5b13231591dd81a44de4fb255c44faff012ecd1af2481409d17ec554409ca4ba4d443e8f3328365609ede9c9756e43ca9b2644" },
                { "ko", "cfd60f48d44abcadfe42d6d3b38f64eb17791a68bf2c3862b8151b49b97c0712ce143f54bdaa1c6e407c1f6bbd9a06318b76a7eed81ca177fee46a38559274f0" },
                { "lij", "121130720b654be6671bb9061fe68368b15651dbc78af09264970d6897f9642f1d87ad441b8c89e3878251b7fa957f3cd395d445234240ba51e675ac5070d419" },
                { "lt", "b5fbaece8e2d99af2ec286eb300e9c3d49a0e95b3e8ff5d1b8f6d9e468117cad08395d1159acb800d12c1280e45be44b77e318e3e76aacc8aeb0ccff02327171" },
                { "lv", "a35d64db0d29a88a5f06a5f788c1125d4b12a6bb17667bea0517a66f41de22c0181c8b78da280545bbc35e71a78216e3f0fe73a60bcd931eb0a30c0c4337f3e6" },
                { "mk", "069b0d128c4c72d2e5a6d43e3d1c9e17a929e9c1949cb6201281435927731eff2da85e740ce32bfa17184363e2da25168b1798cd674bb7dbd5a308bbfb07d402" },
                { "mr", "f1376fb312a6e232b25dad7f891eade4a423ad6b30f1a879311dae85671fbd0287b52b114761b94ac22e3f823e26af6762ce531cc24a3357f69ff4f023bb50f2" },
                { "ms", "33f3885b943d13bddaf31cb212da8d67619c0c566400a9f097d37acae069ac223b39ec143c45c1c745ece502d46b780a606321d591f008708fbd3f8f0c32843d" },
                { "my", "b1483cbd7c69d2284691568671b5f32977d0b1eb607b18180bf41f7f686323f09aee2ca607daa088373b68c3cad6fd0ded936f50160229aa0d189e804b092bf5" },
                { "nb-NO", "98de0aa79150dafc7b0991dffaaa27b99a76386f5ec3f7110e8a642ac1a08f3fb83d40938c0a11a60483f46d7e82e3627bc96ab3b2a9c40821d423f809a2e702" },
                { "ne-NP", "1e8cdfb6101aaffd5b6ef5e6038adff0298179337d4f8851e1d0c9869a885efafec4d60ab7e29e3d814f49498a9758731afe7d0e446fccc9f250490adccbe69d" },
                { "nl", "f8dcd4ca3f256d475ec656316cf33f6297eab5eb1051ac6598986db50f687101000c4497b1342c052fc19e28bdd44ae91b36eaf3739feead8e8e56dc0308ebf8" },
                { "nn-NO", "6ee311960ed3e7dcee252c719ae985fe52274b66954dea5e9a8cc7a4b5b734a4f5e979e4c6e311d383add0e1be4ec00dfdbbe8c85eb27e41956e366988fee8f1" },
                { "oc", "07134abf3af9920cb880382babd04418e60223eafebd32e995ff46d05910353a0c582cc49598899916e9207f8705b1141be886b3ca9fe225f7980efd358e1fd6" },
                { "pa-IN", "c42b24979b8d4e6381fe9608c7c2a3b71f8bd97d0118413e47cf621ada72d24d37326b7aa5ed552abf45cf00e661f00ab2d26e9fe992e5e4a95895a7171df849" },
                { "pl", "d93cafc1dd73ee1f5aefb9d6dc9f7da2e0bdd4ddf98deb68e353e1cb961f1c64af57702032af95931537e25f1766c0ca4c9c7e5ecd68cca53ebe067f0e3b8a6b" },
                { "pt-BR", "b23c9ff119ff52bc1661bcdacd93fbd5230bd5e8b152d157dc0fe2282ccb10fb4fb36abce94979058dec0e5a144660a2735ad331852fbbaaab02b23cbcf916be" },
                { "pt-PT", "71e5af23719934e1275ace10e0af44cd5b7926c905fee69072655826f6be02f9e9346883e5a49b27ad403c60d245a2676c3c7fad879ab101ed0d8d594bf9625a" },
                { "rm", "0223adbf1a7349729bde156b6a6cb4935cab19a76cf572a06038bf1505048e9c2bc4815b2c4ab11cb74716e6b1e42a0942f82622df0b428178b77664f09c0fad" },
                { "ro", "61bfc3c031dc1df808e7eae0dfd3173e6b69cd023058ae53a42d2b0ae97eb595d1c9a9021228ad0064b4124802155749bb3162895ab0b40169e5747257f1f13c" },
                { "ru", "793691192865d46fb432382712cc2ff1e68a816911e4fc24f6857b120e7604cdac472d6ca5711e672de315f133b72774bb55c30b10a4f12a44cc16f568f64874" },
                { "sat", "03af4d62afbdae0b1f7f14929e934f2c07960d4a822b92939226e955d8edc40a41a2cbb45ae0e3a5b2ad1236a1a74c9cb935b09d73d6ebe277d6575d69884fdd" },
                { "sc", "c13bd219c90a27e7207d6d84aed30b259795ddf13c2d1263981ac04a6d141c6d78811691f9640bb5ee1921797882b889c40d1e394f1e1a83bae2e2878070add8" },
                { "sco", "5ae9f0bf731e1802c2f628022987711994893bac9bcf42b2fc9102c2595ab13f618072d612f1c7d138eff15a6a6e6acbe7108760b9aad210f7625c4a798360ad" },
                { "si", "d0e76a48f01d10f4390187d6f25ee46907ef800113825bdd2657e52100296e664ef5d28e60366747ee6da2358fe35831291a6b803711de9c159e7c822114b8bd" },
                { "sk", "cfe010990a8b23e5e4a626f3f43fda54f1f28e7372968e01d1e7472669fad4b82fc97345d728c156f36dc3ad7f51d5c4a1e264a103ec5c5967bede2141ba4406" },
                { "sl", "64f911789763afb45e7ef6a3f71f6b07bb01ef0293bf7cc6b1201ec9ca85eafc8920ecf0cb0a0951dbb5bdece352a2bc6c1878d55e7e5eb3df1a68c48f91d910" },
                { "son", "5842e43dc7bdd6540e86a477619a34a80819ede50c2b4803c8925b10042d6d2ff1105b7d5fb7090de4f95ca7e8d7c39c4f38e51daf5175bf3e1545bf7e7e9798" },
                { "sq", "0893f1f29dfdb454df5b2f66086453037cc9ab757df352f41a56188ee8145d099de0962c5188b71637374278ab7f0036d2aa8461394a700ba718b38fda09daa7" },
                { "sr", "3a0188ed40a65e41da787b132c31242964af69deaae2c22244164e7f1093a21570bc93895ba073ce45f1466c35f3f0821ff39eed272bd5ebbea4c979519affd3" },
                { "sv-SE", "ef20ba6d59e95f99c02b6e28cbec8ad497e17087e9d67d5e8e8389d86fc4528294aec0b8cba6e8210e6c03b09204385fd7cab715bfcfc6135858764d86707039" },
                { "szl", "0c6a44268a69a594fed196f4eaf4136bd96bd07bea294706000d3efaed5d19d3344d39e6a7a7b15d1074aed73dbce700c51e2b00c10987d65442e75ecf626a96" },
                { "ta", "c10b0c6b606c90fb72c4cd38a8cf84c34bd302ce66822bfa58f504cbad34eb889b310af922a55d7dc8315082e8a7a08aa652b5d59b905b1b05165c68b1dfe076" },
                { "te", "7c7c271c6e1ee901c783552da3b57c219e6204607a167f98a33335592f26897c099d6c7f770600ef0c881722823eecd16d142277e5e0ff4f17f94ea0bb672c3a" },
                { "tg", "3ce4a1ce57cb01710b3bdeeba840f09679d998dc4fd9597586af562c934481ef9dee8e91ab6256f3ed20ed9585c01ea9be84f09b921fe4080793061755051b77" },
                { "th", "6d633717dd8f1aeb111dad97eafd9791bef1ba2a339cc18fd298aacc3f99d70789e6332c909ab0552e05d8a8b2f6bc748331fb56b54e699767ef05f9f6fe8c0f" },
                { "tl", "8e6460b7b424f41c1a40c8bef795e9e88daa3ab6f0547ccab30f34f6cc765b21d45491d3b3ea6c6a708b8bc4feb97c56e031970aae46f19971b6d468e087432d" },
                { "tr", "478c72dc6f0fe97934c6a76467d8aafc583469116b04df37548c1bb193d076372823c35509b30a8b44724d8ebc100a46f0d5a3046db827b5d968fcb673c4e0bb" },
                { "trs", "b79825211eec9c5d039ccc7f9090d8b7b21e5a00f019e046ea2d75d73f0e68e46daba7362796387c143f701a33af60545760e26dd7e8e9a414b4533f12e2446c" },
                { "uk", "9cf46703be986696507d2c6d73adb56d663cfaed7bbf712292bfe99ffc9e9f6ed39379d4be2b19d29627f2964448bfdba8c5d4eb3414a8ec119e91e2d800bffc" },
                { "ur", "793f8af2e98a7930a056e509636b306dc78c8ef5a1e101f9a16180ff9f57d05750103acc728e7307a9d36ce3b6f01403f53693ad766150e403adfed87a9fbdb6" },
                { "uz", "711383f23715135879223d09efe8d3cb33f1fdcc837f11c47ffca041e5bd27c0ed5797954ce9fde55cbc6276149d9b767258ccb0476829b5640d200ae8209f31" },
                { "vi", "eb30776317ff824e9aca4874b362188e7cbb816d901a85fde060c712f4655671e192db1b0e922c1a621d6c85ea7e49569659c6bddf5182cef63f279f77c29008" },
                { "xh", "b2a45fba7bdd381272e28e79a3175a6afa2de81ea4d101d801e01029238fdeb757b6fbda7a287d005599956903684556c15566f8f464b6e5096e212c670c657b" },
                { "zh-CN", "0605c37afcb88e7335bd28c524d1b1dcc1cf196693a1cc0bf77b1a29eecac73cb8487c375a22e659746f65da6e7b79cb85eddd18210adbb4666292102e478308" },
                { "zh-TW", "2a0e6e4928075ae259acfd1e1261af3373d77238ae60ff06c0492a2931c41f3686859a55a83cd68dd5ae5e4ea0ea016b9c858c49e0628d3b3801a2d14d266984" }
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
