﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024, 2025  Dirk Stolle

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

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
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
            if (!d32.TryGetValue(languageCode, out checksum32Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.TryGetValue(languageCode, out checksum64Bit))
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
            // https://ftp.mozilla.org/pub/firefox/releases/136.0.1/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "4f8a2cd4dec9278e3b0cce6e4c300aa9fa058fef37263d39364d661ef8e9c491de857c9969f900fc39ab4b4546e15ec1848e1972d36c9dea6aaa5313b9ac9246" },
                { "af", "b57c784d9ead3e498576a08aa04014199c47b9680853b6f9a63ce9b049bd15e28ee4133782cbdee3038bc3f98dbaf379250538f96001f74c25c415aa51e16f30" },
                { "an", "29c79eb73f8d2de71766e9c52568231807d6bf37174f80c0143f78a3957700309688770c9d025a08fd3d9573799ac6e2ad6775b8e26a93f62761ff2f88e9526b" },
                { "ar", "50f49dcc1e3c94b8e983c53aa57f45ef5091f61df06d195183661655596d974bc56375631b9582ee1a37b4af53c8fd650aaaba74a08f5df65864e3ed00ef7f93" },
                { "ast", "297ab35705728b1a247a432bae56dc2ad88cb11c88b1595f4b22b1218b91650e4086795f014768b699390746822711ecb80ddb16cf45156a1c9d7b7fc1c187d0" },
                { "az", "e2ac454423502a1979ae9412683febdc2b411cb69b2c58e35efd8ac22781536f7cd690d8b50f4620cac69805111c701ebc1524013d98524cd63a2740abbb457c" },
                { "be", "0702e248c7b7e982116858d95151e3bea42b18dcbcbb9283e48e5049ffab1e3c306afa3af3ae9f701889ae304dd11785015c2c10986f0791aba77b22925a01d0" },
                { "bg", "35d29ce3bbfe3c9f99e08f0dba5a122f81f1397d53818d9acc3bb0ac2feb0ab2b66c962b4c8f4c991f681a9dff27acd81b79de607184c925a69fbe198062e03d" },
                { "bn", "aedd30b3b9feccd1516e00a0444a821bc38e6e555ccba4a8cc6ad47d566a9103f8284aed6f4ad5146b1b7937cb1388e44237aa5bb50fa0b9b5a90ec6e6277500" },
                { "br", "db01d9ce3291d6d713343d4f3822971f4825d8715cbfda0a04706da09f45a5820382cc7ef17e22f53f50965435882456e17a0a0f2b97a08fa378803c1aa76b6a" },
                { "bs", "6bf64d02afe4bf190140341ebe84ec92cef9cd9f88285335ece34ce3b7693f4b5c109acc8bfec66867342ce0b3000d45d2e3b7f7f49fa81e80a4fac945cf7102" },
                { "ca", "251c9d53b86a285cb6d92b12b41d9037441b89066085929196983491d5c8c2d3eaccbf86a066962586cf1454b43756b0a803484701a140ebf7a5d12c494997ca" },
                { "cak", "18f674ea20ddbf81c0ba3ae561d16171634ae759e96fd648852f610ecfed97699fc0fd4274317ea6d222b4d0b4653621296ded268d8638165b84df9106272f23" },
                { "cs", "5e52c40dfca12f1a3098316dfc14a62a671bbae1f65452eff47b4f7db218862fcb87843a6abd7ebb350e4578c0bc06107470db9e7f095cba11422edb0bb0b627" },
                { "cy", "426a02557f07310433296caa324f6e36c64f2971daedfad1fff15869e2cdfc62ffaed205e4aad6086df1310f9dfe2faaef08f69470b7d9a0cc8a15742db2c19b" },
                { "da", "1f428d047ad4aa07d7a0386daa42e107d88996ac9e8554f887779b17821d78444da5fbcac515b79cbc167330f5d96d4f3411efbc9d5eebcfe01feccb493edcb5" },
                { "de", "e6b8bb8358030788ed6eb75bc482328516f99a077a1be56a907965d697250067d3f04cb17db00e09db1aac838e46af3abcb4473e4fe1e0fe5213c49947253e50" },
                { "dsb", "8933a7c26b6cfa8b308c1ea2908993be0862600062292b4afc0c9934a3a878445e7c06c41ef9140a90a94d610693661296ff41950067ea0326fdf754198804fd" },
                { "el", "d6948fba9fc1eb0a508ccf537306a64c1bbbdd67768e8eb35d43fbeb23af0b0ba9f93d8a71c8183e5909e47533e580d7a85424cc17c02bbeb89ef902a645a092" },
                { "en-CA", "b5befd505d1d3c279eb07ab25f4d7505fd33e6c181c670602a09cc1ec42f172380ef4d99295080abaa41726cc359406b3468792534c69adc55ffcd13e82d174a" },
                { "en-GB", "a45501cd6d499c3475452c867e46ded58188f515468fb7b32d422f79a166a6ebf28e930f9025c6107b06ae3308542b34ed2203704c5dce25e017d44b5065aefa" },
                { "en-US", "523dfd5bf6287ddc7fea7fe59cf86da483aac135cde15510c5d98234790556af75e7e9b4444e16ca6d8cac5fac1066dc1de7bc0964ef94c8c84e97d403eaf0b4" },
                { "eo", "e9f0177ade7d5773687a067faa9e83467a9f61ea73e1186c41021534e7d7d871f29d96a59686f684bd177206cbb8318da06ed01101bbb4a27bd86b479de0a691" },
                { "es-AR", "974b3320302310ca41e83c3518597fe470ef407ff68e28efc9cc8bd6c28721fc0ba480188abefb2903bc058cec391eefbb2cf0684c0198e1f260fb2242c1fa3f" },
                { "es-CL", "631b323009962b26cae7aa3d7ec055fdf89f7ae2633d12d4c83399ead0719c524a37b2b46dfe7b346cdbb7da784a8e74947b43e47078ac23fe7687685c2cd3a5" },
                { "es-ES", "9075731f1f88b66b432b0d07f9af0f971d5820ff6cc94094cee3d6cae65b78b496586310ffb28808b954d86acf92c4c8021318f6a6716100a253dfcae743db6f" },
                { "es-MX", "7982165795e66e99313da49199f8a6a1a99f925bedac0b5122cd10d4fce71b4cd9782580e32837fa367e42596807445ef816641883dce7234f18e547eea3b34c" },
                { "et", "7739dfb907efd3c8faa3630df12290c3eaa7462e0fcee22785a9d2820541046bd42c5d4da279a04bbeea70881b80216d79f145514c12b9441ee7528dfed37511" },
                { "eu", "d234be67c1f847cfe5705ca63f780843dd4c74b22c44dffa7dc1a0c44f5ae867add8a95955df827e2cb0e2f93c50de146f3594542fefc712770bee0db5be03d2" },
                { "fa", "7202ac4176f2b59ba78f36420766196586ec11f797d24f58a54e6f9ea934b70a5ff67c6746af18d1113dd2dbdb1448472bf0834b4414011ea2c94bf2dba034f5" },
                { "ff", "e76fde59388156420dc1d69d1e54e548bca1903d6364dcd08cf0cc73df0029b2a3285ca30fd7c172ac2c914938414668385ae9b9de914040c64451ba8535b881" },
                { "fi", "ea569636761b5cb387340c814273d2682d313891fef22ece64ab220761f3acbcf09835fcdabb5e7341b63349996b667509df6486fb6395664bf22acf7f6991ee" },
                { "fr", "d47a2bb2760e975c0107211f08f87484ca38df8c98fccd974b33fd58594b591f38909784a53a86d536237ba40f2e1b686468f33e03322607c408a9fec3ef0d8c" },
                { "fur", "57c4edb21c920b32b4c4a931f16d58dcd20d29c4308b5d303beb0c9e1ccae24ba8d655a26f03c3e438620e7f4abebafec2ed41c69499614124ff650dfa852624" },
                { "fy-NL", "4b4bb4978728a7c02a22eb678955b991578deb48f47b5b3adff0a2b740e47313e1458929ae1b7c1be0367038756e6493f2a92da8b13f24af4dd47a4a31d5134d" },
                { "ga-IE", "2a250226e8c2db44d6c9a5cf9c61f851397062369ad943f175eded7b759b0dd6a301e00efa5690624c2bbe1cc1899cab0960b75222bee311d7ca83a6a90238a0" },
                { "gd", "677e1b2c84a27c7ef1e6d7dbf3728b5b0b86dfcd0955e51602cb0c6e4fb1f0fd81fd2b9ae52e726d07eb9dd40e202ed09a110a1160162b7644426cc9c18fc84c" },
                { "gl", "c7b33cfa31e4188a271dbbddc013c3ec9c120cefbf62b19347f7735eefe30a74a135527504b5286d6a2427cb771fa60de43de542e71fbfe75f113dd75ebc937e" },
                { "gn", "b138dc911122600787e5bdae50f5f765b82ec0be751d1da42feb45d9be81b714f211ba119963699a0d0db0b799501bb8871928fc13c8e68474e9e0ccf6bc383f" },
                { "gu-IN", "56276a5f6918b59c178aa2f9b0ff8446b2198b86975da05606acec47097628a64c361b5177eb86c0971ac09dcd1398d89086ac8a408edbf95d5b30f0ed4ba2a7" },
                { "he", "869a89a303f67c4675106f472887c029e0927434e3b29f4974de33b6019e159d8539e37b3ce416dce8ced8139cd810601aef57a03ac0b2e755a939df4ddb68f9" },
                { "hi-IN", "2805c5c1090ec7fd9f5c5156a3707fff2c5773318627f33abe8da102af5f2dacca31c3be6813edcc24bca0bef7b7d12e3106a03c8a42f6bd2f3e8a53df318e29" },
                { "hr", "0b4c86a0d34591b19a33c5ab2d05eb97b34880bdc2a7c163a55515aded1927cdb77d1174114cef21e705dfe632b1d38afdd3c36a94bfa1064271d82923723dda" },
                { "hsb", "3c1b8950333152eccd635ce458a76a187573a77df880aec744eed0518db715435b7ccd1f576009de2c0318c69acc1e24f9ad4aff934fca36c33d81a2485e3c08" },
                { "hu", "417593a55c5070ee5807008ede71d05087a7d7f8a40ee491e61346b0babea35ef95a5830e3518cdc3c6949e5b0efab030f7b568b11e3cea1d394582d746c5ec9" },
                { "hy-AM", "1ae3aacc1f612db584b04fd7aa27066009e5bd5df1d9e5ca6302e167c8c12edbfaf3122073efc4a24602881cdd7122d86ce8a02abf7081f1dc4c83c39ba62ec0" },
                { "ia", "598169e7aa323c729d107bfc8ff4e8666ecfaa46a197b2495379f7dcc22be9dc59f964a950884c51c463f2e43c79666c444498fd7fc8a0ae3c5534e12decb3e2" },
                { "id", "f0eae19e0ee6b45291b4203cbf071b87ec67a98d45375221f7a5e4acd360331644d95c9fd8883b1f6969867f4307c72eabc867a75da1bde6a7869ccb525e1c7b" },
                { "is", "41a8fb5ec061a696c4f316a57b0dfa6ec6dccea2317e342f05de8b362c12e5c2b11a42005495aff7da77058cdc7270c692ab48ad40cba7c7d9a733d37ecc8388" },
                { "it", "5af0dadf5b5c2e9fcba45975e53179cccecbd27fe9c4af7515c7d0641c2339547d9e8dff0380336a7d2f9b7377076bbe5ee7659cc7ffabf3edb7d55df2a714e5" },
                { "ja", "6c847f2e70fe0a4895f12a9f4e1e0b52a398fbd79de29e2298b3ae01358d44610b63f03ad26e874bffba1a9b08bdef352567f1cca70093a8446350e5458428bb" },
                { "ka", "049d8de61d6608e0199b03b4b1f9c023293a1459053a10b62532fce1595fe295cdee118744f7fcb6887664730c63e155294f0d4d0a76ae22dc574082116eb50e" },
                { "kab", "89a0fe9fa0e8a2e0db4733667a8891630e098e87ff235aef306c92fb9987a984dc85b2561dffc3716851f38d2f671b1b7bc932d9662589279648d05c722d10c4" },
                { "kk", "21db75f762f024afd34c84c04c14c98a2940e4baee10db14b71d9bcb68d4d2d3ea4821166e3612bc11028a8f5917ec609639f93b35a03ce0d625eeb10f6bc355" },
                { "km", "9590e3de5cac6d1d99b4436d06fb8d3dbeb936c3aa47b50947dac3daea135c9cea99ec808a588c72e0e4579520598d8bd7b5539e3ea2b8103d2d7174dd629523" },
                { "kn", "b6b2ec3fcb0298140a58d516bc1e2c3f4e2655372ed9894303080d4e2bfffca960247cdcdda3e8f4429805ae3d9b47a1050ec2a0150a0c3a4eb3286b481459de" },
                { "ko", "246f563458afe5e92f862639d08284ad806891ac73c78150f8a00180ba7aed3af85f83ce07950166731d4ae5a3d3a04069ce1db0403914f752980b91ba2367ff" },
                { "lij", "7a7dd9f2a8c463de76fe1792ab0ec9aa9c460d37328d86bf1b73994cd6824bb5a8e1964ca5fbcaf6fd07015e15cf0dbe006116ff991b843d53f3ed9775741c5e" },
                { "lt", "8ebf0fcaf7ee10f3a116d9ac74298615da6dfdf568a34ed66588d27868c8bda9d49ea0358eeb0827a46535c8e46f3f3550a70e1e4875b84e3472e6378ac4979c" },
                { "lv", "69c4f9e80a553388cd59d7e304982014bad1c405910efe7fee1239f469de32a62c39aa7e831849f94886377586c06a40595bbe3ef295cbc1095eca1cf4ce860d" },
                { "mk", "0cea83d53e22eb3861471510142986d5551dea61a68e02e4fa8c249ddd4f6848ce93ffa2a3c31b4b1e97ecd3b3c4c4c2cf6767e335fe2e01dbeb912d3de9371b" },
                { "mr", "a94d4d9e2391db26e7549920fa978ed223106746304b8b253c25ba4dfce297637fcca6c02e012827d0aa6dbd4d6dfe8ecd3080f0c97ed25e834d60d8417a1b59" },
                { "ms", "b23ff6073c25914c4b3273e76c1979dfe7dfc4257ab2b3bd11608d1089581823937efe08cc51c7f97775c828655c1e7b35f118d869f128d046ba211c4b063e0c" },
                { "my", "4853382903e3cfa2eec777277d711b227e30070ab8af0a88098eeeeedaa8021b6b3e083d2f7ef6539027091d02a330e112f92b64d3a964427b5954e31eb913fe" },
                { "nb-NO", "2c6bc53767679217fecdc2b20d7c711b5f55fb45bb52eab5bdb70d1ad26c210cbcbbf1bca2cef07ceb71f57a70ccf708884d52c6d2dad0e43d282a068f1c3dc6" },
                { "ne-NP", "781323d8fd5027b87688b2ea56375acf095b79967052768b8b599d21d564721f0788f7713493572f8f598104a732495b12347d6a4d5c2a777e3300e9ccee0267" },
                { "nl", "3b96a20973da66dff554af6ecbe99624318a18d22a56200431951ae5fef7b87acea8f4c6cd95584254641b900b3eecb416a7dda97b78e9229398731630292900" },
                { "nn-NO", "31a81a000d7dd9abb46478fa349cc79f446c357e8db6c6928dcdd908792ce376641a5a7278e1b0dd84ccd5dc4ce75f1473ae8004ade7eba3bac457dc5afed549" },
                { "oc", "4607b4749e6e2f8926085be45205149d5dd5875c1b61809949f39ddab71d5f552697dab692c24b1fca6b2baffce8a7109cde999316a7d172cf1ac7b2ca7bdc05" },
                { "pa-IN", "8b0365e1ae0e6af57afd76d331361939348bfa3c7c8443c9a5abbc5df8b703837f0f3e202fe48519d1b8463fb863a835e68a7487f0d0dd7fda03122fd3c8364e" },
                { "pl", "fae04ac1eb81cdaeb59e61f41c75dd9e81006ccf98997b211ba4a0ce13dbe07c6ee07527b906cecbd389b7c2fc428f51f4ce9d1a73ef9fb6d7ffaa35a49cfab1" },
                { "pt-BR", "c45cbd3faee78bcada60d6bc0d18a7d84b4010eaaa9b91bb9ba2e4b049bd99c7fb2361c330b3c9ab1a4b49722dfb5bd6a2aa0b09454d8100bab3bbdedb812ab2" },
                { "pt-PT", "63af9a141e9cb06bda260320d03501213025f6b79fbae2bd8347bf83f18f379f2b7676104d1c1095ad43f31d5485b75813fe802ecfa446e99a1e3f45dc223e64" },
                { "rm", "887c1ab01bfa787596b2cc76c9353a483e10f0399128a672c36ef1257ad4a80c33a92ed5b6cd882a6797df3da3a713ebaa6c67d04dfb081af31cd80006275121" },
                { "ro", "a62bc9fe63ebb147d596c289f1783e56a58eb0713aae160175aad95914953fa70750eb1298d7ec38a234feb8781d5758a627b308fb135a6c7ffc31f1e3fb1283" },
                { "ru", "9a24723ec9ca37a815f97ab40fcfbf5f4e48b63c22c34701d1b3f7a8333de353d1923f7ee51aa80d2579669089d3da39cd85c0c167ebad25eb6fe3db746598e9" },
                { "sat", "1c51006778b572f79479c6c8d5492425f7fe040c92951780bac7186a392a38118130f893759c3c6de61961129a41103f6ca8587a1c53cd1c48ba5e15803f7792" },
                { "sc", "9e67d50fdf876fab447557620b86abfb39700cf08325e794879265b10fde9f18464e1937f1d3433d39b7a5ce691d4291f2c2b67d281415b7eea68671751b5bef" },
                { "sco", "99810ec806dff9b4c0d14a93f1611e25ba384528523e83d5758109e7942a54917af662efa937db653a6b43e4102922f01647214d435fe32ca6e5f4b1c4bcf483" },
                { "si", "ce7826687c2e28e25323082788704a8d22c0b8b930c0f3048be176cc23b082ada19cbe6bfc3181b07fead422c04e9cc44c548fb025077c0ce11c08f1e7f643ca" },
                { "sk", "34f491b9af1d6fed7d382918fd60057b3c0d9d4e10ff29a0d773262d6568f883a841499a6aa9087f00ae2d18638cd30bf7580c32ab4d16154422ddd98a436cba" },
                { "skr", "2f776e4ba71394d8cac217e39c627dc3cd2ba459d397454791d64cabc7cfd517598e8b88daba2d33ff49ffda842aeafcdf7ec5afc9994b1bf3e370b4880c2cb7" },
                { "sl", "28c3bd94741633b6c48cf7f10fc80fbb3d266843b5da7cdec42924340a6677128290d0b5ff2375b880f801eb0a113ec6287f2b505f771186c29ca53e0d035942" },
                { "son", "da01d6a0c14d36d74f39c3d3664e6bc05a81f4e275db3c9410029e9787c053d8833e52412e7ba01a606557ecf3161b11389a25f40e788eae377800fa66560ca7" },
                { "sq", "7438eebcafdf624e997c814816e39c4c0a06f71da753a13d2b6c4c7cb6554b18323c021ff3cda61782fdb86ae8d48fc7aa8a9a64e696dfb5890d2748fa9cbf05" },
                { "sr", "735c7935982176e7fdf9583d39f23309ab943c0f15ac69b56f15992d0f651349098707606c01a909817894758e265fc20a0ab71a6df8de9e38bb3336d30619ac" },
                { "sv-SE", "50d321ca0d1f7a5e75a30ee2a33c3d78235838dc49375bae3665a3d52c56e17a516f7b0008c863331682298dc39f0bd32798fa0da72a6ca5ea966c5a57f25ac5" },
                { "szl", "49b56d5aa32324de88fba0a3ca7f4f5c2ed88e59ec0a5cd4fdd681d2429654075bca9e033d2abdcc1674a0294090ed70bef3166848aa4ce2fdb8b0a2e26a27d4" },
                { "ta", "eb61486b85e14cc7f0903a736ba24c8b013e624b77482f8bd827a3971e50534316e73c8c09d63252f56cdf47dfa641b7123f7fdc065a6bfc0906b703044024ea" },
                { "te", "bef8342148e3a41261c4faa02117bc7f9bef35a0f94dddef9a81e6e4b7d0d23b4585cb6ec61128886b446a6800243ed908488437ab2325949af9d997f3e588a1" },
                { "tg", "25d2e5fa9e93abf4c374a0454c50e0b0a43e9f9470eb2e90dd02b34d909df82f96b0dc32b9050c04d0cf2fa0612d9fae0af0c452d31732aa434f8d8dada8c467" },
                { "th", "edee907a70e56d398004a86ecd2e7c05c739a2868c0c8c67306063202cd04bd2c8a495c9e9f78ae49273a941907a835b10616bbfa77652480dec141ba32e0604" },
                { "tl", "e68d84c6da3f9377f3e6cb015b80d75df9b3dc3f8dbb756d27b107e12946457189e3a88c48761320f48dcb02cf723a21313026d0ea6384fd0556f159765f9503" },
                { "tr", "ce1d29ea7e4ca089a9fee1c9aa5de27bd750b14264a1efdeda1a5c759fb6023a1e9e464a2f8f3d1ae7625b7e8ba174fbc29a3cee2f2bae20a8e68eeafe42105b" },
                { "trs", "2144da91c1a2a655b2e679a25251af256a54e65256224b1ff2d557b1292cf34482dc2584e8dd92cc25c492d0bcd16aa21e7799a4488f844004f5de496ecb26ca" },
                { "uk", "88af56cbb4d7d03c42c74501dfce083ec2ed01305f14fb806096ea0eb1785e2a2039aa754aecd6fd001a40393528139aa321f399d03ff30d0599b8f6477f05b0" },
                { "ur", "cd388e70145a7acd7e0ee06034885059b2d3c1ddae9ed8223303416219d66668f9231abcbc9d939162caef256fe779870f58b9dc36ced19b5bede914e1b8a9a4" },
                { "uz", "b31c592273cb27a2b7e558cf6b8f516a4455c562aebe20ab307674e77efac321d7c83323a3a4da95737e1f6e0edd535c530cb40953d009c2dcf34b405bc64038" },
                { "vi", "cfa90a7f9a561e21b2ddaecb6fd14799e6c7b19e15ff2feceab52c76ee950b2458d311209b9c40f7a3d173c866e8e371fcf47e4c3e20c9df977405c07b42df3f" },
                { "xh", "9c55fbcb184f84a3bce7b6f4424047b9d4c55a55e7ccc208d0827951b8f8c183a8038599b932c837170a0f450a366286a1f0070dc993458c301f6e2a1de54e91" },
                { "zh-CN", "bb63fda4f38f35b5b5cd24687a08ec45cd96ffaa45ca55112341ac77012598d429a20cfa82e706cb6a71fc5596041d580c5e74f76798bd89a149af0f511d36ea" },
                { "zh-TW", "f7b80b906fd7f3d37a03b6edc5e5b85c01f71fc53bc3b48ebf66a727da75851cbcd0ba91faf9541c8fbf4970a94df7f73f413f7f8c84b413af285fcda78e75ba" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/136.0.1/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "24c5da650d764f29eb780de9bd79563bd3474a9dd070ea1a4f0bc23716cde4034108ecff41046e6fa0fd1775554a3c95bc076f4a95f495e4f92d1134d0a576c7" },
                { "af", "bbc6a9443f90d4271bedeb920964e65b1cd5ac14d7eb5c4fe5acdb432bd9de8e5ad7d03419797ccbe7b3f321631b73a7a1dd99d81a964b973d8e29047ebaecf5" },
                { "an", "cd92f363a34df7a0ddf5ab403ede227f54619712cab608a2f62979a24f5a16ad5cea180d9a97da957973745207b39a67f9ac206025e96fcc166d0aa65ecbd2fb" },
                { "ar", "776c97e9dd513d651abd6e4a0a8220399181a74f0d2432cb77efb744b79dde8c11ccc1663f0023e43be7949e7961ae3f0deadfd6ec342a4f30a116f0eff684ed" },
                { "ast", "46cba00e8848e49692f0093f1e900474fa67478b4f02dcfac8149c876d34f3805c2152d3661e05e72b728ba304d66ae0de331f557b47fcce082bfaa0404844db" },
                { "az", "9439ed13e7f2608b78ee0156528c6964377dd853cec5b4cf03bb1ab95012ae1bf41eb4d926a2315de0a8e99a8b8e24237005335f70d4b653dbb6c41a3993b8b7" },
                { "be", "1560925143a954daae792d60105991a5cf82ca885ad47c4323fc71c7beae68f0ac069619fd54d89499aa1da80fae64200877d1bc5879cb58f65884e40f4afca5" },
                { "bg", "82c957da8ee6dea43055fd2c51a161a3318e4cc8379fc4fc50fde1ac42be3bae8446fa26b104436a2ea9e2a2d47401c1ab95863252512533aa61f5b84478f2e8" },
                { "bn", "21bcc7ff8a9f0ecc1b7dde5a0c80bb00c6b520f3824b445cc466eb7801959cf458e3465ac4f43eb45370f6db8114749fbf188e73938f3750644b8a11257a3331" },
                { "br", "5567726215dce5a23a5cc00df1dac0bd114480de709a4f16c5ca5312d3287dfc837c47e36e7d8505e0a85eb0f7405cdc387b800419df19b7332397b52a9f12b6" },
                { "bs", "9edf2631d415fcf72f327cde6f02bc0b0b19476f6dbda58bfbc51f7a245736dff2a278275276db34a3f7d9fd2f3877dd819a5bc013f4af357ddb9d489a6e2ba7" },
                { "ca", "a72f68237913a61a43cbdc9119148454faf6651a33866b2f92a20872409c4f8dfc227485a8d63259f9f490a323c77b94a1d6ad2b0bd7045777745bbc4441f772" },
                { "cak", "1d876a95037eab94e4b209ba049ddaa3716fedde50e9df09d8df438569712624c840342007e5b6a9e558e67da8090c6ee5815d8f669694d0646bc00b9bccdac4" },
                { "cs", "912df870b690cd70255a6d5307316accdd7d156159b81a3ed37735ba89e5914854f555d9aac95ac1b9c867ecb14b87fb4c8379ba31e464d3e7f8d91b24dfdf35" },
                { "cy", "d0d473f0c441e326c7e8cb797b9535683cf2f33944fd9eb6ae0f16b9e6aa298d02bd716192a60b4c42ab6a1af00e127e175c113c3bb579b482fc8e0b9994ae6d" },
                { "da", "3392e87c5abc4b2053c19d15788bec8be3acd290821e66f6910b48938b06247f7f49f66fe986e33e360ab2d0477aa2e3366f1930eee30cdc490b81d7e602fbcc" },
                { "de", "0ddc3fb07f816862eda97ce76cf107e7400564b38d1a187ce459969853d23bc295f999f872d18f8d6296240e7d8f184d21959f023876d67de38499e640e730b8" },
                { "dsb", "cd0fb7e66a1887a4055ad49420714a9fc9b0a0de8b484da0a4e0f7dc876b0b7f8a4c78846768949f3425424b7ab540c5f2fa0fc1d51c351cb1b40d6ef2f39bb3" },
                { "el", "8a0b6891f6b4041538d09f7fe9181fb130178220cde6fe41b8e5defdaa40c622700eb8524166eda43bfa4ed2df3aa1d89039985a62d92e576a46f0a0aba3b278" },
                { "en-CA", "b51762d3a0a8e46de0c824da0a83e33c20d6e7d06058e0c9a9675ff3fd66e622f2f9757d8f4aa78deb3a945e33b146472e2a3418da9b7d10a677748ab99b0093" },
                { "en-GB", "57e3308f08ba1db49a82d8e30d80c4598ff82197fa46bef22bf9387f2e453c727d8dfbe1e46ec79cc861ecfe90edcdfe18b7cb4a29e1c91426ce14910d1a2203" },
                { "en-US", "038ddd3783b6eeede3253398c9de2214f032ad22e5620bebbbc9a6bf67af57568bb4b12431f23a6d72cb82ec203e59b4d99d4b19dad1e089eebc138766326654" },
                { "eo", "79e67d36033e6acecab835d7284ad6e6048e6100003e3b24d6d53438bd532963a789720fc04e8ed67bfa96665836bd3300f0a6e0ac71c554825022627630cf87" },
                { "es-AR", "e802e4e0cab1362fdf15a3f7317e8dc35b9ced24077e224145791d5df16755422f29a83a067b0ecdbc2831f9119834c91781ec9dda76a86348d06f933ff22cf4" },
                { "es-CL", "47605fadb229bfe4aa2b7b50d6e73daeed653b6cadc343a9e8c8930cbf8aaa2eed7059dc64df89bceaf6f3652aab8a51fe86dddc8caee4926f8d23d8229d74b7" },
                { "es-ES", "9c4225f6f62cac6451addc216cf2a64587b51df5c42f69c67e0403d18a87b0a596faa56feea3073f0f7c2197e2443ee74375eb7f753c118558497bf4e6ebc4c5" },
                { "es-MX", "042d5f7d12a4e2bd52d429ccafb4deeee0fc367c6e73ae60ecb72ba23f366147982c307b91a32d62b5aa84160d4ca44cc2ce54de75a13ae33bb4a1f8b0a6439a" },
                { "et", "c62f1049d0d8ef817438d2b2d2649c67f3cc246a5d2c9281274b15f4f39eb168c241f586cc67d363e2d4f68957eb7b409409f393296c84e7ea41102e720161b3" },
                { "eu", "9d0d1e6db90bace1284ded75b5d3fd80b078090c7bd07445bf90f01d45f6e7ff70ad5d72f2eea811603794b391bd844110ef37faeb87f49921d4fc2c157762da" },
                { "fa", "a071ba6b5654f14d9c03c0fd40d962870bbe8b554b56c2972dd42a9bbdfcb55bcaa8e58c96f23ecf9f8939b2ca8418fda4b0f75f25f237feb99c14811e96ce2c" },
                { "ff", "64cb41cbe29d007ae27117f80e8a1aaa7a62051d188d3d82dc393bcdddc794a8820ecd1bf9fefcfc1744aa934c7763d669bc3bd7b4293b269d3a730ae69cb64a" },
                { "fi", "020f6619ed445fffb6db75890149c78f925d3b263b577eb410030ad56ec6cc1fd78aae50ebe9f31ca002ec0a40deec553f896af2a110afaedd4cb55c70f93f3a" },
                { "fr", "51d7a7f99d04e40f727816dc0c15fab680cbdcb4816a549f1cdf0e6591046995ad371797edf0e9f257828048883f483e72903082bd3f1c0221c7b81a309993b1" },
                { "fur", "ccffad6231f38583a391ba969ce9a1ed658f670bfa9985dc689df46245cb42be04ee3f9be6508409bc6d03dcc1f6457b0a800026ccaa2764c9013f0c818c1ac0" },
                { "fy-NL", "f6fb08258426dd4b35e9c0eab4da6d1671eafb97ee7e1c91649b842e4c8a2409867cbbf4e6118babe40b601245522005e85609ce224e3ba9bdc9f8f7ab24130d" },
                { "ga-IE", "d1c0e304c748ca05a113fe71ea8b80796e8af4be018cdd3c641c975e28be801b62caae6ae830a1e6fbcd36040ce79f797633703b81a81fca8c5c7f244e5bd5f4" },
                { "gd", "6bdca05844556891b3969b5fa46936b01ff3a91e91be4906d572a8d577c01ac1c876eefe7d60250575e2c7274d70812653231627b1e43ed0deac3240f888f7c0" },
                { "gl", "329aca1c7096481020761b222258bc2dc300c61fac67ff3133ea8896f85103990b7387fc82a774f9f1f1024e9a1b5a1064219a4a9a692c435f09db6d129d8921" },
                { "gn", "162b5c433601fe1af8c3e7ab95e73651813ca4b399939d756ee76abb8700411903379fbb227b569a1ff31202b212c88bf82b2d6de7c9f44ae0dc74c297a9836f" },
                { "gu-IN", "78d4f440f2ebe30cfe68dcad9c02f0c3394793ed624a2f9675d14008535164697ee72aa8d4109e5b85930f7a0639221992ab51d39279125494f95e0f35cc93a1" },
                { "he", "ca0097f417446258df39fcea55178b9f06b1483fc91edea858b77d9571e3e69c4455c78409c979d17f8f140ac7bc2dd457c0b1bf12938222253160cf2049ac98" },
                { "hi-IN", "af43a2cfdb4e697597c2fefc26832f9ea5008d6c97deed7d8118c924c916de1abf8b32b9f1ce6c914ad3193ce94cbdc26eb948c0c3cfd1b270a278f91ba1ac07" },
                { "hr", "2c8f204742edcd00081100784b18ed81464becbc2ef805f4e46e74b3612ac0cdbe562bbea0fc84f9153f26a27b2a255bd3ae17688e0e2274f49aa032ed8f3e0b" },
                { "hsb", "dfaa2e6ef7b6bc36115f55391aa770a961d82aeeeb698b97bbcd8e8c1e2fbf19ef1e6c7db9894b8da19a2fee3f599f8561aba4bc376f40944226c568832f3edc" },
                { "hu", "ce020ca590cce876b367a9ee1718cf8d7a0affe2be3cdd06570c005d614372be1f17f6e910b7806f7d14bcd53c70140fa66db0dd06167759010f81fa5011abf8" },
                { "hy-AM", "9433998f26d3b3fb8d898ddc30a9f4569e653766a46b1e1da0dc9245d7b1d9789d430d91a281d029354437405511878b992784c832cc8c3e04d458c6b7edeb63" },
                { "ia", "516cb748179fb6bfec03ad4fdf2baa9248fb1ca40f7476416721485d165f2f586d2c67de65b4a170b665dc0973d717313876b0e23ca39a2f6ad0dc3c44db74a4" },
                { "id", "80a1e4a8785dc561e3a769ef38b1b027af602cf73b198c5dfccf1380c5bbcc6f2117ac3399865d81fa3a47e0322a836d8fe6a4e07c105f16548274e069a752d0" },
                { "is", "a8750af9b7bc713850de2a427d57edb062a79db2521c0e1a4d47097297449c88d4c33627502cbc47d9a0737ea93f3181ae84dc831c44e3b8052981c0794f7598" },
                { "it", "43de7fd9fb4517c1a63ffd9e399c19fd311c97bf0b69c46e31e19dcfae0273bf8870cd21caf8cff7b490e274984d0a3ee2f615b6fa16255eeb94fc7fba411757" },
                { "ja", "d6958cd4f7cda443d248d2cc7458e9b20077a5eb27e4a288025485758c4b54c2ca45c6f6fb00a538b8d046f734098e1944a035b8a2b26c60497bb670c9edd55c" },
                { "ka", "9a11fb8a69a2f02b2f84bc9282b9fdbbd8bd2d68be79a1e240b7f744b53629392574af68d47508508ae88ab225680b62012f1e633f4e2bf8e7e00b9b1a5c9803" },
                { "kab", "a2573dde9b550577cdb9d34accbe64ad73e40759ab0ab4440311958f78a6a63fd50a493f686b5f92132ac54c00b60fb9e6f9cd06aba6d18a746bf45d3273988d" },
                { "kk", "10d00f3e7e43a53ecc8d61d7f1264a1c9777fc87b61ffd5760f90d290db0789cea4355cd92778d3fa5791b12384bc03d6a324286fd74a0458916e68fe614b682" },
                { "km", "743cf920574b47149a4570ee51c5a1b23aaac2a34646b69cae70707507ba73f6a4d118ee7d1e9ed48353a617fb2bf11df11e0481c5a2215663e474f8cb73d848" },
                { "kn", "a55cca6c3e1f1e375fd55ad5965a381cd80b24c4753b08dcee2c6100f6c7d50bae50673a40040a3a638c25c74c66aad228cef6afebc8af737e71c99dc2e6b140" },
                { "ko", "c6ad65197b43aabaf45e59fa074084d0247d6a50c4b9881920aece2a80da4c255dc43e362c7d793480a1a5afbb4c3186b8e4b4aa6eb7ea36171b552947d677ae" },
                { "lij", "8b20e12862e1eae0b0a80ec116dacdcd20fa02f5d698ead31ae4d8d6a11723a7d91387187f1919d9f6cae6df23ddd4d59461cc1537ae0b8160d378d8f913397e" },
                { "lt", "c7238cc364798614c7790d1fcb7bb8cef1902b9fad52ef8b698b9b42ad898de2952ffade131753921a21ee26dd973f0975cb4d13ca2b6338f305c868bbf6583d" },
                { "lv", "8c7770f083b8d54fe298b918d36d3341fef999ade39321306a03c293a22f5e1fd901de4801cded8b6959361d340227f66826bc25d4f8be8408f0d554099ad6ec" },
                { "mk", "8b0e68f86725ce6a196bed7b61726caafecc004f82d88935f692b39cecc440fd856e0d590cd07ceca7b1eff80c3dd245353c9155e4ba87a970ab8c38b2867c76" },
                { "mr", "b53c114fd407e04923f5e155ce7246678104417cc2d34bf9642fef3b18bb2ab65748b72e467875e52b1b804aeb5dfd4c5a2d7028287170d78f18223479258737" },
                { "ms", "17b4b654930d1aa4a07bd1ca2076ff543c7c09cc8280fb1509c5e68c2913a16a1ffaf1f9ddd4dcbc9adfadafcbbc46b30bfa32fe2ea54f23c4eb2853a2407d32" },
                { "my", "aecb95789a861baebcb4b1f5e0b722a49634bda9cb3ccc821be631db0b1aecb402790b6d13ca08ed918c9161ef61baa421bbd1458b17fc1a537e71c28553557a" },
                { "nb-NO", "0ae231020c4a32a5dc1546f73189072760c46950d0280f8cc8351bc364055b517fafaae3bdf368528acaeb509cb2e2e0368b4e9f6461d2d7c1fedf7e99aaac16" },
                { "ne-NP", "9e0ebd1d3c20d5f7592b67834d3bc3d5c572bb445cbfe24ce54a603e9c4c3f0f009133a1d505fa18cba43333d126783e7c9aca3a05e90fc58cb6b42eaa9f0938" },
                { "nl", "48cc47e73b7ac1225e98a64e7524bab4914d44c41c724fb40b073916559c14e4b9163a622c9bce44d568e38d9d9da76117ebb44825220dd4486cde1d27781eae" },
                { "nn-NO", "e858c15fdea7c65da24c583c55e4ee5f686b4a9acf026a6d3d8aa6aaaa71f916df118beca77ca8016fb3117fce739e17749c65296f94e20c4720c6f63ef71136" },
                { "oc", "8067670997ce268ea62f7f57eee5d0f658e048aeb2bb3c5fa58f819c8495b87e22eea9fdbb1a418b3ee053ed70af380d420ae1fbf15b44a65b601858c183b0e0" },
                { "pa-IN", "4e8bbeacb667812fc444b40431c976328676d56f0e9d2a93d9cf5c600d1052ecdaed3c578ceb2d6c47f877b6e42ed011dda3c64a91b1d91af088e39d48895cb1" },
                { "pl", "62bb0fb376f382702c5d6dbf304b878305e8762bfd54063092cfcf177cb45fbb5b60e8605323e3f2d8f425d8086be7f7cc68ca076990ab23331b5263a9a6e39c" },
                { "pt-BR", "2d3de9833bceff291575da2a96d4b1eba4d3318308bef738eee62fef0180e84274a8cd1944713a667f949b9380c76d69456f0fe4697cb63f30608a232d5d2a1f" },
                { "pt-PT", "7422165b1a42442670c73c3aad2f8cf851dd2d2bb786de444433f3e2602dcadaabd8d1a4ffbed4699e3d44d7a48cbad92fbde42e0b6afc61e268d99ad56c8713" },
                { "rm", "89d84ec310713a0894505432401a6ac057ed5a74450d58938cb443c0f49d69c7941aa325ba2af7d60a0e55dd0a2a1a21d4b03ff54bfb9371b76f5cf1eea40578" },
                { "ro", "2e4c3375e6438ab63b3cd0637e4ad91d79b85ca91d79696d4189879e381d75858da1c065b24f48c7ed2c1f7d8756bb81f90f3db6607890cc44351185df7bc4e1" },
                { "ru", "91b7985804a882136ad47a31ca47478c913aa9d01e9060cfbab97de246e70771278ec49af0448eeeb085e1f1adce630c93af45acddc3bc836d9f67cd2d938250" },
                { "sat", "db7d2ef9ba199ac53b5b580241e9060a526724e3d3548016cc9f7c3bc61d1cf62c459b7e2978c7af2155af8702ed645a001dcf4f0b93cfa43ebb9bd4ea5c38c1" },
                { "sc", "9aa96cd926dc3187870673b7ccb22b54d35425c2ba8e9d6c98dbaa4f40e690687f38b98ad58be38f4841797dcf4b66afdfb63cf6984b000b01a8b0d7a7c24b03" },
                { "sco", "0e672e4453102fdc4563f8942734004d18aaadb5faed42e85ebf394df3de889c93ab79750eec812f21a55073dfc2e32f2eb3a275cecf3809317f727f4665a74b" },
                { "si", "b665d7243eb4b842286f1c855e2e95c7d6a46715a2e8ced441d61053cccc71eb789e3012d84aed89203eab81c6fed0a80fbda97d7a299ecc60c707d8b48717c8" },
                { "sk", "65246bb469e1ab392568588ad98ee4e28c3ea3a61ab60eeab04ce25a8f68cc10e3a342abdd3a27dd2f3cbdaa2f2a1a947fb71cca96193ff678f1757468d3838f" },
                { "skr", "370d01bcc19261949ea27d5bd9b363208e732da82387993fb28a34722b4729ac67a13fe8845b6af9bd56c64ab7d4e8d37525b79de33f392991841beb16a8158b" },
                { "sl", "a88cf53d45ef1b1eab8dacc78a7b24a3fff54aa25ab59d82ff63c221fc3dff31da698f7b8eb37168094fd5f12ffe9173f01f64bacb01e7e3fef7e4f907274d4a" },
                { "son", "4ca1b8b539459f543ff328a045e8b087ec2043aba8daf93dc0407c03af6ef1e628ef49c18a26cfb99f14b01fb71b85f28dd7087bad369f5740bbbd6c1da630d6" },
                { "sq", "0be92ec469116d2d11badd75e29d02c786000c86e0a0a52299e8ecdc9c641f4e1fabfbcc3e4dfb96b491fbbc5cfe7c2bf675af28113eb8efb51c615c8b75e4aa" },
                { "sr", "53da230d8f23506a4330aad97a821b3b4957ab94d8fd61a62e2d84c85e9235d0a011ab2561f68df5185293c1e28231aacc412edf02b38a81ab5c35be008dca31" },
                { "sv-SE", "9624cabdf772f8714309a6380776a2a348afc2f984b6e17fdcb1500234cad828194780ebc6ceb4cab19e12c1d5abe8c520600d8ebbe32d9933d9ed3a2ae79bd2" },
                { "szl", "4fb95c193de2ecbc5b25b0ebe1b58bfe070f9a0f38d7d604491668b5927a11d7483effaaaaf98ed2136382717239feecb7502f08bc80d40605588ceadd9ac46a" },
                { "ta", "868cf0a616dbdf0040e31bc92877ddc7d398f33406dd8c2f75fd3038679b01d8b851473f059d3ea81b898e2dfe553b9d880dcee9e7c3bea447698d8ca905316f" },
                { "te", "9a58147e1b6df1adae5fc987d16a689c51809e6695077b51eefdb7b8470e508423de34f77579759f379e761d5ea8b4e10da0c62367ba36a5fcf993601c00ef8b" },
                { "tg", "9755e0a45ae5dbe82dc85c559faa058d933499ff4c0936bf59152443660e47ef633c26c17d63ecb99ff3292eec8febd3d2a001d9a5289ee2b2e2752c9b6e44f4" },
                { "th", "7b1dfb8c96f1bb26a7e9a7fc098528d640fe4869427d4895bdb58257eae9eb01b9c44229e93212cc1a79bd8336f5f7b2c169981b6410fb0acb8fdca22518fe96" },
                { "tl", "8cc123a9bd5016e6a6deaac620980597ebd9a55e6e372e6b131dde0cc5569bb959758ed87911f8ddab0d8e7bfabce4e8b092114a4468ce86fc7988e314ad6831" },
                { "tr", "a61b07d3533f7d91dd6fb7f5653df1a975b57d997b7edd9a3b32ade9648f8e7b1dc86b7cc20d5f62c8a7b8fe8dcf69e24edf2eb781037e0c36fa7dd12902847b" },
                { "trs", "d0c8fcf32b3041a9d53a57c3a55c7fad7b27154b1a39b3576d24a133e36c23ef3a0f57cf3dd645fbed44c39e40921813a0de299f6f3d0b926c6454ca5106e475" },
                { "uk", "252527b8a3d7c3a217c3328eaaefc7afbc8abb961c828a70a6baf84c14a2ba76ee619a50890019550b3f972e4e416289391a05e7dbec121282f295adaf2012c9" },
                { "ur", "b1d794d3f72856329ecddbeb0aedcc27f2ad891fd8e4d5a7ab9fb1508c3bab161654b6f5d86aaca3d2df80d89eb827cbe14c2ab59249737ad51b06cdd25dc72c" },
                { "uz", "6875a32da90d7eee34317392a1c735483e0a9ea5adff00b6567cf3df1c001f5b1f269aca41ba49fec211a5f13a209648c24c79fc3f42abf4562ca2d23b5be081" },
                { "vi", "ea44a4b8d4cdd1e67e29ce53719290c47dcf46e18949129befc3a602b38e2113cca9ad79c4141420770d702e09a12346b67b3f59e43aa7a87dcdaccc3dfa3106" },
                { "xh", "5d89be49a8a485869df46c74ddd5783358d5c329f49d98f6a62c1fa5ee8674cdb00a7ba2deb3853b572886ced65112644276b2e1e477c34938ca0f01b5140652" },
                { "zh-CN", "16f703ab3f934bce5edb9701fd6f0c386b9c18ec2edfd9403eb15cdf10bf8baf0b85e6ced47ef80ba5a65235e150cbd55bcd9441350612171c95b93767136ed0" },
                { "zh-TW", "6ad28e5a4690ac3f3f00c81d8b80cef894d6b419a58c51a96deb55584bfa6e2546cdcc517dd8e2b0ed8d2d864fd171908946de614ae84a976210e28a0b26ff36" }
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
            const string knownVersion = "136.0.1";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
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
            return ["firefox", "firefox-" + languageCode.ToLower()];
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
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
                client = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
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
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
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
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32-bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return [matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128]];
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
            logger.Info("Searching for newer version of Firefox...");
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
                // failure occurred
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
            return [];
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
