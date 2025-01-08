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
            // https://ftp.mozilla.org/pub/firefox/releases/134.0/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "abc961d5483832100db9d1a1c02571247243636e2a9725d9ebf9a86c65ecd5d80d42fefa1a7ae851f042dad2e8a500bffeb9840e082cf34008ce2f3bb893c6f9" },
                { "af", "f0b5881732ee92523c0b1512235c9312110069ea1bf385a7af1b6da99d76854c375288c27849a0a959064fc78bae02c1d30d54ce3fc930c969a5e7cbd5f141dd" },
                { "an", "b7bc246991f82c0758cbea00274691d3029a2e6956a1241f428242d677a672c86d2a3446055a04c3f341c049c4cc8500445076a3a8a8613317381512e75ccdf6" },
                { "ar", "2441f4b22c96af3a3e6ed0ba74f7af2961b372b92fb49b0f4d4977ad25e0e523106cb4309dbf0be1f3273161381bb2ee047005b07d683021bc3bbb14181fa054" },
                { "ast", "6673b38a812d33df53f893c6cdaced436722fb6574064e4a7936c0a424e136df2011221b996e500f8fb02c803bd24ef833cf8bdfe3901a301f537f03cbe47fd0" },
                { "az", "cdd99198cd90ce6463731156445ea9e70e85a28f8e41c910601827b9c9dff8ed8fe6f21c1ae1ed6f269617c499dc20ae92ec6179f6d8ec057b4b8c825dacb063" },
                { "be", "cce5fd74506c5a5a7399b7952b6aa47f27716c5972019e4ecd3fa1b59fa803c75cede360d79cfc156ff1c0c2c379bb9c0164bb312373e37bd7cc009a1f0fec94" },
                { "bg", "95838f11d1862bca609a60de51c7e7fd9fea69a86e6b5571eba19b60487d7bf8ec1cdadc087a3d1b11b00ab60bb8dee6d38b424de2af5565585cd4eaa79bdb03" },
                { "bn", "77813e4b755d31c3d4f94b47e236e6d5c097ab07dc56d1b8304d13e0d767e90dd19108e1d00a822cd9c7d254fea0b7770aa3567df5dbce93453d91fcff481365" },
                { "br", "3d46a998b972480ce5f853acf286e18cc221ac143460508211d6799fb1acc2e71af427cebc32c9c9570682c35ddda04cec08955ba86871de7d3d8d52addef6ba" },
                { "bs", "620f3e2909e8a41070bcc88ee8a417fb668411dbd72e53d57c2d74cbdd53b90504fb5732cb4765ddaf10fd5b267f21fd2fcb388a57697a1f3b21fbda21896a0b" },
                { "ca", "3d64b2182dc6847d9eee5135dafa35bb68e423162d61e2b2172878ad91719e58eb59e93256d09405444cd8e6b839d6ec02d009dd37f93c10a532a2d5da17b9fc" },
                { "cak", "4a33bac2716a505676d1e2c0e1ac5026a403482bb4c040153e2ff6f9cde16132ccfe00967ed77e56d1596db5d4b23856cd26a040a80c7407b09b1d5c2f4c371b" },
                { "cs", "0d062d537ba221005710c2e017b2fd1a2ca37ed03b95997ccbe3f77beb817cefc74a49b01b0c6a80b7ec0ba3cb73fa8872fa04317bfedc74ff5b3daf4abce7cd" },
                { "cy", "13bc94a7ec09679a2ca147431b6918f5e90b698e4abba26b583a7a15b26ea57f979df5d6729618541df2595978e4579fd4e55deb65bf48f33a709f2450757115" },
                { "da", "e9465404db326c7576be5f140cede4bbd33c837e64630205259cb28ed1ce4d91354ca254d43a2b40826e374ad998b17f968fa2f5817f1cb9e2023ca3379963b6" },
                { "de", "d89b8aae95c47777bf1eba25ba6a624e37d90678f6a2597cb1964fc27dfd6423c003f9c098fff59dcc4bf73a31986438fb77f11bee159a01b57996ad4e68f101" },
                { "dsb", "c2f9ee3c084d6a2c7ea4e95bf9e6b22367916f80611f9adb855cb7d59bef09a8a466d4a51a88c2b717cfef9d7d02e32c491ccc6afd604f83bd211a9d815bc10b" },
                { "el", "671670aaeb02685773b46aab5bfc8abac438e4441984def746ef7b032909ce5ab688f2efa3b6cc97595f7403a41b525a8dad9930675500c011d886474465b348" },
                { "en-CA", "cea6be0b7cbb004af8de54ffd9bcf9138d63971a3d7075a6d8ae4b16c45139b6bec59617471816f03c49e0013e3996d1b382b4d2eb681e36dd800358581cc189" },
                { "en-GB", "f482c6c8f60e958927abe228048871cbeceabc53f3d5e84d48e190194f7ec02642627af800a5a724dff16eff9d33d446708585a9d3685e758fbf2d1c53b417f4" },
                { "en-US", "5f7c5a36648913b1eefc4db62de2a2ff2d76f2393fa93e82649357317b740ceb155aafd623b65a464a12a9957a2f6fe9b8d7ad468127663b75d343a6c4fa474a" },
                { "eo", "afed19eb9c55d45988fd446859d716d06f92aed201b156c0c2c5e42c090d324ba45d480cc0f9321c37dc7fad82795c23049fcbf5c1239d41ba1f84de5975a556" },
                { "es-AR", "c0a0a6c3dbd399305cb142b0ff29c482fa34a4195f01b407efad23b9b7cd8967cd99b9a0d4e108de5e732eda84360703ab793a2382f556904dab52729b0025a5" },
                { "es-CL", "3a4e33421af81bdb9023d14edd1f4b7dd74a2b50e8ac2a6132a9a363e6b53efe8cea84fefd87a11b445fc66f49ffd645113074c7d2b59e1edd7b810a2abcfb4b" },
                { "es-ES", "bdcd777d7ca4d4aa8ea9f253a071835dcb092dae320295247cdf849678b2f8d1296a7f18a8bd4d65e6178241ca9adc76737eeefa10d75d82937a788933ead288" },
                { "es-MX", "5dbe8d64a58956792bebb702a6466b090b644d16c8d636d328270b12b11df657dc0447f5ac198f0191b0026e20a485c22e8692126408c865e0fcf80a9d5a72af" },
                { "et", "5289cb16aca414210623da0c0630fcb8ed6cbf83cf385915c8462f48830e7614217608ca0704901467aee92810a1f308501a8a60a80e695c84a036a5204700bc" },
                { "eu", "535f8a217c8d0b179bbf7c708f55ad96122ff560d326ecdf7faa066b36a9541083e8c5a27c2975ae6815ecb3a4900b5755ce22916b6b1146ee93534a59e87d49" },
                { "fa", "98e9235e402d244e21a9ea5695953ba75597ee878437f4b93d5f9dac9ed276c4d6ce7a1764587771f7ca7a1419661c788e8784ed8ed5b68559a16287a7b61aba" },
                { "ff", "51b355c7bd1213090446db612977fc184187a1e5e53a95ad8c6bb84f10862082c21bf01581434567525ef2a901fe359fcf2638c8fc8cfd9feca053c4f6a0a616" },
                { "fi", "517c52861fc914457fc4ef5c0063f311c51d2975f5426b98531ee5bce3676e2763a9e864a6915fb352eaacd5ffc013a4e5c2d7a233073ebe4be94c21e79b9c5f" },
                { "fr", "50136ec446e2076f7278550ad5c15d3742852d5820b0c7d919df9c293861cedc0494a367afb45d6bcea49b1b562eab45ac407628b361995461e004a7633d094c" },
                { "fur", "125f51a1deb3c9c9794b4366a2f26a722904c9005a48922036b02b49291dbf1016ca6d026a407c1108eec5626d5ea5cd772125157dd27416169baf04de870926" },
                { "fy-NL", "2de8f7a752f899feeb5707fe3b253049e38c5ea561a21d71754507e71c086f9e00acc73461876366702e4595204d5f82696006032cbda3d245e167ef44e563b4" },
                { "ga-IE", "2572ba12699692c0e4cace26bb6b4808f20e7b449ed220d2762e04a5ff68ce4112b071e2d468ee910e3d66f3cd033e39c7527f618ac18cadf32b8a901caf6858" },
                { "gd", "bfaea07b525210762d91273ba2d2231b7e10a4d89f3c2b0643fa315aee81824881fb9b71eefed9d4a513657e244a5e75d238b53bf59ebd16ea3d9872688e3649" },
                { "gl", "63c10b1786e32ba56f4dc6f26f57a7c582ea82c5cf87d3e071a5e5a1f059858bcb53f77c1916a21f53adedab9a3d7d961d69bf65b3e78d36460f87a625ba513a" },
                { "gn", "34acc1e48b07632b92fcf0207f94ee992e700f344ca53623e13a15979060a0ef3a45e5cdfdb7012787138b78fc9798e7257389363f25069902cd6f237e3c867b" },
                { "gu-IN", "b933de64f59afb5159a70ef44f7a27be678acc5ec0401570694dad961298b50d6b52b393e52dadd5bb25f01da2b92dc4b69689a486b3da477d7a7ad923492987" },
                { "he", "28247d73979feb40d10f5d17e7ce0469cb3a78f1a69812585681311301741163bcc036d63b7f446cb570b27a7c7426189cec07638dca22b2ca5b54466d50c94f" },
                { "hi-IN", "e4622463943353487ab9ee5ec774392ada887a0c1e6c68ec7d99928aebe6a9e721b9641f39569e722924e6ebe2a8fdffa37330d1281cb6603e333b52d270503e" },
                { "hr", "5cda2cac4f8fe433044eba9f70dd82715a9eb32c6c72d609a4cda6b55bd540eb489c5f54e035669e1c4a22d898edbe22bcd9f7f46e133b0338af7e8478b02395" },
                { "hsb", "1510d8e5a436534cea716d7b13505c5e598497b3b37d7dac46487af92c009c1a674ec4f787da2fad3adf12eafd340b3454d281e09496958d6ad26c621bc1a734" },
                { "hu", "798fe550b9056706086ebd12777289c300072c381cfe3dfe984211599a654d0c3d7d720c4fc585aaa18c92be4f22d905337ebf6c2b1d54c5d0457412798226df" },
                { "hy-AM", "2d9dd7eeff693a52549e1c6183ad84f958549f7ff82b615abcc5ff2ec6ac9679cc899678695d9b66f88365c33169767dc49f800716c216fe884e52308b0252cc" },
                { "ia", "0373d9e77c340f08ff4918c68a50cb15598860b38163c4bf3c887935ab92585c55b1a88feff7f7893e0e12199f953f7134cc7be34066cb1d393cfad0de439021" },
                { "id", "763651279e810382f81f304a6dba927e76c4ddfb9034292abe8bb4e9791620ab523942204b1560900d86ad2653af58de70895fbb8dbd5016a9e87eacdf6c3ffe" },
                { "is", "9ae5ecfe2649add33db84952276fe49895a498607b21c55999120a9fb7187eb52d4d001ba336354afba0e82f4d3781b10ffb764767304a004ea7ad4657fddc94" },
                { "it", "c46f73e8e480becd2df131dd8780e2a2c1e32206e1e962396931c331dc80655e7c721be9084f995e5822f0d96b483330115fee20efce61931027adb72f2dde6d" },
                { "ja", "bd4d1a980a3d73eca8e858ce793f1c5ce993f7fda9f10abcdcdbaf4745949531867c0b5778f953e268b4c946f8f872aca98b5e11f3ab0bcd2e4b5340c3ce488a" },
                { "ka", "2bdf3b4efa8cc7003e52b36a1a94bf0f4a3a162907be8bf632427d8f4958b6cf108f77b840b885182e95fa6ba09c29fd902149e96036028bce796901c38da134" },
                { "kab", "7b1bbe920bd99dd4c5babe00af35ef357b233c70093a04bdf9dd1bfb91f82834cf7b24eb700106ce396270c86b2fb768976ff2c195418b1ec3dcf07230d25226" },
                { "kk", "dbeb6110b7063aa2fd157844cc7197e697f9beb0f8250525664f9192f28a17f1015b94ad67b7fd68420a1f1a2f01fb520600d663216a02c67acffece600a3ff0" },
                { "km", "96fd90468bfaba4d968c65fc8e65b8050468a4171c60903a9d9c2ddf49c94a941da986aa920bc15701c2f7a505e5f67715b8b78047f051080e295a0ee63d9a71" },
                { "kn", "0d47dc03ecdf4ab60bc3ed1a88edaa1c5b4adf26aca593030b3655aa697d27da2338629e0528bc580983bd60693de8a581ec0640cdea3bf4a34baa8372a2c9a0" },
                { "ko", "279086cc71d5407aab315beec35b34f821270d8d86df58940b9a064fb0078cd94a4104f47feb178b4725007129554d124450c94ed8569ceecb0bd9c83f3de6f3" },
                { "lij", "146751a1854a67c37f4a4c79e732f66297e19644c296c682eb4f071b7baa18345446e213ba9d2012f740eafa0adb5dd18b4d43eb7b231f5cd3978af1deb975a4" },
                { "lt", "760267e79f7c349cd0676ddcb93b19b8fc666d2dc794fe81d33ef5de3425d40cc67bf5d5a58f15761753c59cccb58d3f18747aa53b7cfa0d03444350ff1e5580" },
                { "lv", "44e9124a9cd0565d1dba4d9c72bc89d8ce7a69f3fa024aab0dbeea1e53e4b8c5c96a10282b389340dea855b9a1e956a6dfc8a66a17e0786efca8bb405f57861f" },
                { "mk", "7690b1a4443476fb065e4e2e1bb9ca33d2c53515480d0b6e7299755b4926fc1b4bfb9dc9822f1a840575d028bfdaed7711fd3afe7a4613f12790df8b6e80b912" },
                { "mr", "2eca8d3f57add93c1ace5ee215818258a30286fe5bc3410b04098053df6172d8d18e2c13b4b6589d9b15467b644aad4cd1ecf122f24debae2d1bc4fac36c868b" },
                { "ms", "9d8fd9f451ac14f4507e129f003e63db99b3df2747ec29a63dd77115552a1d5ac1f50eb75c653df2d52469dc8ef9646d77348168b2fbc90388a7f7e3b0628b7b" },
                { "my", "1359a9a3a478edbc83b670b5e1c004190d40875992452a8eae93ea4287ddfb94865c9d953aee109080272a78b1c920b4398aa0eca7ca563110697889ae0a6da7" },
                { "nb-NO", "0ef2669fd1dc90d66035005dcc084ea61448bd47c8b52dddbdb731f9680a20eca6843d66e5b76e3cde0b6e0277f7ed6cc28486d9ff7e921fedfb22e63e931a52" },
                { "ne-NP", "eb93d9663bef0d3ba1704f9d41fdcfddbf364dd94ec93ab98650035745faf32e3b71ed0b7194f1db88ce2b316329fe114194ee2cb392d839b0f32d0d4db76377" },
                { "nl", "da56b646814e01883ec4ba9e247dfd395c6837d01324fdf94beab5844a404a31c81f4659ffbafda8f795ca92c9622b3d37bcfdf5a4667e7bb6b51bd42938de49" },
                { "nn-NO", "f3f5204ad14ebdc4be8e4299a457698d7f2ee5e2094effaed47dc46511b29d4a6465b46bb2264363796f86a2e3e6a7724977436571fc11b7d314e4d8c48bb607" },
                { "oc", "53a5fe15009df0972f0205f0860c778b61b72cd7af91ff793980f611fe10b9bee32a37d3176ac6f631c30eeb33bc8ba021a12dc250cfbed8f1ca842cc634d2eb" },
                { "pa-IN", "c4c5560c1df3641f53600654c32cc1022e9a183fc6920375692444870b1a738c158c6f2a8236aa60b05bc0682135f5c80622f3e61429ddd4ecaa9d4df0383a44" },
                { "pl", "d5b4bdaee97581053ae7976e37f8ef92222c847751a86edecb5330e116f125f0955854d8e85ddd1480d5877a028bfe85409f46aefcc19c756b10aac25c095383" },
                { "pt-BR", "4662cfa0456890ee8b30e2b00b5aca58cadf6d6a73311264a20b6c6ce71eb468c4b6d861148628f421779436fe2934f1cd8edf6d61168c7ef47c5709bf18007a" },
                { "pt-PT", "62f0ab4b9272376c8e5cf8ca84bd22e869344b4525411cc0d61681a440de91cc8ee639481c3a46e1f6f92c386e691b11e6e0364ca5105b9e9f6cecddf95c19a2" },
                { "rm", "e10b6b31966d54d366acfb967883293520eddcbc2537fa8784fc7fd45a158a45d8cbbf16b4f65b81b2edda9c79f005ccb346f32a4d783351f87823475eda4204" },
                { "ro", "0a01e7780e4ccd8c40c4b8015b8621ba06d4afc94680e8e8b25c89f205856db27e3acd3dddab8979d477553bbc905a647b03d9b6ca162314cfa28eab9f4df386" },
                { "ru", "aa8572daabe4a392d00bd923ad71987cddb65ac163bcb2c457a15baaf5b918c9273abe443292f10362cce14e61ed6e0304d447218b4a156ba4df95753617f99b" },
                { "sat", "b0324467233307ca4ec05f425c1fb50c326f0e4de10c0dfcedbb1b19a9c7ba459119177c616e7d7654a59bfce62b3b7ce0d2495af8722277e1d1159b467b5abc" },
                { "sc", "907015acc5308535cb82c2445ce206526f8d75250e78e58dac43e8b1738812766cbd680a676337cca2d64e6bc2a69fc1c50be9728104f2fcb41ced85f194047b" },
                { "sco", "144f764e6a42d880968406fe1c91b6171374efae123425457a75660322955ef048607f41d0760b2bccc12979baffd05152cacbb74ad63998b5fa699ac0241476" },
                { "si", "29ed70e4a93a07b2cedea497c76e0aec0ffed6462b4ceaf55bee7623c0baab75c201f558cc02946b699ed21614ae1d4986ef909567ff9346a57546834c37271d" },
                { "sk", "263109305a7db845677e3086e18678360fe93acdbbfa0b56d3c257e8cb5451c36a15c3d3e9aca9f0474881516efbd68a2115249edffa9bf6733a8ab2555bd440" },
                { "skr", "ecab7677e4a61a5fbdd2dd19591cfc912ed54293e0ef506b4fa3f0168a44e24a94a67bc3a4bc80164e8bf9f99d6be1c136a3f867e160ce911b2e815ad4d47210" },
                { "sl", "415f4ff25457baf1aa83d9bfa36ee0f65a4e5716faae8f6cab2ff4f6fbf15bf48c50160a8d002615633889ba4684b2554a00a4d4f168a149719c09f1c28a0692" },
                { "son", "d97467eb9ccbfc74ca4dc52aaac8b4168e9cba5ea757f2e02304cd15eeefcd93aed528ca2fe1f8a2c7a84eb4f28a456af85cf5b85cfab629827eabf20773f6f5" },
                { "sq", "e2265e552ac7897e6f2bbbce9685c7ac3c3dba09da7f5a061384c279d9210c70239e2e538f15c8b1ca6033ff878e93c06cd10abb8645ae708768fb00bad9a514" },
                { "sr", "fa4f4ab51902286839b8a36439fbc8a7c56bbbc6ea0a56d07714e90e613f15cf303913f1b9b1249c964133295a91977805036b6dfa0377f22703b0e905070b6e" },
                { "sv-SE", "372552da4b780e052c379df217da716ba182e5c849c6c6033b87cba99b6c7d31491a9e666435ca15e91548f14c4e76b8a6822dc88a767163e73389d270c65e33" },
                { "szl", "d7449e1444b2cdd2a7167a4365d06733bf06fce9de1fccb6e1ef34cf6dd535323b649ce2d08d556cd2af79af722b61ed91c5a9766f1ed3dcf59a33f06103fa8f" },
                { "ta", "b5e5ef84cf9e34f710d07085950348ea97594724fe51d9240e1e3847d0976bcf116ffbd4518914e5831a58cbbecbe18699bd1e11fe5e78751c193f33f6f0e2f2" },
                { "te", "492286fb5919157c3578d2664134eea7aeed0e7fcb085b68757ad5e305a90cd73127e13bec6b101e9369e7e6293db7724e813202c8900fadd1f177045cf4b85f" },
                { "tg", "4a05e4a9f42731b57f071a2cec3a2db5a3ff31707d33f9137a1b27da1743733b9998108085afecef8c277615463091c4b1691711662aed7d29fc61fac68f2700" },
                { "th", "9cc3c31b785152f5b3a6d14a71eccc88e3c418cc387742b336e9057c9e99472de042d59dd24145876556d1c9cb62af0bd940690cd85a838ffe5e57650863cc76" },
                { "tl", "b86db1e619f218dfdf42bf2b73c1aaf1d49d61da681e5c328ff2bc1d1fc8dd6b15ad6e1d4f01d97fa9d28b87811f99f37f8c501a0cc1a95f9c49a4a290c2c14b" },
                { "tr", "c393aeb3236a6556101fc4b05fcc38fe83d8acb5394ae0e6a3fb2e0ec4cbea876035bb283a79a929c09725388b8ca043a3a7161d9f62ca1437d61bdaf261ee63" },
                { "trs", "1b425907b6372603339a7a7faeecc67e6892a1b0291a550ee32b9c59bfb964b5d036a4c36a5da52f70f655936ee37e93bbede4cf1b72a84417dbd78a9818c8fc" },
                { "uk", "fad98af1437ce521a5f9dfb45b12055a34b38bf7e1cda3bfaf5094b3f304e42ac5d77463afefcaea3b90c4b54d2904eb8d4088d945b28630b797b486311c68d8" },
                { "ur", "5a0f8ea77aa758e539cef30b435ca87ae5425354d4fe5651e59a08eada649e0c433eb72e6765b329fe39ba2a659536becae0f4d702aa091eab422aaf20d7bfc1" },
                { "uz", "de4697258dd706c58e10c07394d9f27c340229ce4a473577823e0c83521eccd68574c210a25022e66b857f78804bd61fc2f1cfa879ef06e1e90091cea18c06d3" },
                { "vi", "c3e8720a8233679e33379de709519754c3f3fbbc042c5a0612478589dbd76f60feb964a679385c9b990082ae190048229643b4fac085ff6d1a7dab3623812b7a" },
                { "xh", "f438d1b1e0a9eb61e9b8f0d19bdf337560cd6236e2e8124cd78ccb28ef51463600d702609af16018d949c3f7258867ffa1e33fd907a1da73b40047ef5e39e55a" },
                { "zh-CN", "5a93188bb0dc5c4707345657004f63ae3333c91fdecabde877ee90660b2d4289ee74a3e2dde46f33c7521f60539d853156bbcb076e44f3aa1e45b9461fa50ed7" },
                { "zh-TW", "17fc3232407fba1259514356bbb0e20bbd29cae6841765005369c037bd6181bead95876d4e38118890edea989fce6b26de0d4e76b4af1548abeee3a24f2b67ba" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/134.0/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "a4dc3305d07888f1bc46abf8547a2b6f2326822d05020b4616e26c1d780519367fbdc32fa455d2d4d9172f001758216316b9a7e86dcf3cf0b0658767b0a31400" },
                { "af", "b5153d486070b49a43d6f9937b2e339527536f3ebe312e05e87c49d2bd97e919c38a8be25c0e15c3d5b0de5feab1f4b058f8f26b3abf748c9e459773322d6f6c" },
                { "an", "128c24d2bf50eefe3513d051e434886bb930b3b78b46f63b25cf5cebf38157c66c738d0d4ba35288e946f4fc62b3b8fd77174485aa0e01fb181c36bb830d0bc9" },
                { "ar", "50d15ea694171273ccef08c1de9cb50b55b95f9bec4c60ca62e10a36e59d1bc638241077d0e77ddd186e436fa0bce43cc239a68c6cd8e1297113935c2d9649a0" },
                { "ast", "4e43d0656c014654e9951a2d2108420b4f3596e4c7a8b3797e85e912bf1ffb16931d113d3ddce27f9dce6809b614b03d752f77d8bb453a50edc0fb89a11e643d" },
                { "az", "c640c9a9c403b6cf2fae0a44394a150175ccbfeddbeabdefd5cc38b1fe2fdb83760fd59a7d9a538c9cd8cfcc0418288eaad4aeb0890132a38e53ba11679d003a" },
                { "be", "9fc8619dd27dd0e2b358d36d65bd8b0c78f5cb1e0657221f066d0fd74346682bd2598b200e47a473fa9c9d0494bc75aa59a78da4a1a4aab8d4a15374641b85e6" },
                { "bg", "532cc983112c6a7df98f0019da383ea5045d4d95aba5beede16d414d7206050f611a5ad24ae1363327eb6af9cecb3a1f4d6caf5396cc5168bc98440966e5d71a" },
                { "bn", "3ed3fb14c4092a343dd455e0d51ceba8dfa063aa233e189ffa228f6c40e398700abc018f656585561d67496e4e6022b9a256485b8f430345ceb1b24bbe9baf2e" },
                { "br", "b60036f9313936a7bf0f05d88b247451b1818975d07fe3413a7247cc02fa4748af456b99c81b933f9ac48e7746254a4927564526cf5b4309d24e90472631e094" },
                { "bs", "036ee5fefa2966bc336fea41e077673d2c852988af7fa7f3eb6ae0fa4bc11010227d2fdd9b18cb679fde8322fc90df1dde613ce724e1d091e5d4e2522aef7248" },
                { "ca", "aab650a33060b03b899f245a3dbf1a08b40243b89761c50a8c55ea4b4eb6acd6e56a39d2d428c6b36b5a7cae5a5f9fb2ebcc835bb2f1ee48d1d4a3b60418a129" },
                { "cak", "d5ae26863821593c55102f44c947c648510673ea18ffb0ba17ea4a71f9e47163fe3f4d9f53c283f9935419768b4804e5682841f2b781c2a02b13670a3088e5ed" },
                { "cs", "699c70c552cdd87ec02b0f278d7f9c9874cf360dd794ea348786d517b5b1a9b8319c471df7da5a783ae9f87efba6701082a09b7bef341b4ff81d46d27869a417" },
                { "cy", "5a36c7673f2164f98301ffc12cbf93828d91f55208fd8cf9d5ebfc92187879129de2447fd69b46013b0ceb088581805814dd5e58b1b625b2b0b23d804caaf2d5" },
                { "da", "3b0b3cc336474ef98684f6175d80cb1428786658e0f1141228211845e8c676e1e6fafdebcc05267adcccd357bc712c79bf60f340bcfe584fed9a547550b41044" },
                { "de", "9a65f7960f2c94d0c79284cc28cae4ec6c8284dd1d4752a5dd633d6b71d6f17d7953ff373da8c8f65c4fb6ee5f841d6698b3ffd7e455b984a056d63fd9325b46" },
                { "dsb", "8ecaf735173ac16d9e11eb9dec2d2aab7b1701902a1220e1f03aeb7ae9ee5a8f54e268a47e464fa6be3bc4798ad473376cb9c0c98af7248d22c21c9ce6809488" },
                { "el", "c208bf92f7ac4d51e7ac70a0534bd69cb71c25d1c6eaf3f11bde09264427e2b28a4d4832e11b5250398a2b21b65dc249ba6d65b9dadf32bc95031741a462e80b" },
                { "en-CA", "7a64d0f93ce74f3e01ca054a1f79d966ad4a6469c9636e632157fb0ed6bbad84a152fec853e80269339dd14a12f20fc2c3b1ce991a8cc1d4be10049a73404859" },
                { "en-GB", "088e4a9429500d91ec29f12a6fe0d10b80a7f4d576e7272938a399a60a1b65cd3998f4c894bac0b55e95d7a20c3de0f99c03e688cad8cbaea20af51646ff4726" },
                { "en-US", "ffeeb5a0b9f428c98520fac19af4d16449dfe6185415931fd5143116c393043585c1fee6a40a0b9da6b8b874b222d5b7e0a9759a5cda47f1caf2ba4c940a3c6c" },
                { "eo", "925fe8413b3d7f1f279dd926f999e476ff00969269c7431e3f8c7a8474f9bcbb3dcd5df50a73a44554d85f25f8be1f51425c505763ad0faebd6c798dd5213ed1" },
                { "es-AR", "8267b501f9415dccc31d74c32c5d1a099b0c0a62558dd2f5a47f85ee0b2d18c95a68ed9dac2e4808643c7042120a460eab5bf57415f49ad88943f86c647c4770" },
                { "es-CL", "d9d728ccb6f7fc76621d1c9706f605763e830222a74393fca8e1f271173ffb8f9733e6acb3106a8ee109d9eea44484904c4ff41a701fda037ce4c572e6aa2e31" },
                { "es-ES", "7f10bba4037e59af1706e18764d43d602939d38921f286b4aa80921e04dd7e02dda8850a21580f51fabb8cf3920bce9b77a8c4c8d53ce12ed41e43e28223184b" },
                { "es-MX", "94755a29cc630154b7cc547fb607eb51ef2a6accb3c2c8ab1025dcee5fad131433f81a36f94fcb331ddab2facc504e2f14d5ee522786777528c64c3b53708d2e" },
                { "et", "e202d6402ec0784f489c3b928504aaad506991a369328aa01d0b7e7de7dc6c3134291337600adf77fd2891ef56aaebc09ca9a2edd229b8db270fd4e68ed79618" },
                { "eu", "d75f495242434b778b9465dd9f38d93b307e639f6f535feec44dc2aa69e0c6a6b4bc3a111c73ecafc88dbad67414118e2ed968c7419b2ba340cff955be8d54aa" },
                { "fa", "2a8ecf6a40b4b919cff5f3fb2eba0ae6123b238740b72b145e6fd6cab9466c0b2e5b5ea0f894d87db4f8f4faca2fb75e7656ef0f7324ef790d1c25fe2e8bed2a" },
                { "ff", "a3d5f9e62ea80fe812bbaf765a773f63e7ba72103153f9541ab574da8ab10d0f8bc0b7e13bbe58948028fd3449f8c90c3830dc423273fc242a5f09aa25b47728" },
                { "fi", "a15c880b5c128b48d413bdf8008ce02e377777419ff2211a7f6ce75456bd9574be69e7734bf6cd8b7402fada68cfcb3f1812d2af997989a3f0d081c4418c5bfb" },
                { "fr", "37026040991a441891d3313ead8b056601a55acaeb439d9015d71943c328e25a3c4df00b72c51b6073f7e1f94d350e58ce052762e373d1845220d716580ef09e" },
                { "fur", "ab2d087332f86bdd6e5a990d68f7b2fa88b09573b2ebece2056a8b0f7188744792b9dcbb01c4802ef438cd6d2a423e43bfc8e85bf53f8546dbd2c04b7b2c7404" },
                { "fy-NL", "2d0b3a2f682a7ced397445be72bc8b4c0c2017f61ff49225b8c862a2aaf014257c4475e1e75a4fd7437c7baa6b275a18d53b87d1d3f7fa000f07738a6000a0d3" },
                { "ga-IE", "2fd2b4ff2d0f403582d613d7ab6d42d3c4ee801faba9c94c108b1a4e2280892fdfcdcd845a9005db3d3017cc4eb0fe24c603f5701923ce74781ba6b4fe21bc81" },
                { "gd", "12099d35c8025a11bb47fdaa0247b7625382bf162b730c96b0a236b048f20c7258f9885e9644dec556d94947f6a5f18e724ef9c5331cfefa8387fa52dbb4e922" },
                { "gl", "7396953ea82ce66167ac76857e5810ddfd1adead6ec3fbdfa45a317af448d16c82c474abfd9149c2125cd4d1bce7a2400436ec3d8ac25a548ee5f9158c382a60" },
                { "gn", "9ad711def6f913f0aadbb97c2c3a20b0774377c11326907babbf01998be64381d8ef85dd5541d6f5da0b0df6186bdd42d2676409c1ff5859d22e44b5c9cdb727" },
                { "gu-IN", "a448d1ee9f851477ea313d124ee3b682eeea665105619ddabdee96bf796a7f69d80e0e3e04a49debc2e89713bc327822bf61773deb1a0b7031125befe17b4d84" },
                { "he", "1c90fb0cac7ddbb7256a6dbdcb9dd3afeb115bc80539801fd268b1aa04ffe438e52c7412b29b5f49ba3769c8a2f1ca59df9e13191433215d8059c302f99c7380" },
                { "hi-IN", "28174e90934a4258811d88390ce0c43e7dc4e8f06e1d7eea9c7fdac913dbd80ba59572e8f77d5e746464f3eeaaec25640b909e3449d24140043a1c8b7e5f7745" },
                { "hr", "13d69083840ca0e8153bf8872811117f1ceb388aa47b163c7504590b4f733cc6bed50ec2da3fc81887212fe99429db3b7d1639e8cc090697e2b905eed26a557f" },
                { "hsb", "0a5e165515d7bb86ed0bd66994dc1609040eb6f2ba0b869d0a72ba8d20c23693687badcf5578ec45d7597c3a9b967a8eb677184c64fece877a30e01c528a6a58" },
                { "hu", "f252eeb672ea2c90fc7142ab5e53ee9c822d341621d8dd13d7f8cfd5f00bb5b11a95ab041af6fec3455effd8364354c42e6655c75531d676aaad38f766aae1b1" },
                { "hy-AM", "261ae0c43925cee330445dc68d76fad94c2520d03338b2fb9049d4e2fc7e2bb1e9307594ba0196aa8b6d1e9ef2b0b1bbcd5820f77848116b1e6001af934ca48c" },
                { "ia", "268a5c6c3da4f89a1b70481d1c3ee16ed16c8069ea915c5967398dd666dcc063f87d5bb855cda4cbf3da9ed8c9fd13f3acb2ae6c1d29111631fda525d51f517c" },
                { "id", "3f43b7154d74241afa47ec3a670f580a127e24a3888bd5927177270fa539c73082b64689c9d2181c12d8aab94ab821a5fad98e7cdce2bb41578240c4e33fb9c0" },
                { "is", "5caf0200f018bbd985f1be08e382c76c8f005fb77712c7dfc9b9118fb2dada38f344c775c5563d8e5c43cf5592db520c4c3bf6fea2c58735b6a9aadf589177f8" },
                { "it", "665fbbac56a370243d42c28df20fcb0d963995393206c775a9c964ee39bd23396f4145330b96641bef2eb1c0f136fad86537f462f522975bebc690de3ebc8436" },
                { "ja", "4a838f66449e7264a0a76161688309e4d17d575a3ddde5dd5bb3eff0fb610f726203c79d13f32b87cf419e86e6eb190dca682fd84aa361e4b049ebcce45c294d" },
                { "ka", "ed097388fec913d3040c447ba8986338ee3a6634135ae9794fdbeaea9c5fbde9e02f49033ee6becf12a797e49ac6962837fcc422982a0d0ca2272bc9611e510a" },
                { "kab", "ace150cddb6616899944809f1d5ed76fecc94397dcea0dba556639da0a9954ee8f59310dd7407711b50260362d643e240b434efe69a3452e5388a7baeca849cd" },
                { "kk", "841590d27c7c63e7eaeea9b06d1cf13c81f981f4eed9b7f0742af42cee9c9bd7c2a410af95bfa339e471ae3ab0ecaf23de4996f7f7013257612b1d8307e840b8" },
                { "km", "d4aa04ab1d8c82d53ea32967cf3f61f37ad8270e5bbdf9d57d9cafe282bad42573277d3634865a1ea10e69fb68fcd8dce9df8de7c7785838ed1422cb4c9c6da2" },
                { "kn", "418ea2ab7dbeabd2c2b4e91c547a756aac97ff55319d18dcbb42ca21af79a06df40cf996957671247edc8e270c19ba0d77105b97f82c3798d89c15424cfc88c9" },
                { "ko", "07faf22d4a139a56b64ea3cd522ab957eca43074259986c25f3f1f2e460816e4a3aa0614bcdca328e7dbc27334452927c895ffa48d2c64dea6b8b2842b0685b3" },
                { "lij", "49cb5eda69904f2ec26dc1619d4011b6cd435056f9ae11fa360e429a6d21ba6ce2cf1f27638de3c1b4cd3a262b1698c8d5f3e5411c9b76acdf5a7a9cc084d313" },
                { "lt", "8b530a01fcc5a28f0def510744ee34b30008348acbca6d51af9be70943993753b602a18daea3cfea326a34a5430c7ac138b4770687c46f6bdd8e8271e060c9f8" },
                { "lv", "d92679c3fd03f42a9a22675b1d6f8031b6d080d5595d3727d5c54c220e87635f6b7bf04249466472288b1df916695dd2e1ac433dc84e5488089a62ec6ba0f666" },
                { "mk", "495e7d4e0b9d3588ee0da9ef3cc339f5c7ad61e1ea8361767d9b6d549f0afd1f6cb2b9bf070ccc2100bbfa421d42bb6d2dbf667bdc2448fca40139ec63271e3d" },
                { "mr", "d824380e2a01b741b8d9212a5d2e4c9555261cc0e717f2cb663bdd7bbddea1f594c5220a651eaed97862f3a39960b04f4d30e69b993ab6177f1b74767a498996" },
                { "ms", "b348be5f55c9dda79fec064fa1811c4ff1e046d36f4678e04e0451a96adb6302d6a290cc4da6c7a2af2f2a28adfc462de8fabf8aca87a0a7b90592ae43409c98" },
                { "my", "21646f4485429fd77d98338f81ec8c431ba92d9abe31764747cd75032c6a1dd60ad80a037369884900a5d4f00ce0f60285d1360772393b0e2a3747969c3822e0" },
                { "nb-NO", "a489f218bfc3a5b62cc5b181d5bb448e4979d205c4669161cd9228aa4df92bcf13702c61d3ec092da31e2122a8fea3c0b41f8c8dfe942cbe421fa6d697ce32d6" },
                { "ne-NP", "6562f49442a824666e0c5a39dca11d4ce7d64978e48674e6313cf20717acb15b60efb75d7189a778f01d5cab695d4001b4c9ec1cdc1239369512e9dbddcb0678" },
                { "nl", "1cdde224126c06cc11b9727a4378f6c97319e739c008f577b89c34f475c9e5186c16cb17f32f3fd68f721265be49e69898a46fd7db020e9f50dded49b392c24b" },
                { "nn-NO", "f8ea094f21a47fd99056d8d4ab792da9ff37bc7518c9a295d25cbe79d3efda529b5e004472aeb9b35441d15d25d3afaf719b8c63e6816e9e6c69a59fceca874c" },
                { "oc", "f154fa746a7af3bf9d2d8f8f62c78a101d2472de5fc9a423da2e2567b817c066d4a987b1540a15391bab8014725e2fd5816978286545d19d8c4e54fc895ed42c" },
                { "pa-IN", "32d066cd39011e953d6d1b9948001480df77ef1c025d693d99582e0299e8fb00959c2a3834a3a77e96f69220bbb9546356b10101c2db70df274b22102d5dd22e" },
                { "pl", "4c4ecdca1d79f74d1890328494c354117eb6ddcc46ec6533afcacf06c8c3363b47275b22a85198ddbf4772eab5c0978f51a3ff6357c8450048a0ad0c8f7f7f26" },
                { "pt-BR", "e4f9b9477e8acdc8031eadc3e47a4067c056dfe3ab0215c5a7ecd80949ed1a939d2a2ee4eb930bce6094477cf8017ba62e21665330cbd117a8be02fd93d18190" },
                { "pt-PT", "06c3aa598abd16098c6e3e7c82878fca299d1a019cf05b1c134b18d1584ad23e1065f29427671a9448540d59329fa38249f6daeff09ebdfd5874edf707819805" },
                { "rm", "1d8c036ef2b660661816a5a0115650811e1a847233a77dae9b917db5da1319c9e6cced0ff0df5e9112a40611b366cb3425d180e0ab1466769099aadd2a248797" },
                { "ro", "b873f6f9d8e50b4b74605bb36c15be7241fa26f10fe6d70a202e689faaf79c0129a2a964a49527c1d68d0a11dc88e60a964020251a421263710de222a9e3dac5" },
                { "ru", "106545f157d9a8c2bb92f5829739703b5fa9069057d4c9176d618caf39b4f7a65924bb6f3cf6d1e3da712d414e43257e1079738c5c291b32acac1ac363c74971" },
                { "sat", "852b69c92d815071cc835bd1d5696b2c032165657868f4cc95657aa4915a2c4993e0dbfeb7b7831504ab2910261fbeb9184ba95ff5c8d2aa82cb73fee1731ed3" },
                { "sc", "5463d80687b0f4d5fd660d179203e52ab4443f3f0aa4b30d21b4dc81b6eb0ab1ef4b62b100c0361475bb43457f43f2c3997ae54db8fb214323888a985cb6cfa7" },
                { "sco", "2c03c149801b445fbb12f7caca44c17ef177a50d6d50dd93734adac2f9ecabb713b11487322195c97bb00385d71627db75d4d374d0a755cc1b4c1a52c3c687d9" },
                { "si", "9f407b6285ee6c37b695fbfac7c2c3dbded137d3f8c4165e01c11fd4d467555d3f8cc9b2e26e4f419b4a8d20c5d4db93d42fbe34aca6b474f4a091cc54e5494d" },
                { "sk", "66946502f2b01f41cbb17671c9129a4bf8739efe9d3f811d20aa69dfe87ffb083101385911e893a88c4f6166a22845be3c0bc950ab32bea51d0593b460319b6c" },
                { "skr", "8ee85bb2648998fc55f82191fd670b4e39728731945539f72b4bb466e8d9a2c3c86a93f6d63859722607f962248fc13899a50a5ebc85d4373760399b835cb02e" },
                { "sl", "4865beb5e277ad2cd4e0d4a2f54c976dffadd12bcb07381405f3f45991d8b05f568f4d0555c95d32aa40e8458ad7cd08c34291c93fae01351b5b1eeb61721ae0" },
                { "son", "a2d2618db233eff402258217942699a61999dd07c72ca49fa60ef1d5373542d0e4a04e8af5656a32ae5a0f8783996be6543616a7ed5a0f7be52d25d40760c3a7" },
                { "sq", "f0adefd821085a43526fa749c1f39cafb34bb7d542439b878f11352765478197b6c7c190418d15f06901e1933ddecc5b85707791df841513d3dedcf11234087a" },
                { "sr", "9a68e64c506803038b7f0162c5ad2784a1014e46b39bf835371614efda3c49983f33fffe2d1471fc90cff5fc2e211ba9f471640005f0fe88559568bb3bca1242" },
                { "sv-SE", "954cd34b47aec9c3e145b9be2d4f97fd2d394d5ce574b6df1a467f53bf83c9aec9255f0085960064281a5e8483d75ae44176888d9cef6781e0261f7253821b66" },
                { "szl", "e4c30dda6bccc0f75f204308c6fed4a0ae0b1a4768e12d149e38ae3c6aff1c596caeb71c82e83241da3010160dd49c50f1a899eb3a21943cc6bbd3482211ec3c" },
                { "ta", "b2114c09893769295da0ee38f2f2e9a636bd7d47c8905872a98051a8b8df74395f7095b6d0a2539c450bf273e9718f84e955a9b238b731399bcbd8306f182d23" },
                { "te", "878f1609a3b3410a97b624565aab9d3af174c8128d49612f567c02f5f567e60f4f61b1855c193366071aee07586e3e99ebc538f1f2221d3d10614993da5d1ea8" },
                { "tg", "d1a36f84ff75f7bacecc448d05460cf0d0c314f8f8efa5b13208b44a3a218e7bd3d388bd3e1e643be85937c0d52d53c6756fdd23a884b311a692842947c33019" },
                { "th", "0023e1e67699619553263e1ccf7de1a6d67003cca0fffcdbbe322c6d3b7f55d9045f1d599299da49605a2a4b27ce6ce85da176c8dd1ee9f890552abea94de4b8" },
                { "tl", "97515da8e6411091fc74f85b2b8ffd4383e04838c536a5d6043259b6c4febb3769f66312bce84c6b763cfe1ffa4ce34f6c1e95f8e51d7df5d20236f4ddb19715" },
                { "tr", "9bc86432f08f91d028de00abf432e63469f1195513028ebf14433600e94759b1369a084150337b23604b8ca947db63e2f3e5654c6e361d185eb7a2b5bb8059c4" },
                { "trs", "528b7a10f12e20a0b91d51c93a3aad9ccba79f4466d2c908546654909a140c785efa04fa2b5263ad0b0a093f00646578ae00b4fa68917acf6e7acfede4be30d2" },
                { "uk", "c79196e181e7f755f70e86bcef061717710517c8adbf76d3486cc70b20e1839402e13daa7ad4679172160ac145e8cac0e813bd50fcf85fea91f56ae9a8c3ff8b" },
                { "ur", "c99f42bd125e1757d2a3b7731c3d5e8a88946120851d6158233194253f96dbdfa3420fe9868404be1212a725da2ded1beb302787db174b98accb277d86571cba" },
                { "uz", "54edb664d9573e61a58b2aa8133fb7c0cb5a2a2d41ac85ce7408e748fdc47a7caf31955f62084ed1051b76b57a11a69fe33c38751e318fac4975eabc908c42da" },
                { "vi", "7c7edab5446ee47277c2b22975303cba8289bab3e778bde52c6aa627d110aecc46393d4f6fa24536d098029d307173b26cb0e79164775c97f53ccb8a7c8e7b36" },
                { "xh", "ca452159e489d77800845bfe836fd79fd5b0dac51e9f1bd8c7fd90c07760fb07f84684bfa590535e12d4c0ebdd5c211be3940f76b26a787c24e207e4756a9c51" },
                { "zh-CN", "b10c07c2d08a429f2cb392a549f05eb7e1cc847ad5d757c8a53807465d8cc062b7b3a19cd224106046c9b669075167ba3e7d1b36a0bf7d59f0616af54cc5cad2" },
                { "zh-TW", "74c4048c7ca6956995e431e7b04db97fde172e82105d58b8732fadba81d414b6caa7fbbb177449a5776105a09d5797b38c0be7b376ba74bba9f6f6ea6f2a6316" }
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
            const string knownVersion = "134.0";
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
