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
        private const string currentVersion = "106.0b3";

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
            if (!validCodes.Contains<string>(languageCode))
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
            // https://ftp.mozilla.org/pub/devedition/releases/106.0b3/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "1e0a1b578b502dc5bb3da11aa6239f0609e2b959e7b993d51a01500cca4dd9c6535b991ae4a78acea6f3629d35f37c790076d7b66a43e257763144d14b9f1995" },
                { "af", "163265215423c7a04ea9ed2ca65889f4e246ba9b5c89c92917608ba45ed02ac94182111cff0f56f9a345a79acadd19542217eecebef8d5c91936379fa0f98b1d" },
                { "an", "ab2135702a75d8b7d416b753a10b9c6db0c6433b626d111f8521d57c78ff3d9560af3af46fae27a130022ad0e6169a3ad51a3511b3b3090b4cd1b2b2dd1e98c8" },
                { "ar", "006954e0a9d9de61fd30108be063fdd284228bcc17545dfc3ca941f8835b81e6b8b49f83701d8758bf09bf27f02d96a8d8c2a058f7613389e085638812a5de18" },
                { "ast", "45cbd9dcadf425cafcc9477231d210caad6259ef3d367fc53496de67c796cc118a7b24bb2d96c4ce39732ef619ce01ced082e51ba5ae32be9ba157ea591ed160" },
                { "az", "921eae228435a61242389ea46ddc681cb77e60737491386cd1fd2de174a19a48fabd498c396de3aa0fe1417d8b73a2acbae687f802604afe0e5a0a4717c32815" },
                { "be", "4b4474726ee5f9a28a468525439d15b134f0c7af4395781cb2f9f765a04aa309030aefbb3381201d2824fbc622c238e32087aa67032332634cd243fbd7b0a4d7" },
                { "bg", "648a84c5340bacbd54b52e182eb0da26dd90e06928509d9080cb84c80e1b2851be69b12c9cb788dd6da679187ac961978d8eb4931d2832decfbafd7d08b0f314" },
                { "bn", "adbf87e4f57c4ef0c4242e39827ef90fab21cdd5aa1360c30c8223fe7cb2c0caca585dfa675fb694430825ef010ca0125b02be3b078d8b059d6e923de5af8330" },
                { "br", "600b0ae87dcfab346c43f259883ee8a95a05a3806689a19a6ca49e25df15b50499edacd4347c0f7c1c67d63eceb1caf303270f50bd87aa057fc281b31eefc5a4" },
                { "bs", "b8fca96255741078d42a199c514ef7df68bb979b607d7a5fb8d78f624a17ac45a991dd22022c4f21b077181f0501093007b1dc523e5f234959582e1e88c0d3f9" },
                { "ca", "3167e4333d94e9bd8967e7e40abec381cc8dd20de5f4cc7f04badf4363561a67f3004b03ed6f50ccaca2cf6a248697390d599114aa14c3464542716128740f8b" },
                { "cak", "1eea58d073b16e7db4d517bebd19925aa84cbe49f5f412a23f9eaaba6cfbac9562a2e368e71501957204d0db6ed498d6ca28238eac8d91c980112ad2bf69263f" },
                { "cs", "0fd96fbe4d3d3a02c0b2479c5fce35afebadc4d8d11f448b32066fb12cbe7e2018c9f66e282dd29ab348f758ae72c1048ee7f5d4e2fdc04904faf0d04789d187" },
                { "cy", "fdd6dc194d62f3a3dadb91b45eedf83fe16a2feb353350356401ee21695bb2e681833cf5f4d8bc48e26183506a420aa9df94cb99d2090ebc5322d67208046b4e" },
                { "da", "eb4ec48f6971c5735f986b9bb438773d8143d502994159437203a47708bba6183a29abf4ad00beb41fc8fa1376321d1924c914570abd73f13188fe890977e7a4" },
                { "de", "a91c34f30ea46fc50156fcb8189793e0e7dfcee46911af2b5b82aaae7aded5d68c82ec2ebd4af24069ae67a568d6aef4f6ff9effdace35f64c7b41b2e1fdd284" },
                { "dsb", "f9c406c3784ab781123e37dcfbdf36db50e172135bc19ef41c0b9edeae9c8abbd455902b60cd509da337bddbacc2cac7d182cb9937bc39eab707aff1534ff7ef" },
                { "el", "b2ddb8d74f7df41553c71e5052afa5de4fee4d070610761e142a28e06a19f6e3edbcc1effc0018e94b8dea22104c10e71650757302d01b9cb3888ca476ddb7a4" },
                { "en-CA", "ceb968adbd87de3ce2cea1d6185b98e3f55225ea63553c1c5c2a0a4fb5d631d89f13d880914d8c4fd8763908cb10721c29783d892d7d1e5046164953fd6b85a0" },
                { "en-GB", "763b59c42a51317f49b3d3070a16323876af84d095bbfad66a699ca913efac682942126fdb9a118a22774fd5eb1b8b57e7ad2f0fd225b805615b2770abdfb259" },
                { "en-US", "5eb67ff4b7ec77cfb1356f38a6888c0329fa6509f372980a53d11bc2ce6189ed0186256ede6e5bf3598fe4b868d8aa23368b2e7e2e773cbc6d8ddb3181191f23" },
                { "eo", "df73ecbf85513876b3111bed9c9089fb7ede2e0facb696fb90cbfc34144b407be7b9c6082ea613be9044c6451b290b28296daec02f99693a30cb782fc328c7e5" },
                { "es-AR", "11ce01a7b6c532943e37dd245535ae47ee5a06feab7453ffb64a80eb5f5e83d565f5d1d0aa58ce55150935202a701c6d18b4da003156d9ab76584296bdecbb43" },
                { "es-CL", "c5db9f66eabd48d8e786309ca38b37e12f5f2fb36fdb16c08c0324c43dd1993307dd760ed972e48cd42563828d2d9b0a56db2ffef240054127ecb8a35a71c970" },
                { "es-ES", "c9550389e105ddf10406505521e8c3db2735f6ec06d435d84a9121db0ac7a262b81bffd14f5483d0b99922bf18a9cfee3fd84709c070f8c919faf86836af8a7f" },
                { "es-MX", "71a8f4d35595f7f65667c94b827d7d9145078762532ecb21c6ef6b76168822a9a4d3549d40f4e9cf098797fd74a2a0128fc28ab356eae9033a53526633dc751d" },
                { "et", "a43e405e1912b964f7c951037cc918b91ee220c86f1fd18b2087f9b8d45aac070ab3a9b1ccec16b18eeafc26ffce1300e58e93eb495fc2ed08d5345470fc2f26" },
                { "eu", "d6707db02fa6c93c9a1748ce3c9a3b5a313c0c455086d5e5c10fa238e5b8782938975e786807c9bcaff4effe06bf9dcb53c75975c7d16b9ea0615ec0b5cf260f" },
                { "fa", "4609474cf0ebb692d214c5435458373da622317cbc77fe0bac79427962f8df0d49575f860e5bfadce831e0c8c81d4d9593b90997b8e90275a9cbb1952ce948b2" },
                { "ff", "17807241ed85a69e84cfc71a6d603343af9a6df9e9828ed8aa6f404865ddc55620be875051acf2d0ede1565a848fc6685b173f93d92b075986dbc82a966a9ebd" },
                { "fi", "82fb7bbdd3e76fecce380ce5e6e2284f14060db1938d9d20e8e450263bbaa84d50bf553f6b540119a4886a16f9e8a7252366f451e2dae1f38a4402592e865d09" },
                { "fr", "3a00296fd31a4c44ce3b684acb2f15e5555a6f8ca59a612d75c8509e56cbb086e75306a4af521447f4a32f6f9e4ab46a38d1b0eded89f679d067f5d04f7a46d4" },
                { "fy-NL", "fc1909039bd49c7441034956a6629396e879c3ade97bb085824b875db1cf849ecf41edf9d24dcec13626188d199d22a75e8ad041762b3d320968dc08c9e52141" },
                { "ga-IE", "9a61e628f9f0696bc3942b470eb54a257f60330cb996c96243468ceecaa90c4bb89cfe36910b0fec3fd9a30b59f2a2801f6f1f43663155a812d4a1862b183816" },
                { "gd", "f8b6168dc6fce66874474fc47c04e7bda0223a70b3d4a7f9435eb17a47175c47b2f4f923f56479a8262df1afdd1442e8cbf68467f0b01e785848d7780cfc4938" },
                { "gl", "0fe560a1087e4baf57c1652791b6ab6f71cf0d9d47490951c035242831d97c34b0970787d61077d4e5ad2591cba3eefe30c77d1096d6668ca253e69cfaf20540" },
                { "gn", "f1e410abd0ee526baf823f5d8ab2ba49b1af0acf850355b48f4e536e6545b441027b0d820675f1a0db0a07cef2419f9b0a6577b0646d6244479628508f0ed22e" },
                { "gu-IN", "2c2043c6ef2bbeaa3f4162734c67c05a9d5e6e0331555f146af60e401c67ef88d4b9a5a24124db3b1f112aff9da0b5427166988cf9f70c45406b8dde17db890b" },
                { "he", "a899bb93d3d07dac40e2ba67190d3d90dc5bb499ec4b9e9f6e0c11d67a92ce7a35d7cedbdbcf805e1d67b0971db48c212478e2da4a7409a307e8eff71ae77241" },
                { "hi-IN", "54657890da5d66fd45d9b4931e4f57a5d3c1c43d47adc047b7366d12bcfea8ce52f0c0c09250f3f5f74850c579b63feb52089f3d0c6bd90cdde272d83e2155e7" },
                { "hr", "6e93b9a88b32f5c12fa5ae786e510c9d79e3d4e81e408e91ff80171cfcfcea861a3cd1f2f0c5bcde02c592b7d71454d5b3f2d7d1b4d7712fb29be73f7f928df9" },
                { "hsb", "b1d993edf8b950c0534115d3c00aa7b27ff6aaf60d16eaa23206a9c6e4d7cde067c0ca6c38820a8d72254c0e12f15afe8cd176b6ef5764c347c98bc1bab2a6c8" },
                { "hu", "2a58aae6071a4061a61676dd7bf3b69de7ba8bc1ae6c55f4236ab73f24fec91445cedf27fcf949474f9ec27c7bc04eeee78b2f5f35f8da6df31d21bdc3d22da7" },
                { "hy-AM", "816a8b23f4a2b6362acec8df8223ac767fba5837ac29ecce3bc89e77e0ee3400e095bb45c7690eb287adba4ee1f55640969bc418b91c7ffdc18c3241da8ee98d" },
                { "ia", "9b266af13c0198b3dd4e421c6150830d9f982e949c5f85eecd969040183482248fac1d5dbad237ff6769eb420839803947a9e2580ef524a7310c419eaad6af18" },
                { "id", "4c66e9c8c838076b8d77e15574d03dd5a2fc07cf5e9aec7aab97f9010a0ea69f50f23dfb59ee3e9460c019dbfa395390df8ef357383b1702076a164bc3619ae8" },
                { "is", "41e38b0464848c6078ed5803d41d58e6e5d6ac8068722edbf58f87c0b8527193fbdbf8e6c7405406e0b797ebb93b221b84fa3fd472b778e61c0a1934c71d025b" },
                { "it", "0719b8073c3ff591df291442fd586d8e616db58b6970f0c722c5130ebcfa69de7210ceeae6f923dee6a62695808095a3d0ac2a0641c8524ab27d493774737735" },
                { "ja", "9fde70bdf8a90e7de3a9bf09dbc02f5b6052f9e5e50c25c3b2bc09a4dc02ce1fb6e7410ae993fb3214c5c4d5449a8073052b2acd25240d07181a4b56d40713e4" },
                { "ka", "518c606cc1ca7787f3f3188e6a4d791f5f5745db68f92eb418e068a65fc8e28382f48dacea58b3f3cde6291addd396689dab747427e0e1530a367cee92e787e8" },
                { "kab", "789403f792d61bb3a605775c66c9ae79eee6027d532458e38ad1eeafd7493c79ab242f411d6cbcd50ef30288b68010a33aa9b7d6cf42f9283b67db2b05a206d1" },
                { "kk", "e4e5089b04124b94dd383d60aa54865b423aea5934e67b6e2fe6065a4ff7e20d98d99d48b29c87841adeb54ed441c08bf71fba7fde39780ce222de137cae4f01" },
                { "km", "528d1c333681fbb879d1839173340b09a6509961fa6b47b6854ae86b64a1cb05802fa243a85992cb1c3bade49fcc5001b3b03522fba134250ba09b613dc1dc92" },
                { "kn", "e4283b010958a857d7352162414150cbedceb3ebd87a5288591c9f696239c5cb954482cfe07cf1a4b931b0afe7200b798a128df1bc3143bf6f33bd1146ace98a" },
                { "ko", "f15145e236340571450775e8affae26c83e568ffcf613dcd3100fb992b72c324d7d12a321763433482bdfe9b726b8bfefe3d0c7bc281247e0720dc0fd6d8b264" },
                { "lij", "e9f336805a479ae447863f451bb33c2b93598635b4788e995bc9165d85ab87979408ae52344a5ff2d1c72ee9cfee85dea2644d0dd7b6293d9e12f63fcc7995a6" },
                { "lt", "a54312f5e793b34c4a1d232b6d5a27955dfeef8ee59ddcb2545451471311c4458f11ed01ec102255cba2d560c5c1d7c3dedcfef0764e45525d2db3d43c5893cf" },
                { "lv", "ae693a92033535e255802be0ebc7d8f7269b0eb3c0385b5dc4a3a9b3b28bb7fc2254b6fad3723b72669835c72c0b53441b75ed1c6460f06dfca7273376d8497c" },
                { "mk", "6d21aa7046803bfa836082644fa63e2aa3c7a1f5e3a3507a9661d3689066e1e45db3563dcb0b2596030f66524bb5178048b27c0a440779a067806f7872382753" },
                { "mr", "0a257091fc485bca7b8ab2b9707a52e946583c8cc1d8250b5621e104f671716187097364b6de263ad759a2c6fada8c8ca61fa51edee814e47615a6a6d02a8ca1" },
                { "ms", "3046ce9725f5c2b1e44bea1a71a7f06867e7653e5e9867f0819b684cc5daf2de1054452a1855717488626234092ce15711579fa0cccebab98e48febebac3fd55" },
                { "my", "57b564e8c1affb5193b3ecd2eea4792b69d4289adce65fb49389f073d012cbab455cd85da238c7b676b9b2940e59246dc83cdc6a1625252dce703965741c9e1f" },
                { "nb-NO", "7004712fc3a8cbf3bf762ad2f84812a073491c467030414bd8f9221db3c9effe46afe2ca8dd3f867c296caab16605cefa0f4867f9e6f2acb9359cf60af49b7e9" },
                { "ne-NP", "eca9487f38718447976dc51382c4f8d47b147233f0c021a699fd008ba25bd87fab04fb1ddb1687cfab96937c403075485881bb6d186768179b0b025034234235" },
                { "nl", "e46729f6ee47dc0734169ff2cc75b728ee8f9edb276197c542ef730f557ae65f3ccc581abf3e1bd5b1b91b6373fc7a1605dff4a2e47da038312ae95c6a6838cb" },
                { "nn-NO", "12a4fa606c1ac1b5ea8bd60eed140116e1d5b9787c4a5ee8568ca0cae047b56a8f9e77b2fefe7da402d9709e442a2311101c49dcd009d55d8896f045aa8002b9" },
                { "oc", "a3bff44d0bea29f4bed75ddb4f7c54396b47a82a3a194e017d37ab009c309f3baa655d37db76a2e1bcec0a979bc30397795f4d019c6910471873106e74d7afea" },
                { "pa-IN", "7845e6e5c38036f2e3b0b7ab8f83ba5a42c6b748ba7ad2bbdcc707b16f0377e4a395de59eb0fc1affc1218cbeb00c21286f664ee1366d8493cf29eef3f007f7b" },
                { "pl", "b87c8d35a4900374e23d16fa8352a788139a57c4791710bc247c73f13811c7817733526de2873da13a577b53b9d5fa5a98b85aae07259b30c0dd709973dd2be7" },
                { "pt-BR", "fb316109b69653b9ad50522d00f03bff814f6e27ab9dbb7d2fd381efb51c7e4374bfd6aaef75e1c7e5c5dbf99fec4e07e01bf8b9f420fd534d19bb3f43ce572e" },
                { "pt-PT", "90a2316555f21fe67a5f80dd45e03337bedf23b09e6931a925821cdae59df888aa79615f6104fc8b5da91cccee689a6c4f5fd52108f407df631ca4ab45057d9e" },
                { "rm", "fcfd1b682a51206f03e3be3a3bf59facf779024a697791d92c941686a0a52cbbafa5938a8ba0cc0b712b1ba982085000a3346ad09e130110a9cc82d41b422e24" },
                { "ro", "eb6b00ba26d0c96549145defc06dc2fec2b8a5564d46d1cb81c27e3b302c365752c9ab874cedf413eca8a5cd37709e836dc3e600670e145f563de9471ef2f359" },
                { "ru", "37e6a0e6fd13163d29e5b9e632a5e283eddd5e222921274962280e15ec29d920382c58235d1976dea24660ae4a6098ae4d43bc1f6ac24f8056c0002c78b5dfbe" },
                { "sco", "a87609714aabba72ffdd3db2fa9abc9bae17df16090c26f21336de963c57e365a5f6202465986a4f1d026e905d0c0482a5839fae02a0c41839361cad6a9bde2a" },
                { "si", "75a316e1c96c6b2be350c612e0fd15455aed088c1658e3463b711be7ea059e63f377d40f5480cb356223a27a2059e4e7f184b4dd50750f6ceb1eddec4179fa50" },
                { "sk", "e8204b8b67f8ef6a76c388bcfb596d4d180c11a44a956df8769a8a4cc5b77fb0d2ba6078b40204ddf7dc8e188b7fc337e9f096229c4f966b90fa601bfe92f821" },
                { "sl", "0f52f15d197a865618b7c4f7e18dd6273ccc5f5c5a727b1c2e865665e9848ddcd5772909a1ef7ca361884a2178d1e2e7f3168be88ab63b273374321f839842c5" },
                { "son", "b5a5c9c2c0cc339117a463208197444046911379a5e19feeae8bfefdb499b0ce83a302cfd9704f8fa76194e477c3ce8e4d915da077a285339706aa28be8325c7" },
                { "sq", "539257757ec7814c134501541fa08c141f7bf044ef7b36e1d12732c9db495a0e473f77914a1d2ea19965e258ad0d6f42a81ef92f756a2da891cb457feeeed6b6" },
                { "sr", "4d05775ca58d1fd744004a7894da11762ae3b6f7ad518bb17df705abc1b6129fafc1399b96ff9e9a5802f5fda943702cf3745c21a8aa93b8677fe25bc221b90f" },
                { "sv-SE", "3c46f28d85d5b71559993c2551d9e00a96a04dde3b87f51f03a5aa391855775a0c58d4e1118c78f4b8aafd4c790232be5deb24a1c777148dd543a97347c18ce1" },
                { "szl", "92a0d55c47c2cd0170ef595dbc643d76360456cfc7d341ace72b8ea9b0217284bae43642fa7945a93a340f1f91ddb2de53782292231edcb4e0e808a0166b09ee" },
                { "ta", "3588952c2e7fe6c6667915cc6508d9a950c80798052a3cc436053bca4ba59564fba9bda4099bbc2e90c22ad351393224c719876e64f01cdc17aa0c33ffc24e62" },
                { "te", "ef46be367222bc1e37390d10ea86a703119b1d28dda0631b3abcfdf182bcd631c0a986b468b0a75e5d08347c16d7a124173a9c92e67203fe411168a71bbc1fc2" },
                { "th", "333932f01312c1179a8580676017f8fce5d3a68eb02ea989337c652c557f7548549a7767293bb7dbe7b2ed2beeee0650a5eb5b6d6e6d828bdaac2fe19b92cd36" },
                { "tl", "58b7fbb2051376435ffe6678673564840f7c3c1f5c98067e16373f1597a8dde0aac60e0ffce085494a5402ba264a459dba5531be9820b3808bf1c04e985e04f9" },
                { "tr", "004a998dafb5eb618e268f854a77f6388cc2212640a820877f549312f2c4250cbca9f283b11f1a242a2b395f1d1f29a618d02b3435c5fee8a89acfb8b0e85cbb" },
                { "trs", "9871d799c81c4bf15ab04a7b7b52f463a39240c9ca2f1092666999ab36886905c7e1a8d585b2865d8608ebd66fda71fd531717a33bfbcadbaec4301904792073" },
                { "uk", "8e4614fe5611f09f6ecc22634a8b9074fe201b92de587434affa5d07f42fa2e399eba46708f3a6fe02ccd849662c2b3d5723b9799af9c067af5646662925282f" },
                { "ur", "a613cc22ede13ec51c7e566952182f5ed6b9349bf9e33eadc815317e2e8dc03cea6f9b7a73e636a6ea17e5f0e535cbfb045a49fcc3a43f569ff3b2de9f8dc226" },
                { "uz", "bc089f99786d65a839e2e12013484eb9cd81f5d352d9e3d517efb1292a15365e7f7b1f6f4b96cd649742de87359c29d9a5b6ae3121e4df4d6c462781f04eb957" },
                { "vi", "6dd08eed142821d98dc6c503c7eaf44a6e35e4742e508881ee347bc3bb1758aac253a09b40c7465d67f698b93ef8ea176bc50f8e3e558709d7d1d33fd474e80c" },
                { "xh", "1c3ffbb238e9c157a251a358840c99c832c7b86f4cf29e0c8ea15861479cb0bdb9005fdd99c8e4142d9eb1fd241dd970cc3d944d12b0ff157d0b8cf3c8d064ac" },
                { "zh-CN", "d366ad9bf178999ee831d0f5745a3740a056ef6434d496b4c5655a2ff246d49355533f611bccba519a4eed4dd3cbdca3b36bc8f650b82268909934a47b26d6ea" },
                { "zh-TW", "955cd1c37058125eb04b71305074ed40fd6bc5630ebaf96ef73a0e0b23d24606fed4bacb58e81088cd05f5dec46cb6ae8ec86c6f51af5cd7fc59f8e999af6b3a" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/106.0b3/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "c6743a908df88bb3691744481807c23b06a46f355bb6cd66a64eb7140d17c76d5e6c3a2b94f53dbed359c0c9f27e100627500940f0e3b4a38ce6251f0335cc79" },
                { "af", "326fb3286f5b82942f3b8b99911841bf1a39e3748a7942848858ec4b92d0dc13f26cf413f25b008f2d08cb2622a92a7f371547974a607484a5034a9263df0958" },
                { "an", "f4aeade500865e304a8fc398d1d986e848a0b7fa2d9a40fb87b79ff3392cb3d7bd2740d42f6ee340cd211e668587f963166f8a79a3024105d04f8f8efe351dd7" },
                { "ar", "8566ac15da292aef90dfb87c421e4a0cf78dddefeec5b1d09bdf5e284c07c9891cb01280c6463f8b1348f5864a78215ce699d6ff51a4815990c4cadafdfecb51" },
                { "ast", "871510f4855c3f1c13249f7d4e8ee1dbba07f3f4c09ec25767305c0790ca88354e0bd93f67b703f8d345bfe690b72bfe050f8bd04194dca2da920a05c3126778" },
                { "az", "0c5c5c42c993ad6e0b5b90e573c34796fb0e5430accf52aa37a5d264303177731cdc1a1a32f4690dcf61b7a86bc87a95ab12865b7c15b9afd5d50afd159c4eca" },
                { "be", "0eb95c3bd2ba527862351023ad005c436fbfdd64402676e76b39f68c46d715e71167d8e5447f10bdb8c44922f001090c5d57a3b918eb632211b0f1ccedb09b8e" },
                { "bg", "6bbd7850a2964217b24d38dea949e0055e9186bd1b7f576c1ee7639ca9f00032d0773350d492821c28db74b97db3d58c53bfc05415425f521d12048fb6d20f8c" },
                { "bn", "69c9026a3e260bf0a2c92847028a2d4ff899bcc640f234fcf4447e76919020fa91b3c534bdd7949a13a15840c6bf52e4f587e3b164d5c2098479091123aa8d12" },
                { "br", "b368aa8bc85c58f844f3e519c5931d5124ed7f84638e436a40075e35b781c5075a5aa0f27985ce8d64c58d44f3bfba215f8162043cb6ad55b131462ff519b46f" },
                { "bs", "2ef2d02c9edcd227c9c6c0c12f47a9957b87f0c70ffefd92bd50d0a0fc29dd9f451459280ea7950254b842a1d660fb318f0fae6d566cf7b5f233213fbbf57112" },
                { "ca", "fdd0f238fb7b46f775fa061844742bc13b61929db1210c39acae54ddf98f2d0147f40b6ed805e1e2e9799e869b05ef72bb78b95d8dc6670d9ec582f65de0f7e1" },
                { "cak", "07a9c3c6ea149e2348c38de1b4b37ff871a6cd3c2f86b13fd10a6b39b837dadab9642d1583a64adedae126bd69e4234d030328b55a1f54cbeb4a378de2ef557e" },
                { "cs", "16a92200a880b9be3c54061248deca3c6cbecb3458faa4219c7115883821f6e77f5516cb86864b4f644c046b71447b61a229cdd58416035e94f969da0c09ac58" },
                { "cy", "c19a652fb0737e087e76701dad84bfc5200d5f30d015021528b215ccdeb4fedef93866ad6590cad444a0145ae151033dd26b5c1662d14b2c7dae89ea0a944d62" },
                { "da", "e0ea4abb9fe3f5c0aa2f396b51f786f1d2a1ec8a071edefeee5ce679f299f6612128fa448efa65738f5bfcd6e4760e42b15db94b805c9cd6793786465478f223" },
                { "de", "6569e233f745c6701d263a34bfefb5b5e5fc99e6a76d5d5a3bda0b5f682868ee77c4fbdce1a36b3c0bd03daee46817d6938843e21b97e23ebab04745b29fb1fd" },
                { "dsb", "0b93505b80976055265090879afba761f6111230e59bdab6abc96d56871904b87189efbec169fbe4a24aa9208573e7291bb6562878e27485a96f03b8ba3f335e" },
                { "el", "aee4f3d86b0bb6b11400e70a20b238b10afb1a2375131add397db742f3dd15d23762569c489ad3856e4c6e3d3b863f874cf5c7275f81afe3b4fac608b644ef38" },
                { "en-CA", "5d02e47e19609b562731424b555aae92f340cc02db1cf154f8430f13ad0b53f253a784af0fb92ccc2d9ce7cc5d1177118d1440e07e0caf36d1b4d2e8e16d9c48" },
                { "en-GB", "b82358ae0c52c02f3c19848155e6326540f10b42d9aa2075ae0b93dd350aa231cfe154ca9ad397f53bbbba5eff0e2bcc71847374b943619add7c5d37c07c9cea" },
                { "en-US", "5a4aea4ca07ab31a9d6e383c38d697bb13a276f556032c3ebd0ab4acb62ba760df8f7c85fc68a67f27917cddb8b502e5f704ad45914150b3001aca9475c518d0" },
                { "eo", "08fef8d5abd2392f55da156ef5aaf37aa195bfdd3d61c43971fdb729b138ef9ced89a5cfec0bcbfca04094d223f099af70d7ce64d3b0ebb7f8036f824e5d3669" },
                { "es-AR", "53979675eb8c3101483b939c2b94e5affda729eb91ea86c22779ec54cf318f7ed9efe9f74cb9fd2e7fb97bca5044db08a4d15cfcad3ac802d5c2dae606a80b0d" },
                { "es-CL", "10d484474d62c2d6c0e0d860f961dd62c03e813a8e995e44d457fb1c65543a741b76f0e0a1abf74697dc52adab97916383c7fb01e59897b84486edd8c24ae5f6" },
                { "es-ES", "964e8b3524a05841ef95da05df3555b512826b074b66e05853fa95bdce8728412e43eea487d483fee4b15b4e254ef61686aeca13ff70fbd69962ed513565ebbf" },
                { "es-MX", "9b00038bf5eebc85b9899bcc7698e1c0917e32b10a93316c02944edf43cec362414cc39e4e8886137bed7e943b649baf7cc9c172aefbcf4a247b4ed7e53941c6" },
                { "et", "ecfe0cda620e932774424d83809a8a7d232ba784e6f17715b577c1fdd01c2ed5abbc0772effb8326f8033c640c315e870005d24b2e9337211b3ee9e00589c226" },
                { "eu", "ed3059e2a5802fd369b52a805eb8616f57fac9f43d0529d77c99529cba92ba06858058d1f6c7429dca9635cd8987a16915fe1e97cf907070e2c98889bfa34e97" },
                { "fa", "88d3ea98c5119745f01f1c2bbceeeacb62287f1ef110154665da33536d477985305384331315215c91daaaa15b2b6334f4214bee6d43609ed9a214d3179a39b6" },
                { "ff", "fba51026353cd517326f1bf154f2fa31e013f4c9d3385cdecfe49cf2b5c4a9c985713d04325b40e7d2d4b312bc7db1457cf2c66061cde7308dcfba1a07242b09" },
                { "fi", "fbe6f0c84b270266100cd3138ed0312051e107797a38c1624311ac3f7fea4898ca231ade61df3a6bc77d352bcd1c680046ff33defd21239b90eea016cb1978f7" },
                { "fr", "25c09d6a373624b1a4e60c300aae3f26b0d10eae6d6ff3fc36ba3219ea435d46309ccc2ee610a03ba32687c049ca3a6d534f8ee9cbc71f674cb543ac75c076ec" },
                { "fy-NL", "25b19d030df847f671d15d096867fcf92c1b4707ac7b3429b0344ef5625db5fdafe3aecb17ad67d4a39740cbf80867bda4b9c9ded95634b54eb4ee9d57813665" },
                { "ga-IE", "f7f025e4c8f6faa8d79af090b5a8d72b00bd822f7e464f70f327ac268b178bf179eda8c1ad345536ab8ddc0d241545ff8a5ace9370dbf0d9fd65b25cdc6b4b1f" },
                { "gd", "568c3fbe47841b394a93bbd9f45f1ac16e05b8a32ca1db409b8c58c725b0fb84a72a245feb56358bc7d9d23f54283f863921dbf1d8030e931cc42fc55f60674e" },
                { "gl", "d7dd739fd009f9f1eb52e01ca2f7e62c48a10c36cf92ce74dc334cad60a6d7b7ed87bf8a31b205642ef7eaf9127a5bac86aeb027de31c34282994b83ad2a173f" },
                { "gn", "7654441405a3aad8d14ff0971f1911eb209ac04bb846a5c5a86559b45b42ae721748066334bc3fd044c0412c4ebf249feada1323a7e24e494f79428103ffb573" },
                { "gu-IN", "e4f92f3c640abe46c11ce264f4ec08e479b4309abb1dd7a088b90364e77a0a54a4e5ff465c4c3f17cb2cfda9750f2ff5d99b8487b9a2d78b6e33176017de9fb4" },
                { "he", "b092576168b302f06312e06a79046fa5d29aa34cff0ca6066c2d5003a6bf9a4db132944befec4ef480001fe965ef4f319dcd29f73c894eae3568a775a0c60202" },
                { "hi-IN", "36468e6f18ba434b7ac0939c8052732c824e5eba8963f4c1c2db7266e8d2a9fe40bc41bad1e7c4f1bb140e557d18fbb609387d70267b55b1147447964b409138" },
                { "hr", "f77d22e490dc2fbf8de1a9b7eff9ba2749fc3cff31b0bc97469d823a0dd1b1ebdfd36d545553ffb0d37f60036334beaca6afb9aa26afd6b011928d63dd0a0e4b" },
                { "hsb", "40c3f480a5b548cf40e02bd36b60c2b84d06a07abc73ced8d7f693410e36ff8f907db0836963fcb8b1e2752fd4bd09f179a659e3c648d54269294bee3aafdd84" },
                { "hu", "1056730ac3f006ea5618192bbf385b4b12addb42330f344476e296e1d792646db0655f6e44feba199689686c067a5c0211abe973412238c9673ecd93cc0b3679" },
                { "hy-AM", "0f7acf6c4b66b28bc877f61af7ee53ef7a7e4c6edfeeea70d89b416602df7a71e493d77e35265ab6ef7893b2641ce01df4e44a65e2672009660fbc6cfe3304d2" },
                { "ia", "75ffc25b8bba37d706360d398185951b2fc651e39f7836b65f840e189311926ac1fda47bedf96fcecf5d888c9e9150a86e8fd424fbe41f15ec2bb784617267fe" },
                { "id", "33621cbbbb32955de187d887fb02ad7149356aec6393be7e99fbe88791444a8669b97ddb915db7e3886573222964f72b4707a91697775e92b76a34d8cb29394a" },
                { "is", "906bbe4ea67ff9299b1e91e94eff2b333b8acfeaf2e88b60f3701f6c02871991315bbc2d31a5ddd711f0ca1282f0cac477176b21265505489f70c183bffc13a9" },
                { "it", "24a5a1ac8d147e8724455f41995c3ddeae24eb2886d31cfe67550057987fae9901ac45de7aa369084be260585895b05c75cf07b0bb1f53669057f8a4f8ca0b5f" },
                { "ja", "e9003e1419fd47c047f6d0dd56a37496aa5774715c26c3c30b27878fc9248f7b10dbe16a4b41284dad31a47f90d4cfe44cb37e28b5117cf3ab422d24946e6170" },
                { "ka", "63485c7c9998527f32c8b34369ec22f08f7c0fefcc1b35fdf224ede65e1c872c6ef6aa8712714b20cb3ea3958dd3fe08ac33901c3a27eb9ca52499ee0116ae8b" },
                { "kab", "9a7f0ade479b05cd0116ea0e98c3fc66579628672c07f74f3207d84fb2c9992d91fe3dc793e2c302c86736170bb05544b6d0c9cc4bc12c32dfd3836058d655ed" },
                { "kk", "9f9cb342243c4884e9b940b4a7fbd33c469c0777041d34d4fb9b6c4f0afea7bc50eb938c1dd735f9085566f50161b5383256d90fae80901c58bb062b30eb3d3c" },
                { "km", "1ea6dc2e5773e33815e3190973b20a988b67fc985359312c2e5fe0a0414242a584e47838f3925d7bec7b1f890736b981f334075bbe27704ba0e0b8600c57e8ff" },
                { "kn", "f8eead967c61594421a1e95065c28e66ac891147b160934c0d5076560d102f1ce6bef3674515ab44515ed523915cd0383e8f7092bcd80316d35cbb436c332172" },
                { "ko", "ffe4c8619591208603742ae85ed5a370b9b8d8e3b041045683eeabfd158bf60209c8e1eb42ee3ac5cba7fbeea5001206f48166500a0a4f22df63ab79a3ec2d3c" },
                { "lij", "30b28e04381438d4cf8fe1f2399c76e9360b0375b03fa8a626b0a16f04cd4c87ae4a47c8beb47d23899d9590bd476206af0c8c5456c00a18db886814e6945ea3" },
                { "lt", "d1c65dc57357ab76f03090ebd4cf86a769111ce7118cf6915bb7f1a6549c855306ca18cfc5d58d087975df7d9a8b4ac3c008e1a975007b80664980c08afc6edd" },
                { "lv", "dc8b4cc09bb219afc8b9186c78cc547bc11eb27ce816e1435a1b41e9e446bef4245e14abc9827e2c661ee8ce3814f1aa132dd44f0b5627f5cb30dbd77cafa8d8" },
                { "mk", "a19a432f407923e6ae6d38e6c5e3cc5dacea639ca6dea6cd79f18c62bfc34be9ee464967f0db35cd3573bdda731fba4ad9e0753b5ddcdd3ee7c154737af37ec1" },
                { "mr", "cefd5f4a6e6ee6cd4070219a61dfad0423d6465b87673803bf4c8ef2803b465af3899730e8d12f68ab5df6a03e45d663a53fd76c4ba189951b5edbc4f91fa19e" },
                { "ms", "2e0fbab36deea81a9c2c5ab4af9fae03d033e1fedf2593ee5d2141eacff5f22e916b58da0377278a6b172556e78dc53e9177eabcd376237621d9f648da6c1fe6" },
                { "my", "b516d339ab40b86fc5fdc78b790ca61c7202c9488d6ccf28c9117dcaa115b6a5bbd7c89a58d426f6516725e6e271f960a62de1d065ff3cf0fdeea338498a1e74" },
                { "nb-NO", "9c82c8ec0210ff737d643806b2e1797c15850aaf709650e396eb1228048b53892a6b363fe73b94d327f35ebbb91cb747821f1dcbba6e0331abd11937a284d27f" },
                { "ne-NP", "cbc6028279548cafe83a044510ddc4ad9a9d92e84c6675a3c996fa06b376b97b847ae9e31f525d9193175b9ae6b9c4d82a908a867d561dd191f115dbb24da3d0" },
                { "nl", "948f8b515e9ae5c4a7e5b4ccbc8a35472cefbfb2d2bd6b833c3a5310e16309e5dfbf56da883eda9c99c39f327960e62b95a39edc238f7a3c8d243cc3677cedf8" },
                { "nn-NO", "77330040ee3b658721897e49cd5d3713d7e106f3bce7fea836deef4aed758e979d8fb8affb708fc98f7d8c68cd1ea2372da76cc489af93e9711b3ba2ab1b3447" },
                { "oc", "c75446ade72a057e81ee98dafc98e385fdc178902c0c89c91c4d19dd0f2555dac16eecd4aafad25231a09429ce26a8ad47c36454ca901d8431fac3cdf16ec5e2" },
                { "pa-IN", "a3dffde09ebe3d7de19903bce74dd200c984ab88d2ba107181702a50fdeb1cb72122ebc794a049fd34a5259e07dc5bba8a31026eac684f874dd26e630414f38f" },
                { "pl", "0e14b7fcd9e00fb39d30e911f2aaa7b75d49b7e2c46676585e8f0748033ad7f8e93ac06f1cf27f66fdf21e9dbbd3e34fb2fd48070289e14fb78fecc6c99c1d48" },
                { "pt-BR", "046ec208496e497f4acfea36b8888878706e068f4122e2d9ed18327eaac0414a843575e0012e73a72aded344ec2fc06dc8b66494eab14a711b149e9281221aa2" },
                { "pt-PT", "5cbd01a8befc9fbefaa74ed2191337a87c71ba9504021494907dbba0923e43279c48e1bbdd1c3f0c5ae3e8e07832c364114e2012b621faa4452ae2640b6d172a" },
                { "rm", "fc6212e3355c423693f7497f628339dbb38ea25f75cfe32f4d8d0f4c659c0617916c57330db542d08794c0970f0c00145709b8ef637f4c95cf12c185c2129909" },
                { "ro", "af91d8b280d6932187d891a058743d929d310687c99e210b8db7f25841c68db491030f1a009b8351d18927c7ab3cf2547112fe230a80b844502e5cf8c25be3f9" },
                { "ru", "a192172a4cbc51e168e9f0cf5f54339b7cbf797a009b81a2607a1891318cb0ad68755bd79491c061709b8a47afb2c9d5bf38401fd667f75e4b8fa622c0ecd4dc" },
                { "sco", "079c310585b6a5bd6a685b7938ff17c9e6fcfcf6c5ae8a806be7c64b38f4b53c47f24b0490f38d5463dcb0658597728b52c5763db672549c2a3f4221f3895d7f" },
                { "si", "d4af51913cb6099e59e27258b7548d704fcb236d83a746ceeaad406e5c8a2c48afeede2c683fa647f69520800576fc25829bcdc1c32e49da127fd6dbc11c34a9" },
                { "sk", "6f25fcc6876062737ee2efa5c2ef0a98cc4661bc3bae197ba6bd96257b72b97cde8266eaeb70f25b207cd96f9a60e21b00686d204a8253fac31a6e213c06d187" },
                { "sl", "b653e505436e93b1bf01ea9426f6d9ba5953ec9f026e741d17dcd3dfd815b346cef308631b7854a8d338cf6b4e8ab9c5d36842eba6b0a5534702af1a572aab9e" },
                { "son", "9f754fa06f42d160f8577503770e204d6ecc20d8ec7ef4ce896cd66d6f189a321663230b1bf3cd525ca545e6ad1a533c9707f247c52104f3fe16755ab587c8bf" },
                { "sq", "1e9b8d3c4b996088dd03209d5f4297000e08f9ad48ee641ccfd95868a7c45ccc3924eb87a833e19a9bfa7b0a138ac679000b3138adc1eab8e4d7fc61121df7e6" },
                { "sr", "a4519b26764c1a6f1581009c6d5ade2465e111bbc2b7b0caddc0ce92d9d87a67daba8933a417937f0048dd060cfa52db39e69d06c83e437d2bdb6c05c3452bca" },
                { "sv-SE", "c61126ec9bd59a6f03a32bcde03f6be1da86cb69f5203c891a0c917f9d3b9396402d792a76d20ac6bdefab2adf696180994bbd96928d5582d2ecafd2378d1c1e" },
                { "szl", "b6227db9f052e142c3c3a834636c6bf6166487c7d72d63be6acedbca3d1d1744092021e32f3ad978a036ef0996dd356a523ffe577d6cbd6df39a9df70a8747c3" },
                { "ta", "99afac6d24ba6de48d22722f1d5c994fe528c7ea62d294dff5c674f6285ad7abae6023e42a4f72fcca0f8041f43e3bb7a5ddc2bab1ec3ee76e129b95ed852be6" },
                { "te", "eb63830c86cd7b05408f3a507c78ff9ca579cc092066eb74b4c829a6b02899617453e0ba29d09420bbc6117e93997bda21575f66687081da77a6b88195053883" },
                { "th", "dd5d44d04aa3016578c07d277e67b667341cdcb03ab50b27c1772005e48caaa552ec3fc3fc11df82bac29fd2c19f8bb6f5d84656e2a5560595b1a720e9bdd66e" },
                { "tl", "feb84aeea1b20a54991a69baa94a62d1bd852a0454bd830491084aefc1ed5912603eaef2b0d7965873ed19fb1c125f10c088c6a01ee3b555ed586e9851232b28" },
                { "tr", "79b3edaa347a11f6e58aeeadc07b5a719939d53c4e19f5c41ba99e8041dd2135e4981f00f25acf4834e3149448030b403b355fc2bd2efa68c15a6eefcb458cf1" },
                { "trs", "d197ddecc6ea84fa873c8f9ca9661dffd03f86d1a580eaea0772d58564019730b9f079ad3257f2d5dbeb05321b3c4a7f9db9977c409deacd2d990aa7c2f1b739" },
                { "uk", "55818f208bb4d3c62c65932d92989686983871dec1775f08abe70a18321f57d5267fe672cd25095437143c3648b9bbd55324450c6f683a996be6ff36b31046ea" },
                { "ur", "628068b9ae8ca4daa0ea66495f7e3a8ca152f5f6cc4cbc0f185015c251feaf839f6c8040d937e2737e0a8d87df3a46d100aead2cbf3d95d46d7ac61846b34c2d" },
                { "uz", "c3973470a3275569da86a32e17e9e57fc08614c9dda7311cf48c82d49fe7ae0de446030b68eb43c06d8b2878ef1994e0c5d0f2b0b45849b9378de7e40ce0cd70" },
                { "vi", "7dcf734198a22a5ad5c116b4679b26d815bbb3b9d13a94f4aaf209ac43faaa7c2a77df20ffa5cb4f29eaa23df944eda9a0f7049e4ab8807243e2e8ff02a1e852" },
                { "xh", "3d5c546bc4a95786f9a10e6ea96455e6e6474ab5d44f2fc2c53ef5485024a2ed703556814b9108ad066df17fec69059128f3e98b68ad61bfa5d483173d8b4221" },
                { "zh-CN", "48c97777cff09aa9ce3e8330e4ad2759e5531d38bdfca89d64968bee608e5747da0e10d16656cd6a09a6b899e51a4b09941349781fbcf8302b42fdc87d3e466c" },
                { "zh-TW", "be24edb4bda17eec07af122ee3d97080eb28fa0b63a0ce63f861e69a7d787e2635506dc3a90837dd5e87945d81bb8f218ee291dda4db771f7e74bbc928b23615" }
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
                sums.Add(matchChecksum.Value.Substring(0, 128));
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
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
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
