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
        private const string currentVersion = "98.0b10";

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
            // https://ftp.mozilla.org/pub/devedition/releases/98.0b10/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "8ecd4494abd19368de8c4a2caee4e8902bbbb774d4f926bbcd5627f2df3899257b22c748fea52eef1421e6face319c12f750b0d1f7ed9542302b326fe64ae16d" },
                { "af", "32b9fc67deebd0a88c06584745e7e7240296957d478fc64bb3f05b9f511dbdc14ee509706073f8fa4c75bb05875a637545eff36798e43a534ad220368f28cfe1" },
                { "an", "bad8f26245bfb937220698fe03f65315ed2fc1d75493768e6ca57ce7102ac7ea85f5eca714ca2d4ae0f2d277f77ab47ac6f71f8d30a7fd640c752be3fb32e6ea" },
                { "ar", "a9568d7054713848b3adcfae0b469db2d2ab6338e1cb36151cb77a5d8ebcd64f26d48b2479c172a0f980147700f65d899ff33df7e99d136e8735896a7b888b88" },
                { "ast", "3af44c4d717e20e9d92c3de8840bf2ea15ab6cc307f9ade7affbe69bb712c083b62b6dd438db3020dd71c4dd19bf5d76d00dfa849f1ef9b8dadde1c4766ad37c" },
                { "az", "d91dc9356e4b2577989a014f97c2f0c6ec70d14e2769f37dc3149fae9aa7b2a1a5c84c313bf140cf4185399f7e0ca6946c92ded98b6cc4bc3739726070f85418" },
                { "be", "0b83d60a5d5cc75586138c88c122984096e8cbfdf87cbb4772ffd554b7aeacc1fbb2f65bf305ca97378ac44a5cf2ff450207d01f8a7250fd89901a50ee25650c" },
                { "bg", "c43db01eb9a50a7c1a632a13e8fc6e0c8dae82185ffc937f2fb90b76f8ab334bea3f8d816440bcc7945059a0c85dff5c4a412180d44cd2ebedb5d5612a850e14" },
                { "bn", "97cc7bee27158e1fc9ed80e8e3357d5356341ce27f7dff2f23a8bb874e71215a09f53b0d7cffe714a77b9e1117de885c08f9c6671ebd68289f58c0ecdad8d2b3" },
                { "br", "048b1a08e30c091c1a02e9d3956634adf642a2a74966dd246a20f14c744990ef992a61261fb7aa6f02698dd703ac59c24b44c687e6eb56e79695328f9c413299" },
                { "bs", "a7b8ad7bf4a7cb2511d1634961c4bce2b5ae080f0f907fc20d98151cc4d409efbd4120a27265c7c9a16729e960406f701be9b75dabc13db9859d6746a90ded5c" },
                { "ca", "58ddb05164f0c69b578f2200a4d7bbb5bb9a68f32bd956ac286f7c20200810cf68ea5aa052385d1165e9170913cc84a5d9323da979aeba9a05c3932884bc5b33" },
                { "cak", "f359291e39893d20374d23816146c59aa89d245d4ab309419273b6258f86e14ad130d35677ff8de5409f7d0ed10859e14ae4b06e99610030c1c536d6a09108ad" },
                { "cs", "3b862021056e203b82634909cc95b6c48449c2de57b0f845eb589e849c1d5717077af50a140841c55d4adc19dc90bdcbfbc1bf5d6a65573a40d7f4d9992e1fe1" },
                { "cy", "c018b4592488b06e1ba87718340e8550a4946da5cf326c88350fa265584e60d4ac64a0e4c3e8df143467338680f822ea48e395c3768c894ee0c09a007af4122f" },
                { "da", "d3277968bf7bcda74ae819f4ecd65575eaf6e061194675943eac895f95c7edfe6114fde4bcab074ceedcfd901d223e7eb64e7847caef89dea4eb11c4930ff04a" },
                { "de", "6f542250a2cbb4bf5e193c87d273db3a14cc7907a61b705681d709b5f0acd4a619c4f91582206e9b5e596e17ac9e9d21f74f3c1ffd58df9938c33644e1e4b9d7" },
                { "dsb", "b8b81abc208acf36fc6aa475232020c291a13c49e36fd56a7b06777b962328e21648c516dc3fc528fc89c99713e7adada246db146b9d642aa0635da2d3515d60" },
                { "el", "6ac7f772b79eb78600a7edb6c8764a2bb1d7ef719058c63d56549c7edfc7028b7a73a54a7c349ec59a31893ae967064ade42aee165f66f664e4b4d70bd9ffab1" },
                { "en-CA", "3907adb5de71eb271d9e0e614ee5de93bf0c6375f6b10d55229a714782faa325e5059a311f2a893124035cfa463f70cb9b6261ac84651913945ae513e8b4c308" },
                { "en-GB", "0618720deb4c36400536e4de7ae764459f6d81955a942c87bb8650c481cd150cbe8c88588d2f63577644060bbed365e52497a53727b838e2ddd66b8748cf73b6" },
                { "en-US", "24b3e0bec385b9c52b60ee91fbc464e37218fe6182987b2f87794ca8c63b6870875ca2b8f0ca3567e1debfcf72e0ddfa0ea8734a23a51f9f4ff9dba0f32dec37" },
                { "eo", "a50b631cff9bf16a7ff751f5fb311b201d27cb981b7c6f15e30d46ec5bb849fb518a74ec5fd9f39fc904972ccc25bde8e2c2ddf809c7624c0d56e692ff47bc1e" },
                { "es-AR", "68581ffc37db8de6ab7892756afe9cc9234471be57ce9f951685ddcf4a71702e4a23ea6caae45f5a2f3d0d8d31415eb3f0a58221c81385de9eea48d0ca7df563" },
                { "es-CL", "4ba93f9d3c92b82e3bfa3663fe0facd5b9a17253b41cb29ae3e68526aa1404c09209ed9e27bbb4dd8f2f13d4adffd5802d60d8f679ec2429b77ce8043702bced" },
                { "es-ES", "9f7742f6409a524c159960350110b17f286ff14b3b8f09effc245980cb14ecc631c585c30c7a274a40f9421732a10e014f9d27989d960de85659cfa96f149c1d" },
                { "es-MX", "136ba351b25533617aca31cbf5b47a7e912dfb586168516a02e2aab2a910e6dc4528fa8c4492095479fec838aa46f1baf7d57ea63f17b1cc4860763772ce230c" },
                { "et", "a06afb1ca0784bbc1b4f3c2149d3a155f8b8f37df013caa093090ea41170ea30b457f3b29aefe0f50f46675b1b72031a893bd0e2e5d01a698c5aacbb1a135ec7" },
                { "eu", "2387abcc6dd1a923f2f71bbcc0b43d99fd0e9ce515c552460cf2b0ccee807a3acbfc50444b747d360e0bc41eeabc7ba25fe31c5c60a79111ba576c4e8072c4c4" },
                { "fa", "9e2d2feea65ed983ceda6a4bde9ca3f668e5e7bec71e976a4757688c15733c213f680d578cd65f5aae1061890ae5a4b4782b9da1600767f2d4ab4add6ae4c41d" },
                { "ff", "364a784d74a4b0711c8fb2b9ea52a9b5658c252487f13d9bb18bccf8a461cac927ce0b634f6afee1c199a8044d10e168ab8f9ab066532a7d127554ee58f55ced" },
                { "fi", "bed94217a7cfc3ca0a21af1a245a321bcd857293564b7d122983f3556283d19076e52774634bc77e25959784319c0a032bff7dd2e76230c3b620513412da9c88" },
                { "fr", "44edf9bd030e3d0012fb7126de21b7df53dfa40958070177de24c90cda678f60093b8f8dccac70e0a35947c8c8c03edb4a174f6150420218db2512fca2bb1e56" },
                { "fy-NL", "4c04f8a3b8a3a16f71c7479865713357c6e347b4cf604f5d2d6e167e2413f6da11bfb338e105a414c9f0f365e66e5003b06f82d0adda873eba7556ee87cb1ef1" },
                { "ga-IE", "4b70c446c83fd2589ff9d79e7bcc811eb0ddd3da0caed17e295374ddbda115f7f85a5fe26858bcf920370890e4fa359aae8a4282efd8a378e2e3da802e66d1ae" },
                { "gd", "7f55968d0143e666db9b488b163a1a3b1d872fbbb1cc6067aeb4a1ab26bbcdda98ec44c977524a1365dbfd77b6954a8eb19aecd866aaa157dd057663559ee271" },
                { "gl", "635bd5b1034d9a75ed171a1226c1306eea2dee601b0650f73a614314bc6f06ab8bb91bed03f48ff1e0716a2b3c256061e4044ea2fcc21eb7811be00c4575f4e7" },
                { "gn", "ba681afb7f31744f30d8f3be83899a573095c538a7362d5836e6161728a5ecf841ee202e6f8e1422087918f88f86317a1307fb723eb863c061264bfa97d321d4" },
                { "gu-IN", "7a76f2c341a043848266a4465564cf1eda0f8f46452d19aed8308a4d6b95b4c8d8a9b8cd0087414e93fa62539a207622d44e960074f15959f074dc99e3d3ed3d" },
                { "he", "6b8a30b295368db5b2a254c125b992c1108cade7989d6a3848fba598f8e87279bcb51973c1831854cf1853963429228ca1070f508bbbf1939359a059af3f281f" },
                { "hi-IN", "489df81077956372afc8aeb01a7e2f39e6d38014458d344e9300eb1e6d9b8734079b8b15ac3a758b228292b5485c2271c4e1308c8b0b0448c43a9916c7b6741f" },
                { "hr", "b473f5acba7071535864c0729c5a761100ba51fdaa6133d6e51434f5773e27d16cf82884442594383fd4100a4566ec3e9718cc6d3e3677254a6c93b59d797b48" },
                { "hsb", "840cc0dba18e109f6398d6a23440185d289ff8ea60f16aa0b9008b15eeb2809588de43ade83f35e399415e53b9ab065e2cd0f2b051a7f23e8e604dc5b452abf6" },
                { "hu", "6bd64789bd10806829a5a8bc2d5a4d0ce1d2e9652b50104ae91d99d967b6b355aa85b7bb4ebc657abb0d80b323dfa577c690da9c7ede386a7e61d48f14337c00" },
                { "hy-AM", "5bb001bfc665927ef26d45e9085a09399d03d237cbc425910eacf8a5d786b2aa85d5ef2ea0ff3c76be3ceb327c12ef5399c8e4f35bd31ca2a161f5e3b30fdc9f" },
                { "ia", "4df5a9e5ca750dbe4e201a3aede8fef2d3958a95ae0037899a8b7917671b132bb6ca151bb8af242dec8591f60cd5cd3ce90f0cc73b354f40f84ee96ca1e78ad7" },
                { "id", "72e32f98840d1094cbcc43be5a97ded5e6193db428441b409ddeac85a0856b780a32393a1f6e3e97a8634425a1ef73975b10bcf641b33866716ad12549e4e169" },
                { "is", "fa8fedb9eb3b206a8c5c193e29228d54c559b9da698df9415a43083a8719bd7f1e9de18f62e2c09a0baa7f8eddbd0c78c7ecf540ca850ccc6ef2cb138dd3497b" },
                { "it", "5d366fe76b463cb77e3f605c5deebc22f463ab9676f9bc2616ac319ba51e29d80255d6d67e9eccf51b13d96f346e7c6d565c2f2f830eba84ad6f99addabd9627" },
                { "ja", "e34fc1cd3dea8aeddafc356b0d1ad97d9c48aa7334b4c8783852aa2bb5fb7fec3fd54d810e1146b256cb5262fa850cd7e20e7daa1577d140b1d3a7fc93f67fa6" },
                { "ka", "0c54dca5975d0332136983074fd7d91aa7857f8e17032fbd367da0515892cec566cc33c84c1c7352aa3964481212d11f802a344e3e76473632be98b0d9afd2db" },
                { "kab", "3f024c5b70b480f0d7483d4f2d29bf0a90063cc714cdeff19796fcc21cf4e989993c9d94629bd96eb05b195ca608942195ee36581fd0838e1da0cbe26051c716" },
                { "kk", "c35f6c51b32fc762b5a9c292b10b50baeb3aae08c4cfe0662a0e57410ede6fa953b6e6eef88be72919e2c3033b7e0079b181ac31df8d536dc612e11de21eae33" },
                { "km", "2af4ebbd9a6c23d859829316706e8ecb6329442ca868503c5daa889003f07925eb36db6adffd3d3f5ae19b2f70063578e1acf6ed95b18f8a74fa0d0d2b56c187" },
                { "kn", "d8bfd3b0e429ca5f2f6f545185f26dbc1904f7d00145192a735ea359f9f93812d5322c5e974612d1115e126cd660cb9f2e55ea8c3cae9727053387446641389c" },
                { "ko", "841a3dd63aca65ab6b4f37064c12c98eae8bc89d4e5e3cc8d1ba249b871c5f3cc395e88c02b75860a514cab7ee158658a3997b3bde5d8062e0cc52882ef7a33a" },
                { "lij", "30b5e67b3e45c9b52c5b5f77637fce5862f55a10992741b4553198b9c544a1e56c12af30302a9c4d8869392668f2e6b0f6692000438d10be70ba6ebade31765c" },
                { "lt", "9f906482c2dfb45b818da4a9af29c61c44957c1e92ba215094457250819e58c840e9f362a6780c7ca3617739bee7ff22651bcdde318aeee18a8b4974b0bce119" },
                { "lv", "1745f6cfcf1ad8b80ce99b4f85dc8c92f6b79d321daff9a19b9f86dd2e6d74fc6e06af7dda90db35f0de3e083ae28cdb819d77c44caa12aeb8210de878d66df6" },
                { "mk", "2ec21934a2f16482cc6116da2fcfd825df222adb43c80b88fa2f19bf145eb2d2a855f6da64026618dff4f1e7d09488087ebdeeb80d893e5db68f5bd9b194554a" },
                { "mr", "570e3017253b37746713dce8e4b3f16ed83b733adb366ef7a182d9febf537184d63899e5a1861467def5b05e887718647d8884c28c2748e06b54aef694578b17" },
                { "ms", "36d599ca51ba3a87f0d5ec75801d1faeca0bf82413a7621bfdb01823a8bf139befa00a87bd489d78dbcb7f66e276685196b0ccb8486240badefa059ee3195f49" },
                { "my", "6a2dd1ecabc413742ceb95994825d422b63430513c0aa695825c4fe9dd0d175074c4347e128872fa220176babdbdebcf98a9f42b25204d2ae1a477b526a08bb2" },
                { "nb-NO", "dd094162d085fc22834463190c69a932e6bb5c0df93522d9eb961b3e5edc62e1f7709e0e0e504e20a1ae4cb0ed32da24ae2596a50b5f6b719f1b09084acb09c0" },
                { "ne-NP", "b7df23f21ad6ed2a9a1c3fa77a9638d937f22350ebd4632bdc7e7f6fc641b94c8ed8b5d1ed87e6c08954adbf7a664fbc1607ffe54cc3a2eb4c8eee300451bcf1" },
                { "nl", "776309ad129cee46013133f9a785080f7394dc6afaedc6dc02bba46b1713bbaee2b66791ae41b60d8186a432f0bd4b9198918e7b38be1bf4ca2c05f7bf7b6fb3" },
                { "nn-NO", "7aa6060c38b8399d6cc7bee396fb827e3fd4d1bf23ee08dae72981a320ec899cf5b2c88fe9a3d0f43f2abbce2d9dac1be48d5309bff05e074bbcb3ecef155b83" },
                { "oc", "f13139858153e7824b535f13b7a2850553512a3683d706676ef6c13be41692d9f0606435da832b84043c983ccff76f52d42c6953afd13e1ef259bec0a1f56db7" },
                { "pa-IN", "48d04d7bac527d75697dc683fbaa22cab0583da4bcc0ef632958a78455b1e20703b9a83ad2a9cf5bba5e3500ec443af72c114702894dbbbcebf0ecacf0cd5124" },
                { "pl", "6d68aea0d9a6e387a897cbf27b901a03ed6be4190b597594c81e094302f967f48d496f4963313bfb3e47cba65cf0d7bd6b2154b4238337f82e984cc5e41b948c" },
                { "pt-BR", "d5fb5011791283ba124a5a823121ab584e7ea8c485c8c9405d2cbbf15686fa7cabd063e3ac19d48066676ce01ede3dc146a227e1450523097ec0d8acc3abddd8" },
                { "pt-PT", "443d607b3df65f63cdf502ae64e04707a429fa6e374ff83846a5aebb69e473f29fb67c9abdc5e5e6fc880636c4dd74c00476fcc166ed9e46b92c67c78724ba76" },
                { "rm", "9bf9d74d0a5a7288d83949a2b9b42a45c490dc634e75a53a593d25dea7038479b270565113f8d0f369d17a95c49bc0fce732da6b450e4472445ab9fc57fa1e0a" },
                { "ro", "3f2ff4d9857aabdf0b5708b1f6f7dbace33f0e597c28ed74cee271de354721c0463dd37c6ba4ad8b564b4090ea9d4e81b5b7feecb79d29fc5f7dcb97653c8f6b" },
                { "ru", "696e6ee86a41f947abbde1cd748c10a09939e6e1aa83bfc98a7c37bb71901046c833d3c111568d3d5103ebc3fff8ac23fa543f1269d58c24c64c6a611139e507" },
                { "sco", "a926d19c18256ef4b21d0d4b60936c31a16a3221cd1a900813bd2e0f29da573289a86e10ddd53db8d1bd19c33bb9ce4b006e9e77df4b13cea382485b92cdbdf7" },
                { "si", "7eae282087f86c64dc05bf7526c11d97cbd2a9b2cc1ab1fced5568ed8830a97ac04267921a58741577506aa233ea5401c6b50a89cbe90af564adc1f233f4fb01" },
                { "sk", "1217a1e0b77fc0bbaf48257b6bda02e10785c35523f875f49cd02c6994cba7e971471b61c24cca9b5cc7876bebe086e384caf8e91ffa83838edf4080bc1a8dff" },
                { "sl", "91fa36322275ca52a3fa730861e52f966e2965772c20959945cb9362694e7dcf8447d0b896a93d166b6b154bacda2a3c8876554d10e28dbb7dc5b8a3015d5ab9" },
                { "son", "5eb3cb7ccfc291782ead45f90f9813abc931d749069c1f63cdbdcad93b02262642935eb03bc1be3d67c14ab4ef02a70640073c0e5b17893e71bd5b75583c0946" },
                { "sq", "7dfb923c78346a6c176a01a7b936b73a445571d9df34483ed4f635683d32e31f617ca2d264a6f615745a22bdaf39b9de10ec068cc72970128f558a33d9173e35" },
                { "sr", "776bba77a399d9b27a50e9048df3eb83b10238bfc20444f67dd4eb07d16c5502afb78ce93634e3a8fb2a2f774c73fd12b04fa9ba22682002ed295496139a8379" },
                { "sv-SE", "e3921e44c0925f1d55e7795aa9ec53845439c7ecf895fffa4cc41227c10e27b3152331db8a9edabb8d0e94f3da424572e0912c7d8ce8a5ea26fa7329ebff0d0a" },
                { "szl", "a4d7641d47ff797691fe660f928c2a4d218e6a885eef652ca5102ca1b50312d941d2dbc78081fb8f23e61b10d0f4074ffee8d8141f7136f7cfe2a80fd4f4ce07" },
                { "ta", "238f1fce80f5773ad367ee2f9de0f501382472b97989d6f0309d36641542f35106a59c36f2ee8ce07755edd68039831d3a590b3d4e9d7ae4ec443f063a6c5f8b" },
                { "te", "3cc5de1ccdbe9077f1282449f9664a4b2aba24a8ab61c0a40cbe2dad183e0a10b1cc801932c76690f46684ab6307798a4372371ad6b08a6fef4b957a04f2ae34" },
                { "th", "87855c5bf78789e03c2822627443cff57b8be03dd551cd0e5e34009c74a2529f0c7ad9b209f7dc69715d0faf80de5923eff57ae2368e0eaf785814888d9d4561" },
                { "tl", "36114c2a4171f785fca3d4ce148366f6644b9a37de7c9289d2b67d220b49f39b03f57beddd893bc9c023b337784a80b8fb415071f2aa29e5b81c7525c6fb58bb" },
                { "tr", "9f4b132aae8ea6313ca12ff31f0afbe81c74ae474e0dc72b6daba33c7b5a1c6111b7ead05b89768c609f1c8fde7e7f2fcd8b6011ba4fb1788ddb1e9d3b63215d" },
                { "trs", "a2dc9d549793b12260405fae42ca69a72bc214f8778c9c46393fb9d8775af9c1166537374b8bc80421ecc0383f529203e46abb6b3d2cf58b6f21959ccef0a107" },
                { "uk", "8a52310e3c1f1c21caead38d690650b0b1975c2ef19b3eeec010f1d06229c92407b3c35c15fc9a9966ffc4ba62347ea32701fc08570bf0940e0de361c255b82f" },
                { "ur", "25fe20c53985aab466c7aa5294e8023a06845e20838fd5687ba737b84566b24285b745a59e550c1dd445d2084a1716627f4cc7de5fd16debf0c16662d6684220" },
                { "uz", "41cfb2ee32808c47586b016d2768800020ea7a0f457694e8bbd9c52a836604ec914e5b95f4f9f7a806843cae4c6cd9a70d74abdf94cd5e7c7d8b493e54c9c599" },
                { "vi", "6d74485f9bf49974283cd2186f0ed12e898285253037b08f772fb52cfeb3ada649ac735df9b9b68fd25001164f8cf6b54c9759c4bd1efb4b1f79a9cb76a52be9" },
                { "xh", "cfe5ebfae7acbe35c650c940dc97505f543f2ec9be4b876fcd96156f22c0611a15910f364b821f57ecb77a7220811b50a4afa607ac0a660916d9c0a33d73a74e" },
                { "zh-CN", "e3efdcb60cc17e0f55f8e8d76fe5d21e277dee34f86632c5601651aec4a6f02244dcaee1526c48d6096e7822a39ff041ec285a5e1eeba4c8818ccbdfce38249d" },
                { "zh-TW", "9e28b0b61e6baf0169f416d53def011fd060675feced6d3cb090b81c08088d60361e5fa4339ad5d11ce67cc1fd93b9df2de7c752fb91cdadd537752e1d7e2c98" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/98.0b10/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "e213520a0dfe209dc4a2b536f54a92375b68e8ecb5e745117496e3ed63400bd0077662d16128e9b2c1099f8c265b42c76666fa2939e50a911054423f2793681e" },
                { "af", "73a86a30b59f72bd244cd7d7792065f44c980334e92bea2b152428f91d81bd296bf9522b0d24c2533ecbe423273aa70e2c4715ea8a0f3f272d9fa9ab833a7899" },
                { "an", "e8921f4bce470be437c376978b56140d86c5b846fc29991e02368c62f9fa59ce8617f322a1053aa002b36f696f74f06cc99dc66b2a6c33b68261f528082d6888" },
                { "ar", "562a2ce2b7f3a0d404d80a0d5d187397c0c8dabb35b6677218a78074c0cd4a33cb2f36d6f8ee2703bbc33f74167fb0a02a371d520e0674c918ecb3578d0f46a2" },
                { "ast", "cdab9b8820612e1c87521367eab17f34e1682eaa7eac87a5b3d5c2066d6aea5b3ed79ba5cf5fc73c8ef00b8098a093dc4294796b79a724bc04f96d9eb3821923" },
                { "az", "4ab01b66821796fb701057f1d5084b36566305a20ee0de6bf25b65c9b7ab9a8e04f8a4db66b1e418c9a38510e34f01ac28407763ede9df691cee480a26164470" },
                { "be", "7283c79342574d3af11d6f883e9def88755151971f914ab42617c0ca90b28a870f21f4a134763aec85d7e6c51bc7979b602692ab4b20de81e553cbe670c7ac05" },
                { "bg", "0630eb7c8813dcde0edd1636ad56823a8e0c9c0e6214c74ee260a212d9b2cdba495ebc4cf1a891612ba503fa408117c91fb31ffc450bf08f8998da3a23497fad" },
                { "bn", "06c85fbeb2a1a0194cf5f6f79a909a37b9a4bbfd4f29b47e1d03d14c87af6bce694c3e321a9a9bc58b41d23a2f4cdc056321ca1b0e74c71054e54670d73e7b2a" },
                { "br", "6aaae6234db8715705906caaa214d522bfa16f1c13c8957505bab853e5e57b4b90a686d1be8c0b5ca550c59300f9e6d609b0a033652eca59b2e47e1cb26aa735" },
                { "bs", "10f2de84e95986426b868e7365a6d946e46f5f2500bb9e61b7b7e325cc1658f853b59962d40fc2254c35979da4b41c6caafe27cfb2eb165d26fdf01584d01928" },
                { "ca", "8975ac2b65a55194ae50decc6a4744f1659871047500de82da8f27f8432217af5e90215e06f9278559bc165f6bcbc21520fe1e919a9590bf976185bf9fe2431a" },
                { "cak", "e3a30cc4e30940dc065e2292f9fcb8bd459c41ad2d8f5e65ce5c1308e89ab66184e80b0fdc12430d2b4d1a1cfb27d3cdd0726b097d851c2f7209cc0d3c1cb250" },
                { "cs", "183f745cc06f9a1d73551a29a7af22bfa5d5790e6e85e2667a7e0530976f426f17f85b8fdb7af9af68835623ae0d6048e5fcaf09642b8e4587d2e521d273b540" },
                { "cy", "adb4989cb39d080d9ba4f4681b83d76205265d34aeb0107b1127b00d5e38f5127a9e9b9b48e55fe6a4af9940697357f615111f3767085b2c4900a35b6c3a6789" },
                { "da", "887309710650af5b16c647c4168ce070fae6f7b3d1b9f427494a33e2b842e30cf1ef836df13afed1c51338b9f1b604f4df32bfaac4e6a168b2404241e2fde727" },
                { "de", "d2f6a5ed6349c0cfdbf24645ffd51d6e7141381a1ef9ba9f7201723cb9866f17d6091a9b6f03bc5951bf619501ed051f41e7e2cc8cc4bccffd0e65d00ec0229b" },
                { "dsb", "e1b7a0c7551387d0c90adc2abafa31555fda4581169fb482189441ec541ca9f910ae395f452992ff2fd5f98d2af1f0d50495e8fc5c0702f290ca9ea451e968cd" },
                { "el", "bd4ec4658efd573cdd09697ad85c14719806f897278c54b51b4aa79586b1637c9a85f3a6ba080fca2e4266938a0c25c5bceb588b33350686a048b2b747ef5c75" },
                { "en-CA", "0ac29d5146b9ed2ca27b5fa53fe63aca70b93a50e6b084ba80abbb1773eb397d51fe02a317ed57431b5d06fb40f80a2e7e8a058ddfc6ae3cb8b20e829a11bf26" },
                { "en-GB", "50272cfbdba6d17797b87e65a5ed7f86d5bb13f00966d7bc99d0de7117cd5241d62fb22aed667af7f0c50fb1258027fd7591b62e6ea68967fa727d7b72078b08" },
                { "en-US", "69e45c7a6c814565d10c95b5dcf3aee6f0cb64bad069c4072af3a08ff49478da0fef25fa6baf9600c4d916043bc356d6985afdef65e6f6787acb092260fcb3e5" },
                { "eo", "10ade07f3e322ed309d83821b0e3d14376afb971ef2e0c49ead3d038ffdf7d0c4c0f2da6def597d3a65a358d80db60098f3a3d73924aea93c498d0128c926b6d" },
                { "es-AR", "3e967509e4a481bce63694471b4d93ece2f05eb14bac86f7cce0886dc6ec6b2a83a092c778179ad2abe87004840ecd7d92d18693c4c15b23fb8e619a85de60b1" },
                { "es-CL", "70d4cad81b440ca354c1ed1c385af60763802fc664e8b718472936a0cf2da96d94717dc7be0fb3ad7c380b285182fc0f868dd77446f3f81d5f7e17e690b1ec43" },
                { "es-ES", "ef27d80628b8a0bef31185c18932950129bae822c4d30aca10a4c8aadc7509e8e58ced1b8eb029885387717a3da9a478ba30a8790dd57a0a0bb92917fa5fc35f" },
                { "es-MX", "e520a3a9aeeac809ffaa63377768b45314291a760295e704d56fae38ef3ad7f791fb9aea35f3c64206b2df81283dad271c34a3b620d429bcd05ef20ec95bf65c" },
                { "et", "a4316c5c74dc4dfdd2b1da57d7b37ae9a1da1fefe0605a9963eb2f96eab4715e95689e16416b8647662132944710e12ce8ab3c18f3cc32786c07c13e1701f696" },
                { "eu", "e5b6cd56f0278daf40b6ecdf4b395798257c1d7b903f8fe3709067cec4ced4923bc89ddcbbc90de6943c7a8a755b891ce049171ef9922ad1fa2e551a35cc14a2" },
                { "fa", "8579c439155a8a9c71846f6617201c60a1c8dac7f39f8219b530b0fd985f17a5ee36fb9d7349f927df90087c708d9779d22c75e72b1765f76ad6a8e98d616a41" },
                { "ff", "851264954de50e2669f4668e43e9b9eb4a8b74eef3e5a1185a3a2ff5d06e5da14d1620bd9e44c4f3a0f8415ecd88e4ff0889e7bca939b949233cea7e32426544" },
                { "fi", "1645f88ac2039865e0a4b4a88c8a02a547cd5795383704453ea8d5cea73d361264c97c6efe146ffff37a7856a5f259152d2b8df860c7c2a62c3635c542237bfc" },
                { "fr", "fb892fd236a1124a8c3e9b15f7e6f7fd35f6307a5ce92500b54291ef9537e41c582dff31a75788cff4bedc6bb10723de16c731181b03bebaf17ff1d9f91aba7d" },
                { "fy-NL", "daa008b34e2f281cf5b0c71fd1ce52ef046c62081ca34f4638e1be7ce6fda934015f0dcbb3ba6d71b57e8c0b0efb0ffbf937aaabd9bbc13193251b16fce09c92" },
                { "ga-IE", "3d89eab2e799f95dce9479cfdfb8b036890aee50aa0182c9a08952e376ce0041582711747008f07b3339350b6f4f7d4d92d0048b4d8658a1d38801f642cd38f2" },
                { "gd", "d7cd81879658fc096e1aa8746babcd59f4a6cafd7bf790f401c40c4562eed8c74e6cb7eb0971e69bbc86de57d300de68b7ee798bd55c69eaab5c044bf6a795c9" },
                { "gl", "93b3a35e92606a23f1a543966492f292c0e2690f163aeeb1678593b7b0a70bc66f3f4e715943a0184a52e17fa743216ed2fae932bc2b48f701e5ad25be8fdfc5" },
                { "gn", "fd55787fd6b92819b36d8d2eb958d554a981889feb592b02f5ee944760c3bf3899f80ae7eb6f0f85afd7af94ae77a5c8941ec81e65381f3488c39e62bfa02e7c" },
                { "gu-IN", "2bc3170205596e909501bd5b0014adf6173259b86abd94f7af130756a8b86f64f6c6191f7a016b48b5e2c057ad968d2e0216fa0470a7fe3a453506f71076c97b" },
                { "he", "bf5cf26da93dd9ea07618b8e7d7dce05cecee322fab48697c1fdc9cb4c7c9b3157110ecc5df3270aab044c4a5edefb9c4c3b893b4b5beaceec5dc45e438a295c" },
                { "hi-IN", "cf84de4e699f04cbcdfd0b9ae843605066926dec41d69edd2911fee2d4ab2204e0f9546ca958a84fc801fd03ada2b0c1d16a73a2d2f030840b1b998a56ff871f" },
                { "hr", "0d7e62ddbc00f8822ce45fa05ff39268b12c01a915265e6da36c7d30ec9429abcb5d575f7afc6830a461e1224fbc939021b07ad388f3f3b244a490489750613d" },
                { "hsb", "c7f645f2d657badc22d579a0ecb8eada021a8f475087eb34f13b3171b0e18aa8c9d58ba02df2ee6cb3823fa154a932d7e6565aa7da10e1b3ffe4cf8322226ea1" },
                { "hu", "ae5ea06b091412cf3be10ad4df5862b74cd28b5c0f164282b9b5f30014f65d8ea6ccbf6bf6e3364059a103cdbf37f041618f4f46af930fdf39f74ea06a7cc8d3" },
                { "hy-AM", "abc8a384ac9bbafa2d2570d94c38f4be174b844edf435ed940aa623253a7d490d283d9a3c96ce8b87d341bff8071c14e9f9a41b8222fc17fe6760473de86c43c" },
                { "ia", "2d549bdcf2b46756fce19b3cfa3bc87c4dc87ba95e15cc4166fce41de1de9e7a14210ba4a68aae68a125d4dfad6af87eca73173c1f641894463762bcf35b8507" },
                { "id", "8b2a2d29c53eef6bb1332f9c1a3e6022774398074a126a769237607c2c98dab6b61a62bdc4ba30318affe818f47cf4b3a3fecb74190365912ab489334f82d647" },
                { "is", "f8ccca4decc4590a45ecde07a3f31ae39ff1b4f39fbad5b7f68ef86ce7a951e6322966ca505807fe503b32157215bde0f2a14000f14dcec7f3e8a2ed93076235" },
                { "it", "47174779625941afdf12466b57688e0cadcfc8841707c0db0b1ddfd5eff7d86fb7d118afb5eacdb7947e38d3ba0cdae2b1c268c8e98f98e6c156b028803e5da5" },
                { "ja", "03e7e1a210f913b4ed438ba809610db19ddbde90bbad4385c42730245d90e49d074cd44b655e3d2c886b91f8d6954f5103ea4677fc9d027ce640d746c14b12c2" },
                { "ka", "6df8c496d947d5b5ee057103e98434f2726adf82247ec61ef88f48ccb971463e1c7b3538dec3d5955eb40a9b12bb013e44a0d33535dd7063ca854de1cbaf29a6" },
                { "kab", "16fd7d3d5546b93c67091cac65e605870175a46d6fa2459643004eaf96fc0cb419c2844d44a6ad64c17f97954636ecb1f05d36f4428ea08b135e6be37a2f05c7" },
                { "kk", "7559be2d297035c915cff6605346c8d4b0f271e2a7df7020a249c131de5527f7d1556f8a9fe614a1d2cff0803ab9c4efba0c33a16563a501327d2c56fd9df8ed" },
                { "km", "ad104eadc03962f9f5d941f49b078b96b94af61d9d9aa14b3b6d931351cc91865ade7acfeb041839b48ae015cfd46d734582615a30e37148f6ff13198b4d9758" },
                { "kn", "16569e7596e72a304608c57f074e54b26973406ad924a5ae586c3bb37a3a12e67286aad9da73acc94a68ba0935749ecce7c804ca114926760e44f4cfe246c499" },
                { "ko", "78f88b949974b6f1750a0d32a1533a13199cb91b83f08c5b216ad7527421a32d55d89bf04d5b014d6457d83f8a1e45a5e8cf1c423c9f86fe2ff6f40b8e1d4a08" },
                { "lij", "b08ad278755a94c9db5bd3e56982082d7a786d99b10767bd5af837a1486ddd0d5f41eb04cda5e3631f5a1180202c967c13b4a0f633706a14fb6ef804209a3c50" },
                { "lt", "8992043b27ce3655b121372bbdd5cae6a4310589f7286d78e0c5a5ef7d553d11875631bd88a6fe4b4274a2fe0993ea587917b2fe1e7d6442e5a5d9a6c7fb188a" },
                { "lv", "bc07ce05fd573bf7ca5e8421699cfb6af39616a56f64597d4c022beca6d77bce03a02838dc1340905d3a70a017d197109441586c0697c6fdc7f43f8b5062e8cc" },
                { "mk", "f160da47d3ac1187794d2e2f8f0f47f1dee4f27ab245a9e08a4651d68dabe196f385eb4967e3bef6a7a169b2c01acfddc36b329d5efb700c3e8f394a72c2a715" },
                { "mr", "a940bed384baa6331606170f9b011dcf11afc329a6c3b7f0fd55f9aabe75a7bf7de3884c4b157cb127a8dd35d07c810d0c860747918354dd676d4cef06ce82cf" },
                { "ms", "d8ff552c581915e93c1c4842f045c50543e429f9c2b73f28a06647da7bbf21bda0469e348a6f89616a939df65f546e5a73fe48e760f87aba855c905079cd49e2" },
                { "my", "6aea0ddb8f814b8467caf2c99fa6ea64d721ccbc9e6c42a984a42bd0eb26700738852356875b16dbb242de6698b268f4c8a6e57b5ca74f158713696ec87b7b40" },
                { "nb-NO", "302a177a30a41a6939e44e667d6726376619a60c5bf0cc0c16eb82f9a12adb273deb36905761bac668801cbfd9ec3af6739845b02a04513eb3b9921ac86f8791" },
                { "ne-NP", "9280c9999d0ac3e370c56e8418faa1eda3b46f41ae417c622ac8b30a3ae3f1b18499c32c3552c51312d555b5aeed626d9f0f42998ee488b10173654d2a444dd4" },
                { "nl", "8c5ef1026db44525471ead78ffb46f4ea74faf85b887cb47283fe03b3c8cf795996ff0d9d2afb8bc4a055facc75093a9bba745ac30be690667e3e2bb2ba1354e" },
                { "nn-NO", "2ccb44096c3e83475263003afee0039a5db90d619df5b0b5236c17a95842e13736b533fb545073024aa85b6b86cdabe9e8e422ce17d6f5b692928d8fe367a2cc" },
                { "oc", "58d6c1b5709cb774dfac0298142b234de4efdbb4ce2a890e29afbbc47c214a6610c4ef680305fc202264a6e705f3f3ba6535a0383ebb9b8e588a1598b58c0e93" },
                { "pa-IN", "1cd662046e89b7d36979413a44ec516bf861ffb5f26ea59817e1d66e3bfdadfcb35d815ebba96447158f5147dc906da781f6f6cbb66d466f84f82e0189e0ee6a" },
                { "pl", "d5222a4fb74b57cd2bc3f3c8100c9c6a8fab32d36566c033eb8c9bf78170060b10f266b5973215dec5e0fb523db73c3e520980c57f0031a32209584845e216d4" },
                { "pt-BR", "34629765af4e3b2dc9c1686b27a65336eea78184cba80c2bb765551118ff5ced17fb3d1a3111295599f8b133a6a78ca517730fd05b27807a4a70a9c310f88b4b" },
                { "pt-PT", "2be13a8a420be2acba8d47af0a023e378e93cd1eee70ea92033f47ea454b3a09b90a7c62db797ce5cd40e8bda97d2db50c8737b6a2e865767d647054d0cdf208" },
                { "rm", "4cd1828a06a4b7498f94307aca1872997f26f296a7ecfde25427029087b52a4e201b0e86339f82e8919626f19d82d4353a499165803d2a253e65ee3d9b249015" },
                { "ro", "4979e2e91763a897cbb0a5b339c902de1629df4bb07a3097b0b125becd215ae3646e5d8690bf94ed921d64a800ff86c09f388a791aa386b498c6fb34e2c028b8" },
                { "ru", "e8692ae157317af81af1c54036d7ae5a1b697898b039bf8a6ebd231f89e4fcb19da8e7bf276d9696479e5e7c7d239f14703fe441a7ca778879048aa7b82eaee3" },
                { "sco", "3ea139f9d8ecff1a84ef90eb79b6139fee4d7577692bedf2b8a6515d9801141fca1409583852039b02dd8b04315b9be096f54f99f61b3b95e2afdc8f4a879103" },
                { "si", "f60f51f4a94b78ce61e750351e4396e176440c073d45d7434d6d92c5f54bc6acc279cc88f1c5d04e43f0a7b1d82afec448fd1670d55b68116bfe8a1fb3dbe8fc" },
                { "sk", "1ff763f1e548b18055f16735005b0e5d9738c3741e025a5a39c12f6e4826637768a24dff88bf8892c7bf0a604fc87272aca8ad5f5d43379dc240150f33f6539e" },
                { "sl", "e604bdabf274c35616bbb5b928cf92b87f90da7bf998b77e10808d87da382d29ef8994e6c2dad28b2dbca73cd5798cc65ec21f6b3515b4a53174e4e31ccb0641" },
                { "son", "f14902cd14c58924231cedcfb65391e5b892fb2bcf9c1bf364e8d94f27ae9dae0bdf7baf1e4664bd1453c920da93fb46f389032fbf22e8b0c26ddac560f6ddd3" },
                { "sq", "64d4e3202ffa9024319718edd428a1c1fd2b522298f9c57286b730305123ed5e1b016654645f84b5a316e9fdface3a649ac68b75d7000f62f4ab2dc860ad50ba" },
                { "sr", "339811263190949878f4c2f04cb3c494e074cbac0bff28274c76367c0f9ca1a2632d4797dc29a29b01dd3b7181affeeea0cbdff2b08f313ced2036f966fa0821" },
                { "sv-SE", "633d598a0b57ec17d89b8315cbd8d699742cc2b749d848107e08d89b423fa6acea496c70638d7f7441eafb6f77a234abfcbff9ccca4a29c81db7c9368d1e7cb0" },
                { "szl", "4eddf79ddaf9c05348c71322ad15a47db05750f4cf524db3f2710a057734e9788a58b763fc9115ca4d43074ca9bcbcfb93855e94b56dcd03c2fe80397a37f730" },
                { "ta", "89c075a77c2790cb3c2b1042beeb44fd962911ac32b144e2b5267fb80ccf18be159cd6c853008e44c999984687cb7e43a525227bb5126a69825141ee2f2ecd28" },
                { "te", "2ceb6d9c39889d3f37059739f7655f47d47b544e1d19f79bf21c1894afd924c27d52ad40d326a651c041dae8ee1720c748556c3bcbbf3a744701c31ba743d829" },
                { "th", "5ee5d33163016a0cdde41edb3fb40406930700c9568ebf629ac943c680915dca58ce59778756b2af317b05e1b1d33da20f97b44c82cb548f90f2d14033f6b69d" },
                { "tl", "860f12211681e9e1a6bae7a36b67a131ae8a4b22f7d42bc3f891e5411519d5f3d9369bae307f78f6a1633351cd63e2fde5cff41fff68f311404ab6acf0acf9f5" },
                { "tr", "f53a6054fb05a9774ca97d90549a9d2c005c4036f1e292f725e05ac916b186ead4a59694038bf09f66c42c068cefdbaf96df9e1e40d8d99fecea211abd6b78e5" },
                { "trs", "82c0cc60e9f10970cf5c87467003356035e04179c23deb52c2780a2829dac54627bd09cd576fafe64164a9f7c7d8ad3f66a7e0f26e74a6f0602ea0c099a16a0f" },
                { "uk", "3418e763cd4514e172b7cacb323b3a91191a3d944f73cabe28400a2b5032c0efe231418b1ef7495301026d3788fcdb9b86146d94185f31e77e1dd56a20d455b7" },
                { "ur", "1a6e0c9f1833f280c9bd7e795d58266b7001a8140e5c550d48736d28572907627ac48dabe98095a814a62f0e3e2638fe0f905d1566cf08f642891bc5c5b63698" },
                { "uz", "b0cf79a7dad1ce557196da8b321aaa5a1aa43301a937817323d3b741df1d6bc9dd079b67843343d68ec18ce676fff272f8baa2882dddab3d0fee8887c12173fe" },
                { "vi", "78c711200f5ff5fe5831bb571a5f9d0c0297c596240a470ed65aa2ade82ca21515b6ad802a9a2ced0652de5ad573548fff090a30114e869a993fa266e133ef4f" },
                { "xh", "6b0b20e1807b6715bbd9d40c7fdd2e7e57130f301591ceede02ab671be68a42cc06eb988ee1993dcab5e367b0a9e2fe8d67805fe73b27de9244dc2c3b19c3fbc" },
                { "zh-CN", "210eea9979c9bd2be5e79c3764916f096c16999a4f3c33c61acc8a55e97704241991a6368cb6f02cd2359f4fbca385ada4aa9be49811b2a71501cb9d9bd9b4ca" },
                { "zh-TW", "1cef10aca700f62876c1a4d766221fdfcea68d1226a68d91162758ab282e3093cd4c134d4c114a1dded6897c4ed0a29024f693145eab603af7f930ae52ad8a1a" }
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
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
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
