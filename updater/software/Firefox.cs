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
            // https://ftp.mozilla.org/pub/firefox/releases/133.0.3/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "8332b504f11d3bed7b8e2260abef9d10243377377b5c250b46032aa98404ad522df6c0f108381cbe3b01b779965aff348b01d3387d90fda6175b7f5511382343" },
                { "af", "7f559454fc0267829a922b2e1714d2369bde365e3ebf754e0fbec3b352dc7ffd1d59ba3885c103e725d7cd0be7a7828215505afbc8a9ad21438c8aa6fc4cb804" },
                { "an", "6df5beee54009a6092f451b3d99f4ebf8b0534dfbbafdca0933651e57f04a8046588693605fbeee7397df20720af6aff11fc6503d53021c8adcaae022907cadc" },
                { "ar", "26b86fa9ae1fd0c93bc5e3f8a761887fa62391575ed79597f186b1118e04eb31898f1fbf7d09c1c1b8899f4ab861ca19b99450246ac3c6b8f06f8e9ee1e2b393" },
                { "ast", "2e5213174c8b524de3db7fe8ca33c51a9cbe444401020da497b4ae41d5f5d7bb1412e9bda491f9780071b11375c5cc2fc20576c101d2ce772a8fcb9fb3319975" },
                { "az", "44eb3aa7b90e2eb646d5f6c4d5d6fae623c6986a0a962acb365c178a73cf19fd698dc7f3c8339b05f495ded6f2bb27733c4fd2aa0b0caab0848b350351eba90d" },
                { "be", "758a2205beb3d8cb1602a7e079f1f7c1acbdb41dbba0af476ff779fab3fc5a8308e2d37ed9aed1d0faf299b53ed54fb9218c1cd984b86d14d8c1ec200d6077b2" },
                { "bg", "c74dc52b4b3c3879231bf2f00fb51af84589b2e7d1a297c1bdc47cf62ebfb8239d4e949acb6b9559c71e7a2bdecec5242d6107bb82ea37288c434b8c3fe7bafc" },
                { "bn", "c6e3eb5e33f2ca317095fcc9c318c492bcb307a5b929b54edbf8a171fb7a9647873c6c398822d82aa3cbc2eee904cdd50c86503ac64ed6d85e7ea0e299ed9c13" },
                { "br", "4f4b4884c88e741d4f3d3747ecc320006b63d4f63a39fc36ef441b8e9a435635767dfd22556bfa452641ad68f370e5bf79b3545e1c09363186c0be851212d524" },
                { "bs", "bda2612e37b9b1256498ff75bfa5870942e53a89e6364774144ade2300dd1f020213fdf2e7ff5d82d59366480204f2d70abf6812eb55e9ec90df3c024334e289" },
                { "ca", "ec443b3836c5e5b11d23ce3a69997411bcb0e483f837ca569f08660eefdac90f180b945e2d9b9e5aecd785230a9af77602969644552cab22f793a8cc6a535c33" },
                { "cak", "404f7defd667034ba5f69dd3e706c00c4d24477eae5ee9ae890b4baf5e0938f6d675968bc97e0dedb24c18c38cd4000edbc085b29ae0c4867cafa0c12637ba1f" },
                { "cs", "1254b8645ac6b8b2174c159024d9a5c4e562d749293cbe3a4dbd58a296e426519b030c17756318efc6a3221f0f840b17107983058bc901a03d8e25573f2d4267" },
                { "cy", "7660e50f8e25382b309837fc5505a6c68619511a10c14c35da4f96a985450116e41f3ac06efe4806e9fb18635e5f22c3117434a308d187d100129674df438d16" },
                { "da", "fff219757bf27111fc01749237fc54340b5a291dfd65977736ca994a69f5e392c89260962ec6214f14e808eb57121d38df61995906209aaeca47581dc7d8252b" },
                { "de", "c74b21f04ccca3f259f202c6c950d7b94304f9bde83bda2da4b8c30b85e8483daf1b6bb0bfc6235b8b5e1f4b77ccb53c0e79b77cb3e3de99c38c11c93c95d478" },
                { "dsb", "fe79375828ec9716538a11c700344a68dd82198533d3678738a9dfdd48c682e013ebbf89d8545d9efa675524b4e1ddbf259ae4b23edfeaa8a27ace60ecbd2173" },
                { "el", "3f9e5e0dbb3870f305b941819f54eab1b85f8ef919b3674824ed0b8d69aaaf017e49c6a53cc8ed7af59b86e2fdbdf3e1c3e1f92185d7d24b982d8b3420d91549" },
                { "en-CA", "cc184d2ba42ac4568b8edb36a7bad636920ac3dbdbdd036d9ad14bc421152e04e1b907131a6986dc03db07fc4de7bcf4dd1a90764659b8c1c886c37112b9b20a" },
                { "en-GB", "7f454e486040da3b7266921d3343123578247263ecdc070e2dd46a7b63f3c43e1751d4dfc462d7b8cface611d893ee390b8f6b2360118e0ac6d40f258074b54c" },
                { "en-US", "162879d13ad44b221af0ed7d4e18834eae8562805401764bdf41e5c220cde7cd3030a3a805aec1384d55568a75aa215b4bdda8d775177b18375020f75824f5b5" },
                { "eo", "193666b2e9d9cc673b2a37396f761e4d2231893a3f9b62afa4db1583833580bb1242a760ac0124e41df1ff17f0cbfbc8ae55818e271ebf5470ca92a37ac6e793" },
                { "es-AR", "4e6667dc7debfd04f0461288cc73e931d94071b0d3183871a11d73f480ba8dd0359e95e0d740a7f554b9b4d51d3c005e44fd48adeec092048fc0f2e819b1fc04" },
                { "es-CL", "97f7c5df6e3c8196999a1b8571bc51fc15348670b7732eb62f24679d4c6e03adc49ea09f1590085aa0aa79b55777f9c96e8fe45d22df10b85c2183b5cfeb9222" },
                { "es-ES", "faa929cff25573f5c40bb6ca8d8062a751662fbdd033b45dabf88fc8fbce58bfed3f8da5e712c80d6b0a1425a02a4ff774e3f4a06e28976f5284f6181fb11668" },
                { "es-MX", "a452a6150b8710a956e125b67fe19725ecb5898a9bae54ff865763b6d60bb4d0d08ccce8ef5395abad9c6735b340fb40be39df710f6117c776e3ef55424994d3" },
                { "et", "4bf2f5778c3f88d8d7e4dd1c10fa2734bc90160982e15a16e20625cd968294c3cd6fecb92649d7c8e8436348ec2180ad3b8fc6f5fe39e4b76a749ea9ba96743c" },
                { "eu", "d4db10a5d8e9f08ccde93f9f4466bc17fac58026cf356a86ec16327687f98d3ed7af7a640795d6914d0eec296cb6f5f81b16dc316bf1a93d6bbb62511bfc740c" },
                { "fa", "3e0064677dae7278f4e520d0b3df2928754859570a69453f21446ade0b49118850e6f9b0a032cce91a5a5f5083e24955681467200a02b07b18b06fffbf670cc9" },
                { "ff", "0f46b51ff1125e6ad6d7489f82a06c46311f727e092322e3a0f4a2374aa5bef2a625e3155e3cecc87890be76cda2d67080cc5a829972d92f4563a6a80e170569" },
                { "fi", "053e3021c75cdfb2d6f204954541d96fdbd33529eeea7ba98f38e656b2da1f408ed037ce298105132395469aa7a28ff7c5ede961e7a5449433a15b47bd6498cb" },
                { "fr", "5f0c2ac0b61ece23312e72c2ca9010338d6d012de402800c8951629443171c6f8f29c836166ea0e6cc21512dcdb32b770603ac1d34d1893b6f73adce58ceffe9" },
                { "fur", "04270aeb847be291fadb0b041aa1b2e4d52e3030a5666efa5b6cba18628ef37e81bd8bbab2b0f69451a02ade17b05479b77bad8da4e6aa10df1d0eebeb4b973e" },
                { "fy-NL", "f64f3e4c5f3ad607ce1fc71059364ab4a6d4b589838785b030136701e7810aabe1e7f9ca386a52fd47f13d90332cef2b608d3c68d99590355a1f41f5992e3e98" },
                { "ga-IE", "84f75eb49fcc82ba3aa4b0535b8e8d27c974f891f0865ae1ded02a8351dd7f5bbb9cd93a07a7dfc835015cfc01301809e9494105efc6f2cdf9c1336838c6acee" },
                { "gd", "e49d30017feb254cad2e845f62eb5698aec6d1398658994afc4dd98d752453d1a144d7eb56347af7feb566ed600100cca18fd6d4808affa103996432a08c83e5" },
                { "gl", "8014d3a00f6f06311c3d7b216c435a2dd7fdacbc3c9f5a6a4e6e402c453ada5420593726cc12c5cd41d5f53daebf3b38fac89e7b95ff07f9d62afc967e2da592" },
                { "gn", "963f29777d29273d7e7284313cabcb8a69b2b1a9ccee08708acd312b73b1e9400c355803214bd71f9dec5492510bf32795b904012aefb12078fb9e0a578e5b2b" },
                { "gu-IN", "1755f123c61ee388382b6db2f1691ea524a4ee9d3d9b838a484bd9d504a7fcdb1f66f414fd60f7279f15244fd594fbac926ce79e291101534c922d62dfc11493" },
                { "he", "97bdee43e955c7b84a2f7b3ffd22955072223f485f3e16e182e2832036396daebf988151df44208efe89e68a4592964c2c27be9cf2ca2c65f5cc286ba69d7062" },
                { "hi-IN", "4bd5de000848a74609617655112af5a4250e75ddff1329142a490a8b0df9eef728375af16188b25343b618190b00fa98fc5be06f720cc016987b499252604df3" },
                { "hr", "0deb274b133f0c784732b45c64fa83de8102813a33ae840fb3e6fe74bc96b62f6af63e84aa861436c03370855250f25adffc836d3efd7f68b57f5085c45d6dca" },
                { "hsb", "def83583ffc758ad7a788a16e462b9b240bae056132e06066a4de7373c1eb3667e31ca91aff6c24c21985ba9e68d5b33cc8fecd24ac3883445aad64b064f8145" },
                { "hu", "3eadc4ba91547179f0117eaddad02449364a633e638a174a0016e73a85bf4dc25f845e24622352e4a50372ec0880c087fa76c688298a5b9746e6f7ba82ebf28f" },
                { "hy-AM", "f3b24946a42b0d2ec7b4d19967f12da89eeaf4a957dfcd744ed485113e9b3dc284c94cc2c440c95c1afdaa4f4be7934a0d931603be059c58d6a2f707578bb6ed" },
                { "ia", "30a7466592344bdd178c37dcac2df7f0dc467523687010fe81fb7c13c1e956b1b403a1765cdebc2b71dab7496da91394bc39ed6e88635c214fa6c0c484bf41be" },
                { "id", "d1e08829b8dd470f87fc6864c11b54f6c3411a490b477c29116beec19b1ae4f2d802158336e6f00bce6b3a0aa09951ea661f4e86f43447a3a293dcfc6ef63392" },
                { "is", "94015d51a552e4fb79a276b330bd3a646934ff94495453c16d2b384c64442dfe0ad3ac455ea16e0788e714aa974892868666dd3caaa1a0d9d6dd30d1b805b7b2" },
                { "it", "108ab7ef50a9695ade9732427c9a62f0659192f4678d24b153f85ef961d6e7249f5ec7e417c28a170e65f301b08e1ebd6c0bb78f0ca8f001cfe285bbd03062ae" },
                { "ja", "a70420d8b67c7d57b5d9a8f176091d7cb3c18db9d0f55deeb7b811e291670277443ec5339ffec73fc1386a219544dca10dd166cbf03dfa35616edfa171c49e02" },
                { "ka", "da2ef33a00ee2b415529eeaf425c236bdaa20c53880c75e35e542d548207b7f07caa55cb9b55d7490d5c10e27aea1da1f78a2506c79d5fddc930d6d545b1ea8c" },
                { "kab", "4636346853265b2ae4a854bc7c1a2160ef5c280c74f976cdd1bc7b207c553bb5aa76bf9963204249278f5132e1e9eac46966489bd4bf54a10715c4e048a30a6f" },
                { "kk", "3e69541db1c6acdb81f259056399ad327a3616f79890955edf711c1e202c170939410c597d531d962159ef26b74c9e1beefe362e6dfd326a96efaa9963c737ba" },
                { "km", "a5188f1dc5764edade25d2eace7c9fdb61e6743f7a577e2e12b828a58dc1aa24abfbdddc3ad624e2697377d337f517432226015cf6186217995275a56669767c" },
                { "kn", "1adf0ca984d2be2fbdd8c802798579dc272f87c9bc156edd04edf6e1052bf60d6ce78a4fa46573326f0988486b7cad861c904a65406092402817ed485c56a409" },
                { "ko", "166e2d0e09a201be4a1aa21625081aa1ab5a93ca779ff11dcd2ece6eccc0e5cb08ff33fbefb5d7e05f4d2f03f83d1dfac5100dbab49b7ffb28bbb038fa19b7a8" },
                { "lij", "bf0d70b3f988d8a36d1420600e03c049721d4f2b1af4d6ed7dbe748961d71bd2f3d55f2749fa1171715218e5a65e543b4f57f39731c3e7a718b582f4484cff05" },
                { "lt", "6e8315b9e9486220b558f8590b1f7b4355482d842afb0acadd7f5d78120de4fd8ec49fa543baa3f1217f9d5e47d713497c94e47309f17b10f46158829d2c9177" },
                { "lv", "58cce415a2561ac2897d73dbb8f144492629b8ba76c04b9520c0f7ab6a960ca401b01af981bee908f897e24d227b2f2727e70a8ae4f645c4893b8392c3bbff37" },
                { "mk", "ebf7ef71c7011fb2f86b019ae772f77d4472b64fc0f1ec82821ce7e79560f7ccc68a9ca66e26140f14b5fdc60ee42a3623965d9a0a140566e561cdec5c2e47f0" },
                { "mr", "96f6a04b3cd1e75311f1da3251658e3d6bfb4bd7ae83d1fc88e4866eaaf8a24a5cfd701acce6206a7d55309686c514aa5b399a3426adf54668e9b8f3b73baf4a" },
                { "ms", "56b220be55912578c06e850be2b20d819f2ad8fc53e240db9abc0af4ef00c2b246d6384ee7cd6e7866d6e3148c3b6e3487b81504893d30099915fdada2b1ffe8" },
                { "my", "cee66db0a94bc846557694541bb9a1fe9d320b1ea73262cb50e8e1dc53765c2b61c74f033ffbec87852534c85fe8d446808ad4d3b82d4bf445a2cdd91e0294fc" },
                { "nb-NO", "9988cedd3883d4d1f8a2de3428eb196341a8cd0d572ec802bbd62d0e2a31b654fe9ae0bd433156b20ff752210fc9f4acc8cf5acd3e4dfec6efb5f096198f2e85" },
                { "ne-NP", "6520c39316c1f52645855b8d9300d1d192596fdbb2a418acd828a54a258bc7c67697aba1b3a17ef359cd4b2abb1daa74ead97feb873e24e2bdc40b0675f059e0" },
                { "nl", "58b5d90e18af8a8742e8e17ba84fd0875e334fc019ee64eea147ef1cfd1b252341f23f448b49ac57a33af41d6c4dd7aa7e365b6510f8c447f0d44e901cb93130" },
                { "nn-NO", "b75a5d14fe5da1944ced6ed68f38471c26b524e9c9019e53832761083befab988b76e15c6fabe9a22c1215fa711ac1f9759bdb3883181203d01e65e446d91e3e" },
                { "oc", "198ce2a1a6f8ca87e7e4eed4e0e1c7bef09018104758b54a3a47587586ef386950062efad0544e15e748034bbc5f64a4316b5d3658fb683783b8bcf710efe976" },
                { "pa-IN", "1867a691988cc630d73503846ab659214305eb5708d7487fbf8961202cf3725066da19b8a0c1faf65025c645ef921c05a3dc2e21812a48a66343256031c2a025" },
                { "pl", "25855fd0b9fa615e96804a1f5aa27ae4000c0c9f891bf4ce0ab5708ef738f3197318f8ef73f61a2d07a1ba2e51a638545b6059f157f934b1879d82f668b017c2" },
                { "pt-BR", "ae889168d469886b494dfdc9b6c13745edc8a88e13fc6b9ac967599f6d5f20e847ee662c86d9334749441dbec01c1fb47edc6349485ee58b0a8166d037ef2836" },
                { "pt-PT", "747b043703de1578bc3ccb72e2c49145e120afe3851bef883a9b384b954c09dfbe2d1b90446ad67e32bc7f968f570a1237d8e7bbef66a943f0806499233d67c4" },
                { "rm", "07daf18026ca26a04214fd86a811b9ad0a9fbbe677c35289fd7c0d28d0257fce61297dc59b83f45a07015d01366d80dd59c757c1a4b35269954e15f26871a230" },
                { "ro", "e932ae63fc0c6e9439f15060925878e6b70e1f9f0740960cf8d720a6cb1b00865cf14336225a0deb5e55d5e9ca29ccb41c442de530bfa1ef71bfd23b0b48a68f" },
                { "ru", "e4e2808fd857af56248e0323dd1f08fda3e4a5c36751e63529570e0e3cbd6e9bb9df7d8025a654b2a3e7a1a3710900aeeb30e38aac19a88fb82f2c672426b1d3" },
                { "sat", "b9847de2199d8c6159850d0d994a19fd464083597760b7f2497c41014dfa4125c65436d30651b89935a76eb9c06dcc6c49d0c6c1a3b8662cff472cbf777fd8f1" },
                { "sc", "c08178c709f68842ffe7a71fd86fb68382d346f8bdfd22a5a0aca3390c4d9c23eeeaba0abebdda5a582985b7799abc9a3ca72af201ad4bb84c5266dfca732048" },
                { "sco", "5450faceb760aa84f5f36b72ce7ccba29daea662bfec7acf44dfd6e0fe9046afe3b038962feb5025d091c3ba25ddd1af8f2d86a0b5fd5ce40c05e5e432e6568f" },
                { "si", "59d9013ffb5ba988c9668baab05ccff8f60893672eaec6730f9039200ba5f42b69336a922c4398860199c24b386556177ac2ef1603ecea2814c11ef08897d2a0" },
                { "sk", "0a9d071f220865fe6f0e4351b157965eaaebf9aad778106ea90118484ee712daed10ffa49b46a5b0ec51a51b63d0b09440fd98c4357680d8335570d5c5c90487" },
                { "skr", "20380cc5cf63644afeeb5f278ab1eb45ec105ac1ad3833507d16f802b3b71688076b94ce8d4878bdeaaafcad9a79c8503cfeababc2d4b648cb58c10d2599bbf5" },
                { "sl", "76d633e45cea965aad3c4b231966ff4c98b091fa5ee7484c9b2c067a37a1cd21f82a7bcdf29978f8380b1fb783387ef061f5fe75e74096ab482894fe958a838f" },
                { "son", "f4202b1e8c9778a19b5b6b104966c498bb5776353a020b3f287eb3724886540d317078de2179f7307c63c1ee934c09f59e1d3f34d09324fa145f908477103da0" },
                { "sq", "651e7ae9e2b2f57d39a0c4ac618240a3f75468fbb55131253d98ab149a475315f77b848ab4293d3f9c03ec157c3f14d415d3883ef6d7a7fc9c2548d25ee74f58" },
                { "sr", "b075cac8c045aa1ee0066f26aa25828ecc6c8d9ef073cd57feebae25cce70ae07b685210f0d6a3cf3bfcf3555494a5ef6f65f5fcd0dd2f4c100f8f3c8629f09f" },
                { "sv-SE", "4df5768c8752c098abeb3ccb96b3395664a5802053f0b68a9d44ce1a61c94e720be7a30aa8c7a8954eef7b0e044304672421d9718e830f35032c0ec1f3ef8be9" },
                { "szl", "5332af1d9b0f1ef67de794da90cf5d1112d1233b3ca6d492cf6de4ea4a3a80395163a9cd3b5a7c9b2baad9297eed9b83a7bd2723505f46964400351d421344b6" },
                { "ta", "39df12c80429920c0db1167576f7b7e1442ed57af278e9f413f9d3d84b09fc82c56e9e24911f5122fba77da30a83010940e9cf620ecd0e3cf87e61f5cbc9fd6b" },
                { "te", "2e7fcf381d2915494181b4d221a3c5bc384dc0925ec3b2f3583bd5216aeec1bb7d81932a37429487015ea21ec36137edd06ee4136cb482471c46960b9cb49417" },
                { "tg", "158942af2765810e74f23e5438a993c6c1171eeda42c0f533c01583dd465d2ccc7017f5e7bd2a6bb8e22029b6dea4ae5f1bf613ef976ac1c7700715ce508244d" },
                { "th", "b332fd61d1f095b950e9b313dea47f0c86e326808fbfe9bc9e28465c8c2bbf8d099ccb4ae955778e67288e713ea7e4193158c92d35e06133510af222237dcd5c" },
                { "tl", "39f6ab9548ce3061b25f4825d107a51e20a1190e8f93eba1151b8cecd7cbdf195652a9ad8132e5ee581760e2467e82f85c53e6072b2d94d33c681678cd35adb7" },
                { "tr", "b5400ec1e8d6f3f77a3c9c5d34f01116f6850f335c4794dae1610d5840c456ac6fa3e6920633a1ce321d7db7224de7ee21e413abaa9feeb02c0de947a4ea2a43" },
                { "trs", "28226bf568ad65a55bf85e4f701b61dcd7296d45a6dc4c0d55f7e197f5a8b5db2124bc2817fc698ed65ddf867dbd99b8101cb8718ea8dbf66ea3e94925ca05be" },
                { "uk", "ca5b02b803486e7fcc28ac9d36f041278f2a70e4a5c447baa414154693cdc6190b13dce21f99643df09148cbec20ba2e0ed43bef93606c99f9bed38cb6b92f3e" },
                { "ur", "ff97620332d357f5b6be8a23758d2a4c65c2f21f5814b0373f54f03ccc3bcebc3c181e1e05b3020dd70c1a2e33f313e0137278df1e9264342a8fe1240ef706ff" },
                { "uz", "a94c92c10bec83f4bb477fb2a5ff316155e1133a7d14631880bc3dc8b976ca1715c1f3023f4d496ded48e4b54d0829c2a013230227aec79e86edc9a6b6c9db0d" },
                { "vi", "69ca182723b0a649f1824f8cfe75b806509a0fe851a0d44fcb8f5d1952ce0c04ef5302a40fdd8415428b45809aed631fc2456f431228cc13056dd081366cdad3" },
                { "xh", "73c504f52a070cabe8df07e2b2390a218e033411a13214daee2785b8fca2d932c91ed5260209f3e488f0f2188924fdd48823df38c60348bffd444660829a808e" },
                { "zh-CN", "9494073cfdbe274f1912b246f26d831ec35b5e60a047e73f6da711baf3212087d4fa01aa14d120301ec250f8de564341f90e48c7422ef3f12ae96c21cf66d81c" },
                { "zh-TW", "076fd3e719835e030782c5f80b95867a5bfe14faedb706bd2ab5057173577a5e6ba06bb7ba7dfb14e505649ae7d6c2389752f829d425d4664857fe76987c1b37" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/133.0.3/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "69395a9a29eabedf9da953d7d8d7acb98e3e1ea0c09db81eaa5205f11cd01aaf284655e9750484afc31411c95d3773a1f88f89e7c28271b331fe6088cf260182" },
                { "af", "5a6deb66528410be8e2c29aa35d6a4a915bca33e627645bb8c4bad0f3d586a17a277079635d52bd11d6785decbf5fd715aff84d7555005321f0237f4ee91ad87" },
                { "an", "f2716b43fcc8a175a14af0fdbe8d1ea29ff886af1cea6d0a6c005e1fe63b78683e28bcd73e7c19d13663c1cff72e0491fd188e5f898f82401c22d95b2564296e" },
                { "ar", "75c67473f56f7c940873ccafe5ec5a57d55ff592399085ad9578a6033a6413f2d074eacc9e0e5e9ef7514251b36d3d255ba54db2be1cfac6faff834519f4369e" },
                { "ast", "25cc866bbff734fdc4569fcdb81c2ca3257b6711dd23bccfb94c14ec1611c9736e89cda8f63fa082febf6d3ef502582f2977b0df038ef630e0706728b0edf0b0" },
                { "az", "6a8dc607126c79f41abb9f0585f28a8b88df47b52255944899a956a7f3e31edd6a0a8338621161ecc179a78835b11ea592c95719b234c0cd6bc61a76d3f7cc62" },
                { "be", "e965835c6e372ba01c2270d493c97da84554bbed42a509840109df3279f2823a6e5ba68cbeb43d6b13a03268cde105ee938221b4a8c5cc923d6f0efa91dd8fd5" },
                { "bg", "a7ace98d8f13f4e5f2a2cc9c49437e4928179fffe40270f59485cea67db4950ea0ab8451b2a10d8bba46c24150bb688ac8b935ceae2f0214147fc60b8220cde6" },
                { "bn", "5831a14875fa2759bdce533924f73de256a29e5d5968c7ec12fd97251137e0607b9c5643e102418107fd6ab74c04a523858e219b57d5b329def5991d7d0472f6" },
                { "br", "9cee986b3adc69c357fe82f5e9229d12bab50580e307f1743d85c5e8db4cee2551bd06271eaefdd40106ee690c8eb12adb844ff3f2146642655556b612234014" },
                { "bs", "41647f3406c0aa6134de8b1dd610f33249ade7aec061066c5935713ada89531d35b6a59bda86a3ee8f4e4333abecfa6bb8301ef1c71c4298756ee97d2d5e21e7" },
                { "ca", "67e93c81c9fcea1752e13802a8a3d1c399ab33fd72e825b7ab8055033c760b65b60c7573a804277302cb679ecb2e8a3fecb3862bf8348f50d6410c0d503269fc" },
                { "cak", "6e7084b147586ce278491897c0937481d72426648103f5dae9423a0dbc43e5e2a5a32bc09e1d810336d28b1cbaddc75846847fbf59255fc2cc81dcb5c3fa0ea5" },
                { "cs", "73a9d4c611ca830853f39a2f93017d44dbdfe127cb1d687a1647328450ad8788b70cd1e4512c2eeb1ea3dead0d6a010cf020cdc7701f4685a862800d39bef5b1" },
                { "cy", "b96a7924ccf6f80107707e569d29f579e816b095f5df3ca90b052f7ad2cb119a26cda927173805cb97b742c75fa35aad60b12e5e56aaa92c9df7b9587be22bb7" },
                { "da", "97ead531bb46dd51b33f04e65160ccc802127fb7b7434315b18c5dd1a983f7b6cbd625a274a3085cdd702d5b67e749faa931df762eebd9036fa627c0438b019f" },
                { "de", "1881988f91c6b0d96d4a846c6a2249313da9ac1c39f14ed5094304dc6beed512055aeca298915c58e178256f627e9c5d4380f28de3a31aba48db7f6f0a2594c4" },
                { "dsb", "9c7612b3dfda59d139c9cac7d9ff008bda3f8953458e67552abfcd52a28cd95f34462ce522492d4ed80156182c826a8cdbe9760108ae21c765fad17aa1a1afbe" },
                { "el", "20c7faffbec3676d27b91e03e1d638efdcc714f8ae63a62abba01e21040ae875abcebc664de4c3e5fcfdf67c8113016b9456caf8dbbde2ac9f7d413a86562c5b" },
                { "en-CA", "20c0abd88a7d537e346dd9ffe0cee40d4a686b2816de73e0b651d25ad05cc520b7406694b8be9aad65b115b5d554ce7483b3138a5ebdecc5fc59fd7f8095ee2b" },
                { "en-GB", "bae7672ec85ed943bdbc2026deccac84ff91973bd9fc825bfe484c9d69d831fdf11b1c8113ce9b74de24da8705e068c316e9c29600caf767610408336cfd1f29" },
                { "en-US", "2d6e9a11635657a29ed5e7f9ed8c3145f05574a7d07a2f1449ac2a66f4dc36ea1e18ac3a3c4435bc91356bf0dee21c3dd75fc431ac43f316ee412659fac815bb" },
                { "eo", "337abcd4add63cd6ccff7312f7972b05359aba76d0aa08382515b619e389c41f2d065a48ed0eb3235bc8428a31d6fe0b05a50dc451d5abbd7bd6d0554954b039" },
                { "es-AR", "1cff3321da885b852d2bb25d274cf98552229a579f7b59f05a087ca164f0e2c00a7e87732e209cb4e8604fe869f6537cf4fd18ee6dd4af6a7fa133fc6c853753" },
                { "es-CL", "89d99f2876d7f98067125a69e3d94675ffac2b8395cc550e66c2f18927d64ab095b5de3cc82dcf06532b210a3b2fabf3559c6b4dbba5e85db3239df0a3e656d3" },
                { "es-ES", "9f8e7e4e628f1c2d370ea6cfcf990fc58cde4d5c33b3e7b9596d44423802553241cad560005b96937596cc406a893ea0e29247ef3a6e6cb916db79a25729309e" },
                { "es-MX", "d1370c2014a3248ff1446e05a369c270e7fbb018456f107160a6e40b01f02c7f593fb682a38093e55bf1b8713046803b2882fb21f4d7ccab71b5d63f00790efb" },
                { "et", "acfc6252b53b14ef11073aeafdbee877df5060ede271e375ba7340d76e536eaf16dae43d90de511a6f0e6a26803aa55b9e524d7c771a5fd4cbef771614d5b097" },
                { "eu", "3dd19514b0e2ed036fab843b7e5429714b773e63f8e21769c6f4a217c3723b79ff2151aad70082d632efc90c9f796ea8dbe0b0142d6686a444b7fabbd9ec4ac0" },
                { "fa", "dfcdfd3cedb56c635c6088d13abb771317ef59ae08f634cb4d89791b3c0257fffb6fbbae74547fd110cdb55bbc7457db26b741d810d38deda9369ad122e17ff3" },
                { "ff", "fbbdd5773cd447e1bd21c0280981f4b7a7484433a6f8fe5020598cbf6ede71ec4edacb903cf1a1aa950e6f5c41aa5d4612bcaed156aeccd43a1980570a265e8c" },
                { "fi", "e85493cb77b8cc649c7bb5b4f0c717c8c90f77c96c09ce899152e9fd9edd5cda5b3932bb51a0d0d1951268a144733d5e090c52040f306af3a7357cb8b34dc838" },
                { "fr", "2618febe577fce789e47a2a78f173bbcc1507792791406886f9ee0718495fc3bdf5b6462df01051c4ddcda053e0ec443660757f9c26c52f64a5708c51e97a71a" },
                { "fur", "2f57e87f45a08df051536fefa2273c244d3eb1d17bfbd9cb0d1bb17e590e737956d1d838d2338963cbb4c18e5d9d40b0477ec576346d411807c2787cb5169f81" },
                { "fy-NL", "faececd79cfa31bf2ed379a96c84bc63926a74d10ff1d0ea2a8db533d860b1d153d967a28ff8ddd0b50fd7335d3d016e9511303aaa16675c9a2434ce812056b0" },
                { "ga-IE", "e79c561be2a489598d691c525cc390709599f39fee34355a40e0766f76585e7fc7563a5d45cb47765fd6a453a1429e8fe091819102a530f20df8400e2a484121" },
                { "gd", "f6fe2f67561c65e443df33cf3852dbb09ebf5dd09318f688bb37278006e2260062617fa74168dfff0e0d96ee195ae536a7007ad909fcf51e0e1cb50602c56cfb" },
                { "gl", "93908bee73b845947ba2c1b7e2465f24311b8899d8f10087baa6de261b6a6f44bea365c9aee767753ae5db7768fbedd5668832777d9c16d82bcdd9cf9fb896f3" },
                { "gn", "f9de854762ec251988db21c0436fef538a4c35f8d78b8b7f9ce265b680cffbafe34e47f1b9ef09fcbc82618604a2376283a4ed1b2fe1017940342feb78d7f36e" },
                { "gu-IN", "cd96c9e47d95ba5437f5c3717c812faaeef8728ad8f5008d329bc3e96623197e51196db304ecdcb4574bfeac791bcc78a00f7adfcd299e489881889e9fa2cb04" },
                { "he", "447030cf3042132d44f4b70197aed21d6d2b6a65bf7787e1dca15f7708ec2dde50c986f660d2ca91a344a240d4bd47b7ab6b731493ec02acc5b49c85f933c9e4" },
                { "hi-IN", "cf707e79d60a1f26eb5952c6d9c69beee5e5fbba2d7d2fee9acc90d19b0a4effaafacbb34d40411b2043fe1b5ed5cfa758ab299e56db82442719ef105d0c6f99" },
                { "hr", "3cadd7179ef15e8b544e086f258c6c5492a0cde1183d81dfa3e8657fbceaed50953c1f9db1ebeaa97ad95575e1635eaf9f255c3c830508361ea01577f3d24d62" },
                { "hsb", "8db98dcf0ba458e87f59723f2e7b4002f99b4edf7fa49e5d0eb71762a651e26cf21a8f5c37e777e82e2262350dd0fa76a49707c88d021957319343d67d556b14" },
                { "hu", "5d71da9f0cd9aab6b6aa37412a2bf0c7dcf6f051c91828b5761d2a7e961b570b075f8a27540d518fcf254c603800038dd4587ef65b2ce4c271621e7fd53e80e1" },
                { "hy-AM", "52fb949d46d036d620ff8300cb42b2c78fe65db2b8864dd4f86e914dfef7f534e23c40dfb38bac37cddf5157fbe4f63664e20cb6febdb2627ad26c0bb22c3286" },
                { "ia", "988640d24d257671da4094103cc87bbcc91f4d97531951b8006642fd65e3e5ddee5553f13fdd44eb09af266441b6bb73e2723f5425f89790276eb0fd45e776a0" },
                { "id", "078ea0484bde5b95aeb3ad6bf4c1894c48146640bfdc2ac1468c2437ad6968bf79fac206d2051e3fda6eda70ba5947912e8c0de631530fd750424453d9b95d70" },
                { "is", "7c22765bc2ba42c0ac5bee7be8e1732889b9d7e7d0c15f730d5ece96bb2c8a4b541736b324baaeea5b9dbeb7a8fcac57b9b222c69a199e26599ea677c6619aa1" },
                { "it", "3e747b5b749db66b9f94e0497da79ba53a3c4435e3539af2ca96be89233ad19c22816886f40662a8e8fa22d5113fa962ef36cf7730d444ea37ffa8ac5d89d36e" },
                { "ja", "b290941a6b3985d2f6a2ea74b17e2c20df1744b74d02e77b1d7d0630752882852e65d4272624a5517d61ab0bd500a5bab6f8b420a66bf139c925a6872e8c0222" },
                { "ka", "20208a6def1b306b60d3bc0b0c9690135a24eb34010e402b9625b3f1a02287ae529e9fba4dcbcb7206a996b80597662985925e75c3d82ed84761d8782f301c46" },
                { "kab", "447a59a0683ca49f7d5490cb578121a0e58efaa2d0da2e5c4578fa5a90e80d6d8caf1799afaa6b45c657c4867de241c4304b153fc9bec7ec043e70f28775abce" },
                { "kk", "0deff9fa362d7a94ad71c6d18398ace876da452cfd7b5d1cddd9cd88e0c833a55251b4f75ea23f77f64cd5215c5785bba80a214105216108e791ff7b413c3b43" },
                { "km", "3855f4711a8f86d7b918a330a17f2f96819a1d6d4598c6fef8b42bb332fd5158ded0cf1ee808354490796e4f7ff23d10882ba6dddaca662cca095734e2cd2447" },
                { "kn", "7072f7dc6a77de6b305720c761b83a7391c371b1be8e142e2510cd607ce19ca4262f009754d0934af9f69e2e34a1858cd03fadb7e77e05311570daf2b6b6764a" },
                { "ko", "b770bbf712689184b014daa5bb2bfb18e09f14a291db0eacdb0eba9e7b5cb012b7b9849313b8226c1f4b8cc601541eb045e1ba5ad9fcffab29f8b3f879439725" },
                { "lij", "bdfd63b64c70030c1fdd34ebb510e5c15d2ba3990b9d40c0305a90ac4e3c579ed8866c9eee359b30294e6823327dc0268bc74f1722ffdc014d5ee88c363e7c86" },
                { "lt", "66463d4cc6a1edea9f392d94baee550d977eb46e78921c941a77c1c388a531e34054bc27cd7f0a09c0d519749bd7bc7f3d66f4473c4065d029c0d0fd77df76ba" },
                { "lv", "3d31b84712470394388303777a8321b3246515573087f3bfb1e14d0f503464ac4b00361723ce59ed8ca6fb67f6b8ad88803701829aee6bdadc708693e3bbbda5" },
                { "mk", "8145f540f851b37fad258de439d1c32c3b883f11bce21b1a07aff32b1813aa806f94fa10c88c878221b466356e06a4b11373fe38e813edf2cb886b0a9f5dfb2b" },
                { "mr", "252ec5fea61eed56128579e420db5545fabb967900af2294e266a2a673055e8950fddcde9ac106df93405203f33bd6c2ba18104385df6086bca93acce2fb433e" },
                { "ms", "d1078a4f9293579c67f19a9c51d024159f4b35512df55221a2983ed952b071e33bee6395632887acb4e54939378b4081ed712e07495f105bcbebe834ce4e58b2" },
                { "my", "15acecb0681c28bf448a09d7a8b83aa12d985cfde996a9138e23dd3403c5b7699116a28131df4de22f2927929171a8a05a2817e88e5305df37035bf64f626840" },
                { "nb-NO", "20c9fd12f0416deaa08ca6e638dc0d38d7c892c6a9477d2e13da0c8cfcc266c6138824617b9015d42d861f50a507bea6a0aa13916b03420e28cca6a123d375c3" },
                { "ne-NP", "858f5399f7a7f95becf180bc83d76157bdb1c4d4c0e56894085f3e29bc39fa483724678082f357a9f15b1cd5329b68b976ea4e6eb2250575703f0536bb371700" },
                { "nl", "bc6fc5acf17308840a11f991d37d2e0b9d602946e1ca6426541d93143c06c30a21c27b0c90f1f07926d26c210818ff2dc5c18e28cfea0f9e33edf9bc99529016" },
                { "nn-NO", "786b542a0f9def6731e1caa74407c4c674d758dc74cc38f45246780de697e41c5c3f10540044701e88886957583106bce1d0820533198bca7450717f2f709695" },
                { "oc", "f9e5426d4a541e7e6d70e6f838d88cdf72d128c058bf9cc3c60d870cec253d25b6cf8c057395e609aea55226b7b878a841e52e6a4a5860a167e059d18907e4da" },
                { "pa-IN", "6393b0c60a101c81ec96f35372b130008397cf5a0b14537d40f401cf261e373e73fc239884c99bb05edc5e1864177f0ed8160315fc204e3d344fcf27a65a0697" },
                { "pl", "e7a7c1d5a15073420fbf22209341c45041ebea2de1ab11119468561c5c5f64361815454ad879a2a08043a48746d0d7a17e7da30cf64bfee009084dd085ad7f1f" },
                { "pt-BR", "2e7b6c5e67c0d4663189dbca492f5c34d8563ecd10aeff729fd9a91b61d629599866f420b3ccecdd440fdbea3a11c41deb7d1d08420e712ce187935fb65781c9" },
                { "pt-PT", "cd0b192fd3fd8b2a75c8e8e22cab96d2a9afbe14f1b7d5de2005697f8325703a32caed7cd39c3124ecb93f0789273125e1ebf134bcb4d18791694f345da0dfc0" },
                { "rm", "b4e2413b93f8cb00f038c07279117c093d323ec7ed964d9eebe29654006553880f843f7b4d4e9850010e5b685f8c58d0b7493a55641a593c2e3cedc143c5369f" },
                { "ro", "86520be8c2c8755ced074edbafa72eccf4ccaf907e378cc8ead4c23a52e1cafb8f2b27ac7cd63518c8fb99a0a54553e7cd8cde1d4e7c65fd4d77a34a13bafb16" },
                { "ru", "6b885479a55834641e2db9fe08bea23b002bf23fb0923861dbdde9c8c6e93f6aea524794a298dac0de26c934671a6787114482fd719c108309c4a22af369dc12" },
                { "sat", "40a96879f82d1fdfbe94fd590e94e0cbc163e54c6f0843da9f3dae01fc3026512937d8ce6da3fd22076421aeabe48c3fd9d1181add0c0450436959781fd7159e" },
                { "sc", "831f37f510d259ab76d637b530be8d5d7732d9891e32adeb0830f901494f744e260f9ff18c27a9f1c7272f4123f5fa4051953e36d6656b0d8ae27a2298a96cf8" },
                { "sco", "d9afa0ecb2a22998525e039d0c04dfeb18a1ca03b9c3edcb980ff8c555297b1b74bc4937f4cb867b519b1cdf93427f8f9e4c8db0df190c2531d73f53824da2ac" },
                { "si", "e5fdd3e7439b7a21f56a5368ef8b0cddf856fbb0451453f89842f1110ae53925e6a6a2e274d9ccd8a7eb01f7ae550b5024e19638b06392cffc5d7f23dc148d72" },
                { "sk", "39f19ccb3ec1d21d2c6f01392b6ed34fcda58c5e5826ea4d54a622fe2d318c2c725f9db95a236bb09fd8cfd3c2c982cb2fbf5650fdae806771d04eb6b8ca5418" },
                { "skr", "490956afa76622b49ec47cbab7e70d41c46cd92407f38cfd43cbf39398a34fb47ab065cb9f64f0fbacfa0b8da53cbd04050884f6f2c17897235c4a2995050289" },
                { "sl", "66cc1dd8aa7d40326d74ba94efbeb059f8cc5039bd3fe65a1178142bf43c5781ac5fcbb9d00b14a1a0a044e8ecb13140cfbb6c0783935aec70efa1078375ba10" },
                { "son", "9388a865c09197863b8ae1390d6912631b287dd6b107a19835afab3b3ccff737085c130e40fe1c2a2c314ef786b1c450ed48e8d5c849a382c1913f4ac890c0e9" },
                { "sq", "f49319c70d2382ffce9238e49813e60e7924da53f57aa942a7212969ae671df20cd7e5ddc0122694801682c3ec9abf9f412988fb4ef72101de8432065e88f53f" },
                { "sr", "efbeba40202b77e35d374dd1ecaa77c3fc50e932d5035755a9bd3462e1447ece2a559b3cb160b66e07b87727af72cc74306bdcc6049b83718b247d17a234f3b3" },
                { "sv-SE", "7dc57aa2ed3a4f5430c0d46694448a844f08e9d2c7842ea7cd407fe4dd4edea4f8de4cbadf3b98cb0883460e0ad5651f67e6d110eb5f701beadd40fe3dfea852" },
                { "szl", "7c563c96111a48aabc701a4ade76415c831cc2574fd20b5c38c2e64a8da1fcd65f58ad3174ac43605f07095f7693d62fda30ac700d13c87221244df1ec3ea4ec" },
                { "ta", "9e8de34c826758c6718fe606312f0f2f375931aec1f526d2804ae378eb88e1080389deb7bcb350802717b2ebad4314c93dd3d656901680bb651f6587e4727d2d" },
                { "te", "e810bcf3f30992339c301225aa08b659e929fccc51773b039272ee2a8a2c847652b698cb7c78ede73b5656919e07e5ed7c71a2f347dc535b25b95a41dd4f9188" },
                { "tg", "68c3cd7e4a5bcd76bacbdade55c46820988b345be6583218b47189530dbdbc82495a368efdc987f36c24e1950fc5a87f3263c9c6ddb4dceebfc622755a51e671" },
                { "th", "78cb99e41f49fc6d0acea4906331ce9feaeff8891a8be71eb70feaa8c45a78c7089307683c826cb090cdde4b0189821415e06cabf1fe9caf956a1174cc054b6b" },
                { "tl", "3d805f2bb9c78fb3b988778d31f58bc6ec91e559591909cd0992a65b81990e04c00038d3bf9d2aabedbeef7f706314f9fb21f6dfdcf3b6f26db40ed7e97b1464" },
                { "tr", "9b1cc5ce49128c294875068d1d735ad5e3cb527f01346700e874aa35fa92b6e8912d7822b97ef281182d3df10d000d0ee011216b3c7886306ecd52abf682d2c5" },
                { "trs", "e89b3234ca3fd48a253bbf7a0e57d944019e1a15ca9672d7f3d83b91edf25f44a8a81eee22d6f7a072189b2eee1186496b290b518f2536860ae5f80690e6f86e" },
                { "uk", "f18d90a3597a0f8b18f56d83e7e7449adf6b421ef9cf40c033434d49c49671961eeb1f68abe72ca3a6b0e83fff1f0cf54e5b4989735589b0011f3c089ca8166f" },
                { "ur", "3d8c5762adb3fed872f61f2e7c3e286315d1a6b42bc51710db23be7a0106426dfc3646431ec3503747799183d071a6d53d5f14bcbd68798aab9dcda43d1fcc87" },
                { "uz", "f4ae0748e7f0a2422cce87f9e6440263431e94dc0cb1267d8ca8524e30a5ba9e12dce9945147772fd2445677d1e72aeba68deb0738dba7adbd05c126445e5e02" },
                { "vi", "b848c29a74710857046ae9234f65d3c9b5f2dfb18cbe482e1ef978a9639465e3d1d995c3bc0cdb4eebdeb65df01c1ef98d4e186c303e3e2b75bfd47ad3662b5c" },
                { "xh", "42f96556c3e12fd2ea6a2b59cee7623a31279135d7eda37b1c94ff47d4409921fe82371d8f9df8c1e8709842580fb5698b485a87b305b633bcea114c1f574518" },
                { "zh-CN", "1bb219fa27e6eb7d01d4877967256133aa050640278a3ac5aae9cb04f15b929dbf79dd1ee1d715021113d15fef75d43f037337da31b63c95c9923e486e1a1c67" },
                { "zh-TW", "caa0b6a8641035514722695424f86d375ca35d21565acd9f6d389675627addcdb8376040e78bd99bfd8b5ff8acdc4cebc4c1dd7eca332d1eabb4c052e8b4d34f" }
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
            const string knownVersion = "133.0.3";
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
