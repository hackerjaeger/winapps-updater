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
            // https://ftp.mozilla.org/pub/thunderbird/releases/78.12.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "3115b5a18f4fb7c31fc8596dab8df61810e44c2b7df5fd55347289ce32862a257d8a8a0f272638df69e6831e8a75024963215dfb85f0c5aa102718bcf84f13de" },
                { "ar", "22601cfdbfc714bd536a9aadfcfbdbbadc3421e2a66cf5ee26adb26fee10aa9f9a9e13001d6f17c2896a0f7474258ba43e2a5277ba36f7491f68ab7a930b7536" },
                { "ast", "5ed485f1836e75540fee43c24f4f68a1f583cd4a48120a0809498984044f9eee3458a094d291104caa0a3f8205965c1d9a2344998a0b6645a4a7f4b17453d968" },
                { "be", "6fe84ef3f0374f87500205181f52b7f1e99a49c035e0c305105793c216486e02bafd25761623e30a000bd079cf94180b3d9a16b2d4db0bebac83f781c81d8d46" },
                { "bg", "341fc90372d3e579f828edf914e74c2a77dacc962df63c4b6f7b6e139a39d562a9caec1c3e610447bcd15df124bcccdbe683d23704629131efdc722e89ab4880" },
                { "br", "f0a565085792fe04c6b2c7821e3c96cb00599faef838ce04836c3b91747f4db78de836d58d05a91eefb5d3128ce21ab62f2e1e8a27293e8111d8c218d453b0a8" },
                { "ca", "49f0ecb6b0d738222ec06ef9e3038411025822606d82a8eeeccbb22ce274e355781bef9d1848f4b23d14ca76b2ca59a8fa58daf87634518060e537fbf14f244c" },
                { "cak", "98f40f426d1e9d362500a8abbb50784040e75582ccd7822e3d2f61d4d4ffd47674193651f28f0a3880c51406e03cd5d738c14e5878fa74d6d26fcdd692b6f728" },
                { "cs", "cc004e45ca65386b6bffbbcd732d6336ee9262e031ba61f6e45716ca54d076ee5226e069f18bb50b07026f12304cc236c7e01616dcdc7055188fb83865f238b0" },
                { "cy", "36f2eb3b7fb425673ae8c055fa594987fda1bce07317e2568acd7897dfaf54527beb2113f93d499ca60302f50dff049068fa07fe674fac195708fae98606023a" },
                { "da", "41c7e214b438999b0800d63f4914bc95ea6f6318e01c27ff4ce7cd554eeff51c2a15edba81cbb76f2448192abd6bcc5e35842504be1dae3e499bcf2048881bbf" },
                { "de", "7d31d01d16b6584133a1092f0039e8de418ec2ecf4628ad02514f91d5baf563d6604f2b209ea25e0502c824042b8456c78d305db21d4ee8aa751dc0723ee70dc" },
                { "dsb", "6775bea9c5f47d3db84d53404980fc5666400f3ad5c27bc886d4d0c44ac9105183bee78da0e85ec57e51d38f53c5a9edbd40baf8863f850b5f676cb8505c495f" },
                { "el", "10cd00762efaa43236556d4a1ddfa21922ac2e530bfdf1c6dadd3d787b8d1891d3d7aac5f5a9b601eb287200be3371fe3384cdfcfaad75695b9a806da331a653" },
                { "en-CA", "fef869366adfb0c1120bb1c975d5d88b0ad149dc090464e75132c9e8d59066b9a31955c329e9f2167b2b98abaca56d410bd1d63f8ae27514d75656ecae1d83ec" },
                { "en-GB", "911e95d3b1fbfc32b2922a45ce17d6adb95d13b9c387ad75b77e8038ee31daa0aecd3b4010f9499d6e530e8d09c4e7b5e3f6c29dc30e78704c9644101d7fd41b" },
                { "en-US", "8e3a245368ffea5862a53515415b97e895ff56c2d5f74963650377c431951411e7ca9464cfffdc2873920211f79a0e3e08c17db47a165176747b446b63bb7d34" },
                { "es-AR", "a3ed70141b7b9cbc8a4ae954576581f8867b5dca630d85598f516e896c67eea9d8d49dde70053f241e6270a5f19205068d7ad641f21a626013e720a44a5a8000" },
                { "es-ES", "fae56947a5ddacaf034201bb27704a262927400c89f4c9898c3654d217391c514796bb25f611ed8ba73c646f0d7f19cfe082a9bed04cd8f5a7491776e6498a97" },
                { "et", "2015f23d46f9c4325434bb6ef4ec05bc363d61d7ee62a83ac84f6f31642171984c88af3c3559832ed6d49d2067600662bbef380f7182b9bb2bf4c19a2553846d" },
                { "eu", "8cfb15dd0afdee0f62519e27e73ac72b6dd78dc9d725d2618569e646bdad9001784f56434057bb5fded90ec83de4458e90acd88bfa0e7c6e8557b455a473aaef" },
                { "fa", "e8679de8b3dcab1f40af09b05e9d68663c27ca66068b34641020a4b258660a91f9e9550962349816d7a34d2e287cc240780e07c755f62966714f4260ca4ebf0e" },
                { "fi", "eb5286a32e57743888833f58cd771f9047d2aef68ec6a6c2ad6a1a736e042defd0e873cf45e1077a36df48e5b81d3ce9a9cf5f9995314a92d47fd3b73c3224b9" },
                { "fr", "05ac098e90a4bba33101e1f5a0a51b580fb7d0eb6fd524f2984e1faf958d2a6bcc3b780f58f05da5c45e425df02ffebf430735e41b01f72e4c9fe4929160ea4f" },
                { "fy-NL", "423c586457a1f6b9b14c4ac843b9bc818cd1584f5cb2b5ebd1046700d3cf572363dffbd42a1e6173f4bb9fbd2861aa268c8c8f7697307889c9cfc3c8c7fe86df" },
                { "ga-IE", "b9a343a02d1a4bd046fa8720873da4939e6e486b058e0d3db71fbf1f58bb7dce94192705470542dc3666d64da2a846ee59f5711a05c7b78240ea421a05829445" },
                { "gd", "adc04cac584280dae58050b5a4eb23c84ecf18c6f57c16f50336e83c00c86a2a7a75509dd55d43831d7cfe015f004d590011bd08bc0bba17c653203dde38fc24" },
                { "gl", "6fec92dfb57c8ec37c76467374b4bdd5812f18b04127ec3bddf90c6aea6aab307b0544aa0ac5777759ad5a5c5038aa8a96ae9b116fe88b05fe2e4a684ea9b1bd" },
                { "he", "989a4790eecb2dde713e6d94b5b51671cf3bd6052e9974d69c933e920c60fab73c7177f16eeb670ed21a27390dc4965b13977d8edcb2a2047b06e05e3c1c791c" },
                { "hr", "012c7cdc787f8c18155ca8597444bc8fe34e3c1685f8f4db47caa640037def0a723f5d2ca6144b420e7874fa9d8ae9f899fbb570981bb1cd908cc7485dd47962" },
                { "hsb", "14b856724a192ddeaf7eea81cb9ab0dc54f06461724e1d6a57bb3962bfac4e229daee52c77b6dc82a91870f856444c4621a3940fbdba11e4c18db7316369b780" },
                { "hu", "0885dbfaf1adc658a52a8427da67596246b193ffb51aa174a422a0a3abe172baf7e1b53c9cfec805cc55f08253ed78031ef7790627e8de6c247ecaebe2536c44" },
                { "hy-AM", "c59eb5bc1ce7017594b9ca109f42e03db12371b0f35711ae752422596b143d3a61d1ad061d1a94f5950790a880199a88551466fff298680290c12232e49b7512" },
                { "id", "c5a1baf7775b89b29ea5ce994039d98e515705a7e91dcc7c272ae71949f5dfb203d720d17347b3fa0ac0123a8974e9fdcdc7a4c6f0cb315c4ed3145f88d91bfe" },
                { "is", "3e69b8d15a26a49a2fce6c954e1957d9eb889fde31277bf1b1414f52782a119492afb1f825c34a7669ea211ee095cbf4a1d3a6179b45cb74165a8fa542b16f5f" },
                { "it", "7277b9cc608ebb36f8360daaadd5b4f109602315b664d6f8ae49fa82bf354df1645af6cde6defce30246fde482e14acf53984957bf8e9313d6cab7fcbdc7d768" },
                { "ja", "3d40f4d7d8c59048db04fc09cb1e3763df211d0002373cec4b9270eff04cc046607f1b9dea62c446a9206ceb4a51efa40816b859626e7e4479aacbe862cb1a34" },
                { "ka", "6f526ade964eef2caeb5e6431e9fd041083728fb5b19a30664ac770d1f9369aac43fbd2c88174f82677331f78e87455b4b98eec0526c79e3e3f31d299335b8d4" },
                { "kab", "7feffefee6d6c67ef09dbc02e8e244573a98a6a49da366d261527b1a15905d05087fd2415476e53d98df1e575e7238f9928c3ad7d8e9e37580b105a2cfb33d94" },
                { "kk", "1c7ee95f677f96840347e1adadf7fb2a06ec671930ea864f2bbfdde181a743a555697224e3dadfbef3d00b6762c92b21ef75f52be26b6b7259a4216170fbf095" },
                { "ko", "f35ef4257bbe0ff15d1e55c4d04c52d49569ae66ece895c863e1cc082662257fa1bc01d9c7668795653e0d4b5812d2f6aef92e2fe9c9ff57f4c8149cef7c49de" },
                { "lt", "1966c712ddea8ac6e44c0b2509c25d629c836aa64ca07142bce96c98f1ee76e7d8da744b632ed3d8e3a9b4d4afcc5b54886adfb1389ad5248880cd44f19c5303" },
                { "ms", "b3e35762c1fdb12facface324819d4cd5cbf6468c8e77b94a810150398694c6d4a5666203530bb9aa1ed79d1e24d3ec5c1e9199a612d91a90901f48a9237f761" },
                { "nb-NO", "ba484daae4767b39d9f8ec6ccbd325b2feb2f1b97b7a58b7ab8c37dacd1907960b5a8f93c641e7ec90679c1ee917ddd605460c990a6f1d040dfeb29e3dda5292" },
                { "nl", "029f33b46bb59c8da62a371716a04337c07f2ca33b0ebc8f810f993c17b86c21d7e14f5134f1005fe8b9f521d8ab9565f5ee39e810249916a66651e5a2057b7b" },
                { "nn-NO", "1b5fba157dd38ffaf6653c3466a48aec49df922dcb9e244c5c0234e2aa61a338102febacead874ff3d70157f401b34970693045237e1cc442f720e7c1e475f5c" },
                { "pa-IN", "a4682e0aa07d2c4614855e6f082e66bb12a7c9acb63ddb921dc359fe464d61b9bf66a9807058dcfc73d549ca62030fe15999e523c3394dd84b37b428af78446a" },
                { "pl", "687cb822fc108a2cc496c24a68c624067b56627b25ba3ba808ce1709ad12cb046929a3343787cdcb6b96e1ecbf04e8eb306792914b461a73df38519e91be0987" },
                { "pt-BR", "2387605b9c236bace8f9b917ef30cbcb6627f7fdd8e6e4dc61040c92db977023493cbd0faa6fade90c1f2e456f4efbee72fe9b3d03da7220afabebc9b0eb013a" },
                { "pt-PT", "4cda460f778604fd504c8649b8f9e359398f60f3251e3ffdb9576296e908b8f18426a8ecaf6826f307e0db1b12c43e507f69e711fc03a6f629636fc2f9c4b1e7" },
                { "rm", "8f4b82e15d16ff51f1e0a74e15cdb21a10b3c6e31df2e47f4cbb0e8e13fd0728f9b8589490dd04c97d32bc0d0880a59fd54a854150b47bbcfd0d658cf7a9648e" },
                { "ro", "02ec1bf438184c116b5c3f9d11287eef3123b5763425401aa5cef74e27aa98d0e2a586efd26a85c7be5b31913a4875888f25df658908972a903847ca55484f09" },
                { "ru", "588d0d4ce1849f15d49ccccd0de923d7d5d37a6bf44217c0a22ab8382ddcfdf05f1a83f018cd940f6218fbe3281e76c02c293ac26bcd203eb650bd1c625c228c" },
                { "si", "120be9a33ff2e2483c13789e0ac961ca1906e486a5c1a9f23edeeebb37f75a36370c2017f769a103d072acc8de790948404774cefe664de884ed298cae401959" },
                { "sk", "35c326b202ce4ae94085240c4c4b227a4c1a069c15d1863f092fd6f8dd5fd7accf7354b650c397bfa0309197834abd34e17667bbb2b9a20bfd38c3985c68ba16" },
                { "sl", "0d533b6d9877c0fbf2c0c45f4d4187659106e3bc4dd062e43d4b07a69ab3fa2d51ec976137373cc46a482d5c840806bc1fdfddd4d726d91edf2d6d87de7a6f4c" },
                { "sq", "e771137e77aa3796ddc6ed4e7c13464af0a2fc134e2257b68f8fa9c4335ea40f9876e4b999a0104e0fe6774695230a961805de495f5624e43157488b3db7bd5f" },
                { "sr", "9a46d8cb1cacc570f0de5f87e63a19e3335aa7b34bf1ecc9c0d2ba3e58c1bb3bb27421aadb874d748af1f395ac9049170c4ea0bd34b4742b45300bdb53837dc8" },
                { "sv-SE", "6168db42f055b907cd31c4c205a211c9d5f4c07b6821bd28140972b1a5e61845b4279a9d48369c053b7fd775f548290b87755181d3e5a30452832d0b647cdeb4" },
                { "th", "4eb62990d78062f1ef0fee2f71585b6f22d93abe2b621f57f88dfd30f52931d0ca8804e8061eaeb2520a38f1e30021dc10640ed7b74131baeab08fbc3b1b1c06" },
                { "tr", "f6f3a0f86c21675a4f37cf178912e9b32ec55da53aab7dccef687fb8749ce267d0173113d3f92218acc5872243e291248310f3b17fe1136295b68a572a9ba12c" },
                { "uk", "629831a832a68a7a486575d6627ddb88b12412653648c2eba377bb6b90aef83c3324a14017c814f5a568b4ad364958958ca76bbd275786612932821ecdec70bd" },
                { "uz", "858700d91aca77d0986bdf11ddcee9f925079b0fdffb939541d92d32daecfb149ad20027f098441da4addae9fb0a04a842b63aac8235aa5a4b9d47fdea69607d" },
                { "vi", "e33b0c9787777541a73da8becea96783d16ce83daa7e460b16eb6b18c0925a654b3cdb88d31d4089f11316475855b84404269834f5e41077db4c2791653e2d7b" },
                { "zh-CN", "d383229c8283286c33c8bd7bfb943ce69a493611dfb679447d3cc12243d3fca1e834603fbb7be7c7d7b5bcf1f35991077a1d9751ef169a5853a99ab599c3fc9a" },
                { "zh-TW", "03fee81b4bffff663391d71d55b3a774b519ab2918f169a496cac27a4e759bbf83e8815e834645cbcda932e09c48839401c4ebde9d7c585b8b0da61ce5e71a13" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/78.12.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "f72ba4377608f24320db9d2eb85ec147c0626aea1e8e895dfa28e19bd4d4bf435622182babe061553bf52a73952a6b24aa986823c9540354b7003bfa011bb033" },
                { "ar", "3d99830243f6d74086b93f2d227e57ff22ca2ac3f24ed72fc635364f7732789747cb7cd098a8f55a47d63a4fda91ac18fc96444a27fe8d0fff1cd98f07c73ebd" },
                { "ast", "e7ed9c60f7e0de986ff35750f7351bb4a2d6dee2c1c496760cfc2567ff42383f1bcb18cf5b841700dfae92cc75258435927271f2c31d523867239d835d177d29" },
                { "be", "8b5060ef3af62302bc9789f64ad2db54ee9a4ae00e4aac4d7c5a47a35e1e643bdaaa03eee4119f3346b5fa93cfa0e1b70541155f9db50866d9f81d62a67313d3" },
                { "bg", "b42f8582bef71e393b73f52c5ee895abb47109ce4ff35e6f17f865cfc0e6d21fa186607ef5919e5be2db7f4e3a063afd046b66d620420a82372ca157ebad006e" },
                { "br", "917deff18608d9b6638f816691251d07a7016d7a53b9e7baca1d755fa003a111d2338ceb751adb15baf17f18b8448fce8b7b02ae0a04dde893b45c4c9288dbc2" },
                { "ca", "ae3e93bd0e385fe3ccf8de623b2f6169ade2fcbf625273f9eb1b5e4ff04cf32a2d59eb3166072b9b304a1bd8deb5e2a326791bf2a6acca38250608704f48186a" },
                { "cak", "824b61736b9feaf4971f80b4a00980223a553dd6da11aece166a307cc5ca9ebc8e69b10567453139e98aabe65b77e40ff0357156f6f2a1e4557b636a38927470" },
                { "cs", "7a4dfa3a5c1cbd9bce5fcfe9495231ff3ce3d61874d94b18ad9e95d137c1bb333a5ba709aa6dc3a096a15fe1b5c5973babe4633a607725929c7e348f738e7a29" },
                { "cy", "34b391fdd0b5eb34ae7d712b81d3cabf4ce563722bc9a17744195e7de6da5cbef18849fd26420d39047b2b319351afbe8542bf581096c5bf20bec0d43ffe11ea" },
                { "da", "818906255128faeefeca047a87c45b116c72d83b230ae6506ae20152a7a2f0d83c381a8b2e59316448a7e874e31cf2236c2cba6bcfae96b5918c68ed983af2c2" },
                { "de", "9b2bd1491e95abf940ca1dd1555bfacec19ab1e4e2c365174cdae42cc90279037097a96c160f4b88f60f4466ad6984b5bd2075be76cb1c45dccd25f63686c95e" },
                { "dsb", "402fa4464ec6394d2b0e1631fe10adb07e4f6fd8869b5cfa0207dda46e659d0aa44295bc4cd930c457e1418f3d0c2bb7d8273b56a358615600b69453bffd8f89" },
                { "el", "7bb35575937274f34b04cd3ca057f755b8559b3ecd5776e7b1a1fd4559f011c88376db4efbc8c547d4456516d259c40414fe5c608c52090a56cb0216aebfd3d3" },
                { "en-CA", "f0e3131f6ba7dc9914ee60295b794cc3ed66e8c597ca013cfcc797d85a1ced5a5a7b65472bdb7be614c7e8506c2c4556fd990fd56d6bc47d13b6689b0c3784a9" },
                { "en-GB", "b1cef386246f140de5cdeafdb5bda2f695dee5d8899cfee7aa412d3216fe4aecafe648d656c820015fd4084add78387c26fe716a3e72f0b905785ff39090d685" },
                { "en-US", "df36bf0500e399dcfad7fc91353071fa376b2557eb7074252544f294f717b0b9c447ec49680b3ea193786e6be683dd1419dc00b584c994a3d324363d94d616e0" },
                { "es-AR", "419878358fc8e4f676306dd52e4817d91286deb0c0e16d2965f8d9b50d6478186c5b355d4a12785a66a21a5318ac2a931cd870ca9143a91c994b27a7e37d6352" },
                { "es-ES", "965882df348dfb4653b1c87ada483e5c136c34d5022af3f73200282f2a877bef96fe25e160da1435a833b49742ce16b2e23849d7e0bfe151331589e09f952f26" },
                { "et", "5c86eb888a24e2f9e06676e4d6797836ad172944b8276735fed54348cbe5e76375c3b6c71e240e92cd44a2a3fc9e60bfccce8247f254d9c71d9feefc9ceba8db" },
                { "eu", "3b8d3100bd72e124844b926f6ee2494e98a2524d730cc231986b4af2685f2c243f6a4dbdede85f7dfe7d0ebc7e77148d160d2b9cbfbd843efe677096cc1d8101" },
                { "fa", "80cb5a6f027eaf323d1eabfe2a45ad9663eafbe2f2a2901742016d80c850f0091a16ccb4ef28235f38f7e6594802598d21140dc5fcd1070a3bb63be7af7f34f5" },
                { "fi", "3b3def88cbdaf76632c4a643af7520fb73a0d1f9fe5e5a59ca144d9ea95be7f4afbb204c39893bf90c35de29041df648da042e0b2969e7bf91a8d4e13392f244" },
                { "fr", "f690d987c9e0b2e387fdecd381d5b6e2145655aadfd636885885a1833222182be3e300601fbb8be770f5521d642342a2fa59ffc43bbf955787b3cda8a5b7df9c" },
                { "fy-NL", "824c8feb4fcd3ae2dc471921441ace0bec861f7138d76eb29c90fb72e069e0f346fa44ce174e04f4ca9a3ef29389ccecb3a7256b49e293f644151f25cb32a109" },
                { "ga-IE", "ab4452705578159b7e0282cb3b91e6b075b4eec64e236b5383360a4148a0cc99b9df23cf53319dfab2b6d4e0a1eafffd5b8ad12bf1e9b3e061590ef872d9a537" },
                { "gd", "8a42abfbc13ecd328aff11f346f4d61cea09a1ba419b66de055f191f7929a1197d85f97aae4a249d1e62a4b4799206e71526147badb77bff9b8f0fccdfece9a4" },
                { "gl", "733d02cd6191abc8208cdfd5d3a99da2591a46a89c6c5c024789ad7dbce28a9bd84a9c4ac1ebeb962bf174abea522b19f618f5ebf004ef2af60ae5917239dafa" },
                { "he", "339a333610cbecef4729901c6a887dcd59cc436879efdc9306cad9af7bbaca33ba27262d04920b13acb2191d700fde5780c91dc2d8acf857f0e8f38f7bcd7812" },
                { "hr", "34f3ed3868fd883e1fcf261a0669a415b6aef4003694e93278e746ec8f89430eeff425487f411e4745b4ae5ed83e1f90833fc5cd11742756430eb5a83aa29bd9" },
                { "hsb", "510bf805048555f49a83de8c72ee224da38c2f8f15b47a87bef0f1447b457679ac560024228537683a594aa1422bbc9ea3d4211fc9864b2cb32c4739e9a45e3c" },
                { "hu", "713376555edf9ce05684aa1ef410e89d4321722ec8ab3010214c01cacbe4a2b322405483287b342366eb448b081a272c86fd3eea45b87f80556a1f308ee7c649" },
                { "hy-AM", "2b9c9175c0096131f88aaa29a78fe5695713fb9b4b3b8c78d092aa230a74692db4544738127404866182b800f3c4f20336696b8398269d39aab6240c6e9c1e9f" },
                { "id", "3b7cdddb0d3286e94aac61d5cf175458740dcc8114cecdd92156bf4ea7e282e53d43dbeef7d372ee8e4f8da327cfadabb677db8062a2ae754a8584f9701ade23" },
                { "is", "428dffec53f527460a8d828500c94f11415dff25e6ab27c568adef6676c1227d09324ddf132c8be6c98ef840595ee97946022073cba135ebf9cf0d07a91b4077" },
                { "it", "1cba40e25f9f6494cd6a06a0c5ce68d776944b6a173e57f230b23015aef3378aa180885507592839066c9a7d7e023276b5f3311c52e9feb2d9d25f3693c412ad" },
                { "ja", "c5cf3b45da4dc1be1bcd0ee2a5f0881aae957ba023b710cfa1995f28a7411556be9d74076c6cf90b9cae90e2fd489e8e7d14c4083ac1a6325765fb074cbc632e" },
                { "ka", "095578b480d60dc81daf8573d3ed2de07a2c59b2247b5f0c02d6042590a8e4462dc95fd0a421abc5a3a5895ca3b774c01d4476dcdd77bc88d7179b1b7f725ed9" },
                { "kab", "ca01140ef08082f5b756ba7d9fde0ed9f0b0c27b172952ba31696c93fd2294e75d43e020ba48b68e57783d6de044c35239c4d14be6912932984ed3c7d41768ed" },
                { "kk", "4ff1f6e0f76acd25f89774845666a2afe9f1bac9b3ec6cf363051a0313694aeae6ace2a11d2c321922944de06e7020b79d50b1363a57728ecf04cf3b1425c903" },
                { "ko", "ecaace8a469f4b595328ac271cc5c5e268456e4255bc834120e3522c2498ae29d6181531707496e3d45402a4d2a6194877859003e8120edf4853fc43b5686ad9" },
                { "lt", "45dc60fd7baf28eaca55039c5b13565961da69451f5db069845c648f521d559d5599bccd4a9750b20f849c342ed520654cd5d2e9c8db593cbeff4e53a70bf535" },
                { "ms", "f44d95a83abb361beb822b7d6c4caf6c15b16a6664c6e66950a6960ef5381497b96954ff2616a75bcc704abd7d20c03a0f6fe58415d9bbe1814d97115629420d" },
                { "nb-NO", "1673be8f6510a891cd8839d32373e2979bec90dee17f94a4ef60921d7c0eb0183af5da776344841bdb30954109ef48b1f70513131de19015b39a9a8cccca97bb" },
                { "nl", "1f7e8653b2b036f5cfe3f7cf6484337a9a802c703db376767ac5542fdf4008a01fd706f11c441fb281415d80cc74e4ebc9ca341714d3a1014af65c53814837db" },
                { "nn-NO", "c4a2e5e196f88653cfeec3223e5e502d63ecc531e8b35a0e646abc828e9f6f25928fd142db4d7f09b994a81341d91750215af7989d12b582734be6f5b22ea179" },
                { "pa-IN", "3434596f3bc649f566ba822850d5c69cbeb8f12c5558fd9672c8d00c877acdff45fa01f1a019e2c0203baa1122b5f6675a0a270511a65fa19f9cf3f0db765ffa" },
                { "pl", "e98c6aea650738d2a0bed34b72bf1df8ad300d3b677c9c4dc03e71c10cd33bf00bb97e5db68d3b57a3ad41ef4d526ab66996aa67546dda16bdc1f4e9931efd51" },
                { "pt-BR", "a2f8ca8f946ad4e08b78883cc7275f55c13aee0e2d1a5ea8630597a1a59742f317ef7b0cb1799c688ab3b6f9c3c663661ce2d5f6e33e9e700cf54cb6cb4fb532" },
                { "pt-PT", "a59731bfe67d39acf7c2116af0c24bd11875542dc5cdafcc3fe67b6c1eaa43235349d19c328ea0dbcba425337cf49c2357e3a2897e19485c7a84140e3fa7413f" },
                { "rm", "7697ec7ce299229a29ef6676809bf891e6939f5d9b575395c590b6f1d2d5a26da290efb4553a47cf77d9457f5682639d315b439458c9ffd46517011cbb8d996c" },
                { "ro", "7a8e4e10eb82cc9c31179d6322fa07324cde54a8333cf3c941bf8dd41c9fcb41c638c288b233608f92b9abd040d625ec482a17cd522ab45ff587aca4803d6c85" },
                { "ru", "d925d49b8326789d828b408dd16ea0e3e9a8fe08de558723880a7fd93acc4b0e574a2a212c89da9389c1b8c62df5580fa5a9ff7b10318766a5ce2c2a95cbc620" },
                { "si", "24fbd5840038a79d4fd4735dbbc64203e01bb5d37b3edba135da31741605c87b8ead466989ddecdc1560b6a8a40a27f334ffac74c5d770ff6a4981abe6d863ac" },
                { "sk", "81e6395a8008cd493b8b97bea32680a67b4fe3781cf5f84f08679c5bd33078323df0fff7cdefb31bc46134877c2e44300470b7762c5efca646d4fd7d1b850da7" },
                { "sl", "410a477f63121fa76e4843c4c5fe9e055628c718d7ef583371734e729bfe4220b9a99a510bc07c5ecf09dd2085d8a04d879c4218342a0743d1350a57a2981d28" },
                { "sq", "427d4c7f63155ee2516a29a6155138fdd36e90c790f40fc6455da16d2bea734dc87e4dc180b7bd8b50a384ee40c281365fd875f8307e13f7f9f197913b14820c" },
                { "sr", "13c0df2e940854ee5777101d78d8ece7e5965ec5f38bbf24b629225dfbfafaf8d432f145621e94cff465fdf3bcce3254d244657c08bb7927ee28e3b0d20c5185" },
                { "sv-SE", "0ce88197e777cc83542e1dc792e8130fc7d5888fe9666198393a643aa0fafbb2dbd2e194c39e01ddd2e5d6626cce4ad83b73fa0459bd8e8319e20984cd6043d8" },
                { "th", "434aa2eeb70e02aa61959eb98050a5e1b5c339399907bb915049d78fa08b235305af00ba46c29693302bcbbd867e56c476b981161d01a0c93b63216c8ab6247f" },
                { "tr", "727e66c8e663520dbd559b173c4948e12d35e5b159139d88fc92a6d0511a8db51414838e9464587af33ae86e76500fa33b530d15c1c600f50c81908462d4e800" },
                { "uk", "4493d583837314a8d71fd7c4f5543a2ecea7c12f6b1e5560623ea7c1fca53cb61333681d7d375733e89a442fd0fe2258c0e49185e586ff1024e3a228af1d7f85" },
                { "uz", "85d79dd0e3995f272949f4f69de5f3a35770edf1b1beb252be216e0e9eb1594fe1c0e736614a05bf508c9362202199326634adf4727cd55d9b8734a3303e50a9" },
                { "vi", "2cf0af9346f994d5a69c6f584af2527b77af097b523620c80a70e9d482e2f7e3b72bb9e2eb7921376c47e4fbd74bf1ac3bcb8c19847d71107184138bc6760467" },
                { "zh-CN", "9a416e87589eb1f6db949af2b3a51dbe53eb72109574ddcf26ac0652b5db705cecee28632517a385940b487b29538eaa5fc94fa029d65bfbd165cbf3001479d4" },
                { "zh-TW", "9e9b022172df1f6af711f9d74555db039291e4a196f62ae3d1ee6a9f9caf89ee9ce274985825d06aebcd65ee7cab815385f2cfa53bc9bb4f68ad1f8b3dac75cb" }
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
            const string version = "78.12.0";
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
