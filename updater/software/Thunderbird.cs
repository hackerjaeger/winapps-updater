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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
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
        private static readonly DateTime certificateExpiration = new(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


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
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.6.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "57dca7f16a9a495bb0d97dbe4e2fb148b13a9049548292ac5254534759469578c5161cc057350881bf40230503cf5cd03ccad7484c80414341f2e789348354af" },
                { "ar", "d4cfabc6053390581ccfa20a044a28a0526646f08d359c534f64ce009d72261b765389c76e2d1701c9207fe06ec73447faa5ea0794c959a6b4c832b5828e169c" },
                { "ast", "07e511f30a47471add27dccb371475ac00bf7ac6b3e96e9fd3c342e5c3780bda18131093e3da25455003475813b810f673b89ebfc839775b4ff5eeee2d50fdc5" },
                { "be", "04c744bf35b5f6096b0500a461512867b37bebd5154b732c3e7bda29f9c9cd0c84484075aeaf9abb7410ac3391ce82c4dc7a9346a736249a090849d3f21e5e96" },
                { "bg", "094381f9bd02ceaae5e8312b198b4c64cb870b571f4d2a4ee62e7e71e6ce246e293504018861a057547ea92acfe07fe85f1173896cf5ec45a398ff8ba47cba79" },
                { "br", "54a0d41a8149bd632e3ed3e010bafc262a4e211286636318a871a6691f00c2e8841b457aaf6baeecaab53f2a5ca016483211ed58499faf3c2215ce2622710827" },
                { "ca", "0027091a3964c7914c140f84f4edb57eb30c3e0e6566bccc1949a93cb8d049f43d4927c6a4e797a0a2692b5de8e74388ea8fd957191606b94f3edf4a45f24f9a" },
                { "cak", "e5f49e347ba09643f2946dae959366a5c6a8c536d5c178226c69fbe06b975d90ba2bd93773bd2438245ce0d2400756a71d24106758e892ba2790ae768bd5bdee" },
                { "cs", "596bfaefa99b302fa151e9972b3ebbdc2c9e65e9d3e446d0863639eaae1416faadb19896f9168004231d24e1b209c1953ddad85561a5be631f8a160910dedacf" },
                { "cy", "e8864313c000c9b693a0a451c58f8cd8c416eaff8bcd0fb79ae78728bea6c4dfe340730af90e07f0daddaab7da94503bb551a1f1a437888de314f153d3dc0a6a" },
                { "da", "b1d8729d98a71382c38cb75db6e51a551392d8f244ad6ae2c229d06c85688cec40d1fd8122917e0be110e449daf35627b60ec9e81af66d91c01a5126632559b1" },
                { "de", "c22b33f35f58613efa7c39bbbcba34fbe7896aa157504967dc55212a51544b214dc3d31b108e9e0925b608ae9ddf5591c04dd92530cfeca14cf6459da18c8bf1" },
                { "dsb", "6eb9cf319233d0fe486f67adcddaf534dbbfa4b474b6e39a483040bbeaebdbe35f8d8e8f2bf1e7ece320364979a417e5c3f1a729290a7dfde6f57e447f5fe4c3" },
                { "el", "79969fec8c7b016e465436ff4ef59e6ff75334114e534735677fc680052c2b38baf5daf6358f55e72c31dceaca2e15ec3937eaac141469b23b3d467a08279756" },
                { "en-CA", "d343112261a5e1a64eaa03f99fa304b3db8ac4475fa88dcbde7da4c584b1b8b45464813601bc812823bc506a1830f31fcc3313c6a6fa0cf0786cdab1bb1cbf94" },
                { "en-GB", "59e3ef748a4fdf829d200e40ad7e4e14a03fc3330fed08e64bfa2e77ee5ac59b3a5a8f006f264d08b52075f4e42638e4171362b184b7685b96ad62c098077ae1" },
                { "en-US", "d3f61423db56c09307282b5114cafc38506eb1623d9c35fb6c80e3585de5ee230e60c687b7e88e3f2854bc9c2cdad9f901b828894b2810bfc47d45da5f830503" },
                { "es-AR", "8b7965f377c8f03bfedf809a638d3c6de3a4d43a8f6b0331ad02d48fa37dbbb1b6d712787f61683980d9a55f335a71a95e698b12dd9261e9f7fe84374e574c2a" },
                { "es-ES", "9450925ab5e4ed8e38de496199eed085c7c34bd80abdd0cf2d9e453e316ef93d70ad32ac0c3a6f3047785dfc9bc726e784991e294eed22d0800c359cdfe44e6c" },
                { "es-MX", "91c51d4960da1e55f528de034b6ff91ad5299a41e52d5499bf2ad5a2db787e54d6ee7be34529205cc8b1b0edd112ad0a96401aa222f5df8205052f2d2a5ee1dd" },
                { "et", "ddaa00498f517e169404609f98a5de3fb161aa4b4730ad511cf91feaef8738eb24d2b639c592cb20d295df77bda904e60dbc86155b36f54ee56a80c9f1e71877" },
                { "eu", "ef0e452e3255890afbf95a62825f0b2803a09e2f221ac289babba8ffc34b3b461eb2a5588c0eb46984cbeec6723edbd342c1de3460a5397d1e1e05abee5fa014" },
                { "fi", "2776e8fedbc1a029431fcbcc07e3c336929995adc8617e44147398417ca4c4727cae409e77a743220b6ac728b0071e66b65acd8c053d885d93e9f458fed41bfe" },
                { "fr", "d5403c717d20d9241cceee0a0aeed996da7eaf3db622c16f79ec8a5f9a54a234a9756ee88ff7ce74153f61ef7ce259ae30fe89d13bf02872d91b70aaf74c9b9c" },
                { "fy-NL", "442b1a0e4f0bccb5eafb991be81448737e088210b6d6a4f8281c7f51549bd4b9e152f0bb608ed7baafc0429d518378cba86e7ac67697a3a258f4944db0d9fcce" },
                { "ga-IE", "f1a51fd6bbad9b02919bdb280c30f049b23cc91ef368edf63fc0bae375b992b3fbfae0b6200934eb6a970e52abad60d9fa4396a222b2357fa4bcae04ffa6b2c7" },
                { "gd", "1f0fb3183973cb0546fd6509edfe20799cf48db55b0b5fae745f321fbe1e7907b4259bb5b4f89427f19a9311029ad010a71daeed40eb46f2b31e6273e73ed850" },
                { "gl", "8338826eaa39a353bbd59df21dfb431b9877597d20751b6cdc25c896f2666e96cd9e7269e9d77c2519d71808575b34d99e4ae02cc311275b62c69e0be898e052" },
                { "he", "2df1af0b1ab763abe765c82c6f6b89a01d928cd002ad4c66acdfd6132350be5ca3e7c443a82a0e4ee4e3cd163990c6357b0ddb67f6f6aa5c4804e0cbfc1a5e5d" },
                { "hr", "dc1e764458db5277c91430ab98a57791c9b686da38ab3a798f35073487dc2adae1872e106c33a5ed249fc9cac694dc30450b573b3d6b98b9c8bca71e52318bb1" },
                { "hsb", "8944072f34e496c67352726516b68aa08c70ac27715d34b733ecb982b1890997c1e58ff9d6c3b460cc275133577f80e06858e9572ead82c480f6c5f7152f88a1" },
                { "hu", "68641090ea184448916ce0f3bd77d4002ae1deb05968d7b68cb3d4863d022ab019a8067b4c28964b6cdd076c71a8b6cf8f01ad61c1959f54fe1057bed914ae87" },
                { "hy-AM", "d41c9304711c120362e7a27a257503419668f9aab12c8d2235cd465852a6489b2d8ea6851ad6707818e4c1149ef42b2eb431a2fe7c69913d7c7c845ce459ecdf" },
                { "id", "222892ed560c37ac958da4d0394682d80e127f2ebe3d715502fc9efd430bf6a1ac40c0fe8a0794d2c8d995c4b7165f8bcd357e8b96e1803d7214804ded311fee" },
                { "is", "0f22d000c457e13aa785b7ed894fa2bd614b3e8ff24d8d006ffc49faf553f3cf6dd725c7e532d9d494bf3cdc2effaa5195faada1da71371ab787a029f09e1da3" },
                { "it", "44e8550ed0a6f6beef4c103932e3928e94da0a0686a5464a0aeec291c5c0a891fc6c227fc26d8ab8f33e01955f41c3211cc970bec12c5c3aab5bb9970cea9ae9" },
                { "ja", "bfd06c748b322994b09a4d8824aad10ce8b6c2547d04fba2cd8c3d5067b3578f2669629b8d790cf5d3472a179c2be8ecb34b0fdf72b67d7d891363661f374171" },
                { "ka", "ac4098c570e31d7b3202b24f59284ba294cbc7501bb2a3ab8eb5b66eb79b12faa7bed4ad3ca389fa576af39a67055aeb3294387dcb5b08e96db1700c5a49d344" },
                { "kab", "34bd6838e9698d84ea88ad4989632c2e611e82eea5d5516bb7ccdfe3ae8d9fe7b5ddfa2ab12ce1d35fe9dad285754413419972ddb40a1c2064fec48045075106" },
                { "kk", "bff32bdec3751d00f7b4ed10f0921b0da396fff64463c367b5e5eede049b564da8546c41e253ee79f761381bce1e003061a00d0fa9a3dfe86a3e3dbadea272df" },
                { "ko", "b0f33e3b51f6fbf966bacdba084fff08e3598f94e62bdf7e328408eb3239729d18b11123aa680216580e0562069c2d7eb08a4483c6c8e68e9c8ada2f89a199f1" },
                { "lt", "fe07322e567b690f2bba296c08837139f8b33c5eedf16ae5003c8c5be54347f58de492c6fdf9b32194a3cf7b729b45c88398f8b1650891bc3185621f83c59193" },
                { "lv", "a81482b47a4f98fd03e036d142b00441dfaaa0895471d638c86fc05aa15475d34446db6d840c7c30de541d3a80cee461012e9bae35de7a2eb68ce153dac7e197" },
                { "ms", "a8d3a40956a44d47af1db866af0b41b8902c69aa4411013537310abe4ac1c57643ac8f59a8639164e8dc2fb46923551fce6a849a79ac76d29abf0eea39171e2c" },
                { "nb-NO", "922ed48f0e71e825a90628b6e9fddb418978783e82a3e59cb3e1766cf27014eb6b0ab4381e9bfdf29aa9fb973cdc785971250f4b4392c3e0b100441f20daad97" },
                { "nl", "e62473529d49b3969178006ee1e676be39a5553df37ae4b6735e995da75f555c5e6046621fb6517b61d4e4959bf85ba00794fb455de9101f26cb9f9f5fabdcb7" },
                { "nn-NO", "fa9fab9a7cda538ca2516d96a41a9736f5249805adcd6009865ff5dfac15355f91eaa3769f928a7992bebc052b2903c57da8a73f38cee7c62d1114f7de7af5c6" },
                { "pa-IN", "242e56548e5c693847c69488d10b3f524f40c41c3bfe1a0bf0da6fc46b5cf72f0bf83d674317bc8d2ad4824ec22659ceec9ebf9dc4a49d7790c61aba62dad20a" },
                { "pl", "14bc7f788e796d3804d5d2df4793a8ee8a0323e11c5433c94cf109176d9e2ae532a3e22d33c40fac7cb4e8758d5ffb866f8979dd4b4272336b03884329ee1d55" },
                { "pt-BR", "d188be92a4a1ad409a46ed530b1a773e6c3353279c976022c0792f33b9fc543e8bd59b4ecc347e0042636204b05bf5c0f9f7feb19988512f02dbe9142f242be1" },
                { "pt-PT", "35eda5bff6c52fb45ee361ee631c4506385b602b799c4e274ddfaa292ce93615a5f07524c93d3af8197c6a58f26388d1e4049a30dfccbdcc7772f90b4a9e8dba" },
                { "rm", "f3799b4d3fb44f4f6213bf86d52891d493f46fa53c0d6dde603f326831640a37b74c5d996a2c58080998950e873bd73d276bb1df4ec90d486af90a9a265934ee" },
                { "ro", "2ed6986450fba56cd4e5f401f82acda20336d356a2e2e358870aacde7e6718c969f5cf18869517fab32b400deed2338e26ab4bd43ce473c53363151652631010" },
                { "ru", "0047d9432b28ef4aa0176ef82d85561e738c993c550ad4d4e3ac65dc696079ebcb17ef1a342d456876f4c258aff49f59cd5d0ebf9889303fe8c5d0280d6c291e" },
                { "sk", "cb98d0888fc6d842d3465a06ab430b2c2518f1cff3cd2d687c1c0d29968c0fd50b8749b9237744dc7ab61391eebf421920f01fb6b4362cd660b02b92d955a806" },
                { "sl", "020f21e0cb5991d8910e1d2e92d4014ea53124957328753c95e4cedce517930f72794afce470136a6c9e5a17dd39a8e958590d5041319c29434666cc9d878dbf" },
                { "sq", "a465350e98f28913a58b9d601c1c3fa9270239dec2461be4ecc2424643232480490eeced6ddd3c1bacf5fa8f5da33bd11ccd382de5374eb866ff576c6063487d" },
                { "sr", "1a6e3bc3fc4471bd6165dd246d8dd18c19b9a2192082b234d5e8ae479f40e88bc347793c66cdba40dbc5eab8ae819c27c86301b61f3f8a8f3c604ec329764b1f" },
                { "sv-SE", "ce1d2eb47bdd8a857f12a6c9d7dc0ed227bc036ead25fac9ad79cf2d7f36da69e99ad492e2600cac2fc6b550f674afb4ce2b71662836791ce32e6357ad2ec11f" },
                { "th", "899042ed1d551d6a94fac69e9b260bd4253abf20cd9bbe3fb93f45ccaaaf9cd21abb7ae73235a0ed73d0fa78e6700ac3e77e4f91eb138e1e7680aa67ffdae547" },
                { "tr", "66849d322fde3bc8354d9fba500721a40264ae133e68dcce6906dc6136933395ad8bab517a61d2468d1551403449453289b401ffd7a16dd01c2aeaae1b8a85c7" },
                { "uk", "4322adf4cae4c788a2a8ab9e2bfb4f1d893990e26a4d9676e9ea5257b2c73e1bea81e93d2c036e8f24e0c32e801c1cb4b62626bdbaa05e2f77418e661e87beac" },
                { "uz", "ed93d681847acb3702c1bcf0b48590dcbdf357471a8474fbad0f1f7347b157a7bb7b89d444dbae65a95184522b7af6a4129e0d1ad8fd07069b60ad7a84d19e52" },
                { "vi", "c8e430e653029204b35ecae2d8569914dbd6edbd016682cace9f0338338a403bb6260323edf2dec8682a811bffd2994829dfd19064db83ecb888668220236a29" },
                { "zh-CN", "a7a45c6ce30ae3cd99eed54a3bfe20b4a8c0c93595e25b85a0e144e0640a2292991a472e3a0a5c74ce93a4c458c4c920a808dadc481565d03e278949c937e301" },
                { "zh-TW", "e260beec11176a2fd721d7453e5b654f17e08f79df3cf6174d4830fe4728b3b2e1dffc686860b316d497d6da642037a8cc5830b79df064411354b7e2a468b762" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.6.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "3a77c155ec222f03cfc768803a3d795bff613d2041f51329dcf91bbd5f882af13816ef8f4abb89b537086f0edb51c383f6f30a14fb7dd4247a41fe42769e7373" },
                { "ar", "d817f1f5f1a4099b5c8dbaab15ca7fe734395d402041060b43292498e6f00c63e96677051c70395a19afb089dafbf0eb188826f8a79342855a67083357e24703" },
                { "ast", "6809acfd10d123b624db5b018dc571fb44aae6e01d402f01cffc5a786ffae03c270ff2d3365a38d428db666051c5868b771a0d8d27b3f4afd191c0dca90651b2" },
                { "be", "b4f9c5d34626a12f8a862502ee19818f5af827650ba344a7605bb0e61fd1dbd876f14c0104805b6947c262bea9a159c2a27a84945be6109a31627a9e50b7a8d9" },
                { "bg", "df1d0a08062f3358dc8ff830f4a03fb7a7f01a494ae83a2972a85059d2620abcc38c835eedacc240a79410dd680b3cb8f94c11ed06b7e240e38f63c4a49cc21e" },
                { "br", "c2b5c35d90429cfefc3d6bdce514749855e260c53ffd8ad1ff9f722055a623a23234c674391c7df2ee671ce65fd50596938f647a4de8b1d42877fea7fed3ddff" },
                { "ca", "e592cbe0f6e82a30ec058218f451bb79efd2609e86aa16aedb26eeb18e5843ea085a5166a9cd5997031461521014826a3c528fe3f3501bf340834976a6ee39ce" },
                { "cak", "a4cd19a1ad529ef275b40589eca451d33e4147ca579be65a6ba90eaa856a58fae9c49577d560d5349ac26a3923a057ed9099fde47b06fbc5c67f99c4aebe25d2" },
                { "cs", "30583396167d17ea935b344ee4e22c87b41b38cf0f39772a285d2cd9dcc2d86cfbcc86f060dd4d1784c961f4caa8195486dff4f22ce57fe020ac4c25ea102f9d" },
                { "cy", "5360f4553cd408696e55dec74d65df93c47acf43884e7425ccd25473749b1907152c58060ac8ca8d5e1a36481afadf08735b9c662f203c60abb5d45c8cd21c3f" },
                { "da", "a1d4eb2983610be78c05d3123e9521363c901a7d56d8d54f4e54331304e575335710d5cda0450bd8193302306ecf3d659e8e99d81e71e2a965efdefefe8e4513" },
                { "de", "502e1339979f53972b4434cb38ea311e1c7d87d316fea9769bb332978a01a7e686f70152c2876fe4fc57b8249cc0de3f094f51e74905e21b36ab568c027e64c6" },
                { "dsb", "13d7baa0fe07d5f74a7c885e2a3d046c52f335d723d956e5a534b132e93196c7227f461cc5d98ac60712b4f1ac7e8a371db3f3037bea2206942642df2c038e63" },
                { "el", "8259190e251851e199ab056cb6a2bb703dc8f0637b48cbab7fd3299242c240ed7ca850ad5e64cca8403760f22a81611d176d8bba59d14fc3827faee86011f927" },
                { "en-CA", "37fc695fbf3e93ee37916321059c0139deb512f22095e4e7991299ab8fc33b74caed7da0d48412b3fa6efa0b8fe9ce5882c61aed8158950d5e97965b2844bf10" },
                { "en-GB", "27a0fd7885ddf5ccee37bef401c3018a5a2aa65a813067667d7fa0569e6b5286387c46648327d56f1be3255d8b67705a5f2f10f18325f2c5da38734fa96619d6" },
                { "en-US", "1ef4d6adc94d18555b6090c9b0afdffcfc8e653bbdff991a63e6b7e989b9c3288db2a8d732053b4fcbeff4c537b894befabf830b1cfb591aba053b90de3072c2" },
                { "es-AR", "f3639f50228207d1bf7304c4cafffe53c64bcc8374a91f09219a93aa11667e39a4fb4fd3c19257e036ec83363c0c88f6a1968ef8d5dbbe7db2c55c56ad3cb7aa" },
                { "es-ES", "1ded3d8f66845b183bc046463d40eaf2a243f0eacef9ee628b6fe93378374ecc8b47b365d9ef352226031723b95c4191d5f5f8be8e0e4fa4396c6c1c278dc541" },
                { "es-MX", "d16f3c0d782926c84f43dcac0315516efb13ab9e58c57a1694362f35c70f65db625abf52c72b5a41d01cadecec968658f9f3ca8437fad428b1ee0abd07b17877" },
                { "et", "be0d0630a10143ab8d961c1da0d495a60885722ac60a241d6e7a917133f74d2ec6a6258ce2fa457d0a8a4837c43bd5db4540b09244486fb5c052ba1fd22c5ce2" },
                { "eu", "7eb09de2b288047279866b88973d421471b5cb5e9d44762a20aea124a97b1a0b7102ccc7183e95c61ad525f431fc41da34d54667801027390327778a47d8b001" },
                { "fi", "6ca074a149333025dcceba2b78953ca0e5c11da6aeb88067cd8a8437d5a20cc741f6390dfd87b08250f2e8c1cc4fb7eec681eb79f43728cd9c3028dc84c7b5be" },
                { "fr", "ca6b67bf3e6e58ff80005ac9957a0711f0dcb3f18d03d743ce2f78d63d09ea37caa9b3627714b6e688b33416768ba530134f4e19ccfcb895968644d5b1d75581" },
                { "fy-NL", "02547d613665c1337745f980f0b07cdecccac6165c34dd9659dd5748e2501d6070af5ea2a5a7a0a2f83a48e181f74eacb9241c2804187a1ac37871b97ca0aa2b" },
                { "ga-IE", "219959aac2591c13c638b7f3110ba8d2b69a31f818e08c297c592212bbbc08dae35f5cb40b8998dc8f81bfe6c6e07546e771378ff9c682c17d1c4abeb60ed8a0" },
                { "gd", "7013a90d38f86ec5801bc6ec4462385517b79caf4b341027fde4457f3e4a7ab7758a5f17c28581075cbee9b2d319f25077a4900d0bdbe4f7720d39403dae0832" },
                { "gl", "90405f3ae24622a1dff7193231e3618c73ffb534e8432fae9b527326a267a9c1701696886af223fa7374df0e3379d4b44e6bcaccb67fc18e0554d4fcec119936" },
                { "he", "e3fbb0cb97385e804e0d4eda4dbcee5448643054141cd2d5124b72569ac08774e9997f548c2aa7ced2a8e32b777a10acf40e7e9ecd7afce06254763990c7bb9f" },
                { "hr", "d6d9a99d60c7bb475362f7b241aa4162e6e810f54c1a31d177811f0499e52c5dc7406ad3e2670f528ce91528fd8622fc1962fb456cd9ecdb917dea7266f93bc5" },
                { "hsb", "6df24c3c0d33249235cb2657eb1f9e6c16201e1e420baf5bcc3b92cd90e4385bd06fc4404e71005e12950d318a6eae2e76e7aef1aa1f822540d3f499d73a177a" },
                { "hu", "955b12b3c465d1d929e7aa33af3cb8d7aec113b6008cb5c065f16efe69dc23b147cefbc97c9a8e40125b70264c9894e7bf558e98de5571259da6fbe6428a3e1f" },
                { "hy-AM", "95d5b36326709a344f01d38b9b2657a18808f002bdfeba18353012dd89f7a4a2443d66953bb480aa1ec523a700027abf2eebec84dd99feb0f6bb27308ba42780" },
                { "id", "974e64797d7ef7fec8d62f42d2b15019de0485767b1acd23d39c411bb7e4494453be73528ad388730094cf85fe7ad45e264dc642764a025fd3987e38670284a5" },
                { "is", "da561f4a2fc4bb7ccbb78b0bf3a0db702128c720e4212440084fb2a789684f82469b59a083138331dd588fc658821078cd70f414bdf643ce6513508d30ed81d5" },
                { "it", "8dabdd85e14f89a05f686a82992081537f125f448c3441f20bdd4d8e03b68e499d3bc59f0738b6b3497f7952e0e63efbae5b39bf8f5ef8172b6c2629fb22d567" },
                { "ja", "ea191564ed7b6fa31af64f179abb223b16c10c2df9ab3ca7b44dd54596d16eac5f15396954fd2823507d4c5a03f19a4773ff1587a4257d0eded0d349a0284d46" },
                { "ka", "3b97f2f40f83d00d46244a46277768eaad07da08b581de481c8a39cc94953693df53cbac13d3da468835768536c8d99abe74e23d12a32a12761a1ce775a2e645" },
                { "kab", "7579a00b413e967a1dfedfaf75d0b9452a5dcc578a786c7a35b92a6eec3d6f3b65a5f7ac14fe60534ebdaec44bdb5e5975c5e829a38ad14acd75f9fa8b971496" },
                { "kk", "8f27efe23442509bd2163b2826dda0a69432644b0479aae9058b08b253272a74842eef1b6bf5c157657a0f8f1bb53b7ee592c1ccfe18aa20527328f0b94e30bb" },
                { "ko", "c082cbcda6a4239675462fa2448d19f9d02ddaf1603ab6c271168111418bbe2d77eb93bc2ea511af18abdcef4efc7cde2e99e7c477bed84a8e6f654624897b3c" },
                { "lt", "311c954c14001b472bcd1aad4f5ba939e52659f38dd5e8b93e791583b19798357467d9c94fd473f5f4e6446b86f4535454e6b114a60fd53af906d005429a7ddd" },
                { "lv", "e8b0987257c4f9c5b9cd18e747f821c16e3698a2e298bd17a07de35ed407a4cb57688590696faa005a3bfbfa39c5fb9a0d2549535c84b3f8c0d71d5627a5dec6" },
                { "ms", "778d785aed3de0e6daa12cf73191ae4db97f32df6d5a42fb8503fd69d4ff00a349c2c5aba01f6417991b37dd7c5c40fce98fa88ab5bf5dc4b477bc4fd989a367" },
                { "nb-NO", "44c09d3a62ed903212bd877a973089c1d5db6ee3c2f7f41c194bdd46b392417a1a753ee109fdb595f97e5eb7f24b42b1bddb31326726e6db152fa33287939eca" },
                { "nl", "444a30ba0acc632e1d17bc31d71a4a597a8f9936bf48ab3088831c00a2b513335edf166bd99fc6c5812b4a1a740a743261a2dbfa1b2cbdceb91bd49abca1fdc5" },
                { "nn-NO", "a6a7b881ce0417fd0bfc5b42d6dcf2e59836f5349961590cd9d42ea6d053dbd674396d365ee9986eb753a2780e1a2a77850716a9e457ea8b9923010ad16002f1" },
                { "pa-IN", "90c361dfa51d27a4ed8227c4fadb1ad917a680ecb4b541f603c92499aa3e16f22b5d321a3ffa25b4e9ce699d382a42bc6ce607307a27444af8e7f25f5a8b17a5" },
                { "pl", "95533fe2454b44e9aacaaf26c200b0034e117c8923128fcc3bf809d8d10e695e860a3f7dbfd0cdf49623a6c4d6b3e795cafbcc7ac78272b1aa67304e02764eda" },
                { "pt-BR", "fb50f7594dce9c20d3b6fc302f125777e0e61de5f0bc69f4dcdc12c3c7c05d017afafb354ae6793c4a0001c4f8ba6badf1fc2188239dbd504ee88618476c09b3" },
                { "pt-PT", "a696d3f5dabcad90284f074479cd00c4565e442186d39c491ed0f67adda6b7959789b1fecd5581b861a75c06e617aac8384b99443ca8f386ab822003722014ee" },
                { "rm", "50105205ad9ccb25beb3e1602b624d0c2ef2baa3edc075ebc35ec11b599e8f5fa955e09e58b18bb8f464250e1c1eb6a3a25ddae8402ac28349f57561ca25ac23" },
                { "ro", "1c8083a99f70c01ddc6a477c4d31a1d3dbac34fcef2fd95112ea8337a3fdf22818ca4b45c760c8b49ff48248a1307f0554c621f21bc9baee9a6948206c92ed1c" },
                { "ru", "bc4ee59185b40b30405437d7d0dc4f3140af8a09c3192907a9b2aa93cdfffca299a63bd0eaa645155791bea1cadc016abd980a1d9c47dccbce2de0dac8f60a2d" },
                { "sk", "8683fce3acdbc09ac3ca537784473349020991ace954478c2dd3880e869dcf9e32d44c2cb81f1ce03b73dbae65d6f605ff6f2ad239d09e814b9e3cc108a6064e" },
                { "sl", "1bff0a2972004c644b298324a2c92ed2a722682fb3c66799d2bae3abb343f6d5e4e3e4a5b901ee701859159f18faec5085d8f562d71884933aa0fd89c0b0c19b" },
                { "sq", "51038076a5a22cc2724a90a273af406de0fb064511f0be7c8ea767dca8204123fb14cb6b44699e822ae65fb8dec6a7071d4f679c514db521c7bb429c5f296161" },
                { "sr", "3fe3a803a72927ad4328831edb219a7324242a1c6ba458d7bcc9388e61c4eba5b2469bc245326a8e46513a72ac8f5c1c874e2a9bb633a73b54b7b12f3ca4ba33" },
                { "sv-SE", "8b02be93848bc5f75d845ff6043ff6918752d45d445a836a9772bea346281bfa3ca4261450713dfaa9a9a85be1b47dc4783297311e265503d9b9c2b4bf6ed343" },
                { "th", "7224ff7fc7ab0ad8b79dfd439775582e71e4981a97965af23b1776ebbfe5c82762c766c61e758ded0d8b3e4bfbbb51528ab20dd9d708811f4d12cd5d4a5473aa" },
                { "tr", "d15a6e2afa78b1a4f9ccc67bcb1dd65fbdacf9881e8cc78496ee94a467fde47fed85bcb91a0682269d8f6360b44b4da9ad8f13addaa62dd9ff1d81ff88cced81" },
                { "uk", "34d8ad733bbf38c261f371203c16a5e7a8f84e02e0206dc9b007c59a8ad8b4eb5851016f4324f741962a2586d665698af4a9007809d363fcf8621337333bb7ba" },
                { "uz", "8b673e699bb30c0955a7d4976e6380d796a9e4d5b209d4e14d70a6528a25d1774e1f90463624a455915d61899dd2579308ac83e5136256e7c2bee6b552feb562" },
                { "vi", "61dc3b1b0f112d6739887c24933a4f469ae90bea3775fad9e543f45a631f5f801d6c208f7a7f1af8da5bdc62a87c60b44b9b14b083df0e1bc17c40583cd17571" },
                { "zh-CN", "ebaa4570f371c0bf5996d8214523aee17cbcf452c5f53774629f108c49cd63247905efcb3da05a523c13f00945968e87ac809022cb5dff1b26b294f357c844ce" },
                { "zh-TW", "eb09edefdb013bf4daf1eb35f0a330960d1f6c6a82ef493edd270bd337dd033b00a001e23c65e9ec4ccc27d7d309c18f329473091a25507b63886c6ca39eac9f" }
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
            const string version = "115.6.1";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
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
                task = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
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
                logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                return null;
            }
            // look for line with the correct language code and version
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value[..128],
                matchChecksum64Bit.Value[..128]
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
