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
        private const string currentVersion = "124.0b1";

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
            // https://ftp.mozilla.org/pub/devedition/releases/124.0b1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "2e1099c5c2732a6ef9fe66397581f217c3cb05e1fb174b08bf22864e63754047e7118109f80f852bd82898635162bd2bf483def7a56701bb492abb6525c87cb6" },
                { "af", "4e9f20187f9f0dcd400f79df8ca0fa44c6d3b81029e43e8eae88e189b09b9e9003e6d160fe30236598ebabe62371608bce4ac4db2c2b76ba3282f4da217764db" },
                { "an", "f48c6d592380b54d6858e00d0f65b74f500150f3ac241f3f329375039741c758994c7b431f7c20d2ea9cbe1baf5717a0a38e876f24c43a0dac186fc0c8fb1ca2" },
                { "ar", "f61e71916ef8fc62c9eae420f6acad542e1765a78fc1ced7bb3fd5a262e784ef5bcf8e7f2e18ee87fd6a3b5e61b8becb11832fbd04e9aff8018603c3106f21bb" },
                { "ast", "b9d5a0eeaa819a384da9b26895a6369c68850d7e511d301007c3b5712cab9770e98b70bbb5f5e5a30d35e8b3eb2de9cb957aa2a7c4d34dbc57e63a1e60020edb" },
                { "az", "c4da29c63a4fe3c0d2d70077788c11d8767cd85ca6eb3452320829cd2bf83db1b43453ba5fa15a5e8198832cc25bd072b4062fa08fdfd375682849b09324c2a6" },
                { "be", "d113347e57c3dcc84f21fbd86dc642581bd4c2f5f500ee32d97a66009132e3af009453c7722f7d58b7da9d5e3d2b7f13aef9df07150cb88de1d4ede41cc86fed" },
                { "bg", "b87960b3b778abd82a14b4b37d8dd4b9825ec2adb2562933c523570db60d7b263855a01275036f8b16bf2711ddf7b0c7cebf3f1e674d5a6f247258caba89ba83" },
                { "bn", "d8748c80b42ead9a0a38e17c894ffdc1a8926dff822f4305e6bb67a153593fea50d2eb5341cea0ded659d15d4c7eb90a58c1d67b0ad2ace69aa4555c866e01b7" },
                { "br", "9b7248f6e84d97d4255fe6f76d86c7a22019591710f99267aec831e5f4d8045a334a624977a5c98fd79403548ae69fe86d2f1fe316562b3d2ec3ab3945b9ecb4" },
                { "bs", "344c12dd2c9c200cb8329d3b0dafe54e42064b123e3b9173c9da2cdae7b7f980e36b60219d2061cf2d6239b49325f1e2de7deb993b6077ef0aa8f18c0d702939" },
                { "ca", "fa01384a1cc485a99c6a3e680cb06d531ce45c08c6775a47d37b59ae2827e2c9c59b248d70b33ec988dd375cb66b4f496a5a0c7f5712e6e78ed918287895a2eb" },
                { "cak", "35adaa1c62ef558629d218f0a6a1e138ab4ac514821328063c4c4a190275ab69401804d6e3ed33dfc47b686d954566f19240ff1752e04f045825138358fbb213" },
                { "cs", "547796d213212b347552eca356825f7255fe36dec1970c5fb12b4b51e7e96cafae958cd0effdb3a72cbbce52ce79d6eec39d604da47472e64d19c2d0282577db" },
                { "cy", "5c42573ab4b388c889195ab9866ac5a75a10a14c781f68b2bfd20b5325a5acc3e2decc2e22bcb26615dda4ebf2a0700f0a41ebe4c900350b2aea35b4a81c842a" },
                { "da", "5d16a03d2cc6ead3ef011a7547d3fa37b3980d412eaae3e0b84042942506020319498518fb06d3c20a3f39c2d691bf1e30beeb1ac80561921d52492dd3c1044c" },
                { "de", "f06475a787589a3a3a9955db25a10df3701efd78cf15893674fbbaf48d407da75e5046b56748bdec9e49aa59214336e5c3ccb7ce48dfec50bed9236a343b9469" },
                { "dsb", "02ef2b9ae995e18fc4bffd0b0ebfa404e143b386d68865cbae08cf5e7deb0e5f80409e9fab155dba92ae9bd2489107ad517bfa4c6da97f54c373643ccbeaf6e9" },
                { "el", "367cb27482ac5f785d7cb871e74dc83ce1eff8cd98c7c9a11527d7e17b4706a0f2d5b0c5a24c2a512c3b34085b5e4feef1250499d00e855e1034ed6e177a3c7a" },
                { "en-CA", "391ce12f59439aa649333858a72c500e069ec2fdfd172e8e685148ecb25c31d260fdc2a6c8632132b0f69715134954dd6854770e484b5fd840e791c58aeacb6f" },
                { "en-GB", "4fcc7b679430f10485aa1693339e0c10d64e6c8f2c89a3ef57318d94ec72ca29026f21ae7448a7689a5b6f2081c5f345bbdc42ed71264d81c3e3faa786609c0c" },
                { "en-US", "79ea93708feaa31af53e06a39884e70eee9f7d67c6e2d44ee1288a9ca6f83d474149e9a0f1588c8fb9e9c9fbd1ef1d512141c96d2e9d95203c38a048a250a430" },
                { "eo", "f05dc73503e8a386939b906c824399f78e4fb3503cfead71aab949add5bfdb45befd5f41995b42c216da9a6cc1ae340203634893c75141be5ef98e0742562fac" },
                { "es-AR", "bf99da5162b77d9583b3367ef2cd3e9056340d6be460c99d041688fa754374737759c6a92808e1e6c8e31a3603fd3fda903d2a14e9daa2ddefac7931eb4cfbcc" },
                { "es-CL", "fd279a28847a7f36306a26844682636b80f4d8bafcd5457fd50b06bf5e950136940f9520307f832b9e92b30f64e4d8cd136808df1ea6707293e3ec42bb28d133" },
                { "es-ES", "6ce03506720b3060f427ec20dccfae1d0754720880960ac69f42d2a32bfca2dcc23e5e2d6aaf59f39a447bd31a321a095780cf802a7979b4f899afdbb4e9335b" },
                { "es-MX", "7a3d270ffe76ccbf948b2edf005ee74c049bdc07cbd424d753ba9489d05b5b9b70dd45fe3ab07e0734a718af3cf740830676c5fb07da9217814cefd2f351805e" },
                { "et", "21fc6dd038d2c29ecfee83416899d1672433bc9fd2786daec0fd840f9e83c16d85eba2cdd638b4c9f5fdba66b3a0713b6dbd73a8b87b83cee3328e5d3f18f47a" },
                { "eu", "6e4892de7e1422492b15b631c6ae20326d2c0b8a7bd0482ff896f3de0da5a84b4cbfe22ebc33023940e87f5949b4ccaad5625a76b9de9f1427422b167b510911" },
                { "fa", "f046a67a9b11cbd4797c80ff76c19e6526f9943f7c04281752bf3462e5c06e6650c4785eb863c66a4d44dc4c12c5dd7ef356c037b4c20f69f28905884daffda8" },
                { "ff", "4c71f613d426bb9de378e2aaf8fc648da0a919f38ff9dd46fc844750474f486d5228bec941bb93740f60dd83c94ea1b0287879f622ad3b76e6f3882b33ab53fd" },
                { "fi", "da74be9f69aae6e3025b0cabb044cc891d7a8a6e6c83addc4b1039ca8ffc57ea01b4bde5f638a54e437d46ae03e0752c0f3618a9f41ca452310475747c987e5b" },
                { "fr", "214d59e364cd1c2f8d47ed0f6f4b45ef1b5e5b1ac34a9604120ad0e8ca9f321271bc04ffe87589d071720d571c2bd7efcb15d75939fb764cf3da67b84364d35a" },
                { "fur", "59f56b363ec84d24f6ac6efb033b0bec492d80403bb2a7dc2bfdb9e854a5d04d77337d6d9389d0a7cad43d25b5d1a0d8b03afadbe734d0ed4e95727076651249" },
                { "fy-NL", "025db890f0a9e0fa1739bd27b78eaffa703c0e265d0367033c6a7ad574dbb494202a4e6069e961d925c892c90e7b90e098270316b6e7503618a6dbd868fccaf4" },
                { "ga-IE", "cdf91da5ed30f3d18d0bd430b99e01fbbfdc935bd8d32a276ca5696c459c9ac97d579a2cae608bf6f29afd62bbf86e4ffb9a3f80ef9696c050561365c7958c80" },
                { "gd", "e663e68bbf8dcde856f02fc17ad81f00fa94d095a3a1e00b8ede00020c2308b98a896add434d0282b7c38356bffbe8dc0c7357332d378dd340355dcd23743483" },
                { "gl", "c1287ab3d0bca777813cd536fb3b5aec94d9f8b22adf99e1eb7699b7670783215e938aa2683c0412c7d3773da40a20438fe612d7905f8b73165e00228b437155" },
                { "gn", "a3d556e4fda0fc2f972ea7a1f4332ee5b7e88ab1fd459dbd9107a7426c89a471a7ef1b2e1e3a945f4d57172bd2c26c7205ef45a9642632a8ed25b435cc5d0fc1" },
                { "gu-IN", "ea94e21f3bca3764b9ad0d21c7c0ebda5aee5cdd4bddb40567aa89f8c0af917136ac329496793bb39f6809134a8a0f3585b707c1c53549f4142010c40a414de3" },
                { "he", "3e9c93d90b416e89b8f9b85116f238def7db0d0108199086d0611256d6cb4197832f5b017dd17c8cdad221ad6c06207763c6176d8edf7a8014d127491749ce39" },
                { "hi-IN", "3685fe275981a93a9de6286414c3be14e125acf3a8c301699379d5b2726a0f91632fb27c798a2eb51fa3394ec89ca15ed5fc64d368ac823f127e98e3db9f70cf" },
                { "hr", "a09b745e37086810c756020227930c0ae8c305cb417e260dd52f903d3af20ec77c5ff26bf24246587e6744feec21c4a0c81dca4303156935b4aabdcb85474042" },
                { "hsb", "40a62e480bf759f04c93df9b60ade9b41599c3e16f03003584c6b81f2ec841de2627db310e3dca325791175c9a0f6dabb7a1f556424621313418d44ec782ce8d" },
                { "hu", "f9b7b516b59778949f7b459fb52ff898d2d358f52aeda281dd241294c76abdd780c93e324f6a9f8d08507841eb299e3534b82e79c04c5c2ecb438594b7dd70a9" },
                { "hy-AM", "ea1eecc2b8535f4d4fef4cf0b328781d8307927b6af673d15080de2dddd6dacac89b64ba8f699d319c54de088df50e19f9bf72c5209d3c358620116a7fe8870d" },
                { "ia", "ef20558a9196da296342358ddcdb07e9343ce1aeb8459cbd23de0a36cce5fbb44bd8b19932aa6bb93d5582ea133dddced87c7e0d6a8f552974d1ba5c2cb45149" },
                { "id", "e2c823b165a687eb515ccc887f76baebe1992d104bbf462b25fa38c8a7678b86aa52072179eb6997b1b4e6bfba34fe64f3c3e87ac1ec4f860bad5a6b6e39e444" },
                { "is", "b64e29ee01cc1d683b668622021d8996381500590b4303f48ea0912fe853312267054c1b0a92d20a12f66799afa710ad2adfd7cd8d0c92c814d7b4ed6f8ed9b1" },
                { "it", "8f4cf03822fc6387eb5d4e0467adcdfbfac2b4658831bc504b9cf93de869e90d48e7686f99981c6a49a2426280667c02d12fe890a6ec2d5187329fbe86972c6e" },
                { "ja", "e5b7aacf19eab48e66b616407ae1c10763b185736dc1f40de9901490beb46c13dbe5061a468ef2e54bccb70732e685f644cf91d05f3fcaf130c8e5294244db70" },
                { "ka", "9741ab9661d9b6d369695c3124dac854ffb5f6e5264c61b223bdfaf90d8052e820150bf9df00d252c089685420ea2ff27e6ffa15df64156b65c5fa0647b22ed3" },
                { "kab", "5a90c3c51a584877354592d7dcb45900e6f50d8d0e6468630bcd9c3940cf4b648576eae2bff928ed2b446f5cc689dbc144a51ae1eabe0170996cbafe454b049f" },
                { "kk", "007da7c4a0ecd795734be07c5ec012eedd0087ea6337a3ec32afb97ad12395c74a32e0b7231ecb907b516eec4bf9b35de1b898dd7c18eb8e550056293c0f520b" },
                { "km", "59652263f985f2faab5b5564bcee4060fc95d4ef112b770483268689ffdada19aba1f11443319e81def76409c2c14377d4a2d0e4b809cab7430020e8b120aeb7" },
                { "kn", "d1eb9246a390cb2e930271044bffbb0c442bc466ce8084d50357f5317b489ff95d79c79fae421a85625452dab7cfa28ff4e47ef776ce968e48913460296181f6" },
                { "ko", "b6cfbc77a343f25367a9e3f3fd143e90857d3914a968e3524e0b8a30007931f9926a75f62952d36ec3631ae6f941ed1294ccb24b9d54a6fb205df88afb9ce51c" },
                { "lij", "30117863b4c013e753c7b3d3ef52ebeb796a597ab1472edf19014fb789dc5671f83dac2829957f108444f17d731175ee85856ee591c51385f420394c3d1f42b0" },
                { "lt", "6fb9175c874e4ec50134e8e684306964df722d151c7ef31de21d609895c94251aa82a8026d4661afac785e183ab23d6a3d73df55b84d7aafdfd997c33fb87db8" },
                { "lv", "0762c35f630b89e3ebc4994c17383684de1529d93cf4f3c41e9387bdc80854f3654d3a94815bac7e984616514e7188b2d98824bc448500c50d71743a950fc134" },
                { "mk", "a07483e5fc40cedeedb355ce860b371c8e8bd322edb19e40665d6b4bab2e589bfbb18301daafe6b4be944ecff0071bf54daa29a7775e9b5f70207829ae2d13e6" },
                { "mr", "25f226beda057f32011a7c662419afe8d3b477eed8fddc9510ba4e376e1839e35f7b2e56d97aafa07b2e753e532084b6f02799cfa25f3fb5764570dc16b46c6e" },
                { "ms", "05c3592db3fe1c3dcc195b57bb0be5c9e980f4a426581799e7a4ceb6cdbc7775357827be2b1e763d92c1ee4c5e0bef080c2257cd31cd8fa7bbb0e89880f09d8a" },
                { "my", "30c3d284b20d997c33aa79e72c497e02257a767d80c4440b03ebe335f19cf6cd334b404dccc1cca712be5a3d251e11e6d2bc17725c7e611bca95aea8d190f9a2" },
                { "nb-NO", "8090702ef9ecb8bee88eab8691af56f57bba78182a3e50e45a4e0a1e60b9d6f06f00a70cfb52e10d4daabeec9a228842e93879a31ac68f40599f64dcc576a4bc" },
                { "ne-NP", "3a64b89239aca9996e577e172616c1eca7d0caa4a8a466dc9bc4d2786d114c3f9a173ec84708f625eed7bf2fb09a00fdd6937c5bdb9e89b7ffb5f71d09760c28" },
                { "nl", "3b7c2029ce08f52528f873389773967ef6523af03a69d1858a180a650912d9a70b5b9d9d0fdccdbe5a643ed39ae6d5e235bf6c0b2c2f351390d0316e3f1c66e1" },
                { "nn-NO", "7fc7b7d7c3bd6da402cf5ca9287795a2dc1d3e89e8a742ef65f6530e3f67f9eb65067d568ef446a93744f1159d9b438510d16aa745ea7757e4dcb62d6a95031d" },
                { "oc", "ba6109993eba3e3448b784c8b127bb31e25885cf00583e3b23bfe03781a1c81e49c1bfbb92fb084097dd6e455fe13db99c633f784700860b3b1d751e442bc27f" },
                { "pa-IN", "35708e24a48628354c057bf3ab045c4dd43ca941004bd53c6484b00f30bf54dfb0e1d1b282c42fa41d086872caf7467b0f6c89e7dd1606a480b14649ccccb6ca" },
                { "pl", "c69f74cb8d26cbaf170241cd3e1050934fe4d206d0faa87df287fd53054c530f691c91455314458d28eb04fa3fc19f73db191af9df76631c7d50e888ae0b8817" },
                { "pt-BR", "d3a5c307e6c0fe765b727d0126d6175b230d4775d176b2bec134dbf5ddc5ce2c83ca35a01900120573189bfdd8cb2473ffe52f2df2a2e85661cdd3c5631088dd" },
                { "pt-PT", "2c13b39d77bb3e773250f4c327067433c23d1878286f7ccc43698b52083ff4dba25863112966fc7a1e0379121f8a9ec89fa136d6a35fc89a29b47a0a2ac01d96" },
                { "rm", "fd2422feb26e3d6d61c4bdcadd74a9921e8cd0ed50d5bf697ea590b73a6cf3766134eafbde03ece5c5d5d7601794769668d8cc1c0c0d9265bf7615fe7413293f" },
                { "ro", "0249f6f693082aace26571ca8191801b69bd5966de793025289c2d648e15f9969ef532f0b473716c5e97f09881bcdb3304cde4fc450f1f059fef2769008ca241" },
                { "ru", "82232b3121e2a22e6778952b98f7692e3bb32f5f459d516bdaf4841860f6be97a16f61f3cdd73b75630ad7e38931e848de86714ac99f8fd6b7b6b28818ef2d4d" },
                { "sat", "1bb40e0b6bb7970ccfb4f45d7bf8c101e5ff43220f7620f4600451edd5029eb7c85ed503d02d5dc668e4a0bc66b93dac806776eb32bbc627efc81d8f4f3ba96f" },
                { "sc", "1c715271eb459aac3ac80f4c62bf2f17e92b0465e49e55a04a9d624d4991c251505761749969b9bcdcc45e126f954cde8f95d8e23ba4bc30ad0b93f7408a4e25" },
                { "sco", "c89952a13b686a8a4272f30a8ff759747f7f8e83b9afbd9b2cd1136c26914580e84ff3c39f21f5a408bf6349b9e4d68a21dca2164041e78da115cc1cd8c002ee" },
                { "si", "72847497de35a40b56c20c9cfb868b7ae864cfc3cdf9fe4172c024f7b276a80db49b255a05ab84715ade49a5dfdeb08f8c689be02629f84bb91329ececc2171c" },
                { "sk", "0fc925889eeb0214ff42816ad933b89aa525e153367192ecdd535cf99e781689dc60b622355d04daa8bb1590c6b1a1fe2bfa9768cf5bf2a54294d3cda12e5479" },
                { "sl", "2785741b4d23ee6a51d382a598abfcc1df591eebd06f16f8f28e8f4f6a5783488a40970e75eca302235e54d9c9b67263db33b036c375b156f542c315f8daf6da" },
                { "son", "77a42f0dced33ab24c65ed49fd0f07af7bd2ce995ec8210da8eafbb8042546c54cd32301a5e2c1f82addb16e1ae37b65ed72a909367815a7c90dcbce324ec758" },
                { "sq", "10872f3f007662a52764866c01c4527f7e48a1e2cd11ceec7d2387e859105bf6e92716c304ab37388e93ea0531bdc8b2ba18e44e9f4711916a12dd30c00f7ea3" },
                { "sr", "8c547a51a54e0d1127f4ca933e5009a106e6da6cbf9f9a77df9d32d24d9d6e3b5337ce0f70b1275578ae5c471153c99425475fbdbab18b88cb4cea6b451ada6c" },
                { "sv-SE", "4b44cbed79f0165b03a153dcf9bb4f0d1e1c34132783778039fcd82c837ceb2b04463098a72a8155911902556b2aabaa271a307b8469da0a31354c5a8d8a7877" },
                { "szl", "28228433e51b7d4f7bf5af1a4fca01891d7c28db0081629ae8752b7e47ddd289bcfbc0d72930de2a0f2992eea9b356b26100f139c66f4152c5be28881223cb10" },
                { "ta", "9c5cabb1f38df7cfaf03e92c4fc2193b8aa6366a580cd401bbac9f3878e4c61ff69e2b88ef848337e091af83cf54ecbdaf68761463a7ce9786bbd60709b27f15" },
                { "te", "0fe471988f69db9fc29bec79b5b5838455a3fe72fec45e95c4f1d6986235b1de63795033119365dc00c77608bda277125b6d1171ce436073ba62a66df48648e7" },
                { "tg", "ea1b6346d597300cd158122d69c1d372947e2849beaa2ed0a86ae5d83e15c4fb6f963db43b18b7889a28daef93098cee2271fe62ba39139ee91f6af2290de599" },
                { "th", "4ae747199092119bc49785759668646fbb68e4af6309b4219dbeb6f9569aeaf3d196b1a012df03777b9d9510e62c03ba0d15701dc174ad69c3c76a4ff9c85981" },
                { "tl", "ca74a41610364f538dadff00602e8958a089ca5d4d413b7103bfd878f246a8e247d318380b705f2f51cebfdf6b85db49648efd62373a6e591d717fde75dd44d2" },
                { "tr", "34bd420bdfb1e1bfceb643a28651340b97a3b625eba7a9d6fca5db6c7c39508e3d62843fa5ea7659862050eb3e15a0dcc26c41e9bee8e5f56d68e8b2048595ae" },
                { "trs", "5586e2b225c2ffe3077a5b53bcd671d6c6e7a95872341af1f1a417a545594636d71891e12f17259a788c7b6a0c5cb3c672c427a9518570878cb5bcb5c1403041" },
                { "uk", "1b30e8ac5a6c239c250ce54d85cf51d13a9de23582d9eee87ee71bc679f6ad22394dcd6cc53494f164e08addfe573f2176bdcefc78aa1b7d191043d8cfd25d2a" },
                { "ur", "3e023133fbf2dde2aac4567e7084c65f5e77f3cc593b5e73d93453512a7ee5aa69b9167ef03393f18a8e2c9e5416900b8c72f68351ff0bfd4f4b00403d5d29a9" },
                { "uz", "c06cd40886741ab2904ea341354c537e14f402e4c6c62790d3ff4d589d19ee9e1f93dc87b913c7ab64c4cfd3b3b54058e02c32450906132902e5c47a55b70424" },
                { "vi", "b30c4e7d168685a9bd9ac8560e49d2503826701292448e399c29fda30d136007bbcf973dc41eed9a9cf163d2a413d6b20c7b03d9098af559961259317a85398a" },
                { "xh", "cb134162034dfb1b4e5c925ddce541796b811a3d7727249ecdf93cd8ea35284ab2ca6e86e876277b980540159ebcb27a080e9f764fc2c4ce6068273292e5ae3b" },
                { "zh-CN", "17bfd9d386307a1e9111b9720d06297e0b44527d0fb8321c7dc2846d5784fb899ee553b3f3944f5e322e47f63ae824ee87975b48dbea19aff44569f4f3a44d69" },
                { "zh-TW", "1445ace9accb5221a197f889614cfad945cc0510351850e064fb19738b4e196481ad69ca3089dd289eacc0942b15535cb9d26d2605da553af86f8b9b3ec4953d" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/124.0b1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "9ebcb9de983d149013da573c9912036823f8ba251ea46d1deaaa5306a7f484f95ebd70d60ce29fd241e15d2ee391d015112e98ef843191ad95548a07076cbaf0" },
                { "af", "afd7b9f4ab43c6cf50fd79bb5a030338a9ebb2dba1fa94e27e714ebec3430d799c67e98699e77c62a18c1f380960b3d78cdc3a27121824c6c3fc58b7031ec3a3" },
                { "an", "71470df1f245b616476581ff1871aa221cbfd8ecade2349c50a4e55af2fef8610d3e4914604ed558eb53fbc3337407f39ba5093c53f14398acd4b5fac8cace4d" },
                { "ar", "94b574b605410d23c81178c45b7e4b692f2aaf678d653a3362dac3dcf2bd838833a412b6f56bacf14c60a3e55523cac1b108a1880b42716688acecb017e64391" },
                { "ast", "2a26193318909f960ace9195d0747051f8f949f13f40473365e31e99e1f9fbd6d152d4067added12619743904e4ec6d84bb065ef158f1cc3b1c09c70a60350bc" },
                { "az", "78a4c60f9b52e83a222a5fcd0493835ef35ae3066dbe3632b5b1638118d0b4520a8525e3e395d54482a8483de063373d9734fb9d7259fd8a2d4c967160ab9cb9" },
                { "be", "23bbe081315bfd58a04ddbb5143e3eea49daaa77001b7c83751a973a25b29becead23c850b9e335837f8d8e5b7dad09caff0f38d2c222ceae6739bc18f1efc50" },
                { "bg", "ead87f774ec6c787ab0b508cd6ebbc3a5b35cb8984e37dc33985c182996eca422d6c871d47795d06fc5ee9d66566e6f49f000e400aae89f402cf4448c890d587" },
                { "bn", "a9b026f69f2c0df0d07f87aa8b3615a0c3b3ea1bbc7ac74435350fecf86ccd081fb5e4d88819cf1175ba6a8f090b1eedef7762994d0919c6b439c88b406c0357" },
                { "br", "93ca2ece861f67e36930041e85f3e8bd80bb560af2fcf77fbd53571af8e029a95af4fdf3ad945f8590ab89b931b4188c87eb316262c3c74db57ea0547f9ff3f6" },
                { "bs", "5e369b760ec8069b5bb79a94ce540d7a0e8c240f056e9d54130fdd59536a9cab1e2edbc00014d7c065699beb37c53765841336f3751427f91092f5d223f88f9f" },
                { "ca", "9d6afd5d1556f5a905af53b34b5b82983ed0e5d21660e6b060e5c9275b3a8b96ba82dcf8eaf4792d1dad9ea4f05cf9365f290a78212c4a589970a2373eb51ff6" },
                { "cak", "0b2685856355b266a9e621c51579937f47195a855baae83c163325c683de0328c8eb49c7f51895a2e548d645f382bc62b0538dba1ad8a0960cc5e7a684960155" },
                { "cs", "081a17f941e7d1fde398d98c62873c9d3da1a99ef17b656c9bd6747b11a5a2eaacb1a6a5292bd6f75eb109661d86a6bc701a2b045514ae1db556a111cc1fb852" },
                { "cy", "68f3e0bcfd73fa0c464e94cf7e15cbbea875171b87bd46c681198533184a6b0a430be7151c413d9225414321dc4b9be0e9aa55d96a6a21fe15920ab948475f45" },
                { "da", "6bf479a1b082f0d0a07e2508c589acff770f640d5acc47f7731242b1e564419e058ebec2da6ecd2a094c0dc544533ad74c033aba42c12fd987a7a54eaaf906cc" },
                { "de", "09d169e6c06b15ce48984c355a494503a95658ed3ec445cc14124b6819d57ca9d0b1f84fcc75839707ee06b384121d6947a6dedfe2216ecccb38e88368fe483e" },
                { "dsb", "613272713054a30fa0f4be1ce54dd2d83bdf9f2e0e2c78a6eb9c8c01127bd59d24f72ff9e4c4a8eab42b02d79aabb245ce66568451009f1c3116ce2212db4e87" },
                { "el", "85a71ef4132583b2c9139f2abada4c36a4d3659196465d34c2a090d4c0420f9c6bdf37dc052c1c0ce8d9f06f0a22322863406c07de41f8fac4ad4e2d0b2ffee4" },
                { "en-CA", "8f5dd40d68b9120f5363edc4f5ee853514a73d0228173fd5881eaaf190a2c034cd59ccdb6f017a7bf0ee793f3d57cb7f3bfee56585708b9d91ab9813ffb3b26e" },
                { "en-GB", "5f36be9909c87318b5e906decad073c2bdd965d23f9413cc4dd17e2e97b3eccf8e56efe621218a83497ff7be6cd3673ec4d8993a72dd096e21e5a5f3704bb8ce" },
                { "en-US", "68e1131d261db3bb23aa457f8a2fb7efe46a72564183fc66e81c364b6f644864ebd101eef9d113561b0d5fe61e47f4a1aaf4da79b5b636d5a0c2114186813c6f" },
                { "eo", "a118e740d8a59eccfc857112c1c498df917e6d50228bb68a34027040ff6b259f520d2249a4dc0eda9496ea56647ea6dc0bd1e68d5973f61330713738a200758d" },
                { "es-AR", "a068b0dfa84ab1e758c26b4fdaeb03672c77cf6621d323b09736ca24c251f4b13263f118d8fd0d8e6be6dbed637fe0f822e5c952b4a3ba07763b755bb89aec85" },
                { "es-CL", "619b92a956cea60cee0fbad88a30a89a976253039dac9a904c91857143b1593b4d66ecbeafc1c667343bd591436b20470b8f51ac7dd1604c11855f3841b4e924" },
                { "es-ES", "9e879db35cfcb5a297fd1164a52ef487fcf68757f0fcb9300c077a38a03a832870afda52d43fab0ff1cdac931dddd04f6637ed298611b2c0cae8b3235d8bf0a9" },
                { "es-MX", "cc743c05443709987caeb748693ec161783d012f21b8cb9e1b3c88417aeb599df8a4fb0e68346548adee4d599f94f061573082dbf4d755faed642b01dd6e9d4b" },
                { "et", "25003d2447f35fa9ecf30e4429a9dd9ad7faf6e62589517f1aece438e2b82010897f76432978a61306dffa6062ae1957069cbecf556268cef348ebb8051d5f3b" },
                { "eu", "a4eeaa616697c00fb6717b6122ade20a30aa905bf78cccd80af463baab8ca6e1b98cb047f3e0217f24e6faa457f735cded4b8dcfe77d715627e727d077410ad9" },
                { "fa", "0dff6478343c52910266f84013300cc08cf6b56b150d80a73a9be68fa0f76d0156288359ecc434d639c9056695c223ae52b736ee6b83b207abb2a6a8208e87b4" },
                { "ff", "b57389587e654b24c591bcd4493d5f25c36781710336b3568b0f4540cf9fcae5d1809881ed2f15873b4c0ebef68ead18c0ead2a2672c8221d4fb52b24acb8d12" },
                { "fi", "51c5106e4213c7cb2867384c7cf90fb7af5bdeb6225288c20cc5da4f0e889e8a5addf27b22180de9ccbfe9e20b54b5375f7510752e78dac2bf7cb68af07bda83" },
                { "fr", "b77fc70aa56e0a1ff2778f45432efa548268133893722190712ead95f50fb5a848654bb140f2b3c33ff16a0d86d01306b851feecc43b0fc2551672fdee6be23b" },
                { "fur", "01a9384c35d643301f553081a92f103fa05df27bd25dfc9c6cdba1b94bb860aba15b723d2fe46f0ee62a9c91d7f9a6a7e7e580e2493ada5b55ec3d746b56e19c" },
                { "fy-NL", "5466ee5c3aaac8309df40df60bac92a8ef88edec84685761624c2158090f95fe65a2f80fbfe3510eb033efd8b161c2fb8376e08c81769fc3b9f69c5c4246e9d0" },
                { "ga-IE", "8dd7b0e26c9dc999e693b5fc2f2f5b444aad193a652b938a54cf7a49bde7b729e7b3e2269ca1f88bcdb948fdecbdb5dcf9b4a85cee6dde48aa270ec80d39117c" },
                { "gd", "8ebf404eba9511dc5b009007a6e3cd80e2ab861c530afe13b02410f8fad32d1bc59c91be6d891d57df4e7d8d57ec87273c28c8239eb351c1caf2a3e019223c68" },
                { "gl", "7e173216270e610667603b86b3b994cc6555bd096ae5c7da40e2968fa20ceb5e9db69a8abc9ff6ac4de7d2c94f72d0109e2eec109bb822e9aa8b2d5873d24638" },
                { "gn", "6b6422f84d1de573065991809d70f5cfed929c9822a596697871f1455c4fbece6c02628cc86c3e94584b78ad2d6977ece08b6a3662fc4a6a98273243d9bc4e0d" },
                { "gu-IN", "0fb32a275d17c192710de244f98fe7b4f98efd9fdf43d62f398ca545c9a468f061e1a13c6f9865913e8ca3497da9ecc99ae20e092249246ea3f11309f81ae297" },
                { "he", "b4c8330ea9a463c4bb63d34ff3da00c26830175d95a5177fd84ed20685b0567db195f9b75fe3037039012e71e766223e08821698cf3ca7aed15c58bf87e4ea06" },
                { "hi-IN", "98e507e762a77e0841aa0bf2eaab0691b67b9793440c36952af8b28d80070bace165cc138444005c744e5b024c061aceb5a973f8981e80eb00ca5bdb2113b8d5" },
                { "hr", "a5ddd6280421983ed6477fdee8ce44159865db46620356835735b16a652b759fce8b629e15d1e734902d0ce25c55ef33de7f6c6b6b533cc96d499090262c8a7d" },
                { "hsb", "85a8f3e62a7f74b6d2feb7ee9ac4bc8944dfb17c976dee6bf18d94b32cca79680ff1c9b0a8f2603804fb5158873162c8f6f50d48a30a93206da759f02692d46d" },
                { "hu", "6c0a0e201fc1aba0ea5b461967b725ce307acf45ed14372c9c5e1e847b5b12623b13eada1a67a0189bd709d3dbd58a905a4314c73f74a20b3d2e28f105106523" },
                { "hy-AM", "c5a4475250aedfd5756c230e6fe8259b6f5067d5170c1aca4a0f7a985bd57219e6b452de2520a5c7db480a61dcf3ac0be4c8fbff0928f67db65936d5e5118620" },
                { "ia", "d827363f583525c24d3512967f5cbc80b843c58ff2a1d89425effd3727870f77c03a9eab886bf853309bfdadb85fdf9ae442f78e6b4ffabc277a2370a2da78c4" },
                { "id", "ea1dcad78fec35da1d2f3e342113a9f9dc826cd8168d9e5ed4566aababf9a86d2cefea97d32cca63baf15ee049cdf7d5544eb3b827182b5ca9fd0bb4a873aaa7" },
                { "is", "780824c73a91dbf59b03058e3d8867eaa3bf31c1b77e695dfa887f9ab25ec57625c295398b773ae497db8cc17f4be0c5145f9f33e8d06ea7e9e7341462ccb66c" },
                { "it", "fcc47aa989768566c1352969014ea332dd4c320a24c75ed3a1b04d2f89968979cfb6e155f829caf0b98b789a3c30f83e0efa39bd5d9649f7b0ad5a31a2fa042d" },
                { "ja", "24e04e982baeef6e18b999496e43386ee82fe35d0b8d5895122174b875a6c614854646f50779222f02aab37a0988273cd355c2b25a251f27a54a0586b61ef063" },
                { "ka", "5b36f1bb89103eba6987bece2d160f61eab7b96079b45085d4d537bf915fcbe14ca31d62073f71965e792d498a6e2405e0985482ad568e450cb6c48a5b643e74" },
                { "kab", "067f2ec98a7cd6cea2da5eb2b6d6110e01abc5ad772ad53f6ca15a3b9e09b33595c35c23a66e827d64096818919fee7fbafe9e37f7bb69a034118cd5b2e3018d" },
                { "kk", "9febfe4b8fc1f877f984be0fcb7b73a30b3e2d88c5d71c467254dd265ffdb8279d8181b882d7f5ca21806fe51a9b554dfeb9cbd7fa4ad679732ec0af96ac4f2e" },
                { "km", "2bede7057d02f8628e1268272f3f20785f921284d1f9ff2f9d2b6222b84f17d4dae24cfa501c619f637c83fe925ff613e6c898322ddb9c4eb2ab679a577f8f7b" },
                { "kn", "1b7f46cb4f48b4a85f1769cf8584a9b6711e0333604379e081b3e7ddc55e222176a64fa74a3a03af853a24938873f55cb3a9a1999d3b5a557486610798fe58f7" },
                { "ko", "583fe6ffc6369c4a70dcf823603d5e176f63d54ce062998f23c36f2d3b1d4020458184cfb1565b84e9393f578ae58b46733f6f7d309e2325d467c1ec536cc805" },
                { "lij", "d64800c73ab896a28234b7731bd68b02595f54d044c269b52f06203cd6e342a3a87096e04e433c6834c2a76c9f59de101598a7f3e8ae8861a9e1c9a7ec94305c" },
                { "lt", "317e5d105662909f2bec2d634f4144bba3b5b62b2002a920ee9e7b1f7f6de944c4be233cb850bbaa97b6e73c4883b1cec8612855d9600d244976499f94d40c17" },
                { "lv", "6be5586b9d3caf1374599a301f956833042d22b3466cab32709d0cf423a2192d00d2d0ea504b23cbecc591bf4b09e523c46dd8f91620555305a9c5bd5a179c4d" },
                { "mk", "323f1bef89d2f28c1812936a2a8463ee194beccbb0386b7a2fa0c1eedaf3bf6dad31b70282017029a5717d094305ca89843377b007566115310eeddefbde8a7f" },
                { "mr", "a34250d8fbd9cd4d6afc3b5182f908504d15a5896737adb0c66b5a921c1545ce1982e96365b9301d315904037ef89115ccca358ff07d4ee9260d243358ce1d0e" },
                { "ms", "7cc46a188218beb0da1e96d37459e87c42a4b183a42b4f3b9d291340b4aee2f83732be3ec521a295f5570c0fdf4eee2fa0e534ba646bed009f8720a92b0863aa" },
                { "my", "6fc18c4e0959e271a1dcfc0c89f5bb6713d0c45f6072e3d20a9de80cc4737bab1e94d31818a48527b09be478f3e4b26c29037532e39e7a6bf1cc67a304d55f73" },
                { "nb-NO", "1c0ad8bd944260ed4f5eb9eb061ffdd3dbf7d7b31c7da7788ff8f1f90ee4aea586f2e3200d542acf9a2d2164b710b5b9fd6499b7bafd28d1ea029965c892e7f8" },
                { "ne-NP", "4bc4a873fa4ac5a382332200f301a4d886031fb7395a274d8a5f08d67d0d4142d7e18cb68b2ae37cb8e4bef1d702873ee13f869ffcbb9ee07617c98c404c3d55" },
                { "nl", "2d7cbdfcbb797adda6dd22bba47a572cae40e7bdbbdb181e7ff3b9e1c467c260e7f9823fe472e46f285c9a78c75090f3573394e872ab860ed851caf120f2e05a" },
                { "nn-NO", "c05f2392417dd3faffd3205361e7d694f91141e7775fdc5c962f696770b2c2d608e39a08909c78ee3829c8318a27085e320fec58308789922ca3faf59fbbcddc" },
                { "oc", "cd02920f5cf07b40abde227247577817e5049efdbe6b74809b809cd36149323dc999c13d75f01135f40da3f79da67f6d044ba0f5ff09d662a59440b94d02c25d" },
                { "pa-IN", "4ed87dcf8419c4c6193a721387219094efc13b6bf06d24984dba26a9c705c12128687cf513ef527f6fd76eaf22cc856fd8da33f555309804ad7a52b43cd1f636" },
                { "pl", "dd8554d2c49f7c72421e7277609a33887386f0c0fa7edf9ccc0c874f4f5797b3d82db0eccfa3f8c48bcff31ccdad36b843e6d930450b28116cf92d16755af7c5" },
                { "pt-BR", "df9d11059a9f35dbd6c236569db68a96b6bf446a79a8caaf40bd96b259101b18d0c2493304922fdc70165f92bc1220a00b0b5adb1537cb008cf191f438924f42" },
                { "pt-PT", "4b3fe4ca181659240eeb5ccab65329ed0a675c983877f969d4ccbe454ca80efc5e6758d8a8cabb40e1cb10e1ef9b2c4d4f87a0a253112d6ad265eaa122ce06e0" },
                { "rm", "512b44f2d121f6ac5bf140b0a280ea8b2d371a2a4ab9a62178ec6f3220c0c4b3020bb6fd364ffcdfa49a1cac9b14a838e17d53f803c0e9a8c6fec233bd69a479" },
                { "ro", "a1c630debf39395edfe346a26bb68d6529ced32c4ad4d4e8c6055b19546e42ad35765d2e739ceeb32aa99cb21f2809d2cb8538482ec5b1a1fc2f5b46f5a1e0ae" },
                { "ru", "038662ec7d4f164f041d4f1ad47aeadeef707b8833dc1caa228ce68ef3916c633a1a92c2b5ed8341c752188583404869e4b60344bba8adfb079a20a5c125c429" },
                { "sat", "1fae15d5670ab3f01fb8d7453021d643e5273c43864d14e298ff60300d42849ff87be28de0ac2baa1cf3bedb0ff508a82acc06290510e9412761bd5169718595" },
                { "sc", "b13b82db1e678e06a5b5337eede311bc50d0ffc7a9842dff391ef5854162e02a7f11b248748eba64a5a8216d0e5b71471185492643975c96cf265d63effb2c96" },
                { "sco", "94c856cc9c4adb0f5bc6430b60ed3c062c7b35252b56470208a269338504422ac6fe0b239b65b154d7ebf575c959069387b7b656bfefe2dbf6978e56d7e43efc" },
                { "si", "b259419a91a86b49b8357b32de29aaa327fcc372bbd42b08ba0480f65f3db5d7567735cdf725d7250276de99c4444aa655b816055533cb860a638f58221312e2" },
                { "sk", "0074e05536a10e0b7ea49179cfa11395c62084ca461d7cc1291d557179b7f4ff2245d3aa36e06c2b299402563d27262ea7c885a382c2797cca5e6d702db4db99" },
                { "sl", "e10d74ceab877ccf283690a53b7f292b6d4ba6b342634400e58bb1bcbfaa23c2ea41ab9eb747be90a09fe24b8f3229d91413a639a59cac3215bc828b07fdc72f" },
                { "son", "094a4d443143d038724cdb29e36f472b2176a5227ad877a9966666c72d00ca009712daaca86025b41a5535f13af0c74a7bb8f833ff7c35fc48eeb5dd060f33d9" },
                { "sq", "b33059574ff12c0db554af2c3ae89ec7d40284807218f46a0933974b3c5641631019f9c4c24d0ab9f98d0b8e5975ebb1229d2795674d9428cf2e789a470d6f92" },
                { "sr", "0288df16dff0e350984281d87aa50fb5e25627edf119868876f58ca8c589c21f8a4bff5b27db8b784ac01d3444a330675383e6f413c122324b826f085ff3aac1" },
                { "sv-SE", "deef16812e4b690482f596fb5ae48074b859beabc717265e64e698844cac5e9d8f1f928d9da5796a1a33c923d9970648f5b56f1ebc612dce3cc7a138f06bff4b" },
                { "szl", "51f1f47e63e7161ad54be74b0cf6856c4ee6d80549f26cd053ff1f7e82785cac89d3c5688386f70569e2383c6546795d46f76777e286e9581965997a903ff2b7" },
                { "ta", "442aec9b3346117929474db913e18143fef53a0a9e5e3f409b13433213014f3c3554ffa9e3972412728cc6b20f35388781c64661baa933d4b4d6e6379a966624" },
                { "te", "71f2db3e871ebba87525647e5994e2058e4c268b22939f81716602190c0070e3a26cb11c7c65f1db917ee635485f1ff230ee376f6ec8e615a135167c36172298" },
                { "tg", "6922ad2f410a16f6838a5898558620954f95cedf5f24edd0e630dc7be9afd1bc46ebc6c9fb0d03451581c615db8a53926cfc0d05e576353589df950fd2a321ab" },
                { "th", "1b4054de2c39f7e2bd135d27eade8827f6689bf5165079a93d652c9b5b5daec61b8eec1e3997fb995d4f3d2a7b563afbf9ab7050dba58b3e8cbb1c717296b1b7" },
                { "tl", "354c1a7d65a704326fbf306d731e102e3b7096bcf56b4b832bc0e7efe4579cc4aef065b31096d51fb01b12e8829b0ff31670189f25ae799b48611558bb47328d" },
                { "tr", "b4d29a7b60c780def2a147204f440ec092f2f787748e757eea35819cf687a01fc4950845eebffbc760d45bf64dd68b99de799a0b9e322afa9823b20b28d8994e" },
                { "trs", "c54de0666ae42ce7a721103bf658120c84256836b76642db868aa435fed780c908574b5429c11378c5c8a28351f26b6f7befcd4415b3622b5ef5c730126582d8" },
                { "uk", "f7236c9c3be081a4860902534ac43936d37c311522019344933cc8ef9def65d1dc9ddd2b7b4209ec0feb3957560921cd1e0e8f50cbc89b8ee64d342c3d861ebe" },
                { "ur", "599d415b6b709da49d1a47016ee9ff7777acfd838ca4888183c34c07b09a8e2444caff9dbd8f18e59d1d430515ef45110faee74d5ba40105e70818e4c6dea56d" },
                { "uz", "16261d17fd2f75831fd6964d990618617f60703264c4c023b3c9b99c5161156b44a4cef906e3dd06c475249a87345e40f61629c88da27cabb5e238785f0fa440" },
                { "vi", "c16715e00f09b6d13341a5133fc57ab0514dc336cede20d12f8268dd1c5c160e5074275af8cb8f650cb3a55799c1e2dfdd887d278ed95d90d6e735dc7400490f" },
                { "xh", "d71625e1bfb96d1d82b9c3f0ad924392c17dfa034b7563c074146aee643f1865529a06862cc60848d69aaeeffcb0a07ec994ab5653d387d24ed67165ab7ee4c8" },
                { "zh-CN", "271d770ed94869d47bbe9bc7c412b4589005269c7f8b5e5017bf2d69101637663c782264de99885d6f4963e95ed03e3fe4e1b8f417251a88af43b5048794a4cd" },
                { "zh-TW", "14dc53764142267209d2042e28ed7fa92305c1e7e7b23425cdae146872932ba636e6c3bc7f4f975fc03bfe6f1f5c21d5d94a9948af491ace6e3f69d14a3263ee" }
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
