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
        private const string currentVersion = "137.0b8";


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
            // https://ftp.mozilla.org/pub/devedition/releases/137.0b8/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "4373ddf28d194580a6387f63785e984d8e872ad423065b9c4dfead6fe56351ef277afb1a5a222d4bbb508324f815ab7efe360fad60f1eea452b5361f1d6fddc8" },
                { "af", "1634ddc19a32ed2aa880aeaf5e993c339ae272068011435340fc367060e7b13030a4d19ff42e7b75ebb2fc842988f928ed5b1e9323ba51eda66ee3d2d72c4725" },
                { "an", "edb2b9e5194bafc6a6b0bd05a3faa70ad4680d4c54543ca2b4f9b253c1e5733da5860c94ea93eb5fa0306e2077bc885fa527937cfaa3e7748531c75842d758bd" },
                { "ar", "747a12f3f27d60fa83bc7363b2337439ff1e6289f3a52b36eb42e5a1987f7222849fbde55f15f557ef0271110efd6f77cee0cc2e6ed49f56d5f04d9e21cf3cda" },
                { "ast", "5dfd8e3f3e0d9591e6bfe38361bc9603bb80804ea53902a2c0b5d543860c585e6eab5b507b6e057955dd5c7d8dbe91458e298ec0ba5d21e89a9dd11424da4e16" },
                { "az", "5c82de5c6339455e94494439f6aa2d1e7127044d52c6a27662cf89b945751cb7f8443f4ccad19a2ff9d9d6a755df17c60033af3c92bbc30bb6f08a3901105c0c" },
                { "be", "2a1a1b51afa3351d547bc1f66be603829c05640134bcb04ab04e63225cf852cc0a4690786448ff02424d7fe5c3cc1c75d456af707a52a5f606f81de5e0a45b36" },
                { "bg", "93edc7536a6c3262319a7070a5b4f8c944bdb3aa691774cd9c36028506d2385a9e9bee381b18bd22861ce48f8c3e7f715cae72ad695f6ee50f0e4c54322b19b3" },
                { "bn", "3097de6ed3c6c28a887c346ed3da58cc9ccabacae420d658067369d4fa8340cbbdec975f35fd2073b0dc697f589c02cd5a38c71905185b6b3704459ff30131d9" },
                { "br", "5fe6cc00aa66f74303767e3a75abde2593fc380414f16386e8d1fe39bbcba24c00299004261c529d023e9e914c9c663d2c4a0d3373cbd1962a5bc5510d0a8f78" },
                { "bs", "a55084177033998f939a8daab1fcdca738eee494e0b3037504aba82561ee557208f669920afc213e77debc71da610cc78511a5b3a3bf91a37346dfa4c64c5a6b" },
                { "ca", "abf7b701d3dfbffeb61d7680c2cf79f67d1c6ae80ec2cb9bed1877b0f2fefe82fa2c3448c95612215d6bf123ec13bb85d8ea8d3430f202f2c01ad6055b86902e" },
                { "cak", "882b1490d4ba4bce8ed17f53106765e6c29ae962bcf3ae55812bd0714821f40f17d686ab868328a89b1fb2e983adae16a16571ccf8eab197830915e17076c276" },
                { "cs", "0a3f328c56e8c5104c84d860562febcc43ef889c5eaaefb280c6a6952165942a1b1c33097c41d02573e725e2de9be810d0eb76cd622079869e0b2b75507dc06f" },
                { "cy", "5f3420bebbf4acb48e6e7a5b1be9b24c30594ed973ab7c1ddef62158b51dafdba303148bbd582557e24299a55e226e2a8c52013b86e46fcf7f50ffd588260b1e" },
                { "da", "e3f7de5ad8e6846fb85c9d8293d25848ace71118735f3988605b2ec24ea996331df50b8fd8b5eaca9c828d718fdc8db189430fa06fafe9ee4446d83d4136ac56" },
                { "de", "7810d35f0d85c00a7e87d510184b3178c9e57b06bafd8f262db2766504735fa84591e48af9a92063af6b74a759350476dcd879c7401eecf534fa08ae1ca4034d" },
                { "dsb", "9e81863b47d07518634662c2d8f2caa74bf1b467801db9f0d610836d7b725a00c3e34ecb809f17f413071797560555c6e8e2321cca19259435ca908b3922839f" },
                { "el", "3cc65b1119d3f3cdff0d6d47912ae0b68e3ce248bd6a85063c21d32d58a81bcc3dd712319b40b7623deaf5243109bd6647430b8672e2a48e8956f96a9c12c6ec" },
                { "en-CA", "e8e189ae7310b53fe1a890b76addbdc46cf05c7eb127221a92f185d1281ebaa5d06fb8b64d809e0c59e1b0df00f786a909f51d4efa936bc8ae31f55d1a34457f" },
                { "en-GB", "b600acf63efd0ad18f7d4f47851bde3345bb2433b96fcb0d5f4b7ec74dbe2e1f6e0926fce6fc4baa929ace26e17a94ef391349cc9284d6c0265138d465ff9027" },
                { "en-US", "43d08218362a4ce7f58c4a74cb7b457982141fc755e71b699ee50ec58196add914fab0ddc10b1514a02c8eb99a4e0705443d783d88154234723d4894d568d529" },
                { "eo", "29274f3a4bc4c2b15e7b3fb347c6ead65757f5fa1237980607a468faed95eeae7cb274f8e2a1eb144e730cd09c0e4ae05f4ec9c9fa73bb4a038f4bfa8e34b7b9" },
                { "es-AR", "8d79688da9f2474a0cac750e7857c18d4c98424644bc2e41e0303be99a4e427337d4b1767043973850e1862805cc8def871d77ab12e358455985b80f77ce25c8" },
                { "es-CL", "3d7a5859e9d6c2f233c85bc50ce9a494cac0030c74a3c66910c4dd58fbb556b473d82282aa781fab9b0c85049fda6120fa47811dfefbce4dfeb31d7125396523" },
                { "es-ES", "244abf5e9259e80ee66cc9e052ea778f91e2ea9468158380156507b08d3b12c3f23a8480489bc81bdc8de33df78c565169aeb2ebcf3166674c964159944d41a3" },
                { "es-MX", "c3f7044c43e0c683980521dae3ec38c8078dce1c247039337d1058395870b9a1c42485513156091ab492b1fdfd5a01445b93f71fc21867251b92547bbf6ecac9" },
                { "et", "e61468019f0bb88faaf891bc54bb9f777fa6a378355e4dd7e827e0829338d2405249ac0c65a452a633b21dc77363e7d13d6a636659d32733b1abee4e893e31fc" },
                { "eu", "e38afa841da0264f494f9905588a4125204e61ef9cbec7123b96bf8f2873d31ad3166c524ee337645ccbb1ba41fbe02e1d50c08d2e3f6d3233083ff9f8479944" },
                { "fa", "e71e436f443c2ec94198cabc0ad4f3a2b5d1f6b88723f3c79e866e51758f1584f03a7ed8c894a579b072d855dab06d126d7149064bc497df3e10a9ea0458d91d" },
                { "ff", "a1eaf17ff30b80c20c422585d58145b6d3f37f45a7fe83a89a8a1f1c235b62e3fa256836ff7d60fd88bb6b387c6921f206175a42f96ff4d71641fb3a7daf9275" },
                { "fi", "f6486a2c8dcb52b24b5882e1102c377380a291865a2d6fce3e55250440dcfbdcfd5de3562f497bc893e0237f589c93e6dce8a1a00f25a0f9426051693523143e" },
                { "fr", "90ecc00445a2c57b48a99c2b392a2a39ca9675b338725f7d20803486bdbd41f54baa43ceaff218080e43eefd40e3f180d72f2536ebdfff4cf93cc0294badc5c7" },
                { "fur", "22a085fcced7b904ab3b5cf4cc301a6d79fa3b7f09d2bd132e1305b1f7a8cbaace8311821103ad7cd4cbac12339cf7f4ac8050a8a3cf461728c0592399ab4bc6" },
                { "fy-NL", "719886ea6f5cf954f4bfe79e8674c1d16c0570734225b24a3def3328c134556dc93fcf236b54e0ac1a1577e0e1c640e58eb2176b4da81bbab045281231d17dfe" },
                { "ga-IE", "58b42a892d9a7d678b3326209968ed8e448270c9fe34d3c2b8dd45a80ee7f7ac728461ff172f9c80cf648798d597a9595c5f66b60c1638b3c7a3fefdd36f7ee9" },
                { "gd", "112bf1eada6f84bbb2df2ce0ac3d08c9e047ebff36514dbe3df6ea1b20e5a731aadb3648a9864b2ec1383226cb0dc1e390c3431cbb7ec78a298840507de73a6c" },
                { "gl", "a0d7dfedc9d955f54957014a2116fd9ba19200ff5e740b6eaf99198338b9edb4cc09be410d382929965b33641731449435356e7dc5a4b447847b5ce4d811ba16" },
                { "gn", "11207aaf2b83fe7e7ea7fb02b933e3136b929246264c4035622685be4c6cf077411328d58a3c03429a8e963c40a4d12aa9449d33cd84a107138d2855336f5c4c" },
                { "gu-IN", "16e17a4a98225a38edbb7f4eb8f948b7c8cf07a840a5c16f0137fcb5b767ae0e44e51da8ca34ca127369b35460b7b17a54a5d774c8f17569866a29d159392fa8" },
                { "he", "6618d5767ff15a6ee1f7d542277cbe364b08a5acf6d005ea9e500c3add04cfe107341522549508ed1f5c75aeccb8e78d29b5403af5a92a706d341474905b8bbf" },
                { "hi-IN", "7ed67dc7267f5c3014c7914da6828b665ded3ca6474d06ba112c156086aa55f7b7b6cfa28ef38d2bedb480fedacf75e1eedeadb76cedb683ae92910b4710a5d6" },
                { "hr", "e22114570b57d480f023a207cd2119babed362649270fcf0648cc443aff3987a020158c80d4b5b05cd6c5ed48374d3af5ad69f09403c7a19b0aa6071bfe4cf33" },
                { "hsb", "7299be7f04e990c497a029aa55f8ab8a33a2b52ce659284675de33b239a63c5d28ff24571db69f02f021d54f3a972416c7a5ce5f0920cac6dddd8441dcd70b80" },
                { "hu", "10c20b95007ab29fea56e8bb0d46b63be961a382a7c48699bd252cff16e76267d2aba2b88121924f160ed8d06ef273ceda1e4c6a3ec58997e0759ab1dbabe4a4" },
                { "hy-AM", "edea1e0dabf4cbc17440f9a91f7f02ff069d19a343f438277351aaa340271d25c5f0d0f916568f7dce3995b804c433a80d3b7676679522c0465c062fe7a69556" },
                { "ia", "f17960a49decdd1ca9d526be5bfd7f9f2db0c4e0eba4cf0323a44b08fafbf95919ee05b9c61782c483be97d1e41d11278b7ed0c4a2590515c6b812373a681684" },
                { "id", "4daeaafc48f741457be0330f807f4c51a80ca91185fe8e4811880228c799417767445e8702803bdd02ec32cc0db9b49f64046e513a140c49b8db766b96fb0aaa" },
                { "is", "66001e6bb7cbdab71c5c417ec59cc0458d7e0252b85c5a1f5dd4f5d623f690660a9d67955d3d893d32699c1e91b291f5aa0107b59bf94f4ddd5dab1bedec574f" },
                { "it", "84fd3cb626537594f7135b5ece01fc0996c72ae8efb109540c8658ac6639330ceddd55a16586a70b3c90a8c0eb5e4ab010d4b8c0a8d21c83300e97944ce8be44" },
                { "ja", "c89af0e176bcac2a4d8bf67818c6be5476031da41835ab071c5668a9f79549aab68c34542bd881af51f19c79a4d82005e32942573be5369d0cbcf3c9cfd82821" },
                { "ka", "4cb6a99b56e60f4e739271d869877c6dd37b0cca761c1fb01fee632fb5b19f413c48281b608be8a281a6dbddac354d947b746e7542185f2c0f01eef1eed54bf3" },
                { "kab", "02202377d1f06c0c1f21466db5bf53f1475e9465f506813df54339ecc181919ff718e2959f7f54c25a5229e3944491013643f3096521fe52c36659dbc0cb9230" },
                { "kk", "e38324fa1e140549eb7f845e0f2e90745b9347308906abc39e56b60c8992d520dd1e94008552c3e2b913e6c16b76c548c03ce1155bf3ec64d8277b20b4846929" },
                { "km", "e6eebac5cd3f3aced99c4c5bb6f98523a2f2f6d0495962696ffc518dfd1d4f75fb041b210156140352e14f054a6d49540e00a9b87b3c95ad686ab6f9736381c2" },
                { "kn", "8857d0abee1105429eab4fa2e3c09aad932bf641907ca4ce5a43df54363918fa1c4f78d06c68e2747e7e4a508f91979dd598bd5bc97ddae48fbfef516c242563" },
                { "ko", "eaaa9d3c7fd53f7097e7a776f847d27d3ae03291c0353eefd2fba1edab280af7db42df11912dc04e20906a423103634fda95cd10122761d850f156b4623e3f6c" },
                { "lij", "0ed577ab22abb41312182dc2b88b5d76ad1684d956285efa96b2f3cc2a2640b08fa8851899977ecaf462c3eeffcce449f80a2b866cf6ced2d57a5c62273df846" },
                { "lt", "3e25771c338bac6adb3f74ab2388b69de1c63f302f83f018fa0482191a4e56cd9e60fb8f0cd119b1e9b35832d9806954dad0e1753ae533236bd838c5c8b397f6" },
                { "lv", "6d4f5f80d8e41c6b55e544a4e94472d3827532919c6f6dbbb840015cad14e5e2ef09a643e574e71dd56ede2f88d231b2ecdb985be3e412e71e1edd27e1761d1a" },
                { "mk", "fcf2880a2c910716ea1abf92579b405eea524cab9922921baadc6129ee87d7326a68308c3e0d89e970787465a447e9682703b8d33e106f2cf43c93754733ef80" },
                { "mr", "60049b61e088c0990597c915e606dfe8d84d16be0f2868c59454f9fad5256f0b3594a24c77056ba4927e8630695643904e0a6ec39af786d3e4b9b5d974cdf28c" },
                { "ms", "c1d63f75bcc6432e34372198183629e07e7650c78b67308021c47f719a38b52a5326adc7c441dd8f3775f1f05ffcbf9f3bc65c59b4ce4ec71f319793c119c90f" },
                { "my", "3d00f24c1d139e6b9cb6c2cdd4d1e097e170c5f9617cf66e8390403fa217f92dd8f4d50868a559b1d4a2262bc5081983a1c55b58b4870676e92d3ef3bad7f32c" },
                { "nb-NO", "43130f8d8c81e7f411bc5c0b08e5ce0b90f9909d76b1ad72fd19221522c8437d596f0f3faa0cf649a6da5a6c6a14a5564d3ee99b2894022ec3d9a5d5079ccdae" },
                { "ne-NP", "fefb2de55a7095ce4ca884f1cfec46eba9c078529420f7c68b0800d998978896d3d8bf3fb5c12813bfe1aea502f2b93deb696047aebaa21b10f8b04cf85e6e5e" },
                { "nl", "e2bfa544fb7682323ac618b8d921716f2ac05f28e6e4dd7ae473d57c3306d27099895de966aeb2119b6f67f112d40b324ea1e23af1f1775ce4c4713897cbe25e" },
                { "nn-NO", "185860a4bb9a9f0683847de911d44dfb2e41d44fa4389ab8bb676e1127aa555b6be07a05b9ec48e8ba9371ca516faa58fe2a6a56a2783eed2fb4a2e1109336ec" },
                { "oc", "a061b124d28a7bb74ca7059384f0e17533a00f9c4facef3afc45381550b83c3b04534d5914ffc8ba4b5166cb8f52f37daa71811d6c28961e710c1244796357f2" },
                { "pa-IN", "cd0fbb9ecc68efb190f3ebb7d19192312d5c368ca9a1d92f27921d68e867563a9f1ebfa611647a91a82ae7581407bf792158c6fa1d9fba20b242d406382b7d06" },
                { "pl", "40739e4bab8f31d8d2e2eabb24b62634e8a7499fb3ef6e86d0c84aaa6008389cdc8343c2f74d368ba3908d3fa5ec44bee6ea744667b6b2445cf3686940ed7175" },
                { "pt-BR", "70e7e2d3a5ac78ea9d92aceb96629cc79703399fd872734cc023cb3ed0720ca181814aa0e5bdbc1b8db348632e1576d79a41d072afe647d86dc7a172106283f3" },
                { "pt-PT", "960018ff3a123a49cb2e31d719961316a5644fe234d5970567b1ee07bc3958ef5deefbd3cc1d1761737f11624f8153aaced86bdfbc885574a15d9092354e9a63" },
                { "rm", "886cc9098afb6f466fa8d930b2e08c350d2eee9b7fda2fc0bfd851d9c894cd7aa66889ce6f6e9d57e85b63c718cdf6e1c6efa72e0b02f06941e06df0e5858ad0" },
                { "ro", "0041c30aed1953115812c6b744ac0c3dad2e124599b1ae33f1eafe5dba7b9dcd2035680df06cae040c1b44b08f80f516fac978638775719b1008a650cd656b69" },
                { "ru", "a448ffa7fb0f611375a1c8648b721d77c57bf2ecb6f29a1708cd6f8c056b1b2a2b893ceba50ba376543a674d555f4917c563be4a8b0794cc5f908422171bdc18" },
                { "sat", "5866c5950620c8462a220b500e1fca71fd2ce605d03d07a0df53f2f4805edbc89c312065b922f08a9cda44ae7f26993a45e60b65dacfc812621fe92c34ba21b2" },
                { "sc", "2592d789d95168c7aa9d843142aa74c5d8affa525d11228386f485c61676a8698606374aaf96a1c4e83d20187635f5e41e655bf3e62fff34d8112eaa454cf07b" },
                { "sco", "10a837516ee8991960abedb5ce22f374dde0caa2db4020dea32748a809d6e73ac42e63f6d6fc6fc72d8e425ada4dcbc45cbaf414cdd9c54a1535c3692ac5b51a" },
                { "si", "4c13bfa39982143567970e16f443ed1cc3e42b4f5684efbb091e163dec4d68f3ef583505a4819c1d47d9f7e13aefa44ec60cf21228b373aa5c36b7098596c07a" },
                { "sk", "67f8992ebea9dd26162a1fa1323eeab1f7671cf2d8ac86e49226966d4ed94567a22ecbaede6f13a537ac94a27b0a1785fec5962dcad201e0ec2530ad2eaf079c" },
                { "skr", "b2a8a52705792e49eddd066a50b06e9a963cb757d295bdee9e94683c5296c2efd015c4fc1151f50b73edc5babb65180aff5c489e191e612b71fcc4a9c1a13fea" },
                { "sl", "07970bc55483cdd631529a020ef1dc77bdcec76cf320330e026f72668c4e0efcfd78e707ea911fab9d667ec0d5e61a162e56f4e8005fec778f45c729f6872911" },
                { "son", "6c767358fcf47cac5aa11421b4b7ac6be60c0a7cc6af59b2e871e0f00ec7a23bd5431043d9202e72fa6b8c17777ab76a025c8de25fe7db4194b841fd5c65e5e7" },
                { "sq", "883eaa634d9f9464f5bd75ce25ff12cc97cb66b7fd932475402fee07407492a8ae77db8db1e946970b9cc0bf2ed695453326e2c030f056c44610c6276d09efe9" },
                { "sr", "1abf319948022b96b043126599bfec068901060d9b4fe7ccf7efccf3cba3b32eb397fbf2dd5c0f7804b2ca3fbd708a29bc56b03764e328b3c798bf73fd7e88a0" },
                { "sv-SE", "b032da1ae82a16f0f448e99eda28c17a69c82633bb594e63c49a5bd6e41964509cc61ff3eecb2b21b3456cf003aa64ee14fa5cebab4b127ad214f72c3b136b04" },
                { "szl", "79c84c63f3f5aa6cb3b3c8da0c1ac52f672074b4241669e82496ba0d2b321b34d524c73577ebadddba925aab588f3d90f5e96c1807e06784dc447dc0643aed74" },
                { "ta", "1a2f3086fdf3d8ebc01e221870f212ae0d83cbca8784585c622c3ac12c191f38c373c292ed98e4b1a7d3b6434b167754e9e794b42397afc3d1d2d54177c8a353" },
                { "te", "f42f1b442730b8149b6b18e16f58da7c23c84f629e964dad05e6f486f4b129c340057c5cb35c9c584b092a909b2926894354e296999b8714d445c3ac7eb78de4" },
                { "tg", "dcd790b813380aaa09aa730b5516461286b9d84968d395205a544032f9c3626b76f88bc81cc4c376150ebbd317de505145c27b636ed06763a70f61d7b9cba612" },
                { "th", "abdf59f30644636d14a2dd464d2bfe8518378018d016dde0bf080d74446a49a04031b4ccd31b38f5c90868212454656681605be06ad1e609642e74b2170651cd" },
                { "tl", "a333dd9f98423d4dc35cbf07207078105c3934cf732532b7842867b4114371d4e1bc804b0298a0174484c3b32b413343e5345f2d3879a2e3d24768c31ab95c7c" },
                { "tr", "abc108e6b32b9f94e4c654e9cf928f11318ea73908e65679f3d97f4da809f77024fdcd981f47f9152e7014304116a952aa2cb99bcc3d3dbfa9acb4d02c54afbf" },
                { "trs", "6b3ea9e477d534cb9320e6d71737b47520f40ebebc5b2240ac26b2fc05f4818e61b9a92207a39092af0b776b46aaa613fed696be5dcc293846ca54d55838256d" },
                { "uk", "9b22ff9629b737c8e758bbb61ef5e22df4d1c11044f4faa43d9297ae6cbffaf10a87d705fe9293271aca1186af2c29fd647b3e02e551d627f243198af1ec1a0c" },
                { "ur", "87e0e788943a1846758afb21f54684c2a6215349d01f8b112c1bc69f11ebcbe8682c73dcc25bd75a63da122c5adf90e032695cecbb9f40ea40e8af2c87d97daf" },
                { "uz", "6225a303a4e5d83714690dfade756a2c45f5f395269ee8dde1fe39f589c2603a63b5f6ed6e014a66bf580e39039ee48d0a811ad6a71756c10cd646bab091f163" },
                { "vi", "be7598167cddf25e08073b9edfbe432fb8a52b603a001b6c118a686461665cdb13cfaa4735da02a437f96d230f128da553e0bfb796426e20c970ed0bce0e526f" },
                { "xh", "daf87c000a3f5589bb164d85aa9d782222f4be38cca1e54306c5864ec550271a140f185ec00bc3f0ef7ee26e4720cd905931edd278683890a4146876a9ebfd5d" },
                { "zh-CN", "bf321c4fc93b7c098df13a699b114dd7ab0f47dd47145a468417e476373f3bf18e9c50a7a1815afd0a818a3375b2c47e3e73eb8495f09cfd4c59e3fc2ce68261" },
                { "zh-TW", "f15eb4f5c29e9d7576721b5a759926bca60f15714bc43b9bf548e52987c4aa791986d3ee1b9d184925664beff10a8e004ae45f0fb4b42e80ff8e4f3553c6925b" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/137.0b8/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "9071891aceaa04e8c3758150bd538f24accd656c8067fc282f00320de1a3ba847d8413c486d0eeb098222b36ff19a979f7f46a443fe4269704b96f5527396d7c" },
                { "af", "23e197804a60f473fd3bcb8c09b151f9f2e6471b4c0eaf01129964a5a56116eb11440a8d63a21441e40693b296de13748a2efee8c0fe62c903747eaf1f300dfe" },
                { "an", "5c0d843ea1ecb851046eff9657b4364b25e2b5360c2da5a91eefa24bcb1abb11e9dd8e820bce2618ac7c1af9627dad997492e3b90841c1475f2cc706ef27fda7" },
                { "ar", "9898f66cec23c9e86e1dee08ad29910c715cb55814f5867004672eec09725421115b7e8bc16da4c46e2f582c319824fc403851cc6bbd3ee5810e0eff31f11705" },
                { "ast", "344f3ce2694ca6477b5a0e68c1f3616146918d64dc207cb4f5a15bd22452e5c170acfd2cec3b9e1c32cab511fc0dac5ac6867abfe4d48e588829945e3edab325" },
                { "az", "262ad82427fe628288b15aa8e9344cd47ef2797e88c2a369e3dff1e9a6875551d2c4b8362103eb47e8258fa530fb2978aa437f794191ea8676fce6804de226f2" },
                { "be", "4d0aeafe96f345ab1e8be33be3e5dbb8f2daf394714ded9ab3ed79324618a87b6adfff0701f1c4c193ecf04bcb0277e9ec1addbc72fa5344e15099e6c49512ea" },
                { "bg", "7f4ec7b725ee038b1c6024d90ac2d92f2a4f0a0cda3c511a8f1708e9d11d075ac84c3969cc8d6e2a80c57072e580e1c96e12928b57b23e35f6d595b63d842547" },
                { "bn", "acc1f119486d79aaa43af7e65ea675a1abde58fc0c860e2b9ab691ec130f2d3682a3ebb1df73243c6f3c9a93545be91aa0cf45737e39e2967fe04692a1163f8d" },
                { "br", "ee371313e321aa16ebe70ff9b779fec606cb88f8dfea6b0da072cc4928a503134240f7a97e3c7c4af065ff86583e697604c862441015fc2c57dd45c67f522a38" },
                { "bs", "9b345e2b36abe6df6fb48505f7b6ef9ff24deca5f869c7aed109bc9be56b450b9074d5ee3acb835483693cfcb3b00aab7b01974154dd16943cc4f58786028a48" },
                { "ca", "a2d6ec69ad2e69ee2773095a042b60a6684877d1a950184bf77a9787c7e70e30d2afebb57be75345d4b6b1ac90f864a2b912f7bd676d0131597bc1a699bf8fa9" },
                { "cak", "dafe82028ffb18cff48176824180d2906024b75c4875778b008740d44ef8648e2c828cf925dfb0b8a23eea0f7ef4af606e845c3dfd4c901fbde4aa9173a93a28" },
                { "cs", "27bf5e897c58ec78995d64aaa7e79c5a208befb633af2c7c73a27e0ebfb169a9b16a73917763b36c177738323d6aaa8a59d2f44255358bc27eb89ecf564f3d1e" },
                { "cy", "b48e10c94df705fb26aab4d0fae9b93390d6d35b67715535e786288573f7474e54de561a4a893a7a7668419a9a8c237fd3339e009c4dcd29cc66ccfb81d7e678" },
                { "da", "87375b9c432621d46e8412b6cd611b8ae0b86dee84d7a8872802519be682b28a2a8556433a231455d28701e861cc398e6a7e0fe4b5ee5988278c7a2361e92699" },
                { "de", "93fa00fbe696db255bac534d0f313448622f8f421176dd22e3c9b490b72fc77ebc1fa11f603008884d6ec791a85cd3ca6541f68246f4523a69080ee402e08b4d" },
                { "dsb", "1e6502257507bf39bf6c0b14a4b7e2fc7122f658bff01d3e4a2e1cefa26b4cb2a072ef846ea314a59067a89f97dd9973e8c027109bb0f3d57ca1de62373fc25a" },
                { "el", "440dc50a04681cccb101ff463fcff549fd3fd06bacf1019cc9c390c3a186ff2a1bb73b50085cf84b7fe1fd0eb3f24dc0fb23894d7b8d38d2eee9cc64a39f886e" },
                { "en-CA", "e7d96758e35109d4f2688647e8ea9062673c4050487c9af5c546fcb675fd45a63ab77172bf498a312259092c3d22eca2434cc57990325b9b627786146c7fe09d" },
                { "en-GB", "99bfbfcc57a71937cc900a0101b9e64f21ee0dfc4052cb8a977d9b9f4d33a663c35366ad704000b5b4f01cfc672d0c34faeed812c3c0a7f6894312ad176f2ea8" },
                { "en-US", "44520a8bbe52d9f0eced441617b2cdc6a0ccafa0e48bf8595178f1eae0657cb23234ca99bab245702c66f5dc8e87ed63c001bd0403a6e9199087f523e285924a" },
                { "eo", "48eef20654ec3cb87a02a32ea407adcb5a2ea18f4d21379238bf8612ba32aa97dbce115f437156e765db16d30ac6bd6aa7b8a51ba3a65e9dca0648ba432620bf" },
                { "es-AR", "3f6a974f7e116402a800f9cb5a6b9da864cbcf4c4eefd104854c8f1145efc03f030fdf05da85e220a1e39738ecdd66de7f893abe31a20d29072376d13df999cf" },
                { "es-CL", "6182b19adc58206ba8f75a7c616d4a0ce6c8d375b05810fa034eeb83ac71f56425f153ea17a524742f196eba94cb600e0b2307ab29439cddcea541a22b8de173" },
                { "es-ES", "97d9cd5b9b6358de10ed2fe8b3fde588b8d7b1185038a15d41b37670ce17dfac04d3b4943a2f97740658ec16633720c0edc309ddcfc88e30c7bc54caedc0e2d3" },
                { "es-MX", "4812e60c05fc04163a540bb467147869d3238f69e059b7184d4713ba3b6f2a36d021e09fa910b8b1d08d9dd2ba9e8e9ae3ae2dacbcf96709bf40d18584bf8eac" },
                { "et", "00340190019b6e48114b9a0dab4cc28dd51e855d3bd912166ea3083e994dcdd679d76be8b357ea37024e6a973b0dc43cc3c38c91167fd17bd4c5990464d0a273" },
                { "eu", "964554940140fc435e4c76bcd0ca9b9377e4aa1bdf2196cbdd3fd11f48a3839345c0e080791dbe8dbe57f4a117a39aa3f33550a77554dda3f795f5bcd0a53a7f" },
                { "fa", "e46de22cf7e011d116e5b747682ba2968e675c07b5f065cbbd0c4bf47f90586d59922cbecb51275f70748b787eab7293e165ffa08c5b986fa8d25ec87b67b85f" },
                { "ff", "cec24df3551b457160bbb088d1713592ebcd00c35349485ba365e8c6ac2dc78b46a6c60b77260241cd294a92ebb0bdd60154548121c6fa3f26a1b035973c09a1" },
                { "fi", "b7d7d2db5ccd6bcb8b1317c2866b2c671ff6c660f28b50172c819aba990a32896b04ba0f1c16f2364414c22ea408facdfa1244109af325a0f31513f1b65e29a5" },
                { "fr", "cb379bddc7f7fc4869a221656bcdf2a7576bd287cfbbc80ea7b16327a695f587b323e3e203440769f7d01f4b1df7625e30dcd012aa6aca2c235e897b2b938a53" },
                { "fur", "3723882c04cdfb90e8a5c12d5f228cd425ac468ba1de3830c2737f45017b110c5b81242eefdd95a47e90c1c862b1ad7a80b1d9d72f6d050897e0e031d31a1e3b" },
                { "fy-NL", "abf99cc1f1f26e1d1349ab42d6a21fa2502b538719e771c870a03e1b9ab233bc043818f3de9b4b9aec05767e96bae7e1284f278d465b56abaa741ff0dfe68266" },
                { "ga-IE", "e61057364cb78dfda5274fddbe34e019f061c4d7bf506fd7af7feac616dc5201479d2e9e74806e2b85bbb32a6ef20a170380f6bd685520a499c6ce3072a6b736" },
                { "gd", "6872d1954498410b47a4cc63c42b04363d722f40ca9d24dddaf5a8466b9f36db8acaf5cd2f78a751abacdbef252783f7880f12a9e68e2852b4b4287071450623" },
                { "gl", "46cafcf26d2e44af9e31b5dc76ff0271b9cf91527e5b5759a1c7a541f13655b665bb414b9e6bfa734d728c603965728b1666e43592fec3c7ea14a06b5b8a02ed" },
                { "gn", "7130847c6802aba30f4511effee6b4c277486d552d93fe8d65d2207661eab36bd0b720a0d972ecec64296fb66937d38f1046cf6559b56c9217ab71d34b6d261f" },
                { "gu-IN", "5b215e83dcb533f0de27a0db4ed19876cadc3c0f7fbdfb23cb29fcc3e53c845a6e69a54ccc7133d38cf94384c17c5e94a2cf755acdb3e2b5177003a64eb77676" },
                { "he", "a2f976ee566d9e6d45c7e90cee02536f6deb49ef820dcc2962cd08243847a4f946bf829b5bbc3ba569136909fa12fbed49bef3cf914036cdac1ba6fbc4be5f4d" },
                { "hi-IN", "633faeb804fbba44fd89672b5d3cf0bee19344dd1d8d39b1f4edb481fd1e5010603ec576d53cc236700b5e2835cc4db6b994ade30bbe2d73f5b35d287fbbf8f4" },
                { "hr", "564cec5d77f3326727a896f7f821683ab1984c7071429c30e04ccb13b7d1a50a4cf01edeba0bcd61782cb211a08fd3f2a122501fa2da9c73897c274b5384cf3f" },
                { "hsb", "d751d3a8b933eadd1644c7cecd0136b1f18bf252e46687bda9fd594ee6bd5d17fbe6d2e079d0b9ca04ff9db70eb016738b73d40cf5cb41f24a96abdb1fa77ca0" },
                { "hu", "2df4840dcfc01b2dc2470ff1871dabcdb8365fd72bdce08803654161b5976005981d3a0c374b1a8b8282e5f9d7f810210dde27695ff5c57e2b7f5f496809c392" },
                { "hy-AM", "877f98870a8bfebcffc01fca0d7c7d3a3bf2234c7ce607345fcd03c8b08cf69153f780f39987f1e44e68d20b5d5caa23ddaa82f59546b2387188e89412ff2d7f" },
                { "ia", "ac0909ccc21ba66c4e83d53381fc97eaf0f7444934d20626d4e8c7e16083eb8a6fecc40b33b14e37292871949e96e99b270f56ce0994824a9511d02a6ba52a1d" },
                { "id", "4d800fb5ed211dd7e61a0d18c4bcb8457c5b88253b139cc5e031e43c4e16165f9da27211807c3d4c1feae7ffcb7a7febf495f7b4f146321141f2fb8a636a154b" },
                { "is", "121ded4b8a2cb5635cda29d7237c3fb548778f9c8f68b29e88c44050b1d084decbb5fa7073d9fb2aa64ce4c5dd41fe37a6bb0eb29643f659c856121d52e96aa6" },
                { "it", "40357591899af703963259d4945bbca9c2d784971f25bf26b338597b5205d17a6e5a1d4bfbe13e91b4637892fdcfc972dfeb8b45e9940762d5b42fa444b298eb" },
                { "ja", "390d7ef5958921c3d1087a472a34306259f31b60d88fe838dbccd6fc42c69556e9c1c1382eff5ac5a6aa5c69d3ea929fc50935ade2bd927a2b328f0e42531de3" },
                { "ka", "d725467e6bca7bbaed9b807fed51f5e74db83f957f1943de4991a7907d1351f071e8b7186e987d68c087df485a333337a983ca5c1e12ba03044735c71aee8f57" },
                { "kab", "a5e1feaa2c31068131d5fe1c8aa9346223d58698b001f2d5d3a7badc14447be83c5614f6fb81382e5a8629a4bf736b1639d5857156b72b11321ead10407f0152" },
                { "kk", "da0de6f3ca99dd3e60e4a30e0d9eda1c989b7c4f741c6133b4c958bae78e2f3b08a943967c7c314d1db4808f4b3c1b3f04bd952c97633801eb1de00e84da64de" },
                { "km", "630009c35329ec786aed4be59c6fe3885e222aff6c7fbc3fed126134a308345956929a127107a3bc33bebfd5352e9068d62c991632d8ad9050f6ba12f8129b79" },
                { "kn", "af38d2b87d006ad3040925b5cf62977733a32cafbfca267c6fe919535c76f6e875c5e8e62bec861608a23bd6693a4fbdd99f8a1f73b1a3422f9cb301640c0b47" },
                { "ko", "34fde38f95cc60e17cc4afb21ead39edfd003a9779fb535f706ef5ebd9ff27947cd438a2e65efc378dc1a34b6c586f80176e522882ad758121e8c285373a1ec6" },
                { "lij", "d6afd751f0f715454ca7cb17e0e06983a54f7257dd68b4c604e71339f90f0fabb899ef747a2cbab20ae6132d7cb01283a7c432ed0e11f6298b7a3996deb55ef7" },
                { "lt", "51c173bf25474f2c91d834dc582d8cc196c3eef451e1c5b70f708bb4a9cf4e3224b123437505d9fa8b2b1e7a8efd26005a416e80613517f347e1de1ed2820f58" },
                { "lv", "e662bcdb7b640de89fd6f392d74c8e2403114e09c0684f6d32d62ebba2e1e241eec5ad3dc97c4ee249fdac067db3bc04c2a074b909e293afaff778a4c9912583" },
                { "mk", "78338750bed4a95c670c172b28c4246a4f3e5fc4ecf3d08246547735c80945e3fe40f124e8865d375cc4d3f79c66bf4f9064b474c5127127c27d3f924cf4fb7c" },
                { "mr", "275a02bc32c8a04d8cb243339aefab3e73979e299df298c45707932114087143a322b1619c508c66e4868f8e1ffa933e8612d0f2ac066c6036395e33aefa181a" },
                { "ms", "a16e56a069c9fd332734050d77b3115e161bd7eb916bf6675e8a94b5f542d545b05022ef3a04e54c79808bf9ffc7d2357559e0c43f778f0e6e2ccdd351ba67df" },
                { "my", "00bc13295c0764ca6a6cc749cd70cfd27d153c1399ff5565b8009db80f9cc6c3f1097ea23d8b3b9e3c0237ec4bcd6350b1c475b5b150721c8d990903f2829ca3" },
                { "nb-NO", "d4fee1a105ffeae152f22a700cc122bd27447b5968bfeee63fcb12d796c33560c2854b09cfd6e01971fc722def4f0e56c6a19fafd1c162b7649e760c8675c3a0" },
                { "ne-NP", "5d27f4dd9337f8b8b378f68cabfc775eb9294b94ec6cbf16d3477bbba0c06e704e1adc995a334b351dd72ffdd1019ff6bee83650ee6c96ea5a0585cc55af2c2f" },
                { "nl", "ac36964b6908b2a69c5e4bb54b4a859e50f932bb85366978b434531d191fa907387f6e7aa11548fecb5151c0ca0f12b74e994f18c1a1443b55116cad7786c820" },
                { "nn-NO", "460bebc05cf55d2dc4c102dc15fb1c8595ac9a741c5589442040335876da59655120f460fa460447917971161b1d4304c64412a616a8b5ac6dc50c484bbafffd" },
                { "oc", "8c47091257d281b57c5c29b125231b0e053d05edd094d7040ae49d701973099754a320d123a1a40374372ee024e58d7208f0d3dd952d68c043dcd5628c853d22" },
                { "pa-IN", "c01fbdf2015f44dc22adf6397a4f356cf3bc081befe99a42bdc26d2cb13db2edaf2c2ffcb37da19ae95fd1e46a5a378ca6e0122914979e19e216ff18e960c702" },
                { "pl", "8baac35bdae4bd64306420a10dbabce233e9993d72008ce8ce8e4fb0a85cc55f8be63985bae4da5e03a702e4fa7f31f369decde4e9b6d538d6a9343dcd4b2129" },
                { "pt-BR", "6d777b7c038c123356bbc9e9f6fd1010004c1de6985ccd8fc2bf952d69fc462fa4ec5757aba2d6057c67fbc6dcbe02899c9b21e43f9316a0aa8fb9f9f3bbae38" },
                { "pt-PT", "d6fd2a60d078b420f3da66c173f61a4e57fab5c0d054d495b51bd3f4b7cd7961187e4d97bbf6416eb391f3f1c04fa2216a3ab765a2effd12ff7baa4a74d37133" },
                { "rm", "6f797cbfea9b751e3d241785465bf72df266252a69296ad80a13ed5a73d03b39ac1ec28626e4d7956519a9086883662816b4955109f89d2a264e153274e5cb0f" },
                { "ro", "a2bd697fcff2a3f0c54114b355cc4c7a0215c9b1ea3415913746c7ab6ec5d5de22485ffe036131f32e6ae70d220e8aea4d1225ce19668388232c9fd58d55e560" },
                { "ru", "330814eb638f0f4e3b47e292ff292a0dd317cfc437c2eb995a85590bb6c4656184fae4911fb6eb3b6720ca17111629c911992da84255b961593f5b3b9152b20f" },
                { "sat", "7e3d4752abe851f405179eb20c9759b4988f14647717663bdd3a66d1fea674fcf51b27b185167711cdcca0cac2218d075d8c8043e61d2cda80fde045e78ff273" },
                { "sc", "6c02455aaedcb16b50f562b1117997a6f19dfa47c5ec14305c8a4b69158f42a50d2fc8f0e4e0182b4a25739d150efdea334cded1602d4de910b7d3c940c70389" },
                { "sco", "7a2ceb49faadbca526f0ddbc93d09cb85b49a87f042fe49f8aa55f782c52566180e331ac4229c59414ed44664f07184474e7cad80b48f626a46fe247ce7be729" },
                { "si", "f6527b2148e5af6dffe0ecfb050f2b989f01539036f4bbff4ab3a1506ca9125af7548f497f5aeddb11fd96c85f31675544d26bdfb42d58c3203163e5e73cda6a" },
                { "sk", "509de4a8736dbb9dd4aa32759aa862c359be487662c0d7a11d5459dbbdd740c9b3d120c54b485641a50c1a39390d05bf573dddd673a8b5ada568e2a1b8a24b67" },
                { "skr", "9c0120ed94cd4a3a92c89940d37de6f95c039135098cb6bbabd084f50bbc546017cb2c334ba8f28a2b1d58508c3ccd9bbf3440f4014b4117bddb50f28643d18e" },
                { "sl", "9ac3e6b27dc8f598c73ac515d71e7aa298e4d8c2bf406d7b3d889a811923749b905d7c37c6d9b4a94266a8e220cb1c9b1ff672c9c381f67469b52b48abc1baec" },
                { "son", "82127520a1116108f8ce80f31aaa8f56a430dfd9d6e482248f17e1c1306ee1494b20f84e0cc90fa749ad7052d19c345f9ba6231e62d02caa98debf7ac18495a2" },
                { "sq", "418db0aabd70deecbf77a518beb1406bf240ea0669feb8b78aad20fac9b03efb497f4a2f482dc8573bce5bf66ed8b106e432907c9ece1fdd89102587db97a830" },
                { "sr", "44ed47a326e2eba0aef6b4cf0801e9e7c4a16822913ada4e8bf09874bfce8dc76dc8f0c1233fe28aa7ef2c6cf346caaed47b7ac9e85dc24e8488e915d4799bed" },
                { "sv-SE", "5752528c3b2b6e1d3718c11e4037c1e618dc129c45859ca9ea3760b4bfc52301d04d5093cccd69cff9093097da9a68e89da68db2141d239cdfb5792480d7da0c" },
                { "szl", "7d3373f87b90746db54f39c5c8ca8cff649f73053885ae0d8cb5ec5c36bf0066f2a6d6d11865a1a50963a508fab9721bccd491bc6040532da497fafb91a5b3ae" },
                { "ta", "3e028d0421a9cea0db65c8e1038318673ee086398a88d6c7d583e62d718f177ff7b232ed630be475687266b9f625cdfa2a515c0b3409cab72009cacf09a67b94" },
                { "te", "95526b590166a5a78cc33778a6e42f3075000f3a1fb83f82bec0c133849ec1383b9850b398fe2839aa04baa0dc09d5a4a42aa98244ac549469ccd74f93fe0f09" },
                { "tg", "7eed5a7ea3a5dfa78a3f5e6a8a4dc5cc66aa0df548f7498cb5dee26168469779f22588e51f522f1071435b97ac4a2072a8a34792253ba7679791171fd5f89c19" },
                { "th", "bf3dfc387f84491a71537763ab583a41a6eb5aee97e24d9ba026abf86090884dfc7fe66140aa42f9087d35ce5ae1b0af949215b09ef8c34d0082d17cb43c23e8" },
                { "tl", "4ec45552b37c54e6f0c614fb189100d84833f6e47bfb46f83e43c5ee73451b62fa2666861f9420cac6c118fb30ed8893a38e11cb032b3242f7a4cd56dea1936c" },
                { "tr", "8fbf78033b24df37b85529381c4ec9a4b330a46ec1caab04509681aa2a8bd79101040cde57d709e9a9041ca48cef9b29d6a78ad6e162b8e78df1744ff37a1a96" },
                { "trs", "2742f853de8a495fbc0473e764d6709f83a265252c123797671b2e3bf391c3a4d09493f011f53f1420defedc799ad9d65b9b60175e2606999bfe7daedd977440" },
                { "uk", "887aae97ea8290885ea1af2d4d1726a232e5a79bacad3300e7552e06da36f9fce6445a4c1211f8b35b41dcead4eb2fcab9c52ddb8baa7c14262dcffa16af3e9f" },
                { "ur", "b7f3afc7011537fd2c23d86078080122d08bebf00f487e0bd088efc681bf200da84ab24f14cc1e4619675ef258e3fd7310b053d96d824f6da9206d38ddd78c5d" },
                { "uz", "2a4e7747e87efd299c6ed070ebaed2ea77246e2e83d08297a6266cfeba238f4c74ca38563a09b8468bb0334fb8b501b3637bb0a272a65d51934ae2c3516d899b" },
                { "vi", "1e78855ead02bb2e3858b990bebeb43f489e239d3803f5a369070018c73452c0d09bae2a7aa99ce0f99d420a2c28cbeec3e53b6ec07935acc766ef584000f9c6" },
                { "xh", "113bf81076e6abad16457524d56abf57225e044df6c6eb24f7c2b441930ea05ef103fdc152a40d40cafa224b1eff586cd64dd57f03e3762681f5b7e472315a65" },
                { "zh-CN", "a0bc15555a24405399a56c91fe9c850e10e2f2710153a7b5e2811431666f926be6831640c32391df3e89e8ba74976c82b216d63861f42dc2b61a4d0a2be345e9" },
                { "zh-TW", "b4e8dce45b991dd98e3b357533d139d3e4ecaa1d11864559f55ed9871acad39ff67f45309a9b084960d6f0c184ed802f2d00e39fd640aadc5724d631c5d92f16" }
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
