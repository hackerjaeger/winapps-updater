﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021  Dirk Stolle

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
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


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
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/thunderbird/releases/91.4.0/SHA512SUMS
            return new Dictionary<string, string>(65)
            {
                { "af", "456778920e967957f4c4fd617aa6ef3b626cb4c239b0e8c156f9a48d2ac358a78d398101ebff632ea7d49f21be477ddec9a2127e666db4bb54a9a803fb97cedf" },
                { "ar", "e21d0ec9306c1516bc556eb1b507f78f69011fffd827f1890c11d358db89eb0676af4b16776dad7a8317a25cef89e72622941ff9cfd06b0f8d0479b148fe409e" },
                { "ast", "793e16e9032ea71f71dca149918f1120fc76f46d3f634ee520c27e1505cdf8a1db2e14ce0dba677888018b1992346b5826661d2350bc46bb5e27b99b07962c32" },
                { "be", "8f097a99325a043840235970e53301bad16a09e4d3cd9799a20338a244cb5486642325c7962b247e7787567b61554b73e044085596559c264513d345b0007074" },
                { "bg", "247774afca0600b8ce10934021bc8b02d21a06cd4bbed5dbb65a634d508ead309f081359a7e005af7e032e7aff22674fae0ef340accda1968ebb04073730ff77" },
                { "br", "2e5ff1d7efe5553b4d7407500963ffd214b570944787f53e7808ab9d99b206d865c1ce4e08bcc48a1f76f5149a3519050de61b98146ea7a075c9ffd544a34b55" },
                { "ca", "88a8ae2abf979d4ced2f93d5fe94b8e239dff3e94f806d5c80fde92f778d5de91f44f3338bf32c0731fa1eb3e9acfb97e5143eb6aeaa6d23f3cc69cd519c3b3b" },
                { "cak", "9dffc3f531620e6282bdadebd22d504406d974c80d85eaff4f773acbac0a6b7daaf6c48191e1cbb9af71fd04a0855407276af986e2dfea9b116ef79f98edc524" },
                { "cs", "9479927bcfcc5f469e1c00f88fc3b74b4aef02846ff2b4295c8f078e91b684a6ee09778cf1a4aa1545835d9297eb13280de58f6daa810255d60698d081aa6f1d" },
                { "cy", "09103ed5259209a6e2fe07487304c765dd055e5c2b56ea3effabbf05f228a78708e54c649d5dbc5084b9e404449b18ee96ad5c6ab05244e00acf3e6acaecdf3f" },
                { "da", "e684b39577777c1e7429c5d3c76ca5b3c710df40a48bc8245e7ea454da9d1cc017d75c9786f227012bf113c04e25c78ae305d13c1b81898280024d6bdc345301" },
                { "de", "2d01224c2b9b47491d5c0312319debd26828acf67b593e76e50bfa64ed9d405e748691c1887d4d4d0005a8e2550b8a9028f092f6bafa6c173775d00303a3ecf6" },
                { "dsb", "9673860456973da08ca339463f15036041312ae2e4eaf460363e357af67e5f0af0e98703752a02c2688baffb8bed628a2add2a44d4365202da2991e76adbad1d" },
                { "el", "17c70d587360da76acd5ea7b0c006a86b64ecb6ac4b3631977a80b96ddb7204760b8a788c6f86aa5aa534340f15bbd7feee043d0dafac61d9ed0d3568a9d30e9" },
                { "en-CA", "5a82b1786d7be8c0afad9f0c2db06541acd602e91b6b1eee0737a92e6e81de964bd7cbc9f14a18257490ca283a561cb1c43b6afcc116946db5ed9eebd5843c8d" },
                { "en-GB", "bbabadef7e0b10992cd9bbda70c3c53ca6c6598636572f4c3aa0f5166f132d0d22108cb30c386d4f5128118a7add8f0290d33f240342f30c433e09e197df8267" },
                { "en-US", "47f0fc2df17f18d0b6336130a0e75bfd4c87fe81a4a640fab158a06647fbdd122bd9b50244c756e194b7d1d1b40e372a542b2f984f962620c106835d67b2e5ab" },
                { "es-AR", "905fc2e064b1fa8c7c9b95363f0cf890a23d5bd92dd740c991966a98a8e1ad1729f55e7b02462fdd69129e1f1b091c0b96a18f72f6f85cec7c335a0434e7b491" },
                { "es-ES", "160b8c5c458a9c5e955898c8de573a4d1cbaa03a272c9424cd23b612535398f17233f8c482b259ba8f1f114cb77a872372e5d053331806746d73758a4bcd9a34" },
                { "et", "a1f06ff6cd10add325cc04d4281b07cc15bcdbeef528b5abdbf24722028097ceaae68f44a9405b7b4569e875dc7190a2291b3a83871b78c7585eefe0c79087df" },
                { "eu", "e23cf61de66abb2779a00c9c5c062280d44ed60fb787089b2c9bd59e1172dfa316c85d7f7e2a5608852b73dcf6d36fb8789de834415c89b740ca6d979227906a" },
                { "fi", "9fcfde0921c32c0c59c2712a0f783077d2f312a2c0a49088223b695e10eb6cfefb04bfe5e0cf55de25552f281d189d536520cab42fa16c3adbfc062770a86811" },
                { "fr", "f831d969014f8f92c8d4337dc559a2a62ef8710fb3cc0520c0613113b49150c5b3a21f1e628c7d699324bba4b853b9df482d14ccd13920b8f55423824d362d3a" },
                { "fy-NL", "dc2734b55b6cfd799804fdd5ed941667c8f129070ead8bb6610a9ab95259672f187d7fbc908fe9457b13dea7f1127ba1029eeeb106271ef8bf71f451a7a968dc" },
                { "ga-IE", "9908bd94c7d2f96bb8e57759d9111d18265c3f1fd79ee424d2b52485ad9b60ed7d4783d5882b2a580dd1cdf11a52e79b9b6ffd12ebe6e4d601d9daacee4774aa" },
                { "gd", "b19264ff6fc683ed9d00a8b739ddb316dc124e0b992dde59dc0ae8c234ed2239665c500a0c69cf6e5190deefd6f787d60dfafe2c39cd6ea5323221ccfefa44e1" },
                { "gl", "9c064b057349a1f8dae2cff27df5d8a8005770e274214ace60229fa442e90f5d45e0e55866490742a6ce4180e6f5754704037f1442e533084a3f0aeb0f36bd1b" },
                { "he", "543e19f407baff7c192c2a8abbac7d2c1a7b564ca4be57a12a2fbd12f8062f5634a457a75734b66d9334e76d5233757a5827bdab0005cbb029edb7650fc06c15" },
                { "hr", "fe7e7adfd1a54fc107fc32952e31e5c44832a5d339c8889fc4c93e9638b99c0dbb931f10a3eeee4c7de1b3f9f377ac1b9918af9423c9059b2e074b5dba45ab58" },
                { "hsb", "1a119f56102b6fe922427a86387077f29621c7b46180fa2804faeb9eb56ea26c95002469c56ebc4705352d4a32a11c9e8d91ff0e68e2726852912a8d1eac94a4" },
                { "hu", "7f4b4088a7f7600597d975f9116c4a3ffddb9100546d318da9af364458b61eee484d3f76767abbc5aa72024d4937cbab3f4e1a45dd79b39113691b73b5c2bf48" },
                { "hy-AM", "6db54171025efecd0da8bb7dc3da5627f2d20030ae19cd8b9b542f901f624da24b6f82607df15f49993532aa3fe960607c3f37f4452a280273e556ec0a3566f6" },
                { "id", "2df3808a98e8cb1653ee3c4734d0ffbabbdd07075a042a0b2875b1a99416ca6d3c99930a1f67f0172fc49f2252d1f4fa1f76410f65a4268ad7c1942e81dd0de6" },
                { "is", "ddf99eaf519f2e2df4da7b0dbf48e3d206d01e85b98073d729f112be54a31b9a73fd77a5cdf2cbf1869dd490fe194eeb03a6aa75285f6950578ba53345011e3d" },
                { "it", "4536b0292619afcfffe47cf19e972fd03ecad2522d2035864262a0d3fa876b5f500a86858571601eda9bec1e23f905966b8f716da968ab514fbe834c4641f96e" },
                { "ja", "7f2ded8c063923f2289a54abcbf00edb45c37734f3b5b6efa184e850383aa00e3016d3a8edc79749050d333df9cd13a0d73f9bf6fb97903aaf66c4b89cbde60f" },
                { "ka", "dc2a822fb351dc9a7e1a4993a778d0a1ca7ea42962a571283b7c95dc16de08eebb019952cb02b97258dc36b5d6c2a3c5cae3740f6b2f04c0da1111bf874b4881" },
                { "kab", "5de0ec782ba46948bf2e8b68e658bf8ad5585425d144711d0b4ef789cb73c338a49614254db2d6dc2c809e9bf0793a264e7799a68d5a3a87a54bc5912ea5cef6" },
                { "kk", "6aa72f795676ae4d5551047b38beafc7f66ad4089307679ed3eab5acc21f174958b1e1fff0e89b0ed7034f379d77714586be2f51da6e042f542cc98c1a19e757" },
                { "ko", "cc1960165d9e881dd46c7c5f606cc9f8a242e14f2dc224eb13f08f0963106efc895b387ca8ab964f7450849c41826a10e4b79505fdcf6eea02a62f8439253d20" },
                { "lt", "b52a8c8b9646d4d0898dcfb924c65c7544425f6cb6ad8a2cc44b2e16846d8aec5d41cbe49a970644b282deb7e9889e4f0494103dab3b6c34243bcae764204a01" },
                { "lv", "4b41e1820b68cded6f70d14027f960388d72a43db53a1d92e0276d77966f7d9bd76c6ca2098c179c77c1843acba03c7eabf0c12d6174b699fec6d8bd0456817c" },
                { "ms", "ff5b50553a893cba384bf0561c802996922cd1b9cf6f74b2c1f6a248c23005d29bb73aecbd7342948b8a7492b0f4ef055ff644b5e0c1fe5a5df4a89d98032060" },
                { "nb-NO", "ee6b12d294c90077eeb99bf31c7edd0a6a291ee22d4aab2ba58bbea50b6a38fe700ef2602ef4b5dff53cad5194e34eb417c06ed0be3bbc501fa0bf63248defd5" },
                { "nl", "3d46890a3641e08f1d4d25beaeb1ceab0949269e96bb6310e5d0da89ccc373d8cb10bf77cc18f66535fd119435904fbb682ea7a2e7d782eccab80287c70b77fc" },
                { "nn-NO", "8d86223facebbd0495b9ca42e5e499b1df09d81b17bceb19f01a2637187f2d96caaeb94977ebcd657439b78053e3eb36f43959bad44a6bf6a9cee55f4562e2dc" },
                { "pa-IN", "631853b4bc23d70417c4cbb4e288682d7fe1b10e2696809a51ebf180e215c9056c8aa031d6680cd9c3d543838208b064f89834d7db61523646bc6087a6e3b797" },
                { "pl", "593a47d581ebd0a52461e065970fa3fae67f5773744c15a637230d794fe9ddf48a41523da490d40668dfd4598a051ad84d3734af77364b3640a64c9b688768b7" },
                { "pt-BR", "5a5d6f4f90e9b7937be5467415fa69a10d3b706c878634880bcb166507505e219977e63ca1a84ad1283c9a4197adf5da3db07fc5968bffb9c6acd907943e927b" },
                { "pt-PT", "841f3249950dd65ee587ce028a26132607e7869f23a3ef28b059bb42e06faa180ba5cf276ca0743bb9bf6570b304be05718d17b9d6ec5f7e52b31bdb8a26114d" },
                { "rm", "89c5828eb910aa03d8ac531eeebf14da2dd9d03363f09751023c70df675dfeee8672baf0545e5aea6fba596b21b9059e3cdc4cac56d3a54f7a6676affae886ce" },
                { "ro", "7dd44109eb761545248ef50fac2e4284c33bae2d4e2b058e4a6b3bd0a5f1fd07b5b228fa33031dcc64658b0beb29f3390a2554af6b3dee087d39d85522d1d022" },
                { "ru", "86b885b3112eaa3fe84471b3e944028ea4f0a42392182ea944f6dd1dd3fe1b290c244ec69d07ce672bba3dbdf7c50a9478a3c855c1f2750395f7d49a5823b538" },
                { "sk", "b34b925dcf086078626f3e08fbf04bc8c7808fbccd38b6bf767f6eb4628adb9cf384c0b33682f511cd9457593b2e68ba7b1e5fd5d3331e50013de275cb835de2" },
                { "sl", "c38c22dcaa1118e1999f7b2c3cf9672a56bf618a3ff111ef845e21b5cfd0857254475ac5fe02f6551178344e913ba8ad9da7cbf076225b0149509902de0bc99b" },
                { "sq", "9043afb80a1d0eb7571135393ffdb0d26fdeeb02e932f7873429c23026a1e0050684d3de857845aff83e5dd6816fa42abb5c7282b036c13e72e952c68e0298c3" },
                { "sr", "d41ff6674432150fe1bee3234ca547c51f9abea04be24391efe9f6bee2883e973cf21cb61c85cad8b0b39361e8314a2784cfc0a2e35b89d63e5647e38d5390c4" },
                { "sv-SE", "c157a903901db48eddb044aee2f7460a700800c90fbc99c3a802ed8587457a458383dd4a53d8cbd9afbfd3239260c654d623c3d0a00b6a38e62408f6e9f29cdc" },
                { "th", "a331b3dd2ac179ffb2773d044b40b53be5a538f2de8c9c1071d029447786e84dda6aad506ef85ecbcb6dd4570b2ac6924cdac459b6bc5e93cf5d116a9e17cbef" },
                { "tr", "f3185d45ba2d4d1e35559ae1c526e573c7cbbe3c65c171fe8363f598553c27650c7de37ee414c53a4e8445fab461ef9438e51057409c9b23ffb9dc5496cf2b39" },
                { "uk", "7c6fef42a2b7c0504aa32b63a6807a792c724e9e1104f6aa1bd4a8166389d6f8079d0949b9f7ae8758a243d331fd0760859d5ae15315522feee84fe107bb34ed" },
                { "uz", "9a0734ee8e7063e6f672e50098e61ec39f953f1c6ae73fbb05a4dba42611d31f86a12f14cdafa0797727f5818c5b2e5e869694253d5564e3ec4b7e8cb73128ad" },
                { "vi", "c03e2be439628db24ed71de773f6ff4210deec2439fd963feea4da39476d6612ee7ac7a045936a79c1f6f8664bb7acf84c84de8d39ce5b969d5a42909fbe5406" },
                { "zh-CN", "abe9e4b12f1c072f861d6d7c5a5996c02f78cca57c28ed0e9369736fca4df792928fec44daf787bf4f09fdf702bea16d8d81e8553873a708c9c500a9f163641e" },
                { "zh-TW", "da6dba095912059cfe8dc9cb8f5a8817c9ce824c7ae676e29eb9b7e3919fe34310c04f14706bea19d2554a5c34a191bcfbabcef2e74e38c4b0c3550904a136c8" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/91.4.0/SHA512SUMS
            return new Dictionary<string, string>(65)
            {
                { "af", "bdee51801a151036ffad7c73ea59af02548c324565f5790376f19b18a5856e02391c294fb87bc34fc4fc46f31ae0e102d16c56428aaf81ee825c6a4395e9a536" },
                { "ar", "55887a77908e0695d75f53c8f94f9f27e23430f5d26df7f062c94d8ab46e45937cf4fc1a08611bb0087a49920f5fecc57f31f2bca7a908f564de118643fbbadc" },
                { "ast", "7275706cacf9314d13e1b6b7a12ccd1f455e9cc32f8ff7025eb5ec2d89f3577c5bb63aac3a8ba1b323df7e6b64b5175f525267387110adbeeda703a1e8956999" },
                { "be", "7930e5d190533049e87ee6f3679fd33a7a99c481bcf403b24980d1304f46a128f944f298378af7c31bb3361cc305c287570c3b894afd1d5f73cedebeb22d71b5" },
                { "bg", "d1d24a2bac6f98bf50ad63a2ed6bb48e40b3efcbf39b7aabbafc1ae57a27205f543574b59c8783da85f5b8d93a01f30375e569365fd43f039b7b6647ecf5ac9a" },
                { "br", "455e7733e2128f3ed16d988015eaf6585b8d9d6b2214cb1f430f3a50171896c99c75b0426848b0fd2ce75b590c9e8e682151070d890c8eb70fd23f952b799ae0" },
                { "ca", "acaafc905a38b0ed1b43077d28c1dd2c6fa6c16bdee3d355e1c2c506f666e42d174ca126c6f4be18f6954a93e2249f3f9454b62337ab4c30e69dacb95b6075ec" },
                { "cak", "96875efe2932924192fca82eeb72c3bc7deef5a36bab5d4afe70ed27b91f7003cc83940fb7a27ae6622c5a94b873c055e49220b5ac80b6e2d128c3214ee88215" },
                { "cs", "204bb9205b2c30af3fce7480d7c18c2ad4b55e21edaf74994855540e84a4f5c9781bd044c35e486d68aaca190099db7607438a6312d5702e392a4e39b323de4f" },
                { "cy", "6c7ed7a10974f670714198e635c280267229fb5b94efac875eb0b3777b7fb776d3e68603d68a10ccb56238722861e46d44df27181cb8d8214bb00ca10e0b3ca8" },
                { "da", "f3fca6c0b5a71c7e4f921a0e4a06c0f82d95bbf40394bedd3962210c3b85682e3ad50a111a18419212c05f545ad5b831b8a151ad7fbad322dea0a1777d258964" },
                { "de", "3c4b9499703c5d7eec0ae9e2bec8cc93a076588ec2be794468df0d434bcd4ba79ba63f37a6aaebfc9d375a62590bbac274855f724b22856e43b5add98d4afd47" },
                { "dsb", "7f23b12182927653bf832e2b114cb64968d02f2b5b04809b091ed9d0e77e6cdf9810688430da366a865c01c849ce5d9c21bea229252e20dcef29ed72fa73aeca" },
                { "el", "35476339d0ea7a87cc96352f3306898c025c66db343740ad74e194099ec89dc484d44acd80a6dfee1f9c231abdbd3d7f9f512b4d81302e341f294da07f2f1a70" },
                { "en-CA", "b3ec04baa741ef45906f5400bc46360a3bcfff0d1af9e4e8329b5f7314fc694b807570771785b5ed8a7d7c3d466491a1e1b609b1e4d052a005aa020f2a3e61aa" },
                { "en-GB", "3cbe97747e1ec8452f2ae0fba5357d9efa186e9d4335cc1f57250edcfdc62a19f03608d65633b54613e30814d800c6cf620665c4c00411acb218d0b2f90b447b" },
                { "en-US", "192b57887f8707260a400348269c6c49a20af722eddfb80079504ce44e4accd0286d5ea97af1283c61bdcc356c04d63518904a92b8b273f020709ccfb50e0592" },
                { "es-AR", "c337547c750f6d16ad0bd2d9c37a743a6a163071149782a1900c83df920bdba4d23b32b0b1c6795e2c6773a9c8c56ab4a83c53ef439559aa314a59b9ec0eea63" },
                { "es-ES", "4392b206ae29339f69e98fcb40af3ccc180c3c1585edf8d3ad545f716fc516efff2418f9694b45f9bf159ae173f5e449bfcf4f11d82b7ab91d21e4467671a02b" },
                { "et", "3840784b9542f3b076a954c7644642e58ab0f7d0a97b6a672670b5f684afc562a62fd62dcd1c1315c27fb44250426bd33dfa2bfca071d0d440756f555aea4c45" },
                { "eu", "6c1b0ff43cd5cf2b1072f20e8c16f9da79b51c34d948b9aec28637c59a94017fd1d23218fba92226815805ad24d2e0d922ad0fc9a849dfbfff0cc512b4033f7c" },
                { "fi", "57d4a42e64406907b9f0eaa2e3ce0c684323f07c04a8b4cd72927ac513c81726b938d4a5f29db50056da23d765d5a518c6a1b6e7a26b338f67d8886faf9e7e18" },
                { "fr", "99a416dc3791f1a96ead8ca0f2baae70f5f137d91d3a72d495a80b4b087be75594ee35bd893602733809f1a118d624e668eeb250533351a80ddbdb965d83a0d5" },
                { "fy-NL", "c19d29b966c56812f8e349b8d150f35824af7f801ab2424fb01b8395bb1bd2e8498fae6292b9e2cac39cad35bf82cc08eb2ee5ed99ab2c79015c357718b8bcdf" },
                { "ga-IE", "0b2b7f7b43b6294dc85134e55b824cd3dec3ffec5a25bc9647973d64537a23ac77f584828ed4777db6cc093db50acf9fc7553c063d07b9b4c1a34223c34365cf" },
                { "gd", "ebed21657020b6089de4d66ecb494d15b8179a68faec34859c106237c509ec1d9af403515b16ae8669a048d92e0bd27e8c00bde537e75aa03b8915eae51ce24a" },
                { "gl", "f2baebf17c176665ac3fe579f61d05ddf4ef1ae9f1093a9f674985728261adf76bd55e8ecdb5c431a276c04aa24921c67bd1c8b20e9783d805ad2f88d0e5be5c" },
                { "he", "c197d6ca5ec02819385583d6670549cba7965a1da475770d714fb809fb426de422fa821e88a1418add27eb8fa58ea7d85d8085f27264dbc2a296abc718dcb709" },
                { "hr", "acf97d0b1687ad2b88255092615dc91e2256e6d57d6491c94a6cd8838fa4e5717e617f9f0e17bc790af71b8396b7710cee7c5031720bca04c77c35bc81823993" },
                { "hsb", "2613d5773b2c626e19cc74bb7f1d2c706867ae3a7e8b5b5a31350c760f33d931f1ed960bb707889e092e4eec57bc6a36874f9f5477b14c304f7dd2342c7f6a14" },
                { "hu", "2dd39b2a2610877c8a86b7c0a40bce27a89fd270d4476a401994a4b0db9492ad847c9eae96ac87310b4ef9c7ebba042f8bdacb1867898d679a7e30b5d6901cb6" },
                { "hy-AM", "9065b7bfd83ba8615b2ce0459cdd8e0e79d399fc866fc9c4a718cee29ef3f12d3b6012b1d126d203eb7eedd2ba3511213d0270622af8e6bc4a9578aa66373964" },
                { "id", "420c2d9357c3a17c1314b484cd893a17830279a4b30cc735b74b923d67394c5732e77608c0ac300fdca5e5837289fd2981392ae7f70c3c220a7f595c107483bb" },
                { "is", "a46c0afd76bbebda219e818c736f893925131dfa04380dc4e3f1f78759a734e241682099e40a13b5854b1b5d0798e70732bb9b3a98dd19b9dfe663ca2bde4633" },
                { "it", "73ec88785803ad8b6f67506760899c6fb85690c4f5de6841fbab78012baf621c8570842c58344ae962fcd527be3f184c2d20152b422269d00ecc2afe3e8fa507" },
                { "ja", "52351ae8a17887d1e054511ee1476b10bb3c53fe1dfa6ee596a70ebbfec680853902482b236950c47b70b2c53bcc75e4f84ac28b82f81afc06b44de793d0fbba" },
                { "ka", "8f8607f3734ac4e3f9575da002bad4dd7ceec1dd1e49d228c8da1917d2c3ad201f3b04ebda12285cb250720d0ffd5f9734c92d6df3aa39625741bb81ac820526" },
                { "kab", "10f0da340cdc8a37cd766641e55ba61c4581bc1c1cc7f0c602b2c3e499b165f563be5248de3b471c716369c1b7017e46c8c1ba8e47bf585048020664a4c29cd1" },
                { "kk", "df835d3d46a4d48365164d1bce9bf4984f75c987ab87c87256a8aa33267ac80d0613dd030364b25eca3896c793132f7782ac4f049c364dde91ce25001f5fcd58" },
                { "ko", "7c25316f292ffc57c180660d96023dd7743cc7791aaffb83726607cd56e2294043f5d356f3c0b783e9f96366af02ed97e6a5569bc252c7572d8cd86634f8bf36" },
                { "lt", "e9d5b99d3eb2ecdb7340a37dcd12ebb13cb55999f2ac7d413fcfe57dd5ec53c7cedd33101290f0f19e2a9cc51ea1fb04ed0f735ad988fcbed6d335b491c243c6" },
                { "lv", "8bbfed221de282abf1774491a94fcb0a45d03bac17e7f7d55e0a47ff770407a73ab22efe2f381abf690a7a400bd9fcecfa7c98e9dc77592000642fcbb767c081" },
                { "ms", "8dab43203d75c798d8ffad82f3d651ea05d4413d195d141a79406daaacc065c3339d4ec07ddeabeff9e0f75ca0d64772993848fd8ca787306783140bd62c560e" },
                { "nb-NO", "4062f02769303825ac9bef9561713d53da4b3dab74c574faa11ae5b237b95b4bd487d2c2758012ac923fb039c1f41472c6232efaa4167ed378d11ac50052b7cc" },
                { "nl", "30f3fa355ad737a732fad3532f2146f077f07814fbd23633313854ac7e15c6728649c51d91950266e467e81a63f703a92ded7451a75d304593aeab2bbe1ccd45" },
                { "nn-NO", "4161cc6aff7e543fdf6f60edff8a4abf4731ee53bab8eb5de00a4e4e26ccb5704c1aa244290481c35bf480b5092c8611cd46c5463382c7016ce061a88d5eac4a" },
                { "pa-IN", "e74734370ab12d0c86b37117d51f654463eff34edf2e1509f98bceb44f65577839f664c6c9d0a00fd769c6c21408596fe44d3eb8faf9653d83549ea4160f5461" },
                { "pl", "ddf539f3734ac6f2ed05117ab6a928feda3243d6f7f8eac0adab18789f1b046158e8b970657f42657e36d486a74572d689656f8159b2a1534f435dd168e7c402" },
                { "pt-BR", "36e21a26c86be0a54c4e2fcfe03ebecc736e1abdb287727211a1cd9c4eb6023ece3795199aff3fa1ce5e18e7a14e1f006b0bc1fbdb7a3f6f437c941b987f5123" },
                { "pt-PT", "9504ed48b9d200f097952996f077cf2bb4b5a5df43c32b65801259b65038e442822a090ea2a4d75fe2530fa834702313425e290c1ea5720d7b5ce749399feba5" },
                { "rm", "c176799d1da9d7445a472a153931745ec66d5e4b63221befb4c24465be0dfc0b6e8d84b7712c683f6cb196ca3e86267f246f55f399130227fe4234e36ca86957" },
                { "ro", "3f09de9ec7ab3f527dcfd4be3a213e2ae2ae2b2f9141d7f74658f41239425b949490571c92183a66646bf5aed543b4a89074ff0b32ec0aed73d6ded46d1e6c26" },
                { "ru", "b9c0e67e61a0f74640973e36a79a8237e7bf2e2ec04b0b8fc42bae865f83c60bc77e9698e64018bd2c33502b494be22c9082ebbf1b5d94819597ffcd232c5778" },
                { "sk", "08879d2119eb108bd8ea7e4573496ab4c2105718fcc71ce2448dd5fca60eccc3b1e679a4a44ff2105b2354747a6a96d2f7faea72c397e1a63666d9a3021468a3" },
                { "sl", "5d6d216c97d0d1f3a3254b780359668cbd848aac2a22542fad23a47e9613db5c42ba144a436fb60ac0cfd4180ecd4faf7c031f0067c314c686b6480cbb719286" },
                { "sq", "f8c8fb17ff2f88cae52f90d76748bbc7ebad8cc9634d735a8c7af5f4bb1d0da39ac77f5248c12699ae82d358d95e86f9d1c3fbc5dc96f936576c058b2898bdd3" },
                { "sr", "1798e28966973a0f376b3e2e974175de617fa6c2028df3f159a2ecbf4e464aa72c4307dcb00cc553ec23c0b0f08dc98d54215548b96b15b0862ff7905c3d0d05" },
                { "sv-SE", "6e3d7847df97f1af25bd0d2086cfad05adc346097523d59abf124a793f30f5f3fd1959e60017e359364dfec87d4a3d9deea85ab29767f741c8453dfcea9a0b95" },
                { "th", "bc28a2c869e403dab453dc384a3a0e3b9e66cd0c58ffe674ad33deb31958c7f8f061a69e1dd2b0c9a3fe95d728e1adc35fe5b57d6ac51acd52c3f645c273c1b5" },
                { "tr", "2182302490008c5d3cc609886645cdb1ba7f7ced9ab827cc8e74aed19026bd8b65511517beddd758bae93a2bf05ac596c5e2311586303b97c2aac880a5527774" },
                { "uk", "0c78951e50d672523151a5830b5c1c399dd7cd104c1574ae56f0d0f68539697e50729afe938419ee02945da1828a87f869f5061aa3cf34953338e30b89c111e4" },
                { "uz", "12604b48eb67d8c14b6b1c088492f96acdd8869c8032b9151b0d14d16da852ed6806e94726e78997699adec37a0ae94bf03bae85fa7e50b8c60410ca750bffbc" },
                { "vi", "7add3926dc9a8e63daa651a97dba89a47e8fac7028c11f6e843274764e96905d4fb7f1c41369a1979d7cb3ae29ecd5bb007f37293543d86421dcf879ef6cf270" },
                { "zh-CN", "abba3b58301ef93ce4433bb93da56c90bc13fb50505574b520ffd9955f172aacd3d6e115068bb637d35a7200de03bebc2286da366537a30e2c4448fa8debe44e" },
                { "zh-TW", "7b16d43776d339c513e8acfa7eb999f9c308ffedbc16e699d43c9395be23ebf65952e8012c9ea3948591013edad1d870f1a4d72cf4bc5682ea53681d4a25ece1" }
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
            const string version = "91.4.0";
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30_000 ms / 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
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
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using
            // look for line with the correct language code and version
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value.Substring(0, 128),
                matchChecksum64Bit.Value.Substring(0, 128)
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
