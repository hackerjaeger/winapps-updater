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
        private const string currentVersion = "118.0b7";

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
            // https://ftp.mozilla.org/pub/devedition/releases/118.0b7/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "fef0f01d850b41842e8f8e24de696fc6dce91266babce482f14abd7fd3ca6ec97366dc80c37f82bce8a0bc46085afc41232ab0bc1102d87913e90cdb571699a9" },
                { "af", "547ce5a0ac57b03c243b67a8b62e54a0e4b56ae2bfe3a631580710fccb0ffdadc0d43a31d3a483542696556e379caa0967d455ad6c4eb4df4a25d22d754efcb1" },
                { "an", "a35c07affc0279d41bbb9061cfd6a58b5b014a3fe8c91e0e10937cdf267a13cca46a7859a02a405a5e56b850e788076bd8c4b4ff75c583f322d5b7c01ec642e3" },
                { "ar", "2cfc8f67905c93f22c9eb9bccd027491760a862bf353b0c554b6cc9ec9eeddebe40b3698718a30967caaf1cb813ffc82adad989b2dcf08da065eacf5332a55de" },
                { "ast", "f37452cf3d95022b031726daa66bfb2d6a54b35f583f2292c6222c259bd7c4feb7f0a5d43fd6506315e7a93c2ba0fe372ad63def6315f5e880c9e72f411096a9" },
                { "az", "9cc159fd9f4230936f6d7e47a2398a54bef7dfa9e1bb57d92cb51c0c1ed7aad4c90d9e0b89a69c4ccea5250826b6085a83adbcc86b683d50ed121fbd4b83b2ec" },
                { "be", "2876d1ba673248339595f3d6140a37912f1a5892009a59411a5153749824d4d1eb62166f1ea92d7b6b33672a1b990ffdaf1f0150e92c99cc05536f8f068b0e24" },
                { "bg", "c71ea1fb8a7ee2e75876658e5e000b22aed04a275e8073ef5e6958ce6ef2c155a64c3629d37ca77abd6d9bb4cbeb9c1bd1e757af6a0b149f415353bbce8e53d1" },
                { "bn", "9b8279b140ea0d180c5e16953f64f3f4a098698a482e7633bd57d62e8cb2a77b246fe20d1a9465b088f893e33b4c1ff8f98406ac3550cc6b511736be62150d41" },
                { "br", "3a2f894228031a680fdf06e8300f3ccedd95896b905029870587485712dae9e31d08a214f31d26c1fbfb4a9a014568554d10f54ca4a84242539c922456b30993" },
                { "bs", "408767c75e4ddd3aa4691f18b3e3e55a58757e5fbe3c853ff250c21e0ee4020f247ead36d38f4b15d18d25d46f433c73d2e197232fef5257248b80e2ff48dae8" },
                { "ca", "00aec802716f615d7f6efd4d033800fb43776423fc88d57b2dfbc06fff1a2a5821abe8ea942b07c6e12dee8238119a19347d5904988739c82ceaa401934f171c" },
                { "cak", "955f1fca84f556a4e21d1c423dc78af0ba8f1d8cfdd17ecf05894b3b012a6c576cd9113a97b02755f41aab5193c21a62eaf0f75a97a42cc957009b81ab5a371a" },
                { "cs", "784d06743ee38b4561e83b0cd70bf15ab6265eafd8286771fdcb5661cd481bf3b51a7fe6d25f582e57d371ad0a65ed4a74d7921534c99f8d302b61b81b0257fd" },
                { "cy", "04b6f10f55336a1e29f80c26fc19088d6ecf5bf252d7a204e484302faef2c4d93cb13d2e9c6e015328fcc8924789c0fc41c868bbab2a0f3cf977a3c033319414" },
                { "da", "7fd282c06e9ca9e385fb3bb940d923f3d74d67f23df4e79d996b0ca281927a2d80d96803a2c54e02b697e86ce0cc42e489b727ca125e33baeb5ed77771444027" },
                { "de", "686bf73a2a2722017902b89410011c6bd72c3fd691d9341df76f05a13cbff2114d78fb595e68bcf27833b370fa7c7ab9a8894453ed3aae5c566b5d729e63366b" },
                { "dsb", "a4691d9a6572cf310ec8944b18b6d47f4fce3ee4157a736fead66c96ddddb589f4ceeccb3276da0d6bfc25fd2d1f10ac6e44813bfccb0fbd081482e599f1b57e" },
                { "el", "29ee98dcaf8ce8796fc5268ead4b53a2d7d9a0c0658d228a6bc2179c6746151c9dcb27f2c07a345868474f646e9d033986ab4eda0a2f5ba94a0edd52d0114711" },
                { "en-CA", "3258ee1e78b558736012c0d86dce4581de2ee650d91c61b97e90ae893753987c42b9d02955ad21e45db745ee2d29a1e926f847abaaf1e8d11a6b42702f512777" },
                { "en-GB", "6538f2491102403db2cb632aa7a13b6034fba496b6aa1fdd5de051b2253ffa176b5f926274e319794b73842ae6ca9bbb76bec489401f55c4a6c762070a332f25" },
                { "en-US", "3ab03657675648a1515def500a013697f9886b42e512e876d10984cef98414728efba9a1cc221aa521e24f229c50a3f478b59c95535355315894184d7fcdc073" },
                { "eo", "c6092b78c4c991d92b4171bff3731b426f0082d4390435189bf36c6a01b0467268b2a3ff4d2d7ebd49e06a4a69d0afa596a7e01e2e0c35b629ad56d558b7ee07" },
                { "es-AR", "e818fc8b1b79183ad4678a19ee7a59af725b3e36fb18ecbc5d45478c5579c80dd64ee04ef5dad1e51bfa718196215735372859283ff72bebf9daf36d9b7b41c2" },
                { "es-CL", "21a06e8d88c7a2a394ca1ead1def35a92d7b147a0a01a96e95b582c24edd8f853724c234db087ada42d746c2beda688ed7f4adb7bf4c1a3c5f99afd4ae868a6f" },
                { "es-ES", "a3c5f05ff645e71c230b3aad094fb631fd9281823cf54cc768c6a1eee280ee2d480bc118e4dbc94b0759722b521c2a761b565001690639acf9cb7343203a2008" },
                { "es-MX", "4a223700f0f5a19dcc109139c224cdbe2e6e93293b02574f3850d5bfce3abd6e5ef5f80bf9cd9702e5e22176732a3b44cda227a238408360b00c85127955b9c1" },
                { "et", "2b8240fc9125d0263041e9b3171a6612881fc5d86be4f90a48b8cb15fcb2d779f517aee0b334db4f4b70e40858b60af08a7e4d753ed4a731609dad96e17f55f6" },
                { "eu", "40feb0701aefa8427175f878aebdd0e56c502e121405a12e0930b8f2415cdbb55aa7838dc902547fa14b1d63457b1efc4640f39311eb3ad22574b7bb2c312d69" },
                { "fa", "410595099973bcb663692b9713851a1de12385f947d3df74c91ade8c574f45aec0d950564f73046006da773d183f13d5375cecc37f31d70136939d5552f2c7fb" },
                { "ff", "6c22ae81de98f92d08a8f8b61dc63a6bdadb699873c7bca59d9241f46e22f5860724fff51dd6137c7ec70f6d87e3735c78d93fd980957148059d888e4f29d883" },
                { "fi", "aefe065e83016540568db9227b828e6cebcc0526bced4207a1f1a662ffb695111ece83749f481e1d3fad74215d4aed009b645661d5c1f2d37a06e9982cc08c11" },
                { "fr", "ff74d750534a118f97f5c73d2611f7be302d50cf80a910e9bc9928b94779a72bcbf242e46a2a9f01d1832a899d6b95e9621aa715787af468bc4ea3a599165ed9" },
                { "fur", "e031d6bcab8204a856c0fa1aec850d948dcf68adf473a38c18cc06f6a324bdd732f631f9cdfbc3ac0891a0d241d10df401c72f751b7f38e3eeca09aa2fb8a267" },
                { "fy-NL", "2482c74bbc59d6c1a3b253d4b86e8ffecae33b2ef9bc456680bb3d237f82757b90a03f9e4f316f0bdc6a0ad45ee85fa510aac1c4bb52e58bd4dddd550ff6f0ac" },
                { "ga-IE", "77da2fdff91999991d669843196223d8b62e38c612fc03b51cbc02485afbf4c40c09081363d63cfb55cfd8e69451839e610eb60f9ac6b0256f51f6c2225baba7" },
                { "gd", "e584bb3e2cda8a64b6ec4bb41f481b8f519e383d3ea091d4e4600f6632a1b24ada2d14e7253e9a67cb031b41fac56bdfc545757b5e99295b1ae60daa8e9b1ed8" },
                { "gl", "12f22fc7f38ebbf01a82c1276811f7f8bef44de4c1c425676c54b18a667f0fdf7e9b26e95d11cd1a575bc7c777cb33d598801a123aa498da4a5eaec1c83e6887" },
                { "gn", "f86cb12914aa13066e57a4c625a1f329f2f141a373ebc0c321626ba64f2a8e52921f76580b34be63c5864fb40542a5a96a54c1e753a816ee07b673096a0faf93" },
                { "gu-IN", "05da89b674cb65793af29549aa0170bc6b0e34ff77424df6b11e023505007f29f002e37f5cf7ff7612294c3cbc4224b562018d4a87bb64a7fa746bcee45053e3" },
                { "he", "a265bd4c02d5a89478e45de98ffdc70200f605c5d693dd4f00a114ef45b25193c617dbb04d343ffed755e2d4d3dc630afe86dab6d6d9595773d8b163979a97be" },
                { "hi-IN", "3c629541f6755a1b5a58abcf2bcb5e8aa5925b85e8451e9d5db1c2260e8d6a72eb3c5df8d301927e98200a700667de60308c8621b7eb5959ae19d086ceedc20f" },
                { "hr", "6519a5700c3cf39da63add426686bfa0bb987748f106277a575476bad76b9684496351a6d2b733345e957daf8fdb25cb205b328a7029c536a2d8172ccc88af1b" },
                { "hsb", "6cf05b656ce3e671c029b09b3e14e95265c35927448733b551cc4131252d79f3a8fc2be2356c947c0f8d11c924d2107af1bcd72249c831bb2eec0a0f28859ef5" },
                { "hu", "28805047ea749f1deef5119363523ad5f87288975ad721a9866d67121f4e905ce5cfe048370d9fffaa069860b7cd4d925b8cb8ca7081e1a3e94ef142eaca9eb8" },
                { "hy-AM", "bcc3b6163e5c65d7bf095e7cf76ad2751abe3ab8a201522d3d291dca57a95e26c3115562109e5186a3d9464d37c94f515b0094df405b05f7591efeefdc3626d8" },
                { "ia", "52cd604d5c4d42f4072768a15ca073f4758688a5938778143407026dc83fc3142972f3084c06454938b9e1c2d18c3065a1d69bef502c2e2196090845471e9ed8" },
                { "id", "137ef9145c53ab23701a52060439dcc66b8cb716fd5803029bdbc4445720b0585218494113a18ff08134c16b92bf8c92812533aea9538e00fcec6e4c6e1b7799" },
                { "is", "a21551777fcc042bad9c9332d8cda71db13c58ac7dfdad31123c6d0ca882c75f5706a171b4fa2581c3681806e6fd90e01d19013043b5328db9d766c392371f17" },
                { "it", "b850d51a5cd008f31f796f8cb7c23a21e9b5257043d13bd972bbb9049921a1499f4044135b76ef2d404e4659b42d60c50458a784b36063638b00df7263cc9326" },
                { "ja", "d46bce52f3c4302af09d2988d5c9432d5761cda22a95fac827745fd1f71b4676c9a084bf32d40aaeb7443ff04639dfa0198dabef3cf6eb461d615e215e17e0f9" },
                { "ka", "2128c9c8c4f8b87a346164044247d359d06d515b721bafd8375324856706fa77b18621ca3f7025e6e71e56991608c60894b467420d6c3376d9390911274332f0" },
                { "kab", "0a9a7ecd2b7823f29a41b524211b7bdfce4c4a6e2f823c85f6bf67d058e8d77227d3b594bd4732957745eb0e798a7e70dbb3a655a829b0f84d02184b43b691eb" },
                { "kk", "aed64bad6e4a7c5fd73aff960e88657180637fc6c02cde5876dd19295c326b65effe5bfd103e1a440c4761f5e421453c9188e13f35ffe47553ccd9380bfbf7f3" },
                { "km", "51e35a3a4c77daf17c08a6ba52da14215d02141d9a90189dd064eded4db5b14ab8bf48a7bdac93defa5884cdde15b8373f8e80fda2b4030e786c1fd47323d398" },
                { "kn", "40b40c990a1454e63fc942f0bab38cfb7ca0c3a3c6ad6908a43ac9f3499fa3fea2c648844113f36528856933eac9d4dcf1a6a307e208e35d0a230ec835ef29a8" },
                { "ko", "42d00363f0c311bb957109c2060823ca2fb19928d27fafa1024e99449de433beada6fad89388eefd8c5a04ce90aa219c09b645db3715ab6ae3917b0e77e4849e" },
                { "lij", "7449ac93c3f68eccf94f3dd75064f655a03525566b86021f415b8bf6c096f1ac3c4519be35723b64000a645344d054c1f0d7e7dc31a53c14330dcfc64f2abdfc" },
                { "lt", "3f30c40d536474ead87dadf48e57cf0a23edf659b62f2c403991b892a939e1658520539c496a51ca4630d26344e46c9ec904a499d44ffb25e26787c4e8464bda" },
                { "lv", "c0141e86dda15850b38ff2136248adb8a0fb77d31b0723439ef4f37f7c86c4df0453092db5a34a60b4a0c86f5b491bb3dec985a1bb8f0e7ef55022255dd229d8" },
                { "mk", "d379d04dfbc6ff02ad17d200e3a2bc2a7a5f3f1b00292e24b5b87f174bd04fb8aa1b8b06b7cf462b5b12ed0232b14897a99c8f5d8a9c87dd0c12f2addd0f4256" },
                { "mr", "a0c252fab62770ad3045e6fba4388054a74ffc7106be6bb3ce81fa7c48509419f50bc2d9be0ab15639b99942dd01f998ec7faf1b620acafec04f19d9771c85a5" },
                { "ms", "13753f7d55902074f8649e35ff103dccc57e1dac8f8ea1b6e7b03ad2cd011c77486abadf819bf804d9ea4010a04567d4a6e251fcc1fff67e06a57396b3d3e488" },
                { "my", "407ff4dec0bbad1f44346822374ace92fe93038eb9e6cc7010b8bd91298b0ca84157f160342a0265eacb3c8f9b6f19e43c10bbb3091a3400cfef8edd9fb192b2" },
                { "nb-NO", "39d1ef5ac13897116cda2dfcaf1882a9ed2adcb9e81b5bed0437e2a2e9e36fa6d202fdf9b023b1f1db0e2612b8fda79a4bb570f92edadfd7bc1087ae02527a16" },
                { "ne-NP", "e63e0bc1f7327a10d6a456c0fc298d0f4da4ce27992405b3028960b7116d02145edc54fbbab1a0086bf9a227993b574d981b8dfebf37d5a88a20127c1e2059e0" },
                { "nl", "37f3a96305ea52e434978dcea30952a1eb3ad233a19266b86f5444fc47cfae999b09c1fa3b73e71898adb2ed34d20074f1819e639a6d6ba604014c811f946987" },
                { "nn-NO", "cdeea72d9531cb547eb00ca422a345ea64cf1b0cd943f22c69e4678ab76f27f7050445881e63d7e1c7a53fd7040902b446bba2e891bf75679c2f99af0db0865a" },
                { "oc", "b16658ed738d137c426d46cf2b0d947b9104d46eb646b75b4d627fc4a720b08eea8d9106c3a4420b84a008bfcaa8e2df9e230e04eb3fa0e0cece27724ab26b7d" },
                { "pa-IN", "ac1fd62c8de9320ee605286dd8c8f7910bd47269d9e64146a6f729f645e6689f5316dd496a5b6c369aaf1885f30d5df99d896c4b0b7efbbcb7a80cb701ea7ea2" },
                { "pl", "805ca24fa584ae50de3403cf67be938f1e269809e14e59e45178a78ca3ebf64b4ef8075d7fc7e5c2f3fac651145940deaa38528fa321e3c53ef9acf2134c4fd4" },
                { "pt-BR", "ea9cec0a21f3856a5f77973afa12a9e9fab95f60b2fe6c3c19694b2cf141d57a4682032d0f8ce8fb7dd3f5621735450951861ee407a1aad74dd8678188fad6b1" },
                { "pt-PT", "ff449cafecb7b84005cedad385cad5563d9f4f2bcb6b42747c808901c001b19eca34a71e6c6d40d6dc0134e08b81a1944af2b97560061adbed0faafe92bb247a" },
                { "rm", "781fce330e4c76a937100caa553713b2d94a249f61ac6e3e618d8a296336ebb7692874b7f212dc2a9877428b8c40cd023373b73f2ff1558dcd6a47cbff1329d4" },
                { "ro", "ba7c6fe69ae4dd9e627ad0087875f29d041ebe3cb688077f82b77c97c3b1b3b9d0c6127f9afa094979d8e7a776518b5e19806bf5071673cf53f2c18733f4eef1" },
                { "ru", "c3d3321910cf0c2a417e3817cd0b6183db9483f5a51bd7497f1a39290d91fc173156216af69f2d9d4bcb2b8ad5ad1b33d2fdb7505fe31425241e5166a8f06d83" },
                { "sc", "44a4d2e18d895ea1eab55667f00155386f25e1680dd1ed55e9464e4c63f3815fa39b75e5dddf186450413c36f1937ed5cfe8ad2fbe110448ba6f891a93e3c918" },
                { "sco", "f2227137009e3c318778a9f046da25be70f542ad4f7b24d6be552d4717ece8d6ef274cdf45c83b7afd64773960c6d0c1e48b4b97a3f4df9f71b8c43a852e7e86" },
                { "si", "444a61967541a2e5602e56f4a4d642380ab1038b476464b5c666f8df563bfd0b52eb3eb3a9e6dc48fbee88080722c7879dd9eaa5f265082464bb93d1190beb27" },
                { "sk", "242d8f968b844a25ea586009542f9f43152816000dc7cd0642d3d5c3ceb6da6e78c4f47043defa2e715bb0e37bc12a82a12f864d84f848e6be2a68fa08b9974f" },
                { "sl", "eb8b12fdf57868cfc722eafe28632897618f198ffa68d3bf1ad9dfd319007ab9f566669c865224847654b83c76d1dbeaa127b3caf07f388f3635f6bbcab2919e" },
                { "son", "897373d6502ab2a9ff397b2db89edf41328ef55d3d85e7000c886d28428eadd24e237a356a9b64bb42d746986f36610f9095e5bd5e183d72aeba5d3457ce9f1d" },
                { "sq", "4d1b2495cc5c889c6a0704ce6b8ff8d322240b06a130abd3a499de553345e648af9e1f550e8a508534b444d85cebd4a784e1aa18d3d173a2766fd3cd4d4a5aa9" },
                { "sr", "260347dc1fdb943713f0c91266a174389a15bd1536a4d34c4dec26effec42be426a248dc232f1cada37ee464ef36e7691a9ef95b8bab760123f7df7575dc85d6" },
                { "sv-SE", "d13f068bb60415b5b344d094c68423b5154ec061733ba10681556078712d5308ad610d69e46bb53b2188725527144d6d44aa72f8bfeb804634cf90990c8d156f" },
                { "szl", "5351aa03150343f940603ef70e4fec4f342b585f1add5bf1b8e45487d6dc89db7609eb393cdfec42a5c1f85c233076e57e35de7e4246a30c15db2fbdd8b629b9" },
                { "ta", "5b195bd4a258fdd18f8ac157bb9fa9dcbc3a5772eacf8751ab9d3a523fd3ccff47705dd4fe5cc7ddde3038d19127945a738004d779257004874c6655f92b795e" },
                { "te", "bb92bec4654e7ba5a09530863a8bbb7089e14337f583599c6fe76066e87ccfbb5af8c021b34feb3bf52a536d4659d295c7f1e2894d450bea6527f45010bbe42d" },
                { "tg", "667d289ca1a18a7a2538221489b319898ef5d8aa39b79cd05bf2939bbd33451fa14b9d72915ef1fdc50aa5443074990055f041a3fe157329035645ba233b2d64" },
                { "th", "8054298fb3fea6d478925461a118eaf03f2b8b4600f9b11b02410b1391a43a8e763bf3d04d00c6111f3cd95681979bd38729441142b8d1c43c26b234bba1add0" },
                { "tl", "ec76903b4b0979135a9fd127faa7477287eb4bdc13bf712422f7ca3590a8f2263aa0be1aae60d0a394a11b47566fa260e6868b1ec154db39adf7bc7032a83895" },
                { "tr", "a42a23440092405877e046f568da8dc18f43aea3dc2e9a0e0bf832a3979ae532373a3be8fe2e2542cdca6e30675398d84dac84d2f1612bb0766d5458ecbaa431" },
                { "trs", "9e0d730b2d1a21ef6d1dac3418531df8feceb3beeb70db85729bba4811f41265b39cecbc8787bdef37cfa0b7a9adc32eef2a76d11010fd46a82b09ba54a61c67" },
                { "uk", "0846c0f3ddd1c5c20017a5c7e4ffa7dd477b49cb1680947aa9f65049beebabaa80351902092ebbd2586c694d10314a74e53579bd4ec963a407c065bc4fa83442" },
                { "ur", "f9f9d3989e46bedb4ee9164334a8bf7b135a0d63140d8dfe551dda5571c241977efc92985e70040b316c83f1174de00f76e6c78830ebc0d4c14f6f0a4af35a7d" },
                { "uz", "f32d496a4329ddf980620693f2fea9caebcfcd09130da6c897ee77bd3b2bba8654e812976838d292afd28239e7801a4366518fe8d97ccdc00ea01258c25a4b61" },
                { "vi", "0286f19f8c030dcfd95ad80794f55e0c170e71e81988dae54b3e22294013bc745bdf66baa33ffd1b0b02327ba064a69475331124d509e15d3a3beaffa8186cff" },
                { "xh", "6f7f5827ebddd108a6121ab433dc24bd3830941dc69d5abdc139801fab75b22ef21c12f35c3235d5be3428ff916aa23d7a9237b05c614d9dcaab1a3d317eadb3" },
                { "zh-CN", "78be95d5b03d780ba3c83d6b96db0a1cb897b428432c9fff87a2927ad32b38d3ef9f9311133667613bf08b655ff8cb4db59465cb141a7c8822fe97479ec94c9c" },
                { "zh-TW", "0793fdd7cb7d0467afb466ac05c55bfa963883d07d6d82bb1cb8f5b969ec80096b30befc8a0483ae385a3c3d96021fcac779c6878d277576ecc8c6c7dc269c2e" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/118.0b7/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "0f5a0139afd7a9adca18212db59857366f42c07a68b3f3caf69a35ed52c77e7d374d4699a0fbdc7c626eae3071c85832e377adea0e71e0eccc297044842441db" },
                { "af", "e72cf977dc365711242d66e4bcadcd2b7249ef36987ac51d0e9ef605d56219a81f93a2cec47d04e779fa7aabf9285e381f60785ce555d75a71a0bb721291ad00" },
                { "an", "1e8191f757f5c8b60d555c3c8deff5bd3019e7f9bb0856f8b27d2f07f8636e03fb9207e7a9c0f81f5c09f28939a11d011aa62ca7f2c08f8ddf72ced492ddd28d" },
                { "ar", "df3e6e7899173c9c899f6562f5b8e6e15ba3cdf1ec46ad0f66ffa0eef356540a9aa1fc70f15b60a3c88b8bcc694ff6354f537b9ae86e2f3ffe6a320d883d926b" },
                { "ast", "72a0d8d0947bd62d577062551270bd0c23c663a53637cdf8d73914e65376d9ba44702ee6aa41f63c69a3271726d6a573f800a75e38e8593196b242735bda3383" },
                { "az", "6e8f6156d94dc465cf949a3c5068896847308035fdb7404c91714ecc89e1ad3980e30e55addf364207904626d6b4ceb27e32630125008a9e4b00cbbc2ec5ee37" },
                { "be", "7f7aeb52b22bd7b4cd3834c01f2f6127fcb2d111fdc6209d19da34a561cc0034f5f606e64b5db6fdf4f66de02fd50e3a9eeaa2d176fffd2ba90038e3f1fd7f74" },
                { "bg", "7db606ac87d6b7e060a531bbb24c522da77c6128d604d97880c19b3c36a3ba4b726f8e8a528a4d37038cfa48facca7b5869dbd67e1d13e8ef0e9375aff90e4a6" },
                { "bn", "36dc32fc8e66311f8c6c196b5ae027c724266397375893664cd2d2954a0e75f2a6c5810b769e439d918f01ea33cbd8cbca094fd67a40c502e3377198ba8679bf" },
                { "br", "07bae89fb5931b2dec007f6cbde8bbc62fbc6985fd9993c5a9168f6e926bfe3ae9f0be48aab51d88be3651910b3825d34320088bc23392ce38e5594c78ff781a" },
                { "bs", "7b0184bded9e4fdb518f2890919cfe4f9c2c11a353375cd2a6241b881e5462f1cf7346c2d36ea25b94beb6250c224a9895d6511a27c0ccd1130a9ad172a2b01f" },
                { "ca", "6f8cae6690b8b1eea0fd4254c0909e2db1e7b1872e6f1dca96a9ceeaa152d0b7c577039bf64f46b07677445816ff93be34faf6dbe0bd2c414f9d3c1c14fa256b" },
                { "cak", "63db88ad70ccc7c55cb7652f1304a241f8a1a944db5516b7c9430a0a2e196479cedba2ef5f0d7bc8b82f84eddefb188d7a3ede9f021532ce8e66bef3e63a8506" },
                { "cs", "89060a73634a41f93e849ff4be87b5875864bff0376d3a70040036999ab21b54fbbbe0f8594efbbdbf5f201230223111694945b154587d7c0760fa5c3f3ea40d" },
                { "cy", "1ef885b9b99a3bc08642b3d64bdc4cf0f3262af30522049230b6458b4f5a5859cca7ca5d17b6e76f67509c3afe3a7c94eafebf12f1b4bea293fde560d7f06e11" },
                { "da", "9a20494d99898df4da057490acc1bb950e0aa44afae4b4226f42631a2f272fe5366039ac17ca6c8b79a3da382bb013b6f2e25bb8224ee2d1cfce8eb852de1264" },
                { "de", "c9a44d692b2f597b8717b0fed174c70322a2d96cb76d8878b4831cccad6d6cc109ff023b63b2c7dca93285f6f84dc943a04842b95083938d51110d921a695114" },
                { "dsb", "3e44006d1648045867a6794efd943a8c5046102bf1d65f8186248d89c984696111138c1648c9ed4aec8a5a8f9dc2e362145f7889dc54049206c48aecab3206ad" },
                { "el", "1157527d3b72a0e875b173c7170ab190daf4977830fa3775e258a6efc07d0f192c09ec9faa1891803ba64c264145ae3780e18d1e456e882cdaba0bc5b35062bb" },
                { "en-CA", "2e142e02d5b5e288cfdcf2744ad861c72ff270d64378fc4c014255439b8f711da4ac94e94271a6b34d363740d7f619bafad94a6c467eb5727ec5e8e3d8882f38" },
                { "en-GB", "d2bf5933030bbb94e60cd5a879795d540509a40fd4f56f5ff391574c210db8c06b41d4d2355e0352f04d66da0c57430e0ee2bebb9ead5c5706af2a392b8c6001" },
                { "en-US", "5c12fa2ed792b5d90eac8f9b59ebf142b7201433693ef6b56d2639928efd31edd8925a78dfc91b94b5f48428330e16436fe57c21eb1bce11aed40f7ee5c8f3fc" },
                { "eo", "f02827c349f678e97f9ce97609e66ac455c49e3946384276b22605cfb95f750f6b7b4b9207afe838c2b61468faf91937a5423c5f8f29041d5fdc64c05538c178" },
                { "es-AR", "a2173993a9624b0aeb684697b15cad62b54c313fbfb49f80379acba323f474ca73bd87e710cd2010b3bb14a9f155536841fface5bee437d88567efde0b14de61" },
                { "es-CL", "1cf5a3cb78e37b17d0fdd2705fa8b035c6a112d38945d9a7b5230e1c00379f1c8c90d0f17a98be5965817a4baac5475c872d3280a4aa21fd15e3ea9ab1bab516" },
                { "es-ES", "81c71da3cda2aa0c882aa47ac32138482ef67308a94dc69b744e393fc70044def94c956a2129e218ce78684a45a0763979846ebcb251fd2fefbaa42048d8deff" },
                { "es-MX", "d52814cd5aec75c6c0e3c2e826f97d376cbbbde521b0dd5a1b2b370045858a0a0e1e30bf3293fb467a55dbeb1927c6da5e89ff922c287388a4961157e9cda510" },
                { "et", "e7fde2f339674b26277de2b56c18e4109d3a04fe15bb8115702ba91e948bb063199cb78b42c1a0c4e320c3ce78bc59b6c5c2d3ee452d9787bb195eb45909c8a5" },
                { "eu", "8ef6f6eefcfee989bb321f5bb791528e9827dcaf6e7e2e5d0fc799783e8a41f3a57ba8be65afdabdfba5768af4512aa48186d29350cb6456f4ab0ec57cf5ed68" },
                { "fa", "a04b7ec7ae63f168547c6010bf6c5a42acbb229b619c01808d3af99711c136e945e85c38b3eb117cc23b80117e790cfe038436e2fbada3abf5e8b555a251e17f" },
                { "ff", "1636d1ad5a063d5ca76c1a7432eea0eaa7aa1911d969f8e9b8e74c3d6da328b90e9847ab0ae2f12f816fc0183ce387c5a6bcd9cc81f01a52b06da7d9ef777fea" },
                { "fi", "85c9b0d5a0017cfcdc44103849bcfa0a5b3498def42b666f166878a5778faefdf79a46d836b4667771aa34c3aa0de59b278d1d8a9999d83369946a7b61c1a522" },
                { "fr", "4e262ba2cd0ac8056d9d084d1c0be61fd712cdf7e884894ffeda8ce7ea6e4acb4152c5ef4c21a7dfa608d0564f1595fa909f09234467bbc83caa1ecba86b9d7c" },
                { "fur", "c6cc0eb5dfc3c0997d76231303cbb31dbeed084cf5cc02a916576b26ff75bd47f2d984f8c6afb51f62447b36bc73877fe11a6752f57cf1e7b764bf7220e105e2" },
                { "fy-NL", "1f947fb00a05954eef742600f1cffe0c1d173148051a3cf94bbc6f83d2744c54618059461e9176fb70c9fa1a5240fdb62d2a76ab86037ecf07514d740c5b8032" },
                { "ga-IE", "951474487c0be925d96691aaa97dc1f35d5cbb3f63cb3231cc35e2b3914c712221adee86773a347ef76361058334b0a92083f184d01df00a2e16a26ba4847692" },
                { "gd", "bb11d4089eecdcfbe6bf928e0eed15966056ebebce5721b75f4fea6e3ee843b623f9aba168737efb808cb02491aaf7e5d7324e87639acf7e21b406aa6bf5e299" },
                { "gl", "b5742a84064ab0797f4590bbafc968e972521df3e4607315451a6fef1bae49faad1227620d0bd233b43f4d800fab24f7c1666d9fa60ddef07bccfd12c7345f84" },
                { "gn", "cd4d3cb9486b322bdb8a680c783b2459279f9926f0e85d162d79678e4007ed335d0f399a64fbf8ba224cb80ced57358457fb091dcef17d06b8da1df2f9cf8f1b" },
                { "gu-IN", "688d1402743830ed2b7245c291d573ee4016c7f746e6a66e52b89a3260e841796dd68756fb0999a3bc238f7e701b6ab014a58533de4b9dc39ad78657d6d32190" },
                { "he", "d2446aa06db0711eab00e2d206879a8885e6c1fc8c44318150cbd87a14de38e9b4c28aca6f128bccf366e7c2bd677d7cf4479d049c036e81833ad4741c7b409e" },
                { "hi-IN", "d81b8231f901e546883f49153aca49af1a91a4bd401b78779462a9c621fb40a5af3be747057d66b1ca50b3441efc81f08da1e80ba50c2eb67561751ff87bd4fc" },
                { "hr", "46b7c0be2750521806f28c74ef22022f07fe055dd95083491fa4677f7efa7e8daca1c354ab60ab9ebd44095fe8ab5c774be692843ed58948ca5ef066f3178732" },
                { "hsb", "2a972da9c2babe15428a4c878938f1a232b24b026e30db8218a1c1896ba1f8dc2a8e53dd12c223cf7c6df50c5b529ebe5ebffe9b170e3fd4dd6ff9026002915b" },
                { "hu", "dcceac0b664766115b4c2692c9198c679afc4ebe0abe1c7f338faf0eb237a42902da039bf5fbeaf02cf27b0af30da9e556b778f1973f8b45c5674a0c7226a0df" },
                { "hy-AM", "5e237ee9341e12e8de76656d7bda33b964e0710d21c709eeed6292d5e803161cddccc7e4c3beff149d33be2b41b27928176108e7d39778e7f9a16abd04d29289" },
                { "ia", "8f37fde9816a6197655f082008a33a1d5bddc0b6ffa9e572ee98b7f01f79626023705ca2ce2c7eceb513b28530d55ec46d2eda529519323df1ac16ccf951e13a" },
                { "id", "58eb1b655d0a657fffaf9a3b7944eb692dd03d303c0679c49b6ce7be010405ed7250684634a2b3cdf18a91879226cc2ad0ed4172a8f75e4fd469f98a4d30ee8e" },
                { "is", "7cb500049b0186cab7c913334f728bac795afae2c8b893a319cb8d92a57a8d1f18347c5f756ce2658104c8531ecb273ad8d7860bdf5905458517f70bac2ec317" },
                { "it", "549c3eb0f3886b7660164735b9fc008ede86da1b65ada0709f49cea3e1109f3f4986726bb7e63b3a14595ced7f4e51e8f990665df051e80e6af255e6fe8ff750" },
                { "ja", "a01645131714ad1fb8e3b0935669330a7ee8023e66e86b3193bf045671e6712e7abb267631b270ad58d83dce9d169cdf483d13eef1664275b62436e8157fc69e" },
                { "ka", "54490a3ddb17c5f275d26ff6a1afe3b1859466d264a1c8d1e5fd22fec6ce0e54bc341d9d9b1131875b3c159b934a74ecdc11ed7f9aca2742e5d3dc9b1fc50150" },
                { "kab", "d265cf56e8e88dc5e40a0e642251a8be93915690bff878a93c6dd96dbdd5e82fc41f3bb733b286df2c368f52cf91b52d52624387210e4b5dffd42e8d820dad8d" },
                { "kk", "523165ea43401e8d00472ef2c65a6f1e0ad61646edf84db0784af5723433c150ca71ed12323c52938025ad14badb252d97065edb36de85291d1adf86f3c6f20e" },
                { "km", "93405ca2a56925f1d7bcbcf5651b611dd1ef942a2cb442c43241a3e599f4267a2077e59be57adbd52008d996242c3c75ab634ad0a32cdef8540b2b22b10b1b70" },
                { "kn", "db277a6e81497cfa10d3df6d13422a6158c4ec1a5721c273697f0b5ccc1010f834661bf80180ce4d13b3dae80c40485fc0ad523a21e042d8daf37afa3c7dd8cf" },
                { "ko", "b5fd6afa6532b81d4a1880f8ca5c12abea57c922766b93505edf2b2c82c732a50b6d9e0657651853488020dfd377d668d65e8553d3ab62bf1d5d81991d4d92d7" },
                { "lij", "7e1acab91906c93f8084fd4172dba7cdb2172ceabc6986605e094c2dcac95863fdc81598ab3278941f2071746fa8bc2d272a0d74234db51a48bda608fda060a8" },
                { "lt", "6e827d9a563aa856a07a13957e3f594b26592e5b04fec3248a5a943c781da058f484570f6a313c1836cba6ae8e63f3acdb7115062b5a07d5c44c7388ac78b039" },
                { "lv", "f2ba1927512264cb466a1427e5c81b0c588111007b21c27a12de7eaec85b946df0952d01e6d735eec545ea252f0e4f5ab3a1d5f3379a5618d57110484bed81b3" },
                { "mk", "65222c7d56702d5ddf4b21ede43bbc4190682f29907ac8fc05b4480d35804327fa81992dc827605790b091f408e3fb7650e19a12e8b0f08cf36340388af1815b" },
                { "mr", "546379b1bb48e4b1c202771695d249b4365362ae2d2e8581c1e30eab1d147f01c6dba5fb335a30da4549fc2d7853cdc54b39cadda5a734ba9ac1cde40c2d04a9" },
                { "ms", "389539deb2d2d544b9ccbb6507cda21b9c4c07c985482c4f860dc2233556bae5657aacdeb932431a50d812ac2bbe3eb8d9e0f4273a6ceedcceee65d1228bc085" },
                { "my", "04f04257720651641730682ef50395fdfcfaeab7459451bec42f28024307663f294462712e7d5325d0be0f7885a5ec39b1e9ecbb4d5baaeab6ec0fd00d46380c" },
                { "nb-NO", "1c1e511e18e9e5ba9e9ae70194f663f2ac1c21c480555d541889649739bb79d86bddd1f1209409a66049714d7f256400b5f5e6f2de451091cb969b216276f61a" },
                { "ne-NP", "a7304f415462fac72d21df27a70807cbf958c3f2aa196a727ff0b6b1ff30b1594ec1b35f58a3013d998c2e1b9947c8b7034b5f979b0ebff3fc0e1e5ed40cde54" },
                { "nl", "aafc81e3790f84edb38d9dba4748c95835343ed693a27c06291b411519334469357e44faf52c3d47b361b6846c40bab1537d1f2bc858bbcc6875488620c2fd8a" },
                { "nn-NO", "9adc6ae260c5292b2b277f8b906d271b4b25f21e51ee4be2c351664f98ee6591f0c6016fac53d4936c1cb02831aae023a206f1d490245b128065e4aba62f6ab5" },
                { "oc", "99991c60407a76cb29d20413d3e50ef050ed3d4fe8355229b9dc6de5dfd024758811b9d46a7c226f509bccf026cd9dfe3271387d169f488046186b26f2ab29f4" },
                { "pa-IN", "6e541113ebf3caf704b25a2c9d771e29ae0844cd696f11f8a188f12aba30544592f47500297869455675cbb9930a6dd4e822cd9d9fcd3ed1b3995aeb0840c6c8" },
                { "pl", "a7de3a14fba910d87f80d55299174831131fef947ef9addd098ad19dc836bda218fdfc6663e87ce416f84c9348da7849c99d299b04bf5800861880d3e5156653" },
                { "pt-BR", "eee4eba00392353b84c2e99cbe973453186c7d24abae2deef020760d0acd69f634a0171d45fffb7bf2125232b5c474aaee7097dd24316b2fe7a7f9c2d7bae01a" },
                { "pt-PT", "ddc7fdb32ff3ce8b2e45841833fe6542ad048702c706e111ba04c05afb4ddb2b39c44e8b6f08e6e6a06ffad90215b19b659bd448b5d0eeaec7424b6ac748e293" },
                { "rm", "34fd8767ceeb34f3744fd3f684f785e8feb98b9febd7fb886b467fee6c28a1dd0d644ec70a9605648c16380d844e576f5c7de7fffeb6a086678d7b5fe7aaa42d" },
                { "ro", "9b55336b47c85fc6f2d04f5ad1e7978328f040e51c770352a68ca61413c1aaaf63a415f23f5107f6ed39f6875db515109c8ef87f1c014e40db78a88405770e12" },
                { "ru", "4f4447dd8c393ff5a5a25dfe2d1cc541c1a2e6a6db13210dbe5334c426dcdd41f6389e6026a8c232a3b2af305de6a8ad2fa9141287c823fa51fc02fc93bee9f1" },
                { "sc", "1809d7e29ca2ad84eee359d18d542f5b1e203d135dc495e53c3b67eeae3fa54ac768b37a88e61897f7181fdb617d319c0f8dc1d420940e427d7acd546ac29ba7" },
                { "sco", "a6bc61a0d8ac9ce2194264c8dfcd7900beeaf1d198ac5c9f1bb9f3ae9d0b65208c17b97ee334d3a65888e174e4f5f18538f2c1a31b7446b4d7149a04932c287a" },
                { "si", "f5a92f9dfd82b035c8faaf6bce4d4edec5ca3f7c3b92ccbc7045be988ef08f8346ef6d2dd5a0ec7905b5f14365df240d44eae14878ac9f03c7690b89a0b18fde" },
                { "sk", "46e62e58a97b2f4426339a0e995dba4d51a0d677e6c70dafec4369019352d0c4054f5e28bcc5a150194163518c12089ca58b0eeb93d88eab2ba186b32bbd9677" },
                { "sl", "2aafd4a8560bc7771028e7fb9758312992f98e9469de9b3367c1a56d8255f77ad9b713a3c18bbc475b90d6b701f4ef478c2ee6ed97e4f76492ee10543297b2a6" },
                { "son", "af53d48d0e80445f0a4aaeace8dc9791f0fe9672011d5017db50068426e73a9304d7909a8789b971c4ac8f0a7fbd7323b8e120808c3ce9ad0953b6e8f877fe24" },
                { "sq", "95b65e271c256700ac9bfd685794e567bdc7772a0784901007465cb5d6caa9474c5ace5735fe9dd34a67e744c0491a17dd623f9fc4a23b37138d05f51cf51656" },
                { "sr", "dbec24fa514d5ec08e0e3efc871d37ceccbbdcc5dbd9bf7594a13a2a4d7f485645c17a467769db144b960707b17714b07392e2638d0f55a01ac9447d3e8c5bbf" },
                { "sv-SE", "9d8384133f6d3344392fa7a47212ec49f5e411bcaee453e0c523cf7bfaa398246ae7e9a6d8a9c2e1bd927edca0c0154227a3b2071ce8ebf1d12b7a9407e8a6c2" },
                { "szl", "26a7fccbbe7c396af5a50a692834ad48cd6fbf7684639e5b1f7d5058637426e8d5890cb0de71d11cce8ed14872bffc713e8a4fa9adaeb469d269a5742732b7f8" },
                { "ta", "7dfd7b31c683667a76dd9cc63eaec4ebd002c1f39408d8c13b2732b917f218fbd15bced0190d53c73a72dd628e16221dc7b24244ea1e837abda494eeb5d63348" },
                { "te", "55203a529de9baef389bf020cdc82f86ad39e2c9f0375e248271b9cd8b1fb6c4ab320c44618a6b1a8d97a7e7ae5b74f6b1edb41c771ad06d3e21f02bad0fa71b" },
                { "tg", "525e17f5f5c4865d65e5b1385b353f1d67dd0ba4b4358265bb5849c28d334eee498b59bbfe311211a637223636708244edfc74b24cfdf1c178174ad715fc8739" },
                { "th", "fab1f46265f554d91851653a2b4ed3eeb43af546e1219d7d02c1a84f5d49ec5ee3ee2510bc191da079b9625aeb2f0445269a5402906f64f53620f77504292e6b" },
                { "tl", "901332a5650ce471aba375ac6f214a932335bf10b6b04e977273cc595ec24426e1cb4372638952794d785bd8058ab7f8a3a0498e80382728687995a93569c0ae" },
                { "tr", "2a59a4b761c580713e3a0c91c7f5aa4cb6d560572f98e3fa4974cbb431dedd54d93f1d460c5d4b90bde8c6c0bc18d23e09302072b1d6cf5d8a25776ee1620f4b" },
                { "trs", "307a2fa07fb7fa72d678213e673c57f49c5144b572b3bad50027c938f80c4a44fdf173e36522868a815c1fd55ef3c6d5607534ab98d157f81d30ce01862ca76b" },
                { "uk", "86e6bc0cdc65e20baff643c90fefce4c674f708f2b90f998d8c933f37a613826822d818b1ec2f3ad8655892b83634b4c06bcbed0f854d1c12897ff14fd71cd13" },
                { "ur", "9c5e6fc1aaf4afba2a812656c59a834f906f4cca050733c56e49cd2bc4f48934dd6f1eb951eb5afb51217d0647d0e0e094e34a4fdcb42dd5b71930d8c2c9bb6c" },
                { "uz", "be42ab2ccbf4b2527c222e4e35f66b59ab88672e31436cf2ae1235c5ff0507dd81aac63534656402f39b5461ab4768c351496817925e4d01dce3e01376fffbb1" },
                { "vi", "bb91652f215c0118a03868a900248d32933a54e1ed540583e613fedd35df6a7633f5521829e7e3b1a3e729451b106a133f7c905b2a27c781f6a942ea70f944ee" },
                { "xh", "e16ee85cda436342bb98c536a8784a4849721715a03d263f02a29ea7e53e0545bf1c522a82dd0651c33a7c8876f7ea59b6f45dbc6f506dc07b18fa1de512bbcf" },
                { "zh-CN", "dd73025accceae3244acc15e9b2c6b01e9f19db9807259a7c73c7c9db3c14f55af6c44df1d1609e6519bff6251d5cff21a205ac375fdc43a3bb2294f97038400" },
                { "zh-TW", "a0963e819963443ab0ec8363bde2df8a09e46f23d78e08f6dc352f54f66ba92c3ce73832f96e3f24d6ed646f6869026f997fb4f662e57313b4905a7dc42e25ac" }
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
