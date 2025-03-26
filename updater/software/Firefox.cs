﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024, 2025  Dirk Stolle

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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German, "en-GB" for British English, "fr" for French, etc.</param>
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
            if (!d32.TryGetValue(languageCode, out checksum32Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.TryGetValue(languageCode, out checksum64Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/136.0.3/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "2cd0a33b9a67a4278f2dc786d919dd10b8ee044730e0b9f6e07c38094bd37ec77e82ae0e9ae81bd71fbe1d24b3aa171afea97b2e958c8d0ccd3b7388b151c7af" },
                { "af", "8346e844910352748d76077d259cdd460dd0a3393bf73d7bf3d3ae9c1466a9d3339f290cf0ec444ebacb721ccb88473d078c240846782166b417e76664e92852" },
                { "an", "839dec18f6265440912e7146fd66936a955eaf04cdea164edb820a543211ab8bd946d337840eb729d4b29db5d9365266979369b4a9b6c8b572cf370add4d1de5" },
                { "ar", "484285b72a2486e6a53eca248d93d51ee0492d20513f7da979f96ad4fb00989d71e013fc20fcdb1ffbde672a334aed928e547cc8b7ec43ba1e9844ef97d59038" },
                { "ast", "6ed1c3c972b698ab2d1de9cdc0f2a042754adcbc2b0b292f74730f04ea29e1cea8bbf26310ff84d7266feac3f88b02b64cfbd2c2249faad0082acb29f92c5249" },
                { "az", "944ba6f2728157d579628cfd70a8b3842ead8e2c19c70f355a5eedd571c7862407940e3d021727e94ee3ecf14d0d993446d62ff94a32c9df191fc0baad05b529" },
                { "be", "8c10ad1e797c2f00f412e19304ced8442bf7b1428b916654a587a10f3f0f017bca1c4d87a7068a5158433301d95759ea02161579af63dc158348563a621010fd" },
                { "bg", "5f5aa73a4a1c88d98705d4d1cea74179bd1bda6d4d35dc4b935cec4a1b3184fffe4db6ff7e4e7de7d0849515830d869167eebab72432bd217ad234febeed4224" },
                { "bn", "d71ae3d21de36e63fccb068b3899fdbf2436f067ed7b7c2d0e719f8cf2a10913143d5c455dc07c9c2a8c0babaf97b6cc602f4d7bf09a8e248de65578d76d95e8" },
                { "br", "405c57e7dcd7b327849547ea412f642c810be7aa1ce65a037f3a9607b8faf4abd40a9a30af1ee060c76827afc126828d9daa219b728ae9e52a204b9b705ba18e" },
                { "bs", "4e87d6a71389ccb57cdec4664194a789a1044f61732b6e024c2a5346162cd7aed2ad37d0975be0e7445bdb28898c23d6935a745b7936adbf2eb5dc8cf9698c84" },
                { "ca", "ca3dd8b762d646cc870fc88435ff895c8ac108968542cf003783292cac0929bc10a548e14c67348d90c99f67f68b96c23d170eb0cad00bac3fa35892da0938a8" },
                { "cak", "53581ee2674b2a7099d834b525c28ad04ac1cdc3fd5ffa838b980ac1161ff4031ca4ee6b96921a8d73e1161b7f68862d495ad250d6c6d3631a9404e3dc9ec32e" },
                { "cs", "9f51e3d7656a9602fa6381442397a92e8804b0f810c252014c3199c193fd2b996738655ec4ced187411a6f25e2d564e132082bdbc58a43f6b39829256e4c3b03" },
                { "cy", "0ae253ce5cbf384dc277f0af12090100cd003009c9038487fc0f503ea1446bb839603937c5d595f4af75fec6628b541a9088c517d15a8eb0bdc3c81bd87e734a" },
                { "da", "a1c5495b264c8b9d59fa30912e807e72ced3131473cdaa2ffaeebad8ee033157e0d40e10413712c83763ed4268990b1b244c292bcbe551c776c7306fdcf65e3d" },
                { "de", "0d7c7e4148d5911156735c618426d309d79b25be9cb814ab4b70d70dbee9ab04a827eb472e9710b57f67198b996a14e99987ac4253d8a75812647ac47c5792a7" },
                { "dsb", "64ff8a881a28714e771b164199970824c25fdc99dc4c452fc3b3af34cba09ff77d386557c97288f1c08803f3655659928ed82076047167caf81f7c0bbfdb4b6b" },
                { "el", "dadf67da195daf8bdc8aefe97b5d284a4a15138513ec8ed1cbe7f38c67b46e1a1ceacd2e4395d9b4580f2d3af2b51f76054bfac7a9d639aafa3bf77dd2d99d0a" },
                { "en-CA", "9d7615d998970704df7d761866aaafe8469cdb05cac2a8e689db09bf6cd3791eb89f99b631b8d0810bb7b49dcb6adb9b7f122f8f9e7e6df89c40e36ee0a5c7e1" },
                { "en-GB", "7df4df1cb815d925e946fc678e89eb1781716a11d9173d7c2a79a4ea78b0ca44a0e86bf2373960f51be6549613ec1798c051a6b5553a916b0717ac00d793765f" },
                { "en-US", "64882ec26dbaaf57cafc99c63d918cb8f25605e25129b81a52e6a513b267fbcbc9872b9e11c774ba410d226f2730d4ff6e2caffe80061bab0086c7b216ad6574" },
                { "eo", "c37b8929795548c00f84f9ed99299984c2f9db96b28638585ba0d1aa39db3b60ce478304f578173eaa52d8d57260331cf4ec613a5fe33c1067d14f72bdd2bd0e" },
                { "es-AR", "b162307f37d5cbb8c2c86a9281c4ffcc9fd1530e3b387641f68eafe85363225d45f1ed8acd471797885ca9c0bf9e930d62ac204e263715e568be7f7ba8f57e04" },
                { "es-CL", "7018c744d6e3a84f727bc76d3212b01103b992517fd5e5f7daf74fbe622b9c171cbac4d39abd23bbf87f4d73c28670e183ce851c74490854c6669f8fd98a7231" },
                { "es-ES", "e85e76f10e0cce906123882724c09f8512a8e14a8f31101f7fbacbc13b4c23ed7ff710bb0ccbfb884b7fc5be841c56309fc99891521cdb6692b2545e78768950" },
                { "es-MX", "d83d72c407a2d7162d855af4caf1f79e75af250fd18f63345f5c3986419d387fc15e3bf76393cd41c685973dedca3e6c17c57ae9fc928684b9143713036b7dbb" },
                { "et", "853bb0177b31c8a4e13ab6f947bfd57694418d44f3a62e98a4bd4699bd1c6c04d06edf299b62908cbc27e4acbbb3f8a7b57189d46e66c806a2808c47572485f5" },
                { "eu", "e55a6a5d9973a469e3a66e339bad248562a739b105ce622e22cb31896f6f55ea901f33cb791550de2cd6a26b591ebaecc7ac80dd133af8113273a68f216a03ed" },
                { "fa", "eeba7432a6d99035095ab6992356772f60e7d12a47b7bba921992f4e874ccc7d55f8e8a8c36b2392b1c53da33c92a40ed18671caff97cfaa954e2abd5e707be8" },
                { "ff", "d3f09a5fcb240cb718ec8caa72ee34b6a2ee3df03e60552f87d00416027c0dcdb84ed16062bc40e8017f851cfa308f0edae009508d824d0c0b2b966e4455bf3c" },
                { "fi", "96e1ff3335fcc05d61d338e28390d97655992477d397ed69cc10c50e16013be1a8d2dd7492319d5cd61e5cdb972070cacb06792c46c7fc70e5a498cef9d2588f" },
                { "fr", "cd0f4e7e083e0720cbd5a2dcff31e915d85b57c96c1557a88691ece2991fbefa78fcbce80b296392ad63a2c754ef86a0d56ddf2592abfbb2bee7280c96fc5064" },
                { "fur", "d4fba8d78053e5ddbb62af338a933e4133ec18b1b49139acccd0bd1a45a0029bebd67ae9c47abf5c11bec38624277409b19e9bdbdf085c68a61b88e972632685" },
                { "fy-NL", "147664aa8a69a620f9f4c3b554fcbb4fd4f308385d499334d7c3e04fe5228e54a36ca283ea6d629891f5f8323a353480fe1d61dbe21dfa3ea3fc8ae52fc7b06e" },
                { "ga-IE", "5dcd67896237e0c4fd4d69a378c5130bb690b14411fa43d75897a2b2ecfb3f165efccb98939c4f661602ad75f42e637e73878c41c4018b2b02b20a37fdfa9084" },
                { "gd", "bf754fca79a923e4b79c7e75b72c5d3490592dd27537df6b843368bcd43548a45d7a6f81c12dd060dfc8e6df1620cbcc8016fbee46f31444816fedcdab408662" },
                { "gl", "fce69d8edee8a4253421342df59a03f16f539c1a5ac4be92af8c3cd97d5d56d7b425783e9f2232df13fda6792a1e2c80eb58c63bfb7eafe003d1ed2f9ff19982" },
                { "gn", "49a05230e4d812aa8ad23898f5f4950b3f28e630030c81e287cdb0912a1dd8866ff9b3dd657ee12faa81e24450b5d08625f6de67645e2c13845826ac02e5c5fa" },
                { "gu-IN", "807e995bc4febbea2b402ce39012be151218a334b20d9030d63e6ae0114224ce78370f8f4110e1cc3f9d9b845393e5d58740e411822de546cc27df4577d2038c" },
                { "he", "d8534dc7045ee06f44dd7b0cf3a9c88216b78ee460f05b4e7ef00ae508e7a05db57275cb889a036190636682201b2f0bfb82758d1514adbd6dea9a455989ebad" },
                { "hi-IN", "77c8747933f55482f1c6d7583d364595a550825ced18cfdac663ba95afcbf35dcaea38d02711e8d2df0b42b5ff504129d6be3aa526339f0f652725e5e2bfb431" },
                { "hr", "71b64bf453c044a9a93e278d92ea0ed08ae5d14e179f0a9fb82a65eca284cc0604aa8376ef70d1f4166397d6aea92f860cfa67a8d0f2f65e9cead993c9ff555c" },
                { "hsb", "7efc625376137d84772543d368873c1388b9725b609c6a2505be8927141bac5b0a8c9c109243713fe55405ec77244a93cd674e6f6ca28278ad1785ad2fc0f76b" },
                { "hu", "522e21c27a9b3eb42ab83a461734f92f7618797a1c0068c6917da25c7c08bd2ab2cc429acc04af9a7050d8fa50cb49e25dfa724591df7908c5b78353476cb20b" },
                { "hy-AM", "215db8b4f6a02682f0415a582378844a01f7271af496cab5dc01fbf6d67efb53eed6f25103641b22047bcc2072635eae84caf638da89ea4b656045af6ed074f8" },
                { "ia", "aa98e7e39a6a016c721e1682bcc960a0651444d5ffc7e2e18b3ba9fdb22afff740c7792303a3234ac2ffc9af80f95698fb0bbb77c6848b94bce0be2462389d68" },
                { "id", "96eec661d245a4ef1886e57862f862b0684635dc426152e2d4cf821bbcea6ed1c621c12182f786194e1d6eda8aa516b581f2655876f7668039761a0093cac5dc" },
                { "is", "5ac6a55444eb4affc5d48b7ffa62933ea4cde3eff6aa1eccef6d29feec8c2ed040ae2210f9dfb80a1814c22c02fb2cb2d86e702b357d6fa5a85d08a61dd97ab9" },
                { "it", "ca40ae95884365cb9f2bca94e69e8b9489f89a19f67537fb7d7165f02c2449332806a010715b1e1628d173db1838c0d1520ce489658cce7f506a3f98116dadf9" },
                { "ja", "0ff24352894411c384415c296723a6ba91ab0fdab1b1b7a8ae6f413c70fcb8ba117d941923ecd90fed5e871bedea5a2ab3f34db73afd79aa50269f77babc985a" },
                { "ka", "06fd2332ae5a95cf961b1ffc77de005756f5d687ea8c3b53f5778398f11a249f9c7260adc0f907bd36811fd4ff2d9ff98066f3805fd5270618eaca529294d519" },
                { "kab", "12848d5f75da86fb5a6e810d137c1a6fa4363e24698770759e57dc30fa70536d32d3c0061a51d09b7e1494870339ee1b7bc133bf20e6297a06cab2502f766e38" },
                { "kk", "3c8b07855366c0b1ef5cf0f6c1064df6d09b39f51af4875eddf43b1df3e7b5c622ccd560c9d275701a761d5ca74322c2ab31efa9e2f7ee29d73f03d6d7ad42f1" },
                { "km", "db3206dbb12b61200a9f34354cba0c90a9e3ac2cbe57e8b7acc882ab098e865bd354af3c52b4b16739c75394487d0dead1b81e7abc152da474c1208dd9e0033b" },
                { "kn", "ce00ac8dadc09761af47bdef9f705deed13b14f01a28ad465884fc42bf93ee133a20f05ae0c42e6bc2f75e0a44ed462e7f6abf0b857cc4668008ee43f813b96b" },
                { "ko", "5a242a1d3ae2254b73c4f0990bd28ad5a49d34ecdcbab2c81c4f216b773cb676cef3799891e6bf14735fa5efd3a0523378caf27862f561243050ec0aee7ec3c6" },
                { "lij", "f4fa7c1fe98706752b678a00b47969573aeed3da7be8737f04338ab8dfa967687e6271ad0b29876e29451eb721a68fc054826f2e39f1323cad32ead42e8d1759" },
                { "lt", "a622cc1d2e8b1e84e8d7743bbe624a7f8b1fd2af3ce162fc836f803f6c03b34f346721b7819e54490b88c2b5c46191b6174a0a5606188c20931057fcc263a6e8" },
                { "lv", "3d99512e3882326d4e61eff07ef9ac9ce49ccd458fdb9df9224f16c5b31a3028de1bbf72e470dfe697f5c50203149c185de8ada41f576021df27f9cdba43fb2d" },
                { "mk", "eceb50ea4e712093da3ce4b934e60e50c09931c3ce7d8ca47222bc3933a09c6ba9ff8a4b8ee765a232f44fc54f39e4f6773ddc4c08af9753465c60a229c0da70" },
                { "mr", "22f4ed46a94cad67213c8fb579dba27b869f56770bcb5f64666864b408b2312ed987c157df7abb07077ffaca10fb25ef805ebd47c2dc13ff9266210156b44e4a" },
                { "ms", "a48cae0df110dd14b873240169a739fb0506e8e48873da5dc6583798ae4008760d5a6a8df76228a74efb01f209f94a2602275135ccb6f31d667d77e9f32f831e" },
                { "my", "fee41784c44df8772f49b20f5fb7ec8c0ffe0f27a67c3e98cfb1ab2d4d6509ceb72acaf5b1a79a39079db16394ec7597fe5fd84161fc0e9d54529e6c27920527" },
                { "nb-NO", "14e21f4ba1df7c6ac61948779418fb85da9e42bff06a7b804ce79d8600a9fa39a065f0a005b43a5a70157646b8e39fc10fa2da3d86f4272c127f34ff5d18575c" },
                { "ne-NP", "5027f3260c62d1eb12ff67d3dc2533c8a7d0df8c58263d44aaa7e1c44e84aa29601424dd0b679afda8e282c07622fd0359d1907f9879c65fdbb6570525c4872b" },
                { "nl", "0adf8770f43f8e5fd9f4c61068dd5056cc3640dd91bd1074532bb30cbc1949668e47cffc471741ee8ab1d4c1a36fc13a9e103099388d83ec9e8f08c9a31e5757" },
                { "nn-NO", "cb7e67ec95bbd7383d7c935d24be576eb57e6fa4d15ef20da2a2120042b64701d8a1e0f1b914de1ea1e8eb87af6d8795e9e9ee9e12e1b5e55efc3562e64c0d4c" },
                { "oc", "62530de22fd0157119997832a6a1924d3ea0b14bd70b49171b6346ae68f84b81e2458ddb1023d05b0de3352221452be108cc2d8737bd35b1410cc991cb0d0138" },
                { "pa-IN", "dd54cad85721bee132c45ac0f439a2c1df0b9c0e8efb67e4066e75af34bbaa3e60a31d652b70fb13ad0be257f6217afb02fc9d99c61e15c76b4408d2d0fa7595" },
                { "pl", "fcfd125031112e8b73f69c82272537adb59d08466c4b591516c00007b29fa7c1ae575a2e2e33fb29517eb6e0b3eeb3579440c1e0b5183ea524e697b59c8469a7" },
                { "pt-BR", "93cd17a97afb788bcf487300293404553fe6e50011c3f5d51edc5085fab4e02d0c65e524c0369b9828cb876946e5878437347d4f2bd9fbd58b6c7f1f34714e86" },
                { "pt-PT", "5e2199edb9df8fe9499e4b53c2f141db1d53c59de7f45b4fc17a5522429a9577f974f2b271760d58738e71451623919378b6b13e54ad0b17e9ec02925fe39561" },
                { "rm", "3517ccbc4c8b3a59910516615d7f2dd8bbbe1792e4b1c6ed0abafa39d02e4f5c1933856bf79124a8eb0c5d6c9b3dd7fa59458d5723f18025b866f6cd6d9dd8a8" },
                { "ro", "cd3369c69a1e3e3de318f8d6cb14e745d9d2b8fa8365c963e8865ada6ab87398d29c15aa830039d3e4f99615a09750f025a3f40c6ba4c7026d041fdbff654228" },
                { "ru", "15bed9834bf0722fdf0c99f4a287ac545732940b54b63aa716dbe15e795de904e29c659387d902931a1cab83e3c4b754116f3428a1dade828047655852e99f6d" },
                { "sat", "b7b0ca4ae26591c781168f38f7af8089bdc4a048a51538cb0f4563e90117b6480c78d8a4a11873e91abbc076bf973107ed85faaafbfc93659c43a4ad0790f93a" },
                { "sc", "4f67e473667eace149768324fc3894ab3c2263a21833faa3ef91a92ad06f8ef126b1a1819079dc7de5ef97890c7f824b4273ee6979ba349fa5c02149ba089565" },
                { "sco", "0038f535811f2202226c25390b74a560198bff49459954534fe47d7c20fa46280547e95039b7af4ef0ff648596ef8a2516ec1aee4e2ab8d511735b2efc52cae8" },
                { "si", "e2bad2f286a9244e22346c401899e5409d6be6c2311b13b236209e4c909a6f627f1d0349ed4618ad8fdba3e4a0ffa473fd25b96a8bd40795daa7b5522a196911" },
                { "sk", "48a7a3de84dc6b242220591bc3e7d5a2d52d974a28a67d2242783fdd603ac6ed2848fb964f0de3c332b92073ed63c7d89458ccff9b2a5163f92d646ffea46b85" },
                { "skr", "20c393a5596eb639a695e7e7c903ce4a862ea3f4ca2144beb027181c5a1c348101d7c306e4cb21f1f4f7caed8c2390167d291402dc09de22713637d59a2712f8" },
                { "sl", "3b3da57195d271f8500a61def0e643f2d3a3c59154c3114e7c7503440865d3c945f4655c5115b3aa1ebcf45ad7c01d31819c006b3295a2e5d76c68d630c7b21f" },
                { "son", "8c93784cbc258a8cad4ab2027e145c00a848585575f2c98485a39e7c02bfa3c4d23d6d1c4db6095c02b53f1870982e900f58f4b866ede3401d20a22676c0b9ca" },
                { "sq", "96c5dfb40a2fc6f49fc0db5c073a6da96341856650b4e5d93fd88d6dbb2399ac4fe414cda2a32d1db12d1039ad2ce9d7928986e4edb6625912aee107ee991a5b" },
                { "sr", "e660c98cc83389e488b58ffb3c6dfa3362521d7f3314c10c639314960a1e6e7eabad12ce789deec0657492a0d87693c504b5d9e24a02b3f58978868b29d1abec" },
                { "sv-SE", "7e29b47f648bf53a9c5a78afb21e0e61b3b5fe24740457694d5f884bb1fdb5d0bf02c45252864ec34946a8adee3b235e31c5acd0c01fb7612d46363aeec1d5d0" },
                { "szl", "14f5240421ff40862a1fb3be1e41f7e7a73838a66eba6288aa3220881e60b79952a72e0cc31caf928b6fd89d483048f4fb319adaf63a59992b5c175de4630c6e" },
                { "ta", "27d893699200973ac5cba8ef7e23f5dae206c3956eb4d601a09bbfa0e08ceaa2935fae9bf256582700f9f052bef43c2a11fc3567fc03f8839b42774d0d202f31" },
                { "te", "353b316f59775196fc82c14a0d13a8830ef2e4a2330ddaa30371ef823115bf31e6d82febf59dd024756f4de5ceeb2b9692fd1ca1e41b68de946510d5eca895ab" },
                { "tg", "eb3d975218cc6fb37dac43fb948e5f9d171e40f4ea5f30b0c5bb5a534507d5f4169dcb8eab95c4c77757035b149a0c3a41bfdd7387fb56ee008220ca5c570f4d" },
                { "th", "90ff5ad12832c988cf5fab5096d3dab069c253bd19feba5e48852db88ecc683de1d29171cc41eb2350d3034f87fefc3f6f3627a544c96cc66435d4cf129f2d2d" },
                { "tl", "1f9c16cbd4a62b67c0daf0462b0cc74c2ce4d5c6dbf1668cb17cc33d82d49c5a7e97dd1c5e9e013f7e453e386e6ddb00335d09a3654619a39c425e848d286a55" },
                { "tr", "9c86bef615fc8dc896a94e2ded07b9173a5425f4db2f925efb7eb4507760c026cf6e3f9492feee0ce2717d2d697dd15ad06c0fcee12e4fe951b19ccd5b447e9d" },
                { "trs", "18b3a302d64a9231cf9a0e45abb88e067f50bccdb7ae8f59943940e284a7a2364d57a14b204b6101c799484b88d33bf0f686a0be08fa803067f5bf8feb580454" },
                { "uk", "38776da3834b920629968287b7945ad379738e130cc59477807c19ca2590de5f6e4fc8e623579cd7dbd6cf4e4da4c9a77997a71b286a786762864a8a9db777f2" },
                { "ur", "5b7a7f759d8b7e400e27501792fa10c934051dbe16cb09318e69e0f92fbad66cbbdca3da5be867c68c264ce487df8fd2257a3ee52e977d8927c2708d0a4b8f46" },
                { "uz", "8b5cae324e2c2b91a2fee15a4708afb0ec02f4a6880169d4cc5a7beaf793e25c54b050b8366c346b97041ec66c98bcf64e94c55a8826c461851154923e696aa3" },
                { "vi", "6cd611f649067fd85109d706a693b864fe9b246854df3935e9f716201bc3a4089e7bd43913ba1c8091451885e0885d7d09a3f4f7be2582df533e2c4665b59bc5" },
                { "xh", "0d87ffc3d318ad2a3c46eab3f5733214ce8ec90f46033ebe479c473a87fa05c84b6644e8117007f1aaba92e712b46472125d44d408cdc1588bcbbeafbbdeb33f" },
                { "zh-CN", "63d14cd144e4412f797a4b8456e9c3d820470cbb71f9eb54822b56f85370f1402bec0e76b7a17c0ee96f71fe3bbc2bef18a347a67fbcf0943c3ae6292ff55f59" },
                { "zh-TW", "1b2a080bb8947bada6eb5ff06d200832aa90b6c6d728d1887c22d79d9bb1f760720de1255ed1c1b81fdda0ec73ff03170fb04854426fdbdbb846d83c34043a77" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/136.0.3/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "f50139ac35f2e455207628867719e9853b535ffdff0945a7e137e365c516b70b8d884d997b7970dc2d7225b762b51be91409deb33a2d1bcee3507253715e214b" },
                { "af", "90f7d3731c7639b7514f70f65d16f35af76faa82932a0e3bf621486863c7111b04bfdc2b697b721da8bc285eab6d43b2ac15b91f880f0312ccc6d9403c7a40fa" },
                { "an", "d1f0dbeca20fce42f0330778db5bd41b0a5f41af605b811655e2af9f88b5e65ed484ceb002e959768036de619ca25f551bb109ac64d322848b7d1dc9fe5b36c0" },
                { "ar", "c7612701afd4d9f6cb2ef37bf42d2359dfb48c01ad5da2f5f3f9e738dd09498741d738eff48727def89e4fe50647cd01e4a5ba9798242fa6469913df9a6169d6" },
                { "ast", "1a77b569179416c63a0af07688ded7e6a6604e695a072ca472de81f790060be8b0bdcfd6035127503c74116bce3a6142b6e3e897852eff6d24bd75404af6098c" },
                { "az", "f8c2b5b60776a429e94e516a06a7d586b945056c8ad76444e2569a6e4a397d88fc651ad16dfe1d8912a8b11ddf36384ad44186857c61ac4b3b7354e5463abe7a" },
                { "be", "1df7c61a8bc613ffeb48be399ae3a471df16b43dc74f3e4e18dc74a4005a3f44be01522e3ead85efffbb803f73bcefd2a95492b789c4e52709b0d8dd54b52db3" },
                { "bg", "6d2db39040154af68b8e7f67c5a343bde2f180f68821b5deff82474b099dfc72e3cd8c62967b6d17292bd50c1c128e86eca328615a89f48e3a3b02ab2aa4f52f" },
                { "bn", "9a243aa0c6e95d970c28f2640d142d58f8f1331e5d8366dce60c81a5755a5b7b2316ca545f4198bf736ce7bcf5f9f2144c0250cfa0469b5d380b7990a37ba475" },
                { "br", "dee7ee646544d884a4893c6ad77cf55a52440955d6a2c963f1df53a5845d6954dd580f56c37bd5f67ac18e697fc7e11539d489378c38e910b9795e7e4e1f2de0" },
                { "bs", "68c07bf153ff4fff855d94d130ab9e71807efee8e79a5593ed54cfab1b506db9f8e84966bebd8dbf45698a63950b0db59b006a9c64c5f588dc096b6f73a2f6a8" },
                { "ca", "34e1f1d693aae5b978b25834cdc9a9502483f6e2a1a00c8793d34196e24f055f5e77224afeec90286622224bb9ef63c9d5bf91f0cd9a4fbcf1fe4704d7a70d4f" },
                { "cak", "dd7d1dffd3e3ce5a1b786823e425b212904fa89425130938e547df55b2d4c86abe55d8346f1ac71f5407d0772a974385e4276d2cfe8b2314b1b18c605b177c2c" },
                { "cs", "b74c9c6a8b35ec4ae00d792e91a4c4ff919c0f529e910cee86686971b29171c99f5d6b1048c71d02fe89091757b54c5be3e118a9ea8fbe9c85c761e9a3253823" },
                { "cy", "1f6939f3492364e52ac4dfacaf01a5dece3fe4c82903e2f2290112e53071e45e52df351ec836a12b9d288f183b0072281e2feadc6484298e23f724e874e8f570" },
                { "da", "d12aeebbc5408c652c17c7144f8c2aa8b01b20095bddd22f789bcbe044fe16519bcdf637cfb7f259393354eda30c0eca90a6f8ec606d03562736e3f9574d69ac" },
                { "de", "18f204b0e99c0e64703cbad5e1b5228de62abdc3e9b714561bd07333d3fd04140a2bca039d43e12786368cc570369d23d8089fc4277eedac763a31fc76af9afb" },
                { "dsb", "fa4c70a2b6b0a8b319c54cc11848467e51a59149c726e69f8da1d2117c385a66308d4e0a2196a553f9e943f40f03fad8aea1ad2e0dbeb73e2b013dc0efba0e8d" },
                { "el", "6164354f0b848c0043b163d99931c2d709ed676cee579319b0464c58c4afcbbed47b116fdb8c83761a39dee4cc50f2582f397405a28253147d534eae392d66c4" },
                { "en-CA", "c2ba1c1e674cb34a27c3dfea9913fadfdd8285157a96a9af6799739449337caa3596f9263778203bae4b5cb3c019d5d3842d2954b99e06a51099784e5a203e25" },
                { "en-GB", "8def751df069ecc431cfe2cdfe507f2c4f5e34f2ed89d967a9d0a048c42508df0675b8e20b86475e215c115a85fca1ec7c21d8ef8d1efbbe5bfbaa34e93755c1" },
                { "en-US", "aa6a36234915c817b16618509d3d707bb36239bb510ac7a9659d14e02d2d483c73e1a6ac86426d2148df76ba9e3515eed4e72bfc3bee29b4361ce2faf36c168e" },
                { "eo", "8f8c81092d08744cfd9e4c09d8c54dfe789822f1e8da45128f1b568411bf65ad76f2a6496f425ccbccbcf944198ed6a36f997c4adea6d967d9731bbf4561e33c" },
                { "es-AR", "5ee07534208abf6c8f1d79a76126d9c969d326af99f443d9998d48cb4ea3b0b879663e0ced7e1c7605347530451ecc285b75ec7f94471194ea5629d9d81f61c2" },
                { "es-CL", "437ea0214b66ea485eaed3bc788812aac9fdcf53a05f078d313fe4fff6bcf0708f20df092590360dec6aa6dd6b8c032fe4f7bd6d6155027c38c83163ddb61841" },
                { "es-ES", "e9dfd37bf86a01df8ceabe857fe55430da72563fe037db259e8a916870ce748762fd09a59fb6e0d283ce7610fc49b1e1ccf3d0cc55761da4abde4562c450697f" },
                { "es-MX", "0d63f31e7058cd4092a6ea5cf99315a59b79ae50fb579017b98b79cda59004528f8c808774ba20e341e1c9ee21ed8e1be7abdb2b1cb8e1b63ad33b80fe3ffaaa" },
                { "et", "5b77400358e583a72aaa483e70fbbf09d46c325004e4790f7f4c299d2ce00b310bbd6196da2ad25a5406b769666d5803c954027313e4589e2a4248088f0599ef" },
                { "eu", "b7550b6059b01b992f349ac786182ea0c8c10c5c90e55a87c9a3ba0d6809ba968f261505a61f226fa9cce766e3619c8cdb88a005d11a31419dd50e23b629c307" },
                { "fa", "da49d9156ee6ec2e5d4050f4dbbef464a024f55e6a0214ef14ca1906583eb61172d6a40161eb1e54d58faa69e1b196b4a5b6784a518a6ecdb1e121973e59f1bc" },
                { "ff", "cea95949b2bdf3b2bc0900c1cf8a89ef25a42eb083f38e13f5135e99c0fe43e7e9055e5aef1d068fbaf4befe508450b27a61e617c21ec8d52fa6b2148d6f43af" },
                { "fi", "9e32efab8da20ff87437b31082ad0c4fbb0b102273d0ec320bd7e3c80ffb9420211901b003ea2fc4ac053c63cb9f5c1b5ea152d3deeb0f48ab906e245a28d910" },
                { "fr", "2c9ee1214401e4e1a3a447de0ebdedf0195a46b6a4c5583b3ef158528c0ae173b50cbeac9017bf920b8e75e83eee48342fe462e6be18deff5ab08a546861666f" },
                { "fur", "59bac234d6bfee648918eb3eabe891a74d592d242ea732ba001d03a2e88f7a4add7f3ccce205ffa68f2ffb2bfccade9b861cdfffbabdff7bc50665814fa925da" },
                { "fy-NL", "e8bce03a3f9f21ab817673bd38de2a37c14732483ab73a6497bce9f8160078dc851a9481e2b51d001965818e2530b8b937751020760072e7d4a929afd743eb0c" },
                { "ga-IE", "e6e747279631a9e102bdc0c8a5a7a530bcc6a2c08f84743c174b5f672671582839d5415b9d1e199b2bd482d3d77542168d26b3dc536bdbd866a453f363ab38a9" },
                { "gd", "56139886b0abed8de1b439837dda6df6d178a98cc808797d8925d095aa60ac1024b518ebd8ed3327dbd4c666c2460d542711a2d47d96a09c450e1b9d5e8b5b97" },
                { "gl", "c69fd0462c10595c26d8f14411cd67b989f19013b66494a1403a3c30045260eb1f23b00feb02e4fcab04ca6128143766337ddffafd56c0a05a52569f7d5ccd5a" },
                { "gn", "34957e48483f1d0cf27480ecb96feffa420dae7f20378880d45b7474795caa300374424926f21d6da842018df913e15181e18c469f7d7eedf8bc313d3df83e72" },
                { "gu-IN", "df726bbe391fff2342bc1be3779a52515a20dd9f2affb1a12a220fefd769c1d6f71d0df7f1a289fc778f4de8a42291f0b4b2bd1ed1dcee8d06e7ad62f6f562cd" },
                { "he", "f496087f7beb8b6659013b7010bb34ebf3a243eedfae047f5085cd7b7416beeae9057aa87a2f4b93e6eeae6623445072436743b5ef97195ca9a66513110a7e68" },
                { "hi-IN", "36ad5268d89711ca255e837ed40704717bbb2bef893a87ca6948e0101b0aa55251555bf67fa2e0d7f326fda294414bb41c3748c98730a1fe15312ebc84e8292f" },
                { "hr", "f38a4f783af8e1bc1d7911f62be79dc4a20dad35ff121235a4d11748270e1c2e8cac328ae0462a0e560dc0d81933969d49372b78d73dcc0020dbf8628a1bf9cc" },
                { "hsb", "3913c37707fa2b5f569d62aceb4a508e1d4f80878a2c2ec3f4fba018c01311b65a2642c5fbf96908f52b7799fa0ae31b5f19374e38c071235b1dc79356977753" },
                { "hu", "367e8530325b2128a7f0a277caadb24610c8e852a4f59613b5da3262f2c4e831e6793af659ed511578b7669598e04e5396ee3d8bb998649a39aab30b870866af" },
                { "hy-AM", "2974f96d82b1b842de44a729b9d0d4a01f5b3ee04aa1e6d02ad7ab3642a061248c3855473a5b939330227b402c93a2dc5406082ae5c06e5f5e29f0a436a82eef" },
                { "ia", "83f6f4ce85b832529b2ac048e17a05833cdde94fe46cf37864cfcd15fd957463305094d60a33469ff35a18dde107d6cd8c740c8b83d903edb470d334ba4b97de" },
                { "id", "17eb5ea36660d0ba5a074d1f15c58226555985a88bd5158306321fc3ed729c969e4d97a79abf69d7e43e126ab437c92b35ac970357d93a6fb18ac484ca8342d0" },
                { "is", "0ffec733d66b177d2be4d12dd42bded2efe6f81cc79debadca0a0306441ef369df67ba42b5ac78dad4ff11f0ba935f1f4a9b8717e2b16ba1e1e77770f331b0a0" },
                { "it", "55f68549123c739602801563da27343845704a6f0a08127d39c7523a6fc9185dab7ca70f4b733edcd13486b3030507fbcb786f09d97b26000825396218c52cd5" },
                { "ja", "f948a7e94ffd29e5f78461e1eee1ce9f033e61d83338adeca3ec94f27f22e8b0d963b979a0b7c1c7a896c5882cacdb6a7302f2e4fa307901c619b85e9a9e0bec" },
                { "ka", "63102c34b84556d2afa6fa8c8cb9b2e5e99789f1f6fa847b9ee99a5ca1caa25f2ee25c377d3d90cd946830630c76fcf64d776d86b60e00dd5fb2d8b40761c817" },
                { "kab", "3c9937de1fbc4a87baa21a61c15637b739a8487c3320c8083f70fd9de3feb39851cce833edf158d9c300dd01a4460b80182550f73917f7a97a37f709a84934fb" },
                { "kk", "af74b9bf4a8ecc4a2a6da7f4a9e22412aa20810ce3b455415dfe4c8669ccaa8ea7a10e0eee0c5948338e9fd22d1a6eee23abd38a573842624eb4c21918b31f57" },
                { "km", "610f1180ac54bd78ad975f6e6a63b64e23807971236fe0f1938d71bde6ec50d99228a7a2c7db8529918f61c8a869e79a29b0548e7bfd1a95713880d8a048740f" },
                { "kn", "95da6177cb40ef1d8dd553840db3de7ba216a95b63f22068549540e99950e2c6c273f74a534a6833654d7d5bcf755286e82f4386ce6f86e9bb9c2c6c486d0210" },
                { "ko", "a5a9a36f609a37e4d49b54780a7ea295984e1ef67dbd190d43cc016d82cf499468ba52a1f4409b5dec81d59a5168c56daa67856f636e38b1c1ec504889cd3f81" },
                { "lij", "04d0ee2c40c7318bf1409e7e5653f1421e36dd66a5916b4d90b2f501c431259e04fd9cdf3c65f0ae2d80331b5a356e1dbf0000473edd874a1d53ed5ab85748cd" },
                { "lt", "31826eed7aca5ca65c141b2fd9838be61f183f71dd8fb8509c31835c8a60a7995973f93993994caa63b062511777159c7bbf34f40344d889e3cef67a1134175e" },
                { "lv", "3d0ca5da555dc97251f5db47560b58f2143d1e38871b33eb9c43592512a0d73425665c02878cb424c44c06db6b090b46be0b7474fadf0f4fdbc0ec990cd6861d" },
                { "mk", "65d85d810405987062301fd814d910cfb8cc32a91968754045bb57272462223b07b6359e5203122b5c2faccca514ad499e0bcbf00c39e494252ac1c95f74fc24" },
                { "mr", "5540ca714702c0d57d6f852cc8fef7c62c030316eb06bc105580d3ff455342734fbd387a15d9527b37bed19b9118e280786bbdfcef4379c8daa079d2a3dac537" },
                { "ms", "38349cd60f6910597319e33dc39e7f18f724e01ed8d2067b342db0eb677093e939e24ab36e8a6113f87e395d9e44201d2bbb326ed5aeebd0be02a49c156db7c1" },
                { "my", "d1aed7f53c0e76d60e41780d3ccf1923d2501707afc9c5e3e85f4408eff7238da737d161210cd370acb4817d9d22b25406d12527fe5f6ac82a9c901294d03dc6" },
                { "nb-NO", "64bac9d4fa590eff2009007a92b842a52ebaa7ddf79557d5aab25dc6757be835665aae3e3c0014fb9284d44bd8505a9e4356c3387f15c3683cd23c9314d106ec" },
                { "ne-NP", "239d3ebd3a391355f74d3ab9344b5c6d22bd21a27742d10b6668e2d8c323f5ff1c52d722928a5b601850641bd31a6c2597ccef808daaf10aa3fd64b587efdf45" },
                { "nl", "798a860204e69cff4564d84186d7f7e390f82f8900587dedb9fafaf1b32f57e509eb0354f2b165097ec5137fd80f1da873a78e66033440ed17a9315537222960" },
                { "nn-NO", "537c26902d258e7834c84954918f3ed7f73a5539a53bf949b4ef88168e653c27983bb86a666341c59732083ebcb6751e8b2c2a0d5b4d903ce88dd0776792752a" },
                { "oc", "aa91ec104b2aa7fe0909821c041afb72789b30686a71e1cf7f3bae64a20e823fbf81a7cdc4e00903d7ea761d01c10fe70c7b65edaef9b9b0f020fb768c9a1f57" },
                { "pa-IN", "d6122632784ac363e1e4a1266bce1e3582cf81cb0775ad321d5f677d283c4c4908fbffe21614f72dc3091a613c8be158f3aff01fc074457cce2c080e02b01830" },
                { "pl", "298ab39c65f7f16ffc005151f67685f8b85ba5d793f086d6e14c5b35920ce7b65fd4c1f84cf2424ce552a94886c94c4c4bc37377eb55935e174baef5a4d5d69e" },
                { "pt-BR", "4e6af7994c48a0ac91706d1192b11de90d8d69033d4d6addb4cdd0635da0ca1cc183fddbe09c121e6a29a0e983559f7df6771b69b107675b270389e12c83a263" },
                { "pt-PT", "d36dbad2a23b5bd8025c95f45e5728311eb9b1110a934af7ab1934efcc588c073a6f4c966678c110c1e3c11dd2a2c3cbe66a9d53572966b5075f75a4cf717f2b" },
                { "rm", "6ba04973b445eb15b9a05ebb870d343beca29f24eb6f12b9a5289128c12bb148d2a33bc1f303a3d9ca9c7225a2aa4f58617a02af6e3e738688d71a5f72a1f577" },
                { "ro", "28ef23e065a0d34e372abf75dd0901c8eba89f7ed7a26f83830dd19c0a8c94841a46578a8077da53b2488bf44bbe07f63658ca73515cfe1664e7f00b2dbc6479" },
                { "ru", "0bb13f2258d481e98e1086a5f1382b948a68f13710f65d1759c5fcf46f7feceb8e9ead0cd2c8047a1af22883ef42abe862cd90ef27f12eb937daabc405c8cf8e" },
                { "sat", "f2a2ee80cf48f124eb31f100ee3b846c6c97c371d4c2683ebfe1a191aa77b901dd470d8191efa92b1a2fc9374a83c4d206182866a6027f556016ecad312b3baa" },
                { "sc", "788125492c329e9ba538af5adcb14c9e2aaf1e29a5b5b6581ec5bbf7f4a205ac76270e0c7c8035b59d7446f70f4a2ba13d648359cde6f5ee7d2b35f1a7b93d5c" },
                { "sco", "540d78794f40faa6ec59e48db446683ad3faaae3d165e050ae91aca1a06693721498e60231e6962236664b5ebe53dc0c58496398f0a1ab507680d4d6dc6762bb" },
                { "si", "d66bda89a30d796080a424911a93765426e780a23cc1182004a723def1864fb5664b9034834662aa33c5de60261942e8f620d79009715083e57886069708b22b" },
                { "sk", "cdad7bd5bd735beb80b3f283bdfe3421047b809a7af3140f9f04ad0387b1b9e3c5c0f4ff8e2b1046e55dcad6e54767c11caf77a33bd53265e32f5f86af6a164d" },
                { "skr", "df036605c1cecb4407bee9671c9fcdbb1b9aae5ff94884fd738476f7f896b6331acd397c8b2813311f771ca9f91cfa5bf0ff3ced729beab4e32a214742f910a3" },
                { "sl", "ce55e66a155d8580e0c0cc0b95ea891a8f51184270242320f835e4db69c1ddf1c5c2d46a2c1197c4c52edee3b9c448c02c3ab2b2141d5be86015023f51b4a217" },
                { "son", "ce2c6834217856b6b8e0a0cedcdd64b7661f4e23dab0359efc3547e163ac8a6e9ba3b907f92757213e0b91dbbb36a1deae914cf89989565384caf8d3e060aaaf" },
                { "sq", "f23c74f6e0da5ddaff092312b8b0a30b5bb2ce38634fe2a57ed83888780cc1de953c11eb10e47dcadadd48db4b463373b77a684daaff7bdbd21d306eb3d6eed9" },
                { "sr", "f91c5a4fe25a44bbfb2ae181d7e9e3e819a7b143d36fd94316253340f45479b86a770a69cfe68359fa47a262b8df9b0c864e0400c11afbae3e73def1985d8693" },
                { "sv-SE", "c8776d5f315f7a2f516ad8a7ad6f41e5436c6161e5bffcb92c9db2dcf659512741baa8accea19768ee58e6ffeeea27d823e50c9f72d6f4ac8c6fbcef9999587c" },
                { "szl", "01988ec47f848b40e2eb9fb959716b0de2224bbe9e3fe31a0ab405f6684483e8ad56742ba42de9373a74ed43e01712f27977fb2808ad52075f8dd2ce887b1727" },
                { "ta", "2545500740bfa85d49fe476e19f655da8140c969acbd72b5db16490b8d6c8fd75d8324f107c9667f636ca33a34af03713df5c87a37ba6823132279cc65b64383" },
                { "te", "212f1bca1d04171c2b908cf958a29e9bfbef53ba9b0ebf49b03b94cf6e54c2f87d09085a3ca8c4949f586083f1a940933d5e261048c5508758a43c454ffe4249" },
                { "tg", "e26e13f90a1c5762419a10250cc393ebf76a13e699912dec2ce5a63c645e241b95e1ff9687e7ac022fc63c1bb35adf014e2c83c06ce891a9ef8bb284825e4511" },
                { "th", "eab117ee3ca7847e85e5c37c1c76eefeb34727603fb524285c96ef8b1e4f16a8444500987cd028ea5e0e7983e2813a0936290ce8bb80b9afbb41dbfdde243420" },
                { "tl", "b047d718f069291964924c1677cfc12f8f1903637308549e09521e0150a4a93f8831e12d1c262146dffdd42af68a4d7476838f6ab8ee6fc5cca2ecac74f4ef1e" },
                { "tr", "8e4ad98aa5e9218753ecdbca1958f79227c7ed2ce530fcd1d065e9e6e828863f312d7a5b43b2277dc45e81cc63edcc6b41837aab3ffab5a8f9b737ebc95cbc21" },
                { "trs", "f6efeca32c306a75e7d539771625b10b6a9b3de8abd0a979c2adf7faebdb6cbd073a5a77201ca2cf0cfadc14181ed29cbf289045f7c7ab600853649c19311dc3" },
                { "uk", "50fae34b00dbdb871d5154e9dddaec5df15019d28c1892b17726b0a7ee67beb4dfd811b8066699be083de674a009d0d3ea81b284b9209c4f759d67bc5422d0d1" },
                { "ur", "86210c007e2dfabfdd15c7d9bba34b422fd100d76b858caadfbaf61c13b37bca8c05b2e0613579c0364afb223b87a68d73fb73ee3874697b0cd1bc37d759baf3" },
                { "uz", "f16e62275a79e2df1fe37f046d56fcab51f174e5ea68b1b01bc1d04c4024f8e78dbdc900d1a8bdbcbf5c4c8d763056b381704f21d3131e53a66671fa2a9e8a8b" },
                { "vi", "791aa145c9aa1d123e5866e991f18c04e58bace86c4f2988c46598ef670d914c03ef64b2fadaed2b9c684578353e314fcf516e310918bd5464b140cd0eb049fb" },
                { "xh", "0e4b10829b21ffef1bdb79c8df64f695cb07f64c19d8f3fa7b38d27ac25a2c704e85cdeb336767392fc8e31042a05906e3794399868e09b056116cf0388eca02" },
                { "zh-CN", "b15ae4d4817edebb7a7bd1fc76676feedaa9acf49becca3f93aa93ba383977d952f0b4c848167a607c2a1ed06ce0d7736a9d99cebd507e5e38bbf1d42366c7bc" },
                { "zh-TW", "c876bc84dfe8fd75e66bd804fd29181fd26bcee3895a3fdfc1e0a5cbe0343747b188dde6724f09d80d44bf7596d5a7b1005b3bfb025bef0a538970689387f854" }
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
            const string knownVersion = "136.0.3";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
            return ["firefox", "firefox-" + languageCode.ToLower()];
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
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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

            // look for line with the correct language code and version for 32-bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return [matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128]];
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
            return [];
        }


        /// <summary>
        /// language code for the Firefox ESR version
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
    } // class
} // namespace
