﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018  Dirk Stolle

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
using System.Net;
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
        private static NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "66.0b2";

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
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            //Do not set checksum explicitly, because aurora releases change too often.
            // Instead we try to get them on demand, when needed.
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
            // https://ftp.mozilla.org/pub/devedition/releases/66.0b2/SHA512SUMS
            var result = new Dictionary<string, string>();

            result.Add("ach", "2b00e7f0ba7bc5d17fb844aa7d3afcbf13318e158f5f2fdb55764930b597f9df49812a31e9baad31e7a2c75036bfc7e140a4785c3f47c332b300ad90d0f339ee");
            result.Add("af", "f64e7f062bfbae157a48b47ee3ecdcfc3c3c01c984c601a06aca47b75cb574f8cd1cb3365669aaa922e119c1ae7f6ff04b85e6cfbdc55de16ae238f3abf5fbb1");
            result.Add("an", "4f25a55977759d3307560a4b1f877b141acd173cb0fe18d7fa9483a6b3ab673b105f1e58fcd1d6a53b3f70e358a131b32798ff38b8ab3b18d597409a6f29d1d7");
            result.Add("ar", "99754b42c5a1d7cd9ee23c5749dfcedc1d41e894eed34cf949b63c81c156f82f0046e1efaeb0b300a4906de4d38912e86a4f39b6f51c2d0df1d4783f6ae95579");
            result.Add("as", "383b3fdc2efc69d44d8a3f1910cf2bb38d58acf0451e63180169eb4be9e9fc612448161b27798db84ed401d023ee2c3fc987f1b9591f19bc71e47b6534539ef4");
            result.Add("ast", "31cf45d4a17fe985a7191e50d2a25e7cff738b35eee4d83d1d44b9b8418bd15c4e0e953df6c3bebc59d96a3d700056ff82c05e208cc7c592b768d788b7d36574");
            result.Add("az", "2a07bc1bdeb8ec3518ea336163bb46a7c2639235007c9ce9df0a732e3ea5c508c07a72930697911f70446c08fff8b0fb80e102b726634d9f50eccf8f4b4c6081");
            result.Add("be", "ca6e87e3e2a57e07ccd7834305eada94270515f9c7ce584787b59cec1dc734c417d46aefd2bfbd3bd7298fa731402e306b9fdee3917a880515f9fed81929a5ab");
            result.Add("bg", "fef25237a27b3cd153fa4e725dbf81b2d3ae90a02a9f18b81e954092433bc856b78e073f769bf6a9d588d60438a79479cb63f1f2cd89adb0d516c5378e220477");
            result.Add("bn-BD", "2094b4c46f107ef09392f66b53ce877956175dd7e596fc44cd1e46628e7b7828b16b91069a63bd5d21dc2126ee39da79193c5948ec2e1e52f95e307856cde892");
            result.Add("bn-IN", "9d7d11f72b12e50eb61d3059c260d7ca237171f230a449050be72b204d1e3807e0445cf5e37781f4609547086112581dfec0919ac142d7127d8e8f24313157bd");
            result.Add("br", "9fa6c1c279c232071287bac20888847006b333ddd0e33f1bd48fafb51acdef6d26dabf1be4d1fbef5f7ad1159836374723d295594bfe7491f662d11efef426bc");
            result.Add("bs", "9a556a441d3aae84a2bbe8d19d90061333966026326892b997d66761d0b95cb21efa4c626ac8ec1b70d73f2e1b02d556f5734f73604340ec3b16c0dd07b0f500");
            result.Add("ca", "0457ed5632bafd6ba40292e46d9f3ebef15c5e097e44b0b1b3c203a0eb76f4c4ff731abc4dd6ff1cffa6df8892426cd68dcef5b32085d3b88ad512ebe74b2b69");
            result.Add("cak", "c54ad4993dda94584543d402ed459046b993929cc22f38aa7899bb799d8cd2f8a36460b4a3914e8f0f2738a7e46670704a2a632cfc0eff66ef865335f7c1346f");
            result.Add("cs", "194f6449eee37d1868611a5dc18325fbddcf4951bd4ea202028eef80fc2507aa824b0559834f6dfd030b3cae50c86862fd2168ec6408830c4121db206e98a47b");
            result.Add("cy", "cf850aac08085488056ef0d9a8684126dabd45e6ce34e085cfb3d3302bd87bd41748e646a9e169f318342d9bf97851971d1e11fc6654f82d8d70a34d6378ab13");
            result.Add("da", "5fe1716f198b17d6cbf3b83f0f2e66ba7e528a7a8464a387cf89bd13b4cf898a04eefefd0d5c3fe47150a3ab3d6aa6dac6bc3033dfde25f1786a07ed76f0c36a");
            result.Add("de", "14fd7c85efa33b4568a8e2a2fb875d42a5c42c3a920473c6bbd786510f67d9a2e9656d0582fc54b7b7f755cd824c3208346d1c8b8d1e9bcebb21ec04379f0792");
            result.Add("dsb", "ae34ba41360d931611568bc3f1495940d6790feeea6136c047b53d7e1a82f8f66d24c353a45f26afabcbed982b021975b7bae3588c3f172a08eb81010cdc6a9f");
            result.Add("el", "9ad39f7fedfd93d2bfa97ba5847148438c4aa7b440eaada06f60b3a79bb81307869f829a2c61fcd2baf61e6c31b7cf74379b81f04e1e32cc1e27a85e224431e8");
            result.Add("en-CA", "f8289041ec3835b23fc87ad26a52fcb158b2de24ad083aa3e24ff9bc7adc2b2a3dd67c33602c57aeffedc2cfecbe2b7a02b9060fe1bb956513942f4ab311834a");
            result.Add("en-GB", "86fd77b01a82fb67785cc49bcb5b1672dce9477979d6932f4fd92995c6b7057d4abbb7cbae9ccb858996e7eeee3936d817094ceabb2bdae02aa85125818a995a");
            result.Add("en-US", "59d00b5e3d944b37e92d6ff1c7a15815117b703c9789f4a7dada42e8dea1b52ad690ae0278cc61e82000e58c0857b4628a245850d0a0551b2bf6d0ed3cecaf57");
            result.Add("en-ZA", "5c84c374ae5ebdd57754a9592a3d13b2ba0021a2371aabf079d4a833c6911f7cddeae91c6efea3f26fae7ee405cea28c55eeeb7744b23c5b4f0182fda4e82561");
            result.Add("eo", "bc256014e15041bfce5480c143721561a8330d66492257324bb697a8e8b8edb320d376ff873948ccd2fdd4b6111be9f3420a8497e81b2d9227f9280c2da71d33");
            result.Add("es-AR", "c0638e993c736fdd676b3afeb53ba32edb606304eaad95e55639baecf089d065ea50decf922a57ce4714e3fbaaafa8bad19e6925b399af19a0d60fb0dd1eabfb");
            result.Add("es-CL", "00487e8d1d9dc2dd94946f5f733e5174c52756aa0cd3bbc1cd63fd0563ef69ebe1691491f0fc186c94ed89770f00dfab538217c430f8f6efe8f512df7e46ab44");
            result.Add("es-ES", "29184954cbbc49da63cdffbc672959383e25d6ac6932034705c14091eea1a0bbf70c0b287f33e4474746682f0634ba65a0aa0e408902ae954a23b5c8561b7c25");
            result.Add("es-MX", "7aef5897da8ce2a23b764256596ed2e416dc117194f2ba6a6209b4d7eb03ba879cc52ca5c50a3dd14d921ef355a6c6bfa24909fc782c86d2685c6ea5188dbcc5");
            result.Add("et", "0813460e2d3d7894f8f2763fd0cb39e2c2235140035479be8166abe7e22f0fadd86a51d70a8af3f87fa05af9bf2f3edfc76170fd93ca65a6e7b192b03961b68a");
            result.Add("eu", "bc97e96e8dc2681ceb2b8a37abdf6bf5f1207af780a11252cb94476bc2aeec4aa7397e8b1ed579d76b028ff08a3c436a119531c9b7bdd24dda305ad78f6bb52a");
            result.Add("fa", "1a428f45982744ab566245e303fd8cece5620b8c77a1a12f77e91c3fb508765d56edef1184d6e883ab1f5c466834cf3b14f00c86442ccb0fae9b11366b54d517");
            result.Add("ff", "319d4d683f40934bf036eda52665f026e474a6cbde4440e0b2ce3295feb89ff0704373644c6921e79361e623e6b7b51ea89c28f0139c7fd678e7b4db2cb20643");
            result.Add("fi", "fbcf5f7db87536eb1e79fa2e18cfd68e00be93e2dca4339a7800aee004a246b7c4d26e4f4847354ad23509edf9742c97f6c8d39602e4013086d35ead3f313bfa");
            result.Add("fr", "2adbdbf537fba7d2fbf14c3faf4d2ea978df05f669863bfdbf42d73a14cc76b4cdc83573470c90f4f6c7320eba1470c668313fab5874c69e36d67e3190440a57");
            result.Add("fy-NL", "9d5d5ba83d1f823e6c535b59fba4240f90cd2e8309acc62b944ad48480882ff57645b1547e23ef8b8f507fce334a881a307f6559914d594045a460d0dd411234");
            result.Add("ga-IE", "9f37cda0f43c8442c83cd505bb4860c951f6d45c4356281618a76960c0cea45695cb8acc8a0938530a7db6979bc9c514375dabeb937460f5e6d497434ffa12a1");
            result.Add("gd", "d761b74cbf8c78fe9365774e53222f8ca19b2b2ea5d7cdc001d304a171bcb0a613c9eafcb33a7bbcf6c06e8ed8f391af11bb2670696901d41637252c1b7ee4b3");
            result.Add("gl", "326b46a139cf6d8730dead621c27fb28162c41fb704a0583b1b655ca103568ae25498b8ff19809f58673070e0c72f3659a7fb9b8f379a94f741f75be99d51dea");
            result.Add("gn", "48d9defdec26d1e5902a64b6d8c9f4002317c20063861b05f3902302affc67ff993ebf67a7a179c52d0915d01d5f4807e222ef866f120047081331ba4cddfbc2");
            result.Add("gu-IN", "9a67727a7154040b4038458437cf40e099125186227ab1231d9c162db55b02d0bde79bfcf412db1efe3c92b1e4592c1e351c09f67452a29e842d48e3a4874fa8");
            result.Add("he", "be0db0fc094c7f8e54e1554a75dff36d7caf3c7621ae6c38d5ad8f59d793e608d304edf36d660c62ef8268ece30c6fca7dacf00934ad53f4f64b98453cc8ff11");
            result.Add("hi-IN", "905a0dd9626066f3bca862d5733192a95b6dc33b09478024762584ebfe26280ad8ff7ce2c8ee40388a0cf5cc000ca6bda724352370ca6a9ff23a4bb4981e560a");
            result.Add("hr", "0ea8a98d293b24fdf305af5d39303cc4ce9772a6dfdec50310109a199c476348401a4110d838ddd9ef813998c9ed597cf54cce493ad84997c73a159d5b525a4b");
            result.Add("hsb", "18e6113dc32d10efae4c8498cadd38b2d6f456eb986b52070e1132868df5a521576c59c9e1b8dd4bf2bac228baea8cf8f20247f618e3bf3d555957e0ed544885");
            result.Add("hu", "639da034ff3f5c6345f41f75cb85443a1eb88df5318007ef1ae4ec603c38e58bf9a038b5266f97206ddd9acc642405384b8942602bbcba71b5114e62ecea4875");
            result.Add("hy-AM", "2d2f898a6d137f7cc2c8c5a8a2d71f753f07bf338cfa81e68b88bc3d5bdbaa243c16b5afb7fd17174b144db2dad2de8d83773acf6fa1e99aa83642a12b24ae13");
            result.Add("ia", "e190775454f898f2823ca2a21d22f89df271464a7691e37b0c8647875e063a63abf94ff29de5c3356cd142e48e9ac8943850aa2f122d7cf99a771b8408bff9b2");
            result.Add("id", "dc126e08a04e7b3fde82d42cbd2cbe246bf174b6c2087570ab906453c6d2a093d65aa0e5265fdc17fb50255461459658a463b1e0dd0cbe0feed0173d3c2c7c4d");
            result.Add("is", "09dd86419c641ff7b44efc0ce90499b96fdd60a1a0521b72f0d064b4db1c5eb1b60ea07e9841153361079f9e8189963088584b80f0b6c60caf96bff849915f5c");
            result.Add("it", "9ce0c2089f3b4449b5af0a84e2b79bd401965a20e3a8d5007688d43773b3b64d60e6ea97665da88e1757000549d5a77bc22f408e59bc466d0e245a0645cc9a9d");
            result.Add("ja", "db5d3ce7efbb81cdafce81f1b8b9f91461dfb598b204853ad9a6688207e11bccee114f6c4753d80a632efc45fae13ffbeebc0389bc27974083bdf98da7cd082a");
            result.Add("ka", "649009b2a6369d5a4502b25aa556803410f9fff5eec98c4e0f6c055f8f3e9b1b1058ab3590efe493cc7aeff42be132a19442b7cc6aade3ab5c133c703d54f898");
            result.Add("kab", "d9eba9b27c2a2231d0d0ab54311670913ae97f31c9147edd8a245633964ca96bb567258cd484aed43b5fd3e0695f2c1906baced35b9f368f5265e7c2fbf5a53c");
            result.Add("kk", "64e723918e72844d3596efcc9963a4ac9613c898d0da69e883eab9e313d859b681c6c1b40ff27e8d3b05d62d27a3c44aeb1b707573a93dbe0f1543fb6fed94ba");
            result.Add("km", "70a367195b2060dca0a657a8706eeb23849a7899516862a7473ac60d89c8f2bb3dd9b676a04dd062da56d621ca5c8cf4ac574ee7e2cb9bb876cf670de8b5923e");
            result.Add("kn", "3923a5aaad7fddb2d0a997ee613d74ecdc59bbb0fb550df787ccaf658075d7d293d8f118d7c72f053f080e9ee810d5c1537ce80a3c89b93e4f9b499bbe42635b");
            result.Add("ko", "802a42d77190a8ea5bad288b1441089184c5228bf99627aa4d991fbc4a4e5f8a826be90848a3991815ec525adceffcefb8b0c4c9148b1707bbfd29174b81e23a");
            result.Add("lij", "357c9473e08410961a9fabc9c17b73d4dff937d01c17f5c16192226b8860cfd6697d65430fde216ad26ef45d36cec4c8dede45f4f74b15047e3041dc0219be2f");
            result.Add("lt", "bad4b6286a0fccd96d9b0c5f1e47ae2e6ecd820a7a3ddf85775815b9252e28d5bc7bb2b150528e00995c5e7852d842b5eb6a483075d77bd2ef2f340169ee09b6");
            result.Add("lv", "1c238643794802acdc83ccecb30fd10c84c2ec72ed5d65eec458d6fdcdeefb924b38415095eb848f1945076de870e344a55998810723bc4736001898a8a50e71");
            result.Add("mai", "834d0584400894a986679f0aac549ac25b601499a3bb72c46961b0311a7ef39c8214c39e97967d6de50fef1a5875bd39690887253e580d146fe8a47b1eb31859");
            result.Add("mk", "515e4d5d2e2bea07ed53136d254ca2334a585b804a7f4be8c8ec12f042638c359d342f2a4666793e425803c748424b3f1bda206c50667abac87bc020aaa137a0");
            result.Add("ml", "bbca84eb591aead367aa5c637eda0a4530e1b72e3b0a3faf3b487192710c062da1f7bbaa26a93cd485833db4f11a2fb51f27e01919317cb63ced5a13daf2b4dd");
            result.Add("mr", "8323ee93ddbe3fd9dbbdd0544fb449f0cc42935dfaaf88448ed3ac3232e616ae5cff7538189fe5860248846337179cd0a5e2a95ccf28887c7339d3dcda7194c6");
            result.Add("ms", "de207fae55b3597f1864589a2c1fd9e027f168c18b89bdfbd56d7fd202410278284530fb2c89bdec6e923028ce18236f2d4da577d54b5136cbf48926643a5b7d");
            result.Add("my", "1bc79cb3236886d72b883bd27047fdb20428ac361fc3002dd76c882055f3adca240ffd7c2f5b6321f30238d5ea02f9b722afcf0d17f95e4d9ba50fcfb62b8853");
            result.Add("nb-NO", "d615f971380e825b2bb4bf5ec0e0fc9429f5b6ab882adfd35d46afe8f6ff1c4cdc5d1e069bc561742804c3e848a90a16f7b933fb351b739e4b1ecdc654b1e9b8");
            result.Add("ne-NP", "231e005deb21a027d18d76babfabc06b417d33f8b24eb28d0045ee218c143fcac70fe546dd11069055797329f497db67bd4224a21017e4519186f658dbb592f2");
            result.Add("nl", "a01eee8879bff8d091fd6c386cba8782ddc1f13f3e8513b501db74f7358962cd3d4f597457b4375787e2336b98c58fd526943df3940faab848e9030cdbeaf96f");
            result.Add("nn-NO", "c7b6add9af9668c55cd54e332da1b1b6826f27a560d4f66a75830ef0e62af67ef59a542bb72537dbf7c8b6cd6463b1ecfe486a830e2fd0b678a0ea6ce4d72e4e");
            result.Add("oc", "4938afd6c9edf6fb1e2e187e867793fce2b43be0c38cc67f72465c62f932b829a0f33aa17d81fd93cda467455dfbdb95d830137454e12ab7701a02e54da9c314");
            result.Add("or", "d1d2b26ff803685799cfd0594d5d94e21405ca14c519378139914ca7efb12bf691d171437ed0e4afebc4c127210595eb5b6ff1b76d994d09590f9329aa4c8ca1");
            result.Add("pa-IN", "6eace67a88e15db1804d11f4c5bdae709bb99897d0ca0fe7368b6033be56d55df4a78af6d105d42934bdf2c8b58f0e3439e62fa697abe96012872631a566dd3f");
            result.Add("pl", "932131284e4f5e7d47ad9528d1b55f403765373b6adeeddd69c8c85b9cdc055d3bd3d4c4d81c6d65e39a567b58a272f15c37c5809e67bb79d0c08d2f8fc6e631");
            result.Add("pt-BR", "83a22383aa8d821071ea6b686ddba2595d024245f6df442a03ed1e19721afe9c997933fbda91dff59dc8b28ee3449d3d994039b42f257aa1a141ff9b680e23bd");
            result.Add("pt-PT", "f1194a98dd58d3dac2623189eb7c3be5211260c8d29638daa8ca82350cd6fecb2df896074d79aa2d1c0d9aaeddb1850998f79fc5a81e094a41624e2706dcda01");
            result.Add("rm", "8c51007e119a2e5a8dc983cce4597c602f0a9f6b3582813caaa34424d4b35f53c1774c3b901fd9d0c38b1e61c6a3b6532110d467349d47ef3323fadd767fb481");
            result.Add("ro", "706bf4aefb81826c9e1b179bd74c300d8b7d8b36ff83da43e1cc9edf1aa9d164264936705e255f2655d8c98e88e07e2d5684efc798a21be197dbe28ba2e58991");
            result.Add("ru", "0e3063c0ae1d7ee0314502d44a23dbd087c234145b098e76f8fff87ecdc5b757ab11d391614e1ae873758b9cba27575ca5071c6db278346dbf39973cc2a22992");
            result.Add("si", "a7368bb22645ef217a24a63fc6b7123dde2ff59cbadac7de9e9a7ad0d259608c1b7c189e03f54a6160e504c7440c499dcc02cc1df4f8e3b8bc2fc350416b2918");
            result.Add("sk", "060681d5df64ac4484203ac62fab5988001f68ed4da785973a1b90a2f06b5c10660be290c8c9ea4d748f9da51f7e38288085a43be7e2aac43b1fbbfdc665bf57");
            result.Add("sl", "a7f01db9483832b58ee5a7b4c553dad71bc987c768c3ffdd2779e5ecc796e68cc47a78e93c9636daee32edca42466151f9dedd5393a42379f38303b08cc44769");
            result.Add("son", "b1cc1fef0992826841ca76e8da1f6f455a4e40ed027297b7930f75cef45f130e0d8478b23b04159fbf6abe156a67d2844de4e9402f42d006b3960d591d4b5700");
            result.Add("sq", "9c98d8e163e2d1f0f9b3972d0c961b5ed639d4c4359682b8a26ce5d6719d1adf5884821e8b5b1e95019205ebb319b8a999b6ef79fcea047f7a34e74f1b132f64");
            result.Add("sr", "9348cf92669eb004bd027169c6bcbc4dc67c747418a40a40c21fdc330e18ed6f5c759f23cc2e758c822c3a45cca977fd29e6e11d6491be4768dce32d69fb197b");
            result.Add("sv-SE", "3c66f0c25c66c045d2b6ec66e05606c1c03675fa6da0b33057fb1e06679c9761f8d6e4f0a7492df65a04edbde8fbe93a3c36da4c1ed5f429536a21b1bf1f71cc");
            result.Add("ta", "9a8846336e6f003c9e2b8b070ecb56a7d0e8a1a10d1b023ae7ea56ed7f6987d40e3ce3efd7448f2511e3c2a6f11e0215693ef1a21f30d3539a96cc66fabb0c32");
            result.Add("te", "4fce60b45ab1695ae15445a0d2c8751d8b50306ad0beda694987bb02e0a76568ec9eb50aabce1de5e745341b4d7d4c8cdeaff1d07aa09efe3d771a2fd2e3a90b");
            result.Add("th", "18906aa65d64de1d11069152c7653b102e273667a724c3449bb2c36c8a325f3dc4f16aef1e7f4e14c459f23d551d03477e3e0ec5b772d5d098dc8bac0afc9ab9");
            result.Add("tr", "68f8e66389f9e12ad8c5cb283e58a03790643bfba93f801663e7d025f7f49954a9c7c8d3164408b40c7d133bf1ad3132d7ad39dc0bb5b11ecd086c036d586adf");
            result.Add("uk", "de632a6772c7f56136f3bef9f6e41255c358170f193132a0d0b10f33b2d640e52b73546dbad726eeb0d75a261f95a574714348f4094e81773d87b723a58a447e");
            result.Add("ur", "c64134af8924d44dfa2ab0234d54079010801d80606ffcb74d27a39462977fb92d5080237e8b72d875a7749a3d687dc80844728e82b246185b51be4d9dc1d6e4");
            result.Add("uz", "794d268659c0a531a702f4a4a35bd06952e0dc63a015729af730e3a9331ea4e17f396a3e1fdaab3037afa232ecac86bbcb68714761dbd74dc53a4ad208f5839c");
            result.Add("vi", "23fcedf802a923c66cb15ea0238efcfe07a884744dafc820e3cea2bea62de46316553858b7526f021e7862a9435d17e2780e97bc1a48343fd4af8345fb925bc5");
            result.Add("xh", "f2fa92fff4bd6e6d2105579d819cd9a16f19a5e6563c116ae0752b71bd271afd1b52cab6222ac1a25daf6e0d3b2fd99f16a86e1abd8d2335dcf0fbcdce0d1445");
            result.Add("zh-CN", "e2132e53babe174ac900c2dadb27ab791ad975e871e0f69e89414b5391eef7bf680f1a7551aa3ae3c699f1d9777caf53147a96a8910144ae38e1f765dce5c401");
            result.Add("zh-TW", "c127c1c586dba7a602a6b86b739776ff05033e5bad7869c96840e734e62ca84a22fa7b566dd2312032520e9de2e7941d8fcebdbba95aa3acbdd8e9fe5d9c2469");

            return result;
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/66.0b2/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "5fbf2be229a554b07a5d119ec2060c3da3dafeb1e1cc3dc90845608516a2bd9ca14e2739ec3c249df4883eddb3522e6d8a1223370c60f23627fdcd643c90946d");
            result.Add("af", "cda9a499fddcd943929fd68d765ca4b48f60fd4df3a76d4ebe1dfd3b80a517f6f4323f45fed6dcecc2482a6976f6fbdb69776ca87e4826a53a5cb17c7d9d29e2");
            result.Add("an", "25e23f1c257ee1c79dcd19bce069aa842a834bfffb72a2a5cc988330172806fa2f41efcae6528c09ea6c92b60683115994c1b700e72d263173c20f321aca33ea");
            result.Add("ar", "de83ec33020584eea9225b36260d1548e57641358e46ca7dbfd6ed5a1c30471e9d95291d8ab7aef6c81855a4b963b7cc9a98aa50978235ca12d85462724e3ace");
            result.Add("as", "705490b07b5b9f624213e46b900e1f5a24662600ba49d45e9842487737a4f5f73f8e13b5433a9b1ef733666d40c77940fca8513b6aaf1cd01848f79602c52044");
            result.Add("ast", "d92a5fc0b23b67146bf04d80556d29d833175c6aefbc1b88790a3bde44cfc65f15dc661ef7bd39a4be441c050cbe1e1ee36ebefb921db9e6230153b973ead0a1");
            result.Add("az", "8f34d14299b269a95f0fd960db7d7305642af3e996d7bebd8af7971e30892e1bf126051748b07bff7b8fe2338c40c7c2bcd18a888ea9914a5373b1e35d9a7e03");
            result.Add("be", "c8f84588a5b649f622eae5f42a855492056084b0dc2a6ecf70c7fb2e55075a01324bfffb06f5f150a24a1f395b61e26b3439444f35a171b576ef6efb86fe6c38");
            result.Add("bg", "ba71c0e5908f4137e78a3ff7db9df764d948374e55c4f0f0733c29e3401325431e35d765e299e10711858a661fd59ec85ee809835a7824a8e13ea122014726e6");
            result.Add("bn-BD", "2ba9d195efcda2ef1260449e8b08227a4c7d85b08fd054109843e759973326aab9223cbd570e88ae68c75a0dd20335fc88a1b2f5d762998eba521876925d65ff");
            result.Add("bn-IN", "aa9724405057abfcd7ab26c38e50510e3666f099d032417cb356d212531a1ec3e244333fba3246735b567f24114a845b586d81958aef6907e4deafe3e071d6ac");
            result.Add("br", "dc50543eb2a5f8b4772e3ec95048cb53ac3088f6d6603b5d3bedc1898905825b9708b1a7e1d9958295f77fe6d68912a53ee0d19608b5f1afea225344a7e71d07");
            result.Add("bs", "8061a9cf76eb7a6fd1a78e31d60dad187da22da57fdaf3e0b8fda3c9226377d64eaab4e4e99b8d128558583e86bb6087644ce3086d76cd7ff8d0f18570b52ace");
            result.Add("ca", "a647eca0a99d24c3c89c4bcb808cfdb9a1a1f333139e97a70f2c140ca8ecaad393a6561a2ff8083678e36680c15a42eab310a67aa3a7ff887ad91d245b640f0f");
            result.Add("cak", "c8e77bbed0a8b83808ec550f01d2861d58bda9722cbbba5d469b50a5f0a62ccc5e28ff3407012bedafaeda33a21cd936e560f30e4b9378b3f637fc637f7247c3");
            result.Add("cs", "cc99fb1bcf414ae5520515615e28893bf558885027e2caf6439df89dc253e29fc7f56a0ab80972c8a79ef6194c37225971bc4e62dd7273094dfb5d83ae8254cf");
            result.Add("cy", "b8ef5a6fed4fd10ccc32377fea539acda2808f7fc58e7bdb03e4db044d121646c054d57aeb1e5f534aadecea569cf3f692b9ebe89d45aae8ef124bdbdf533473");
            result.Add("da", "66fa9f42fc23c09930125346b207e8b5c340c9b39a72cb98f6809564a209b0312aeb451060cbebfadd5dfe4ed78d4f2c0b2d092f11ff728b9ad17a922970c24a");
            result.Add("de", "7974beea52c75520a197e1a3e40fa9232fc0180f44f15f0db399ae727516d2d7a5339523023738abc5116803b3f25b23723efe1c9a8fbfd6aed2440925fe5974");
            result.Add("dsb", "d75941ade795ed6d4d0a884dd814ee5711bf54a579e51bcc7d1ff32addd1abf4a19f2fc7efe4db6833afee0900e884cb9b1f6fac87bf52393230599137f03ab0");
            result.Add("el", "6c7410eccc88a707076a6a032da81b04217ecc85f6400eae52601b23b244714851019bd2dda2fc377aa8d762391c89d169cb4ee699a9088cf85fa6042e4374ab");
            result.Add("en-CA", "1066b053869696124f483e63172721aa61b431423483181fea60ffd3644c83cc0cecc9c55c55ea07515c62278a1169f38006f18bcddbcdf069bb31625c6042c5");
            result.Add("en-GB", "3f397d58d31a9567b1346f88a9c8c8eca1eca08914f68a2217b5b2b623f2c8ba6b5c7c3813abc892ac01f0a7e77a8451d2c194c190ad8a30fbf1e0a0d4aa99a5");
            result.Add("en-US", "7533c2d4a01e5b6e44ac19b13267b55aa7c3b6e27c3c80149faa8d217b444dc3ecce96b69378d06761f6c2e0faffa71268f87db397951c9a07ba96ec941ad744");
            result.Add("en-ZA", "6cb93bacf78cf13e62af0675828a257a69c3cbc7364f0e6932104302d63a6b527089be1f690ae9d517326a6d73c94785fecd77ec71b58928ecbd9baacdc2d577");
            result.Add("eo", "b585bd980ab57cf178485c773a5829dd6e4359868113dde668179d6173eeb04b3472e97442996477bd9fd6a461597473db68310ebc630690bf66d34defb98a9e");
            result.Add("es-AR", "cd038120333bc6de509a5ba119cc0bc3474acf4895d3c4b6fb391490907a222ef3e59fd49251ca42cd3cdbda526738810512de80e0f67b311a9c1fcc2752b91e");
            result.Add("es-CL", "7c06a1c5e6154381e44fde253ed7825deb6c18d5fbb0be8816b687386aeefe1f9185585b504ee21999c1995cd959c367aff3a9fde1540f10749d9c35be7c8dc5");
            result.Add("es-ES", "06a82d2a822d0bffb7d5e2ae3a32d87f9fa021988a31efede71e4dd807eea58d9c56a8bbb12b8631424bbeabac6378c84dcc9af2e498d045e6c9c4051350ac9b");
            result.Add("es-MX", "3205d1fae083f9780f0d017b62f8e25d596ea0d83e189f26d0fc1447e43c7ee6477af181ff67a16ca889e7e34b79154b822db4c74d67883119c117bff4858cb9");
            result.Add("et", "bf055ce61e0ac8b862ed3daed0cb1544d789e33ddf16717290e0b5905e202ec45498c93f6bddc4f70036e5e9f578748e5e75fb06738f7748a79da26849b797e4");
            result.Add("eu", "46893b8c9d14f65a6745e1d5a9f694c393e225d66a3779d09775b163429eb64b67269f402f9ed6f33c247d5bfdbf83ae5673970fff875cfdc3bb48dd5cefe9f5");
            result.Add("fa", "2ea965166d3083691a81abbcbc62e9539f2a68bcc5618bbb3e279d1c847482723653c52b7d09db668846da3a473754e1d7349dd0b26af93d072958dbfe3a08f0");
            result.Add("ff", "6116bdef9704ddc9adfe7b1b8f1b17aa287ea4cc4f71170a852d362e3bcb9a3cf13c7f0d4f0fde6a140488a35bb63c8544a3867867569f1f4eeb5af8a1abe720");
            result.Add("fi", "c321277dd520296058e0d56f25bc3b5f44c537a33a0ba7d8643aad64756db4022690f0b8ac0f05be9bcc702745c38e0a5f806d844959bb51cf345963c55b1c32");
            result.Add("fr", "90e7619d309b4381ef22e40ab5865cd0f79234e1ba10f6ac2b1e761994c7b2c7b00e0d1f441256d8e685339381f288225ab60f02fdcd7c39ea37ac7065c20b4f");
            result.Add("fy-NL", "8974b4fbb7247d02ac41095317b58c30522eb15d1ddb5db766510f3640dc566698cd4adfc5aafe584eb1a146a6822b754ce99f991e586b206fbd5d9a0679e26c");
            result.Add("ga-IE", "1fdbdae100353cb5fb17629e055d076a38088960862a6ff0d336070e861fe3b3f4bc528483153fd86ff9cc5ed15cb6f33886101871c295fef9516e30efa7b10a");
            result.Add("gd", "e6da81550750eb8d1ebd324e221927c82717244239794a7351aa54959af30c9785b804be5d05019fb7249857d83eea6efdf14f8e9199b020f54cf0751c898046");
            result.Add("gl", "82efae71c870a19b4a659fe0052d27f25c7acca4ab3c5d0c484c6a29217bdabd87844cb40d0d2a7ee6a99ae1e37bb1f0c3700530a94d5230a9ec90f61676d58d");
            result.Add("gn", "e278815b59549a7cc9315118026106f9490b38cba3feebd3d88b7e14849475b5a646e2fdaef8caaec1bc38bce1ec1243ee6f4e38a75643876ca5d63fe238de06");
            result.Add("gu-IN", "164e62d10ffc3e4025f4fc850dc495550661b7e593c9f3448126c3d58b1b7c47d6581fbb622788d1e804b59d40221536ba4ad96b672bb3fd3e013a46e5a5c007");
            result.Add("he", "b5170cdc8cceac7d6959f837821ad6de97edf34085ba47668f6b2ccd977eda63d50d07416f995128697fc6e42c0b241216019aaf5734852a1f41c745959d15b4");
            result.Add("hi-IN", "9a92d95fe62b7daf2b74c29e5dba61d3110da22048704efd8818d94d6e20810e23ea39f32dfcccd268369a3c77f1b0b49ef723c9b9e2b47652c0f0b61bdb4f5a");
            result.Add("hr", "d132f9539b4bd909573c7dcf034093c3a2157875dd70399ef671ae0e404013a78ff90e90253969c954fb0e739da669224eea60114e477941d556e6645379e670");
            result.Add("hsb", "e8c3db4d7f5ff22d2e42179f046b5048dd230239ccd229524b4ee78df3d5fa8d37c791722b515d9ce00dc3b353da2d9493339d0e0c9e82358d3364d16f1fdf07");
            result.Add("hu", "4d1136efefe1a9076c0b9d39e64c2453f397856102a3fe2d9f9dfd9a4e5bb6cf0bcf2e400fcf0171c0ea8f0a41b6b62ee3f226796d5687d5fe7dc882c6851d96");
            result.Add("hy-AM", "17c9571c42584b6dcc796ea522647f4a24aab8f3ee350bfea33ce7fc47f1ed0d65e585531ddc25887d4bf75e16d63d4dd728c6d0fabfe3f0614c62a40084f4f3");
            result.Add("ia", "6e722fb456b8ba6c9b365d6c74e57510994c72cce040d329a52cfee53461933c01891f43e7b264bd9e48b9662229a065b9ef332593b6e8579af6e3285b14202d");
            result.Add("id", "ee126f9c9c24759467e0b31e26e2d4e0368f297ed4a34fe92dfcc667a55dd2cb6620bf75f887ed8a50aba8ede514b5ad81b37712016893a5276b3999ee2ddf8e");
            result.Add("is", "367a9f8fbc150a73fa752712a26ba844e04f65ce5799a36ceb9b28bc685c5d9c599072a8e61d81408078ee69003af7fecb32cc73c7a4fc472e12b0dc9d9c199a");
            result.Add("it", "df310c3d17b264784a2f78082c4aa99c4c40f4faa1c7f0fdc7f5aaa185ecf15a5b8b20717ae38d651c4f78644beaf5bc409df806fe795ea2f51bdeb0bf272aa2");
            result.Add("ja", "a922fd74ecd1d737202e0dc528bd6c0f6be6955351055b8c84fcd5ad64f6d169e0ae4561515d3171f5befef4a8086781f2eab8090ef45aa7b2c4c5cf6643e0c1");
            result.Add("ka", "9573e537cb708e1e936942b455a3f58b35bb85a6f38adc20c989fc159341b8ac4a60b8b8169c584bcccf370bbdddf4a94017022ec0c6e7d70379a933423353af");
            result.Add("kab", "ffbdd7bf585e8c8d6817f4168c500add702ff6c94ba04dad4b49fee7e9ba474cf039af4684a378bcc192eafe7768824b8321fc44221cb76b4b76e3091c113d28");
            result.Add("kk", "7c083d8be66fe2e1caf3c7f1bd9da8215b2fc84ae186c7d0e2e5a37879ca9187462a29e450c5b90e531cb9b7f1aefd2c845389b383e82bf7c875b43c87b7e79c");
            result.Add("km", "14763577f65eeb6661d65a00d6d4d08403998ddfa6e527d6b6bce119c7f46134d8e4f38dba68b3b6ab2a9e7ab38ae854e430e85c22c74a2f15ddd92b38828935");
            result.Add("kn", "f9c0e6da94f3dcfac43c759e2bc7baa9c3575bdc63aa03bc96daa37984a3802f7cccd7192e3a3eabbbfdb59a9899b3da3723eb8bef07d76126581a6420a7c504");
            result.Add("ko", "48e4b0f3bbac12be447e8b19688c3dc0b817ea00a671ccf3743bdcec97e31e78cc768440a9a342ab8f9668c91f98e45b899d34ed5cbbb7541b31e98d1771b3cb");
            result.Add("lij", "73d385c23ac50756a2730576a63e1ce6ed732304d5367530f8dbebefa5879d4cbabef12b967f5e16fe490a9ede14b3f993c920f54218c0e0eb306bba1badd55c");
            result.Add("lt", "1166b335b73234f9306377a9c61c22f54dd5c13669cb4fb6a3487990dbbd7d83085cf9ccf75c021df8774b87d4d202e9775e10ecae3965829b02c25391661644");
            result.Add("lv", "27fd080343769da536bd34169fe9be368e2645cbe6c664e44face58c3cfd690c3e928c3163a31c9c495595490a52021f81a20f603a1394f70d03110add9b8e1d");
            result.Add("mai", "edc6ea8dda9541e8fc59e8da0733a053d3e3927b59acd36ba3acda3bf5a6f9cb517012c59635cd75143dd61fb355cfd7710853a4de82563b48d8fa71140438db");
            result.Add("mk", "126d176b1ca96c21b778a15d93983db2b5a2f54f4b5241bd4d4a23a06f6e60f6d405e5f38b87f2ce530bde5fa202c4a471e70af474e2cd9e569e38d677ce83c5");
            result.Add("ml", "08f9276bcb90573087b77f7adad96db9f5e8f9c74f21ad61bcbe79e4e3abf332af63c24eae213b12445b3a7d63003df34226e28dafe78af901440c1e7a4a8fc7");
            result.Add("mr", "70acafd6ae12bf4f0f1b1a91e4d37c33d44dcccc5eaaf1675020d3e89b64c773c7a052260d59e25be17467018f15bff9509cc3f9c9ea8e0a1e20c64ababf4f61");
            result.Add("ms", "d45460b334ed617587661ee63b8d5736cbdd749d175fa658c7a5ec11d2721f46b94fe2a7768c934ce8c3f3304ac7155296e74b9ad34afabe42e2ebc126515a30");
            result.Add("my", "85cb407d618ca40c71f72cfeee44a87489914a2b556d7aba16d3e008e2bcce60f8b841b2318be648e2338b67faafadf1b517aca46de4d9502c1e275ca032dde8");
            result.Add("nb-NO", "a44626dcc11703fcfef58082956ffaf8a353e9557b268cbf055d8b67ce9091651668b1c753fb54b01225e72736034a3e70a9ffbba9eb81b3d09c3f07db6745d5");
            result.Add("ne-NP", "625bbbcf9eafa68b188713bd1dfe9cbb1f2e7f93292c76a68a3d54bb7101c6370ddae937f63e7376812123af2bbb219b2135a64dd23ec576166b11b0efb7f18e");
            result.Add("nl", "755502eedf0fdb65f769bae9d14d02cee33e9f89063e021d68ac57024d14d42628dff0a949b0162f77be958f3726933edd138209a6e0e2166b7af2cf5e05e185");
            result.Add("nn-NO", "37bec602b7444a20318928c5d85b498a3d98b0fdb20e41bf1fab0a06cabc76b793216c27ce7778bd84f29f0f8b5614fca724a8fe5bef9cf116cccc38c1c3e548");
            result.Add("oc", "74a25494fa0fc526369b0c0fcfe6003a7a050b744b6934c2e5c303f1e3dc7c2e68e39ea090ea358a6a8e20e3892f773a12ac3994f540c25a8db4f8e8cbe955ec");
            result.Add("or", "49256dabead4e37ce095e6f6817c5c47805b5b7a430c6f386aa1a81ec333f133e0529f312650ab96bbb1ef3e80a30873a29cb3f117aa94e7117bf55965bc73e3");
            result.Add("pa-IN", "d30ea7e8f5b7b3fba3fa3642481998bffdbe6cb6f40db61194e5e7a72843b963a235d4e56f6d17c2c8bdeecbc2b48701f53b0b77b0ad1a8e30d381821ecb584b");
            result.Add("pl", "d3950acfaa4d456f305b4cb05bafd746cfa3622cb81f89bb5de593f509d38197dbee4c1482547a9d61fc75fdcc8482bc662d3d3170c8cc55aba075cb98ca3ad6");
            result.Add("pt-BR", "89531ec34ece9028cc48fa607e1fa647bb66f8a2e922e46e12a368758c474991221f60843d9dc748ea4c6c1bde840898cc761c7d5d6e59a23ae9595563c1eb94");
            result.Add("pt-PT", "3e2a646578404660f3fdeac2dc5888f4f4fb13886efaf6dfbeb56a30faacca0d9d56d41f27aafb72f3f6505395f2451b48f5430d6c2e8bea2583cd4a4fc05ea6");
            result.Add("rm", "8f6a9fb50dd921e5373e0b77414c30def4e694904149038d5da74f356063b5bbb59a04671599fe33ee8505189ea337d2ccd32f30c8588dd61a5799c294ff3413");
            result.Add("ro", "181a37548a3afb1296a855c61b27f30ecfc31d895db9f8cc1af61bcfaf6f667d085ffb1cb256434e8ea075c6f67aa4e9a8b088e073cd38d0bdaec43880b5d884");
            result.Add("ru", "3ad93a91238136ad17b84d79b1f529e74c332aaf64145331298b7ce05dfebdbb5239b3da9f7f0cb34fd2ff5b846ad90d3a930539b622581d5f1557221a99df19");
            result.Add("si", "f895672d0c06daec3ca48cbc88e3852068950713d7710f66afdb7308b932ea4505f7513d404a909811c25430174c26d665562ea18c558d11b39a6cbc29b3e62a");
            result.Add("sk", "cdf5a492ae245ba1d8e13389296e09755b67ad8a4f5f7d6db42ac62ac8ca724fdcd6fdad7db1a43d225ea90b7c21120520706118a04cc617727b0ab8c1adda43");
            result.Add("sl", "3b93c9e9f76f432a452eb4b28e41975cbf51c4221b0731a740b57cd61e18af2baac1fff17b80853de3723bf0c4e241b5d4adc59b271c8df8cff07e7cb26db6f0");
            result.Add("son", "fdd599c9f9bc16a7a24dc842ee0a3cf176d9b2cc6ef5bd64551095a9425fd5d3c0aea737fa6f5c5701cb04462e68161cc0d3af80f7200a6d0967857c07417b4e");
            result.Add("sq", "2e24523b32dd3b2fef48a860dd1ad9881b5ce6eb627b9f841c80a6fe1fc71c8c400879229cd8bd3b60821935ac3951af04c7b3cd31ac35eee08eeeb866190fe1");
            result.Add("sr", "595ec63835383a8025b51602c54e48b73d978b9d253df8855b065b319075395e1a6e799959e16729066d1b155a1089fc4f11d36e596ae3469f25746e7322c8fe");
            result.Add("sv-SE", "b96a722db4b3cd07c709bb218f7f1466b660e6f72457bddc8088141b1046a19eab563f2cdc57d0617e1831eb639630104abf11aad6fe86675d168ab507fda6b7");
            result.Add("ta", "c253832cc79481d94318e7bd48dd7828173ef28dd28c3d57d260cbdc81e626de445aa643af62661dfd1209299dcd2be81597786ec174fe3247ccb13b69165d1a");
            result.Add("te", "ef7b204770c0da76f0e22c357838462395523073c9266c223fbdf8613f1e149600801ce3507845174893b3730730db437d971291dfafe2ebd392174aeb60e2b7");
            result.Add("th", "db6b6ab1829a59154c39bbd554ba141562f15008d6de38fa9f12e8abc46950ecfdd7bea501e5357f7308abfbe26901e3d493c60d9906d866c0b531d84c5a1789");
            result.Add("tr", "1f62323ba1cf3018e0a7d84852ebffa1cf96c04207a73abcfe6e4669d413bdbeae52842c04a2b948c6d5126b28f1882330c2cc36dc2a916a71a19a8d8cec4be1");
            result.Add("uk", "165b619885130463e34be04c7fd17e541cbf7d33106088538e4a4de6bf0d5fd2c5df59d6bb5ccb57aae653549c5395e7a53b0bafb9995beb1336a46419b63ac1");
            result.Add("ur", "5e8e9c0127a30be313dac1ff12805ea7d107dbbd2c31eff70bf2addfb1118bc08971893a3d5c0b42c0683ba64db2a1ff994a84e0b565e1ed82383d3ac226390c");
            result.Add("uz", "26fe19c612edaa621f7666c0beaa80d45ca59adb4f84fb43244f0dc3f42e3ca844575aff3c37d4aae176ffada6f743298f2f4bb43b63c625daeb4856a550dd49");
            result.Add("vi", "f86f5739961cd2c03346f4abec0cb1e80381904f7f7eff5740d3a0a1ce834c3e7bcf1d26bdffab4fdca6cfa493533dac4013c9eebf75708697632368dbea6741");
            result.Add("xh", "e8be55221a01d5b9ee6bf4a8770cd6c7b7410bc77771358a16a006f01c9b9e9dbbc309758f50f0aed91bd268823a919fcefded3ce0a444071d722e9ddf60d482");
            result.Add("zh-CN", "7e9c86c95936b2f1bb1fd40487b27fc3d4f51396d15554d9c23a9108786af64912fa5acbf22fc46013a4ca8f4624d8f81692ad1d8a985f61818655a828e226ef");
            result.Add("zh-TW", "e894f1697ca4c681459c818b865467d58fca84b31c148e37f6e2c941495d71c852ea3e46af68bb9762169aa0a1db7a993b39e807cdf8791d1f8dd89e7edc4bc1");

            return result;
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
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    null,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    null,
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

            string htmlContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    htmlContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            List<QuartetAurora> versions = new List<QuartetAurora>();
            Regex regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
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
        /// <returns>Returns a string array containing the checksums for 32 bit an 64 bit (in that order), if successfull.
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
            string sha512SumsContent = null;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                using (var client = new WebClient())
                {
                    try
                    {
                        sha512SumsContent = client.DownloadString(url);
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
                    client.Dispose();
                } // using
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
                Regex reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value.Substring(0, 128));
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
                    } //for
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    //look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
                    } //for
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
            logger.Debug("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
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
        /// the application cannot be update while it is running.
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
        private string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private string checksum64Bit;


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
