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
        private const string currentVersion = "118.0b2";

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
            // https://ftp.mozilla.org/pub/devedition/releases/118.0b2/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "b6e4d0b93a644e1601e1db84d6e753260a55365365c58ad3229369155ec3ced1c235830af8c1f3ad7d0c5c6e8a4406597ecc835b14836deeeb4ba892776eb7ea" },
                { "af", "c6a018359181b1e7b3b37c3eedc2562fbbb872478122880a1efc8706bb23d6f50f26bb46c87983ff646024c344df972e76cf56011383a66a25f9214583630005" },
                { "an", "4c5e2009e1c3bfad0df296ecc9c5d39091671e0fe5fb27f642a2c71331e29d620aebc1a38919e632567396123f217134d8fbb6590c264fd4cb008dfd71ff6684" },
                { "ar", "7971d6eb63360607e55a757b70eea8f2b08486688098aad588e0766dbe4ab6417c06419a5343c132148cccbef184e88675ce8afb3b969bcd312d52f79c2ef215" },
                { "ast", "fb94dd0c4da8d18884006370b370906a10e164ee2e65d842dec9151ae80677d8522652f41b10951b204f581a5d522bc058b718ca6639a9879d0c44ceb45563ec" },
                { "az", "9c23df4c31b5f5e8fb30c6e5c73a009ed0c72c51879cae26c8b5afd582e30f7df5bf4aecbb116f3b95876e94a0d7a8ecc4aec6ed893559ee8a11ebdcec7a8d0b" },
                { "be", "83c9d719ba2016855865543e4248b3518838d45fae21ce5d3d2b0c983499999a570557cd3db050dda8ae2ab93e00b7bde78ff188215b3812fa245640489009bc" },
                { "bg", "d95ab0ed6c016b19b9a1455adc3a2ee382969fccdcb2878c8fa7978fc5e29719d5c7c84c90eb98fb895c004e489b892dedaeb41f8c082c7a17bee2fcf6b2205b" },
                { "bn", "cbebdffb565699c3be1b3d3777cb5a6f4d8ecd0530be5b1132faea6dae5aebbae2fc9e13a92b5303ed77f7f877180dce9d6bc88e2719e61610f8d801e8a76e82" },
                { "br", "d61fd6094052ba6c97d011c7f6dcd59ed3e949eb13efe7137299ead90e1c55256ed26b9eaebd61df3ecb0a923fb93c48db0056baf371203fe64b306a77cdde23" },
                { "bs", "2f725fbe9f1e6d45ad803f28818a2878dc88a294b13c6e04f4674ce3033a60f6f680b8b0f7f2b5d7a0895cab682d0a6030a1f697acf31945c6ef13ee68edb9ca" },
                { "ca", "b21663ad3cb4c47f33b73d7c901ba57a9ee353a6b9fd98cabbf64c0579d7cd4fda821cfd565d447fc234e474690958bd92b0b08164bfd864190db3d12b5a171c" },
                { "cak", "bf59f7e43559448bb8b1322890d024a4c546beca9c19d67ac7ab1cb572c8fbbdedcdc71ee6770179120635ac01d4f94ea79c22080f5b6ef6f91eca154204e184" },
                { "cs", "2d03bae8c9e1902536d8b2ddf87b36b3dadcdcbb6068fad66acf82cb5035e8b5cb4bb38cc803384796de3828606471c5a9b98e094785079ce02a91d4c90eecd5" },
                { "cy", "63910d5c7d39117272f3b8412f674589bf072bf3edaa3ee8852e9f021d1e23d907103bc97bf76c9fe9bea8d43707baef3e872267d89706a63815380ee04fdaaa" },
                { "da", "c6890c3e377b34d00353b9be5c7c52bbb11bb525f6f80b6682b93ec8d9a0000a3edc83e6f754853e7cbedd62488a2a96dfc0265a232889a59607b67090820d85" },
                { "de", "bc3721e24b6f3a897c2f8e77b87273dec0cdd718285134c6639ad104e88f997f82b28a910e86d381204f55544a19f6bb0d8b72c62fafdf65acbbbed5f49afd6d" },
                { "dsb", "eb89354b85acf67b624fb02682d2af7d7ed84770e7148b64548a52020cf1457fbec0c3f219520c1bfd6b18441cfd863a69caefffbe0ed3451ec90efcdd268b26" },
                { "el", "dbf4508063ff2c8d181a5d76bf8f62729549b0a4de2aaa7c48e53ae88723cbc8235fce7bd48cca3bd7721022c605aa81634b6d7c6ba843c73c092c9effff9bc9" },
                { "en-CA", "cc516ced6aed2333493f1bfb77f21dc4ebd5d214cffc85c02c5843e25ebe1fccbca8fd1b4409716e9fdb9222cd4960bcb0d93e44dfebff6f6c805d26ad307c74" },
                { "en-GB", "2bb7b338dbe319c664867f42c4baeefdc443d11fa6aa93de5382c804837c02cf1d7049d538c0b9c21210218f1080252d288217f2e7d7c120101115785f8a1682" },
                { "en-US", "a4abb75bfdc1f86fde8fad0f06b4410a0e5929a0be10d1fc3af222f8601d9ab34a409eb5175691a57c115df6dbd552c6b36e833bde1668e4213ddfcd2636a37f" },
                { "eo", "d5af7414b785301ad558e025b8e05e019c6eab6dab781dede2ad36d4bac753831beb9e29efa2e413638f3d04efdf9d9e4b03546157365e68ac222932f16d2c26" },
                { "es-AR", "9ea0f245fde6bb29687dccc03c6971f8c3b5982acbf0366ee4b7ff9fd121cbd6ea1f85da14621989f6fa302746b9faef876ce2495860d582ce42578f73c58226" },
                { "es-CL", "c3c582a9c4aff8adddadff80cf201ba29bc15fe4aa97d3ae03175c63f254a7da2ae00bf8516cf3e55c000c984ec8e8e8b870abf88dc462bdf64e5dda103ce784" },
                { "es-ES", "e159250fde196c7a7e1b9243059c235305d036953f4d4c640e7828d89c33ccb01bcbe787f92667940d9bfc7444b7fc61cfeacf9495d31ea50bfd7075647ce8bd" },
                { "es-MX", "72a859fcffae5b148a0e3b88e4f17bc2f88051e75c95244622b0f10d01bc0800b9e5eddde13e35768d91a87eaaf0d88bc997754222db1a7cf68e9bdfc4dc841c" },
                { "et", "d740fc9864b394c362258cc5dc06fef65caa3168c41c814dd6062c303d04caeb72e24e6d7f2013975fc0b47c646ceb0a7b5d93587347a8fea3d58b64a553c547" },
                { "eu", "505f91bcd1ac361aaa2d8d3cda4b469d9805c0b3db86b68e861860b53d31dc73134d404f41e0648659618907ac9c48e411c332390b32539faaa75348d66eeedb" },
                { "fa", "414089c2a0674d7857a296f9d5691d2531760a37d29414b088fdf39b29ae7fcaeb43aed378c11fd48e4c00c20e08927d24c15f7aff018993cd74b2f2c2d9cff0" },
                { "ff", "6fcf9306daed83f37a60bc6374fe22cb70a90d93b4d0f5071bdd3ee0287ba00a31ecacdcb35953dfecfefcfc4354886f6dc2c10f6306b880a607e225d352748b" },
                { "fi", "9ac7edefb7e4e1eb3f499dfa613eb47a156339c7ab83b1f67fa9c3ebfc7d93966a0458f7619f6a25af0c6953097fb75c08d55b0c913cf216bfa8a6eba148b99f" },
                { "fr", "e8ff88c298f62c5897ac11ba571a21051caa58034613617c98aa4ee064162d1d8bd3b26a5d337ed5db4d85de87d851168f3b5597d228eb7c4ef20fcf690d21d7" },
                { "fur", "25e1c921e3ace4bd020089879801ac43b3847f637e6724f3f092e98296ad00f97a764d00259309fd7c2a77e18094b86c973f0cac574387759dd17cab63a1dc11" },
                { "fy-NL", "4f6dec7f2297cc86dcc61e425617f16da1e42ca5d98eb82eedcbb18cda36da8ba4babaab66398fc32a69cc5ff5dccede454d778fc48b418060c6ca7c8954358d" },
                { "ga-IE", "f4ae7f081fef50d265f76cfe7d29231ea8e8a7b51e1228ab78301fb8c1ba2d3c58d60dbb57628fb70ed722f50f51d4cdbd3bdb589657ed27ef5688ec02895054" },
                { "gd", "356c0dffd0028eab4f9b5233d3987df8280ed606701234d9c70d55d78733d38d89065e31a72bce8b49ffe90fc73ef2289d43ebeb39b2d8c873f14b5838413dea" },
                { "gl", "d18ce1a024baaab5c498948d0056bfcbb99501abc41ea97ebf6cdec8c62abeddf27c6300e25f9f3b029e187819da454e5d0e1075b6406050aabb09a803f2d0fe" },
                { "gn", "da7d402e4bcc4ff7a92c6fa125f0dff6527c9d8d6e73437985949fa54f43d5e3779b14cfe5c4a2a52ed56e190c72073691f84f65cc808fd51736090155002539" },
                { "gu-IN", "b3e1d85513805f11bd554e30e3daec626f01e77ae3b85d9ecdfe97e35c5b411656d4b9a2d1419079d340c84c3e764ef4a398562ddced3acf366fd8cee2d9b234" },
                { "he", "44c09074dab78803566dfa4f68d3f3eeb7290ee284564b814f9189a12b36f3a97beb3abd88f7ba3f790aae40282d0663cfa7634341895d5272b13117915cb051" },
                { "hi-IN", "a2b56c5cad88c0cf11c6f9830d4c9a8db453798527fc11654cbc24fe49a676ea9b001bcf856bf085aeefdaae90c8b302651a4644bf514baab9873b482a726123" },
                { "hr", "a86ce2733b7305e937a6f2dee219e65030ab854d739d1601c4b7f9483076ce7b5d79fce78167c03f343f234cdba14076f473d96987462091f63b92ee0cc597a8" },
                { "hsb", "ee6bbf68c6d76f2c4879474be28c644464684be2446e85fbcb0cfe5d64d9f836084aa16fab34efa51a409a32d418d8a7abac66dac4c61cc3aae6aa6f640a8ad5" },
                { "hu", "53f77504aef2263699ae003acba5764b013382369daccaf5c67727756d54c25465de0669dd671957e958f3b48e01f1c744a452c338ea6234ef0ef99d8a4430ca" },
                { "hy-AM", "eca7eb4bfbb8c127df7776019721022a24cae34b965c23159e1b7cfeda73b54f3e131d0e8e8b97603edbfde2b33975448184863ad5d77403a22ff3494f006de2" },
                { "ia", "e4c5d2a9ba0d3afbf06f1e542b84102f2cee86471a6d03f08c54df4ac259625faa084881550b503621327bab9698999ed87e9cc5657f4e30f2f1fac4e0bd6c97" },
                { "id", "2bf89328291c819b3e981ec56c24cf9d2ce7159471917ea14796bce4e47bd86ffe9200986f8732bb20a7cc0de09ec22e022cdc5d21980e8ed5e08a681b9ad9fb" },
                { "is", "e178afcb78b285221a479793b0e1ddfc3240dddbdff1c4573d08fba88e0c5fa845626414248017fa68746df9dbfff37ebb7fbda9d2adbe870c1d57c1bb1a2cec" },
                { "it", "aed2ddefc8175c41967bc5b68894e65b345eac08e410fec200ab455dbc592ecc45925f07d8d156b021d88b4cb19f765974d292065570a2d873ad552585212b05" },
                { "ja", "b11efadf52ce2821defb2f03e22f18da47d51b00fd748fa34b6d2cba4a69f35dc24b454ae8b5e8344ab0d4a724145d6cb46dcb133c08a778aa26505b852129ad" },
                { "ka", "3c723fba42e7152cc8bbc42ef737c409629fafb65d67eced40ca0427cc67cfe9382181354de762d3ec08390f24de0f3ea049e6f93c2ed060c1737cfdf4eed4df" },
                { "kab", "4d5d8ca08d9c5ce8f7233ae9824c4182e898a21f7042f793f4936b9d92fe6eb67aa5137ae8598b63653670d38fd6166c70d6cc1159fef149ac8dc78d6e40b0ef" },
                { "kk", "e31a2401377c6bfac89c16fa77455064338e515cb82b017057a3999c52f83c31dce8fe9948a0e91309254e8bb569eb2990bb9230a9d3b997552e3fedb68ab068" },
                { "km", "1b342f0f6d84aff432b7bf2e0247207d03d61446b550d8d0e329a4de7faa47ae0f9c2d6b0f6e46f0262d586864a58cb6ba102619827cc025e8b0e43200d38885" },
                { "kn", "90a79212e1c1c75a7eb9ced5c450cc02b028c47a09b23961b0c1530b9f65b36d80a20d253d38028fc6b388fbc6323b52a97be1a55971229f2f5cbc2ec44c7a2f" },
                { "ko", "7895b86d506266dc8460f2f4ddec0b44c814e0f7f1c5997791d3b7dba061b4a98a7ebfd11cd34fd4f4f024f4ac055975fd55f97b56575aeb47e985a7be01c043" },
                { "lij", "7ceb5401d891ca559d6d1caa75ea644bd0c325b1013e60cf428fb83a10f9a9abc1ed7c58ce476a770d58cbd31b139503f41e6d0d0611160733bcc0fb7b735c73" },
                { "lt", "f7918de6b09267e1706b011e920a7df79140d3d4d9411c70b09eb4b1b4f54f074841b608648578e2a04ebc21c203031ae4a43716b66388250ea06545cdbe2924" },
                { "lv", "2e7d327aa5cad5f3758e2c27001498be5637338367a57646e3ec7a85612dfb2d035112916b4888d617047136de2f364ee2012c23a996fc0672bcf014318186be" },
                { "mk", "dea4ea88a04430bd4d016784c7c2c55fe60c35205779a312506c2e99bc446088b891a609a024d3c1b1e718066fb048c19ae4830404e10a521a736b63efb652a3" },
                { "mr", "ca7c6d1b2e1df598d8ec5ca898c09ea9294cc03fd9d28697a913724c71ba3952237befd83c74f7f6b6252459b14a4c442b46dee6b12249cc883ec64f03aa4bca" },
                { "ms", "610ca6c3ff715ffde7804c101db0537aefc3e2c3c998425f7e86bb42f82ef616db3f62e48f75d1e0b575ebbd6c99ed270b566d4a92ad8ef04d79ff9ea24bb75b" },
                { "my", "08cf55dcaf33ba5a41404c976ee52225ed90d6d9b258957c4448a1b262f15b7e714bebe52eb865087680ddf9f60c90a10b487a0aa829ae97924f24965571e252" },
                { "nb-NO", "94e9411b8217be55fd52eaffcc0cee7ee805b138ce671980a85e165c3e235c524ba0fc5d7ed3be14039a97fd54cfacec25fbb0c34df7d5ed4865b1f6e2842d4c" },
                { "ne-NP", "d08529e3d1e79876895bf2d0f5a2a4bccb2ced383ac4a364cac9c7660a578843a3acbe04c2401bc008ebb3e2824446b21de20446c4f6018afcea5e620bac4d89" },
                { "nl", "f9cfe4504e4ef4f08ee94d7970987111b8c7121fb37386a2b300f88ff9c3b13965c4e3aa64a76ae71b0eaa732368a68e61f93f0209cbdf1657472ed28ecc15f5" },
                { "nn-NO", "f0d0ae23944eb91ec6e4a9e2930f1eb61b4572e8e5c1b5f2c536380a4120862dfc0eb0491ad0fdf768591af8973c1d55c4e93d7f43009d903e72e0861cef774b" },
                { "oc", "0a4811e3f612dce39b24ff67007e6b9c7a4237b953a70dd8a74029fe68fd74ae28acaa9b43c7b6618b5dbd52957df47b2feeb3308a5a2888aed7d13abbbd2427" },
                { "pa-IN", "2156b47c246ed97eaef0b29e38d7f846da82bb618449ff15c5c142c9698cd7990da471e2ce08c8862c66e693fa73877220703b7d0b3c282265a0a05f8b1b75a4" },
                { "pl", "32424fa7512950472270477b76d66d09ce2bdf585400b37b979ae34900b9387f76f51664db8be8b782c24ea460f2f5cd8c667d3f12b4e4984bb226a79a0f910d" },
                { "pt-BR", "4a27970e35451248ea4136495326dda1cc3882259f2041c1a0bcc9a07558720fba3a1348eb369ec12f26fef723cc99fec39bf7d83f823d53b82fd62b68d09335" },
                { "pt-PT", "3ea31ce705616020359621c9bc6b01fd4cb2c1cae9e6d74034a23f255c30858888fbce53bdb7b152cb831b4f0d6e092ada13ac0c3a79dcee6d9d5460383c289f" },
                { "rm", "a0e0e86a47f099692b0622ac61b8750d7c761815d71f78e2fbf13355d99fa3e91ceb52f310f8054279c4925eddf555a246904d8244ff9ec73673f40e01e45770" },
                { "ro", "90bfae9d1124291e4a92ff033900b78038189fde3238802307be875b9c2dfdeb0c4ab1274c1261d73cbc61d8c892a8380337eb8fcf9191901781266a0eef1f8f" },
                { "ru", "71da27ee52e550352261c2426df015f640667e922fed7c678bd3057c7207b554d5a1ab346d80ece7d7a680010d3172515a4af5f0254ac893e9eae76282790486" },
                { "sc", "df365bf54fb6357c572600e0aa8d88bba702049c1fc884d99406c7837f55f0a33bd27556c25d6869355fa22c726c504e158fb796a330e0da4078f11ffab1e46d" },
                { "sco", "36d65bcfa220377dd94d751eae500f734493a37acbbbac1b868d59cb9c3dfa287d21cfe2024ce262eccb34db0ad2d0134f7d8affce2c4c722d6858c805f13ce5" },
                { "si", "7f8b20b651ee8f1bb0b719fca4fddc822ce993feb1bb3b705ab5d7af8ee2ce9122d9807db0561c7d227c546fb120e12ea45ce92ee5156fff2596f7c4eacbb446" },
                { "sk", "1f1af5278abc4bf3ec786b2942231a17f84318a4cc1ca8913987a1f97811a4ddbcae5af5fa830425645e7ca603e9e3f0069d9dadf3c393a74cc700b104f20549" },
                { "sl", "c9ca5d51db80c90b2d7e02cc7ff41f6985145748fb355624121e927435a4f5f2b37a953d7f218293b9f3d67091fa8d36bfe6ad8bf9f4b9636a372942cb2dbfd4" },
                { "son", "36771c3ff3d204d6659a8b7823abb96f038e655c78980f60801f99978034887ddcbe1a31b54cbc654cb53d4d762589551ddef5b4c356834f4f359cfd7db00585" },
                { "sq", "3d05ae33dac8f158141482d00feafa6940bd81818e34773adead575cadb6ad86c3542a9b82e4122d5fa33476f9f11b180d7e3f9a51b2c3262a90394f1fef1418" },
                { "sr", "6f85c48b6d1a97eddb4221846f8dfb7e823a66851a4af8f43568ea74ac9d6173a71ae9fbd4dbaac7eb890739c88b5e943a93c04aeae74271a1273fe60b90275a" },
                { "sv-SE", "ed5af0dcd7a44522e920178e7e8f661d3f6d8515eb30e3a71007b3bfe2c19f11cdc9f2054d7b96da50750b8d9ba5aa6d85244fbd96862bbb189d67c326e28a56" },
                { "szl", "96b55f83155baa9567b73df784596f0540e80f9aa3de6e610a18a5a6a9b0796e248959cd144d6ffefde50f392ae216d49e0816952da8fda08a8ceeeaf2c34e65" },
                { "ta", "b99ea1fd630ba4ed4c26dabe0b07078bc4882f0e510564aa5a1ae5e917ae56c484dd1c9ef1a05ce42b5a5bfe5458032db63474737698de68dd4f97031a0a09d6" },
                { "te", "70b2914cf316e8a646b36440e90852d8797ec71219e1939cb8709cf747715c4df16e0d4f6aa603efabc1fdd5b6b705ac622c27c44723d04da5f2758d867caae6" },
                { "tg", "55dadaa0e3f8a5840b07090950dd820ab7adf3e6142b4c87bc1ef111da37f57524751dfc9b00c8396cd6320b7c8cfbed9fe0d8adbe49a0d5404e0408352148aa" },
                { "th", "27e9269ee6416fd780c9467d0cd615154b4a87a02332198073cf4fa3cb7c4846adef55fed54714ecfaf3e7337ec9af50b6467dc3ffad72394d4b4b4c620ba29d" },
                { "tl", "44f00e3d77cae27736640cbe4360968f88a20bad9ebd9eb5d91661219d05f4a5ec9a70892f7929d49e3b43031c3d62f7954ab074797f8f13c113e1cda707f799" },
                { "tr", "d66fb4e289303ecd592e0251989ad9774db4e176dc18d6d3636ecf57c816d5f5fb43308b95f096e78375c8241e86b0ccb9d0dda5c4c073db2393814368e0fd39" },
                { "trs", "51bc849753357627347fc14177039be51f325a65c2242f6fe0158061baf1db85622d1cc24c9835607655ae11a3583d781243dacf4686aeccd5487cf33e2dc991" },
                { "uk", "9afedd3f55ca19090908954468e3332061a3b534f5d5023db654431fd721797b2efd342ffa3a01eba1a2362650d92ff7bd29ef643024e5d5cf25484d0084769b" },
                { "ur", "9ea38e330d69e44d7e6dfa8ff115cac9108fac1037308e316214655687947601800956f9eb43f3b873ea5954940c0c4f4d2acfbed89d76f773f258c170ea2890" },
                { "uz", "9610cb9d7dafd138d5f3f6d1e4bb807a68ca71163f7f5b59cb8bff1c1e7593a589ab5f928ff410d12222d462bce017279e2c416272db2a9167b238707557b450" },
                { "vi", "3d54128421a392a56cbd68d76f7d5cd0abec65a2406f32b4057ec68c177b7c0a1b29f1d8b62a1ef9b0d625440949798761dfae898303c1b2c020abd2a47ac719" },
                { "xh", "77102d31cc8ac4b4996f99341fc42ab24f8ff6fc9807a2ddfebb7eaa803b6259cb5803e259f487f8a0858f165b03459396704590503bdc7980518d6a24931973" },
                { "zh-CN", "6e6e0c27a87c14b51b1e7a6e4af3131081e302a6a5caefdd8b1d05ca1cb78fd7a10683541a93a48b56ff6de78116619f113451d545da0670888db204be9bdca3" },
                { "zh-TW", "c44a878d1ba0e35d6c0d1e492ed8480d2d3456a9b627cf60343efc301af39d097e3694eb0bda7ed435704fae239ed326c0f1ee62ae91f2367f7a5c3cec8df108" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/118.0b2/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "f0bb4841d272ad29ed23db836f3a22359022399aad73e71379fe4c5deb7b4930054dc1f7b6647415ab1c12bf81d9ee85751206d655d0459c0c406263a7e97d66" },
                { "af", "c0e7e6b52ce6b4e598bcb033699841f883d1fc5d63f15f28fa1f9455379c47900a6b332487fb29df2c8660f12f17010500f1a3f07ab860f24a3eda695593a407" },
                { "an", "83245a74dbf7a1c1ad831e83536eeb34a9ac5bc73b49269b02eabbc8141035d849415732c8cc9f3f0bf1d31ceb1c34afd45a8f66dddb0fd7c8a2488ccffa7273" },
                { "ar", "5f83a2b73d995d13213d6437cd10bfe90ab7cafd7fc020f113fda61950f96a3ffdd2b66a72da5ea3f3c5fbe87d17718dccc5bf2ff05bc2203dd734150bb9d392" },
                { "ast", "d19d82322bd04ed3308e603548d66c23d41f56f89212fe724bc7d49df859ffa6e99c72df5fba6ab50b8c8f9ea2163f750cb168cd5a8586aa4144abe3aca81aaf" },
                { "az", "196e98b4c97af9322cbb8239711e8c6764780580c239ffeb208825a01f90f8dd0283e10fedde2dd5d7379d2c9a78d0075cf11280792aa583b7c0ce1202ba3821" },
                { "be", "87dfa02175dd38a33996acf08ad7daea651f4883d1224b6d702010179136b487d4b69f30e65fd7f033623600bc8b36f4e7c13fe50e0a0331cbd9187d249ab272" },
                { "bg", "0c5b0d6508cf9739c03ed4a93440e8356473d9387ad24d07ed84f8a47c2448c12516bb779271f19727cad311638544705820a953226c48e48739143ab342786f" },
                { "bn", "1cdcf11d1e5a490380425dbf443333ed7b70b29cd5f9830572b6f5f8722c0109429d8db36f9b571ec7c6fc6fe06e1b947e1c295dfc02042abbc0d8de80b8ca2a" },
                { "br", "692d1ad9343847e9086064a87a26518049ee3f557b2f530be69031ac0a6911e39a35b60d77a4906a2a5e46c29118aaeafd72b9b776d3cac6d359b469d61e784a" },
                { "bs", "cd500b67c43131bc102df8079c05b8c611e93bd997aa4cdb7c532df04b652ac4bfea0b0d321efab2f38d5fdd78d977eb785d97aecd4a13f3ed8f1fb2b9f166b4" },
                { "ca", "7876f644b7992a5fe6768308011829aa91dc4dfe49ffd5e30fd44b2db03b539cad3ffec810aab25f3b2128940df93190b333ae3456b3375fc917a608ad0fddee" },
                { "cak", "36f01b3c690229323827ff97fc64b189179c450076adb505f33992620a2e76c16ae305b3d84534abd5e6ffdffcf842c95e65445148e4cc5c25a8d13808e594d7" },
                { "cs", "75223039d304cef4844f669fd3b60ac5fb72ef7508017a76eb72a854aa1628ebe5060865e54e007cbe18ee0eb775298b1342e0983ccf62619ef84ce1cde50f15" },
                { "cy", "61db5081fedf43cc952125070a038921ddd6049351745deea8ec72ab5be96a6fcb8fc037106d469dbd76fd6c1785ebde7e83382c55c3231902d624f7f912e9a1" },
                { "da", "47ef6b971cb1627aba994553046377cf18193b52cb3f6e9c57986bd596241a85ef6b737ea62609ec93541203d989416e54faea011407ecad9c54807fb54a5aee" },
                { "de", "a0a840ddb2e5a6bc79581de46d7a9969aca69869cb6dec085ca5935f1c07ee70a623cb3912e7e0bafef2f0deb932bf456c21b9ecea451bd24b1b48754a3bc427" },
                { "dsb", "9601f8723229f1aed7d2353a79cac0135438869011aa552bcc5c7199c359166cce5bb88e39e1f581d638443b10179a1ec352f3c12331356b1e0037cc0c564692" },
                { "el", "253557fac3688f02543868c0021fd173378028a8df5376670268d7d4c22736fbe88f69650a4e41c50907360fce32e3b5e93dbc816df19d4d04202e871852ada0" },
                { "en-CA", "f2d2002099aa9ff56f384ce76a94ad540b01fee1609dd843111440e342a573a3605bc2bfd3f39ca9bbf1bd24afe75bdc8404f91605c672502abf2b6a5732ed68" },
                { "en-GB", "768472982be02c9962541457c116d1c51a4dffde0a0b772fc4dc22a643de33434157fbf5086514ea56a4ece9294cc6bf9686ccec977bc06cf3f4966e0ad3090b" },
                { "en-US", "6536b172db70b9cb988e02ec802e3097565c981f6ad70eedc96ee1de102d91656bdfd918a4a7de03f1d76c8c588e8b1a6ca8cfd88266027ba8855c406ccaa712" },
                { "eo", "822f54d87f728b1298c48894c910e28747901722f4ec6994016fd2197513555f03998d3713afcddca38a91e61243ffe67e05e8c874a44894f8a47c7a5d7c4494" },
                { "es-AR", "f72e7ddb8d4f24b3f7eb5746d1444e1431e6760503b250458af145a654db4936f3b0fc6701db9dcda33cb8ed1d9a6d0f47f49c9df88a96349d77d1834c72cdf9" },
                { "es-CL", "776f1f63bd2f12eac5e7c6c6799fd900d771836d4378fd0d866856f910470780dd8633e13fdecf4d3cf5d7f8ceba2832e9a1236ffa025470ecb99d1479ad26ee" },
                { "es-ES", "840b7ccabb40df62983eff18b7860c0a1633f2728c4824135ac26d10a2eafa5556e6cb3938a51b17c6fb89f1b7c40eb3767a3091187e9ed51c0875ae99943bf6" },
                { "es-MX", "7900478b5a427fb18592ecfb0a377ec4179cbb105c225e728e0f05aaa0fd7d97601e86914e931e632e8f13aa765627a2f6843b82a1e113202e6e996ff98bc069" },
                { "et", "484fcb39bad18d8d8645dd4f31708eb398bf174edc67811f5fba2cd9cfcbe3c454456a31cc859a0adb1b9cd9a60035449d61a97bbb1f409e30056faa02a87778" },
                { "eu", "f40495f4ba202798f135c2b1ca191b421a8c6d077a6ca427ce40e72892de69aac14aa370bf8a0b7287c37ba5601a840264914932a055546ecf087b2f95112ce2" },
                { "fa", "895890db52dbf8a00b10045eb380cc6e4a0fb6f4173a48206514dad2f3fe93f781e622413dfbc642fa2710ca58028d6dbfcb012b3a0ed6a1a95c30fae285516b" },
                { "ff", "770c9f4b4df6b7072f234ff429f80abbf15455b363779600b8ab78ea6eaebfb4f05faf3cac661a52db82424876ec677d96317fb107d766a5ab55dfae060d49bc" },
                { "fi", "3547244ef308a5c2aae376b61bfc02bbdc5936e6dd7c374e43244d7c033e35b11cd09c4cbf7b35496817a5ec06d6b11c646894edb17a6624da2f8a3a1cc8723f" },
                { "fr", "fc18af058d3cbd7f96d28ce4bd12661da5580c10eb22ad5db3a2d341507907c62ad5335c331e267775830f0357c412768ecc43aec64479f92545044ab18e7b9d" },
                { "fur", "310486fbea816248f56272a0150466eb61499a532d509ab1ce58c1932291f42cdf786da202cff9842c7b9ece66888aa9e765dee55415f23384d95b3d662da761" },
                { "fy-NL", "ef62bac5f391ff302dd4e6676fd92d25ed608e8d8a243ba71b62e16b865e5d7fafa86f7f25b2c7256f1b7be320992915aa3aa2810c22b162c3af8d79e1ba4ade" },
                { "ga-IE", "e0194da4c0656519796f19c643e82ef2653651c675bf7069a1d7825e89b33e826eeeb6406ce20770abd3d678c8705c7e309c3de711d9788b190de6642d39ee0f" },
                { "gd", "50a3d63cf2a1688f254577af93086959746edb2616122b5406ef860415ede6ebaa697a20cfb55327dfc0af33e7d9ef0e744e65337c761ef0177500277eb293c3" },
                { "gl", "ef29f5e039a45d58e147057bf06580451cfbe272c5d6c2c5302bc7bb0baedd7111b13cf545875563ac6c540f076f2b1d27acf77b3023d5ec21970aa5ae1a118c" },
                { "gn", "b5d0ea0383c31bbb50d64e4fa41ab5651d894a50de8f5e6a9bef2b189f475f11a271e0efcf86290f46ff94591942817fdd65552d818f91ba399cbea98e63b2bf" },
                { "gu-IN", "c55c5831dc8ed3a9e9785e137665a18c0c0d8531a968c0ab9ffa352a5437f4366319fb26d7164fc0a302649c3f3a0fdf2c71ea9feb2b92abe5e0287cac76e5cc" },
                { "he", "13bf63ae389ee14dad3d1e3a5c1558039e69b67c1b6edc1ff5c245f2487fd4c3ca21e14c5a63ea27292ab23fa5415628fe6e5d6fa122c3751e819936910b5dfa" },
                { "hi-IN", "d670dbc8a376cca5cfdf0e8c87b17036056a606b34873b0c9cf617dab6ca48c8c8566041953e06b5b680c4e0ace82545a440960f187d426b4523d428253e5a78" },
                { "hr", "653d180bb45b37d2c95606f18d1374cf65f7d16928c8eae968b6b3b4032bfda35f812ff2bfa9993d5a9693a6396989c7c5e242c4dd7158216ef0de91467f5cbb" },
                { "hsb", "6912dc874e310f928eb0364c4be47809a2a4cd294db603800b5ff962f15d2f795bb64a181ac4ad199eca21435355b7307b3f4bfc613bd03541b82be2da4f1b60" },
                { "hu", "3209a8bbf4c7d08b27c4b03742b51d890e0614f48de5cbbf5fff548f8a15bcbde2408fcf2617edbf990f42115a1840995ac7667a19ba563c50e021b73e10126f" },
                { "hy-AM", "f2e8815faa2765fc84729fe72ef7f9665ac310cf3f5427d6c5afdd5c6c83518017f72b5e034f7af970a40eb0936896732c045842f01c5bb10c653ceb79f682e0" },
                { "ia", "027692dc3cf4233466107a0b0bae904bb7b5441a346d3e3b6c75f988364bac27597660d1d2dad53682647f2999f801acb7fa7ef79cce37cb1de14cf468c0f910" },
                { "id", "fe97ec572bc6894ec83d086a95c294dd18fd4e2120afa47ce7b5e0b6fc3d51acbe40b8535c18c6dd3addd8187c85e0b61dc319b1071073a4e14dfe6b934badca" },
                { "is", "df4d42d6af9d5f13d4676f771dfdf17fbd5c7839803da7b7ff9cb7dbcb46df2cb596a794ed526a372f599e1729f21d6aaa7c9cd39a8903691013dfda9e003999" },
                { "it", "ecf702a2e007cbe30bd99b3ff2422b9d1ac844689c782ae591d16f836e581cc35ffbeafcff126f42fd5309ffb9d98e4df88ce3fb03560a8b2c1b6072b9af3a2b" },
                { "ja", "5068c55a70fa324bb02164afed03687f669358e947649a74293decd14118492b9c0c17df196f93fedbda5a9336a443a0d020345a10d4deae313ad7cfe57019ad" },
                { "ka", "02cfa836716b4ef90f3e0656e3f6d0dee342c4d02b4c0728dd3cf17d5ba9e1263a55ba8d4203fc690d4512365427c3c2dea231d69eb4c322cc86edcf4cfd1e01" },
                { "kab", "ee0b88788bca5021c592928977409faedf632d3a3c45d3d1aa09dd4af4bddab4f8f91af3ef91d345559385b76d61e244bb4f6ddba39eb246141911bd326866f1" },
                { "kk", "ada08084f30dbbdb42598e159c50b75935b1c30bc22196189dc9b9e4edc8c5f20b86c67c6fe342d63cab0c870cc3b7770b38f1061575c3e2998281e43fe58642" },
                { "km", "7c64bf4414bf32886540489290e1536a820527eb312c3459dde5ac139aa22043bc75dba3b6e387af122bca347df02c9108d2b3108f907f906ad4f7e3bb9d62f8" },
                { "kn", "cd3e56908eedf117f59753781b82929e5e4c4ff310439b8878e7fe3c763649905bf5d54ff042710e75a186e7cd1ee6372fb0917198106b9dc46c8932bdde14b6" },
                { "ko", "314153ab4663888e58cbef64680864e341c137966e3d5ba2ace0c06f9eba35b8a65c53d686934fc36cc1041617794d82b5fe71048978e7e75c0f17584f9367e7" },
                { "lij", "b582c84b6bcd074d55ed521de70415d24c3df1437d63e287bc3f0d7e7c951449f08458c41a7c52ea15d8d13b0face431268f662cc7c3e597c50612c4186a0d69" },
                { "lt", "2c15e5fbbb68e13965119162890194b17da5bafc9e29080db03d4c6e7e580b65d025226761fb2fdaae46f4731238aace3ad88489a122601f33c17b8904a7248b" },
                { "lv", "b0c36a68a8fc832acd4c152b365ab9884d5227c73e74bfb76b9a4a935c39750b171028359db705b35df97c7ffc100a9166cd9dd431586e017a0778c58c9a1c10" },
                { "mk", "c71651297c96c8a831320df256bbd0b5ba5266e72d8867f77a79c872cdf87de35594a4348e14ca6f6c13373ad9f57a12ac87d4793c38c92065a3262870c8d06e" },
                { "mr", "c4a2b5dde5dd7d72b1ea2822fc5a942cb2f7376e086b0157b7270e783d6932f0ea884aa0070e40de60626a5bb65ffaee6044ddb2ae4f4de244ac381f0661c26c" },
                { "ms", "0441f3a3ff43b8b361af55c9f643780f3027f6edf0bdb80afbfdfb40d4999c95df73974c59ca198cc3bddddcd25a345cf42a3b974e781aae4895cb8fbfb57ee1" },
                { "my", "a9374ea854bb71a4c10e9071bc047c23a45e0d4c3e1a67f85f9fbe9acee5392c9426e45c019a7a93d99984c65fa5b90e800080da2b8be4c8919f9aee68f228ce" },
                { "nb-NO", "b190ab2ad240a49449da6a5e342af28a27c51b4e9cf945c2658579bed24f6b7ff8d3142ddf22c7f9fffff2dc334119899a449e442b4aa649c21bfe7eabbe8c06" },
                { "ne-NP", "2a24e057d7048d480a976d467f48a2a5c5f21407fd4cdeeb9037e4d4a1553034b9184fdefe49f27eb2fdc675b804455b25ba98a071d60d7c4598e5d8664e4733" },
                { "nl", "f4497e9c78789418cef607d6c1e6599d2858d829950550818b66f08523fc0721e0d326fad259f6030bdf65ce8910d38b186e6081fa952e2a3b647a3839692489" },
                { "nn-NO", "29c87adc26db5bb1b2520f0b62bb1c4049eaa584e9fa218f5458410645c28aa7987bbae7cd3b79fce8bdebc20cccc3b1bde73d5b7781709b4ed4978ed75b956f" },
                { "oc", "6a8a3e9bd994c87b8e8a4bb8e4d5f101dc513ce152aeb4e301fed9a756468b4628984db71a08f9b0350b61d7bcee1269196ea39b39d07d2be22532030cd52b81" },
                { "pa-IN", "c2202883aec871153e77dc6767a35dfe70459550bbbbbdb35daba4864164ad06088ec6c10541910c3e15e9cf0079040e17b1484ebbcc14baae8c052bee076c5b" },
                { "pl", "86adb25e609e7c11ce058d848037971c265d3f7b88a136346bf6710a297de072d5181ccf70c7a957474afe0623b6f3270b35356ad2dbe7802fc68709d54cb485" },
                { "pt-BR", "1401b3ff0d882f213f10452ffb233a10ff44bd43a16eb8cae4bee2c46c5f44b5db90367145f12be5eb6ac293672249b4275ce8e9296058e813908d8c90c7bd2c" },
                { "pt-PT", "74bfe4a6e3fc01b0e14a181bb30c762d1f0348f15a1284e81e3a0764f1023f08f2c033dddab1f3cfad99eae9c7d8ac1493f1b95b18b49f3ceeffb63b87a69db7" },
                { "rm", "17e54129d42a9e9eb4efcedbd2ae73f32c98bbb7c335486fbd273523651d5628ad6d1b930978c0b8fce1a1c389e01a52389f9e1d9eadf7362ab0b69fafdb479c" },
                { "ro", "2f00140ccf3414f2d51bcab4bf266583fbe753055d9be583b28ced5971ff9b08c889c3a3bf6358f0e4c521cceb226d8936cb3d1b8cb78dedbcd7492ab6f85f15" },
                { "ru", "44384c2208cc3210374f421245c492de61aebe38bedb351c0dcc70568c6a2a450c9fbfd867c4708756d8dc88d97fdaac84c3b82502d3610f7d7571d9fa3e2aa9" },
                { "sc", "4e943c8c0b406faeb4d7681599e75ae42f5926dc405ff8d0b46d8a97489fd8183a8590665250ae2a04f0a3e81463dc5a782eb6c7401a820ba485803da1f6edf0" },
                { "sco", "03d89ddb9e1d3a9a3aba27b03b1685946f3af177986593229cd34751fc3d7764690b978d4ed31381a7bcb51b69cb5597a6faa0298ac59b465a526f845cfaa811" },
                { "si", "9b35fcfb4eafdd94e09801de68ca6f0da5a475d3780a32a345a97a2fea1b35dbddfcd383c08a779e3758ebd677c642a9109395b5268b40715e5cc16784513113" },
                { "sk", "2bbe9db6f782991fc34e75d18ea94128898835846bb77025325c89aa0ec87057e7904f8468e2dd6ce357e32c710605c2a502fefc67d7a37a2e9db7c7b8e7ab95" },
                { "sl", "130ff201188c191fd5c711afbc13eec80803864a37106ea11000783446cac00ef727ca6e3f5f98204eb5252c2f96a1532f5aa24498fe4af38ff11b5c634c044e" },
                { "son", "db6b9b1bd3e1c8fc8208e384a76d72e3baf03160b9ebc3dd434a01a2b2375f866deba74bf879bc320dd5d29593d9b1383d3511fc89250f557ac96d70d71412af" },
                { "sq", "fddd741613d31ef25ffebd37fbb7cd790ee1e1f41186c4af3ae2f07860e22979449390fe8616830023ec526ac1a109fb89148e6734ec2999011912becefd5986" },
                { "sr", "09fea529b5c3f5897b4f9b25e988397ba515c5c23cd533a4837aab38e104aac3a10825b1df551c77b59d8b9d109eed81459b4cfbd4533bdf85df3ad4ddfdc874" },
                { "sv-SE", "1c8227668ab20a6a8f1b4d828107c3563c03619fb41bd1e7faf64befc4be8f34af1654dcb7de0f3fdca2790252d8107b36cde5e54ea9a8ffb07a6457d5c20479" },
                { "szl", "30afe33c0e4fedd0ecd51338b2b09dd6fb26db22a9eccbfebfb6f1c868617c6e89bebcdc3f601512159b48608a57c9b66c90bdccfbb7d7d718f9486cda8a4985" },
                { "ta", "d9e598cc4dc95ad8c140ce4b8648f01145a312ea6ed43757be9042c69b7589db50a0203af00c3bd71574307458ffa372a0c2ee4cdedeeadf963daf7af5ab0b6b" },
                { "te", "3d4901782dd305121fa650544231f7a22e21bf09e4d8424008f55f3bf991a815f4f58654bc474e9834ef2d30145ad9e386f510444ae0c8f0936c878a75ef7069" },
                { "tg", "a53e06224b2702722831018dcf0f86d985e9855b68a5a603084b8aa7a7b34025c43be9bd9af79dd8a0298754da0c78bcb4e247791974e21d7d28481e40a0ad01" },
                { "th", "f7332233ab89ffd6a1e2a9e66eb836efecb92af7ca0f2f5b63b307ad1f6fa8c47cbce61f929c15f1881568221fdf5570a4dad9b4a4292ba83548bdb4ef8d3669" },
                { "tl", "ff92539a454d87a4987d299dffb2a8f53fc72e0196ab97eecaf60e11b8ae2250712c9cdbdcfbbb986e5cff9bd3b880f5601cc390157b7eca8f7b56b1c6016fc4" },
                { "tr", "3429564ddb53ecb88b7caa060623b5445059d35b102f4535a46b2e938448d0fc4a847bbd77501776856a79b5e54f06796251e9800f891f8560a668136f186e3e" },
                { "trs", "e35ae63b232ecbd9e77b0eda2f8f2a6eaec93942a657e7fecb7db94144e98b05c5192ca8e98424890cc05fe3eac4d93cbbfc5056ec001f860a02e4b83b3b0fb5" },
                { "uk", "fcd10f605dacb1bd46ccf73de63968ef6621f0745fe21aed2e6cfc806aff06315d85edd643b63d29bd5aaf7ddaf3108ded85b80910f516ccb7ef25abb70c23ad" },
                { "ur", "47aeae8e4413e73bf9a9d1f2158a8b820fdc462c3d6e6ac65c954cce0e6318dac900511f4246b5dccae3f528eede8ddfee7f2485652cf6d5084a7fad572396e2" },
                { "uz", "f416178eb5b3afb56149c1f4c14360040fa51fbe22729a5f5f70bf6edd9c7cc91b40ef0f486ac63c798f731c6113b8e0547ef76988834d185d29678297a6950e" },
                { "vi", "0329a3c8c5acb7d465f11f5eb69db726a72ada0c2d642632dcccb4168ccea7989c52391bec134aca9232b2e8c517d9997e1416f0d8ced41ce14da86f41044f51" },
                { "xh", "9487f9f2ba9651a8f3afd8a675ff61c4e8f1d1bf727c046adb699c44df841244826f0e77aad618ead71f5d15d9d6b25f689cc7143966d96ee6ea928c8ac20314" },
                { "zh-CN", "9076f18dd761a7efe0d43d41c79a4b9530ede364d5ed727b930226bd60ab22aadac9f1eeb31079dced7544cc4b9faf3490f1a61c4040026133068505a1ee736c" },
                { "zh-TW", "eb62ab0b920db4279d80b77c251cca3d16965dbd9c58bd916a569eea5980c1a31ee30e09a37045e0786ee9da7f94523c80ba4adb4f6477e75d51a64b96254aa7" }
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
