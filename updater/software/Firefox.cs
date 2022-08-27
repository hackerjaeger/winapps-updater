﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022  Dirk Stolle

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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


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
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/104.0/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "1b93b9993144163cbdcf41c8587d643a82cabc16943c3f29387b3c94f4df1cbf3e8bd3cae702778c67af49f55100cc2ee07fdd3d70283cbcff849106357a5d65" },
                { "af", "b535a82ff32783f85053610a2b513435dca75c3a91195dd2ec0d00e6299ce4fcb6b48847d4bc40bcb51061f54cda3b16d88b2fb2dcb18461b9212fb4e704ab7a" },
                { "an", "d013ab3907c297a74328cfe46a2e22bca9a726f2c3ea88efa11f28a7b3d6ba62093a74742c0854352310fe0eaaac98120d32e64cabdb06bbc8379f21886bf3ab" },
                { "ar", "c9fe0e6511e1a379de1b6757ed2d9cd359c92e69b9f68a5a8c5316b8aaf995c46a80c81a4ad5f2b2d5e30eee9228c3b769fa6839a7400db7e808e013e15f3037" },
                { "ast", "ab722694baa7a6a0a8407051b2e85cd6685c38882dd5b9dc3b4f288f1ddbfbe3db718d13c69a1d23c5be1f3bf408b7af83ee6cdfab08ada7847f4f3fb18ce7c9" },
                { "az", "bc32cc97db73211c35725b2d5ccbeec3c0acd9bb4522da5926fdc3471f1a5cdf25e5d6f9850834d43397072c8be956d4df9f20e1d960bd0274db0155851c7975" },
                { "be", "d81d2d3a72b20749eef7ea7299579e2849eee143ec6eecdfbf5e533a74537f925cde24d334263e1712defb801b07e5a2dfb70e63cc1f7fa86c730400dbbac56a" },
                { "bg", "e7ac5a436df612095b92ca44737fd7dc2ff13a1000171e871f9c8b410c7ab7bbce5eca3e77d2e2586a72ca0eb06885fe847a0d6af9002ab6479e61419c09174c" },
                { "bn", "307a8b6f1c832733e63e8c66b05c88182e3aa84be8fc06e2c0bfa4a3a9e31f70960302d580cdc95937c1f5239eb82d8ab9c4509f988374d080e579bf5fadba78" },
                { "br", "670211d65079f491c82b74430b4ce91e817c0b19bb23ef2e1a6b3f61103807fc625d3784d0174ea5a8c18a60b1ae3bf1ed4bb573b5f2c489821a824f8321c371" },
                { "bs", "45459fceb73f33a0457e97a327ba5e301ba9bba41b372c83e092664f0d6a22a2ae95f0cd02cf85b89ee87d1b5c065086afc1addde9ababa922d7dc72b8e0a469" },
                { "ca", "85be656b379533b50cfead464c7e9fbe915998c47e761eec7fda99d83d0e1b45f99ac1d09a5374bf5445ed6ae96b86ac552ff3604ca52fe7e2d5b0fe43419b31" },
                { "cak", "c8a754565994286346f479e38873a8b4f84f01dc065df5e71bf3afba9b22e14e5076d6baa3496c6c80b894ed38a330648418f27d96dd591f8ba1193977c3186e" },
                { "cs", "8b86a4ec05f711dc75c1320398be4e8ee1e0e024c253a846ac7938ad3ddb19800ccf993748afb728f29334058b4cc77ee43f83a5db1251b60aed7e1528e4c572" },
                { "cy", "acfba3e93940756445692ef508a6fe6519f05e79bf6bf3a77e24030da7089ee6e5a9356f29ef6f55c068a2c86983b30ba79d176b3b047e3964bb0f377e5866a6" },
                { "da", "3b0664c76d2ae048e8af46cfafbf8113de7cd747980a4c9204c74333b2241f8df423785139152dc44807ae8c9937595d4fb2095f963974c917d095095691cd5c" },
                { "de", "b8c831df14ab20b4b851a44458125458b786cc64e19a3ae33eb746da9d5f95f7489226f8729830b416866d1e13158d2763d41314e4a3ba1f01dc5baa1a4aad08" },
                { "dsb", "98367c95510e2b7c40910335f4e01a294ff506035e5c74345f855fd1c8f7e3d1f2cbc1fc6343962547ae423913aab88b8a969196e34640c9e813fe41a51bb454" },
                { "el", "5345ddac86fc04960d1a39a61afb77f4d4d67202efd4b623fa94bf6e6f9d084606addfd7f555575c674fd2caf69a008c1f63872a3304e562395abb8cc463f187" },
                { "en-CA", "b8d83f929351be186efe44b3dd9143846626dd26d47a2425edda80e51f5177497bc8a12f7e7be41f099a4fc1f1bb861404bd214d6452730e75bde63a8a5fd755" },
                { "en-GB", "610e47ec95748354d8dfb0e3026b9470bf5e9004628f92a7fe79854ccf01d15d9ecfd5b93dccbbdfe6d588200ab4b226d6c85ed8d313ce8cfaca7e286737bafc" },
                { "en-US", "fe78a2fcb01f4753454f06ef00f26ed07dc9c8d85f9ee04e0d6384f2139c39a53a05c53c519956e81cc95aeca0c4862a2026f4b38d6b098e36e4414c19efbcab" },
                { "eo", "df0aa60ce2f2ae68a99095a79699d949d9a1fa2e5828eeed2bed3bbdfd5c2da997c4ed558928f4de9ac78afcf05d5180ce489ba073abfd188142e74ec1168e1e" },
                { "es-AR", "08b03c0d98a97dbc185b0ca77b2c020249bdf1884e6d73a3be3e7f85c61ed93e61a08bf506f1abfd9f20850b21b1b599932b3e968109053cb8651c0b7a1f5d14" },
                { "es-CL", "e331791d487b5fc6b9da70cd842892b06eaac09f4c96eb53e41c72b2d06f6b91bb9c9a9a4e7715665839ec9f96f676cace312afb93c8f77ec5b9ceb177b1d150" },
                { "es-ES", "6a771ec2f9afefa6e95d33e035181e216b701a28bcd50e43618ed00be5b2066977985f6174f4d03d2576b5ef0c99112356c0f86aaf5cd5e2ff548747cd5f94a2" },
                { "es-MX", "f6727cefd1ebf6cb20c5f85e1bb60037ff4181a137d2d9de6e0b86d20be97556d44400d7e3e98657d9969d3d723c3d5e4dff2ceb286b30fff058c953806489e7" },
                { "et", "e3e335c5713923384a0ef87ede5c6809e40e4d811f9556e72f943e088747253182b26d65ecb135e7a40476b61de296d97f1ec7bc3dd7e7ff98e229297c9cb1ba" },
                { "eu", "49e305bbab3404b5e8be9a6e085a2f0bbab2504540d71f432e049db2e0468bc0d75cf2dc8003b438b437d3c7a9571e6e384c7949992ddd7b24512b3bd5530996" },
                { "fa", "b04eb7b432b43fb31f1503559e022420b907da3c153878b6988514a7be86c754ae2dc193b7e0347e3e04fd9dbe3f37dceda9ef6e4c33de9c4534afe1e97a52da" },
                { "ff", "a44a8405bf956eca16b7340062657fb6875944d114d88710e7e874f7d232560b7d9dc9462b33e8061e8a8162271bbca9d647915d29da4b14e003b687fa769141" },
                { "fi", "afea15b0a4186da4e1032f6e326df2a1cd5cfdc51c11ef076e8ba4dea40658bcb2cd4bf86e70e1a179e7c1c671fc7c9bc78c9583808c16e12fabaa271b15066e" },
                { "fr", "9a88bc8302edd17368b10bfd4ba0809a8125f3b1fabd6a02f36ebd7f8a3ee60fea2577c51025f637b527cc35784cacaae5bbd48feaf9b3d2123f1d47e53f9779" },
                { "fy-NL", "805faad7abbc74299263f113b17ad4ef1e12d5eae2ab10ac2afc9160254247bac5e809ead1f78ab36f42405dd7d8dfedf8a8288ea608dd0cc74354a1f8f3f111" },
                { "ga-IE", "f944e44c5ae203d1f1798067bf88ec9353fd93bde526195fa606a0380732253abfd0e4a1100866dc108d93159c7d4243830f2a63cf16fafe051354c50894f768" },
                { "gd", "99cd13a36f147f8e0ab525782a46f75423f7b8dda8831222297ca7aceefe1f550c30ab4aaf0dd4891de6b58d65b7d32ebb2d8a7ab4009aceceffa76ee9ede468" },
                { "gl", "39a5b206561d3bed34c318eee784b84204072ed286ba30b2a47ac14fd7d79c0023ceedff29cc2110c29997072a3f3aac12261b788852840c842b596618e3a735" },
                { "gn", "77141e6c9d4f158fe26a2ec90915fbc6fbba45460dac3d90d3ac023cca8c1bcd46effdbf851bad328f94754b3bea84f9c30b506093ead86d9012b798b3b77bce" },
                { "gu-IN", "3ab81832c49aaace7fc37de27b4a84a632d955e50af703571b4cfa83b998539f8ad22b44505ec125d067714c7313f2109862fb7e4cc4ca812d624cd22f4b5265" },
                { "he", "5f3af7f5e7a9166a8878aafbffd595a52380c04dee0a0daa08770ccaf7193f96d8c81316b0f6d583fe1eed5b0883d86bc20e148008c325b10b0e71a3f231a1ab" },
                { "hi-IN", "6a1693bc7e0bddb80660b0c35c41dff6cae081aaf07ab62d2c73d3fb2c8248d1af15d6aa7425b74a4850fe2525cd8b513c58897bff46009c80d9e69b15e747e2" },
                { "hr", "b381f0b2bad45984f9e9687bb51db481c5b148cd36ee5c486034d4f849c552607b6020f26dee319e8861003a08b475f002370467ed50c779a00bd9143ed2fd71" },
                { "hsb", "9ccad35981b91d3e5b94ba8efd22d37a0ec1b287fc736de6182760cbb72529eeca7276eabbece6efd1c8c533ea982b1182fb4db5e78b28584c0527d737c3ded6" },
                { "hu", "2e42e25ad0ddee788f4d537789bfc075318eb93ad4cbc2f89733ba9b8f74cf122d7a315908b418b304417ac3f04717fa476120bb129715b554943b31bcf3cdef" },
                { "hy-AM", "805a815da2cd0d78428780423a6375afde41bcb0c1f365ddff34863cb65b39ab84b0d7776be5b865c76b031a0aa84d340b08ff064fac3e0c88eef4f0f3b215c7" },
                { "ia", "06de1d71fb818c52b030286d0dabe5348945b12286a081c575d547adca61b9b278bda1348e6f7a066a221ea33fcf6b5129386856836b430e2913467de56dce22" },
                { "id", "014be8eb8cc97ead55b0414063fe34ee02da0982cc0faa13c0c650fe719099da98ec77147b5c2fb90f53c10743166031edbc89a89832ba5124442ba37fe493a8" },
                { "is", "3aae770ef1ca0ce86c6f30ddee1b7a844f160fc6ac5b59a295cdd04fbd71c7af90b78f723d9b5a582ce3ec3419457441c16a3f1b39bab19b7a7cb47a94eb5ced" },
                { "it", "d478872c668c48886cc9ed125a9f236d56ca83bcad93d0ec9371a1f3ce7a4a5d5850228d890dc1bc6d00a163edbdfd3c7e025ef349c8c959bda859e39dafb73a" },
                { "ja", "82d9da9cef826fc1f0c76a256d8e05f8e42d678bf6f47fd737a4e76e0ee1c30ff1a82c15587e2747c577e51455a92410e154145d5c88cd9b44a599ba18f882ee" },
                { "ka", "90e7225bdede3c081c323b55c66360eaeedd1e23a1d4993555d997ecdb59e4b956a82163dd84616d525dca142b7a58bed74936f05f7e5b2c2b4c02ac0671c478" },
                { "kab", "cc624cb499e67a2ac37abf5b306f5990956cd49c77b07949d4b590beb4424a140ff631c9aed859f0d81f5fd195fea349c13e665333404cd256bcdb6ba15b8516" },
                { "kk", "7ae908bb55c0f8c2517b7e2d8a295011671fc19aa0e80d1182d16d78284b7246a8267851fa985d130b199f766e65252ebd7831719de62bf22eca8f192e93f55f" },
                { "km", "5e64078628c0428deb8b7d6500b71b3fc61df0502ec0d0ba72dc9eb959e0c6c63cc70ae323b190089fad721c5ff7ef5483f122a9c000e3dd5cbf47cb0addd0df" },
                { "kn", "768e88b88f20b2bf7098900e3e1b092ef22e2be3637779a36bd19341b59afbaeae516e7b6b81b54cdceb2469b8d7b46ad51c52843e48de8bafa62ba475844a20" },
                { "ko", "067aa93624a61a10c962039d78f7fdd980f4f661407c578a9bd3d78e6f37e8ba2027cc01c9343e2499d6314645e7a2355df8bbf6fe963c34a014c5a90cde3d2a" },
                { "lij", "7dbea796bd3f7bfc012addb5562b5105eadf338af181d98dbb8fcbf7abfc951b03a323d5f65244d1d4e8332e0af162cf6384e42ca811580d0fc2fba1d1990d8a" },
                { "lt", "a72a3d85a466616b762acf9d4bfbcab4ae3b5ad1102a0a9a52931d263d1d61bb84d572790aff8aefb79320f0372529059710f8e5bf4e7f59f6a6ab4727d35b66" },
                { "lv", "56b4d16fa852a8393c83e621089b2047c4fd8c69f13a96984ea68b2ca05e348d566d73c0935759186fdac80d579eb050a76224c7a9dcad8bd3e24e3678d0e401" },
                { "mk", "b1404147369f085bf6355458557227f02112b9dc98b2d0e27273c4949ea76952734ae196b74518c32793ecba84d1014fbb0a9e929ca92609a6e3f97f3309c9f9" },
                { "mr", "a5f60d26c2cb695f728e0307b7a8f23f10bde29234e2d33afca12ae75591fcb560e7cc4b561999a47a29faae5b89c6ee99ce8ae8775cbc391eb321703878335f" },
                { "ms", "b779fad09537fe773db81554127cfae143ded1129a3154df15b9224234af4c945a95280e6625840fe3e74f01e9e5fd87febb410a60eb9f877b8cffe0f910d2fe" },
                { "my", "867696d8c5949b95664530df37546e317a76c9121d93e9615bd15c14a39f13dd06a6839b94d67b10ad73855b55e5335abbf34ca4da6db97de6bd8e44e5a7405f" },
                { "nb-NO", "1665282a7b607f5287ac7b6237291be155bf98bb253cc5ccb60b3cdfc055d96a2dde9318f25e539e9b92d44db0417d6aa877082a5731b8555be90c82e2c7be15" },
                { "ne-NP", "13a88c4b50290330265ace0ea802b9dc86d495a7d7a8451ac0028a2d175109e1a59032718c3c35a66289bfad6d6e7e0b7a01c8cf4b210d63f16a5814a47718b2" },
                { "nl", "484e4c33b4b6c598704d84a8e83629f0cfba5a35ed5fcb62d51b3f5eae8eb2b7aad0b39a85e98b65483de3672f62cf3ab82eeae6d0821a4d2dc83b7626157ef8" },
                { "nn-NO", "b70714b4e17fabb2de09e6a4aac31c1e132243bfe3fb1e99b3ca27a04cd674448a99e9faa2fe323a5f30cafd7bcebd4352d3e9862eb7711c283babf7bd9b2d07" },
                { "oc", "62bd0fdc7774dd4dddcb6581c6cf9ea226c077a4d8518c7348a9cdd2bb3b8ad4195c35589477cfb9197c13af7924db50d84ddb22975a5bc3c8bb51a132f82faf" },
                { "pa-IN", "429cda22a150518ba8346eb64eca7d73067b9541daa7e02543ef02d6481f8963853dae622d8f73a8bd477b8935a99b7d6c9dce31f5204cfffc0cc1ebf34e680f" },
                { "pl", "f88bc1cb38b1d9a2c21c155ec803f44bca1d9aa909b29bd697b38464a9be513fb00fc4bb0d2d8e0fd79f592b8bdbf6ce25465958e9d73198942ccdea9a3d1f56" },
                { "pt-BR", "c15de28c27f3bc622cf1b1db76e889321e9871e4d478b303eb2817e8dd163fa510fdb5dd6bbbab422ff14bcfbf31430aca2db0fb8b7ae5f4f83f36ae34bcaf05" },
                { "pt-PT", "5ce2da94aa65dc9696cff520e53d1961b2b8531e9da9f7781aab2a05cd1da527a89eb0718769394c20c645d9bafcbdd936514cf742ab17b3030226a923481ac1" },
                { "rm", "ca26fd2982a58792ed491acfc98f18ee120a0d4a8289f62b40b80d0bc32b6fe3234c20ec8d5c0a4d99a0ec89a0774fe63f6ac69fecdc7599f41d2fa601f240a6" },
                { "ro", "2879bf05198bc6451106d395895dfd0f143c0365a2d25192c8058606376031d7a19d52e2c2d7a12d03e3c99c0d2f44c2f4fa79d77061d18e45ea4de2f6aa749b" },
                { "ru", "a18b3aa00e671fd1552240e684f3fd0cb337ddec5e556804f12fa1b430f639e07a14882e12849d526dd41251f5edaf2d35a0b25a8d6d8fa452066461dd7bb3f5" },
                { "sco", "7ece7cb847a0d8ed6bdb4a419f2d03187c0fc759291d9be861e487f6b261e835ebdfaa0284e6497eebfd2a51eee339de78f8498c1b505ccd38c46f5e4a9afd17" },
                { "si", "cdbef2f92253d27594481c0f912995c223aafe83844493539bb1b46c23366438ce9059d3384da08f6fc081dfa57c8df7c4e90e2ceb68a6e0547fc770deb43b42" },
                { "sk", "a5769951e627c6b0e0446920fbb1b8724aa3223d641d560f27c64933015c2d4d487930490ba1ca65b76c70830a513c94519a498bfefc1a2ac6ae6eecf5adafcb" },
                { "sl", "c83457177d2a813f257b7a854750c01ba3516592ead6022f32b77c15a47cd3498abec7e5eb589c872f8bfd88c6b4d50b755c4a5d8bc5bc283ee7ce25d3954082" },
                { "son", "147bf09061b8f7039e56527d49d3445f84de1a120e54ff6920b42337f55e79ebd9817bad11c14cc1dfc9835e59cdc7490795290ae3228d584f3561f404a417df" },
                { "sq", "ae418352c223b79100a424f9c6b2de7b915f70fcaa75ea518eebbcac18c4e58a263c83567784fdfc07135bb604b7294f904b60b69917075a6d6e5e54e0cd18b9" },
                { "sr", "36bc2a21a5213e72b68d89ce2309322d6dda7048b4b08cbf9f92742e0d022b9912cd9c3f8eadab95e28810b06935da8e2e48b26e6f6af1663ce6daa520759a70" },
                { "sv-SE", "e5e6591dad211be82020fd67608b0beef4e1fc057cdf296446c607dd5f990dcb9400f5b874ddd65ade6569165b2fedd2b7a6ed8f15ce522f26beb4642702a70d" },
                { "szl", "ee66ef8d0599d3584c485d95f52ccbdb23d5664c7119f62481719d4384d0fc77143db222f9b51f6501c3a40cd09935c688e7dd3d18da1957d6d3d284bbd02b5a" },
                { "ta", "73f9390041c4317c7714bd2631571cdce9492a8471db08e86234593fb7cd82b4d2c4199c66f69874eb80c0a3dac60338e9e785dca75ed9cc13d5cb23892a610b" },
                { "te", "aabef5393f5636246a9791ae1a340b6a88f91e7709e52087c6a3384991ccde2d3f24855373c8f632b6fd8a81f664622859088bda6398bcb980333a72b37deff0" },
                { "th", "06a3a3e5da032b7927829a9f0274f8aee99b8c98012c1629c705788330bed5ef80049653c6a8a32e562c0713b6e75d569392595a0e04faaa210da2db1d96d54b" },
                { "tl", "7e286039ef68b090802462d0c0bc1c4e95c3e34563f4dabe81d0daa0933fa363847166c2c9dc686937a40246fef22edf49c916b4f2e58661bef9b24f59fe564f" },
                { "tr", "97ecb6f2d4fe846637c7448b18cdabaeb246331dd092c9ac37a63c06b49f28237785c28e3d53f4449f838b76cd256e0de4dfc59c64418c750e540cfdec3343aa" },
                { "trs", "db3137ad4140582f780a1a1e84c303b514e6e0faf9484ffc8c6c0d82a38d1023c86630cf79ccd024ed247dd0d698c0124d81581a60b8f4dfde1aec212db200ad" },
                { "uk", "916d6c613426aa986aa87557846110ed838eb7c43e8adb36485c2e2167509adb90756861ad20e2ec6da0531f9bceb078d1740b3baccaf7db64a0b5db215b2268" },
                { "ur", "bf0239c15d8adf7b4a1fed2728787b1e17ec94846b9f3df1d0b365bfff02407d2ea17b4aa684d445997dc4840b077657d3a4841c2c8bb4d2d1a6d270e247c74d" },
                { "uz", "727e3f192c502aad05850fc78077c88b60ee161a82f2f5e62311ef2ccb449dc54b44bb50e244f219f60b36797dedd90d03902922e3830584625ba90f15721353" },
                { "vi", "7cf0f20b308f57ea85a6531f4369fab886c476bf8943061ef50de96b6e3aab3fd49fd365830a58ec121201882588476287ab66339df1ba12ddfc17941a7d76a2" },
                { "xh", "d42e15604d04e4f8c8afe508d4a3dd3f417477466df412ba2fa3c3ad05bc7beeb3f2a4f8a914fa1685055c4627a8a3dfaf4f3f5a098b01bd420f91cb938276fa" },
                { "zh-CN", "e354d7764adbc43efae42bda4730f8788ee5b8d4c545276cce64c1a174b6c065454009518fd21054405945c3d3cab6d3a862c3e23d6010c4e8b3e9734806acee" },
                { "zh-TW", "aeb95f04fd891e5427c8e6986689c304f0a96287290c86000634ad7cb00c1d4eb994f0ec3bcfd15e5b28b169ad04f47a10662c3821ae41801fef9b7741d21c8b" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/104.0/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "482f38d12f7bf80d335f9b120c87ea556c8c9bb353384f213bf36b4b6a1aa212a1d8ca8c1ae8267177c5c2c3de87a23eae1bc0def906fb925f1918eb4e34968a" },
                { "af", "738afc5d3d3092682234f76ce86cc7d50133656aa30c68891500eca1852c3b8c78a17d9aacda8c1051a13fddca8233dc6c5508066a5864b88220eed81a08a920" },
                { "an", "2bb4b872560ee3b69ed8f26a8ae229f81de1fa38f057189d302e31b2e1d3a84d99bc243058fe60b46873420b0ef8aaa499e0ae5f99de3172a7bcaace3c7799e8" },
                { "ar", "b345e7c0d533d8a750fe9c3a7b3a5621be98494fe9605be5e7c2f1794a647aae700692fad6e6f398cc6b1aa512981829a7e6b1fe8f23a6a6a8052dbac3e1ae65" },
                { "ast", "fcaaea97df893bdaf7a97c5946cc2f6aca751ab22bf79504558b38e9f9e477ec2ffa4881292875ca3fa56f771dc14a2d8fdcd1a34b479226994a5b21a3e98d0d" },
                { "az", "69f83ae0c01096a5569e5dd7af03bcceca509529910bbc4f3c8e1ab2a10576fca1c11db4a400bf62cde562a653fd398decd02e56eb8d1a61d5c6cfe96603416e" },
                { "be", "9f854df4a483499eac233e5eccb874f1772fbb48eca4bfd082f77c7c29cec6900723aa922bc5e847f3b7ec3030702e88297bbf63aa9a5660127b84ea82b3d785" },
                { "bg", "08786a8663a571df8d6441241c957953b2e73160723d2303920d825f4d6ab1df7078178987717befc9ff713b417a62aedff5214ca4ad26cf58fd5285c51c9af1" },
                { "bn", "180c5c74ddadf8f208431343bff18cfe928fa1c15dd7af2ff6855f1368c9f73a36e51059bb66d660d54eaa02c2764e517cf7e09b2c9043f50dc915c15c6e2f07" },
                { "br", "4b94524139539c3aff9c5dd0c91b691581bcc092b70542b51412a8a0b3962380396a8340b278e3564b1ae2663a8b09f79ec74eb0efd24508f717819b1e769f3e" },
                { "bs", "71b33d00d550701094620fc167c2d83ec92511402653c6cd421af5cf33d7c4aabfc6cb53299bd52d18097be8a472e0779a25371d47682cb93cddd910375aa856" },
                { "ca", "6d3082cc0b11213e0d0c0c76c12d23d79ec622de16cbe3305a6d62c64939e7f29809f7a412ed4d46d510c88912347cf4c80a70c6ceb2460222dbe6bea4a369b8" },
                { "cak", "0955238bcfeccc7da4ddac9f51e2f1cddf44fb6309baf3b48a52c5cf5cf8d7f7605c1f558f5569ed6a3147fc9a83fa9cb824508c68947b003a03ccdb1f7f07f3" },
                { "cs", "7bc9643a61f90cfec3886c6b13515b612fa18dc9ad23def73024ec18ad1776fef34c7a540305dd0b338e4451c713ff7487fe833b352766e9be3eb3cd68bcbe53" },
                { "cy", "464f6cc715855bffbee32096606f730234bc4b1c283d84e2d5a9e491d776b9d8fa6c3737634a6d9cfbdae92d2e6fd4d57e5f1bbe73486cfef69073f6f999edab" },
                { "da", "f21acb9ffbc810e6b4d725f8d4eb17db44dc8bc7553a63b48c4b4785f473c4a624f49e09b12550df988d89c308765c8e149198de38b6a411c5691cae508222b9" },
                { "de", "6890a5cfe72e94d23622af14383aa2f8058e8347ba5d134e2149d02a51a53540b665a6f45d3b0e530e1fa50820ca1c63fd77ff5cfd9db4c1946787657284fea0" },
                { "dsb", "026f9bb33067c2c64c17d23b1ef4b1c3d6ef6e3d682fe6bb949e3555eea36dffb3727bc7ce4fb0a4dcba5f62b0c13c73c95e24ae9036c855784410f9c4fea3b5" },
                { "el", "8d1603a6f8d877f326c2f46faea852aea85f29bea908d15ef5fd3e709c5e1da06b65dc2f0dee65357af49135f2bf13a5c220bf542c9ae9800fae2141bd785e1e" },
                { "en-CA", "e76cdcc4b1e8166700af41f1129d6ea03a6d0dfd68a0486b4dd63b5bf955510f6bf6486623e539caa513746e1609f5ef792c9efb33b1011cf69ca00bfe080d9b" },
                { "en-GB", "37ca88dae5765b1fca7c6f6de4c6030b1f51d8f28456aedde26675c2294eb0375cf226fa017ff7aab02a3487d5a794182907f65de7449fdea3add954370f61dc" },
                { "en-US", "1f05debebad9e657437e603a99380c676a948044194b4daee48675b2c9bb158a00b08c21ca674dc7d55ff7f5ea48dbb9cb033ceca8e40c643841cdb19e537107" },
                { "eo", "7b2a8361f324d268923d815578fa2560471f973235b07af604121eec1d0a87bef715b59024261dffd3c86ed06eeb621a839ed1c177879aeaf22c55c60cb4fff5" },
                { "es-AR", "41d12a83b585fd2023b37b21904b658b054776bf71b19bc6f8f51d582c712573bd8aa1b834c75a5efed86298be005b9d08e718b77c87b72333a7f5d717593cf0" },
                { "es-CL", "62f277d1c5353ceeba7dd92a00b87e5c50fe346ab3579a23eccf952f707fd05cc352ea507b706f0f899e571cd970b3285365804ec85f9c87f6c44ebd750572be" },
                { "es-ES", "a72dcc2d1d9178f9e56ddc81bcfe821f96f5d63aa55215a823adbd272e6682c91d273d3e523d0a4415883e56ae0e381efcdd407eb88d376723d6881965a1225e" },
                { "es-MX", "0b8fba380dc4efe3791e7c58b1836fd1483d5143f643c1331fdb35d31f34808c29c980fe528a7a895a7bec1e748c00c1ce40411efbb6e5e0026fc99919bbd6a3" },
                { "et", "168adff47017d76ea9e552b887163a6f6543c5abe67c2794f50b6d020b06584d1940bb69ef8ecd515c390db9c9c37f12f20acc1502f519ded6c910be156180fb" },
                { "eu", "570e6e8e85c5e37635f953c16b24220e78e8e5f247a2f4f26405ee4aa2965ff222583e704964bfb3b8a1ee1974153627c5a4ed3ebc4f1657f351e9e19566ae1e" },
                { "fa", "80eb8cf6d7d6588e7c1dc828b2f5385631caede2e351fc68b6d7f88ec8267ea0f8cd6fd857728fb0a7d85e3f8454b0f30e425ecbc740b7d46504a7c6ba54906b" },
                { "ff", "d99d7f9cfe412adb66aaf7e993b94194883dff36c30109c106cbc563a53216b5372582627eafdcba615d7969ade0d74759eb875d72c04abf393eafc9787b6b9d" },
                { "fi", "c39d7d0f0d25b5effda06e591443da6572529e7a93a8da0bf798d4f808d4fff84411208468a70a37879cbd4a3261f404dbe1e90113fb21fe120fb55beaf626dd" },
                { "fr", "2538a6b7cdc2a9c2aa1735931aff288518785a7d64c990e7ad844fc18c0478901215dfc7c510476e3f8b218142073869a3231341819190881f68615cc6c63fb4" },
                { "fy-NL", "4647ce86365495ff9931d9d471f22ecaabe4eb54e503cc0d36e2e1b75b33186a05d3e262f40353652c45231943f5f05d58b2b1e428697bf4de4da5ae8cb9e2a8" },
                { "ga-IE", "b749cd7136b25d2d52bf5b4b5dd8619963d66cab1a7dc8ae59fdb5036afc5a8a8624da2b0570910b7eb2427e5cafbeffc11e17f6c3d62bc230fe85f726933dc5" },
                { "gd", "a7b3e2d6026b26cdec80320fa28a6ee5e139ffe660aec797a93bbd1c669571383fcf36cb333d70256e6d812dc5c8e5199f9700103af56ed9c880bf38d2421b93" },
                { "gl", "298e96c70f031db42a3021642a99510493b8fa243594ae2e666de6df5b01c254a00fc5e6c6931bb32ce3e7fac6d989b5a5ed3013184136ce82a40be93dc24f3f" },
                { "gn", "13f38eec2b11eee7c04ac86952925a5ee8d7f3b15571aee3d58cfec0403c416806feefb052ef95311bd7b15986490b511a494908850ca65cd488169d1f913abf" },
                { "gu-IN", "2717c0235626ce60fc8d4c46c4e10523ad29b07a54a5d34f8215abb745839cc2873357ee8450da007b0e480aa67967ed2d19ed71f13136887b5eec05a3c64256" },
                { "he", "009b12209572892fe277ff73a59f8e0c231ad0bad53f27072f748045a3397f16e8bab1d29fd1f97e7ec4678d7b8c564598fdb5b387b2a2417495dbcbf7dd06de" },
                { "hi-IN", "11e571a43e7d06c475fe3967c147f420d9173571a1b08006c18ea7f8bd786b455a59197e1b3c2005e71b4696eecc35ffc09cd0610a1edd7d5ee04e91f93fac93" },
                { "hr", "ac889d9fb278308ce19460c8f26fe730f234d7a16943e1a6ad547721acfc6dcef9cf34e950a8445b254995433600af267e04dd0d202d36e5226d7604f5398fde" },
                { "hsb", "81b0d1a7fa237386734af0e287853af9bacbd7eea8174d0024be341881f9f4c18f524e90507ae4a7592c59a4baeeb16592680961229839b4544d5cd441d60c12" },
                { "hu", "18cb6f64011b1bf941efb9c49c1d58df695efd8359601cd328c04ebe7aa99cffce924ab0640d76ea7ea5680f7988cb634c204d2b7e0a5fa580711d60675b25fd" },
                { "hy-AM", "d537909520fda6637415806e94aed42a72ccbecf276c0238c2d3da15fdadf36464fb8988b8499ae6718035321081e2d8fefe6565b25a24b26f1a087c042b2ed5" },
                { "ia", "89f536e782a50e96e224bffea222e72bee481203840e11968eff8c0c485be7e13b9057f1a4e525de56cedc20acf5d7bb0d28f47986a0f851e6df8e7b17f8c210" },
                { "id", "ea48afaa63d73ed05b32b837b2ea0ed89dd5f147763804937ae238c83c63bbaf873137ce4d7047698f9faeb7d86a41c5ecf435ff1272960da7ce23e941481281" },
                { "is", "33b84523c049d4976ecaad2e17f8467f68140efb2c95d0aa88b7ada7f8311b4935e225351735b2ee7130e47c8ff6182f92bc88d2e3149cc1f2a8c038780c4495" },
                { "it", "e0e162ccbadfc8eb1246cef50391f0dd15af177d0ef119b8ec78bfc8f7ad735abfa2cf33a657900b617f7374077e46286fbd1cbf29fae2decf09021bf726166a" },
                { "ja", "e50ee49b0c124f9409e5ac947a10c236487c981a054ea3f2ed581c8117ca46af183d7c7ca5454e41f990049fc73fe17ebf4849116edad7b5571fb54b25ea7ea5" },
                { "ka", "de5a3557d1fd4db278d16c5b50666f8954179a9e6647bced3a3a91ba8b2f1e92d6a80c34f4d9cb443f1a3837f580839e051b3c63415976981ca87d79ca6e66ca" },
                { "kab", "a21145a1228d479b76e202f6a7c09afc6a79f90792d3fdf25967a30945c57c07793b0100a11f200a11e2705b365093b99893bd3682374e04338dc59388646e83" },
                { "kk", "3feef0996029d51f10df09a110574cc47f246bc1df5c89520553f47aa4b2ec9332a24619d9f5f8b7e2189218888c86668c9f1b40e0e5f95b72ed07ed8fba19be" },
                { "km", "9befb5b6ae1083e41071bb65cbb9e2eca16deacb1ebb1919c8e8513125907a02e32cbe077a361e67d3b3f87aad30945eea2988e1a140adee531df8d0701e0d04" },
                { "kn", "c31ad39d49438d8432be3ece259443125a1594eeb654ba40ffbcf831126bc4b9822a8ca533af4ad4da0cd6469b5eb3beb1ed7bbcc4cf3ed13d41fa4626a98845" },
                { "ko", "c020018da40d91f64877dd734b4a0bba06c20ab14dcbf86b9d7e57a07ce5ba797aea2121231322ff540de4982357b94f970c41a082eb3bddb59c110a223033bc" },
                { "lij", "04f2274ea88a7f9c8e9c63e35ca3515e33ced9348128dd2cfea8940d7e49de44cb757e87b39fbb79823108701fd67fed5d3eac9cbeecafadf9137b5ea21dad99" },
                { "lt", "5f8d6e8130222c19f2d78fee7dff53725b68734221c80dd61cf724acf36d9bcd94078b0973fb168d9cfa0b382b8bf57a824f1469b1dfedebea3dd5cb0f092cee" },
                { "lv", "ac7851defa64ee685bdec9e4bdf7df02599a3b8c5f714e930cdb4bef0760b3e0756d1d2ce26593fc1ca5083d568714358e37d53975600a55f60a1cfe31c377d3" },
                { "mk", "0d20cc362140a15109dfc0addc8f1a44fa38d01d16819377b616c38ba550fdd65f87b3da9cfc98cf5a37788d6cca0e3d1f1d7b721a5c792fee38876030e5787d" },
                { "mr", "17751fab0eace144f4c05869cb46803c5acf2d8de6a07964c1b0ff0fd96e791ddec1aaa7b8c46989182a599256295335e1ed8e3fc95acb28654f11ece8d5f791" },
                { "ms", "0c07aa4e0430d11d689500435ba1be229e6fbc0b03ebcc5680d65d8f71bf8f18ebe21f3570493f60443204b00d7e2c93d69139950e885b7a763736fc9a8edd0f" },
                { "my", "b9d69b646609b954eba59710ca175c12abed5d89efb24eeb5eeab9cf445b2ef972de73653b0b2511e4ebdad9dac31fb52ecfc87b323407ed4b2db576f03e9d35" },
                { "nb-NO", "a0c641cab6d7d9deee1fc34381e3464896561a7590ceb808a398c0d6328ef6b028f36d57f7ed4203160ffa3e3341b04972cd3b5efa641c422d5337e183c189f0" },
                { "ne-NP", "be8328418bc6950c5ada43132a638c55ca777640186375c86902634d11a84b388c34299fd0be254b8af71ec3b89b601138c7abd8b5a6a890cc00658ebe6ea06a" },
                { "nl", "46a9a9680232f22c5ff505c46572f8d935b4ca0ab6684dc4f2d96e4812ec1d85d89df79d5bb6b9c00efda2b33ecbebd545181efb8b62b871cc93b06e816cbd6e" },
                { "nn-NO", "284a865cdf3479356afa1861b73a8cb70c3826e231763cc8570c694cc56f3591fdbb909ca6f49d4b652e5588906b441af4c63cd0cda4d518472ed8d0695caca1" },
                { "oc", "839440634d863d7bdcfeebc42e58739ff19a9dc393fb19b7a9839c17af76bdbc9dcc6c22e4b722c948f9d96471712dd1e1d5d81c047162efba107675b50d7985" },
                { "pa-IN", "935f69ac3271b8123d482a704bacef31daf1a45123800152a5dead9734371d5667b53978a4f88ab7f35ca6851fd3826b973ce5b4cfdfa5e73203fa240f4f10e0" },
                { "pl", "8a642df817ad87d5c544e396c35bd362745dee29019d550ac578bdc5b3930cfb97ba4553309f78bbafe22916677de024fcaacb8164d0a19c60ea597e5f692223" },
                { "pt-BR", "e4a57e58ab2ccd462d494d88841624ae149e248f6aa0b683f5cbd598bc6d91ed14c8991b6bd2e33ce3909a90fe855b96c019a469888f0c8f334769555555db52" },
                { "pt-PT", "483eb6f648e401744369cb494ad0057bbe2989a7b500e2e4686d4a620cc19293db4be38b6ca7e8e764fcc122d497ad81d5e2a5e33959f3140f01e36999bf98c3" },
                { "rm", "62548658758cee602178950827dbfdce575f20727392b13dc7571cc9cbcd86977388d256a61b99990f8bf925917b10e2a5eba4cf4e1557f08021894b8ac33288" },
                { "ro", "2d25828cb353fcf1307fb0e8ce497102600586022398646f886c1f4319c4eeeaa85c4ab0cdba8d539c6d9cc7931d4fbbbda45743921124db19f46ce1de63a917" },
                { "ru", "6cb53a9f5c5abda0de356a8c4feee4550fde745b00a6d4d46a03b0173a3dfe2653cca9c6dbd3d400730b8589241adbbbe580b608c4bdf0e73cb1b5f1c2db1a31" },
                { "sco", "38dcc05c28843f26b7ae817edfc3bce0701cfae16b8b50da390eccb32307011139e7e05f92f8bb21cf1ced1e3352dcff408a818949faba44ad3b03008c2f172b" },
                { "si", "242879d3f8ed5ed8b1b1f5c940c2727b5c95f22bf7099f9fe15be4b2ff3cff8af5b51e35e44f57235b234e5e76274016044bf8dfe5563df719dbb204dce008d0" },
                { "sk", "d1af029f5b6449b94ef66aa1687feb0002931be9db9a5fc1ed97c66a84e3fcde4dfc5f16ea22245e9e59ea72d654d79f8c32b86bfd7b953e785f788bc67607d1" },
                { "sl", "b80999d4f24fe58d04559cab42d0184eabfd762eae0c6e972d85cb236f3120506561cf928b5908e3ff6146b5aad14ee19e5aaf70c8a2cffe6c4b026b7b282b42" },
                { "son", "0d3eeb9f9e9f34e11c0558df54d1b73327b8c108d1acf70879eaff31e897f2539acb28957b8365829f4c21b93dc3d6e7828d36869743b8e98be0318cac43fef6" },
                { "sq", "5be3c89d66a1a2fca61b0dc0c30aac0123c85af02fda269a1a4d07781de949562ece890e1dbd5509847129bc881d2683ffedaeb36ac66684478825457c609c4a" },
                { "sr", "6755abdf8e559c49c1b0a81e5fbf9240c298c7a8e045d9bc9e1b4fc3699cb7793ff8b6baa06dc299ccbf7cc9b83526f38600fca3425a880ad1ea846ffd21e9f0" },
                { "sv-SE", "ae96e157e44df506ddb62bd6ec971f31c12896185a0f469f5d077075ee55e7f0478c37cb6f6d081b59994b7043c0eb3882f074bff45bb7be49ed184526acb309" },
                { "szl", "953ca94fecc94522b3232b758eaeb0f1e413fc888d8bbce72c5a56e3b35fbf0ec026f44d1487f2000347387a785718de0b694603f7347a5a5e74e67fada0f03a" },
                { "ta", "4e9b746dce1388823bc9049f989143e0cdd646438a3440515ef794a70a1027595b96dbd2194c9cbf792271e07e4b3b818920654275b56c9505afea665d821e89" },
                { "te", "1b3fbf037423b81cc890548972d94a520f02c0d04fd608dfb7e31957072dc22780020d0c84e9f0d253d33083f75469a806d42a25a79b153b0d24b2b46992cec5" },
                { "th", "bd3f56f9cf8d0dcc61d80b2f62f95547c5249181bceb046ce6a8274fa8daab899076e6b1bdc0046b9fa3f4b3188f29d514577274d2117b1da7aa58817b03fb1a" },
                { "tl", "944258dd2cc881dc939b346d7499e6dcc7140914e1346c74b5f7efe2668b3b1b712ee49d083e2e1e667de1d64331dbf3dc4d527919d2b72f325121814a5b80e4" },
                { "tr", "02ef61651443a0b50407613bf8cb684aac2ba46f5e9081592309051d6c434fdb79c1878774dfa5c4e2afb8e7eff1d90a81bdc0f7187ebf895c777d81272609be" },
                { "trs", "3c88904246930dde8d597008d3058085e3f1546148b328a5dc68727f4a33ac4c51c5b910bf543681a8dee33a681ad30862aac3e5d68e634b66d8d5dc268ad7d7" },
                { "uk", "9a8dbccd509193c949702b74504d3ab9e58f3f045aefccfb8c36cdfe1efe996ea8436b7175831e44114cd8858d7a271d54761ce91a04655dd943365b9fb12784" },
                { "ur", "350b2cbfbbb834d8cca4649d94a2d6ce6f5a616af4622582440f57c7f26f26cbcc6742e5c0c15cc3d83e782f2d29724e13f95b6ac8b70401858a724ab3bd4537" },
                { "uz", "a6266049a89a351572bf3f6421ae5035d6a298832cde7a3170e044a00db86fe7390eac8d03b6039adb47fef13f96243dbdfa8cf9fdb7d8c970467f3203fca6f3" },
                { "vi", "037394d5b17f8f5f4b0fdc66269098af3a6364d865c348936a2bd7b6bc9ba5c5b1278ae9893c5650b5fe5dacfdcfc0c20b1ec6c6870acae9558c2fd66e465f70" },
                { "xh", "043d92192524280ff0316d16e983cdbbe53bb65b31ae19c358927477d2f847969d53fb3cb715d2e92803c323e8641acf84d8ca8ad9e4814c42825b0b82273983" },
                { "zh-CN", "1168c31a8e2ee396529fb32be65003d41b0670cef42afce30e4cbd9f4959caa9975dbe2b25510b032c899db6a5e9cd71b824c25058bc2915dcada2769c66f927" },
                { "zh-TW", "0fda30d719fdae6fbb7d58259161a3ea82b7da20bf0f4ddfa8893cbf1edd8b0c5819cd640e722f4b4f2e8e369038820f492465dabe41254ca5e7c22814d61b7f" }
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
            const string knownVersion = "104.0";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
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
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
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
            string sha512SumsContent = null;
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

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value.Substring(0, 128), matchChecksum64Bit.Value.Substring(0, 128) };
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
            logger.Info("Searcing for newer version of Firefox...");
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
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
