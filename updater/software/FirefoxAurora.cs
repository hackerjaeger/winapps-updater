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
        private const string currentVersion = "111.0b7";

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
            // https://ftp.mozilla.org/pub/devedition/releases/111.0b7/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "5ffa83cdc603ffdadc0e3736e75327f73511a3749a500c7edd1714fec37cbd74394d615878f9432b3e84a696b560982525ce13ad91fb769190cec288fdf6285b" },
                { "af", "5e861375ea044befc960e59648c8a95d1f46ed3f66d7a40e67c2b310d91ea36ef9556b17f4add0917dfe134635821ee56b242d6c344036985a822d9a78a24150" },
                { "an", "c76afa5fb570b7ab54c3c5c2cbf46c9017906d8534dadc9d252ae4a01d2aac313d97a8a478e999173671144fcbfbb8b5b4666340741fbdba1ab7c0f42f4f2c15" },
                { "ar", "efd81956b69652d922ec4f332d8a887ff21ef5027f9b66d8d2539a636b9d6683bfb164c77f5665f9b4e76b00157a76bb1b3fb25b36fbd89cf6d21891c2903a15" },
                { "ast", "e4a3eed384ec9c7f30395a2cd6a595d0b1d83b541fc69f75566e170435e40a61f1505cfea34adb24425aba62c8965f4ca2fe9399c59fff759f829f878713ea45" },
                { "az", "ff642a0a35813d328b605143d57c3626c65ad95e18c8064c26dd221c4c267aa1844ecd294596e33b79fb846bfde29fb1ef010813df6973e57ff93e70b0e8bb6c" },
                { "be", "7cf3e00841762b78a88785698b00d28ced6a6b46edeb88b04cc9e3ffeb78e5dc2f1fb191e676f516a540452336c0917bfd506287a2c2d63ee38b648fb0c32cce" },
                { "bg", "26898187975f0d9725f816a58326c8d83f493c2bd900244c01ff7051a6f018a8884b1df8be062a26b0998238b65ce6f5ef35826720ed7668f0d1993aa8edb3ad" },
                { "bn", "25b47907e60840aba2b5a20997430c565eca9c6bd678bafb99517de0ec3ebec363b1546a1d8ac786ddf8802c9f4907bea9a14a1ff0c81cceb2fbc86eedfc1b6d" },
                { "br", "142ba5a24c14d0ec6004f11d8dd6e29e6eb9eef3cb07189836072fe3d4adf6a2cd637b90e0ff448671adf4274e7cb3830c18d501e642de17388e2f47b0010d59" },
                { "bs", "7a53a82eeca8fcfbb595751a53dfb445d37dc64ff9e8cf4fa03430d8068aed8f7801ee456f181b57245d30dd46628341bc30c0b5fe8d8308577f7fed33ec1241" },
                { "ca", "56d5589933fd9e1ee4c5a33c1800425bc33c65116ab06b0c744590b4caba09ab4271fd91d8fffebfac23177c1b461d64ee763d1fa45f1ac649b065567a1e6eb0" },
                { "cak", "84795645032cbdf71726a3da1c9de84bdc89ae511b865bb1cba464c6c3ca32806ef82e0888dce2fefb91cf9c0e080e2b485f121d1132d9a9b914a5d9dd74ecc0" },
                { "cs", "e09932e7c5344c66eeff3df3a8c643ce36b4242951983f6045fd5cad0916b97ca62a046d6d4bf851bfe082e147dcae275faa6b87772c7aaee910d4b852999ec2" },
                { "cy", "a63bbc6ba22935fb46f8f1a590d35bfef00e334c4ff58f7a4edb384f3550f3c349e2c06d36cf793ce78fcd58248dbba6a223f480bf4c99325e7a22dffb7df9a5" },
                { "da", "9a974f5f8fa9e2d4cb7d5fb71b905feae3d06ed493510f1392d8b893c9ed52c32fd8f4e08a52773ceab99e022ae92a85c8365fab4346025cd61fbd9108e0efa3" },
                { "de", "82b50864102650aef5689f5c0406098c28c8c93e323a684733a9331618c10aa5e0c2a9bb3a68d27bab7150733760855eef40eb3cab87f3ddb9a4610379d34f70" },
                { "dsb", "d34e8be0f7cd4d160236c347d3135dcab03f8d1d87f6b5c56fcc9b4c510950b17b522951f98d9fa1d9a28d76baef12c1d24d3f254cb7ccd74fde5d4fe5268833" },
                { "el", "cfa0879615f683d276cb238e2bc203e50271f471b40659a0375472a5464c6b2d9126a93bf371773e33df2df6d685513052ab9d25e65e1732d8f852b5d40dd363" },
                { "en-CA", "d812d3b3edb1506d21d789409a35b1280004be2e9a4a052cd0d7f1320938d6753ea24f25a7e9cce0564ca161501a4561617123752f23fe4d4810407937d544f9" },
                { "en-GB", "1697803ca32564760ef5fe7c6cb0183309d1a2b1c9493fdda52af51e8618713562d9cf17091d86973df3ae605d82935b7a824add74b811a053440eb4b68d8c8f" },
                { "en-US", "4fc5f46cbdf22e52c31c565c6aa0321f5973f57dccf893cc01b037df8401da7403f86d477792dc6f5f467bd45e72752036cc241c0cdeb52ef99f405902b81dcf" },
                { "eo", "9c6dfbce4700aab4a5fa9725d0a5e24cb2cc8d09b0744ef445170985b213d7a5abe2fafea400ec3d209db255cbc1150fde557896dd2be24c6f34cc36d38e21b4" },
                { "es-AR", "1e2c00dec909df88a94c0bb83c31234e7d9f24670f8f951c6304647a848e950c9afbfac3a1addc7f687ce86782bb8c9eb89e16d8a2f59a494f8453ca4b8e26fa" },
                { "es-CL", "82d723e437dc9989c5e8b2c80f21096465e78c6fd8b70bc9e371f2afef99b7dabe9be8181c15c5c2ff39257ae0d301c0e7cbe68018a1a2168247b9887f3faaa1" },
                { "es-ES", "179939fd2d74162c31b7e86de5b1aed4079f2a2578f3484072544936b115e896edded92d1ece058a9f22b41b9c5ad1ddc1c46f144ec5bdb563ed7ad05f5736a7" },
                { "es-MX", "55ae069aa9dd0bbd0a281a1e686f6e9f80cff26795d3dbd2b122a5bd0ef6f31eb21454acad191f1b7eeb8b6c4d1f94bf3efc920c86b87984d0b34897f549bbe0" },
                { "et", "4a5494490a2ecf70df7bffbf137baa17ab4828b850e9f0f87c59bb87aa82edfbc1715081962c49cdb60f497d1af74c1fbc56b2005af871e085b6076709e13eaf" },
                { "eu", "08a6e6d9f2b98a43d783899a7258990d9db35af256ff0b176f9fa7a397a748274134f351126e0bb7a7822cdf582af54d9fed9a3c640c2f99d802ac068383ac79" },
                { "fa", "4972fd667e7dcc4c35fcb1574abd889561027a2e5aa7daff9864812feb242a405ae9615d6c686f028e4db673da12a9d97ed7b1a20ae2f98daa48b64cc925e908" },
                { "ff", "5ea44f09cbfc19391315545c26f3811bde2fe265fb5fbefa108704b320cc72fc76472986b53581ffbd46e013f3a11478c06a89aedc89757261aa81093c93d295" },
                { "fi", "6812df8029a42c6f9547254a6aa087e57c3090d10a8db2ab8fcc83afc230e79885523e7ad586127e6fca4a368058f05c338c268423907b63c9ecc72ac178bec2" },
                { "fr", "809c7bfa6a63684fdc16a44cc3771cf06fa2eec940ab800a1323888ee74340b6bb5f53bf16d6d623f8284874f0440e218967ac38a358424de7ae9286f04beb8f" },
                { "fur", "ec0bff81ee12a8952c030b4d0d0b9d1532dfeb73a768c392c283a798adfd66400d68a67cafc6c366e1f76b47b731caf964db80983ea67ba32efe14ba558485e2" },
                { "fy-NL", "91612093db5dd2f23633163b809aec43480db345b0a303394d73d82b7dc58e8d846d761c45769fb084dd1406cbdf8bb7e62f2717a73645891c4695f498974fb7" },
                { "ga-IE", "ff52a1cf505ccec5c1f49c45b170eb8d1d27bddc91cd229c5daff8e1e9ca9dda9f53602f188aa869fb5a1a2aa0b223fa053f133ccc7c83bb0725f1210bfdd6d0" },
                { "gd", "04fe34b12fd0890b3778389b01c7626c680fff1332482a0b19a8002310c47357804f202dc0d3b2898c275092914f7ddfd234bb5f8e0468a83d56baea99d0376c" },
                { "gl", "11f176cee74d660ff26c992a47224442f12a6d058d5654f94338a6a371d897d27f96f3338d8fc410d72361e3395705fa53c7b54c2e8ca05efdf6d3353bcada38" },
                { "gn", "9b673cb0b14bc76b943297da831a6c74baf9e874edfe5863ad1f3ba9844338d6e9fa748683d19e8306abe40138c6500622a0a97f18d790e8f372826af44591ef" },
                { "gu-IN", "bb42f3cccfd1e6b5f546ec75dcdb3b23b0a9582ae1fedd671d92226178c6e13d7ff7e42533d85d0bbba7428d22edbe499d1411f7aa3080b633c8ce6a0aded2d2" },
                { "he", "48ddf74d5dc9678ec69ad5e823a869121c96a1f3bef210c90116bc7fd59011bd33e96fc65ade69f2ae2379fa3fda656d3b963e96e50f91bd92659011a2498996" },
                { "hi-IN", "6fdf28a97452fefba7033258ccf0250f38b890cc0c6c8b64954eec080b3dc62dc21d68892e418a5e0f5adc7d702d32eeb42aaeaffd0735bf7d0848af3374aec1" },
                { "hr", "63bcaafda95a5c5799445baaf94d66f567febce24ba05c0f7832f2b4df09cc52abc40abd7fa77f7db770b2fa75da5287dd002c32373aade902891186b9fe72d4" },
                { "hsb", "cbccf86c9925913e0c055067cb19aec9ef0366299078d7d50d5e5938ab72dd773066f62c24e84c731b9dafc3a1663425354c0a96c4e05cb22a9b9d07ec1ca5d4" },
                { "hu", "f54a29ecb1b063ccf13b2027fc345d878aaf2dacb605c1f707fdcb1c686bd1facf0b7688e1c88930cd731de1510d5dcefd323b38842f0882a78a7db0849c9deb" },
                { "hy-AM", "16814218ad8d8913a8924b96ccce930c5c0161d54ca39794f8adba8c330f0a2e2b25b684d92331f6ef492ce442bccd2a8295301144dba3f0897f4a806d744897" },
                { "ia", "b8182113f913b9ddc76b3547a75c5f13a5f0e074245019e8425c9bd003f5fc487af293b3fa3c167b26ffc7225617afaf4975a365a9d58c582bda4280be34b2e8" },
                { "id", "656d05afbce25c225a636ccc1a94cc8e663bfae7db2ddc65e31e3c1be8b0f2ee104a949415fac8cc39885524bce1f9b9a1677de552e8ab85b11d14c16f8935cd" },
                { "is", "e9dd3d927b9db67767e8103b04121c9702475337ef63e4790e9a3461d2eb68566e64437f8bbe58fa75625e7f1cf7f4f20591bb7dce8ee663b669a5dfdabd3a54" },
                { "it", "22a66578b43cdd81be8c608759f14baae8ccc03985b94007aa1f81f38fa027ccebb5aa92c27c97151d629755cc45948e04974d4fd580e78a47e0ea4043e750bd" },
                { "ja", "6a8e7d8bcd3874f98f4406c02de21cec1fc3a06d5e9cdf92424ba7d8830215a29b6574d5d4695e4b8fdcdb6b7f7e7764176a5ce5f5a622fc5dabfa23aef6ce48" },
                { "ka", "5fe131b25f1913ef061c33f1a5cdf490b7d1d39d43137aa778e7be13917f149304b3cee5082c1f62823214c8c4826dbe0855bfa8876eb4b5eedf86652d12c3e2" },
                { "kab", "6c6b965d548e39358da331c707688733d443a3952ace5c5724febea3ed71e97ebe7739f09b02e81f6cccce40eaed8c958076f4c79fa0e409fd985998335b2927" },
                { "kk", "ced48ffc4665edc0386232ec2f3f3c17ccff3908a3780da93561bf7db47b3106ff4861b6620262f4114a2b677141a5aacef30e00335f4deaf8358c4e63795381" },
                { "km", "60e7736bf1a0e1d7769c44fd096ca2e87b00311ff1d45595462772f7aba06827f6e28fd413be47bc8f0a5ffb660ad3e3a3ec4e1cb4a5e21f7a7fbeaaa30ffb3a" },
                { "kn", "7c4944a985dcff1000a0250812f7d53fb7cbec03b14e29fdade1ad22957ae8e75e75968719860c452c9fce0bfda8e947ec4eb4fa98c1277f754d86d1fa6dd169" },
                { "ko", "edd71551953e12a14d8894c572abf1df751231fc8f4d3c79c0174c63bc93e9227f71dc2471670f8ba58c5cf7ffca1ddfd1906c3ed711e0772c8a7782618bb6a5" },
                { "lij", "824cfd4dc48be423fcbe91e42f0ebf2a2a30aba4067a731a3f97e95fee40f38a23053bfaf4e21d5fdcfb64d4b1787a888cce7cfbd1d7841e8762098659fe54b2" },
                { "lt", "03eeb5f138ce4790ffa09348925ede3bfb955d3aa52486dd135a2e94b2cb305a8dd9ff44bd10b7d78daca06f8bfde343692d7f2af7d18732224d31fd10d770bc" },
                { "lv", "91a5fb862dd565c8b46b6089faef661cabdcef87b206df50cccc0e9cacb267ac87269a29bca81e4e2bd8f02358eba9c95758b63eaff16cbbb36246886a5d43f7" },
                { "mk", "362c9be72f2d0fc7e1331e8cbafb9d57002915e0dc738eb30024a33bab3294b72f607f461cf1784f60fcfad46de110e3c1ed5b37a5628074a0be8f83813be5d3" },
                { "mr", "e32d94ce98f1bfb63964f6667cfea26b7500f0654c870b020967601ef314668d6bc00fe449420370d0b665666482d9348db89d06ebb5328265fec1656fad1336" },
                { "ms", "d709fcaed9a311bc1bbde62d651193dfd39588cf1ab714e374e4a42036465b69b66b73a6362db14d90d6f4e9caeed031c13040cf72b2a29793740dbe05da9d7b" },
                { "my", "460c856eae9facb936d5ebfd7655ddf8c77461aee6f3cd731cf5f4da4701ffb185d35298fa2f44197ccd70d5a084516a2e83241527a4fbb0b03797078847a5c5" },
                { "nb-NO", "365fd5690d1ad654e8b60cce87a3ea2535f5cee4600a3a1c75a56f066a9e1acc7a4b1fead8a3b5fe1dfa5445f3ac582c2ecadc25ca4cb89c2bf07bbd38ad1e85" },
                { "ne-NP", "e9334466679c44ad05068ee804871e716bf20ab794961fbedba7472ecdc5c45a60c2ad0264f3b9b2890ff0561933933acd578d680cae469443cb806bfb94b494" },
                { "nl", "70e62be66b1adff3e15b8d596b333ecb657018d04580deafece0b6f835d503a4780f59eab035518a570c5c8b16c08d6ab9d2439d6220323fbac68f330085c36b" },
                { "nn-NO", "e9fccc6af026bd906090a23c21b1740e4f98a37de48d80f6dcc06ed7eb1f80dbaf1ca2e4fa612ea96b8c5147965f39e79700debe550001dd01eb8e79d793f2c0" },
                { "oc", "681e46b00c053772c2cf05bb4bfb70e428798ae6e7680bac874399043ad5c707c9c0b49c95962ad3e244779b27656af88d3785e96f8f0e1468b12babb46dd95f" },
                { "pa-IN", "3fcaee6ca6ab2b96e0c13dbbced87878e331f1bfd9beab8529101bc22a4363d50809e3e3668b8e50e6c6cab53dc448a6e4592359da98df51fb37dec1a17d04ca" },
                { "pl", "bdb29f25bdc9f29a86f1ca4291360f18d86f459cfa25087ed7fd9477a2b715e50c28161029d31039d45b51b1f2253f2f554a13468497b5a71492111fca0400d3" },
                { "pt-BR", "5da6fc3b6d785e0809d79c479c1437c63e43d2e1da17d143b68acba3b64850975b3347f921e318d3b6a898d8d1e524aebafb6b2d83c7d5ee54b48076ec67cd5c" },
                { "pt-PT", "9ead928e609800394b9d25e08f693643b9874eb104f3ef0cadd0051b9b78cd0dace719fb545dbfada609ecb511d4c68a2752d006f94db1c439a7d6a80c666437" },
                { "rm", "62b1ba70b48d83a8c8094198ecb219bd47a4fe138dc1d37d049c1716d4f40faba15d86ad70f55baa0f866dd0a87e16c57fcdb38ad16fe5808f6eb4cf8d51e094" },
                { "ro", "2fa3c24256b9f39094b5fa49cd2d7e38147b36a07b82deb453d12b1ef492f6e13d3a1e9b81cfc5f8e9cc3f3ca68070869377465d628f076f21d3069a55433e7a" },
                { "ru", "eff91307e8620f31968530eb0e347b6015500edcab45aed072676da42a5603d366a1279b2dad782006e4c075dd2813d3edec671236d32981c1640b02dd3be802" },
                { "sc", "6963e709a0da63e437d5b8f8b6aab1488eaf9acbb17009b0699c7da8b92361e3da99487b15917ea157b19c8daae4c54dc078e2ff0de3150098aa1bc68d91e586" },
                { "sco", "3702ff09812b1981567b328282c79ee86f2954f7bc1138298f3e4d7288f0e47d71b682772a1bc55022652984f08c20e25b6e2b216899f42a209bb4db6ea52e91" },
                { "si", "922b510411507974fcb62f2070f51ac2e478597e66a9e9cf8ea640ef369e5c3240e84b58ca96106a79021a08bc57b6f0da102dc78951e60408ae84814120ffbd" },
                { "sk", "5702671d4655b466ab8a32fde15bb8b781dc5af21f306bdaf58e1cf89625690f42006cd2d41dca41d2b7c078b195c9eb9bd57c81c258a1d321b2ecfe9f715af4" },
                { "sl", "5aa2c88a5d61e0f54d7eaef7cf1682784f419c6d274665d4053bf4d4f9a6551d52be5c3c416261afe2c1af3d104c4f096da02cb1b08ff23064b8fe590f45c6ad" },
                { "son", "e66dfce2b32550d62777016431e092635cd2685b1d7361c5c2cf94921520c6b5cf2fc5d39a8a20433bc958c47bcc6ae8bf9ab62ffe9d64543e80a7a3809aee11" },
                { "sq", "08ed59c96759d4ff4b63cf44124efe70ef676b4157a6fb6496da0d736e165baded1669643ce07a6d7ea18bfe7d15f2d400e5b46d75b22d4162a8022cf23baae3" },
                { "sr", "c2fda6f98165e2f714e9c98579e76f2fdb7848569f580224a8c69fa78cdce8d2c39b8d2b50083b1b04d4bb2885abd186401bdba3b978b2567474721b5ae74874" },
                { "sv-SE", "9407288bde7e83305795e979c54c3804bf87461d58cc7e3122639b7a48e5a17e766b5136ebe504a8a8a7cfc97b1e18adfe5e355278f00110a9105324f4c5b226" },
                { "szl", "2a782701b0ee8ca80f3f0be79070ce364a71079a9480136569c128d2ca7761ee05a8b7b2ab03d14a1e2f737962b8d32332594610f837268008ad4a3200fd20aa" },
                { "ta", "78fbf3003eb0e92a762e6f01b448a8bfe826daf165eaba10474bdec42dc4e145003c3e2ffc17b07578ba7cd83f98af626c5ef50ba2558f33e67ddd34ca647602" },
                { "te", "9aadbe54611366b40b8a3c33fd9f58941c20e01287d47fdfa391ce941e2874a6baf39a3e2b2c79c95b02ef980be2ee61ac2991f14c19fffb86c796c6e52829f6" },
                { "th", "7abaafc689fd15e257fde5beb3f8a5f1387af5c7e9d4a76202afc6cdc375bf1fbd6a7fded0d6cf503659ee385f8b4d6f4c49faf1234e0a518dfeb3fbd371b484" },
                { "tl", "b310b0e74b321af4b6d33ae66d9cebf9dc0f1b9ce40d511b10128ce454e7b300d11b933f12fcaa28f35ce3a004fa9c85668c23e469423926a047f54fb2dbefcb" },
                { "tr", "cc0194dca7dfa9e41c68714fb1cb1c8050f0318215aa6f33875da9827463ad4297aab8a3851ccd1b12d38d5494e2ab6a2db7004af7352717400fc0bde0cc939e" },
                { "trs", "39479ba1b02a56d08d59c68d9c627c5d36ccb07bd69f099c158b035387986b22def8d81a749a9aaf6691b6d9d66e0b0d6105c2c552b2f32c0dc5661f5875dbfe" },
                { "uk", "180ef8bb679342fa6813940eb026db0831aaa55ca3e9c79ba46a8832aee7e70fba851581097e4667726ab71f1d8fa418274077470cf52196cb22bc6d97de905f" },
                { "ur", "220617ec7d522b0050cddfe65bab04b1bd6d69a108ff326ae9fcaa548040c7a65ac1743abb65f875a926ec7fbd686bd7af30ceeb6c4cf6423d82d9dd431c5c89" },
                { "uz", "94bacb5ff23cb1acf3071bd8af7feb4aff2ac81f70ea332c2badc39a69276dbc48372ae2e651d7c3f85af9ef395823b40ef67ec4dcf04bd31251cf68ec8560d0" },
                { "vi", "cc719550727cad3968506a78b382c1c088913d8404e9f32b0402d65295c36b2a44acec7437bbf22be011f20c66da7b989e8ba8d4fd250c510736a9e0ee2c6eed" },
                { "xh", "92ded94764efdd353c9a1255d30a217af9d2f0568f38f9584e8443605deec380ea71ce15d10f935a57fe51256807a18791dd9509a33213f53ddd58de32911159" },
                { "zh-CN", "8b93f662fc4b42a6a1115b100bbeccd2d1ad0efacbdf520a0a94c4712d538c2a735bd21e50fba28a3f90d88441be5c6d8f76af5135aad548129be7262460df4c" },
                { "zh-TW", "f0ebe203ccdcc85aa67b6080e6fda1dc89ca3f7f7631233770b972e10245e9d0076943b2593f8dc496cbf70a9995e17ca8a602ee3752f5d54906514ca045ba1a" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/111.0b7/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "66ceee1ad3f818e7ede883ed4bc7de3fcda4d38ddb6ae0480139c5f4cd7e1e93d3c68f42b63bbebaede4d24dc7f45f208ba2d84c87557a96e799bc8bdbbf9486" },
                { "af", "e1ebf7d6d6b57487dba0c944e4c1e32bc7961ae9d68fd3ccca9f60e71fb340553c5a9e64a96c9e720c86fd1f2003234e1674e4df8f8b270a0a1caf1ebbaed36f" },
                { "an", "7f100b98cbe77965dc5bade270ea88ee63e34a750726fcc47c3ff4f379ca9fee4d5a8309a9e7572232f6108f6be1e73a94337d0a555e7c37e03840885e70e7b2" },
                { "ar", "d3aa6a3611d78124e25f98b307e5fd6a71b31b24e6f2faad8e07b4e458788cece18008cc72f49067790509e709681d36fa5dd5ed5cee0dee895840844de3b351" },
                { "ast", "af8c67c4c74c67bbb1e8cdc359e1e441206b6f3670d0998edf070260a4a8a7c26133029d2345c18f84cb112d944c6cd3eeec33b021f4934299a1c6bf9b508c28" },
                { "az", "3386be983e9c3c3f2d47bc5c18658d0b90929e4616064ccfa2aba64dad1f531ccc6660b6f819793ca2af674eadf34480f3ab244577438bbd98a040e6f40b8ec4" },
                { "be", "ec27d2f1325bbed1573730cf8a5e5ec3ed8628ff9bd94dd80665bce8fa6c80c353ea2094a9991226f5d6a9c2e09650b608513d213ac960f4211b078eee31fa96" },
                { "bg", "79bb5af516c38b85e88e30076f44fcd13d8fe512cbf4518e4441674ecacb9e91e8d6d501b27f64a282cb63dc8861d98b16d07823698942ab73ea024a72cd7f60" },
                { "bn", "047bff1859f1379d7e173bfe62931d26fcf0c2fed4b4748182c6177db63b5f5b218430658c3b29bb43b8aa78add2f57cc38337cb6fb8db9b13bc6bf827e60b8f" },
                { "br", "9ad49c254d644f26704f339d750eb25e60485a444e0746286a50f35881b47de76a7acb7c3fc3e6c799b31b3f5dca0d64e55621b091f2c170a173ed27af2ea49c" },
                { "bs", "9f9a95aa738b1d1ada1e715cc8055143dc130c03b97586af56030c77283f41a1caf0d34e41ba13a2f7eedb9565ed3d502bb92a65e0fe287f3a0a49b4cddc6810" },
                { "ca", "afb628c23433225280a84df7960b54fdbee8f05c271abaaaff6a110d69834e203020d4cbb23cb7be23579e605c9fbdf06af85a39f0220de90058f789d54daa36" },
                { "cak", "6ad7bba52aeb7339fcad9021a4e2cdf74b3210da789005d05be647038e1fa39e5a22b9fe57fbaa8b4da89cf1cc6b5e56f959f04fd73eb951f2575035aadc685d" },
                { "cs", "03fdc51004828d23e70c515ac16c124fc4f3b7d5678b66bbb0ab35c928d78bc55c17f71e3b77b7bae60b8e3dfcfe97856c379ab851f3109887dea3c0607bef21" },
                { "cy", "92e292c73a4654663d3723b0fb0dbf46b6124503a5d14a60abce2b9584350c75574d3f6a1358a40306869a7a76674650b80cb18c0d9fd9ac3704167a7ede65f5" },
                { "da", "4a1d4ff23f722525284523625f73c9c2f888d8f7cd45a96b1e8c537ac11bb6fdbf5179df9ef16304687d79352d82db68402cac90d7e4e8e2dfbfd6ab19be820f" },
                { "de", "eab3f67561817a3bd07162cdc64d6d547a8786e0e0b7e2c56c14f9da37d02ba87d1ae2b414a7859ea51177ae65fc7b9af918cf037d1f96a92c38363e03c4e1ce" },
                { "dsb", "33645f3b6cae8921bcd194e4938979825071a4f52529c0f896f64002f4a3ea8df3cf9319733f2968e84367f6a82c20400f115368223896c71855c91ff23fefc9" },
                { "el", "45cb82529b53c07a33a3510e5469ada03e9ae77ff5dca0da0e5cd84d31e2abb6f89e069389a94f288a92c1d49bf79a4122b626035cda291996bbd55fef2a6df8" },
                { "en-CA", "dc2cbed561d96b0d839669565056beedac00a96d7fff36cd6eedd66a4e70274b4d92794a67752f9d18e38e29e59d7aacaf41fa78f23d4eddaa3f64e1b05daf86" },
                { "en-GB", "8b61857253759636f12d3f0584a9243d9511806ea65958460b74a834db8af4fa6aec9504a918bad8337d3ef75e63385cc322f7dcc6d962cb01eda5c5f3996fbe" },
                { "en-US", "1274b3da9df15921da2a16c6061833693c3b27e2b841e83901640611fe2cfb91f5ce5e396f0842605ec00e21c66309e333c0c988b88c6a2c9d68ca06493035bc" },
                { "eo", "e813133376e4a5dee7a1a23911541b5fc36493bbc62c983ee4bc237252f5db7fdd36a2793352ca3ca06397184c958baffaee2f7533543ca659707b06f5d9c7d9" },
                { "es-AR", "3d4ab3dd529ddb4598ec81446c8448cbb7d268d705d00541dbc4c2091f2bb1e79346f1fa5e00af11a4d5da941ebb9c4e4075651f509a0df5f49740425fd62b02" },
                { "es-CL", "96f5ecca7f3f7dfa803ae93de32c6a9e4b106fedb8b6da09c27a561039ec03e81bc92d7908e7f6bf343e56bb7b4fe2930a20213fb32bcdcca650695954d91790" },
                { "es-ES", "42161e495c1603c444dfb213e8830cd8e91f11a38177ef9d3423d85272a63f61a432d2eb2ca2933897085eb9fc757d0997c56b625bd809a52d1b61474045b2b5" },
                { "es-MX", "2f1182420f79f8d4944bef68259f9bc58185652c3ec6e2f52264f9d196ec552d6109512ee0e29aa96933abfcb9f7691a0777edcfdd41aa16dfaca36cd694ed5d" },
                { "et", "3d491fb16b8b0a6fbb43c2ba3b129344c99246ea09f0fccad1788ec67cd364ef24dc4694f5b926dde0ea94c9f6ccfce4b438ed468c533c248d37ecfdd59cba84" },
                { "eu", "775bed5e4c8a401325d7fb36edd1429bf48c84984f62c9e0dd6c5924e93691ec7018e61c563fc07f17edad28714ba9ad24e865ac77d4bf669ba20fa07970729a" },
                { "fa", "abd775e83c1bed2721e8e17b65c6a8fdc702853bd58260c3f96b91baceb38e99d2ed76525a9364f8ddb599d5c24c5fd1c9b91bc0ad3491f63c059fc513c37d22" },
                { "ff", "787418f0c5830023350e821ec8ac21c9f43896f49e5ad0631c770581430bdc9c24e9d81e03ed66cba84283a9eb076b4063283716dcf0d5825405aee1fceae355" },
                { "fi", "800c9e7801546a720f98f9bda77ee0558ad03fb43700d0a44e7571cca601dc676b3a23c9cf9a69e00765456fa1314091e2a9549c5b0545599b208280930dc2ad" },
                { "fr", "62f858df9170b1e1ecaa01035b557095a52b574f98166543609cc1f05c2a85909720a8f140901641c0d57f491777633311d2c3b5512cfb234525262743d16709" },
                { "fur", "0e1f3fd923861cb42196774af5b910037f2fd170f38c875e6eaae182882cfdae4da6d4c106281636a21ff2cbb669c8e9f61914f94f448b0618180a0ba667c67c" },
                { "fy-NL", "002a1f086e5ba65ef337f16f6bc73106b895d36a4ba1c29ca540e9d4af60b057110b9e8cd933ecaa37fabc552e30745cc1f682c799244a94a085f3a0037e054d" },
                { "ga-IE", "63cad78e770426909777a0fdf17734472a70b254a304be00f7829c91be7653e5f3060d575f718199627ba0f710b1cda51c8eff4628d0562a684940b729a35355" },
                { "gd", "09b4a0085c167345bea91dc1471af7b610c0ca3174eb035020222c6a5bfd2a52c376160b9f6ff5c15cffa08c620717d2eb979c3f9e07afcc3f04284bf252a4ca" },
                { "gl", "b965815e30999fa8e4dc44fc3ca19ba23d08df488f9fa129a5d3127c022bba1911dc8642721b8631884ec3a6d4e209036520b12abc9b59aecc95b153a380a9c2" },
                { "gn", "85de554b1799170bcb60f89b7d633a093a65dd0259648dd478f9e5ee3340b17a42c866efb44dd729e00058de04eaad96b6768a0fb4aa622e7139e7060b6a03e2" },
                { "gu-IN", "d21ded3239dddd894bc627ba8a93265efd3c9885fcc5cca4063b3d667b06c480f26bbd6cf8bcfe180d0dcff02471727d6e5f2d44209bafc5f05806762fa348ef" },
                { "he", "516f044e8acc39b726cd01c5ea3890a9ec4903e12e5c02a16a6ff00dacccbabe694c765e5ef03be3579013ca92c245e26ff683ebd294140d5ebf26e2367aba9d" },
                { "hi-IN", "c4c66cc573691cb378ae8dd36d95f157015fec9611f83f7525a6fe627989cf1269057a3887a8665e039bfee120cb7be787e7290ccf59a8f20899565759dcad88" },
                { "hr", "5ef9db384ab7801ab221c8c6949db0b80944eef3a04ed0a1eaed5f8265cf815453a194cf7d23372f35e867947f14509c6aa7a57de09b77634cabdf392321606e" },
                { "hsb", "b59ba2918cec269689919850b9ca083aa74495d235e48b00b53b733faf18f13e7e5af088696d84ca90595e8b7f88cf924ed9b5c68758bff5a748ce6dfb6c3525" },
                { "hu", "15cb8406a1163bc6978e8432a2c0b72057ab247091a8189794f966396df3a8c1355fab432c5be51b92f4d7292f8d336ec8c969721b411e6dd19386935f30d234" },
                { "hy-AM", "e55c990f847b5f441b2c619296abd16f24c868508582c5338b6f4b4605fcd1916e289b6bda48ca65ded24178e863caaec993ea006aed940e193768bd787d2794" },
                { "ia", "e39c2e2b9a154f68903769aa46eb8d01217341dafbab4091c84c37d41bf395e794494ff74a973f2926feffc81fd6198d6e688cd45963e77bc421e355408ec45f" },
                { "id", "c7c44f0112901310de90bcfd806cd21b5897c3fa15fdffcbc84e8f2a86630baf3772dcde54a2740f891411d2821c3bd59f79cf6447137a4c90af62ead88a7745" },
                { "is", "18fbc927a3f7ad6e7b323a37e8cc0be503c6f173cbc14650f261693d18c98031ecc56c7318fbf644d6252ecef37097ff32e368a335e2be1edab57560f3080039" },
                { "it", "8ebd4330d3f5a65263bfb44898d86344ccdcd2d960f1feba5702dcffb4177e5224533a1f279c0809f065fc1671327cecfa9dbfe1980f243b4f3a0c19394c4328" },
                { "ja", "81383c83c600b4c674bc382a2ca831dfaa6d83ae0475473da0dca825f5a49e1be27794fe4ed27572da2b5ddf06ad00ad6d47ec7154b243c58f9289e94f652555" },
                { "ka", "3f7ba8a412620ff2e5ac5020aa413172b965a0b9a6d5f1d92c2742c61dbbbb4c0af6ea24ae2a825f6fbc58bd35fe7bfed21347aa95821e859424c68c937742aa" },
                { "kab", "128c30ded9444f7387eb121d661baf9ecc7b37dc113f636efed5bc6fb5d2199c94c6247489482bf76702e4d99a9b828b145e644e948434ee005ab51c910aef46" },
                { "kk", "2d1d848f34d6903c6173fb154f991eeb136860d01ab5e6c4cfc64018d192f6cc4b246e68b6d67bd1d4d994851ce1434bb99da7437cb0713a6274efb16a79bb83" },
                { "km", "27feda45bfff64663b0c38d9c7f263343cef9a2acfea82cbcf3edd5052a1bdac101e728f34e4a0c0f6bd1d0bf6c440b34bbbd717949f0ceb8484af661f8e81fa" },
                { "kn", "c05cfeb7f7e591cde11a6e7e9f1664131b2424f7d67d431db246636bc993d69a7d66ffe27cbaa2e07792bfb18c370ac949e640d9d8094e8b9d547e66dd7e2d7a" },
                { "ko", "9575b94e829b4e1c7e39519018d94009b185ace3a50e9e4b7e9b5668de4aad0dfbe228a25461b3be6b15190f1454ea1ea88880b9586b5dd688ecc25368a262b9" },
                { "lij", "1d53c2db9dd02317f9ecc8cb3a044d3aff2c251fb820cb63ad7cf546db55d96060071bccedef06c6a40911c23cc4abe8c9f67cc5de433e4e6221d0f228aa9846" },
                { "lt", "2e1600209c821c7b2fa30a94286977e548cb9cab7de273a259989996c0adf4106581de8ac58c0bc8129431a547585682d8b9a2c6899bf05bd8b591d2947706e3" },
                { "lv", "a4dd7da48dfa7b75da859d37e051acbe5a9e18fa32501229c0fc3a3c061498e3b56e5b90f4e15ee21af3ef8a058b06c0f9d23cecfa352c5469785d27fee7130b" },
                { "mk", "2110cc8d29cfa488112bc46e39caa473cd478388cfe6b9244a3aedf29768d611bfdd6b151dd27a0d403995da0cd6caa69662ce5485c210423f3cacf0855ec2ee" },
                { "mr", "bd24e471cf843430b7cb69cd8bdec7eefa52a3517c9bcbfaba8ecb9a80efe3dfc6693bc9d1935250089c67c5487c75eef337bb31dd219593f423dc606d639305" },
                { "ms", "daa29765624b4a42a29361bd7a75a3a365f041de0aa1ed2b79581ae62f636ba1ebf074750f452f0ea566ac8acbd4322922b60d36f55f6ae52cdf4e2ee6a9a80c" },
                { "my", "9280da7dedef3c6d92046c74c6ffb55111d817d4312929c3e545efd7ef7c41ea57cea5525d74f15e1409055826130831b27b85ace351161669241d717b68837f" },
                { "nb-NO", "2038a5fd216da477f097410cbccedd0d9ff3e624b48ac2d7df5740c3a83d68d41a73becdc4537649e89d8b4e52c5e76e52a4411c82d4dfe4c713fb93555f64c8" },
                { "ne-NP", "034a434d931d84f2ad40445099dd341ec2a9d5e32899f9b1f2ea7d35865ba9f7627a5d3b3ede8254415d0931fa804a163c8dc1b4904883360ee8d13a38e1c2e4" },
                { "nl", "8a6229eba9704536062b802dff45f3b9f1b60370d0c96739f298a62b312041984f19719bea78816853cda3ed3b40a2b90c01bb62a21ac19787985a3b74633d92" },
                { "nn-NO", "2e321e9f93ecff333bc957ca80322ab60997e3cda96d37f017cfd9862a4bf92b5c881f113cafb6aa5d844656585ad3dbdbe41e1814071a0f47acb22b4f12632d" },
                { "oc", "0517102d3d76124e6cb9923a18664fbeaa5eb5b1f183d62f18ac7ffffd0c2af1a5b174b7f42920d79a489a10b9fbdce1f7afc5f9a550ae816f0e10c2132a286b" },
                { "pa-IN", "7e5bdc3802561173be2f131bf9aa3658ea60551c86700ffb8e16ad0e43be2f6b62c3223615a1a2c69abd496989474cfc170ce37769eaf010999a22626c959ed1" },
                { "pl", "0ca46e3f2a03a20809d7c2061314f9f10101b862ca390a172f1c2f27e176f21dd4893a36d4546485a11df2f111caf5e435880630305194ceea9bb90799757b8e" },
                { "pt-BR", "fb5c834e7b2a301233e9bd07a71768842c978ebff0bd53b0428df8e35edb907afef4f5e0d7f8d9c151e278e1f481b77df76400f9e57965950547ff3ba63087c6" },
                { "pt-PT", "433cbc318942b27a5bcf6a45c8537dac26058db0a7a5b8fc280080e8e4a35cd9b196d8fda7b0357890dd0568b363ae0986b73a1975e64529eba837ac5de1eed1" },
                { "rm", "d09a9bdce742ba379e47bd0e9286b803cb3fe652eb024c41f075201d28a94cc0f7ed48d5f9edc16b102d23f54d5495b64117213dfdc231045891f13fc14fc6ea" },
                { "ro", "81fe72acf34f8fb816cadb222f913688221f364ea759782e6a2dbc697fe217f935de374cf51b793686b95758e8ecf9cf20981a430beee72dd4463aaf6f3d3344" },
                { "ru", "754c0ca862d034e712b79b37d130c6821b594bc1af7173df6ae34e4d8081600eb5b4b7dad5db5ccfd9c89b66d7de72eaefcc7990b79771841f05930ef6722a85" },
                { "sc", "11b56fafbdf08dfe12b65b5bbd71bc98beeeaa6e6fe79f2960eac76b13545779f62dd90b216b26c42436dd57618710d723caf8ea1362d657d39aaaa60c3c3299" },
                { "sco", "bad88973b73b62452a1856bac1f54ba5acbe2523df6bab49a6ee5ed1e47fd8b1295fa5a5ffa4a65ced67f544b5646658be1da92b4815de5ac481b5d0052ee60c" },
                { "si", "a528684142d0a1450eb25203dad495df5214945b0db2599e9501e5fb1167de2062a75fccc7775f294410139a7d6f2d4ea356d2ef8123208e9985ff048e87dd96" },
                { "sk", "963d8921097ac683139af423052f35a76358dab08c30a2cecb7d9a03c843ac402d955073b66b7f062a0da7e60709dfc6445f172eb0c99e2d67e69d96233de841" },
                { "sl", "e3c3a4c333546f3b8137021c5cf283e330186d1dceb71960dc0089f6cf6f9854d19260b5372605fe9775d9aff70a7a4c014aaa66e79d6e43d75dee1229dedaa5" },
                { "son", "378e398100a8c5e7f3df0eaa3bbd77847ac66d8124db0a76c6e54cfd710400a6e0540bd2a13138926cc4e407eda1015ae5d64ff6711c55d0b76a82733cc2c787" },
                { "sq", "5aa81782e9933127f61174b246219dbcb0d64d701beeaffb8517f49fd8cc6dc201fdde58c326088fa462e8ad698e6cbb34644b22b02878674a0cb75905ba0b8b" },
                { "sr", "0c0701a908cc4b7266e22d60c74d7451f8daf791078a8bfa737632f70af1135681e2bc9ac046855e2861b7f3854875ca037005adbf0bc5f07d871826b1987aa6" },
                { "sv-SE", "2ba4589daccea4617d9302856b581fd5a96d5a13a957667ae2f1e4021f479aacea289a49d04a5a076d07700241b22f7cc6e1855653cd8182d1f1c1d6cba461d7" },
                { "szl", "75a752cf6231657eb4a5712c2b6dd68b42ced8b1fec780026466b0a123f0e55432c3a62bf803a4902724cd3dfc5e0e2957cd5b69e99cd75309f03e14bd41381c" },
                { "ta", "9bfcfdb5d360ff4e8d48c1f0b97cfffe26fe2dc07d9c424153d65387bb405d345c6cccaa0d19adb5621653f1f4fe63200d44022c644351b12dd9d61c34f7b584" },
                { "te", "8c84782d1dba48e47988522c970645c923876ca34d1884d951747dd3566b1d8c93d2518702fc521fb581de55cb3481773d21e02b0816e31a247796bd2db18b8a" },
                { "th", "9982a5804fd517761f749d69f780ace422c84e19b41a3b77d5b0a0cfe4bc2dd7a6e987d48a012244d424405002123090cc769d21153f5ca1e4db479852a7bc57" },
                { "tl", "9e29e49a1ce7e3c009b7766e0c3a29e972d1b8f3a130486696fbd444b4c3a9f16f29670447e28571ebf096aef1eb218d06247ec67e64d27e57b2b4ba70099981" },
                { "tr", "91b2208fa76695ae3798861fa442ba7b473863ee833d7deb08f6bf21935d94444e95bebb5b14c6e5132e130e48fb8221f4dbb181bbf2bfd8a6cc196138f024a0" },
                { "trs", "065e62d7f32501eaa521149629fc07c4735463f8ef4aa9a5aeda39b1bb3011239f1ed15e072040623f5f0fa6459a84a4dcf856164bb2ce0c539d95f0d16f37c8" },
                { "uk", "1f2be4f4369218a7323f5442736a61efaaa21e0db577f5091302a5f56aaa7584fe2fc6246c98930253a73644f47409bad092b35ce33eaeb67d7fee4133103577" },
                { "ur", "03a0211f12057d828339077a722d571759fca9a283b735e179ec3e34d0f163d3642eccabf552da5e9740f0404d8288bf28233dc6af22ec1c049337f99b89941d" },
                { "uz", "ecffbe227cdf6e823992bee9df99654ece1658bd547ec28139085ca8c4a52a98b9ab8703824738288258ec5cabf6f1b2a8ace9ec419c540bbc685bb3510b8f7d" },
                { "vi", "f9f43c66185165d562df8b1200a02d05748be612be6d81b30053eb8e47507bb0a3c6e9178fb47baad1f0e6d39f6060b94ddb39ed150bce0c0d7126d6d81d802d" },
                { "xh", "440b4947762058b13867b16fe75c566188515b11b5bf1bb3121c3dbe0f86c6b809d1a8c287d4fe533c54ae37882a9a79dbc74384d7a6c8c582593e89d50e45e9" },
                { "zh-CN", "e4099f8f4841429f63ae2f4f140877c475d3f465805b19ccfd3a4094b8e74be6b4bdc94d3f163a66c14330dedb478c1e681aae9a9caef957e54dd6966a3b48b8" },
                { "zh-TW", "f62d55054f3c28b7b7e22b114ec57d3a292f8c26e89355bb82e057fda36bd7178a4f5c3b102597b19eb1b26f9cb69d8d28767139483cfb947b7ac9adf667b882" }
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
