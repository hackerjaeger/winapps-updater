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
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.3.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "e7133f8c313cff7115da2258a7e0d5187baac0ba6ac9d6ea3dd0bc25033d07a27006f2f3d990d64853dd89465d79d9eb7c87945d1bb6e71ba95cc05e35bfc9f7" },
                { "ar", "926dcd9d67c1839ec33a85e2f93249c9ab596905dbbd5fd1f23fe257faa4e7a8e37a24274bfe6ed3aa49ee2a06c3da4aaf4a3d606268d0353fc9022fcf46dc17" },
                { "ast", "35c6aef2eafffa794f92e2de71d0ee415462f338349e89586f776be6e6e53b90466e33d8d61953031bee5920326d9ce867e9597b04bb491244a2a03532dd21ca" },
                { "be", "6d75976604353aaf9d1d42c1d4c1beeaa06e6fd3ee26166f58aa1f953dea0fd93bdc170ab7538b35b95b65d4ac7f3bbcda8f801b680254f4cf451e4e937b21bc" },
                { "bg", "b96af41ee9e852873b8750b5d8b7e009c561a3ae0d13ce934576a44daad721959081d512af299aa2c5b93245bce96522ea0b8ac3261853de7abb30b9870ac63f" },
                { "br", "c55348f766205fe00d8a9341aab5100fb1252906f4f9ed82751c5d8bb3752b9f58fbd2c28eda747691c8ec7657d396065d7bdb3d952d5f25e9091660affde3b6" },
                { "ca", "ae088e0c9380fc41be8e1a5ab3df1391833accf5088dfc7f3f46d4b4b2ce196a58f4a9eb184c545791441d747e3928a57b42b7c81d90ddd49fbfa870ff98b886" },
                { "cak", "2e39b21acfc4e030b152954712f9deeddf396c41e209e4aa12fb8af94b357c50beee8043c837b29bd6b59b038725679e9da76702dfbb3d88c29817bed3807b76" },
                { "cs", "f3e9bb7b30dbed1237d6f72d6d0f116188facdd6a84099c365a51686c9358f4a0b95158fca8d5808bb405dd6b75f4afac70976603c37475fac7f1fc5eac53ddc" },
                { "cy", "ce219faafe6dcd388612d22d83542daab2b6e801fca21d6334c18019d6d9a6fea79664b1f4cd83ded6a2eb5cf1cab458265a53ca7eb0e54b0a8cd527770e0899" },
                { "da", "ac94d53e3c01ed56ea43a97cd8a44f2b113b01adaff9076636122191ee29900dc615f9615282991453d82c44cda081fe4e8ff412bcb8ffae13ef4f12d859a4cf" },
                { "de", "a66161ba5d838194746d5948e7986245ebc2e4363f47e252373f36a7269c7d1378e504673731016b5bd52169afea09f7cbed44e2bad1269b89f1bbf26132fcb8" },
                { "dsb", "c8c83fc09ee67c3d4b911931a86628f87f11c779101e3d5712ed40273677f12b2622ba71ef2f464b6d3bc1991db4e1995c310e72a14d8344c881f1938763a84f" },
                { "el", "33f48a608da6b44072f67338746d229c2c4b650a188a57cb5f877a3914a382ac7b38a536a7c8d8fced6fa5ce08d25b036789c19989638a4a7a8721f3b7efecae" },
                { "en-CA", "c9337b3c5e7d8cfdfe238c2913a087b99f937d557f1ac36351575a335d4df6f8dc61f4f8961a644569f1c6f9ea7b62b426bc40acff6f74213619625caf4a5aee" },
                { "en-GB", "e95c79e24e4b2a8f43dd5a9ce4e45cf2b7a077b05e702734e961fa6e3f34bfbf01acd455a91dbb173adf17c1cedd69cdf7454aa6299519878c53b71909086d11" },
                { "en-US", "4d6e519db15533c5e397dd1d98635b4544a0831e31191c63ab18f4ebc6286612fc4a0a7f84f8776c880651ffdf73e23893bce4cfddcd527fb20b8ec98511891f" },
                { "es-AR", "bef42d5864973f7768d43bffac89f0e10387f3cebde58f545b8b0fa3f0d9ac2c4233f4295adfff5d1c53c92138b189c2d10b4837ebda33735e2d57652c85b4fc" },
                { "es-ES", "2cba715d7ee7e10ee317aa5d0c16b0b5d498b320ba45089cfaaba4b276d081e8d9470f13063f2e8dca137d14fe079974c8163f9baa37e81f55ba2f06640de02c" },
                { "es-MX", "b2ad39b8c60e3b8ea0cd7a2fce19fab8f7d73e3da47dab36719e428b33bd36dda380748f774917b1fc3449ed1672ba3de2e65301e510e615d69ac8c19f91878c" },
                { "et", "a27fa9b6d4381319a3d8283f5f634bd43915e4d5a9211b86cdd0adccebd74f70e204d239566eaad1496cf67c5eb83b597c760d01f0df61db94fefd4d0770a9e1" },
                { "eu", "40cbe87d6bfc94b4a74f7a3a773c7e927aaa9da32da07075255d3904e39e690a3c63236daa8afe2224dee8eb965ed8110a312786c613b162d9177f917bfea55e" },
                { "fi", "73b6b1daaca4538a478ecb5a67af0e30f31e1fbbbb1c71cfe5454ee59bbb73debb953de8779265fa33b4d339e7348f816893ae873bc093640900c6439b67f99b" },
                { "fr", "3c92bb26617b09643c455f36cccf5cf6aa0c75668801d3d06b22ecd189a4f6cdba8f05f92fe6314c87bececc251ba7a226ee7ae64279ad7ed00f2a175e49e587" },
                { "fy-NL", "ddbe59ab4b6076328cd83745432f7d64ad555effd835a1d6ce63a8d3db28184942621f733d908c5c46f4a5746a784fd42d5c507baa9d4eba0702371367e5ed54" },
                { "ga-IE", "31dbd6e414cf88511f53238d5e2b1103bb8849176d3909907fe74b142ead9730cf94472d6132bc69d5dd16d770916bcc66805c10ca8c09d8a35d5a41805ded17" },
                { "gd", "5f2f859268bdf5d88fe7b5a0dc639161cd6809c0b447bcbbf753a0c8dd726131d8c4d264c2eb1688adadd1844194759e8a85ddd08701029f68f70a145993e6da" },
                { "gl", "c3f4afcca4df91f1db15cea8114ed24b5480a10633e730c19cfd45a571a43353b284d526a613167c2033a355242cb9d7db10bb0a71fec0c061a46c4602683d25" },
                { "he", "c1b8bf9a5f0d93b34e94ca58946a8d045862f5cd38170834db3322109c0afc56c5ea9e3ddec2c75dc46e696b04de87939d41c64bb0a599d7cf95d79a6c63494b" },
                { "hr", "d053faeb6a9ce7ac758151a3c4708efa265484276726aaf7c9eeb8a9143f8268ffe417a08f02f5050a706966dced6ebfde102dee49bbfa56c4d3276822d2e27e" },
                { "hsb", "e0890594e528ea2ef1adeca115bb413ca929036e60e43b342421da35d3d3bbdaa3d904c993c7e5fd4c7bf7b4047ca5f62cbf26a3cad07b0729b01cb13c84d11e" },
                { "hu", "59c1bc8465e01752f64357530aac68e48e5d266c57c71b206393be1243c3cdbf30b671b6212a2d2de46621fed2831aeea7cab3d7faacc81fc96025b72c4c522f" },
                { "hy-AM", "eaeb99ffb8d8be3121a4ea3bf98603f3bf793f9211dfe03096c4ecb62a576c1471c5a0ca4e40d694e6a0a2c017e5d54bed46f8eb66be81a11766bca3561e7fc3" },
                { "id", "503882b4c260b1c1e19233ebf26dcec006a84264bed7e2a870d0abf38b689c98aa792548756d9932b99274b363d7824d309844d858fb7f7ba5ae7dd0b1430c11" },
                { "is", "aff87db78986b6da3e0e885b1ab4097be98fbb3ce54701a052f43db9d966549d252379583f6b4b74c82a65b0455e6abff7ed53315e2903174a320db0a7a199e8" },
                { "it", "70348434a567d59dfcc05692aa2dc497a4391fcb6e4275a52296ad3742d4cce25a8d63d2de76e439686e4e3ca2e5f2fc347d8b6085963040a00fff5b4bf000f6" },
                { "ja", "9e4574c616a9a0838099a3e3d6a37ccec41eed0fdb64d4695e660e7146c9b4d6fc0cf99129014d44d6c4fa088bbb5250319079663b0bd36474bffcd692afb6f7" },
                { "ka", "b894b3c44be8ccfa7202314c126db8dd7630c51a4f2024d0eb5a45d870d3a9158485615674c67788668022bb278d1cc6b7b2d5f49b4cb65138ec6ed3c425e195" },
                { "kab", "f7c7c2195a34ac6c1e9cde93a242ef78ec9d0e883f3fe9bb72d604143d7ec08a9fbe1f0ed51c2a476ed187c923895aa2aa418b39ccd4fa730b80f49011e6c6d9" },
                { "kk", "d72f77505b04fe801867636efa36628716f4db879dd45c755be60425c6920af2c137ef2d32b5f3c8a9627ae388caf2df005d012cf8e22d8efbc56ca87e114232" },
                { "ko", "7e85c7110cf09df288a0ef09deee39914343856d87e83f0e88adfb2435d82d3ed5164c700a7342b2fcada491ae9f2241ad66d6d0d99b6ffc5c4f5333ca7a95a9" },
                { "lt", "dc05dcfe8548ec8cc9d66c3cdd0db88dc0380339dff3d053c9e5fb10bfe31add7f9535b1bb184b3aa6bddb5734662dea1a8a975308170e017b2c91eeab1653ed" },
                { "lv", "377570444980e5cb9aea28387ee4600cd56d978f27583542a6ef11beb2ee2d429c9fac83bf6f0cf6c00bc88116bec7a22f553473ee396023fb0cd80da5acb151" },
                { "ms", "74df1494e1ef091b29711fb0b1d0e857894600a115bd0c1489ad69996cdffc803f50100a145b25fbeab9bdeecc7f2a52d3caa255c41ea983fdd5534825a68522" },
                { "nb-NO", "101b4fcdf19c3ea9a4d672bd16ee6a70366f82b946fb96dc671eee4780270654b0c8ae9dec11fd4095f1be23c5b098c0588eb1c46d0cae2100be5089dffc6434" },
                { "nl", "4c88238eff93f8df4138711c58ccf3c5313f4d164a099b6773edb40975101a13a3b35a95a19fba162afbf6402bf90bd82aa52769d39e98d2db542206b1fda40e" },
                { "nn-NO", "fb7bc7d78279747b7419472324fafb95683de7374e2424d8e1cf404d7b53b59ad0a31a0a0ef4924ab7538f95e5cffaee2aa924e1627dcd63b644079c434cee92" },
                { "pa-IN", "ffd14f0c85c844907eca96f32d29b66083d810e8bd23c2eb7e1d52ea0234f4159683730aff694dd7029d17b431cf87b326a3b64cd3b676c0f289ce3b82cca9d0" },
                { "pl", "4a56a4928552859bfa0bc0aa3bde4738270827ac203842c5f4ad5a69ceb8a5bb0bc0565e58618b6074357cb9ccef3674ea232e1e532fcf587277c97e184d3651" },
                { "pt-BR", "7a98ca69cf79c2181e9f0da2815b9558bc3cdf06f36ea0273bf2d3181f8dcb78e42a305ae68301c9c0526d347db3393ee54fd22d352652261b11e1cae7cdcd86" },
                { "pt-PT", "eefeb5980a8123ee49718b0fbfc8774e3af96b29ca47e33ee5ead242eee92f1642261ea7bd4ccf06c76414343b95968a35b2f46c8edd897383719022d55b2930" },
                { "rm", "fbd539f2b619d3bda5fde0c34a5f179de57aa80719cfd328fdf65f062137a3ab835a38f670e221f814bd2c2a537555d998bfd29b1e428107fa6823acdcb85ffb" },
                { "ro", "c7c456690f8fb59930d4c8619546b9937e9751cd8265086fc217d8d1fd6ad6a438723c10369ab5c92d4160922daf99766fae8bdd0ae8569f6cf0f3d10bc3746d" },
                { "ru", "690785ecd36da693e21d93085e8fb0f43832525491c34a3a0f361ed06c02b284e66b67a19d7cbe31b62c88cb6acccbd1c1481a27a5a59f48ac8526619d68b3b2" },
                { "sk", "c4648f52e286bee5a51d8f03b0fc17d904a48bbc811496363d0b39518a8062b1efedc45aae702babd4244be44088aaa5ee43492cb1fe5c8659bcb3daa30a30f9" },
                { "sl", "1cdf751c60a47f5266b993ac61005565c4768b04f2f73b974bf439b05d42493d4ca4e6bc8691ab522e49d16139ddf8357f04ce0b5499afc0b925bdf76d3f1d79" },
                { "sq", "fb065fabd581daff80e635c5610030db0f4c0b28d6f26a56258ff42981bc5ec946e4ddcc646998468051ecda5496219649caf99cf5d51243c8a8c6a607ad6848" },
                { "sr", "c3f5773c1f675402678e55a5e1bb64705800693772fd500007c25adb324f0273aa56b50ca8e7ec2157a3bc291aad9e73e5b42d0bdcf7e1d568e5a4a7ea0bea58" },
                { "sv-SE", "427bc3e61fb281906e096583b70c87d2aed1ec500d18337d75e83f3de69df9ff3f20e99784b81a1e1558a338b85c131081704907888df546bafa67a3f62b5e21" },
                { "th", "f6dc38526dd835f1f64ae17569fa0a0456cb2e1ac2357e7deb4c9a5845aebeb0c82b515421293dfa289fbd3cc9dfdaa0fc6ef2fa1a501af5c1ea469d3354f82e" },
                { "tr", "68d750dadde72630db4a049a2800ef5c7608442cf4ba2929c58409c0ece23d45b2b9635fa4e6ffb5c066bea03157bf10884291de7a3aa9c87ff19b446063b1e7" },
                { "uk", "598c507ffb0e85bf62692118e65a3db5beef4e8c5c6fd40f22903f3a623b87df12cee0073205d677e14ec092129e698a7fbc8e103cd7408e9c1fb15e6e40c127" },
                { "uz", "ff2cd96391a4a8c13771f1120cee84249e43495b78f0ca97d74513ab53997a37b22b1384b880b3035bb113d5d4712c78625867f6a8658119cbd0e4bf2032c77e" },
                { "vi", "874624fcfa56e85a7f2ac785cb2bfd7941e884acae3fe03d9e526d64575746b00b14ec754f61ae233ef10588b25231ab4c0ae1ba34f5b343c697a592c9c7dbdc" },
                { "zh-CN", "70f363f663f93eac463f14f8d9efead979a8d167593ceeaa605b018e7798f379e6c05d35499fe9cc9fe50cab686f6a641272702f1f7002633f2a1632bc822c5b" },
                { "zh-TW", "e7f348adc8b93079d3de6fa6a76123076fb8e2109506d660d7e5af58019bcc9ab579758d3c4e9266c965d35491a3e26f8b6f621ebd678df9efa597d8e7d5d011" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.3.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "4292f3cba121a06bc18ff19edd502c6a5cf356ee6e5aa66cf7102a103a0d758d8e8b2d6990b5540443a615085e9a97977f06575c25e0c0b673ae54748eb0c361" },
                { "ar", "ecdb28054079c327c761c283e4bb21c76cf8309452fe02de16fb6bb04c458d23add881a70a084b5dfa4ec8af25f341f6b75e013cb643e170c407528c2027203e" },
                { "ast", "f9e373668902f559b6a46b65871715574f138264545741f64cabefedc9c424e2ac16f477d62d48df5863b5b5ea9b46d1ae54d3470a1f88e9a83517a0e83611a0" },
                { "be", "430a68a64661f63dfe1d2ec5cc413c4811bb30666cfc18aefd0cecf420f59f57e928f4f073364183071d789a82c9d53324703145dfc1c9d44a59812d9b9ad7b2" },
                { "bg", "22b56f0ca01fe92e567ce525a05108736fd1bb29bd568287af1ab6b49277283c4cf29720df36a66c8c87b9932c1b5707b43b4ba17b575e039aa8a856d1459aa3" },
                { "br", "bc19aa7e540034dee56fbe7b51f346f18ccfa29a51ce8d707122dad4d56a580249c452122eeac8dca4f1d723107e4acca8bd38e360f7d1c538531fb73e246cf6" },
                { "ca", "ed4edb58812f30e855859565519de2b4b447aac2c07d2f49d7ba23e604cfcb847106edb851af4b0990bede984b672c86c6dc1e370ba19e676296d499ce875ff5" },
                { "cak", "8778fff6d1b0c8d17ef48cd485a1288bca492efee37cabe4222154b7b639695bd81087d7a553e72d6f3e1f56547efa19152a75da9161019364f0cb865169210b" },
                { "cs", "634aa735bc91416d59ba39938b0c95747614aa45ae95f0842cccc91f6edeb499a20df8f65272c80f6c96d3ccd91c72b130c47746a817e70662da439537acd33a" },
                { "cy", "995bbfc6de799e5b3f3466efe95109a6b005139f52243c00b7b9332cf3c836125c82d5a265815e482b00c53ee62359003b5f9da1b59f0bec4e6d7fe2b5e34e23" },
                { "da", "67442d7a91a9f27021edf033d5fd68d17dbed396e6994350f7da7eef8d2ab1ef3466c380bf8b2bfbbe4ca8579185d9005da8ee65d9836d2165811ba78d55ddb0" },
                { "de", "196c8b731218b2b60fc613e67e4f59d3b58b4d5de6700e82561e232c6a5ad320435b43f316e14c81a41959001c10f5534aa52f4a31b2ed0846ef91654f2e4f70" },
                { "dsb", "c968070367065686885be53a1285f4471b9093f09b9e7a4382c6b246a65dc08c45fcabc5e0e9143c8e31a7d9b74373740d3f58d9e70b7a9cd88462734bf1baeb" },
                { "el", "3f52acb88a2316318513a5869af3327b5d89734792e939e1c1d261405ff7539fb53b622549dd6e0f4fc019265892a7ac263b0ad7e262b8e921865be6e1a638c1" },
                { "en-CA", "90d41d9c00c35fb7d644e241ac6acfa36ff2d94a063ea85659e59f963e36c4924997a7a62d8f0dd0d20ad27aeafadcd5d94c6bad510ac11179a12a90e1e788ba" },
                { "en-GB", "6c1f8383222226b38a8a5b109d5558afaaddd46d9398f1cd31255f2f4fd3d534db4d2e00d523d160fbf281fda888968aec0cd79533aa62e17f67d1516388b912" },
                { "en-US", "f0ee25427b876e091a9c3f27f2d4c0eee63a8a377c136ad9bac7195d9bb82ad8fe5a679ba32f741be817e7c5e5e472be1f77dea05cb6c66263f6db811ee8ec71" },
                { "es-AR", "2da551fa1abc0f8b632d71da539291bd99e44a303148568c4662e73d4c8d5d37ed02931450f093810a07b95696d1df1ee042aee7b2a0686026590792a6548927" },
                { "es-ES", "dbb8322df9c8432c050c073f74da95d0930762f02251a89c1752aeb2788c670aec325eed78cd28fa5b623caaa563c10feb22b32c2ad3e18d7aba6c6e803577e3" },
                { "es-MX", "4ce9b78c7554bd497a7b4c816b1b3506feffa2f8f99f7ece4578cfa74f2c5dd739a3d925faf622b3a43e4ad95c4405dec02883d19497fa5fab08e25853e6d58c" },
                { "et", "ee643a0fd9c7522406a920ce53389f949165994c2f17b7865b483de612cb5fd50cb46f5c9704c07cb8d17bfb3e735374fc556a0aecce193170cf1f9788d939de" },
                { "eu", "152b2fe2d560f46b64cfe2d17f974fd3522ac434a740d0aba713b63eaa89330b1465493d5d82e2f8a1251fdfd2008426ffac3815ac933385abf3781b15bb3f54" },
                { "fi", "d280aadb757ee007985276c4348249e4e111c5b8ae195075d4dd6accbadf9d6d47dc184a000c01fcf134385793380da0509d629424789f519744dca92a6e5d1e" },
                { "fr", "414bf3114994f913225131f39a9c69c7971fd3c58579128d941cc4c1e215e746a1a0913e2e4fcc368839584f3849fb8604d978301b58e89b63754b7326df6004" },
                { "fy-NL", "4fd8bd48833d3dcf9b84550bf3e2fafb766f6c594c6458bd3d565a5b599a288a5716bd13585e90530aa47018956a831a6b48db03ee55e163ba2de7db99b0c43d" },
                { "ga-IE", "dfefe153612b92afbee4f54e6b3872770288d569452bc683e3413c10f43847c4c2c1c3a223e0641a5d9629f36b78b0cfccd7d881791e00060cdaf5b38df97b14" },
                { "gd", "ac5264cb5a53e8e899fbd1af336585e5b5ddc5922eb15647430136da0eed79aa1d2b77e5973f96e62350340f94fb190a1ba9312bbdf9976e1b1e3c591d8f9372" },
                { "gl", "e1e77a221ed4e9e4d3dd655d7cf97058dc8d56be2b99ed3a07418a52bec836af93bc376c85c6a03178e2a691745a2bbdf61899d5d5970ff26ffcd237cc3b1294" },
                { "he", "7f82a0fcdd5c726df26c2a9961365da037432dcc9d06401e1ecbe8f1182e2aef6bc4bd9402a22743d63fddbde610d566f3062b8879119b5d99eddf7f4d4132e7" },
                { "hr", "0617a1ab40bdbeb9b5c9568d85a296bd77f2899f3909bd1b33066873f6c57a934a7dc23e164b2a6b5d96508a60b23711caa1727b5db9dc87c91c3aadf998bce4" },
                { "hsb", "e6a028369ac5fd32a70f3c3558e95691318f5562366739ebaa33c782a3f2596d16129ca9bdd186112f868ba03b2c6ea3558261bb2f645d6b097077fe627b211e" },
                { "hu", "ade9f95301682e6bc900788f148317737df0adec8d08347b7072edd97a1967cd2470669506cb5832ea2043d01b931cfecd97b2cc6e10ca1770d882260e3cb029" },
                { "hy-AM", "9282918ab9ed063bbb83973aeda8dcd870b6e9711ef36abb7e152fc33ae11570d85367551050f2c591a2b5e02f08fa7468a09d3ac3ab93720a4176091f9bb28c" },
                { "id", "23f17106f77e7834e6b5fc1e9257f0357d57d8afaf50a5e064dd6f315a0220c81f7581905cb1af986c1f12fd7994dbf6ecc2e499c08127de520835ec4cdbc2cd" },
                { "is", "2333f6e8c8457a4bae5bd12d3510196120650ea0fb310e8a140a74016ba58e6ef0d1c8a23239ce1576873c9ee2a8ec4119da843e20c8e94fddbb77b725eb6b7d" },
                { "it", "d3c2325537901ea732cadc7c984c5a46f0b1279832cbe8381d6815b88e38e241378c94a753011833ee3fa27e6123d7d274f775bb64f2619f0c4c4e9f5ae42c41" },
                { "ja", "a2d10160daa6799d00be27e25e569e432172a8a37b1f7989a3d0b97b9066c9b6b0c6b322b318957953ef793c1b6ef7e0c9656d8a9ea3fa5796f3539e9226f420" },
                { "ka", "d88f720f17b906a18150e6d97be089b40cee7d00fd00e0e2fe941eafafee525a9c4f272cbbfd7d4f19bc5e8c1cb2b03b14357c9129d70ebbcc38aa1a5d3349c3" },
                { "kab", "b9c1d4bf40c54abaf98b1252d9cfe4a2af0c8f2e7469dc3b07e1a364f196f660293e71c4ffac6fa0049ea48aeecd4a6a11e804bbd4787860c3d271f46058c8c8" },
                { "kk", "0a9ecb3e6a6d7c0c728991dce03634cc266b2107ef7705f984d089304bbb54a07377207b138f9776aefc8597f5e4052f5b1b359ed32e22ae16de99e67e8969ca" },
                { "ko", "48098016653bdfa4f65d04227833d484f595090ab5ec6f91e561074b9b49c7ed3320ebf8404599b5504aee5ee93b3884637a9dbddaec4f3b0d053ffe74793181" },
                { "lt", "b202b8d4c824c9aa80d268d7e666a0719f92ae3bde73f69b97cf0be16fe7c7b37a18ea70b38c75a323e9ca1bc7f018d66579461d83c182170acb2c767f04a70f" },
                { "lv", "9e0d731591c56f241819da050700b853d6c6576364d36dcedb76ed04b44032bb6e607e1628923137ade7aa4d2820e58fea35caab3731f2758979bb7b5b95e60e" },
                { "ms", "352a183682c21caa753883e4d1abd769686485703a67f615ff8f34193f3a3734bbe62d4dc935ece12d93079c487e99bb7fd0c70c42399feb2712ab11250b7f63" },
                { "nb-NO", "72f17c0492a14a044edfec87cd0a373d9bd8102d11d56d77929d9b7515e20fa366f269fb1bcce76205777c615706135f8087f597f8722eeccaa28a993c232d83" },
                { "nl", "9add41a9d305b8560ed1631e997a47dc2e19e12c0b11a0c06d7fcb8fd9d50a0a90ab9109cab8b6fbab353d5bd4d474ba9724dcab265e41fe349e648704555175" },
                { "nn-NO", "273b3f96671286e6c6026f98d123818653266857b84e9283bb4c300fc2b530df7316713ff98d6c75f702a1ae775edae3018a2d4441608dad6a562d0af99d0e2f" },
                { "pa-IN", "a8b77349cef25e748e0d5078acb08daecb0905db0c5a5cc291305bdc551fe4f90803984008ee74c80c8edb924b43de104165aa48172d84263ee790a2cd27b186" },
                { "pl", "5f2800251560566160fe8004f1f5a49cfbd01b59933fee3adbf81cc07a3fc980b10be15bf0a7e8d8d29f9644340033697a45b070c519d50c9cae15977d773131" },
                { "pt-BR", "4bac7df2f491aa553e06204d2cea272a3d38f0f1750da336052cee519585682397be1fad41df4e392b677e17c5a772069bda1b8e440bb0d43d3115fb62e94eef" },
                { "pt-PT", "483251eb5349858a20df3dc283aec7dab30d4d58bda2a9db70c6b1d72c323f89971ea759de1a62c8a1a6fb9fd4fc77bfdfb9dfde0011e4c144e6637390925f67" },
                { "rm", "56d31692fd53fa03b80c92d0d6701de6c48c71a623b4e4e6cd923a44ecaf180fb8802e56a2d58c98e61cd1afc368ac63a06e98df07c20d2b1420c5f47c10329a" },
                { "ro", "339d94594612aaaf0f77fb7a20e5309ba8d3f8c9db354a25dda3a17dfdf42f6f732c2769f766192971a736ddd3c00f460f6391e261f4763edc287d0d2d26f0d2" },
                { "ru", "4ae01dad178188020649944da32a443699058b13b03ec5899d5fed5f9c7b4b2dc1e99758bd32da65528890b50ca7a2f0ffdf7b6a6dc08cea126c846fd11dc078" },
                { "sk", "7342fa1045b0f8ba91c2d10d5ec0c1f0fbaf9034daabcd649a9c4ec117e9377b286bf027d17edcd07a3701681933aba28d540aef0b76d99544234562887716ce" },
                { "sl", "6818795c1029565d999eee0245f0f869af8b08cea97da54a71bf546a8ff08e12d6e807cc27e9424f7b36383a8dc2d57dd151f5df4aea9d4e024086d8220c156b" },
                { "sq", "4cc216642cfdb8ccdf724395000ec395071f91451f036db85d9c7a1d635a5ec0a0e6294a7ce7df043d19a6f61b6a232839b878b8337929fb43f00eb966bc88e5" },
                { "sr", "8a61d2b74b5f2c1b0125dedc1ff098e9ac87dfc12335d8ebbd3904dfddfb7b969c182b4453de802bef1f516e7e43fb2b7fe091673ab75100ee7cdc2bbb4e33b9" },
                { "sv-SE", "0b2558d0b50a7fae6baf649308e61ca58991fc9b297ca8d6fe17f579413715852aabacdbf5e2902396282b47438f62351e43682e1a1ff2a2abfafb6a63b08374" },
                { "th", "054a0250a527444881f8be50f7d2160c95335c814895f42264a60169ca74ec165e2e51341b83c84b56df7978f7ba954fe5c544104e29106b47d15d6848240b19" },
                { "tr", "9ab1d48705a78749d7fd69dd8c0c2b30354f27b2167159dd97088ab5582729aa14e6d3423ac4d4af7fc9eddd54fa6c9e7b927d6b614c3b67318a62c080c196f9" },
                { "uk", "bd14574845230b971fe5368cbb4a8dd3af16259c75f6036882b447ec87cecc468845f0037dabb0950778388d298febfe22ae21ece35096632c5660c152d36b99" },
                { "uz", "6d8b73520d1547f96e4ae94e2391b74ad9348b956f95430c9f5f07901bf3c54c2a537ccdf20ed39d9cdbebb1345a132aa9d48e7706a0a2ac7db2d287981fd942" },
                { "vi", "8a2d5ed7886bda3fa0cd5ca2b172ad5f3b0a63306dd8f78a28d47a971099b1e81c631854c94abdf886c7628423414cb023d26ae91123158f983738b6ddbaaf4d" },
                { "zh-CN", "764ce7531495080bd2658f70a65a598bada00cc53538b849712bffebfbc01749ab2468d4aec266234af0500be0f0446696a6c7adb63b3d5ee04b0b8ecf694652" },
                { "zh-TW", "9d8cba23a17c55c13d9d6d78cb28c557a743cf70866f862ead492c1d8577e66b5d5657a99862139bffef2c785f0204a30eb7d61da98ceb20c7292d364377a866" }
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
            const string version = "115.3.1";
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
