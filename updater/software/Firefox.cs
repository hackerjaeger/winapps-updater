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
            // https://ftp.mozilla.org/pub/firefox/releases/123.0/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "a9216908012a4fd143b1db4e4c82a3303a457133b94d38e0653f499ae75b81dd776ec5d029b1b7f4a33a8b27e5eb5230f7a9e3853439629447fe07dc9a2a825d" },
                { "af", "95ede6610a4b9aaba32d4a4f9ef2576b41e616944ecf2ee0f751251997ababefa7526d98086118c8b846adfc6aee8b042bcd9baecafdcb51aeb170cc6e439d67" },
                { "an", "ff40ed7fa5ab01c0521f12c885982b9868523863130cd046cf2ff765939ea0f8e06827b03472d7e2fb3838a84de375ba925a166635f381f835b8898cfadcfde5" },
                { "ar", "962764d17b62128246267ca1ac9cb806565ca6694d1a95f8b25d5a4775a997d73a9fb9ff9af120ceb943cee225cefa1a51ae83ac5e143a7cbf41ed349ce8ab03" },
                { "ast", "240c821e21626a72d20e283c4c67e55ce6a1bd50a03e4480489f67082b37fe3724f0cca810f8e0acb2bb313dcffc20106eae2bd6161bef38bb33849512e2f30b" },
                { "az", "f0a337db0b6d1f04c5bac25ebb811d9c9e37070c2f1c24e58469db930b2728bfcbe73491bb8c4d3ed556a0835a8e0d9aaaee78a40d6bab1510747b89aa004e05" },
                { "be", "f23174b0ac7670c07166f855bf75b4104823a4aefb31a5ad23f8b75a4f970fe30deafebf052a65336c183ee93e057b5d3242ebf3da599e10e769a905d3c91b15" },
                { "bg", "d46b63f5f5256568f76ce2022bc6aa338761f67410517fa490ab4cc093f7e6cb3a6bda7f2a831c0417464fcf823270efda4d305d429e41ddd22d4237a5ac0ef1" },
                { "bn", "b1fb1c2990f42572288e91ee1634c0026256b31f197eca22ff98c117bee7cc5ada11c4554a84c714ff0dea24e9204b78faeac7b0b873578c1c833e888e9d47f5" },
                { "br", "277ce6f9a0dde2039bdb9d0c015231f61eba99fe9cdd914ac5290ba907a6d6e0ba9fc111c85142b8e8762ee348a05e72219d26665b0fb301d224b17fdc7f6895" },
                { "bs", "6d790817e1d50d6e655911f9f7af003b6d6c686593c4f82914ca5f05cc4fecea748e940f022bbda78e45b80328ea82b2228be2e50dabedb381905763a56c90e3" },
                { "ca", "1c3ebcc67a37f5ee57fa19625472c79dbdc4cb7c43eb90077ecdfa3b7af2bae7b09aab7eb46b69fc19dd3bfa60c8332587a816256d0b16272f3f7b8f91c030f3" },
                { "cak", "43a93465c5739b6a4cb001e3c40ac649578449f5b242ca23af24108cc37be8edb90468bd534f7ae77d2a7b17424f908266b4195041c26cfc80ac095b307fb7ee" },
                { "cs", "789337fb95ae4800f3f29153ceee2961615a3397d2fc532111d297bf779e8d87c8021fade66c6b8ed2b08d633270f115954a057efcf4fac9bcc834b51e6db2cb" },
                { "cy", "5523b08001a5a28ad28e6381997b01b8d13399a70dda5f0292144fc3cf567a252e17500323baaa6f18deec5c90a6472329f36efa9020067d92de7f61ed793a0d" },
                { "da", "fdd7fa56bf60ddd1c23bc6c0815c8af4042d44e0c683939ece5ee091a4dfe7de2adaef4b14d4c724703b9544bbb75b0e852dc6f453535e58cac2287bfd703488" },
                { "de", "ccaada7241b782a4168ad1ace21e6ef26edc3d7593904dd846cef90cb6b815ede3b914f087afab4e0d015a38df2450b264619eb0e3b7f430d300a04a3750043b" },
                { "dsb", "7a8d49e2736a677462413b75b7ff51cb08945647c09aa14ea9e508b051a76bf2a7ca0e7e34268deb5eafe457bb4d81bf97237f4a2825a6d9c58602d37655d9ec" },
                { "el", "292e31ec55bab507c34a92ef4622a847aeb0a97c3a18938d85df368a6957cf200b8c8b801809eb7ad8f8c908854cf5676c97af9945583d137bca7d05dd18ee66" },
                { "en-CA", "a4719f19c0618b3167e0a7f07e5b5e487b32bd963c26ede9a03a5912aa1eae36e00beca90ae568d5bed77534948a157269adae85d6d0da191c2f48029609d5ab" },
                { "en-GB", "db0b721b0985be25d03581f2117a40c984d6577e7f456ccaa69b1f69e477b4d96015b0eeecbcc6c9d5c677109c562138ff69132452c46e720d247ea8ffdfbb25" },
                { "en-US", "dc8348dadcbd47e25a65a8ce7b2e2743d6e70a8d20e7d3d240a5e14ff9c14a58a90bd3145b2697913b938ff80a237891eed67f14911c3866d7f999a8dd57bea3" },
                { "eo", "5c256283d688f8b62bc59f3c0e5f07d9588dd4c1654d15a18cccbb84e65f3222099ffc216f88779d0b54a5528074cf0d526fec9dcc6880d855a0315e1a17482f" },
                { "es-AR", "24151be50ac686d0a01a1c942378270c6378b41c60427f87045d4eca4f8ed4feb54f6e43b1b4f8133db9b4cb4b0935ae87cd141e13105ee6778537d299c087fd" },
                { "es-CL", "5b4f24c40637b8aae7afc3d48cee9a63df7f8d3d98623a9f7c327574141267f4ff8d06668dd0cc03c481ccd051b8ddae6f461ab6ffe5b9744075985bb0d636e9" },
                { "es-ES", "81e1400075688afc41e1f9cd48eec7099ffa7467763c1c6cfff7718e608831df9fbd4b5ddab7f7a14a4b02ad0604bbe6e0e55ff8c436a44d71e462c596e08dd0" },
                { "es-MX", "cd1139c5efe46294f4879867803e15da62f6944068124bf5897eaf532bc5e73be55330a4f79633647e396f234b6b18e9ba094480c2e1dd78c5e72bfc49c20d59" },
                { "et", "ca400bc727f3b35976d6827cd20b7ef8d3319e62829b9cfa771edb41e0914add407e07bd198113815ac5c63b9f90ac6d9019905d900400be644146c9fd883851" },
                { "eu", "92ad9f21e609604f973858db6a9d656a1a4778847bcd87e26f9753a6de5ddb242b31294e5ec3b0aa7354768bd727a62c5cc4b9959c68f7f4a742ea2f25d5d2e5" },
                { "fa", "7e0d0f73f88b95eec4921e98d55bc2db6b161d3538eaacfe8a89b5878bbfdb8c1470148c088a8ebd020a54fea49c676123015b2813f804b616bf837e6d2a53ae" },
                { "ff", "d4cd8515ee7294ab29ce650230938cc053f786f41adb1832273261557a870e760ebaae364dba72c465af99a0f3a52a7afce4a8143ff79d6fce1848fe56d34a3a" },
                { "fi", "3d91b7535a73f49f8d47019c7110005a9027ca18ca19c4d889bf8dabb34b26604dbc9b01884194245ce399a4929cea2c7afd5efb3c6c20257768f46ed8839b95" },
                { "fr", "5532705f70e04146db5b9426f5b8ab1929e42053a5c7a1426037b45294049982f89f86f36bb6b827c5181761e2decd99a2db113a1c27ee3862e04dd767512f9f" },
                { "fur", "05efd0209e5d3c3cc23c3423cb354a75fac06742d3a50bc48d102d7e4a32464e9074ea0eb5ce6ba6a2cd6392efaaa30f91351e3e3232bacf4521069013d94730" },
                { "fy-NL", "b8cfbf3bf9bdbb1a8a52bea065e6f12a1698d397a9c9303e95572a32ce9095789879fde0d5a79a3e398ca83d3e38be2cfe291a52825039d76e0edd3f6935e2b3" },
                { "ga-IE", "27643cd677e143a34bab271f93d52a945b8dec45164421333240b678e1a50c2612b89a323b9b391b379e560269fdcd4cc0ffa68eb8647049f9427940ab135540" },
                { "gd", "f5bc6ce5a866e12d9a7d8efa492885c08dadcd7833f4828196dbe093c95cbbd70db9e07638e5b9292048983125fbed3bdb4da15a7bc500a5f83ec9aef9231776" },
                { "gl", "5463e89e55861219d88f292246213d6112c31fd81cc377f1433e4d3821784d7e92b0875f972c7fb9ad7feec44b2dcda2502d808492ddb29e4919741e370e6195" },
                { "gn", "c1948c555aeb0efca0fb77a99cf4e8b77654ce34d96f5b129aaf18e5c3b8f4990e73610e4c38ddf9b9721e9afa36c680f7fac06f2dcdb44f9d002ee1480d972a" },
                { "gu-IN", "f39d4d2829763c254ab0e2d9bee1472e6c489071cb35e499d93945fa506fc0b93204bb40055a0de2b4e539dcedcaeeb246916a6eaeed41656498c052230a5382" },
                { "he", "3757cd96c1d3160bf344f74d5da559b23b0b828e3c116e74c383ef9b1bcf6893e02774800dda457291c36362d52e88dbbe349e86d4d872d2f7d0714cd72a7f18" },
                { "hi-IN", "e34384919de67e7c63fdf3c930288fdec1ecbc4ed684daafa0cb31e8312db1772aebe579ef8a3640b0955565419f09757569229f255143b22fe339e3a2e53e94" },
                { "hr", "091e2b74af44abc2ef066acb99437eb59c071e9445e9d0daab89e9ae591c1f0026c89f55d56afaff0edc3dc67333ef1919192bed323dc0c44a8ad22e871d84c7" },
                { "hsb", "2dfa2f748c695f8d91b1d5bf18bf4898375a37e6f7067c3a3a44924da9137224029b6c3e7ccd82a3bce13cc39ee88d67e49be562808de5941ba486fe4b964174" },
                { "hu", "da8dfd0665ad4e08d1e523afaf1d9875d7a236a598c8866da9019cb5e3f205b3d58e289ec23a8e11a12d1086ffc419fb6e384854dc74efa28bb7782039ff6b50" },
                { "hy-AM", "c3d50eed27e85037a68335d8c23c40cf7fa70ac16a9b67f0a7985c67525929282e0dee6a324c994fe7c98568885ccd7b3273769d7db54a2256f13137c086913c" },
                { "ia", "a0aff75f92a187c3b8679118deb9f5e7adcdfe7873fb2e000e0e24eac2272e5460f6a30ef1bd8fb700ef182c6e5f5e639388edc74e2777098294664c083bbbab" },
                { "id", "c978dbd1a4848d7ebbee6641a2668a6575eafd7c9ee7a652b9452597565ee023361d4f6b2a9a835067da2beb5fe222e0dccadf2d759936c64e1e955ca3950689" },
                { "is", "38deb5431bc21c533e8465e9640599231f07a90992c5e09f00e82a5e44091ce9c0d57907b7994040f7adfea3e64e9825cf2cb55ee61f27f0bbee70835c2615cb" },
                { "it", "4c16fcb5cebdeaac5666e3e6714fcdee4b4b9832658a6d938b8f1fea87f5167ca9d706d840dd3c2d6c63e503f36ff67d82ebddf012b8a44ea47a123987ba620d" },
                { "ja", "85e6635bccfd94366b3e140160085891e99ed62feb2c705aa62e7c374b2421df920c77f4c58d72420ae21e674ad0415456631266a1bb7adc49226461b9694b15" },
                { "ka", "88b3b57c81e41ec8f43d34cc8b44c1378a843e12ffc139ea5e43c805d1dd4532299436302485296de87d5fad159ea4b43aee044a0afea6abfa47e0ad8dc52fc0" },
                { "kab", "03a1ba2e89ac830249ce08bfd1371f39611ecf7486521e2f37065310846836f3eb7facebcba955f74cfa5d9765b0fd2641787eb7ca1313bbce340872b94b32bf" },
                { "kk", "46f429d94d8816b580b80a6e5a7ce90c2ca5b406232a531b8b77e812dfe8880aa6dd871a56420a3d079f0131047e47be40f0021acb5e7e5246dcfa971fc8a232" },
                { "km", "e997860ebfd01b36b579fcf777913e2aea3f9975524546c2c7c3946ffba426398f6faadcb4c86ced0014a506687369cdb7b8b3de86c9b91e7e49afce571a5e60" },
                { "kn", "d58bdaad960a97090ba09fac374d867339e2e9b3e52fcca89dc843f1cb1a5fe441acfc31878d7cebb1cce954a7378ac079e483aeb4de89da5118e6234b555960" },
                { "ko", "5f3d2e82619d69af9221cf3638fd150ae32f17a69a4131795a0cfb8b8113315495faaa2471b81d1a6f7cbffa33c0e13c2fefe08eae32e86bcfac96de71ed47e0" },
                { "lij", "957efa61af6b28a90734c5989ea52574a798d35bd649f5ff1e9a66d0b3206689917b1b022a0462382ea9fd2d417093e0b1ba5de5a8a5482f3e366ccb19391c04" },
                { "lt", "1ee454aa333091d8f0b304d77deb1c71bf634cd52f34dc9fab16223817eec1ab96ed2a32f0ba48168413d8bed5b9f25c4c36bf2cf541cbd0d743c43b21cbc276" },
                { "lv", "895d7aa5d796bffada4ae0f0f44e40263a81f75978ef28c1f5dfc63db7c1a0b2a1c5933fa5599b03ec6312e0ef0c7f9df4e23d3126a241e18fce1d1dab935bad" },
                { "mk", "328d27ea420cb821ca9b31daa824402c921077e15cba75da21f1092e7af22fe25d44307d940c7cc7bbcd6e4a2416e285b615c5ce7c0f9250e63cc041c428ac17" },
                { "mr", "a8cb7e97ae730959fb7048284086c7d84ee55ee0f6f3b116fab524e1bdcbb13350c58e987a20299235734a0d735cda2d8d2ffd4ce2dace16d147ec8005c8d087" },
                { "ms", "0afbcbc22b3ce5bb10780e6a77651070b6763040a9831096fed96c44ad3cb4001858fe581b505611aea1b1a3e98a46eeb14d986b4c6de03a51cabb1e46099ae1" },
                { "my", "957fcd85b5e78253c5d7f6045f128b253c7831a1f49183990cb5197be8ca43b297f1e49f0bd59e0510ed51a4ec8f03dd153ece80e85bfa5ddc46a6fb2714b1df" },
                { "nb-NO", "9d54e246a02e0a39dce25f00f6b2af15da47d6a6e192c70f416924736598c2f40fdf0f09de22e5a5f587864329f47cc21063985bf8218cf5aef2e3ba6fea8ca2" },
                { "ne-NP", "6bb26140354c114dfdd37f19a8f33819166ee6f9b8ea55d6b7f48fb51bf30d2e97c8af0163cb0c85ae42da6bbf4f28d6feae7f576c4cba06e0c164d62a7b3dc2" },
                { "nl", "31fd31dcb2149dbef194e8abd4f720fa5efe36750441458284a27370a4e9decbf81544849aba9b612caa3418d1c6ca8adb2452deb31fb1417407836fa1c8b70c" },
                { "nn-NO", "076f33ca22dfc28aef7b3f963f7d42455ebd608b2e6639b36cc4d8dd237f8ee1bd8698574b9ae18d7c3fd6888c385981bd6dca65898b43ddb8317d819e1bbbdf" },
                { "oc", "8a399e5f1e780b1b7592734b55ff6e4c777b5633d67c26ece35d8f02fc575fc1ebc5abfabe388a82ce9dba60a70bc04aaf7f078a2d3bca9b17dd3fd78ef40794" },
                { "pa-IN", "18d817979567120f6f365d9fba730360bd93135e6c874ceb81c28dbdfd0f70f489ec918dd9ed725b31eecd8a594484008013f854836582745cdc3be7f9a68a11" },
                { "pl", "36a33646b83c7c4af77733ab1820186bcf0dda5f62ed0b4955be707efd01daeebb8b74c8c7371cc0152f1b0be24400dd4973422343d9317b4192851f9fecc9a1" },
                { "pt-BR", "5e1b56700901a2601d8f854fdd480e24e3348b01f2028977f2df0f9232dc28c77314ad99438286fc6b963ac521dd13add7579a40e1503113d52646cf2fc097c8" },
                { "pt-PT", "8f37f98e1c66f1f1612e9ac86ff21f7404c7372834e48fecfa2d6de84742361da4403bdfa68d4613dfa7ddf55a6bfae24dae9f1d51ed7812af021596ecdc1dd3" },
                { "rm", "0f327b138a40a2ad62d998b509d447001ad709976dbf3d658dd0c6da3b6cc62894dcc0bfb65a8243f73fcb396094a21d2495dad9792971688b766ede5a517bb6" },
                { "ro", "f909017309c2d7953e59e9e5ff320723c39bb342bee23a80f72a63094372888d024ad027c27a4a69af8bba1d34f5a2e1f13156f41418d3e29ad3137642d8a206" },
                { "ru", "d9d3ea47c49eb06451dda7d8a30b891ba10f132fcc3ca5ff40ad9b37a60aab4bfea9b051cd10757fed15c8e68399c4c6d383bb909146274f5b6b1864430ac4a8" },
                { "sat", "e74806eacad155424e71a5f756bc41d6848cf64c8601b3d81b9f79746ab0c6a8252291a385832103efbafc125fb424beb2b786a5ff87b6da05d74cf263220efb" },
                { "sc", "a5c92666ba97a203438de4a1e030d364b1526594e447420dac6670bf5f198e0f06497010be29641fc91fd33ea15e0ce7d36ac6ab795a3177927a45fd68a57ec5" },
                { "sco", "61bc9c05427e0bdf09efc0279849711842946041c994f542311e69db5d3c8934635c4a3d4ecb36518e856f67ef2e98f6a767c0f38c3800301ea328265cf84609" },
                { "si", "b6d3e69c8a6439292234d67ac24f52f175e4d80c24ec909a8b5c6bb851ae7344735a6969111a47fed1cb70ac7f5fc8b4d7f22c057d857f5bbc02876af3056ddb" },
                { "sk", "76834f091a321b958b8148538866549bd9875fc189305195cb98c9b4143d8a70e9941abc52ae94cb64df1abe9230a190d61bfa1cf72234a84f115a35a111a015" },
                { "sl", "558787c7396df3177657b066736401f9f9d694650482c8e4e8bfa1a2c5fda29eb622af66432ace86d3cf2804517991c918dfeebf0291d74c0984a4de8d22c7b6" },
                { "son", "05582344c00dd68ca7fdac15d88b436135f00adfac3c95c5c8750986572a36c8daf5a68486b26fa98097856aa95bb46709d6b4f3dfbabcf448e1f3466adbe968" },
                { "sq", "3629da89a68f3d3780eaa6b3e84bcef93357c78de8216d8bd9663ba435ec510398bb88136cbfbde2539995fae5ecc3f1d6f4297d44d4df47fe60cac9bd2af96e" },
                { "sr", "db571f7e083a8c8feb4280eddc4b1766ad8d53bf93bb60ebe0870f3b7c6530653ebe489acfc0444a59b5c916944a5932e54136111f7bc6fe0bedd9a901ddc279" },
                { "sv-SE", "f247a25c14c1e70d7cb96214a06559953c24099b1ead34ec0849e1e83eb3b7e0131ddde6f295896fcae5be45e2207bc7c57eec1eefd17861ed6dddb31fd6aac2" },
                { "szl", "a006ee326f137dfe926089f76df2d098e270f427a6c13f418507938f4e342e6f5e9fa53b5abb8e499f8959cc9b073b9af767c8d48078f94c1a0d8ed9d1d3f76f" },
                { "ta", "183467423d8736d101d15039a222f64a8ea8443530d54ff49669e96ea21150fb228a4a83e6741124a4aa4815aaa718caa226ce44e406cf096f60a8c7fdb8fc80" },
                { "te", "73ec43a239cd7a412ae50bffbd06f2fc860ce8a8df3cefe7394f2b55088e5dbddb6110cddba5da4fd5b36d7679b3726e12854295e1e2e51a5a2cae039e4fdf40" },
                { "tg", "77847630f0fe6486da40771f1ea0c3434089d9e7df464d50a0b429cb9ca444340fa69a6caecd8799ab6879901c3d0c41981ee7816b0ccfd485af3c84624268d1" },
                { "th", "ab65b6e5bc8a9155d72a1631d7de3e6d8e1be17944e6556eac6978cc4b9b84312fe57b34b4ba867558b11e8bd14afe7561afa038e6d7519137cf75bc50902b16" },
                { "tl", "667a691d78255deb832523e44402d26ed8e900d0c79b04762aa43d3748322f680b5748cf0fbfb70946fd66521cb733a106ca6833adfcfc30fa58215c328b5b8e" },
                { "tr", "8c2d6c828b5f67ebcdd932731f173b5d701f3e7a989679c7ab91c2e8efda0b205b1038d1c164bcbd3e642e2d32bcc44c3e76533c7c16ab248a07e07f3bbfc8f4" },
                { "trs", "e091b543edb6479bb2ff17f93cfe3b2ff9e5918eaa21b55336401171daa022119cb7d263290fdaf86a71b13ecf1015892e9a5914ce42bfc180ffce62f78581f0" },
                { "uk", "76f63b6a72afa2ebad48f5e72876f73f8b86fc54f8e948f37c95e68b25ca9b02df5d59a572f3e76374e4a1894aac9be6346887a8703a9e7e182cbe079b86d786" },
                { "ur", "397ccd5d6d2abd0b24fd7242a23b3b12015cf73383dcc0dda2b2c69fe33b7ca05dbeaea1095df56895705321bf9a2c785d36d9ae6c5b8e1de81cc8a0a25574f7" },
                { "uz", "0e4c200a4e91c9a9ea085c911c01dec116972a3f1ac81a57edec8a098bab94bb1b78a64e419aeb2649a6ecfabf14d712f8ddc5e26d643e65ef4b497c6b979a1e" },
                { "vi", "e99bf6f0fdd3d061891971f50547b6b42fadc69ec386dfa8977f5c68defae986e8c2f82ccde51c55555edc4abde297fddb0676ce08d58f0f3bbcf14c97a01933" },
                { "xh", "5c91b0f44dd14802f80a9208b2bba0b5bedc422e85a8b80d503f89b648c7e0c23380b44b7a0898185ae7e3514f7aa49b450f66a302f1c4742475585353a1dbaf" },
                { "zh-CN", "52522f7e377ac5ff3e4e62686019b8a18963262585bbef9252629dfe306a0759c4a71ccf2d3bd9baa37c9c03c6f0196e730d788738220b4ac6746bdf29b4647c" },
                { "zh-TW", "9d528745e1a531c6e0ed28f7a400abdc394d2f263d6a292c007b7080e208d2e4b5805c1711750f110a4567e117ccf9ae779415884815029d12ab65313c9a710e" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/123.0/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "eaa680a369ae59e344fcfe3056da35ffa8264c0ff7bedfd43ac01ac6db91b2d883598d1439937e4556cbaa1dee1b6e5bc706355eed7c97b6b11bcf69acc90bf2" },
                { "af", "c200dba32e5b84492e8356bcdb9c0b748cb6bd33f9dac2f342e5cdab1477cd69b46326bee2cdbd87e9706abd281ebf165b806002688ab9530718e82daf961ee8" },
                { "an", "31ff41010cb63bc0171cd0cbeed8b7fb59ce2e3b7c62156e4bcb5a3ca9a3335196bdc3fa3653a3393af4e35d1513c1416a3d22e37dbaa031da8faff55047343e" },
                { "ar", "f868cebc40b8368e38a0c5defc8a1f02f3f34fd17112d371ae18047ab7382bf65da8881c042afb714af5cca6e29542512c7019b6a51c22d8dca14dd628dfb76c" },
                { "ast", "553fd736589aca72dfb8fef21c754db2bccddb1871fd3d895d6af1a327a865bccdb5861a229e01ce97b6ed2aca36833c81278598eed8ddf186be8602680fafb1" },
                { "az", "9bda834c268fafe90399d8f5e25360541b4066a5c34a93e3f0b53ae811bdd228ae46dd20b82f25fc7f1e6e9a784c5a79a177ffef95286e6e9311e474a4bbe95e" },
                { "be", "6842ea8007d1d53bc66557fb102350bddd40dbe65829a8d0f488e553d1ce932d5ce9d1904686dd5bf76a0400163c806d6f212c00756da69bd6e0c118816ceb15" },
                { "bg", "59445a94f37ad3026a75ad86d71cbd89045162636c86589251a88307af6f768dd056176efcd1077514c6a476313b99bde3caf57df200b4874bc80de98f8d716c" },
                { "bn", "5b8aef0bbb96b4e2f3b028b58df696c37cffd05d0dec51af3b6f7cc8b53b89cf36d10caa39d2468c1841344aad4694f140e138cd3e374f048988f12cc551c636" },
                { "br", "511bbd9fc04c33bfc90fb52b4bbbac90628e8109cae0fb595636d1b7890b45c400cf3f7e86fb4ac5f385f7028f9f85db660b8a50b908f6ea420fcac2a3321bfc" },
                { "bs", "60447db1785e3b92722a09d825e1c43c81e102ce807356f2dc5a10dcdf22172a070b787b0ba084032ba0db94d826f0de14523dfc3804ae5f7715f5582199a4ca" },
                { "ca", "88bd1526a0490054f07c2959eb275143ac6aee99b24700182ac781518a1c135433d25cf9125ccf40bf13360c65d5e74b6c0b0302e3b99f76d1895d30c59d249a" },
                { "cak", "767c08da71c663e4a7aa5055cdddf29107c6263c9af1908d8278802b81fb5d43266821b71ab9211ee1639a189c58ff066e4f880de9edeabe8e54973f500aa834" },
                { "cs", "b2aa8a9e2b66a8a734ee9ed621f9730998043640d262bd95aaf537084782f3bd1119159ac343048e9d9077a99801827b6a8806c2b1141a985c619dc281a82958" },
                { "cy", "89b9bbfe0cd205536e77023e66994bc1b818841fde9e76cbdcb74533a4d5750e9a5112481be888079c648ac9cae08bd3af7873ef11ada11da12ad351ccc50171" },
                { "da", "4245ba883b44c5cb9aa0d39921b3f2a7bddc37a646dd67a5217fe37ba11beee435d3704eefd2b13a161bb35ad6045c93a9be0fd0d6ea0b27105aa56659998dc2" },
                { "de", "eb7d3ef0092866043954e89487f2033ab5ab46c073be11193278d325a9c65caf240d09cb6a159dbb2424a8144b3c28ed490afeeedfafc1e3d633aed0321ce65a" },
                { "dsb", "349469e5aa60b091044412edc5f71ae9e1eda6fd6ef1ebf28a183c8a0d36626807e5c44fbacf0dbdb40c6c2bcf1d16873a8f33e172ecbe7fa5fcad6eabe39d0b" },
                { "el", "72b290cf1970229a8d4807d06b05b91e48dd626fe1104abc0a520adbbd039bd379290c2eca5c2abb524ec63ca64efb097bdc85c3a9e50029b052194cc6013ee8" },
                { "en-CA", "f4e04e168b271bfcb69bb818c192425aeacf289663caaed324fbeb74746c17e5846ae2d98125c59734c11f3c0561655d0f5fdee6c205019653d4852b2e052009" },
                { "en-GB", "28e40ca71ab4a422f236f708165031e7c37002a34f94ddaea859b2354bd6ccf95cbf67a762086b00b0e98a94956613f6041371faa367d37af462423dad72d853" },
                { "en-US", "52ba0daac2c2f01da33840ec7253b464e4e07a74a56caf169619aee7f70257248368424ae5571a9caef43d3da741b92742fbea34bd605ba8d438975b57d8086c" },
                { "eo", "924ee8c851ef0617377ed7a2225eb29fa6231eb6d5f8ada9ef14273dbeb11f67f44e05f7fa930f2cbee9fd8db1f2edb17400b09d79eed4f74fa0b8fc93814e23" },
                { "es-AR", "88cd68add046c28f4aa79ce72ac6fb2e52c19db88391991d9e177916198176ce4f1ebc85af1a27f7f9d7e742e883ac7f4c58967cc83087ce581e2bc21e22e17a" },
                { "es-CL", "22c692ae1a6b1921c4ea651dba54b6cbb8953f42df0c3771a3a7f5b9826841abece84b2d227ce5a6fed144b2b4ce3bef1f986fed8d68a2081c00fc8d8213cf81" },
                { "es-ES", "34bf7969319bc60cf1f2e3b7ba9d1f55b50aec9443585f2567012585428d81c79cce58981991e5bc0b33b1cd74ee1ef96ab5566a269ba706fc6922c6e8e5e5fb" },
                { "es-MX", "5e6a9302ae3bc3d6d0b53e360d5f8a0e88a45ca121bb18565eb39c11310b917033a4b28fa8c2e8756280256fd8fb38ea4973c671b48a2ea7577d979055329d7a" },
                { "et", "8263263dedb8df500b8fe956df5b953c05f13d42349f9510266f29db059156afbc945c8c178e54501539a0d4222e7a1e7cfe5cd564524ab6b38f2a54f469fe5f" },
                { "eu", "a21678a7103266f33bc3f4ef46e98f6de5e8d6d4137ecd9ca78fc50d8810cdfa1467c6ebab50b855c19b499e1261324d9aeff91638fb57c99b2835c6b7ebba25" },
                { "fa", "ce9b105604c30d5f1c9b272ea65a2bf60ac15a5a02672244600ae773bc0ab0bed1d9e4e2dce4d41e1c88311fa67917003eae7e4c9c1df5432066514c8feacc77" },
                { "ff", "5e022940f791701b2398fb82380a05d5e31e2b7166e52cdeb3f5b361ba918af85f78791efa921d5592b449b267ada888e47ded6887ba1c9a1623dc797ecedfe0" },
                { "fi", "03deccf15748ec989e2333d21116d169a6b8c482e4bc49edb5c0bd97af0f41a77a11721211941be6b7120aedf0a2095b8829c93617c107c2362a6ea3603ae893" },
                { "fr", "94f8dce1beb55ddae1b2d36fefaa5b1d1b0669ec2c3977c18a6eb566b556f3a5b530ffd570017d089214cd8263069a45f075a5b02fb7ea3cb95951e68e181f8e" },
                { "fur", "e402f4b7b89d208348e57ffd66e5f903eeeca302771f8208530daf256b9eb93e4032dde546b3c7cef8a0b6d1ef1fd2f42c6af9f6091b65d90c7192f0c76534a4" },
                { "fy-NL", "7f080c2603be15cfc9b9607f2032313f33e330d77aa8e71d3866ccf106e7e2787b515c42b2fdc003118484b22dc5d938106387bde0305e4af4ac9afe74bf1895" },
                { "ga-IE", "aafb2f7939b7893da82e1a4342680c19e4a8b869f9377542f4737eee62d3ecca7c1d5006757bba83254d961289bafe2d7ca56a93719221239be12f2846ecac18" },
                { "gd", "a56a686c34b0d625f16c1040212297882e72d65c64efdbdc41f2012a51bd197ce86cf62820e2576c3776dd454c547f5ee4182701125054cbe63900ab5a431542" },
                { "gl", "474e4c57059fb8804a4ee7908b3931a7474f570908fd83cfa2a829a04a23e859e256fb379badd8f8face0d53225dddf40710960dd47d74f5e64f4c83051a71fa" },
                { "gn", "86b3061374a91482fb6ce0312f646793a5f5285d2c187c7f08abc32e5dd24e1c3f3b58380ae8ef12e748eebbacbd6fd0f838ecea2e5e44dc6bc3834efb562163" },
                { "gu-IN", "636f633204f46029e9bad3c16efd3fbbd62f8b1e433484be987ac322cc3420d0a58d49f13d3637fba79a6d159c753bf9f14abc4d14cdbe63797e0bf253d040cf" },
                { "he", "0b8011adb68c2f6671796d2c4f5f948fc4f0b6fc8bc91ada3f8de91ca0ce4f6ba091496cc754fe13f9dd5854f6ef1bed985b9a2ca8e98e69b5877e8a99649030" },
                { "hi-IN", "0879d0a1296eb5182c46c9e2fa4e533eced546f0c3383e21f952cae51087af9bf3050683aaedd910485bfa078820f6ccd68b91d68367a81322a03e3a27147b07" },
                { "hr", "05d8dfa2e5ab6e16fae7c4e6f64a6f932de42e350d7c3c5e1e6f4f4672351976d744a08563b2875c249db3a9f7101633eace25258e8f74660162b0e0ba21ebef" },
                { "hsb", "181a2e33f34eb767d9ac9d585ee0507bf52a52443c05d8d57f3b9929617d328c41199f75ef1c393764802d651202feed74871dcf56350f0a85ccaae241d397f5" },
                { "hu", "0315dea8197c263a7b2c5e9ea6649b49aae444e51e5c97983ccea7dd1131ea46df25d8664969e9d034c9a4b459d2b88f3b7a690918df4d3acd9a41a9249d8518" },
                { "hy-AM", "e4cb1622172d2ccf15b3903770a1513159c6af93590548de3500fe7e862c69714664a10f9cf1330ae4fcc022844a295ba57ab1019fb46e32de387a2f40bfa3d0" },
                { "ia", "d25a5107865e1fb9c77cffc51b42fe273a3c4bf1d0c0a5c075e0ab8fe4efe357605145cf6d7c8488292a94b48bf9880a0931cd1e765d9984b34b20f93091e7f0" },
                { "id", "6f8a859e15d622427220bea938171a31fa5c97f33948f9715c0464af9f076791fb5b9c4626fe474c7db12c8ca958211f0a188403ee8eec22e978fa128b60485e" },
                { "is", "ad28be3e3cea03f794926d6c25651b0b3fce9d855b271e40a377660b4ef1f6f77e943b410006d39b42d068b64112017aabcb4128762296f7d20471aec138d219" },
                { "it", "9fac42bda133b5cc75a4310dd3dd40718a85a31c84b1e62190418fa8ef5060962568e37babfcbc4b147f14b4aa976fd5114f9129978c68c888786fd40666fa4b" },
                { "ja", "22c578ca36225f7342ed9a6551a43a12323889ce55b45cde4a64452cbda6e167a6263020ffd61b1d0edb898e24af0fed41f60562271920d58af400dfa7626605" },
                { "ka", "e59e4a000bc698ace5d44e8fe83bc34e6bfab0c3b187c83f68a085b6a21531016f33a4699b8ea21bb87c545629882916e57b512d6e39a8a1e56d31ce9bd6cc7e" },
                { "kab", "934052bee5dfec40951c66ee517737cc23c49faf341bc2461ff967c55dbe19e06b1e23264c89582c31bbca9ecdbeacdb109c4437f360ee8c8a38439957415685" },
                { "kk", "68e71951175dedffda127171a9701782dda6845640e1058fa934e7996f1a8c4483e5f3b548b3bf70fd0a844fdfc080dad650a0e508f8f8c7f54507081ec0873e" },
                { "km", "08d5f2fcd0fec7b27685ec10a7f8bac938d7611746c7cee360abb437f87e5a5f40fa7972f0b1ac8d07f751128b3666a26bda4892cca7e57cec7ae85445a37be9" },
                { "kn", "0f77b51e72caa8439bf36c69d1d4bc18143516bb15fa088c075eb430215fbc39e3a5ff363990be16ffe8823a9d046c37c5ee0dce551fc99d0c8242418195ee22" },
                { "ko", "21f497af361a6ba782634c3614dd66cdd698976053f5b1f225158b870d80dfd3e4793aa46c9359fb1e52674bf7c452723500824712bd90fc1199c0c8186091a1" },
                { "lij", "056c4bd96e61ebb96170cfc74c56a10ad9bf1437121911f782be1680a7bc1812d0474e4ee69002f93b4153ab4facc6922076fc8ef30e7c1ac22ce8facc720dd3" },
                { "lt", "e79027cb7d5d79176f248ea2a1d317c46777bc12027d6fe62142867c009846af3e4665224d2fe757f5316d487966429d47efe061fea89424e4d07fc1bf5948b7" },
                { "lv", "0bfdd6930ae9c9845639cd1f0185158cc2201d2eed75dff94bc00bd3631e8b113a913caf24ba0233c894dc7db060a20055a472ba4a85fcee79c41908099d1d6f" },
                { "mk", "11d96973391326f27543cbd7bb93feeaf88469ac016ea60b6f4ac26d7a6d0638e6f97c0a894e8b5707f447bd50b6ebb4e676536fac6ea89671e8efe377c837cb" },
                { "mr", "b760d19dbe9dd85cfecb36b77b31c3ace21b820530b6671335e6c5905508b87996030353c1f0a1d614efa786dac9c5663ca96c8d0129dcd1609e2f3ddd4d5c9f" },
                { "ms", "abf04f33e8b1670b94dd5bbba2e2004ec2814ad3eb703e30f77bbbced5ff05d791d081e6994c718c1c7a734bcdf46f21efa3012233b55f1db9986a0784fb10d8" },
                { "my", "9a911951ca6479ea8c1ca48326d1603ef653cb4ce2603f17065ad04f71d6266f447e2a979cde7ec34d31551287478da257df1d3ed6cc10d587f8cc7628a14138" },
                { "nb-NO", "457b9af02a09478013369070ecc557775a6540cc4b0058cd865cabf7296b29aba3d923d1803a0f04116d998b69ae6ab7061ccdc1208e9e6ab57786ec81187360" },
                { "ne-NP", "594f841de0e1ae4fb09c16ff7953595bd6f273ef46940da7798170bb284b3079f6c198442726c840a013b01ee52d4a58e9b4d3d0c92c0a3fe9e2b3a5fcbb55c9" },
                { "nl", "a4c3bf0b7ea8a8f3a846e58739f561663417f35938b03efcbdeacfc1a4dc1c243f102f1997312f6859ac2cdff92a9ab7efac3c7654d46faba74743e14518e8d1" },
                { "nn-NO", "43fa53dfbbe33dcd68606205d1213c6877c061135982e4390b770111562f61c0f6b798bf76d958fbe6538b217d29f685af3ef67de80be7dc29dc10ff5101655c" },
                { "oc", "43275d661f5c29e623047ac7609e36c95c82e39a3f34b2a74202e96486fd7e59d3331a5c7ab3bef7e2c50acef791bad1d065c03d7c67b0ba506ac3036df7b489" },
                { "pa-IN", "bd9df10e344231d31eede5229a29d3cc171d3de2ea8448f024c6a2ad668d563c05a481692bd059d0021824e911193e9d87ce42a1addb112a6c9244b09e8a4015" },
                { "pl", "0b6eae140bcb0ec5b9798d8669e73e92f9158727b25d985b80fe0ea2cb4b731eb3b04374565f9f72065d07cc7654045523d4440e7c812fd4fb671688fa60c9a7" },
                { "pt-BR", "372dd5b11546aa8043f9f6d81535c3941ce37deab1d191ba9aa2c9196b35faa0b0b5dd235ababe616452743cf4ee1e9633fafe541925848b3874eb1bc67404a4" },
                { "pt-PT", "665cc17f8901024e57b22e19d700a76edcdb7831cbb02a2e1e0c1f8284d1e2b32300984cc39f8cde784d8ee1acadba1cc7e53ec0f5e43a418c66546e5dae85f1" },
                { "rm", "d64698f36915c00494f67382fdf7ef8c3f7c092770f82ab915cd922e7ae35bcc5ea29569e50a10326cd17a8d192f6a2b14567926d258bc36f74436bc9ed0d750" },
                { "ro", "baed447c1879e907e1ca19ef387ff6577cbb2b4ee88179253a23da2847f22a5d32c96dea88fd0f9740a69457935a6105cff0cb3e14c87ab834957738eaec20c9" },
                { "ru", "d0bf4afc0c23e1fa7ecf819289069fb84045756fc21419021a80055815ed0538ad68d2dc8b12c4c286c962acb7608a06869bb09662e55d0c61bc569bd1589503" },
                { "sat", "2054e62ab8a26f4b54db62d839fcc82e0cbce68f633c0a4255988fc3867230b31cc948af39a81274657e6bf9c62640a05b40a9380449b743516e054c9e00c01b" },
                { "sc", "d9cb27df0d07485ff2e7ab1312510802bf2b19b9b4ed30b48da257585a9c7349be71827b20f6db3e1a47e9eee3bc5fd5f727ee723f79584dd2c0c59fef0706a4" },
                { "sco", "eb7df7b3a353357d816154b912325843a6f4c0ade05307833c9bcf1e907370674536f22b639d2c3bf818dbc133cc74f0d380c89bca27db0da68769426fab1d23" },
                { "si", "a8d52e02bf502d3eb6ad3f2c239449fe06bb0984b852d8058d7e9e3b8a6da5623363ccc69d131faf7324e92bf0804cdd743c6a249bad99ea5c1ddce9459cb18d" },
                { "sk", "9b59074b8b575cc7b6658dfc9ca55c66329823e03ad16d36c58a397e0aeb74db56aef8d4b4589ca9ae39f440dc5e1e876b00dcedaeaac95f63c22703479bf441" },
                { "sl", "c9352bc334677075c764c9d3757f0a30d50e2a178dde4e00d2f4ddc4a5accf9cfa329ef66988d431c255752861fb67e9f1dd8e1cfc386fac2df500e37d7d5db4" },
                { "son", "a37ecc28db18875e0b66b1160cb090bd26461c6b871322c9d2dbc9fa8163ebc0fae989d556556ba36091f367ce873213c3fd2f54ca46241da351de93d0b7849c" },
                { "sq", "c77f0370e89b70e287a7c9f9085682ec8d54d6eb9d5650876df5c9e78219626962872d885e63a18ce4c7ff1fcb05ef810656f7619e861fcd876455fc0f96a95d" },
                { "sr", "50a3f8d3c3d433177b496c55425f1ff71b097625fb4890eaf87b99b7d36fd29b1c493890655b8d081b89e978fade55ef13acc75ed1fa82e2475f5869cc76b571" },
                { "sv-SE", "7032a7389739b750ff3328e60b0e4acaf675621f792a778271cd08d2429ec0b521461d369dbe0c6d7343589658060ea38d482960a1b0e3389a8e06087b3571f6" },
                { "szl", "059f15f5a4a56b592852349d31f890cc99bb0c869966d74e4c0e56ad8283941ae37646c2dac4052d4e4b0bb506780dd0057e299fd7f8a66fa522add99c0f8816" },
                { "ta", "2c9160bf0573e34495779af625ae1c5bb512bebcd11863ef8f1823baa6497e466c01674bbcd1c62ef6d173b83fa69bdc5dc305b796cd1d83692d7157ce3b7b0e" },
                { "te", "a9a648a7421a2de72aa12fdb738caba571a87c41ee98ced43f47dc586ac82cfff8d4848b13433b08e8dae66738f7dfe1f95a17dd71b802b61b925d11a681c2c4" },
                { "tg", "0da51ee69adbfe3a5f40c4d83ae45fe7fa2170a87d9a23fd1f187bf061aa39b09478f397cc2835c11d08783e395e65beddb12e9f06a30e82e34193fe00d7ac46" },
                { "th", "d03aaef65c065a6ca436d5ca13ffaf4ea27ea927c626f8f8204a32931f6a12e2baf423acd2382bcc7aab1d6e93eca0e72a9f841a987527df8ab9eb4b2f0f626a" },
                { "tl", "893f75afcf951d46159cf031a2154fc20646a309daa467380dc6653901df780f87057f77eeacc5342b0cba65eb72f1f723825d5231a756a91ea5f03f7dee20b0" },
                { "tr", "2ccf36a77a8b0af5d50d48e9728d09fa0ff818d8fb1c21de59424c511f4ceba76d11623edb05555393fa44a64dbeca15da65a8894a820e260f3d507f28009b2c" },
                { "trs", "67a465c90b952dc781aeccdf44d1f8b539fdaf373ded0c675b67094bbab8764d45f282a8075d5c2a1a537624486ba548a87ec558cd22c56dc5c133a15756602b" },
                { "uk", "671b98ef1e6df18ee198a7dbfe02f56156608678d14f3e1e97be1412d44110e5a111b097f859666223d3fbbaffe8a3cc864c34b54464185fc45595a85b1203a7" },
                { "ur", "7669dea4965da79e775b26269a57913f8980a93e21cbb7d273da40d9bc747521da30e652e57533e2d9db2001c3e66f535f1b9a8c8dfc135372cf17e914d96efc" },
                { "uz", "9f47db57364319e7ef3d6952bc3d9aa22ac7360be9fda0e007aa855a0f7921afceeafb6d3968803be9fcf67aa1a75cf8b49f394a90baa7d4d30f1038970a8092" },
                { "vi", "3651461306fbf2e64b255376cb24d88ebb401664533f6117d467365e9c451510cd731db2d1240eef7cac1febaa63d85983e06a44eee325f25e32ee28f533febe" },
                { "xh", "94378320af9360c9fe7408bafdb48f56817152b204f94fc1d3837d74f656d1d7297e57a5df004a685cc1c396bf1698d562a1fe7dd4c05ab5ddb027eab5850d66" },
                { "zh-CN", "f6bf6dba8ad1d8348b30369de5ae3cb8b0c0e1441e624864e7c9ecd4fa576250295e57a555e558c17ac87a7520c49cf9057b4fd30b0d59b1001e04f06c952c52" },
                { "zh-TW", "5d14993c31dadcabb2586dd4bf7ef49e2d201fd6463c49d6de2a006d156d5514727c73c5a225c2f95941dc253ee7b1fbf2f7c60c5ecb5c96eef8a12f7921fc91" }
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
            const string knownVersion = "123.0";
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
