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
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.11.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "627447ac37b0becf27df88822bfbde37268014f3993091d09dacf8ce55cbeedbfbbd01b536e6d021fc2aa0dbedf70864652d1aadc4d98af46ef5284a4ce2c191" },
                { "ar", "207607cf100bf7ea4a60b09ac148c3acda805e95e37a60bdc75933313369605fd987761ba9b31a77e8e9b8f28b6e080509570100dc95eb3a57ec8f3ca115de3b" },
                { "ast", "ea5db19d655918cc1fc8e0cbb911a2a20384a27e3c0e3f4ae570cb94b276cf80fdf3d3e3e88565131f267c07d0e031213cd326dcaf1e4b9250ababc59d87027c" },
                { "be", "a0d3f0fcdb423e604670924abf3d82660a34cef45b761eaab48dba8080952f505444d9441c3b74937f067acde18616c174e174f85ae65a1994eebfb551cf4a58" },
                { "bg", "0a6a8686b95ad803a391dc2f381ac4061e714e511168d65645fb847321d4bcdfa6cc1c7a973f1dde952dcd5f2f185990d3efc5d0984adf09a81874923670733a" },
                { "br", "c79806088946bf7ac1e12ca055a05fa7203abd826679f7fbc5717c2f5c8ea04681fc4fc4b66f6ce574511bc7d7fa4bb9df1f934efe7de16b0ae8f1ad847c2b5d" },
                { "ca", "5f467cb74af4db7a26f08c964fac2ca8c6884377a6fcc4e096c0fe4beb6202dd5dabf54d8c2539aeae0c39cfa04ee0b159a681de43dcf737460840a2b50a33c6" },
                { "cak", "1cb7291affce94f0ee8ad553063da0f8ae6c7bceec56885717b3020216576b0d6c1ee6b6f16cb89dfff441166b38899f9cc779d7c30e531ef759c67c7f9e3939" },
                { "cs", "8edac063b4cfb7aa9b4140b1c4d4150d46a88e735d72b74594f7f49c7e9f28aac68122c6125c80e660ea12b910557ba6886914b208722241dae0b11883e4ebf2" },
                { "cy", "2b523f042f2f251e9f38c083028192fac3161aa557e1a24375d52494922bfb69beaddb0b786def1d8c6b233adfe1d841523409ad2a23450cc5a5494c60a5e555" },
                { "da", "d0a782aa17be27a8d983a4142f4b8ef9d3791b248d31026fb82ab6f2e31a341e1dc4ae3fe19bf5e6fcfd086d66b8ff50057bd0bcefa778f2f59f20afa6739e8a" },
                { "de", "297235004f70ccb3e266a3c23081c6f7c6e7e5f9d98a65fb546dde0661bfa3ac264de85fa2d8b73f0197d7218d2bcc4b7fc1038062364e6b02d289a19bb52444" },
                { "dsb", "29c7cc9466337194dbe4d2f7874e06fe25fb0cf856cb30e630b554bdbd737e7eefb552efb4cc1691be388303822916a592494a28b7cb20ccd8a3c1f2cb3552ca" },
                { "el", "532340ee9d7dbfd8a61acbbe5757b7184baa8e56754e9bbcb0d0d2acf038b2c8c7fc8d960134327c1246798db670d60b0816404d0bde5850e00137d07a95f9b5" },
                { "en-CA", "12c030359eb22ef5e722d3f5b20c97e5d4c40105ba14f96f89bca26448396c714b3672fa9843093807b0890be8ca2a4b1c0de0c32d8b4379d68f331ac20bbb24" },
                { "en-GB", "39c195b5cee57db6841bb9b9018f2b968cfa413c31aac51e41b16a1167d3b40440490237136e61c858f9cba9c912f873f94dea661a649d04f3f702331a89e7ab" },
                { "en-US", "3bdd888223b28e4a42728692ac01aede2fa50bd51533bef6742c39e983d3fff5c05683f571fa4240b04ddb961753186bb52d3843e9f267b56d92b04d8d1f5fb0" },
                { "es-AR", "1b688fbeb1289b9ddbe7458c68fa862f0c1df5896057387ea24a3807efdc96009af5cbadf410bba234c774fd4ae5a260b1533b7a5800f85158faa2ea7fddb495" },
                { "es-ES", "6ad66fde018451b157590e892bfd5ddc636f2d05b4cd571f9b0b225b7a2e79ff2a8c819cdbfb058662aa4739c0159f9b42dea1936d7b23afa5ca48878594587a" },
                { "es-MX", "0f856faf40b5a5c8702ee9cbc3164d050b6f18b98a39ca6ca99dda92e1e2bbd023915a0c94d018517ff7a47b8f3f4ca4697ec9fcb4f6ba9bc539c77d3aedd604" },
                { "et", "acfd4b2805e5517a3066450c196ff591fe7bcb39d8a6248a49f6c517576e30d4824be25566240601e9691e527932b4066227894b5356fc7dab7ca7f12e26f8fd" },
                { "eu", "800cf5dce711adc172b9b02faaa5469c51f1f3a4db984725f57b4f80ba2de90cbc9a5ffc4530d2e9e23f7dd821799f2ed5c74f77637b066ed66198db995cc035" },
                { "fi", "4a6da9ba51be0960c98c2a388d5802f56ff436fd0656cca12cfa6781da1c2687b7672d8f2dd9f66e31807d664b7991f333e1bac8a71533eaef4631b66620940c" },
                { "fr", "d5733ef8daba1964a30cb658cf97eacd59c09c80f832c1af4d8543d40adedca244da190497c62b47b81447f4db510b2ce2916452c42544bc7d03b15d76f1b908" },
                { "fy-NL", "20f61971abd4eb32b8001c795044ede992a9ca2c012d21c5f787637b5b5d5dae04d2a22ca4208baac0f5c82f317a0be0233c13b1eda208f6ac93c1f08e6e3237" },
                { "ga-IE", "0d2447db3648dd2e1f7523f13cb4e91c0f6ada5db4f3011dd8a5b800b5a010bcae30f2b584ded61bd1dea99b4b523e6f84ab3f48bbcbcae3a9fb70979f0e5717" },
                { "gd", "9c1b9593d5d28427c5b257fb7e5cd8b53e34ae9a964e1b9b08985cb521cd271b7fe8bab80c02c8b9fd273c4d06aaf9ca696ca279957b09ee1ff7be13cb1dc396" },
                { "gl", "8ac9d423034380e997eeeb91d1fae87fde1e1464f15b9c455269c8e255c83049ab3b8f619fea5ebb92790d26683e71a9a45bda251011cd8b2ee52887754d94a9" },
                { "he", "66fc293e0fac126358c4b976c90123937a1863296e65e147f4010634604db3eadeeeab1a732b8f515e978eeb5b77c08596c988e8125369a9a3f7978863b9e081" },
                { "hr", "b1673a94ffca768ec919c63fb75bcf685d25c91118cfdf658024d821e4da2a044455a0b035b029584a60aab5823f92dbc5c48b69f086b275c341c58866dbc217" },
                { "hsb", "fa3ffc9b6d2f8dc8d53dea129ba9c5b33491744eec26fb7cae54f38a3cdd04b3fd1058cadddb9433d9ec5d78dca6f405ff10adebf25e706033ea6618a43ad843" },
                { "hu", "75e587d0d2b981ab6384abc87add9a13a8d6d0f057a1f752a32b57825b8b5585d2e10fac9d3c415a1d49ebd4041357b78e41b8ab42d2a7fbb18f78a3b6db773a" },
                { "hy-AM", "5292f7897518af2582d622abebfb979f24d4228db2bc2265d63bd4cf37072ef0f9999d3fe2da7d93cf95e526d73411c9647de84e1fb95bfc56d35c53b382f499" },
                { "id", "3d09d7fb59a16867d65520a5d8740748e3e8bf3f2306058ed97a912b8db2f7e30408c722166e0a25fa89a76472b4a9acbd5363a22f05f79a6ef8da7df29936d8" },
                { "is", "ac0cd3c31a7f6200d0c7342902df283392a665a6503b0e01db2e94a2331ae23c806209069d4bbc41b2a7600b5d5d9a15aecac7ee78d65b169b86e53c5a90395f" },
                { "it", "43e5cbc4ae5b59f8aea6245bcb09d549fb00608f87ee9967459119d12577253988deabeac7208a8fed8bfe859c7f1f69a4451cb1d2de1de2093a9a6c4a50a30b" },
                { "ja", "52e1691cd92b6d2eadd080f675d219a0e15db622a544f519053c0e892757a0cd47f45f258fdc4e74fb443ccfb211de25321fdcdc4983eb855ac8e249f2296e8a" },
                { "ka", "542196d840a4f9b6ae22e5f50c243c5fef688c1df6089c6b6dbc55cc69239ec26933f380dfbe8c4b389f6ce779b13687815f313b39e82bf4acf37d5181f9c94b" },
                { "kab", "4aa648c227037b305580d40b6dc8b495bfd62c52b1f2a1ddc3bd8d5e5a2374fed3e194176559e65e37281ca95be64b26a5dd350def26fe0693e1703b9b8b057d" },
                { "kk", "9345990776ec0b1ec8556b6c8ef42af15224062a21eeefcfc467a3aafa944e75581d783f97905d7c2b8ce69549abb62406b9493563f88bcad9bad1a3acd98da3" },
                { "ko", "edef7e05c9e783abead7f03d9c47ac7d47d5b6f67d1e1adc24cb3aff50189d36c8e9737edfad1acb8cf91ee5b3bf2dd1f873102ac2fc41a5b2f35fd69470d2fb" },
                { "lt", "bc4d8b75df872d4444ea169f56ce4ed8ac356b5eb9250e74ba3e0d7bea3c760a863dfce1299694bcf6eea4ca2931b2935b51f22578b7ffb5fe8eff481f361479" },
                { "lv", "e280c4e4b4dc2d1cf3c4750e852e9cd14a1cda907d4d0894fdbbf0b3498c79f59bad66d5ad5c80589075ef17c0ecae624c2a0ab355cb034fba34b66cc5f05cd3" },
                { "ms", "12d2ac6ac69741338871e164bc792d4a354385388ab7480405d77fc26f26fded8e2c1db1ae77a8fad02811abe6488378a3d175b8d98bd9a08466a84bc47d78e1" },
                { "nb-NO", "e91decd2371347e5c880d95ecd6a04ca091811a55882bd8fd3205f2fc0854bb5fad891fa24d0e87672b28edd62130b1365be715d78da05bb8196bb29b35f4ed8" },
                { "nl", "c2c813b7b4e54c41aec490dd03473d562e82010d2a825f5cd62cbf1887d5ea0f023dbc013ba681e3bf6b5f19df32f4fa15e587bc7d6aeb8a294466fc74f85347" },
                { "nn-NO", "525df6104e5cc902afad9e1248d07ed8371b284f89493632bd6e6835dbaf4dec3b48361d7ed70a41b2accb7929b9a479166fa32eecc758685479e5f8a726d3d8" },
                { "pa-IN", "3cf8dd35fff318999e3011adad7ed4df52cb2762526270b34eb4ab7d543374426ab3a73ebbe273f7d4751e5e2a065a938b3c24749e135513a7725ef0afebf45c" },
                { "pl", "696bc594be4317daeb26d2b9130acb927d1704e89ee1bbf2e94fec4d2ed83d190687d61ddba1a12c5b56ea580d58ad88017bef648112e7475b573bc42ded911a" },
                { "pt-BR", "c7fc644cdbefe93849e147ff8c3d4e5276764f6f27257b31428f7a1bdb7aa54d3eda6879d302d3d5a81e65556fe6dd1bbe7de0ccc1036e2e409001c6b1f6546e" },
                { "pt-PT", "829b8952846d0097f2485bc687a1a1b9b1aba64f83710e88ef07b02fde4c1cf2912f8f15de87f5db9598c4e2449ad844dd724de60b11d7e779c1889f27afb338" },
                { "rm", "4cefbb4eba64fd4e0bf8226a427f296f14bcae0db9b11ebc14decdf3ab7e7baa7e9c7394a795475b9f223e46a11f73a1b2c1a79c876a4ad29a8e6b329454d018" },
                { "ro", "0026f0a117738d99000e6412b72428ffeecd1ee9fb270a944476e84e1a8972d83ab3352372a872551c7952f90b68389be9bf0d503b05c570716fd00dfeca451e" },
                { "ru", "52a6a7e81c5434af0fd8d5e334ccb34a7f9ad880ee3ad534618d0a2167967d31421ba9fecb322bc895dbb4b9ae8f2fe63bab80f3d5d122ae042969d38931358b" },
                { "sk", "cf6412e0d5935c08f5a156d20ddd570f127720c512e20c2b872bfaeaf61059891c1e6be85f5da93014104e1139f1afb99ad1ed9da4e22c75897513d6f8885422" },
                { "sl", "e53b6333e31ac66423ae9d260498d8b0ed25cbf954d9200de6e432c1e8b0728e1742d575ec92f5ee6acdb996421fb48dc653e763d82f68cd261fb4248f6b9cc3" },
                { "sq", "77546c4cad93be88d733f24ca8f1cc57f2e44b7372338acab8e1ff85e18a062507946e23813aa34507fc4af81feec5ee4c713278005c40ce657dfccee49ede85" },
                { "sr", "b5da632badea0ee479f69000f3ec62e57f7ed35f00ed467a03960cc87ad0a908ed568a08ed70deaace22a294dbf1b4c47c0c73ab1aed4b8b20dd451b8e751d4f" },
                { "sv-SE", "0d131a1d8d21db8016cd16cff53a2fbd51055702c306a74165b269b81c3b881a403efc97110020e418731f2882f74f3651a74e42c85c964fc1d03a8e0770068f" },
                { "th", "558c9b157c26c7a8150efaa715b1784ef9a9748a80e42a7883de4ab9bcc5b53037494a6020181b6383d6b8948b245753e26c9840276519e17e97eab08c0cefe9" },
                { "tr", "7eb3bd99356a87daa201e1ba6d17c885ed729154a417678cd110f7a4e4367a5e43fc4c8f41b2da3b4f2ec6073cccc24765751ef42400e19ec5ca02e0c95421eb" },
                { "uk", "988a3cb43c1f38271b81880ee55f14d0ca565e061de07d1274828fc8f19b9f68a6cb4f51e492eca6206348d30c6eb067f3713485dc5ec1157e963fd14f839d48" },
                { "uz", "b1e384caafa12faa0be272caf9267bd97383d2cbd6876660d915db46f5b0f1c0611502a12d8c86c421f2eee8332cbf23a375e72b9eb5ad44c185a155be453a14" },
                { "vi", "be2a5afd0ff330c3094782e1e129171eebe7372d58c5b22d1d0f45667c22331fa277dba0999fb1099a38f1c18894d7207a3c98155285e8525c22045ff4a198f1" },
                { "zh-CN", "e6e3ff3b0d04d2c65b172d9df4145de327a41f3be91d1b3d61cb51127b0b7870012dee504e34d03bba358a717f75c1e62671309ef6348eeec675edf262012f24" },
                { "zh-TW", "0d72fc4f230edd902714aaa9c03e13e56d01b13cea35644f18c69a86e2198b313967ce89ecc6d9e538f555e9e96764dcd41583f3be2edceaf310cd1206a67dad" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.11.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "e69e23c8531c8022ae9e69aace11ac221b45ab9f380ff0f6a4b2cd2bae6b56a9b803243f34b75bb1de2e243a8ad2de993b478123b55629684c4ebe3e1f4e9d75" },
                { "ar", "982be13add377f848ab415be413d0db36c536687b0fcd8b7dfa63c8f29043dd879fbff51e1f6c7926a88ebe72337861200b97be93ad2f44100aee5ca0c8cd744" },
                { "ast", "e48c14a03277393bf17df86ae9475ec9d0878bf28e9fd13423ee2b63b0b11fc9659d381c6407dfdd3e456e4cd8f409135f8b6a52da6be7fda975f221b17cf479" },
                { "be", "216817deba5e555b9f89990841572fbc36aaa06a4ae2b7cae6ff2f147c0137a1af06342c2f30a95b0217ae4350fd0242d46938be9b7537ef4a15b4f44fc1b256" },
                { "bg", "1de2bbda92221cda70584f4f10d3ba9b648bde79412e1112b588a9d7dfe216c075f2220b0a1ceea40d45e159dfaae64a87f126fb167c1f64582548224b143296" },
                { "br", "9182bc6a77cfbea7b913764e2d7621b158466c87df52a27a60e53832df8159362864d0975d56e21c823a649c72ac0eb8acbdd2edcc26804129d986391fd2fa30" },
                { "ca", "a8f21768f2dc164dd093ae05a6a8f36ba96e51bbf6bc6d2bf531617db3e29ceb0c2fb4706021ae89870f4636202a55821b17386cbbefe32a52f18f12d41d3b88" },
                { "cak", "9b22d5fa66491d477ae8d9885b0e9307d3eed86386ce40096482e44b5dbfb70ed388a1f6390405bd2829bb2605912b92291f156b17d3c90c25adbe5b62dfb7c4" },
                { "cs", "255529e86f82cf03cbf5a59468b0cad9bbce9d1bc2962b6f78ff724035fdda264bfda9ee8890faed838b58d8e5d4dacd281e9ea0bdd103e316b3f520d3f56eaa" },
                { "cy", "fa40fd27cf0c36d52d114991326c294e4f49931045fe83b03c17153bd18a4918faab074e0fdded77cbf4567dba6468f7fb73055c53932567da222915ddbb4f13" },
                { "da", "ac4c5e036def29cd2ac078d4613433032cabbdedcab4779b28e5ad80f4f24cd31e9d5e0cc19d39b3ec683ad3f1738ee06d503dd6e1bae04efcf52a7892a1338b" },
                { "de", "28e7b2984a2c0991a0fa0851ed5e1dfacc6ec37db75e17b302f9f7de3ef4507375b7e3c5e12463dfc86fa3627f470da2dbdcd29382ecfc22abe7f63be3c9e4b4" },
                { "dsb", "8d66832819e0761befc2b3ccbabbdcbfa470d511896da1bc33c489a4a6713d530a6eaa3ca5516389f5b1d71fab94e838e3c71e3023480cc553ee9569bb749263" },
                { "el", "a38714a9cbeabdb5ef7364bd532a0104448b086e88746c09565d1e95ab6b33c30776236e5286ccc3ae6b5f18f06189b2c7c4e0df3d06d894de08a2f9dbc405c7" },
                { "en-CA", "32936e85cd0a86c8dff441d7a83e3a83af8a4306bad73b2f391507ed2fafc7593e8fa720c4ad6be982a5aa2d3e898ae4bd33e201ed9860bd5cf2858b32b9065b" },
                { "en-GB", "41bcde502987d2d8ff99b0a561e00f1d457fbbafd394b4d4fe63c9971ddb95764b37a2a90ca52a629c21d8318d9b644a2e98b76aa0395b139710a53c3f9ef97e" },
                { "en-US", "5fc66a0ff6e9255339cd9f3510546bd914a22df00d4d4e05c89b40c4c220697ce29858e84d572045525df6e5bdc109ce4b30fe61a869c0adfb6b0d805d9172e9" },
                { "es-AR", "915137996e0028f09f87ddcaaa55ad35d551795c5a2f895b5c83362d9dc250b1734e28b8bf0cb31f02f2f83d6b4a17f46b4a549968315a10e5cad25c25dde963" },
                { "es-ES", "d984c90855d5bb48d6070a8f7149040cd73637483a8746dbc65457a5ef93e510060621516a14100d74dbdae9b301cbab0bed6361934ce9fd544bd910c4c62410" },
                { "es-MX", "f95f59c9a07962bbe1d6150322152293acccb9113c4ed25ae531b06c5170322f66f7567879caa364912b179cf37efbcc9d4ccf81e53b63fe062ec8dfa8726816" },
                { "et", "14ac71512d055b534236869d99a0314107e1681ad460b9f4408d2c633439dcc7dabb220fbe2155b6b5da56ab2b12ccb5d78e1646204079a9f41fcdf3323ead72" },
                { "eu", "af97d43e0dc795a91ae2b92c6e89847c2d48415461c4efc3bedfe698136b573946657b5cd39ad75027f8fb55235d5945e324d6f39646908a53fc1d1ea94417cf" },
                { "fi", "c5978c332a5d885973e2092527bc244f688257630cb677b9f955d32ce98477ae3266261b0e79600efbfe3aa511f264e4d8e38645dddfa1376aeb13dbc13971c2" },
                { "fr", "aa0717d03a67b9892520c5c2a764966129d8c54106e98b4f82d5305b9a802d6f4bb61f724c0aeb9f59464ade9be1ce526560841520b4a160798d0d81d3b03822" },
                { "fy-NL", "53c3cb997e962281e324f1f8c11a23d8d79c11a1f92dfec1d879dd055f65245252645336ed0144a70354b3b88edb48799d1ca8b03f6ffada3c8eb4be4fe57495" },
                { "ga-IE", "7ac2e881aca3a85fa4dd15f866a299bca25c0c7738ba1424ea4ec2e49143e1befcff6590fefe18fa5d467dc5491b15ee02a4424649b710020942e95f569c150d" },
                { "gd", "3e9a1fb8e66fd8fa5ca2926c8a746ed22a333f4408e1f7ac3238f059b7a3d68a59904fdd30918887a1da62c78bf8dd735a0976aa72d3ac55f1573e06bea16595" },
                { "gl", "347167e9c995ec453b4237d6efa9628bab6cc1fa6557a8c603a66b6c9eaf83e14f4a6c8c25b1180bd86ca12187bcf9e5d6a4b302af907b9ee272b35da267e9bb" },
                { "he", "997d0d2663e32dd8008b49ecb14ac95c3d4c837e127972c76fcb3970603b8de32ca9c3ad0bc5372abcdbedeb82aeb0ba6486858936ad82c56b33b3fcb6c84674" },
                { "hr", "1eb0ff6ad408ddcf7810ab6d79817d4f5f32444bf4457072841b875458ada8cef8cd231525a367371f3fa8d0e2acb82ab50ec070b06844d71cd6b90724fdc8ef" },
                { "hsb", "0d5ccda3ad791626f21742d61f9fed6bbf14ac730f06bbd78da88e0b35aa240ae8b2f167be20985f3cee2f6394d993f96a972a8df940ac6e37f89bfc742cfa18" },
                { "hu", "d2bfa3c649f966c846ff69687f4202f242275e6cf66f7b6ef9fb438b1e32f2d4b9491a653a2ebe1f6b9b8760101269cc4012e72752a171fc1ca341bca4ae78da" },
                { "hy-AM", "26fcf6da684ff15b26eabeb42937d4c6dec442b07d03443912fd191ae8f0702dfcb15ff7a33f0d1893c3457787ceec561c66d53c69ce22a75b6153598932f96f" },
                { "id", "06b9f6a0af54ffba9b941c59f14fe2b4389ab80fd89c38bc23d5779e0c5997b0816107c57090105453b490f3c9f0a6521aa2a2d68b68c21f68367a3e3f7fe83f" },
                { "is", "0d33cc00eb6d3cf61c909acf49c5018c3db90d535d7b6e81ae8197990d686f66d610b47c744b0d1f370c919d700d3f25a929ba5d1a17d2c345c64033b03a4215" },
                { "it", "bf912ae935963e4561fe768823fa2b70b72422e1a738f04a3431a67fc3369fa70c9ad40ba2ee39922ae64aa7a2be11f7fafd4f496583cec532c59cab2e776bca" },
                { "ja", "1f93eac238ec9d84a384e13eb4779d6988a34c06f199020effd38c809bb6225fa9e1d11161ec99629d0798dc17727e4db9b814891fe9c8096dd7ef0b83ded6af" },
                { "ka", "ea68f24416939baaff89b005e7f9ac9da81a912fabfb73d90bab92bdb86451b98b3ea541e0c79d84c0f03a2d6f63df05eb3e39fbfb2f5d92c419244685398575" },
                { "kab", "a74d84523b29b7c788e27d5e7c664f0805701dff468d7d0a2852cfcdf8a7be189835f9972330108f4a2b63711610824237ce0fde7f0fffa42aad588e1ccc9fd0" },
                { "kk", "62899be1703847138d8c0c33d64382733a110d7aa913820de84066b4e7c3d355a5b3e270c64e5f518a2b20c8f2c83fcd942fc733c073dc990bffd3298c4a847b" },
                { "ko", "4e2aa9b00d7b986535760395114ac57c194e14cc15b49f0aed3bd925995083bb2f6a1993834f3d25565756e44b45c9850926fb80db1e1575a49ee9b02d329911" },
                { "lt", "6744ddd8f1dd95ca3a83046968608dbe13026f9eb482a1076cd3c32a17d6ae92071f2917c1c2ba0a8adc9167d68e9f696f1816f1fbae2ee7429fde2bf082dbcf" },
                { "lv", "d83e02429985957bf5cf4870d4130dd3f28a1f5c258ad4bc55d78a8689cea923c2173be2c47ef02591a12f59f154bd35d36792b7429778309ec525c5d156cbb6" },
                { "ms", "144cdbf3ccf8f459561fb8875311e7c0e659a3efb5a82f7da9c9bd14858e125bab6f572b5be28f76d83dae3eee7f0f06b2e2b11095af982fcd473bcae6057257" },
                { "nb-NO", "12855b61382acead7b527542962645f4015162170af185799e4a91607c27669502af1daad686d07731951074bf4933142028e9fd8f8d2960e9a9ca191b884ed4" },
                { "nl", "c5860f5c88ef32ed2975dacef97dd3d6e052a55d186a195e5f6910606ee1db8622ea0cb1c6f558dbff2a45f0904fb8327a0d6c00914b7ff7590a00c6ed9b0644" },
                { "nn-NO", "f55e7837c6f308dec71e143d25f3c894832e76759380a922370c4dd235b89920978b5526898962d8766e57dd4eec4da10bfcd5ede029a06494310af60afbf810" },
                { "pa-IN", "df749715857c5c704dcb07147dc50f35411f53159e67f379fb570a4bd435667b6c0854b90e9598342b51f23349cfa3adbbea7712aaa3273e21de5a71c8a8fbe0" },
                { "pl", "3e7cdb3fcb0a611cf714c2c745af1204d006251c23acd50423198019a315b0b0ec1a711e33947c9085be6aa3dfe1ac9ce55e1a9aa1ded8eebb7c772debf2967c" },
                { "pt-BR", "f5f82435b1c854e5a6a2ecfd7a87b1b791b74180fda664309f09a8cd02bf016526458d61f1539b3935887fc2d61af55b70da9c9d294186b85a3dd533ce9165f3" },
                { "pt-PT", "0bb487979c052c15d5785e5ebf981087010a704e148b3f6ee2e37402714e461ae794efeaf5e35767a00e5347f8658d12cb1c58be8c9cc6844050b49c9777a50d" },
                { "rm", "f8f971733ec33ca633e2df67b304ece9adb59c2e35760dbd922d2a56f8f9ddc54fd0ded572682a1d6c614a091e0cff78b7f438450f9b4b2526c93b0fef2d2790" },
                { "ro", "077685bdc97fa8624c4b0f982f6ab8872c83b4499bb3a7fe1da560ba27234717fa2ce930878293bac65b5efff8dc98d0bbc43a8f2fd7d34dbcc5543c84dff9fb" },
                { "ru", "562dfc917fd13549fbcfb276594eec46e37ed0f46fcfff2f0d08a9aa0484d2c3747563c23831d5031541ca647d6489da283f2887011db2ed58c2788b95f9953f" },
                { "sk", "5dbdc52e8f68c6c63b2d23dffcf606253db4a987c800ff4cd60d9813d851ef52728931bc084a7ac6594b41101997b795547847a4ff836d4a15ef97a49b983073" },
                { "sl", "e627906a5105053e26e6ff792791dfd5df194605a74c2c1dd07394b0c4442e8919c8c1e07fffd6561b4a1bcc0e0cf77ac0e6c2b9a46faf1fc9434a947af531be" },
                { "sq", "05faa2a3c131c78f1a61a4cdd0dd68b340579204a1b3bc8a957199486279cb6a2262946fabe12c37026159068656a23849efdbe415a2cf8650a6f9ab2495ea8b" },
                { "sr", "b9122f85d515ec00dd28d3c8f93bb5a5215cd4ebeb4f702f8e3e3792615284505514f2f9ee148a473f1e593211ddbd27d018d8e381e50cf7c13217fd58b3d508" },
                { "sv-SE", "06eb2bd48ba98072d5793ec3fdda6c6f17fa36bf3fcec79053523ab8fefe50868fe72ccf2cb1fdf0546cf38114d78cb2c765e85d0e1bbb2feb025e6f1e6aa3c7" },
                { "th", "84b4b1958fc7772a85cd94a024651f35adacee2bd9dc13eb3f5a3c963ece4d479b123f9fa2f5db1f55f3840474a564e48db0f319533e27da83890f90a80fbc55" },
                { "tr", "13d24ba71ca71a6c7e046b9763a6ac04904c2e5de9480745180b636160bccf3cf8a4234614e072707f7c4c03e97314197313dd53d7cfeb5b0ef082bda4788fd4" },
                { "uk", "4fba5ba51036e14ffefd89a4ec07e6941eabf0993c407efc40a5018bebdf2131fbff2faad9faf9d62b59156183654a6cd1450e8d9d62842405b833fc33e634e8" },
                { "uz", "e2bd5a09bd24d1035949ba0d81c408d8dff021778459d21c26e515f35af1bad0767542bd762ba41a8b82a01313838fd1caa430a0efafa4400d4a80ecb21c5b3b" },
                { "vi", "1d0bdc977005b88dfcaba97e7d4c34e7d1c67a2d8548a3cecccfa0355ddaa6305834e949ff6b821b3791172613b0de94d13f4358e0e43b8887496553a3818ce1" },
                { "zh-CN", "f3016e8b3bfc83ff774a109960a4af383e0de92fe3d2e23e7276e5b6408c7749f1110b37b9d5120e5043e8af9a305a483ed49944c3cfc8cf2037f473004d727d" },
                { "zh-TW", "9b8be17b8977d0c6843ef2d0cd47a1402cf5fd31222e2cad26c28ef11db336422c402ca912d8b3c52cf0e97690ff821c4f0bb067ca1a698074ccc3d26f5a9140" }
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
            const string version = "102.11.0";
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
