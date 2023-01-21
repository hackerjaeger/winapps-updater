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
        private const string currentVersion = "110.0b3";

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
            // https://ftp.mozilla.org/pub/devedition/releases/110.0b3/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "4df7e2bed558afda53e9ff6596b2f4bad803c41a5ad9f74ee0a4cdcf6377d80922f65193ca2b3bfd62935d65015e591cbd8ef2f8c7b977f2fc5c1a5a0e93da91" },
                { "af", "3abd34971b4529a1f4ca869931fcc9e7c77dbfb8ac474dd4f4735d096f0d36df7d7098323ae22f6e1f5470e730722d4310596322fa7332685f8b08310609cbdf" },
                { "an", "d912954801f1acb6c2904ba939cba25b1022b6ca6858782d6b737eb925f5a5fdccb367bb5ca5d987d1f4f1fac024954541d891c5d2d230b559326e3dc95f8f6c" },
                { "ar", "49334ca51b0389168483678f3f3655e17ee396efbe3445087056d93d31f39325ca82160ef011d819d2c2f6ed0e3b18ad3ae1520e9f82d60a79ce73481dd912a9" },
                { "ast", "9dea8241f8b5d10a99bc6e3896417729aa92131ba33e47edacc675a883d3effebcd416ec7d626f187ef0da040a5075ed411ac34618a528587ef6bff0aeb3cd64" },
                { "az", "df20814d181efae4e7ee9455c2569984ef1cf23b70cba25cf815520dbb5b9d42004ecba1cc68b4bd12ae2c233da143c0b7b26ff93e954d0b98df997605a03bed" },
                { "be", "5e9555b6481024a867bb3233b03e48d572ad4a9e9fb9f1de3c8f518c86793ca19e8616b42d31b1c028c777cc74aad2af4563bcca4ab6400d62220c7065a04967" },
                { "bg", "3ba0840b318a095073f850d2eee7371a26fb5dad0645f208a9fa08caacae25821b7398b8bfc18d0e32726914c0f1acacf7e06eced3ca224f7f40c4570c7536ff" },
                { "bn", "06d2e9fc3438e329ff7d023e6c31dc676ccc2dc11fcf98487982429cda57a1e6ac7633b4c5b112ae6efb153ac0a7ccf6bd0e3faf4479cf4295492f7e89de3edc" },
                { "br", "15e64cebd4d1571a3aa26d12bdcf09b7558f5a71d2579ca711dd25b9c8f216d77387bdb74bbc261ed9b33338407ef42378e8f1725738e70b6a8e2715510e2953" },
                { "bs", "9da071551532cded934118e0980baa36e2f29771dcbee5bed0bcb7f60167a6d0bcbd79fd2e62d74999ae1e7fd34a8f1a04c8806d1496dcfc697ef04008774d5e" },
                { "ca", "45405460e501070f79447ed4ff394231d61692682888fedadd204ea21db091c0e49bfd2c287e9c11744bbfb3028861c7f565972835c4d13bfc5d2e3c72a3be83" },
                { "cak", "ab67fdc79182e735f697575e98215321f19303bdad5a426a8475a078e177d381ae2321678b20b28128905534f44ce4fc2a23fa21ddb298bc91f2268fd6d24659" },
                { "cs", "a2d38262919f14313d4cca5cbe4daca706a3f65a917295347904d6e8508497b7d6cc1c4ad4d45b2baaa903f7de971db1ccbc94628a6527490a8c82abbe0a7988" },
                { "cy", "543c4b87cde3aa947011af49479031ed39c9b48e49bbd51b2e69f328f55d5bc2dc7df8ed6cab5d63fb56dc6080813c30e419e6425296e361bcf88f36652191f9" },
                { "da", "9f55d59631b08c5c421b751c7a440253899a371139f3824530413ce85a53cabef50b95ede261cd51a707cfb5c5f05821c6c552146b7f95a8f96585b5c236432b" },
                { "de", "b6361048f12db841c65b152b25cbeb35b17654cfe01ab0eaaa038174674d9e4b51de517d6102a9e75960d09709712706e05bad8a7ccb4da2ff38f71842050546" },
                { "dsb", "e07cdd479d400b131d2a3ce322f5bf3ce1aadbb1a5acedf594f81fb19e1218e85bcd101c011fdaa02275a42a9f0664ab5993698201dc7d25912af20231d9c5bf" },
                { "el", "35410930487dddaec97fd8b8c6bde3e6991775d45337ebe7ee4bfba298d4485ea094f38cf0fa272d1745674acb85720cf40b4924bd2e3abceedea6d9afaa737c" },
                { "en-CA", "2ce210633104475fa76d089364c43f440977660f7e2a57749d229e61d76487a1586435167815644f9853d24f9cc25b5ffd4f9217ecbaeb8b9f75da3622eb8083" },
                { "en-GB", "9cf791b3867491e0fcf4df659d32a32957b46ee838db838aaf52b8f937e43fd503a2b0f14538a8e64e730aec5ea9fea990bcb09c2fd3cf09f209e7035196730b" },
                { "en-US", "02544768e491e6f85e26d0dfbb95c2e58e8149a947141462334c600baa7d52dacbcd6cd7d6d55029a09513347dc15e8b547c4dc3a93bf71cfbaa3934a6a33e2a" },
                { "eo", "cfe8906ec9dbeca300b9b58640552febbef822bd3fe577627d7e936a64afca1b531d4c0394aa5db0b30cb4ac4919bbf2f5457876d1c6ea92fe8b5a08ef5a18f4" },
                { "es-AR", "fad1ea864fd4d36cca4c4ec9c77846b19ceb7c38626b78c83e5360fbd03ccf87f0e734c2da9d927f6523593952ed2518365c124e3d2f54a7da1039e4656ecae7" },
                { "es-CL", "b03321a3b8268f4b5be3406fac612852a5dc3d7fe248911e0f2feb0b9667a42dd9797798db328616d73e78edf8f15edadb51f21feca1d67112160233b3dafec8" },
                { "es-ES", "32741a9458dacc02108883559c212440c6902dff1649070fea692590c0da2cffc50139985f89ffca65e18e2db032814d08b6c277c153a1334c91c9ff14fb5c3f" },
                { "es-MX", "7fc23e5069b992ae31ba3d1155fe0664762e5f46ece5dbf23fc09ab282d3b6d31b3b1a1a41c1c811257084f719088755d6b251cbb07b133c14d0e2e35e083d29" },
                { "et", "549f3219f7cb36e4451b7ccc5859122f9c14f81bde6061501a05e5eef0acb8f61a90e71b7dfc5762691bf0debfa0eb6628d2e1b759cb2cd647df342ced8a8c7e" },
                { "eu", "604eef6af5d0232fc3d7d50f99e654e2aa0f641f51aba327a5d76964e9e45f3bcce8aef860488235abf0555ac0afbf185610b185f0aab8a0d92d5ad29d5e4e55" },
                { "fa", "1bb7189e999d420d14e277804b72a9d96d2249083b1c8b3eebc15fbff850d411258516ce914e31407da7ca13c45dc82d2884deca098d2fa931bbcce4fd2ed63d" },
                { "ff", "6f060f44213cb53b2ca97792368fd8a20feef94e8b387afe3093292796a3a1425838a4f44005e85e83afa1eb78538548a2e60e9e1f6639175d89f9a47dcbe005" },
                { "fi", "90b6adad86c930aa91d19f31e6e597d72d7794c713ab3c485438a9a74cb26597f00ce923a1160f9ea327a187d641e8aa69afcb9cd5b60a62bcacd74a1927437d" },
                { "fr", "8f993c6de8201528bd52360beb2145c11778c5107aac3ffac67b7aac7ee18666ec22e6ed293f61cf0628d2c78181de0ad26767c4b3e025263367eb7411c1eb26" },
                { "fy-NL", "6646ca98b6c501a6b6582e429ff0037690cea84eaa3eee6e8c487db076cab3ca6c7fc9cded71ce00deac7fcf5b5b571aa01a499ceb05bf9816f75fb79fd53941" },
                { "ga-IE", "dae8544c3b4986778f4b6f8050102c0e334f83d2026b57d0a5260489af3f5bf5506050212b55d800a9d2e0200a22e0990911f5e57f58d621fb52bb3f5b6f369c" },
                { "gd", "3bbb7f9af09e62ca160d61eae864cdf9cc6b32dc94bb4234619d21ade03a091e91e6ae78cb68d9d0b2ed329db2a305147013dbe122798993fad7fb815cf2281e" },
                { "gl", "27da82537ef559e0137418428b1bcc153e223bdca2204844b1167553de072884fe1fa4be09d56d01dcf4c2b99ab140ecb8f697f734ed25543635c3678d4617e4" },
                { "gn", "3db4b0e7bc147969f710042e7e5d4b3a5010affaf6dbd22a866073d667a1ceb732d04d01f68c2f8a3af2c2b8c48dbb8659d986e72f8b2f4b5f58bae3642d83f9" },
                { "gu-IN", "e1a86cd2d729e8d2f13f1574990f0b1ff6e4a3a35ba466d948cc554ccff4bfa9c3804e854ef48fde1eb4ee278e5f2eae384e1acfe6822548b2e41ed1c707bf78" },
                { "he", "23e993a3a0b01cb180c6e023c02412e1224998f1317b976a4208336704676c399ab652c8b31d8d2f6d8e093a755f2f4d6b1224b97f24bdae5b725de487e2cdd3" },
                { "hi-IN", "95d5d7571c4f5c892d2694f3f1cbfcc376ec5e68e9b6ccc81e6c12ba1e7cf384c5dee71aa533f9faa3546f20b7bda4132393de13bcf93afd48ee32b97d965653" },
                { "hr", "8f87a7a6c732fe2276969c0601702c34a774a96a0d33c0f9e2a50d409320039309fda5da5a7656666657b3e05b719c58a9929cb578b37844a4e43b513f4792f1" },
                { "hsb", "f337523a523f13e1edec6f3418bb1aa5459bf48fa87788f78ae71126ac615e5056b11e23e6c256388a30cf881e319b17c30182f9aac4ccd5eacc716b44404a4e" },
                { "hu", "dfb9a323eaa61c0929f8e4f51c53e5a73e21df4e8586bb95654110efd95b60cc86d2126db308b35b7127a3b9d77d13b979c7ab508c8d228a60d2cdfa373629e3" },
                { "hy-AM", "dd9f269de7a4e401ac3abb81d0be08be9fc26567e936dae9d790b64b9900308c876550d59c1abc9d20da0da3c6c82e3153ab3dc3c4403c02a064237f6939cbe8" },
                { "ia", "7b485fb7709b779055a3887c56841a3c444b3177e8744ac2b38426d9187bd4cbf6b945e8128609bacf292629bd2275c998705895c4a8afeecbe83db74e86ba0e" },
                { "id", "e98178d76133380a3059106bccff2452ad0636156f3288f70e28cf28451dfd28797b4e75dd3240c54424da02b7996537717a451cd2698876407136ad656f810c" },
                { "is", "0c2584dbf86f54d963e1ace18c6ea92a7364a3f28e34e4d0c8d2d91fea9f26f0a23bea4a1ab59c955a2c44fcbce13aeb1de7173303591601aac9759665110688" },
                { "it", "43b32140ad91c59cc31c2e2250b2f6ff66d044f69965a3d5142c69aee26049552542b06c0c152322b91f0dcb27c42231b540a7bd3999947c36b15b8c8bec1fdc" },
                { "ja", "7048998e2f56abf186214b519d1df328bd30f3f7a4423e1fc2bfe20dac81d7f09b74769eb38911d321e67b722bda482f1e5859a4d9a9e1f5b29e261d84b8fa82" },
                { "ka", "31ac653e94c939ad9520d582f1481286e50a914da72f20864e96c824420d3ca2d0245e1f533ec7b67653186eea261d09d755c245fefb7deea055397876e67543" },
                { "kab", "8b3cf695033fa1da1c5ddf73dabf7976c5bb6d888f41efd7492fff76931cfd5dd1eb89eed45112edf3863a79054224099423fcb7470ce5a86c6619f372c5c06f" },
                { "kk", "5e6b76f715b43270d94062f2a66bc4f5923153268093a8aedc88065828fd22e23bc3fb826a658fd602564177a122e915625144fde543c68e6d2fb5679d1b556e" },
                { "km", "2e679d8263e2edd0664b551aeb2f1c05c400eb10da8209d4b10d1455276b586987b9803e7e8f109629e825aeb0c12290f63bbeaf360bd64ea35780102506047d" },
                { "kn", "1747a94ca87db3f846c30d7fb286afaf0eeb2c81bfce28e3a95b7fa91e65856e3ed6373bd5ba5c883ed42935dfe08a823d29e6e1d4f0cf627ccb3d3ed257bf26" },
                { "ko", "9afc7d17f7ce7b5827086c1b17cbf418eff2969cb85377de69819e696179ad0f3b46acb8172683756acde2c85e9b8c71e661e59427abc71d5b12e3b971f80908" },
                { "lij", "0ec7ad693d60d3b9f3598a485532c2650d74a4aac08198ad8413b4c19c48f9d6693808b9abf730502752bc0e4fc9c81c5917ec1772673af196a1a9caa95656ff" },
                { "lt", "e4a3a5d9bc4eef8f8f6d5b6d3eba0411e437f2ea069225ee82201573838755e9dd146735973ddcdcb27a6231fdd460bddf9a730c40370a975b713919e191e6d9" },
                { "lv", "ca008579915d7abe487d433cbca2201ac69a5fa36e64477f7a5595bc75957602f95c4f87bd423d664f509d78da50e02c2fd5ffce9243d044215d3bc009608e46" },
                { "mk", "b44fa606165ff62062ae9ca168d47dbca8eed852730fe5c5c900d63ad4d04f2ce995e6e30269ee0a8f9e1c7f40b191816b6f77e03578ddeae3b0c73ae9dd8b50" },
                { "mr", "ff03282bc951ea5b2c9875781c4227e437631e247e03037e201f06b83e1cd01bcf10e6d947cf6854951c10576c1fed79c3d9583fcb4ec9b4e16f7ef623a33992" },
                { "ms", "df05c32a5c60b003305af93000e857ec8df4e15ed00ecb02dee09ef3d7a924e98e76dc866718732ab556d5a9a1ff2250d39be18c9d5fd6c31530320aa23d6304" },
                { "my", "1fd215ed1f4a9d7a74f8b40e123ee1e55dac161dbffc44d7a21a9aebeae464ad616a8327589b27f0ecced88e17f9cf16e85d6940216eb389bb50b1b1fd44dc36" },
                { "nb-NO", "69dc412b1ebbeca61ddfcb7f68526675cb28bbfa2e93ec17fdf04aad7cb8f14d42e9125281ff22267817529a2268134e96e4742a0f783e99cd4dfd194b5b43a4" },
                { "ne-NP", "bc898e911e0b967fbb28f1322e6fa7a2959d495cb87348b6a5f3b9770cb6182aab0c65db2757c09b5255232b2f66c36d83d9f08689667f8544778d967116cbf0" },
                { "nl", "1a7cb157dbc54c7320b4d97cd122404ec0da8eb99eba75b0ee379a5704869cfe5a411340c51087ce5072eeed77aba16980e544a5e6a8d931a7699ea41b81df41" },
                { "nn-NO", "2e4447641eed672073d69bf27dfbf188cc601d1bc4ea4172c3348439c43bb94f098fee2fcbf4c70cb137739454806ef473d7f9e63a881a7a7ef6fbd7eca61550" },
                { "oc", "957413a699f7493598be7ae640a520afdfcd86b93799b8bf18de7429bff8e00b6a9a6041a575be3f9f48f59763835b197dbbd1ec72dfd35bc5103b27400ba840" },
                { "pa-IN", "c584cd8fa6b62ce2fae2c28684bbd6ead8fe150784470228ac6956b8016dfcc00c563c1421fa5c9baa98f60d3a06e6b8192886329410d8a67f5dfcb672be864d" },
                { "pl", "0147956706681765e7327c52453beda586e72af14caefb562249b192ff6426240785ccbdd2bc6b2807d0aa65097ed05dd932b12dad23ba09b2922c140d58d83d" },
                { "pt-BR", "73dc2469ebdbd0e7b411f2d1ce30c196039febb6b24a9ea2b2b9c3d4ad869dfa5f1b3e78b7bebd1d2dc2bb191c8ad9bc83c9162ead2c92b1bfb2ad49d9cfe492" },
                { "pt-PT", "d9eb536751800a1f35271275eefeab488e4305b0bfc05e4dd0dab8d39eb7bfec188fa051b80b95377e9bdc027ab3e7fdfa578e090ef04ffc458b260770afe056" },
                { "rm", "f8b4cdfc6ad9c964328a888fd4edb232774518726e3c67616056fadc72303cc8da49b809fb5bc3430fdefdbf7b107b9faba1301a913d119fdc150c7f19bde715" },
                { "ro", "ebd04e7ad26861c66c18e56fd069c8381bf6c7095081dd99096378a6dc236ec364f9e7d4d76f2fef767c259a38b6b6d315814acb3d99f2c573e3dc8bf57ca469" },
                { "ru", "6f5a6062f3482ebc4a8aff8f371c080f76aa7b15b5ae3c6ae5cd7f10abde930c74c2621c3a1e7413aa4d212abf45d9d4f8064d5890928e9cf311085a134b930c" },
                { "sco", "7479542fb5e0be07490e6eba0f0098dd48fcf2c422a3056a628ec6dc56c9519b3b2736c9999dcd58e0409d2e79683abf95e1839f32f80743e43929acde779234" },
                { "si", "4403cc947fbf1f01ff5ca1f8cc370da04f0baa797e4398ba585f802e0ff40a78b265a5f917a3e5c84ef2f27653414e53f6b3e57855aecc058a5c4b7e7a87577b" },
                { "sk", "4e8ab9dacecc6b0a4be223ae31f3fdd51b12cc67a45d98524a17ec748c5445f207ed3ac1b7e9bd4ee9d0084f8d77ec59331531dd0763121a53f3c013502aa72b" },
                { "sl", "21c3aa794a55f7b9dd45f8345248c3f220156da733b6fec1302771f92266aaa866b08afaf647a4d1c20e2bda20ffffb63ede2beb3e6de15eda89ef30502060e8" },
                { "son", "d1476854d18d7fc73f6b33a21739cd6f33a6eddd9ce9bb4804594dd4d5c90893c2753bd8718dca9fc503e400c4a2f23bc184461d2fbe9a2f5b5eb0122d6dbdd0" },
                { "sq", "ca69a2abafc4ad9b87db84195cd9f2a8b9ce30bd663050c19da404405221d3f167969cf74cd97bc76d7adb9b0a0e8e9c3f34c3a304f69e6ace2c6e228eb2ecfd" },
                { "sr", "c601f9224f3511e9808c036fc0ac193607411957b1fa001712e57e76f43d05eb5a097709f3f39bc4dfc79a81cd7f1f6fd6d7671f320eb3d9393cbc0a6492eae0" },
                { "sv-SE", "914c3c36870069f918c79a67675b8968fd97e5f1ce86fb98d135f6f240afe2a41d9403cb442d39fe60a57910d3518e7e70a31e1cf4988abb9f64401523c947fb" },
                { "szl", "4485e5fdb59d9d8611dd2cefad8c399b16793345f641341101538e74cfd9512fdd3c002ef377fbbd2333905f19346aa5cf563c7bda3a72428436f71dbd28c9da" },
                { "ta", "3ab5994b132d710e17a0ae6e7609eb75c124db554b47bf8ab984a71da8107884fc54ca9a0942463f72bea9dff5a8a3b7a4b7b54528289bf966c29893b5cb6e45" },
                { "te", "a60e57ce583736053b14425899e677e2f7c8b41d785ffd1aa0ae0ab7568798df8415959953bd7dc51badcf65a28b93a3819339f4a366b58a70363687078225bd" },
                { "th", "c057d9558a7fe4388f062274faa87c42576be5fa3897de493740bd060590ad7ce7076e643e1899d7fbebbe7fe115bb6d6502364aaf5b19b6f2244820927a837d" },
                { "tl", "2b9a45738b3f8ab1958d67faf32cb8fa4b4bbf97e45c648569b0e099f0cd42dff0aab2330a1cdbb715bd79a8d52b0eca20bc1ebcafdb46078b3b2b538e6840d6" },
                { "tr", "0dd07edd6b8630e3bb1a05701b45b07fa887bb0919583b8e1a565ed18ccfc89dd10a3a59f8a04ad382915450ae5f74c8e0ab395bb88a9f11c14bbcc3ef06976d" },
                { "trs", "d4760b56677e30c38c7a3fcd1a4f2c3f4c57dfa0d6d4502839de3ce16a4f9b9a588371634c2c839187ff43789bec1cad7c44f71c6db69312030cc1f00d8831fa" },
                { "uk", "05eb2e8732ad27c542f0a7a828db66ed5a996a9ac9367fd594dbcccaba7397b18635595e7a4481a6f47225caee1d6f984542b2363eeecfba08c6c091ef70faaa" },
                { "ur", "1e52ba4ef56bb0c121563baa3be2cbfb08ca9615ec1b92108bd99dd6314eb92272348381b0f43ad86a5b5f0ac0db9e1ccf179dec2dd644d43ae9626467e02ea9" },
                { "uz", "b6622fb938c04c767793b9e42d8120430145b77e2b35801079d5dd27fcf17b6aff1f4793e2482145a0f6936b698a7212ce394ff885280e6363701f961afd63d0" },
                { "vi", "3d9dd2ce04914a4e14cf71518db1631942cd64fa8b6cf19c81816167ccc7d8d9edce8c42aea4a98f002770af0f7cdd4ede412344686b5c8c83b634b6bf6b0c04" },
                { "xh", "5bb4fea78d27c2047d601e8029bd1826ffb0f40215663ab39eae699bee471a5526ca78a7a188bf03badfb901e01f24bc219278f3003cfc2ba3e5d01440af9816" },
                { "zh-CN", "ca0cdd9dd4f391c4faccd5326b8007ddace7759949d5c5ef6d7bdea243f631b10aab05a414a3db8c6cb07d82f92a893a0a973c0c65793361db4f387f2e0d9a06" },
                { "zh-TW", "52aa00ab92b97b6fe452669b7fc6a65ec4065a2385465c91fa684ffb0aadf5e509cef02aa5f2090225107cef3a3e8a8908861eb58e97230552caa79b87bdbe5f" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/110.0b3/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "7b1c951a8ea7f68ac66ffad173b6720817917106557064fbc50d343ff98c3c407b2c1363734c5b0afb9246ebef5a8e6eb8d79e17074b241aa1edb78530786125" },
                { "af", "b2955408d6864a488e864858a6658294825cdd76a3b10e414c1ccd578bdb1096bac0969d32e22776d168d57c65d6479cf48b2fdcd244cb1dd2b16fa2db7d528e" },
                { "an", "71a9a0f3c6a9b282a54deb6f62e7fb48d0497ffb51ba3cd23afb17837dc00e9c39bf2b4f0753ff2c8e4f72c5593db5b7bfeae428851b6b8deb806cd46fe39cb4" },
                { "ar", "68e612338f6650901c77f35f2d40802a5c557ac31b772d21126456916ba57298dcb88b582bf321fb365c98025e3e603b6c1a6e3a0a1b47f14aa0e5179880881a" },
                { "ast", "e9e00da848c95b4f5672a6dc3da22c4834c61f43d3769db65dbac7ca0ed3a8b8dbafb4cf27a6d3ca4d19fd91fa0a0d559e7f1589a13da8ee503c0af9dd55a1fd" },
                { "az", "368be996cf4dbcf5b3f913e41221576eac8d745e5dbf3d04220b471c37f7ee4b0e47a4f3ccd20f534b0bf1fb0d50a097478de0a05e9715b96ee87c4936ff92dd" },
                { "be", "0ce9d91f17a11adadc77d0afab367a2e09936c48da31b93ae8ffe7363f2576e4472bff08bf0443b5d8a6d5a778047569f7516ddaf6ded2b2c47ed204f82ebfdb" },
                { "bg", "f684a27df69014a58cdc3307570a06d80b80733ccc8bfcb6bb66f427b1520a8dc1cf95fa9884e3621ab8898a7bead971aa763f106700a84bea4b58a546e3a4a1" },
                { "bn", "1a106f0c0a4d5713b9584c286e2018f4b2042115408e1d732f009c02d1528a1ccc271989b0584a85e06fc8dd50e4d2feccfa324718ffdb008248dff2ed69575a" },
                { "br", "c8fe18db6a342d29b3f7517c4390fef351889ed141e210d8aed5c79cac4618e6dea3642ac435e53ba49b19a4d8496799d27a653b0ec5e585bba3ed1f197caab1" },
                { "bs", "bd596c8940630ed170f5f20fa69b81273a77b082f991a400f771ef80a069cb9a786f9e361ba1a88fe8c60d7f9954737c15ee53021c360abd7f61ff2d2646775b" },
                { "ca", "cd85aecf4152054b18634933b202a9eb3cb1ab485a6fe1e783b72a90ce0eb76c1568e4baf7afd064efbc8c78bc47543f3fe89df5408bcc2d0f0b5518caf19564" },
                { "cak", "87772793cfb032080a6ad600bb756dd07699e23e105d807a927dc86aefa6209a5daf459a786a4814e311c57255ce9050beefe51450f48155312122597c009200" },
                { "cs", "0bb1a384b028ffdbadb389db852c28d3dadbf5d4bc0a96dcb42f96a7b7ca9904199ff34b46be7a0bfc382c52a917c47c5e8e436035ac4ca26234cdd0a8c3ad27" },
                { "cy", "778dd371ad499c0b3268b5c723687d5165ce9d4417203c90a42325573daa815ce4e1475e8e8393d1d8750159f96c2a9791d2622204b144de971674f38aaaec9f" },
                { "da", "57e3c78903f149d268b297c7221ab1af67a65ed5d2f175e9ec7c17119519201ad97fa2609eeb8b2762072e63dd827cc5e3f7b780ed689cbd98d62282f9ec05f4" },
                { "de", "62d9c08a30a9225d89bd17cafd7d08563be3b9b356082d144518a249ae83716382db48fe25a73da3d44b5c9c89395b98165f8286bcfc7a3354850751addfd67a" },
                { "dsb", "2ea9f8d92b0048838c47ff8d29f5facba2038169670c8377847e6ec77807904f4808210ad7041baeaf75c20668dff1231a2ff94e95081f3b0ca2fc4424f63aaa" },
                { "el", "3d0502ebf78aa9b9bdea875fd6acbf800322ec0e0a072d926f644890bc45de35cc2107a4a20698393622f7ca039ef02584805c5bafef7d830ec8c30cc418321a" },
                { "en-CA", "b3e5fc0a9f32fb9c7080a669688c8b03ab6bf0bbf6a28badb75ddfad0cdad02fa850e25f1839ec121391a5bf744ff529ca657cf49ff6d3281b9d97ff9751d950" },
                { "en-GB", "dd846f434a95831ef311f6688213ac614b1b78caf07459588f7a691eb3fe4ca29ac2ae3d51375e3538d4186c32cce7ecc25d9f78ec5a47c37ae0f35065ac52f3" },
                { "en-US", "5158ff4bb82318b9a3ffead76dc9c82c96369db034b944b497b70f019f49a1a63fccb01262d3ed770e4e07628dd9021f7daf93ac9cdf1a944d377a592ec8af6e" },
                { "eo", "8789c27fcf9740c2d2deb1e408e79346d180505de20d08572312810a5974a0efc118a104199d92d124aad9b8b635576a9d0d162fe902a68a3d24faa28d509f97" },
                { "es-AR", "8f3a7ad6e9f27cbf833f20b316ac0888bc2551dc40f1d5699ffc094b45fb8417a4a8255051fd3afac14bf8096da1615bc425ff42c2aeda1ec44bf21d818ca503" },
                { "es-CL", "8507f7edaba6769bb94489c8a0a433cbb5dab9c7adf863545a7a175a8d58264303f402714fd14b9fa599f001488eefb5a718ec6bf98774c02b223acb5a84db97" },
                { "es-ES", "54e5656e1b81ae3292dc8d21f5b446cf1b5ea0dd77aeab00af485bbe08a8dada8628d0932927c1bcb339ec7bc7b768e0b96cfde3bf99950bbf6dde117ae9908a" },
                { "es-MX", "1973ed43305765f0b1be859084a6b537ce539aaae7543a5abc1bcef72b17a5060f13408eb19998772fe4e5f57ecbeb4beecb4de081319dbc52bec54f4830edf5" },
                { "et", "330eb3d3571432a983237f531cd900169b2da23adc60e8c04529db04393d071eeee854a59f4df4ae1f16bddb570b676078e8ec2eeb03e0212720163b11923aa9" },
                { "eu", "6e4c0ef18866f384ee4469cfbeafe033470572b54418a9d78f31b8dafab1f70f6162625f07205088a9191eb2c7ea97c6c654354965b0ea45423e6a3adf86ff6b" },
                { "fa", "7f7bde3561231612ccda18d0aeebe6dce1e3ff06453e6c527f81975fa4f737c6522b65c242ad85a57febb287951f44040c996bf76aaa91cbb71619930e91216f" },
                { "ff", "89c7ac2ad9a049f1fb77a4ba275ccd2e12fb130c1c28201a9856851f4369040f874c79ff788ae1f4dec0e5e1b96df3636942c510e05d40f77202edcc620cd875" },
                { "fi", "1219a37569041be0ce41c68afdee3bd41801e8af037ea1c17b899822bd107d5887249e7cfd96530d3c4b641b84bef44c7d4d0e5c35211e5bc46b98ecb1054a78" },
                { "fr", "be86f5c2146187b31d37aba79b02233c4f5773e8b03b1fab04943bd33564fe331e3760e15cff5fd0b181d4afeef7fe1d3743cc1d665ed4da4a443d1b009f47b3" },
                { "fy-NL", "f0dccc61eb136263744fa00951d898e6213e2224c162295f99885e28f36fe4ee0c4004d2ebddc32afc73f9a79e40a3924b2c414b34bd43460b35199ae55e0b87" },
                { "ga-IE", "4353ff68f2f09b8141c4af324fa0ccebe0601cb6636af783809c008b91fa14bba63e19f3ee449c2fa03ab27b22229f7bde12ab0ca786284a38e928ef5c0ea911" },
                { "gd", "42129ab771bb0b65457a1b69caa5edc0713242d3bb76045877cde231feac2c19f24264994dfc578b00fc6d3b244bbda285a61291ba607f25012b285febfb8e70" },
                { "gl", "366ed8dd36a0ec0f1268f43abdba84b24a095c1366b691f7728b4ab1ebcbe2921a0b914ec18f431a76b5af5ce729b4fcfe3b7aed1db9c963dcd3c49e0199fdf2" },
                { "gn", "52b8d2c0471b24b38d058ac47a09d267a5818c445d8e18bab5f922061214daead066ad2091397517d9938b0cf006ac21dd0a65293772684ae2b349dfdb6facc4" },
                { "gu-IN", "c6f8d55a39e9127195a0d53bf897d96772159938ec4e60a3d2609bbe4d02aa3f0f2a75cc908bc8764194a8bfab2e41f1d6f5d3525a229eda7080cc8223a3e900" },
                { "he", "412806909ad18d7ee46728cf9e8a985bc9514329ce90d14e3d51207c51a74c62692b48edc3d2b5b4bfef91aec6b1f7542b202466a5d8766f41a4ab4d78f2ff17" },
                { "hi-IN", "4979818171388bf5b8af5d057df40788c2f0bb41f3ad82e1325cffb59c20f48659e9557634bde3f974a68f45e2132a1b855a14d66d0533304c8bf7ac3c11089e" },
                { "hr", "08dbc43e0c0c9bd8798fd26b68580088b854b6a6f3e8940302bc505214ad600781ae245805243f2e40da4b2a1f01c4d6513a6501192c35251c80fe24ee079eec" },
                { "hsb", "cb1f9bb8119dd81c3845ecde4324e89957ffe3b54e165b39df715758aca1e1baf0364fbdd6676780d941c6cc9ae96e43780dabec78fc57620ee72c9ac5d23054" },
                { "hu", "5594c8b983a5da412127dbbd1d0d63a6535dc5148914cdf31c3aaa6d194465f9904ce89ebf2bfb8dd2bc5e132fd5522775bbb42e9e238c400fb1a0c9e3008a44" },
                { "hy-AM", "6f7684671657ee85dcbc0efce8f28ec85f274c6bfc43f36cf616ecfb30de5ab9119b5c59582106bf6e68af18c5ff3406160c4c12ce1049e1dc346dab6691b51d" },
                { "ia", "21f2cf7c7c87425608cee5848597726c5e8ff1aa0f8730858bb442c2fca7c592fd57444ca089a27f838b859e15e1de266b1b82102ac5810dacbfb4988135b9ca" },
                { "id", "ef136c6acfb4ad7f9cdcd6cf8376d755835d9d96114c73233a5ef03482aa0510e6fd1ac8309ce005e51071d931c5d31a130b024d39816c528f1ae9b95739f068" },
                { "is", "410f3756c3ae7aa10978375759c936715f188f97f3ad662dd6afa53102c313e5d32219855cddd008039dc5e46d9803ada3ad8b88776c1c6ba57947e16d1a74db" },
                { "it", "aa76bf6965d75ab13007b7fc3af21a064632341c31c47327929adcad5cec4babbe1740c4b2316b0c551bb3c21b95f10550e298b6bfb7c2cb2a9888082b616cd0" },
                { "ja", "7112dfb57b321a744cdca8e17b07c94f1fa63f1075e8e770b8a5b001a4c05ed339777db12da3c248fdb3d19014d5ed8bb3a379787fb506980200abf4c5651dec" },
                { "ka", "a5cde7deac02f4c9cf9793cbf19c7f9e40b3d65486f04476faf8b24fbd95cd10a0999bb983e1b560f4182ee1f84add5627da2035224d1f9befc3212aa84f07f9" },
                { "kab", "7669669fa4594a5bc4c9c2bfcd89ebe9f20236c3cb05c8a276691142705e7d15238ce53149aadba2979a20041ce5f94fd4e4f224f0595c7e89658bdf947813db" },
                { "kk", "a89272f057f7b772c8778dc9c5a1a37ea8c3ea355203db2185457ec834b9f94d5d96f5bbdc15029cb2cd4beb4682acf4a2b734ca2b7eb90a39643fc360c9c9e5" },
                { "km", "08ae6bbc912bcdf2779b65e51ec0786cf2a0f74c8dc835d955cdbf9a7f7107774ea0f724d3cc25c232dbff54b1a46efd3dd711a83c9abcfa7c6a56177fb4f343" },
                { "kn", "193de909a1076189fc7bc93a08b1e38dc5ace668c654f8b617ff8b4736b6ee905c0598a69a3b8b9f12e0555fa39d5b7ce7e4fff5366372e8369e8292b3730a65" },
                { "ko", "7d1b1117e23b51a3b8d57de95aa917fd58d68fdcab4eed6a00ce52f542617b39558b3368e241b590bbcfed8e8684096e99ca16ad5f684b53c30b3e78fad53af0" },
                { "lij", "16d800ad752cd2db8d9b2c7abbc38efce18accf042fc08c090269fb1c9ff8988853ed4f6cc000244c661149bba3a7676d62493693f277aef907db8818b762ea9" },
                { "lt", "b5e168b17e7f58893c0befdaa54d4e9be0babb2b9931cb8de19632e8f273382a5c1cb69ae696e180937ebaf4610de78877451c4c2a3afa6456dc381b0e2d4a83" },
                { "lv", "e0d577b1d24e6e5af712a443a7994cc58e710d31df25b2845bb019cf4eb86a3660c91c2b376e266103f82e3df51c478cfc911c525d072ff19bb553ba23722c8b" },
                { "mk", "e680d7ab1d04189eab63b5e951e42d82a2f8e1e0dc1ed6757f27c15aa7763a99cd612559d31129bb94376d5c1b278b51f09927a15d4b2526e0ca28fee70921f2" },
                { "mr", "80a940267bd545aa71ac53857cf76c28b46eae3fb81774a0f3c166c252ed20cfc51e073817b3005840021758d133c537a47faa342f4c081e35dd7539cde8a2d7" },
                { "ms", "f28aa65e3654e651dd6f6f026013701bf8a81b6cb04a2eb5918dadf5ad9f2eb44df0da3d8ab450e6e9fc8bf14a5c07bb8c5849284ad60d8863eb8598ec94eba5" },
                { "my", "698c4e45122cf27069586754ca01701632c294ebfbab01f737c598213706bab90fe2c66a9df11e6490ce816b81bea382542957377c5d3308551fe8c39c11a0fd" },
                { "nb-NO", "0b7826b512612ffede409770afe769a13462d2069d359a39dc1b63c8e2e52f28131006fa2494beaaffd3f563979adba2688a515027a020c2df0963b69f4b520f" },
                { "ne-NP", "bfcb43dff0e6bd229c4023fd390dc3e8e4d4b11468a72f7576aad5f90f9691a64a2abc73b758761cebcb997e849d1d858c114f6d5f50610b66d5c3f3c6416412" },
                { "nl", "aa94a10f4371c83243bc8a7192fda66d9bcff864b191366628a6433713d38ae479811dfe9a17f271260b326208511fa7cda1699709bbcf43665d7deedb798fee" },
                { "nn-NO", "45318e0553b782edb463e553b63d13a39c614b82cc80fbd2ff7a0f5e9e5ff5d7e06c61242dc0550966424b703f98c4fe65c5d7542dfda17898027b93a6a4291b" },
                { "oc", "12d8fb683df5fefdf6e5ea91dbb5a4d6ef4d9fdb4aa7fb48b725baddd727a30f3d57bda7de49d00d53cac64b2d691b0eddf2680e95f696baa46c99e1baa5e65a" },
                { "pa-IN", "4f8b5f6f70930a6148581a4f10920210e2e032b344a8f44cbfeee986a75582f54bce7e63743ee6159119f1fb4331a9e0022245c129df555a0db5aabb843b2f11" },
                { "pl", "38f4092b46dfcf9db40ad97468bb87ffef861157b0bba791516ad7288bd96c6202200be60c368b7b8dc03e468ccbc2cb58e06790e7e4e32a67f185a8cc5e0b9a" },
                { "pt-BR", "fb08c00721623ab925fd4feefbd4349adaa556e3c5eccf83019a8eac72e4d59bc9776ed8c47c3d3067af1d17bdf2387d461f498b02168993b64a9b36ea7ae4a6" },
                { "pt-PT", "8580859e838edf39b5d0e9529fb565a07341fd056c31237edad9fa1bb329ec0523685c2222c966f6eb3b9747891a0e285fe84133ab3f92819057a8bbc74a62f4" },
                { "rm", "427c93ab9fa94aa44bd232c6704f55442e25d926eca5df6aa21b189705e236f905ac014dfbcc262f6aaa139336ac1164a61b64b8ef4b5d5133b88657ade3bf3c" },
                { "ro", "03d15eee3941dd19d95032cc363b8be27680706a58bc4b7572398c69eef010c95f24eb9ef6e0f63c80678bdb923d41d25f04d61f26046d550f5b994a543874fe" },
                { "ru", "aa90c58da2879a00f89888d2a0e50881e72b6fe648f16cf0d0b8533500adcb5f975ec492e5ebb3a20ad178e63d742f6248d7b37d979d01f127a8656fc8494dee" },
                { "sco", "db0c2472da62713988de224c5b72d442488def7d92994e8a204c363ee414e8a9d3043d4ed670ba75d6f971ef117f24602472726698aabb02bbad2b81b0da4b88" },
                { "si", "63dd0f47e7f38f1c8dd3a77a95866b62ce1daffcd89ddaa07798634a8b0979dd525f67f1aa4fa39723a1c00aade5dcdfee6adaee9877918f49c66c2d6a88b5ba" },
                { "sk", "9fa43d7f4f97f8633ac4893c4a509b810bcbc8675952b5a5ec73754acec1a0cbcbda602c7885c8adb6d91f2940d120c83b0d2ee0bcb879c868621b4902e4b2ee" },
                { "sl", "1107134a28b02940afe3a2ef560f74bf057e210f7f4c4ebd20b5b0dba0607369c485d6ec8433fea03dd250edfda183139cab3d067eff8bc4f949c735b4fd7cb5" },
                { "son", "3fc5cd325bfee23b30392b593d958ecca5998e5ce1193c37b40cfb086d32b897b5f2f2a4ba0ec9b741d724b6da1008612e88f2fc945e50c3707660df95fab255" },
                { "sq", "fe492b1c3664508ab4129cd81b550db57de82061e24ab432388ec8bc98d4ef78e4595a3c8216c29f09dacfacb8ea49e0983b5cc809edc0354be943c36df1ebf3" },
                { "sr", "4a0f6ecb1f55fa2b1813e150d47d303c1fec3fd422be47bd178c4251a7437269d7fc246976e43f3b6341bdf32dd8064291d00512a4ba47da937328e7944e30bb" },
                { "sv-SE", "0bc036985042135ffc4ccd7a32ae17405b6c87823543a1a1ca69cde9eb6d9cd7beddbf0614fddb335c63fb00983d655a45914150eb94053e6f242f672bb823c2" },
                { "szl", "ad34f8e85772d65532c2196d51a526c6e1b7265be56e8a61ef9a4fde6bd935cb1cb4ab2fd6c1f45e803c883be92a585e9cf78e07b327b7e0de7232c9ab957c8c" },
                { "ta", "db99ffc7e47a475169ebb1952056309374d46b8afe41e52adc695f1078e9ae426f0d8e5945eb8d5aeed789fdb15ef34d3e3421471efe347f9e22f68b9224a323" },
                { "te", "602fa7396c59b42de71cc1b12d707c76231fe517b4fd59df9110e77827d7cfd03968e2acef2f5f13824fc922defadb392c400e7e0ab2fa382c2730f37b2394d1" },
                { "th", "e31a23339921517b300fc7e198282a1d289aa60bf65d4bcf0c7df0aa26125d7fd160ea6758ad6a3608bb49a6d743abef4f95b290199dce63692bee651a415d71" },
                { "tl", "ad15e0f1e8a4bb0f6fd47e67ff27a06ecd469fad1cec283ab9d7fec7527fec4a46b1700fcdcf8542ef3d46136be491ce14f38f475e5975a7706b32f058145d1b" },
                { "tr", "c5229a3f1f17cb911d8c691a864857cfe0fdd1d8713f89fdb58ea4704afed2a08ef1f6819fdfa4fa54ff4326c6bf37cccc0c709d016c623f2debd57b1f9a2601" },
                { "trs", "377f60461b04646c89faf7165334a5106f309a66c623936e1073b5f55be3e615951d533e5edfae38eaa15efa03790b33f48d1a5681642d784d05d38dff97ee88" },
                { "uk", "ddb71bff01617fde3adebd860fb2929a4b9cbb6a5f063959d0c896c4c05bbc751158633e44e82d4f13329439a727d91f02e0ad64289b77518ab0e27f5b1c63e7" },
                { "ur", "f31fab4f6d4f1b3cd8e8c2960baf8ca03fd2e43a2c82d761b6c82f210c68089f530f368d1ac1672463e8c296004d5ff955c7658a19f641385bc8a70cda74abb9" },
                { "uz", "85c2a8f52dd0bc126ad8108eb3c9f341792850301133669a1818f011814ea6795e3b6390cba39886c8c54128f2fb734af8e6318d22e00820ae16a48f25499feb" },
                { "vi", "045cf3b52be0934696ae17189dd0a0827260eae3c01b02098ab7cce360e77e08aa60f22ae7ad3d49d8ba63d2afc4399efe79cdd1f668ded05dd961aed03329f0" },
                { "xh", "ef96e72c5bcdd2f008679834a1f1cb4f6ede1171df6508ab54c7bc58e97b4a6d4ea22992e4b306bdf7268dd04a74b959a547ee0326439604c7bd93623c221327" },
                { "zh-CN", "2bf547ba36c85c4f1d076547a6224bd5558f346df9cb7468a69e1a1ae8cd911cd3d737901184c3e4942e632d4129fa0e624b37734a16f101ba3fe7d5d38ac8a8" },
                { "zh-TW", "76a4e7a7e97682068f9819ce708cd4fc2a3ac2d82547728cbc0daed6a18fd7960242f1e42956819b4699f7c58757ecda9f8d83df49387d55b86dd66304666914" }
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
