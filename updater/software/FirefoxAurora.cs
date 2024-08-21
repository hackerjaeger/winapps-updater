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
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "130.0b8";

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
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/130.0b8/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "cfc86f8041d5a078bd00d4874466c19caf519b7aef4f4884cd4905f599bb2db1c15da7bfd95598b31f22829e64238ae4dedfb7d3b5f2cc75af6b45b72fa843ae" },
                { "af", "5f99df80b1d97f1671beadd01320a73e0cd30e43513c13e278f7bd6f3f42f5be58338dcf32c223e2fbf5a406f674f7cd9fce483426dd49e6be4f3990e25025f1" },
                { "an", "b38cdfe68381adda29fa102dcce6919bc6c91fdfdadee38a0b3d8cdc987abc953291031cc2eafd3a83884bcdb3ffd4ed5d71b2d3540a29cdb7b668993061c394" },
                { "ar", "ae67568da00c6a6e2f57d8e64cad28e2e282d537645cc10f5d3d4d9f14c6cf0b6122657525e0acae0326bda134c02ada9bcb8468834d9c38dac497c86ff6a2c2" },
                { "ast", "a6a8c356b3e96cb724495a52da0f10147fc1f73dabdcdc3f274557bff08334e4ce672ff2847f7cf3d8c151ca6db887be2d827c6f3da4f6a39080e8ccca04f283" },
                { "az", "086517a77b4590aef31f5d754087819e97e870f657d2044b4881ebf80b1ecce78eeef6796a9b2b134aa53a4b9d618ecfef3863873a7c291091fbbdc89992864f" },
                { "be", "085c98a76bcc0a443977bbb15398b75380ce97856266992c90570835f58ced585b64368cc73196f5fcfc7c3396add80f70545659bb7ffb91ab9482057bd18ef8" },
                { "bg", "c0e8277adcb04d60cd235220d061a504cbe36dc053024b67125bfb1068e15483ae37427a402fb1b1f0195b2ce1411419521cf5ee31628695371e87ef5f3f4673" },
                { "bn", "63ff684302f5959f406a1060534bcfbb420b50102e9f9878242f318f3acfb06d3a100e13f9ef776f835dc2e3104704440ebe14c92b2eaaba517340c1e44fa1a3" },
                { "br", "7dcb9a1b9b6c9d607992f7e785aa2daac6a3f1d1a15032d92fcd201650efc3cd38d48d8915294e4da605d8702b2ee32cbdd295fa6e926ff0808fedede1658819" },
                { "bs", "239309df764890925542900819e2c7564acd42c0827026976117ac12c0a0e7c01cd44b8cddbcbc2e7f9d203ea5f4799aed48a872348360228f801c015e83db32" },
                { "ca", "9ecfa0294c10d0f3e0b30af60362e0f9a1c3c8c1637fc670c7d8d1494e768c5e5fb5c420173799f7e1aad20fee7e3b53ee64aa7c0c67181ee86c33367124bcbf" },
                { "cak", "40f2304bc6794d25b949f4f7169516005e76256c9348636266b42cfb74221e23f7459131eae8f2b61956f28b2e9b5df10db2b41336773f3e84a60d9f0365e7b8" },
                { "cs", "1e80a57d28bdcb4239cab38d7e48c840e18e3a9b83a168fd721b35fe91b1903bee8c98e55a6cbfb0d6f539e582c3fee09b01a298916c7cd1efcde46530408513" },
                { "cy", "f7d6fd0e126711e76c410ca73775e08e2d8b3797acf9f3541888d9263b24eed4f76bdec17736d766ad61f09e3fea335c8e464ad076615e2406171ae19b7cad7b" },
                { "da", "e685082df96937d387e56baf62f0d027c43fcf2a51800a01bc88025c26ed4c93065112c8c6c981fb137f784e8827ec7aed3fe36972a38e57152439af9640e20f" },
                { "de", "3cc4050daa04c85bb22f20ad25e723f473410cc479c3ec199bc42e7fa409f5e81176db18ffe82f326e49eefa9be6fdf5efed7a49e1bbd23da072ed66c38d2e35" },
                { "dsb", "f92997dec28f854762f3e82b447f6b0d39bbd385027fffde3e49efe7237388166fe92ebfc224d25a656d0176b13996b3f7079e64b79572c7a87c34a5aadbed61" },
                { "el", "bf2de5fa5cf1f93962e93d29c3d83108e64d4903497b990268632b28c7b73b6e3f3fad9ac741f61f397f394ac8f7c78685e665f110731c2410f09abc1f49e61c" },
                { "en-CA", "5333bbcc0379444357624696be81065063cb70e0bdfd18f1ab559507f4db2c8ea58d9730b9bff186924bdc2c4dffab2fe811f3938dbe059f98eec181b4d698b9" },
                { "en-GB", "2becaee2ce6fa3c5018dc54472d083348295644a14323b32a0e9fb334f35fc0ef50d0bbbae0f9e1796629988d845c95c2c22ab1136ce9fdbb8c9553beb8454aa" },
                { "en-US", "9b7558ed70d319d80451e2240d3361820effd9f9c3306a2908c5595bda580f1595a7a11514e760084067ea335a3a65d0107507d132e16b906c126678701a8d7d" },
                { "eo", "2aa4993c2b374ff2be54386cb7ae283675cbdd6168ba373999239ab21aea0f130439dd2e4b7d64928a77e82ed135a0f8a5bb46e8f4ae2fca8843ec9248ee9ab3" },
                { "es-AR", "636082124d589876ee080b39b163ffa942326f16ed23fcc35f8880b500cfeeab0d40f46346c0a147fe79a46d5636184742096d3bd8c89de0a2b21ebdb207fc0d" },
                { "es-CL", "5790f1b2f9cef1d57ff65df7c2b54ce640a6e5f2d7b98814770ceb431eb3cd6ea14cf0050632c2a1efbb31ad5d5a95ee9f9f2a55ad177b55a4807deeb5b480e9" },
                { "es-ES", "0cd11f69e432786cf1abe74d41598dfa261ec3c485df228c872a6e3b9d3068d20c74cd17014ad50d70162762689980c4736134b16f356a51b1167559611ba8a0" },
                { "es-MX", "1c02cf1998ce9cf63e98833418f775ef8233514570c8d05472dc77c2462c03c61713c310e5b2a89edf7e1c34bd35eaaf77ee56e7c11a47d5e4e0383e8b1aedb1" },
                { "et", "12dc7d2f928d89dc26ef340178f94dda12a3d0ee97f0250578138d8428c6d8e8f6ffe470e622e05b627d32c82ba90d10ca983fb4aea54bec4a7a10195bde03ad" },
                { "eu", "fac9855001649d14ab361c5cbc605e5dda7362dbc0b0adf6b8d4d47dcfec1b2a8c3cd8d81f3098c62893b7934c00bd25d4f0f18b911c5a9817eec8ee32e9ac5d" },
                { "fa", "32761aee2261bd5ef27ce5b86e4ee03b4aa2f24fcc63a9e2a0cc44a24913c95cd9a766bdc41d168f2373c3f786e6be9018adb12386063755c8c263ff70f091bc" },
                { "ff", "619b531bf672c273308eb10cd3ba4f731cc05ce52a01c5d3bb028b600633c246a72b6f74f9ffd8f37bfd9675f5602aa784c226d4015e75fc08b5fce34c6fed8a" },
                { "fi", "3ecfaa44ec879b401511c9d0bdcee3cce3e803f49d75112c6d27556d028a5fc8d82742379743df1e4f2daecc3c6b077ecb9aaf39348a138909aa98b1cd3495bc" },
                { "fr", "a622946ad3dd42b87795efa6ca9907a42c732f168b1664bc7d3657c5023bb736786867f020b67f51241e90be09d85160c0504a06b0225958b7e02a1431547db1" },
                { "fur", "0c574b3762ffff89c2378d7b31e46aa4667a2337c66b503c6c0342cc0405e19b558dc1bb5473690947ff1672d5a77970e59bca3b5c9a5002139b33bb7708aae8" },
                { "fy-NL", "5b30d9d525a24980740d1c697265f2b36aed377e0032f13842663e0979af0fe3f921800cb35f0b1e27b4c23efaa02d88511c65ba9c3300e41370901419efe3ae" },
                { "ga-IE", "f3946e632a6373cb25e1e461bc670a5e7136453fe5f79a33ba777a972e06b972a19879bb5d6dd4d129adfe71fade55074781428d6b75b02892ef1e78771c09b9" },
                { "gd", "2ae19e07fd33348f9253421cdf4f03b0a852a486fa0e5ed19c93a6f46fefb012de830a10dd901450d0931cfd1b32e329532a87563477535e8d80424dd7bfe63b" },
                { "gl", "48336b81e2ea8e37c3541179a33a4fdbc28b169d610f39e00bac0901ebc26e818b2af14e9c03a0aa670b47ec13d482790d17dc1f6c27aad1f5b047e9f597ad8a" },
                { "gn", "e58c629733defc91ed0000cc1bd757c2c1dbabb0f5386860431e9afc868a7c816fe388168d5ba40f17bb13c58827ad8679e828951ee5330eb6e801b3649c749e" },
                { "gu-IN", "c0d2229ca6f760206c5b839eec9933985265b5f9583c9dd5a06212b99187358daf4634499e6fa7ede76a8814c19d7e66ae289ae0646f76ff6d84879853d91c13" },
                { "he", "4a264d27407f37f6f00b978c7aa9dad16d54d91cae0fb00027d72dfd863c20153622751aa28730bbda6a4c1e8ea8d8397c9466683ea6519566e98d50d945bf19" },
                { "hi-IN", "26ecfef0a9829aa37b9d3066b43c4208eab9d91cb9092e26d96c5553443440429be7d55305411747d56d75652b6d04e2ddb27faa9b1c1176a1f22d893f2d7916" },
                { "hr", "3007e6d0a81d3d869d2d3aebd9acac750a6326d182666e050dfdfdde899eda9b0bc02a52a5c766ea13bcc22d81d022c73453651687602a2c6a37b1cfac23fbe7" },
                { "hsb", "8ccaa7cdaa1e434c0398ebca644ece3e33ee2c32fdacfeeb328f5290eda590cabe2a96239e0f39d6f3f9d3976b75403512aa5cdb6db73cbbcb4d856260d4f180" },
                { "hu", "39149354a9bb5adc02e2240fda9e5f44fcd62989bd43dd1b69cba8dacf6b3a1af75469798f6424f3c64ec18e97ec2b5d18a409c96a625b74232f1354021e08f5" },
                { "hy-AM", "27bf44c0566fdf696623a88771ce01ed7bcb4a0b8548b65068e874898fcbe013babc75f3119f0394b81725928753ba61289ac741ee9f843d87b14e4e2a16560f" },
                { "ia", "eec1c32f4f75ee3cb89c5f4d92311adfa0b19284249636f90964a0a3b0d8c953a5683a27e07d4da0d90edc30d4441c7c83cbd4e90e4b1859ffd6acd2fd747773" },
                { "id", "dcf2c17fff1581225445eaf0f11978630583b5ff899024de0e21e970ef6f992f014c5ac537deaaa456f77cef7563f7bf1a6fe9edbe95700550dba59bfa77c411" },
                { "is", "20c54c183b756f9ba3d2b188c6fdd328409d61ad633f3dd4038bc317f04a1c2ff87023b0cd840293cb7399ee97000998bf871cf18f48e3a30a7ff3f77809890f" },
                { "it", "517918b4ef82d076455ed4e4c65c5260dc043a0003a0639058102be68eff7517017a775e72c9a5fd4bc64529afc9872674c0436b68fe7e72eb231c04ac0a31bc" },
                { "ja", "338c376e852bcd42829e7be591279e3e9dea6f8c4c9b0b984e4377997c06620906f8b9dfd482cbf7ed3d6bb19f05ba3a01b08ba64913b13c3ccb63db8ed1abeb" },
                { "ka", "8cf474c81775291edbe4475b93937ba110bd552048e767e6d13f43ead6066476274b23c4b388267504478389e457413887893fc7fa807ccb359ca885ac78fbdb" },
                { "kab", "6a6664f6463b324bfdb2282fbbe976f181660c21ae299d48c3e9b99a301e101b7e7458d637052869a80f16b5c4811706debd985e90aaf0acd0e0e92b52881c87" },
                { "kk", "77ce0445a5ad918cdd130ec4f49337b62fef60b7aa2023b3f2b85508fb519b3fcc381fb901ba0abe576eb3241665ce289628f4153a92fd5dbd11c0f542687c24" },
                { "km", "75034538be13de7b03fe1637cf391965c4d27778d10d2d2e35b94a40b3d30ba80c56434066c99bd28ae77737ea722c3d0af90c5ed24b73146e0eaac81b069554" },
                { "kn", "92b256f90f6995f7d6fd689d03dae9ec90e993ffd080e3a41a5c33d880d3a57473b11a0670902ac1bda6a0658f6c30225b68315bc8e774bb5ccb5c2ff396e72f" },
                { "ko", "82403440cbdfea84add8cfcb3492ad95c5476c7f9c187eb200365e4c8455654504b1a8b2ed0991f142e16176830b40d60eacb94e339fc5386e205a90d8d21c73" },
                { "lij", "3efbaeb46ea817f68bfe000b505498bafe1fa91066bac26cae85d6748e17f8eb516d29e6ef6aa135da29a10697a7b2994deca23e04a3b71ce34a62673f35d7f9" },
                { "lt", "67c848a25845fe137f19c6a38f26b2502f09e228f8b46c7b0b8d29dc46ec23e54563bd9242e94011b5a82ab2a413e9d32f9f2ffb2453f65c35536c2a0fe8c87b" },
                { "lv", "ab9cdc279500f8b87b93b9c7f5eb12bf4080020f4f68e58867497abefa161bfdf2aa66a62641fbc38782ba75b8cbffba5121f8ea374d34d4aa067b4dadc4baa9" },
                { "mk", "59ff9d496349bcafa31bb4020ec14269a4c0a9487c735ca96f81e7e08a41c629cf1246b5b8a34cb25a795ad9fa9f1ace120741643b1796051772ab5f5b9ed1e4" },
                { "mr", "08c15dd4cc2efca1c86aeb14e045c7281e5bbb35f89cf935c806db07fbb8ab79852d86e36fccbe544813b7dfbaaa58c4c26ad8bce9dd2ddb758ce0bde9a511b2" },
                { "ms", "7977508716bd8f37fe978de0e88b53aff71f515d4e751106f1f4e90e491b6cd3b5ebcbd331ed5d60d7bb6fe27c6dbec110649666c56b51f1d2d270e360b72a10" },
                { "my", "431729e2aaa8390eb0572899285f8b6a86feb04ff4bcba5a64c325368ca994e6708d0a7d427b49723c7d0096b54655198189e664a7eae8a97a2aa7492649cc6b" },
                { "nb-NO", "95725638dfe70b37ede7bb91e3023e74a6bb04caa1f6d71c9d08850bbbc80ea9754ddeb69f59089afd5cfa0f55db08cf7d02a27a6eea073a4921d045619df56e" },
                { "ne-NP", "3a75bd5635ad126caa0337d90b0724c8265cba77573ce6829c67455a06670a3af9d341a69e8dd081b2070e37e8d09b42b23bb141daad98766fd2e07f035ec6ff" },
                { "nl", "f67aa1e3e43c63310b6a22fc9a536eafe3f70df4b901949678e16a9a0132e68995b9c2ff3b59cc15cfd0b55fba9d5de393f069139159568f7a02b0154863a843" },
                { "nn-NO", "32f9463ae39cc97604ac660268553a1fb2de4ed555398e70119c9775242c595c7d860aab6d7eca9d7733f4937fe2064bd102a69ef9b0b5e96b52fd994feca062" },
                { "oc", "8fa8943745859c016730cafed78113b2c880a1b578c85c37e6db66107ab1d130009dc10aef7fbfdb07d7034a3a8d3b066ffcb72995a474650df2451f7a75a67c" },
                { "pa-IN", "36cf0ac17fdad97294942e475d0ca90ffb7afadd2d63e6b3909362bde4a29f8fb891f48e82f88cf65eff2cf936ca879701624e5792c70dc6db81fb7c9f9e1685" },
                { "pl", "4ec5cdb9c43530d89721b651ef72055e66adcd2a544a44c86351263c49170c870413c21ea9fe922e899f0e23404026a6b3065578cc496e4201b6150549392cbb" },
                { "pt-BR", "2eb0b66176c50e15f84a8ab714c7b559a0d3249f2d5c8d64f95e0c433ca647aafc1779f52fe905618d8b56c6968052131acb22ba6d37c5e50770f191b99939fc" },
                { "pt-PT", "9c2e608e62b724a63558fda17d58eb19e1cd78763c4edc3907018308a0a891ab205464da58397d084f90af57286df2b91980a3b8acc7191267de7580e70338fd" },
                { "rm", "6eb072557c518944c0900871f3262229151a388fc18983049e7e391f8269efc98b6e019c34066a1475f92cdf0238da89106ac5c520b309f238c16164d6625c01" },
                { "ro", "3de8a35dcc26f5384cf65608d58307c3783891b81b81892abd5a2e43b80f64db2ebce21ea7955cce755b79b265926c901b3854eb4e6fe579ce98a31f64891ddc" },
                { "ru", "b5491b0a1b6683dd26a1032e4c571a164d6f5cb495d7901f7273e00eb7cb62d55dd1fd57cd2a8d23752f2ed9cc6d2166f88265264320a61c3520ab92edefcd4b" },
                { "sat", "488690599761b82fb3882dfe675c0f8e730d063575624d17bb7d0b5c02c2bd50907f1a42ab2c5717ffe62c3e309d7ead00e8c106280626f5ee6fa59f9deb0870" },
                { "sc", "7f3fc062dbe5610d25f7ab5c705f96bb6e8dbb3e0e042883353fc9c40cbd3d9960cb2fc0694bee9db532c8d18288f9d996fbebd8c820dfc6b9f1d2ae0bc657c8" },
                { "sco", "d6be5b01f51ceaa68852d812e28353122b885024f77582501f0f77cf1d4bb48915d6efd13d8b655981a31245fddfb0c810e1832c441d8974a320d3b3fe803682" },
                { "si", "4d6c4e87bb4bf3294abb27331a0dba4fbae3ce0dcc13d30fb7b7acce15b4835acbb0ce177d7ef1af774c17b6296c5dba1cee78791976ec350b81d0c0ae2bb26b" },
                { "sk", "c4a342f44ab1e1697ca68c07367e8d1ad9d6a58a36916ebf6d8fff826b57296d6f447c012b1bbc5dbd81e80177aef03069b9a22da20dfc6217f58c9832823592" },
                { "skr", "3003661602e07ecbf7153629f29d3e56e240c59279af7d01504364e1d9682f93afa185797f63464e66efa942a74d904ebe15f68b07c2a6d405c8bb63d3972092" },
                { "sl", "5ecaf46e25fa04dd43ff628f5b2fe5373cb3d5d2d8f1fec1a6f01516d4193c547230e63d398b2b419aa951f8ad84d7a168785215b9a0bad67bc1e9fc636ed7fb" },
                { "son", "7b138a8f27377cf99b0df45fd09d4ca191cb35e97afda7264813cbcc3e40f358dd0623e8ac8a5080acc554b33404946720dd35036c6390fb8cd7602b6c3716ed" },
                { "sq", "5e4b723e6694cfa9ae9e4aa1a9213f289db49ef7a9e39a49ae1d6644e4c24a5e04ff2c18226f9a60e3705e35b73a22b70a366d5ff78a707156e57b9e04da80c8" },
                { "sr", "bcf8be7fdecfa90c92a755e5ca4108cf4ba0cd25ee9bb5142535d3f8ef8b39c9c515ca77580799c20f954a81b1c8dd9f9b7820a73b2f1289a43857b79324d0a1" },
                { "sv-SE", "d65e4f5768b9c6c002c2b01953984d56f670fc20400bdb53d94f4982bfa7222188f0470ea9c029f5f92ddfe354b29c80f665205de1680620ed50f8159b3863ac" },
                { "szl", "3eb363ecdd3243d868cd893039af89ae0f2c7cb6dc6fbfd203a1241bce0d97445c8f6a6c436d795da65d7debc6bb3a89979630c79209b1f526ce0a3399bdb174" },
                { "ta", "9b31efd1c947485a4d4451cf6c5b05d75cf3e725437f2b0bca674cf4c37bc56d86dfd04892fea0b702b7011b43c1e16f79affbeeb4aa1884d38090fe953920a4" },
                { "te", "7eee3c51beec48a91cc42adf22a8a3689f2e14eedd882a869c9eae16574d3c77e1c90119844f3c5452ba917fcffeb709d62d96131debbc6765f4f9662e378311" },
                { "tg", "7a1789511743fc9e473cdd268b9c2a660137c4f3a5f36c9e5b19c2d00c68255219fe18a2d9e05f7a5fe92eb475bc73f99a54f3796cbb4eb4fadfddf6c8526915" },
                { "th", "b703f246e8c7c372c856d6e4710675cd1fc26ecacc30621f2b9558b3e4b5962f186c6884a29da1affd26fcda941015bb2f306aa8567e86b1675c97d8ebf91123" },
                { "tl", "98ed0c88f196e4e11dfe7a874583edbd9be0a9595846ab55bee3a609c69cdc022fc7d8ef551888baa23809f24cfed554978e413e9990d3cf4b648256d1ad3e2e" },
                { "tr", "499d9aa27769007464846bca4ab64903e4f6a95394bb1f8e397c5124f61762df2b17bf9cabd4342a45094b4dcb95e532d74b3d8032900a9c85fea6e2415a2998" },
                { "trs", "1767c2b93c6cf41f391234b92db63c12662881c6bcebb8aa00760fac5ba471426648e5b821dfcb6834f7e5d06298c56b302d94b81b4fd43815fa665fefaa8555" },
                { "uk", "2f762a25999c3b7d197738e6f70c2c3a28c74307c455b15937794d38bd6b7f05765b3bb032d91e73a7abc94ef8dddd7672fd825824e14f7c31fc64f79e12ec55" },
                { "ur", "d48b3984a8d7b8b8c87bf9674c49b20fb20dc0cac0902f361a552562c05f92c4da998a0bfb5fd312e3586f4988bf63a88fddf7a7eb21bdda0829efe9b1761ec0" },
                { "uz", "509da9d86b633a9bb35c260080248a362c7df025d98ea85f990e76022dcd7024568c8b04f8cd8404e6be3528fdd87910a3e0de25c3a074a23d902eee32d8f750" },
                { "vi", "d5b97357b8b39d78f3c479c69ba233ceedbe03027ed7cd13edd4ddb13deb93690d9a587a0267e7d9a9ff6efb62de285e966bd829e017df434c670fe6f386b9db" },
                { "xh", "2df8c890c6427ac3495172b673dd9b4e5b869e3dfdeb223f782355a0919eb93e2a630e44b11bb27af015bf9245f04af070301dab2e1f82d66ee2ed9ff9e46016" },
                { "zh-CN", "bce02a77b6d8f25b4c0b3efe0a2ae90487bbd707bbd0acd26944d5380b31cecac8f5c73d0d3d777fe105f54d17caeeb40107825db4a410918b3edeb5aae1d2b6" },
                { "zh-TW", "73139e433cc6a20c0b89ebac8639891c86332b0d8637d0013b436eca34feea66b711b17c621f04d43410351219fecaaa6770a18512b182c6c4085707bf0d5e76" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/130.0b8/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "04e2aea7fdd29ed4e4eefa1fda2c3073d3419caea4540d55d10123624ca70f16376209028373bde5e9ba45db61e69f145c1ad8d0bb492b4bc1e3b5ddaa7705ff" },
                { "af", "b2d91498c69c762b794744e58779fa4ba228d800f1823392d00078835fcef7e7c52e69758e47e647e30bb4f6c96687ecc91d060f76e93e7887b6e3963dee1f76" },
                { "an", "3def41928f9a988dc6c626d06bb3bd6832794499724f81b44709bca50b4cd46165db4771285d82ea55fc26fb94e7d7a4d88fe859fb8ce5e5b3b594a2922574b0" },
                { "ar", "6ebebb6a77bf38ec64a8ae87f86b7e12bde25fdbce78fb1be993053087fb246c5b9da18675ae52a15f40912b6858bc1fa4d230d448ad0aea266faaaeb285fdd6" },
                { "ast", "50eea0f243082c59e2a7cd5b851a1f747a967de285b335ae7178a9c0db37fab24ca33673b29355ac19480d470ff99a4b888be77e3f175c488c89805d4f3c0339" },
                { "az", "84a19f36c9d3230c5dea40b1f6618f137037d0b029b71fa8e906d17d62de30cfdf1ed3ad240d223534de32c4224b008f248f809a6760b3e010889b6d4cfc2678" },
                { "be", "a639bc11ee3043081a002d3e3a7e2c4b0622ff0f6f007c796aac1a6433022af195428c584a11046f5e4d558330eed8d8ec89250d5a2a6819411e000d2ff057f0" },
                { "bg", "ce7a9a393b7136e79e5aa74eb496139ad99d7dedbaad65284a8e2431f407ce8097b5246cd04ff1ebcd5f9c9774a1823176077463053d6928a76fd5733b1e9f5a" },
                { "bn", "5cae745ce251c68424e901165f67fab2d36d9065ff2aeceabff3ea1498f017c313f618e214d197065229016e889c753fda2625e30af499fd4de086a44db2ee20" },
                { "br", "b63321e7bedb1865ddf867ff6dc480df61207c73dc424cb6526a804c68b463ef11bf0c026bbdab69b7a6a8a878b8a920b80f3e7406fb7319174e5273f9811cb4" },
                { "bs", "387ae6163a7649c17716e1fc9c80b52a9435906b335b85abdb6fec851286fbc4ee65bb26909ae817f2e8d3ea283b9875897ff24843cf5cc374d6fcc85a3846c5" },
                { "ca", "d128d1e48b944bfff9c59b21ae886cce801915a209d80270abb31051494c0a0deb1ccc6c6641f91d254bb4f0c764b1e6afcb60d0afc13a9040fa619b0244a862" },
                { "cak", "db3177a3ac1a6d483d21a90cad24d737f87eee3d2bf8157982a7314525c91a77399fd913134df9df4854fd4c592766bb0c331a2d507ac6c164ce8e8d5729ea12" },
                { "cs", "1aa7a68e7246e3afcf51ec5b0a29fc07dd229253b80c21f51a3c3efd13ae60c9ba476b6ffae5d81ea6da605804a69d592ab893b78713dbdf26d650d345b89d17" },
                { "cy", "4e2b98a7972a03a235626e8afc87ca961096811a1431a68cb0f83f2c177a2758512d16b4273ffffca1b15975b928feb6a2643f36dbb306ebad4f62c030fe3be0" },
                { "da", "b50c05be0fa01e9f7574ba2fd28ac6629393bbc8f0aef679a5a5a28675cdaeeb701e04d01566c57a7dc8ccb01bb80e9dde1c58729a8c753bced31d52727d0c93" },
                { "de", "b2cd5d7f211d0536bc0c052711ee21581042ba9399616ddb9a9f27b29f0b59a65502314f159bf6027e81148c7b88bb1351ca30105eb790507286fd5a5885728f" },
                { "dsb", "5d8b1eddbdf3b94ab8d2b94f5b252550758eca446dc4732fd8e3c34db53e1f710f0d78cd24574e32cff101a28bf7236fc344a793e0e1799d1ea494e75b72c12f" },
                { "el", "3f770b2f617c9c0d3d654172ab554141fac462d7edbc8c428d1363797a783c25decebfac4638265882a9a44cb660b78b29c40ba31a11067354adccebd61a7fbd" },
                { "en-CA", "0f47aed3b6c7702b0cf015f0d11b2c617c53aac9537b367b58147cbea672cc925af10b2062d6b655711d6788123144d180c174860e621560776f93453620ea62" },
                { "en-GB", "0ce0e9338a5325bc520026b4b458202195d1761af0d88a07c53f324d83e7432b7984b3ab8335aaaeab81c82c955d6994b2db6c3da0d6c0ef70295700f962eee8" },
                { "en-US", "3fb9cf74f30d63c017155336053c7769d8cb9a6911c917b791576a804224dea333e0e1bb5bb5ddf7228e4e9dd76ad2bbe18613b11b6a1b40e27771fe51e96c93" },
                { "eo", "cf6018e49a29ff45af9ce62690d086587247f90501d89417e9e6146033e5e95769c49967413fc368a6eacc476c5523dc6911c918a1bb4c00526c984be15c9e09" },
                { "es-AR", "08eba35586378774701cc4a20ffcc2fea19e6ea977c71f555b9f6865928b49a0a1f3cbf7b8ca27522f45a25a19dc1fae2d238114e8c2cd4a9adefb80e53ca2f0" },
                { "es-CL", "f4d309c46cdb7c78c2a3fa2ac6f56fc90fcdb267e9baa6fad73ef97f9d1934c112edd0b6a6ac14291c962ce652e427558468cb66636505feb8a89dfd2220c0c7" },
                { "es-ES", "53963de34bbeb7f870d8bc03cee6d6caf4865c6b1805fc0a952d3404058e01933e7f045d591dfe36fd322b2c9eef8f9c9d06e26bbab25b6f78d4c6cd4df5be10" },
                { "es-MX", "8acf189703bf79464d1ae88320ac9c43641513f3015e3f6f13dee3f8ac3376aabd0a56972d7dbb1b3b9c673841e959e3875534b40ba3b2c8b1a04768e7404317" },
                { "et", "4cacab87dc7dae209a724189d09cd3f908a158e463eb728bbed9b0d2e21673c42d27baa56f6d484bedc8b11f6b33f016425d79e926c514d92f3c64a6b39f6993" },
                { "eu", "08d5709fa1d7500a02824810bb18d500b5d91a4036dae3fcef92d5192316f2bae59cde1dd53f50c9dea626cb50b2ab6dee7285da92682c19c759f63b6e17c882" },
                { "fa", "a6469ec05544ebc50f4b5354eaf996e917339d57ecc65b47b1a34d3a25b34300b6778a41f2e91a0128bdc8e2a929d8e81ebf743a2ff735234cf32fcb93038a17" },
                { "ff", "24ce3adc369867e32a78db25f9dcd1015a19782686e766098abf961d1115a6d5ec7a2ac36be9e4f815856c46ab6fd53dcf0ef7f5c5633e3caacc8bb6f25c7e87" },
                { "fi", "1f90a5b28c0e5eaa69e54ccebdb8e7943a26a6178fce4e0854742c7e77e13885135d66b25180c3778868692667d59c65f5897ea773de37838ade94411658a9e4" },
                { "fr", "5a44838a2828325e690df4d34cdecefb5679bd526194ddba500ce4f660a1a3a4a1954e03f515561a05bc0c2f865358cd0c3e1285f92c8165835b6749de147474" },
                { "fur", "6ab3d06583462c4238274dc4dc3f21da5a2a41942b5bee4a07a5d075ee26bc81027edf868010d8c8dafbf068b960734b4a17d7907cf5829bd5985dafe3161123" },
                { "fy-NL", "aa3bf1ede9873917559ccea07a3e53d1ca266d153ef4fb55cfcd39e6b60882bd2c7c5ab8f105d5ee4b58ff0c265ddc8616ae8174bbd3edae6dcae3cf31463390" },
                { "ga-IE", "2a38c261c96bd51b8cbd4c5fb77aa50ebb5388e4ed9b1b29193cbd9f36c485ad989bb34eb5c54c24101fb147586542a404eafa0056c08e2cc67df26f8a5b52e1" },
                { "gd", "b6865d62a792005ee4174b1f81412ffb95bb098baf9e8b8dec1c8e0bfe1c147e4ede8b69882b85872c5202ca7bb9078c0d4b55efdd4f1664bf684ca68039f1a9" },
                { "gl", "b29cbeec6699c309779ea351d1f4f8e5457be9d3148c6d3bbf6038eb2a427f1e71f073b95bf906dc1764fbc8ccd1b03681856449b5b9990854d71ce2e6ec6cf3" },
                { "gn", "9436e9063f8f09cf439a18e50cbbbadc5c9d27f4948a19099455a05dec10c5125b4f12d7fcd7b58a8a03267c1f6f8291f7eecb6f690088016acca371f16f5e21" },
                { "gu-IN", "2880053ef23cdd283327afe9d194b3f93470d60f46b9edf31bcfabc89879d2887a7d45b1466d74e1644a102b1df46cf249145fbcc35b6a624b3de165d3c89bdc" },
                { "he", "8e0deea618a759e3b5475468418b101336a55f4294afa2b87366014963dd59d9921313a6991458d6edbe938c254ce6fa9b931a2e4c4759382a9f01b435c2c083" },
                { "hi-IN", "4195f0f7753d89ef70ed3ad83c12063c899ed37d293b8522d18a8c2d9cc337e0fc2b2f766313623dc33177ee9d80323c9c23ab285509a9483ee2e159f5a93258" },
                { "hr", "e79e32185229f62156598696e0035365122a280859e2d841dfd1d0968e7c9bbd260e03e1859f5544a183d2cc82fc543d0712893644875295774aa4b3f3cc8a8e" },
                { "hsb", "c1ade33fecf583671fc3b9fd5263d977150009251634489614c5ca6878b30cb46fe8ee934f63a6106d495e963b75a231a53914ad66aa78e81507e87fa624edf0" },
                { "hu", "a723a9b086f2393e8aab075598126d4e6064303717143daee4724fc67b9196c6340a02fa6741bc3e5783b530f0350a2fecefa42be94d9352c1cdc0f54fdee22a" },
                { "hy-AM", "c23adb39f94619f172debfff1396190623d8f5b4277f05d7edf60877339206ace9fd7bb9f005435602c2fe26f2bd7ce3743d49bd6ae1dd0fab29455dc0a75150" },
                { "ia", "d5a3c2f3f7638df171c192f7e617078b265833678a8fc83bee7f67541473bf28ab023caf6b1085e12e7e7b78e55bcbd78ffbd8fa1df22a31ad93394c8bb6ad4d" },
                { "id", "8a4f1e3f74dadb66e1a14028f1a145e4a9c5ca267b98c8785466231ab6753df46828c34da5e33eea88241d76ef1ad5c4cc539a42542ae7158d2f3e62e594b47a" },
                { "is", "5865fdbe4f624aa382b15f879c75c9d6107a09a693730ec9ddc0c4f0b8c31246435529e3ebfbd5fb1c4b87321884dfbbaebb6b55ff43972df635e8bbb6e10693" },
                { "it", "61ee3225a1244ae8512c26621beff140584416155206e03e97e7e01a8c72de4b71e6250dce2de77b07434cf63442224ccef33f1073bd3edc26e435697a3e6a02" },
                { "ja", "c8c8ca6ecb7b44d581fecc0d8e4eb3fb050a8227584ac17a39d4bc7aee9d7d874f3368dbcc3a7046c80d469ffca27d1f753b6be6bb632e3c78e343a234a5fd0d" },
                { "ka", "b0b89ee5abd7af2226f11fbedfca7d5e748a960d548fe8b7cbcfaa038acea644a77b10425e5c58c9548e5d2171f833fd3bdcfb6ac64825b212e9984450183225" },
                { "kab", "69a0b1fa11e53bccbd43e560a45be6d3ca8a77da733a988c9b87bf5a461449b6b85f690b0e0e1f847488b8f7cd20b53ea86c6112d9ecc738a14c81daf3297e8c" },
                { "kk", "840b9d14c2b85c3f9de9cb7860b032b71bba6939da7dc488db32e528fdfe04d43f87ed58ce7ca505f182969fd205c138249a255b2950f2d4f8f7a0ce974e6391" },
                { "km", "98cb61b755d8cc5727d41983e108a488ffa73de8a0c48c8e5e0da4d207a0e4aef9de9aa6df70b50213875df33b3ee5386e954c640a2bf233209b5818b57f75b7" },
                { "kn", "43691852037f3c0691f172ca6eb87e5e4183e9ab808dbfca20c2a4de44b60039de2cb3a5205f99306006f9376896b797d32aaffd16304853651259877a29a5f5" },
                { "ko", "40bd7dc8a7c781a02c925ae85522819ea80dc1eb2a6a21260ccf309804c832f15bb784a0c018833234987882df4c834a1430d8fde0aa669d2def51d22c1e2ea9" },
                { "lij", "2d39f434494416173e3dbce8a450c76629e1692f8adb95631b531967b13319ae80f1d1f1e6c2c68c30b89dd88264b7870f6c42255eace9bc532bf87e84ec1a28" },
                { "lt", "6ce3e5faf75bd5c056b7b58c3abaca2151704f2d0d456158ef4225eff482d2dd2bcba8ba26597bb8792c50a0ce9d2f69f33c7f2a9419eca3919ac808fbbefcc2" },
                { "lv", "0a8bf7bccaea66195978833d02726e3fb6ab985cd2defca69c4001c35405c28ca0719f06f1952cbd6b1489b45111eacb5efd00b06902f407b5f2c761ee49bbe3" },
                { "mk", "a4c1b841b542cb9ad3d99946720b2393da7aea852a6713ea853857205826d66b3686aa9ba6c014d011d810378d1580e3bb06fbd09ef1f303fb3465fae86704ed" },
                { "mr", "8c2ccf62990847b8c1ca0b1ac963b3302c1dd4e259297778443b0e4ba6a7e42d73621c28bb2a01c647a18e600fd87395e7c01b48fc9c30147ad30c22c94e904e" },
                { "ms", "675b2ecac8c7db55ed971e36042cdb50efea3db2f182b9d676d4a4bcf8e796dc632718e1e628fdece43073691d624137e3246260b520cc89fe85160dd6da908a" },
                { "my", "64f1fa0ac0686245e3e968bd43454d1fd745586fd62e92ae5045192c4a6f329c0a67478273e66b4f8fa086c17ec63c2113ee1fb09f630310463d9ea95ad70f00" },
                { "nb-NO", "cb3efb03b9cfe1661ea3b272d1df9c172c73964b4a9d13c497b187d69cb5f219357cf1fdf793216ef10875364951d9f95d2c72eb85ef0614f5e6909f2594941a" },
                { "ne-NP", "56025436d15f224ec9ecade0b85db57af7c7f71f91fb7a8076f6ceb5dbfbba96a97e3bbf342ffe68ecb3ee24c1b546727c9a4a74d6f41c62d88be06d207e9dbb" },
                { "nl", "4b1ae73275db8e7c84a6794d33dddfe8c9a631de8d8b8fb6a5f3626203a54a59b0ac3ff03c0473e9d96af8465e727c07f14161df634e64557baf4e4ff9bdc65f" },
                { "nn-NO", "4f3cd1eda87f6bf7a1952d9db0ffb66cbe98601c1ec810c2b49618270897fc941a56aef8e5f97ac99d6978544710ba9e7470d1ca4d5a3a33ec2e6446991bb3e8" },
                { "oc", "c68bf8a67239b7c2fabd40e36e07a407af59da7bffdfab29e1f0fed85012c70e0871531fc252c0c0b9a1bfcd6fa7dca833289a055396164062c707f7ae8c1a81" },
                { "pa-IN", "310471d1f6a44f7c580b3ff2a1aec43450939e6c210f0e52ef87aa10d91945a5f30e518ca595cdd4bfc488060cb9d395d53d89af64cbbddb1b5ff283f6d7c07f" },
                { "pl", "c4adc0a61512781539df66641b3243ced66176cfdb6c2ffe2ccbec3e0a09e82a2ac225c9aa92b3f4175b22798b508e6ee78b4e7294c1eb97178ace0c4d62efd1" },
                { "pt-BR", "a4472097760d584149f640ce6eebe0b073db6c04cf8d52acb99b1d85a7c650b59f162ded3a3aecc7d7159695528f76ad781d2beb7ca5cd45ee7d7af744bd7137" },
                { "pt-PT", "e437683c3b2f0986a8b2ef9f3d147daf7b63bfe229625c317051daa97db492cb9be2dfae0cbe36bc043759e9ddf8d214ef47aad8e64adf33a270a6671ccd7f70" },
                { "rm", "934a8950fa336cb2fe02d0d99d39b8ce61c8cddf4567201fe3387f1eee4ac59b9f15a075d3aca0044207b872ed133b0cdc9a23ac4e2ffb064e0b22124018fab0" },
                { "ro", "0c7e2b02be9b67c869d2556f8384b1196db3f196c6cd48cfaf61ed24ed5aadd85e217cdee4be8476471214b6d28ebaa4d1d66427c91739d8f9966d4a6605cf78" },
                { "ru", "ef0032748e88a27c1d1633f86c019d840551d91ab606c5d8e57524264c68d5fc4ae57d5b2a531325db97cd3cb7f077f0f31fc9096bcd34a8688f70453aae7914" },
                { "sat", "69162d1d7e2a83703c43995572fc6d219b81eaafec838eb412df736bde2c2a219fe83c4a3c30836aae2087910bbe9fe1ba8f435cbf120645c0540c35a94a48fe" },
                { "sc", "ac5f50a99533561b16ee882f2e5effe716b09e10f95e73886dd7ab61247db52350489f876500d914e30f9ef8031046a5d0834c7f1ad9c46ed02c2a8c2c46d16b" },
                { "sco", "8e20ee51682dbff9afe0bb70c573df2588de42210f7a681fbea718a3eccea43386b7731ae3acb3e9da6295a631f4cd8ebb6ce4013092e95fccc05457c23fb923" },
                { "si", "f26eaf40e497f2341af01da9fb91e85f3c35ec13c7359836e7dead6347e61c5a591ce012dc00c1a34a1f127ad5b5e72b0cfc25ebd8df24a027347287db16cb96" },
                { "sk", "cd1a72afb5fb8e7b03bc5d46388e04352070fc0c8c7430c7804634b8fe7e852538abba692899964e395cc1d34de09442181e8cc64baa3d2eeb410803c0c20be2" },
                { "skr", "bdf9d1974d0aa812c7f80ede6fd1ccae6fe2afba2e3f8300c0cb2613296c36ef2fade699a50200e523bc28ca8084057ae21750e6eededc62e3b4290a0c1f8449" },
                { "sl", "2e355d4f81690fb4d19ffcd426fe19c560475602346627a0415a5a09ec5d2fca317e31edd6ad4007b584f94b18dc80daa018a8b5199b385564400c8f313a0dcb" },
                { "son", "548237eb91a6c200323cf794e38d7e3e027daaebc0ec506ee3bca02c355c9642b42b2e09241447f63afadd96497427b464f4c560caa3a904aa183ffb48b3089e" },
                { "sq", "3a3bca91a9ca7d4b2687b86f319ed1e150680cd8830532208d7792f401667f630d6c7220a7bc0a33a82ba1b0bc6d4829fc995aca4b1d5f1753a790e442ae95f9" },
                { "sr", "38ddcef6a03759208eb7f20f0320246e1ed72db2c05b2c7fe698160a17f2b41d0b79e0921c0d7d8ce9b0f0a95e14dd8c0cb5870c310ef3f229624de17e23b6fc" },
                { "sv-SE", "5752ce8a167d265c4180506f9779e76475ce8e01741aee70c1d40fa5ba8e2a5993b58346085513ded185d15cbbc7d47495c504ab2cf4f03d69a6961ceff68622" },
                { "szl", "7e49490a620b63c8913c40b3fb81bd7b42b2456b852307088b6c0f55c9a86e58134696d804ea29bd948201cb581b945241ed0c74e7f79c85f3a73f85d3db7ed7" },
                { "ta", "775378206b0194892dfa859dae91f3b111f4182d2b353bcc54df7a5d5497e77263a9bcedcb7d412b52e17052ca1eefe36706c1d6b985497158eca17b9edf97de" },
                { "te", "eafad302dd4ee0a48a3efa3884f15531839d8a1cf42242f1a3737e6125daafa97943d59cff3825cb0501d4bd85cc7af0a179deb657bfa4258af45cfded3097ae" },
                { "tg", "56e473fdb52e53b071820b1069eb3c72465cea2e67cbcba7a01f0708e8d85f553ca648c80616db1fd5d001962d7b74e1b4b2843cf3429af612697598551d01aa" },
                { "th", "970e5ba6429c291beda8952514b834d2c725de2f8a5bdd3485fa3a0d24daa425823e288f81752292b28f6fdd9e8aa0a0445fe4149c1c2703149c8fc6f01d73cc" },
                { "tl", "ae9130806b4dae5279e1bdf322f43c710eb9a6fc1781a5ca63e00a361ca6849f5038a55f88254ce40b39332d8f25b9e5ea7c55593a7b2754d12b7778ddc44d9b" },
                { "tr", "64e11cb49859f4dc0349f21b8d808b5c06228545ceb21d0dd2de77124f09df9219f9174318def387e47977be0231c3d1b8b41a117b9c8a28427ee5bc362e62c4" },
                { "trs", "e0dbc7a3a6ec2284e88f5cdee9fd84766e7c8a3f78b38043b51c0925a5602ec23da4514faac25a8b11bb9bdfed0457a850fb8bb6e14b065b71354648120d23d5" },
                { "uk", "ac8eab187e2e1cf9607b1b9a844999ae06c152ce7b7a064903e1109639bc510b4d70c850034a080cd699f05b2f856fef5cebf1d30974ebb6cfc2b34625ee599e" },
                { "ur", "a20452f2352af34ec74c7e64178f472424d7e971fd05a7cb0cb184b91010927593b1d418bff417ca03648cd9e5ed0b3d2c528e7a4906b06018b438a2d5237bd0" },
                { "uz", "66723150bd8322558f1851e3be52c2d87f548aa16c48c0edd97d8626366b3504e01e145df0d84a2bfb79fe2cea13308a755930e94a8d888bf39b72d8f4bae476" },
                { "vi", "40bd3345c49e90580ae74fbe0d34a1ea3dd1db8abc59b2b011da5eee5d5b8573edf57ebe6fbc404add1ef65b353e11c066940a765cdb29053dfe3373b8f09f1d" },
                { "xh", "3c62577d8d0ba40eb2b759a468d67d880853c0e66c9bb22e110bbc07466f61645be01c404739ac7264fc7362c0668c2ab745374f3a83d6e0e579a9580baeb63d" },
                { "zh-CN", "85b75d76088f40349a8df606355bae8d9adf49666d5b8bd8c7e756a915ddc9acc7c4d214872a0d25926451e766e213c55d6b6887582628639aea0bd9b51eb55b" },
                { "zh-TW", "8fb22fcbf9e878a3a987b60448d7c68e2f439883a8e5d90c987973ff2e58b9d3f69454f85910dee72f8ba2c201bbbbac3b0d27bd40d0a199690232f531efd31a" }
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
                // 32-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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
                    // look for lines with language code and version for 32-bit
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
                    // look for line with the correct language code and version for 64-bit
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
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32-bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64-bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
