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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
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
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.6.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "bba22e1e82fb5a25ec0279563f4ed0dfdaa741520d311ef7020dcb6acd3733cb71b53ac4c04b9a6502a9ba12c3b786813acd7885bf40eb1052fbfd5e17396d9f" },
                { "ar", "756ecf2027b4ae80d30e083e61bc75d1a1a5aab8a80e1f3132b2fa7f3c7f4d3728453404ee0cf98a0e39adaa856d3f2a4c70276b8c0e64386d09482aeedf6857" },
                { "ast", "f511d3040bad73c0b6a3cc525bd619c48e45f239b184f520b84f72ecf24683c921bd71eee1d72be455ff5b5b623816b0b7a103416f7fafeb3e10cb68c5b7fd28" },
                { "be", "e752721491d160d9dd264bc69ba2f8776ccd7a27ed4485014a572c13b52f62182bbc1a4685032e12a9df664adfa51e53ae7748aab62c99f8a399aef9d4bb4c4f" },
                { "bg", "5a0b9f2f9de24315d1dd419785572e292d26c6fb4979431001742bf91e461f413b5884b668618d5ab8f097cc150bc9857c863db5d016119c25af3c7bb583de0d" },
                { "br", "711f85030518343072745daa3530cb9ab2bab23719427e8377c625462af8b29ef4be2c0175c264ddaa7c89d01cedb642611f65d8d8b9a7002f5b6d81a4e8962d" },
                { "ca", "e3e4531ab1c6fd3a5753f3ad2bf485d03590ca61121796202d36f3f6190c1bda6df4ceade3237868cc87e4283daa77b6280dda141cfaac55333d148636820ed1" },
                { "cak", "c2c6155b8b20e8761a82a363a427ae3e3d6e3af6954671d51968c6c78fb2ccb5458a7008471e11ff97193b0881ce01c7a10a071ec845486f8596ecd60b65f386" },
                { "cs", "6bfedd6c77c6facd39ee541433c17ea96852636bff96ab57229d3ba219ed8ec9ea3d5c269266c69dd90f566b660c7eb32ab060448cf4651216689b1c34c6c910" },
                { "cy", "020430bd14a6c08446159250d6c10654900610b519316729455d6c22af12814d14ea5c92b93a20bc4081de4c3dc7f558a736812b7b363bb077df3f584f6d05cd" },
                { "da", "039b271450c5f092c461ab836032c8c8508f8b23b985f0465efcf27723b7d04dab39a42d78b477a8816326d401b215a60fcf5ec2e3f2a27af4c9ae43e7d6cfbb" },
                { "de", "8a975644fb5b84308a3f1eab4533e8ac9ec99f0956cd162c00e7a6e3e6ec460bcd7e6d79a2bce4515783a65f8015f7003fe18d8c0499c65ef5e8f51d2121cf9b" },
                { "dsb", "23209a3be3a300f4a76783b79e1bc86842c9115bd5cda668fc868396d637cc0f580256a97490950a72e2d196597e887e34de3b383d709c52d4a9e7f12522c97b" },
                { "el", "53c4c6a95f484a3b537cf49c26dd792131f12f77a1066856468ad3e2aa5d28a4e2d4337edc5f56cee3090aeb130583abbde728c675518d4de17aed47f98798fd" },
                { "en-CA", "47fa8a6d8f1f551ba0587860a35d4361c3b0c150873190af6004d6e5781d21df88e53dba26ee3028d8a3eb0c78b00df5b920e86fc28e0b8c4aad4fe3afef9806" },
                { "en-GB", "a4c7f12ac9756f9289d6cf9490298068d6e7074772474ff633bd10e3f2a1a14677d91ab1d634fccc8efadb5abc31a3b0acc6c5d3f98afa4766adee5e3a436906" },
                { "en-US", "bc672239a050aeb96b20ba89ac39f63dd9df93a854d22bcfe6c002b1ff15fa80c3325d470c84d7f593972b9697711829d3ac3d1682e203412237a815cec40f66" },
                { "es-AR", "057d3a30493d83643782279618d7b7b54676d50db3a5276ac115b55e88f7b285a0866a54feec8e7882be32159d9809e7d879e8ec4dbfa7dfc9ea5bcfce0c2a73" },
                { "es-ES", "7d0477e5f1b96e22f1c86e58f009b9cfde09df14729f91d0285c19228c3b185b74d6795d6d1880ac79a1fe33667ab13e07d0480289f60f678054ab81107b0480" },
                { "es-MX", "cb3f65bc6c06e447af60560377918f3f5cf73304ee54d9daecaee9b1174c9a3194b8c21fb265e29261ffab2029d191c7b15657020a79af9291401225cf52782c" },
                { "et", "c18d4d57880c5d6c181b32ee0614f0092086026be15c5866e43731c5f9d196f5c2d4e9e8b14c6f8fc096d89b40a39b2891233b6d7707c1d8e6f1a2bfbd9d4d6a" },
                { "eu", "ba067b873da160b8b9dffb39187652e6dafb5debdcd1be2abb9ee40b8fd2c6facce9bac689de7655f37c2177af10650032c5224dd0c71ca78cbb4ed8c2f9686f" },
                { "fi", "1711c65ca000887ba83485a0095391c8849a24b0d2136b9cfaf0b4839c606fecaaa7f1f77f80fd9f061e82474cb3719b181a1b9363b71a0ffc484ae9a4bf796c" },
                { "fr", "e83240440b1cc285eca7775d1961f41cd7671cd19021198baefdfcf264b20fa8d62d04e849127dfdc49e731237fde7e8d31a71d435000e25aaab1e6d9c6a02da" },
                { "fy-NL", "4c93758c4252fe7b28e9bd90949bc33df3d2ca61de4bd535a8c9dd2c7673d700aae48c957e0c28c86a40bf5df04b25a1b1c9ce471a94364c689c290f5994300e" },
                { "ga-IE", "32969de46ffb24fe48d3934ec2a7acae72d36bbecf251d6d4942766c5d65527b467754fdfaed2872872ca5715f427e64afb5f3af099f46ee4c8e178ce1b332dc" },
                { "gd", "703e136aa42648ee642d2b4e882b7a631e678bf9b30036ded26df55f3cdc27c8e7f47478e09e4c6b30cfadd17278e8bca306b32e0cfc2311b5c2fe90c592f876" },
                { "gl", "bd20b75a3223089a38ed23e06b57f399ee7fb91836125c93305969b2e45ff5951afa68c4c0131d1fe98078cff6ac72ecda5d622169c12e9364f8c1c101acf279" },
                { "he", "9b62645d0137adbb52355df764f991e1cff219495488beae8be6f11b03404b0c1eb66f17c6396381dc21f61cf413edd5b2af911b9b1009b2a6da08b2507b96f8" },
                { "hr", "163474c3a79fc72dd137fd74312a0cf7cb3d1d346951fac0ef431c543668d85a35d74d761695ced663c7734bad45813f2e039df9be9386bfa415a479f31077d4" },
                { "hsb", "65913a7a21c0a46a0fc7c995baa211b8c045a0bf4c192be8357118a7c66e2d6e4031419f4b9bfb881b1ee2ae1d7584bbbbcac127143c00854bf3051ae8981e8d" },
                { "hu", "f6af80385a5398e861e52fd65bdff561a11f4ea9c63f4d714fdffc372bbf5a40aec32c0be8c3c100addc9fbf85de9708548d10730f694bf72eaf23f253f7d1cd" },
                { "hy-AM", "a9867aba9d26c79782e91924095e822e336b90a87a80238d0291ad9448d53041b182b641c0bc74740e116add745090580cbda527d15bf9b9905b77b15dddb45a" },
                { "id", "45e4e331bf0c0624aa7539d8bdaf97f807901add0754f3861fcd5004d24c9899ff44318c7e9ce0d03520e144e62c9e5f6ee6fc24cbad97d8f588da2c5a262826" },
                { "is", "97ba62ed6352f23a445430072b68bfd75ccbea4e6b7849b5f697bf1ea2d2073622ec909d4e2a11fe27376502ed92987b9835c4d2c6eea9c4dbd3b4349928ee74" },
                { "it", "99a9bafd54fe8b99dd22adc63c86f7729682a2c4d18af0202d6f39e3f0c3dc5960d9a9999d9f3f264931b229c0f91f59cc9402e8f4535a047f6427ce32123d95" },
                { "ja", "5963b15df52aea77816f1d18545fb59ae21a46da28203be19c95fa11568f2a6aca2ab68fe2e074429c62e11981ce669a7110147a7c59b059518e7ee5f9399f57" },
                { "ka", "3430813bd2582ad9cfb3bd435f463b7f24c5eb6db5592f806e6d1c9c4d6aad17a97e34dec2260579c46d4f0157395f6eeb8db26fa3c397f3bd02c4055af71612" },
                { "kab", "9b627b094b5cb850d900854363c2a91ff593e73d72f1d85346f0a26ae5e57eb5d70710d315ab2e92c0bbff6b7a22b3cda555d18033cafba182fc2b1ef6843b88" },
                { "kk", "e5b527368e38ca615716fd216e01e309dbace28018ac8f90aae57a64e60a31cc59cb5fb4915c28809c9a726eff01fbec94f29c15660539be173c3cc953e4d9f4" },
                { "ko", "ac4407f948310957a211d044187bc2ad8051248435f925e8b72f85fcac9f83144683e6d4151f022809e6a0244767a2d33d9c8b449075cc9cedc8e4cc0bba056e" },
                { "lt", "8d5cfb055ea7bbf8942e50588da89c82f1edd74c16d0f5f3cc92fffa3a391ae8b0a749c34ff443a17aca23e96a8dfe4fce294bdea128488b852a0bbb1ce19aee" },
                { "lv", "ad3ef48c2426038b7b082fea04d76eba3080bb80214028989c22f82ed70825830fa1d68849f7f5bb484dea23fb0b8c6d902abf44bc000a8ccd3e5879e0b9b483" },
                { "ms", "6dfd51539fec53d6a196338989f0fae1c5ce7edb8ce713df6a043438ba84b0231a16ec2039714b9ff7fb947758d358cf94a1a44dc8e968c664ba4c2ec647c348" },
                { "nb-NO", "41386917ee9e483d98e3a35eb74ba906e5295c31262da811cbd1c3eb88c23f08af267c4a74750837049ad52aeb9d8d4e8b093838dcadca7be2d9a0ac8dd3f2fb" },
                { "nl", "ee657f17b5e26f80a5ac9cfe0f0c75eb1a3217d48fd7febb5e2daac5367da6b8cbedf4f559044a55b5717ca19d64ddc15158c158a4f18148a1627a1cd336b0b8" },
                { "nn-NO", "d739be26ca8977ae73bf58a45a770703527ebc413bfadbd6a35789ddd1a6dd388d18f0ad159eeac8d3f85b6ea5dc2b486cc0d409d7112aca31ecf76c8f200912" },
                { "pa-IN", "a6c43a7e4347b8c851d8d0fa88a0f5507f30d861549376b7b350f8302a91b0e59df3af2b303d076500dc7707f023ddf9b075d87df996ddf2575298e6cabcbe83" },
                { "pl", "7ad79de76b7df01342c4783d31b4af9405904d2bfbec856a0382ffee51ef50485e827cb1e843981bbeea1b2449aa0fc3e4ebe1ecccc9e51e10a0b087e127fde4" },
                { "pt-BR", "20cb90efa483465d040998c4a860cdd029c06ae7bb35c6ccfc722cb6e7b974563a8ff3ace9c8f5018cd68783a568ee7e8c033e67c201bb0e9b24dfb2101e9db1" },
                { "pt-PT", "f58a35aa4ce5ea4e2389cfb997807f23a8681d03356079027d491d29237424ce6358e79a11e11c44350d9b574427b359cbe3dc46b5905e202b9f8e79ea51d4f7" },
                { "rm", "c2b6ea12303983cd5610b0dcec553d79a6737d646a73dfa27896f95c12dcf52c119190de84c756aecaef5e90243ad1cdf6ad4625e35a6e08822723c30a0ed6ff" },
                { "ro", "fc7da6a39780bebed0471c943d9fb309e525c24131073ba66e7501c96d5ce01bb05ed9318a9e43b66c2cabb3eddf7a7573cf2b859364e6ee229d9d165bce3604" },
                { "ru", "a2d10f5f6c6b67a06cd78452ec78410766fb86fdb50798d78cc5305dff4a59f7cd040d84e2e6c14d802677d0a8ed1651059c89b07dd0f21be171b7d78fe37842" },
                { "sk", "591c33cdd0d3bfae86d67e3b0482a9892533a7cc635e1d9c447c9b3a7bbf74b7f227939ec40f7e31f4a7e2cf90080c5a3b66c9e47e7bb632fe1cdf5f2a33826a" },
                { "sl", "07ded8762d674fd1bee9113254ea4d417793b1f94cbcc8ec2862abe248feca23fa99ce8f82ae250270fd06aca8541ca969de8a846e787d8ce67dbd255b978d6a" },
                { "sq", "56c425487765c40fd14f2bde16d9339689408434edf3d20cb253653c9429e885785f97b2bc7c97564ab86a4c016c25ed9aea9214f6b62d92265939282a03e934" },
                { "sr", "ea8e4a3d2d6b72081a8f72656f36c09931025c9ee5c0cfa34b95d36ff6375e2af9687c62dc9d7295373ceb820677f5121a512c27c25d1944865397f098d2b982" },
                { "sv-SE", "67d99aae0aff7a3890ddddba61542c17fe5b24f0c01b73e8f011a7301d50d6f4c2b0ac176ff5e6726a914199b5f7e2ebe677b68360ae671006c3e27d2d0e8384" },
                { "th", "5400676d0c2618137ebc346b7697de5981502b6be4972a0ef282c91eee551a8b236937aafb13a11791dd6c06cab2d386251022899db25c40813a256d1dbce766" },
                { "tr", "0028a47ba1f526c0a9fc70c2be8ec46c91d9dcdbd809297a74bd13628629a8b2538d351112f38a3096aa867c9b1fb6f5b7db5536140d592a3ed7504c21abbec6" },
                { "uk", "a4f969cecbb41abc4bf8344bf4c5d0f581c653229fba047c33f2da1ed1604f7a8709774a6178c777b31be39a49a5b8f38b3357474c40c20ccd686fb80eef500d" },
                { "uz", "48fea35a0fb936889b0c36273e4764a019d90cde3271ff0effb4883ca27f335d41298c859571f7ce1df43419b0383133a2d0fff63f49b8a3ff7807339d07b162" },
                { "vi", "04fa679c2f8afebb14cee264b67e5e87c4983cd3dec2cb1ea8a7e4534daa68ce3f2d8df04c78a70402d2467962ec2e783178b5a48ddeb16b7cf1afbfffd9880f" },
                { "zh-CN", "ebff6decf1064a9de4fed05a49bec64fa77c41766c1467e31aa0a0fb56225afd40c3f3c789c3fd601094316260e632c42f07bbb25a3dd2a68af38a32f38ca486" },
                { "zh-TW", "5afadaba813ca37c051cab4cdab33b755e7f672cc9e63fc6cbdfa210d7aafa084ae64b45064cf4a314824340ce465a2ce8626109fe8ed1024832462220e26442" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.6.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "d30ae4b4e91936b5f1155388d7e8660fab65d3d4128c5f23f878b5b84d0fe0cf1e6145efbc4dbb959b203d10090572b32c6c299ef342c3af0d45f3e263718c7f" },
                { "ar", "6dfb28bbb07df3064faefb1a7ccd42bbc5e8bee360a502d80b000de9a1904d3b34060213ef675bc81fe236410bd324b40f026bababf7663c8b418402cd9cc19d" },
                { "ast", "babc797f67a8b9fa1d9e502126a998eb4abfd6172ada613da244677101e58ca8ce0bdabeb44aa2390d9beab5def7e05e84d663975600fec368745369030897f6" },
                { "be", "f5ceb91b9945b5eae896c5a7aa4c33fe01177c256b86614ed08d8ad49146725a87f7e28b843a78f5a3ef25060d5fa5030e1f49541d00fd9c8c8e84476be8d79b" },
                { "bg", "34e1c26c9203c933447e3a55c966fd6785dabf053e766ba1fb95483433d83d9f52ef010c56299ce9e8e05b650d5b3ecbdfe805fba8a52b397fe35440b8f12508" },
                { "br", "98c56298139aea57c515357e901f91d8032ed4ab417b860bac3929629ee28c31c2dd872a331abada6af60d1ca3243776d13dfc24374ad550b0087679fa172689" },
                { "ca", "5aabf3e5fd3ccbf3c853dbf31b79180799413a90c634330a7b5f4a717a081ba46ac9e4626704a3a4f937d5f006d23c7ab102a84725394e7ce73e19c77e5d0e52" },
                { "cak", "ab885b2c2af836f4f48e3737740bf9b76a589beb2e550054c6598225f975f7891e171b32834f77784a5526371a65106db6073956e740b67aa860c21ab5313247" },
                { "cs", "c3c9d1868d3cd4c6c3f32ed795ff5d46fd465d0756c4145b9db0e35fbd98bed81aff826475f416278167b09ee8f00af9f09642e504f6a38bf4969926b8ff4c88" },
                { "cy", "8734ed9e538dd4fcfb42646cec39ed7c0bd12a7b6788b9883a01c28a5eaf0b3a5d9ec8a0cf6677cc6622b787756b220cd18e6c00ae3d33ea2bc662293030c92f" },
                { "da", "7dc92839a031e8cfcb5c3c129113510c1cf4d09d3a7bc34a739b2ba70a0488c168c20498443c97e715a10de51d766bc6b72a71c8f8eb6ee1e91710f301caf01d" },
                { "de", "3b679906f26df8e8a064eb0663de4a20bbacc39b99ab9c030bde2284d6387a4f4be3ed71b01b4c41d5d95fa97dc4d736eb2817841c0b9e188e707ee61478492b" },
                { "dsb", "56182376388753b1743dc25afa96aeb21c20ee8b6b6987aafb405ab40cddfa5b7f14c63718e1b463f218d70fcaa806b5198fad01a55b68a24989b08c12b0a1f6" },
                { "el", "74a33b5342b4ff1cea5711ba78159e25f671d848cfa5ccde9da005c4a107c8598d25825f789c6877df84735fa7ce8321b32ee985ed4d84cf94ee0fc2f633eadf" },
                { "en-CA", "22642d70a92dd4c4e71d291a2d16f9fa4186b73bf15339cc673ca9c53e492ac84e552e03a5e1cc18cf0dd78d2e2e42cc78e0e5541e80c3ae13940cde993d9714" },
                { "en-GB", "d1935a87e585a11429e2ad4e40b85994bf42d1671092da43a705af63fc46b28ea5b09ff05d8a40c3a276cbe3b04c53e364f5f4e062f797559a66e00d470034a4" },
                { "en-US", "a1b6517effaae0d9d934576e006289e39b31d85f140f4d7526ba9c63ba6004c0185cc59ac3e4546e8354edb3fb84ec8fa55941a307a95f5c5ff50539852add22" },
                { "es-AR", "71848225791b0166d09c3af6317d1fd36ad726ca0076152089edd8f1cefb645245b2988b6a6429663fbc24f79e844f5aa2d73090cd22f19de9910cc8f2ad7e58" },
                { "es-ES", "acb41ea07e27b38d2b7869fbd63d0f974b603aa25e20fbd5eb6ca66faff7abe5a03f92b08d803e5212c4b955fd621897ffd9261c83d1c211ccb37bc872122b6b" },
                { "es-MX", "1c03a52b3179cca5c0e673e19aaea3339db47c4140f0c324adf993a267f020d5d7343491a1766780b7525f11b0d0b0ef3bbd3ce0b9509287e0333b50f9dc5031" },
                { "et", "ecdf47f75120a67a9b2e7e3f61e906238a723f404e2fe566c7358e420ca82695cd6f77ef984c7c8ed0b825e98cf8abb62a6f689a109bdbea440ef63b140423bd" },
                { "eu", "bb82ec157d39a6ecf5469b5aebaa43e9d81f2f19a44a8660dcd28199cc0b5f71ebb8a550265b21a6d137168c847a815f1a1061193ce1ca2f69727b5ad3d596c6" },
                { "fi", "7a1cdaf39ed3dc5af7f9df8c685819d688eb13ad53423060c5f71a6b4e5770b3f4a5fc0e32d851a5352029e4291ad66125254b49f31c65f426fe6cba995a99b5" },
                { "fr", "c37495ed7a7bf2071c4f6f23d93b7b9c4ea45923022c0f3c814df52438f712729c9f808796001b97fdd9691a358ec0e035a78de2faa32df4e8500679869e7793" },
                { "fy-NL", "d77cacc12d1aebdf225863349d204fb618cc72041632a71c2365ca2a20ca81c3fb3b648807cd7a70a1bab00dd82dc936843b97e678b4280efbcbe2ede2c50c26" },
                { "ga-IE", "09b72e4e671270f39cceafa099c7277c241eedf7a48a6dd1dc415169f424344fa83dab5c1740753231a0a0c719daa58bf807af8a6297146c62bb88338511576f" },
                { "gd", "2b4307edeeae7e1744f7208ee8fe248b26576847db09e2d43874c34a007a3846650face05ae18f37b409f107e835f99b19fc67be6e3cc8f9a45584d35ba6b69f" },
                { "gl", "a5bfe51f6616c8b33e84322a6c3372420cb53894dd8a2ea2dc1f953f3798d98eea76ede7b484fe28176100c873b962ec34044abc90fa9cbd289353f2b4a30b34" },
                { "he", "fec6d26abaa1a989c973183ba17941e1d18eee5e1aba23592c1c6fa09ddca28e0519494c69673f7fc3bdd3addb2bce74b562a212bed206b50a40e84ed79170a1" },
                { "hr", "34cfbd9af6d49b141fa73fca0cba8b36bb7b12fb352daf10a77cf3bd8335791b892987126873962c66af9905b17fe56d719860896f76ce0e8130704c18cf0f49" },
                { "hsb", "bc9f84e7a70407269716b1d3a54c1706ca6a5a3baf7d29492a6c2e5096969f1408f02b467eec2a0293e228fa9692f570575dad5bf10e4c18fe0708f789b7843a" },
                { "hu", "15b7a32c323148a958511cf552c3e6e788f754aafa398d62d529eb013385c370c4690ef8111dc0fa0d2643f815c7b82c7faf2d0b19e0d18747671f713c000229" },
                { "hy-AM", "89be2495912cf5c817b6fcadb08cb480f7865a570eb16a668b6c366da0251909183aaa3078e4d5c2deaed30c4f4fb71cd9273a07a2eb47bd6644f07dc20b5712" },
                { "id", "b851a05d014ecbccfc0bbf52ca3fe2460b8eb51116d4d8ff0a483ce22ec3142ebf2c76e0da184547f9d57daca9ceebf3566de03f64103fa35560b2a249a98f55" },
                { "is", "1086aaef85c2dd7445766479dbc5e27e1d4f5f60bc3e0251febab7beadd6d4e6b70c0d610352519b4eb06c9209ab225ce1a0ca59f34b6384b09baa114adac9e2" },
                { "it", "6852332649368e5596de3b9a7fd44c259dc492d1abe4526564b324ff4e62b50da1b421e53e631da169c9aec9ce7b074863340eb0918bb6fa27d953e4bfdc6e3b" },
                { "ja", "1969b978011578da8221f7e174773a6b21c497318d88c83b1e14fdcb3261c13d656657ced0690f3df08d7dce71f1bf0fd392c2c79255ab8fb188572790f6e449" },
                { "ka", "ff9976ca0b53215f6d1e5f4bc3dc4e967b8a9504dbb4e6a769ef6dc830e2cc6627dab9a365d2e422ae8f17a659a2fde3def2b3d23d0f7c5d36839787eeadc9a2" },
                { "kab", "7071f1191bb22f0dbc0c6f783910586779eb7e8e03bb09637a66e325e2c5336ee50ceb933954d74529576fac4b8bd6c7e23622f84640db8c28ca7b0f77b2f0b4" },
                { "kk", "33bde82b98846486eed6420e4c3ee5a680d6769facb3f76d5a69766d40130daf078824d01b5f93853435d507dddaf04b37c6ec1050e5dffac82002003213d6d4" },
                { "ko", "2a3211b02ce837e9f2a2e669f9a0cf2ecb4e9b78b360d4dc46b003b1c511718338688a152a7dd2a637108aeed8aa9d3df468856e023e901ff7f3584b45f3495b" },
                { "lt", "b12339d5a5bc36e3138421829f4df9f6bafbb0368881b8dc5630b4fb6c37ce2d96cbfece6f938ed1255dab6b953e492c9229230c01adfa7f9956ace029aed236" },
                { "lv", "4e93c5430fe8aa38876cd3610f1ca6923a93b4a4b78054bc0ce1c2f791a556387b5561060f792f4826608fb9a44b4a12e4c7280f1550855a8e2d926183987e91" },
                { "ms", "e59a1d9542de56ac9e6a564e584931d6d983b89f125907af9a0d0f4658f53545416bb0a9b4d92c275a79f87d66975e5f81378f5a2fa1188b415c3476c2f14684" },
                { "nb-NO", "703286e895b0868b998bf1b13c33a85332c49e9c386a9f22cdbc9d006402c92f7d14bfe59b902633b98d33d0a174eb9f4e8cffd1f0106bf59f3e7757f485d698" },
                { "nl", "ceb8e32d94d6cd81d1abfd4c78045258a85ab0e71d97d6e7d2f9f04d279ba2d11b6375a851c052305ddd98c6dfcfad7614aa9f4b42ad7924b2470150dc2ed403" },
                { "nn-NO", "7ae6068caad66674ea476fb97438aceef0baa2542bb323dbc69285038268e7849ba9a5c69b18502b79fe3627bc2ff11b95a56408a375df8f961323227da2f1f0" },
                { "pa-IN", "e4ddc4832fb27fd85f4c653c6a1d2a14e00a378047877a7409474d41d9613c19362d4d11dbabb846469f88642d7832d617ec4158c3af17e995d8e6ae774151d5" },
                { "pl", "423b71eef329b4d912c80dc2db603ed11b6837d6b7a604cbb86bf13a1766b806b709325ee9931b855e9de64bad556fa02e48edb6abfc6382fb50101d2c98d0cb" },
                { "pt-BR", "2ceb5be92a347cfe4528f44b8353bbcf29a8316cf318ecfef2e92b7fa8de723b48265df8b02ac62b1cbb2dc070ee3a9aa1a1b23c0b87b02f11c6d2b2df9f88f1" },
                { "pt-PT", "15d03335b9b4b5883145220eb636f39e980190a5f6920479f8d5d392c13b3082c5d6cbaee44c4239b48ba4f590a6990b6d2e4969b845d0eff86312fd0fae66e7" },
                { "rm", "0ca54b235f430f36ddfff500e62ff28cba56245ab54047125d21ddac472103371199cadeeb4b0ba4dcac5c58f0d5d57a250b097ca0549f06ccb0758b2ea1f690" },
                { "ro", "42cfe9d8e986d0d87fb6a27a185f12b963ba31a5a5dc0a8ed7473a1f2a1972e2af0186b67232aee992122596954ebc18f992aa967be8eac0ab4cc53b2db20442" },
                { "ru", "b53558ce4d64363c0dbdea08316eeb28287a9718488b83f483afee2e95adac6fb16773adf8c7d92674acf080e33105349b1833596c45992a30ee04408936cae4" },
                { "sk", "e5b525ddfdda1c643a30fe660af27503b96a28ac9c0f76c531ab59a75d5c61b4af6abc4413f6087fdbcba5e522d7c2174f57ebd3d899578e232a17eec27b6066" },
                { "sl", "13a4a78782aa4d8e336b1cc1e686a6ca5a316ce00df0c1a6abc3f465e6db3e66f6a58d78faaa7220a4423d12511670f0863725381e4b7a2f1d51f49baa40e8cb" },
                { "sq", "35f4a82bf513dacd251e618928496f49e026b909b5a09d82bb45a63abbe4ca7d22da5398d512135e764d80ea6e7024ab966f413a9ba94a4b504addc5ec90f284" },
                { "sr", "0d1b290e8c8121ffa361cc7cdf7e37701a88b65d29428e326c414e5616e91b63784b3a12a471be463e647e641f5c4103e47ef9fb7a602c92d7a0dd1bdbdca68b" },
                { "sv-SE", "5e97f1d012abd60ec636525abdd98b91ed37e59175c638c9ac10fcb98abae6d4899a21664f25d39166dccaeef3e4864558ba1cc2895f0c3263461997e04d2ca7" },
                { "th", "a4838cc3eca79e1f063ac9e9f21808ca37e42ce70296e420f3a36df94d867a3ffa0322ba8573967fee6ce7d87431fb312421fa5c555a98c1f5f678803298e5f3" },
                { "tr", "77508bce36308b1e1dbe82a2321cb4e18b407a55fd105208ebbfb6be0e659069ff2b6d7c49d67dec9baeecc5105144b832784b64a8744112d1588b4d3d808dc3" },
                { "uk", "ca64f95618ff65e3cbf2747ab07f185fe98e944dcb94d6d5f640929f9cab4bb43d1b3f244c3ce28b16e335fc67578197798a423d93a32d93d50b8df4f249ebd1" },
                { "uz", "b9e7b5ea056b438d6a02fcf9d01284606424df322aa10b635713bfd2b581b3f1047df1e3730734e937eb08c5e94c1aee5aa52a648ce5a90c9ffbb648f91219f9" },
                { "vi", "49490fdf2a20d4df4f422f06044e8dfdbb18f08770bb8ce33e872850ad428cc43c930d4cd4c1d03f6f3b84334e2f07e28405ab6f3f551d05eb6c45c03a370a53" },
                { "zh-CN", "cbd7507bd04734ce69be737408e597ff3600d780cb01d0b43a0709a4831f4be8a1b9f9a0a7d71523b14706f37b802dc406cfc1ce851a939084e70edfe40674d0" },
                { "zh-TW", "d1a25a1b75a2826d1d10f723b62cc8ce840439fc247981431a42f511f6e54ffa888a6530f6244093ba0e58db275971a0c3ff0eaa80d2dfd9915c26d375b2a34f" }
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
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "115.6.0";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
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
                task = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
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
                logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                return null;
            }
            // look for line with the correct language code and version
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value[..128],
                matchChecksum64Bit.Value[..128]
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
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
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
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
    } // class
} // namespace
