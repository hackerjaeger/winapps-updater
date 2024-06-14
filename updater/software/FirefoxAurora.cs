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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "128.0b3";

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
            // https://ftp.mozilla.org/pub/devedition/releases/128.0b3/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "c78396ef6a31ab631d3dd817ba1e86560687a0418514f8a38f21cc0be3011b87e660547e2cf023aed1d598dc3d8350b55eaf0b792c97b0e2f845a2e43b2ca970" },
                { "af", "d59d61548d4b63b41a19cb28cbb0908a305edd9b984423011b618d2e59ccc991263b567d15993edbc08cb6245cf296ca07d61a453321296eafdae8a7a16e89b9" },
                { "an", "88134e073560e0f35387edb5a9eab7fad4b98bcac887844f520954ba82cce0d406c9cd78f2ee7b731c1467ed8dba177be728ed5040bf83b540070a43028a1cc8" },
                { "ar", "ef34f1433bd12a85e297cb27353704d227e9526de7b0700e94299a9433ac184ce9ac5dddd6b4e12675097168162bb5431175fc46c161a5280cc82c84d0376b3f" },
                { "ast", "ecfb3fa83d0b8a92b561d422dd5c853de2fdbd03201c35b7fdb1bca4f6076ee619139059379669cd1ec4fd07bb0135c627922020c47301bec36f3bb93df4a82d" },
                { "az", "d74483e3fd841573eef7c8806b86ea423963ccffdd955bfa61ca62b7895f4474287119454bebe4449c43a8c1c549a4618c50fbcf37d472bc7e11230519b9e006" },
                { "be", "da773afde601a86a924bddedf53c8ce406d0e2cb6e07b3f75f353ea7a760d968509dbe4ccc170a51930501f86d69951a5051c1390f7646445f9a2d9da48388fb" },
                { "bg", "de3d9646f7848cf6fbcf0481d58ba572430e9c722d8d2f52d058dcc7439a62811e158fb48dc92bb19af0aa46db983438baec7d454b3afa6906b275a65a7d1859" },
                { "bn", "9664bf3c06dbcfff2642098a679b12ae4782e6d5b3a2419f7c3a39411f6a89cf8b570575a5d441679dafda72259e34acb24ff76f9870a57b6980b6adfca4a4f8" },
                { "br", "8a4e8b8a2a3825cc5be205e1554e0919e4c05e48b34bf9fae98d6885283c200358fdcabc6bd844fc111766605d37ad4ff2ced32a95ba327164edae7c324dd25e" },
                { "bs", "4ca9d4d641e1749ffa3b8682bcaaedac97bf6447c61890f648b3227c3acc282cddfe0a19ba725d2bd69394b6763709b98c02dec04d1e2b7f1c6e363c9f427b2e" },
                { "ca", "3eefea6916bbe340a5c1800e5f875a880820259f4ee9ef615faa80349ae010f887d90b4591bad399046880976d853cf542be89f530e3164084694fded401b981" },
                { "cak", "d1554a5517c1f36375c8c0b279ef5866ba6d73b37306545f3a1e81167056c526798351e07938cd94644a790af0fa8a9df5bc93e8ddb16a60539ed80533bf817b" },
                { "cs", "74ddff2ea304129202a9fc914e29a81251f805e4e510f25bf8216a373a6f25de8f80d99700edac0255b8f98561b52b2239b27b608fc816f21fcdaf0b2d2c9cef" },
                { "cy", "c5df394456bae1c84796b224cd88d631b4f06c3decde90a91082734c3b5b646bc8063c0fded6210324e0e0f964766c51b87e274e29fa4e229cf6800a1503bebb" },
                { "da", "0f2f6fa476786c1f1810d72a69d3e1f6bbda14e013aefb3bfc2657b146f21c1b8ca465e960cc1f5e34fbc5c8fd460c65cd064716d6233b131582c8c307184729" },
                { "de", "5463fdc72cd570a314d900593a8a173154b1b3efd6af5cc4c0a1b6d74189a76422918976f271cb44d31871f1facc70d495299129a192067d7cba5d69c4a48e4d" },
                { "dsb", "e861d7641b68820585e9dd7aaf93020457afb3fe5176f4780e23a65c8af5cd1ab08696474efb3b3292d89d8f171aafd94388c30f7e900e30969fc5d42d04fe55" },
                { "el", "82ee8ae7eeae00d51ebaae9ff0b3944930c6a948d741be212758b2c6077973cfd4ac1c217ee37c0ba185304f02a197c38b56326bc946215645fa35b23d518357" },
                { "en-CA", "f30e0e139ad9186c45cadbe94ef40fb317ccb0676a8ee180223f0995a309f97700908b544c3fb5acc22f9b14a318bd002419099b44b9cffd14d58afa837e60ed" },
                { "en-GB", "c3f8245f2707bf53a93a45e845ca4448fbee939ba4057d4b868a1185172cfd3d88a216d14724311e8b79030994703049b25698fec319d7af0f19e365ab141bf4" },
                { "en-US", "892daa598bdc4e01e744720ea8f671cb6286c1fda76c92d91ccd7f85eb948f5ad572aaf85e60d73a10d49aacc3a1aa38a1a70c0a08eda39665b0b1cfb0a2e713" },
                { "eo", "1ae032ee0590db4453877f082011a33af29619aa01aeff9fd8bd40094a9261046511f37582db65dc806a4ece4d2f6e4ead1f4c8714336946581a65fcb3a89202" },
                { "es-AR", "80ff34ca69c08ebdf132e2a0fdbb4f888a26fe150ac1522a42b799f33794bb0dd69b690ad4d395867f1b06095661292c06c1640249174023a1e6326c1a340806" },
                { "es-CL", "2b44c9a39771b68becb03cc3304f7598f87eaa93a1136eb61405950bb54a844cd5a663a5c03e3deee19145c4d36f8e88aaf684fbbae5728b5058fee821949ee9" },
                { "es-ES", "0ab749e893560f22e1c14b527a3596678d72b163eaca87fdcd21857347a45c010a278b26d1dfd7c41136fe3da427ce5d9766b35ff22cc7c45863af0dc1d0bc47" },
                { "es-MX", "217f32f322ead7b8cfb3f970e4686a35d938743d2d5c0545b570e4b4ec4ae53db9610ed16160e181ddb072f5f5a5633c97a5064f4b43d00fc68819cc617d38bd" },
                { "et", "5ce2b783b92b247040156f231365c8129d8a4f197c0ef383f8f397b4c2de1cd3058fd82b765242f254e26fdd3a953147428bbdfc20f420f2f87a88129375d8c6" },
                { "eu", "be1f83fc45c2166d3ee5e35addcdaa925f5476e9bb425fb126925ebb146e806887082604eb09133adef4b4489d892c33661e00b95f427fda7c4779a447ac4181" },
                { "fa", "bde6f502f67a1bb1139933220bca7c4220ef122519fe1c6bbe80f627c331471ec321f75b3343a51796bbb04ddf9c05c52912150181108d6762cd21dd83980817" },
                { "ff", "38bf407011d4ef76c0de3de1c8f6b1f4b03aa8641b4886cd223d8f426d31163fa0fd938b74e85c481c0acf4350921b566933bc34b66ebea8a010f615f9ff0a51" },
                { "fi", "95d97d7b8f2af6cf3b04b03d79be45616bfda9bdeecada297c88b615e3a85f04b1d0db098589007b33e73616937bc222228c2bab245585f2adfac39eed9fbff5" },
                { "fr", "de18f033b97c484376a9c347efccf4174cf18369e3025bc452805957649af464166cbe3779d08e16af2dbe9614f04d46ee64593b08abe43f3c8ed7edb2e99cbb" },
                { "fur", "7ef13f547df197f67944ad799af2508f8d90629cd6d848ce260cb7f297ec4842d16c5051e7cd2859c98308e0f7a4f1b7d5badcab12f3060ea04275ee4e3bb105" },
                { "fy-NL", "3035e7086df224a291e979c6f174400b0e99d5dcbb46da03c8f361ecf148f98c0bada96c1dd6df67b648a1f87a8887a6fd8a0e1f05abdd7981863e40a88e007c" },
                { "ga-IE", "7711b0a1dc3f4792ec61f419a39da04076fa09ec15ee52ccc0ffb5282fbb64678f3b9330fbf1986cca3853588d3c5c53dcadb27033d628a2e0dd35b76db7924e" },
                { "gd", "929930a1de227bfccbaa117b3a6f8510d3cab16cacdbbcad8e7eeb97627a2d588f00a3ce5d1945b8dbe06cda4b8128fcb2cea05f9ba5509a5cde5186654e6f79" },
                { "gl", "ea753a15ed0554bc0830f22bfef3d1ca3aa1d9f1b485686d86e53eba3d6fb3558a6579e63ba0aa3b4c9740ef38bd4e1832831714384e740ecf72541f1b8de764" },
                { "gn", "96862fedb0a550617f7ab013e7198bd847322ace6af831ff74800b4bae94b5941a3dfa75a4cf9e7f99b4dc83ef3d14760afa3eb5637b827d8e7e3c3dff8d6951" },
                { "gu-IN", "123fc6e88be2dd4ddc049c3049b40645fb49615c010c1b3fefd6408cd7acb9fb00126f3ce35631d59452bdd6453cdd78d31c934dfffef80f10d5a566072f1089" },
                { "he", "8c2df34ba63e526cd191c0ecd37abc69294068c0f222375d182a9afdb74e4d7bf0b741c1300cf5b8f95879c9cc23ac2638353ac0e7e8739000fe4634c5d1898b" },
                { "hi-IN", "bee6ddca9df82ecacd2703bdabb721ce7d7b731bd076717ee0740b80a3e3b3f123bf3b953a64707bdda6e56f013e4a99d4f9a2eef7076708ad8c63cae0eea2fd" },
                { "hr", "754abc48e9abf36ce944bb6d0ffc977ebb73b500438bcbef8ae0997fb68cd4d46e6996a468105c626b770412065ae1db2154b0a20fc54664bc0220982edda8c1" },
                { "hsb", "62ed05c232719ce54cd413613cc2130d1107454b1bf5b7d2e2e5c73028b73d102c1bacf97fac3aad9c0d2a9952354dce7d09cee1bb87f699d6e718cc7b0898b7" },
                { "hu", "d21be0df01ab1661ffa96c1bf6c7fbcca0c52fd973967a397b3d3e92131ab2d5fbd1cadc107a8511e7e7f3dbcb5c118125e685d738070b2d58c6581babb46e39" },
                { "hy-AM", "c2d975e49150e3a440972c949a36d33e093acb717d8948cddd7fb97d6935a0f7d444b1daf929ddd9a085897b59eb45e3350ff5ba71721c5afedb099775a4ab1e" },
                { "ia", "24ccc662ed48db1c6e5ddf649585911e50332bc200092547c7b7d354d2c622e6fb814dd8ad24a2a916df18422808653dc11e192c827825dae716e4d2eb0db4ce" },
                { "id", "70aff2bd75cc82159d145c68d85a01e6062de87099166da2c033b1ed3391c7c22252e24a3b383f2cfa04e58ab745f4839c622ca9788701a48c68cf30a158ca58" },
                { "is", "e5db45a7230eb32beaefdba70e514872547b4cabc99fdbf0ad867347ec871de43c31d23e5f7f01dd062da15dba2fc376b8d70d2b32e8c1cee6ed3167f546d57e" },
                { "it", "bf7dfa8a143dc28ef5d66412fafcb4d6dc92a68a15e84c544830d3267e58a2a2dc1ea6cf461e4ccde9ea90b700c85a9f2cbbd43267058f2332baaf86054e49b0" },
                { "ja", "e38f5c9e40d0008dae782122d313dded5aa3cd99122502ffab1dd714e02c531d00e3c10b2a7d92c4ac222ee427c06d1b0c835da783b16064c9c71ea4bce0fa3f" },
                { "ka", "dfae3091d7f71bb92eb4de857a29e8a5105f0422f577fce2ebc7840a1d73a3e8dd40ba27b465c76d36216374b4f0e29565bc148764618e9c67cd7c7251117152" },
                { "kab", "018f5741c2d241dc15b78f402636aa856a6f9e7cc141d8df1ca09fb844de35a62b519e9e685eec95794353008a937e7ad253ee67278b47947a4b1c73fc3478d8" },
                { "kk", "d07b4a843dfd54d3779688a63380785994512ab88d144dcb90c307c03e723203792ca80b8abe4416bddb66bdfc16c555328ab19ae804b2ba1b08a0f95f4f25f5" },
                { "km", "84103145c8f597a15ca8ab395bff9436dba7fd5c69ee7c9cd3e9b349ada769833e0ad623d9c66b16132080a97da380c78be592fc5f91e0066e625900f16e2736" },
                { "kn", "05bbd614bf017cdd6dba1e983914c8a1ee39717bbef9cae5e5138cebda090b310069a1b20b673b9cde23f05963be4ad372b28bfdff4504317502b7ab04defe8c" },
                { "ko", "333f0c8eabc3a3cf1c3f1ddf2708aaf41344ba23e14e655a86288debe1c1e1253f4fc37541a69cc2edffb971ce123bf344dec1c72f794f860005a0638f5deb04" },
                { "lij", "0d888cbe67f01dd57f1fdeeb2c55a9598ef12f4d1e4e20b50b0f7b5f922c956f09574e058930a43514b9c2583e39defedcc81636e1a853b25f583f1a2d72f49f" },
                { "lt", "b3fb6f5b9250d775f069ccb94c0fbe2844b4434c3544b6d6e30342c3bf7152f100704a3fc2ca19123e11d27b411ea4bb1bd398851e77d1f89001277cbbe3e47b" },
                { "lv", "8c2feab111031a6db55939747fdc420b4fec08540b5f9b9654af5e2faf11c8529a9840cb61d86c36dfb51c03d73ed216cbabdd58f5b67c58328d941e20a841c9" },
                { "mk", "e487827baf58d673640169c53dbd1aeada4ccfa5192b839a2a9b82a15f8e5ae2adf72fe62defaad125dad1876ecf518e90453d06e771aade6f3fb5248569cb41" },
                { "mr", "a3277fcbf7c9cb91ff37f932f9c4fb07a4fce2eda29449072388075dcb1cf91592dd5753244d0ab5cb3ac99d372eded24cf891996c5308c650fe3c09b5df091b" },
                { "ms", "70d5003aa09aacb132488d92e497966749002a680f7473d376aa71e77ef3cb003f353ee00693d650e4fe86967b7a3822f353eaf0a491d99db94a8728e2ae8a6b" },
                { "my", "4ec4abd848e5cc029dcdeaa167d14d9819734fe3752554f26f3bc4c7e5d0a3b82c3070d6952b4eaba46205573a410f6063300d8bbdcec9842a05b0e4f97cb0b0" },
                { "nb-NO", "246befe07bccf59df2862ed3baf31f370e06b8d1fc95f67f327ceb7aa4d182fd482b003a77e5f326f9713ca6c6c816765c12af13f3fc27e4b17646141f92c457" },
                { "ne-NP", "5281aded35eb225df4b033c12eff921e258582186ad661405dead47a7e45af8b2a62e72c84f95de1a7a7950c76a396fe6f5477e3fc70f70a783cc696a5112afe" },
                { "nl", "9866d342437ebe844ce452a1f2a9d2f03bd5680050a56d021d2ad191049ddb0fb01d354392ba403e682f1877f859432feaca2440671a7c15f8b75c7bf16c40bc" },
                { "nn-NO", "88911a48336d19523285872262ecd37d98d1bd069135e05b2370aa9713629de5e8c8594124f1186205ac8fc4143f5570a56828e484dd11e8538a6083083d4cd6" },
                { "oc", "7f8f0c8d659b9ec99deacdb15dee770aae51c0589d9e5174df0dd1b9bff325746ac79a6dd33ea2a00eb1964a14428376bbd37ca90222750570d2aa6022f5fd97" },
                { "pa-IN", "fa1501f6badbcf6aec74975ba8ecdef8d17f63077bcf2e134f59b419bd75fbad7182ea9c5e09d2cde70e987ee0cb1b758619d9ab0e4ecb6e04b63a1f7f4152b6" },
                { "pl", "7007fbd998986881bb1e3193d5956bead25eb403d6527b692978953eeff9074f303872a788b4b4f551474d937dfa9d200b73adcdda81703d78aadb0db2e0c54a" },
                { "pt-BR", "bc40331bab3b602e89320dec2781fd37df8ccd906bba26d3f9548713fd0ed80df86b0c0256fa3d0bced19d45625b8026db39ccddc6a29abc2b4a4c387b3d81ca" },
                { "pt-PT", "4cb7191bce0e331cfa7aa07c67b889b39669384cb114ecd65ed63bef6ff77bdf1bcc503cedb299360b6cdbb1ad1e798e9b5a1dddf0d6319a2dcf00d9f6d1cf42" },
                { "rm", "57d2f2b450549a2fa134b031520bda8eb90e7ab53ca0c3151fdc310fee5dfca423e5dd43b5a61f1946cbe5fd128aa587e78ee0b7e667f288226acad455a9db33" },
                { "ro", "26a869a5e8a4fbba34ed5414e8c0ec054eb3cb62b928d1ce0e88053f9d059548925375e3d86666d7701ed923a0ebe262bf3fcabe4fbb04402d742a68bdc82b8a" },
                { "ru", "db7ceebd990a5ece3abe8abb4c99f05a31e2a15a890672051d62bcbee2f9f3a4f3ca39007eff0c1fef09e267760fcb5ff2a0f02d320a7c5f190155d47b6d7e53" },
                { "sat", "6ea7a2b9fc0ec5f9969c1445561954d2521df14997bc80c987dfc9bb17286ffe6bd2dc4145fa1e1db5c1b6214f45c36b3875f7ffa689643ff35bcdddfbeb8a7e" },
                { "sc", "e5ac787beae9da2e3f568385738bc37cd1df8a8a4976753ed6614b9b4df95291a6dfe1b83fff79b273cfad91203b4f1de287dedcdcaefa6cd8c24b2b79aec1a3" },
                { "sco", "f8cb51a058d43cd7da2566eaa546da8ca7ff8c997cd80d193b258d67a1028feb62ea3ce57be7b214bf8c2d67df635f9e7e5fc1091132259e837593dbf4e2c1cd" },
                { "si", "28dcf108f4fb99feee2b3ecb652bfd413fc36a9fc11fd7cc2acc1e3b43f36186ab666608835044f0f42bdba6f888d3a78fc00462212a7aa42442c2d06da9ee90" },
                { "sk", "af139f181c230e7623da7798ed3babb4bd0fe1c4574e755d9a794f96309a0d386afbd8ca5d25a7e365701e14dceafc3339f73c4d2588da78cec85a2b409c25cd" },
                { "skr", "72d221c43a7fc8b7582f0d3d0c2b0cf59bbd33a7167eeed899946bb030c4a0cb8df048ead80f96881072da3ab0c3a6e1b78b7f4fce21a48fbdb5270966f6c649" },
                { "sl", "a4f1572e469a2a2e4d3e0bcf62671e76ed7e099693c218df26619f3a5fde65b9d68f04b2d7d01a65e56bcf05910b5726a7d3386e8a190f0f1a303e2f3592245e" },
                { "son", "37b45d4ea2175199f441f4b46377cd2d636515830532c119dfbc02fb4fe807a007784b1f0542cc706884fd77890488a700402eeb806f964ab53dd84e508b5f4d" },
                { "sq", "9530903f27d659ba7cfffe21c143cd7f1c531bc5ead3a9c35fd31a9abad339671d024008d7b6a99b571a20467286cddafc17af26c5e735aeee60c8983d804e98" },
                { "sr", "3b4208c01a466181374cf69d1d9bb996aadc6cf32b8efb0aada644a338716fc8e390dbd5e6d94845a16100aa8be6376df78a415063df0cfa7247acaee690fb0c" },
                { "sv-SE", "085f69483d7674f1f53deba2f0fe3083ca7e018f4f9a3a35fbded7f3691a7e1c1f8c31ed9d41cad7dbb2b54fdbb71f0a706c22c5ae8f5843effc2d9f51102fdf" },
                { "szl", "691f232b6498d0779746467c75b9d77910f11c73c75941329714a99af9569d8b9d954980e68a147cb2e2b7d1b5e5a34679fba90af1e5579b02fdbd514a5b584b" },
                { "ta", "53a578a281b00a14bc88c81c44a007a564a9f847b23e49157921b551ac125002e99b048c95656499370047523495c53c4b018b5ab0fd0d1ff675b182369fb6f9" },
                { "te", "1c99c10aef722dd939ac2fa482cce8def9f2963505d0629b4b98eaa26184141c44dd2e941c4d4a105ad5a23619c316a9920fdcc417c8df45db6e315c832597ca" },
                { "tg", "fca91f2623d4afae9c8d1403564ba3411b8068732ac83a11824c8268da2ae2372ef4574c6f3726a63eae5511ea36c3ac99e3020e90535f32bffee508da01dc0a" },
                { "th", "d2573e92125262a310b5ba9ebc7e8f9fad6fbb3728bc048929d9df1fa9ad26415b6949f507f0beafc956364b375a9e929ad9b1b9a6a73adeb370b0ba8ce2a1c8" },
                { "tl", "13bf799fa5874fe2f108085f31ac7377f78139e7c35527de2555caf8fb89b1adad8d5a3be65f098b6b66ab4247716e9a27b5d423b81b7f5247d244a443a6c18f" },
                { "tr", "60e91c643464107ce650356d5aaccea5c4bdc5700827c917fa9fb3cd51e90d5e24b749e6c2553880869114c557c8f9ce1112806ef5f11bad97e9ba54dde56041" },
                { "trs", "4fa410e9174cdf4ae7ee688a8929f24f542b379270ab9bc2632d184ae2638973ecddd3f38699fa9334ebf3be3f7d714505af90a46fa44c7aad9e4d935a973bb7" },
                { "uk", "5de9ee7c62293d745021c76c70579b31c0f952ea4594b9f9c555a06e151601fb3c162eec483e69dbc72f87f5563696b833f7fde7d4cf823f40e2b4264ec24647" },
                { "ur", "645faf8b1dc3008e36ea01a78100bf383c7e367183638fef4fa3cd6d21f7dffb9eefa1c1c19b0d82429de2c2ee935d614b9dd6da94387d5cd985384d4536fc80" },
                { "uz", "4c44a84f644540b7315e329e8f9e67bb21c528396e0e8f4e5dbc88a956050c651be0a9104d95a7f063b1053a89ed642792bc78640696250acc753a974094fef5" },
                { "vi", "f295c957580c08445e01376aa5477ae7253dc9cbc3849c0403b68fc572d31d00f6cd8ad0e15a3884589a21a467c5a84eb100ffab72a894c9332fdc8d0874f3f2" },
                { "xh", "b9640ffd110cb8d92c9687bf098f1a3fee48a35a53d1a8c1a7844f64adbfab92a6e65c88c2ffa37cb79c107ab100166aaefd624575eb03fcd0dca942e92d86cf" },
                { "zh-CN", "65002852649644c1700648da5bc5bbaa750d7baf136b476dc7f7f8a3ea3bd867ba61c33afd78222aec2db0524255dd2d10b9037698cc97295622f2d2a2a016cf" },
                { "zh-TW", "457135d834a0b3c1a76d89c92d6d2cc919fafb5f3f31f9e461cd502fea73c2fdc90ab6f5d022a0ef61f1d324aa83a277f71ad0bb255e9f0a56f1900764c05f0e" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/128.0b3/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "a05315318b6fbc8fb94f7ff384299614608eb0da7127378319402c5062fd5db1c2ad00a1a83dff79c6a711f52c9871a2fab772a13104bf2f64e013a663e33453" },
                { "af", "e0e283dda41a5d871b5a349887011f46a32cfa64010516696e65db0a4f31739299f76cc195de3417e315f78322a8ac7683b6cac56641ca772dfdafffaa3fb5e6" },
                { "an", "448184b6556d42da28bc59f214896afc2638690f5ad09e4f7b743fa521c92cf572f8297812379ffde61694f75a5afadfe5341d74612888e2c0cc8d533849f220" },
                { "ar", "9c89ba3a21fcb8a407de97f2d6f20d21183b805aa8437d45be09d1cffbc91adcbe4264537be6b7f743a8233f27d85a5c7c8c901e6d4fee6cfc5c5b8ee00306b7" },
                { "ast", "8efc539c48c64040efb2cebe36513419857a55126ada0cdfadb8c5daa80f66ebfde22d2713fc8ade525d1e2a967c9bea93312265bd2d37b17c9f84e3aee63e17" },
                { "az", "688a348886ab7392b32577458c31ba54bef6f8844f73a3687f9e069164c1da90be303c25000ec2ef03d4682486c60d5b0c73547fbdeb2352f17aea7537468be6" },
                { "be", "b159af74c80b5018083ad3eff733da9cd6fee5c845cea026e0775085a40b13a99f9c6a25c81658adf7c6406b97b4ede4d87b12e2896489b0d7615de0eb9ba237" },
                { "bg", "be8a692825a7482a7b8ed7b06b7b44cfb837e8f4c39789ce696161cf17f13380b7d134dfb546d2c91201f236264a430c53a7ac22c937f6630d995a564903c879" },
                { "bn", "b02ebf017cee92551bd30e22eb506489c6587c22a53fe6ad630f52a1ef3caba1ffa5299bcdf95ea67fd442427834b1ce396288397e351c7984f795865e029c1b" },
                { "br", "346791e8face2d90ca31ad967eea8b65a60ea4b419003dfe226c8863b75edb9d4536c9f46d146163627a51d1839149f49435f9836f083481a3058ac244adcd26" },
                { "bs", "65e414800e1b6ccc42b1ca68066790beb1a8839f7fee7376602ecf39ce80056671607d3b6e73fa6d084a0f71f3bbf5b8c6c29303bce620689480eb52c1284621" },
                { "ca", "59eca6b8f8cde44d551f8cf79111f300984e5d84677e93cd1dd16b471b35d7a495b0a85e9bd297e67cf76980dd5bdb9d6971702da68bcb211c4058a26bc8ef23" },
                { "cak", "00145a70b26397db138b62b79a64214ecff0e0e81a21148b103852bdf45c49caea326f5ec9b458a8dc227ff418f78e6db6a0a15b45da08ae00b74744b5bba10e" },
                { "cs", "1800b74bab13ac303241bc3a990aaba9543badfa62f703464d8d230d1508594ccab08bd8b0d7e8e379434a106ccec6b3bf8ac18ffc0da87492826905f3ca62e6" },
                { "cy", "a6fdd1649d3e28895044f8d510ad84f8c2f9ec95f7317ab9e0d09f665cbe94f09b6fd0844e6782a2118e905f1320d438e18a4bfc568388675d5e4c4771b277c6" },
                { "da", "34069b08978b67a8cae37b9e5bf7fbb4f59979f7ad469277736d96b2c010c757f3054310b70c6ee097e69a76d4b1bbb6510405eeb0bcaf764caaf1116357a8cc" },
                { "de", "5ae583764872163fedee9a7695c67567c90f83ed5b7e4c89d55165de304eb522609a7757a691b6121024123252042eb1a3c5b26416ff1373b9d1460007abce49" },
                { "dsb", "b1556aa62e113d67af1dc6a2c6430c83e10241abaa731819e15c5694b4878e6102e01626c01dae5cdff4cde3c3af70461ab8e842f757b434ebd546c80c071f3c" },
                { "el", "df8f338b967169f7bcfd2a2b3140b8ef9d48e3372cf3abf3ea506d53f0ed1d7dc9b50adb2d69e243a7f7540b7d0f0fac0d2cbaadcb1fb3633338e425b4a0b3f4" },
                { "en-CA", "df19c4d442835503be06c280279337da62de53128042763deed31c6c205ee309376103884affb31600aa8923d2ed61c12bf8124ad7cb29fdb3b753715c8c9b38" },
                { "en-GB", "017c9cf190db0c20fe8088662baf628f1f7565ee09ae40eaeb921fdbf24bf87fc221b9157dc6a67bdf4178dd9c89de6ae4663753cb050d016f7d15e1a28c8158" },
                { "en-US", "4728a1c333573dfeab1f3b09468369ed0c8eddf2c0367169542665fc074fbff68e3dc730c817c81ad5da383924c06b5235be81c6323382f76d79a7354ecf6332" },
                { "eo", "514f3ae689304f20ee2278eee27fb192ce8ad9ac30579d18bd12e5a2b49da9638d66ebf28606473d036f78189e1c0255a5732aba637278e71f175a0c0b4408c8" },
                { "es-AR", "3b0f9bbaabf46751349a2195eba9a7b655e31d4f75bff1e26afe80614922dc0e93ce7a4a7c716a357df2274d9b8bc102205e87bcd854e2a07c42d30665194035" },
                { "es-CL", "9f90d60cefaf3ee3d29e0e147c0f52e78e951b0837b6708171a840766f5315d1700681ca13699606ec437a161d9b0dcad0773a55deba09ddee9a5467036d7814" },
                { "es-ES", "d5ca52955551bad2250523e7fb1c4a2f863be009669502fbb59f06d4f3366d4b72f999dd15993f42606094995caa2ba72f7614b099089f3ac15f8b7da88b51f5" },
                { "es-MX", "a855fd5dc67bb664ba6156d01959a7494f3326011f0abd7b4696883bb1fe6152cc93db63edee1bed5beffab54b9559d55fadeedf8ea1bd4fb6a243c3de90a1b2" },
                { "et", "da6916df1eef4fe4e6b145705649a85b9f87e04aa6faf6512aa20b9bd23d9d9396023d8778fb4ddd86c2a500e97533fdc46b013e24037ff609621c20658b1737" },
                { "eu", "971cb7fc7582903339734b8e463697c477826f0bec7480c444ff8f82216faf27bc5f526baf3701ba446c4f12c44acaa9f9d3cbe9b1851a0761718a6588fba6b0" },
                { "fa", "00c42d4ca6286bdbe8f63a78ccfae3850b5d9caa94d6d8c48e0aabed99eb1866ebb374e8093d711c1e3c5ce2b9a375a5ae3822a76c2f72c3c93bea027f10060c" },
                { "ff", "38d10f1c895f83e4a4f3fbbe2c84efc1130d9d9cfd1c69da9d7caaad5cac023d50afa7eb74c7c8acdcb5dc1c44ae9fcc44d7757dd4f86318e4938be22d5e0a59" },
                { "fi", "59aff7e3d53a16f9bfb1442d041ff0662fecc760feebcb6605ac90fe827c9c857383380a3d07729c0ed9ee8b138e36ceb32d87c82aae91813b3eea5a8680b9c4" },
                { "fr", "b63d92594da51d912b9a73020978c3f6cbe0097c968c42ff0d9bc18fb3f4bce71d7b9646a7a1f3e7abc87a0b68d8a80ffc7c6af8d1a62ae5e87603edd7b6db2a" },
                { "fur", "05cca48ec14bcff8927556c32ba185d276f21b01b61dcada59351963a564cc13666bc06fe04e56cbb35cf6e1e5e57c5b09b27fd5cc99ab7069510310ada04d27" },
                { "fy-NL", "d81cec9596a6973c245a0b908db6966142ab334b5b4bed7664eb65af14bfdb746dfd3fb8d364fe20da8db1bdfdfceb1ac79b35b996fdcb4118ab94b2cb64c799" },
                { "ga-IE", "fc1f12bdaf9b45dbdde92fc4bd9e20b24672b2b9891d67f1e8117d6110908824f38295afc66731a980de1d2d1c445fe1ea89013569bf7d1e5519e5557e006795" },
                { "gd", "bef0e831438ce8d32f69e9783e91ac9542eb50057fa734197c4ef709c2363caca20e153eb174f308c01e7a69f12ad27dc23b24aea2b36bd1b1e9f6158f35da21" },
                { "gl", "6d2d6853605141f7732f03e6bf57fa8b29d19e89496e7340274bf0401276be630b6b7e3bf2e82dced8313b2657a904345427c3d7903e15022f659542eef006d1" },
                { "gn", "c3755a4a2d70bed7962f9d2e581e7e5e5109532b000245d46bd9121030df4cfa57ebb848706a069ed3fb80451fd56e84e49bc92c9c82ad00c6fd0d38b90ac03d" },
                { "gu-IN", "b3b61d4f67cc9aa44822aa584465103242cd7f5577f6750caa26d22a5d3ef7ba205f5185c26d8ab94fa688ce3660b6ec932a17f14dba2cb3c5d144f90438b441" },
                { "he", "96f5f9c743263606082075022e75941850add2915e9b5b597a96ef4dac78139a2b1a7a4db0bca59acb216739b17f91944a73a6a2b2d7bebed66a59f0e6c70c42" },
                { "hi-IN", "ef00730388e933f80d7f6d99289f99b044568315297259c666906bc6c3d5bdf58769aa417e72134b58d9bb59084bad8cc93abd5daa939f8c09debef18ebf6e3c" },
                { "hr", "5d1c8246a410bd28cc298152d4f0f8d868392ba7959fbeecd8515b20b1fc005052074e6f9c075477278658192a138191feed56e6a48ceb0e2ea09bf7075adc61" },
                { "hsb", "f5bec2f35e363e61a3a0b061ed2059db3dfc91008551ef9862d5d351c1cac2cb7d6a6f2d9a0d72f5d10e1de2710662606bcedce4a294df492173d92de4aa8cb5" },
                { "hu", "9de5541c0d351d2bea9d3030a9a9c160ce317f282362fd403841522f667271c2c21cd7cbaca9c7077bd83cfa49b0add39333609a33dd94a8051a4d12542150d7" },
                { "hy-AM", "fabac4f531a1759e585170b368dc348e0db01b7de08a32d127b0f01af8415f2cfc7047ca3bd6b4748d1bf495c4348292e6aff200486e3edda05147411f3af654" },
                { "ia", "0a2b9ffb9d99cb0736384c8154c7ddaab13eda54a9767287ca4685279eb6981dfe3f6a38c5d6c0aecac77ed32855b871eca218a01ee4e6fac56085108943afc9" },
                { "id", "3867c6e1b29b66c153101e11701516f7572df5b474390ef629c7d86956fc0989f3ed460c0b46ed21780343f48226d1939123c0ff16d05e2be1f9bae744e7cb8b" },
                { "is", "412d2b9a474658d22c63a8782d7f6e87239afb45b199ea2b47c8291add76e70ca46dac68130befed5b17927a38b77ad7bd169f597460a831dccd49a036464ae6" },
                { "it", "f2261b6234a1de1dea474a47e29698487ce46bec1442985f1f54f358383073865ddf962cfb20330eede37f38a557d70d6bafd829deaa7d75e766cf0a9725e19d" },
                { "ja", "ef6e06a64c8780b24e800e31f82b07c2db144e31806cf67432f60cf5d445dbdfaaf42b25e1cf6f9faed1afbf3df97150f1cf90aa244c49af5117e4f9dc21af3d" },
                { "ka", "0ea7fe493f69b08da3687faf82b65c9596b8e04450d344f4762b10ed90d13f5ccbb792d4fd6540e7cd3295d7f509bd9df3eb7ef2d9a44c6b9c6f11cbe2d34fb3" },
                { "kab", "246dc4557f3a9df5b7a8e4c0fa91f83a92e8b948ec2fd5cd6dc9baca7393cc1a3572d081d67a126204b2ca6a486f2d599b645dd9d8bdc50049240021b8941283" },
                { "kk", "f32472df24301abc7feb851cd627e881403f528c2912a0be0df0ac348ea4abc8122d7977f26fb306b0df8b948c5c0afc0646142f193cd3aca7747e3481663c11" },
                { "km", "21d4c3910b505e1ccfbb54577f2e204f07521a763acf5a666dcee57d5fffd78121eb16cdbbc68739dcef62913c4d338735a12a0e56e96e044aa6b98c5d6ccd9c" },
                { "kn", "256733c718cca44a4382dda9ffe74df8934c106f9cc67249c87d3ad7967871e7fd3d11510fbf927d269fad7fb064b6d2b7955864e41a82116c32e7431c465a17" },
                { "ko", "240a08ebac320937d730ea247b92c97fc5a0e29a56401f24c90429453f74bfa19c742c480cdb07a657dd450d4a2e0188f8cfffd7e0a3e40db8bfe759bac2e51a" },
                { "lij", "aa3f3e7e1acba4d0047186b6d3c20649c050273248e6f658f194209984181976a0453125f84971bf3af38007e2e252bdbb773fd1ac42daec3855eac8410a7780" },
                { "lt", "bd04c0fcf3db51017254dbee218bea81d27587088b2e8b5c5c2700c5328f5bde8e426844c820fca9c2af9ba724ebb37bbbf94dfde62077098b90dc41a498769f" },
                { "lv", "3bc4d9049fa50f6e68af5e2555fb7e9f7bbec061cfb11f5cfd50af888b824d0fd42dfd8eca03255825734a0aae2a9887020734008f32d856683ee52ddbd41134" },
                { "mk", "b880a30405384308050f1781181eca4b80341ad3f1714be45410fb443a0856f42fcfbcbd5fdfbc02175691dac72f2af441a8dc7da707cc3f7fc23680bc5eb7ff" },
                { "mr", "7920290c5a71f27ee613cbf3a29a75d7ad55c7dc34ec504c6caf2c5d41ca07fa67322c1b4bb672784583d354380939129fd48988d4ac7dfbf57d56edf6e0e282" },
                { "ms", "0d7873f352b7676544261fbc157b861ce0cf96c3aa18c0c0e833495a56d55808352bc4bcd26ceb6ba8cff6d8d9fb3c5602129983bead099dabccbd04fea9368b" },
                { "my", "20fdd6c3d7e2c37ea798028488043b9f87563d6664e973d06e66a910bf6469f04ff501b966815a9358b0b57db96c5c896d6b2352c13aa27cbb01868ba76c6e7f" },
                { "nb-NO", "ba590722a2b1f8981508368134465097ec166753a024cf8224b31247f2d11791637aed80e7f455445a7e831a9ee0f2d09b66f902f8d68b103a2fe93e8d7a2655" },
                { "ne-NP", "1c3b12d7735e74d1ad0618eaa996130421be877ad198588d368af6fe0a9c5a9f5d490a04b39c9ccdefdfb09525b2190b874d1c898f58aa37edf86c9bac7a69d2" },
                { "nl", "3f9df95a29f7cb9c4a01685ebef7c047d421906b014e4388497f5abdff78841ea637c60d04b21c5240a457aa943979331adafb35f86e97a371df55cc441f9dc8" },
                { "nn-NO", "84f9afd5570dca9d2808eba53da80755ac627a51ad386d5ebd3f1cb7703a6bf09c6ef96474d32950d89d11633fc0bc2a387aaf6b5b87be8f40ab037a7a46756e" },
                { "oc", "d19950512b5564ff5174664625116ebd1c91eb39256e17322f4d55f03b6f4e9af4dd01ba0b9b0d2e8732941df0b12d10c133997769d9e127a070051dd0c63301" },
                { "pa-IN", "dd84f837522f832cfdc695a74828a5d449268dec9dad77a81360cee2008bf3c5fd10df72f6d1b91cbfd14c033ae7e87f2ed9085e9357b3fed5581495c59e2716" },
                { "pl", "9f9ce463d1349eeb7dde576c98e3e941592bca103efdd787cdd11ac93853e8a8064e7178665b6236aec026ba5e61a00a3f78ca3b2c4658209cd84ea327ba3fd0" },
                { "pt-BR", "8f75d472a3e79d1f63a123f85b5d950b5f1ef9c3592166b481350ea5da3d0f5178c67ddb361c1f02d6ea837525e70873e1930a8ce36b743e59829cbbae739ba0" },
                { "pt-PT", "6ed0f7a804fd191186ae0b00a5a3dbaf96b5cc3dea36a9e4e5a434def5cffcd63a55b24e7cc61a47a48a99eac9347ab52f47982fee0070dac0613848c44e8267" },
                { "rm", "187055513e1ab1c3864186ef3dc9d10adc4cdcc0fe17dc170f7b2334b47b39456a263507abb4de1e917528e25dafc08e3c6c959052b79744cec7ca17cc0b5fc0" },
                { "ro", "698a2a311d82e44a3a8113505e15a6b4193c6404a6dd2b5dc81174d1d93246f17e9d78df66f48bfaf48989bc02f9bcc35baefc8d0326cb269dd4075ebab05b98" },
                { "ru", "f2ae458709b162eec2d4ed1f4bb9f5cab9263deb4c6e3b8fa14a12d57ff8b33e4a4dd6f9250162946bcf93c31e0a61a2a9240c1e29cdf7193146cc23c6aee61d" },
                { "sat", "c7b43642c629142a81404f23ae780df65860a188ae131578e4fce1943be4e92b334444441a798eededc12e249405b050a30e31b8a3830e8f916d3e48c9546e56" },
                { "sc", "33e7c3869f4b7a6bb2290ceda0384352e84d3b0d0cdc785a6d4404e585325a005ccd5f1136d21e5679aebde0b13255bf5609a90855a1c27c85875b22601e0626" },
                { "sco", "5cd898c5c6f2c1e4219d0579b3aabb736a037943725b4b4295cd87ec68eeb1af1ef09f1ca8a5c4602da895c6deba9e6a1bd4bf960da5c59cb4886291631be83e" },
                { "si", "c9e13bb4212247c906b700ad5320f358222547bb9200196f2e307bc44254e26cfbc88821c7a701c7f6df0caf2b1989ec332919c9efefa772f95ad4b1c0fe5b60" },
                { "sk", "1d92cbdf4f3a07cc170e82f8b6b414770d72730745b9309880b0db84b175c36b158bde103290dc402f974e454058687b270c245e1d346104b31cc5cfd73f47f0" },
                { "skr", "ee00c92bd1de6c99f49401e80541f54587dc47d0e989a5db029cb1a8a34936b8b354cc710361fc1a99092401e16742edf02e45940c14e457f78789894db5e30a" },
                { "sl", "02d0ffb07a39f2258e1aebf0a6144508349deaa1fd5ebb963797e894090b30ce91d026333a6ef0b6e2473103de29acd652b7b205f1d4d7bc60af22951abc2f41" },
                { "son", "7edd7976f9366e29da27321700eead0f5ba9ef0b15f55fbca81fe6dbea6c9825c6e41cd9e42df8d2fa86c3d0f721b6e77fe9ce5abcdd9122c240c47da0fc497d" },
                { "sq", "156b05ab2bf7f9ad2c0469f518aca7f55c397eb1a21a3bb1d5c289a4670a62e14647c0bbe0da3ce93651194ba7b8a0211387d49ef4ec3bdfa141871f27593df6" },
                { "sr", "1d315020f7a3a0797803ff6c993ea5a06ed04c650be0cd86591434fb171ef5e61e0ea4d7aa7fe3b7e43ff0f7f931139c34c7e9c24fa4eaf8d746dba64273ab6e" },
                { "sv-SE", "ee8fb2fed7cc61a6666c4c2383d1a8bb3cbf9d2a43a20db0c62ae9ea451b7ebd371ce41b98189957170e078f1aa8ea72b1d920a314f7426d80fc5806526a677b" },
                { "szl", "164dd792710df5ac6efcf3f26649d536e44691ada71b65b18810670bc537debe5ea17b14d814c0553361ae7700d338ed55b49c734ab043f7ebd8f2997fde7b8d" },
                { "ta", "5e38b157c864ff992d47644dd7f55254ec2655ba2df35a9af01e9953da6ae6a214f5f4fe6c32cb284a595792c2a093012a1c3f50f35b3065226bfc4243565e5c" },
                { "te", "9090911f7c51b8496a3e51109842493b70f1f559e924307ef55b51a09315770bb592309740f1959f627ad58f3b3deea653bac8a37e2f7bd2cf208104c5866eff" },
                { "tg", "271dee3cca5743bbccb52f7565ba4c51a44071621990638e72d64fb2788a152bf2dbcd76813055bc877baa7408f15d3d6b79384e3f3dba64ef8d972c76cc9307" },
                { "th", "5ae9e384c4da03bc4bc026ecacb2c02694cd885b75490660adf27bfadfc0db4f77b69d874271c97264367174174d21b0a2749b9442dd88bb05eaac734dda13af" },
                { "tl", "eb9f31e1619612b73a6edc4754e51acf9da7879501e82a8564b1ba4d57b8491c033726a902b1157859bd197cae01699175b700f1f92222c936142453cceb108e" },
                { "tr", "b2f30d7443abbea6577dbede048628bc22c4944fa45816c77833b06e7d7ea083831dd17ed34bb0a6c8c1b51a799fc7c8c3beb106c380c120bb89153061c72224" },
                { "trs", "ca7ca5c481957069b05f113097f23e1d8f452a376ea68215aef091815919cfbabcf37fdd6e92492a662dcbbffbcef43eb1508280ff0242ec44d1c5d3a03e4a01" },
                { "uk", "a210dea58242176e9c35da36e5503e3dc027aa9d1b21ad52edfdef54b7b37cd6e157cf12b3072b2c154a00ee7e4f2abf5c3841179f232768bf86c6bc10067ebf" },
                { "ur", "910d1d3a91e61a5e327e3420081fc8f36051b38d5f7c0e0a51cb8755a73d5cebbffb048407a87782ec12f241916d9460ac2f94ef0a51eb7fd57f312792cbb531" },
                { "uz", "4ec66554c832030ec3ce6fe70f7d14209b2c7027ed5aad11042d6fa0f2f4b2d1d2c43c41f2677b376dd62f89549d4be7ae3a19aa72640f40f3c4e620685fd689" },
                { "vi", "7c7cb2de60f41c129a77961a61bc665e2d72834eaf8d80df004457ffed26bec8543ebce9d57452501bd42484f0b380f85914c4a2feddd2d8c21fa8ec2d6ffa58" },
                { "xh", "7c06152cb7279541cfa12121e6926723cc3c6a11c582ab0f457872c40ec7080edee48d0f6f84de683025537a932b88e4f447ae599c9b5c5330bee63bf920a212" },
                { "zh-CN", "632f5a20b7bda76c83decf8d7c2bf594cdf46e6766ea91807df3e93544cd837a78a3cae2049004fa6b4f12ff434a0ca60b131584365c95890fca8232335b35c0" },
                { "zh-TW", "fdd3fd92a8b4122f865de520ad5aef7fe00a143428904db8c5da7a2ce63dd2bda5bbb843f0fc4b3501b7b533de141d0aa48ce430ed6e610ed642df8f217eac68" }
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
