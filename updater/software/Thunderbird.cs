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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


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
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.10.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "3bece73013f76dcd6c240b4be77127403dcaba63091d981128ed3fb8aa2dfcd90798784330ebd847455d024f4e63cd827647a3c18271b61620f1545d65c8b217" },
                { "ar", "6f7cc72b76dd4802b4914020fd861e8efc5deb32c07a2d88a0edec806198b66b9dc924a1eb72a6174aac2157527bd43b14b1e68cb036117f3d21a6468d360ca2" },
                { "ast", "fd6180f415f1a2a57bcb2d04ee3f9dd30479a32b4411d7df7e4979cdb7805b2ce16f5a131a8a09e8c7a8e4dfdd2e349a3c5f3f508476bd8047d72464f6aa3ef8" },
                { "be", "6b5d537a2e23ceac710a5403380ea53547b2b361f00888fcb0bcd8d0768bf96c81e9698d01eb09afab0bc6ffa78685a520b6502beb4fc98133d2623d0dbb0ebb" },
                { "bg", "920795212dae9e99beef01465959dd67629e9e240390caea6994140364e1dcec19c16a13d7946f62d605b1b51bf379223c61a9f96286d642d2fae131a4f06226" },
                { "br", "763cdf0aa4ba4344315917e2dbf7844fcce4f788f8a7c329b136f2fdb993354d678da2b172f1e6c432822ad1ecccaee3f57bf4a3422bf98b7ff98655d84066e3" },
                { "ca", "b03bf8656345434982205954b448c939f947c5e6fd12f943f1d3f988809f44fd92fe438960ff569bd16ad662b4c746fc4c7293adf28706cc465d0e086e180b61" },
                { "cak", "c85fa47ef8368a2e425c4cd89d7f4bd8edee95abc4578940c2ac7a0193bc20cc62b49a8570e4aa7ac2b632bd53551479037ca612b7e6712573ebe405f3103206" },
                { "cs", "cfb67825e9ca2b96833c18d63b0b1c3dccbf60f74cccd47ab7788b0c759ae0df2988d86e1eef76e5b6a4a0d77928d04ae515c75cf7459b58e595e28bd2facc97" },
                { "cy", "bf2b04f4a1ddfe1b73a38a30801b820d27306a989c0e177ebe6f980eb9a786c7e133e557227e23988488a5d137f9b6446d5f4f7dd722b7d0e90aed35525954a1" },
                { "da", "7eac07bfc3c139d10e96b59fd866bfa1032a0f712713f9de61628db61a227b119bb8bfdf423ea19c994f3a0c811725b59d475f10104413fe34e40443b98c311f" },
                { "de", "8a1662a8b179d0ad4c87dd4ee0e7d2987e9120b7ec3b71c4955f69017d5e28ef365ccd049660ee5a949a3e99b30a7ff030346ce9b1f87cbb6f24ce6699ad68e3" },
                { "dsb", "7104ff18be08052fa4f49cb4b1ebefc338f986179819e78c5a806d034d613afd6130afb232ec006e7ec70b47963b59937efc89e846bf23b1fc9f4342c2c4911f" },
                { "el", "ba9d761e090cacc8b2c014992b215c982ef449be8ab31bc9b04183751ddd3bcfd744d533d78ddf9bb49d5cd4b204e3ffc63189e398da3bcc0a98adf0ad2d7c87" },
                { "en-CA", "511cc3e69da4614ca544dc1c631c2e78f8edd6123d4c11b46b6083bda995efa7fc9ce7bc00295dc9451bb78adb99662462b97bc89aeb34887a02372d1ce04f4a" },
                { "en-GB", "bd880eaa980a9aaa6e3088c03d9a5a37589d61981e965bb55182b29539250425a344090b5a8daf6e81e8b5b8237ca065e58a3c2fd176abb61e981422f7ab46db" },
                { "en-US", "685fd27f845ca8e09583b04c2c7d10c85734059423ef9eb2e42a02c5bfad2cb2195b46b43f96841327d05a4d8e43b0c5c135021c0d8133640cef839f9d35abfc" },
                { "es-AR", "2cb9c3c2852b67bdd426ed0f73705ab0b849c7049e00b00fadebc2fb207fce7c26cf7e74db8fa0ae38398707ac22faffe7628d4c166c1854731f35c0b01d31fd" },
                { "es-ES", "6e148c7b079506a7222a88112104fb589690a03ae618bb0968ed6e85f66987aeec0609aa4041de341660976a91759e233c3a2282a36754714d57b1acb75a18c8" },
                { "es-MX", "0be171ad7d9d3e1f18764d394eb71ef809e4a423ebab86ed37dd6d9c3c74ea47f2f9a83beaa6e5064706435f8f8d9116f86cc96d34956ca45ccdf79dd9e54958" },
                { "et", "4e89c90cf9b4e1ed63dc8e9cac900f4f69fb1833d136a325d47e3ee991cd03054146feab470e34acbd3f34267d6b6ad08258de7e41ae23d5210ce9253a15de07" },
                { "eu", "b48e99bc9b29430aea3e3ed3f8dd42c5823739eb5beb45756c2f4b28d7ab392b0285bee4d21874a971ff6bb390fa8b90a965eaf52d38b8bec92fddf362e66d9c" },
                { "fi", "d0ec56fbaa6ddd961b2a44d775ce33f7a513078d62e71d5247d49ea843f79f4f347d99676771a554cd2c589a22fa78b24875b29a99221c81b52825fbe4821a02" },
                { "fr", "599f55f588442b07ae609ff42df7743b1bd088d801b352329589696e7c1ebfd7c3425f44c64a7e7d81f0092d3bce0b000a76ba2f4d31963a49c0feaa29e7bb54" },
                { "fy-NL", "3c0bae2a90617609b837dc0ec17bdb1d1e98d0b01d4a69de76e3023550dc8ddea4f52cba6fd639ca6df89da99f088fa0c18761f73c178211d1560a84802e3faf" },
                { "ga-IE", "c9450cda7c6ab0a24828fc9a264d4cba7a1b304cf9f28e8f56ed2cb0cad539760d912c96ce28f0d98410720532cc6dc59f8aefb781b95510ff7e40e44da09b2b" },
                { "gd", "d2831b7391404f8a104d1ccdd7f99fb8dc81600da73e04b2937719f2d7cb0cdaac8d991a5669f2c802b7e4e075b2a9cfc4c9a6994a95ae24829077ae2cafdcca" },
                { "gl", "a7476e2ed6e88c759a93921f0d88fd9c7e87d0080e1635f9e715cbdf729e9775f84f042d1d29766051253ffe69163a576439a40602f0bbd2a6091adfa28b46a5" },
                { "he", "e071d67c9846f6c1d4572133fc2c37e60ad5be89429f17dfea9ece35df14190e857b2ea27a7ccaf89b0d7d6c9026e8d8a4021b74d559231e000a5b6b08f1bc5c" },
                { "hr", "16c448e7a27bd14735ad89eebec8e9ace5b8083d4466db8a9300d71a3ae1678929c702636275ea3bcce8e5eddc58ec80229f203284fa47a8193769f0858859e6" },
                { "hsb", "ba8264e65db0c74b9c28d5827f3e7aa3133c265699c96e604990a88a0892afa0a9f678204ea369d2c8fc1575d3bffe0e2a015ff30864cfda204b135135f76d5f" },
                { "hu", "347d9b4ab13ded6899074007b311ed5a51cb43a495298550e764a386eb8fdde49885e4309d03de5f61f0a61e839a55e24d77aa1e2d01de0d26dafac6eb1930c6" },
                { "hy-AM", "9e5106caa70c9e38e30ac9bfecdeb8c632bcc4d06af091a91ef0336ebbf769044b069df5be6f3eab739d9285f64f5b3b63085e6c26045fda3a34fe2e5a42b4b3" },
                { "id", "2c9e041803520a9b38b5dccad4dffd5773616a34ee0fe3281fe09101d7b9a2f73d845d9fdfedd01e8bbb1eb063dedbc3ff18464320aaa7a6c4ece164718247ab" },
                { "is", "9c132413507b728b5030e56516b0090cb37bf6e59b220ffd9e7507864077cdf218c20731f87b4d755fd1878b05225f6eee45a9846847ff2e0829b71983e1b2b0" },
                { "it", "f1b3980582c68e865e66dc86ebb49a78331df811299030f711a6c2a4c5b7074130196433c445489c414d946f1e48255dc7b0f087b0f5176d684b5870f51aa01e" },
                { "ja", "e63f5fbd38777e3138230716563e6c3d87ad7ca4193bb330e41cea1f89142270281e6f5d9792b77fce88dab5310288a4bb55af2210df71c312123ad243b0f1e4" },
                { "ka", "3b737dcd5e5c616bc48903b1e9c20e267e23433bc8dc678e4f0b992d1f93abce5895afc1a1af24fb644bfad2b8e71ab9e8bc28a492e403f51a5e9b32eb259936" },
                { "kab", "7f48e95f52b59151aa09cf96b89029b764acc3fa2cea884da55af930356c275738df5f2abc7f92e52a658f123d932df63188f8a8578b8b84695f539b457a4398" },
                { "kk", "e22716a4511fc92f0564fd20e451d1df63ec781f33d4e88688fbac553369317452e18a3591c0b71ca81d51069d3c169268954134d4dcd28063aee1b14992127e" },
                { "ko", "9514dec265aac776267b11d0013ace54f19f785db11e16d6bae06765b06cde16bd3939d1912774d94bb8e0c2ef540cbd73d2454e7dd36202df15adafc6eb8fb0" },
                { "lt", "17566b9f2c418c58f7d5a9e2a9a73b31cf68c1c8121fe8e75e8495e577c906872bd023172ec6d42225d6579d0ed29f8206aa5edc9a424803dab1c2ab4063e2a9" },
                { "lv", "094d82d967315b811d2e2c6687de7c54d8cad872278047707fa689bb3e3c3ffa2a24215f633c78fbe523f678cd09bc05c3ec066ab54c8ce44bd78ffc1fb9ce2c" },
                { "ms", "aa679aa729b6cce272d18e8ad2ed459f1ee38922e0668044296ca2f31a1075ddbd0dade73efcfa18f8ab8c85f03b5dd8140a09c291a99d1104e15d9a7bc800a3" },
                { "nb-NO", "cf1c5002a9fade50dbede9c743f8d10f27c2d2675a10769ab6d6e907f109083a4ece4ef054b21d6b4d752b56ab6b47cfced585249106311637a5b2082f73089d" },
                { "nl", "5bf86df7bf3ed08542dacb0212c66ab0416d786f7bab4fb27898c19b24473f8a3dba8185be89abbaf368c064148c86050f34239e1bb75c975225496a8884cd8a" },
                { "nn-NO", "e9ade876c4ee1022e9f0695de41b526cc0efe030425308b07cc7284ce484589caf0dfbe64f5e44e8f057ecced3438fdd5177c3ce79aedd2516442793cb0d2fda" },
                { "pa-IN", "e8fa12bd0c43237395d43360962b01d471710427e3205aa4be82c508465b5f873b99dce88c935717f78782bb334daa48d6e8296bb7502cc0018eb25332e3a4f0" },
                { "pl", "ff6fa070e6f29f325f1aa1bcba60b6c4574efae6ba9e50ed68b3adef73311c7ef740fdd833af7743d1c2ed8d51add985782124929f27ad71ecd52b8e033df6ee" },
                { "pt-BR", "d63acf474e64dd8adf474131a653764f5ed862c5052eb9f0e9d614b4087d86db71a7b048e8dbefe297d3da8733ec0828c2199edeb84a777734031eb977e0013a" },
                { "pt-PT", "91c93c14db10260499f8dee04976d30397c8202b12f54bba21dc50401b821fe45d0e70c8bf2b79bc585457c0a0f5fd2383af0b73ec3ddb9ae8be932fe6b17047" },
                { "rm", "f2f22dca91b38bb59a4ff809451e8b745e09f67cf86e5a8554c002e3564fc68ab0ecadc7e4d391da184f05f5744b6dfab76d372979d2e27cb778f63a1ef07e85" },
                { "ro", "9bc862228d2345a1214f41b026785959ea81b452544238140d234a6e5e8ba87682999f68750380246f70ffbd26241025de1c17497254122fd6f44cdc9543faaf" },
                { "ru", "f8d2bde8955867461077538714ea55294046fe83577b60a0e091ffae72d333c8ccd808db8e4f448e2e12e08c45166e5e73c5da9769a82d86fc0c19a284ac84c9" },
                { "sk", "d2b2a3976e99d3538a4531915d7cd8972b6d77c9df141c5d531e5031fd9feef922b66ad714bb7c5a03b68d525f0070d6f299e369adf456533a5c235978473694" },
                { "sl", "d613c8dda784de2da0b376c47b52ade2cf4bcff85bd4116e61b6aa6d696cec7f95be6b5531fa1768444969ca1819bc4b37065cc886a7d974bb7f158ef372d7b5" },
                { "sq", "8e8a1d8c14ec33d2fa90284f4a62b38f43cd900d26cdfd07ba4810af4cf3969083cb59fcfde009385ac4172beb0a8008b1a487341ae7d9535369149f7b5278e7" },
                { "sr", "51f13c1737c40a1ab40391aa92e7e698171f9e61acbe5db8240497363cc30e2b1917522f3c6c80089cf3fe3bc6df3f139f4f0dee5ce530673583b71a6f7e6356" },
                { "sv-SE", "406ce4b9f3da5ed3df612f70e771b49b6a96f721d65c5ac6990c0aa9dde00b96ee5ac7924cad550c7622d79094d23dcf2d80c8681894c5206ec88a27620cf10c" },
                { "th", "35e1fc4f67d9d1955cdc7163f45ca4d2909cebc5c371ba278020028c3bb27f8ce3e5e0b31a299ae87ff5a853cb11b09c299af3c612c648d58f4ed65b821f3a31" },
                { "tr", "b85d0e81dd6cd8b0aa3bcde4092371bfc32bcb42d1bbf60ed2f211be9328490d479ac08a830681e203d4331fc2ddc2ee6888317037b468a859a2e97160663db1" },
                { "uk", "d145e8abf6405c5b09df4cda22bc4bce2c8e7811eb63b9bc2b697777c88311cce21d11fcaae2c012efabcbe2a8339bc161fe0f3747d04f333dc7c9e2636a20bc" },
                { "uz", "1c8162653d2a38bd58ca7f91d245bd115c44d2f300f603c89d80c12b04d541f5f0b8c59239934635175ad5fb1fa2989933819e033bf99b9e107d625e16966bd1" },
                { "vi", "d349932b1c9450488a555521371b1ece392fd296721315a7464cf0dd409d40ac6f1773cd5d2c785867bd1cdf7c9232143a6132c9243dac9fd6befba3ee711c1e" },
                { "zh-CN", "6b1b38e7513479ac683ebec3eaabe04a7773fa591ca54cca26df04fa64a461ad26c148b10dc2b2ab37358f9bd1c2ffc46b98c9c041142bc19c0ac080a3588d6e" },
                { "zh-TW", "0a353eef736ab14acdefd702fd2454cb40ecdf3d10e2b674426bfc78aef1f78b929339e2cbd84a130d992bc5787fdb403078e0cfd66e35df67cab14ac680d3bb" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.10.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "5eb95719a46f15432cdf23cc2944cc73f50b26cb6dbd98a690132d8ccd246a50ff7de471fa8ff2e61249aacc70ba23f43217898aa219839d35d721bc673f45ff" },
                { "ar", "db296f659e89c5b861a0b59011638025ae730e29eeec100e8826acae7d253b13c801c00baf202dc41702c692c2d54dc8367c3bc700a18622524e5db00a205c27" },
                { "ast", "e1403a14f1f045734d965ca722787cf3e0939f410646542d9310c15ef20bf742da32f0d2272d37cd5222eef7b2a1255a4cb0b6c48d061789f5e4b35c9179f685" },
                { "be", "d2ee9280156cfe220b2f2cb7c593c39ad0ec421e45f4fbb97255606bb15575d85cd3a77e9f2f74a25787bdf18382667202ef29411749eb2aae13aca9b878c59d" },
                { "bg", "413db144ba3d5e851ddc5b3458eb5b3c40f23d7d758f2af8d8fcb5f7ed1a2c8ad4d5b51bc6079581894af878ae6042ce642b1c1bebbabf0678a69d22cdf1a175" },
                { "br", "4cea2dcd699d26e8e2f9672dd5df51b837a9b7d2fa0af796b9c4e4753f94eb15934c31a33e55e7761df1e4c1442c93f6f65f469cb08cccaba1b03054c7b31e5f" },
                { "ca", "ee1018b65f482570192c9e20fb48c59389d2c11e26588484d1418bf57e91bd62ba9f8028f22ac958ff753025b9c9358c31faa87267f313b71486e6a2f2d4c7d8" },
                { "cak", "38ada53cbb5de52ee32a8aa0bb579aa7e4b934d12649a100284b61d925174fe04f17e1697f0341c387736f715eb8592e8055fbf411ac82d6033d073a5fa7a5cd" },
                { "cs", "3adf2f815792b6b4e3102dfafb3bafde4fe4b7a7873661c4f1d9f1b51c7ff9e1fab18b47e9b601d8cdd3d92a18fb36282aea21289c645c58ae6a0dba75c03b93" },
                { "cy", "6c96e84ec0b6d083f360e186668dbd5508d662b30361aa009f75fa152a9ab15bbe5fd6e4a2f97e803b5ff5fc8e4d4a2da281dbe1d4244b53254028b1ee003e98" },
                { "da", "972e37342f4758f5c4bfdb72965daac81d7180dd90e3a17d262f34d780e0e835b1edd3671b5c87575fdd4160629b15c9fa2991ba01bcbee4700f5e69b339a679" },
                { "de", "87f2f92bbd2f9d5911fb382e011c711e909f47785334790ed65954b98aec47de1f88858dc28d8efd85852b4f411790d75974d9df988f96a7867a70eb342aa222" },
                { "dsb", "8ac2619d630a1eebe00c78376b7d7bba78b9c58497d30513a16429dfc7c7f7a1fdecd9bf84e025887763ce6573f426adc54de5d695a72fa3e0ff7a919ef59f80" },
                { "el", "f95a998e2c787df7f7332570deb8fbe046cd8eacfe340b0780cf8e7cba0b6bb29f082495c59c26cc792bf09bb7afd07ef6cad1391ec6c3cb0d2ba160eb464fe3" },
                { "en-CA", "b2b5d4206cdc90684cd6b72bec18d8dca60767aa4507262ef02a3d2e21fd6283ac97ef007d0cce48078cf8a84c913b666361e43a6378bbb5ee777d3411212d65" },
                { "en-GB", "e3e182db56dcc1ec9dd5bd2a1de1427e2ab812a2ce1a2eba2aaf2f21a075c2acebb1d80922f2c995a55a36239b5b5aa8fc19def436fc859797da997c1d538da7" },
                { "en-US", "b67cf1feb09f77a2af950a518819459bf1118c2e4e5b8779797a8562cba0a3f6ba432e6b6a8600128c9714b6733bc7341cf48881959a8532cb6540f262f2b8af" },
                { "es-AR", "171ceed39badba0e273687cc3215523a69f53f040a0e4208440b6bc7aab0f635c7fa7eda67101b8e697e51608ed798ef06a7065f73369786343bbf169e91dfc3" },
                { "es-ES", "82d657910d29982cfbd59c40e0ff65c81b0c74b0623cbf04b248e3c1c656e9ef4ea17564cbfa98d32b90000dc880937c8ca9d605f586d54ba9b2dd8bc83a80cd" },
                { "es-MX", "a30c3aff7896fa085a3819ce30e035b4445696bad2cdaa7eedf737f5ce8844abc24995d61ce0410eb0e164bc9200a02c9c615d924f1bb82838e1c68837ebe589" },
                { "et", "010309734b9e2adcb9cb08d3ae811d6b1b360aa767d7c499ead6f41cd3316a99e302d5455ba8eb927e404e36e1bb43f31c826c43f6f82e043a38bb5d6660786b" },
                { "eu", "dc23c08687d0ecfffc76d57fbf36dac9474df50ec9aaa5c4876943c88aaa30a902b058813e477c4c6ec2bc05aa0f28dbf44b53bdcf291c6be6f9f0968e8d47b3" },
                { "fi", "b57e4791e2fc7e8fc856831cd8991731088e77984b1476c8219e6ef4136b1931965c0e0ecd45c094b6abb3dfcf066208d5c84188f9885294fe301518e5862ba8" },
                { "fr", "4fcbd22a39f5d3759171a634f351935df1f930b4a8948e0c565e78c8ccf2cfa23e6e4aa727e300a5df2c458d0b9155b2896aa34f0fb1508cffa81af0849ce808" },
                { "fy-NL", "1978ab82048d35d1bbc78bdc7a6e39142524277542d1c7346b8fbcfcb9c9809b487ecc6d57477d78ab14a58fce681706a5fe705449f63625d037167400c31b97" },
                { "ga-IE", "baf059c04fdb4a74929967c7a1f1d6dba75f47b9c04ab1c72d0199a462745119ca3228f83e39dca1a0765484a8031a9a804fc101f27f5a34b22ebb974d03551f" },
                { "gd", "a2f47e9af0d07238ba09f8dfe755643604ad005774f60a0b8eec0bc639ea98a290e60feb7b14a105c9547aedfe232e40e35ec7bd5a33b5671d924f188549e36d" },
                { "gl", "48033adaada6b92cf1223076365acfec5f7b398eb000ce716deeee63f0b8dfad1cee08edf185c109844885f8451768d50f2cba9295f96be0e3504bd91e51f0ad" },
                { "he", "325e94c97807cc415156f21904d363077abbbef71cafbb3ff13edb10d1df90afa3b03eb2ffbfd315200ed9ed8ae4e6a43d0a9fd5f0ffd932e67908c5356c2b37" },
                { "hr", "b175302be73d98e6379a7201eaa8d73f0968c6ed4bf4e3107cc4da21c34d6766f3707e34b76367e1d667b6f1415cb11a846473878cbe4b1ac76ee09e57cb8f7e" },
                { "hsb", "09a958df415c1f7d6dcfff3023498183fdc059f249796474b50fb98c0fa6d6d5b5ec8f4cfc1aae0bb249c3212437b35494f212bfc47e9e379d6abb0179688b33" },
                { "hu", "81abd461206bdf1d2a27685e7d68b50b72f979b8b6df256cfef2ca6c5f07f6eec8df8a1f3e36402a988ba5c73090c05b7e86e923599a5632159975af1541061f" },
                { "hy-AM", "9edf5d0f2189568cc6ba3c7f205ec3728389fe8b0b1f35d4907051e09fcbb88c6a2049989d7cbaec020411698d8df3176c0a54943a31c5c44d90da1b231ddf6e" },
                { "id", "948ad5cf98adbdf7fb0028c7d48606f034dc6a0c97daf5d1734b84dcf918c3f8b01415127f7196fd963568645c54c171029a503aeb9f7587b2b8cebe1513dfe0" },
                { "is", "49ad132f880be0742f602f29ef374bc4684d9d67f497bb0cb51773d67e836eae8ec25a3632d99e47f81dd7a4cc0691d3b8033f3d9b6722876b0474478e0134d1" },
                { "it", "f5b7da1c5044f2895661d86a27fe05f61f5c39215896140f884cb18e7e0c46c590b6fc941db95c4c7fd34f262abaa37f488b4aeb0d80fe733516fae3cdd18b9d" },
                { "ja", "4c1441e9fcb4484ea2845b67411a2f58e47121caff56af178661b5d446dc4f4b20925091f4f6214338057731073d15420677031ff3a8291ac2220367b3f967e8" },
                { "ka", "1835b7e70c19941564d692e5dd14f8921ee7f0d307e3028d16b9a5c961f174c317062fb49808946617e0fc25405535ac6db6b6fa11fae57833000c0ea95c5c32" },
                { "kab", "980ef841196ffa2cf2125df37a8cd0d55ccc3ace7be4bd225e998fde32b443da7fb4c2a3c4e52c1f7c94217517074eebe3a9fa1693c6265ea9ef4607154fdaed" },
                { "kk", "0ddf6d380352d4e7a406c957d12ccd8851e9b29cf22ae093278a87852ced7fc7f79d0a76f2a88c289c344e48964029c84c668450a38d049ac8e39b9dae4df33a" },
                { "ko", "cc6414be263874d223ba0b20fb5224e55560094e03e1be3b1873b84ad327f073c669ce91c22622e70dac48cf13ab5a9c9ef2dce715a5631b4b58468cc41f105d" },
                { "lt", "910ef0ded95a5e524eab3dc0884f70a769dd285b60f2db5f9ca923b8a9464015d9e0c39640c1fa19ce16795ec6a79664378a931eed47a1d6ac6f7a7279c5ecff" },
                { "lv", "ea0492958cdccc438ea94427f734d71fbfa2ae6f0cd2bd61209418cc99e3d9541324ab7dabcc1338316d0d5cc6ae2e5b05bdc6b68df9b6943ec09405d61cd978" },
                { "ms", "1f2a882b96e152a09f39e4b4cf0f904bec741614a6dc9413195af70c6e7feb5f8f6390132f37b0a00dd09527ef6883efd54bbaad1219afaf6b34b1fa6ac2299c" },
                { "nb-NO", "419aa6242dbef8e1391df7a47eb072307878cc24e33f055a3e9892a69eecce8b0f5726172434615e9d72b01798d3d58bf9c4def6c73be8cf89ad214676a23d4e" },
                { "nl", "d2a2a871f74a511bbe64dd4351a5b717bed193b56a9a4a4c84f47e6ea413ae64038c9b2c9554db86b0e224615bb460337e3b8142522b7d6166a136fa6311f5e0" },
                { "nn-NO", "e14d5b5624830d1b18ab69be949dd2b8100c03c46711f9b6b47c01e031f23f0573dc5b8059dfd7deb54c1f512303789d04775ba11183e7a4fa73939682a09ccf" },
                { "pa-IN", "06734dfdbcd73b4a8415793839cec95117637cb50318091cfac48ef8a62a36a368cea5e83ad83fea27ddbc6b0e176210dd40d72a9b743208a936a59c0e77848a" },
                { "pl", "08bc9675aab5884c4bd1cead0b6069784d049db46029675abe2f5aada66292a957c76806c674446e2feccf703813dc465750957e111c3af9cbc120ec6560e164" },
                { "pt-BR", "bf39aabda579a660282d91b5b7faf3ae44a8a6f668cb0ac1a271ac2b782ae185e11d903920879d16b202e721308215063b5fed45813d65aea662b9a250ae3434" },
                { "pt-PT", "a669feadd527b77f27e732c2fdd8362a196a20a0f41127211e37253a67374e9f984f3de4db2b7af55b9fe9804e624a654b7966df425c392057370719cb280e94" },
                { "rm", "9a009aaaccce5d2d9c99ffc3d8ec543cea104c8fdd1b220e8a887cb4c25d7386c333f4391becfed47e2e0d87b2dc69b56c57bf2cc510cb736fa40acbe7a33383" },
                { "ro", "f1d91892ed1700252ce942d41406da6b523be4dbd3fa5f941debaf42f529f9d4709b686b95a8b337f33373da0390b544bc73555daced83a70a1b0b527643ad34" },
                { "ru", "fca3bcff916c85276876cbd96c93ee1d018590e2e7b6c097d8d7d4f94d3b1f2482588a7f1ec39a50bac45615f378b69cc18d05309e99f238f16da6346f798d2b" },
                { "sk", "7f9e431bd3d7cd3f7521e401bae3be30133ca84a057eb880865a7e46a9f7eebfc1f49d08177deacc595a8a6d088851ee094b14ec9556b80dd3e64f4f04b947ec" },
                { "sl", "91dd445445fa0224a34dbe0f79e1c83f9e8ca1a24bd83093f33850f8ca6a175b8f717ce6342bc70532b388f355db103fbeb2c42eef991db8496be9955c7af821" },
                { "sq", "8e2f440ebf1148bc6ca25ca565ab0e8168fb337c9703f92ba9038d230cb5e9214c2436cb398c3a0cc69aef330140f1956d4a2d75f93b37f4018bbb62ae2212d1" },
                { "sr", "297b5e34ccd37c2115ee59005ab01e6fbed3f643b54c933f7e30298e829bb801269a7b11a0667fe004b7d8519a4accc65dc3bde63e98ea5880d2b08a5a9cc8b6" },
                { "sv-SE", "40e4d2eb847fa29fb23e03d73985b054883f4e877cf6c6d20db1626c78a16f1572f7d76386684daed94903e3284f1faf99c69d067c01a1fb4e01999af17effe7" },
                { "th", "b49f04c40db011a2207851c6212d5f323c9fe3cf4eae1d2e614ff6a510e767d54fa927584ad78044ecb5bceb09520b7dc31a3d39590a735048d8c00db9dae8cf" },
                { "tr", "54b00a785f4a6149489c8217622b284ab64bf00bfecba43196a3f940c75761067ed8469abb592bd34c7663bec4b914468407ee5d813be1fb8b56bbdf41bb59f4" },
                { "uk", "9a37fa1980d9b0dfe9a37a6ff1e483a3fdd625a6beb07796a3c7ddec8f0e7e7b7cacfaaee25ae53c91e07c7a58ebf812f34bae1ab50deb5478f0ccef862940c7" },
                { "uz", "ccbf8cfb36777b71c5da3189a32125368b60ab241ef2965d8c098af3a7ff930cfdefcf41e35ca36eaab208e8ca9b9ab6cd05de2e690b613ce109d0b57880b51c" },
                { "vi", "0c0dab9e00cb7025699e9d4f63b8652d9b736cc3998ddacf1ee1f68b17c9a9f12ee711212f8e89521ba03b14d43f06ad92ac7a1caa173ebacee9d6f1312c97b6" },
                { "zh-CN", "eaedfc201c40e6e154a157b7e6773c46a2e8b4b6cecf6743386ffec2f0d0eab0297e44c3e74fbda830a7339092d1a5583e63222184757dbd3075cf4e56a93dc0" },
                { "zh-TW", "53de8c450271229ec87e04bf5833abc71124a76297aaa3648f346f5daf03ad8ff2d16d78c7f22535060e8404fa97ad77a3ee0ee675681201f84ce1c3565b23cb" }
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
            const string version = "102.10.0";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
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
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
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
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
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
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value[..128],
                matchChecksum64Bit.Value[..128]
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
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
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
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
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
