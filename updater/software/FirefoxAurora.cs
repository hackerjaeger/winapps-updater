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
        private const string currentVersion = "121.0b5";

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
            // https://ftp.mozilla.org/pub/devedition/releases/121.0b5/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "1c6388d1d51b3649d56d294159e39c93014481ad25dd387cf4acf2df61541b9215a5f29cedb291a629bfe59c6ddfaed087036337d4d18de24b73ecffa92df8b3" },
                { "af", "7b32dc9c8c3c20c4548c84ce371d82c2a88e4a5bfe51b159e524b24fa0cec49a61ae5efb146724cbd3a10bb8acff7888e6952ac5a4c2dcc7af7c648e34281c26" },
                { "an", "0cdcef256274626c892047c29b7215a970acb651a1dd7dadf749f96e177bf6f3c5fbe2eb7de2fe29443d06a5069df80c84ff28c3dbfcf94f8fecd05b3ff697a2" },
                { "ar", "ab49894886a325316669cec1c9faf52817d01e75ec95d884ee0a271f6b9a0d40a3ce9059cf2d7be260b7ce51b08bf1c7873a48fef47c2d6594dd5f01cb8f8660" },
                { "ast", "ed6e5ce682e6162ebc3a013b34e009a1d1dc7e782df815561485449b5c1d4bd81e6e95708ceb4b31a039988669f188eb6f033d5d1dd82a3a3c2fa8d2d0bab2c9" },
                { "az", "f7e71ef4b33131059b9aea8296aa281c78aa35d598a4389f66f52816a1aa3cf3de21eb129f1779f9d451f8ce845c70d30364e4039de9c8985ec465e203dc789c" },
                { "be", "c09771bfffca24f89bf2ad4f8407b7cc1c0f5bccfc599450ab4928668661997eb4acb0964be2661e2a71c1559a3d7145dfa5b014f2a5230d2bcc3d836405273b" },
                { "bg", "d00b5053f425ced68639639d7e9562c30b74a4aa66df9c6a93ca92349113fe799ee4d3aad4910c36f22b875487cbd1a758dc9eb913520ea5205c283bad44fed5" },
                { "bn", "e3c43ad7b39f899b811443876f236255e0cd3d2def4eeba2276ea349f3afda34301ff12ea94bddce399a6a170f5eb2d2686421561187e6294201e8ae2710a81b" },
                { "br", "07730afdadbbece0c2d6ca36b45d631763cd0704393a86cd312d44d63b361ad8f339390a4d0a0615d60226e481d1d131051c91e53781b1cc7a9e5d913d242201" },
                { "bs", "80b18e98c90e108c9b069b85ed426d7a53626ec99a7a740dfa36b29cf276b7ecc5ecde9772db84aba3bf2c897cefa9933199096ec884814c5db8b25ebd3a280f" },
                { "ca", "677395eab903f58c742c46bc65eec6b1d36aabf104ccb7a35d9905a2da858ba03256437c4a6deca6c4748f5515bc08e72f3a426ef1f689abf5185e06f2cbf419" },
                { "cak", "bb611b2c5b902c78986726f375aed59c5737b3433fd94efa98dab0b99aecd7529d7dc0f4af5bd3e4c6be4bc710071355b6cfb83bb7f3c6508632a67f0de9c1dc" },
                { "cs", "dd09aac7406b934f315b0a556985617dca0d9eb45628035bc3aedebecb92532b17c5df68f4c131ff8db2e03bd70f802dd3f57da58a4287518c8751d25798c366" },
                { "cy", "8f509763c08ff8cc823a5ca3f4f9624db10d919589e7249b9ac975ad923088fa062e4f2b903a6100d87b5e87001c1f3fe89464d287c409e2a4dd6c203fc7bb93" },
                { "da", "e3b3e17e0206e4323b44ab3e607726af05fd27aaf71eb10bc4d45aea8e476c4caa98dd89854333c8ca34cfc3b72c8e92c9d24e2a0875821e79c4961914a9601b" },
                { "de", "85a24a9ef4f1d170e29e70939afb9b1753006f00990ed1b89ce66cdf3f8db6e1f2e986f72766698d9de3a63e77a468bbd60fffe2a27a08c10b7f90f1e09c2488" },
                { "dsb", "8e959a83cf36516f8c38b06672ac638db11557d00c7ccd0af6d87c28346add75f0c57120c11bc43684965e382eb6d5277f17ba7097c715119002e7b36fd05251" },
                { "el", "c6be7a8d6836fd7c21313fcc9a955659baad77d2c266b691b41f4fb3cb5688bbda93f815e4605cc89fc03dc9669456e3e27fa845597a66b4694bd5a8cd87a248" },
                { "en-CA", "5ea54bf066d07959d79431cb1e4ec7364847ad7ddd70fc89a872f0b8b8b44c6a8d4aaa2486270dc13be2ad00e8cfddbb87c27ad5ccb179cc1503f07a2bca326c" },
                { "en-GB", "f26b6a87fd3f24228a1eaeb6311b4099f5270435ecd3e86f13b58e7d9a67176b980474cdecf344935240168d79386499dfcf466626405a61d4832c82442d95d2" },
                { "en-US", "993ef086ef6a998535453c480ba3231124781cc23266207d443697a4c0dad2c8389be2072ad628aa66e5ba9dbbc0ac0513ff7680151ea2105d6473e01969d5ab" },
                { "eo", "7da40e88999992d5a5d02a70501652485d041ce01163d8077fd15c248b3e0cae522a6e7d791c25639703ec317429f55967ee28c15ae155686969dc8b67658d0b" },
                { "es-AR", "110c86fb176c8e73a85256ae6e4f72f5969bafdebf003eab990a43a8d4e097e00762470a6ac75d8a2a0d8f92316a85557905403e19542dd8562c202a281f2d18" },
                { "es-CL", "5f476a06383aca25de91e5744d607f485391692b189f42da48eff8cf939bc31b7fd9867f194e19e0a9ab73b5a36c20ace0093c61de0ec68b317c3eca9465865f" },
                { "es-ES", "9f87475139ebc63e0b7ac1a48cb4f89f67335de16294b88dd610df0fc132547ecf48e37071bacd738f45d55814bc85e774186d69b472dc85581d3ddec5ebccbe" },
                { "es-MX", "8a47456a12726a9a9fa956d2955dc338ff462a0ab4b6e52cae0204eeea3061e6368b21380765e700738dc0cbb9e49a5a8e4b1df55c5c3c7f335753fdcf600819" },
                { "et", "a2af4c9f220f5fd559124964b1320932bb2e5a15ea35103550af8a8e18e5820bb4d7c6d6f7690f47ef4d05ab6602c043718edc8d17165fee9661f264dc982283" },
                { "eu", "52809306ae4f6ec21273582bc6309f1455e25d5e169b7a332ba50a5f1b758fdc2246e9798a8dc0ab49b2b2ade9f8b88831956a8049764fb15f8154b1b73c319a" },
                { "fa", "f1575c747722a69c3da571ab8c9b1542bb4959d58f0ede2185eda9eccb1d4ad742556ecb2b58966b97f3632872c8070486475a97839803a7bfe14f1771e2dd3e" },
                { "ff", "d31882bb8d4ee79c493d223ab8c153b7137e98ee924a5aaa9c4be8f056bda4c84e5dc3d89741bb1191f7d136cd4ea0165ef77b730b0da3699060cebf55dc2b17" },
                { "fi", "1d3fa4459339e6aae4efe27cff7bd7d8b4fde85cfc252b9fd8741a5cf4548e2d1cc4b34eba3832b1eee92bd825d6a4ffafb084495ec3ede8e13ffbad7c2d18ee" },
                { "fr", "a4b1f2bbbb3a717482e23b7e1f0e4928641e0ba9259f2ac0db8b5b2bebb1ccb9e615ca70d373297ec76c33f5a7f4edb523cd6d432687e0ac14e464ba2342a8ab" },
                { "fur", "91d6f425576df0b88e84a222dd2222e6f7811cd414bbb002df17dd6eea0ea506445f7837b19332448982b0b5ff6e1862d768334841b0b9de4c3c0906f3adc87c" },
                { "fy-NL", "23244309f9ce9873deb9543bcd32561b5285323e28fef0a66dd1f013c9b7588bc7a5f958946c3e7fed43e90e8ea77d05f9a9cce7ae1e3d3b3a7c3c43854c2e40" },
                { "ga-IE", "e22319274446ab38bb9eff9bf34773753dbb5aa5b82440d6d96170ceb3ac6e523645b93f4e250b23e54dee1d895e4cee8b2eb8380abb455c5174497db454e6ef" },
                { "gd", "4607f64dff16d4709293e9d588656422f7a0672c43724c8afe25d7c195cee1e4fb07168bca9602a639a987e088348549dbec3b3a5995377b557ab6a5ddbfa165" },
                { "gl", "94c545cb0602c336e317d5c88032836bc1617f00939080312e586224f1e84d4ae49f6812909cb6cc4e078d6f677d0f399370ddf07f847d3581696844b8fe13aa" },
                { "gn", "ca9c32d890f280eddca0d22f98b7b2fa5583a229dd49f54bfd6cf14d0488f52236f29aed0c98ba00c9143af713cc2f5c3235720b28e8a58755bae6b1941540c1" },
                { "gu-IN", "778768e42e35124e97518dfeaeefaa706e8f06187c469b2958b7288e5259db56c1da80c9ddadfeb800f0f01946c4c02bb635c2a18876224b17bc6dd6c10ffb1a" },
                { "he", "d5317561b207f1a6581ca2061053f4005541bf17c0c23935f08aef943619af816bd25c01fb74d8bfcc063e34078eb8ce303f05e506db6e9149e686349bfffc8f" },
                { "hi-IN", "bc8d7a84c459cb2e03534911997e81421bdc80a06cb9129b86ad88b1bfaf8736a4ec0ed1f534d399d42f34b28e6187b6ef66747fdb0adb9c8d2a128bfef7041c" },
                { "hr", "e95a916044d74fe43b9d6f52a2478e2e069ad5a335cf570eb0f493796df37808a52f6918200f929ded318eadf64f33badec90b0c5190403dafcf442ec6c68dff" },
                { "hsb", "a03baf0876c4979fca389ddcafa7b3e9388899b2efc05a25bf5b8b0e507f9153c32584f3d4eedab524576d1ad32132c11495ae20276d4e81f7fdb15105d90aef" },
                { "hu", "d133a8e0d314e84823ca94ed5929cc7f082e281228d096954117b29f61f95f8ae1d2c5b9592d92eacac842afaaa28697f8365ee730eba779d7336938c24789e3" },
                { "hy-AM", "1eaf4a3faa8faed636f40e710e6690975992c968083d2aeb554a99702737160011678078dd8e13d45a41f31917cb0c5f6360a0f629ecb03ecffad95042156deb" },
                { "ia", "16ba67e79a5c5772296ce1cf0d00e432048776ab1bbe7d60e8ba60014d2c856e8debdb8b166a9741abe27c1c4d9525ec4b79d2d3e5f43335c22e37ef0606cf69" },
                { "id", "63e7cc9bcd7f02e41f264f147dbb77b91cd07ec2f06746be092dc03fae3f0195a446863d5882e1d4d475cc5e2993d1012f75a0267a5ecc0f7809b0e38e1f669a" },
                { "is", "cfd710e54d15b77c288d9f002cc051a7e0d27e09084c991d635fe663cc37a9c21b7affbc65284c4e53b33afdca63dda10a9cc98be5ae7902fed12d6721da5e0d" },
                { "it", "d4a8c79ac5a8501f40e03e9ece596c76f6f5af0c7b42e07570be314b84623af5e6fbef82d25263a2143c18e5767e693d2a298fe931539021c4256fcea9c46bc1" },
                { "ja", "34e87fd0e0a58d87b80f63a8fd1cd66d79c17cc8c2ab72da32308f9017c4fc3670a1c8767089c5cd760f0824dbe4835939f876ab5d0ef5743e9b593900594132" },
                { "ka", "deeb3e87f9aac5268b4b885cf0d54500717cf5f0e581b99f999b29ac5b652d22461b8cc71148965849a4fa2f7786ab5f6d738871f42bdc62b43c7f8349f684b9" },
                { "kab", "3695a99454b4ae3c4be47b0b36b19cb3ad43c512e94c9c72e496ccb4491f4ae6c4608d64d7f66976ec605451def52a89455584528b426d0e88857749ecb0d422" },
                { "kk", "be5b9ec45a3a67ab4f42a4ed0aedefe53ce3ed87179ae75bdf7499ade0425adbb1e72fdfc4546925eff978554f9f99833bcd1d82f08750802078bb425e5b07e9" },
                { "km", "f5de40800e1c5b95392e5564269401c9a0ef30f887a606b36da30a6e24b4fb4f4c47cb254beb1d29d6ef905fed281988865d0559f67559681cfe4a4894fb7041" },
                { "kn", "a06c810e6ce65fa44634520281d4da4626a282b8a68026be001d975586075c537a0738a43c6bcbdce68924dbdfd605066a0563da3898b0d34d886de9cf723d08" },
                { "ko", "6960a1af4531683e9898e2e73dce651ee92c36bc41b4659ea693fb7d5213a79a696dad95b4dc10ebaf3fbae49edb118f7ff4bc329d246e24fbf4c4fd19b7e63b" },
                { "lij", "5059bbbefc5d712eee6a7d80639ecfc41fbeb0d563ed28b37521b255e95007ded2ea2a299a2fb202ce93a7a0fdeb1bda1e479155a4b205a9163b8fe82c32202d" },
                { "lt", "1900a94f5cc413718f6b5cccf1b2249df9b38e329d18f3e35c13d0f3b54136db826fb0a8c00180b0a6547642623714947150ae9346a48199954ec79b4f803d33" },
                { "lv", "7651d409bb0c76cd8d89341f57a27da80a6a6d3cfa75117702a2a532acf65cd89de497763075440cc3d67e78764228635850cd17f7b3323633b6fd4bb16e5a47" },
                { "mk", "01d9822745a55e5f509a763c556e58aae9a65af8a55fc9d984457b6f6114eab93690b4490f777f38c9f9dbcbd6d8ab884bc6eeadeabc42cdd45e32236b70383d" },
                { "mr", "2472f54894e320774642c56fa9cd3ee86f40a33b927bddacb5c5d80df8fa5e13e4a8d47bfd09a54fb3ed45fe9de17c5b84635122e1efd72d502801405b7773a7" },
                { "ms", "3d7da07a93cabb831d9c79ba45cb7a60212b35759872d188bf0d3eb0996836bbab5ef98779a1af00dc85955dd52a4607615cac3c76dc6c03a6e52c9bfdcee182" },
                { "my", "30cee726b941c7da228bc6fb13049adb2cc1c93d1bf12bef78dbd5ab7ff5178a6818ac4bf162cdf3a8489761de34058c14ce81ed2570759cff4bbf236e6dbc2f" },
                { "nb-NO", "834b5844ba9c201ea821e0c2741fbe4e690ec916646aad86bca559b6764e75ee9f9fddb1bbd8391dc07dce7d13f6b08923685ea5f238c0c3a68017af7aab1292" },
                { "ne-NP", "c1dd9dd375d43b163f871dd01a7de82b2c919f4d9ee07b51f5de9ffee2c7baf5c345351d05085284b9441b865cfb7b35846c5ef2b7ebc37f0961eb83c0aba814" },
                { "nl", "90ec51f9603986fc08fdb38ce81979dfb533fa22e61c31092c013f83f0064cd1d8698a6c9d5b04703ce0f4d19b774b2ca82659c77732ba5975f7197837eea2a0" },
                { "nn-NO", "babe1a06526ada37be0980734c9a5714dd606a024c82848d7e2a607a5fec2844ffe4ddcd7b272f89efefe311ae1131af5e7b29c0f3578c34a9a42491c7176062" },
                { "oc", "982438ff46413ea445fec77826546e80c2b43dbb3694c8cf511da9c26eeabc4905beb1ac8ba40f11e7beea3f406063e14bcd9a8ab7261724697031bbb55d29b6" },
                { "pa-IN", "81104e1c0df518b7a0fda921e0e08c140b6ed0c1812bd4fdc745fb231c75e2e2cf2aa67707cd7d51053f6afdcc81707e6d8b14f48486bda667540e670c3abf5e" },
                { "pl", "0207726d5cef2e37de808ababf87807799805a26f42fe665ce0969796149e2869548cff44a148bc1128534325e857465685c3a7abecd13c70167f9ad90ce1f09" },
                { "pt-BR", "cb079044eaf15b6cf903d26ba2358353a01bdab55896c060200d3c453cd21ada5f2febf94d481b6a4da8a408e514bc1459465a573f4a232021384229ba1b3a09" },
                { "pt-PT", "042501377a8787781a6b7158ebc6d9ea6f72eb301b04265da40bbdc6e1d224e26eab773156653c88f74e847bf6ebf2e58cef560433ed2453de1985fbb2e60c69" },
                { "rm", "c34bd22c089bcc2d8582b5a318d121b34544c39cb4b62dfd9703fab262a38ecb2a12f38424cf5eedc807d469e1ad1d55cb07ba078ef4d8e0ccfd80d728dac164" },
                { "ro", "807e4a1ad624cec4d247b0d7c6b489a65c7dc0f3949d78f6ac4b13786a2e31e7d9c51141c6e5c3340356b5fdfe557b802dab46f1e0d1e31daf74b192929d79a6" },
                { "ru", "40129a8c60a4b01087073b3000c5211144785a789ecd99a61b88a300eff9c6e71b31603c1732810da2ed4b456729932732ea8a928300f00656f3038b532440da" },
                { "sat", "58f95e14132a834847ce7e8db689e1448eb9b112724234a7178a76ca4a5d7cf5df2ba847c5b315f85bc076d31a923a7a887a27c4faae7a34bfaec4320087443f" },
                { "sc", "136571a4a483bdbe8dbb4d7b82e8e0b10c34bb3a3c47640b183423828ec8d7efb5ecc6a483987a7251ff53602b50efb7427f9df16e5f3c88e79218e41cb25220" },
                { "sco", "e4f264c00bec4bd548ee7c4a32b8f9c905930692b4ce1c5f49fee5e2522685f92a2f09339705d0b1c49989b4abf9a6148f859363983b4aae84474fa925f59637" },
                { "si", "74a0f168d1a9adf9c13783e674459202feaea928a5c0a53fdd12f999912f4080b14d843ed38b4a1655a63c189faf77bc68bfc3b41b857acd33bc6dd14a959d1d" },
                { "sk", "0b5e13a6506ca59d91cba6e42165c0cb3daecd6297829e60d80c1fbfe30918d8be3f6eccaaa5f8e281b64d58d6ef4fed308f39f224ff14ce4a9c5fda1cb460cb" },
                { "sl", "676b2bdfafdaac4e80c5b6ba6bac342a5762f663d828f0108abb3505bc97d5d4e8c63c551aff659103e1d69d7267c1351642d65956fbb7e71234747d4f72e4c4" },
                { "son", "da5681351b4388df55b3ed171b94bea8c388a932f47dcc827474c686c7b95d2d41c7e7888570c0dcb057ac39b9485daf8376087f7eaa8817b0f0e16c8de3730b" },
                { "sq", "befcbc63a54b50028eeca0e12cf015056789b54e21197b85bf1f29d568d4f9eb9627ef3bde2ce5f3aaf3d7ccb040762aab44bf6a0a94142a86b41a956be150a1" },
                { "sr", "b99e369f6dd095f9de4a8ec288cbe1c8f16ad63c40285f5b62402a9caeec6fbdd3a4a4482ef160d9cc75c3340404967c0d77cad4a740f709fdbbe7bbe59f552f" },
                { "sv-SE", "5316e2eaf2aedf0fdd965c51cecd95636ec3c4b47f80a8850044a4fa1a2a2817a2240bf2fc20dc292949aa03c131491114a144c93e9c1127c373305257e9a3c1" },
                { "szl", "3f31dbd3c5d67af19c498eaf940eecb78391687f2809e06d94bfbc390a07378e6bf3729f96d5b6c68e24891e4d672e1c5116e2f1592a463212e1f4c389b52b70" },
                { "ta", "ebe08557ccbaad92d81be248aab6dbd8a99154d77347f2f74b700d3fb4e1e0044d16078059a3a6aaae2d38b1625aa2cabd1fff25bcba142ef193778bf8765308" },
                { "te", "62052f6540f5abade1168a21914855d3466d3ec9ae40b78582cfa6e72cbae1f73dc38c4dea6bb2afff6a1a30d67a48bcf9893aa334cc711c4a0536c802ba6fd3" },
                { "tg", "5a111de33749e39ae8b1e6c2b9da9d807018dee80174cf4a45e6624d796b7bdaaf2be9d65aa62d2f4038dba4858390f254dd2256a4e134471bacb0cbebf0cf10" },
                { "th", "7b4b18dbc26f633d677e5e084e9669152c403cd5ca46deff9fffb2c4e19e7f35634a5ec3511520a08537748e3ffbe5e9acd438c7efa5e94d62d61076646af432" },
                { "tl", "6420fc588377f936eeec45f78e1c142fab6b7b20453b1777e51d7ac76181e98d3d386a8996d58728f03af6b7f7b48049c720b60011398642099c0abd2a2e6cac" },
                { "tr", "51bffee76334687f350e6489bb7e89595aa8e8e5a65daa20b8674a75dbbc5e38a6f9cdebad7365ebc9781b108c4fb92537320b4ae84abd2100f8db235a97300a" },
                { "trs", "acd432a9eb29fa16e7c9d2e68e22abaf75a75c6742b40e65c1d3ae0675d22dff703ed60a01ab07e04e9817248702d20eac39ee93610f1b6dfc7f25fd1df624d1" },
                { "uk", "98d33bdb7a08ed04867e95411e7253cfe8b67662e1b4036a72a96dece05cfa991a7cf27bcbb180a49bf21b07bfa17ab9bfd008e55fcba23231efaaf3e310ecf4" },
                { "ur", "472651007c3278c1208c968bc1280efd9417ff370ecfa00a32d60ab708cada52f762532384eb281cce0902e87706f8bd7ac957992cb7ad46bb1dda0f6b2d113b" },
                { "uz", "b1bb0438c41010080df760bfc08a85856c6c4cec47beb61cf3ab1c8a3262d2a569f2c98f476821a7ce047cd08816141e283c314d7e97527908c9a8fe113ea7cf" },
                { "vi", "bf7cf54e959703b0ddb964e84208b8716971e05854a40f45663bc874293e8bfcef36e90d7a493a3c950aecf9563fe6630056909c2823dad11da12ebd6f589843" },
                { "xh", "adfef14cf64fd8a3bd7699ef5b87cd1be429bfec3b6517ea9324a6d787fe19b72d4b0f97551f5df8ac62e830375d5ef96a9fadbc68c2f9639a7d160bd2bce493" },
                { "zh-CN", "d1f7ff6353503fd73515cd11637c2edbdfd0f01b1168c0ec930b92bd5185e05b8b746811f66a8312f0a130501b359d1ee7bbbe7317428ea257b4f768392bb775" },
                { "zh-TW", "76cf7ffd6e9c54044cd3292e54c65096bf88f2988cc6fa966142853978a7f0ad19e972d726d99a489ffab7625655d5fc490af43e97277730b655a504ac511d43" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/121.0b5/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "218f2c58e0686117aeb3890707ea8d7f6c4ba4e168355e47746fbc22c965e5e84dbaaeec862e58e39f4714f39fc2531184fd204fc65f5fb7061ed8a73c441941" },
                { "af", "5f8ef651ff75ebf164cb94e870b57462cf95f3cf44220efe9cca7c2e7e19c470f144475d6d7cbae10dbfaca7e978882466c14ce1a3f02e13bd81bbcccd31f896" },
                { "an", "1849e462dc5183c373caa1cb7fe1a850084535fa8708527726c381603e972e528cdb68b5176ead2d4c89d93a5cfd8a2fc687638fcdefbbe32d7b47df017f21ce" },
                { "ar", "6f3c7e8bbdbcb390a440b0e0da50d5ae66bc989d08a16f598cd8ca357ea45fff2591160db98021d120c28b8402f7567c35d6567b1f92063bb999f9da2a1431ab" },
                { "ast", "0a7dca17127f255f013aa5f70ac3dc7166827c7e842f880943c119f4075d4b31596d5e148709805600d47c150d7b6fb2dce8e33950a505988511dad522a312ab" },
                { "az", "c182d90472d7d0f3f92eb215c14ae676ccb54f471fae1c15571a0c4d210784aa1fdad6a2df2172b978692927c32f1fa8c60ae2c5649db8b429c6e819071c5cdd" },
                { "be", "eceae8df492c3d21072b252f85f63acca84af1d39534b26be4a73ba0f204753679096e48987f107f674674a633d2b3abb593e97461bfebf2382b8701cdcc20c4" },
                { "bg", "5df6a892613473705174980602fbab8d115eccd2cabf7144af975ca8f4f280b3aeb7f58799a85e618632ba1a0a418c1e7400b7c7a39098bf7a062065c63db814" },
                { "bn", "f68e4646df78e1c1d4ab979668f23e1bc7c24a8410cd5fbb35a43eedef968e8f183c81a2c91940b5451906bf90b1f420d0e70d66ae32e898b9971a6a40639d93" },
                { "br", "5878e87d72d12f50dfd7ba770a16e9a9ff14ea5727c7bfb10b13b48f51f20691d387c1c44ceb73d63ef1e2285c18dfb85c11969c4e955b4c80378a9996cb7b41" },
                { "bs", "9b9d09698c10dabb0a63881bce9d0ae56f10accf09f9063759b9ec16132c1b2d07071a94eefcfa3b50b9dcf73736baa072b350967c6753d7f8865cdead429118" },
                { "ca", "68c29a104fbbd689c052c5e2b0401da5c8b74e30e859daa7f733f3aa73e79a3d829ef91920af7a635778a52beb0ad77ff3a5c91ee0028ab9b5b812cb15463955" },
                { "cak", "cce0b19c16f1f129ac0446d5d9a2220437fae685766996d7282d7ee58ffb0fa0f0e7dee622a8755ccf6cd47394e68e4f26946b157259c60aba94068be2bd03a1" },
                { "cs", "6b29228d65b86078ed276bf6ce3606f4d5c7fdb902911fd52e5c55c5a181abc4b5b191eea502c06979fffc44c680de260687241054dd9230908e5431943bd271" },
                { "cy", "0943c3467d81a275bb333fe6917f5236eb20317a96fb63ed9e7a2e64bbe9f564401eb444c8011b89a76aca55ed57c1818dd4c243d4be23ae63ad3e3799be7818" },
                { "da", "c8588d41351fd407fe01a9984b9b3aee19f767e65b2238814f2136fba60c341e18246a6700a91c0a2eed3f4c00b10a41648b4504682ab03ab606a6492f3d2841" },
                { "de", "7704588f80a24da01937bed9e016bb160bdbfcda364f74c4555d3a041a83caee9a92c8452e968ea722b4dfdcb531d5838e58567cef2190158da513f6e153a70b" },
                { "dsb", "a810e1258620fc5ed7eecaaf8627bb769d161b288bfb5a649a4889a5edfd08681b7f95c86bd5c420de70673c7f4f625da792887f9cca613dd2aa083df42e6fc5" },
                { "el", "9824aed5dbf2c45a04cfb6b9050502fcb350a6db2144fc0c401c676ce6afb8482c2a62e87ffdcc7bce31388c10bf36dfe0988f9f4d574ba0bf1c165d88efab4d" },
                { "en-CA", "a2fe70e87e6dcfd57cee63eec573985ad4bf1759b8ef8e913c9e459acb85a3c50396f7ec13d3e6af0eb470b02c952e674f2c299757e5168404b397f13ead5783" },
                { "en-GB", "e5c7989ce7fd8b6a8f320c1556fd37af25894520a70602a0096aebfeb9ea0868c91970b6469797b806d6f478e6271f1615cd7d9d4043360dda8448de43745df3" },
                { "en-US", "bebc60d55dfb14571c6a04918b6c7e22a93d44911ed84c76ccef151219df4a29307540170e05cb682604ff15d1ae0e084c600f0031e350ac52356b363c0f7b02" },
                { "eo", "63b80032f550b875c010f4e2f38e3ee3284e669b9e6d779a5fdb22de61c3f35c7a70ab650f659912797726db7a2bd7d8ccf3d17c0943ae37c7d66f22b50198e6" },
                { "es-AR", "f044d85ae8dcc50da723f4911911e6fbc2305e9b932d39dcb50ff6638dad822c981989b2c05df0541241c16349d399550ea16796028e5129819c3343a0e03889" },
                { "es-CL", "11faf794983f0dc75c22e0d27ce914703116255c0224e2775cf9edf09f53541abf24ddcfcb1738c898338e057c08681a0db0671b77254c4532961b43014ea289" },
                { "es-ES", "9a387aa3195fd12535a6d86918023c456bb75d8524394c0ea0aa987a52e68c5574e38d1c6b0dca98fd99532e83c29e033e90f846bebb81d64446b67bcdf9d1bd" },
                { "es-MX", "0815992844ad4bb35ebaae29260fc7b98a87712ca8c2a42ad2ddb320eb5e8c32608a6266bed6a11617abd4b1a81e2ab285dec1223616d82641d4fe5188ef9020" },
                { "et", "8d27c474e0290943165c9f46d7e0d48dd05134d2bec2b3c7fcce6ea0a23dbcf2fe32a5f7bc1b4b78495309abae8883e635d44cc315d5f092dedff7f74aa32122" },
                { "eu", "28a2e84aad3a12166642e5dc9a8f7305916e855b286151b13077bfc42fcaf33fc86ea6a5ee44d01ab310609c7324e40907d2f0aeacbd543a0cf5d6e9be276758" },
                { "fa", "8a5c1c0fab8f6087f7b465afb6c05c0a24b2f713be708e94ebe7466847b711996c96f636a9e3eab93d66ba3dcb3814f7afb04ae32ca11247fb897c8bff075eef" },
                { "ff", "f1bb030b6164dc13e7e7bfd6a92f74d40aa5f4bd0db63abdd0f9dbf787807f5139912edbc7f1a154d4d096d42053e0908d02544622a6e1b3423aed6e7fde9604" },
                { "fi", "3e89e14a7262854c0c73b49013561a2cf31cd4f2cb21a8d966e28479256f687fa49ff14d2d6c843eec806aec0588aab12a211fd44ce81f53e9789978c1f8ffa9" },
                { "fr", "61e43e81ec38138574f8355ec71b10b5f569f894409ffa41c8db294704b9303983f64bd6a4d8aee647b7327fc181c33297b1618bdf69649be1da3f66948b58f1" },
                { "fur", "8233ab2016748f59cce1b456fb99a93b77b801c1cf48e36fa029c5fbb6ed29f13ac0fb85d0e266721064f781f1d57854f8e25e441328fd7e5d7ff24f4941aba1" },
                { "fy-NL", "fb30e984705a48c106e1f873ec0792638ccaa40c5d1944284e175fb1c9727b98e865abef97b302772fcc9005427e6bc2f7c0ef60085c2d50df3caaaa815ab105" },
                { "ga-IE", "77748a80c5f677b03ce22ee2e1343e4ad4f08d96b33499f053425ae4a1f9fcff28ad4197e5ae4c729809eceab9b2eed22ea6f930d366acd1465eacc8fd51ea88" },
                { "gd", "ec1597ab5a9e60af77c4a5a2dfb5a29bbce9336352c14465ac1a2037fc3eb2cc630c974d6d21cdb28a5c23b74d1a617615f55591ed8c5db3bf740a978c952e7e" },
                { "gl", "e46d60ebd888e0d14cb198874d7b1d3cb09e5a3c17b19196125ae845a2844e8ec846840667fa3305f53e2b74c54e1ab3bc2e69f900a29e3a9bc30f807cc0b771" },
                { "gn", "8ec778557d0053d5633db8281bf3ac2441662f0b4ad49317935175cd5202a7c3b52018772b55d07a4b3cd29c1f37f626af941bd0f3ea2b104538be089200b6d6" },
                { "gu-IN", "a17182335711af47d16b2b53efb05b005b6f631b33db8dc35dc62ddda5fb928de0bf0afafa57a9686738a383f425fd4b9cdd545e02969cb294f74c08e2c85a1f" },
                { "he", "1ec80bf8017e2969c1b7741a8d94d1e6470216ef06d3bbfb6ef7ddf30de06168627ab9040f4253d7ebd4d467bf0f8d5705b188c129816950b596515cef3b6316" },
                { "hi-IN", "f27361dcc28e12c6e8e8206d8458bcb437a842187669e8e6677e4bb8b9d39488e2ec09ec1e5732f929b4c550166a7d727849fbaefda9f691081eb1b35c02e505" },
                { "hr", "367a7acba48c85ec436ebea37c5f05ff9108c8d1695d88e4f6753a5f63d5913af99f6dd9c3fc84923389eada7e1dbba38e69d6ba542aed49b652302a6862824e" },
                { "hsb", "e20f19c67b60c00439a5e0657a8f19288b3c8c27ddda5ab7ceb8a39f68329c14921621d08a493e6d00d383dd988392aa3622c28240914ba83714d539d0dd6726" },
                { "hu", "a14b8802845e49c1b9600ca9a6180a67d765d8558411ea767e1022b3db5e1aca4d6d1cce699be9202555473017e1a6dd62d24b6e3311625a81490674eaa251c9" },
                { "hy-AM", "bcb5cb9ad09a589a11f64e5160a4a8757714364706020dd1f5141ebe4f2189be307ffd44d99cf4a69d94ed87a4e8b41696a3e85c83d99acd2ebbcadd987dd0bf" },
                { "ia", "7b9ad4a74c5662e4729becbd366b11d6e50c901c54199e75b9f237fe8dcb0d5e61cd800b526054628f643a4e0a9d9fd2afc3e74877e6c04786d2379855acffa4" },
                { "id", "bdf9c5223d27d09eeea3a4f7e3e4fd4ee34d3bc1556f15573892bd9d9709aa9eec87d6f359efdaad18fcbfa3541b904d85fc704d4681aff0b27075c19b48c32d" },
                { "is", "5214675af738fb0b38c6369ae5280079b276ed56a74951f4ab07121876f6e8d471bcbfd86e7dd76015351cefaa555d1f32e8e69dd60268ef4cd842f6ad7d436c" },
                { "it", "f59ddae1a211ca3cf81ecdf37c8a39a2af21cace454fe6f25c5b978e0316f47efe68f802f1149127e2282ace1da1756f3626a874d67f85d9f1c53e57403fa231" },
                { "ja", "17705379ba9bf3f39efbbd05a9dc7f6c2255eacd45c8e89715e708d51e0dd5cb4b1f7d99983870540b539b4711caedddffc43fd9769e5985dfdd93446d5be27e" },
                { "ka", "b59aa927714de6670c6f9fd066ab078a4d838aedad76d7287bef7a1c34d1f475c4caacf75793ddb376f9f8a2cf18f392e7070dcb1fba91125f9604bb25794d9c" },
                { "kab", "27f9701d23077e7287f39ccfef81e7bf4c85c2809e5dae4ac508fa0762bb91ec45565e432884c4528851b8a946f0209f13d604044bd984dddf17558b76c4eafb" },
                { "kk", "55903081f19725d54de46cff9f4f66cad0b7b059f1bad7f85aad0322ae38948d40d97deb19bb9ca9a00ef5427402a4c634e42dd44d7757218074ead0269c5e91" },
                { "km", "abafdd61adfb4c78f5dfea201a689bbda4090a643e109f49242dc36662a0a362fbcb214f400ed722347882a200cbad33151242137d5b8fd2cf6a1a7b5b27fe97" },
                { "kn", "487321797bbc70807f7f45b3b261156e11e15bf06042e7d01e421c00f55e534534f14aa22f10033ca13a2e208587cde3432cb35e4dc076c659d76e0babfaf249" },
                { "ko", "52e66e138acf3c469f6131631fae7d70a8d57c053097fdfb54771d66ac3fdc11ad024b1cf60e857c2340d7a0a21c80699f9bf0095cfe689a455429c3bf0eb841" },
                { "lij", "88fcf3b73bea8462952e63e71159add3c4e4bbe375a800e8edeca5e008424c3e156a22f7412de80099b36ea700d7c4aa38ff62238f28ba1f9fabfcf0f7c29dee" },
                { "lt", "75c0c51fa0735c8220104825db9609e3f3d41bab133ab6b434d7b6910d595ff870edc6b1741c7a73da92ba4a8dea18cfaf22900dcd6d83a4355540740a012fae" },
                { "lv", "3f962b346f0114f5a26d8b774f1fbb089d5a26edcb0c66a97ce839de47c67b02af36234199fa8428727f8cff9b8448952c05d1e0aa23104fb2770adf460f4503" },
                { "mk", "f0a49375e18ffb38cc37f2d84eee408b51d0f742be6590304e877b8ed218bc95e0e52bc3a622fd161db7e7540947a532e1b5b919bec89c8524de4882a287337e" },
                { "mr", "3d1484ff81e6e612207dcf41af7753e015092cb2a0fc502708a3b27f2bc0c7749f5ea94608430fe8955c606ab2210e5375ef0b7731c913bdecb6065171f2f485" },
                { "ms", "5628ac3cf887c3885a20a02595ec43c8e3082b89f2ff73dab991aef92e5937698e2756fa8a6a2966459f6ff8304c0f8ea2ebf56bada74a7540370639ef227dc6" },
                { "my", "67a7c31937ae1dfe4c99b678d1580858d6f4571ea2b059e82dc5b36f6abd146937203d3e92d31d08196846dc8407509ffbee24b62cd30cbc3aa94642e2b2dec6" },
                { "nb-NO", "8fca604e48b99fd5a5246e4de400fb7334c991f71bebe28810321be1bfc759e0d9619b5c270c246fdb58d99799cf733b92e7387159c2b3a6a2baf3748461ff09" },
                { "ne-NP", "bc9a73416b10f4af6b8d510074887f776d7aec8e1db9c52c475c007db4b5127d91f3206ad06210c4230ca5f76b2ad711114398cf32b0a92f296da4f2169b77c5" },
                { "nl", "f8588d7b22eff67acf5e3d57101b9aa939ed80c5527343c6b6239f44ecd5d7cc1bb1e0d49a5b05aad7e625c2f861aef5fb476a5a5b5ddba289509394dd9fa522" },
                { "nn-NO", "39610f1701167d2c1fd7d4a50abb52aa3851b13dcde13952e18bf46d9afa539d2c237e0b2516fafae91b0e10c2c426dd19de068fdc03a99870b82c43ef575622" },
                { "oc", "7f08f7d0fd3617c9d9c018b6cb9172cfa86d432f90680dd560a8457ad5492adbe4eb3cdf00d8ec17a69947854cfb9e92bdad32799bb707e5f0d873de39275a1e" },
                { "pa-IN", "adb6b6d0ca2360b2826fe5d9411b6c19a21718e5e3a6b4fb4ad3d3de64b7554237a737d72a196347b1f1d3d81056f233fbb0d6822b22deb8f8d163bc75f2bb92" },
                { "pl", "106189b5ca12aba6c07e9936457ca925adce2102a444556cabd540e02febe38f0a0e38c54181d765ae6504c43b16e174a03a157e12ef583658aca119e66fd86c" },
                { "pt-BR", "7071c39e6c1e6e8c00746aee248004be3674727e53e4bfced494bcdc240698cf058e19162e96c73a4259367dfffeb1fd08e2bec3b19a7bacb686864fe393a927" },
                { "pt-PT", "b988c45bd3bdb1b937b20e52c1f8ed919674b6eaff93c310c8f824b4aee4aac107f009293a055e528878e9148085348368b1d699f36041974d9c35e4c4ce5bc8" },
                { "rm", "bee0610de7256e22c2d8df176ff05768ff11b29f0ef66c4a7f659cafa27b144d41812d21ae9559b49f2fa08468cb7d83e903872e19a8a7f6ca6bf1803400602f" },
                { "ro", "9cecbf391e4a3e290d79b44d94b66bc13269ffe7880ce1069cb3fbd294750ac38d839603b8151c0e235261bb040efae5412e15cf99aa40924dca27eab3c5fc6c" },
                { "ru", "6d3da377f2fd3d7635cf3d325287af3cf1d76f154689564f0ca10fe1e620c0c564374809e3d6241ba80f122f5921d23dce9584a9415f1f7d6ae7d34b6f896155" },
                { "sat", "eca16b326e461c90044ce8557e99ffe9403fb111bfca9bb679fadc65707008a3bdee92f0a7ec63e10b6fe54e31e9772aaa2501f881d5df8197dd52eb4094f23c" },
                { "sc", "bedd8f8008201137e8dcf805f1b97698264f2483d96a460e70ec89b6e7686bfca28f07fb06223af8c0f67915bea8b360ee11243eebc6f3ed5e7a8acf5b20f6ce" },
                { "sco", "410ade3983353e39d24aac52aa9438cda22989efafe56b54d26cf1cee392294ef4891cef88f5aee899538e413e014f9920b24a546fba43e6c2072f2eb981cbe4" },
                { "si", "b71ccc01bdae228444a5e64b7bc62273e219c4b517537b48cafc4586d279a128eaa097c0e2788e99d4fc67398748bcf8639154c9be233614786f986739eae66f" },
                { "sk", "e2bc94cb49f48889154b44ec753a04832bd40e3577e653bd0d3f5d76752a9c447bb28ac1dbc2af57d4bea65933042c249cd1434735bf149150aca1adc92ef4c9" },
                { "sl", "37b99a690d96544b2394016a41a728851efa6a9fbf436d8e729ee65380f55c866c543ad6077a115f6f5aec0ca1de4c3dc918062843c00f565e9d0b332cbba531" },
                { "son", "7dc46bf2cd551f25fb5c8be4bc8432ed65315f896d7bda7465df461d95004ae886cd4330c9697551130e4dcfb353150285eb9cf4eae8621e4d10475ca1e2aa04" },
                { "sq", "ad7c97c896fa18aa1d845bab78689296ef3de2d311a0a36ca33645982519e30e34f198cf7005eeae2e0901b82c76f47a3ec171cf86605f3e23815649adaf2a33" },
                { "sr", "78a5057f53ab8fa7a08639e07df33e7ec58392a404bd396859016059dc11ea79f38be1829cee704f1f4b254cd63c80d51cfe468900e2b63c3b73423e6fe0dc5d" },
                { "sv-SE", "386569396814c9455adbb220819147344e2c8c554f82cefcfe5fd190ab85c24d9a861287b5ae22a6d1e08bdfc258bca157fa2d36179409bf99f5d3bd5bd0000b" },
                { "szl", "50b2c55f40bde0e24e83a56710e4d93876ffcf40ca8a1b334abc35f01904e90cfaa7cc969ed61775ebc1bc29f7889720c711df7d84514895005a8170ad649aee" },
                { "ta", "861027d577a01ccbf8e501808f4fca9efa10073a17eaa8ef46c51b586ecee1673341832cd75dd81a4e9cf42a7528a5988ec729f2ec6a4dcc4dc34bb7f7df50d5" },
                { "te", "674ca790c6e662a7d2304664c4723fe316ff50556a19acd99ed2241b2a5cef21cfd77e4ac16535d9cf15e03aca5188e8f9b51d001372fd6988fc910b379887a1" },
                { "tg", "2657eb4c518b13442f75e1a1626ceda0a4f7fa3d0821e8e831aafc919f586bb9fb9f0a83e162cd51f002e7b3fc912f0e6743a350c20687eff50118cf5941595d" },
                { "th", "14776a87ffe1852d99a385265b18683c3c51a0bef739a405966e250eae9e479def16f9eb5eaff482fc8806b7070fdbe86c89254ab9e5a55c0ea64e8ac35888a0" },
                { "tl", "6759fa083474939aa35d00d7c2ce875bcf1b0a2f9c5e88570c83067744bc58d0c37782c359884db48bc8813bf21a2224b2d5457c89049a994d563e77f53ea9e6" },
                { "tr", "f2e0e88ecebc81824e39d242874267971523a1d97b157c993ab5d8e51eaa8f0b11c535fa678e6baa4aeaca2cf64318c516f55c85a98e21b0107a186025ae128c" },
                { "trs", "bb739fc537cf25da8b6cc3dc466785965470c2e04dfc3ef9d76488b5b2847cc9df555239f40e98dbffb42c4cc7f9918fce89463a6cc82efc33e51f3bae4b13fc" },
                { "uk", "77b1d10042f05feb53a416963b6eebfbb8baa6b26eb9bd6b44ee644227f0d0f88310b99d65155f3926150156cf50c44496ece2926759d7269e92d1d6371533bc" },
                { "ur", "1d722d7f47aa37ef99296d72f98a15de3a3306a80ce74275d10a748186c5de5aeaff4eb31ccd7d30b1ab6a8b6996072cd6fc203f8c509c7d36f3c6e339b69ee7" },
                { "uz", "45f5cf2e0608e9af8075b47221dfa05fca22391096f2f3192095b7a3b0c499233243d56983795edaa31b84bdaf9d61bef399ec5f027d6a3022c42914be90cabf" },
                { "vi", "5ea19e8cdc10b959dd6047ac4a32dc63394ac5bbbfafa5f0ae628265999de7b340fdd8c347e8ebe32cc8feed024bae6570b942a84a2610d1d7712c63f4c900db" },
                { "xh", "30590b8d01a34c1e8568205a928d99b69c1867462cdcbeab015475009daf2a851c9f0f3e8ea3699d89aec03b9543630a815c6d3ffb09df15c510f15f02de6353" },
                { "zh-CN", "2d4ea91b8c2b2fc21ed8586552da1ad86027b1963bbca9f1f9ddc69c0356f6517df376132cea558679b2924493db2422dfb136c04807c0d1c743d9007b1122c8" },
                { "zh-TW", "1f3d81611d647936563d26fc6a17bc104c02e2d6522e33a9e6ab893302d0425469906a4b8a58d1f77ef1c0f54a32240e34867d276fc7b12662ef454fa3112885" }
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
