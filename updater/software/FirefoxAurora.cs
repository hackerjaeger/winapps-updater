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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "127.0b1";

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
            // https://ftp.mozilla.org/pub/devedition/releases/127.0b1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "37dfbe75629829b48f4b5b38690d8f1b337c44b002721ab6f9fe43d7c2c113d3c110a0c418132ad8f23a13e7fe9b6fafa30904d422c414e63cab75849a51251b" },
                { "af", "fed4688872ac08d32bdd202882307c4e4a3115687edfd884756c6583fb6a1bdf209ce1436b38b1dd5b9ca6bb663ddca70752ab613061a1fd40a046cab2b3f917" },
                { "an", "85f7e3997b5e73c9c4ec5a5140df6824863d91ba18044b16f4739c44c5f8c1fc1423ce980a9d1ee6a806706c41da2fe66d8f28cd77fb3958ae8d2fa2a6766c4a" },
                { "ar", "5cc96c2a73660b125381e210d3fa25683ce6156316a7a1e01396f39882fb2c4f7205209f4d0ee6305f9d81b713e63c9b96b76e6f9b84498d03b77b2e4effd846" },
                { "ast", "73ce44a1b161e924f0478673ae4a4311b901bc0857c7e7852ca2181512a824107e3c8915826febe63404afed82ff413780905a062c1058d01e00a955ac4fbba7" },
                { "az", "78019e40793af78f74cead05991eed064bdd0aaea98bcd6f505915644e535b3d0948e949adaf83a50bdb0c3263ab1fe17924a5ece6657ffad8bc23ae3c614016" },
                { "be", "d27d4e14648239ceea12a328d13b799487266faddc31318f1a552c154473f0450212230854d3e2cb716ee781168d10ee004300069f2d54a1645d0dfab3a7a503" },
                { "bg", "296a29af300e66721d95b5fd41cebc2cc6b48584f252e40e8828a48df6a2af2d439e1769986b7a2d6097259c0dc07b3f9bf11613baba6d4e6df6decc58fa2bd2" },
                { "bn", "e30cad2a4fda22f8be15011e06311bad14e04658a0bae3d65475753e32b4b4952f41c45140b8ece2b1cd738d12ae5d65795b6649bf3266c3ee02fc404ed38bbf" },
                { "br", "0d463eac36dba67d5746c35c7899153c6506c88bab8f410d54c0ddb2343ac376c308d20bffb010224fec78189edfca5b4f9abe3c2f79e8881862bf7d84da413f" },
                { "bs", "5918463e572b8b150d5dbea3a27f123f6de63cdcf6ffed57c1ef9d2c126dcf90c7c41568659b7c3726628c9842dea80f562b2f1d899907bcb975cb2d873bc25a" },
                { "ca", "7762a369cab5963dc3f56815062f176917fee12ffe3c305dddaf82e6142637358690dc9886ef95f6a3f20df586e2f07c76e5309df19114354fa34a9fb6e40fee" },
                { "cak", "433eb78b442240996b7f5c6a2b76594a0e06bdfa1d0821a51511397fb9e4cf99ce4db7c9eb5a483306bfaeb6c4614939eafcb57772c58517798780a18cf9eb80" },
                { "cs", "7d09f93dd3bd86b390f4ef726517702fdee046e989ce85f962bb031e604c095dc1f33302f6af9b616cf5704f4ea37c98f71a5bd5a41b06159a11e43281aef65f" },
                { "cy", "aab297f84a5852808ccb456d4682ccdf3c1a6b26b304efc4a1ab9a0a2d634116dfb9e2539055d2238b76fc76238c92b852e281b43c4113300d28ce168501481d" },
                { "da", "bdf4d184cb9c31d23619aebf77e4bbea702bedab4d108723f66d988bb2a33de8af82cd229b184bbeae28781ab9929698b95478f4ab5b6380192e9207ab8f27e6" },
                { "de", "c2682219052cc5599f481eca82a853ddcf63db2c7c8d79daf8ca630b8eba75e439a3a2e8bea826e6707212954ce91a7b49d0c3684fd8a7b9cae97b5d8dd6c52b" },
                { "dsb", "96a3ae1e02a11a7edcbd4457d951fb1ac988fdb075d6ca5b03bfbfcd3dddf111117c5cc6832326888f5c11cbfbc1dc49d576d1aa496c954ab1de9bbc65477ab2" },
                { "el", "2b5a52f924d1550f4f883d9d166a64a885c29db38d58b5e97229589d4f5b1124e3e7112d7ad0900742fc1032c9a21fa4ed9cd813d81e1b3d713d7a7c1a947ab6" },
                { "en-CA", "f6df339acbde543d7f1482d50035a61422adca451d4647cea7de7a5174bce4c0e131e5fb01d0f9b46af6769f05d8adab5dedda50fe98b8e109ddab17d5b096ca" },
                { "en-GB", "409977444cec423ce7d9a75da73ee65d2719d6b721cd6cd99ff4d7233c1a16fa16127213d542b1cb6e71c29077556bd52d9632b18cc2a544c23d21b85d8526ab" },
                { "en-US", "6935757b936e36ab6654b8cf582f5ef78cccc1c5de20d4cb50015b0829a98fa6972f02fa69288933fe05272bf270ddbaa57d8743eeb5b54341c4b2d138538707" },
                { "eo", "93168982ebc0e74bb5c330000f43745cef5f54b8e2462b34b63c5fce823477912082864a569dd0d532362b3bedb1394a4ded4924790d16f67677e9a892dcfc78" },
                { "es-AR", "9ed2200b5363d9bfacf2f12f461c827ba34555bb86c7b5048e2935c202de4fa518ccc92afeea259bbd9d15dd5e2c6f8f2eaf3e98fb35ecbf6f9ce7f1b877f508" },
                { "es-CL", "653d3deaefa79439fc3a56222d60940397711ce59761286e9d72e833993f638a551aa1598fbea00a884c25d9df23286d9feed38c0b82182b03b1b059657ea9fa" },
                { "es-ES", "2050e7f4ffb6d60345d4beb67d766d8ab1cd2320c9e9a1ddc9134b03365fa1a389b1b49bfc4844d8ff458eacb84ccf1fa81863bcb376cd2488defd83c599e540" },
                { "es-MX", "4580ff849f291bad53caee06bbe3117ca020ebdc685f264c841bbb07bed9781ad135fbaec189671865df26c3f1367dbe71c3e9fa512c9c3421cf3c94227725c1" },
                { "et", "1c48ab3c133638d5f5138e7490e3a4a6e58b0ffceeee54e3b438bf7dffe40085d4807cf607f8762f68b9cdd36def3460bffd0d7ba88dbadd7d15906f27376385" },
                { "eu", "9a761900c92ceb489d57f42fb0aa07ac9c76a9f3893090028dba02ecdd5b03763224fbb5f4170058f98c2bd9babafdead406a3f525108866919deec8ee39cd3e" },
                { "fa", "87d4dff531b94c8b4716266d06538dc136a5ce3454a94a9e17bf43af359add5942bac11dc4189bbc4ca7af211bc61114cb2c8eb2a009d3f330438937f19877e4" },
                { "ff", "5499793d01e6cc345c72164fb45c773e3904c94c0dde94b3c13c944f88a38bd189c4091908274d2e8aa3f14ae9e728116bf1ddb40ad7add711795a2fedb37776" },
                { "fi", "e62c3545f43d556f7b9f949d6e02dd8a94d18078172f9221ef6ea6e66e5efdd2259c38bc7fbfc978e76e547c2d451f64907f846bd68c6c574449da573032a1d5" },
                { "fr", "7f01ff39afe846daa7af621497a5abda5ce3679f54b48fed1aac3e906544141dd03425e1521396dde9add164494994bd1c3466752b79d30b683c535925860e0e" },
                { "fur", "0277ffeeaf09a3a88904615b00e0d8fd3caf90706096e152462b898c250f663bbd7b36ddb17aed3bdee646b5597e09f0af2c587810de832c0ccd3f685fbc4078" },
                { "fy-NL", "14d02f9b98ce238b5b2c605bf191294a90f4d8d4e9cb797d8024b44b13aca69984125b8a45f492d6efeabaeb4cc90857a009c35290ce3bfc7936adf226d75d8e" },
                { "ga-IE", "78db81ffbb84d3c98760cf7d6b7c5185381dc571027b54b31e51255dc3bd82894ec763cffd4b49eb5b7b12bc3b2ed7d6e7b4f3682e8b348226d876ac84048c49" },
                { "gd", "385229bbf5ad73ed2bc4efd483656247c363f49113557d1543986cdfc5a14a7dfe0345f0db22e18aa47dadb56edcd25e33b165de47ef58c2004ee93ad674254c" },
                { "gl", "c8f918b06b11724d29354ca4dd895a6a9a193b5a174d9669a9dd9b4a08dc6882fc7e10977ec249359e32f4635795b45e75740a0d768a7c5e4765a2216afc99b5" },
                { "gn", "a58d776391a7ba46f033deea5122b97fbac068a311bb2953e4aa5fbd4df5304b7657369c06dfbcdd49e45ceaf4c5a813e4dfe4b247b4f1999cdb196405ef2da7" },
                { "gu-IN", "2e19e3037f9dad03297743735880f3f5481de1b7ea6803fdec01dd501f6732791bdc8cbae7d99d9b29195f8877fae5c69b267bd84604c96d0b940987f79afc33" },
                { "he", "894d1a635e700f48826fc40894867a2ada5b2261fa592bc0cee4020c2f9e9a9e5485a73cd0b23e181f7b9c94c620f47c72350e466edd1f7c373b16b678b0c732" },
                { "hi-IN", "2c13ddedc6dc5865d461f152954b891cf246eab658c775d84b6d42f3ba92a657f5444575727c029bf0669d3e6e8c1c946eede033e3d17c20b8707124c991937e" },
                { "hr", "6afbebb43fb759d6140769a31657760d886fbad21b1dbb13207d74013c737688846abbb1abc9ae1e4cb0231d1acf5025b94b97283e3214ed4de10419493e3ced" },
                { "hsb", "777bb8810981a97e39f4e0c110a7840af28b812b22d32fb8c264efdfd24deb034b593768d51d53c3baf639706cd426210ecbc57068a7ae49a853ff82bdc3408b" },
                { "hu", "d36be5656f9df3fc698cd37fa548137d804a33e0f0ee92f31c16015a73c14889d96c9cce76a89a92092596e081697c957f04e8e058c9bf38c3f5f0cb465e91af" },
                { "hy-AM", "e223083c46e1320a2e790cd560350a0792aa994bc37d3b122b126fd736b385f23c66997c78f95632cc451f1f42511561fdff1762e4ada5305926f18a7caceca3" },
                { "ia", "f8a664cb686d23a5195660d71fc3c63a472046573b2bef7cd972ec6ceb68a4ccb0827ce53ba15db167423113a20cf37eeea6d3dddd81dc880783e81b18a05949" },
                { "id", "de1b939afd0be83641ed6df98b3136c256210909792fd91b53c961474b758f941c6b5178eb3a8249c59acd3415d7a2ed5d0aaf1cde2dd69114313b6f37f8b917" },
                { "is", "e30e861e155d8786ad056558dde42965b5345a778063431aa113f14d438d0b6e4ce88ef5ea8a24322a979d8d907c61a91cea27e2aa15bf1c38023a3ae22eb00e" },
                { "it", "4d6493ea14bf9950cf0e9d91ffe60089d93103800b1c3b4d181b37363f98b23c686729e1dcbdb4e9d25eb6700e69657479590ba5d118e375c55fda6582eaacef" },
                { "ja", "ea443cbd299b3fe21cd204e9cf1c93b9317c6a8c0d0462c7491a28f030d778c902ab18e03914126ac1dc46cd3d2e5e2a22a1565f8a530ea78dd186199826da3e" },
                { "ka", "06a6f767f92873138c809354f64c96507f713adba4f334b6b6d1cc83feba56f2d2b2cd7bac653ce9a86df445989028171fb60ec44adad37e457fa8ec83844183" },
                { "kab", "c28e692febe31a985a3b56f586e611f74a00672818e332fe8c4019b7373a53a96d41f7f07fb57e183ace0cad6bb948472d20c9de91b434790fa734af7a9fbeb0" },
                { "kk", "ef4a844619ef6f5dd844f825664862505e05090853fe37f9da02cb43545d88bb568cd9e9d68c83873ac4c121d66cc7b2c7073de1c2960d874b3c553c8c53b37d" },
                { "km", "a2b593e298f8273354d74df5a403feb547042637b8708ae0af2399626ffa4ca5161fd79d9d46e5419c61ff97840266350867ab56998a3e14c9118e439d75502a" },
                { "kn", "958bb62b77713a03146bcf24a0cf9bc812fd4f2647a92595d29d1cab40d148a645233d46ec95bcabe3d05e3801c4546d6885022c35dd62cfd0ea38353ccc13f8" },
                { "ko", "60bfcc1ba33555f0c87011c2cc042dfd02591244d09b1bf006fc9ecb85d30b73356109912b28d488faa56769e615890f024923434845674dd8584e38b5805379" },
                { "lij", "d1c1904ba5c28ede51dac9fe25adfa15bf1a2b8b6297dfbeb6d1d566bca0b3b9670952960a30972f59f78410ebf8e2bbbfb5dc6fc225f4199fedf13e9a61861a" },
                { "lt", "85ab018b89bf1fccee64ba60cf4b8ec85b45e19317b4c307f5e2a2251a0e9cd5f594f033ef834ccffd8be5bd0d16533ca4194c05491575fb488d6b7256cba846" },
                { "lv", "42d880bf8da3e876dbf838d2d52d55a72dd706b3ea66dbccda70242c12759e4dda6edf5ee870cb845e0a01d9b88a4e9dac6b2ff50842addb2aa34657ec023dfe" },
                { "mk", "67e2dd0e63abec662fedfe58b8b65b861a6d3d38e53022dfb4852aa38c17118801903a1610198d745b1923fa625c6692d7be10c887ba895164ae1df501b66af2" },
                { "mr", "3ff61e3eced8dd307162fdc561eaa01f90de4bac23d2ceb074147aafca7544e58d2ee84f94948a825900f0c015395fba1fbeb62d6a326cd4436ac66bf49ee57e" },
                { "ms", "cfa9e74c6020cb991a1871568629bcaf7c2966ad694a530b81378ce3fb89405576f474834794e4e7bab9cd8851894ab576a5bbf53c5dfda1f41ddb4f8c5a9bb9" },
                { "my", "e718e4a1790c6a9b34556a496d3ad6e130e7219f914e48d5add4a8573ae34fa56102b3f09bbd548bee8c60753762ddc28dbc941028fbff3b6db4b8477d7998f3" },
                { "nb-NO", "972693d3f973e454c9de70a5dd87fd2e3d19f4dff7429a36e5acbd5c7a6a16d7629a45b6fa6b6f52c5955db36f47f6be7aa4c0f19e828dce199cfadc040d1203" },
                { "ne-NP", "456fd0afa2aa0075a69995bc534d2622f97988afd8f259312845c70f6a9beaf4742a5cc6033cbc2200098088841a3644fde9b39db2cc7aa7b9f6b10542868b61" },
                { "nl", "e329606ddbca7a8bb85290ba1b29d235af0da6c9d6089d247e46f0a4344e022571d0f1879cd172967447030d4bc3afd739b9912fffcfd1369ecaacbdf1fb7c1d" },
                { "nn-NO", "82d72d64836bee086042e7351a31c6877680f59104cefa3f6719ffcce7138b7540c91f287c738b1e59966a7039465cf14007d012001ed363ef7e483d335ef337" },
                { "oc", "8ee8122074464a7b8d648264cb694c4313225bb8a2e31b287da9b934f769552e40b2401e8a5eb958c98d3790df160b0bbd993c7bfecc9ff74080b2484958a9fd" },
                { "pa-IN", "d33586ad59ce381e699bf3e73b81cb8bb5074903e22c9b4a679c7b5a2b6d92aeadcd2761576bc976818a9a5aaf0a6c6fc1318a29a6c4ffe15aa6b358d278ab6e" },
                { "pl", "a82a32cc7e30abbb77c95d7771ccfa1863d724c3795a3bfac495c1a6cd013c295d05e1c48f2e3e776432c35a2e4ab664b88a4ff9029f5a3f1e7bd1ca0050363a" },
                { "pt-BR", "4288147d7a856ecb07ec84389603e9455e98ae547682f4de510d6f37af712f020d96bcd2b816eaa0230ce447c6b6bcc13c560b287865c70752759ea2678a4379" },
                { "pt-PT", "e5eeeeb5e7736f491856a9df0a9b84006b325ab2fcdaf8079e2ce32f2cc19b753da09a8da0e912f060cd5a74d15b45077d5740860d4075d86723165dd160fd24" },
                { "rm", "b3c10845b66652ea56ceb655d02c1cee64b9118d943b4a2c1d608b4a1dcaf48ee121f70b83b927e456404a57b7bdbcef51715218010d194e8d0e1bddce616b89" },
                { "ro", "d98f18e89c0ae49f0e94108f03da4b6b6b3303e167498dcde72e0d8af12daea27e6bd937133eaddce2172c52a3e07dc198a97e09c4fe32a8c88648eb58a03372" },
                { "ru", "82365fe37fe58d6ca144bd241e06656c7eeaf0db3a1035107cf1601fab2cb2d4da65fbdf959457e26045b494347c714e3d0f261ae1fe318e6b50a53ee1c028cf" },
                { "sat", "f1b9e72650a5f98a6547ba3b71cc16413368478fd3cac8bd622301c6526a32853e2e26d86adb7a84a66fb8f31dc594d3f1e98e6b6a9570a1479fad4ad34a03d7" },
                { "sc", "bc51cedb9570fc3c090df9ebe9979135987c0714b5fc7ecbe80fcfb1a482ec11473769238c36f496db0448159150f809cf5c9120a78292faa4e22359d5f99413" },
                { "sco", "891806d6c108c42ba4fea02a41442496302e977906c1161fb019fea3c4f40b7fb621f05964d784c544a3e4dc156b00050da9a82778c2ceb171ed3b12599e4493" },
                { "si", "4778d4423906d5e3147d271f418da0de6012dd2c1e02a4861efdae015c61b4bc482571d2dba4d54a67a14c661505c066c564a37ec4d4b3bf46d0fac013657231" },
                { "sk", "cf8928ca5d9384d14c86204e0e52b36dd09af4afac5c8185ac8e4aacb2dae0dbe409725d61bb22cfc9917048975a845093332dbc9fc4e7c8cf3698e5517be0a8" },
                { "sl", "00f7482a6484e760b48b5794758e29d6a25f27d2e3494648ef06a8b5f2a6ca4016620ba1c4f13c0a6d6b5819df1a2d13a64958eec7c04d75afe3ebf73e398c5d" },
                { "son", "3155277da77a784494678a7d1e92f8f834be68da536fb07012531d7e655ff5da5dc14374878e88a48dfd2c7da444be76728af138a01ab26dc8a69ca89ec93ee4" },
                { "sq", "7d979f96fea050f32c5145d7a3e8d0e93a91b5f75fdd056987e4b6bbf9b9c9b603d42fcd28b0e34bc2aaf690b090ee5e3bd2e87585385db0458a8a968c450398" },
                { "sr", "425d3b359771a1a4c911226f0fa2822ff91c2ec17a42f182895a864b4692a6aadcf28e02282246ea422de40036a5d97a10cad9396a482204c36db46b2be78733" },
                { "sv-SE", "8b8cec8961cc4d9951df2ad41b20580f04cba1b3119fe20f588c99d1bac2a30de189896166386f48668cb83f4cdd774d51aafc845d79ae94df68b02776469bae" },
                { "szl", "6057eca47b62e999cfcdf1e82eed97f216bd88188f8ad6372885f1ad33a1a38424fcd873fa114dc6bcca0a4cc6a8e583c049838cd4ee197c6c50f6d53c171525" },
                { "ta", "47fb0c4cb2cf326fb720d8bee3c207419e556b9fe42aa7046cbd3369912ee17bff6323ed875f7be758ca6c26cdbd94c98f835e8cfa55f89868c747914c151cce" },
                { "te", "9e1f85d042bdc03860ebb9f38df8c60ec0aadf406487b68a2b3d1f5ef6e2ad8e5a1abf672673b131a553123e328254ee4b4e280bbd020a9cb58ce3eed931cf8c" },
                { "tg", "61cfd1410ba41bc2ad6637b2fcd048f583d5e10b18e1d4bc4af572f54bac0968c443b6a4e43aa6a179e391885eae79def62e030d23a6994926dea734a40ed440" },
                { "th", "e950a887d38c46014d6d5a8907f250958ac25265102486dc6a7b5d28701489e4de23320c96bbe9d29d73561facd93f4841052f5443d6845124ab67568f1a6fb4" },
                { "tl", "fa5cdbcbf06eb3cbccb5925ea66b0c8b45a00df90a0284fc6887ae39703162f2ace4bc4c31bebb83acdb78ea757b064ab2fc98edf92d85d9e7cfbe00954ca924" },
                { "tr", "5146ada69ea83478e3aa3fe20231673a411d9edc7d9f24e92803c967592abef38072c46b3b7c4073d2df276f2df8a1047017a369d90f0e40c5da2e1c73b7452f" },
                { "trs", "3c0b5652ad8c182f86777ec8c99c9b941262fc3b9d9ebd6a3ed28f142f5fdfac37f7130594b31734f7d902ad98310a53f5f820a91c8ecc1076c0b2fad6c03426" },
                { "uk", "0e21e15237fe72ae47dec9c07511c676c90d4c41ba3dac5eac58daaff0a2ff42d154f75bdc515c36797eff92c8c81bc91a6456c3cd41af1f6df6f1ef62b0ed09" },
                { "ur", "53c189895aeb7c760310466c658d661b54c77c480b9371cac19ad8130a31a500fbf5b58d6bd81b44493e5ea8e140d580f2b3669d53dc23f840a80783644c22bd" },
                { "uz", "02597e9385b810088adaed68b48a7cc27fa0508d26aa07c1ad6cfd1f10c063735ad6c503a619d887aa6656d2ba5332b8d28b5d0c27b162e5e855ea2440cdecb1" },
                { "vi", "59d2744b2f6494e4c021194728e9f692d75903373747eda87f7b10f81cb7828eaa92b5083e604e3ea4c9740c136def782676522e61858c61ad0539da377d71bc" },
                { "xh", "a04e28932c1c323d060e9185a60bdbb45fb86b5856e3c8322b25ee40d00f13b4c6d33ce1569c6e486de546f9bed9dd441fbe65c40f080f4f12d9f881c8b6074a" },
                { "zh-CN", "faf18d59c3056f74af5496b52bdd925334cb5fac7626b05612fdfbd829ba688ab228b46baa0b4e9826c3110bf67c1a1e795ddf0d5cec81a333c1458a0013ac29" },
                { "zh-TW", "b414c98d1e1a8a09f83dc9d4a1a7fa10fdb48cf26825188f39676505f68ecd24f41d1fe56cc8a16468718db2283c2055894172a23b5b04bc34342ed48d8f6236" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/127.0b1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "20225a27ad0900c6dcddc7c55950bad1dc84118f9438f28a26c0bd94473ddc370834fe58bc740dae0a9e5c084c7e87b8c8c874d6adc23702d530b032a36e952c" },
                { "af", "165f2c9e2ecb54855e09c0dfcb04ce9da4aa8afb6fb3e954a78bc2bd274f4570baf184db4888a551f7ff0ec2c5a729662b31bf457e08e28b504cc66e1e073e0c" },
                { "an", "79bb73a2ab3911b50c314d498032ce146f993a418873164a292f1dd2a65562f6df48aa4e73693c665d0c2ae7d3618e618e3bff59016d1d4434f8676c02192143" },
                { "ar", "39fbd94ed1061607caaeded781e97cb34a650c4089b3a5c48396e562975bd8d2a605ce582de52601e3ebfb693dfdf424dc29a737b13d30ff32a6ba96165e345e" },
                { "ast", "44b9780653a2d74fdf376a2ce4a586a797b72b4c5b00058f16d4e300305700ed6b756b56703310cb012da2fdd89337a903093a911d973c64bd38711f7bb8da84" },
                { "az", "5dc76c89b3e42c1ead0d00db21385bbb9cbd2d336524533f3d8ac1f0782d2bed3f4cc458157c42fcf9c828a7d36e41746bdabaef9df53942f997b84db75b2f32" },
                { "be", "8b52fbafb479d9aad7cd2d312682bd587cd514bfa8a3054b0c755e3eee3157f7bc94213582399bca0f9aa19e0e57c38c235b0d32b0291a4209163eca1fb71e7b" },
                { "bg", "4a2a73bcc6db703db99b91bc05dab2dcff18596b5d4f01560fad83c6788f7495f2f3c49e200bb7a1a66576c3550c412bbe7ee20574c851a09a878403e2381019" },
                { "bn", "104f5d64cc6d7370d8624bed3a62b81e26ce850b7aed2c0bc2707f2e0546bf4053949b893b750c7b179b7444dedabc231be7d21c40b15954f4705ccec0e89408" },
                { "br", "1cd4c44d1a3ee29f97d57df95e0ce0cdffd0869d7e93135eb531082f5ad97dba34db2c1f087e2d89fefa7591553bab43467812bf48d686855935e735cbe9418f" },
                { "bs", "b87a0939d92f28e03c5ef54ff296a8dfdfd2e7dd67beed79d9c1c865fb36c55c581ebd0fad3c353c686a1621153a1158e9bedca40401f273712f39c3db3ac915" },
                { "ca", "e82bfd7cf94f59a4e10b47cf4a347ae178eb7c21a667ec0177948588c5cc829bb60ff5a3eb2fcc031f777f3cea126e14b8b085a8340772380c4edbeba6bfdc3a" },
                { "cak", "b3385b1cf4c021bc541be87d466527243db5d64fa04b02bacbefd749585813fe671838cffc9d684d31080480281740daa47a020ff90184363f65822f3e3cbdb4" },
                { "cs", "24dfe109b9c7160c5117970f0fa2734fdc5b25fed434f61bea64bea81d75705d98ec383c2c9d75b4b08e798c0538a3dfeb413adfbb36aa95a2024a4e6596b6dc" },
                { "cy", "ed09789018d4cee6e89fcb1efda30bc3d1e3162ddbffe486ecb5bfebf028c1a6c1c7e2d02e45cb60a153584f770fede5fed6348d5bad3fa9fbc4c4af00515f40" },
                { "da", "76fc20dec3a42c3e93470ec6a2b781cc9e1e3bf522de0d8769bc5ce9ec77970949f40795afd55e8122e11f07a24f6f4b0e7514b580a8fc17b91df46aa49985af" },
                { "de", "ded9df30fb5fa72638421c771b6cfe2fe5c292103a79ba0cb935ec09849221f807f7aa1a8ab12f70a2e4c18a4647aec1941b85d4ae961835eb644f992f52dfb0" },
                { "dsb", "d6836e24ecc5e0138062a3b32485abe164c8115870c294ec700577a68db31fffd617f974d4224b79745544b443a7fc9c15cc4090c4ee175711511ecd1904cfbf" },
                { "el", "c8850fc29c353ce78da3282d5a7e1bac9c79a8e260270ee1025b860d3a1b51ad093412326b813fb0d83c382326b889af94db499d04f76e3cb84fb5d4dd25d7c6" },
                { "en-CA", "f67e52883da3c88556854a07aac0b3182439b1ef105a630115b6b9421a4760a36cf1e53ea9ec5586be7e512f600080668636f689fc468d9ca1778c0d51fba692" },
                { "en-GB", "48204d557c011e272b8144f8a651b2809184b10b38f1efc8e32fb48286c7277630ae50e2eae16fe74fd3bb2dc6d42e77c141628b43a8a5103a6f41f7366d47b9" },
                { "en-US", "3897a7382833993a6e4f966154d90fab570d297b88514df22fdfe2602cd347d1ca30a911760ea576368b1b3f136025dfd024c4e9703d6ce09e06ca75420a3882" },
                { "eo", "74708f6cc9cd07b1e2a6bdb8331a57c8a4b200b26345498dd5c545cbab52eaf6d18753cdb102f1ca2569825f10eefcd17c585d2fea3009ce7f89cf294a2a797d" },
                { "es-AR", "61ec8c07515c878ae763b063f5344cf46031549afb43627a485f840ba30916c1a25736878b35561bd87a52806df86753b255905c2cfcabf0e62070ca04a40aab" },
                { "es-CL", "e6d02f8da3c4d7dffe3c6591807e9ee3335f8d4afdaa7b2df18364479ce32b62a9c33799b0d732c14a7d9ef02fe46fb548092979f56a8eb21d01b7ddad09efc8" },
                { "es-ES", "ba455dbcaf3796fa5d28d7f5c291adfe83bde1ccdf2eb08c68e7d728f32be4d0533bb197f39307e17e76cb4c8f5b5f5399576b0588b2df2ad96d904ba5c9be53" },
                { "es-MX", "29a4d8edc23de16aeb50c437c233436f95dd193ae0ba9c0267ffedd3f6420858034e81334f7b190a51c06d6081752be28c54e77cb03450af04ecb6700afce909" },
                { "et", "b3c8a2c0a1c9a470a3a3c64ee5d42b6d4704e2708bcf34e0056afaf7c9cc0a50e40ac082084b931593c3c2060f8f2209b62649bb036ba297dc4d3b943c86650e" },
                { "eu", "58ae1577f62368fed84227c814ed4979b373bea2e3a4a2be991e9c3249c1f2eaf56e5bde0e6f7f8b27f21e52f63a057572def17faeda3e0c8255f7433f9be28b" },
                { "fa", "b27e9ea78bdac178a4748e6bd7a39619cb156109bda34ce5eb3b9fcd512364b5d2aa1ddcb9f2667c24fb1a207139c08511751b43189e0844926b8eb794309bc9" },
                { "ff", "6a4d3dac5d4ca0491759fe717843e949dc9159902356ed1cbcd828e2c6f4c5d05baaf4021b9e27745fc3b6f5975af5035b589245ae23fa95fa02edcfdc47b4db" },
                { "fi", "c7f82f3c969c177254077bd2e105fe0772ba825710284889ed5b256adecf3d8aa25103392e28b91598f0e66de3dd2fd6eddc0db209b33baad5761261af511b94" },
                { "fr", "fe3039eb874a6f751cc99984b11e00e18c80c1fa10a877a63ef2d52036c911c52826091b84fa63e0ffc26d342e534b31a59c74ad72ff1e1fad3700cd73bf791e" },
                { "fur", "f3e3760893dd7739b538d3d575abb762fa8d9075c514abcdf2e8dc70e6cba225c8772440000f714018c51bad08266d987ea90af5f44317457adb15ea64f411aa" },
                { "fy-NL", "5b091d282faaa7476345f728e0a6a445454da5921f70443b848bf570f27cccd2f8a621d9e67817853ff1415d34b9f023fbebf8f14e9142e9d4574df30e70d9eb" },
                { "ga-IE", "621f0738e33fd4e8d93510219335273654a7351807586de58172118f945c7eb1814bb6806fa3cd4978ff578118d3df40ea04a4acc1939f3a52cf88716497e188" },
                { "gd", "145b07cb973ed05008c3b4b7789374ecedf5ec82726437627f3749190e505dc0b42bde516c85b4a6428209ff571e4a33f3e1a49e2904c6df523145fdfbbdc324" },
                { "gl", "a9d8da51c771f7adec25005b22d70cc78e30deade90d33d0cbda00d8996521f78daa4ce0d91435c39130edd76c4834dd56b00443274fd5a1c6a5b35c399a52e7" },
                { "gn", "4101c05f51bad280423b772b5953b50f7a0ed017ac0a5fcf06a15683d9904e97e67d753e5de00199b23f1d141d281fd6aedd3395b42bd57ee3cc6668c9669371" },
                { "gu-IN", "9e199a4289a07ff62b7dd77ae2dfc5ef95b5571a705be906faf0581451d62b6175bcf6718722422e4afa5c7540c44552a6eedebaafaf4f8df36d5f0cb3f881ee" },
                { "he", "b90c302dca6aa8d4d2982863a3743f6f3d993db6db1eb66921069a2befef2d6ec8d5a5fcbf2e66266e256e9cafe15136310a0cfbf6de6cc0c89b43ebb6b2679e" },
                { "hi-IN", "e63db15fc08eed4877fd506c252f26031ff8bfd441833e032b83fdeea63a6b257337ba186270f06cd77f78cc731536482d825317540b1547b0f15d228fed763f" },
                { "hr", "be687538d463b90649962a2487c731316e2716ee0aad744c3d1443040cb1c9f41816ad5a036f56f055e5c0e88d179eb86eaa5501217f900e268408e4006854cc" },
                { "hsb", "3452e2e1f4666bfa3572ca15932e26630e25ebee129f116b20c023c05e43cc655ee84e0043097db895cf4b2d362877e539e83c9de5f18b4e7285301f58286a57" },
                { "hu", "fb3a8540b9837746feef69d137abb099d8330355c0010ed6b05139b7c05af5b6b7b2508befa0f38a8aa41951a03001b29b7d7be2b9bec7fc2f54de7a56fe1236" },
                { "hy-AM", "3302701307cbb1315dfecaa64c740d64afe56315803dad635eca25bbef08018e02b4488ddad3e7e5389e813dcca13f830b1f11a76cfb0c9dc920824a06ddfd6c" },
                { "ia", "0185b594f8dec60080b7770a27c077f2495226ed5eaf20be6701ebe41263112cd72fd852089469d22ef8dad6a66dd03b7b0300d889c860fdf398e10c77a40f1e" },
                { "id", "e82f6d3895d7c513834e5e6f571a5d8e980aaf0cee5f1305b40238eaf9edd5ea76ba4181f842d268427cc4e5a41d56da9e26e5fff4f4db90d9515c351613f603" },
                { "is", "beb34c9cb5d281a6185fcbdf7cc264fc514c6e573dd52e168b9c5aeb907c648d58cef048b9845f1ca0c2a31c132804e405596a17a2dcfa6ef501b308d19c2de0" },
                { "it", "622aff597b9e0c8b542b6fffd38dae3ab0e937aba93ef733c4d64f318c30dd3263813383b3f6c46deb15d85b13cb1d1067fc3eb260bc1882d3241ccda95bd6c5" },
                { "ja", "102044bb0a0344f9a78be05ee8172a5e7b090132ecc0b0a68a6f5e1ce6604b5aec5c1a0a3f6ca1cdbded475da9acde7fe04e05b9e0dac65d3b91fd05396467aa" },
                { "ka", "d447b3e106be5bc9d8e9fcdb880885fa62cc12694775ffa5612da674be6d801133355d7d56dcb61355469b894ed3c2d8e8f1223fc4ef467a3dda955ef0f90453" },
                { "kab", "c33a543a15de8105854cbe3943f8a507ff7c629e8655e74d4d99944834161bbb04088cc8cc82036fa9162cbd619b3cf9ff94b1448efc3e358f965541db76abbd" },
                { "kk", "8294ef62618dab55856f91b071e02d4b9d23a9e8e3f6d485e22480dd75ff5a66b29034f434e063b49f7db58dd8533699e91506b12a207717506853c39709f0ea" },
                { "km", "74b1214f27d7b18eb3b4fb5c968eee943be6d0cf5aa49267627e4f3b5352397d0bc3518b97337ce49dae14e1544d0e5eccd784ef4259fd2c0e1bca412651c931" },
                { "kn", "5944ac614fa802f53e3741ea46f59ffb853eb5c6c9de56ea30d000e1d71a90985401acd2f6328e05e8f22108a981aae6da72ae08bd1ad8daddbeaf88a79719da" },
                { "ko", "bf3cfd5d4e38dcd4c3038b0361bc48205eb2be635ee441a33c7ef2fa4316820e1900f0d736c74785f3ba05e071d591c13a48b6ffa072031aa9c65cc671e7633a" },
                { "lij", "c811dcd1a1e4093f756bdcef582025ef370fc36b446b04ff0e1c4d5c5ac82eebf47a6f45a979b7f793b7f6fdad234069019e3a39ee9dd4f0e452dd520d953d1b" },
                { "lt", "88606634bcd474c2694067f169f60ab76ddec1281c82e986171a4276bc331e9fd2bf7c0db0c9b2bf30543ef7f9c7f6c554e1c407449743a7ba3f8391162c4c3d" },
                { "lv", "be912a0ad6ffbf20756660151e99db702a590880e707d9007250828d2eeda8b4559c080eed0c59b70eb93f2f10a40bcd04d049f916c1fdee6d295e80d6d076a0" },
                { "mk", "bc44b58c747b1fc45f635d151b8e463715497cdc97df8f4573c6935728b0bcf126614dd95167b55e88a942fea8680ac0fd2a2325b1260b206470c9f3cea47bfa" },
                { "mr", "c7152fe5b8c3947c62384251718c388b0418db0987fc71dbe832e2667f9247bb6a57b364251017415f70c5e4eca3e6ce97360df9102f88128384e6c4859c0cad" },
                { "ms", "f8124087447683c8e428a739d89b4a7d938b7071d94b4cbc7f4bef4a32478b3249f64094b14f520a4bdbd9fa0c467eb511ddabbffb0502d0bafa1910351b85b6" },
                { "my", "2548655ca5bd31c5bd7a6176f2648ecaef413cba1e98a3d72262e1907b4ef3213543072c133aa9929f3b99d1404b067a21c3d266a1de6ae92bbf563ca776e366" },
                { "nb-NO", "9dc637df4bc8902bc191c247c5fefb0d74582611d02c27bdd4b150f3a1bf155c079eebc06b8e26a9d0db6c28bc4e8ffaa753b58abef6964e210bc7566b1c32ae" },
                { "ne-NP", "a8cd8672f3b2696765ace00cab873f17fc8f88e314b2774709bd9dae8367a5149d396aebec64305c775caa8b8fa29577d975afb9d9e5f7ad978870e8e1b1b277" },
                { "nl", "86aa157dc3b8399b55774ffa2b552cacf54ea2f5d9695ffe95e5b2549f4413830bac6db68ca454b6f3d2dcfa136e44e6f38a68d617f54d3aae5c0858b21cde0b" },
                { "nn-NO", "7ac0bd22c84daca99c1be59e76929b74fdf2e65d01a053f8021db07a0f17f14dfd9bf25969dca0db868229643ae1ca0b898235a1d56bbd7cdb320f6d1042f152" },
                { "oc", "9218ccdb1bab81b07f811cb955cfae71bf2b8d15a4be82086f0e32d67bfe094b0e81704988651eef409c86a766e08c69fce0d79b8d990f69531bd7dbd0d443e8" },
                { "pa-IN", "232767fde67a3a7d2d9e7c83397b69c6dfd89fc76679f7b0c92ada0bae94caf92a07d8e018b67cde08d46fd3eed7835e53dc6cd935281eec2326d8f61a394f92" },
                { "pl", "f955f707642f2a344343e3d6ffcf8e2da2dfbbd92637f842c9ac0d564985ea1644ed0803b00d1ca93463a773c8ae53c8f9d253ca898ac8b79db6888e58a7c35a" },
                { "pt-BR", "3a05257c632ea215a6af7c29abfe9bc43dd63125ed797261709bdcaac6933f7815858dcb82c044259c2db020ec22ec8a92ef157849b9e2f47971030be5dff08b" },
                { "pt-PT", "a252a82d5a4f1875cf4838d37dd65f8b31dcdd7f36c366eca336c125102f5d3d7b6a98d455f241741eb7276e9016a58ef898fe76ed6111df48d42e73fbfee7ba" },
                { "rm", "b10f030d1e8d60821e663006679a7fa1d684fb560e734b8e50a0843ab1d7bbe03c063baa03e41b7ee4e2061cf9340aee5a57c896a91971b51713ca6c8bfbabc5" },
                { "ro", "842cf40f66266dfd83f2066654641a9dac5c7d4ef0074fd4b8708bd7bdada67a35e3c3b281d839844ec2a97bc83ee14cb8a469329f58c3f6fd8f88967c0e482b" },
                { "ru", "7edf4184e0b349e87b944d8b7200bf58f98b5639039530dd4fc0c346ae6da338984710b6b553686d44514e9fdf287e38de40e7c30a661f354087763251ef655e" },
                { "sat", "73bd150681424bc427a7dc2bdaac12f4595a536b9b4141bbc3b89036c4a147ca55e0988e8d08a22a7de9ec256ff50b610f79b3bbb551b83e1ed0ef76095aa8c1" },
                { "sc", "b15d371141048133bf76ed0c7826e242006a1c534b9989de0e8119b9d966b32fcf419a58a1b26b21547bc76e6a7978a002c50a44f602a08f5e72120e5c8468c8" },
                { "sco", "d4d52bd0690ac0fa10756bd3c8f8a60a8296c090ed31dade922cd878514b676922c678368b859d7c49364790e87e9e24201e7dd700538deb553dd9fd113d4bf9" },
                { "si", "181cdc3300bf906ce83b33017c66a21ff140b52818ad552959a1e799ccfdd63c26b2e6c1bd4ec33ea6bd40add5860f52b27adae97de5367409527cb3d4229d0e" },
                { "sk", "88b55e2243f501dbd2f18333f4344c5394b7a32bc05f9affa4e03c09354e775a1e737e2e430dcbff9c8dbe57d355fa227328b03c568678ce65fd45550dfb1cb2" },
                { "sl", "bd466b777fc233a3ffc6408dc81319c5df809e7b643ab0aa4c4bdf947fda64ab878d32cae3171421fb66d8b89d772c05e78df819e853768f33cb73a1f31b57c2" },
                { "son", "530155990a7429b93743a558e3a4c8b67f5e8080557d0f8676757176b76f7225b50894cea75cb0738fb558ebf57764d2104e48eabb19a497461f7825119f547a" },
                { "sq", "80d0763192d7794c43561fc41d9bb5532b0b8561a01e99561f4ca503312fa1001b39eec3bf29fa5439ca1fe353457d39ec489d3cfcc3d0f0a74c9dec0e35a8ee" },
                { "sr", "df1dda53cde3cc780696a2dbb04c81fb3599724811b72958cc1ff7a17590b373f937e13c4395fc039abdc66daac618c3b7eed49b884e062addd0e4dbbd7409e4" },
                { "sv-SE", "ec723d3d0792ff642c1cb2201188b5897209eb4ab503a98cff873e65f67b1e67e7e4f4a2bef9c33324d24c9cea4289ab264204e3c672bf874052236f1fb64115" },
                { "szl", "531c768ac356bbb32fb402d0f8ba9b10068a5f926a5e5a72bc8622327771c1b9ce26c64f58e203f2244526b382f4e8d263059c47de90790b5e242de20e4233d1" },
                { "ta", "7a9fe941ceb6814cae5718d7a05d265de3897b24195e8a347375c0edcd25bac41e165c6d22485a580b4e682d14062e524505d7840c5ab837ae15738f0eacda1d" },
                { "te", "c56ed008820d18cdf29391486baaf6c990c8490bb6199d76829ae832b30d723dd71426146091334ce1064975f41dda332f19c5128a1533b09c32177b5138f1e9" },
                { "tg", "aab5eba3d3d9265f94ac701bd3608007ab06f347cbb07fb64c1aa78d58f01cbc5646bae0a3385d2939fe44fc7388d5d50b64f27b8584d64f1492375b872c36f3" },
                { "th", "7cfeb9ba566958801eb3da14ecfe8fa733ac5087b8724313d8403beb46c88caf2469f5c19ba84c7a5fd97a0abba8cae10d006254872682666d5a74af12992231" },
                { "tl", "2abd984410b86559ad0a83ec1188c991e743e587a9738510ef3e149e4261f3318297cf9c27f5585877d54e45f98ecdeea8f942094ef011534f7fe4536aa02bdc" },
                { "tr", "42d6950d2b288e047c6bc491e29f8d56e597faf0c90e7487d930eccdd5d51110e857cd5b93b96f32c6f60d42c30a02b24b53a4fb2b4d681580db80981b95bfc0" },
                { "trs", "42891827f70948e2ce453c77ce6e2e4e91aba510dd21d43bf531582ae387324f077da2e5c8fa327545b6d6de22b3b87318d856b4f119820bc30eb4f7c8eadfc5" },
                { "uk", "0b94d5949245e7b166f96ff0243a4c3b628ac5b708e0911419de8dadef61b27cb78d91fdce103a4d8b20a45297cb5bec07cd5a15658f8e68765ff68f36092ebe" },
                { "ur", "a4d387dc1e45256df7ef6a925f19b8c6a3da261efa9fb2475cae50f031b3fd370199f8524bcf119da7ee4da7ca4fd76b3ac1c3b9b3015c683ecaaf07a3308f2b" },
                { "uz", "3e81c49054d729046d3b6b38c1005fdda7cb221e7949f8978a3001a36479742dcb5dc16f117e2e83c9e0fc94f464e96881a7b44a0f493c3eece97a246ea1aa66" },
                { "vi", "362fd5561e52785612b5e967ffe572ce1dd32370f61b5cf8cec4e4cf35c088eabce1299490090d959f7a98a22e3cc6557e88e845dd17c5a3b6ed8303c415e7fc" },
                { "xh", "40418d5f1eb9223f89e8ea441cc4996bc7855064a721bcaec4f9ae04b8a1635de98d9360768293c9dc91014632d86372fd0b98dfc9981ebeab2b527a5b83bc27" },
                { "zh-CN", "d9ea62e251cd99be76f37942db52c0a217ee029139a39c1b1a1d82a311c4a2be779af7e4c8b375c686d67f7ada81815095f3f5702a612b4803db1f3dd50f989f" },
                { "zh-TW", "b2a2bab1a67d03b3bf77e8736f843a640f6ac75febd6f0cd964b06075138a20eaf50e2e4236872d14cd0516e8d746d46425713f409cd9cc4ffc08dfb94a7914f" }
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
