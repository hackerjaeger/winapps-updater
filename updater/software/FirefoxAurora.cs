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
        private const string currentVersion = "106.0b6";

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
            // https://ftp.mozilla.org/pub/devedition/releases/106.0b6/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "567de2c4a8211a600d39cf5cc6e0bf64302d64614844b5f2100aae14a98993ae648aaf7a6a4c334ef4600d58165e19604d80cea96a500ab3ee25ea1678f7a9ad" },
                { "af", "df1938f0d519109b8de8110e4e419e8384ace81e53071c482872c123894414f48d832c575b4e06ed153822dd65f6710828124804dfd24b80a4ebb7ebf58f9191" },
                { "an", "09563c42050c162070cd80709f2aa16c72c0b37c3b750c75dcbcd4e239c9dfade2f8cc0662b55100e0a786041f319ba379eb1aa48dd61788ed0b177dd597f469" },
                { "ar", "6e767a05db71e8921440dc9c5d202b335184b7c0f79723a29254b62bd11b0103a8d9c45dc3588e0287dc943d05d9d42c00e9856320ab5036bfeeda7c04b9a0c7" },
                { "ast", "1c015f08e6291439389a4413b5d66e7b64bec29b159f44c0b1d554dc9cf2c1bd6a109e96f239b56431e30883131426ec7474fe488be03d66ddad0c5376e27c06" },
                { "az", "a0b3c0451c26c19d1e12631bc427051dbf27eb1de1fa51827175fd05855b52eb2177ca50237d2892e176d85232052b13f674148bba1da442f1adde1aad9f9dc5" },
                { "be", "db8f014a4a361a0c8d6b7c140bf5fbdc590f0068c8b6a87a680449bafcd643e85966b292612a5c06033a41aee373444f486a1a8f58f455f09176423623c390e3" },
                { "bg", "7cf6ad4aafcdc45c0ddf5792aa301982efc41b5f7a50fb9a668a70eadced58990113e50252266dcd79a28f1155ab832a62e31e21011f0a8d97d40cf25305d829" },
                { "bn", "b89fbbf24bb81bb7634f0c18e0f2d27a96a92aed9534821eea5ef163f43d2297dd9a00190ba56e077a500ff65359098dba06aaafe420554ab51c9f2899b0bc8f" },
                { "br", "c9222bd428f550685fc47edb4767f03ccb8aacde8857d0e80b71871fce4d960d8a3439ba1157f81a1e9b4070f6397529f4704ad8b602639423c7a1496ca89b88" },
                { "bs", "62c7ac4d7d8df467b416d48af5d889bf394c58fedce1e23faa39a1e71d7df62587120c8033d051826ade37cb25bde28b2e5a548233022d308a1b7d99d8afc63f" },
                { "ca", "a455934829d09d081f98877ec3b14c376a81623490449b24312a569ccfc145448c1540c99b1576e552a979c51cd50ed0b223a78cfa7e4df3406893e9431c109c" },
                { "cak", "9612202360f84e93bafda564b6ac2cd15883ddd35c5cbc72938d0f2591c696fa52df039ceabdb2c3b5b886f35a5140f2761f8b51f73f25dcb6bb8bb1392fee2e" },
                { "cs", "437cee6f4f96e83776de33a8d032f15b0e9a7faeeb6ac2662f4dedeb0873ff2df774149995a5647fa894f95511c796a2a48239463b334e93d3a4ba7b65677206" },
                { "cy", "8a9f580214019e2ae8c5c75f76682a3fbf444d644272fd600f8a74abc6a60a0669fca829b72929e9abf2ac68802b78d983099168a9de2edc479b7ea1ae57c1db" },
                { "da", "f3eda6647830a1cb108087cba74f99be3dadc94f016ead32a7a606686cd771ce61b6662cb9b5218791cae5dc9dece69ab616840be64de78cc426df13538228db" },
                { "de", "4386776d59b6c6ba341f576bef84e134d4de9af653baf09646b34a3bea55ae899d91c77b4bc23fd6418b3facc8f9779edd32397aae3cfecc52259220fdba32ea" },
                { "dsb", "75d6da7cb902a1b22d03828bb8bc8658185b175d76c888c6beed9826edf6b0404f3d76011e85ee1b9c0b23564540b35309bbf5cbe1e1ee73dbf421a330a7a962" },
                { "el", "a0a14edeedec1e494791a8c4f56a2284ee3abaaa5fd9b6430d3991566c2c5a89f256697bd6c0a8d43cbee4520c8588d3b2f6187893681f72d501eb8b7349aee9" },
                { "en-CA", "8a56c1debe86367d5577a182e7d6d2af46e457659b9469b7bd8f90381b8bdab72e049df3fc90e6c303bb7f153d125009308f1df15d3b9d265c5afe0b06d23f73" },
                { "en-GB", "8a05cfc19da96806623d6f92eaef4c0ffb0795b68b586e6ad893d31519ffb3b86490bab22733c7e92d213520975a6a81974ca5abcc92608d1ca13ee6eb5209ed" },
                { "en-US", "1ab4baf88ce2f871fe727418ea4d2cb0edd0dccb3919981186fa49623b70e043d393bb2e2aef73a961876377eddc99f38f2486b5c1066ea800aca1fa2d55a340" },
                { "eo", "01a374716426f0bbbd243a6e5835d3ec6684ccb2fee507d70d0415d4e341b2244cdd3b8a96982e8f44a2d103e0b0db53c8264b282b310d8ea79ee4d3ca602068" },
                { "es-AR", "90155e23fd3184e07466501b70d2ae269f81777bb594b178d60549d2559ee116f4c13d813bf564a980137ef9d030876f9627411c77d6b8b7999c1ed6915e4b06" },
                { "es-CL", "6fb3637b2b61976e16b98990eb3f5da76e56ff5ced70eca2916ffcf76f86cdf6817cbc65fcb644cfd4d5abcad2baa621d8bf10fa51167c813ea6a970a78431f0" },
                { "es-ES", "6413d170f7e8c03c3b10699225b3b93df143f44233b836faf9385d3bf4377c2dc540da07f92f7f99a24f776472a99efd562542592c10715b2b202a83b3d18fbe" },
                { "es-MX", "521e0d856df5efd86a2e7da9fe37032acb7c5210db08a12eb2722222bf248d6b521e98a04e047921c9a4f8080581566b7499093e9247f064371e847d53f482d4" },
                { "et", "43a465aee83ba9e1a1467df6c03978e79252d8ef11e9b57ff043e517a15022ab70aa40711977d3e9d3fb847437563fe3ff07bf180a4e96fcd9dc4f6595069534" },
                { "eu", "20da57511edbbffdbecca8baf47bbf586cebbc6331a726bc95f33476ac219026d9365fd59ef55f0127872b4704c6aa66671ae05f388d215af8aefcb74e95976e" },
                { "fa", "a7d6129969495812dfdfe20808abc9c418a564b7bdf7b4dc8673856d8c0b41166cc2fe4f12b90e4c7f5f49c0be728d293cc766d9888c4fbf3d669395cd2d4dde" },
                { "ff", "7d28dba2820e959becaf5bf68b4ac8192dfa45ec50df09c12c342d42a5928c5270e6d4cb0c6c7766735bc703a4d6911396bff2e9a2ea96f915b3b2d8ea944b94" },
                { "fi", "b33371b45b39b4538b91aea94073f776ae6222da2e160d6b12142cef50e0f9462a07dc5d4a341526af8e12d799406fc010bc0047dff166c609f70c43fe1ae80b" },
                { "fr", "4bcffea8bec4ae7b3d73a579db57bc3ad45b84d64d21977568c169e28954fdd9a144aa9d6484c2b546767e3dfd9cc42da538b355ccfc9322f001d758673f47f7" },
                { "fy-NL", "4e06c7daa75b6b21ff0580e9bfff2f8b6fb409eb0bb6133dcdc26934becfdeed72d427be83e01192d82089395a2e98d9ffe1a57c66b1562acc04cfa3010ed412" },
                { "ga-IE", "ce539797bfec30b8a9d597fc61cca7c5df6bb8482880da18b4e28d2484b6ec37f8ccfb24d0fd6f3638fd6bd863f50bce8bd943e4a376d57bad7f96b4dbc09318" },
                { "gd", "4b62a8740049c459fd12c83b02bffaadc85c26d930a34654b6a4821996ad59d2ee5e85d822e83d1e4f0fbb703661baf1e62bc410ea863ffa551476867216e785" },
                { "gl", "fa65a4a4909a14a1b7d1be1b60f41e4d5131606da53ac64dd6366e14bc13584af3305b244c24ac5aca64435b9c43721127436ff27b4546105136a75e7883dbcd" },
                { "gn", "4b0c126e9276e02b9bb183bc5164b70bb6d9d5efe9109a50e698bd656960349f26b2c4154faa237f12582e0460b3a77ea2331a791a0e75e13fdddda76251b78f" },
                { "gu-IN", "27003d83e69cc921c9f3d3d614eda2cb1cde114f44e0372fcebade7aa2643fff0d512eaf3e6741b303ec0efd011e3366e9224bc8dc058dd169300c31a5608591" },
                { "he", "f9992d1222583ad4aac52708c218187c83fec833f81f8f7e8ae5aa9c2feb5199275947816dc796f0ac38c3a276941d1f54abe15fc19f625fe70140c45633da7c" },
                { "hi-IN", "32aeb09d2e20a80593b6b8ed025dd9c6f15a3e561a38ac06be4db154da8d39d42868749ed38bfcf008eba8d3022c14218aa21df1119c1c900d16e9fee0703954" },
                { "hr", "bcfc89c23ccf7c4cc4d9a3b9082853f0c96ae5a695dfb451f6b04ba3502bf3dc16eb68ba978d4481321c759591b008c8bb5db8d1903b6c272e2868aa015cea74" },
                { "hsb", "5174c390d1aec820929406b0137b661f2842b725fc5eedb0c9845669eec157860c14debca024d9eb8abb7f7a9c52cb209fd920f1c4dd54fc1b1d82dc6ebb2f6a" },
                { "hu", "1575db3ada1b28d02ac10793fefd0d407bacf2d15876053902a3bee66e8fa9ba9bc9e08f14da210929952f1fd3603178c2e2470736fd5bfdd29c7c26531655c2" },
                { "hy-AM", "cfe34c691008e9554195484ca86bd76362233f1786c80df92b238ca8c8ff8e50cf0fefeaba9cc915321a27bc627cb7085fc7eed100d814c34f0080eaaad170e1" },
                { "ia", "3f864b175070840f0921dd75f00daf15c6a4376bebe855fda13599203da2a83ddb7a06483a839880aff6a4b6502ef9de3c8345a6d773ce67a5f6bbeea062bc14" },
                { "id", "6bcf87a892636adb6b7b618d0050928d15d30060252a374c4095b3469c411019d30f69fb07f4f551f5bfef84d5df63b9285df1262943cbeb4e76b7cede4c6fbc" },
                { "is", "48cc5a2840e866d9c3c539a0f337267a366d3bdd846b9e917a6bdd2791e4fb73b38b2725a4709908635173cb4dd35b8483fcb9d842e70fef027210176c526582" },
                { "it", "9dbfde43fbbbd70cde916a41054d530073789a90e5d26e8838518e78110160a667f8eaa01138b21920cabb9717fa7b961945b033fd7ed319ce91ef929b332963" },
                { "ja", "6cdb69a9a085df6606c57273eaccf4155387f3eae3895152142ce6843b4821c3a5f41b813503746f9e2bbb7ea2e8030708d5bfc3b32f2fa9bde2444a9b958e89" },
                { "ka", "6e4d07e2b1873dc2ffa5f5b5711485d7a872121981e777c930e57f926c281a51cbad0096cbc86595b5bf61c3c6c6ae21b78765704410f05cc7898c41deaa9385" },
                { "kab", "ba78c12fd621d9a648ab3c290ea1aedf27edc0b5465f686e663eea311f5484c98a5eb6159ab5030ad2e886467f054d0350b23edf357e6a18d7a35037a959c9ac" },
                { "kk", "e9eb6afe7469e95087aca668ae08b8ced679940a427ca3ba8084a47d23dc5ae7a147652f309db09bb7ef48d4407cb1b896389409bd650580daac8e32ecd2ac68" },
                { "km", "23e49643bc0a5c2d5d2a0a5a536ad06b42e0715cecf8db15ceafa01df99429f330a82bddc0f9412d2ae2fd592adbc66511b31bf861bac5d1c96986c8c28034ad" },
                { "kn", "694e58d319f3219ff35166dff0f5ae1d4e23faa5aeea0d6537c2153ed693c856fabb0a464a038eb7ae88ced98b0ea27895fd39976255b163c4f7409c9e128886" },
                { "ko", "5333a52898b339d634854087178f1fa7a8900fe06c40fb360974894a2149d6e2ce94e5b8d7853043b3f89b21b4320f75ef890786f8ad2f1ff0b0b2ae8b0cc5a9" },
                { "lij", "e3c1a676763512aab2ae702869352488c9d9d1ff068a5f441826a5836aadcf756e7f48f49629cfed48f329e53b85e5a81aef05a9e1f5231c74a5d077cc84db96" },
                { "lt", "28bcf5b0ee5152e97732b4f71c41c8845d96776a6ef15a8b3418f6e99d2ae55645383cf66ac69b35c86b688eee4beeedaefd0be3fbe5b027d50e8f02ddb0b866" },
                { "lv", "2b956c03e220288cb25f16cb23553d78732f3603f44779b73dce9a35f829fcb77477f80a4121187d1ea1e22ca904877f89bda95365b0d910af715c1ac3c1c801" },
                { "mk", "c4af9c1f6e5ed2b494799a2735acb453a571b60440f065ad7469be99a92ced0e293858e06f5f31a237e0fa583975ce1d39d3059d692e40b9cfc1620a682a3e4e" },
                { "mr", "c5c742c35d17bc190da9aa56469346ed40e9016f8be62e505430e7a768a40926ed2b67542355cba8160ad7358145b6995f8cc1beb27e6e84ff9b9d86f3b57272" },
                { "ms", "94ddc941c4f3e9aa00d128d57e8526c87bf59374c734aa1f5f635e28cfddb07654e22b229c522ef7413fed745716408c0f083fea4c0f8403e17efa82b6d3d022" },
                { "my", "862aceb76213faccc0696cc3c60493b4cade84605201fd1ed061a6d7b7190fc10d1ebd78c1e553e977248af687171e6c21f9f2660f5a8c32f03bd92a0a6c1f20" },
                { "nb-NO", "cb074a2ecdf5dd60c3cfab73f2ad6fc6248345b72d577914d56f10b5b4027d1e7876bead7da3bcf98b6022c65ab882d65485b8dbbc32256b2cf824c216465124" },
                { "ne-NP", "0dc6327d703d24dead6adf3b9a4467a69f1b4661bf8497c603e04728f3f0b89631f2edb9d5aac016edb7e80c99b5a384306aeea1c147606533973cf7ffbe4072" },
                { "nl", "2ae8256f06661da22436c6527cb1dec1a705f3adda7b9db293bd28e2161bbf487e80d9c13b12e2421078eaba3fade11f69dd74893558badbec47309c4bf1df8a" },
                { "nn-NO", "e0ab94ac4b0e1af62e2ca020a53fc32e0160d787b3c7ae5008d68529774e4e1636be71beec865f36107a9ea2177c2cb2257a8a828f27805eb1b4689d70da0e7c" },
                { "oc", "9b04fc25af91ed040e277b20781f8a7c3eae117e913ef34be42fbf388b791cde6882b5523b49815e8f0a10b4b32c9ded4cf85688f890fd7edac6772f9547d1c4" },
                { "pa-IN", "f7554617783ac964f875634f0bddd23159514b76989174de0d70fa7ca45d3e7a1658ddca7ab5ccfec4c302d1c35d5999aab8b34a9bf4e7eba720292252bf1f77" },
                { "pl", "9494b8f0e6201d51044a11ece6775ad5e7df29a2e078a65bffce52525fe6aa8a74c81ac3fcb7f5426bad1f430c3ab94112897edd22390cdff05e8f12525878c6" },
                { "pt-BR", "02f41187bb4f571073fd7b09f15ac8a829e81cf71876a20dbe9151896ed34569c268a9023a99f1bbe571e9aa10063507cf3d3288ebab2c6eec68bec7a94bc7f5" },
                { "pt-PT", "c6c3da1bf9547abd1080d8b520aa60190757ad7252d55efba13f3d808370d0cf313f4fe844c80884e6167280db26992d247a4be346e9ccbede58c5cbd8e46c8f" },
                { "rm", "1947ca5b5ea2dd7a6fbd8253575dff6902969058be496d525aa063d38915e43a82a030080dc8c243bb624fc6426c53c4582ee5f41ed0cf4f5dc88ce3f5504f4d" },
                { "ro", "d98385cd7cf78a211d31b896feede8052215027e1f15c8210e76a9d8c7ff58dba2639cb1d0e3ad035f585b98b158906f717969884a97fe0d724d30fd7d63cf7d" },
                { "ru", "f05dbdf6dcf36cca0c924da21aba573495509125ce0ba51aa074949cc46e442cc931b17c00f0426fbeabdaa60ea81cdacfe32e44d4ba2c4435faa3298bab745c" },
                { "sco", "7b8dafedc001b4751416504910547ed063d458240e94ae96f4754caf066061fe5906b67b02f45ecdf5643cbeeddfb664b61705cba57d7e7b2278a231f304b47c" },
                { "si", "19c3cb1de453f88c34e500f786a59fc74cc846ec02e5e2b4e4db09bce826ce59c5a4723a4ce3bb6a5ec1cef06d5fcf8c299f3dc67adbf0ae5addd0daca8e49f7" },
                { "sk", "1cca356fd09771b291119a0d195ee7685d8d57d9284352d6cd68a500640cfc59a091174c1f34d8d4af883bd27cad0774a13f7b17630e55659f0a3987566eb33d" },
                { "sl", "17cb020c54d1149687dbd2c34460f93276d17076b4208c967b00fb6d826822e1fefe2049f22b04882860f489871468c68191da79e118026ccd5f47fa2b88cce3" },
                { "son", "afb2c595cfa981383badda3c13aac2ad1d1b17dc9a71fbf1e3e2c9e5d89ca9911463d2e9998d78b2845ee30438cddbab21986c3d612d67f6c0631fd45ec7a792" },
                { "sq", "431dd2c4f178bdf2a3d573a87a1a5ccd020524c58a6bcababef1df66b3be6a9cbfc0c890ad6a5a0c7453995566ec8c414273ea3c93725d5b8dd28fec17f995b5" },
                { "sr", "02d1a817009dc1892eae83fcd79e51f5bc5e06132eb4c56256201730c15c733b493868fac98177cfe31183702d53399629215897ad14dd9ac6a8cea568c8e59f" },
                { "sv-SE", "fa8494b9190cc7b5fa98f8faa84ff38715c4d0cab89a022f38080c805f47be2d80efbf82cba0a487ad846225417be2e9db45d3c3aea5f5c60b805acceea2af70" },
                { "szl", "41799fc8b10e16815b49ce00f4b74dc3507867ba29e46e892aaa5648675bad6ea284ab1be0b2b641153943b6c40d0d9afd372bef11ac6e086b5240197a7bad41" },
                { "ta", "58eb96d3f0a03418befd594f673b928a36cfef2b16e5db6e6d10aac8ba79388fe1e2e9a9678c8285a17c6fe38b19bd5226d357ef7dedcb3fdb79d6f5f3e02a03" },
                { "te", "867372644f96538bf3abffa41a700f592a7c9d43e9e17a9f87e2d26e13be06d10b39ce3d05a02f401372390b18a88e2e65ab045e5a8504e1582ba6d35a7e8317" },
                { "th", "be985c5296d6f6c974ced315c4097c09b1a24e653250fb45ce702ab14c18854ffdce9de4ac0acf114f2486adf632a0315db479b9a663e102d0505c8fd96ad90e" },
                { "tl", "198330d04e33c6e9ec41a860f9a3f3328144b80593d1f79366e44282f5d1b2e718771548bea409ef68836002328b1945c95fb4c84e8053edf4f89d7c469b8cc2" },
                { "tr", "f77132b8630f94d39796ae935ecfb950a641d5896bcbe7b0d82d1508c0de34781bb435240260a82ca632e051c4ba423e833b44e54cb6a0bc671bdc393e08caab" },
                { "trs", "bc6bcd790d457cd2cdee5eac5504cf2deea288fccc15d8bc466a287dbdb797f2bc9e3ca731862d40c38a5d2cd58a4cad05aa038dd8dc05631957f57598ed06b0" },
                { "uk", "ca3aedbc4485713c69438f59d70d591ea24c212df004c0de879f4545e2b579250d03ffde5a0815538c40e902d39958b117326383e3f8e267c0920cff1f4358ea" },
                { "ur", "12fc9a5f5758d6a3c59ed5950f1c7ee05f7ecbc5e36a342d09597e4c88079278b4b49cc5b1fbbc75cce00b51e03e842129228e23cd15d4e361f5b90c0e94e032" },
                { "uz", "36cc527603e82319e1f4524be3164dc022707875c486d513e5053676ede1767b76f898f9d0cbfa0dd2486b5e8075698c325e58627e94921e699597e299fbc615" },
                { "vi", "0a92f512d6656f4c2268af8708782640f8ec63003760c3ff80e38ccb8eb21fe0b7a808e583dfe0a440d8e5f6e5529e7fddf52e007986a1608da357f40d722f08" },
                { "xh", "633bf195702719b20d481030a6b26b9aec54d19b1c72d5798bf152d9bc4a4abd586992f3146146423aa37d6caf402c7357ec6bc7dec214d0bfbf9ca82771c8e6" },
                { "zh-CN", "f1ce1e46386e3a625a93e6d42c62c9f83daa0f56cea6d214a4d9299d39f45a6028f328e1bc968ba3e51f13a1628c642b7a748b56cd7601c6646f84a2f3809e50" },
                { "zh-TW", "5bc5ce18ad1de4ea76a032043cb3b73b8cf5441b87cd61cd12a37e2812f1b3391e70a0831ee709015a4498260d594a0d8b09fa50d47db1eedf60ea0d77340abc" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/106.0b6/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "599add1ecd9d62811596cf6b81d2840d7250bb144a8dc359b77b650e835117e61bc8aee75d6605893057fc61276f85ea38d10f7b642a48140fdf89effbbda841" },
                { "af", "7ca1775f11f9b2c8a5ea8b5c6870be4ca557b6c8621709738e92948f99d6b03f4e77043e44c1b260621503f7fdb4cb07f85c9d63fceaddff49449a6dca941ce6" },
                { "an", "148a70df1c1cb95b24c55a4c675119e315c15da02a8a1fe212dab1df7d6929467ca9acb30862deecef40942d36c71b3b790ae3dc7d8a0c45cdc3d5b805c6004c" },
                { "ar", "2d59595841b8045ecf1c9d4f210db729ea7d143d296339c5b5c610e1c65b15cfd269484ecc42cd3b203b73c0d87819558db266fbc5ae11a64f23efcd5174bbcf" },
                { "ast", "093fafad95afae0e9f32e20231c0fdb9639fcfa1e303d35a9a56eb90be7eeb4d4ab872c567b56ba4cffa0a395626b8920f75517846b9938f3804e702454db4a1" },
                { "az", "2e8e551b308b72896254c6fec8c32d0b66d235478cd3d2759a1ffb2c0b3576c9ae7341f46c545e7ec3cd224fe56bc178c9ee68265b2e3213e97294d8fc2af585" },
                { "be", "6c916776d3e9fc2e135885727f6fa08174caa846c7e4d824745a34ce73d81b3ac07f4c384f45cdf08cbf33a74ee175f6cac35b3f5e810e0008d4d75a1c42820a" },
                { "bg", "d2a37a3297460ac212de525f4cbf094cd81cad670fb88b0515d74cc4973119e85041449bfb0e4c74cc6d8b3d4a7ea2f4e16339c2221f1aa6de327ae846f9d6a8" },
                { "bn", "d8d9ed410b56ffe9ab2850c95c79a5e051d4d87a98b57c88e51b9ea28c5b53bb73d735aef9344d36d4fd0da52b297f076efde4ba12f51890090fb0bc4fba2e21" },
                { "br", "6a6df38bb3f6516ae563cb64c8d44208f498ca04bb5c612fc06ce0181e979024397a44c9cc8bf834928a2f6a259215609435956827005d5eac2e4f913d1003a8" },
                { "bs", "c6a553cd504b8e0e9317dea875fa370e991eaba57a98cf9ae310c48d9b3c5ea255ae0fb8a849d0660a337f125c7fa224aee9d676eb90465d2d86d121f8280207" },
                { "ca", "2c3a36a912091b57281669ddef5ed5ca33d550487b313451080aa2d33c5dc4810ccb7c3f38ea90897594ebdd29a6c3600aadc8c8290fc431ea4bc6467b7df46f" },
                { "cak", "46439f2507ed30b671ec96d12218a8d397e2d4f82122dac25dfe2fac0d8ab24ea5d9cbe65dbab10c03005109c9c6243c287b6fc783367d2d81b3686ef215bf64" },
                { "cs", "58c4d5723a000118830431daefaafb4ecd3e745b9b464423bfa6364fb319578022b9cfeac335e5e247cebc5804072008ffa3dfe4698a76e15f9513820577597d" },
                { "cy", "3e52982dfece73503f8df4718e9889751ce66b2970bb5869626930f750938104798b44dc675c1517b477ffb4dc2d66a421ea4f9446a385e734a1401e5675f923" },
                { "da", "e42be3507b9c240da992c0a9d48e64260292ee39fa0c1cb44f8f9ba4744d8fd01ccd955f3df186e9065a6ed6421ba3a4359a51009459a8285ee1d01942ba2daf" },
                { "de", "80bc7c1add8e39500be5bb535017898a393432ae4ac1cb3a9ff1000b4b031fae417d7efa8a1744d3adeef516d50b2ae4960c693c64e7a66ccedaa0f8c8963520" },
                { "dsb", "cb14d7b155148cd0908a1217439c5c11e68c5d4ef508f7ba673e7cd67658c2e88eac22d92a616303e226f91110f191bad5972c231eaf7fad2adc2d9c7b4d6b31" },
                { "el", "544be441d5a5b17d42fa1f632c49b90d12c8b3791755b61374a11f2cb4db1e32d08ed95c9baaf83c9a6d48fcd0e81c24bdc97d252e2dd1760aa4aabd43a2dcd0" },
                { "en-CA", "79e246fd052a987006e105eb8f2be4035044de2b257bb04eedb453d6c0af450d7ba6caf61ee5b63caeb96bf78506201ba07daebe22bead48dc51af9933aab0de" },
                { "en-GB", "d84761e7447f3783f8b5eb5e36c532b9406e77112c5c2b23d339729d2e963319789ba4e558804f1ca84924c83106e8fcc54d60f61b6cb67584629d774a64d339" },
                { "en-US", "744a028611cb41e5f76f257bd6e3089207e3b5d4c2eb61eb0cf152dd152368a26dee559babbd7bf1ba2401bf351a3181b9db31d426b2e82cc7325a9f6ed70ea3" },
                { "eo", "523909e4cf70051b00359c4bb73345843ef894c6064149170043e731627bfe5adac5895a696030cb7a9a4c3849c9e6bd292eb51d1c678ef5cc2340ec62762978" },
                { "es-AR", "6eb429ff7f94127beed8144da897b972d5f2422718ad61a84fc060b680d4d10d5622df501f162d990d74133cc9a5bcbd0cb73dd124fb9ef53a375c47e9f8b4e4" },
                { "es-CL", "ff93a53c868018ca35d88cfddde5f58599ad1cb958fd17ff09771ac308ae61603ccf9ab77b5bc2d6e19703916734284e38b89e2ccf7127d537edc1f79a81ec93" },
                { "es-ES", "17fa4e184dc2828a23b2210578b909bde1891d7e213aa42dd8760e8a0b93d432cec805224190797e74c92b5992778a612a9c83d6672763cc94f7681c1e7e2bb4" },
                { "es-MX", "ea7d381456784ada73ed6bd5946412d7a85e53450c0a6dc1145ffae97bc079867180c08f317f5f0ac8d1d242451721998ef2ffbb1cedc10eee74409e503e0481" },
                { "et", "6951509b5693878dc9a75b78dabe9e1e01bc49eb864a3248f04b3f93dbc049cf3f7e9c1fb7bc2e6fe05833de7231b0c7d99623e1e2cd0f8c4d6dd1afca610909" },
                { "eu", "fa920fccd33a73d450fdd7de0b6539c37d50ef7f25c42645fde8c5bacedd6ff743e719dc2032a8b0388de7b503260b52fe842c505fffe6473224638848f3e3d9" },
                { "fa", "24ea53e495e00db519f9b2185b62f7ebda714f817ca12c5a2b837a52b44f50891b75fd46fa08f9a3df7b49e526c8f4d1a5a0191a42ff91a91a6fec39cc09afd0" },
                { "ff", "5aa56cfd291091749eddb8b108be4862e298d589ad3379fbde2b95e04a100549a14ef68a5b0a6460b5b0229cc7d0f4b56f6b197638927703ac0ba0fd0901f192" },
                { "fi", "57a258fd5f7e1f2db944b95cf248803298e83a6a73c07a249fd9b5b0f3b08f590e041f2a4d6341e57600f19c9ea8d96ee42589ced0106bdb644a5672ee164d1b" },
                { "fr", "971164edaaf63697edab38a0d9c7f57e2dea558cfce3f5bde6a8fc137684d743fe0c184368bcb0206583869a79141db0a15ae5230e919ccbcd0cdd34bbcaa50e" },
                { "fy-NL", "d5885955a6a6cd6117bcd2ddbf067266b174e4a83f6c44755d955295867b6d2bdceaa59e345786b2ae0d464bfa059f5e084d1fa2bf4d1dab8b3c01811a1f3256" },
                { "ga-IE", "c34dddd1a424562e95046e8fb234a517850e21dd3f34d7f230f9bae862c3b135bca4771e47ffb1b01a2863649606f5827645735ded89091d21b94c177fac0bb1" },
                { "gd", "b3585f8d05b29ae66f0c5975c920fd01cf032567f87ba7469fba8e17530d55dfcdd55b35aefa799cd5141041176db5aa8536d3b0ec879ed72181cb1e5b13a9cc" },
                { "gl", "8583b5c0eccdeb97ae2d41c12f35bec2a3e26f4b271acbd3b6a60dc0c889465a82c9199ca75ab190dfd04a0d935ae2c5e541878d68e9aa9d984c9b3b2fd2041d" },
                { "gn", "cd1eb5a592f27c6546e631cd00905b4508833637b0a12baf527cd7726f2492a604ce8aaa3e8bbae0659da070547e9f8718d6cc7cebaa5e7181ad31fb6c980d8b" },
                { "gu-IN", "8edf0adc7fd731ad91542646b8614de0c096290a1fa088d2dc5fa4b7a7064629ebab30230e88a1dfce8427286efe19ba6c59ed57ccb12577c0e72c6d62fde2c9" },
                { "he", "fcbc62472bfb9b61088598fe9bd539b8296198f681479453cc40a8b824a63225f11b3e6d8d7d49d68351e0bcb2f5804b8e1f7e98f19ba76314031701ec586327" },
                { "hi-IN", "a996e420c16aad4962478c7359e950d25a5c81a45672e2742f40095f669fd3636f6d5b509cc9500c318837017543cee3c4ebd22dc53399c4fe140aa5f59e2956" },
                { "hr", "4accfaf008f37197b9f82c18a3a114ea4dc5f52ac066e418ebbe4b51c395458bbed07ca4894b64e1e85a7df883f76f4cb36a61bc6c326d5008cfbb566845027b" },
                { "hsb", "4fe671c5904668d5c8168e1a55fa196701e890b2c589bd7cab352666c547559a78d0fd431eeb8114623d7ff2d2f155faabb4164cc6bfffd6ac8708d385866161" },
                { "hu", "2323aa943ce9e465c08dbd5eddde971188a05650a78afb8a3d47d87c40735720295df2c85cd16ed80e2dea1d60fd98158cca43fcefbe18b8461b866788493cf5" },
                { "hy-AM", "ccbb5a936f5d86a31455d1dc38040131ddd3171b41d8b6d0f747f69ddc2ec257b6701468e95d5e11bcef2a1dfb63ce57794fa80ea1db0b8956b1bf8ba93e7e4d" },
                { "ia", "10680cf56f3e8818a2e0282b7ef84496afa800daaa862da823c3b6fd5be884b44ac41c8145b3a9e81badc70bd93ffb4d51341610bbe192703b9239d77860a5bc" },
                { "id", "77479c785ccc5f7ff175bc42b66a192a27ea0495c43170a6d562efeb2c1b6e48d71f0847ca86450cbceb57e30e2f9face3be0cd47b95c5a3530c097694105c16" },
                { "is", "6e797188037e31424058ab8d0a661c5efe234908d58d41788cef25cdc79ec7c21455416ae9bd0fa5d2b258608fbc00141c5312ceaa0668454c411b9c3254f308" },
                { "it", "4ec916beb5bfaeb4728dec48494982e864f753192b302c29357408bd73cdc6beef460bd924161edbea72a6e9761e7294bc9a881001134ea2a341e0207c5216c6" },
                { "ja", "d26355357c7aead9c823378c15fc5b8efe228ca504a5f5571e0cabb36e4017013bc80afb1bd4a79a7ef45ea43c20554c11b2cfa21ca54a823b1824ab528f360f" },
                { "ka", "557f2e544683c4fd62c9e2aec90e999a0815a00195538af40e4a5e8936de96c17bc3f96b6ab6dc9d393563a8e446d019ee1d7cd25ab927ae61555c6ec29232e7" },
                { "kab", "cc429129ee95eee5b26f8ab5e07baef4ae33abe9972da8e93e8ba6c1c01928c2ac9e82f23228c4b52ac8006aba9c5badee5f69f51054caf80806eeb5eaca57cc" },
                { "kk", "fa33ed0bab0d147fcd349dcdba4b65bb5c216a0dcc74689562672d099749b95415a2d7e78769f026984eaee70db14d0036b500db23d3947852386f7f09e96ad7" },
                { "km", "d4a769b9c1d370804d33ff1fd9a12f6204a4e633bba549836b369be098860a0df4a23d86449e412ae461163456f29af7e9df65b10a4e5461123badb7a539edb5" },
                { "kn", "25dc4ec26020929243ddb4e7d17d2f78ee169c176eb018b308f4a1c977df08cab3c0d3c6eac6a40efd43305ffbd4b9cd5629728ebf911f8a3f2ca9d336a4f986" },
                { "ko", "f4d0cf1c75fffd4ea0b752d46dfd95de86d2be097a6e1927e5788a5a21a86dac6e9c449ceda616d0c19949b7b562caf82ebf0992f03aa995daf25fe1fce5add3" },
                { "lij", "d550a867ff40cfca1a32ec26126baa1f3fd45c5f1c5ad4a06511586d3553a9430bb1584ded38e0c5f987355c1c30a2fbac258a8367e6d8013cc76d28b61c9046" },
                { "lt", "4332007e4bf6689c4a5b5cb9c4ce1f1382050f0ce9d443bfd2aedde8ce8359b250d244549ff2ff101e93b0d6d865b14a03cd51a45e50a9e2121ac351073c98d6" },
                { "lv", "aa7d6c96eb766ec70a6a202f30a30693eaa18ee587bdc823e21cd3fd31624df4b0cd4eaf4a6e0dbd31fd4ecfa3f65c403b1ebb272caf9f0d9fd78355346f2eb4" },
                { "mk", "f88bed8c32cd93d61c4dc2ef22b103f1c2e090c332d426e3d0ece7cf0532af4a3fb60640f38af099df19cf9024e524d22842ec5385bc5ac041cd63a5c6d1ffc3" },
                { "mr", "4838f7a225e19f58b593d0acb736f967edd4e8bca65f5e577be86ea73295e0cec708ff1983fd5753e4e66dbbf03aa344eadb15ca94af1c7a7feef07d45d1426f" },
                { "ms", "9d917782cb3dea10271bd9c71f1d75f1395d624e2492640741fcd5a4c41e11ac1fc0aa4f358ed9fa2cba1c8f2f14713f0096866e13c91de0e473d76b8e7beee4" },
                { "my", "2fdd463fd86e6c92e08d264563d916a9dee8cfa740606177e8b1ec9dea98ecafdf41077da96ca2216e9227c92553d26a8867b88b99eaeb6dd5374f6f6a0871c7" },
                { "nb-NO", "33061e58ba8307789bd4b6e76f09437a9d0ad74dd40ecfec2ede6cd019036b027a3dbc718cb37e049058321c8d81450c201dfd01eb9047aeb55d260d008b3f05" },
                { "ne-NP", "acb71bc8b27b46bf68cc927f33923bed850bbea2dac0e160fbebf5c32913568a6c32d250f496c250377aab02744a4da4ee1d37db23a2a453a600116016833597" },
                { "nl", "542c7bfbce69b6edbc7bb7f645559e9fd0d9ad6fc1dcaa77049ddd571e70683b4f9b47346702c1c61281036ed1f43a6e215a4191cf73ae15029f04b055f72c56" },
                { "nn-NO", "6861a468a3855fe8eaf9029716738f78d88d000b3b0203a0e4bfbbde4a5abd0d53173b5b29d1383d885dd7c419122c8c86f1434da471b87a19841e95a1aea56f" },
                { "oc", "9e59483b2f18b8b355f6cedcde9e308d0c06671d11e26084e3cadf2f35310ae48d47b3ba5a0886954daf33ee2f2bc4e4c93ac8c039a81bdf50bdec20e6667b7f" },
                { "pa-IN", "ba98befa2b5812346dd9656cdeeb928c478ffadf62c004f9ade41ad0615f5d8505b06db53f3b44f9ae41388947bf813f8c32eeb876deb259fa964c60374f0fb8" },
                { "pl", "b141c7ea3ced750d8f901ab4a99213e23149ac4769b1d83b7e68c5ac2902a7ee00e59e9ed2c32a7328a58863214d96f1318dee29339217f59c89311770e6c63e" },
                { "pt-BR", "3fcee312fd03461650983fd1282ed77454431245eb3dfc290f3cf65d93371cfb7384cc2cb218f58001ed150efad045ed7e0d9957578f89f92e7cff2927fa377f" },
                { "pt-PT", "ab122a535e8a2ab7e011d53b036cec2d29d5de1cedda57634e1fd2994fafe515b5c4ba0162679db04cb3683284fc51eddef2b832349a9352c826d8a9048ab1c6" },
                { "rm", "3971ad7185ae375fb14dd090c72dd8d1d84ad2d55e744bafa53fb4deb862ee9151ba74234d595e263e186600b22d431425c5de9bf73bcda83df766d953491d88" },
                { "ro", "5966e6dafa191f2512cd50de8bdfec6a5e60b7a9a2ae573489fd6899a0fda083714b9524bf914efab502a2821592ae5cec0f08a3914222198fd2faa425bcfc6a" },
                { "ru", "d04b24c9da5a52da298b36a77cb50c7fba93ec2e87d1def1228575e9adeea2d4a2e240430854d439aed0da18b165d921ad7c3af5e6db487611fd7250363842b1" },
                { "sco", "272f3ede8bf10967007c4bace581b13f0a0253ed9b7a905b489cf0bf5e70499183ff930b101030e7f6faae05562688226e503126bda84bee5d473dd071f3a565" },
                { "si", "fb177e303fa17ef908ae88cda18e94c532e1885f82683267aa6420bd1399ceb4c5787c950f40f89cb18a5d05e54e4de968725d78431913153a8ad94d201cc31d" },
                { "sk", "20cc3d56a2500d30212c75f2a6c267cf7580522238b4c4ca1bd96be20e5841b150d32d9d31fdb8a1f490a99d01441a70767f66b122d0879661424154f8f48c86" },
                { "sl", "d49f254abb8a91a8f6b56b0755795501e8adbbbedf10952ba3be3460ca3c591d0029c656eff077aa901ac7b30100e7537e02d8094ff9065ad14e5b94c2a32754" },
                { "son", "bd62f6bdd53af496a98adb3febfac70969ae43db356266ca0296e5d03c8c3e624a0f95c546483130ba233650bb8d3d2a078f0feab068967e4b30cb8e8fe154e0" },
                { "sq", "8c95a6043e6b3ecdf8fb143fdbe203804828bd151f4cec1938ad0627ca1decb52f8c51393d12c01abc6a57a81a925ade1d99c8f28efef7e81e5da9e255de533e" },
                { "sr", "441cfcdb4e6410d36b50fc04b7f2e057067f83ebc056c453702d05c368ffac74f475b70d247894c626c4a8d4d2ff0f380ad725678237d517c7435733f26b5649" },
                { "sv-SE", "5498a6b2cb36eecf31ef2857df21ce6aab7562e04696db5ebdd2d0b76b74fc7a33ecf82f4f3d0262de303a43f66a0ba408d6178ddc51984e02ff4020589f35bc" },
                { "szl", "a04e61ff64ccfca6d97e36fb60fa85d4ad8bec12c942b8018844b2dcc51f900757a7abfac8b96b1e2f28e7c402e16c388d2fadb0481b6ca357bb39865ee61aca" },
                { "ta", "730a500ef9d74a0bd20460e48f28e6f352b0ebdd3d92579a447f83ab82065650663f366e12aa676ca3f2eed1f3ce8d25488d2f12116a5fab51c61b0005cb4388" },
                { "te", "4ecc04b6c2f0d74fe7f9e2f3cc4f25c075879aca89efe413693d9f23dc321f82a4b75e3fefde0ae53e2a3bf2a057be48c3d2b91565ea3a258bb813e77fc26a65" },
                { "th", "2c8a69444f7308af5abfa4b517bfaacc0b84f6592ef707b46d61824273ada0816cf764b581dc1f950417af16971e737a4a1d742ce595d8bf7f8cbcf475a13b0d" },
                { "tl", "980da54f22afc9d523f0e6439a1d03a73ddd75bad053eeb6562f39750dae7cd4688d3de10220aa168763c816455ed2e819c98632ef63998a747c2d5dce203119" },
                { "tr", "5482afe6d3e903d418bd3b2ddc11badfe91f5e38c66f39d3efddced202f50e4e4be20bd268144b6b8979fc80a0a365bdf39221c93dfb8485452b32b1d89bb2d3" },
                { "trs", "a272b295c10190498197a1d7522f033c47e9f92f250352bc1e387232467b90e63bd5ec65cf2f8a56c7aea09717797b5affe63890d7bdf93ac40cbd7065a12626" },
                { "uk", "7db107d077606c563f1bdf8e94832999b738fca645798e090eea603bfe59a5e2240aa11bda38a0242d79afe11001b580dbc46d84bd0dfee79b467019d53d1e9f" },
                { "ur", "4d2d9ad0265fbb4c0ffb376f6ab8f13e1df983b5e28852ad55bf524286e96b8d75235259743c440239af0e39ceccf9c8c00cba5fbc49bb650b7ed238713daddc" },
                { "uz", "d603657dba9fa19a44aee1d1c7bdf29dd46378618896c2e56ceef7a4c1684748a7e0b00401ec00d2ccb70b680652751213b3f3987bd1e463916220137eb10ba5" },
                { "vi", "85b80121a571e0243489e9e06d70af1dcef560002ec845d003e55d4cddcf7b46ec4de75b9c2b983c72d1286e6932077e9f5c8151ce93772e6b855d085bdd8b70" },
                { "xh", "3b8811d6eb89dc111c0848b348c14364a0f2476915d5d6640446a1e6e798bfcb80a5ad8504c1e5c586a85cf1b2a3b1693423566f4b6e43fe46fb01ec70ded209" },
                { "zh-CN", "cff7dad2c3977fad5d02db3f400256e52cf96afec53794ded7fc3084adcb1c09ddcdc17a335bca042b87719a14013674cb787fe1862ad2ee096a36f19d4c7889" },
                { "zh-TW", "71650e1ee3f60241fd8969a6600122a6b97ed039c4962995ba8d7a7a37fdb198bad5dd82a7fc14c3cbed8d8a30ee9cd2d3c9e360a0e4346547458d036631f3f4" }
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
