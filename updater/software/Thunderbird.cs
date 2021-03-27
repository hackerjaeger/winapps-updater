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
        private const string publisherX509 = "E=\"release+certificates@mozilla.com\", CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2021, 5, 12, 12, 0, 0, DateTimeKind.Utc);


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
            // https://ftp.mozilla.org/pub/thunderbird/releases/78.9.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "b500af19cccba12fb8c4a79f061ab475a6a7a9250731271828146ba8d5dab6140d2b469fb85e3d340c5b8848a3ed48937968d272143fed117141a0cc6c8b36e4" },
                { "ar", "5c37ad745db418fc3b6c4bb6b9aa4881a0cebb02f112b7a9a98b227ed832ee23cd989e5801fcb18b8d90fbaa95674d60e8e822f6cdd718348c6335bbe829ac9d" },
                { "ast", "dd045205a66bb5e1c02152df9775448e1b84e09089efbccee16ba494e140d14b20e0bc6ecee509cfc119246efa5b90e7e76b6ac9b016389a10c6a5544a14ae35" },
                { "be", "12507e83c7e8837c2ab8a97b4f5e1129e35b4b0711c05c422131db4ccdf1743767de7c94252198f768777e3afe853af8d9c98986c68181df16396ff0eff99072" },
                { "bg", "d8adbef34ce471f9bea3446332a4ba754286958e9f26cf4e16fe77cc10cb753b795f1ec59b55532aca55a7c8649448513f6cd8b2cc618cec231984c991c08294" },
                { "br", "5ba634a4a6aea26112487a74dbd0a51e542a6d2063329f50271920bf7387c00e45c821506893dc00881c99588d9e5cdc8c70413ac99239fd2aef7cf3213de81b" },
                { "ca", "f792c8075211d2e13cd6e3d46a751afb66e324788e1db275d8295a2475fcfbb39b84c58c694c97610dbebbcc1282aeacc09f2ba7c1e3ecc18061844fbf654c30" },
                { "cak", "04a1ade2148492ae996405916e2220dc00e464f920d55870330de081ee50400459c0e8ba87b0199aa5be0908d6bd8b3ebb2b75af1c9f74932c235741b791d77a" },
                { "cs", "f595b91d81f3ec9c3fbe8edbe34700b327ad401fce86e72e515d7277a5117bccda48a9602568db0b4da866087a03a0e396dd19a04bd26974eea4f2eebd363e01" },
                { "cy", "d8d12d2481c28ba079792c81a0691eaa46c5a9383c36285a2f605c28e15a38a43b489dd1fffc673fbafdbcadd37574c077ce580841ceb8d8f556277073b959f0" },
                { "da", "1bf339ad4fd03bca58e9f1f141035f749a245952d2fcf8ed968ead54d0a6bb9b84fb5f6f0a3dd60fa927adb60c79f1f29d10f0e064f4cc1c4b60d1a5369c693f" },
                { "de", "f2e103b45bdec4655c257ddb2bdd0b3007483d42aaf1edb209251057a93a69a1fa76472cb31edee9eb79e35ba624dff6efb1b0b6e958a15e2c880c9179a8e66c" },
                { "dsb", "2eecc0cf14cbe9522409b11ccb72e9c912c809a9a7c03744682da3dcb100fe8e75fbda76fbd92160de737136ddaa59962786b976ce4d266837d7b1f696c2ec38" },
                { "el", "88e54466c5274dce9f476183fd20020f5445fc33e677cc50174f261529b37fc5f654a31d153a32b43b3319644dc697cf6365e58925de8e001f4b32d5e2568f5c" },
                { "en-CA", "09ae2e706e6f5c84c75904df34637c085b6cdc6aec9754427c53e3af3a2ed7223822497efe2ffc9e8df14cfcfc52645e1975b90c24ca02f14706dc2978d7ee57" },
                { "en-GB", "590ac8700f64bc40fc04732d367d38aba334b3fc794efc60177fa71e6e971dc8fefad11a8ed3d02fb550dba4f91801f0f2835aaa8fc44496868104b89377035a" },
                { "en-US", "81f57a7d8ef826af1f3cdaf0fd2a9869e56170cfc95d0cd35d8581a5edbc8fde6f197a838d75bc30a39acf902c92f2028a8c9b6ab24febed42569685929a9afc" },
                { "es-AR", "887e553fedb1a2bbbf919b917c7c80a182b8c2f055c0f61e198751bbcfef01a05f095651753fb93d0e2947e1e2369b14c91500620d45745b9b3833e71a5016ea" },
                { "es-ES", "4965699f7c7955e108a7a68f56017fd69547ef14834464ef3d6b8be8d4ebeddb730de8193f5c95ed868591f6039eca242967ab59cd53567c133e459eaaf69579" },
                { "et", "bdf0e71a2f7d371cb9a76ff4a2792edf879d22eadaf83a07d1dc45ed01b84a85a74c72355ee54d9070106dedc9f8fa6437bd7535c6be232cb190879d36eac929" },
                { "eu", "38e80157defa3b74d65d7381e1e566cac5bab3b98b9b6824902b9dbd38ee45e498c90c6c8099c015bdd4cfa544ab805cc1f78af3c7265dab88a236a3fbe6fcec" },
                { "fa", "4e67266ef718484f295ff8bbcf4c00eedff41a013f6d1f7ae3c36eb32ee9db0d54bf236974bea705743a8b7f204085a526abb65422fc229717f830c30abf472f" },
                { "fi", "abca62b94b18d2027a77ae6c01507003a3806e6a5d0bc9ba8f681c38af2418fea03bdf37ed7c28bebdd71962794c4b7ddf7a71d20f740a96fba6d17a871173ea" },
                { "fr", "af2d1e15b022d1b24df50aa86ef51f0bc3a078cd2e63555d84ebce4b1c66ff9fb6b02b9b9d48703c5bf0aecce62dded7724dc57884a6147df2f4d45702d40888" },
                { "fy-NL", "51d284b1e5d20a1c37830067ca63a54500ddd4c0d708a48bfe7df98e78ccdeed65449c8fdb09e5607b90f317d1fde938133bbd5cc9c232f563b9d439b156437f" },
                { "ga-IE", "24f0817a98bdf9552b80ba7c45cd0cf9592a1e02b1856754ef9cd8a24bd95e01a28ed43356a1ec24e863a9b6c92dffbfa8543eacd82b4639e8a0cb7235d9d729" },
                { "gd", "4f487e22b08341d540baa562b0e9ba44bbe1f9698c02c1d76ed1e93eea3378d999e755c710af0809f1d9fa05c8c0d6dfae1a895d8964bca8e8807f5d15f90efe" },
                { "gl", "55ddf12c9fd02c2e035db31af2b3fc6c1d1de7ba491627437c592f274d7836239f1d416ebf13427b363ef54c3263f2422f7fe7b64ac14066e24a1d9cde0077f5" },
                { "he", "f43e867a6548da396373df32b3a07bd7c3151b4963864af9d3405e134d16d2613b4f6544c75a8a11006b735d144fd52fe8740b647d0e649a214fa1aa25cabe59" },
                { "hr", "a421ec8fb0f4ffea6a62d0f69c8ce34167c906c0c8523bcfe875eee1090e62cad3a9da7950eb816e8fcbc5f4925ab954e573739f086bb8765f154f0881b66bf8" },
                { "hsb", "039c35cdc1a7adb3a0a0f3cd7141dc4e7b8f09770a6c977b311218ec1b3881d7e4caddc615caea87f78a75f4d804fa638f7417fd9f70d2b3c67c57158dbaf6db" },
                { "hu", "8194386dd07502ede2b1bee79ee3b0238a9c3d8142bfcceb139e80ec97cbc4550f25b4a42c98972498b069f80c4ec0ed2c13a23f2ad171e8a926fe6c607ebbc7" },
                { "hy-AM", "2a544e77c8f0dc95de4e5808aa7f3552c6ccf2acf79dbc63a3e99a3562f66c382a4431bd191b4b819063f60ef649b95792c409943161bfa81467529a2c24236e" },
                { "id", "f465afa59ed5229e7d37171e59379815622fbbe29bd6ff9d342e8595605eca645d3ac47d83553735a5b00502460e23a46637fd8583e9eaa906c95d61b874d078" },
                { "is", "dc2e93888d578abed14dd5839b3a513dc477c2fd9a763b77a880ebb394afc5cdeae84f27c51f5873434fd4ee98bd38444c74f814fbb64fbaf13281fa34931930" },
                { "it", "64a0e2e78e175f5a1c99befd41caed813463b9d34bbd9896c0fdca0e147c3fd5c531f8665838b5db9723e00edd187c7bc8507df17d0802f50e54f669ba1b0aea" },
                { "ja", "fa0ad11f3bdbfd0f758bd74dd6cd1957907d3eb4c32f07b09ca87c2c5aa4ab963fbf9909147e077a71faa697b97221a024795668cd793504c23d66960824a6d9" },
                { "ka", "a190309417ff01ce0af5af2dc9ba15013d6424426db7aa379ff6e0b5f4eeef2b63433ab34023486a89cc38c75f0238355d4736b07d15f6edc4f80f1fd47d8764" },
                { "kab", "b41260dd0db6e448572de64e516a848a43e04a7bfa6d75024a60d99d163e4d36f81e6e42022b62ff1eda4a830cc1fb30e6449c5167e27ddc1547d6f11c0eb534" },
                { "kk", "9ababcdefc158012b85199d4c3971cfd3fa3c54b27af11738b9719cf4d2f73317610115e6e3dd962c90d422f558614c051cdf558139f54e160a6233220d51192" },
                { "ko", "048253c2b9ee66ec8cdd21718942fd926ec6ef4aba50818b8c9aef91738eaf0b147a42b71e7ef92f1bd275bb59e51487518095b883ddd87f29cff4aa47f0d300" },
                { "lt", "8fdb0f35be5c1968bb790a3e3c85b25b9e2e54907779b11140f9e3362721c6215ae806b3b4f7a83a21b4217bde13686949ccc1f8bd22d468ef31a13ceb0c59a3" },
                { "ms", "a57248c21c68c2a87422f04bfe8d77f9b94ec0d92e1fa55e35b7541e6f8b8e9aa24802e74e9a719a6035ea821ab2abafaf84546b19af6ec3b1eb2ffa806e0234" },
                { "nb-NO", "670f8104b432bce7ae1f4f15fcdaef8ac95c9c39dc5681c008ac09172fd252fc0b749e42455a10f0e836c40da1c240ba503ef74b693e1295ee0175593c93e573" },
                { "nl", "23bc83eb180971f9156c1b18101e1e15b1b2428a5c35eb11f51a507e681be9cb90f3c10a85021a77c3ab77cb8f67ec03b0350f568bc69236986070e7ab1bd0eb" },
                { "nn-NO", "9cd6126997ab42a4e0bc3042eea0c0cf9327f585ac633a383e7e9425ed32b441d5e26502cce157cc4f9d3e9971c68dc5109a071b5b693590182f2c92eabb7383" },
                { "pa-IN", "c40af151889b1761b0222cd0ae807dd7dac46edab22a32e646674a34d7b9f48126e735e6e6254f9bd4d3c38beff16965e7d8b098c76e18f54d39443d72ef977b" },
                { "pl", "630cc5d2bb1b7ff8c7992983f0be6f260c02e614b76a847625d4dafa6b311103ff4bff5a7ff920993a02e985904561abbc7233914fc86a4c575e5d4a55bed1e8" },
                { "pt-BR", "d02884996edd84fde9d319a9697e0c8cd462996e161f88e7b81698e15d7bb834d2447e9b5bae672b1e86860e659e3fb8824a09105c17d8a6496c55e063216a2c" },
                { "pt-PT", "8bcadbcfd59264f79776bbe90d381ad5af87d5c21768a46bf532f3404df108b8dc56353ed1d742008aa1d5c29bc21c1f5c54fb0a9d33e7c7cd534801b6972e6a" },
                { "rm", "fb10aef6f315f02d49538dfa7a67863f40479d1eb3338a1e5abf221ecc64c012fe5379d07bb5f309c472109a4d9e13552637381d88a9d1ae8337a2778cbacb88" },
                { "ro", "da80ad04ed2b7c12c7ed2c4c2a5be4e605d2760a4b285ca75acf1fe5a93ec81995eb9d323dce7a0d9769ab38d2743e6c2dcad00754b6b4be35342631c0daae87" },
                { "ru", "6a46fabb130b98e8243ebcdd7d9b98a602dcd0bf09e973ad96e5bb9e0c394134e0f67d3ae57b31b73896a6960cc6bd8a1c564a2b00238380f954e7c97f0c8eed" },
                { "si", "e2420fcee1aa9db472c21181a796f93407651219130de7488903262075925d55b6a373d0db8b1f782fc57e56343967c7f52ccad16029a9b9d58e3552b1f9b2e3" },
                { "sk", "02303b84c54a5ab62be2c695ca31599e329367b619f50a50d953419f8066b7cf0004326a48f1dca3029103bce9bc997309a865b13f220c2cb4b8c98c16140916" },
                { "sl", "71596363c196c0c043754b3782adcc50c26f8c4620561a2b61a17089a17f101c39387496f7ff30e578c18905aa38d3cb9c51e5bd68002fa4c57cded2d689fc9c" },
                { "sq", "fb3a6b4f0816f0508730856f1de83a1eb95f3d3b8c061cbd67aa78dd334744bae82709748509237e615dd8cddfc8c9741c4ef62d7fd83eb491a2d651889bd132" },
                { "sr", "a66c0daab210ebb00c7cd712879a872bf1ba8e9bb77ddfff503294cf75a67fe2ef0733e3237d4cdfaeab3a86236df59b4c8cf94ccc11a6ea9ccd7769a82279d1" },
                { "sv-SE", "d53593af7dd150cd3db488432bbb410da036b30f31a08169c7b370967fdbefd74a1d30ede762e58c8f4b48ffd53fced6537f3a76c261f82e2620c2eb84a201fb" },
                { "th", "3bc8813c5a334e3cce2b5d3d7d6d8fea17978b203d748b6ac73f3925cacb8c721ef180695ce6ef51a4561d789ae270f9d8deac39a24095bbf0d7c93ca0bf7f81" },
                { "tr", "aa6b37a960b46cd9bc1409c0258a5d2532afd28b534e7b249b4a150fb768854e8abd704f1aa6a8faccb4873e8aa2c65b2dffb884cf3c9411867667847b6cddb8" },
                { "uk", "90427c13ea05855d54eb61371f9273a8e5ee51b184139537286092a3ddd520e4093b1a86f7d77eb2edf3710c46d6cae0766cad5aa7aa4ec13f45dc5360e90778" },
                { "uz", "1bfaaeee430efbbbd4f26a25c734e36a14537698fdb328be8aa0a72f78035feed461b198043ce9ddbdfc09e7b370ec2e665117be08d27458a9d080eb5641b75e" },
                { "vi", "1d353c2b05636daa833da2c13155d7f972b32cc71e5841cac5de23685196f60d0cf5c5df858a46b16bff68aff1f980714225bf0f9ab515f342faedcf21e86961" },
                { "zh-CN", "9160ee5c1158faf3624a85319db6f0f52a9aa3de072f26e8a9e0e0b8bf86aeab7546f68b60ecfde5992b60d9db00ed7abcbcedba517774622eaa4f6bb79dd853" },
                { "zh-TW", "ac0cd7d63afcc606c003c42782bed5f4b8d22a08c68173bd9806bf12a6ac51276f85e2bbd88aedbce90daa1200a053322372cc549e0f9cd7338875ded6ffe1b7" }
            };          
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/78.9.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "79cdeb92a66843e29eb77af1cb02f60571c6b014ee45b427052ab57b1357693dd8e8a6ebd998be31d9ecbc7f14e58043e6fd12216e2d5e76165f37f1f87f654a" },
                { "ar", "1886fd02056e63c3136062a2f8617fae32f4324ed29796993fd87ab9bd80e357854f8a7344159474fb4c414a1d9d1b0b477b18c005dc3b77f805f8ba0b23076e" },
                { "ast", "d8fe55371ce4a27e342c68d1295e4ab05c27c55881190304c64c32afd617bb3ace56fc2e6feb286812742d3a2502c932822ca1f4e1a0527ec8d840ba84852e82" },
                { "be", "6da003136cd4faa098d0e895e09917dd59e158670c955a14e3d942ac391f084e92fdb7067fda5694feba8ee3f9603e72c58628bcc47e84830c5188e28ae17c3f" },
                { "bg", "2d1207465e75bd2ea8444844a16d99cb80813ad12f446f4064a068394da548905734400d998c4a5c24fbd71a4ebf01818dd812eccbfee65690e1e4eca1144c91" },
                { "br", "15d3d30ff2c250d0436882269c5fae1d813d373d6a5d84862119d17e7d6b1af956eb7332ced23923c72c61cc70ce48e9e31eefb63dcfb803659b7fe37c70d12c" },
                { "ca", "5e090d7578da3439e2afa68f55b01dc037803097ce23e899f02546cc736161c6da3a74434116c4a9c9924ae4eeba69365b51bca4afc534e0305869f5410caeae" },
                { "cak", "31be09d794f676085b61d4fc176cfa7323a50c3613b687357d9e5a522767c97df075a4ced672e0969ce6149aa425862e7679dbe9c07582710c195e32fd3c862b" },
                { "cs", "ab2fdc7a87ee8e59becd25ccfb6827a6f52cdcd4f4f3cda2bfa54140f1277bfb0b5b00b7e29d83419408cbb9afff5cfe4faa0e3c68393b08123b81abdca86757" },
                { "cy", "55f4099c64a97244402a2ea344a8d600673b5d34ef085eaf00837638a40341a5239b4ccc004de1f2e442b773a412fc25701bd40305cd9438cf65486b0ed801f5" },
                { "da", "1a0498e32cec830e9a4db079113c5f3df48ba6698f1cd4cccee6aecc461fbd2ec49665aedfb38f8e8c2a0b95d3c30a1398f7789610dddd49d3dbebd06bd6db58" },
                { "de", "b93e11d6094cc563afd6a247919ea26fcfa7b3c4bbd8407f858de8b10329512906c6b7723343c7b6b149b1321b361aa2f771d19c2efaf086cdb6a2689589fc3c" },
                { "dsb", "7b55476481e636e8c6d2438400ad752906cfbdfb4fcfc70b0d552bac2e6a8c5f3614b91a33a1bdfb6ef3f136bce24cb98ae1e253e1dc7d94ffc655b1c896b5b2" },
                { "el", "a376bde61be4ab729e6c565ef3d953e6ef9db78b25f0b76eff895c95a75352cbf43986a6e7ef97c28e0016c5d8380b1343935369fbb1e46aa5ef94d84a438753" },
                { "en-CA", "01a9144440a58b917e8ae98dd752f9c0e17af6f7127a16ccf2c621c81eedc04019a40d457630e7b87447d64a2f8ebb332dcf4c6300bc679b47e76fc78f9385d7" },
                { "en-GB", "b3b31e2ae81d9317407c9416ab45b0a9fb01d541a01ed797bc0922b81544b87037538ac979c0796f3bf7edc7a79dc81375b4669fe224e2903c51de5dfaac0a1a" },
                { "en-US", "846af6c6afebcfb2c114bdffb1122d3f1b61f06d2490abe6e4c670f11c80de76c36220a734bfe197530aaec5cd7ad38c5add40a180a95f800156c55ee54eb514" },
                { "es-AR", "d8a47b90fa42da000191e59df1d06f68a71ba59e10cb5d90b5aaa3b4c331deeabd2fc15c8cf59bcfbc9aae6871ab70fa44eb0d69537d9de6e20edda7e79ed712" },
                { "es-ES", "78dd5dd7f892f2a67d4070231aceade154d1e77200dbfd140b5a77e9cdfd59e1389938c122c0334a52629fcf5c823cfcfbe12668b9117db2ed581800338dc21e" },
                { "et", "b9b349a61f2650cb54019482fa5fd64ff72fa904b1a4e022ff1b60aa54e2a1e60709c34538abd22ca0ef0d484b6560e36162b2d112537756941d1e9fb6cc0807" },
                { "eu", "68fea5c048770bb6c5b0faf21d52175d988bab9b86dde78da65877b2987189cafffd0309af0767f977030995e44ef46a4715f0ad7c0967e991c3c6c2db525f7c" },
                { "fa", "079036d868a6a8f82d92fdf8e8fee3c06512f88bbf5ec3affdbbaefb9c8db41094bf2cfceb63a60479abbfe079fd8bf93c7a6651513ba60a3e69b4e851c64568" },
                { "fi", "a586a5216e6586b93b5118c4b40253b2b5bff200efe5f33ce752efeecdd4b4220b53059648557d74c3daf7e22aed8a03025b774d2a950ccbf4e2d5a6dd046de6" },
                { "fr", "46176e0dd3086d0eab9d547fa96808e7b158b5ceebe571e0d21d0a34553e990b1325afd22769a8ef1ddacadfb2c9b2ad58f4da807c1285625e4abf6e61ce60d5" },
                { "fy-NL", "b336c5e02bf9648d8c287ff9d4ddaf13eaf410551d169672a5c01c6f5f06aba652aeffc623ba9cc0c399bf1f1c81091b6cbddc904317c156241e3c9b77df0fb4" },
                { "ga-IE", "a3647770d48b3e30ab6ab02b37be40c306cf9c28c1581e28cb3176470d133fc4469789dbbbad00bc7b0ce5d7e3191360a696a0af1906272e3efb9f63451b94ec" },
                { "gd", "cedd96f23363f04fa27712bb7ef55ca83f936c89eee3763cb2b20d1d328df12382785a060b919c9d5fb44051e517779ccc4ff2fb4ee49c559085117f4e7f3415" },
                { "gl", "d23e78f4cededbafc139c5c06796de62a37c7a3fdb59e9cf287bb01342d41468273ea4cfe95dc30c03e4bb37b3fdc00f477f9110864e97913748ec5d18b98200" },
                { "he", "2caaa293580fab0840efabd9cf62bc164c3fb96b5a778a39d3dfc6865b9311e962770fbabdeccf7e8639de609a9bf81f82ddabc2bea6b0070e1ece14cec142d1" },
                { "hr", "d7c0caa153f318af0d91509ca0b2f0f714b8a1928b9089d5cd10ab9ff46a5faeb980c1144ffdb96cda72850e2d3ca223c2006f0c85c474ec01eff696e0999f32" },
                { "hsb", "e9de8fbcde880169e0bc6ed06049d7c0684af845aeb245f3e8de4b801bd56260ef5dd7203eec67794478a20959d8da40a0a52060f426618a32d04075346f9a34" },
                { "hu", "1b66c1fce312db31956b2dc38193bbc59e1f118dec22126344c1696274a909ef9a891d1a1a271dbbeeda525db3338b0b8d8e331e641c79324e1434bb327eccb7" },
                { "hy-AM", "dee9f19fb5fd31a99c60718859dff3991a92de4ec4e7b079ff4804bae50119199fd2472b0032c5c01082ad3ec9c9a86e5187fd85e0b1616498b3a37bedfadb26" },
                { "id", "304784268c441f74254322116b480297fbd35bb2c8d59ffbb4794870edf0500c7feb96d9f1191e2a1c9d80d5fb03037745776ea4c5197f7a523a4f9a9b6e4aac" },
                { "is", "c755190df343333a2d96126d545b61f1f8bd64ca44101f61d6009d909954b52ae20fac6e0a94e1ef7abe8746d19e48e7a4e993e253738d92e3e15675b94f80f1" },
                { "it", "a33941f06db158c03294375246b09eda3dc937d9ad1722dfabdd47cea66bd482ac0784c8e0f0fe18aa764ebef1b82e5b1f7b123f057499280eb406b38013457f" },
                { "ja", "0726a2647e3abe0dfd92eb475e3bf2df71f2ea2ab9680735d9c1c147c5155ae88fd7db303292ffa457eaf8b529ecd6e3f442b4bc6ceb74bc8889fa986a734961" },
                { "ka", "87391a4dbea8bee90e202389c77192e2943b173a5724711e4a7a46b5ea45f3d542010cc73ade94f9b3dcbcccfd167c9a84127266045eb9db63eece4186836d24" },
                { "kab", "eaad8b5a530639dba9aa80657b894deabf493cf227b002b28b839d83c95d1d236454a866a717c082a546a6acf95e7088a5cdb4843db9910d7eafd4346e824a09" },
                { "kk", "6d2ec1d2a35b8933583414e899371b3c2da9b259bdebfaa6693cc4d1b68f680958678c6b62a035c053b3007cffd95debe12bb58aa816bcd3d40fe5d5747a5893" },
                { "ko", "719442447e8d9e8a9dfe37671deff2fee82b8234f0fb28c8d63c90e7a0c4e2fb39564b1175eceb725f9d16eb8e1fbd1e6e4d3bed5ad9bde03a00c8f46f7eb041" },
                { "lt", "fc74c9bc8e368d19902b992ff119548dae6231714ea4fc813865a9b201a476520ee262e904f810673fa827298f087d219e48d91f2e67409ae661e9823f7ee5a4" },
                { "ms", "88d7d77bed8e21527521bb2f7ea9a05abd83ed34ed64b7657acb25cf9d156dac076a735161ab4643e3340dc156331809db3b4420c59e62dbd57684774165b5c5" },
                { "nb-NO", "7c10edc4a770728418e80d9b77ce2400bfa6925f58b35c13ef171a72142d392b0c66de25c2a4313e8fcfe85fe7399360452179cc45d06d52aa89fc5fd89cdb1e" },
                { "nl", "1fb0f20c933a6becb794db58564f881d1bd5cbde234f55b3f48a0d5407e53ca32aa85dd3917226e751e48f6f80d563961eb163221e341ac276e8f36d94d05dc4" },
                { "nn-NO", "576ac730b131b88a42e8381647e397ae0388ee64fcfe3cb739aeacf7767f37c4e40a1f106abb949a4a4ecaea0f05b001d811fa388f01dd0acc4e7aff820c37ab" },
                { "pa-IN", "59430aa74fe3ce49d56bc7cf9e7e085d63c42686afd3e0303bbc54db567c7f58f55f362cb66440dd4cb219f27372683b4fd2508f643d2a5fafebed612a66b681" },
                { "pl", "bca6041a3d0835e86c64184e198881cfc6ac8642e18fb04e60c783ee6e43d3b3d2e906f9dc039d6084a1914beae2f8e0b86d7630f3c88818fe6d5dec8568051e" },
                { "pt-BR", "839da2140813e28a6627a2c56ea6cf97dbc6e6d9a770d5760ce64a248809b331fcb06e7fe824779d4fbbe7bafab279bb76b1479e7b61c74388e4fe9f32899a89" },
                { "pt-PT", "77a79d751230e49a9fd18b8dc4f98de2ff794c43d294ce39e329430cd8649af27d5a33fe70a2b2d26abb6cccba8ca0514fd7100a272fa9b6f740dd7ba0047927" },
                { "rm", "d5458a6dbf4b94523d1c40f86705e2fa2b8fb3bcb6064312e764fefb0568169d55b6f6dd4d1feb1c9d4516882157c8e1270fb85d69ab8fa888f80c1dbc8c406b" },
                { "ro", "ee869784e8b680b5fb3284ea14c705919e4c21f879089926a132ba1ef0c4c8ccab642023fdb5a87632d8c053fe004819dc3cd92f27d8b037cda06745b4a4eedd" },
                { "ru", "f4fe385683ca888595cb7d70cb53f9fba64cc0710161633c0145aa9838aba1117ba48a94018a7e1f31e2de65f18864276f4d72c0adb60289c7c9aba1a637e1fb" },
                { "si", "648e349ea0d31e4cf45549b83124dcdc3eb69d61886a61517412738ef0341515def620f5507c68abfc0330e8ab46ba8b5894d129609ef7230c47d68855b1a107" },
                { "sk", "8369046040a97c7937a8b36e53170d43bff0f93222a0668f48a7956cb880110089096778ed8eec9f15689210385007cf6160760a21a1b8ec7b934fb08c7db714" },
                { "sl", "3c3cc9393d8ee29e604f29f964fdd2eda59295a1fb0eef3ceecf3c0b870894f4fa8fe0c9faced782709fc551d2940a0921ba0142b9f111dd64ebe9f706065b53" },
                { "sq", "a49fd2658513d0ece75dd3eb11a936cfd9bb9ab2cf6886b6ec43bfad2b157d686663bcfbf62b48faf3d34401ad85a71b3d66e48f7b1d492ea650f0ef5d8e7fb1" },
                { "sr", "b2428e6fa1a5160991c930c1c9bfaaac23c90ed4d9758ac9aa8345fc4923e129e82344bb212302e7e6961a02dbdc26bfb237f87b1c7682fb4dc0ff3784d9401c" },
                { "sv-SE", "c6c650dc2b07da5dbb8104de292ad9a306347cab103e12943cf777c361d06da000147c226d22ebfa5d61a27ce77d818c838dcb8704a9a568403dee84340c6ea3" },
                { "th", "35bd090f26f4a780cc32f905315b63e7d0de7fcb6762994e64bb89ed1db4959d484d1b96658b172d3e7d2cdf92408a11090f2f9f833d9f7c36d0e540d4a174df" },
                { "tr", "bad10422c8eecffb9431e74667b455fadcbff1b11371480830ec5038eefc2820fd1c1bbdd71ed44715a491dd163481cc3374defc07fa9953ba718cf063dc519e" },
                { "uk", "9b4d51c32f15beb4eaa5c242b6847958d796f6e73620a1fa64e8e65274cebc6e645041697268021edf2644aca135dcfe5f54f84ccb299d1d18fe24f690e1a4f4" },
                { "uz", "008676ce957883aac35deb168ceefbe6dbf3ca7071c1de36e94be848ae115c6b8c23468edeb1abbadc2c5c11c89b8907bff58a7550d6b8213f2a0b54f99332be" },
                { "vi", "0a2c497f8e91359a1ce958a94f5f0fc54e890ecba5c15d9e3faafa0266a12ba6007a8f6969cf404fa40ed1507aa31623f5d2e001d9885475b8d396a61de3d0d7" },
                { "zh-CN", "e86902fc431c4db597a1dc141974a6945642bbe119cece4ed8f648c94e9f89f14b3ade1942b213c96df127205364b80b0d824e6f5d795d867a522c93f6c38256" },
                { "zh-TW", "79aaa5134eb4b3d5152580cc3b9874828f226f6e0864a4f749c995d57f6d187fed5a1e8263edc98b3ca9ec0f813ffec5beacddab3f31ceea833280c5d5e1806b" }
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
            const string version = "78.9.0";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird [0-9]+\\.[0-9]+(\\.[0-9]+)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird [0-9]+\\.[0-9]+(\\.[0-9]+)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
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
        /// <returns>Returns a string containing the checksum, if successfull.
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
            logger.Debug("Searching for newer version of Thunderbird (" + languageCode + ")...");
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
