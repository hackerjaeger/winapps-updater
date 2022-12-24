﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022  Dirk Stolle

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
        private const string currentVersion = "109.0b6";

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
            // https://ftp.mozilla.org/pub/devedition/releases/109.0b6/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "c140459bfe52eb424b3f35ffb778763a7cfa16f6156dd960ec40d0bbce91b536265ce6f581eba9088458251309ee4c1b96f7c8cf7172da7dc8cb309cfa6e835b" },
                { "af", "98ce9f90d3d4032e7bc6f1f8c8ca06bd798f876317229e2bf6a89cfc1713e7a33e9e250db09c55f397b6fec2c4e5e3a49d938c56b253e8d694f759cb9bb4dc8e" },
                { "an", "7dc7aa8a5950cd3bc7c42205d3249de387de5683f439756072467d050bd780c7fd1b5e55d45ba4cac065cc08705883f460fc1bb824591f2b1759d7b5d1ff2c27" },
                { "ar", "14fe395311802d386329d1016a9d3c118953e8c444ef961a7acd62473062b1a06fc1919f0f54189304abedd470f4a1f2f3c30d121621ff58a06afd4db5464688" },
                { "ast", "7a643c2a827f7cbf842d428087604f4d8e84270ef4465a196cc50a9887e91e8010402581ba88b48e762ef2c9aeb971ece9cff6c46476d698eda41b2d4667de6b" },
                { "az", "cfe95c5cb2610ff8c745cc8421db18bc48e52c0a5eb0ad534e6bc9e5bc4ae30ea11ebaddc04384d245ead31396bb0ece2e6e5fab86cf0820de964652fd5033f3" },
                { "be", "46361ab602a678ecaeea1b1c365e44bf243238d577f2ee70b4780145f7084b95de159e3f4bd816d5e2155a87c6677a89f2ab7ae743de2d85109eed38592426b8" },
                { "bg", "09c9d6bfc1d7b3a22c1479da9d532515354d8e1456b02d4b710748f8212557d10758f698a415c89043b6b7240a94abe65cba3b8a25cf426ad20fc0b5caa8ed25" },
                { "bn", "aa9357856bceff3b3314c4a498caff0e7abbb8c6df165087f37b0e99e34e077fdcca2fe60d37d19e03708c3b000dc43809ff50cb509ebc21f47d8ac608bdde8a" },
                { "br", "a72de699c7fa7d975789ce613c840f1cc4e4a4771bd7fdba709a66d479e8c5ee1580accb8ec001e1655bf7ee2c69fcb68a66c4fb0d8f400341c6dccbf2deee13" },
                { "bs", "9cb2782e8126dec147a9473ef21f32cf24f4d98563954b451a18a08aadb9bd6a5b90f97ac0ed9734674b9cc9fda80a3809752b4a13c3a910a795ab069b258c93" },
                { "ca", "1121437cd27233521c7612ff3a8037d5886b7b80cd54dea99fd3e7388a0f0ff1c4c0fd3942915cec4dc8dda42641766ef3834ae036927ce6dbc691f7645c55ef" },
                { "cak", "a637edb4ea73baaf9c703bc29f5c1353a98efbe54d36d8cd60bae35c61c85cf668379ef33139139d65688801312842f65b751e1f4ac02f66e9b57d7df077264a" },
                { "cs", "86d4337a75ab6595129cf4bbe79a713675b382dfeb3f63b64024c874e549be7146802e783a11672b5ef5fa214f154c4039021748449742a4144f14f2945e7a7f" },
                { "cy", "144c3a73f12f1f3981064a49b9b15ab911ce3e505947a6999dbaafbc09b25b3a09d56211bad9105c86bc0501f28e11baff5d27288f11bfc13aadf60a30899c45" },
                { "da", "a3bdb58f590794827793b1d0228528952455bf13f47244a5b7372ffe2dd004f57805f30dbc3645f990a0d4ac34f9a63dd4309494a2179b53f7940172dd9d3bb8" },
                { "de", "a12b7a9fdc48c0ac76495e16adf93180f42665cd7d14930e9b0c4e3ffee21558a3968fa66dadee92fe32c5b3cfe25b30c3ffd971813a179c305c338d7ebdeffa" },
                { "dsb", "3eb788e3e8a527c555b89af85a71b293bf79eecfa477b6b89aa1219ed8ada0989b860e2aed2ca53519e72544a0b24ff0e798fba6512b525538326d8545baf275" },
                { "el", "d29c97ce2c7133f3aebd876e6f0c9b7999c794ab2bde7da06fb8d4f23f70a90492fc3e756e18342c83c976020d544dbdd825f7be338c7942cb2561962cabb299" },
                { "en-CA", "d91f9f841c607c18b9373dbd8c66b11d1b79306c49d2e3a4115f0e470f7064bccb44e168ce45350e284b94a12e91a728aec02717f5b59f7698d161b9e470ca5d" },
                { "en-GB", "249b7549b14770a53fa8fef5102d30ce6666751be393e91ae3db33e4927b60729bfce2ac81e2013e46f1634326da7f76ab7b2bd705c81a41495ec55035b49198" },
                { "en-US", "2ce312c11b5b290fed6d6eecfeea96ca8f455abf7ffe087b7448ae59467bfcf30bb81e0855253199441fe679fe67f28e6799e4aac70b9afe7c8e1accf6255788" },
                { "eo", "31c1aabdcac0518ce8818530527dea2e99fcbb316a53806d5231833555a6a50bb1ec7dff9e17d7db4b73f13f77f98740c62c4c945e4c7194282e4671d297b076" },
                { "es-AR", "412cfd8ab921eb05b221d08efd1c3fb2c95e6339149c286c340c4099ea403a6f164fc55f7523d8bab05b3f9b0cc33ac55378c17466eee0101e07f66f393ced94" },
                { "es-CL", "a8521a5f83868b283b0365e512c5f7a2526aef4a3c16fd432896ed30371ad69f7e7b101ac88e5e0ae9849abdd38e25fa1c894c82c5900d00a3479c6509ea4b68" },
                { "es-ES", "fc847092ec3fee3581fdc85b6573254ed25d015cc74fd24c7b153a700fe8a04865c00af1c91ad086e7382e2f64708b202ffbd2cc0180b5985eced522d44a1b0d" },
                { "es-MX", "1dbc3ecdc076ac02fc6d53ef5f341ca918f3f02cdc58a18494535dc933a047b7bef97756ed8c6e3314a8f2eeeb5a692662b368aa169323c78b9bdacf79cf2b35" },
                { "et", "ebe41d313c472d41c2c96b822d53f2706b35aacd5f1bb9e5eaf264bc63d98f00c7fc2a702a06f276f2d61e5d3b1445ac06d13f3c74a9ee279981ea290d31355a" },
                { "eu", "78ac3cb28dce5206d4350bb281bbd1f443c114dd202bc248e420caf975fbbabdfa362004e22fa836c28bafcbd70e32b14e191c345528f007571d1c2ee6cc815b" },
                { "fa", "d37b467f3f5b536ce65b5d3a309ac90ac25efd6a898826d046b2cf22446a4d8e36af46bc1d69eb15b6377122e168bf68fb664d8c15178fd813f8866a35a04dfd" },
                { "ff", "c7c1d57592489ee85a52184507046f95cfe54b3bda963b5ba885783095febe8600a4056e89f1079405b9b20bb23350252fcf02035e60f7a1c36c56b9bbddbcce" },
                { "fi", "76510ee09c4e57dbccb732365899a59bf65513edc0a44306a34e7768f1df2dccdafb69439adb985553285538d89c0edc1faaa94f0c004c794131f0b81de27aea" },
                { "fr", "2b8f7a7687c047eb0aef26d36b5b08565b0bec19d9d3e2bcf9d0465491118daafe00db7efdad2e894125ca6509d6d3ae7396d2e5a4cc616db3eb7b06a910a780" },
                { "fy-NL", "2013a73934390bcfc054fc899dcc176037b18af92874bc125e6b739966f068139f4959905145b452e50a0845615c9e487e125bd0aed2e80ffdb0d451eca1aead" },
                { "ga-IE", "734bea0e2a7a391cf5820f824195af7b6dbed5c3808c4e80be7b7c13ecf89a595c966abf08b14b6501553fd98cb2067a3da3f2fa7fe3e6c8d8f31808cc38d148" },
                { "gd", "5bed9cd1cb4257af540dd3c122cc2ac4ac4128b4a79e92875477649444e6f007b498a383194253cb2d9f16f8c2b78753d7cf9c8d074fa656bdfdc074e7340e08" },
                { "gl", "15e8278d47f1bc26b15a1988f890071c58519bf049bae5b1f338ecefd1333552473731eeb8c75031c37fed1aa2488091e51bd99488f4e194c6a54d903da8e5a7" },
                { "gn", "1d026b7fccdfb8c1c5c88499539d50dc3274b6b11775ff6773ff49b4284bacda2a06d35a0d0ef6578e645741ca2a5b0ef6b118fc57bd7adf9808c91127e806bd" },
                { "gu-IN", "aa84491e7e253c601805da36f71c0b41464960ab306b146937abcb3600a434471032ff265cdbe326bb16cafaa9bb4922ed882d945835a012fe154a59552b915f" },
                { "he", "1302653aa2dd28199f1278e664e46d7dc4bdc345af2313a3516176ec7c89f59e424073dc1b9d2e619e49d776b1147cfd5c561ca2b461c43d72dfd65b13b9feaf" },
                { "hi-IN", "2b439b9b961ce06b0238734bd7e3898900ebac203091ddf1ac6eb403465e7a34bb884a5904b3fbbb04a968b18343a3a25da4182c02631af9854d74afac9b24e1" },
                { "hr", "d5c01f301ef7068765bbbc1561de10b1d0ff84e34987df49932adc99f8ea86b22178749e2b7f46294890783bef1d58aaa2356f0eab20aae8959ddf8d8e227351" },
                { "hsb", "912f0758e9c55a84b6f65ad07066379f4633866a1b46e9430fa69a716b7d874a5a9ca002140a9a0f573d5c452d8dedf96e9bc29842436c863c1a955e650c0ca0" },
                { "hu", "9680afd0eb49a7a636c9bac663f48453f5856b8d50964159b9a90b64397ca0cdbf84b23a2c12ecb79e602efbcda6824dec2bf479acf963020cc7c40315f0460c" },
                { "hy-AM", "a57e80ba3e23d8d4a8c2dbea9fa81df12603942002b8a037ce0c7860b61b1cb12e0271f3a552e41e35694d9f4b371efce1082173dbc71320673753cef19623b2" },
                { "ia", "07c1654e8b0326adcd5c38ed46495bd64e5a4c0db472189f4bff041dc260219049ed0cb68e80da15a1391e6c8f4630076658a4769f17c8436d594a8b537856ec" },
                { "id", "7b8dadc8b678abbdfebecf47d8043593cffb5ee114e5d01c5f3ff0b152bdb50fcbf047fb8d00447e6c6443937960b3b4debe63942a4c92c041021ef37fece968" },
                { "is", "bdea8113d041662685116b86b1c0013b6887c4b7fd1e61b88020f8952cff0cd637f59c076f7f07b103f05066c44241bbdf115529d08efb225441ecd5a4be1e89" },
                { "it", "6b9c981023e33f00b1dc5f116b8f70af4b5d52c033d1060403de76900889ece6b131eb8b0df6b3a21a8aabf4a7971614d9f72d8932ec2da5c47d60c1ec641d08" },
                { "ja", "dbeb6ed2d7555c9dd5b550acbafe7a25cc5e9ce58c9e62905bbce243e0b416d53c0b066488d62173a9d4100ed84d918c1ffc7feb90510673b96f162101f04854" },
                { "ka", "d6752e2e33ca5a86861346dfcf6e29891957979ee4fd9d46de9b9443af95fc6c6cd750ec01a7a3105a5c90c39c99d98fe246bd2930b30ff5d7c91cf61044d6f8" },
                { "kab", "4a70f8f33c85c2b49f2d4d2cfad0d7df374d1c244efc527fbd8bebdeefa2e32ca8ee420ec3dbe2991c81423172781810efba960b2c1b2ead137e17b4b4529204" },
                { "kk", "469ff1d0061805ed64708281c1c38f9b8c25c0d2b01c44bfc451ee8390d3fd677c6ade27d08e5efdf8f8f4bfac4e02702d3b4659e86bba63ec11365a2533a105" },
                { "km", "93671ee44497b46976ed439727fe6ae7c87bfd692364bb1719588c555db5f3e8b388757b990b7ca69cfb5fa3cbc2d914c8d14c7a581156451ada00d2cf9b29b2" },
                { "kn", "507b0eb9921dc8131be6f9a6e0a79bafe314d8092bc092fe5b79ef47bbc1f4fb225cb25cf69d51e787d5cb3b7926a26718d4168abc77988fd20d475910a40090" },
                { "ko", "42f1e0ea8a51d3c2399130e20d38ea52397395ddea13e12bd0b95633ff53cc52cf606f9bb22044f9e97e524d0dca3f39eb94512fd643de34352837a827a48506" },
                { "lij", "ea3b9427d3be14e64663f78ddaf579d731bb824a4c985c8598824db49bdbf9b586fce914fa87f376965f66350d1999521bac20d2ce3a9ff6f8bd6bd25d7d908c" },
                { "lt", "7c42ab87319b0b0210e65be1f8f5075ff8b2a4cba81efa2f307aef2c92b40b07e0ad1399f4da6cfc96e491d0c548854d32550ac42f543f8b14544e54305f12dd" },
                { "lv", "c8460ce895dcc6f2379773bed4cf64ae55fbc16497732b047050a21092b6cf77233e7fb9e977de952477f14cd3a98a4633b6206e44e0de869fec6afb562fd6b4" },
                { "mk", "866d2b02ab74af1e26825272f69bad3102e046076008352cff744374abd166c21216b8e80fa26ec2c585529b7d08393d8097e5a72f458a5236fd58b4a597591e" },
                { "mr", "10b48ef6a002294c9acd189955eb629699042196f777452d4f8fb842bbba382096b7a3db1525e01aaaf80c5820ebbaaf6668c2e188d5dd98b3a72a18196f8894" },
                { "ms", "f3ced58ec9683f3be37660f210b5746f8e7b57c50f907e2710da4a103156d86f393187be8b29f86eacf9424f79dc574f51254606be63c0abd775402e24619bf3" },
                { "my", "5e083b1ff58ceba93268e77d2701a876971c7d3536a736e8e08a5d641f1a0140a72785887fdf02c04da1c17d33b04981ce9012ddb2a46180931c3c20ea585d6e" },
                { "nb-NO", "88bc761bfebfcef1ae85c9ce7692178d1488d977a34dc7d2642625bec8658dcf34e9cd6576e595eceead7c559b1cd000fee4521c742d42a2a339d9d9447450a4" },
                { "ne-NP", "057b3d467e24a300c0388b176496b9770f4f708abd78525af2e78d580ead892b101bbab0180c22baffbbc1b7b463e2ad7d845a9d1a86f3f1d3478daa54d531f2" },
                { "nl", "0e95144689f983a441136fe0a768c8d31c6a75e473975d9eb3d6d4c33b8656f6ddf57c904034e85bec77fa37eb57f76dc0f96c7835ec3a7cfee6a402aa2f5baa" },
                { "nn-NO", "1c48f64373b9022b64b4e5d0c1cc29584b79f55c091815dbdb853fe0aeda1b74b54268f055ea4377d7f6b0ae1bed693d6425dc149fa4651f04c9d7980f071d33" },
                { "oc", "ef35d5b7dc72f25935a01ff6f12a8daf16a7f006e0f232cbf325f36e5ff8912c61e286ccd7ab88693cfa56c6d35656b38ee884f871d2e238026f788c1be1fd3e" },
                { "pa-IN", "9ecce984694585895a4867e75ead1f64aad14a16248e8d6b7a32ed9999a9e357fe0f150e06f75bd9c25169c8952c720136148c43a86332c62b56db90dc82f0ca" },
                { "pl", "0d45ef621acf11cf1aed82682a7ad7ad54be52e30139821a9b87c5e1b6b7660b7f77c14455757a38d2b8403f9ab5e84d55de1fb680b61ddff8cc680ba6b06285" },
                { "pt-BR", "be9c91675a0a436d214f3e6a7319fe0b29a4eda0018b050157219eed5ca4f82f28410641bfa9cb2e26e3e8c8878aa3cfe80d2674fdf560ae960eb96447284ae9" },
                { "pt-PT", "4023a4cab1cf27f23e9b4f3deae36af4600b5e5d6df64acdb297ad1dd6c3527f5170f7acfa2e12c8b00b0217dc7a0803c585deb333c8e5fbc63d373c495c0469" },
                { "rm", "848d38c93936a60cf78b3b9cd631f1f3f89d31bcdd22288d49da88f5ee3e32670548e060c56d852a507b0845f67acfdea7b984eaef0f1e2ae5bafc4fff6901fb" },
                { "ro", "b89645343632ae0c1116908c01d6d197f555e61a28d0f6489725eaa3a0e3c29ac19c504df8b05b0298c2f826c78a5f5088c20e9ef334ced78a35e278434e043b" },
                { "ru", "97303c58d86edf5b044934432c32b641625a7b0ae07f18cd39e172d8c02f3c18fbb81f0c940845fc29f838f69d0379ba22303d42f06de4f30ba435a2eef7283d" },
                { "sco", "d2a93f0f2449dba3b1aeaf5e555fa47895a9364bc28478e1c1d9c0f560f6e026bd6b9299c7d00461d3a100dc9fd741ac0e2b90c737afa076dd6488175405f365" },
                { "si", "4494d9c0c5a3ec3896e9ccfd289f5bc42ba460bc444addbf4879dc4dd9749e8268570b7fbbddedd4c5417409336224b903a4c18bc6e3b60cb2251158c2240eb2" },
                { "sk", "dfe1576a32c7db92d830f3cf025532bca85e16eb11ce2e821259a24cfcf595c711dc36f8f0878be3d5336062fc0cb1b43ac4d833ee8347019ff8f57138c343d7" },
                { "sl", "9ffb63496546583d1eb81e47796234d7228571f2fca963baeac819683634bf5d0fb7383e5d87b7aa4ebadd0893e1a88fa688787afaa53ee15d36b069fa85d26b" },
                { "son", "4fe2e988a524844707264df066b3dc33cd3a37ac2f326175f591f508fbdedec2c32c6d4be2f9e9208e09687434621eca9fe0a5a61c2b609150ad909e72eabbd2" },
                { "sq", "743dc7e1aa5a421c07e941503880c9f853d4842bb839f84109d1975e502d20e9dcfb9117b476fcad8e5cd261ef79b95ab66ea919036fa7cc5b68119ffcaa2b5a" },
                { "sr", "e4eb2e056a2fbdc94de04618e2dce7cc6223b286bb9756af62bb1d4789e1b8f3ced992eac18d5391ed95f92a0371d577314dfcebc45c85c04d5dcfd3949ada90" },
                { "sv-SE", "7c0d0c1a52e4016aafd24611e726987caba94118353a208919a5d865f81fbc1789b50e2ac1fc14e55f1cc7c723cafebf73edac9d1748145426135205a6e2a65d" },
                { "szl", "a454964a9a2719790bbc6618d96af5e640a1dc573368b0d29c2bc61960ebf19e1f71556290049b62aa160c32e741c647d4b13184903829f748bb4d3bef4b8f4d" },
                { "ta", "f19d592c76616a7e56722d91b9ce12a45796f1fdbccd7e87e83bc7f723bddd18be92d73fab0149e935a9109d9c27eb139607f1982c9876cb58520cf1c4461c3c" },
                { "te", "043dddcb14127fdbfec9e6279fb7e78c9e92755f6becc11deb770c88494823d2b2b5b8005acec6f9f7649a80bca173dd2ddca720dcfc3f13a7c7cc4464343de4" },
                { "th", "36a09e7c0ed9a125adf6ffc032bd90124c62b79bf8fb77e8d82b24d667f9c4f3de57e1a3591ec05ae010cf111bddd5e9a902f95ee048d646b45c5e3ccb9a89b7" },
                { "tl", "f465f3956dbafa9f7d97ba2761e026b87249d71697bcc9c28271adbaf49d05f76f1dfba9201c17b8b04d9166d6b099c532aa53c173a5689eae77cf9ab8c6509f" },
                { "tr", "4e5936ba2f5e857b187ebfcc644f9273ec1dd7f9cdceb738479a7dd409257bcbd6c140133bee4bf565e5cc188035481a9481da02a609e8fb67041855e6d80d2d" },
                { "trs", "40bf7de5471de3a5b5df0f0cac610869b338118180d4b82d508b452df640e88b1554549b4bd2262bdf07778b84b94f684e917272972c551bd64cd06e0508fccb" },
                { "uk", "609134e6c94a81bbed70aa14b96b60181d7e8a2a4b26740a11cbd9c94220cbd3117c9f8e4f88964bc04ff5c2dcf9cb9431366c2f9cb21e828a76215b4e0df825" },
                { "ur", "983d93946e6da53aa8cf46959d9ab868bc74966daa07181ca2f5258fa4e7feac246547032abccee954a29b13ad248ecb8f825f1f0ff7e227f82c14c2853f66bf" },
                { "uz", "484543b140a719438a6e98314f2e0d5e8672c8a01b2799a8f5184a8fab44aa0c70c08861aa014db33b230ad81f1f2d584607fd07cce978cc7e6ddca66e511e68" },
                { "vi", "23cc58eeda6fbd3563be19b813488ae5def81b9c5f4019c18964d63531156709ee7873079a5eafcb5322444a1811549e86e9d925558c4e2e65b2fccb4db31b8c" },
                { "xh", "5644414bfbf472043e4e4a1e6a62a3cbafe45b7f15270cd826f02543e90b315852b953223f3335c90cb7a6ddfa2de50d8d89208d219bd2cdf9fc802960c6f03a" },
                { "zh-CN", "e783c6efa7dfdddea896c87b2561647408e50f535ed7e489c6bec77e2de2ed77acc589980a1e7382217abbb67e22ff280494728df18a2da39464cd0b4d5ea2bf" },
                { "zh-TW", "f10876dbd2e78d27523fc2e7eca7c052e5a123923a4f1f881a4234a6c25a85ed96786d5ad7dc068f5abe4a37c1707e7cf9a5fd73bf452731fded4016aad4ea74" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/109.0b6/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "bf415d83ef86158def045b69e7141449436df54071fba10109f1aeb8044a7e7e97c9bec2c67238755f04889af0e675c707ec66772a8a539c1cec1ec5ec787daa" },
                { "af", "ebb3330e7fa8cc54f8965220460191c745d977a6cfdd306da97149a454908a0e27df1602c68984468b30e5977f6672177bd20ae455a3614883317ce9faa80459" },
                { "an", "b14c483edfaeea0372efe09d8dec74013a0b56feea76896ae6bc0b85e85bdf15e2f8b8339d696b4d0db966ca93fb74ce97ef0e5573d3f25aa7fb75bed7ee268d" },
                { "ar", "d2cca06ff9952aca02455034b21928b37dec48ed0af7e6f566159d6361b751049cd514a6db105bb2b89432b7d9c9dd2bf502bd8e75db061d248f55cc8cc8e45e" },
                { "ast", "4690303932ac2984453f2f968fc61e427a939ebeb911f71bad70d6994fe94afd8c5ac736bee9981e37982b61b9154aa3d664897883e328e5513eebb038c931f8" },
                { "az", "544906944571ce3e0cbd98781e86049458cdc668297d46366139de1aa0a3bf80d1b6aece7c64e9a4e1cb4b0d573a3a02e929a2a17870eb78f9d148ed563c18ec" },
                { "be", "0b9dc8c44812a607bd3ddc749d3a4c4c15d10652b2e303fb9642df1f38cc4709bf18dd5f8eb4be8d49f10db7ec71bc7db936152f7bca0a70e10fdccd78bf83f1" },
                { "bg", "b59e0ec38379f37bf59577223b097f77133bfeffdd1964acf2da9a2e49ae615144876ab783ec7a192b3828c40412a6b4945d4a6d6d15d63e7b5a297736d1cd07" },
                { "bn", "0067d0a2b18f1bbd31df9d314b9a580fdb5b4defd73f8f5dd706e96492152c5f1e9343ea8f1b4e6de909639bfa4ee0f8379e2fc160031ac2d66abb20f281f2f5" },
                { "br", "ec24b74f903b95540cbda512f32a96d21f2e08d75b7d0a8d611bcfbafab900c18b1ab7a65caf29a670b15953729e3aae3e859de73fe2a3851e60f497fbaf4fe2" },
                { "bs", "776609c12f3f5d5ea2b4252ed59460ff8b1eba68ea1eaabe8fb49ccc28c4807fb23965757011acec190b42e14f0f4a5aa682f0d3bff92264702d08faf264e701" },
                { "ca", "3623b69c9679910f11898473a1457454d4852120beba74673217d0f6b02db884fecef5305d51c94230347107f5677d7062244c4826526be8ff82c19d31740199" },
                { "cak", "b63088b54e797d92eb6d193fa5c24c3147ac5e94b369150ef1d8312e5862bf684c178bd72382437bd175cfb15f4cea8f661c2c137c9bc54d3325ecb921184713" },
                { "cs", "1e6b26f96a80daca823adc6ac08e9ad1e5884624d83d5219964cf6ed1fcd7329895735a7965dd45dad2ca8170c7b15de6e6a5ab132d747cc6c6e82214664402e" },
                { "cy", "44f0b0c6936456ed12c11325c1afdeea65ee4af5910c3bcf94a3c6c8b66209863d5e0d49584b9fb0dab0cad320948e6a3c3f280d145a4c6a10d7e8133a752305" },
                { "da", "4f4dc763974e2f197253b517967fff6f9bc2c2b9f6bd6197cf2f1b5ea447988cc8ff08669977ba19fbca5ad215fac6b6cab277ad5c98a9d96f7e1b8e2f3e52c9" },
                { "de", "dac189d8511bcc060c88ea08dc2c029a7210618aead9467f494ad0dcf6ca2c0b62dc7d288564e2f1d16847a95b1702063a300fb6e89dde75c41b37ab039dc52e" },
                { "dsb", "7cac01e69113235dbb62133ab3ffdeaa1976c846d463ed6fa494e52bf5b85bfc7517259f844d17d3991b8c24f84a1385ef347096f0b7d2770d343c9bf0d78190" },
                { "el", "d325ccca8192d62449534e88ab80cff7e184cbe9c11ca161d122cd231c058070d4ecd6a69544e29deca9e381edae340b784ef1555fbb0acac7e6ec9c5babb4bf" },
                { "en-CA", "997d7e2ad27d3b4749351b6751bcff2e8ef5a83c4f83550f687a09cf12cbdff75dcf14ed7accad2ec32b4250b45102ee76fc70b4fb8c9203511dec9fd319b3e8" },
                { "en-GB", "ec47949fa7e8d49a0873ec2c2b5df87f830f7f7c8a012cfdd7297b6ff00bd70e9b014d404f222f4ed8f0d948bda4ded7af2263787615be23ffd2762a8f3f0c7d" },
                { "en-US", "9f7accf58971c45a613d7c3f548399d1c82df8130077e0ee83c2e7bc2ad94c8bc31d78662baf31c42f27c04156c6f1cc19394b0e04722afb9a64b10083208be1" },
                { "eo", "045bb9d86973a59b2309ec30b6b71a49ca353cc05dbe50a3daa90e4e7dca59c9df55c12f82e8aaa780b683ace204500454113284bed0c74be04c4add14b6a661" },
                { "es-AR", "8e06c3db70ff3d0227caf7af9a69b9321ba8f61ec3c8393edca30698fd2d82140e10f564ef2a24b3bbbbf987897a62b7c6ea00b68fff2e0035875c00aa777140" },
                { "es-CL", "f6100c143cdc0bb362f6023bf71cece6400b11cb25ee8b2fd0932a6475071679bb5a08e1470f103cf90f47e06f7c7199ecfacc22ac8ff62fc58bcc8d1457b527" },
                { "es-ES", "bf2168834778054b3e179fb2fd25c9158794209053e512af223ccd79722af10addf579d341afaf186706f14bcade8dd13ebc606fbefaaab5079db6450259b169" },
                { "es-MX", "65956a3806c034620c34691fc4b7df02de52f49e5bb63851c4e33f48e428247564f7dfd7e3c5b81e311195d2820970aceded59f33fe6c4c242e111198d3fff81" },
                { "et", "c7e98d63158e4449206bbd495281b4c53bae7915a65caf242ca11db3ad80f7762b5bd292da669d9e4028e5116818b58ff9ad11a3c82681996b84aa4ab61fb2dc" },
                { "eu", "c3aa5ab885f93645eadc02dd103bffdc876787a5e90adc30bd37bf6b6d5225a5ea997dde8eb397bcbe51a83e94e633ddd8d05962aeb0bec391d3873904ca7322" },
                { "fa", "9a020118916bf07cf1fec7c8d94943cd541c0bd376a164b99daeba1b1f64690ebe120551f424d216de4f0bfc1750478b96b2e7e5b1d906436de40864f457b8e2" },
                { "ff", "6c1dd27f26af3fc78ff854352030f80e3bf5505969a76dfebacdbb80dbbe4bc483ff4ea2c756c3273995311e00cee875108a815c71c78f0b7c3e9dcf91767b53" },
                { "fi", "8d66495872920a8e7bcc2da6d8ca1f5ba781f5ea75acd4c24036bc131bb56290ce48c584cc14468cdbd757971ff8ca6580571a9d1bb14babf6c29318b1cf3e32" },
                { "fr", "8277038828f8456242382e392d2deaaec314ee557333cb5a1052ea9ffd49be06148f033e85a159354ee47dbbb0352ae46541f626f0c40b8f4e7ea68c1d57232b" },
                { "fy-NL", "dbaeb237cde3b98afdadc24468458e39a358da8aad8719714cefcc36b6d23eb883ad29431410f232c38df779e4cf9b53e7bf65b54b6c03a6aec780167b44a884" },
                { "ga-IE", "eb5fbd0cde1199dedbfa95b641f5485852e1ad58a93ce2ba2f67e856461aecde8dac0bb7876db6623a41eb40faf595f555834a02757d76ccdceba76567b0275f" },
                { "gd", "5368318a02923d4b5f034508ab95690d0211430120f83e1f5aa1a7ffb75a3d683b5a957a297e97e38881ec3ebeffa0bcd8bb844a36ee5fbc4e6aafc3149398b2" },
                { "gl", "bbc89a516e6f3082078a65707eafaf50b20258a1730204673081ff1675c5155bd3a24a949ea071489e931984a721a10feba6ba157a3469dbf19812ad5cb4e4af" },
                { "gn", "ac4ba26a7831dbe22b147eac6764ba0ca2f8391f784d31ee29b8f72f7d45601b8493c2ff0d09dca7ec52f18324c3802608118ffe798aa3aec230ff995c6648c3" },
                { "gu-IN", "fa55045c6ec2a18f10039258e4469e26e640e8be6f2fb053f8c367eae0727b9d39c2fe0b160c1e3972bcf4c91bc0e9618a9f025a6f3bfbe67e22b14152845f2f" },
                { "he", "61163c3b1ac25fe28921ceecbf07bb08d408cf43e2f9906a44fd90db8afb7260004018bb8080c995d1c1772d62dd3411302a884a83556060f787d3a4cac543d1" },
                { "hi-IN", "454cea36d1697a344e2cf6a7d10fc0664a5fe5cd1d74095765efea90be2964b88d2da0718f001e63a00ca0b732f3d5593f4551c1f67db7190dab8ea9ef764445" },
                { "hr", "0fd60ea7b27c23cfef229e0f7947ee26e6c5ee3e3e80b95ab71f992c7dba7bc6977894496f24b7d9f2c8c1f68748021b5e55e90ceaf617fa2baa44224390935f" },
                { "hsb", "67634a1042febb8723d70f2e29191a68589526892d008fa84082d7c049f5494fd035a0d7bcfaded587b75e3243c0d674ae3d3e0ca48e75e961bf6cdda032fa54" },
                { "hu", "cc33b5498b77e5725966caa68d1a4b501a920b710348b05e07ab84f4e7c8a31b4a032eb7874e2518949b8bb6e7960890153c71158dc56f41877136bd5334158a" },
                { "hy-AM", "c10d8d09849c980e8ff769e6bff556642233e9a1978ad80226629735277786b5dc9152c7b0ecefc66a96f83b7ddaf9fa5a9db3a88db5d57584c45db303b2f2cc" },
                { "ia", "21f7d1532907c2c5f6249f288a3808f2564cd0a9bbf4e1570a5d1df129ab8eaa464a98070f82366270a220fa2c0ca5444a77b395204134588e0feee004166d0a" },
                { "id", "d70f5eda144a80d05a72933409287b9964abd45548bef37ce3259218165ad6b6d76e2f49e725f9bc62065519705120ce42aec469256bb77fb74182d00dc54f12" },
                { "is", "7bdee63fe2557e91c8ae2798cbccc0e6d88f74ff8a680a22757bea4cad115f1629fccc288268a2b88330044696db169bb2c7e2d8dd695adf90f117b3b83dc249" },
                { "it", "4906f888545b408e00c2f6e4b49ec67b0a35b6e1ebe22aa27885e980aaa0fccadf331f5dfaadb6b69e12b38f43513d94a9bbcb8c28de2cb6e52df37adb0bfb8f" },
                { "ja", "0d53e67a4d6b1980fe6db1db02d6782a8961d64c17f2af2c5d4a1eac39397ce8e424aec8dc7eee75489ec8df6af5924205bbd60cf2a0ee3f0b3a4aaaaa8a71f6" },
                { "ka", "d4d3f2797d9d46b7dbdf86f3695589f2f4f0984a5ce411e230b6c57bf7521fdbf12c08cf2f2514c3f9fbf9ec620fe267970f72258b691251d8404d5639b7e43c" },
                { "kab", "db0f2869a1568fe02075cf97699f1e1dda968e89cf2cd512c380ff1c7ee05a03cce2728d36a34a9c6d01797b49c397c0df97e9b121528837572825d44d1c6fd2" },
                { "kk", "267d5f27c1dea4cd0726418630c0cc2996a3257d5de22fe45d22bf29dc030aaa3945af3a29c00d5b291e14dafb89fc42680a79616ac150f41faf49aa8b984658" },
                { "km", "126b3ab872efd7158d7f435992ec6982a6d310e1715a955538527a74b80f5284ecf476d371c0edaf64787df807b4d4f1a1add77238975a9d3c3f48ef8b04a3df" },
                { "kn", "71089b46fc566ced03228eb202f837dcc6ebc03b0d01a3f987b4f7a607da53df9aad0908627dddfd9d4512f3bebb39c960c7ffbc00475ef13cce375b146df4e8" },
                { "ko", "68e465f351feb9699971366ed53c97c891198385a6dc3188ba4bc91cc8a17c0e63e200f8125849e450d11e449656c397dc05d7d48f3f6a420c2bc492e7052e50" },
                { "lij", "951eec7eba4807b4c85e14f00289909a014e1700d9240c6e2bf6f3f84dc4ed8041d4286b51bb0cf0e8ac261e23e0936b7ad282172bac57ca8df7cfcd9cce77a2" },
                { "lt", "03cc1b7caef1da1bfef7973a4cb2d84caedccd21674504e24afc26d47eeaaf2ab0327f081c3239a90e52418e675b7930938b804285439fd08155ac99162284f2" },
                { "lv", "c96a2c2d8c85f863c6df2154170766bcd15d6a6e3bf23e5d8969a9a745a12ab671daf2e97f1ebebab19bbfd860cb75b19894882d2a161e55bdac5df2586cb97d" },
                { "mk", "efca8b5920c9167b67e760b1be9cdf902ededcda86ae45d2f31fd390a617a084526e6b65a821b4352d0581e8c071692f0c0aad45b49d6fcbe751e0c0d03545ca" },
                { "mr", "124b2b0a0dce20f1e6b2799dd89d34bcacfbaced61f9b7caec0bcf687548315df0a90dfcad3e3083f94af47dea3024d3ec5a956bbcd4af6dade6c86fdbcd670e" },
                { "ms", "c021758fda1aa527bb5910d29de1d86a6131cdac47ad6ec74f44584b08b7d82395ecb111a7391a8b78f9179a74e05308be159e767b9c3cb91447253d925b2414" },
                { "my", "64bf7c87aa7b038eb4ee521aab150b555a6366431e0a3b49b031bae47573f63a17418356181eaae8ab0cd16f412c03eee7014a2636c923697f17b52d0dd79242" },
                { "nb-NO", "e1484dceec4318ac4a5ce40b3005074e6b887009fac5b918237cceee7061ecdb07b9a285cd3545ac43538e05dfd5480762715116e993533ee7a51293d58aaa04" },
                { "ne-NP", "3735cf7b9dfeaec40e7d732cafdacf4bd09c7aa88b2f807dd0e1bcf3af8f7b21071ab29f0b430752da27d1607acfda7e751137219c5a8c39e8d27d9b232dd265" },
                { "nl", "6b605bc98d2f67c0a03599fbb420c7b9888f8a9930c56bfc6dcf4e558dfe455dd26a5b1cdf6b4dd1917d89594c442c3f8ce3874c24d3f615000611397b5a623e" },
                { "nn-NO", "5aa150479205dca72dce68c0c8b0ee369cf1703777711dcceac5763b34542d313edf290ee1c1c562e6bad6658aae71b8821f5ec2b29ec3cc67b5bbead04693e5" },
                { "oc", "ffec6beeddd141d9384a6f994c69affd8673153d2ec6e40d09c73d79eb06c2a3a76aaf93c73b3380d39b570a70556112b912808b05ac393e80ca682f4ad4a333" },
                { "pa-IN", "5578e42c57adfb6c7d7d7ba419399fc9ea5c78df6c34b8ad8f099f2e135f5e891a167520c1a6074fa2b0640d2e48843b46846f7e7dac41adaceb8b544362bf03" },
                { "pl", "99300a1ebeb7f1a2f59e0189365b8de21db48cb9a2f51761982d34f89dc91116e39607a1334f061063a6c389cd055becafefef495fefcf0c540bfd6fd908b9a3" },
                { "pt-BR", "6021a1610e19913cfb5f3a3b12529a1853c32c337c21e3f6261735776a1d632cfacdd67b01b3dbc01e3cadd4565b605b8ffad793d0e080aed4948031cf4d3eb1" },
                { "pt-PT", "2d6cabf126ea56965430d625449f88323ffe9082b2e96654bae59aea303157704c3582a261c71a8419465dcfd325e326ec58e9317c3efcb2d0db5fb32e66895c" },
                { "rm", "4bee42a19cc04eacfd798fbf4a2294d7e03a3d1ace1300685bc78cb2d789ee6d475c2aaef701ea281cc5f90ffdbbf3856e4d457763063389235766b3dce635cc" },
                { "ro", "cc16c945058eadd8d611d51b0a46929b56d5e4ffae1d19fb8b2f4722a0d599cffdfc86fe4d8ceee72adfaae5b023071d599df337b174cd1c1f0cfc015f615873" },
                { "ru", "4d09fd773ef15b4cb70ad68b9f0465627d19b2460d4f4b4b5d16e673876c3c33d1a90a8334fe810405b3c2dac0ea1c837d24cf647f82086b86a94a15f7f2fc81" },
                { "sco", "8107ad6fc51a6a5b4ee45969cd6536dcc838aff041dc662453c05faf1a5f5c2d3fb94c633b7e8b701d678ed4707068313ae079140b861f745a5f97c7bdece42d" },
                { "si", "72b7eda0230775859ad9b921186a96ee97c9d27547b1e1d6aa3671a5ed47a96fc9795867546676dcd96b55f760ca2b62a6c3e288485121a99402bf65ec8a1e36" },
                { "sk", "433e0f2747c920e092fa880946516a1eae4bec6cb3ad9f714a02753e5e9372ab7edebb5004e111088998ee3d7f56ce264e8a121d63d645ccd15239a9ebe5387f" },
                { "sl", "13c22d35778a5f2dc43547229cc3a3033f0668c077fc3916601c42c77b9ca8d8f206dbdd04fc441293ec9187715ba8fb6bb8723657a98eb54cafd45138d0d2c8" },
                { "son", "768dc7d9b90c6a2b4cdda4873e1f8003e2589728132af84e3ec23c0980c40507d641d2f188f79546359a2661195011e76f3fcb11317f57b4b60e211722d4f7c0" },
                { "sq", "45dd5e3f49883b2fc6ab7ed11322d99440e2d08f1e2beee58f837f311f1b2c431e733269b3157c620206f6bce01fbe26047787605f3df1e2b53ccde8100a9f9e" },
                { "sr", "33707439b3b2f1cf21aa70cfcd59c61a149b543e652b6c083bc4da4cb55655a1dce0804fc529315c8944ce3fef2916910d27306528fdad020f0456bf7ed7e14d" },
                { "sv-SE", "5c65361ffc887f7ab23b423379ac8818117ac1992b539b54a0a9c227a71f72591bebee9862955292fa9d9199a018f65fa1748ac7d55f8fe0e76ef5193212c79c" },
                { "szl", "d50e465bef6e204cc1006f08bbb865e2bcb0fb3dac2fdbec37bb6f3939017f002be673c4c45d6f663be0e7e04e9cc308ee1e01e8a71b46b7ba31a6387911989f" },
                { "ta", "dcae64080f8dc33b91aa86f499c6d1b1cc31ed8f70628d8485b9dc1eafb3a3a9754a0d180769d7a2ca0ee1b0beb5d5ca016e7ea23f37ec10c21e242d2249e736" },
                { "te", "67c47c8523153875c48f4d225f06efcc0f98805179e8c02ea05c4e70c907e6e01e3ab16c8f751503747566afcf69efc075f7a2f195ec3eeb6ba7d4c202b4371c" },
                { "th", "cafdf91442947974bbdfc9ccca792db7031462a48c0c2ef9245065d0fa42e34ca6849ffc9f2e997237493e1f346f09f6d74186c0ffca04d0dbdef932cd767141" },
                { "tl", "f3eed73c7208f0cede7de7e217806461e83ef910a710ecbd48fbe8cc23ba6331a6356d65f9185265cbe036a820f383009a14163bc96a01115e6f91bfe34ee784" },
                { "tr", "fea36c85ae3bc7a182bfdea8f24bf6605090c0d4f1065403598ecab4e9bcd52c815cea24ddbf65e29877486a3c84249be83496c0bcccfea9df1ef44280004b27" },
                { "trs", "5e3e1e33451764906f13c5b89e62478a991f3eafacf97e9145e3294a6dc135a37edf0c8eb72655e467b8b6e6a1427bf64b43f305568e1cdc63e9939760f15211" },
                { "uk", "1961c4f99828686d835cb1a81b47171c61db451278226fc31aad0518c9de535d66beebf447b7a364e27b399c88fbed827cd764e680b7537d8db5613fd23f1bc3" },
                { "ur", "08fa1d8d6a260f5fc2ff5d20a3b6ca05206020e7260d9ae062b5f4fbabba6116e3f804aa9397dc43221837056989b0bded5b2be487595e13c88e08cf41f842fb" },
                { "uz", "7bbd572b9881ee4a35144f9c5a2959656f1a9679789f6563adc4a6644a4caf13090484c9cec07f89219d7ae98bf0053181b1122c702f45a8dc7cb2a294792572" },
                { "vi", "836f6ee54d4e61b01bfaa1f5327fb7b21b65badb7068ea65042c2185a664b71463d97e3b2ceba9b9a7fceeef256157c58e484eca991f6ef320fce1e503be35c7" },
                { "xh", "eba28422a4a98b8bf0765ede95a804eab625d66ab63b349930792fb9b64ff6b5fd28a9feeac65dd72b1410c4bee344517b6354f867aea11fe672bf43ada0ce27" },
                { "zh-CN", "25f271a9c9d30d73be03b974cdd0a0987f5bda961681919f23a0d6a42126c600bf31eacd3e2a5a5d0177be1164e2c365e47577d28be862dc543df62b74619033" },
                { "zh-TW", "6dc81d714b071c010b20afb66ec238ad8bdd24dbde34ca882bb1e9501e2839029e2e96cc82bf85e0e4b49fc4e5e00f83b59edd8caa61f5a31e3d1f0c0e8b1483" }
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
        public string determineNewestVersion()
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
