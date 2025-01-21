﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024, 2025  Dirk Stolle

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
            if (!d32.TryGetValue(languageCode, out checksum32Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.TryGetValue(languageCode, out checksum64Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/134.0.2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "f193b33ef976bdb911634d9fc87936e3fb8a4ecda38a8fa947a44d6e13774a2d43c8bf3e9a4a66483e3ae9c34f9790ae7ae799aa2da33da64296ae6767e685a8" },
                { "af", "39ff7d8259ade683c70ab1089825ed9a5a593a911d146edb0b96734955631eaa76c81d351298a51b32553c10ac6467f43275dc3c4d2f5f38775ee157f3f2bf19" },
                { "an", "84db4d62154138fb81d3aecf1db7225257c1b9c66d69eb1e1c20ad6fb080cde5e8c1ee4577a1fe560e6c30ef2005d1bc3b0a861435c67e27a55b80b8e3f2c760" },
                { "ar", "d50b099aaa2cf2a59291b87e3c71a1bcafe4b07c6716a79eaf7c4fb29b9f05a3ae635a3e621609dcdf5076a73f391a6d3dcd79ecd4139f51343f81c014a08ac9" },
                { "ast", "25c5d2ffb3dbbc2855fa3bf1f76b2145ffc2a2847a6a71dcc1e61a88b2253ff75047c4a95ed17a1136e0359f17898be9a4856c783ada8b0c692c83aaf75102e3" },
                { "az", "04d726ab87c457d811722eb56985b37737a3324aea32fc0fa6381f5761661d736c33e92015d9676ba8db32d044e7fddc568755107fa84b3d198d6abfa7a66aed" },
                { "be", "531c795806ee57099bb32d2ee7f7451ed7f077268495813bf8dd2de4d89160b9863c535144b9bafeacfbd369f9298b4e8445754a9bedf75a0038e6ac0f8cc64f" },
                { "bg", "dd16b81fab221a75f12e2e39cc11a82653402324a2f0b66629289cd0e6a19ad1f577a1f24230cf29f0a61dde8cca2a1b578598458a5b893d28cb60931df2f823" },
                { "bn", "0613562b12fb0ab925e41a37dd4f0742f8bff2e7da510796255dd5bde2720b1b0f4563056a9d80ac673582de8c0c88acdf6b848d09485253cf57972e1129faca" },
                { "br", "902e541d5c71b3b2f8401c7b08ec6e59801ebd0c306a72b1df4f4871c720cbb4b5ee69d4df525fee6490f0607c502f7f5e3e510da9bf1b35f0e45ddc7bbf82c9" },
                { "bs", "a23e3a2c4db16e83994e786cb7587e98b75752943adf7061a7971176e62dfcb62e2bb5f809e46adafa45f1aab5db9bc38dc9d594ee234182d644f31aa5f445d6" },
                { "ca", "0bf2655489ee7941a160c12e6cd6a48826a60bc408c1707078f464a941be5301cb89036e5922e3249500ea140551fcad05381b16f6d3becafcd778a45e23f565" },
                { "cak", "abdcd1503ea4320f7aae8b09893639f143f2e0d4c46b201121438a5b9fff0de4a6197c9f58a1f9b46d0e0832be8543e7870cc44806d23abfcd833223790c5ccf" },
                { "cs", "d1d8d61e61050e3b133c603e34f42e828555984b95eae2f196e9c5bb668596f49b2f7b9fdf2561c4034a7f90af4ae031c7f4ff8adeab1991acfae00c4ed30dd0" },
                { "cy", "f759bdecfae663d2c9fa2c61802de17c5229bc21a607edd2b15d06b7d0d48d33d54d3f357d2083552a5aee07402f95b219f38b27e9aa072fc85c0319dfd84e55" },
                { "da", "a541394d09187f0efbc42016550b6f18d891291f0ac86de82dbef64d14300286418a11a2f957db4794f853dacb246896a759856183b588e34c5f5cef25a89594" },
                { "de", "c7082c5da4b1a4c5a7cd92ba2d8ef78c85d0136ecb986b19ef7ea9eeccbeccdf092293fded4bee7e851a5ff2a49287af95bfae7e09d2c0d75b2818c074b0b4b7" },
                { "dsb", "77199cbfd48b9e2793195164ab0a64f9abeb011d94699dfa8f670a8dc8d1318a157a63ec1ea253e4a7ab22b68d69145b4c8760e86e7ec3e7d9eedb30762b3b90" },
                { "el", "cc6db23f1ac396f97d95b2dcd7a54f6b9b58680317e07e6d722d4703dc564b788c9ad507ee3fcf90e1acf6ae298529914d038a6a18934b3e41b83a62174f5c5c" },
                { "en-CA", "ce24c5ff5cad9a14153b27c62c2dfa6d844469f94d77e9c88b111425b0b94a828c3d8324b5dd0f52d24c12f3e2e28dabcee245fd4b1b3ea596100737d65583e5" },
                { "en-GB", "ccdca9d4c8fcf48c83a45469a9b9e6ccfc1186ec7d71168ea0672e6ec1894a115349766eeb4a2e5ea5d096da23a169546649bb8dbb65bdc0d7547f8372db4f73" },
                { "en-US", "cdf9016ef11834e1b280d8b0fa7b98b11be4b9ebeeaa2f968160e77582d2ddf2d0e3d250939901cdf6c44493ca9d734594cdf6e6621243ae526790c195f52adb" },
                { "eo", "2976f5e418b9a9154148671ef9d5221c056d66918600e0d4cea8c9eab4267ccaaecc4e307044838706d48cc4a2eb01ed9e1a211d951e6746d65c22b7507d7b2a" },
                { "es-AR", "a64ce3ada2c550b38aa2427e1c6b83422c2d2922a2ea45f70eef8749fa73c30fb2850c0fa66fef64f4e698461351d8c0fd819d5fb30a8d983bc446305d19585a" },
                { "es-CL", "cb90165e116259f24b28e6b9cf06d308d6f8f2a934b13bba23ce22c329f88fddf72def47fa65f7f841c34b4c6f18d91c9a1b56954ad10d1394a6fff96bcc78a2" },
                { "es-ES", "5e81d66109d217bcbf9506197edbf11d731b3775346d2c52f6730a0702b58d5af1fe83c6e7dc19eeb87aa5ded9fdfc477d783a0b55fc16eaca811f1569483694" },
                { "es-MX", "a1c814eb1a6827ebd33e36767c67f515a796847c00fb5e6a522c5c368204d094f4c0f7c856fa79f8c130580cf2b19c409a65528655e609d92f9c20b63e26ebd8" },
                { "et", "dddd4d920263b13c486b6437f777a7985dffaa8b44bf1cc92764747259aa9750324d37f7963a98b3700e9a03edf93dd2fffa58eba8b15ef257331ad4203913da" },
                { "eu", "24d8f2a72580a033c2c766a31705747584d05e61cd6fb95b37698f7d5fba407d0aada72ac20c3400f606a84483eed31b482e1954cce267526e1aac5029daa414" },
                { "fa", "7a2c35d41d089bfaafaf68d725b47a6671b747f8e95f7d24957db0e0cf5d6ea948d6064c143afcc14c977f9ef75475fb0f417791a1bbeec7565d157b0ded2302" },
                { "ff", "76bc697bd0f5fd7f1573cd690b24a48fd9168605e3a27b5033d019c131fe912c55470fce2a5661417a184e4fa310ee7703b37f0b463c08b6bb1807fd939658a7" },
                { "fi", "654b96ff19e3c6e848fc35a0a48c9b27901461be396ce43368ed6e702165373e157e7d9e2b041b08a1847213a5f1ae5a8d5ec7321cb5e68e9867125e4355da85" },
                { "fr", "7dadeedf742c846d2af0dcbb524ae5adf8ff0f6293f2786ef1ff7080af9659c9947348da7ca0c8724da53d1003b54b1d05ad1a0e965f26ba12640e5f474d6534" },
                { "fur", "f729bcff49f673972c9292a0dc25588b63f510f35d479fbe6a148ce04004c7a83b9dd5b501a8ff27dd05612466a1b96a4de53bff061999e5b360f161117a25bb" },
                { "fy-NL", "a1c6eeaed1c669f99a11adeb52a7dc09adacc45caea91d7b8f64d79aad10523ec200fca3db29e0f0c0328b88820a208df8c8770e50983c04d0f77e66d329d908" },
                { "ga-IE", "cb4cd25f648e146260bd71c08059c8794b1cadd8574ce8f615e01eab701a5ce53ffa92437933fd604c8f616b5e3186f9013d1e23607981f8f5a7e32c9bb49bc0" },
                { "gd", "b2d58f971d5ba4b8acd2dde97042397cff05d57d2f6049b5e2418d18d6ed253c717fd814e0e4775b2c0d5205fe6cc48553cfd832c57dcfcdbb14b7607efb8e87" },
                { "gl", "0dc339ab5f57f7503112f124b2487ecbb1c3cb985d9d083fa28ce42e50b916098ced171e7e52a4a3aaedc5e84fdc25e63ceaebaf1f84056cfb7798d75f12b1b3" },
                { "gn", "de6066aedac38d15f987b3cfdc2582ef46390e4b0161b076a649d7397940550ba912da7c40e963bd091a4ba5376cbafd9f7d23db8e2611ac6598f1d3dc54b1cf" },
                { "gu-IN", "9991fac29ff179b45505b756097a0143f009fab49dd8884f5b263e35054cea0c648d3b05c73229002be64f2ea497bd5d1a7966ab88bb013a886129d8626dcd9d" },
                { "he", "8136f9f5003a79aa3d7fc0c16e9e23b06fa3c68917d7212224bd0ced833502d21ba9d72a7462950214eda9ca4279371526df81dc5e6bc9288b86860a3de08930" },
                { "hi-IN", "c0a166993c5224fe69a1dd23fb6ad960b3e9fa62b05d278a7725b23147cf485ec8e42aa4d92241bc7e079d854342023dfb2174234654d4c4d2f01042e96e68a6" },
                { "hr", "da8eb0cc747e02fabda1f94949eee5f3edc81dcd78c7b8f1bff745c1780185698a051ca1135db3c06f5edc3fc4e364213964a85bdf4b8a05fc577c5d302cb2c6" },
                { "hsb", "75d593703219c4dfdd9dcaf3b4e1ec46fd4784543f5eb945b90a522ff257b915460ef528d8d6e733bfdf169f7a2f86dcd0231deb8ef027f9fb8e59f29670e764" },
                { "hu", "e982301e21f555c26fa16260032057fb9f907ec537150ba7cc38d0ef06f51f260e7018465fbba9bda44947cb583236b1da37bfbd20e0d599db62c8a65d78662c" },
                { "hy-AM", "d2f09090db05649c723b284541cf9891112ef1f6cb53f8279d80f3dfb52781f033cdf88cecbf9ed66c651fee9fa4f6451b51c19dfb60175d5c0aba12d815fc8a" },
                { "ia", "98ec2bfbcc77063e8eae31d3bbd3cec5c1eff5e82702bdfbfcf72b6d69d8867b7853412707e4187a3ee4b463ff67ea6d55b800bb46493865a36873641d5e0acf" },
                { "id", "9327b5c7f67df4c277dfce5d20715fb23dcc96e26edd6ade5b99695b6a4b940c993102d4fb0421d5ebbf8fbaf3c9513dfcd1c256fcdcd0e98ef15fe31dbddb20" },
                { "is", "430fcaf32b3cd2538762f6a07a254eae10e541e1ad2097d693af4e66731a0785ec3395a49d0e7c75ae9cda68162422a10883ce42901bb19f2acf8112a4a2fd7d" },
                { "it", "609d8bb717a811ed823c9ca197459e925403c1d84ef586ecfa9ab3cd12b72352d76cbbb9356f5b7d1f538c2d4c1bdcacdbbf967c76f3c5f3b85fc59baea166b3" },
                { "ja", "96f7696a669260c01174b3c0bd69a53f22d3ba2e57055d3b23b58a2f96a30e077b9de1f14453ada866bd3410f9cf85f8e2596fed0f927c24b73a9c6028ee0d3a" },
                { "ka", "daf4e58bc42660790cecc5ddc478d7437be7080a8a744aabdea5f2138b39c1185bf88ad7da560f70a714482d0ef1ca46dbbf1d54ea333e0cba07d046062d51fd" },
                { "kab", "1e3d6f6dfa7ed58b4baa360349fc1a6b90eb905b8c37b532189ce7eca1d3ff43a2d2de37f6004e80b8beb5f215016cea0a2031cbcb361c5119fe77378e93d5a6" },
                { "kk", "a600d608a44c3799ecd8af90ffde2a0bcf427c684cc968ef2d002dda526971f219c98efd44d7133154949c4c0fa9a6dc775bbca2e86452048db14b5c02442e78" },
                { "km", "18e5dc4ef8d70e60f808a518cd11c8c6288bc707b2456a7628decb58329aa9f8f1a3898ac8b19b66cbcde5ceb88f760ab9a5a429bc137c7e9dcf76bc418ea0ea" },
                { "kn", "2c60749eaa5b1e0e7eede3fd316450ab39c3416aec1a3cd4e5bcf39a00cdc0bfd9badd4db15fa73bf09938a8f339cdc57f2c176e573ad2f49bfe13f8e336c4a6" },
                { "ko", "5374dbe7627896c1156913ed6419c6ebaf3cc96a2ea27f2410629bc11a57584620890f71e26a5d261534f3fafb13147aa44f935ee0e58ba32595e7581e893809" },
                { "lij", "8bdd938bb117dafacd3abceebf1145ddcea4806a767b189e2c6b044471a7a99fbe894eba86074f2931f2d2ee3da2f10f5416446e6f398acde895b1a2a6abf47b" },
                { "lt", "60c107fefd21032fc7442d1a890e118f515cc704a06de7490522a1c99935f3ae70e91a2053f0a2161aab8bb6542f0547d33ddf817a88b0b82ba6f07542679eac" },
                { "lv", "92e413ce57ba354630b11e7e147ebead8f3f0aa89fb580a300ed1829e4d66734d3cd32827c3c868243498e505bba1e42174074b0b30a6d00afaac7b571a44a57" },
                { "mk", "82beb91ee0a74275b8cf78ce162b3eff6f48c636aa03379cced95216277f10b07d4f70765ceaecf823ecb4a65fa8a8fe812a46aaf8624b7e2e71c4249e0e71a5" },
                { "mr", "7d63b57ef27bcbb23261f11d1df7910f82e52dad845e791cda36e2ac493442a0faf87dd2192c373e3804ce6c24469377a7b3806295aada53917c2c3b9f1049b9" },
                { "ms", "481e79d8ccfd8d5bf988f2a298fb5cd86c453fac8497d3020f77dd3a93a89eacd9f214097d6b3fe61627d04ece4ecc5123b9e97bb4fa89822f8a0204e05efc12" },
                { "my", "6963b092bd34c736246eaad4812ee156fa81958119a3a03b12a3b39f837ffa62a7f163420a17057379cdafb60a7a94bb463132af26f0497672bdc641d1a04176" },
                { "nb-NO", "eca1c5a0622697f4ac2a7afdd993acf75542803b9c0e8599ccff1803a5d3c8aabb04466872da83bdd2d4dcd8226426334905c1e240149931f00e0d51939f54a6" },
                { "ne-NP", "2de7dd05f4488633ac305bd74ab691eb31e4e0d0d791ddcf2228a02ae4f921c8900c0c4891bf543aaa652e6e55b7eaf1123530f88a00ee028a57dbe46284d55a" },
                { "nl", "97ec4feed989fdfde4364dd706806c84e67b4028975af3e32d554196441fa4155dd5f2e1f8ef42b03ac59d356bb5d8f8e7b2ac1f01b23abc511b20f9ac36fcb8" },
                { "nn-NO", "b856cf6c7cf14e2755d2cd24d451b8273a6f48639f1d4ec8b8ffed819fa0b8fbff57252c4f15b0697575ddb87135162a9a9f604abe46d6be23d8c61a131baf12" },
                { "oc", "a2d7aa03ea57b5e6e625a08e929f104d1183ccb0bdfc9859f748b8febce9d36048c6d40a6f5ed305e4a3477183338201c23dc2f8bc2435368f0fea0a5935f483" },
                { "pa-IN", "a8fa69337b02123bfd8975f9ecf03f1bcbef123735899d941b98038f8bae910716a84ead31029b67b39d502fa00266e787ede494c951f63e5b4436f70f59f8dd" },
                { "pl", "b8d013135379351c30bf4bfc5ee96b7daa2fb3b8f5c085932947b70611e970e0ed2d50c50dcc657c6fed1f52a8ae702c257f4543ce7efff73fcc4439d92f85b9" },
                { "pt-BR", "6d46d8d50c0521d416b7cc2da0b2f5efb3c0b4835582375d4909abc3250d0552af639ee29e73a0d6eab1d43d75abd951f7e277dc4a508473838faee0b7e36f2e" },
                { "pt-PT", "7eb55c578109bd6df2fb96f3cbe3ee729c586157534ae5aa148608f1ab868cc64d56fcdfbed64b451c71fdaf79e467f55d3801d25c565341c2da6dc9a8c8f17f" },
                { "rm", "ccf6063adb47d24c58c63618155a421168671947a0b270f6ef8141fc960fc5a099e7abcdfcffc3fb51118809e3004e4f0ac0eeb94bb4146227088526e7f0614b" },
                { "ro", "a387a02d0aadceaf90c762857a9663bb53080a36917a64156b7b6aeaa681bfae9d77e56c5579072922f7d99315870430b457063430bd4e86ca5177f08503eaac" },
                { "ru", "7b2fe7815a4b9d9317984f2ea5fa328e32d1c7d375d0b83c0d70bdfdce7a99f9e60b709b27d2385bfeeff626631da19dd024127fa35cac5f641c174b5f059f89" },
                { "sat", "8ce7bab24a8e68c9c54d980f30c545f0e0ed16db8508fc50955f0de3462b5d5b7743a037e931347f1b9c529442f9ab7251ce93fbb3a4218e985882b35a321c45" },
                { "sc", "7e45dd4811568b5d8e4274f6d5b9cfa7f0e4db7752132f6e98006b06e950cb05baa78e307e933bdfa1e2016157ba4bc7e3db0d5dd37a1bce2e7dc4fc41f44b55" },
                { "sco", "c233561a90cd74990b775002abe32fdabbd121fb79722305192d7e052b467716c2511baa60e2464762286fc09f5acff4821278f8d03879fe44d006f5cab623cd" },
                { "si", "173bebbf48cf5572f57d0474b8812eba739db8dedcc67b03abba50bbba6192da7175b8802a370f38c64f54c6e1699fe4b236f3ddf7dd6dbca1715c9deb19ec2a" },
                { "sk", "771dd3b0c2e0f76c257906eb3a05d516a59797b48d904fbb375a24d9f01c58ee7d9058d6a1e081ebf3d4848fa3aa363bbc3643a47c9f9b933db3d1794f35e0e3" },
                { "skr", "2e479498f205f7762b6eb74c22b2484cac46b3d4283a8aa35fd48cebf8ede93a9044e7fd72a778c567d74b88d87d5d2da0ffba49fe3fc7659670409bb21f8315" },
                { "sl", "d6a7d5d9306fbae615d80b5e9bfe2e93e8057967a8a1b0365c580268d4f724a6d225cfbfc6c32bd9c511e9b37997b5aa36c81089493e821e3214d257ae8f1c13" },
                { "son", "31974df5f1123082b649c28e3bbd508c3551c343a666baa50dd47467438726c5f924c7d624d9ed002798911a6f62f5500d6bb395f602a8a10bd3e440045ea5cf" },
                { "sq", "946570a4db9a72879a059a34ba1c3bfd76c6df48e66a50594d6c50342ef32d69fc1f661098b102fb98ea3e6fc70f6783db471696abe3d007078c1bc52d4176cb" },
                { "sr", "af21792f752c0ae2c70b3aa3b6032ea2f419b76e6516e290c966ab3028145fe487afaa4e39442cdef56634930302d8553716007ec16b5bb1c67cc32641ebfd10" },
                { "sv-SE", "ef7363cfd9ea49c4f9d423487ac5b97d1d0970d2063b76d020063e2cdb175a961a664f3e231a8d2aaa1e1a8bf63fdbd4e65d1a1cf8211ca55ce9eb8c4d1442ee" },
                { "szl", "827c28a176b7ece2fbb5523573925c36ae08b011a3691d57b1a7267237d3cf253fa632a3045966f379928b323c1dbd8411c98fa1abf373105fd8395c03d99fcd" },
                { "ta", "472e6580da2b73cf884110ff98906082a61e24a75cacefea679a1ba74c61a071c7ede7c4ce5e4619ea201ed671655031fe0a5ecc3812170afc8cf590f3860e3b" },
                { "te", "aac3cfd04aa668b6a66e31972f49807e7bc08257fe2a6f36ab11576f97acf2ef5745312397348996e66c92f3b91cb38d948deee873f0b3fb17a082ee798db5cd" },
                { "tg", "11d205be519c0c0876d47608df9e5137f729987abd1e5bd964abf20de2acbb4567057bda39754c2394dc6ee1d389ccc95c51a3f2547d9e576cb37703383cc866" },
                { "th", "72d13b434a34009575bf1ebed4780d54249ce371e26ba4097d5793b7db5ed6c926fd4d818770ad73f201ca388ee97fda1dee13691bfab667362f950b135922ae" },
                { "tl", "1b521e5b308f6cfcddc7e68f1f5b8d48eeab1e615a12fcfc2c99440d897378d2a2bc3ee1f30f566f6d8ed599127c16c73209b652cf837f08bd3f0970e02c0e57" },
                { "tr", "40e34893c6e8e6a4596d0c77f6d0458de12a74fa655d9457533c369e7aec272fe31897d71cf8c82f27284ba309257132c3a87d9566a1fd948fcca763bf2870f8" },
                { "trs", "bbd902064b6f08a04d3e02900c806bd956f1acd29f422aa151ba63a4b91e9bbf1c12cdf23c45f4e8c22e7150481881a564850c280358edcfc5cc1d7cee849f0b" },
                { "uk", "8f079c7b28196eaf8d60e35610d581e0f455202b2322e63d076d3b789828c3a64a77cba6a3688e4f76904fb080d53aa42378ecdc793ba0df06ba39e0ad2b7488" },
                { "ur", "59101a9cdef5c91a0cd0bc4d1b3f59a687964289e5edaae25e29e69b03a8934c3c24da5b4d3f745994a36d09bc72a613adf0b977d054da690bf8480c2a3d0fba" },
                { "uz", "8f69e378d5681980345ab6117a88aa2cfeff381e0ccd83076a64c9914dd098760ba2a82f589b73986ca56b3e47dc51f9174cb4c3936b3542fdcb45c7d832fba8" },
                { "vi", "3b6b4cf132ca31a58790c6c23a3dec5ff0b8753e41ad5a4f099079825cf8a6ca5a32400edb31601ac03db7dab871c3455ddbb980c4d02175c3f52b6ab1c76cdb" },
                { "xh", "7e6c781c9e1ffbca0a59ea08377a21116a10b2210849ceef0879a0fe8b3a5e20d4f05433697c04b25f40b8b430f8f28dba17060eb534342f219c64da885b23b6" },
                { "zh-CN", "67974e348293466455b167c7d03085bdea4720a118ed879b1f3ae74596f34cc5bee86d289d10b50815a9773e0b55620bad32f9259ccc9f35165e33fdaedd15a6" },
                { "zh-TW", "ca0e6880408b96fafd9709f5a18f1162b3a26e6197260b5b64643aa9c6ac2c9f84f63e4a970cef3f6b04cac0a6a62e1f01864ab7155c0e5b993b4faaaeec1af7" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/134.0.2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "a74781feb2209b1d03c072b04e5b942c9ebc8de21a87314601ce1bfb635784de409e2a0e35beb4ae8857de4c03bafae81ebe132fe8c06ba2c204da24a2f3b74c" },
                { "af", "f07f628c33319a198e8a0c9b2e1237031b888d133593f499ef550791044bfe5fe8df92916bf4a3e504f5d52d6a800ab76190cf725fa48e4121eeafb99d28d741" },
                { "an", "052aced63244b6e2cecec37b6914d42ed9ba5a2fcc294ba31745007a249079f5da9103e0b29cf5abccbf3d611c67a73226c9301a53728994cbea601aee571167" },
                { "ar", "ce284751fbd18ed9d1546ab24dc6208d625b5c3295395a834a0db076493f1665d2dea510ec844fe254294067b0d284972ec8d334ee1a92b3938d5d2d075fb09e" },
                { "ast", "8579bba98e483a7cdd5c7c1ed9e24efd104a1d552f1071071e39af630b61f3cb2eab2868f48ac3aed1e911115960cf771772d5d7331bb1f9617d676a928304a0" },
                { "az", "cc5320bc6ac87e9fcae7d047ade41456c7614c54c5dd78ef576ed023f144a825e49f6cb5ad39f755640fee99e9bc18b1fcb2ae5a90aa6a5eae5798065bea1667" },
                { "be", "68686deb5dfe02485bb7db942b89dee2f62107d2e628a0f576600a6a26cc13bbe29685487f5a414602bc4a4471b6b9e555484451ba3c9748d8fcd48d9b427e3d" },
                { "bg", "341f60900abf59fc295151ce85f779247bef3819841c347e50cc56ec13b043977d3a735d0c3decc9fcf964dc8f78ab6eba3edc85b9a05d0dd6410ecf51694ea2" },
                { "bn", "38e4d8353b652840e6248033658f08d5a49c5504bff8baf76db40ff95b0a87a8c30cd8660d05a53148c11b39d82a77bc70b1581217c80452a048f6f3d630202d" },
                { "br", "42a4a01578585959aa8ceac58d6755a4549b42f07f154b0304319b302e3c07797223d0b38024834925a93bf80a48ec69ca5aaae322d93fe5b3a55d4a36cb522d" },
                { "bs", "e0426a1eca9a70fa27c6df5bc7cf17450ef8ee7646d96b92b6df53c2acfaf5d8f69663ad4f9d0c62685a937273f378d89922d907a4ea0d0a26adf895be4365b2" },
                { "ca", "b87e35823ead3a5de289391ef8f9b49fcb8396a24c5b0a2565d9d9cb543eb5f7b6631ec5a423137574caf08fabf39ecb1c6179395653a3217e009e5f1c032afe" },
                { "cak", "e29660581f75c2f523ee172a77554a75dd0cd7a2b9c9fe5787f16f737f6268f66e2c79b104cbc875c6845c9bcf73409272b08ca6291c45c2581e76e1c61531f3" },
                { "cs", "e3d92f5df39f81fa8a0e26f8baeb75acefc7a1a8739e830c1143321838e09e3921744d3bf8d49a6c660bbcd6cff8d91763fd5a7a349945f4a7e9537d6ce4074c" },
                { "cy", "e1b272554d87c95c85d0b4183e71fc1ed8697c06e8685b7dff42959f314339fc8237d226895250b12178077ab46f1a6b4856dafc2df9b797541f3bcf75e9742b" },
                { "da", "2080f051c9643aaa9af321cacd095263518a39e6b537fb95ca4222d9260f3e2666e7d2f02ca1532d3585b2095e88beb8516dd20105b91d8645d3611632905f72" },
                { "de", "517d3404e46037cec9a98d3603f8a1b1b0ba1ad3405504b38f2876c6aeaa294291633811760360b8f6b111a7c39378e70377fb08dbcf303e116ee0488c64ee99" },
                { "dsb", "d505262473a04688e2799e819feafb46ecd5e8863e7bf31b87d319ac091ea14ee479546ce7744a0a5423f3ad2b87bb755397ecb760e64326081ed4aa57f5b314" },
                { "el", "4fe3768cfb393f01aae1eaf0f9d5b07629456ab56f56ff1fff38dc98aa0a5ef56a96f80d2e84aa946e1b75aae0aa114943989dd7ec8d4868cfb39389b530e39c" },
                { "en-CA", "a23cda4c04a6bb68f72429448740b9bb8a443462b8d2698f2342e70f9ebe62f0909c089feefc6a9b3680e4069c0d885e56939e1d17646ec96ac3566dc565515a" },
                { "en-GB", "b5e322872b6e8a303b313e90c65a8f05f69881a2f8b204e955c6d43656fa71b37dbe6b80ea8455629a25fc6d43efc48258317f1ba05da215a992cd2f46ad3245" },
                { "en-US", "8452f8b56f50b5bb8286f0f175d193e7b16ebcb33772158c02a017bfac71a2a7a9bfa82160dfdd3965ebfd592907c902dba4c9747c248e894750b30e5bb28e2c" },
                { "eo", "1686923c7dde5101eeaa7ea93f02b00742a0ac1644c12e16e955941c855249faa121f6e165b122805ea4a338ae9db3912cac11e34c384eb87ed1ab2cc278e629" },
                { "es-AR", "f7c5b882befa7cf6f1fa03228f3abdd0e716a5777e823adfdfb658ea4f22d11ae0216f473616d67524135791ca2bd3c87b9bfc94f1919af8cf7de04aa53ab07d" },
                { "es-CL", "e7a49b5eca68c962e589aa49b6b1e77594d8d2b5b2200459e2914801ef0c68884d5e3baa7027ebbfb55410507fc93ca0567f03889d59c6e951650a569d42ee42" },
                { "es-ES", "bb9966ad0bdd20506e6aa7c905da4c375d8f67890a223c911d54f2f4393479afabffa1b8a57eae28e5c88c7a2552c9108508c526dc9b7dd4d3304fa5b070ce23" },
                { "es-MX", "878ab6d7acfc637a7e6bf4779f1bc36bcef65cbc43eea39f4542c88c85573f0b79eef7a18ce97ff61f656fcc3b89e0607b4327820fb9623e7fae17d8426f0ce9" },
                { "et", "0266329997e41e7e43de17026cfd84cd4f64a52492f7122460cdea66c100690f800f85ba443356e9d2c4296c665ffffd88aa0d5db1df66ed18520418b994faa0" },
                { "eu", "12fdfaef5fb7747b05459cafa5fa386e8abcf3f49d8080214f55e9a992f6da86adb6d3e4d3af59a78d55347c501b729ee7f5e3dcadd04a1a7e6c0fbe92d561c3" },
                { "fa", "1ac93e30df92af0394d5f507b88bba8a76257dbdee66c324e05dfe4b7b853a104bd478164b168c3b8e0f3d24d6ec6456d54991cb6573764823721717a3b5f397" },
                { "ff", "636a362568b69405e87deb5edaf169e856c47a10759be4506383eb14121e7a9a52e9d2d0e0b9551830b28df6df363d6700900287981a77955214d765f1fafe53" },
                { "fi", "0c9adf120b338ca02ae9c427396826351d9f5bb000298503fa86af14a7b15b22764f136ab401c52a79929ff73a315cf493f3973d448eb29728675a5aa4f2215f" },
                { "fr", "81e68b21b6d880df00f398f7fa3f5f6329d93f8201c78fb50daaa33049cf71947e6177b74fd365797e881ef1cb628d7974aac34a9c46452412f8e9e40c5a796f" },
                { "fur", "a49d083a3ec79b2d9f0f1ae6e05979eeea77830938d952f57cbc9a0572eb8f346d3b0950bbc844a2b20cac5e65c97f4e36e3b2f6159e7880e23447f56cebe512" },
                { "fy-NL", "b7e249fc94178cffa9599bddd02698995a12d4082951b57e8818115b94f6ea5e736be0d40564b27cf7dfde918528d68ccfee9575ff5ea5f97242d0884d9bf528" },
                { "ga-IE", "fbc3cf6b9c3689cebdd7e499b6283ac72fea8247e86d163e7065465658b240d7c2438f72bd7a6172144e4019e298a6f53dc509ab50353a84ef8b26118923b211" },
                { "gd", "2e9832b13655b301b68f69f02884d248760c7a3ab51383070575f18721b9a141c606ae669e0b292f5533d570300b832a0d926ce0ab9d4f77ca189cad4adde7b4" },
                { "gl", "25dd8fe0e0b85eb78f7ef3c2e0300533b6f5b947a2ce7d1dc91ed96cbe0dd4f2821518f66140441ab388223931556ba000de01567819b44c9cfd3ab14caf4b94" },
                { "gn", "140cb0d5fe8119abc4a05a019aeefdc4a416dca44db95d202d4f36be3d173cf45e34c4f9043589df8904fef5118d8912e3626956e4ff8e819db0e44b4897ed24" },
                { "gu-IN", "f124d1f40e89bd74e81a5b1288238cb8ec2b08c289d0d4debbf5c28a286b7eb8a6130627230092547e5f8af2f1fe10777dc6a42f7730fc2c8dc917992206e04d" },
                { "he", "aee8b1b4bc03adf244e0d477899a5b054a0e32ce7aa6bda8c5d5a866174391bf548399b12883c42cbe40d4c89d260d836813f99374ceb8de7c4141ab30f478ea" },
                { "hi-IN", "b536c25a95fb2543a8dcfe8c76238b7e47749c13f138d78584aab893a853029c8b7a461f873dbc64d4b6ae7c1f0b30686b47cf4b57daafe5a845c8557df26d34" },
                { "hr", "e91f44267a55e4707cfad645acde078be07160e729aae69f4ff3e9355929c8253387450ea01631ecd87254e03030968a5f50a0b1b062b0e602a1a783c3f05fd5" },
                { "hsb", "bfb25ba96dcd9fd33f43bd4c33801be4a3c1f7bac24b9a02bcbba15c72bc4ce69caf708e9625d4b80c4e09bd7bed5086c45efe1724a0e708b45b49857b922b45" },
                { "hu", "418be62f3416c56b9518fb09e3a66de27dc71863af1a3cba990c1193158ebb8994739861b6267dd1b3225a6e475b7700168fbae58ac582e5ece03b3fe1708f42" },
                { "hy-AM", "6e8ee46a844cc3d841723b2d3c52f1aeaa5c350299346057a399ed154653babb03fcb4aed16a36f601a7c4ec575df59882fffa04be355fdc98742403a8657d3c" },
                { "ia", "fc17c2a4c666008d167c99d12c4b4317f3388253db31cb1e6438e2e1ab4d77e5848e262e9c23ff290abd8069e0ba5269bf24f1a080543b11ec8d7d788cafad91" },
                { "id", "3df08a89fe2647474f4255a126b1ed6e876d90cef9552c5c0a56f7910defa11488f61e879a5fa5e406ce1a1c5578ca9383f560ca49823c3685fa2d55d94b9b1d" },
                { "is", "14591464fb022bafa1d3a3f14d47e7186e9c4e251b0cb94c31409c943af4b82fec1a6624b56559ab72dd04f19f1f458ecbcdb2650b2c44fe0d61be3f8fb8dd69" },
                { "it", "47dc6692ffc51ce59f75d5ec6c677993e9aef17c4f5c76a5a259376155bb8db3ef3cb4fa9c2d71ac1eed566063aeca35c12bf4a902f5032ae4b0090a0a83887d" },
                { "ja", "73fd77b4630a10d2695f7bca5c8454447348e1c8df87c09b1842f7a3c8ec531007e080feb88b21d7417bda1d885f3cdfa1f359f342e390255d2a1fb5201a9aa8" },
                { "ka", "f9705a9492e14395d90d5da52d095be23a1ea6362648fe015327f740c1fb77706135c63a0d966012a4a2934cb553f4afe1b3c2a473599f04d8fd6ccc16884491" },
                { "kab", "8b17cace318a8d650f56334cf78ac55b200e83c5ad1de719263ded3e1f8a434383f26ceaf34d5e504f1aed5ac90bd989068d08b0371b449ea9dc26934f709703" },
                { "kk", "9643c8e96acb983d5d96c37cc3de9b7f776a6b23c09baf6c7ba5245201d1148c1d4963407dfca6284e2520c66948d52f2e657c447217be768a484e7679db8200" },
                { "km", "2d2977ca4e3986a6e0509f18629aa04c8f98b9ddf7c0b3959656eb7a236d765e7a477e374fae6e8fb08f9c86825284e4cc558db1901900d9e63ca309b6679d96" },
                { "kn", "85b861bcd572c64ab184a88781b503083ec6b717ee0a7b2e43db70b16d3152992329cb6ec0c3f4cf624570dd0f969d7c617de0417d59f74c02fbf7a9d70d3e78" },
                { "ko", "6eaa8301746c3bee6430c4aeeb8b7c91819537780a0a4f55d8c3b4778355ba4687536f6b04a903872c8d8d2637cf07de05c6a1343bea5ab0275b123a6df16d5b" },
                { "lij", "48b885095b95ccba04ac18f8fe48f028a22cfbb4095745624420d0b67fbb86471abe128702c3f779c6454ccd32aad82bdd38f57258e30d3ec6960208fc652584" },
                { "lt", "183cd1e5c8be8c572eb4679229851648f8b165b179692c364416139f737a47b7b079b08874cf8de67024590f322dd0ff9e8f4d190648699e099088da76db35b1" },
                { "lv", "616a58f8367fae188dc65144639b73dc50ac4899b8ccbc30db95d4b857dc5a4809fe59a96e4c30b1a3ae85c94daf2d756506dd6096bec61ce7e60be733189ba8" },
                { "mk", "7b6db45dda1be707cf005f830871aae99a4baf886911e8d2cc9aa2ac9809a513cd658ab1e1872bc28bc126ace416682ee90be1c0364e706ca2d5cf0d3b40637e" },
                { "mr", "1840896b2a4873477904d8d4bf816f1864d1c4e768d0a3eb275453e9c89527979a2f6ee8aee5a1ccff4deedc62b3c06c34326d3d2a215835400dc066dad379ad" },
                { "ms", "c3c48879eafaf633229d91d6388462370735baf4c400677cfa35eb18c242cc9126f3715e4a0623ca9a247d65b4c45d0fe129a8e048971bb19dc542fa3fa2f9ad" },
                { "my", "3be495b8a43986f5bdf98b13e57151df90da667a58d4f25564baa72401bf06cdb289c446af3d96914a4a1eb30cf35b05b5377f15ef89e38e570543709756d831" },
                { "nb-NO", "1236fe4a6f35b95814fef130d33401f54840f9f5354ec576bb9cec816cd3c9e82bb7e85a2b9afe691a0d9eafdc08cbd60c5f6d2309e85d36f1ce0e6ed5399228" },
                { "ne-NP", "c6abfa5164f107ce97a19fc364940757e06da34af090e55697b7279341e617469c80e9100598d8133703de843476986dfa4bb633d281ce0d8c7ded2a4b8a54b5" },
                { "nl", "615768b7d7dee17c29b6107cdb9e8010b901e9993c40a6cece2835872006c8439ea930a4a0a6a4b1fa3d1b07459caf8586e3ba192de79cdd48067172489dd31e" },
                { "nn-NO", "f388c962dac7f0bde566e2399e2c3a296492821adf562645bb83144a36f5a4d4e61c0e5217aa4223f5d432459966ba9009613a874ea0040d0789c4564477cf2e" },
                { "oc", "7955b2dc219f64c774fdd973806d1133055e1948e09fde795dba666317b2f4ed387e48bf19fb34661fd6dac6decff767a1df248c202b5c48b05a44ea9fe19914" },
                { "pa-IN", "e449702a5e468e20d880569131ecb73335cbed78bd1cec9e0f1105f592ea752cbe2cf10daeb5e4bbeb77f1edc2d91d559681844f7d49f2ede6ab97a9aee24b0f" },
                { "pl", "4529023aa8f74030e17d279bfecfab6be8bca6abc2a033a04a60df9222173ca1f6d3e864ad2aabf6bbe2b90aacb2ad152d5cfd383ff1f7bf5646888d1c39208c" },
                { "pt-BR", "1bacceffa826d07e6f1d673ce50e34774a63a570eebb9ea619b4287b6fe1b5847b43d02ef4f99ed81dda8f40a3b49acce484156e4fa52c616c01d29700c4da8a" },
                { "pt-PT", "76010c4cec587b30eb2b42b4372f3077a9764240405b448322e941a37bb343402b1544631defdff36c7f551cd7e4674b5d9cc76a8a4e8c4a94d578df1745e72a" },
                { "rm", "b8bc721569ca7c5ed6cf0b476559f2d59dd0a3fa3dfada5a2125c3a4f26afa6003ee19adfddb8116c4d20bd228ee9bebbf5c7357de92b854556453881361d7ca" },
                { "ro", "a0f11aa90a1a94d9922380c264f64c6c8af8a067cd2ddeeabf7c4fd7b3b72aac355e238f40dbcab5fd53148b825e518d86a17b14ba3d5ebb11053588929db98a" },
                { "ru", "cd2b927540737230d80c9bbcb1d6c5616f4e8812626b6284d93bede3f012bfca15806f9c76a91c240e881de419e8438988a7bb88c675d0ac35113d2817781cd5" },
                { "sat", "221d9522faf03fd33313a4a051277e5f5353bb16440a453a790a3e920a6afb29e224b1f99187486677d88702b1f448e35e2ccd09bc1ac87cc9d050a1cbcfdc0e" },
                { "sc", "daf8f68214c5217dd0988b170e74a06ce9d126270242b3a274abcb4c761f542eecabe78318776d5328b50c22656c9696b1cb6251eecc83a14a22a4e2147abc21" },
                { "sco", "e184a5b187388591fb0a3f746032eecf5316d633ac31f8ba030135891124a16b39f629f207c6289f9bdcbbc6ca89d11c02d30e4c83b83c2a5a7f0d9b63562fa7" },
                { "si", "dd0d976e4a1604f6917a42e94b955a1b7631bc847c681a7c6e6cbf2e7d8c23529a8c7c5c0177beab085cf6987e8ce6056c834df8a04fa89aa187520ec341687e" },
                { "sk", "c958136cdb99eb9f502da68a7e5d81e026d0c926965514563049573ef562b39ea556bf572bf4a8f127e35c1213d1bcdf4429bc481a19b814e9628a36680d4417" },
                { "skr", "2e07c6fea7a248681ba8a1c89996be8c0cfae85170e257c297b2227229308b6086ba121a2f135786c6e35edd832bba4d2ce1fbf01f9010527a4308cfc4be7d19" },
                { "sl", "dcd5b88acb72ef7efa25a8b2a58f0f72e36a4828f3e7faae6f1d240981a680c77c55966ead37ef59ce32e32edad13fa331d048ff3de40f8f6f5e706259c72945" },
                { "son", "08ecb076cf94c1def64737821be4c6cab2c6d5387e141e01edc6cc8e022ec39d7e2a4da474d5ab7ebd88237e2181f8cdd8b1f576591ff25485b48b3ce1762082" },
                { "sq", "10392d0aa2198d5fa756ad79c15925d794eb862240768006a1f53f78b3d6ea86a89df85ae775631ccdb0791e3e14cbb19ba5a75e02e9ceab1f24fcd75caff43f" },
                { "sr", "6f7b6a463c00569522e9227b510629fe7730dace9a2d550fb1a2ad23e41ff5deb4728ca52fa114e0cdee9afa6ec821b80ab9d6c2316f06e165f4bceee970848e" },
                { "sv-SE", "75e8cd0532ffee1c181eedfac7368b13834922ebf43f09c95564c15bded48cb215de9884da79eb606c97f79fb210b2868b26914a60426a4b93ef051a896744a7" },
                { "szl", "d47cea7252408766985ffb582922f4c28b2155f41bb7cee60d6d345f64dfd9edb07a5a78bb9a3823478475861358b5af4cb9e696c6ed24bde3348defd0a6ed42" },
                { "ta", "da4c4c3c28937d1865525359cf394ba181a10cbef5da870072718c11806d267966dfc9d25598f9cbf50acfa83f6f9c0a7977cc8c076fecc62d5465870073ae2c" },
                { "te", "23f007265686f41846f39740e81d1797a6000ce8f7d00323f14fb1862f87d8792fdb78856a8f9c1c187da2fe26e35df32b7487da971b99d6f1ba1a7888b70a4b" },
                { "tg", "81a4cb359a987c4dbca4e337abd1ca167dc47793c117d23703464cd2c0ab354ae544965912a0f9c8d9fe17bfaeb2e2dd767983489772432e7ae8e079447d7cf2" },
                { "th", "63d22338148d3504c84a72326eb11de5c786b388087d117cc28a6ba6f65d166fdc56683530c12528bd38d805dd6c3755c89c1159f35fb47f35c2129eff13ff7f" },
                { "tl", "0a3196417a50e9444d0834b632a9095e51b70fea9275039472079296e5ceb2b3b05d5e91fc41e11e4125e864847962c56758f1460494480ed56264cecd8ca5b4" },
                { "tr", "8cce36673f0c545c241ebcb4ca2568a518b3dfe80136ac3afd814a2536f0a0bfe0a055790bf91d68515dcfb8f81712eda561ef8f640e569f8299f51ff1d28f04" },
                { "trs", "543586a57c192923e1e4b169f2e9adc0e9735bc42c9b6264c1ab8d880c96d7ac4da1dc5308dac2b24bd0d25b9843f15df44edf8e79e93d2400bf4ee29130a3d1" },
                { "uk", "2340f110446d803090778529ee8b0cb3db1fbad2dcad6702b76fdea5712e560c2f6e1d8de823efc07c8fc3af9601eed49e15946771c3676b5c45c15ab18f0333" },
                { "ur", "1f2d0fcb9292da49d459f3904a8b9a293596da392d75fbab237192a7551235c8c1c7ced395bfa2df866c765fddf5fee30a19ab7fca7f621871b51a27c12a600e" },
                { "uz", "7cf3c407069f3f02de5a7500fcef94ed85c3cd59541bc7a007347e155a0799891822277af109f316d90f6ba046fa426721a9e1aafc339ae342a5a09f018f16af" },
                { "vi", "27f5f696614c502a5ddcce11ad3e7e68dd49c522aa3d00bdaf61ecfd19578fa34b09c9f2ba1aeebf373abfbc20faa6564509b1d0dca8d888f60c8cf11a9e015c" },
                { "xh", "4a25a332daa71df73274d686aa953231856c1af23400c3c5cdf2406f68811b02a797961eb9085b2a4f8151b2e7658fccdf556db8a1a7e577d443a801323653b4" },
                { "zh-CN", "2fd07b8f9e65a92df32e0833668244574c6fd48d033e29ae0c124eb6e7d98ca6c8987e57008807459e6cdce01942c7db262f3d6e49eb977b4554936cd059a3c9" },
                { "zh-TW", "d0d2e617e94fdbdc07a6992a01829bf5fb97aa23eeb5e674354d4097293cffb275a1339881265738d65c8a66e94f3e0108e9a9787613e1bf0b428724634b1c89" }
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
            const string knownVersion = "134.0.2";
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
            return ["firefox", "firefox-" + languageCode.ToLower()];
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
            return [matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128]];
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
            return [];
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
