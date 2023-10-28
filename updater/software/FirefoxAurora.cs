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
        private const string currentVersion = "120.0b3";

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
            // https://ftp.mozilla.org/pub/devedition/releases/120.0b3/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "4bcb3c0980772f82a5b346bef88618a91f19b25a020988f5d1fd7b718b62444e850e109fcc3bf8f3b4c03cf4d90df22d3aeb166264fe016e2265294c4921e07c" },
                { "af", "1240f7bb34b27f4eeeec30f3830b3f3aada9c4c80e35fbcda1ac6336c3f4252e5fd1bd1524f197b79623a8e0a587923b42dea72e972ccced2c6a802dead60804" },
                { "an", "14c50dc5aa7c6ccb44cb4329780009f751704293949835c9daddae0262927a299e1d2aefc8aed45345454058c41db31fd3e011d75da9e032a02bd0235dcfa519" },
                { "ar", "7188b21c16157c4d97d67d60cdec00970834aee26fd525dfdbd56a27fdef40945afed9c8747e82c51f45b2e2e0e62e887b3f5203021f5ef2e427d0ed423dec7c" },
                { "ast", "528733d98b06095969b537b542ed60d72d0d56d29f9265da71e4ebda0ad715b855803414c94a6fe76fc21ed0d5fc2276b4063a46fb6fbe7b41f1b443f5a90562" },
                { "az", "a0acb9bdd03247eb7097b58020e9e2b899c22deb9ddbe1a404427208f1769b676132386e37bc26ff3e6a99c336bd1f4d385ada5595691c1f0a59a47c9e648821" },
                { "be", "ab15c7c02f16d5df82563b7d49bca6bbec1852ca1c4aad38c9a4821e474ca45aa36cc497ddd98e4b6b6e343acba8874fed86d2c1ee77f2818715ad324dd5e724" },
                { "bg", "89df8b76b4a28f436d23ec88e2861e4361cde81751e1805f00566a5f3d6db1f9ebec56a72efde30d15997ba829f6eb710c6707105ecd271582698c016e56ae5d" },
                { "bn", "ad6261ab51f796448bf2f43bfcd64c41100af4b03bfc8e67dcbc27a306387b1e8c1878c9b2e409cb2a0ad83f2cd953083ba4bd5ffb057d7c55cd43ac38547c6f" },
                { "br", "6431bed68768afe0e8ae34c146b3009aeb4f2de3b2773c6426036892a3d1ad873009d2d55edd74ce0ed346fe157ddc535c8dbe6a302eb4da6ed042207c64f4cb" },
                { "bs", "582f02c35161712cebba2dc7b7fe1e5dee5ddcacbc6792c708d46fe2ee14561b122460e193bc8e7aa322603bdf86a606e39a6e3b0187b08c59eb9bf9f4bde321" },
                { "ca", "97c0d263914d34999a123ea42c771c6a4ff2736d09337f022114caab1a51c7963f6193499420c675c094d215248290118460f2692e296ff484a2c87f813a4004" },
                { "cak", "dfd9ce57deeac6b8086c3871bbc204ad241c1f60451b93616aee5bccf6248d76b2efe41e3722117cca9ce6ecf6811eb214cc82262c49108b9d101a2de5a98b1e" },
                { "cs", "8ffc15562b112246debdb6384cbbccf437f87212d27e310fb70a28d11ef65dedce79ae9f48cf6358c54e24c5cb55e8a5ea86a335490b9c570e01098c468aeddc" },
                { "cy", "3f1428f6dc410b7568ab61ade4638774e226a801e229c36b9aa4a1305bd90965cd8c73dbf4b92da063251c88b18bafb71a122f71b15f13a5759ad73fd3035437" },
                { "da", "ea2ef9f84b152cb9a5b122bd61f24846fd195d52f85ca99bc65376e4e9e0885163e27b1a4aec217aa5ebafa3750f57709481e42788e50505c5c1f5e9763ff9c6" },
                { "de", "c246f7848ba52ebf732f244872f9d04d7e5619b50972c4679a1138937307aabcdc260c2d347f3a4bd5889c070d8ed1dfd2da0494a0bbc033b131fa2661f7359a" },
                { "dsb", "7e8c2ff6bca07bfb5602358ae8c63092d83c5c1a6d7158e2ea118eba13cc143443f8025f17e1b44f88b78d1982ee41b7d736542091e3e332eef0df28e049332e" },
                { "el", "09f2e8df828fb3c9d9e1eb9627249623222ab8c4fd01e67a41fd5dbe87c5a4768364fc92139dd26e499e534ba5e5af2ca9767acdc936deb1949347284e0f0f8d" },
                { "en-CA", "866a7833409c292f3999a2a4fd984263530728d8725530bde242be9e092e0fc62440e44592efaa1ef00b20cacc35fc948b87532e1e0ea71f209d3253d4f8e9b3" },
                { "en-GB", "9ee0f2cd9d5f9b808897b3d904641ff33a47e98f6e6761100db249da4aa1826625ce4375c5e7d1708c2e7f300835f9d20a6f6847d562ad80c67520d8dd150c99" },
                { "en-US", "ad67505db4cc2f86e8f880ba93f306aabc07a0cc482fa057c8738de19e6baf714d6ad2d8fd24274c0923f8f434097cb625782614d29233ca89ced7c218155073" },
                { "eo", "2d6ac66830394d0bf9eb68d622f065982edbf2aa1f308d6d9a2a6b8dfca64faeed9282481557ed2eb50fd2814bd6ffb20819586cd3349a0e2a52e81ded23d7e1" },
                { "es-AR", "c6c34ac56483fb88e2d12bc42f4839b9811998b17fe927ffe6365c68b0052a152d274d62e953ec91fa3db54215f48d9fc4b6b9c416948eddc14628d01c638a2f" },
                { "es-CL", "af93132e4d1e7e3741f14695a91a2daf04e3b590c0b4593ef3b00f70b850873b7fb0980cbe5b79ff101fa0b152bc6e0ce18077550bef00396675d3ba7dc6f55d" },
                { "es-ES", "db70e3a70a95df83f9ec8a2e29856819c5e9ed06923faabcdf74eb74fc43c92b37d9186cced3606cd8aa0daea5d271133a59b8ed71fc9001ca2d831bedeb885e" },
                { "es-MX", "b7637eabba410d441bcb2c60993ded13743a39a6e65584150ff271b64da4d9e094aab07482120ce2af3299e0266174dbeedc6e464346825a098b45444d755b70" },
                { "et", "3c76999cd2487ff8b98f11767ddd8ca881badd9016d9a77aed934ffd220601341173381b8bf7ddcaad5c13bae8de3ef2d40f57bb6382f1619a53a377141ea64c" },
                { "eu", "d3c34467afcf87d22a90efb0a5877adaaa062853acf8456b80863e282baf2a691c0e72f595fe7a57512fddd050cf16e5b9d0d0f83f4bdb8032c33928be88b71a" },
                { "fa", "b3bb50454075d297696fbb7d12650172ff98584e0cfaaebbc62045c7ec140e6ad86150d86ad5048f1103bacd9020a5b9f25184ed2db55d61eec0d6454254ecb0" },
                { "ff", "820f1e5bb70704c23cd7b586fdcba30fa3af562be9758516a70e401f977c0d88d36602492dde08c37c089e63a1b80daaa2168586eccb03206192d605c4003d04" },
                { "fi", "eefa98d872c72b5c142c2a5621439a36361b0a30051fa006596bca5f1dba2f69fedc8d56cf0a9259ad8bb2960ab190da234ede74bd0969e460054c95e6cc663b" },
                { "fr", "a7729170656cff5d4f1a0aa11a9e34395de753691f6c39132120088550fac62c17bda029ca2748ec062b7e534858959e0113fd5faff4008ae8e9083d35d123ea" },
                { "fur", "b5179e7677ec41869551d96bb70b89db851960b7c990705c067d0fd2ccf20fbdce5f47fd764fb95f0fbecfcfa41a8692b2a963d5d5ce57a17ae3b78de3a6001a" },
                { "fy-NL", "edb7a050fb313caf658cbf52337352cb826767ab25c801bf25c7fbb5d6ccb1b98ee67902401dad6e0afad307b6225061d4df56e11acf9d656019b89a26aa2c92" },
                { "ga-IE", "8a3e9c196c2cab1abadbd0bf6f2cf5f19beded9057439613bfd1601b6aea221ee135b4d1fb569b6772705e19ad35816aff54b86b296d7b9c5e3f1b19886240f4" },
                { "gd", "d0aea20d9fd284460c9089c92b876dde20daba30d9441729210b5b60af953e39e7a6b90e40c201fe66d8fd16e01db96022689a0bd39d25f43a23a4a16d05d604" },
                { "gl", "dd6bc1aa74dc8452aaf15042991e673673fc966b2a966d07eeda13ed02964719e05942a48a36ad6b90120ea8035d49a3846fcea4745afd8b99fb736db192866c" },
                { "gn", "832c704afd37d14a6a3d723cfa68cd10c373c5e3de3de7ec26c6db58989745d37f61decaa629f9cf5e094097ec5ce3b70bf99ae1cc929a07e24f1f88e7f1714d" },
                { "gu-IN", "5882dfa6c6f16a99ba5e1a3f387bf72683561b9f5a275c7d15f788fa5487c2432e8d71f114b5bcb6c593be3968f41ac99dea43f4ccd86b89ce3f5a8457cf1d15" },
                { "he", "e2ec89f338272a6e22bbe1dbc0780ba25a1e7970ea7268c4f0a0a1c46ca48b5cf8ce2846efbfc3ca545a6456d0047d40eecd3e7aa2afc42f1e55992c3ab39fbb" },
                { "hi-IN", "2aac043b3bb73adc8229ef55c1194bd9c33ae1ab4a5d8971abcfa7208da1d0bd40138ff19f78c113916be051876631fad40e581cc8e7f984b07d3c0ecb5e897f" },
                { "hr", "bf6b60eda797f6f4e2194f3bf3e950cdc2764d48f5a8a147be5a7739c11dd5272b51e26bb30c4cfa37035cf3aee123e30efb58848c69d9d32f69e60bde0401fd" },
                { "hsb", "849674b1041a54c47afd753dbabd863d962976664c2a66402dbd5151dd5ec34eb3f62fd99926c189fdac2cb2c33e8eef02e0714d1e7552e0add1a754ce37ea6e" },
                { "hu", "39b27df0febb4043e5618e91270a67419ca3668badb1b291c9a696e27304ea7163dafaab751da219e776f4a92e3b355a47e96eb2cbc63646c9ef73b43b7b5d09" },
                { "hy-AM", "0ff173f2b95a005c941eeb08b631d2b82533f89e1f578168c33bdd004c795d438eeb14d0c75586c92bdfcb1841a525f140465e51cf2c635446bccc6f33adf1eb" },
                { "ia", "8c8e8de521e9b5d7894969cf28569b946334dcad235c860490d6987b3e255d5f29ab4da0a76b11459246f459fe3e4c3e8947b1afcd0055c72d42f572cb491cbb" },
                { "id", "e484c8ce15a03de9f008c4ac1d991fa551f76b1aaf43e3840c918346bbba52929c43110de5bb689a9ae6e9883f1de937bba6e898cca44472d0946c605005e28b" },
                { "is", "0c9cb5ab9f26dbb80c8fe8d29f4fbc29c87acef6743202dd24985f3adb5a65394f814d480d922695d0b85594a55b7b568bfebff9427897300c1a8337980c0051" },
                { "it", "62f37732490e3c071f296c2329130826cd9db7edd3cf1d26c21e818597946f1f84ccdae40d8449e813e336b8c48f85216b71e73df0fa7ad9612e034d5f6aa6a3" },
                { "ja", "b019dfe5c8192cc05e8dadced388fde67300a75e022fb9da32b9373a2f5afaac2c0978005957485d580e33af1562b57eb2b87cb83e281cab7985224232394689" },
                { "ka", "7983371f38a518be602950ae15b53c5d054797e8bb6c76369c5a1f071a7ef4b4ce2deb3675d05ad7f7e7914a6a949f1b479ee0f1c8aa9718093d1a4794ec52b3" },
                { "kab", "ccf220b1cff136c4560265e2572cb5a1fc9743fe8ba19d7e2de0e06a46e7d7889cc183132241b5d136c7b600e2de31ca04357119b710f757c04e3dc9acc4235c" },
                { "kk", "2f93dc2d4f562f7ad4f5960db44cc6596034e9b20813b9496743827f4476793de72744dc6a011b03e85d8ce3397f347251e011e1505a47af921c274bd8c1f285" },
                { "km", "a08befc7ec1f629c850bfa83363219ce948256a092bddf168f51f64e29107d8e5ac5696709c67d3dde37c0f21f6bb5d3434a87007cf416cbcb20cf298214da88" },
                { "kn", "8a8fe74c00153ea7a614b95e4d6fe010186997992a451df36f1174576fbd5f881edb234925751b88ef9d16f13af8d380266e058dacf7b5d54e1fddafe8c31f58" },
                { "ko", "f7ef925f11327593646630fc8bbb97e1e44b16afd6f8b2f44b5b81851f92a04f0f7f14338b88110e429676f251840ca1c43698fe55f58fe5d040b80a613927fa" },
                { "lij", "f468716fa36047a45176c69a2fecc9aeab1a087dcce2904f4276b5229fa98a670c0e50564bf2efe69623fee341aa87ca90d9de49bc3d92eb384036de433f0e30" },
                { "lt", "39fa5f84c56995239f6e3f73011987b0c0aa22ac3463662307271a7f98aab6348506a55b2653492640510adcb3f38fe8c8be168d890190be27368804851cdeb4" },
                { "lv", "e0a852421c4573982ee3aba10998e735b722951a05b15ee5e71f8326d11993daf350670f59dedc961f18c86e86bfc986f4af16f5e07c16e018aafd99c2652217" },
                { "mk", "432df6e47dbd8e505e942cf8fbb7c6f170c46bcdfe24b6c0d76aa7f1cf30e4c2fc337a496fe7a3a29e9f28a1f838cdadd6adfe0fbc0613d8233560a3f4f5572a" },
                { "mr", "60d331c7434f1136747d881badca520c58bbc83f74e4f26a0aa636fbf2c1f1dafb1a672a876929c4404f2f0289cede7fe4cc9d1366f702e0728239c951d88a6f" },
                { "ms", "87aea8bf717026a6ecaac2027ac1014144b879119b7e6fbed49e8218c739d7f073a65e7ffb4f0a21a6829808f5c6edf8842dd58e4af38d8b814fff069fa8cb95" },
                { "my", "5e94d1c0d5dc8933181e4b18cc535ae8eab5fac62853c76cb8a758021e2d69cd49e663b9d10e3e9e02f1f1c1250785fe47eccb8d6220620ee0217a710548002b" },
                { "nb-NO", "8e5726dc66db482181c32a964b8d76435e1b720a33314b44ecfda1e01f10baad5b6ba13543cf66d497ef8e1878e3e15d6e6021196b4d2aad526a5bd7e0f05ae1" },
                { "ne-NP", "716c052c2ec53abcd1e1db4c2819a6337e95db8b57ae06a852b6cb59e210dbddc10e09fbdad9b3a2a5266e11578b2fe39889b7258aa90de69448834413599772" },
                { "nl", "9dfc377bddd0cf6fd24b3535b75682b3e5d5bca01a35aaf80688d6460263e581810094e6536752c7d292827a9e0bae6d14c7311b53070e0612b1fb540c7a5f43" },
                { "nn-NO", "b907dd8935b62478ef5cff29ddbe1cbc6b9c8a9706fba4994828b2c1f24692668f970373cd5623ae744d0eed2d8e6b9323ed9c4ed530cf3f1e2233b46c26cb72" },
                { "oc", "9e2f2e2a0b9045354e9b703d2d8da3aee1a589365034a3d19a4af49b270827d729511ed22bfa1ab1900f130debb133be6f53eff815613a3b4fb93b76a49d8c38" },
                { "pa-IN", "27c4d730444175cddf134b616bddbbe8b6179fbd6abfbd609497364a4eefa0488be0d663582404acbc35d49380949ad5920f186793a11c9c33fe738f6959bd62" },
                { "pl", "c24a31b4adc195b05ee7fc184518348a781f0117d652efc9421d3c95911197fe40e81624881e1dc05bb455d2659af3e2e1cf39841f8700817c10aec92041b95b" },
                { "pt-BR", "5447cd6692bc8955a523a041f4e17855df899b8ba7f2849979a38deac15689574f680ff62d976d57d6ae52817d23740c220d8e4a61069b80ef706de9dbdf4001" },
                { "pt-PT", "a0b48ad9f80599aeba6a04c5abad5048a0283aaf9742b39b12960c4de4a864a115a578fbc02aaba50342fcf126fc094be5af29ecb259c7f44df8fefe595a7568" },
                { "rm", "ab1c2377eddf8c870cd3c32e14ce93f1f2c873444f3b6b7978f980030e216a2b662ea4fcd3be5c935bc0d999ec2a1da145cd9558a6ad571ca46d8679b0f578eb" },
                { "ro", "1c160dad73afc9bf77443038918dc7db270082191ff8c97efe6bba75dc5d13b274c77cebdf0470ae946bf7fa2872ec523cc1a422b8b074c26857e1d9a7bb7750" },
                { "ru", "5c66fd929a3ce01de058491034b23566cb2eee8b22fa6c2d54ef3fcc00a7fca214c1d3d79f4a81bf0d069e460d7051ef51b8ab47510552255843b3fb70216df4" },
                { "sat", "99121a85b014502a73ac59432709b0d31a9669f2da4bd8329d564b37c61bd0e07b57d6210da25280584fc2a1c5094b0676705439c76992dcfdd42cbc2df992dd" },
                { "sc", "614e637f6bd666904d02b53a983a91d423965f0a8a05bef8e8beafa5800c1c0d4736f752f2321b9a1213a2f621e47ec4e4be0699be86ba51d241cee6702d3c32" },
                { "sco", "026631418d7c66bb68179f30ca208d98113536932e43049ec2e466b6fe44e87e3a86a7f515746d299c87201c01620e3f0e72124ec7f17acd4a080f61abaab6f6" },
                { "si", "c8132397256ee44e08a965936211c83ef9a5c188b08a356c4e140d442b24139e3a44a4f29d45c431b92cfb9c58f5d9e6c5c471f38438be7c1ab62806ca05fda3" },
                { "sk", "27167b79969d5815110d3c439634ad17d97f134ae6dc4cb07a855e4fbd2d8027a8aeba8fec21ea690698fbda0a7086e633f9f33443edc7cd31f11109d09be2dc" },
                { "sl", "a1d53e143fee281497d209a1b9fc49a1c06a1891052c8c0801138836af84ae24e134873f24dc1937b1bd7a246847d2380e126061489ca4cd8135d24701dc20a9" },
                { "son", "a88eb51274cd083d07788ac9edd680c94390a85e3ad0df651e857716cd0345b1b9226aa4e4025fe2573dc3de6fdf8a810dbd51ee2d231e55a0151c1c4b41c088" },
                { "sq", "187f442573bbaaec0ddb9eb7126a5530df50a778ccdaa7eec88e3b75b513c0276ad136bcd21056e6d5ca36330195b464adb142282619e684145cc7886357e2b4" },
                { "sr", "7386ffa0b269c0081fd9e00734eefa00cf17a22794ead6b191551413730b52064757982bf1aff98639b6bae0404d2fe1e5ebdd4a82e31771b51315704aad9b1f" },
                { "sv-SE", "d46aae29153f041812514e6e10c760c853ad044a830cf1b2d1cdb74d2bd5dcfc4c30986bc1bdf1a0fd5116b7f02797d73ab53a52ef1714dbc445767435eee469" },
                { "szl", "d01b8ae8c207956440381e6ff5e473a314bf3a6fcdecf99ed1e7bac3118e5397066a5b5f5a570277ffa34ef2a48574b27c13d64a08e7480682b91882f86520b3" },
                { "ta", "f31d3b90b305ba7b3e103a597a581f0ab2e27ae8fff9c36bc08ea39b2232807e2903d0cdde142e5b638a454a398588be98a53e04c4113a92b9638142ed75276e" },
                { "te", "185ca7f7cba403b37d4e89b12d244b0b6a5f9adeef0dd70a6f35d879d5536a6545eefa99372eea15a1b0504e502179dc616989912ce6048f925186a43dce960e" },
                { "tg", "d87cb06a3f9638534a9cbf6856f4b6abaa491b7290eaf605509fc559cea294f6a61e9982842eb532c8f9443a0f3066579c02ab27c10440212c4460170731587e" },
                { "th", "233511efe6ac0c604c5a2aa310792b908f005fb8a7dbd05c8bcc8a368616f3238e2b4686212e75dce7526ca2dc04d7d0b9abf705df23dd172c181198f2031e8b" },
                { "tl", "eaccbcf6dc453fb7221cbaf686834185090bdf0b2532735a0e2afb8398fbbf503ffa74fd2a66402c235941b61217e6ac198d6c3456711328706fa5c8e24a12e9" },
                { "tr", "a1f5d3e7b56d59eb95a65e59f6f1d2856fdd219976c6cce8a29c6d67616722de07e9b1f026c60a28d815dcb0747baf54fe97d72174703b0477e7dff1acff8c2c" },
                { "trs", "23efc3bde8d21d37ac530be75dd24e9758c0f4337bbda8e602073647758f64285ce47c8705d5f52d358e7db701441987c443b8191b3d7cd37cf0d4c6416e7360" },
                { "uk", "00f3d7f5e1ea7889712936a7c7c49c00ee7c9b233eb2dd03c5a0f1e9932101676f2e73cfe189fabe6ee1d5d176dec1e10a5ceb8d296d5afaec9fa5e9e3864003" },
                { "ur", "00ece6b94bb528f4fe421db022bcb90f14ea244c91902917c180502a1546acc25cd5d84eb68e1a7e972bc78f5065caffc46b95759b7e8d88f4a0ad9ae3e8b8ca" },
                { "uz", "03124d54ce0d4b1dfe521f3ede5e2c1384e8af041071ec895d5df48a0057322b34cca5852b360ca42b3394ceed4e6ec24c6dc23024ca4dbba8e2ca0757cbf8fd" },
                { "vi", "830accac7ea24064b6e936924ac45e9f0be5236980df286fb5ac041051e47e8d40354097d4fbf7a86313c974c86876bc3a44d2640ef8671a9cd0739b9dca9e26" },
                { "xh", "23fcab35d3b6ed0c0b9936d74f5144b7ecb0d69a170c8962188a81488da039259d0c5997ade4fa47f14930d24e4335cc635bde934259eecbf16a537c34f250d3" },
                { "zh-CN", "9a420c813effc84ce4ca9eb713511622c926bb0aa93cdcdec2f5ea6f29a57a890762a8458694dae5b9d5474395a0dd240eb2a718d953b21b857e1567ca2a6627" },
                { "zh-TW", "f9ac0bf977d3e9192ff10dadc9304ed05d46da4028b883ed8f6f84f302b1de6b24dc78081c39e9046265d290e270a87f23c6b9770b669d8e6c153fb5ef3a88cd" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/120.0b3/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "5654173ef049f707670122bc914a45b78a2c2740939322a7b2285b6068d87955c2016025afab141b8717278bb479b42e8b9157705688721c49564b1bba38fb41" },
                { "af", "a73ecd35d5139f4bb11a93545b10ee57822de7091481454433bf4e814d09452ce2f09bc53be098bd276fb40799464eb7583f9f65b326a418cdc6edabbb840678" },
                { "an", "a9e10d5978f8592d6da631dca6032738dd5a4653a2e70d0ce0d28146cdcd3d4ecce0b91dec6603445e8e8a9cca19fa9e4687966efe90b011af1f4062147ffec4" },
                { "ar", "5426973d890233c6b6752ea783a34c30bf4149f92e1dd9e6db3f37e154e72d366f0d89da5e84d57de157696e3d53fdb5d11ad1242775479b724764e39ff231de" },
                { "ast", "fffe4e8c21e8fb527431c5de449bcc8e05fe0eb017e1d238d5f3b34d37574340fbd05541f118330e5550e48f78993ad735c1b2d0265cc380f09692452af1b203" },
                { "az", "3dac7c6b714c894d89950a7f1d605e1d54716966607b0a8c081f1ce959de0531088abadf3f6c6dd2061ee3d703a27ec4d747fa70151cc397e2ba43f2c51f8f3b" },
                { "be", "961fc052d953c1e4a93c45ae5717678bef0813f1fe99a75df769b0c82eade85edae9ebae0ed3bbab57291e1714abdd0610fef66e69e72166f86436f192b40d55" },
                { "bg", "4a41011000d56f0f2bb6deab53d45626349577de7da8a379ecee442b5f0d5120f495200e290299c6dbb2631685c72c3f05560dfc7d906f7b7c799d47286fde87" },
                { "bn", "68ad761052ce7c6ba31f2a8949bcb90ad7935c7cf3af95167901edba5f28ff4980a84a40d3ce35f9ccf5f7f5f396c72ba143c894e8fb00a9c79ea35e50304a6e" },
                { "br", "118ecf35769257a67feaa80e0ca17ef22f295693d4a8ee039e7108cb05ffc63156d9913aa1fa8800f75ffe893695e6f3a7825a049d52b4b0a6772886da02b573" },
                { "bs", "adfdfb54ab05bd7a9b8d062c82f7490662bb4ad9dbff12200c3bf56131ebd435e50ad4089141916db44e234acc89c2c9943b620a5caa98e3ce85706e39bf2fe3" },
                { "ca", "05e4cbb80473a3ff096a7a9743c8ecb5800e448fc1e69e9178adaa45332b2196b0785707128cabd93dd36490a518d5c10748a8a07cc2aa7badc3b8240a26a914" },
                { "cak", "ede1ce0e6924d506cad1adb11ea4a5518c7cd33f825189340bd3bfc001561bf9ae5dd26685a1a0ee66fd14ab153abf59807bece5d8f126e829754a6392760b1b" },
                { "cs", "f2ada762d95772375a061a188d409c1c866712147ba073dcdca2601f53098f3f1d585c9b62ee3a2a8983aca2e701c2eb2dc20310216f0ec5b1712acf7f0d2146" },
                { "cy", "737b84a10e436b58fe8760a5c870c52fc7ea2af5c2ca43e05869eab21675dbb442e25a14b76c10ae8dbc7c4d5e4b7398d7a3a372e35f1e126313242825941248" },
                { "da", "a2fedde4af6a91901ca36cf68765483c4127513fa161d499545a6a93bf409c5adffac9ee08ad057871a9007c86714309ce52876d6fcfdf2c7f470c43374d39f3" },
                { "de", "60b9729a8f5e122e7298234f32826ce0f592d68d55d7a95e212b41f45ee705886060ad3a6d77159cc825cd5452f628b00feb8cc33038a31e031b826957342382" },
                { "dsb", "fd1f3b29907fc59c24c109b65e7cbb13daabb1e89932614f6caf22f1656eb2816bf9f5e5bebb5e45dce8a6186239f221b3e043482d90f930347b0a85fdbb2ba6" },
                { "el", "c9e373f2a90eb1f68a07b8246ab82f5eef686e3f4877ee18f7e34dc422ad88cc18c692a5cd50e595a61f8e0f0805e3ba2ef795b4c2b9c780ed5eb52301454510" },
                { "en-CA", "9a92c31926f01f4b13c17c49df9ba137121466a57bb48a920a1de576d456dc9c25bad0741e4f79a05dbad557330ffa34639b14c1ca3da822c943de349c1631ec" },
                { "en-GB", "436466f23217693482151075d651714ab820a949e6905dfe20b559e8a46c4eedfe0c37498eaefa8746dccefea5779fb8aa2d0b550243a387c086819a3cd4e18f" },
                { "en-US", "28558c7ede5c91f102bafdd0329a46f2124530cbcb3d0ca1ec460156baa7ecbde999afdc16d556587dc56be5e1e1d8409e55012a8a87d32219a07527943168c2" },
                { "eo", "cfa2460bf31e6c120280ef09b4ffae7037934afc80baac25360ff4a8c1bd1ba83e52456ed5a488199f7158bb4613f5e233f1843497166d138258cf1c94d0fa68" },
                { "es-AR", "287e03dff21bdf07bbb385aa1abb47f01e4b7bccd7600966f3d37f6b7c76c0656e3547894f2d0013bcb2455958975e694b03febd4d971432478fb310fb5162d0" },
                { "es-CL", "14c9f2094525b89ee0826c33ced58c723aebbbc1cdbbe9809da878098240c7b520d67dc7b8f1d5fc038e4126a0a3a2250acd830ff10274e87915ad06a91a5961" },
                { "es-ES", "47295cf1ab0b84609d1f481412fd05974d7cfefb7ea1e02e99434510a4b0afc3abf343f1efba8d5b07a363f33c9e28f46ff0745a05c6454bd3cf23070e46a318" },
                { "es-MX", "bb0782f0ae942f176c971569dd05c3bbaaac10136d0b8ebb28d3df83a834b1ca5e49178ac3e127d081d9454f05ddf93a15f0d790c70254f7109fff6179beb05b" },
                { "et", "38da57ac4e23f2279dac00e18279cc43861da17151b3bd106cedca505b743a9ff26bb7ead6e5f979b1d6d0308bb880e3922bc77c1b658f28171508677be2cacf" },
                { "eu", "deb845ab034d5a6ae2ef9329c2f9a929e2c34a8286f8de59917f0be75bb97d8aa7ec527e85205cdeb20ebb83ff5343629015a4a3fcbbb47d11f053d165b89a9d" },
                { "fa", "39cb519afe75c62bdc54e6a17efab241d6277c3b815dd3c1f9ec7a119101768e98495cc5bdaa9f2cf296f709bed55329c0678b881f69647837e59c225ff8d3dc" },
                { "ff", "9935a01aa38db27356eb5c296badf8ac4aff1ba16fe7cd4c31649a764ec2813d32cb3bf058eb59f4d24c718116eb99bd952ceaec7c79bbb2ba70a3364739d79b" },
                { "fi", "52f378a5150c77ef1dc406fc9bc81bae7156ac7df587d9aa8e20bff0e164daf503619e5beed309720c2ee9e216069bc21c5c27a6efa99e5afaadecd045505df3" },
                { "fr", "b42ef3d79e8e3c2750825226b38b86bcc7b53ef28174bca2d76411b4b552b7c9d9625949acd0a9095a26a0ece4fa843087e964e5d7c568a0356f2a64cb7058a1" },
                { "fur", "856cac9f8b276a2c2c81e8d39f6b6a882adf458159a00978ed78d57eb78520bf00244fd92da28d320da26d30d78c2b23f686f65b8054b6a0630b541a53c6f497" },
                { "fy-NL", "a6a2c31c0ee9364b55c22e8393e40cfcce7d6835e7d15e7f99187be071aab8a87bbf06e4ea1c198a138ffe18c70b8062b7504165064a3412130993e59cf90747" },
                { "ga-IE", "b929738f3abdadd7678fe986348d70c0b1023eaecb81a45f1a03fca053ce0e1cb3b4e9d1f0381a80b8846b4c9b30f4aa19b78216f82cee78e84a5fa0d94a1772" },
                { "gd", "d1dce53fa4f24b78e72204cebd925517b9d51090d9e64740f563427d28d2c19051a5f9d5bda578880792b2c9f38ddbcfc88821713980b6e7efa170a4593a6119" },
                { "gl", "79f3a986bf371457682b4134e175ab4bacab0d1783a199a524a16f451ea70e849335804dedc4e3102f4bfe61a800f8c9bc3d95c7f17d004e7cc23d90687e62f4" },
                { "gn", "b153b04efcd167497d2852a2e77f81cdf2b0050fbf011473d97ad54cdd3c19873801695f36aa5997bffdd6a03544ac3b67b235f37cb0cd8a3b4355b29c3b34a4" },
                { "gu-IN", "40cb3f98c0b891f147048879d768423808ddab854356fbfcc4e6b35aedd3455c4880388c6350cb0948f915ab0909093d1369c6c45da9f51c0ffd67c902ae7006" },
                { "he", "d764cf98336e59254d4c544d3adbbe0551c34fab1ebd576ac26a1728141d10ab22ea60b7bfedb11808061b637ff69b333830f383eff155a11cb884f06a2552a0" },
                { "hi-IN", "bc220c2250d6dfd4353a0493f84600c9295ffbb8783e32a3113c182f7f47ca44a3e281a040eb34b8e745f35bafcbc5c4b7ec6128d761125d4d6ebc0c20a85579" },
                { "hr", "c612a68b1ae5f8be4d5b873faed0c905bfaf5195042291cbca4b6c2691e0f7026feed02bd458d1138373dcef50c2587682824bbf0e2e8b015547fe8dadc8fc9c" },
                { "hsb", "3f96d96feafc1fa59f3cb91f3c14a839a477b282a1161bd3d6b2c6342f751e265295fc311da68b988bb274744abde45527579cb33b62d377ed604d15a29962fb" },
                { "hu", "d46c3e64d4c543ce91856008a4919869e0ac7786012d689fbf2c50dd3aec3d7f729a98af19abb8bf30913d4b09765d6d957a09baab9712d85bd598a04e7d44fe" },
                { "hy-AM", "c16a9a88a2a5614f132b555ab3a0a41423dc261c37dd06fa3f19ef2437f8513ceff7ceb3b9e65307660a744f35582b2788c3db57ad12ebc00fe24e23a8f60c8f" },
                { "ia", "501720a0351db1c158daab0b91b6d9082d49d02312368b3fdd50288f4c30ffed77b7e3c18385b60d39cea9ed296edbcd9fa87538d5eb7778fa0fcbce1f975e43" },
                { "id", "e0d5d5c2388403343205fa534a602f8525e68bb0707742efcada6ea7001de8ebc5451b51165dbcb6c68c295ea98c3f035faa6f509e9ac3befed3f4aefb0b30b6" },
                { "is", "1b7236dc03179199d0f371f1913b584851eb78a7442d23beaa67d734a256711bfe83fffc7c35b622ba9ad4d88915cb2f0df867b354073cf77cb04e8e9a9be96a" },
                { "it", "e30fdd7a238486ea035ec467282829772703cc83d8f611a4da25a4b79545cdec91880343733d6d641985ed2e9675b7e96288d687e70447823c10695de01fb9c6" },
                { "ja", "dc595528625c7b3033dea3abe6c82c38111c196ae1c7b50235beec0878c2112f14ae81dfa23a2a50e760d18ded591e756238e0f97ce3f1e40370bf1b00c0dbd6" },
                { "ka", "3e05a7b426bd9ba0950d0ba4ac604f14674ed181abae9d19315f49ba9ec213ad161325f05225252996ca5a5773a7ccbf6e8252d8b861b9ddd1941585a491dcf0" },
                { "kab", "9ef3292cb3501512d8e23abb22be5ffbbe2b6310651b49d8bc81a8f7e3df5764c8102e52e6d73628c596520c1d6a2d7c0e82b2a9205c8e2e6a37bf2bf5e891c1" },
                { "kk", "784960ecc042d0b60eab39e0a92a98706c999a778d33f78c453059ac76bed28d72e5b0d22531d9752fed277fcac746dd5bb38d9fcf238b9cc80057a24b64750e" },
                { "km", "8d2104b5e45f478f375b9a308501c85c8566fdfe3bfb70a3d50096417f70b4ba6a2bec46cd77431b148f64f742284f5321acbf42fe0bda20ae7e9acaa53cda22" },
                { "kn", "d8235000bff309a2f350f1bc377c7ec2ff20512b0cbf314043a484ba828615e818044b515a7469a305d67d877c565fc35573eb13560ac1e7d2d87e9bf07b9b6c" },
                { "ko", "b8c111bd8ed2ca6cb295a581f8426763bc7a71a40e7fad010e1dcb6750fc7b8491d441f6bbd5fdc9f5b3b9ccce5db4dc17f212989ad4ea2fd5c91494f7e7a618" },
                { "lij", "993f511a66e142b5940eb433ad6dfedd21721d58b69b618da2fd39e97cfccb4aa5b5a0a69f4d22df7cc56e1d364cd4f9098f8c71d762340670d3f6f6e8c0efe4" },
                { "lt", "3c44597277cc42313cdbd525468261ccc1c96518a4ee9aa315b788584fcc66b0617caa8cb4335d03d639371efb9734896d71c6ba35d07bfada5e358fcb7fbd72" },
                { "lv", "50e1db06eb01e58e53a3323c675bee846d9e13d31c8c6f5015e713291d1952324ac9cd185328544ccf3b265f62c7f9561c1b8d4a10ebbaecd5950b9851773102" },
                { "mk", "89679e76509f1b034fbdfdb53b4dc8549917b98899f0ebe9f77a21432e4462e2705a4b999304ef6fa3d35967fb41bd3ea7d558f21867ce581779793ace3c85f6" },
                { "mr", "cfb9ca043f3bb0cb749ed87742af6413a6fafc7f7ada4ce66a671889be8e73bd6ac6865048bd6e24129e2d2ea566f2fb4578e5229f0bba79f2adeb3d379ef0e6" },
                { "ms", "2e44de1971e710d96f14eedc153593facfa464e4b3636fc9b8335ddc956634cf7537ca9277cb879a15088b24598005013995917499d7f6551684f295b0545046" },
                { "my", "e50409f5a5b252bd12158229f917cc1dda747710e0c71b525c2c5f22b2644f3f365203056090d05d6de9b23d8dda419ed2ba92f62eac301ff5c7f842ac41397f" },
                { "nb-NO", "6e18576a679fcc2e9c0069da86218e0191d6ba4415edcdaf0c3a7f42aee6f28256681432fc502cedf2dd6443f9331fc490f6b13cc6b45aefa7c936b461b246c2" },
                { "ne-NP", "344c4dda135d717391b2ec54d76c2855c8698323be4d52b5e6f0d0d1ade207ed3bad4d29c7a4c8fdbd25013987ddcd9d260fa289d97b1bf87c565c4235504982" },
                { "nl", "82ace410155c39b81a7e4aa05506c96e3f712890117377ce2f2ae8d9b395df45734af068c7045f7d9220c808858b0fa1b5e9fea009c04aa9af906b49c2a25d95" },
                { "nn-NO", "ce99ac3ff7d7f01947d861ab06fd37f3ef6827fe023a742c366084004f0c77fa14b91e7be254bb197b6ee9e6b34a83611a38008d97dffb0aaf72fed368aff5ab" },
                { "oc", "d25c9ec3bd146d85857c2903d416b26ad26c2a342a98a33863a7eb9629ee005446d207fb997833a0b2854ad2b8108240aef1d59c5a067271dac37ce323657879" },
                { "pa-IN", "dec882201fa765fa47dd919c73288d1b7d5ce0a78996d5e10122047c718481fd17bfdb97133f63649c81acc0f15f657d11ee0dcd56ad0eda48e16f6bc6fcfc29" },
                { "pl", "3f570d48beb3c7ea8493a9b257c5a50e86572ae2fe9f1002c0735d41adb51174c0b27b6636bead38e50698798458250e98d4084131817911dec338aa922e8d2d" },
                { "pt-BR", "235c3711dbd391ffc93289ed61c8156587983013f179adda889a7c44d900ec73c894dd5770a94341c92ab80947dd43a415e988f8629393554716bc463216d880" },
                { "pt-PT", "d3d945dfd84bed133456cff8bedb9e297c597b71b68b5ac049cd12e000a3f40a98d05532784217ddea4fbb5c21b65a485887ce52b3da22105b7f614ae98d0360" },
                { "rm", "d560f908165299da8b0de197310b6938707eed423e03c1ffd8633b7eb1733bc5eb5c00128f8d3178362c083744e60d7d860512c8dd8fd401e2a300b63f8b52f9" },
                { "ro", "43d8cf49bdf11ff2fe24fb9cd4fc54a6df105d1e2dfce91dece7df65d788dfb870a451b721235a3ddbf7c4a8aa014216ea137fb0de63eb6cb2d84d41b713619d" },
                { "ru", "a0e5d0dec6f360c3de2bc87cc0091daab2dd6583d03cc14553a99294b4aa6f6afb2e93c893b828360c5b93fdf06686aba2fb67690928a597d9dce3ef81998a12" },
                { "sat", "4474c83a000a67868522b4c2e23d58ae2211011269cdf18cdb37b668ab52341ee404315db08942d734d90d5a8d98e3eb5355b92b7218cb9d193da7a846c1ac0f" },
                { "sc", "c3d06e9ab86016a903cf06dee7763628d7ceb0fd52b817bc77488b0c662911e5dbae932802419593feb798d0dcab140a0cb0ed0778e28ab30bc5da89b618ce1e" },
                { "sco", "0e6a761a971e9e9349bc96864566eaa7e50db28d31f5e5291ca36ba5681170e67740818d5bc6be93d8e60410455c1d5841f03183e52501b42c8c640a44021175" },
                { "si", "a78cc002c778d9d6f471606182433e9ba80686cf370e011caf117ec2931153bf7114e444bf20c55fce31190e9de623f7be50c042a7efdb3cf6dccece501719f4" },
                { "sk", "61b794860b256898229191797baba66a0a4c42ce5741d2f07c2e1a6a50e86908daa0fc3175b224ed9d8db7dc4625b06217a4cd3cd9fbf0aea6b665f86a0bfa61" },
                { "sl", "c2236b4dd538cf2383da75ce9e98a9150e2cc07aa79b1786773e7a400308f2a8c853408fee55f466b039ee9eb924aac8bf8de29f96c9e04d839c0989ef763360" },
                { "son", "281513fa1cd4884d5dd31eafea8f059b1e44080fbaa945abad8b072ee6208a43d32da9ea6eaeaebfd4d65d665038ff163c184e0e1afeda351b3aa055eae39285" },
                { "sq", "a23c349b6adf405480ca858ae914303ca59bd98b335fd363d3013bae365d22b9c27e4b8d614e1aa10985e1569aac90626c4fccdaa87a91e461860f40b133fe17" },
                { "sr", "ab8486b36e085f35b7d9d54127e1c6cd44831311264e6762c24c7e332367700f6e23d30c53069c690dd046e1713ab081af4e5a03c49bacefeba414dd9271b76d" },
                { "sv-SE", "15510a25b485290c05c2ba6ebf41da95e35fc43dd49c2664a5268db48a0baa36249e79139125cfc4f4cfb4f62bb1b391d288594cc429876c339664ca866d3a1b" },
                { "szl", "ae5efaffd9aac7dd5170c79697ca3546221d68eecfb43f2ab430e292873231701a013ff6e24c95b5f96e772f78e4af7bfbb7996353bc69a9a936b1a5a3cf4710" },
                { "ta", "897154c73302325f10c6996f45de8e90c147a53f9800a4a68498757cddf8aa02bf05a346cdc1d2d67ad0095e7ef79695364cd493a0addc606040ee2b53b490a1" },
                { "te", "ab823639f396b538be75dc7d21fb4b014dbbdcac9b92bd8cae8ef6a6c0a47d37fb5578a0d50fac7a8077c4b6e422cef473f40b106aedb60206b1633c912ad453" },
                { "tg", "bfecf5385d18bec14929d5eede77a3dcd0365a06febc696adea87aff26319ca1377f287afe25dca44a6d2fb62644deca5cd2afd41ea1b809d1fe44d1b68e0186" },
                { "th", "0b768b5897044439e1185b8203c47f8117a728ed14f4964b98d35a1fef4649f246433aca4126c7e8b7810667fb345586e5712d6de1d7a7d5406d00f2d7aba3cd" },
                { "tl", "170d7bfc7faffcb567e20f3ff7a88fa1960c66798d1e91c284283c900bf386fa3ce3769ffaac7140e17c6a8b75b715500f0ed1f002584363695ec7310d3031e2" },
                { "tr", "7173a9d94d1fbb5dac94d7c738f7eec56f045cdb60654c57c6cf12176130e54c4fdf023dbc58759508f332829f9fa395a468183f7e1398722f9f33f1a87e0299" },
                { "trs", "282b955304f76c0a8a6e40390bd1b4870f613fe6c5953f230f7747be944608b1c94400b7011960127b3103ca2709e57526abc9a3b4799932ee864f2924943c51" },
                { "uk", "572439c56a3303b71174d782f23e6cd05e8ba1ac8fc0f6d838ad101466a07e5c4e2176aa3540418cbfdb5261317023358b529ec4941394fd7eac979a7a1b28d7" },
                { "ur", "aa96477003b69f1d606898921c86541a324c130c7eec02b157d909b108cec434a17e79ad01e286b87d89ba4241e5b397302f557c312c5f0e95080217af052a19" },
                { "uz", "c14898fdd5f1f4e54e1b9f73668e3be1301858adb254dcc17b0b120d4fca7eb0e3e3d00428d98e3f6a1fcfad4c2df676300cf84dfef316132c1b3273c98bf6ff" },
                { "vi", "4d5c59f002921bc329e1d8950b2f204a96089e06fbe7fe1f2142c8f87f908ebf691d86e10a8191045584b352a7f0a57d69b25cf36b947fbee5fa2570ebff9ab3" },
                { "xh", "347b1b5071a917e7c0e9825eedb9540937596a287f2b9bb359910e3f7cce28000e36e798bc3b1427144e2bb00c545e5f0d48b7cbd6f81c853c3d924567984033" },
                { "zh-CN", "10582507b614acf44fa1d41780e3d20d6a658810dd682f0c3df08e1ac3e8259ba45bc733546a3d99442eb2e4fa1131690be2ea3ed1d17efbf1c015d8906ca9d1" },
                { "zh-TW", "bf7adbdc2f59a56e3c0b6afdc225edf23022c048cd73d7b76d7ee3a314850337c828d075e1a98547bd7c9344a83404dc93c542f58540801037ef495c86bd8374" }
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
