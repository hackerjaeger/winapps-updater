﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017 - 2025  Dirk Stolle

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
        private const string currentVersion = "135.0b8";


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
            // https://ftp.mozilla.org/pub/devedition/releases/135.0b8/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "65a2cc28445f23724f44ba679746bd4e51aade5dd30fe026b504bb9b3f92835545df8db3d9dd85c22c4e388d7f54013a397ef057d84def02a6f2ba39e18ca004" },
                { "af", "c072215860e28d8c46310a67cc02130947875cca5f001d0a5285fccdd3ddebc9eae2ea84426a1a96f2e6d8f006de0d1cf24b1fa69c2740f64a83d8fee6e6d225" },
                { "an", "009d63b59371965ea73c553e5152e8961910bcba048dacf4b022b329b982514a6c38f560b987c8a6bd23df609a98c07ab298ec8a92469705edc7603ca146ec93" },
                { "ar", "3f091d599abac563aa2bb977941f5c9b9279d935ceed9c6c7f48597c5cd4b066c6aa31fb2ca27a987da9527b283e610c7c4109c290bb41a7c459a6c9d5d2cb62" },
                { "ast", "47b2ecceb80bd714ef42a3054df7fee58bbc5da66ca579462075b1a0a0217ab4d646eb5f5155cf23742874d30e83c313fe6404805ba2e2e196eec2846a4bd38b" },
                { "az", "c8bad2b21c1665a064c82123cf84ac90e5e517c732ef376af2202d3d115960803c0242895f7a725638d4ac047363ac21a24b638dbd95fe6a004bc1cc71767329" },
                { "be", "1dfab7714e2765035b0a7c4578757e9a8fec2c5750a699b057b75ec7d6cb4ecbd065d9546726bd940d6ccc1334795753ca4f97c9199640728161d46aed2f2591" },
                { "bg", "c073164e38fbb9dfa182f5ed23c8a84a0e49e1ba357c8a2cfe76330e85e081542ce90c389aa9d380fdd29c6401410c16d9bf8d0cd14c323c88a6cd98ef1b01a6" },
                { "bn", "11f4dc25aba72fe73f165f48153fcdd61c0d30a4f84c68dd8194b9860b67fc0ea3d22c7734eae7ed3aeb9e17533e8ca8271e15abfc636b3f8c87467586846d65" },
                { "br", "cb3f586c924569d4935b9df3b3d9753a711d2c4a9e756e43a78843819fabb08644779abf371b78afe36856478050280ccb0ad356433b63a19be6b6ec695eb764" },
                { "bs", "299cedca74c790d16ab05399a69da934f5c6df8306669d002251352aec46de5f4fd0ca2eb79501f232918c93cb5fabc7da8df569bd1875e21d0d94eec3dd9896" },
                { "ca", "b5412f5b98d7f7ff335e8b5b708c8cd1c4b2090c92b29fae3f4df942ac55c6b0184cd15d59eab004fdf3cd9807c56ee42e1428029daedd6dae231de440ef92e2" },
                { "cak", "10599d11a96da5a8c9e815619249f1ee0d891216a40913ccb38246896f58bdeb193be33ac81f9de7896389ab13d2f96e59e8e01c473d760590dd3825e141303b" },
                { "cs", "5a2fc4cbfe0c349ff3e3db4971f70cd14edb742601690b27672174200344815f3789b2b00a2ad43d1b035c27a832403e174306bd5622718c3b6b3ea9cf6bc2f9" },
                { "cy", "04b327f682b4a1778f9dc1795764013b2a7ca00b951ab9c4bf2d55e6c2d858ca456652b0a463645ff2e9c626d2bc6e160bfd13bfb979c76017fa6496b1f88dc6" },
                { "da", "4ec2c546f76d4eb071c8b1e3360a1326c59116a7e9d74ef212de2a0e34e35df4d4bc919d2f16b1c3aa3d8b5778cea5d4344bc6f94bc8024d74bb16bee8c7899e" },
                { "de", "d4ee44426609f6e2edf2378d493e31f9d5b2ce32b23a064107a1d53324e8f879a801b8859726b8255cd6038c18dc8e3f36af78d958b0096b7e7b4cf7053d345b" },
                { "dsb", "05e80da55fdfb8a191975c05ce83f146808b189467851f74528890d9abe438c3973c728cea19076141c04735e4dc0dd32fbbfae9eb857b1d124b28021776940b" },
                { "el", "f8dad8d6c9b20210d67e04d0ff5dabbae57fab5125aeadded57763e4261e7b3ad5d0ff32e89a8ee2a659201331efd10c452258fa681937997727661cc5af2bf6" },
                { "en-CA", "b1680e8164c7c138c2d746cac917e9adbd263c554dcf13dea3988e71b8531a99b2068d55ebf7131dddd3714718bd440d70f02465a033aeef22877d0af09db7a2" },
                { "en-GB", "30c9b1e1f826fa31bd6dd9f9933949c86a68b32a43e63f4a9a2110bc189fe1148d708d1b199a30fcdd889c2cd4d2e21bb4269d8e2a36907fbd0dd6270638bbc2" },
                { "en-US", "4ab6ad61495deeb2a88bd7e22194504b5c25323c245b2a34696a70c2c14e7f70b7a93ebda91df9aea23f19b1cd955a528dc916ea49aaf79634f615c3ef51f723" },
                { "eo", "5a1cba1931ab7e5a83bda9e94c834d72369cecc12e3ab0f8ab3fe35c6120b294997d8c45db2173ac0660517f9f35ef79c7ce01aa005807e14e5e10ff043a8dd6" },
                { "es-AR", "3dbeaa8d1c04260ecef1cbba62e5467442b20158ad5752a38a0ab07c62e89c4763143c2e33d2f6316ec42e872ce93485790dcc7c5163f538ccf3ebf22c8610dc" },
                { "es-CL", "fee8b7027dd8c4954d154914151bb6b49ce8a7221499bcaa4644cdc3f6ed1dbb665131aff2325ba0c5d8f38a2eb4594facd274b7fe26f08dae8df700905f6226" },
                { "es-ES", "5b291149bc882cf3d8138acdba1f680b4fbdbfb38aa8ac9cb21cab384f3c32049dc2a932c74a5ccf30682da879411e6ab2d688cb31a0e42e3918406c0f686a90" },
                { "es-MX", "04c11ef4d4f0be95a5796dfc34faa92bff36ed45d701a82c668fa3118b358db19fb319d29b74560d0ba2bdd997d14005baddf6324b0889fe2c82bac947429092" },
                { "et", "730ee42f8bfcfd2695c2b06c55d6dad247da4ad17a62e45d808e70e608480642037fefb0014fac075804705e17c161579c6086976713011f80d5a5fab923306c" },
                { "eu", "029cce40a4f30d76988d4d5f423fad1192290a58b3e0b7c5da446617755c53b4e6c6dfc4815b4fe2afcb60347623581ed445c145b58a086f2738e82c8ec9baf1" },
                { "fa", "549db1985e9f5b3c72da55646009e6daf8d1814368c1720130029adc37108dea594918b92c2f26f50e5784b2ca7d4210216639869fd906a436616984af91e6d1" },
                { "ff", "015d95d127c4f4c3ea6c2a436ea0c36c942b4085105a67ac7519999d4c7861ef6fd06846c322f57f5d3efa2596a0b15fcb78cb00532dee04b15ae34388e58554" },
                { "fi", "e497c6ccdf2bc137000885fc9ad6f1c9eed581f45427199764fa45b7cd7e20ea0dbff2baf713da7686ac3ad88776d9783302cbd3da80b40fa1e91958ff629409" },
                { "fr", "4feef91640774d03dc2b06de0694f8a4a8e91e73a7e9166f22c187de4b5259c4e3dd09307f077ea3ae86805431675cb47ae4cdbd539753cc234680cbf7312640" },
                { "fur", "46d75832bb71de155368a4b922135f6b4cc8097aa45da4ba6ac2b894db225b8ce259ce7a6f768d36d7bd96cad2f8c17dd5532123c54356a3af61a66feed0760a" },
                { "fy-NL", "bf630b12b0d73f18305718917acd7005810f0a182a15edec305b737090efc846d773c07d074a1caf8b79562cdd86cff68c186d253ae4c1c278d6e1f1796d3338" },
                { "ga-IE", "dbe4b8356ad4f09ebb99d54982710a88a89f31dcee977f6da99a53babd889beec8515fccd1539f76b1ba49816f54897c42578dd909dcab0dcb91dec991a0886d" },
                { "gd", "1553c42686363945a5d590683326b7a04978efe465b364d00a8f494f2b9961dce7cee03a5c7c53fd49cadfabf39be490d1e024555579ce538830cab2b58d1003" },
                { "gl", "8c8a2cd8d15cd77e2f072f0b8d168ed3ca1938c2121d88e4a970b0695a89c9d2718aeecace05d3cb09f8779510e3024ea028908bb2e359278a2346c89266d18b" },
                { "gn", "ea2291c1bbc0bb6e3a9d4fe98f029151bddd888f77ef91663ae0da30d05f35b8a7d0375f86c3e6292dabc8a59f48eb3bb38823201a350f08f301e8b39cac21aa" },
                { "gu-IN", "2dc746d72fb4a6088662743e808ed9f31ac4b2aafc6afc69e26f34bf7792608652b7a828d2d0e4a35de6b90bc14824900e476ae625281acda7f05f5fdef6ce19" },
                { "he", "889c2e259d857737e6fda0d04fd5fc46eecef6642a0af437b81c16188f74fa73bc7d19efdbd00ae72852f8363d1597c609c04627ed97d19b3c866eced946baf8" },
                { "hi-IN", "c6d1200114fd502eb906c71918580972c9915bd1a735eb9a641261ed9aa6de8e651335c4ad38def08da597f96b31cddc94b0d219bed8d4f12106027c398b1614" },
                { "hr", "621bd24e9d2e35cb4bb2a16257e23c8ea3a6c300c4f56c8632849a4c57218afa87eaa78be3111e327a51e4b180b19fecb2eea84e90883c8dd591587be72159b7" },
                { "hsb", "c43bd58340df24917aed8c9e50f2970d6249dc0caa2fd13cd44c3e3d77c5cff0ce73565b64d84218c5bf6444f23e2dedf16ffabd381a82d853d7ff623f4f15c6" },
                { "hu", "c4209b4d745d3b4aac65fc8d89582843e01a16f43c1f477443eb92774fc15399d33f98e9fdf0f3262a4e68ce225d2409582bc5d22541550cfd399a2c7c7dd887" },
                { "hy-AM", "321ffd5140eded6caf24dba5e2ffc241c8e61782a7575abeb15059791169bd7858abcf746a07ab7f77a51bc3d5e6a407740186189bc7299ac526106d7227ad10" },
                { "ia", "482c262e8e2298898b16ef3943f0322d0b69c239ce2870e2550a62f355411876098f285c7ff374a4ee7c46c6f1ddc79535632f240aedb3d2d056b8daa83f106e" },
                { "id", "29fc65aa6a26bb91a0761a6a728a0d01944f360622d16b692c1faebba1800775ab66794783e20b9120da8dcd0565c06297ecf3978363dac0caf715705443da85" },
                { "is", "cb43f611e04105b060209620667451d8b3b721bd2e61919fecf38313b45455c4a9ab7dcfc725d39c302361a07ef3f3f718eee4b38c72aff6ee1dfa3e59764090" },
                { "it", "2c103cbf5b9ed85a7fac145f50deb244bebc0f7d0df8ba9d866b88685827bbf5a06e3b44126e9db29aee93af0fe758388075ef9b54c23daf6019e6046a0420c9" },
                { "ja", "1b5b2eca095c9626e77f3c9840cfcaa9203f1dcf3f99f2c00b72282ddbce885672dc502ca03771374ab0f2cb8fee80612fa578e496a156d81413ef3574be7adc" },
                { "ka", "09d9c8f82e96bc1f9cb512d24b0e9f937c91bd5f35f4900c7604b6fbf747bc72edc7ac4093b02f2ec7ef05cacadd64023c23180be46f6c85d8f423ef18fa6206" },
                { "kab", "532df319eac0adb943e8ad619693c4123f007f6e661b6ed6d2d7d7da07e23df9bde0f11f043de160bc32abf9ead4ac1a3eaeecb824033096c950cbe0dc298028" },
                { "kk", "6f1f69bc08ab8273ab2e9d842ae5bdcd48e24176ec925c8524a8977244fe860b3c601f720a143a34e9dbd541cfac54e22bd25e66b0573f04da052c7598054a34" },
                { "km", "02bf2e13c909667ef5df3c0ab0fb11e9615464693c6abdc34a52a45bef282c9e7a9f4d451fc5b4b96576a8eaab30133867ec6074cee95abb259315d5a05a080f" },
                { "kn", "04cd7b6fe393aea0a2d62418dbdce975bf42a2a3fc66361873934a39923513d28ab94ef70a21fc5550cfc276c394e7caaa0b9a7c8ca2149b94ba2a4c998622ff" },
                { "ko", "6a350279852b0402826f10f68302bc9912f07c81ba3b5e52688bdee6b5bcfed0cf9ad74e080a591478cc64adb7f322d8c788688a030ba392ac69fe4cfaedf29e" },
                { "lij", "a6ed043051a7780d8d4941828f7bdd13248829db85d5da6c4c8c4922aa87218ff57d348e257af91f44c3f1ffc41b35f58af9a75e5f15b6594d623197686ec3f5" },
                { "lt", "ed8552530efabb0847382e66ad76f917a8c9a19d6fe32fd7b7a795217172dead69814579ccf31a65658d37ebf53ad1343b338a03099bf3c3b534d3ce1eebee0d" },
                { "lv", "1dcaa5db29d190bcc27fe06a8466a875e039971e409243fcbc34a0e8344f48f5d8a31c7f9526622329c63cee702f48aa9b3b2dd6263c235adf9a04992e000da6" },
                { "mk", "88bf98e40d3417b3fdd923c1dd2548c9539ea405720f6fe02771daafb0598176ba83f1f4c896cc6fc008990af31989997eed1aa9659e8d94c009a89c198f1457" },
                { "mr", "305c950cb78a9694a6d44f13744267f0b6d0d06fdddb50042245c157efee9c8278c9484ca8c01a6e841cd6741c9b4d669736a46b8fca08414f54a421aaebf0c0" },
                { "ms", "fe6a9fe8595c5594b766ab9736f31169a1d9ba3a5220ba78c8dc53167c79955821cac311102aadea98a22352f432764be51b737acb889e8c3e158cf88e1a0c21" },
                { "my", "6fea4ace59f505c64ed5d977735247ceeea277c508232e9e751d2c7aa602de530d924fb85ad2e28de89f4dac3895d8ff28c8b326ce0a154f11d81a63eca7f868" },
                { "nb-NO", "2050290bc30850303a49d382191f2e8dc0a8c6104a63e53f08a36d50445ceb8be8f4ae02b4274a5730a1c88c6c5106e14463136ce6ac182c0ce620025ca9f7c1" },
                { "ne-NP", "83564ba158e63b168ce523f371fbc275b94c80ccb82f5a35380c265ce392b04c312fdd76aa50fe93502dc946f3c2e367900ea73c5031f37ce2dec158accd5347" },
                { "nl", "9ad25e8ab034a9ef29369d7020bd961f0a7fd2cda4678861c47583c9649009757e9801e35c27b1bd27cb2bd126e419894c20bda3882e0588d846dd4f6dca85a0" },
                { "nn-NO", "f4e2c095b2cdc8f88c3cbdb4b9f954d489703dc8bb042d74a35a112573a2dff187905fc153505d05408bb5920bdc8a3d7bfc93a823ba5fdf154c0127d4df410d" },
                { "oc", "2ed84e3753b9a7753b5c3aeef89eee93ad7c98981822029b13cc71ca94f7096a1f0ca392719095ec38be40fed23c35a50299db7eb23655ae51a473790a0906d2" },
                { "pa-IN", "88c1ab7466ace0ee81d827919f29d9869d2b4d151b4cfe17e4bf9afd3818d87135bc7149bac9f11d2dde0ca3baeb8525eec5031b3bb23e82fa99bed40dcf0df8" },
                { "pl", "3b6341aafeecad86337a4a14c858b5f26d9d87236c9633f87ea295da2befcde17a07791b560ef8c7945aa88e0244db0534ba35d83893594563fbceb8a98ac450" },
                { "pt-BR", "5b4368ebd72a027741f7310bc24c92addbca078716adbab6c655bdb6e5b2265d38f6eddba67ebcd62ead706783c196ef8817eee93d33b5b2d6f3324ba5d33802" },
                { "pt-PT", "508f9a11c1226641b36ba9903c5c2db4a0cd23b8cf9eb0cc3814fee7289a6f0880b2d5e41f30f9609aa0181314c7241d4b3860840e9bb6979d3f3924cd1297b1" },
                { "rm", "09bdb53e46da363d6b3768b8171c75bbe351f16d0d439858dd960b107d1454a65faf960aac5cd9f779cf300133dbad5580ef8c57ceeae4824626eb8cc377314e" },
                { "ro", "a29ec0faf91d2038e9897cb18f096c3f1c6e4518a91ea986bdad7dd550eaa4a416e0e4c797ebe6192729f3da34167e68156a5e9e2005228ff921c3f5962845c5" },
                { "ru", "1fba2554f1d6672e2074cf177491e8e383e01875c37a9653aa537d5101b1d3883ced693b7255abfe81911748a96d3379fa6ccfeee9f828301bada7e1056fb13e" },
                { "sat", "157517c289bcce24fb367a405f41c05da1bd139dc372261cd5971b8908e6a5b629a676b54ad042365a4a46533a964ee17ba8ad18ca6bec634be2ee3ec19deb01" },
                { "sc", "6f4f052903ee5aa972b72db308bc6066987761f5e386315e22bdb8f73f2fa5419a6be20ab6dd75f653bd87380549190c2508a7dfd711f830032903b26ea9b9cf" },
                { "sco", "d80bc5c621f5fccb679de81b8a91a0772e90e9c2608bffd6c050c0d5f505a8b58588fe600b3dac366a3a93969cbec1f12c564de72166d6af57f96c3317898220" },
                { "si", "b8d68711d5767208d1cb218c5a7a0ba5f94795f4c77c222ca8a567e1c7632d6d7d0ce4f86bf6eab052e60dc6d1ec48ef9cf9bced75192b87546f08db66297b5e" },
                { "sk", "09865fcd2ae962b856370b299ff43dd4d08483bbecedd0c9b59003ea2f7a8ab929c5af52c116a86f00f72fdc333697cec18efc92ef8a602457ba38f398193b22" },
                { "skr", "db568c738527cefd5e41c90b611e37d7f71ebb469edd869134fe0c3bbd13a86c8d87af5b461213272fbd8794f40ee685d6e7e8219304c6144c4baf003d76b72a" },
                { "sl", "1728a0199653748edeef4dcbf9e3d75904588b6266dab2a1519f73bd2facb015a6b5e3cb313561d53c5385adc4340810dbe6c069d3521b63ce379a679ab14610" },
                { "son", "ad1851423f580fdfb3d9ba1c7bac3018e1694e318bc54d6c6d1da2b5f6332d2fbd2fa9a2be8ac4e292407cb6a065c75623283f5081953ad6a15d00be468cfce5" },
                { "sq", "69ac49a21f736a9015e37b6599106b1456d9bbee3072f8777b17305bda3a731bd10b691c4aa5767c8a15a64b665c487b34bc6101372e5e30d37791cfd4152293" },
                { "sr", "2fdf0c41de652dc64652be6bb85ae4dd1f89bf268d7189979582dea3dc405a619083f00e268f809c886bdde3bed02f23130b09fe7b4689f63e81b996488aa730" },
                { "sv-SE", "ca6af21a4bbc433f95d14ce8fda7481f54e0e59d650e8efc37000b96faee5008c9a7a9bdd137999e64917f3f1010cc96c0082a4876a8d8ecf8a4ced0407b0837" },
                { "szl", "0cf02be6b58af858f6ad699bf540cb6a9ef651fab21022ef82ea3f14b91b3f661958e5c16d31d5df64593d3077bc83c6bdd069dbbb0abf32fe2ee615065d6a39" },
                { "ta", "a0b4f107dc88f32368bdd4451a5c972ba39e0e5681f434297d9e8aff4b6635d6fe6822fdaaafb53986b0b6a010b4f21bb126aecc9ce28799fb01da4afe6c060c" },
                { "te", "b5c6594e29f058b97160b9674c33f16c4d56fc0120844d175d03365bd74a69140c383de7737028e980369d11bb36a855fa4db0eea4ba02b234464edd9ec8a4db" },
                { "tg", "107d9bccff0f2b68271a953ba753f80b0775053eb5ebcb2f08afc8b191d3cf1555f9dcc1ce1df4933a15251ba41aee9a875724e02bf55d30fac78b8b9df59e05" },
                { "th", "3cf9822f0853f28312f9c15fb5e4837e6c407b61c68be83679bb282b07ae357c711f35c2c97a8341cfdf8fc1575663fbb5cb490e1d380716c8beca69480a3dcd" },
                { "tl", "451c3f86043baeaa0eac6f94e308becfc8f694d38349959258040f3cde39efdb0dc5d17408dad69a6d637d70f14388f9d7d39d7cc79912b7d32c24e90b735acd" },
                { "tr", "7d98bcbdd8124df9b65c764fbd5e5ee79930e485299ee5fba546fc0deeb7541de2034dcfab187668cdeca1c548cc7fa3480380b0eeafcbb31ea3f4e3ea8d29cd" },
                { "trs", "0519cd036f5b6aba6aa335dab71a21f364d4bd4b847f9d0b1f7f29681cd628eaaf65d7f0bb9a4f2ae9322426d41b0477332340e0e3d874293fc11f70c3a88332" },
                { "uk", "cd6c730a28e61fe152336570d1c79b5fcbbdf03018075679b8057f55b2e5afab51b1b8f2af531dba720ca1931d19a37d2c10821e069e680cf4ada38cda149b16" },
                { "ur", "6fd62305240cbcfa7b5dfa27bfe9328a1d725787928b7ce7707df68b1824821a7055e9ff59b41814bf7ee9baf8314a6c8d8f5347303ecc84780b99e18f889e73" },
                { "uz", "5b1a751fef0bf993d0ae2dda8a1a21c9a2a30d61dadeb7d07d72cf300d8fb5bc974a2e01e5b6664e560fa8670c9f696bba27ac79eeab786d95910aa80b8c590a" },
                { "vi", "0c0e9ce21a06870927ad3671b77ad4398cfaa76fcb98b322781834d7fa2d13dad9e590d389796c96387a4a632a21e20ea610e21257702c3eddbb4e057f16a780" },
                { "xh", "16c209c3a58d196af06d8f649600acc9f400a2fd266932a77b09968a285d29fbb7b7f76259e6939defc8fe26a7cf7ef6750d8a8c2135469680602d1f1ab25c21" },
                { "zh-CN", "14ca463c9cdbecc4830a7c0692328c71eedaeecac7f0693cee30555c035148e2a04de4d9ec05d8c99d4a5a7894fee4ff73c752940c47bc001c1992fe6a49938b" },
                { "zh-TW", "ae4e5454f8beb4ebf557f43fc1ae469d47a5a9587003594c9623ce15b39bd1bee7fb1f154eda4fac465a5434934479939dd8fb0fe4623a4a9464a6303d428f1e" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/135.0b8/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "b9b78485b3030326a7d9d1c6b5709b3498ace801db73985b46f4ea210d6e2eecc062bac4a8a9412f28c9e95acd1d9b34b5271292a6150c44e0cab09e99d75bdf" },
                { "af", "b062ba8d45065708919d288765b5e34e40c4cae11022779cbf500970a63fa8d90d03fb03bf44faab5f7d1f37e2a7d60b8516a01f781ce8c27279ba7d9a710b4d" },
                { "an", "5bec6342d05a738e25d1ed2c87ae6479f36e4a3022220d2e644c8865db4c4d5b63bc3d4773a126891e1ce61f564e3715a100f63be406b989bc7eaf112ec768c3" },
                { "ar", "597ec4515211fd2d81c93bfe6c10cba3048630f0983052dfbd94f281e67ad2998ea72f0e04e8376b25977577858db0507e54abd09df193eedab708344fe1a5c6" },
                { "ast", "ecc79b4053f576141503c5d413044d541348747ce663cff3b68c6cd3fdeb2c2e3b0ce9ae2fbc1cccc494eb8dd34a0391c8a9caca7bb2fb4d1844d56ff80ad2de" },
                { "az", "14f81a35e71c589ffa3514f3f170e59d93eb3c5ee2c7942e2eaaecd81d9ac53bbdd4e6a014df8106c86f4f00045222b0ec02b06fbe694347029a975a4d6b1e4b" },
                { "be", "d4bb93e94155b75c951aa523aa598928dce19165826b2cea2314d83689feecb5e0862b6964dcd1a0dbc7b0205ebbcd6f48e9b7dbf095f46c764143c8f3c6b99f" },
                { "bg", "b2c3338fece60f37999c1a543960b87259a0c6fbc59ed791d31894457eb888c20e415911ddaf454d7f67623deceb9265e8c34454902a1c7c2d60795203e7c84d" },
                { "bn", "893488f03658e780d3c3532ba377c559a695efdf9fa4ec985ed260f286ed44265a00e53ce7ae608bb6a71990e03579f1a3aaee5a268b6f2fbba547cc16aa93e4" },
                { "br", "599e092dc3405a78cd3bd890c0588e1d37a4274822411272ef33fc9698b22c0455f0e5984f7992f8386c91c97588272ebd4fca23c29026d9990493c6931d5eb3" },
                { "bs", "13cdc6705e34a5a498b54a4d0382dd6f93680ec2a1ec0c500f1303a8716fca5595464d0d2dcbaa3bcc6509da2904ba527025b40af659e7937acece437f8eddf2" },
                { "ca", "4c2dc51570a4f6ecf751c5639978177cd9af280b658743e8b7580c637121e9e42429a628b92dc7da09739a18601b2e9c5a27af26b152de30a1d080d31ba311f2" },
                { "cak", "d60ce93afb5bdf9606ed0827e54d81fdb13def095fac250f94669e5dfd4a945a8ad46c316fc30e9bc14a6baa8dea14f5d36a443dfdeb7621670eb71c109a7ebf" },
                { "cs", "c0df1b88fbf7ac12c900357b557b555a9dabd70f5bb5e199551a83d6f936cad169bc09df73a2422eb054a17affabf59e620bc9988cbef42888543cf881522a97" },
                { "cy", "fdfa5b18baee747ed9e9aab67fd5bc7426f2f9d231e2392db00c30648603793e734a62b2a01446c967c9d1a5a4679eedbcda277b750a9b5d7faac817921ecb67" },
                { "da", "30e72d5318e7a10c530b9fba112d423ef3a289d7686aa7ef38b967b4e7f19e82e8ca97cd0e569f32e9d418829ee14e4c3c1e469a0d98aa3a71fb646d58338ed4" },
                { "de", "3bb3a3947152fb9d4e3a9a5d537d214cacb0e9878f515772a99c1016f2ce699717b30bacb02d0bddbe9b1cd0fc02d7e3b7e8112ed975ed7992980147576c0628" },
                { "dsb", "b92b652f218ff0fc89a6d1f89e8b0fe361781ce4715f21daa4b7fec2258069a217f309d97358d59eb2bbf42ebbd92fed07a1675a17fb92086e8344857f90707f" },
                { "el", "a0d5efc8bcfe1ec97616be3c1735d835e039b42e6684ab19fac87187752ea09727ccb3ed814f757d9a87d09fb30591a8d7efe78e2c9d670b80749425b39dd6d9" },
                { "en-CA", "0688e960f772130955e542bc4fa2ac11da7d6feb2c80bfa23deeffface46e372a221f9bcb5757780dbf8566770df688793458d6e0fabeaf22407853cc8791561" },
                { "en-GB", "5523a206ef5facada5c419b2b60fe725ad96f0b08e2fdef3d4ee6962c32dd6d61c20a8f3e7218e6442f92543bbb825337ca75bf64ad9d1043d847a644102cce3" },
                { "en-US", "eccb1d466f5926ab644d4eefc7b206c92cf5f2adf8d71228cb8cd7e6373e2427b22b7360a75856b3b8a061f1fc9e55f1c13e02e3e1397a64382e5b80c46de308" },
                { "eo", "13bc33bb03d67434fa926f0bb6ec0987cd9f07ad159411973f53c665caa106a892a1fea7e35bc9cf5793a1b6f74eb5ebec71c29bc9603e7a4dd1aca41687036c" },
                { "es-AR", "7d9db542d8494210943f180e7236c12057c74858ae2974325374c3e286631e01441284f920e6439dd6ef5fbd76ca81b8c1ea21f6f0957d240ac24f9d10ed9422" },
                { "es-CL", "a0bd2a04048de104690eab01c0d7805e780586a6f3de2f048a57ec259e70a8253064878918ffad6450a572d9dea01d6c4e0bf9359f8fb6eed21318014a450f5a" },
                { "es-ES", "20d993308f97d46cf23f789ca52d13e2ddaf23d3203937adec1da5b4b0c1b74c8c20f157d9918af9ce7ee65fa7bdeba68fffd6567ad15430e1f4efae355fd4ee" },
                { "es-MX", "9cfbab250bcd90396827d0cd3adee2e95dd1eb97010c562bc24a19165562dc488bbb0e93134f4fbe6ae799ecc01ec28f7b8ba8ad3b9da3d2c0ec1dcf751b9959" },
                { "et", "fb405e892072c0795b4cd0f570a62c58c333c22cd5f8364c5731fc8bb9507f88c7282d18f7e9a7cc53247a0af5ce8dffd10fa36c8e12cdec9ce5a4644c2ccd0b" },
                { "eu", "55c93ba41a50310466c0f04d9eb8e450ebfd22b62f49e786df538b38fd660281116989c0cf45e6dcc1e07973c0ed5f765794a0fa57bcd5484b1957184eab69fb" },
                { "fa", "47837ce630bb9aaf350cd71ae4aeb23a4aa8d5f497ce64d7dc0b415e7a0eaa4034f0d56e1fc9c745f38bf75151947b536891ba7afcf2cb280aa9341cf52a41c9" },
                { "ff", "354c98bd682af598e23e4688fda41f701a2658b03921f89f7bd02179c1ada1f41cbd085bd98c49d3bfa9b8073f54a17501f9d6cd9c6333c4bf60f4680d9c41ff" },
                { "fi", "cc1240b266995b7c176320174e0cec4dcbc77804aee9b2db85cd792ce9af305f8ca9075e0f4d9602ccb69994a4cf4de3e41abd900077d24a8fa9fbdc999c3d59" },
                { "fr", "75122ffe61371617e40f154114d3289daa2b66ebcbc61c9970ba24c219678478927c5dd2d24abe655f6e74afe08919dc1b62ca86364a9e5e1b1b67c302447685" },
                { "fur", "4ee20d88ec81d5ce339150eff53b30bcb1fc2ba374c33d439a924d36966d9159867e17f375acb7dd76672bf8bb3141ffdd9cb3fcedb1ef69ef423a82b36be5a9" },
                { "fy-NL", "f9298886141469340c3b6e61d8148929c751e35ebe3462d861bbc873f4fa4d59326c53ec4aaa68e0dbc29df33b29de64b9b7549bcad7aa121ea8ce2daeb5ac55" },
                { "ga-IE", "5cbbcd33cb3fd049a00cba71c372ca5fbb277f8279db0eb6c5df63f85871931534c1dc139ebf651c53ec69b00e2ab6f43f6d823be199f888dff27804fff358fa" },
                { "gd", "0a1661c16c88866dcc284918725c4ba555a95d0eb5438111098d9816e325c392548bbb28b328ec300411c42fbda877f56be98975b8864626ed5a354c5eb4be36" },
                { "gl", "17a8129f0c1648d3746620220189ed70f0150a1e3eae25ccb2d9905edef6e7c8557f47bb547884735f2712be9ff48fa3177f688cc32c3280f29f3727194e7039" },
                { "gn", "c50bacd2b515e8f955f021bd2c7b54eb191734a2136d32ee53f6d4dae87e2b1813648b0ea2a525761a5eda8c775f43270fa512fd1b157b1b83f5fdf723ac733f" },
                { "gu-IN", "e1f95a2d670429f3b9de07d82cc50062a65e33d4341a32de4eb0582454828a9637bde8b3ac385419339cf7860fa8ee4dbd3519e3f68774c89e85d356b886334a" },
                { "he", "138b02ed7decd15f1aa03465cb805cfcbdfc23df0f2509b794146bb716e6a2929d0add0784521d259709f54dca73a58b6d8c743699684749eb3ff24c8b1aafd2" },
                { "hi-IN", "68e4d75f95baeea619c3df198f1b15e377b6e71b3a962b6021470400936d59bb4b73533b2606599d4edb34305610642ebe10493e5923879369d57a896386ccb5" },
                { "hr", "0279aae4a510300fca7b47abbe9c32b2ae170687c42b02edd9567e2e0c887e6175f6a9491d33eac5814fec8115580b4f1689e9ecdd9bdc86c43a76a27352b3cb" },
                { "hsb", "cd174f847dff8c19944d09267e76d85ae4232c3876b58dbe0aaf3091bf7727af862ebb953e84d65fe60c4252db89c48514c18e1a89e2a39b3fd68e44ccf2bd53" },
                { "hu", "c583291ab753bb98d114d090ff87485ccb655a57496c4877d69dfa8886f421328b6c3165bf0c0c87bfc6def6df1838eb08165c633eabe0a5bb4c9065e4518424" },
                { "hy-AM", "525a97c3a36a68fcab6d5b9fd10fb51a564b801916ab3db3be03bab2e1fb6d25901ba2f8c797b84ea1eb562bcce338b850805d223aaa1f1ae5aaf269e9abe019" },
                { "ia", "cab41281b5c7d57adf91549d06a0fb8e1be71793f1cf70c7a7784fb13d335c15f357f40f2febcea8f487ff9bb387d81f682a3778878bc2e4b4957762c7731ae8" },
                { "id", "af84a7ca47676f76410fbcf8dafe625b2eb2a4ca019f845d3753be37d981ef419d863e9ebaf37d55af0db28065b41e4ff267c2ec4dd697b8b419023f600b4f6b" },
                { "is", "ba8c5ce99f230f5feeb6d87858228d4e307fad68d3c8b0f20528ee87823c5ae95c9d8f7b0a9aadf456ede2830a91f7b6d3ef09a1e36c3c66234bba67c166d21d" },
                { "it", "2c2972477b8b8e780eb096e97166c12ab24e449555c66d625787293b0a75368ee64b81795fb2f1cb14d01eb7ec19bd8d520f60c466aa2e2a2f55845ca00e0c5b" },
                { "ja", "293bedabddbf6f0c2a79f57e1189f9965fedb6022334a5a993c01b0c0cd13d04263fc0732d3dafc57557bb6d37693975448bf229e08b17c05fbd45c4793f70f7" },
                { "ka", "243c2222ecd3eea2519a2e49b1bc51770efd9de4a962b2c2862d1518ece769a88385a49d7805797c74ab2f7a082a0fb184a7141b0d1e708f32551922845c830f" },
                { "kab", "1e94b7a33ea4b5bf726a3a417429166a3690edbbeb8d38fb442a97961543f8192c87da34da8fb0667de89a1325693cd1b7fca4c9112792d1bbecbdb91a6c610c" },
                { "kk", "463ee84b250b5cdb75825455fda5c0ebce8521a7e1486ca34a134651bb0b1785deb81ce8f2fa53f30bea2368d44eeb7e81e43b0bf8ede0ab8b4c2eb4f0ee66f8" },
                { "km", "059d853f25c8882ba592c421f9e6ff253bc7c3ebc03a323f8d5d811cb843ce0a51a209b2b356e29463f3c7f8bc18676284c028962603c3da96da6331835377b9" },
                { "kn", "fe39d8d9f3938855fa38afa25b429185c1371a8adf6cdc5a500b61902afed9b1de3408c1e9e80f6a8fd82a06b47abc85a813ded3fb6a95954c4128890f06a9a0" },
                { "ko", "f4be4d8fbe09a88eacab313526def07fbe607df19a7d859fdff58d13e9f1e0649aa0e61ce2269521f7103788b6194351bec60f806b344f9c6ef38079ca87f2f7" },
                { "lij", "2df9a1db9d6ab52d572395836dbece8ac452e54e1b2e47f4558df3d12e7a8d35ccd0025a4e2a677c837c2b19a9446e5b770627455945b35616ae5839fb255940" },
                { "lt", "89c5dd0f6a1df2355c946d557dffcf472cc024bc0bf053bb4b12bbdac58584f23ada1c65aceae4dae0ba3e1ab495091c2eab3ad7da3efdc92d86fa66d5e1b115" },
                { "lv", "269fc2e8aa7a9593ef141018ad86424ca1c9875c7226c040abcb072c83be52a986414c0c70e56b5e4bafe57e8da2c065836a2080e5cc7cdba8a57b0d755e3763" },
                { "mk", "5e9600edeb9c9144c08d67a7f1e2222fa73374497b9f0a0f14d9ab2c18a806f3c9862c4bb30e631ce9b1ceef5e076552d77f1490556b0d84d67c69baf2772541" },
                { "mr", "ce34370eb4af6a90da5bdae1c277a3d99c8665087e73856570a63488af90ce7c5a3926855da027e57a89e2ec46c28f785d9844caf076ed28c8f6dc62824e690c" },
                { "ms", "6900a3b10360c9f2da94427d56dbea3a48efdedcf344a1630a2559a6e2e9529625db950d5dfbdda9689517f354bc4056e5ab45e700bb3eb9ff0cb4f7bcaf9e9d" },
                { "my", "387985b3ec179df3ff2a980b0f69cdddcd317a605a404036e486eaa40cc26f4bbb8ef873a425277ed4a8b4be1089cdf75389e83b16d5c41a8639ff09f49d0f10" },
                { "nb-NO", "e52b15884de42adda1b36291f92d92891f01e58aa657d1563d1e02dea7257832d192290aa29ccd4dcb12034723ead294f816822c944fa5f0a36a48764482b807" },
                { "ne-NP", "adca4fe4afaf6c09bd84f23597b8543b37cba845adfd4e38168b3a28a12ef9e7bcbe9b3d1ce384b4da89d4c9bf966d1615f51991d9430006e05879769b0ee1b6" },
                { "nl", "5109c51841892ed2c4faeec56d6267f045422b44b72f650ca2562b5dbdedccb424e8a3fe6760b21b6a41a2e7e97195af1894d2ca5eaefde7c594ae662b5c7929" },
                { "nn-NO", "d6be4619ef6045add6e6cc58c86dac9af469861f3ff0ff6d9fa67d97bf70002d8e78ecb8f5089f7c956c547e494127381ce498ef6687d82b298c7888c01b5b91" },
                { "oc", "2d25237cee8ed052b9b91d9c3be8951b0cc60121a734f9ab209febdc32c15800244d9b0c8e980c52e855e1766c8f6cf97b7bbacfda98eed984026e2970ea3db2" },
                { "pa-IN", "cb5e7808b28607a815dc9128353303e6b4f4fb914d7ae2825d6e0102997aa189bfa8c969e0771ffcf68dab56819244da299fcf5b5ecfccddc070b2f442df5b26" },
                { "pl", "c3d15355f31f38766d3fb7a78ae8bdea436cbbbb378643b39f24b65050ea84101c472cefd6be818f4805e015bc186f5c765f137061b6d9b32510e6340560dc53" },
                { "pt-BR", "b068a1f5dfff2a1418035bc3a4a03dfba12d55c700932ec9ca42cab82dab087d2963d3852c83ae5cfe0293e683fe7bf83e0ddf46546430c6c5ab6870a9f2d555" },
                { "pt-PT", "cd188035f086e17e740a16e08a56cb5584293a685b9f0ef1aeda49f64163f38915bc85851bab37b90bd119e9da77c0f56091ca41834a4ac26ad197c4f0837cc4" },
                { "rm", "dcdbe64b74e6559138ce796276ce304665ef8a75ee69339abb2d2b361eaab683189edce096ce9c0f62b2b9d45753aa1c3f8e896b0376b101d1b83436d2881fba" },
                { "ro", "35687b2222e8473e57de338401f260884c95bf98cb4c53ca7c521ffd97b43a053e9e35bc7427618490dd2c47c8fbf1d7545b2752ac82167b23212c22a211c905" },
                { "ru", "04687f3e0f1dc05a4a7578bc56d203af9e78851cba43e8e4b4df10f91b00c64c8ec543af964a6126c577c3bd6f0855e2d9b9b48571d783331ce26b505ff5f3f1" },
                { "sat", "d752581aa2abc724c7a0c9c2806dad179625e57ddd77b34098177e3707e4bb72a5d5e384b8919a7bdcad131fd51ffa857882c4ca3f7c8ccf5f9a67726d9cd2c9" },
                { "sc", "6c1c1e6b29fb38439ea354cd28252923da9718788240db8dba79ac6ded09ede3c33a79bf02fd45d669448c7f92c298302ded45495c9453488346149521e6c5db" },
                { "sco", "f9a982b431b244dc0c638b6e32a9c37029bc1c2dbd6867523892823ef4693600aa008ae195b91670f551322704b00ffb13bf0c0c195e3780fa4eca95b8a12848" },
                { "si", "e28fc89309f0bbd9bf8c89d54d9733571f84300858ab93a677085c9df77318192f587f4ba58aa6ae58c2927f365ce6f3d91de6e9d1a796739ec2cb0b5a4609b5" },
                { "sk", "8e0689993fcbbadca1567feb9655719eb4815eb373db6c3178adf6e9f9801e3bd1988042c15fd8d7d972dd9daf370b13338e1fb0939253735df17c83d0abe277" },
                { "skr", "95dae8b32d95c105a01519bf04ba60bcaf310a5b17c198caf53be65903041c343699e697099162bb388f6d4d08796f6694922b8838ebbfcbd39219428f7356a2" },
                { "sl", "be5f2426a2608e2463c5223509a0cf038542f291c7aa44562e8356dd6a7cd66795c879d45a616b52634a652da1a3db9a273828a124baaaab696df58bb777556b" },
                { "son", "f5675579ac1dde93ae7f4395477a03b85992ce7c9d0f3e97f4d3a941032e1e388c1cc13d24a4c83473d2ec6f7b44d3f2b76ff5dbbab809032cf4725ae719fdf6" },
                { "sq", "7c769fe425c19fb30edc239d4db3cf4066db7e30a59680e597a8b0f1c991ea467abcba307bd3e26af657cff90f3d8ad4dee7498ae94272570f7a6b7de5e8af31" },
                { "sr", "cc80a6b7ec77fb6b42fcf9effd4f32a299253ddf50f50badf9362e9aa37ca418eb555b14c37da634dd4b228e34eda8f75baeb8b2d641cb571e92e350bd3e76ff" },
                { "sv-SE", "b6a7ab4d1971474c7abd5f2fdbce01350ea71ff0303a00ca05a78d01dcd1b25bc2b3e09c5700723eece95cbf372a58f1d2e5c8a453f05aca8f23573d9aba6f93" },
                { "szl", "a52a6cacbbb1449c136c97770a63d48079ea1bcfebc19d77d68b04233d151f00606f410f97a10e0a9f13951ec745e7f1e5dc9047cace0d4722a8717f2b161106" },
                { "ta", "2b6d0a24c418d0839ad6bf75b05c19c8def7341287c4e08243343e9757c97d3a939f5fac2ee736503c8b67bdbc2cf353ecfdc0ae91332a7a731f83494422fd99" },
                { "te", "d82af088a9a855e0dcf99312fbebc32bdc68b3d944a3e5f61d22f5b1e733b306430d41d1fec16d44c486476fd3e6cec476db7dc0a6726604483b5208ce7eb08a" },
                { "tg", "39faebbd4d5cb450c248dbf401ac2ca5d5a6d80d85665b155c603e8ab2090b2de1579c1a5ac8a77707d9d27961e8a1a7a353684e7e475149e1d201a02611a788" },
                { "th", "9922ce8d96d9e70ffb11221f39afca25652b890efa79e561a53e4216cb64bb34e2d74029ce2fc85f16d661a9ac75b147bf762621b49317a2022f23a75adb92bc" },
                { "tl", "d02b87ab36fc1b3615d8a2125b35833fb9a016a7436fa16db4d2c09a1d73d0506332ec0e59e75769e2423286954f5500c47ef682f8ac46214f668964a392bdff" },
                { "tr", "55885a11af1784e6e33b7a6bbfe5add348e06137f8f41adf251d033a4e24a5a8239c445fc86614e865dcb41b42f53063bfc0a3eea34e628cdc4f58114976c7ca" },
                { "trs", "121647ea405cfa5bff80478f9b2cb65ba386d2fc3e615e2617b1f6f17b5720dc8a3f0ec1eec9c528a50d4d5e9935768d4e49d6373e4d878b3c3bbfb31b073394" },
                { "uk", "a549e3beaca158f44827f34d258b959d509e3a29be2d3e165d00d7cd1325f5384a161dbb0012564d4b1d17fc485a859358d4ed2a6a98ce6be11e19a69eeccfff" },
                { "ur", "9b7da5766365dcf53e9990ffa6c571c6bbaddf2cd9eb799d1fadb6bcbb981bb4d0b7da99e93d1c13c7c41d3531369c8a543a8650eba8180b198ced7588053317" },
                { "uz", "2b7c9ac3fe324d1d2bb83e4e662c2cc39710ff7e20ed8d9f604001755760650b450fda413c395ea807832f5bb32b6d25441817072a883078f72ce1e2f9a6ac6f" },
                { "vi", "bff84c5aeb52babeaaa133d00811380eba2175e9aecf9b6cfeab49c5d133b1d22cdffc3fa5db32c8046bca8ad8a0ef45d7fc01f57aa5e76ee9755f8be4a43093" },
                { "xh", "4b2bcfa3ef3a179f00cc8631b8eba07e9330a33aad263399c3b9713daf069d3faaa55d9befe780b8b92619abbe1c4f91a439bd07e14e681f417c58c898a7550d" },
                { "zh-CN", "f04fdaf7f0ee47bac83b4edb277f9c4fa353d77d57656a7f03703a1d896f56af6c19ff9126f444f0d96dff82fcad66e0dae4967d5e45d78f6d991bc4e4a2ac58" },
                { "zh-TW", "4fc5bd9c8fbb43ba0084b21c6d3ea768ca106aaadd7950b877e41f752fd730696d859de32326a1cb1bdcd3f0ab4ee5ac3c606d9abe64c11a71a4a0d492bbc989" }
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
            return ["firefox-aurora", "firefox-aurora-" + languageCode.ToLower()];
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
                return versions[^1].full();
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
                if (cs64 != null && cs32 != null
                    && cs32.TryGetValue(languageCode, out string hash32)
                    && cs64.TryGetValue(languageCode, out string hash64))
                {
                    return [hash32, hash64];
                }
            }
            var sums = new List<string>(2);
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
            return [.. sums];
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
                    cs32 = [];
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
                    cs64 = [];
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
            return [];
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
