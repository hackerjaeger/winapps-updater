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
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Extended Support Release
    /// </summary>
    public class FirefoxESR : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxESR class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxESR).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// currently known newest version
        /// </summary>
        private const string knownVersion = "128.5.1";


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox ESR software,
        /// e.g. "de" for German, "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxESR(string langCode, bool autoGetNewer)
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
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/128.5.1esr/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "fb45ad2076bb22efb8373d180f76a310c104b151e6687d81f2dd7ccfda2012d6e11c2c342de1fba98c8fb38247284559e7e23e8f68c69bb8bae8814c6476968f" },
                { "af", "0b35553a7eae8fd39f8d7713b19933723dc7dd4dd091b2ad6a09f005219b17f564292bbb981632d342418d3a8c9045f90d6eb937a7f8c46a0896bea42f029264" },
                { "an", "8e5cf75d3f00ddadfbcf4b794c53b8645644c221058047249300d026354a8e16b87016dfe383e2e97f727ca6423db291213b7b260ceaddcacc3c15170f214314" },
                { "ar", "5bbe9e4bc7e0023952fab33daee47a2118c851cc3babfebfc0aee306e751e7e0cce9de1d988a5251d57b4524f4478e0270cb0963369dcccc241fdf86116986fa" },
                { "ast", "95d313fc4b93d86c051a6782865fd5711917aab14840a8fb4933ffdd8d68a8c8effa3bd5c779058b2d969aadbcf8af230f62354f35f9b302e259c31643f1f5b3" },
                { "az", "17bd126d0e6bf57414fec357d5264c5dfab0b2fafb80ffed5b742277742d21dd9ff330ed7b79e738f9087315dac40d0ff83b0d38031b7c0c722c203a9d20fe38" },
                { "be", "557454a9d802b7288fc9353ddc04aceac73f2069fa8327d68170e8010e318ef9a20377c415468b78ce1b1baf1e781df54bcccff0a84311d5bb3d28d4e3b35ee0" },
                { "bg", "442986580e70a37732a00a348d532d1fd44a0df8c13803536a90537eede539358461ea8409905ffdd6e38c9fb1be4eb28428e7d76cc5620cc81fb4f7f1a6b86d" },
                { "bn", "4c357f9d03498dc6fdeebf0940ce87533155a9f3fa3e6f8208f979a7ed8502ff14011c5badac6a9a5d2fd5ca8cec12da6ccba61179fb232608af38c426c8e766" },
                { "br", "f7f4b87e1b757fd919e05cbf85bc7d452920676b74b0bf0f708b98ca67133e6e2429e403aa3d0b4345ab2e696a6d1484e0876e9d59f09a4ac312f49651877d4e" },
                { "bs", "2076da388dde61c041fc44a4dfb9c66495c09d7be315e27f3115c661d4fcd6cccd6e98f01d2968fc56c7b4974113e231eb02723b6c23341ed43488f071ca0a2c" },
                { "ca", "55bd7d172a228c14645b7a0d6bfdf4554e619a985d83bdebe74324dd433045f7aa04115aa82d8cee06ca7734356310c30f62cd475b0f60de5ed459debbaf4597" },
                { "cak", "d04fbfa22c14abb1b6cf3e0322f0bdc4b04c7a052f817733c53b6be48dd6d283ee843215cb09b609bf2bba48dd2a128307be4105190e52f2bb6709580d5714f3" },
                { "cs", "cdca32db2a33882c2672ecb001b1d4b96c31d703c09708540d6cc2e292b268d05294cc320b9cb7a9d7f20415bb24c8a9dc87b39c586acfd8bbcc0e24021627e9" },
                { "cy", "463536961103a1d0898ef3d0056ae175fa031a646b3ce70efab0a9c829a4722cbaa9d41decd4161c715295fc53862e5eb813b24dc9a86db1143f1e87a0cf3a7d" },
                { "da", "f504593e1d8ff9b22c3da9aa741a7c835ee2edbf8607d7f790dedd5429ddfbc34b09d350a5090dcada2aab0d770c0b4bc1df77646458ff3275f49a8feeee2092" },
                { "de", "abe43d86b3aacf9541a6f7f8ceb25913ad4207e0ab9ef2d8b03fbfcdf1eb14c9537e39da4fba19a393abc823d50fbbb8e15689ffc460a7b6b2a642fa7582cc48" },
                { "dsb", "2fa9352c1213ea464b46b1e2d10594ed74af8fe9d564a512e6250402357903937f0b755e56d28bcef21ee9a58c0d502cbe57f51b3820d0a76ee0ea0436672c71" },
                { "el", "be4ce5c3fa611bc4163217b3fa54c3e370593eaa5a512c80ce839213a8ef2d615ba6bdc11fb79b2bc254bbfd18201e7a63ba7a345ff7c3e09d81489f1b20a01a" },
                { "en-CA", "c2539180acbbb75c66b30db541ae294e5b343d48c1853920ae2d69aa9fc8b869b7c76d302290fcd94478a38c792f3e6200858fcf9a336b720440d4343b9effd8" },
                { "en-GB", "16455f5eca6f82a69a178477fb6baa5c8c297ecf27ac25873ad18fb15c7e934ac235d5ee1923e7f81d5f16c7540afb79915542644111649fe43969c1c11c7659" },
                { "en-US", "09e6184926319f27ba95b02a2226972659e6af0af6531648c44bfa0b15bd9d105a13959949cfe9da0afed9e0a3782553261e0d12db9255f1e40e8f8bdd35bda9" },
                { "eo", "9c560f3c2bb6141e5dcb32b6955d3b749718e64a53ccf4474eb664708950800eaf9e45f6fe0202fd2d2896cd113bcad67df7db35006df1a6088f5074466176ad" },
                { "es-AR", "7af0a8ab7c580d8d3262386d3f1cbad8a9f4b7453953b007a664f812dce3907922c806d0fe84edcc81f68d959a20475b670a786998afecbd2816a6975e53b7a3" },
                { "es-CL", "fcd746ef8a389de6f49aab8aa5feb3c1c6ea3084baf9b947e429a955f8ab2ad211aa0db436d8becebc53728152ee170460baeb531e3464ef1796535d1cfc9b5c" },
                { "es-ES", "7604f132052c4d322a46a07411281e1b4fe0dc81bf601926de9e7249e2881f5407d28b848b8085d1acaed9bfc89afccc07d17b6a360d57743d8e35da25a0aa96" },
                { "es-MX", "471f7eaf6c6c0fc9740e7a3a2758a64aa7455916bbaf1da5833f4f2867bfeb1cf9963896e8a49e9bbe6aa846c6413ac795f5c0f97ca0a8bd71d47611fcfe546a" },
                { "et", "acd653a9a806a664f07c30ac5c0f4bf6a99ed1dbf8ef5ecd547bfd9e81a560e6c9f2d33ca9e6466da866a4b9fa8a0b47d931b4bbbef915d802a5593f34cc792b" },
                { "eu", "e6e841f48fd4f65c2f70772c2cb7dca5b86c2831f5b687548492d842a2c1d11e9b5829b1b73c3a6c8b99bbd3334720495386a1c3e1e4774f1888b7c60e67f394" },
                { "fa", "191c50c56559e1edea608fde76a4963916d64241c6a87b842b980629b47dd2cf712f970d7187c5c8ad10de25a6eeab7d3e7432945695f98bf4f411427dc4a52b" },
                { "ff", "56fce88c0b08e0a4e25694afd5bb96fe3a706519dbb179ae94b090d41f3420af0728701dd79780e902d7d2444cb84a0732df15cf3bd0c314d31aaea4b30d0585" },
                { "fi", "c2bb01d94b3a58868d41164f8661c88357ec1a3f21caf0b503b073bea581d34633e8d698a6ea215c5278828f30c3e3cb66e95367fea6cf89995eb4a4fe1f7604" },
                { "fr", "3c4079cc49fb505b8bfe34b1fc64019c7d0effd27a315e47a5f0c49f90abc03fd7951ac12cf704199c86c8b152c67fc91e0808acadff0d818b219c88d7da96d4" },
                { "fur", "6f55d74d13c2e410db8691fab312384620ac2e2013021af2a9408f816d7b9e1778965e26ad2235a37b3d714851967c9cfdc7ffa099507a6a6b52076bca50026b" },
                { "fy-NL", "c1ced3a957b47a80f252999082e9deb2c91c3f179f16a812d9631ac0b5ebc0cafde58d0eca836cdafc37583157b68153ee0df7d13d6aea081bbf84d9a839e723" },
                { "ga-IE", "c4dc79963f4a68c1632d2b0e17d416bf447fea42c25e234c938eee6bbe9cf98e164ada259dfa261950fc3545021b497a532fa69c3cb369e9cc8e63f245d18789" },
                { "gd", "4befb9cb9550bd7767619bc24a1a4f3ab4858655bf40a98e56c197c39e2a0eb135ad616fc1e9064a8a2459636bcd9e39e47206f325533bffa2cb932ce96a3a6a" },
                { "gl", "2e5a9c4eaf8b7f90ec47d8a34004055c6452bf5ba9d378c31ffd27a7a5fc56bfb4044397a6c32f1cdf0ad127e4ff54e20d20d59a77b5a226f262d7542aecc144" },
                { "gn", "12813b3b0cbb822045308358c1793ad79aa23d376641ab811bd12dfaf7308679a7092569d996124d1336974a129eadcf8bd34006aabbda1f1a905284fa82ddab" },
                { "gu-IN", "828ce52950f0f1a320f95c6a0c473fad7be3502f5690bb9a789e0df75ea6c49db1f1fbcd71c7150c676a894a5be6229f4d42d49a79e731c5d47e98c6ed3ea072" },
                { "he", "2a293633413c896e03c7210e35b75968bb69acce37dc5baab269a3dd42f822781fb311d9bd4b44ac37e9c9f0067e72ee8bde2996b90678ffa365a3301e28fe0b" },
                { "hi-IN", "66a7b4a566c6c5976088d85b00e72dc75ecff2635bc075fe0c9ba7508457690289fc48455426152fe929fea02d9bf1c916953a337e0cec595f458bde9b14d5f3" },
                { "hr", "1d66e1ed4849de1835023fe49e6067edeb614a9887dd916e6ac0744a0254aacf775d1ddd3fa8c7922ae8b03819fe1527f2756cd08a8f54789c4acab767c2df83" },
                { "hsb", "4cb8f36e311fb809faeaf7605b83a8153f4fdbef91c6620fc961ca5c29ae6eacd8e4f25c494bbccec160d45cb148588f399b15a8d18d9d98a0d2fa9c3cce947d" },
                { "hu", "a2db0a407163028c7731af47a344a8135aaf4da7d91ad44200dc6d73ed8c955aa23ff16d9edac71bac96226dc0c7cc9ad5292f965af9e9822004bb848f884e15" },
                { "hy-AM", "2d90b742632dba5e5c096a778578485f7640bed787663afc3b5927a8be9e5834c265acdcfc0b932638b0c9d13da8cd6dc86cd8f4a2c33595e2f66bee7c385d99" },
                { "ia", "721d2ae9bd4cf77eb368a9ed36bbe31570f185dc15f8f703c1881866eb53dbd26e1fb3b544b5b90605e6129d677af410d5806b921259a273195bdbaf6013a76b" },
                { "id", "7703bae4ca78267eebb9d7fc5c0e90511afd81fb0d059b32eecee03a7c80848faadd22cd178f33b5361f39cf770a435f78994167f0ceeae7f9336166a647bd62" },
                { "is", "a5c8a52f4fcc43b2f17d4cd0a815cf6389eb23b573183b7357e51a6da916be571b0cf3c2a79825e8c466d376233e8240149dfadd0b1ff6d2cfc79243aa848ef3" },
                { "it", "9314b524e9d634664d9154d93f678f834899d08908379770e7a0c0d6f42f0d80533238df353db956f986785aa77a5bb7e96d6a0bb921dcc791d727829520f103" },
                { "ja", "4e55ee8942b11b0f0410e20d7ce4797993791ffbb668005ed394c01018f9d3c770d8e265bc1f0b80a0b820d43fb27d25cd4fb4e1a7bb12c54bbeac6780341786" },
                { "ka", "833278c84ed584cfc722f11b0eb20b7881cd630ade860dc00c56e1faca336120b89d9d261022460e6cbc8e792874b098f1629a76f38c54ea1524a7e9c16410f9" },
                { "kab", "300610def0fd100d3f67227f667f6b3d1305e3b2bea8b2e1507cd8c4d44175648b34bc200ee6f87255a486c6885b99e32bd363365a05db9d4fc4a1aa2da86e80" },
                { "kk", "264e12539dae32ff9f9356421cf4b2f3decb2a96f0b7a75a59fc2b72b2178379860764e2ec6e5c6a29e233a5be01ff48385ec822fd5089e6fdfa7eb8573d4c5f" },
                { "km", "26c872dd950a0a2656a458bac97a2097f553f3ddb1dd994d42a3407b22015eb8980743efd7d5dfb28f0b602c8fdd1c872bd263d5ddc114a15370f49e9783f1e6" },
                { "kn", "a8f2c29e29257da31605a6eafd40312768f55216b825e5d667d68860df525e13dd38110239cff27206b61bd2a8b2d6fd9f8b982db61e0c476d56d6e513ff262d" },
                { "ko", "2d65c772becb1940b1c798fbfe41da80eaab91cab9d08c16a4c1b30ab10b7410576739dd61abdc8af23d4d27062be387ea0cc17dfd38b1c365e0b81bbac862fe" },
                { "lij", "110172a87d874b813fb591e9b24f3fb3bff26884ec12b13693f340b97d92279b2614a4d748ce241448815ed0a1a624542adf9e6ccb60dbc4434af4150cce9af6" },
                { "lt", "56bf1fd43fa29d109b709be54f4937603f1a0ecb14c94c26644dfb010e865a140e4c1d9883bb349b2885d0ae7ead80cc09070a787bf6ae765709d6f0343546ef" },
                { "lv", "53bafeee1cf8cc7a7fd4eeff8a777e75e1d8fb70a92511d8f04972c220aa865ea93bd08397bd91c4669e4dd4450794cac08433ee7b3107387285e15fdbf3a046" },
                { "mk", "76ed97e4ce09680648ded8fd2729d93c58869bf12211437004ad6fbe6e55d2135a21ac0a207aadef43eb1d56eaad6a14f4dd40fd1fa51956f45b41d1a8140152" },
                { "mr", "1dc31d39b79c1ba5b99d9533ee8d6ed27952e053c9ded164809482597bec1c9bcb9d6da205b3a90e43ffe0e42fd804fa9be3b84c422ed916dab4f54e525c1f8a" },
                { "ms", "c67e53c1145a0e21bda823889f598d6f1f6a48b18d05d8170766680dbd96a50605e449f193d4c43385e8d1fe6195a094ed9a3cfe7817df14b5de804f0827d7b3" },
                { "my", "b98f7a063440d28a7be03df5bfa9c4e7f9a66d14bdf12a88ea5132a249f8568770980dcc6bfe7907b7732a49414a142dc5d4f38b84e96b45ac6702e74b82249c" },
                { "nb-NO", "959b84c454c13a8fd7f0bd75f1ad68fe76c25cd57485852c8c0d2562d741aa1ae3125dcea1a9b2da4c75fa070352031280bd30f9f78e343cc87f897db6839638" },
                { "ne-NP", "35cb7ee21bdd523706f78291e7c4a5caf1001fe83a6bf7d15353b79be329043b6b1c3920ad296e47e1ca457c0c855e90f711a60179bcb9e15e57ae131ec1cf70" },
                { "nl", "ca00504cb368502f33350215cc4e517baa7d4cc4a4dd3a8f23417576d60620611b21b4226beff6cdedb23195ed52d059bed0f2191486d5a3401d3f6faa6c72a0" },
                { "nn-NO", "41405dccec1adfafe8e5ef7017f136e866c17d2dba00f81d661c7991933342ec454bdf30cd049969d8307449eefb5f4d7ed946712c0b18b6ec0f3ba54f7cf19c" },
                { "oc", "15abeed5b5ae5e04dd9d67be22d59d57bd4577943dc31365bb9e1ce6ffde946a9c0df2fc04e769522de02e6dd1349352b23ed8b14052776fa222a61d950dbbfa" },
                { "pa-IN", "36401380d39d7479db01b57adfcf2e9f509747a63c06666514f7ec10b6034a600461cbacb9bb3f7705e538eeb01899ee39cbe3da1024d800603c1a1487d09f41" },
                { "pl", "83f0e05ca8c17ccf2517afa2958cc7f04c5bd0aa99a99ac186dff9722ca82ffa5f754b40acb8f091a526a6e8410660490789f782fa925e172fa0c8c7946a45a9" },
                { "pt-BR", "1ef6f36dc6bb3a783001945cd2f24b1ee45a658bd2b2c27a15bb5083803da8c7a310af780d5ab53606330e97b03e5ed05465eca99ead10242717a0b9d81a1d0e" },
                { "pt-PT", "90c6c6d044f5c1751e11560a67e145d9a5f9c6c499c8452ce235e01b0b0c4f394527ccc663a025538011e58185f42408c60009cfac45138bc424b6a90ec03738" },
                { "rm", "080111eca2cd8ad5839c355940b61b29f7339018646c230bbd3f5885aa48cf2a01e67ae45cfda266043798927daab324d3ebe8182c5d8e4403ac48a57cc86a74" },
                { "ro", "2cf681afb060c17d64f3d7d055051f1ef59b2d46c4c9d787288a82e2c8247b95c7d9c236e55cc3a5eddbe25dd199af4f77b19991e8bf3dec7a0a2d273736d143" },
                { "ru", "0983ced91037c042e00be3a3b2e5978ca46ee73f91719059fb3017900eaf21cc17cbbddc10c946aeb7daf73d9619634cbe03c0eb720e2f2794d54c74d5f1c736" },
                { "sat", "74bc49a996c370b481be4f62bcc020df06adcba104f3e3db9f8c4455f4f256733a993b2ad52763e3e4bf506722964513a956040e689955d8b1265b84a1e5e155" },
                { "sc", "d6a3901b0458482261b3208dc443838a9824e6f111ef933bd5a6cc549fb7cd20ed60c0c338600b7ff7a595d09d47304c76535588082a359bf2e33d2263742c8a" },
                { "sco", "469ed06cbf3a3211b8f3c0fc05ac6318581fec573e29d6dac9faf770bc15637d6e8ac80a30e1ac6eb7885420bb016db23d789da180c14da2f890b6aafb9ccebf" },
                { "si", "3e3341a81428ec33bd9a2efa3e244e0ff7039b9775318600699ff9b0d3b7a200ea29fe048f41854d5008e490f640cf4714f17e80710ba223a287de6aacb14601" },
                { "sk", "e86530a4b22c36b31580ae93f8f4a10276b43dd270d2121056496c97904b3a0f779395e1aef2b02843f42c0bd849eb3241e3fe17325e4defa4a45909f45d4859" },
                { "skr", "733c4fb7b75ffcb4511d3c872df1e7697309d6cff37fa278a935a8daf438c7468f7ee85c2556cc7f0b2c25ddfc07d20c784fd38012660eea6da84bb87379064b" },
                { "sl", "d84364e3f740bb0ae4b16c458cd433db7b7ca4c300ee5b4615f51069403cee8c5423e2eff6b5d1d0c327c2d38061da4b881fc62518310d07dc27253d05e77825" },
                { "son", "093360d0cc2334244ec41d7b017c5b658a5b0e128c35ac33a6083cc47823fcf8e98e6150f8587c1eba22c913bb864e3c6a6642bd13f4fe2331276ee3f1e89f73" },
                { "sq", "6f38685886ebc392ed3f5196aafc734ea133346f7106947440f0b1a38087a11b67142d46f7102fde03482c35215056466653d498aa756dbdca96ca549e4fa055" },
                { "sr", "228b8c250f7cf0ba138a4b1717c8f55b72dab9d12a5b184462a46318335dc6efc68825b247ff8fbaa33e3abe1c419833fe380defab5f3971d183a9189aabc991" },
                { "sv-SE", "be3af8a2aaf36ffaba2251828b4038d44357eced5ee391a6bb2fbf7bca370297dcee35eb88a1d5fea5cf29da74c1c3d6d9c03458b7eb13b67b4c0f4aefc3b234" },
                { "szl", "da4e3e93521cd14826c968f7795f0decea6f119359b211d19988dd94bf53f3ec396017a76c831ae457e5c36e7c75715ac023947dac017dd5294c42681476ceef" },
                { "ta", "c8b74ac81f39a00b0840a833f9d8bd22ab82898c53045d165abe6cb0dbc811f1763d7e06ca3b0e4a71002d485076daf04809cbf83f32c78649f511a8df330c15" },
                { "te", "b1c5e4a84c0a52e9d285cfd7d14d9c6d077b31c7b27383a8ffa61a97be79793f92480c9ce346c1cd2c38eae3e09aedf60882edfd3d0a0d83c2e7219b150eb0ea" },
                { "tg", "da744ea19cfe85fcc24b64480693d15b3746bb6c17b295507a117423e20bf96b46d6e9fee0673f40e00826841f1b41899df5de846639b3221b5fd481ce72e5a3" },
                { "th", "7385af9d9f7c38b1ba7e667dbde9f6276a898caea7433a70ac492a489b05dc4447a71b16a6a8fd1221535a5e11d38279058a5f24714dc0c497f154ba56c6fcbb" },
                { "tl", "45c02e3234a78e3d9269918957179a11f6d1b734026e467eaac7cf89bf58bb87d57aabe53a2974cf02b76cab63dffa78ed03388a4a9618db1bc473691f8e55e7" },
                { "tr", "b46996bd1ac8c48f11350d3260ef6d04ba1eb4d43820c6fa8f8cb88995a983351593e69e98b2666ce1d9857f0f94a2d27530d013ee8f6aba558d1c579981a401" },
                { "trs", "bf1b77667cf8c1369a1ec93b6d9a6e459feea8699e947276a1f801d5ab2ed7df01277fee98a35e66df9ec4d21080492042736b5d7deae0a4facb5020e2f7f88a" },
                { "uk", "894cccc776b71adb4aa00eaae9bddf9345345262ca4eb793379c97624e9b461cf93854d934792896f2eeac556e7f86b6e0866d7fb4d2618c6e5f9bcd8781f4b2" },
                { "ur", "2942af48b631e1405ca7cf93fee4fab9472ff6e148bb1dc0ca9e96f6407e8a4ccaffc8e26e1f456ba70130fbeec89708181fb328c8ed4575b581e2992fec1535" },
                { "uz", "594ab57c3bdf920946d4b507779086913ca7e284e53db2b01fbf4c0762cd4ef85b237b2c4c179299691c0e9533e9b68b9d63a9c2709b65d1fb8bd28fd6ab74a4" },
                { "vi", "9d3384176073a2ba2995f8c5f79263d065764a0b4e0cd9f97a2209bd48cb98df4e93c25dc27069ddb36c66090122f0c1b55a7c2183ff6769493c0ce3b3dd379c" },
                { "xh", "73f5eb4ba31cac3840334851eb1c18863e07aaa382bcf4d402887c040d7677425dd5fa783a9f056989377a3a5b31b5fce082f291b8c00e100cf27c2a5c1d4945" },
                { "zh-CN", "68d0a14ba74513b9d14efa1d61ff789b648951df936177f8e390f8f387cec1b8ded8071c365c7dd5d63d7d0b652ec8833f21833aeac4ac1ab8b1a0052e52bfdf" },
                { "zh-TW", "84ec87d7d8b8fed1dee38c23a64d6849377a107b8f16e457f10b856b1c7bb6fefc676e2a5b6de54b610c0bd0d5e9f705a31878c90bfaf33c32bb64c1c6518c79" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/128.5.1esr/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "5176f8fd0f5f48fe44d9461389697649ac6b62a195df7482bbfcafa35b25fc6a73ae4e89f5e2c02dacb42cd9e1db2b0f680a9480ee2bb50c943436bfe8ae2d93" },
                { "af", "ad41eff4c6f003a6a8b4c3ab51471de01ed4c9009274a75f8263b0d0033fb411f79f3e49fbef9158216325f61c38cdb04d520e8999f92acca45db6bb065eb086" },
                { "an", "0573a40579b26bd87984f1802cabed166c32388f475784a6cf8adb4107f3690cf8477dc2f717a5887d59d62fa740fbf2ab7826bae8377e3a8233317ca0344dc8" },
                { "ar", "ddb305fca58d615e9276427d96f7d31a2f6d9b9e5d57a3272b3d382a3612e04826480606d51460713e740a1c8307717e128c8829eeab6e980e097a681680b4ff" },
                { "ast", "98013335f12f6540ff931fde312158365bc35b58cd8b8baa48d0e8e038189804370eeeff9a0eeaf2e4952dad73c6b42fd7aff921af9f52bfecb51c237cd5764f" },
                { "az", "595cba331f3ae301bece9c7a775b6548a1da668033de94375a1f471f93ce64edc3a97d91c0fd3a71a41485cdc06c8d61fea3d9accffc536a501db814161e8a91" },
                { "be", "8ff3f45eb07c78cd4a78ab29512f83519a20aae85fce01fbc9b9c79acbfd82f12700f8f88b0a795eeed87f5e608de35e307b0cb0e4ac8b78ade49d0092abde8e" },
                { "bg", "4c094fffe36c0e1cff5f77b0fa6ad8f21ade529c59a4ed38eaebe2a8c29629f1f4955fe3a157f922b764955c2246d3c5dfe4593623ac8413bf0130a8e18d9d46" },
                { "bn", "c1995308ae90a78ff5fae0773991fa7b7915e4b125778f768015838ea9d8811b8da1c73b6035b18f9e800baf2389201b67bea0a909a71f558331553e41887214" },
                { "br", "fc905820b8906f055bffba3cc98978e06169d201731728059e751ed6c70c874325d7ee7b268b41185c76ee2e640f1301edac976c200b95450c7c0c421529b902" },
                { "bs", "1a193a4319e4bd72e71d2cbef4749cc1326ab0c8eab1c1ad143927635f69f0b4a8c668a6330edd852af979978df01d3ba94e9fc0f3e162a23710c311663f7e67" },
                { "ca", "1f0629c0287be9a0c3b31fa7f55422bc2411a0fb889ad80e725f728eb8a4ec7a2e4030a2e8fe872b7a9ab7dcf701d5445289b4ac3a98f5d090ef3d24aca08909" },
                { "cak", "f0daea5927d89c0dd004788f4a871467d889ca332c1490ad920b4fec745776a71caad06c5e48c143d620ef22f0e8c018817f168ef3445b977c3f44b162b244a1" },
                { "cs", "508d6e70df9a34c7061a5540b45d01fd9c5129e97d04f4737f17c03960e4900fc3d1c4193403a5082419eac1c9a9370cc7b6b29bac08ed4be2c88de14099082e" },
                { "cy", "3d44cbd8e80b2110db06059fd3157ff31da61c89be3795fea7239c3849b6ee84813128740c6c1d0456292a451595d8d46a77b88b1251f0df45e3807f1283cdee" },
                { "da", "bbb84f3073b577caf44921581741acdcb2595c2779fb2bb3dce68f9e6016b2da870323bd5aac31d0ff3073fef7056720f134a73efbd2edd2463e859bea665aca" },
                { "de", "67da41dc3cf979b4a6a29cb70a8d8d022e603e427b7513af62b9dd7553694fdff8058f92cca9626d62b74b16d2013c602c14cf6c1793605f2534d3aa6344cd27" },
                { "dsb", "d78df0bce2e8aeea3805780a79a3595d04d138a8f4f7ee03ab56ed7e32c6f8e37987c0af4fcbffd5d869d56e1fe71fbfa5a28cb4b13b499b6aee4f9b7cad85b4" },
                { "el", "afa6844974bb7477563045790ed42155f0a8ddefbe6cfff5756dcd07bcd308d3c9f553cdaa6af384f374bde220eb1ae851f7bec74fa4ed21badf8f62803df941" },
                { "en-CA", "0fa3d87031317ef783455125fba6e1cb52f8443fa89b1b1f5f297f8f98860c102aae688cb76c15c88cf3b41252c90180785d010c7d0baddf956b750dd0fda008" },
                { "en-GB", "280024b33e5dac7acceec63c2a75aca12dbf98556858cae71c6e18da281642c1ea2e1365210daa2cacf2ceb27fe62c58ee4893fddf870f44c44700eb671e4b2c" },
                { "en-US", "1b80b9650fbcaa3d029850abbd834acdc0eb10b66c5a73edbdcf542d7dfbc3226e23760a869fd03f7b2b100000fb222cea423771bb5eb27230595f082480717b" },
                { "eo", "1308118752a3ba72b6b8cc718085875caed3c215a441f9fed59e2f05e91f4d824629984a2b78af7bb038e26f796c0242cf170c8080a73624e3c8f202b143f843" },
                { "es-AR", "397c4e12fc569fcd3c3a183d46cf4e7adfa85c8ba8f2ea7e9e2aad5530a63e1027566521e569e6ac56b9a0a4d276c6ede8500d83e87310363c8e6e19f2da7620" },
                { "es-CL", "b3bd18532e793fade3316e985ed55695fe0ce60f83be54d6e7e9308cdb8298f042a6303b3bbabec75eab51c1b1d75ef43cf353e5ba4793a56fd68508c7b91974" },
                { "es-ES", "79e3ca40a60163b3587512b7787fe7984095a8cb4b06b99c3935d43c94ef0ad424e203106a9e6dff8c69dd020daf92918ad4325c215ede7f263e7be3190710f9" },
                { "es-MX", "a44a35f9f483d87de40864bbd133bfaf1694076dc42c8fdd4a94771342df94ad05c895e8b5871213c948a816c12e0535acf178888268888a94352fc85481b1e4" },
                { "et", "3bb71a4800cb5ee9c4f1bf44b901fd8f49fed1fbac5a8677ebf6fad1e5221e79cdc7b09af4b4c10ded398d25eab3bb3e0db898327b179541f7ebdf1ae3495806" },
                { "eu", "fa74f6bac9e624d3d20cffb4a52fe4461d5464649bd502d2b6440c572e6470ace81991b88debb6eb064e479162ba5e9448845f77b0925c4d01e102ab1b5798b9" },
                { "fa", "0df65cf2fb252222055dd9b6c240df5bd1e9ad96a62f4ca5eab983e8765170e469f67b49cf94a8c5b6922ae7aadb1d65e511580df460a914da884c4a3babe2da" },
                { "ff", "ba756c8d5021c0e0eeac3b6a046a0bd931601acffd2e280513587453a8525e2fcba46fa7608c8683f1cd88b8ff2b1008bea6198e86b90e4072b0924b74617a26" },
                { "fi", "3b59e707ea4928428a8a3b8429f6f383d649c0d3100facc827eb283051a9a286ebbeaeb8283b56370231a83c0b6c65123f7ef377c878b4f52b0bbc0d932dd9d7" },
                { "fr", "ba26438cc7d1bf48ae303f7521e232649b5ac5765a5faafaf3f1c2735f6b45462a7fedbb46587b9c47fd6482291dcc20cadbcb448dc67200f51de2eb3036d0b8" },
                { "fur", "201c4d95fac2cd1c6f99a1bb9d84a0ec5b635fde259f9913897ef0267868e46c953712ac9053beb689f7bbc707e5ae602ab939f3be712bc20cf155a3edaaff08" },
                { "fy-NL", "415364fdf9dd374dab5db025a0aa3e3f1755e3889a232809c588e6f13f33a8cf15e5ac55a7958c78b28f5d1d8a71fb581f03bbdd6808f706b67ee6b678ff35b2" },
                { "ga-IE", "0c618697b1e5ef21a3043cddcaf39e6a8e7d64b8453f13a2b193d8f425db00eff570240761ed6f7a71b28b055f9ea084f7275bba0806878340e14acc183d8df2" },
                { "gd", "d3e54be410af7e2c937c032d83bd6dc3dc8c871a3aa07cc8ed16890cda273425ac4593fae088898619d42c04958421c6fe4e13956838c9595e3ad411c5234689" },
                { "gl", "d44a630c194991c7b92b87c3c75c0a3bb170f3b64dece5581b0c9a1a8c62c09763fd3460c9168217d7fae4ad71dc90b5a6de806ef1f206f0ee09c1ac0d65e557" },
                { "gn", "aec6ca6288d1a10e696fa7f306b92e94f2c3f371ea34ac882297792a6a64e00982fdf80eff01ae84477a0a9391c5f7e0a33eb6322d26f6996de605779af57de9" },
                { "gu-IN", "8ae26c0e0853482b6105e6d0cb7b4469a43b957b7751620f1ed305cc77fbdc549a6a9f7bb32cd48952cbb496b605c8f8f5604e64c8f3f3e5cf15123f36b83ae7" },
                { "he", "7ffb1677ecebdd6e3d033539d0dd5faeb4ce8dfa1cab06d5798baab86e35897ef087e18d86f35ed837ac1bba4ad24b949e09d3bac8634665a0062106459e9a1d" },
                { "hi-IN", "f4ae7922c98f0897c102b8c5823f7436c1557fabd0ce4727925ab6925929966c5607add995f9ecbcdbc7f4c36d902c7e1c7533698f27b0e1b11db129e6db8841" },
                { "hr", "4719e16b202cbf41e89bc4fd96439aa172b9b19e83a701aa51ebbe53ad7db7b8551267bf731b1a83e61d7f37e775508819c57dd1cd7b77fba5f7abdae34f07a8" },
                { "hsb", "5a5e8e7500cbd27d411fe411f133fabed7faff15281466cefe35d074f65da0308a4d13bccb1ce98a8b8c7df58adeebcaa241908c7386b1341edb0843ac138c71" },
                { "hu", "8acbba7bfa33824d04d7c006b190f4bd51897cb2013b1d9349b811d0ac710e6a61e00781149b2a8ffbf93ebc4942f860406b3fbf0ec9712b92439de52542fcc4" },
                { "hy-AM", "e3b64d6209474c3b1295f6ac0706b2088081af491b8d78e42bae589588610566bf8e8ee2636dd7ed5e009e3c83c398cad835e324118e9233d99abec1ff354b15" },
                { "ia", "b11ddc208073208989581485c42fb22191c323b205fdd79f29b6a4eab3e5b01cc87941d4df14d46cf872b42ca3093df092cb9223396af56564a3a96649ca3eee" },
                { "id", "370e29c7ae43f19bc9ef40dc3ce2698397202e64bcd2c1c534cf59b84bc53bb4c88507d82e88bbb47ff1ba94b08fc4a5492623795b6ce73c6bd7ee382f51d6c1" },
                { "is", "486771f4e4be0274e3fab02e4029110f7675260918c885512f7921e45def12a45be6c3943870ffbc7dc732e2a1593f3a35a103b21cd753766f0960854b9556c3" },
                { "it", "b3b6154d55f4a4731b4de34cf4e44cdc7323fe950a5d701ad905be2ccb0382346e955a20f46d10209eeee530fade233ae0ce65eb2f8e6c29ddede83b8e7599eb" },
                { "ja", "2b1d23334087ea7363eb01e72f4216bb7960a271a687f694fb1c634dd1b7f0a0aa505a6551f438c1a61151c12d9870e6eb43b18cc262e011317906373e6af992" },
                { "ka", "4aa80131048893c8e4564887dddfc85899820ed0ac9db6ea1958087cd5fb4cc37ed95b23da41f3aed28595660f8d1a26f7a8400d76d92ce07f900eb661ecc17e" },
                { "kab", "237c12b7692ac14388d6289c28f1ecb68bfdd1d80f1d6bf1541362781a4a5a5a8e8d43a11fa7ba8a5fdfea0d8c6cc6d6df4c9fbad159b8204b4fc6d59d67ead2" },
                { "kk", "28266ee666c470faca5ce94e20033a153f5b02625bc6a1efd8ad314cbcf5de03e49a08471e1ee26153c90338832af4d1e4a2df4ee3d430e1282ce7adc0d8c5ef" },
                { "km", "b080d1e0785bc574e8049be4530932181cd5be1641eddd1f907f29f765473e5062ba99104748430b03352246ea3207324981143742daf21021d7ff3cd5dfd9da" },
                { "kn", "13ddad509c57ecc9bc95582772c4d7dd26699f064a3db3aa13c900d27b9a0dd626b1024887ea96c9b81a20d2ef7b4830dc4dcc62ddafc3f89ece3af3f35a5296" },
                { "ko", "23ef053c81d92fc7baa6095bc37a770dd2d473767bd7ce77e060093f958e2121c959c7e22ffa0cc1dad124aea96a4c90a177632b001602fc5e1fa487ac6fbec8" },
                { "lij", "903a60934b865e12dab18dd92d410382a5ba12d3fcd744508134afd2a3f7ae15183f8ad004ef71fc098ce88905d6a544897f544c2f15bc7ebc4eb1cf53cb5e3b" },
                { "lt", "cc4dd09bffa951082b1cf83bfa8afba2d374414d53bb882f3d51572306c391e02afd763c3b2f0f9a0b1d0c8ad622d3d1f8893d6b0a7632ed5a6c50b4c3a10aa8" },
                { "lv", "fb35793c289af237d528d63975050ada8f070c8899f9771944f061152bf15b69a1cf94763bef21b502e4a9f223fb9d04c46606cfc05ed77b7ac7fb39c5090ead" },
                { "mk", "3a9574698b4434a8b01389b105521abcce32deb1a0eb54b3cec6cfa8fce1ccd87125e6939bc25b3658d65949f86c962b96394ac38e81660dc14ef2eb8c57d221" },
                { "mr", "5fd859289d7956af39664d417d55180702e0fff78284aca79fa8ac17e155fa0593f7cc6508fd9fda0d64c159d382d8adb7e4bb97b3c8981c8cb9713e822aa711" },
                { "ms", "2288993a528ae002a310d7bb4a44adb6c8a2cc06d2b6d19ea063eb096a20e2f43364bdf5c361330efdb577309738d1ab5488fc75a0b8956fff1d81a9da1c2b05" },
                { "my", "5023a278073e68bbf3b3d59f118732046043b83f195c5fdaeb7f020c5f023a29319706157606968ae1279b3550ab77f49a3d5a49b7529fc7a666b9f8a0deec86" },
                { "nb-NO", "9cf0eb050be79fcf16babf59e1f500829fe5b76d58491cc6847fa97a82627d8aecd64c36ad6fbf081d86b79aaad1585b6cf48637a1abd476d4d06bbb9e117cd3" },
                { "ne-NP", "37286791cab33cddeb8dd50919388a9ebe2a912c5fd54aeb10be54db0444242490d32da4992c991c1102a2cff8c90b6a18c3fe506edb86c0a6140b378203e10a" },
                { "nl", "ece9c33ef8dd75500773baa19885736bf63c90b1f83742a81b303bc1f9a880a3040ab463bb68ea047b61599ed42b98eae9b65fb05d3b7b07afaeb25eaba96ee9" },
                { "nn-NO", "c0b9f7270de4b017c97611eb0a067c1667fb87430634abcc12f2e826baab4423aa39ec17e211f7d7adf4e78f119b7c576f1b0df27666cc8d692923bb3fbe068c" },
                { "oc", "015032cc975fea7903de55e67e99f6d013e100e517cb9a94130d36bdfbecf8e033b1aa0dc52866235df63c389f08cc13c796f7d33081e9c7facbed3e5df10707" },
                { "pa-IN", "493ba0036b7c597f56cc625107a27a0bf4fd837b7a7bff773213729e9d71dbe2555922208a46771dede9593541f5ce6ae2310f60a22c0cf02fa2a270eacd3c26" },
                { "pl", "28123b41db2c15c8ad8cbe7092288a74c9e164fb05ebc8f9ac0ba22e6858b0bbd3561adb3947fa7f22a71331f7cacbd4adfe53ffda332922f0e8fcc2464d0fcc" },
                { "pt-BR", "a0c08f31096def2cbe18a5a2ef962ff5097385c307461321d7311a7f68e03fabd3a0dfe5dafac363018f22614e42ef7fb813cb4ce33896c6d7a0cce4ab5dd950" },
                { "pt-PT", "3d2d60337bf74fd6a2181309885c62ee098fc8155f689ab6d942d84ebbcc436cbc1d52252c2d6fdf0d8c00adb5ea8fd0068dbaf9c4b5dc241b52fcc3afda1d43" },
                { "rm", "1afe7f8d88fda680d96e904c074e77765e092d6111c86119f0fc4cf519918ab7f384562ff34a586bf0aac953667a464ff1e4b9162a8ab3f1104ff3da4b63a4f6" },
                { "ro", "9858f4b4cbc8d197033765be04877c13a2ba44f09a8fd3302de5420431c6865d899acf54a43f050b1ae1e2d4227464269d8317e94fefef46af9d14410b6cb20c" },
                { "ru", "c93f0e00e2890b6bf6228f4033b78f6a145bec0546a507d4bc20517199eeef944b06324a46cbd1bba123c9e1f1625cbc945123c3b65345093b05b17cab751261" },
                { "sat", "27664c9f9ab9b3859c0cd2324b42f2359ab6cb40ab4651d5e9a402c7c9d55671648a3305b82814ed19b1dd773d285526baf17fe1f69bfd45b783f4bfbb2f3f46" },
                { "sc", "192cdd5bf9684c0440c97739f21b0e0e079f1c95fd44c1acd45d7947b74e16d9a7c907f21d3e1d8e323291e8e8ee31f2698ae0145bdbee3056f8e2dd6d7b8b83" },
                { "sco", "6687af52f62bd8d623a2ea3a621e40d2fd1809291346d979f1f7534f2643f484d36aeef927a2c3c1c03f5371fb144aca2a834afc6cc5fdb57586bb820b67bca5" },
                { "si", "270c01ab101a9207fe64e28e927457c3630377ee8bf9c8af8c7f239d781aa65839dcf45485ad8b8c0a8c49724a7ed8dae10f41f237b83712394ad4cdb5f85489" },
                { "sk", "8dad467ce747ba32c947f2153dacc23cb1195425f8dcba2618e637abb4ca428733214e3d002274f929025e8d30fb55cd369ab231f37487ea7ebc80c25012574c" },
                { "skr", "75480e528bb82b0d013cb88a622b1d9253bec670485e28013edd831426e3bcd889889426cc24a43ffa117190e6d69f0786d45c1506b240a279b9cb456f331e90" },
                { "sl", "50bb5154f21b5c448c1436f092bb256d5617e2b718f6946723b5e0484594cc79817c1bcd166bdc316e4b6e874664c9920d52d01af156289382721489bd7a76c3" },
                { "son", "586c0924febcfcbb83b94c3d1d996e1ab449eb0944289ffdb3bd297fb45a681c0ff2b33af71757d7ee4251d9e76d390e2e0f676d866fb4c18c4a356526eac96b" },
                { "sq", "33881253304dd07913bac00d9abc21592988e8394399ee535c42a248ef4d2c2bf328152b382b44080b0c120bbe37430eea24db2f153c93a7f07404643d1da401" },
                { "sr", "444609ed349eacab13708e030a486926286065d70aa303a9157aa49365d2d183c647f360040a8b9695ddb8d2f8a2b09a5b25842a8e1957d9e97929419d943b2a" },
                { "sv-SE", "c163991a4f112ebc5a943e46de2bdd41045fead8fa4030ac42246f40616cd9a50a9796f48e144e9f690190e6c6cc87b9dce032199acdfdf4441c840d1da023ca" },
                { "szl", "ba3a36425e95ad90ee33cbdb077a051bf48d726132805b9770fa27605cfa58a9070d84f203b2b83690dd4663bc70ec125f25802ba9bd739f5a63ce0dfcc0940f" },
                { "ta", "926433f5812953021ecb5b90077b26954a322c84018c15eac10a5ebe5ed0e9760fdba3a7e2678940616230257cb96f6785e4c981ae849735e5dcb9eb1566ffb9" },
                { "te", "6e783d54d2d0abdfeda8eaea19f60bcea095fb33bd6c94586ed210eccd63262cdc347caca1c94327a0fe4c2bd09d9894caa802494a38d04651ede857630cd550" },
                { "tg", "150810614d91bc88e8a7fba655769111f6ca9cdec8cb37f7ba42ed755c4da1884c6141a1e8b833232809c8a9b50ce57f2b81a91e8d45bd7d8d8d5776342403df" },
                { "th", "1b50af407525bf049023333590dfd94c1d6a50b65a0be0c71a0d8ffa1cb6ee5dfd7e5630d6de4fbcb42710c7383e40d9cc1fa9a7121d97ac287be5e17980fdc0" },
                { "tl", "59d01649c774435ab32af772a5c6590d933d56038ab0e9f910ad1b3c04d64f26b7ca8ab0243537381e2787e034a57f903f19ec2951abd12a5dd044b25f84d0b3" },
                { "tr", "5d6acdf2341841ab57eb4bb254806963132d2220a26c03d21b46fa1984fef5821b143f9b141c86e380f6ac8eae86ad71aaa708333db13ba69b7743cb9dcd8965" },
                { "trs", "1b19e3d1a8fb52cde8d984b2cd08e9c7cc1aba3f7206297307888d87fe5edee03e52498f0ec74f5fbc159578395b189f6f4e5d2c0a86e928e1fcf46f39bb3505" },
                { "uk", "18d1da702034a096625d5c5b605d9c78d313d77f1fac4523ed486a988ca01ede9967cd2ce4100e564ffadb12a503e03589cdc78a4986585bc0724c1ee2b37ce8" },
                { "ur", "a22fa1c655208ba94ecdd8732c10e8712fc01cf895b73034dce97d52a94d3f88152be4d9e5828ef3989f0ec1d0969920836daf4d73bc2a766c1223a13c7f4a12" },
                { "uz", "b5b7219947262ae71eadc31013a328e43ea7383e3a97dfecab24e8ea440a6e4fa71ffb78a4554c5f9b40814185606e1927e9c53d955b4a100451b0820d67ef8b" },
                { "vi", "5d40f413456418044155a315fa3b0120b1be062805302468fec8ce185b38035247ee31aeb664bc51fb6de85ee67e65ac6aed536cfe0e437a6b7ad77ce50a3945" },
                { "xh", "2e3a7bb471fdce5cda54b696d1243fabfb7b07cd1e44b9604c9c3b52c6e199dbf33b411c8a9a48523dd5d311f7087d2a6f8e74ce426326ed0035657df071eb62" },
                { "zh-CN", "fa7ac5a478a8b56cbb129a84dddc65315e3ef44ba0c87078c14492b458981f3826d95971af3853f14759a97334d7e9e5be8fa8edf0d511384dfad675ceb5f3a6" },
                { "zh-TW", "8d075ab4a1da23401fadcae0318b61f889b2bb1b51a5651f64347826da08b4180ada89c09d54b008559c81794b11ad891eee4749d323378616515e2cc07248ad" }
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
            return new AvailableSoftware("Mozilla Firefox ESR (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox( [0-9]+\\.[0-9]+(\\.[0-9]+)?)? ESR \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox( [0-9]+\\.[0-9]+(\\.[0-9]+)?)? ESR \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "esr/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "esr/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + "esr.exe",
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
            return ["firefox-esr", "firefox-esr-" + languageCode.ToLower()];
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox ESR.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-esr-latest&os=win&lang=" + languageCode;
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
                client = null;
                response = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                Triple current = new(matchVersion.Value);
                Triple known = new(knownVersion);
                if (known > current)
                {
                    return knownVersion;
                }
                return matchVersion.Value;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox ESR version: " + ex.Message);
                return null;
            }
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
             * https://ftp.mozilla.org/pub/firefox/releases/45.7.0esr/SHA512SUMS
             * Common lines look like
             * "a59849ff...6761  win32/en-GB/Firefox Setup 45.7.0esr.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "esr/SHA512SUMS";
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
                logger.Warn("Exception occurred while checking for newer version of Firefox ESR: " + ex.Message);
                return null;
            }
            // look for line with the correct language code and version for 32-bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksum is the first 128 characters of the match.
            return [matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128]];
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            // Firefox ESR can be updated, even while it is running, so there
            // is no need to list firefox.exe here.
            return [];
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
            logger.Info("Searching for newer version of Firefox ESR (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
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
        /// language code for the Firefox ESR version
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
