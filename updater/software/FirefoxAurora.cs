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
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "105.0b9";

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
            // https://ftp.mozilla.org/pub/devedition/releases/105.0b9/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "f10b11feafad2c01eb11a2e8bca4abaa9505fd788bd0eae4186460da32c0ac2795d9905eb8b0f20f158a319682c7a3824efc5a6c1b9c67e2b09ef0bf365177af" },
                { "af", "258b30d99d758351f9aaf62f43c309dba33505c35a0f54656364b63f3845f1bfdcdd6c6808c8b44faa522461c1e003393b9ceb8e887b3b15e99e610b895c36a3" },
                { "an", "6a9f32ffd03e3c8708c05f9af5bb9688434708d93b2aec513609bf5cf3a35226b6f28af55f576751f8fa38cb80084c52f164b8f23d3bbedd28ae9ee55fa97fb1" },
                { "ar", "c5d7bbb6d0947fe7adf7338302d1794a2f63ae59bc86ddfe6e17932df35ea5739ee34567cf8f2388b8b3f06d3a9a6e28aca1eac9b7fac75efc6bf0317b4f7e51" },
                { "ast", "ef8e7153d4c725192a0aed573e18f9e2fb96c18415135079c67c01f6d0e100c4a222d8c4e5630bbbf9e11635a1708eb7b03c62380597c9bf855bbce967074d7e" },
                { "az", "a75cd40ac34ccebe9334c1a025fe29bb29cc2cd469c157981348f3f4739e57e53216fe18c3dc5314348096a03d2f9339cbe5353301de15da88605f8815792ccb" },
                { "be", "4172582e951b998538a41b8e0c5805ac3892bd3c21672cf73e6845ddc9cf6c7f26d6f20dc2d43db88f8ac1532fed149a8dc2d83526643dd0599697e241caa5b8" },
                { "bg", "e1263437912ae85bd6b8c644a0340a829db80b0c52b26a94678132cc2980d8d719232803620775e3ba3f61f59b2276323302db3ba697bc5c5ff3f62be433bbd8" },
                { "bn", "3b0c83ad6118965e2a705744d8d292d7442277cf9514c93aa3ff906b93efa0d12fc7f2dd54602f16b70b8a0bf146e2c71dfb0f694036f7cdbf3799bfee1adcff" },
                { "br", "cfbd70924937d93043d54e51173201adcc09079aac5c6b1be08e898e187aeeae61bb2e3aed9cba28881ad753bbc72de0ba653b68edf61aa7f80788264f4a292a" },
                { "bs", "91d57f54cf2471e2031c67fb815117723bb4390441a0fd35da31749b6d2414ccdd2ffca23f133f8d20afb7cc3c1bde6fa14076d77fa18f6bf8445ab89f53906f" },
                { "ca", "b8ae1aeaa60ccf293c9fc000366e855dbcc5470eb4ea35a651154b90e8f1e1588df4e585761fbad7bf4712be1e6d50aa3b0db2d9f370ef9f2ec001b67abcc2b7" },
                { "cak", "4035f9abfc3d5505c9d2a7b49549ce0e22095b7f98d3ff6ce394e4eb090173deda2b66c57b076cd314d8aa07993616261f87cedffa232abf9d7f363882dd79ae" },
                { "cs", "e9a396cfb1b173f6893e68384bf474b0f9287265e42b8d630d9aeab3268d37b1091c2e9c5b6d25f6292c66a72249631d48cde98d51dddf0b314cb67fb5bbad27" },
                { "cy", "848e22e3b76ac544b94bc528677862cee923293cdb1985a38b9140f9608a770d9afb49ab1f082c229fa3a0bf3a1e3d268bbd92895aa354c32f4fa8ffc4da096d" },
                { "da", "b5f05f276018408458075f0c0a58283c8ad6993dd0f320b65844c5e1bb9b866707038be36929e28412282040e8685da930e462c59d5010bd198655f9cd2d60ff" },
                { "de", "e4ee32a875c49cf5f0414fc8be99290d449b35bc36f72e1327c50f7b3391c6dc6c059da16e45c03397f4a6e2cd2512388137653596350ae7066bf66adf625b2d" },
                { "dsb", "147683dc01478b4787cc6dd7b9ac7c32c155b98b284cac254addc0ccac306d72c09159ea64d0574b7a593d942e1e8b07a0acb8b6b8b730e7c5fd14db78020ba5" },
                { "el", "76010ac20910beaa0d87603143f290c42fe2c5141f89c33d8a52403093f38dfa8df7b9acf911277547279f8c2fe5237299e5c1d2f39ed9c3e936ca7fdbb1058a" },
                { "en-CA", "fc02f51b74625f03b36ee1acc8bb989d83c6af75ef34d065faec3b393256cac4931f948ddf0f6e755e457b99edc1aba84c6d160aa5eb8a37bbdfe621ff7836a1" },
                { "en-GB", "9931954379f553174aaff7850e5522b274fbcbced0a60637ae02c851d7c4557d677c7698afb88f09ed0b491d9f5cb8130af42c01904ab4be1459552e4a27b032" },
                { "en-US", "9e756da95a22d807fd2ba4e7448fc6587bb3998c216df7042e8c45f277e75ee0db20eac7704da7b23c935af9ebd2ddb76070cca7ff39cdf3f2508db7e8b27e7f" },
                { "eo", "1cf3afd76ed5deefe9b552bac83659386518a2370d80933b3318c1a4fdb57f2097e31124c8fff37e25989d62d7e4ab04fa7e044109f2ceddcec20c8122f932e2" },
                { "es-AR", "98a298b22c739b85d447b8aab873f3ca71bf61c48afd009fd58bdc1d0b163cd19a1b442e1eb6a0a76414a01fb5c4c70f066e7bd9deb722e8db25aa53fb66f087" },
                { "es-CL", "2e90bc8ffcca562b4891247284231b8640129d63e4480771243381216d43b854aa691360110e01431c81b4fe8c060363a1664adb1889fafe54b64275775352d4" },
                { "es-ES", "224537b03bb94a53b5a2a8a04b7e06201c8de19a9489dbcaf8397a221f68192df8a09f80e89efafb8a7821725ac01498e394eb04f1d29131906aa5caf49ed1bc" },
                { "es-MX", "88c7f05d0c974757b879049c42cffb0d9aa19a1607f864c836c7b056660b14346a9a37719f69128af0468da72a9173bf56aecf658001229bf9491f2b45e1115a" },
                { "et", "fc6c8aaf5b271e21bf5585b1c6c2b2037eb85ac7720d3af4e82d8122da5bd479199998f3085fccb7e3b856ac616007b4d032f23208b9c0a1408180f1b3c5488b" },
                { "eu", "47f2ea8241f12bc9fa22d6a6500918ead2bbc08c763e9628dd13bb4faeea8abb36677cc29ee4ce6863e2003d1a004e0868637ec9bd8e3f0bc5f86e9109f256f1" },
                { "fa", "ad365b8208bd755c081ea72d5e58b31f1c5276f31583b6c45e227323220f1e4872c615ac419293e17b591938c0d17801673921dbc45f97c6a67e3873b95f9ffa" },
                { "ff", "46ad5d2817dcc34b5d162e93440206255f04268df5491f6baca6d537feafc9e7590ad8792b2a6206924b3e51718fabe87e2d2154f645fbfa6c78f92eb4d79ab2" },
                { "fi", "02e152075da21006f81d99afbafddd0070a6f5c323c0b17973c5438fb4f30ec536575e37931d85eacdb386d6c6f26b94cd2c75cd87f36d422cc68c243767d218" },
                { "fr", "be068310e99f64f093e6e8eece8d4f50620b934a4b4e25446973e2c2bbcc73b9ad6ec44b4e927fe9b9f3194b5395be64c24a9bfe791934738847e07f5c8c72a8" },
                { "fy-NL", "45e24f7a0b7196198e57656ee2b890b71afe4feb8caf7acc5c2a97e6854589c7db1c726bf36c3779d6afcb66c9658860c8a3767376b1b49aacc25bb1f6eac32d" },
                { "ga-IE", "c7870e62ce4339b03ac0389809ce3d115dc95a974a09127cb08adfabd4f11aef4edae41a3a4e96694306f751ddb96eb575c5bfe445c8aa8e4698165165c292f9" },
                { "gd", "b83748c8182a01677dd50dc927df3d87fb3353c9a7b79e509e722dae3d925711cdc061e465d8d2e19c5c73b33b145f51964fe771b14fa3770cbca67a869b0b88" },
                { "gl", "addee503ef45dd143232fddea82a8839ed2b5d6f2e1137735bb2841297f78f1991fcd8a300e47083f7f976b741a6943a6df1f02f84ca55c01875161b72b435da" },
                { "gn", "b0470cf1e4d59f06a894864d188494b902bb256b644c38b9ccec59c19c2c3277d9a3ea4ca191b033a69261ed7d333b30ade8d0f025432815938091cacad5df52" },
                { "gu-IN", "1120529f31a27caa7ff571f40eb3361485668287e7f580538f79e7197faa6688ed6b36557aa700b4e56e9dc2ec5363ddff64738d55996413eb68c95b0b7cb253" },
                { "he", "62903c1633e9d7c4eb9830e9d041ea40f0abae15117a78edbeb8f8460d8b2ca8eef2c0de71e26f346f6b8cb2eeb808bb527187128c3320ec59c426b4560e95a2" },
                { "hi-IN", "66b3bc0be761dcf2ba8869996a472eb296e2a444e8a3fba34636232bff46c1a4241f448636f7e2339ab9937833b08436ff554916653cdc12e46eff0a4c3b524d" },
                { "hr", "d864d10cd2e8084fdf8e1126c95e3152152cac53aedaf0d62b01a0c9ba4b845ac0e110033967d77167801b089984ef3b43699e4e655221fce71a8e65ca076d92" },
                { "hsb", "c659d2ab415b67bc1a1c63dc0cc0140a01e265e7c5a39a60a40225a0a89010385124ac51faba48fdc90d1a826e6d995065b1df9eb987616950f831c71b56bee1" },
                { "hu", "85520404957742291c019ccd2320ef79a969c1c254b72f579eb8e2113a1ee2c76017208ef0c984de2f0126f07edf54181e1a6130d741a9a33f5f96fd707666a1" },
                { "hy-AM", "bdfef62b5b9fb8603eeeb00bfb748ade3bc2da83b7b6c7e134500780fc745652a87b2e4000ede239b915b0b08d2f5a814fce9eceb891c44437871a22751a5dc9" },
                { "ia", "a78b3ccf42a913b4dcf85464a47aa839c78c7e7a3cf5585ee32703ea2c8ec770286da558f38df3658d12653805eb9634a5415007906608bae363c7aa2dee1025" },
                { "id", "b127f3f6437a4ae121d8ff07c93de1026261d2baa3e1aa92bd1022cd701bae9439392eead90a89b1c5da8f39ad4f0eb0c109d16b07ebe9ea29825bba574b4dbd" },
                { "is", "d5bbf10e06a6b98e45fad8b5a09a5ced636a4742338a1dc8f9037862f63a2aa6eed2223756a4b948cd6edbd5f2b751bd091bdc2b64b6466acb2ea2c7ac4d7ca7" },
                { "it", "b7f268062da41b3fe78a36011c960ebe76f5292610bbbc5cc4e316c6b67d37821735e377c8f9abf7ca7baaa0c2aec60d0f1ef65cb36e544b96b6b85abf58b44b" },
                { "ja", "d6c222ae0c06b56d6792d7296aa7a2a1a9b81a10f42f4e8451fd5205fa9e0ff9a94e8ddbad115208e8820bb94c8d6a26ac8e8ce785aa35e9298a0575488e3955" },
                { "ka", "a56ad25bd18ca1575ce2de438c6619c2a0ce544ab9a65fb8ffc7817d3b66a02b0838a9b7a269bd38e3d467f758d862a4a0ac872bd07a1d623c3a1e9c65cd9b67" },
                { "kab", "acb4e3abb514ae298aebd4bda3887a16de4433109af1e81c8a4a2117c68355051a69105f1b9d46b475444660ac43dcbf5dd81cfad819943ffd6fb3d180610b70" },
                { "kk", "8e6b3a037b36a6d2de9e2af0d50321b4eb9d410fa68989b3305edacbf03f85aa665a3a616c807f080df715e522d7e515f303ca1a18790d089c8ff2f2ae56cd45" },
                { "km", "ba98c33a484d1bc473f73b67e6109480dfe441c7cc148c6f61fc16f20b79bf09882abcc226d1f93d63a7dfbca13c7ad15676ceefe52e337b89bc551fd0f2fc96" },
                { "kn", "ea825178698145ace09bd4bcca056f8dbf64e12b14edf7b9fc2e352a088d8477c01876822710755fc0941e5435206256455f361692820d9cb4fc7bfb3cf6c86a" },
                { "ko", "ede6b2537fe2db50b03acba4208767c035785b9564b9a972010c1d426c42890469c4ac46ee204f64ff65649e6d4e22de1a0ef137ff1518f1ca08c8b9cfcc905e" },
                { "lij", "bfb368d58bc12c36e4fff19e7ade8e8251a717bf24b520b44e182a1615b49149efa892183a3a598484161438e75ebabfc2d3cb920efae0d5bf943f49c343b5ee" },
                { "lt", "e9f274859eaad2196013eb43668bb0a78b59c9d0f53f87042fa736445e72ffcabe8ec79906193648bebf1c6f4841e07a646689491df39efe203f9a44ffcd1c9f" },
                { "lv", "2d626d93a7ccbd43d9dd6cea3111dca050ce021c326cd54632c5fd09a292386ca0ab2762dc932d96e3720f71805d9b37c27a25b559653cd838ab9eceb7ecf89e" },
                { "mk", "e9c8ebc0f1811368756cd2e2f442399c1593d02778c11296c52fc214c3b6471b6dd65156e415591bd8ddff397cf57caf48c9c31cd455f1e900f9e0de5350cd84" },
                { "mr", "09083ae84278cac2cff6b6edf5846e78272db0311c3b958b1badc386ba2386069361fbf0e56af9891190928219f57de5991999c1e838bbac80ab290d6e9764b5" },
                { "ms", "d905c9ccb73f91ba36d97ccc304c4a5e75665100cf36d2a5a12a4f1d67a6908876119dc2943877a097c519728102a9789abcef67a8562768d4549cacec0dc1c3" },
                { "my", "8cfc66edd6793953ec3208760769af4f64d5480351465a04c5ec016e828523bf2f592240529a432192f4477841698fbc5770f4d477f6964eb15f4f46da021bb2" },
                { "nb-NO", "58c5bdc100a6489cffe5894b11474b29c6a25122d900ced89075733f5e0032e42fa9d4d3b0a811a2b5fc045b0b53b42b7189803899288f3fde079de5b80e9273" },
                { "ne-NP", "0c3f57605dfc9beec7d3f67d51d397a7176efa4dd63c22c69a00f7ab0e130465aaecef7e88dc3a2d2a3d17602d5829f1110b4ef18059615e1a141ad416cd932a" },
                { "nl", "833f48fe7f000119eeaea02dfe80e5f01e7800160b73414e108e6214a8b083f9cbcf9dbdce32047fee2add5cb114e1f40b50336ed762cb854703e64d58ebd949" },
                { "nn-NO", "2aefe83c26db2085f50d62d4c767445d692eb4f777258264aed937cdcbcb18d5880c781224ab1b5829329bb2ad108293c8b84b622f19587883912df3de42b420" },
                { "oc", "c4f01c0a588cbb6bb66d6594c80a613310116cd2cf4b077dd50c20f02e350ec689af15af42f0b6351885ee85644abf5599e11ce83bbcbb74ddf2d13dd7aa7da0" },
                { "pa-IN", "19b4325dacc00c69fd7030a9dd696801ab13d027251dd53b9984ad56f498d4104e2418ec97ef8c269ff9ce6f8ddcec2ee2d49cc6407bef7657e5077e7af7f0bb" },
                { "pl", "d6709c9591f64267ad192b4624686ff734661252be15db8059467280b584548853b722db56589200a492851edd527089d06c86e619088125b5f2451fad516dcb" },
                { "pt-BR", "7f55ce3cceff6bf9a66133461301e21c96de1960219ce7fe3dba0258de9aaea7b937f16fd14ca66f92c110a76ce6253ff368cadb32f5e4560ec9a2419212e323" },
                { "pt-PT", "c8998736ee97762fe68824b890e23e755fe89d1385ed75833984636aa4fbff133e42e2912acd45bd11da47c092785808b17883d4d58aab5a4193a33648a0c051" },
                { "rm", "27d21542af48fe4fd652dc661d28b37806216fbed6b2199f270cc53aad2251543a999637e471769d8ed4a5da81d63c7797808c47a130ce36754540a2364b2183" },
                { "ro", "9bb80957d9f1c56a5c4fc16d685486b2a635513e33bab001d9f59a233d64faed958f9850fbf92e424c939cdb775df22a84d1baf9a6285d1298498966b8e91be2" },
                { "ru", "074c1f8984e6fecee972c754c70adedae723a624a6e2943d191ac0f030eaf1f46c492c53d38667fb3deb905c1da0dcb98ad133b3f03c037648e513ce5301dde2" },
                { "sco", "39c1149f6ec01f853064482b08177294e274435cc533c4160022301d204fce3714d0cf91c48de426b2f6d331f508634af3b90ab0428ed2e1f840e47fcfd3cee0" },
                { "si", "993af5f0510fd0e96ac667bee4ea80469e8459bd48a5fd6f83d515d36086d65b93b36756f26b1ebd96b58c1258683052e5ca9fd8a319b3bfd380d25b77018300" },
                { "sk", "3331317794ef3f2ef7351ab6f307ec55277b3dab7de90f89084dd51fee7bacb58bd1cc0c0ca757f675505c3e18b5d28691c77211911694f77b4e15d9abbaa6b6" },
                { "sl", "621dd0be014da845fd410b2c3df5e962a17f0a4b6bf512d802f92e4545fa5c5b42f6ad4bce0f0a2196fa82c64b907faeafbad217c8f2ac3b4d6beef236846fba" },
                { "son", "f6b55678e25642a998257d5183be9c9e0e6309d17b441b53c9e4606bb955019aa7b5d5578a0101507070f2b888a5ef1a46b09124f774207482b3d4ef792413c1" },
                { "sq", "1786e92b58a456e986ed685686eb2710954a4b7bc33ec131df82a78890a9ef3984da172ed23036e96fb3284f1daed6353cdac1bbba810d6fb7c6c9cb497e8afb" },
                { "sr", "8c15bcae10f7ecee1aadca05e513b2f1173eab86b0b55034d00d75790821a3a36e46790f96f3a2b9ebf5d0b0859e73822b1bc4ee3665b9ff8bb479823ab943b4" },
                { "sv-SE", "de3c637d4bdaedbe08efe25950f0009f9a12f0cc2ba64ee147fdd5b40e0637ec37b604b9384673a2a7e8e87cc745af0856f2442d062dfbfea533dfd60d456738" },
                { "szl", "c6cfa35848add4439b00e997c84a628d1738d50992b1e5d5d6b9cf245b5e27ff108293a08e23a991d8ed75caedd5813d1d96e551003d687935ed512f69f2374b" },
                { "ta", "95e2899999d0c1d13a24f2e66f897fc8c48a1abc54abf4f8ab05e745d87fa7d4dac32537b9e5ee1b06256d2a0e81589aa2c8f9080c38f1e80ba7948fe4c13bb8" },
                { "te", "acda53e549969957b1d9d723a4a214d3076ea958e5632f943d4bcbc618070fb71a5c5e26963a24f757dffb28fcc44798f49c91dfaf76608090efc949f4741710" },
                { "th", "448aa1982efa0d99bea2f043e9c690eac7b6fa8899fff7f5888f7cfa15298f87e6811f694c9862f92daf090cd14e76901bbd573eddd6149ba9821f0ee0c1f5d7" },
                { "tl", "b77bc61e44fea90bf6ab25214b2ea82a482da38a851b6c71d5df1c365be16c4c2285f0ccfce3396d4410c949955529c0424a4cbe600505870a3846402dd0bbe5" },
                { "tr", "a15e1fa6ed2903b74bc8b2fbce6611203c30e686428a597bbaa1bb4687633343ee42f1d4c80207a4f8c4b6efa63f4b6684c68cedbd87977380804f49f3669673" },
                { "trs", "fc390e2d83b3e9301e7c05051c64ea091c1d50361c169fa87ab1d2a255d6122b39c7e36b9438251abfca3a580051424dbdfaacef4bf287616dae9f6ce9f06644" },
                { "uk", "f043b87b450de63f882253b084b22f52a787ebc5fe9fa9972dabd955a490e7d9b75a50fa83ab7ac9a3f34318a457649346b4c446ecc5e04786a82c2ef8d9e1af" },
                { "ur", "95cd41cc5487df1e5c8542843c9c043b9118d13b95cf965f578e225eeed5e15ff90b0cc2c84403581f2c5ece6b37705267131d17e4680f13c680c84876a1928c" },
                { "uz", "7e08aa9cd32e2436e0e577dc9908a25bea9b8b04e430f12c61491da4e9163f227c4e2396b44fe5b79cbd7186c193bfee886bb4faff5898da88e739ed758b5ff6" },
                { "vi", "99e31cdbecd0cb1dcff9abe625c051571acf744155980e3cf503dad74ddcfd0ea5c5f51a13a9a9560475ac0fd57051b0d0779b497e4e0b19878c320e9da65e89" },
                { "xh", "e865b2ccc6c900662bbd5bb5d3adf07b21322e5619f8087393e9c64d6d1d44d635fed12b734cf04980401a426410e6a700928384d96869694de7835ac3a3d83c" },
                { "zh-CN", "047d9bd88ddc8b22d011305fa1e7e7f1c0d99c3e1754de7ddb7b88ba81d19289887e44e3b97ab07340698e61883b57b20669760604cb7fcaed49990818f1b3c8" },
                { "zh-TW", "8e001e9970fe1404bc1bf3e4d5ce92062e3a570c08800b7035f776e8476baf2b641ede1fcc8c4f4aeb78cd542f1ad2d0741946b921c9552bbdc8c53f339b6060" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/105.0b9/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "3b0f4e19daa24fa143bc1005fef0bb96220c3e6cdbbca237453bf55359b21b793df4c979abed59049aed35a3c5d2e2698bfe3da22e63e87395a56ee5e539f796" },
                { "af", "6dea17f4ede7eb070239d3ee845da15ef7c773baf3c4838145f19df38a234548a274800718a8e1254c42bcb65a6f77117954796c434fcc6c0e816d80eff23dc6" },
                { "an", "659e03e2689027e0380fc5125a24eb18ae949ef4819a2fde464a7689ff52c205e77a0c91cebc6ac25a1a42118223ddd04a9f597d0664d22d94b4bfb0c4489911" },
                { "ar", "dfd5afb33fa64a4f78a46758434077cb9dbf1ecc1d900bdf33ad730420cf7b6fad4c895d7ff78bb10f15d1cc5be7dac27bae137861cfe03dc6a6bb9ad423262c" },
                { "ast", "94880ad2c319d762a3976745174654a1f236948278ddfdc734d759c30e1b00adc9f05e4ed1b130048773a6aa58b43d8bd0a600df1f5d7ba5744927e9cef96748" },
                { "az", "905e30c39917df4f41b471ff77a763f836aa0ba94baac8fa463584b518e152fe79426af5032c096ac532941fee60d2d3fcd1d326d6e7aa5c9c4db56965fe4767" },
                { "be", "6d79a17fcf759898a9b90abe38af2dd2eebf749773437d458591b81573f52cc5eb346a7bcd5f395c32afc6535cb002e6654561285803d5081370588c64c99c56" },
                { "bg", "6be9e001799d13d12a4b8f0d6ed2baedfb25a31f06849f7615a5e0a8978d1a05b282d67e3c2a38437d013d64b15fe783462930fdfee1df6b97ac6f11bffa048d" },
                { "bn", "97429ea5a0c90abb5440af1eccbe2f0688324dde8709b056541c243600cbf03f4df87fd4db7cbd25566c1d1e1c1f0751c08576b394bad03176a08a8fd49f9d63" },
                { "br", "9a714158f16901511ffb94054603e6cc9565a92b5c52742930aec901c847841fde150202a2981d95c760ebc4300727258e7eedc9e307f6ef6b8151e7b25018f4" },
                { "bs", "0f01f8d4359dab991564f5e709466130825121b9fa0a75443b8fe5d6574d4443fde28cddd2c77052bfd85b5204f055b56ad4d86a3f0009adeacd558d4d2d77a1" },
                { "ca", "5b4e4310f0c0461b8d7ae95f5d11e1a94aae64a2ed802a55793a937a1e29eaa58f25f6ffecda8e07e4929c6a266e80034c96ae384f3de9e16b0b02cd9d27fc12" },
                { "cak", "156bb1b17bebc4b54d387979c2e1838f642430065112f7eaa4304f7a1f80c8fd4664cb9ffe78a19cc34fd0f5a81f27569f5efb2fcf20981ca4321711fecc2ef8" },
                { "cs", "1ca7007cd9cfb5382037964a08f5f94918f15a856bc6fb00323809b68a3be5edc5a0f364a745db125ac65d4f99ab582ed6111fe35fc5324a468180d8106aafc4" },
                { "cy", "c0e346dd310f03ae790edbfa095ffc0f22ea6fac64abde92df4fbc0b3d5196492618b45218b2842f42e8da7a0407459aae65c06b3d78fea298f350ba8abd321a" },
                { "da", "355f1ab5c7958a7cc648806a4da823f5ba45d9cc48e86e2fe86d12873246c3094a786eb4a0ba37b3666aaa5dfd01c4a9de6adcb70a9f13e4ca35e04cd8137b75" },
                { "de", "9d189433143ba912e3571561b06ce6eb8bca73eb242866180c012303e7719e2456ec3cd3069dd7cd48fca9b693dd8ac9f4553f06dc9c1e3d010dc50905cb7c19" },
                { "dsb", "72a74cccc7c9ce4456f2eea8502d885050c08984cc53d27abb5c4d124b7658d8a241e6dd718226872287ce0e52eeba145c8564283df20920b78a0ac496c2b0d8" },
                { "el", "7ebb404b9d1a92456ad4ce808c8c52d40b2aa86d3e29104b35b634a9b9168d2244918825611c4a92f677e057026bb4409a5b34197127fc4b9a23c1db023a0f9e" },
                { "en-CA", "e3a8fafae56d15e98d50a4b2460ff2fa4d7c5195e737da7358ea6e393e9aaf26fb7794e39295b3393662426bf1124b70322f7b9f87ac15c350ed3b67d40544c6" },
                { "en-GB", "218f9c68dc208c4749d40d4af278dd42bb5e9ceac42ce8e4fc6b9e31aaad5b4cef1924a709658fd753b1fb4b44f3a1c71a02cdc1048c7f30e6da374a0b6bea6b" },
                { "en-US", "19eb8e6a38277a368f89a5b05428c74c4fddcc5b42a8d4f97e2e193ac8437479c7436593400d721241241612298e17de076add7e90685c7974707e356b111ef0" },
                { "eo", "ba17ca1a5bb2daae290c534179f570ecb3f1b2a02bb2bb990a249642b13d0e99d4b0d50450b8166316329b205f1bf03492046c91b56934a9759365213873893d" },
                { "es-AR", "e984a72d47fef0ec556324b8d3361c93c9dcedc51ccee52982aa1ead4be02d094e198dae6fb5e2e69e699cefe5353198d9b416fa2ad0d20d6cd6100abe98c20e" },
                { "es-CL", "7ee31f21275ea00f33c8a26c05536e099f86ce392e8cd5bf5fcaea4abce4fe7b8652d3c3be125bd0740448223f87046f6347d05417d12f5a3491c0db758c1277" },
                { "es-ES", "465444e3d3a00f11f89935cd69e5920930e182e102ea0367ce56d57a18d6032d45e48aaa23b1735c7d3b86a78b21d95e1011a45686fbbccd419ed41b3792a796" },
                { "es-MX", "badffeeaaf846932494cf9a51209a7ac204aacd43ba1eac6b71c4b66e7625bfafb4239979033971f8018506422742b5e2626b076266d91978fd70eb631fd6d91" },
                { "et", "428e1ef1668a605a4d6f34ce00c9518158b439caee67f6492e535ad4a269c52e85c6684bfcef1ed93ca35a45f6f0f094881d0d6a87842b35a1bcd39d2ba8e936" },
                { "eu", "1242cc5f0c3ce151c96c0bd1019095e8bd71ad6cb7e81de39215516f85d7279bd713acc5be4186bcd473226cf4d033e98cbe769e09c7defff84263d14a08ba1a" },
                { "fa", "20264d1462470af4f288851e27e7f2609955733422b92175f69dd0cc98914a299f1be9c331ccc2e066e5c39b5573f80b3543cdf620c51b87a49f7dc48a06520c" },
                { "ff", "41f738f706cbc94ba2702a75bb8d327fd66b6cc563eb42b5a4a4ad87de85755b246e662d41c0d9311f296bdfb25ee0a1dec62de9c2dfbdd80351970838e3ff76" },
                { "fi", "4abd9f48d550544a088e160cb5b58496fe73e745ccaa12967594a8bf0954d603acfea680d59e91becec6053a55da066ddcd933e7584a40a42b85d2bb36e49b52" },
                { "fr", "72ffc813a90af0c5ae39bd2fb68e26cb14f92c1216349af1dd13249ead8c4bfc7afd8e5a94dbc2d8df07ac010506091ced6883fcc932c2b1ef762343a4e36f90" },
                { "fy-NL", "d3e0577a4660c6b36a02449062f661b439d4423445fa0847d2bfc02ae649b47837fc54183796729268b772db80f62fe192c50e337b34d0a03dbb3e5cb03d3703" },
                { "ga-IE", "9d275d380a12e9140d430a15385747b40307ebd869f8594a8da602b9d25af8736e93f055a76714f94a2a3f932a8f2fa6b7410379d238f6a6975eca4beba1417d" },
                { "gd", "c539f6778b7d5de1d306c8a248f965157281879249bd793c92f0a987f33f4e537d9de1bb94ac263307b04e7ae6f291b53d8f29013c30bcf7616ea884ab493262" },
                { "gl", "22482c4b81fec9ca8930ff21586b120e1ad9bfb5a6978203b697214c372d6a7354a70d0fdfa2f901f297553b29751692ba5ef23e4c679c8c2d75d29e687c9a89" },
                { "gn", "6f4d66d204ff724eeb300f2020c0e81741d9ef0c45e3d7eeba1bb1882d92fc59bfd1545fe7ec51553c95290f6264860cd6a34f293867ad10de12d903545bd701" },
                { "gu-IN", "f0276093f28c65add2c368fe02e28b88be393348b123fec48371b9e756c64c216eef7ede981c303857417a545d95939ab3b4c27a3f0ac32d75ec73eafef4aef3" },
                { "he", "d1fc50774f2233c11dc7f4c0ce9351e3acb661f829d69fa223a6669bc49e15f3b8e9488adf3c259b0f2ddaa3c8f60fbbbaafe47530d50253fc8216008532fc9a" },
                { "hi-IN", "838c32ebffd7a9f5a56d4bd234a9fa922d79f6c9b7354ee7e4decf122846b3175da2ed74117367f0d5c1a24e99f3e48d2e5a0059a7208fd7c40b12572063fff8" },
                { "hr", "b7f6e50dc662e20098677a20a21d28bc9fea293645ad3b0252d1ba235c1568a64c0760d86a29883bb4d3699ad8f47d6edc6ee39d44d81339f0ee18bc833d5338" },
                { "hsb", "5240d233695e3536e997cf26c78f03f56affb66556ed4701b29ec51b70e187033efb0521c1f53c00faecf6dd74d68fded7b10b62d5a62ae8bdf957c5919fefc9" },
                { "hu", "1ca78e372e86216e731489bceba5f410f1ddb01b5d1ee3b545460eb9c43078e222087cf3d33f38dbaa2f70b0d4a4ea656a8256f55617ea089af1bf20060dd1b0" },
                { "hy-AM", "1bc884cb789d26153d73845a29c7cb700e5b2959344b30e2f0e5fab03924267d83ee4483c556e1792a38972cb9a14b888140061725f93b637767276d265d2533" },
                { "ia", "a317af39d3c868f9c36c97570eee308ef84e04043e2e3668ff1dafee6dc0e42861f86811c1c1e1d3b5aa9ad0489e81584981551cfb5e00a205aaa83798d48f40" },
                { "id", "4f987e13cc263c1bc0ec9d3a1faf40c74f1bf10e69ffed5cc16af6a2ecd6d6d372ac396af0e45d1ef80b3186535a0a9f32f133bf3a38a9b232c8a76d3e482726" },
                { "is", "65856dab0efb7b365cf298109d3a9431303277bff2b0db59dabc7df007c0caa8829f1aea6ed6dcd842eac75fd431e625054b45cd9fd10e7a9af1ca72480a9c9c" },
                { "it", "43dd7cb36d9130cfadb077403b0fe44782018eea3804f5fbe633e5339d3071019c2af0d055cb4d075f793020795277f8b5d61341a6af88785658f3617e279d8f" },
                { "ja", "4a58ab9813f8ae5189011bddc7dd74a5ceca79440b4b2f28c4fc96a09fd32b9ad2ce9883df97e36bbe7da784a03dbe62e093f510184b66d9e751000bf4379172" },
                { "ka", "1a217a0a52a5c14b6aefa26f06a4b322b8b2bc14c54f3e31f0cb57dce55d47ce2059c6bb69e3965ed81b7761bf2c78edaeb8a6552450acb659137c3c20fd5a87" },
                { "kab", "dae8240136b01077adf6493f7384e900c479f9cac5e5b7d4b403449e058cedbcf53a99df78c883ee47af9658b8ba557af008c488bf697253c4269601c40fd994" },
                { "kk", "77b0e31d0065de2ca6e382a13f1c09c1dd10d7eab65347c6142d5e0b2186911992b3d45753691a1ce4424657f84179f5ee359f648ccd73f219fc596fac185b44" },
                { "km", "ab7b914f5457ecf38f47d982ef615414bea4895df689d2a8c8ce2bfbceed78c0be284981f5d86d5369f747ef59a87d8c2904ce35c72d5b9e375a39514bb82505" },
                { "kn", "f0f6d5aef2d6f6ac33eb771a0f434268d9db85a6fa272c10113521a22875cb37ba8d1c6b9d7c9c6392f61356834db10e558a5aab1c33cc9cf84bf7a097a2666b" },
                { "ko", "70bb2dd32ec712da82dfc56ac861af87eebe589c9296120a9404bc50171d924a57a78b22fc9b2edb7ebc19949165596af349fee07093001e060f29495ce5c91c" },
                { "lij", "d1d36a357b5455a9fc5d1bdc34fa967fec8fedd30b9f3b3ecb13141ef36c6d2aadb75ae3f04c584b2058506733cd803a7dd19708a64c4b94d8994e0e300cd5b9" },
                { "lt", "b77eadbe5d3954c0f4f62c9111d7768bcc90c41d0bff4e76c2366ceb58b12efbc7f2fceef5db3c5127b14108658e12ca1f118a83447cd5bef748ab94f9124d07" },
                { "lv", "b602f0a0e9bdfaf49c72f2be4c8b2b0cb9af6404657ae607f0e6cfbb7e46ea53e90776f571332254b2772fea2ade7002f1f63249986c5c77fe947ecc7f80a9e4" },
                { "mk", "2fe115a7c43c692365d5d288a58a3137975d94df69313f31751a9219b5a966e4007ecd3f84a4545409878a9523c49edb0735215e6370654063900d2b79ebeeab" },
                { "mr", "34ccc901ca4cac84ac67f27b87a3ab821f505d0eab5aad4df2708748f50a01a7bcdb08f85661f3a9a92134c8916b29e4a06b8d52e7e4f4ac22fb85eba4e72a74" },
                { "ms", "f817b1cedfa9d37a4ced99e28a7b2fe47a890ebe80adbd60d859bfae38814a1c90556574921fd40541d0d58d2bca24f559011491df0347b24c97c4b6b26ee7f0" },
                { "my", "6004db6015bff0eeb04dc94ac59115449bc5a44c31b8432ad955fd88a25bade4e2eb7546972b9bb577cf2af8aae7f4a32a66c62e3a067913a794ec878ee14310" },
                { "nb-NO", "00e1f86f637ec4961e5647eb819b5c0aedde1c0e3f4f67d255c724c8a18f51559d810d50602849e19dda87711c737e3554ab62fb3c48729bd6b3ae2d281bac11" },
                { "ne-NP", "4c9b0ad351ec991971a203431a2340543dd2052f714387d8811b34179f91edf75143c03fcc87c13eabd5e685984abfaa9f901cbc4ea0a7ca665d218d77a6ffd9" },
                { "nl", "ac70adfa7b42190f7df78ceafb259ec27e6ce0870ebef9619e9014c2a099ced32cd58245ca2166bffecd45c4736605f02f3ec58ab8424d11440c75648eb66352" },
                { "nn-NO", "ba6b3384d85853d0c57cb46b2c8d1247451817ccf32a2cb7adb3beb7932353046d5fc9a69744881ab74a9a3dd73a3e8922ae3883ad01df8836c8630657fa6fbb" },
                { "oc", "5873c141441cfcc7ecea6a0d6eac9d2f2baf074a887421c4d94b4207637c70d7a66dcf9b13b69a4d57ef5b6a8d994d7d6b84af1f19ed5f5058bc3ae615117030" },
                { "pa-IN", "d0e8c268a03fef2c56804c33eef8999d86b7ec27a40b0349959dbf6e8efc2f8e7ebbb273746aec163e64ef7e9585e4f0db2f91e8ca8373bdb70b2fe25e79c487" },
                { "pl", "7752f0bc0dcc58b65dfdf76dab8e1da937d285df8f7e643220162218d485d31c87ec4e972bd9fb4fac327961d889582ccd2a942a9ef09066b9258356aeadeb84" },
                { "pt-BR", "74170df667bec1aff3394d1e69edda0c6a1e1c13a3d2e2fd3e18a2d6b5b425fd39d5b15cbc522192669de0bbf1dc95db2e8608624bbe0f67bfba4a0578a3d267" },
                { "pt-PT", "f81ae886c9d8968e1b45abee17725df92a0caed4cde37aaf7d696a088f07abf6796f609d7b62becfe939f1e273d0e95bffb002c1959ff8d9108adc222d94f2e0" },
                { "rm", "efde91d713b4e74984b0b64babe6aafc160fb2c3c37f19478721851ca5ab1a8211608ae990bdf5c0aeba95c8945e0eb18bd3c25dfebe423488902407ec1e6fd3" },
                { "ro", "3ae17bad726413aae11ccafcb54b007eb72fd5732abc8b110e41a90c899a8046aced75526ff8fcc420e9ff137aaae82412646954628e9b49f20b28516a478545" },
                { "ru", "0cbf89f05f0e15e0e6d7b8d8f876bc7d73fd5d800db0fbef2f5fb35f955b31ac515f80bf13f38b515ab0410992ad53d503a5ee70c00550bf8ee0c83977aad2d3" },
                { "sco", "9910b0a5b08fc677d268f5c86b1e76d31335c201e03bb3adb13d5b95985677caba1e6cfe5e9aa9ee185d413ca8043e9961ac881e896a80e81d5c4ef52ef69cac" },
                { "si", "395bf2e9a7f3a535d3bdf5a67807029582a1e3f5355e0e91da65cc5069650388fd58a0344d093b37a6c182896410b5c3f158bb1f6cc9d94c643dab983cb61a19" },
                { "sk", "46e58522074fb865de46c7d0a12c366025b41500bb43d2ddbdf6e556db7b8ec0fb301de3065363212a365f888927b484f06aec2e90e6b31a6ecda37b1df82e53" },
                { "sl", "0aab2c442f91c57b147dd56d15ff01a373a155afc5e0a31e0cf4cbe6c4083edc8bd37535e6c8d0bfc0457e23461221b9908e574f78ae0e92e182a8b8d0be9a15" },
                { "son", "371d20e6d9faf37a906d0fd768434c8beefa76ab698d3638dfea649da15df4e2ef59cf4ba10f5b77df7c4be6a4af49c1be701d30f538971295637224968e447a" },
                { "sq", "bd79b80835d458c5f1be8f9cd92feee3a483663c78beb321fac58eeb46a2eef305587a64b7b911cd8f48121ea42a1677d818e3f35c6b763338b81a032d1e0f34" },
                { "sr", "30ccec20a097cfa78b38d1a6e3e798d8f6f8de1c4f38e64fe189ec569097a82a5ca20f38cce84743a6ffeae7e413ab74144df22d052c27407fd7ff0d5a7f951b" },
                { "sv-SE", "823b4288d4e20dd0b53801b3fa2d5f20f9b99c7bd4b5287ac42d03c1a408fb4310087c3aea17ff862623aee5d1c44567b7acda8a0e09169794d996b5e040f914" },
                { "szl", "ae172ef540b379a496fc90654613ad0f15d4f42983a5477f3c1e07097782d1bbda2fce388d49e0bfec3e62b1645bbe62c25c2b11d0252013d0fa75249e7295d3" },
                { "ta", "745f7cfc3562d20504799dc85cd1d7677296f3e3da4d342e3b52620f970da88558dbde6a89a558267f132f599f94ce5c0576e35c619417faa36d5e04371bfeb0" },
                { "te", "de1f1056de8e33488db93499a78c78c3a6142d57cd0685715b8db2d71672002093d65f578e8ee14890fb51873a21131451ec799c6024c03630b5491744ea7c4a" },
                { "th", "be404e5fdbbc123912a2a40b8ac05d32a977bbfdbe496db9a85863af23a1c1c2918f4c62cf4f35319faac5148b5f327d31bc7b29b4d12cd63dfa5c00f589f160" },
                { "tl", "07076ea61eec756a141e6ab8357b16933341c28ca54f210c4e34e3a6d02e5d7d127a7effa02518996cda9e41c09f9b57bfece2db57c0d6a21cd77831344911bd" },
                { "tr", "c4e5dc3bc8e972e1bc801e352faddc3bed1c54edb0ab1293d1178641a01da655414a4614f749e22619f96c451c73474307f973b96cd2008a1a98df15a51fe53d" },
                { "trs", "41e03d39f828656335b57d59d97809f88ff11f058bd25d1c044f8b4f75e173922aea735e7af49b46b671ed6cdc5754ebe7f663933ad1c895ca13f39d8b7450db" },
                { "uk", "b1a4b9640387b52ecc62bb8a4bf94fc91ea067f8417d573cf15d4616bfc1768522a5f8a7fc144a67f2f617f41cba5a93b2183a356cffc935fa9b318609399a63" },
                { "ur", "20fa49637d702f258c8b480f5517c37c4e5607d3a70c41d87bda9d74ce8be028d326da1fd951f76da053059949ba848bd92f611bb4eacddf299850c994a8bb80" },
                { "uz", "2858a48d11d066fd172cb4dd4105748d09150fe73a19af08346889e388d8588a89dc3d74b9b9a84704b670f805ad1f468146c2699437fc6f6e80a09dc6a1957b" },
                { "vi", "72f2ab3bc61df33a8267d84f40dde86ce8bef46249ec098c1d085e919fbbe8c8a6837260d4155ea632bd40097d91740c45aba6e542c44fda26d2951512de24b5" },
                { "xh", "226881d2e268ee4cb543bf672a8a0d4f0657ce1d2efaa8b84f4709588f5a6a23cc958d611656dba3bc3fffc4b51685e411aa5090d9b13e5e3a2cd10e6fbdc6f0" },
                { "zh-CN", "a7234c03b73b7db92598631d0ad559e72a3b098259bd5623e33b75e734dd03401826e61d5e5dc3079e2f2fdb1c701cf471305fdea1dfc9a54dd76bad5aa435f9" },
                { "zh-TW", "853c629e3fdd13bd52c649ded21c7804d41dc1093edae358f75fb42153565e8402ae69147813e2aac8fe7be3bef908437d36d1c94273e965017342af7fd2800a" }
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

            string htmlContent = null;
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
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
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
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
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
