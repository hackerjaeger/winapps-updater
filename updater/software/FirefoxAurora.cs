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
        private const string currentVersion = "128.0b7";

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
            // https://ftp.mozilla.org/pub/devedition/releases/128.0b7/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "29e98de3dea65ed7348bf94cabfd564b7edd75c0dcbdb5be4ecf23997e9a68c9f357a6dd5d44aa4ba86353b36a019f9dc1ca551014fb294fa682cdfdab619c0d" },
                { "af", "350dfe1ab4b7b5baacebe3fdd59e126bfbc06c46be728b4c20709a95c3455d35217311b3198a137d36124e92e68ccf9d53e1ec7c9bffc849cd9b8070240c3437" },
                { "an", "ce6472fee2fdced8834c39f1e4df4e95a545a410abb6447e3db6f57617d8ca5054cf4e0a1377d2691c45ac92fc296146fee73c233d95f09e8ff403c474710d29" },
                { "ar", "c53935809ce40f72cb4857d75f65a2230bab01ae6e6de81e65de9c14c92421a48c779a60ec7f6e116035832ebf250600e9ccba59b9aae2d1a7a6eca62a3198d4" },
                { "ast", "7dfc63411b4634a174d496cf296f9e4874cfb6728980c2ac361023eff35db0b54152b2a0a03864f7a572dcd7701edd3af08af7441f61376ef8fffa8cbf7b3fa3" },
                { "az", "3cfe8c60a948bb200d4a731ed807ba8a2723812048ae6662e4c7e0638953e68d00be4c296c5f3a926ee891adf621b0957b5b7957ce04ca45e44e1f0626596602" },
                { "be", "2c404a624f2d731582580a80f45b270e429e5e5b14d716bc75d4f779c9e98fea0f5a40a33cc6814d87b98487f2f9ef32ff04716e26f97cc252359481644e512c" },
                { "bg", "b03387bc743d6e1817965fb7123f4bdfa20f3f2dc5c79a329bb1ae99c0deb9c6d94821e2427365d106ef59c8ad86b0d98be6c4b8bb0c2f79e99697cb7d01455c" },
                { "bn", "76e9a294d4d9569bcad7c3910adfa8f5b4a4effa7cfbc90ce64df90293a2591c1e21a0818a2ec2fe6ad2cc1b4c5888206d5463df66659643000c3f43724962bf" },
                { "br", "3b30ee5add81681f8694ec56c6273aa4201cc6762658883e1f4bfc663a57aadb9f3b18668dd3886b278e730d6b94162643504be5e068aa8223d83640edaa2e1f" },
                { "bs", "94f3aa1095bb43fd25ef624087383c89b264f7697e6e37769c439608468e8c449f9a18e0aafed64889f07d05d4c1686c59735d356fcd4a574647e0110e9ead57" },
                { "ca", "e651dcf1e4fdafc62eead8de648900a534bb171b79abb0bb639c0fbfb4e478d6cf3cb25fb0d5aa272bb65e75885b09e774f582e95ca345a1fd044ca9f77f3ae4" },
                { "cak", "73a8cbd8da235cab7702772bba6384f3f0b8e3926e204b3b953688896f1ed7c4eb42f9edea303e7795236d9796892ddb7583fddd1a33a92d84ff7e6db387f179" },
                { "cs", "ffed286e3dfdd9b4758d3e58ce787446819d5bf3725b19cb20931c79636f5db1ed2ed5d65974d734eabc79863e0a9b79782dc75421ff01ef1953270b5b5bd086" },
                { "cy", "7ab96e16751bec715a20c5e5c1688ae326de7164758ef852b47d0e58673b9f9e93b7d4937f55d87318d9836ded59a36a82662dc4ad36acb5efa693fd8dcc3049" },
                { "da", "acd87d1b649a91a605260e6338a251d58b6c74dad5d7c8119a2bb4a24ac3888575f3f296ef80a86c89d70033d576c5d57ee512dcf615648a71ac7d9f82c590c9" },
                { "de", "c94a1a495daccf5b9bbc067dc5224f39805cabdd57c8b6ec832e63ed49049347440154dd83ffb42621b29ada68f9062941207cada0654a5b7edfe44020d89147" },
                { "dsb", "b12378f0f73c917d5e303ce2f2f26ee97828813bf3a6a03bd4b62e05610ecff0afc2e0afd3598091ad318f80c7503d2652b166c9083ef7a9ddf5cb49b7c3a1e3" },
                { "el", "4f62c7e0c44e69cbbcf29d11043237c38741600089f669742c6d43564f2c107509d2f0576f3195d74bbab8ea99c4e7611e7cafbddbccd74990e54c90ac4918c1" },
                { "en-CA", "f623bd5d144bd72160fff7da5715dd09ad1bb7d5a40befe09ae9647eb733c5a73ac8addebb24c9294ca76027009e310a8388ede7f5d7bdc4941ef381b2ea6912" },
                { "en-GB", "d4b1703bdca25f0bbb406cd4632502ce31b7aaaedd099d0e499ba7a17f44b88863f8495c1757c2863069dac8a9d2cc4beb2d35a67109073eb8b90e9569a96657" },
                { "en-US", "f3e999902fa668186681c42b4eb50719f9c2ec69c265cbd321160a17dd8f4ea3ebec9b3fde2f0281623e64fecbb7e9c0b2807fd330b810de59c79556a4373295" },
                { "eo", "3eaa4e6cb48013a74f225c884a29a2ce75c99f1854930623d05a0a703a96d453b651db765283dfc222407235bb6613fcc2ff0d123b39a11eaae98048aeca1ee2" },
                { "es-AR", "4b583320296db41ace7b0d2ac11ff201c383ff1fb9ae0ee8e365a0bccd0d659d1538aabf401127e978eb67dd2b389728e23be9555ffc902a65d582c0a8fa89f1" },
                { "es-CL", "645e04454bb152b789bdf9495b625c289fc1aaf677a6296088873cbc324b739986107c4b97dc2fa9dbf41bc10dc2010cc2be02b8992f3a36b8d5fab9c35fbbe7" },
                { "es-ES", "316692809934a70ed2f08c397409c9f69d6d4c43b81306bbd5af6a26b480fa0607c78c3fbde9902e4410aab7647d69c935fb8eff09caed6298583a1d62f9c8fb" },
                { "es-MX", "9e8d3dac99b22f2c7e47bdd75bd2392032ef2f967893ca0b2fa84c432e9a96d86e569432fc6e8b4ff09b230413489391c3bcac6b01478af183dbe1afb5f19ae6" },
                { "et", "39d1e669ae234a6e8c19e069858e484944a7c8277316c37339e768f266848366687f990214785a9dc763ad5e4c3091376545ad9e176c457dc82f52bef22b0f94" },
                { "eu", "97326920cfd92b82ab8158a16e15a254781e1b535a1afbfeac6fdbbc4e829562f192ab604b36f28bef0d274ef2e2d0f955023a3eb1371a7385e243faa6cf7b72" },
                { "fa", "5d1c8b587e695bf84a217f27a4b1fcd5214b8b6f647dda35c260ecba72f461bc6b717842b25e622356c7f92f69742fa2720d285184bb7f6b975b05c9c364ac57" },
                { "ff", "0d9b81b6712ffc0c61ab78b9d0f77e78a45e3e51108db32d8d87988ead60a0fcddeb7b3f4fc236402a879ca378c49ac7b24fd542887c37d9169f9cda6fa68352" },
                { "fi", "e3b3d233bad7001d8fca093bc34ab91382ab19e6ecb6570fa96dfc6d3b0bf9d676e353bc4204e71132af950ed78dda4b4d4571fc0ab6a98c4a485bef26819b0c" },
                { "fr", "c89b1aede6d065dff8fbf2d36b37c7f3049a93c9a015ea112db225d1ef46a3e9c6c89d6dca366b3e5679932e55ba3248bcf66fac1ddddd29cfb0af8e7d401305" },
                { "fur", "1596c52f22e032fce2002ef38b1ee3ac135ee5bf4ffb8324ff57e35afdd809f1948dd953145ce6f0306ed360c3c90ce76aaba3048801c4afe3f1b3021b5badbc" },
                { "fy-NL", "4610ddc7e2b1258c57b336b11609f911360b37c1f494534074427e5292de57e585a98c2e16a9874e3b27179080cc8f3651b9c8115b9ceb8de797f6f7c6f8132f" },
                { "ga-IE", "75c68642f13ed56e35e471eaad50bb9d74caf6b79430d9c36e1fbf368fa308082c843a35c9048c92556d57d2e2d3c68093e7781d1c34cfc6b36eb42d3649341e" },
                { "gd", "081b886d6e0f7da0c97c130bcdf72b69f3da32bc9a183be1b9f2313004e4116be9815361a979ba93f043ee3a190cda5cb2ae4c706ffcfb21af9b335bee057196" },
                { "gl", "4e03e46983804c2368efbd50b9a9a82a6dee116db4a0289b35e4c4d04f647af4f211b40106d9a32b9fed06e26e908b395620eb02758da587fd1bf079cda97e1e" },
                { "gn", "645505b14e6ac4c2721a1c9004ebeca7bcddaf85adb7c744f5086a6428a6e6cd639bbdccc07458843bfa77d40e3df8258e8542bb8eeaae77b11ae0f1c22b5389" },
                { "gu-IN", "c6e9c40c1741aebb93dd1c5b91bf7165dcfdb73c40915dcc59167dfc2ca9e1100117a08545e8dc22832995bc472fb2b1382b2504c9f5fb96a7d2dbd2b2daba30" },
                { "he", "4cbbdea611ff3c0587aadde64a6388e7d42bedfbe21ee5102870218e3b07fab19fe3807599e1a6814c7707d0b9f038d141d50604f5bcc5b6a015065b52264ed4" },
                { "hi-IN", "365e85666a0cd1780110aec5dee5d8996d39f65165b7eff462c585b040dec7029c05856e76a4ee745487f7543f2ffa66a37d0602fc251ab20276b195acda95c5" },
                { "hr", "b64324539ca877cdc99d5305faecd5356c46ba210e05cd5385293b16073d214e1a0b5e53f177fe00c0aaa94bf89aaf08ec31756c8ee8ed3f0764cfe53d5c68df" },
                { "hsb", "1a1e658ad47988c9882bfbc442d9f280b6ced2a2d74f5ec553b923e636a7c93fa86761b91588206049b24732e8af7abc224dad7b0dac30e563a1bfa4aa863f27" },
                { "hu", "0632bd1bc9660a48f420ae51ba35b1740dc0e07b1be17d860574ce8cca51cd5fd1dc83d478030f6598bc60893895d920b06e55ddebd690416645062d33e10118" },
                { "hy-AM", "0a3c53dab07363851fe85bc13e44824b949e0994c56a1ecab65cea2e1ecb3c9e31356c48cf51c7b05b3e94ad88e3717d94b2a0bd95934b15900ac47e813c1285" },
                { "ia", "7f2f4e963acb0767ea87d5889388a9f434423229db99a38d2ca362f2eeabe711ff64b3d16a94014440167de9ebefd16942f5c7cdaa41e3eff056d2e4ed0b9679" },
                { "id", "c7b35ff1fe4d4a625c4f13c3a579fe96e6c8396646a60d64ff35375853bcb1478bab4e8b4dd26a1f86e8f914ab56e7daf1b55bcb92405a66ea757c2f64845e24" },
                { "is", "cef9d2cbcb0907bbf659903fb0c00c8f9a854eb2924a4bac9fe630e684021b37cffd17e604143f60c03bf57f69c6a01045665b5c442d09372747c82b8e976860" },
                { "it", "ce12e5d859acd44f115574d1d7fcfaa816bbd45db63591874fad8fc356bdd24655ce7d09e030b413e4856cba09f512f516611ca859c0b2041b2ead6c250a5c5f" },
                { "ja", "4308a4d299f688246c14da9d3c53aa1b840733f683764f94a4e6ffb8ec06d4e647d749023dcc9113c861f2fc45e12e18b4c2ff3c718780dda4f4c93d9c124f0b" },
                { "ka", "647572b0945e7c69f77c44a8ce6d71207f81d76ce0cada0255019e967b308d4b6bf9f5f913a8c454174879290b015e0cb73e02c4ba6a066dbc8fd5b55a226656" },
                { "kab", "711cf5dd2379a8a65eb75b92f808c6126025e377598fef0711b2a3ce453026425ec91b025d91b3f58a47bf3df82dfc9b1f757b6677a0d408b7ea90f6406786b9" },
                { "kk", "0cb4bef642ccd9a310f791aba8568cf1f789aead4608e5c43a60c1f6bb321ea3e09ac6b59a8ec21047c7e6043a51d84b582596dc078323989f88aa3f3551dab9" },
                { "km", "3e2f29688d6516b045959694b5a4081558bcc7c9bd504161ba4c57ad3ba4e8890164cd3874580a87d2daa37c80146362377a6d13684bea8f031eb49afdabb8b3" },
                { "kn", "814fac9169338fde4d9994fc8bf335200081e4a0fe199e39bf21cf5be9ae56bfb7bad459f412926a59017283abde03426d0d08b1292c763aa5630385ead64ba5" },
                { "ko", "dfceb84a7d6661d856379451d920b48a1cb13c08c22352a7fce65a5c74d55491c4c4b7801ca3271abc58b77f1a0742e8555598cf11caeb4c98c595d6dc52f1dd" },
                { "lij", "ef2c12f7636f1aca85b78f8f24353fa5e2b237e84e7262adfa3eaddb5175cc5d7dfb532ffe2c430aaf08fb88d9f03c3967dc95fc498b3acf16f5dff72ffff55f" },
                { "lt", "728438ae833aad27968d39cf5f5f1d47b3fd22f09d1764aecd39e736c6f22fe9452554cef994729294c55524101269ca85a97e94d46678d01a519c0d8851df6f" },
                { "lv", "0c139c2969fcfe5e981c95bf54046be01d8ce4114dfbd5762a735f75482f38d1669a99b19a88cb4464f82ca890736fed3660d14294e3f210bec412135d3653a7" },
                { "mk", "c1b81f7443545b6af7ca7070a437544f3db77ba5063f230f20b5a22ec4ba9214e94575579a2167702443ade44f78582cf40d79d84bc77afdfd565053b7f06e5e" },
                { "mr", "a1c6253a24bc6914b0030e36aaafd721eea781ca690f40d960ab683dbb75e1f6ab4c1da3064faa8978c649c60fb4b25f2f4a228b7d3489bd5546d21de6716d70" },
                { "ms", "d87b1b1994e5434d6429fce8122e50b6660b8f728bf0dfc1c86bde4fe13d3d5b33e51fe605ef8d28b3c4a40cab1a28a44e2ac364eac6f9c48d665908c0179e20" },
                { "my", "112e22d8b86d55f62513a120d2c6e0defd527a95485ebaef3c237b8fdb9e90c0e3c0ac7ceb019a54688253c1a6b1ba0e64c8d5a5d83e73a921d38556c327f624" },
                { "nb-NO", "8b202dbb05bbaa5136a8a95939dce5f7ad344bdf6e6d9c25c6d53e723f71ab26be2201d969d68da09803773807868de4c4c6a7448b3a6fe2f4f295291a8d2052" },
                { "ne-NP", "7b5d6ff8ca9a8812882901bf9b0fa30f10e68bdaafa7b37e1b5d001c637ab6a452113d9d8feb8cf5470d1a72779ae592f32daa6dd5b2f84b36ef322ef44f1464" },
                { "nl", "4820f313a0dde20d2abde42982008c8d85f2c3681f33e9330fb6aaf07946360d85f120c08c015414dd03ca0a35036af85712197a45819bf589fcaac548276561" },
                { "nn-NO", "1bf8abcc30ec347d4b313997544af27fd3b873bac6ea3a3f6b1308a6486c2de9f661735794f65467f85ba9ffc74eb05a7d896bac71ad85ac56c51b651d6c92f7" },
                { "oc", "2d448aff8777f9eb2b2292a1dc984816f9e89e006494231d4c04278c0e0a9eabc33fc62304fe393e1cb6a5338793aaa8b12f87c0d2f86e2bbafa8922d42a4990" },
                { "pa-IN", "3227c7eb1dee9232aadbb1404e4b7e82fbfec91c0117d88d52dc8bfb6724b6fb1c2976c6be2e3dac81669990fe9417c780595ce97cd96b5f248dbccb2ad800ea" },
                { "pl", "28fdc3cafa720c06d62cdc2d87a20e3aeeeb79dd10d7759a9305c751f00a8e25e651606dd3677ac81b1f1f103bcbb08f09405a3ba481346bcec7d07f7bb4bb7d" },
                { "pt-BR", "147094d4ab81bcc59fe2e9b8a52de077414e7486a95fb62b2c53cadb2615173f408e5a8885f07a066c1e3b2c668307cbad4cbabcbf2731a76db4da981a242fe2" },
                { "pt-PT", "34c133fa1f786474afa4c856313d1aec97f8862193f220974dce9e3351a8cb3fe16d722e151bc64903df882dea5641406290b6ecbb150fffa07c44ec2f891788" },
                { "rm", "b632992824406a11c5ed5b7fa07317b2dec722d5a9b5d8d648dbc39ec1d7ed1c8dfeb66922edba2b8de68db765a4cca5be26477412c5d66fa76201dc67c33a69" },
                { "ro", "9d3a6e5209c1cce6e278c40f4e757c5c4fe71295068d2a3ce5ee86f428a04739285e1013095bdda9b3f8f6247dd930734e4150cd4957c43619da7cc4fbad4a93" },
                { "ru", "2e45a8159cd53d197dc6d37db34ccf900dae09835efdbc5608a37e3a001b2fdb90cc14962d98d6e4127ea1913b8998dd8b982122e9f2be83b6e1791716766422" },
                { "sat", "ee43ce26c457ce86aee82965fde9d3e435848a7232098d3321c15e3605f7514a4bf218b8c1b40d4b3b46027c94f19bacb0582b122f9047918a8ca77ff7d0b9ce" },
                { "sc", "395d0a39138d2315466518b31466024c6f95a324646dbade2b544244a5b1a0f128acc6e815330a0535764e34583acb3f6106e413faa8bc62cbf269d6a3f20955" },
                { "sco", "23fc77a7401bfd9d20559b5745b3ebbffba5983dc93e678e938da39203843cc4fd2f2601167134fdd0d81394006ee31f0e5e290264bbae1bebd8476ec9583f3c" },
                { "si", "3b4e305731f8261f97be79f3865faa7bcce3fbebe8dc7214347275133e665f361c4454c67be65cdb38bae957a0a1ec9d250487d665ee7734801433fc16e8d44e" },
                { "sk", "c4d41396d1c21175d0487b8957cd32e1b23a595a8c8e13ebfbeb124d8b45b89908a0fc354595d73f167dbf9d43be598e55f026139a625d79df37919b7cfda6ea" },
                { "skr", "e26d33901a58e100777a738bb98f5d25994a5fa410415cffe5d493370ffae149553bdda5c90f50fc24ef0db1df2d2d27697294cd22c30e47b44c82d2d70aa202" },
                { "sl", "eb292a061d2172e67379eb066de775a1fea1e361a5fcac664843a452fa8c8947e95f48cb802c016f567c3f171a5bfd41687801ca5da7d9d72673d4724689b8f9" },
                { "son", "9f66c725a6062c4e49e75811394c227bb6dea77f0751928daf173a1b274209c9b07e2bbf531b863df161e8292ad86483c41bfcc35639ac3057403512f6a70b23" },
                { "sq", "822426aa74138c7ac512c44012e737cda0c73fa3f983b76d7d711329f56fa3af9f38d40dab78fb255a61d26b0b4e29e7e95d0229b06e8023cff550c7d0d8c83c" },
                { "sr", "cd6ef4eebba69d4735641391ed01a62a70ca79fe8b8b3ae15ea8a55045a457ce31ed2c700bfdb884cde279439cbb00fa45bfdb49eac6b0a45cbfcb3be6ff3623" },
                { "sv-SE", "0805f26dbdddd3e87fbf606098bbc646e04f590e5b0ace9e6a44e9493f5d561c0daf6479af623cbb3628f41e9bbf91bb9cc7607e2964783025df45ef78f03c17" },
                { "szl", "bc92f8336fb03657704680bd17cc51719b0d3bf13b0e1e6382973fac01f9c2d4cc9be0827c6065b19a790c5888cfe9723a584b00601e5c29b6f966d862df1eda" },
                { "ta", "1faad95d4578d7f610b5676482cf10cfe7c6c4bc20fe1c821e3a8150979f4a7c3ab3750070371f8fb9c62f4284432bea2961960fa863892964c166f3dd6484d4" },
                { "te", "79a5fe4869eafd129047831fb84ea68090e6b0f611f31bddab83a845bbe798b228f175372b2d7635de5d078de25dfa523bf89c9e34f71129b06b62ce9d5e3d12" },
                { "tg", "90a1b8a51455df191c168b33dbe10331489b85d8cff831e6450ba9387347e3c37871607fb03034efc8e8a18b3fa43bc8be53e6a4ea24503732f396f2d2ae1a94" },
                { "th", "8027b4c84cb36276c6271cccc1fa413967466cb6e2c51a3118c94d4cfc03be37805932ae777401a99a44e385b76f6be23680e80b7aaf6f9d4bff12247e085b7a" },
                { "tl", "b0ed09cdddabe5e09b05b91443d574ea60160ad30af3a2bb65b940f9357b410837e70cbf8af01829f0d04cab04fd0e72f9a943798041ab9698e80f3887c08405" },
                { "tr", "86dc57df580fff0abcf75fb9f2f03050d0c2df0500873452c9e0caecdf2780cd545797e4e80639fa133efae709dbdba775a6d1ed256cccfa0c2ab24459a58598" },
                { "trs", "e13229f4210481842cf6a393132303e349d252898b40f22a8a1bf7f5f3e3c97706e9c3ac6b2b139de2a2fb8195997baf87994a67174d6e10659d036e9cceea65" },
                { "uk", "262b2fe2be246e7b66cc6caa1ab41e45da9365e18f68c573bfcd38432f97555c511c30242c712e0af948d5e585630b91b2cfaf46fdf3670cff7d1f1d4894cfc3" },
                { "ur", "8086a7eb6264b93c8b8bff8dadb76c8d44a305e31727ce76c4a4156f70b93421d737b5cb92179e6e44225da42d45b84cd9d4bfe289b40ed7b7a3e8e1e0af71c8" },
                { "uz", "df46896936bd411942b50877461fa94793b56d022b360631edb3bd6f3983240945a2e0d30d4864d45e4205c954ea2e1540f9ef559366c4276d90be6b8447d8f5" },
                { "vi", "c7640323d0c7366208e6f2a981595ca6c18fa7a2b05f2079403e73d1fc87717900425e6b71d71462091224f4218d30178fa886c86c45a10f66f69ee30f89331e" },
                { "xh", "cf66005eda8f4c11e6250a5a9a24311f3296a8a7e36c5df7ed97c097b193f0bf79a533138e8d9f324f3bc47d19030abc1b3b61adfdd302f1c1dc91b9245c22e7" },
                { "zh-CN", "6610fd56bd20c397cb58d336575c71a0a0840a24692c6284b803a3f197331fad314e13e24818ce32d36a5f85a01c83eeb1d7f4f71d757cc249d9a08d6bb98015" },
                { "zh-TW", "295ed701673fad576b2eab1cdacb461178287fcd3363a6692a3acd7d0f58728f527dc9aedf4e1c9f4184c50b5c504a81c71ec82fb9a20d040b5c8f063fc71c66" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/128.0b7/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "63313c3fb3a1a5783b0dad1de14dc42f22b27f0cdf70b3aff29381e1263cd4ef3ad09162e4fae1e16e312cc17e266e2693ae538b0de7c5f2286c8f13bcb58de4" },
                { "af", "bd994d33e2305daa42ed79f38bf8beec0ee27e3ff254a1cfc68df0f1eacdf50cf1a72ad5874c52c32ce77665228927c9394080e5df03c4e1a489017d266bd6f8" },
                { "an", "754100066692f900b7e552d5dd9fead02c916ced08a8621eef1c3eaa06d5c842bd1deaf51b81795e0520b8f1ddfe090eecdcff21f5e9047b84b665b3949a292d" },
                { "ar", "3df6b85b8f255031cf426042e85e4ae9f66ea6ae152b2f3150209ab3277065b10e2b4bed3248cf1e4455c584e8dbbf6bd16757f9bf2bcc1a71ed393f89379006" },
                { "ast", "c46b74c6709c8a41b257950aed2cc61be2cee676d7c436d6607aabdd7d621dc28bf4d34a78233b77e651c2e72c389f6d7dcc55606c71599be883b4cb3684d525" },
                { "az", "2ea1d85c1d9907520a08097264aef1cd2dab03b5c301335ab960bf3db11782d9a79ba5cab5716de60de929f176ea4d0e5b45ac7509147b78347b1001d08e4045" },
                { "be", "a8b5c54534da7ab3295d016eb1964c92ca1ca4a7bf2bb7c97872b7cf2ef966434c77cfe83a78ba59d091b0c9b0358d62a5323e561e18ecb2a157e9f6879b768c" },
                { "bg", "067874445f398669146e8b4869e6cee634e07d27ef88a666023ec5dffaeef4e61799f13135ed9aaab5dfc536e2754eef47554827db7bc00b3595394f87f450c4" },
                { "bn", "c3f712b376355104d3d76137e3fe392ee53ef3e91fcf6b70b918aa1eef2b470cd3f31c046b847858cc7af26a949fbaa152aa3e45747e0391421808f7dec65254" },
                { "br", "9c629b56596f9684d28535973215a46e621b07284801d1170f46b6c2ae8f7744665d890f4890fa8a049d23cf89c91abf636f6f2148887529fc5d5656aba95c53" },
                { "bs", "b1312ce463f952e81415f5b37d59b768206a5fa5e1027c92e6e18c26348915c0d106299d676ff6975ba653025ee3d0d6bbb74fdeb58596eed855e674c0d690e1" },
                { "ca", "8bd2a7d3cbf62fbae4ec34b3aa30bc7fbbd22bf731e77be779e885fa3852d7a641ac5601dcb278f845447f56f4355d03afe8b6d72ce7cbbcb82bf1a5c947e2f8" },
                { "cak", "90a11afebf21d94491f9f53a0c3376c655db089cd89a21ba93a34b6c25a5dbe850df972fda4e5eefb826e5a47c868e3f76f94db8c6a3a6716cfff314409d7e89" },
                { "cs", "0f240baf89e2f05dc629c1e2d6c8e60bb0c8532b96e85bb26eef0ae300c73f0bef35253d6059873b663520493bf858347dc64f0cd5b5353d7c17afc8499c7b00" },
                { "cy", "59568880349dcec2ed38f2517c26224cf41465b7aa6908930e9b352fe4d5940b449e9401b0ca644f417a0164cf32d4d958a6ad139e59d54ab0a24f7f3f4fe96e" },
                { "da", "6ae51c573b04500c371146b632048fa0ad3db9f8ba7404232a0c0f8b36fa91a9f8eb75b2ca6b3e4d01166b1c98b59aac4669aae7598d7b11615b0e72374c3419" },
                { "de", "c5f020c6ed59dc8c9c10a1975d9fc00854f36c33623c3c97660053a39011655b12d5e70ebd18cb88ef17b1133feb7d23367c6ece9bc7b12eecc0e73ee100fbdc" },
                { "dsb", "fbf59391b6458ce8e814e07575d258336085474994b03416003bac96dd3e2c36987fb3840b139e81665ad8687489bd0ad0373382b43ec246c13d8fb7fd5768ca" },
                { "el", "1cac907fbc406b9a58151d3b1a5f464d7b614ba3eab56d27d11adc8a18c6864b36d99cb8227bc194dba74f4cdd671dbd09ee66cd6f0ecf38766a9eca6a043a09" },
                { "en-CA", "c58645d3d4e02321514014eebd1c4947c823390bae189b4f9b1c85ebbd3372038135431048a2bd07ea6540d03bba42b7c5747ccab0eeeb376b075bf32dc7feec" },
                { "en-GB", "a6bf58cdacf2971a8450172aeb1b2a411d2d85d46cd21520feb2e4fcf3c46830ecac9f56718943a6a25edaa1c6084c39245422b5635a5f5cfbd90b7be124fb60" },
                { "en-US", "2c63d3173a795d8b2bc0b1b6553b8e7d36e9467fce61bd2e37eeba895aa88304023776176e260aed31bf1cc79b58ecb1a85f6629ac2765cafa3bb40bc95408c4" },
                { "eo", "2f4c8c9d5fe279686923ae96163f19f6be3f1409821425b0810d7b9312201873760cd8a3fbe12c94f158e5ab4b22ce6801025b1b05870322b7086aed886c9bf5" },
                { "es-AR", "d4a2984af1321ae0977ef1c1bee311093b1ededf033bb046fae180224a5acec4f7e954941e99d43cf571f33066fcba5888f4f677ea3182232becc84b5041d6c3" },
                { "es-CL", "add580019732060a9bf4fd1c9c68c8afd509ce2279fb2c2c55af539d89deddeff095bc04c6f44bbb8b68c86f5c0cb95454a3e19c51cc4e1ccb670a28c5c1d019" },
                { "es-ES", "d0bbfd1ed5526c7915446dc6719e3d9151712d57c0cf19ed2eec7e312229e1199650885747473199dd247d7e47a779d901996f2f917f0d9869bf12d24e89d6c7" },
                { "es-MX", "0f5e367eb1b99b41435bac2814fcb8bc28a72789cf3f4dc491c3519ef137a3352d1abb75523736033f00256fd61165abff9a5df4ab3a22ee997411ee68262a14" },
                { "et", "aa5a21f9c18988f2b14913382c4477f2fa59ca10deeb30fd819a64ea486d3cacad1c1e5b6263bdb934bd3232be339713818cc9126b3d840ff5d6ae995de09c2a" },
                { "eu", "c3bd29ee460c8be4740ce087b1c307089c668eb6209a571eece3767c2c40928cabb82d67db4c1402462fef166c8ebfab2e64906597199048499a1454f49567cc" },
                { "fa", "18aaebaa0dfe2ac05bd3eb1dd72c7940c8b3e88bdc0990071a1a72a6cb00ce54e8f3ed8e52845af01507d770b43f6d71a35c5bd265fe410bbd359dd6ddd66553" },
                { "ff", "1d0dd4196519bce606059b346a1ec88779de99517eb2ecc988bf19130e310c785d2030fba49d49f544d65817bc374ac532821167570f69f3ee06ffafb9fd1962" },
                { "fi", "d3525eaa458213489f4e3667e5db30275130bfc09662b7ac0033bd50351ea9bc344f223d1c6c488a5f8f99271359e33d7d1991bf90a3a22b918433bf40e5e0ab" },
                { "fr", "dd5345bc27b11eaf24716c2e63d31b09e4a184b99b3307529e36683586af69a58f81644097397ff488a2740a49231964e57a9d6d60ba075d316928ada5fce50b" },
                { "fur", "a32e3a2505f92945d5e58e2072672102647377ca4b7f2c5cf5bfdaf9540ee8c4f2b84f78cb54a2ac7ed2e8a7763407b40dd40eae3b6038d3d570c6aabb89e621" },
                { "fy-NL", "57e4a784fbbd94ec56a5b510ad47a1af2aae6089549e2b5e8ae4b580bd56cb7ae9d38ad9dee4ec19667882208d1d5a2bac6c7895233656b656c569b699ad93d4" },
                { "ga-IE", "7d3bfe3af4012af9791687f383be72fcbf3c24cd435b0b5c8272bed2d3603f1f39ae10a65475cf58fcf86e77bb87307411ec9df4afeb7b413079df78b188cec9" },
                { "gd", "ed0d01b3c01fab083308cf4fafb5d9c2c92ce3ac557d82ccd76e06f72345ca3b84b153f88f52b840da5ccf0c03f10cddaa4f2d44c76aab1f4fcb77ee71855f48" },
                { "gl", "2c7fb0fd52cbec75e3bd2bf0b83d3bbf715fd64221ab7abb087f1697d64ebc2edfd53afd227e9a327359c92d72799c7931ebb1f77dfac729b902570c318928bf" },
                { "gn", "3c01dc79649d0155e945246175a4debffce67ff855a6d76e14fcc1724957e5cc89870d1b4083128ee944f24337b249a3d98f19638fa7598310ab250ae9881d44" },
                { "gu-IN", "2e0b23f97f1c2f233222198246af57a91c3cd5a64cbc543ee0a729efc21b6bc5c768cb0de5e60850c2cd2043ceddd5fb5f3f0789405c64fa342adeefa564ac7e" },
                { "he", "295ff26414bda73b2926c385b9d4899caa3e7eb29f628ce3ed8fed19c398825779ddc2002f95e36f56b6a0072b68ee926237d3a07eaaf41043c8744becb3331c" },
                { "hi-IN", "9c01ac9f8866097503b94c6006b403b3059d9e2b8337731eab4d2e5fa59aa8b49a45714039b35c4612b819e9606645dbd9eb64a452db524d874c24ce558209fa" },
                { "hr", "49c44f1fea8f087e255062643e4428d11677089e79cd5473bdbb2d76f5bedb79c6a89e651e2d5bf36584aaee33ce32ad0e7c353eac64826b0638cbb931eb92a3" },
                { "hsb", "7578a2e1dbcd26440ccb5de2d69ae897c18a64948348132c5637227e5a184b53d6c453f4c6794652c731e6e263b90617378ec325fa1667425a1290d7f98abb7f" },
                { "hu", "00f03fb2a86ee25e93bfa8778635822076ffbdd8b7203934c0a1cec508064d5fec2480a35e32195bc630ba74d3e5e10f57c9a7e262f8cd0631ccf5a3bbaa2c66" },
                { "hy-AM", "6b4d21900ffd7416287aef26e9b7ea388c3325a285ba311e067d702757d9dcd997ed9a64202c382c6026d58ea78b8193f872790ced867092267a514b255b3c8f" },
                { "ia", "190506e98266e6ebd0ebdb66813f03b8a46a2561b388cbd356a7beb8f9373c82223614ff14779914f89f36c4fb3399e7677a0cb3b207ab443c28a14063c04173" },
                { "id", "d30e05d867d9d3673b7df8b27ab42652d0f3cc2ddb6812e5f118f3736b00ee697cb99c5198b9f70afb145dd76a1da582684f2c41870315ade07b40f3e8b19499" },
                { "is", "a37927cdee8ddf488ef50f332fee26b8af59654023925cc1b1a17eec88387c0786b95806a1d073373345a738e6bfd7e6273b0ad407ad3fdc44bcb4b362ffa728" },
                { "it", "6c66b77aa0e55c39b5ec1d3c8a454729f90508d8eec5949fc7507fb9d82c9c38f4ff3af37daebba2f77c2d793a0cdaced2c6ebd56f95c87234c001bff9eefd06" },
                { "ja", "f25f980c7a43b4731340f691aaab88a79c6be6cb22226e77cfeac707e608fd2ec4be5f00037b93c4cecda026efbeb64083de89ea2179bee2e380150309000acc" },
                { "ka", "e98c61a413716fae79ee9ef09570a41800d1e527fb925a7a25d81c3478fbeca288565f2b0843b17bbfc7af454a38aee647b8b66597f33c0b4407ddaa12dfc18d" },
                { "kab", "e7abb39f458d833e2da97ccfaa11a9e36f9323ad7f17df2e4c5ba56b201f1f5b1fc2cb5fcf93adba626cd006ed7ef8c03e5b1c202dbe25c22febbd971b6e5736" },
                { "kk", "f74d1103d04beec76b50bbbaf1cacfae5ad4d4e8f0b74338ac3a2ef1202c412d061d17929b71c5b276d9396708c1838fe0505f96ad859e50f3561c2aef7c3406" },
                { "km", "039bb80c1c6fef4cba24b41268bdce311eaad3175d47cf697c83acc861f867d1e89a76aba58c1ce1c3e45ffc4b5c5ef89dc1598548b4b29f9afb519a58c08eb7" },
                { "kn", "f4fd89b230c6fa38cf48a68c3246d5945af26d08fffecf5558eb784c830cc12f3e62648eac7d48046c0930314ea1578db81bf9e7fc89d59de7b5d4f7c2a180b6" },
                { "ko", "88644fb192d47abb60ff54785b938374a2edb050b2fcc3c22d403e7e4d5778cf5cfb54042b16a715b5bb92acebe982bb38914f116bfef5b5eeee0d33501c2efa" },
                { "lij", "8ad8ff84118860b255a2caabe6e7d515455899ec56eae3098e8259f8767580e713b3a43bab4a752b66d1e772966c5fa3965bbb0654c51d0c2a180740baf219e7" },
                { "lt", "d97528382eb910b0821d0be3289260d8b005395eb50708fdf2f513a800fad7dd592767b8b884998f40854af65390e766a942d3d6f7adc7b7d7628ce58a84f347" },
                { "lv", "f8c230c67b149e897613ebfb03bbd24b3f12adabe4d86362221f7893f0184c968209d25567b457e6b1275a2c04b7f9acb697ed9a006533933749e3b42100b058" },
                { "mk", "b526fed274a1297d26be41a884d59b26a8a5f709aa512e066678f7127655d6bf71ba1f8de7b26997e841a745f931ea98cb8a654f2ad63a55de45f0da0f6cd3e6" },
                { "mr", "24a18d2d89c3fcfc253dfae22f91918200d1c1c4a400636a70501d0031b8edeabde47d2d556a4a630dce5579fb4720c59c8795b7b2c3f9a109e1be3b49e59577" },
                { "ms", "addfd27ec9b62bd5bdec1d4521cd5444de3409624d545f343230c944c2fc2ce9c82c98714142cfa9c1861c866be3260e97daea452fbc0d2f38163ce827f0a545" },
                { "my", "9e3688923f200f787152130936a3eeb1014619db73877213d04cdfbf98b5de981dccf9ff9936a6f656e8edb19cab49c4ea12839cea17470f22dec23ba892a669" },
                { "nb-NO", "115b3000f4b10776ebc127e7cb36545113fff6aefb6c0f5655331265813a3c0eea6b3d14fc80e405d832b5e0867dfd2cea321d113d82601860893f2b4bc9970c" },
                { "ne-NP", "817c774ddabb6bd94577583a1bc80c897d964431c76c93f72de087cc8f682bf014322557e6442913e24482b0266eb1d2dac05a7534a55756a457e41e70c0768f" },
                { "nl", "318ef66b4e90ab885d65279e7c9c2307f97ab8b48a0836e887a7f76c1da603a096304ce3a7580259a8cc34ab4230cc16b912a72bc969d83b21fa7ba3f4f97f8f" },
                { "nn-NO", "edd7180e08ea66ef18ad7180c5b72ad98a666ab3e8a4074ad89236e9fe79f35521d157e5f5c8845968c376d13ec381a932dcca18d5cb90339fc5b122942b575b" },
                { "oc", "04d10c2e7e13932e66a8c115c7d21ca8349a3ef69db6ffb142b86d934dcd63cc68c3b5ed0a3b72031f4d0370119f1e59f474f8c1d72edd64b73d7093e8928e8e" },
                { "pa-IN", "9733ceea09768e24ddc36d86fce80e744a28df58a8ee620ab7646c610008cee4f4b15ecabba1d15f40af278aae65992ec25db70205d931386bd3286b4016453c" },
                { "pl", "5b2d1589e58d5f9be99c7378f6254bebaef2dd37316e5cf8551c49afcd893b85a3656af2e9baed8ac34280f65d048449a2261bd3a3f84735435c76232332693b" },
                { "pt-BR", "08af6f4dfec21ebae798cf26373185b1b609d44055c0a041ceddda00611cbaaecf21e433cede4be4df449a5e3dde8e3c954605495a09e665d56cb21f6cb61b06" },
                { "pt-PT", "b8aa2281cc738843344d1071c819ca370d2f8edaac63fbe0f6e44aea7a3158aec045b3934cca69c2b33236329d0783be725edde99d447db2eeb3172b6547abde" },
                { "rm", "262aa7a2f3eb349cc631f163793bbab661acd04051d05eba973e2694f202dcace3d4b63aafb9b4c54d614eb016bc90d875489c8adafa93b72a1a6a7a7f674066" },
                { "ro", "dddf892b937224c5ffa00efb70e7e588979542af23f7373288bca9554beb5df1bec87935eaa4bb551711581f3f67e037ff2a400204b23b54676b791d800441e1" },
                { "ru", "7a0c25cdf9999fe692981146dab85b4d8e6b31520299bf8342efd712128b0e5460e0b21b193f6a94837f5acedd77196c47093b4e266f4ce7c6aa2f8134fa910f" },
                { "sat", "cd2ea064e152343f1287aacfe2249bbfcf7ff46fb1617c976f382876b37701d39138e7fd66f7a444ce67c5903b45ea1c009c721840a9682ece31bde65b5f1441" },
                { "sc", "5834314d360d9780514443bcf4bd5065d903fe199f910f69182e0e94de006ad8d09f140567ac5d41dba8377c9c84a99ba211252aa4710efcd89e36dea5662d53" },
                { "sco", "d7598a35a38cd6914ce0c97db7220048cab3164bd4d6ee22b3b4aa58f435c19a4a417f4bbfb0ee70dc243b942ed731005c114d9faaf76985b2007131b6fbfaf0" },
                { "si", "5315d524d0b0a505b8593a72562fd6e1338e7a47c78e370f911e17413544836dfae5a4eeba20bcace1e12fa09e5ad9f78763b26cd45c2aebc2656b3819998905" },
                { "sk", "e43773e240a0ed099f2a031b9bd94f1001df7ca0e116ed3d1055a1bcb089ded2ae0f6f5983e6acaa04796edc1c38143c990c1906a1e8ad9fff39b8198d45fa71" },
                { "skr", "2242f835d4f0be54e3886e556082fc99bb18d563e13cb34d42536c1d2377d936d35dee0f8a142f683fca4c875a632b9101fc954b96741ad793d2369aafc13086" },
                { "sl", "cfa0192bfb88d4027153f0f711b6dfa9b12114007d7704890fbd80cebb35e23a57ce537c77987dce7e8c791338d2e9f5508b1a24166f4064df794dd6f42d04bd" },
                { "son", "cd7721c05b0303903f0acffca695c3dd79dd946c825aa95be9332792a3231bf13b99f8c9306ffcebcfbb6e73b4f039be86f8c9df509f1b333354b5a7b78bb9bd" },
                { "sq", "a094678d7e3054ae7a19c1895ae8a8f71e3cc1a44c4203cb848171357c6b86abdc318e8139fa2659309e97a6a7c8ac66ec728117009f24dfddf987f0e86d30b5" },
                { "sr", "a559918a55642504bdc32120763a826d15b6a43b0361c84cdc30d19fd8940a096b537f10de847347b3a6330d681a07bd81147ba047c16a8f8ffa3529e592605a" },
                { "sv-SE", "dd3df8634afa3906e9b857ed87cffbd6e757b27acc7e7499eb09b0ed0a1857e1197593ebb38af5e01448d89ef14fc10e95dc236569f2c58c8a318e2bc35c978a" },
                { "szl", "e81402aade3488eab0d183babfac9616526bf8536f6a7541144230438ad4a0f3d137ec3ac63b64b49fb159ed73a77a18837c77f9a560c33b512d2cd6fb609003" },
                { "ta", "a151899f192167da984d2ae803378cf344c3f2a194df833d3ca3b95f1541bd6335b646ac7e144662c33b92df359092e434386969cd7562e4d75eef4c10911e29" },
                { "te", "481e4284fa5cec7e3717745d35ff7f7dacd86b4bf2716a45864a9cfd6ebcbf6449fb2a7ca74205820a8fe5003f6a1f6c3fe5e7739082e212e713b3208129542c" },
                { "tg", "ee605026ac5dc0ad9ac88a97159397219ae02dd132658d58ddaa83918bbc9aff299d8d0b91ef7497e2f8629dfe37aed242892f932a130c0a2545114e64fab363" },
                { "th", "d750c8860a429503fb382bba358167c7fa295dde865416fab18ee36b5dbfcb39049b9f52b9f2d0b0a11220af33111c3d8642f163612026ef8b8e668c803a629b" },
                { "tl", "ff646794de69be56df8843d5a4083b88653194a45743dc2f6bd5c3af6c5960b3274a17280d8a0f96b78f366a0b2e249978d272a3f8422bf1501655a02194ba5b" },
                { "tr", "30f51a6f4f2d38d6e5f81cf1cd6772ad3ce5a22165f3a05d9154f1769178431cd07e24c5e0089e8a81e80c04744bff2486f3d2734c0eab000389d9118b9060ef" },
                { "trs", "19bffb43d9fc5479193c70b6260620005849804380f9594d9b809905ebe4e682c0f1bb581a49581940ba766b6a775ff52c4f2eea4488d3810e0df1412f269af5" },
                { "uk", "bd3dfffd5a235c02106468a3b719ab040899ab915c582fad2bc596804cbd89e9e9bfd95421eddcb40523a4870ca0bba574132de5264c18b003724c40d4d9d029" },
                { "ur", "475de33086e634c199ff9ff6308f0da9ec1ee8a257a13655c6b7fdbcd08d60c2ee03b60f2d28dc79909c111ef2ec75321ec83c90a596c6280675907f77ecc214" },
                { "uz", "71489af954b4b23546351c0220263270e82fd05ccbfaec676afd9eb073a5af30d6761412dd14e7e977cde132c4db796e0ad0c82d8db7e7c2c24f25af29502d55" },
                { "vi", "7e095dfd800fa4073a67b3e5cfab9070633a14afeb209efadf92b30da56dfd95ac5dd3c70dd588370ba0b42c6a452f70b8ac8547d8b35bae894f6a8b3c6ac8c3" },
                { "xh", "c593744bb0760e2b5a7be7960eefbf89104e980099174d192fa6350dfdc411fa585d8885b56468139a438519193dfa96947cca7e60379ba6621a2f79b4ad0a22" },
                { "zh-CN", "6632c4047df39257f4f72f0ac4fef9c6e8290c8cb807abbfc607372cd8684bbf1c32f905fba8ab51485845541f24493698d1378a3cd8eb26837d119b23d875ac" },
                { "zh-TW", "ae7f28f75597e13f982c895d2d5eebbd81e11c88e0ea352379815afdc0b16287329daf80286ed541b7517d58052f1093cc93fb753177580245787f693824f754" }
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
