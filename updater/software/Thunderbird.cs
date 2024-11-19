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
        private const string knownVersion = "128.4.4";


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
            if (!d32.TryGetValue(languageCode, out checksum32Bit) || !d64.TryGetValue(languageCode, out checksum64Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32-bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/128.4.4esr/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "7575ba4032a335b7f39cc82016ba130815e6574b51612431988008e6285c3c73f0e7681e3851bafd94b243b89f8b2996b9a7ef439ce595e8cc1f37278bfb5c5f" },
                { "ar", "60641118b0ee8d938c91b34a8cc4aabe02576491eca2cf348f070e1afbc2d8b78723a8de52d1c3cf7878c080095fc1ac2344bcf04a7d0b55f3f05ed3f95a154f" },
                { "ast", "cdc2535067a8fb5912d7fb594a65e16d786a43c165062f9b283022fb8c4c4a49ea7089f7d2dad7733b9b5fb4393d1b23d73f166e515b00b27206cfeea093a295" },
                { "be", "c34fc24be37edaf7945838fcfaef93ca5181db543c2e8c94de6876a283cd9519d7f4ffff84684e4a40ac2997cee373c400ba3da7ac9912ff93fbab961d5a3886" },
                { "bg", "0b163fe35bb436485cc6d90ecc48eb8381e1b71ffd56c4dc26aa359d048330384701c0d18fd7869f9facc0d0de1e4c3ce646f1c8d0e391388d4defaa93e62147" },
                { "br", "cc306f9fda48a6de6a0d22ee9a3265346b7979fcda69f51ac1b2eee5611de9eeca4fead93f85007e8d13a661a6ee0075bba60ca5834a38282f909b34ea80f41c" },
                { "ca", "afe8f184b73c078d3f4afa40c6e93b9784416da2797b875ffb1a60cf1fd091a935cefcb6d284712882b6ff500fdc69de9b39c5e7749105a3d5af6485fa214cfa" },
                { "cak", "6828285a1b8e112324c88d3a3af232f425ab10e7c5fa3cf21fc3e62c4542e4d34a7b025e246515254f2ff5c900a60057c28f9255ffa6e433eff1ee4c69b16d0e" },
                { "cs", "2bc7e5c254fe3d3d38c61a293b353e7cdfa39082340f96c53559392a47a2a131fb21b44186213a38d70ca57bf60ea42a330701303367f9767845c27b4bcb1562" },
                { "cy", "f2be5b294c580f6bf2681ec56ad23af439ef505cba9ac7ac821c65e9b899a34fe20d50a254f98ff54485dee18751ba5d0b9692e980b750cf6f401fd8bfce0b48" },
                { "da", "c1322aa5579d419bd7bdc9a22cd2d30107155e32a1c60b7ad3e9aed5878097d993f6dbab42ced392251540332368dfd199e3c75da0c78e2189688fa274f3d2bd" },
                { "de", "e887861134b9b747f7f6c3d13d95d5485b16c4a2fb2681ecd982da983f2aa09cb4e415d004c549789b6752bd55f3b4066fecb64e42baa5c5561c2371bcd240f3" },
                { "dsb", "b77823e1c210bed158abc60d632ba18123890fdec30efae53ab39d7d78c4a70b7563f659c2efc610515d326c79f6cd51f040ad5a796b1380a8c5471e2b999ab7" },
                { "el", "e183067fb3762a4ac45f52747563b4fb2fcb8b846e8bb8d79c6595b66f22ff92f83dfb19c185a2495e70ebc0dc839c5e7e83f80c62aa7a166ef5a53c76fac5ee" },
                { "en-CA", "c4b03a145a7916db65e138d88bbcf0c401d2540c5192f8d29ab57c43aa9d9279af29e0d4398e4d60fd135ffa4df7349b413c412bc0f553827f32569d36e21f6c" },
                { "en-GB", "7842fb4210de8ef10219c97acb894c184d450d928297028bd92d3255499a95dcc026a329bfb04f42b45a38fa3222e129fc00005e5a0f624c116dc8ae2ac91a17" },
                { "en-US", "00fee984cce81ab7ff3be89291c82fbad18d20da4abf800988e0686fb05ba0be0dcb85163b8dc1f5a06b21e5eae0db158c90e51ccc18af3d908e8a396ab93181" },
                { "es-AR", "8dffa94b3b53e3ebc41c2d48be7d73c057e0730c4aa4014d007d406ca0396ea08a31eb1eb013dae13fde2ac9a957400003da3f9469bab1723cf56a82a3acaa72" },
                { "es-ES", "16fe6ac6912ac00d9a161f218ee9d925cf2a95aef575d73d6457314eade0834b1b945e8ea1a0f847fc087778d33b27aa8dd49254956a594d87c27b1a0052efb0" },
                { "es-MX", "65c4d3e9d9a345afc2c3d0a3ad9a9e205dcde5cf5b1c39226ac715d6bee82be76edce6648949e6911c77f58aa4c4097cc0a646dfd9c0f98981ff735245ec5bb0" },
                { "et", "31ccedb7e6c2ec3a7c94711664ec9097764b5e11728defc48d920f8fe47e019ef6f2c6c7701a42084c79f65545b05ed30a9019105ad257677256784708302a76" },
                { "eu", "becdf90c9ed9211a729ac799e3d93844cedce831b01c49fa44ba51bd7c8419152762bcf8c6c8567b7f5a46186752806705f0efd0bddc133499b2a36aa212cbee" },
                { "fi", "c5dc1cb8bb6fb10decda17d41bbd8ac9212fe6d96e46236048d09de985554434fe236bb88678e834be4e0ebe452f1e87d755b8b1163e6c22b13d75eab546bbd7" },
                { "fr", "56260411e01bcfae75e14252f509b3e5e4979cbbd8cc9356b722fe5c3bdeecf43e817a386d1002c80259795636d313d6f5aa84eeb57bff967e9b65f77acb7498" },
                { "fy-NL", "b12c97287b59017771591f54566d08d813cc85686878a0d3f0a517eec661e884c784e92af34ccb3d83ed5b0c1057e60b6c680de1e0d3840e7f218a26bedfc064" },
                { "ga-IE", "8893c2f51ed1daf755fc6141c23f2637b309b12b114d130cc48dbece0e2e4175070689d03d8c92aa1f2eb76d764b6ea7b7de49b556817d10d753ba749419b69e" },
                { "gd", "a310ef861954a1b78e64d62fe29cf2ac1941c799b89ac793c0227c2df9f1f78f157947aaf8701a8a711aaa0b580635d20f89b7b9f0c0b8aedfcbca317ae028e2" },
                { "gl", "88fafb999fcb296945862122fc9608a210bd8f01489abb0eb289ed3b86d43eb0a5c5bf60d5f22cf9244f9ce76d7189e4e1f50d9165e513a803b073f0e9687f8b" },
                { "he", "14d0b2aebcc20cfa18b63ba7c35c8da8f964291d504ef651ae0fb7584f40e3defd7028bf3d091aef6d2c4d4800da5ebf804be842dedb2fdc34b43a2d7ae4e563" },
                { "hr", "cec7e7d5551f3d6f674c7638a85279a4e36c8448b80af8a1c7a78fb409dc5fd470e1daba9999d608d22a91e891c633fa4f9439261a08dc1f002dcda296b2721e" },
                { "hsb", "9cb8cdd5ffbdf7dca4c1cc75c3d5867fa5b07274563a8863c2525f1261a6305ae84c1a7dd5e4742f68d4390e819bb1d380e043c4aa4d22ba8102a4b9fa8d0b55" },
                { "hu", "f77021d70900500c7a58d324d3b4226bdeffeae3250eb3d4f6eb23a69f9ad0d2912fa2f43e9ceb89f04c4a75fbcf5ee6c55fd171001d8e2f271bccdd2cfba27f" },
                { "hy-AM", "c6c2acec314c6ef0daecd96b72564c0ad5c81b652216e025e48df1c2b0343c73fe60acb6c382296d27c6bc9f3febb95b38fe8fb18c54acc805379a82559e0686" },
                { "id", "9f4d99075e5be868c3f490a6e758c670bef3b2d34a19cddf6d50b660ddcde21af1fa3b75bab873b4d1584bb1dcb2767e1c257f6279bcde34d556815797e3726d" },
                { "is", "835903a1cadc9c57a8712a6b5372f8e03b18bac0da6ea41f1f587b3d8c97706509ed9d1757c493226443d2405dbc2e6373efa1f5ed427726f3960dc51ca23cc8" },
                { "it", "699b369823bf9d5c212293a5212feb504091a8394baa5e6ac0c7280c4c4a4c1181736fd9dbb585513bcd43fa417b21f88cb48bd70af43c99f184bd54979dd7ef" },
                { "ja", "d78afa45dbaefb6c01ba15cbe6421815716387e0d040099a059e3b067a4faffed647ac25b6cae4a5034302a3f22f4384baeca4cf36f7496cc72b8079d76e05c0" },
                { "ka", "444d52d489c6e28f30a4f4c489bf0f7b7f6eb3f158069d4a372207f0e1e3ecf05303fca6bc9e0119d18430cbd00cc35ce49a1bdca263c2f0014832927afea469" },
                { "kab", "ade627ac0a154450c178ec0b360ae0d3fd89f4f9b3ce4c1717461da1a4bb6288f17e20e6ce5a3fec37fa7a6ddd6d5de28ce4fc081c4c59f92e06c253fdc4b53a" },
                { "kk", "77f268336ead6f38eae6b6f8d805003e71d8201d95a9275db24a4d2c7c05b28f93cf7ce097d29b6def318a5c58f2681d4b4982af2f3c6d7b1144db344d2a84a1" },
                { "ko", "54f099b79be0eaa9a80a555c870df3cd9f7e6b0a4e03f5a595177c44215b77d05a221770bcd7e45a394ecbdb76fc6a6c49aec9a0e956985c247328ef880f02d9" },
                { "lt", "40f0dc09da6ab782bacd36d92f9baace19afb149a69384894951f9174e268575b723f22e5db3a369360482e0e5a9db6036cdf81c19a8464195a3698f76e4fd24" },
                { "lv", "5a6412aeb6df9099cad534ee4236fe289c78451c506b69d6343f0ea33f54e3a1781255ef6aafd6c9434ef176b8ac59abef1e3ba2eb598e6abebc2f64cc59d6dd" },
                { "ms", "4c36b45717bbadb4fe2b08fbab1cb2009d6dcdbe571141ba8390708634a4d4f0e7c10d6ad03d522f7a37c03ed04ed98806987b91bbb4972fdcd8289cc38a452b" },
                { "nb-NO", "d24db7f7cf7214f1119255b91e44fbdf0ec5b2e4dcb4ec0a646b2b9a5a24ab01d087e44b059f3d44c4b0a78883e5c13e7a2782f5ab8e9a72f8f369df0d794a16" },
                { "nl", "32d19522c456cc202e9521ad61a8e35ae853d7f68c37bdb627f8779f65d656fdd6cbcbfc9f7ff9cb19be5818916f24aaa13d7a74394b1c3eddf5dadc733830f5" },
                { "nn-NO", "68e3869cfa38f5f57078fd313d6cc8ca9c6b1f781841b2aef3a4298f0e90cca834b01cff6ea84ca408524b759c517ac4a71211db82b6e5595a5ef14224dfab49" },
                { "pa-IN", "59f81c5fbd88070d89436fbf93c2324e84c72d20a58be4721a65907658b165a2d273430da8a963cc6c4905ef658d152a4c5d3434253f7bafc2ce20f51e7e2f15" },
                { "pl", "65da9124bdfa5d27839c18e820e31fff94b2bb9289c8a369312cfddfa7b01d534dff1b782e187bc62582b449634b03d0a150b99bada27ac10600dc51464f9f03" },
                { "pt-BR", "b0539e062288e5da144747844be829beca0f0ed7681cdea4f56065ef59ffca915ef996cc7dacf45f8044886f917c7a993be1867390b0723b9867abad6d7bb810" },
                { "pt-PT", "25890763013fcef5b153d06114083574b346eb4ebe181abc1006f804c49ef3839e6f71bdc91da44d217a1fadb7fdca58e7ca4f87b1ebcd2897551d49da5536f2" },
                { "rm", "8f3518415ba95cb1dabd11d3b8ea18d557fdbc9e200d741feeb2ad8c4242bdc098e0ddbcc9ec2e6270bba72e3742b5e0801eef7d2cd5c627927ad04af8eb5e8e" },
                { "ro", "271abdc9a3a8db03792cddbf2ca90b0a5f9648bbf63077fe381d88b5b2527dc1b30ca8a66a148e00bd535185492c9f8f392ed67af36575909930473c09763aa3" },
                { "ru", "cae198e53995195d1ee872d84070e616594ffb57e198f9165f2c7317395e9fd3a26eba58da0d2530b96c6165dad4fff57b14acf24c52e6834d38145f7a18f494" },
                { "sk", "951561df2d5dd763ad312f49d87872622213d8277dc62a046ac04a39e155626894295b7fa062f5d912659075720d84c763068578684b3a47579e20bab8893ed3" },
                { "sl", "2c963497f9d45d4bfbf363edbd100bd1077f52f2548d645a22c628963ca741f06fb7838a836463a35a3b92bc0a5c128d1bf1cedecb133f05eb1eeeccdd3336b5" },
                { "sq", "4db13e4a3d6fe9ed884ba1563946bc1ca2d07975ef740c927b31ebbe9a22de60afc944a209eda8a68aa018b328c7c9e31f5ea771fbcb8d127e6ed1c6ac959062" },
                { "sr", "c635c737d4e9683b58e4f0b74f95c6c613e2b7185893b40da3e0c7670b234b53ecef145da607a2249f218fd3cc491fc4a6604a5ca8c71713de9e36d70fe33087" },
                { "sv-SE", "be7de904dd8eaef2c015d9833b0faf219a79e31464c568aadda79efadf0b310ce37ae3ef7b17009dfb6d093eaa3aa3bdd6586931dd69aebaebecfa8623d5e8ae" },
                { "th", "d54c2bb5b92f9bc8cc250c26da2a0affebae769b1184185a9a5bb5af085324a30c0a10a0a480ed5c481d700a8684dcd8d2067d0026a33105dfa2d8ba05e506dd" },
                { "tr", "b2bf19f7360b3d0394f77fb18186f9b158a2fb009a3209f1e2db030424583948e1427df354a4329727c949ddba149e89c18db4cac9d4c08a96ad633e40aeb6c7" },
                { "uk", "7cb93efd388a981f958dd6dbb0191a8136c15ad4504296693636bcb25b4fa028b92a340ed6a758d2c8d6755261f40d203c75fb6b240fed0cbd86b130604ddf19" },
                { "uz", "70f7b35c334810650f3e4b8acd420539ae1224612ce78fe23fe11e82994021c9ef0e9812a0932e280218a36f94196501575765bbcfdec8f4d3e3102cfdc78f80" },
                { "vi", "329abecf3279329d1fdede56b17834fffb586b1c3a7f583cb5183abba6a8e5fd8210e488486135131e8d9ff5ad5bd51dd5e5c0852517a996755ae707fe2016c4" },
                { "zh-CN", "5d7ac27e106729d545624854cf05b0ba38420fc7780514f212182d99ab414b6ea5aa3dde9dfe6027f7e2ac9dfead820daf3d1d92329b5fcf06fc1b86f274b886" },
                { "zh-TW", "67e30aa1daacec8eb4a8ae4c84ac78799c2810e1bc55fec16ef189c8392f3e88cc58993248aab87b49fca08e0a7948ab2baf3fc053754719dda41b4146ec2558" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64-bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/128.4.4esr/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "970552eaccd8674d8185c3bc844312c9e809b16a48702153a38e8337763f51269b5de8379a0271c71ddd04394a2bea8494f698fe3cd4dad3cbfa2177fb4c41ae" },
                { "ar", "6047c5b3193a0cbd073ea49e7f83bff9ffb4d1271d469c305032715f0f8844e7581f8d3963a6c741463ae3625533653a35e757d550ec2032eced2b1cc7fdb7ce" },
                { "ast", "3ddcf8459d2c336c2b778726d37c4447b1d02b25016e50079a7ff28678b4c4d18d97d5ffd3e9cd7b1e9b3e1099c775fdc4fc3da128a5140f53595f60617b211d" },
                { "be", "7bf7735e8f4f05b1cb5954bb2c1b3694627e5b2810fea752c864b119426ba87667e2bb5bd70ed400beb653b0bdac2de85a6749590784f5a55628e83964be25c9" },
                { "bg", "7d60f5064d8e170df1f2ef05684203fc9ff03096abd98d1b8b9713f9d4d5ab466ba3ef5a41d75ae5a5ca2a36958c1b74433167db542d2d0e68e3a959ec9fd715" },
                { "br", "0412f173d4a6e4593686c62cf9be1515116e6f3310cc4531270f9c74a54170ee124322044def5508a667344a618d3bef26a4924dd7b157c37f113c8b9dbc8e02" },
                { "ca", "281bb34d93e8a3d294aee1c6d90e7dab5d051191b4781fd7edaae7c6facbd92edd25e6acad00a32ce49eb25c6e9bd2d721da350c14791eaadb2e185535230a93" },
                { "cak", "fef18c69fcb8d2be41fa4b747e1be99b0618f8e83f331588a40e2dafc06d4647fe67940aabf6f166fc849e248fa8a5ab3a79e76af151f67b946a08f5e51b9bec" },
                { "cs", "de7feeceb9f9e4cec93afae40a3b149668357753133b6680c5f4cd6495760c0dfa4d5a89920f93f20ff2fe9c7beee788fb93277b6bc9a69011a7a57fbf76a225" },
                { "cy", "f6fafc7dd5f248a61b0a3c7e0a66ff75e20a2d9e6928eb5b38a153b0a409633c96584c55a9db9ccefafa02840f18a06d45697e70f15f5048fb4042b300b4e5dd" },
                { "da", "b0675dbb8158f4af9ad98543fd1e5bc8133035aaf9c07bea71133425656a311ec7354f0343576dd543cd7cce352e2132568ea348940e42caedc1cff87183c105" },
                { "de", "b629058c971a3556e458ff3865fae48ba4bf7657a7482360307cdb0c964df0e4e48836d1e33b4ce2d929db3b7e67a563b16b309f3216735f2400081e7a1c5561" },
                { "dsb", "100b910f49feda06457dc1e7fc20f8c754e73c37df4e6b1f510cf7a69632e3d410401bc680f036b1200ad26babbd162435baaea956515267b7e11aaa2a8b939d" },
                { "el", "b441e7a2bd7b3d611be9d0678d3ec96b8a55dcba57525e0baa8988ab988133f4089ecdfc9b7de06c5ecdb1f9f2f378d72d3c613ddaec3689e70d0695b48df152" },
                { "en-CA", "2ebfc6514dd02960047f65952d5dada8e99cf7337b3c8acd377c4f2fbce334fcafa4a61d8c35020e21a9218ce21622ebed5a057cbed3f003b225da0f5cf37b3f" },
                { "en-GB", "f013722ee38a2696b961c896d1e50b34b11620aa79fdecd6f96c6a16ca05b8b0557c6f0179366931d5bd07b8860277db064e4e68c6d822add049a344c9ef12fe" },
                { "en-US", "ed93c0f9626b6845be641eb5f58f97f20c30c4e8b61d09882942b9d7a877d7a065d51bb678964ab02d30f769a3949bb03d78fd8088446eca8ab3733668534d11" },
                { "es-AR", "ecdd9afe1feacb37ba2d6b508634f10e5ec6fdda21a636dfdaaf757ef2b1842d07c2ddb16b9093039f32689049134ddf60843d1653ff4ac51b611713da3d04c8" },
                { "es-ES", "2632ba0750b3ad51cb03adaca0b05170a33e500f3c0aea602c8c020e46db21bb0bec70b4683f28d4d29d000b99deed0170f03affb6fb666a94d99096b2382665" },
                { "es-MX", "63f7f0c3d299debce61845269c6d660cd936400fde3ec4e3cb71730b0240c9df31a3661e820381bdd547f96aa864ef5cd011e8f93c03b0a644ee9192479b4f11" },
                { "et", "69074b92921d51864ee707fbf2ab59c8514d98acbd8e7eca46a3d38d2dd4b692218ef5cba85215232eada7c5e8bf28a877a84d171b1c7efa73591fc7a54bfd8a" },
                { "eu", "412bd7795dbd1502f19382b5d9ce8b9745a606d8471100246d688e500152395d72ca7ce55de3ba6278c46dfa67b02adf0fe845d102c858bb18699a349aefb714" },
                { "fi", "6fe038ac629bdb5ce49ed5337c637b6d5bd535aa3e8cf75af4a046da5c9c46a048f48d401b04d9b6aed7785e9da24d181227c05d9f2136298e1ce15daea93c96" },
                { "fr", "d36fc1ba3fe9cdf1a9716c2e11ee7c3c13cdaa36e828f4e34e26c327d738165486f02adcd1dce094fa6d10455f9a37f881968b97d3faf880df257d7b16ffb1f3" },
                { "fy-NL", "6106ce9b5802d84781be8a01048747f4fcafc7f405db151ae1961ad3304357816e993e2c1b35858ad55c88b9c8ffcdc9f720c6b5a56eb2faa75de8c65780f108" },
                { "ga-IE", "8c89104b0597397b5533c9fb9a4361f43442e546e81b3d334426b0ee9d813544da749cd692877481ceef1e5e1c1af83420535fd90d4711a8d7626a12dcaa1ec2" },
                { "gd", "f6fe24f74e18967d9505c3a160d1ad1e21b23ed7fbdeacef0c5b2352f1dbfbff6119830e33a64b22583ead2a24fe49f325c40204b9d69f6719d40d7b628216a7" },
                { "gl", "8a8fdc670b794c512d50fc2de558d3439f4963a8a3c66f4fb8924796c4bf3bedde9cc000887d57e1e9fdb0b0357ef462b2870b1a491d189e361ed32683f777a7" },
                { "he", "5442e3381ad3245950157b6b1d2eb8f7e7e1b4d55c6b45e8b4c24fc5f140ef10e12c996b7406e4f25e0c496cdfd188b8ec1f67ba58da40f0977725951b3b8fc3" },
                { "hr", "69a425ea7b0b9915ecf51f30f491c63e39d72e99a21701d7f58588b25bafdae45a05660327ab85faa6e44ca8dfea8eb6932eab36918484304d12e718a6b270a0" },
                { "hsb", "b5f3619671c36e690b67faf87af7b8ff3926f5f9da83064d5396e15bd6533f5e75d56471149cbae6e8ff1ea413ed9b408e331ea4e3878a5d56877ef9f225760c" },
                { "hu", "828145b204489ca63ea51e4b61827052ddf7a65f766d68b02bee200e7bbf874f3c653e34423d3a5b86f9423e3e9416d7618d1835975486f394579e79aa8d67d3" },
                { "hy-AM", "9c54021f0ee049cbc1a1aa247e6c3e1b61d8cad76fad35b674c5a87b7b75ca7e6bf477cc4198b90cd78e7ca1fc5516c2515cda29c962e2113db2cf630363602d" },
                { "id", "0641a1656990f25373ffea97bf7cf4a00021a431c0d2e1da34c5e38820867f4878ff09644c8b51969fdc46c2b0b3c121e07cb695b77d68e6e2fd49a0db038cfa" },
                { "is", "4982c154b4f41048b38044edb3693fe565d9e3be5eccfca26f9a0130fa0b83282d7034771dc9b0a7250a69d56f98215eed754153392481064158da84f5901719" },
                { "it", "3854ba1f5d6660c2e49f2259e8af91953b68b73a6583c07859255a988aa237797aead7fc529ead5d718a99251596aef0d1c48babdb9a836551a6257e65622fa0" },
                { "ja", "e7219ebf785a74c2667bbfe82ab88fffdeb5e89240e72651320ceceb32d4198b4f9cdeaebae65a1d389bfae8bb563c6fb89e4c58f06a4e4c33122b1649940388" },
                { "ka", "e73b26c1d88800d57a992fe3af47107ce5cfa15723a9fc32d21b8ac414734df14b6e59a9b1b7b6cd06404082c2cae418d8d30220301c2d7d9231582713db1e83" },
                { "kab", "3ef6b9a0ea65da22c009f305f2e443965d6501e26171e79086f76f66467b131d06333ccdd3cdb5d21a1cd11720da90d74f4eb500a9d9286a3ca9aae1af26c8a7" },
                { "kk", "a494046783cc7f163a9cbb814c9bfbcab5b4574eb2e49e17783a08a757f5f4c571a565ebb125675b938652bd6b0b504e7e674dddd8f3293ca116e1b3d0894cac" },
                { "ko", "72cd296d083b37b4f9fccfd112077bf6c7d84167a0803aff7a7e78cf1372b4d5750a60b631ed9f22c61a364d8dd81a7fef741dfe2e988147ad6b63551c4e88ef" },
                { "lt", "2242fb6257222d97c4d5dadd7444562929f4d20a3be71c0965d7e09e95041d68208a6f346158f6dd9370710f5d4accfaa347c09018aa760544f9abab31b4ea61" },
                { "lv", "93fa6bda6f8f5fa18a429fd5bf79dcf1dcabcc2ed25b626d85b7b0b8f5cb4e1a57116029e234d1a04bb3ab74a74399fed879d1f55da3c7b57cb773d7cdb10e4d" },
                { "ms", "278bbcaaf6a4ad67e5f8217896ec02fba4b4353afda0c24f2d46e9b9e4856fdba857f5c80b67fe1ac745d59fe1f3954607d5c951cae16d119ae23e06751f4653" },
                { "nb-NO", "57b3ff2dc02f83f88a6dff49c62ce0a050e549a5c9aacd6f1e249ba072a201a8e9979f0e84de1b78022a62ffcd6c8a6220dc9118073cb86236e25a9bcadd5166" },
                { "nl", "9e4d0b239c18cd3540691893147f82429fcbc860d7a953eaa355d44265cfadc738b4987144177fc7c4b421295e09b34ab8af4104cd3ea8722b3fa868f9f0f159" },
                { "nn-NO", "b93f5f069d583dd81bfdcbaa3d679e86e0e312b774fc34baf22374a628a114826979465e6f188f996335bff72f455a915731eca7cccc00d7984ecca956e7c106" },
                { "pa-IN", "1dce03cdb2b472b1c1c2bc3aa2444161d01841bec573b0c3fcaf2756ea9eb45608c85e903c995587895206bf62e000aa5c42225cd1dade10ac28614f7d068f2e" },
                { "pl", "bd0df0cc775ddd917c3482123299d42918bdf53f7cb0d9fb492bc68828a256f02bfc0cd3112eec99c4fd80e730b04ca1af355124fe4a8e4885470b08cf66f07a" },
                { "pt-BR", "fefbfc9d641e4fd6df97ac8a67e5791c66f7c64b69a3ff4059ef3cea782331b1dfbd65b773871b287121dc9a7faa2b84934604ef6b26098169f93305011b3902" },
                { "pt-PT", "deb7013a4151a3738aedbcc9c30ca1b794827c48e396a5d3fe98b20f38a0313a5a94a57453384e9e1c857cc38051771d442c0ec7bef547e7eed38e26d18c1253" },
                { "rm", "2288f2703e0901b3f081fc4f075b235ec01aa2c4932218c1a87b70b952e5767c9d757fabcf34d8e5bcacdc9a666132dc4c2f873c0ec2e9e8f19451376c74f4fc" },
                { "ro", "355d759b79241391498e5a0f87b879fc2789543592bb88402b4737d62b08c4678dbdefe1358fb58e33e10142f74cf50070ea503d8e5ce0c641cc2098af747e1c" },
                { "ru", "964f3a74acdf88d3129a1915300275a7ab0918ded9b700ae5b4cabb7ef50eca6cb0e3dc3d1ad4026d5ca488d454e2368ce1bd18052563b1949f4248dd1a4cc27" },
                { "sk", "dd1e747571c856531ab45ce36abc03d5bfc7b9d1b0fc99f4adaeb4688f4e36151e55890067bf90a3a29a984f2da14ceaca419f4dd6eb1717aa9e136ad0824994" },
                { "sl", "823b8d00b34102092d87664c6a618e8f7e23919f8005dca8fd566d7e01ffbb58e0b89fe35ff4bbf029f9e1ee9271124429be2346f668ea2d01b765eb0a81e5dd" },
                { "sq", "900fe3fb6dca6da91eb09a7913b3ce5d62d735c64f611126e8ea495e0d8056510436f59ca63e4881d51af701a33a8425c03aa39598f241bd57aa54a80e060406" },
                { "sr", "dcce0f4461e3baed381d7a278294c6c690142d11e50399b2157a5d09bf918bcc7aed051861a611963ce118ba67e584619382564db0052b3e786d2af0cf5ef4bd" },
                { "sv-SE", "63e872b55e3dc98a505bf157f736bda97233aed63851143c8b4e5c6b7648ee5018b5362a65afbe03f6c818b03ebf9f3017e795e57398ea8e93ad58ea0153fcf9" },
                { "th", "ed36091e15155a7844a8b5e79a1351218971f21ea46049b26e6349bf7cb1fff51e08e3c49f3471cd0d23aff3d5528d429a331bb25ecd71546ba13829ec133aaa" },
                { "tr", "3d1879ecb489af9443b8a51687076002307a293366e9d4d774f617d0c758cb66d0df010cdf9c4fa3065a6c3526407427bc43bde17c9c83a8440d9cabe1999326" },
                { "uk", "e601d2b6fcebe8bc3da81b3694548a3c5f357fe3372a6969387bef0e0c0fdc60058c333ce4d2107634f537ef9d39e837bbd5209f48ef4e8c5242a901e4b9b836" },
                { "uz", "ea15c68cb3feb8b25bda00e2ae94bb580741440e24177b8635ee4eca694dec25740fbee8e217e67a6020a543e249242a7a5130fa8f2a307235ee8af20fe9d9b9" },
                { "vi", "20cadd5e9d735926bccd16e463c2cb3870a522d6c07d5f34f573236a8981031a9b1057cc2a1070b7f409d562a4f8db11fd27555ec81dd50a74e4789c48246ae7" },
                { "zh-CN", "a322193ccd6875f0d3943df3960a65e9542e951160897c1424bd596c2c2854de402f70e8f29174373ca91073ec5b8c6cb04222707dfdb38e65f0aa4ef0c3464f" },
                { "zh-TW", "275bfa1c468b0ffd7e9833b0a2d5a8e7d881e67063b3f31d379d86121b054972b7a67ff72d5f970177b8c76089202d6526bab5db2ff5fd6f4d0a03b1fe58029c" }
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
            return ["thunderbird-" + languageCode.ToLower(), "thunderbird"];
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
            return [
                matchChecksum32Bit.Value[..128],
                matchChecksum64Bit.Value[..128]
            ];
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
            return ["thunderbird"];
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
