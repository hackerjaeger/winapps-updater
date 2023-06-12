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
        private const string currentVersion = "115.0b4";

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
            // https://ftp.mozilla.org/pub/devedition/releases/115.0b4/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "f138b9315414d54f14d1928a66ea7f238404b1ec0dffd2850289f691cf823745223f76764762383ad7c6b00a7f5b105243ba05da83e10944c6fa7ae25e006b12" },
                { "af", "e12a9000a01fe5d9274ac4f7cfa995d2917cdec1ed1028cefbc234427c6bf2759cbd7cd1fdd650a0981319459b6dee28af5387d36a557412f3f4d94aa0afd19d" },
                { "an", "a9d64ed6540ff25cb7a93132792357d7b82e30b7fff6d13a35722d6abbe3aa79a977154a887f025998dc5849f00992913b51fdd6f708367bad78f436bd262a13" },
                { "ar", "96d798e997696960b8fac28b53b6ea1fd26e15883afbb9fdc6e96a9dbae1068e762965d44381ecf7fe4ef19e59e7f830e143ce937729be407baacc6a9911b272" },
                { "ast", "db74446ed3f834fe586f5521bee4cfb20b839c337def11a444bcc334bff818394afa4f0a6840606bfa945b663e44e4c8ce3c54bbad3ccb63803a6a35c09c524b" },
                { "az", "a1e0cf0558bea4684303ae100d063ddea5f03173b39140167f213f97f84a9b6e3da5b148ae80a0b26d2664fea492f0f0a4735de7567c439515955bc575c1b074" },
                { "be", "cf3c62a62ca5114668ba5c6d837943b657e0a3d33df4c593b8800c6c476935ca676b7273e593416c27657f1ce7e833adc11cda5cfdc53330b8474a9fd3f6d3cc" },
                { "bg", "eea96bffb400a1eb91ff148c456443e16972331135658942284c0e199dbdc1511c9ae6efbd60c20ecb8fdbf59766eb3d9300e3dbe2b9c142f7cf1e9e967582f1" },
                { "bn", "953530de4e1c4ad46cb0e6484903c58542b02b6f9623134e4f76c3d6e8c02783c97b6361e6550516a2e1a0b4e23afb6bdcf4aef0364699c70e95f7205fc13017" },
                { "br", "a69d5d6fafc33ed158a86587104b8bcfb30e32c1953c02238aa1bc1f2b85b11b9d170c1320ea9be950824e39095318934c32af8a0848ac8f284fd014a2cda982" },
                { "bs", "c2462710c308f3668687e58f8a8108fa00f700cd26c0a68d183de17296622cf74e498ac4eecba365e65336425fbca8aaeef521d15a8a1ff14a404c1edbff8061" },
                { "ca", "3506f6951d7d86dea91dac5c66e11df93dec5799f09d3e8bd3b13282e3630b6b9515346dd426284159b822e53f8df6b66c1a9a9625dd788ca6deb113c6e01629" },
                { "cak", "b3c41a9b4ccc309855b1cf3e8bdbc963b439939a3cd2b705e9a69cb14c274de7adb3b2d88dbccb3bfa2c8c83ad2e1e6089f842fc7abaf18a66faf035091460c9" },
                { "cs", "8705dbfd0f28765b1fe21aceba3e2d3b74fdcf07f68c74c2628d51ab4e70d3d12c20470418862ca6e7b57a21e6440b814c4b2d73795a1a7f320044ff4be4a875" },
                { "cy", "22295e8a22879a23afceee374d2cf2100ec5c21e330f3419dd6a77f3da0565130be7921199d875aa78b62d284ed7d505fc4288a53eb10a7f9eda5c735689759d" },
                { "da", "726c14d8f6fb63858e46293a8d79201bbc141aa95a49df36bde96b5f960d95a2e3d8ac77d0bc7ce38e45a529691802662b12671faaac5a73336074b10b8aa2af" },
                { "de", "84444f2a165de694302912ca468b0c18c72657ef02cd3e863b73cba5162150ac23fe3c20bcc90ce9674e162a4afd099204fbcf26d326d36881495d376dbbabea" },
                { "dsb", "1b25d33fb2e3555a9bce54e234aab3ac7ac42853496719cfbb3b260fa7d62523a829b0d7200019d6489de196bdd20b3a6eaaccbc826fa7340cb95062bb74d2f6" },
                { "el", "7a19d83f4e5b02970dcb890060a80300e23501509ccaaf99f03493694a3de8b759fabe04c0df0c91a502c33b042ec8a315f24a827fa28255b0089bedac224a75" },
                { "en-CA", "bc6437626c42ad72a98d4f94eb2f36eaf14a53d00c65df40fc3f7607deca26c478d1c30ab8c5112eaa310054c855f92ef8678712ca62f4162967332bc18f8073" },
                { "en-GB", "594ec4023b69d4fdedddd622607bd47c9bfbb560164a308abd8abae0fde4b4085a3099f573a1f6021369b9c8fb63b1d7f9921613ae52d1ac9fdc1cefab60e3d1" },
                { "en-US", "8ab7f0c4e953b266371e0b2e75c9886c74ea930b6724f4e0ed2586680f5bc073fda6e910f2b6fcf425c61fed7d4a99b78cc304ce6985fdee0625a056fea608ce" },
                { "eo", "6e99d9ccca2d3942309ffa608b0a4b759caa8b589ae87f103bac1c431454072c52dff05d91d9b678b0e70500a4b3b032026b43ffeff7ffa9008eed6d2dbd8eb2" },
                { "es-AR", "acfe61ae2c7b74c50051e44edb6b327dc1d1ba6fad56d1dd686883b44a0b1905efb4c0fe8866b09d4d6c90d7acbe2433179d0dd2204f36ca4f630b73779c907e" },
                { "es-CL", "d34d3fd0fa8f18b3932a3a64e3c32eb02066b530a0f092fd49bda9d2d59d1a6bd90974e6709865176f47c604ee94a2283bf4b0d5467746323677b9c41ceaaef9" },
                { "es-ES", "0fbaecebd1cf7d1ea574c15829d23c514371066403f3787bbda3bd9127475d375c17476ab049b96cf0263830d898eabe8cf2d37fc783378c3909be7f76d3b0ae" },
                { "es-MX", "5e3e26ae582d4a5238d44e168fd520ce95dd65d128516a10f76b06a7a7997feaa90c0e87f245a89560f773c123ac900a93833c8ce222875cc61f7c29a6fa8811" },
                { "et", "02c75922c19d7b42155245cd525c8303710066b2637e9127e1a8cb1b2af5ce934d610ba2c1ad1efb22310651e5436535b136d114b28004901d673e264c63ac26" },
                { "eu", "e9bd31f7400eff610efa1018f9cd8a169136ce1653ac8c2164453b4e0d1f6a70dc0979b9c221c1a468241d7beebc8574a29922c0b7db9b020178134c2936e166" },
                { "fa", "c6466a01aeecadc2b4956ca9a1a4e08b19a9faa949d4a7001500bebbde5ae2f17a340389a0f45b66dd944bfe47502a36d354213e8d3d318d2350a1d7250ecd23" },
                { "ff", "609a7c24f166c887ee418d77b1502af12a88a1520f0776a51344940b9db1a0b66b6757e29ffd6209dee3ac117e56ff935741b81f4285d18c79dfc9ff79b6dc61" },
                { "fi", "97f1761491ef0da5b0aba3e40631531f64c224859a3210e1fbd40354553c55c5e98de193ae2a7ccbf5ce15ef0088d10f202ccb9fce61192dd77bb28f0e0552f3" },
                { "fr", "501b2d10485fa68417a38bc72cd3b365df1cf043e2541f5a19d9a9694ac6bfe5e1c8904ebdb60822f4a86df948521b59232feb33c09546a0bf713391a0a04eb7" },
                { "fur", "cb72f3bcba301dc99e3f6296f664870b5c05c433255f651c3b42790cdafd8a5b2356c08c152a9c876f3775652f106b1b80cd3c348a1bd1eaa2aee8cf23fe024d" },
                { "fy-NL", "9fc4717b364b4036f86be940dbedd29d080107c115ca1149993b19e4fc04a31262b008f3441d3701cb45b33a2651e3391128b38098f3846c536e00d2ad29a617" },
                { "ga-IE", "730899892e3af84492acbb5ab1721788f76bdaa3e1e506eef8b19eb95a90a5a0dc84f7db584b0298a8d2274f4c22d6c9a2d08952c92d9cf4be1bea6e5e50c87f" },
                { "gd", "86d3276b8983ba75450e99496f381481d2fdefa6d1fa494b513b5a561d82409f545a01cd2fe339e09648cb47e21ec57c967e193e97281cb4e0500ba0b6afde2b" },
                { "gl", "f1559ff9c357bec7a42cf781eb75ef6bebab5ac004d852a83e6b49e967495b804f39abb0f04dc6a20fb2fb6e7227dc109ec0579c15b8f5a86dcd9181a9db93a6" },
                { "gn", "74c43119636f1c72250164a43a0864fa529c08fec229865a1c911495dc897999a7183575a9d0b06a5b8881dd695abfd23eed439c9a780d5435b40fa9df4d7e90" },
                { "gu-IN", "a71c81cfcac62d3618bf875ff54d170aa26cc6125a50c259a51e4067bbcff53f68571e22da0aee636dc4965ad3dbaff677f66d4eb07368245eff8d5fae595907" },
                { "he", "e198a946b416f081f2530030130d5a8f0cd983596fc65479854e2e8f9df2932b44442548bcbe117bdfc00228ed122888bacacc89f7127382894ee4448de433e7" },
                { "hi-IN", "317903bec8e1c064f48827cf406521927c842d8c8c08fe955938596a050fd7c4dd1c1a09f8841d36a7badfac23311dd861530860f756510045baa7b424cb4e06" },
                { "hr", "54bda6f5469cd31e09c8da48483d4a2186c551b1e19ec01f81f1a39800dea94bcbcd6000e45030d7ce781a78a7c25870a133387caf5315713b9448266caa94e8" },
                { "hsb", "e05ba339f0987c3bf9f387e83d4ab4660234178b38c22c4410f8739298ab0babe8de025fcc0d5c11b0e40ca2b4ba07e95242bc936275a2de7046dff62292c4f4" },
                { "hu", "dcd34384e72282c75fb03cfeb0fda6a0fefa923dfde2e541b39e6cd35fe9f107f4fe21e1ed410f9754a3e7c1c29cde54693431c99ac9b38e2b7b1d021005ce62" },
                { "hy-AM", "423751f42ecb220711da67651f520e56a0e7b603dd804c0e344141d715d6e1f788003316907eccb625539c7c265dcc1f501ff179320929700dbb874909e801f1" },
                { "ia", "df108e253584efdd2a2c36c8b3fda770a4681137bc1124a7299920f3c6d933eeb537704d2b619614a420b81fa67284b76119879be3324b83ba1da7d5743722be" },
                { "id", "6b48a3a7bdb548919d44b9adbb77eef1e3e296aa869e8b8143f600ab23f4787cee16dffa91abe9832b54aa90a458e397a03d7999aa7dc7771dd529d2a8a4efea" },
                { "is", "d6ffd9d3b7d9d69c66d9e3852a91467ce4a713cc9f936e298b297ba62148a794151de3e8a38c19f649ffe610a56a4b24514785c281ccc902b3dcf451900adfdc" },
                { "it", "1072bf96abd79a0e3fed0855c603b3c2afdc23c23c287e115434b501f012f7a765f912aebc7e54ebbeb938cd24fde762988f5468be7eda96807e74999a9e600e" },
                { "ja", "206dab46fbee14cdf62f7b9580d015125e9636e4fe64e7df3daeac6916761e1a27704f1dc4e61afe530111a7a3dac4a34e2fc7bd30d18aa5944490e635ced6cc" },
                { "ka", "bf6b3749d4a6a943f409c1addc0419e26d3dc0b6d2410053c57f0144f5d407d05e9fa29d3cedd3cadf2e7be15a2ed6bd30d6b8a79dfd5160a5932726b7cfe3d1" },
                { "kab", "d7ed30d2c9b171e56e3369daa52788b82a3e4b064eb35d8733bf15a1448c535892c143aaf6d7cce75d52d4d5550bc8f93696720084035e11a6eaa579e85ba474" },
                { "kk", "8469a2e0135daf7f26a569ff0c5ecf1c97faec4b4bd73a210eede70f4f5f2edcf985335d28a18f88b89caf9ac8025251e31cb9453de61ad192da98e3518e674d" },
                { "km", "7c4bc03a56d452cc4f7451696a0cf97d7fe8fb352f1a9b499f10ac7820a6c1be8276afe8654a297e594a93ccaf6e6be38f31518699f4f5513aa6da631f992fa4" },
                { "kn", "a749f4fab0f27f17d7e3dfbeecc3b0c6ce3edbf14523727dcd2d1b548d47ef966acd98381cf6bd36716e993b5bbc1dd8046c74e9f0761d39017dc7e665081af3" },
                { "ko", "b2682ac053b0da9a0a541da0cbfa7aa3b3890f08eddf9cb649203b2171bf2c4646b149206ff6fc91c525e7fca2f52c2e88859bc9bc743b19752427fc3e62a539" },
                { "lij", "0c77cac1555d293753abe707173885e4a106fc018847b46e9171e69b8cdae57b1f647f0a0131082b30cbd3ba1e8025c8b5fbc5803cb0be44ef504cf2b47480ef" },
                { "lt", "95284ee6dfff4903424f9c951415e55534dd5f339c0964e00099c35630e3a3fd9b802ba974de61536a2e295562ce302c7dda5a52d9183b3e8b061543485b0fb6" },
                { "lv", "5ed763b586f9cb983803410f70089bb63032a62255979c630467987603fe5ae3b80d416cf897fb60784442784d360c7d0cedcc0feb51ce86e254391be906ea27" },
                { "mk", "715e4ca776b90dd5a23eaaaa2dab0b49998de58b282c233071c44c42ada5bed5807c36c56639d3a54011e61af6d0b0fc2ecfd29f6cde943a5eb46a097637da8f" },
                { "mr", "11cc41187e3e4150677077a920a5e99320c1a9d806ec0b78e8ae276b8e0dd320a91076c0a311bae9ecad00fcea6c8808b984ab0793931efdd3dad47791634031" },
                { "ms", "2123657297bf8eea66bc9acb3c93bab567e6de59104011ffe43e875f62b2a8e4c81a5cf8d3557b13957fd9e2a81ed58f2699bafec4438f79a2790e169b2c8d71" },
                { "my", "b7af28d39fcf4911594e280f47da8c240e64ab741cbad2d90674a09e7e518343934477ca540bc146781bdd8d099aedddf3ff805325c219bc66f9ec486c9d683c" },
                { "nb-NO", "a6cecd1e3f32d3b4c8605789d22e0d6a211730030d359f86b79de65f0a6927577f175a72998973c2923f21bcf73867d32c8ea6a2dfa23c68fcc9ee471641bb07" },
                { "ne-NP", "66dee1a2aeb85b9a1b7dbed8f7d3ee8e6e7193ba21246f01aa24e6ae40d5636af66f0a08cb2d787a6829b4cedb2d023f91b6f19f4ca6848baafaf3d941e11114" },
                { "nl", "8fea804d1b62dd038799b4b88fbf2f7a7dfce3f5304aaa379173fca26fe3e661e195e5ef11c6b2472e86424209508c4f213ecb3c8a28b349fdc5b5cc77394a11" },
                { "nn-NO", "952c029c0491cb0cb216614b116756c7aca5cf9aab539137c41226511051a107808a3abc4b57bbfcb710a9ed4aed3461a153e078709de0fd0b8c785ff700e940" },
                { "oc", "cf2a93852bf445a3feef0547d4d1ec5c0b2022a81d671f3e530bfa4030f419571f334eebe55c8e709e6db2796ff4314bc72eb8d1d3429e2c63f74ad2686cd5a4" },
                { "pa-IN", "f646fcde9c1231cb33e9825dc576fe74ed8c09332e0b8ee2d8cdf1ed5249e433edf5136517babea3152c80a19e9a98f551ca1df89e48970a3606516ac6c897ed" },
                { "pl", "cf642c52947e26137517aae2336a1b42b1d14020b1a3584a413c95aa93dfa4010716bc0169e963af8b2791f99b988a1ed312b7705d3ba7b73f0435c6473eed21" },
                { "pt-BR", "ba9271e6d1419c6e703b92d3f9e4d6130ea5f2b2c0f4e8e896e5b65685838319ec072c5a4e00de15ed8dc17f3a6ea3ad0dce73abbd50039f74d1d7c82375e59d" },
                { "pt-PT", "492f069c1595fd60018a1e1a70f1501a4dcb55676e8fe56c2e34e0c54e6b987a91381a4530e29e78bfc95f36ef4735f823f3cd0cf4e31b7c3481f4866206381f" },
                { "rm", "1dad6a2a48c51bcd206b2ad518b6d449c5365b69bd6e4eec1f195ac1bb6a75f62d81e08dc9d03fff0a33b9e3871a0767e5af1f1f51e302dc45e8e4c3073b3ae9" },
                { "ro", "484c45f8633c5a13215af6bb997741bc05cbe170f16d0b5d40a58d360491dcb68eec7c87a1fd0824d757041bc558aa5bb57fd662b8e2bd2d35d34437bc29cd44" },
                { "ru", "223cfbebe2efa96415946c27c2a5d77f62b81d8a63ed4bb1b9ac831a148d45031e63e38f923655d2e2a1fcb1de6f902057ddc0d77130ebf4f2ff6a5f2617a592" },
                { "sc", "cefee1224ac572e314f4aeb8a20a1e1b4bd6e09faa794e88f64852fcddb39dcfb8a70295a4c0d15d33be0d08c8e364608a95fbaa2477ee7b9a9bd78fb7870cc0" },
                { "sco", "8d1b63f3ec83fe195e0bf46a40daccab043ddfcb9b2e257402c9f812c907938cfb65f9b3e6a4067f0a1f93f721d57b820a53f2584cfca1e9f7c5dc069bd825ac" },
                { "si", "29802aa46aa204a4ba59c3739269b4b357018c1f1fe40da95acaa13da7d051fe9dfc76db6e8f7bc24e6359b72285c45cf6399796657992e6d7163fda5dc5a9ac" },
                { "sk", "39b87896671f0c34b917473204e3e053b75b0efb12f4061288ac7f6e9b71cb471eac033d43c291bf17473f59516928cafd317e0eb7f6cadd23c08ded5829cdb5" },
                { "sl", "f49b20b9a49bd4ff4f4c571b700ba31b41c6602bbf09aa6b6b399643a61fd46f163ed8f561c511a190041e4913b2df79dde4bc493bd6587757c126860dd070ae" },
                { "son", "8057878a565077ce6c4a36873d9f9aa92659b0b7371ac66db1e33c99e25fe1b82d6d1ba4df2c5fbc02f37506a56ce49fdce46501076aae4246c67af5280f5290" },
                { "sq", "52b51f0cabe80bb70a1934a21f1d159c2cfb792bb18afd82936d27d209cc52a7eb6326d89c74abbd81c15832bd7b22c35e4710ff5f7a40a7b87e57c52ecc077e" },
                { "sr", "05dd1c24acdd3e2b74667c37e69d49d2846c7420e4dd929804dedaaeecde614660d35cf8748a1a1e4a96a7f3be682d795c9f0ae2fa59cb63830f828284f023ac" },
                { "sv-SE", "11dbf362485495e6d5f1839aef99b229a67b61c9389f9d5df77b8f4619d5f8ee92b203452869ac3e8e84373a923ab087888b9953c3e7ea37d5032a25e5860ec5" },
                { "szl", "8577a1ca5ba7fa6c4e1b08b81559e94ef9e44eb3558edbaad110c242adb44dac72fff467cd1ce4e3ca0b2171f8408b301cbe5f67351ce2b010eb9ddc4bb5aca8" },
                { "ta", "c10174118375214c313fda7828e056b485f4443bb3154ec81eaaaa30018ab2998de4b5a2fc6c8cf064eeaaa963f5d1b5050a6a01f93d791d41af8e84e568b49a" },
                { "te", "91a2717263a2eb4d27690d7d925d92f045ccaab997f14d2a738e076ccb703e0c3592fcd55401c21d96318e4a8739dfdb938b0d6227d034fbe91ce5fa130b1825" },
                { "tg", "ae42ab60d17ca7244d4b71ce6ea8859fede19c5bf5dd7997cf8d3212f9eeeb3bdece2adf167838f23cbef5d516b73ecb9078a445d7e24e94af642823983dbce3" },
                { "th", "a863c72adcea134b059c4bb72b46203fc9f1368a150ed7ab60731aea109bc0261ab7fb6ee1d302179b6ae31cf8cb2003cb9af1f9320c030428df768e78a1da3d" },
                { "tl", "728e3f1f38bcb153d52daa63e33c53ccf5f79bc92c192692da0f964437561af2e52b3f5858333d75a0d57406e68ab8079df3fd9ef890856fe41af13714003b7c" },
                { "tr", "9677202379e4acaa37b42f61c73854ecc6d1f723f671aad6b283913315566c281e6e6f599bed6b96da02f1248362937317afdec72812f1ec642a9fc287063039" },
                { "trs", "de77f14722a6c803f9e763ae83f70be90bcfc60c78ecc7e7de63e6d023ae439c86aa6ba89b8f4574c92566d1b37961335f52e3710a7c3c4350cc2009d02ecb50" },
                { "uk", "c433b58826dd580eb883b808dc7782f984407120b3550e3dcbf175ab16b430b8a6e5855b59b381af74455df72bd48cb0c7ffe1e5f0750873cec80d37920286c5" },
                { "ur", "dfe4144ba06e217441d90d5a841f4d36e106538813350a507fcc8818f8cbc6a4d4414ac294f893372176a80eca793cb1d9383639d37783e92ce9dbe9b6b80bf9" },
                { "uz", "81de5616e890ccf3bd88779d897f118ad789c57980b25f5ceaa4637665e01245e1ef2d5734f46b75d2a2d59f871e3c6f9f7fac1c014b442f6bdb634575b4a575" },
                { "vi", "ef2c87ac0ca26d6e9483bcbdb19ca4cc994b4749765e99b2bec5846a325f0acd42eb85d51d1a7db16b9bfa34bef318ee4a550049dffd8a2094584d2556209e78" },
                { "xh", "9507159a21b86c7a705bdbefba5bad4b3dd53338293e7d6ede25eae2bbce605a7521505e28346f8421976faab8b5afd175fba6da8f46c0cfd3715e9fd5f4e896" },
                { "zh-CN", "680906b9e237c84380806f58335677b4d5bf064be075bff9c91a8ca23d96604ece74bf607e53633705dfcdc18f266cbf7987b372ed96ab0f50d550ee5323137e" },
                { "zh-TW", "e55ba0f1c377a9148ef1138e2c4b8a86a132c1118a2c0e7cab44be8b6b2477f19762b047717f370b7aebbcd28b173bc0808b36c4d7eef1c7ed0ab3227f73a74d" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/115.0b4/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "42aa5bd4b48f176de4cac265580a73db3d834e735e74621393dcfa14bfec805d51cafa113c4d1994b7de380fdcc23b091570767d5f918e6dc8fc37a52f7908f1" },
                { "af", "247efd00a60d0de7a3b8a0d336b6e697544583f891911de37c020175094f7665328bfa800e85da50f3cdad02909caf8147fef441ef824b3d6aab1268457c57d6" },
                { "an", "7c683837c7a52eea421557ee24aba1f7a724974f9b4219a223ef0245dc2f982af423e9ae3672cc22301d8abee2e8fbc4540322e117d527d56bb09886a9bbc535" },
                { "ar", "eeeefab851967737ab5c0dd4e0fecea6185e595f32b06760be756e2d6282514fb0c65b9b961d33d56dd94b8731ebb86813ffe237edf9d47d0f6ae9624991a1bc" },
                { "ast", "b554d23c808d6bc78826c395df8a479574264987d2ca348931aea784df505e02ab57a2b5ff9dcb7c72263d2da234984e8c0e35a0a4e038100106e47e4ec52abd" },
                { "az", "65a4aaf240cbf4603f1d79dbe836e4afb236631df071d20cd30a47f14e71ebfc8e3ba8a9c05d34e90668048e4380c725d7b70d8b4444763fdeb048112f98f1c9" },
                { "be", "f6e80270d34159fdb40904ba13fc0746c4ef1959f6f3bbf2ac03aaab22962a09ba6060e6ac6aa6bbee086fb69213f45af51e5911ec11594a8c552dd3fb83f3a4" },
                { "bg", "62c59f48827852b59a79abf2487e03f174e1fd25ccdb56b41da2f5af03aadf42005fb05a44c497155e57a6e73b1dd836cafd1d99e37ddfbd7dd54ab3d81d30b5" },
                { "bn", "8d97a38d3791e330c3ba9ce58e71aacfb870a3aedd111f685622bfba5cabba505b0f2e69aef0680596c9fd4fadfa8db4d2e3490df3787b17567660fad6686834" },
                { "br", "2472726672a106d34760f3d44a85dd3657997162c49deb7945ce7787837be1ffd9968eac5d12c6671ba8854b6ab2cc34210715c8bfbef499d9fc3d4ad6a11648" },
                { "bs", "939e0f84ee779cc39978e5f278fb0ab14e25ed53b29d75aa5727e4082711faf6bb6e33c0eebc0ab6efb48516d004c891cdb2f464e9ac44635093f7b965ae823e" },
                { "ca", "929fabef1b3f898e11d253508fe11e2b67ba45a9f8f0ad86b3f5a252f8d40ae8aba12d1a5e30a2680a0c90466d57d74141e8dbd9fe2a5db9ee0236f9047f2182" },
                { "cak", "b9ad039c44ed14506693c6e1cc86ff29a08eeb1db7603b7eb4870ae7ae36b2cec67ff7e53a4808e4f084f0d63868518047fb2a3381702d947670b2bc5d132bc7" },
                { "cs", "4c994feef546db06decb420dae916b2c8373a54e0cea1e08d84b851a6af5e9547ae04441e76c7ad53b8fd48cd7ffaa06b0fb1b6c54bd39c36803b6c5c84d9691" },
                { "cy", "f6bd1529117bf0fdd74d74ea130eb1342f8234296e159fcbc9cbc845e9c095022b13164dc25da5170256a90308dc1e9a67531ed037ffa48ecb46817f9bf1a752" },
                { "da", "77addd2eb1bb150e84d748260b5f1c6013614e9c7bf1999ce9654907aee2b0a514973103f5aebdb2875bd29ca2f4b1928930d6ed857e198ae66401df1b37b144" },
                { "de", "08340b6df6200384fe1ac9c7f16c47e6283989d68752f235c6432e89c3a3b34be5a3b5611c8a81823347a0f244d351446a9806bf032b7079c36f1db0791ebca1" },
                { "dsb", "bd5185a4dd8a51b3fe0f49abefcae9c7533d163f99b0fd05118437d9c68a92393a40c416af1e7fe23aaac3381c4bef443d1ad55338e90fd408d80705a361a3da" },
                { "el", "0c7e2380d1551510a71541d619c5ba055d70336114aa5fe00b9fb39e1a10e55a4bd15e43d247fb4de13fca5748361c598366e191a12d5e51e59f41cab8f98e21" },
                { "en-CA", "aa7020653b9fc2030b88397e32d01518e590b9f23d06ad94eb12a2170437a394fb3eb29146e233a192f37e67edb4b09ef178a94af687a5431445e0c3191b88eb" },
                { "en-GB", "3fe8d8acbc7d9fc72754e58b5bd88b688cd5ee9464365dcd644f5cbf9b4125b99dd6e09b69d742d9e676e81299e37d80879de4c35bf748a88eb4159bf806dd05" },
                { "en-US", "6ad1cdd70eb7e6823281caa9a3a1330e970c8b1b0b4c14873d6a9c3f6e5744194585968ec236107937d7e90eb91cf9fb1db57b2beb6346733eaf8885c0904765" },
                { "eo", "dbeebdf089bac631111c868ddf734a3015cf7cb976b816d9ccc3bf592258ac31624ee712491357cd57b3f890d8340a582e1a8c06c904d899624a6d9b7f4390b0" },
                { "es-AR", "b90df02432d9973f24522013b09844a1397407d96a4b56e25db44a71778d66b816a14f71017048e1ccd565151f7abf78195758cb7c76d812f7fc94a9735837bb" },
                { "es-CL", "118aa53d163baa288853ff7dbfa2af80956983f97d1f55087f3460cbd973189e45f56253e8a03b11b9c2c05061597815677fcb1e394d54adbd0350c7ca7a57fe" },
                { "es-ES", "2edd3bfa15c001e0c8f13e1f26332717429e472368709dcfbae3753ab1128f468f288ddfb1cfdaeb19898a9a942ada5d2b9edc9b621ad64596600582d4248703" },
                { "es-MX", "37cd6b673ae74f7423e14ba848bdc640921ae09f9c0ec6ae5a9810b4da7502b3cea0464ad7831a82bfedc1bdae48fb985f86e616f3da2bd9c779f40a342e6203" },
                { "et", "284c09a16adb79910841ed0242a4e03deca77cd42b56d5e7621abafcdf4267230559ddf00d8aa0bbb1983e2c15c9b40a0dcec3fb67c5fd3b9345ebc98a49ea97" },
                { "eu", "3c688ec7620b4cf55973bcc5622cdf811b737eee61aca63f3c65a091a368b2f24c5d5284659b927a11d98540f5f9dba18b153bcf592ef76c84fe8011f8297dbd" },
                { "fa", "9cc1bd9b9a358823c5961dbdaaabe39d478483519e8b3c1b8f1f18424738e95227586f2991a59cbf4c4b557befb3224aa2e248cd1a0c672534d797b6e68d202e" },
                { "ff", "c27564bd4fb27b3e4928a0c708262798444dc018580ce6b02f18b5b6f8c643924f798f0a749bef10dde73c19ebae72c54b12f95d953c590b8fa52f1f2279bb27" },
                { "fi", "386a41ea156039ed0d401abd9d2dd215ac9317dd5ab91f574b8ac055cb9533bb98eb50731fa8e734a97254dca0111f2ce2f1644082b50aabde4478aede887d2f" },
                { "fr", "70daf126b2be3c18ef493c591d365ff836d95e36e2e07dcb3b2ae003e16ff4ce8834c9c0eb268403fdd336cc447eb6c8fcaa87d2cb992782003c02a30d175479" },
                { "fur", "487ebe12db4335520b168f35f5f081f37a61b8671b618d2e346eb8a98c24846fad3f450f12e8ef93278aa044691a9c1f308aa1dcd90420e1f8e1df258ff4b070" },
                { "fy-NL", "c3b0e8970b0b9f7b7ccf4607a5cfeb1b087db51574e393398fce1a6e639bc8a67e8b2c664a9996d4315aeeb937583bed256984d58ecfd4cb80c255c8576225d8" },
                { "ga-IE", "9537250c3807c4609a7a64b0d499525a7fc693553d9fab2455d4dc80ce1ca2573b64b18bde5067ad0b414af9590e46ffb29c48ad27034331c4e17ca7800d79bc" },
                { "gd", "aa926ca7ddec9e86658cbb5f9ea2c5c5be5f46af6c11a29959d50c3d8f45b39053df873076abe8ae28d88eaacc39e4e01d20144f22c5196718aaa3165f9f3e63" },
                { "gl", "1f4bf8bc378fcd6f1634689fc74277e44940dcf8e8f5a6fef8f18a35aa3fe899d182f8304ecfe6ac88ebe22119fc8fdebb637df39db0e43aa36609f68c10a4d0" },
                { "gn", "7939eb7163d4843488030d7a95aad411ae420e4a2f019d75f2e41d3a4a8396bb30014a491d4668e10252fdfb34ce50b448f4bea8b5fa2f6cd28f308b81cb9b82" },
                { "gu-IN", "4dda1aa7e16b8ca7c3b0b3cbf4cfd3d4efca73025efb5df21987f8e9ce1db8f79ed226e8c60012ccd3b8865921dbe2358b0a9de53a6045ea01e460df25043874" },
                { "he", "781e9fab9fee046b77b1eca0d10b22ee984905153666b7f79755b6b2ec5dd427512a4520bbf24dc6394a7f2513441e26418b112be11369f8f13dbd47b29de1a4" },
                { "hi-IN", "a53c20efaa93d1a72be9e659430c6d503c723bcb4a1762d3e172bcac9c1d7707e84495aa79ca1589e5c588a2400088ba3d4e115b94a0b097e9508112af2d926a" },
                { "hr", "f3978157480a0692c8b790ac5473e0b80c81f75c6fd053f801228786a043149804993236547cd3d8b28828fc523ef8ad71a989a51b72c56f9981577a71578187" },
                { "hsb", "9c22315d7cb25be9c2daede5ff3d5e9b6615764ce4573e0b3432e835462440b0d039b715683b681a60aa060226ce9e25983a0f810c83b32a64ff49c9d970c220" },
                { "hu", "0298369d7461d35f86be0e126a90e4fdfee65efa4cd1114a10601b04e0b2d62eafc4553299acd645bce13b6fb6347c408f2ea8eafe883be2a7c3c4315647df41" },
                { "hy-AM", "9fb2d894df862f92ccd118a3f6a1e4501ebe1d989065d4cc17842088af6247f57bd70ad24de1148d0b9db42801bc3c2b0d497910929ef96e8cd1c02201097eb9" },
                { "ia", "072ef6f144d9a07bbd4febf6626b151abfdf82811d13933daf65e5eb1314f36d202ebff027dcc799c9c740e80db11062d35d411fdb5ccd14bd1059bc1d298ff9" },
                { "id", "a94245ec7623ebf00f91a05f502876574bd4900eec4e5adb3169564446e05e2ffbe635d74f9555a2311db4c1407ddb8ed87d5669209f279bc354ff098db460bf" },
                { "is", "c6a6fdb1e14c242cd7d8f230dce9ebbfa64819c6cd655e3c75acce73ecb91caaa799654e25440c5fac9f2ca7fdcecc3c9ce3d08ba7c21d58d8a0d557ef1fcc97" },
                { "it", "0bbb6d7ef4787e4b29d6cac3389b643e35db129feebdf86b5460fff0ac5ec048900e956e4efc776f24d5b91cee09f9fea86110bd8c956b4fb036052066384eb2" },
                { "ja", "6314ceabc8ddb1b9c519353d069c9cf49306b42cabb2ecc05d0281178577707119869656be018fef9f4f3e0a21177a8c829175a83e3c5bf9cf465bce3cf9fa99" },
                { "ka", "546d666cfc614ad84a15152ee4aac1894c6ecee7330b94da5dd9b873cec9a095aad7115d5f5a8067510b573d965af485a631780af2af47464d4d85b91544482d" },
                { "kab", "9076eff01ef854391b18032cc2c7616f5ff8b625d52b0d720e442c08a2f0a89977e897fe28478e50e4c4e971170dc27436aa9a171d348ca0b0d07630a0453580" },
                { "kk", "8f4771af13e586c734794cebfbcf73473232abc12c8d593fd864154969d9b5c4b3c4b53b648e2b6be5ec7f80620043a1ad3fc7afdec147413099cd776d26b87b" },
                { "km", "d20ec07d63b67e745b8255e2879ff853b743021c7a411923ad9080133d195f925650b8c77cc47779b1428ed76da6d73ca26e7a1dd443e2ab715dff66df6c1207" },
                { "kn", "90a6ca44dd75915efe838dbb24c9eecdabc18c2aa3cbe5118fd1e65e220cd3069eea89f0fb0edf34b54d1ae1d7e6a5700d96eb3d12637def6b3562182199e196" },
                { "ko", "ce976d77d34dd533a0b4420fdb41143f4e441603a8b29fc1b4e3ff66e3c0f86b84f02c4eaaec85996e6b986bc914cbd1df26cde60a1b226c1bd1d4ba48580811" },
                { "lij", "793d732fdffcdf4caf1de2b190f39523fae0e648c63d14f41030f25e4972e7a7686b386c1b7ce9a46ae8d04fd5f6ba0b128351d72efc4761505d5142eca16178" },
                { "lt", "038e2b800ee6a43b64cedbebd8119a9f436ba8d2d35bd887d8183a6339ef2656e9eee8ea5e077f6d209df4cc8c31b6bad6398a2e0085b061cb1f4e03c395b30f" },
                { "lv", "3b1cdfdc8c407de1196be688c3da93b13bded648ac5d83a5d110afea584cea78017fdfa6d5555c3f1524daa70ab86fd33a5701a2d34820e0c408f9923fe6cea8" },
                { "mk", "8f8c1edeb90f640569c5477f0c5f1a6ef29395bda6864275b73717732d60cfeb52785591866105731b17f8f73faa133bb10c90d8df65b192356bb73716f1d85e" },
                { "mr", "b923b9c7008d54088da442be2ea4e8b10454b9060e3a7f094ec30e7470de123dedd79d6059eb0afa672977bd83c2fdc616fce82cea0c9e282db0d50d63b47cb5" },
                { "ms", "cd09fe2fac7d58f4c993596db25597ae649993192c5b379524ce7a97a8ff568d250eb821aaa856c18c5dc486f3e924dee676e4c96f2b3ba8791e9a8d06d480f3" },
                { "my", "ef6974069b09b98658491cdaa26f9a45c76ed08dee4678ee0d67ff9ba6c34786d14ba636a6d6c6f295a170d6a0f0adbe75809e02f8b7de35d0d4b7185bbca1d0" },
                { "nb-NO", "f1951da3db239ff8b18975465bf6a3bc1a62c6710b08cf02bffbf713d587d1b5944e0ae4a992487bbb597ca0f288fdc152a1c21505207292efdcf5eb663d121b" },
                { "ne-NP", "be889533f6e1cf59bab7f6ec70d61437de7215fa1e831f7f76b0aed1dbd84dd2f919c4974a11c67d86ec2b9d9f0e9127048e46f2e50b23964db82a1434da7c10" },
                { "nl", "1cf35394b787ec87787210dece000926361527449b2ac275196c5f7c816adc81f991a14c3facd862e23e9250ef293070ea0f181efa871e3f640d054c4f088622" },
                { "nn-NO", "291652da3cc1f940aca125ea85f93138feab93706856a8abb9b27c5d97c8ba6cbb5d1ef6768b3bf59ec0dd16c39e4e7ba343b56a7a0cb457008b55b1758f5bf9" },
                { "oc", "ea7222292a0755f9c2496b08b1cc0f4c602e81404908c166e477126452aa39fa112decdcecc1fedaf2d0a0dd742dc4927b1d833477b871985d55878bac188bb8" },
                { "pa-IN", "c8013657ce32ef509649a03d6739a3a516d12a934b7652453a8beaf0d75a2e5f4d32cc34cfb7f95df511c161cc97e3b8281d329678138b3d7623783d3d15c828" },
                { "pl", "e9936fc4b5b2b8a172329d773fca1a6c6e73718115ac5796a01154072287e059e7c6b536bbea810f6ff2ec2fcb7b77fb65aef38e7e0bbecb3cae9b1552d88e32" },
                { "pt-BR", "67f9e07137fe64ce9cf8e940b656d9f8f88d4fb0cddb38e980cd19545a85b0c1b53384f9fca52bf38620e5d28242f69ad54f0362d69c6f9ca98fb55311c93564" },
                { "pt-PT", "6d9f9a5568cd2da63beb0a2425bf41880aa936ae30371f85a2e924b272c93b714c31c1aab253ebfd4b0193984750ea714fc04bff89c036df80c8d56fb80256c2" },
                { "rm", "76bc536716df9b98f165ef90f988282153c8574fdb57407e93292175735e5763c2aa33dd6a8047052a2e074998d789ef68eda55b9cb1777e90defa50f2c8a354" },
                { "ro", "4a5b48b96f9611554c7e4b4fcd04d23e32788af6387c11a24c212f76a15bc8b984f9d2ce625993ecb67562c2f6a1e58211aaa13ae399aaffcc9dbd7716c1ff97" },
                { "ru", "a45a90930cad3b24dfdeef15b28f8b8a2e1b247f19b5b4b4437520cdd3657bbd9ee021722313bf56f295341f2e9e675faa0ee647002306493cdb0221c467aaff" },
                { "sc", "7b53308ced99741524dc7145045c0b2cebedeae9a8cf0f928dad41efcb5179eba9248c8af6f2665e6f9c6977f499e4c44b6d35e6d1d97bc76fc33d76b82de806" },
                { "sco", "4c71d1bdac6db9c8345d5230aea992908309deef67427d8d0524f0031512c1f49fcde8b4efd366fd8a9b2857f227498b14779ed43e2f233317938d5bac1570ea" },
                { "si", "66b49af5422a7eb53bd48fc527e607e430a19047117534a7be5ed3eb81ee9419d236572e55665a8d77e7ff22f6945d0626ffefcffc0dab5945d68a8a90beaa5a" },
                { "sk", "f6ce7311c32ff1d3ec339ce32d59ef2eab1aa61d6fbab39334c1281c76c4c9a376e19d709d73b3134f3a02d61ecb094fd4e53b5c14a4727b5528e52b52758984" },
                { "sl", "d6b0ebbf54cfee2881123adfaf5c1ab46363f2bea786317f275b81dfef49b3f62d1d9b0ab64e05a3c96de47aa53aada792fe93c41fd4b460d5e6026291c14d6b" },
                { "son", "ee1beaf596bddc4ad6a23e99f5e77989d98040d4f37d2370480e842d847fda3c745d8125c70343f827a74ba35a6c19072f0045485d0471227d67294aaeeb9889" },
                { "sq", "1bfafec64cadf86a8068a006a851859d4fd882995c99e10c26f36ade938221280e28094e13b65eff6d0b302427e7308b7bd0a2fb44430afda4060cdf783bd191" },
                { "sr", "280997f85d3f216fe9ed65fc9d9c2bdd7a558c4e4c7fb5629825181e4624ef627cb7a9e05cbd7f836e3b37a9522837a3d419337f7be42f83da53b5f6adb00dc5" },
                { "sv-SE", "74b4ef554c386b580371ca009c817ba75bd8d3264dcfe86773b7b41d4804015b52b59f75ad1d7db8f9b6f7fa6d8b63b22515153e6682ab0d909e03b9f03f0400" },
                { "szl", "f63db29d1d6b998c21e55b31b0cf22d05aecad2c247f5846b86084d4fb086f213936cb8ab1e0dbd1922e1c75bb754a56ae4c5117baba090ca1e9920e9b5d7ba7" },
                { "ta", "3b7ab22b9577ca20da14f8bf98ea87386d287e572f35c731111f0d30469bf62bf785bece53f173e9ba47bfb19252aa9480fa99576d59af2ca1f24efdd196ea7a" },
                { "te", "ede91f819a0635adea8f963f524913303066b21f596624ec133449b9f87075020d36febdb8e6a7ebd20ab0684803e1b0514a81a9c40469efb7cb3f63cd7f06a8" },
                { "tg", "dd6508c2a68e2d4067214a542afe4f14572ef694ce6f8938b8d0ec239a2ce422c14ecebe44f55d33542fc306c7a5039833c4e7b4f113d90fee5b76be76809fa4" },
                { "th", "50bc06276000a1d6abbe3b04f93256c4052a3d3ed0d89e01aeb1808a8de8bf43a5024ef907aad4e1f20c97a73cb4b224518696caf1c827b647cea266932d2555" },
                { "tl", "f250d71f8a7ff12aba7f47d10db4ba6d5c887a5be7332649a64b28c37b9863ba016ba8264a9b5b681b5b2eef813fe7abe685c8f7aee83c26bd20e894dcb4f008" },
                { "tr", "ec7ff80438733dd93b751edb8d717ccd838f4930f6d77116a610f8d4df872864c72fbb52a13f2bb0da62502f8a99ef6adc4fd8a6884c43f665fff2ad0556cf76" },
                { "trs", "1cd6f80abb21f84f907de9a33e29befaf160f1a2211c208765d82faeb3ac41cae3419c6bc5b1e319139d17044c88a24c2f5e231da68651579370c061f21f091f" },
                { "uk", "0fe25f948aba52389611fdb4f63b6b16661e4b69a1a8a8a16a1d415deefefd767ae28eccf7a688de13ec025a58947e044a4b31cd1580ed402583dcb886082e87" },
                { "ur", "a23fe8b83e1cd30e144e84024dec9368f33b4375fd65d6231fb1803e542f4593c17e6385291cc1a3180bbc5c6dbad866623eba38a207908a615e5a69f1981e9b" },
                { "uz", "da9290adab6028ef9a6270f89a7adfec926e07ff3281a36836c1871cd69e9fc470dd423bf063f6f55d1d0dd411bac7ffff95adbebb6ba05c60386b00b5f84434" },
                { "vi", "3b6b991ffa54af2ab1906b0cfddc47d1434e994fbf7c712d35ffa65e37ea58e7cc7d1fa843c89fb1c8462947b0fad45ccc0b21ce2ae8d83d2c9dc33c4d85f554" },
                { "xh", "bd00f360624cc2a5313f1ed526d715c1bc0d122138ae4c114554a0a72cbc7212162bf4504e2939781bd0554777a701e152fd7c164bed1fce178fd8aeb06bed3c" },
                { "zh-CN", "f65f2554f5aa865c18b6e54a85bc83d9307360b984e1bbdbb6d5598e111f2243e7f9b3340285edd878319182f7cb63c1b6a93c8b8a6efa92919bd368cafb630b" },
                { "zh-TW", "a9c859ebe104cbb31228c301e11204b95e5d3a381019797d7fa07b3ff32bf7177a2597a7423863b645953b4f159b6a71b384594522de4213859cf17189a70f21" }
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
