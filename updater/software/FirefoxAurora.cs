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
        private const string currentVersion = "121.0b2";

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
            // https://ftp.mozilla.org/pub/devedition/releases/121.0b2/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "11feb061a84f0e45a5d59770988c4a3a7e19034758740bf5d55f7462bf7ea8c467a9940809ee2e71932f853565a3239f29fdd3d6a7b5d6c005b461f1054b3f46" },
                { "af", "384ab372712c988c9fed93bdd4be4c2a31ae7fa9b8c40be29f74460d9c0c781013071a06f226e13149a35b89dc4e4e448afc1b0319e1fbce39b43d7fa5613fde" },
                { "an", "5155a0267a90711852ec6839143ef71fbefb67dcb82640eb1eea0d15727d0339d530a0a18d3eee4847ebd250b213358299254c80899770570e7b7d4e22305759" },
                { "ar", "70093f02e51d2504f5740062fded5d5c7b802f8537901287779a72c0a168b61ec8cecd769c8ed05a9c6927639fecd111137a7cfeb19f586336a93e96561bfc50" },
                { "ast", "cc4709c3eb20f28d3080a7d7b82d5700c08f56c171ddcced319f4e896cb6ab7eb07ad3d9c1eca4be17e52e4209e458ed9bd344481c14b179f5ee0817f24401f0" },
                { "az", "53c22f28263db067bb50dd1c8db99566e4ac517ca00aa81b47d9b99b1a1c67f7cb86c55dc46a62c13626bae45ba98478545d5deabe5511c27bad2e9086a1ec35" },
                { "be", "4fdecc1dd063b9cb0207e63b683b553bf5fec65a7ec9c227fdac6c622a723d2359606db83a90af3c0c951ad0b218cf1a51e161c14cdfc35b545860e0dd2b3f18" },
                { "bg", "65f91bafef1123c8282f7128eb406866bdf70d27601f4d56596a093137d1712087d5f16e2e89fa81c45e4907cce0ee329eaa84acc2138862bb2c2e973422ab97" },
                { "bn", "f2f0a188edf207ccb9729d08698cf09e02498fbeeeba5f62a8da5b9c0d1ae5f9d7b58e0f9a54a75ae2bf4309e4db86e3a87d4fc3adba5199797045b3817fbf7b" },
                { "br", "e48bf4bc67a748956c886c0087333088cb94017de78a83db29fcbe92174f27245ac217acd016182cb0f28025d4d6d156e77bfab3efe94ce6db479eb7fa5eea04" },
                { "bs", "86d43cd7053f7296867f96a9f7061cb4fdc57814e1a1ce71541e9627246603ff3922c23d05f52744845c160dd04f61c9e0f751556fd7210629ac08aaf67fd1af" },
                { "ca", "71fffb19f0fb84b8e2d436cf99ab0c06d5cea09b6532e87cd1b4bfd70eacfc3ce113504bccbf17781df584f756529e8fc538d56b08701c41d6ae3badc35f52e9" },
                { "cak", "1fe871ffff54c69d5d106c3ebd688a2750cde8ad2c26bf47a9b1d5b700e40ed6f0a729e261deec7ba4a98a54e654ccf30f38e21ef4d123bf6f693bacfdc30411" },
                { "cs", "5dd344067b059896dc72f5fe295176aedcffca186536060a112d3f1479eb4d00844d2e4e29f9b857d11b284714531e5bdc6fc3e51af4c7b47a5833cdedeec00e" },
                { "cy", "e568be0b77aa76d76acc6a845f04a7363f8ede087762b121e3ab3a344b8fdffbc4f56cc36635c76c445281223f60692ebf316f14ed7b622d5e0a9d1d4dfe7f6c" },
                { "da", "67dcae008d4d5e9b524404e95522c69e29de4f684d010da880690ebee2b2e575f1925024203a2d1aaa3788124a2fa94b150247a71f61aa40cbd0766ec9f06a45" },
                { "de", "78e54964bf7a61b027a960937bf96dc3f3f6a4bbe6c1752f67a7072c0a824856590bf1576622ef5b94c87895056a2ace6b0fb19f2c7846a5691ca47eab0aa9f9" },
                { "dsb", "dc82b545591336c0e7e2cebc0e1b72e957b1352adfa35dd55f6967be784e237de7730248aefa13d46cbca32a98ddb81b81b44cf8fc2478a4f195cc8001a9b385" },
                { "el", "6b9572e6f2dc2765e532ef63d39b4aa02bc90a0e1c5ec7353cf1d76d5be7c7d1b82ff128c9d269ae128653d76946098c18b9b2a5645a0150ce5d1b9a66b6e68e" },
                { "en-CA", "b6965f59b1aef507e7ad700f44531085a3a872635e527617b4f655015362883d5a6ac86f06dc991b47184f4b3ae7e4273eacef47e4f07ac62524499aec0f97a3" },
                { "en-GB", "4c20989e1af59886aa633b89ae37f395839fc8e7b6649de570d8dbb209d8aca9a7065bd336d9bf480f72824fd937845b77d7b0586a84ba3e4de6497dfeffdc24" },
                { "en-US", "96f27b0021bc2ebca8be58ff5cfc22acae478588083ebf0905633732f5009e30d8626e628fb76e4182e04101b737ad8a76426eefb52678e96449129f62990745" },
                { "eo", "a68dbb857ec12ec7b426b544e4693d9780105f6b6e486640edcf02b6e2cf0914f6088ba1dca7b356a6336d9e2c8cde3c7615db0e83050cecb16e7b167d68c5d2" },
                { "es-AR", "4ca04225fdcb7982de6e1fe2adf469da8bd7bca6dabaa565e15be222a528e885df1c143a7639c610ffdbfc6ad3312df055f42f74240e6459e9ad9f38669a5fe2" },
                { "es-CL", "1307cfb1b5e6b120de43f5d38c8b06f8f9fbc7f6af2bd36c698698a3ff35a70734de3864ce1c0d52aa954b955f82a95e6cecb16603c02a0d5033e70c7cf253bf" },
                { "es-ES", "855f291209b7b8ceeeba0248dfc3ae0d971f67da6106042b9e3012a71e33a22e56db57e1c12dd10e739e4add33f15b564257ade223edd358f7963ac59e269718" },
                { "es-MX", "e12bf1d3cf271c28b2e0ceade96d028305f6d98cffe2b2ed81710a05ab78b7c1aa2581999fe01634fe51152fbaa142d2d3dc35557dfbcceef0e380c077117d8d" },
                { "et", "3d1382ad182b21635f81385a597887212159f38995f676ab844ae6789e107cd916c732261ed579689bf7a9ffc355ef14777c48e96311c8537d7e31e2e8b3b32f" },
                { "eu", "5c7cc3951dccf01305a0fc2aa887ffd1512f2825528914aa5058ac3571a050277b08f4b94551fd48c4249c9c988d5ca3bc8b872c887b87ce98c2b98c52e03c68" },
                { "fa", "2e6e887162b5b1121a291d9702dd9cbda5c07497dd2c5b36cd3809b55fd081f71fe9af41e00c302162133708ef9d3c57d5701320c7e3edf652bd6f667098713c" },
                { "ff", "802d5e0dbb5a071581d86584fdb8aa2e6c9fe6a0c96a12aa6a1b4798cc6dc15b0f58ae429c9262c5bf88e549c45eb8e55d001beb4987fb9c2a2857f4dff83462" },
                { "fi", "ff2785f1557a9ec33a340f35c94e04985d26089a81120ee3cfd495c2da675c4c595f70336410a80362faa1d8be45d9dcd3af7ed116a1c03dec7e4f014982d3fa" },
                { "fr", "46c79a607bd09256e6c046faa965473d7a830f07afbef0dd366efa366afff7cf70032caa5b96aa4e01ea65b9d552426f063de6c17f99d10571ca39c71227ac98" },
                { "fur", "d8b98dbfc10561aad4eb9d57f9b92881e504b4d4c16e9b42ed9168b751d77751ebcf621c5770f13280348370b7dc4a78df7236a3eee86a513ecbf80be084b486" },
                { "fy-NL", "e33ee3d590378155d62a972dab034288a41fed781696369b4ee31569422e1f8f73de0a730ea80165de8d4f8fb584bcd63302de8d58ca432ec8306665e46fa515" },
                { "ga-IE", "1e4f018e73ddfcbc3c1149c76c582231622adaa6f4e0e90f43d30b196d93e4a6c395e6aeffb7c03f16b25d18f78125a1e1f24d58af1d5dfabe1a9092e53089e9" },
                { "gd", "d47b49c7ae2ec86540a1cbacb2b825572d1d8267f9e03bff8c359f92d4d5e02d6333e32d7b5e7b323099e83991d1668dffa9d576f2f2eb2c38059972e3dedbef" },
                { "gl", "14f4cc49e5efbde5de6e1da3a13ca85a51c6f018dad89daabb553be89a094facef6cc85f02c43d699091404011a5911065ec258e57f139b4e39c7586425a0636" },
                { "gn", "bd1de2d922a8876b6cd9d68d52ddf22dd6ccf04a64bd4a6673ec5b890b14728eff221966b5be9eca55e717575ed8f7256a9b228992aa31a6e0f667129e3c91b5" },
                { "gu-IN", "e2b3808b76f533ec14b878d90656e58f30832d13bc4ffb690a57647d259f3378d6926ffd1adc099398bc86c2007c3a02c3efcca9df01add35c48ab30705c55ce" },
                { "he", "6dd463286f12e124fec80f9e5a8f66efa9a2852594796dc5697c1f2aec5a1024bbc38ae42b03eb9e022624c9a0263e7e575ae6db430c5f1860617a38ab750fb7" },
                { "hi-IN", "1c2dfb98314cdec2affb66864ba159f2cc25ef34e25d06bdbde86473cea5962247ca9f875175538511e6959b87770f144755e9ac509214f46f56b0aedb660649" },
                { "hr", "f1d7f850906484d43f34dfad93cbc28cbb3510016b4d40d95f810cf25c99aae243b30edf621363dc8755341d446d2f872c44eba310fa7f41a0a74db144577ab2" },
                { "hsb", "f3b5c54141566b3434c4d649fd6483ec08fa817e966e5706a3f71f682340828c81426a920b6107fb1980f562ad839160eb6a043aebfb4c608cfd3ad01806dfbc" },
                { "hu", "611d09ddd0caeb27e479273e12b26cee26380876b413deec0654880dd9e39f01b43cce7f064976fe07fe8bd3e508df6233f135a55ce3a9f6f0d966825b7104a7" },
                { "hy-AM", "aa96224a4d0bae18dce19233904c1dca73c3319700114cc94100de0516ce64e462db04b72e0cd5a7ba7fedf61bec467ae3eebf67f5e950572e988f71dd349268" },
                { "ia", "5298ca96c7074f27396bcaeee030b448675c86fb80094d3728e8df64b04d4f4e35f7e9dd73f1caf7ec276dfc57e8f87053150269c62985f000d5db274745201c" },
                { "id", "304aadebab315bbc68e19e92338726b5f7939775f3f8f7dea844726ab6cbcd6cccb338200ec2cd2de8e9b380562bab7c5b6b697be5c1b19db2c40f3559d768ee" },
                { "is", "65360b6ba67d8fe8f7aa185f0f2a77a911023d391491c7e6cdc8a478e74726085011d4e46107a231895ab64c6c7e75a4fc1349c74a754dbb5437752018fcde5e" },
                { "it", "2fa0b2f58ef2b8a62dbe14550d55edb44c7d791262f40be0229621c88b18effc5a96248f79aff516e14cdca77b8f01a4e77fb7611e437a3b027a2545221c1af9" },
                { "ja", "35f9ea119654328b14eb00660cf1e65c546bacac058b54e4f62d712a775ada457b75acf6ab45dd22ffc68c81d89c1ddeec038f6a0dc068973a3567b93adac9ce" },
                { "ka", "5461df2317f279f2588fd822e014496c95df88f40318c311f5df408658283a3be8bccd268d8cf968d85717af90c14b95e533103c5dab2c326600f6085a43b9b8" },
                { "kab", "dbd259b6bdc6ad8776447ced8c88d067cb2d31fb0878919683e9db36d3db1554f1d8d5bce65cdca9231dbb2072ac55f517fe3fe1939102c1a44f007c2dce3b80" },
                { "kk", "fbe1ec6c1e09a8d5142ba933854b3fe4d92adad70a41b451cd97838f1753a80db9dc639688dcc6454fb528ef6d3047a960bcee3d863de044e137f21c182bc7c3" },
                { "km", "6f3a84f10e2c0d40827dc2b9f56cd21320cfa5392925f6c39e32cb4072e20318d1fe266af451274ef6aa28da870abb8e864769070cd0df51e16528fd2c60fa7d" },
                { "kn", "71fe45b83e52c45265b1a050947fa41a0415bfb1df71ae60f94636f21b8740dff505b401b265a32efc711dfd1a009ccdfc2af8be420a89d1a904e4d3af1ad10b" },
                { "ko", "787ea8db56ec6cc24e414968f38301cb04bae6cf7f19663d0f856fca2d076a89271f1ef0fd9cb9f68338f0011526025271c02a6929ceb8822dd09e5027c098e0" },
                { "lij", "151359444e765f9098aa983d7aa92ffa76662ae62b6d197e9e77cefb5753a5c131b1ca1a3f088942c11d1a63b884ee1c8f2c970270bc87f187f5e705b27e5d0f" },
                { "lt", "042930366f0867f6f968276ca3c97391dc61e76cee2fe8d3197768f3ae9d1f746029ae4022d6bdc78fae28308f7a52abda5910e0dd5044b2a9e08af18f1aea37" },
                { "lv", "8fa0f1c8881a0e388536220a3b9de55ba9c3f575779fc3d02be772dd819cd063d4edf376285e987aac17756474ef271c7b8d99b598ea87d94b7cfa842a336940" },
                { "mk", "7d03d217a99f192de13bcd7ab4fd4d152f1095f316077abd093b3cdd8b44b98708b6722d350b5c77d72feee47025a845155c2e69d3d0eff0931be64097080bcd" },
                { "mr", "06983c8dbb8153bb7424b13467e3cf0eecdcd9a2849b9184d6e0fbd2c816188d665601a408828db9b198c83926d29470ede98a32ca3ed8f40f3e2aa5dd559851" },
                { "ms", "eb2750148950db43c2913bcd3c9291c4dbf4345e22354b2c38cd831bb94f2133f884f88997b7d553f5ce26c96a6c7bdf448f0d79116bb4fc6f5b0c4a9c7ef268" },
                { "my", "0468caa701e4449955975b0354059bfbf0d7fddc7f3979e1059c6ed91327b60d2a845c12cb70d4282fa4cb77f9399f46aac7d3be9c7ef4c90fe901a8a13e7fd7" },
                { "nb-NO", "652e8b4124d1db27d6d90bc2b6e9b8b8ccfb808601d1aa7013b7eb5434d89906e9d53d69106207b56ef04680504f3ebb4dd3b72ddfa5fee579eeb66c1d2c4adb" },
                { "ne-NP", "9d15cf0518ce2e68bd7e1159f1c2af874d989f78d6c3fed1e3ebb5a141e2619be684946d239ecfafb49c8e90268684103d6ad38b0e6478b7ae517cc99dd90251" },
                { "nl", "e2519186dc6f2d975a8c3cb1aa328fc0da7dfcf39e5fb5d7d0b60438d7f92868e12f4c0848dea07adfb2ad319a52e13bd02fc0bfbae165be3e7d47c73ce89310" },
                { "nn-NO", "2ab06bc175ec179b81146dc1c66ab98e045775d7aa346c8f6b2111886fa77f58b6375498364f2f4a869b35eed2d8406c0f8e12be0c215f5dd61dc58fcb175559" },
                { "oc", "3d7b14721767adfe956150659655a8978b6b5421cc17c40a1fcdfe27b16cfb3f064bad97f3f6ada56298579e36ebcd38fb897821134bfd607cc544b7d60c1179" },
                { "pa-IN", "cd68bd7ca99f75fe81dbe7c8317e5a25721cff5b7f864e3b364d4401c5d4e2ed78961b1cab77c846eb8287c3cbffd8717853e86a76428693e698cb2314fe3080" },
                { "pl", "19539b156ca935c115f832a42cd18a3efca261c9b0f5e2f7e7bc4eef1c04a0a9058d73cc8382e0091c904414ca160eff850a805a66b5b31d15ef26410f4b0acd" },
                { "pt-BR", "9f803bfc3d213bdc2ad7182b9d6c9db7eac50129069ae6547d5a01de481300394b1c3efddf05877f369a6d55c8207416aea9801fdfaaa2afbd8fd08a23ad86f8" },
                { "pt-PT", "9efb5f7e451b56e34f11e1da19aba2eb2851cb23a1fa499dc08aa100d75bc8e46711ce472c8c07fa42ea2c4cb93f225bcbbeb98deda42ba0f99db74d855e97e6" },
                { "rm", "def69f6b35c275c9bd70730a0c02da6de9bc9f121f596870a5c94a0a1b4093c62d119cd2d42b9fbf897c429d0f2c3569f5f2a811562575bb0c3026df5f5fb6a7" },
                { "ro", "5e9b9ebcc154a7a1624a8109c03de7ad539404da084ce6d79a7577477b40fee0559b9ea9521376cc920112c5ef6d1b27c64568627a554855e5e1cb1b78b3401f" },
                { "ru", "a9ea38f2b8d2e940eaf850f5fe1ed8da2100d39899f0694f73edaf66d669f32cb24bd19e840e9f0ce70bd6adcbe09578b66cd0815515e40b62535ec9c751026a" },
                { "sat", "bca508b7eebbb18e0a1f7a925d30ea4b8a23da4aabbf1cd657d14410e348c9d001956f7073744e91dee02a8c1b4e61c9b52f89517997ceb8824e8809d852d18d" },
                { "sc", "5cf2639ae49714413e15946a8ba816a8c5bbc50dcab6948735f71f98a7757aeac50f27fc3f5ff27613670f598398316046b668e46dc183d2aa3de4152c92f800" },
                { "sco", "d9aaf61ec7e5942cf7ac3c54714ecd57c2f7d7cbd9b9bc8a348577cfd9d51b44c1744c53835cbff6ce0438d72aaca4f2b9fae2a7a345b1458d110e5702d08580" },
                { "si", "a53b497ad1919f2e5b08be044489078761e6d00332c0fb4ced374dc0aaf0922ca0cc00747a271f714fe05c612876d86adee4a4dea79c9b68ecebebebb453e7a4" },
                { "sk", "6dd03cc3db318c77defc8e670c87b9a081e5b858f98a3d7698c3b32ce17407af2ed58b8c85ffb6db9faa577f3b5afd710d37cf2a1c73a85b1ec14854b57a39d6" },
                { "sl", "a1e164e3f8b9dfa8321dbba5f92d6b223ee3a7619e14ee6cd3188dd40f5df833c7bf589ff579edd09ace76cedc2861ec104c932eeb5c6326a826145ae9a40257" },
                { "son", "3843edc60cb80e44a1daeb15b56d0cb2f8dbf5f7f6512fcd5e94b3364388b6a505ba9d9249f82b38e553055457f15a1941c06e3a0b0b0dedc61a2d110fc033fb" },
                { "sq", "2a74d9685c48089a343e0e19b976ac9943495f6b5c7a9c4be71ccf38bed975e0cffda3a0e9593466bc84e73eb8ebef06e0adf5fb387c6a0575968fda47b78eda" },
                { "sr", "d1f56596bd088ed8c24def9b8b0e8a8c86d6292d8b8aaa5cea7a571fcb29f6319307f58d155cd39adf9fab444605d5e353075db136bbd1bd602088d70bc78291" },
                { "sv-SE", "a136651df9c7a797e1a3498027d936b9fe971ee1872f4acc54d7255a41306312610b65e0e627a2ce6fb09f6d04e406ece418da5cfa3e69595ba531f897a73da4" },
                { "szl", "d5e12a0c773fb6271a755d14259634e89b2bf84b7295bb8233804e0dfcefa1bb29790ae14557fe60239491bbabae0433cd7a75986ce59c2f7e34322cd04eac01" },
                { "ta", "bb684f2844a45dd353051269069be9c20f9fd49c54902b166f3b263148c0c424221f1cf50a87680c993875d2a5672bb0765354c03a2bd4c8096132c705707b5f" },
                { "te", "372f7dfe849838f00b827f559c762726e47a503d0efc12c59ce3862c12de8502d6375c08aeffb1bf254ed4c2ba11edaf9f1c5615f43b8c61a77c98df5890bb0b" },
                { "tg", "78cd6a529a0845a65b14f7354e9bb477d62ddf2d121aea6ec32ad9808f879b69a0410f60e76f87bc8052850f1cc898d3a5f91dce411cbf7b408988e47b0f19e1" },
                { "th", "aea74b98616233314711e142d5a840e3d997b3941fbd617a4e1796b2c2a444387f6b8180c7a3cee2af4fc7c944edd6cef80931ee5cbf34ba960e5d259549a256" },
                { "tl", "d4ccfe78e92b2830a9775d17530d353d4a80ab54ecfc88c87e3e854efe22ae266b558d57575544b144d4c90e1853bd91b50e4dd9f6ed03f9ec997849af7568ea" },
                { "tr", "d1b53290cc972c74a619f90a9c3a55eb3096539bf85579eab000a1024b6bb06f4c0f2337806484619e5b90463d9aeb53590fd7e69aa3e1c81aad441d5b27490c" },
                { "trs", "0674d43491b7a2ca4cb9474800bfb7fdb4ef3738c6945a065db4b275b0c76a059f1320dcd272f01a0eada307dc5aeeb4964d824ec396704ea1252a7620cda1c9" },
                { "uk", "b52581e932b51a46f1abb0d0b1b993c22ad7265ea42b96c1f15492c236d878645e8c1e8a2f229d0d169f6e8476f54a307e27c28c85d480515de2d178ec4aa766" },
                { "ur", "398e9dba442615470c3be9dd862edd33550c3c669812e83cedbf8602970f3e0df9b0ad9c028020d15d118f503cfb451a254ffc759f85e85a398abf68a497880a" },
                { "uz", "6b002d831aa57397c369725072fb4376761cd5f1560601376636d352022adbbdcf3a9c8d3455590f1f70468f89b49905c1e4281fe2fbbc40105d8820e55d8b80" },
                { "vi", "2e39db51971b963d42df259fe5199c7c9e054989f7bc38a703be3597c5c4ff0e2a81699bf6d1eb299493bad0da9d795b2f71e135ac496206e46e227e204dbea1" },
                { "xh", "70a76937fdc0d2f112e2b604e18fdbc6ac74b586d438a2015a89dadbb9c792261d32de40680ba04eb5421fc933fd6a17777bbd5aa0c1c17596cf7282ff314b13" },
                { "zh-CN", "8a7a8f4e9057c11b1a0689934fa7ceb90ccaa0222404f4de2168e39b69d8a095c55cb4ff34ca8434108250b4763ae349d9d5ddf8131d1fd33c8918a488a53b6f" },
                { "zh-TW", "0e707308b5b227ecc06d4b1b439d6751a39afb0d8f03d2257e8bcb037cad08dab3ce1a1166e3ae617be565edc992a01958acfc07606180240efc6bbc92aeaf67" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/121.0b2/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "6c5bb54d97be544c571785043218913a3b1eda1298cf02afd1d976e5808af0ec0fcb5a39b79f5c76219e00f0437a3b9e34358d0a27b3c931bda78ba8e990d727" },
                { "af", "70ccd5770f431df5ed1eca39771f42be3286ba9ac6f60e878ad4889aaefcc8d5b0d2518d2d1f75f2f8d88a6cb84e2f3a0d0d1ce1378b260a1630c336f3bb575c" },
                { "an", "307d44a3e1620458d78df0e68f30158f7b0aa12900aae4ed29df500a28b47a896559a813d04d9a0d6a95f28140337d0cff7fa116d19a5062ed66a87bcf547c10" },
                { "ar", "751475b0aa5afd408490b4e71ed022b3bc65725a92b11748c619f5a8f18465d64dbf37366196c25c51f7df87fca8133ba0c7391f6e875efb6a1a5239cc2784cf" },
                { "ast", "d0e2fd71183f2545cc2bc7482a02354e157a13208a1ba060c38a9c15d5178a2c73bce5b412d31b601f8d6587b76a3c591423bc1a2469e42ac2bdc8d5c84bbb0f" },
                { "az", "e40d2088faade589953ac29007c032c43db88dc82a8ba763409b1b250ad7e1d0f1601a3f4450f0ffe85dca4c1f939e443d61ee6aca33c79c9b967ce2eb353312" },
                { "be", "318c6b6af2ca60da159026899187678b2a886d5716d124526589c17d1133fcc890bc365538eebf8065e655dc126e0de4717aa89844d88c5b39ea35134766c1c4" },
                { "bg", "9425afcd5534a9dd82417622598bfb8f4453c6303876cfb764f1c0114ea2f35fe00efffcca8e217a7dee89b773c9f12a7c394fa01e69c3dfbd1ea9d721d92db8" },
                { "bn", "b3912b020ee257663b04a95aebea7b3cdb44799cb2be056e390c8c101c4a065dce4a830e3254bd30c5bee140f68e9752149492f77c837def2b14dcaffe56fc2c" },
                { "br", "d08ec92c231ea92a13d183a90700c4ffff4cafc6999a8226630ea5b2c587873498a2a632a4d82daa56c868eaa8593d53000f52fb9569bbd3e7585cca7abf63b5" },
                { "bs", "f87f1f96947ce77348f902913f30eecb9816769086bd6d23ce97911af44a0696cf4a2603723f10ac6c0f3a388554b571a78ebccd59c3b9e88508677794b21458" },
                { "ca", "fbaee5bc3ab54dcda8094db6fea38f8f001394304af95b59c8015f6630a888c74b814971ca8edf259d1ad81809763b986c31e16cd1e99d4d58c3bedc331ecf0a" },
                { "cak", "67f677a85a3c5d717ff4aa8cf84c2ccdb9b229079085497cc71b003fb4290e5913533f816eda16d6c8164aa810eda81cc3bafdcc1df652db876b8f7d33204a22" },
                { "cs", "d671efe1a41034a3e150638f4873a1646088f2f65d7ff0c56128ece880681f2e87d1a2d81db6803ecfb7c9978e73aee3182f76775bed0ed5f44ace0e33c19b3d" },
                { "cy", "fcb5c2ef8f9e0f638b64008832dd2cadaac3e4d15a3feb0c5a4613b300718b9d22f2ee4a04b78597ac30704834511d2dbb9640ab47b505ff632518f6432aa4a9" },
                { "da", "8e11ba177d3900aaaeede3d1f3b0f53da97c30f291a01a8d9bd3895d813983aae44a0d4b7b9ba7fcaae54085bc9af431acc193b7bb59aac83078d59c8f4826a3" },
                { "de", "074c2d1b89a4e2aca2ff5b2b05ab6eba3c73e07f0eb1d1ce0ef44792837c063371fd481f06a0d5b7eb20cf4ebba25489915f4e9a382884571560fcd804156c7a" },
                { "dsb", "e8aa3334c6c4d49d6df19bc890c9d1b33513218718796cf4de2f6b1ddbc1970f2cd9b81145badb78442dd3ffbbefa187cbe84ee8564f67181ae841b35a854cc2" },
                { "el", "fff8b48d4f3acedadfb39c38279bd7dbf5c1bfcef7f5eb849190b009d3c614652f62734d8ff18ec8e5fa47bd446d2ef4eab7995a7a22879313b213707724d437" },
                { "en-CA", "d8d27dec7e4ede7fbb188b1c0988f4646e99f385115309f47f4d0370aedfba080434bc88ae147669a71105fd92ff056ce7f8aeae653b01cdfadc3eb9a67e32a0" },
                { "en-GB", "4e3edd987c84ab5fbacd334aa430b16040ff86c9f05e9ca5864f2a0a443dc7813afc2192f07dd219dd78bbca8bb3ad1cb6bf41ad60d8add0694970ba642c99d2" },
                { "en-US", "d7d328ecd5d143987b19727d47b4da7a76eafcdd2fdbe2e8ea3b591886c98084af7f90056179c17689e2b9dde97feca3dfa0d6e2110db63746ff1313a57270e3" },
                { "eo", "3c1d3245e55590baea49208101de065fc9b4bd5d064cca2f93f768d46ca1b9206279e383186061e7017f8ba55329445f8a8f5066dfd331c4a1478f8993d3862c" },
                { "es-AR", "bb6f1256855affe719cc6717c4ff456d3cf1cea372f997c7f24783366b95b455ae59d75249cb7e3cd277b8a58c79cca34c31eb0185ad2485c9e7a5184e636020" },
                { "es-CL", "4581c32bbfeacd12e7e5819305d13dd4ad74aa90aed1672248d775881dc6b81ab9a36c6e64797be438b8daadf9fef6b019da0cf186dcc25108ea167cd89eac10" },
                { "es-ES", "3ba47c9f6376e51403413dd113f7834a7adced350cb84ffd07d50b939344ee414350c19763d20dbc21d4284cd1f88ef9a52e7c895653824bf65ebca01682b51e" },
                { "es-MX", "20e78cec7d8e1e9ae3973dae597e06a78ed2d97bb0185f7da893462283b2469b6856b5546d77a0e864a2b464f63211838ee9bea6b170f2b808928618e6a40a74" },
                { "et", "76016e392987963258292f4e7eea1ddb442353c87744df4f3334e38cc982b748eb64023aeb4be7dc6e5db43338c1afc15d9644b990170546bfd51797a7dd8c5f" },
                { "eu", "45ae2d30b83740614c67a6db5b05c91108092714239746e847dd466a75b645f4425877700f473d11e8926a6b42312e98b41bfceac7a88683f5798e6cafc3a922" },
                { "fa", "f79016a4b7fce1723b1c5695e4eebf5de286105fd29e07d168bc6487f9b2a76e69207a4f71b2d3dfd0e1776596a1fa625017ab72943ad6d66a899849736c01f0" },
                { "ff", "6e36de492e0d115d4c21d80075a60453d4c1e172030e6bc0a5fec37df5bf498d1a57e38e955ded52413b1d2687081382cb3b70768b4ec8d47e734268cb30ef20" },
                { "fi", "7fd24dde8bb94516b7240e7666d4bce0e523161a595d779ce71b4a58bc7ddf5690daacd5552c26bc8e7cbf86b723efa70527c1e1e259d96851dbf4b10a136bc8" },
                { "fr", "25a55b71122bf1b62706374aaff40cb75518f428c2b9a17f83190d9eae30d08c5bd607fd814a946f4db821b4c099af39905bf33393ea7b3787718592efcbd58e" },
                { "fur", "8cbf6b120de3dcc372323f646612482025256dad97781cf6e3a048e2bcdf408d6c825515f691a98e585d36c3de08953aca8ed78427ca29f6b56b15cb9526421b" },
                { "fy-NL", "e1f1a6bac3c60f150d8e12377bfb83bc12cd45eb31f1f66ae8f0b1c0e9453aefe20b17ac331363fce382669939ba6b600e3fafcf5a977d0865d24a524f890406" },
                { "ga-IE", "9f82d318414728199bb51573a93a690bf90cbc7a37eb21a924c2d0fe9db63bcd660d44288f9adc270b6e25cce6c4951b534c9e67331194e1c61c22ab7c7a9b29" },
                { "gd", "08a061e9c133571dadadb4bc300c9a99da39e8b57f973cef38c330a2e95a11fda2670a9336ba1436e8af0d216d458942d818d23e2c5aaf11db09a02e8e80ce53" },
                { "gl", "7a788478fb7c8b8b1cc08fd81b133d0fdbcfbda92e5fc4fccf3906b3dd48e0d51b08dc21bb0b08e68a41d78e6fd1cfe1295731fc940ed2ebcadd9d871ddcf5da" },
                { "gn", "b69f06058b85751b5a31aa3ad5f9fff288f0a6cfdbbb5da33d1a0ead161670dd020c1f7d25e0dbd115bdd94282d88552fbb84c81b5bd7126323ad8ea67aae129" },
                { "gu-IN", "6d046d5e0607f6f667b208b8f745ff88d565024ba348d2d0f83ac59a4e9a4a6960ada2cf3ea1ce587d04d527f46171055f860e1757c8741c640c86f978284ef7" },
                { "he", "fb52890ac2abc983d019c03279aad5c0cb984fcb3c76a584754d9b62fc9f1eeb57d3a74620c8ddce366ba281cec710946503d5262036306333c7fc18fb79ee19" },
                { "hi-IN", "dd5348d86e0dd905b1ea9b108ac7890ec8b1ddbf350461e52d6d0efc39acb44f14f3408b6feaf98155518aa1db803e6711fead9ddd76560e286831c29de63089" },
                { "hr", "81016b2f9fd3f3c2fb20c82e7d24583f5b82091d52e346bf69cf8246a6acedd190722423a8c3272e788de741bf7ff2f237f38a5d22a344ea54c705d8f6fd44c3" },
                { "hsb", "8100b29d7b0324a5ecba58fb1009fd33d4a76c269fe58cd94f7f85617838e0de82785cd96136f028b4a4f230d94d22b2e9648c07bc1b5ab9fd399339c1378a35" },
                { "hu", "ba8df563b022470b46bec76cc7deab9d7223061c3b4ef395dceac88d320dd71412968f12b9cab9e78a5974b88ba7d6d5e0e5293a4c201f22b105ecb5f11ef254" },
                { "hy-AM", "e9f1686b111ce8fab953766ee55011de195b199e817d420ec2931f495bdbfbc9362cacb4b3ad46cf190aa1691c59becdf6bfb01d06b7df03bad0bfa0fec7d4fa" },
                { "ia", "67543992575de5c1b6d7310a5460025715a24550a8fa2d6cdd7b988840cb6c8272f7cf74f5c6cc44bf97a5fba5644b550c67295659543a2b0ed4e3c921d69e18" },
                { "id", "74a8d2e1259f315f9e5f66ba8aaea810b401f0998911d53ccc0e4574e79e8fb294136a76df2e86991b77d98f5d306f43175226c2a40e4ba9ab68dceb97fa330c" },
                { "is", "ad2482290f879f8304c324d1ce8f55e6cfb90d6f65f9602d72b9f86016f2cd4c4670a1e4df63bcf7fced721fee287118542fcc311ea564520ca91d147ebde212" },
                { "it", "409b9d488a13c720046b75a4908eebdb00adfed042564df6b0f49c8880edc16855332c27ac143a462b2da1061425f4133d06ddf4a6cd16f670c3482112c61afe" },
                { "ja", "47af7a1af5e8bc5e85c0b1129156ce1f09c3e774e38ccf100640f36bf6db6f497eb78f37513ef58258b6162f2ca6d5cb54f9daed7cb364a21fe3d40fff4dbbb5" },
                { "ka", "b1c539a653dc195a393f467e5ec52c3584dec1f6684389da3efca69eacc6934e750bc82ee27acbbddb54625ff82558376b59db1e505008bf9c6fdaa727693676" },
                { "kab", "8de4bfeed2d406f4d771aa0892360f991e7ae3fd538773a6db4f17ef2a7d328b6fdc13c902703d493c85b164198ca379dc3730a0a778de7c24b9742ff59eb4aa" },
                { "kk", "f4542a4cad09cf3c4b11d7b76dfe71b47a5dbacfac33146781ba927bd6eeeecacc4be28e0a966defe10ece9fd519bf0d95278134041fc9ab0d1d716c615c59e6" },
                { "km", "a6a5840577cffced9e085a1ecc02f4095db4ee6e3127d80ea14d0794b4b854ed929bbc0fc12c1c6fc749bb743d23f0c0cdc472aad0f0d1ed7bcace7967f35f4b" },
                { "kn", "0ee441d19b004530007d5cdd26e2438889a687a48e2da891c258a3789c4ffd84ed56ea951895378231c049ad12a97bf580606faa944bae1c29c9192e4feff0e8" },
                { "ko", "68a1c8f653a9e027676a46df287a9a8ecec136dcdc948172fa505095531302bf4e07e0ed0d44dff4bb78c0f0745b11bd1e1f76eb12b50634ad71844ccc2a4a08" },
                { "lij", "2521ed257169ba3afc4c69c68fd9f31343509c7f4d81e3cf63c6c6e0a25de526e6ea7ede9ab8780160ea47e7b49535a359d8ea125bda491a5d98ee1f64ca2457" },
                { "lt", "5d76ba7befb2f9463702beafa132226773149c666626a10d7f180ca14d1d09605c6f24c0572bfd5bbead1ba45a09408a8f54164dafd0ed69c969766d909d4980" },
                { "lv", "c08ffc33f9bc11e6e787e862e3449ca005ac0b11c120d734fd7261e795ba97a70a55994c11c2cb426e81264d5b1e1f21bb9dcc2489a5da4f695dd5d20221462b" },
                { "mk", "8978ed5d9feaa136c5e9abbf070128e580a4ad642404eac94eac875176a5b15ee688cf6693b06b4c21f59e42d456d963a76bf64a913e44b1502d8bf9a38452ed" },
                { "mr", "2a8d4e7802e3f3c5993d1d33ae4e5ffeda6d96955ceef699391054fb84ca2d16176beba5b59ddbdeef3dc55ec9c566be4a355a4f14dab2e32f5725c1a74fc5d5" },
                { "ms", "049b33bf734f3656ec52333e85c919b5df88ca3f507d8e481d3a0b13f2eba7dd568483444c66183b0fdfbb24fcfbeab80d68823e26c879b02034d333ff876ac2" },
                { "my", "e46c67d312199a75c61538d7923f837ecbdf8410a225d36910277cc21c1964363d1978d7bcc0d31e24db33096e1a440522db12c28c8186174eec43fd8be31451" },
                { "nb-NO", "0ad88909b189e5a8f890bcaeda9048ecaef4fcdd8f766b81d7d8d0c00d59c82f100ffe575a55d38b8dcfec6233e626cf164dc961f41cb58d1cbe3e6e41b2b7d1" },
                { "ne-NP", "9d144f61267dabffb02f12678113cf4de8a6f42182aa14abe986ab96890c0ea4262ede748b5cfb1bd4edc35a5e8e056ef4da156f7c7394ec543ece44e12bb855" },
                { "nl", "28756f7bf6d6af4d627813505703caa34c22262c18ac580cd77b60305fdf753680ee52e1048b0f4ce7d1bd5f92bd9fd1ed1bdb5cc4d53ff2eff27714c4c40b41" },
                { "nn-NO", "f8c80a648f791afbd0aa6c4c97dd9e14182be113e54acb1893a08a17cfb6d2922ff48ec8d0eda4ab7e27f38281696b634068bbf45cc0e91565a52e840528a235" },
                { "oc", "eb5d241430a115e3bc4c42f81ef8458bafa99de5bcf06efb54f741b89214e49e119000e7ce7e9af627285d90033576f0adca074413faad4cabbb8102ee31d14b" },
                { "pa-IN", "d358d573fe69730590febfa33dcfe0cfdea1cb10858fab98d6acb02af7d1a4fd921dbab41dc2669ff4ca0c178c51434cb05e9bf04d6fa8563752501c9ffeee76" },
                { "pl", "47e06021416cfeb6660997136c1270926a47187f59e1268006ecd14f91b4f98c3587027840bfc378b9674edd41746f4fb3a0538ce3c2993507df3f3cab1fd96d" },
                { "pt-BR", "179d245af966e945ab122db08e4cb8e9045b87c16ebaaddb42383d300e7a93a2d588dd5b697ec625e569971c236f86424c8cd3341d97d8b9f998b136921e61d5" },
                { "pt-PT", "fbe45eff8b1b1914eed647bf1ee503eaae9f07dc87466922c3263222dd02cb210e7fbcb4bd48b11cf95722a56a20a1d762338c6c36c4c13fa6cd733470cb9386" },
                { "rm", "8270501f701d1c20c151e5dcbe992b1d584e07b592d550e0d51135447b7038b7686807fa05e199edb587040bd65c4a30846f108cefa0c5dd1a3b799c59cc765a" },
                { "ro", "c4a6484a1ba00ce5f154bb11ea840f42fc7750309f35dba71a57227e1c94ed217f1d97f47c6a36436f72fedd08f037abdddf2f05c9ce53c79fbfd66e3fc3784a" },
                { "ru", "cd0344e8b78e7e21465a91899dcc0def18ea559e9d7c7b4fa82bd4b86727dade3bd79b7831dcb4b9cbc3739a9daa0add313167f3a376520a2dcb7db5c78d830b" },
                { "sat", "9509e5b3b05e6fb752a27e4b0ade24ce92ce6fba864e974acfe7344c382a089f8c340ba58ec108c6645a659b86627ea610a0744ffc554cb450c6facf119af212" },
                { "sc", "40ff98d7176a2865e70919511ebf312b0be2a375d6b72789c476bd13563167b675c003220663fd4fb8792c04b953ee3a26b713af24af66611b977eb62874880e" },
                { "sco", "c1a0431e383dcff6f40854cc1a273819508630b7d411890baa742400eb372a0cc7fd1257fd089651a3a80f058dfc5831eb4d4aecec913f9899c9dc66d37642c7" },
                { "si", "fff0804c05b55034f07a4d871ee7a4e97f937351e5a3181bb885ec88a115423ab25b6252aba9cf0e2c18891490a699ec8b2d5e8fa0ef49c18accf520458c4f88" },
                { "sk", "d4bac5f16987af13dfc9b86f000c2e4c3b914c989985d42d72a7f83c5527f38d721dcf0299d37a392e583fb1f781224eaee4f51b2c90f172c1c7918790e4b214" },
                { "sl", "383816fc6fbeff33a52684d8ed28f1dfb4fd6850ded80658fc1039143a3cd415d1597fc60526503b5307a774b9fb4add3d3797ef8632b49d339c323fd7ac8ba2" },
                { "son", "74c3591775748b6a311d9fa5f97e2151accd47303a5e9b427214b4ead70f28b2bfc6a6a510e91dad3cb0ed9632c19350c2fca140c692fe5d774179b5a145e5b0" },
                { "sq", "3fb67adc097f4582c5b419d17b9589eed75f7d7681cc495789bae1cd0839309e308dcd6504e6137f0e5cbe91af2db9b4c36a8fc1c4afbe03d92f27ad3484ff3b" },
                { "sr", "ccbedbf561b196e4ed9da45bde128b99a41f88683e107a2e44019417fea51e58963ed37845586eec65970ac50a7b3309ea25455cd34a67a579adf02395254ac2" },
                { "sv-SE", "bd8e20f72e69ec7a5e6f178fc91a0bb99631f97b10d1c3c74cfc998b2281ea899ada8ae0ec2322d40f57e06d8852c43478d0e70d40a7b43623f162317da39b17" },
                { "szl", "77b8f11ac00da0411d67d879b34a8ead2557a2baff90cc32dd40c87272528124de2fced27d88ecbbc3b755903c90f3788e951137a8d0663a4d8ab9b0c658fbf8" },
                { "ta", "27af32fc9a921c65785abb7cbf4b8f3462b66b01b2ea223989628937a4e837bc80371b4f550bda11d2cc7a0e8c1fb836b0d9130264d174e71a4bca0d649cbc70" },
                { "te", "a0b24a1c83d9816816e492c7af76bd0eba632b26563b64f48d13037231e40e2352f9238bab5f59ac617f1092eafe73c9e6531e2c066280ddcb035a192d60154a" },
                { "tg", "8ab64687ba51d79fbb243616aadb71ce05d571f93be993bbdc9a9056b22a5eaf01d10042f73a92e02e0dc797420e52165ef2c8416e161a314ee59b928cd896bb" },
                { "th", "c57a7c6c886a181a8626c843962d0ce0decc71e9e8415a1aeb204775fabd0baf78f5000add20002024d9296cee71f5752a02accca93cca16b7bc14c246fa7210" },
                { "tl", "71090b7ebbf1e3144da64a237d68809d06d17b9689b14cbad40da0e98806cacb63c9f4ed82b16bbad0d5e7d1f5e8fd06454249042dc37eb41ba48c0a7ec08d1d" },
                { "tr", "02a45fbc6267255320dc2892aa19945f0f3bdce78d364ff8a76ec567985d7858e0cf38db34fd9082b3fd66e7a02c70a81311aa3e075e569423578f01dee90a42" },
                { "trs", "f78c1f4d423c2d0d1aa650f190c017454a0db1873eda429f0e510af99fa70ae192851e61aa239a940c76f9a78d7a5ebeb65c4c4adba26e336ae1870bef4facd5" },
                { "uk", "30810260fcee9b19653698c83a40ad58d6369969f2471fdae6ceaff384d0f23d2ded760a58c71be9024366bae803055d00e71ba611d90819eda0e8fc0dd23500" },
                { "ur", "66f67919ac9f14a5e8d3c73a0ffde862e96a0793768532055c6f609fefb9abb879470fc4387965f0f9aa28a170878c8b790fac97fcd15e5b315065b3d4a968d4" },
                { "uz", "e871345cdedcd6ea3a5fb3fba765d39a7e50b362f2aa7b518d4d16c34f4e42b92bedeb3969cebdc96b93afe2692d6eb9ef12a5ca18d54132d9e2e566faa322e7" },
                { "vi", "78df2bea8a1e966e96f0eb6ebc72c8b93b93ab5197c64188432a1a5ebccd9de4780948066b70fc0613c258061d2ae34f4933367c31a3cdf41397b6d130e7d5bb" },
                { "xh", "59da0cc74a025d7e50e90718768d7e85ea4440a1853df1b8a91ec2a620b98f9410a634b4be0536cc407fde2a8c6d892911ff1718a4f8bfbf4b88a647e969a803" },
                { "zh-CN", "813ffbf7d78747836abfc58aa16740397adcdea580307fb797e399c82668161c777633119959b7918785b4fa480d875cb5bff325b1fb91aced821b5376ff7c83" },
                { "zh-TW", "f05e43fe4b44b92479f66677d4b5bd661405067c9f259be45822e65c99c1e44493cb72d9ab97c137ca8e2da68bcbd2c5ed5a57cd9278e62ff46cd06563e52b50" }
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
