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
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/129.0.2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "8d33eb0c9011be2b466e38c03eb57c498be555a3cd1c48221350ffe2a8244e36ede47ca31d81b9a661f833a37eb6266c0d0d3e48c22c91430d3e207e4c8df56b" },
                { "af", "4648bf842b3f7be71c99b238b2c3cca9f29c6398740b0d9c0f3c8619e3e660608e8c62b306f4260dcea71e0961915718dcd87290dd215ef40fd4882a4abd453f" },
                { "an", "a129eb38e8c220e098635f679cbe1c8179952ba02f4224f24ab42a253468b881268df64c7700d361b5fc61a899a74b11e9dc76f0d9e2df0cfa2e48c2ff824655" },
                { "ar", "d52e6a4630e115070e0a2e6bd9f59b38a13a81c0baaf71d8913d62e9fd6f20c10e86b1383fcee3db812a4eae2ad4ecf8ed513bf478abde020308dac0cceeb04a" },
                { "ast", "a140aadc54b7d7db4ccd052841936d63bddd84d7bcf4a56ef868559aff3a57b0fb6a367966335d63f2b8eeb6779d5b22a9371b4deb907795f710fb466ddae94d" },
                { "az", "58b59ce9ee4bb02d9e3ed221a45e63d080f53d19e7bf20f93c7bd614de97c03add149996c7e81c99e76d4e6bfc1ac6bf51516520f1d2164420833fe6c2f6de47" },
                { "be", "081e310120b464ab36c0375123aa88939bfafe44e9cb186d69d4e0b808aa3d7fdf179979246109848230f37f20e8d72464db9ca4db700cff71b4bca36d8834df" },
                { "bg", "69d69a6bb1321bedfbbbdb1b09c11261a7acce3483575a9c0ddeab87b9b216daadbf807ae0040f417523f967983527a3771eb7e9101a9ffc41f207523d7cee5f" },
                { "bn", "7c0f0c3678563038c7ca8d64d5c799d94e6b1fefeaf7a0aea7cb86f00631186123df0acb2525fcd9f80d67d17794cc6e555b0458a698d6021fa7d2c8d4fb66de" },
                { "br", "a5c1c1086bb4564badcb0dd27a717ddb47646fa8ade5d9e76e3e06e5e4ba2e6b6d305eec0134c7c7b560d56f749ad8726fe3a46521ad9fc94ff8119923c56651" },
                { "bs", "8261df03212375a4f3a30c9f5577da17a77527a36bc88ce654317d67704badec8706fb05b4d285f4aa4c0054eefa1c6880790232867a6961606387212e5bc86d" },
                { "ca", "db38355fecedb05f5db955b84d9cbd8419f0b3f8868119cc58695d9f732f1ced962904ad58a087fcd91151d0be5cedbc8afd8b54a06e82627a0f22b39972641d" },
                { "cak", "51630b58d7a3de1dad18f69be5082343e3d22424556832b6a60315fc4f88b54291af87b614c7a7b0638bd7b84cd23cc3f0b90dfbb4f26f100025367895461727" },
                { "cs", "eaeea5f6aeaa8ec790a3aeca1430842784ce382b8a53ea66710671e00203295d19b8d717b92baa0a21b80bfec12cddf2c5c026af104b313eeec7fb88f55889a0" },
                { "cy", "e2e79eb815e76909d9847dcab2f3ea938f48647ce7e44e1be822639b17f3fdcb2902f9f61d407e23977deafd0841232bfecc490f8b78964bae0e6a48727c4971" },
                { "da", "a5396502f52bed3d2806b5731c924295ebd1bb726d6dda467502773773f1ab89c744e5fe11cfd8d36eb919dff316523f8c6592ea5281068d15ed47776bbc3bd8" },
                { "de", "85e034c5adf6666b409af208163cf47380fa56e51399ff74eb8760a0fa5966bfbf7a984d7a7bd2d1900613db5f0ac22bb0abdb28a9cf8d7f9d3ea4dc1ce47724" },
                { "dsb", "42578f1c5fa146699bc8d3ea21a55cb4adaf471a8395836f1aac9144cd055cdcfcd4ef193dfeea9f30ec819af552f8909329e20f90f534610dc6c4a2c7bd0a3b" },
                { "el", "4605e786e769c336738ddb81cff623ca2cc9b654d5271668ab36a213fbd3ce9edb06521b2ea788da4272e3672fee7c9af15784227bdc9c1d3caadb4e1b3dfb64" },
                { "en-CA", "fa1bd1c0f4fc2c181433a5741dcfce8e350b2cbe33896c433a0d025b84b6aea988d8f987f1d891572ab650d380d332f68ced4ebca3fe635c8d82d7f91a0a42c3" },
                { "en-GB", "8aa6b95e778ebe9ebd2bef1cd8e41ba57c20cca368f8a1dc489586cfa6ddacd617dacd1fa2fc5a1634e588a1b47c0265df39e5ee357ace451c91fdb2aff7163a" },
                { "en-US", "a787e373a7c3f21a52807a130861e0657c0fad17cbc7e3dfab484bd6a63861cd703c879935c039cf0e373d25189e3aa4238def9fdf04dbfe96e397d0ca5f67f8" },
                { "eo", "1efd6ba93ab38801f30e79bd320ae40d0e0128716fa5394a0fe877c14c6471fa5569b6b12b63506b8ba22b1063fee11796367ccc00af721f538f576cb859b909" },
                { "es-AR", "87b6fa4e99e75829a10435a3282db157e20a5666df72cccca1ab70d2aa65e7edc383410826c73cfe462322222715bc0a2db26c448746a38140403aa5017d3934" },
                { "es-CL", "b830b73d1dd211f9e70241430081ba3ea723b3b8b958f3ac2ad251381b7aa5100f9196ed480212157ec3677deb0a14fd926e7398a500ed40194d5fc404cb2d16" },
                { "es-ES", "6c8bc5d8ba72e61c578f2d1b3119082fd9fc331eed39a2f45c5c49953f4aadc6d49723ebaf418e00ca00f146d4a6eaad7fb5f3613eac29c91bd2814bc31c65f9" },
                { "es-MX", "e1c8757004e643ede5430e8b86ece1cc0cb86720091d3c6dda300a2918f8bfe482da28006eddb1567513ee43f953d2de4e7849da5183fd052efa12c35862947e" },
                { "et", "183f22bdcfa8e85107f3b883aa4dc6308d1037a7139ec222d8d8e3440e2f2201f9c1c586ca19ce0aa5ba788244fb5d339164a3ffade12f5707c88fe30463715c" },
                { "eu", "fcbf840565ca1268a3552479b469a3067b7c4d8f3dab6876d7a28e7c14c5a05d3fa5025f27f7208b763f8c9ac81f6cd6ce81462503b1ab5ba00ad3819785a7a8" },
                { "fa", "b21a450cdced2f1e1e978455eeb2f0ae9ca9bff9434b15881e8fe959491f6cc434f1f6c485d850a71512751b039e2d50d595707a765c222df75fb528afcc6e64" },
                { "ff", "2a19430419581d1bdbd0e2ad8dc30411530d781e348ea25923c3d33edf16009b3b51be873d2a1ddaf2bcaa499097306181038f560209773bc7c055b596b49a16" },
                { "fi", "464e68c97b74127ca76077d80b4af6a38aea9bcbc44f8e0a00099269bdf36578aed61da2b4610a91fd8f42431fd6cba40661a818f8728553701860c7fece8435" },
                { "fr", "3837435991e863edea81ee3425b841d0764298d86c91599b4a9b0a414c3a1fa0f596fcec51212472061aef0f3a07b55a44901b4061006379530e3c97ac077746" },
                { "fur", "84116c8a4751726859d2f7d7dc82e94381b6a8390c3d340cffb64548104cb02d705385d73a4840937def28dff977f19af89f2e891ce4c6101700c7182a83791d" },
                { "fy-NL", "4ba854523a4d40dc2b223553d975470e1e7ea8a67c4e7914d5cac075136b91c6933c53f9bffa8bd2e3768cb61278e9213cf8685fb533744875d725ca2acafe92" },
                { "ga-IE", "c365b1468ea754c10dc6c1028429cc822bb39e088116681d1b6d87c88b3527d8227c99735ad4342dae3ac335dd835d527b00125c16c79027d059055ec9c34a9c" },
                { "gd", "15687a1fcde761dc617a09bcd1624b52f28740f04323ec6a879b1975b3f2af27c5cfbe800d66cbdbca6f673b7195f51548a8d93e82c141a49c4c6552687267f0" },
                { "gl", "2efed5b880683fda6513a775ce919ab1479808b4c042df81223fdde9a7b8793233f82837f9bd2b413c67f586703699423bf064031588dfd274b72b5ed5be9180" },
                { "gn", "dfd71d1275b2a0215a45a7885be4231db9ff20015683b43bb4a3e5461b44446c34922c98d8950f3aa1a0632aae065b9b28f0b49c4c9287da28d230522b665796" },
                { "gu-IN", "9c96cbd6ef415829bf221393ce04e4a759cb64c4e93b2ac6217c8e81e935fc2aa27cfee55270eaa364f1a172ba6153640eb409cc285e788c1c8d1d18bb12ab42" },
                { "he", "6a727ef0a441a6bbd2d89b037bfaf974dc5a84558b2a2df74f4775efc4ca164b7a093ae0b952c0cf47ff1f22636f3b272067cf79ff1842ab21facc79e5c7a08c" },
                { "hi-IN", "fc971b2eccfd0599bd7fbf8fabce5b624c85372b1bf24297721dc0c3aae80e871bf04a0987f9331822a813c6eef3a27ac588f710ee705e38020660c95014f81b" },
                { "hr", "db12fd311aa416acc592168a6f131a02e5aac47092ba3320b3033703b0b6d5b640ca8fb6d8987dc4d52ae76f0cb916f02d04d4ab347e6e223e49c317b674ed31" },
                { "hsb", "ca1723d5ee00c005f5d622ad37d65512b84c7ab2de13f4a28b52c73bb9e9650be5793f55bb2bbc31b90a21efd1b7f290aa68a48d9a24868a1a2289db233a00a2" },
                { "hu", "c7326637831c5f6f651231b41ceed52afafa2ca236820d24d5b294fb88bad8fbd2cc2527f31547263ad07dcc2adea3ffbb99c4484891ed2205400ad5415eb464" },
                { "hy-AM", "5edbd866efb50d77240d464d5e6f24c98d1bb78e5a3d33a7ebf1c1d18e65115fd58952888b4bc853848298b819bdb3f61cd42c0a5676fd094922c24b56e8829b" },
                { "ia", "71986a4dcc468603bb438e1c5383351478828f0357089f1d22d0a107117ea90f48e727b55fe67afeb87c14c60f68b1709cda557e75a5ff8b5217ea71e9f01da9" },
                { "id", "0ca0775edddc4f94b7fa56cc3a1e3e8f91b64addf8b45ba62adc8eaa9c40d0f0a053087c4d56ad72daad3f8dea7b0d6485ecd6e438a6947d041dc442904c6ae1" },
                { "is", "3fda205b436d0821f0918895ef1fb79803340e45a0057f9c1f4e2be6d3cfc4b61d33696ca2d426977d3c22399f6affbb6c7763f97b53ee8da53859d592faff62" },
                { "it", "a7974ca43f3dc5b4eca9e1efee3ecd6064e9c39ecbc765cafb46c1b26a760fb4797830e95d51f7d2cdcd2de5f004ffd8ac0662599b333faa1b78472e34f90c7a" },
                { "ja", "8bcf6d80f5f58fc9582720446d6d33b6f2a4e80b87bfa1c86e547d19af48698e3c0d4fe2cb7bdd0bfb4854b36eaeb6aa7fec839ffdc3693e2beaa55d70ebc9b8" },
                { "ka", "0dd35f86a05598472d4f2fac00e938b69a989b6c1c76ac055cbbf0598b62fc4e7f1b2ad6e0fd39540030613cc8a6f552f9d03166636684ca9b449e2b7928feaf" },
                { "kab", "e5bd2c973e77bffcd880a9c18b715f292610d13d2178b22bce02be03943ec6a34663c5f07f464e3a9c34d357615266149ba5f701c566d6d75170194956f179d1" },
                { "kk", "28dc67793132f08278a86784df1e6e582f87dea2669f7016d1794bd287ca92f9b7f52b8860b59f0a53191034956ba0f7f0715a93a2218ce485067dbe04241a69" },
                { "km", "6938500e292d422cd80be591a90b016c99c7cb5b84e12c8f75f649276291a0b37eea40ed8d58586303a5d3d693b106c84cc20174eb84e80bb06ae292cee6e71d" },
                { "kn", "85f967f18ca265a2fcfecfc5f5ae7d3b57f860a0fd50551f11054c4e7a3a3bc5047f9f0403affe8509eebdc6598e8d99bfd46ac859ee93337d8240db2c4d6817" },
                { "ko", "6d33036ca97d262dfd24041097f53a35dc1e14c6a2a9f11c727d8e131e914487a035ad757e2cec4ca093b6c9b176b3157ceb8829f05d036b285907bc01b728f4" },
                { "lij", "7b6cd4d6a7e075efd2013416d9bfb6263e96af224e5530cb08cb18d4bb6a971e712269f4721f313c6ef7044da4d5f166e263319434325474851cc31a7abb1677" },
                { "lt", "a9f939a04eb8d55c96c4ddc97b77c9be8d2b96a1d69f01352efe2f95417bab3799a71044e93b985cdb2300ec659556126f40ebd6086768b68556d73616bfe5a0" },
                { "lv", "a9b55429f6511346602ae4e78a9fb57c5766ece66e987fcebb377e63d7ad9007bbacb8644153aae8dd62c625195e8e2e5dea4ce674b9851495eab012fedae303" },
                { "mk", "22e5f31071310452416f93ca74da9afc6ecd31d8bb985e120a5a5aff810a89bb29a1326f12175c8904a45bdaa2a9d79c5b4c0165a7def5701402d8f9c7a71e0c" },
                { "mr", "5ae9eee58259b6fedcc06937403a64da0066c0388e6c9cb466d708383ffad523a84da23fe95c1056c520d7cdfe89f7fe03232f47622e5b44050dc220319c7801" },
                { "ms", "6c3ba7bce15be862c0fce12c2a348b5fdc92077df9119d0373b606ba2391c94a33202e67016a03227241074741f781656aadbad3479f2068b62468b24737e602" },
                { "my", "617605775370b8519b40463265cbc61bbdc5f95e1503b09a4767101e1bdeda96640a7f196efe95216e4eed8dccf9a2cacedbecb2dd0ed88c72721730c1461362" },
                { "nb-NO", "98c22782b58adb46f0ab7e3950ea02f2811f268925b666a3923309e4931aa510a1c36dca1a72f7b66b883f823bc9053641afe436f7938f07e0a5c749370f1634" },
                { "ne-NP", "ea8bc8003431f833675d3588bd017377e5ecfb6325307d9ab8853b9752ecdd5ded40ba9e58f0e7dd02554d22163432be5884caba04cdc341d66255c80255e4ef" },
                { "nl", "d46de7a39b2753bc8df7b98aabcfbc68827a729fddf4d365b96382aa31c940dbe183efc70d3e2f36e9d6fa05c73cf7c945a9dc15bb32b64695d976da817a7516" },
                { "nn-NO", "5998dfad15925f3200c1e767af07e3470876410f74c8c9284f9663ed7d0108cc681427f5b0920ff5e52feb62587dc11b9208e7b5e54482b75db86a0808f7e80a" },
                { "oc", "187ecd00e918e0f9681527992b84a20b09bc6ed71b255c07893643f6baa4e0daa2ff130a5bac4afb5cfa78960b39f45055156210b917ba38d5730c9c81ce8cac" },
                { "pa-IN", "de400d47224eea2f15046f41b33a9f22c50756a3874a954cd7b902381f67647670703dde748c49393d36b27f07b260c5aba48fb22967d7b4e98c6285a6f9880d" },
                { "pl", "feeadc875953d770efeb86c784965fdec25f855945dccd4a83424fc53c7fab19d4dc2ed42eb8d7893485b4284a1a0e1ee87ccceb9013c4d15620768340299fd6" },
                { "pt-BR", "ab3b8e664f26ae84dc31e8628cd401427eb813acdc2f1548554bee153e02d3b898d01f7846b0bc0e1cb231374221986de281c8e3ad77e8319bf500e81f76618d" },
                { "pt-PT", "5a8ba7b28194e497d792f98091f36553ff3f8c45ae38ae20c4a7f12cfa187bf42d2831a4d69f804cbd4981f9fd0bad0f2bd664d59fb0274d80f0cb3c06ff9f8a" },
                { "rm", "a58072f44f780179cb63b22fd3cc24bddd81bee2d47679a24690c813f16411aeba7ec89ad8802db5fb2b1029ab9300516427acfdf74522285af9e37e373a14d1" },
                { "ro", "a248054938f7adbab9f2afe3e0d03784103a28f2b0815a40d6733a552686d9e7aa4254ac86c37fb0d1551c71cbb8767ed805e5bd6e3380af872a5635432ad74e" },
                { "ru", "673e7e274c22203073959bee55364895d902359e3181d6266c32b9b19539b04a60cc3bcd28250217bf3af964c4c19f5ec95a5920c994456de7586da7d534a8f1" },
                { "sat", "bfb49cdda1889d734faaf577e41756b937073ccc5f3b4441049bd501f37c816c96f21cf0c4bec339c3c3ecd277f9208aa0150a7deffc65506f820b60c0e406e9" },
                { "sc", "4e615ddc77587728082bc3e50573c32cf2eeba9b37b9260a0e33395efac1901032a8916d6d5fcfcf2e2683fa15d5ff30e4ae89eea1da66ca0092131acc752043" },
                { "sco", "e0c654c1bfef9ad0bc02aaec8029b9cc711fa55426e0830354c07bd3d591ac9188745b120d47a764f21cba722f2985bb773998cb9aa9e1b0cc65d13be1a4df5d" },
                { "si", "2cae9d01caaeb7cc0dd573c4fd94f5ecb6d0a4e8b6da752eada9a76842f1b77bbde228eb8d54a4fd59732b76fae18a885cc01d280e27ef74233daf836dbfd0c8" },
                { "sk", "6084487d37ddd04408c568395158c2d1f386b5164cace4ed72ef6806efdd4a08e0b09187e7d3857d4bf3748e16b49fbe5ed4d262f038803432b212e575f30141" },
                { "skr", "f95b96db7a5bc0af11c7727a2594ef131978789dff8c56bbce4cf2a03365c57e17eb1af4bd99101ee0c122fce4eb49dbe58224cb5ffd85c18423a0f6a2d3015a" },
                { "sl", "0fd11c19d52abe478c9c3c7fde606c7b263ef92b62d587dcfa51eea23b9c885398dfeb01e0dfdb23c297ce68787e6174babfc0a26bd3dceb5b19d1eeee957185" },
                { "son", "b5d44aed125fb20ba7cffae2680f651d6fe8e764503afcd6af536f2b0aa13523901d4645b96ef847d2d592700c71ef5f8bcb3ffa12b5d5ad02a22edf609f97d1" },
                { "sq", "8017e6867398b5e43bd12da77bf870d80577370955ec396996b3b6295625051a544e25c4167504d078b5161c4e4ad093ae4e62fe79c2e0c8a8a5b4f657862d44" },
                { "sr", "3ddc01d8a4258b7fa83bcdaa027df0318d3feafcafbf097360f7bd198c8c4b4b9f4b4e9c23ae53c60108abfef8a3f2a089e3be99b3a3caf87f30372fdffd6ab9" },
                { "sv-SE", "5f210d24bef002bd8077c0976939399ff99f7ba810bef6c6b61dd3d3b933b7f42b6a8159ab1f39ec20829ed781417f9bae2ecbabfa674769ea7b622249cdaefe" },
                { "szl", "795a2b8d004149e0e2401f269f77bc5fd8c8fa6926c2153f47bf22672cd3178becdc76dabfe3a521486355d9bbc04dd1ac6ec8bfee775ac1a3c1c50e914ed26e" },
                { "ta", "9d16b6a710c972cfbd0c0e9737f03cc60b4e69ad492c76302aa0629d0ff64fe659b5ed53d4a10c5b633a925e243648650ebb24034ff1556006d9a4487e8f34b4" },
                { "te", "9874fb74590663686930eb3a8d656a07fdd0e956860b4131770b62d9affbb0b60ed0a7958a85298277fdfbeb2c8853cdbea0176d4c6bf58cef66870bc992085b" },
                { "tg", "db917e13b2a223a7c83996def6464e6fcf947955c8cb01b255731361a0a7c624d0d94c9e4cb5bfb51605aece6457a7e6865d9c18fd00e66a5e25982567af3b0c" },
                { "th", "6cfe15c2db3c8fae42b38cf37053ac3435930de4f2eb2ea10a6981bb030e9f9e9c90cad40e50b88a040ef5dfb6e6cb1eb2a356c0c6c501bf5acb0d51c3b387e7" },
                { "tl", "cf0478a514c404099967bd23e10ad9565060d805a7c40e0e4e476bb0459887b7d989f7425be1176b065c001459d6c2a91e16486545a851b56c5a0e1eee6987d4" },
                { "tr", "6c4e81da2ae67fd761a9eace7e8deafed144aff385afdac7512899c327e06e89959a034cecf7bb84c72f827c6b670d3c4e1fc5b1ffc7e931e65bffd8820cf3e6" },
                { "trs", "04984c7bff9d0f4be31705c0d9b39f2529332fea2910a298a5642ea663829a8ecc2bd5d77276aac7f6c7ba35b0aadb01a6cdb4f9160b8286bd1dd1cd66e73221" },
                { "uk", "4ebacf8519b75ae60d522b015bea7dac78ea579e1bf3d448e1df7f4b7ffdb25dc1f6eb905a44717c1c0b789c1026729186b63c181624722b78bb9789baf8e2cb" },
                { "ur", "e559dd4b06a8fd9cc45f0c58248e85e86ac6ad11cc1b83bd03e665060374686a092c8270469867fb221f6b34c901a017a47f78c7c2baab954d96f8987901cc61" },
                { "uz", "9e46b5dc7ed31c6a119269877bc396f50d324d1519ee8ee37615b264eb4ba83b320b2e841e4e890f3128919a1ca1efe6cb6b615fab9c7bfa64aa09ac24a3a6ef" },
                { "vi", "b2d99d08dc2eb4d2994b29a0a13ba3b14c663e29bf9474dda8203064f31601f4b47f2d0138744b2c566134514b4c8d1734017d92d05df825db5c91df8cc14fdf" },
                { "xh", "b67b73a34e1f6d445ea5865a4f6bab05fc8fd4c386e387aaef2163d35b905f30522363a02823f13c58955efabbac37d6731b8f4dc654bd5b2222fcd61434cbf6" },
                { "zh-CN", "10f2db434fd1c8c4cb748dc346187f1bee91aee4259bf8ae710dc782812ecedee1b139627a4454a300689dd6a4120c9bca6d67c9af748d4bc8eac779bded0bc3" },
                { "zh-TW", "e015e1d82d6bbc71d38acaa99a3cc0062ab07c130770a94e7ccf88b20fd422f747f3effbd9325db427fd4df43a22f214eacd08a5574f8c3009e1f6415519c0b9" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/129.0.2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "7186b7dd7fb2f91f623b656d358eeed7304ab5c212b3ad09047f37691f5b56f27167e37f48d7f9fd1bc31fb82a788015bf61c5c52176608dcf72f2bfd91fa4f2" },
                { "af", "9309ba7de46d6e40eedc86eaa05f39b5bba72c070b220120a64534b0075dac7cc2134c87045503de6f8f8a425bb0547b4dc19510af55eee7858c13aaf781004a" },
                { "an", "077fb14641538fd0aeb03c62347ccfb9512c1ac112a320c41bd54b5c9c9d74c2e7628dc19764eff2c07ba2b801f0aecdd719361b846a82a90dd45dbbe692d840" },
                { "ar", "5261468d65d03be9253c03e6ba3937241d503e9f5547c6aace6d36cee0752f4fcb9140652477dcf57eacd28f2ba999c1379b06e0137cc9fda403b2002fd35374" },
                { "ast", "4acb4093f59c2c77121c6cc69673b5b58479729bb4ca3ede6d186762114ef25b6749d0a5295a6b51f2591414d0b73277e8d20cfba08f2c7cbb60619601122b2f" },
                { "az", "35a436f823feb989a49e9ba80cb2b046d3251d17a8f92c2b85ff511c94b1c450da445a02125f79259aeb56e62e8fd21d616fed4eb830667ad7a9d16ac84f6186" },
                { "be", "21e2c617e7e1588053bb16156531dc314d7ba3487730ce9a83ff4c995efd6f7059131c062c7ce11ceba59de747239074ba0bfa6d8959615d5be142dfb603177f" },
                { "bg", "9ebf7ac66388be0507c76bd07fa9de287e100b1bd24eb6df2d46093071fbfdb3999621b301f0d2369f12a65cf2ba66905089cdf4236af56876106cb36aa2f6d8" },
                { "bn", "e37b879261c0c664ec7d569ddceff05654806d902b208c20d10f5b9b739bb34c40fd652f19f86e353b6956352a6e6497a0b3d3747cabe6d5bd52eeb8c9e8e253" },
                { "br", "d8a0d931e04af093fd94a5820a5b6086c864cb9366a27ca91b8ca94f071fa2cad4d72a1af15dda2edb91ef90d990aad9317ad1c0065e67c05bdddff6a05095d9" },
                { "bs", "c44557a7fc05a5c7ea1b62da2c8f93d03f2a9cd3ed4a7763ffef1c782595e3746e9d9831437584e376ad38573e32f4c2eff759c9dea0ae4840bcb42bba7bfddc" },
                { "ca", "55c55e63244821e71068382e3e586080e647619cd61abd56389276df2912c378e9d2a655a5adbed533a93f5c7aa1b948aba1117bcedd1fcc8ab1248420a35ca3" },
                { "cak", "91b7f8a1ca8af0a2bddf3a8567a9e19b0105753da0dfa4b9e544e229d2a74d6a3a73d8d9c2176d7dcfe6979710a602f33a3e46aa4caa2e2a5f0a8ce63cf684af" },
                { "cs", "d943f27eda4f32ab4c6f041bdcb3a0994c0351f5bbd9f9244c74105cfb839914b4d3ef13c3edb835bfb44ba2c78c643a0bc0c862538a41e3f692e3b51823512d" },
                { "cy", "a5a6fb864460b2fe6ef38315586477b2bec71fd4a36b3b3390f1d377f03b676bd2d418e6a6e54403c9a6cf3e3be6af8809d757f0067ef69a115b84c3b15896f7" },
                { "da", "fd60401c7cde30cc4266b662951d0eb9e6abcb0a8d5cde8874bad6550593e36dd52e335d2e6b767b753eb34aacab9c6cfbde76a663cf58a0cf321bb60afb6799" },
                { "de", "30c36f9db219d33d2faa202acf94849045d8dd765d9cad2d6c649cf88b2d17b2e2d3e22c4f1ffd3121c5b05f5610791a98bc8a383c677c48d85eff3296f351b9" },
                { "dsb", "a6451b6344df1acc1a533213225d893652aa13edf217592909be61de3bed48128b491469126643d543c99fb5474258d2d644c5d008d146ff27ac77c5461aed8e" },
                { "el", "6476b67f5c11b67eaa7daed4af039fabc2e64996a24565d45a0f592c8fc0015eded94bc8da5139326b405b8543e83d9998d21d2b89e0bf59353d25324e483513" },
                { "en-CA", "54d80eb0d116fdeac198669696116eabcdb1ca9f4299377a399f4edddd46c680f1949258a865c63250941f0928908b07d3f256f7be4943790ea880decbf0bdd1" },
                { "en-GB", "d3023e677472cf901724da708c05921881ccf61acde9d291253e41c5e49b9dbc78ddfbde9b79629e6cb15f391fccb4cd3430f95fd1e09595e52db303f4d62411" },
                { "en-US", "5968c5801bb11e2d206dc7bfe317832cacd8b8976f253bdc4433928549fdd996e124c0c6db90e8666cb07d569e916b32da3db6a4c651d677e5f3563d697bfafa" },
                { "eo", "b8030ca1467cd3b5ef8a25c5d6b2249e390cbbd2d5ea0fbd99d82c5d1c2d30d1beb6c922e6581275bc8eff16df3f2d69eb9e6d3cad0c0e07831fa303259884e6" },
                { "es-AR", "64c00dc951344d41ecfc69f1ae9cf30f06a311c19b8392b2f5c56cacf5e02e099f1f3c60c1a561431464a90d6921ee832ec106647167b018b2def06a252eff49" },
                { "es-CL", "7d6fca3bf4bf15a864b6f5dfeedecc84ce619c18e3ee29a23fc713aa5321ffa1089626b37da0b071a4b42f1668f99321c3b155c3769555e4afadc085e3a55e65" },
                { "es-ES", "7b830ac116d508110e84ce7a18b437abe921dda9988210d00adc33432f4b3326ccf4106ed81a269bfa24be8e33408538438865346f84115dd56c411ace33e24d" },
                { "es-MX", "cdd092ab1a475a60e23a061168b7cff2737a644806b24cdb56a2e91d3df09bfac613b6668679d432e3b52334a7644764e64b095d0e63fa3b5a98a569a9f2ff0d" },
                { "et", "111392ca99c66962134b58b6c6d443056a53152c7679004f596a864aab09e16eec265b084fec8738f44b8f4e32f7d0edc50acdf7774bb1d7a3c6fa2423637925" },
                { "eu", "b3cc18c74184744d5de3e6ba8d71439b3c9d239424da2a0f92612ecc20be8c04ddcf12e129810b8dd38be5acb63185db65508fbcba9da6fe1e36b0fd9f8ea709" },
                { "fa", "9965822b7a02fba4da1c8f3f8e214ef4f6af8cfd345e137d4f715443d95db7da8f1ab7ac964bb707c2593de95d555f40a5550e4771e95f0607094fd1a668da13" },
                { "ff", "9e745bca68e9824f5bdadb2139214d9605db72004a969b4abca28f2a674cb9a2f684b5dcd0c974bb9bc0d81b45b4e79517b67fd76ac7aad14a82c37e78e0d371" },
                { "fi", "1a44a9e61ff750014619a5db63a9fa39b813e6389cc49fd793a7ac0b709f703d64ac2393162b6db01798969c6f6510967434e34dae3f0fa6dc9ead8bf87cd616" },
                { "fr", "59608b66221bbcee53c3af483fb4ee4f31ad61a56b297b50bd07af5029b732e2fdb7b9381d96acecbf9828342d8a3986abc77170d377523f7c79591fc762d944" },
                { "fur", "09114fadf63b2af2e789962fabec39c16e27dc1e5cf14a139a17a981c5af7c59167f4106171a48c2107d672d8deaea1f50e4a690f4236e21f625dc58226984ba" },
                { "fy-NL", "52c45d910bdfc84785dd98a2fdbc9ad28c853462ca740d5e699178b5d1316e29a8ee4ebc9d4c0760ad4f88d19fbdfeec9926c26e558e8c099e46a228d0dd4aa2" },
                { "ga-IE", "3ed539734d77f7d7a6b2ee8fd7553aaf4856f5ded5cb1b863b9d770e2eb877129cc12296f89a662a8f5b031777c6dc235518d7fb40ae052c8bbc379eaca051bf" },
                { "gd", "ca5213c1aa7574d9814ceb96a41cc870c372067fc06d9a622d78aa847fcc179bc2a432ca9365aefbefe3c29114537cece7608251f6ca8527b29d7243b9e21cc4" },
                { "gl", "768e431793d583d604bcc35919545fb52e6f52d9dbd844f7ba827d94b1ea93ebbafdfcf31f4ec74a95841fbd89b3d973781bff3b566e390f20af0ce12e934ade" },
                { "gn", "e58184d5a49d633ab5982bf71ff6c95de919e4e6652d6fb99636e58428e1497f0dd77db15f82b393de08e4ec92244d4121c1812fd22a7062056391f6955f0d2c" },
                { "gu-IN", "f5a7333f413f244c248e1342f155cb6fc977762e49c03e0b410b55144ac8f23db2b04fab56cce9b43104a54315cc5d483a6accfa130697731377a0be2a72d0b4" },
                { "he", "617c804c4e6f8fc5cd18d40cfca956b7373d6f9c2a1f881bdb63b245a44ae378bbf3b6c89b59e51cb7f05b3cf89758ad291345f77c7fca37486bca38951ff96a" },
                { "hi-IN", "dc8fc942bca0fb7cfc25da9e9ec99673f7e816a819bc685190fdf9040abf0dbc4b1b68fb59041729d785ac1efbea23ab671de1170bc8e9d415eec6f436a03c61" },
                { "hr", "b4c6949683d119c0efb8754bb9e6c08d7e18579a5ed0fd70e7c0a317ca4534d98e22a86f9d9ac60d832e20f49a78394ae9e7fd8580902fd84aefa3608d9fb246" },
                { "hsb", "193cd20057065190976d692c5069ebf7dd03bcd022ebf4935204c07b1f43203595bcbd7b2b935ce31f5917bbd16dfaf42f955bed26241299979d270fb3f7b3eb" },
                { "hu", "ac0f2cb3089a44db95b33507479f9cce6795641df92d7ee4a6da5cdfa94a9e8edc7d324ade55b7a774b76939ffb79ddc5c2f0accfc960a3d8ebf4699963d0be5" },
                { "hy-AM", "48a0a49301f005d8f538621eeff987246c81acff37ad6be9816eca8ab01b8317fe92f662105b4ef03665d6b1798ec59d8a65a66f3f3eb3b1e1c8516bb9944667" },
                { "ia", "ebde2ef0dd99b39e477a9cad1924f83bccf41cbecf2d9705f5e612f03fa2007dcdfddf3e9605a25456bc5e00a0def1d78f9a3c2d2c1f508a979354256285f1fc" },
                { "id", "d985049f9a8e743061f45fc9bd32d1182ee6652dd324737ddba7991f2b1498fe45492211dcf7d2e73adc93c8cd64c74f71f7d1243d255ef0f7705ecffd0115c0" },
                { "is", "947d2fa9047f5aaa412df3468fb657d4badfa0cea71ada00d770fd4abc034136f8be0dacee21cfe3c70a10f342909f19d7f3434069a43c8d28b3062c7c8afbde" },
                { "it", "c69f09095f3e17cdcb73141254afbdc5f95fb9b763b00a410aefd2e90424b2050054670879aec60edb3aeb0f947122f4f0436e9428b88a79eebf72415082742c" },
                { "ja", "e9c8e5e8320c1ad41f6a1e3f2a8cb39a00596425f2215d1720989d9cef6979c0c073c7ddb0c382c43d17cc4f530bc8baa8a6376b3d4d4db1533d33a818fda2cc" },
                { "ka", "08572b52bdb8201995f1244caadc0c376dc86bacd5ff7f0a6dc9c8626eb9708b4d9a1642db5f1c7d0eb388059389e944463470a7399445ecb0a223d15d629b7c" },
                { "kab", "7c1e5b8a61c15b59917bf9ff8f0592e211d7d08c95b09cbe9e5758426778171923eec55adf4f02e6e37221b6c15d8dc24c7bfeb59fd0e867f6d3d9571b7a9063" },
                { "kk", "a2a34955d5cc9cd306d81fd09dd5261330c42f28692844fa0cbf3aba740969acfd92ff8c41c605c24f4c827559d81e8475519e485d9c79df2974b727b35db211" },
                { "km", "ccbbac6d445f73fbac1e1543b1747b19fd5578098b88690240cad6b0497167512399fbee12b63240c3c472f87e85a4ce49639679f11555d20993607bb7dac1bd" },
                { "kn", "196b6c99d45dd619e18f4b31b5e91a519f2f43e1386f8db189bed03e7b722160b2781ad88ac7bf9df4979670c011536ddec3b25bf75c3e111a2fb380e2ec3cc3" },
                { "ko", "45bcd04fb67c5294eef1b8fe095e50bc43145b576040185d556fd94af5ff8de423e11f9f868a99fa277f311066fff037e04fabfeb70119e5d0f7c3db73b760a7" },
                { "lij", "d5b343a67db1245735ed615acfe5a140682484bf3eee7496e4fed37ab7430c8f5cd166c495e759034804fab8f05be8fe991e772170c0fa2875bc0a9acff5b45d" },
                { "lt", "365e0125467bd6671b72786cd1207e62c8b986a5b559e97cc0f1c903c05b52875a85105759caf2cd38fd36c42947322db45f348210ebf4e6b9ae4bba9e61c868" },
                { "lv", "020b1eb2683e907115b733d7ffe089b2ab120940ff6d36b17630d1cb788a00bd4398b9660d388110b4533b446bb71fb4dd12e23c71564ed92ede2dde1d482820" },
                { "mk", "79e3f18e74cc288b9abf8c6b99a10044e21d34f5f565a9cf77a1b36bcc3cc877ca8b4de8dd5e7bc944d82d2981c77270ae32db7ef6217c4223086861cac507cb" },
                { "mr", "1d2c9fb1f9982326448e2ae3481a04ec25c2d46d9c871fc80fb98fc871a9fa1a0960880d27df45622cab23248fb67863c78d235160c794e045491a12479d2ea6" },
                { "ms", "bfb60462c0842de543352d9f18d20508f5fd9e8ec04c24733a66b7da5ed6d2f82f6f526b7aaaf0a9447f4bbb133cc56153a9cf803b1edf688ab30a08afc37ad9" },
                { "my", "2326972722496f061e725324e1b0af2f0b7ac405d5e14fe5131507a3d4d92296470213a787e55999d87d61bf54c4818c0ed6d0785b85dd64cca293b9ae43e83e" },
                { "nb-NO", "dfe6de9c0fd038efe9f1df630b94059eab524a30b8fa1998a96e57494ddaca493469c5e6fe916fb0d7921a21e4ad1a807d885ec8080bf1372552139beca25720" },
                { "ne-NP", "1978352b5179022ebf4aca70c214c2990ce8f3ff864c32a9a7c93dcc18241ade8cf842ec48f20328d9be80e1f86a08f0367de73bb3dc9eb9ecf4595427b31e7c" },
                { "nl", "784f67a2496b0358372483e2c67343da9f95e8b28bed0c3e709ba600e98b6cd6d598245df21d24c269b631144c53a8c55528c66a5258a87a41ddb412dfcd953c" },
                { "nn-NO", "90dfb7c81c4e0cae008bff74c60f06812208568dba34eadd98245b5ba277c7e4790b44fd2348a29e53ba1f8a00e8cec6d9651c4e63548d40fe0192878593597d" },
                { "oc", "1ee1adfa9f32f65742b919db38796f1fdb55f198fa8c70f235948051f10517ea18ded63cdeeb03fcb32da7cef4c21024359b0027484b7e61fa39b88502315863" },
                { "pa-IN", "a54f399c7d6982dea13acc46ad48a5aa43d5eb52ff6f4db2d12fde013e722922fd20225f285668be6b764f309f989b2abc3a3c602885a4220d62c7b38b92cb4a" },
                { "pl", "c492ff2a38eebf07d94ed10927bb4a49d126f62a1120ed98e0afc39022f329ee9fa63da041e28b50eeb11e4f4f4f8a7fe4763bd08a353dcb3e17247dc51eee6e" },
                { "pt-BR", "b6a11e5e3901823f71118cfe86d1f2ae406d9c4cb62abcba4858d68f8f5681589f25093379154b11f928790b171962cedc9a8eb401d56d28d680b556ae773827" },
                { "pt-PT", "b63f069d2aeeab0b6f1be420b1ccaf90b3ada614713b42a7b894ab215dd4077ab16cd645663e780e75b98b368957aa7c17585a76d39952e306e29bcea15079ea" },
                { "rm", "00fedd322f553a6c7bccce8d2b1d9e41267660213a1b61fe6dd7dc8a7dbe6c8ffdc64db773fb9a6476c2bbc966a96bd13e18245e9ad87ac82e0bc84edd2b8437" },
                { "ro", "ac9e24c50f2df5a788cae64f1153f74265ab399b93b54b396398c89f61e192f303290a09f37feca6008429f7e5ca1c08c153ee0968c90d2bd4fdd7a2ba419155" },
                { "ru", "b1bc8877cec82d28cbce551247e3d788bf15bc615a742a6c9ffc408e5644558f1ab8d531d783d30fd9555d05d96e0181e9e4a5876177408b9c2928dd213a2d39" },
                { "sat", "90a0c8f50fe235bf7c25bc0e84cd9ff900d4c1dac498712c142c7a3d3da53958f7943099a1ca22990bdddae25fbca7ad49c5c58443118830d4d014ab516c21d4" },
                { "sc", "62fa27dd238cf27262f1f1fab850187c70e17f0a8cbeaa7022738f0b53222e2df24f84919d65850ef436bbcbdfb5519995a475f07ff1a50250d44aae1a770384" },
                { "sco", "e57768c88f976e364044d85bcc67562c95176041a310aa7eaf53970403a2af5aff120d131513a20eeb6bbe06cdbc0307eadf149d1d2eb3cb1886fc6cecd7b7b6" },
                { "si", "8bc995e861f0c348e321f7c06c49ba523b0a90200d59b561ddc15a21f5916c66121c3882b3a5a7baa693ad7fc29e75e255219c62009c024696d9364e23f9137b" },
                { "sk", "3569850cd12435a18909ab5afb09f1ee086db96dca74ca788e7dcb9025792245b542602a2663e1c2e2e054d65ed79c0ac5ae51c6635a120f47e25c41bdabf3e3" },
                { "skr", "114873f94b6635d678277cc34b85747ff5720f9484eeb50d9ae3d78fe9b7f132235250b939b9a6df55841afacb2aa7372b5fae29693d1c27957a76b42fcb2fbb" },
                { "sl", "8d00f949246da1ef9d79632cd290d366bcd9c4a44bce7df217c12fcac38da223a455cb1ebb3989da969222db43c8dfbf79c3b8114af568ee1413250fe044cf75" },
                { "son", "0735794350f78231a972387cf9e38f251d8a6099e0e4835b57aed9617c6dbc5864900519afeacad2bec842c99e60890b5518c76dc9648266a4d7cef2ff9583d6" },
                { "sq", "c03c5e551a0bedf3578ee1563d4e6288afb4c6efec3ea075118aaf5d565b5d19497f0d9fe1a8b43407af9da12857600619842191b4d60ced26732d8e6d60a485" },
                { "sr", "983932b50d69eb1e332dfcc3a292128996a522a001ef5afc2068acdf86bf79027306d538c7650f6ddc7d1e1a991c4262950c8e8266ceca02f076d76378033516" },
                { "sv-SE", "ae018578140669f51bdd91fd6748228ef7d75ddff68a89280b15b6b96ac3dddba5e67d012aea13c92642317adaf0bf424ae6a81b030cb081cf819ddf1ff725a4" },
                { "szl", "f14cbb95676e7c0885a11d962e078630c40249a5c0cb0cf34917ec5f763bc30bb9147fcef58bc9f3fd5f9a661832838453ea9aae0d477c6fea9c8c7c8bc03db0" },
                { "ta", "85b68fc5db7da4f2a88a41f50bd6b14a0b8ac07b3055d7a256508d051f555f9761899f9f9bc5aeafa4e588951af99e878a264f24ecec2b5d454b1ce828978bcb" },
                { "te", "ffaeaa020a644a05f523048fd6a06a6d1801e8a24ae25c56ef3131fad431db139ccc8fc0f16ed46641000bc5262acac1d95ff9c74377668e3880a9bc77b98e82" },
                { "tg", "823718e3926ea21e0339144d679990281dbd85ecd8236e171bacf293c5c0191e70c27da119548b616a49dcfbc9aa32247dc0979b9ccfc3b2fdc3ba3e47da1b5d" },
                { "th", "d1d509925d22a6a2d73aa20ff9a4777429cd2ec0bbb51eebc6aaaaf0c2da822c89b1ea3ea1c2a34c2fa6053613e6e24614477ff230fd30b1b531899efd876be7" },
                { "tl", "c80a24c74b7dc5a7ebaba798a9246dd3ca100f334792c90eb5b23ed09e007e01a31f85f7cf9e5a9444111589346d0b5b9160d2de26bd0a72e9d649761bc1aa97" },
                { "tr", "ba6b62a4455ac371ee65a5586a8599d03b9544d8820e1c1e976185b9563ffd681a06b512cb43954f8e12c1397f7ca17b5780e30877a59d2bf23cc2ccdff972b1" },
                { "trs", "111a14b8cb31ed4970c1963e62687c233be273c176cfd7928a93756594884478534303e0d677a269193b4139d3a240ff8550e49a18e16a343a1e64e96337faad" },
                { "uk", "6d081aacb1032b500a5b3a28cc61123af7e807c34d5f9b85d74597f4056afaf014bc99977ce213e11baefe40451c2ceb0358c6491e5a4d3968aeb81005b0699c" },
                { "ur", "74248f44ad79c6b55b12c19a0bfd72be84654f7a1e481b2b2114e15656d42659ccb180d00b7c48055c500cf1bdd9e8aff70fd6e7f0c11ea76e9acd67b9295de5" },
                { "uz", "40c216981be4f05bc35edb9a7db8acede5416d362fe289e70e5d9797402e097738b66319c47285adb3bf030404df415d4d02810693f4ae7f1eb9693b39bbcbf6" },
                { "vi", "0c4c36d526acbcab8d53df46c9c4f5b329adb3b60ef8b8e85e34b18f47d2ada1e9dfb3b9160a97637643ebe40f1e91a7ccdaa099df68d31735461c393ade51fd" },
                { "xh", "5392c15a3a616b91f10114e090ae7dced96d770de60a49766212e6ea58fb1b9385ab785b9bb51a4ee1f418894ce0da964f425787ba3698c418fd05d48dff310b" },
                { "zh-CN", "9d863c51fb33dbab87192577d1e5c328a35e090ccc1df643ed95771e16b997cb17e5376480db10b226a51be9215862c874ba638544455a91a9b98a8ac6f58e43" },
                { "zh-TW", "5e6f530d0a23d978516a573122c5b0431d15dbe76df5afeaade918b85eb737b71981f65dc453f4f5692e19ff663ba8bc74951f186c2625c49bcd940962b53512" }
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
            const string knownVersion = "129.0.2";
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
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
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
            return new List<string>();
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
