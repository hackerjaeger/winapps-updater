﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
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
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/132.0/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "66322a8bc9bdf2acbc59137d43d491098f7288490c9f6c088811da29b85611f753ee8fcdfea370332cbb6666ab5234a6baf1ca8b4daba93bc316852fea5431fb" },
                { "af", "bdb99c428a88315e5fdd8b3e9b4fe25d69e171b9bec9eca797ee2f915e62d7271418d0bd3e2851d0d867c68b498bf72e378ca2fd8097cbbcd544667959d2f8d7" },
                { "an", "8667c40630181e2949341f81d24ea86c7bb8316547f8031d77892c4a5619e792259ba99918d730f7b32af170690f9d0201c58fd4c2d9b8d7cfa04a50c142c993" },
                { "ar", "f8d53ae1f1060fa0424eba0f4f75f64e7af51cd98bac5c6494ed5499632dab99a56a66f7c37be035f7084e566de3394ef4a0c4217c5ff557318170477b811984" },
                { "ast", "b23f694aebf53a9a32ed57db7c19909c9c315d59983d0a765e4b64ad8f14f9d23f7dcba298d2dd413cb09e32154581e045bd892c71ba1023bf8fc4ac1a1278eb" },
                { "az", "41f61ec451cc567d01fe1885c5c4efa3c1a1618713c720a380746b7433d27343d424e2f72ab18e5206ac382f80620b378f8e37c9c2dccedcc10721858a32ad4b" },
                { "be", "65120c84dbef9ede38e3d9c175a333b3409b7ab72dd4602e8dfc54e8a1a0e86a29864e8b20cd9353afaaac19d63fd2234a7d3723b856f42398ccb2d38bb1cdfe" },
                { "bg", "b383591a59a975845ae6d44e626829ba5dbaa4dd503f5cb15d450072dd4ae825a64f39901e8e90c11adc0a4dc481427423d2160f7d734b84937ffa8190c86de3" },
                { "bn", "eab36a343dc9bacb5e38366d496af739f74d2696108ece0ea625ea8625e6be0b3cf208100d9c9006c313e5c95eaa86890c1c22ba06542f3488dd83c53e735f34" },
                { "br", "379e868e6a6a5cec8ee1ec8ad0b6cbbf19b13255364058a3af2e1670a71d5a89344cd2425d45caf248b2b9ad5bcefbe1477227148fe825525d03ba44a9b23a04" },
                { "bs", "b5a216837f12486346f9e3f08587a294a60c796f7e4b69e91600167f1bd5ddeca2f42cf408b1f8ce4309b4bfef0aadfed1dcba280f2c7266d793fe82a164d01d" },
                { "ca", "1e66f1c50def5ec0a1c53b620fa87757ae99531641fb83876d2b657872aeda4587e057e381c79420fe958f298226d185edb977d039e557fe323b1a61e92d109e" },
                { "cak", "b0984f220f6068d7631c687f5885ea3e860b410629b00312073655b7b3923638995e56d5efdba0c886d2356bc97f1c7b1d97a4846989c8c81225343d9796d07f" },
                { "cs", "0aaa1f3dd55c50a4de828fb4bc679c305af4c8d2b8247732bd3505fcb72d28aaa4ac3bf103ebc3d3fdd6848af49a233a86f81d49d36f62daf4b8604e646d5a0a" },
                { "cy", "f8b1835d685af6634ea7bc83d59836b6d26ea1bd6fd8e3abce1779db06909a31c65fdd55ae6919e746f912d2661ba9dfbe510bc004fd06407c10a8ead82d3843" },
                { "da", "c9883cf7a4fceed1e1ab3f6d3a676aecb842d0b24dfea3b5f425f7a5cb8d30e0fb7024a6a75afd85aae10b2f81b3d66f03b5fcd5ea90d1690b74b081f7721b4c" },
                { "de", "87c4e2316e486c624ccc7b7a2a9df684facf4816953a78b6bb255b7536a7553b4526a464cf5576a21d05efe09323b19800591e3ed9ad2e33785ec264373b6e6e" },
                { "dsb", "1187688823918e12883910fec216be16311df6d5c08142d2fb8a40f266b17127bf4e1af5b53a88b53f81d3db694478adececa442503498498c649aac9a151cda" },
                { "el", "4b860373d34ad5f6cf186c4560f3a7ff2aa5dea89b5874b9f77ff98abea2010f0e2bcb756ce0c5726b53ec64a0dbb471b10f881178ac852a769e626f5e2ba394" },
                { "en-CA", "3355b60becb2f0aca62efa633b745e0b722a2d40ee295fba0a5e21ee65f8700063e9c6032635d905edd6ad21c09594cd99c8340adc8ad3d18a9542a29a673386" },
                { "en-GB", "46eaf10d11681222496425187344a24212857aabb4a2c7a43959041cf792d23d61a8eab18d627bd00f743000282ee30c0cbd73a368687b0f5d2684cfe3dd859c" },
                { "en-US", "844b14c73cfc842a45147c0c0afa9a7800139d468bc6295bebfd9d18b5a1a83cb3f22358636a4ff4caf8c7b98c7ff2ff956182790c97a10cb0f651e1fef08484" },
                { "eo", "34dfba44b201ff5be9c824cb81d20e0bce1bf2b66e43aba810aeded4d93326b1b71cf8606ce60caf1b68a04cd00db53a16046b1245982b735593b24898365929" },
                { "es-AR", "53b6e8b651ec5d02e6f313f849f7b12382c4c60aa03c21a1d8da264b2933ef130bc1a79262e1b70acaed581cfa749d66972aceddbb6707ca5f86e7c56cb1da7e" },
                { "es-CL", "e5d28d96cf9cf3eea1792a7f0f0201181c32e1898d2be627befac6d01cb71812853161ff908fc64746f63179e9cf193ee8b4a30e18f52ec2b5df0a26595e5121" },
                { "es-ES", "df1a8bb5c35ead956df4504aa0867137886878a703c9e0f64539dfa2e30375fdd24f7d2870e7fe361e55009fced559282def36725001290d5c57dfac94ff53c4" },
                { "es-MX", "f2a8ef0cd862f65008c31c7e5197b7123235e7f0cbe99d32e5328e4e6abea7eb01707558082d490df75df7396c1b9bf63a89c802bd1eae932619f9dff6a72d80" },
                { "et", "c16507b63b89d42bf8ed0968874c65fc71aa7d6882c88901bb6fcdd56a422682a7825b69bc365b5b6bfa3797e8bdd557ee5bded57289e2fa6aac408961338b51" },
                { "eu", "9152e4a0e33724f26bbf87d2fe9aa7ff12888290cdd01d6d33a891cad03d012d92fe632678fad5eec876acb87410f63a397528e2343a3fdb06ff9593f4bdc2ad" },
                { "fa", "41111657ed93053f0e51bf974eb0db6b9c6bf65402e0d3b8f01884f225753302b3e5f80cdbc0e72721775d7f156560eb12cc0cfae98755f575353d660b07508e" },
                { "ff", "07027fc118d38f37b28971a998bc7439289216069509186fd6806dbe2c0eedd42e3824bd3d1618f5821280e2dcd38f3186ed0677d4030ce49d24f51c616a60c6" },
                { "fi", "000e9fd12acfb48310990fd4ce197a01d086edd7ffd7a2b557961d7b94e009e5955447f355a9bbc2d8fc0f714a1477ff6121d598b9adc64f5cea96be3f3d75aa" },
                { "fr", "fd17730fef977eca7f31418ff45772c07fead9f686647b6d79328fe39104e262cdbfbb75f7ee3c9f584bcc4356bca2bbe13ac2e379299dfd1a3ce788b6c02249" },
                { "fur", "e1c046f3a18e770cf89b2a9e3b5ff64a23213af0c538421201b2c1d985028c9a89a48b25a5cc22198e2e6d512b809ee5f1caa66c15ef5e45459120e79a278d9f" },
                { "fy-NL", "93f9dd83af1ab4c3ef1c30b2eefd32d43fe09bcdb5354e711cf78ae7cedc4887481314aac6316889112fa94477c21514ff33c653ef1a8243282c029b2fcffedd" },
                { "ga-IE", "2f0ce059d274f78e388caea4fbc029b431ab2d3ebb9ebee95f2bcc03785d9a2a243c64a10f6e6dcd432b466b6027757fe8f41289c9fd38ec2c9290b54fe8d1bf" },
                { "gd", "22b98d801106faeb7dc2e7bfc70807ba7b257caee9f32b0d67f826b7395164740c4e6d9f39ba09e40d84745d58a4ae72c6cccb53a2e168467e0d6b64d13cc337" },
                { "gl", "80d126516dd0895fad622df111f1401221511c09439a19ee55c6c6899102aa04842780f42acc932cb90fc79a497f08e8160dde34962e7d996db10dc3f07a3a72" },
                { "gn", "64a16bf6ae9a9bb66b173ae549287d8763ec1f6ffcfea1afc1526fbc70a189643a6566cc83a9c711d8cc01542bfac564781a9788affac94aabf3884b50982a53" },
                { "gu-IN", "4b61d66b4737335c204ca96c31b8e109fde79c0e4dfabf611ba3ca2ab87c5d1c1e5731598ebcf0f9a35b278fe9e161619b4481c9415afc214deb2d5ffe215d1d" },
                { "he", "9bb022782245f02b23262c5d0e4c6caacbf5e4ae7e87b6aa64fa595fdcb82cf0bf33d5407217429a9423236c9b6217a7951b66057c53a7bc53a860ac99b146e5" },
                { "hi-IN", "782a616bf8ed51ed75965b1b8f8377489c43af6c40ac6f1a886036defb174d73fd6219384e97528ec63bbe31969ff98d80be47bbfd624040743e024f3448010c" },
                { "hr", "5fa778b31a289e735d4f55ee852a9bf280b097bb52e16f7e3d410da2e3d50cfdb4d25f2bc2d2aa5837d8180c54ba2f7dca15e5a08378f40deca13665f9baf073" },
                { "hsb", "5cce0fd6ad9d13202aebeada1b1d94c3f87abcd3a8fab6ced86a50249e27c1a8b1808f0326e76d768c10a5a802fe64658c6d044fc10c7ec4d3aadaa0a7d44cff" },
                { "hu", "3ae087636789d12e6fa6739e00b0662caac4bb8089c9694f4198d51164e6f6f7deb76176b76583406ac1a5d74a08b924181b9bf4aee3bcb4b174407993f4030d" },
                { "hy-AM", "74ccade15677ca69dd0cbd18c030f98b786b3d5f3364b75a62d0b7daeb6659d91c74732d19c017edf992285a73f86ce4bee8fa653e0a4b1f461060798f91c3db" },
                { "ia", "0a20823df400d6c599c7f14bb7a86ca50ac7eada8bf43fa97007ff570d5b59a9abfa3d8ea6a4e3b3417eef586130937f2c0c79d80cc484eca600752d2078188b" },
                { "id", "b8a6f11a30d1799152949530bd84fd5a000941a6f777777a8cef8b8cb51ec41b7ffeb07434dd4caa00c95aed70ea7ad396fdad3d5e0c5d1d21f8ec1607946550" },
                { "is", "168df647866a5220f08a59e8f986a4244d9d43d677fac843c5dbf30609a23ddd1dc347ea4f31695ca348e021914f36f35c868b926a38ef5c775e72360b674fc4" },
                { "it", "57967037bce2084402d32271c23c7c81cc8566906f314b194ef3336fb2b476965d22b89bc5b73dcf588d1998fbb4660e5d1713e355ffd371e85cc173bb0beac0" },
                { "ja", "7f1854b1280b0108e16bad01019fa097bb9511abaa06e651f32861d04d921641e386c16502f59598aedaf09d59e9928bf66739d5f747c7dd002f47d857a5d4c0" },
                { "ka", "153fa91f87b6c6cf171d771d9cfaea844878ac6b47639816042b4bd4c93bdca5c575ee24c4e34f628ac511f4bbbebd6c4fcb1bbbc7b0eb8d3217fe31c2ca8634" },
                { "kab", "db770e6092153d3a81028eb8e80283d58eba9448f5a58ccff23e4631c2f5785bc2f717928f8768ed52340c11db03a26f4e338ba303867e42504f75e48e7bd455" },
                { "kk", "9be6a3fd066cfa23210963cdfd6d4cd4cc3c4b2cf51fe62373cfab3c496ec2414c8329f9df85abe8f9a278bcd6e3b4eb72f059619295f6a96ee9a1493f3c7986" },
                { "km", "cdbb5853d5fd18ffc1f5511ad6ce168e37dc10229647af70aceea18fc7ccba21a7651bc68795077067bdb9c43a743ebf858ba9f28d1cf6f9da5fa25a95c57b19" },
                { "kn", "ef6b5c6560c7b5a8f80db18c40c7555ce0ae537946492d8e2bd2cc272e3a8812a286884c0173a882cc5b2672d0c0949f9bac1d99da8fb03ee063932c6333cfea" },
                { "ko", "c211b69e7f6751dbf59a026074c13c2b206781d6b9d3e971a3de9cd67b34e999f4d3770d3e9bcfd64b3b35a15dc66fcfff8dd943ccd6386d0c9f970aaf863e2f" },
                { "lij", "347b5211cd4414a129c74c13549e3f92d69f551a7dcdc2e1e1d5426aeebe984fc0492953d88c755c32d6abe14e175f26c007b3fc22127184c1a395daee24205c" },
                { "lt", "8908d6abcd0301e2f65e8a7dedd59861bf81708209bb50bd85553dee4f4e3ceb5d491da0b8e8fa5eb9a1d27a0753b6ec024dacb6d97a072639f9fc64cf082699" },
                { "lv", "8cc161ac92acf5b164449d8b9d5dd5c1223ad378715a6c064391a0d84674f0887df6bf2f0ac5585de0305c242d21e5136c311cbb28dfe29a557e639a2ebafab6" },
                { "mk", "85e33a993ea411a411d5a1ead9c0d682fc206ed1ce3f1c67abe038816e39dd59f4965cdf6206401e787ab4741b2098f7d4dfd909af9cc0b02a136043b5b91464" },
                { "mr", "1b0eaa9b7d5e6f1c0d03b4bf3bc12062ce1929e156183b3c2cb30f8c325d4e272390ed99ca39f8f9e2d952eaec3e8ede4a1aa75e4f7ed6e27ac4f401f0d344c6" },
                { "ms", "9a08558de2f09e7f1c119ac9b1f71024354acdef4cb52daf1855938ee552fd46b54715006c766cae4d6e64d408e44f0b66f8275a22c564d30eed0a2fa54b3f9f" },
                { "my", "4071a4b9a8e3e805559ae0a1c55ef09a792a97621d7f660c3b48fb59c0c1c0352bcd06fcb815c760b4b98b0db55ac58445ba5bcdc2f09c00011583eb2af671b1" },
                { "nb-NO", "9c7ab66dc0279de2c017d37b0b06ff543f5917e8d6678998853d4aa364c27ecd1d64b1df3175344b3f163e321de7c53b5419d2ba89f3c62f5bbe1f08a3b97a51" },
                { "ne-NP", "5d9f6be5ee9d060adacfae3e76e13b274ff9030bdd2ae957cb2fff0ac5bd3ec5920d58e5e0ebdaf890660a5d8fb92296240cdc1d104481d49074f2f5fdd43680" },
                { "nl", "07acad7092f6ff9d338dc1fb8ce5246a9361502352aea7f97d88e3d166c3c64e6a9cc792e6acd3bbd87feb6040d923d0402958104e53568166fc9d38c48a081d" },
                { "nn-NO", "e638f6edef633c035c12cf3522d0932b558fa5957f810172f08dfb832647a14aa15872209039f072c4668ad2797d10974c7e0368b0f6654a345c6510e8950498" },
                { "oc", "b483f384b59c7bd42c05c9d3d28659e7e443325b172b4adb41a9afacea753e4f2367641b6fc70cc152b3a8fbcd220f063ba152f75cc409c90e04a1162e313ab0" },
                { "pa-IN", "947229c18a8492b424f58f0b540a61929414a157706fcef2eb9447fa4cbaab1fefd6541dd7a2c7c519c1ce5335fc01efa4804f2a21c328daa57c697de222abd8" },
                { "pl", "2a8765463441f185a828a4cccb54b1790bf7feef9594b68b0765c5139b3bc69175c529c78b97bdd53223a87a8cb690a37af4c99a4af7cc2f2b525c748109c4d1" },
                { "pt-BR", "3a2f736c277dbdef70baa4eba8e7fb043bbc90967570cbb3c6fefcd88577cadf014d975c783fb5e50756932b5137f64bdf27de006a11a7765a822e15f3d16090" },
                { "pt-PT", "b2b0be10c75b27ba9d412140626b3958456d04a0ebb6bb6553c28318ffde1852c11dce5dc40d60e73c6c0b757073cf413b7dcb59d0ee98d8dbcfdefd8ecc1ca2" },
                { "rm", "243930faf9e34b478b3f044f2ae271b7068fe74f26efc63381de01eb30760e68c59df6b6105cb4b7692d18278629eb70c642f70e500812207a04b3165c0fa79b" },
                { "ro", "3deeb704fb349cf608467b4b3ede40a4c9b67c8e0f34d0728fc7ef353f995889fd2594df676748f237aee871db5a1f80aa73bfa00a390d92d292babc7d8bdcbc" },
                { "ru", "d492399b7f4c08fe4ab7d9c53f5488d0478d695de990ce5fa1811edc6877be04487e1594fda9544c8db3a59629c6e655e02273f9baec4478617f9233605e051b" },
                { "sat", "c598726679720c3f1cd9ea6cce456a0628c97854cc91c61e3cc842c1fb6fe3f881531fca9d8338484b7d435a1477f9b30f792306feea9f405f9f72167739a911" },
                { "sc", "e60c5b2dcf77cd017bb643778fc4cdcab34f8a298f0b45815c3b207c431c95cc91d9a9cd8b2d05994780575d409a1703b70fd72d6fbc10b9a813c8368c55dfad" },
                { "sco", "6cea3d7b1d707435fe91244de84f1003f9864f02db017de58a517f0e6fe474c4cbb2c8d9f401af45a29c3120bef4734c28f5e006ebc0ca0503587e78139e240d" },
                { "si", "d24a70d14a134e692d14902455436a2cafa68fcbd91d460df656f2a9fb0a67c6b570ee9cb067e0d2f18dcb59bcf188af16e37fbfe04cbb9fe9d19fa4e779891c" },
                { "sk", "2c6912305396d4ea0f20d1bcd23e2114de9551ace6d629d5de2763abfb27fcf0a5de691e40916d229bc24403a8642dcdfba03fe8b87eede3f9cdad57e45656c0" },
                { "skr", "faed3abb8324e7d3c28985016f43eff52533f7042634a42047a71e6c4f810b0dc9ab692bae06ec850cc20c0d9b53fdfb1851e2e542f0028841a91a2ede3df856" },
                { "sl", "967d8bebac66317658d31ee4e38e7819cf20288f15334d94f0e296ecf82ee87d83b30f0a6a8effe7c7845e4d2b4d61ee32b1d0f7e0c25c6d274245e4319257b0" },
                { "son", "34d8d2f018421a890ab1094f8995349f795ddfc0fe3a04f4d08bb47f31600f2ac5baa882a8bf2a1678ecdbbb054916d0a4674453902764c7c4c0cc3d41531349" },
                { "sq", "1c4eb9ece903d8c2115ddd512b515138f2725b7f342d82b297238f878727b8cc1c89b29608f84ba76cdfde36eb71ff7220f026bbfe356e7d5481f1275ab7a3b3" },
                { "sr", "d97115d00b966202019feb3f7392eb11b4346a0d5aa029b92867bf89f85e00e9583c6dc018fb2056a5b153436465d833f06f324247c94d2f9881578ff6885d5d" },
                { "sv-SE", "21ad2e454e088d3262f312e03c4b651b854a2344b2adc0dc84440d34b816ea6d92af8b84c91f8b4f411fab877d05f6beb85af9a33a808d60111d444641160243" },
                { "szl", "1f9d66d480053ce4173bfd23567733b299d8acb47b705c5e362b3782d70bb44603251e9709826d7394f72f033aef54d1dfea7586ac248575d5cff90fa8a2f969" },
                { "ta", "335eccdc99cff025399424d48d496d83b959e6234a40963e82e1c52c4aa271a71b316ab0b705cc264b2a8cbbbe7754e53bcb8168c52aac1629da9d62624e8266" },
                { "te", "674fede1a4f6ee821368f5aced0ff4b3f085b951143437c83481fc5c96fb9d6edd9cbbcbc861a7435b1cd983b9e6e39abad74d0ab4fde71cadc249b94fd0e1e4" },
                { "tg", "5af7498401068df876ad478fbaf44947a9978344c26958a9afa9c7ed355ad5ba2ea70586cc8a63d479930fb7156644c3f02d5df15a765719021f1979b55da97f" },
                { "th", "50ab984ac29f71bde77e2e0de59d9970bc0707abf51108e44b346ebc3a697b9cc7345f0f916b2e6cf108a52414ffbf2d638b28b8730e00bc81f63612d81daaf7" },
                { "tl", "6f23434ce34c9f4b54cc5096aec15a2c48e9161c40dd04a9f5201115b0867397852f9a3c81d3917ec5220ce3261936cc56eac331f8a4ad613deae398fbf46f18" },
                { "tr", "3cb1bcf77a566b341c9387ac329b46d98049029e2553d77b364521a7e0bdb2f56f098162c8e1f4846476dd32b1df067c336a8b72e09d51b400cc88d12abda4b7" },
                { "trs", "ae215ffa8191bce3e210f66ad52e3134d6bac57d862e2c26e2c95936f89a5b80b7b46d4998dc0d67a24bcd21820874332bde641d27806afefddd8f84c9391bd3" },
                { "uk", "fff6c565c4c52eb6542fd181f792c9ab3d462ad204dd1c867a06fa1b4a2adf481b780dba92ab73017e44d3ab47fa1af9251297b05726799d2a4692b87229ee86" },
                { "ur", "9e6809b1ee9c2ee0d215d4fecb2453703b79e1f26b8ed4dd0b19bc2425723eed55cbb01d97ee3031a0d578989e648ac35415c99b720a3e175abe67be1d560cc2" },
                { "uz", "b2e46ccdd0c2c855a9caf7bf52d74534a9b5bdbcc0b4e1ac801f941db95b6587aade2f809c8627157ff1a268cd4bf10c2727ca4c9e1ec9f2e14e39fe6cd57534" },
                { "vi", "9110077183a3610f530a071fe9c78df8c4a33f00012587e800106bf0f4e40f1d2250b91071552b9be8afd4da25a3358ea2de9e7ddda84c80aee8799129fd03e9" },
                { "xh", "733379af2ab16cd0df1f143fa8a806c2012845c0d8a3d5603cec3af9129f728bac8c12fab06c73e37304b661d3ab5e5ac32bb56e3b51dedb4cfcb0c08e019ac1" },
                { "zh-CN", "7af12315d5ca4d06403eb2eb8f3e308b4b57c64d706bbc82f403586771e968d861c94d7e09cd810753dfbb2d7e0148beb3859c296534cd4b25df53a097f2ee15" },
                { "zh-TW", "f389497dd9f8f693e5dc8e4ee4aef8015da6715b9bf8ca4bbd41ac95858bfd3c7c2a9ab1fdf40f8963ad784ba5e182242b2b985193b7ab093e526daeff9f0cc2" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/132.0/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "b5001419652c1516c67f26fad792b86a7c200f3cade51e94dcac1472350389909f0a276c5a0131404991839a1912563d9cd77967caac59dbdb173f1ff120d933" },
                { "af", "62bae42ef7e2005d8f2698496f7a1ab221a51c8f686f6712c62ff2addfa2baa75e1798315b7725a0d409745ea1fb52093020b4aa4dee3f907fb04a41bb093793" },
                { "an", "2f9392c12699a012f2e012a3ec7257f43c0000642b3c5acc12caef7769a1f09ee4a1d96ed03d952ed9ba3f47312d947716fcaf4e8f28f3d6da5d88a0ad3cdf5f" },
                { "ar", "af6ee59c1ec0706e7041255dca72131d05c9d412310be9380e17ed936151bf3dcf7bd2d43baa0d3e606df89805f318a37ce0125bf46970eae7dce39f51f1eefb" },
                { "ast", "7c64a9ab789e5250db70f014dd595627b18a6c6a684fd531bdad546a639bf7407cf2803f2a5bc16f128b7a8e81a7eab2c2016cd8165580068e1894c754e58ce6" },
                { "az", "e17326851dc6b1737a655adff7073612ebe2f94f3c0c3c7a5d821a43a0dacc827d5820fcdad957a8d18a7ee25e59d7900cbdf089072f78348430cb7e70040575" },
                { "be", "e6f8f3e57325d9dca2e16525e841508ecc321ce666ca49999aeb6f4f827b3b829af1aa4b89343f64047a20c23d9441fe64251767114ef1c2588f346783166678" },
                { "bg", "f2ef3ae18c0455a09597015289cd2dbc34953f4206859e43399910a9e67d741dad596e8c4e816b69883ff8796a155a400fe62733236b3c5ad2161314364071fa" },
                { "bn", "2cf8652a5dc3795f71161e3a1e471c669e06afce8458bcb8a4746e3bd0c6412ac0e60143da0d2a44f7f1ac881ee50e132809cc0a76bc44cdbb8bf6ce674b33da" },
                { "br", "c42de515e9a4dbd3dc44047bda0611a91fa738901b937ca1e599e26354d4e922cb3e821a0b90f9346905dd3ebf9d75a08f9b65b57aafd21bec11b15a7c2ccefc" },
                { "bs", "11f0f2cbcc94296e7ad34463d3659437dc72b28dcb31ac845d15e2cd6327872896b8f1bb5b50d8ffe9e626703992391b29a9e44e93ce3796cae227cc6739e382" },
                { "ca", "09a3a6304cf5fb9b5d0a045707069a02acc9a3281e89fb4e87bef0e1edba66c86c953daff4773188eddc8085e87d1d14f895bf0f5d237b1d76febff736f7f9aa" },
                { "cak", "55fd07c58494534d53ac1dbad554bb0ae555f0c03932e6cd308a389939e9435f740e42857a8e4830d9ea8f8422d1e0edd7ee2b73ce9385aa2c2fbc6417cebd41" },
                { "cs", "9b4358fb128ea1a6bf02c95f2c7e2856dca6b72ddd6810504d1ce130ee8cd578bf25f82da261a4b626e7c1dbf08b0234b453a2b91bb5da73035359981e27240b" },
                { "cy", "d0b4afac5b9c974a9e65874022c200dd557324fba9d66024215b2d0162efdfb5ab249c230ad6619bfe96d3c0f0ec64394eead9c7f4054ac7ca23da807424b434" },
                { "da", "b50c4d2ab7d67691b36de9e022e4875eba67bda27a28118f454def1be580fefa969361a52beaaf3bcdadcf92e7e8701fb0b3277e9d30e1fd5df8a567293dd640" },
                { "de", "fb4f719b0616d8a553ebcdbdfe7b6253d06e280e45dcb9ad9cfdff8405503227e9ec45bd4a43ea4bd78c32b8f6839623525a115a119827fc6a1eec7587ae06c9" },
                { "dsb", "4e3faedeac1c9041d6e9a02224cdb3521ac16409ae858cba9417b5c909ce6efa869315ed49a18ec5cc7bab1afaec5c803421dc19d9f2e16dc208d63dbbe2854d" },
                { "el", "bb3b16bb78f34a91e247eccc4d1b3d5549edde684b44cee9f6f3ebaf5459ae416204359272bb99737d1bc3958e3dabee43e447c43c32ca75bd6b2af0d9980f7e" },
                { "en-CA", "5357219804b9c6085fbd7ac403b4682e8d7c43e493376764bc38c39fac4a367bf312affd3e7da5ed99c661a57504a160fd32f94eaa993ac5e01038fb4262e54e" },
                { "en-GB", "fdb8dfc7dd47fe2ab153ad3b27b84113a174e07ab8b6f60b55da84f8876350d095ba6e1a13487c2b7c578a1bc041259f9a0f62a8737978e9943dd43330e63b7b" },
                { "en-US", "00ed33debe64a62554b1d59e7711b7199be7bde997c0a2647d00652a9162a1cb68bf00b8ca059d2c5c85fcc2e5bed14deaf7e31a585bc211d4c0410bdd451f3e" },
                { "eo", "79ec39029cb39596881077b45cc6c08d33531dce0d9c3f327a5b79a832b34cd450ff40f19a7cf40f3835b6214b6c664c1d6370293df65c002f184d7f9ff57689" },
                { "es-AR", "36100eedad4c3120febc00ee20c814df32888e7fb20913810a3fdf98c12e80d56c6afea99eaf7f4d8599348ac7f38935953043e4972a1001d2d2d6dd458b1fd4" },
                { "es-CL", "df1718cdc00df1be1eeac819dd6bd4a7154f0d1b0f7cdeb75c85142c70630fb1e34d7a0a661c1f1d519ce6d6502d163b8d69b19d9e2f08615487e6a6e340ecc0" },
                { "es-ES", "8db626a3a573494cb9a0b15fe1464f5a89b905d47cda3f9703a4cdc011d91eede7d746c3aefad225c75341311959760bfa926e001788de04aeb941ef9d6c340d" },
                { "es-MX", "b567d347489c99f388d622280af9c6864a2ddf9384c7c669abae1cdadddc220c8511160f023f36b29e4c3e9a2ed77127a35f4f250bd651fd8e141b1d0b6cd395" },
                { "et", "739664f51ed577831c3aa7d032d9605677626ce1d59af947e5e516225f05162bb2851991f2e3d5dcb7d43f0380283aaadc44cac83b438bd193a2c3652a69eb71" },
                { "eu", "9cf2fe04c0b9261be613dd702acdbb08fdeb093c66b12e64dc0e1c36daf97586b7eabc9aa86793fc781b3b3c821d0bd89c6c21e1d944cd57183955665c2cdfdc" },
                { "fa", "b935aa774f75b8041845eeb4d99d362697fe59868cb89fd6e237ab377526d74f34923297d25ec4f3a9304577fda4208a3340701d7ace2a76b10dd6d3233d59ad" },
                { "ff", "b8257e4a1a7eb0d83254ee2dbe8864a592fc07c89699e9c227c87f315ae42e5deaf07d73134b1acae21a6b87c41e74538dc1d882d9a785121e0e87c6eba094a9" },
                { "fi", "643fc3c428d93e1369738c37ca6f56324c88ecc1c94a40a27f10f59e01f611518e39a139a089d4a225ab70abd2f8aa6558f23ff094754483790f17d008239977" },
                { "fr", "5a00a1cc8f7d75d6c9fea30d1be086cc4d2898f9e3abf8c746a9ed16c416121dfeac9040003d6746667f08c04725a368f473afc714964b67fd0e58eb74e34a6b" },
                { "fur", "651e138d6b2eff9edfaa21d1ef03221a31b4484030ad005e159d650be97ff0548ee8ec6a66d25b8ef9000ade6604f4bdd9d373680bb5913ce0cdc904a49cc53e" },
                { "fy-NL", "51aa9eefc93a6e0dfdc59261f9cc0d3ffa3b9f1878f0ef7169faf1a7050b7c82a95c2e5926922798be2bbf64dec51c1bf856ad257f1f833cfe0fcfeb07de92ab" },
                { "ga-IE", "dcdfbf68bee72b30771edec0d8eb32bc6992c7a60c29dfb0452e55c573c7bf88ce2fa89461b6079ad571b5388ca7867a470c7c3200d23abaa733caa50609fe0b" },
                { "gd", "c83f3ccf4e3a5a562586ebbea460d975fa204fddc5799541aef1dc12559ddb4433546a69b5d77638ea9ae51ace96b0a8eb963b1ede4684b24a7d5fc657540227" },
                { "gl", "2d9a12ee1be00341981f390671918d0c29f7ab26d850c781826fbeba5cdcc09befe472f7a8d25d2a53d02cf63e7d735018e804e0ac6b0ac468eb537d21e76821" },
                { "gn", "f1e77c924c909aab47160ae47cf98e3c9858489f6c7a6d39ac8f8715dd1e9af200f47575ae457f35a3b8503a7eeb1016eb2a92078bc6d7e87de8373e36e5b2d7" },
                { "gu-IN", "b890cabc10a8f0c5db83c6cdc0da4205ba232d9ed3b0196a3a24f3b67f87fa2da1a02b13ecbdafc5dae3181b29894b13953008a2ba4e869cb8c131b5d1daeb41" },
                { "he", "bfbf33527734d15a933d38f08c2b642c583067bd19fd031996d796919bc970075f6945c34893ade18898656d22de7be707f04c15108d7fe838b1f4396ed61ab2" },
                { "hi-IN", "03ebdf5a7f4e244a521ffec62982fe534ace3049be848067995485a45287ca7c5d3f8e2a3d15c376efe3c16479ab85bb181606de5904b1687cf1dd0ea7f9c226" },
                { "hr", "f9fe7b2129e2adb4fd6a1de51dcae679856f45209505d8c88453d49418d4d92bb7756a140623681738f47901bf87e0ddfb1e931132b51511ff677da1a7b39451" },
                { "hsb", "1df7e1bcd41eba489140bb29f70601f4773f753baa06d349ed5e538e040f522be9031882a298b0aab11efef4288a039fe94081dc9e5e166a3290957ba38ef624" },
                { "hu", "02e7708829114a4e34c0960947304527b659ad3411e0147a9e3777d6e64a060453aa09172c3d4951d908bf73b2c3b7a23359e5ef26f36afeb191915958e999d5" },
                { "hy-AM", "f8c4c55204a10793aca6b0d0ca97ecc7ae3899b874a770dd998bd21874b7e8ba09e12843182f947b77724182628998a684c981d94fcdd88d8b6b2783ad60fe48" },
                { "ia", "14091e1ff51b88b83b76f5deef48e7ef06c0e15da3005f58d6696a139ce3b2c8deb68079a4359cf2a9ccb72bad7c0e1115cf833cb35faf1ccb9ddb883ea7b913" },
                { "id", "bc3a74fe2c1aa2162957f9afccb2744e0eebe08c6da23754e1800b1a2873a157a6f8ab736f07df382c6c402d9884b8c5b44dd9d7d1230a4e309092e557140476" },
                { "is", "4155d8f84fa20f16a504c18c302eef6453c6b7ea44fe1f2f8866c9488ec2bff854c94b39baa8a608ebed6a9e73cb5464d1286f04b931f06baa409736b862d726" },
                { "it", "8d53d72ebfc3fb4aa12e18028d45f210430014515e65cda4aa7ccc6e241cf0d0991d6869f4dc46bea3e9910dc4f090fbb5f574e347816e316e1e4c0544cd50bb" },
                { "ja", "e336be9d65cbec75cfa0caf8964232db29a63706847768bc1ebff51f0bbb4dcb91577f2ba52420542b2ed4d8ca05f38fbb82f43f588aaae7f3d7552333f6eecb" },
                { "ka", "5922d912bda6ade1b040060768f515512ea175c2b581d961ee68a5bdbfadda97814abc501a2678074793ab51efe46860895f1afddd81c0fcf514b1e96df67efd" },
                { "kab", "4149d9f8e979fcbb8facc7508ee7a0efbf3bb021bdc6ad9d22ebe8f2ace5cd021f6ddfe5b079d21f1b2206ed0cbd3c5256deac44cb2b527f76bb952625e3e057" },
                { "kk", "f0e0deedfb9a3885cc902dd92f9dc5175cb87b74665e5de5f8e8f685f70594db09a5abd7f514640e360ef126a6624bda13edee8982942b03bb34cc26a88f083d" },
                { "km", "5409cdf6098a8fb6b25861fd0017cb6b999586a5715ec08c125afa80ace04ff426bcf222dd46479a6e70bafd98d69e52d75d10915454c8bbd34aaa85867fdef3" },
                { "kn", "41c0768a52daeedd5d60764c20e8c68755a83d9acdaf51cc301cc7ddcf5fcf2a43970bbf994630b4dd799b4965012b150534e435fcfe22fb45b88a356694096f" },
                { "ko", "682f138aa36050ea19a1732464580a5d62db5fea1649ae347ea570417477f1459db668862d7bcf608c5f9f54707866cfc06543ce82ae04f2c38c2d46a50363da" },
                { "lij", "ab6b174c0aed0bf98955faa8e84bd49878b19b84ba29423f5f3091863dc320b25c376047287fe7a41316a0566afad8de4fd406c7db76112dd36b79d05c3a4119" },
                { "lt", "32edc451012a90b021c1d81171793a445ff84076e54e092824e46b7047ce4a5153c72e80ddf3112b9827d34e026eaf75c7683e0d176fd6ff3241d1406126bc4f" },
                { "lv", "41999abcbb7bcc0d9699b0513d35564606552cb9c786663fe09d829608103a8d466c0c0993094e0fbb347d130f96315a1262ca5561aa7d8b694abe4ea761db40" },
                { "mk", "74ae586224f8f7368e694c8edd273ee50bf16a6a3629eee7bada0828928e6dda870b87248646563be455f64a996c7739b62b15a876cc86d7aa268a5539bcae10" },
                { "mr", "2355cc3962bb41795b288b413e444672cdfd660bda02068e16d48ed21be788389e93463b3de5341d6f3850dcac07c67d91797d7742e0bd1de76c25063657cb7c" },
                { "ms", "46f2178e091c95978633b342b450bba39c8cdcfda0124d9d40d956f9dff2a308f0eb1d7564f4b90220f928abdb77813a3063a5cebbb17902add24278b4c4d170" },
                { "my", "ff8884193c0f8a91e652fdf17ff8b9f647bef58d9f4c742384cb8ba95cc8eae4aa771e8a70959a5d821d78e9f5ebe56a3e361ce544e6c0840eee76c659b17d3e" },
                { "nb-NO", "6bd20221fb4eadd4f7069237e9195bcdb492f2a3a7033fce5109e25d15d4cf00600e85b111fbde00819b65113ca8d2c9f85957545a508085b8eedab810aa46a6" },
                { "ne-NP", "e0148f4e9f40f995c6f03ec45daf3110189d44e0279eeb1b44c1b1cf7e59fe7d0394e8f1b41113025ed5018fd59ad2acdbfc4bd6f3e0bfe62401a8de0e5d8d1c" },
                { "nl", "cf7cd3c92cb3b94dc1e25b0c9e696c8dc15fe3a1ddc62adba8109e9c57722a9b4b9a6a551d50422795ac31957dba410471b832ab6dc5130e0a63cc9fdb8da77f" },
                { "nn-NO", "b24ff7e3b5307dfca6fc1e9c79d4194c6530313c9f183cdf489164aecdb2c800e72710db6aad8a258d41e753315caa4f1311fa40a489c12809c128ab0254e030" },
                { "oc", "103afad3a4dd8aeec5001e5926cf4a7d12a7353610abec61ae5d4c43e35f4a9ce11595388b27241597949896398a32a9d6b6425201d33ccada2622f6602360cf" },
                { "pa-IN", "d8471fb0cc1e27b3df5c41e9a63aeb9cdf537a4c58eb0198686001568b2268accec3d0498c2bea03ced5b98602f84b34d5135b0ae4052a4b91221ffab04ffe9a" },
                { "pl", "9cca53d05ef5914464c30f71631d00fdf77384b86a3e2c2dd7b45daff4a69398f890fd78c6a66b701595d8653169445b5e86a9338fe7a92dccd8c51f2d079c5d" },
                { "pt-BR", "df3c733bd3542cd4703b98e41d03fe315fadde93ca014588ca66d4d1fbf42929220f8a8f7efad622e69b6d43a1a7b3709c8aa2b2987b4d823bb10c1ae5c1309a" },
                { "pt-PT", "b016516e47619e5ec2d70c249775f36ccbc7c0204437d77c2b52892b7205512b7ae4b437b9ef1c97bab07d6317d66b6e065dc5da6ab7e5c781447748ca880e9c" },
                { "rm", "fed8711590a9bdd12a80e5eb981c86af75290e9b338ac8bada2738948042daebc456563bb688b26ae8b6d46a3d2291a71f64db33a73e4eab5debefc59cc4c3cc" },
                { "ro", "196bd02008b46861e75ef52efb37cbd6c0d5ffcc8002f06bdac79d0fcd28f861d2e49b3f4a7a1974ef384031065ede22c4a6ccb8d0c4dd2f700176aeadc680f6" },
                { "ru", "fcdcb488637f6082fddb7bfb589fa67aba41909a8a71234bb0e1010793df130988e212bc315e87bf661eb4706cc15c1742117455988727908b32c313e644a08b" },
                { "sat", "5deb3960dd5efb7b202e2e2b2d3a173e91cfbccc50a81f6b446a54c4b4bff81f0d3e3faf783ac9a8dae9b47def84846a2a0aaeef0d1d220dc0d30c9352447586" },
                { "sc", "e754abd51993a104fcdcc9b20706bd09a68210e684776cfc1aa537affe6aed70d1650126bfea646f0d5126de6e0e3f1dde521bcb7125641d02f79ffb4b98bee8" },
                { "sco", "58285ee272b89ea5395b7bb45277b2cbecfbf4b9d33140de659c26282c6e65643867df23234a9bccc0571e1c446f1e871dd071244c19c5847c88224629cace1e" },
                { "si", "749fe10ee1039e0f6a6a13076d8d62a8adb2263c88fc0cef13f3cccc859ac265aed6bf4819575b00ff4ef077b9742338e0a1ff3c3cdf2fde700639cb6023985c" },
                { "sk", "47788bacc447eb836f96a9c694e0805fe606f7b5eb59fffd616c8ed7ed71f9a129343adc9934850b69bd3d3d8905859c1b7a4d0e61e4f8689e9cfef01786cd9b" },
                { "skr", "190215c3334d30dd798525af44d7c7bde36958091f44a0224b87a50bebecc91f1f9c7ea188efde550294366c7ff6edbe6a78b7ab8e2f54c938e6cbf0f38a6d51" },
                { "sl", "d883077d1b8263df1a92337ceddd6b20b233004c9f0ef1d4d8cfb362649c4724746ee08c5d4218838eed6cd3ce5abc18bc29a51aa1db2c087bb96518a0879086" },
                { "son", "eb44887640859604a979707425138ad434fba89cb3afee3a09451deec90669eb86839bb4b74583c4f57300dd8b17f645c1966347ef1d06f71ba0269e9634572a" },
                { "sq", "4ab6940671fbe22a537e21cc1c3ff287e8d006a265ab88198b217ea1821ff085d47886c79be2314802f741293e7bc520ba85333c0f038aa29f537abb86fe117e" },
                { "sr", "dceaeb9c7399f9b7c7c2b10dbd0bd27eba2ff7a0662a50dc7522b88fd6d2be40f95cff19806609f3ffa56188cb1317747dd78ad13b5199f92d5729be5071a8b8" },
                { "sv-SE", "b86e55ddbbd10787ecbeacda378f9c0fd3a3d6b2f97a62c3764351dbdb256fc4a7de095414e08839773ed3059eb5bf1d1bf0aaa363988b508caca313f6acc0c3" },
                { "szl", "f497da566b7a762949ab801777ca2efe83f954c53e81102549e3173d17dd1896782b7f0eafc6d763cc16302df12eef582306f33efcad3f2b718597f9efbef0a4" },
                { "ta", "bd0c9c0067e25145c7c8ea714573daf420b31debf4989d7e57eef91d1bbdcfa71c2f1effa30aa9209540cd361651a2f48472145e1ce42b79fd9a789671ec3958" },
                { "te", "440f731df3836e1431daab9cad39dc9a6c3ce914b23574031f9d66115fbab586e423ff2a71302242a3e6028af2bf2e50bde61ef22c4bd303bf278525ca500e8e" },
                { "tg", "c631487522a9d0941bf018e7799ac7adce9c0772b3f4b93dc8bc16be668429069bb884a621f7d253158149447b0fab81ebb4e0c463ba9a12619f0418bb6ad3d5" },
                { "th", "c92a8588bc6f3b65256bd17aa254c55d1083409068b3a2035653a3fcaf6cbf4eb32d42ef41a35cc5ad20d309727f8e7ec0075353d95a828e8e0b950f1358c01d" },
                { "tl", "71399ca6aff3730a216da03a00679923e4ad2984223639d45c787fba41c1a9a64dd2bbe675631d8798cb1cfd96532a2ee60bb7973683e595f53bfa2583ec77be" },
                { "tr", "4dafeb58a569602ce93f8481fedb44771d261ac9d122735c98b58565aa89aff55dbf2d888c004f629c0bef0f38c8515ab3cb0f3801596c4dbcb6bad21c5d9ecd" },
                { "trs", "dbc16acc60d629680038894bb023adc3a4779a2514e46c9a01054e3dd6777050fc73dc1ab43ee229beeaa5d6502c3dbc14ef1e4bb0237ef7c126dbaa140477a8" },
                { "uk", "44c7267cd4da17516476bc9dafd279bee5ca863b50318b23b73fc4339b9794d4c007d0557e4756a2159d28c84bc7a91c0927f3e1077b42e3c538aa4dd2b7b322" },
                { "ur", "de1fb1677de61758b882d0161db1be32a966b8c98c3196d5b5329a35ad2009c4bf6b1a81a94d06d3b4bb68732e45418ddda05bf3b1b506df5c3938cdd0809ff0" },
                { "uz", "1826d558d522ca4e50234637177e79b5456ddce8fd0ecd62a6d012bcc991b1b420ddbd40fb9020890e8acf6f601e82de0c5a93d1b08dcba2fffb3591070ba368" },
                { "vi", "fbbd265c353cc785781a7a98ad18b92c72def4c7e5bb1129d213fae2c02243f57e1cb5ec674e20aa655f80d42fbeaf93ef2c7eebc5dc06afb09f40fadde85941" },
                { "xh", "d5cec32691ed1f4184cf8bf5364c56be0eb3affb3bbfd327de6cede6bdcb4461046f2030108c0f2007114f333f7a48f56949a3f817426497ac2a9826ff21caf6" },
                { "zh-CN", "433f1ddf901b246264dd5eafe1fb9a4055c33ae73ee0dfe28e0370657ccd099b32f8d20b7e69b365b8b34840055637449f865e8052aac313ef0fac881f07b453" },
                { "zh-TW", "3a52a202b926b70f891648f9b8b48cf65747d704de3eee1d1c52c8e894b31c52eaf6fdb91f4cc5f83a60c6311682c0d9776eed041d07ecf800262c220897adad" }
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
            const string knownVersion = "132.0";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
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
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
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
                client = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
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
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
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
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32-bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
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
            logger.Info("Searching for newer version of Firefox...");
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
                // failure occurred
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
        /// language code for the Firefox ESR version
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
    } // class
} // namespace
