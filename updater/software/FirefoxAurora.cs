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
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "105.0b3";

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
            if (!validCodes.Contains<string>(languageCode))
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
            // https://ftp.mozilla.org/pub/devedition/releases/105.0b3/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "7698d531d31e9362af520e5f37f31e422c3db78c2b1e12b6ee881d4fe155736924091b8faf4bf8baacf2c25dad9caf720ed193cac8e2e03986ebb19865ca33fd" },
                { "af", "5e4765cb4225473307002c9cf01d916faf0fbd1fd088af466534f57b014e3250f02318e85b6a711dfda1fe23a5e4b63f3f96c5c1a017f8faa8d94e51db045ab4" },
                { "an", "1d75e487928b5020d2728771f34db2c8f9e265ce04c2c2ec1b99bf83dad78cd41a99ffdf9cf3b3a7e2b45b970f5fbb6e46b054043a5a649544c03107d2a5800e" },
                { "ar", "cab1273b715a308e4a1cbc2fb314174ec98f094802d64b5e48ea424bbbf20d2f94ae4cdac964a242665d1bf7a831485889b21cf2fe8903c418cbbe4bf1479651" },
                { "ast", "65893f2780ebb6a5a35c5a3d3cf53bd4f2fa1e2d30821190ddfab67c9a248bc2df4f3d0c352319c2bac3b7451fb984a3f0b5e5b9f642f5f22321a0a12d9f463a" },
                { "az", "8d0ecd879973173b233c9d97d138df34c8c0e4eebc546e3f32bbac9adfec5179e1eb486ebd10c5e8341226bad330154249b58688a18683e61da8385a0079a759" },
                { "be", "2e4d35aa5aef5c27fcfa10e989a7fd6c94b18a22ee38c8ba0ffd6fff2c20d0c9597cc8f5cd14b76ff7b5d50040c5dac281e3e7cbc3d7ec0a848955ca9837254c" },
                { "bg", "730f5454386cf0154bd7063b2a2a6c128aecf217813a983f2eeb0d5fc1fcf63f957d1739a57bdb1164223726493147dc3db519e5f5a6ccd42eb82a7944cfbe0b" },
                { "bn", "ac7bda7f5a797794aabeec60c89389dd56b35d4b3adfe362ec592844209a6e965b782738427da4c530ed0639d5ea0fa74beb05d34bf58a3d9f7527e672be78ae" },
                { "br", "d645da6d80bcf9bcf541be159af3993d7eb22e0a2be865009ad29cd7d136c1061e1a6782849d0d35dc2a0d308c598a79c585dd57e301764a222770440a817d1f" },
                { "bs", "3d69fa00cd50ccf108e7609794ca8d3a6e856f1607234daffd77fbf971c3b5f381e6de0dae36bd4b33944993aedc44168c56ca459852673a37371e81675b9f70" },
                { "ca", "1840f9ae6ce03d852e58f6e09ceff5d7015ebbadaec6b8946accc7c66052578ae6e7f0cd31080ffb728fda140aee101a40139b54cf51f9a6be69530e5cdb094c" },
                { "cak", "d446cfe76c3fefe855417ec1420b0346152fbbe8201851f0cfc06a325c5d558d75c303f807f7706e79248ce6afda7077202d14f1d98b324dd5b9cd0992b1307e" },
                { "cs", "a8875ad9602a79534224b87029dc9222af7fe7e28911dc9088e79b44e8f1bbd35b893d1c9832cc1b33e0b30b907007f5dd6cb88a2303b008d3193111feecf56a" },
                { "cy", "bc5b54fa7afa95f30dc2a46dfb6a19fa01008b807e96b9a70cc592e4da690a7e90565f4a49f7b68dcebea448618655639a56b6f04bad2ff34022b5f7be72db26" },
                { "da", "df8297770138bbfc608cbb157fc3a2b84d617d2fcf6499832148073a395f858a4362da952a598a8218f3a0c02e902b862123e27186c4c2d8780fcd5d00120934" },
                { "de", "2752285552858082bad2028edb20b3ae407dedf887f3a2b6dd51a76eaba277096664c20c25ac590a2de89452f3b190d83861e25385a87f9b22273771939c3f48" },
                { "dsb", "213626b13f328193f78b59d356f8bae28399cab5105f95045dfd416230a1540e4521ad566be379a57dbadf242892ce592867877e67facbb7ba2c8a86dc86b185" },
                { "el", "77b7eb4fce6bbde4113bf44d366cecfb09473ffe5a3f42a28fc81c826580d6f21312440eb3371449b411b0f0dce48face50d6a29d288bc140f5dbbc20993943a" },
                { "en-CA", "0d409afe4b99d5d142f2ac3d21f433373dcf73a629b2824f690ac6ac7c387ca72bcd49aa880ae841ad1ae3e9ef3d9e25c0de579abde0aa24497ff85e2e373f0a" },
                { "en-GB", "bf91197ab40882b809b8e15cb85a826712407ce1437627b782fcacb0a15940e259eaf426a86ec340961baa99d5f5edd55cc6ff62b45f308f83bbfc048b0f51d1" },
                { "en-US", "76e081ce037b59b7190c9c1f2e63c3609031e6d059647cbf898444cf9930aa8d0eb329b5503e7cee0ce63d8d5d347c17fa05b7c3dec9f835dc9faac738247c69" },
                { "eo", "001e1d67a6418bd2a25c7f05905f32a62124c93ad28a6d30f56ce1b5c11d927e8401b1f95fdb46270fa97f2659f18c06688de9a58957697bdf45fa60fb64007b" },
                { "es-AR", "90a3e0c925a853acf0629efefea9b2e9b8741683bc04d13bd4cddd86dedcaff0b297cec65558effbad847a4f064f7c8de74082439d00bf8e35a82f0f424bd056" },
                { "es-CL", "7ebe9e361f0c6eecc091d3358456fb12b6b682ad27aad77011faebc72741d29c732c48a0b854b3ff950a5bc92a8465dc29d11227dccfc3b44df71645e095f2d3" },
                { "es-ES", "9f4513dabbba8c10244420c9458d6fa9d919544e056fac4a0dd413de9299a9363ed9495b7ceeafb6de67e745e6d242a1b7e302dd10f6a5c9a25ab07bea7759fb" },
                { "es-MX", "10430ca005c53bc956a6c096528f2360a75f305f5e09a5d24e34692dfcc730a49e951a0963c24c59690c3a127761c00f896fa80c54daff73cdd0471929baf6ca" },
                { "et", "8d0dfd6ba9b1e1f8e6c2ae08eb554168d2ec8dc82e0d42700f2998f98f41cdcc77ed5dad0b37835a48c94d65630ec2bc74694014df9bf655e07a26d0d2690687" },
                { "eu", "c6e101e7a75329a6cf017dd01e983611ee4813c2374b55887c771df04869ef3472d83f9eb62d5c151bb89e28cf8b5669d415512f5ae77cf48cab5dc19d0c94a5" },
                { "fa", "cfe136bd0aa659b209b0cc52ba6cdef623fac7637912a34f4c809b92fc02c8dde5faa05347d758b287c3753dd7e4b9e11ee1ee66bf65889dee28752b555c1754" },
                { "ff", "323fe5bf65337ecc83e8e820c4ed27cbe9a0e400765381c4cca4dce6e1e2f69bda28706d7d04da406b24afa384a51902c8f10dc4f6c4bd0ea34eb3990cf60527" },
                { "fi", "8b9c12ce85c93e9a5af0449d0596e18fed282268e9721f52193c589778f5d14b70536c22277542e9b3b3a1590a3edad43fa0455d236e8a0c2f20df01dd44ddd3" },
                { "fr", "b5332dc47e522bbcaea2c55be8fd4fc1fef629ded1aa074a6caf46caf41a78dca40d938d84a9bf0d4a23dab672acdf28cf0a7c8b82bdfc290093d4d37dd98b17" },
                { "fy-NL", "395617a21eb5e0987c4899e2aed354c4adf4401631aa9c2e363e5312c01874eaf9b3eb940d54be195a6316fcb1fd7147e57d065b614cf7f5e79f92c47045cc47" },
                { "ga-IE", "d66dfcf08491b681cc3ddb334aec4e745b1f7db1bf2bee1484d6955377846ac0afbad918c0fca9ee0847234acb25742b26f0100fa290407013f5dc084e714e70" },
                { "gd", "f1b5016d5d4dd46280f6a5f281c70fd5f93037adc4d8c560e5ef2c6797f13052aceeb8b14bd8a2ca7ba7a0a34f2796e98a4233b2e080967cadd765d99b65daac" },
                { "gl", "e3b2853cb2d77445012ce9fc218ab5fa606d86fa87b22e109cb06bc2aa71b38f155e5ec89a825e8b970ee9aa76172e49d52147c1b60f155b4f34db73d7156175" },
                { "gn", "1b3f7275aa8169b409799defb9d14a618736fc321ab45fadce2e5a4a79470093342d39441cb122cfcb08ad1d39d619e81361f9591a28a6e4ac87b5a74279e70a" },
                { "gu-IN", "259349ca7bf161a995b5e8c5d58737d8da3b749b4829299483d6af2b99e2d75cf969623bc0d33a9318dc1bc01391343af296d74415baa0a904f649d92d569048" },
                { "he", "c9228634cbe685b4172a18d9f599e5f4fab538ac06ef66bf834141bee84b2c42958d2ce48b47a98845f0b7174e345b9a486d01bdab7590f37f55688f2facd249" },
                { "hi-IN", "f91c3c092aceaae6d81dced69807348d3cbda17f1219c488dd2459d36fb53e5b3450ade11e450e484e4ef912e2331a49a24c120ddb681bab639b460b98ef32ec" },
                { "hr", "72ec0fc84ad757eb04f32967343fadefd3d3491497e817954f378ff4527055de69b4baab8da4e74e5d34373264c6ff7fb03277a2a1154cc2c65e9e125ac3393c" },
                { "hsb", "fb2d21605f4576ce22ca74befe603724463e2224cdd8729286e67c05d7fa3fee2c4ccbd629b901da0a5bf1753a4748c602afb44bff2a6ab6e82ee551c22f715a" },
                { "hu", "b5ed00caa0dda281357949264967ad67fdbe36f8444bb1a895cf651fc3352a7bc156eadfdb94cc2a570f766be8403e4c7fd13c18aebcf849e87d342d6ef81169" },
                { "hy-AM", "08bcc2b35f8e8c3c299c7980c44ca0509829ef38ed76cfb0d433d8087dc70157da641dd3d4341e527de0ac8e0928c701c763b0bc6e50d4ad2d787f6d937876d5" },
                { "ia", "8cac822c95fb160f68f11bd996074531b57e4172add6dd6d17b061da1effa915aea4066d074ddaf0acc91a8c39728ebc2b7300e9ed1b24b4e43413acce46743b" },
                { "id", "e5860256118f942f5da689c54b6148ab16262f6cb957f1fed610da3d3337bfc89cd1111bb8a74bb17e4d6eecf31a5b3774cc3c3822d8ad227588f8f9529f4600" },
                { "is", "fb03f9697f4bafefa6a5664fc25e73855ea154b070440fe6e64f14dddff85a3470b909c4933d6343fe93bc10b8786f8189821ab1b4c9820096dca934db78bf2f" },
                { "it", "7897e593924cdbbfcc4a6eda0faa1ad3e2824d9e66aa91b7acc95b280e85fa80354801c47fa9e24dc4f685bbb9a2e43c9962dd29f154411ddc966a8f00d03d80" },
                { "ja", "3ef2d8f46a9a28de146cf84ec07b9929fa982affc81e08a0362dba9bb4fe9d7a2102273d0ae6e1f8ac4e527a17da0ffee97c05bf503dfa1b79ce67d80b6fa9e2" },
                { "ka", "5fa7eceb6628e626e40ba386b3c1c88e227a92adaee01182a43a4dfa9a556b0cd2c4b2f986f1e625db2d3a870de509b7c747147444285877c6398af93cba1e45" },
                { "kab", "f2b4aa2d1405c8db7c5154aa499580f1064fb8e4e417dd40471a18451d940ae45bc19a2aa344f6f218a15cb396aceef87b131c1eb75a4f3e02f045f1b768a137" },
                { "kk", "ab53a58c3108b936bd8cafef26d94d4e636c1bdf64c5ae91adc3f7dd1305fc327f8174b893c7585968304809501157da9db1792e729a02c3f64de2041e7a3c8a" },
                { "km", "263aa6768aeba6baefe16ddf3c22c567a63066ef0820b3867fbeee4167516a41b6e38ccd7cde0afd944f16981ffaabe055352783af522e4c75b45cedc4bb1eb8" },
                { "kn", "85a876a0225c5cf9dcfd39b4bd4150193d4a692871782e60b8c792d955377c0f430edd3602081fb343029498fd5fa2218ef3b08cca93636b63d83d039f5d3dff" },
                { "ko", "5d0a7300b459192a8fa9e1e7a974a130173f3a5d8fab6abb8aee7b51e34bbf59c29d9080aeea7a428850c6c1f743ccbffa784a9122728dbc29604b7fd03f3cf8" },
                { "lij", "110d4c1864eb3b24dc3f17f73763a6994bf2b498e06d486ddf4daf05c6a4a33748dd8e44bca5a63825fefabf41d7f39f8c21b7e5016214d1af48bb1ffae4353c" },
                { "lt", "236045ae0beb5a9db61059cb087006731bb75b5ee1fd743258dd15b704da51014e2f60e9f99cadfe817008b77968446a00c1a7a69556d955599a88468280b599" },
                { "lv", "426d8c644c7eb3bfb8b04d29f25574c118d423b04e5b456a316ebb29208bc9e50b765e74773b0e501a085d78ae8f7f68d8a4fdbe532aedb693de8095bdbb282b" },
                { "mk", "ef35c566281c4aeb9c28d0876366f8465f0b6e883a2c7bbd1a135a2715bc583aa1026d1897590684ad5711be990f16405a5ea9f686c45fc59986d9fbdbfdcf94" },
                { "mr", "2ff9fc5636639663ffaf62a7b678a50ac7f273ba2f27c01c963b6a929c0acf754f9af2306ebbd338ed1964d6f2210dc7c1810e9e882ab55b9a615a731999e0f1" },
                { "ms", "8618ce8ccd5695edeed3992c532084935aee95cf94d768bb32660e88e66c002d14f9038b00291654ae4159334ca9dea4191d79839341d8e327810f4f80026182" },
                { "my", "c993198a1cce2693827b448fdef9a908a595b11597e73557701758e5015f5ede7d0f956bc9364db345bfecf3d475af10b075c2d334ff9ca365333e3757dc156e" },
                { "nb-NO", "b3ac3c01798115dbae4c1319134141943b7c688c2034e397bafaf093809e2ddf85cdf4d5e7745adb7c12c70b4d17f2254376ecd57fc96b5af18cecabff8e9c1a" },
                { "ne-NP", "2a8cc37ac913896c8603fb45c662efb724cae9ad80fc74fc13c534a3809fbc774e3523ca4a7416908790cc58a2832678e75bdd0072feddd3eb49b1b94f82d373" },
                { "nl", "28c0fab1c3fef374a46862bbd355e602c240c16137a9bfb377d5da5933672142a95ce60782102f999b6f19afbfb9d06609327918cabe2784d84bdef1e155269a" },
                { "nn-NO", "b34ebc790a8361579176ae0ca09ab9fbcefd3bdf73b33746afa756d1f608aacae3107692cdf226000ebe7d3c2d73f6a8f41191f99834d666bddab3020e0b7034" },
                { "oc", "d2c614d6e8435e2c1990151c16add0bcf9810986ef3c062d2eda516264f94e6274605ecd9d09b4cce013597e2ce139d67b13898bde6a1b3f50290b518bfa18ae" },
                { "pa-IN", "da8275b659dcb1190a199267eecc5e14cf3b459037a851600dd74608c2a78ff2006765b8b0aa03b87a47a695a4c55968b307a621fa335aec936cc5f9338d1bd5" },
                { "pl", "85d3c556a4ab9b2c453c913aa02380ac45a2a90d2080b3a8ed57465b565ba742a460a4170ce984ab85a7d4ed7c2420135ea4bc8f83c0eab1669d46181e08d35f" },
                { "pt-BR", "0202ff1eec8b09298527849fd2f6e068ff3645f496d189b5dd2d50bc970f09092211e38bc51b120a493a8bdc01da4b6ec979e0ea3638b00961aeaa3a5fa286c3" },
                { "pt-PT", "0a3f7f84599f5e505b010a8d9ffce5e4427e0523d50acd11e2afb9b0ecac6f386fc4c9b1e75adf7306a7cf5fbc9e1d4b3c91b444c1057a743fa834bba071ee90" },
                { "rm", "0ced2025c8574327f34f9e95bfacf150d2098dace7877fbae7ae53bd2243edf5791d5130683023633e8555f05272df73f04258e6508998e05e34aeef37904f15" },
                { "ro", "7c2d69b5d05985bb8080ab7b67e2a8c6f30018f4d9411019eb8f1c1081455c6111c7bdd0780287e56203d1c6796123b8e291e9638a3fecadf9f954cca02a5962" },
                { "ru", "a6d9eef91d5a6f27cf2ba3a467cf118d561a4a3cd5b6a880aad7df7b3dd6a1b81775e6d42312da4ebcf01bc3aec56bc20034e3ee6a9f9c822b11d70625b27991" },
                { "sco", "d7d4b23a0cfa7250d33480be86bb1e12dbef8a5ed24e21d78adba37c0f4134c5cb816d7659c9b9d2d8d6f35b1c0bd616e80a64c35c0e21d242481ad9d45c33d4" },
                { "si", "0f8252b360610a84b795b6ae37efb8fdce33ec0beb69d3c95c748ba4b886a80c1804d3a21ffd4738341a6f79ffce944d0336ca5e8c345b0e8b0132348ee2637f" },
                { "sk", "d7c15bfb3746f3bc0a13b7aebf3902a28541506ffbe059df677e37fdf3a63c9cd074f567fd751e0f8307f95a520f590213dbbc075619ffe7e48b186f512bcdfb" },
                { "sl", "b195339668f8449ddbb761efaf66542ea971c433a8eccb6628fef8c4b1e1479383767796ebe166d140b512a6c205fa9aa448f747a2992419c908a744731f51fe" },
                { "son", "dfca5ab0bd253fa5c5d5dc34d5bf69077fe1be489004227f09af7e9211139dd2e5f5e9ebe54a372760efffacb201e26e3eb8ae588d209661473c36ca9a09b50c" },
                { "sq", "67b645bd5861473980002b97605d8a784356fc43db4fdcb65052afe91faca4fdb1f493f985e0932d2680ecc731f9c51e5dcf3accc86a4e517ae751f1483af49e" },
                { "sr", "b8041904ef1bdd1fcfda074229ab1a145aa9a11116f2274618515c6fa1854ceb619d93c3b3aaf750bdb08e73249dddfaa4403b196f267390bea20460e669d816" },
                { "sv-SE", "6b01f759f1452b8a5db20f9713891dc1741a2ea34444f6bdb0e1d69d4886db381881c4dcda067b48f3ac99410f71c11fae780168550bbf24e8477047fdf1265e" },
                { "szl", "259e032294612ae3d7fb0b7462210d357f5c4977df572a95d399ceb28958dcb3cd06b22c7d6181c6571060437845835250516da76ec4e778651a4e03182f7ac1" },
                { "ta", "7aaa55cac33beecaedf9140094f949a6ae8e71be7c6e9011bf17447ca569f1be699471f18f972ef572382baae3b6a4ac60364b5f123210c38316474e48545322" },
                { "te", "dfefe7ec0a5969c366b7b75730145aaf3d49e294fc5d5866edd910aaba79689587e33dd1508ee7efc87d2abc33e911fccba73c36736963b2e78f4a8f93ea81fd" },
                { "th", "9c966e7250d95c1d6c5454e4fa440a66b175c4bd028afe21e9c5f1cb969f655af51744ac95902c12ff1a639b8172365143f23bc423979e7cd8fa743f16a3f8b5" },
                { "tl", "01dea0b5bc6e14993f0b603f6571cefc791362c48c3a463453c2b43c233b0d5394c4ee957382ee2387089138adff518c40183cfeb5af6079d086b013d505c16e" },
                { "tr", "c595b34327db780c90de3167900def329d63f453d767d3c88016166690e8530c8cfc40eb50cdff903ea35d7c7821bfb70f4408b78f09e1de30940a530245507b" },
                { "trs", "fae291b5843878d1f547f390e5cbf3e5dc2bf1e89c91a6ee812f77bc7eba29b43df24cd90ca45db3c8e288a90c530241e3d530a0d48aa3ac4e714708b66a5599" },
                { "uk", "47007737d54cb9a35d14269b9fc947ace3fb580f143c1f2b1065caf6123eeb92e4049a98e8ac7a14ba499041c29619cbadd9e41dd925de5c50037f12d8d9eec0" },
                { "ur", "014d6e01d9562a72e7b15395a5cf6ca54a8410412cf07e7373611b7b0227a040e2acc633bf5c0aa040801563843f792c47120e1da60d052deaf5c8a9f34f3360" },
                { "uz", "977c35eb0878da7a3b211ef45df114fd1ee85a96c4e88305721cf0d3bf049afdda88c9c6deb8bc2bcae78624886d15a0d647aec9d5438a890fa090f946bd6d0b" },
                { "vi", "5955514cbd62369e8fa621bcc6bb4f2b47036c2036d2270c2a036a64378e0a88f2f01aef580c92d091c43262444275ab4aaa1aeda1523dec595a2fbb98095760" },
                { "xh", "05cd6e33c7213dffadab0c5de6ca390dcf01c7ba4366f709599058a538b1d3ff0f21a4e5dfe01f6205aa41164cadeb7751d50e9742f4838426acb6ab4e79b938" },
                { "zh-CN", "f04bab6012bd3085d2ce3bbcc06691f126bc88852655fae5f042fbd5cf2fb351e8ee9fa1889af3091f2400e3444e88f2e9cb443c6691df747a26bcec9783a32f" },
                { "zh-TW", "25698e6d401516878ae573410c390acee0b28f447cd286d654e851723f1998f4aa86b4ab0664824ebb64ad37c7aefb5f518f06e0589e5e4d936c5b1ce0212998" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/105.0b3/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "3387020b986ec8c85714cf68cb54a65c7030e28df48ee8507012849df3fb71213e7e5afc8bf470d02d56ba0b636cb54bb9f13f53dfb7b342c920f1ba06d2b90a" },
                { "af", "2a62fd5f91ff059fd52ad6c6f509f273a3660f04f45ba6987bf6363298b5e1e1c0d6fd1c6ba8c3d5008ce44d832a0e6586e5a526240757e093978417fe00d97b" },
                { "an", "702d702fac600715c57c9e5017e4de70f16327ec276e946291ca8f7d3c65afebb9994b61c4a2e5394763fff270f52e7b58cfa40491386704dca048dffac87943" },
                { "ar", "5efbd975b914ce0b62870ae0f6490a4cf9a3e0c4cab147f6c5a93683a92e8f85c81626ef930f9f61fbc969bef788c0d7dd4aa590841bd8a37eec2c374c67163e" },
                { "ast", "0a338f3ffd5ef3f50b1317f22f5b4d2a3d64d971a38e960f55a5afeab13aeb459f9510130fa5c18c46e0a04e65f5eeabbf66d6637c43a2ac7249a53fe4f10c57" },
                { "az", "719c6af614fcbd10382dee881386ed7ec82682c21b09048e465bf8e25c5f1b6572b2a853e76d05cbfe2857c9279645000463ccf842d8811dcd62463f043350ce" },
                { "be", "f7cb13345e74866d1a1be2a601d24edfc149d61d27acc5fe2512070b87a43be18e84d0d7b4bcb8d1b087588bd6bbe9ca27c4a30d0d5d6180bc76899bbce4648f" },
                { "bg", "9ae4c2beaf95409d33183209390e94e4fb99d726f299a7aab74011be42853193484eb57d596c02d0cf277db1c04574ff4f9116b21b9c7d37a9aa40a754d11821" },
                { "bn", "9c20f72b47de03f43aee8dc285193d6f3fe134cb95a591b4b80e816a505eb9db216c8bc4ac5ae50115734602860b448dddda31165f0cd5d25a0fd7dc9fb88cb5" },
                { "br", "bfdfae3d85531d63df0fea5f93ba18daa1f7e29e5f2bc7374a59958b7ed4d4b54ecc62d8587571d2e2a66f42c948a91060ac15d613b3d3278536a39eac8df7f4" },
                { "bs", "431864742394b97837f944881dc78817fea5835ab3f5cbced5f52206cac86c52cd6bdf23a580950a0a0b2df9960e3ac8d3677fe23299e7755b3fbf058d037abe" },
                { "ca", "d543e5c64dd02233e841c833e4fee995e5d8d9646c4a8eb40279b51b6e07258b148c84d22e76de7f0d54b5a8980117fdb3c6c141e3085fd9ef0406cff5e81b97" },
                { "cak", "c2d7efe25eba95368926bb38df32c88e295903f470bf752bb2470d2032d346ecb36e0ead6a15c63afc482d34f1ae559ccdd716f18f281eded06c2cbaa393c1f0" },
                { "cs", "81e30f7453a1e12988440adf4c0974eb392717cf6bfcd2e17318766a134f1a2c3d503ffda89b18da6384e2c981dba2652c1da1e12c94665530515f300a75500d" },
                { "cy", "3ac92923abe4e6abac34a131b6d1ce6838e64397d746f2f6a004c05983b72a092da19ea490bab00c6a536c73dd3b1209fa61265a7b8880169331abfb0fb0bc81" },
                { "da", "52bc951f218cceb80c3bc52155a5472c9b0b21d3b46dd66619640b611abd515d3a5d9054424bba5e608dd6793dc089f8d68d4bec4042574a888f209ecc0c1bc1" },
                { "de", "ccd49e8ba2365a59ac8c3d2f2700404e8607cc952fa28ff75d4a260330a4528a143ade67ad8fc83808895eb1b24f728aadf62569eb827b0ae394576545686031" },
                { "dsb", "578fa8129f874b4f94dd319682f87f1c0c0edb0632cf75302f2e83714d31efd86bec5d6a533562fe117528a7bdd3f82c3e5f965bba6e2c9533692e172e115a20" },
                { "el", "0fc748eaf20cbf361d7633a3abd3f63c0481bf57b8909b0cde688a2de7813ccd071be5971a1b4ef4184fb4743e15ce91e322a57741939a3c50d83246edd94853" },
                { "en-CA", "9cf14595df1384e2343eb5027ca43dac0c5e4228c59095532d3d88c399650309492bbc2daca380a39f01e546714bfa1ecbf929773118298337794121d9bd86f9" },
                { "en-GB", "38b36bc3f6de37743b2c8619663be4cf2d32c44eb34abed205aa18134d8569da1b1d5a6632989025a5194382e3eb88ba9c38545c329f66b5098aa57636ce7937" },
                { "en-US", "3cf17a8ccee178ebf82771ebdd91c8fca73b8a394ecd29f76491dc0203235d8b0832847fc39ee11a93e48314bee52361eea850d95c9bd441d79e5c9ba3b0ef3c" },
                { "eo", "b5402fbf0a09064d0a0297c60891c6fb6a81a5031b39d4c41f8ec777165afc12bc42b9621c8729aea33f97a2ad1f3e0bae29fe251b7e140cf270417691269fcb" },
                { "es-AR", "ee4974da74ec3748d9390690747a9fe0e271000b120809977a80efb87077e9458bfff3113df08df309bfb235e0e6d91d3579c06e8543c2c570388d11a638d58d" },
                { "es-CL", "6e4ced4677c4dc74f1fc96c0d533ecc03d3713bf3c1c4c36c2b9047805b5687c7997b5bc6ae228b2883c6df4f125e3dfd69e2174e40aaea5766aa4827359e95f" },
                { "es-ES", "3bd9781c46deb5033597bb94b37a5a087e64e9169dfeb04f9c8ddda843dc4d458054f11d19dcb8d77554e201c694070cde1bcffb14b7083f7a6872a490384d88" },
                { "es-MX", "cdc25fc499af86b962cdb5d3fc019d1c9a506a525c2131054c1832b9cd103d114f1e2c35c301608afea535ec64bcd5d48e322f537c23725837b465e9225d2977" },
                { "et", "b672ba1d417eb103a34b718f112fb490d7ff7bff6fea14c958b2fb98827b0004644955779896be381a89da9d5dc805002ee033624f5b7cbb6de991e7e34c07db" },
                { "eu", "4aa8bb9904621434f08e780e1639127e8d33f140e8e619178e8c6f75962e648157655072d112d3a31d56a73c2a54c6538152a5b2171b4feeb02ee1206a5c8f03" },
                { "fa", "20f81a57dfd9a8b6d66e9160858341c0908d5fcca7053e08ced60e090207e981ed7ca195477e808a44e0e71f598cd646b81613bd44f533e42b89076179a3f8dd" },
                { "ff", "dfea16db6401eb88575001b510300db28d6be7b556725d5bce2bd2268cf266b884faf0d1b6eece3293f5d93f4b68ee1ac24ad678cecdddd4c06314f4807af341" },
                { "fi", "ff603835282a11c19466f93ffeb04a55182727ca05dda37b69084ab9a804644261fd320dfc47fe14d437d4bedcbd1731de3bdea453be19ee6332fe3aca6e4ba4" },
                { "fr", "3dbf7dd4818b43fc13c632e5d300d7293f19df7da62b329680b4a681116f85f9bdce50e0e3a43281454ea67ade146d6c937ce117c008887df552ff3c380d0317" },
                { "fy-NL", "9a7d4ee1c3a507439915e798aaa4edf1b9c671b65b630bd5500dcf59c458aed61f0f951f2eca7bf34dd1817e23d37bd8b8d74b7781db48f73c532413e814bd84" },
                { "ga-IE", "df903e5c3d87e0e6bb46c5bf424a2f4006dd61188d33ae91134aaea2ce41f8fc11d98d658eecaab177be8ba451bb082b5df73bd37006f4062e084faaf0f5a779" },
                { "gd", "fa32f8daeb36fde8420c135d36f27cdefa0b40b0b378185a7f1471dba7f962eaef5af377078c6a6f53892acc84aa5f4f8ee596655c68645e79a75f8d7fea38d7" },
                { "gl", "a6da7dd42d72cf066c64e7cf5122d20ff81fd66e7771f013040cd7f249223ef0c89404ddd55021b1abbe1804a5e4dc6b0e3b0b619627dceba5aab5a669d9d6aa" },
                { "gn", "e863cdafc738fe901cbb2678b876992114ca965b8e7ee35a518ada0fd9c35775deab4f06218a0d74c922c053d937fdba3a10f9800732fa5fc3a0fa8c5b4c61ab" },
                { "gu-IN", "643dc323c1e284ce55805b2a5f455e3ebd6625e44c935ff01e4c4f92ff6ef3d886a1b04ac0e0cf44f2a14e3c08cb42ce45251baae58e3b04434b85f4219d4cf0" },
                { "he", "2fd73c402e9bedd2b9ede2bef6379273605f16de8ab56c283d8b19ce97a55207be649e18a690ec9902070ce0523848f2ab0e1eda44030fcae773c60c0062a837" },
                { "hi-IN", "77324284d7dd6fa377ea54fa883e120fdb9d3f7a0423254f7c2c2c20868900c4040a762d4f626da35e547c42ebc3da6ff64020f97f4eb3bf87ad246db782bb16" },
                { "hr", "dce132c7154994b9e6e63841abd16534b0884987d35bcc80c9ad47bfc2ff17923d7675db79e55f19ff913a3df1719dd2cf36044c33de6d09a72da11e8e8ec73d" },
                { "hsb", "666280580b4a17e5270528b9d0c03a2640f58207fd167d57eb75ab62f70651a65251916c0d624c8f917c0540d544e1f67039fba8312dd1c97d5b7e20b0bc04ef" },
                { "hu", "01f1dfd1d4563eb488f15b06a4da5c6bdeb043a8fbd38fb2a81833cfc62f4c277882d82de545be409536dc3aee13fc1f780bb626dda0f1dd2762e9d8d3ae1029" },
                { "hy-AM", "882deb472c224e1871e1e48c47daf5c3f5aca6e200e58f66a4e500c404f5a06ed13bd1072ede7a563562cea35bd3fe2993e6a0448d0c52cc94f9bca92e1b6d82" },
                { "ia", "de7c167fcc3b74c67f1830493ae0fa7e0352916e34ae897114d51af38675897939470a059c02de4e80616add524fef695cf2bc23033587f1c50de24eb1d432f2" },
                { "id", "dfaa00980875ecac062eed5917c3740c426c5216f2e428a8a3798a83e8e9b1e7044bf3f25cb89405af5dc692d93f1f4637a904e88ac5f56face37c3210896bca" },
                { "is", "9d245039ae0d18a2f0f09cf027cc8e16a4c6dab13058a75228480537e765f67aa7fa3c2932e8b425d02b9bc9109c9125fe093cdeb4541d9c07325637d1423c68" },
                { "it", "1446f66f29e49720dedc981a0e87b5cb31c5410351c4a9140fc128b45d00cbba7b6c5575aa400aa311933581b93153ee0a1a22c15020d019f88ca2df1595c5b0" },
                { "ja", "5c24a76267c2804bfa30cf5eb8ae997ae654fa8e1ebabf3d84924048ab718070d47acc02e7c188e2b2d969bf3355d66df9bf926df025042b156fdfacefbdc2dd" },
                { "ka", "9f964df80f2f882f77e68ff84c7d9bec9a336ab09ea235ec4d1c540116c41a2216b7098d840abce501101257addec3732e35a50b121032eb464bb9b7672e9c3b" },
                { "kab", "4bfca46143bb26b3b37017b768905a1954d3f5f3ee581f7a20627e859dee374c8d7a4f9d650335c995dfe0b023bddceecb15df7a0b65f803d9b57407a954d444" },
                { "kk", "46877b7a31c5699a66e8ae336d57ad3b4906b0cb68b7f9a626551f3bdaaee89337517d1b072c5a2e681c28bc8b6fe6d50b88de0a3fe7a8d239fced52989e062a" },
                { "km", "c0e869f03a2b547d13a6bc72345872459487f34d80c0c1309f70a92888966668940f9730ba94f32d51328159d8f7f23fd7d614b1c3ef1999b4b12214a958a709" },
                { "kn", "2d75a167a9b170677fb2eb2d376ef1fa521ba3adcd645fd6cf2cc1670b0a4ef1777ed7e183f6924466214a4719a24151e8343cd99227225e5d638e1fba010b78" },
                { "ko", "b1ce3f35fa1b55f388d5866a71a47a16c72bdfcf3c63babcbdab4795cfedb9a7c199ae2d00caeb78a580b9fedbd7662e0e74f3afd8b64d541db292e88014a920" },
                { "lij", "9fc030a0d1ffe82e8745199e6db24721e7023fa10a100769ec8fd312237c3b11116bca76776d8ed796b59e458c2a337357b34a41477106d2bc3d6b8ef385e8ce" },
                { "lt", "1379b56fdc4abcacbd851fd85e999b9bc33890f07d771430a1bfede841a21d2b5d3150d95213e70e4b96128bef92284da7a3782175fcd89988812068032118f5" },
                { "lv", "50b8ffbf78a6471adcf39b93b1a4bdd5ca96c4519b0106570e9f6a2f2cc0bfede553a69e6563473c6fc6f8a4a32e6fcd69bd198ee97f8aa28bd4a22a4e1a5955" },
                { "mk", "076356b4d93f36893234f72da79b7ae1e1c6c985ca0fee83c1e1bcfaeaa3bb73de29aff75f9b0fbe6ea87522e084fab95c13c4a0c5c58feebf434a1350ea7757" },
                { "mr", "714a62ba2903bf5726b36eb5faf75f33bd6085a7e60aa7927b654ea5d1d4e790367ff4e2d40a86975c9b16216e1e3c76c24764c4f4934f30a0b760a03bcd8e81" },
                { "ms", "9dd986ae86e39c39f74e01aec560bcfc98f6408e9b89d172bc0af4e1f0db39d2868c66982e010404282df71dd062d606fce944aa29d54ec885c4b1855dc68527" },
                { "my", "100ec8c56385dc1562cb726d1d29284f77dab0d6163a9bdf5a8d61e04ba1ba6857195e18fa2c6aaff7c72fa8720b0ae85a35101f1b7733df196620afe5f56140" },
                { "nb-NO", "bbcf014aca740af99fb9210d607fdd3b4fb0ef28974530879f0fb4031c9f40a95a1cb03593ce0937125fb13b349fd828b8ca98ccd172694ac4e14b4b78d29664" },
                { "ne-NP", "a8ec7aa991340bed6d7cf28d27a8d0e7b0fba3b134ca01f00817b8a1f9c9fcec72454cd73b594173f633d3c675dc503c806e9c5794e6bef4a1849c7eb4fe235e" },
                { "nl", "1d553d95eaa60cb1b5341bc08db654afde60636f011ba3c46aed507ffcbacc5d42d22ed8587495ed1e4d999aa4244000edb2294d8d5f07fc5c03a63d856a0f8b" },
                { "nn-NO", "208ba10d3df4592d1fcddfd0479ebc06093361ed3d9efe1fd46d82fbd4dcea6afbaa4285f35be46a24323388429327682e6ab0a22437b4031d7f011c27b8feb9" },
                { "oc", "20fd80fbc8a2f8f45f943b28f0d080d994ccaf7ed13a71df97e47483eefe215d56519eab39ca17ecf3b80c268799f5866b8ebcd2f3f6551652ccae13c2344a34" },
                { "pa-IN", "040ff7f5ec52eaf21b782b8be290b349b775f766af2fbcb896ec5d689097d69c95864e6ed8ebec7eb27d72a5ca91f0c5c2bf6a4de287878c05584fb8df9fef37" },
                { "pl", "c426a56304d99ae5d4f557107027bebe0417de369e75e6081496191340bab2ffa7fe0c463118f51f5a1f5ce39e16f1d366ab921b9bd48d7bc52308889a0c1859" },
                { "pt-BR", "b2985ba9eadc02a885312847b0038173525c8df35890a3e6ed501801a8fcf9d6e042609ef3b65161e9649122e834b723076e93c93425ad44f12f6e9872f4cb68" },
                { "pt-PT", "f85d74552a942c39d7ae659d17137080020cf3d0b50370afd4808ee302e575c279bbe377e6c4e56da2dcf9f3b20240faa3254ba56adc4c501e72d5ad9b35e861" },
                { "rm", "23fd1b2a9bea0804728c77e80bcad11cbded38dab14746d5ef27ff9b6383adff284deb863a590c76301ce48ce51d6aaa722b31509a38da9babcce39fe7c27a70" },
                { "ro", "1998d8c0a43278f33320e08547f6b45b59b0eddd199fe27ae7ac4e589323684f0e591851810b64d7683d1b94652bd2a09762054bcc9f474982d2032a22a49784" },
                { "ru", "d1b6a679ab456d6cc5b5823792a3473ff589d6627530529875d6f83243a4f9707622a625a4b9a95fd20fd86bff4156f6445d3c3784bc77d4edb97edec354654b" },
                { "sco", "674b44200567fa971eaac4d7fc81123bff49b629ca2279c82a77ac4ab2462d2a991592cd939cfce83306e905b9f8c0d141ea7277a92cb4cb36a9f742abdc3089" },
                { "si", "04f45948a8899dff4c3e4d2d2ef869e3cb9a695aa30115abb6bf544fa689ad39c7af7029ee84c000606b77903321709b6954e87c914bc1b703ba54179f448f01" },
                { "sk", "0c355b60a52854175976377ae5c3bd6f24d7f85df5477b2dcefe6531d882ab57bd12bcd0e55e8e395c737905690530c0e55c04dfc8a37933dca8563c87d27b5b" },
                { "sl", "d77eda7f2a369d28e59087288729366cf7dd879eea68517098c4ebd00a958c5dd5284efaac629e9583eeb178f0786c960a245faf196fa4ed2eed62bc5bba7753" },
                { "son", "c44d9b8842f7893ec617d06dc3f58850875543eec5e447063d61126a8f2fb8f00ffc7e66afa6749217c3d6c91e3ab28fd968fe35e8da6057fe7c035ae0020415" },
                { "sq", "18da8440b13527fd65c8b95ac029f1bca5d983674290ceef55ce306c1da9ad9a20aad9a79201479ea3a70b8a78b978cc425fcce588d2d6ef391b57eac04c05aa" },
                { "sr", "cc4c4d8f119392c3fa9c75d438e5b47ed3c3deb52dd3faede053c7272a5b5f82765c2885f9ca37832e28d3859a0645e31aa02ad7e527defc405322969411c796" },
                { "sv-SE", "91db858539404104baa9939fa770db419e2fc04056a602b23c1c58054fc89d5ed5e028cb532cda00d69efe1e79cfaee347402b987fb0b3d3c82bbbad0f8a72c5" },
                { "szl", "b7b4b7f15e7780f269d5e524465dae3c099a42039eb936cb39e0a01037cc9bd45bf80d4ed3d2883e42fa58ef9d787e56de14e484128eb4bffbdefdaa5296d252" },
                { "ta", "c4d80f67d5c177cff1452407edeadcf06a1b23a2629185d4f98d2e338a66a3956d6e8d11b7cfaf705f3c263fb1fe30d054a609234763cbe02bed630125c3c88b" },
                { "te", "d2b544ea4555481f026eeb04cb28908e02c59a901619ac376c16d77fa69bfcac9c6393112fade7f8caeaea9cb2f6ba57a98e6cd5bcac2c0e8310a2bdfaadc0b0" },
                { "th", "4f15993eb1213683391dff8de53de93779e6db0ee9472864544a5b952942daaf0b0375c4a046b58729b5336d6d99e82fcbf8a483b8c40f99697f225b04066c93" },
                { "tl", "b2fa9e2d6ad7a8c66c5d27435b9a18c33a8f508b267263dc70d91dc31368f88ca112198e66de613ccfe17abae40b642e2d8d24e650068facea188b7c9cfd9e8f" },
                { "tr", "ed20c4379b8c4f0497d397b316895fda8d1d0a8047d8329756f4163a6dbbc9f6286bd86f4d8a3df0b913950a830199553e157f6577d444478cb97efd97f6b073" },
                { "trs", "58510d07e2fd73f2790ad4b373926c2812de00b72a31c887a6b45177f58ec4e5fc33f262706f4441980741b747d6da314aee6e1f59db1ac7a599b0867f97a830" },
                { "uk", "562484de2d9c17cf8407b31f33d76f146446d8597ca87dae7afa2630dda96bfc8854a47b3c59ce7c997717adba8d5c43e911c750d3b3f570e4b181b36baa11bf" },
                { "ur", "c7fe06e2b5b04d2a240c4140cc66dbbec68e715f25eff3ee39315e0c5f5d6fcbf4e44d4c24682e2a8c70b032b7e80cc0abb087d55939a30b48acba72109d041a" },
                { "uz", "17d9326d0def64cce7527292bf11f5f7b8b7720ce3914a747dca8a27aa2d9178419413ec7c94d898af3a9678d10ff15ad8cb09dd3883323948f5e736799584cc" },
                { "vi", "821c624d7cd7d029d530def5d789ce79731a1a427e11875261a18baa14212f6ce5a622fbd8a31b86e5685240ef11a32152baa3d855faaf1ef42aa204c59249e9" },
                { "xh", "f7244a960a0314a4ea8683fab01bc3143f2a152d5e1583bd696f6d777535c4497b76a2633a02e41d554b9bc48fbdbe165f8ac3186aee3a7c88eadba90b133acd" },
                { "zh-CN", "c8e45e6bf7b7e8d7bf8b3266d3e131ee6f53a572b3f69661fff51061be823f9f0f5e5da890e5411e6fc7c5ec6c18d7c0cf0dc96b9f5aaa3883773bf0288cfba3" },
                { "zh-TW", "88e52f85a25369427c010fd3a1593cb2e836e895e83c09d5751ccc38a6e3f2fc8236b41ef29e76756be62731bd4500995b06a3cdf06953923aefe9f1b816c88a" }
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

            string htmlContent = null;
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
                sums.Add(matchChecksum.Value.Substring(0, 128));
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
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
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
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
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
