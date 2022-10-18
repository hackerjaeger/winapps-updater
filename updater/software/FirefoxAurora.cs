﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022  Dirk Stolle

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
        private const string currentVersion = "106.0b9";

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
            if (!validCodes.Contains<string>(languageCode))
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
            // https://ftp.mozilla.org/pub/devedition/releases/106.0b9/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "5142f63c1997c382b1bfc22c3d7ec8281506f0b8285cd8870446aa128b63c28e58c396b3bf87bb93e5a543f9f35f4ca64fdfd3c17bd18b86c70927d9f09bd517" },
                { "af", "115013480f443adc05adab277a1d53efc705daee10aacbe425f2576c61cf885851ee6e3dbce860f4042d72f2205a90450673ee895e86d292ce1ec5d31edfb23b" },
                { "an", "c0b9b0b3454fcd03b4c2b33aa32579e8d156939e5c69b648c78057b4dcb59eaafb063a4e164b98afd38ddb655b35f55cda479d91e3a4a8dbce8d7867e98894e4" },
                { "ar", "0d0e4ffa53a7ed772b3f059d59e6e9901e0878e3d76c7c5f37e1e98ececd11424c2c6717c63fb2d22fe5af429f5ffc8ee52c0887d7ac9bcd5eb61f88b6365f98" },
                { "ast", "0f072c561a391eedc34836e61c3522a463680f56b9c1b6649a7b0f31c65dbfd5085440b62c00efc08b997e9ed92a923b97ef58375d966f75d5a02b84e2d828b9" },
                { "az", "c0170fac614d822a62bde66f6e8d3689fa058e8175f061a12f55de7f015bc38c2e811e9fc0d1f9858581cb35dca2ff3fd6fe7b77b5479d90fd0808df9af1a9b1" },
                { "be", "477f2727b113567eb6d49df2c914de783284e74506cf2ecfcf39f1d10a3e18d1ebd786fad8df9377433a25475a6c71ea36ab3cb780c55065cdd9fdd1ad05665d" },
                { "bg", "15b0aeed5759dcc756f759c14b593f330aa62f0b0401fb558582c2f8bfdf5058df23db53a3f3a181b5c9078005b815c95bc7fd6f9bd5e371ec6c6c2821d21d61" },
                { "bn", "3f8fc6ee3b407569fb8f178653a90f3fe01fd2ea5960e3e114395647456ed1fffb1b2fa406c3e7f43d910fa1ae8ee6eb5fc7fe3b6aeb840ba541bb1583d907af" },
                { "br", "051a99b83bc350bd22283818eaec410c2b18f976fb5f8533e88d9b543ed159b23f9eb11ba75d508bde2dc75c2f8878bbaea4fc8ce77720b972e78f0aafe3d4d8" },
                { "bs", "471b9f671fd256443a6ded09cf91e58450c9c325da744d3f3436cd664f936482aaa6e30b7cb9674d299dcf16cca24223103af66ac7f707a03c7f522e2853e433" },
                { "ca", "0a76eecc5f1a9a4aed68fd90d6e7b6b59b5874f415083e160a2e84094217a58e5243c9a1ad765597739596a0e8d38f42d60572b0629b7614b4901bc2defd8031" },
                { "cak", "d01006939dc4c15820a6da871fc666b62cf46a315aa8762397dc7a80b32329a39a5ffdb97a8a06a2dcd91781c0f8c78392f90a04b74cea52d9d2882ef098dcd0" },
                { "cs", "4a2f74dfa1a13e3e542a961080d017c2a14c442451f8332dfab8534a52809177854b77a216bcece1e8648a6273bca5f615ed52b26cc2b7c1a057953d5ceba189" },
                { "cy", "10c6e649fe389420567c8575752946162442e2e36808d1f748dbb8a7bc14cb1625f0cb66f524bc7628aa1fd25a4bb317b651e2ce043d9a9bcd5629c53d89511d" },
                { "da", "2b07e3b4563d42b6efe8c0760b61a53a7b38c077c1b0c07efad83b692658cc9d8007f6c8f9590febf746095c5d475993d02593b2b613436f85c43e7d3bd1f292" },
                { "de", "40c5e5d330f6c526dc3c858085ce386364993a983347139fa5267a8cb3e34503b8a2232f4c170c4c734d7cb3e3a3b7e0db4332eb9061a17c37a95627694eefbe" },
                { "dsb", "7dbbea7b444dc13eaebccda96eecf438eca068b3e2a8b333251e1e8ce36c4b99759fdf7a17c11741afc494451368686f1c538749786028ae4ece47c7af97b186" },
                { "el", "506af3dcda7db64b2a1166b2ac47083d7de82621bf0b3a52263baff41003d95764ec0c8dbaceebe4470b8ff8abd79414cd4be42e4e60ca172ae3d71e6f2a15cf" },
                { "en-CA", "4c88d472e79862a6f2424948b9d434130b5d6e3c8f77abc8102e70fa4c07f1eb04c169e6ec9cfddb1a3a776ed71ec242a693eda63d313862ffabdc3e92ab7947" },
                { "en-GB", "23d38dd2707599666c2b027c8a6a864fc4cfa98edb0584db1a25965ef11ee68f04831f767bb121be81c2b5121cff18d7e8d952a9b9643b8b7b62147f33f7bb31" },
                { "en-US", "f53d377ee4780ae95f431fd3c496beee9f9aef4b47148b8ecb50b2193bb9135f856f490b4c5de82786c0256620e1d1f96f80eb3ea2a13e1081c658a775300fec" },
                { "eo", "d82da088e4ce562a36d1bb2122f8240e6ae52fd99bc9e8b8f2b052463611fe18c7d34c9f6d99a872715ebc8f6c060402b34768e0261cc63ea251891dd28f9b61" },
                { "es-AR", "03b7c2ff75c16bdedbe7bbe42531040e0692d12c87d518637cf5da99af942bb6d997b11b89c3cd0003bef158223342555d0f21b85d957ec19696c6f64b486a36" },
                { "es-CL", "321611016a9827772f1039e82ccf491e677e9989fefe61e6e95a1be5e3a0a814bfe9dbc2d953997612ed711a85aec58c5a00c6e4ef18ac614b4cb5605e1c4465" },
                { "es-ES", "573d9e758f60264ff2ef1f3b890c3e68538586737fdf5f8c2a32f9ec464012a8dc27bdc94d0ea837b041cf226983d2d68f3fa14aa52b8d4bbef8b925300458a6" },
                { "es-MX", "0c88016b37a9ae571bbb5860a1cd97e6415ab059337501e38fe63980e14dd1f7cf10e1546948a7f749b9493346ca2120f6bfd71dd9b920b7ebef2b8f15b553eb" },
                { "et", "a01b09a0b82003f3dadd81f8f344cf68eb31372ca2b974702f6ada327829b5f4a4ddcd441e14c1733e17d25daa2c1638ef290945fec59859ff62a3d76de82b91" },
                { "eu", "4e55180f79dbdc538694ce5270c2a461f515797fc1ca2bc7ce9df57f498b4a152115e84a604c4207448ce887e43bc37d01841b72078967c619b4423fecf0b9d1" },
                { "fa", "cbbf97b9ba30974082681c7a9015f49524a63f790617c0856f8121767b6c2c97d752eb720cf998d6f2275115cd538f89eeab8c1a63f1e791450928dcbe2f092d" },
                { "ff", "ed27748cca496d19903ddcaace0127f3dad21e3c78eff035c6823c284ad23ba1be4d594f7297487ffc4e6690ef6cc849074f9fcf253dc1a989e7e082819bfef4" },
                { "fi", "839426594f64a4de2fab57271727bf7c019646fc59898d4e6e3da26cc6af912376d50cf06648d91a7b6e9d9c3ba28dadd4a25b6282328e9623341fe3e9871278" },
                { "fr", "227d027567f7486c6ca44a61840994fea45d47007bab49e0ec7556eef4ffb12533b7d351c115cfc11db341a896b684778b1e40e1f6b821fa2f567ac729d7a089" },
                { "fy-NL", "69a653a422161cc1dcf9866e08a39e946056e38e96b1064c608d49bd8c7828387b5a99f946cf11ae55b9fc8762d88d407b8accd3fa0027f76df10558212e3f4c" },
                { "ga-IE", "c1a6cd6cf9a4b5e96f95e52d36b52dd8510c0c3f90f065b5dced623ea018be0289a32055f63761d7f3d2473455bbb2435cfa933422f784c34515f7cd5513fbf9" },
                { "gd", "b9a2e1e62d89754ae84939165602198c2713803011159ed653e81c635358fbc8b11046990ec31ea8125ce6d55cd2ef107cdd4f6dd7b474d4fe328ec53c9d7713" },
                { "gl", "87b380fa8ea9d6fd6afb6be69a7b890cd492757ed1798e6b4258bbf34012e3d8d60c65fdba769226da0ea2e1e6b30d1fa0cbb98b017d0b3e960025744fd2b49b" },
                { "gn", "ab8e253d50be96f26aa21a4801d96f624af2afaeee05f2290202a966a8150378ba0ff7a4e1fe703586af5ee417ab256e3ccd17b3a965a66789b1dd59e040dd75" },
                { "gu-IN", "9f3317addc39c907ad1a7d0de3b39b93d4ae319a2407d5fdfcab36e1e4d9dba0d9169d16b048238010419fd1fca37f42877d63bbc26d88a7ad67f9ba6b4ee663" },
                { "he", "3966e29161a74ee0e410c8fd2d3cdd0eeee64bd37d8c3d49a4b088b2364be218462e08b04bdb9122e05e1ba419206746135a0d2aaadafcf5963ddcf209fcedfd" },
                { "hi-IN", "f3cb8268d4cb4d00141b7f4ce8b3988587ae36ecfb223dc0dd82c6f18644877633c4af7ee1c99f91b4e3462abc092a7a89eb9a6c75f6cbb186be9511f60987a7" },
                { "hr", "73ba2516c2f94c609e6e3a012834dc29563fb205b3763cde0c45cf4e1790508d67f7f773b59ff222970ca4364d8dec4a5306eab84018d63b68848c6a7dd31bf7" },
                { "hsb", "cd8424f72a50887e7c94fc0fece30696ce1ded31cbaeb2639e71169f29b90097d7bc9fdda6aa3bc2b17b6052fff32fa1e9ad50dc9e3ad1854302fb621992e543" },
                { "hu", "ea13a0178c54895145bc149487c5b3aca05958e777705fd53a92974457444a4eb0054d8963382e0bdec9dcf5f46755183403b74b65e3b9f9bd7cf9c2623c4479" },
                { "hy-AM", "c6338780496244b052967a5be2575beb5f4bffa05d2833f02f415135abf8e38da2d256ee3cfe781e8063fb8b48bb91c9ed7e9d409898c535f647be264cab6c3e" },
                { "ia", "c9cfb74ee1f1e8893e28832dfb6085b1ffb02158923dff29d59081bb111a3d66cb52bc6b1be4714348a61162c3a1c182920e2c0081b58b2fcf3ff6f685ed1534" },
                { "id", "068cbebd0b221e3c412ff3a84d4b4f73e6560e5aeed14cd1865474b057162752ed77a5f5d6a5dcb36f8489b2372f925135a11138584209a41dc0ca62004a776b" },
                { "is", "f7997c236b8e5cd130751e34db5f84f5920a82bd390d02da8c9b16d50a6b897b39de8a343bcb8aec03ea90891c73fb35a8090792dfad7207ed93a4fc73e00ed9" },
                { "it", "fa918005e9b39fd88313e41ce190ed8a58ed544652a9cbbd3ddbea8dec940e8371c9149e6e760d8d50ec693015881a1dfca1b57927ef8bd823db94589f947e59" },
                { "ja", "b4a4b02d13e1239f3e2462f9ba74ba7f3a8dbac865a18b7dcdbdd05fbc4380848634dd287e920b62da759701b7bcc6685ee78cf4dd5d6fe46f2a1455109702fd" },
                { "ka", "1b3737712a5a96011c3e31785c471d99138d373aed62568e0733742c3216fcf88a669afb171383a45504598696b3ff55b02814f5f893ab155309e63e03d17c70" },
                { "kab", "874f89e95dff78942ef94f3fe3dee2dfa728da2c1e90d955a5ea16d7b7733a3e5fd66681b4fef294cfd022458758cdd7cd332ffaa3fee2be17c588e926466bd4" },
                { "kk", "d4b84e55ad380611a54bfc6f29a8ca6e32ef3e333a945c7eb1af87af6d8c58a047d72b9fceb8dfd3cd83b891bd4b8b3b77dd4d3f164b4204e48419ade71b94b4" },
                { "km", "5455103f7a19ba174fa171e463e0eb509479a91f453f1882eb1d74e1a6976ae8452c9403f0feb8ccb437f9206f680c0cf87e2cf330f983ad148164321527f695" },
                { "kn", "49da4bb5b0c936c4e199bbd7be42c9077fa29e39046fa9fb899bbd6ae26fb013f88bf5682d126c03dd8b7877813ad235fb9b18cfabd2a08ef516dc3058c8d0bd" },
                { "ko", "d482716a5d4f13cdc96b75cfb0ce31d18afa8c6eeca486923d9d749b57c52d4f75a5221bd49677e10272118d8422edd7baf17b12e20d19874519a517862811c3" },
                { "lij", "eaf1271ac6476cc53b001e3e7209a72b17bf224f835d864edf4fd64f23db9485306e2df0f4b4bfff31ff49d90b48b2a9461ce48a613b98d951a397569dc46278" },
                { "lt", "a97cd791e98da7ade55e4504c5e61149733e35e32b4771993a519f4c9ae73e453ea55c7e9187a70299e8b3ac106d1a4de4b997c4fd03b30f79143699412d1d5e" },
                { "lv", "4f9c73402024c516a41c89363409186f54a2779b73ce3f5768c5cb8d26a9564a4be8c4e466c269fb466bdbc0c4748f93135780ee37164bccbc71cdb5050c2bbb" },
                { "mk", "a5c4dbba759ab5c0e4e0a9aebc5652f86a0250c32ba715afd8d5a0c740dc2634889a9fa79530b64db2d8f8566097b96399c515dc80519d8fef665cc87763b2ab" },
                { "mr", "eb507f79a0168eb3676558f378eb79285aee697438f705555fcb56c2608ea97c17adf492f88a955178aabce155f6464e0ef3d5aeb2807c58e76a654456359437" },
                { "ms", "179ae263c2ffef7335ec9d29bb097e54b11548fa5be76e748352093bccd89519aa1f413c73f8106730d73562b978f15aae8fdd9ebd8c6ffce45d6386865e23c7" },
                { "my", "96e85ed23c71a057d2bf33abf80c0d887737a06c3f332cad9c51dfb117aee915286af7332e634b5e941a955184769310e6fdd7ba8cd774e48c2c2d63a97c705e" },
                { "nb-NO", "99931ce54eda7fb80f951a0a59587f38c2ba3af770d9f490f9415e59e8fc38b944d83ad8032e1977e64f4124a5bb0eb5e00f01dfedba014efe380e724ba44c69" },
                { "ne-NP", "4a18ff980909dc874b74edb3f0a6f35f9ea92e2df04122f6b8fd16b6dc0ebad6db2e4d100408019fa178c3cbe3fc83bd38e2b1a08105762532eabaf88571ba7a" },
                { "nl", "510d0709bd2787c70c5e71b9c92d34be2b0605d1278dda5a026cf494fea899cb568f577e8ed00f336c1167bdfcd9dccdf6548594945ecad785ab0f035818e7b5" },
                { "nn-NO", "45e8317e7b1c7aabc6c84bd9fc05c11dd8bb011c34a0adcb3abf07559d468f2edc3b6b8c9e107503399b8b48d7f5da6247c41c25749d16d289de203f398a7787" },
                { "oc", "6444c2860b543dae2cef1517d557535e2bae228ccddaa01a6556acc73fe036d155285dc96ea2f8865434d7dbf62f63b89ae5cf17020fc8d690dae5121b10a7e5" },
                { "pa-IN", "f2cfb2bf118dc0c36cbbdbc68897833832391d0b11a03de064150a487364bd73c234cc81d78cabc883c821344a37ec7eb97d4caaaf14a68218f1a95c33515b28" },
                { "pl", "460723b88bc2933d150abbea45ad77fe6f6942a4dbf85a41540b201ce75a4c01dea065d1d69edaa95307c811bc775ebaa333e3b363e7db28dcf3536e7725cf73" },
                { "pt-BR", "28a09edcb8c382c3a65237746e310289efdbc64c075a9a76c9a39937f9f851f5072c758e0bc27aea4bbe2d7c99d79fb9bd0746bb67ca88091d35dab257a688aa" },
                { "pt-PT", "9d8278f29da55e579242cf8f7e6cedad7b530f340e684c42ab354d40cfccf6d210ce804db6186400009747cda54f76f096a28561cfdcd897604a4ee9ef3abbff" },
                { "rm", "57a836c8acda593fedf04e28cc552ebe687e0e2048c719719dc7bf2663cb98b069d6421d08f9998a05515ec845ecb644e6d215d704a8c21ea810dd8cdcf34cc2" },
                { "ro", "5178fed6966288da18c111a3e2cf61ffa6460073dc79befeafb0587d79fab457e711ffb5eaceec465bb2f021953db6fc2875a698661e6b1b3e63b929a23ca343" },
                { "ru", "e6699bcc1249847005f53a81b27b089e0506cb5d5f790dd252d0313a035d130075cf8cec2ea1c7528e20d46b1eca3815af5584e97ad5e7841117fe1b862c3200" },
                { "sco", "e2f1df3912c937f31359d48d21fcfc8977054371bdd82b22044ce7286ecafa59797d691a0bb38dbacd38d8ac99e4a9e63b86b52072bf7dcf9b11b8e300568594" },
                { "si", "77df1a1d1f798144c0ef6af5190659511a9a23a2e1d2b968612793cbbb408e183cfe1d4bf41cae379fbbef70531c90f7a3d3f2788ce033f072f0b0db3810a516" },
                { "sk", "193851b6c4f0949b7c12c8276c8cd85102ed15de8a89dbc90a9331bd0d6e624a38b5ad0875a1c850c5016a42f49956642d66e24469a854138f94e4705884cd3b" },
                { "sl", "fd16d4f4064672a8768f9bb4fa73542f8329cd9804e4a9f9ceea11b03bd5fb7431f80f81e35aeba6b619474ecd22aea229964865419f9ec88ff7c2bebe115fd9" },
                { "son", "edd9b9ba58135dd933771b0b35472581772a488cc5bc751302ae2aee6c5030329a4a1857c76930992ad4915c3b7ba5e125431e3a2b7f7f00b6bbe76f79679aaf" },
                { "sq", "d08f6fdd0fbfe22ea19c36e41e928f1f09a2f02f6c33af98d5c791a9baa7b63ca362f97dac7d20d6283c8a6e3653f530be6fadca28e56181c1981c34920d5b5d" },
                { "sr", "378b46597c09eaa3e799d788eca42a1e63e9e0ac59cedb3ea700b9b3f78b49b2edf078ec91d52e452a48f060203007338f489584c79e5ef120d89a09c2326cd5" },
                { "sv-SE", "0c9649889bcf8ebb1a5e66273999f708f2e79fdbec06eda31bca94f9cc4aaa3ecf9d53ec41bae0273dce66129710442218ec97096257cb446dd91cad29ac4b15" },
                { "szl", "b180a67b2d1741bba6aea392e3dd82e6b0700a9c5e05ee4ad824f388367b3a7ac796d12813f246548e6e911e15d83ec996daa39316378e5e78164d0278d4f9ad" },
                { "ta", "15bcaf7d04aabef81326d7cd21a7211ab7bc5fb85d4f0d12fc23d3e7fb6064a2d4bc430061691b749e13f6b015bfa4dacfdad8f773f02c07beea59bf01bd39f4" },
                { "te", "344887643f7146e873e48a9cc2adaad6a78b22093d6a74c4bb5f88912c53015b32a3f2d4bc8501741fbe479909e7f577c2007de9bf7e74eb608fe0556982e8e2" },
                { "th", "335dbc353c2503fcb7b17ddc56fd7f55cbdcbeb4bca50c4fd4a08da8fac5d8da8059271719c353d107c89c4aac2d83b71b4bd642330a81de7493cf99e13010c8" },
                { "tl", "2ca8aa3716ab32ed787abaf7923e4691836ee492624066a2abec73056a14116eab7ccb449fc143e95d2a45d7bfcee33bacb68430cd4578c20b164d8fd1548e7b" },
                { "tr", "80ac4d493946a0610caca6011db9a707a4a030f952828435463e82d7999c8e828b1ba5eb33dcb259d983b8d6362a1fcde0d45f5c24df6d130720f69698fa4448" },
                { "trs", "705bf913967adf67ec9f99e0e7cebf1f5818dfca56769d4e58e218669ba069c0e0251dd6ee7316fde97fd2ffa25bc7e03630df1b0a8cf1c305fadf5946bdc4bd" },
                { "uk", "17dc47bd8f7cb0dbb5b0d67e706cc1e60741719adfcdf156d697d28da33be5ee48a0915e810bad442a5c504329f80a059494da6a4ea35dd25c59240d825e6bea" },
                { "ur", "efc47bc07cf78ad94bb98968e6d60286ef76b72b45b56817f00c3215e06880e94d83a0db3e7ccfe0ad6b2ae897e5b7ff3968794662bfef105b6cb739827d5f5e" },
                { "uz", "d8fed33c057a2b36760531e2b6d1f662e95e26d6bae71f8e5174a6b2a13525442f0528ff220751f5b7303593516245b934f855f2b07eb953d33f7b6f9efb6b9e" },
                { "vi", "4ae82b84285cbcc34f45a21f8363573a9d6c206103c038b846efd7577c186376b6d8204b9def10f3fb23b839279b839644ada6d57bbeb7d88eb211684950c2ce" },
                { "xh", "c6d4c73db1c82c2f4866c79c0869e3ef869ed05f990c2ed100f3f6573112cb029578de4fa48d82705a4db63247c2d7f746ea90b8fa7b4f7e388ebc083ebdaee4" },
                { "zh-CN", "c3fed5ec56ac5878a6958dade463ac631042b861c3b4e7b5fc3c5f11954491ef996f9e59972b314c6d1abdb01bc34e0bdedd295214932601103c9050aefd4129" },
                { "zh-TW", "9b508294036ffa3a9b4b1bba5891254723fd1ca6020d311eb0d57b379c16f9805d6980ddc3c6d5a41e3b716623a9c693de70f01066548589d3c25f144662e1a7" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/106.0b9/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "f97ff9bbde20047f84aed4397fda22cc96bde728a2bfb44b7f60451d554c14ced7219342f2a1aca54e3f5e32ffb36dfeb99ba0a8e6ccd9611862db69fef69f31" },
                { "af", "be3d53cc8746efdd17834fae6821f173945517885b7f3508cbfbb862cf96ed85ed80e825199399fc45f78311a9600bcd70e941208ec002c52296bbec45815f09" },
                { "an", "83a4aeecc967a8eb4d5b8b02e2700062b331eb76110e4c93da2b2847ab76241a856787453144d85308a4a7899b152e04da30c858c4ef4b8a3e9181775977881c" },
                { "ar", "24bd7e4eee77eccd0f428ab035f5054ff8248b630ff978e60c4d66a62c1d7e6090dbf532dc340ec2c7ecb2b708e72078c6d9b8665b3f5f2fa16d736bcdf38e38" },
                { "ast", "5df68204bc59305d7545a0fcd9034bb45c9c2539295396d0d3382e61d6f224194ca1e16072e2e0a9a5ab395eef28bcd4e84c7749135bf5c3705be78ad920691b" },
                { "az", "5b420c6f57de862a8e5a7d42141c5576d83730952695cca0fd4dc0100f2c597ea94e096dbf24e55a921c4727296ab1f147de5b6830f34e7eaa8ca15c000b0476" },
                { "be", "eb37ac41c6b9a8f82e27eec408528edf92e1d4df21e4ae2ec4a6cf0f87536d009a10d802dde3c4e9239a608bcbfa452a87768c87caba8a10b9af36610be231a5" },
                { "bg", "b2352098e7e4e368725049eb3010130f961d8a43df1c897618a0a53c182fb051b3593da4cba83a86edf54b554bbc06b6d10e860588163c4d1a88f6315b7360d7" },
                { "bn", "e834e1bdf2913f7aac3db0ad01363d68917ce51db0df46bcfbe38adb2482b67dc54a3622f708f4f310cb03e90b532564187c7eda69f3f10bca76fb4c9f77c37a" },
                { "br", "e3c0c8c37b58ce9958d47765c7afb6544775c1cdd647705eeac75c78578349bc9f99f3cadd890fbb8b59718fc4aa998a6a8074ec0e1f31ba7a59d15f0d4f8761" },
                { "bs", "4699171f7041fcaea13500c70476f1734a421413335c042fe1e41031bfc0957705ea78463cfcf2029761ff0a5a815f244578cf67376e3da7469c7aafcc6fb2f2" },
                { "ca", "e370424ade943fb1c96ec45071962353aaa497c91dd26c1c358240ec4abf4dde8e887f372430552350dae42368e7e270de36d8349c198cf529ff43da10e408b1" },
                { "cak", "efc89b5125f0c59762c4f06021beb2d3a713aa7c5e6b101db1351df5784f09d542be1ea01442e4c46480b884e33e8e8c753496b3282667e95c74467d12f36d25" },
                { "cs", "fafb88d6cd2cbb0b3679f5f1d86e6c52620e142eaa4e3575e4d5dcaae348fa160c82d5e2474c134582f6a631a55979973e88fac7cf83b2facae98d78f49f49a0" },
                { "cy", "9f3f41dd96902f1c2c33229db8952fb7179792f4ade981e9cc4ddcd0524b702d071c37d2ceb4dc7570f3e9751c9d35062b1dcf9db366823dda91d9aff4be5ba7" },
                { "da", "a65d587887760cc42f734999d7bef69f17ee93c831dad63d6db6b0e17d7dc6aa66eb328859565bf7c314f84efdeab8b53fc9d7a2b40dc466db721ab0b41d56a1" },
                { "de", "f03fb1a1e662da1bbd1471e1de5563d3a314220357fa380f78bf16cccce3e236bc781ef0fc6f95201b424739e12d560f580e008f17c11166d4ac9136eb7bcf3f" },
                { "dsb", "5979ef40f0e25c335ba9d5667dfdefe31dc197c98f97ff99628b8d923a0b7e56e7f97c33e89cde285c11be5fa9f59c5a51a19a946933919ecd8a105a8d5ee108" },
                { "el", "a6de98b3ea82ead107b4b2132a1e896e6394e73ec3f11c34cc65b272c2ede66625f01b9ba9597861756c4c70900fd4c7f6154381d8e4cf3b9219c0992a287801" },
                { "en-CA", "80844a8a018ecaaf6e3049aa1d0c219b661b8c076cd792f4a813ec7e9aefe51d699cd52041c0465fa43065fd47aa942fb761b89aefc4855dc14e5d814b99033a" },
                { "en-GB", "5cfa14683ff52fbb377dd7df6c47d0d445e42b094f44d9840d87fe85de569ccc61f8cf95ac8c6ee478b44a05e641f57a2aea58e199949293bbb3bd460d5680ab" },
                { "en-US", "02f30b42677ee414209f1d79b2881962a587f26305d6bdeb3bbf1ce6ed54efe50c34ca20cfb37d2dee50c34447ee6001b9dc593e600e79eb9c67bc39aa34d814" },
                { "eo", "541fa05ca026e5bcabe6c6bcf37f575c19ba7e9d9c9a0077d1d60ffdb3cea5424522f494bbe3bdee2b13a55df6c7ad3ced6369bc6f7b64a994d5c0ad057dc934" },
                { "es-AR", "d5f1028e9a394db9d693ee89f344ff4cbe3be11b06cb6e8fbe17a5393998aa007455a90a1fac4f2046df6958bbecb9e9b6012fd2bfa1f76b07ce21b6bb65a994" },
                { "es-CL", "fcd378ac459deccde4a6100095c6a4f2bfb7663f815033789684113416edbb5ceeacf1afdce1e594c73e47766bc64fa47534bad247b37db720a59cd9dac627df" },
                { "es-ES", "f8f1a82362b3a89e3a3da207e14513008583fffdf33900c57bd5aae5bdab248428a732d9cce827cd8f7dabdde7c47e489fab95b862c01cdb3d395a377b32e80b" },
                { "es-MX", "50f4e3436fec743b107243e7f7d3a66cd8985a52a277bcfb2c2bdad9b5253ba8449debf0b31b14c82da3fa4c85348e53fa5b684b139ac5e6b8b9eba95b70ac6f" },
                { "et", "93ca4a9ef5cd011966c1aaa5a34dfd20960db8425602b2b8b0cf3d0edd96093435cb38d19dcf57ce8765d8c72eb4e946526f9353fa07708c98123e252343d497" },
                { "eu", "7a8fd6ce533e6a01221e9ba21a1e2eb570936fd2b405fb401aa266685a5627faa833cbc99e439840078d83ff467e4d516e933bef1832ccf17741e0d49196bba0" },
                { "fa", "357bcd05b835020b667173a5d1f326439a4d7f5e294ee7d418581d099beb05eef2d2e07b2a1d88c32aab6aebb3a47554db71f4befaaa987a523da7009b9a0ebe" },
                { "ff", "21590f33bb3ed5e39128de27d22d19f7f967c6b0d99009f5fcc708dd765e8ee2f0c59c42dd650a30e7d58646cb1c0d84b697011103ff79772304330ec27191e7" },
                { "fi", "83441d0c800163f6977e3a88badd9914e21e55709f8a1001a2113b6e76a6955622a032ce43bea98c204a6141966ade60ef828357e125ab72e31a6c8ee9e76cc3" },
                { "fr", "e97979c305908f9f76b2469c65340ac2bb7885278484268387636235d9d19be004caaaa1a06c35b01f345f30b2aa86d7daaf52c1ce5a1bb6b8be2f9ee812bbff" },
                { "fy-NL", "1f22bc198bb993aec15d3e93589f75db747068d55cf78bf0c740f4c2d7593405409a4421b29accfe29ea20dea806cbd6ada430f40a54d61eb8670057e17b2579" },
                { "ga-IE", "324acd63d978ce0d0d596c24e019f2ae3e73efbd80f170177eb4f18af19729c10762b12671fd1630fe4ce005bd1bd6347d20d58e8c080f2737fb0fe76fe52f26" },
                { "gd", "d9a242b2c65d737d5d6c0bf382aa9f003d0864af653457ef9516dbbd140cd5e5f13baa704aabff6c89f6fbb8fc0252432230ae6ae56886b5d05878b4fb24931d" },
                { "gl", "ac88a875b93019605a97a6f5b81ff47d3bebbd0d6852038fef70de71e2f08275192e7ddcf263eecef67c46e5ff88d36424828c8191183f583abb2d1374deea1a" },
                { "gn", "cda75364281bc9da0a7ff842e1f5e1f36e0d0af4a8190d8bf05dfc207577befbcf201b4ce1021728b009df92935d3dd3a6c9d04df7058aaee9b8b967a1fc800f" },
                { "gu-IN", "efbfede4fd5783161f36052920a52d848684280c0bdd882308a0ff318322f02d67ef2455b10daf7a227cdf4783e0014420766e4b05d917797cb21015e4e3d77d" },
                { "he", "1969d89da1064c981c8d42d22f57b032fa69c3ca4996e64e6f1fbb925642fd62b9df1deaf9481c37105c41766e3bd2afc19a9418757842946a41b2cd59c0a0ed" },
                { "hi-IN", "cfb1448fdcf6a4ecad5eb0d8f6ad23eb8c3b08c36dc4463fa2217682058151ad4923d1e92a1584bcbfa94bbbd24192e9a7c6a7207d81aa197824e891453d7a42" },
                { "hr", "9ac9c816136afb102ad53514df0cee85d805af763d5d7c85f08b510a83e1948bf074947c7ce24675c1b7df8b1fe75f3ec03b8c6260c5d0b632fc29f83158ed7e" },
                { "hsb", "32e05b20923711de184f8c6eace20f6a7e78d9bff55aaefebad992531b0b9ab5708df2fe9abc7b96cf06a082b4e5ef8d24d358d9e59d44cde9ab69ad9cba1762" },
                { "hu", "8347bc43a8cf5bed0a5b337aa84d14673b5dd788e8a05e0eef58879f4e18dfa90866180c0e23cfd870033b327ba150923d1ef65aaf9b72eecf2f1ff0a18c8fa1" },
                { "hy-AM", "a2488586533fdaa08ff81220e5a8b68070ddda8e6e59f0c0daa0bfcfcc9a9906444b2908290e254df56b00126c7664d48f8048b861020caea0fefbcf556a04d9" },
                { "ia", "cbf49ad80b1167f881338b7554c433fe5cb5faec0dd3e60cac8f7ce24e8cb22f1917a820786e12d1b4a83a282e7d98a8598f21f0b90069fc31ffcd84d0e42753" },
                { "id", "ff41081fe9650a3a4e44193fa643c45e02c1b6f6978252392174dc858559a7aca55b7e42931e7728518955e684eb142091b22945471bba84643a37e5bd74feb9" },
                { "is", "207667e3fa8f62d37b57dff6d80a815eeb6c21b28bd63bd9ab9d27f7cb464e3a84fda19ea48ef1efbd6e4441dafe3bfe21798b225cc065e64a399aedd943ef6a" },
                { "it", "ada5102daecb1ce5b1a1a1c05691359a1ad0241d29949ed47e31b7ea1d15f518800a9ae0d60c55f4b0679aac88c61b10ec9590f1755bdb373e19fc26a9acd2a6" },
                { "ja", "1f20bc7734b9b972d751d0f5bb083cbde925fbbb71959e3f36268327253d50abe9f5b43655c92ba99de154228f989aabd0c047c1de1985077cea6805c75c8d54" },
                { "ka", "e9e195a9dd849c3827e8eea31dfed93b9d267befe36626923042c962e17194e58e43b10aff5e9d855d6a3cd07a4f6d3cdbc8fcd58bde3ed27fb0fa85880972a7" },
                { "kab", "3bf0388b2c6f8a75fb5fe3967e22038be4b0dad34cb37154ea89b8bd243bb91dfd515e997eebe44d089b34314a794940b35e6b4a4ef5a0590e35314de2bdd20e" },
                { "kk", "c7f080107bc752dfeaa6d5a25da0d791856ddd92f0208229f3b2fda3febcd2260adbd1a24a8a5673f102a8924037ac0c6da4eb765b17ac27d18d7ae11e30bdcd" },
                { "km", "0012d593761f3c56b9868e8975c074346625d881cd03c620eee8dbb5ee0d23ce0bc721b17bea093779088043f35ebddad2d0db90f8a2f93069edef056bf81c16" },
                { "kn", "31850cf82885ed2c2d7db465d7f09cabef4b2b958a6454b111d58b46398b9a863065917b2d3179605cc62f7a4f39ac5ca05e05dac3a575aba55b063341019766" },
                { "ko", "57f305a30dffc98bf32a09df43e127d65ea6bb638db2502b276ba81e33d7d36965b29fdabef0ff805942ae340a411057bb0fbfcf1a685385cbc9d248c9229b7a" },
                { "lij", "c4ecd25ad771f7b571ff813b7ecdfb00887ef945fb5b550895c687c1d58fb2b554f27783f245cd4feea7b1d4acb5f9d0f9b80b40775231d8e9d71b5df7c9d67e" },
                { "lt", "d28ce694159cb3e7bec132afe37b5b1ef391aa6dd53654b8544c3866d431b1fb3b6c321af9aefe6f65c85c7ade8df40198dbe95eccb9719bc0d14e656a548542" },
                { "lv", "8b8a1b6b44e724cca4cde46a0f3bb11269f282f9d161d4d46db020baadc4d2da7a1d481a49f8f8d5d8b44c1a187a601b744d92aeecdd8e26ee97ed37d2c69e4e" },
                { "mk", "6fdc1f67522b0f295c65b662e55b006ec48677851eba87285ef11678174c1f372baca1dd7c39b7c5dc8158b1b34c46915ce2e3c4230f6499c0a69e427f4bea72" },
                { "mr", "f33321d3938c6aa8304f73b7a43838242a7d93a36547558546906bd3a333643b69f548fa1e976e791dadbb1103f84989d69faaf37afe1f8d1ee747a56fa451f0" },
                { "ms", "d9d68d8bb6b059e7c6f65d00966d73416192156558dd03e18e8a06ff31977de1767692f7eadce4f237ebf90b9b207083aa428b3f5fb294e8cd45dbd792358b72" },
                { "my", "76f4e691e44c3275e753801c690fe46439cf780fb0820eef91518230d6bba8aa7fd6e3d7d467065c7340a9ce6333462249add3ee33ff8d14305134e46df929d6" },
                { "nb-NO", "519f61b078ea98bb8ec37d950fdd7a78e5fb3cad0f9c7b68b2a318560014fbb84aae2ebd9302e6df4adc9b20241bf5b5d54b8e10457309e35c3d31b1d1c79040" },
                { "ne-NP", "0bc673665dbda91ca1953c8e16d002ce86f320d8be16f40e54310a3a7b6dbb0f52fd5afda3b6e72ad123a8446ed0223d5be13288b813fa5759f5643b8ff4826b" },
                { "nl", "a94d925bc7e8226b6636c26585efb0d16bbf21df156a86d97ee5f33d1f0393cd5db9912eb12a05352bcba3594839b04dbca6fe1048156439f67c6d2e887ab08f" },
                { "nn-NO", "4247f51f47d2e9abc9075eeb8628e526c35903d0e7b9c57b5fae43f1288259fe1068a06edc9fb740f3095147adbfdb8817ffabf1be90d64c1ac47e3795ab26da" },
                { "oc", "e75bcc21655375d7765f8e6560eb35b506d33aa57afa5a67879691fb5ddf3e1758d6ecb1cf901c2be81b2c444a74bd2a15a0ff342002b038305affa84f8b4d41" },
                { "pa-IN", "74f6d688ec0564345557cceb6d28ef037cbe76ba6f8620d4d5ec49f53ca07769c0d035eac5faa0943075918fe6263fd02ded898e03f6cc29fd887bff13fb6a28" },
                { "pl", "0f15064937a921997669b5db815c8e938cbed28290437fad7b69407d731b1c5265ca895f6837ab944ed35ffc8e1c913dd33449f9fe81a53942e663e35967b7c9" },
                { "pt-BR", "0196a4d100e5092dba1ed4c2da1d25d6a833bafdec86798e0f9df454a05c06af3ccbc0bf36d3ba5af6d13404e69eb8053b329ecb41b23ed5480f2a34d0470a8d" },
                { "pt-PT", "9b44817e8595e33736cf52d71d69cdb6e55cbe786fb7d9ec2783ed43fd0801c41f57f53826ea826b474969aa4e70e2cbb20014afb207aeff74907f627232e311" },
                { "rm", "f79cac26bc5356dc6a12e2b37cdeb54aa05ed32e5693c0fd9c12155c17827e7450b70ee89b66e32f7ef4034732ebf8a27fbce2394c721dd8c21ca580b3ce7877" },
                { "ro", "ffb34f7cb480674263dae4fecd6eb170f7b0bd121f280a76027ef2f4d5caa5a4d6f2cbee6b64410fa8847b2f1427d9336b45a153aff8927b95fedbef91751f8f" },
                { "ru", "8e97f68a81aa7cace0d784af87f938c8845d273b4f5c9cd60f7c9a64a4a17038ebedf4af03045798b36f7045ce568f94de20ac7c23b90d16d412302844b815bb" },
                { "sco", "a2bbf90b42405f7ba9a43ec460b6f7ff6599f19700a9bc0e7eefb6342e56d9b6bff6063d1e36b578cafadf86033cf1f94da3ba2d6a2572ceb8329aaaea3d9ac1" },
                { "si", "0a7402046e1ec015ccfa909d5e450c43c15bcf166b13cae55566da06ead142923b10c727d5a531eb791d5439769cd7f8f0eee6ffa7c609d643c1ed06d99ad2cb" },
                { "sk", "d4ce2db03454ebb67d42bcaa3a9823d2ffb7a35aa676624eee05c98b4de18e31cf25ebbd1f8b36b2fee4d3d525d8933f2d3760fd8e11b5d02806af43a0c4d766" },
                { "sl", "c33daf68eff98df8525fd619896554ae157b3d4c643a84f7d60c96b64cc134a177ceab93c8611860598b3d4530fb553a2952da78f552c2e2781a49577d256f3f" },
                { "son", "9244015f18a7705d494b4670390c2ec8adc44d0520032a0cc711b2267f8e3d08a5651b65c1451a5124e61b2be3c937fb9f0a4d7051a3431600a823fe3e30dd02" },
                { "sq", "297bf008a3f7d7cea2d6475bab3be3c064aae2d4619e576871071d938a74ca2092fd987e2f8b53d28259ebc55142321f4225e020df25e1d6ad404cb43246614d" },
                { "sr", "bddb197ff2fda69defa79d024c5c046df9fed89a3afecc3b88710b37cf969a1bec581eeeddc96c581bfde20c1fb75305b430648906603a85c3bdee084b176aec" },
                { "sv-SE", "842a3e9fef942dc502f7fc8916e64f286a899e5d18e9b1de26d33df2682a652f472c8b48b8f42cfce9b277164e5e6080c859a3b4cf95061f574ab75bbb00c039" },
                { "szl", "3c01a21de83048c1ee91171178aba9cef3fad054335a5682afeca2caf7c1ebfbe1e33d5f9b8dd210e04bdd4b0ad5be86b057dd15706df226dc3f2c140281813c" },
                { "ta", "f3fe55151266d9b2f774bc253ab2ed261b78d37c61da32b47c8d27af2ee5f433be2581ea0701e2a631a56da7ac3ca8f379e98951ed361cb449f2793c1b00694e" },
                { "te", "6a026976a4734ba8d15a8daff04288d96e4864dbccccecf11997055ac826c7a937ffc358cf220d957c72de3443ff7e3d21a4439f69b9cc96967b5276f47e4fb1" },
                { "th", "be4f64c15626265861284cfb582510248ba298cee5c27d23dec62c1ef5ff9a352898fa256bd0f1195f227d45779409b8ce91f4eb5165fffe49596cba4c486094" },
                { "tl", "f4666968b5d174d25e6a1cf558e988c5f2d9bab039292499cfa61d041f24b1c2de87d03e461f3b799110a441799767ab1a719f1f60a566d68f94ad70350d185f" },
                { "tr", "cf9d9dd32336701c935dbc03f2cdc1531a9368f0ea5849ab12a93daea8bc50dbe7ed1824c418ada1aa852acfbdc30a9b03ef3c4a3bc16f8fd64f186e8c674900" },
                { "trs", "ee70db525271d0128c395c1e049d5c54b648545fe1c109e06b8cb0fb9bdf8973f2e375ad7a449ec119bf95a5c9cbde158709e1c1fd85a073f9647f32aec3da24" },
                { "uk", "d4e557ddf94b94610ed48a7a10ea175bb0059d155d99cc781e502e3c9441a8807c99fa8a1f8c274914377ab5a02bb4ed0de1bd3bb6d51e124f1de3c8ced482fc" },
                { "ur", "88192d7f00ab651c1de281245a11461c7c7792f2d654ed3fcddebbeeada17912315ce77be4486d8a9547dc3b746ef73f3f1c1074934917c0d22ffd46cd72426e" },
                { "uz", "ae38e170dd3bf3d6414e41dc82e754b186064b999ecdfc5e531af5f8e7b2ae51e19b9cba76d65ce22687df367d9e7c6f27f76eb87c1a9d82acc888922b15482c" },
                { "vi", "b66b5271b1217f94a5affed5b9955db4aa3fac6a77cc5b5c1cac3886bc2d85a37645f88762add6e0fdf3cbb17d8ae70068c38698f0e64472f059098617e4f91a" },
                { "xh", "79660ccca60cb2ed39184e8721a2f38be6e4ba8b11b9c1958324486e3af02db43ef38d4084189ba3f9338279ef45a687f4b8553d4ab9fbdd3beb40c14de1fc06" },
                { "zh-CN", "19fe63d035197dc83cdd13f3cb99796a38800fa01c693c2909380a24783e00346cfd9348262ce82a5b3546286ef188b4f4b27a98bbdf984d57f40fcc6a6a56db" },
                { "zh-TW", "d2c127c45f98c96975710cbc8969143c14c833ad0c71810aceb0f8a82b7f6e273d6c81ef8d8bf4c9a8c55ece9b78f04e51e58dcb7269b6da53d46df79664f1fb" }
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
                sums.Add(matchChecksum.Value.Substring(0, 128));
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
