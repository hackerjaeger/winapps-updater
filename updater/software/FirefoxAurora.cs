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
        private const string currentVersion = "112.0b9";

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
            // https://ftp.mozilla.org/pub/devedition/releases/112.0b9/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "d90511e31390aade8456a839ef0f8725a9b9fb91c935818047ed11528897fd6d775b90bfb6b30afb188e37f816706adc66b5ca1b9a612d723769b04901fba539" },
                { "af", "4426d4fd26ddf9dc05a3ec9f8003f47575475cf19aed280c9dac622cbccd35744ce4a835b4bc90810a5b8f15d283c003bc8a6b174eed8d3668211d4cdf96f56d" },
                { "an", "8b2e818c8b861e2ee64dacb8d660b44bb17b8abc92dbcd801a22fc07c11ba1344e1c11e35fb0f68a36402fe534e727f36aaf79998c3d6623002d9361f45a5937" },
                { "ar", "7ea5f5bf5fa42fe6c53444be10415387b33ecb7787324d1f2cbc90336b1904e1376915c083ae9393ab67bdc4066f89a296c37058acac93b2c8857073458f7257" },
                { "ast", "a9c0b719850301e4a3004d78c8d5da4546957fa60f91aa234cb07c6870dca1cd80671b2cc98e0090adcc4980164d9ed5fdfd5bd2c50fdd5aa4ccbc1d89f7c48b" },
                { "az", "86a8aa1b29bd3d3ab8041d4ee400826e20fc87a0e64eaf207d522e4c468dabd242d8eb78755b74eb4bbeb07c367fe0df32d44e425a9f66443e2317c2fa157ff7" },
                { "be", "35a40c3ef1c3bc067e803164e0b86decee886df3d037312866b2039681ac69b4c8f8f2fe8b4922d2f472d707fcb84feb270b1aa50b2a95d9fe3a11d6e594cf4e" },
                { "bg", "dc80dd8bdebb0da42ef33399587575217983f50ca9fccedf849cc35912ad890a0b3525b88868755d066af7c7575a258422ca60d794626f4a9cfcb1c73a94ed9c" },
                { "bn", "a3ca13c0e5b487e25baf58a47c417ff68122d9e18f22f83c7691fd744b83de498174754b380a92987758eeb7dc6b53c7e65bd2a6b9e30eb4b205291a352b285b" },
                { "br", "2a5750fd362b309add632d6bf87a0feb3af4b400334b0471fa7d39f6d6a0c58119caea64eda0e7f96f757f5d94396ba27a85bacac452c1dd9a99869a1737744f" },
                { "bs", "5b0c1ccb8fce855756ce23a9cbc2a520845f27ca27b1f207a165f6a0ae2d41cd585a01daac07b5be4683f3c66939851584d02bdc0c9fddea4ac4408c60185f96" },
                { "ca", "729e3fd832b5fbcbc5fd74c5e0509137b04945a1a69ef03bc7f7f886f5f8658666d1e2d02cc954f1d5b9d3cb10a1a0257353de4d4d1ce946ccecba7fef63c99a" },
                { "cak", "2d753f6805d71e1ac30e451a815c1b5fd4ef8c5f587115c06747479bca630e278e840a29bf4ea7e456b90eb9780d279ad1032acde6bf8facc513d5256bd6606e" },
                { "cs", "7bb24d4bff3875a40c754808ddea21884721457e933328a54a5ee68868050c8b88953825be90d84c1a2e0e78eeac05febc509760b8545b8a29e9cbd4277bb310" },
                { "cy", "9ff048cc9089bb3bcf21e06df56a72d645d4a09ead6e06b4bcb0e483f0f575cd5de7036f5056f0b2a75d70987a974ba24fbec131ee49102bd60de66578bd5d3e" },
                { "da", "e46393879ff4b1b1532455a5f2e9c9b3e6347e25e6a0fb37ae77214db7599ab9d1408b6af5678115a1e80e3dc20e279350ce064d75fe1aab9cda1f334c19f30b" },
                { "de", "0b75fcbe79f44a369441c814f9037cdab399313445ae1fb35ecb6a52d5f68d408eca1c7578062775e2fd8c67fd82b47fa7d8c1c53861ed3a87368b2cffc51540" },
                { "dsb", "27e47d22721a03a5aa75b0d09ec0f19cd28d9a81c7eb9207fc1b007bab3444f625fa7dc4535b6611b85853e8882ad7a9c6d887029a931e80e92a2d186ff0005b" },
                { "el", "415f4764c22d8e5c0a9910a4c36f0f0bf0c967b558e265c5143944c18bbe39e9e4af86f97b0d1be4cd3e7fbb53cdc48d557a596c5fa287f5520e54c7da0f7085" },
                { "en-CA", "6b9f9388e6fc6f6eb5cb380fefa9c166ab419425909417715998186e8d2ef97470031d36520730d5d946d05922a4e058e78c8beb61b479564f99fc93deb2d846" },
                { "en-GB", "fa94ee44e15e6d6f9b64a38b59f13001487f6e8dce61c8ecfd646c72f54ac9e53d2baaa83c3d400ade5e5bcabc51a2fa8f7d0c65a6cb62b28f5d6ebc370bdc05" },
                { "en-US", "d4a7f6706d4a292beaca7bd453b2711b24c67f20a70a016a503640247cfbd6cdb6a407c09ebfe8cccdbf90158f30d7daeab86447c8dd82ce442463660eba9efd" },
                { "eo", "d77ef3a2f66fa765d08d466adb499785fa7d4173443eede988580240085ee1afe0e364d3ea595f81b123f555ed0cc76b360ed7026fb6ed7579c2d0c70bfa2651" },
                { "es-AR", "98806725c3b72fd1b7b8e0af1b4e91b4d70ec6ca34e2f67263e1c8f2bff653303e6182a28ba1b4fa3233d5c52c34b22f2fc5f01122827ca9e8f2f3b44fff62ab" },
                { "es-CL", "f5121e9bc5b5e816b0583fed9f53a60715da7bc7c49bbecdf399bac9e3136cbe577a87626bf1decba58402a15f4dcc4132978200d834954fa11e07c2407b46b2" },
                { "es-ES", "df9ca134fe2f44bc6caabf5ea0ad2c03ae208511e347b59846e060375018056eb108951a3cad86352ffa5cbd0642afb78f1b93ab9962d6baf383ac38ebf92310" },
                { "es-MX", "f2956c6bd0b116a7d1a0326810fd78de3746c3d95dd043a3233a36adb105750d2172fabebda302b400082827a0a574f8f00200bef121f4fd23c5194d939a85b1" },
                { "et", "c39b36371289bb8c066ee152102a0a0aa505c7cdc7884dbcad118926ca1a08732ee589f1916d667db4af2cea40debf2a4da5d856695a31e537696623fd165a1c" },
                { "eu", "0697fac1fd463374ac005beabc3544b7ad3be1df6918bd461d1ee3b4502b420d1b94c3da580ab082995357b4c2bf173b34e1c172fa19a31cd835f2181a474164" },
                { "fa", "21eec7451fa06648d3b420f374522cbb5f768936441c133860f3875cca89ca1295c7aadc2ca269d4547a0627e59c0077a8124330e7fff429607e1e496de5e830" },
                { "ff", "a701a11eb8ffa05d5642dc27984e9db64f7142e920c2df6927247192b334b5419a10b71a44e16c45a4b7350f9c309e8e2d38f9531993323c8b2bfb7dc416d18e" },
                { "fi", "bfa050e290cf84a0b278ec923ecf82cf1d3b599bd96688114b08bb17a2d9f13de2c183092de8109606ff0366fc67e4b0cc00b5e95ed211e5340ca2b12e31dc33" },
                { "fr", "f18cac02ac51cce5b5d736166f54bb11047f275e80290f21ac125359c6c12d448c575a2721781e4096afc7e3c7c1f98d1c0126459a35da342a7abeb591728c35" },
                { "fur", "8f044fdf73de4c57555a2a767fa880698af26ce720301233e52a5601fa2c93ddbf366e4eb621429845284c20b645115166f866aee2300ddfb62a3634598a8366" },
                { "fy-NL", "37acc0b94e4d60333083fddd79dbe973d5e0c1fb0e79d41411b5dd49f632ca0510c882a4b753e5ebcecc2b63d0a18fa715d027ffd6d8bdc46e40ba52612f4e6b" },
                { "ga-IE", "668f6553ea5bd2b1e43dd4c9cef238e15eaa8416b64cf2cbdb090bec87db522a4cc3c365e59b4a3dd6eb6a719ede2be7001900fc7c46b9e7b91ef9f03812e31f" },
                { "gd", "246156269104d38e37b31ea19a3e48c309587e5463313e206c4d275d0a58eb92e7c0b01185c0daaef4d99f358d2ae781300e1e67bab8e6d97ce34713d319fd0e" },
                { "gl", "6888c9ef37d1215b48c1f2c262e3e149f3b542dbe8427cb7a2c611c17dc1a7e0bf8c83541f8962e897f5bd9e9a4b9282ba3449ca6b19c644e738962bbbef452d" },
                { "gn", "5bced472555f3979186d2c32529c66338012a03cec22500280853b04ba389617b0bfca0c532f91a2cb7aaca7aee29e097a8ff3edb3223123e04ffb3d878e958b" },
                { "gu-IN", "d930dba132cab91583dff5806f6e02110f82dd1d083a67491f1ac3f29fa41dd89c007edabbc6495f669ea94d144229892f6785e266fcc73ac1dd5050505dd3f8" },
                { "he", "dce17e18e531ba864f259173f55d135cd36412f113b804f14263dabdc447a5943dea3572ce1aeed774dfd6005d7f8aa0f6cc8b71da44b71fefdd40670f555f0d" },
                { "hi-IN", "255583672407edba72dcc4b33a90bbee27b078212203e010f7ef5357afba2c189e2df37474f18a48d510d0c87ea47bce3283e3b681a0cc099b6b006f03a56b56" },
                { "hr", "1936d00ba4017c45e3f5f3d88b4f8a35441fb1b65daa4fd644aff546bb0d85adbc08e9ad0776b52fb391256fe13c3c2dfa01aa8d60a3c59cc10f164f394952e5" },
                { "hsb", "c78dce5c105fdd61265f6c8c5f7cb8da81c4e86b6cb5846252b975fa7fcb019436ce9b35f2c2b9d00bd99b2b8abf8bb066f38bbddd9bedee573dab8fb0ee3808" },
                { "hu", "62a53d81da8de8fb6e697b9ba3d9db1749c9e57b68600ee11a5d14694257668ec6698eae29842a16bc2421c630e64dac003d2da91df9fd4d7dc4d69e9dc4804f" },
                { "hy-AM", "97a8a8ca3198fb9fb644c3f9fbf111c7d02d55e9f21167125b484e25e3caa20f087e1628b75669ab72d9bef8fbe2f58bf0094efc1fe62002883fe5f6d65bd528" },
                { "ia", "de5ff3e89d2b302943b95c5ee29c8796fadf012c830ae836be68297b8697ef95f70718e6dde457c91ace5db6508c11d8a0d952d35e9f5e98063d340673cdcfbb" },
                { "id", "bb9fc0a5d1944f38b6225fe2d89557f231879c0f07ca68a94fa0683b303f9ebea9788eb699dfb5473ccb1f31cfa939e3469ad0a568d4e5923be0147d6202dfaa" },
                { "is", "affff00412a30ad03aee7bdf74e7cb18426257e3cd8372cc7b70b214ceb286bf263e10e1e01e24c2200f7528c43de9a76f5f78fe6b37745ad2e99c0b6f749422" },
                { "it", "b8d52bd2fc3ad66ebb894ab80e31da0f05f2e011268f1816eaa8a06f10456113bc317c68d58f5bed462ebdfcd770399af76052e6ecf6cdb2b446f985328e399e" },
                { "ja", "8d842d4356544e46e51d78284ea860d8cba8460c37eb694dc23e553445b86e68d8fd037e0c31e3fdd81024418f409245955c571422cbf19e5f97eb1564c0effe" },
                { "ka", "ee8251bfb50b7c2758a3f69c097720709c95a04dfbebd5b1193e5e35ef493251bb0df2f0b18c7b63673cc8d2dcfcf1d88d785c603d8648c04c21eb08a7af1aef" },
                { "kab", "5f15ed5ad4674088dc91a28065c0e9d56f461198f082b85ba05448ab40b363f11c9fda37a4f790d0b3bcdf60e984bed7d88effd2be70c311f297b55e120c2bc7" },
                { "kk", "3734b0e01373c79593af4b70d204388390190eac44a9e5f5098576126a68a4328ae30c25c9347b7c3d15d1ac96a43b960b967efb303fb41d7a60bf73ec23b3e5" },
                { "km", "90b6c8b99f60a03db906e66b533e4e2c78084cdb785f20e60e9010bb2421dadb7b33a4e22e7fde16e6f032db9b2f39ac9f829c075336560e71b53745a0fcf9a7" },
                { "kn", "69c2c9772a40d5c506ab37a5d46f095ecdee6a372a64367e5bc77d473540c78de58543a0685657a7fb432a8e03ce5736762b8c7c21a109699533d9cda315a8fb" },
                { "ko", "8e3e3547cc4fbc457b10c2beb551bd12afd3385c334f609970e78fbc06a1e9219d105f2bce6bb96db38eaded68476d1b47aed6224c6829a376e90b13b97db989" },
                { "lij", "ce41a51213c2a29cc32e3946366cc03259852e8008caacf2a97cb2a7638b4870c8eb96bdd57f335f91c9691c838293c299e818e069978289bf169ecf61562703" },
                { "lt", "79d795832ae371928d2a69ae638e8572d9a4c4c3fa0b842e5aff618aebc48c0412adb563b8c5dfa0d2205e3c344ddc7b262c0e4fe1538cce7fd6c8c344e13c4a" },
                { "lv", "eb5e19bddf09a6a054ffb91fab2de8c1034a5c4b3cd2b0aa58b6176e7a53464cc4877a8f16e03a295a0414da71e95c0c287a9558e334e4cfe53a867122dbcf1e" },
                { "mk", "203e54aa7b476821f0a9bab4f95c68553574f7607d1951870213d23daf1346c536df741ba1e2e2d52947d78a9c20d4a5ebca814ab20e132ca488ce6ae70ff2c3" },
                { "mr", "d73304322c9da8d0629876b107f6946b2976d11db0fc8dd38d4fce946582ec675091387da6e159c892a8a3028bedc89b9008f1ffffe5811636508dd4e45dcb8d" },
                { "ms", "b4f2533b482ba724ec6ebeea88a2d3a80fb5217bbc513885b1ff12fc3e21777783d775d83c95a7e904dd4d0947a6085f040d3475921459f7d702fb1724b53608" },
                { "my", "296406624ae9e84c092dad4211c374d2d5e08a624985df4b3fb66eb1b8c1af52addc81b112ce7cb38a5e25f4d9abd6a59cd2129c25e82d2ec5c7201a9b54d12c" },
                { "nb-NO", "3b62fd9469686d4778e464e177745a2f524aabea78c463f0229a441f73d5e18be1f475058ea010f7ed3c8778c1de54a995eac441af7905a7f3d6a551da565c8b" },
                { "ne-NP", "3fdd18bc319fc94f43dfa25e8e24766831f566e518ab16345a2f0fa2611fc8d7cf5b92cdb48a59d0abab43679c931015f753c7c6394a91e13871399361b73ea5" },
                { "nl", "456cd217a833b47c6982672ae27c03d40a3e976086d85b6c5141f3b9a10b2bfdfa2151872fde6ec8e6a57166855c9e96ae0451670286d6e011e9e8a4e36000b1" },
                { "nn-NO", "1235390c09929ce2aaf86319088db60c4d1aec1e6669ced24296ac9f1b8a6f62d90fef367e96022477513d0fa2920b11676d75b6382d7d8047668777c62a7b4b" },
                { "oc", "2436c9f017ad2e1315b0b2fb6ea7f4f34a075758f9dac0a47717ac1f7a1b5161ad759a6239857058e470550c5223524c352572ae81e6bf8925e3f4723926b8cc" },
                { "pa-IN", "022e4cac17dae9499949402095ed797dc0545f746a8ab85a1ce6ad07094bbbfdf76b563eb48e9ba41b86e94a90da259310bd14fe19e2adaf01ff83d0e00f6810" },
                { "pl", "596ad34aeb438f834b17ad8bc1414ec63101cf7b9b45e07d739bd3377294f40b7f203665a0eb459f9928524aa40eddd8d669986536c9e90623f62200093ee165" },
                { "pt-BR", "ecda0ad8a77a3a17fbef3b8a788e18ae6d6d631e887b2392ab4fd9edad1d9b6c71c7457b0d257ecb04226f19ab04a3f6d1aa4b315cbe6c4141216ac42dce6667" },
                { "pt-PT", "d3999f0cbf776b0341489dee57a42a4970b6caa0cd242a449492f8b10653c36dde7231de92bfa3383ae1bf2cf5f8122c86fe14a49dc12d4fb0f18fbe50eadba8" },
                { "rm", "b696a4fff29bf690c49a0e6fef162a225826790cddbd1ab4a17d4ea1c76aa9bf474b6db91a3298303c6c78f199a7d12e048bc8c87192015b32caa012ea0f9196" },
                { "ro", "6e5595e4c30c981354d1dd09f9102602c85e232bf80ddc968e00b146cd5db10127b0a343fb101f12256bd10da4ef36bf89a9b66dd2b2f1f8eae97d410791c53c" },
                { "ru", "2a19d388f7f359ad6ea9ae297ac6a03e2377a04ec42eb7a001145f59343638b8821854bb99714d0f3657d8e372711d3825160bba3e12d33c4b831a33d3bde8be" },
                { "sc", "0b5eaf79e59f933a2e9b5d475daf914b6933387f76fc7fa5c21ada520f36a7f2f893b361e7339f14bbd3f7b554cecb59569887059f16c18e3ab83c744b629f74" },
                { "sco", "8634f60ef61350822107dcf39e7f6b2141070d6bf3084a803523d24e83eb30663458e1a5fe36aa2c8b885a36963ee4a39f0a0e686bf22204ac4aa5e0edcfe636" },
                { "si", "61d398ffe34e32d9bf41cfdfdeb4f67361320d6bbff8f8a749b99675cf113e2adda226d8b76e542a1f8526ceb1f409afb9e99b67685bdf17cb75d8c6d24502ba" },
                { "sk", "80b1abd865026dc8404dfd683ac388dce2e89ab315b9aea89708069e9df4354bf88072c7abbdba1434671780446c913199180360d2c7a8fb99f0501145990ad6" },
                { "sl", "b29386aac5f974a4ba699b1a20fec7bf19a79404616acab1a66ded66ec348dcbc96078e88703b60ec4747523f3e80abc34dbbede572d6bae2f5432e9ec63358f" },
                { "son", "2a64d0c020589cb6f6865b703a5c58e29b88f08b5893f43225f483d7e7ca8a51d5614fdf5a61aa6f9184022e1d610b45f0c32f894c309e71412e4dc4d37ca8b1" },
                { "sq", "9658d4cbd33b9669aa29f1504002f983b355143cf67e97384188839eeec036d3553cb5eed98f38ad064bca8e33fb12b46f02c2246e5bf04779a2015c00f7558e" },
                { "sr", "a66a29dcad6c503d78b35b8f6174ee5f398db1a4e86a9b2fb47620f540200cd4af9881b9b029c7cf6c0e444a2aca6479349abc0a34fd3fbde922a7cb2489fd7c" },
                { "sv-SE", "93616bb8d9e268563f01f46a322f3717ca9817e717b8ed073698c6cfb0076655482ec9eb7e6d82c8c9407ac3b067df0c597f8aaa13d57201e3b2ab56143a89f0" },
                { "szl", "85351fd3ef4bf9b2001f3235072fc2178808cefa9747a6b7006ab59782dce8710f94ebe808d131ecaf4f84606d5a59e31006d6930640c15a36ed72adb5b33b3b" },
                { "ta", "654aea514d8230638b693c0593fc2312cfd82ad39d0bd162656d83f324a77a444309309e0cf91dc13080b54e3ad5eb4fbc7f9e321e66bc7596d8ed87fc03ed56" },
                { "te", "b50ea23cfc08d3ff27f89f39f760df30ed9711bf7e5358d9f6c47cab57ea10317fdfd23294e13aef5c619c452b1dc117e2870f9b9c85fdd2cf8666b6ece7302b" },
                { "th", "a5a50bf384cfb5b248dd724451979bdbf8a02830d3926c474a182703df8946fd65c7738528216f8ebfed617d90389d5d68bffd659f1fab75ced1f895720075a5" },
                { "tl", "25cd6a08137d52867dade5bc7f462f0540d8e2f3af021a25fd7be008290c5d42843cef7f6a072b10aa51e7068d9771285306f5b7cc3791743a18ccbfcdeee68b" },
                { "tr", "3d08cb25428684db689bffc514ec30feaeeff04d7d6ecfd886543fa563d34fe533bbfd8b36ed33403edba30c9833366dba634aa64f23cdab2f8b9ed1ca5ba2c9" },
                { "trs", "20a66955ebb9516d5138dccb84352787339ea2782125f0309e30000defa014589e3882a04bac6bf185a49a71548be23173a76662163838de820e2c2319adac49" },
                { "uk", "d91b484addb142a4f252d885625e280d1c39472a77548553faa093867c2826720918f7582a9acf94acec505efca4f38155fb44d66fbd91ee6671f1e27dd0666f" },
                { "ur", "6165650d357e1b3ca843a77ba6a35c1b3b3b25db86542fe91dbb54116853de0fba26ab6614710d466a2bbe684968a6b630d4751e1dc149437d4a071a9e171adb" },
                { "uz", "e08434ec881067386c12adb4cc314363da30855d8ec0f3a1890812cc2eb88567becb7d502008dbaff31fa7a19c2021388a13e7ea53c206a8ad2e978876b55ae2" },
                { "vi", "0e1ac570f06c1e52b26764a35e9c0cc65f0a25a17a31552b4010b482f1de56bce4f800cba4e32ba749bb2fc1df5ed8a8d8f81e965b6155fb77bcfd510107d5ed" },
                { "xh", "703df022b154a097d66b6d49d6d21a0eaa01219a14fa170ebc9b475dd117b6154d1b28ed9304856635fd3727add98a4eda42830a8b96cb1de9436beb308d07b5" },
                { "zh-CN", "d0861eac48cf62b02e4341f57ccb38663c1a9d4190d69f40660f94a6bbce51a9c0e1f60609cabe003b0975e017e351053b0073d7fed76aa6eeb89c8f19b099ee" },
                { "zh-TW", "b16743c7286cf38fbfcbd4609b9b33051e0ae920b7e78c8421218646a36a25a226c91ca0b40796501bd0ed97c1669e3ecc306c9b947c2f4059d3e4cc60a5dc97" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/112.0b9/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "e1d87f230bbbc68b054a464c2317061f56f9d3d3f1419503d3133eb7e17f5260704247da5f6b4d5b80a4e73176891e1caaa605b5e09401251057d0314a73d54f" },
                { "af", "82b5c2f883db3ec705ea1dd455afec84d62e0b9d3b6aeb68e77ad83c0937da2123bac6eb7bc2f8cfcf74c03334c6ae1cb68acc5a1f1776c2da52d1e81ea13072" },
                { "an", "7f33a08e95b7db228a67a29664875824f90b56a0de5d71c19cae67159b917a39cbd2299142457969b9eb78c6cd1a389b242bf45147c9ffcae371412f59867478" },
                { "ar", "648b907ae24a73c24cb9d07ad0192ac6c80e03cee8d15c80dfdc1992a465f2bb4caf5cf15270bb49cd0ad719f0588606cb10e5baca09cf5243738fb2b23697bd" },
                { "ast", "39f9d1c62add27020dece306701e446056c8ca63022be3841c8c2543f813fdc97678f600a867141375717c57538a72e003cf574edef14c7ae635943d71f54834" },
                { "az", "0e6a15ba74eeea0d36dca6aaa1260dddb0f3cf503ad433bb891da3ef2ba3c473be1a2453ebb68159d2ab22b249f08edbfc5c73875487d8291f25d8de9d627164" },
                { "be", "dbc97e4c80f0f8f6c2889dd8990d945fbb4d5e1a807b49a03924a56f2e34b68d7f58f4961bcd5a61979298569878bb3a2c55a6b30f9fc65fa7bcaeb7ad264e9f" },
                { "bg", "455d835faf2f5c6ca2aff4e9ea4ff9175ed26fac2b0a821d7b440eb0e5d62489f3e2acac8766a9c52ba88a248d3906061f1c398207fb04dda1afbc7b629e3027" },
                { "bn", "c05d8fc30b57c74b73ce94acbcdb9ef4e32fd939ad025438ab396d27318b3e1fe98a3e4f680102d81af4b3228c98bf338e95842336d395246b96aaa78393c2b3" },
                { "br", "18fec8f196190f9dddb8e8d6f0dc3a0f8f65a5412dc0c15ceb857562133cfa59f8018860b52d9b900998b4cb0abd1ec073043b2f8f3b73ad25d8d251e094d896" },
                { "bs", "efbd8b20fa394c6965c932fc815a9ea061759a5273993472a5bfb47870d40c2097698facdfdb0f1f01739c47a8148750c6e69ef0979dedd44fe892d00ff6ad06" },
                { "ca", "1525ebc5c91b129fd2604b3488b59bc28e8fd75b8580206908a650318b1d45cc9ad90e866afc7fa554a941b011686fbdefdfafb3d07c96b13c47153acafb0f58" },
                { "cak", "d790790867b706a0594f39936751aee9598d1e15afe6b889b29ea0a03c1f9ba78b42a929c2544ea3b64d686462843c6a2a01287e20213fbdc21a2231866d19a1" },
                { "cs", "fabb4ce28ce5e174b7917c93401eeb4a2eee2aa3d5da3c6d724a659bef7cf7436ab9dd9410efdd535828f0d3f2b700a5c449a25c7e1429b6a0e70b2de6e78508" },
                { "cy", "85d7e7f18d2665252481358047c5c0e6a3c6e9cad0b084df84c484fbd64ff91fa5f5e941ba329c208c8474fbe557304585aa41c06488509c8cb68990420c4e7d" },
                { "da", "76de3a127d7a00cde59e0480b8b9cb843e9109ff1ac35510157e79c5a6ee544e0502c4b5c1fc4cfe08aebbc227a7bcceb55c325a008cfaa19bf7a80e54fedeae" },
                { "de", "56340d03ddecccbf1b5551cc8be686ffa1c9887bc40d76ce23236b9c7fc844443ef047a7178460c6861d4e58c7d485f637f34941f17516d375a661f1fb718b3f" },
                { "dsb", "a61b252b0ae7ca6e19b4971f04ea08e3e473da5364092c96ab21b1d128d6fc4859e1cb51bde2d0d9acf16cd8b43fe7dac57b524da9134f6a789f1d5b9fcf66a1" },
                { "el", "9bda0eda39ee345f01b79c6c4e0818acb66424a050f444eca74f942e7cde683a5705cc52f477268b4a3e5aaac979ba74a63edfefab5b61e50f54ee973c151068" },
                { "en-CA", "79f4fbbce6044504315443fe7825862dfa74dae4515f3624607c20bc132bbdeca32280f5931b80efc7dcec49187eccdde34cdce678e18bfcefcc8a7739b85eda" },
                { "en-GB", "3c0d9c6fdea2ff6e6ca1ed1014496a8ef82e6d673f6083fadb682dd879eca302c440e3f658eb6d12465aff4aad7d3ed4be50773f55fe19d4bfdd9773dde2385c" },
                { "en-US", "5afbb9f50060c66dd15fde953a33c0d6b885d140dcdeb17001e5eebb9c51366392e3520b705e91f8433f0c435d23a969e49d1feba08fbb6bd7af930270ab8160" },
                { "eo", "1f9abed2a2a3ef121d916e6e1860a9c4d79d92a903a734e930873e0e773dd417ac29a22aa1512c3bcb99e29c30571765e2b11bc7f7b75d3ef238bdb8493344bf" },
                { "es-AR", "0e550b4f7ca08d1906b718f006dfaa2378ca969a2b4a0653c03a76775ac39fdf87d2d9b9957cbbe62f39d884494230a2e64425b886a9713d9c037c0c6216a884" },
                { "es-CL", "a613d2303267fee989ceaae22ef0739c4b07841e5faf25ec292870eb7ea7a49cc4fc282245390329a1529b35df3cf272f6fbe790c5393c739d9204f2c06c1fd8" },
                { "es-ES", "356c321b1747b40b52f2d2869e449ecb2ba859f413583137efe1ae6825d88e6ad5cf782690db619b200eadf87fda9c4a25f4e60cc22b6c82c5bb7d8722312686" },
                { "es-MX", "46249b75d2284a2b46ae2b253448a73ff652ef34b3304927a0abeddd4691948222d1b64c60e81bbcf164ebc4a626d5e82f9a6f46af418d57d18726eb32fb85a6" },
                { "et", "c0d913ec8750e7bef5405ef083e3eaa154a587b75d725ddc5128cf989215ff22660a76996468c6f64e55ba717ba678135616ce80de466b24ca714328b3e52669" },
                { "eu", "dfc960e8209f5622e96c989a797c68445b8994bc53f29010a094fb1f01e86e435c9c83ceb4a3df1ec3b217ba48babd941d5424e685dc92dba625d18950d8515b" },
                { "fa", "4faea1a0627976c7936cc0376e8b82a316215aa326587a330f51f2d7d23e63f72206edea6548e131a2e7dcfa125d51854a5bab4f625c329fb1ecd6a002b576c5" },
                { "ff", "e5b3c03d57e637def2a541a1a25f871a5d8837244f114dc05fd8d42f1cccd13e528396c3c0da721b813306a48ce141c06b5a5835710eaf4afad2bcd8ed51bf30" },
                { "fi", "7f957c983fadc76c679d142970cc34f94efce3d1e3c8a70c37e951a242e8ed90cffd4ca7d276e82174653151519f1118701a6eb5276376ffb4f6937f0de100f3" },
                { "fr", "baebc2a6cc687c1878eaffbf50ca22a713aa07aee15eb77bc9e507a37bac74b3ff91c97b7712cff2c7d3bb25989b35a8cbbcfbbc90241aaf8f0395f97959ac03" },
                { "fur", "37a8734fbd7a1348335a04948fedcbbc9c87340ae107032c046ed2d88897cd8bef5530f584b4d99d18f4ec8f5d0d697b0fc7f67126f2a81cb5eed05fa2c7c021" },
                { "fy-NL", "31a05afe6f50c24604cc2762877bf806aedec1a2b9c6236688b8192e9098ea174930abaa511d3bb3d3242d44233df07f5faa79254c93e9013c04aeb37d461edf" },
                { "ga-IE", "18c8e25ea0afadcf20c26c3f26470292c33ca7d7ad6cde2f84277ed374cd7d93a897060b9f74fa17ce63ed37fc6c92e8788746daa09b0797c75de8d67b7d85af" },
                { "gd", "2167dd7d9ce0478088f9bebd43b827ac1a1830cbad63b433befded2ed8e6f5f83b1d95caee889f588d9eb7eaf6813bd267db9257a5ba5d2b9a369ab9e5c2aa4f" },
                { "gl", "92344a2712cb58e067152de8328e0d693be27527f23db7d64bcc33b6d8987c0229713d7ad5696f30b59f804ec20726b55a035c0f9997acd43d96bb718dbc6ab6" },
                { "gn", "ab962fcff2d09519de776b86e29ab0808a2a5bf4217efd35a96b27de57ab38eaabda50d79f8b0c5edd264f3f07081ed224be27e055e1d2fba7930ea2921605c7" },
                { "gu-IN", "84dc66262f7235483c0f19cece21b98196178d1d5339cc7b3e4e4b2784cd1ae63614769fa9919da21cfe04734f722bcdeb10e5d522944fa6ba5ec50f68d2732d" },
                { "he", "21242f29179578964a50b4441e19379910c048db656750e0ef4805b9823f7fdf8fff7d43a601adfd81f622b560ad0b600a0ff95b473d7e26a54a54590c228b18" },
                { "hi-IN", "01d1db877cd6bd39026374a0255904ad2a8798724ca7dd741a5860758a60c93e065b973c8d0840d5804523cc58cea1647133049a0adc91ea4b1be13cce9d0bfe" },
                { "hr", "a9c9323495ed3217f0e8eabf61b5d90a1d29a764a47c93c9be6a8987869d9ef454705e9c939a13746282b3209ef29dd71ad2afbc910d0b0acf7176788674eb6b" },
                { "hsb", "e18984f9b25276ab1c7947f097f354059c03095e80859c062129e2f1cbdf4b2bbd6b73af2c84bdc7ef760dc1c5715beaa6babad77e4d3bc5672dd9a1fa0a09b8" },
                { "hu", "8fd641a14279849a1852438bc5ec33da2fcadd6c5737c04efe9c17a6d8df1e5d12fdc5ecc93e217f013c6e2b7ef04ab8b6faf3375d43150a353c58464f1dbba9" },
                { "hy-AM", "ec54d6ab0104aeb8eb2b4cf9c6cd937af62f83bfabbe7830c202ae756db5131d167179ea0c332722bd7106d53840be41ee5c2bd5381622d4a3fe35d0fc66465f" },
                { "ia", "a3c3390b90778df18ddeefe23eec73f34f49f6e5f28021cb59fd023088d7c8b20aab046b9ad4fdeb559b69bbda3da4065cc3f652ec28fd74755cc4f57e4fd855" },
                { "id", "5aca9127071d71d0fcbda6f56cd69377adef2cb787a0e58c6cba2ebd0c5495f33d755037631a0c0e8607d62da52a8eb06f4622926cf8d9fc0d712cd0418ed640" },
                { "is", "b3c9c34cf0a4b14d8a3dde2ef6ed86b773bd57f3a93e4f3d6baaa73313488ce680eb110f50d9ae318a432b202abe3f09fbb3807182ec670e4b36d41c1677b573" },
                { "it", "f6c8bb9dd9e06865b849990ff703bdfd9627e00ae082e03e9707679da0932c02e15e010b9b500efb6030ff0e2b706ac7a1c232dc607ac4a35c9a0914d71c535a" },
                { "ja", "4a0b0c6fac3b68b3978f322401c8fae7a57d5c91b766108859d58943a337fc75aaae93203d630dc6433269e5d4469b495560b958ee47f9cafe6bdb9284043d1a" },
                { "ka", "b00bfd18a98ee7b08e09ab25725c614d8166a3b2d7d98a6baf7c9fea3f18c478b39c85c6ff5f9206a48850af36fc0f970a3828a0b1c6f81be5f056e3f3aa9260" },
                { "kab", "5c8f5aa2f520e04be68d58aaa3aa0afbe1e89a072262f3de857f0ff1e69887e845dcda33a3f4625387d473f207f7fb172c23c871944dc853c74548e4ddad231e" },
                { "kk", "5ecef87febf888e5f2369fc18404c3bcd18cbec269b3dddaa53a9932888f55efa6366c30efebdc06467e8c1013c268b3d6e3e36c4d64d73ea43856757a59bc5d" },
                { "km", "e1cda427c81cd5ba3537c098d95074a106a66589d410957471ca8dcd625feae52c7d22b9d51cbf90c4377468fd3b661bb7b6e9e61701ea057ab52a051f650c72" },
                { "kn", "45bbfae0d0d6ff403d5b8c1dd91c9c52649653e4aee82da8d33f60dc6f674d7520ea29b6020086b2b4c30270a0c9c674ba196838055e455d5bb4407a141d03ce" },
                { "ko", "2bfff1aeaa116c62227fabec2b187a465c2ad118f3e0e6a56e239fe21e15ef451829226f6054f3b016c5da890ac532e7c321e76ed19f3534381cbfa734b94679" },
                { "lij", "f21e2a79620cd8a14440a280e3772845ee06c6f9b568924a936c8f91c0ea92f9ada5db99141219b2412760f7caef4c87fdb47c810e5de340d2790efddd91e848" },
                { "lt", "42e725e29666b5d6be39a10870962eda5033094bc73f8e57839b1d2ff102417458786dc3c79f379962dfdc5e3871c4c1312e14bf004d26e294a91783c9838f90" },
                { "lv", "d3d5fe901579fecaf0e4806a4bb61acda2af3c6f4d4afa33effaced1d925cbb7cf3eafc77ff4c4169c8286fbb132a3821b893ff240a4c12c2e2ca5c5c4fa0e83" },
                { "mk", "4924622b13483bd086de98734ec60bc0d81aceff919a0f68672d3d6df7fbf6af253e78d95d5f6e3ad3b0c84045e9c27e591b342124b5477316a8d742e832c098" },
                { "mr", "1e1ba7133dd711bae80a5f0344f9dc3e4a31a4a9f90f3062c75d9a8e811ecc14c851f00a5c691d23251befaae89c35d44bcac1e9e6be90124dfb5b27c9f00504" },
                { "ms", "698a2e59605771ed3979a6621757feb166a31a28b6e2802bf01f35530a658392fd277f6e3bebcfd441dc134e4483056d2446bbd728e741c8cc906e2056d2f27e" },
                { "my", "02fcb6033b5f77f866be29cf6723bb73618f7fc6a5acd958b71bff5822288581dcc53d42dfbd26d89527338e5536647e81f8a6adefbc5108c3beb96bfb037656" },
                { "nb-NO", "1dbad8e4704fd7b1c97550f0233f6a6bfb1059c97834baf4390b833dd5008866df1709d39af9aedff7a651cb5141ac65a9e42b8dfa82b4ae752a8b98d618e1bb" },
                { "ne-NP", "b81d1ba3344baa191d0ee555e01f254fe24cf708b28c962736941a62875088d1b3621bc26b4854d4ae206771666c9abd8b9db6c6b8fb56086d8c42e31a38866e" },
                { "nl", "699426748d58c177b62813afe1f78f3616ae2497d07ceff1f0cca85ebf5a36de2333add9ef4d283c23e79ddfb3e0bf896e3de8e69fd7164e814c5fcd69205aaf" },
                { "nn-NO", "a89ba31454a3fa5ee0c077c8d1046af8685e8b0d8103a2e460f42a75a3be397dc27153a45219d54b9dbeb7a176c035f0d136e551adeb3c4fa2f897f1d1d2fcf9" },
                { "oc", "e5c4231db610cfe49f5a11355623d9f86c27d8582450058f17fa4ec76b97e8351522b46f952eda2b7e4dd05c54037886c5adaa16aad0f7a3a8cca2d38356edb9" },
                { "pa-IN", "3ad2f9e475f311e692dbb6e46cfd3d0473aeeab31eadd66bbed9ab82d690928e3810b56b200544e0b01817f23f06f524861e4947ea7b054e068786ebe9619b74" },
                { "pl", "7d0fb8f11e7c00a3fcfbd9c58e3435c50c445fe2b67b889d3fd63488652280c8a41389e71b77581b850eec277e64fd65ed4e1e44c3e827d645ca03785608b25e" },
                { "pt-BR", "2243172694292170a304aaf1acc10b048d27fbb9f0003759bb611284968fd045f1e3055f33edbfc0b1a28ce660a69d0344bb7559c306df107656cca6269c6b35" },
                { "pt-PT", "f39e088f6edb5d9479e78a822759b03968459e4c11e15429462d36aeab6b4414b502138cedb098e3cee5ca87a91726c9a3e574849c7ccf9a6965794e6464f1e5" },
                { "rm", "01e7f9f9eb10f9a21e7813ce855fade746fc04e6a771e5a3ab3242e6553609b94bc5b7538dceb6d901cde42ebb92e98a0a8be63e9d0c9813299576a0d8c37357" },
                { "ro", "ea8e94d470bf18b886a62c1e6c4b025250df9c8001f82274acb8e71e09fa72da5d8227bd28b6c9fb8e86321306745df7df027d2b4268ef331fa234501f766361" },
                { "ru", "710cfa302732af0b00926e395eb90c0e80ed2c80b65b8cb2d88fb35cec673f68c3d72f743739043b9f2546d128db850f140c38bbda90fb02951d675357980d97" },
                { "sc", "75d05b9b20eecf8dc8bd01121f69d70d72d0d88ec4df1b681dde7b1dda2e802ce4025dcc6ef375f77e4965443b6c1f2d320d82bb5b90119908a91a46c7e369a4" },
                { "sco", "375e5e7f07da9b2fef27db06d665d1b07618bcf53e93dfedb421737c55ab9852d75404c1d2cc24024de8946282a6660c0783a620661dc3b994afb3cf157ae9d6" },
                { "si", "02296261a2e00870f9e7970e18b601b16079965bcfc08e51683ccf2e26ed93a61a05088d4e4831e898bbb3fb3b7fad836bf91c8d2f52be4a507856ec337fbc9d" },
                { "sk", "3ca31c74e8eb99ba814fa13ae5d5502981a45456f9a067b05e3046f49d35e6ac4baa7a6ac5d93c5c13c83b02b6a7a31a7de1e3edd1ce73042cee9f628e481196" },
                { "sl", "f96fdedf1ec2e595ec82b8a06ce73544fc48837e3dc580352f9f003a01cf6a5e618e90a8bfa059bc31f124b999589d0087a999de8e395ccdc1f440acf7872ee0" },
                { "son", "997a1566289149c021269a494dd7c8ba55846575aaff7279a7219ec646fa55e77aa4e6a1edfe4b3015988a62eccba22b63c372872a696bcc6521ba31d487fb1a" },
                { "sq", "96244c0311705d893ca5ffddbb3e7dcfa744f95cf07f2ef9778830e9298c3ece929aad7dad2a5b95faf9909439109defdd143833796e07bd498130675929fec6" },
                { "sr", "9adfc975988a77beecc354c1551f48731597193a2f99b70e95b49bc7a5d6bd26381bb8a023ee1d66a4de7841609adb4059f9b0968fbdff098678ef9a3f2ec010" },
                { "sv-SE", "53f1fc0f2784b6392860f95592cff825f21d16e1bac83e7b4e84932b60e61f7a9fba9f9c65d19e2b0f6990df026f49a5a80290c3e4a64e66bdfc3d726594048a" },
                { "szl", "723ea47712d647853cdbbe9e765b456390219767b6d9926805813cb7794d28b023c0a630d1d9501dbc04a86452baca791480e854e51f35a0e63a71adbbd0c03a" },
                { "ta", "6d4f48d94a1586c6abe6fa98ff96dd9ac7ebff7bc4ac59efe783ab5e28cd7851d1afe81605cbe72e9606dad2bfa8b9261de0371ca73f7ff00f92d863d8c8f64c" },
                { "te", "d805aa4011bc07880f69d892a10df64b2b66e6fbc92e3f981a7021a393d7ff6836608bb80a29d95935c6ccaf483e991b1b004f94c4813ffbdef96c7d4614017b" },
                { "th", "55c965a3c72cad8e6a56759e103ce17d81a4c567f3fe6ecde6c5f661bd7e7629b04c12ec81e0af337e07f8cbe25218fb1a1fe39af3086a233a946617df81390e" },
                { "tl", "7683d3b19196396d035fbd820158e4cf1b92481dfd9eaa542ad119da34bd0a1243f13577ff75ecddc56b5545d34a9d620d473bbc73cc51d05c9a046f89f26959" },
                { "tr", "466b89171a713bac3e9610185cf1b2b9acd8ba4d85618ddeea1aac0c0359e0dcc22fcf428937f48f999f6fbb8569afb7d6eea597ba7115ad565574200487ed0e" },
                { "trs", "06663c4f7183289473ef65e83a5fcc9c3dcd648ec1bd8a0553b7a048b8a1a810e95366e6b27effa578b694646222fcded83827e709b4b6c98a81105b9b43f7cc" },
                { "uk", "c686ebed89d0a2c86f9c0eaeed4c179afe40f4b6ed0dc406c583cd4377cd36d9c422dcda42b7695251b3e471e55c23e5bb8155acc5f2d970b48ace7a9d656416" },
                { "ur", "7b6efa9d88411fc69013173f8c0eb083ef401449a01cd78e2fa07f6d8480612a76f930c123cc310f1c8cfcb08728757a6d71d22f49334f46ce2a591934a97e4f" },
                { "uz", "a5878b6f3fde84c6d09f596c02e26288f8aa1436b5e58f62cacd9c483202ae49de619e66f7aa9cd7879b7304dc73c5b487c9628df8892a262f04f445bfed9788" },
                { "vi", "3a2f5f758085b40d31b49fab748d8c06b46ac3724dd96ae2af88df4d799c4bb92ddeaf867a9d813fc7f51cef86fa0fdc3a74cfff1323bf459dcd5b859679a97d" },
                { "xh", "9b1f83cceff023402699c4aaecdfb7cde3616d7dc8cc58c1578391a6f5eaf7bf55f87ffaeecf70f1a3cfac2a2d1cb77e7b943cab6d030d2fa836a77f9a3497c8" },
                { "zh-CN", "5f41018c735126f200c5c40813799580ec71a18ede10e09b21cc2cae19e9a9640afbd4172d8eee0cf45dc8875dad30bd61268f45f716a8066d954f82ac2f01ea" },
                { "zh-TW", "57475d00dadbef6085c81e86b80ac141ce3bc47b5280242afd73c9fc674ca378b7462b3bde945b7de4669dc2d3b663144179930930a42781c9d6936ee0c56423" }
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
