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
        private const string currentVersion = "115.0b1";

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
            // https://ftp.mozilla.org/pub/devedition/releases/115.0b1/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "b41532593c141f2a16939ed6be2dcbb0f6c3568ff95a3449d6b62358ba1c49e3fd6e92fd27d741898f5a47e91d3900d40e9635de847c376dc2d965ae8eabe624" },
                { "af", "75566c96321d05446872a65a37dc473bf22a814a89ac01015c154913625833afa8111c9bffc0cd4284ec676010f3cbc4e2a2bc8a251122fa1dc20fe8313144ae" },
                { "an", "64fe967a0c851f989b39c2be5711723126020a8f7d780a095a06e39cb291146d78b9b9e6c9f8e0d872cdf73a3eec601f13dc2655b46dd35a3ab19c510daf167b" },
                { "ar", "787bca082a6083bbd79ffd939f1c2f4535ac8d45c099c9f91e1dfa7f08f24a4a1c451b1331b9f9c0fbbcf7609b239e74afea7eb8195bb8f0c6fad99c0782d7e2" },
                { "ast", "66ffca3f1c26809479328050d52a7107273fb26ee6867d275373f7a81e18c106c0cb305bccb1f48ee741e702d184e4230256278232bcd21947ebb2d4594c8a0e" },
                { "az", "02797ffeb1578270299c246bb03bf182bcb9ba8d6a34e7f6bf32e4c766dc513fd83d3f5d37a9421d19dbf0fbbb338c2f73fae6bb2f44076d542389f46553c8db" },
                { "be", "2bb769f161feba25764a0ad788a42947fdd20f89ad8c9cd110bde12b0ac5980d1372010403e7225099344384011442b4b9629804ff3562d3b492f2a859a48b34" },
                { "bg", "c55857958a6749ffea1fb19bd53043e53957f12f33abc84b85990b6ee96a711b7af29a8acf673af4eec074bb82ffbf824308cc95d258484b1a57a32f477090c7" },
                { "bn", "d06539628d1e2d15ecca4a45ad43f62daa06d5fa22b9eda3b2a7b4c4b56ed3db2b47640ae0c283096300d08ce133513cd7df981cc8ea23fc56b184ea2f14b96d" },
                { "br", "a2cb287613c90aef72508c387f088082c7cba7e5f9f2212222f76f1efaa56dcad9da4a2ae129eb99262114372bfab4d14db10ddfb29272e81afb884ab3de18e8" },
                { "bs", "2e9533f6bc943648c281d7e4b5478e3fcdd55aeb22a35024af2f14c8b9f587fee3c720abd4f90f4cf5ae4a26a926c344a5d6240565db27da4969fdb3871c9508" },
                { "ca", "485401e82129d926846205a40a41e0e6932c0f6cd15117757c6f3324ed7f6c66eecf87bd346f3b54d89234ab65c0442594ec4863c7c2da2a0caf904b143340c8" },
                { "cak", "d5bd809a06b74a1a50b5de3382acd327b0a63d145c5c2f05732387abb6cb15fdd8074def5d28e4b131b174cfc18a68544bafd2c922321baed2259d2e35c0c92e" },
                { "cs", "76c0bb2fd5fe855ac530adc6fab638c79bd00a98b450e61d21af5b06b5e4d0aca282b81e252ccc093ee1659ba310a89a06772035586129892af34946cac28953" },
                { "cy", "55500860a30cd4d72886be07e707bd4995a38d7f5608c9a03551c0529d6ad9c596ec8ab10fcd0a9ca9c0b71b5d08f88677ffd07fb0d64cd78746cd34d89ccb69" },
                { "da", "ff722d682f5322406fc0a51ddcf340f2fe471578fa1b3be875539b4286c3ce002db67fa546e8e45693dbe0c2b49ce6f384e4c3c99720c82f0031ccba25441ad0" },
                { "de", "d94b5edc7d3bf7d1f328b5dff9a0bdb0fec7270374d835bef4c73110ab15b3b8f099450434ad59ccbd5c89f62b56dbf53c1c96d43ad0a9fbcc80f35466ebdbd2" },
                { "dsb", "2d613308734f9e69465cbe4af0a76c0e356a64184f9dcef5bae08aab35c4e23f91c6ec82129599aa2716b3eece2c92059456f73012dd4673b41f032e0a3c164c" },
                { "el", "9eb3cfde22384f3981c69ba21c2b7e8bda11bcfcc888160b16918bc7ca1a2041cecbf93e3893392dc0bcc186bb8e8041489b18b09eadef4b2d76b7aae818e23f" },
                { "en-CA", "ac349dadba9546e1bbec9734017fe0d34ce7dabd13c57c98798a9039a0199211a0335bd7935d350fe5f6e0233c2910b9df4d033fff27ab6eef49030f3ac6bf36" },
                { "en-GB", "e0383ebcea36184b1a70db895db170fde956d7e351aae3c1d62c91d292c5defb204d319a7cacc8a9a54f2cb164f0100250776e0fdaae2b43f2360a2bd99fee77" },
                { "en-US", "cc9138b2fcc9d76b3ba8b369be0e819a2eb1c5ad3224521a6ab08f671379e5c0897f628b5e5cc5729978e73d62cac47a32cd5f248ac711a176c20381960a0c90" },
                { "eo", "c130b132af1f1ca0757e22d47a50d114f81f2f21f0e4e088ec37ba0ef38450beb94718135afef3dfd145845b57609475831b49150129105a4533c04b508e09e9" },
                { "es-AR", "aa6b3efd140c3be021db1f0b541c879e9a6af0be1563078b5fc2052051cdbc3e5df9c6caefb8a28966b48d81731913e1bdb14bac8d8d5ce6e8c04f35a20019d2" },
                { "es-CL", "e0563f711fc3a2ed92517b1e1b3d996015af50ca3eba7d24237e0ab52b05893767c1921c6b1a9fca771807b4dc9bc28d58bbeab3a408bfc922d60a4ba3c9e1dc" },
                { "es-ES", "3d458756b79c4b5025e3bced0bc39862203015ef48464c23f2c93f7843e3da9cb2d17913b0d5fb0057e532dc3ed339e4c4352510e92ad878e5382f831aa44175" },
                { "es-MX", "24fe76e4e848c70f949b1e858f880a474dffc5dd03750ec3eb2fc3953c6d9ea42b018dc6cc5243d3f12f8b2296df8d1aec5a07247c23231e45153062401ed952" },
                { "et", "9310eec71b47cfa1476ba8d56a9b0f69368e8419ba8f7bba7b545466f51de04d73fc9bec74fe043686d78688fb7ef567d82c0de4b461d8ba37666893a73ed36a" },
                { "eu", "c1904d01b25360092c4577e3d168ad7c89a5fdf54b2474ac9278f9c91155bffcc499f589328c08cf360a78f9beaa908a6771a4e44d1f272592457c717655c7e5" },
                { "fa", "cad9001642878830a553c3e8d4946f4ffb423159d6cc93886bf30869742c2fde5aaf5f61c603fd35084fffb295a0d03d1279a0802b9f511fa724b18e8c5dc7a2" },
                { "ff", "aefadb7e417ec47606cb5345a3666f5181544a788e7e196125693c1da5c3d3a16436c15c6ae4675b2e809eed61b6f51c5baa6df86d33218f361f0acbb0e79b9f" },
                { "fi", "0b283ec52ebbdc403c4fcff8549ceee5611b9e2dc749fd1a95f212416ddaf30368a693b99e531c5fa1b579be89018dbc99c15b3b1defe4e92cd61644741f0f91" },
                { "fr", "d36092ff8120eba5ea1e62cde5de05588178eb4dd3f3ab9bd0568a9ca9bb9df5a00c3e3a24a37998913039a8e546c4b6d81ff7569e7c343b978ac1d24b1961de" },
                { "fur", "5e18a23b2b41b4b129ab74f3a4275ff3ee2a62f21929aaa0cc86840c2cc44ebc83a8f10286a9b6422d85cb9cfa322cd31eb1e340d897647bc2aeaa16b564ca92" },
                { "fy-NL", "9c34febd109fa80620b15c8a8ad14d95d978f1637197fa70599f256109499909a924da42fe1abdf9f95f846e00ec225589b6ae35469280ddc9273382ea10008c" },
                { "ga-IE", "5a6137e8dc657578edf54587f7f3872e37832e592728ec10b01d749942dfabed667e5dec28c2dd8ae72f95f1bd7ce32b1bfe6d9f719ae8c75d7c214baa437b56" },
                { "gd", "ec5cdc8033ca0ce91ed54936a0cd9434eac71537469ff1259b3d62bee96a899240b393d2e3c2ef7dd2a9836ffeb7ff846b152a7e99faf0f6e83da069a2b9ac84" },
                { "gl", "c5a8a274ceffaf8a63c9d79e8870a74273d22d9ed7ff346a3cc9c2ccf048e5b6f5a5aee09d8c7732288a11b1414fadb79f3086bc2dda1ab52e2e08009abf710c" },
                { "gn", "558bd7c45547331d6bb5b4a0d31ec4d7f91b1fb0204bbfddb32834bc7c9be4f17f67e6735ead107d79b0deb8be46e13d9aacf8778f434f2f166cf7a4bd064d2f" },
                { "gu-IN", "ae91bc3b3c4ebe8f2fc19667e6e10c06e956bb08da0342f3566c882ffb687a253d92f7453b6a6bbf7c8832e772d7326d94bcbfe3f34383b1c29f45cec52f8027" },
                { "he", "353a6e95c6f286b0628d696aa738ba197598c68aab7879b18c46a1523fe2cfca0e80eb509b25f793e8ad10401f76e9f9053b3722f3de239767136355578d2d93" },
                { "hi-IN", "9255cb85c3c47fb0bfa8f7d8ee27ede2978e2de639b489cedcde6c15b8451605edaadd2de8c757ab4ce54bdff9acd091e604b8ee3ee85841c0578b81af751a3d" },
                { "hr", "0980a014700c1cebd078984363c765f0386897262374660e2907b9d5d58eba207ed12cf001925a3a32fbdf1ee2f205a52ae9d757afa4b8f5b81f5f4d77c81172" },
                { "hsb", "663e205814c59beefb3ebb94b8ed160145fc4a2fc715350e0264a87d327c61e3231d3a1142c26bdc18245b2dd066fc1254db869654f1faa790c06f0f3ba7d6ab" },
                { "hu", "41484e2261abb11bcd90845e66dc77d75c3ebcf413501e33b7b2bd7e132f0528c28aeee394886141442bfaf775ba2591323b5e47996aed2ad1974a9b434de60c" },
                { "hy-AM", "510fd8514e42c1d1651e18b4c78ae0ba68715b6565e9adfe0c03dedb126909fffdacf4c414ca51c1fcc14b22b2a5aa37c65f304543e18f4b39d8f0c18d9f4043" },
                { "ia", "72778ae3514a8a7fbe6faa451cbab0486a887bf6d1fb0d3ef074e94a7966028aef83910784f9099120fad8662a9205d495ccd1975ae4634c3f257748a31ac742" },
                { "id", "84c99a7cd32666e8ca9412f38ae024f7acb57c5f07b17df2c97b90202efc5889a6a02e0f99c586098b10090cf67e55f58191c8c8ff5734b440ba198bdb09d8bc" },
                { "is", "8550efa9b54518fb119229b4021eae42fb520b246d4ec47eb58787c20fd3a0424f7f6f9a9d8191ef639c7501c255e08d248751c3cc2b0e4b5790ec53d7d390c7" },
                { "it", "02167a8319a1c64f24a0e1c158bfd76663f2a595c1378ffea533e9f04f190db629b88fda5dc5b2f1c474b3f165ae21d96944fe5312392f1fe1ee7e3124a12bd0" },
                { "ja", "1dc72c03627ad87b8c92106be998c12185aa32564de16f965dd86a3bdebe1c0375ab382c04344964f97ebbe50069d0caa99fa6c8250768fabcdaafe828c64aff" },
                { "ka", "00d87050ea4af2f1380fb9e931bb70458c9014b80de7a05ae5f781d4c821db9847d54f057158da5ae64bf272b8c3987fc40a93a3cf934b8a04805fd74b6fcf7f" },
                { "kab", "5eeb8ac095cd55fb6af9a83972ff70f934ebe016a3d97c5bed8d813fbf155c24f4e8568ae8b11f11b50c255ce93d8a3307066d0181acdfe4e07e8f72d0349f0e" },
                { "kk", "74b867979d6af8ee5048b589b62b9b54fb20296b89648d0153f85f6fa80bc29c40d401337aeb97629c86c9f9e474364105fa7fffd4e22c11b8756e7ff89f245c" },
                { "km", "b11413d0465cf6586cdd5e23959c6975cb39eb328df5df095cd8eab6bcfff898244b7085367ffd3bfabebf892470ad7fc8103322b7ef981a63acabbb31edfb8c" },
                { "kn", "9028596b75bf8a8f2cb2ff6b59c38db3ef24157dd4c8dc6ebfbe0d4d8df4641238eec8aadb39abccce323cf8166d3f92ffc8a6e0a8348d7813c57375ba1111e5" },
                { "ko", "eae32ce0552d8d8caab3fd5010486a6977d638ac4ada954540489ca4d4c35a99184e62a787a45177441b1743bd30d22f84e5fb4532ec40cdd250983e67b5e5c1" },
                { "lij", "f382ed17a69da7a1438b53bde5ba9192352d8af3315596e32124c24e5b992c6f4dadd14f685cfdbe5ac59986294db8b8674a0d023535e97db73d065d1ae7de7e" },
                { "lt", "61a5709d13d4d2d2d76ea7ac75daad42d42c86cd116f6b872dd7d2c90d1446952306b951bdd6d3bec187bfb052caf8378116a9c77c3189ba92404f4197504faf" },
                { "lv", "eb227f169963c002ba52c5ebeb76e1b0a3d92d38903ca7706436daa19a663466019bf4db513db416fd2bdf19956ac034446e453f88c487039bbd8700fa93f7d2" },
                { "mk", "e936682a207f67cd3bd6399c96d4951758200c1cb71a585c03174aa43f5c1898dbcef47592cca62204bba95ed137ea6632e73bb5d98076ca133d704d20e5b104" },
                { "mr", "fde8423ca9ca75f28085e1b3e71aeca62fbeef13a36d146d665888a39d1b4ab289f39e8c65dc496c68d3c0e3515c000dc77fc23214a50b698b41a7387cc625f1" },
                { "ms", "913fe88410574df50136282f510e42cc8a084407c8ea565041f77263ee786bf033212085592a8f93af3d4d6e901423bc90370319fb23b45385cbddc250392adf" },
                { "my", "2ac115d389b4baa269836e532e2d14f830c119b09b367960856847e2cd4f4adca86493fef21d84cf297372572fc546650d81c1bc78811b39ab0c8e80cc57b114" },
                { "nb-NO", "59085d1ebd0e78373534e7bc3d431374770ac7e1f1752df6661a8cb5d50d610a25f40a5d042e484ef8bc639f9e507abdc990b2c9885a5279f241fde67989d8de" },
                { "ne-NP", "a74a11de8fec85e26e1b4db48d478240289226353ffddd01cebad99c756b3c73996ae93ff85b18e5fe9f6172f6b06970b0f4325a4d631e903363f0cd32a0d316" },
                { "nl", "6d7e81120cb2646fbef0074fdca743576f37b588abbfcfa50dc6ade562bed487c54d8e3a688926fc48b1a7945b2493122164177090439df3475d959d643be856" },
                { "nn-NO", "ae199f55247df117c7c78ea950a841f8fd8e9a41de5645af9cadc1402b90197331f12d0dc5106b485ba9c6a56195f3f1d4dc95909d2349a19c78c2615a0a4d43" },
                { "oc", "09cf7f7dfed678e78493f7ee02b4762e66a90ae251b81b485f8555c83d9edb2d5b8e4dcdfeae6d5ec9fb75db43db4f6ab3d73c651a25da7a42224fe470be48e5" },
                { "pa-IN", "20839427fc89513623ab01c1c61a3b4267930f4e121fbed246c9545b7445ecebcec7a166fba5fdbbe7113bc7628d91de5cc882c7c31869942ad66334660fe65e" },
                { "pl", "1e6708ce38116370696ac39f7283fdebdaccb22876cfb6c5e36598fc9cf6ecaff3909f480a22fa4eee9f55edf0b91d1adff01d17cf5d48cfc8e1bb667c7cf2c4" },
                { "pt-BR", "b8b364684c7351f72657337c52d9b3992a94e3b47a0e7293843e23ccb3429f3fd98ddfd4612a7df5caf7c8e24234b6a4c2b6818449467e7357dce14f754e120f" },
                { "pt-PT", "3454f544e71c568031f5dfc9edf4b06a3402c700aad6ee451782fc9c2ca659d85c303d5727555a4f33672733c5f5cce6c96b0d23e1c7893c2dac0d78f8925ebb" },
                { "rm", "b6d8bc762b8ced844fda1d67e9d51364455f84576f753bf9961c4778fb31c68774286cdce980c4c262f477606a154906aec8be3bb0dd5558737e25a21bbc2771" },
                { "ro", "f0d02a26faf6dc0303acd4eb4db0e80fbb33e05a4a213148145fec6d57771b91fd8e0c9a51492aa2561d186f6ae0a9aabd34b73a767f133d4ecbbddab6d5fb8f" },
                { "ru", "9e17b08537be62171b48fb8d39c8d3883d18630cf4295cbb1d40cfff991a823df914f391267846dc137e400c00e16f8b84a43ef63afb47113cd6359e082161f0" },
                { "sc", "e943c549298b055fb0e1efa8f86c3ded5bf955b7c93fd772505518e606883702c58bd86ae270d9409e1bd9508591547bb9b737038237651aa2c45586f5a9124e" },
                { "sco", "9dbfa9fb2cdf0350684c066f20b802e2a5412ca0f86b84948217d9f7113dc192f547347e2ac4d2d3b1f21d8399b2c9ad167634ff4ffaecf9ee865b96e62ad98b" },
                { "si", "5152465142d46c38e0d343683efb1d9601d6f39b103b52646ce25fd18c38d09f83447b1035d2f930f0374f89ed6b5389e9e3055f080d9a5f2b1b4caa64d25ed1" },
                { "sk", "5c11a8cb8ce39c9322c8de031394465925ddb67c3aceb7e467ccd67b0f82c6b598bfc6509725188442b1f6827169189c8205ba53b45c8fe127509e265be62169" },
                { "sl", "dd44c416d956dd0360ebab0ab14c4bd44a8503ca7876848a7285fee960a5280d17fdbc50dfa4933fb147bd00ff9378fc3a1abf22a3d75f5c5d7b54d8a08a5317" },
                { "son", "247af544332ec3e14c559ab4cceac4467760294ae4aff362cb079f6049a82908ad1d05e652425558189a0589fd2b6b9cc0007151733a2ffd937a0c5d3726b1b1" },
                { "sq", "4d6e64abe2495c1a0340d162b12dd13d00ca859d5eef44378521fcb29935dcb90702ce0e7b85329eb972839c9661aa68a9ca9bf1bd5528d2cd3a056722e8c82b" },
                { "sr", "1340e28192843d54e52db8564b0da52e5613a9c14d56a89474127decbe926ed5a43747725e5bf926480589570eb2ef106d14a8317b761b2217e9deda1ad54b33" },
                { "sv-SE", "e08164e6746a9cb6fa6d94c61065a77e1ed24a9b9eaa72ee2bdd3a11e10bd6ab207e7f133d2243d801a60534508c2c34af01c66a52d6493bafbe27ef6142653c" },
                { "szl", "4e2a898d6886039290c2ab2b5492329d0561425739e8c448e2bffa40831b7f9a4c71e02a3d9240f63024adc3601d2a5ada3042477a0654bd58478c7700673dac" },
                { "ta", "38ca8434dded55bdba7d9477ee99940f479d288a79ef8e1d0030482e0130ebb3289913b0c2e532752fff8630a0f4cff93b98ab9ce9924bfe1c60397c208ecbc8" },
                { "te", "c597609ff50dda4d83882ac20703036a4ccdda630b8c3807ab1a0ba68be510031e79c79144bf224b60bf14210ef05841154b6c2945fdcd846b1b413879981c2c" },
                { "tg", "2f59b9918b00a0f996d906e98f5ec02aa8da98887e375181c67ebed1099a89e9d775b958e4fec5f5d91f2b0a0b71d43430283047755d83983221c21a20e69cb0" },
                { "th", "16bb8da84a3f95c1bf23f2674df4ab92f4b808ef333c291312685bb3bc8326c0c2a7c03c552b5ebfcee1bc5c520458415023e99262daf7969e452cc328e75a3e" },
                { "tl", "adcc289d98134c91d3291e98ee94f07a25799857fcd6ca18439070330f19fd45ec872459b905091c6fcd52e99e0ed74fae2c0683faec8cd33eaf79016ce0dbb6" },
                { "tr", "1fe01c3b68958d997448f894f5b68630c354b1ad6cd29a703756ac0a5fbd551b15a7cdba1dc05349018d74c97da9fc351051fc995275c74f975a57ff000a4c92" },
                { "trs", "3b4c03ef11e554fcd4ea7c6cfa22138030c315e212cda9004c88bec145b23477287922b14ce13c69d11f359f7b2f58049eeb2a42b497a29856f76f55474a2dfd" },
                { "uk", "c85ce7b177fd99ee3581876113fdaa97a9136f858a63731a20803aa92225ce34fffdb5583bf1bd8668d823c8b1dc2ce75f7edfd97ec19aeb4002f6e80a761b7c" },
                { "ur", "e3aab71320a3dca8b0f9d546a64e9ac64621b11ddffdeb55e16b1345746ec5ab2db0bc9a555daed326811b076d2c3bdbd56f2dd6ad3108905db2a8755d3fcc79" },
                { "uz", "2b51be55e70d178acdf1001b8b2f0336b15fe2e556966cb54d205a7e21ed117fbf3543467d86accf72ba96a7a7c3140940107d1241fec4cebb800d4afb7556c9" },
                { "vi", "49a9b98d1eb5930b2a4940d8a736b1bf142d0cb83a814096942ce1d7c79cea118caa3d5cbdb93a513cf09de69a1e10eaa8433ae564cad5707075f4b1605a0773" },
                { "xh", "85a40eafaf14c1cc6aa8185e2d90393cfb27945a0216b5bb1c8ef64876450553050091a1db3b29963e4b2fce5bad500bdb6fe2592a4fb1e2739bc18cac680cab" },
                { "zh-CN", "fa6be4a176ca3d8902fa598235fc08d42f9a00ad53fa0739980ad16a5ea045cbe9e4f94449db6c5c153c12f7d06a4d971c8d2a3006a49ee51d20652a5248cef0" },
                { "zh-TW", "3a65542f1115fb392256d589a63426786150b75278e15bfd7754b9c485042fbc4e1da7b6304d446bebe96ae76c7107994d14948c78f4fbe6ec7768f60384e808" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/115.0b1/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "aa55e5fd7ef5c5e3e7ccb09b12b5b6d36db63502e26e55e79be9b93f78cd495282a3756aef511d65ad4942367f42f95aa1b8e59a2c3251a6f7de3974da7e4445" },
                { "af", "3949dbea714be0f5ccbeb6548b55d47b215526219bf0a4e669b36deac5ad2e930b031fa6c882b0204dcb5044b55b882988c16117f74d697d83be18e48b2999e1" },
                { "an", "538bf6b3f24616e92a480ff2cb1071202c5f04e99911b9a67fb6d23aa4f80f5bbd2f54ab0a2ee8176d39a3eff499fd78d486c6264a4b8681ff434495ab2b202e" },
                { "ar", "93ec1689f64726168b666d304bb3965c377a952b208106315839b56a0a3c7ec063dff93d20764840276ed8b9547b10fa845c611afa1b37d5629596e136a6ce06" },
                { "ast", "8a4618db63d55a7867275b080ae7b4f3aa6b498acd277b1106cabefebddf49fc9c9a69990bfd1559497f49517c163fbef4b31667d7e7fc8606df0096d109e99d" },
                { "az", "fef9d85e9b5c0e5d609675574a766d819897bcf83d02c52f66a924e18b411b3e7b3b7e53b8e8211eedc99759764581891b6ffa6970f3fa96137fa6093f83ad7c" },
                { "be", "ccca0aacfacffed5ac4fb8055d57c3ef1fe707f13f15ba477ed779912af44db891d9fbca5b996ffe1e0ec0d37e687b830c2cc5fd27e7ab1bd0c4b545faaa7b4a" },
                { "bg", "209c78bdbf0f1996eee7b187ac6a776c5712c59933a60a1bd3378ae0b3e73681375b910b2162af5ac25e11788c5c9bdd0c5985888b49fea9f7d1517858d43bde" },
                { "bn", "b45bacc261b62239081f0c76e31dc0bf2c06c772108c17cd79cbff19a2463d95fd8a0ff1853380051d98450afac2133b739f0b12bd9c76eef5c23f84b25da06d" },
                { "br", "517c2cc4e8a8e7582bda5e5a747d78a0e9367e014561b83be754f5f9f353fb4df3cd8cdc52ef608ea812a2c9cdd57cee7c0e7ed3da1a425c5565278d98af232e" },
                { "bs", "96be5cd29eea99d2572d18c61b360496df0aa7ec25d5789bff3edc1cbdd5b98e912ecedff526f0a4e26e30462c5575f5b18fd6762a36ce097ba0fb21b40174cc" },
                { "ca", "9feb9ea9a5d6e0881ca7e152d84d676a3d9094d1573d18683cd25e8ba6339f7cf9b0fc033deccb2ede3953e61569b406ba910c83af04b9b33ca66b43d17c6d36" },
                { "cak", "d0ca6c6bae1e3a0b00ccd67738ddf327c356f16939f60ddfe1cc1def159cdcc8752136d60463bf48b3ef721e85f58840cf677b0fff03c81ef95167e56ae4b43b" },
                { "cs", "6fb29697ebfa65d758fd39d89f716f88ac308db39141fc454186d279c5c7e70fc647b29751b2c0ef4eae5d0e6433ddedc1c7ce969904152facad9f3dca6d1813" },
                { "cy", "0d5c3342f7befe0e9c7cee668ed9834a6975935501508abae2bbb1ce80f2d57feadb1848e9462a8244f643a569ff42b8f59f4cb2a3fe2c32053d7a60aab09643" },
                { "da", "0913f19f83200981410e0b759a78b71886201d5f341130745689fdda8fc2d28611d8ad107023c74eb420f3390589925c415a529af9d22fe3a3fedbedfd539e1e" },
                { "de", "1b1d58fa2b27f88c014c1d96a60674f2a1360db1a69d999366c5fbf7fdc2f3489a93f719b29134df452eb973bbfd4015d848332e714d9f1b36008b5acae01798" },
                { "dsb", "9730edaf732d72eb126b22b97c11f691da8d525c77cd3c58ae86382182d576c11b63a677b838e544651295ffc3d1030a65d0a8755984fb2b3d394552be37dca1" },
                { "el", "ee0a6dc34ca3faf4ed5ab14107c58eae6c533a1c9ca00b3b629a8cba019f2420295dd4cd1a25edc95d8cdeac1c716e217117cd2a3effcef36b3175557975659e" },
                { "en-CA", "9508e2b0eb4fcdceff2fdd908dec82844082a7d952d705e487c8f51685b85a53600e3e59c126c055914cae8e0a18f95fd7de3f3cee398ee4bcf1f6d33e5dc655" },
                { "en-GB", "b44117e360a1c17fd31381e06a9d3efeb61810de7a6574467a706c1a4add44f746063ef385e5af2eb8f386b7a4199bb9b30c2b1a5ba672fc39d0c0d77828ebb2" },
                { "en-US", "04edee4643b325a163a40c58aad01a01a70afe5228d90964b029ceaceae47687bd38ecf40b4055445c057dd5da40f7a498cf9cbb35e2b8d39ad6c4eda4cc54e3" },
                { "eo", "cbb6aca9df5f15622d37be2ed1b7b4f04e5dc5256654a742d5c51c94153b99b2dd615ad8f03525f31464a64c51d36e9f67cf5cf6c6a17a1e026a46207a091fe3" },
                { "es-AR", "cf92386d821e2432bb25ee6e04a3914ccf0b180e540a562046930de091462f2614893db97e904bd1bffc8f1d568a629c7254357e030c02de00d5771afd9bdfe7" },
                { "es-CL", "d3ef25500c422d39ba8e9f0bd4aead988009a74bea8f965895ea2e2f7507055782bcbeb4692c712882ef6e7135ba74af1f679a918e438345920b914d80ca2089" },
                { "es-ES", "f9a1f328f09cfa73e93fd4a3bc86cfe4d0e9cf86d9a81cb14bf041b9e875a297c78a647ce4427a1c853cc745288183d117ac5633f3d4531a96a46d0073c3b1c8" },
                { "es-MX", "43c32438bed5ded6e3ac2969fc6eb3cfc4331064215bf67358c432dfe8258a2eacf520b8376194a7990bb23747bd1f2d1fd53dd7658b0c0bc95ce5b103eb3d92" },
                { "et", "905639e968506404a060b143051ecb18db53077074ff3bc133fb20d2f91219817e3abeb234111a047fe78708e243dd36c3172f1f29cfbf6b12df55279c1bb0f1" },
                { "eu", "914132ab587e192237bf13ead126cd3243ef3d7d64bb3a33a935b9f036c1b4b22184a976021692c30d1a32e963c0e4b354a8e4be7b1a98383d065a7c91509f80" },
                { "fa", "6b12ddbf94351ea2e509842a66158a936a8bfe839ca3e36c478bd347f56c4de7812d22ea2860c8a0bd48e8320f1c85b3d8649f9463919057822bd5e902cbb04a" },
                { "ff", "71b6025dbdb8328100944e092372112471c82cf2cf1b1a789f6cbcf974bdd1ca8c8c2f54680f427dfe7a5338c32b2397dd82aa72207fbb8b95de4f5554281f6f" },
                { "fi", "99ff8a11aa4f00ff8002f4f1c56f55bd22507c0022572d5f121ac110a6f9ea24ee84029e0baab75794b86cc098a03c6bdd4f1dd35b1fd6d1d389b0235c9f595d" },
                { "fr", "29bfaa6fc9983466fe28913750e0052d63f29f7690ce5a3afac9eeb995dfd4173eb5b1c4cbb10bbacacca0e38b7a59f8367893154480b5e38f4adf013e2c810f" },
                { "fur", "2559743d9f28294c8441d8ed7d602edb91883650e3770e020ee6d72c2b9fcd94724b555c2614732c57bacf8e14b7653577be0724b5222c5a65841f55d1551a8e" },
                { "fy-NL", "c61039ed3f51e838cc369d760ec3fc1eed1dfea436f799b96391d0010045a646862689e834a79b7e31101e1630686fd358c845313b570f71cf24f32fbae51620" },
                { "ga-IE", "a817472592cdf650b95c15b0e4a36089b8c8acae16a490683e9ea1978b9fdcd117a87c49154e0489270818fa819e6eb0c6c5e6c13b502cf648e0f3eddc069dd7" },
                { "gd", "9ccb08d0c9ffff705e9cdfb9862eb37b7e65c0c909d2bc4ecabe674e572164f1d680cd1c737460ac7adbacc34a22c1037e75a42cffac28e760dfe961b568e780" },
                { "gl", "8bbc540c3c76f5302b91f60decba5773c27c4c6bf69367a5bcdd67be967ace67d4b5285d9e3cce6ed3e0b24863bab8f855e45e322236d446da3b2db36f705731" },
                { "gn", "b351267a419571cd5462ae13099f2580d59050921589b2c59769654fc082fb1a0c8b88e8eecd2e953ae192acc048a0ec9336544afd06d9be771e3a8cc0f616c4" },
                { "gu-IN", "0a4233a540735d161618f94969c32610bda827e63d1f7ac819d6b94b89997dcc47d92313bf8aa60bf9748c7d92d067dd983ae32ceef04f8f989c384a00b095eb" },
                { "he", "d01df472caf2cec61126d37a671bab6e8329fdb8ab9625cb366306d2f5802ea3a0d93607abfd0de466706f1f4e3da9fa8c5df0ad8dd51312a94bceb312a9556b" },
                { "hi-IN", "1d13fc40a897d11cbd04ee101c52780ff29af1c645a81c9cb39fc723a24b8afae62c9dbd8db8ce39634434157a926d1ffebf4f45ccb87bd587832a39d36b0151" },
                { "hr", "0decd640eaaaafbb19c9740f7149c8adeb4fd4398cc045a701d5b09baad6ca9d659c53d553509eea9432acf4df8ec7e4fae77f4587d9501ec0e837b0d9fce10b" },
                { "hsb", "49e98babd93850f7432f82237cd8b889c93b5618c4f2445a8a482b9e98516dfe7d2c36858eb6c1a8e40f16fcc726d9533724ea803de6877da1ae0ba89dc4cc2a" },
                { "hu", "414c61b262917cf5133db3590c64d0190daeadf3e43584cd70dba015bf520e9e30c320c967559850ee2f17df758c60c4e66e92157d688098cf2e5819f99c291e" },
                { "hy-AM", "8099ec36a7676418553453af3f98c8e77a27c9b8c5b1b876435ae79ce34e679724ac485b7e4d28e3517718a920efe45e81611111e9b9c4ce22acb7e83612750a" },
                { "ia", "ff480bc363de4b271f7e3e652346b57fdafcdaee734c5e686b0e228d6832a22b03574e8b23831022f28a9c191f64f7a27e6c6b068a314f64125bfbfc394bb605" },
                { "id", "5b53d00fdc66df3c6d8b5370235697f668c08f06b35b13ec75dc25735e576601bdf3207ac3f1df446b8100918287c1d846e4e490aab988d9ba2a7a7bfb4c718a" },
                { "is", "3841793ad003ff6a9bca87c57daf9fad41ba4b39553263703a5e90cb00fd46ab434c0fbc526c128a5b22d10d0fddd742d4ee7b6074b9d1fd0a960c7fc08e3449" },
                { "it", "9d1420cfe8e62fa8ba3bdcfc331bdf8658716d18e6af5fc362a45b232307ea425c31ec722578b8228eb2d36241f649046accfcd0377c72ebef399f2f7aabdb61" },
                { "ja", "2572cb191608f479c357f2c7801300ab052e7dc78699f1ccf2c44e53228f5fa151e618c5fc93dd6231d07a57df9d696376fe45427c8e69204042f63702e14273" },
                { "ka", "34ebbd058ad40ddb62899774fcc9dad3968737565db8b6104e5af08a7d17ec775130e8cb9ae7be21c36702b2cc9f3d6624635e0557a8f82eed8161cae515cbe2" },
                { "kab", "16fe485e84f56ef0f86c1f08a6ea4b61353a46fb088b16294954bcc278b426e33c3a2f68b2e82b9fc110c0150403e62376ea0057975a43ca2827545c02489a42" },
                { "kk", "2db6792490769ad18bbae948b1f00e6f7b80ef58dbc6b76edcd174a19a4aeb1651e47ab644aee21963ca18fe54bfca7af291acd0d7c9aae7fca0539f24318f6a" },
                { "km", "9be01d574c0f5069012588be11443846306d43c836307fb219a0ac221ce8dd1083614d64f0c587bd7125fc92a7348dddf7ca4752f8c8b0849b58e89ca647b9e0" },
                { "kn", "962743a84527908a6dda7984eb434ec26628ff5310b999e4fd3f8359cac54ff467a6d5fcc6874af30ec212c5395faff5e666a1837cb1b4b46d09f8ffbcde8785" },
                { "ko", "64fc063863959dd07ba101eb34efe0de84efc6d2812ef88ce02a596a685bbb60d13458c48c2bdedca4b2bd138bf210abb1bd57cf8f027da5e526ea36ef7aca6d" },
                { "lij", "ca2975a324c4a963975d92f8695fb17ad6676a37c89b6f671ea0ae45f31f38ace95897c6b6c6d7e1cd8eb2865d1affb82a78ba56160e1ddc0efe38b9e3503edd" },
                { "lt", "cedee214083ebd8a7a96459bc696f9f3199fbd124743d55604fae56307db2fdc50a5dcf1c32ca3b9c95b470b41f78411f504c9b216c8024afd146fe419fd462d" },
                { "lv", "400e6e02f1d321f37bcbedbe15c6f18dda821a7e77bf7e5b777ec77d68280b7f4345fd53daa30994fba4428e7af6c13c45e0ad99f4bfea1fdf7aeb610a6431be" },
                { "mk", "212ade54442074864c09472018e0b05564896b3c0d3fb742dbb8d4c57cb338e4ed77d0d08635edff114cd3a777eef70e5e5ca00d4079092b8bc704509e9a71de" },
                { "mr", "951567b00f4e2c057d3407078fcbbf82d180b327a9a1693a6e958254526af4a08c9bee4330a120017c0d73c48f56654221abd4437d9684daff3df2006f6ca98d" },
                { "ms", "25a5d23035c800f859c9abc6f03be1583bb6aaaea845fab062a0e451ca8e0faa0c5c9f179ddd5f5e0e82bce7ee93a90ccd0f1c6438523d17628266b575a26628" },
                { "my", "b98683e4cce50c215c44252ecf8cfd02baf13c00908e84486ae567728c4d535c6b0880fef172d3f5a3cf1dc908095c009a90f375384f2d7cab17a4e9489ec47e" },
                { "nb-NO", "28c5764d7283c7f3fee26124886a2201599fafb79fb72ad4d0b4dfdf07cba260e415ed430e1800f85b41048d3275ff606c3278ec0fbcdd793134838343ef291c" },
                { "ne-NP", "76e89ecb9228f607987d513d40c3684632422dee997c8d8012f2c836378d95f0ea10d4b0aaaef82bf358db12e3fe226c6b3c7dd85df62a9ac2654f26bf01e3ff" },
                { "nl", "861c8421e9b362407f54a0219e3b6d6d975ea2e4b47ad1c0a375eb2256f445bcf752ea91607b19d7d7ade8ac14bee3b9183c2370226e35d36964aeb6194dc801" },
                { "nn-NO", "f87362ca21fe7e1552ea2d73acebeded769a5732ff80464be21af572a29cd1d2496f8b82fcb274e483632266827de19f097053832031ebb796240638791099de" },
                { "oc", "af5554a5528f50817d16201bad00ffb774a587e02059b79df76e0d4be10e2de57560ef621165bd04b358a7ac5e16ccdb9e2a7510bee54ac127a30fbf45b5f40e" },
                { "pa-IN", "8653959c80ce1f8dcf3c80aff54e5e5876a3403af3187de109827dd20c39ead3f242f9f9be68779bc806a7f65404ea4e48d670cfe8191b26bbb3f6a07d72c0c8" },
                { "pl", "cef9f4cded95114105270b3ea781d5a725d8ad7aec8456b9fcaae099b3272e993b757ad7094eab68bbf012ea7b3625e958086314315cf7891407038380ddcb66" },
                { "pt-BR", "15dc0a8787fc19492f41d452f432c7b9fd6bb6c1a95ffbfed88bc554a2ae43ed9f72ea2249cc7167d8c63668fbeb670b664de2b8d2a984bfe56765568ecdbe3b" },
                { "pt-PT", "5cecc17b8b5b0b207e473ae2133fd91e7cf9cae69e55cef60060e7517bbba4700b80ca0b81d77e69bebd1c00bd87ca43351140a56f55ab76db1c01ed86a75646" },
                { "rm", "3af82616161ba86668adc7c64f47df93da874cdd7538103ec9fb3a7e1cff452bfeb2051892b838bd505a5b5a0e1934ca56d1b16f2bcca0ef8ebc36eac5415dd0" },
                { "ro", "19ed1e0809b5e8f50bfd0a85beef71a4e2b5473a939e74c09890a474719762ddfbc8da76150e4004c817368350d93f3608a17b573177a520a86d1802977e54bc" },
                { "ru", "808b8e9f3b573a09f3c4d13cb75f186997b7e5e96c7de59e2fbfe8a4aebfa0f692129e9fcec3073fe2eaf0960382558059a59007d7bbd2fe54d8a53e651bd837" },
                { "sc", "7aa33448b70e48a77952f1613a537697ce442f2486ad7da86dc0373e789d735e122a7015d0a0cd846a63f0cd536feed23c266e0e7c3dd54feabe30ef739f1930" },
                { "sco", "14156bd08e162c21a3862e1316bda037e71eea9fcb55e67cc44d318f0963acb54534cb093582cb6ef7ef69048bd7816c40dcb4f9e5965fe79698ee8d58296688" },
                { "si", "837186a661deeacd534010b034e45046af26bda5da3f849e1b29396183a33ffe940ec43ff774fa3212ae4ae7d621fed863c306394511a4148997514eea03c4cf" },
                { "sk", "b55a11aa6ec6596475552fb60e3d7aad81eacf05dba0fc57aa4a6e7babe67f206b7fc8944133d21ff3444a60cb0f01347a02a914c4c8ff7fc2b2f11f3a9f9c60" },
                { "sl", "6026149cbd081d3751c2976ba9d94ca435fd49ac14f30a257f8c7f75241b6476eb55858f7aee50ab37f6e0d7dbf3ed84d4314b0f9531303c1746636f6d112d05" },
                { "son", "c9cd1f31feb9ced521f40c3480ca294033400fbead878257b5cc9c91703117533166aeee63fcd6558e79804abc7b7e061f722a4dbb3fcf436e6308cdff2c9dde" },
                { "sq", "3be6f9d08214acd47da804faa73c0784db85579f537d6b2a9c3353fb09e87cad67cd124e147a2b7f7a495ae7b9a219e34e74825c8b319ab3b9b3f4bda374dd45" },
                { "sr", "d929636d988dc19a991d6dbdd19fd67b1da77bffce580fa8a06361d3b4d6e5bd90558fbbe44fe3354fd394b6fd0329613cc5ae19bac205ab15199847397d5ef7" },
                { "sv-SE", "131ac1b29519231b2764adc1c4b453cbc5e5241582050bd7156b080a53470e43c1411ccea44b5ff65827b1536739ebcbfeab1d26387124ad801591f968148ee9" },
                { "szl", "0fd2c91e6f2823cdee9a50165287a10eeee98de2ff8b3611b03ca4c599672a5ebfec6cdb33fdf302f8703e1e4ecc92aedfd453155955ac83c86cdc0e3b6bf54f" },
                { "ta", "6e193db5c118e918970fef291d575c35cd5b4d1b79fe23b0ece7f2853e27b965b887aef953a682be289153cbc73c912fcef99a3e962e24a5cf9bf4f7f71f6a5b" },
                { "te", "0ac8c601e252f36b72c7f23493fdd99e44fc5531d916d5ee6cbfe83895132ff885b33bb95b70878b93a0a820554332bcdae8c5835e51a32a6952ea510d203586" },
                { "tg", "a2d55069d26d5dae0c9e38e0443eb7ac1acf339660f9851c5a48e4475c511d67bee9225bf7662b68c07d825fa4ecec0426f1dc4a2949799c06a9a5e5dcded9a3" },
                { "th", "5119c4efcc444d23bdd1d5a44987813a7713b5f47fa48c8757f80891215d83391c4942ea57f82d823e17aa13614e7f5432f9f5a676860b409482628c1edfff1a" },
                { "tl", "76974eb86b606e9e287e2542a0e0f4195a882e55104b815eb02ba301c6ba57f1278d3216072c5a6013330d6fc8b8fd2142c9bb0bc193cb8fedc7e65b1e962beb" },
                { "tr", "bc99f54b362f35e7986d87c7989c4501fbd5b7552bdbfce1319c9f985e298aedf20fe4e0a2fd1fa89179c0d9c0ae9c9bbda5cf4f980b05a917e7e892d274e53b" },
                { "trs", "c1688817ef39d78f0db9802bd4bdfe9ba5e69a75929a3e0db7747fb8bd732ebcaf97d1ba6a6c1633056a554f0f6b255bfc7d3ae3fd1de3cc11aa3e9e76d13022" },
                { "uk", "bda2f79988a7ec1fb44061d8ca96cbc36c60284e0cae47a05541fa08e0dfcb70dc3d6d61bc2a845277ff67991d0f083f2ab91e1b742cd2972adab78f86931af8" },
                { "ur", "4c377d6a268829bc2ef78693580ab4e86fdb515df2e99a46170fa84f87d646723c167851cd69d5a260c1d144a2d6473c6687679f9d31cd72ce4e8c20b64bccff" },
                { "uz", "5b43d3505bd27b779075e7d250fb39c63c46b4acb9a166c01f8245a2b7636576287e56c3726cb2b39905c4bd4fc85c333103e5c18daa278a07d22575ef3bdcd7" },
                { "vi", "59283aeb77bf5e8b37cbd9407c301cc5f16b11cf7b2c452ca92ea4892c1cb2e57e4a09d5c551443da572ec27f1643983fdbbc8c36f9d759339c02779db4488b7" },
                { "xh", "89a9d89021c8c798cf00d63fef4249fe290b992f463d036362ae41738b469ce8c458135b6a17f8b10781ead4c5133af4ae20499055072ad26a90e238a1e7ccde" },
                { "zh-CN", "93b56bf37a446469a53113c12a9bbd893506495e919a2302dd984a730deb1a26e9127f41ac0342117cc7c63063b5ec4293d491e8f2ca5843b1557f2cc4b8dbb0" },
                { "zh-TW", "391327cf300133316ac39829166fb96a96691cbf571d80a48e3fb32d3f3dbff8f2f2f3079fdf806c93ae468f01d98265be3624dc624c706293a8d6d235fb9a67" }
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
