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
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.7.2/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "321f17278c0ff3d238c045aab7aacfc8b9f72085cf51b727eca1ee584833b91e2d629b4d6e2744376018cbaf1a6813187fe8314776966abfbc1cc4e878162359" },
                { "ar", "b9cd552e32f802799584b0ccbc1a333db98dadf8c671de1f05f092202a82d2720f0ad41a67b9ce27c57df21166402c02f5e3152ff3674839c934e21524d01a37" },
                { "ast", "4436b0896fdbbe8001a472cc4e137dd1f454020383a82962088b1dd14c8a3b003f9ad7783c0db80bee27c2ad9b2a7055b8dd394dee69e4321991292d638b8c72" },
                { "be", "fa6ec3a0e36540d7dd49c937f55b6a88c2708b5889058b86573b77d9497d434c66cf967ddfff10204261673dde99c047a4aab54a221225ce237282765afb95c0" },
                { "bg", "034199c8b3ce4a51f371f997c2c68f5376c7b8294fc319dc83586aa99bb8cd251cb57ffb0a54668319a174e56a3375df2ca4ee00e1cb8774f14068616d2d1e02" },
                { "br", "432dcea0b7c3f18c37eea1b8378b04846131e246d3d7a533304aef99d228bcc17d51c795362ad32c4b71f1b0a3476518b912bfeaeb81d4e2905c6a7eb073b51f" },
                { "ca", "5e84bdcf3a15cd5ed97d37c2b99edeb35dd5d0ae84dc7c31acf394af51a78999b5f5ca67327ac226f5658a8abe721f129786dc9d160739df1ee4bc718d8cf0ef" },
                { "cak", "12bfc552762c4ab403fba1745b398a0aab528d8820f6711371c7a501e35a00af99fb94ccc60420f932e732da7545d03d4c2bebe488f8ac18d21264343b0df258" },
                { "cs", "5c3a0f5f35f95dfb453b270051524adb5c17171e4aa44db55176878d0b55e7f79be12a27c73fb6c5366d8ded7e05440cfb1ba1c032e8ba8b6b53485fef80a74d" },
                { "cy", "5a73aa296af3f5f3c1f7dd1122c03702b3803cc4ab7eb337e16613eb2d1d94d450ecff79f9eadee59403d32f63d9fcbdbbe095a4137463584b9f19b501858d22" },
                { "da", "c0528eccdf0f8e8328268022fee4b65e72ef03b0bd4d513abfda94a8ad88854a410851b78280d363d0055e5fea10cbd65e72f01b891b45164ac11ce33ee04967" },
                { "de", "8bf35cf6ef8dec959735662aa22b0ef21fbd6dacb80e2dc1cb583571f49f261163ed937735124b68de264d3262eac93e334abf38737dfdf6ccbf150a5bbdbefe" },
                { "dsb", "b52f758f4f878aa5c134ff6d95383b59feb489cc548d47cca2e3c04411a526667235c05ce9817260fc773344135751fc2a56832d09cc46b26c51f1f57a800318" },
                { "el", "dc0fc498863ad3d23de09609b30fbbe1ccf521170e5837b6c58e3060d18211082b958d3cd888c2794b193ee43236e40a9341bc66a2a748bed6a3f01a991e0e07" },
                { "en-CA", "fb1b843ed541c101b5f366dbaa6c8fbb5f2de51f8a41ab8484ebe86ff1a4d01e998bd83b8cfd4433051028e8dee3efddf167421a21cf8331a7a4cfe32560c8e3" },
                { "en-GB", "a31dc766ace853daa86c6aff00712fe305cfd033dae7c63d9dcac34d29e45dfc117257a69840f12f922a27dc133a68a6bbb6dbfcfd1a956b4cf357a06e46f2a4" },
                { "en-US", "9d635615c771f780179659f426856e5f0013a4a7b6a5016b821ff6c856b1b00925491bb42922c9bf5c4772c9cd63e30c73a83b4110c55ecf3d48f77d05755364" },
                { "es-AR", "b5e5898c30b4775766f22f32502d6720d6bf682cc34247bdad8c35b21440dfe4d63fefb8e1fecb7326ee7bc98f43b56dd7b3b485adc7548904ead1fc2064a66d" },
                { "es-ES", "ab7f34006882fa619820b2b043f4e4d228ef2378adb3071c42ebeb6574cf12131bd1383ba7a156f2510319a01bea1f6e8c594862b1e47d23253626c016f13bdc" },
                { "es-MX", "8fabdb99d267efdd559618dcd0b14aedcd651c4b7bc44090716d549d307b53ef979e5ba6c1ff78559499153b248c4111e894b187f0edef868e33e2c8084aef93" },
                { "et", "f3525403286cc7efa79f02b70dae48a210e722f24d42e2b86c08c29c9fced56e7ae0086864c4a0a64aa05da382574460ea97d63907b32596c40040da1b0cc439" },
                { "eu", "19f172ed5cd5ee0ac59ce0d7db574e970cbf22c5ae8b102bac3420f8ae83691c6c2cf653c1bd071a3d5eaf71acd78a15018b404bd9ebd366548278470b89b162" },
                { "fi", "354928afc1d650a5723be0517a3e00f0f6c7d6e99d46c44875f26c9119c0e731d70e9151d71c50aaad103ef5b7aba6d6179db8451f1396d82c921a13ac18a5e2" },
                { "fr", "cc5bf18a3cd57253fe2983645fc3b2e270f69bbb3100392d5ab7866259362e5d76573a5dd8e8c94cb765b297cb748c7b4c4c4fe4d4b1cc985ca7bfe259c333e1" },
                { "fy-NL", "39b1b9d48c044202e508ef617db3f92a35791ab001a2abfa3dd10887a50b603301a97366d4739f82c66b92d02c9b89a782e8754564a018753f482e7ef41d5cf5" },
                { "ga-IE", "3def1ee63ce18fa179bcc074de9f1978779d520f3af2ead555eb3c958d6e8d7a95c64bf3d605fe5fbbd9744e772a1fb36494412c4a48f0c6c928398f787bd982" },
                { "gd", "d7b34914250a4ccf513373265dfe23abc2a8154ee7ef221b7f5b51e55359943308b25eb5f1e2928098873fab7bfa84b35ca3c73144b7a102bb056d315d80da85" },
                { "gl", "2283529b83a7e8d61121a14c7d10876877cdbe3de986869cf0a736f7ec37d7017353c21818354c93e0f6184efb946763a3360bbb50eb995bce45149012d73c77" },
                { "he", "50ee0622ff8d60629d12aa15e24a777763372c242afb6906fac3c71cf4c6433def456a9090726850906472d6d3786829e3b6e640ff92e1902def5b1fae9a3350" },
                { "hr", "39586de09f9e75a6f1013b6794f4cbf0a9108314701ed1321e2f03d0a61c3888ac0d174a440d2305bf070e7aaf715e6d84d70d65aee4922ba4edc0f208870fdc" },
                { "hsb", "2fb70ba19e2388c25cdeac4bc65bf5ec9f39f4089d1f05a2f0a3821eba682e011dc447d1598a5f7d5b77baadd74ec37b080c1815b71ecc7e5b2d09090c1023b4" },
                { "hu", "6babb233d36b6f9f6c37278fc01a02aeecbc4c80569f021f0dc4564e7a7426dd8e189b1b23540ab614a30d878f9eabbfb7351f9864b16b25c47f16893ce61004" },
                { "hy-AM", "c700f4440ab317386efee4b182e73e338d784c68ed01adb64f092c00f10e1124c1b4708fe56e38d1e97e471d8bd3b114b277b0c2da6fad342c048dbc6fd43fb8" },
                { "id", "960b4e085fa2b59efc46d584690c050c7bbc01c3ad0c5b28b154f5894da750099f1eef7bd254ef05e12617b67b606ad1bb742904890065e581d53f5fd3df9c67" },
                { "is", "56637d1acc3e3a5665b6f9e75dea333c5ff810fa0772f135b0adfb7221868607feb3b1aa213d17b4dfad23504a9b4a76ad127563b02096f9ac1646b2999bc4cc" },
                { "it", "b60d4d7601f28750ccffe8ba24c22f35141840ce390eace105f8aac0a56803e9c833d5450cf0835fd960c163dcd0e61da4c1d104eecbdd33e192671d407e6c62" },
                { "ja", "68086b9e35ba600381176558e99be09c6deb6a3191b1549c6a2fbbf0cfc6d19688284b9078378d5f94e0b48fd03cf33c8c9c2524ea06f2472f9ba444f5811453" },
                { "ka", "af0dfc2c7f641f0ebb7f0b775f5b94430b959f207c8ce2a84afa12b9f20cbc5ccf46db6f2bb8ab74b55d650677c5024ac4caadc545e979e5430b365440df15f5" },
                { "kab", "63f5b3fd876ba3d40af641a50be40a3851657de61935dbb1c57969e5d853733060c627d5b028d1dbf806e2829edcec2d70ffc18b7f6e247e8bc390da58a9e3e8" },
                { "kk", "71e20b05922fc54dcc7d8d2e9d35475cf0569f45392cdb40f0346c11602650df0e4cb340949754185ee8d43440e93b6356ecf147f3c53611bdd39fda83de5f59" },
                { "ko", "c0aeaaec324bf7a72460ef192a6797d66ac82ef09987a1865a728aab56a3d05b6e170c3fc10e50e363cd618e1abee637e92258b8bb3acfeccee3263ec5540e04" },
                { "lt", "66e6c4fffd4cb3478c43861795a53cfe4f79b7087583e4ad023920bc6f8f0bec6c37e7a7e368390f20ea68b1acfb994e414c6bb8394b124d0738f710a92d0ec6" },
                { "lv", "7df545f8c763217d971c5ad2faf72715756524445babcf8a03d46e764b12db1625bf808106c85371229e7000fa433000d5c2494644b68b71fb0189a0f3ee0fd1" },
                { "ms", "1e8e3b663d54d8be398a76cf8f0dcbebe7647b0195f6537ea34b383257da5a06a980568cff0418cf405af7c29acf718c921c74a5f4551a9b5a81fdb73cc7cd09" },
                { "nb-NO", "10ff395a915dd662d3b8e653602a6a01ca0428da4d68dfff07f3016c0b27eb1044ea74bde35f8df323a70c16f88ff35369f6352f22a95f6796618ecc95f133e0" },
                { "nl", "edb20161386a3b416ceccfa13d0f391e5df48309b92e392628c73f59fdcbc30773fb56b1a64043bf540e76e3ae4f544ecbb0ec2e503f4f0eeb1872af293fd391" },
                { "nn-NO", "254cb29704715df7d85121cea58261baaac4928216ce2320533b879153f5a0b571e7c62cd7ae2c1538a0b03588eccba86fc1af9eeaa73e925ba37ee8dd65c620" },
                { "pa-IN", "3d3f0a544639dddd615122e64c5e7566a01e2b111b5b335c61b68169b53881d3e39e2a4a63e79804e3f90ec6fb99f35839259a7193eb89f604a4f69effc377af" },
                { "pl", "12b17d3522b8255e9ff11c16379570b0f4e51edaf3ddc6198d0d0d3305bc74792b28751db987ce4d1280a7630493f04793884e1766daa5d4d98d8a5dc23771a8" },
                { "pt-BR", "8b623660aeaebd092438fe4813e7007933d88bcfbb87c4717554b758ca6994ce0847ec735131e7998176707eb12e7e98bb0b4f4b9a67645105fead3a672f09e2" },
                { "pt-PT", "b5bb893e86323cb13807d0f3e64bbffabc93c98585207c24e535870b899ae6d8caf70dd365dbebd0ecfd8325cec357fb75282fbfaae2c49bcbb944889c385f91" },
                { "rm", "b459ea048023f4443f624558b104d59e04e67bcdcc2a83147f5289ea1af5a5d539587f71e317d4751215648e71578ab8026e5ad5b5ea0a49de1399f477e88ab4" },
                { "ro", "b0eb373b357a7528271dd37e5865aa6e5869372b667dc83adf83cb553248e5814614117345ff763ddb8eeaa4af2cf9f85eb02527826070ec589c6cdb20995c4c" },
                { "ru", "8c50c26c0d17200c08fe44561db9bba54afc3efd4d7e8cdfd6fc0a838d8ebe0cc3fc186f1729c822b6968cadffb62752fc241e4437f10374c43b9389b4aa4629" },
                { "sk", "5c9e0cdf972180313a760c373e4ef1c7c48727479522b109ec74e64d85b54b3bd25058b91799f9dc16e487e2fe189b247b86cfa049c602fc66ad5c68b7c85301" },
                { "sl", "540ab33aa66664931765787add3b87d77283042411e8f408fa811095000080c102374f475a061fc085cbdcd9c0d0ad197680061c516d559fc924b8c3253ef275" },
                { "sq", "80b51c9f080167b9180a3040a9a4ed2750af47cddd96780d80def3ec23931f374588adfb63835498322acb14ab4c3e8867b8d6d3a470853e6e6362d5c18c8dbd" },
                { "sr", "2011d1c71fe5c44d316bc32f69fdfc2ce5a1a7283669e22cb337acaa2bc765f66ecec1178494755df8063bb3015083c2b92ebf1ffb41a4a470bc7b59a882153f" },
                { "sv-SE", "764f58589aa772a2b025ff4a5217f0a2eb58229403c2a34a0cae2ff5d6620056030ad22a57683cb42e5c95b7188eecb6c45850f9681550686a55c95da44d4fe5" },
                { "th", "2038a9e6facba690b9abe37a4078aa25182ef3fe362d2d2ee681b2a8a70261e025f6448e4ecaa3971ae99f8a1578a1348d71ba2bf6d0e9162ddf97a68d79aa24" },
                { "tr", "16fcefdba938645cc97b7e92b56e06a9983dbe4656cd90480f7fe964f79d59174ed9022a069a77e1af120302929e084e954db13755c8de4be91462bb21eeece7" },
                { "uk", "f6eaa1bff6572fb7dfb720d82dcfdd65348576819f497b91a75a267966074989693051a37b35823630521252484dd1fb1b3ba90c02452621dca889722b0a2f19" },
                { "uz", "01f8a95a3ae022c25545895bea2b0656cbbf46260abc24da66a7c71ce3da771007faced870514e07fb58496b300095184a5c79fc5e5bb88f6be6476bd555e57d" },
                { "vi", "2ebb3a3b2a9a4a30c8b020909fcfd5cb2b06384d361a5dd1bb5a284306541110063f19886a4e7454eb46e995135738185d716e4032ad4564b291b58d7b392ad0" },
                { "zh-CN", "60219c8cba4446149cc656e070048e9cbcab7fab610062189d82a520178a4df5bee54971faad4f0a3a93d0a727d9da9db3e16a218751f89de73d756106121d21" },
                { "zh-TW", "0fd26bf29afd08d3aba969e4e8e4359d99b9f4ab2946f6b02d676615a3cf38dedc01297298b1bdc1ab5d20440836baa7ac0b4348ed41378ca415bb2f99ac37db" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.7.2/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "1f664a008b43f47fe92f86f7aeb3c4d87cf87435e4f8a6250f5e8a82d3c660d9d8dbce8686064a44f6afc1e7a22f9fc2802e1705b1c543b7cb1d926a9f93a0c5" },
                { "ar", "998e40bbfdd9dbdacd233070da03e612042c9796e201ec7d861b42905db50a34e56909f91bc7af5103a374f65b74e7098fd0058f0bd7d290d0d9c2bddb2d5e8e" },
                { "ast", "28330a059b6933c972f441353df246e08370faa7f1c888ec93ab3d2dd7909548767513971ce3b65053169c59c07ea18cc4485231636fd23a2bd78a565241102e" },
                { "be", "5fa8b6024919cbda6e797f3c585ce70c4135293e69182ae0c87b87b78d464f5c66dd97bd4c5afa8a1ff041db22649bc9fbd9d268567b84538fcefe2564908df3" },
                { "bg", "ef955de3f180016ac1d7cba8061387d07f2466d9c03bf43081e70c2e2bd72b44790b538caee65586e54a5cf7965c003dd3961ad3197cfce9b0d0782fef9fb0c3" },
                { "br", "d5b94f430146adb7dd307b993bc9594447af523c108a32be7455006ca34fcaf18e99c7b4cd444a7cc16633656d1634b3dc871c1e2f4bef70c68ee9e72902aa79" },
                { "ca", "4909ade6fe966de6bbef4ffbdb39f7ee6279c58edf5b84030fb3499d78cbc276aedcb64ecd9234310bebd37a0a383cf8916d78dacfc8aee25d5753a0079f4461" },
                { "cak", "752faf6554e34231fc7a526ebf0a0812904a5df2eb0fd657ac51a995a032ab69a3642573d381c616773ee5e04992a64c7247837451606c54e560b5c35a98fc9b" },
                { "cs", "d531b57e4530b2ccbe017cea615d65a9c573eaf0e3235e491f90881e301ca18f48c86a8291b949ff3db0d18ff92f0aaea8708f7265eda11cf7a0e9b9efad4b89" },
                { "cy", "267bf3da737cc6ceccd50b67ffedb0427aab1e58204692b1b79531a39dee331957f79b2d35dcbd22f66c57f3baa38b6ad655e6130c109c8a5c1d1fdfce753c68" },
                { "da", "0072db5447c459f1be4b0bd506e7e00d89e3eb9887b49a2f27005fd0d35927854aebbf4b69c1fbdc9a857e5cc33842c33416ab3c96442ae1b29f37d6b1451f91" },
                { "de", "a2366d6dd741e2a310f9b65f4fde12642c9b34b75469d297325efc928cc7e25e5d12657f717e7f085b8a8aa56cc37de6e80528cc94cfdf287f1abc4e3fb6477c" },
                { "dsb", "0890e1ea2d61d1fec96216b2fe67757af535de0ba7f682d37dcae6fd4a5ff97017444360aff4605f83fca66de51a5cb8c5d01d9c68da3683cd45bd2c204655f3" },
                { "el", "3726feff36cef818749a64f346ef292bd52f382135c11dca7fb1be5e6c21b9ae741c08f954d33d4fba87a3f53cf9bc566dfdc36430b0a23be3bce819c123ed5d" },
                { "en-CA", "7bb5d9df82811d60b1ece9cdce76168d9ebf5bb84ce2baee6a626f07bd8a531dd6e0e866378a164df7711e5f2c93761bdcc7082e329534a194427c750869ce4b" },
                { "en-GB", "ff6be745a95603bdbc7e99d226b88f793bc679419d4526ba49871c00238c96dc8cbaab5e899c256b0a2eae1d880fed91d437e82a512c9f7190f14608476c48f0" },
                { "en-US", "f88c5c8d171d4a7be783aa32a9c7e0d25ec538ffd8e7576559e685a353a3a73b2b77ab509ba7ce8e7651adf5c76e5a39dc9eb07d46df607b37d62cbe59008be4" },
                { "es-AR", "4cf55ce5a8071e5be19d204b092cd1039e97f5eb049f908abef031bb363ecf79f8269ac824b2c7422792b72237f7527da95761d44f39c2cc86343e3a8770adf1" },
                { "es-ES", "80c59ea498322564cef642bc1c02a9f609d0a75604f748b2cc3a352b6f48ab851d166dee19a28172746d0616dcf80a3052781b34edd25ba75bd79d363ff37b7d" },
                { "es-MX", "26346ec75949a4d5045fe510db75415f126cd953765dc1eaa918b2206b578c164d16d819f802b48071da6a5253cd795e27e4869e84e82c23536eb46e08ba8b87" },
                { "et", "3895929c0dda7b11f42b853650d7c575bb5753679a181ed4d6055480ec0641b5f8419a3089846662927cf780af02af4d4485585018aad63a91473b77fda53883" },
                { "eu", "cc9127cb581f056c1777844ead45787fad18c2ac3a53ab9f2b30c6168281ead0758a8c2afbe980a79814a0da6bacc72ab5c24ea04c4042d7ee53d5fa7197eab1" },
                { "fi", "65ce9b11a4ac6cd39f6653da9dc1e11ea27dd8e1e4bd28a55702b6b8005464ba37db706b7d3745da7b2761eb284a026056745daa0ceab0cd2592d5b2e8b5b34e" },
                { "fr", "c2eccf90c7b5e3abced8c8a9ab49b2fe9c9a96fb90b532bd0a39862bf92e3e6c38d53509fbc188b75b44018013c93965502638d615b4087ebd062edf63f9301b" },
                { "fy-NL", "c44914fd4b3722183a94239d385bc40cc488c7a33c019fdcfa4262736d632ed48dde98ee05f2ec9c7a6529437dd4a8bfdec10c649fdc7c5bf434ec97bc974a67" },
                { "ga-IE", "81ecc2a099000d64e17aad200cedf23d5f3ea523cfb2042e358438c33d0925fb51074c914bf68c00af5dc64f16577a5a57f324d51cfec110fb9651e60c12e5e8" },
                { "gd", "9368c1723980186c6ddb6be5828ddfb5a460de073de9b8ffe6822a4848ea871730eae449a6b5355c99cd269463148855d631a661d85b9e5a8874ab9d8ad6290a" },
                { "gl", "58d233edacf9abeb6ab3fb6fc3ca70cf973b7f0f718e7cfc80ea2070bc9afaffd430ee0f91fdf2d7c939945c440447bf5315f8eee58e5a944ec88d830e04257d" },
                { "he", "00c1a0694543f353c6f93ca6db996c3875a470728345d265a9b4918cd6b9709fe44a30396f035fcf6fe9e606d2ab20500608038354d248f59b78f2c73d044ac1" },
                { "hr", "d911bf9cd21cebb7fad6a6ca160db63c7c54af4d3986397d00ec69d365dd90b403bad5fdb1b847207e3d15a88b341f122b1751d0707c48328857ff566b48ac97" },
                { "hsb", "d558bfe97e01f5b0e927254b8bbbd6eb1d0150adad9f9716a91db0586258ff769067b46d6e459508f0a1111850e3cbd9c937f3a3e7346694a28f6552d3691cf8" },
                { "hu", "a8e9ad166fc614594d047be20806096222f2fde3499d33e99d7ceb4fa950bac7096b56c0c140f7dae1617824bb5f892f7f6b57c09d2df637b8d5bbb026f0b126" },
                { "hy-AM", "bd9b6421732e113fe2aae07ff043be6986dcfda947e6d8ccf645cf83a416247395e04b4b0700c446f5212c5b25d8ff82ef76c63d34a768b2207a299d6d70a8b1" },
                { "id", "2fa034f08b1691c6f344fe1c75696a748d2ba130a953c83a55cc1530935a20f68e24606c108a84aab2c22d472bb75570ad888fdb9780904e3a151f82c8755300" },
                { "is", "71328a827568df43f865c4f31c4bbfc991175b9e7c1f24f2573eade7f94e8cf0b28fd7bfe78d6c314a884da3bf70272661809e2869dd052bcffc3d1d6bfbc0f5" },
                { "it", "a3e9539613e0ff20f3741866d23037d0602019ac6886d3e0046aed3a8884f661d819f03f989740283c5570354d3ab0846d074cb08171f97cdd62424dd6aa9da6" },
                { "ja", "42d6b6aa0c24780c37820235975912c8146252601e3548aa7e6d40df17a79b77eb4fd2a44a98a00280cbb81544fecb72ec012bcd31479635a86045ea9620e29b" },
                { "ka", "868c137cf03e2915f35dcb7f90e3bd29a820f28f824f6b8bf4027b1402036c7078a5ea2ecfbc01248e182b09092c36e9c8302fe820f82ba7e7a2237173f2358d" },
                { "kab", "678fb854236a48ecca4b8b17cc5e72ab9f2a9f4f7b64451aa6a69039186a58dd6fbfdcc04d35a0d00dfc4c3120d0b20092bb1ec2f82452928ed72351dbe5af68" },
                { "kk", "24a1b570ce1e093e68bf74c625d68d4ec6569c3726d8194a6401f5354efbe9e5fb0d520939260f729efe15ed251e98881c95ce7c23ec5dfbface86be8cda7438" },
                { "ko", "3dce57ad06b860df22aa4d83059d75d7921171c71c9532f89b407d62385ca2553ccb30ec0c255d95e11fbc96ac22ed3898e18b6cf7f31ca7c232057a1203c431" },
                { "lt", "fd3991ccd004d7cba6dbf0b2f691810e3298affd477755cf924e9c068e1376b2f40e07c908f82af4727d914c2ba276f39bfc53303085322ba5be8f7f67fd575b" },
                { "lv", "75df6b4945fb95a9dd7107ef03eaab539422cb5e8ff46468d6faf28e428a1748877da82c9c7cb465a3d4fc87361578eae4f03bea4007cb464b5bcc9ebe20a5a2" },
                { "ms", "c575066988e047a915fed4c8c8f6481c6c0c1f869275ac2b9f80a396d64ddd09bfe76e9966ccc0c063aafd8c06d036ff2808543531721b009ef3f9415d7d450d" },
                { "nb-NO", "c545883c67affd65d4a7969cff0e63634b902df994e8fa4749d4f2a929754b9a89071f39a7cd564cad9f8408c61608f4613a27e66ae7233bfc56a8645d0881ea" },
                { "nl", "b02d79f2ff6643575bcb8c77ca9370660777179524c9aacf51e3b67c9e87718f62e912e1a314143ba024c7e0bd9311d40ad6893f975858a9dcbec31422a192eb" },
                { "nn-NO", "49ee8e581ec9e06747548324d5d7f7d3fc5da94be512dd9500da2dcd91273dc722f29c32db02e737b638f3f7138815f05a79a1183f64bad5f9bbd1019c0abe6d" },
                { "pa-IN", "0e69953a7a4f91fa7b022ddf08d0748cfb40dfda54e299375d7f1f9ddc4ca142de8db38c82fd34ad2021bb4d81329d6502d7a4639fb037f4116226c71b2504c5" },
                { "pl", "54223682f424b219657399c8b437b35f2967ec085639a7b997ea4f9ca81d89546490e1f76b125312909ffe9906b4ed5a77539ed527d1e4a0fdca99cdec175841" },
                { "pt-BR", "759116b404ac6f1609189fa2f39e0e6616fba55f25d9a80c34b28a3a90ae1a16568acd232b093e3fdb343060ee9838b359f84e03776640eb95c738ef1e59587a" },
                { "pt-PT", "2c17e4ac095500c82239f26c46b310e609dd131f0f462f75e96118a8c656e1e82f2e1e549de31dbcd5ecabce4e7856f09a48939a626667dd96f8af74c7acac91" },
                { "rm", "2eea7ef7a1000e54a2b321558cf7673b117c40524ca5a470bd0cdcaecad2a980a3e08dce80fa23697e6528edeeeaa0fcd06dc71792828973ed279f586a5ffa7c" },
                { "ro", "1028c440257ee8c06e42f3fad584be5a548dfd5f143c1a84c2b6ba4fc7b55bd70abcc605935cd0bb9c4da26efac1ed7925aaffd53705b424c3c5ab9ed86755fb" },
                { "ru", "4e25c70501d51d4bfb570eb9eae0fdf73d99ae0326bfa38d1e0d4a57799f7351bae23d5998bbc2bbd04146986fdec80ef9095052611a6ce7bff4fe5c32d2f14f" },
                { "sk", "fc76869831305d6b1203650bdf061b9143bff7848d972bd55340594c746cc54d0b36afd23acf5c26a8b08b131a189cb33931a2ae4132e9dbb49352e48b9cc0fa" },
                { "sl", "94209c14b5d21f41a7ce11672551e324af44067f5c919803f36b89d795c4479cbd338b34fca820af50647128246e4fb53c8d1a76d542a77b9f7a635319a46eb7" },
                { "sq", "ef4b0b47777a859570b0d3876018f199310678c9e139c706a1baece862735762bdd3543669253e8754987b94bf4324f675b577590f01012b9b18d21ef1c2384b" },
                { "sr", "c4e7308c04b00a85610df082817718e82cd135a5676cab68dc4971085ed207d96f330b842de89b26be775ddab10b858b71684d8885b71dfd3280e19576672983" },
                { "sv-SE", "9546e8038ab423f036c9b3cf8d451f6a2817b9ccbfa1bc7c7ac8c7c082ce6ac9d394eea0747beb6f0cd2e19d43f220d46efa6222781ea9cfad026e8e2c4f7332" },
                { "th", "c48eab60f3d99f41551e91290c6a9e0b036abeb377829a51f2837fda9c008d9ef40718197716c7245745a61f72befab4054f9b24fc0f1fdb016050b94df28186" },
                { "tr", "74ac9e459722d2a5286a635ac9202e9343bc96e0fc437ea46648aec1fa2b198afc980a6f37d1afcb9189cf1059c35f0fbcd60cb6c7eb24d54095c143f23b18d9" },
                { "uk", "948242cf0994fc077366ddbdb93862256fd1b782572403ae81f7b22dbe7308bd0fb0bda147754458350fce6e11967f18334f9c6ac06df0eed3a2c6d3db3032f1" },
                { "uz", "d0841cd7633e92f99d06292f2a6496b286581e686e2c5f9c4920de6dd90bd8d4d71df721a5b094db71127782a08d1605daf38071bb37d4f4375f84cb20ecc54f" },
                { "vi", "18de2363640e2fa02b8f3fbe36b6c5f445bf08fae32d8f8633f65ce35a3bf4a347442a0205eaf27a73dfd655eb599dbabdcbfa829b60494c586b25cc682ce2a0" },
                { "zh-CN", "e44b8d01499421e2e56c1c339ea1f092987b6afbca67257d806db2aba51986c40471968b92d7e7f9cbff77dd8e5d3944bc79a485e0fdfeadccebfee8da5533d8" },
                { "zh-TW", "21fb2c238a60529da0f9835d3ab3bb73da1f021cb23f8ca3c8a9f5be1eb4bba979db80bfa980a7c009be21f11deaf4e07923c1e2358f6da3ccad2bdf6de6b184" }
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
            const string version = "102.7.2";
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
