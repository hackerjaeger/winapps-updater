﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

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
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "113.0b3";

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
            // https://ftp.mozilla.org/pub/devedition/releases/113.0b3/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "b13eee1e2a7aa8416128ad566b57e6d5fa32260c0d9d0cd4c71edceed5bca09b69d71cce621cb12e115b5dccae47cd6d721dca979548f12eb5566790c084c2b8" },
                { "af", "0e1cd0736fc3b3377dab0ec0722f37f21f944c96f9fdea7cc6d843db210030edb2490bc7419dc96490239d916df966ae3c07118bad916bf88167ed55734068ad" },
                { "an", "e662f1c3e7a5d6600435df5355417e27e7a29295d9b13f3ec8ed3c6ccfdf5ee4cf3666520f977d424521a4f2ce264b0d65367f70310fbc5180e2c886b01e728c" },
                { "ar", "cfee30b12d33c2500e1844898431c45c406d62b5b7ffbd4f5c6a73bb1044d72e4cbf70348c6f6958a7991e722ab996c00063b681d0b55344a1335a37ef9e64a8" },
                { "ast", "a8ea6403a2fc6ccb72419acf337c507e7a65786d06fd7e04b0e1d22e8d19c377038672d697d71064c49449a6bc9b9564ffec6320b2c2bbd0057eee868f9f0f81" },
                { "az", "243a3ec3f437a911039ca05639c6a8a04f44574a9e63612399e3bbbf85f3c51d55f43e332a3e8fde837b6ef9398180ce59d340355ef18aebff48b5d9ab686caf" },
                { "be", "ae4ce00e1c55e66b90fc75bf9596926715b8c628c223316eb616ac8d478e1691556d84eee22fe2b76a52ad33803b1e0adbd53db7acb33f42f961b4881f68cd20" },
                { "bg", "e982f864a26ea85c61c0d961475ca24ac4cc68b2aacfd25d93a47c66269834da36907d3a2d00b5d3c61c1b1190d3531da02eb9d1e3bc06818a957fbad609615c" },
                { "bn", "a25b2e81ef2a82383c19c3d5e1494fcee46d2638cd354a282cb11c69e2cb7f4ad4460259cf8784adc3dccba28ed20f950bc7251e54ac8ec2f10f79ad88b1edab" },
                { "br", "b5591b4a5af030d36a2ff27169a725f30e2f2cafa89ea655b5e9661889d27a517a5c3353d9ec4bbb9ed0e218aa1d5dba8bcd15b6bc049efcc608d6c74f396249" },
                { "bs", "730a3b03286479e56c0d6b25cff5436bc45839cc3ae9e4a25b147e2e8910dfe2637b0adc928b28959e58deec287c9828c0e52721cdcf887139c7f11d9abc7b5b" },
                { "ca", "9e0ac9e521f137d37dc34dee5c22c15c864747df35022551cea2765f3ce77e2474ecdca2c934bdebdebe7031a551e013ed3f25a9fdd530f4d176a79c4bfdeb1a" },
                { "cak", "f6f6ae337239cfb3785d2e756435f4d7e4c4e82b03b6853e3ce29a38ccfa9b2a4656896098c26574bc34d1372f5720d902417c356ffb4299daa7b59820cd5710" },
                { "cs", "e13d0146be169084b51c35b907a0be586e4a29796c1822b63e63fdbe2d13c209943c12f23e7065878dd40c97d7475dcbf25f0d758ca0c9bff11f5d1ebd3ce6ea" },
                { "cy", "5e567c4f5260f39f02203acd35319496fa8a75391fb1658d774ce834e4636f98a6114aff33829b171cc29495c6693bcaaa2e0205ecdd4c2cdecd7193cb1a253f" },
                { "da", "e51de4079435a61d00a732ef5e4d06bc3924eb5bcf09b404c3df8278364ad960b96bc60731cf49695ded38f3f3fe52acc801555ed25e81348643d88bd1b17844" },
                { "de", "13a05f93535af36b7aee6fdae1fd0624eeddfca3203fcba2b83c7276d3c7dc6155da8484047ec611e81e043930d1f661808055c3bec33c305846b54fd2d317e9" },
                { "dsb", "e9b085b027239fd3ae61c43e1e8eb7e3091366dec07855286b1a6e883e26fb8cad891c66188e02d94b0eb0fc41cb39057b88660e50a5ff0cb1d2f370a415fbb5" },
                { "el", "b9a0343756ac4c9a8ac1b790266c74588403c220a50e64418bffa24eb383485eaced439ebd13ed9ffa82a6324f94d50c586cba2b2efc5106d610d7a560a8cbd6" },
                { "en-CA", "afca24548a29327427e664adff23ec04deaf689157cf91580996b28527659ca6b0a65a2169893e523f0735634c70332ea34c8ef2ebac125184d56004b30ffb5d" },
                { "en-GB", "3ddaafcd4db64c648ed955b3460a264bdd5763a0e4f96164cb37308cfa1d82b46af91a424b68b6f2a48c93690ad549859915fdf2d43c917de839d87bcfd6fd59" },
                { "en-US", "e72e7a1516e9fc011327467a0ff62889836a2db3ddeafe47bb10b906ac79929c98d8298a5ca0f79dc67fe6c9617062407c10492f01b592b1d281cc12e332e397" },
                { "eo", "6f89c8b33d646b009150fc9420ca5eb942d829ab86addf42c147bc764e073f9bb8342f45d52ef2f7e6dcf4520153a1bcbe4106068281b41597d7dd245a58b2ea" },
                { "es-AR", "41292d21d68ce7a5bfd12802cbd73efb55364e5a932d4c90084843f0aa2007488d8450ff710d18562b2f0c1d69898cb7a91aa86b604d374fbc8b3eacf9081193" },
                { "es-CL", "73f5cc56c2abd910fd5070e6b66b657df2c9e1e7e2c19ff838e14bd838479b353ad776dd4776428a6b55c46b47eb784f49863d8b148021ba108f19e31c048a79" },
                { "es-ES", "4dae2750ecc365f712b1d2939b339520fd02d54674c6f301e7d0b7f434018164b40c7c881cd5caa066d92506039da8cbf5835f582a8ad74a3c15bde1e8aec79d" },
                { "es-MX", "6b81b770758838a73aa37e9ebefa88d40165102e0897fe1505dc725b6919ab8719e8604f60086403d2bef96d55baa45980b8c65801ad9613473391f1f295f820" },
                { "et", "a3879d3b8dcaeceb0bec8c294de8b1190e7032e2945feb937c217195d0f7f4264c044099a9bec5d7faaae0a8a17aa4e91bc30316281f0f755a317464c39c078c" },
                { "eu", "d9849813584bf7e31d1bc93f95ffec257413d04fc6ef7e4edc42922ddf0de738d3bfb2b4318067e5c422b2a06499a792e925cdbaed61b1fc154a22b6ceb1f10a" },
                { "fa", "dad364da4eaa7f41388460e0333280a894648dd2672845be23dda194b46fdd5d834af8bdd19e0d0ec3331452ba68163c718552dd38c55a71d0bc7d8fe247fa4b" },
                { "ff", "602f29ec8943e42b9b2ed2483a2958bab412a18f443d234b4c3a8325df91e3bef895407e3faf3fa7f7f157c2a54541c2d57e226d0cdd4fb60d784a933740fcab" },
                { "fi", "bf919020def38a14e7ca1146fc45ec29f0f2cc0ecb19f271037f3043d84552b714bad60d118f39cbff212f567057a84de654183758b6d669719a255c42af88ef" },
                { "fr", "af562de276a025d35d849acd16822d237f54fc51a03d5c8602f6a1aeea5cf34ce3c5be7345c5af161324ccf1175dcead15702ce9c0469899ee3c9c3ee76bab75" },
                { "fur", "b66101ba6b6c3e6150e47a87e3aea13e7650d0b3c0bc49bb73b18889e1e8975580fbf5a737344ef3ddb3471ebcf4414387138f605fee0bf2a6cd0d17dcb92990" },
                { "fy-NL", "9d784f8667e8a9aa4b322ba1ba14d19272f72cbdd3a0921d3d16e4518f290988fbdc38af8e38d00db5b5d0bb4f0d367ca29357ca289658d1a4f1bfb667546ea1" },
                { "ga-IE", "0e74c27cce38ff84efbee90ba390a9fd937c780ca40462dbc606079200c2e215fe19fdba1baef257348f73b50e3877b20967196943987b7d04a4cd556a458c02" },
                { "gd", "de4e6898328bc165c55685ae1821efc71d42a83c548dce72eae8b133c28170837fe43e09d359c3f3f391ea950bad23d868c44bd2e341ca978bbd793f08da896c" },
                { "gl", "079940de48388c03c4a52de950eb321cbc348c812c6609efea065aee78c62780d0e2af0165f542703fa3d2efc48bb9e416fce8b3e868f6f769aeb3f207d46a3c" },
                { "gn", "cf4c490b1a65bfcf8100a3a05a8994b8490d35e7f6d8c4c87712a52b52f56fde9ad37a22f77c8cd00485bea1e97f98999f692686ad61cf5e3b1791291a3946ae" },
                { "gu-IN", "88dc77c7fc8159134b7be7a7c0ef9196848c06cb0df05912554f77b8c3f2fef54a92cb3062ccb956579c7d9028b3ed02c1f76cef52a8a135302ec5a96c662492" },
                { "he", "a370e137ee1284f36b93b32c9bd102ba84a0fd8d86bf75e2faa921c13d45fe5e35856d23d4dba0d6e2d962049a43d19016a6adc6c8eb7d67b544d4763a65f2f1" },
                { "hi-IN", "3781faaed38e6d04d33d5e12fdc765a682078b80f7e1790903f8e4bda23b31945314c6cd39eb10bf21e571b27501581ce7fa594d1bc2f10dfa3e0e5b399436c0" },
                { "hr", "62676c9bf9eace73fc7e63089eb87a8e21d89631bc97ab731cb3cf2d2c7b49fa36e4bbea3c1300f48282505333def829f1d812c16522f8476a402cd686f841ba" },
                { "hsb", "afeefa8c1b2b9aa73aea0ea9ca7de6157e1c18db9f3e6ddf21c77ca57175863c19799bb7bc0058632d244298bb70c591e7be1463bd257e41f2c82ba3ae98dedd" },
                { "hu", "3fde171efe1a195cfa5193445ba8c78d41f27f7dd0893ae509cc2e57d85544d67fa9bc21b0236570f6dcb35ba60857d10666ac22ec28f2f65fbd5e71f1101a91" },
                { "hy-AM", "dfe0f99f028bc1277c5d5cb3e48304792468167c4205ab0a0a3376373fcddf8cdad2810241951f7e89c52d430bd223083316eda6786144dfbd1d677ce9b470f6" },
                { "ia", "bc460071046b303689359da229163d6a8a986975328330d95af13e5b3574b2f35e39e8154204f9684d2160d0c26276bd4876b78c6862b189a19d970d19f7459a" },
                { "id", "1fc2ca1f08eabe77c5166f5f018f43a4b5d1ba4fe6c7238bbc885e1ab858cc2b0bd7581df937a94d6d81fdec4265d41f3a8be5c5833020a951a912570b399e72" },
                { "is", "cb5416d10361fed4cec08f0cd93024687fd8f5c9d80b6dfd3f08866ee834457955a49027633175ea1e48bf54965ee71f26faf11e31e4e02684df131d31347534" },
                { "it", "d52946e2fc4414f7448dd21461094b05d814e4547d696e3d1f2cf60d78b144df0d12440bb687d2b6fcfa52d7c8a510b49ed888c65b8f4f198ae22cdf83bea13d" },
                { "ja", "f1a771c17a4d6b7936b60ba76e29f46d9ccd60517613f3d23df421203934b93d304fb50cf54a43496145095e89bf471762c68ae59789c8057876b3a400d7235c" },
                { "ka", "f46d2eff8339bbda08b14bba846a0da5ed46f9c25c3d5547e157ed38ab509f751d9d85c279d08fe9b2bb97833f08536761a4b384fa887a2d735bef6fbbe244a1" },
                { "kab", "20409181ae4557f2312f2b97fd0b4f98959ca433ccf43ad4558e8e18d8aa507f9e02c0a477939254383c439e9f5cbad865e219277859e4e01cf4bc62bcbfbeba" },
                { "kk", "69dc2976207e045ee6fac49482674a0ce8e47d54479ab27cecf8f0f423fea7607e2d35c0bb7b061bd83b30617815b990585896a499504c22322f66dbebde2c45" },
                { "km", "fcb1e8799cb048298a9fd024c4380d73e68d073b65b91d5a5df9ee68fe00bcda203ae9def9d9cce18922371eb074ef1d984f594a48d0a9f2f6ea69586650a009" },
                { "kn", "56df997faa9111d632f52052e957eba21ed2d6e1d7aa0318c6161831a2e3af200d05e778a8e167515509edcf04016f66744633a0976681d673b4f858d11c0526" },
                { "ko", "872f3274a23c22e23b9f84cf1962138f935944aae6ccedbdff6f8b5cd31b2f0fb54b18ad5668ce64901d1662f23fde2b7e36bbfd50f8a8332ce7c2339ced59a0" },
                { "lij", "292a49dcf18ee429ce702f329f5c27aaaaf9fc87d1d2e13d7033d6884aaf1d285164f5deaab814a3cdab2325d5e145b484e6afd3fd0892640ee76d5c9d763f82" },
                { "lt", "1f433a44286bd2bc1bedc32b247f3c7aeb6feb40675a1c07f0bad8640356897fc09cc3aa21b3d1950b4ff46851aaf67d2eda5dd6addd6e0593dd65192c03450f" },
                { "lv", "e3035ea08b3440b0480e3bdf9c2762837b68b645f7137eb1f319f177c402676f5455e584ffeed572a5ab92532df9d9d6aa863986cae336586de0c7a0bd4cc92a" },
                { "mk", "570962b3218165aab5b903832a2ec23c97b7714213dcdc82671f01d09b87946dbc1477dafdc67366a15b879e5a0ba1ea6c83b99928ba36265f332766bcae754b" },
                { "mr", "1056752886f548b9c8083271f13db10461f291c4cc8e51694fcb85dbe1b4a5f29881abe507bef82535cfe7e3a86b22fd89808e7834e2b98996325a49cf34a91e" },
                { "ms", "c6f04040b02b38d419e461f7d94ebea3a15ce23dcec51c63f2f161f8feb6de3b03b79f289158803c8e871fef6cc62777675aa6368233b9b21c5280b9746b08ea" },
                { "my", "7026cb8d1407d27a6712330d3d9f90d9cb7802e64071c6c5f81094c1f7b8dd56d3799be9edf3c3cd8a49a5d3f24b9548279a334b5000c2733058d19191a298a5" },
                { "nb-NO", "c3a374f7e07296ace96521334053d515622ac13f77560c4d00080e978a9473fef0c42efea1322ad6d61000e53c6bbb883a806bddd8281ff6008525f8e1212849" },
                { "ne-NP", "7e0ed33a3621082f5b47b6c115f54d38cfeb9e8106eb9c05a8146427f32dd594ef61501aee757cab427be99030c58c42d2ec55ca8c1c825830fc73add61288e3" },
                { "nl", "0b283b00047a2d32ba54ab07bd589dd72472609a19450981c4f836ae756e4e170eacd7d4778688851aa74fc96605003940377f71feea45524489e94720559831" },
                { "nn-NO", "ea96e919a617fb5cc0cb778772a6846b4b18a126528aaa934991b10b8a39ef848d6591374931fc1bb919f8f241949a302d1eace8d75b4ebcc5e48c52f0ce25d5" },
                { "oc", "4e70c1fbc440496dbca44e4dec57787ee5fb87782e8a368dddd61bb603b4f6a5cdd4089d8d2f555f49fc3ebc005a4bbd838df7031ad56dd31006f343e06286da" },
                { "pa-IN", "0614df4997d58f7e187bc3823ddcb4ef4d061747012f0af0a0250a4f78edc9518cf7794ba058b0f83e6d2a3b942d105f94848171a0a8be25278938e31e2f6588" },
                { "pl", "6787c4ae3922e60033b35bb3d3aee2458d2272eb1e533fbac4b35bbcdac9a3607a6e29bdc9d1a6f0fae6efc887564a97b669193513cb3321f7ffc6a005d7c618" },
                { "pt-BR", "a13952f5cd383601a9fced48e97656b28b354c1c126fac0b43f98a3fb4e7222edf99cc8adcc75d40e1b273b25849f001934648b77af6398c1a6d74d029870512" },
                { "pt-PT", "90fbab8693f348948164021bb36bb7a2189989f3265961acc199f4cee754f101867b5d3b7b78812598c9949452ba537fff78b7f67fa14051439ffb6670be8866" },
                { "rm", "b9040cd47e5b4ffdfaff80c7ee1584080401e41d16c3199e61ea7058e618f8bc9f25d69580b90110104bb354fcd04baaf36bd357f9b1978e95cf7e4ce62cbfc6" },
                { "ro", "640f262df783691713f9da2095888f52896dd004ead3ad32b07b8371080d3b4d915d50bb5b3cb3f416009bc64160e5a21666ce3a6d75a01035600a61db9a1b1f" },
                { "ru", "d4c6cf553d3454c931169c81944db93ed7e8e51203f35d2eab20c77dd178dda0abe8a92456f35be46b441a19630b17a288738891d6e3dc08dcb112d2f79dd065" },
                { "sc", "582b9a7a76dfa0827d571b3cd6da2238f38459a63b76eba8674e77f80c848cccdc6a2577cf47e67073f43f849dc1bf7e5aa8f8b6e427e3e9ee4a904d923b0025" },
                { "sco", "fe7b609f40b12220d6df7ecdffea2cc02eb85b72b98752b53eaff585965715a810264eab48022b76f5fe2df4e80ed71865e1d848f83f64b0b789bd67623797ad" },
                { "si", "72b79f1e4ecc5c718cfb5155991d301d416bb33a1cc79d2426331c9fb7ea077f97a569b9b61354ae32334a9b0789c10bf0383388ec696d8067032cad02df10cb" },
                { "sk", "ceb5f1aa3c526f668ee6eb24c4b76ae27cdc53ab777b39d7f4921fa3f2a8cfef36cf98f8fcc0f412137126d2a004be9ff6aa08dd5c459742fc7639af8175aec3" },
                { "sl", "41c206e13fb6e3ab9b3bc6fe434dc8c09154e3cd11e0075647cc84b6d4b2b5af9ea9e152e95747f7d80fee4993eb689150a1da0a1fabaaa3b2908bfe5a988c69" },
                { "son", "cba72e8a6dc8c46cd13136948c1a131591621b3048252d25a62ff356d1e2149a19b9948e9ebb8e640adde232b7b2df97d2bc0ad7eef98d2cf2133d0f3b69268e" },
                { "sq", "2577ac4e17f1387a0fcc56fd3cec6af2e628e015acb3236183098155dea63dd0b832cd38cb9a7a01b8061a8ecb2a10b4907c2c7eceaff044396e579041b6c0c6" },
                { "sr", "d89cd3ddba1eb42aa59251ef018d0b2ef9df6d55a0674fcfb3991cb90e100e2947dd60792f15b628219e47ec0f2399dae5f22e701555755e60595a8accde2773" },
                { "sv-SE", "e3665d5c7439ef3e7118e4ef0d8953e0a0757a5f912486926e3f9a6e81eff81ad2be339cdced604acdd9ba7c805f87b3d3c3c7a25cc4f0fc364ee9bc84b7a6ef" },
                { "szl", "a8c5859211e4ed6e3fb72d227109b2840eccae5401acf00b4674c56d51a633fd51c8efc990b421c02902354e142cb43a2e3312ada10e13daaa163509684fec59" },
                { "ta", "248ff0f91d5619b7b2cbdd2b01eb14fc56064bb64ec15aad51b856b70ccf0d8ef436f5a976199b1eeb7cabb8e605d721d801a494638212e018a3562a81dcfa74" },
                { "te", "aa9304ebc04ccbeb584230b5ff0064baab7bcffb5d8e5137c8828c9539d5141f6db86f340cd0bc9f15e7a28781397f39baa89fa3a33c36b313851842d8ac3ec2" },
                { "tg", "61d5e53e44a9ca458ba05bcdb4d1954594b917a2be4d718258b4caddb8a3ba6d15322e1ac3b039d564ed13f9f9697c24a1f43a577f770974c1a4801d6dbb8867" },
                { "th", "7899ceca90a7653fc4d77fd2e6dce9396ff9de0e01e24b70a2381a9e03b9b63f8b1c55f0f5a3f86429cefdcc63f8e4b1376a9f4bc7644e953d29cdb5e83a8b00" },
                { "tl", "9e47074ec1fe257a8d4809c436e0b91464f98dd72222a6f053e6ca595e5aa3badf86ea16f8a7ac9904b8eab049a56bb9e3f3196439961f78aa3c1ba2d4caf23b" },
                { "tr", "74a71762045b3cb8c64656ec241cee539c8164e098ea5fc079623eed5e67e6c76ecd263903a83f5b8d97017e8b61d1e630b5a05aa36aaa0a678ac9fdc028618c" },
                { "trs", "6303b93c2f7c4944a982c35be4ea426ef93c3380244a576885ec82010a31f61625ea35964ba15a1ec5b7402298cbba073d2c60d3f9ac4f72ee9b63f2ce53bae4" },
                { "uk", "3290f4c54d8f40fc6c79d5ddb2fe9a42f028958be40904f99b4985e40cc59e83d81cb18c3c5d9128490ae09f85fa1c06487b628bd3abf32a13952976c1989410" },
                { "ur", "ab2dc3cf17ea17c637443d9540956c5364f5d77045d226d578a8556b65ef59f6f8615cee1ba2365f1532f8a0954a411c8d1b99388ad25a508cbab5386d02a257" },
                { "uz", "0ddd3941d5bbd87ba3781e96c322c6868921265aaf78588c504618a4a5cdad9a5b967c69c68ee7296657aed99a3cd87525d88eaff80be9134a1a190b3660f470" },
                { "vi", "d4a477dddf882825d015199da67594644d3d8beedb2ff770635967fd4d65a4ce403d5d0977ed12c78a4335eec689d1d6a718f3acb0e6d9609995dbeb49806cd0" },
                { "xh", "6c784e3ccd3e1905a54d200fc33c802c90a4f54d5a9d553714e8dac7cd37d51af91a1d12654ecc77b0b32be1271ac309eb0368c4c53e58fb1fd4f0fb5404a615" },
                { "zh-CN", "ca5de78ce5c30bfb542ee7ca560bf0b89e3be8636c039b3f51e91d70a19d06fee833b74c27c28e249a8f212a74299626aaa6954bd38dad517e00e1e5077b8b09" },
                { "zh-TW", "6ea453112dfc467f46d29ad34eb4a4d4f364044c62eb8b2dfe118c99f21d3bc5f3e448c313ea392744b255a9365825f25ce4aeaa36d793fc9007603989254f6a" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/113.0b3/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "e4a70163e91b5cbf52bc9d778bb4f836fb9a8ed09bce9fa5317e759bd8f0e1288a52bc7b53e96f938b30397470a4fdf59ee8e8e99deeaa1ee9a9bbed3fea4bcb" },
                { "af", "72654db2a082ec887ec38ee5ec892708eff47cf22b7214914ecb75785a62708c36a9335721c04d5735a905c76c8cde81dc4c925bcfee9f16e30b1cde6b23725b" },
                { "an", "c3f3b42a9984c26deec739d36bf326045b9c08fe072f00ce6760ec7034a27c75c698b8241f8cb63ec5d33c75fe74ce504f609ac452351c2e91a71599b33f0c58" },
                { "ar", "96028b038ffe3b5e6abf86553754aedac1a4c43aa2eba90b34a0c1ada8fb6fc89df6321071ea0690a4574e648ad63874ee828cfd567f42c73bedbcd8950a5132" },
                { "ast", "822ee227a6d3ca7c9398e70072e450d6e2a96a11e549b82022f8dd888cbb2025d1470fc34ac42db6cd7238f00f01ce3167c361a627619ee19e34aa41f7e50d1c" },
                { "az", "633d7e71921e779d867dfa92afff4e16cbb2b987f4ba959ffe98bd03a106fccc00057ddeb1ccbd2469d32339023411c40cac6e9ed2471985fd08f74d312fbe02" },
                { "be", "6d587b36267c97d510a3e635c7252c553481ba54fefcde4422e09a0a53e9f29bc5de908518b7d554cec1d0b18da62059ffe0f9676e0c302a6eef035b981944b4" },
                { "bg", "fc3a6a20e062ef0becbc912dfd4b989a98158f426cc47a03d102c6c684ae6df6a6d4b57c895a6ff582c1e4d74c67408798976474d64f36eef7b4f4316bc93ad7" },
                { "bn", "c27e90657f91132d1893312e8cbe50ef1110c1312dcd574a4cdbb65e8a79517f07122d880884826d81147516c82de1cce77056f26d7bc445f2e8a86f09931ff4" },
                { "br", "15373416226621583f90ade8cb9e1a07eb00b8ffad1cf14d2cfaefe52757f7b57dbf53e368e7cce9aacbe94ec4ffc5f0fba598cd3baaec727a1b2b9c3802f08b" },
                { "bs", "040f46713472257186667289eeb0c1f79218f71ec3c14e606f8fc5da0e075c892c0990f1b266ef8c1a1c7cef121aff53124524cdae30a2ce41025200ffcd3a84" },
                { "ca", "1f4c43517edf772383ce5dfce93afe6b210e0e49a31e25d5e2136b29b8c7702d4ddfb200a434de5c59921d6f67b2ac13ba980d4e1937b95c0139e38d9483e2c9" },
                { "cak", "7c7736f9e8ae9ee51975145a197771b819a51685e1a08a7d379aca69a90809b04e2b11a91af9e075992b8dbbf4091ffdad5965f45f9fb4e64b2e04854bfc13b2" },
                { "cs", "5f79454461e296942ca295412b9522f9014c1fa5db719d5a914276d68e7b47874c8ff83433e9bd68d7ca745a7d49bbdc37975ad99dd83587064d70e00b553d01" },
                { "cy", "df22b9685ff7ce6dd835cd15b86230e6a0d5907279fab957773a1dbb7a0345504f86e8ee5efd8efff8a8acba77e05ccfdd4b62ea50762c9e5c3407bfe612e58a" },
                { "da", "1aff8ba697ff8b31f996af23cab28ac3df6102fdc3e9bc274652f334e9ca35d8f9902b124339c4115ffe7662382e441782a22a3e825e7eafde1518b2bcfb109a" },
                { "de", "864377e4e977fe32fda87afee7c2038501fcac69c06dbb286b9fc8a8db9cae3c0a75b668a8e7401e36696bb265ebb6d3de2fe55c0b6812b47ab789c0cf420d82" },
                { "dsb", "8636aab8f779516e8502efe703d9512249794da049231e7a089e77448519fdaf2e0832b7c683d408e262e3e94725564e732892b5ba0b75d6edb4aaa1f2d226a5" },
                { "el", "1ff7c02f60feba50ae25523f9b93ff382882d4ea745f151e66c92e122c99fdd6f2ea816ca164ce90f1b82890fa599df86515da56f693e6100db8e10f0cb29d4f" },
                { "en-CA", "2bf79adb4d867faaa280dec3a32184bdaf26307183a973b0e3d2e43f63901aa59096b8ede2b039cc5bb439ae905b6fac5815df623f68811c86d9bb1f7ccbc7ae" },
                { "en-GB", "b59818729708b3e4bdaca55016110ec114fcc86d1acf3b1a049adb9dbbd17773ae4e5b49c8604380b5dcde2cd4d85f179e6f419557fbad726f6b979374367068" },
                { "en-US", "e75e65cc90aed35e250f15b50b197bcb557c67792ffad107903eaef97b960a7d0026a199b2acccfc7d94280ddc49d0c2d06ac32c589ae2c5296e2163bc214c13" },
                { "eo", "9a8836b018960ddc66e2f154e8b3f039b0fc11c222f4fa3e8f36cb3e4d47cdeaca5f87c2a4c22d5965ac4a39fe0a340470c73e1170b541df59767d9162be4d01" },
                { "es-AR", "b2a78ab7ffea18c0a167199d389c0a6c8aeae3937156f5ef162ca2c5cf2f8ef04556e480dacca30f68764de4b67ec12671d0d9bc6022f5568c366c00a00b2e54" },
                { "es-CL", "39329ee6dbba9388d3e311cf47eae1a09946c089cd1fb1f90e1bf575f887f1385ae9b4fe1b2f0c26fee19395a4ee456f37ee418247a3a585b73b89a05fb8dc90" },
                { "es-ES", "37c376312f39da08a5504bf429f05ec4b5472bf56b5f1d879d0f2f2bbd3e7fe9cdff413135b4eb199dca71f4e59fb312d8a57613c66bc5a3806e4c404cde2d4c" },
                { "es-MX", "7db126d903e1f2b9b660665afc4c5fce0a4ea8a215283c26dd1e392ae79f6b6ba206a2da1126c713ddaf5544877a3ef3ee23d2a7a5a4324f093605b23f504071" },
                { "et", "4215cbc2631af059b193df49e230f61ec7a7378db918afed43e43eddc441691204e55e8d3f941d19de7e2106b8a430309d5cb498b829baa621494a35e1876b3d" },
                { "eu", "2664ef5f43c5d03735f484ac09db5044bd87f85454b5805644557363a8732a8f0c8845f8cd1212fcb5173536eb5f4ebcaff01f77864b790ec542191bd8b06b1a" },
                { "fa", "271755ade91381feaf6410a54d8dd329e50b739c6db26c205f381269605b634daabc9da556eb1c143ed9dcecd0228ac59581a295bd182e0589832d96f3aa5e3a" },
                { "ff", "0666c7c0e05d544ba008d6ff74cebd3ba286ea42b12b82e120e8a989a339b77a283137cda49d2ac981d1a538367c3ad448ee1c5f629e5f8e032c2189d0cd0d66" },
                { "fi", "7f1266bce9b6fcc0c5a98fa3f95a5e113ea8641039003fd880b0d630cdd0ee0bc03815e381e92476c9a8374768f98f6141cf503fd0eef4e92f8e9fd4cf5cedbf" },
                { "fr", "acc0d21fd5703fc5319df4f96e68aa9ad728a48e70aa0edacfc72b821c0a196c12df183f5fd203a02cc1de60c3bdc74c52c11505d330215f201713b2e7aefac0" },
                { "fur", "ab22a18c8cd7e5f8a8a86f42d05ddee6ce1fd71aaba6020166a38abde5ba2ab1085bb118b582da4d01fe32f8cadb11c77eceb2fa668cd63c3afdc4bdee60b20a" },
                { "fy-NL", "1b1e885a6b39342176374f366e513e25d29da118481ca94e920ec1bd7ac38c0a761931b4cd3670832744e3b4e590a078df9d9408e28c60bb2e054d1e141301e8" },
                { "ga-IE", "0cb3930c17d9018e66d713fb35f566e5c77ae9900287bbf37344faa228466d0835722fc694845c7380affc843702596bcf37f9fcf7f0552f3c7bcac9dd65bb76" },
                { "gd", "0e21bc8e840c996591d13ed7198949720062c0d97ee62d8374f6884d53fe9025d66116fbd5164a4513fbfe003945f5d386c5dfe91ac1ac1823875ee46abd1ba6" },
                { "gl", "a34781294a37e6227e305f77936cd96668bd0fd90fefa6af80ca0d5e80c16735ec5a9f69496bfac6153f8e5f1c241f4e7d0d08b6e537f95d05c497e12c80f8c6" },
                { "gn", "45f4fc7c612b3ec9cc2393c875aafef0940d1ef5b16eb35c7741e07d70deb945f6b0419a026041233bfaae08258e4e7328193f004c5e3792e9fac7ac0c484970" },
                { "gu-IN", "76df85c21c8f898cfb81c8ede71993b81a7c417480ccbe2460d48492f3c1b8504a077026296145ba446723d3edfeae74ca3e3d52bbebbaf927d306da19320823" },
                { "he", "8ee72d780ea4efb51f712c65756def45df4a80ee95d973c44ec0095c1e7755f07834b4b2981ecaf1f8f7dac492110d9d329370236f498b91b0deeb999eb35ab8" },
                { "hi-IN", "a63468f41fb87be3423dfa48627bac283f98f96b1ba0b53b340753ee4e0798949094733ac64781fbca0d724501d3d5511b028ae2e49f0328f791fb95d45c1a61" },
                { "hr", "94867dc9c96584f9d6ef420170db93d7efa9c4c3973195360a0435e6f8a6fbf3883c28286d055113eea416e6b094367d8ba4b7d4488e493437a2af0f05ecea05" },
                { "hsb", "ac3e0bb4a76a776e05f774d312f2baa1d3ab961c41ed2f4b9be8134021bfb6bd364a7b9b9e694eed23b794811f80c5853a3fce3fd4871a66f065a75a1d056701" },
                { "hu", "39324e44efc6ae22300142aa455ceb8d28917fca317dd734e89ff0b51486c9dc9b5113593a4123f8b250ab8d3eac53857ee538cd24003f6ae1e90280b967eb20" },
                { "hy-AM", "f09c2d248c339ad537f381fc4e6f622f2624cc503fecc2c3df67ef12be8c010b5566e5a4389f885ee174813b2a88adc64648c1494d640245c4355f9e65cea23c" },
                { "ia", "d0ab4db38a03626b258fbd8ec23b5496c6293915ef8aa516666d310334df320165065a6a89797fd5f0926b3ce5576b2fa5c03ba80b9ee6e5375b38049fd7dffd" },
                { "id", "f60581f7ec497c905684952ae2294d667c76a88e0a5179c9ef17f12c1768e69d0ae225aecaf028af77e1d3b179480e1731be16f50dd50b6682d61b7403cc1993" },
                { "is", "7619a00c340290f8eb02347312d604aeecd1c5bb5c483ccb2cc0e9bb341cb8187e69671ad974f8f3cdaa6c30a39c688a3e1ec4a60c805b07fe095f625f6baf8e" },
                { "it", "87f39c01f9a0700a3f28cfa65f991fd97691ab85c6a263e9e2acea9edefe463600a6bdd4e94184c3c8e9e7096c2ac2c530549d0df9fc738b36032470986bc5aa" },
                { "ja", "e2d4878c9ef5a9bac71a04a878be08d5380c220ac0cc1a54bfaf1a644b60bfd13c4e73f62f31d632c25cb9ff00a69d30770f9c6621337d238db2453e95eb9a0b" },
                { "ka", "9fdc50a0997212cd04d2a4bed2ebeb179724ea2a49099a0eae59fd63230961e2ae885743377977ef8affad22dbb19b3aa24362a3802f5698983b79a84a4552af" },
                { "kab", "cfa26ff33bda339c61e4f8f52678c19c1e21b096048b417fc69122135f989a4b4925d9a8da00ebb092d0ad6f5c9fd7876a08be22679d7bdc6fe156337e85edb7" },
                { "kk", "2806945eaf01101d94d5173a8d21bb1abf30041a534c82ccce07de967697626106a6a3069d495a7eb35baf4f586703f53b8d8e66a5428dacd879e972c62e8f49" },
                { "km", "fdabf3ecc24fd7b77e28bf3d8514f63651bca19382dad9381a62ef573ecb7913137d5b462a0dcf9483c9e3d2f8bcc704faf57d839f3263fbae091f2397314e4c" },
                { "kn", "92e470a637e79531eca3c84b642b2feccfaf89b7112f71c9662400b65374e14100c673e1ec58a6a68ff4c52f62b31c40b2141c25adeb6ef6c52e8af2df3a82cb" },
                { "ko", "2cf26c75f9ced658358a1ed7bb17ea21cdc682c100e81fdb9ef9139db8c2ffb293efe136b14d46654aa7cd4b70eae3dae98830e0dd71f0cf8950f932e9249b7d" },
                { "lij", "c129a3764e8b8306279725c803c2cbe247fc6aabf2e20aab590878866a9ed70d2520bb9cca879ed51f568f95d7dd0cb269038f0ebf386f4da3c3eaa644c2c62c" },
                { "lt", "dc21a1f42b75a4408f09896f01beea4f061fd6efe0e771fbec0929ed96a36a155ff993f5a7c93e42690ae072e4d35dcbbf07425e78084937fddb6c5815d8dfe3" },
                { "lv", "d7d13f065950bf18879ac1f8a1f5943c0777dff556fa137906883a33fbf38b5ff65517c0bae10be5f94f774f36fecbf39c8b9d03404559ad072c95e1a01f5711" },
                { "mk", "ce0d12b4130de72222edc65d2c646c8647da299e0f099eb0b98136e884365ac6d409754ec1716b7718ed73b45cf569975c9e44e0c85fac84f5327d3f677875ab" },
                { "mr", "8b8e399877640d7297fd7af354bbd927181284485da727339a2b59a7a470c67e984a584cd5db43be755053934065ab2d91a72901e89f2b6688ba3bbe47129d48" },
                { "ms", "e2fe4558c7025e6c03f507cac4c888d8bf49370687f6c0883cb9409ba468ddb5f3551c009413a4f24b35ca65fd32660baea5f59499068cdcf99bc95b0735cc19" },
                { "my", "fbf6d6044c8785afae0b35bd0802758807da6df0babfbeb9d7491d202eb75561e0fa871e676d103be55b2612d8f1ee7fe32783450a178c8f891605f560f4067c" },
                { "nb-NO", "9987a2e21358478c1f45ff4e35c2647179a23fa223eb58e918080b8a75333286f3ac194f6c34aca2f7b52ecf22d075bf0b533cd2f2ee2812e9c8b1ad5b129cd0" },
                { "ne-NP", "e8633fce38d1e7f29c70595ed60d74e5239c48ad4ddea9778a5ef3fa19ff2eceff18444412355f865045fa6ddbd3ff590905983f061bf4a5a2073bd0da8c2040" },
                { "nl", "996d91bc55ea4fa28932df09b438a00412996cf09837c9315336de4393158f683f3be6e42df217fcd3f03039696def143cdd9ff460064dbc0088016783fcc138" },
                { "nn-NO", "67efb924ff0c44fcca60cea4d9b13ca2965789cb5c166854c8558e8fa55a0ee3b9448e32f01fd8e10f6ab35779f3a3ec3938635524fe501b26d69bc34c9f2e52" },
                { "oc", "c723939986a2eb15abb4382e524df24742c988f92336b534431664039b798e9f611f3e40b5ac73b0c803745bf9c7be21e249b331a2113ec9b5f0f763d45105e9" },
                { "pa-IN", "f9fe578fa00afda64842dd95b668c3001a082f4c83159738224700f0b12b9c1de6dc911c1e6ec9e12451d12ad6477860e409484270e45d76e2ed91a8d921283d" },
                { "pl", "6754f5fa1fac494a881ec7f4d177229f2b5926183982cbeb5a4c3611c123007e2dcb77447c88a424c5cd03a522fe95f8bd0ac1ce98c57f9ed23be87baac927d8" },
                { "pt-BR", "e046196ed5748d2293bf5a9084d4d9f20935b54c020e1e1a2d3306e89cd5e5de25b39f4658b9b3592f3e2305c13542cb22084b57754a015b69706f9f61841fb3" },
                { "pt-PT", "28a9bc3438716409edb8f7b051517b4c72cb98633076a2f52f5b6e94f7f63a79af645657328357e10ddd2e938074287cb913d2c77dc31c06a4e1602d5a8c7948" },
                { "rm", "105beb2ee3ec0109c5cf95c191cde8fc8947b3e443d660d61073dca68b3a2f48b6cd9ec6fc7a5dae4e0d25e1ee7f3c87c5f3f8f580f2a09915f0a9ec1a7f7fe9" },
                { "ro", "ff63008f3f4bbf6dc5564cca5186abb6daf6a50f974db38770482638f76d0a92b834ebe6832f8496b07309b940b039642b840b30a3b1c39a67239b61db5dc9cd" },
                { "ru", "e31ba00441770e8347aee0b9a383c8ea0952bb62de649716f739121084c68e217be349e957f4ca870ad0ff04adfe32c98fd270f0043369ea8762cdefedd8f31c" },
                { "sc", "612cd21ffc9f07ba099d752511d6edc9476ecd748dca78fc22accb99fbfa9d438801be9ee85eb801c83767d2c3a976ac7e1a45aa62aa422bfb51cfac8376a5f8" },
                { "sco", "46d84edf3597fb59c83aff58631997a958967a8cd58b98f1e0471eb0c0adf18d8e6f9e0645751875f806a5dbcc08395a87c01796cced50573bacce367370ef9e" },
                { "si", "e71dee60f7e88655dc1f4f05ea3444145f76a6ecab531abcb3de9c63dcbf38c9050cc3ad5144776533d94b47d630c12ea0e4fc1c6d8bace2ec6286be945f4eec" },
                { "sk", "be84c69c13feb2d837c0e736bfa1781354078a30f75d9f0b28a718a77b1689c1916c8c5c8f78b2538f200277577c1549b98c60b0141cf58c91a2984dfb9f3394" },
                { "sl", "b9cca03c07c89ecfd3920094a2d3acf4781f4dfed928a0afb432ec190a55d1ce49097279cf3d7b79028677f0313e8d9fd6448c5f87cdb45a33bbf8af70c21ff9" },
                { "son", "1637f46e408727d89e3bf105ef3b605635a4dcefaf47543d150cd25bf171b4eb027c01f04e95bf39193ef9c35dcf164e35b86c0698c1090ba954e98e5c49ceaf" },
                { "sq", "4aec03f9916bf4f88133d27c96a51562db5135d3a5c02b093e73972dcbb72eecd65c5ae142512e6f29cf61d3e76fe6c009d2eb4a0a47d840ed22460b44659555" },
                { "sr", "24375ec9f3e44d2512439179bc141366f33e7c7fd3792d33a6cf616aa9af9d90a6442910bba7732b486e34288dba1b5acd3438a452de3d6f811ec39c7bda8d1b" },
                { "sv-SE", "6ea388191750568e3f1761039cd823513dca73c861940338596a9ae07cf52409664b3ffef4ad02244095daa86faff07c4f4719dcc13895026faeb80eba685eab" },
                { "szl", "e1698e41c1034027246fe27e761d4a4c65b396d6214d06bdb784d95fb805525d60b8cb0d86f946b5e27e1b5b172cd7513b55a652407c77e3abe02e97c0cec64c" },
                { "ta", "557d04056d51006744dec32dee2d7a67797ff426a3f06ab9b4935124f10fbd0c2fe7f2edd073a0ed224a3770cf065af726f03014bece85051b024966d1aaf49f" },
                { "te", "48ce6ade3fb72c0048850afb913fe6c3da163846caec89c8ba94982bedf121affe4911b2bd46772c96486dc6565f7da545838c5619c2b22acc3f502acadd37ca" },
                { "tg", "9461cdc3ac4437069499794e936678e1e047dab483783ec6cf2bf278f5a5439e120249da3f17a506ef2f8e2872c428c1656687e189299be3b817e65161e49782" },
                { "th", "190c5f4b1864ad796fa1ccbd32f5736d44d796ccc81576194f450aec66184af3a0b8a22bb75bd98731a2b9fc93baec34a756bd247fd0ad5c6970d8513bac31f6" },
                { "tl", "4cbd7dbc52a6a83cb9f5dc852bb1062fcbdd29183774f9ed64a833981e4e4e35714181107ec27d656253454b862d50f310e098ecb5217e087520d0490c8c4112" },
                { "tr", "6f02eae8ed381bb186b04c4780fbcd3c1d8b23b2cc5881c3917e9c9f44c9f985c65fd9d127642d2e559c7b3891027885e2599b785f86db999e1f59fd881303d5" },
                { "trs", "a847e6f50f22d63a2bafc36bc019cba70bb51b38302c0ceff1294eefbb9b910cf0a41fb1e488acc0daf3970c0bd019bed487f10dfbab7f07be6b43860154757b" },
                { "uk", "742c940d5bc02661c3c7fd27306550aa6d697b24c8ce098544ba0e495e33ee09e7509dcaf2ba4513d97ad2f2b0762fb383d483a02faef62f4ec800a4241bf2b5" },
                { "ur", "57c51f8bf5e043f82703f3f56b3434a16b2301a6ca5dcdde422330213bc620541719c17c786f5e3f9ddd8ec7e4432ee87c8a018959cac2a7af733c5a23763e6d" },
                { "uz", "d52bfb6657e0e31a47be8888e8545dded1769fb5470ea94bcc60ae1df684c7d9b3a9188b6f1d3dfdfd0cda9f5c8228b4c8e24e34ac813be0c7f73354877b1fbd" },
                { "vi", "0e6272c2bce578f3039bb9bd1fded306e7c6922e24930913b9700a7ad50bcbe5af7d7fad63e7fc33c73b90e804198cd7903faf23eab7ffefabb58973490a2029" },
                { "xh", "264950eff6cf43b9e03e7e973806af36a92730141b89bd92ffe55cd1161e3497f15e570d054abf047520b6bd32e699628bf229d39c35757b8a372ba7a238ab91" },
                { "zh-CN", "377f018f53a615a65e6aa2ad5ae4341461379377afe042204abb662bfc65cbea01091c0feb7a5b77c2357e4c2dae1b95e4cfe04ba1a01ccbbbed576ca8a8f1d0" },
                { "zh-TW", "9bed9c492b2b98c61c5a3a2512b4703138b876b55a428a112f653132e007edb23b8a8ab0558aa8bcb03d2a92f4eac81d1d3c229e581a64dc1c81f623f925232e" }
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
