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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


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
            // https://ftp.mozilla.org/pub/firefox/releases/126.0.1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "1ecf78c4b18475ccea254bb9cd54b6c42154f6c89c3ad5b6a42bd007924e00a305cfe7a1a7081610fbc924fac380c950b01d1a6d253831818a7b73d931424cdb" },
                { "af", "9693456d20ffa509a9de7962153e42f838aee88e86c26c4d88696097032f4b54338e70d97b2f2f62ffec778c58a386567935063089b5362e6b110c9d20dbe2a1" },
                { "an", "f15e30dda227564c46e61af2bad733f249e566228f6e42c373be7e619463574e813191fbb7bcc2487c9ecc621cfa68f485166209917107688465e43cbebdda19" },
                { "ar", "53c86a207420a08f8169cd6ad6bac309a751ef9595bd5697309c7aed77965bc2af29af9567706c1bbe47cb5ab5dc0ba4fc3c1847000b0de44587a804c0190318" },
                { "ast", "8a57e97fb97ff5cc3aaab067943d82a66f6104239b95ea366ea5d7be9f0565176089f14250750a056efac1808a760b7dbd26424f9be7ebaf754ec560048fb851" },
                { "az", "22ea018c1ff60cf93a9eed302e54f68ae7299047390d9bf17e17b397f71705a24267bd0f15e56824d8505e5c4fee7345ba15460e6631727bfaa8af2cb7abfd75" },
                { "be", "6151cd14e665374b81fa8d99bf123e22c97e3a3f63a5288727489fe40622250db14dcea72d01ad359645d831c0ef4e7a8cfeea603eb4063166889afe22dc03a2" },
                { "bg", "54d9587fe5437843c8c64b8adfc9c023739ebf34182d74b3a75d0715711779409abfeafe84476e26ed8c7ebfae4eca546eb60c8787aa37f56999ae5a8fdba261" },
                { "bn", "587d415bf42c177caacb8d913401f565a8556a3a43e78a1c900e4f7d3d6a3fac41b32752a81d94aff71cc7a743ee1719c688016501e2a9a4be5fb90564063bdb" },
                { "br", "ec77f5c109128da878e765c7426357218b8a242459e23b09ed38555ca8cc5025b1109b652144c88037ac3dadd3ff4ac3b2b81a6fda86c190fb0d1af38cc275b6" },
                { "bs", "6c19bbaa480bee6a61363b4b3813c22804072bdc6c844009be8b9e35931e7e9016b2ccd0cabfa3995d778f9be85b0b6adfe97d7937ef9ef0c2a5f038229e5c3c" },
                { "ca", "a2ed0f88758cefd8491eda52fe7425b6d00493d1af71f52ed0132423f07dd19fd57cf2db3fe06b27b03699a4dc25792e4481cfe861e2c600706c15ae0f6045a6" },
                { "cak", "e7d92b68b119374764bef0dabec71adcbdb8d024c514c68f340fd373fbadb0a5e5763a465090cfc5028460d2bf3f2f320d239871330696f062a7520c5b9d9eec" },
                { "cs", "46c87a99873d1a046de28b87f9aad812be23ff5900cc1a549c6d0a1ac0658848d90cabcd1ce5cdf73fa09bbf1cf0415ab11ec1d57ba01f7dd7d77d4e2772fa96" },
                { "cy", "8674f383dd3212993ccf6caea54a15cc7bf0dcea445395e7c7113548b137827fb3a1faf4f4f55ea7cc2c88803f769bcbec2ecdc344c1d89cd852cbb482c796cf" },
                { "da", "b1602b16253d41f8f747c8427cd24fbd0c8dbab8ec199727d7f4ca206287e33e66fd1def7a33515209630aa45c8978e191b907adf29ee3fc8686e639bee60f7f" },
                { "de", "8c188a2569949a09cb8531efb6c029d93c99c02a390d1fb7e3cfd0daaaed8eefc383bacd1d33cfdd7aaa64e5fc936e2ad67aea799dc22610849e0bec4cfb0e7f" },
                { "dsb", "4abd94112928d9a6493496c77640d24094afce47b05bfef889c60fc1d6bf586dcfa7e2c3285fa3956731a256a704762b6866a285e55556c77c5fc32c63b7ac17" },
                { "el", "fb768cbc032c8053ecdc66272210fdfae7014780964ae4b33bda280634bab6a5720b9bc3b8de3cc96a894a015dec4345f829afaf1b98871e59711cf1b6116552" },
                { "en-CA", "f8d1d431accfbc933281628475502cbcad3fa72a1f73618f56769f02730543bd2057331fc7d26a76877b49efcbb85bf4b3d33626f55a0ef9d431002da57f4727" },
                { "en-GB", "af2d8c7d0b8d1780f87bbaaee7f8910c42bde373ad737c410e4ecfd0cc05e32e597a0450df365880e3f42e93a159a57f2cba52e25acdc65ec8bdfe220811e791" },
                { "en-US", "c04a119937a26c64367e57279d7f21bf529d93149eaf07fa1ab136f0a0cde98a69255ff2714b892f75430e6820fccfa80ee163b5fd6f8671979d4e4540a67d94" },
                { "eo", "59da018424b8f81cb596c75cf2fccde72678090047471f1da03cea0aa37452722977142110426347a4228362d51c8f40b41955aef15f6fb5dc31ea050f2fcd32" },
                { "es-AR", "c1ff015e3d7e084bb398fa583f2d99c635940bc8f063ee79ff897ee47058a3cc402054f552661cabafcaa7a10109d9c1a9b89400872f64ab4fff4aa925920350" },
                { "es-CL", "a2cac155555d3cea3cdfae2e04f126329b2b22cf1463daaaaf0a355a0d33574b3d9a81ead692f3cd93d4bb7e4e3650a78c1ac593b38540578ca2f7f8f7308c8e" },
                { "es-ES", "91b1be55fc95790e49b971a1c457d851474db86e88ad4acbd36da4b8cf2f5ed5d0001317ec8127354dba2ab97c81d225d9488bb43ddb083338c3d4ef7f982a5b" },
                { "es-MX", "539edc1a0397ea22f64fe9a21ed28ba146007e6fbb2bca2e0986b69c384972bdd225cc7a2762532e8a0962e97e38458a60f06e464ba01508c3f6d491605e406e" },
                { "et", "bd11c1fb7d329d441589f64d968f8138b38b3c4a7f12355b0b7b6d18749f455cefa7a6cc2e5b35f5a6c9203fe1f8f94b5a38803665ab5e2da4e8f1ae0188607f" },
                { "eu", "8b0763a930b2575c19ed822676a609cdc0e1034d2e0644e0f74c59ff3aa3cc13fd4981d085b40834f64cf15d5503562909a88b96b52ea88d622d3ec67bb2292e" },
                { "fa", "65d2946d262e4e5f0e3318b4e12056f4ccb12d810831f22939ea0131cd9ea6d6db69555aa4896807897735fd6aa5462fbd7499dc5ac320a07a23c660f99eed33" },
                { "ff", "87a3f6fed01d853cc1fe8e7d5b4a8904639dc1afbbdef23fe52cf8f17dc9b211626a94392b8d29043777710be35a422bb19dd84c19ba6d5f7626a630eba3afb7" },
                { "fi", "99635f2ff4d49fc7a76b6cab70329f8bf6c91b6386bd1b7ccc9a2063efdfb3ff875962971b57d03f4a33ee2abd2ac62d05f2f96a7e69e66bff46e0bc1c55ea77" },
                { "fr", "281b5412d78d1153c9647d29faac4e9c66c935fe031ae283c3d9627e7b5a066ed50261cbaa6d1d1a01ac61099b4c8af05a884408a56285eb3afa8f849c2bc2cf" },
                { "fur", "1598a1d822d79647d5e005598fa32798c9d408bbc2d4209e0a914e8395bcb2e014969619ff863cc118bdb50409f954573cbfe873e50c4d37125c447452e263f0" },
                { "fy-NL", "c5fec585eba0c2c2daf613e99d5099944f09379ad753ea63fede97a0bb36ad9cef1c13a697f86759c8f1faba967aaeab1656f2e80535ae23cc2f0d5e069060a8" },
                { "ga-IE", "6b3033ca9deb6bd4a9075901e8e8cfd5cd973441c9a57e8e1cbbc7de323838bd043dc1e71bf341440fce4146a219da072c4015626f5afeb52272a0d92903298e" },
                { "gd", "f794c2bbc6ba331368c3760322e06ab2f512db16f14c1342913ed8e9f1bc84a44bca16e554ab931efa67dc7d7859c4dc95e3b869acc6153d6d1cfe45fb8332e1" },
                { "gl", "5d69855885e41f406a4fde67abb8d4cc0450c032c054c3477cb918f6814a96af47e77d371223f44464df5b2083a2f53d44bd574643c8d20f321161f8f20eba28" },
                { "gn", "6239ef3dea627b84cdce6e8c75d1d5c30503f82ed755eed44eaf5aedb81dff0826e41d01a16bfa0cb3c57ae4c0616470681ba7a3e3d9120bc863955474463ca7" },
                { "gu-IN", "a2701d793b7fce9818626f7473ae8f95e385212918c049bc9074ee09237ce6377ad22370d7ab0393d3ba7e938af49392afdc63a303b365182fa731a6312e3ec4" },
                { "he", "88a727f8760b5a0bdb21696b3719872ed52753a4edf4c48d8d330d233593cabeb0fab6f6a8ad42b88a85e764172d2b1b89d999f8ad6561a3746eae2b578c405a" },
                { "hi-IN", "734612e24dcc4d685b6fa7b2462b688ee7ae56922c8d01506bb1c3355905e2d18dd677130b66865506f126af42f542471fcbf3f2fe44ff9099b83f2798f70884" },
                { "hr", "c92697ebdb219adff413da6853a8ea821c5cdf4c580f6e437b68b8a38b022c1c38286f91ce9548ceab7135beff2a50c8e07355fb35ede65e02d9497a4e3d8cf0" },
                { "hsb", "e4f0cea570d21f14a3ee639368ce759a72634c998bc7f0f82bca401da0d96e03275ac5f324d9ccd0f8307d2af5885609154bdbf7975e52955facb7d2d16f8b98" },
                { "hu", "b3a67f7a290fed209d1d4bed349e9146126b3843851718c064e4a91ec8c637af4470bf92cbcc5b54f3f58794d4f050e3b754c25423431269230493cdb15f3086" },
                { "hy-AM", "df28e6eccdc94242257ad1f7506f131a2ff786dac9ee9bf72057e96e0991d24bcf3c622469ca44eceb01054d7968851c62e5c8b14e8d92f5614e1065ac7d4cd3" },
                { "ia", "791ea3235eb5f36d626ad99373a82ebd2393487236031531ea104363299a44e3331bf8cb169918a20e6ca35952c6967f7fde1e03e5572938e16cfb42eb531821" },
                { "id", "217560316309fc1fedab69089c506fac73a2774d3a1cb22870a23841cec00b02346a5b1e4808f48493e124d214043cce8892d6c8ae26a337185afbd4ce014afc" },
                { "is", "a1e8860b74bd3c0b9cf242c65c56c77749f31395758e0c6b815db6adf6144c6422ca4259b4d1b47767cf0109ed1c80f9cea41c176cfd2d42eb424d871799e50a" },
                { "it", "13462e154a673733cbde45c3900c2e27dc0acdf8aa3e94fdd04792d927e96849d2546ae21521d928f33bc4b0e346a8c08248440b1dd7daceb3ff3565541f270d" },
                { "ja", "0f30fe881648978103d46beb680bceccf362e145cd9a624ccf1f2c19f49612aa81704fe944d3f430e946079ed017e09cc258df0f5118c7f89fa0892e671dad00" },
                { "ka", "5132540690abcb4741b562f636b7d6420f51d76ddf6a970660c1620e22c6494f955622441fdcd4da8a9688a1a1e31e400387a0a3ace19610e917a8863b17f183" },
                { "kab", "ba00f1bc64db597e4888f19270a5f4d453b0514f8fe1c8f8a43a6da65e5e62455540e8db5a999766b067cdebc9dd01640465f45acd1e8281e26b8896d6ca945c" },
                { "kk", "de60526ae44c007417127a3dc5171cb37a948878380cee57edecb96f1a32e605a17e90b4c3f76c7b59f55981f896f408e0bfa7a38a0ed2516a8f7caba0233c88" },
                { "km", "866150d6e7f01f3b218e423b4baf1ad6f50327b67386dc1fcd2b0b3943fab7b377e0323e0051e0f0239fa11f544a39365812b80bd6d21e92f264c7ebeff4b1c0" },
                { "kn", "e238c11217ae5dc925f312417485b6fbace9762fd309947f5faf8110fcab6eb387011e670588a35de28716d4f26d0c78453cac6e0218221ec893e70ac44b97f4" },
                { "ko", "ba1c34e2b752afb9f08f45d3378d1bd31f0e4cc46a3c401383579bf78197d5f39243fedd4fe1b27ec7de7464481e47e3c80ef6dc176522a5003366ba48db15eb" },
                { "lij", "78ebc8f2d300591238b24d04f1e40601d59ae1af3dc90544d071630254f8d3f2bc296b8b9d89a86c916de13cd3bd8be3e8c7c05ddbfebff614b4a8a01eb43b21" },
                { "lt", "320e4a421b7f0f54e765dee7934aacdee442e9fe98c5d899d062c9e6bf78d91a52ec6face94d48521b27b424894411e2a36b9d863045f6955b9a410ea35cba50" },
                { "lv", "efed8314c504bb6d0056cd0260cbddd6cb59d4dddd63f8b9159d31de16206904ab5a85337537c58fe6a00a94383f11d34a4f61023f92f3a3e505ad2fa264372e" },
                { "mk", "da2c6b5817b2af58c979e221ae67935aebea38fd087db646166232fed9b8893cf34918db9a4db06261bd25fad0012dd78cf7dd45f46dadf731f836a5345a3272" },
                { "mr", "a5255284ee81429403e51a0bf7f8f638658893625876ebdbb6c093dc59ac182baf7787249979a2dd998ed2f44dbc0e0257a7f82acab490ea6ac6b1b6ff09491e" },
                { "ms", "9eb6a740fc9b6879e1fbc0487addcce752b319ed7204412dde6782f44e8d5741e477882fb35f345599dafb0de959629339c4c408dbb244778952e327bc975b16" },
                { "my", "11b5ab9bbc71a730666e2a5e0d3fb6d0c71dd9deeb1b1ef3ac23ffc8f6d092bb74883c63ec2711aa90be854374e6a455c7bf7d5067b53a6685dc4f129a1f4993" },
                { "nb-NO", "1fbebe37ccc580d17dd4352162f3e87d883977abbc3f63cd24850c376e8b63ee0763e471ef9b1f27cf719f4376931ab4a369a5ba1124bcd254f6e8ffdf956c33" },
                { "ne-NP", "41a6295e674e3691612f95d129fd4367f4a5ef5a100eb8df29e923e430b2b3de1d66771dd1f70138dcf239f3761606bcb4ed8296822d92b743e7e960a6d0f159" },
                { "nl", "220a12356c6b06e2d4738122ab62cd3b3ca7476659f726663b5d53edccc3d489aec7884c7e9d48d63019cff5e5135bbfed545ad440f7f3086f8de341de3dd3e2" },
                { "nn-NO", "6797295154ca085c767b12e4294e38da79ec680859e5392ad4b24cadaaf84d7e084784dcf39cac451af1869b0e1bb93f619653ceb28319e8c325874237db82fc" },
                { "oc", "abe113034acaa0356c7afcea10ba721e0135da73cbfcba66293c7c232cba681f0c1b775f93be03c3ad18f31904e8bc25e906467c44c3559730172ba423e52c61" },
                { "pa-IN", "eada898677ff478522865003d43bc64c76d0c398dde62f499243daa9ada7b4e55ca58059dd7ed5cdf3e4abbe76349524d36ec27b50678bb8a945d28e972e1287" },
                { "pl", "c6ff9114bd4a3f97e9b84e1277aa2705702acd01a66560d1cbcadff0daa91dada1587caffccdd3593542d4b302bed2f903908163ab77c3afea812c497e5685e2" },
                { "pt-BR", "45bdc8bca44590bdc8d1970779af2eb64e0e6f34b2700de5bf6c69fa92e8a1e070680a9ab4a140c7539715ec128b85e008452724f56d980584998a2629dc770f" },
                { "pt-PT", "c7176437755702d8045110261f46908cad2c13667b92a945904932e0179339e4655a52900cff61e0e4c4ee48839bd0f4311b051cae93cc69d73509fa83720a81" },
                { "rm", "bbaec3cfcaea05ec1dc0ce2f4ed038a2612629abf1d1e52799bcab971a65891e4764664fd30d3a6ed1d4423761bb624083fb558c98428e36a7bc5a36ddbf8a53" },
                { "ro", "364c19580f292305c5b2ee391a849073230ef7076880883c2a696a930bb714ef69a8460f5914d1c6bb42347d114428cbf1dd48d45a15f60fdc0107480a96d2f0" },
                { "ru", "766996f627b71a84bb4d2dfc2b99cb78753e256009e93108a7ab92e3eed8c6bd24b9fcc2e2cabbca9b72880c1aa14e896172962277bb86a7716881aa5acef322" },
                { "sat", "000942378dfa7f27b7341c11b680bf622d258be23e927a3df6caeb0868c42339bb4fc09fc2d65a9c20a27f126e0d51dc49d93d6e342b0c36ee1a7cefda71c7be" },
                { "sc", "b08034f19e246b9344ca4011aa628e661dd178472f0d5e96d514766c68852283584f8a44a5690e603823a8d2ca8385170a5cd06ab3e1b6580ff14f8dcc3867ea" },
                { "sco", "190f4fb09f350e621ad2d7da381bd41f146a2ca6c2b53a33ae70a6480c1fc023622f0d459564b88db0139e414e6193ef3bd4a35cad4c23aa6371f285fcd6eacd" },
                { "si", "6cb5832c0f94f4ccb7ac0219bb6ce587e52aad2855a88dfa71d9605682efb0b1ff3bc850c4ad7ab1f76a336373a821246502aa7e6bb62bce0092196a008b0a23" },
                { "sk", "913d6946116604dd69d5e55d7f00e99bb6742655db77ad069a721b6ba32ed1d82e1f0b43d8ff36444ea6e3de91f3f4f1fbbe487a23e3088cf0418c30f4b2f860" },
                { "sl", "3daeec95433d1742e7cf2ea0de29c9479324e05cebedf654bbc7efc419b0ec7f5db644fcfd50bd1d7029010ee9c6e62c1a67f05fcca882ef3d1d2ff87e100793" },
                { "son", "1567a3e471ade6b7a014c47113c0afd0adcb1401ac781f02398756b5a39072a00825cbd8b015638394fe8cf266d769f19c9284bb68cfd1a0597ecdbe1df72bf8" },
                { "sq", "7e8b399c4bbc0ef3fa17f81b1f4f05f07e202143e6a9690ddd6c039daac9b38ee2d3714d57ff2a7921b30e7138c25687427727c2ea661ff9b1c3242975928c26" },
                { "sr", "fc3e1090b55092bc948eb59b2cda03a66ceb00e5e4ed74ab7e39a8e294e9766a10973505c98cca27828c28570c6e190512c205d3c98252345b3565e94c0f3cb1" },
                { "sv-SE", "c5715d9904ce257a757687198e4b30d4be6a3263b69269be2a290972890bb2c40e523cfd8fc9973c72e009a4f3df6288fe66469ddb55f30d0f9655b25edd3e8e" },
                { "szl", "d3a697e840144f3c771abc6c7ea89def7a79706509ecd1b47b368b01f3dfc4d57f855dc110e76e15152e99d2346c6dcfc13589a0f69e5651fa904f1088ae8918" },
                { "ta", "baf61cf2b205a5b4298fc1975204d7c6aa65b2bb1e4cf9f2df069147d381720c4f2cf79445029a70c670f4e1231a0761c68b96cf0af7b8809e2a98d12a05930e" },
                { "te", "9464d3a340173e76bb8758604f9a80f78fac22ec1e0f3b64fa001b39f152525d07c68719439267b4588352876f2c4dfefba181bddf06eb20278cb9d678c015da" },
                { "tg", "672fe362937bdb4c869ef98fba95077a54d08bbbb0cac387b627a4dce1ad44e0edae66a6b647bdafc641280dc3b24ac1623de44347a6c74213a41c5d260062de" },
                { "th", "da8ec3cbb13cf2e19caa9065a80aa2bcdf8d5ffa3e545a78d896b287887e07adfa50b9d0c87f938e2e6eade3fa7b1e3c612a49f1560b5672ff011c2f3fdc1fcc" },
                { "tl", "5a71c31553e4ce41e58798f534b164b237139235c4c7a699b4e746e8e530ba12677968555e5010154dc3b5fd2a64b1fbd0f517cd68c676819cec3111a821ceb7" },
                { "tr", "06e2a3a1149189de2becf2ad07efb54981dc487acc7973c7fc43d386c906b701a98d1469d9a90e4797da3ac8d72f190a9962aa1259667c9a4001300154fe2c01" },
                { "trs", "ca2673387d9787cea1b6e9db60dc74ffccc5114b4a74b74098c72cfbcacc19459465927cd6392e4bafd97a5042845758ff4a2372c986ad46208045da8daf55c6" },
                { "uk", "21c95cd7ee34525f72fc7a88905a69389d68a8728c3b1f2cef9b5180fc39e7ad240b5151c274e0bb88ea99717fc639621fe1f12f09ac4c203dcaab917deca4a5" },
                { "ur", "278b05803ccf46a1911190a8fdb7b002979952ff20d76ced5a4d52087d0cfc000bdae56a7e46806912be995167bdce146528e378f27381776cb8b11c58709519" },
                { "uz", "8d0cb7298295267d527163dc5ee8edc552f8059f168c88e5817e5de94c0489d026bc5906944e61a88fa46b074e9ddd49a2e04ed181df1800dc40fe548ed67b4a" },
                { "vi", "00c13a5b2fe268235a02e75cb70a6755821e1716d22be6baf538a3165e39b29c580f8e795e43c393d1314976a41b030f590c70ce926e92126189b8704ae02ee4" },
                { "xh", "b0ae179f53dddf944ab2e566dc45b71b6c3095154aa41ccc777191c49d0c9b48e4a8402abf93804df2419cb3ac4eba40c3a623fbdf99b2350ed0512a813dbec3" },
                { "zh-CN", "2a9bdaf9b2a5a8b8f6628c88e1f72457b7e67517b742d2f1485fedf86e352614134cd248663b3781b4a11909d4f4fd1669284e823045fc8328c3ddc0772b4b98" },
                { "zh-TW", "c37c7e376e54ae9908172369b916051ed3888e5c452c0a0bcf36cdb7f783922b9bd797f5144f0708cabca74f6d663462e5dd539339d4507bec9211b4be1fb4bc" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/126.0.1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "342edad5d377d89799a5fd9e53e93db6c9ca504c0e01056ee385d4d053770f3fe1159c83e395f856288ed352f94f48c43841205dec5e16aa4de063bca01c2fbf" },
                { "af", "c08186cbf940d94fec54f33ca934426cc6e7a068668567531f680370190a56bbb0f75b7863307c82383526db30fbc00042eb21aaf47f192d0e49187e26919d70" },
                { "an", "40c3dc5596ac8cb9c1ef02e115f19918f5da9e7e47da00099444a9cc0a60a1e398dd4f30911c35e7736e9c3be468510139f76813fbe312b25e747ad8b1db63eb" },
                { "ar", "22bc5db4a8720b7497ced9e0038c4f7df653c76c943beca1050c8ecc2158f51cc13b8cd89af9bc0639a5e44aab6d881debba9dd580a60be65757dcda639e7c3b" },
                { "ast", "a176b39ee2cbf5f21e1d12638a0876a98200de5d85bb069b2996f852c9ae09def08a640201562932094653c7adc27b4608479c33662921543e706bb34e421956" },
                { "az", "214f978fdb6e72ba782317f114662dbae7f83e2cf8191f3f8e57c94648ddd5bb07952d0204ca5f9e4a369ddca2c0df7a75fa8513df0e8852b8809ddacf31cb10" },
                { "be", "3c3c6fc8bc12aeec8760b1d3df4a540c75d62303fc9ad6b16cc286d6e8ca9058ce12842ec988881b50eb3a967eeea666ff782e6889bc984783f9ea4a5d9d35eb" },
                { "bg", "e1d8f2f95a755df9fc61cc8c807f2b327fa2b546183cd3200bd7bcde4cb6ffb3c7aa5305bb7388d0e33d6dc2b4730c5e47f1f52c3bdc90d5d6990b29d627e0b1" },
                { "bn", "0367ab036268bc7829ac31b0cc91ed66bd010db5ae29113e8cee5dd20083a4ef4fa1440180b72fe6e0cf0eab5dbf180b0d004e3bfe37d429b796bc61d22126c6" },
                { "br", "0cdbc635c6e9e5e54e15e4f0a8cf3e35b949140b4270f41c35221924eb83a61c20ffc64cdf6180baecf4ec5daba5930b34c29a01c5c70eb7920bc2ef8d69ee76" },
                { "bs", "169d73b7425a0aa6bb114e6eba0e4d4ce54723cad6d1c4780bddb0c101c47526433b9cb758855399d2919d4dbfd7bcef077543cdf256c2ae5f043a749c6e224c" },
                { "ca", "b23d7ee76139c8b6531da9c3051c82a5f0b7a6375cef4bc10cded259a088aba9cd4ecae82b7024ca8160e3742c4cf4d046d16cdb6a029b67c82459ceede0949a" },
                { "cak", "93b8886dbae4f31b30401a0f50fea4f0b49a708972a63896bd78ae161708d7127a6cccc0e7bbde70e04346d360e3d83057ab1e726fff93548992f241133e753d" },
                { "cs", "9f35464a3dc4c8a073bc4d3a0b28abad44b9b33ff9d3eb766916f3c1a3de26462e9b77cd4e0209a900eb56a6f84446e225279e3a44df5570b5f44a4a7573cbb0" },
                { "cy", "956e736a51cb9a1d2b27c7c7f9c97b72a44fcc15b956840543743080f1221ac26778e72a4db3decc8cece84422a313cb0d083567d8baa8edaea341f53049d72f" },
                { "da", "718d6d2d529dd12113ac9fb61b365d47644e0f8df6b67f200a5e225ad3b4a54044f369a7d31eea330281fb19936c7fb1cc37e6268ede1e236517c848ab947fd7" },
                { "de", "3e5fc0381f1407d98a95ac41c163202a0efbb6a2b7d895e4546ffd4cff3665e52359fcd025f16ae426cf168be13cf48cba3b4cdec2a184660c0268ab1d468db7" },
                { "dsb", "8932158afac4cefffd0c5dc4eede5a718bb9d0b3e32b84f70bec94e959876a75fde0c9b5c957b2fc3495ce88380b64a54446738faf2491fe98929ced88cf2a37" },
                { "el", "17aab42e81431aa5e88a8811af8d3a907eb7742453aae5504c0ea265394e7600c08ee3d2c28f2b5c6cd875e08f6c1010120cc9c57dca2092e66db5c567eb0d6b" },
                { "en-CA", "1f8f7295123d2f6212cabbfc10ca2d3f3c888e3b54d51346609bd53a54606c8a205a47cfae51693ec1af6e0fa9f03d104a94b23316334c3760c2ac02574e7188" },
                { "en-GB", "1ab7a1f3ab39c983dfeaec920664daa8f6df69f18e39dcf3658a24d557459a4a07dacd76fe398c6f7298be5d513d9ece66467f63aa706c178f4f922f446a2611" },
                { "en-US", "3ad83690bfa93ab6db36e998776b600430b146243bf420d00e2bdc438b6b95452fa6c3c8d265a71bd94b808854bf9d87a83cf979cf597ac6bb6b8180230dad14" },
                { "eo", "01c02c436d9954ee5232c160082f873f44b2b70bb599f1a6a113cd3fa344e6715c54106f4ca18d55386f36fe3a691aa3f10feb0a6650b54222046c47bd10078d" },
                { "es-AR", "5438b2ea017c1c3ceb0fb9603af3362b4bde52e10ead6c62d5aeea4e0c96b62def5169deb31912e264dde521c82d6dcd804dc51c9395450e534e84c0a0c0f7d5" },
                { "es-CL", "fcc82c82115781d17b5c18bdd425517a78ae7e0648800c66a50f31bdfc4b0c2f3f79d122cb15a788c270c790807b4ec53a9351b5ec5ce2f7e3fb1455a789f45c" },
                { "es-ES", "3d4ba7729a6a01fdfc48acf07263c5c6ce2a07504859871af7e5a6b064b04e208ba9f0e6d37d7b977ebaa671fd8ad68caa615d9ddbb6535d049f16ab92102202" },
                { "es-MX", "ec34ee260c6fe199cbd73431d0bdf1b881be7a06b96dac9ec708327413945bce10add52b93934c6ce2d235c558c939ee54ff6638475ded5bdc07fa3f4e17dd93" },
                { "et", "c2b2e24ea73dc3fafea29543b9c4a5373379f96e01e06ebdb1bf08cf848176512c8bf29b15df0f37f26986cc6707bc23c5e63b06f0f4595282d11239fda708f2" },
                { "eu", "23eb7805cfbf2969cdaa8326de38bf8d910f03bc4e066dcd8b2d943b3d43ee6cf6c6aefe467db004d1b24c83670981d8a7b7112cb13b6b9b2d1f24749426f3ec" },
                { "fa", "c97c69a21715a259fde6e2d0c34425172c65a5bbf6fb962619950b94122fbbf9980ba65506e2854d07f72932e665b7ff514af624cfb00ee12b94ab07cfb44a77" },
                { "ff", "d1f223e4342f271886d643af6a342ecadb83ac2b3b57ed3a82cdb0de28e46a720f71aae3236054d6331420eb445e74be967040b02d344cd1fdb403a9bf540ec2" },
                { "fi", "85e9e14544f775ce4b2ce3427e6329c690448acdcb608eb7368a1c86c37f56d2201714955fc5ad275dd5b919aa4ff11333cb2339acd79ec9fa188ed5f506f53c" },
                { "fr", "58832a869938c6a11e6846cb4e815a72dfb24892aee92252342aa54f4eff76ec24bc954f24fae8697315b416d83271554f75e5883d8f7eab2a594cd0a45750eb" },
                { "fur", "291a0fab0dde165a22dd77c2ee35795245cbb37c962cd97d50f85cd93292c0aa5e14a8e0665f1a2da6a7f23cdcd10cb4313c65704589d4246eaf920dff012d58" },
                { "fy-NL", "ead52b82d76c2303748a93817e36f40f85f01527de5ed512c3e14c95ac09375eff65e583fc9152e1f746ef210d5b0fa0e4214ffd7079d839f6583eeae15194a2" },
                { "ga-IE", "391190c214da23d617b6f8f1819977cb5507391c9b3367e55c320ff102042460380877dee9a0231b33667d987b96a68db33e11a9599463fab6a9aa652c33cb66" },
                { "gd", "6406c8f64b533b1600d7d4c2a5fd3d951dfb5ca95195b27c9a25f0e8194e1cfe8be637eae8f38dae765eb71b0b886be56b6299c5950c5f35e8332ef4292f6aff" },
                { "gl", "c3f0813de16b13eeb9b9d84ca6b4666d49ef3c385faf560bd9375b72532896c6c3d0f6a3d15e4119666cca6d3fe4ed18ccac43d418745ea7dd7c877ff4ba0ba2" },
                { "gn", "debd2673a19c0780f75eebdadef82f57f1be77c4d5f99569f5ded54559dff3b4612180ec3ffdbb8ed54407d22035757099e29065a75747d2db34b6bf044d213e" },
                { "gu-IN", "bd3c33558c11c26cb003bd2f0aec7b60e0cf3cd2c69fa203e3d01b4c4c77184da9d495234f1444beae6380f3f97202044cee4e284bebbf73548d8f5313f3cb40" },
                { "he", "5fe93b821b191c71ca8e0b1ce428b676fba4fadee1defd8c32773139c9de454e311b4e5eb7a7f3da9a066b2fe58e40302fde9d344dbd42e911688642134f6a34" },
                { "hi-IN", "cf470de926a810eca0eb6bf3a483ce227a36e2de836096b5428f919795b789b7392946aff7ceca6130ce4fc000202925fd9dd833d828ec35c73ba1629726f02b" },
                { "hr", "139ea25073f31ab9d0ff8cbe60fa8d0dce4dc117a6123b3ce1091411d437cf0d19d19de764e51f42b145f198d3f295023d5c489d93f0612b6814242c1748202a" },
                { "hsb", "7bf4d0262bd50d1a1c7bb37b9e86f2a0163753765d68e02db86fe0054c35696412c963a34e2a05bd99d052930e51d672c9a499b42badfd127fc8bd9ca8cab088" },
                { "hu", "71be8d1191aee1193686f1fd11dfdfa281dd8973b971132c23c457f286619ee6138a794e12d784b2a490a61224892dcfac662d8f9828f5cf19bdbc5fee9c812f" },
                { "hy-AM", "982aa59b85309d3580821ec9ee2e32df3d394842001768e53e60a7dc8bada10cfff1d38a92ab2fb5512823a714577b04901f63e2a249ff0e80dd6d881bcafa24" },
                { "ia", "a47ca3b0f4324aed508f7785598a38d1ba8cbccfd386ae69aaff0175466a9ab1a4761ee3f9d5fbc3a338467513b1c8a748a267e9ac7a0d50130e2c52dab0ce4b" },
                { "id", "3e9dac967a9642a8eb4502eaa40376a778749f507c821e991c2dcc68a347b82071b2a7d0a92971e8242571d82e0eaf294cb876fff93098078c547890b5fcce61" },
                { "is", "437d093859f6dc0d645117fab668e55ff5140895f017ccb7e52b8162fb9b4e265af75c7c47b8035176df57168b2a8d17bb453af2ee3ad33d8ad4b7dc0fb0d461" },
                { "it", "e613edda8feaace1015ca1e486a550ea70567ee947b5ba28cfe4850f6f7335ee30fd96d961b70aad9bb6bd4b828e683b9490ae2aeca8646c598215b57713a7c1" },
                { "ja", "b140bfb9391ca73609030ed20fe72e679d15c61e3d0b700909d974890534617b80e08019db0ebd32c0b362e41ee86734727943ad92e9a55098d6e4cb45396129" },
                { "ka", "db9820ab99c35fcd98a6d04ea8994a8d46315643398e62d69972d133acbb2b294566a45643d35a160d373b94b8d97a5bb28a66679f983a9c9429a5dbb7787ec3" },
                { "kab", "5beef5840aabbcd0fecaa17f37c663b7c46b2e8210caac16b98042c622c3883d3610b4101c8a5fb058da0cc2a01e5b385ea245fec7d501e97e134335c440a705" },
                { "kk", "0ed0036a50c9bec2d83ead7696a93b1bfdf008fc127a42ff11dac930133d5e19118cd67e6c34e7d2e1fcc079950384b702716fa854ab8c7878ad1720cb842202" },
                { "km", "c001b4e0c0a409a625f03b4bc76212ffbce50c2eadb79f637020f29b3feb4ffbcf56341dc44b068ca3dd04c4c23ab8221df524640f7ca28b79b374b2ea590eb3" },
                { "kn", "95a7dbf9e0f188d7d35c1d195546a5911650795ecd37aa9880666776f4f182278c75351682f8a3d812db80dd2138360bad8055c3deeb3811c2c8e1bfe0103b6a" },
                { "ko", "38fc41f615702221e94ef89c4eeb3d75149e933d05b4f37425650f7f25d0181279ab884eff653a880440e12ceafca39b04f262e54acdc3a8a3bac4784b6a6e29" },
                { "lij", "a861d69155f9dee1467146a1f6b638f55d7db50ec778fe8f78b29ae63cccb154d3cd0d81b9d47561b79ed7368c4c60bc6f930c2c31b68a074cf04446938414c4" },
                { "lt", "e6daddc23a0c8264f1789e8db866b8b581d6cae806bcfb4c5a8cd63d75f095969eeaaf1503dd2858831b7b774ff4180175c5dc6d164818e42cc9d3974eececfe" },
                { "lv", "c1d58d562b785c79dbfc47a432cb39f23e89627b2e18ba1a9401b77bb5f452b7a454e658848f04391a6c72051c138341759ec4fd0f46fcafc51caadb49d50872" },
                { "mk", "b78c249fe4322ac24b972aa9cbbda3d6c95fa1370fdfbf59aedea257b98099857a6fe64d40c515055977defa4b860d6ee8669d5c8bf65fbe521398759972cdec" },
                { "mr", "9306814371404638df9bcf4a3a6c217633003da6533d457571b471e83dd056d1e000a6afdc176f12fb9262a8268c8590f3c959ece97fd3b5693373c429679392" },
                { "ms", "f22bdc45a39b5bc903f1a7c0f1da38786336f31b714f1080a67e38a0c06deaac82aa8e0aecfdb6983099b84205e9f67f27d8772ac35712f3dccd57b789faeec8" },
                { "my", "c5798f8ecd4dfc8b7f2e59239207481047d5165ca504d308b4734ca5f1a9ed9bc1672c18b9af9f855051128d8ca95ffb125cea77883c10ef66443cd3facb4716" },
                { "nb-NO", "a568313e5ac84ba98a1ff3bd451ddd0d6381000e66d38e6b044283bcb342238765f8427c32011975cfa152ccb38c0e249649b25544537fb4772e31522278b979" },
                { "ne-NP", "f956eca4ff5128bb56d35c39eb1018dac32aef0b45f5bc902b34dee10b8e3c981fa472a1f0161926f6d8d64c8584c79496cb145b7d425e5fb6402e898b5ab181" },
                { "nl", "dcfd24f6503fcad37feb9ffc641f9c30d48a180f39650ec8827a03421fc9a9cd898226ae30f00501af4e028dee476d58fb6bd4f00e6daad0726f81f3bb9843bf" },
                { "nn-NO", "7c1ecae1c6ee9f3305805fed2acae2c742a587f21f16dda20643aa4dd37ae6e590e48031a24ff2f8f75668895c8f6c4cf5559c60c20a4cf81637c3fc2aa12b7c" },
                { "oc", "30224c1dc9d8824891f55a32dab08d0abb1c1bad6e992282ded218aed313dc1022ffadb24660420a984f3540cc9137e8a18b34a1230c5dafac41eddaf623af77" },
                { "pa-IN", "994528a1a4fb50810b35b9bf849c8b255f1c0b099cdab858ade3066d91d384abc83d211761c9e8c3653f246fed27d6a7f31bf474937d80ad6c7f3a7f87ef679a" },
                { "pl", "47ff7cc3799474186a2cd13a478e5baa9a2b6e4006c778395b5d524e919d062339dac8f1e1037fb160cec667a1f8f7dd7aa441b9f9431914c5f487358fc74e89" },
                { "pt-BR", "a61592976741120b330c4b15d06a2bd79024e923fe4422c864fbdfba56b333c8ae432af5ad97b70b0037b8c497ea29ed9c8ab4f4edbaf41352ac3e809437d106" },
                { "pt-PT", "75710d7185a5e260bdf69892e3fa71381c40dbca398651bd355f35d166bb9f5022e6790288fd1bf365f00e8cad4d170dca9e1d45e56fd70307f85606986894d8" },
                { "rm", "c81d511e1d1c4a9d99afc9a3c07235a3eb4b061b41a67e747e2747634367afa3e3de012f77361fbc18c1226e1e6ec0c62c566097a8991a3206974402dfb7ed0b" },
                { "ro", "9e765ff1c45b27e21dd22e2e109ed43278d5e8fb6d4542581289d9e1217fe68c57ada9852d4fdf6aa0a5872b499082a517b6332b7ef6a21bf4d0e313aa7b49e4" },
                { "ru", "224bcb9b057a99183ccf351951d60f382883baa0550da6f1199af85a05885e9b049a52cf1bae4eae37dae4869ccfa3d4bc33f6f16efb288dbf9c73a517742b68" },
                { "sat", "5f0c6f037632400b5246850f7d0536082661d61083da166e7f90830915eaceb1557ff0dc06684fa3274458eb0a1435a8a8f9c0e5b4e56d3a608986c60671ebb0" },
                { "sc", "84c3bb05a1afdaddd30a572ade9f526f66af8cd1ece00ad5e89df5e85d7cb1669ab646719d978940e96e6419f557d116d43123d2c32441722dd867b088e468c4" },
                { "sco", "a0c1eedfbcc485eb76915b563a21b32b8ce2551522686f81c21e2810c3a7da8737d5a996ae25f4673a1173dec0771eeeb63bcb199b63c08a863b0abc38288590" },
                { "si", "a961b627e9c6f67890dee966a11cdfbc83f4a995e91b49fe4c1a550d20ee508de0338be5f025b672253f3b2057a6d7de09493a56af017a5dbe9484f31f347fb7" },
                { "sk", "40eec757c05be356db11e1daa18f872cd3bb4c11626fc770b286d406f89090865fce30333820112cb02d0eb988a743963dd1597f3895edd278d66e1594b27295" },
                { "sl", "13ca7fbfe478830fb41fc1de778ecab63a39b185efb305d14ea1271c8e86e37f6cbd88bc8cdf0b2f5c8aa9e6df0805484a6b4321f74546627b2d106c5840af71" },
                { "son", "da6b02ce68bd6fde1ccf6d4bb32a839508f0082743bb4ea323fdcc2643f00f7bb2c5141b6920e240bfb25afe303d55c7d797ea72eb96dae9b59e08ca24477b32" },
                { "sq", "2a635c13545b675c7191b3d371dc0214879e7d91692f24e144e04907548c7d1d5c40cba590b1046b87924928560d9e877025ee3c31b9672528ce3584dee836e5" },
                { "sr", "caf5b7418c83f5ec7718457ca12282346933ca40e60a081f39d5321e72e41632eaa57296c5c3eddbc485d7caccadc3b7501d6dbc9a3cec9fcb65ded3cdeaefa3" },
                { "sv-SE", "c773b590a69ae748a74360f4a5cedc332d4b725a66756b83a34c024a57fa0c40f65ed7d7ea217ffb00c78ee3eef38db1b3e1b06cf47185d7554f02aac7651913" },
                { "szl", "95a16b6f5f58141f594942bcada19ea4e8e47713a77672ef4aadb1ad84a29844a0aeb17607ba581ff37e8ca30d7db8aff7869ff7f7cf18350c1a01fff32ab9ba" },
                { "ta", "9b6567705f790bd35fae7c3005e93064b2d620e5a0540cbd6ef38d1a9b85a97a4b46c1da7d05788ab44f5bc2063e1ae9c6daeddc6b8dba015fea04e43a12f884" },
                { "te", "6556a9bad4967aeec1c3c2fcb522b4ac6a8f5b40029b2c3f3038d95ced96eb87363ead527ad907fe47d601563f30bde4182a44392d4f848fce4d1e191ad934a5" },
                { "tg", "5bace4fc8b042b3493e70b720e5cbd917b527b4817a0167bce29e124e114eb247d26102f2437d6a53a7a0e02dc71e28ef156b70fd81094e3a3320a4c940dd60e" },
                { "th", "ef540c1a729662e86f61cb0615469879893632fbcdbadf8a09bd468587025976fe452cd0f98ddecd950f55168b9961cb2b2a8b567d766d5ec744b3458d2c67d2" },
                { "tl", "b84e4a4f7d7011592e491c6a4083606e22b819f524a092bee263d1e53284ce7bdcf68dff83c868a46f17e291ae151e0f0e3b6355bf10126d0dac6ec2037b474a" },
                { "tr", "01544b56ebb08f79142f60345e8de8cb339f2fc8768caacc8c647290722336442fe7970e8512dedcba88a36edf97bd3586b71e6758a78528320184cc4e9f7654" },
                { "trs", "be26fcd28154fce75b4bb8f94bab1590b2127c04eb270a897009d5f50720eb1f3738863f9cc5a020c2ca0b6e2aa3ece49eb1fc802def5dada11ec2bb1e7e95f3" },
                { "uk", "59a1dfe35293996765948fea7cd7f02bce34e8651d74d7619665e1b67069a0dc7cc4513e60d605abda7558acd1151c2d0c894e5b78a68907673788f5bf7b03e5" },
                { "ur", "fdc5509b5c33302a7f6fbe365998c16b17e92a08a20d20d229b55e72b0ae9f928dd23786dea566368e91719c05239fb45c9a480dc7f66b9871cc2b690ec1c121" },
                { "uz", "ff54d851fc66f33ed3ec7353311828ba431a5d9fda12dafb45e121a57dd4380006602a24903cf1949b478f42698fd6d301f3f236593151f4736b173336b374bb" },
                { "vi", "e7767afeba44b38a79f0d3a98c727ffd38acfa1806cb2794ab03e3fdb6e03767f42ad16439142bb33584dcccc04533ce762ccd5178f149bd7bf708ca6644859f" },
                { "xh", "ff8e95a193ab272c8135b3d3c96c56fe9341e487be0af430bd6d8b28d8ab38d3839a8addd9b937471aecbb89805e4bb65bb022d52ad9ed2f157b9650e30f578f" },
                { "zh-CN", "23ba08687ecabd54eda8cd83e016417c21d32dd67ce153957ef0bb3732047c7891ac2fb4bf6ca7e86b33043e3002093ee713f7aed58081853b3174605cdd3d58" },
                { "zh-TW", "5bb16cdf6980473cc21ef73b6a13940d20a57c83936c0bdb767f7ac263335141f5e8bfed9189add5751921c36631ab792654911f7cf3982c643a56ad33d33e20" }
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
            const string knownVersion = "126.0.1";
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
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
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
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
