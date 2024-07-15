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
        private const string currentVersion = "129.0b4";

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
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/129.0b4/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "bfe78907700e54490f2c1d1eb655027243ab07fe063ec7f7d6c36939b9b6ca6b1d354e0e371df30aca564e94ac97970b68368213cd3508e94af4fbde73d1ded9" },
                { "af", "3bd0732c93dce5d545d8cca7296f9cd29bff096573a02cf6ca94aa08ef65f88caab2213ae785ab505affedef84b33cb9c576d8f2f365535620c90aa25d90fba9" },
                { "an", "4f2d7eb7cad79929e210800292d04947740655ded694c3b29ffa19f84c6e51dd21cce015ef11889561478299892fc2bf881c5592747bfb560dcfb35b0135ba71" },
                { "ar", "ba5e97f42df973045d6e6d8e3b72eb0838094227dc890fc534cefbcf97da1c383b21cf0e6090087818a2e92ad519baca2f5a8fd1b0a822503784770403214cb3" },
                { "ast", "3c1c473e14f301c52141f6371fbf845024d9025f28e2dcdd37c8fc644c606382c3df5e45653c69f5d3603be18f41cb52b774257ed85b077eea7d7fae77eb52ce" },
                { "az", "af5ec277d1842ef2b5eb1628cff2d36d097d88eb1b40b0bc7596a3c24eea9fb12c766eec7abbb71cc2c92ba22cde254963f3b9a533bf1eabfbd7592a460f7b6d" },
                { "be", "95e1f708b4a3daa94b2c76b06200ec563fa07e22bfb81ca904c18e62ba6785498a23e1fc860ef40e7ff35b146e3d28f180c45092734d5eb234df68789d48bfc3" },
                { "bg", "40f6924752f0762f3cdec5f54ad074ed32d99e51c52a4b3ca130a620ff6647907cfc6faf62bbd66988b5beffca659833db290700759d18c10c08f27397100172" },
                { "bn", "60c51fb7e893ca8b60f37c7f52506830c094cdc091e6ef6887122a67878348ca10bf8e12e91c8690ec110599d3b6b3cc6702b7e5bb4f96f04ec305b102fca25a" },
                { "br", "aff8913754f743f677cb3266903521a54cb77c293df8b5eb17101a813db414b8f4aade02cf7ed18ce29af189bb48c73b8f68d17326866891bc1adfb6fdd8d125" },
                { "bs", "93bc7db2837dfa330afcbc005e93291f550ae8e634706775a62390e852fdce115bf4ebe2605500927ac8507a34efdd92f227ae44d8a73747b9ce3bd91eb88535" },
                { "ca", "586941d8b6d96766222dad1f67f71821e915658ba0d889c48f68ab3b4cc7d8ca891054bb5c3fdba94d1f7a82bb20ddacf925288e816e7ccda0c0db2dc8a0fc27" },
                { "cak", "53881d72fec62e01bc7583d5cc84b2f58327149bc82e1a79cc9b59736638d86f504224abfb9301336ba6d1d2240094b7da8a652055be6a2d7a523aa2a70d128d" },
                { "cs", "62c689455def177db58ec66aa59dae6acd1e9958b851ac76821be287b66f85adfb8bab447fdeb01c865f4fc3df2a965316864fd94ac58b090f4d103fd57b8ed5" },
                { "cy", "02674afc2afa7c4b8d6c120bc5bab2b3cdd54588053ea5a1466f57ea0619eafea4684cb816b7a99a5c19f975a4a90b1ee54c43b0c04488f7eb0591d87707a01d" },
                { "da", "f16b14fd8efb8a9e296bcaead1795766a28e0ca27a436edea3a6e7bd38ca19f17d1009922afa2f7e1b673490e80fdb20905a6896f5d262bfda959c75b983eb5d" },
                { "de", "2ddf6e0be4941fd5228f62de1d7d55c574a22621af0a0943e314512437992e4bfdf5a9ca83aea12f26682b1fde912d0eac29005dd49104476d43270762f2097a" },
                { "dsb", "b583976e6e5403e09a61d060869a98550ba876a8b40cf445fe76c9a62e6b02f30293ef66b633843708d0bbec3fb1d65a81efe652769b23869db4c71352b3f92a" },
                { "el", "e437fe4247ac9afbb489b66261120dc326307056a39c635c56b79e6a3b0437134babc8b6b5127bc82c0edd31d8258723b2ba2c40612914a6ad3b0f126f28da37" },
                { "en-CA", "477ffb418e30c33d773a9c443e1166141e216fe7facca3075a520300cd5cca7a97b9361acfb1e00eeff1a78730897bb7caabce55d5e954e7fd5fb1e601c78850" },
                { "en-GB", "9db37b2847827cc7cb82e609a8ee664017b837c515646e05df359a7675b4a39dde57980eacc5d3f7fb5bca9ba330a3e80c7324a400aab09bddb40b3906bbeda6" },
                { "en-US", "b1f992c674b5e14a2f345c2fa707d8635073d686ef6f0aa794f5b4db65ba47a767564e217831fc618f61a2523ed5215da7b2ba288f7950bead4205cfd89df712" },
                { "eo", "a3a1a9066e3e83251ff9eabe9d665ea6d6d0f1b99fe0431f46b6b67de33f7f5889312694f94bfc28f9ff701ebe7a1a523c43640de4b033074c7e45e46fde8828" },
                { "es-AR", "ddc095c24928dddbb85f7ffdedd9b1dc7db01eb94c074d77095a904d7727a3dd677bf227239e59da4bf2dc0bcd4add5cd5f77caefae48f0766be30a22ec361d3" },
                { "es-CL", "3f4fe64faa6995bf7db4f7ef40471be8c46e369f5274bdf28fb348d1f10a0b53d1f99004ae45d0a808291511bb6a3f472da225b5d77969949677a8939839707d" },
                { "es-ES", "d2d6ad5c9ea853f95f70f11827c1fd3df226f2f3f9dc27321dc3e79881a9c281ad113d5599cd86448f491a64a7c6aed2f301b5a4c6a56ccf32cf3c7ec0ba96bd" },
                { "es-MX", "bacb6ae27021c2600c6c0e0c798a9ecf61ca0f5dd7f1f107f7aa25e670f493e88ee81758c9541feb6a7cea3eba5ebc2c7a478b3d43cb7e72c7da0696cc23bb19" },
                { "et", "e943e876481a2d8b96190bffe63913bf9be840350dda50ee575434f3a951f0d387d0f81d80302c819cd774216c273518ccdf400398fe98941a2743f52a3a88d9" },
                { "eu", "d01e0e0af1a1b2e625cf3cd5fe8ad194b06fc03f8aa454a29f36d81425dac7a71725af87a518f2330f1aacaa8575a9d4f26d9c7ba10c42a2c68915c94c885201" },
                { "fa", "08a61b2ab5fc37cba5e3707559d4356dd851bf1c52f05c4f5d05300fd8812f68c476824d9a9f584406aa9accf08bd6d8679ac6f75fe50fbc787c2d7aaca2c937" },
                { "ff", "7bd8a87d9d655e5570136744033460fd1bdbb0dd7bfca23913ce92e58c3f50369e571020b01c91790eb9df6060bbba0f1c51b9573eecac5c525e361d9ea532e1" },
                { "fi", "79480393da472a1cb63fc79bf944c225bdcb73a1310d4d108156f0013cbc940c1a55f336ba2b21b3202ca59fc07347c5221554298323a2fe49d733410403d9e7" },
                { "fr", "29be8da504f6d232a2b3d7b88702a2f88d7dc974beb44d47eb0983ac5c8e98451715bbf6261fe4a9287dab30362213b20200a0d3042fe4fa4e4540395c33dc00" },
                { "fur", "003f3c6db2f0b0d3b0ddb50650838e9de59bc45202eae3829d089f45d14583c4e1cc22564bf64393236d8b7131a735e4d01a3f566babb3cf34eca860cd76624f" },
                { "fy-NL", "3d424d58bc95986e6ff1b0ea6d0abe653de9ab2b5cc82189fcb744b6d9f9cfbd720a7e0d322ce48f476ae2287757554b3b643ea4d3d693d0b523ca993dbdf223" },
                { "ga-IE", "f4f0c67474577bb658f35737f90228f8ade3f76af71d0a25307c752b00fc8a9a7c7b334a09c250203d67175378c0ade4c660cc14ac055ce84ee04c7ec99fb1dd" },
                { "gd", "7e3fb80c5ea1589ec0730d5172e63a06a56b767192d7900a87ce4222919010fd9f551b6618e5b902cba853052e5f5227ddddd2502b4c3c8c1975215c522dbfd6" },
                { "gl", "9b7083b35ba5b5ec5e83d706ce1eb1bd32e300337d97f111df5c286c94c0f44d7c64ff85c353b489265dac9d9958e674af2f8bd6b91230c61a70e5861eeb2432" },
                { "gn", "661a1bb0c30e8680a2eabe63fedf8ad630f360f630171e402d283d245ae88f26c3fb96961d59b70654c6ae6651e90c1362d5f3ea90fdccd5d3950f0f7409841a" },
                { "gu-IN", "6c63e9d31e3fbf997a6d78354b823aee3cb884010add91ac12c5c6c1dc1790c04d1cf11a1bcf2d6306ee4aa5aef6042ddd56b2b930c63e49cb318ea311386088" },
                { "he", "ea0de217139afe68557e11f87442aa981c251d0ce9274216e2b04d920d879aa001178c72e75378f8a82a31b1883ff2f5e010d92ffbd8e3dbafc5e2da87542aa0" },
                { "hi-IN", "01bc3d2f39c47536d3a923a4bc86cf701129aa6fde34f2193861670f4961179e758e9e9347fa2499e7ca332a46f2c36141c65877b503346fa95c0a86f1178bcb" },
                { "hr", "e4ba2a7f1551c011f58d9b20ace77b9ecac91adc5c2954784bc628b92ded5ca7aa8340adca811a41805f18c3049ffc7ee2281ce613a49c104e1da1a4396312bf" },
                { "hsb", "d589b20f9a57ce539f56694a6c2ede5230e26a74a2db6bceec861fcc1f2f249bb89b918186043497a6e51ee34efc7187cbb63b4f9ac49b7c95e453111091b4d3" },
                { "hu", "278a863e7b214b8fede942cfe6ff139a85762e2205969b1c108d77d9c8c7bcae45b4f70b7dd074ab436fded576323cb2badbe14d36bf14e3ba53ac6fd6e49a62" },
                { "hy-AM", "1a8d99a87c66513027414fefb368e201c116abcab2cd5db2b1756b316b2843bea59caafaaf5fe40c43b3b295f4dc335f8a5849cf4c16089968555ee6b4f074b3" },
                { "ia", "8aa79a45b38c91629bba1d80478c2f43e7f23a87edbcaaf291430a4bec0e62b1638e0829cc867d1b42a51cf1d1712c0ff43c556ebc60aad47c3d473327787907" },
                { "id", "6528bc723f2a589c79caed0149aa993a55d8039c71785b1167dad756ed4ae0c0fe451c25ff739d66a76857cf2159d46a33a4af802248b60718aa0a20113690c3" },
                { "is", "41ac727853d2009c141399f8a3200bff9e10d35fac7bfae64398cb54cc209054c72c07976f9aaaaa17d260c669770639f232d66fb12c67e9753f06470d5ed371" },
                { "it", "a110ca4ae69114ca3399981ead1fb98ab3e0e2a696aa93a6f552c4b738d2fd47280047e046713c581613012fc1f012dba14100c8a3a316ca872b2766cb3f3a74" },
                { "ja", "1f3a375836c8953b6f5729e205366d9cd259098e53729c9685b7414ca81294075572fabf2a171ff1df59f1a609544a98fed9a6cbba83a25370dc698e6dc64de8" },
                { "ka", "d8344bb65395a10c780d7c317eb189d444a1a7c0c6013e8a7ecf33a52eef471b6d640c187f62cff09933908fecb0546a65f00864467ce6afb46017fb8c519433" },
                { "kab", "c9f7a4bacfde0255aac502fd4565e00bc7fbac51465b2474ab82861510b421ed060a130794a11c0bfcd0349fdb093a09303b7b0b45e809b98aefa808a366c4bb" },
                { "kk", "f38bb0dc9ecde4e56018bd08c61921044470453176b309c9c3127d32fef2da38f481cecaa5d28b86d9ae04f10beb4ea49250eeb0c7731f7945545622221b4b64" },
                { "km", "bf525a57ddf18ae397f6d35bce3df204520bee6f8bb761e12c0ce1c3fb737598e34c48b05c53fc97f4c69552387c9585214a56bca6800ba3ffbc9cdee748a901" },
                { "kn", "15aecd7261b54330e9a3f405e31608e3be1bfd46aabbb9da0d2300b88280549caf72a817616c128a420b18f79007c92a35163aa4ec248f621f536e7b80df0f79" },
                { "ko", "b17d27efb9123a1b656a5b33c52d6dd3886196f64121f2078428b7279d7b089ee477121739200aba22e34151badbfda0f631591ca46a83388e55a14b1c44d0b4" },
                { "lij", "b43d3d750f6b7560320922ad76dddd785fafd399f17dffd65277cf44d1678f762a289ae7a8c9f30219ab9d1e15f88cc6bf0a0d4b346cf71d44a76eeaa7757e8d" },
                { "lt", "0618858306db5a1695dcb4eeceb05f085c6a5f9e3fd67642b40ac68e2762d713585a2eae8a999751caae03e0b43ca302573ec92f21839d3be794cfd377addcd0" },
                { "lv", "386e2fbeba179cd94c2ec2f77ef24b6efe5c07639d50638c9fc9eb456299980f91c6d9f690b0517274ecb836f6e78e0fc682a23ec74596e996e6d515ed703bed" },
                { "mk", "7ce83cb3a5d1d5f0416c3319497fb95037d290d2df1538af90e17e60bac336419130d836e5370ecb1153580b58a5cad91c42011f6ac4b6858e27c052b18e0dc4" },
                { "mr", "504b6cff007baf9d3088c2a544f501b8b99b900c8114fab004a26a1b3a86887ac9bdf895ad69eacf87668b84e3db83643c495d63f92bc0149fce7a2f92a47250" },
                { "ms", "e6554595ae78eefa16436bc63b2955a2d846411d8c664b4a2e2ce3d37d54d7d462c9b4529f1224793ffe57e75e97c84a9dfb643ec5a9844486f976e30f30e884" },
                { "my", "4884a17b16edf1d0f0c8a65d2ffb7981dfd9c8bd9706b78f81f94d8b7545c6cadfd524b1f7d04e4f945fe242fdb9b9a15572fdbc5d10cab3aa6dc12292ea5d8d" },
                { "nb-NO", "2c1f4a6711b923445a3820545b33ffe7e11afbcf118ef88bec015fb2eedcc423d659da755e2072adb514af350cc874d3a37f1b69ba986e42edc2bd235f60de8c" },
                { "ne-NP", "e84b1079f96d90e1632481d063e2c7ecd870d3f3bb21bf6b617e0102fd34ec7c4ebf07d8b64e750ad9d99c094229c0afdfdcaceff8def3590ff208915e418c8f" },
                { "nl", "20040e95e84a27c9f34590f3187cbabcbcb0cc4c97b822260e50d269c29235cf084298fcee9cff699eeecdb39b76f051a0a094b43a61a730c21436d4f159aadd" },
                { "nn-NO", "96659de639f85cdf8ae32d172c0ba85df018534c295a9c647d72ad8c2a00b5215dc0e069ceef5de6b46e741bea9d2cead544025006959eaf882bcbe75a891ced" },
                { "oc", "ba536c15dd53a9039df497cc282f679bede50f8ec04e8c83adc2334516bbeec02e36bd63552d4eedcbfedaaab49d6e0b45e7b7f6c5d02e69eaaf791904ccc252" },
                { "pa-IN", "35a61fa04a1297ace070bbd64b3bfe619d7874a1ba4f44c20f61ef9f86cf45ec7975299f124020228672c343eff380190bfb08b992e67d609f9b8af3991d5c2d" },
                { "pl", "11d5ac2cccacc73ee78653f01ef0beb9a850e97f819a84c300a770aca51d87e1fb436fd781005479b70b7420ad20df4cd58b9b620f4e3cb9566141571555d283" },
                { "pt-BR", "9d48b567f6c31042e96430687947982616180de0c14891508d4b17de3ff9add5898cabfee68ddb3d75ceb8d8243f44652b39cdb300ac3cfe076de51db4cd8116" },
                { "pt-PT", "4458f2066623d1e1f0417df59b5775a4d6168ad2455d288fa5c9d6df138c6f427b4223d4f9cb9b0a65a436455a8c34866ea42b71dc663e4fee6ad73da7590ab1" },
                { "rm", "5c00151ec4cf3133518d968a0d076c36ae09613281f9cd0ca9632083eae42f7b8f3eba0533cc0f60c08daf2ecb973c1974b1397d428a7f48ae36c3b26faca32e" },
                { "ro", "55f877918aaf27e20656b26f8fa28d5eb807669b83cd7d295f4e2d2aaf394e17b29a94d1f1731c2545caaa96a8f4303e5b13822f0d970df62dd5efe4edf2cee3" },
                { "ru", "32fb32640c82eebdf012a5abcc1298355a488dbbefffe053d77dc32eee63a1af2fbd24f5dc2a8eb9660eb59f46322fd036c9bc6db9f29d6f46641343d51be7d5" },
                { "sat", "0dbaa437470904f060f563ee08aafd006b1ae1c4eff93f8456292fafc34a4b2ed7b934a26a110628463e0d38b4d93ca0c6ac49cf4d7761207ce552900a4b35c4" },
                { "sc", "164a2e2b3b005ccf6c6de19038ceaa89ca88fa09768a530a8c03e5c57d82ccab26881ec22d81644df12410a5d44949d498be072c9266512b9313c307f64f6d47" },
                { "sco", "c7f8fe6b174b7b91ca921a3e2a0465a2dcbf0a64482530f092e2fd0922f54a55e4580a9dc5ee4448c8700e73cf8c2ca036fb5ee2364d8e7cb134c743612c5c80" },
                { "si", "ce0a5b86a676cf6c6b895697664e95ca496bffbb3c3f24ffa34f1485e4f2fba34e99ae69a4192a81682db33db411aad837ab29bf480d3c2a30842cfe8c09aa95" },
                { "sk", "c49f46a6cec84df3608138369f3d15f4a07c2aa8bf68a3916ce03e23f6cf6a8d6c4c31ea581ae6b1b4effec047c134f47aa4d2c718721d2796aa5732f395e7c4" },
                { "skr", "bef1f2890f440eb27905e0dbb2516f58661ddbc75d4316e438038ac2fd438b9be658b1e289208a1baef61ceb48582a91363de295a29a734189df5f07c024ed82" },
                { "sl", "2e7c68b329cb1049c38f95495eee1c36a1b7c3074dc84d86c4c811952b6d353b6ba43034c3e32b76e28471be74200bc207587c8c65eba4df1c83c8b0f024e582" },
                { "son", "3d18c525abd164872a16256b6d99c10d1a36c5d8f0f4526d45344abd5aa6d6dda558667b671cd2e1a2b6ea1ac2bce66401bde8c2442edcbfee918b60407e218a" },
                { "sq", "7c64c9d0a9e069178fac8351f0df84001e8d6db5e2e7865b70f7f31acc93ccc1c9549d0ad620f32e95af9e45c83159fc44325d8f3b3ec85fd4e10235f16b2f2d" },
                { "sr", "db8f91f174b0d4d92e1b17fd126c83a627571e4785bdb1698ec48232a6fbc57c6a018dc9eb3b0b2edc013b7ef938074acaedaad6253c8b305f098b3af6132291" },
                { "sv-SE", "b2d14bf5258f4ca8bade05fc5671c017ca23daadbf05676a333d02e2e1f42acbe4417d191abd76ebcfcb89c536754e74e3d17a285c8ad880f548138aecd184fe" },
                { "szl", "71178615ce78226491ea2bcb0df9d863b33d0eeb817d79d9f03d7137613ab54a175423c54904a31fe8aae372ccc2f2052cdf9c9560edd2c7347bc3022a9ef48d" },
                { "ta", "64becda813d5e83d71ffa48cd10379a7d212099524680d77434f6e3538e5b3c39c1fbfe5bb681d4506e0976c85a1d28931dabb59da0555c5b6f5b7a3a3905c7f" },
                { "te", "1b3f3d58c8b2fa9cdd325cd132a39f18d293f0a277bca51fbaf0d57b86da8b9b18a58b9baa6d083ed5979c1d3fe71b409df416e6c943a980e08f2589fb150aad" },
                { "tg", "7f4c1372dbcc8c6f66f373838d122f4096d0f417ee5572676aac29ca0a09e1ca7ecb6ed0aff6dcb35f746751eca4788900fffe415867ba29893e7909c034326b" },
                { "th", "cadf8778c53f86eaa2f23314c4d2d810dffb60a8bbc15f3265371d9c423c95599446f8c3659424b1b36013ed5765b0515d2a5251a16f5b854505a856939a9da0" },
                { "tl", "0e055915b9f65d0353454396d1cf85aee3105e9b586a8a45548f7de0cc0e3259fc7787d1bf485252482b745630bec92dbcd3783030d7b0dd8465e14dfb0f0746" },
                { "tr", "985c8b2c4fc3bb90f0a02211bc405fcdc35e9e27c8e0a134caf00d53a4d951400f8821f300eae7bfc362b29b4ecc61249f588111911f4669e9da2d77ae4e84c8" },
                { "trs", "4c6dd48e4cbd957d6b97cd1c4e03efa2a47d8544d11d8256e222c77c8c57e3e65435d6cb7c9116e6d29befa8b3ef785b57d82e9d9e79e61ae4367631fbd6653f" },
                { "uk", "a6fb89fdebc817a0859f07105d571ef3909ac83056443ce4a11ec2ea5356ae9aef20e08ecb4a21e0e3e3b3fd29a4b1e12095ff15dc8bf136e41c392c7d3d1945" },
                { "ur", "9cd63e0bf26507e56f203c4c508f0b240ca27aa7d489681b13eeae76a25fc88bf13b78327f368fbe702214b2d8f03e269f64ae1fbf9c86f9c0ac7d4b60d1f6f7" },
                { "uz", "ff6177b61cedca34b82381bd7123d7c2e000eed6c95bffabbd1f4579d65de25f99e5777e2be8827218703246a1d4417aa63d263dc12ebca37bc5e64dba438e62" },
                { "vi", "477e55fe48d86b7769903f04150c6ec25bf68b4d70fe01288d09687d241c10e2a1df5861ded7bac5679c68a4627fd8641a8e3afa33e6a0e2beb27b0f77f1f0ed" },
                { "xh", "2ac12b917ed123aec06ee896b26a4959df574ffc3a8fbfd368a8c141e103d134df5bc91f97240754c89d9c044807e1225a12d645f90d35612ef1a3f214730340" },
                { "zh-CN", "031c5b965edf6f957ef593398f8cb03bc1175b014736eddd91ea8e7f26a6fb36b127d6d1a26494be1447997daa490cac55076be23ac34683770dcf265463d7d8" },
                { "zh-TW", "2aead441f83c33f6a8cee035e2c07e23072deb37c363754ed86bce9a1ac23fb1f1ebaa24a2857c5e75a1d4accb4c04a4c26245120b9cd4e3961a6b4777d6a43b" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/129.0b4/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "ef7d8cb8c622a2822a04748370cc611eb00a02a548335143cd188838389c086cd6be51c81073a8a5673a4e68c175b781d8c128d57569da33aad65432afe7cd94" },
                { "af", "ccf7d374c52fe87e05580bde6a34a2482139e9c16be83ed2d57c8fd304566d3c6478729874e5968b6e55120e90ae7cac90ed88b74ed8a657a92eee0d883b7fbe" },
                { "an", "aa082489e1fc0db7dd53d74460b4a967b3dd800b70970cbd43b22b15fc0872f76dea823364cabb0cd345ed17014cb47e3c4bb7939ea545d22401d2952a6695bb" },
                { "ar", "99f21114758e4672f41b2390dccef51a716ff3a6e2027a4a47494b0336b3d35ecad46b93f4eb67dbf04329388af36ffa6637817d70a2b5286bc15c3656174369" },
                { "ast", "8971c990bcf5ee60e7ab844164d972d5e050f6b5feba43dfa29aad403da30eecf37b626fbb77b0cceb24d7cf9043dcd4984ebd5d515c4a08d61f76f1a44f3fc6" },
                { "az", "3ea543302c81da0982041a36a98acc120a8756463c131e18efcfeec87249d0f447d2a2dafb32ea8791d9912189e03a121a88a1eebd81609550f6db9ef3d610b7" },
                { "be", "8ec9e05a5446de6e3df2141938e8f9662e965c744cc96f59dca8f6f3318e2275128f239c65c08f60fa5aa4230e812c3d9a39c780f11b26139a8b4a4e88a92f6f" },
                { "bg", "635adfa3b70783c73338137389b16487c4cf510c94cd9ed534529b7ec8af1191a4104418b097e3292f607ed16f80d8842f65f84365c00f3eb89125fab25a9704" },
                { "bn", "84d85ca7a8cfb6ff1feb863272643a3fc18ce786e267da1b687285e11c0fd769b3b805e019c8743283250303d814a20a845585cee3ef474fcc14c7db06df1802" },
                { "br", "080f573de6a577181df9236818008ede9e5e065add3031a9c646641ee32bf52483fcfb56fe505aac0de6370d688d97d094ccfdd7fdedbd21f33938c986959fc0" },
                { "bs", "e017e16e252223223354adce5c72bd1ec24b720555b2df6924738d77c97124af01ff2aace50948c612badd542ab88abea6354392f8cae2bbd1f6c661c40bf093" },
                { "ca", "3fd443eb7214c5fc4cd7db2d081027a506892e5b1851f8f6c1fe2881f142169b5338c9b05c780730aa88cdda252821cdb235475544f35a6611c89fb5b9d76b52" },
                { "cak", "19233399d611a2a1d78c08e772567a3ec02f680c6c769c2293f2d4aac380d442144b99b2739c43efc82e19309f1467b4dc94f362f27b247db3ae37892b74f53e" },
                { "cs", "aef47b992e62dcb11435abfb4d2e525576fd176949ac449af4e39c261ef5154f58fcb3f46cab11c28dc6a51a38095824a0f19a3aa7fe2dc4f3eff0e0a2eeb78e" },
                { "cy", "88565e9f662b29fae98b3124e39a0ad88655a54a1b7e383517a05cb6a6d044a99bca5292e7137c20b3a0b803ed6eb1e22a27fef58259a0edf51dea2a2467e44a" },
                { "da", "6530b491101fe123c8626074fdbdc6b6ec250a14939600cdd55da2d56b68fc420a08a2cd17409adb2b16dd0add1b0c5a8fad87178eeeb382921d7e3424e7b3b2" },
                { "de", "cec1bbb16af47e24101efa4e8f3b4dc89ca4ad5d7b21727aa7d90227db0a37a34d1e663a88195738f799a822f36c2dae214b2dad6f2781cbfd704f19388e6e5d" },
                { "dsb", "17a119fd908baa1a11282ecb68dcc5a4a06da333b6e67ff3b6aa917343c6915549fab75114225b558efe71ca9d503f2f1845172ed45ac2e2446f5c4c6db2e628" },
                { "el", "7008bfb95d6f12172ec0d534399c1b3459ebf6359dd59a2ba87075405798b0410ef559898c08530a70d7c4f5b86c4518f3f5dd03506708a94be1bbba7d623a47" },
                { "en-CA", "87e709453f5b7ef1fd3be991379f7a33bdad0b015f5851291d02983f8e01a6e87506603c9e8bf08349b0b78c2ba54397af03b3485acd3eb22ef6416e7a7e979e" },
                { "en-GB", "ea59102c8ffff851f2efa620da91834e150b0eb5f7fd386f8576b614dd8bdf88f865ecdb938956f5ee8bfb3f9cc3ccb1fc686d1d01c1f545c7a341ea9373d9dc" },
                { "en-US", "d3602179a7025d7a6bad8641fcdc4494a4f5169ce7cb8b706431440f784f6040b9fe3144d45bc9d2711784a6c435a4f872a0689acf5f36d2813eae216ed6159f" },
                { "eo", "18769ac7f1f0a347fe2106e7643d73ce97ce5b6772e6a89bf3c2866515788f894d80c006fd0a78a96a25ae52715567f19525682bca480aa53a38724e4d313ab6" },
                { "es-AR", "f0441caebb3ad8f030c201f8dc474caf5b7894780fbc354fa8d67dad9ba8a3c83cb0ac571055866630052ad957cf57c1ae594cbd914a6efe7a1fd6e1ed9ff5f1" },
                { "es-CL", "15f1b9bf7fbd4443cfaba83b3d267205cbf4da05004bc100274c6df899e32072196ffc2965d11797a16181d396446c2312011c38139b7660968e2fe6166c1fee" },
                { "es-ES", "120ec6f54cedbea5be1dc9f048369d0b2d45b24436f7f3bfe9056a1234e3012d2492d97201214f96e309ed52b85e13fd15ad597a8f336760f4c646ccffdc817c" },
                { "es-MX", "27235d4c24386797090c3b36b93399beaccbef49076641acd990b63f7777cc26fb9cac5d3e5eb8eba42de3304956b8a8887a84b9ea48a77809744d9b72ec2ff0" },
                { "et", "225eebea64c99d8c54744918e0a1bfe644d0651ab0bb5229c230d291a254f6c30cda67d619f536163e3f6b52ace08aae94b0c0cdebcba0d3d362070ee7697029" },
                { "eu", "0e324a2d8b2074a16a885c2afa37aa28d2cd9233297325424546911375c876ea66493cdc5f3adcbcf084deacf7c03b204bdb75331f0fd0e35c14982952297d73" },
                { "fa", "d341a6507e70fc51b04a4354f31c593c932079deb47ac51f51f0a9c2174cf0a9121b22c484ecb195d69ff9db21b2100663d7155e8c973824f8a0a09c4cb3377d" },
                { "ff", "ae01d82f1ba523cefe8d339e3202d1329c344c99a1d0890b4e9e20e58e2fbfccea1989cab9dcff91fa4bb690f3fc2294de11b4d1295abedb8b2fbee86c5e8af5" },
                { "fi", "251614ca60dd2d2be446b2f11d1dca5158e6caccddd68439dff293e786012bd642e7d46071298c9837e68e0d9044ba7d87327ab1bc677c023e0495030855f467" },
                { "fr", "140c47fa05a55b3850ce16a97bfbd6b82a5ca75dfb53a6c65659703c379d94cd58372d4c7c4a61cd4b27f22b153f15ceccd2d751fb7738dd27e1b5fa6144105f" },
                { "fur", "e034251de1d42ee7e9a3b647e581ed24c220ce5af1b7d3806fce4208425d4cbe88a4bd6d3f696383dd6b27301d6eb52ed1b92646a19132dfc2a3c52091951396" },
                { "fy-NL", "03c3fcc419793d1d20c2acd797d93ce8225ddccedfe3adc4b8fa1c78b3829c699afd67fbeb7a62d77eb570d6ada9e5e9070c37d701bde67a4eaad0f77131db03" },
                { "ga-IE", "7d828fb72ffed1f06c0382613ea297e92820d4943ab892660fba5d3961dbd64c101b20e1ac053b68b7de66af1a4c09961e8d5faa2a8ef8085ced297497978366" },
                { "gd", "4f4486e599fa00e3753fa36fa30155a61d3f2e909d94b8be423f56612e39ee85e50d09075f5318dee73ed737164c1f12f152080f56e6796b4368e3ddbdcf0843" },
                { "gl", "0e33be2def38ad77aa43531c35393055f5cc23bda81ca0f38dcefe2903a5df919ac4c24fb761bc9c81f434c9ce4bc9db89dbaf7ee6cd0fd4439f26ddb4b5aa39" },
                { "gn", "135d35d4e13a2b7e3f67eee9e37b3afac3b7aba56266b94c435c731c6e019c5869b2c6efe53f104489551c94825ce1550eea296c56bb7e6492cad3bb3f7b530b" },
                { "gu-IN", "be6f38e40ae0de63a1cb15ce776d69847cf2bfedf584b8ff11baf6ba95117e1d5cfdb3595a336533d2cd1a6b40ec7f9a42f0635cd965a4d39e96eb9b80a608b1" },
                { "he", "31d65430afa041500db0e661b85601335185b7d7a2e0e4e7fa6809412dcf0206e7a51c7c29e01b1f62291a210644a15949ffab3a105f8b49e06a71e046cfef20" },
                { "hi-IN", "2e976a9c8c111716df55c5ff6e262f040dbe1caefa0b8a0e5f6418547f6295b8b3d3300bef93224ce3fcf2bc805e36a065bd7a5361745665cc9ef27197ab99f5" },
                { "hr", "b45a219caaad7fc274c4e997d7239b15a75a8c39c99a11b9b01e4ef565d03e6b16e53c37209cc737f37b1af28c9caf94242ebd0ba1d6ce467f5b775cb3f2dfd3" },
                { "hsb", "474105559e1c196bc05b5dce46a1a90409e0a97bd7d8e25897a0646b55d1f0c8576cf8ed48287ffd901c8f29b6f324e0e672f61580d88929ec8686b733523ffb" },
                { "hu", "fce0e8fe8351eb7d657d41224eecfde0ab3e8ae73dd71811d70a1fe684a458ed8707c0629ff95e0d67c0e86a6596c0d3c386fd0de23714e25d3b46594b6f442b" },
                { "hy-AM", "40e60004e88c0a13687c62f3759fb966ba3b1d82b77274279214c514d650e21086689883a6102a0ddf589c2d360edd9540d1c80435988e08f880ffffc10682d0" },
                { "ia", "6b86a7642bbd2821570fe3e974dfd5e2b354caf664bcf25c3578f1e9455d4ea04502b53c88659108da040d6566b4e5d806df8743b09d9a09f98765f1554b1e1d" },
                { "id", "b1a9bf429ca5be6b6706a2532009a377f54345ea1142076587368d1908910a390df35d92753bff10262885e298d6246ad9c4f267f0bfe86f01945fcd6e19dfa8" },
                { "is", "4438257d2d650ae738158f4e48c8b7757bc1ad1cde66d26294bf9bbd5cfb6ee2fdc374ad4f86331e73aa224a636ef5c1d638d1eb9818d3dda324d2e44e00080d" },
                { "it", "fa3629bccbe34c8f7aefa03770ef103dfda98f8299aa1adc1e708ed7a142889f046b597529c5dd7cdb784c32051a6b6e45f844deea8be7fa159585c2701cc276" },
                { "ja", "0b2c7de8576381f31d47a15d682fce6be07d30b1c8a9e3f20bb04e7776d0dd0722e025bf7b604b3366c6cf312fb072d7260e50eac06f52fac92dba9db7408d6b" },
                { "ka", "eec3a18ab5946662ba3e6d3c543c3fdbc06d00a0b65080ca2a281af9c029516bfc205c4a88fbb650bdd31c4ecfd630e3c25bdabd71e475202ccf35a9adfd2c8a" },
                { "kab", "154c232899241fb8579358c5e3ab37da5b68382591f0249cac53b20cbbd792aa32da07dd06cfe5546a1048c12e041c424a4d02f566fea46a3e907469716d539c" },
                { "kk", "f3394c9c13aaf155a63a7d72a9c595184efd1cd230ba8d3299fd4a5290283a0221db6ba02dd748b04abad4ebace6417271e320aa4e3139bb7ce4fad601ecfad9" },
                { "km", "27bc0e0af0833555cc988542be7a6d563fe7101f9fad61b23b7e747f37faf753d05293e34550b9b4c6680f04a3d323e1173f2a90751faed91cf6bd6b2e1cf817" },
                { "kn", "b4e0f900182a6904a586389f90a85b8d9d2dd3596b9b9cb2a868e8beec5d393d295e092d4a350dbab3379a0bdb95b3aef258d8252f1ee55a238365b5acd5eafd" },
                { "ko", "645b2feb415c6bf184b698bb39b01a2a4ff5203444cf72b664a3a43e66cf226c940dcdfb9f552383f8181fb831ee9df1bdd0893ed89095f1b209e04563daf857" },
                { "lij", "035ab20ea05898dedfb53aa61eeaeb2eb605029f020f07707e81f544a240aab7531701368bf70403eea3c8602cdf1dc2299683638f8da2e89870203d6c1ba48d" },
                { "lt", "106d14df4c7425eb98ed4b1fa0e1855556efdade41676c859fc1223047ac07b2bfff981fd5cb2a56718180810751956ea44d4456a217d080f3c9fbcfe8412f6a" },
                { "lv", "65a045b20c353b61aa8154977334c69454b7b2f52743b1802fc4a0e51202a56c279d72cbaeb4c5657e02781aa6c3b32db3c69dc4b3b9ce29e2cbd7967ba29d77" },
                { "mk", "7de962f32f900a2e0fcc4e4ecbbe6c9d45163ab92e91660b18aa753f041594382137e429a9687f15b3cdb6eb65165d681eeac77d9988097a7a3c714335a213a6" },
                { "mr", "789f3feed6f3d3e8042e9d6e9494b18aab3794bdb4dd7a8f31dee9854c2ff3aca881f1995bde780eca8cd2ae64a559d55a6ff6678af67244136a73c26bf260b7" },
                { "ms", "75e54ffe654c9e05cbedd0aeb6f48493e2ff7aeaaaa48ea830f73e750b448430c5e0f02c5cba138c4dce7bc1762ca7fcfea8606ff829cb105ce463534fb98f1b" },
                { "my", "80be3a81111484745d98a6446a36068042d257b2e39e518c4488e9b22223b21f01be301d4ef45099e6edac4eb6849bb03e0d9c9ab5387a1478922df2ed38dc55" },
                { "nb-NO", "4275f476a3817e0459b1416db314ae831d52a1a6cc6b087669e985fee760f9185b98254a4da657636f442d244c61c87ba4554a3f493f1d45a56fa555de154c58" },
                { "ne-NP", "f3e308115c571b70577c903a470ffe5432c7249ac00900a5b31fa32222b82133562def52beb2af0bc45039692d5459710e51a5e8ebdb9f44a78884c1bd90ceb4" },
                { "nl", "4b3b736dcf7e6fdd8f9105c4543a35ca98bfe7bf15b2f98cdeb26fa4a7cd1bc7c9a42492c9ba3f2282f35195f179114da2372470d9b7c8e42a8947607dbc288b" },
                { "nn-NO", "942f7e02a88bd3caaf78d36a2539307b5eb8c8e387e13ad5ff18a3413dcb7d6fd2732ece9248b72a81ab50fcc3743733e927e6ff5de57034fe3c77278355bb32" },
                { "oc", "d2404365bd05d2281ace6281c7e058b419328ea129eab3af4e3103e311389b5936fc55c01c1d4e9774f2f65770cdd0102867e6a70362704d88cb00e9db87d61b" },
                { "pa-IN", "75e5d9a238b0e476c98baf3aa4a9b09396a98061ba6c4ec829a0e676284091d118bf38b78b80db006d583214e6365a8cb5bffe1f4f173a26292d2d256989d1ea" },
                { "pl", "c1efd9b3ccba43a0fd6cd9d0e909095de403be7699161b1ba54fb6c76b0e750aad624514a953b5748477ca9374f99580e86bd51377c63d8440904f93d8faabcf" },
                { "pt-BR", "27000d3460260c50aea8e2e1df262987b76cc74fa3ad79e596fbe337f6b65aece4505f82aaa43196f10488a971fdf9447de0635c29e5b6af6a13ef458759390c" },
                { "pt-PT", "f8651c37d82ee1cb3ec4ee82752c52e3cd2ae81ad91c4f686b5761315423a797f03dac0352a946a0dedcc5dadc2d137cd0c6c7ca7f1a49040eba57eb68201128" },
                { "rm", "c02c1e4aba837d8890d99b9f10db99c786eca39fb36af4959c1bc84330ceb97aab491a7317706eea89911d6fb641886dbed4b11fb203d18ad50691c82589e07b" },
                { "ro", "ee252002828843eb3c2f4f03aa8196bf57d4794e306ddac250d65c180f9c700e7b49fb8cce7a16cbf7f9005e9fcb4a276bf45eb41463287073e3f37822e8afc4" },
                { "ru", "eaf34a37eaac1a9249d6f4efea6938a3fde05fefdb1eda4ec59f3f1e6bada5a58d1a36019e6f5c6486f0f0fb01c175d15eeffd6c00214a4e5155f30d2ef74256" },
                { "sat", "796828f3d502ae86aec22bdaeb66c0ee22d7233ea151e2f31377e866501f94cda2810012e662db7d561736f08a4f7510321211461d4b25ed489b9e2e6314ad7b" },
                { "sc", "df2870d0c143e8746ea4dc40efad04244f6dfb4ae01257cf737662d3fb75cee7c86c81c8d6414e3c8e25787feb804e2b50cb4b4079c54b6369ce9433dc288d3c" },
                { "sco", "6758f2cb6d49fa7c8b4e79d9609834a0a70b556426ef307de24aba9c72bb7e910a526f919b61278410e1ffa37bd3de7816270da33b14080aeaaac9bec9f648ff" },
                { "si", "c3ee4fcfa7775de500664bbe2491e60316674e4de3e5bf25ee67e0950bc8af66f2188aa2b8832bc98c0babcacfc4344df26bf8887a568a907a501d625c53482d" },
                { "sk", "136d6dddf29bc26b2914396d076a4ea2b5ef2ea7d31217644fd81612d3e203ce50191b13d4e1af478203f01b16de1d2c0865a3b19b4057da87ec2cf056db3dd9" },
                { "skr", "8bcbba6f87c9626a5ca28a875e918de5b8d5376129e55b2adefe45e1dbd418eecd11ee0df8a1de8763f3d54782f6eaaa6420c7c7a0eb57e58c8238470d341d3c" },
                { "sl", "043e8072070655e1097fd1310f37860462cd4fcf7b256f9c492224f921aa9fa73519bfa72af725f60ec64bea24bcd5dda2f0ddc3052a0a6873dcebc8d2123a52" },
                { "son", "816d39fb559a902cce266cd470e0767860aa372fd8df4e8f3e48f92cf5804da4e487419be35a7ba75525741eee9e605cc3f65be416955eed757b291247accd7d" },
                { "sq", "7c8fe816cae362301a385cfc22ef3e7c918e93a2748dee2ea272d660aed9d91384b6cee10b0b7c2805dde1690342dde55831e3369d42cbf23c7b2b4b0630ea0c" },
                { "sr", "339a5ad2afe9c2ec8f0cf50acc049c821065897658c53cc4046a03ec639bcf4ec8d221b6bfec8a1d0ca4ce3b9a0c227af8b3ae419448e000a29d035cc21e5019" },
                { "sv-SE", "fbf109ea8e2b649f01f3f44ca413b2607d8d08a1b9b891c94dcc5daa4ef4d1dcbbdef3bf72a0537be5373cc8da66fbf20dcacbc8018bf3ec782f13e6b4f280f9" },
                { "szl", "548c424d57d009a77e5678052c558c19de0efdc72b923236be7c60126851649a4b140f8b82dfa9f36586c3a3bc826bc833a580737ee1330c92ac114d99f5dfc2" },
                { "ta", "dd64e4e48cb140be9736c10fc8ad7f587450f847621a9d379d2911ad40325d85c1dc24ae5c55f8644f0d3b85261f1903e8393ccaf36663182807adf230af869b" },
                { "te", "5869d45e1a0780adee67dd2684e8076204651394d1e2f01a8f413a489a5e255663857f10eb3069ae06f9d6205668f191d93afd3d89e977a514051539fe5fc081" },
                { "tg", "33fe36fcc2f34c2857a772fde7c07c864d6f0face00df2f85c08bda15e4e84b2380154f0c640a985cc031832f851680be88b02e94c1e4245f864240c778e91d3" },
                { "th", "63dc1024dd946eb4a1c0afe34d4238e2fd546b969bf644d541784bb94eb987835754ed79572877b766037935c3780153429f28e99454133d0bd08a33e2786381" },
                { "tl", "05420ccd31dc21c5cd33348bf09c7e5d095f040b7c529e51c958d8a35cc07866f18372aa527d4d7ad3aecf7bd3391701e214e26138e2131ad482aa0d0657c1b8" },
                { "tr", "a65d499459bc81d730542fd804d84b29c2b3db81a7481c24cfe074456d0dab2d91c567d3a79dd8ee14369b0cf0b1032863ecc0d24ac7e5ff83e0967dd8ba422a" },
                { "trs", "22a793c3a1983ac10d7150ab2d1b90715f2222136f30e7d78c098ed45db8a92dafb4d9b45104d5360ee5e54a8daf39ce81cabb62f783619f9ace1c8e29307811" },
                { "uk", "d17c170c85ae0609c6e6df70ad47594313b7367dba6bfd440dc91b65acd3c11a39f0f0b27286fdcdd6e5569b0badff960094bfd6eb29790bfd9074ceef24ff17" },
                { "ur", "b2d365dbd67456ef5c68e03e7e2efedd69ce222b277ebf41b3d83d17dd92631879627525b387b998b268e79aec53ca27c33fc1ea5932d11bf45cbd2737ce6e84" },
                { "uz", "fe8df8564b23ffef620e5d15c07b6b1fb700261aee2ddfac660723a6a9eef4209fe9b54f29a9ca19f255260be33a2fc2698b8936474dea5842c56cec65c90163" },
                { "vi", "34efd505a400c48ec3c5e8caf3a8806a8ae84cba725ba1ae470416a0c03a5cafa541a064e89733b6c580662170f8350320ddd7df8cbf2d2c230e8ccb0437c39e" },
                { "xh", "edbb00f951cb28e3961baf6f3c498afb751fc8a2d285595b9940c9cada22bf2de673263c5df53a051914adf4bc193a44daff502d91aafa31918848af89581fc6" },
                { "zh-CN", "4a093a9dd1a2c78ba7a532e66099afa037b7ad914820275d1c6cd4e5de22987da142544bb7b7f3bc576853dec9f450eae79b0059b63dad734a9d83b7aa66d3bd" },
                { "zh-TW", "ba38415d76674494ae19b6d3fa760534433c19886c037a262659c696b3cbcfbc73a8b69c6cb0ec20298955bdffb1adfd83678c45de2d56167a743a88570c3e59" }
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
                // 32-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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
                    // look for lines with language code and version for 32-bit
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
                    // look for line with the correct language code and version for 64-bit
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
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32-bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64-bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
