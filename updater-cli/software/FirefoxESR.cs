﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017  Dirk Stolle

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
using updater_cli.data;

namespace updater_cli.software
{
    public class FirefoxESR : ISoftware
    {
        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox ESR software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        public FirefoxESR(string langCode)
        {
            if (string.IsNullOrWhiteSpace(langCode))
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            if (!d64.ContainsKey(languageCode))
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// gets a dictionary with the known checksums for the installers (key: language, value: checksum)
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/45.7.0esr/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "3809f161951abd20d94dc1b8a885c86b98b662bcc2bf3ff6d961dc1edfaa32b2bdeffa2c0fbb2dad385d445a7e3ec8afaff4d3d9b0d9680b68a87c031e41ff33");
            result.Add("af", "868baa996c69e97e9cc75d50189137eec40e7b37c5f38353261260e8154d12491e89f37c641cd9b1621e27a090d26062eca7b86b3e1b3f5032a16c36373c5428");
            result.Add("an", "9cd889ac07d6837468856d11d914c900cb7a8a90d62a549e9a00f6c048f7b0eb757f9c7b0dc8170419794912ec5f56e1c7be45878aca7bf3a781dc5d56832838");
            result.Add("ar", "1aae98a9ba54c6fc62e33422923491296ce008e5ea0fdbddd753bc42d25e6f8728d6ef36d92972ea0bdafecf4e8feaed7f825447af07fd10ed31a071e1a3255b");
            result.Add("as", "e8216a165c22de9bf7a62635d78907e92b7b7f39482218e9860e279a8f8cdcf18622681a34b0d18772b6b97f555bf6baf8b7a129420bc930e3549c12ad30cafc");
            result.Add("ast", "8742a974b06a6ea3f810855b130f1a6e1d52dfacf7b3f91801f39e77414a7a45674261e2f5ff699eff96ec991b89a4faf7714c345e76c440f110db6eee9ed28e");
            result.Add("az", "5041eb3d8b9455d6753ce048206b5eb0913c7d0db20e0a8509eb7c3341b5622a3f96d0d68ef659c48d1e0c2e1797e4794264e4c9ed2c46dc3b6fb12f823cbf5e");
            result.Add("be", "a0881050836142f5a8308c565051ad6160df707a17639183bd7cc23bb7692147a34b9629a0f35364e3808b018e6fef958b85e3829472481a8e48f5cc2f5fe1fe");
            result.Add("bg", "b7b7800025461923dd59798efe6483f2c1a72a6936abac1c4fc7476f7607a8e6e3b640b4dea57d63b2bde0aabba02c7d38af9a9a10c3a31b059099c8972f8ccb");
            result.Add("bn-BD", "02cdae3a8b33964f75b9ea8ff6c244f51a36a95e2674a99bd11dac1a1a889a0c40e7334d64fdad757530ee5154ad6dc55be811813612b357bf0b752c7e47c2da");
            result.Add("bn-IN", "493d12fe73cff1ece70e3c8c4714c661798ec0eea18b6b7c2b74584a34f035b5ab74ae6f35d2f41dadd907f6c2aad1d49a604ed7cf58fbc9ba7f33d591f87372");
            result.Add("br", "13ea673964e19a04a2c3488ffc89098b4001d3c198daf41936a65c1f52cd3a98493ae56c58f7445462a3d4dcc0c86d095d89e665935ac34797d0fa6d3071b95e");
            result.Add("bs", "1a7b11c1d4b65f8f3690f39b93ce501df133a129577fe048d0f14e4c4ffa0ff008233f12b1a6a0e15181cd9dc6b9a2e4743b600276fd7971662a0e85a28ad4ed");
            result.Add("ca", "cb7a21e84ec5c703b2e9b99ee07f02090e0f2024a2dfba5489ba46c7a1c00baae08ebc8b90b24c83b0b0ebb09b827df0cbecc23dfefc0d7a9ad9a399a4ecfccc");
            result.Add("cs", "e84c2cc361eb32ec79e229f96836a995d86fcd66909628955385223b09a45f043fb577838afc3893ec62e212fe5a81e417c96de0beeee8f39762a200affefca6");
            result.Add("cy", "faf9a1a55e87670e536dae2e8e3f5758a33975f25cc3a2adfe7d7a87b614eab3b7a0935b0feeb95e938333826b3067a1cf91eae0df1e75fbb074c3322940a3c9");
            result.Add("da", "413bdb0c00d337a051c5ad3d3e7b47f2f535e8fa25112742a261d2a0985b07b4e3158dafc1db91c1ab2089d98262dec6fc13ccd9b99b4d6c450a12f7cc6a1f87");
            result.Add("de", "711836134daf798fc9c3ef178a999887be08107c47ea75a00afc5c7c045e9200fb7925be1074a1a04510606deedbb2743f256b8b7e52a67a39c85ab7c2eab9bc");
            result.Add("dsb", "1cd8241515cedc030e0259834e42408bbb3b7d232949207670fbe135c24acd2ac06ef9bb4041137962e59197213389a25780550ba297c2a8bcd619a85e231b8e");
            result.Add("el", "832b82ac5c62f11f4fd5d1a138f0aebc41a800eeb92f4f420809c2d221e447dee7a3f26a133abb6dd78cb90ef95175f3c1beee03e86320e738337a90b892ccfd");
            result.Add("en-GB", "a59849ffd31eceace18bd78562006c6aac5314228d1c0fc37e3da6af1c4e528f16f9d2a3eac48f4fd960574ea64aaaad6bd8bb1cef3aec1741a9c402c2bc6761");
            result.Add("en-US", "555664433eaecae572f02143271324fa73b81684117f44b4cf3f9344b89bf01b9434b0317bab3aa4d8b58ab4e77d8d760e668b9d86bbd3c3587cda6e3793e504");
            result.Add("en-ZA", "ab715af929235b7825fe0ca792d08baf0df2ee8ce96acdb8c87c5fa94cfc5de1b7e9db43f13701d56fa04c5821c802ea324f8728deb5eb94f163012507fe629e");
            result.Add("eo", "33c367a6303a4ca048543a271733aafd192bc5c6ef6da1973f01b12b8ef675f9555ccd2ea1ed27c0e307a9dc66a03fe5fa0dcf0fad3e3be0bd928ed72d8bd887");
            result.Add("es-AR", "3b766157e3c7b55b5985cecbe48ef04bd268dc7767df41ed9e22a7bf7f6d071c454976ad2415ffcc80f01a1d440d3e9c6d205e6f9cb26350f010fb47b2a17225");
            result.Add("es-CL", "2d3a31e9c01a9a2fb95d310573ef6c5859c375c8b8d28686a60ad7b250bf74f1209fd9c9345d459f47489e223b43cadede02413f73ec8313003f3a61d1c03dfc");
            result.Add("es-ES", "7b8c090e894ff931e1fbad1f8bc16b37170d66f4a1720da145739e48ae31cc01eebc9a717dedf08c365443f7055e1b8cc6a590a1b79470a9db558d4f918ab4fb");
            result.Add("es-MX", "6af2f27c4bd02c486f0cc88fae129126c27f559ac77506d5f15f33bf831f805a059a7ced426c2d8d66e6024a9b16412a47ec7c51ae64212cd45dc07084679c22");
            result.Add("et", "a3bd4a301d74599c02bde6c7e67ad2376b655e5bc6c266736d864ffb4954c62f6f1c254be029d7330fb8ae69d5f65860b4f42e158bd5b8d5f6d1f861ff1a8b02");
            result.Add("eu", "e97e617d52ef1a8bab3dc90fee70d35cd90905869580565ee8ef3e5ba410b534e0e3bb484fdb4bf4ffa21fd6e2b41b2759268375e737b39050b2128b7c551301");
            result.Add("fa", "9a37da6ad19266eb3eab6d462ffea704ef541f80b15ad71133fe3f0c35c1d318a6e7f4e8872c9acf9d8892c13dc10da3a375c7f4296c6eb673992e7d9b74b0b0");
            result.Add("ff", "37706ce9e604f900c24f6acf62a3f2353906e56c5f76bf82512cb3216d0d4b634d38d96eb54255c7e8d3fd88ccba899df258a674132b98cf665e03dbefad515f");
            result.Add("fi", "21d24c498b7afecd1146b4522c0b29c166c6eaaeacb1b7c4d8fa1dee2a8fd80a4d251926d15d77a825ab33b2d4052ff49cf16be4692f91aef1c81f46506f561a");
            result.Add("fr", "e9d4705e8b16ff96998a064c3379ffb8e6dcd07f5723f51ca35cff12168121af23bb9c28bf97cb36aee480cda4bd999893705d4d734441d7770241a324e6e637");
            result.Add("fy-NL", "4bdf09d02f2b2ab33f71b71998f2b2b5d0af20c04d320b66a3b6d2c6a804f57a49fb6ea420e5eb1179dd1ec72901f535d91ebd954244315b9a5b48e9c192dee4");
            result.Add("ga-IE", "3b0868da75797275ff906bca16fe824edfa25d64e2c022d855eb3ab4b0fbc2896cd7a8b01a19d8501eaca053edaa903c620490780ffff7c50d4e381206108ba7");
            result.Add("gd", "8b437d5151b9d1ecec404dcfdb80acfc61925bb288f51e632f2118ca92f83abecab37fa0285e6442fa40c744a47a3ec84dfce2e5521d471caddd57be570dcd57");
            result.Add("gl", "7b2568f191912ce22d9a15957766e2ae811917d6fefa695b8aaeb5f11f20fc499977c2e1755e8161a7dec600e22a7c45cc8883b16427a3a5e5d8a7747cd6343d");
            result.Add("gn", "9d27dc514e5f2a69de7c7e8d27c224df1ddc7529adf4c3aa0a56b8e500ac5403d875886baff8b066e0c5c97ff312d2107b4871ef2694870e323366794022c886");
            result.Add("gu-IN", "d9f09655d2e79f798e48d2eaf2812749543c542c3bfece4ee1e0b49ed300207cbf73d59ad5fb99c4358356410ba213358e3cbbb4e444f8e1ff9a3b6022b31f84");
            result.Add("he", "4caf51314e3911e541ef0be99c05e6c08fc7897a62186143d618162145860f1f28e72fdbd8d78c532cbd5eef5d6243d87d3c00dd7d0b30749f75daad0c54fb51");
            result.Add("hi-IN", "f9e0f5f13f19aca510dbf16faa1cacec4cf94186864687587f372bc99301098142420df9103c3a1003beb92d2740853404c4d8f7745a7e9a4ca4bb6c1c630bd1");
            result.Add("hr", "1fef422429b108206ce11b8238a89d3b171292c090f86eab8d069c5e207c9df66ed20f7b20428d7cfa75562cdaebe1dca3b36bf35cc34ec942c1baf5f8f82796");
            result.Add("hsb", "a5350c7a958286a701508f2e407a324bcc6a62d2717c400f5b1612adfeb53dbda8e456b12f73c45f7da01dc44c596ec2dd09aa0524dc1e32196da6852ed079bd");
            result.Add("hu", "5575bd06bfd9a0594e1993e3e73d3d89be68fe34b459d87b90614fa4231e3a112406bc9941e37158a545fb8b26a3987d9d9a195b62e9837d29d533f26c709b36");
            result.Add("hy-AM", "157f69be82ed7a533bc3c77f307146f4b25ac6a3a452f28a71682e542c52cf67199cc8e340c2ab5a355e7f3ff1cf8a11446028a6772198715c5998022678d9cd");
            result.Add("id", "85fcbc62c37f30001636fd3aeb65a5084b9d7cba4d2a298063e02703decc4f028dc77c0b328b34c0bb23c149efdabf37795d7da580b1ffa590cfa49a758e0313");
            result.Add("is", "6b2c5ce2fbee80dab4275b4dde6dbb825a6dcfb62b13993059f4acde472fbf7562eef1f6644144f34dc5507801eb528f2b29eda87261319a6e5bdb4f495f3e08");
            result.Add("it", "5f1eff9d7b700890a2d8a927d7a1cfc67c37bf5c6c3b0c84afbd7e550b72132a7d17429d55a2b5957590edaeb5f28377839c0014422158cf0a9daba34d976944");
            result.Add("ja", "a93b8f66ae6487881c7570d47c9dcc799acdac6a4315b4edbcba1572956ecc2232f1530b8129f2d1c515ed46f9014db9ef506a5a06786aadbf414de20490af19");
            result.Add("kk", "9de523b3c5b5ab4e42d020608e316402914d85bd3b01df8bc05ff1bfa602f5f96130c682e8bad071477b307593579f02e7a53c98aa06f8eda5ced175d8119f24");
            result.Add("km", "3ec970ed871019311d7fa968e437b964a5920fe683d177d0749feee24564395ab51ce689bf402dd1f5bacc810ef3ccf218aa3dab35fee5da0b2bb6e6b07c6da8");
            result.Add("kn", "a0500aa71cad89a85e6bbd71bc801ca4d20a48269c84539b595d12969beb6cb3838254c10bc47ee8ba934223e110b63a06500def0225a74a1917d50099dc0590");
            result.Add("ko", "a86ce20b7b15834d4d0d2d085ae84b7ef8b92b2b4be141c092129bc5be6ed87000cc53df852b1d9857bc2ee0a3ea9da78f9f30ecf57b7150ab800b8e72f51168");
            result.Add("lij", "c7afa2ac43b2f54bf289305541e2d6bc8855677fc7a555576837e2d64e4029ed88aea68b8c9ff5d92cd25e6019b473996fe4791e2bacc1091b9793607cb83fbb");
            result.Add("lt", "e57a054464385f6d222eed3cac4b73f767bb4485be1c8522d9bcea46177730da961b45978b6e32c0fc41d2aef3f47a287e8a3bbebc17687fa5e54a3d9260f506");
            result.Add("lv", "4f2d6ec5d15d3aec50f241030b62ebb888fa143d635978558384afc376e918ca4517be85ec9d984340ee94da5e05b39e7b8d9d0bee7fb6a5d1798a407e46657a");
            result.Add("mai", "0ae14e7cf2b1f9adde89ade16dbf942ccdb03e3a5db99e686f24d5db9e848a055abcbfbd4136f7fba9d61deba16b1c2ef40aa13e4b0d423f235a25c33b1fa7af");
            result.Add("mk", "d20b8193ceb3863bb7740e19ef6d9ab0c6dfe63159222e698d017f8120ad66e12b9b754f3d31583cb838e9d6b43a2212b5b42498d1b1d52b669d4a593064ee43");
            result.Add("ml", "e6dbe18b6824daf3bc5e57ab7ec18e22bfb122411dbbd3edeee05202df7fa1c74df4ab9a5a3e70dca346a7c35930ceeac7e30ae0f7c036089f41f3375ab55d73");
            result.Add("mr", "d803e6eafa080fb3242ca886ecffae31db96292145e59944d5753e6a80b06f70e0e3bfceee860175c35bb2fe58e1c3291f132e24a2122086ead536d89532ff43");
            result.Add("ms", "65415482e679dff7712e8a9fc2a429655385585acdf36e4206e599814dc175d98462ece47643e2f85bf642436e771b0463914c39457d80ed2cdfe611660c25be");
            result.Add("nb-NO", "f359579f39d2468f5145426e1ebc0419dd977bc203eb35ce8fdf8b189cd733cd1d0a361405942424c61778f12266a33acd04d3b5d324741886823e5429171bba");
            result.Add("nl", "a0f38c66e7c9663d7b1f4a7f44a0c17efbbd6b364f5f0e0da410a60df7a39816497e386a90918413dba4a01ae319ad75bef40a9706b80f4d402801855bbc0306");
            result.Add("nn-NO", "dd91775fb2358b58c5f7a7eb6420faaa89444bd3408e49931522c52280d60d934bb1818fb32e2ffd0b17491c6e4b0be626d150232f63e19065f4c49d59744f78");
            result.Add("or", "b891ee4e4505ffd4fb101b6dcf750849706dca602865bfb71130d10564fe7bc0913ec7c2756741333e4c886d96e3ee8b4227afa16d35dd8f4276eb0cea45b93e");
            result.Add("pa-IN", "293775012c69691b445e619140e6dfa3261108536653b98b92a00fedc926b1b66ec98f394a62521f389b497856b4af739dbc511e923d9d4d30cf97dc55c5fd6e");
            result.Add("pl", "ff3a6e52f9a2d7cb913494d201a435545b16fc2817f1ca2042972e2bdfa8f21bfbf2bf15bfd4ec36ca7dc416664bcb25761eb4dc01aba886c856802197182468");
            result.Add("pt-BR", "4767d8501ad0ff9f0ebde1c4118a62d52f54281c166df3122553dd947e2daef22f6b3fd6ed4bf1cc5c051657f206988a5473dcfa88c026cf4018ba2e43d3d4a8");
            result.Add("pt-PT", "96c4deb68e557472b8bf48621ba717f8a309694bf65c17589a6a9b946295f17d81e4859d184ac4c9076a4185bde66ace8aec133e0f4b9c636e1ad0ba9394c7f1");
            result.Add("rm", "8bbedb4ba9f1e6b7bdce2c06800f914b8d5c4a013f7547114ff89fd0bad4496cb54c74f38d919aecbbad492df5f4621999d79413dfc0bf8394cab3c307de6351");
            result.Add("ro", "7d9aa8831d65b64eca6692996818affefeedcf354856ce08a450ecf12a7c9267256fc3efdd7b1b1afbfcbfa29039fbcbb7b28f070baf20427f2d9a9dd50d6a58");
            result.Add("ru", "2e865c80cd6e6305a42199644eeb9bdb4f884d2aef6410d383470cc1d6ad263fc27bca5a901bac1fed31bf19d016446a5aca72c60b6d52ab7e361acb7c0df823");
            result.Add("si", "2ea97e2d8c73a640395ddc7b08fa491c377c8c5bc4185828ff132ca7f822baee0921759e7260fe84dbcc371487fe4e8c786f2773349a0e3694435a2cd3d4d0f4");
            result.Add("sk", "b614a94b9a3fbbe8cbe04c5ec4463a11ce5887542550caf4f74200bdd6b52289431472a119308d2a078d6d39eb55f1576112571ac887e00e019a0d4dac063ff5");
            result.Add("sl", "c86f5b39bdc2d01f8f34e86c35400377e82eb3812976ea135775593b66a52d5c42fc2b6aad3aac37e2493a9b3773dcc73be7967f9214a8faf683d22914bf762f");
            result.Add("son", "3e825cb605eae822dc2a1df0f3a66358cdcfce34a42c79bbfd5f5e408770e8ac651af9b0d023efffa583ce6f796daa56e15e8d0096d4a7e887c7cae41e75e72b");
            result.Add("sq", "b324e474b4fc4c8a1bab6a01b1bf03705fea981c1b0e0cfd88a7677fbb9f3d1b1f37d3a58014282e644a3ea36109ae5697738eafb0166df30b75bf510e57a9e3");
            result.Add("sr", "bcc9105527df4f400a198cf39a93cd488d54f9d913be0f59a81c6a2942d4e2ec757ae52333116035d27315d0e636da213463e7951ec5fbd5df15f7804ea4ed12");
            result.Add("sv-SE", "338d527523559ebeb13d69eac275d2399d241211b3a882867ee5695ebad9531f63ae3f2ab4e203c307f5e63a12445a511fff30e998ea64816bfbfdb46f023c76");
            result.Add("ta", "181c46ce219719e7e291adef5232b8e386811fdd5f41270d1857a70b13a2037a816d690fe703db3e6940f48b699f7f30653cc1d28c82139d26fb464cf9894aab");
            result.Add("te", "2db710655fe3ace54ee13686e48b7a01c1cef92dea2ecb578a82186623a7f470c67babff0672a28fba5f871f18b864677f4d39d28226db3eabc02b929d46ff0c");
            result.Add("th", "b62ff3ab1e616a525948f2455304abf2a44ba8a26ecba49edaa753cded0b7eaae447d29f74bf71dd265277a859c5a5419dc6c624d964f4769f1f3094dae0f3e0");
            result.Add("tr", "f708b423e59a55b64609f57e9b9742b4c8725538dd715d1e9548a1e76dda2e1f94e31f5edf3616a918d6e1c248cfdd901a94a278ef5bdc3442e1ad91e380f119");
            result.Add("uk", "89ee04f2225f7222d2b70dbf578a1616f178b52cb26d7091e8e35df54dd24e29d7ff0bb89ff0c829d50b5f85ddd2114c44f311a91de74be900557859c844c590");
            result.Add("uz", "6362e549181eea28e47d0a3b8953677be75e8c1585b0bebc6c29f91f6aea94d7181a34625da28aad7a743235d09af9082666670ef6bd2fed74aa6adf904407c4");
            result.Add("vi", "2ef93c3a0cf56ab347ca4baa81c9a05883644ccf19795f1a7144073c59d9b90801f5fd246e1a2ae100dcc66f810bc525a36b8b7b786ab24a334e0cbd2dbda18e");
            result.Add("xh", "b8045a0da1ed35ae71035488d7db08b7b471c9bba3e7cde1b8bcf68eb65208aa0730d31f3fd95a4f8e8821eb675e4d953705a7f4836e7e15eb80d3676f140b08");

