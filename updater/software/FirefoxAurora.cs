﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021  Dirk Stolle

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
using System.Net;
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
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "89.0b15";

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
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/devedition/releases/89.0b15/SHA512SUMS
            return new Dictionary<string, string>(96)
            {
                { "ach", "371ef535498d4d151d90b39e16c01b99ccb1518952915f0d948cc7bea96b883e82732e126eebf464a0bf5707f06fa94df14f78c67cc07fe0fdc30e06b4472a48" },
                { "af", "5552dd9862d955bcd9defe8ed458651c39916f5a0d96f229337db5f381572b8f31fe46842c50e6734a24dcb3c1948e1c473874f2ade898e523646706eb39dd2e" },
                { "an", "7b82c1b4a943eaff3f3970e6330722bb0afef00a21ab430cc67663615de40f1da75aa2bed73c595ce29d267a7e37bed4968199fb5d97bf56176db5b10d2c7fed" },
                { "ar", "332cf750f0d4c1e551ce80e61fa75155f52f611cbecfcfd0d88ae34300c5e766402dfbb242c85fe4d2cc8c6d864e88ed5cf8e19b9cb5aeb3ce8c9c716a51946d" },
                { "ast", "6fad948ce1cc6653d470822fe84a6bc5f8d37b58df43ae2c69698db6ec83d07ea3c0c428009eda40b08a6222d16cae1c33642d13af854bf406583f6bf1802feb" },
                { "az", "e9667d5ccb71e255af6674997dfb99cd4d914631ed6c423fa9e3a89c94d8224fba02d966dafd19d33da081bdcfd5570d5290c605fdc61053a135d7b523ba34d0" },
                { "be", "ac7b8644accc29003d9c4739daf65cfe10dc7f0f5abbf5dc23aae62dec35c2e0c5d3f307914f034b57ac8922e3900705c5537733d1d17e94d3ef3bba2edd3b8a" },
                { "bg", "a239da25faa86908a05836f2059dcbf3f3447c1c89a9d2473e79aa6a2b73a3d3aa3c1de27b51bdfbb551f404c249b1a7d4ee26427225425ec9e71bad5068b7cf" },
                { "bn", "d9d3a6cbad80ccc9ff2d33e931ee41f714420280a87fae9204e9889384a1d650460e2f3fbd705be0cea7abe23064f223e097ae566f849bd6ad7190061bb8b9a3" },
                { "br", "d220849b6fdf2e71cbad39b003342e6f645f77f55f6868c1be83d24751bb46e9fd9a089441821e1448e25f7cd9d91582cb7a94746588a17121f007bf8c2e497d" },
                { "bs", "91ebdffcaadf48719bac97d8c1e31bc8ff47c771d0f90f33455699ec4fbade3eab2ee102491a64b9905ff7febf2167a3aaef03e3b09b878775b7e2cec460e0f5" },
                { "ca", "0c10c9a91a9a20863a3b0d6d02a9ab87c2c169d9cda4f15129270a9d526647c550e393797a69b50803fd2fa905dff2efcd82066dd5ef1f8fbf45dbabf69fdbe6" },
                { "cak", "170e95fbab6359f0dac34e81e9cfdd0319b9fd3a0b4aefd849f7441ea8f4d7aeab31ef6c06402f61551cf8e90159c151b659c1c885098006e713d63d1acafa17" },
                { "cs", "4351756e2ea7092f2e30d3091d73e6e2fca595abaff6d687cd030dd0de9e6a76e5560e57a344e283ebadbc5d104c1234f7d139c711bf0d871efe0af2e5ecdc48" },
                { "cy", "ec423f1e9506a9ac66ba00a06ddfd73d2cb75524c6896bb010643f268ca8e8a9ec641e2ae59f3b4da6b71ba65ec7512941e8cde98a1a2e2a467c49f075b777b3" },
                { "da", "458c0e1029522fa875dc7a932381f7fc09485d0742dc335e8aac18b4cbaaec55586aaea216c22d101844b62ef0c7d015ba0e44088e3052162a6cab1e4583c825" },
                { "de", "0d48130b0c9e630a08ac5f1ebc9d665bceed49b1a7c4a91a877a9cba2c63f68f5a366277156ddfedeba3acc9027a93fcf7f0d4562f55c6b0f424c432203f7b35" },
                { "dsb", "917e6a7119a59d8f5eb6f69f46fa5fd36b9201732ab1a9676bb1d78ca720a723703cd7664a1af8b1d2021fe5543ea49968234814bbacb9acf19afe5444ed4e4b" },
                { "el", "20300111bd7b2cb6478163b4cb58f4430592cbb9ac1f086bc903b072003a4776f2065f6c13e0575da4f31b92c4d5efe60492cad5df8b11209de926e8e0001e9c" },
                { "en-CA", "2f7925681c0210956aae7855679842b75b36bf9ec566390f75a970460e197d07d9f155a8310a0ef023ab9910615c9447748cc5c0f6ba6002e9d5e7fc23660e8a" },
                { "en-GB", "b3bc13a575f9a4729be6d9b2a73d8efb1bd7d98e8e9b1ffe93927adc8af1f168d7c73acc655d2567f0d6d7b84a3b9218fbf6ae0e0d028afec3afc476df3aa25f" },
                { "en-US", "d8c7423062d5114514c9cc3479892f8aa643c10dfd5627fb79532d932d6e2ca56c6ac6ea230ccc05aeb2736bd85da7073f50953cd44005f85e44f5c64aee9200" },
                { "eo", "d7b04e836cc9db88d5ce7706002e7fc3ef725277bc50549fb21ad090df663e222a38a6392bceab6fc5c0e32545f6c0e817b662b9887df35f32602c54e13a5f51" },
                { "es-AR", "6a1abead24e0bec8252af7821ffa6ce9edff70ce0767e0934918e910cac395a18a7aed1a03d4097797f533f609dc02b6e8090a3c1468676e820d6454f550ad65" },
                { "es-CL", "508b10fde7fc95b60bc31b65f9b5193cb4fe2c282e124c3167fa1bee6696755b8b21ef22ed5986f98497fb5f0ab23b125fbd8b355a73670b077b82cb511248f9" },
                { "es-ES", "209fed4b9bd3c783c409102c11a092b05adfe8be83eac16ac27401f0fb2585878b9e2b8a38047585d46c01cd70fe684ee0b45cf9c93f4476dc21d93741a9f2d7" },
                { "es-MX", "ca3c1ed6c29a0185a15900512ab1c8adc48ecebd55d8e420f4eff960ecca1282a3b074f8d83f17f1b0d5c30d9e8a203203954780d494c554355bc2eb7575f979" },
                { "et", "b9703dd728984dc6f67d1b04679cf351284bbe0174af3c44f0e0428aa4a93af0f241c537c5155c0cbc50be9b9dbf9d8d87e6308b869b43e0a8bcec997cb918cf" },
                { "eu", "862de136e3139130bdfea2d1daedda7529aaa11ad910ceb40698a2e711be6813ce4747a0da04900bbf5c59b07b513618fb03c9b950bf6da0055dc2d89330a302" },
                { "fa", "540e85cca7eab0a35af1edee30bc54e5c31f51a896a2c40e0bdf3af772c4646ba2699a01d6131607d61feba8f69e7f8999c07697ccd43598be8074b6cee699ca" },
                { "ff", "e20edd97e96843120ca374e09fbb553075ae3417b95bab316c0954493893d789fc605092b70cd12b283d04518ae29a0bbf52b5e6a442dd8d22b565910548c2ab" },
                { "fi", "18eb19ae555812d45fd523f66d5caf648619cdf721ce11c7e16d6edd9a46158101b4eb26a0b817bd984921cd7adea0bcec4d06488297085635883a70934d4231" },
                { "fr", "18e64634446632c9c37cbbf7868e6246c7a42b9c2a2399b4551da6a27af3ef4d85e1195db2d589930b83d2933f803a08cbf46e5f5c7f060e1e470b896036653b" },
                { "fy-NL", "d6aea41fed3e7effbe7393a2b18701878aa21eb5b99d49887af8d3b9dd25ec6835265844e1687566018ab145cae01dfa2931fb5369f81ed3d8daaa963bacf94f" },
                { "ga-IE", "589dec94f3dbf66080faf57775cd05bf496262c6ece23f37208324c2a0c9add963cd81eb98b78cdf21ff052502a1615f0009de4ef1f2349d7635cba4a71962a7" },
                { "gd", "b64188976b9cd28fe0ffb9088746ba4429a6190877b7a9ceca7bc39fee520a826b2f96c9ae9c0ade54be564cfbbb915af109eb6a539fb96da5919fac92197284" },
                { "gl", "2a7cc0b68e88b2eb58d0b9b92fb97d6e856b04cb875741fc9bf2935b1460462b0a8788c2114934016cd7f1a57f13f722521ea11e3479bd7d4bc7ef0069844d8a" },
                { "gn", "4521c8eafa2d753429f13800d575813e07a72ef4575d8992a73ec69ac988ca741f2225ab3806a83558d02d66be286008a9e6e46984f0c068acded61618f74340" },
                { "gu-IN", "2ee13986c03b7a70bf4459e2fa777af0e351db386c03995062c94730ab320860b4825ef7040b8f0279a6479195c181a4722e5b19055341103b20438ea8c261ca" },
                { "he", "2b1c45090e18444ae47473042cd384aa0654e24181df2e7dfdebe21e6841e3cfb08c23b91b8fc0ee5910abd36aa61438ec9b089d07bb4fc7cc2441ad1c8a927b" },
                { "hi-IN", "1898a21be6340fc7d270fc2c091bc67147e813de6a5304fd2a8bff94295848099994f6c2c75d3cd4528f2dd355fe0ddb19da488a469e22b5fe8cc41d6584e6ed" },
                { "hr", "4c03c0248f55234631aeac86d291e0a5c7cbe7a2fbf6057193e83aa2739eab6bc630dd587ff0e7b5a519fc611ac0c2e28832a7a2ab2af01fbda6fb85d5b7e3dd" },
                { "hsb", "87039c42a6da633955f6d6dbf32f80a2f29d3f8df4233a1956d781971a6b478bb33e87d3f104b8877915765417a3d0df303575aecc5f50c6e29cb5c6870f721e" },
                { "hu", "52f52884e233e718fed4a5b4c1e2d9b5c7cd8768910b65b4a8dab0dc35e30b3897906cf61a350b3806770fbf93888d8ca1cda5b4a1773a60b131b9fe4247c4cb" },
                { "hy-AM", "15cd810f291f7ee86e603d871e7b8cf129384d030b36fe29b351e82d7c5248cb26973b2707a06ea738033874207a5c5dc73da86c2da4c1bc1150073a55a5af48" },
                { "ia", "286454e75d242de81e69dd7485bd098f722db06775e755615e93b7e47a080cac4095053d24b24b7d2554ce88fa916bf718b3cd604c5ef4b2401754f949db692b" },
                { "id", "be31fd07e5fa2c73424838f846c77c4b7b4d1c3e5f5e00d49b9718a7fdddd8e85105bdaec23db8bac6b9204c8789853e04626104c6b44f24beeed4332c6bf013" },
                { "is", "e0c505e1540eeefa635bc2887c707a94739e3dba2acb2af6455cd819995813ab2258da30acb6ac5c8558cecae46f85a8e1c909ada5fd2f409dd21062aa56f000" },
                { "it", "8daf82b7fe8ea88f1ec169d24c5298f24a1a39d92fa73996b004d000df378583787492cbdb46bfb30e2c3e12db9139fe168373cae630a2e2b7bba13bbb869043" },
                { "ja", "e89628c3fcf727b948337263f80e88f3192b3732a6d9c78fb715c06142450672405355ea8ba807e6e081d515e06b6888e98d3029ffc368d69de713c5812dffe3" },
                { "ka", "6eb2bfc1f105196ea4756e8dcd587fcf9dd7de048ba9bb5953cffbf39ab37ebaec84bb36bc95628c3cbf614874b1f9e9dc70cfcc7adc9f847afd51a4016d2a0e" },
                { "kab", "68b0af90d554f10993aced82a780db641057359ed97314c1a7cdaecc79a4a28422ccfe99baa8dfe70903c517e965ce8bbfa1ee4fe00ed026de77f561f6b24447" },
                { "kk", "ad2421c748888ded95dacaabda763b321d1ff01307d79597b3ed94af2f03cdb540d0ac6ad6730da332eac4b9ca1321cdc6361a07efd949574faa28e59496584e" },
                { "km", "8feff464a3b246f8229a3c953bdd1da02666aa853c6c9504b020da71a1b2ce7dc5a4da8e21d1dbbc432a095956e1c96e323f756d0fd66ae129d0caa8a7917616" },
                { "kn", "d92f01a31e1e928cf4074c0b57fe07fa153ef1f8c39a9d9751797ace6c712e47723bcde68f39aca986a3a52a7d3e5cffc386933649721f196d2600231f0484c3" },
                { "ko", "94d13e97162d183e3515d2b80511ecfe2cd4a8cf7b92a8ed028b6cab2949b971910ed4f4328640cb152d80922871200db39c3d13c0f813e5d5aaaf6bb009fa2c" },
                { "lij", "6351ed5451806315b4cc42c786b0d01ab7a011a333dbc093d0cd159845422ef259e8d6057059ac28c41348a7f3baaaa82bd071d730d1d1d1840125e89932bbf2" },
                { "lt", "31724fe34340d242ccc1ead7b1ee4f5677c071bc6d6de447ef51f867a8db47b0948bde668a8da53edc3a3e4de4909455315803cceebfa60841e4ad3f7a494089" },
                { "lv", "f4bcdacfc70456483dca98d37304a4d57d7fe874be858bef2dc20be90e29dfad5677199d1465c1870b36135b0428c0774df3f1b47f89115db69e06ad7aa032c1" },
                { "mk", "f375a8a77e86f2ac19d39f34070abcaa1ee1f8aff5a964beb0755d43f887ff0cc314349b50919df436b375f68efc6bae0904a135094785e614aa85e057f4634e" },
                { "mr", "65801ffa81addf1b6afb5c81c7fb2a770f8f4c33153d8a3af0c0dfb91d9404d4bc3c92397055c02b644a327adc6a36ac531a252e77cd9566a18367a0c5d42f07" },
                { "ms", "9f964c91680d2ff7294c900bf1de46c31b4dd986c1af80eb7f900ebccc83414c820d2d8734de7f58e1dda0492a84c6e1bcd354cdb58156287190307f7335688f" },
                { "my", "e41be7d92f4e2f816e450388afb011788f1a84245b94e19a49134b067939d02933a7be1040881734cb1e2baa7b807697c5379bab14da8226b8afc7e0309fd7a9" },
                { "nb-NO", "e6e12ab7879561a1eb10a6cff6ea8a2efbb47898e11074d1f1efe1afeec701aaa07c56bba6d3933d5225c7534b044db5e8161fd49fd11d6ec217608aaff0b5f6" },
                { "ne-NP", "1b505a2ca2a9a0d1d5b9dd20c6f2efcb47e4fa73bd6ee52ea1cff1fdd5c17ff3a3e0a1f79d2bb1efe69cba2c820c7888a1ab12f2034f2a4518fad9c7810199b4" },
                { "nl", "99c881f29c936aa4378dcda6ec0b25cb55145e3352b78b79e59f80daecac13d57083c5986858a6b031c2eb51856d5bd677271ed42a16594190e2c2d8b76323f5" },
                { "nn-NO", "0d80da258408eb1550242aaa874d24135dec5e6ad76b4844962b3b93736f630ad27779e203e8dd20fdda811d412d0158816193390c47b6d2a0b53bfd78f9c538" },
                { "oc", "7222ef8ee4871119b4eba0f44a352a63710e794bf7783ebeb4b564ae3f275d3624b1cbb0ef6765ad55f746f3f92e3c1299f35f87ccd326f9f1fbcb4f0711d81d" },
                { "pa-IN", "026b3050f505cba3a45807788cac632e2e644e46ca34458b5e8d0dc4bc1651a9d16e27dcd86ee5e39874d9f658d401d74d42b2b5c65b042050492be8224823e0" },
                { "pl", "b29bab7285ff048434f4eb054670a8c63d93f22e9e617c96009dd4e336817be1683d3a8c9c4529f3f017394c5dc1428ddf708220909efc3be0837c3266feb41f" },
                { "pt-BR", "6f6d9f9802d7eb36c60432a95b5e550c301313699a01906496d7898940ddb4f73dde4f35f1e8687994b7c96a1ae67676d7ef3ae6fb63be4920aadeb7b80914c8" },
                { "pt-PT", "53677cb6c887bf9b03cfb32f80cc6ec821ac7a375623c246242814d5aeddf201ef2504cbfbaa14ec46b89599dac95b2a49d4b3a22245fcadd685b4e375a6369b" },
                { "rm", "33340d080a12bb4b2e12c26570fa04d87d41d093afa65cf5dce5726af2286af66994338019c6fb3f3af4fa5d193feec6642ec3d00882374d53299a2e63b52330" },
                { "ro", "b48a85c96b8aeb72bdc64a7548b66e5ad83b1ed48a1e4a99b4cb68133cacb10f8584cefc38226d9f7c878c817c7573847ed7cb5a304525e6bf38f46ae7966193" },
                { "ru", "ff9462d918a60e8ec1c0e6e5ca4ccf859855ed2e708819725aa366041f0d370bc3599976da267b25acc4ea083dbc008b55b951e45a260a7b420076085760a2a2" },
                { "si", "af3101ddfffa752b03619434f6a7ae13b94ab16467a1753e538c0e2f6642ebc28f8068494edf23d4620bdbbf44634daab840f4ab0ded10b2452d0d15492aef14" },
                { "sk", "4d285c5eff035eb76a8f3441480e63236e61a3c81579c021fa80893bcefb33c4156798cede90b42fe02e643d851469929f526bdea7c5e99079f2764875564fed" },
                { "sl", "bf6e4836176c14ff9a6aeee3a3b348bbdbb9e42035d92df7d14fc9ac547c3ed004d1128bf8ec9e145fb750d6879f858aa4f6c804c12a697f21f8c29562054fcc" },
                { "son", "e6475934ea877e88e8fa327d69e422aabb4e7293af74d31101f9a30079c172124d1b518288953546735d02da0f7a58f8b125efac94c5929fa89c16aa651c4747" },
                { "sq", "c8203ca429f69142180ee1459fc3835c2e630f10b58f5e8ffbd119df85d1c7bb11f4aea4c7aafa8f9e256176a6f79fd33f33e8713a6207c6298ee972a1624180" },
                { "sr", "789bfb4b0305690e3ec48128989827f50da370ad9512fd6a9f868cca0d3273bcd83c2ff2befd3f90de7bcc1d991f29fabfbe3d41a82a55d79b838a07cfd5ca2e" },
                { "sv-SE", "3d4355ceb9f964194d77c8ebdcf8466f06f0910a5d79485d113a53454128653587858a79485a6eef4bb5eb23a4c61a15114f8e816bc7295caa850467c27951f2" },
                { "szl", "7c233053b42644c76cafe16feafd36798da7b24fd3eeb035edb5702cf346aa9a75b5e8cd5d1f35b74a63bb7a08ccc2a3fa59cca5e7bf7ccd7bfd31f53966f079" },
                { "ta", "933813dcd745f4c68ce8ea0f9aac16a5f8cb34272b1e04c97500719e874ebc8f8a38d9fdc615bbfb42fb28ae0d0ea54d66a5bc47da4d5c2e74fce8fad3082f14" },
                { "te", "4c7af93b1e8f4f738214879373aa7cc964a2d160652fae9149f79f0fbc0d2afa7ac4d8505fbd615ffe306ea9f8a175e485c4e1991cff4529bac0128f1f298d07" },
                { "th", "37c721716cdd0f86cac320fdc52ce1d12e9162c99a3ca37c2ebacc40d1ae51517d74b604eefc15e9dcc1e0b14c49aa0d759395e08fbbdb58ffbc1cef8c838461" },
                { "tl", "be4e08e4e7c76c354fea02b27893e5d3f985956fd31bba569fea92f193d5d896e7330a27d75c574b7bf3768187a637592742907dab8e0c9f47bd0720b2ba8c31" },
                { "tr", "6d36b4df09fc21d00930b44067d1e9052bbad2d5613851003107fb82b1ff1b3feba90437d5e022df70ae8960528008ad6b99b0d15b0997003fbf6cdfa9d1daf0" },
                { "trs", "205943353b8ddd93ebc791403d770efb7c0f17e6249c52f2452f5af4dd367386777fbea83ea1e2bbe41fff3e0f1aad55272102a0a7b838dc94f249e14d2abbdc" },
                { "uk", "197e6f630b546cf18e6bdcbb86b53f8dff0ac43d20c2824c4e78cc50d39f28025fd1a94c563292caad6f3cf5ba2c689fa4b1f339b0b6b603c4588d2fd8d5e3bb" },
                { "ur", "4fa4df4035cb4a0bae2170059e341c067645ad8f5ca7312f5e520b52616ab286a62c8baa4f31005f8359a42fe5c7663a0333bf0c9ab2eb3d77aaf9a4daecde0a" },
                { "uz", "95f1c5375ef75a04774841197078abd9d659bff73ab9fe09593346f23e6c2a16f7ff28ebe9b6b3be0ff3a627ca2958c6543601262b7d45ecfa2e1808a5b6e5b6" },
                { "vi", "16be3c6908c82dd98d67d13c1d8d614a03103f92c10c620fc7cbd35c41c2a135824bb23e920bae1b50e7d70065520e59ac0781fb96d307160e3d558a0f1089a9" },
                { "xh", "14fe820f3bf9ab375cf5debbc44709a35807966f8f42d077c0df48e245d67fda644841200a526015c347e722224bd3e59720099a840fcf33778145a33fe388d9" },
                { "zh-CN", "b79d7fb6512cdf3ef731624a0eec43b6e3c50641a53f7d38924ac9eff298ed41b7a0ef760f1cd364268f51ff55807f06ca5371431ca1569b3a00c1d371e0cb22" },
                { "zh-TW", "6f0b130d66a8423b196f383c6c8c88a46cc5e5b50cd1e6d9709c9a2a765bc6ec89856f30d539b6c84d7d8d11091b0e36e5111e977320465a0c6006dce323a295" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/89.0b15/SHA512SUMS
            return new Dictionary<string, string>(96)
            {
                { "ach", "d3edf1de6a9eec60dd6b3095b1e27633787dc0b19167041c82021ce5378d3fba6f54bb58922c72c00f400b17f2f585dbae72164e9a62481f6502c6ac6925e8b4" },
                { "af", "3d351717ce8b68661124ea8dc062b342a46ce4e4082402945d37e73472361ee9222502fbcac32c09034c89a8c3c6aa9ae292aaf6440794bfff19275b4f18cf3b" },
                { "an", "9494f1ddefe7d20c0c2300002cde734c2ab119ee2ac28ad6c1823346278e0509e4c0eb456aaecbd4156ac14d89ad2396c51c1ead2e6300647540ef81eee29a8c" },
                { "ar", "6a9484645fd40d55f25ec484addff7bbe07410002b0802c3244917903fac03e1fbd80ffb877db3dc818c69b8227d8b49b4d51190145a60c60243ead26f195efd" },
                { "ast", "7428cb2f20e6314c7cfc7009dfeb855febe4185771e91d5b83100edf294388460d70010e45df970fa784acc7eac4da0d1bd839f312956d1d34d287da22c06563" },
                { "az", "2643476e6b26e8063ec558bbe45cef1e84f311dcd10b4765609608ddb06b020eb28a4ea3d3bd47b322f4f61cbc8bc7624bbd82dc2e53654b05bfd1e54f5765bc" },
                { "be", "68f2a29b77f17a70b48de197549615452ce466a2d16e3f838db9b957697acadb6ac6896e0eedabc769a5dbf128850053f19c9a3141fa4598ceb8a50ea50a02bf" },
                { "bg", "2da38f77be2868bff016bfc6b29281ba358bec44ec2ed82b21e8fa69776cee0b6c46087435338ed63ac4d4735c015c5fe29a243daf97a08bd3345b0a906e3063" },
                { "bn", "feb619fa95c4225c9abb0958031959879a58dc265c4bc3d776fd1503d32a969a48fb3c245e0d07b8aa1ac423a8424f00c68054c6bcce3a97024be1d01ed462cc" },
                { "br", "548ec0e73e535744e61fb172a1861ac946230aa46f5cc54a6b6eeece59fd1c7f33e91036fbed62fc90d39dadf6ecd573ea52baffbf1eb89690253a7af96f21ee" },
                { "bs", "dfa281192b76c0c5ed110a323be196b79bd5ba52e05b2e7aa00bf7ad415ac024bbadb9be2ad45b57c7ba18776c11f372b2c43b588b1fcf583c0682cb3ee18ee9" },
                { "ca", "c1e109665ea5807612844cf60848adf6289c6b0ba7e222a513faffb4b3d0795638b00e3c012e238314159fa38bdbfa6e824f260c545b6fac35b9a1b1803d010e" },
                { "cak", "c751271ed3bdb0ab8d66cb3e7b4a763318128c57f8e9d7b71630e7fba50694c73e179bb94a39d1c43822af06bdc7adef65392f6fa643a71717a07b7a56a087c2" },
                { "cs", "9db9a9cb95f76740f0f13260f70205b8137cd04a61d998cb02f8bb9900b65a143ddc5ea51632d966b4f3c791d14ca54596f4d404c983ab4fa4f27c78af0e1462" },
                { "cy", "feeaf5cecf5a1d96f9639503cce29b46ebe327accf1d3ee8ff678c6ab3d902039b885c35de1b7d45f82e3f69224d96d75b4d3f1bd57fa2848ca7c824bc69196b" },
                { "da", "d2d09a1b264df385ec381595588e61511e8ad1a758dfb25dc3d5418eee35327a9f31ece32f7488d6e0cf96c82396f6a674e30b34317036ed2d7e00e9d91a7fce" },
                { "de", "165e8e62bfb7ce463c40ee406738055a7dc37ab82b1c48720f2925e48c381c6a2dadbf6ea513b86602a96902f4a5aa98e96df99c2535368ac98c41921d1d1d4e" },
                { "dsb", "36b14e7eaad279f2cc587157b18bdbdf8de82c050a38e2643d9e6c0e96d9199a508afacd732520f9cc81d68f7e28d0ea77b8d36a57034d5b907031bbabf6eaf3" },
                { "el", "cda59ebfd42c1c112c828300b1412d8cb9d4457e019e3181d616558944713a96e8a96b301e563a54cf8f135dba27ec95b9ab3e5c1664143cea1ebb1382b32810" },
                { "en-CA", "20adb5d06170e17825f790083aa1142d40ec036115cec05d2debc501f7671b02ddcd3646de5e282e077d91a4200f6c5fd870d5a9917d75c5a19a03648374583f" },
                { "en-GB", "47443c0fd58f394783c115e99fd38978b14bb4797b15fb308630efed2e930596cb86e90e55d66c2ffb73e8a5209ba6a905dd7553e3f13e54fd6b299bb030d9b4" },
                { "en-US", "f795db147a4f2abe19718d45df4212276059a4bdce8da686302371dd699db125a773bfdeecf763186dfd1dbeab32aa9e300f5d7c45fcdaae6eb16d4c7016829c" },
                { "eo", "819de7676e63755a970f2c78f82771f8ce4311584de9793149181ab47cfed5644140b431a2eb9d2a678933df9912dd091eb219c3ca72e46636dafea5ed0a53b7" },
                { "es-AR", "8cf07f75f429e1bf54ab942f567e40303c341c9e87b0d1402617cc7abb5fd506e5c7f035e09d09297f2ceca963456f0537feeef41c0e2a2a1c46ade7e76d0b1f" },
                { "es-CL", "69e87af925a013428d0b55811ca7241c2f2d311f7386040a16e3cc4256bc72d6a6a9439d7c67cfa9607c4a59bd1dc3bd782fe883b6271f4fb7c9f243391d882b" },
                { "es-ES", "5b7d4dea1933980b5068f5f55765e124c059acfa0ced2106070632d0735c4db12cdedd26c58f983312bf474e289f54a3582d9f6cbe3f5d9fcd2d051169b0e718" },
                { "es-MX", "08f16218812a55ada35c707babea1c9e3121ba58b9b49bdeea0486314fa68abc3943df3b7440521989546b259cddf49645681b9bede901a109b9b42a84351694" },
                { "et", "bba4f962284f134464755b8e706e55282a1f9c0d595adcf3b330bfaf88eba2f7a3f6b022c36c3c0c0668cdc8a5faa2808a0c90e8d613445daf2214cf5f7d7d45" },
                { "eu", "a6d0465d48c3cbd2fb310b2b385a76b87edb6aa817e148e5ec8e6da80dcc9881515cb690a8e7013c8679fd020d2a4bb76bd420defafe22f59b2e42f79dbe05b9" },
                { "fa", "9ede005c33c0e0c06d9be813eec1c16a60a645eceadd26b379abc54f87030262fc8cb68986b34174526b86777be0f34a2b698c640cdf6ba027c0dd485745b535" },
                { "ff", "b2ff0b433d7cba7b097dc0e374116ee81147a7232d94346ad725ba6f8125d4bff5dd656460091623d41362166c2ea9065a3980e627efba21e170d5caa62b4eb4" },
                { "fi", "15cd20d25654dde063752612b37f4b1484ac9ab64e9a5fa6b9f6293afcdf54b540afaa6f8d30097e3dcc55fb27588082f80cc9bd1f6954e35c38e2e5c5bb7542" },
                { "fr", "ed81081d35e186717f8a62c0251eb0d39d2243e1323aed2c11f2ecedc6976b72674396d877cc518be01a446f6eafaeae6fe0b17a7c461b9f39c266be9333a8d9" },
                { "fy-NL", "5ba8cc0fb0b6ed4dd02985c54ae804e9bb2b5b735257abc6da665fdfd660da57a8d75d8e2430b8b35dec6ce7ad0577a7bf86c77d2b0023846c6d6230ee035159" },
                { "ga-IE", "69290240614b82d2d21cc47b489b99f87f9e1f198b6982aeb25417e60848fea17acbb344bceb49728ac52801c06df7561d2e64c78e1603edccc4c2dfa09796d4" },
                { "gd", "6cd195ede7273483c2efd2749777c550beec3b9d54c5af7b803d34bab70a5f5743844d4a6caedfa51583b937a2b10eb7b760fad00a800e6207482134e58007ec" },
                { "gl", "e86328fbfe539f3be3ed468406c09d3c37afb6adf858cc8af8e0cd3babca09660cec8905878bf8b92c5d90a51ed4a696536b3fbed9c3a948f1ccdb51638e2069" },
                { "gn", "b41780b89c7ec2c518a56fc9c892656fb52d4d7fedd45689dfc5774a0914efa25807d26811832813f249dbc02040e3aeab867fe884b82bb9e9d65dd426c3e973" },
                { "gu-IN", "847311ad960e98be57bf20733044a593a286168d39f1c189231568acad0cdb01a358f2623217157ecd082e2baa049894e4f596ffaf8c312d9da4c3dd43c8fdf8" },
                { "he", "9d372f677471dabcb2b2b38e2f35c412091b87a3d4fca1752bb6265c8a97e6edd1ec0ebc972ccdbc91b87157942c4bffc14a72e0a4e95d33154df4edd10766bb" },
                { "hi-IN", "71b9fb51f2686804ff19209832961696a24a0fe350f241fb93921c160d42694daecee47a734ef3f46e164204b46a6c8a38a4d4799c1bf46b4fcfd03631d0a251" },
                { "hr", "53ea0674f2c0b56c7b8bf57c177f4cbddef33dcbb938236247ece8d6788dc1bd5d97410fd5a5094d75c6d6dcc09b7c7ad4a27dc51e3cd968f93ac62dc47e6e9b" },
                { "hsb", "e47d876bf8c534ef56cfbb3987ca8961b388e0ed6a19e2de1719ec38dfbe403ac9fec4e719877099057e82d53ab38c2db36e9c7c06fd51c4145e0b935229470e" },
                { "hu", "414a2b36751724a00c2054ea7f5428c3853d019c1206d40c05a175af4dfc1a462521e6598d2f4d90665e0544088b5035262002a74c646f6caf594093b7662f7d" },
                { "hy-AM", "1b5db2d9b2a65ae6b7d00754edf239ea3c78c2fe9f4d6446036b87d4c2ec217d707b48030fc897f4a7a9a019219e0db2cd5816e914964f55d75a75b50577af3b" },
                { "ia", "da1f219dd7cb008b0b8c8a40a15f6fb497d330d38a24cb0500ce4a51f86533fd63b324fb453a8918497abf9e71944822998ac85ae2953cbad294eb870a8a040b" },
                { "id", "82853d4975ae8ee74af1f0fd961168fcc8a0891a6de9f69605b5684d9bb0c6261e6c43448711c8f883fcb627f11717a1a31781741a81ea19481c6a91e350b753" },
                { "is", "a31711ddf932e63e7d16d730d67b8dc45a5bad3ecfece1bfe447d97b199e69afe8c4f79e3a1fd9a7497f1cbb8abd8507934ee4f63a5fc9322458de8ec9d3ea23" },
                { "it", "218caf268cce8596e4e593e15b73b60bacbd4bf2d7a74388296d74deccf02760c69c9c600298a533edbd99266f0e461befa6d08c3f75ea1bf4fc3bceb398efd5" },
                { "ja", "5cb53c91639023a59f1bd03f54a24dbeb975d41a55e4199d52e818c6f5144054667950d1c1b1a9aca9e898f84985ec99e72f1abc2a3935ff10c5017d3f0e11b6" },
                { "ka", "b0840b375c2e127f77a35526027e678182d81c2070d1b0883ce9ca7d1d9f035894337a3916c38075a1a3cad104513f21e79dfb17316d7b552125cc3c8dbd73ae" },
                { "kab", "f28b35c491694cc5bbd1dd48c4c0ba5721c0cce598d7df6b58b851625b80f70509255ca464224b17c63f6f370d82f27ac4d4fa3c68d7749231403d424b78c890" },
                { "kk", "afeb7403a7b765809b724b05f681b7a437b1877a825f4f1e84888cdc73c77c06e6e98a53c2810de1b881cc5208f196c7bf5df923e4d93747464b9c3164ad0305" },
                { "km", "f9ea0e3f27b0102ab035f7fbed2f75bd0927cec55b95e571247819512eff849c11f2420e741725963b259f6e326caf86a436eee110cc85e98e5c338b2a4d65a7" },
                { "kn", "b5866651ca81a5c45feb643ef94d73ef3a135b5c517162aba5c9be069b094d986dc5ff60362b9cb37bed7e9d2e8971246f081d74ae624aeec58611403c8ce8d8" },
                { "ko", "a4b1900cf042c79d84bafc63539a5937dc3accceb9282323b3bf5417d5699413e5ddf7c78b3c553a5a58c6e1609134ffb8bcb54117053778b7eae9db3dfadbcf" },
                { "lij", "4fbecca93a3a2065130d084800d4f85db42319a8ed9f8ebecc22618d760f964a0ca3382a10dcfde028218e765e6b1b5be7f193540d7c38a0fb441db814e7fd00" },
                { "lt", "d609fa684a6a48246e787509d81ea4c174d6d0d0e7cd8e814750b389e959d5f73fb3b7eae50e8e4964751e472e5bcdfbbaca87cb3ad84f830abc3b465b0265ab" },
                { "lv", "b90a9d531cec5f35a77a595dc4c5c05d9d2a5df5cd2cbd1573d6f7ccf536d8102db44acf0fa1ef44c68cb6a51823b72b53fe88237b0399e4008d8fe3f6d40358" },
                { "mk", "b4094932c4a8f554ff1611e61ae8c76ee21a0e0ce09958c7546fada83375c5782605d64c5ba5a5bbe62a7fca6f3ee55f02c4cedb729c9881d0f44682d07249dd" },
                { "mr", "362201df89e586fb4dc9acdef09fa5c519817a5d34139e0c6476ebd1fc022c03f17de9e60e1e3071363227b287366069344cbd77e3fa109320d6681cb81b9933" },
                { "ms", "a2a427f0a4a6fd9cbfe90e6f427950339193084f9fd4056726f8c3fac7c218024fb794937e4fb5597416f6c250688fd9d520fd9e6bc0fd598456f562ca94ca71" },
                { "my", "6594f3f5542331d14f8584f1d49d40a1c5043564cd9ceba28e749db0e8dd94b78afbc6b105b0fe8d35793d68cd1000155f1d38c41856965f5f20190942e37ee5" },
                { "nb-NO", "093b9c73c7c4414bd1ca7bb5c15c95140ca9c0033f9d25883650615aeda20786e02d880de3cf40c01e5f822ac9592ab760e10afdbcb31187a8cd36c9be9bd4be" },
                { "ne-NP", "4c31c5d1150b9a7aa2164b2205e27f076faad89473eebceca2b327b2a3ecaf9f98ce92f6e321e48c8f896cf8216973c96acd379fb4461fe17b8de3634fe6c783" },
                { "nl", "b5e15c3ae2bf62bcf3f2da4179936029d694248c783f1781a24f9154c3a97ceab61d1c6e8d71ee9dfda0f1034b38e78ccc9af1c9b63102d492b430952ea37cf5" },
                { "nn-NO", "82c906701a803239e2642cf4c371888625bf211e7deb28293e85db9768d8294277affccda90e9020256b2fdbec91b78bc9f9eb2877708a56a298e526b75cf33a" },
                { "oc", "3f3bc7bbba43e5d9043906d3331604fcb7d268a3fc3d53bd6fc4b039adfe94565a2d2e7c60d1dd34b6b52b90b61f3f30b38fc353503e677320a2defcf1284589" },
                { "pa-IN", "7ddd8b953692d606845f6876d505171bc6d329a9f2b074c77aadee3e29290b2f95c457a4e6343de48a6bcc00e6ef7e89417cd9cdb7680d315298dbc6691f46f7" },
                { "pl", "bf55e283ae51b601f6203f267328ba811e962787d95460f1f6cbce573f3f5ef2be1ef7e02b6eef5f8079b173bd11c3f1f37970940a4f9cc3db269328088fba65" },
                { "pt-BR", "9300795e7d51dbc2ef63895f09910b9570d90a0d5d4a26bf616fd9e55b90a04d96d5b53f0d82ffd97db45a33c2b497c507a1b6e5f52a4b291abf33ba95407784" },
                { "pt-PT", "b7ebe65b2cf1d8a3bb1ca0131a796646d6725957174ad0403f470a2acc2be3671f08854ecc0cd5c19d52efc745aaf2d3f70dccc16ee65f41200c410efa54fd14" },
                { "rm", "3c646d08e9f1e9b38f098a664e2942bf78281ef869f518f69ec4f8335128429d5b195b83a923ef49112a8254f9ecc9e9e03269afbb8fe265b5d1c58d9c650bc3" },
                { "ro", "7667c0e76a53dfbe43b3abf151339f9873d96122da55c29d2c51a7b44875ca9f2aa00251fce69f6f9476c3aab4a13f2663d7d50dc1ad14636d4e105a061ca9c7" },
                { "ru", "7b05093bdcbbb5b7c10be600cb9a7f36e8de51c4da6ec1a9d43e6c5c71b78bcc0e73fdefbc270459b4ab4188047f28fced25a10e97154de9eaf4560213e3c525" },
                { "si", "13588e9e3dbe6710dc24c34ebdfc23f139865146712575c36fd5800b8b552b4e1a526360c87abc855e47dad4f5386cb6d51c1a0d0dc7d6c91dd7ef70f8a0412d" },
                { "sk", "af217a7797f145faa14f310b3542418ee56e4294038574377f6d32512113921b1f24aa00f783faa56877eaa0c385e66787935c30c54ab43f6025b8c4ecfa25a9" },
                { "sl", "2225338bfe2fc7060e987b23728f5f4792d61866eb94455854572824c5d7498c14d76158e50888ba3509eb4b1ae592a85eb6fced2fb52dc6f90ddd1c7790462d" },
                { "son", "c4216b1d6a8cda9bc2bedb0cb68af5485349370019c39186f5fa02404ab272160f9ae41cbadcce5c0a04a1da44730906304a3308bca61f18bb4e48169a6d8678" },
                { "sq", "58341b6e28ebc70033df19276fb309b90023c216aca83c1469a8c4057f778db57c926b228a1bb21440c0e0b6246cac88959c78a14255a3685d5f88f9c765fa47" },
                { "sr", "9687b8ab4a740be70646471f17a5fb92c6a00431ae0010f3723d306e404666ab264893f151f7558e1d95ceabeedd6f84a115a118fd743bd02e837892734d6a23" },
                { "sv-SE", "e76dd5ce1f2dddbf4594440907f89677f687a5a955826fa8fd041cbf282d43218fe308f751b41665fc6f4118a1304050f884f23316602726cfcdf1c0e5dae618" },
                { "szl", "b1369807661081b4525ffe06f0a5d44a454ee57ba77f301e3ef813633013657e210913528321c3f63bde762eb47e0b1c71a2419b786153f9ef7633af111e1c77" },
                { "ta", "94c9ee61763dd1a2bcdf541013a2b58d0845e989f7cc207700420dd0bdd00e9150d8dbd40cef0f4724e675a8d1fdacda5c7c9edd397d3e440992a6a1b043a369" },
                { "te", "5ffb6d5bdefeec111f4a96b4471bd1ea25459b8d929e18d52f023304050c1c72c0c187eca91dd8e35ee2aa38e489c8607904dfb3f064afe219715381107dcf25" },
                { "th", "85afd80f141e89eca9f31d23bac977eca29af6fe3767f8a51454b82cb53b53c10f7e76efbe7886a2074eb61436c94b136333b2d44f23309f6aee3f75574150ee" },
                { "tl", "076acd730ff581f1f3a8824ddbbfa2bc8edd147eac31ddf36cdc11497024566b6bfaa460b93f83906d070292eb57c74046d5752baacf16a52174da8763d64d3a" },
                { "tr", "348359891487d1ec8f2e6a3b867d306fe4e691426922e70f6b8768be0070ac14624b7f186ec59a34e2f49dbf1df7e4474b72cc9389cc44e154f43dc7c13f52da" },
                { "trs", "601cc5af0cd46f7ebf82845ac3b4c02f7c3c3a1a764d49976e52d660beb23df5dd1f60558d836490e8ab5577b8c21d19ad3367404ac88962f4a3a190189dcd55" },
                { "uk", "e621cdca969a764f80ded66dd6a2b5b2925fad90689f12635bf24829db6a43ef6072b259cbc1e6dff5272c87ad8926ccbea8ccded56ede958bf83977aadfa7c9" },
                { "ur", "b28d9156008f30e37385cd90d40dda46b3cca62e57502210dc4e78c217394c1cae9b790deacf2ec52954b7943ecceacaf669f9d2f43f9c5a24d77c41c0cc2015" },
                { "uz", "16b1e8430b58a0fa8efc4486f3c544b6f21e04920ca6b89941ee74e7e0ee85f2f1e07684b9d1d2ec457212d6ed9dbcbe486699c241ae957b3cba725b1299dc05" },
                { "vi", "39d9134d951731ffb5a0727b93944dd50d7d347c9fd9cefe3d0f059e7b76bc66f59b7184f534a75fde794644aeeaa0d0ae2c404723f8c4835a76691588f92939" },
                { "xh", "ecd459a3a246bc80a31c16f70fd27abad37553b6767e159aa15d4ba0f2640b652e82f56425d7bd6e7585c0415c313c3e428c088d7484ccd8583ffdc5a6eb5206" },
                { "zh-CN", "3245af6c90f68ca9806836700b7d44e0bb9ccf49df03d00308c524fd03be7d0d1575151fcf9cf65e06c9dc64e94daddd89761cf25e2e188affd0ac6615619c28" },
                { "zh-TW", "d68f9ae7f6b8d5c85556b97b1c336b2b7bf3142889c9f6136893d9238a2660ac3c433ff197c678c208660dd180fa821023a839336b2e4d197e8d1ba8044d3be2" }
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
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    Signature.None,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    Signature.None,
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
            using (var client = new WebClient())
            {
                try
                {
                    htmlContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            List<QuartetAurora> versions = new List<QuartetAurora>();
            Regex regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
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
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successfull.
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
            string sha512SumsContent = null;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                using (var client = new WebClient())
                {
                    try
                    {
                        sha512SumsContent = client.DownloadString(url);
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
                    client.Dispose();
                } // using
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
                Regex reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
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
        private void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
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
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
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
            logger.Debug("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
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
