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
        private const string currentVersion = "137.0b4";


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
            // https://ftp.mozilla.org/pub/devedition/releases/137.0b4/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "9664fc054b52a3970fe25e428cacec46d777c417604d3d501f408e89e07124c5ae8d679ec96a6fdb75896cfa58accc02a78a382dccd19b56deb95dbf1dabb613" },
                { "af", "f55c28773eea0bcd4f612797d691cc4ef5ade54a89613d5f411ce4de1bb0abacad24e8e52d6718fd2a9d5258d4096b4c97f1622d060d69e51830977f1800ee72" },
                { "an", "5ff0167ca16ecacaf36f4ee696f7cc2723ae7f1ac12d69b916542b3c106cf17edcdb09ab08593d8075b14afd88ae478e2a9d1e23345e2e330599239836d7c369" },
                { "ar", "5ca252f3cc9a14fa80c323aae79302dcfe063f65856ed1f1e16e3d52de2e89b23c227e962ed90eb57fbb642c33c4b358860a66e87cb13d19671b436f78017f97" },
                { "ast", "7db1a84cb0de3f7a3805a49ca28ee35793932663ac7041ecd6713a8ff6074c9db2eea2cc94ed04c56891238bd3341f2f4415439568c8eca5c1d9921f7e3d464d" },
                { "az", "ee98a71c62e0f44862e2b0b603a3862d64fae0bdf76e9477cc2200a1ffce17c1f226d3f620b363e6752dd4cd28ca10074a3dc2b4ba2ac2343dd38a88e3e9c55e" },
                { "be", "c62cacc49922d9b636fb837eced0918ab8413a8eb55fedd219be24309bd868a71d916bf72128ebb47d7de8ae373bf82c02d8877142b901d89f7c698b3e1ef717" },
                { "bg", "f96d1efc9274f06d5deef1d482497f4859fd13e0a786730464fcf081ef84e1b11e6d4d876bf93c9fe8138d6ed4285649a4ecee0d3375b4734f36567cc6adbdbf" },
                { "bn", "bede10e823cc9756b25180acf8541ac6e1a95c6536cb6b10740337cabde14974f791bd4cb0b4cd7e5bbddbffc45cc0d398ec96f71d11f8bc2c683787eb1cce25" },
                { "br", "a9022c1bc4aa427f114010e2c2e23302e15cb680ce1da5890060257be9e496a3517bd0537a01fb2f78cdffd7bdfac8835f0d325c19aeda77c991e809885cb8a5" },
                { "bs", "15411408a6aea0c5deb11107cf60c668ae148378ffea38617f2b1532554554d5fb72fe13c1df10c737ded230beee155f3b8f4987e48221108749f9bd5bac9203" },
                { "ca", "42e1c9c7759b0d6abfa3c91d5a1d9b34ffa2081eb20d5a03770bd208ef6cb9c1ac48fbcbbaf3d46e21aa4c743f23a50f0efa3244b96c10db3d7a76af3ddd112f" },
                { "cak", "4bc242229e33b7f14f80583f748c87ec4c0f6000cc164fd16a42c02dfc04091efc092272fc350797cc12f22679551af02d11c993b53ae138cadf87d0fb969b97" },
                { "cs", "75bfbc4827905ab3b0eeeb0a7e64c8b963d8b9746ea65094bd68883dffed8ecc3828b8f22b247c8678c277bfb84686fb9048543b09391a426de841bccb87ac06" },
                { "cy", "39d1fdc3024e83448aa11efc8a4d69c9c6fe3b5a95b4d1a6549cfd18dc70caa0feffbb39f9beddca2ad1a5b35d1c81959b71afe5dfd79761e78e4cf837057f36" },
                { "da", "a2ed7cc90775560a775bec7054ab94f1b6f97f5631bbcf0130706f7508ccb9f415be5b38aca443372b61b2b4673ef916779a445c38dc4b6d2ccbcb56700458d6" },
                { "de", "4b168417b24cf96795a12b453400a144e78dbf27b05421db95b51d645481930fcbb2e1a7482ffc72732c9ad0dbee544f03d8007f0ee54161a96539a7e17fe780" },
                { "dsb", "912094ecce2c3022e27e7c62d70c82807c83cc1d82bb8b08f96824b9c245d0c325f2734190f746357a8ee28fa3f4f172d8df32b8c55b91455c7102ae0bb22ff4" },
                { "el", "8b1a6e1a9c63a1c104fdbba6026e688a351a98b8943d1bc54a548af5e917b0dbb2acf4510d7ff9492840ba7e31777eef41ad0efd63a442df3ce32c3a0d6f3cfc" },
                { "en-CA", "32b9918227d48bdb7ec37e03e1ced11ccfa58f9f4b3a962903d594102295768c350dfed86e22d0bac9c9da6ae06a0d9d79ad0d9823013130c5ab0c4a17f0d4f7" },
                { "en-GB", "0e3741a1c461b7061c225ccff8225fc467a80b06ee450c4dff5e1de29b43c682819411b15f3dfaf0606167b5404423c8dfe5e15469c055f28b4d2fe683027ac6" },
                { "en-US", "7c1e8c24b36d1ff6e12c6266b0dd131523286c9f8dd61014f664dbdb30cd350904196e3831f2d4d153d0528c5dbe413cc1e8b5ae016e3d5aab30879c1c463e31" },
                { "eo", "dfd9b467a9f213c6f9f268f036ae664a41e804e2b759272b73c3075141d228132b0efb5e60af48c526a81930372d2a114e3bda7595cb6d1fde92747b2c504acd" },
                { "es-AR", "9f4306d7d5eb734314ffa45086c5994ccb92741d25e5fbd1add6b83b594db56f04f2ac99390571eab08a5f4ebbd4924d2a634e61c2a298ce5d01b10eb96518f5" },
                { "es-CL", "1db486c5557535e8dca9ddf60c32527903b1dd339a6201780d7c80ea32321a8318188cf11130592cbc2895700b1e5ca15900e7638ae69ab7601824c530a6634d" },
                { "es-ES", "d474465e61ffe6f25b4079392123b8670069b3bd9841ad09515eb474447e1849d2bd906ef9496a31d2b9099e6a61dabc94d885678de721c7704f97904d78759f" },
                { "es-MX", "03cb5307d239c4efff89a87abc4a1a104ee16dd9d981a2f1f1a41e39d329d982a097b51b974726fcd90a878d77f8a2195296fa6de1ab4a872abd579be3ccc105" },
                { "et", "8a673843e1de5559372c68c632b8ca51f5e6b15e58644409e02ebaf0de3e513375051db300dfc8b31c081d1e5f578a5eeedb6ce34e9cdf5b048431f81b2827e6" },
                { "eu", "0c63b0109114b198b7fbf884e940c5c02fcdbb4f6d06c0e71f9dfd89f50e2000f6d2d18470fe266c2b85335a8d7ab3a11f305dcdf0fb522e9c1e6ece23116bdd" },
                { "fa", "9188fadb4e0cc9d43265e1f00451349e44c23306743e1ddd2b6eea02aa3b256bffa1acb957d24f9b14790f91e8203e890552854f98e717a9c9a2a958dd0df203" },
                { "ff", "9e2d59355ffa4141d42be02b0c10263a850101a967de219b900e7754c140c57bc6b2a7c0a8515cba0daebfe59c8a09eadd6f6f02d30f3543b5e733dc3b92e989" },
                { "fi", "38446cd5ced4e6a29780906c420e997ad7b4b12b297ba2f53d1ba97c2e969d30094291f20da624b201f24bd96ed0d149e38a22b2fdf1875cd4259660c11fd4e0" },
                { "fr", "bb758c3fdb7c5b88cfc20f4455f84ce0ff5060e18d17760e10bd5a8581c194e25c06060d45b1df942cffb470c3f9de780837eee5c6ae24465e875466f0ebdb5f" },
                { "fur", "0beaab47e0fae8f535b3d0aadfcaaddcd3bf9ef43597bfef1f1c9e2584393f137224a803e6265c9f700e7db665e55634acd6923fcdf59df1733ae9fe80552179" },
                { "fy-NL", "23a1a22a2c2f2f0595d88b8a32297ace8dc54796ebe5666edbb1dcc89c37dc2b6dc43e09c191817b2c25a4b1c67b5c05984cec038872f6544513a7725a387253" },
                { "ga-IE", "5129ab1934a98de232845413975ea8ac827345c797f3a60c10b26d3ad3a5ae9a10110b591e817ad6dba6ba2fc631d338ce7910ec84d4e77954af4af37f79f58e" },
                { "gd", "bb056c124ef5044a8a7d52288c6397bb87091b23a188f7282f067661282cae32b48d990cfc80dab0e5e7fae755ca38a6183137e0b4c86fa8fd9834f8fd403cec" },
                { "gl", "8a97add02443f75929772446527d0a720315c8bf3dee322c52442c2acb63c4abe48b99d0596f516568a7b55ef398283d2d38c7ac5e77f30dab2fd9a7adadc372" },
                { "gn", "16642a43f206894ed32e969509b10ba152e559ca9bc0cd8cbdb79384b9fddf5bfe82c43b0b22da19eaa28ce8367ff2464f0f2896873abe2a40f750f07bb82aba" },
                { "gu-IN", "fbb336b453ee34c7e4e0d94d67bba5546bea2df7b44c135c244584ef3c24c246450b4f17fca4e611ced484c9a55560bfe48f311a131a37272c67805ff858ec71" },
                { "he", "220a0f85fc6602de3e06b85c2bb925a9b77c23749d2318072cfea9dd7f89a13a8d1bb39033f67d7afbe85ca2d48cd1402df48d0511437b5171358d124c58dc72" },
                { "hi-IN", "ef180881029722a2a5f894c8721ddab0dd5d88b798c2596d658d29491c8d13416b3545bb6db859de466bbeae44ec3c9249c27d58a6a87f220b299719b7a57d19" },
                { "hr", "72156feab5cea26df5ffcbc03c9eb31965446b7fcc89007e07acbf7652a2b2791c035a708df5521156887ea303ef7d071e3f0eb0e3d924195e5ba40448505514" },
                { "hsb", "343d970b5cc33e6033b720b0bf7f81d64fd53b08e5137076f72debb7a85bc378ef35bf966a7fa24d12770582e25398e79ebd6844d452c8ab505fc66b29ae4b14" },
                { "hu", "820fc7b0e7421d77c495d914d803289f917c326efd990338a130b577d088dc55b048abfbf82a7c571bbb3528c886d9a0109d3f01537d584d817f0213e54383c0" },
                { "hy-AM", "151a0b1d14ec58566b3db2c4f81c7866d4acaaf57f4dabfdd415b33761ce8f0720a5caad7cc834fb3283cf593080a873bd2c5de2d54c146ea3cf42eab1fa88de" },
                { "ia", "55542a73b185abd7e18521c561b7c14b14df0188cec7b28a40e96adb2242c61fff7226154941e2431269bf43fba38d351f6f87b6b6e0d9228bc57ec5b952c51e" },
                { "id", "1d6bd29aa57fed585fda68c3a5779a2a332375c8b6e50ed186109568a046789dc6560167b7e1c014a900a7e80f8545ebcb3e57cf6d0186c6958eef78716bfe96" },
                { "is", "af889f2768060026997c86b9c4aa282e36c4000f177ca6860d509221e473bc72d05fae1b5d283f4c94589986776db872cb69e3dab67531c74d5cbef7cbf196b4" },
                { "it", "1c7660da3e3f7b9c91572118890bcebddeda37ea24fd991e8a6c88211f4d562eb7fe78a79bc91ff5e44845443f7b3c54bf5833e5816420e3250a597c24b355ea" },
                { "ja", "f9e0fa58e6a6168e976cd2065fee2492e179397018f6e8df83baba9eac9c4a367f625b6693a10dc4941d4f4018f06f435eb30577d94630eb23c1646384128af7" },
                { "ka", "98a1e5f0a7326204976668255d1d36bfcfa6e1c7b88459ba18490aa09fb262ee1c612f85954f88b1449c068f689d8e79deda0899af5733536f68354fb2953f71" },
                { "kab", "99d0fcb40f6a71ed8805e6974b89c8e267bbc80aede1cb103c3dde28a37f90c593d8eefad4c2d46446ba3b2fdb7fcac86aec0c3da8ef666907d692e98cdb8514" },
                { "kk", "58f091ac2897c560a37a5f37f790db32084ce05a014b30cde417f99a6bbad9e21eaf3ac87d81bd626a407f45e9c84a5201f0732b68ab38f8f3b9efb66e69e24a" },
                { "km", "e9f5f2dd49b6854194e787f83f12ae0439701145d54415c11ac291a712ae8b2e5953eca12365b8a8aae5b6c6c576501d9c42fe1e4484a531fcd3348e5e1ac395" },
                { "kn", "fdd7dc69ddb4e514f24efca3debfde6ca1b78878aad6a1cec42df17912c009d4668dae2b2a940c730bdc790471189d62c903393e6be3d84f7b3f4292cd8fb176" },
                { "ko", "0ed460f5ef79c1d450f9e331b6b10e2118ea78e9ba36ff36ba9837d72c59011625a6eff5e2848cb2626e1a67d1e259eb2e8d18becf9db19cb3b352906c7f6c50" },
                { "lij", "1db9ec5c188cea491fcae9d4d72511b9b490eda6ce0e29bc49a980491bb9450a4a993bf63e169da6c0a02322575d29fe8342666bcc52128e619b0a86afbb4614" },
                { "lt", "543c85ecf26d24223a4d6e2c2697331283585f1c0359a1e48ba08555c99deceff3082a1e4c2743561245f129faadf0f9c7d218e4accc7442116b94b9faab212b" },
                { "lv", "d4f4215eca9324d8cb4c593035b95ae5862eee0b174afa6b307ecc8cda5d3b8fe7752860dc35191f4f2426cdf9fc27a438808c8853348d556efd707414e539ca" },
                { "mk", "a5227ee8e8df4186a7cf15d7fd3be20dde77c8c68c0cd26dc42d481739a5c3b7962c9d1c02fe2fe962db895e455aaefacaae3e16d68ba4811201eef3fb5a2914" },
                { "mr", "f7b6249970f17f6d075edbc85ba3edfdb8759150a9b4fd8dcd4d49189060a52b262009da784c1d3cba52906bd55d1f3645ddc3894dde4329843db7298bbf1bd9" },
                { "ms", "a4dd42ce64140f89c108527418dd73ac4218d3df4c173f3fe8bc4ada7eab86ca5bc95392e4a9721d5f96c0cfc7b34b697c8480e7eb6fa44d12dfbc714fa765d6" },
                { "my", "6391162fb339d8ffc9925a7631f4505df93b78609f70e434eb95eeee2a3a7569553240bb51363c0ece374410c8e4420316f42cb2a707c63fc9929b6ed2157718" },
                { "nb-NO", "9b2f027956d01aa91b56a8179166c95b11befb3cd0cbf2fe3440834684259ec2a6738230db31cd620127ad9b7ce520330dc8e669dd48db0ace59afc93b0fa4c4" },
                { "ne-NP", "5145ed5628c739fd1db0817e0f5c5323fa2cc2a842a582ffac2cc4c140a6388e2577fd6574b020f71deafe5c24f2b6629eb5debac464b7d331a7cf4de1bedce6" },
                { "nl", "93383c60632fd688407051836e1fec981dd787dec06c38f5b72a8ff6cfd18ef57e60a87aea0404a5254a7b5f4d31370aeac55aba807e503a48d64e31e321a22f" },
                { "nn-NO", "e159d567193f4ca2d0a02a4af2b46f381d29244b5ad89d33e777cf3d4ef1da82394d7566a6551e95db71e459101f215478fae5c68729bb762c37b9ed50d99e5d" },
                { "oc", "f3240cda8e4c127b5cf6c45c8d083744914af9b871b7e3693186be8f3a87a5fc8be100400ac87a72c6cc9c3747240683d8691e9426d598c81a9803231b1515cc" },
                { "pa-IN", "ddc30bad88d26c28135069a805009e0aa666a1147b01d29a3e639f38e8428c835a4b995a7b83a4b4eab22ed8eb83e5827b8e812addf3646339cb24d01f988f87" },
                { "pl", "94a78f6bd2033bcfa9796eb85b367f5a338126a9f125bb6c181c4f33992d1096935477bac6a07fad57dcf27e1d828f0c233a8e3199d7d5bb03b47346fc78b58d" },
                { "pt-BR", "f112aedad350048dea12e17eab8e65b9875a272680ba4c64b38c7f9eae94da69ef96f671b02d135256621aca20ce3b7e8c3cd1bb5b3aaba82287ed4faa711aa3" },
                { "pt-PT", "64de742421740f484f2cb3a82525b5b99eeefb8fc4bd836eaf71e6abe030a892608fc0baba2c8cc88aca0cd3379045f505a6510ee6d913860bf4e18dcd9e006f" },
                { "rm", "4e8fd9676a989ead7e07034b52ebc6ec295a19155f701a2871a5f963e1ea4aa600d07296532db60990e34edc1aebf46da8368c328940de828a613b7d96630614" },
                { "ro", "4066b6f1374db3443448df3233aee3d53bed3f6994c84b30d04253bfbd56f57abb8c47b731f35dfb752f58273ba0f486303590bd6e0c346dc6fdf47853d75a9a" },
                { "ru", "9e86396ade543e13ffd3abe0d8653831483a17ebbd057a4c90b32225e9afa89f4eba764a2d4ec40000a8cc189de20a7473fc635199a1d195ec838fffaa2202e0" },
                { "sat", "af8f8e2494513da8faeae7b709d76e8f013ce2204d1384ef362b71d8866ed7333f2a15e03ff4d0c32fd14ab1a04d6b9db2f7c4c137f3b5ab3cc53e9f2cd03a83" },
                { "sc", "88c9175d51327e622676a9952817a801a2e21724efbcc4c08f9a74292cba459d0323f1f7e66f44c925cafcc72781faf29239d35210fa9944079634cc386f5219" },
                { "sco", "77013892d24f5879457030ad427192d194fc122be9de32df7444b717edf08abd3406b7bb52551469316bbc3dfbb8c7626af282b6722c16abf23c9f7473e922b1" },
                { "si", "9b68fa104eaa999946657a0f014d365f7e54eac56800c5623690f932070a33125b7f69ab43dbef614f3116b0a0a6273620b92ed81d223faa825d7902ecc7f4b0" },
                { "sk", "046a8b32465b85c6574cfabd3d35b6096935dde9e0e9ab0a0d7d88862d7369f41ed5bb42fab84c08882edc8704a198e22bb6495ce2e6463739fdc0b3c7d32000" },
                { "skr", "55ca311e2dd94ddc82a132f452053c90a00203ece875db9ae70c0f225baeb14eed7fcae1a885872676c9d0ab267c2aecb2a1b6ef38b2a8f4d0e3a0c7ac861f97" },
                { "sl", "33b195236a2cd32c4564a2fc5c333e88f9a2de5f1703451371f96614888c610f7d4a6fabd86b644ec5d966ecea3ef1a3c21c774f53d42860705b27d11ec7a3b8" },
                { "son", "ca6d5043c9e5cda3e9b72c65a1c137598f4030ecb45d63f81f3532ff70231fbb8692bc1891eba3666b78cdff4131244fb526ba5ce732a6a41e1165a8e17318d9" },
                { "sq", "925f7aae42d419c60ec265a221fbc011b3bd9ad26c9197a3539c0fc0362e8158224126bc2dcb9cf53265d410aca804381859374cbdeb7b3b1b76621d0020cdd6" },
                { "sr", "d0f45311a33fa4d2b7f36c00c00f41886a935ad749bfdc110fe0ae52ed413753fb4d7448327a92f3460f5eabc3b584da9de4a02bd9c22d7f209d444253a6f65e" },
                { "sv-SE", "e4c0509ca0ca97f139d1ad0cd7ba9c78d03ef34ca6c24afaf0a030f7d8b386e66cf47073d1cd033abfb0686c48ee69893c87f9ea3e49564de49c4e9e27153f89" },
                { "szl", "17c1f5fd28f4b6def9e0f7893773467e15d2b03d08c049a2b7b6e0ed1384fca666ec03b99b21dd7baed6e122806eea48eef4b947b1bf10c0cb05d02b11aa46c3" },
                { "ta", "bd24e5c5d5be3ec2a209266b4d0477c574e2c3fce6dda96aa81d973070c6c401964ae38b707398d70643807ce1626e81e87f95b0a470a7b08f46613c05ef31f9" },
                { "te", "6a3c044b58ce8db09df1b601e1be01fad99548d9a43f85acad8f736cda3b2428563e1a708d9570ddbbb80297bed4837b6f6d7eb0ede611e1148664ae6d09c013" },
                { "tg", "de676efe952ffc84627612802cad94bbbacb043640cbfe784bcef34c112fbf5cab1b6a3d54eaf9567a3fb013726c7cd6ee1ff51102e4948208097f7772c9ae9b" },
                { "th", "e5699f696a813fde328e8d98546ce9f8fc0cdf7de9b252b56f4be8a6608dbcaf9eded11d70be774ba461150f1ba521b25aee9440a91a243a571732f7aba344c2" },
                { "tl", "0bf41d6ac8f1ce263014ef46ca320d083f59bdce01b5a2a2dce30ce731cb2c00f30cf79fba3235a403d2010e11ad487425d687641137fb8ebaa0a089544c3c05" },
                { "tr", "17669bbef77da3754bed3f99dec9e9e4eda64621861813dace92cb03c3abde3b4872d5b9865732ecdef3dfb5d723eba6cb1e024748e8585134e5b3cec72e593c" },
                { "trs", "631ae7fd5640e0e4e5e3838572ef22bbfe06eba8ad7d334acbce368735165ab5167507910b3e88388db91598cf4884f91a380d5324c71a3396fcdf21c93355ec" },
                { "uk", "0264beeee017c2b79ee9675648e6390038c76cb66f7eb72c81550b113bcfab8f9b90bea3f055de4f8d69b443f49462611b88669ac4dd05f22336a628e21f1f7c" },
                { "ur", "ce34ac545c7a26b3eaac7ddd596770d346127e62d9dfdf3cd067b7e8b2da478e1cedf398cc6d13473ed35391ffbe6bc8a7194facc103414394b4da0197e32ed4" },
                { "uz", "cbaa9830f92c71ddd2d1d413effc1e34939d36ba9a7356fb3a5610960160e5c09bd14a7783fce71ad9d47064c7af1d10f76e9a2f1f48ec608df6a00491bda75f" },
                { "vi", "0d377745219873f38aaf42629353af207b476b8e12209493e6a45376569dcf89b25781666acb935907f8b897039cd109fd5a9b34a65b6ad4a5e31191c27bb1cf" },
                { "xh", "70559c59814b9cd679262f74f878d84867bcc8f9385cbfbb0ad9aac7d3bcac04273cd1918d4df029073311dd8a21e9d6e9cb47a6765f28f7c5e254f53616d3e7" },
                { "zh-CN", "b485c063342126c05a7c8e16760d8030e2c7d39053d67384bc07221d58a7b733abd32e92c973e3a9905af5af4cd47595c1f7ed38730b0aa8ea3bc2e85fc7474d" },
                { "zh-TW", "bd7e2475025b04336052615f8b01035f9a5955a921a32521d4882a734faf7d747ebff469c21f404fc8f596ac188a387be89c6f53dcd2d046a360424b397716c2" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/137.0b4/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "73cec3aeec7e26faa276b98a4a839c8d108fc914224c62efd61e5ae19c5112a63f401801fea275b5dab9f5cc846918988beda8f3bd699855a2b9e7c5db9e8483" },
                { "af", "b4e2bda0a8a5f138bb7a4afb48f656e26ddeafb6f28326c6f48f222f96205ba6e43e82885ef9d3212fe5e61ac79ee5864b55edadd6a7c4cadda9bebe60602502" },
                { "an", "14d3cd1cdf07fb82e927ae1cdea88b9775bdc75ad8cf0ee334892f51d7b0450df1766d56abe3879a35fb90e3754e320d65eaa7de38ee70e0c823df807569d0d9" },
                { "ar", "5202e8fa49b7024b5913e8197ea72a22d401a0146dabd2120c54f3241a3270fbbb688367fcf5edf4397cf448d056de70cdcdff6dec0af8704cd37a5dcf4d296e" },
                { "ast", "d04f55b2bfe6ecac20bace591932bd62020f59fae18d3d56d61d9f5794b59ddbe0f028cb7f1ae020cf79580d59b61bd7666e570422b61ff64158052462eea135" },
                { "az", "ff2f01bd990bc0d49fcef8cd6185fe621ed5bd0ae7495e85a7615be10bf29625dfcae42b3abd017ba635060abd7b981099ce2e572354cef4148d8d41fb5626fb" },
                { "be", "81ac090ecc71dce182e7b57f8c8b4ca2a140df0f71bf015970a6c15181c506b17502342870573f871682cf43821943a1f6c94543da785aee46c8d7ea13bf5f43" },
                { "bg", "809145286dea12caeb0f10e2417d7ebef9291ec457f9f4c34d52bf9a1e17e978f0e41a7e5ae8797409bdf740f43c7bb3f943ab3096d2dee2ab0b501ebff3321c" },
                { "bn", "b38a7added0d6dad17898e16a19fd0fcba78f2d3a8f35776003c1202808d8c1675947d985a59e3c68858fe18508069edb06f409e779236ac9b36f175bc0f7064" },
                { "br", "8561b10475c2ff09f5adf18122a26b8da0294fb343d31b1050ea43f9bb672d14c4fa0fbc6007525eeb982869e2a85ccb32b152045c7b3e4af5de7905994e3001" },
                { "bs", "a55cb4b3b74e9599f59bf9ca197f1963d63756fe03a6755cdaa54a304f6d0dd3e533ca2acec09f148ef406e5cadbd8fb1fb53102965c6c95db01b970a76da5da" },
                { "ca", "9d42f155195d07561fd13c85b52b1d4fe8d52d78fd07432e2da4ea5e9e36162c68b515e48cca8f9a85acd8dab393af95264efb49593ea20ca2778a55bbb5191b" },
                { "cak", "95b499ca85d88ac17d0f812c4f365016cff53c8bef90cc7e59060c335468cbc6ddaa0fcba0d7a7cea9c60b4dbdd1a131a7225bcea3ed69cf51e3636ed1991dc1" },
                { "cs", "e0387cd03535fc9c396846d55f17d4b33a0f8539c4597bf8f9342928f44788c37f8d3a4cd29b5384104f3b7a3ab9393ededafe64cf5e88bac34fb97f8b5f05df" },
                { "cy", "cb75d29089b775ba6c2d1f74a668afff205e07de9397d3b607770b20e80536b261bb1680b66323c9a5a155fd59280cadb3073589b29e3b3f6385b7417a6da061" },
                { "da", "2190683df8b3e48096e474b8de18273ad88ad5296576efb14acdbb0fe5eec16d12b0b0cfb4f5e406e6d1cca4bd875b43e11f65f4510db4f10d620a628801648b" },
                { "de", "ffe27fd1bb141b1b7b6e046e80369b2dbf5e5b6ce07f6de027509a78b1f86b3dedc2e6daa158e569f68090929a351ec3ae42a46e1bc1ac642904d10d6230864b" },
                { "dsb", "b40ec4eaa4e189020128058508dfd484777ef632e425f9bae64f3410ed9b81d8e66f9e25845ad1148dca060e3bf61643aad16d4da48d31bd93bcb42b45e0f657" },
                { "el", "26e921c6fe6a68aca53ee70f4fe6abb8057c9f947676da15bb9662f5d86297cc92d607acff56b41781f485e205085e17980cdce87a7e641310bcb469d4464a7c" },
                { "en-CA", "b9226fa361d9c1bff537675554bfba9068cc65621d113ac86a39b0b394f3f9fbf61b889d578468a2c1565e64c4b4463bb5c3d80dccadc9b376ff3082ac0ada68" },
                { "en-GB", "414d9117b97d98e966471281dfc45fb500aae20d482bcc296f010fb9243317474f4d70e56572a6bbafe54e31210b555e0231ac55854b5f3194cc2fb86635a5e7" },
                { "en-US", "0c5c08159ba3adbc72c997c2ad537867e9d4e1b1ecf1839f9d1d3b65cdd156151062931b0d839da24adee9fc9f1d30471e68d74e9e38e7ef040fec169382b295" },
                { "eo", "66e5e351ab921dc28a7c34b8a75362d401ede7e2aa949effe56b875fca26363e9332ba3c68613824acf0713f996d8cd3ad036ffbaade75961b219c164240c486" },
                { "es-AR", "6eaa4881bc7cf32f62aa887e7dd2ad5c279137d4cde30a5659cc57a342f6e6f7666de570db6c587858c200555d40a8e25a27b408680350b2bde78e59ad1bfafb" },
                { "es-CL", "329417edd33c9f538558675da644f241972853e6a501c005f079e8c4ad5d5f62bebbadedbe46cadd09de3d23d301699f374655c9345626717fc52b6e42a916e7" },
                { "es-ES", "245cb5377365f9b6b47972677a3297ad4ca54271e3193acf571a95e62a4274da6c666ad8ef6104ca64632b87f3eb9e09c74bada913fff3bd3f2ef48645898cc5" },
                { "es-MX", "2af30410441045a3ec6917838daaaba746aa81a887d8dcd12827f4565cbeef0b9da420bb1ae0687c4f94c8b20fc3ac591740f7cbe1fb304faf6d3aba3d095920" },
                { "et", "81b31e8e27ebff76a6112f46e862b49966a29e33160f1cce4c6c83cc4d7558a4392a395d77e8c7d52358784d8d03eb07b5ed0f7d49fb77df31c6f396e3311f63" },
                { "eu", "23c823ee9d5b964cb269c6099bfee88f38ceb05e92375eb662170213519a9581a07f813cdcbcf78cea4d860997cb4d7208a4d8d9607aa42eab1bfe0ffc462cbf" },
                { "fa", "f3eef57ec8c7fd4bebf013ce1a01941c5cbe7590d576ac41fc2b74cb8066960de1aacaed5b7e333c4997b5957c8d3a10580ad895aea4334003c68620d3eb0dea" },
                { "ff", "66d88c6d17f8693ff014676e6ab562d66f6e3f284b04ed0eb2915c5ab63779fcaa2723c895b2615fef5cec33f8a2524272c69d893cbea4d136b97792b1086724" },
                { "fi", "9873a8da14ad2eb4733bec380243a1fd34bc42e7890f3dd789f052a89f56f08b790fa27f2f027f4e74d59015fe91fef91af82dae4d403a3a07249de40b06123e" },
                { "fr", "2764374b9c96737b72ba0032605aec783f7c9f6f8fa11fab59ca195eb0f7dd9b31c0663c5f892654641a15e79e8ca49c5138038b990c242cb614299688364b68" },
                { "fur", "f5da4cd181acb69846b5267d022c6e3d47d88ce9d99357a44b754e347900348bfdd73b42bc7efeb5c734764a10f1a6ae7c161f0e73c7a97776aa2c864a447ea2" },
                { "fy-NL", "b9b78b2805be701b4a93a1ff6c5194d249afe29daae95b5a889be94df9e5ae6a1fdb4e123cea3b040f3d6afc0efed9870f2386d8093c26903518c29a0779528f" },
                { "ga-IE", "a2f890e0c21eff32506196060e4f95988a76baae2cdd24f644e76d75f16f2339ef549843cece5f7e2f2aa0272f4c96146b7c10419763bc05716f07bc88801785" },
                { "gd", "fff65993ad93b38e948a5c556dad27046497640830e568ef3f36612868d24c4df067694f607eb8ec3ca979cac531310b7a3f423fea6f8c79f01f648ea5456251" },
                { "gl", "f46049252a5e5d927b5d311f0bcb8a75bcc15db77b3aa74727e5169056a82c839e784a0ee6b1cd05d557a5aa3e7558505530a84cc81dc9508a4bc1020407399e" },
                { "gn", "fc1c19d7a9b9addb7cef063d86613ff99936224ba07fe762357aaed405cf634ebb313b8a0fc01f517fe7ba316ceeb690393c7d50acab212d8085ca2df3d54809" },
                { "gu-IN", "e1fc090287659afbf05f3462e0920ad804ccea9570e4398735a9f22716af76bb47557036710f9590a4045160e5ae4ac182ce428d8da6d487bdcebb77e5c765eb" },
                { "he", "dde8d31baf477f6f5bed93ce80f26a5d005c06421e5554bcb7b7589060de5bbeceb603b10d3ab87a7b13a38e16f44fb4ab9cde27254ca56b162a30a74728bba1" },
                { "hi-IN", "dcaa2293512d23964d917250cd61ff29d400fa14fd4d1f2b835e288120339284c99ad29052761d468f5b7d448c3aaf4b3902fa7cf195eec987e58431ff02517b" },
                { "hr", "23af6f2b498a3d72a83dc703c6b143333fe17c70ce6061d4c101c7139c83e31c3e47704da6bfd3ce23c4e3761abad3eddf1677e3fdc2055da63e3634b4b8fb47" },
                { "hsb", "84dc8e85192008b02ba30049d9f85c915640821eb7499efd55475d2f5dfe1b10c949ae4df0b35dd7bb3192f8decb5ef1865cad4e54f0af9463a0e73fb0a52d73" },
                { "hu", "724c8f6285c76717d80ca5b0d84efc703cf01d1b76aa50cd4aa1ff8f635c211b9511de98fef6a0c94233b05bebc6a245d8f6ef42ee23e8289c8f5026e9ba09e0" },
                { "hy-AM", "34126a2f1a86a790b4e3d939beefc62e92a97647991a7e3eaaad97e192c214fb4b27a92444e7bab2f0d3a43c456e3fb78156e7a404bb2c5d24c801cfaf6af391" },
                { "ia", "c6118abdc4b3224cf69e1c3b7e44dcdc7c6f3a5b61e06b48804685891bd7591cea2a8bec2fcbb448971eb4db33d98ddd32e765c3fc695889e832c71459542265" },
                { "id", "6a77e3cd9f1f84304359c6a04fc414e04de937b92bbd3b7ea2bc3a238069e0afc76eb5c22889fdfd97fb665b8500132bc4049a8f86e214fd284829b05ebecc11" },
                { "is", "970255b8a5fbdf88c3aa7fdc4115b467d3e7552f7f6af6a0305728ce6e0a5f78365fbf1b4a098f634bdeb52f614864afe71941f1ecdbb3fb099a4cf07d722403" },
                { "it", "2c0a81352d98692d133f0455a47aeec2d72f4b953a9a379465c49a1c309395213214bf2de85d4ddbe1b12cfca17cec1e804e79c4d5415bfce04e5c7f24b0e5e1" },
                { "ja", "2ba7df28c757934604a3a87e63c1e8b58200093a0faee6f894f285a536782467700a1fd3d145bb532fa81da83d9bb8d9309fe335712f299a45387efb2f6eafd3" },
                { "ka", "45db4def583d3a7a1e42eb51396c79bc48124dad1f1db14ee582c7ea03b60a32b4336b16883994de89001ff3835aea4ef72da04c7f81df7d1a41ebbf5c964c8b" },
                { "kab", "5a7ba16624ced06fb730f0535a5a9419c79b1dde8dcb09158b4f807da6fbcb6fd69736cecb4c92cd866967e1b8b790b4f2fedee5be07f750c34242dfbc787ec5" },
                { "kk", "8af0efb1ff48f2d989d3b194b9ed56e2a085e669df5769e2d2bef75d45114e58598b0c3ae202a0d1825bc13c1200dcda14a346c0ec3bf8f2370b233f3b785ca7" },
                { "km", "a6f5dcd0ec680eebd98b177db17cdb2836715e8e5927d1fe135517011962d226881f033cf284ef091d849a8519a61cee7064e0b531d459597608fb60b73e6431" },
                { "kn", "3b397cfd68ce9be51b7bd5e82ab4e33d3cb3b3d126aac8055a673ce820c4e048e2af628a8623cf31565ed4fb4d6b5458ba2ae31fb2846dc486c8f801780792c3" },
                { "ko", "422a1cd3ca88348a1e6fc141ce851fe5db19bdd9ad7d62f2d3ed2ed2d6de68af1d2c61fa04e052395a6fd48aaa84496eb690fc2e703ee7296a232092d8227de2" },
                { "lij", "52c46e1f7d9b6718494ef2e2e7ec976808a3846da10e062781594ba180e26f3fd1ed58f4ababfc69aab965cd34e85a779bbd93b156cfdd280aacd80eb9971a8e" },
                { "lt", "b8faa3734fd21eb8f2079d0f92976d5f0147dcc47a96f98ef33c8f43bfcdef79f8a63b0ffc08011360f3b379eed17c82ca89e8e1745178246eb8cda50c8e0fac" },
                { "lv", "e35377366c79acb4a792c3d443ed3500fbca99377dc859d4c401f102ec8916912e7d74c9bc2c8dbeca487478e05d02c0415ee06c61440f4dbcb4098b42f4e85f" },
                { "mk", "a5aa7dbe6960f4d4550c081c66155efe75cc1cafff143c5394ece78b7594b4b3ab5a96a02b11d6a9c16841aea61ebfb1d9393f3578a17e86c5e7abd567f2769d" },
                { "mr", "bafea5753be8e9b6caccfb217c98671d2c2250c935a8cc3fe2d57eb500f72a4459aa8dfb11396dc3167b94b3282544c9f88fb26d9c7812f235aeccaa62c24220" },
                { "ms", "96166b2aaa903b9377a8fa1e9395f3abe4546b8e954219c10c2416577b0d23485a3c09db6b1965f20a21e10f782cb4b219f54fa035e7b81b48e6684600fba2fa" },
                { "my", "63392428987de6e1ebcad12abf21f44c4882ce0f2895ab050edd2642cd24d2cab43501b3dbb8fc6f341a0988614fca470c9a9a967ac91a73691bb687cdd15b7c" },
                { "nb-NO", "317a61c866a683055fb40f30c4d293de4a11e7d710bc21d766170da19439d6be7f8b00237f17b6163bb2c65db1bf216e32c38c1240f4db11ec605f3b616f5272" },
                { "ne-NP", "ae3298d901224b1a30c9265aa3c5050b60e5def4cd2c95141da60ea05b78652350a8b9a604bb6a294a6783df86078ea7870837ebc1d70d0d52e53e20e9402c4e" },
                { "nl", "eb4773ff4e6891f4f26f26912837efbf1349ce90079d9f99ea11fe2d37af33e6d71964b0003f3ca445a1222d32ff759dc6886bd99044a019e1cc9a19e78d4e56" },
                { "nn-NO", "dec7282372fd978af260a6f74d99fd18888140004b88d30b86d73491216a54f65fff3a010328440f3ab7f01077fa3ac9fcc47bad331216d55627021909b8e3dc" },
                { "oc", "219c537241befd0f60cb20894a1521b24f52a1e0419665379fa1afe2f9e81f2c5b762cc0de8543288a508c148d3c03fb117f3e3a4db97bf8b0533198e260e3c1" },
                { "pa-IN", "a2ddb702d79c7b79e6a3b2df0f2e9482fc6c24e8503f1f926bb13423d434679482741951b5755ab8a0df01356cb0b434363d660f8f76ac543a2ef85d3258aedc" },
                { "pl", "6892682e74390da1096289c58f33bd5c8f89ce940b5b531ff598cda2facf45853e1ac595b696b0b984cfeed8ae4f49b2c26ecf69460892d8c4c802f857e94a96" },
                { "pt-BR", "99dd58815772b155c26abed20136577f93f27db9c4112555ffd65171fc2a2f6b5e9259893d900bc4001d0c3690d42dcafb0fb668dcff87b7129427e75ac9b6fd" },
                { "pt-PT", "122b8b999ccde3e77fedb7e372ea0fa87e5f0e467f7194c13929b951db989d8e4cf311e7456acd11f3ce6ac6136257949a498b2cc513d9231c96ee170874c189" },
                { "rm", "51628830750fb26944f72d947cc8aece1e185e26840f123d02b944f2adae9fe36bf8b5175acbfe005bba820c2febf61f9e556c58f496f0c6897fa13344a6ecd0" },
                { "ro", "e87a757049207b10d68dcbd657a6c5b48c61a222ad244aae1bfdb85be065afe5fd65f93400f0aa2b83b7684081aed3931044e4131c4a61158d9675d4382d7169" },
                { "ru", "eec5fa284c0cabaeb03916fcce54ac33a4979fcc5abf3157dbcfd2bec1f80389678a3b23739f428aadaa45e7d3685267eae55d571345441432a8842e2f417dbd" },
                { "sat", "82930c9ea5695ad750d82d7bbee898073292a38e4072d5d91a3c454c3f3ae30b19c67f1d9fc66c2bbc2168c9455622e0d90af8624fdd04d52e6c177a5f16ebe0" },
                { "sc", "ed7211ca5fbc647dc611e6797bee9dac0a3e87323f6d2f13339e83c4748ee558e297cbdddf50292b7e8b72b13f58a9bf72e79caee1c27cbcf6ea79a72eb241f5" },
                { "sco", "032e79f166a5838dfa1018a7e8b6cc453d0aa7a8a03cbe1c68b4343039c46539b5b5b2fbd542ec7049dba7c6bd0fde2817ce21d13f862ea13b3c956393e22e78" },
                { "si", "d0d104adc2298aa0d0feaeda0851637181425b44701a4fc60343ff05035090f4ab94284cc7d5c62d6ed7d0747212cafd402795dd481eb1c8c36bc0632fcdc362" },
                { "sk", "ba6f01e6c69711addcc93b5844781491aeb120341e5784ca1cb876358b918c81a18122f6e5bb8b1f455a5aec8b7895623c72b82779957d5829e3b9aa50ead08a" },
                { "skr", "1633ce4c21cc57a60f4cb3c930d17edad95292a9eb67397d8fc8e4ebde1a2618d1bc6faee8ba68385cde107f842d707f97494b20e2b0d99641e4e89e69e170c8" },
                { "sl", "aec66985814948381770da7dafeada5c7706c8d7becd94fe65ed8b5797ac16bd96ad285b60a9c74caa5ad63a9906a163c3963704355420a71c95d4ea7955afff" },
                { "son", "c593e16d265ac7242f0301cf1fcac5ee2db3b66ad842bb44ce4bfab556cc2bd4be7b0fd1d42863180e643b66009668fe265022b303afc5048a2676308cd79ae6" },
                { "sq", "7c1f05b6714d89b783ff40f68eca21d7b27b440ce62fd0ddd7553e63d9efafc4c5d97009831c878d725609ce121e3b928a752eb9e15afd34539a5219a9a8d088" },
                { "sr", "44a2c07b9a6c15b3f053bd41abf24800a2fec77dadf84560c3e0caf8a1812b9246b4156e655987668db73bb8542785cb7c1703c4776157eeca39e18fef38533f" },
                { "sv-SE", "242c331ba86fee37951e4277be981c2afe9d4171e7da82b60bf4ffff207777ca190bbc4fbbfd2b022ac16c808966851865cf358bbbb0bd2033532b64c3df64ec" },
                { "szl", "9bcb87939f8a5b4ba8a29e67ee2cc5b8e05b4ac31f68985cebc0becad9661c12215975650f0090f7f70cf6b388a8f09ce593ec67a5876387ef40872cd2b58374" },
                { "ta", "54d675d48d54316b1c2c0ca312d54e86bded4584270a25c5164f49cbedcaecd864c32b24e72b2489ab4973ee683ac86068a582412948302fe22d4c907ed08bd1" },
                { "te", "6fe84ce747cf705e7bd2dc6134292127b7ef60b7095bf3eb3221199cfd95e4be663f0fa1a3617c6d37350126b8255a31770fd5522ab840efa98869a29f3ec3c1" },
                { "tg", "41e9eebed5fae920034bc6c55f347d9c0034315f14bc947ac75f2fc013398dc3d00383b4678d2cde2a35ac4d551aa980fb235c9e39ea270efc832063faa8be5c" },
                { "th", "b606ee08910f5dff21b995d206a4863dd24b9a6b0a1d68f8997190df4a5a5d674fc73fc27e8b1c14b36644f293c805fa29b6c51a72d76cd86a372fb8924f4a86" },
                { "tl", "81aea2058639aff37cc07d066f9c1e09ed708e4d86b811136d1815ebbb455fb2c7a009018558ad73d33fcd96d446503103b9913467d33b0dee191da7292572e7" },
                { "tr", "a32fbd5e0202e768b6ba37da56157d61c32ff96e9706f7992c922c12d02991d4f7ea6a2d488742501147f2e8bd675de61d9e3e4f10fdae4bf91e7ca95260c015" },
                { "trs", "6a8a618c843d0044f6cbcf9f7010d66152a258721be4bcca2bcf788d0420effbdd4700de2ffd68e94cef10aa0abec00980c44b99ef3e1c155209d992da4d3c0b" },
                { "uk", "e760e36e2f440847f475c2d8e734e0b9d30e67fab819f8370df12c5e79204b0e0d45cabca0743098085ff8f9fbb1e7c99e88fb0e6b45405ae4299ea071ce8619" },
                { "ur", "a55dd2897abb22fba7429c9c07d0fee56fd98a8c521d162233160453cd813fd95762b1c163e091646ac6b524844b59ae7321d409110bac84285d5381ba49d647" },
                { "uz", "f639df5b132a97d05354378a06c256d8eceab650295d6eb4400e865c6e7e13ba5a578be14b5de08ee12ea4ef5dcaa44bfdb9a779510ba6f83e6fa47f32277cc7" },
                { "vi", "813145bf2d96b2394a10c8aaeafb06cd3775bb9bf5c43bc14e655d415349c04a9d75ae335b6bcea55729c9e6a2b1b2887e996e6a1e8ab7a46bcaa8facd16e9df" },
                { "xh", "dc626d30ecbd086572a1b0f78ef4bdf2b9a26b0ea9c6254e774cdff9e11f6e1d73db35ab78a547179a8191b8b6898b0bb6900c22cab49620553bf0b2c19e47c2" },
                { "zh-CN", "808d0d6bf1afa87ff07b15e4133dd964ded36a8e75b0f6d41c83325362125273b4efb99f0fb2f756d83dd623875e3bba4ea60d5e69f36e16d317d3030715a74d" },
                { "zh-TW", "2c14c797c0d50e876cef73502194127438dd6d73fd63c5612c64f45586af872ce31532b172f76ad57b95b59c15da7fd530a02cfb4a066733cb80e3ceb33cb5c0" }
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