            return result;
        }


        /// <summary>
        /// gets a dictionary with the known checksums for the installers (key: language, value: checksum)
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/45.7.0esr/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "63c9f3402e712e934a24f4c0bbef9e3e7fa102feeb4f9370e0eed6f8a74edb80d8662fecd806276d32053a0b7b00b55656c4cb518c6c468847424293245f8c3e");
            result.Add("af", "0d04a0776a34d71cfd233fffb541823754217dae44a4d7e33b81c27e654ad196e6f422b17defb6b760ff73f736e3ea4d33d4737d232588675e67fae8924f240b");
            result.Add("an", "be6b4e47ac51b82151bb4ea2356fe9449100f104c3711e0c4a560b2386c77bcf9f0c59fad433ac47aed1d6978b32ed1e1a114dff42ed2f213f66730becc457a3");
            result.Add("ar", "cf5926372670304e19d63c484b9b12869ee36e37f7793e8aafb9357c2b7a5a86462bb3c2ce649e15d9bb5c8bb02f5c7bd8eaacd068df863199be00039a47c6f1");
            result.Add("as", "073ca1a9098ae877d4dab6a82cbab9ce3d6cba8d13f17d880f421bef4e490d91c5ed9451d3881bfc16ce751c6764412c18e8d7be7e6d327953e48f0eba04b17c");
            result.Add("ast", "0f32ad660b4a3de46b16300e2b5aee1de2d60480276419adb15c9b3cc5f0fe4c97a549e8c0092834db443a09c663024b6105fe87e69ca4e6f82ec2c1d5949b81");
            result.Add("az", "2d959b109c153fd5fe95e4fec1ceb96bbd2ffa8af8de178d3b4ac45b875c987eb257259006f67d0f08e4baadba0fa2d44722d51f14442e36d0d05e4722e51dee");
            result.Add("be", "3dd3af9729100f30664bc4ebdea9c2827aa9544c2cb7c92846f3536084df949d01c606c62886fddd8f51a76d99d7d76db068065aa1bcbfba5d23587af2233671");
            result.Add("bg", "5497f53916906879b7034acdcf813d48520be096212cd2596b043cb9f7cf162def88ebb65a9f4c81941c34a40449513edda8c0457eea740d431eb8eb743ea74b");
            result.Add("bn-BD", "de5d398e6d9b5fde9ec642c936306150bb34d4304e840c5f3bc0b18f58b95201c8ae7fa078ae22e41ddf58594e08ddeda5406fc3d347aa7481045ec488fcca32");
            result.Add("bn-IN", "17bc907a2c314d586fd504ed862eb90fb4dd1b2f69e3514ab5cb495bdbee2734ed1bfc58dcad15f08c961198e8ceb7891828eaa5caf9e0035cf0f24354211e34");
            result.Add("br", "6430a8ac91a9f68a798827e532e6c528a2eb76655c154010cc9c744a284ecb1c29960bdb3a1d56a53e749a4a6b56ce54fab9644ece98ed9000a06075536028df");
            result.Add("bs", "0dd1894132fdd6fc76fc429479562ae7a010894b7e23a0356ad791b8ebda43c8a470544fd1e335f32a2d4f7185e97544ac013f1d6253c514c512c7dadbae7784");
            result.Add("ca", "786e00d2df5a83c6f8e8c7fc2e7263da4cf68b976654d5df1e013bff82e3286f7d431b83ff68f27e4e9ca39ddd28bace4658d53dd49405c585766c4cbd73b2db");
            result.Add("cs", "7718c6b8ad12566c05b7dae0dd2850317c071f6d5cbdfd95db807d10c389035b015c1b5498df54ae3bf2752f5269adc0301fdb7d6b098abe9a8c07c456bb0ee1");
            result.Add("cy", "f6d54f3212eb1d733d3f48415e9a882861768d2690394cb5078d52f1dd1ace4e2ddb503274a788141576ee5ab915b367fdca9b611e9aa407240a22eefa7b7fcb");
            result.Add("da", "e8935d008b1c5606182a0d15a7e7c325a9471d10e391dade956cbe6f74d2c9edd360ced7f6928fb472935305e61d1eaf8e91d56fcf0289de72833ed357e3e94a");
            result.Add("de", "d3c3f4a1b51e6d730f0be0f8af07cf4b90afd6355629098844e227ebcbaee3c26dcae7d6c3598e1e0b157dc200be8c2950551c7ca191abe027550c72e3f905f7");
            result.Add("dsb", "177fd314e91b11864dfc9878e6e1ab7d9412f77f03399bcf074b1b8eb821d93e76f8f5c8475be0b1209fdbb4a985d0022c96a3316a0e6b26fa8db17c37404474");
            result.Add("el", "7c22efff4e6d423470062eede553d215127e65e546dad7c13d4fe083375e883b86f99ec7754de92f02aaf2aa701ddc904a9cc8002e36e2b7cf9d7134fb7b9f06");
            result.Add("en-GB", "2a64c8421dea149eae0673d39bea367534520ded270a296bd55607d04fbefecb53b825dbcfc824a53639f1bbff9cb3d2b6475b44f0773b711a6011227dfa2989");
            result.Add("en-US", "da0516df772f6f1ea5030803c218e679694e1aee2891f120a3b5837535645bf07867aaab78522dea47694ebb5b9bffe5d8aebdc9deec29da89344db9daa33237");
            result.Add("en-ZA", "77d6eded8284d545af60e60fad60e5ddb8e3c616312707952c1f2839edbec15776c34c405f360ae301e83af75c72aeed775ea7cb2f95d06a12fdf2e8f311780b");
            result.Add("eo", "1edaf600b4232458e07c6694327d8ee79bd07e620686a3a69c4cd70d395c6108966e16db3ef656bb299c814d0c65775b33c40cad1e9030986b3599786805e485");
            result.Add("es-AR", "6c6c8fae19b4e50bad2e2279b1fdd0ca1559a5de1a5d3ba718f24c0f1f574b175ea3245e0c1eb0c01f72b591de9331fbe3871ed169d20ed9f769455097611e82");
            result.Add("es-CL", "daa42560357decf2ae03ba4a3f0605e053dc6a4bd1cd5eaa7fc5120bfacf6514f1ea59a39d24ca9a42a0eef56d70ee2ae67a0f44815cc7acb955aa1319380a2f");
            result.Add("es-ES", "a9ee5176b95f2b0c5b78103a47580cd019f5f5ef8f0c1826a3494430c82580a1abb59940b9b1ff6c0b41935727620724ccf44a8aefc79b94df08152d9dc12adc");
            result.Add("es-MX", "a4af718b3e8e62dab0e7507183ab90273a282f6de7364143f7b0a0c40469a7bf51208bf8950f5639d9bc42cf0572591c7a2e6c23a6dc393ba3cea0a266ebf408");
            result.Add("et", "86f0d0c7b9458e6517039e4e11bb7d4a370166194b019b617bea62999ca80da7e4eb1a4f3e7f0f29815f6ebe5a86af1f922af59296406d6230fa45e23a8757e3");
            result.Add("eu", "57323d982991fd87d00a6db55d220c799c55eb3ee7af1325b415e5beeb383154e869e5518e9264868c7e97858773ebee368e66518a14a8f2874cee2c952f2205");
            result.Add("fa", "ca5d4c57b85c7c230aa4da6167d6b1db374a70800a3ecb06ad12ccc9564278af52cf46e7a2eb74f868c8becd1642357cd90dd47556cc3ee3f9ccfa22974af7fe");
            result.Add("ff", "ef8babcc5b82f5123ea384c766239985b6988dd015eb9e44a08af16a236c746a1b4e02d9712f01669c1580482ad97b8067db4d05aa8925fb4960278bb8564e0a");
            result.Add("fi", "fa4fbad5c46da1e30cd39fa12eb8eda0f40d868d8bf6a0f03b1134010430075f5efd133ddb3344a551a605a82dd64038a119d8be5368383d5c385a663161f0e0");
            result.Add("fr", "65ab1c227ad77609000574f84ee34dbebb2b37c587914a906857ecc6c4c342e8a82f76c41f00ac1304b8095cc46cf3a2678191016ff25d8bab7d464ece71e0ad");
            result.Add("fy-NL", "5a2328330a09091a4b1ce4cc7c645de4eae1bc9d2b7df4c586cbcee0c61379b235537c89bb5cc8187e01ae8283731349f2a6fcf4926e6ae0dd702a1415212042");
            result.Add("ga-IE", "8445dd996d78d0c236deeaa0caadd03135fd2d234559cacd1ece591522224e10eb5fe804897ddf5f8a6e727236ef9be7d80caa87d433842de79cf1442adbf93c");
            result.Add("gd", "72b2c057fe5b8eaba846e535f7bb15c474d70de8b0fea8dbaf9f0bddd9cbdd5694055d43d27cab22f20f94eb4662b03c4ef7bd157e02e3e474e9d80d9f077d2e");
            result.Add("gl", "18a2cda28ef97d0bd5f367812c8c169f61871b6613457b5b94c244e46b5cf51efe543759718cfa8160f86058d4b566618068df69e18b07d7c63773861f15b4b2");
            result.Add("gn", "8e3d2a1e7ae62b9da292255bb1de66f646e759ff7cc3d2289ad15aac667770fe56f182e4434dc9b7a4e8dedd4ee369309e06998a33d1ace6a42e43eff5950c70");
            result.Add("gu-IN", "3ffb7cef417e56b22e681f481602905d88c26b820052323600dd58b83eaa0817cde27c1d70c8e02c8c6ed4a83abfb534af599c54997310c3901697d1d142409e");
            result.Add("he", "a777b3449cb19c92aed048a3bc342f18bf1ea96e8945ca98ba86dc0b9614b3ece896659c0f3bba40c08dd6ab9e83c9c41e4de8ebbba011cd2195048a28b6d1a0");
            result.Add("hi-IN", "447b594b21436536d1c7e112a7477be817ffae75ffb16b8c624479c897b3e31c789e16733456dc3a379c7935592d70300e6e20bc9ec2c36587f8fdee577ab7c3");
            result.Add("hr", "298d6618aebff3668efa1366b043574493cb7d6f0197c96f6eae7fb6e06bdc875bca651de136e695a398d2cf1daa58301a1565220b247b5d2000561021848b23");
            result.Add("hsb", "420f1b5c03232e017ca5d636c40da3331f45a9bc8df5f1e6504f0055d79326c637892c5c6ccb2fe2c216649332c132bef48026689b319fb2e7c086832b3fb3e9");
            result.Add("hu", "e3f915d8571eb58f44768d54e25389202816157242be3c07bd3a5815f1cccf91e77df8155b3cb31cc35ab936f7026623f7a597267d3250504c951facf9f94a61");
            result.Add("hy-AM", "dd89d32803d0df7e11971f623e3b6a9d12aebc1040a03ee415ad60e601718810715ee2db704afb3f029c97b4fb916806c12124a83a5793073ceab8cd99295885");
            result.Add("id", "790fbfeb303817ab72c90b37fd811102d84e3db158f62077617d730c73606a4c5e382dc252a1dde5131e0fa9d6ab9f0d61ebb121bf13364bfbb6983e527b6d7e");
            result.Add("is", "2fcb13d38ebccd55f4cced77d86213cf54c22c702f15128762f508e81bd595316731880570f306ef358aa9d807f970b876dbc29b780051f8b84d40d005fd7617");
            result.Add("it", "964cf467c5ced25f7a2fcc3bb2b2b43ac9677c538bf84f160123ca95e8450e087ff222395e07ea6918119e4c9c23a766d66679e7e67485565cade0788d04047a");
            result.Add("ja", "cb268e657c5b737b89979932d31f8dca219034004c44fa63c1d10c0902b5fa896853b1a989196aea08977dadcd14ad2c5c339e08139d2cc7de4182ead2eac479");
            result.Add("kk", "116aa74fd4b64a19a85478397ee77375472127a76afe49ca0791042f421d9e4febb0a048d40e8cba96c5e1417622f670ef17a3a382e2aa2e64d8911461c6bb55");
            result.Add("km", "4a7b005fda37f1f07d43647a3793ffddcf7afcda2a5076027707568c691a642066b44e12b31b6acd36b5a35fb089a4aee85e6a2b1e0a89f19ead4399fc5539d8");
            result.Add("kn", "61ee13df4cc6f80cf6456ea1fb12b30a2a47019562ef64d57dde3c7a6b872da16f05eb952686324df56702070a0c8f8775f1e2786a496aec9102388767cfc196");
            result.Add("ko", "ba3ba5df5da31b1cea4e3fa06316acbfe061369a7c1bd7552fc6a8e65b7c1d133ef36404a8e675fffa042df85a7366e79ed7e82bdcd9d1d326a299586ffeb04f");
            result.Add("lij", "d7b858501a4603ce4dba876882bc3f4b32c955cca2e004c76a38806541dbb97c504aa89d775389b34140a0e1a7e159cde59f35ee9c2d8d3d1740ddd1554eee19");
            result.Add("lt", "9f1e1304369e52aa2a9a97c7683e765ead7de8de35599a836c744fc90ef32580e9d99ed1b13d75a7627ea47b5e08ae9c75a2de1611be8ed37c852c28d58ed39f");
            result.Add("lv", "f9884da979a8b672c6d1a902e4615c01866ca90820ff247302e6c3e51d88dbe8c0fbaaaa38facd3e1bbfd9060388df5fb1af953dc981cd5228d5a975e0ccfe06");
            result.Add("mai", "37185b56b16b2a5d83972a50d4a50b8ab1e61ec11f880da46d5fd9977c1e96e73f38dd0a5076962fc5e6a1bcc5a9a22531a2ad0d53514b011e01da7de347428d");
            result.Add("mk", "a88a5712579dcc5547338bc14173de140d37031e56ad6147d3c31561b1353b32c4e1a0a0e8deb237c8e32e3251cfd27c14ad23011bf87f4ede2b8b476937b63e");
            result.Add("ml", "b8232a13a8e1ec6fcf46ef21437369e392f6e6fd7d499a9bd6706a848936c974a080c37a916656365f6bee99766cc58bb8e613ccbaf5acd2c83edd8656e1c878");
            result.Add("mr", "ceaa89b32cc4fb759816560f4ed2614a470daf62e48928dc7cf0681afcc4cfa4005720c0c5ccf13dab4f02e56651a7e97f7b22c6ec4a44ed36e88b01aeddfa47");
            result.Add("ms", "9bf7f42022615a9acdad3bc127b694bd1d7cc6bab0bf75aa7ce91045981850e9f35e92f1b55852a5f53d9398674276fa54ca84278ed6d65a24f7b480bc805d3f");
            result.Add("nb-NO", "013a479c7c18f6f7f5a350f8ef81d1d231b88555f54fe41fd060011214cfb8c4526f38ab79a89ab191bd03e0a134521c3d1eee2fc4f2dacbb8c46e2939f05f55");
            result.Add("nl", "a999ccd9138ac13f9c094e6cdf9958ae511098aed8c5a54246124ebb5263f78729f6c07d5c8b98db9a3adea3de14b3309802b25a82fe220b4c646b924fc4f3f0");
            result.Add("nn-NO", "16bb3f171ed10cacdc490636668109ed86f7d8f8a3883c5d1cdb14fcec5859312f127810dff78f218f44eef50f909dfd6e4d26d4dc3eda980897bf217c53459d");
            result.Add("or", "e89116aef78e5662db07a62c62d770ef2e9a82be19f3c7b25e2b5e909930e519fa70185e59b00e6a386e7b22562ee7f3ff6962067bb9a050c62b054bf5fcab3b");
            result.Add("pa-IN", "337894e7ee7873e557b99c0df26003f9e9ea4e98a65e7c6c31270eaedb0bb703b910ad35d0934620ae35cb26615297638b9883d0f178edc543622a4c58d846f4");
            result.Add("pl", "3845ecb23d88b7b97686b5cc34a95c0f3e84ddf87ee204e303fe3d22094a947988c2a8395c73bda1d19a59eacad37676567af799cb792afe13c31c4ca3d3e9b4");
            result.Add("pt-BR", "d35336cbfa46f6fba74d106a9d4d88dae501c01709cf30aefebf958fb0b3e7293067c4e7e6807c3f15cae664b0a5c312b85a327ece4a6cfd94690f6a386b2eac");
            result.Add("pt-PT", "27f3b49611cf014a6b2cf313c4343039477a1688f412e126c6ee4495cdbfb7210c89d2ffd1b77cd18c549c015e33cc9c8ede92e7fa55b222d40466142be0cb3d");
            result.Add("rm", "cc6e9a8994e952e8e59fee54229eff49b7d10eb8373f9d92c1fa937ee9ed9e1a61199e0bef04c0b982e6317a534746f9e6c5769a7f6579dd79f434c87ee5b03f");
            result.Add("ro", "a4e2c14fbbdca8388462488a1102c95bcdd31da55db1137f0572f381887eb8a29e1212444a0f4e4c722f100f32d219589f175ac7c14660b5326bcb88540729c7");
            result.Add("ru", "1e937d4daa9313f57d8851c693c8de12ae88041d64ce55fd21dc8e709d0926aae981f65e1fb99fca1289a5bb3f039afb2029e53afd9e7840fb1e38664fe1e072");
            result.Add("si", "c9f37178f82a1b5e69bf099ee8ba68d6a41632d3121b09f52d03ca4421252a2a702974499bd605adb1312eba996a49681b5e7cba3eab6986714169f7ef39f32c");
            result.Add("sk", "35ebe2f76d492621a0a89986b405a3fc0af3bc29eac4b85bf1c0e80a9d94444204c7c65e036c429c2e3fd81e1f8f2495c1b4c20e0f7ff2a0bb1c34e28133537c");
            result.Add("sl", "ad82f46038eb1b498a2edec5d3b3dcb76d407fcd605a121d0c9af7941e215f6453a377d0da5285c9af74c617fbf671b6a1e7d27ca3b8f508120511723f1cd2b9");
            result.Add("son", "a1906af2a56381f786edb68abdbbecebdce5e39233768403738a4eb43fec18e1d3fbe01708de290229d4af9c1d0214cbbb96e4a5089441428a8124c747890cf8");
            result.Add("sq", "46126ab08942f8080458621cb59b032cbe98d04f47f70fa64eb07372abe32edaf833b9fdf36078891b08f288d4eeb0d65a8b2f15a0d07ad8bdd85eed8d210213");
            result.Add("sr", "eddc3f9c00af35a912ad8d91b14975e4e5e065cbcef60954c19efba0af074a58328da684a2e411363590d8e13bc6f3cead69ecf23c661ada34d6a33ef8e7e86c");
            result.Add("sv-SE", "0cc21abf9768f0d76165e80c11cb9253a2a897d0a7974640e220db12b933c993f78a7a30ffb6b8c37b096dbb9e1f9cda6a2a4f754474fb93972bd2aac7b17ca1");
            result.Add("ta", "ec2e011ceb6a146e5295c75e5e36b050a80d3cfd2df4d51702ed5854f613d237521ff3e37807b2e6bd34440b72dc038ff98aa6ae17e78f63587114164c322a9c");
            result.Add("te", "b520ca2011a5bd76eeb8fd72e35025bd8656d6187492e11c8c3c55fc150a6544d3472ac3e7d81871eda7e75ac24b4ce1cab9eab1a5773895c39f61ca0e9643b2");
            result.Add("th", "26efff7da4aa34f4b4ad8ff492822613ed34b63ec9ade5f723194113a0a6d3b5ef96125808b7651f6d3ff7f0c13a36bbd5a50e02b73df0e0b28a82c88048011a");
            result.Add("tr", "8157f22316f5947b6d8bc4f595c92ab0f06d08f18a7f93e8a701fe970ff7eb7fd13e30d978a13af3e1c2f7c968f5f57417dad089073f3cfe9e17b4a5f69d0e2d");
            result.Add("uk", "836023590a7e09e18b5935bf8a86ed50cc94a03bf153ed3a697c18a083a324d1ed130f7de71b06ab4144b089a289ef83f792abae9983147360b8af21b7b0ef44");
            result.Add("uz", "bb777aee8e058a5e642344fa48579bbe8655a21530da2233a27c818a6b96bd0529b85fdfa05a4523a4401923fe603d0b8148a7aa499b1a413f5e2cd6afb9126f");
            result.Add("vi", "614b29d77ae6b15e4ed9e1292d563a5026d3d0cf07984f19d1ecc74909934891a4e773632fdaafc5da8336dfa9ed78bde07312d03b65b0003500bd8ffb982357");
            result.Add("xh", "86a3c348e7e116295bea541ac98171caf24455d7a4541537fba7af017e2a3f57095940d25666ec86a8f7feab337873be7cf34c3e7818e48f033a3bfea521b5a4");

            return result;
        }


        /// <summary>
        /// gets an enumerable collection of valid language codes
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// gets the currently known information about the software
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public AvailableSoftware info()
        {
            return new AvailableSoftware("Mozilla Firefox ESR (" + languageCode + ")",
                "45.7.0",
                "^Mozilla Firefox [0-9]{2}\\.[0-9]\\.[0-9] ESR \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox [0-9]{2}\\.[0-9]\\.[0-9] ESR \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                //32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/45.7.0esr/win32/" + languageCode + "/Firefox%20Setup%2045.7.0esr.exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    "-ms -ma",
                    "C:\\Program Files\\Mozilla Firefox",
                    "C:\\Program Files (x86)\\Mozilla Firefox"),
                //64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/45.7.0esr/win64/" + languageCode + "/Firefox%20Setup%2045.7.0esr.exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    "-ms -ma",
                    "C:\\Program Files\\Mozilla Firefox",
                    "C:\\Program Files (x86)\\Mozilla Firefox")
                    );
        }


        /// <summary>
        /// tries to find the newest version number of Firefox ESR
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        private string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-esr-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]{2}\\.[0-9]\\.[0-9]");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while looking for newer Firefox ESR version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// tries to get the checksums of the newer version
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit an 64 bit (in that order), if successfull.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/firefox/releases/45.7.0esr/SHA512SUMS
             * Common lines look like
             * "a59849ff...6761  win32/en-GB/Firefox Setup 45.7.0esr.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "esr/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception occurred while checking for newer version of Firefox ESR: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } //using
            //look for line with the correct language code and version for 32 bit
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\esr.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            //look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\esr.exe");
            Match matchChecksum64Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value.Substring(0, 128), matchChecksum64Bit.Value.Substring(0, 128) };
        }


        /// <summary>
        /// whether or not the method searchForNewer() is implemented
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// looks for newer versions of the software than the currently known version
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public AvailableSoftware searchForNewer()
        {
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            //If versions match, we can return the current information.
            var currentInfo = info();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // fallback to known information
                return null;
            //replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// language code for the Firefox ESR version
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
    } //class
} //namespace
