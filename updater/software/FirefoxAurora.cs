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
        private const string currentVersion = "118.0b5";

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
            // https://ftp.mozilla.org/pub/devedition/releases/118.0b5/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "cead5b14c29ce0edb386a44c312957b7070c5dd93453169f7e8248a6b25b8f1bc1b213e1222cb70a7559e5165a8626889cda10049a93bf07ffb3bc13fb568922" },
                { "af", "2a48424b8a0cd4a442fdaa55537fd9526ac4ddde9210c57d820c1f6cfcefed058001b32d43ae0e0399b87ed780cc63ead5397a1bdd8f94c91fe962ef7a6155f8" },
                { "an", "59551faabde0c390a06bebe4fff5c83ef3f6258f27236b26c0854d11971b984ef020e40cf1da6d58d4097199d8ed1c0a0002320c25378da04c582dcf1a898d65" },
                { "ar", "13b03a38fe442ae6bf61b0b88db799dbf157b469ab53d207618c6f83c0c8615163ef70748a1ed75264b46b9cee73ecd81308efd6a4b744c23b9a3e01239c9994" },
                { "ast", "a09fe2a080a8478a41fbf9899785457e7a73caba3993efe3edc8f210d9767d0f42a41d9b4cab19c532b64193e4a0b82746b034e9916b2b1dbbb07d96bfd482f2" },
                { "az", "592fcf2ffe442f204121b39c08cf4db382b7332db567b3fafe581b0ba30c3a845e4c97a971d998278a3beabbb318451d82f922e199d36dc761d5e9cabfba4010" },
                { "be", "a5bc906adcdcd0fd6d2b53e94a13e4aae20cf35e1a3ea936e88bd62fa32c5c20b17fab741ea7e0a5de8057f20c6f62bfdfdbbe6633e6db8a6d4737cb459a19bc" },
                { "bg", "33f4de858d119ea6d21abc2e1c8e7a9493c19d7b0a0ead0222ce3c1c99b1e7fd1eb63737f8a000ca15627e5f10e5c567fad00249292eb41a755ede0212af5d1b" },
                { "bn", "35e72116ff47a8c9a548a83faecb5991791e774dc20863e9fab3ad18e2e64d8e44ded5ed6360f3cf036f5b755020854655272028db0a99833f3f2023276051fb" },
                { "br", "3ffbe6e25e89ebf96c6a868751e7e0c3c1ffa5fe8550739c1fb33358959794ccb17a814c6e22b82c5c916a8c377737ee7b12adde79fb604fcd5d07e56cf093f4" },
                { "bs", "18fccbf61a116880206ab1064c6084ab9617431dfc92f0f6cb63e42beda97f0d732a816f807c98537919516815a926aba426b3d622f32c6330ad2e30dd481262" },
                { "ca", "5fb036e1ca4932c5d37199362fa0b3ee272dccade343d76d6cc221d7335b00bf3342724fd746010224718aaf7943a80a186fe631c8131bf8682ab8783925718c" },
                { "cak", "b5e11fd13c5d8d74e271fc0b1f940ea81c26c006de9ea688aecb515953dd05b161a3d61f214f16be4ce8dd2cc6de61c24041802ce10d6ab4607c9008c9f623b1" },
                { "cs", "b2a23da15dfc055cbcc46e45c7d4173178934fa84dbd6a0d4361c537e745b84ce682d18765dddb4487cb6bc70f7bacd0a18b70cb53b4f9f773ae2769c2e0fb1b" },
                { "cy", "486a214989524716da5405731bdf305d07bc0001a646a4cea0f033faae6a221913b5206e2cfb41968f94e709e3969f465ddb97b2048b1cd3df504e8b32e6822b" },
                { "da", "3972924ed96621c732467d374be66e7d0f2f0121078a45ab90aabdf97c00e3f758bc5549eb353f91b00c3f5641b38d4ef3cbe8bb7bbb70cdff2bac84bcc2b457" },
                { "de", "a0c4814ef5b9050b0973bee3b13c035dc7e5f4d899e73b74600ff3b40b9d27eb78e09131b8182bcbc4240bce0c278febc13f9ed084519537717d5a3239254ded" },
                { "dsb", "707c6b224308a411832523957d66ac162265808153a4ee56682a8501231c17ddec1b25988c57a9af841a1495f9ba0f108d7c8fb01412afc575cd857f4a0c7bc6" },
                { "el", "f48477ff19e43be13fa238cde5a1e2cd8dadd796f6c706f758632cf95becf9af943fee3dfc481201932fa215bb23d5e7f57466cc54b3ac31fcd7a1f697195ea6" },
                { "en-CA", "98756c1f01416f0a23f81321394b4f8ff4fe1d31a1fde09c186e47f9b54186d9866772eb2d1f6ad1520fac718b5751c44f2df947c69fc6b75972723c1eb70424" },
                { "en-GB", "3cf22fcdf14bd3316a4e962fa9df2671c1b99811ce29d5fbe5d847bb1a474aefe15eecf123cbda1ca8046a9b90299feca697ae3652d92d6c230648f1f34aa4a3" },
                { "en-US", "ff34fa51f6baebe7ef5ff6f69b2f918707ba80bfe2aff5c4f95dfb33ec0b31010082bf9b4752010db981a4e635f81aa6d10ba3aa303e4f82384698f63f958663" },
                { "eo", "8ae6251e784e97124a99734b569474cb9785f6f0e1599cc2e054d98a0b26d3cc137f4a3290283556445e07fd681f1808b22ab3bd6f9aaa3a0ed0fc598a3c5525" },
                { "es-AR", "f7b9dd78a8e4bef7f510d309ba924ed23cb2cd23e9eda4af848a66e20ba59022a6c3f65fe9890d3f48cbedbaeca983948e42cc5145fd9b42f6ddb177d1e1913e" },
                { "es-CL", "afa31d0fb7457e6469062e2fe70d8406f0af7b6f40856d6ce8eba5d7c6405a1d52d0a90adf96532f45fe4a9fd74d395db881d25c80059f714c366571dc91b7ab" },
                { "es-ES", "2a65939381aac57adffda6a27f9db1fa15fd833c77a49cbf257ba28003e429789a19f774961c82dd5b2c4a3f5a7e63cbb312e8b0ae585b6cd647a7dd5635a5c8" },
                { "es-MX", "2b9542413bc7debb2dd0c3ab235de5a070079607270a7192feedaa7c45576f394b73143ef2214b53f4c30f0aa0d844b93ed3d6337e5fdbc78a0bab8dc73e1fc4" },
                { "et", "1ed500fedf74c8a6a088e72823736091505507ba03da4bcddf9ad50b84efa461734ce762d2ab55a619f97623c799969f55584c289c0e52a9886941bb1d618ef4" },
                { "eu", "d9258ff43cc9a9977ef55a0ee0378b1400400128be8c6f588bbebea089ebd7b7bc375716460c559057a2628062ade8a1c52d2a70dfc9a46c53baf12fa5ef45da" },
                { "fa", "efc4a9cda80b6eefdeb142ebe08c9683e4e12c4fe23e13d69872ab95e90f21449fc396bf04527d32f53cad7fb2d9d8e4a7f2e86b53e084da210a6edb811e4fb3" },
                { "ff", "c45abe3bcd5baa81c77ed9d1c3e6280c5a56227989c08eb38fb20a960526df0fecef44c5d16c2928a22261819a818b320a8a6bd3f49520d5d70eade6c9e2ee88" },
                { "fi", "fac367aa2f422a2640b025ad40f915d24a6b9e4e46ff7bd7db39ade2c277f1d0c2a52e33d0df9210f3fb6a2c34e24551ec0bb5c74bdf208d755821ca8ca84903" },
                { "fr", "ca6359d4cc7f1c9b7c007e85e976867131d59cc53562fa9393531424c49180b16f3a4904c6bfac110c78e7bae24d42128c64cb3a63a5bae00d598a4cb0ef41a9" },
                { "fur", "8fa436cda0b32963af77867ab48fc0ed193c6d6aff69341618e73515a9bcab8305cc31a81d5720e355d72d08f1d6f7df2356d12d7f194255f5af593cb3ffe200" },
                { "fy-NL", "7f58af39c2b555db370c2e40340c5cd9743ecb9e308161a2904bb4fdaa87779860df5a31d50ba8ad50a202aa8ecee6b82379909e7cd99e6302d764b2b2e15ce0" },
                { "ga-IE", "69dd0b85638045d2049acb5d9336dc3c9d48d409fbb82e1d563f9f974bd87f14682fa0a8b77e5f4f5318733da6a52eaba74ae740f3f2b3bbd950193163845abf" },
                { "gd", "2aa51c043dc8ff0cd7cdc4ebd1438bbd8727613225f5e0b4dde4cb93ac78a0913243e3c6a5b572479deb93aa6bf19758975ced799e4b2d6c3428d1a3c8d2823e" },
                { "gl", "05829e165fd9ed8580ae4687b9385235aaddddbdd51f6220ee8e1b4b2362d87cbd801121c8296e94de177f9629528e9b9d157ae6be234b4adbb36f7544ec1b34" },
                { "gn", "9c84ac341be3e89b97399c5cf8abb2502f4f437792d5d939281ba01a37671c375cbcd3d4c5ce28b7b07be11ebed22708fc2ee13ebf32f7c40b406be9047a62c6" },
                { "gu-IN", "6e6346c5ca00d65fccceff30537f2cd58f8820e23f42cf8a27f00750b56a4c4673bf07da2f7f41331c1c9d4a34ae8325c8ef857b26cf65ee06beae7843c1a299" },
                { "he", "e5d2d7ce397a79572aa6c841e74b1b52bf497b3945d7bb3b73ac06a64aedce3a946ffdf3057d10f72bce9cd8a5e646798808bc3d861547e9e8d08eaa53e9b046" },
                { "hi-IN", "d25af2bf74e4ad2ef41f4253aeca09c50948b64f4670096075eb5764517a86c2fa32645ea9ab4b576f2a649a7321cdb4ac082b8423599f5ea434f8dfcc66dd8e" },
                { "hr", "c346d590bbaff1fbcfffc630138154d6fe528363e7d186d3fc5a35c5d6aaf6be19dd7b657a72a80041df53b9a40275de90aad6a71bc7dc67d89a26aeca3881fb" },
                { "hsb", "1831df48332603e77adc9c775693e9704f464efc5bcfa2e2e953f966a13c0423fefcaa941eb5750b8cbe0b9a8b4e89e7b735a715e403f1c034603d6a9edd1980" },
                { "hu", "9ac4ae3b2a25ec71ab657028070dc054cd2871799121fc79950244c43466777d31076721d2531f8093edd6a35aba7c8bb7384d35d3e3d172d48a4eaae27e367c" },
                { "hy-AM", "6a87162f30f4e09b83e3b7a1ec37797750b41499e92a36a53e8d9348578ac00190fa411f9583fde5c3498a1e7367d2e9060092379b58f0e0133e2b33c70948e1" },
                { "ia", "202fdd60839f9b9fcbf7b935d0aea89f17234a8f73a029508b76ab148c2a3ae4884fc08d0b5fcbd2a36572392894d68c39fa60d78f64419b5d653c3718cbd57e" },
                { "id", "c34c9346986bce08c30169206bd0f59ea4e81dab22ce9f262873c3b1b341fc03438d6bc2af62ef65b00eef13a26a80016ef153db860f9d610b8a99d526b13148" },
                { "is", "57caaa480a512b9b5e3535f7881fa6e09994536bfaa21912f1ab4db12e6c1e382a554165836564167101374059500057d09cbffa343870ee60dbbed8effea25f" },
                { "it", "b18202ada317f00a1204c9aa0fbb41f0ac2d9ab3fc611bac6d81df1ea2fd454d7be85ccb411df99c4ef746ce5e78a92332d38e6d54dcf487b424623ad4751c65" },
                { "ja", "f5314857aa66f29624b5365544d74de7107f0e30807340bf239c42952b47ea1281289cf6de293b9d8fae8f74166123a2819fc5903447f192624d9430b4169ee5" },
                { "ka", "ba8ec25ffafde54e61b7d2db0b389a38d03481e1d558b7f226f96f5d2a60b40f1ee79140b8a914040969ddd31c95b460b141b3817215eeee8e1c2f694b3f3222" },
                { "kab", "41201deb0ba12fefa7dda184234cc3a22b304536622d945824e58a51e140138861d4066b934c4a96e2ec79a94d427b897b90759fd57b82e6d5094ca16f4b7903" },
                { "kk", "ffb9969e4658b30fed8ac2a7fdad3a824725842d067b68ee54bae1f2c6d653394868a60034e097fa1d0328e0d9ea26c939958ae7f0d1517b634430667967b078" },
                { "km", "8e16826978badd62d932a3c2c47031a43df7eccdb801845a11f1e3b4851bfa07ce2ce26c9362dd9c40fa56289773442457cf80ebdce17b4acab56746194ea937" },
                { "kn", "f2abe61e51d0bff4336157891dd930e8fe2c99e503e024f71d350baed37cec8f85eacf477f0e13f2cb256c03cddfd84a5362e4ce8a1675e4d4772b517975a180" },
                { "ko", "07c1eb1d7438875a9e8e627831d9b7edd9597b582d0c3f65b14fb081fa9f9de0869e95ad20228b8a495e51794a20d357dd40997d95c4c23e509870a827e9a9ee" },
                { "lij", "6012536c6340e1a5200adc0707a86c8bf18a2686e4eaa22fc7526aa78f9c04a6489bbe51678ae12eb6326d4e6e7c2445f13fdae2104d6ca34dbdc05e837bb284" },
                { "lt", "a859682a8e5b72adfe7e8aac239817745da8c2c45a5b1839a0b0b089a2a86811ff139d22d74538788f6825ccec6af1775cfeee8e3c21e41335d674c9d432b1b0" },
                { "lv", "ad5cfbbc8581afa84961f3a2f0cad2a7dc2f04f35f59a77b1fc038cd5ff0620ec57e837c7dfcad84e94a582e9fd1af7f38eca3838cea107955bede9343b1674a" },
                { "mk", "1799c69daead22859cfa093a8caee5f96a6c55dfb1042b5c8a215f9314e7f804c9a347649381854ed17cdb54f364b40abfb3be7815db90e489ce68d05372132d" },
                { "mr", "3f0e78eb7c2810518f4bd2bbf778f33e6a5d0a343de10961d51df6700a1edcc7d9021a50de05663af4ffcf942290290f718952d587a1ccecee2d1e508214a660" },
                { "ms", "82b5fb41840601b94dc942ff547c90a14ffe86eea61e8aa6a7b809c1b26d6979c718ccb033f4cdb1d6c4119381cdea35e2e88eaacc67bfedcd0694f62be573b5" },
                { "my", "c1d27657656b7ec4396f654964204610e57c6df7f73b7d5c7dd3c4d10a647d089b66e9958d9e9315b5ac59df08bbc2dc58b14b85ceffa6ba77aa779cec6bc7b4" },
                { "nb-NO", "c28c41cbc1c344d5432b147bf561f98d49b1dd3aa10e7b86f8d250eb4eb7197a4f6d632f9214efb7dc1c7a6d45c03d4a6fc3b4de6def0bdfef88f8fbd15627a5" },
                { "ne-NP", "332ed32e4f02453f6d488fa4a8c93219759e534d8532a67ee7fd84ae6446b45b7ce73216fd190b68d512874ddafdff05cf5fa04f11880156dd54921b3f2ab630" },
                { "nl", "2f79cfe00dae1b824e304db94db55487836d2ea5a7925f028be29d040ed032dbfc8c7a59a87dfa9bc955d764affd202c94bd1d32bf2b47af6cdc0ccee6d58e09" },
                { "nn-NO", "996e2729ab63f47cc2c1f352bb2830cc2f473934bf24669cb5989638eab030b90e74bb0a857ad3b054b281d27995f5ca185a80d4611d1cb9ec652807efdb2a1a" },
                { "oc", "60ec59cef329303f3eeb17b6bde18c005d28db966698fbefac4c3d64c56033de111f4a60807b06871439ddb9560963e18c265c88c2e22305600f7947de0fddef" },
                { "pa-IN", "042a03be327d7692f09e577417d2af597c7694276cbf4bcdde9dc77a451e462df034209512f190c09e1060194225bffb7c4e19c4308de39d5835ae9c2ac80e65" },
                { "pl", "b17404362cf15c3e172d111cd5cb153f8d670186f77b372b84f472790d91ca87e39cf2c5bf060ea58a3079dd61074582c6d8de6ab9f8c9be6fc50b718d22bc5f" },
                { "pt-BR", "0dad603b4deef167b9786703f1118430ba0d2f70a492f97429d6fbfa9489f3c4a51f579a0ca6ce215f2f32be7d519aa229bf18906c4c3e60b0e8c51d9bc3f692" },
                { "pt-PT", "5a55ef4888a605e497d71f0ca4d266f3ebf05320ab45d8a2ab6775a9b7ef104a6efbe24e57d6207c4ed57a9d3f4df4c56d46605dbb07e1fe84e5a846c2da2f55" },
                { "rm", "935253821d84b5c578f1802c00b9cf5a1e960829f997bf09160a6663a74f9dee850a22d9e5b17a5d54532aab168d0b60be8e4752c419bd6f5bfbc5e1ebbd2283" },
                { "ro", "20638023ea70204c08f41165b8ef091a8eace52f2e51f54594bc49f43a07268293c46aee763d12714f5cf96b85adf459b1509f07801bac05e1801286c35269b8" },
                { "ru", "446303d3132377bc183c7c9efcad997552633d0103f07ff6cfcacd5c2ce6a1b327b91c56c550f10a92f45a23975446107ef0e8cac4d9cd294c778191448dd710" },
                { "sc", "a2307a86b2c752f37b79e4a61481c9a3238eba47226c9cbb7bb87e25e85d7573dbb39312bff1280841fbd043be396dbda7b0ed28ff17770b055af5f145772d99" },
                { "sco", "4cbf89b06ebb8b1a2458b6b3ac5b8960d00cf87a2c57c9e2a2b419eede599cc3896476ccb9650f2efd0ae0de6fbd214ae984dade5e8229e4dc3a628a72546561" },
                { "si", "8fed459eb3139583e21fa9abf767e6ef905c174ea3e907cbb21f32d9cfc58e6e27def6ca69ab6bc8e436ba47358f2ce7acbfd3c0ee40d8e66a2fb4d34260240e" },
                { "sk", "917f9b82a3b40f8fa7417efd3901ad000dc1f6aac45f82f330056a5005aff0ba66d6c5bc9bfe4f8deed1376129d9d0d14dffa19d408b42ffb8b378b310d9b46b" },
                { "sl", "56234ac4927e4c3ccb656007dabc5710f5fd80af409ac84c31dd890333ac8803f2c94757cfe4261395aa45f1c9e41da6fc4be4715dcd081f658eab6ceb020425" },
                { "son", "718bbefbc011a1defb5e5d1beb143026b3301582b795e013f2e40518c584b3a0c40a2c31222dc45025c4729fcf8b570452b627172f878b0f71dfd77644d5c7b9" },
                { "sq", "32ee9b8bfa2c4e01c4cf5cbb12b0a1c72e7b008a111760591d5d272deb07e1987657b4e203c18b323f7c7c46ebc0a6694a5dc79c4d9c77bf0cd73eb31135c479" },
                { "sr", "8ad376fe3ac67b251ea2fcdac53ef92e785b9ad26c683302bdc168e55d95ceb3cc4dff43a3befebf704df659679120290ac79b890122343a6d39fa6811e7f609" },
                { "sv-SE", "7efd4e9fb593094a9eb278989813c962c19be313b0e556b10cb769b8e6cb0225584300ecbe8bd0eeeff2b4941ee53a69b85a8fa924706221ed8e7fc21ce34ef5" },
                { "szl", "38405cdd52608fd5684c7db6393ad450bcb9926f60b3d34ceef632775e4c6bf3e25126909feee9cb1ed3fea7901f073f7fbbf9222bf0b2568d66351ca0978a3f" },
                { "ta", "bfc4bb508f17191aca099cdd823144904005fae73d7ff33ba3687cee46729955b5f8fdbfa5273aa23b30e20b339aaa05ce26613bfaed16683a581e1841afbd79" },
                { "te", "5a6e25d95a70d33c0e6fd6097ff357ecae3a89a321af6ac29195c287cbc44e4cfe324a410014ac3b39c8354f02d76780d0886050d3729953d58d54f99590c0e6" },
                { "tg", "e6b9f43b9584321668efcaf4ebeff2db291620ea1053b7d3fbd88f35928d4b7496d3a07b5ca0c6b068c2207eaeec8b997f196c428581d8045ef68cf6a9c2fe72" },
                { "th", "6311e177152c5cbcceb502959522128fa44750a1ed5370dd6fda5e867a1ac55e3c4e1943d8aeb21fd1757bfc982d27d7fde6787c1df9286a5636d7cf357e5067" },
                { "tl", "a27c445a74ce6baba70662bc7a6b5eed9eab33a03d0ae0b8501fc6371f26b4de3d8c8eee0f0bce75c46662d0509f24d6ad03b1e5cee314d92b2bbda594c539cf" },
                { "tr", "f005f61d999150f74136881f98970818f7bd466e4f001500e94cfdc2ea878f936010c13e7c361feeb38ce41e83ee75f8000d72f544eb688111386696d100431f" },
                { "trs", "ce72efbbee85d35b23744f5808db61d6d1445d8b5729fbcb95ad9cdf36d326e97a8de71dc885a40ac2b21cd281c9fc94cafd74cf397794aa4e122a8167448357" },
                { "uk", "5307848a7f9d14d0def6ba55f566a536bdab3557756e4096586525ea800639f2352e9f841b12348ccfdb75df6480fac80887dd11dfceefa69574424793bb3d6c" },
                { "ur", "7699de7f14be51a13a2542bcc431c03e9c604525fb64846959d6f206e40c91aeb54d702d4cd1d500f25c4684ef2a581fbe85438ca2b66c4187eb2d67b599f89c" },
                { "uz", "85248b54e21916bc02dc16e81dd6861ca16dbb899a6f41a58a51c68ca7e9db87da0de736e544b52599f1acec30f000e4b0256e903b77cc7e85c5c1b42907166f" },
                { "vi", "21b017344b0208dfa8695d5addce31b66bb4939c6fca720fbd6db9890edb7f0a3407275c68c59773e6958b2fa419f3293122b4d53edd8ecf7021b0692877e74d" },
                { "xh", "608bf4eb8ed25c396a7b859f6f39dc1d89f7e3f1c7f607ffba07810398aee897f3325d0e9a4cd678c21eee02b8bc99b9b6573a2f24551feaa6f785d26d1a23b6" },
                { "zh-CN", "63ace73f4afc6d61c83f1ba055f64dbf748c586c95ed09c9d3fd385d87509a4af47605d55743102a76ceb07e99ef6edceb451abf93dcd10e66dda4f0173b8ba9" },
                { "zh-TW", "296dad43fffacbe5f6d06f775e8c039b74c0fae71a601da6e90a594d62d9706d691d1ffc9fe8766f23193b61ab2ff807e59048ea73d0abc7ed1c47d5aca1a599" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/118.0b5/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "330b9663a0639f92407444090174ac4221aad13f3a5f536688d659469e4e6a4b0be231c1e71c6b178b1b1a6e880114b54ed42c14e506172a80de2be8decdb95e" },
                { "af", "372462cb0dcce29cb7aab2636364506a531d60c4ebdfd535e07b894024c47e74e416b88de6d221f6d0f9bc3955b6671d4a4040edf89ddf63856200c4c5b4f7f9" },
                { "an", "fb81876a55738f78f489fc42753c5f6d3bf1dc7d77aa8e76bd24390c008d4dc43e6f1f7c27a19d5559a70d72d62c7ed44d2262f3e1dc413038f2f2971e08d756" },
                { "ar", "3a42f35948519957bfb523470842e4628566e32f8800420bb5115e9a3384f48a57676d695c8ea4e2b579245a22e001525411ef314c31b6bf0e7840131a6c7075" },
                { "ast", "1ffa8c2f4295a8dfcaf370d1897a446c8e804ab1ce7b71ede4d1ac943a3f4d90b1acb553f85097a126edf2b19f2c84618675eb91987ce90923931aaa270808ec" },
                { "az", "f5672ae83cc1981ed8933d99782fa5f7527d01cd6c1c28a366c0a4c6ef149367e721ed9002f4893d986eb4052e5609502d31015796f609a8ed4455fac90a372f" },
                { "be", "eea9149cf66d4ac84b241fa3c1f0838c6ae8fa5f37eadc04a6750a72504b514f4db260d1dec09aadd95cb5912be6eacb9b437f5554d65887f36aecb19ad2c3b8" },
                { "bg", "e1f1743652e413cfe381557ed4d6535dad718dcd6b0383474523f5c973b5cc4680183e440f45f123cc59673fb0d643aae29caa8d02d53d49717852ca94eba78d" },
                { "bn", "ac44ae99a514671c1e74cfc3d37b95a652cd10b0ce33c43e841ae866fad4b81d41dbff22cc996d4531bcffa926b42b38e9b3ab847b5c3a94ba230cfe85087919" },
                { "br", "a4003791d8cc24a418a692ee7bf4702fa8c6730d9c87c63df531bb09e3e2e915778eadf0eda7eef82f8069ed1b7c51abf82dcc12939e4455ee2a9b288f2f99d1" },
                { "bs", "6530c15856acea4524421a93c979dacb8ffacf4aea19ece1d951c7a5745d6a57ee866a430e4d14f8debfb19713d2ead063a1450543f2ac77f80fe214f018cc6b" },
                { "ca", "9d9e87e3e1e6dcb4d31455c2a9193b2ded6ea44b6709807189196e93ecc6a8a3caea2bb6f60aad60344cfe4fb358248ea8483c13d86eff9a28714a83e3b8409e" },
                { "cak", "574b2c3d6ccc12021efc587df82db56c9b2f0f44b0b17208fd6e749df6be6dfc0b95e88bee6e937f9d0b0e007517af00e783a3f0491dab0478f34688e5f7d7e9" },
                { "cs", "dd48477f066cedaffd609e59a95023948a50bb3126e9a2743f32caf6efca97f70c71166d1fe4014842d8546fdd40888f8ffeabe727d773ca34ce9c098263cbef" },
                { "cy", "6e5ee996c9a7ce737ef6a96fd5ea844fb79caab5b1c8f7a1783876777ed448ef11b4210f9e2898e95a1eef8293bde330fc84f9149871337a7fd09c9392ff9252" },
                { "da", "7794e12d04547e78d3b79c7cb53352613810e99f33be67bab018900339c18546963ec7601f1649e61ca77bea85eaa714963c29509fcae010641bfa7d28e1c392" },
                { "de", "132ed27e328b96cf8a17abfee23835d7733090d506a0f9bdb94223684804a888a8048741aa54dc27a0351e11f4ff0edd3cc66dd2d3a4f71c53e1d3ca720e5229" },
                { "dsb", "10e1b701eeb79269e924da9ad096994134f922ee581a5735da38bf6f7f97e717f6927f85a180b7afdb611ccb26ddcc8aaf4523153a147df354ac2c7ca40704a5" },
                { "el", "15ffff41a89b229d3d2f50ee063e04c0f92fe4a818cde575b2aa904ee6af324032243c56e8f8aebfc5d198f83ba6a5ac674c99b348d7920bc16a432bfbca748b" },
                { "en-CA", "7a1025f79cac52209b7c980fe824393c155f312af6e0f8e37e2d63a1689c07f66222123dd65c795240802046505f03968b6fbce864aa41230984486bd5664f16" },
                { "en-GB", "6ddd5316760408bd7bedfc57a4b5dca7496fa35ff77e87d4d75bfa1e6becfe6245107d2dac9a680a59101776eb635fa666e5ba85a23c5356ebe92542c8499fb3" },
                { "en-US", "c52857c9f274db60c1bfe5ee58e0a89f32d80ba5940fac7592eae94a89b0373b0e49287c591bed22ec20174e55f48355a6404efb43a0feb79d94abf6b64beeb3" },
                { "eo", "dbc4cb7173448801cb3ccb6b04fd652dc289e36b3f979747083e846e1eba382dcb9fa3ab16fbd1e2b6bd10581f5efcda25c98031bbb889b08dd0ff5fbd15803a" },
                { "es-AR", "35ee3c8d6de60cbbaae5dce3cae36b59b3acccc7a5c876df69bbdec56b5522ea7f89cd479a03b62a5220094cb613eb3e7f7b7d6a5073120ca71fe691fd4575d2" },
                { "es-CL", "898f7cf67979fc8d3053de0175a0992c2f33c9a235ea34dfbed58e014ed2c6002426ae56795e61846d5ef41bcf4d86cd3baa9daf6908eb0f970089b2c6953651" },
                { "es-ES", "09299a09dc33c7af795cd1349fdf5e2acdca34ef34a2940fe1529cec7ba09c894c59475f370f05b7790321da80ce33d6407ef74c9eb46f7ce11a805b4ccb710f" },
                { "es-MX", "329003cf9a6c0682a12ffeb92edce578c92fa0bc31675e834db347208515e96655c1b506d0eb5c1a1620f71203a0d1d3fed713bee23f0a192cddb343d9185f4e" },
                { "et", "dba7a566cd1b1d59c0a9201773849b7086250439774a7ec7dfb24c82048534a7aac9927362aadd48667c18745540ec20c30247123adb8ddf9ae1fcc6f8972647" },
                { "eu", "2fd7b0327d9f88554e4c03c3c16788a910d8a3501a996d8c70b5261439d3cb5799d55a9358d2f38f15c36278041e353d4e3ba3339ac777c19d781b05a0abfce0" },
                { "fa", "5295c6c38cbcb7e89b75b5c6119ad89d6dd37fef7b577880a4bc3f1fe8e1f72e5659be3b592cf8f19b646d948baf1bd9569e56e4b172c07b1f2090f176caeeb6" },
                { "ff", "0bd14087ea64a52892f1740962bfa0f5814c36aed2f8138d21bbafa5056997c1e8310ab4d3867d02572f8a27cb5302c7e9b7b29791327a0a6b1d8710b4fa3f0e" },
                { "fi", "aa7c82a05eba3aa6a87e73c93cfb78563bce02003bcccc55c763f7d10f819939b9baea06d267262dd546312922275e773fa652be6e1dda1bcc995af4a89f5ca1" },
                { "fr", "dfba629f2df14a436fd2ffd91e892927706c0db5768a481e8dca745874adb398bc3c6f3f9ce4d705c27f0ae3c22750ed0ca2be74183d360d8d3434eb41c9bf73" },
                { "fur", "8f01af8c791f7d59d005e7a61e8e192c31b696d974d0ee0e12dd7772e5c6aab4cde8ad9f54c3e535b7bfba1e37164677b094e7f86191fb175537d20ff9817c74" },
                { "fy-NL", "a43c114671494f245a94fd095afb033839b55d37cdc8e550729a10e3d64e830cc3ff0aa9f922d95b5dd7b965548b77150cb50b4663206b3c6a9f20632be2cb11" },
                { "ga-IE", "ab12841d7f5ac52412eccc74b4725679eb469d3b4b772a3b14e2772114c86d8f3552d68dfc4b0f1d3fe6fede2189955716a5c08770f71ff9d153e7e352b54241" },
                { "gd", "236510f3ccd3cdf1293e429da2d228401ebcff55b548a1cd6542fa0becc61aafe7b712d8e52a5c39e83259df24fb3e7247d41c67604e9b673bac10e2f44ed39e" },
                { "gl", "947eca089a00c178bfd92aad06bd96c0fc2eccc05eb87e7f1ab7d684e7f5f9f216277c5b823e8634897182412afbdff00a7600c87bab240754369eea63ee7123" },
                { "gn", "1ca77881b9370a29337b8ab3bfcd618c7af4a9eaf430a1b2ac68ffe5134d53e044876cffd8a957da9952828c53e197c2be0b838eb4a03c2e9ea0b2062426cca4" },
                { "gu-IN", "6fb596acc9245f47007044571d3ad64c53416dbb6b2606d0e403a6e2fcea41ab7dc4dfa685164b1ca3ac329d77ab71a0929e06e57ed498b577f63d7186ee509e" },
                { "he", "89d8f970b1920ee698e27278da55d032f3d0698f6f8c5133bc778af082e9ccc860396a6e84b8fe847d7598873acb7139c42c5aceca31b42dc393ecbc9746a3bc" },
                { "hi-IN", "e07658abd9b72b3338bf39b85de476c0fe6b7f8ce4422be93b74e8ca864d0e8d7271dee5ec87c397b2206a37dfbabd4e3c9b14d0ead3476f437e420b121b43cc" },
                { "hr", "d379537ffe613adc9f8f5f797dba20ac2cafa6934bdfd3771e8cadb4dace809782e358ffcab0191ab5b985117f413ad0c050c7e461596c65e66635a53146928c" },
                { "hsb", "3b9410eff48f607255e9c0096b3d7c23418068abbb1c88b54cdac6ca3d3d42a3222a31b4e998c6511542710487840b7d25b3e65076f20a32c0a3044e6f25e280" },
                { "hu", "17b9ad9c1b123b25a3c8d222cec7d57f62e7c8ef245bf50c65184d7f04304bc95dca19cec67e810b237a6107eb5e9afca047d32074104d686d18c1d8791c9e9f" },
                { "hy-AM", "7e938c044b4810c5ca46516eeb33787ca8b06b04adbdd9b217a313d1660a057c10b021462877f80813b8ff9faf4d76de526f0be6a2980cc216364e8eae78f369" },
                { "ia", "4deb5c2fdaef1699ae138d1fa73d3045d408a2baa4e7a7585313616b44e94543464c9308cb38a43bd6e9a28315ee39704872a57de633801c3527908679a60a80" },
                { "id", "fa5b61354c136e5b75006511a20e25a417dc72ddc9aacb51b8e262b17403bcb2de79ec96c0aa5b3d41b3f5446dd7759f79fc24c6385a5854eaf17a192a9e9dfa" },
                { "is", "2711a9273005fbe3f1f6548250bd4c8921b34d692f58e7786c8caddc61806d69ce77d0ba8b546e4144002cfeef5758f7f2dacfd95513813d54ce8830a5044964" },
                { "it", "171a4416d21f8fa8ee8f72b353edd7669e6e4ee271238341154c25039cc86e9c350347b5439648c7690fb11b7d4c5238f8a66e6c6c7ac96c0b40c9749dedf5f8" },
                { "ja", "2dde88e91dc92193b325411b08a7e26bcf8717acf6a5f04e5bef1dfac9e7ec7027578eb4c715c72f6cbf5b682572c22f5d463521b4cfe262384691e92d35e7fd" },
                { "ka", "1968a878562b100fd4d8d6d9301ee7e430b5ddf25fcb6cd46b644c905d261f4aa3e24d055bb6bb68e3d601cd2a302f141b37822f9c242fb920ff36dd0919f9f9" },
                { "kab", "30c95f73997a285d55305f53bf36f43c7e4f1755664e67b33fd5589d9377cb451467d28661af84ae6830bb4920e8f401575c01a7c7f7d615f8dbbc4c84595b25" },
                { "kk", "5c697b67164fabe86314812532bfb6eb258623538844834ee053b7062f0adb5d1e3df756d49ebdc2175d5feda215ee4a2bb1e28601b93b1cedf4d701355b03f2" },
                { "km", "bccc854c9147e258da436d3cf157bc421d6ec50b3679567d7019df7b3446afee0cc2c454e0adc1b6f94bad771d3690dcc1c2fffc7d47744924b2562427099565" },
                { "kn", "3f31b32a976288083cf7df9becf1db53ff45954680000222b0cfeb15b1505450742b6824705913db9db483e2b89f35082f72a51d47aceaa579fdff2259217e23" },
                { "ko", "e97d73f84e7ed0c88538f178e0e60df98488bdf894f43595ce5e30f082f216e9d51689c5e6b2c1971c23334ebf7a0ad15077ec4e0c00323510e33fef791c2062" },
                { "lij", "0c136faa92d2468ac38d433f3bd1cdfb5e16531573a7e7ec24daa5cd6058b108dbb7ac6b3b47d8d322d623c50e3724ffc0e0e3cb0f2b18c112117600f320c257" },
                { "lt", "1abfdcabf0a85147fd7a7fe61c96717abb0c39aef4bcaf6dae2dfce9dab755d66a403534c54243f61ce8ef6d90f6e9b617b01f7cd247f53ebee585f565875e7f" },
                { "lv", "6874dc48943ab8b21d932c2bce164fc396a6a198585badea323e07d6d485ca693e5dbc9b6e195f420e85a075599b1d848d49b38805f7bbad8a300db89ce2f682" },
                { "mk", "3b0f57966a3a8441e966481ff39e0d1d2c6817eed3f7baef9f952dd93872ae1caf6e3d2c8053fa86c7e890489dd9694e7aed28cc56206fcdeaf9c30b6b1d8215" },
                { "mr", "aa67e9b2c0e7d1b1f36c8c7bbb1325ff3f2dcba6f8b91160abc0a468207e21f945ffddc19b7fb4197c27267308bf23e836c3dc1fb99fe70df691492e50a0f060" },
                { "ms", "bb9e0f53a60b65256ab8246c9684741601dc91163475ef9c67b4892c14b10da87e1409ec0936c4d9b89eacb5a91bf30f6e7520f99fb48caad2ace4d177580858" },
                { "my", "862aec467a5937bc7050cd4e8c8baf163b2023c714a0807dfa408e7ea895171896f5aeebfc98bd7911111ce43e729ae53919c8398e4e7ffbbfb63bdc16036769" },
                { "nb-NO", "7230c49e2aed31c51e0a52e38c7db018d68d9594e63b08163981ebe0f704641499cf2b79e7ffc42d32aa4f7b355fb6038057be4ee8092164e8b2d1c8ee5667a5" },
                { "ne-NP", "2944973954efd29945f942bea09b7c0b4608fffc66cda63b067e14179bcdd7fb654eb1bfa2fe04181a4f9505eaa63f8c7f7e1ced0b90166f9477438bfaba9a3e" },
                { "nl", "b107147050b8a0c9e02cc1d5a12d87dd45f5fb09e8e2bc42e3934dc82e9ed2ef651f6229d1737deea815c10ee4315375294cbecfdb1004485cb7724c6b5890aa" },
                { "nn-NO", "9589b23c23bca6b486c635e3f0ca04819eb009f0e110a2fc3d2bffc3360e104391b5284530e5b4bdfd19d52d70daf98ce033e89f4d2dc1ab685592c5ad10cb26" },
                { "oc", "8c6081360900b12e73dc8213e6bae2b47a38ac5d305cbb6f68fdb5a36d04ab6e92a1ab5f397a3c5e1a9def8778fde4b2c7798f81d7ff167dd48dcabd6b819a99" },
                { "pa-IN", "faf9baa1dd296be56e1ef110d0ba1ae7031b8a678f07b1917356364b11169ce8652e7995af7428a7927de81e5329827dadb890c6c97229ce5bd926b83888b552" },
                { "pl", "78a6473b934a367025fbbb7ea6a9432ec03acc9493e679ea2f21cbffec384de317f93f2cce1f17b1763cfe5f63d93f537bbe17dea6fa8d91a62d015cce7be9b3" },
                { "pt-BR", "27aa89446876aa6adb054d1a04a5ab1ad789ebc3902c7f097e4bc36d62825cfa8df2657df13f8d740e502ee3af766aca42804282f3159b932bf8f62d127f36b4" },
                { "pt-PT", "f4889ed0d99972c290294b796cc0843e48559fb9341ad8ef5945e67e70d1de20c0ec6ec61629f12bb06805386c54b3e834a5c48cf8aeb4b6a14ca108ca880d11" },
                { "rm", "b6bc5d46b7279ba48fda39d4dd3a7dd076feb7e40d0bc87710cb3309763e077fa35dc262e741ad589e74d6cf66307c37d22c77b851239acb103ae1c2de7690bf" },
                { "ro", "852790c9b75f7b85d0e28902ec738c7dca8c3a419244a7aa26bb6a061962adcacf2a81d71902f1b7bc3b16b28b2bc282a21a0bd5dac2ca38f430307093579230" },
                { "ru", "05fde39202fee86a452d7c11b2a3ab4a78b8300e53279e2e2c98a0a632bc8c8c58c877c152092a31e6d7a8b551fcafc3c5cc70790fe1d82308cdab9442dada41" },
                { "sc", "7ba515c2ab5154bdd803f5fdab0218578edfd70fc89531f799a977265acc5654bc2e2430fb1bac78bee3778efa51076a45aa27dd8343ac7c5edb8a6530d9e10e" },
                { "sco", "014df604edf08513db135c0602179327d63b37fdfc82c68e437a095e3ce03763854a40a6cbc3c284a0a6bba8849dfc747f77f3bd57e6c51c2530764e68c1be84" },
                { "si", "ecb37f6727d197ad3b054cd8a1ab3100a1877834451c5e8f13f289f190d2856f545373961b2bf3ceb0ac20a1cc98f2c7cf14270c86a9ba4f8cfabd399997b593" },
                { "sk", "e7134780e570a57648319f6ad41cd3a6adddd3e3b97bd10bdf952915e200f7f16648a78d1395d692ebcd5d3eaf864498e22ed9a89100fc1d7eef57c2c7ca72e9" },
                { "sl", "493d819c4932db84e4df7e2f65d8772f0d5448e750d38ccad10168bc6efa7d2361ac6a155e7c7ba172b367a61045d39f16a2f54144c5ddb8f59de5962ea2544f" },
                { "son", "f78b40e6822168a47a4ee2f64caf02b254f52748753129fb8f8889a35073e6183a81618b6c97177135a3fbf72a57ad2f3193c249469ea908ee852b94f038afcb" },
                { "sq", "c63639f182beb53779491102ed01cf1f1245477644e0373390ea53944f5496b7a26b618cd87bbaab8e8f28a704b8880e9cc140aea010287c80370a28ca2b2b8b" },
                { "sr", "1fad4ad27dacf5bdf09d0023cd3eed58019a8c37c035a64fd7628faff2d54704a8fb0bf65ead7d0f814c95cba1253d1bab5c12be3e9e1af7988a229e39b9c052" },
                { "sv-SE", "bc097e599b638b29386aa533252850410d04d4b774293bd2580928d44b9dab8142ca8b0d5dbda1a56ce05512e70db112cefd07f71db00541f56f41a8ae6be794" },
                { "szl", "ec376d762e015fe179c05e3a1ffba3cda4b1c99f582b53c2f2c932b1cc59c0cf5076f6b938bec3d1d6fdee8feecf78e45b3d303ad7ee21e48a137db44e0433da" },
                { "ta", "6c3bb5296da3ad41203f5ac77550b7eb29e2d27398306b5eb73ea61322c524805bc3cb9becad03ea6aac122d92f81e6c6931975e405ea75b32afc0a8368b62bf" },
                { "te", "ea500d608e9fdd21c4bd33abbca521e90f15a2a04ab69a9c63f86d6cd6bb4cd1556d7db5671dfdf84c5917d544d8c094b170fbaf811b689d085d576980b9e368" },
                { "tg", "9895204f9b99697726e006b354d3869ba76ef0becce961ed80fa06b37252ef4d5aa57d6fe5e157e63d26eb20df9bbbd91ec2d89a9cd3504fa38636ec15067fe9" },
                { "th", "43259be931c969265ce770b9a559ebc138c62e429f42cd120cc86f0b7fcc17ddcbac35e27c942ca34bbdea055d5896bff9bdc25257866bf9d17e0fbcb9f3005b" },
                { "tl", "ddfd905618d368fc5a5c47090e554894231a1c5c6feb72159b2828a32b68a2865e2dd2435e254a584f40576d37dd561b84a88005aee18d6f140f327c4206182d" },
                { "tr", "f3dfaf3ebb3a2d24be8acf29f215157e71d83fcc3cd27c565fef272c0b7372c24d4c45a6224e78e2917a8172669b3d82c9efe6dfd0d8c60a9422d44fcf4e0a31" },
                { "trs", "4903f27a04d273385a7c07bedcf59948c80150cb0c7e893884e9e79a89292a79945f606873ed18f719734e35c97bd1a42e7cfb26db9d146c3e99a4eb94ba5ff5" },
                { "uk", "c00432f56ed36e53ed6ea1c95f01770cc3bade01bab884a1c5de1f388455ebd6b74591d5a9468836d5735abe7bd69c796d4ac3ad5fab7ec9d613a31dcee47c01" },
                { "ur", "e35bccf00082b08af8101ea8e003477c32a192788b669ac24e32d15f064792772b2ca455beb98da93f53a601212dc04587429e2b43a6d537db7eddd23c939b21" },
                { "uz", "ed036931f3865273d6768f3860852d17803a1fff98d82e03f0c48907bea602ff9b4d2099140bfab86999943e7c94ccbcb939323bd6038ce12b0818255eb7f714" },
                { "vi", "f8b1e8ee79db2e9096af3f8511f50e39a7242b18ef5aa811b1009c3547efc30874a0506e82774d92a81c42b72536dd3e835ca1745838a21bc470e26472562cdc" },
                { "xh", "b3ff9a4fee869ca261df797e0a703f586c7147ff4f36d8a0465c00f629d99ae88960186f0730bd80dbd23ef850095684ebdb377ed17a9172fdd278c906473ab0" },
                { "zh-CN", "73d6b200461f0e81a3dfc7185cd58c85f91950fa925580381f99b33419dbccf55e488dd2e5d0bced9b01054b001ffbf611228397c75daf7f3c17cf457f854fbd" },
                { "zh-TW", "18dfb5990ce036ed578c1668e4e357962d62ca2df7da3bd5eda5fad54114bf52a08d7b449d757a0bfb7c75034fceea4e676208c910348bcc2b6fbc6bcec774df" }
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
