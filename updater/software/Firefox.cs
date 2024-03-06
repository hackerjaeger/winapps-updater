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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


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
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/123.0.1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "4e4c52848866cced00870f195f502bb135b18b23350eea25927ade5733b577d893c35b7f9af704c4a35a9f41ec3a528183412782203bdc4438df90e16a976263" },
                { "af", "38c6860add779cc0c9e56615c6e7e2d3df59afe5b26d89fe16dd38abfbaaeed4b211a0a9a7ab78a65183e6f6d372fdabc908cabbe94e0b48fedf5940a78fe2ba" },
                { "an", "ed6bbed492a14cd86bd22f572a3dd8cc2dc2a7894ec634f69cf2730c73922a95fec07a35c36318600a561c4a729be906a199a07f03867fa6e80e782aa7c570f9" },
                { "ar", "b612d804273d0529bac984a17e2935200f8cc37c6c6d6c63036a22c9ed514986368cd7f48a78525255dd1e2aa2ba61fddff2474849315a9bd2c37780715550d7" },
                { "ast", "075ef86a039f0799d261351d6b4c1baa7291a8ecf4292f0b18ff40a1326ec7d1b6b46867e62ecf73af38541c2682964cd9efeddc72da687a95dce5ca2932e946" },
                { "az", "6eef2b09e47a5c98ed7178af3f281b3966ba0f0b92d9c2e192a505f7012d88e805ddd862019f9aa0fa8a9e02a8538baedc95b0bdc7656b0ee136143f9040c31c" },
                { "be", "d7d1924d205c14e66e5941e00dc11b80fcf5c3fc0d931ac3011c0ba1c08121e09a85195ba1e2e87c5095dd97d7a988c77e44e9465d77a0162fdef6c55c71dfcb" },
                { "bg", "7462984d01899593240b673560a62b4a54c65899597543a4a5a624bdc89f2f51e4c8d79a225f6ac8bedba63710d61739228c514218cf9b113b8d157d78e075a4" },
                { "bn", "9d93f2f459e6f241cc412193575554c8bffeae1e3c32c0041fd5fda515382b06f81a767afb9ef09ac4dafd1c2c7bde6e2a28e756412b63be569656bf9725d47c" },
                { "br", "1073f5c07229c7ee48f5b062365e821f7a585a9e4f0ee3726c363db77f28c20048c40475d6b58508c182917cd1c44be86229940db223c5f264907f794cb05038" },
                { "bs", "0716131caa01e523e8c1e014be58a35b3bf304846b12267b8ae8c73fe642232e8af1a7534a0c205c61c15e24c5ea8ecb6181736fcec7c59d81f49bc3e476637c" },
                { "ca", "9c1edd9f95b887374115a37761c56e59b1d8dbada65a82e7afb3209f2d3fb3549787130d467f5a4e50c1c14a9ef33e1c922e20e5171ec11c84d05da533eb0fae" },
                { "cak", "371a98d9a828851c182244e253bcfb3b9cb08d28baf8b49ca62cbda446c52321bf6858200d1d24c3147d753c795849e7ffec14c95207fcff0ed93e4e170f3bfe" },
                { "cs", "67d7f486a757e8a9f476f5ad4c49c89e000a48597624af58fa54a995d8ade0b423fb71196809e57c178a009f498f963530370ca1ec829b2de8137e536f4d413e" },
                { "cy", "b9809aac2f3810ac80e073b8592b2bcd58a13be884cbfcf2bbdae2a296ae8f1b82e9be9f82f2dac98230ffbc4e400cb20a6f3728ff6db138c96a97f610b7f288" },
                { "da", "3912628805655a5070b41dfe0bc6b385a3e015e2527b64bfeb9f596d4a06eb013bdce011773ef95141c89bf6c0f9f42d91c3c7ef87e134e001babb8029fb3535" },
                { "de", "8e3f6e3cd09feb4e2bcca27fcba45a603c324d0582ccaeb8575532c74838dbc0f070b78371750ebf84018912c7ed6f7a94a3ef671b5cd828eb06012f6a1f7693" },
                { "dsb", "7dbc1b48c4db173fdca1cbbd419c1304041ced5e13591679d6764989eb47ff305007d2e6494226f9aff6357485b0cd899743e1d6021b1e189eec6cec1eb6f00a" },
                { "el", "a5a0a77ad416190dbe21c5ddd434c2871537c9d10f8f6265c7c951b85d6ce45789e6d43148ee1650f0c9e53a39aa03d074f0b75695b9dc565c58d38e5256f60b" },
                { "en-CA", "3ae2d1aff9e1642a69bc92ee402c0f27c2192a35d7a81d0bb4171ed69a1adcf870fefe95bd07b84838f677296c61ff1a28f77bf837084abaf6cdbf63ff85e32d" },
                { "en-GB", "7040daecbcade808594ea8e669cccc475e08a4f50750770b9e34cd182b7422474d5f62cfb8de04b69eb6938ddce8fe79bdd22a51cd49c53f49294f6717707e15" },
                { "en-US", "ef0a33bd08856f4f9c422bc4f420111a65f15141660f6de53d99947938ced05e7b7fd159ffc521097cb44ea820200f101a24127e6408e1dc7fa711ac3b12feeb" },
                { "eo", "7f8b2f53fefdefe77375bae4895b9eeb059279c0ac0ba6b2eb07ae3635cd1fcac21b54b06e413f18689189e9838f4c25360feeb830859f2d5be6487239880708" },
                { "es-AR", "fe83a67fc5a145dd9c74ea03edf28250323873aaeaec7f6833264ac4ed6332c7f78981f438965609abae941e3fde0bcace1ba8107eca646d93678c0fb58fe4fb" },
                { "es-CL", "8ab92ed35a8f2bee18965b0db24df5b8c70b9cbf2cca9351957710a850cc6dea8c8bdae29da6743b49a6e2c9b37ea8ba27909f61958ba75f20e12121ef4dbf2d" },
                { "es-ES", "210259d7a09436d282b5613ffd92b187cd2dc9a9b5437b649564bf0962b93c705bfe0087d29845db3a97dd84127842ee80e1fdc3a060a49a447ce4c72ca65310" },
                { "es-MX", "618c41b9da8f22e81cd1a608266cf18b9467af62fb7da6d6228c1c6a90a8717f17e3f20c5cf549ba10e26752d21825d026a86f5c615af84ecded588790faaf62" },
                { "et", "70594cd720880d4ac7d7ba84d08ef729538fb036c8980997d0a5d3955f74cee2154f853da16958c3be5faf29aa65446c52066f67204940ad0ced4db69ac2d97c" },
                { "eu", "f388855255784ba06e33b29e5383ded3f457e95be532f01e06598382b3d5b274525ce10823f64a861cebd6f6e8401ea5c85fc2f1e2b59e68169c0ec50f60e371" },
                { "fa", "c99130be2edb97a31360f23404e8f6608b3635aa74ade71975de6808633963e3da8025cfdd4a9911c0413abe24f12e49d569d1f4203736dd12939e217b5c7915" },
                { "ff", "d7ef11264c7f6f2d298fe27a4b3e88c3b9ca4e144fa213633b25faa0c0059121e83e544591b38e1946f422ee8029ae6b43e9f05e280f61b3491146a50ef7ecc5" },
                { "fi", "dc5c912b6d25fdd59191dac63764ec5c4d1a908c488f6546e209dc0959711095bffd55cbc9e8a4b57b3bd6eb6f70221ea7b55951f304f791adf385caadb3dd90" },
                { "fr", "fabc069f48a4ec70c509639ab85d453e1dfe932a97c4a3d989ff8f1cd30e973dc7b9e3f5386d3ba1b472dd58f2cbba0af3f42796324be3809c8ab48e3e9fc00b" },
                { "fur", "bc895f7a09ac25b58ff82351bea1a1fe29af0489885963ee6b1a6ea825d89f012796ed694046e1d2b3dd9357414c00cfab05ca2d435ebaf0780ddc6bf3b4b712" },
                { "fy-NL", "ee7f1ad2f7317609783dc974756828f1dd0bbe84d14f13bd3773b9ac5b03599e7d7ffd43eb5af0e219f0915e01e26d80612d74293a6b6ce489fbbace168e4b25" },
                { "ga-IE", "0e2846e61c8096854565894265fa87156df2241604eaec1c0026a2c1430d90b18061411892b6838c5f9bb13e10355e7ceb292211f18b3d502cb3f061203f8222" },
                { "gd", "45d6a382d41935cef5140a7cd20f0a71b9489075e25f9425e2992d8071dbb774cdc330cc25e343185e125ba176db94ccb7b631cb8257953b331570f42434e079" },
                { "gl", "e92320c26c277452e96fa0d5bdb9b8a27000975c70348212d3d2a77b71cea90e817ada1a7d85eb63ad11dc73660429c052ba80106eda4db7ca2a2c8b817d77b4" },
                { "gn", "8f1aebbd8982249b4a568bb1da101592b4db60a63d2664eab6ecf4bb8b01b5bddbd16084d21f82a30985f318fa76ac37209ee93ad61f2da55cfc5589ba026b8c" },
                { "gu-IN", "1e904f06f04a05f1e2c99db854e821882772d085a97fbd1c280809ab4c7c8146b890db3254a9f6ec6eb983d80fce6b2668a61a7a960746a51a2097dc73bc729f" },
                { "he", "8404a832329f1d11ecfe174a465d85b290c098abf8da2601d400da7a6c58b50a8863f6b69be31b69d19c8887e1d56a08cea4c018f28550dec333b1010e04748f" },
                { "hi-IN", "c82cb1de80497b9254dfe11eb4cae32fb53f10ea3ff12d8ef37f449cad62b72312a29bff178715cf619a87a15686e964f0eed8392c7010a5f05beac2ce9da32e" },
                { "hr", "f07c9a77714776745dbc3c37c27d4d0e0bf5651cf83802f4fa70eba974977077d46678537ed83f73225f516fad15e86420b78c26184c9cf376d486919694387d" },
                { "hsb", "ae358cd26a87b96542ba4faf4ba3e8e44a1882ef268961ec24aff8b6aa2eda4e784e4160600ccfc123c5f5d54e5ca9d2a403e3d3dbb0eacbc66e143d13f7b6ea" },
                { "hu", "2eff60f317e67cdc22c148718b5f8bef9c28f9909cf92d18ff4ac8711ac88efdedb73331b5b3a245499252c5480ca553c436c4097f0bc77260d916087da24495" },
                { "hy-AM", "ef03d8c3de4a1425673ae78268117e2ff437e0b63f37a9e768b3fe1d764ba5cc53cc7655aa0eae5cfeb8b81268c9fa266483c5bf2d3ce42ce0c6afff4f794f45" },
                { "ia", "6e3d6ea1f6f8f0c1118d286e1004b1ad1e68ff2ecda5af1a49bd6768a53d6d0d39797e142218b8d30c169f2809995a41dc7de3f4ad12d1f4e38305b993fde87f" },
                { "id", "426a87fce5d80d34a9c42f8b35223ad1d888104606e4e358003fa2ad4180495d159675a28e6edf63ec80dbad5271ce8ad7f8f3ed5b7d60d73b84de1509f3ee39" },
                { "is", "a11f0b102824cce7691198c2bbc69ef4c88a9aeef4864065924c94ec5f2c6a9b27abebad49ecc5535f6aad5ed6aa7c153c4997b71f235e11dc5f5209c7372e74" },
                { "it", "ced0731c590593a7716f1030f556c4f16a4146abe50f8a0676323546b924af8a16d6d327e13c8869925b80e21dd92af6e51a6510aed9b6825ae22bd08a914c7b" },
                { "ja", "bad8ef2e0d1d4cd7b966b4918c6ae82fa24c658d03f373447f2064598ce2e95abe9b8c876956bccb1d0a6eb12240d493006294fd57d2c21f8d60d6f26eddb603" },
                { "ka", "966a1e5c0a1429a13b7adb0621f26b1f30fc6ce8466836f344097a81831882ff75655003acf1c707330a3ea6b32367f0d9e85f23b057cb016a88ed6acac49efc" },
                { "kab", "217d913d7f47581cd54c11510ded6af1e03bf2165bea928832f89b14e4eb4f452f053cf0e1f9fe6d79e72c877b278ab5572e9d9aced0357a9d6ac2c56163231c" },
                { "kk", "d0fef48d9b975e1b8b548336115702b741defdbd6dd1ec53d0cc6b74fe1b521fb2bc967bd46b73c7511f01355abb2feef740ff47a09feb309f7ef8b3be7e600b" },
                { "km", "df675813733c130387448d955bc0067fa696deec19be49304c9a50f992b3ce03e54e606a337966edf7c1bc7a614d4a18bda8f98439f4b6367f144880f55fc158" },
                { "kn", "80666e9d08c403dbc29e03e14127cd5e3a469ef615071043c7b7b9a0eeb62f15d7ef20cc83a06114d282ee2d44641de1abc27fb18671446dc3b95304e06f58d6" },
                { "ko", "f3d80c4097854e4c05914fb6a4f186dbfd27eb89110b41313805a3eb1c93b92c5658f755fcd8ccbd523c5c01a71b275ea30cb6495c40b1406f2e10fb5eb44a0d" },
                { "lij", "4da111e9214a3eaee8f6d45a3edc1f4f99c101645efb6b3b3682ee2d06b7fcdcc5a0d7fb5d7eb8166ad288f6ac1afbf5fd6d3451e7d78fe222eb968a42622023" },
                { "lt", "91a5940e5e71c904c3296095ec99cc93ec1eb05d9347cd18b0ffe1b0a6f75f611e7247b2e2c70a9c8cda45bf7ac7e0e7bbf9b15301708309f9c778143ac64bb0" },
                { "lv", "09a9442d31b556d31ad13e4d2d1699428bc227da1995b2dd4c92931e109865ac89b3a38f22decfc619b14870c11835964a61f468d038f31a3dba0ddaec9dcd6a" },
                { "mk", "569045251d3254be0b428d94aadc8970e56a8556c2f5ca2514ba40bfc504d3b51941957a7f7aabb0ac5c70d726530134baab38d72f13f380eb0db487bade3b08" },
                { "mr", "ac54fd381285c6a70fe3201d04a98023d5044f206ed7a94ccd2891dc22fde595a27c1958ec98697613d8677e7a5ac546252400f365757ad3b4667432ea2f8b5c" },
                { "ms", "22a6e59956378c4b326adbf56fbc37545db32016a83269c2b9a300b139024be1e3a178469cb4adaa717fbd7380f986e611da82554db8e2a2a490c4456381b09c" },
                { "my", "e75f1ab89317f0d9f26a7cb78e9fd0b052663db79fbcfaa2da7acb7feff542ed9bade3df033029e77f3ffdc4da5df9efd3319311354effa40d959c60105b31e4" },
                { "nb-NO", "f027c0e4f35bdffe56b45745bb0928b59fe517806fbf3f35b3fdc15fa24620dca9082beeae0af4e33b33eebc38d4d6c54bc748bbb2d9e74f7dc9e90677dd51be" },
                { "ne-NP", "7d3787cb032f75c76a0c150410d1b8692d100460d323ffa6970f2ca425b3462b9863c2d06372dc9ceb1aa3811a05adc3569f3c2873ab99482af38d074e2a696e" },
                { "nl", "5305764b2d63e23688c3948f0055153d769c8f915e70fa54303b070e767ce3f92d418149cb38dd2f174dcc1acc076314f8cbe3da2a80a26544b0ea407edc4f3a" },
                { "nn-NO", "8947f24f7cd9cf9804c1a8a539c355ff8d60d1138da7374a8e0c90e85aaecea0628240d1127e74a84e0109d97e4d292c9508b4835e19a6456abe90ef327845cb" },
                { "oc", "ef03ae57837d697eb87f66a20a7b84cf13b2c63739a70f33c0e3d3cf6708571b313120f55e5d114e26a7745e4d833a8a5a89a08c42b7bd2d011efd36a7f0e055" },
                { "pa-IN", "53df198cecf0c9f27ce51d081b6ad20e77f0f71cebfc5ca740f87d919ffee9aa5e26ef1fcc624b1cba1589b0faa449ec07759248090579eebfa2e62392039a9b" },
                { "pl", "431008a2dda7132ea68c4766e46ea4aad6e2e300afd45d9f3b11e67726a7a59e039e926d76f6a38da6ac784bea299b4a056c0a3d60a435d2d04ee19cd707b884" },
                { "pt-BR", "b65f29a84852ae5d0463ec7ecfe6aec5fda879ffaf85dcc658fa99c8ef54e6fb0bbf6a1fcccbb60966c9233ca4c7d3c09c87b062628de242f1024f27db3a6fd7" },
                { "pt-PT", "92f3642ee1d8faec4d5f2501f642de7ad6dea3bca4112f6adc7fc45c1c1f55b27ff695e78b20de91c476f4644cf64b93902cf7fcd60a2b326dc0f3f0d4135748" },
                { "rm", "2e53da282c7794c322353900588f4883a1202c7b4b62686115302afc4adc05624b5e5a2b08484724893ac99eefede98e72e5cac47cb093730a4d8c7c9c6841ab" },
                { "ro", "8994bf6350d8db2e48bd640fae85afd0eb74a89a5b1e9ad54d9054b73578ee75bcff3001f648d83ddb10f30db85a2362cff5c3a1e50c02156d35ae4e191f160b" },
                { "ru", "c4a234cf167bb9211cf2acb0689000e57e9a7dbc76281cc5ea9ade59c5f097b04797710bcfaa141eb1ef951489b174e63b14cc94a4eb5d3a301d7b2903325dfc" },
                { "sat", "8356fc930f76eb36523d2abf19e23a56ef9f4f2fb35453ff4cfd99105c19ff4bc15a8737482dfd9cdf986658143dcfddcdbc9ac439e314c15d7c0b9452e4ef1f" },
                { "sc", "7b0c6cd70115dfd6c77932aebba43f726efe87c65dc8ab1107f4980f5c4ea5a2d6362ae350fdf0ae4cb3a3f40de01a069d09b1b080e954537053a300466e2d1a" },
                { "sco", "421f3aff0673a91241fc73a7502e1c633e7cb0888d531166e515997f4d4fce78c1e511dda8f7576ac0ef48f29d0df802c5fe4b0b392d709421f85c21ad504480" },
                { "si", "42dca50c34e99d10723336c2f2e7e03cedc6f7f2bed343777598858dc5140bb9e9661413ba38d994f7952e625bbacf5512894578d4f573b7b7e30d282e950d78" },
                { "sk", "43eb9241643d6734d023a24ed9fce534a5a443eeba06367117743e55e7110be3bd028cd28f12a22788feef5910a8142f0f57d7157375c58823d43896e7fcca2e" },
                { "sl", "1df02739ab47f48c2c4268945093e3acb79f8e27f921fd2495ac5911bd81dce4f2bd55a360b5c8414042ccacc97de03bfd70cbf476adf5998086526377a3b977" },
                { "son", "bdb368b36c9f2021eac6211f9ee92bef4dc85b2d3793a679d886d53b032848f4282ea4b2d416c0a3b70c61edc146c4de8cb2b3493c8ef9e0f2868bd2bf237d2a" },
                { "sq", "48249ff7938c353493b8d796b13ac4cb141fef3ca1e193618d1763d13a18a2bbc2978340d3a59015b0deeaa81f0816c6e0302d1cb8415ceb87d83a6769476bfc" },
                { "sr", "1f3f21d9411cdc409966d7465c71652c88c4523aa2ff08d1a61a42030f648e9c198161e870a3e5412c04cecbb804b47614c3528b6fe0648a292b970146d8a6c5" },
                { "sv-SE", "4de3b03e3983167ef255ea25e5897ad442459af9b48825bd8d0290d9942c7fc6403f48126cbb1c9f231b30de3da684cbb2e476f91c86e427d337e3635e806dc4" },
                { "szl", "042f508c7f62478bb33b315dda52eaed251355f1df793e617366a1df6a138965423643518deae2cb43fd0802318e696869174577056e03678f73189d96787794" },
                { "ta", "f2acbd4b65c60b6123c41c906cd1dbaa8a83a502b6b65414aafabfdddea3fa95fa1816d492b338f1877f15e9e49c009112f1e1ba900840d80b260665e9b901f4" },
                { "te", "466817a770c9f87e4c1f4bd629169223bc606567ac653217a18eccdebe1ae7f46bec56e032fe2351603c203d1cbb86c64371223acdeb4d09b4570c7fdab8d58d" },
                { "tg", "d4f7fa0d485657fe6e6e77d843e6dfb4ca4f05fccc307dad9f12a942a7dc6bd2be471ff640b58948f46af0bafe28e52537767bf2b08d598e1a08716c8e3cc90f" },
                { "th", "79a561d33d215b5ab8a175acd0a623e4ad222ec44eadbdff72ed45db2d250f483fddad53e59a5573390b40972a0e92b6ec560cd37f3fff1e18acc6a0f8f27a95" },
                { "tl", "d8702c2cca3582e618ae8ab124a9526f127f82883a75784a1f7c1efe9c181409cff6368001a87832f1ad3dd7917e39dd2a34c34412ed99155439e10cf89644fe" },
                { "tr", "fe3d16d14a4ae12db55bb33455a96c8a8fa4fe26e78e1f2ddb5160db7e6f586d971d5322215a2fa4732d1bcf540605befccbe701f409cf3036aaf680ca2ecba1" },
                { "trs", "50f108a1bf8c12ae7976f2b9c60d5fa84e49e2b18238cbffd9ecb4d7408a490ef575e926e626158e6ff182a57d888a634293fe39f03868ae4d9ced5df18981c6" },
                { "uk", "045ed24f2c3e09b23a0bfb29715a032272aa293a216bc076dccf0670b0825a439854db70fed1ce757296b382fad9331f3c11c0d359944616e3e6d86db062176a" },
                { "ur", "6b5ee44dba0f71881c2a5e6aac6fbcec039803d9104a7efc993683acea03b73f0288a677c5ae6731023bbe1139016a96b53921dd31a37b353a0eab77a606879c" },
                { "uz", "55615746bee3a4809dc5ba9e1e52b46e033512e5a192d07fcd87ab1422220e36dd6d678e768ad599a119bff43ad652012925a9e2d9de762f73f2f64132a485d3" },
                { "vi", "2bb6a7bf6e4fe89fa153978bcb2b505c442c0b4b6fb1e7b72435047267562bb04a3d1da3c2b152b31c679860d4797655942f94e9ec96bde7dcb326f5b434b312" },
                { "xh", "840912b5be448de005d51cc6956c937c82280bc63d39417c819e58fbe2e267bcbfa287e4bb6bb36e08cff8d2079f3d6c916cb32e7e08b842a29a660de7185e20" },
                { "zh-CN", "8a36482b18d671c5bd114761c0ecfce01557c56efe94ea79c4ec63332d218d482246d8cb457f9fdd6fe54edfdb36e534bcce4c6fca8f548f52f885fb9ad5e6c2" },
                { "zh-TW", "48d3693352f665c58fce41c52dd82eda03a98f31cc61e52b7dea0bc939a6d41422ad887e06c763b6b4527cada6c221c14b8cebc6e200b47c658c8f83b2bc7fc2" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/123.0.1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "122cdcc2111f7c7330d47b73454a6c27a517accd5d79d61b7cf7f5714e51ce73c9638dd035d0afbd1797e4b6e974e4762f6d4940c9e3900c5b6d0cd3e1741da7" },
                { "af", "73a89d36f7e213ee69eac718a67068c06d6941765ac93bc4c13b9d37b9906c2e650fe2dc54aeed2bf38ee06060fc55a77f9ba7f88b2e8a4c245f9fce7b92add3" },
                { "an", "8fb232da533b210a5f65a3d5c3b319a7192c062d4f7be18147dbce5833c52221a61c72c5acffb0b5d230568c418dc76c1d3b14e91a79af06a1d1393cb7805777" },
                { "ar", "cf60b311c24390bc14a2196bc6fe7a032ec01723c9aea3dce7baed21b97f3614c61c159cc0f33f33470b509ac067791f5fd25abe9610383372dd22657b9f1273" },
                { "ast", "542600d8649347428f5d5849e89298f0ae5f5b96a47d18009f382b1144ede1b1ddf27d44f77b17fccfe2caa6fd3aeaa28ab66d201c60fdb7818e66029155ec91" },
                { "az", "12efae8712b8e69c4da8522ee3c8ea2b67b9c7b5ad15b377aae1a966193ff0e69f23dd0d1745052ea05f15b6b4f1b40da64636bdf5f6e1a991aa6bd53fbb01f7" },
                { "be", "57e2c181a54a002e879aa3d48a56acaeb0982875defde78a5d7915ad73ec35e2b00149d897dcd9959fe004d5f8d84f286dc0c48d6e3fe9f97ce4a4e7fbe4d8d0" },
                { "bg", "3840b46bf8a512e356eb79cce72467e26bf8ff74f3fd6bfc6f4497df71071b1e042bb59732e12d83ee9400ebc391dfe237107df547921d9a22d56c956a5ac9b1" },
                { "bn", "e05edc3a998b600ec9c04f7bee6a53530b6ae11bf491bd77acf308dbff933e2f98582c98975901cd23e31069527e8a1a88dcb0a6704c200841ae373351a1fa9e" },
                { "br", "5689e45a985b6bb46d36ac4ad24fa03d9f52583247f4a9e9bae80df7152eb8ed4b28107f2a968df30a9ae97efcea6aa7e8789cd3f8e884967a30fdd219857f93" },
                { "bs", "f6639eece3d3d82a031b75dc736736f39fa492580f9c6e1c79cb924cdf3d69c43cc9a6081c000e25264c9dbe9045e41b4e793645951999519c163a6936d8e17c" },
                { "ca", "df14a12dc28403ec3b3a636a7debd1a2779df1c59422e9c651b30941bc29f1620af4402d9730344ba8c06d5f69f1a056d7c067e4e0849a5d4881fb4b04a6c4de" },
                { "cak", "ed38b185e620df3036eb619c5718bd2035518260d57534bc7f0fa45d797aeec4b3d4593b9175ee012fc1f33f4acdbbf1bf9959b69fb7497f6b9587c5f0ca6c15" },
                { "cs", "aa9e58337614a9a82b7f805c514dfecfd73e8a5c5686e191c1b7ec23e8c8ab3470a689900aff37a18944319ba707ea4742d324fc0ea734b6b5d6208832bb8dda" },
                { "cy", "3ed4db665c716dab5bfa15bd9ccf8f7dd275b4fc53396cfcb170360c756cd78755d956deb96c02d34adba450860a239acf337657222cbb97eb35784f23764759" },
                { "da", "951b03210c8238e98608d6c9fe1ef50e8614774af371ce552e946adc481940509741e97b8fd9b5b022e0faa3410b93810826064ad456b704ec1b65e580e5da31" },
                { "de", "b4f5ed1648238da59a89be5a824d6cee3d7ac5f01505552827f1d0f9c1001a337e3c7c0d6568b424a0ce8a158a89e1d3955cdc90f0ee034083166efdfb01fd20" },
                { "dsb", "3b3a0d5e43165af0548b337bc7662cf42c6f28629a219208037176ee53e50a2cf3f8e089e60952db58d876e89021d229114fc9eb5933281aed36e0d6ba8a6b07" },
                { "el", "5fd23a64b94a9a6d85f9ac121a650eb61f3f7e4f4497e92b788848d501ab51761e97e5835131457e1df4b4586b03952f9688b2d806c326f35342d4f832b58644" },
                { "en-CA", "3378d60cbe5ad6613a1c7732682ae4dbf0bca41676a3ff5951b05532733aaffa3e98546b7fe1cfc74831905aa5fd3be88f4c1c18d1d991675f582a5ba6273f35" },
                { "en-GB", "199c8ba902d1e26bfe35b834b11f3d1b0ceccc4c964bfc2add5a38e1d03bb2cdb919349eeaa521e6c014af49464e34ebf7bc3585bba2c14833e91fe0dd7e60fa" },
                { "en-US", "df806b8f4f05cf1802c23551b7daa22f18c92ec8730126fbc0d39299d1607072a2dd0dcd060a770d5c6c3ee7d762594237ef4ec2846a8371425007626f86ecb3" },
                { "eo", "068db974bac24c80efc64ec01aeb94a825c646293c31b0bf8238115692f9d5c40b419cf8620f597cdf11fca63367f8f84ac97b82eefb849e549b80b985890a27" },
                { "es-AR", "bdfec35ce5279073463cdfad7bfa806be5e2583c761b649f1ab61b9ae05c3512d7a09d70bdfb94b03c9d4bdf633f0a37c41df03965c3ffe9143cc4e4ccb302cd" },
                { "es-CL", "7b95c375fd01c5e2401b1d3d575b283f49089d3b2dc17c49a6985e1a383e6c56758c3ae63079dd2a0f689621178df08ed041b0f42e39185d0afc6d4c56fc0461" },
                { "es-ES", "94e623872a96e1b70af86521644cccb0a7f10b6aa4c2f9dca392a4f2de83a61421e71c099bf8fc20ee2a386b2fb454e6db6138c1e7e38d6c778684c5f52fb7af" },
                { "es-MX", "0f5c0e794821b212d4719f13683713240d18ee7c20a6c67489e999a5cb5dd241828d710b6dabb578125caf53c224e291ada5775367c131cabe6d37ea1540dfe6" },
                { "et", "5a3e0ca63dd66b56bcb1209715a830f340780cdac5412b1e9b6102c77b94cf0330f6f1fe2c9652ef6accfce7d6af10b23b35f9afb99d2440f87ad1dc0feef07d" },
                { "eu", "8a422b1d7a501822c8012822207ac8f58b0136c2371136973938167c96f09a69f07835a177aab570677f8adc981f087084c598c21c589432b36f111a1bb3bfb9" },
                { "fa", "fdfc7685d6a37dcaef97ec8090aa4b63d6025aec951fc60846c344b4495b35ce47c9191870f3bcb4ba56fee5eadcb234752db4aae19d130a87f49ac913f78684" },
                { "ff", "72873e51d122bfb90e3ca82aef1e231bbed0af834e0988d47185ca7d17640d892885ca8d6006c9991645009ac53beca00702325fb1522bf6e8c230a40bf9ce34" },
                { "fi", "69269704e8cc46c369c5a75c464d5eb4a51f80860af880557e14ea0b771793aedb8bbbd6f003f647e1dc8da007821298f1b0b58ceeb4a70e5bd6f6757acc898f" },
                { "fr", "779ea8acc647592d54cf28d443550225d453d71fbc964979a9e0ede0b800a422999e40a6ccbcdf92f50e7ec9e6e9d8bed26a959ce2e3623bb2dcf25d9f98d6e0" },
                { "fur", "1ae2b87fa1eca38eb03daa6bc1454e79d3e7b8fa5517dc2ffa451d34418bc0e05457c28d2af3c062bf6dd36dd718058a86d5c01765b48935babfb2e5ab04b204" },
                { "fy-NL", "88972a77f8433f6a8418d59ce47ab7bff7c85831fbd76e07bd658bbbb37c704ddcd0d4a7e507ef539333a2f3af85e23e38cbb0b6e65ff4153769df1e54122238" },
                { "ga-IE", "1686e041330942ee6028b6a0bbbd146928fcd312339fc4a43ff6aa485971bc1f91653b09f5e2c19cb988cd426a5b7b96a1b545429083da859b3634dadbf3c14d" },
                { "gd", "6fc493f0bc117d660018d1595da840ec835982cd70b56c91336fdaa0daf78dbbb51d3c66959f77a72b93f9733f7940ddbb28b53f6f4a701aff2f408c20f885ac" },
                { "gl", "14939adcb3431dd6afb0e468ef95672ffd08de6bc6c5929128f63f73658c5c02413884b5bf6140aa374351915f046b5ebb5f7ffcf8fdf29b89e5ebd66db09356" },
                { "gn", "a91c1de4ed394ac5deabd6118aff41048138f61e367befb0a47de2c260a913c36c52aec9d01477a7f0fc5d46acfe9740974179be3b4055a2aabf9bc8d738dd8f" },
                { "gu-IN", "52b94e709a4ae0bf0cad350efffd03d00e21cb277f459768b4b5f7f80a0ff9c416e414b9fdd49d2e1b44f4b8213374bf7a8025a6c9697b9bef00a953897d0928" },
                { "he", "8df2a715314647eb57dd17c24c51595f5cc15b945aa922cb535621159a167265a6b77863e63113b6fb530f8feb14952a8b8fbf735c877f6262dfc5fab73e14a0" },
                { "hi-IN", "86e3b0d4c13c4aafe42d665b9614196042d25a137fff19989cb3d43999a1291241ded4bed844f6c3a7a5b3ad0c1853385b5d177e4608bcfab2c1e3c977c340de" },
                { "hr", "9737a13e97451d2b735afe82582c4f310d91c978b5ec338bb3a2701234a19da4af40f7f26a0bb908bb14172149561041e540b313608310fbcef98ff04a588ad8" },
                { "hsb", "45899b946364c6136d2d39352052ebb1d032d67b76218c3e3557e7c0d66c99150430c42a51c0e6537d6df9bebfc194d6eb0f551c07e65858737b2980f08be2ff" },
                { "hu", "71b148963a0d3ade514ba15961b71f068c0578e970a609a743c18ac73f23241d17cdae3c2f3743f4cfbf71e450e96abc722fe5a8c34e5b964c7b97617c484414" },
                { "hy-AM", "a46f4efb16eb9776661c3230dca0df2b489ee3ce07c3b835a51c748c961d90f9a52aeb8512cf6bf3ed777621eab97c0cc4575119ee5a169a84cff0a9baec8978" },
                { "ia", "bfeddad1ce04903c27629f03cb30aebdda55186260a476093c0637ef96693effe3a4bdcfc40f682d3eaca0d5d5a6d37eff3eed149c8b05e1797190468f8bad54" },
                { "id", "db9f462f94d6bb8ff8e9022d5e759d44db8864587408d3506e677afae95c4ccdcb2b921ebaca1fb6a0efd62f54bcca47a596f3912170bd8d5d07b883ba9f75f6" },
                { "is", "2430a8f6835e223fb441f0aef40b57033b59b6300126742c29ba36a5c521161d5619b3f6eb9c84e08c69d96bab2b31bbd016052ee138cd084e9953d2ba34ff97" },
                { "it", "4395715ad7a172ff1bec7d0027dd986c6227293998f9c3b362f9ff1ef3573e5f6c40a85a71698f943aa9529293e37f14562ec146449d31989745d832c18f36ba" },
                { "ja", "5c80872346f05aa5b1a447b2e0677d615713a31856f1348a36b424366464a96a0ca8456eadc16d6a86dfb8d39216a626cbb43eb05456be68e888877d7f91b78e" },
                { "ka", "4303a4d7712e6d4b95df579eecab8fcf34bfa0eb062b7a77628dfe771a4835a0fbcbef446c6857f1d38b4dbcffb62cad040ee18a3c1ba44fd005d5edd0925425" },
                { "kab", "2ee10509eb4f0623819e853a5b60d11d42b1df5b51e7e49f96ec469bf073f3c927745aed49991af2ed21db9b13209364beff5f9d23015aab651e4225bc65103a" },
                { "kk", "4177aa9902465911ae45009a0f4581f6494f9a8c1ef6c49f2d0262f95f67a5787231ea5797241da7470ae046445b9ec79349faea4faaa2da2d885f5f4822d26c" },
                { "km", "5fc52db2c8e531cbd976f4e5940440d05393e0da5576cd9b4c8b281449ba4312093fd48220e414764348ba276bec7d011eb0ea2edc4545d02a36d2ce1119b095" },
                { "kn", "e8232ff83c1ea6b64d33bcc5890de99ebae7adf3782f25a85952d453e4d432d4f210df00cfcfdd675dedbf69c0376185113e307e5cb8d4738b82fbba901baa56" },
                { "ko", "f8acef61227797e743598b882d335a780219d8693d34aae5a5d5c1afaff91227434aaaf6ec24e43046451ce9c657f06437f5c6970110d1ded92bd4290be8ee68" },
                { "lij", "31f960cc78ea30d0a7eda9d4a438097b2246bba71565e71a1144103d7db6c136a3812ebb52b1d23b3bd6abc0935e92543183100ac99bcf43cdecb868b81310cb" },
                { "lt", "0d3d85d1c05694bb1621c4a42b425088afb490e00a5692e961efbed5dd4b9c1cdf37d70605610bc08c51ad2ecd5251658b263161a6a1bfcee130e36222a7323b" },
                { "lv", "40169346a5959e779544e53df34a4c85713465447bdce0f829dc02c7eafeb2557d3815e441ac490917e137da9e8542dfff0bcc3c0b0c659a2fef597fd2ab51e7" },
                { "mk", "dcf1bb2952cc9eda9f018ab3a49aee4907781ff4c44b003aa5b2cd8916afdb02f24c85e3edd8985f99bf71d10afc8c6885f1b28739948ce78fc48cda5215c0e3" },
                { "mr", "264312fac5a6e74df8193f0d1400093d0495f85612d508c9384be9ff0f4b841c59f0e39b7d4cc036a2838455d7a97a0ebefcb6736b9200f90baa97bd4f4f3584" },
                { "ms", "18267992dce8f16f3ccba6b9a823c7827a7fb2687706eb10bc7a50747a1795976a876d5fd27a952972216e790a7071f26afa69b423a8731b35646d1391c947e9" },
                { "my", "a016990c62e527991bdaf44d2010d81c6cf2b5bb25cd869edc1e2c94aa3c39362209c7ba001763d3c1fbf9f54b5be132e35221e26c9168cd0a26e92a11fe0209" },
                { "nb-NO", "a6e73e86de7d81668b2278ae9fa9493ad5be6a78c937cadd8a1041dcf71fcfaef609ade8ad64b4b0266561a58658cae2bed3257c48907058b7953a96db4ce36a" },
                { "ne-NP", "612354073a62064ce28917dee332297fbfa15d2b45698966526933d75731a04fb740d04ab30ed5ea90dd58fda84440f59e14618cd19f8a45d3de93a0d0babfc0" },
                { "nl", "d8975d5ab52af74a6f1406deffb42d9818449ce5e29d6da22c5307c23bec09960f9eccc0ea3a7558ad0b73acffb412e7eaf62aebe98dd5dbd725c915f6009c66" },
                { "nn-NO", "29d3f6f3d051b07409907277ce679e2db32f030017ebd77b51258ff0e31c4b12caf1d634e7adce9d11f8df76439dcf26ff7b4ae1f5dd4e8870a6868b13989b7d" },
                { "oc", "7c18cc4da12fd70a975245bccefaaff6099aceeb3685e7e83be2addd0f4e0e15375e53563f63ffe8f5d82a87f7907570822ce355a6a07d9ad1310ac186c6a806" },
                { "pa-IN", "6783ea520d03c949ab5bdb732109f7cb5ce39709855c019fb6e305914a229cefc713b7f39bacc623bdbaf3fc8d1faa0284b872245ccc5fbf7a2e33864412d5ec" },
                { "pl", "20d42fed3043da5a31be8bb69f31527425206bf35e6c4fd10011680a76f6c4a5a1d64762833608b2143d5822ab5eed3fec8c06c37504d83e9af5de5fe77c4808" },
                { "pt-BR", "14de915fb7fee5eab6f5f7489343d57ab5556decefc968cddee039c87bf60adb095196d6b25d0b43b4a062d670f89585f8a0db8a48abfd0dc862d559aaba685b" },
                { "pt-PT", "de5365d7d2988bc8396db2eec7da424d438aba4d39854a02e36567d8678a89f4be46688f591ea55d75cf42fdd286c7b85ef8e4206b40a551da535fbb3338677d" },
                { "rm", "593bf587e09f60fb02da7deeea4f508d3175bf00a97f4cb0197ccb169d5718fcadfa70be1eece12b280b24c2913feb97ab23a6d492a345f98acc2e984ac9504e" },
                { "ro", "b310fb064d587607cae8fcc9579a931b54977dc47e24553d3dfceda917268a8950bd089402e2d4ccf8000eca8f517946773124d52d4212b3fb1bd9e279a7e935" },
                { "ru", "df026345dece5a0ef6300213affbd76d9bf2e6ae04cacfc3e0bd156b7d6aa79f07d3fd6c6b3aae08600850ffd9545be5726141618e2000352ac5f593dd8e5d5c" },
                { "sat", "54e3fe6b05b11d7d83153bfee8432fbde3187d9367cb29d4c4a881a3b5792c1c958cf0eed22bd2399f13cc22f9b5e760c35b8f4abb0618854b9a795975f85238" },
                { "sc", "564c140ed38977ed0a6b7f5b27a39b94e8c2e7c29b004072c8eaa7106b883d2f92d6b1b892ddbc9d087f7b8e73ca792b642c19f23f11822becded3ba5d66da48" },
                { "sco", "1bc176d9d954b2014caa1089931ed3f83da5d202273dd48c9d4d8bf4cd0515d56620cdfaff6cb461e91776098a6c112973445b40524cce0b109e26e834f2c7de" },
                { "si", "2122c8e6312b7986c3497bfd5e9be5727ac55c1be0e4441b68326becc70b236cd85921894b383e3eff18d3f8e9fc3c29d05c519c93cca15ef359567b3818c58b" },
                { "sk", "07cddfe1399494bc0cd03ffeba5dfb997a264adbf1873a12d1fa6eb812e2e37426a69326f76509a1de5081175904c2af0b598dcad99ba5f5c56f262921090ae5" },
                { "sl", "a770d0905ea8476b12fc98b09f8fb5be4d725e923a4b052ccd374288999f3214e21d91ded792c2aafe6e2a2e5e729477a0aa9396e7f1659963a9f747e2b9999f" },
                { "son", "0ca2fef08ea0ef8c98c561bf7b4a75b0ae856ede40d85263dd1e6ea9c9252c98080f1df50a4f47017a010cac6b194a604bfb0dac5c4f21f0ae0051c795d32179" },
                { "sq", "bc653d082ff6bd2c4eb3a6f50a11105ff9b691f210ce3a0798b678fff6d7481ef8355ffe45a0ea5d784bccacb489dc5a2a5d9a40b761038328d2e3469034e26b" },
                { "sr", "610402feeacbade1a2373d30aebfbeee86c1fc68d8ec324c17a55f26d8ee96b3d23d7150226dd91cd18bea5943fe28232abd757c047b0ee84262711aaf6f71f4" },
                { "sv-SE", "8a35bde8fbbaa1979a70122a20f6227074ba2341fa9a2a769786f7f093531fa624f4d51b071bea17f2fd1f080ce7361491e901459950870eac6024885cd53d8b" },
                { "szl", "7f8d8061b7e15f02c32f6359948378462426ba4b9b847576fbb54d2111b4a726a69f159e2286847fe97232fffb05103c9e54b4797dc9ab079292e3a6e00a0fc0" },
                { "ta", "468c8f34810b86a024e52d98b4fc84f1004cf25297651e0905356bd103cd80f64140f9840c646d4e8b718d08a3f080c0549b0e0174809b27df7c292b4dab3768" },
                { "te", "7df139c792b60e1dd2babbcd58a9944efba4d5535d4ccafd7fd0a718d6720c10616fef3bc27fb431436ad4fa38ba0fb71dc0e95abadcd5b5476c424c9407ca92" },
                { "tg", "e546fc0bef0d5abe5025838e14a814a25ca6bce943e520c3248f467acb989e8f2e117242ba8aa9e822091e4d288e173b3957bd333f4a1b350cb413583e351ad2" },
                { "th", "4f009ca9dbd6160683ccf1c9868e94c216b0118befb6821ce52205ed100eb8228557aecb605d927d2442bd8af31661abcfa7d77c457c17c594b7a2fc6c3b84b7" },
                { "tl", "68b140f9b5986b5ebf27f3c932f3bc2e65b035024308799eff328f285476ab3361bc1966af324dcee08fda64aa62e64169bcd804afda5d09e0f7d07a82ca9efa" },
                { "tr", "ee288c01070348dacbb3ae521034985f1309a2d21c399e25d90448f8379b3db47d03cd87bc1aeeb52acee7e9cba8b68839d7f987ed135aa144ffff7e13d12f02" },
                { "trs", "1b32baaa03aef2c6502714626f95644956ffc6d20977efe24ed398c73138bfaa41604c32d0abb6beb75a9c8004189be434b782c836de8c1aec4e87be2c5bed7f" },
                { "uk", "1610288e642087f256b5f858c260778fc130153c75ebca214a92bbb6ea3277c944c73dbb0b27ec8c98b8519f911768d8c8aa1866f2d72a53a47ae1e597c04484" },
                { "ur", "72d4a2cec2b8a48c714b4beda940a8772e765aa4c3b32d8a60e3f85a7f290c508c04629698a254a7525da38eb74f31a8e2f3367a3a0ace90db7fe3a82ddee0bb" },
                { "uz", "36fdbfc62dd12c7dfc69c32c50910322d8932fe4de31c278fee9a59dd897178d51eb2236f0c66a5dd7380c131978b2be7f11253a7724bcf8ac02b3e02c3e5040" },
                { "vi", "5d7228501024f141df24c7376779223a07aa1c8ed0ff25e2684b0642ffc1a8ed2307afe429a86558a8a51236534e9bd1d34ba9228e7551d14e4fb25fe66b6643" },
                { "xh", "b49f28d37d44949faedd1ca1d130bc749121896b25cc4ecd5772d0a1df128bf83211e6798b4ce762f61e21f7021b5568aabd1b982500d68dd6b200a5fa208cd9" },
                { "zh-CN", "dba52c6fc461d84111983fc8b966b653a1e34105c35517c893bbe456bfaebfb487dee26c65b9261ddee0aa360d73e5dc26b45d1cd9b355b47d4553e94a6d5e4c" },
                { "zh-TW", "edd20fd7470b5d5892d1995fdd7189f1571893f60f57e64808c8dabcf472733fcb4c7852ac32f0ce9756d3056cafce811dad7228d7c5313f45f3be61ddf10448" }
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
            const string knownVersion = "123.0.1";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
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
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
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
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
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

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
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
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox ESR version
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
