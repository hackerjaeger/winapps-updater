﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "124.0b8";

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
            if (!validCodes.Contains(languageCode))
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
            // https://ftp.mozilla.org/pub/devedition/releases/124.0b8/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "d9ffae7c6b3ac5ae8167cbf621e24c282fb9be5bae93fe32f1de2c6b2dce8e0b5865c7ac4ed901e53186819a306b6ffdfd194bc6a6ffcf14315a4401bee336ce" },
                { "af", "4b14390174c7ab1ce9ddbe09256a23458c0b95ee3fa28ea269835714258892f2f40170cdde7790d3582e5ed285b149e3d9f6dd106f38b46be6ae0fb3bf8efe7b" },
                { "an", "bdb4e1379a3507f77954a198063241f4ce50f5a6eb64dd651abac5d02179d5d18b8b6122b1a4767ab93c9ba81f87ea4a6236eac1e7765e84e288a815ecc83fe1" },
                { "ar", "28ae0d38b5686c11d864033ee91a69e988f323c8acefbc5ef612244ec58a9051eda2a156bafc28ab4216504c41134e5588ae5f5f0a627c6e2b0de1552b6b56c8" },
                { "ast", "0cdbf52ce0417e824eaf633bbe8925416fc20acd683ae7f55ac6fcb05471e9d07f3b200f6faf9778a55f30272dd7a27bbe3f7dd506a7e58f32b89941ef3865a6" },
                { "az", "833bd050e67af9e877fb55809a0e833e16137b9d003157fdd29894f4853888ffb56d33fd4b9bc39373fdb8aff3dedd6c52028e641fbf8b398831fa46e8499a30" },
                { "be", "c8761f6e222dcf3769b5885a2ca07fa251cb1934bd4aaf28dbdd67edd327e33fa2abebae61a6640137f737214b13098ffe9a5ada33b4af523cabc48602b46738" },
                { "bg", "1a8ae8fba4764e089cec3b3468fd4955e7e627fbb4b040c5461912b6bcd270cf141879a87894f53e7f19af58b2f8ec600e59b2e4e757e511a0933baa74ab0c4c" },
                { "bn", "de18b8b1707c5cb39d53ff51b7bb6dfb50604f9cb93505331f175f23e896d27436247edfdd4180f09fa84dc443e418bfbab711123265c721d2850af29e056876" },
                { "br", "0176a7e38b3441cf87e2e64cc62ef48975c625b47218c4e34e2424273eec1aec181ac88e4bad01e424fdaaca8aeea0c51f55c4a3aa89e14790f7aafa7cc341b4" },
                { "bs", "51f73d5373b7a18ad7c5d4c3fd17f04855704e4db127ac485c5278c237dbab299c755fb44e2ff01fda25d5e50c603a24e3b53291431594760857aa3821c8e9f3" },
                { "ca", "03beb550727a07ee281923a82352da4c41cf206aabfe624f7d0abcf88fd1384d901bcca9182610be8b0b2f9a988ead816b90eed0b9031627a911a2b093ecd0a5" },
                { "cak", "0fe0b4b2e213ff97126c075ad61f4694b4011a758515d694f8d58d2dcc5066f4ce2aaa0ba2ffc61271e150f24847ae88c08ce94a41731735ca84a9dcf4a68d60" },
                { "cs", "db49a971f3b8655d63e10125949b37bab30cd846c35c10bc79043d19c0154746b98e746514952978418878bb4ecc5180c021264002cbb793631f760b97093f39" },
                { "cy", "f502dceb99d6c259c87c44dc03826ea39254fd515d1b4bbd0b63b2b8a6283c8fd0adbc2842f6e8019cbfeef6e5d479bfe9688c1e3e145826fda881282961b320" },
                { "da", "d72debfdc7c8fa6926bf750d3d1974992b294c6c15adf6d36dc6db70e7a71529c15d4abe12e1b8f59c1bdc80ec7e2ee1d94c8e2bea08973e0b211d79b06952ff" },
                { "de", "7203a28863ea3ba4e494f3464f27704b075e46513abd28ed8ee177d4c73f99191ed1549dd7f1cfdd1c44eb623d6dbdbd9c509fd2cb6b296c00b13fdf013a904a" },
                { "dsb", "088b7cd3ab337b9d5771e55426fdc754c5e16b6b935480be438335afef43d1e9c160b897c4e4cc451a6c0082597bb7872736915c4f99c6e597c7d43748cfe5a9" },
                { "el", "fc29f269fa0753f9d4687f09789c4c728e71714066542483e92c4c0d0c24dc5d772e134b70900a049d16ce17acc537d34316c36260d51a969a21c684c3492a89" },
                { "en-CA", "c26696c9bca4a6cca9958d01e7be7cfce0eb75feaa0ecdfb38da681dbd8c68e1985e4a9a6bb74331d71dafdbf4548839e2e52c0464d0ec0e932cc8d7a1c8c7d1" },
                { "en-GB", "0d2bdd483d76e921765a87caf15c85daef26139dfcc7701b813a1aa042557de36e81f94684dd818b2a8fef56b3be953ab726caa3436a8dc603a875f7786379a8" },
                { "en-US", "989eb107f0b74a9ef22ba7471a9c883870dda0cab7f107dfaee71e9972c57a7db50306c9784130e4d8663976df5cc7b8e9a0214a353205972088f80395ee37ab" },
                { "eo", "fabc70695ac2eb450bbc88cb333e493c08aa1a14619514627d1f0ae1397835b4760e95260a9013fefe5c58bfadf063eea10917b2597ffaa05354df72569f3b04" },
                { "es-AR", "860d88d58d2eab37d95986dbc378c133aff33caf80d3889848fb87d48a7c6329358f7f8a98ebf60c124f5740712a78ffc11e34220c40e2460d5fa34cbbd74def" },
                { "es-CL", "dbc84e2cc790af42172da70ad616f3f89e24cd7687ee654a0edcf678b992ac2be524793b901e43cb4ec7acf22c5ae40bfe7109fe1bdf3664c35c9d30a7ef8ead" },
                { "es-ES", "df879460648b69b0378e263cfd1adf294a848d36bb27d613a58b6e70a493e10078cf88b2bbefe602ea59ddd6a5102bf89dae8cd590b15514a70513bec5196455" },
                { "es-MX", "d9c597ce15678b23cb12969c0c896fefe90ff69d96766bd0fbb61b5737730b443cc78af4caccac956a034e52e0c33b485704ca74fa0907c5d7ca516ce21c125b" },
                { "et", "3b1583fa7704bfba338582df5f657fce76a2ae527b73df4fb135b2d3c8fac28f5d3cab9ee1e207d79215b669c7f705289e96680d5f65103125758bd8f94c837d" },
                { "eu", "e014032ddba66198df530d3d80a2c5341b02ff708cb5f6c308ff52fef522cee8f9696dd82ed099f386b97d36437fe513090160fc70b11749ed04c824c7943120" },
                { "fa", "b52130f22d71c676353e439db567b402414a9e99aa73ada4b3f46880c342dcaf127569dc6c29874b3ded0ce56a8082b904ef040333c666e62e3da9ae105bac4b" },
                { "ff", "240a6b1c2e45695e45e476595397479632bfd40e2ea8ec36968ef6b8c30b0cb50c9c865017f2615fc4c164958dbb697d2bbaf563dec99ccc359ec286b00f72ae" },
                { "fi", "1316b8fb95bacfdcc19074d6de617adece98004b862144598e45bdd9b38317bd45e79f3e1933565ff0ab8b9bc72dad5c0da8f1a7a0a81c7a7511235c58b94585" },
                { "fr", "81ceb80ee41d73b6d18241a1b0fe721cd9af7ddecf5f453fc877f227de32afb0868d19b841d9c44d61317df42f5925ae9f116110604160289f73711ceb8cfdbe" },
                { "fur", "dfddaa6959a2d14931dc663237ce0c52882dd520ed847ac8842c4c23393791f3412f44753289567d12f809e9b73ea6a90b4183b8a434d0ee711ed9ad27eaa196" },
                { "fy-NL", "931386cca35c6737c876f494e81692d5ca6a6d1e537b23431829550fa02a96631950699a6a0b0f6c5cacd53efa9e34347bdc82f5d2938a14748c57e784dd8f28" },
                { "ga-IE", "473ead8c9f1f1e94c9dd55f39c17044608c78374de8dbeaed78de650f106549596df61ce97622b63628cb75a01561d7f94bcda1a814fdad96538e615adb65525" },
                { "gd", "c35aa77da5c6dcb244468ce0856721448481a667fae58300acbd495c1103c8b5f56f252bacd5cc66ac71c357f024d5fea27411b7bbf7c7678e1b5207f4f7be8e" },
                { "gl", "b1803b5e1b71fface7d4642bf6b407e23eae57b5de6804eb997a0871df93baefcfbf5f23e165fbd4277268a5bda328a214a75aea2ea7616fc37b0418415b3094" },
                { "gn", "a2f2cf08be0133c0741b440c84e4dd4ccb7ece8a779d3b28f8cf765db86098e24bdb13438935731f50ef1e9acfdce598ca92962007cbad5e447e5e6fbb97da7b" },
                { "gu-IN", "fccfe1fbdd6cfbae77f12745688697a62095fcd7ef6b39e3946f567aaa45f410c2e10b210d4b00fdd70424b55803112621bedf632a531c170447d6c6eb069417" },
                { "he", "da6a3b67a3e39e468a3c86b5eb61035df002fca931e2c0f8a5d8249b10c5d537daa43c2c0ab5365e3ed885ed746aa7389238ca41a441c30c79142a134a4ad86c" },
                { "hi-IN", "28ca17aa3f0efe5a1639670c79345b13be9c4aa18b99a4bc25a49dcc3db848ca3c6851a74713ea3cc7c52690e05b7db2f1f99ec64f86b82171732f32e77c4dce" },
                { "hr", "82dcef3d09b35a7c2329aa1fb3597327883da8eaa8ffe4a3f509884890d792869b22bc2971f39a9d9ebba33809a7a38b412c92f7ab62a399ec290960a6feb565" },
                { "hsb", "9de2a5e444dd05ed9402aeb5bca28d77209ddea4e680523c4b3604f716229435831354aa2d08d558f54192a06d20aa77dae07c7f74432ef46615065f7ff5d1b9" },
                { "hu", "03c1766666b617a7303fa0e57f42d3fdad23a8f3efaa02799092ffef6e1da9ec0277c60c21da9d942feef1ebb9392eb879b3d45e6b1921806848044b6361a8e9" },
                { "hy-AM", "5a47d22b760cafafb9d5c86484c4f44462da0c80f1eba803ff76d1e0deb52e595dbcc36b62f7482d4a9cdc028d20a8be28b1af697ab79a793f6cdff2a18b78b6" },
                { "ia", "664c11da7d0e1272a684c8c559967a5acbdf82696fa51e62f48d4ef4a826b8c324b4c8d8ef070fab1c57be47736dc157f554d410f21de7838e460a2907927852" },
                { "id", "71be5a23b7efb9400c8317741a046cdc6682243b45d46ab7f11ac035af5eb6cbe057396358a21f8f56333b54453976c7fc4cb893d86c009c3b3d3771c7cbf6c5" },
                { "is", "fa526541107d75383ced464886509dfaba5beb1351933af62bf85ed3f06c56b12e9fd3884e26f889b6d1034f0bee1ca8b1fbeac95e3642b7b2f1583aee30ef5e" },
                { "it", "405a173a8113cd63b6bf865c79bd5c56024c5553df117202139a2b1b2a11436769d5e0ea4f8a343d74d875ce1ed62503e0e49acf4e846a89eff39be6ed805d39" },
                { "ja", "e918d1a1fa819f0797b49bf3d840633e30b43d8fa010d5468d33c00955169000e68c94dd34101a7872fb712e8ad4424f95e26364d2209ab2967e9a11ed5f2d19" },
                { "ka", "7d093e525b2d16b40596498a7a4b6cdb5441063b2480dab3c05ed25cd297719ec6e0f9602dd7e8c8d5e29da455844d4252131978a5f773c158d640c0cd3993ad" },
                { "kab", "9a37af0e91fe7947a3b2e5b4f683f0e354754f19479b3d66722955a9b90ad6953830c281d74f8d898e2e47722119744f1b4be18d7aa227ddcf679672df548d93" },
                { "kk", "964e0991a57785f1151c2fb21994d0a89c6f228829f0d22f8806adef87df681986a262901d322b9707c794d009589bbdfa4ea146950ae2b52970320a6facee4d" },
                { "km", "b667fd66b8b64181326dadf13a8f6cf11acde134b3c646c291e7698f7cf212f7db8986294a2bcfeeb61e3a4614643dd8dd9722085e62ed779ddb2bcc98a4f4fa" },
                { "kn", "9afa6e10689a5eb11a33286c3d59de1f7e3bff64107d0bf299888c6223d2a465a825c837d43eed427185c2e3664d588f2d159f1854f7535dcd59bc7f809ebb61" },
                { "ko", "f8359c287014960411455badcc0bd5e9aa770a13428b88e4494870a9aaeb30c62a53c9f7a5beeb8201090dbf24dcd53ab2e766950b8a8510035436c9da4b089e" },
                { "lij", "788e1d511ab5f287e069d49abbf1d24da4a6a5034e2e7a7db8e26b035427dc704a83f7b82d176541a15951e5bf4cb3e5a84e23795868998cde82224b7570966c" },
                { "lt", "721ca129b3ba38dee417ac23a07655243c88d2ee62462b5424d82be5f98d88e202350e3e10a1a346d1e8fd4043669795238295e758159ab4fc12ee0b4077518e" },
                { "lv", "8d32b992ad467fea6e8a33b60e6f3392fcd16d80df90628ccd868721ceeaff86b78c0db4368c01227aa8e90a228f3b14723e26f47d4ced57ca05a66604bf16f5" },
                { "mk", "dd797fd5573763b64820bd84d6763ec475d5f3eb111f93bd4d573e177fe15818433182e3edccb4274747c1f519cd906dc0d306937b2d557aebc4adee182c3464" },
                { "mr", "e5710d37006e6ff169b0a88dd5d76c5663225c7d2826e9aac476b3268b3a7a52e7235aae4f8b3eacca4190a959a68a293fbeaa26cd66adb5079810f093e701fe" },
                { "ms", "d7cc94880cf16fbd81aaa008c27fa672f56e4427847299934bd95d0edafca38c161c43e2b38f3494385740645cf44e2915c53e43c46505ef44b7e7964308c49e" },
                { "my", "8c2bba7e0b902af8f450a3801b629b3951fd483fbca6e40e01b5b8a2bdb297ad1ca9fa0d070e8244887dfa523ee4b491cfa6ec7381b72af026891f7cc814cddd" },
                { "nb-NO", "ba53ce71f08833f633670b228a4080ef5202e5be6c17d5180e9bc0553cdd499a9a4e54565c6311f4d0b4f473c7f0d4c41763f05c511004feb69a1469217ed2c1" },
                { "ne-NP", "07ac545037341cdc61a93e6ca5dc3078b39e2fbfae0e1bbf40ae14e66256194d81a355fc0187d7a0a1efd31dd279138615a9b901845a533bfb69e24fc4398653" },
                { "nl", "8e12fcb0930cbb8c9922ea3318da76f659206ccedf3fdcf90a792e9313635cc4ae2f560a9cc9e1c1ebe0b46f4a97dd41fe86a56833d41e030fc8e88cd0338542" },
                { "nn-NO", "79cd005feb61aab79a11a9af57b0aaf34d53f034016096ba76ecd1dc57a71dfaa7af3757a77c3036fdadb60e0f809d2614cb35a7349d75b4e6c371836ac83675" },
                { "oc", "3425fe860c4761478b02446b542628b207f2d02fe64ac297c7e7a21d4dd5cfb0d013737157c764ea7671a5ed932b784c22d822969d9ec759601e3e71e1b80e11" },
                { "pa-IN", "aa1f15215d78c8673a96e5cb1f0d9767db6b1fb24316914381c0709877552ab06ea9e2bfb5390d9e3dc0ab65021867fb4e04c010e61050801ca6a30142fbcffe" },
                { "pl", "64d426abaa2162756d52db069db74c401476b1dc51b262a73b339256faa4e045d7567851b8644d31598031a784442aadb952498d6d050593d53b1bca196a2cbb" },
                { "pt-BR", "b8326d3fbe409a3deea9bc07a8225cb540d7af163f83f46c85481664b793f0521cca738460677bd0d63f965f232742cda04d65b625cecbb61c9f165166079760" },
                { "pt-PT", "c87762a510157802ef82b00e88236429e62403020417bd3d94c2e773a362dc025478d66d031e5320ea6d6ca59053bb56932b78d762e0d17605e780cbfc842f2c" },
                { "rm", "98046ec79a3b2260bbafa82bdbb18343b7d2b3e09d5484bdbb13450e4f3bdec393a019749219a7a79915ceabbf01a92c0ede8132a3d5b5c751321ea4a6d99bf6" },
                { "ro", "2a99b823ef4aea9c536f27529fa66466f0d13ad90033811b519be0400d91d46bfd40b01b4cac129f7869f980d0432a2f1e834d578750d0855926e040688f1600" },
                { "ru", "33789098adb2bdd9943e0264d6afde4f701b2109fb3907136cd3648283aa97d1c9fd443feb45a0c0d912c3cbd2cb464e6c8d54d99ba8ca3b678fd0bdc7f10e2c" },
                { "sat", "11e0d9514db18116b379793319e6cca486fbc5aab7a706ac2a9671144d7d83303465a12f1e4a2c9028ad46cc830051caa84121a04d1782e616199caa0fb47151" },
                { "sc", "8e84969f864870d4161d66d8fc608666869fa96180e8f1cf958137b9ef0366126682d9bc29bcda09d6eabeedd9fd11f7cade8531b24b828098edccb6174c4c09" },
                { "sco", "fe5102009f7e506f85df64b7149d1736aafdcf7c25d3e9a9b281266579f9e7e7c2af5d49c9015d65f7549d9db5720e365464b41d072910698b783a73a1db9a50" },
                { "si", "221f84aab6e3c47c501d4d828e77fd5407f70e31f51adf9b1313cec735d5b4a372fb171a74612d108592797925a17614be1735f5eeb41aaaf818a09a7a2a345c" },
                { "sk", "022648c91abf34f6f414e3100beb3569caa248cfd285d26773d32d56395439728c47a289dd205a20d924e4101da3b05559415dbc4c33ec8ab84c95a3dd2e5969" },
                { "sl", "7bf7dcb2d754c6547ebc191540a4e031a43c3968782111b97fd6e9463930fe6cba374d0dba0f42650f7469b0c1667b24c0697d2082a474607cfd0239512a5050" },
                { "son", "8be587fdcde9ed6ec0eb94b03b9aab6371f2d8d62c75edba5d50cc1dcbb071f32791be7ca5b184e052a74313c73df6fe9ac010a6a9457f60ca285a55f0394ee0" },
                { "sq", "56445a60c5640253ee2c7f5f09852e141e96954c0074ff381ef14ef5878253a4cf83f0d5c78bf8f88ffca9ca25c26856a329ed219f069154a4a3273c08cfe048" },
                { "sr", "3909e9471a64985bbbc5e546cb9d8c001052d8c5b5cc77918ed15746955e71c38a909892ce3b5a61bdc87d0023ec31cb171f75e2058c5b9eca7db9cfaa3d4b41" },
                { "sv-SE", "7f1f23dac49cad3427b04c8a3fe3f7fbfe92965139a311ec34348c3297375980259fc6fddd398fb9644d426ada65b3ce34ef3f47eeb3b72f2d570a4af9dcbf80" },
                { "szl", "485cdf642b0fcd286b9b835e031e2d1c548150a36e04dd8534dc54b21da43b8ddc116eee1cfeb9b9751d5e38adc4872f5f325a8fc2f4222d308655a0e659330d" },
                { "ta", "83b1706a56910dcf31297750517a8bdd7f2b4674de516c9dd89d4fe8e3920dc5d1786ebf0e0a7e6faf7af6a23c808ac13b92c140195f12eb1859bb9ae2e18a9a" },
                { "te", "e082a5b2ed3bd2cd43065bdc6ac40d98c924a0973709bd39e3f8eb9aabb4a7c542cc235f250e14d6bf731b6b898cbba3f486ea3e30d3d9e97defcde43b2a2727" },
                { "tg", "7aba01e27fea1aba85af03fc645abb43e63ef2b2d639851cc6ff985802be67da154e1b9d5b29a2950891effa5b6884a742c775b0bad94ea1215fbeef9dedaa52" },
                { "th", "dbb5ee0095bc0f43a137e1a4353227d9ea37d8697b2a6d7f00c0bd96ff62bbe9d499254aa7ad4aa8805930a15070b559f51e7300aa64e1ceb23a2b25c4b50c6d" },
                { "tl", "abb0232905e40720cd810245405a5c762895384ffe9bcb93e9222db5f73ba799e2dd4a75ae74d1856e41a56a54dc5af47b79b32aeab2754dedad2b4b69f3640f" },
                { "tr", "1260964f80e6d4785f129b8ac458963ca87f567d747dbac779a335a2a9bfbd255381edc3d817a754f26a63644ba107583ad347748594df01016e979217bdaab7" },
                { "trs", "a8492f1aef1a8b77b73f406c66040db7ee14030ea5c9f5e6112022f618c19b8605b836118f39b7277b298bd3c051dd89d1a8215f81cf0b7d5668910d57502e2d" },
                { "uk", "49a5b6a41ca655c693d1707c2d3509cc7a6650147fa11202bd7e5266416e2dcfc1c6934536a2e141e6197989e694cfd4a89ba1493f50a313f1cd6ba7c28019a8" },
                { "ur", "d9b75287e88017dab90e2c0feafd1cd2a3778b7218bcc1dc40a39758a8c3372a5eb982acf7ca77b76446c0a70c3155e14607bc61549045bde3cd96100a164f3b" },
                { "uz", "1f90e80548196336d2966d352d66c5f8e4edcde4284f39e4f871c75bd834e12cbaa6acaf9e5b80520695f52f277ffbde3cc345419117271ca7885c15490c82f9" },
                { "vi", "692e120d41a0d205d8b3b00b4c06268f55b921c38f39721ec237c139adb27a0c5868befb6d6c66f99995d821189808164c78a7228dede0eb5bda4b1bed197d9b" },
                { "xh", "0f7ceecee3c03eb5bf6e9e6378f66ddd325db40b693e8c23d2e1e5160cb48be7de3c42bd901e4fc4fe56e118fb0c3038b6805f20125303f309422497fee15330" },
                { "zh-CN", "39ab0f3d97dee85ea5d2e58970b595dd81c913cc0169fc1265319879739ef2e53746f04c4bfeb3973836c36855d818c6e494fc5e12ae8412613355e8c57b71fb" },
                { "zh-TW", "dd531d477d09855764c31b10052f2c2a43774a637f5cee058675af123781cf1d27857f205ca76fb1dd9ef110831b1ddd98b59e53588829d9d87a4693409eab1c" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/124.0b8/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "719d760aa36e93699ffa419d6d99c8731bd6ca157c09f11be9c0234b08f9d2b62c6533dd5060ca4e3be26b5aa9979d1713c13ddf8e784c8f9d976c6b9c32c7d1" },
                { "af", "86668d9a3d4cebc97d9e5616c126bebd1530afff730440a25343622d15e4bf4135a4b79bdea9edadb49eacd657f7f69dd72c56cf633553ee9b751dd0fb8d1167" },
                { "an", "38a48e1c85eb5b60b50821fe2345594638e71a39ea48681b1b64653a92a997b3a3f15037d7bc292f82aa9802325cb07b272df9e4ca6a9d37e8a9839a625bf7ca" },
                { "ar", "b0a7a0212ff85b697af4d8b75730f38a2615e817a54a4bd7d30a9e63f59a467e148c802d3d6e704a86a7ea3b61a5d27481bb585774295be247d1cb44ee634010" },
                { "ast", "9bf56107a6cfaeab8eceedf88579ef13e7c4e2a07de6b39a829b19f539d866861e52680d7ca9422843a1c5aa09dec21b119aeac2aa73447d20c80d9d7c0a51a0" },
                { "az", "6d83b4eff565960e9340d68492ace80029a3911ec39b54be710a047d67bcd8ef76aba48f6ba9188312c5836f8aeb2ddfcfc6539d29f8eb90dece7f62acab5dc4" },
                { "be", "89aa97ae918d74f491998f8740b2a0fe4506f8ebe2804b2fd836901491ea533bac02c4eb19b67f61b2bd9d8170b4de8d41d6795bb955b677b5b091d68edfe6ca" },
                { "bg", "0178e3cbdc6270c11f6161859eefc69a3e86cfbeb3e4f53b2a7820db1e425bcbde6d540d317ebcea795cf79d3e957f22a7248739ae9f79a4739014054f0efff4" },
                { "bn", "13a5b1aee624dd59c6419e87e1a2be8e6b4b71937fbfd4c074dc58aff0786c75f606b9a8e89a112285ad028479636ee12ea991b259289c7269beda8a993c6f20" },
                { "br", "38117512ad788824c521ed84ba1858303cc8ddacae57394feae796e4d55e4aa93c0f41972a2b37518b6593e315233eae60e0bc911f5f9a861774db8f342ca041" },
                { "bs", "9e8a0b04356288bd8b676e327f832d127bd56d7dd4147e5c368e159a96be006580decaa111614970ba3bf3b0b02a749ceff69005534b9710b2f0a30222204ba2" },
                { "ca", "d748dfd010c14587dae863348e0b4853d60f705638732c777ec0303e8f25f6b780bb530a506f1b5c63c1f6a807e26e3e6ae6b5b3887b4e79d114afb42e636bad" },
                { "cak", "e9ee05a83b98a6869eab088f364de6785a64b5c2e24137e0c6c2528ca0dd3aa3a783a8ca00494949035f59128b7bd0ffd46f35cd534d75227801800995f033d4" },
                { "cs", "3df12c19d6aabf8147bf7d35dd7540dfea88ccf1babc7096c96a063d9fe357ababed2b2dad3037eb78262b29446df1bd716d9428efec112fcdd2931ee8cbd8bd" },
                { "cy", "e04c9d7748ba0ef4599b864539eb760a1284f90386bd072527d3f1a670fc092f85feccea5b1bee6c6cbd49801ec2299ef035bc0de1dc4b83f36c52cc94f77cb7" },
                { "da", "25c874f8aa33e2b97406f791cf14366f87006f12f7d46a8a1be9e384c27db2b9d743d67670512ee3e76dcdd9cb6e1518ae72d4f501d47efe3bda3438587b3b02" },
                { "de", "aa1ad706f8a1773b2056df62c2f8529385f049632ecab3c16f97202c7ee12a3dbc6aa2bb875f9f10dc611d1bdb299f09c35243029af5be4b83bb7dccd441d140" },
                { "dsb", "6b493ccf00c1e182f0fa0152036993aee46a7180cc49ff69d5d34377033c35578c784ea1a02ed929a42cd57c3d9275b959849152cdd3cb8ebeb056fb5d467598" },
                { "el", "99571610f714ffd5eb6dbac8d912b0da81e0c936416c997193c6842dad8ec2cb4eb677b00ae3490f60b705c93b403562510fd1acd7389b378cfd6040d4a3d83a" },
                { "en-CA", "cb371f5fdd3fe18523b8fc4218097210ded31b0e523c9894aa3df6b17f38ac93348d38fdc11c78d31b8d6de600f02a9dec8e860c38169f350788db3a2ab9f750" },
                { "en-GB", "ba019f7b7c2ff7ebd76f965df73b51d0393909a0b633dbb90862ea46e0eb6ccfbb353c52e3b99c1d2f4221c183e53c42b4a8a0d5ee7aed4178108753b9e1bd34" },
                { "en-US", "953feeffed405a3205ab16b1f3eb9b0b016b30241994966f12c4494a21101d8a7c6f7bcf67b08ac1c133ddbca050b898894f5a8b38ac879c3ff9d576d0bf9eac" },
                { "eo", "3d4c495f834f56e4f028e49bc25f66b21ed62c10e978d19167bc9261431a0fab9330c1162c17c5112246abd59b86d7b77e129fd696866b88887060db343d3cf9" },
                { "es-AR", "6bf62c1550fe900ad7c97c70388cb4a283b1a00873c4c26c7602072e58272233ef1879833c28175867501b85d81b47c8e4744fb9a8ae1b810cab30b34860592d" },
                { "es-CL", "2af96901f87916dd9aa2fcf1e011c363e11a426eb40396f7fc19de72bbd9e859d20fdc61e41e51182597b73d211a6f9113be025f3600ee8c34e900dd602fd16a" },
                { "es-ES", "97ac4db84d27ca275ede6a48fa73386b81ed65ac134cba0c0bec810f55406d0a2f2712fa473556bdf0627936f6f0b386cba43e915027f78dba857c9b206e90b9" },
                { "es-MX", "72a93e98e5a1d9ff25181d9101e386c6dcca18be2961a0d92a3910b179b2ff151091b9c06b7f1640f0a15c9a6304c1a679e41f7520a79d21b58d5d89a40c9813" },
                { "et", "f3807868ea3d4f10860982446c9fd7264e994a1c4e2ef7b06d65e8dd01f116b0f0158de96a45df9ba558657374e09da3b7c8902583e55c1150bb8318b7c7f00a" },
                { "eu", "66f1609b98f66ca69168ee57b70efc17afd493ef2bc988eb5f0c976ee7af344dedcd9e5ece55b579c8450d7bcefeb4cab0d981eb607b47945c613216b2db3d04" },
                { "fa", "267d16979c98fc3ca3a011f27edeceec752fd1988882faa65149304b043dfe54ee17de33e70ba69bbebca5ea9196ded846fabac1f9620cf6b30ac8f06349fef2" },
                { "ff", "604a5e7c516be797b8d1d4701a6cd8be8452e51a57028b7b4f82b9c6011f76ce96ada93c33845679d38354862e705cf0c6f56ae5fb4f76795e7698c0a7422377" },
                { "fi", "7ee87daab93ef457a8c3a6c9ab675d951c1ed734a525ba158030ff065fb5153640a8405290c0c81dea78c4400537ac8696be30105143e6803000a84d80f72638" },
                { "fr", "41a3efe512c4b2164216dc503c9fd6ff526bc29d864965f1069c0a58cfdcaf139c7daec394138880f660e2d8842f4885d12c082d393ff0ca5baf7c37cf2daf3e" },
                { "fur", "23b2f8f636feb20e21310216f9173de953fda0fa53ab63e6c4808e5cc314c1cff8ced53c9029670e56b152b6e6fa9bb9bd2f8789487ce68655b13887923ecd76" },
                { "fy-NL", "5479954b339a0f4177d5b275d41c2c5d984043d2920414ff7079cd02ce4f74f94a2ae6d5a86010b4ef49c5f746b8c39b3596542f4d071a22abc70c048e9b3267" },
                { "ga-IE", "6487455b8db0d39c247b7e9e2f6f99ae04276af94886568ede64a9ef351a6c378280e7dd4bca2e6fa4abd7c47118eabe7f2b76b46ec02971007cd833fadecdac" },
                { "gd", "402943de45cef6f08dd4a6498f10d457e97724b0c843e7e42620a165d57c95071060cdc9fbd254c70d0d8af1292721604079aa05bf6e9beec7160a6ed6bac092" },
                { "gl", "adcf7c79a3558c3096411d6c7757e5bcb66818ab13ac95c3859ddf85117f1b2e359267718ea680c8b037448dc210563ac81bb90892bb83869b5d3c9ffc52ccd4" },
                { "gn", "e16d467f035961043df8a27244944b66aa1213df72803009ecef70f50e447f4f666269a13676071472d05fc36f96d2a45321c559feaf4f9c2ed8b0eb191397a8" },
                { "gu-IN", "8550418f50e6cdf38028ca9f22fc4fa2a7a4ab39b3b312e2bd62068c9bd7b54198c8f22d854b1909c737daa71a1476327f9e09296a9c7b6067c3cd0d306b02f5" },
                { "he", "46ca8d272c1c6ae5161376cc8324bcbaafb9545cd7d9e475887afa4df513d528f5105b6d53a9a2bad6787370fea2026af6c5c574fe2fcfde838a936fc8c99d9b" },
                { "hi-IN", "e9e6af63ca78b437b80e7e7647cd132069db1d6c9b24b4b0eb93ec4ff814c81165ed35e498e27a57013655ab3b7b2a3315c52aa29454e3fa72114a85941174c1" },
                { "hr", "5a31a6c4df61e3b558f12fe9988f1f2fd4b4251d80ae3562fbf24a4cc737ee783e74b16e21307e79741c6aea89f15f9564f5bc1c42c329a33f6ef2d6a5e193b4" },
                { "hsb", "0803e1d3142b5fd35596c59d07087f6c50ef30f513dccdfb845ccc6a081f30a03945876a114342c150070f4951c9bdfa1520eb0ec1ffd07b18e55004bbf62d4f" },
                { "hu", "be0249b7d4474155d96f5b48cedf92d45dd3031494de86e1750710cdc0fdbe17e973a5de25f0f996fd3cb13dbc8a79a537ede9762524a3dd18c67af0d438a8f6" },
                { "hy-AM", "169fa083efeb77645a925e3acb3bafa35e72473dd20335820d0665a137bdf3f9e6473aea5864e9b0aed4049b9bbccaf4b02a5e57b09ca7ffbc94e59d35a86f63" },
                { "ia", "146b2b33d9c2db9309758a1cf9cf766b4ceeab305cdb453999f8f2599f1a5f3f513c589833e03199708fdd9041c167e73063ef21fbd72ae1f0d035a7d49ce1b7" },
                { "id", "1c6b9d952fd80390395b8788fbc7c165f61cb195e17a4a0383f033d0788cf61dfd37d91d8a88b2c331806649d5cc18860740119d56931a292442f6adef421d96" },
                { "is", "d826af0776401b55a1fabe438d02f9e906cf64d8ff80ea278565f113807b2ae645aa570b05dc41f7690952ef8f344e99fb64573d4397622196adf90d9c873c6e" },
                { "it", "a0666f6acb8245ad9e74dd2f2ff844eac35be20da3dbb58111f761100156bdc949c0e6471f7cda61044428e2c866eab53cf3a65969d0de62d0b8deb013db3326" },
                { "ja", "8b5274785b06c028b86343a0aefb8104dd1bc6fd8ab14a93278f8ec1f25a2647b70cba74876be7b84ad6bdcf2832e7b1ca18d547981142f9f33b28e7faa359c6" },
                { "ka", "8251cd69432771b39526ff8a4e91756adebe813c24f04d033ef45c196d2ace6bd773022560ed305c4757d8cb1d33af10207839cd18759ad9cc906135d9148f88" },
                { "kab", "e8f91f0e6bb6e25ace4dd90d7e87d779d9bba21f7d5eb40c4b87e1f7457a065677554217bc6128f61d717f8327dac778713530884751252388d6e879b0ba11c5" },
                { "kk", "ae37f0fca2bf20ed777e4de159da1f152ab3f505c02b89fb1f2617385e1a9412c9e964817e9a500bace0905b132fcff2f489cf24c3ba93f74e85cada56f4fee1" },
                { "km", "e4ce377375e432136b33c0e508c42b3132d96e994beac69d5f31b2ab40de1fcb7166aee1537799542b1b3663aa7f6200205a22f9700f95224bdf221724600f4a" },
                { "kn", "b484ba7f46ec45b23e2eeb3771616ca82e468a1089257ae01bcbf239f6dadef2a06a6615deeb5517ecb6beedca9e09b47d926cb197d30b5bb54522e441af836b" },
                { "ko", "132bccb1fdbc148f95694b55a32cdae3acaca6a043c0aa5ec222e1ebb637505300442706d9cd8a0deee0dfe193865d1f730666c43258764a5b7a918cad7668c0" },
                { "lij", "1ac67fc14654ee7b549c059fd0c689f39bf9d02bbaccbfb14291b4953970a537745a99a0c353a6827122948bbee778b6be02e0f6a071adff9d7c516484fc3dcb" },
                { "lt", "2301a2765716a6bc0c467eceaf4ce5cdb4e3f2dcbe8be4cc449360d682529b50f90cb094ea489d9834ecbe4babc33255a95fc0e1f6a7d17d6f6219d685de4aa9" },
                { "lv", "3dfffaca7d743a40b1a4e8cf1006c61f2ae513d583db32d343a3cd046e74b6e04ec56923ec2f7d66af613dd315fed5cd07250a3b03b63d3237ab521a19d2bdf7" },
                { "mk", "585b6bdfe46fb5f2dfa37b8a9cf63e602afbd3d3b428f6435dfc3d34d0b370f2941d720afa8e04dfcf616127814c578062d4e7b26bd44e565c83d930fcf2e120" },
                { "mr", "fd801ce75e1d470dd3997e86c5966e1cf4b90a4b9ac3731b58dd310483fe0d6f1ef0cb2c513ef18256cfb9c731aded604a7fef80ca810ec32c1fc160a1ff14db" },
                { "ms", "d5e002041f1961025111a2f5ac89031e5906dcbda6861c621316b3752084f843ba991bfd485c4c11d96a469cf3b2ae652d1d4661f450afb4e89adbaa679f4a7f" },
                { "my", "acf54e1396be6a7bc9523c111d9c8d6d19e02a7b7d33b653d03bcd1c97fcc8d6d5ddf115c29506890300c81d2abf47f0c7d9eba0af5ae252b2af567f7a0b3394" },
                { "nb-NO", "61e665b45cd0c4825fbe5481d4186cbb57ffd85a7969cc210f263172f030a32f00d4116fa41084ed55b976e8d7a5be65f9eb9a3b5fce8d9a91aa77fb6096f834" },
                { "ne-NP", "e42bbd9466aac798097279d96003aca1b95dce6c89c78eca3954a7d849d575280ac9555bc1cdb6075d01041485039ce806fe5fb6765c7e6908dfef9313e13525" },
                { "nl", "66f622fab6660e0bae39eed4ca00b7900825f8918d98b7dc33a21b5b6eb05d3174c5a754066fa879d75f790f0fd7110ec440a792b3abf51afb60778b65813953" },
                { "nn-NO", "f82f14abff92f5e0fc4650b97305d240ff90636af4639275b0d7b48dea195493346c135fb78864b498ad81510dcb32697274b635c7ad47375f23c87336eb64d3" },
                { "oc", "e7614924f64b303f884f25723a797077e7bd15a1fc519a142d9243f00cbd9b67549787767dac476a0111cf48513a557c432e456463043ab9d86625cbb7c71a44" },
                { "pa-IN", "51cf694fc3ba1f63777bb93681116dd293536e8c22c89fc5fa77ff6b00a0c06b2e80bc37121362a378f992192a89ff34b097b791c36e453f44c3cdfaac5f4684" },
                { "pl", "cedf9d8048de10098f380d3e453b9e7dd097195aa5cd42d213f0673664bd4550b615d1fd249830d02974bba2fe7be075669e009ff3b32d45c22af0aa1003be69" },
                { "pt-BR", "cdfec69a6c903a87c657882c0bb1eeb6c2d6acdd0f31ac68f1790525e096a88b8520f09fd7f5932a55f891725fdff368bbe007e48c54b7203f1dc6c32994ef2f" },
                { "pt-PT", "36d1add152daf561399d5e8d9eefa9a56d20c5b008f1d75b4e3c83e46f25d24d08118a5e7541a1ab69250c37791a42a86de08ec8d369a1d4fb8b440c58032217" },
                { "rm", "93e5b21c9bf2f7b96789960ab990f79a0ec19e54675d902aad0c5db2c039ffe6083da5d850be32f09c5c1e3a7390db678eb86ffdb5ce2ac839bb3a60be58f11a" },
                { "ro", "316420b5f664a79b0035d60385e525ffd5326975150bf167f2c03cae45e884f81843156761ae98fc0861279d2ff7f11de3d58d24416fddbc1397b0826b0f87c2" },
                { "ru", "d60d810a946b554f20264a69c443e8d30af1f51a2167dedc348cab2930eb87cd43c66a7c918a9e19a5ef84bb0c3eaa162a28ff4b908b228f7327d6eb4e9a9b35" },
                { "sat", "e0376342879ecf991c0bd35a5ebf4ee526afb05e1df34a41b7706846ed365bd6c54ae9294db4950781efa4aba1fbd5cd97f6dd2c8c685335e17790a48084ff69" },
                { "sc", "ee5fa453f4f5b84b3852bfe0eac008d2987f3c7874c28d8eb3579f6d90eb27f4c6b9854ba5206603ef7854a636441950aa0f1899d14b9f346f703afbd5e74477" },
                { "sco", "6554b4dd32787ee2959979c6493fb704cb41152237b7332d4f2bedae9eb5dddb8ca099251f810362a95aaabc7a0fe6e6862b61f0536b7463e914e49e995b68a9" },
                { "si", "7be45e966c778fe00089e90730b743c58ed2fb032468a4a7bd56b723eb2eacc8246c10429f12f826ce3cc7a3b7df46a85db10924c19b74378b7ef982b30ee74c" },
                { "sk", "865097fe3865d7ee6e3df4a3b940171211e15239a8996cc08bbf0a88444c3be8384756363762ae560533380355d0ea53d79206ec229d96501bc0827eada9a762" },
                { "sl", "5a3580d9299e854d63e2b85afb44d526226ce4ce8eacbaabae63571797f54234605afe85bc868901c1096443911549d3578e16a69d509217964a58a6bd87e589" },
                { "son", "5c4f4b90c8bf8785ef8e5065aba256c4ebecd57788d360b9a3c09db8e86d0d9679055bf2766561360f594a266d6976566608b2c21e9743a8f3e9291f49ba060b" },
                { "sq", "ea9f85bafbd3cf1a9b890fb098657914d06300f62ac25037cbc8c323bc9d7981e288a9f0edfe4edeff0453f5a8fedb16a43ee0ec1fef6faa6b2107e1da2b1a08" },
                { "sr", "68a4487db9f9335dd594f88f63f25dd46061a82894113f6799564fd776618fb4f4c7f322bb22d08b999621246c8838a73148f5f9f48dbf7cc8715ba8b8f08ad2" },
                { "sv-SE", "3f97c935d20cc73dc9c5d210b22cd26ccf2859f89ca90af9e6bbb09bda3f999cf76affdd17a80296bf07b84eec469f6d81a8598e359471106a87718c0d66a433" },
                { "szl", "ae78971026b863a1f911b19c29b4450ea99219898775d5412df57f2fb429d9e06c5a6951047c607d6c34c54a9144025ccdf36006f2323532a7d38266a7012c14" },
                { "ta", "0e4b2d2f87c40b5a5b2698c88fec00ed11bc01d619afc24a0edd6b2c9786d3abeecd231fc863785968a54c265507f75c477beddeca12391ee79559cbee27c802" },
                { "te", "ecd0eea42f2ecba4123755ef04adb1f81d7b2136729dc86ce3055d62e3bf8bd68f0f1e11b5eaf0826f0dbd1cfdad205b83fac2fe9b0505f813afdbf8a16db088" },
                { "tg", "c7522d18eb213405d0a850425e43c4e25108ebab404361ed138e9677d5ca806c55993e8431e41f35419195ac2b4c87c03ce27b04e677e7c4033853dd08c68d32" },
                { "th", "95f711a99d7b41649398af8501b19fea6a07bf2a6b22a85ac77559988a6ad7b6ae3151434ffb6818bf2207ac609be5911892427487e2a82f4171662aec1a79f0" },
                { "tl", "423659ea93f1833d277ce990bd0f68c880e7fdaf1eb573f64cdb7fb1a81e47e5f47cf376b406f4bbc06ad7a4eed5244b931bcfdb5fc147af8b8d8194cd0bf611" },
                { "tr", "f037318ab693eba341a8ccc6c96c9b0cb6d858c1c9ce3e22923e70108fd54262b57109c9e06cd4efefc61daf55057be72d463f3cd7d23d835f2fd6228a913295" },
                { "trs", "0879693b96d18cc428e2dc4dca56ca7cc8fb409a878b814c76a51e4949c9533a1b14a39a7d4a5030f7e46b7e8084f3b9a1f13bfe686435ae80de202ad29ce40c" },
                { "uk", "29f039c16c23fcd1ddc09b74e36e784e5dab18bd7a14239b045810bb923196b95202fa38d449cbaed2a277a9a4e0355817239b073513285c67a20f548dcdaf05" },
                { "ur", "e8d2672f410bc5911a5a707b3e3e47151f6627d7b718e5340873662b489e7f8a1d888d6718b2e4963cbfcdc6d95f6fbaf3da1ef69cd6178270fce43ccc8586e2" },
                { "uz", "82b81234eaa83c1fcef8e04ba395c7908ece4f5987b6f2925fd43b875a6c5e32096ea80369cae9ae2911594215f9973164a282da5e2043914679dce73041d1ff" },
                { "vi", "4d6bb899ed33a2d7bd45e698b248cf0370d890908fa2c42d8754f8bb4705348247d904eab09aa28ce536058c0ca7e28fa34a7b02f74f554f396e32f1b9461fc9" },
                { "xh", "90ac497ed9a7696df1dff60585bc01e8a61885e58ba94635d92b57ec8b99d252b5d93982ea8f2f5d20f6d7ea636dce68f23924f0959c246fed8ba3e4ba87f306" },
                { "zh-CN", "1c3324d5c4a218258ab679e3bfdd2fce36d481fe87a88277f9563f61475e4a7ac3d237f51a94f4ec13ce5a0faf32ab56d45f5504585f85c672be88dddcf6387c" },
                { "zh-TW", "39644a01a719045366941e1e085a98949e57930a9788c38a9212336b9dc0bbe7da54e3c3254c226a53f2f50505a483033e598a7127ea742f9c2b680ed5c520f6" }
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
        public static string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
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
            string sha512SumsContent;
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
                sums.Add(matchChecksum.Value[..128]);
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
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
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
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
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
