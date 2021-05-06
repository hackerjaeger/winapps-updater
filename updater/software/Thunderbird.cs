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
        private const string publisherX509 = "E=\"release+certificates@mozilla.com\", CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2021, 5, 12, 12, 0, 0, DateTimeKind.Utc);


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
            // https://ftp.mozilla.org/pub/thunderbird/releases/78.10.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "f8aa052f1bb42185cd58e3fcd398cd6d2e3322a1fb45e177736f00ea5a5695287451529cc5091bec3d12060656eaa0ba46782469949fb389af7409251768e216" },
                { "ar", "9901a9ae0c32df2c9de7b8e982f98b3c5c250b770a4e408c2becf22e4f80a6eb736d2032ab08711c2712ea3dcd68de99a69f1337bbf4f0cd5bccc89fd66d2670" },
                { "ast", "4f5a487bb1d9f855da962aae7856af27774ef502f5dd99d978acac269b27afebec7a7d0f2ddb9cd7df1836045987788dfc508efb76ff1964ad183054ee238f9a" },
                { "be", "ea50a69159868c9b5af00e43bbcd299339feb38c3d65e438b43bdc24ffe32fa81594cb19a561e5095a4c569f5a3cf3e46f0571f4257bb8c5933a722f5533fc59" },
                { "bg", "a60a1b0ec5a5f5ef7c319ea0c956d799e0ce620c7895239e3cc6af93d9362bc4ebd2b11449afb1890d4d7bee868042da01562e1963392cc33c79df15b60fb544" },
                { "br", "053b0f1be74da7fa323c5a7d1e19861c367fe3a9e366c3f9f3f7393658c2e97dde6598bff5404c54c3b4338fee79d9f6c14b37daa40f510c7c9d19a61da680fa" },
                { "ca", "0b7ef1524288fb4dfef45c169705fb4a70dd8eafec2b045f893cdfc3eb747f888dac88d3394afa465b7ae644cd7e68c4b87a856795d94a867b64b404eabbcc93" },
                { "cak", "fd5a4ea6b0b1c5743b9d03d5dce15a964ada762a98135a5bcc872015e1c1500c80a664d445b09389d2c66add57eded627a4dad2c73d24cd8aea6a540fc67a4b1" },
                { "cs", "dd4b1562bf97595ebbd5cfce6759559a1fb8c9033b932f8de02eddbb5c0f118dcfdccfbfba02d1c6f486abe7ad9fe0dd95bcbb3bd9f741f0228e172c50de3135" },
                { "cy", "027ec6ae098ba51f5c42fbba03e4f0705a328a90135249ba28e3cb0325d9ad6968843681fd4546d0775b79cddc0e7f596787d5532fa59a10143d1ed5ea78edf6" },
                { "da", "2bfb99066af4eacb9ec864686ddc03ab9fbd8c62461be7e4d1b8ea315c71274d0a51b339df1440fd231332cf7eb7e4dca7f8c55f0133ea9085e4fc34243f61f4" },
                { "de", "921c694bab865a0682a2d6879c4dd54ab32ee0ad1e3fdc506cb6d8858a83fbee399144c1d0055dff9c3d69e4f23cabea973e1713ca9758f57bcad78a2057c635" },
                { "dsb", "a42915f86eaa8405a55e1d9af31c86c86cc7dd989be8b6e678c5f96c01275656b7ebffb728817ed148ed3e1464ba8066ed565b969cc0d5e4c4d25f6f8a40e6f2" },
                { "el", "de8e1695b36cff65fb59a5a69c199c65688cbe1081cf94f9ff76b0627b55c6af889b1553a97ab020c8bc845ad4f321fb833c14a137e848daf80a3a9d472f5229" },
                { "en-CA", "03a2031b0fd1991d599432423c4f8db01fb28491d73b449b1a7393420726a6c8e92d1311afb02cc8d5f9a060e4bb8285310e3cc050516d02aa294a4dbeffa2e7" },
                { "en-GB", "3a7be793c6d3946a532783463e58a32a3efe3441ec7943bcf5d0e21ec823f1bb86c6b53680ea710bf9dc7446eaf886aa5e393e1043b42110a831317cc4bfa3d0" },
                { "en-US", "0a73d7aad51b29a7aba98b62d737eb6cb6a7bc376b170e2f2d71b0fa51a25e55ae015b7445c0c4af8dafc284dc1d069beb997a7138cc7e61ac27779177efd731" },
                { "es-AR", "df00c54008ae044ddc03cba883503034260368d1ea9d0f6ab8066cb25da95f4062a206d7f8b5419a92661749301b84d5e04e03336417e90217fa939e68695a10" },
                { "es-ES", "4d939fa46e834cfa3e795e945b916fc1af60a4b12a3e100a47a896bc8de3c68bbd05d436e0a0e51ec6bf9f6c139b79861416708f3450023cdd1b0ca74249f323" },
                { "et", "e25040c20bacd305defebd40fe9a1e68df4633fe66f9661536de06bdf846f0839746754b81bed727017776be3bc8c5cfab3f886d23eaf42bf36bd1c26a03a2e7" },
                { "eu", "cf6556864b1e72bf895a57eca3a91da2b1fef46f3654cef9ddfc4ecabe8f4062c756c0f66d537e67ddb2d947c98634abbb42b4f9bf37c3a8341aa872073e44e9" },
                { "fa", "9eff2dd3572a21503e31c19defe936cd9ae28371b1bcb6d27de9673aeff609f8f34098d9766c5aaff7e3738f580a3e6e5ed6ce250be0fd76260193e090c59619" },
                { "fi", "7cafb6793fc501c3205092b249f2c5b3f36be6a581d2cef557c52d568d04946b44e54dafccd375266fcbadd8463a4dae643f4a51b67d22f887190e3f417fe214" },
                { "fr", "f5581ec5d0c5f4f34cea58e565a87c2968cfb63f2ea3edcb2994be733af759e098a3a80528d27b6634127455995bb01d6a0b53a5c49b0f396ab5d867df5d3a3d" },
                { "fy-NL", "b8891e9fd3deeee203391f411ef354818ca25b9b47b673969e62093c7d62e2886d026cb6d7fdfb7bb215848d4f227ba032504f76c75b07569e98e99ff0471f07" },
                { "ga-IE", "7d19f9fedf00599dcd1e32aa397881a95f59a1a5cc07dc0bd63594ffee5e77518cdad5ef04a6241aeaa4541fae4d266960068d5c04190df87bc13e6e45762065" },
                { "gd", "4f083e1b104f57023b93f88c9d616b425296189aa29e48c52bc29f6111695aab8064cebba7b2967a185f9466f7b0626d41dcf3ac23a03bde7f525f138fd7ac9c" },
                { "gl", "24cc56684c1b9b86c5de1cd99c478007a476433622ce5031b950b304259cdd34917c7ac662286fbdd7425eeec6ab239be25c3b6b2d33bff1ba58ae91fa116f00" },
                { "he", "0395b9a55e4147830f374f5553ed5624a4e7b69ea556cc2550bfdfee115d0ec5829faaf6650baf3c605b8928001a25ff45e8b39c536d3e3f89708bbff65926cd" },
                { "hr", "1a1e09ea4262c25b4171202b6ed3f8b6f7e71802802ab78746edc891e3221b1699d76b88eec982d16c04c0e1e0bcd38388b75d21a1fbcce77a575b835e3fe5eb" },
                { "hsb", "02517d7432ecdf1fe5fac12941121d4c42916ed731d8ec92f3a13b1195bd130049d064da52ff50f07bd7c03e49c8da8235167f5a306577127ad1c588b6301d3e" },
                { "hu", "4ac75424ab10f1f99e3263fa518cc1679a5dd22680018224ed5ddf2a3d76d93f77ca71a1e2a63c521aa4688de58cfd75aef0fdb4b1b6cdccc7b5c2652e8ba84c" },
                { "hy-AM", "51c66cddf78ffb7606270e56b081295667d57b0099ba8dcbaf7a0fe490347cfca5f979590933a5572957c0c34aa54ea183aea62a2225e4efcbda373cbf929acc" },
                { "id", "401e4c41e3a2d15716100319656a898b8a276fa36cb07c5b843d7adff9d801f5919c3bfdb91157e54fa5b3225d5cacc26e39b0eb11716da743ebbb092b28a8aa" },
                { "is", "21d2e5ff897f202aaca329f8202410e22dc93ea4a6e00037d07dc75e5d19956802956b584ffcd273b5f1f0990da45ae704f20f447c165874cb4574c9c0d2b889" },
                { "it", "862460ecd487246b4e49cd01bde14cc479d94cf40dc20c5a48cdbb2bde3e672d4cf9acb9e71b5031ceb082d430e6255bf41346e26fbe16d7024d5fffee3e0d37" },
                { "ja", "7c96454283ab29cff0ecea2096c87edd1974f43f5d8bf289c0f445c11633c7288abe924c564d73e66e31cabfcf2dd7abd2df7a7bbcf48706fc431be40eb661cd" },
                { "ka", "dc76361825fd43af146587697476362ff0b360abd60eb29a542622ced4f4689a56c1a09363aa3b958c9f0348fcf2261c4d484d3985b16e0d97ec35e2629eb330" },
                { "kab", "b43bee89bb951f3ff929180a9cf5380a2a45927817bd495621b3ea5a7fb0e6c70ff6f615316a8e6035c20b3715203cec7537a72e7f0d3302ad2482284bfe7799" },
                { "kk", "5fe90cd97c55869747624a378ea2f73aa8e6e9549aef4d8de7c4fb1ef4dc49f72b75102c7afbc7a58dd39473e1d9c1229bef87d243e4719b5e36095b7a79d188" },
                { "ko", "ab8dcc294347e29245ecd23728b3c5990898c09cb57e86de0c756cacab5043c28d46ce5d1a3e5fda79b209172720aa5cd5b53afa0c230641b04f77fd43d52e75" },
                { "lt", "da2abb54c37cbf2f1f860e54839770d8833e97df83c44aa111412b27cbccdb176d23ce46cebeca040c2dfbe0513fa6f4d35c9b39432b0583401c200a1f8f8029" },
                { "ms", "ef3c16f8a0f34d1ad64038cfdda635f5f11aa77f2ef5c2548860fa396ee70f8c24d11d77748da64310decd2b51f362f6be4b3560c9dfe932b109076c90cbe7c0" },
                { "nb-NO", "c6addab0d0049efb9fd59a9d0e5386a2177083aa834a2152607b651ee88669963361aa35ce1dc4b2710c7475fb7f6b64335167d0261732ae0abfa6899dd5230a" },
                { "nl", "c7ef794e20d18b189a6be219ae6add6721cacd91d0de20c3d299c8513fec5a0990e04895631f8562c4539a2d95cea24953daafd207ec12439e67188bc29c0a46" },
                { "nn-NO", "ed2b728d1447bc7ff5c053c9d6eed414760f912b5410a39943e23d7574de29129716798ce8db8976ea052b03194eb6e0ca3c9bede6c1725a9ecf809e85df0681" },
                { "pa-IN", "a8e3a9673f7fcd98f96db7d17315f1aa655cfe3bf2c383ce731b6b2a93f38ef32a4b72e9603219b90a49d4b157537c1d7557de3125abb587f515b167efa7adea" },
                { "pl", "7a6b11f1b7931cba8359c6dc2f76785d114635ac8bdd8f52f9f9ac198f2a6fb4bd2ae893321452c88be975cff136a53d1ef2bdf312d68c69bc0a84d0824e925d" },
                { "pt-BR", "afb7f122618db1888872873cb8ace92bd5f9713611ba22f099a4cdd877d9ee7c81518d16f9a6cdf9f53a36f7e6ab12c3482650adc9dfc76dfb05cc1e246365d1" },
                { "pt-PT", "b9fde45912f00fc85e77099d71b8a4e85f2cfbbfb38813a95341968890d6f2b0c9647efaef0285794ea689d1b6ac7427f1a353a4d6dd337127dacc23067c31de" },
                { "rm", "dd0545a3caa376a87996da39225039cd8b370e24d965cd1be91ff83df59bf4e778b096c43b5b05d981851db034c20d49582fd60763a5f8f7848f21bb0f8dbd19" },
                { "ro", "b4e5a1afb7ed9699cd1624f156e2bde18334942b759e0235abc35659c783cd45c54137039fb063ccc371ff526f125d24b232ec0ba03149eb66577605b55deef4" },
                { "ru", "a0a5ec8000b04114434ef6683f459b6bf1f57ffe799790b71e49f2a97442781e6793640e839d30edb0a7f3413975e26db65c7c76e51968183eb9fec676515ec7" },
                { "si", "8ec9c5e67c5c6f989ecf816d2c930c968193179da366a50944c407e15d1eef52372e1fb041ec468b9bdb4069fa49ee0f604fd3908c62de8d9d753d7599500620" },
                { "sk", "591c7bc6f81946191fffde73b72f80b36ddde4364087cf1bb1f0f1bd8bdf600b8b9e41ff52e7276030940db1cd6cb13f4bbad4ab0953faee0d630b8fe8294ebd" },
                { "sl", "288b9737dd6542c5a230b2680f35c4e0535aad674a93efe341797115f18c9e4bd50458e67293fa31ac8f9dfbcc71441241c90d65b6b72ce8427ba3c9a338effd" },
                { "sq", "ec3e26bccf8c60f2551e4dedfc2f30cb147f1b4fb0d41ff963604264ebdb568ab85af72a3cfd5d5668aee2a3193c9d88a5db5a9f6dcd1a61e76341d5c9922648" },
                { "sr", "acf37ec1769cfbafef15d98616361bcfc3cc4afcd0ebac5f7dfed3d94dbac3d53579267fa993a4c0b21de8eff24099f86f93fe1259a0ddc30f46608530f84919" },
                { "sv-SE", "7d87b74754f5648cd080f9b990517249b3a72a5d68b468c4d1d0f9ffd7ed71e8ba89d1aeb487c6abb51898b690cda9d1b97fe009bd5487e293695e0e717a507a" },
                { "th", "b00dff44a3ae39c143c9b23f1ebc2a9be86a1c23b3a63bca4244f60c7209096076a74d2fb57e7ee3ad108644fac917ee74b5b40200bb3dfed9207e9accc7a381" },
                { "tr", "6694fe6ef3aba6022bef0ff0c7acb7ad8ea99ff65f0363a7cf78876bb6710d43f88c7e7acb93f41f23a2c972dd61da7efa33e36be01d175e129e57eb834b9ec1" },
                { "uk", "7456810119afcd26790fca0dde8345164b1eb8603228b3d1bff1e0f6254b00b06ce945e0c4db85ce33a098cdeb991e4c45cc534e9f0e18fcb6f730198576f648" },
                { "uz", "27671f053b9cf1ba7ea5b816fa4215bc5fa58bcad6742e0db65bc403a799cb4d4885e2e39ffaa190d0c6e5740dcf2d6df33035844d8e546bd803dfeccadc46d5" },
                { "vi", "5159e178e7fc538a181a6d4afaa9cc79efa41ab7b4d81de54a3dd14102cab305416e6de3cc0080c604fa699a167372518541b5865ad0bbc64fbab2677fb2bc27" },
                { "zh-CN", "cf424e9edf831679d92009d13e7939a4f07a6c77ed5ff3879c3a7c1d505899a027d8726169507447deb30050b8d3af58c16179451102ae28f3f53ae3061d2256" },
                { "zh-TW", "2c3e2c06d0ebe51120a69e17f0192f565fe673a03b7b3b9dc0c7297029efa79619a9317d1efa6c46e3f3706dcd99bda4e876d84eb15794bd6b29d48b0e842e82" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/78.10.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "e882b17f0596c1da8e581c2a98218a09a49d7248cf26b81aec081854387664335f4aadd8687bb62c8c772df7ff8e41c9c0151159f828d96b3a6a339e09d4a21b" },
                { "ar", "d01fb13ff950eb9c0d62380fe74a4d35701d4fdd9771106772fa708652cdba1ed9ad6b411cf2434489a380e72197ef1014b7bad74b3eb3fe8b730e013a4d1312" },
                { "ast", "e00694e9c97ed8ed0fac2685f71dc973f2efeb0ef0a8611fa70c71285f3f5bbdd6fb62266013fac7ba9bfc1d1ae810492a7926bdf009c53f0b04c125c6bf4a02" },
                { "be", "b05d6cd83384247b3179907d6bb354f38c9bfb501e55defb3e96d98330c6ce2f5c6d39fec0e8c5517dfc9d389885974e1bfe8229b01a2703282358dc0a871086" },
                { "bg", "c817a962bd35c793796f4e0e644b1e7ca87cfa618802e1e655d7fadeb427119692e1b7da4af0a42f0414ad8701e5c750f681b0f4a313422b5ef4e68d6b68b39a" },
                { "br", "df4ec279bcc4613aa84c0c06bea4d81dfeee76ccf58ac7b718e95098527f8a6a31d29012ed7dc42fd6c82c39715d723f1954f9fc0ed15d3bf4186df904b11fa0" },
                { "ca", "7a3a241f5d861f097d3cd544455768f5ea38efdfd04353c259ed27cc823fa80224a7615abc3600a8b5ebc3b856d07097d525c271810857a434bb51769c03e931" },
                { "cak", "83b2fa6d974d695cfa2a976f39d8aaf095f8ab5ca384c3ee57e488bf1c0c0f7001fc6f689450173a9365c8f2f84bcefe10a22127d180ca63f5b227539b3ec9df" },
                { "cs", "a8b70c57b47612e064dadfdb41af8716fcae7ffdcee05663cfa294479b66a6b0d09bb6b250ad438b7af86f1714f64a22210936386c75d650200870c8cf158496" },
                { "cy", "69d285f89ed78c4b5ab8f62d82512babba0717ea33864a37f670a3bdaccc08d1d929fdf79d46abd2cd82206652a911d5903bef71e3dcd17930ea54a0864519fd" },
                { "da", "cc27c00ca7b267e73721ee52141643c652cae6f2f4663f953f15b64e31cdcfadec74d87b9c0ce3bbc46797ea2e0348b64f13b4550f5512727b477296e911992b" },
                { "de", "047e2e1aa554d5976041837fede32adb69ae9a1e21760dbed5531db09fe432cfc30827d76dad6c16b25d1a3d57a8476c44818b19aef73a6a6b2c36c4a6b011aa" },
                { "dsb", "370d907f1ac3253a203cdf06a654c672cb1799495fb24dbb5c3b60c80e927f7e914d5d705024db1669364af82033886acd70ff40412597a86462a43f8f6cab75" },
                { "el", "3fdf38ad79f9e5d3793d1e6fce12b824ce4d74d6507b94b7fe3a760f2a6165a2048957fca514d690289436f90907cc7f371a601d92de6c8da0ad7c9d2ab11142" },
                { "en-CA", "59e626d822969ed15948766fbf5f77c2b71780701e5a6b1f656e01b5b121b7d7347a8aab9b9435e3ce1faf0a12630d77c725cf5fdab0df044b8eabd700362e63" },
                { "en-GB", "0f030f20ec306a8a8b5b47eb224ea348c90234fa90148307a21a1c4165af54cd5d0d4a856f9bba91506a49033b3e2bb5bb48ee13fcaa231aa53cf5eb8eb8fda6" },
                { "en-US", "473101b04f834936e4061f8e2054cee3ffe433752de908799acf0498f6274ea9d9e9049260f6a7e0bcb455b05aee9462a212b1406bab6dbd9bdbb704196b1da6" },
                { "es-AR", "b4ab4d18dd710a8505cd6cd5f6076a37d6232d0571ac0645b49bf4b212ba4eab298f6586787d8dfa6b2212ec85e9a6fa38ea2fbed55fcd3aa1a2601a306aa331" },
                { "es-ES", "f9476fa46ea1f0b6ac31902cc1d49b37de0e69605d2499d2a9e74a565db64b9e20920ce629d1c67e9d7b5a474e3a4cff8f3290472261d6ffc27122cdd2f6a315" },
                { "et", "250d5803a9d7344a9274027ed93534a2a9b881ca2284bb44fcf0f589917eaf59473c92863883acff193c342c1fe275f096d62d79ec99a9a365dba2c75dada83a" },
                { "eu", "00412d48cd08973214708ee41f3e82eb7464eb60b0d7b1baa086c31f7548788fa8615e9723ec73699a7688bcb60e367878197a0cdfb8f3c15c9c5a659d5dc324" },
                { "fa", "b1144eb72ecf39eb54dbf6516332b8c43333d9792a0196f40e891d4ee7888016b306e40c7c014837b669e4ff696d7a6705a07bfb4cd4edaf22a535bfa27f5b37" },
                { "fi", "19cc958d92a1a955700337420bb7788a832286fe99559b0e8e6bb08cac0912814974455987dff665a2b5f774ebf94c34fb7910ce1d08b45ce2d06308f03b39d9" },
                { "fr", "ec7b00b1ece35380b59bbc7f9322f8c5085dbcc1005976eb02de9d001e37165260b6de1e1cec5918bf7a86a7bb3ba3507efee9e902fdb51a3b124c08b9e381fd" },
                { "fy-NL", "8e3b32f258d16965c11276ddf450c6814f1667728525950a6e145ec9f93c1aeac1c99decb97dd281551fb6a6402d05d6322452a8608bf3262f0a276ed7906af8" },
                { "ga-IE", "3de5c7c573f16796e42349bee91645a7f47387d77c4c65644d473dbe4a03baf9227196138ce5e442c5b677b16396eecca6db0f06df8d352ae31ca1cc8cf3a87b" },
                { "gd", "0371ea98005135c952e91e72143f97e2c8c419bf318a6ec9f4562e031b7910df122dec8f81b634022788e605eb620eefbfb6c65738b4f1ec6f6373f842b404e1" },
                { "gl", "e1dd632d41e880ad4566e9729d210503b45d32bf7f40e760af4c677d19cb4dd14ab296746e7d8409050349d43abbe1dc5aba797b064dcc1e3814d04ca740039e" },
                { "he", "ee01d6f232b842e8151e1767198f73f363312af5d5be7e9e619d82e5bbf2ea37e9256a9cd028fecd0c7560cef1ad59766c028f5edce60bc5c827059218258909" },
                { "hr", "98238ef2ea5a9978a91638c81c196c33a6bf93e8cf3eec9007472415c71d0416c1717d47e16bca36f821584bcbf2aac1b4f117c953479552612b0f6c354b1285" },
                { "hsb", "30ea32e2dbb1421f4abee0fffdc1bc16fbedccf0ba5ec1d9bc359856be9b6988aebf2e116093d3f92f3713b3019266ab666a7657f2af59a44faece6c0b86ab65" },
                { "hu", "0e942285011d11a14ad4c4eeae1b5ef2ac04b550564ebfda162595f9c78a9228df6608ff90710c1d0ac9482f0eb79a81e857478269a858e778a58d1acf88e0a3" },
                { "hy-AM", "0e0cd2c2d7f6f57540ede08c0381ac0f7ac52c6f5e1d2a753a1826328f55cb369de9f514cb2d573933ca1ff20b8eb5073c712ddc9f34fd586539f5222198dbb7" },
                { "id", "35eb90f7c5e5da041a0fb1593a4d14a32762b0ac6f5548adc367a2ac0c78b2ab33c1410fcc5430aa8cf198d7c16c2af36115004d4c6c87fe50d64d0010e9fb94" },
                { "is", "f0b2ec4a00e61c9320a5174a5a0499a0999b33edf86df238549ae03f6374963be4a9a1fa4488815a00d65f1c39cbfa5de69a1eed4cfbc71b9ba6b25382baba31" },
                { "it", "eda113367858eafde4248447ebc2587754ea9531085000c356b77d1859a412c198f4ca81aeaa1a78eab6e4efdbe39b0d2ff05e6d09b4d8a9c5d054d5c223d9bb" },
                { "ja", "5a77ff11e1fcd0b1337abbf2e940cb727b2a31e92a075e50d38c4f30faa58262a75e24f31a305f5aa586eb09532eb41ab92ddc6869712c4d956de22e35726f6a" },
                { "ka", "590afba1bb6d29be3ca82e90e5027a717c2253b8d394582f639c0871548c4c40c298738c1c82804fb858690f9be29553fb3e25647c953117c963d13144ea681f" },
                { "kab", "dfe35a302f8b1fba791b0daf19f9b3bb28272f9629bb573f78485340da1cf753c81e62dc138e99064f4c82bb563ebd30c67d3098e8d1bfb83d85d3a8151f93a0" },
                { "kk", "73f80018dbada598d6d3726c6e881c13cf89902adf3af0e1dfd0294ff6314ee1b441345b4955e5ed7e53c1668e8a3584882f17215b093fff4922c6ff57c06267" },
                { "ko", "a8458740784c8a66a6a916186108e3f9e22f8781c3bca07260c7e177b3c1b2e03fb2ecc68be17eda5cf4fc1f5114bdd7c58665466fe15270b6e41455f818a842" },
                { "lt", "1836cb37e8248fdf50624761f770bf5e50b2c02a6bcac09209de0dac5e9c83100290ecced13b3037f88767f94f59acf92e2dc7364b44073f5174d30cd8b2cbd8" },
                { "ms", "3d1ac0a10d5f0ad58b8f76edc5a71588baff7ca4ebeeb70e9550da4164a4e4a48b24ca22bc772793efb50a1314dc397bb7fcddb9fad51638ad7fa6b8d3a1c0fb" },
                { "nb-NO", "2759abff69d8594f28e7eacde39578748b2e54cd4df9836329fac7430238abcc7f45303b483bf5840f61e11361ee7955a14bf4a9e27c5efee36cff5dcce72f77" },
                { "nl", "6f3f9555d22cbc1836e92473b36702bc5f094e143a8ba3726ca09f2913a55ec049826c31321e34e190de6d2240086fee638ee767c143c12f46838c2a6b01bd46" },
                { "nn-NO", "c005283c7abaedb44927965b2a588f5e5f588b3c3d13993dde47e16c721abf1b456ae8abc8a75c60e6b178b5967d4c1cfcae43fe2c2a05da66ed2e31ef4999fc" },
                { "pa-IN", "fb1f1e1345ec9ba3656049809de50445c9e0a3673a4fb06bc8634e5ebd65f0cae06d67e9dda4908cc67119d64794168aba584db2c199f1bc00b5b991d6196842" },
                { "pl", "9d8dabb9ffc195b39da7cfc0c1e46304fec73762e20e7056d0c5dbbe65ddf7eec98f5ba764cfb29ad543c21b0d2c797be6314bbdbd547f77b6ab1afaf40a91c7" },
                { "pt-BR", "16ad0069dc5dbcc222a0852b30bea304e945a7718b51800621fa991d0c4a0e081b036081d17920c6c1dee1c89a52501501845667a3c291b308ea014bbaacb179" },
                { "pt-PT", "293d2b4981a1c043fb6ab7b29256c68528e009adc718f292ca73a2c2040fcfcd7f5eb8f0b4a63808ff6648da94b4aac71c1dba329fe36c09859d388495c31ff4" },
                { "rm", "5d11b069427664dfcd1e35b6c851ad10515fd7ff87a01579a23c9684d161355b5ac2f97209c32279e28270326dbf76e342492cd8c3c5840b02a7a3a3e972653b" },
                { "ro", "c76b3f5d46b3159e2f58b6e99300597a5fbf2e659b87035678c8815184395daf0355f8eddec6b0cea87fa816f0d0ba224922469425af4cbc0535a0cd97fdd9a8" },
                { "ru", "3d0f8c7ebcf2f105d27ac7ccef8ae32f1853a088318a3b7efc996ae73136da8e093a929135f9e338c7763d25ea36b7d26eee75026a91227fb525d37e5d6a7af1" },
                { "si", "187f3d03648ac710b2bb159ba3845e95dbc94b1dc48f157f3e5d55b7afe97c3a1aedb0d2938bdddecfaad6fdebc9455319238e7df0ded04ab75b08086b68dd18" },
                { "sk", "1a15b7bdf2d4dc4bd6548a89d79431e58cf7b6e183bdb2e89fc2391c425cbfe44b8d1aac9f51a5663d6c384abf24caff7e54713dd43442cfbe021c347efd9db9" },
                { "sl", "2e09983a8a3e37998d4e4298af4e00632b62c490698cab06ba94fc2a818a833d1738f760dd7402541452c0968fc50da0c3f0223c478f7d3e31d03a4fd41f7ee1" },
                { "sq", "6647a7a137a275c88d3dc0dbb9f228c98f4f278337d853239524e1936ba46837a03d9860782737a0487011713cfa9e7d9546db5d4a6280932055633ed5ef132d" },
                { "sr", "846f17b12b3703cca859043408aa4efc8c85d16a012912d9d4cbb10434f92570bbe6009af11059618e76476256ea9607d98be5ef150c00c5a60b26a9cc136551" },
                { "sv-SE", "f2d6cb8186e5fc411cfeacecc4378dc36cf50f2492691daa63e820e34ce5bd8c0853f71026f9bc502824b9ffd90816ee919f8aefa668be23b55b1067947b8baf" },
                { "th", "b7584ca353bd89069993abdfaf1518bdf157a3d5c2e4a158101e05cbfa53c734e8487211a724c7ce58fd8fdf30f0c52fafba55f821b1d7c4ef7467c9b0c8fd1b" },
                { "tr", "58123bdcef1ee0cc7655713d99d85a7dbc260f8bafa2548cce654fcd30fa9bc5d51a6272083350f79d435145bb21279406270e7862e346a4eac43e738e04d17a" },
                { "uk", "2a13f99bef691a34d2aea21f1b6c37e8df08374b3105df829f071b5cc7f67d5ce9634a7b99fb91106c98513a63f88f50dc84be8d89e96f88bc4c14c30c975767" },
                { "uz", "f674bc289645f85eefc00d292afacbd897bdc5e2e49aa25009150e6f11cb2c9d107ae988632e7f9a52e1a8542aaaa972b26efa30addbaf65dc446c8a57caf24e" },
                { "vi", "882ec37850a7ade4cff575717e4c53a20112d4f6ac8fb6e67733a75ecb9664f508ed06a3ab791c07cf7f22dc2106d830c4a96e3a997ae12671026f6897ebcda7" },
                { "zh-CN", "1ac635df1a6741fbb23fa86cb656ca0b5f00420c36dfb4e15175061c341fa2f7773b34df2cb984ef03b8e409263950afb7b475f0e32db0a49a9b30b9c068d488" },
                { "zh-TW", "51770e7e1674dc63ee4c8f10de35ce75f55f21dc18aa8bb8d94ce7bfaa783863f57395c30a77c432edbdf70c4ab77c16e9eb97bbb7ad249e4f97c76328be4851" }
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
            const string version = "78.10.1";
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
        /// <returns>Returns a string containing the checksum, if successfull.
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
            logger.Debug("Searching for newer version of Thunderbird (" + languageCode + ")...");
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
